using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SAModel.SAEditorCommon.UI;
using SAModel.SAEditorCommon.ProjectManagement;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
		public bool OpenNewProject()
		{
			if (isStageLoaded)
			{
				if (SavePrompt() == DialogResult.Cancel)
					return false;
			}

			using (OpenFileDialog openFileDialog1 = new OpenFileDialog() { Filter = "Project File (*.sap)|*.sap", RestoreDirectory = true })
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					LoadTemplate(ProjectFunctions.openProjectFileString(openFileDialog1.FileName));
					UpdateMRUList(openFileDialog1.FileName);
					return true;
				}
			return false;
		}

		private void LoadSalvlIni(IniDataSALVL ini)
		{
			isStageLoaded = false;

			// Delete old object defition DLL files
			if (File.Exists(Path.Combine(modFolder, "dllcache", "DELETE")))
			{
				log.Add("Deleting old object definitions at: " + Path.Combine(modFolder, "dllcache"));
				try
				{
					Directory.Delete(Path.Combine(modFolder, "dllcache"), true);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error deleting old object definitions:\n" + ex.ToString(), "SALVL Error");
					log.Add("Error deleting old object definitions:\n" + ex.ToString());
				}
				log.WriteLog();
			}

			// Load level names
			levelNames = new Dictionary<string, List<string>>();

			foreach (KeyValuePair<string, IniLevelData> item in ini.Levels)
			{
				if (string.IsNullOrEmpty(item.Key))
					continue;

				string[] split = item.Key.Split('\\');

				for (int i = 0; i < split.Length; i++)
				{
					// If the key doesn't exist (e.g Action Stages), initialize the list
					if (!levelNames.ContainsKey(split[0]))
						levelNames[split[0]] = new List<string>();

					// Then add the stage name (e.g Emerald Coast 1)
					if (i > 0)
						levelNames[split[0]].Add(split[i]);
				}
			}

			// Set up the Change Level menu...
			PopulateLevelMenu(changeLevelToolStripMenuItem, levelNames);

			// File menu -> Change Level
			changeLevelToolStripMenuItem.Enabled = true;

			// Load stage lights
			stageLightList = null;
			string stageLightPath = Path.Combine(modFolder, "Misc", "Stage Lights.ini");
			if (File.Exists(stageLightPath))
				stageLightList = SADXStageLightDataList.Load(stageLightPath);
			else
				log.Add("Stage Lights file not found: " + stageLightPath);
			// Load character lights
			characterLightList = null;
			string charLightPath = Path.Combine(modFolder, "Misc", "Character Lights.ini");
			if (File.Exists(charLightPath))
				characterLightList = LSPaletteDataList.Load(charLightPath);
			else
				log.Add("Character Lights file not found: " + charLightPath);
			lightsEditorToolStripMenuItem.Enabled = (stageLightList != null && characterLightList != null);
			log.WriteLog();
		}

		private void OpenAnyFile(string filename)
		{
			switch (Path.GetExtension(filename.ToLowerInvariant()))
			{
				case ".sa1lvl":
				case ".sa2lvl":
				case ".sa2blvl":
					LoadLandtable(filename);
					UpdateMRUList(Path.GetFullPath(filename));
					unsaved = false;
					break;
				case ".sap":
					LoadTemplate(ProjectFunctions.openProjectFileString(filename));
					UpdateMRUList(Path.GetFullPath(filename));
					break;
				default:
					MessageBox.Show(this, "Unsupported file extension: " + Path.GetExtension(filename) + ".", "SALVL Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					break;
			}
		}

		private void LoadTemplate(Templates.ProjectTemplate projFile)
		{

			if (!File.Exists(Path.Combine(projFile.GameInfo.ProjectFolder, "sadxlvl.ini")) && !File.Exists(Path.Combine(projFile.GameInfo.ProjectFolder, "sa2lvl.ini")))
			{
				MessageBox.Show(this, "This project does not have an INI file for SALVL.", "SALVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (projFile.GameInfo.GameName == "SA2PC")
			{
				salvlini = IniDataSALVL.Load(Path.Combine(projFile.GameInfo.ProjectFolder, "sa2lvl.ini"));
				salvlini.IsSA2 = true;
				Set_SADXOptionsVisible(false);
			}
			else
			{
				salvlini = IniDataSALVL.Load(Path.Combine(projFile.GameInfo.ProjectFolder, "sadxlvl.ini"));
				salvlini.IsSA2 = false;
				Set_SADXOptionsVisible(true);
			}


			systemFallback = Path.Combine(ProjectFunctions.GetGamePath(projFile.GameInfo.GameName), salvlini.SystemPath); // To get a path like "SADX\system" or "SA1\SONICADV"
			modFolder = projFile.GameInfo.ProjectFolder;
			modSystemFolder = Path.Combine(modFolder, salvlini.SystemPath);
			//MessageBox.Show("Fallback: " + systemFallback + "\n Mod: " + modFolder);
			LoadSalvlIni(salvlini);
			ShowLevelSelect();
		}

		private void LoadStage(string id)
		{
			UseWaitCursor = true;
			Enabled = false;
			if (osd != null) osd.ClearMessageList();
			levelID = id;
			string[] itempath = levelID.Split('\\');
			levelName = itempath[itempath.Length - 1];
			LevelData.LevelName = levelName;
			log.Add("----Loading a new level: " + levelName + "----");
			Text = "SALVL - Loading " + levelName + "...";

#if !DEBUG
			backgroundWorker1.RunWorkerAsync();
#else
			backgroundWorker1_DoWork(null, null);
			backgroundWorker1_RunWorkerCompleted(null, null);
#endif
			unsaved = false;
		}

		/// <summary>
		/// Loads all of the textures from the file into the scene.
		/// </summary>
		/// <param name="file">The name of the file.</param>
		/// <param name="systemPath">The game's system path.</param>
		void LoadTextureList(string file, string systemPath)
		{
			LoadTextureList(TextureList.Load(file), systemPath);
		}

		/// <summary>
		/// Loads all of the textures specified into the scene.
		/// </summary>
		/// <param name="textureEntries">The texture entries to load.</param>
		/// <param name="systemPath">The mod's system path.</param>
		private void LoadTextureList(IEnumerable<TextureListEntry> textureEntries, string systemPath)
		{
			foreach (TextureListEntry entry in textureEntries)
			{
				if (string.IsNullOrEmpty(entry.Name))
					continue;

				LoadPVM(entry.Name, systemPath);
			}
		}

		/// <summary>
		/// Loads textures from a PVM/GVM/PVMX/PAK or a texture pack into the scene.
		/// </summary>
		/// <param name="pvmName">The PVM/PRS/PVMX/GVM/PAK/texture pack name (name only; no path or extension).</param>
		/// <param name="systemPath">The mod's system path.</param>
		void LoadPVM(string pvmName, string systemPath)
		{
			if (!LevelData.TextureBitmaps.ContainsKey(pvmName))
			{
				string texturePath;
				string extension = ".PVM";
				string textureFallbackPath;
				// Determine whether a custom texture pack or a PVMX exists
				if (Directory.Exists(Path.Combine(modFolder, "textures", pvmName)))
					texturePath = Path.Combine(modFolder, "textures", pvmName, "index.txt");
				else if (File.Exists(Path.Combine(modFolder, "textures", pvmName + ".PVMX")))
					texturePath = Path.Combine(modFolder, "textures", pvmName + ".PVMX");
				// Check if a PAK file exists in the PRS folder (SA2)
				else if (File.Exists(Path.Combine(modFolder, "gd-pc", "PRS", pvmName + ".PAK")))
					texturePath = Path.Combine(modFolder, "gd-pc", "PRS", pvmName + ".PAK");
				else if (File.Exists(Path.Combine(systemFallback, "PRS", pvmName) + ".PAK"))
				{
					extension = ".PAK";
					texturePath = Path.Combine(systemPath, "PRS", pvmName) + extension;
				}
				else
				{
					if (File.Exists(Path.Combine(systemFallback, pvmName) + ".PVM"))
						extension = ".PVM";
					else if (File.Exists(Path.Combine(systemFallback, pvmName) + ".GVM"))
						extension = ".GVM";
					else if (File.Exists(Path.Combine(systemFallback, pvmName) + ".PRS"))
						extension = ".PRS";
					texturePath = Path.Combine(systemPath, pvmName) + extension;
				}
				if (extension != ".PAK")
					textureFallbackPath = Path.Combine(systemFallback, pvmName) + extension;
				else
					textureFallbackPath = Path.Combine(systemFallback, "PRS", pvmName) + extension;
				BMPInfo[] textureBitmaps = TextureArchive.GetTextures(ProjectFunctions.ModPathOrGameFallback(texturePath, textureFallbackPath));
				log.Add("Loading textures: " + ProjectFunctions.ModPathOrGameFallback(texturePath, textureFallbackPath));
				Texture[] d3dTextures;
				if (textureBitmaps != null)
				{
					d3dTextures = new Texture[textureBitmaps.Length];
					for (int i = 0; i < textureBitmaps.Length; i++)
						d3dTextures[i] = textureBitmaps[i].Image.ToTexture(d3ddevice);

					LevelData.TextureBitmaps.Add(pvmName, textureBitmaps);
					LevelData.Textures.Add(pvmName, d3dTextures);
				}
				else
				{
					log.Add("Unable to load texture file: " + ProjectFunctions.ModPathOrGameFallback(texturePath, textureFallbackPath));
				}
			}
		}

		private void LoadLandtable(string filename)
		{
			LandTable land;
			string ext = Path.GetExtension(filename).ToLowerInvariant();
			switch (ext)
			{
				case ".sa1lvl":
				case ".sa2lvl":
				case ".sa2blvl":
					land = LandTable.LoadFromFile(filename);
					break;
				default:
					BinaryFileDialog bd = new BinaryFileDialog();
					DialogResult res = bd.ShowDialog();
					if (res != DialogResult.OK) return;
					LandTableFormat fmt = LandTableFormat.SADX;
					switch (bd.comboLevelFormat.SelectedIndex)
					{
						case 0: // SA1
							fmt = LandTableFormat.SA1;
							break;
						case 2: // SA2
							fmt = LandTableFormat.SA2;
							break;
						case 3: // SA2B
							fmt = LandTableFormat.SA2B;
							break;
						case 1: // SADXPC 
						default:
							break;
					}
					ByteConverter.Reverse = bd.checkBoxReverse.Checked;
					ByteConverter.BigEndian = bd.checkBoxBigEndian.Checked;
					uint key = (uint)bd.numericKey.Value;
					// PRS
					byte[] datafile = File.ReadAllBytes(filename);
					if (ext == ".prs") datafile = FraGag.Compression.Prs.Decompress(datafile);
					// Trim
					if (bd.numericStartOffset.Value != 0)
					{
						byte[] datafile_new = new byte[datafile.Length + (uint)bd.numericStartOffset.Value];
						datafile.CopyTo(datafile_new, (int)bd.numericStartOffset.Value);
						datafile = datafile_new;
					}
					// REL
					if (bd.comboFileKeyHint.SelectedIndex == 3)
					{
						datafile = SplitTools.HelperFunctions.DecompressREL(datafile);
						SplitTools.HelperFunctions.FixRELPointers(datafile, 0xC900000);
					}
					land = new LandTable(datafile, (int)bd.numericAddress.Value, key, fmt);
					break;
			}
			selectedItems.Clear();
			sceneGraphControl1.InitSceneControl(selectedItems);
			PointHelper.Instances.Clear();
			LevelData.leveltexs = null;
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.Black.ToRawColorBGRA(), 1, 0);
			LevelData.geo = land;
			LevelData.ClearLevelItems();
			LevelData.ClearLevelAnims();
			LevelData.LevelSplines = new List<SplineData>();
			LevelData.ObjDefs = new List<ObjectDefinition>();
			SplineData.Init();
			for (int i = 0; i < LevelData.geo.COL.Count; i++)
			{
				LevelData.AddLevelItem(new LevelItem(LevelData.geo.COL[i], i, selectedItems));
			}
			for (int i = 0; i < LevelData.geo.Anim.Count; i++)
			{
				LevelData.AddLevelAnim(new LevelAnim(LevelData.geo.Anim[i], i, selectedItems));
			}
			transformGizmo = new TransformGizmo();
			bool isGeometryPresent = LevelData.geo != null;
			bool isSETPreset = !LevelData.SETItemsIsNull();
			bool isDeathZonePresent = LevelData.DeathZones != null;
			levelPieceToolStripMenuItem.Enabled = isGeometryPresent;
			objectToolStripMenuItem.Enabled = isSETPreset;
			missionObjectToolStripMenuItem.Enabled = LevelData.MissionSETItems != null;
			// Import
			importToolStripMenuItem.Enabled = isGeometryPresent;
			// Export
			exportToolStripMenuItem.Enabled = isGeometryPresent;
			// Edit menu
			// Clear Level
			clearLevelToolStripMenuItem.Enabled = isGeometryPresent;
			// SET Items submenu
			editSETItemsToolStripMenuItem.Enabled = true;
			viewSETItemsToolStripMenuItem.Enabled = true;
			// Calculate All Bounds
			calculateAllBoundsToolStripMenuItem.Enabled = isGeometryPresent;

			// The whole view menu!
			viewToolStripMenuItem.Enabled = true;
			layersToolStripMenuItem.Enabled = true;
			statsToolStripMenuItem.Enabled = isGeometryPresent;
			viewDeathZonesToolStripMenuItem.Checked = deathZonesButton.Enabled = deathZonesButton.Checked = viewDeathZonesToolStripMenuItem.Enabled = deathZoneToolStripMenuItem.Enabled = isDeathZonePresent;
			advancedToolStripMenuItem.Enabled = true;
			addToolStripMenuItem1.Enabled = true;
			addToolStripMenuItem.Enabled = true;
			unloadTexturesToolStripMenuItem.Enabled = LevelData.Textures != null;
			AnimationPlaying = playAnimButton.Checked = false;
			playAnimButton.Enabled = prevFrameButton.Enabled = nextFrameButton.Enabled = resetAnimButton.Enabled = LevelData.LevelAnimCount > 0;
			isStageLoaded = true;
			selectedItems.SelectionChanged += SelectionChanged;
			UseWaitCursor = false;
			Enabled = true;

			gizmoSpaceComboBox.Enabled = true;
			if (gizmoSpaceComboBox.SelectedIndex == -1) gizmoSpaceComboBox.SelectedIndex = 0;
			pivotComboBox.Enabled = true;
			if (pivotComboBox.SelectedIndex == -1) pivotComboBox.SelectedIndex = 0;
			jumpToStartPositionToolStripMenuItem.Enabled = moveToStartButton.Enabled = LevelData.StartPositions != null;
			addAllLevelItemsToolStripMenuItem.Enabled = true;
			toolStrip1.Enabled = editLevelInfoToolStripMenuItem.Enabled = isStageLoaded;
			LevelData.SuppressEvents = false;
			LevelData.InvalidateRenderState();
			unloadTexturesToolStripMenuItem.Enabled = LevelData.Textures != null;
			editSETItemsToolStripMenuItem.Enabled = advancedSaveSETFileBigEndianToolStripMenuItem.Enabled = advancedSaveSETFileToolStripMenuItem.Enabled = unloadSETFileToolStripMenuItem.Enabled = addSETItemToolStripMenuItem.Enabled = LevelData.SETItemsIsNull() != true;
			addCAMItemToolStripMenuItem.Enabled = LevelData.CAMItems != null;
			addMissionItemToolStripMenuItem.Enabled = LevelData.MissionSETItems != null;
			addDeathZoneToolStripMenuItem.Enabled = LevelData.DeathZones != null;
			saveAdvancedToolStripMenuItem.Enabled = true;
			timeOfDayToolStripMenuItem.Enabled = stageLightList != null;
			upgradeObjDefsToolStripMenuItem.Enabled = salvlini != null;
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			MainLevelLoadLoop();
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (initerror)
			{
				Close();
				return;
			}

			bool isGeometryPresent = LevelData.geo != null;
			bool isSETPreset = !LevelData.SETItemsIsNull();
			bool isDeathZonePresent = LevelData.DeathZones != null;

			// Context menu
			// Add -> Level Piece
			// Does this even make sense? This thing prompts the user to import a model,
			// not select an existing one...
			levelPieceToolStripMenuItem.Enabled = isGeometryPresent;
			// Add -> Object
			objectToolStripMenuItem.Enabled = isSETPreset;
			// Add -> Mission Object
			missionObjectToolStripMenuItem.Enabled = LevelData.MissionSETItems != null;

			// File menu
			// Save
			saveToolStripMenuItem.Enabled = true;
			// Import
			importToolStripMenuItem.Enabled = isGeometryPresent;
			// Export
			exportToolStripMenuItem.Enabled = isGeometryPresent;

			// Edit menu
			// Clear Level
			clearLevelToolStripMenuItem.Enabled = isGeometryPresent;
			// SET Items submenu
			editSETItemsToolStripMenuItem.Enabled = true;
			viewSETItemsToolStripMenuItem.Enabled = true;

			// Advanced Save menu
			saveAdvancedToolStripMenuItem.Enabled = advancedSaveSETFileBigEndianToolStripMenuItem.Enabled = advancedSaveSETFileToolStripMenuItem.Enabled = true;

			// Calculate All Bounds
			calculateAllBoundsToolStripMenuItem.Enabled = isGeometryPresent;

			// The whole view menu!
			viewToolStripMenuItem.Enabled = true;
			layersToolStripMenuItem.Enabled = true;
			statsToolStripMenuItem.Enabled = isGeometryPresent;
			viewDeathZonesToolStripMenuItem.Checked = deathZonesButton.Enabled = deathZonesButton.Checked = viewDeathZonesToolStripMenuItem.Enabled = deathZoneToolStripMenuItem.Enabled = isDeathZonePresent;
			advancedToolStripMenuItem.Enabled = true;
			addToolStripMenuItem1.Enabled = true;
			addToolStripMenuItem.Enabled = true;

			isStageLoaded = true;
			selectedItems.SelectionChanged += SelectionChanged;
			UseWaitCursor = false;
			Enabled = true;

			gizmoSpaceComboBox.Enabled = true;
			if (gizmoSpaceComboBox.SelectedIndex == -1) gizmoSpaceComboBox.SelectedIndex = 0;
			pivotComboBox.Enabled = true;
			if (pivotComboBox.SelectedIndex == -1) pivotComboBox.SelectedIndex = 0;

			addAllLevelItemsToolStripMenuItem.Enabled = true;
			toolStrip1.Enabled = editLevelInfoToolStripMenuItem.Enabled = isStageLoaded;
			LevelData.SuppressEvents = false;
			LevelData.InvalidateRenderState();
			unloadTexturesToolStripMenuItem.Enabled = LevelData.Textures != null;
			progress.StepProgress();
			AnimationPlaying = playAnimButton.Checked = false;
			playAnimButton.Enabled = prevFrameButton.Enabled = nextFrameButton.Enabled = resetAnimButton.Enabled = LevelData.LevelAnimCount > 0;
			JumpToStartPos();
		}

		private void LoadStageLights(SA1LevelAct levelact)
		{
			if ((stageLightList != null) && (stageLightList.Count > 0))
			{
				List<SADXStageLightData> currentStageLights = new List<SADXStageLightData>();

				foreach (SADXStageLightData lightData in stageLightList)
				{
					if (lightData.Level == levelact.Level)
					{
						// Adventure Field day/night stuff
						if (levelact.Level == SA1LevelIDs.StationSquare)
						{
							switch (levelact.Act)
							{
								// LightData acts: 0 - day, 1 - evening, 2 - sewers, 3 - night
								// Stage acts: 0, 1, 3, 4 - outside, 2 - sewers, 5 - TP entrance
								case 0:
								case 1:
								case 3:
								case 4:
									if (daytimeToolStripMenuItem.Checked && lightData.Act == 0)
										currentStageLights.Add(lightData);
									else if (eveningToolStripMenuItem.Checked && lightData.Act == 1)
										currentStageLights.Add(lightData);
									else if (nightToolStripMenuItem.Checked && lightData.Act == 3)
										currentStageLights.Add(lightData);
									break;
								case 2:
								case 5:
									if (lightData.Act == 2)
										currentStageLights.Add(lightData); // TP entrance doesn't use Stage Lights though
									break;
							}

							if ((levelact.Act == 2 || levelact.Act == 5) && lightData.Act == 2)
								currentStageLights.Add(lightData); // TP entrance doesn't use Stage Lights though

						}
						else if (levelact.Level == SA1LevelIDs.MysticRuins)
						{
							// 0 - day, 1 - evening, 2 - night, 3 - base
							if (levelact.Act == 3 && lightData.Act == 3)
								currentStageLights.Add(lightData);
							else if (daytimeToolStripMenuItem.Checked && lightData.Act == 0)
								currentStageLights.Add(lightData);
							else if (eveningToolStripMenuItem.Checked && lightData.Act == 1)
								currentStageLights.Add(lightData);
							else if (nightToolStripMenuItem.Checked && lightData.Act == 2)
								currentStageLights.Add(lightData);
						}
						else if (lightData.Act == levelact.Act)
							currentStageLights.Add(lightData);
					}
				}

				if (levelact.Act > 0 && currentStageLights.Count <= 0)
				{
					for (int i = 1; i < levelact.Act + 1; i++)
					{
						foreach (SADXStageLightData lightData in stageLightList)
						{
							if ((lightData.Level == levelact.Level) && (lightData.Act == levelact.Act - i))
								if (currentStageLights.Count < 4)
									currentStageLights.Add(lightData);
						}
					}
				}

				if (currentStageLights.Count > 0)
					EditorOptions.StageLights = currentStageLights;
				else
				{
					osd.AddMessage("No lights were found for this stage. Using default lights instead.", 180);
					log.Add("No lights were found for this stage. Using default lights.");
					EditorOptions.StageLights = null;
				}
			}
			else
			{
				osd.AddMessage("Stage Lights data not found. Using default lights.", 180);
				log.Add("Stage Lights data not found. Using default lights.");
				EditorOptions.StageLights = null;
			}
			log.WriteLog();
		}

		private void LoadCharacterLights(SA1LevelAct levelact)
		{
			if ((characterLightList != null) && (characterLightList.Count > 0))
			{
				List<LSPaletteData> currentCharacterLights = new List<LSPaletteData>();

				foreach (LSPaletteData lightData in characterLightList)
				{
					if (lightData.Level == levelact.Level)
					{
						// Adventure Field day/night stuff
						if (levelact.Level == SA1LevelIDs.StationSquare)
						{
							switch (levelact.Act)
							{
								// LightData acts: 0 - day, 1 - evening, 2 - sewers, 3 - night
								// Stage acts: 0, 1, 3, 4 - outside, 2 - sewers, 5 - TP entrance
								case 0:
								case 1:
								case 3:
								case 4:
									if (daytimeToolStripMenuItem.Checked && lightData.Act == 0)
										currentCharacterLights.Add(lightData);
									else if (eveningToolStripMenuItem.Checked && lightData.Act == 1)
										currentCharacterLights.Add(lightData);
									else if (nightToolStripMenuItem.Checked && lightData.Act == 3)
										currentCharacterLights.Add(lightData);
									break;
								case 2:
								case 5:
									if (lightData.Act == 2)
										currentCharacterLights.Add(lightData); // TP entrance doesn't use Stage Lights though
									break;
							}

							if ((levelact.Act == 2 || levelact.Act == 5) && lightData.Act == 2)
								currentCharacterLights.Add(lightData); // TP entrance doesn't use Stage Lights though

						}
						else if (levelact.Level == SA1LevelIDs.MysticRuins)
						{
							// 0 - day, 1 - evening, 2 - night, 3 - base
							if (levelact.Act == 3 && lightData.Act == 3)
								currentCharacterLights.Add(lightData);
							else if (daytimeToolStripMenuItem.Checked && lightData.Act == 0)
								currentCharacterLights.Add(lightData);
							else if (eveningToolStripMenuItem.Checked && lightData.Act == 1)
								currentCharacterLights.Add(lightData);
							else if (nightToolStripMenuItem.Checked && lightData.Act == 2)
								currentCharacterLights.Add(lightData);
						}
						else if (lightData.Act == levelact.Act)
							currentCharacterLights.Add(lightData);
					}
				}
				if (levelact.Act > 0 && currentCharacterLights.Count <= 0)
				{
					for (int i = 1; i < levelact.Act + 1; i++)
					{
						foreach (LSPaletteData lightData in characterLightList)
						{
							if ((lightData.Level == levelact.Level) && (lightData.Act == levelact.Act - i))
								if (currentCharacterLights.Count < 4)
									currentCharacterLights.Add(lightData);
						}
					}
				}
				if (currentCharacterLights.Count > 0)
					EditorOptions.CharacterLights = currentCharacterLights;
				else
				{
					osd.AddMessage("No character lights were found for this stage. Using level lights instead.", 180);
					log.Add("No character lights were found for this stage. Using level lights.");
					EditorOptions.CharacterLights = null;
				}
			}
			else
			{
				osd.AddMessage("Character lights data not found. Using level lights instead.", 180);
				log.Add("Character lights data not found. Using level lights instead.");
				EditorOptions.CharacterLights = null;
			}
			log.WriteLog();
		}

		private FogData GetFogData(FogDataTable data, int act)
		{
			if (data != null)
				switch (editorDetailSetting)
				{
					case ClipLevel.Far:
					default:
						return data.Act[act].High;
					case ClipLevel.Medium:
						return data.Act[act].Medium;
					case ClipLevel.Near:
						return data.Act[act].Low;
				}
			return null;
		}

		private void LoadFog(SA1LevelAct levelact)
		{
			bool fogdatanotfound = false;
			if (salvlini != null && salvlini.LevelFogFiles.FogEntries != null && salvlini.LevelFogFiles.FogEntries.ContainsKey(levelact.Level))
			{
				string fogFilePath = salvlini.LevelFogFiles.FogEntries[levelact.Level];
				if (File.Exists(fogFilePath))
				{
					stageFogList = IniSerializer.Deserialize<FogDataTable>(fogFilePath);
					if (stageFogList.Act[levelact.Act] != null)
					{
						UpdateStageFog();
					}
					else
					{
						osd.AddMessage("Fog data for this level is null. Stage fog is disabled.", 180);
						log.Add("Fog data for this level is null. Stage fog is disabled.");
						currentStageFog = null;
					}
				}
				else
					fogdatanotfound = true;
			}
			else
				fogdatanotfound = true;
			if (fogdatanotfound)
			{
				osd.AddMessage("Fog data not found. Stage fog is disabled.", 180);
				log.Add("Fog data not found. Stage fog is disabled.");
				stageFogList = null;
				currentStageFog = null;
			}

		}

		private void LoadSA2SetFiles(string setfallback, string setstr)
		{
			string setTxt = "set";
			string setEnd = "_u.bin";
			string useSetPath = ProjectFunctions.ModPathOrGameFallback(setstr, setfallback);

			if (File.Exists(useSetPath))
			{
				if (progress != null)
					progress.SetTask("SET: " + Path.GetFileName(useSetPath));

				List<SETItem> SetList = new List<SETItem>();

				SetList = SETItem.Load(useSetPath, selectedItems);
				setfallback = Path.Combine(systemFallback, setTxt + LevelData.SETName + setEnd);
				setstr = Path.Combine(modSystemFolder, setTxt + LevelData.SETName + setEnd);

				string useSetPath2 = ProjectFunctions.ModPathOrGameFallback(setstr, setfallback);

				if (File.Exists(useSetPath2))
				{
					SetList.AddRange(SETItem.Load(useSetPath2, selectedItems));
				}

				LevelData.AssignSetList(0, SetList);
			}
			else
			{
				LevelData.AssignSetList(0, new List<SETItem>());
			}
		}

		private void LoadSA2DeathZones(IniLevelData level)
		{
			LevelData.DeathZones = new List<DeathZoneItem>();
			if (File.Exists(level.DeathZones))
			{

				SA2BDeathZoneFlags[] dzini = SA2BDeathZoneFlagsList.Load(level.DeathZones);
				string path = Path.GetDirectoryName(level.DeathZones);
				for (int i = 0; i < dzini.Length; i++)
				{
					progress.SetStep(String.Format("Loading model {0}/{1}", (i + 1), dzini.Length));

					LevelData.DeathZones.Add(new DeathZoneItem(new ModelFile(Path.Combine(path, dzini[i].Filename)).Model, dzini[i].Flags, selectedItems));
				}
			}
			else
				LevelData.DeathZones = null;
		}

		private void LoadSA2EnemiesTextures(string systemPath)
		{
			LoadPVM("E_G_KUMITEX", systemPath);		
			LoadPVM("E_EMITEX", systemPath);
			LoadPVM("E_GOLDTEX", systemPath);
			LoadPVM("E_KUMITEX", systemPath);			
			LoadPVM("E_KYOKOTEX", systemPath);		
			LoadPVM("E_AITEX", systemPath);
		}

		private void MainLevelLoadLoop()
		{
#if !DEBUG
			try
			{
#endif
			int steps = 9;
			if (d3ddevice == null)
				++steps;

			toolStrip1.Enabled = false;
			LevelData.SuppressEvents = true;

			#region Initialization and cleanup

			if (isStageLoaded)
			{
				LevelData.Clear();
				selectedItems = new EditorItemSelection();
				sceneGraphControl1.InitSceneControl(selectedItems);
				PointHelper.Instances.Clear();
				LevelData.ClearTextures();
				stageFogList = null;
			}

			isStageLoaded = false;

			progress.SetTask("Loading stage: " + levelName);
			progress.ResetSteps();
			progress.SetMaxSteps(steps);
			IniLevelData level = salvlini.Levels[levelID];

			SA1LevelAct levelact = new SA1LevelAct(level.LevelID);
			LevelData.leveltexs = null;

			Invoke((Action)progress.Show);

			if (d3ddevice == null)
			{
				progress.SetTask("Initializing Direct3D...");
				Invoke((Action)InitializeDirect3D);
				progress.StepProgress();
			}
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.Black.ToRawColorBGRA(), 1, 0);
			progress.SetTaskAndStep("Loading level data:", "Geometry");

			// Set current directory. Otherwise most stuff won't be able to load.
			Environment.CurrentDirectory = modFolder;

			// Load Landtable
			if (string.IsNullOrEmpty(level.LevelGeometry))
				LevelData.geo = null;
			else
			{
				LevelData.geo = LandTable.LoadFromFile(level.LevelGeometry);
				LevelData.ClearLevelItems();
				LevelData.ClearLevelAnims();

				for (int i = 0; i < LevelData.geo.COL.Count; i++)
				{
					LevelData.AddLevelItem(new LevelItem(LevelData.geo.COL[i], i, selectedItems));
				}

				for (int i = 0; i < LevelData.geo.Anim.Count; i++)
				{
					LevelData.AddLevelAnim(new LevelAnim(LevelData.geo.Anim[i], i, selectedItems));
				}
			}

			// Initialize level textures
			LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
			LevelData.Textures = new Dictionary<string, Texture[]>();
			if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName))
				LevelData.leveltexs = LevelData.geo.TextureFileName;

			progress.StepProgress();

			#endregion

			#region Start Positions

			progress.SetTaskAndStep("Setting up start positions...");

			LevelData.StartPositions = new StartPosItem[LevelData.Characters.Length];


			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				progress.SetStep(string.Format("{0}/{1}", (i + 1), LevelData.StartPositions.Length));

				IniCharInfo character;

				if (isSA2LVL())
				{
					character = salvlini.Characters[LevelData.SA2Characters[i]];
				}
				else
				{
					if (i == 0 && levelact.Level == SA1LevelIDs.PerfectChaos)
						character = salvlini.Characters["SuperSonic"];
					else
						character = salvlini.Characters[LevelData.Characters[i]];
				}

				Dictionary<SA1LevelAct, SA1StartPosInfo> posini = new Dictionary<SA1LevelAct, SA1StartPosInfo>();

				//to do: Add SA2 Start Pos
				if (File.Exists(character.StartPositions) && salvlini.IsSA2 != true)
				{
					posini = SA1StartPosList.Load(character.StartPositions);
				}

				Vertex pos = new Vertex();
				int rot = 0;

				if (posini.ContainsKey(levelact))
				{
					pos = posini[levelact].Position;
					rot = posini[levelact].YRotation;
				}
				if (File.Exists(character.Model))
					LevelData.StartPositions[i] = new StartPosItem(new ModelFile(character.Model).Model,
					character.Textures, character.Height, pos, rot, d3ddevice, selectedItems);
				else
					LevelData.StartPositions[i] = new StartPosItem(new NJS_OBJECT(),
					character.Textures, character.Height, pos, rot, d3ddevice, selectedItems);
				if (File.Exists(character.TextureList))
					LoadTextureList(character.TextureList, modSystemFolder);
			}

			JumpToStartPos();

			progress.StepProgress();

			#endregion

			#region Death Zones

			progress.SetTaskAndStep("Death Zones:", "Initializing...");

			if (string.IsNullOrEmpty(level.DeathZones))
				LevelData.DeathZones = null;
			else
			{
				if (isSA2LVL())
				{
					LoadSA2DeathZones(level);
				}
				else
				{
					LevelData.DeathZones = new List<DeathZoneItem>();
					if (File.Exists(level.DeathZones))
					{

						DeathZoneFlags[] dzini = DeathZoneFlagsList.Load(level.DeathZones);
						string path = Path.GetDirectoryName(level.DeathZones);
						for (int i = 0; i < dzini.Length; i++)
						{
							progress.SetStep(String.Format("Loading model {0}/{1}", (i + 1), dzini.Length));

							LevelData.DeathZones.Add(new DeathZoneItem(new ModelFile(Path.Combine(path, dzini[i].Filename)).Model, dzini[i].Flags, selectedItems));
						}
					}
					else
						LevelData.DeathZones = null;
				}
			}

			progress.StepProgress();

			#endregion

			#region Textures and Texture Lists

			progress.SetTaskAndStep("Loading textures for:");

			progress.SetStep("Common objects");
			// Loads common object textures (e.g OBJ_REGULAR)
			if (File.Exists(salvlini.ObjectTextureList))
				LoadTextureList(salvlini.ObjectTextureList, modSystemFolder);			
			
			// Loads skybox / BG tex (SA2 only)
			if (File.Exists(level.BackgroundTextureList))
				LoadTextureList(level.BackgroundTextureList, modSystemFolder);

			if (isSA2LVL())
				LoadSA2EnemiesTextures(modSystemFolder);

			progress.SetStep("Mission objects");
			// Loads mission object textures
			if (File.Exists(salvlini.MissionTextureList))
				LoadTextureList(salvlini.MissionTextureList, modSystemFolder);

			progress.SetTaskAndStep("Loading stage texture lists...");

			// Loads the textures in the texture list for this stage (e.g BEACH01)

			if (salvlini.LevelTextureLists != null)
			{
				// Loads the textures in the texture list for this stage (e.g BEACH01)
				foreach (string file in Directory.GetFiles(salvlini.LevelTextureLists))
				{
					LevelTextureList texini = LevelTextureList.Load(file);
					if (texini.Level != levelact)
						continue;
					LoadTextureList(texini.TextureList, modSystemFolder);
				}
			}
			else
			{
				if (level.TextureList != null)
				{
					LevelTextureList texini = LevelTextureList.Load(Path.Combine(level.TextureList));
					LoadTextureList(texini.TextureList, modSystemFolder);
				}
			}

			progress.SetTaskAndStep("Loading textures for:", "Objects");
			// Object texture list(s)
			if (File.Exists(level.ObjectTextureList))
				LoadTextureList(level.ObjectTextureList, modSystemFolder);

			progress.SetStep("Stage");
			// Set stage PVM name for stages that don't have it.
			// This also loads things like skybox textures for some stages.
			if (level.Textures != null && level.Textures.Length > 0)
				foreach (string tex in level.Textures)
				{
					LoadPVM(tex, modSystemFolder);
					if (string.IsNullOrEmpty(LevelData.leveltexs))
						LevelData.leveltexs = tex;
				}

			// Load PVMs for stages that don't have a stage texture list.
			if (!string.IsNullOrEmpty(LevelData.leveltexs))
				LoadPVM(LevelData.leveltexs, modSystemFolder);

			progress.StepProgress();

			#endregion

			#region Object Definitions / SET Layout

			progress.SetTaskAndStep("Loading Object Definitions:", "Parsing...");

			// Load Object Definitions INI file
			if (File.Exists(salvlini.ObjectDefinitions))
				objdefini = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(salvlini.ObjectDefinitions);
			LevelData.ObjDefs = new List<ObjectDefinition>();
			LevelData.MisnObjDefs = new List<ObjectDefinition>();
			InitObjDefReferences();
			// Load SET items
			if (!string.IsNullOrEmpty(level.ObjectList) && File.Exists(level.ObjectList))
			{
				LoadObjectList(level.ObjectList);
				progress.SetTaskAndStep("Loading SET items", "Initializing...");

				// Assign SET data
				if (LevelData.ObjDefs.Count > 0)
				{
					LevelData.SETName = level.SETName ?? level.LevelID;
					string setTxt = "SET";
					string setEnd = "{0}.bin";

					if (isSA2LVL())
					{
						setEnd = "_s.bin";
					}

					string setfallback = Path.Combine(systemFallback, setTxt + LevelData.SETName + setEnd);
					string setstr = Path.Combine(modSystemFolder, setTxt + LevelData.SETName + setEnd);
					LevelData.InitSETItems();

					if (isSA2LVL())
					{
						LoadSA2SetFiles(setfallback, setstr);
					}
					else
					{
						for (int i = 0; i < LevelData.SETChars.Length; i++)
						{
							string formatted = string.Format(setstr, LevelData.SETChars[i]);
							string formattedFallback = string.Format(setfallback, LevelData.SETChars[i]);

							string useSetPath = ProjectFunctions.ModPathOrGameFallback(formatted, formattedFallback);
							if (File.Exists(useSetPath))
							{
								if (progress != null) progress.SetTask("SET: " + Path.GetFileName(useSetPath));
								LevelData.AssignSetList(i, SETItem.Load(useSetPath, selectedItems));
							}
							else
							{
								LevelData.AssignSetList(i, new List<SETItem>());
							}
						}
					}
				}
				else
				{
					LevelData.NullifySETItems();
					osd.AddMessage("Object definitions not found (0 entries), SET files skipped", 180);
				}
			}
			else
			{
				LevelData.NullifySETItems();
				osd.AddMessage("Object definitions not found (object list file doesn't exist), SET files skipped", 180);
			}

			// Load Mission SET items
			if (!string.IsNullOrEmpty(salvlini.MissionObjectList) && File.Exists(salvlini.MissionObjectList))
			{
				LoadObjectList(salvlini.MissionObjectList, true);

				// Assign Mission SET data
				progress.SetTaskAndStep("Loading Mission SET items", "Initializing...");

				if (LevelData.MisnObjDefs.Count > 0)
				{
					string setstrFallback = Path.Combine(systemFallback, "SETMI" + level.LevelID + "{0}.bin");
					string setstr = Path.Combine(modSystemFolder, "SETMI" + level.LevelID + "{0}.bin");

					string prmstrFallback = Path.Combine(systemFallback, "PRMMI" + level.LevelID + "{0}.bin");
					string prmstr = Path.Combine(modSystemFolder, "PRMMI" + level.LevelID + "{0}.bin");
					LevelData.MissionSETItems = new List<MissionSETItem>[LevelData.SETChars.Length];
					for (int i = 0; i < LevelData.SETChars.Length; i++)
					{
						List<MissionSETItem> list = new List<MissionSETItem>();
						byte[] setfile = null;
						byte[] prmfile = null;

						string setNormFmt = string.Format(setstr, LevelData.SETChars[i]);
						string setFallbackFmt = string.Format(setstrFallback, LevelData.SETChars[i]);

						string prmNormFmt = string.Format(prmstr, LevelData.SETChars[i]);
						string prmFallbackFmt = string.Format(prmstrFallback, LevelData.SETChars[i]);

						string setfmt = ProjectFunctions.ModPathOrGameFallback(setNormFmt, setFallbackFmt);
						string prmfmt = ProjectFunctions.ModPathOrGameFallback(prmNormFmt, prmFallbackFmt);

						if (File.Exists(setfmt)) setfile = File.ReadAllBytes(setfmt);
						if (File.Exists(prmfmt)) prmfile = File.ReadAllBytes(prmfmt);

						if (setfile != null && prmfile != null)
						{
							progress.SetTask("Mission SET: " + Path.GetFileName(setfmt));
							bool bigendianbk = ByteConverter.BigEndian;
							ByteConverter.BigEndian = HelperFunctions.CheckBigEndianInt32(setfile, 0);
							int count = ByteConverter.ToInt32(setfile, 0);
							int setaddr = 0x20;
							int prmaddr = 0x20;
							for (int j = 0; j < count; j++)
							{
								progress.SetStep(string.Format("{0}/{1}", (j + 1), count));

								MissionSETItem ent = new MissionSETItem(setfile, setaddr, prmfile, prmaddr, selectedItems);
								list.Add(ent);
								setaddr += 0x20;
								prmaddr += 0xC;
							}
							ByteConverter.BigEndian = bigendianbk;
						}
						LevelData.MissionSETItems[i] = list;
					}
				}
				else
				{
					LevelData.MissionSETItems = null;
				}
			}
			else
			{
				LevelData.MissionSETItems = null;
			}

			progress.StepProgress();

			#endregion

			#region CAM Layout

			progress.SetTaskAndStep("Loading CAM items", "Initializing...");

			string camFallback = Path.Combine(systemFallback, "CAM" + LevelData.SETName + "{0}.bin");
			string camstr = Path.Combine(modSystemFolder, "CAM" + LevelData.SETName + "{0}.bin");

			LevelData.CAMItems = new List<CAMItem>[LevelData.SETChars.Length];
			for (int i = 0; i < LevelData.SETChars.Length; i++)
			{
				List<CAMItem> list = new List<CAMItem>();
				byte[] camfile = null;

				string camfmt = string.Format(camstr, LevelData.SETChars[i]);
				string camfmtFallback = string.Format(camFallback, LevelData.SETChars[i]);

				string formatted = (ProjectFunctions.ModPathOrGameFallback(camfmt, camfmtFallback));

				/*if (modpath != null && File.Exists(Path.Combine(modpath, formatted)))
					camfile = File.ReadAllBytes(Path.Combine(modpath, formatted));
				else if (File.Exists(formatted))*/
				if (File.Exists(formatted)) camfile = File.ReadAllBytes(formatted);

				if (camfile != null)
				{
					progress.SetTask("CAM: " + Path.GetFileName(formatted));
					bool bigendianbk = ByteConverter.BigEndian;
					ByteConverter.BigEndian = HelperFunctions.CheckBigEndianInt32(camfile, 0);
					int count = ByteConverter.ToInt32(camfile, 0);
					int address = 0x40;
					for (int j = 0; j < count; j++)
					{
						progress.SetStep(string.Format("{0}/{1}", (j + 1), count));

						CAMItem ent = new CAMItem(camfile, address, selectedItems);
						list.Add(ent);
						address += 0x40;
					}
					ByteConverter.BigEndian = bigendianbk;
				}

				LevelData.CAMItems[i] = list;
			}

			CAMItem.Init();

			progress.StepProgress();

			#endregion

			#region Loading Level Effects

			LevelData.leveleff = null;
			if (!string.IsNullOrEmpty(level.Effects))
			{
				progress.SetTaskAndStep("Loading Level Effects...");

				LevelDefinition def = null;
				string ty; 

				if (isSA2LVL())
					ty = "SA2ObjectDefinitions.Level_Effects." + Path.GetFileNameWithoutExtension(level.Effects);
				else
					ty = "SADXObjectDefinitions.Level_Effects." + Path.GetFileNameWithoutExtension(level.Effects);

				string dllfile = Path.Combine("dllcache", ty + ".dll");
				string pdbfile = Path.Combine("dllcache", ty + ".pdb");
				DateTime modDate = DateTime.MinValue;
				if (File.Exists(dllfile))
					modDate = File.GetLastWriteTime(dllfile);

				string fp = level.Effects.Replace('/', Path.DirectorySeparatorChar);
				if (File.Exists(fp))
				{
					if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
					{
						def =
							(LevelDefinition)
								Activator.CreateInstance(
									Assembly.LoadFile(Path.Combine(modFolder, dllfile)).GetType(ty));
					}
					else
					{

						SyntaxTree[] st = new[] { SyntaxFactory.ParseSyntaxTree(File.ReadAllText(fp), CSharpParseOptions.Default, fp, Encoding.UTF8) };

						CSharpCompilation compilation =
								CSharpCompilation.Create(ty, st, objectDefinitionReferences, objectDefinitionOptions);

						try
						{
							EmitResult result = compilation.Emit(dllfile, pdbfile);

							if (!result.Success)
							{
								log.Add("Error loading level background:");
								foreach (Diagnostic diagnostic in result.Diagnostics)
								{
									log.Add(String.Format("\n\n{0}", diagnostic.ToString()));
								}

								File.Delete(dllfile);
								File.Delete(pdbfile);

								def = null;
							}
							else
							{
								def =
									(LevelDefinition)
										Activator.CreateInstance(
											Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
												.GetType(ty));
							}
						}
						catch (Exception e)
						{
							log.Add("Error loading level background:" + String.Format("\n\n{0}", e.ToString()));

							File.Delete(dllfile);
							File.Delete(pdbfile);

							def = null;
						}
					}

					if (def != null)
					{
						byte timeofday = 0;
						if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins || levelact.Level == SA1LevelIDs.SkyDeck || levelact.Level == SA1LevelIDs.EggCarrierOutside)
						{
							if (eveningToolStripMenuItem.Checked)
								timeofday = 1;
							else if (nightToolStripMenuItem.Checked)
								timeofday = 2;
						}
						def.Init(level, levelact.Act, timeofday);
					}
				}
				LevelData.leveleff = def;
			}

			progress.StepProgress();

			#endregion

			#region Loading Splines

			LevelData.LevelSplines = new List<SplineData>();
			SplineData.Init();

			if (salvlini != null && !string.IsNullOrEmpty(salvlini.Paths))
			{
				progress.SetTaskAndStep("Reticulating splines...");

				String splineDirectory = Path.Combine(Path.Combine(modFolder, salvlini.Paths),
					levelact.ToString());

				if (Directory.Exists(splineDirectory))
				{
					List<string> pathFiles = new List<string>();

					for (int i = 0; i < int.MaxValue; i++)
					{
						string path = Path.Combine(splineDirectory, string.Format("{0}.ini", i));
						if (File.Exists(path))
						{
							pathFiles.Add(path);
						}
						else
							break;
					}

					foreach (string pathFile in pathFiles) // looping through path files
					{
						SplineData newSpline = new SplineData(PathData.Load(pathFile), selectedItems);

						newSpline.RebuildMesh(d3ddevice);

						LevelData.LevelSplines.Add(newSpline);
					}
				}
			}

			progress.StepProgress();

			#endregion

			#region Stage Lights
			progress.SetTaskAndStep("Loading lights...");

			LoadStageLights(levelact);
			LoadCharacterLights(levelact);
			#endregion

			#region Fog
			LoadFog(levelact);
			fogEditorToolStripMenuItem.Enabled = fogButton.Enabled = currentStageFog != null;
			#endregion

			transformGizmo = new TransformGizmo();

			log.Add("----Level load complete: " + levelName + "----\n");
			log.WriteLog();
#if !DEBUG
			}
			catch (Exception ex)
			{
				log.Add(ex.ToString() + "\n");
				string errDesc = "SALVL could not load the level for the following reason:\n" + ex.GetType().Name + ".\n\n" +
					"If you wish to report a bug, please include the following in your report:";
				ErrorDialog report = new ErrorDialog("SALVL", errDesc, log.GetLogString());
				log.WriteLog();
				DialogResult dgresult = report.ShowDialog();
				switch (dgresult)
				{
					case DialogResult.Cancel:
						initerror = true;
						break;
					case DialogResult.Abort:
					case DialogResult.OK:
						Application.Exit();
						break;
				}
			}
#endif
		}
	}
}