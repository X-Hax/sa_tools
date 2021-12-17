using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using System.Text;
using SharpDX.Mathematics.Interop;
using static SAModel.SAEditorCommon.SettingsFile;

namespace SAModel.SALVL
{
	// TODO: Organize this whole class.
	// TODO: Unify as much SET/CAM load and save code as possible. They're practically identical.
	// TODO: Rename controls to be more distinguishable.
	// (Example: sETItemsToolStripMenuItem1 is a dropdown menu. sETITemsToolStripMenuItem is a toggle.)
	public partial class MainForm : Form
	{
		Settings_SALVL settingsfile; // For user editable settings
		Properties.Settings AppConfig = Properties.Settings.Default; // For non-user editable settings in SALVL.config
        ProgressDialog progress;
        bool initerror = false;
		bool NeedRedraw;
		bool NeedPropertyRefresh;
		bool AnimationPlaying;

        public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			Application.Idle += HandleWaitLoop;
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
			string errDesc = "SALVL has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
				"If you wish to report a bug, please include the following in your report:";
			ErrorDialog report = new ErrorDialog("SALVL", errDesc, log.GetLogString());
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
		SAEditorCommon.IniData sadxlvlini;
		Logger log = new Logger();
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

		// light lists
		List<SADXStageLightData> stageLightList;
		List<LSPaletteData> characterLightList;

		#endregion

		#region UI & Customization
		bool skipDefs;
		bool lookKeyDown;
		bool zoomKeyDown;
		bool cameraKeyDown;
		Point menuLocation;
		bool isPointOperation;
		bool unsaved;

		System.Drawing.Rectangle mouseBounds;
		bool FormResizing;
		FormWindowState LastWindowState = FormWindowState.Minimized;

		TransformGizmo transformGizmo;
		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;

		#endregion

		// project support stuff
		string systemFallback; // The system/SONICADV folder for cases when the mod doesn't have replacement files
		string modFolder; // The mod's main folder
        string modSystemFolder; // The mod's "system" folder, such as SONICADV in SA1 or SYSTEM in SADX
		Dictionary<string, ObjectData> objdefini;

		private void MainForm_Load(object sender, EventArgs e)
		{
			AnimationTimer = new HiResTimer(16.667f);
			AnimationTimer.Elapsed += new EventHandler<HiResTimerElapsedEventArgs>(AdvanceAnimation);
			settingsfile = Settings_SALVL.Load();
			progress = new ProgressDialog("SALVL", 11, false, true, true);
			modelLibraryControl1.InitRenderer();
			InitGUISettings();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			LevelData.StateChanged += LevelData_StateChanged;
			LevelData.PointOperation += LevelData_PointOperation;
			RenderPanel.MouseWheel += panel1_MouseWheel;
			InitDisableInvalidControls();
			log.DeleteLogFile();
			log.Add("SALVL: New log entry on " + DateTime.Now.ToString("G") + "\n");
			log.Add("Build Date: ");
			log.Add(File.GetLastWriteTime(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture));
			log.Add("OS Version: ");
			log.Add(Environment.OSVersion.ToString() + System.Environment.NewLine);
			AppConfig.Reload();
			EditorOptions.RenderDrawDistance = settingsfile.DrawDistance_General;
			EditorOptions.LevelDrawDistance = settingsfile.DrawDistance_Geometry;
			EditorOptions.SetItemDrawDistance = settingsfile.DrawDistance_SET;
			EditorOptions.FillColor = Color.FromArgb(settingsfile.BackgroundColor);
			disableModelLibraryToolStripMenuItem.Checked = settingsfile.DisableModelLibrary;
			hideCursorDuringCameraMovementToolStripMenuItem.Checked = settingsfile.AlternativeCamera;
			wrapAroundScreenEdgesToolStripMenuItem.Checked = settingsfile.MouseWrapScreen;
			if (settingsfile.ShowWelcomeScreen)
				ShowWelcomeScreen();

            // MRU list
            System.Collections.Specialized.StringCollection newlist = new System.Collections.Specialized.StringCollection();
            if (AppConfig.MRUList != null)
            {
                foreach (string file in AppConfig.MRUList)
                {
                    if (File.Exists(file))
                    {
                        newlist.Add(file);
                        recentFilesToolStripMenuItem.DropDownItems.Add(file.Replace("&", "&&"));
                    }
                }
            }
            AppConfig.MRUList = newlist;

			actionList = ActionMappingList.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SALVL_keys.ini"),
				DefaultActionList.DefaultActionMapping);

