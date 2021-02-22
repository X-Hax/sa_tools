using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;
using Point = System.Drawing.Point;

namespace SonicRetro.SAModel.SAMDL
{
	public partial class MainForm : Form
	{
		SettingsFile settingsfile; //For user editable settings
		Properties.Settings AppConfig = Properties.Settings.Default; // For non-user editable settings in SAMDL.config
		Logger log = new Logger(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SAMDL.log");
		System.Drawing.Rectangle mouseBounds;
		bool mouseWrapScreen = false;

		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			InitializeComponent();
			AddMouseMoveHandler(this);

			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(SAMDL_DragEnter);
			this.DragDrop += new DragEventHandler(SAMDL_DragDrop);
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

		protected override void WndProc(ref Message m)
		{
			// Suppress the WM_UPDATEUISTATE message to remove rendering flicker
			if (m.Msg == 0x128) return;
			base.WndProc(ref m);
		}

		private void SAMDL_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
		}

		private void SAMDL_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			LoadModelList(files);
		}

		public void LoadModelList(string[] files, bool cmdLoad = false)
		{
			bool modelLoaded = false; //We can only load one model at once for now. Set to true when user loads first file and ignore others.
			bool modelLoadedWarning = false; //Flag so we can warn users that they should do one model at a time.
			
			List<string> modelFiles = new List<string>(); //For multi support, if that happens
			List<string> modelImportFiles = new List<string>();
			List<string> animFiles = new List<string>();

			string modelWarning = "Only 1 model can be loaded at a time!";

			foreach (string file in files)
			{
				string extension = Path.GetExtension(file);
				switch (extension)
				{
					case ".nj":
					case ".gj":
					case ".sa1mdl":
					case ".sa2mdl":
					case ".sa2bmdl":
						if (!modelLoaded)
						{
							modelLoaded = true;
							modelFiles.Add(file);
						}
						else if (!modelLoadedWarning)
						{
							modelLoadedWarning = true;
							MessageBox.Show(modelWarning);
						}
						break;
					case ".obj":
					case ".fbx":
					case ".dae":
						if (!modelLoaded)
						{
							modelLoaded = true;
							modelImportFiles.Add(file);
						}
						else if (!modelLoadedWarning)
						{
							modelLoadedWarning = true;
							MessageBox.Show(modelWarning);
						}
						break;
					case ".saanim":
					case ".action":
					case ".json":
					case ".njm":
						animFiles.Add(file);
						break;
					default:
						break;
				}
			}
			if (modelFiles.Count > 0) { LoadFile(modelFiles[0], cmdLoad); }
			if (modelImportFiles.Count > 0) { ImportModel_Assimp(modelImportFiles[0], false); }
			if (animFiles.Count > 0) { LoadAnimation(animFiles.ToArray()); }
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			log.Add(e.Exception.ToString());
			string errDesc = "SAMDL has crashed with the following error:\n" + e.Exception.GetType().Name+".\n\n" +
				"If you wish to report a bug, please include the following in your report:";
			ErrorDialog report = new ErrorDialog("SAMDL", errDesc, log.GetLogString());
			log.WriteLog();
			DialogResult dgresult = report.ShowDialog();
			switch (dgresult)
			{
				case DialogResult.OK:
				case DialogResult.Abort:
					Application.Exit();
					break;
			}
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		EditorOptionsEditor optionsEditor;

		bool unsaved = false;
		bool loaded;
		bool DeviceResizing;
		bool rootSiblingMode = false;
		string currentFileName = "";
		NJS_OBJECT model;
		NJS_OBJECT tempmodel;
		NJS_ACTION action;
		bool hasWeight;
		List<NJS_MOTION> animations;
		NJS_MOTION animation;
		ModelFile modelFile;
		ModelFormat outfmt;
		int animnum = -1;
		int animframe = 0;
		Mesh[] meshes;
		string TexturePackName;
		BMPInfo[] TextureInfo;
		Texture[] Textures;
		ModelFileDialog modelinfo = new ModelFileDialog();
		NJS_OBJECT selectedObject;
		Dictionary<NJS_OBJECT, TreeNode> nodeDict;
		OnScreenDisplay osd;
		Mesh sphereMesh, selectedSphereMesh, modelSphereMesh, selectedModelSphereMesh;

		#region UI
		bool lookKeyDown;
		bool zoomKeyDown;
		bool cameraKeyDown;

		int cameraMotionInterval = 1;

		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;
		ModelLibraryWindow modelLibraryWindow;
		ModelLibraryControl modelLibrary;
		#endregion

		private void MainForm_Load(object sender, EventArgs e)
		{
			Assimp.Unmanaged.AssimpLibrary.Instance.LoadLibrary(Path.Combine(Application.StartupPath, "lib", "assimp.dll"));
			log.DeleteLogFile();
			log.Add("SAMDL: New log entry on " + DateTime.Now.ToString("G") + "\n");
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			SharpDX.Direct3D9.Direct3D d3d = new SharpDX.Direct3D9.Direct3D();
			d3ddevice = new Device(d3d, 0, DeviceType.Hardware, RenderPanel.Handle, CreateFlags.HardwareVertexProcessing,
			new PresentParameters
				{
					Windowed = true,
					SwapEffect = SwapEffect.Discard,
					EnableAutoDepthStencil = true,
					AutoDepthStencilFormat = Format.D24X8
				});
			osd = new OnScreenDisplay(d3ddevice, Color.Red.ToRawColorBGRA());
			AppConfig.Reload();
			settingsfile = SettingsFile.Load();

			if (settingsfile.SAMDL.ShowWelcomeScreen)
			{
				ShowWelcomeScreen();
			}

			EditorOptions.RenderDrawDistance = settingsfile.SAMDL.DrawDistance;			
			EditorOptions.Initialize(d3ddevice);
			cam.MoveSpeed = settingsfile.SAMDL.CamMoveSpeed;
			cam.ModifierKey = settingsfile.SAMDL.CameraModifier;
			alternativeCameraModeToolStripMenuItem.Checked = settingsfile.SAMDL.AlternativeCamera;
			actionList = ActionMappingList.Load(Path.Combine(Application.StartupPath, "keybinds", "SAMDL.ini"),
				DefaultActionList.DefaultActionMapping);

			actionInputCollector = new ActionInputCollector();
			actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			actionInputCollector.OnActionStart += ActionInputCollector_OnActionStart;
			actionInputCollector.OnActionRelease += ActionInputCollector_OnActionRelease;

			optionsEditor = new EditorOptionsEditor(cam, actionList.ActionKeyMappings.ToArray(), DefaultActionList.DefaultActionMapping, false, false);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.FormUpdatedKeys += optionsEditor_FormUpdatedKeys;
			optionsEditor.ResetDefaultKeybindsCommand += () =>
			{
				actionList.ActionKeyMappings.Clear();

				foreach (ActionKeyMapping keymapping in DefaultActionList.DefaultActionMapping)
				{
					actionList.ActionKeyMappings.Add(keymapping);
				}

				actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			};

			modelLibraryWindow = new ModelLibraryWindow();
			modelLibrary = modelLibraryWindow.modelLibraryControl1;
			modelLibrary.InitRenderer();
			modelLibrary.SelectionChanged += modelLibrary_SelectionChanged;

			sphereMesh = Mesh.Sphere(0.0625f, 10, 10);
			selectedSphereMesh = Mesh.Sphere(0.0625f, 10, 10, Color.Lime);
			modelSphereMesh = Mesh.Sphere(0.0625f, 10, 10, Color.Red);
			selectedModelSphereMesh = Mesh.Sphere(0.0625f, 10, 10, Color.Yellow);
			if (Program.Arguments.Length > 0)
				LoadModelList(Program.Arguments, true);
		}

		void ShowWelcomeScreen()
		{
			WelcomeForm welcomeForm = new WelcomeForm();
			welcomeForm.showOnStartCheckbox.Checked = settingsfile.SAMDL.ShowWelcomeScreen;

			// subscribe to our checkchanged event
			welcomeForm.showOnStartCheckbox.CheckedChanged += (object form, EventArgs eventArg) =>
			{
				settingsfile.SAMDL.ShowWelcomeScreen = welcomeForm.showOnStartCheckbox.Checked;
			};

			welcomeForm.ThisToolLink.Text = "SAMDL Documentation";
			welcomeForm.ThisToolLink.Visible = true;

			welcomeForm.ThisToolLink.LinkClicked += (object link, LinkLabelLinkClickedEventArgs linkEventArgs) =>
			{
				welcomeForm.GoToSite("https://github.com/sonicretro/sa_tools/wiki/SAMDL");
			};

			welcomeForm.ShowDialog();

			welcomeForm.Dispose();
			welcomeForm = null;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded && unsaved)
				switch (MessageBox.Show(this, "Do you want to save?", "SAMDL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveAsToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "All Model Files|*.sa1mdl;*.sa2mdl;*.sa2bmdl;*.nj;*.gj;*.exe;*.dll;*.bin;*.prs;*.rel|SA Tools Files|*.sa1mdl;*.sa2mdl;*.sa2bmdl|Ninja Files|*.nj;*.gj|Binary Files|*.exe;*.dll;*.bin;*.prs;*.rel|All Files|*.*"
			};
			goto loadfiledlg;
		loadfiledlg:
			DialogResult result = a.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				try
				{
					LoadFile(a.FileName);
				}
				catch (Exception ex)
				{
					log.Add("Loading the model from " + a.FileName + " failed for the following reason(s):" + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine);
					string errDesc = "SAMDL could not load the model for the reason(s) below.\n" +
						"Please check the model's type and address and try again.\n\n" +
						"If you wish to report a bug, please include the model file and this information\n" +
						"in your report.";
					ErrorDialog report = new ErrorDialog("SAMDL", errDesc, log.GetLogString());
					log.WriteLog();
					DialogResult dgresult = report.ShowDialog();
					switch (dgresult)
					{
						case DialogResult.Cancel:
							goto loadfiledlg;
							break;
						case DialogResult.Abort:
							Application.Exit();
							break;
					}			
				}
			}
		}

		public static readonly string[] SA2BMDLFiles =
		{
			"DWALKMDL",
			"EWALK2MDL",
			"SHADOW1MDL",
			"SONIC1MDL",
		};

		public static readonly string[] SA2MDLFiles =
		{
			"BKNUCKMDL",
			"BROUGEMDL",
			"BWALKMDL",
			"CHAOS0MDL",
			"CWALKMDL",
			"EGGMDL",
			"EWALK1MDL",
			"EWALKMDL",
			"HEWALKMDL",
			"HKNUCKMDL",
			"HROUGEMDL",
			"HSHADOWMDL",
			"HSONICMDL",
			"HTWALKMDL",
			"KNUCKMDL",
			"METALSONICMDL",
			"MILESMDL",
			"PSOSHADOWMDL",
			"PSOSONICMDL",
			"ROUGEMDL",
			"SONICMDL",
			"SSHADOWMDL",
			"SSONICMDL",
			"TERIOSMDL",
			"TICALMDL",
			"TWALK1MDL",
			"TWALKMDL",
			"XEWALKMDL",
			"XKNUCKMDL",
			"XROUGEMDL",
			"XSHADOWMDL",
			"XSONICMDL",
			"XTWALKMDL",
		};

		public struct BinaryModelType
		{
			public string name_or_type;
			public UInt32 key;
			public int data_type;
		};

		public static readonly BinaryModelType[] KnownBinaryFiles = new[] {
		new BinaryModelType { name_or_type = "1ST_READ", key = 0x8C010000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV00", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV02", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV03", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV0100", key = 0x0C920000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV01OBJ", key = 0x0C920000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADV0130", key = 0x0C920000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_GARDEN00", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_GARDEN01", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_GARDEN02", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_RACE", key = 0x0CB80000u, data_type = 0 },
		new BinaryModelType { name_or_type = "AL_MAIN", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS0", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS2", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS4", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS6", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_CHAOS7", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_E101", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_E101R", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_EGM1", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_EGM2", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_EGM3", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_ROBO", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "SBOARD", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "SHOOTING", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "TIKAL_PROG", key = 0x0CB00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "MINICART", key = 0x0C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "A_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "B_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "E_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "S_MOT", key = 0x0CC00000u, data_type = 0 },
		new BinaryModelType { name_or_type = "S_SBMOT", key = 0x0CB08000u, data_type = 0 },
		new BinaryModelType { name_or_type = "ADVERTISE", key = 0x8C900000u, data_type = 0 },
		new BinaryModelType { name_or_type = "MOVIE", key = 0x8CEB0000u, data_type = 0 },
		};

