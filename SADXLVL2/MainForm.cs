using System;
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
		Dictionary<string, Dictionary<string, string>> ini;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		string levelID;
		internal string levelName;
		bool isStageLoaded;
		internal List<Item> SelectedItems;
		Dictionary<string, List<string>> levelNames;
		bool lookKeyDown;
		bool zoomKeyDown;

		// TODO: Make these both configurable.
		bool mouseWrapScreen = false;
		ushort mouseWrapThreshold = 16;

		// helpers / ui stuff
		TransformGizmo transformGizmo;
		PointHelper cameraPointA;
		PointHelper cameraPointB;
		//PointHelper miscHelper; // use this for anything you like, maybe for SET things like rocket / dash ring destinations?

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			LevelData.StateChanged += LevelData_StateChanged;
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

				EditorOptions.InitializeDefaultLights(d3ddevice);
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
			EditorOptions.InitializeDefaultLights(d3ddevice);
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
			ini = IniFile.Load(filename);
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			levelNames = new Dictionary<string, List<string>>();

			foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
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
			isStageLoaded = false;
			SelectedItems = new List<Item>();
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
		/// Loads all of the textures from the two keys in the INI into the scene.
		/// </summary>
		/// <param name="iniSection">The section in the INI (usually string.Empty).</param>
		/// <param name="iniKey">The key in the INI.</param>
		/// <param name="systemPath">The game's system path.</param>
		void LoadTextureList(string iniSection, string iniKey, string systemPath)
		{
			LoadTextureList(TextureList.Load(ini[iniSection][iniKey]), systemPath);
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
				int steps = 9;
				if (d3ddevice == null)
					++steps;

				toolStrip1.Enabled = false;

				using (ProgressDialog progress = new ProgressDialog("Loading stage: " + levelName, steps))
				{
					//LevelData.Character = 0;
					Dictionary<string, string> group = ini[levelID];
					string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
					string modpath = null;

					if (ini[string.Empty].ContainsKey("modpath"))
						modpath = ini[string.Empty]["modpath"];

					SA1LevelAct levelact = new SA1LevelAct(group.GetValueOrDefault("LevelID", "0000"));
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

					if (!group.ContainsKey("LevelGeo"))
						LevelData.geo = null;
					else
					{
						LevelData.geo = LandTable.LoadFromFile(group["LevelGeo"]);
						LevelData.LevelItems = new List<LevelItem>();
						for (int i = 0; i < LevelData.geo.COL.Count; i++)
							LevelData.LevelItems.Add(new LevelItem(LevelData.geo.COL[i], d3ddevice, i));
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

						Dictionary<SA1LevelAct, SA1StartPosInfo> posini =
							SA1StartPosList.Load(ini[string.Empty][LevelData.Characters[i] + "start"]);

						Vertex pos = new Vertex();
						int rot = 0;

						if (posini.ContainsKey(levelact))
						{
							pos = posini[levelact].Position.ToSAModel();
							rot = posini[levelact].YRotation;
						}
						if (i == 0 & levelact.Level == SA1LevelIDs.PerfectChaos)
						{
							LevelData.StartPositions[i] = new StartPosItem(new ModelFile(ini[string.Empty]["supermdl"]).Model,
								ini[string.Empty]["supertex"],
								float.Parse(ini[string.Empty]["superheight"], System.Globalization.NumberStyles.Float,
									System.Globalization.NumberFormatInfo.InvariantInfo), pos, rot, d3ddevice);
						}
						else
						{
							LevelData.StartPositions[i] =
								new StartPosItem(new ModelFile(ini[string.Empty][LevelData.Characters[i] + "mdl"]).Model,
									ini[string.Empty][LevelData.Characters[i] + "tex"],
									float.Parse(ini[string.Empty][LevelData.Characters[i] + "height"], System.Globalization.NumberStyles.Float,
										System.Globalization.NumberFormatInfo.InvariantInfo), pos, rot, d3ddevice);
						}


						LoadTextureList(string.Empty, LevelData.Characters[i] + "texlist", syspath);
					}

					progress.StepProgress();

					#endregion

					#region Death Zones

					progress.SetTaskAndStep("Death Zones:", "Initializing...");

					if (!group.ContainsKey("DeathZones"))
						LevelData.DeathZones = null;
					else
					{
						LevelData.DeathZones = new List<DeathZoneItem>();
						DeathZoneFlags[] dzini = DeathZoneFlagsList.Load(group["DeathZones"]);
						string path = Path.GetDirectoryName(group["DeathZones"]);
						for (int i = 0; i < dzini.Length; i++)
						{
							progress.SetStep(String.Format("Loading model {0}/{1}", (i + 1), dzini.Length));

							LevelData.DeathZones.Add(new DeathZoneItem(
								new ModelFile(Path.Combine(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl"))
									.Model,
								dzini[i].Flags, d3ddevice));
						}
					}

					progress.StepProgress();

					#endregion

					#region Textures and Texture Lists

					progress.SetTaskAndStep("Loading textures for:");

					progress.SetStep("Common objects");
					// Loads common object textures (e.g OBJ_REGULAR)
					LoadTextureList(string.Empty, "objtexlist", syspath);

					progress.SetTaskAndStep("Loading stage texture lists...");

					// Loads the textures in the texture list for this stage (e.g BEACH01)
					foreach (string file in Directory.GetFiles(ini[string.Empty]["leveltexlists"]))
					{
						LevelTextureList texini = LevelTextureList.Load(file);
						if (texini.Level != levelact)
							continue;
						
						LoadTextureList(texini.TextureList, syspath);
					}

					progress.SetTaskAndStep("Loading textures for:", "Objects");
					// Object texture list(s)
					LoadTextureList(TextureList.Load(group["ObjTexs"]), syspath);

					progress.SetStep("Stage");
					// The stage textures... again? "Extra"?
					if (group.ContainsKey("Textures"))
					{
						string[] textures = group["Textures"].Split(',');
						foreach (string tex in textures)
						{
							LoadPVM(tex, syspath);

							if (string.IsNullOrEmpty(LevelData.leveltexs))
								LevelData.leveltexs = tex;
						}
					}

					progress.StepProgress();

					#endregion

					#region Object Definitions / SET Layout

					progress.SetTaskAndStep("Loading Object Definitions:", "Parsing...");

					LevelData.ObjDefs = new List<ObjectDefinition>();
					Dictionary<string, ObjectData> objdefini =
						IniSerializer.Deserialize<Dictionary<string, ObjectData>>(ini[string.Empty]["objdefs"]);

					if (File.Exists(group.GetValueOrDefault("ObjList", string.Empty)))
					{
						List<ObjectData> objectErrors = new List<ObjectData>();
						ObjectListEntry[] objlstini = ObjectList.Load(group["ObjList"], false);
						Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;

						for (int ID = 0; ID < objlstini.Length; ID++)
						{
							string codeaddr = objlstini[ID].CodeString;

							if (!objdefini.ContainsKey(codeaddr))
								codeaddr = "0";

							ObjectData defgroup = objdefini[codeaddr];
							ObjectDefinition def = null;

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
										});
										para.GenerateExecutable = false;
										para.GenerateInMemory = false;
										para.IncludeDebugInformation = true;
										para.OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile);
										CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
										if (res.Errors.HasErrors)
											def = new DefaultObjectDefinition();
										else
											def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
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
							LevelData.SETName = group.GetValueOrDefault("SETName",
								((int)levelact.Level).ToString("00") + levelact.Act.ToString("00"));
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

										SETItem ent = new SETItem(setfile, address);
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

					progress.StepProgress();

					#endregion

					#region CAM Layout

					progress.SetTaskAndStep("Loading CAM items", "Initializing...");

					LevelData.CAMName = ((int)levelact.Level).ToString("00") + levelact.Act.ToString("00");
					string camstr = Path.Combine(syspath, "CAM" + LevelData.CAMName + "{0}.bin");

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

								CAMItem ent = new CAMItem(camfile, address);
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
					if (group.ContainsKey("Effects"))
					{
						progress.SetTaskAndStep("Loading Level Effects...");

						LevelDefinition def = null;
						string ty = "SADXObjectDefinitions.Level_Effects." + Path.GetFileNameWithoutExtension(group["Effects"]);
						string dllfile = Path.Combine("dllcache", ty + ".dll");
						DateTime modDate = DateTime.MinValue;

						if (File.Exists(dllfile))
							modDate = File.GetLastWriteTime(dllfile);

						string fp = group["Effects"].Replace('/', Path.DirectorySeparatorChar);
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
							def.Init(group, levelact.Act, d3ddevice);

						LevelData.leveleff = def;
					}

					progress.StepProgress();

					#endregion

					#region Loading Splines

					LevelData.LevelSplines = new List<SplineData>();

					if (ini[string.Empty].ContainsKey("paths"))
					{
						progress.SetTaskAndStep("Reticulating splines...", null);

						String splineDirectory = Path.Combine(Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["paths"]),
							levelact.ToString());

						if (Directory.Exists(splineDirectory))
						{
							List<Dictionary<string, Dictionary<string, string>>> pathFiles =
								new List<Dictionary<string, Dictionary<string, string>>>();

							for (int i = 0; i < int.MaxValue; i++)
							{
								string path = string.Concat(splineDirectory, string.Format("/{0}.ini", i));
								if (File.Exists(path))
								{
									pathFiles.Add(IniFile.Load(path));
								}
								else
									break;
							}

							foreach (Dictionary<string, Dictionary<string, string>> pathFile in pathFiles) // looping through path files
							{
								SplineData newSpline = new SplineData();

								for (int iniEntryIndx = 0; iniEntryIndx < pathFile.Count - 1; iniEntryIndx++)
								{
									Vertex knotPosition = new Vertex();
									ushort XRot = 0, YRot = 0;
									float knotDistance = 0;

									if (pathFile[iniEntryIndx.ToString()].ContainsKey("XRotation"))
									{
										XRot = ushort.Parse(pathFile[iniEntryIndx.ToString()]["XRotation"], System.Globalization.NumberStyles.HexNumber,
											System.Globalization.CultureInfo.InvariantCulture);
									}

									if (pathFile[iniEntryIndx.ToString()].ContainsKey("YRotation"))
									{
										YRot = ushort.Parse(pathFile[iniEntryIndx.ToString()]["YRotation"], System.Globalization.NumberStyles.HexNumber,
											System.Globalization.CultureInfo.InvariantCulture);
									}

									if (pathFile[iniEntryIndx.ToString()].ContainsKey("Position"))
									{
										string[] vertexComponents = pathFile[iniEntryIndx.ToString()]["Position"].Split(',');

										if (vertexComponents.Length == 3)
										{
											knotPosition.X = float.Parse(vertexComponents[0]);
											knotPosition.Y = float.Parse(vertexComponents[1]);
											knotPosition.Z = float.Parse(vertexComponents[2]);
										}
									}

									if (pathFile[iniEntryIndx.ToString()].ContainsKey("Distance"))
									{
										knotDistance = float.Parse(pathFile[iniEntryIndx.ToString()]["Distance"]);
									}

									newSpline.AddKnot(new Knot(knotPosition, new Rotation(XRot, YRot, 0), knotDistance));
								}

								LevelData.LevelSplines.Add(newSpline);
							}
						}
					}

					progress.StepProgress();

					#endregion

					transformGizmo = new TransformGizmo();

					cameraPointA = new PointHelper { BoxTexture = Gizmo.ATexture, DrawCube = true };
					cameraPointB = new PointHelper { BoxTexture = Gizmo.BTexture, DrawCube = true };

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

			// The whole view menu!
			viewToolStripMenuItem.Enabled = true;
			statsToolStripMenuItem.Enabled = isGeometryPresent;
			deathZonesToolStripMenuItem.Checked = deathZonesToolStripMenuItem.Enabled = deathZoneToolStripMenuItem.Enabled = isDeathZonePresent;

			isStageLoaded = true;
			SelectedItems = new List<Item>();
			UseWaitCursor = false;
			Enabled = true;

			gizmoSpaceComboBox.Enabled = true;
			gizmoSpaceComboBox.SelectedIndex = 0;

			toolStrip1.Enabled = isStageLoaded;
			SelectedItemChanged();
			DrawLevel();
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

			ProgressDialog progress = new ProgressDialog("Saving stage: " + levelName, 5, true, autoCloseDialog);
			progress.Show(this);
			Application.DoEvents();

			Dictionary<string, string> group = ini[levelID];
			string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
			string modpath = null;
			if (ini[string.Empty].ContainsKey("modpath"))
				modpath = ini[string.Empty]["modpath"];
			SA1LevelAct levelact = new SA1LevelAct(@group.GetValueOrDefault("LevelID", "0000"));

			progress.SetTaskAndStep("Saving:", "Geometry...");

			if (LevelData.geo != null)
			{
				LevelData.geo.Tool = "SADXLVL2";
				LevelData.geo.SaveToFile(@group["LevelGeo"], LandTableFormat.SA1);
			}

			progress.StepProgress();

			progress.Step = "Start positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				Dictionary<SA1LevelAct, SA1StartPosInfo> posini =
					SA1StartPosList.Load(ini[string.Empty][LevelData.Characters[i] + "start"]);

				if (posini.ContainsKey(levelact))
					posini.Remove(levelact);

				if (LevelData.StartPositions[i].Position.X != 0 | LevelData.StartPositions[i].Position.Y != 0 |
				    LevelData.StartPositions[i].Position.Z != 0 | LevelData.StartPositions[i].Rotation.Y != 0)
				{
					posini.Add(levelact,
						new SA1StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position.ToSA_Tools(),
							YRotation = LevelData.StartPositions[i].Rotation.Y
						});
				}
				posini.Save(ini[string.Empty][LevelData.Characters[i] + "start"]);
			}

			progress.StepProgress();

			progress.Step = "Death zones...";
			Application.DoEvents();

			if (LevelData.DeathZones != null)
			{
				DeathZoneFlags[] dzini = new DeathZoneFlags[LevelData.DeathZones.Count];
				string path = Path.GetDirectoryName(@group["DeathZones"]);
				for (int i = 0; i < LevelData.DeathZones.Count; i++)
					dzini[i] = LevelData.DeathZones[i].Save(path, i);
				dzini.Save(@group["DeathZones"]);
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

					// TODO: Handle this differently. File stream? If the user is using a symbolic link for example, we defeat the purpose by deleting it.
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
			d3ddevice.Material = new Microsoft.DirectX.Direct3D.Material { Ambient = Color.White };
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

			if (splinesToolStripMenuItem.Checked)
			{
				foreach (SplineData spline in LevelData.LevelSplines)
					spline.Draw(d3ddevice);
			}

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
						renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
				}
			}
			#endregion

			renderlist.AddRange(LevelData.StartPositions[LevelData.Character].Render(d3ddevice, cam, transform, SelectedItems.Contains(LevelData.StartPositions[LevelData.Character])));

			#region Adding SET Layout
			if (LevelData.SETItems != null && sETITemsToolStripMenuItem.Checked)
			{
				foreach (SETItem item in LevelData.SETItems[LevelData.Character])
					renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
			}
			#endregion

			#region Adding Death Zones
			if (LevelData.DeathZones != null & deathZonesToolStripMenuItem.Checked)
			{
				foreach (DeathZoneItem item in LevelData.DeathZones)
				{
					if (item.Visible)
						renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
				}
			}
			#endregion

			#region Adding CAM Layout
			if (LevelData.CAMItems != null && cAMItemsToolStripMenuItem.Checked)
			{
				foreach (CAMItem item in LevelData.CAMItems[LevelData.Character])
					renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
			}
			#endregion

			RenderInfo.Draw(renderlist, d3ddevice, cam);

			d3ddevice.EndScene(); // scene drawings go before this line
			// draw helper cubes before clearing depth buffer
			cameraPointA.DrawBox(d3ddevice, cam);
			cameraPointB.DrawBox(d3ddevice, cam);

			transformGizmo.Draw(d3ddevice, cam);
			cameraPointA.Draw(d3ddevice, cam);
			cameraPointB.Draw(d3ddevice, cam);

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
					if ((cameraPointA.SelectedAxes != GizmoSelectedAxes.NONE)
					    || (cameraPointB.SelectedAxes != GizmoSelectedAxes.NONE)
					    || (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE))
					{
						return;
					}

					float mindist = cam.DrawDistance; // initialize to max distance, because it will get smaller on each check
					HitResult dist;
					Item item = null;
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
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
								dist = LevelData.LevelItems[i].CheckHit(Near, Far, viewport, proj, view);
								if (dist.IsHit & dist.Distance < mindist)
								{
									mindist = dist.Distance;
									item = LevelData.LevelItems[i];
								}
							}
						}
					}
					#endregion

					#region Picking Start Positions
					dist = LevelData.StartPositions[LevelData.Character].CheckHit(Near, Far, viewport, proj, view);
					if (dist.IsHit & dist.Distance < mindist)
					{
						mindist = dist.Distance;
						item = LevelData.StartPositions[LevelData.Character];
					}
					#endregion

					#region Picking SET Items
					if (LevelData.SETItems != null && sETITemsToolStripMenuItem.Checked)
						foreach (SETItem setitem in LevelData.SETItems[LevelData.Character])
						{
							dist = setitem.CheckHit(Near, Far, viewport, proj, view);
							if (dist.IsHit & dist.Distance < mindist)
							{
								mindist = dist.Distance;
								item = setitem;
							}
						}
					#endregion

					#region Picking CAM Items
					if ((LevelData.CAMItems != null) && (cAMItemsToolStripMenuItem.Checked))
					{
						foreach (CAMItem camItem in LevelData.CAMItems[LevelData.Character])
						{
							dist = camItem.CheckHit(Near, Far, viewport, proj, view);
							if (dist.IsHit & dist.Distance < mindist)
							{
								mindist = dist.Distance;
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
								dist = dzitem.CheckHit(Near, Far, viewport, proj, view);
								if (dist.IsHit & dist.Distance < mindist)
								{
									mindist = dist.Distance;
									item = dzitem;
								}
							}
						}
					}

					#endregion

					if (item != null)
					{
						if (ModifierKeys == Keys.Control)
						{
							if (SelectedItems.Contains(item))
								SelectedItems.Remove(item);
							else
								SelectedItems.Add(item);
						}
						else if (!SelectedItems.Contains(item))
						{
							SelectedItems.Clear();
							SelectedItems.Add(item);
						}
					}
					else if ((ModifierKeys & Keys.Control) == 0)
					{
						SelectedItems.Clear();
					}
					break;

				case MouseButtons.Right:
					bool cancopy = false;
					foreach (Item obj in SelectedItems)
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
					contextMenuStrip1.Show(panel1, e.Location);
					break;
			}
			SelectedItemChanged();
			DrawLevel();
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
						if (SelectedItems.Count > 0)
							cam.FocalPoint = Item.CenterFromSelection(SelectedItems).ToVector3();
						else
							cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
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
					foreach (Item item in SelectedItems)
						item.Delete();
					SelectedItems.Clear();
					SelectedItemChanged();
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
							LevelData.Character = LevelData.Character - 1;
						else
							LevelData.Character = (LevelData.Character + 1)%6;

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

			if (e.Button != MouseButtons.None)
			{
				Rectangle mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(Cursor.Position) : RectangleToScreen(panel1.ClientRectangle);
				Console.WriteLine(mouseBounds);

				if (Cursor.Position.X < (mouseBounds.Left + mouseWrapThreshold))
				{
					Cursor.Position = new Point(mouseBounds.Right - mouseWrapThreshold, Cursor.Position.Y);
					mouseEvent = new Point(mouseEvent.X + mouseBounds.Width - mouseWrapThreshold, mouseEvent.Y);
				}
				else if (Cursor.Position.X > (mouseBounds.Right - mouseWrapThreshold))
				{
					Cursor.Position = new Point(mouseBounds.Left + mouseWrapThreshold, Cursor.Position.Y);
					mouseEvent = new Point(mouseEvent.X - mouseBounds.Width + mouseWrapThreshold, mouseEvent.Y);
				}
				if (Cursor.Position.Y < (mouseBounds.Top + mouseWrapThreshold))
				{
					Cursor.Position = new Point(Cursor.Position.X, mouseBounds.Bottom - mouseWrapThreshold);
					mouseEvent = new Point(mouseEvent.X, mouseEvent.Y + mouseBounds.Height - mouseWrapThreshold);
				}
				else if (Cursor.Position.Y > (mouseBounds.Bottom - mouseWrapThreshold))
				{
					Cursor.Position = new Point(Cursor.Position.X, mouseBounds.Top + mouseWrapThreshold);
					mouseEvent = new Point(mouseEvent.X, mouseEvent.Y - mouseBounds.Height + mouseWrapThreshold);
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
					cameraPointA.TransformAffected(mouseDelta.X / 2 * cam.MoveSpeed, mouseDelta.Y / 2 * cam.MoveSpeed);
					cameraPointB.TransformAffected(mouseDelta.X / 2 * cam.MoveSpeed, mouseDelta.Y / 2 * cam.MoveSpeed);
					transformGizmo.TransformAffected(mouseDelta.X / 2 * cam.MoveSpeed, mouseDelta.Y / 2 * cam.MoveSpeed);
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

					GizmoSelectedAxes oldCamA = cameraPointA.SelectedAxes;
					cameraPointA.SelectedAxes = cameraPointA.CheckHit(Near, Far, viewport, proj, view, cam);
					if (oldCamA != cameraPointA.SelectedAxes)
					{
						cameraPointA.Draw(d3ddevice, cam);
						d3ddevice.Present();
						break;
					}

					if (cameraPointA.SelectedAxes == GizmoSelectedAxes.NONE)
					{
						GizmoSelectedAxes oldCamB = cameraPointB.SelectedAxes;
						cameraPointB.SelectedAxes = cameraPointB.CheckHit(Near, Far, viewport, proj, view, cam);
						if (oldCamB != cameraPointB.SelectedAxes)
						{
							cameraPointB.Draw(d3ddevice, cam);
							d3ddevice.Present();
							break;
						}
					}

					break;
			}

			mouseLast = mouseEvent;
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

		internal void SelectedItemChanged()
		{
			propertyGrid1.SelectedObjects = SelectedItems.ToArray();

			if (cam.mode == 1)
			{
				cam.FocalPoint = Item.CenterFromSelection(SelectedItems).ToVector3();
			}

			if (SelectedItems.Count > 0) // set up gizmo
			{
				transformGizmo.Enabled = true;
				transformGizmo.AffectedItems = SelectedItems;

				if (SelectedItems.Count == 1) // single-select only cases
				{
					if (SelectedItems[0] is CAMItem)
					{
						CAMItem camItem = (CAMItem)SelectedItems[0];
						cameraPointA.SetPoint(camItem.PointA);
						cameraPointB.SetPoint(camItem.PointB);
					}
					else
					{
						cameraPointA.Enabled = false;
						cameraPointB.Enabled = false;
					}
				}
			}
			else
			{
				if (transformGizmo != null)
				{
					transformGizmo.AffectedItems = new List<Item>();
					transformGizmo.Enabled = false;
				}

				if ((cameraPointA != null) && (cameraPointB != null))
				{
					cameraPointA.Enabled = false;
					cameraPointB.Enabled = false;
				}
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in SelectedItems)
			{
				if (item.CanCopy)
				{
					item.Delete();
					selitems.Add(item);
				}
			}
			SelectedItems.Clear();
			SelectedItemChanged();
			DrawLevel();
			if (selitems.Count == 0) return;
			Clipboard.SetData(DataFormats.Serializable, selitems);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in SelectedItems)
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

			SelectedItems = new List<Item>(objs);
			SelectedItemChanged();
			DrawLevel();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in SelectedItems)
			{
				if (item.CanCopy)
					item.Delete();
			}

			SelectedItems.Clear();
			SelectedItemChanged();
			DrawLevel();
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
			if (importFileDialog.ShowDialog() == DialogResult.OK)
			{
				foreach (string s in importFileDialog.FileNames)
				{
					bool errorFlag = false;
					string errorMsg = "";

					LevelData.ImportFromFile(s, d3ddevice, cam, out errorFlag, out errorMsg);

					if (errorFlag)
						MessageBox.Show(errorMsg);
				}
			}
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() == DialogResult.OK)
			{
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
					bool errorFlag = false;
					string errorMsg = "";

					LevelData.ImportFromFile(s, d3ddevice, cam, out errorFlag, out errorMsg);

					if (errorFlag)
						MessageBox.Show(errorMsg);
				}
			}
		}

		private void objectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SETItem item = new SETItem();
			Vector3 pos = cam.Position + (-20 * cam.Look);
			item.Position = new Vertex(pos.X, pos.Y, pos.Z);
			LevelData.SETItems[LevelData.Character].Add(item);
			SelectedItems = new List<Item>() { item };
			SelectedItemChanged();
			DrawLevel();
		}

		private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Vector3 pos = cam.Position + (-20 * cam.Look);
			CAMItem item = new CAMItem(new Vertex(pos.X, pos.Y, pos.Z));
			LevelData.CAMItems[LevelData.Character].Add(item);
			SelectedItems = new List<Item>() { item };
			SelectedItemChanged();
			DrawLevel();
		}

		private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
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

					int stepCount = LevelData.TextureBitmaps[LevelData.leveltexs].Length + LevelData.geo.COL.Count;
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
						Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, materialPrefix, new MatrixStack(),
							ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

						progress.Step = String.Format("Mesh {0}/{1}", i + 1, LevelData.geo.COL.Count);
						progress.StepProgress();
						Application.DoEvents();
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
			DeathZoneItem item = new DeathZoneItem(d3ddevice);
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
			SelectedItems = new List<Item>() { item };
			SelectedItemChanged();
			DrawLevel();
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			DrawLevel();
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
			using (BugReportDialog dlg = new BugReportDialog())
				dlg.ShowDialog(this);
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo != null)
				transformGizmo.AffectedItems = SelectedItems;

			SelectedItemChanged();
			DrawLevel();
		}

		private void clearLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNoCancel);

			if (clearAnimsResult == DialogResult.Cancel)
				return;

			LevelData.ClearLevelGeometry();

			if (clearAnimsResult == DialogResult.Yes)
				LevelData.ClearLevelGeoAnims();
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
				SelectedItemChanged();
				DrawLevel();
			}
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool errorFlag = false;
			string errorMsg = "";
			LevelData.DuplicateSelection(d3ddevice, ref SelectedItems, out errorFlag, out errorMsg);

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
			if ((SelectedItems == null) || (SelectedItems.Count == 0))
			{
				MessageBox.Show("To use this feature you must have a selection!");
				return;
			}

			DuplicateTo duplicateToWindow = new DuplicateTo(SelectedItems);
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
	}
}