			actionInputCollector = new ActionInputCollector();
			actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			actionInputCollector.OnActionStart += ActionInputCollector_OnActionStart;
			actionInputCollector.OnActionRelease += ActionInputCollector_OnActionRelease;

			cam.ModifierKey = settingsfile.CameraModifier;
			optionsEditor = new EditorOptionsEditor(cam, actionList.ActionKeyMappings.ToArray(), DefaultActionList.DefaultActionMapping, true, true);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.FormUpdatedKeys += optionsEditor_UpdateKeys;

			sceneGraphControl1.InitSceneControl(selectedItems);

			if (Program.args.Length > 0)
                OpenAnyFile(Program.args[0]);
		}

		private void InitGUISettings()
		{
			if (settingsfile.PropertiesSplitterPosition != 0) PropertiesSplitter.SplitterDistance = settingsfile.PropertiesSplitterPosition;
			if (settingsfile.LibrarySplitterPosition != 0) LibrarySplitter.SplitterDistance = settingsfile.LibrarySplitterPosition;
			if (settingsfile.ItemsSplitterPosition != 0) ItemsSplitter.SplitterDistance = settingsfile.ItemsSplitterPosition;
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
			welcomeForm.showOnStartCheckbox.Checked = settingsfile.ShowWelcomeScreen;

			// subscribe to our checkchanged event
			welcomeForm.showOnStartCheckbox.CheckedChanged += (object form, EventArgs eventArg) =>
			{
				settingsfile.ShowWelcomeScreen = welcomeForm.showOnStartCheckbox.Checked;
			};

			welcomeForm.ThisToolLink.Text = "SALVL Documentation";
			welcomeForm.ThisToolLink.Visible = true;

			welcomeForm.ThisToolLink.LinkClicked += (object link, LinkLabelLinkClickedEventArgs linkEventArgs) =>
			{
				welcomeForm.GoToSite("https://github.com/X-Hax/sa_tools/wiki/SALVL");
			};

			welcomeForm.ShowDialog();

			welcomeForm.Dispose();
			welcomeForm = null;
		}

        private void InitializeDirect3D()
		{
			if (d3ddevice == null)
			{
				d3ddevice = new Device(new SharpDX.Direct3D9.Direct3DEx(), 0, DeviceType.Hardware, RenderPanel.Handle, CreateFlags.HardwareVertexProcessing,
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
	
		/// <summary>
		/// Displays a dialog asking if the user would like to save.
		/// </summary>
		/// <param name="autoCloseDialog">Defines whether or not the save progress dialog should close on completion.</param>
		/// <returns></returns>
		private DialogResult SavePrompt(bool autoCloseDialog = false)
		{
			if (!unsaved) return DialogResult.No;
			string dialogText = (sadxlvlini != null ? "Do you want to save?" : "Quit without saving?");
			DialogResult result = MessageBox.Show(this, dialogText, "SALVL",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			switch (result)
			{
				case DialogResult.Yes:
					if (sadxlvlini != null)
					{
						SaveStage(autoCloseDialog);
						unsaved = false;
					}
					break;
				case DialogResult.No:
					if (sadxlvlini != null)
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
				settingsfile.AlternativeCamera = hideCursorDuringCameraMovementToolStripMenuItem.Checked;
				settingsfile.MouseWrapScreen = wrapAroundScreenEdgesToolStripMenuItem.Checked;
				settingsfile.Save();
				if (log != null) log.WriteLog();
			}
			catch { };
			AppConfig.Save();
			AnimationTimer.Stop();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveStage(false);
		}

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void UpdateTitlebar()
		{
			Text = "SALVL - " + levelName + " (" + cam.Position.X + ", " + cam.Position.Y + ", " + cam.Position.Z
				+ " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X")
				+ " Speed=" + cam.MoveSpeed + (cam.mode == 1 ? " Distance=" + cam.Distance : "") + ")";
		}

		private void LevelData_PointOperation()
		{
			osd.AddMessage("You have just begun a Point To operation.\nLeft click on the point or item you want the selected item(s) to point to, or right click to cancel.", 300);
			isPointOperation = true;
		}

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

            NeedRedraw = true;
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

			NeedRedraw = true;
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

			NeedRedraw = true;
		}

		private void levelPieceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportLevelItem();
		}

		private void toStageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportLevelItem();
		}

