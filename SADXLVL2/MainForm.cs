﻿﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SA_Tools;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using System.Linq;

using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SADXLVL2
{
	// TODO: Organize this whole class.
	// TODO: Unify as much SET/CAM load and save code as possible. They're practically identical.
	// TODO: Rename controls to be more distinguishable.
	// (Example: sETItemsToolStripMenuItem1 is a dropdown menu. sETITemsToolStripMenuItem is a toggle.)
	public partial class MainForm : Form
	{
		Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			InitializeComponent();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			using (ErrorDialog ed = new ErrorDialog(e.Exception, true))
				if (ed.ShowDialog(this) == DialogResult.Cancel)
					Close();
		}

		internal Device d3ddevice;
		SAEditorCommon.IniData ini;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		string levelID;
		internal string levelName;
		bool isStageLoaded;
		EditorItemSelection selectedItems = new EditorItemSelection();
		Dictionary<string, List<string>> levelNames;
		bool lookKeyDown;
		bool zoomKeyDown;
		Point menuLocation;
		bool isPointOperation;

		// TODO: Make these both configurable.
		bool mouseWrapScreen = false;
		ushort mouseWrapThreshold = 2;

		// helpers / ui stuff
		TransformGizmo transformGizmo;

		// light list
		List<SA1StageLightData> stageLightList;

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			LevelData.StateChanged += LevelData_StateChanged;
			LevelData.PointOperation += LevelData_PointOperation;
			panel1.MouseWheel += panel1_MouseWheel;

			if (ShowQuickStart())
				ShowLevelSelect();
		}

		private bool ShowQuickStart()
		{
			bool result;
			using (QuickStartDialog dialog = new QuickStartDialog(this, GetRecentFiles()))
			{
				if (result = (dialog.ShowDialog() == DialogResult.OK))
					LoadINI(dialog.SelectedItem);
			}
			return result;
		}

		private void ShowLevelSelect()
		{
			string stageToLoad = string.Empty;
			using (LevelSelectDialog dialog = new LevelSelectDialog(levelNames))
			{
				if (dialog.ShowDialog() == DialogResult.OK)
					stageToLoad = dialog.SelectedStage;
			}

			if (!string.IsNullOrEmpty(stageToLoad))
			{
				if (isStageLoaded)
				{
					if (SavePrompt(true) == DialogResult.Cancel)
						return;
				}

				CheckMenuItemByTag(changeLevelToolStripMenuItem, stageToLoad);
				LoadStage(stageToLoad);
			}
		}

		private void InitializeDirect3D()
		{
			if (d3ddevice == null)
			{
				d3ddevice = new Device(0, DeviceType.Hardware, panel1, CreateFlags.HardwareVertexProcessing,
					new PresentParameters
					{
						Windowed = true,
						SwapEffect = SwapEffect.Discard,
						EnableAutoDepthStencil = true,
						AutoDepthStencilFormat = DepthFormat.D24X8
					});
				d3ddevice.DeviceResizing += d3ddevice_DeviceResizing;

				EditorOptions.Initialize(d3ddevice);
				Gizmo.InitGizmo(d3ddevice);
				ObjectHelper.Init(d3ddevice, Properties.Resources.UnknownImg);
			}
		}

		private StringCollection GetRecentFiles()
		{
			if (Settings.MRUList == null)
				Settings.MRUList = new StringCollection();

			StringCollection mru = new StringCollection();

			foreach (string item in Settings.MRUList)
			{
				if (File.Exists(item))
				{
					mru.Add(item);
					recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
				}
			}

			Settings.MRUList = mru;
			if (mru.Count > 0)
				recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem2);

			if (Program.args.Length > 0)
				LoadINI(Program.args[0]);

			return mru;
		}

		void d3ddevice_DeviceResizing(object sender, CancelEventArgs e)
		{
			// HACK: Not so sure we should have to re-initialize this every time...
			EditorOptions.Initialize(d3ddevice);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFile();
		}

		public bool OpenFile()
		{
			if (isStageLoaded)
			{
				if (SavePrompt() == DialogResult.Cancel)
					return false;
			}

			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "INI Files|*.ini|All Files|*.*"
			};

			if (a.ShowDialog(this) == DialogResult.OK)
			{
				LoadINI(a.FileName);
				return true;
			}

			return false;
		}

		private void LoadINI(string filename)
		{
			isStageLoaded = false;
			ini = SAEditorCommon.IniData.Load(filename);
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
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

			// If no projects have been recently used, clear the list to remove the "(none)" entry.
			if (Settings.MRUList.Count == 0)
				recentProjectsToolStripMenuItem.DropDownItems.Clear();

			// If this project has been loaded before, remove it...
			if (Settings.MRUList.Contains(filename))
			{
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
				Settings.MRUList.Remove(filename);
			}

			// ...so that it can be re-inserted at the top of the Recent Projects list.
			Settings.MRUList.Insert(0, filename);
			recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));

			// File menu -> Change Level
			changeLevelToolStripMenuItem.Enabled = true;

			// load stage lights
			string stageLightPath = string.Concat(Environment.CurrentDirectory, "\\Levels\\Stage Lights.ini");

			if (File.Exists(stageLightPath))
			{
				stageLightList = SA1StageLightDataList.Load(stageLightPath);
			}
		}

		private void PopulateLevelMenu(ToolStripMenuItem targetMenu, Dictionary<string, List<string>> levels)
		{
			// Used for keeping track of menu items
			Dictionary<string, ToolStripMenuItem> levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
			targetMenu.DropDownItems.Clear();

			foreach (KeyValuePair<string, List<string>> item in levels)
			{
				// For every section (e.g Adventure Fields) in levels, reset the parent menu.
				// It gets changed later if necessary.
				ToolStripMenuItem parent = targetMenu;
				foreach (string stage in item.Value)
				{
					// If a menu item for this section has not already been initialized...
					if (!levelMenuItems.ContainsKey(item.Key))
					{
						// Create it
						ToolStripMenuItem i = new ToolStripMenuItem(item.Key.Replace("&", "&&"));

						// Add it to the list to keep track of it
						levelMenuItems.Add(item.Key, i);
						// Add the new menu item to the parent menu
						parent.DropDownItems.Add(i);
						// and set the parent so we know where to put the stage
						parent = i;
					}
					else
					{
						// Otherwise, set the parent to the existing reference
						parent = levelMenuItems[item.Key];
					}

					// And finally, create the menu item for the stage name itself and hook it up to the Clicked event.
					// The Tag member here is vital. The code later on uses this to determine what assets to load.
					parent.DropDownItems.Add(new ToolStripMenuItem(stage, null, LevelToolStripMenuItem_Clicked)
					{
						Tag = item.Key + '\\' + stage
					});
				}
			}
		}

		// TODO: Move this stuff somewhere that it can be accessed by all projects
		
		/// <summary>
		/// Iterates recursively through menu items and unchecks all sub-items.
		/// </summary>
		/// <param name="menu">The parent menu of the items to be unchecked.</param>
		private static void UncheckMenuItems(ToolStripDropDownItem menu)
		{
			foreach (ToolStripMenuItem i in menu.DropDownItems)
			{
				if (i.HasDropDownItems)
					UncheckMenuItems(i);
				else
					i.Checked = false;
			}
		}

		/// <summary>
		/// Unchecks all children of the parent object and checks the target.
		/// </summary>
		/// <param name="target">The item to check</param>
		/// <param name="parent">The parent menu containing the target.</param>
		private static void CheckMenuItem(ToolStripMenuItem target, ToolStripItem parent = null)
		{
			if (target == null)
				return;

			if (parent == null)
				parent = target.OwnerItem;

			UncheckMenuItems((ToolStripDropDownItem)parent);
			target.Checked = true;
		}

		/// <summary>
		/// Iterates recursively through the parent and checks the first item it finds with a matching Tag.
		/// If firstOf is true, recursion stops after the first match.
		/// </summary>
		/// <param name="parent">The parent menu.</param>
		/// <param name="tag">The tag to search for.</param>
		/// <param name="firstOf">If true, recursion stops after the first match.</param>
		/// <returns></returns>
		private static bool CheckMenuItemByTag(ToolStripDropDownItem parent, string tag, bool firstOf = true)
		{
			foreach (ToolStripMenuItem i in parent.DropDownItems)
			{
				if (i.HasDropDownItems)
				{
					if (CheckMenuItemByTag(i, tag, firstOf))
						return true;
				}
				else if ((string)i.Tag == tag)
				{
					if (firstOf)
					{
						CheckMenuItem(i, parent);
						return true;
					}
					else
					{
						i.Checked = true;
					}
				}
			}

			return false;
		}
		/// <summary>
		/// Displays a dialog asking if the user would like to save.
		/// </summary>
		/// <param name="autoCloseDialog">Defines whether or not the save progress dialog should close on completion.</param>
		/// <returns></returns>
		private DialogResult SavePrompt(bool autoCloseDialog = false)
		{
			DialogResult result = MessageBox.Show(this, "Do you want to save?", "SADXLVL2",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			switch (result)
			{
				case DialogResult.Yes:
					SaveStage(autoCloseDialog);
					break;
			}

			return result;
		}

		private void LevelToolStripMenuItem_Clicked(object sender, EventArgs e)
		{
			fileToolStripMenuItem.HideDropDown();

			if (!isStageLoaded || SavePrompt(true) != DialogResult.Cancel)
			{
				UncheckMenuItems(changeLevelToolStripMenuItem);
				((ToolStripMenuItem)sender).Checked = true;
				LoadStage(((ToolStripMenuItem)sender).Tag.ToString());
			}
		}

		private void LoadStage(string id)
		{
			UseWaitCursor = true;
			Enabled = false;

			levelID = id;
			string[] itempath = levelID.Split('\\');
			levelName = itempath[itempath.Length - 1];
			LevelData.LevelName = levelName;
			Text = "SADXLVL2 - Loading " + levelName + "...";

#if !DEBUG
			backgroundWorker1.RunWorkerAsync();
#else
			backgroundWorker1_DoWork(null, null);
			backgroundWorker1_RunWorkerCompleted(null, null);
#endif
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
		/// <param name="systemPath">The game's system path.</param>
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
		/// Loads textures from a PVM into the scene.
		/// </summary>
		/// <param name="pvmName">The PVM name (name only; no path or extension).</param>
		/// <param name="systemPath">The game's system path.</param>
		void LoadPVM(string pvmName, string systemPath)
		{
			if (!LevelData.TextureBitmaps.ContainsKey(pvmName))
			{
				BMPInfo[] textureBitmaps = TextureArchive.GetTextures(Path.Combine(systemPath, pvmName) + ".PVM");
				Texture[] d3dTextures = new Texture[textureBitmaps.Length];

				for (int i = 0; i < textureBitmaps.Length; i++)
					d3dTextures[i] = new Texture(d3ddevice, textureBitmaps[i].Image, Usage.None, Pool.Managed);

				LevelData.TextureBitmaps.Add(pvmName, textureBitmaps);
				LevelData.Textures.Add(pvmName, d3dTextures);
			}
		}

		bool initerror = false;
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
#if !DEBUG
			try
			{
#endif
				int steps = 10;
				if (d3ddevice == null)
					++steps;

				toolStrip1.Enabled = false;

				// HACK: Fixes Twinkle Circuit's geometry lingering if loaded before Sky Chase.
				// I'm sure the real problem is somewhere below, but this is sort of an all around cleanup.
				if (isStageLoaded)
					LevelData.Clear();

				isStageLoaded = false;

				using (ProgressDialog progress = new ProgressDialog("Loading stage: " + levelName, steps))
				{
					IniLevelData level = ini.Levels[levelID];

					string syspath = Path.Combine(Environment.CurrentDirectory, ini.SystemPath);
					string modpath = ini.ModPath;

					SA1LevelAct levelact = new SA1LevelAct(level.LevelID);
					LevelData.leveltexs = null;
					cam = new EditorCamera(EditorOptions.RenderDrawDistance);

					Invoke((Action<IWin32Window>)progress.Show, this);

					if (d3ddevice == null)
					{
						progress.SetTask("Initializing Direct3D...");
						Invoke((Action)InitializeDirect3D);
						progress.StepProgress();
					}

					progress.SetTaskAndStep("Loading level data:", "Geometry");

					if (string.IsNullOrEmpty(level.LevelGeometry))
						LevelData.geo = null;
					else
					{
						LevelData.geo = LandTable.LoadFromFile(level.LevelGeometry);
						LevelData.LevelItems = new List<LevelItem>();
						for (int i = 0; i < LevelData.geo.COL.Count; i++)
							LevelData.LevelItems.Add(new LevelItem(LevelData.geo.COL[i], d3ddevice, i, selectedItems));
					}

					progress.StepProgress();
					progress.SetStep("Textures");

					LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
					LevelData.Textures = new Dictionary<string, Texture[]>();
					if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName))
					{
						BMPInfo[] TexBmps =
							TextureArchive.GetTextures(Path.Combine(syspath, LevelData.geo.TextureFileName) + ".PVM");
						Texture[] texs = new Texture[TexBmps.Length];
						for (int j = 0; j < TexBmps.Length; j++)
							texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.None, Pool.Managed);
						if (!LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
							LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
						if (!LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
							LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
						LevelData.leveltexs = LevelData.geo.TextureFileName;
					}

					progress.StepProgress();

					#region Start Positions

					progress.SetTaskAndStep("Setting up start positions...");

					LevelData.StartPositions = new StartPosItem[LevelData.Characters.Length];
					for (int i = 0; i < LevelData.StartPositions.Length; i++)
					{
						progress.SetStep(string.Format("{0}/{1}", (i + 1), LevelData.StartPositions.Length));

						IniCharInfo character;
						if (i == 0 && levelact.Level == SA1LevelIDs.PerfectChaos)
							character = ini.Characters["SuperSonic"];
						else
							character = ini.Characters[LevelData.Characters[i]];

						Dictionary<SA1LevelAct, SA1StartPosInfo> posini =
							SA1StartPosList.Load(character.StartPositions);

						Vertex pos = new Vertex();
						int rot = 0;

						if (posini.ContainsKey(levelact))
						{
							pos = posini[levelact].Position;
							rot = posini[levelact].YRotation;
						}
						LevelData.StartPositions[i] = new StartPosItem(new ModelFile(character.Model).Model,
							character.Textures, character.Height, pos, rot, d3ddevice, selectedItems);

						LoadTextureList(character.TextureList, syspath);
					}

					progress.StepProgress();

					#endregion

					#region Death Zones

					progress.SetTaskAndStep("Death Zones:", "Initializing...");

					if (string.IsNullOrEmpty(level.DeathZones))
						LevelData.DeathZones = null;
					else
					{
						LevelData.DeathZones = new List<DeathZoneItem>();
						DeathZoneFlags[] dzini = DeathZoneFlagsList.Load(level.DeathZones);
						string path = Path.GetDirectoryName(level.DeathZones);
						for (int i = 0; i < dzini.Length; i++)
						{
							progress.SetStep(String.Format("Loading model {0}/{1}", (i + 1), dzini.Length));

							LevelData.DeathZones.Add(new DeathZoneItem(
								new ModelFile(Path.Combine(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl"))
									.Model,
								dzini[i].Flags, d3ddevice, selectedItems));
						}
					}

					progress.StepProgress();

					#endregion

					#region Textures and Texture Lists

					progress.SetTaskAndStep("Loading textures for:");

					progress.SetStep("Common objects");
					// Loads common object textures (e.g OBJ_REGULAR)
					LoadTextureList(ini.ObjectTextureList, syspath);

					progress.SetStep("Mission objects");
					// Loads mission object textures
					LoadTextureList(ini.MissionTextureList, syspath);

					progress.SetTaskAndStep("Loading stage texture lists...");

					// Loads the textures in the texture list for this stage (e.g BEACH01)
					foreach (string file in Directory.GetFiles(ini.LevelTextureLists))
					{
						LevelTextureList texini = LevelTextureList.Load(file);
						if (texini.Level != levelact)
							continue;
						
						LoadTextureList(texini.TextureList, syspath);
					}

					progress.SetTaskAndStep("Loading textures for:", "Objects");
					// Object texture list(s)
					LoadTextureList(level.ObjectTextureList, syspath);

					progress.SetStep("Stage");
					// The stage textures... again? "Extra"?
					if (level.Textures != null && level.Textures.Length > 0)
						foreach (string tex in level.Textures)
						{
							LoadPVM(tex, syspath);

							if (string.IsNullOrEmpty(LevelData.leveltexs))
								LevelData.leveltexs = tex;
						}

					progress.StepProgress();

					#endregion

					#region Object Definitions / SET Layout

					progress.SetTaskAndStep("Loading Object Definitions:", "Parsing...");

					LevelData.ObjDefs = new List<ObjectDefinition>();
					Dictionary<string, ObjectData> objdefini =
						IniSerializer.Deserialize<Dictionary<string, ObjectData>>(ini.ObjectDefinitions);

					LevelData.MisnObjDefs = new List<ObjectDefinition>();

				if (!string.IsNullOrEmpty(level.ObjectList) && File.Exists(level.ObjectList))
				{
					List<ObjectData> objectErrors = new List<ObjectData>();
					ObjectListEntry[] objlstini = ObjectList.Load(level.ObjectList, false);
					Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;

					for (int ID = 0; ID < objlstini.Length; ID++)
					{
						string codeaddr = objlstini[ID].CodeString;

						if (!objdefini.ContainsKey(codeaddr))
							codeaddr = "0";

						ObjectData defgroup = objdefini[codeaddr];
						ObjectDefinition def;

						if (!string.IsNullOrEmpty(defgroup.CodeFile))
						{
							progress.SetStep("Compiling: " + defgroup.CodeFile);

							// TODO: Split this out to a function
							#region Compile object code files

							string ty = defgroup.CodeType;
							string dllfile = Path.Combine("dllcache", ty + ".dll");
							DateTime modDate = DateTime.MinValue;
							if (File.Exists(dllfile))
								modDate = File.GetLastWriteTime(dllfile);
							string fp = defgroup.CodeFile.Replace('/', Path.DirectorySeparatorChar);
							if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
								def =
									(ObjectDefinition)
										Activator.CreateInstance(
											Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
												.GetType(ty));
							else
							{
								string ext = Path.GetExtension(fp);
								CodeDomProvider pr = null;
								switch (ext.ToLowerInvariant())
								{
									case ".cs":
										pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
										break;
									case ".vb":
										pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
										break;
								}
								if (pr != null)
								{
									// System, System.Core, System.Drawing, Microsoft.DirectX, Microsoft.DirectX.Direct3D, Microsoft.DirectX.Direct3DX,
									// SADXLVL2, SAModel, SAModel.Direct3D, SA Tools, SAEditorCommon
									CompilerParameters para =
										new CompilerParameters(new string[]
										{
												"System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof (Vector3)).Location,
												Assembly.GetAssembly(typeof (Texture)).Location, Assembly.GetAssembly(typeof (D3DX)).Location,
												Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof (LandTable)).Location,
												Assembly.GetAssembly(typeof (EditorCamera)).Location, Assembly.GetAssembly(typeof (SA1LevelAct)).Location,
												Assembly.GetAssembly(typeof (ObjectDefinition)).Location
										})
										{
											GenerateExecutable = false,
											GenerateInMemory = false,
											IncludeDebugInformation = true,
											OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
										};
									CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
									if (res.Errors.HasErrors)
									{
										// TODO: Merge with existing object error handler. I add too many ToDos.
										string errors = null;
										foreach (CompilerError item in res.Errors)
											errors += String.Format("\n\n{0}, {1}: {2}", item.Line, item.Column, item.ErrorText);

										MessageBox.Show("Failed to compile object code file:\n" + defgroup.CodeFile + errors,
											"Object compilation failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

										def = new DefaultObjectDefinition();
									}
									else
									{
										def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
									}
								}
								else
									def = new DefaultObjectDefinition();
							}


							#endregion
						}
						else
						{
							def = new DefaultObjectDefinition();
						}

						LevelData.ObjDefs.Add(def);

						// The only reason .Model is checked for null is for objects that don't yet have any
						// models defined for them. It would be annoying seeing that error all the time!
						if (string.IsNullOrEmpty(defgroup.CodeFile) && !string.IsNullOrEmpty(defgroup.Model))
						{
							progress.SetStep("Loading: " + defgroup.Model);
							// Otherwise, if the model file doesn't exist and/or no texture file is defined,
							// load the "default object" instead ("?").
							if (!File.Exists(defgroup.Model) || string.IsNullOrEmpty(defgroup.Texture) ||
								!LevelData.Textures.ContainsKey(defgroup.Texture))
							{
								ObjectData error = new ObjectData { Name = defgroup.Name, Model = defgroup.Model, Texture = defgroup.Texture };
								objectErrors.Add(error);
								defgroup.Model = null;
							}
						}

						def.Init(defgroup, objlstini[ID].Name, d3ddevice);
						def.SetInternalName(objlstini[ID].Name);
					}

					// Loading SET Layout
					progress.SetTaskAndStep("Loading SET items", "Initializing...");

					if (LevelData.ObjDefs.Count > 0)
					{
						LevelData.SETName = level.SETName ?? level.LevelID;
						string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + "{0}.bin");
						LevelData.SETItems = new List<SETItem>[LevelData.SETChars.Length];
						for (int i = 0; i < LevelData.SETChars.Length; i++)
						{
							List<SETItem> list = new List<SETItem>();
							byte[] setfile = null;

							string formatted = string.Format(setstr, LevelData.SETChars[i]);

							if (modpath != null && File.Exists(Path.Combine(modpath, formatted)))
								setfile = File.ReadAllBytes(Path.Combine(modpath, formatted));
							else if (File.Exists(formatted))
								setfile = File.ReadAllBytes(formatted);

							if (setfile != null)
							{
								progress.SetTask("SET: " + formatted.Replace(Environment.CurrentDirectory, ""));

								int count = BitConverter.ToInt32(setfile, 0);
								int address = 0x20;
								for (int j = 0; j < count; j++)
								{
									progress.SetStep(string.Format("{0}/{1}", (j + 1), count));

									SETItem ent = new SETItem(setfile, address, selectedItems);
									list.Add(ent);
									address += 0x20;
								}
							}
							LevelData.SETItems[i] = list;
						}
					}
					else
					{
						LevelData.SETItems = null;
					}

					// Checks if there have been any errors added to the error list and does its thing
					// This thing is a mess. If anyone can think of a cleaner way to do this, be my guest.
					if (objectErrors.Count > 0)
					{
						int count = objectErrors.Count;
						List<string> errorStrings = new List<string> { "The following objects failed to load:" };

						foreach (ObjectData o in objectErrors)
						{
							bool texEmpty = string.IsNullOrEmpty(o.Texture);
							bool texExists = (!string.IsNullOrEmpty(o.Texture) && LevelData.Textures.ContainsKey(o.Texture));
							errorStrings.Add("");
							errorStrings.Add("Object:\t\t" + o.Name);
							errorStrings.Add("\tModel:");
							errorStrings.Add("\t\tName:\t" + o.Model);
							errorStrings.Add("\t\tExists:\t" + File.Exists(o.Model));
							errorStrings.Add("\tTexture:");
							errorStrings.Add("\t\tName:\t" + ((texEmpty) ? "(N/A)" : o.Texture));
							errorStrings.Add("\t\tExists:\t" + texExists);
						}

						// TODO: Proper logging. Who knows where this file may end up
						File.WriteAllLines("SADXLVL2.log", errorStrings.ToArray());

						MessageBox.Show(count + ((count == 1) ? " object" : " objects") + " failed to load their model(s).\n"
										+
										"\nThe level will still display, but the objects in question will not display their proper models." +
										"\n\nPlease check the log for details.",
							"Error loading models", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
				else
				{
					LevelData.SETItems = null;
				}

				if (!string.IsNullOrEmpty(ini.MissionObjectList) && File.Exists(ini.MissionObjectList))
				{
					List<ObjectData> objectErrors = new List<ObjectData>();
					ObjectListEntry[] objlstini = ObjectList.Load(ini.MissionObjectList, false);

					for (int ID = 0; ID < objlstini.Length; ID++)
					{
						string codeaddr = objlstini[ID].CodeString;

						if (!objdefini.ContainsKey(codeaddr))
							codeaddr = "0";

						ObjectData defgroup = objdefini[codeaddr];
						ObjectDefinition def;

						if (!string.IsNullOrEmpty(defgroup.CodeFile))
						{
							progress.SetStep("Compiling: " + defgroup.CodeFile);

							// TODO: Split this out to a function
							#region Compile object code files

							string ty = defgroup.CodeType;
							string dllfile = Path.Combine("dllcache", ty + ".dll");
							DateTime modDate = DateTime.MinValue;
							if (File.Exists(dllfile))
								modDate = File.GetLastWriteTime(dllfile);
							string fp = defgroup.CodeFile.Replace('/', Path.DirectorySeparatorChar);
							if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
								def =
									(ObjectDefinition)
										Activator.CreateInstance(
											Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
												.GetType(ty));
							else
							{
								string ext = Path.GetExtension(fp);
								CodeDomProvider pr = null;
								switch (ext.ToLowerInvariant())
								{
									case ".cs":
										pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
										break;
									case ".vb":
										pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
										break;
								}
								if (pr != null)
								{
									CompilerParameters para =
										new CompilerParameters(new string[]
										{
												"System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof (Vector3)).Location,
												Assembly.GetAssembly(typeof (Texture)).Location, Assembly.GetAssembly(typeof (D3DX)).Location,
												Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof (LandTable)).Location,
												Assembly.GetAssembly(typeof (EditorCamera)).Location, Assembly.GetAssembly(typeof (SA1LevelAct)).Location,
												Assembly.GetAssembly(typeof (ObjectDefinition)).Location
										})
										{
											GenerateExecutable = false,
											GenerateInMemory = false,
											IncludeDebugInformation = true,
											OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
										};
									CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
									if (res.Errors.HasErrors)
									{
										// TODO: Merge with existing object error handler. I add too many ToDos.
										string errors = null;
										foreach (CompilerError item in res.Errors)
											errors += String.Format("\n\n{0}, {1}: {2}", item.Line, item.Column, item.ErrorText);

										MessageBox.Show("Failed to compile object code file:\n" + defgroup.CodeFile + errors,
											"Object compilation failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

										def = new DefaultObjectDefinition();
									}
									else
									{
										def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
									}
								}
								else
									def = new DefaultObjectDefinition();
							}


							#endregion
						}
						else
						{
							def = new DefaultObjectDefinition();
						}

						LevelData.MisnObjDefs.Add(def);

						// The only reason .Model is checked for null is for objects that don't yet have any
						// models defined for them. It would be annoying seeing that error all the time!
						if (string.IsNullOrEmpty(defgroup.CodeFile) && !string.IsNullOrEmpty(defgroup.Model))
						{
							progress.SetStep("Loading: " + defgroup.Model);
							// Otherwise, if the model file doesn't exist and/or no texture file is defined,
							// load the "default object" instead ("?").
							if (!File.Exists(defgroup.Model) || string.IsNullOrEmpty(defgroup.Texture) ||
								!LevelData.Textures.ContainsKey(defgroup.Texture))
							{
								ObjectData error = new ObjectData { Name = defgroup.Name, Model = defgroup.Model, Texture = defgroup.Texture };
								objectErrors.Add(error);
								defgroup.Model = null;
							}
						}

						def.Init(defgroup, objlstini[ID].Name, d3ddevice);
						def.SetInternalName(objlstini[ID].Name);
					}

					// Loading SET Layout
					progress.SetTaskAndStep("Loading Mission SET items", "Initializing...");

					if (LevelData.MisnObjDefs.Count > 0)
					{
						string setstr = Path.Combine(syspath, "SETMI" + level.LevelID + "{0}.bin");
						string prmstr = Path.Combine(syspath, "PRMMI" + level.LevelID + "{0}.bin");
						LevelData.MissionSETItems = new List<MissionSETItem>[LevelData.SETChars.Length];
						for (int i = 0; i < LevelData.SETChars.Length; i++)
						{
							List<MissionSETItem> list = new List<MissionSETItem>();
							byte[] setfile = null;
							byte[] prmfile = null;

							string setfmt = string.Format(setstr, LevelData.SETChars[i]);
							string prmfmt = string.Format(prmstr, LevelData.SETChars[i]);

							if (modpath != null && File.Exists(Path.Combine(modpath, setfmt)) && File.Exists(Path.Combine(modpath, prmfmt)))
							{
								setfile = File.ReadAllBytes(Path.Combine(modpath, setfmt));
								prmfile = File.ReadAllBytes(Path.Combine(modpath, prmfmt));
							}
							else if (File.Exists(setfmt) && File.Exists(prmfmt))
							{
								setfile = File.ReadAllBytes(setfmt);
								prmfile = File.ReadAllBytes(prmfmt);
							}

							if (setfile != null)
							{
								progress.SetTask("SET: " + setfmt.Replace(Environment.CurrentDirectory, ""));

								int count = BitConverter.ToInt32(setfile, 0);
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
							}
							LevelData.MissionSETItems[i] = list;
						}
					}
					else
					{
						LevelData.MissionSETItems = null;
					}

					// Checks if there have been any errors added to the error list and does its thing
					// This thing is a mess. If anyone can think of a cleaner way to do this, be my guest.
					if (objectErrors.Count > 0)
					{
						int count = objectErrors.Count;
						List<string> errorStrings = new List<string> { "The following objects failed to load:" };

						foreach (ObjectData o in objectErrors)
						{
							bool texEmpty = string.IsNullOrEmpty(o.Texture);
							bool texExists = (!string.IsNullOrEmpty(o.Texture) && LevelData.Textures.ContainsKey(o.Texture));
							errorStrings.Add("");
							errorStrings.Add("Object:\t\t" + o.Name);
							errorStrings.Add("\tModel:");
							errorStrings.Add("\t\tName:\t" + o.Model);
							errorStrings.Add("\t\tExists:\t" + File.Exists(o.Model));
							errorStrings.Add("\tTexture:");
							errorStrings.Add("\t\tName:\t" + ((texEmpty) ? "(N/A)" : o.Texture));
							errorStrings.Add("\t\tExists:\t" + texExists);
						}

						// TODO: Proper logging. Who knows where this file may end up
						File.WriteAllLines("SADXLVL2.log", errorStrings.ToArray());

						MessageBox.Show(count + ((count == 1) ? " object" : " objects") + " failed to load their model(s).\n"
										+
										"\nThe level will still display, but the objects in question will not display their proper models." +
										"\n\nPlease check the log for details.",
							"Error loading models", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

					string camstr = Path.Combine(syspath, "CAM" + LevelData.SETName + "{0}.bin");

					LevelData.CAMItems = new List<CAMItem>[LevelData.SETChars.Length];
					for (int i = 0; i < LevelData.SETChars.Length; i++)
					{
						List<CAMItem> list = new List<CAMItem>();
						byte[] camfile = null;

						string formatted = string.Format(camstr, LevelData.SETChars[i]);

						if (modpath != null && File.Exists(Path.Combine(modpath, formatted)))
							camfile = File.ReadAllBytes(Path.Combine(modpath, formatted));
						else if (File.Exists(formatted))
							camfile = File.ReadAllBytes(formatted);

						if (camfile != null)
						{
							progress.SetTask("CAM: " + formatted.Replace(Environment.CurrentDirectory, ""));

							int count = BitConverter.ToInt32(camfile, 0);
							int address = 0x40;
							for (int j = 0; j < count; j++)
							{
								progress.SetStep(string.Format("{0}/{1}", (j + 1), count));

								CAMItem ent = new CAMItem(camfile, address, selectedItems);
								list.Add(ent);
								address += 0x40;
							}
						}

						LevelData.CAMItems[i] = list;
					}

					CAMItem.Init(d3ddevice);

					progress.StepProgress();

					#endregion

					#region Loading Level Effects

					LevelData.leveleff = null;
					if (!string.IsNullOrEmpty(level.Effects))
					{
						progress.SetTaskAndStep("Loading Level Effects...");

						LevelDefinition def = null;
						string ty = "SADXObjectDefinitions.Level_Effects." + Path.GetFileNameWithoutExtension(level.Effects);
						string dllfile = Path.Combine("dllcache", ty + ".dll");
						DateTime modDate = DateTime.MinValue;

						if (File.Exists(dllfile))
							modDate = File.GetLastWriteTime(dllfile);

						string fp = level.Effects.Replace('/', Path.DirectorySeparatorChar);
						if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
						{
							def =
								(LevelDefinition)
									Activator.CreateInstance(
										Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
						}
						else
						{
							string ext = Path.GetExtension(fp);
							CodeDomProvider pr = null;
							switch (ext.ToLowerInvariant())
							{
								case ".cs":
									pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
									break;
								case ".vb":
									pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
									break;
							}
							if (pr != null)
							{
								CompilerParameters para =
									new CompilerParameters(new string[]
								{
									"System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof (Vector3)).Location,
									Assembly.GetAssembly(typeof (Texture)).Location, Assembly.GetAssembly(typeof (D3DX)).Location,
									Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof (LandTable)).Location,
									Assembly.GetAssembly(typeof (EditorCamera)).Location, Assembly.GetAssembly(typeof (SA1LevelAct)).Location,
									Assembly.GetAssembly(typeof (Item)).Location
								})
									{
										GenerateExecutable = false,
										GenerateInMemory = false,
										IncludeDebugInformation = true,
										OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
									};
								CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
								if (!res.Errors.HasErrors)
									def = (LevelDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
							}
						}

						if (def != null)
							def.Init(level, levelact.Act, d3ddevice);

						LevelData.leveleff = def;
					}

					progress.StepProgress();

					#endregion

					#region Loading Splines

					LevelData.LevelSplines = new List<SplineData>();
					SplineData.Init();

					if (!string.IsNullOrEmpty(ini.Paths))
					{
						progress.SetTaskAndStep("Reticulating splines...");

						String splineDirectory = Path.Combine(Path.Combine(Environment.CurrentDirectory, ini.Paths),
							levelact.ToString());

						if (Directory.Exists(splineDirectory))
						{
							List<string> pathFiles = new List<string>();

							for (int i = 0; i < int.MaxValue; i++)
							{
								string path = string.Concat(splineDirectory, string.Format("/{0}.ini", i));
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

					if ((stageLightList != null) && (stageLightList.Count > 0))
					{
						List<SA1StageLightData> lightList = new List<SA1StageLightData>();

						foreach (SA1StageLightData lightData in stageLightList)
						{
							if ((lightData.Level == levelact.Level) && (lightData.Act == levelact.Act))
								lightList.Add(lightData);
						}

						if (lightList.Count > 0)
						{
							for (int i = 0; i < d3ddevice.Lights.Count; i++) // clear all default lights
							{
								d3ddevice.Lights[i].Enabled = false;
							}

							for (int i = 0; i < lightList.Count; i++)
							{
								SA1StageLightData lightData = lightList[i];

								d3ddevice.Lights[i].Enabled = true;
								d3ddevice.Lights[i].Type = (lightData.UseDirection) ? LightType.Directional : LightType.Point;
								d3ddevice.Lights[i].Diffuse = lightData.RGB.ToColor();
								d3ddevice.Lights[i].DiffuseColor = new ColorValue(lightData.RGB.X, lightData.RGB.Y, lightData.RGB.Z, 1.0f);
								d3ddevice.Lights[i].Ambient = lightData.AmbientRGB.ToColor();
								d3ddevice.Lights[i].Specular = Color.Black;
								d3ddevice.Lights[i].Direction = lightData.Direction.ToVector3();
								d3ddevice.Lights[i].Range = lightData.Dif; // guessing here
							}
						}
						else
						{
							MessageBox.Show("No lights were found for this stage. Using default lights instead.", "No lights found",
								MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}

					progress.StepProgress();
					#endregion

					transformGizmo = new TransformGizmo();

					Invoke((Action)progress.Close);
				}
#if !DEBUG
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					ex.GetType().Name + ": " + ex.Message + "\nLog file has been saved to " + Path.Combine(Environment.CurrentDirectory, "SADXLVL2.log") + ".\nSend this to MainMemory on the Sonic Retro forums.",
					"SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				File.WriteAllText("SADXLVL2.log", ex.ToString());
				initerror = true;
			}
#endif
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (initerror)
			{
				Close();
				return;
			}

			bool isGeometryPresent = LevelData.geo != null;
			bool isSETPreset = LevelData.SETItems != null;
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
			exportOBJToolStripMenuItem.Enabled = isGeometryPresent;

			// Edit menu
			// Clear Level
			clearLevelToolStripMenuItem.Enabled = isGeometryPresent;
			// SET Items submenu
			// Gotta clear up these names at some point...
			// Drop the 1, and you get the dropdown menu under View.
			sETItemsToolStripMenuItem1.Enabled = true;
			// Duplicate
			duplicateToolStripMenuItem.Enabled = true;
			// Calculate All Bounds
			calculateAllBoundsToolStripMenuItem.Enabled = isGeometryPresent;

			// The whole view menu!
			viewToolStripMenuItem.Enabled = true;
			statsToolStripMenuItem.Enabled = isGeometryPresent;
			deathZonesToolStripMenuItem.Checked = deathZonesToolStripMenuItem.Enabled = deathZoneToolStripMenuItem.Enabled = isDeathZonePresent;

			isStageLoaded = true;
			selectedItems.SelectionChanged += SelectionChanged;
			UseWaitCursor = false;
			Enabled = true;

			gizmoSpaceComboBox.Enabled = true;
			gizmoSpaceComboBox.SelectedIndex = 0;

			toolStrip1.Enabled = isStageLoaded;
			LevelData_StateChanged();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isStageLoaded)
			{
				if (SavePrompt(true) == DialogResult.Cancel)
					e.Cancel = true;

				LevelData.StateChanged -= LevelData_StateChanged;
			}
			Settings.Save();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveStage(false);
		}

		/// <summary>
		/// Saves changes made to the currently loaded stage.
		/// </summary>
		/// <param name="autoCloseDialog">Defines whether or not the progress dialog should close on completion.</param>
		private void SaveStage(bool autoCloseDialog)
		{
			if (!isStageLoaded)
				return;

			ProgressDialog progress = new ProgressDialog("Saving stage: " + levelName, 6, true, autoCloseDialog);
			progress.Show(this);
			Application.DoEvents();

			IniLevelData level = ini.Levels[levelID];
			string syspath = Path.Combine(Environment.CurrentDirectory, ini.SystemPath);
			string modpath = ini.ModPath;
			SA1LevelAct levelact = new SA1LevelAct(level.LevelID);

			progress.SetTaskAndStep("Saving:", "Geometry...");

			if (LevelData.geo != null)
			{
				LevelData.geo.Tool = "SADXLVL2";
				LevelData.geo.SaveToFile(level.LevelGeometry, LandTableFormat.SA1);
			}

			progress.StepProgress();

			progress.Step = "Start positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				Dictionary<SA1LevelAct, SA1StartPosInfo> posini =
					SA1StartPosList.Load(ini.Characters[LevelData.Characters[i]].StartPositions);

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
				posini.Save(ini.Characters[LevelData.Characters[i]].StartPositions);
			}

			progress.StepProgress();

			progress.Step = "Death zones...";
			Application.DoEvents();

			if (LevelData.DeathZones != null)
			{
				DeathZoneFlags[] dzini = new DeathZoneFlags[LevelData.DeathZones.Count];
				string path = Path.GetDirectoryName(level.DeathZones);
				for (int i = 0; i < LevelData.DeathZones.Count; i++)
					dzini[i] = LevelData.DeathZones[i].Save(path, i);
				dzini.Save(level.DeathZones);
			}

			progress.StepProgress();

			#region Saving SET Items

			progress.Step = "SET items...";
			Application.DoEvents();

			if (LevelData.SETItems != null)
			{
				for (int i = 0; i < LevelData.SETItems.Length; i++)
				{
					string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
					if (modpath != null)
						setstr = Path.Combine(modpath, setstr);

					// TODO: Consider simply blanking the SET file instead of deleting it.
					// Completely deleting it might be undesirable since Sonic's layout will be loaded
					// in place of the missing file. And where mods are concerned, you could have conflicts
					// with other mods if the file is deleted.
					if (File.Exists(setstr))
						File.Delete(setstr);
					if (LevelData.SETItems[i].Count == 0)
						continue;

					List<byte> file = new List<byte>(LevelData.SETItems[i].Count*0x20 + 0x20);
					file.AddRange(BitConverter.GetBytes(LevelData.SETItems[i].Count));
					file.Align(0x20);

					foreach (SETItem item in LevelData.SETItems[i])
						file.AddRange(item.GetBytes());

					File.WriteAllBytes(setstr, file.ToArray());
				}
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
					string camString = Path.Combine(syspath, "CAM" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
					if (modpath != null)
						camString = Path.Combine(modpath, camString);

					// TODO: Handle this differently. File stream? If the user is using a symbolic link for example, we defeat the purpose by deleting it.
					if (File.Exists(camString))
						File.Delete(camString);

					if (LevelData.CAMItems[i].Count == 0)
						continue;

					List<byte> file = new List<byte>(LevelData.CAMItems[i].Count*0x40 + 0x40); // setting up file size and header
					file.AddRange(BitConverter.GetBytes(LevelData.CAMItems[i].Count));
					file.Align(0x40);


					foreach (CAMItem item in LevelData.CAMItems[i]) // outputting individual components
						file.AddRange(item.GetBytes());

					File.WriteAllBytes(camString, file.ToArray());
				}
			}

			progress.StepProgress();
			progress.SetTaskAndStep("Save complete!");
			Application.DoEvents();

			#endregion

			#region Saving Mission SET Items

			progress.Step = "Mission SET items...";
			Application.DoEvents();

			if (LevelData.MissionSETItems != null)
			{
				for (int i = 0; i < LevelData.MissionSETItems.Length; i++)
				{
					string setstr = Path.Combine(syspath, "SETMI" + level.LevelID + LevelData.SETChars[i] + ".bin");
					string prmstr = Path.Combine(syspath, "PRMMI" + level.LevelID + LevelData.SETChars[i] + ".bin");
					if (modpath != null)
					{
						setstr = Path.Combine(modpath, setstr);
						prmstr = Path.Combine(modpath, prmstr);
					}

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
		}


		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		internal void DrawLevel()
		{
			if (!isStageLoaded)
				return;

			cam.FOV = (float)(Math.PI / 4);
			cam.Aspect = panel1.Width / (float)panel1.Height;
			cam.DrawDistance = 100000;
			UpdateTitlebar();

			#region D3D Parameters
			d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
			d3ddevice.SetRenderState(RenderStates.FillMode, (int)EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderStates.CullMode, (int)EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = Color.White };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 1, 0);
			d3ddevice.RenderState.ZBufferEnable = true;
			#endregion

			d3ddevice.BeginScene();
			//all drawings after this line
			MatrixStack transform = new MatrixStack();
			if (LevelData.leveleff != null & backgroundToolStripMenuItem.Checked)
				LevelData.leveleff.Render(d3ddevice, cam);

			cam.DrawDistance = EditorOptions.RenderDrawDistance;
			d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
			cam.BuildFrustum(d3ddevice.Transform.View, d3ddevice.Transform.Projection);

			EditorOptions.RenderStateCommonSetup(d3ddevice);

			List<RenderInfo> renderlist = new List<RenderInfo>();

			#region Adding Level Geometry
			if (LevelData.LevelItems != null)
			{
				foreach (LevelItem item in LevelData.LevelItems)
				{
					bool display = false;

					if (visibleToolStripMenuItem.Checked && item.Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !item.Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;

					if (display)
						renderlist.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			renderlist.AddRange(LevelData.StartPositions[LevelData.Character].Render(d3ddevice, cam, transform));

			#region Adding splines
			if (splinesToolStripMenuItem.Checked)
			{
				foreach (SplineData spline in LevelData.LevelSplines)
					renderlist.AddRange(spline.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding SET Layout
			if (LevelData.SETItems != null && sETITemsToolStripMenuItem.Checked)
			{
				foreach (SETItem item in LevelData.SETItems[LevelData.Character])
					renderlist.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding Death Zones
			if (LevelData.DeathZones != null & deathZonesToolStripMenuItem.Checked)
			{
				foreach (DeathZoneItem item in LevelData.DeathZones)
				{
					if (item.Visible)
						renderlist.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			#region Adding CAM Layout
			if (LevelData.CAMItems != null && cAMItemsToolStripMenuItem.Checked)
			{
				foreach (CAMItem item in LevelData.CAMItems[LevelData.Character])
					renderlist.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding SET Layout
			if (LevelData.MissionSETItems != null && missionSETItemsToolStripMenuItem.Checked)
			{
				foreach (MissionSETItem item in LevelData.MissionSETItems[LevelData.Character])
					renderlist.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			RenderInfo.Draw(renderlist, d3ddevice, cam);

			d3ddevice.EndScene(); // scene drawings go before this line

			#region Draw Helper Objects
			foreach (PointHelper pointHelper in PointHelper.Instances)
			{
				pointHelper.DrawBox(d3ddevice, cam);
			}

			transformGizmo.Draw(d3ddevice, cam);

			foreach (PointHelper pointHelper in PointHelper.Instances)
			{
				pointHelper.Draw(d3ddevice, cam);
			}
			#endregion

			d3ddevice.Present();
		}

		private void UpdateTitlebar()
		{
			Text = "SADXLVL2 - " + levelName + " (" + cam.Position.X + ", " + cam.Position.Y + ", " + cam.Position.Z
				+ " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X")
				+ " Speed=" + cam.MoveSpeed + (cam.mode == 1 ? " Distance=" + cam.Distance : "") + ")";
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			DrawLevel();
		}

		private void LevelData_PointOperation()
		{
			MessageBox.Show(this, "You have just begun a Point To operation. Left click on the point or item you want the selected item(s) to point to, or right click to cancel.", "SADXLVL2", MessageBoxButtons.OK, MessageBoxIcon.Information);
			isPointOperation = true;
		}

		#region User Keyboard / Mouse Methods
		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (!isStageLoaded)
				return;

			switch (e.Button)
			{
				// If the mouse button pressed is not one we're looking for,
				// we can avoid re-drawing the scene by bailing out.
				default:
					return;

				case MouseButtons.Left:
					Item item;
					if (isPointOperation)
					{
						HitResult res = PickItem(e.Location, out item);
						if (item != null)
						{
							using (PointToDialog dlg = new PointToDialog(res.Position.ToVertex(), item.Position))
								if (dlg.ShowDialog(this) == DialogResult.OK)
									foreach (SETItem it in System.Linq.Enumerable.OfType<SETItem>(selectedItems.GetSelection()))
										it.PointTo(dlg.SelectedLocation);
							isPointOperation = false;
						}
						return;
					}
					// If we have any helpers selected, don't execute the rest of the method!
					if (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE) return;

					foreach (PointHelper pointHelper in PointHelper.Instances) if (pointHelper.SelectedAxes != GizmoSelectedAxes.NONE) return;

					PickItem(e.Location, out item);

					if (item != null)
					{
						if (ModifierKeys == Keys.Control)
						{
							if (selectedItems.GetSelection().Contains(item))
								selectedItems.Remove(item);
							else
								selectedItems.Add(item);
						}
						else if (!selectedItems.GetSelection().Contains(item))
						{
							selectedItems.Clear();
							selectedItems.Add(item);
						}
					}
					else if ((ModifierKeys & Keys.Control) == 0)
					{
						selectedItems.Clear();
					}
					break;

				case MouseButtons.Right:
					if (isPointOperation)
					{
						isPointOperation = false;
						return;
					}
					bool cancopy = false;
					foreach (Item obj in selectedItems.GetSelection())
					{
						if (obj.CanCopy)
							cancopy = true;
					}
					if (cancopy)
					{
						/*cutToolStripMenuItem.Enabled = true;
						copyToolStripMenuItem.Enabled = true;*/
						deleteToolStripMenuItem.Enabled = true;

						cutToolStripMenuItem.Enabled = false;
						copyToolStripMenuItem.Enabled = false;
					}
					else
					{
						cutToolStripMenuItem.Enabled = false;
						copyToolStripMenuItem.Enabled = false;
						deleteToolStripMenuItem.Enabled = false;
					}
					pasteToolStripMenuItem.Enabled = false;
					menuLocation = e.Location;
					contextMenuStrip1.Show(panel1, e.Location);
					break;
			}

			LevelData_StateChanged();
		}

		private HitResult PickItem(Point mouse)
		{
			return PickItem(mouse, out Item item);
		}

		private HitResult PickItem(Point mouse, out Item item)
		{
			HitResult closesthit = HitResult.NoHit;
			HitResult hit;
			item = null;
			Vector3 mousepos = new Vector3(mouse.X, mouse.Y, 0);
			Viewport viewport = d3ddevice.Viewport;
			Matrix proj = d3ddevice.Transform.Projection;
			Matrix view = d3ddevice.Transform.View;
			Vector3 Near, Far;
			Near = mousepos;
			Near.Z = 0;
			Far = Near;
			Far.Z = -1;


			#region Picking Level Items
			if (LevelData.LevelItems != null)
			{
				for (int i = 0; i < LevelData.LevelItems.Count; i++)
				{
					bool display = false;
					if (visibleToolStripMenuItem.Checked && LevelData.LevelItems[i].Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !LevelData.LevelItems[i].Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;
					if (display)
					{
						hit = LevelData.LevelItems[i].CheckHit(Near, Far, viewport, proj, view);
						if (hit < closesthit)
						{
							closesthit = hit;
							item = LevelData.LevelItems[i];
						}
					}
				}
			}
			#endregion

			#region Picking Start Positions
			hit = LevelData.StartPositions[LevelData.Character].CheckHit(Near, Far, viewport, proj, view);
			if (hit < closesthit)
			{
				closesthit = hit;
				item = LevelData.StartPositions[LevelData.Character];
			}
			#endregion

			#region Picking SET Items
			if (LevelData.SETItems != null && sETITemsToolStripMenuItem.Checked)
				foreach (SETItem setitem in LevelData.SETItems[LevelData.Character])
				{
					hit = setitem.CheckHit(Near, Far, viewport, proj, view);
					if (hit < closesthit)
					{
						closesthit = hit;
						item = setitem;
					}
				}
			#endregion

			#region Picking CAM Items
			if ((LevelData.CAMItems != null) && (cAMItemsToolStripMenuItem.Checked))
			{
				foreach (CAMItem camItem in LevelData.CAMItems[LevelData.Character])
				{
					hit = camItem.CheckHit(Near, Far, viewport, proj, view);
					if (hit < closesthit)
					{
						closesthit = hit;
						item = camItem;
					}
				}
			}
			#endregion

			#region Picking Death Zones

			if (LevelData.DeathZones != null)
			{
				foreach (DeathZoneItem dzitem in LevelData.DeathZones)
				{
					if (dzitem.Visible & deathZonesToolStripMenuItem.Checked)
					{
						hit = dzitem.CheckHit(Near, Far, viewport, proj, view);
						if (hit < closesthit)
						{
							closesthit = hit;
							item = dzitem;
						}
					}
				}
			}

			#endregion

			#region Picking Mission SET Items
			if (LevelData.MissionSETItems != null && missionSETItemsToolStripMenuItem.Checked)
				foreach (MissionSETItem setitem in LevelData.MissionSETItems[LevelData.Character])
				{
					hit = setitem.CheckHit(Near, Far, viewport, proj, view);
					if (hit < closesthit)
					{
						closesthit = hit;
						item = setitem;
					}
				}
			#endregion

			#region Picking Splines
			if ((LevelData.LevelSplines != null) && (splinesToolStripMenuItem.Checked))
			{
				foreach (SplineData spline in LevelData.LevelSplines)
				{
					hit = spline.CheckHit(Near, Far, viewport, proj, view);

					if (hit < closesthit)
					{
						closesthit = hit;
						item = spline;
					}
				}
			}
			#endregion

			return closesthit;
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			UpdatePropertyGrid();
		}

		private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
					e.IsInputKey = true;
					break;
			}
		}

		private void panel1_KeyUp(object sender, KeyEventArgs e)
		{
			lookKeyDown = e.Alt;
			zoomKeyDown = e.Control;
		}

		private void panel1_KeyDown(object sender, KeyEventArgs e)
		{
			if (!isStageLoaded)
				return;

			bool draw = false;

			if (cam.mode == 0)
			{
				if (e.KeyCode == Keys.E)
				{
					cam.Position = new Vector3();
					draw = true;
				}

				if (e.KeyCode == Keys.R)
				{
					cam.Pitch = 0;
					cam.Yaw = 0;
					draw = true;
				}
			}

			if ((lookKeyDown = e.Alt) && panel1.ContainsFocus)
				e.Handled = false;
			zoomKeyDown = e.Control;

			switch (e.KeyCode)
			{
				case Keys.X:
					cam.mode = (cam.mode + 1) % 2;

					if (cam.mode == 1)
					{
						if (selectedItems.GetSelection().Count > 0)
							cam.FocalPoint = Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3();
						else
							cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
					}

					draw = true;
					break;

				case Keys.Z:
					if(selectedItems.ItemCount > 1)
					{
						BoundingSphere combinedBounds = selectedItems.GetSelection()[0].Bounds;

						for (int i = 0; i < selectedItems.ItemCount; i++)
						{
							combinedBounds = Direct3D.Extensions.Merge(combinedBounds, selectedItems.GetSelection()[i].Bounds);
						}

						cam.MoveToShowBounds(combinedBounds);
					}
					else if (selectedItems.ItemCount == 1)
					{
						cam.MoveToShowBounds(selectedItems.GetSelection()[0].Bounds);
					}

					draw = true;
					break;

				case Keys.N:
					if (EditorOptions.RenderFillMode == FillMode.Solid)
						EditorOptions.RenderFillMode = FillMode.Point;
					else
						EditorOptions.RenderFillMode += 1;

					draw = true;
					break;

				case Keys.Delete:
					foreach (Item item in selectedItems.GetSelection())
						item.Delete();
					selectedItems.Clear();
					draw = true;
					break;

				case Keys.Add:
					cam.MoveSpeed += 0.0625f;
					UpdateTitlebar();
					break;

				case Keys.Subtract:
					cam.MoveSpeed -= 0.0625f;
					UpdateTitlebar();
					break;

				case Keys.Enter:
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					UpdateTitlebar();
					break;

				case Keys.Tab:
					if (isStageLoaded && e.Control)
					{
						if (e.Shift)
							--LevelData.Character;
						else
							LevelData.Character = (LevelData.Character + 1) % 6;

						if (LevelData.Character < 0)
							LevelData.Character = 5;

						characterToolStripMenuItem.DropDownItems[LevelData.Character].PerformClick();
					}
					break;

			}

			if (draw)
				DrawLevel();
		}

		Point mouseLast;
		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!isStageLoaded)
				return;

			Point mouseEvent = e.Location;
			if (mouseLast == Point.Empty)
			{
				mouseLast = mouseEvent;
				return;
			}

			Point mouseDelta = mouseEvent - (Size)mouseLast;
			bool performedWrap = false;

			if (e.Button != MouseButtons.None)
			{
				Rectangle mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : panel1.RectangleToScreen(panel1.Bounds);

				if (Cursor.Position.X < (mouseBounds.Left + mouseWrapThreshold))
				{
					Cursor.Position = new Point(mouseBounds.Right - mouseWrapThreshold, Cursor.Position.Y);
					mouseEvent = new Point(mouseEvent.X + mouseBounds.Width - mouseWrapThreshold, mouseEvent.Y);
					performedWrap = true;
				}
				else if (Cursor.Position.X > (mouseBounds.Right - mouseWrapThreshold))
				{
					Cursor.Position = new Point(mouseBounds.Left + mouseWrapThreshold, Cursor.Position.Y);
					mouseEvent = new Point(mouseEvent.X - mouseBounds.Width + mouseWrapThreshold, mouseEvent.Y);
					performedWrap = true;
				}
				if (Cursor.Position.Y < (mouseBounds.Top + mouseWrapThreshold))
				{
					Cursor.Position = new Point(Cursor.Position.X, mouseBounds.Bottom - mouseWrapThreshold);
					mouseEvent = new Point(mouseEvent.X, mouseEvent.Y + mouseBounds.Height - mouseWrapThreshold);
					performedWrap = true;
				}
				else if (Cursor.Position.Y > (mouseBounds.Bottom - mouseWrapThreshold))
				{
					Cursor.Position = new Point(Cursor.Position.X, mouseBounds.Top + mouseWrapThreshold);
					mouseEvent = new Point(mouseEvent.X, mouseEvent.Y - mouseBounds.Height + mouseWrapThreshold);
					performedWrap = true;
				}
			}

			switch (e.Button)
			{
				case MouseButtons.Middle:
					// all cam controls are now bound to the middle mouse button
					if (cam.mode == 0)
					{
						if (zoomKeyDown)
						{
							cam.Position += cam.Look * (mouseDelta.Y * cam.MoveSpeed);
						}
						else if (lookKeyDown)
						{
							cam.Yaw = unchecked((ushort)(cam.Yaw - mouseDelta.X * 0x10));
							cam.Pitch = unchecked((ushort)(cam.Pitch - mouseDelta.Y * 0x10));
						}
						else if (!lookKeyDown && !zoomKeyDown) // pan
						{
							cam.Position += cam.Up * (mouseDelta.Y * cam.MoveSpeed);
							cam.Position += cam.Right * (mouseDelta.X * cam.MoveSpeed) * -1;
						}
					}
					else if (cam.mode == 1)
					{
						if (zoomKeyDown)
						{
							cam.Distance += (mouseDelta.Y * cam.MoveSpeed) * 3;
						}
						else if (lookKeyDown)
						{
							cam.Yaw = unchecked((ushort)(cam.Yaw - mouseDelta.X * 0x10));
							cam.Pitch = unchecked((ushort)(cam.Pitch - mouseDelta.Y * 0x10));
						}
						else if (!lookKeyDown && !zoomKeyDown) // pan
						{
							cam.FocalPoint += cam.Up * (mouseDelta.Y * cam.MoveSpeed);
							cam.FocalPoint += cam.Right * (mouseDelta.X * cam.MoveSpeed) * -1;
						}
					}

					DrawLevel();
					break;

				case MouseButtons.Left:
					foreach(PointHelper pointHelper in PointHelper.Instances)
					{
						pointHelper.TransformAffected(mouseDelta.X / 2 * cam.MoveSpeed, mouseDelta.Y / 2 * cam.MoveSpeed, cam);
					}

					transformGizmo.TransformAffected(mouseDelta.X / 2 * cam.MoveSpeed, mouseDelta.Y / 2 * cam.MoveSpeed, cam);
					DrawLevel();
					break;

				case MouseButtons.None:
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
					Viewport viewport = d3ddevice.Viewport;
					Matrix proj = d3ddevice.Transform.Projection;
					Matrix view = d3ddevice.Transform.View;
					Vector3 Near = mousepos;
					Near.Z = 0;
					Vector3 Far = Near;
					Far.Z = -1;

					GizmoSelectedAxes oldSelection = transformGizmo.SelectedAxes;
					transformGizmo.SelectedAxes = transformGizmo.CheckHit(Near, Far, viewport, proj, view, cam);
					if (oldSelection != transformGizmo.SelectedAxes)
					{
						transformGizmo.Draw(d3ddevice, cam);
						d3ddevice.Present();
						break;
					}

					foreach (PointHelper pointHelper in PointHelper.Instances)
					{
						GizmoSelectedAxes oldHelperAxes = pointHelper.SelectedAxes;
						pointHelper.SelectedAxes = pointHelper.CheckHit(Near, Far, viewport, proj, view, cam);
						if (oldHelperAxes != pointHelper.SelectedAxes) pointHelper.Draw(d3ddevice, cam);
						d3ddevice.Present();
					}

					break;
			}

			if (performedWrap || Math.Abs(mouseDelta.X / 2) * cam.MoveSpeed > 0 || Math.Abs(mouseDelta.Y / 2) * cam.MoveSpeed > 0)
			{
				mouseLast = mouseEvent;
				if (e.Button != MouseButtons.None && selectedItems.ItemCount > 0)
					UpdatePropertyGrid();
			}
		}

		void panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (!isStageLoaded || !panel1.Focused)
				return;

			float detentValue = -1;

			if (e.Delta < 0)
				detentValue = 1;

			if (cam.mode == 0)
				cam.Position += cam.Look * (detentValue * cam.MoveSpeed);
			else if (cam.mode == 1)
				cam.Distance += (detentValue * cam.MoveSpeed);

			DrawLevel();
		}
		#endregion

		void SelectionChanged(EditorItemSelection sender)
		{
			propertyGrid1.SelectedObjects = sender.GetSelection().ToArray();

			if (cam.mode == 1)
			{
				cam.FocalPoint = Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3();
			}

			if (sender.ItemCount > 0) // set up gizmo
			{
				transformGizmo.Enabled = true;
				transformGizmo.AffectedItems = selectedItems.GetSelection();
			}
			else
			{
				if (transformGizmo != null)
				{
					transformGizmo.AffectedItems = new List<Item>();
					transformGizmo.Enabled = false;
				}
			}
		}

		/// <summary>
		/// Refreshes the properties for the currently selected items.
		/// </summary>
		private void UpdatePropertyGrid()
		{
			propertyGrid1.Refresh();
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in selectedItems.GetSelection())
			{
				if (item.CanCopy)
				{
					item.Delete();
					selitems.Add(item);
				}
			}
			selectedItems.Clear();
			LevelData_StateChanged();
			if (selitems.Count == 0) return;
			Clipboard.SetData(DataFormats.Serializable, selitems);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in selectedItems.GetSelection())
			{
				if (item.CanCopy)
					selitems.Add(item);
			}

			if (selitems.Count == 0)
				return;

			Clipboard.SetData("SADXLVLObjectList", selitems);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			object obj = Clipboard.GetData("SADXLVLObjectList");

			if (obj == null)
			{
				MessageBox.Show("Paste operation failed - this is a known issue and is being worked on.");
				return; // todo: finish implementing proper copy/paste
			}

			List<Item> objs = (List<Item>)obj;
			Vector3 center = new Vector3();

			foreach (Item item in objs)
				center.Add(item.Position.ToVector3());

			center = new Vector3(center.X / objs.Count, center.Y / objs.Count, center.Z / objs.Count);
			foreach (Item item in objs)
			{
				item.Position = new Vertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
				item.Paste();
			}

			selectedItems.Clear();
			selectedItems.Add(new List<Item>(objs));
			LevelData_StateChanged();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in selectedItems.GetSelection())
			{
				if (item.CanCopy)
					item.Delete();
			}

			selectedItems.Clear();
			LevelData_StateChanged();
		}

		private void characterToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LevelData.Character = characterToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);

			// Character view buttons
			toolSonic.Checked = false;
			toolTails.Checked = false;
			toolKnuckles.Checked = false;
			toolAmy.Checked = false;
			toolBig.Checked = false;
			toolGamma.Checked = false;

			UncheckMenuItems(characterToolStripMenuItem);
			((ToolStripMenuItem)e.ClickedItem).Checked = true;

			switch (LevelData.Character)
			{
				default:
					toolSonic.Checked = true;
					break;

				case 1:
					toolTails.Checked = true;
					break;

				case 2:
					toolKnuckles.Checked = true;
					break;

				case 3:
					toolAmy.Checked = true;
					break;

				case 4:
					toolGamma.Checked = true;
					break;

				case 5:
					toolBig.Checked = true;
					break;
			}

			transformGizmo.Enabled = false;

			if (transformGizmo.AffectedItems != null)
				transformGizmo.AffectedItems.Clear();

			DrawLevel();
		}

		private void onClickCharacterButton(object sender, EventArgs e)
		{
			if (sender == toolTails)
				tailsToolStripMenuItem.PerformClick();
			else if (sender == toolKnuckles)
				knucklesToolStripMenuItem.PerformClick();
			else if (sender == toolAmy)
				amyToolStripMenuItem.PerformClick();
			else if (sender == toolBig)
				bigToolStripMenuItem.PerformClick();
			else if (sender == toolGamma)
				gammaToolStripMenuItem.PerformClick();
			else
				sonicToolStripMenuItem.PerformClick();
		}

		private void levelToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			UncheckMenuItems(levelToolStripMenuItem);
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			
			transformGizmo.Enabled = false;
			transformGizmo.AffectedItems.Clear();

			DrawLevel();
		}

		private void levelPieceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() != DialogResult.OK)
				return;
			
			foreach (string s in importFileDialog.FileNames)
			{

				selectedItems.Add(LevelData.ImportFromFile(s, d3ddevice, cam, out bool errorFlag, out string errorMsg, selectedItems));

				if (errorFlag)
					MessageBox.Show(errorMsg);
			}

			LevelData_StateChanged();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() != DialogResult.OK)
				return;
			
			DialogResult userClearLevelResult = MessageBox.Show("Do you want to clear the level models first?", "Clear Level?", MessageBoxButtons.YesNoCancel);

			if (userClearLevelResult == DialogResult.Cancel)
				return;

			if (userClearLevelResult == DialogResult.Yes)
			{
				DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNo);

				LevelData.ClearLevelGeometry();

				if (clearAnimsResult == DialogResult.Yes)
				{
					LevelData.ClearLevelGeoAnims();
				}
			}

			foreach (string s in importFileDialog.FileNames)
			{

				selectedItems.Add(LevelData.ImportFromFile(s, d3ddevice, cam, out bool errorFlag, out string errorMsg, selectedItems));

				if (errorFlag)
					MessageBox.Show(errorMsg);
			}

			LevelData_StateChanged();
		}

		private void objectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (NewObjectDialog dlg = new NewObjectDialog(false))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					HitResult hit = PickItem(menuLocation);
					SETItem item = new SETItem(dlg.ID, selectedItems);
					Vector3 pos;
					if (hit.IsHit)
					{
						pos = hit.Position + (hit.Normal * item.GetObjectDefinition().DistanceFromGround);
						item.SetOrientation(hit.Normal.ToVertex());
					}
					else
						pos = cam.Position + (-20 * cam.Look);
					item.Position = new Vertex(pos.X, pos.Y, pos.Z);
					LevelData.SETItems[LevelData.Character].Add(item);
					selectedItems.Clear();
					selectedItems.Add(item);
					LevelData_StateChanged();
				}
		}

		private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Vector3 pos = cam.Position + (-20 * cam.Look);
			CAMItem item = new CAMItem(new Vertex(pos.X, pos.Y, pos.Z), selectedItems);
			LevelData.CAMItems[LevelData.Character].Add(item);
			selectedItems.Clear();
			selectedItems.Add(item);
			LevelData_StateChanged();
		}

		private void missionObjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (NewObjectDialog dlg = new NewObjectDialog(true))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					HitResult hit = PickItem(menuLocation);
					MissionSETItem item = new MissionSETItem(dlg.ObjectList, dlg.ID, selectedItems);
					Vector3 pos;
					if (hit.IsHit)
					{
						pos = hit.Position + (hit.Normal * item.GetObjectDefinition().DistanceFromGround);
						item.SetOrientation(hit.Normal.ToVertex());
					}
					else
						pos = cam.Position + (-20 * cam.Look);
					item.Position = new Vertex(pos.X, pos.Y, pos.Z);
					LevelData.MissionSETItems[LevelData.Character].Add(item);
					selectedItems.Clear();
					selectedItems.Add(item);
					LevelData_StateChanged();
				}
		}

        private void allToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportOBJ(0);
        }

        private void visibleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportOBJ(1);
        }

        private void invisibleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportOBJ(2);
        }

        private void exportOBJ(int mode)
		{
			SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj"
			};
			if (a.ShowDialog() == DialogResult.OK)
			{
				using (StreamWriter objstream = new StreamWriter(a.FileName, false))
				using (StreamWriter mtlstream = new StreamWriter(Path.ChangeExtension(a.FileName, "mtl"), false))
				{
					#region Material Exporting
					string materialPrefix = LevelData.leveltexs;

					objstream.WriteLine("mtllib " + Path.GetFileNameWithoutExtension(a.FileName) + ".mtl");

                    int levelItemsCount = LevelData.LevelItems.Count(v => mode == 0 || (mode == 1 && v.Visible) || (mode == 2 && !v.Visible));
                    int stepCount = LevelData.TextureBitmaps[LevelData.leveltexs].Length + levelItemsCount;
					if (LevelData.geo.Anim != null)
						stepCount += LevelData.geo.Anim.Count;

					ProgressDialog progress = new ProgressDialog("Exporting stage: " + levelName, stepCount, true, false);
					progress.Show(this);
					progress.SetTaskAndStep("Exporting...");

					// This is admittedly not an accurate representation of the materials used in the model - HOWEVER, it makes the materials more managable in MAX
					// So we're doing it this way. In the future we should come back and add an option to do it this way or the original way.
					for (int i = 0; i < LevelData.TextureBitmaps[LevelData.leveltexs].Length; i++)
					{
						mtlstream.WriteLine("newmtl {0}_material_{1}", materialPrefix, i);
						mtlstream.WriteLine("Ka 1 1 1");
						mtlstream.WriteLine("Kd 1 1 1");
						mtlstream.WriteLine("Ks 0 0 0");
						mtlstream.WriteLine("illum 1");

						if (!string.IsNullOrEmpty(LevelData.leveltexs))
						{
							mtlstream.WriteLine("Map_Kd " + LevelData.TextureBitmaps[LevelData.leveltexs][i].Name + ".png");

							// save texture
							string mypath = Path.GetDirectoryName(a.FileName);
							BMPInfo item = LevelData.TextureBitmaps[LevelData.leveltexs][i];
							item.Image.Save(Path.Combine(mypath, item.Name + ".png"));
						}

						progress.Step = String.Format("Texture {0}/{1}", i + 1, LevelData.TextureBitmaps[LevelData.leveltexs].Length);
						progress.StepProgress();
						Application.DoEvents();
					}
					#endregion

					int totalVerts = 0;
					int totalNorms = 0;
					int totalUVs = 0;

					bool errorFlag = false;

					for (int i = 0; i < LevelData.geo.COL.Count; i++)
					{
                        if (mode == 0 || (mode == 1 && LevelData.LevelItems[i].Visible) || (mode == 2 && !LevelData.LevelItems[i].Visible))
                        {
                            Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, materialPrefix, new MatrixStack(),
                                ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

                            progress.Step = String.Format("Mesh {0}/{1}", i + 1, levelItemsCount);
                            progress.StepProgress();
                            Application.DoEvents();
                        }
					}
					if (LevelData.geo.Anim != null)
					{
						for (int i = 0; i < LevelData.geo.Anim.Count; i++)
						{
							Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.Anim[i].Model, materialPrefix, new MatrixStack(),
								ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

							progress.Step = String.Format("Animation {0}/{1}", i + 1, LevelData.geo.Anim.Count);
							progress.StepProgress();
							Application.DoEvents();
						}
					}

					if (errorFlag)
					{
						MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.", "Failure",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}

					progress.SetTaskAndStep("Export complete!");
				}
			}
		}

		private void deathZoneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeathZoneItem item = new DeathZoneItem(d3ddevice, selectedItems);
			Vector3 pos = cam.Position + (-20 * cam.Look);
			item.Position = new Vertex(pos.X, pos.Y, pos.Z);
			switch (LevelData.Character)
			{
				case 0:
					item.Sonic = true;
					break;
				case 1:
					item.Tails = true;
					break;
				case 2:
					item.Knuckles = true;
					break;
				case 3:
					item.Amy = true;
					break;
				case 4:
					item.Gamma = true;
					break;
				case 5:
					item.Big = true;
					break;
			}

			selectedItems.Clear();
			selectedItems.Add(item);
			LevelData_StateChanged();
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			LevelData_StateChanged();
			propertyGrid1.Refresh();
		}

		private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadINI(Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (BugReportDialog dlg = new BugReportDialog("SADXLVL2", null))
				dlg.ShowDialog(this);
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo != null)
				transformGizmo.AffectedItems = selectedItems.GetSelection();

			DrawLevel();
		}

		private void statsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(LevelData.GetStats());
		}

		private void sETITemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void cAMItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawLevel();
		}
		private void deathZonesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SETFindReplace findReplaceForm = new SETFindReplace();

			DialogResult findReplaceResult = findReplaceForm.ShowDialog();

			if (findReplaceResult == DialogResult.OK)
			{
				LevelData_StateChanged();
			}
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.DuplicateSelection(d3ddevice, selectedItems, out bool errorFlag, out string errorMsg);

			if (errorFlag) MessageBox.Show(errorMsg);
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditorOptionsEditor optionsEditor = new EditorOptionsEditor(cam);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.Show();
		}

		void optionsEditor_FormUpdated()
		{
			DrawLevel();
		}

		#region Gizmo Button Event Methods
		private void selectModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.NONE;
				gizmoSpaceComboBox.Enabled = true;
				moveModeButton.Checked = false;
				rotateModeButton.Checked = false;
				DrawLevel(); // TODO: possibly find a better way of doing this than re-drawing the entire scene? Possibly keep a copy of the last render w/o gizmo in memory?
			}
		}

		private void moveModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANFORM_MOVE;
				gizmoSpaceComboBox.Enabled = true;
				selectModeButton.Checked = false;
				rotateModeButton.Checked = false;
				scaleModeButton.Checked = false;
				DrawLevel();
			}
		}

		private void rotateModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANSFORM_ROTATE;
				transformGizmo.LocalTransform = true;
				gizmoSpaceComboBox.SelectedIndex = 1;
				gizmoSpaceComboBox.Enabled = false;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				scaleModeButton.Checked = false;
				DrawLevel();
			}
		}

		private void gizmoSpaceComboBox_DropDownClosed(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.LocalTransform = (gizmoSpaceComboBox.SelectedIndex != 0);
				DrawLevel();
			}
		}

		private void scaleModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANSFORM_SCALE;
				transformGizmo.LocalTransform = true;
				gizmoSpaceComboBox.SelectedIndex = 1;
				gizmoSpaceComboBox.Enabled = false;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				DrawLevel();
			}
		}
		#endregion

		private void duplicateToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItems.ItemCount > 0)
			{
				MessageBox.Show("To use this feature you must have a selection!");
				return;
			}

			DuplicateTo duplicateToWindow = new DuplicateTo(selectedItems);
			duplicateToWindow.ShowDialog();
		}

		private void deleteAllOfTypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			transformGizmo.Enabled = false;
			transformGizmo.AffectedItems = null; // just in case we wind up deleting our selection

			SETDeleteType typeDeleter = new SETDeleteType();

			typeDeleter.ShowDialog();
		}

		private void splinesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void changeLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowLevelSelect();
		}

		private void recentProjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowQuickStart();
		}

		private void toolClearGeometry_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all of the geometry from the stage.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearLevelGeometry();
		}

		private void toolClearAnimations_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all of the geometry animations from the stage.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearLevelGeoAnims();
		}

		private void toolClearSetItems_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all objects from the stage for the current character.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearSETItems(LevelData.Character);
		}

		private void toolClearCamItems_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all camera items from the stage for the current character.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearCAMItems(LevelData.Character);
		}

		private void toolClearMissionSetItems_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all mission objects from the stage for the current character.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearMissionSETItems(LevelData.Character);
		}

		private void toolClearAll_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will clear the entire stage.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (result != DialogResult.Yes)
				return;

			result = MessageBox.Show("Would you like to clear SET & CAM items for all characters?", "SET Items", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);

			if (result == DialogResult.Cancel)
				return;

			if (result == DialogResult.Yes)
			{
				LevelData.ClearSETItems();
				LevelData.ClearCAMItems();
				LevelData.ClearMissionSETItems();
			}
			else
			{
				LevelData.ClearSETItems(LevelData.Character);
				LevelData.ClearCAMItems(LevelData.Character);
				LevelData.ClearMissionSETItems(LevelData.Character);
			}

			LevelData.ClearLevelGeoAnims();
			LevelData.ClearLevelGeometry();
		}

		private void pointToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 1; i < selectedItems.ItemCount; i++)
			{
				Item a = selectedItems.Get(i - 1);
				Item b = selectedItems.Get(i);

				// TODO: Put somewhere else for use with other things, and configurable axis to point on (i.e Y for springs)
				Matrix m = Matrix.LookAtLH(a.Position.ToVector3(), b.Position.ToVector3(), new Vector3(0, 1, 0));

				a.Rotation.YDeg = (float)Math.Atan2(m.M13, m.M33) * MathHelper.Rad2Deg;
				a.Rotation.XDeg = (float)Math.Asin(-m.M23) * MathHelper.Rad2Deg;
				a.Rotation.ZDeg = (float)Math.Atan2(m.M21, m.M22) * MathHelper.Rad2Deg;

				LevelData_StateChanged();
			}
		}

		private void calculateAllBoundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (LevelItem item in LevelData.LevelItems)
				item.CalculateBounds();
		}
    }
}