using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.UI;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
		/// <summary>
		/// Saves changes made to the currently loaded stage.
		/// </summary>
		/// <param name="autoCloseDialog">Defines whether or not the progress dialog should close on completion.</param>
		private void SaveStage(bool autoCloseDialog)
		{
			if (!isStageLoaded)
				return;

			IniLevelData level = salvlini.Levels[levelID];

			Directory.CreateDirectory(modSystemFolder);

			if (isSA2LVL()) 
				SaveSA2Data(autoCloseDialog, level);
			else
				SaveSA1Data(autoCloseDialog, level);
		}

		private void SaveSETFile(bool bigendian, bool isSA2, bool manualMode)
		{
			if (isSA2)
			{
				List<SETItem> substansiveObjects = new List<SETItem>();
				List<SETItem> unsubstansiveObjects = new List<SETItem>();

				foreach (SETItem setObject in LevelData.SETItems(LevelData.SA2Set))
				{
					if (setObject.ClipSetting == ClipSetting.SA2Unsubstansive)
						unsubstansiveObjects.Add(setObject);
					else
						substansiveObjects.Add(setObject);
				}

				// TODO: Implement some form of Kart save support. Should be using another flag.

				if (manualMode)
				{
					using (SaveFileDialog a = new SaveFileDialog
					{
						DefaultExt = "bin",
						Filter = "SET files|SET*.bin",
					})
					{
						if (a.ShowDialog() == DialogResult.OK)
						{
							string subName = Path.GetFileNameWithoutExtension(a.FileName);
							if (subName.Contains("_s"))
								subName += ".bin";
							else
								subName += "_s.bin";
							string unsubName = Path.GetFileNameWithoutExtension(a.FileName);
							if (unsubName.Contains("_u"))
								unsubName += ".bin";
							else
								unsubName += "_u.bin";
							
							SETItem.Save(substansiveObjects.ToList(), Path.Combine(Path.GetDirectoryName(a.FileName), subName), bigendian);
							SETItem.Save(unsubstansiveObjects.ToList(), Path.Combine(Path.GetDirectoryName(a.FileName), unsubName), bigendian);
						}
					}
				}
				else
				{
					for (int i = 0; i < LevelData.SA2SetTypes.Length; i++)
					{
						string subName = "set" + LevelData.SETName + LevelData.SA2SetTypes[i] + "_s.bin";
						string unsubName = "set" + LevelData.SETName + LevelData.SA2SetTypes[i] + "_u.bin";
						string subNamePath = Path.Combine(modFolder, "gd_PC", subName);
						string unsubNamePath = Path.Combine(modFolder, "gd_PC", unsubName);
						SETItem.Save(substansiveObjects.ToList(), subNamePath, bigendian);
						SETItem.Save(unsubstansiveObjects.ToList(), unsubNamePath, bigendian);
					}
				}
				
			}
			else
			{
				if (manualMode)
				{
					using (SaveFileDialog a = new SaveFileDialog
					{
						DefaultExt = "bin",
						Filter = "SET files|SET*.bin",
					})
					{
						if (a.ShowDialog() == DialogResult.OK)
						{
							SETItem.Save(LevelData.SETItems(LevelData.Character).ToList(), a.FileName, bigendian);
						}
					}
				}
				else
				{
					for (int i = 0; i < LevelData.SETChars.Length; i++)
					{
						string setstr = Path.Combine(modSystemFolder, "SET" + LevelData.SETName + LevelData.SETChars[i] + ".bin");

						SETItem.Save(LevelData.SETItems(LevelData.Character).ToList(), setstr, bigendian);
					}
				}
			}
		}

		private void SaveCamFile(bool bigendian)
		{
			using (SaveFileDialog a = new()
			{
				DefaultExt = "bin",
				Filter = "CAM files|CAM*.bin",
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					{
						CAMItem.Save(LevelData.camItems(LevelData.Character).ToList(), a.FileName, bigendian);
					}
				}
			}
		}

		#region SA1 Save Functions
		private void SaveSA1Data(bool autoCloseDialog, IniLevelData level)
		{
			ProgressDialog progress = new ProgressDialog("Saving stage: " + levelName, 6, true, autoCloseDialog);
			progress.Show(this);
			Application.DoEvents();

			SA1LevelAct levelact = new SA1LevelAct(level.LevelID);

			#region Save Stage Geometry
			progress.SetTaskAndStep("Saving:", "Geometry...");

			if (LevelData.geo != null)
				LevelData.geo.SaveToFile(level.LevelGeometry, LandTableFormat.SA1);

			progress.StepProgress();

			progress.Step = "Start positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				if (!File.Exists(salvlini.Characters[LevelData.Characters[i]].StartPositions))
				{
					log.Add("Error saving start positions for character " + i.ToString());
					osd.AddMessage("Error saving start positions for character " + i.ToString(), 180);
					break;
				}

				Dictionary<SA1LevelAct, SA1StartPosInfo> posini =
					SA1StartPosList.Load(salvlini.Characters[LevelData.Characters[i]].StartPositions);

				if (posini.ContainsKey(levelact))
					posini.Remove(levelact);

				if (LevelData.StartPositions[i].Position.X != 0 || LevelData.StartPositions[i].Position.Y != 0 ||
					LevelData.StartPositions[i].Position.Z != 0 || LevelData.StartPositions[i].Rotation.Y != 0)
				{
					posini.Add(levelact,
						new SA1StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position,
							YRotation = LevelData.StartPositions[i].Rotation.Y
						});
				}

				posini.Save(salvlini.Characters[LevelData.Characters[i]].StartPositions);
			}

			progress.StepProgress();

			#endregion

			#region Save Death Zones
			progress.Step = "Death zones...";
			Application.DoEvents();

			if (LevelData.DeathZones != null)
			{
				DeathZoneFlags[] dzini = new DeathZoneFlags[LevelData.DeathZones.Count];
				string path = Path.GetDirectoryName(level.DeathZones);
				for (int i = 0; i < LevelData.DeathZones.Count; i++)
					dzini[i] = LevelData.DeathZones[i].Save(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl");
				dzini.Save(level.DeathZones);
			}

			progress.StepProgress();

			#endregion

			#region Saving SET Items

			progress.Step = "SET items...";
			Application.DoEvents();

			if (!LevelData.SETItemsIsNull())
			{
				// TODO: Fix this so there's a Big Endian variable.
				SaveSETFile(false, false, (salvlini == null));
			}

			progress.StepProgress();

			#endregion

			#region Saving CAM Items

			progress.Step = "CAM items...";
			Application.DoEvents();

			if (LevelData.CAMItems != null)
			{
				for (int i = 0; i < LevelData.CAMItems.Length; i++)
				{
					string camString = Path.Combine(modSystemFolder, "CAM" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
					/*if (modpath != null)
						camString = Path.Combine(modpath, camString);*/

					// TODO: Handle this differently. File stream? If the user is using a symbolic link for example, we defeat the purpose by deleting it.
					if (File.Exists(camString))
						File.Delete(camString);

					if (LevelData.CAMItems[i].Count == 0)
						continue;

					List<byte> file = new List<byte>(LevelData.CAMItems[i].Count * 0x40 + 0x40); // setting up file size and header
					file.AddRange(BitConverter.GetBytes(LevelData.CAMItems[i].Count));
					file.Align(0x40);


					foreach (CAMItem item in LevelData.CAMItems[i]) // outputting individual components
						file.AddRange(item.GetBytes());

					File.WriteAllBytes(camString, file.ToArray());
				}
			}

			#endregion

			#region Saving Mission SET Items

			progress.Step = "Mission SET items...";
			Application.DoEvents();

			if (LevelData.MissionSETItems != null)
			{
				for (int i = 0; i < LevelData.MissionSETItems.Length; i++)
				{
					string setstr = Path.Combine(modSystemFolder, "SETMI" + level.LevelID + LevelData.SETChars[i] + ".bin");
					string prmstr = Path.Combine(modSystemFolder, "PRMMI" + level.LevelID + LevelData.SETChars[i] + ".bin");
					/*if (modpath != nullmodSystemPath
					{
						setstr = Path.Combine(modpath, setstr);
						prmstr = Path.Combine(modpath, prmstr);
					}*/

					// TODO: Consider simply blanking the SET file instead of deleting it.
					// Completely deleting it might be undesirable since Sonic's layout will be loaded
					// in place of the missing file. And where mods are concerned, you could have conflicts
					// with other mods if the file is deleted.
					if (File.Exists(setstr))
						File.Delete(setstr);
					if (File.Exists(prmstr))
						File.Delete(prmstr);
					if (LevelData.MissionSETItems[i].Count == 0)
						continue;

					List<byte> setfile = new List<byte>(LevelData.MissionSETItems[i].Count * 0x20 + 0x20);
					setfile.AddRange(BitConverter.GetBytes(LevelData.MissionSETItems[i].Count));
					setfile.Align(0x20);

					List<byte> prmfile = new List<byte>(LevelData.MissionSETItems[i].Count * 0xC + 0x20);
					prmfile.AddRange(new byte[] { 0, 0, 0x30, 0x56 });
					prmfile.Align(0x20);

					foreach (MissionSETItem item in LevelData.MissionSETItems[i])
					{
						setfile.AddRange(item.GetBytes());
						prmfile.AddRange(item.GetPRMBytes());
					}

					File.WriteAllBytes(setstr, setfile.ToArray());
					File.WriteAllBytes(prmstr, prmfile.ToArray());
				}
			}

			progress.StepProgress();
			#endregion

			#region Saving Splines
			progress.Step = "Splines...";
			Application.DoEvents();
			if (LevelData.LevelSplines != null)
			{
				string splineDirectory = Path.Combine(Path.Combine(modFolder, salvlini.Paths),
							levelact.ToString());
				int s = 0;
				foreach (SplineData spline in LevelData.LevelSplines)
				{
					string path = Path.Combine(splineDirectory, string.Format("{0}.ini", s));
					spline.Save(path);
					s++;
				}
			}
			#endregion

			#region Saving Lights
			progress.Step = "Lights...";
			Application.DoEvents();
			if (stageLightList != null)
			{
				string stageLightPath = Path.Combine(modFolder, "Misc", "Stage Lights.ini");
				IniSerializer.Serialize(stageLightList, stageLightPath);
			}
			if (characterLightList != null)
			{
				string charLightPath = Path.Combine(modFolder, "Misc", "Character Lights.ini");
				IniSerializer.Serialize(characterLightList, charLightPath);
			}
			#endregion

			#region Saving Fog
			progress.Step = "Fog...";
			Application.DoEvents();
			if (stageFogList != null && salvlini.LevelFogFiles.FogEntries != null && salvlini.LevelFogFiles.FogEntries.ContainsKey(levelact.Level))
			{
				string fogFilePath = salvlini.LevelFogFiles.FogEntries[levelact.Level];
				IniSerializer.Serialize(stageFogList, fogFilePath);
			}
			#endregion

			progress.StepProgress();
			progress.SetTaskAndStep("Save complete!");
			unsaved = false;
			Application.DoEvents();
		}

		#endregion

		#region SA2 Save Functions
		private void SaveSA2LandTables(bool hasextra)
		{
			string filter;
			string defext;
			switch (LevelData.geo.Format)
			{
				case LandTableFormat.SA2:
					defext = "sa2lvl";
					filter = "SA2 Level Files|*.sa2lvl";
					break;
				case LandTableFormat.SA2B:
					defext = "sa2blvl";
					filter = "SA2B Level Files|*.sa2blvl";
					break;
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
				default:
					defext = "sa1lvl";
					filter = "SA1/SADX Level Files| *.sa1lvl";
					break;
			}
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = defext,
				Filter = filter,
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					LevelData.geo.SaveToFile(a.FileName, LevelData.geo.Format);
				}
			}
			if (hasextra)
			{
				for (int i = 0; i < LevelData.secondgeos.Count; i++)
				{
					LandTable level = LevelData.secondgeos[i];
					switch (level.Format)
					{
						case LandTableFormat.SA2:
							defext = "sa2lvl";
							filter = "SA2 Level Files|*.sa2lvl";
							break;
						case LandTableFormat.SA2B:
							defext = "sa2blvl";
							filter = "SA2B Level Files|*.sa2blvl";
							break;
						case LandTableFormat.SA1:
						case LandTableFormat.SADX:
						default:
							defext = "sa1lvl";
							filter = "SA1/SADX Level Files| *.sa1lvl";
							break;
					}
					using (SaveFileDialog a = new SaveFileDialog
					{
						DefaultExt = defext,
						Filter = filter,
					})
					{
						if (a.ShowDialog() == DialogResult.OK)
						{
							LevelData.secondgeos[i].SaveToFile(a.FileName, level.Format);
						}
					}
				}
			}
		}

		private void SaveSA2Data(bool autoCloseDialog, IniLevelData level)
		{
			SA2LevelIDs SA2level = (SA2LevelIDs)byte.Parse(level.LevelID, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

			ProgressDialog progress = new ProgressDialog("Saving stage: " + levelName, 7, true, autoCloseDialog);
			progress.Show(this);
			Application.DoEvents();

			#region Saving Geometry
			progress.SetTaskAndStep("Saving:", "Geometry...");

			if (LevelData.geo != null)
			{
				LevelData.geo.SaveToFile(level.LevelGeometry, LevelData.geo.Format);
			}
			if (LevelData.secondgeos != null)
			{
				for (int i = 0; i < LevelData.secondgeos.Count; i++)
					LevelData.secondgeos[i].SaveToFile(level.SecondaryGeometry[i], LevelData.secondgeos[i].Format);
			}
			progress.StepProgress();

			#endregion

			#region Saving SET Items

			Application.DoEvents();

			if (!LevelData.SETItemsIsNull())
			{
				SaveSETFile(true, true, (salvlini == null));
			}

			#endregion

			#region Death Zones

			progress.Step = "Death zones...";
			Application.DoEvents();

			if (LevelData.DeathZones != null)
			{
				DeathZoneFlags[] dzini = new DeathZoneFlags[LevelData.DeathZones.Count];
				string path = Path.GetDirectoryName(level.DeathZones);
				for (int i = 0; i < LevelData.DeathZones.Count; i++)
					dzini[i] = LevelData.DeathZones[i].Save(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl");
				dzini.Save(level.DeathZones);
			}

			progress.StepProgress();
			#endregion

			#region SA2 Start Positions

			progress.Step = "Start positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				if (!File.Exists(salvlini.Characters[LevelData.SA2Characters[i]].StartPositions))
				{
					log.Add("Error saving start positions for character " + i.ToString());
					osd.AddMessage("Error saving start positions for character " + i.ToString(), 180);
					break;
				}

				Dictionary<SA2LevelIDs, SA2StartPosInfo> posini = SA2StartPosList.Load(salvlini.Characters[LevelData.SA2Characters[i]].StartPositions);

				if (posini.ContainsKey(SA2level))
					posini.Remove(SA2level);

				if (LevelData.StartPositions[i].Position.X != 0 || LevelData.StartPositions[i].Position.Y != 0 ||
					LevelData.StartPositions[i].Position.Z != 0 || LevelData.StartPositions[i].Rotation.Y != 0)
				{
					if (LevelData.SA2StartPositions2P1[i] != null && (LevelData.SA2StartPositions2P1[i].Position.X != 0 ||
						LevelData.SA2StartPositions2P1[i].Position.Y != 0 || LevelData.SA2StartPositions2P1[i].Position.Z != 0 || LevelData.SA2StartPositions2P1[i].Rotation.Y != 0))
					{
						if (LevelData.SA2StartPositions2P2[i] != null && (LevelData.SA2StartPositions2P2[i].Position.X != 0 ||
						LevelData.SA2StartPositions2P2[i].Position.Y != 0 || LevelData.SA2StartPositions2P2[i].Position.Z != 0 || LevelData.SA2StartPositions2P2[i].Rotation.Y != 0))
						{
							posini.Add(SA2level,
						new SA2StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position,
							YRotation = (ushort)LevelData.StartPositions[i].Rotation.Y,
							P1Position = LevelData.SA2StartPositions2P1[i].Position,
							P1YRotation = (ushort)LevelData.SA2StartPositions2P1[i].YRotation,
							P2Position = LevelData.SA2StartPositions2P2[i].Position,
							P2YRotation = (ushort)LevelData.SA2StartPositions2P2[i].YRotation
						});
						}
						else
							posini.Add(SA2level,
						new SA2StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position,
							YRotation = (ushort)LevelData.StartPositions[i].Rotation.Y,
							P1Position = LevelData.SA2StartPositions2P1[i].Position,
							P1YRotation = (ushort)LevelData.SA2StartPositions2P1[i].YRotation,
						});
					}
					else
						posini.Add(SA2level,
						new SA2StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position,
							YRotation = (ushort)LevelData.StartPositions[i].Rotation.Y,
						});
				}

				posini.Save(salvlini.Characters[LevelData.SA2Characters[i]].StartPositions);
			}

			progress.StepProgress();
			#endregion

			#region SA2 End Positions

			progress.Step = "End positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.EndPositions.Length; i++)
			{
				if (!File.Exists(salvlini.Characters[LevelData.SA2Characters[i]].EndPositions))
				{
					log.Add("Error saving end positions for character " + i.ToString());
					osd.AddMessage("Error saving end positions for character " + i.ToString(), 180);
					break;
				}

				Dictionary<SA2LevelIDs, SA2StartPosInfo> posini = SA2StartPosList.Load(salvlini.Characters[LevelData.SA2Characters[i]].EndPositions);

				if (posini.ContainsKey(SA2level))
					posini.Remove(SA2level);

				if (LevelData.EndPositions[i].Position.X != 0 || LevelData.EndPositions[i].Position.Y != 0 ||
					LevelData.EndPositions[i].Position.Z != 0 || LevelData.EndPositions[i].Rotation.Y != 0)
				{
					if (LevelData.EndPositions2P1[i] != null && (LevelData.EndPositions2P1[i].Position.X != 0 ||
						LevelData.EndPositions2P1[i].Position.Y != 0 || LevelData.EndPositions2P1[i].Position.Z != 0 || LevelData.EndPositions2P1[i].Rotation.Y != 0))
					{
						if (LevelData.EndPositions2P2[i] != null && (LevelData.EndPositions2P2[i].Position.X != 0 ||
						LevelData.EndPositions2P2[i].Position.Y != 0 || LevelData.EndPositions2P2[i].Position.Z != 0 || LevelData.EndPositions2P2[i].Rotation.Y != 0))
						{
							posini.Add(SA2level,
						new SA2StartPosInfo()
						{
							Position = LevelData.EndPositions[i].Position,
							YRotation = (ushort)LevelData.EndPositions[i].Rotation.Y,
							P1Position = LevelData.EndPositions2P1[i].Position,
							P1YRotation = (ushort)LevelData.EndPositions2P1[i].YRotation,
							P2Position = LevelData.EndPositions2P2[i].Position,
							P2YRotation = (ushort)LevelData.EndPositions2P2[i].YRotation
						});
						}
						else
							posini.Add(SA2level,
						new SA2StartPosInfo()
						{
							Position = LevelData.EndPositions[i].Position,
							YRotation = (ushort)LevelData.EndPositions[i].Rotation.Y,
							P1Position = LevelData.EndPositions2P1[i].Position,
							P1YRotation = (ushort)LevelData.EndPositions2P1[i].YRotation,
						});
					}
					else
						posini.Add(SA2level,
						new SA2StartPosInfo()
						{
							Position = LevelData.EndPositions[i].Position,
							YRotation = (ushort)LevelData.EndPositions[i].Rotation.Y,
						});
				}

				posini.Save(salvlini.Characters[LevelData.SA2Characters[i]].EndPositions);
			}

			progress.StepProgress();
			#endregion

			#region 2P Intro Positions

			progress.Step = "2P Intro positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.MultiplayerIntroPositionsA.Length; i++)
			{
				if (!File.Exists(salvlini.Characters[LevelData.SA2Characters[i]].MultiplayerIntroPositions))
				{
					log.Add("Error saving 2P intro positions for character " + i.ToString());
					osd.AddMessage("Error saving 2P intro positions for character " + i.ToString(), 180);
					break;
				}

				Dictionary<SA2LevelIDs, SA2EndPosInfo> posini = SA2EndPosList.Load(salvlini.Characters[LevelData.SA2Characters[i]].MultiplayerIntroPositions);

				if (posini.ContainsKey(SA2level))
					posini.Remove(SA2level);

				if (LevelData.MultiplayerIntroPositionsA[i].Position.X != 0 || LevelData.MultiplayerIntroPositionsA[i].Position.Y != 0 ||
					LevelData.MultiplayerIntroPositionsA[i].Position.Z != 0 || LevelData.MultiplayerIntroPositionsA[i].Rotation.Y != 0)
				{
					if (LevelData.MultiplayerIntroPositionsB[i] != null && (LevelData.MultiplayerIntroPositionsB[i].Position.X != 0 ||
						LevelData.MultiplayerIntroPositionsB[i].Position.Y != 0 || LevelData.MultiplayerIntroPositionsB[i].Position.Z != 0 || LevelData.MultiplayerIntroPositionsB[i].Rotation.Y != 0))
					{
						posini.Add(SA2level,
						new SA2EndPosInfo()
						{
							Mission2Position = LevelData.MultiplayerIntroPositionsA[i].Position,
							Mission2YRotation = (ushort)LevelData.MultiplayerIntroPositionsA[i].Rotation.Y,
							Mission3Position = LevelData.MultiplayerIntroPositionsB[i].Position,
							Mission3YRotation = (ushort)LevelData.MultiplayerIntroPositionsB[i].YRotation,
						});
					}
					else
						posini.Add(SA2level,
					new SA2EndPosInfo()
					{
						Mission2Position = LevelData.MultiplayerIntroPositionsA[i].Position,
						Mission2YRotation = (ushort)LevelData.MultiplayerIntroPositionsA[i].Rotation.Y,
					});
				}
				posini.Save(salvlini.Characters[LevelData.SA2Characters[i]].MultiplayerIntroPositions);
			}

			progress.StepProgress();
			#endregion

			#region Mission 2/3 End Positions

			progress.Step = "Mission 2/3 end positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.AltEndPositionsA.Length; i++)
			{
				if (!File.Exists(salvlini.Characters[LevelData.SA2Characters[i]].AltEndPositions))
				{
					log.Add("Error saving Mission 2/3 end positions for character " + i.ToString());
					osd.AddMessage("Error saving Mission 2/3 positions for character " + i.ToString(), 180);
					break;
				}

				Dictionary<SA2LevelIDs, SA2EndPosInfo> posini = SA2EndPosList.Load(salvlini.Characters[LevelData.SA2Characters[i]].AltEndPositions);

				if (posini.ContainsKey(SA2level))
					posini.Remove(SA2level);

				if (LevelData.AltEndPositionsA[i].Position.X != 0 || LevelData.AltEndPositionsA[i].Position.Y != 0 ||
					LevelData.AltEndPositionsA[i].Position.Z != 0 || LevelData.AltEndPositionsA[i].Rotation.Y != 0)
				{
					if (LevelData.AltEndPositionsB[i] != null && (LevelData.AltEndPositionsB[i].Position.X != 0 ||
						LevelData.AltEndPositionsB[i].Position.Y != 0 || LevelData.AltEndPositionsB[i].Position.Z != 0 || LevelData.AltEndPositionsB[i].Rotation.Y != 0))
					{
						posini.Add(SA2level,
						new SA2EndPosInfo()
						{
							Mission2Position = LevelData.AltEndPositionsA[i].Position,
							Mission2YRotation = (ushort)LevelData.AltEndPositionsA[i].Rotation.Y,
							Mission3Position = LevelData.AltEndPositionsB[i].Position,
							Mission3YRotation = (ushort)LevelData.AltEndPositionsB[i].YRotation,
						});
					}
					else
						posini.Add(SA2level,
					new SA2EndPosInfo()
					{
						Mission2Position = LevelData.AltEndPositionsA[i].Position,
						Mission2YRotation = (ushort)LevelData.AltEndPositionsA[i].Rotation.Y,
					});
				}
				posini.Save(salvlini.Characters[LevelData.SA2Characters[i]].AltEndPositions);
			}

			progress.StepProgress();
			#endregion

			progress.StepProgress();
			progress.SetTaskAndStep("Save complete!");
			unsaved = false;
			Application.DoEvents();
		}

		#endregion
	}
}