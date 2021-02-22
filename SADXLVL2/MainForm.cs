using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SA_Tools;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using System.Text;
using SharpDX.Mathematics.Interop;

namespace SonicRetro.SAModel.SADXLVL2
{
	// TODO: Organize this whole class.
	// TODO: Unify as much SET/CAM load and save code as possible. They're practically identical.
	// TODO: Rename controls to be more distinguishable.
	// (Example: sETItemsToolStripMenuItem1 is a dropdown menu. sETITemsToolStripMenuItem is a toggle.)
	public partial class MainForm : Form
	{
		SettingsFile settingsfile; //For user editable settings
		Properties.Settings AppConfig = Properties.Settings.Default; // For non-user editable settings in SADXLVL2.config
		ProgressDialog progress;

		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			InitializeComponent();
			AddMouseMoveHandler(this);
		}

		protected override void WndProc(ref Message m)
		{
			// Suppress the WM_UPDATEUISTATE message to remove rendering flicker
			if (m.Msg == 0x128) return;
			base.WndProc(ref m);
		}

		private void AddMouseMoveHandler(Control c)
		{
			c.MouseMove += Panel1_MouseMove;
			if (c.Controls.Count > 0)
			{
				foreach (Control ct in c.Controls)
					AddMouseMoveHandler(ct);
			}
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			log.Add(e.Exception.ToString());
			string errDesc = "SADXLVL2 has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
				"If you wish to report a bug, please include the following in your report:";
			ErrorDialog report = new ErrorDialog("SADXLVL2", errDesc, log.GetLogString());
			log.WriteLog();
			DialogResult dgresult = report.ShowDialog();
			switch (dgresult)
			{
				case DialogResult.Abort:
				case DialogResult.OK:
					Application.Exit();
					break;
			}
		}

		internal Device d3ddevice;

		#region Editor-Specific Variables
		SAEditorCommon.IniData ini;
		Logger log = new Logger(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SADXLVL2.log");
		OnScreenDisplay osd;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		EditorItemSelection selectedItems = new EditorItemSelection();
		EditorOptionsEditor optionsEditor;
		Direct3D.Mesh boundsMesh;
		NJS_MATERIAL boundsMaterial = new NJS_MATERIAL
		{
			DiffuseColor = Color.FromArgb(128, Color.White),
			SpecularColor = Color.Black,
			UseAlpha = true,
			DoubleSided = false,
			Exponent = 10,
			IgnoreSpecular = false,
			UseTexture = false
		};
		#endregion

		#region Stage Variables
		string levelID;
		internal string levelName;
		bool isStageLoaded;
		bool DeviceResizing;

		Dictionary<string, List<string>> levelNames;

		// light list
		List<SA1StageLightData> stageLightList;
		List<SA1StageLightData> currentLightList;
		#endregion

		#region UI & Customization
		bool skipDefs;
		bool lookKeyDown;
		bool zoomKeyDown;
		bool cameraKeyDown;
		Point menuLocation;
		bool isPointOperation;
		bool unsaved;

		// TODO: Make these both configurable.
		bool mouseWrapScreen = false;
		System.Drawing.Rectangle mouseBounds;
		ushort mouseWrapThreshold = 2;
		bool mouseHide = false;
		Point mouseBackup;

		TransformGizmo transformGizmo;
		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;

		#endregion

		// project support stuff
		string systemFallback;
		string currentProjectPath;
		Dictionary<string, ObjectData> objdefini;

		private void MainForm_Load(object sender, EventArgs e)
		{
			Assimp.Unmanaged.AssimpLibrary.Instance.LoadLibrary(Path.Combine(Application.StartupPath, "lib", "assimp.dll"));

			settingsfile = SettingsFile.Load();
			progress = new ProgressDialog("SADXLVL2", 11, false, true, true);
			modelLibraryControl1.InitRenderer();
			InitGUISettings();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			LevelData.StateChanged += LevelData_StateChanged;
			LevelData.PointOperation += LevelData_PointOperation;
			RenderPanel.MouseWheel += panel1_MouseWheel;
			InitDisableInvalidControls();
			log.DeleteLogFile();
			log.Add("SADXLVL2: New log entry on " + DateTime.Now.ToString("G") + "\n");
			AppConfig.Reload();
			EditorOptions.RenderDrawDistance = settingsfile.SADXLVL2.DrawDistance_General;
			EditorOptions.LevelDrawDistance = settingsfile.SADXLVL2.DrawDistance_Geometry;
			EditorOptions.SetItemDrawDistance = settingsfile.SADXLVL2.DrawDistance_SET;
			disableModelLibraryToolStripMenuItem.Checked = settingsfile.SADXLVL2.DisableModelLibrary;
			alternativeCameraToolStripMenuItem.Checked = settingsfile.SADXLVL2.AlternativeCamera;
			if (settingsfile.SADXLVL2.ShowWelcomeScreen)
				ShowWelcomeScreen();
			systemFallback = Program.SADXGameFolder + "/System/";

			actionList = ActionMappingList.Load(Path.Combine(Application.StartupPath, "keybinds", "SADXLVL2.ini"),
				DefaultActionList.DefaultActionMapping);

			actionInputCollector = new ActionInputCollector();
			actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			actionInputCollector.OnActionStart += ActionInputCollector_OnActionStart;
			actionInputCollector.OnActionRelease += ActionInputCollector_OnActionRelease;

			cam.ModifierKey = settingsfile.SADXLVL2.CameraModifier;
			optionsEditor = new EditorOptionsEditor(cam, actionList.ActionKeyMappings.ToArray(), DefaultActionList.DefaultActionMapping, true, true);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.FormUpdatedKeys += optionsEditor_UpdateKeys;

			sceneGraphControl1.InitSceneControl(selectedItems);

			if (Program.args.Length > 0)
			{
				if (Program.SADXGameFolder == "") systemFallback = Path.GetDirectoryName(Program.args[0]) + "\\System\\";
				switch (Path.GetExtension(Program.args[0]).ToLowerInvariant())
				{
					case ".sa1lvl":
					case ".sa2lvl":
					case ".sa2blvl":
						LoadLandtable(Program.args[0]);
						unsaved = false;
						break;
					case ".ini":
					default:
						LoadINI(Program.args[0]);
						ShowLevelSelect();
						break;
				}
			}
			else if (Program.SADXGameFolder == "")
			{
				ShowPathWarning();
			}
			else
			{
				using (ProjectSelectDialog projectSelectDialog = new ProjectSelectDialog())
				{
					projectSelectDialog.LoadProjectList(Program.SADXGameFolder);
					if (projectSelectDialog.ShowDialog() == DialogResult.OK)
					{
						LoadProject(projectSelectDialog.SelectedProject);
					}
				}
			}
		}

		private void InitGUISettings()
		{
			if (settingsfile.SADXLVL2.PropertiesSplitterPosition != 0) PropertiesSplitter.SplitterDistance = settingsfile.SADXLVL2.PropertiesSplitterPosition;
			if (settingsfile.SADXLVL2.LibrarySplitterPosition != 0) LibrarySplitter.SplitterDistance = settingsfile.SADXLVL2.LibrarySplitterPosition;
			if (settingsfile.SADXLVL2.ItemsSplitterPosition != 0) ItemsSplitter.SplitterDistance = settingsfile.SADXLVL2.ItemsSplitterPosition;
			ItemsSplitter.SplitterMoved += new SplitterEventHandler(ItemsSplitter_SplitterMoved);
			LibrarySplitter.SplitterMoved += new SplitterEventHandler(LibrarySplitter_SplitterMoved);
			PropertiesSplitter.SplitterMoved += new SplitterEventHandler(PropertiesSplitter_SplitterMoved);
		}

		/// <summary>
		/// I moved these to here instead of setting them to false in the designer,
		/// because it makes the designer easier to read.
		/// </summary>
		private void InitDisableInvalidControls()
		{
			// Context menu
			// Add -> Level Piece
			// Does this even make sense? This thing prompts the user to import a model,
			// not select an existing one...
			levelPieceToolStripMenuItem.Enabled = false;
			clearLevelToolStripMenuItem.Enabled = false;
			addToolStripMenuItem.Enabled = false;
			addToolStripMenuItem1.Enabled = false;
			duplicateToolStripMenuItem.Enabled = false;
			pointOneObjectAtAnotherToolStripMenuItem.Enabled = false;
			advancedToolStripMenuItem.Enabled = false;

			// Add -> Object
			objectToolStripMenuItem.Enabled = false;
			// Add -> Mission Object
			missionObjectToolStripMenuItem.Enabled = false;

			// File menu
			// Save
			saveToolStripMenuItem.Enabled = false;
			// Import
			importToolStripMenuItem.Enabled = false;
			// Export
			exportToolStripMenuItem.Enabled = false;

			// Edit menu
			// Clear Level
			clearLevelToolStripMenuItem.Enabled = false;
			// SET Items submenu
			// Gotta clear up these names at some point...
			// Drop the 1, and you get the dropdown menu under View.
			editSETItemsToolStripMenuItem.Enabled = false;
			viewSETItemsToolStripMenuItem.Enabled = false;
			deleteSelectedToolStripMenuItem.Enabled = false;
			deleteToolStripMenuItem.Enabled = false;
			// Duplicate
			duplicateToolStripMenuItem.Enabled = false;
			// Calculate All Bounds
			calculateAllBoundsToolStripMenuItem.Enabled = false;

			// The whole view menu!
			viewToolStripMenuItem.Enabled = false;
			layersToolStripMenuItem.Enabled = false;
			statsToolStripMenuItem.Enabled = false;
			viewDeathZonesToolStripMenuItem.Checked = false;

			// model library stuff
			addSelectedLevelItemsToolStripMenuItem.Enabled = false;
			addAllLevelItemsToolStripMenuItem.Enabled = false;

			editLevelInfoToolStripMenuItem.Enabled = false;
			advancedSaveSETFileToolStripMenuItem.Enabled = advancedSaveSETFileBigEndianToolStripMenuItem.Enabled = false;
			saveAdvancedToolStripMenuItem.Enabled = false;
		}
		void ShowWelcomeScreen()
		{
			WelcomeForm welcomeForm = new WelcomeForm();
			welcomeForm.showOnStartCheckbox.Checked = settingsfile.SADXLVL2.ShowWelcomeScreen;

			// subscribe to our checkchanged event
			welcomeForm.showOnStartCheckbox.CheckedChanged += (object form, EventArgs eventArg) =>
			{
				settingsfile.SADXLVL2.ShowWelcomeScreen = welcomeForm.showOnStartCheckbox.Checked;
			};

			welcomeForm.ThisToolLink.Text = "SADXLVL2 Documentation";
			welcomeForm.ThisToolLink.Visible = true;

			welcomeForm.ThisToolLink.LinkClicked += (object link, LinkLabelLinkClickedEventArgs linkEventArgs) =>
			{
				welcomeForm.GoToSite("https://github.com/sonicretro/sa_tools/wiki/SADXLVL2");
			};

			welcomeForm.ShowDialog();

			welcomeForm.Dispose();
			welcomeForm = null;
		}

		private void ShowLevelSelect()
		{
			string stageToLoad = string.Empty;
			using (LevelSelectDialog dialog = new LevelSelectDialog(levelNames))
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					stageToLoad = dialog.SelectedStage;
					skipDefs = dialog.skipobjdefs;
				}
			}