		private int CheckKnownFile(string filename)
		{
			string name = Path.GetFileNameWithoutExtension(filename);
			if (name.Length > 3 && name.Substring(0, 3).Equals("EV0", StringComparison.OrdinalIgnoreCase))
			{
				return -4;
			}

			else if (name.Length > 2 && name.Substring(0, 2).Equals("E0", StringComparison.OrdinalIgnoreCase))
			{
				return -5;
			}

			for (int i = 0; i < SA2BMDLFiles.Length; i++)
			{
				if (name.Equals(SA2BMDLFiles[i], StringComparison.OrdinalIgnoreCase))
				{
					return -3;
				}
			}

			for (int i = 0; i < SA2MDLFiles.Length; i++)
			{
				if (name.Equals(SA2MDLFiles[i], StringComparison.OrdinalIgnoreCase))
				{
					return -2;
				}
			}

			for (int i = 0; i < KnownBinaryFiles.Length; i++)
			{
				if (name.Equals(KnownBinaryFiles[i].name_or_type, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}

			return -1;
		}

		private void LoadFile(string filename, bool cmdLoad = false)
		{
			string extension = Path.GetExtension(filename);

			loaded = false;
			if (newModelUnloadsTexturesToolStripMenuItem.Checked) UnloadTextures();
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			if (extension == ".sa1mdl") swapUVToolStripMenuItem.Enabled = true;
			else swapUVToolStripMenuItem.Enabled = false;
			timer1.Stop();
			modelFile = null;
			animation = null;
			animations = null;
			buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = buttonPlayAnimation.Enabled = false;
			animnum = -1;
			animframe = 0;
			if (ModelFile.CheckModelFile(filename))
			{
				try
				{
					modelFile = new ModelFile(filename);
					outfmt = modelFile.Format;
					if (modelFile.Model.Sibling != null)
					{
						model = new NJS_OBJECT { Name = "Root" };
						model.AddChild(modelFile.Model);
						rootSiblingMode = true;
					}
					else
					{
						model = modelFile.Model;
						rootSiblingMode = false;
					}
					animations = new List<NJS_MOTION>(modelFile.Animations);
					if (animations.Count > 0) buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = true;
				}
				catch (Exception ex)
				{
					log.Add("Loading the model from " + filename + " failed for the following reason(s):" + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine);
					string errDesc = "SAMDL could not load the model for the reason(s) below.\n" +
						"Please check the model's type and address and try again.\n\n" +
						"If you wish to report a bug, please include the model file and this information\n" +
						"in your report.";
					ErrorDialog report = new ErrorDialog("SAMDL", errDesc, log.GetLogString());
					log.WriteLog();
					report.ShowDialog();
					return;
				}
			}
			else if (extension.Equals(".nj") || extension.Equals(".gj"))
			{
				byte[] file = File.ReadAllBytes(filename);
				int ninjaDataOffset = 0;
				bool basicModel = false;

				string magic = System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(BitConverter.ToInt32(file, 0)));

				switch (magic)
				{
					case "GJTL":
					case "NJTL":
						ByteConverter.BigEndian = SA_Tools.HelperFunctions.CheckBigEndianInt32(file, 0x8);
						int POF0Offset = BitConverter.ToInt32(file, 0x4) + 0x8;
						int POF0Size = BitConverter.ToInt32(file, POF0Offset + 0x4);
						int texListOffset = POF0Offset + POF0Size + 0x8;
						ninjaDataOffset = texListOffset + 0x8;
						//Get Texture Listings for if SA Tools gets that implemented
						int texCount = ByteConverter.ToInt32(file, 0xC);
						int texOffset = 0;
						List<string> texNames = new List<string>();

						for (int i = 0; i < texCount; i++)
						{
							int textAddress = ByteConverter.ToInt32(file, texOffset + 0x10) + 0x8;

							//Read null terminated U8String
							List<byte> u8String = new List<byte>();
							byte u8Char = (file[textAddress]);
							int j = 0;
							while (u8Char != 0)
							{
								u8String.Add(u8Char);
								j++;
								u8Char = (file[textAddress + j]);
							}
							texNames.Add(System.Text.Encoding.UTF8.GetString(u8String.ToArray()));

							texOffset += 0xC;
						}
						break;
					case "GJCM":
					case "NJCM":
						ByteConverter.BigEndian = SA_Tools.HelperFunctions.CheckBigEndianInt32(file, 0x8);
						ninjaDataOffset = 0x8;
						break;
					case "NJBM":
						ByteConverter.BigEndian = SA_Tools.HelperFunctions.CheckBigEndianInt32(file, 0x8);
						ninjaDataOffset = 0x8;
						basicModel = true;
						break;
					default:
						MessageBox.Show("Incorrect format!");
						return;
				}

				//Set modelinfo parameters
				modelinfo.CheckBox_BigEndian.Checked = ByteConverter.BigEndian;
				modelinfo.RadioButton_Object.Checked = true;
				modelinfo.NumericUpDown_ObjectAddress.Value = 0;
				if (basicModel)
				{
					modelinfo.ComboBox_Format.SelectedIndex = 0;
				} else if (extension.Equals(".nj"))
				{
					modelinfo.ComboBox_Format.SelectedIndex = 2;
				} else if (extension.Equals(".gj"))
				{
					modelinfo.ComboBox_Format.SelectedIndex = 3;
				}

				modelinfo.NumericUpDown_MotionAddress.Value = 0;
				modelinfo.CheckBox_Memory_Object.Checked = false;
				modelinfo.CheckBox_Memory_Motion.Checked = false;

				modelinfo.NumericUpDown_Key.Value = 0;
				modelinfo.NumericUpDown_Key.Value = 0;

				//Get rid of the junk so that we can treat it like what SAMDL expects

				byte[] newFile = new byte[file.Length - ninjaDataOffset];
				Array.Copy(file, ninjaDataOffset, newFile, 0, newFile.Length);

				LoadBinFile(newFile);
				animations = new List<NJS_MOTION>();
			}
			else
			{
				byte[] file = File.ReadAllBytes(filename);
				if (extension.Equals(".prs", StringComparison.OrdinalIgnoreCase))
					file = FraGag.Compression.Prs.Decompress(file);
				ByteConverter.BigEndian = false;
				uint? baseaddr = SA_Tools.HelperFunctions.SetupEXE(ref file);
				if (baseaddr.HasValue)
				{
					modelinfo.NumericUpDown_Key.Value = baseaddr.Value;
					modelinfo.NumericUpDown_Key.Enabled = false;
					modelinfo.ComboBox_FileType.Enabled = false;
					modelinfo.CheckBox_BigEndian.Checked = modelinfo.CheckBox_BigEndian.Enabled = false;
					modelinfo.ComboBox_Format.SelectedIndex = 1;
				}
				else if (extension.Equals(".rel", StringComparison.OrdinalIgnoreCase))
				{
					ByteConverter.BigEndian = true;
					SA_Tools.HelperFunctions.FixRELPointers(file);
					modelinfo.NumericUpDown_Key.Value = 0;
					modelinfo.NumericUpDown_Key.Enabled = false;
					modelinfo.ComboBox_FileType.Enabled = false;
					modelinfo.CheckBox_BigEndian.Enabled = false;
					modelinfo.CheckBox_BigEndian.Checked = true;
				}
				else
				{
					int u = CheckKnownFile(filename);
					if (u == -5)
					{
						modelinfo.NumericUpDown_Key.Value = 0xC600000u;
						modelinfo.ComboBox_Format.SelectedIndex = 2;
					}
					else if (u == -4)
					{
						modelinfo.NumericUpDown_Key.Value = 0xCB80000u;
						modelinfo.ComboBox_Format.SelectedIndex = 0;
					}
					else if (u == -3)
					{
						modelinfo.RadioButton_SA2BMDL.Checked = true;
					}
					else if (u == -2)
					{
						modelinfo.RadioButton_SA2MDL.Checked = true;
					}
					else if (u != -1)
					{
						modelinfo.NumericUpDown_Key.Value = KnownBinaryFiles[u].key;
						modelinfo.ComboBox_Format.SelectedIndex = KnownBinaryFiles[u].data_type;
					}
					else modelinfo.ComboBox_Format.SelectedIndex = 1;
					if (modelinfo.RadioButton_Binary.Checked)
					{
						modelinfo.NumericUpDown_Key.Enabled = true;
						modelinfo.ComboBox_FileType.Enabled = true;
						modelinfo.CheckBox_BigEndian.Enabled = true;
					}
				}
				modelinfo.ShowDialog(this);
				if (modelinfo.DialogResult == DialogResult.OK)
				{
					if (modelinfo.RadioButton_Binary.Checked)
					{
						log.Add("Loading model from binary file " + filename + ", key: " + uint.Parse(modelinfo.NumericUpDown_Key.Value.ToString()).ToCHex() + ", address: " + uint.Parse(modelinfo.NumericUpDown_ObjectAddress.Value.ToString()).ToCHex() + ", motion at: " + uint.Parse(modelinfo.NumericUpDown_MotionAddress.Value.ToString()).ToCHex());
						LoadBinFile(file);
					}
					else
					{
						ModelFormat fmt = outfmt = ModelFormat.Chunk;
						ByteConverter.BigEndian = modelinfo.RadioButton_SA2BMDL.Checked;
						using (SA2MDLDialog dlg = new SA2MDLDialog())
						{
							int address = 0;
							SortedDictionary<int, NJS_OBJECT> sa2models = new SortedDictionary<int, NJS_OBJECT>();
							int i = ByteConverter.ToInt32(file, address);
							while (i != -1)
							{
								sa2models.Add(i, new NJS_OBJECT(file, ByteConverter.ToInt32(file, address + 4), 0, fmt, null));
								address += 8;
								i = ByteConverter.ToInt32(file, address);
							}
							foreach (KeyValuePair<int, NJS_OBJECT> item in sa2models)
								dlg.modelChoice.Items.Add(item.Key + ": " + item.Value.Name);
							dlg.ShowDialog(this);
							i = 0;
							foreach (KeyValuePair<int, NJS_OBJECT> item in sa2models)
							{
								if (i == dlg.modelChoice.SelectedIndex)
								{
									model = item.Value;
									break;
								}
								i++;
							}
							if (dlg.checkBox1.Checked)
							{
								using (OpenFileDialog anidlg = new OpenFileDialog()
								{
									DefaultExt = "bin",
									Filter = "Motion Files|*MTN.BIN;*MTN.PRS|All Files|*.*"
								})
								{
									if (anidlg.ShowDialog(this) == DialogResult.OK)
									{
										byte[] anifile = File.ReadAllBytes(anidlg.FileName);
										if (Path.GetExtension(anidlg.FileName).Equals(".prs", StringComparison.OrdinalIgnoreCase))
											anifile = FraGag.Compression.Prs.Decompress(anifile);
										address = 0;
										SortedDictionary<int, NJS_MOTION> anis = new SortedDictionary<int, NJS_MOTION>();
										i = ByteConverter.ToInt32(file, address);
										while (i != -1)
										{
											anis.Add(i, new NJS_MOTION(file, ByteConverter.ToInt32(file, address + 4), 0, model.CountAnimated()));
											address += 8;
											i = ByteConverter.ToInt32(file, address);
										}
										animations = new List<NJS_MOTION>(anis.Values);
										if (animations.Count > 0) buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = true;
									}
								}
							}
						}
					}
				}
				else return;
			}
			if (hasWeight = model.HasWeight)
				meshes = model.ProcessWeightedModel().ToArray();
			else
			{
				model.ProcessVertexData();
				NJS_OBJECT[] models = model.GetObjects();
				meshes = new Mesh[models.Length];
				for (int i = 0; i < models.Length; i++)
					if (models[i].Attach != null)
						try { meshes[i] = models[i].Attach.CreateD3DMesh(); }
						catch { }
			}

			currentFileName = filename;

			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
			AddTreeNode(model, treeView1.Nodes);
			loaded = loadAnimationToolStripMenuItem.Enabled = saveMenuItem.Enabled = buttonSave.Enabled = buttonSaveAs.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = modelCodeToolStripMenuItem.Enabled = true;
			saveAnimationsToolStripMenuItem.Enabled = (animations != null && animations.Count > 0);
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfo != null;
			showWeightsToolStripMenuItem.Enabled = buttonShowWeights.Enabled = hasWeight;
			if (cmdLoad == false)
			{
				selectedObject = model;
				SelectedItemChanged();
			}
			AddModelToLibrary(model, false);
			unsaved = false;
			EditorOptions.backLight.Ambient.R = settingsfile.SAMDL.BackLightAmbientR;
			EditorOptions.backLight.Ambient.G = settingsfile.SAMDL.BackLightAmbientG;
			EditorOptions.backLight.Ambient.B = settingsfile.SAMDL.BackLightAmbientB;
			EditorOptions.UpdateDefaultLights(d3ddevice);
		}
		private void LoadBinFile(byte[] file)
		{
			uint objectaddress = (uint)modelinfo.NumericUpDown_ObjectAddress.Value;
			uint motionaddress = (uint)modelinfo.NumericUpDown_MotionAddress.Value;
			if (modelinfo.CheckBox_Memory_Object.Checked) objectaddress -= (uint)modelinfo.NumericUpDown_Key.Value;
			if (modelinfo.CheckBox_Memory_Motion.Checked) motionaddress -= (uint)modelinfo.NumericUpDown_Key.Value;
			ByteConverter.BigEndian = modelinfo.CheckBox_BigEndian.Checked;
			if (modelinfo.RadioButton_Object.Checked)
			{
				tempmodel = new NJS_OBJECT(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value, (ModelFormat)modelinfo.ComboBox_Format.SelectedIndex, null);
				if (tempmodel.Sibling != null)
				{
					model = new NJS_OBJECT { Name = "Root" };
					model.AddChild(tempmodel);
					rootSiblingMode = true;
				}
				else
				{
					model = tempmodel;
					rootSiblingMode = false;
				}
				if (modelinfo.CheckBox_LoadMotion.Checked)
					animations = new List<NJS_MOTION>() { NJS_MOTION.ReadDirect(file, model.CountAnimated(), (int)motionaddress, (uint)modelinfo.NumericUpDown_Key.Value, null) };
					if (animations != null && animations.Count > 0) buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = true;
			}
			else if (modelinfo.RadioButton_Action.Checked)
			{
				action = new NJS_ACTION(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value, (ModelFormat)modelinfo.ComboBox_Format.SelectedIndex, null);
				model = action.Model;
				animations = new List<NJS_MOTION>() { NJS_MOTION.ReadHeader(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value, (ModelFormat)modelinfo.ComboBox_Format.SelectedIndex, null) };
				if (animations.Count > 0) buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = true;
			}
			else
			{
				model = new NJS_OBJECT { Name = "Model" };
				switch ((ModelFormat)modelinfo.ComboBox_Format.SelectedIndex)
				{
					case ModelFormat.Basic:
						model.Attach = new BasicAttach(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value, false);
						break;
					case ModelFormat.BasicDX:
						model.Attach = new BasicAttach(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value, true);
						break;
					case ModelFormat.Chunk:
						model.Attach = new ChunkAttach(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value);
						break;
					case ModelFormat.GC:
						model.Attach = new GC.GCAttach(file, (int)objectaddress, (uint)modelinfo.NumericUpDown_Key.Value, null);
						break;
				}
			}
			switch ((ModelFormat)modelinfo.ComboBox_Format.SelectedIndex)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					outfmt = ModelFormat.Basic;
					swapUVToolStripMenuItem.Enabled = true;
					break;
				case ModelFormat.Chunk:
					outfmt = ModelFormat.Chunk;
					swapUVToolStripMenuItem.Enabled = false;
					break;
				case ModelFormat.GC:
					outfmt = ModelFormat.GC;
					swapUVToolStripMenuItem.Enabled = false;
					break;
			}
		}
	