       	private void importToModelLibrary_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "Model Files|*.dae;*.obj;*.fbx;|All Files|*.*",
				InitialDirectory = modFolder,
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
			item.Scale = new Vertex(10, 10, 10);
            selectedItems.Clear();
            LevelData.CAMItems[LevelData.Character].Add(item);
            LevelData.InvalidateRenderState();
            selectedItems.Add(item);
			unsaved = true;
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
					selectedItems.Clear();
					LevelData.MissionSETItems[LevelData.Character].Add(item);
					LevelData.InvalidateRenderState();
					selectedItems.Add(item);
					unsaved = true;
				}
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
			NeedPropertyRefresh = true;
			SetGizmoPivotAndLocality(true);
			 unsaved = true;
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string errDesc = "You can submit a new issue on SA Tools GitHub issue tracker.\n\nPlease make sure the problem is reproducible on the latest version of SA Tools.\n\nIf you wish to report a bug, please include the following in your report:";
			ErrorDialog report = new ErrorDialog("SALVL", errDesc, log.GetLogString());
			DialogResult dgresult = report.ShowDialog();
			switch (dgresult)
			{
				case DialogResult.Abort:
				case DialogResult.OK:
					Application.Exit();
					break;
			}
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo == null) transformGizmo = new TransformGizmo();
			transformGizmo.Enabled = selectedItems.ItemCount > 0;
			SetGizmoPivotAndLocality(true);
			NeedRedraw = NeedPropertyRefresh = true;
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
			NeedRedraw = true;
		}

		private void cAMItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			camItemsButton.Checked = viewCAMItemsToolStripMenuItem.Checked;
			NeedRedraw = true;
		}
		private void deathZonesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			deathZonesButton.Checked = viewDeathZonesToolStripMenuItem.Checked;
			NeedRedraw = true;
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
			string saveControlsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SALVL_keys.ini");
			actionList.Save(saveControlsPath);
			// Other settings
			optionsEditor_FormUpdated();
		}

		void optionsEditor_FormUpdated()
		{
			settingsfile.DrawDistance_General = EditorOptions.RenderDrawDistance;
			settingsfile.DrawDistance_Geometry = EditorOptions.LevelDrawDistance;
			settingsfile.DrawDistance_SET = EditorOptions.SetItemDrawDistance;
			settingsfile.CameraModifier = cam.ModifierKey;
			settingsfile.BackgroundColor = EditorOptions.FillColor.ToArgb();
			NeedRedraw = true;
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
				NeedRedraw = true; // TODO: possibly find a better way of doing this than re-drawing the entire scene? Possibly keep a copy of the last render w/o gizmo in memory?
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
				NeedRedraw = true;
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
				NeedRedraw = true;
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

				NeedRedraw = true;
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
				NeedRedraw = true;
			}
		}
		#endregion

		private void duplicateToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedItems.ItemCount < 1)
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
			NeedRedraw = true;
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
            LevelData.LevelSplines.Clear();
            LevelData.ClearLevelGeoAnims();
			LevelData.ClearLevelGeometry();
            selectedItems.Clear();
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
			LevelData.InvalidateRenderState();
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

			NeedRedraw = true;
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
			DialogResult result = MessageBox.Show("This will overwrite all object definitions in the current project.\nWould you like to continue?", "Overwrite object definitions?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				bool error = CopyDefaultObjectDefinitions();
				if (!error)
					MessageBox.Show("Please restart SALVL to complete the operation.", "SALVL", MessageBoxButtons.OK);
			}
		}

		private void welcomeTutorialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowWelcomeScreen();
		}

		private void boundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NeedRedraw = true;
		}

		private void PropertiesSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) settingsfile.PropertiesSplitterPosition = PropertiesSplitter.SplitterDistance;
		}

		private void ItemsSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) settingsfile.ItemsSplitterPosition = ItemsSplitter.SplitterDistance;
		}

		private void LibrarySplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) settingsfile.LibrarySplitterPosition = LibrarySplitter.SplitterDistance;
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
			NeedRedraw = true;
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
			NeedRedraw = true;
		}

		private void missionSETItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			missionItemsButton.Checked = viewMissionSETItemsToolStripMenuItem.Checked;
			NeedRedraw = true;
		}

		private void JumpToOrigin()
		{
			cam.Position = new Vector3(0, 0, 0);
			cam.Yaw = 0;
			cam.Pitch = 0;
			DrawLevel();
			osd.UpdateOSDItem("Jumped to origin", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
			LevelData.InvalidateRenderState();
		}

		private void JumpToStartPos()
		{
			if (LevelData.Character < LevelData.StartPositions.Length)
			{
				cam.Position = new Vector3(LevelData.StartPositions[LevelData.Character].Position.X, LevelData.StartPositions[LevelData.Character].Position.Y + 10, LevelData.StartPositions[LevelData.Character].Position.Z);
				ushort rot = (ushort)LevelData.StartPositions[LevelData.Character].YRotation;
				cam.Yaw = (ushort)(-rot - 0x4000);
				cam.Pitch = 0;
				DrawLevel();
				osd.UpdateOSDItem("Jumped to start position", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
				LevelData.InvalidateRenderState();
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
				if (osd.UpdateTimer() == true) NeedRedraw = true;
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
			NeedRedraw = true;
		}

		private void jumpToOriginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			JumpToOrigin();
		}

		private void materialColorsButton_CheckedChanged(object sender, EventArgs e)
		{
			EditorOptions.IgnoreMaterialColors = !materialColorsButton.Checked;
			NeedRedraw = true;
		}

		private void unloadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.leveltexs = null;
			LevelData.Textures = null;
			unloadTexturesToolStripMenuItem.Enabled = false;
			NeedRedraw = true;
		}

		private void DeviceReset()
		{
			if (d3ddevice == null) return;
			DeviceResizing = true;
			PresentParameters pp = new PresentParameters
			{
				Windowed = true,
				SwapEffect = SwapEffect.Discard,
				EnableAutoDepthStencil = true,
				AutoDepthStencilFormat = Format.D24X8
			};
			d3ddevice.Reset(pp);
			DeviceResizing = false;
			if (isStageLoaded) LevelData.InvalidateRenderState();
			osd.UpdateOSDItem("Direct3D device reset", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
			NeedRedraw = true;
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
			settingsfile.DisableModelLibrary = disableModelLibraryToolStripMenuItem.Checked;
			modelLibraryToolStripMenuItem.Visible = !disableModelLibraryToolStripMenuItem.Checked;
			NeedRedraw = true;
		}

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
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
			SA1LevelAct levelact = new SA1LevelAct(sadxlvlini.Levels[levelID].LevelID);
			if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins)
			{
				LoadStageLights(levelact);
				LoadCharacterLights(levelact);
			}
			NeedRedraw = true;
		}

		private void eveningToolStripMenuItem_Click(object sender, EventArgs e)
		{
			daytimeToolStripMenuItem.Checked = false;
			eveningToolStripMenuItem.Checked = true;
			nightToolStripMenuItem.Checked = false;
			SA1LevelAct levelact = new SA1LevelAct(sadxlvlini.Levels[levelID].LevelID);
			if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins)
			{
				LoadStageLights(levelact);
				LoadCharacterLights(levelact);
			}
			NeedRedraw = true;
		}

		private void nightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			daytimeToolStripMenuItem.Checked = false;
			eveningToolStripMenuItem.Checked = false;
			nightToolStripMenuItem.Checked = true;
			SA1LevelAct levelact = new SA1LevelAct(sadxlvlini.Levels[levelID].LevelID);
			if (levelact.Level == SA1LevelIDs.StationSquare || levelact.Level == SA1LevelIDs.MysticRuins)
			{
				LoadStageLights(levelact);
				LoadCharacterLights(levelact);
			}
			NeedRedraw = true;
		}

		private void exportAssimpSelectedItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.dae;*.obj;*.fbx",
				InitialDirectory = modFolder
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
						else if (selectedItem is LevelAnim)
						{
							LevelAnim levelAnim = selectedItem as LevelAnim;
							string path = Path.Combine(folderBrowser.SelectedPath, levelAnim.GeoAnimationData.ActionName + ".sa1mdl");

							ModelFile.CreateFile(path, levelAnim.GeoAnimationData.Model, null, "", "", null, ModelFormat.Basic);
							levelAnim.GeoAnimationData.Animation.Save(Path.ChangeExtension(path, ".saanim"));
							File.WriteAllText(Path.ChangeExtension(path, ".action"), Path.GetFileName(Path.ChangeExtension(path, ".saanim")));
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
		}

		private void advancedSaveSETFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveSETFile(false);
		}

		private void advancedSaveSETFileBigEndianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveSETFile(true);
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			FormResizing = false;
			DeviceReset();
		}


		private void RenderPanel_SizeChanged(object sender, EventArgs e)
		{
			if (WindowState != LastWindowState)
			{
				LastWindowState = WindowState;
				DeviceReset();
			}
			else if (!FormResizing) DeviceReset();
		}

		private void MainForm_ResizeBegin(object sender, EventArgs e)
		{
			FormResizing = true;
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			NeedRedraw = true;
		}

		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
            OpenNewProject();
        }

		private void changeLevelToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
            ShowLevelSelect();
        }

		private void loadLandtableToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
            using (OpenFileDialog fileDialog = new OpenFileDialog()
            {
                DefaultExt = "sa1lvl",
                Filter = "All Supported Files|*.sa1lvl;*.sa2lvl;*.sa2blvl;*.exe;*.dll;*.bin;*.rel;*.prs|Landtable Files|*.sa1lvl;*.sa2lvl;*.sa2blvl|Binary Files|*.exe;*.dll;*.bin;*.rel;*.prs|All Files|*.*",
                InitialDirectory = modFolder,
                Multiselect = false
            })
            {
                DialogResult result = fileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    LoadLandtable(fileDialog.FileName);
                    UpdateMRUList(fileDialog.FileName);
                }
            }
        }

		private void loadTexturesToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
            using (OpenFileDialog fileDialog = new OpenFileDialog()
            {
                DefaultExt = "pvm",
                Filter = "Texture Archives|*.pvm;*.gvm;*.prs;*.pvmx;*.pak|Texture Pack|*.txt|Supported Files|*.pvm;*.gvm;*.prs;*.pvmx;*.pak;*.txt|All Files|*.*",
                InitialDirectory = modFolder,
                Multiselect = false
            })
            {
                if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName)) 
                    fileDialog.FileName = LevelData.geo.TextureFileName;
                DialogResult result = fileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Initialize data if necessary
                    if (string.IsNullOrEmpty(LevelData.geo.TextureFileName)) 
                        LevelData.geo.TextureFileName = Path.GetFileNameWithoutExtension(fileDialog.FileName);
                    if (LevelData.TextureBitmaps == null)
                        LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
                    if (LevelData.Textures == null) 
                        LevelData.Textures = new Dictionary<string, Texture[]>();

                    // Load or replace textures
                    BMPInfo[] TexBmps = TextureArchive.GetTextures(fileDialog.FileName);
                    if (TexBmps != null)
                    {
                        // Load texture bitmaps
                        Texture[] texs = new Texture[TexBmps.Length];
                        for (int j = 0; j < TexBmps.Length; j++)
                            texs[j] = TexBmps[j].Image.ToTexture(d3ddevice);

                        // Remove old textures if they exist
                        if (LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
                            LevelData.TextureBitmaps.Remove(LevelData.geo.TextureFileName);

                        if (LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
                            LevelData.Textures.Remove(LevelData.geo.TextureFileName);

                        // Add new textures
                        LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
                        LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
                        LevelData.leveltexs = LevelData.geo.TextureFileName;
                    }
                    unloadTexturesToolStripMenuItem.Enabled = true;
                    NeedRedraw = true;
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
                    selectedItems.Clear();
                }
            }
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
                    modFolder = Path.GetDirectoryName(fileDialog.FileName);
                }
            }
        }

		private void loadCAMFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
            return;
		}

		private void splinesToolStripMenuItem_Click(object sender, EventArgs e)
		{
            DialogResult result = MessageBox.Show("This will remove all path data in the stage.\n\nAre you sure you want to continue?",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);

            if (result != DialogResult.Yes)
                return;

            LevelData.LevelSplines.Clear();
            LevelData.InvalidateRenderState();
            selectedItems.Clear();
		}

		private void wholeLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
            ImportLevelItem(true);
        }

        private void UpdateMRUList(string filename)
        {
            if (AppConfig.MRUList.Count > 10)
            {
                for (int i = 9; i < AppConfig.MRUList.Count; i++)
                {
                    AppConfig.MRUList.RemoveAt(i);
                }
            }
            if (AppConfig.MRUList.Contains(filename))
            {
                int i = AppConfig.MRUList.IndexOf(filename);
                AppConfig.MRUList.RemoveAt(i);
                recentFilesToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            AppConfig.MRUList.Insert(0, filename);
            recentFilesToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename.Replace("&", "&&")));
        }

		private void recentFilesToolStripMenuItem_DropDownItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
		{
            OpenAnyFile(AppConfig.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
        }

		private void forceClipLevelToAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
            List<SETItem> itemsToChange = LevelData.SETItems(LevelData.Character).ToList();
            foreach (SETItem item in itemsToChange)
                item.ClipLevel = 0;
			NeedPropertyRefresh = true;
		}

		private void NextAnimationFrame()
		{
			AnimationPlaying = playAnimButton.Checked = false;
			AnimationTimer.Stop();
			if (!isStageLoaded || LevelData.geo.Anim == null)
				return;
			foreach (GeoAnimData geoanim in LevelData.geo.Anim)
			{
				geoanim.AnimationFrame = (float)Math.Ceiling(geoanim.AnimationFrame + 1.0f);
				if (geoanim.AnimationFrame >= geoanim.MaxFrame)
					geoanim.AnimationFrame = 0;
			}
			osd.UpdateOSDItem("Next Animation Frame", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "anim", 120);
			NeedUpdateAnimation = true;
			NeedPropertyRefresh = true;
		}

		private void PreviousAnimationFrame()
		{
			AnimationPlaying = playAnimButton.Checked = false;
			AnimationTimer.Stop();
			if (!isStageLoaded || LevelData.geo.Anim == null)
				return;
			foreach (GeoAnimData geoanim in LevelData.geo.Anim)
			{
				geoanim.AnimationFrame = (float)Math.Floor(geoanim.AnimationFrame - 1.0f);
				if (geoanim.AnimationFrame < 0)
					geoanim.AnimationFrame = geoanim.MaxFrame;
			}
			osd.UpdateOSDItem("Previous Animation Frame", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "anim", 120);
			NeedUpdateAnimation = true;
			NeedPropertyRefresh = true;
		}

		private void ResetAnimationFrame()
		{
			AnimationPlaying = playAnimButton.Checked = false;
			AnimationTimer.Stop();
			if (!isStageLoaded || LevelData.geo.Anim == null)
				return;
			foreach (GeoAnimData geoanim in LevelData.geo.Anim)
				geoanim.AnimationFrame = 0;
			osd.UpdateOSDItem("Reset Animation Frame", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "anim", 120);
			NeedUpdateAnimation = true;
			NeedPropertyRefresh = true;
		}

		private void PlayPause()
		{
			AnimationPlaying = !AnimationPlaying;
			playAnimButton.Checked = AnimationPlaying;
			if (AnimationPlaying)
				AnimationTimer.Start();
			else
				AnimationTimer.Stop();
			osd.UpdateOSDItem("Animation " + (playAnimButton.Checked ? "started" : "stopped"), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "anim", 120);
			NeedUpdateAnimation = true;
		}

		private void resetAnimButton_Click(object sender, EventArgs e)
		{
			ResetAnimationFrame();
		}

		private void prevFrameButton_Click(object sender, EventArgs e)
		{
			PreviousAnimationFrame();
		}

		private void nextFrameButton_Click(object sender, EventArgs e)
		{
			NextAnimationFrame();
		}

		private void levelAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportLevelAnimation();
		}

		private void playAnimButton_Click(object sender, EventArgs e)
		{
			PlayPause();
		}
	}
}