			if (!string.IsNullOrEmpty(stageToLoad))
			{
				if (isStageLoaded)
				{
					if (SavePrompt(true) == DialogResult.Cancel)
						return;
				}

				UncheckMenuItems(changeLevelToolStripMenuItem);
				CheckMenuItemByTag(changeLevelToolStripMenuItem, stageToLoad);
				LoadStage(stageToLoad);
			}
		}

		private void InitializeDirect3D()
		{
			if (d3ddevice == null)
			{
				d3ddevice = new Device(new SharpDX.Direct3D9.Direct3D(), 0, DeviceType.Hardware, RenderPanel.Handle, CreateFlags.HardwareVertexProcessing,
					new PresentParameters
					{
						Windowed = true,
						SwapEffect = SwapEffect.Discard,
						EnableAutoDepthStencil = true,
						AutoDepthStencilFormat = Format.D24X8
					});

				osd = new OnScreenDisplay(d3ddevice, Color.Red.ToRawColorBGRA());
				EditorOptions.Initialize(d3ddevice);
				Gizmo.InitGizmo(d3ddevice);
				ObjectHelper.Init(d3ddevice);
			}
		}

		private void openNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Program.SADXGameFolder == "")
			{
				ShowPathWarning();
			}
			else OpenNewProject();
		}

		private void LoadProject(string projectName)
		{
			string projectPath = Path.Combine(Program.SADXGameFolder, string.Format("projects/{0}/sadxlvl.ini", projectName));
			LoadINI(projectPath);
			ShowLevelSelect();
		}

		public bool OpenNewProject()
		{
			if (isStageLoaded)
			{
				if (SavePrompt() == DialogResult.Cancel)
					return false;
			}

			using (ProjectSelectDialog projectSelectDialog = new ProjectSelectDialog())
			{
				projectSelectDialog.LoadProjectList(Program.SADXGameFolder);

				if (projectSelectDialog.ShowDialog() == DialogResult.OK)
				{
					LoadProject(projectSelectDialog.SelectedProject);
					return true;
				}

				else return false;
			}
		}

		private void LoadINI(string filename)
		{
			isStageLoaded = false;
			ini = SAEditorCommon.IniData.Load(filename);
			currentProjectPath = Environment.CurrentDirectory = Path.GetDirectoryName(filename);
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

			// load stage lights
			string stageLightPath = Path.Combine(Environment.CurrentDirectory, "Levels", "Stage Lights.ini");

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
			if (!unsaved) return DialogResult.No;
			string dialogText = (ini != null ? "Do you want to save?" : "Are you sure?"); 
			DialogResult result = MessageBox.Show(this, dialogText, "SADXLVL2",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			switch (result)
			{
				case DialogResult.Yes:
					if (ini != null)
					{
						SaveStage(autoCloseDialog);
						unsaved = false;
					}
					break;
				case DialogResult.No:
					if (ini != null)
					{
						unsaved = false;
						return result;
					}
					else return DialogResult.Cancel;
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
			if (osd != null) osd.ClearMessageList();
			levelID = id;
			string[] itempath = levelID.Split('\\');
			levelName = itempath[itempath.Length - 1];
			LevelData.LevelName = levelName;
			log.Add("----Loading a new level: " + levelName + "----");
			Text = "SADXLVL2 - Loading " + levelName + "...";

#if !DEBUG
			backgroundWorker1.RunWorkerAsync();
#else
			backgroundWorker1_DoWork(null, null);
			backgroundWorker1_RunWorkerCompleted(null, null);
			unsaved = false;
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
				string textureFallbackPath = Path.Combine(systemFallback, pvmName) + ".PVM";
				string texturePath = Path.Combine(systemPath, pvmName) + ".PVM";

				BMPInfo[] textureBitmaps = TextureArchive.GetTextures(GamePathChecker.PathOrFallback(texturePath, textureFallbackPath));
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
					log.Add("Unable to load texture file: " + GamePathChecker.PathOrFallback(texturePath, textureFallbackPath));
				}
			}
		}

		private string GetIniFolderForGame(SA_Tools.Game game)
		{
			switch (game)
			{
				case SA_Tools.Game.SA1:
					return "SA1";

				case SA_Tools.Game.SADX:
					return "SADXPC";

				case SA_Tools.Game.SA2:
					return "SA2";

				case SA_Tools.Game.SA2B:
					return "SA2PC";
				default:
					break;
			}

			return "";
		}

		private void CopyFolder(string sourceFolder, string destinationFolder)
		{
			string[] files = Directory.GetFiles(sourceFolder);

			Directory.CreateDirectory(destinationFolder);

			foreach (string objdef in files)
			{
				System.IO.FileInfo objdefFileInfo = new System.IO.FileInfo(objdef);
				if (objdefFileInfo.Name.Equals("SADXObjectDefinitions.csproj")) continue;

				// copy
				string filePath = Path.Combine(sourceFolder, objdefFileInfo.Name);
				string destinationPath = Path.Combine(destinationFolder, objdefFileInfo.Name);
				File.Copy(filePath, destinationPath, true);
			}

			string[] directories = Directory.GetDirectories(sourceFolder);

			foreach (string directory in directories)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directory);
				if (directoryInfo.Name.Equals("bin") || directoryInfo.Name.Equals("obj")) continue;

				string copySrcDir = Path.Combine(sourceFolder, directoryInfo.Name);
				string copyDstDir = Path.Combine(destinationFolder, directoryInfo.Name);

				CopyFolder(copySrcDir, copyDstDir);
			}
		}

		private string GetObjDefsDirectory()
		{
#if DEBUG
			string objdp = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXObjectDefinitions/";
			if (Directory.Exists(objdp)) return objdp;
			else return Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/objdefs/";
#endif

#if !DEBUG
			return Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/objdefs/";
#endif
		}

		private void CopyDefaultObjectDefintions()
		{
			// get our original objdefs folder
			string originalObjdefsPath = GetObjDefsDirectory();

			// get our project objdefs folder
			string projectObjdefsPath = Path.Combine(currentProjectPath, "objdefs");

			// clear the project objdefs folder
			if (Directory.Exists(projectObjdefsPath)) Directory.Delete(projectObjdefsPath, true);

			// recusrively copy the original objdefs folder to project folder
			CopyFolder(originalObjdefsPath, projectObjdefsPath);
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
			LevelData.SuppressEvents = true;

			// HACK: Fixes Twinkle Circuit's geometry lingering if loaded before Sky Chase.
			// I'm sure the real problem is somewhere below, but this is sort of an all around cleanup.
			if (isStageLoaded)
			{
				LevelData.Clear();
				selectedItems = new EditorItemSelection();
				PointHelper.Instances.Clear();
			}

			isStageLoaded = false;

			progress.SetTask("Loading stage: " + levelName);
			progress.ResetSteps();
			progress.SetMaxSteps(steps);
			IniLevelData level = ini.Levels[levelID];

			string syspath = Path.Combine(Environment.CurrentDirectory, ini.SystemPath);
			//string modpath = ini.ModPath;

			SA1LevelAct levelact = new SA1LevelAct(level.LevelID);
			LevelData.leveltexs = null;

			Invoke((Action)progress.Show);

			if (d3ddevice == null)
			{
				progress.SetTask("Initializing Direct3D...");
				Invoke((Action)InitializeDirect3D);
				progress.StepProgress();
			}
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			progress.SetTaskAndStep("Loading level data:", "Geometry");

			if (string.IsNullOrEmpty(level.LevelGeometry))
				LevelData.geo = null;
			else
			{
				LevelData.geo = LandTable.LoadFromFile(level.LevelGeometry);
				LevelData.ClearLevelItems();

				for (int i = 0; i < LevelData.geo.COL.Count; i++)
				{
					LevelData.AddLevelItem(new LevelItem(LevelData.geo.COL[i], i, selectedItems));
				}
			}

			progress.StepProgress();
			progress.SetStep("Textures");

			LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
			LevelData.Textures = new Dictionary<string, Texture[]>();
			if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName))
			{
				string fallbackTexturePath = Path.Combine(systemFallback, LevelData.geo.TextureFileName) + ".PVM";
				string texturePath = Path.Combine(syspath, LevelData.geo.TextureFileName) + ".PVM";

				BMPInfo[] TexBmps =
					TextureArchive.GetTextures(GamePathChecker.PathOrFallback(texturePath, fallbackTexturePath));
				if (TexBmps != null)
				{
					Texture[] texs = new Texture[TexBmps.Length];
					for (int j = 0; j < TexBmps.Length; j++)
						texs[j] = TexBmps[j].Image.ToTexture(d3ddevice);
					if (!LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
						LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
					if (!LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
						LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
					LevelData.leveltexs = LevelData.geo.TextureFileName;
				}
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
						dzini[i].Flags, selectedItems));
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

			// Load Object Definitions INI file
			objdefini = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(ini.ObjectDefinitions);
			LevelData.ObjDefs = new List<ObjectDefinition>();
			LevelData.MisnObjDefs = new List<ObjectDefinition>();


			// Load SET items
			if (!string.IsNullOrEmpty(level.ObjectList) && File.Exists(level.ObjectList))
			{
				LoadObjectList(level.ObjectList);
				progress.SetTaskAndStep("Loading SET items", "Initializing...");

				// Assign SET data
				if (LevelData.ObjDefs.Count > 0)
				{
					LevelData.SETName = level.SETName ?? level.LevelID;
					string setfallback = Path.Combine(systemFallback, "SET" + LevelData.SETName + "{0}.bin");
					string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + "{0}.bin");
					LevelData.InitSETItems();
					for (int i = 0; i < LevelData.SETChars.Length; i++)
					{
						string formatted = string.Format(setstr, LevelData.SETChars[i]);
						string formattedFallback = string.Format(setfallback, LevelData.SETChars[i]);

						string useSetPath = GamePathChecker.PathOrFallback(formatted, formattedFallback);

						if (File.Exists(useSetPath))
						{
							if (progress != null) progress.SetTask("SET: " + useSetPath.Replace(Environment.CurrentDirectory, ""));
							LevelData.AssignSetList(i, SETItem.Load(useSetPath, selectedItems));
						}
						else
						{
							LevelData.AssignSetList(i, new List<SETItem>());
						}
					}
				}
				else
				{
					LevelData.NullifySETItems();
					osd.AddMessage("Object definitions not found, SET files skipped", 180);
				}
			}
			else
			{
				LevelData.NullifySETItems();
				osd.AddMessage("Object definitions not found, SET files skipped", 180);
			}

			// Load Mission SET items
			if (!string.IsNullOrEmpty(ini.MissionObjectList) && File.Exists(ini.MissionObjectList))
			{
				LoadObjectList(ini.MissionObjectList, true);

				// Assign Mission SET data
				progress.SetTaskAndStep("Loading Mission SET items", "Initializing...");

				if (LevelData.MisnObjDefs.Count > 0)
				{
					string setstrFallback = Path.Combine(systemFallback, "SETMI" + level.LevelID + "{0}.bin");
					string setstr = Path.Combine(syspath, "SETMI" + level.LevelID + "{0}.bin");

					string prmstrFallback = Path.Combine(systemFallback, "PRMMI" + level.LevelID + "{0}.bin");
					string prmstr = Path.Combine(syspath, "PRMMI" + level.LevelID + "{0}.bin");
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

						string setfmt = GamePathChecker.PathOrFallback(setNormFmt, setFallbackFmt);
						string prmfmt = GamePathChecker.PathOrFallback(prmNormFmt, prmFallbackFmt);

						if (File.Exists(setfmt)) setfile = File.ReadAllBytes(setfmt);
						if (File.Exists(prmfmt)) prmfile = File.ReadAllBytes(prmfmt);

						if (setfile != null && prmfile != null)
						{
							progress.SetTask("Mission SET: " + setfmt.Replace(Environment.CurrentDirectory, ""));

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
			string camstr = Path.Combine(syspath, "CAM" + LevelData.SETName + "{0}.bin");

			LevelData.CAMItems = new List<CAMItem>[LevelData.SETChars.Length];
			for (int i = 0; i < LevelData.SETChars.Length; i++)
			{
				List<CAMItem> list = new List<CAMItem>();
				byte[] camfile = null;

				string camfmt = string.Format(camstr, LevelData.SETChars[i]);
				string camfmtFallback = string.Format(camFallback, LevelData.SETChars[i]);

				string formatted = (GamePathChecker.PathOrFallback(camfmt, camfmtFallback));

				/*if (modpath != null && File.Exists(Path.Combine(modpath, formatted)))
					camfile = File.ReadAllBytes(Path.Combine(modpath, formatted));
				else if (File.Exists(formatted))*/
				if (File.Exists(formatted)) camfile = File.ReadAllBytes(formatted);

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

			CAMItem.Init();

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
							pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>());
							break;
						case ".vb":
							pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>());
							break;
					}
					if (pr != null)
					{
						// System, System.Core, System.Drawing, SharpDX, SharpDX.Mathematics, SharpDX.Direct3D9,
						// SADXLVL2, SAModel, SAModel.Direct3D, SA Tools, SAEditorCommon
						CompilerParameters para =
							new CompilerParameters(new string[]
							{
												"System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof(SharpDX.Mathematics.Interop.RawBool)).Location,
												Assembly.GetAssembly(typeof(Vector3)).Location, Assembly.GetAssembly(typeof(Device)).Location,
												Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof(LandTable)).Location,
												Assembly.GetAssembly(typeof(EditorCamera)).Location, Assembly.GetAssembly(typeof(SA1LevelAct)).Location,
												Assembly.GetAssembly(typeof(ObjectDefinition)).Location
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
					def.Init(level, levelact.Act);

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
			#endregion

			transformGizmo = new TransformGizmo();

			log.Add("----Level load complete: " + levelName + "----\n");
			log.WriteLog();
#if !DEBUG
			}
			catch (Exception ex)
			{
				log.Add(ex.ToString()+"\n");
				log.WriteLog();
				MessageBox.Show(
					ex.GetType().Name + ": " + ex.Message + "\nLog file has been saved.",
					"SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				initerror = true;
			}
#endif
		}

		private static ObjectDefinition CompileObjectDefinition(ObjectData defgroup, out bool errorOccured, out string errorText)
		{
			ObjectDefinition def;
			errorOccured = false;
			errorText = "";

			string codeType = defgroup.CodeType;
			string dllfile = Path.Combine("dllcache", codeType + ".dll");
			DateTime modDate = DateTime.MinValue;
			if (File.Exists(dllfile)) modDate = File.GetLastWriteTime(dllfile);
			string fp = defgroup.CodeFile.Replace('/', Path.DirectorySeparatorChar);
			if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
			{
				def =
					(ObjectDefinition)
						Activator.CreateInstance(
							Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
								.GetType(codeType));
			}
			else
			{
				string ext = Path.GetExtension(fp);
				CodeDomProvider pr = null;
				switch (ext.ToLowerInvariant())
				{
					case ".cs":
						pr = new Microsoft.CSharp.CSharpCodeProvider();
						break;
					case ".vb":
						pr = new Microsoft.VisualBasic.VBCodeProvider();
						break;
				}
				if (pr != null)
				{
					// System, System.Core, System.Drawing, SharpDX, SharpDX.Mathematics, SharpDX.Direct3D9,
					// SADXLVL2, SAModel, SAModel.Direct3D, SA Tools, SAEditorCommon
					CompilerParameters para =
						new CompilerParameters(new string[]
						{
							"System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof(SharpDX.Mathematics.Interop.RawBool)).Location,
							Assembly.GetAssembly(typeof(Vector3)).Location, Assembly.GetAssembly(typeof(Device)).Location,
							Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof(LandTable)).Location,
							Assembly.GetAssembly(typeof(EditorCamera)).Location, Assembly.GetAssembly(typeof(SA1LevelAct)).Location,
							Assembly.GetAssembly(typeof(ObjectDefinition)).Location
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
						string errors = null;
						foreach (CompilerError item in res.Errors)
							errors += String.Format("\n\n{0}, {1}: {2}", item.Line, item.Column, item.ErrorText);

						errorText = errors;
						errorOccured = true;

						def = new DefaultObjectDefinition();
					}
					else
					{
						def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(codeType));
					}
				}
				else
				{
					def = new DefaultObjectDefinition();
					errorText = "Code DOM Provider was null";
					errorOccured = true;
				}
			}

			return def;
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
			JumpToStartPos();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isStageLoaded)
			{
				if (SavePrompt(true) == DialogResult.Cancel)
					e.Cancel = true;

				LevelData.StateChanged -= LevelData_StateChanged;
			}
			try
			{
				settingsfile.SADXLVL2.AlternativeCamera = alternativeCameraToolStripMenuItem.Checked;
				settingsfile.Save();
			}
			catch { };
			AppConfig.Save();
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
			Directory.CreateDirectory(syspath);
			//string modpath = ini.ModPath;
			SA1LevelAct levelact = new SA1LevelAct(level.LevelID);

			progress.SetTaskAndStep("Saving:", "Geometry...");

			if (LevelData.geo != null)
				LevelData.geo.SaveToFile(level.LevelGeometry, LandTableFormat.SA1);

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

			if (!LevelData.SETItemsIsNull())
			{
				for (int i = 0; i < LevelData.SETChars.Length; i++)
				{
					string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + LevelData.SETChars[i] + ".bin");

					// blank the set file
					if (File.Exists(setstr) || LevelData.GetSetItemCount(i) == 0)
					{
						byte[] emptyBytes = new byte[0x20];
						File.WriteAllBytes(setstr, emptyBytes);
					}

					List<byte> file = new List<byte>(LevelData.GetSetItemCount(i) * 0x20 + 0x20);
					file.AddRange(BitConverter.GetBytes(LevelData.GetSetItemCount(i)));
					file.Align(0x20);

					foreach (SETItem item in LevelData.SETItems(i))
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
					/*if (modpath != null)
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
			unsaved = false;
			#endregion
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		internal void DrawLevel()
		{
			if (!isStageLoaded || !Enabled || DeviceResizing)
				return;

			cam.FOV = (float)(Math.PI / 4);
			cam.Aspect = RenderPanel.Width / (float)RenderPanel.Height;
			cam.DrawDistance = 100000;
			UpdateTitlebar();

			#region D3D Parameters
			Matrix projection = Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance);
			Matrix view = cam.ToMatrix();
			d3ddevice.SetTransform(TransformState.Projection, projection);
			d3ddevice.SetTransform(TransformState.View, view);
			d3ddevice.SetRenderState(RenderState.FillMode, EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderState.CullMode, EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = Color.White.ToRawColor4() };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			#endregion

			d3ddevice.BeginScene();
			//all drawings after this line
			MatrixStack transform = new MatrixStack();
			EditorOptions.RenderStateCommonSetup(d3ddevice);
			if (LevelData.leveleff != null & viewSkyboxToolStripMenuItem.Checked)
			{
				d3ddevice.SetRenderState(RenderState.ZWriteEnable, false);
				LevelData.leveleff.Render(d3ddevice, cam);
				d3ddevice.SetRenderState(RenderState.ZWriteEnable, true);
			}

			List<RenderInfo> renderlist_death = new List<RenderInfo>();
			List<RenderInfo> renderlist_geo = new List<RenderInfo>();
			List<RenderInfo> renderlist_set = new List<RenderInfo>();

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
						renderlist_geo.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			if (LevelData.StartPositions != null) renderlist_geo.AddRange(LevelData.StartPositions[LevelData.Character].Render(d3ddevice, cam, transform));

			#region Adding Death Zones
			if (LevelData.DeathZones != null & viewDeathZonesToolStripMenuItem.Checked)
			{
				foreach (DeathZoneItem item in LevelData.DeathZones)
				{
					if (item.Visible)
						renderlist_death.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			#region Adding SET Layout
			if (!LevelData.SETItemsIsNull() && viewSETItemsToolStripMenuItem.Checked)
			{
				foreach (SETItem item in LevelData.SETItems(LevelData.Character))
					renderlist_set.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding CAM Layout
			if (LevelData.CAMItems != null && viewCAMItemsToolStripMenuItem.Checked)
			{
				foreach (CAMItem item in LevelData.CAMItems[LevelData.Character])
					renderlist_set.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding Mission SET Layout
			if (LevelData.MissionSETItems != null && viewMissionSETItemsToolStripMenuItem.Checked)
			{
				foreach (MissionSETItem item in LevelData.MissionSETItems[LevelData.Character])
					renderlist_set.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding splines
			if (viewSplinesToolStripMenuItem.Checked)
			{
				foreach (SplineData spline in LevelData.LevelSplines)
					renderlist_set.AddRange(spline.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Debug Bounds Drawing
			if (boundsToolStripMenuItem.Checked)
			{
				MatrixStack debugBoundsStack = new MatrixStack();
				List<Item> selection = selectedItems.GetSelection();
				debugBoundsStack.Push();
				foreach (Item item in selection)
				{
					if (item is LevelItem)
					{
						LevelItem lvlItem = (LevelItem)item;
						boundsMesh = Direct3D.Mesh.Sphere(lvlItem.CollisionData.Bounds.Radius, 9, 9);
						debugBoundsStack.NJTranslate(lvlItem.CollisionData.Bounds.Center);
						RenderInfo info = new RenderInfo(boundsMesh, 0, debugBoundsStack.Top, boundsMaterial, null, FillMode.Solid, item.Bounds);
						renderlist_geo.Add(info);
					}
					else if (item is SETItem)
					{
						SETItem setitem = (SETItem)item;
						boundsMesh = Direct3D.Mesh.Sphere(setitem.Bounds.Radius, 9, 9);
						debugBoundsStack.NJTranslate(setitem.Bounds.Center);
						RenderInfo info = new RenderInfo(boundsMesh, 0, debugBoundsStack.Top, boundsMaterial, null, FillMode.Solid, item.Bounds);
						renderlist_set.Add(info);
					}
				}
				debugBoundsStack.Pop();
			}
			#endregion

			if (dragType == DragType.Model)
			{
				if (dragPlaceLevelModel != null && dragPlaceLevelMesh != null)
				{
					renderlist_set.AddRange(dragPlaceLevelModel.DrawModel(
						d3ddevice.GetRenderState<FillMode>(RenderState.FillMode),
						transform,
						LevelData.Textures[LevelData.leveltexs],
						dragPlaceLevelMesh,
						true));
				}
			}

			List<RenderInfo> drawqueue = new List<RenderInfo>();

			cam.DrawDistance = Math.Min(EditorOptions.RenderDrawDistance, EditorOptions.LevelDrawDistance);
			drawqueue.AddRange(RenderInfo.Queue(renderlist_geo, cam));

			cam.DrawDistance = Math.Min(EditorOptions.SetItemDrawDistance, EditorOptions.SetItemDrawDistance);
			drawqueue.AddRange(RenderInfo.Queue(renderlist_set, cam));

			cam.DrawDistance = Math.Min(EditorOptions.RenderDrawDistance, EditorOptions.RenderDrawDistance);
			RenderInfo.Draw(drawqueue, d3ddevice, cam);

			d3ddevice.SetRenderState(RenderState.ZWriteEnable, false);
			RenderInfo.Draw(renderlist_death, d3ddevice, cam);
			d3ddevice.SetRenderState(RenderState.ZWriteEnable, true);

			d3ddevice.EndScene(); // scene drawings go before this line

			#region Draw Helper Objects
			cam.DrawDistance = 100000;
			projection = Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance);
			d3ddevice.SetTransform(TransformState.Projection, projection);
			cam.BuildFrustum(view, projection);

			foreach (PointHelper pointHelper in PointHelper.Instances)
			{
				pointHelper.DrawBox(d3ddevice, cam);
			}

			transformGizmo.Draw(d3ddevice, cam);

			foreach (PointHelper pointHelper in PointHelper.Instances)
			{
				pointHelper.Draw(d3ddevice, cam);
			}

			osd.ProcessMessages();
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
			osd.AddMessage("You have just begun a Point To operation.\nLeft click on the point or item you want the selected item(s) to point to, or right click to cancel.", 300);
			isPointOperation = true;
		}

		#region User Keyboard / Mouse Methods
		private void panel1_KeyUp(object sender, KeyEventArgs e)
		{
			actionInputCollector.KeyUp(e.KeyCode);
		}

		private void panel1_KeyDown(object sender, KeyEventArgs e)
		{
			actionInputCollector.KeyDown(e.KeyCode);
		}

		private void ActionInputCollector_OnActionRelease(ActionInputCollector sender, string actionName)
		{
			if (!isStageLoaded)
				return;

			bool draw = false; // should the scene redraw after this action

			switch (actionName)
			{
				case ("Camera Mode"):
					cam.mode = (cam.mode + 1) % 2;
					string cammode = "Normal";
					if (cam.mode == 1)
					{
						cammode = "Orbit";
						if (selectedItems.GetSelection().Count > 0)
							cam.FocalPoint = Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3();
						else
							cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
					}
					osd.UpdateOSDItem("Camera mode: " + cammode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Zoom to target"):
					if (selectedItems.ItemCount > 1)
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
					osd.UpdateOSDItem("Camera zoomed to target", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Change Render Mode"):
					string rendermode = "Solid";
					if (EditorOptions.RenderFillMode == FillMode.Solid)
					{
						EditorOptions.RenderFillMode = FillMode.Point;
						rendermode = "Point";
					}
					else
					{
						EditorOptions.RenderFillMode += 1;
						if (EditorOptions.RenderFillMode == FillMode.Solid) rendermode = "Solid";
						else rendermode = "Wireframe";
					}
					osd.UpdateOSDItem("Render mode: " + rendermode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Delete"):
					foreach (Item item in selectedItems.GetSelection())
						item.Delete(selectedItems);
					selectedItems.Clear();
					draw = true;
					break;

				case ("Increase camera move speed"):
					cam.MoveSpeed += 0.0625f;
					UpdateTitlebar();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Decrease camera move speed"):
					cam.MoveSpeed = Math.Max(0.0625f, cam.MoveSpeed -= 0.0625f);
					UpdateTitlebar();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Reset camera move speed"):
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					UpdateTitlebar();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Reset Camera Position"):
					if (cam.mode == 0)
					{
						cam.Position = new Vector3();
						osd.UpdateOSDItem("Reset camera position", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
						draw = true;
					}
					break;

				case ("Reset Camera Rotation"):
					if (cam.mode == 0)
					{
						cam.Pitch = 0;
						cam.Yaw = 0;
						draw = true;
						osd.UpdateOSDItem("Reset camera rotation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
						draw = true;
					}
					break;

				case ("Next Character"):
					if (isStageLoaded)
					{
						LevelData.Character = (LevelData.Character + 1) % 6;

						if (LevelData.Character < 0)
							LevelData.Character = 5;

						characterToolStripMenuItem.DropDownItems[LevelData.Character].PerformClick();
					}
					osd.UpdateOSDItem("Current character: " + LevelData.Character.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					break;

				case ("Previous Character"):
					if (isStageLoaded)
					{
						int chr = LevelData.Character - 1;
						if (chr < 0) chr = 5;
						LevelData.Character = chr;

						characterToolStripMenuItem.DropDownItems[LevelData.Character].PerformClick();
						osd.UpdateOSDItem("Current character: " + LevelData.Character.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					}
					break;

				case ("Camera Move"):
					cameraKeyDown = false;
					UpdateCameraOSD();
					break;

				case ("Camera Zoom"):
					zoomKeyDown = false;
					UpdateCameraOSD();
					break;

				case ("Camera Look"):
					lookKeyDown = false;
					UpdateCameraOSD();
					break;

				default:
					break;
			}

			if (draw)
			{
				DrawLevel();
			}
		}

		private void UpdateCameraOSD()
		{
			if (!isStageLoaded) return;
			string cameraMode = "";
			if (cameraKeyDown) cameraMode = "Move";
			else if (zoomKeyDown) cameraMode = "Zoom";
			else if (lookKeyDown) cameraMode = "Look";
			if (cameraMode != "")
				osd.UpdateOSDItem("Camera mode: " + cameraMode, RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
		}
		private void ActionInputCollector_OnActionStart(ActionInputCollector sender, string actionName)
		{
			switch (actionName)
			{
				case ("Camera Move"):
					cameraKeyDown = true;
					UpdateCameraOSD();
					break;

				case ("Camera Zoom"):
					zoomKeyDown = true;
					UpdateCameraOSD();
					break;

				case ("Camera Look"):
					lookKeyDown = true;
					UpdateCameraOSD();
					break;

				default:
					break;
			}
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyDown(Keys.MButton);

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

							LevelData.InvalidateRenderState();
						}
						return;
					}
					if (transformGizmo != null)
					{
						if (moveModeButton.Checked)
							transformGizmo.Mode = TransformMode.TRANFORM_MOVE;
						else if (rotateModeButton.Checked)
							transformGizmo.Mode = TransformMode.TRANSFORM_ROTATE;
						else if (scaleModeButton.Checked) 
							transformGizmo.Mode = TransformMode.TRANSFORM_SCALE;
						else
							transformGizmo.Mode = TransformMode.NONE;
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
					actionInputCollector.ReleaseKeys();
					mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
					cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), new System.Drawing.Rectangle(), false, false, false, alternativeCameraToolStripMenuItem.Checked);

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
					contextMenuStrip1.Show(RenderPanel, e.Location);
					break;
			}

			//DrawLevel();
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
			viewport.Width = RenderPanel.Width;
			viewport.Height = RenderPanel.Height;
			Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
			Matrix view = d3ddevice.GetTransform(TransformState.View);
			Vector3 Near, Far;
			Near = mousepos;
			Near.Z = 0;
			Far = Near;
			Far.Z = -1;

			#region Picking Level Items
			if (LevelData.LevelItems != null)
			{
				for (int i = 0; i < LevelData.LevelItemCount; i++)
				{
					bool display = false;
					if (visibleToolStripMenuItem.Checked && LevelData.GetLevelitemAtIndex(i).Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !LevelData.GetLevelitemAtIndex(i).Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;
					if (display && layer_levelItemsToolStripMenuItem.Checked)
					{
						hit = LevelData.GetLevelitemAtIndex(i).CheckHit(Near, Far, viewport, proj, view);
						if (hit < closesthit)
						{
							closesthit = hit;
							item = LevelData.GetLevelitemAtIndex(i);
						}
					}
				}
			}
			#endregion

			#region Picking Start Positions
			if (LevelData.StartPositions != null)
			{
				hit = LevelData.StartPositions[LevelData.Character].CheckHit(Near, Far, viewport, proj, view);
				if (hit < closesthit)
				{
					closesthit = hit;
					item = LevelData.StartPositions[LevelData.Character];
				}
			}
			#endregion

			#region Picking SET Items
			if (!LevelData.SETItemsIsNull() && viewSETItemsToolStripMenuItem.Checked && layer__SETItemsToolStripMenuItem.Checked)
				foreach (SETItem setitem in LevelData.SETItems(LevelData.Character))
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
			if (LevelData.CAMItems != null && viewCAMItemsToolStripMenuItem.Checked && layer_CAMItemsToolStripMenuItem.Checked)
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

			if (LevelData.DeathZones != null && layer_deathZonesToolStripMenuItem.Checked)
			{
				foreach (DeathZoneItem dzitem in LevelData.DeathZones)
				{
					if (dzitem.Visible & viewDeathZonesToolStripMenuItem.Checked)
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
			if (LevelData.MissionSETItems != null && viewMissionSETItemsToolStripMenuItem.Checked && layer_missionSETItemsToolStripMenuItem.Checked)
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
			if (LevelData.LevelSplines != null && viewSplinesToolStripMenuItem.Checked && layer_splinesToolStripMenuItem.Checked)
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
			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyUp(Keys.MButton);

			UpdatePropertyGrid();
		}

		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!isStageLoaded)
				return;
			bool draw = false;
			bool gizmo = false;
			switch (e.Button)
			{
				case MouseButtons.Middle:
					break;

				case MouseButtons.Left:
					if (transformGizmo.TransformGizmoMove(cam.mouseDelta, cam, selectedItems))
					{
						unsaved = true;
						if (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE) gizmo = true;
					}
					foreach (PointHelper pointHelper in PointHelper.Instances)
					{
						if (pointHelper.TransformAffected(cam.mouseDelta.X / 2 * cam.MoveSpeed, cam.mouseDelta.Y / 2 * cam.MoveSpeed, cam))
							unsaved = true;
					}
					draw = true;
					break;

				case MouseButtons.None:
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
					Viewport viewport = d3ddevice.Viewport;
					viewport.Width = RenderPanel.Width;
					viewport.Height = RenderPanel.Height;
					Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
					Matrix view = d3ddevice.GetTransform(TransformState.View);
					Vector3 Near = mousepos;
					Near.Z = 0;
					Vector3 Far = Near;
					Far.Z = -1;

					GizmoSelectedAxes oldSelection = transformGizmo.SelectedAxes;
					transformGizmo.SelectedAxes = transformGizmo.CheckHit(Near, Far, viewport, proj, view, cam);
					if (oldSelection != transformGizmo.SelectedAxes)
					{
						draw = true;
						break;
					}

					foreach (PointHelper pointHelper in PointHelper.Instances)
					{
						GizmoSelectedAxes oldHelperAxes = pointHelper.SelectedAxes;
						pointHelper.SelectedAxes = pointHelper.CheckHit(Near, Far, viewport, proj, view, cam);
						if (oldHelperAxes != pointHelper.SelectedAxes) pointHelper.Draw(d3ddevice, cam);
						draw = true;
					}

					break;
			}
			mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
			int camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, cameraKeyDown, alternativeCameraToolStripMenuItem.Checked, gizmo);

			if (camresult >= 2 && selectedItems != null && selectedItems.ItemCount > 0 && propertyGrid1.ActiveControl == null) UpdatePropertyGrid();
			if (camresult >= 1 || draw)
			{
				DrawLevel();
			}

		}

		void panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (!isStageLoaded || !RenderPanel.Focused)
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
				SetGizmoPivotAndLocality();
			}
			else
			{
				if (transformGizmo != null)
				{
					transformGizmo.Enabled = false;
				}
			}

			duplicateToolStripMenuItem.Enabled = selectedItems.ItemCount > 0;
			deleteSelectedToolStripMenuItem.Enabled = selectedItems.ItemCount > 0;
			deleteToolStripMenuItem.Enabled = selectedItems.ItemCount > 0;
			addSelectedLevelItemsToolStripMenuItem.Enabled = selectedItems.Items.Count<Item>(item => item is LevelItem) > 0;

			DrawLevel();
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
					item.Delete(selectedItems);
					selitems.Add(item);
				}
			}
			selectedItems.Clear();
			LevelData.InvalidateRenderState();
			if (selitems.Count == 0) return;
			Clipboard.SetData(DataFormats.Serializable, selitems);
			unsaved = true;
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
				osd.AddMessage("Paste operation failed - feature not implemented.", 180);
				return; // todo: finish implementing proper copy/paste
			}

			List<Item> objs = (List<Item>)obj;
			Vector3 center = new Vector3();

			foreach (Item item in objs)
				center += item.Position.ToVector3();

			center = new Vector3(center.X / objs.Count, center.Y / objs.Count, center.Z / objs.Count);
			foreach (Item item in objs)
			{
				item.Position = new Vertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
				item.Paste();
			}

			selectedItems.Clear();
			selectedItems.Add(new List<Item>(objs));
			LevelData.InvalidateRenderState();
			unsaved = true;
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in selectedItems.GetSelection())
			{
				item.Delete(selectedItems);
			}

			selectedItems.Clear();
			LevelData.InvalidateRenderState();
			unsaved = true;
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

			DrawLevel();
		}

		private void levelPieceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportToStage();
		}

		private void toStageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportToStage();
		}

		private void ImportToStage()
		{
			importFileDialog.InitialDirectory = currentProjectPath;
			if (importFileDialog.ShowDialog() != DialogResult.OK)
				return;

			DialogResult userClearLevelResult = MessageBox.Show("Do you want to clear the level models first?", "Clear Level?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (userClearLevelResult == DialogResult.Cancel)
				return;

			if (userClearLevelResult == DialogResult.Yes)
			{
				DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				LevelData.ClearLevelGeometry();

				if (clearAnimsResult == DialogResult.Yes)
				{
					LevelData.ClearLevelGeoAnims();
				}
				selectedItems.Clear();
			}

			foreach (string s in importFileDialog.FileNames)
			{
				selectedItems.Add(LevelData.ImportFromFile(s, cam, out bool errorFlag, out string errorMsg, selectedItems));

				if (errorFlag)
					osd.AddMessage(errorMsg + "\n", 300);
			}

			LevelData.InvalidateRenderState();
			unsaved = true;
		}

		private void importToModelLibrary_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "Model Files|*.dae;*.obj;*.fbx;|All Files|*.*",
				InitialDirectory = currentProjectPath,
				Multiselect = true
			})
			{
				DialogResult result = fileDialog.ShowDialog();

				if (result == DialogResult.OK)
				{
					List<string> fileNames = new List<string>();

					foreach (string fileName in fileDialog.FileNames)
					{
						if (!fileNames.Contains(fileName)) fileNames.Add(fileName);
					}

					List<KeyValuePair<string, string>> failedFiles = new List<KeyValuePair<string, string>>();

					modelLibraryControl1.BeginUpdate();
					foreach (string file in fileNames)
					{
						if (File.Exists(file))
						{
							string extension = Path.GetExtension(file).ToLower();
							Attach model = null;

							switch (extension)
							{
								case (".obj"):
								case (".fbx"):
								case (".dae"):
									Assimp.AssimpContext context = new Assimp.AssimpContext();
									context.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
									Assimp.Scene scene = context.ImportFile(file, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.JoinIdenticalVertices | Assimp.PostProcessSteps.FlipUVs);
									NJS_OBJECT newmodel = SAEditorCommon.Import.AssimpStuff.AssimpImport(scene, scene.RootNode, ModelFormat.BasicDX, LevelData.Textures.Keys.ToArray(), true);
									model = newmodel.Attach;
									model.ProcessVertexData();
									model.CalculateBounds();
									break;
								case (".sa1mdl"):
									ModelFile modelFile = new ModelFile(file);
									model = modelFile.Model.Attach;
									model.ProcessVertexData();
									model.CalculateBounds();
									break;
							}

							if (model != null)
							{
								modelLibraryControl1.Add(model);
							}
						}
						else
						{
							failedFiles.Add(new KeyValuePair<string, string>(file, "File did not exist"));
						}
					}
					modelLibraryControl1.EndUpdate();

					if (failedFiles.Count > 0)
					{
						StringBuilder failReasons = new StringBuilder();
						foreach (KeyValuePair<string, string> failure in failedFiles)
						{
							failReasons.AppendFormat("{0} failed because: {1}", failure.Key, failure.Value);
						}

						osd.AddMessage(failReasons.ToString(), 300);
					}

					modelLibraryControl1.FullReRender();
				}
			}
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
					LevelData.AddSETItem(LevelData.Character, item);
					selectedItems.Clear();
					selectedItems.Add(item);
					LevelData.InvalidateRenderState();
					unsaved = true;
				}
		}

		private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Vector3 pos = cam.Position + (-20 * cam.Look);
			CAMItem item = new CAMItem(new Vertex(pos.X, pos.Y, pos.Z), selectedItems);
			LevelData.CAMItems[LevelData.Character].Add(item);
			selectedItems.Clear();
			selectedItems.Add(item);
			LevelData.InvalidateRenderState();
			unsaved = true;
			//LevelData_StateChanged();
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
					LevelData.InvalidateRenderState();
					unsaved = true;
				}
		}
		private void ExportLevelObj(string fileName, bool selectedOnly)
		{
			int stepCount = 0;
			int numSteps = 0;
			if (LevelData.TextureBitmaps != null && LevelData.TextureBitmaps.Count > 0)
			{
				stepCount = LevelData.TextureBitmaps[LevelData.leveltexs].Length;
				numSteps = stepCount;
			}
			List<COL> cols = LevelData.geo.COL;
			if (selectedOnly)
				cols = selectedItems.Items.OfType<LevelItem>().Select(a => a.CollisionData).ToList();
			stepCount += cols.Count;
			if (LevelData.geo.Anim != null)
				stepCount += LevelData.geo.Anim.Count;

			Assimp.AssimpContext context = new Assimp.AssimpContext();
			Assimp.Scene scene = new Assimp.Scene();
			scene.Materials.Add(new Assimp.Material());
			Assimp.Node n = new Assimp.Node();
			n.Name = "RootNode";
			scene.RootNode = n;
			string rootPath = Path.GetDirectoryName(fileName);
			List<string> texturePaths = new List<string>();

			ProgressDialog progress = new ProgressDialog("Exporting stage: " + levelName, stepCount, true, false);
			progress.Show(this);
			progress.SetTaskAndStep("Exporting...");

			for (int i = 0; i < numSteps; i++)
			{
				BMPInfo bmp = LevelData.TextureBitmaps[LevelData.leveltexs][i];
				texturePaths.Add(Path.Combine(rootPath, bmp.Name + ".png"));
				bmp.Image.Save(Path.Combine(rootPath, bmp.Name + ".png"));
				progress.Step = $"Texture {i + 1}/{numSteps}";
				progress.StepProgress();
				Application.DoEvents();
			}

			for (int i = 0; i < cols.Count; i++)
			{
				SAEditorCommon.Import.AssimpStuff.AssimpExport(cols[i].Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);

				progress.Step = $"Mesh {i + 1}/{cols.Count}";
				progress.StepProgress();
				Application.DoEvents();
			}

			if (LevelData.geo.Anim != null && !selectedOnly)
				for (int i = 0; i < LevelData.geo.Anim.Count; i++)
				{
					SAEditorCommon.Import.AssimpStuff.AssimpExport(LevelData.geo.Anim[i].Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
					progress.Step = $"Animation {i + 1}/{LevelData.geo.Anim.Count}";
					progress.StepProgress();
					Application.DoEvents();
				}

			string ftype = "collada";
			switch (Path.GetExtension(fileName).ToLowerInvariant())
			{
				case ".fbx":
					ftype = "fbx";
					break;
				case ".obj":
					ftype = "obj";
					break;
			}
			context.ExportFile(scene, fileName, ftype, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//

			progress.SetTaskAndStep("Export complete!");
		}

		private void deathZoneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeathZoneItem item = new DeathZoneItem(selectedItems);
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
			LevelData.DeathZones.Add(item);
			selectedItems.Clear();
			//selectedItems.Add(item); //Crashes
			LevelData.InvalidateRenderState();
			unsaved = true;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			LevelData.InvalidateRenderState();
			propertyGrid1.Refresh();
			unsaved = true;
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://github.com/sonicretro/sa_tools/issues");
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo == null) transformGizmo = new TransformGizmo();
			transformGizmo.Enabled = selectedItems.ItemCount > 0;
			DrawLevel();
		}

		private void statsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string selectionAppend = "";

			int levelItemCount = selectedItems.Items.Count(item => item is LevelItem);

			if (levelItemCount == 1)
			{
				LevelItem levelItem = (LevelItem)selectedItems.Items.First(item => item is LevelItem);
				NJS_OBJECT levelNJSObject = levelItem.CollisionData.Model;

				int faces = 0;
				int vColors = 0;


				if (levelNJSObject.Attach is BasicAttach)
				{
					BasicAttach model = ((BasicAttach)levelNJSObject.Attach);

					foreach (NJS_MESHSET meshSet in model.Mesh)
					{
						if (meshSet.VColor != null) vColors += meshSet.VColor.Length;

						switch (meshSet.PolyType)
						{
							case Basic_PolyType.Triangles:
								faces = +meshSet.Poly.Count;
								break;
							case Basic_PolyType.Quads:
								faces = +meshSet.Poly.Count;
								break;
							case Basic_PolyType.NPoly:
								faces = +meshSet.Poly.Count;
								break;
							case Basic_PolyType.Strips:
								foreach (Strip strip in meshSet.Poly)
								{
									faces += strip.Size;
								}
								break;
							default:
								break;
						}
					}

					selectionAppend = string.Format("Name: {0}\nVertices: {1}\nFaces: {2}\nVertex Colors: {3}\nMaterials: {4}\nMeshes: {5}",
					levelItem.Name, model.Vertex.Length, faces, vColors, model.Material.Count, model.Mesh.Count);
				}
			}
			else if (levelItemCount > 1)
			{
				selectionAppend = "Multiple objects selected, can't display selection stats.";
			}
			else selectionAppend = "Select a level geometry item to show selection stats.";

			MessageBox.Show("Level stats:\n" + LevelData.GetStats() + string.Format("\n\nSelection stats:\n{0}\n", selectionAppend), "Level/selection stats");
		}

		private void sETITemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			setItemsButton.Checked = viewSETItemsToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void cAMItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			camItemsButton.Checked = viewCAMItemsToolStripMenuItem.Checked;
			DrawLevel();
		}
		private void deathZonesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			deathZonesButton.Checked = viewDeathZonesToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SETFindReplace findReplaceForm = new SETFindReplace();

			DialogResult findReplaceResult = findReplaceForm.ShowDialog();

			if (findReplaceResult == DialogResult.OK)
			{
				LevelData.InvalidateRenderState();
				unsaved = true;
			}
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.DuplicateSelection(selectedItems, out bool errorFlag, out string errorMsg);
			unsaved = true;
			if (errorFlag) osd.AddMessage(errorMsg, 300);
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
		}

		void optionsEditor_UpdateKeys()
		{
			// Keybinds
			actionList.ActionKeyMappings.Clear();
			ActionKeyMapping[] newMappings = optionsEditor.GetActionkeyMappings();
			foreach (ActionKeyMapping mapping in newMappings)
				actionList.ActionKeyMappings.Add(mapping);
			actionInputCollector.SetActions(newMappings);
			string saveControlsPath = Path.Combine(Application.StartupPath, "keybinds", "SADXLVL2.ini");
			actionList.Save(saveControlsPath);
			// Other settings
			optionsEditor_FormUpdated();
		}

		void optionsEditor_FormUpdated()
		{
			settingsfile.SADXLVL2.DrawDistance_General = EditorOptions.RenderDrawDistance;
			settingsfile.SADXLVL2.DrawDistance_Geometry = EditorOptions.LevelDrawDistance;
			settingsfile.SADXLVL2.DrawDistance_SET = EditorOptions.SetItemDrawDistance;
			settingsfile.SADXLVL2.CameraModifier = cam.ModifierKey;
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
				selectModeButton.Checked = true;
				rotateModeButton.Checked = false;
				scaleModeButton.Checked = false;
				osd.UpdateOSDItem("Transform mode: Select", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				DrawLevel(); // TODO: possibly find a better way of doing this than re-drawing the entire scene? Possibly keep a copy of the last render w/o gizmo in memory?
			}
		}

		private void moveModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANFORM_MOVE;
				gizmoSpaceComboBox.Enabled = true;
				pivotComboBox.Enabled = true;
				selectModeButton.Checked = false;
				moveModeButton.Checked = true;
				rotateModeButton.Checked = false;
				scaleModeButton.Checked = false;
				SetGizmoPivotAndLocality();
				osd.UpdateOSDItem("Transform mode: Move", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
				rotateModeButton.Checked = true;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				scaleModeButton.Checked = false;
				SetGizmoPivotAndLocality();
				osd.UpdateOSDItem("Transform mode: Rotate", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				DrawLevel();
			}
		}

		private void gizmoSpaceComboBox_DropDownClosed(object sender, EventArgs e)
		{
			SetGizmoPivotAndLocality(false);
		}

		private void pivotComboBox_DropDownClosed(object sender, EventArgs e)
		{
			SetGizmoPivotAndLocality(false);
		}

		void SetGizmoPivotAndLocality(bool silent = true)
		{
			if (transformGizmo != null)
			{

				transformGizmo.LocalTransform = (gizmoSpaceComboBox.SelectedIndex != 0);
				transformGizmo.Pivot = (pivotComboBox.SelectedIndex != 0) ? Pivot.Origin : Pivot.CenterOfMass;

				if (selectedItems.ItemCount > 0)
				{
					Item firstItem = selectedItems.Get(0);
					transformGizmo.SetGizmo(
						((transformGizmo.Pivot == Pivot.CenterOfMass) ? firstItem.Bounds.Center : firstItem.Position).ToVector3(),
						firstItem.TransformMatrix);
				}

				if (!silent)
				{
					string pivotmode = "Origin";
					string globalorlocal = "Global";
					if (transformGizmo.Pivot == Pivot.CenterOfMass) pivotmode = "Center";
					if (transformGizmo.LocalTransform == true) globalorlocal = "Local";
					osd.UpdateOSDItem("Transform: " + globalorlocal + ", " + pivotmode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				}
				
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
				scaleModeButton.Checked = true;
				moveModeButton.Checked = false;
				rotateModeButton.Checked = false;
				SetGizmoPivotAndLocality();
				osd.UpdateOSDItem("Transform mode: Scale", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				DrawLevel();
			}
		}
		#endregion

		private void duplicateToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItems.ItemCount > 0)
			{
				osd.AddMessage("To use this feature you must have a selection!", 180);
				return;
			}

			DuplicateTo duplicateToWindow = new DuplicateTo(selectedItems);
			duplicateToWindow.ShowDialog();
			unsaved = true;
		}

		private void deleteAllOfTypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			transformGizmo.Enabled = false;

			SETDeleteType typeDeleter = new SETDeleteType();

			typeDeleter.ShowDialog();
			if (typeDeleter.DialogResult == DialogResult.OK) unsaved = true;
		}

		private void splinesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			splinesButton.Checked = viewSplinesToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void changeLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowLevelSelect();
		}

		private void toolClearGeometry_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all of the geometry from the stage.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearLevelGeometry();
			unsaved = true;
		}

		private void toolClearAnimations_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all of the geometry animations from the stage.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearLevelGeoAnims();
			unsaved = true;
		}

		private void toolClearSetItems_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all objects from the stage for the current character.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearSETItems(LevelData.Character);
			unsaved = true;
		}

		private void toolClearCamItems_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all camera items from the stage for the current character.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearCAMItems(LevelData.Character);
			unsaved = true;
		}

		private void toolClearMissionSetItems_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This will remove all mission objects from the stage for the current character.\n\nAre you sure you want to continue?",
				"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

			if (result != DialogResult.Yes)
				return;

			LevelData.ClearMissionSETItems(LevelData.Character);
			unsaved = true;
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
			unsaved = true;
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

				LevelData.InvalidateRenderState();
				unsaved = true;
			}
		}

		private void calculateAllBoundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (LevelItem item in LevelData.LevelItems)
				item.CalculateBounds();
			osd.UpdateOSDItem("Calculated all bounds", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
			DrawLevel();
			unsaved = true;
		}

		private void inputTestToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (ActionMapTest test = new ActionMapTest())
			{
				test.ShowDialog();
			}
		}

		#region Drag and Drop functionality
		private void DoModelDragEnter(Attach attach)
		{
			dragPlaceLevelModel = new NJS_OBJECT();
			dragPlaceLevelModel.Attach = attach;
			dragPlaceLevelMesh = dragPlaceLevelModel.Attach.CreateD3DMesh();
			dragType = DragType.Model;
			unsaved = true;
		}

		NJS_OBJECT dragPlaceLevelModel = null;
		enum DragType { None, Model, SET }
		DragType dragType = DragType.None;
		Vector3 dragPlaceLocation;
		Direct3D.Mesh dragPlaceLevelMesh = null;

		private void RenderPanel_DragEnter(object sender, DragEventArgs e)
		{
			Console.WriteLine("DragEnter");

			string dragType = (string)e.Data.GetData(DataFormats.StringFormat);

			if (dragType == "ModelLibrary")
			{
				Attach model = modelLibraryControl1.SelectedModel;

				if (model != null)
				{
					DoModelDragEnter(model);
					e.Effect = DragDropEffects.Copy;
				}
				else e.Effect = DragDropEffects.None;
			}
			unsaved = true;
		}

		private void RenderPanel_DragDrop(object sender, DragEventArgs e)
		{
			switch (dragType)
			{
				case DragType.None:
					break;

				case DragType.Model:
					COL newCollision = new COL() { Model = dragPlaceLevelModel };
					LevelItem newItem = new LevelItem(dragPlaceLevelModel.Attach, dragPlaceLevelModel.Position, dragPlaceLevelModel.Rotation, LevelData.LevelItemCount, selectedItems);
					LevelData.InvalidateRenderState();
					selectedItems.Clear();
					selectedItems.Add(newItem);

					dragPlaceLevelModel = null;
					dragPlaceLevelMesh = null;
					dragType = DragType.None;
					break;

				case DragType.SET:
					break;
				default:
					break;
			}
			unsaved = true;
		}

		private void RenderPanel_DragLeave(object sender, EventArgs e)
		{
			dragType = DragType.None;
			dragPlaceLevelMesh = null;
			dragPlaceLevelModel = null;
			unsaved = true;
		}

		private void RenderPanel_DragOver(object sender, DragEventArgs e)
		{
			// update our raycast position and re-draw the level
			Point mouseScreenPoint = new Point(e.X, e.Y);

			Point mousePanelPoint = RenderPanel.PointToClient(mouseScreenPoint);
			HitResult hitResult = PickItem(mousePanelPoint);

			float objectSize = 0;

			switch (dragType)
			{
				case DragType.None:
					break;
				case DragType.Model:
					objectSize = dragPlaceLevelModel.Attach.Bounds.Radius;
					break;
				case DragType.SET:
					break;
				default:
					break;
			}

			if (hitResult.IsHit)
			{
				dragPlaceLocation = hitResult.Position;
				//dragPlaceLocation += Vector3.Up * objectSize * 0.5f;
			}
			else
			{
				Vector3 mousepos = new Vector3(mousePanelPoint.X, mousePanelPoint.Y, 0);
				Viewport viewport = d3ddevice.Viewport;
				viewport.Width = RenderPanel.Width;
				viewport.Height = RenderPanel.Height;
				Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
				Matrix view = d3ddevice.GetTransform(TransformState.View);
				Matrix camMatrix = cam.ToMatrix();
				Vector3 Near, Far;
				Near = mousepos;
				Near.Z = 0;
				Far = Near;
				Far.Z = -1;

				Vector3 pos = viewport.Unproject(mousepos, proj, view, camMatrix);
				Vector3 dir = pos - viewport.Unproject(Far, proj, view, camMatrix);
				Ray ray = new Ray(pos, dir);

				Plane placementPlane = new Plane(cam.Position + Vector3.Down * 10, Vector3.Up);

				float intersectDistance = 0;

				bool didHitPlane = SharpDX.Collision.RayIntersectsPlane(ref ray, ref placementPlane, out intersectDistance);

				if (didHitPlane)
				{
					dragPlaceLocation = pos + (ray.Direction * intersectDistance);
					//dragPlaceLocation += Vector3.Up * objectSize * 0.5f;
				}
				else
				{
					dragPlaceLocation = cam.Position + (cam.Look * objectSize * 2);
				}
			}

			switch (dragType)
			{
				case DragType.None:
					break;
				case DragType.Model:
					dragPlaceLevelModel.Position = dragPlaceLocation.ToVertex();
					break;
				case DragType.SET:
					break;
				default:
					break;
			}

			DrawLevel();
			unsaved = true;
		}
		#endregion

		private void addSelectedLevelItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			modelLibraryControl1.BeginUpdate();
			foreach (Item _item in selectedItems.Items.Where<Item>(item => item is LevelItem))
			{
				LevelItem levelItem = _item as LevelItem;
				modelLibraryControl1.Add(levelItem.CollisionData.Model.Attach);
			}
			modelLibraryControl1.EndUpdate();
		}

		private void addAllLevelItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("This operation can be very slow! Are you sure?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (result == DialogResult.Yes)
			{
				modelLibraryControl1.BeginUpdate();

				for (int i = 0; i < LevelData.geo.COL.Count; i++)
				{
					modelLibraryControl1.Add(LevelData.geo.COL[i].Model.Attach);
				}

				modelLibraryControl1.EndUpdate();
			}
		}

		private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			modelLibraryControl1.Clear();
		}

		private void upgradeObjDefsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyDefaultObjectDefintions();
		}

		private void welcomeTutorialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowWelcomeScreen();
		}

		private void boundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void PropertiesSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) settingsfile.SADXLVL2.PropertiesSplitterPosition = PropertiesSplitter.SplitterDistance;
		}

		private void ItemsSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) settingsfile.SADXLVL2.ItemsSplitterPosition = ItemsSplitter.SplitterDistance;
		}

		private void LibrarySplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) settingsfile.SADXLVL2.LibrarySplitterPosition = LibrarySplitter.SplitterDistance;
		}

		private bool ShowPathWarning()
		{
			bool result;
			using (PathWarning dialog = new PathWarning(this, GetRecentFiles()))
			{
				if (result = (dialog.ShowDialog() == DialogResult.OK))
				{
					systemFallback = Path.GetDirectoryName(dialog.SelectedItem) + "\\System\\";
					if (AppConfig.MRUList.Count > 10)
					{
						for (int i = 9; i < AppConfig.MRUList.Count; i++)
						{
							AppConfig.MRUList.RemoveAt(i);
						}
					}
					if (!AppConfig.MRUList.Contains(dialog.SelectedItem)) AppConfig.MRUList.Insert(0, dialog.SelectedItem);
					else
					{
						AppConfig.MRUList.RemoveAt(AppConfig.MRUList.IndexOf(dialog.SelectedItem));
						AppConfig.MRUList.Insert(0, dialog.SelectedItem);
					}
					LoadINI(dialog.SelectedItem);
					ShowLevelSelect();
				}
				if (dialog.RemovedItems.Count > 0)
				{
					foreach (string deleted in dialog.RemovedItems)
					{
						if (AppConfig.MRUList.Contains(deleted)) AppConfig.MRUList.RemoveAt(AppConfig.MRUList.IndexOf(deleted));
					}
				}
			}
			return result;
		}

		private StringCollection GetRecentFiles()
		{
			if (AppConfig.MRUList == null)
				AppConfig.MRUList = new StringCollection();

			StringCollection mru = new StringCollection();

			foreach (string item in AppConfig.MRUList)
			{
				if (File.Exists(item))
				{
					mru.Add(item);
				}
			}

			AppConfig.MRUList = mru;

			return mru;
		}

		private void deathZonesButton_Click(object sender, EventArgs e)
		{
			viewDeathZonesToolStripMenuItem.Checked = !viewDeathZonesToolStripMenuItem.Checked;
			deathZonesButton.Checked = viewDeathZonesToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void settingsButton_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
		}

		private void splinesButton_Click(object sender, EventArgs e)
		{
			viewSplinesToolStripMenuItem.Checked = !viewSplinesToolStripMenuItem.Checked;
		}

		private void backgroundButton_Click(object sender, EventArgs e)
		{
			viewSkyboxToolStripMenuItem.Checked = !viewSkyboxToolStripMenuItem.Checked;
		}

		private void setItemsButton_Click(object sender, EventArgs e)
		{
			viewSETItemsToolStripMenuItem.Checked = !viewSETItemsToolStripMenuItem.Checked;
		}

		private void camItemsButton_Click(object sender, EventArgs e)
		{
			viewCAMItemsToolStripMenuItem.Checked = !viewCAMItemsToolStripMenuItem.Checked;
		}

		private void missionItemsButton_Click(object sender, EventArgs e)
		{
			viewMissionSETItemsToolStripMenuItem.Checked = !viewMissionSETItemsToolStripMenuItem.Checked;
		}

		private void backgroundToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			backgroundButton.Checked = viewSkyboxToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void missionSETItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			missionItemsButton.Checked = viewMissionSETItemsToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void JumpToOrigin()
		{
			cam.Position = new Vector3(0, 0, 0);
			cam.Yaw = 0;
			cam.Pitch = 0;
			osd.UpdateOSDItem("Jumped to origin", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
			DrawLevel(); //Had to do it twice because it doesn't draw SET objects properly if you do it once
			DrawLevel();
		}

		private void JumpToStartPos()
		{
			if (LevelData.Character < LevelData.StartPositions.Length)
			{
				cam.Position = new Vector3(LevelData.StartPositions[LevelData.Character].Position.X, LevelData.StartPositions[LevelData.Character].Position.Y + 10, LevelData.StartPositions[LevelData.Character].Position.Z);
				ushort rot = (ushort)LevelData.StartPositions[LevelData.Character].YRotation;
				cam.Yaw = (ushort)(-rot - 0x4000);
				cam.Pitch = 0;
				osd.UpdateOSDItem("Jumped to start position", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
				DrawLevel(); //Had to do it twice because it doesn't draw SET objects properly if you do it once
				DrawLevel();
			}
		}
		private void jumpToStartPositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			JumpToStartPos();
		}

		private void moveToStartButton_Click(object sender, EventArgs e)
		{
			JumpToStartPos();
		}

		private void MessageTimer_Tick(object sender, EventArgs e)
		{
			if (d3ddevice != null && osd != null)
				if (osd.UpdateTimer() == true) DrawLevel();
		}

		private void showHintsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			osd.show_osd = showHintsToolStripMenuItem.Checked;
			showHintsButton.Checked = showHintsToolStripMenuItem.Checked;
		}

		private void showHintsButton_Click(object sender, EventArgs e)
		{
			showHintsToolStripMenuItem.Checked = !showHintsToolStripMenuItem.Checked;
			showHintsButton.Checked = showHintsToolStripMenuItem.Checked;
		}

		private void lightingButton_CheckedChanged(object sender, EventArgs e)
		{
			EditorOptions.OverrideLighting = !lightingButton.Checked;
			DrawLevel();
		}

		private void jumpToOriginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			JumpToOrigin();
		}

		private void materialColorsButton_CheckedChanged(object sender, EventArgs e)
		{
			EditorOptions.IgnoreMaterialColors = !materialColorsButton.Checked;
			DrawLevel();
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
					bool bigendianbk = ByteConverter.BigEndian;
					ByteConverter.BigEndian = bd.checkBoxBigEndian.Checked;
					// PRS and REL hacks
					byte[] datafile = File.ReadAllBytes(filename);
					uint key = (uint)bd.numericKey.Value;
					if (ext == ".prs") datafile = FraGag.Compression.Prs.Decompress(datafile);
					else if (ext == ".rel")
					{
						ByteConverter.BigEndian = true;
						datafile = SA_Tools.HelperFunctions.DecompressREL(datafile);
						SA_Tools.HelperFunctions.FixRELPointers(datafile, 0xC900000);
						if (bd.comboLevelFormat.SelectedIndex == 0)
							ByteConverter.Reverse = true; // SADX GC
						key = 0xC900000; // Key always the same for REL pointers
					}
					land = new LandTable(datafile, (int)bd.numericAddress.Value, key, fmt);
					break;
			}
			InitializeDirect3D();
			selectedItems = new EditorItemSelection();
			PointHelper.Instances.Clear();
			LevelData.leveltexs = null;
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			LevelData.geo = land;
			LevelData.ClearLevelItems();
			LevelData.LevelSplines = new List<SplineData>();
			LevelData.ObjDefs = new List<ObjectDefinition>();
			SplineData.Init();
			for (int i = 0; i < LevelData.geo.COL.Count; i++)
			{
				LevelData.AddLevelItem(new LevelItem(LevelData.geo.COL[i], i, selectedItems));
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
		}
		private void loadLandtableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "sa1lvl",
				Filter = "Landtable Files|*.sa1lvl;*.sa2lvl;*.sa2blvl|Binary Files|*.exe;*.dll;*.bin;*.rel;*.prs|All Files|*.*",
				InitialDirectory = currentProjectPath,
				Multiselect = false
			})
			{
				DialogResult result = fileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					LoadLandtable(fileDialog.FileName);
				}
			}
		}

		private void loadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "pvm",
				Filter = "Texture Archives|*.pvm;*.gvm",
				InitialDirectory = currentProjectPath,
				Multiselect = false
			})
			{
				if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName)) fileDialog.FileName = LevelData.geo.TextureFileName;
				DialogResult result = fileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					if (string.IsNullOrEmpty(LevelData.geo.TextureFileName)) LevelData.geo.TextureFileName = Path.GetFileNameWithoutExtension(fileDialog.FileName);
					LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
					LevelData.Textures = new Dictionary<string, Texture[]>();
					BMPInfo[] TexBmps = TextureArchive.GetTextures(fileDialog.FileName);
					if (TexBmps != null)
					{
						Texture[] texs = new Texture[TexBmps.Length];
						for (int j = 0; j < TexBmps.Length; j++)
							texs[j] = TexBmps[j].Image.ToTexture(d3ddevice);
						if (!LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
							LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
						if (!LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
							LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
						LevelData.leveltexs = LevelData.geo.TextureFileName;
					}
					unloadTexturesToolStripMenuItem.Enabled = true;
					DrawLevel();
				}
			}
		}

		private void unloadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.leveltexs = null;
			LevelData.Textures = null;
			unloadTexturesToolStripMenuItem.Enabled = false;
			DrawLevel();
		}

		private void loadSETFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "bin",
				Filter = "SET Files|SET*.BIN",
				Multiselect = false
			})
			{
				DialogResult result = fileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					if (LevelData.SETItemsIsNull()) LevelData.InitSETItems();
					LevelData.SETName = Path.GetFileNameWithoutExtension(fileDialog.FileName);
					for (int i = 0; i < LevelData.SETChars.Length; i++)
					{
						if (LevelData.SETItems(i) == null)
							LevelData.AssignSetList(i, new List<SETItem>());
					}
					LevelData.AssignSetList(LevelData.Character, SETItem.Load(fileDialog.FileName, selectedItems));
					bool isSETPreset = !LevelData.SETItemsIsNull();
					objectToolStripMenuItem.Enabled = isSETPreset;
					editSETItemsToolStripMenuItem.Enabled = advancedSaveSETFileBigEndianToolStripMenuItem.Enabled = advancedSaveSETFileToolStripMenuItem.Enabled = unloadSETFileToolStripMenuItem.Enabled = addSETItemToolStripMenuItem.Enabled = LevelData.SETItemsIsNull() != true;
					LevelData.StateChanged += LevelData_StateChanged;
					LevelData.InvalidateRenderState();
				}
			}
		}

		private void Resize()
		{
			// Causes a memory leak so not used for now
			if (d3ddevice == null) return;
			DeviceResizing = true;
			d3ddevice.Dispose();
			LevelData.Textures = new Dictionary<string, Texture[]>();
			SharpDX.Direct3D9.Direct3D d3d = new SharpDX.Direct3D9.Direct3D();
			d3ddevice = new Device(d3d, 0, DeviceType.Hardware, RenderPanel.Handle, CreateFlags.HardwareVertexProcessing,
			new PresentParameters
			{
				Windowed = true,
				SwapEffect = SwapEffect.Discard,
				EnableAutoDepthStencil = true,
				AutoDepthStencilFormat = Format.D24X8
			});
			if (LevelData.TextureBitmaps != null)
			{
				foreach (var item in LevelData.TextureBitmaps)
				{
					Texture[] texs = new Texture[item.Value.Length];
					for (int j = 0; j < item.Value.Length; j++)
						texs[j] = item.Value[j].Image.ToTexture(d3ddevice);
					LevelData.Textures[item.Key] = texs;
				}
			}
			osd = new OnScreenDisplay(d3ddevice, Color.Red.ToRawColorBGRA());
			EditorOptions.Initialize(d3ddevice);
			Gizmo.InitGizmo(d3ddevice);
			ObjectHelper.Init(d3ddevice);
			if (currentLightList != null && currentLightList.Count > 0) LoadLights(currentLightList);
			DeviceResizing = false;
			if (isStageLoaded) LevelData.InvalidateRenderState();
		}
		private void LoadLights(List<SA1StageLightData> lightList)
		{
			for (int i = 0; i < 4; i++)
			{
				d3ddevice.EnableLight(i, false);
			}
			for (int i = 0; i < lightList.Count; i++)
			{
				SA1StageLightData lightData = lightList[i];
				Light light = new Light
				{
					Type = LightType.Directional,
					Direction = lightData.Direction.ToVector3(),
				};
				light.Specular = new RawColor4(lightData.Dif, lightData.Dif, lightData.Dif, 1.0f);
				// SADXPC reuses the first light's ambient color for other lights
				light.Ambient = new RawColor4(
					lightList[0].AmbientRGB.X,
					lightList[0].AmbientRGB.Y,
					lightList[0].AmbientRGB.Z,
					1.0f);
				light.Diffuse = new RawColor4(
					lightData.RGB.X * lightData.Multiplier,
					lightData.RGB.Y * lightData.Multiplier,
					lightData.RGB.Z * lightData.Multiplier,
					1.0f);
				d3ddevice.SetLight(i, ref light);
				d3ddevice.EnableLight(i, lightData.UseDirection);
			}
		}

		private void disableModelLibraryToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (disableModelLibraryToolStripMenuItem.Checked)
			{
				LibrarySplitter.Panel2Collapsed = true;
				LibrarySplitter.Panel2.Hide();
			}
			else
			{
				LibrarySplitter.Panel2Collapsed = false;
				LibrarySplitter.Panel2.Show();
			}
			settingsfile.SADXLVL2.DisableModelLibrary = disableModelLibraryToolStripMenuItem.Checked;
			modelLibraryToolStripMenuItem.Visible = !disableModelLibraryToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
		}

		private void LoadStageLights(SA1LevelAct levelact)
		{
			if ((stageLightList != null) && (stageLightList.Count > 0))
			{
				currentLightList = new List<SA1StageLightData>();

				foreach (SA1StageLightData lightData in stageLightList)
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
									if (daytimeToolStripMenuItem.Checked && lightData.Act == 0) currentLightList.Add(lightData);
									else if (eveningToolStripMenuItem.Checked && lightData.Act == 1) currentLightList.Add(lightData);
									else if (nightToolStripMenuItem.Checked && lightData.Act == 3) currentLightList.Add(lightData);
									break;
								case 2:
								case 5:
									if (lightData.Act == 2) currentLightList.Add(lightData); // TP entrance doesn't use Stage Lights though
									break;
							}

							if ((levelact.Act == 2 || levelact.Act == 5) && lightData.Act == 2) currentLightList.Add(lightData); // TP entrance doesn't use Stage Lights though

						}
						else if (levelact.Level == SA1LevelIDs.MysticRuins)
						{
							// 0 - day, 1 - evening, 2 - night, 3 - base
							if (levelact.Act == 3 && lightData.Act == 3) currentLightList.Add(lightData);
							else if (daytimeToolStripMenuItem.Checked && lightData.Act == 0) currentLightList.Add(lightData);
							else if (eveningToolStripMenuItem.Checked && lightData.Act == 1) currentLightList.Add(lightData);
							else if (nightToolStripMenuItem.Checked && lightData.Act == 2) currentLightList.Add(lightData);
						}
						else if (lightData.Act == levelact.Act)
							currentLightList.Add(lightData);
					}
				}

				if (levelact.Act > 0 && currentLightList.Count <= 0)
				{
					for (int i = 1; i < levelact.Act + 1; i++)
					{
						foreach (SA1StageLightData lightData in stageLightList)
						{
							if ((lightData.Level == levelact.Level) && (lightData.Act == levelact.Act - i))
								currentLightList.Add(lightData);
						}
					}
				}

				if (currentLightList.Count > 0)
				{
					LoadLights(currentLightList);
				}
				else
				{
					osd.AddMessage("No lights were found for this stage. Using default lights instead.", 180);
					log.Add("No lights were found for this stage. Using default lights.");
					EditorOptions.SetDefaultLights(d3ddevice, false);
				}
			}
		}

		private void loadObjectListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "Object List Files|*.INI",
				Multiselect = false
			})
			{
				DialogResult result = fileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					LoadObjectList(fileDialog.FileName);
				}
			}
		}

		private void LoadObjectList(string objectList, bool Mission = false)
		{
			List<ObjectData> objectErrors = new List<ObjectData>();
			ObjectListEntry[] objlstini = ObjectList.Load(objectList, false);
			if (Mission)
				LevelData.MisnObjDefs = new List<ObjectDefinition>();
			else
			{
				LevelData.ObjDefs = new List<ObjectDefinition>();
				Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;
			}

			List<KeyValuePair<string, string>> compileErrors = new List<KeyValuePair<string, string>>();

			for (int ID = 0; ID < objlstini.Length; ID++)
			{
				string codeaddr = objlstini[ID].CodeString;
				ObjectData defgroup;
				ObjectDefinition def;
				if (objdefini == null)
				{
					skipDefs = true;
					defgroup = new ObjectData();
				}
				else
				{
					if (!objdefini.ContainsKey(codeaddr))
						codeaddr = "0";
					defgroup = objdefini[codeaddr];

				}

				if (!skipDefs && !string.IsNullOrEmpty(defgroup.CodeFile))
				{
					if (progress != null) progress.SetStep("Compiling: " + defgroup.CodeFile);

					bool errorOccured = false;
					string errorText = "";

					Environment.CurrentDirectory = currentProjectPath;
					def = CompileObjectDefinition(defgroup, out errorOccured, out errorText);

					if (errorOccured)
					{
						KeyValuePair<string, string> errorValue = new KeyValuePair<string, string>(
							defgroup.CodeFile, errorText);

						compileErrors.Add(errorValue);
					}
				}
				else
				{
					def = new DefaultObjectDefinition();
				}

				if (Mission)
					LevelData.MisnObjDefs.Add(def);
				else
					LevelData.ObjDefs.Add(def);

				// The only reason .Model is checked for null is for objects that don't yet have any
				// models defined for them. It would be annoying seeing that error all the time!
				if (string.IsNullOrEmpty(defgroup.CodeFile) && !string.IsNullOrEmpty(defgroup.Model))
				{
					if (progress != null) progress.SetStep("Loading: " + defgroup.Model);
					// Otherwise, if the model file doesn't exist and/or no texture file is defined,
					// load the "default object" instead ("?").
					if (!File.Exists(defgroup.Model) || string.IsNullOrEmpty(defgroup.Texture) ||
						(LevelData.Textures == null || !LevelData.Textures.ContainsKey(defgroup.Texture)))
					{
						ObjectData error = new ObjectData { Name = defgroup.Name, Model = defgroup.Model, Texture = defgroup.Texture };
						objectErrors.Add(error);
						defgroup.Model = null;
					}
				}

				def.Init(defgroup, objlstini[ID].Name);
				def.SetInternalName(objlstini[ID].Name);
			}

			if (compileErrors.Count > 0)
			{
				DialogResult result = MessageBox.Show("There were compile errors. Would you like to try upgrading the object definitions? This will over-write any changes to them that you've made!",
					"Would you like to try upgrading?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (result == DialogResult.Yes)
				{
					CopyDefaultObjectDefintions();
					initerror = true;
					MessageBox.Show("Please reload the level to complete the operation.", "SADXLVL2", MessageBoxButtons.OK);
					return;
				}
			}

			if (objectErrors.Count > 0)
			{
				int count = objectErrors.Count;
				List<string> errorStrings = new List<string> { "\nSET object load errors:" };

				foreach (ObjectData o in objectErrors)
				{
					bool texEmpty = string.IsNullOrEmpty(o.Texture);
					bool texExists = (!string.IsNullOrEmpty(o.Texture) && LevelData.Textures != null && LevelData.Textures.ContainsKey(o.Texture));
					errorStrings.Add("");
					errorStrings.Add("Object:\t\t" + o.Name);
					errorStrings.Add("\tModel:");
					errorStrings.Add("\t\tName:\t" + o.Model);
					errorStrings.Add("\t\tExists:\t" + File.Exists(o.Model));
					errorStrings.Add("\tTexture:");
					errorStrings.Add("\t\tName:\t" + ((texEmpty) ? "(N/A)" : o.Texture));
					errorStrings.Add("\t\tExists:\t" + texExists);
				}
				log.AddRange(errorStrings);
				osd.AddMessage(levelName + ":\n" + count + (Mission ? " Mission SET" : " SET") + (count == 1 ? " object " : " objects ") + "failed to load their model(s).\n"
									+ "Please check SET object load errors in the log for details.\n", 300);
				log.WriteLog();
			}

			LevelData.StateChanged += LevelData_StateChanged;
			LevelData.InvalidateRenderState();

		}

		private void loadObjectDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "Object Definition Files|*.INI",
				Multiselect = false
			})
			{
				DialogResult result = fileDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					objdefini = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(fileDialog.FileName);
					currentProjectPath = Path.GetDirectoryName(fileDialog.FileName);
				}
			}

		}

		private void unloadSETFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.AssignSetList(LevelData.Character, new List<SETItem>());
			LevelData.StateChanged += LevelData_StateChanged;
			LevelData.InvalidateRenderState();
		}

		private void unloadObjectListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.ObjDefs = new List<ObjectDefinition>();
			LevelData.StateChanged += LevelData_StateChanged;
			LevelData.InvalidateRenderState();
		}

		private void daytimeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			daytimeToolStripMenuItem.Checked = true;
			eveningToolStripMenuItem.Checked = false;
			nightToolStripMenuItem.Checked = false;
			SA1LevelAct levelact = new SA1LevelAct(ini.Levels[levelID].LevelID);
			if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins)
				LoadStageLights(levelact);
		}

		private void eveningToolStripMenuItem_Click(object sender, EventArgs e)
		{
			daytimeToolStripMenuItem.Checked = false;
			eveningToolStripMenuItem.Checked = true;
			nightToolStripMenuItem.Checked = false;
			SA1LevelAct levelact = new SA1LevelAct(ini.Levels[levelID].LevelID);
			if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins)
				LoadStageLights(levelact);
		}

		private void nightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			daytimeToolStripMenuItem.Checked = false;
			eveningToolStripMenuItem.Checked = false;
			nightToolStripMenuItem.Checked = true;
			SA1LevelAct levelact = new SA1LevelAct(ini.Levels[levelID].LevelID);
			if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins)
				LoadStageLights(levelact);
		}

		private void exportAssimpSelectedItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.dae;*.obj;*.fbx",
				InitialDirectory = currentProjectPath
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					ExportLevelObj(a.FileName, true);
				}
			}
		}

		private void exportSA1MDLSelectedItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog() { })
			{
				if (folderBrowser.ShowDialog() == DialogResult.OK)
				{
					foreach (Item selectedItem in selectedItems.Items)
					{
						if (selectedItem is LevelItem)
						{
							LevelItem levelItem = selectedItem as LevelItem;
							string path = Path.Combine(folderBrowser.SelectedPath, levelItem.CollisionData.Model.Name + ".sa1mdl");

							ModelFile.CreateFile(path, levelItem.CollisionData.Model, null, "", "", null, ModelFormat.Basic);
						}
					}
				}
			}
		}

		private void exportAssimpLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.dae;*.obj;*.fbx",
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					ExportLevelObj(a.FileName, false);
				}
			}
		}

		private void WriteStructMetadata(StreamWriter sw, bool level, LandTableFormat fmt, string[] texnames = null)
		{
			sw.Write("/* Sonic Adventure ");
			switch (fmt)
			{
				case LandTableFormat.SA1:
					sw.Write("1");
					break;
				case LandTableFormat.SADX:
					sw.Write("DX");
					break;
				case LandTableFormat.SA2:
					sw.Write("2");
					break;
				case LandTableFormat.SA2B:
					sw.Write("2: Battle");
					break;
			}
			if (level)
				sw.WriteLine(" LandTable");
			else
				sw.WriteLine(" Model");
			sw.WriteLine(" * ");
			sw.WriteLine(" * Generated by SADXLVL2");
			sw.WriteLine(" * ");
			if (!string.IsNullOrEmpty(LevelData.geo.Description))
			{
				sw.Write(" * Description: ");
				sw.WriteLine(LevelData.geo.Description);
				sw.WriteLine(" * ");
			}
			if (!string.IsNullOrEmpty(LevelData.geo.Author))
			{
				sw.Write(" * Author: ");
				sw.WriteLine(LevelData.geo.Author);
				sw.WriteLine(" * ");
			}
			sw.WriteLine(" */");
			sw.WriteLine();
			if (texnames != null)
			{
				sw.Write("enum {0}TexName", LevelData.leveltexs);
				sw.WriteLine();
				sw.WriteLine("{");
				sw.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texnames));
				sw.WriteLine("};");
				sw.WriteLine();
			}
		}

		private void exportStructs(string filename, bool selectedOnly)
		{

			LandTableFormat fmt = LevelData.geo.Format;
			switch (fmt)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					if (usetBasicDXFormatToolStripMenuItem.Checked) fmt = LandTableFormat.SADX;
					else fmt = LandTableFormat.SA1;
					break;
			}
			List<string> labels = new List<string>() { LevelData.geo.Name };
			string[] texnames = null;
			if (LevelData.leveltexs != null && exportTextureNamesToolStripMenuItem.Checked)
			{
				texnames = new string[LevelData.TextureBitmaps[LevelData.leveltexs].Length];
				for (int i = 0; i < LevelData.TextureBitmaps[LevelData.leveltexs].Length; i++)
					texnames[i] = string.Format("{0}TexName_{1}", LevelData.leveltexs,
						LevelData.TextureBitmaps[LevelData.leveltexs][i].Name);

			}
			if (!selectedOnly)
			{
				using (StreamWriter sw = File.CreateText(filename))
				{
					WriteStructMetadata(sw, true, fmt, texnames);
					LevelData.geo.ToStructVariables(sw, fmt, labels, texnames);
					return;
				}
			}
			else
			{
				foreach (Item selectedItem in selectedItems.Items)
				{
					if (selectedItem is LevelItem)
					{
						LevelItem levelItem = selectedItem as LevelItem;
						string path = Path.Combine(filename, levelItem.CollisionData.Model.Name + ".c");
						using (StreamWriter sw = File.CreateText(path))
						{
							WriteStructMetadata(sw, false, fmt, texnames);
							levelItem.CollisionData.Model.ToStructVariables(sw, usetBasicDXFormatToolStripMenuItem.Checked, labels, texnames);
						}
					}
				}
				return;
			}
		}

		private void exportSelectedItemsStructsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog() { })
				if (folderBrowser.ShowDialog() == DialogResult.OK)
				{
					exportStructs(folderBrowser.SelectedPath, true);
				}
		}

		private void exportLevelStructsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "c", Filter = "C file|*.c" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					exportStructs(sd.FileName, false);
				}
		}

		private void editLevelInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (EditLevelInfoDialog dlg = new EditLevelInfoDialog())
			{
				dlg.ShowDialog(this);
				if (dlg.DialogResult == DialogResult.OK) unsaved = true;
			}

		}

		private void advancedSavelevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "sa1lvl",
				Filter = "SA1/SADX Level Files|*.sa1lvl",
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					LevelData.geo.SaveToFile(a.FileName, LandTableFormat.SADX);
				}
			}
		}

		private void SaveSETFile(bool bigendian)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "bin",
				Filter = "SET files|SET*.bin",
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					{
						SETItem.Save(LevelData.SETItems(LevelData.Character).ToList(), a.FileName, bigendian);
					}
				}
			}
		}
		private void advancedSaveSETFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveSETFile(false);
		}

		private void advancedSaveSETFileBigEndianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveSETFile(true);
		}

		private void boundsToolStripMenuItem_Click_1(object sender, EventArgs e)
		{

		}
	}
}