		private void AddTreeNode(NJS_OBJECT model, TreeNodeCollection nodes)
		{
			int index = 0;
			AddTreeNode(model, ref index, nodes);
		}

		private void AddTreeNode(NJS_OBJECT model, ref int index, TreeNodeCollection nodes)
		{
			TreeNode node = nodes.Add($"{index++}: {model.Name}");
			node.Tag = model;
			nodeDict[model] = node;
			foreach (NJS_OBJECT child in model.Children)
				AddTreeNode(child, ref index, node.Nodes);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loaded && unsaved)
			{
				switch (MessageBox.Show(this, "Do you want to save?", "SAMDL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveAsToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}

			try
			{
				settingsfile.SAMDL.AlternativeCamera = alternativeCameraModeToolStripMenuItem.Checked;
				settingsfile.Save();
			}
			catch { };
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void SaveAs(bool saveAnims = false)
		{
			string filterString;

			switch(outfmt)
			{
				case ModelFormat.GC:
					filterString = "SA2B MDL Files|*.sa2bmdl"; //|Sega GCNinja .gj|*.gj|Sega GCNinja Big Endian (Gamecube) .gj|*.gj";
					break;
				case ModelFormat.Chunk:
					filterString = "SA2 MDL Files|*.sa2mdl"; //|Sega Ninja .nj|*.nj|Sega Ninja Big Endian (Gamecube) .nj|*.nj";
					break;
				case ModelFormat.BasicDX:
				case ModelFormat.Basic:
				default:
					filterString = "SA1 MDL Files|*.sa1mdl"; //|Sega Ninja .nj|*.nj|Sega Ninja Big Endian (Gamecube) .nj|*.nj";
					break;
			}
			filterString += "|All files *.*|*.*";
			using (SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = (outfmt == ModelFormat.GC ? "sa2b" : (outfmt == ModelFormat.Chunk ? "sa2" : "sa1")) + "mdl",
				Filter = filterString
			})
			{
				if (currentFileName.Length > 0) a.InitialDirectory = currentFileName;

				if (a.ShowDialog(this) == DialogResult.OK)
				{
					switch(a.FilterIndex)
					{
						case 3:
							a.FileName = a.FileName + "?BE?";
							break;
						default:
							break;
					}
					Save(a.FileName, saveAnims);
				}
			}
		}

		private void Save(string fileName, bool saveAnims = false)
		{
			bool bigEndian = false;
			string extension = Path.GetExtension(fileName);

			switch (extension)
			{
				case ".nj":
				case ".gj":
				case ".nj?BE?":
				case ".gj?BE?":
					if (extension.Contains("?BE?"))
					{
						bigEndian = true;
					}
					fileName = fileName.Replace("?BE?", "");
					ByteConverter.BigEndian = bigEndian;

					if (animations != null && saveAnims)
					{
						for (int u = 0; u < animations.Count; u++)
						{
							string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_anim" + u.ToString() + "_" + animations[u].Name + ".njm";
							byte[] rawAnim = animations[u].GetBytes(0, new Dictionary<string, uint>(), out uint address, true, false);

							File.WriteAllBytes(filePath, rawAnim);
						}
					}
					if (model != null)
					{
						byte[] rawModel;
						bool isGC = extension.Contains("gj") ? true : false;
						Dictionary<string, uint> labels = new Dictionary<string, uint>();

						rawModel = model.GetBytes(0, false, labels, out uint testValue);

						List<byte> njModel = new List<byte>();
						List<string> texList = new List<string>();

						if (TextureInfo != null)
						{
							foreach (BMPInfo tex in TextureInfo)
							{
								if (tex != null)
								{
									if (tex.Name != null)
									{
										texList.Add(tex.Name);
									}
								}
							}
						}
						if(texList.Count != 0)
						{
							njModel.AddRange(GenerateNJTexList(texList.ToArray(), isGC));
						}
						
						njModel.AddRange(rawModel);

						using (StreamWriter file = new StreamWriter(fileName + "_labels.txt"))
						{
							foreach(var key in labels.Keys)
							{
								file.WriteLine($"{key} {labels[key].ToString("X")}");
							}
						}
						File.WriteAllBytes(fileName, njModel.ToArray());
					}
					break;
				default:
					string[] animfiles;
					if (animations != null && saveAnims)
					{
						animfiles = new string[animations.Count()];
						
						for (int u = 0; u < animations.Count; u++)
						{
							string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_anim" + u.ToString() + "_" + animations[u].Name + ".saanim";
							animations[u].Save(filePath);
							animfiles[u] = filePath;
						}
					}
					else animfiles = null;
					if (modelFile != null)
					{
						modelFile.SaveToFile(fileName);
					}
					else
					{
						if (rootSiblingMode)
							ModelFile.CreateFile(fileName, model.Children[0], animfiles, null, null, null, outfmt);
						else
							ModelFile.CreateFile(fileName, model, animfiles, null, null, null, outfmt);
						modelFile = new ModelFile(fileName);
					}
					break;
			}

			currentFileName = fileName;
			UpdateStatusString();
			unsaved = false;
		}

		private void saveMenuItem_Click(object sender, EventArgs e)
		{
			if (File.Exists(currentFileName))
			{
				// ask if we want to over-write
				bool doOperation = false;

				if (!doOperation)
				{
					// ask if we're sure we want to over-write
					DialogResult dialogResult = MessageBox.Show("This will overwrite the currently open file.", "Are you sure?", MessageBoxButtons.YesNo);

					doOperation = dialogResult == DialogResult.Yes;
				}

				if (doOperation) Save(currentFileName);
			}
			else
			{
				// ask us where to save
				SaveAs();
			}
		}

		private byte[] GenerateNJTexList(string[] texList, bool isGC)
		{
			List<byte> njTexList = new List<byte>();
			List<byte> njTLHeader = new List<byte>();
			List<byte> pof0List = new List<byte>();

			if (isGC)
			{
				njTLHeader.AddRange(new byte[] {0x47, 0x4A, 0x54, 0x4C});
			} else
			{
				njTLHeader.AddRange(new byte[] { 0x4E, 0x4A, 0x54, 0x4C});
			}
			njTexList.AddRange(ByteConverter.GetBytes(0x8));
			njTexList.AddRange(ByteConverter.GetBytes(texList.Length));
			
			for(int i = 0; i < texList.Length; i++)
			{
				int offset = texList.Length * 0xC;
				
				if(i > 0)
				{
					offset += texList[i].Length;
				}
				njTexList.AddRange(ByteConverter.GetBytes(offset));
				njTexList.AddRange(ByteConverter.GetBytes(0));
				njTexList.AddRange(ByteConverter.GetBytes(0));
			}
			for(int i = 0; i < texList.Length; i++)
			{
				njTexList.AddRange(Encoding.ASCII.GetBytes(texList[i]));
			}
			njTLHeader.AddRange(BitConverter.GetBytes(njTexList.Count));

			pof0List.Add(0x40);
			pof0List.Add(0x42);
			for(int i = 1; i < texList.Length; i++)
			{
				pof0List.Add(0x43);
			}
			pof0List.Align(4);

			int pofLength = pof0List.Count + (njTexList.Count % 4);
			byte[] magic = { 0x50, 0x4F, 0x46, 0x30 };

			pof0List.InsertRange(0, BitConverter.GetBytes(pofLength));
			pof0List.InsertRange(0, magic);

			njTexList.InsertRange(0, njTLHeader.ToArray());
			njTexList.AddRange(pof0List.ToArray());

			return njTexList.ToArray();
		}

		private void NewFile(ModelFormat modelFormat)
		{
			rootSiblingMode = false;
			loaded = false;
			if (newModelUnloadsTexturesToolStripMenuItem.Checked) UnloadTextures();
			//Environment.CurrentDirectory = Path.GetDirectoryName(filename); // might not need this for now?
			timer1.Stop();
			modelFile = null;
			animation = null;
			animations = null;
			animnum = -1;
			animframe = 0;

			outfmt = modelFormat;
			animations = new List<NJS_MOTION>();

			model = new NJS_OBJECT();
			model.Morph = false;
			model.ProcessVertexData();
			meshes = new Mesh[1];
			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
			AddTreeNode(model, treeView1.Nodes);
			selectedObject = model;
			buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = buttonPlayAnimation.Enabled = false;
			loaded = loadAnimationToolStripMenuItem.Enabled = saveMenuItem.Enabled = buttonSave.Enabled = buttonSaveAs.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = modelCodeToolStripMenuItem.Enabled = true;
			saveAnimationsToolStripMenuItem.Enabled = false;
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfo != null;
			SelectedItemChanged();

			currentFileName = "";
			UpdateStatusString();
			unsaved = false;
		}

		private void NewFileOperation(ModelFormat modelFormat)
		{
			bool doOperation = !loaded;

			if (!doOperation)
			{
				// ask if we're sure we want to over-write
				DialogResult dialogResult = MessageBox.Show("Are you sure? You will lose any unsaved work.", "Are you sure?", MessageBoxButtons.YesNo);

				doOperation = dialogResult == DialogResult.Yes;
			}

			if (doOperation)
			{
				NewFile(modelFormat);
			}
		}

		private void basicModelDXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.BasicDX);
		}

		private void basicModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.Basic);
		}

		private void chunkModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.Chunk);
		}

		private void GamecubeModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.GC);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		void UpdateStatusString()
		{
			string cameramode = "Move";
			string look = "Look";
			string zoom = "Zoom";
			if (!string.IsNullOrEmpty(currentFileName))
				Text = "SAMDL: " + currentFileName;
			else
				Text = "SAMDL";
			cameraPosLabel.Text = $"Camera X: " + cam.Position.X.ToString("0.000") + " Y: " + cam.Position.Y.ToString("0.000") + " Z: " + cam.Position.Z.ToString("0.000");
			if (showAdvancedCameraInfoToolStripMenuItem.Checked)
			{
				if (lookKeyDown) cameramode = look;
				if (zoomKeyDown) cameramode = zoom;
				cameraAngleLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
				cameraModeLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
				cameraAngleLabel.Text = $"Pitch: " + cam.Pitch.ToString("X5") + " Yaw: " + cam.Yaw.ToString("X5") + (cam.mode == 1 ? " Distance: " + cam.Distance : "");
				cameraModeLabel.Text = $"Mode: " + cameramode + ", Speed: " + cam.MoveSpeed;
			}
			else 
			{
				cameraAngleLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
				cameraModeLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
				cameraAngleLabel.Text = "";
				cameraModeLabel.Text = "";
			}
			animNameLabel.Text = $"Animation: {animation?.Name ?? "None"}";
			animFrameLabel.Text = $"Frame: {animframe}";
			statusStrip1.Update();
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded || DeviceResizing) return;
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), RenderPanel.Width / (float)RenderPanel.Height, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformState.View, cam.ToMatrix());
			UpdateStatusString();
			d3ddevice.SetRenderState(RenderState.FillMode, EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderState.CullMode, EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = Color.White.ToRawColor4() };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			d3ddevice.BeginScene();

			//all drawings after this line
			EditorOptions.RenderStateCommonSetup(d3ddevice);

			MatrixStack transform = new MatrixStack();
			if (showModelToolStripMenuItem.Checked)
			{
				if (hasWeight)
					RenderInfo.Draw(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting), d3ddevice, cam);
				else if (animation != null)
				{
					foreach (KeyValuePair<int, AnimModelData> animdata in animation.Models)
					{
						if (animdata.Value.Vertex.Count > 0 || animdata.Value.Normal.Count > 0)
						{
							model.ProcessShapeMotionVertexData(animation, animframe);
							NJS_OBJECT[] models = model.GetObjects();
							meshes = new Mesh[models.Length];
							for (int i = 0; i < models.Length; i++)
								if (models[i].Attach != null)
									try { meshes[i] = models[i].Attach.CreateD3DMesh(); }
									catch { }
						}
					}
					RenderInfo.Draw(model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, meshes, animation, animframe, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting), d3ddevice, cam);
				}
				else
					RenderInfo.Draw(model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting), d3ddevice, cam);

				if (selectedObject != null)
				{
					if (hasWeight)
					{
						NJS_OBJECT[] objs = model.GetObjects();
						if (selectedObject.Attach != null)
							for (int j = 0; j < selectedObject.Attach.MeshInfo.Length; j++)
							{
								Color col = selectedObject.Attach.MeshInfo[j].Material == null ? Color.White : selectedObject.Attach.MeshInfo[j].Material.DiffuseColor;
								col = Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
								NJS_MATERIAL mat = new NJS_MATERIAL
								{
									DiffuseColor = col,
									IgnoreLighting = true,
									UseAlpha = false
								};
								new RenderInfo(meshes[Array.IndexOf(objs, selectedObject)], j, transform.Top, mat, null, FillMode.Wireframe, selectedObject.Attach.CalculateBounds(j, transform.Top)).Draw(d3ddevice);
							}
					}
					else
						DrawSelectedObject(model, transform);
				}
			}

			d3ddevice.SetRenderState(RenderState.AlphaBlendEnable, false);
			d3ddevice.SetRenderState(RenderState.FillMode, FillMode.Solid);
			d3ddevice.SetRenderState(RenderState.Lighting, false);
			d3ddevice.SetRenderState(RenderState.ZEnable, false);
			d3ddevice.SetTexture(0, null);
			if (showNodesToolStripMenuItem.Checked)
				DrawNodes(model, transform);

			if (showNodeConnectionsToolStripMenuItem.Checked)
				DrawNodeConnections(model, transform);
			osd.ProcessMessages();
			d3ddevice.EndScene(); //all drawings before this line
			d3ddevice.Present();
		}

		private void DrawSelectedObject(NJS_OBJECT obj, MatrixStack transform)
		{
			int modelnum = -1;
			int animindex = -1;
			DrawSelectedObject(obj, transform, ref modelnum, ref animindex);
		}

		private bool DrawSelectedObject(NJS_OBJECT obj, MatrixStack transform, ref int modelindex, ref int animindex)
		{
			transform.Push();
			modelindex++;
			if (obj.Animate) animindex++;
			if (animation != null && animation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(animation.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			if (obj == selectedObject)
			{
				if (obj.Attach != null)
				{
					if (buttonShowVertexIndices.Checked) 
						DrawVertexIndices(obj, transform);
					for (int j = 0; j < obj.Attach.MeshInfo.Length; j++)
					{
						Color col = obj.Attach.MeshInfo[j].Material == null ? Color.White : obj.Attach.MeshInfo[j].Material.DiffuseColor;
						col = Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
						NJS_MATERIAL mat = new NJS_MATERIAL
						{
							DiffuseColor = col,
							IgnoreLighting = true,
							UseAlpha = false
						};
						new RenderInfo(meshes[modelindex], j, transform.Top, mat, null, FillMode.Wireframe, obj.Attach.CalculateBounds(j, transform.Top)).Draw(d3ddevice);
					}
				}
				transform.Pop();
				return true;
			}
			foreach (NJS_OBJECT child in obj.Children)
				if (DrawSelectedObject(child, transform, ref modelindex, ref animindex))
				{
					transform.Pop();
					return true;
				}
			transform.Pop();
			return false;
		}

		private void DrawNodes(NJS_OBJECT obj, MatrixStack transform)
		{
			int modelnum = -1;
			int animindex = -1;
			DrawNodes(obj, transform, ref modelnum, ref animindex);
		}

		private void DrawNodes(NJS_OBJECT obj, MatrixStack transform, ref int modelindex, ref int animindex)
		{
			transform.Push();
			modelindex++;
			if (obj.Animate) animindex++;
			if (animation != null && animation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(animation.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			d3ddevice.SetTransform(TransformState.World, Matrix.Translation(Vector3.TransformCoordinate(new Vector3(), transform.Top)));
			if (obj == selectedObject)
			{
				if (obj.Attach != null)
					selectedModelSphereMesh.DrawAll(d3ddevice);
				else
					selectedSphereMesh.DrawAll(d3ddevice);
			}
			else if (obj.Attach != null)
				modelSphereMesh.DrawAll(d3ddevice);
			else
				sphereMesh.DrawAll(d3ddevice);
			foreach (NJS_OBJECT child in obj.Children)
				DrawNodes(child, transform, ref modelindex, ref animindex);
			transform.Pop();
		}

		private void DrawNodeConnections(NJS_OBJECT obj, MatrixStack transform)
		{
			int modelnum = -1;
			int animindex = -1;
			List<Vector3> points = new List<Vector3>();
			List<short> indexes = new List<short>();
			DrawNodeConnections(obj, transform, points, indexes, -1, ref modelnum, ref animindex);
			d3ddevice.SetTransform(TransformState.World, Matrix.Identity);
			d3ddevice.VertexFormat = VertexFormat.Position;
			d3ddevice.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, points.Count, indexes.Count / 2, indexes.ToArray(), Format.Index16, points.ToArray());
		}

		private void DrawVertexIndices(NJS_OBJECT obj, MatrixStack transform)
		{
			if (obj.Attach == null || (!(obj.Attach is BasicAttach))) return;
			{
				BasicAttach basicatt = (BasicAttach)obj.Attach;
				Matrix view = d3ddevice.GetTransform(TransformState.View);
				Viewport viewport = d3ddevice.Viewport;
				Matrix projection = d3ddevice.GetTransform(TransformState.Projection);
				osd.textSprite.Begin(SpriteFlags.AlphaBlend);
				for (int i = 0; i < basicatt.Vertex.Length; i++)
				{
					Vertex vtx = basicatt.Vertex[i];
					Vector3 v3 = Vector3.TransformCoordinate(vtx.ToVector3(), transform.Top);
					Vector3 screenCoordinates = viewport.Project(v3, projection, view, Matrix.Identity);
					EditorOptions.OnscreenFont.DrawText(osd.textSprite, i.ToString(), (int)screenCoordinates.X, (int)screenCoordinates.Y, Color.White.ToRawColorBGRA());
				}
				osd.textSprite.End();
			}
		}

		private void DrawNodeConnections(NJS_OBJECT obj, MatrixStack transform, List<Vector3> points, List<short> indexes, short parentidx, ref int modelindex, ref int animindex)
		{
			transform.Push();
			modelindex++;
			if (obj.Animate) animindex++;
			if (animation != null && animation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(animation.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			short newidx = (short)points.Count;
			points.Add(Vector3.TransformCoordinate(new Vector3(), transform.Top));
			if (parentidx != -1)
			{
				indexes.Add(parentidx);
				indexes.Add(newidx);
			}
			foreach (NJS_OBJECT child in obj.Children)
				DrawNodeConnections(child, transform, points, indexes, newidx, ref modelindex, ref animindex);
			transform.Pop();
		}

		private void UpdateWeightedModel()
		{
			if (hasWeight)
			{
				if (animation != null)
					model.UpdateWeightedModelAnimated(new MatrixStack(), animation, animframe, meshes);
				else
					model.UpdateWeightedModel(new MatrixStack(), meshes);
			}
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			DrawEntireModel();
		}
		#endregion

		#region Keyboard/Mouse Methods
		void optionsEditor_FormUpdatedKeys()
		{
			// Keybinds
			actionList.ActionKeyMappings.Clear();
			ActionKeyMapping[] newMappings = optionsEditor.GetActionkeyMappings();
			foreach (ActionKeyMapping mapping in newMappings)
				actionList.ActionKeyMappings.Add(mapping);
			actionInputCollector.SetActions(newMappings);
			string saveControlsPath = Path.Combine(Application.StartupPath, "keybinds", "SAMDL.ini");
			actionList.Save(saveControlsPath);
			// Settings
			optionsEditor_FormUpdated();
		}

		private void PreviousAnimation()
		{
			if (animations == null) return;
			animnum--;
			if (animnum == -2) animnum = animations.Count - 1;
			if (animnum > -1)
				animation = animations[animnum];
			else
				animation = null;
			if (animation != null)
			{
				osd.UpdateOSDItem("Animation: " + animations[animnum].Name.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = true;
			}
			else
			{
				osd.UpdateOSDItem("No animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = false;
			}
			animframe = 0;
			DrawEntireModel();
		}

		private void NextAnimation()
		{
			if (animations == null) return;
			animnum++;
			if (animnum == animations.Count) animnum = -1;
			if (animnum > -1)
				animation = animations[animnum];
			else
				animation = null;
			if (animation != null)
			{
				osd.UpdateOSDItem("Animation: " + animations[animnum].Name.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = true;
			}
			else
			{
				osd.UpdateOSDItem("No animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = false;
			}
			animframe = 0;
			DrawEntireModel();
		}

		private void NextFrame()
		{
			if (animations == null || animation == null) return;
			animframe++;
			if (animframe == animation.Frames) animframe = 0;
			osd.UpdateOSDItem("Animation frame: " + animframe.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			UpdateWeightedModel();
			DrawEntireModel();
		}

		private void PrevFrame()
		{
			if (animations == null || animation == null) return;
			animframe--;
			if (animframe < 0) animframe = animation.Frames - 1;
			osd.UpdateOSDItem("Animation frame: " + animframe.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			UpdateWeightedModel();
			DrawEntireModel();
		}

		private void PlayPause()
		{
			if (animations == null || animation == null) return;
			timer1.Enabled = !timer1.Enabled;
			if (timer1.Enabled)
			{
				osd.UpdateOSDItem("Play animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Checked = true;
			}
			else
			{
				osd.UpdateOSDItem("Stop animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Checked = false;
			}
			UpdateWeightedModel();
			DrawEntireModel();
		}

		private void ActionInputCollector_OnActionRelease(ActionInputCollector sender, string actionName)
		{
			if (!loaded)
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
						if (selectedObject != null)
						{
							cam.FocalPoint = selectedObject.Position.ToVector3();
						}
						else
						{
							cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
						}
					}
					osd.UpdateOSDItem("Camera mode: " + cammode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Zoom to target"):
					if (selectedObject != null)
					{
						BoundingSphere bounds = (selectedObject.Attach != null) ? selectedObject.Attach.Bounds :
							new BoundingSphere(selectedObject.Position.X, selectedObject.Position.Y, selectedObject.Position.Z, 10);

						bounds.Center += selectedObject.Position;
						cam.MoveToShowBounds(bounds);
					}
					else
					{
						BoundingSphere bounds = (model.Attach != null) ? model.Attach.Bounds :
							new BoundingSphere(model.Position.X, model.Position.Y, model.Position.Z, 10);

						bounds.Center += model.Position;
						cam.MoveToShowBounds(bounds);
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
					DeleteSelectedModel();
					draw = true;
					break;

				case ("Increase camera move speed"):
					cam.MoveSpeed += 0.0625f;
					UpdateStatusString();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					settingsfile.SAMDL.CamMoveSpeed = cam.MoveSpeed;
					draw = true;
					break;

				case ("Decrease camera move speed"):
					cam.MoveSpeed = Math.Max(cam.MoveSpeed - 0.0625f, 0.0625f);
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					UpdateStatusString();
					settingsfile.SAMDL.CamMoveSpeed = cam.MoveSpeed;
					draw = true;
					break;

				case ("Reset camera move speed"):
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					osd.UpdateOSDItem("Reset camera speed", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					UpdateStatusString();
					settingsfile.SAMDL.CamMoveSpeed = cam.MoveSpeed;
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
						osd.UpdateOSDItem("Reset camera rotation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
						draw = true;
					}
					break;

				case ("Camera Move"):
					cameraKeyDown = false;
					break;

				case ("Camera Zoom"):
					zoomKeyDown = false;
					break;

				case ("Camera Look"):
					lookKeyDown = false;
					break;

				case ("Next Animation"):
					NextAnimation();
					break;

				case ("Previous Animation"):
					PreviousAnimation();
					break;

				case ("Previous Frame"):
					PrevFrame();
					break;

				case ("Next Frame"):
					NextFrame();
					break;

				case ("Play/Pause Animation"):
					PlayPause();
					break;
				case ("Brighten Ambient"):
					EditorOptions.backLight.Ambient.R = Math.Min(1.0f, EditorOptions.backLight.Ambient.R + 0.1f);
					EditorOptions.backLight.Ambient.G = Math.Min(1.0f, EditorOptions.backLight.Ambient.G + 0.1f);
					EditorOptions.backLight.Ambient.B = Math.Min(1.0f, EditorOptions.backLight.Ambient.B + 0.1f);
					EditorOptions.UpdateDefaultLights(d3ddevice);
					settingsfile.SAMDL.BackLightAmbientR = EditorOptions.backLight.Ambient.R;
					settingsfile.SAMDL.BackLightAmbientG = EditorOptions.backLight.Ambient.G;
					settingsfile.SAMDL.BackLightAmbientB = EditorOptions.backLight.Ambient.B;
					osd.UpdateOSDItem("Brighten Ambient", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					draw = true;
					break;
				case ("Darken Ambient"):
					EditorOptions.backLight.Ambient.R = Math.Max(0.0f, EditorOptions.backLight.Ambient.R - 0.1f);
					EditorOptions.backLight.Ambient.G = Math.Max(0.0f, EditorOptions.backLight.Ambient.G - 0.1f);
					EditorOptions.backLight.Ambient.B = Math.Max(0.0f, EditorOptions.backLight.Ambient.B - 0.1f);
					EditorOptions.UpdateDefaultLights(d3ddevice);
					settingsfile.SAMDL.BackLightAmbientR = EditorOptions.backLight.Ambient.R;
					settingsfile.SAMDL.BackLightAmbientG = EditorOptions.backLight.Ambient.G;
					settingsfile.SAMDL.BackLightAmbientB = EditorOptions.backLight.Ambient.B;
					osd.UpdateOSDItem("Darken Ambient", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					draw = true;
					break;
				default:
					break;
			}

			if (draw)
			{
				UpdateWeightedModel();
				DrawEntireModel();
			}
		}

		private void ActionInputCollector_OnActionStart(ActionInputCollector sender, string actionName)
		{
			switch (actionName)
			{
				case ("Camera Move"):
					cameraKeyDown = true;
					osd.UpdateOSDItem("Camera mode: Move", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					break;

				case ("Camera Zoom"):
					osd.UpdateOSDItem("Camera mode: Zoom", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					zoomKeyDown = true;
					break;

				case ("Camera Look"):
					osd.UpdateOSDItem("Camera mode: Look", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					lookKeyDown = true;
					break;

				default:
					break;
			}
		}

		private void panel1_KeyDown(object sender, KeyEventArgs e)
		{
			if (!loaded) return;
			actionInputCollector.KeyDown(e.KeyCode);
		}

		private void panel1_KeyUp(object sender, KeyEventArgs e)
		{
			actionInputCollector.KeyUp(e.KeyCode);
		}

		private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			/*switch (e.KeyCode)
			{
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
					e.IsInputKey = true;
					break;
			}*/
		}

		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
			int camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, cameraKeyDown, alternativeCameraModeToolStripMenuItem.Checked); 
			if (camresult >= 2 && selectedObject != null && propertyGrid1.ActiveControl == null) propertyGrid1.Refresh();
			if (camresult >= 1 && !timer1.Enabled)
			{
				UpdateWeightedModel();
				DrawEntireModel();
			}
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyUp(Keys.MButton);
		}
		#endregion

		private void LoadTextures(string filename)
		{
			TextureInfo = TextureArchive.GetTextures(filename);

			TexturePackName = Path.GetFileNameWithoutExtension(filename);
			Textures = new Texture[TextureInfo.Length];
			for (int j = 0; j < TextureInfo.Length; j++)
				Textures[j] = TextureInfo[j].Image.ToTexture(d3ddevice);

			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = loaded;
			if (loaded) DrawEntireModel();
		}

		private void loadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog() {  Title = "Load Textures", DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs" })
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					LoadTextures(a.FileName);
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (animation == null) return;
			animframe++;
			if (animframe == animation.Frames) animframe = 0;
			UpdateWeightedModel();
			DrawEntireModel();
		}

		private void cStructsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "c", Filter = "C Files|*.c" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					bool dx = false;
					if (outfmt == ModelFormat.Basic)
						dx = MessageBox.Show(this, "Do you want to export in SADX format?", "SAMDL", MessageBoxButtons.YesNo) == DialogResult.Yes;
					List<string> labels = new List<string>() { model.Name };
					using (StreamWriter sw = File.CreateText(sd.FileName))
					{
						sw.Write("/* NINJA ");
						switch (outfmt)
						{
							case ModelFormat.Basic:
							case ModelFormat.BasicDX:
								if (dx)
									sw.Write("Basic (with Sonic Adventure DX additions)");
								else
									sw.Write("Basic");
								break;
							case ModelFormat.Chunk:
								sw.Write("Chunk");
								break;
						}
						sw.WriteLine(" model");
						sw.WriteLine(" * ");
						sw.WriteLine(" * Generated by SAMDL");
						sw.WriteLine(" * ");
						if (modelFile != null)
						{
							if (!string.IsNullOrEmpty(modelFile.Description))
							{
								sw.Write(" * Description: ");
								sw.WriteLine(modelFile.Description);
								sw.WriteLine(" * ");
							}
							if (!string.IsNullOrEmpty(modelFile.Author))
							{
								sw.Write(" * Author: ");
								sw.WriteLine(modelFile.Author);
								sw.WriteLine(" * ");
							}
						}
						sw.WriteLine(" */");
						sw.WriteLine();
						string[] texnames = null;
						if (TexturePackName != null && exportTextureNamesToolStripMenuItem.Checked)
						{
							texnames = new string[TextureInfo.Length];
							for (int i = 0; i < TextureInfo.Length; i++)
								texnames[i] = string.Format("{0}TexName_{1}", TexturePackName, TextureInfo[i].Name);
							sw.Write("enum {0}TexName", TexturePackName);
							sw.WriteLine();
							sw.WriteLine("{");
							sw.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texnames));
							sw.WriteLine("};");
							sw.WriteLine();
						}
						if (rootSiblingMode) 
							model.Children[0].ToStructVariables(sw, dx, labels, texnames);
						else
							model.ToStructVariables(sw, dx, labels, texnames);
						if (exportAnimationsToolStripMenuItem.Checked && animations != null)
						{
							foreach (NJS_MOTION anim in animations)
							{
								anim.ToStructVariables(sw);
							}
						}
					}
				}
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;

			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyDown(Keys.MButton);

			if (e.Button == MouseButtons.Left)
			{
				HitResult dist;
				Vector3 mousepos = new Vector3(e.X, e.Y, 0);
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
				if (hasWeight)
					dist = model.CheckHitWeighted(Near, Far, viewport, proj, view, Matrix.Identity, meshes);
				else
					dist = model.CheckHit(Near, Far, viewport, proj, view, new MatrixStack(), meshes);
				if (dist.IsHit)
				{
					selectedObject = dist.Model;
					SelectedItemChanged();
				}
				else
				{
					selectedObject = null;
					SelectedItemChanged();
				}
			}

			if (e.Button == MouseButtons.Right)
			{
				actionInputCollector.ReleaseKeys();
				mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
				cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), new System.Drawing.Rectangle(), false, false, false, alternativeCameraModeToolStripMenuItem.Checked);
				contextMenuStrip1.Show(RenderPanel, e.Location);
			}
		}

		internal Type GetAttachType()
		{
			switch (outfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					return typeof(BasicAttach);
				case ModelFormat.Chunk:
					return typeof(ChunkAttach);
				case ModelFormat.GC:
					return typeof(GC.GCAttach);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		bool suppressTreeEvent;
		internal void SelectedItemChanged()
		{
			suppressTreeEvent = true;
			if (selectedObject != null)
			{
				treeView1.SelectedNode = nodeDict[selectedObject];
				suppressTreeEvent = false;
				propertyGrid1.SelectedObject = selectedObject;
				copyModelToolStripMenuItem.Enabled = selectedObject.Attach != null;
				pasteModelToolStripMenuItem.Enabled = Clipboard.ContainsData(GetAttachType().AssemblyQualifiedName);
				editMaterialsToolStripMenuItem.Enabled = materialEditorToolStripMenuItem.Enabled = selectedObject.Attach?.MeshInfo != null && selectedObject.Attach.MeshInfo.Length > 0;
				addChildToolStripMenuItem.Enabled = true;
				clearChildrenToolStripMenuItem1.Enabled = selectedObject.Children.Count > 0;
				deleteToolStripMenuItem.Enabled = true;
				deleteNodeToolStripMenuItem.Enabled = selectedObject.Parent != null;
				emptyModelDataToolStripMenuItem.Enabled = selectedObject.Attach != null;
				splitToolStripMenuItem.Enabled = selectedObject.Attach != null;
				sortToolStripMenuItem.Enabled = true;
				PolyNormalstoolStripMenuItem.Enabled = outfmt == ModelFormat.Basic;
				exportContextMenuItem.Enabled = selectedObject.Attach != null;
				importContextMenuItem.Enabled = true;
				hierarchyToolStripMenuItem.Enabled = selectedObject.Children != null && selectedObject.Children.Count > 0;
			}
			else
			{
				treeView1.SelectedNode = null;
				suppressTreeEvent = false;
				propertyGrid1.SelectedObject = null;
				copyModelToolStripMenuItem.Enabled = false;
				pasteModelToolStripMenuItem.Enabled = Clipboard.ContainsData(GetAttachType().AssemblyQualifiedName);
				addChildToolStripMenuItem.Enabled = false;
				PolyNormalstoolStripMenuItem.Enabled = editMaterialsToolStripMenuItem.Enabled = materialEditorToolStripMenuItem.Enabled = false;
				splitToolStripMenuItem.Enabled = sortToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = clearChildrenToolStripMenuItem1.Enabled = deleteNodeToolStripMenuItem.Enabled = emptyModelDataToolStripMenuItem.Enabled = false;
				importContextMenuItem.Enabled = false;
				exportContextMenuItem.Enabled = false;
			}
			if (showWeightsToolStripMenuItem.Checked && hasWeight)
				model.UpdateWeightedModelSelection(selectedObject, meshes);

			DrawEntireModel();
		}

		private void copyModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(GetAttachType().AssemblyQualifiedName, selectedObject.Attach);
			pasteModelToolStripMenuItem.Enabled = true;
		}

		private void pasteModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Attach attach = (Attach)Clipboard.GetData(GetAttachType().AssemblyQualifiedName);
			if (selectedObject.Attach != null)
				attach.Name = selectedObject.Attach.Name;
			switch (attach)
			{
				case BasicAttach batt:
					batt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
					batt.NormalName = "normal_" + Extensions.GenerateIdentifier();
					batt.MaterialName = "material_" + Extensions.GenerateIdentifier();
					batt.MeshName = "mesh_" + Extensions.GenerateIdentifier();
					foreach (NJS_MESHSET m in batt.Mesh)
					{
						m.PolyName = "poly_" + Extensions.GenerateIdentifier();
						m.PolyNormalName = "polynormal_" + Extensions.GenerateIdentifier();
						m.UVName = "uv_" + Extensions.GenerateIdentifier();
						m.VColorName = "vcolor_" + Extensions.GenerateIdentifier();
					}
					break;
				case ChunkAttach catt:
					catt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
					catt.PolyName = "poly_" + Extensions.GenerateIdentifier();
					break;
			}
			selectedObject.Attach = attach;
			attach.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			try { meshes[Array.IndexOf(models, selectedObject)] = attach.CreateD3DMesh(); }
			catch { }
			DrawEntireModel();
			unsaved = true;
		}

		private void editMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenMaterialEditor();
		}

		private void OpenMaterialEditor()
		{
			List<NJS_MATERIAL> mats;
			switch (selectedObject.Attach)
			{
				case BasicAttach bscatt:
					mats = bscatt.Material;
					break;
				default:
					mats = selectedObject.Attach.MeshInfo.Select(a => a.Material).ToList();
					break;
			}
			using (MaterialEditor dlg = new MaterialEditor(mats, TextureInfo))
			{
				dlg.FormUpdated += (s, ev) => DrawEntireModel();
				dlg.ShowDialog(this);
			}
			switch (selectedObject.Attach)
			{
				case ChunkAttach cnkatt:
					model.StripPolyCache();
					List<PolyChunk> chunks = new List<PolyChunk>();
					int matind = 0;
					foreach (var cnk in cnkatt.Poly)
						switch (cnk)
						{
							case PolyChunkVolume pcv:
								chunks.Add(pcv);
								break;
							case PolyChunkStrip pcs:
								chunks.Add(new PolyChunkMaterial(mats[matind]));
								chunks.Add(new PolyChunkTinyTextureID(mats[matind]));
								pcs.UpdateFlags(mats[matind++]);
								chunks.Add(pcs);
								break;
						}
					cnkatt.Poly = chunks;
					break;
				case GC.GCAttach gcatt:
					matind = 0;
					foreach (var msh in gcatt.opaqueMeshes.Concat(gcatt.translucentMeshes))
					{
						msh.parameters.RemoveAll(a => a is GC.TextureParameter);
						if (mats[matind].UseTexture)
						{
							GC.GCTileMode tm = GC.GCTileMode.Mask;
							if (mats[matind].ClampU)
								tm &= ~GC.GCTileMode.WrapU;
							if (mats[matind].FlipU)
								tm &= ~GC.GCTileMode.MirrorU;
							if (mats[matind].ClampV)
								tm &= ~GC.GCTileMode.WrapV;
							if (mats[matind].FlipV)
								tm &= ~GC.GCTileMode.MirrorV;
							msh.parameters.Add(new GC.TextureParameter((ushort)mats[matind].TextureID, tm));
						}
						msh.parameters.RemoveAll(a => a is GC.BlendAlphaParameter);
						msh.parameters.Add(new GC.BlendAlphaParameter() { NJSourceAlpha = mats[matind].SourceAlpha, NJDestAlpha = mats[matind].DestinationAlpha });
						matind++;
					}
					break;
			}
			unsaved = true;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			UpdateWeightedModel();
			unsaved = true;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (suppressTreeEvent) return;
			selectedObject = (NJS_OBJECT)e.Node.Tag;
			SelectedItemChanged();
		}

		private void primitiveRenderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawEntireModel();
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
		}

		void optionsEditor_FormUpdated()
		{
			settingsfile.SAMDL.DrawDistance = EditorOptions.RenderDrawDistance;
			settingsfile.SAMDL.CameraModifier = cam.ModifierKey;
			DrawEntireModel();
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FindDialog dlg = new FindDialog())
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					NJS_OBJECT obj = model.GetObjects().SingleOrDefault(o => o.Name == dlg.SearchText || (o.Attach != null && o.Attach.Name == dlg.SearchText));
					if (obj != null)
					{
						selectedObject = obj;
						SelectedItemChanged();
					}
					else
						MessageBox.Show(this, "Not found.", "SAMDL");
				}
		}

		private void textureRemappingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (TextureRemappingDialog dlg = new TextureRemappingDialog(TextureInfo))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					unsaved = true;
					foreach (Attach att in model.GetObjects().Where(a => a.Attach != null).Select(a => a.Attach))
						switch (att)
						{
							case BasicAttach batt:
								if (batt.Material != null)
									foreach (NJS_MATERIAL mat in batt.Material)
										if (dlg.TextureMap.ContainsKey(mat.TextureID))
											mat.TextureID = dlg.TextureMap[mat.TextureID];
								break;
							case ChunkAttach catt:
								if (catt.Poly != null)
									foreach (PolyChunkTinyTextureID tex in catt.Poly.OfType<PolyChunkTinyTextureID>())
										if (dlg.TextureMap.ContainsKey(tex.TextureID))
											tex.TextureID = (ushort)dlg.TextureMap[tex.TextureID];
								break;
							case GC.GCAttach gatt:
								foreach (var msh in gatt.opaqueMeshes.Concat(gatt.translucentMeshes))
								{
									var tp = (GC.TextureParameter)msh.parameters.LastOrDefault(a => a is GC.TextureParameter);
									if (tp != null && dlg.TextureMap.ContainsKey(tp.TextureID))
									{
										msh.parameters.RemoveAll(a => a is GC.TextureParameter);
										msh.parameters.Add(new GC.TextureParameter((ushort)dlg.TextureMap[tp.TextureID], tp.Tile));
									}
								}
								break;
						}
					if (hasWeight)
					{
						meshes = model.ProcessWeightedModel().ToArray();
						UpdateWeightedModel();
					}
					else
						model.ProcessVertexData();

					DrawEntireModel();
				}
		}

		private void showModelToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			string showmodel = "Off";
			if (showModelToolStripMenuItem.Checked) showmodel = "On";
			osd.UpdateOSDItem("Show model: " + showmodel, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			DrawEntireModel();
		}

		private void showNodesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			string shownodes = "Off";
			if (showNodesToolStripMenuItem.Checked) shownodes = "On";
			osd.UpdateOSDItem("Show nodes: " + shownodes, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			buttonShowNodes.Checked = showNodesToolStripMenuItem.Checked;
			DrawEntireModel();
		}

		private void nJAToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "nja", Filter = "Ninja Ascii Files|*.nja" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					bool dx = false;
					if (outfmt == ModelFormat.Basic)
						dx = MessageBox.Show(this, "Do you want to export in SADX format?", "SAMDL", MessageBoxButtons.YesNo) == DialogResult.Yes;
					List<string> labels = new List<string>() { model.Name };
					using (StreamWriter sw = File.CreateText(sd.FileName))
					{
						sw.Write("/* NINJA ");
						switch (outfmt)
						{
							case ModelFormat.Basic:
							case ModelFormat.BasicDX:
								if (dx)
									sw.Write("Basic (with Sonic Adventure DX additions)");
								else
									sw.Write("Basic");
								break;
							case ModelFormat.Chunk:
								sw.Write("Chunk");
								break;
						}
						sw.WriteLine(" model");
						sw.WriteLine(" * ");
						sw.WriteLine(" * Generated by SAMDL");
						sw.WriteLine(" * ");
						if (modelFile != null)
						{
							if (!string.IsNullOrEmpty(modelFile.Description))
							{
								sw.Write(" * Description: ");
								sw.WriteLine(modelFile.Description);
								sw.WriteLine(" * ");
							}
							if (!string.IsNullOrEmpty(modelFile.Author))
							{
								sw.Write(" * Author: ");
								sw.WriteLine(modelFile.Author);
								sw.WriteLine(" * ");
							}
						}
						sw.WriteLine(" */");
						sw.WriteLine();
						string[] texnames = null;
						if (TexturePackName != null)
						{
							texnames = new string[TextureInfo.Length];
							for (int i = 0; i < TextureInfo.Length; i++)
								texnames[i] = string.Format("{0}TexName_{1}", TexturePackName, TextureInfo[i].Name);
							sw.Write("enum {0}TexName", TexturePackName);
							sw.WriteLine();
							sw.WriteLine("{");
							sw.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texnames));
							sw.WriteLine("};");
							sw.WriteLine();
						}
						if (rootSiblingMode)
							model.Children[0].ToNJA(sw, dx, labels, texnames);
						else
							model.ToNJA(sw, dx, labels, texnames);
					}
				}
		}

		private void ImportModel_Assimp(string objFileName, bool importAsSingle, bool selected = false)
		{
			if (TextureInfo == null)
			{
				using (OpenFileDialog a = new OpenFileDialog() { Title = "Load Textures", DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs" })
				{
					if (a.ShowDialog() == DialogResult.OK)
					{
						LoadTextures(a.FileName);
						DrawEntireModel();
					}
					else
						MessageBox.Show("No textures are loaded. Materials may not be imported properly.", "SAMDL Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			Assimp.AssimpContext context = new Assimp.AssimpContext();
			context.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));
			Assimp.Scene scene = context.ImportFile(objFileName, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.JoinIdenticalVertices | Assimp.PostProcessSteps.FlipUVs);
			Assimp.Node importnode = scene.RootNode;
			loaded = false;
			if (newModelUnloadsTexturesToolStripMenuItem.Checked) UnloadTextures();
			timer1.Stop();
			// Collada adds a root node, so use the first child node instead
			if (Path.GetExtension(objFileName).ToLowerInvariant() == ".dae")
				importnode = scene.RootNode.Children[0];
			NJS_OBJECT newmodel = SAEditorCommon.Import.AssimpStuff.AssimpImport(scene, importnode, outfmt, TextureInfo?.Select(t => t.Name).ToArray(), importAsSingle);
			if (!selected)
			{
				modelFile = null;
				animation = null;
				animations = null;
				animnum = -1;
				animframe = 0;
				animations = new List<NJS_MOTION>();
				model = newmodel;
			}
			else
			{
				selectedObject.AddChild(newmodel);
			}

			editMaterialsToolStripMenuItem.Enabled = materialEditorToolStripMenuItem.Enabled = true;
			showWeightsToolStripMenuItem.Enabled = buttonShowWeights.Enabled = hasWeight;

			if (model.HasWeight)
				meshes = model.ProcessWeightedModel().ToArray();
			else
			{
				model.ProcessVertexData();
				NJS_OBJECT[] models = model.GetObjects();
				meshes = new Mesh[models.Length];
				for (int i = 0; i < models.Length; i++)
					if (models[i].Attach != null)
						try { meshes[i] = models[i].Attach.CreateD3DMesh(); }
						catch { }
			}

			if (!selected)
			{
				if (animations.Count > 0) buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = true;
				selectedObject = model;
				AddModelToLibrary(model, false);
			}
			RebuildModelCache();
			unsaved = true;
			loaded = loadAnimationToolStripMenuItem.Enabled = saveMenuItem.Enabled = buttonSave.Enabled = buttonSaveAs.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = modelCodeToolStripMenuItem.Enabled = true;
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfo != null;
			saveAnimationsToolStripMenuItem.Enabled = animations.Count > 0;
			SelectedItemChanged();
			DrawEntireModel();
		}

		private void ShowWeightsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (showWeightsToolStripMenuItem.Checked)
			{
				EditorOptions.OverrideLighting = true;
				model.UpdateWeightedModelSelection(selectedObject, meshes);
			}
			else
				model.UpdateWeightedModelSelection(null, meshes);
			buttonShowWeights.Checked = showWeightsToolStripMenuItem.Checked;
			DrawEntireModel();
		}

		private void ExportModel_Assimp(string filename, bool selected)
		{
			Assimp.AssimpContext context = new Assimp.AssimpContext();
			Assimp.Scene scene = new Assimp.Scene();
			scene.Materials.Add(new Assimp.Material());
			Assimp.Node n = new Assimp.Node();
			n.Name = "RootNode";
			scene.RootNode = n;
			string rootPath = Path.GetDirectoryName(filename);
			List<string> texturePaths = new List<string>();
			if (TextureInfo != null)
			{
				foreach (BMPInfo bmp in TextureInfo)
				{
					texturePaths.Add(Path.Combine(rootPath, bmp.Name + ".png"));
					bmp.Image.Save(Path.Combine(rootPath, bmp.Name + ".png"));
				}
			}

			// Export the whole model, the first child (in root sibling mode), or the selection
			NJS_OBJECT exportmodel;
			if (selected)
				exportmodel = selectedObject;
			else if (rootSiblingMode)
				exportmodel = model.Children[0];
			else
				exportmodel = model;
				SAEditorCommon.Import.AssimpStuff.AssimpExport(exportmodel, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);

			string format = "collada";
			switch (Path.GetExtension(filename).ToLowerInvariant())
			{
				case ".obj":
					format = "obj";
					break;
				case ".fbx":
					format = "fbx";
					break;
				case ".dae":
				default:
					break;
			}
			context.ExportFile(scene, filename, format, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);
		}

		private void aSSIMPExportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string expname = model.Name;
			if (rootSiblingMode)
				expname = model.Children[0].Name;
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Collada (*.dae)|*.dae|Wavefront (*.obj)|*.obj",
				FileName = expname
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					ExportModel_Assimp(a.FileName, false);
				}
			}
		}

		private void loadAnimationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog() {Filter = "All Animation Files|*.action;*.saanim;*.json;*MTN.BIN;*MTN.PRS;*.njm|SA Tools Animation Files|*.saanim;*.action|" +
																		"Ninja Motion Files|*.njm|JSON Files|*.json|Motion Files|*MTN.BIN;*MTN.PRS|All Files|*.*", Multiselect = true })
				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					LoadAnimation(ofd.FileNames);
				}
		}

		private void LoadAnimation(string[] filenames)
		{
			bool first = true;
			foreach (string fn in filenames)
			{
				string extension = Path.GetExtension(fn).ToLower();
				switch (extension)
				{
					case ".saanim":
						if (!NJS_MOTION.CheckAnimationFile(fn))
						{
							MessageBox.Show(this, $"\"{fn}\" is not a valid animation file.", "SAMDL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						NJS_MOTION anim = NJS_MOTION.Load(fn);
						if (first)
						{
							first = false;
							animframe = 0;
							animnum = animations.Count;
							animations.Add(anim);
							animation = anim;
							UpdateWeightedModel();
							DrawEntireModel();
						}
						else
							animations.Add(anim);
						break;
					case ".njm":
						byte[] njmFile = File.ReadAllBytes(fn);
						ByteConverter.BigEndian = SA_Tools.HelperFunctions.CheckBigEndianInt32(njmFile, 0xC);
						bool useShortRot = ByteConverter.ToInt32(njmFile, 0x8) == 0xC;

						byte[] newFile = new byte[njmFile.Length - 0x8];
						Array.Copy(njmFile, 0x8, newFile, 0, newFile.Length);

						string njmName = Path.GetFileNameWithoutExtension(fn);
						Dictionary<int, string> label = new Dictionary<int, string>();
						label.Add(0, njmName);
						NJS_MOTION njm = new NJS_MOTION(newFile, 0, 0, model.CountAnimated(), label, useShortRot);
						if (first)
						{
							first = false;
							animframe = 0;
							animnum = animations.Count;
							animations.Add(njm);
							animation = njm;
							UpdateWeightedModel();
							DrawEntireModel();
						}
						else
							animations.Add(njm);
						break;
					case ".bin":
					case ".prs":
						byte[] anifile = File.ReadAllBytes(fn);
						Dictionary<int, int> processedanims = new Dictionary<int, int>();

						if (extension.Equals(".prs", StringComparison.OrdinalIgnoreCase))
							anifile = FraGag.Compression.Prs.Decompress(anifile);

						if (BitConverter.ToInt16(anifile, 0) == 0)
						{
							ByteConverter.BigEndian = SA_Tools.HelperFunctions.CheckBigEndianInt16(anifile, 0);
						}
						else
						{
							ByteConverter.BigEndian = SA_Tools.HelperFunctions.CheckBigEndianInt16(anifile, 8);
						}

						int address = 0;
						int i = ByteConverter.ToInt16(anifile, address);
						while (i != -1)
						{
							int aniaddr = ByteConverter.ToInt32(anifile, address + 4);
							if (!processedanims.ContainsKey(aniaddr))
							{
								NJS_MOTION mtn = new NJS_MOTION(anifile, aniaddr, 0, ByteConverter.ToInt16(anifile, address + 2));
								processedanims[aniaddr] = i;

								if (first)
								{
									first = false;
									animframe = 0;
									animations = new List<NJS_MOTION>();
									animations.Add(mtn);

									animnum = animations.Count;
									animation = animations[0];
									UpdateWeightedModel();
									DrawEntireModel();
								}
								else
								{
									animations.Add(mtn);
								}
							}

							address += 8;
							i = ByteConverter.ToInt16(anifile, address);
						}

						break;
					case ".action":
						using (TextReader tr = File.OpenText(fn))
						{
							string path = Path.GetDirectoryName(fn);
							int count = File.ReadLines(fn).Count();
							string[] animationFiles = new string[count];
							for (int u = 0; u < count; u++)
							{
								animationFiles[u] = tr.ReadLine();
								if (File.Exists(Path.Combine(path, animationFiles[u])))
								{
									if (Path.GetExtension(animationFiles[u]).ToLowerInvariant() == ".json")
									{
										JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
										using (TextReader tr2 = File.OpenText(Path.Combine(path, animationFiles[u])))
										{
											JsonTextReader jtr = new JsonTextReader(tr2);
											NJS_MOTION mot = js.Deserialize<NJS_MOTION>(jtr);
											if (first)
											{
												first = false;
												animframe = 0;
												animnum = animations.Count;
												animations.Add(mot);
												animation = mot;
												UpdateWeightedModel();
												DrawEntireModel();
											}
											else
												animations.Add(mot);
										}
									}
									else
									{
										NJS_MOTION mot = NJS_MOTION.Load(Path.Combine(path, animationFiles[u]));
										if (first)
										{
											first = false;
											animframe = 0;
											animnum = animations.Count;
											animations.Add(mot);
											animation = mot;
											UpdateWeightedModel();
											DrawEntireModel();
										}
										else
											animations.Add(mot);
									}
								}
							}
						}
						break;
					case ".json":
						JsonSerializer js2 = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
						using (TextReader tr2 = File.OpenText(fn))
						{
							JsonTextReader jtr = new JsonTextReader(tr2);
							NJS_MOTION mot = js2.Deserialize<NJS_MOTION>(jtr);
							if (first)
							{
								first = false;
								animframe = 0;
								animnum = animations.Count;
								animations.Add(mot);
								animation = mot;
								UpdateWeightedModel();
								DrawEntireModel();
							}
							else
								animations.Add(mot);
						}
						break;
				}

				if (animations.Count > 0) buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = true;

				//Play our animation in the viewport after loading it. To make sure this will work, we need to disable and reenable it.
				if (animations == null || animation == null) return;
				timer1.Enabled = false;
				buttonPlayAnimation.Checked = false;
				timer1.Enabled = true;
				osd.UpdateOSDItem("Play animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = buttonPlayAnimation.Checked = true;
				UpdateWeightedModel();
				DrawEntireModel();
			}
			saveAnimationsToolStripMenuItem.Enabled = animations.Count > 0;
		}

		private void welcomeTutorialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowWelcomeScreen();
		}

		private void UnloadTextures()
		{
			TextureInfo = null;
			Textures = null;
			unloadTextureToolStripMenuItem.Enabled = false;
		}

		private void unloadTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UnloadTextures();
			DrawEntireModel();
		}

		private void SwapUV(NJS_OBJECT obj)
		{
			if (obj.Attach != null && obj.Attach is BasicAttach)
			{
				BasicAttach att = (BasicAttach)obj.Attach;
				foreach (NJS_MESHSET m in att.Mesh)
				{
					for (int u = 0; u < m.UV.Length; u++)
					{
						m.UV[u] = new UV(m.UV[u].V, m.UV[u].U);
					}
					att.ProcessVertexData();
					NJS_OBJECT[] objs = model.GetObjects();
					meshes[Array.IndexOf(objs, obj)] = att.CreateD3DMesh();
				}
			}
			if (obj.Children.Count > 0)
			{
				foreach (NJS_OBJECT child in obj.Children)
				{
					SwapUV(child);
				}
			}
			DrawEntireModel();
			unsaved = true;
		}

		private void swapUVToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (model != null) SwapUV(model);
		}

		private void DeleteSelectedModel()
		{
			if (selectedObject != null && selectedObject.Parent != null)
			{
				bool doOperation = false;
				DialogResult dialogResult = MessageBox.Show("This will delete the selected model and all its child models. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
				doOperation = dialogResult == DialogResult.Yes;
				if (doOperation)
				{
					selectedObject.ClearChildren();
					selectedObject.Parent.RemoveChild(selectedObject);
					selectedObject = null;
					RebuildModelCache();
					SelectedItemChanged();
					unsaved = true;
					osd.UpdateOSDItem("Model deleted", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				}
			}
			else
				osd.UpdateOSDItem("Cannot delete root node", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
		}

		private void RebuildModelCache()
		{
			model.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			meshes = new Mesh[models.Length];
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					try { meshes[i] = models[i].Attach.CreateD3DMesh(); }
					catch { }
			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
			AddTreeNode(model, treeView1.Nodes);
		}

		private void ClearChildren()
		{
			if (selectedObject != null)
			{
				bool doOperation = false;
				DialogResult dialogResult = MessageBox.Show("This will delete the model's children. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
				doOperation = dialogResult == DialogResult.Yes;
				if (doOperation)
				{
					selectedObject.ClearChildren();
					RebuildModelCache();
					DrawEntireModel();
					SelectedItemChanged();
					unsaved = true;
				}
			}
		}

		private void addChildToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedObject != null)
			{
				selectedObject.AddChild(new NJS_OBJECT());
				RebuildModelCache();
				DrawEntireModel();
				SelectedItemChanged();
				unsaved = true;
			}
		}

		private void clearChildrenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearChildren();
			DrawEntireModel();
		}

		private void MessageTimer_Tick(object sender, EventArgs e)
		{
			if (osd != null && osd.UpdateTimer()) DrawEntireModel();
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			contextMenuStripNew.Show(Cursor.Position);
		}

		private void basicModelToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.Basic);
		}

		private void chunkModelToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.Chunk);
		}

		private void gamecubeModelToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			NewFileOperation(ModelFormat.GC);
		}

		private void buttonOpen_Click(object sender, EventArgs e)
		{
			openToolStripMenuItem_Click(sender, e);
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			saveMenuItem_Click(sender, e);
		}

		private void buttonSaveAs_Click(object sender, EventArgs e)
		{
			saveAsToolStripMenuItem_Click(sender, e);
		}

		private void buttonShowNodes_Click(object sender, EventArgs e)
		{
			showNodesToolStripMenuItem.Checked = !showNodesToolStripMenuItem.Checked;
		}

		private void buttonShowNodeConnections_Click(object sender, EventArgs e)
		{
			showNodeConnectionsToolStripMenuItem.Checked = !showNodeConnectionsToolStripMenuItem.Checked;
		}

		private void buttonShowWeights_Click(object sender, EventArgs e)
		{
			showWeightsToolStripMenuItem.Checked = !showWeightsToolStripMenuItem.Checked;
		}

		private void buttonShowHints_Click(object sender, EventArgs e)
		{
			showHintsToolStripMenuItem.Checked = !showHintsToolStripMenuItem.Checked;
		}

		private void buttonPreferences_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
		}

		private void buttonSolid_Click(object sender, EventArgs e)
		{
			EditorOptions.RenderFillMode = FillMode.Solid;
			buttonSolid.Checked = true;
			buttonVertices.Checked = false;
			buttonWireframe.Checked = false;
			osd.UpdateOSDItem("Render mode: Solid", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			DrawEntireModel();
		}

		private void buttonVertices_Click(object sender, EventArgs e)
		{
			EditorOptions.RenderFillMode = FillMode.Point;
			buttonSolid.Checked = false;
			buttonVertices.Checked = true;
			buttonWireframe.Checked = false;
			osd.UpdateOSDItem("Render mode: Point", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			DrawEntireModel();
		}

		private void buttonWireframe_Click(object sender, EventArgs e)
		{
			EditorOptions.RenderFillMode = FillMode.Wireframe;
			buttonSolid.Checked = false;
			buttonVertices.Checked = false;
			buttonWireframe.Checked = true;
			osd.UpdateOSDItem("Render mode: Wireframe", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			DrawEntireModel();
		}

		private void buttonPrevAnimation_Click(object sender, EventArgs e)
		{
			PreviousAnimation();
		}

		private void buttonNextAnimation_Click(object sender, EventArgs e)
		{
			NextAnimation();
		}

		private void buttonPrevFrame_Click(object sender, EventArgs e)
		{
			PrevFrame();
		}

		private void buttonPlayAnimation_Click(object sender, EventArgs e)
		{
			PlayPause();
		}

		private void buttonNextFrame_Click(object sender, EventArgs e)
		{
			NextFrame();
		}

		private void showHintsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			osd.show_osd = !osd.show_osd;
			buttonShowHints.Checked = showHintsToolStripMenuItem.Checked;
			DrawEntireModel();
		}

		private void buttonLighting_Click(object sender, EventArgs e)
		{
			string lighting = "On";
			EditorOptions.OverrideLighting = !EditorOptions.OverrideLighting;
			buttonLighting.Checked = !EditorOptions.OverrideLighting;
			if (EditorOptions.OverrideLighting) lighting = "Off";
			osd.UpdateOSDItem("Lighting: " + lighting, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			UpdateWeightedModel();
			DrawEntireModel();
		}

		private void materialEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenMaterialEditor();
		}

		private void saveAnimationsToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			string filterString = "SA Tools Animation |*.saanim|Sega Ninja Motion .njm|*.njm|Sega Ninja Motion Big Endian (Gamecube) .njm|*.njm|JSON Files|*.json";

			filterString += "|All files *.*|*.*";
			using (SaveFileDialog a = new SaveFileDialog()
			{
				Title = "Select a base filename",
				DefaultExt = "saanim",
				Filter = filterString
			})
			{
				if (currentFileName.Length > 0) a.InitialDirectory = currentFileName;

				if (a.ShowDialog(this) == DialogResult.OK)
				{
					switch(a.FilterIndex)
					{
						case 3:
							a.FileName = a.FileName + "?BE?";
							break;
						default:
							break;
					}
					SaveAnimations(a.FileName);
				}
			}
		}

		private void SaveAnimations(string fileName)
		{
			if (animations != null)
			{
				bool bigEndian = false;
				string extension = Path.GetExtension(fileName);

				switch (extension)
				{
					case ".njm":
					case ".njm?BE?":
						if (extension.Contains("?BE?"))
						{
							bigEndian = true;
						}
						fileName = fileName.Replace("?BE?", "");
						ByteConverter.BigEndian = bigEndian;
						for (int u = 0; u < animations.Count; u++)
						{
							string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_" + u.ToString() + "_" + animations[u].Name + ".njm";
							byte[] rawAnim = animations[u].GetBytes(0, new Dictionary<string, uint>(), out uint address, true, false);

							File.WriteAllBytes(filePath, rawAnim);
						}
						break;
					case ".saanim":
					case ".json":
						using (TextWriter twmain = File.CreateText(Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".action")))
						{
							string[] animfiles = new string[animations.Count()];
							for (int u = 0; u < animations.Count; u++)
							{
								string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_" + u.ToString() + "_" + animations[u].Name + extension;
								if (extension == ".saanim")
									animations[u].Save(filePath);
								else
								{
									JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
									using (TextWriter tw = File.CreateText(filePath))
									using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
										js.Serialize(jtw, animations[u]);
								}
								twmain.WriteLine(Path.GetFileName(filePath));
							}
							twmain.Flush();
							twmain.Close();
						}
						break;
				}
			}
			else
			{
				MessageBox.Show("No animations loaded to save!");
				return;
			}
		}

		private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(true);
		}

		private void buttonMaterialColors_CheckedChanged(object sender, EventArgs e)
		{
			string showmatcolors = "On";
			EditorOptions.IgnoreMaterialColors = !buttonMaterialColors.Checked;
			if (EditorOptions.IgnoreMaterialColors) showmatcolors = "Off";
			osd.UpdateOSDItem("Material Colors: " + showmatcolors, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			UpdateWeightedModel();
			DrawEntireModel();
		}

		private void modelCodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ModelText text = new ModelText();
			string[] texnames = null;
			if (TexturePackName != null && exportTextureNamesToolStripMenuItem.Checked)
			{
				texnames = new string[TextureInfo.Length];
				for (int i = 0; i < TextureInfo.Length; i++)
					texnames[i] = string.Format("{0}TexName_{1}", TexturePackName, TextureInfo[i].Name);
				text.export += "enum " + TexturePackName + "TexName";
				text.export += System.Environment.NewLine;
				text.export += "{";
				text.export += "\t"+ string.Join("," + Environment.NewLine + "\t", texnames);
				text.export += System.Environment.NewLine;
				text.export += "};";
				text.export += System.Environment.NewLine;
				text.export += System.Environment.NewLine;
			}
			List<string> labels = new List<string>() { model.Name };
			text.export += model.ToStructVariables(true, labels, texnames); //Need to make the DX thing a toggle
			if (exportAnimationsToolStripMenuItem.Checked && animations != null)
			{
				foreach (NJS_MOTION anim in animations)
				{
					text.export += anim.ToStructVariables(labels);
				}
			}
			text.ShowDialog();
		}

		private void AddPolyNormal(NJS_OBJECT obj)
		{
			if (obj.Attach is BasicAttach)
			{
				BasicAttach att = (BasicAttach)obj.Attach;
				foreach (NJS_MESHSET mesh in att.Mesh)
				{
					mesh.PolyNormal = new Vertex[mesh.Poly.Count];
					for (int p = 0; p < mesh.PolyNormal.Count(); p++)
					{
						mesh.PolyNormal[p] = new Vertex(0, 0, 0);
					}
				}
			}
			if (obj.Children != null && obj.Children.Count > 0)
			{
				foreach (NJS_OBJECT child in obj.Children)
					AddPolyNormal(child);
			}
		}

		private void RemovePolyNormal(NJS_OBJECT obj)
		{
			if (obj.Attach is BasicAttach)
			{
				BasicAttach att = (BasicAttach)obj.Attach;
				foreach (NJS_MESHSET mesh in att.Mesh)
				{
					mesh.PolyNormal = null;
				}
			}
			if (obj.Children != null && obj.Children.Count > 0)
			{
				foreach (NJS_OBJECT child in obj.Children)
					RemovePolyNormal(child);
			}
		}

		private void createToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddPolyNormal(selectedObject);
		}

		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RemovePolyNormal(selectedObject);
		}

		private void showNodeConnectionsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			string shownodecons = "Off";
			if (showNodeConnectionsToolStripMenuItem.Checked) shownodecons = "On";
			osd.UpdateOSDItem("Show node connections: " + shownodecons, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			buttonShowNodeConnections.Checked = showNodeConnectionsToolStripMenuItem.Checked;
			DrawEntireModel();
		}

		private void clearChildrenToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ClearChildren();
			DrawEntireModel();
		}

		private void emptyModelDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("This will delete all meshes in the selected node. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
			if (dialogResult == DialogResult.Yes)
			{
				selectedObject.Attach = null;
				RebuildModelCache();
				DrawEntireModel();
				SelectedItemChanged();
				unsaved = true;
			}
		}

		private void deleteTheWholeHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteSelectedModel();
			DrawEntireModel();
		}

		private void transparentOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			transparentOnlyToolStripMenuItem.Checked = !transparentOnlyToolStripMenuItem.Checked;
		}

		private void sortMeshsetsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("This operation cannot be undone. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
			if (dialogResult == DialogResult.Yes)
				SortModel(selectedObject, false);
		}

		private void hierarchyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("This operation cannot be undone. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
			if (dialogResult == DialogResult.Yes)
				SortModel(selectedObject, true);
		}

		// Model Library
		private void modelLibraryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// show model library
			modelLibraryWindow.Show();
		}

		void modelLibrary_SelectionChanged(ModelLibraryControl sender, int selectionIndex, Attach selectedModel)
		{
			if ((selectionIndex >= 0) && (selectedModel != null) && selectedObject != null)
			{
				selectedObject.Attach = selectedModel;
				selectedObject.Attach.ProcessVertexData();
				NJS_OBJECT[] models = model.GetObjects();
				try { meshes[Array.IndexOf(models, selectedObject)] = selectedObject.Attach.CreateD3DMesh(); }
				catch { }
				DrawEntireModel();
			}
		}

		private void AddModelToLibrary(NJS_OBJECT objectToAdd, bool additive)
		{
			if (additive) modelLibrary.Clear();

			NJS_OBJECT[] models = objectToAdd.GetObjects();

			modelLibrary.BeginUpdate();
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					modelLibrary.Add(models[i].Attach);
			modelLibrary.EndUpdate();
		}

		private void Resize()
		{
			// Causes a memory leak so not used for now
			if (d3ddevice == null) return;
			DeviceResizing = true;
			PresentParameters pp = new PresentParameters
			{
				Windowed = true,
				SwapEffect = SwapEffect.Discard,
				EnableAutoDepthStencil = true,
				AutoDepthStencilFormat = Format.D24X8,
				BackBufferHeight=RenderPanel.Height,
				BackBufferWidth = RenderPanel.Width
			};
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			d3ddevice.Dispose();
			SharpDX.Direct3D9.Direct3D d3d = new SharpDX.Direct3D9.Direct3D();
			d3ddevice = new Device(d3d, 0, DeviceType.Hardware, RenderPanel.Handle, CreateFlags.HardwareVertexProcessing, pp);
			osd = new OnScreenDisplay(d3ddevice, Color.Red.ToRawColorBGRA());
			EditorOptions.Initialize(d3ddevice);
			if (TextureInfo != null && TextureInfo.Count() > 0)
			{
				Texture[] texs = new Texture[TextureInfo.Count()];
				for (int j = 0; j < TextureInfo.Count(); j++)
					texs[j] = TextureInfo[j].Image.ToTexture(d3ddevice);
				Textures = texs;
			}
			DeviceResizing = false;
			DrawEntireModel();
		}

		// Splitting models
		private void byMeshsetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(selectedObject.Attach is BasicAttach))
			{
				MessageBox.Show("This operation is only supported for basic models.");
				return;
			}
			bool doOperation = false;
			BasicAttach basicatt = (BasicAttach)selectedObject.Attach;
			if (selectedObject.Attach == null || basicatt.Mesh.Count == 0)
			{
				MessageBox.Show("This model has no meshset data.");
				return;
			}
			else if (basicatt.Mesh.Count == 1)
			{
				DialogResult dialogResult = MessageBox.Show("This model only has one meshset. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
				doOperation = dialogResult == DialogResult.Yes;
			}
			else
			{
				DialogResult dialogResult = MessageBox.Show("This operation cannot be undone. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
				doOperation = dialogResult == DialogResult.Yes;
			}
			if (!doOperation) return;
			int id = 0;
			BasicAttach opaqueatt = new BasicAttach();
			// Add opaque meshes to a child model
			if (transparentOnlyToolStripMenuItem.Checked)
			{
				List<NJS_MESHSET> opaquemeshes = new List<NJS_MESHSET>();
				List<NJS_MATERIAL> opaquematerials = new List<NJS_MATERIAL>();
				List<Vertex> vlist_new = new List<Vertex>();
				List<Vertex> nlist_new = new List<Vertex>();
				ushort newpid = 0;
				foreach (NJS_MESHSET m in basicatt.Mesh)
				{
					NJS_MATERIAL mat = basicatt.Material[m.MaterialID];
					if (mat.UseAlpha) continue;
					Dictionary<ushort, ushort> vmatchlist = new Dictionary<ushort, ushort>();
					NJS_MESHSET mesh_new = m.Clone();
					if (!opaquematerials.Contains(mat))
					{
						opaquematerials.Add(mat);
					}
					mesh_new.MaterialID = (ushort)opaquematerials.IndexOf(mat);
					for (int p = 0; p < mesh_new.Poly.Count; p++)
					{
						for (int i = 0; i < mesh_new.Poly[p].Indexes.Length; i++)
						{
							ushort polyind = mesh_new.Poly[p].Indexes[i];
							if (!vmatchlist.ContainsKey(polyind))
							{
								vlist_new.Add(basicatt.Vertex[polyind]);
								nlist_new.Add(basicatt.Normal[polyind]);
								mesh_new.Poly[p].Indexes[i] = newpid;
								vmatchlist.Add(polyind, newpid);
								newpid++;
							}
							else mesh_new.Poly[p].Indexes[i] = vmatchlist[polyind];
						}
					}
					opaquemeshes.Add(mesh_new);
				}
				opaqueatt = new BasicAttach(vlist_new.ToArray(), nlist_new.ToArray(), opaquemeshes, opaquematerials);
				opaqueatt.Bounds.Center = basicatt.Bounds.Center;
				opaqueatt.Bounds.Radius = basicatt.Bounds.Radius;
				opaqueatt.Name = basicatt.Name + "_" + id.ToString();
				opaqueatt.MaterialName = basicatt.MaterialName + "_" + id.ToString();
				opaqueatt.VertexName = basicatt.VertexName + "_" + id.ToString();
				opaqueatt.NormalName = basicatt.NormalName + "_" + id.ToString();
				opaqueatt.MeshName = basicatt.MeshName + "_" + id.ToString();
			}
			// Add meshes to a child model (or both opaque and transparent if the option is off)
			foreach (NJS_MESHSET m in basicatt.Mesh)
			{
				if (transparentOnlyToolStripMenuItem.Checked && !basicatt.Material[m.MaterialID].UseAlpha)
					continue;
				Dictionary<ushort, ushort> vmatchlist = new Dictionary<ushort, ushort>();
				ushort newpid = 0;
				NJS_MESHSET mesh_new = m.Clone();
				List<Vertex> vlist_new = new List<Vertex>();
				List<Vertex> nlist_new = new List<Vertex>();
				for (int p = 0; p < mesh_new.Poly.Count; p++)
				{
					for (int i = 0; i < mesh_new.Poly[p].Indexes.Length; i++)
					{
						ushort polyind = mesh_new.Poly[p].Indexes[i];
						if (!vmatchlist.ContainsKey(polyind))
						{
							vlist_new.Add(basicatt.Vertex[polyind]);
							nlist_new.Add(basicatt.Normal[polyind]);
							mesh_new.Poly[p].Indexes[i] = newpid;
							vmatchlist.Add(polyind, newpid);
							newpid++;
						}
						else mesh_new.Poly[p].Indexes[i] = vmatchlist[polyind];
					}
				}
				List<NJS_MESHSET> meshlist_new = new List<NJS_MESHSET>();
				List<NJS_MATERIAL> matlist_new = new List<NJS_MATERIAL>();
				mesh_new.MaterialID = 0;
				meshlist_new.Add(mesh_new);
				matlist_new.Add(basicatt.Material[m.MaterialID]);
				BasicAttach newatt = new BasicAttach(vlist_new.ToArray(), nlist_new.ToArray(), meshlist_new, matlist_new);
				newatt.Bounds.Center = basicatt.Bounds.Center;
				newatt.Bounds.Radius = basicatt.Bounds.Radius;
				newatt.Name = basicatt.Name + "_" + id.ToString();
				newatt.MaterialName = basicatt.MaterialName + "_" + id.ToString();
				newatt.VertexName = basicatt.VertexName + "_" + id.ToString();
				newatt.NormalName = basicatt.NormalName + "_" + id.ToString();
				newatt.MeshName = basicatt.MeshName + "_" + id.ToString();
				NJS_OBJECT newobj = new NJS_OBJECT();
				newobj.Attach = newatt;
				newobj.Name = selectedObject.Name + "_" + id.ToString();
				selectedObject.AddChild(newobj);
				id++;
			}
			if (transparentOnlyToolStripMenuItem.Checked)
				selectedObject.Attach = opaqueatt;
			else
				selectedObject.Attach = null;
			RebuildModelCache();
			DrawEntireModel();
			SelectedItemChanged();
			unsaved = true;
		}

		private void importAsModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.obj;*.fbx;*.dae;|All Files|*.*"
			})
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ImportModel_Assimp(ofd.FileName, true);
				}
			}
		}

		private void importAsNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.obj;*.fbx;*.dae;|All Files|*.*"
			})
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ImportModel_Assimp(ofd.FileName, false);
				}
			}
		}

		private void exportContextMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Collada (*.dae)|*.dae|Wavefront (*.obj)|*.obj",
				FileName = selectedObject.Name
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					ExportModel_Assimp(a.FileName, true);
				}
			}
		}

		private void importSelected(bool asSingle)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()
			{
				DefaultExt = "obj",
				Filter = "Model Files|*.dae;*.fbx;*.obj"
			})
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{

					ImportModel_Assimp(dlg.FileName, asSingle, true);

					if (selectedObject.Attach != null)
					{
						Matrix m = Matrix.Identity;
						selectedObject.ProcessTransforms(m);

						float scale = Math.Max(Math.Max(selectedObject.Scale.X, selectedObject.Scale.Y), selectedObject.Scale.Z);
						BoundingSphere bounds = new BoundingSphere(Vector3.TransformCoordinate(selectedObject.Attach.Bounds.Center.ToVector3(), m).ToVertex(), selectedObject.Attach.Bounds.Radius * scale);

						bool boundsVisible = !cam.SphereInFrustum(bounds);

						if (!boundsVisible)
						{
							cam.MoveToShowBounds(bounds);
						}
					}
					DrawEntireModel();
					unsaved = true;
				}
		}

		private void importSelectedAsMeshContextMenuItem_Click(object sender, EventArgs e)
		{
			importSelected(true);
		}

		private void importSelectedAsNodesContextMenuItem_Click(object sender, EventArgs e)
		{
			importSelected(false);
		}

		private void showVertexIndicesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			osd.UpdateOSDItem("Show vertex indices: " + (buttonShowVertexIndices.Checked ? "On" : "Off"), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			buttonShowVertexIndices.Checked = showVertexIndicesToolStripMenuItem.Checked;
			DrawEntireModel();
		}

		private void buttonShowVertexIndices_Click(object sender, EventArgs e)
		{
			showVertexIndicesToolStripMenuItem.Checked = !showVertexIndicesToolStripMenuItem.Checked;
		}

		private void byFaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!(selectedObject.Attach is BasicAttach))
			{
				MessageBox.Show("This operation is only supported for basic models.");
				return;
			}
			bool doOperation = false;
			BasicAttach basicatt = (BasicAttach)selectedObject.Attach;
			if (selectedObject.Attach == null || basicatt.Mesh.Count == 0)
			{
				MessageBox.Show("This model has no meshset data.");
				return;
			}
			else
			{
				DialogResult dialogResult = MessageBox.Show("This operation cannot be undone. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
				doOperation = dialogResult == DialogResult.Yes;
			}
			if (doOperation)
			{
				int id = 0;
				foreach (NJS_MESHSET m in basicatt.Mesh)
				{
					int uvid = 0;
					if (m.Poly == null) continue;
					for (int p = 0; p < m.Poly.Count; p++)
					{
						List<NJS_MESHSET> meshlist_new = new List<NJS_MESHSET>();
						List<NJS_MATERIAL> matlist_new = new List<NJS_MATERIAL>();
						List<Vertex> vlist_new = new List<Vertex>();
						List<Vertex> nlist_new = new List<Vertex>();
						List<UV> uvlist_new = new List<UV>();
						List<Color> vclist_new = new List<Color>();
						List<ushort> polys = new List<ushort>();
						Poly newpoly = m.Poly[p].Clone();
						for (ushort i = 0; i < m.Poly[p].Indexes.Length; i++)
						{
							ushort vrtidx = m.Poly[p].Indexes[i];
							vlist_new.Add(basicatt.Vertex[vrtidx]);
							nlist_new.Add(basicatt.Normal[vrtidx]);
							if (m.UV != null) uvlist_new.Add(m.UV[uvid]);
							if (m.VColor != null) vclist_new.Add(m.VColor[uvid]);
							newpoly.Indexes[i] = i;
							uvid++;
						}
						Poly[] polyarr = new Poly[1];
						polyarr[0] = newpoly;
						NJS_MESHSET mesh_new = new NJS_MESHSET(polyarr, m.PolyNormal != null, m.UV != null, m.VColor != null);
						mesh_new.PolyName = m.PolyName + "_" + id.ToString();
						if (mesh_new.UV != null)
						{
							mesh_new.UVName = m.UVName + "_" + id.ToString();
							UV[] list = uvlist_new.ToArray();
							for (int u = 0; u < mesh_new.UV.Length; u++)
								mesh_new.UV[u] = list[u];
						}
						if (mesh_new.VColor != null)
						{
							mesh_new.VColorName = m.VColorName + "_" + id.ToString();
							Color[] clist = vclist_new.ToArray();
							for (int u = 0; u < mesh_new.VColor.Length; u++)
								mesh_new.VColor[u] = clist[u];
						}
						mesh_new.MaterialID = 0;
						meshlist_new.Add(mesh_new);
						matlist_new.Add(basicatt.Material[m.MaterialID]);
						BasicAttach newatt = new BasicAttach(vlist_new.ToArray(), nlist_new.ToArray(), meshlist_new, matlist_new);
						newatt.Bounds.Center = basicatt.Bounds.Center;
						newatt.Bounds.Radius = basicatt.Bounds.Radius;
						newatt.Name = basicatt.Name + "_" + id.ToString();
						newatt.MaterialName = basicatt.MaterialName + "_" + id.ToString();
						newatt.VertexName = basicatt.VertexName + "_" + id.ToString();
						newatt.NormalName = basicatt.NormalName + "_" + id.ToString();
						newatt.MeshName = basicatt.MeshName + "_" + id.ToString();
						NJS_OBJECT newobj = new NJS_OBJECT();
						newobj.Attach = newatt;
						newobj.Name = selectedObject.Name + "_" + id.ToString();
						selectedObject.AddChild(newobj);
						id++;
					}
				}
				selectedObject.Attach = null;
				RebuildModelCache();
				DrawEntireModel();
				SelectedItemChanged();
				unsaved = true;
			}
		}

		// Meshset sorting
		private void SortModel(NJS_OBJECT mdl, bool withchildren)
		{
			if (mdl.Attach != null)
			{
				if (!(mdl.Attach is BasicAttach))
				{
					MessageBox.Show("This operation is only supported for basic models.");
					return;
				}
				BasicAttach basicatt = (BasicAttach)mdl.Attach;
				List<NJS_MESHSET> mesh_opaque = new List<NJS_MESHSET>();
				List<NJS_MESHSET> mesh_trans = new List<NJS_MESHSET>();
				List<NJS_MATERIAL> mats_opaque = new List<NJS_MATERIAL>();
				List<NJS_MATERIAL> mats_trans = new List<NJS_MATERIAL>();
				Dictionary<ushort, ushort> matsidx_trans = new Dictionary<ushort, ushort>();
				Dictionary<ushort, ushort> matsidx_opaque = new Dictionary<ushort, ushort>();
				ushort matid_current = 0;
				List<ushort> matids = new List<ushort>();
				// Opaque meshes
				foreach (NJS_MESHSET m in basicatt.Mesh)
				{
					if (!basicatt.Material[m.MaterialID].UseAlpha)
					{
						mesh_opaque.Add(m);
						if (!mats_opaque.Contains(basicatt.Material[m.MaterialID]))
						{
							mats_opaque.Add(basicatt.Material[m.MaterialID]);
							matids.Add(matid_current);
							matid_current++;
						}
						else
							matids.Add((ushort)mats_opaque.IndexOf(basicatt.Material[m.MaterialID]));
					}
				}
				// Transparent meshes
				foreach (NJS_MESHSET m in basicatt.Mesh)
				{
					if (basicatt.Material[m.MaterialID].UseAlpha)
					{
						mesh_trans.Add(m);
						if (!mats_trans.Contains(basicatt.Material[m.MaterialID]))
						{
							mats_trans.Add(basicatt.Material[m.MaterialID]);
							matids.Add(matid_current);
							matid_current++;
						}
						else
							matids.Add((ushort)(mats_opaque.Count - 1 + mats_trans.IndexOf(basicatt.Material[m.MaterialID])));
					}
				}
				mesh_opaque.AddRange(mesh_trans);
				mats_opaque.AddRange(mats_trans);
				ushort matid_new = 0;
				foreach (NJS_MESHSET m in mesh_opaque)
				{
					m.MaterialID = matids[matid_new];
					matid_new++;
				}
				BasicAttach basicatt_new = new BasicAttach(basicatt.Vertex, basicatt.Normal, mesh_opaque, mats_opaque);
				basicatt_new.Bounds.Center = basicatt.Bounds.Center;
				basicatt_new.Bounds.Radius = basicatt.Bounds.Radius;
				basicatt_new.MaterialName = basicatt.MaterialName;
				basicatt_new.MeshName = basicatt.MeshName;
				basicatt_new.NormalName = basicatt.VertexName;
				basicatt_new.VertexName = basicatt.VertexName;
				basicatt_new.Name = basicatt.Name;
				mdl.Attach = basicatt_new;
			}
			if (withchildren)
			{
				foreach (NJS_OBJECT child in mdl.Children)
					SortModel(child, true);
			}
			RebuildModelCache();
			DrawEntireModel();
			SelectedItemChanged();
			unsaved = true;
		}
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
		}
	}
}