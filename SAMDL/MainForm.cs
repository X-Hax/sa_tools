using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Mesh = SAModel.Direct3D.Mesh;
using Point = System.Drawing.Point;
using SplitTools;
using SAModel.SAEditorCommon.ProjectManagement;
using static SAModel.SAEditorCommon.SettingsFile;

namespace SAModel.SAMDL
{
	public partial class MainForm : Form
	{
		// Editor settings
		Settings_SAMDL settingsfile; // For user editable settings
		Properties.Settings AppConfig = Properties.Settings.Default; // For non-user editable settings in SAMDL.config
		Logger log = new Logger();
		string currentProject; // Path to currently loaded project, if it exists
		string lastProjectModeCategory = ""; // Last selected category in the model list

		// Editor variables
		bool loaded;
		bool unsavedChanges = false;
		bool rootSiblingMode = false;
		bool DeviceResizing;
		string currentFileName = "";
		bool hasWeight;
		NJS_OBJECT selectedObject;

		// Model related
		string modelAuthor = "";
		string modelDescription = "";
		NJS_OBJECT model;
		ModelFormat outfmt;

		// Animation related
		List<NJS_MOTION> animationList;
		NJS_MOTION currentAnimation;
		int animnum = -1;
		float animframe = 0;
		float animspeed = 1.0f;
		bool AnimationPlaying = false;

		// Texture related
		string TexturePackName; // Name of the last loaded PVM/texture pack, used for texture enum export
		NJS_TEXLIST TexList; // Current texlist
		NJS_TEXLIST TempTexList; // Texture name list loaded through the model, ex. An .nj NJTL. Use if next loaded texture archive has no names. Clear after each texture load attempt.
		BMPInfo[] TextureInfo; // Textures in the whole PVM/texture pack
		BMPInfo[] TextureInfoCurrent; // TextureInfo updated for the current texlist. Used for Material Editor, texture remapping, C++ export etc.
		Texture[] Textures; // Created from TextureInfoCurrent; used for rendering

		// Rendering related
		bool NeedRedraw = false;
		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		Mesh[] meshes;
		Mesh sphereMesh, selectedSphereMesh, modelSphereMesh, selectedModelSphereMesh;

		// UI related
		Dictionary<NJS_OBJECT, TreeNode> nodeDict;
		OnScreenDisplay osd;
		ModelFileDialog modelinfo = new ModelFileDialog();
		EditorOptionsEditor optionsEditor;
		System.Drawing.Rectangle mouseBounds;
		bool lookKeyDown;
		bool zoomKeyDown;
		bool cameraKeyDown;
		bool mouseWrapScreen = false;
		bool FormResizing;
		FormWindowState LastWindowState = FormWindowState.Minimized;

		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;
		ModelLibraryWindow modelLibraryWindow;
		ModelLibraryControl modelLibrary;

		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			Application.Idle += HandleWaitLoop;
			InitializeComponent();
			AddMouseMoveHandler(this);
			this.MouseWheel += panel1_MouseWheel;
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

		private void MainForm_Resize(object sender, EventArgs e)
		{
			NeedRedraw = true;
		}

		private void SAMDL_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
		}

		private void SAMDL_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			LoadFileList(files);
		}

		public void LoadFileList(string[] files, bool cmdLoad = false)
		{
			bool modelLoaded = false; // We can only load one model at once for now. Set to true when user loads first file and ignore others.
			bool modelLoadedWarning = false; // Flag so we can warn users that they should do one model at a time.

			List<string> modelFiles = new List<string>(); // For multi support, if that happens
			List<string> modelImportFiles = new List<string>();
			List<string> animFiles = new List<string>();

			string modelWarning = "Only 1 model can be loaded at a time!";

			foreach (string file in files)
			{
				string extension = Path.GetExtension(file).ToLowerInvariant();
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
					case ".pvm":
					case ".gvm":
					case ".xvm":
					case ".pb":
					case ".pvmx":
					case ".pak":
					case ".prs":
					case ".pvr":
					case ".xvr":
						AddSingleTexture(file);
						break;
					case ".gvr":
					case ".txt":
						LoadTextures(file);
						break;
					case ".satex":
						LoadTexlistFile(file);
						break;
					case ".sap":
						LoadFile(file);
						break;
					default:
						break;
				}
			}
			if (modelFiles.Count > 0) { LoadFile(modelFiles[0], cmdLoad); }
			if (modelImportFiles.Count > 0)
				ImportModelFromFile(modelImportFiles[0]);
			if (animFiles.Count > 0) { LoadAnimation(animFiles.ToArray()); }
		}

		private void ImportModelFromFile(string modelFilename)
		{
			using (AssimpImportDialog dialog = new AssimpImportDialog(outfmt, Path.GetExtension(modelFilename).ToLowerInvariant() == ".obj"))
				switch (dialog.ShowDialog())
				{
					case DialogResult.OK:
						outfmt = dialog.Format;
						if (dialog.LegacyOBJImport)
						{
							NewFile(ModelFormat.BasicDX);
							// Load textures if there is a texlist file
							string rootPath = Path.GetDirectoryName(modelFilename);
							string texlistFilename = Path.Combine(rootPath, Path.GetFileNameWithoutExtension(modelFilename) + ".satex");
							if (File.Exists(texlistFilename))
							{
								UnloadTextures();
								List<string> texturesPngFilenames = new List<string>();
								TexList = NJS_TEXLIST.Load(texlistFilename);
								foreach (string texture in TexList.TextureNames)
								{
									texturesPngFilenames.Add(Path.Combine(rootPath, Path.ChangeExtension(texture, ".png")));
								}
								AddTextures(texturesPngFilenames.ToArray());
								UpdateTexlist();
							}
							ImportOBJLegacy(model, modelFilename);
						}
						else
							ImportModel_Assimp(modelFilename, !dialog.ImportAsNodes, false, dialog.ImportColladaRootNode);
						break;
					case DialogResult.Cancel:
					default:
						return;
				}
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

		private void MainForm_Load(object sender, EventArgs e)
		{
			AnimationTimer = new HiResTimer(16.667f);
			AnimationTimer.Elapsed += new EventHandler<HiResTimerElapsedEventArgs>(AdvanceAnimation);
			log.DeleteLogFile();
			log.Add("SAMDL: New log entry on " + DateTime.Now.ToString("G") + "\n");
			log.Add("Build Date: ");
			log.Add(File.GetLastWriteTime(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture));
			log.Add("OS Version: ");
			log.Add(Environment.OSVersion.ToString() + System.Environment.NewLine);
			log.WriteLog();
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			SharpDX.Direct3D9.Direct3DEx d3d = new SharpDX.Direct3D9.Direct3DEx();
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
			settingsfile = Settings_SAMDL.Load();
			EditorOptions.FillColor = settingsfile.BackgroundColor;

			if (settingsfile.ShowWelcomeScreen)
			{
				ShowWelcomeScreen();
			}

			EditorOptions.RenderDrawDistance = settingsfile.DrawDistanceGeneral;
			EditorOptions.Initialize(d3ddevice);
			cam.MoveSpeed = settingsfile.CamMoveSpeed;
			cam.ModifierKey = settingsfile.CameraModifier;
			animspeed = settingsfile.AnimSpeed;
			alternativeCameraModeToolStripMenuItem.Checked = settingsfile.AlternativeCamera;
			EditorOptions.EnableSpecular = settingsfile.EnableSpecular;
			EditorOptions.KeyLight = new Light()
			{
				Type = LightType.Directional,
				Range = 0,
				Direction = settingsfile.KeyLightDirection.ToVector3(),
				Diffuse = settingsfile.KeyLightDiffuse.ToRawColor4(),
				Ambient = settingsfile.KeyLightAmbient.ToRawColor4(),
				Specular = settingsfile.KeyLightSpecular.ToRawColor4(),
			};
			EditorOptions.FillLight = new Light()
			{
				Type = LightType.Directional,
				Range = 0,
				Direction = settingsfile.FillLightDirection.ToVector3(),
				Diffuse = settingsfile.FillLightDiffuse.ToRawColor4(),
				Ambient = settingsfile.FillLightAmbient.ToRawColor4(),
				Specular = settingsfile.FillLightSpecular.ToRawColor4(),
			};
			EditorOptions.BackLight = new Light()
			{
				Type = LightType.Directional,
				Range = 0,
				Direction = settingsfile.BackLightDirection.ToVector3(),
				Diffuse = settingsfile.BackLightDiffuse.ToRawColor4(),
				Ambient = settingsfile.BackLightAmbient.ToRawColor4(),
				Specular = settingsfile.BackLightSpecular.ToRawColor4(),
			};
			actionList = ActionMappingList.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SAMDL_keys.ini"),
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
				LoadFileList(Program.Arguments, true);
			NeedRedraw = true;
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

			welcomeForm.ThisToolLink.Text = "SAMDL Documentation";
			welcomeForm.ThisToolLink.Visible = true;

			welcomeForm.ThisToolLink.LinkClicked += (object link, LinkLabelLinkClickedEventArgs linkEventArgs) =>
			{
				welcomeForm.GoToSite("https://github.com/X-Hax/sa_tools/wiki/SAMDL");
			};

			welcomeForm.ShowDialog();

			welcomeForm.Dispose();
			welcomeForm = null;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded && unsavedChanges)
				switch (MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "SAMDL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						if (File.Exists(currentFileName))
							Save(currentFileName);
						else
							SaveAs(exportAnimationsToolStripMenuItem.Checked);
						break;
					case DialogResult.Cancel:
						return;
				}
			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "All supported files|*.sa1mdl;*.sa2mdl;*.sa2bmdl;*.nj;*.gj;*.xj;*.exe;*.dll;*.bin;*.prs;*.rel;*.sap;*.dae;*.fbx;*.obj|All Model Files|*.sa1mdl;*.sa2mdl;*.sa2bmdl;*.nj;*.gj;*.xj;*.exe;*.dll;*.bin;*.prs;*.rel|SA Tools Files|*.sa1mdl;*.sa2mdl;*.sa2bmdl|Ninja Files|*.nj;*.gj|Binary Files|*.exe;*.dll;*.bin;*.prs;*.rel|Other Model Formats|*.obj;*.fbx;*.dae|All Files|*.*"
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
						case DialogResult.Abort:
							Application.Exit();
							break;
					}
				}
			}
		}

		/*
		static public List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
		{
			List<int> positions = new List<int>();
			int patternLength = pattern.Length;
			int totalLength = bytes.Length;
			byte firstMatchByte = pattern[0];
			for (int i = 0; i < totalLength; i++)
			{
				if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
				{
					byte[] match = new byte[patternLength];
					Array.Copy(bytes, i, match, 0, patternLength);
					if (match.SequenceEqual<byte>(pattern))
					{
						positions.Add(i);
						i += patternLength - 1;
					}
				}
			}
			return positions;
		}
		*/

		private void LoadFile(string filename, bool cmdLoad = false)
		{
			string extension = Path.GetExtension(filename).ToLowerInvariant();

			loaded = false;
			if (newModelUnloadsTexturesToolStripMenuItem.Checked) UnloadTextures();
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			if (extension == ".sa1mdl") swapUVToolStripMenuItem.Enabled = true;
			else swapUVToolStripMenuItem.Enabled = false;
			AnimationPlaying = false;
			currentAnimation = null;
			animationList = null;
			setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = buttonPlayAnimation.Enabled = buttonResetFrame.Enabled = false;
			animnum = -1;
			animframe = 0;
			// Model file
			if (ModelFile.CheckModelFile(filename))
			{
				try
				{
					ModelFile modelFile = new ModelFile(filename);
					if (!string.IsNullOrEmpty(modelFile.Description))
						modelDescription = modelFile.Description;
					if (!string.IsNullOrEmpty(modelFile.Author))
						modelAuthor = modelFile.Author;
					outfmt = modelFile.Format;
					if (modelFile.Model.Sibling != null)
					{
						model = new NJS_OBJECT { Name = "Root" };
						model.AddChild(modelFile.Model);
						model.FixParents();
						rootSiblingMode = true;
					}
					else
					{
						model = modelFile.Model;
						rootSiblingMode = false;
					}
					animationList = new List<NJS_MOTION>(modelFile.Animations);
					setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled =
						buttonPrevAnimation.Enabled = buttonResetFrame.Enabled = animationList.Count > 0;
					string labelname = Path.ChangeExtension(filename, ".salabel");
					if (File.Exists(labelname))
						LabelOBJECT.ImportLabels(model, LabelOBJECT.Load(labelname));
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
			else
			{
				byte[] file = File.ReadAllBytes(filename);
				switch (extension)
				{
					// Ninja/Ginja model file
					case ".nj":
					case ".gj":
					case ".xj":
						int ninjaDataOffset;
						bool basicModel = false;

						string magic = System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(BitConverter.ToInt32(file, 0)));

						switch (magic)
						{
							case "GJTL":
							case "NJTL":
								ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0x8);
								modelinfo.checkBoxBigEndian.Checked = ByteConverter.BigEndian;
								ninjaDataOffset = ReadNJTL(file, ref basicModel, ref TempTexList);
								break;
							case "GJCM":
							case "NJCM":
								ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0x8);
								modelinfo.checkBoxBigEndian.Checked = ByteConverter.BigEndian;
								ninjaDataOffset = 0x8;
								break;
							case "NJBM":
								ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0x8);
								modelinfo.checkBoxBigEndian.Checked = ByteConverter.BigEndian;
								ninjaDataOffset = 0x8;
								basicModel = true;
								break;
							default:
								MessageBox.Show("Incorrect format!");
								return;
						}

						// Set modelinfo parameters
						modelinfo.checkBoxBigEndian.Checked = ByteConverter.BigEndian;
						modelinfo.radioButtonObject.Checked = true;
						modelinfo.numericUpDownModelAddress.Value = 0;
						if (basicModel)
						{
							modelinfo.comboBoxModelFormat.SelectedIndex = 0;
						}
						else
						{
							switch (extension)
							{
								case ".gj":
									modelinfo.comboBoxModelFormat.SelectedIndex = 3;
									break;
								case ".nj":
									modelinfo.comboBoxModelFormat.SelectedIndex = 2;
									break;
								case ".xj":
									modelinfo.comboBoxModelFormat.SelectedIndex = 4;
									break;
							}

						}

						modelinfo.numericUpDownMotionAddress.Value = 0;
						modelinfo.checkBoxMemoryObject.Checked = false;
						modelinfo.checkBoxMemoryMotion.Checked = false;

						modelinfo.numericUpDownKey.Value = 0;
						modelinfo.numericUpDownKey.Value = 0;

						// Get rid of the junk so that we can treat it like what SAMDL expects
						byte[] newFile = new byte[file.Length - ninjaDataOffset];
						Array.Copy(file, ninjaDataOffset, newFile, 0, newFile.Length);
						LoadBinFile(newFile);
						animationList = new List<NJS_MOTION>();
						setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled =
							buttonPrevAnimation.Enabled = buttonResetFrame.Enabled = animationList.Count > 0;
						break;
					// Project file
					case ".sap":
						LoadProject(filename);
						return;
					// Import
					case ".obj":
					case ".fbx":
					case ".dae":
						ImportModelFromFile(filename);
						break;
					// Generic binary
					default:
						if (extension.Equals(".prs", StringComparison.OrdinalIgnoreCase))
							file = FraGag.Compression.Prs.Decompress(file);
						ByteConverter.BigEndian = false;
						modelinfo.CheckFilename(filename);
						uint? baseaddr = SplitTools.HelperFunctions.SetupEXE(ref file);
						if (baseaddr.HasValue)
							modelinfo.numericUpDownKey.Value = baseaddr.Value;
						modelinfo.ShowDialog(this);
						if (modelinfo.DialogResult == DialogResult.OK)
						{
							if (modelinfo.radioButtonBinary.Checked)
							{
								if (modelinfo.comboBoxBinaryFileType.SelectedIndex == 2) // REL file
								{
									ByteConverter.BigEndian = true;
									file = SplitTools.HelperFunctions.DecompressREL(file);
									SplitTools.HelperFunctions.FixRELPointers(file, 0xC900000);
								}
								log.Add("Loading model from binary file " + filename);
								log.Add("\tKey: " + uint.Parse(modelinfo.numericUpDownKey.Value.ToString()).ToCHex());
								log.Add("\tAddress: " + uint.Parse(modelinfo.numericUpDownModelAddress.Value.ToString()).ToCHex());
								if (modelinfo.numericUpDownStartOffset.Value != 0)
									log.Add("\tOffset: " + uint.Parse(modelinfo.numericUpDownStartOffset.Value.ToString()).ToCHex());
								if (modelinfo.numericUpDownMotionAddress.Value != 0)
									log.Add("\tMotion at: " + uint.Parse(modelinfo.numericUpDownMotionAddress.Value.ToString()).ToCHex());
								log.Add("\tBig Endian: " + modelinfo.checkBoxBigEndian.Checked.ToString()); ;
								log.WriteLog();
								LoadBinFile(file);
							}
							else
							{
								ModelFormat fmt = outfmt = ModelFormat.Chunk;
								ByteConverter.BigEndian = modelinfo.radioButtonSA2BMDL.Checked;
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
												animationList = new List<NJS_MOTION>(anis.Values);
												setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled =
													buttonPrevAnimation.Enabled = buttonResetFrame.Enabled = animationList.Count > 0;
											}
										}
									}
								}
							}
						}
						else return;
						break;
				}
			}
			currentFileName = filename;

			RebuildModelCache();
			loaded = modelInfoEditorToolStripMenuItem.Enabled = loadAnimationToolStripMenuItem.Enabled = saveMenuItem.Enabled = buttonSave.Enabled = buttonSaveAs.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = renderToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = modelCodeToolStripMenuItem.Enabled = resetLabelsToolStripMenuItem.Enabled = true;
			saveAnimationsToolStripMenuItem.Enabled = (animationList != null && animationList.Count > 0);
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfoCurrent != null;
			showWeightsToolStripMenuItem.Enabled = buttonShowWeights.Enabled = hasWeight;
			importLabelsToolStripMenuItem.Enabled = exportLabelsToolStripMenuItem.Enabled = (loaded && model.GetModelFormat() != ModelFormat.GC);
			if (cmdLoad == false)
			{
				selectedObject = model;
				SelectedItemChanged();
			}
			AddModelToLibrary(model, false);
			unsavedChanges = false;
		}

		private void LoadBinFile(byte[] file)
		{
			// Start offset for X360 exe
			if (modelinfo.numericUpDownStartOffset.Value != 0)
			{
				byte[] datafile_new = new byte[file.Length + (uint)modelinfo.numericUpDownStartOffset.Value];
				file.CopyTo(datafile_new, (int)modelinfo.numericUpDownStartOffset.Value);
				file = datafile_new;
			}
			uint objectaddress = (uint)modelinfo.numericUpDownModelAddress.Value;
			uint motionaddress = (uint)modelinfo.numericUpDownMotionAddress.Value;
			if (modelinfo.checkBoxMemoryObject.Checked) objectaddress -= (uint)modelinfo.numericUpDownKey.Value;
			if (modelinfo.checkBoxMemoryMotion.Checked) motionaddress -= (uint)modelinfo.numericUpDownKey.Value;
			ByteConverter.BigEndian = modelinfo.checkBoxBigEndian.Checked;
			ByteConverter.Reverse = modelinfo.checkBoxReverse.Checked;
			if (modelinfo.radioButtonObject.Checked)
			{
				NJS_OBJECT tempmodel = new NJS_OBJECT(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value, (ModelFormat)modelinfo.comboBoxModelFormat.SelectedIndex, null);
				if (tempmodel.Sibling != null)
				{
					model = new NJS_OBJECT { Name = "Root" };
					model.AddChild(tempmodel);
					model.FixParents();
					rootSiblingMode = true;
				}
				else
				{
					model = tempmodel;
					rootSiblingMode = false;
				}
				if (modelinfo.checkBoxLoadMotion.Checked)
					animationList = new List<NJS_MOTION>() { NJS_MOTION.ReadDirect(file, model.CountAnimated(), (int)motionaddress, (uint)modelinfo.numericUpDownKey.Value, null) };
				setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled =
					buttonResetFrame.Enabled = (animationList != null && animationList.Count > 0);
			}
			else if (modelinfo.radioButtonAction.Checked)
			{
				NJS_ACTION action = new NJS_ACTION(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value, (ModelFormat)modelinfo.comboBoxModelFormat.SelectedIndex, null);
				model = action.Model;
				animationList = new List<NJS_MOTION>() { NJS_MOTION.ReadHeader(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value, (ModelFormat)modelinfo.comboBoxModelFormat.SelectedIndex, null) };
				setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled =
					buttonResetFrame.Enabled = animationList.Count > 0;
			}
			else
			{
				model = new NJS_OBJECT { Name = "Model" };
				switch ((ModelFormat)modelinfo.comboBoxModelFormat.SelectedIndex)
				{
					case ModelFormat.Basic:
						model.Attach = new BasicAttach(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value, false);
						break;
					case ModelFormat.BasicDX:
						model.Attach = new BasicAttach(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value, true);
						break;
					case ModelFormat.Chunk:
						model.Attach = new ChunkAttach(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value);
						break;
					case ModelFormat.GC:
						model.Attach = new GC.GCAttach(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value);
						break;
					case ModelFormat.XJ:
						model.Attach = new XJ.XJAttach(file, (int)objectaddress, (uint)modelinfo.numericUpDownKey.Value);
						break;
				}
			}
			switch ((ModelFormat)modelinfo.comboBoxModelFormat.SelectedIndex)
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
				case ModelFormat.XJ:
					outfmt = ModelFormat.XJ;
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
			if (loaded && unsavedChanges)
			{
				switch (MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "SAMDL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						if (File.Exists(currentFileName))
							Save(currentFileName);
						else
							SaveAs(exportAnimationsToolStripMenuItem.Checked);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
			try
			{
				settingsfile.AlternativeCamera = alternativeCameraModeToolStripMenuItem.Checked;
				settingsfile.Save();
			}
			catch { };
			AnimationTimer.Stop();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(exportAnimationsToolStripMenuItem.Checked);
		}

		private void SaveAs(bool saveAnims = false)
		{
			string filterString;

			switch (outfmt)
			{
				case ModelFormat.XJ:
					filterString = "Sega Xbox Ninja .xj|*.xj";
					break;
				case ModelFormat.GC:
					filterString = "SA2B MDL Files|*.sa2bmdl|Sega GCNinja .gj|*.gj|Sega GCNinja Big Endian (Gamecube) .gj|*.gj";
					break;
				case ModelFormat.Chunk:
					filterString = "SA2 MDL Files|*.sa2mdl|Sega Ninja .nj|*.nj|Sega Ninja Big Endian (Gamecube) .nj|*.nj";
					break;
				case ModelFormat.BasicDX:
				case ModelFormat.Basic:
				default:
					filterString = "SA1 MDL Files|*.sa1mdl|Sega Ninja .nj|*.nj|Sega Ninja Big Endian (Gamecube) .nj|*.nj";
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
					switch (a.FilterIndex)
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
				case ".xj":
					if (extension.Contains("?BE?"))
					{
						bigEndian = true;
					}
					fileName = fileName.Replace("?BE?", "");
					ByteConverter.BigEndian = bigEndian;

					if (animationList != null && saveAnims)
					{
						for (int u = 0; u < animationList.Count; u++)
						{
							string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_anim" + u.ToString() + "_" + animationList[u].Name + ".njm";
							byte[] rawAnim = animationList[u].GetBytes(0, new Dictionary<string, uint>(), out uint address, true);

							File.WriteAllBytes(filePath, rawAnim);
						}
					}

					bool isGC = extension.Contains("gj") ? true : false;
					List<string> texList = new List<string>();
					Dictionary<uint, byte[]> texDict = new Dictionary<uint, byte[]>();
					if (TextureInfoCurrent != null)
					{
						foreach (BMPInfo tex in TextureInfoCurrent)
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
					if (texList.Count != 0)
					{
						texDict.Add(uint.MaxValue, NJTLHelper.GenerateNJTexList(texList.ToArray(), isGC));
					}
					ModelFile.CreateFile(fileName, rootSiblingMode ? model.Children[0] : model, new string[0], modelAuthor, modelDescription, texDict, outfmt, true, true);
					break;
				default:
					string[] animfiles;
					if (animationList != null && saveAnims)
					{
						animfiles = new string[animationList.Count()];

						for (int u = 0; u < animationList.Count; u++)
						{
							string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_anim" + u.ToString() + "_" + animationList[u].Name + ".saanim";
							animationList[u].Save(filePath);
							animfiles[u] = filePath;
						}
					}
					else
						animfiles = null;
					ModelFile.CreateFile(fileName, rootSiblingMode ? model.Children[0] : model, animfiles, modelAuthor, modelDescription, null, outfmt);
					break;
			}

			currentFileName = fileName;
			UpdateStatusString();
			unsavedChanges = false;
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
				SaveAs(exportAnimationsToolStripMenuItem.Checked);
			}
		}

		private void NewFile(ModelFormat modelFormat)
		{
			rootSiblingMode = false;
			loaded = false;
			if (newModelUnloadsTexturesToolStripMenuItem.Checked) UnloadTextures();
			AnimationPlaying = false;
			currentAnimation = null;
			animationList = null;
			animnum = -1;
			animframe = 0;

			outfmt = modelFormat;
			animationList = new List<NJS_MOTION>();

			model = new NJS_OBJECT();
			model.Morph = false;
			RebuildModelCache();
			selectedObject = model;
			setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = buttonPlayAnimation.Enabled = buttonResetFrame.Enabled = false;
			loaded = modelInfoEditorToolStripMenuItem.Enabled = loadAnimationToolStripMenuItem.Enabled = saveMenuItem.Enabled = buttonSave.Enabled = buttonSaveAs.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = renderToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = modelCodeToolStripMenuItem.Enabled = resetLabelsToolStripMenuItem.Enabled = true;
			saveAnimationsToolStripMenuItem.Enabled = false;
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfoCurrent != null;
			SelectedItemChanged();

			currentFileName = "";
			UpdateStatusString();
			unsavedChanges = false;
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
				Text = "SAMDL: " + Path.GetFullPath(currentFileName);
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
			animNameLabel.Text = $"Animation: {currentAnimation?.Name ?? "None"}";
			animDescriptionLabel.Text = currentAnimation?.Description ?? "";
			animDescriptionLabel.BorderSides = animDescriptionLabel.Text == "" ? ToolStripStatusLabelBorderSides.None : ToolStripStatusLabelBorderSides.Left;
			animFrameLabel.Text = $"Frame: {animframe.ToString("0.00")}";
			statusStrip1.Update();
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded || DeviceResizing) return;
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), RenderPanel.Width / (float)RenderPanel.Height, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformState.View, cam.ToMatrix());
			d3ddevice.SetRenderState(RenderState.FillMode, EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderState.CullMode, EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = Color.White.ToRawColor4() };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, EditorOptions.FillColor.ToRawColorBGRA(), 1, 0);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			d3ddevice.BeginScene();

			// All drawings after this line
			EditorOptions.RenderStateCommonSetup(d3ddevice);
			EditorOptions.SetLightType(d3ddevice, EditorOptions.SADXLightTypes.Default);

			MatrixStack transform = new MatrixStack();
			if (showModelToolStripMenuItem.Checked)
			{
				if (hasWeight)
					RenderInfo.Draw(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, buttonTextures.Checked ? Textures : null, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting), d3ddevice, cam);
				else if (currentAnimation != null)
				{
					foreach (KeyValuePair<int, AnimModelData> animdata in currentAnimation.Models)
					{
						if (animdata.Value.Vertex.Count > 0 || animdata.Value.Normal.Count > 0)
						{
							model.ProcessShapeMotionVertexData(currentAnimation, animframe);
							NJS_OBJECT[] models = model.GetObjects();
							meshes = new Mesh[models.Length];
							for (int i = 0; i < models.Length; i++)
								if (models[i].Attach != null)
									try { meshes[i] = models[i].Attach.CreateD3DMesh(); }
									catch { }
						}
					}
					RenderInfo.Draw(model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, buttonTextures.Checked ? Textures : null, meshes, currentAnimation, animframe, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting), d3ddevice, cam);
				}
				else
					RenderInfo.Draw(model.DrawModelTree(EditorOptions.RenderFillMode, transform, buttonTextures.Checked ? Textures : null, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting), d3ddevice, cam);

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

			if (showNodeConnectionsToolStripMenuItem.Checked && model.CountAnimated() > 1)
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
			if (currentAnimation != null && currentAnimation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(currentAnimation.Models[animindex], animframe, transform);
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
			if (currentAnimation != null && currentAnimation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(currentAnimation.Models[animindex], animframe, transform);
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
			if (currentAnimation != null && currentAnimation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(currentAnimation.Models[animindex], animframe, transform);
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
			string saveControlsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SAMDL_keys.ini");
			actionList.Save(saveControlsPath);
			// Settings
			optionsEditor_FormUpdated();
		}

		private void PreviousAnimation()
		{
			if (animationList == null) return;
			animnum--;
			if (animnum == -2) animnum = animationList.Count - 1;
			if (animnum > -1)
				currentAnimation = animationList[animnum];
			else
				currentAnimation = null;
			if (currentAnimation != null)
			{
				osd.UpdateOSDItem("Animation: " + animationList[animnum].Name.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = true;
			}
			else
			{
				osd.UpdateOSDItem("No animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = false;
			}
			animframe = 0;
			NeedRedraw = true;
		}

		private void NextAnimation()
		{
			if (animationList == null) return;
			animnum++;
			if (animnum == animationList.Count) animnum = -1;
			if (animnum > -1)
				currentAnimation = animationList[animnum];
			else
				currentAnimation = null;
			if (currentAnimation != null)
			{
				osd.UpdateOSDItem("Animation: " + animationList[animnum].Name.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = true;
			}
			else
			{
				osd.UpdateOSDItem("No animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = false;
			}
			animframe = 0;
			NeedRedraw = true;
		}

		private void NextFrame()
		{
			if (animationList == null || currentAnimation == null) return;
			animframe = (float)Math.Floor(animframe + 1);
			if (animframe == currentAnimation.Frames) animframe = 0;
			osd.UpdateOSDItem("Animation frame: " + animframe.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			AnimationTimer.Stop();
			AnimationPlaying = buttonPlayAnimation.Checked = false;
			NeedRedraw = true;
		}

		private void PrevFrame()
		{
			if (animationList == null || currentAnimation == null) return;
			animframe = (float)Math.Floor(animframe - 1);
			if (animframe < 0) animframe = currentAnimation.Frames - 1;
			osd.UpdateOSDItem("Animation frame: " + animframe.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			AnimationTimer.Stop();
			AnimationPlaying = buttonPlayAnimation.Checked = false;
			NeedRedraw = true;
		}

		private void ResetFrame()
		{
			if (animationList == null || currentAnimation == null) return;
			animframe = 0;
			osd.UpdateOSDItem("Reset animation frame", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			AnimationTimer.Stop();
			AnimationPlaying = buttonPlayAnimation.Checked = false;
			NeedRedraw = true;
		}

		private void PlayPause()
		{
			if (animationList == null || currentAnimation == null) return;
			AnimationPlaying = !AnimationPlaying;
			buttonPlayAnimation.Checked = AnimationPlaying;
			if (AnimationPlaying)
			{
				osd.UpdateOSDItem("Play animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				AnimationTimer.Start();
			}
			else
			{
				osd.UpdateOSDItem("Stop animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				AnimationTimer.Stop();
			}
			NeedRedraw = true;
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

				case ("Zoom to Target"):
					if (selectedObject != null)
					{
						BoundingSphere bounds = (selectedObject.Attach != null) ?
							new BoundingSphere(selectedObject.Attach.Bounds.Center, selectedObject.Attach.Bounds.Radius) :
							new BoundingSphere(selectedObject.Position.X, selectedObject.Position.Y, selectedObject.Position.Z, 10);
						bounds.Center += selectedObject.Position;
						cam.MoveToShowBounds(bounds);
					}
					else
					{
						BoundingSphere bounds = (model.Attach != null) ?
							new BoundingSphere(model.Attach.Bounds.Center, model.Attach.Bounds.Radius) :
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

				case ("Increase Camera Speed"):
					cam.MoveSpeed += 0.0625f;
					UpdateStatusString();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					settingsfile.CamMoveSpeed = cam.MoveSpeed;
					draw = true;
					break;

				case ("Decrease Camera Speed"):
					cam.MoveSpeed = Math.Max(cam.MoveSpeed - 0.0625f, 0.0625f);
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					UpdateStatusString();
					settingsfile.CamMoveSpeed = cam.MoveSpeed;
					draw = true;
					break;

				case ("Reset Camera Speed"):
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					osd.UpdateOSDItem("Reset camera speed", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					UpdateStatusString();
					settingsfile.CamMoveSpeed = cam.MoveSpeed;
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

				case ("Reset Animation Frame"):
					ResetFrame();
					break;

				case ("Play/Pause Animation"):
					PlayPause();
					break;

				case ("Increase Animation Speed"):
					animspeed += 0.1f;
					settingsfile.AnimSpeed = animspeed;
					osd.UpdateOSDItem("Animation speed: " + animspeed.ToString("0.00"), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 60);
					draw = true;
					break;

				case ("Decrease Animation Speed"):
					animspeed -= 0.1f;
					settingsfile.AnimSpeed = animspeed;
					osd.UpdateOSDItem("Animation speed: " + animspeed.ToString("0.00"), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 60);
					draw = true;
					break;

				case ("Reset Animation Speed"):
					settingsfile.AnimSpeed = animspeed = 1.0f;
					osd.UpdateOSDItem("Reset animation speed", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 60);
					draw = true;
					break;

				case ("Brighten Ambient"):
					EditorOptions.AdjustBackLightBrightness(0.1f);
					EditorOptions.SetLightType(d3ddevice, EditorOptions.SADXLightTypes.Default);
					osd.UpdateOSDItem("Brighten Ambient", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					draw = true;
					break;

				case ("Darken Ambient"):
					EditorOptions.AdjustBackLightBrightness(-0.1f);
					EditorOptions.SetLightType(d3ddevice, EditorOptions.SADXLightTypes.Default);
					draw = true;
					break;
				default:
					break;
			}

			if (draw)
				NeedRedraw = true;
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

				case ("Play Animation (Hold)"):
					animframe += animspeed;
					if (animframe >= currentAnimation.Frames)
						animframe = 0;
					NeedRedraw = true;
					break;

				case ("Play Animation in Reverse (Hold)"):
					animframe -= animspeed;
					if (animframe < 0)
						animframe = currentAnimation.Frames;
					NeedRedraw = true;
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

		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
			EditorCamera.CameraUpdateFlags camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, cameraKeyDown, alternativeCameraModeToolStripMenuItem.Checked);
			if (camresult.HasFlag(EditorCamera.CameraUpdateFlags.Redraw))
				NeedRedraw = true;
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				actionInputCollector.KeyUp(Keys.MButton);
		}
		#endregion

        private void UpdateTexlist()
        {
            if (TextureInfo == null)
            {
                unloadTextureToolStripMenuItem.Enabled = false;
                return;
            }
            if (TexList != null)
            {
                List<Texture> textures = new List<Texture>();
                List<BMPInfo> texinfo = new List<BMPInfo>();
				List<string> dupnames = new List<string>();
                for (int i = 0; i < TexList.TextureNames.Length; i++)
                    for (int j = 0; j < TextureInfo.Length; j++)
                        if (string.IsNullOrEmpty(TexList.TextureNames[i]) || (TexList.TextureNames[i].ToLowerInvariant() == TextureInfo[j].Name.ToLowerInvariant() && !dupnames.Contains(TexList.TextureNames[i].ToLowerInvariant())))
                        {
                            texinfo.Add(TextureInfo[j]);
                            textures.Add(TextureInfo[j].Image.ToTexture(d3ddevice));
							dupnames.Add(TextureInfo[j].Name.ToLowerInvariant());
							continue;
                        }
                Textures = textures.ToArray();
                TextureInfoCurrent = texinfo.ToArray();
            }
            else
            {
                TextureInfoCurrent = new BMPInfo[TextureInfo.Length];
                for (int i = 0; i < TextureInfo.Length; i++)
                    TextureInfoCurrent[i] = TextureInfo[i];
                Textures = new Texture[TextureInfoCurrent.Length];
                for (int j = 0; j < TextureInfoCurrent.Length; j++)
                    Textures[j] = TextureInfoCurrent[j].Image.ToTexture(d3ddevice);
            }
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfoCurrent != null;
		}

		private void LoadTextures(string filename)
		{
			TextureInfo = TextureArchive.GetTextures(filename, out bool hasNames);

			//Use names loaded from model, ex NJTL, if the archive didn't have texture names
			if (hasNames == false && TempTexList?.TextureNames?.Length > 0)
			{
				int tempLen = TempTexList.TextureNames.Length;
				for (int i = 0; i < TextureInfo.Length; i++)
				{
					if (tempLen > i)
					{
						TextureInfo[i].Name = TempTexList.TextureNames[i];
					}
					else
					{
						break;
					}
				}
			}
			TexturePackName = Path.GetFileNameWithoutExtension(filename);
			TempTexList = null;
			TexList = null;
			UpdateTexlist();
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = loaded;
			if (loaded)
				NeedRedraw = true;
		}

		private void loadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog() { Title = "Load Textures", DefaultExt = "pvm", Filter = "Texture Archives|*.pvm;*.gvm;*.xvm;*.prs;*.pvmx;*.pb;*.pak|Texture Pack|*.txt|Supported Files|*.pvm;*.gvm;*.xvm;*.prs;*.pvmx;*.pb;*.pak;*.txt|All Files|*.*" })
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					LoadTextures(a.FileName);
				}
			}
		}

		private void cStructsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "c", Filter = "C Files|*.c" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					bool dx = false;
					if (outfmt == ModelFormat.Basic)
						dx = MessageBox.Show(this, "Do you want to export in Basic+ format?", "SAMDL", MessageBoxButtons.YesNo) == DialogResult.Yes;
					List<string> labels = new List<string>() { model.Name };
					using (StreamWriter sw = File.CreateText(sd.FileName))
					{
						sw.Write("/* NINJA ");
						switch (outfmt)
						{
							case ModelFormat.Basic:
							case ModelFormat.BasicDX:
								if (dx)
									sw.Write("Basic+ (with Sonic Adventure DX PC additions)");
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
						if (!string.IsNullOrEmpty(modelDescription))
						{
							sw.Write(" * Description: ");
							sw.WriteLine(modelDescription);
							sw.WriteLine(" * ");
						}
						if (!string.IsNullOrEmpty(modelAuthor))
						{
							sw.Write(" * Author: ");
							sw.WriteLine(modelAuthor);
							sw.WriteLine(" * ");
						}
						sw.WriteLine(" */");
						sw.WriteLine();
						string[] texnames = null;
						if (TexturePackName != null && exportTextureNamesToolStripMenuItem.Checked)
						{
							texnames = new string[TextureInfoCurrent.Length];
							for (int i = 0; i < TextureInfoCurrent.Length; i++)
								texnames[i] = string.Format("{0}TexName_{1}", TexturePackName, TextureInfoCurrent[i].Name);
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
						if (exportAnimationsToolStripMenuItem.Checked && animationList != null)
						{
							foreach (NJS_MOTION anim in animationList)
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
				else if (currentAnimation != null)
					dist = model.CheckHitAnimated(Near, Far, viewport, proj, view, new MatrixStack(), meshes, currentAnimation, animframe);
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
				contextMenuStripRightClick.Show(RenderPanel, e.Location);
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
				case ModelFormat.XJ:
					return typeof(XJ.XJAttach);
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
				editModelDataToolStripMenuItem.Enabled = true;
				deleteNodeToolStripMenuItem.Enabled = selectedObject.Parent != null;
				emptyModelDataToolStripMenuItem.Enabled = selectedObject.Attach != null;
				splitToolStripMenuItem.Enabled = selectedObject.Attach != null;
				sortToolStripMenuItem.Enabled = true;
				PolyNormalstoolStripMenuItem.Enabled = outfmt == ModelFormat.Basic;
				exportContextMenuItem.Enabled = selectedObject.Attach != null;
				importContextMenuItem.Enabled = true;
				hierarchyToolStripMenuItem.Enabled = selectedObject.Children != null && selectedObject.Children.Count > 0;
				moveDownToolStripMenuItem.Enabled = selectedObject.Parent != null && selectedObject.Parent.Children.IndexOf(selectedObject) < selectedObject.Parent.Children.Count - 1;
				moveUpToolStripMenuItem.Enabled = selectedObject.Parent != null && selectedObject.Parent.Children.IndexOf(selectedObject) > 0;
			}
			else
			{
				treeView1.SelectedNode = null;
				suppressTreeEvent = false;
				propertyGrid1.SelectedObject = null;
				copyModelToolStripMenuItem.Enabled = false;
				pasteModelToolStripMenuItem.Enabled = Clipboard.ContainsData(GetAttachType().AssemblyQualifiedName);
				addChildToolStripMenuItem.Enabled = false;
				editModelDataToolStripMenuItem.Enabled = false;
				PolyNormalstoolStripMenuItem.Enabled = editMaterialsToolStripMenuItem.Enabled = materialEditorToolStripMenuItem.Enabled = false;
				splitToolStripMenuItem.Enabled = sortToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = clearChildrenToolStripMenuItem1.Enabled = deleteNodeToolStripMenuItem.Enabled = emptyModelDataToolStripMenuItem.Enabled = false;
				importContextMenuItem.Enabled = false;
				exportContextMenuItem.Enabled = false;
				moveDownToolStripMenuItem.Enabled = moveUpToolStripMenuItem.Enabled = false;
			}
			if (showWeightsToolStripMenuItem.Checked && hasWeight)
				model.UpdateWeightedModelSelection(selectedObject, meshes);

			NeedRedraw = true;
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
			else
				attach.Name = "attach_" + Extensions.GenerateIdentifier();
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
				case GC.GCAttach gatt:
					gatt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
					gatt.OpaqueMeshName = "opoly_" + Extensions.GenerateIdentifier();
					gatt.TranslucentMeshName = "tpoly_" + Extensions.GenerateIdentifier();
					break;
			}
			selectedObject.Attach = attach;
			RebuildModelCache();
			NeedRedraw = true;
			unsavedChanges = true;
		}

		private void editMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (selectedObject.Attach)
			{
				case GC.GCAttach:
					OpenGCMaterialEditor();
					break;
				case BasicAttach:
				case ChunkAttach:
					OpenMaterialEditor();
					break;
			}
		}

		private void UpdateMaterials(List<NJS_MATERIAL> mats)
		{
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
			}
			RebuildModelCache();
			NeedRedraw = true;
			SelectedItemChanged();
		}

		private void OpenMaterialEditor()
		{
			List<NJS_MATERIAL> mats;
			string matname = null;
			switch (selectedObject.Attach)
			{
				case BasicAttach bscatt:
					mats = bscatt.Material;
					matname = bscatt.MaterialName;
					break;
				default:
					mats = selectedObject.Attach.MeshInfo.Select(a => a.Material).ToList();
					break;
			}
			using (MaterialEditor dlg = new MaterialEditor(mats, TextureInfoCurrent, matname))
			{
				dlg.FormUpdated += (s, ev) => UpdateMaterials(mats);
				dlg.ShowDialog(this);
			}
			unsavedChanges = true;
		}

		private void OpenGCMaterialEditor()
		{
			List<NJS_MATERIAL> mats;
			string matname = null;
			mats = selectedObject.Attach.MeshInfo.Select(a => a.Material).ToList();
			using (GCMaterialEditor dlg = new GCMaterialEditor(mats, TextureInfoCurrent, matname))
			{
				dlg.FormUpdated += (s, ev) => NeedRedraw = true;
				dlg.ShowDialog(this);
			}
			switch (selectedObject.Attach)
			{
				case GC.GCAttach gcatt:
					int matind = 0;
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
			unsavedChanges = true;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "Name")
			{
				NJS_OBJECT[] list = model.GetObjects();
				int index = 0;
				for (int i = 0; i < list.Length; i++)
				{
					if (selectedObject == list[i])
					{
						index = i;
					}
					else if (list[i].Name == (string)e.ChangedItem.Value)
					{
						MessageBox.Show(this, "There already exists a model with the label '" + (string)e.ChangedItem.Value + "'.\nRenaming to avoid duplicates.", "SAMDL Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						selectedObject.Name = (string)e.ChangedItem.Value + "_" + Extensions.GenerateIdentifier();
						propertyGrid1.Refresh();
					}
				}
				nodeDict[selectedObject].Text = $"{index}: {selectedObject.Name}";
			}
			NeedRedraw = true;
			unsavedChanges = true;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (suppressTreeEvent) return;
			selectedObject = (NJS_OBJECT)e.Node.Tag;
			SelectedItemChanged();
		}

		private void primitiveRenderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NeedRedraw = true;
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
		}

		void optionsEditor_FormUpdated()
		{
			settingsfile.DrawDistanceGeneral = EditorOptions.RenderDrawDistance;
			settingsfile.CameraModifier = cam.ModifierKey;
			settingsfile.BackgroundColor = EditorOptions.FillColor;
			settingsfile.EnableSpecular = EditorOptions.EnableSpecular;
			// Key Light
			settingsfile.KeyLightDirection = new Vertex(EditorOptions.KeyLight.Direction.X, EditorOptions.KeyLight.Direction.Y, EditorOptions.KeyLight.Direction.Z);
			settingsfile.KeyLightAmbient = Color.FromArgb((int)(EditorOptions.KeyLight.Ambient.R * 255.0f), (int)(EditorOptions.KeyLight.Ambient.G * 255.0f), (int)(EditorOptions.KeyLight.Ambient.B * 255.0f));
			settingsfile.KeyLightDiffuse = Color.FromArgb((int)(EditorOptions.KeyLight.Diffuse.R * 255.0f), (int)(EditorOptions.KeyLight.Diffuse.G * 255.0f), (int)(EditorOptions.KeyLight.Diffuse.B * 255.0f));
			settingsfile.KeyLightSpecular = Color.FromArgb((int)(EditorOptions.KeyLight.Specular.R * 255.0f), (int)(EditorOptions.KeyLight.Specular.G * 255.0f), (int)(EditorOptions.KeyLight.Specular.B * 255.0f));
			// Fill Light
			settingsfile.FillLightDirection = new Vertex(EditorOptions.FillLight.Direction.X, EditorOptions.FillLight.Direction.Y, EditorOptions.FillLight.Direction.Z);
			settingsfile.FillLightAmbient = Color.FromArgb((int)(EditorOptions.FillLight.Ambient.R * 255.0f), (int)(EditorOptions.FillLight.Ambient.G * 255.0f), (int)(EditorOptions.FillLight.Ambient.B * 255.0f));
			settingsfile.FillLightDiffuse = Color.FromArgb((int)(EditorOptions.FillLight.Diffuse.R * 255.0f), (int)(EditorOptions.FillLight.Diffuse.G * 255.0f), (int)(EditorOptions.FillLight.Diffuse.B * 255.0f));
			settingsfile.FillLightSpecular = Color.FromArgb((int)(EditorOptions.FillLight.Specular.R * 255.0f), (int)(EditorOptions.FillLight.Specular.G * 255.0f), (int)(EditorOptions.FillLight.Specular.B * 255.0f));
			// Back Light
			settingsfile.BackLightDirection = new Vertex(EditorOptions.BackLight.Direction.X, EditorOptions.BackLight.Direction.Y, EditorOptions.BackLight.Direction.Z);
			settingsfile.BackLightAmbient = Color.FromArgb((int)(EditorOptions.BackLight.Ambient.R * 255.0f), (int)(EditorOptions.BackLight.Ambient.G * 255.0f), (int)(EditorOptions.BackLight.Ambient.B * 255.0f));
			settingsfile.BackLightDiffuse = Color.FromArgb((int)(EditorOptions.BackLight.Diffuse.R * 255.0f), (int)(EditorOptions.BackLight.Diffuse.G * 255.0f), (int)(EditorOptions.BackLight.Diffuse.B * 255.0f));
			settingsfile.BackLightSpecular = Color.FromArgb((int)(EditorOptions.BackLight.Specular.R * 255.0f), (int)(EditorOptions.BackLight.Specular.G * 255.0f), (int)(EditorOptions.BackLight.Specular.B * 255.0f));
			NeedRedraw = true;
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
			using (TextureRemappingDialog dlg = new TextureRemappingDialog(TextureInfoCurrent))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					unsavedChanges = true;
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
						meshes = model.ProcessWeightedModel().ToArray();
					else
						model.ProcessVertexData();

					NeedRedraw = true;
				}
		}

		private void showModelToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			string showmodel = "Off";
			if (showModelToolStripMenuItem.Checked) showmodel = "On";
			osd.UpdateOSDItem("Show model: " + showmodel, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
		}

		private void showNodesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			string shownodes = "Off";
			if (showNodesToolStripMenuItem.Checked) shownodes = "On";
			osd.UpdateOSDItem("Show nodes: " + shownodes, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			buttonShowNodes.Checked = showNodesToolStripMenuItem.Checked;
			NeedRedraw = true;
		}

		private void nJAToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string outfn = !string.IsNullOrEmpty(currentFileName) ? Path.ChangeExtension(Path.GetFileName(currentFileName), ".nja") : "model.nja";
			using (SaveFileDialog sd = new SaveFileDialog() { FileName = outfn, DefaultExt = "nja", Filter = "Ninja Ascii Files|*.nja" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					bool dx = false;
					List<string> labels = new List<string>() { model.Name };
					using (StreamWriter sw = File.CreateText(sd.FileName))
					{
						if (TexList != null)
							TexList.ToNJA(sw, labels);
						else if (TexturePackName != null)
						{
							string[] texnames = new string[TextureInfoCurrent.Length];
							for (int i = 0; i < TextureInfoCurrent.Length; i++)
								texnames[i] = string.Format("{0}", TextureInfoCurrent[i].Name);
							NJS_TEXLIST tls = new NJS_TEXLIST(texnames);
							tls.Name = "texlist_" + TexturePackName;
							tls.TexnameArrayName = "textures_" + TexturePackName;
							tls.ToNJA(sw, labels);
						}
						if (rootSiblingMode)
							model.Children[0].ToNJA(sw, labels, labels.ToArray());
						else
							model.ToNJA(sw, labels, labels.ToArray());
						if (exportAnimationsToolStripMenuItem.Checked && animationList != null)
						{
							foreach (NJS_MOTION anim in animationList)
							{
								anim.ToNJA(sw, labels);
							}
						}
					}
				}
		}

		private void ImportModel_Assimp(string objFileName, bool importAsSingle, bool selected = false, bool importColladaRoot = false)
		{
			string rootPath = Path.GetDirectoryName(objFileName);
			string texlistFilename = Path.Combine(rootPath, Path.GetFileNameWithoutExtension(objFileName) + ".satex");
			// Load textures if there is a texlist file
			if (File.Exists(texlistFilename))
			{
				UnloadTextures();
				List<string> texturesPngFilenames = new List<string>();
				TexList = NJS_TEXLIST.Load(texlistFilename);
				foreach (string texture in TexList.TextureNames)
				{
					texturesPngFilenames.Add(Path.Combine(rootPath, Path.ChangeExtension(texture, ".png")));
				}
				AddTextures(texturesPngFilenames.ToArray());
				UpdateTexlist();
			}
			if (TextureInfoCurrent == null)
			{
				using (OpenFileDialog a = new OpenFileDialog() { Title = "Load Textures", DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs" })
				{
					if (a.ShowDialog() == DialogResult.OK)
					{
						LoadTextures(a.FileName);
						NeedRedraw = true;
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
			AnimationPlaying = false;
			// Collada adds a root node, so use the first child node instead
			if (!importColladaRoot)
				importnode = scene.RootNode.Children[0];
			NJS_OBJECT newmodel = SAEditorCommon.Import.AssimpStuff.AssimpImport(scene, importnode, outfmt, TextureInfoCurrent?.Select(t => t.Name).ToArray(), importAsSingle);
			if (!selected)
			{
				currentAnimation = null;
				animationList = null;
				animnum = -1;
				animframe = 0;
				animationList = new List<NJS_MOTION>();
				model = newmodel;
			}
			else
			{
				if (importAsSingle)
					selectedObject.Attach = newmodel.Attach;
				else
				{
					selectedObject.AddChild(newmodel);
					model.FixParents();
				}
			}

			editMaterialsToolStripMenuItem.Enabled = materialEditorToolStripMenuItem.Enabled = true;
			showWeightsToolStripMenuItem.Enabled = buttonShowWeights.Enabled = hasWeight;

			if (!selected)
			{
				if (animationList.Count > 0) setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = buttonResetFrame.Enabled = true;
				selectedObject = model;
				AddModelToLibrary(model, false);
			}
			RebuildModelCache();
			unsavedChanges = true;
			loaded = modelInfoEditorToolStripMenuItem.Enabled = loadAnimationToolStripMenuItem.Enabled = saveMenuItem.Enabled = buttonSave.Enabled = buttonSaveAs.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = renderToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = modelCodeToolStripMenuItem.Enabled = resetLabelsToolStripMenuItem.Enabled = true;
			unloadTextureToolStripMenuItem.Enabled = textureRemappingToolStripMenuItem.Enabled = TextureInfoCurrent != null;
			saveAnimationsToolStripMenuItem.Enabled = animationList.Count > 0;
			SelectedItemChanged();
			NeedRedraw = true;
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
			NeedRedraw = true;
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
			if (TextureInfoCurrent != null)
			{
				// Save textures
				List<string> textureNames = new List<string>();
				foreach (BMPInfo bmp in TextureInfoCurrent)
				{
					textureNames.Add(bmp.Name);
					string savePath = Path.Combine(rootPath, bmp.Name + ".png");
					texturePaths.Add(savePath);
					if (!File.Exists(savePath))
						bmp.Image.Save(savePath);
				}
				// Save texture list
				NJS_TEXLIST textureNamesArray = TexList != null ? TexList : new NJS_TEXLIST(textureNames.ToArray());
				textureNamesArray.Save(Path.Combine(rootPath, Path.GetFileNameWithoutExtension(filename) + ".satex"));
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
			if (TextureInfoCurrent == null)
				if (MessageBox.Show(this, "No textures are loaded. Materials may not be exported properly. Continue?", "SAMDL Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
					return;
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
			using (OpenFileDialog ofd = new OpenFileDialog()
			{
				Filter = "All Animation Files|*.action;*.saanim;*.json;*MTN.BIN;*MTN.PRS;*.njm;*.motions|SA Tools Animation Files|*.saanim;*.action|" +
																		"Ninja Motion Files|*.njm|JSON Files|*.json|Motion Files|*MTN.BIN;*MTN.PRS|All Files|*.*",
				Multiselect = true
			})
				if (ofd.ShowDialog(this) == DialogResult.OK)
				{
					LoadAnimation(ofd.FileNames);
				}
		}

		private void SetAnimOrientations(Vertex position, Rotation rotation, Vertex scale)
		{
			for (int i = 0; i < animationList.Count; i++)
			{
				var anim = animationList[i];
				int endFrame = anim.Frames -1;
				if (anim.Models.ContainsKey(0))
				{
					AnimModelData animData = anim.Models[0];
					if (position != null)
					{
						if (animData.Position.Count > 0)
						{
							foreach (var key in animData.Position.Keys)
							{
								animData.Position[key] += position;
							}
						}
						else
						{
							animData.Position.Add(0, position);
							animData.Position.Add(endFrame, position);
						}
					}
					if (rotation != null)
					{
						if (animData.Rotation.Count > 0)
						{
							foreach (var key in animData.Position.Keys)
							{
								int X = BAMSFix(animData.Rotation[key].X + rotation.X);
								int Y = BAMSFix(animData.Rotation[key].Y + rotation.Y);
								int Z = BAMSFix(animData.Rotation[key].Z + rotation.Z);

								animData.Rotation[key].X = X;
								animData.Rotation[key].Y = Y;
								animData.Rotation[key].Z = Z;
							}
						}
						else
						{
							animData.Rotation.Add(0, rotation);
							animData.Rotation.Add(endFrame, rotation);
						}
					}
					if (scale != null)
					{
						if (animData.Scale.Count > 0)
						{
							foreach (var key in animData.Scale.Keys)
							{
								var oldScale = animData.Scale[key];
								animData.Scale[key] = new Vertex(oldScale.X * scale.X, oldScale.Y * scale.Y, oldScale.Z * scale.Z);
							}
						}
						else
						{
							animData.Scale.Add(0, scale);
							animData.Scale.Add(endFrame, scale);
						}
					}
				}
				else
				{
					AnimModelData orientationData = new AnimModelData();

					if (position != null)
					{
						orientationData.Position.Add(0, position);
						orientationData.Position.Add(endFrame, position);
					}

					if (rotation != null)
					{
						orientationData.Rotation.Add(0, rotation);
						orientationData.Rotation.Add(endFrame, rotation);
					}

					if (scale != null)
					{
						orientationData.Scale.Add(0, scale);
						orientationData.Scale.Add(endFrame, scale);
					}

					anim.Models.Add(0, orientationData);
				}
			}
		}

		private int BAMSFix(int value)
		{
			while (value < 65536)
			{
				value += 65336;
			}
			while (value > 65336)
			{
				value -= 65336;
			}

			return value;
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
						string labelm = Path.ChangeExtension(fn, ".salabel");
						if (File.Exists(labelm))
							LabelMOTION.Load(labelm).Apply(anim);
						if (first)
						{
							first = false;
							animframe = 0;
							animnum = animationList.Count;
							animationList.Add(anim);
							currentAnimation = anim;
							NeedRedraw = true;
						}
						else
							animationList.Add(anim);
						break;
					case ".njm":
						byte[] njmFile = File.ReadAllBytes(fn);
						ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(njmFile, 0xC);

						byte[] newFile = new byte[njmFile.Length - 0x8];
						Array.Copy(njmFile, 0x8, newFile, 0, newFile.Length);

						string njmName = Path.GetFileNameWithoutExtension(fn);
						Dictionary<int, string> label = new Dictionary<int, string>();
						label.Add(0, njmName);
						NJS_MOTION njm = new NJS_MOTION(newFile, 0, 0, model.CountAnimated(), label, false);
						if (first)
						{
							first = false;
							animframe = 0;
							animnum = animationList.Count;
							animationList.Add(njm);
							currentAnimation = njm;
							NeedRedraw = true;
						}
						else
							animationList.Add(njm);
						break;
					case ".bin":
					case ".prs":
						byte[] anifile = File.ReadAllBytes(fn);
						Dictionary<int, int> processedanims = new Dictionary<int, int>();

						if (extension.Equals(".prs", StringComparison.OrdinalIgnoreCase))
							anifile = FraGag.Compression.Prs.Decompress(anifile);

						if (BitConverter.ToInt16(anifile, 0) != 0)
							ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt16(anifile, 0);
						else
							ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt16(anifile, 8);

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
									animationList = new List<NJS_MOTION>();
									animationList.Add(mtn);

									animnum = animationList.Count;
									currentAnimation = animationList[0];
									NeedRedraw = true;
								}
								else
								{
									animationList.Add(mtn);
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
									string filePath = Path.Combine(path, animationFiles[u]);
									string animExt = Path.GetExtension(filePath).ToLowerInvariant();
									NJS_MOTION mot;
									switch (animExt)
									{
										case ".json":
											JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
											using (TextReader tr2 = File.OpenText(filePath))
											{
												JsonTextReader jtr = new JsonTextReader(tr2);
												mot = js.Deserialize<NJS_MOTION>(jtr);
												if (first)
												{
													first = false;
													animframe = 0;
													animnum = animationList.Count;
													animationList.Add(mot);
													currentAnimation = mot;
													NeedRedraw = true;
												}
												else
													animationList.Add(mot);
											}
											break;

										case ".njm":
											byte[] motFile = File.ReadAllBytes(filePath);
											ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(motFile, 0xC);

											byte[] newMotFile = new byte[motFile.Length - 0x8];
											Array.Copy(motFile, 0x8, newMotFile, 0, newMotFile.Length);

											string motName = Path.GetFileNameWithoutExtension(filePath);
											Dictionary<int, string> motLabel = new Dictionary<int, string>();
											motLabel.Add(0, motName);
											mot = new NJS_MOTION(newMotFile, 0, 0, model.CountAnimated(), motLabel, false);
											if (first)
											{
												first = false;
												animframe = 0;
												animnum = animationList.Count;
												animationList.Add(mot);
												currentAnimation = mot;
												NeedRedraw = true;
											}
											else
												animationList.Add(mot);
											break;

										case ".saanim":
										default:
											mot = NJS_MOTION.Load(filePath);
											string labelmo = Path.ChangeExtension(filePath, ".salabel");
											if (File.Exists(labelmo))
												LabelMOTION.Load(labelmo).Apply(mot);
											if (first)
											{
												first = false;
												animframe = 0;
												animnum = animationList.Count;
												animationList.Add(mot);
												currentAnimation = mot;
												NeedRedraw = true;
											}
											else
												animationList.Add(mot);
											break;
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
								animnum = animationList.Count;
								animationList.Add(mot);
								currentAnimation = mot;
								NeedRedraw = true;
							}
							else
								animationList.Add(mot);
						}
						break;
				}

				if (animationList.Count > 0) setDefaultAnimationOrientationToolStripMenuItem.Enabled = buttonNextFrame.Enabled = buttonPrevFrame.Enabled = buttonNextAnimation.Enabled = buttonPrevAnimation.Enabled = buttonResetFrame.Enabled = true;

				//Play our animation in the viewport after loading it. To make sure this will work, we need to disable and reenable it.
				if (animationList == null || currentAnimation == null) return;
				AnimationPlaying = false;
				buttonPlayAnimation.Checked = false;
				osd.UpdateOSDItem("Play animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayAnimation.Enabled = AnimationPlaying = buttonPlayAnimation.Checked = true;
				AnimationTimer.Start();
				NeedRedraw = true;
			}
			saveAnimationsToolStripMenuItem.Enabled = animationList.Count > 0;
		}

		private void welcomeTutorialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowWelcomeScreen();
		}

		private void UnloadTextures()
		{
			TextureInfo = null;
			TextureInfoCurrent = null;
			Textures = null;
			TexList = null;
			unloadTextureToolStripMenuItem.Enabled = false;
		}

		private void unloadTextureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UnloadTextures();
			NeedRedraw = true;
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
				}
			}
			if (obj.Children.Count > 0)
			{
				foreach (NJS_OBJECT child in obj.Children)
				{
					SwapUV(child);
				}
			}
		}

		private void swapUVToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (model != null) SwapUV(model);
			RebuildModelCache();
			NeedRedraw = true;
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
					unsavedChanges = true;
					osd.UpdateOSDItem("Model deleted", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					NeedRedraw = true;
				}
			}
			else
				osd.UpdateOSDItem("Cannot delete root node", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
		}

		private void RebuildModelCache()
		{
			CheckModelLabels();
			model.ProcessVertexData();
            if (hasWeight = model.HasWeight)
                meshes = model.ProcessWeightedModel().ToArray();
            else
            {
                NJS_OBJECT[] models = model.GetObjects();
                meshes = new Mesh[models.Length];
                for (int i = 0; i < models.Length; i++)
                    if (models[i].Attach != null)
                        try { meshes[i] = models[i].Attach.CreateD3DMesh(); }
                        catch { }
            }
			treeView1.BeginUpdate();
			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
			AddTreeNode(model, treeView1.Nodes);
			treeView1.EndUpdate();
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
					NeedRedraw = true;
					SelectedItemChanged();
					unsavedChanges = true;
				}
			}
		}

		private void addChildToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (selectedObject != null)
			{
				selectedObject.AddChild(new NJS_OBJECT());
				selectedObject.FixSiblings();
				selectedObject.FixParents();
				RebuildModelCache();
				NeedRedraw = true;
				SelectedItemChanged();
				unsavedChanges = true;
			}
		}

		private void clearChildrenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearChildren();
			NeedRedraw = true;
		}

		private void MessageTimer_Tick(object sender, EventArgs e)
		{
			if (osd != null && osd.UpdateTimer()) NeedRedraw = true;
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
			NeedRedraw = true;
		}

		private void buttonVertices_Click(object sender, EventArgs e)
		{
			EditorOptions.RenderFillMode = FillMode.Point;
			buttonSolid.Checked = false;
			buttonVertices.Checked = true;
			buttonWireframe.Checked = false;
			osd.UpdateOSDItem("Render mode: Point", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
		}

		private void buttonWireframe_Click(object sender, EventArgs e)
		{
			EditorOptions.RenderFillMode = FillMode.Wireframe;
			buttonSolid.Checked = false;
			buttonVertices.Checked = false;
			buttonWireframe.Checked = true;
			osd.UpdateOSDItem("Render mode: Wireframe", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
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

		private void buttonResetFrame_Click(object sender, EventArgs e)
		{
			ResetFrame();
		}

		private void showHintsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			osd.show_osd = !osd.show_osd;
			buttonShowHints.Checked = showHintsToolStripMenuItem.Checked;
			NeedRedraw = true;
		}

		private void buttonLighting_Click(object sender, EventArgs e)
		{
			string lighting = "On";
			EditorOptions.OverrideLighting = !EditorOptions.OverrideLighting;
			buttonLighting.Checked = !EditorOptions.OverrideLighting;
			if (EditorOptions.OverrideLighting) lighting = "Off";
			osd.UpdateOSDItem("Lighting: " + lighting, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
		}

		private void materialEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (selectedObject.Attach)
			{
				case GC.GCAttach:
					OpenGCMaterialEditor();
					break;
				case BasicAttach:
				case ChunkAttach:
					OpenMaterialEditor();
					break;
			}
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
					switch (a.FilterIndex)
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
			if (animationList != null)
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
						for (int u = 0; u < animationList.Count; u++)
						{
							string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_" + u.ToString() + "_" + animationList[u].Name + ".njm";
							byte[] rawAnim = animationList[u].GetBytes(0xC, new Dictionary<string, uint>(), out uint address, true);

							File.WriteAllBytes(filePath, rawAnim);
						}
						break;
					case ".saanim":
					case ".json":
						using (TextWriter twmain = File.CreateText(Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".action")))
						{
							string[] animfiles = new string[animationList.Count()];
							for (int u = 0; u < animationList.Count; u++)
							{
								string filePath = Path.GetDirectoryName(fileName) + @"\" + Path.GetFileNameWithoutExtension(fileName) + "_" + u.ToString() + "_" + animationList[u].Name + extension;
								if (extension == ".saanim")
									animationList[u].Save(filePath);
								else
								{
									JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
									using (TextWriter tw = File.CreateText(filePath))
									using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
										js.Serialize(jtw, animationList[u]);
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

		private void buttonMaterialColors_CheckedChanged(object sender, EventArgs e)
		{
			string showmatcolors = "On";
			EditorOptions.IgnoreMaterialColors = !buttonMaterialColors.Checked;
			if (EditorOptions.IgnoreMaterialColors) showmatcolors = "Off";
			osd.UpdateOSDItem("Material Colors: " + showmatcolors, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
		}

		private void modelCodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ModelText text = new ModelText();
			string[] texnames = null;
			if (TexturePackName != null && exportTextureNamesToolStripMenuItem.Checked)
			{
				texnames = new string[TextureInfoCurrent.Length];
				for (int i = 0; i < TextureInfoCurrent.Length; i++)
					texnames[i] = string.Format("{0}TexName_{1}", TexturePackName, TextureInfoCurrent[i].Name);
				text.export += "enum " + TexturePackName + "TexName";
				text.export += System.Environment.NewLine;
				text.export += "{";
				text.export += "\t" + string.Join("," + Environment.NewLine + "\t", texnames);
				text.export += System.Environment.NewLine;
				text.export += "};";
				text.export += System.Environment.NewLine;
				text.export += System.Environment.NewLine;
			}
			List<string> labels = new List<string>() { model.Name };
			text.export += model.ToStructVariables(true, labels, texnames); //Need to make the DX thing a toggle
			if (exportAnimationsToolStripMenuItem.Checked && animationList != null)
			{
				foreach (NJS_MOTION anim in animationList)
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
			NeedRedraw = true;
		}

		private void clearChildrenToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ClearChildren();
			NeedRedraw = true;
		}

		private void emptyModelDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("This will delete all meshes in the selected node. Continue?", "Are you sure?", MessageBoxButtons.YesNo);
			if (dialogResult == DialogResult.Yes)
			{
				selectedObject.Attach = null;
				RebuildModelCache();
				NeedRedraw = true;
				SelectedItemChanged();
				unsavedChanges = true;
			}
		}

		private void deleteTheWholeHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteSelectedModel();
			NeedRedraw = true;
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
				NeedRedraw = true;
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
			osd.UpdateOSDItem("Direct3D device reset", RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
			NeedRedraw = true;
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
			bool transonly = transparentOnlyToolStripMenuItem.Checked;
			bool hasopaque = false;
			// Check if the model has opaque meshes - if it doesn't, sort all meshes
			foreach (NJS_MESHSET ms in basicatt.Mesh)
				if (basicatt.Material[ms.MaterialID].UseAlpha == false)
					hasopaque = true;
			if (!hasopaque)
				transonly = false;
			// Add opaque meshes to a child model
			if (transonly)
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
				id++;
			}
			// Add meshes to a child model (or both opaque and transparent if the option is off)
			foreach (NJS_MESHSET m in basicatt.Mesh)
			{
				if (transonly && !basicatt.Material[m.MaterialID].UseAlpha)
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
			if (transonly)
				selectedObject.Attach = opaqueatt;
			else
				selectedObject.Attach = null;
			selectedObject.FixSiblings();
			selectedObject.FixParents();
			RebuildModelCache();
			NeedRedraw = true;
			SelectedItemChanged();
			unsavedChanges = true;
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
					NeedRedraw = true;
					unsavedChanges = true;
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
			NeedRedraw = true;
		}

		private void buttonShowVertexIndices_Click(object sender, EventArgs e)
		{
			showVertexIndicesToolStripMenuItem.Checked = !showVertexIndicesToolStripMenuItem.Checked;
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

		private void ImportOBJLegacy(NJS_OBJECT obj, string filename)
		{
			obj.Attach = SAModel.Direct3D.Extensions.obj2nj(filename, TextureInfoCurrent != null ? TextureInfoCurrent?.Select(a => a.Name).ToArray() : null);
			RebuildModelCache();
			NeedRedraw = true;
		}

		private void legacyOBJImportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "obj",
				Filter = "Wavefront OBJ Files|*.obj|All Files|*.*"
			})
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ImportOBJLegacy(model, ofd.FileName);
				}
			}
		}

		private void selectedLegacyOBJImportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "obj",
				Filter = "Wavefront OBJ Files|*.obj|All Files|*.*"
			})
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ImportOBJLegacy(selectedObject, ofd.FileName);
				}
			}
		}

		private string FixLabel(string label, List<string> labels, out string duplicate)
		{
			duplicate = null;
			if (string.IsNullOrEmpty(label))
				return label;
			string result = label;
			if (labels.Contains(result))
			{
				duplicate = result;
				do
					result += "_";
				while (labels.Contains(result));
			}
			labels.Add(result);
			return result;
		}

		private void CheckLabels(NJS_OBJECT obj, List<string> duplicateLabels)
		{
			List<string> checkingLabels = new List<string>();
			string dup;

			obj.Name = FixLabel(obj.Name, checkingLabels, out dup);
			if (!string.IsNullOrEmpty(dup))
				duplicateLabels.Add(dup);

			if (obj.Attach != null)
			{

				obj.Attach.Name = FixLabel(obj.Attach.Name, checkingLabels, out dup);
				if (!string.IsNullOrEmpty(dup))
					duplicateLabels.Add(dup);

				switch (obj.Attach)
				{
					case BasicAttach:
						{
							BasicAttach basicatt = (BasicAttach)obj.Attach;

							basicatt.MaterialName = FixLabel(basicatt.MaterialName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							basicatt.MeshName = FixLabel(basicatt.MeshName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							basicatt.NormalName = FixLabel(basicatt.NormalName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							basicatt.VertexName = FixLabel(basicatt.VertexName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							if (basicatt.Mesh != null && basicatt.Mesh.Count > 0)
								foreach (NJS_MESHSET meshset in basicatt.Mesh)
								{
									meshset.PolyName = FixLabel(meshset.PolyName, checkingLabels, out dup);
									if (!string.IsNullOrEmpty(dup))
										duplicateLabels.Add(dup);

									meshset.UVName = FixLabel(meshset.UVName, checkingLabels, out dup);
									if (!string.IsNullOrEmpty(dup))
										duplicateLabels.Add(dup);

									meshset.VColorName = FixLabel(meshset.VColorName, checkingLabels, out dup);
									if (!string.IsNullOrEmpty(dup))
										duplicateLabels.Add(dup);

									meshset.PolyNormalName = FixLabel(meshset.PolyNormalName, checkingLabels, out dup);
									if (!string.IsNullOrEmpty(dup))
										duplicateLabels.Add(dup);
								}
						}
						break;
					case ChunkAttach:
						{
							ChunkAttach chunkatt = (ChunkAttach)obj.Attach;

							chunkatt.PolyName = FixLabel(chunkatt.PolyName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							chunkatt.VertexName = FixLabel(chunkatt.VertexName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);
						}
						break;
					case GC.GCAttach:
						{
							GC.GCAttach gcatt = (GC.GCAttach)obj.Attach;

							gcatt.OpaqueMeshName = FixLabel(gcatt.OpaqueMeshName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							gcatt.TranslucentMeshName = FixLabel(gcatt.TranslucentMeshName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);

							gcatt.VertexName = FixLabel(gcatt.VertexName, checkingLabels, out dup);
							if (!string.IsNullOrEmpty(dup))
								duplicateLabels.Add(dup);
						}
						break;
				}

			}

			if (obj.Children != null && obj.Children.Count > 0)
				foreach (NJS_OBJECT child in obj.Children)
					CheckLabels(child, checkingLabels);

			if (obj.Sibling != null)
				CheckLabels(obj.Sibling, checkingLabels);
		}

		private void CheckModelLabels()
		{
			List<string> duplicateLabels = new List<string>();
			CheckLabels(model, duplicateLabels);
			if (duplicateLabels.Count > 0)
			{
				StringBuilder str = new StringBuilder();
				str.Append("SAMDL has detected the following duplicate labels in the model:\n\n");
				for (int i = 0; i < duplicateLabels.Count; i++)
					str.Append(duplicateLabels[i] + "\n");
				str.Append("\nThe duplicate labels have been renamed automatically. It is recommended to recheck all labels in the model using Model Data Editor.");
				MessageBox.Show(str.ToString(), "SAMDL Warning: Duplicate labels detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void RandomizeLabels(NJS_OBJECT obj)
		{
			obj.Name = "object_" + Extensions.GenerateIdentifier();
			if (obj.Attach != null)
			{
				obj.Attach.Name = "attach_" + Extensions.GenerateIdentifier();
				switch (obj.Attach)
				{
					case BasicAttach:
						{
							BasicAttach basicatt = (BasicAttach)obj.Attach;
							basicatt.MaterialName = "material_" + Extensions.GenerateIdentifier();
							basicatt.MeshName = "meshset_" + Extensions.GenerateIdentifier();
							basicatt.NormalName = "normal_" + Extensions.GenerateIdentifier();
							basicatt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
							if (basicatt.Mesh != null && basicatt.Mesh.Count > 0)
								foreach (NJS_MESHSET meshset in basicatt.Mesh)
								{
									meshset.PolyName = "poly_" + Extensions.GenerateIdentifier();
									meshset.UVName = "uv_" + Extensions.GenerateIdentifier();
									meshset.VColorName = "vcolor_" + Extensions.GenerateIdentifier();
									meshset.PolyNormalName = "polynormal_" + Extensions.GenerateIdentifier();
								}
						}
						break;
					case ChunkAttach:
						{
							ChunkAttach chunkatt = (ChunkAttach)obj.Attach;
							chunkatt.PolyName = "poly_" + Extensions.GenerateIdentifier();
							chunkatt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
						}
						break;
					case GC.GCAttach:
						{
							GC.GCAttach gcatt = (GC.GCAttach)obj.Attach;
							gcatt.TranslucentMeshName = "tpoly_" + Extensions.GenerateIdentifier();
							gcatt.OpaqueMeshName = "opoly_" + Extensions.GenerateIdentifier();
							gcatt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
						}
						break;
				}
			}
			if (obj.Children != null && obj.Children.Count > 0)
				foreach (NJS_OBJECT child in obj.Children)
					RandomizeLabels(child);
			if (obj.Sibling != null)
				RandomizeLabels(obj.Sibling);
		}

		private void resetLabelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dostuff = MessageBox.Show(this, "This will replace all labels with random identifiers. Continue?", "SAMDL", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dostuff == DialogResult.Yes)
			{
				RandomizeLabels(model);
				RebuildModelCache();
				unsavedChanges = true;
			}
		}

		private void LoadTexlistFile(string filename)
		{
			TexList = NJS_TEXLIST.Load(filename);
			UpdateTexlist();
		}

		private void loadTexlistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Title = "Load Texlist", Filter = "Texlist Files (*.satex)|*.satex|All Files (*.*)|*.*", DefaultExt = "satex" })
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					LoadTexlistFile(ofd.FileName);
					unloadTexlistToolStripMenuItem.Enabled = true;
				}
			}
		}

		private void unloadTexlistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TexList = null;
			UpdateTexlist();
			unloadTexlistToolStripMenuItem.Enabled = false;
		}

		private void AddSingleTexture(string filename)
		{
			List<BMPInfo> result = new List<BMPInfo>();
			if (TextureInfo != null && TextureInfo.Length > 0)
				result.AddRange(TextureInfo);
			result.AddRange(TextureArchive.GetTextures(filename, out bool hasNames));
			TextureInfo = result.ToArray();
			UpdateTexlist();
		}

		private void AddTextures(string[] filenames, string paletteFile = null)
		{
			List<BMPInfo> result = new List<BMPInfo>();
			if (TextureInfo != null && TextureInfo.Length > 0)
				result.AddRange(TextureInfo);
			for (int i = 0; i < filenames.Length; i++)
			{
				if (File.Exists(filenames[i]))
					result.AddRange(TextureArchive.GetTextures(filenames[i], out bool hasNames, paletteFile));
				else
					MessageBox.Show(this, "Texture file " + filenames[i] + " doesn't exist.", "SAMDL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			TextureInfo = result.ToArray();
			UpdateTexlist();
		}

		private void addTexturestoolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, Filter = "Supported Files|*.pvr;*.gvr;*.xvr;*.bmp;*.jpg;*.png;*.gif;*.dds;*.pvm;*.gvm;*.xvm;*.prs;*.pvmx;*.pb;*.pak;*.txt|All Files|*.*" })
			{
				if (ofd.ShowDialog() == DialogResult.OK)
					AddTextures(ofd.FileNames);
			}
		}

		private void modelListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadProject(currentProject);
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
				int id_trans = 0;
				int id_opaque = 0;
				List<NJS_OBJECT> transparent = new List<NJS_OBJECT>();
				List<NJS_OBJECT> opaque = new List<NJS_OBJECT>();
				foreach (NJS_MESHSET m in basicatt.Mesh)
				{
					int uvid = 0;
					if (m.Poly == null) continue;
					bool isTransparent = basicatt.Material[m.MaterialID].UseAlpha == true;
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
						string namesuffix = isTransparent ? "_t_" + id_trans.ToString() : "_o_" + id_opaque.ToString();
						mesh_new.PolyName = m.PolyName + "_" + namesuffix;
						if (mesh_new.UV != null)
						{
							mesh_new.UVName = m.UVName + "_" + namesuffix;
							UV[] list = uvlist_new.ToArray();
							for (int u = 0; u < mesh_new.UV.Length; u++)
								mesh_new.UV[u] = list[u];
						}
						if (mesh_new.VColor != null)
						{
							mesh_new.VColorName = m.VColorName + "_" + namesuffix;
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
						newatt.Name = basicatt.Name + "_" + namesuffix;
						newatt.MaterialName = basicatt.MaterialName + "_" + namesuffix;
						newatt.VertexName = basicatt.VertexName + "_" + namesuffix;
						newatt.NormalName = basicatt.NormalName + "_" + namesuffix;
						newatt.MeshName = basicatt.MeshName + "_" + namesuffix;
						NJS_OBJECT newobj = new NJS_OBJECT();
						newobj.Attach = newatt;
						newobj.Name = selectedObject.Name + "_" + namesuffix;
						if (isTransparent)
							transparent.Add(newobj);
						else
							opaque.Add(newobj);
						if (isTransparent)
							id_trans++;
						else
							id_opaque++;
					}
				}
				foreach (NJS_OBJECT newobjs_opaque in opaque)
					selectedObject.AddChild(newobjs_opaque);
				foreach (NJS_OBJECT newobjs_trans in transparent)
					selectedObject.AddChild(newobjs_trans);
				selectedObject.Attach = null;
				selectedObject.SkipChildren = false;
				selectedObject.FixSiblings();
				selectedObject.FixParents();
				RebuildModelCache();
				NeedRedraw = true;
				SelectedItemChanged();
				unsavedChanges = true;
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
						{
							matids.Add((ushort)(mats_opaque.Count + mats_trans.IndexOf(basicatt.Material[m.MaterialID])));
						}
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
				basicatt_new.NormalName = basicatt.NormalName;
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
			NeedRedraw = true;
			SelectedItemChanged();
			unsavedChanges = true;
		}
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
		}

		private void buttonModelList_Click(object sender, EventArgs e)
		{
			LoadProject(currentProject);
		}

		private void renderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sfd = new SaveFileDialog
			{
				DefaultExt = "png",
				Title = "Render",
				Filter = "PNG Files|*.png|All Files|*.*"
			})
			{
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					var bgcol = EditorOptions.FillColor;
					EditorOptions.FillColor = Color.Transparent;
					using (var backbuf = d3ddevice.GetRenderTarget(0))
					using (var rt = Surface.CreateRenderTarget(d3ddevice, backbuf.Description.Width, backbuf.Description.Height, Format.A8R8G8B8, backbuf.Description.MultiSampleType, backbuf.Description.MultiSampleQuality, false))
					{
						d3ddevice.SetRenderTarget(0, rt);
						DrawEntireModel();
						using (var surf = Surface.CreateOffscreenPlain(d3ddevice, backbuf.Description.Width, backbuf.Description.Height, Format.A8R8G8B8, Pool.SystemMemory))
						{
							d3ddevice.GetRenderTargetData(rt, surf);
							var rect = surf.LockRectangle(LockFlags.ReadOnly);
							using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(backbuf.Description.Width, backbuf.Description.Height, rect.Pitch, System.Drawing.Imaging.PixelFormat.Format32bppArgb, rect.DataPointer))
								bmp.Save(sfd.FileName);
						}
						d3ddevice.SetRenderTarget(0, backbuf);
						EditorOptions.FillColor = bgcol;
					}
				}
			}
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog
			{
				DefaultExt = "dae",
				Title = "Import Model",
				Filter = "Model Files|*.obj;*.fbx;*.dae;|All Files|*.*"
			})
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					ImportModelFromFile(ofd.FileName);
				}
			}
		}

		private void SetPartialTexlist(int[] texIDs)
		{
			// If a partial texlist isn't set, remove all of it
			if (texIDs == null)
				TexList = null;
			// If a partial texlist exists, trim the texlist
			else
			{
				List<string> texnames = new List<string>();
				for (int i = texIDs[0]; i <= texIDs[texIDs.Length - 1]; i++)
					texnames.Add(TextureInfo[i].Name);
				TexList = new NJS_TEXLIST(texnames.ToArray());
			}
			UpdateTexlist();
		}

		private void buttonTextures_CheckedChanged(object sender, EventArgs e)
		{
			osd.UpdateOSDItem("Show textures: " + (buttonTextures.Checked ? "On" : "Off"), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
		}

		private void LoadModelInfo(ModelLoadInfo info)
		{
			if (info == null)
				return;
			// Load model file
			if (info.ModelFilePath != "" && File.Exists(info.ModelFilePath))
				LoadFile(info.ModelFilePath);
			// Load textures
			if (info.TextureArchives != null)
			{
				// Load texture archives
				AddTextures(info.TextureArchives, info.TexturePalettePath);
				if (info.TextureArchives.Length == 1)
					TexturePackName = Path.GetFileNameWithoutExtension(info.TextureArchives[0]);
				// Set texture IDs for partial texlist if defined
				if (info.TextureIDs != null)
					SetPartialTexlist(info.TextureIDs);
				// Set texture names for partial texlist if defined
				else if (info.TextureNames != null)
					TexList = info.TextureNames;
				UpdateTexlist();
			}
		}

		private void MoveObjectInHierarchy(bool down)
		{
			int index = selectedObject.Parent.Children.IndexOf(selectedObject);
			NJS_OBJECT parent = selectedObject.Parent;
			if (!down && index == 0 || down && index >= parent.Children.Count - 1)
				return;
			List<NJS_OBJECT> newobjs = new List<NJS_OBJECT>();
			foreach (NJS_OBJECT obj in selectedObject.Parent.Children)
			{
				newobjs.Add(obj.Clone());
			}
			// Move down
			if (down)
			{
				newobjs.Insert(index + 2, selectedObject);
				newobjs.RemoveAt(index);
			}
			// Move up
			else
			{
				newobjs.Insert(index - 1, selectedObject);
				newobjs.RemoveAt(index + 1);
			}
			selectedObject.Parent.ClearChildren();
			foreach (NJS_OBJECT obj in newobjs)
			{
				obj.ClearSibling();
				parent.AddChild(obj);
			}
			selectedObject.Parent.FixSiblings();
			selectedObject.FixParents();
			RebuildModelCache();
			NeedRedraw = true;
		}

		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MoveObjectInHierarchy(false);
			treeView1.SelectedNode = nodeDict[selectedObject];
		}

		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MoveObjectInHierarchy(true);
			treeView1.SelectedNode = nodeDict[selectedObject];
		}

		private void LoadProject(string filename)
		{
			currentProject = filename;
			ModelSelectDialog mdldialog = new ModelSelectDialog(ProjectFunctions.openProjectFileString(filename), lastProjectModeCategory);
			DialogResult result = mdldialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				UnloadTextures();
				lastProjectModeCategory = mdldialog.SelectedCategory;
				LoadModelInfo(mdldialog.ModelInfo);
			}
			modelListToolStripMenuItem.Enabled = buttonModelList.Enabled = true;
		}

		private void panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (!loaded || !RenderPanel.Focused)
				return;

			if (cam.mode == 0)
				cam.Position += cam.Look * (cam.MoveSpeed * e.Delta * -1);
			else if (cam.mode == 1)
				cam.Distance += (cam.MoveSpeed * e.Delta * -1);

			NeedRedraw = true;
		}

		private void editModelDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int idx = 0;
			if (selectedObject != null)
			{
				NJS_OBJECT[] objs = model.GetObjects();
				for (int i = 0; i < objs.Length; i++)
					if (objs[i] == selectedObject)
					{
						idx = i;
						break;
					}
			}
			switch (selectedObject.Attach)
			{
				case BasicAttach:
				case ChunkAttach:
					{
						ModelDataEditor me = new ModelDataEditor(model, idx);
						if (me.ShowDialog(this) == DialogResult.OK)
						{
							model = me.editedHierarchy.Clone();
							model.FixParents();
							model.FixSiblings();
							RebuildModelCache();
							NeedRedraw = true;
							unsavedChanges = true;
							selectedObject = model.GetObjects()[idx];
							SelectedItemChanged();
						}
					}
					break;
				case GC.GCAttach:
					{
						GCModelDataEditor me = new GCModelDataEditor(model, idx);
						if (me.ShowDialog(this) == DialogResult.OK)
						{
							model = me.editedHierarchy.Clone();
							RebuildModelCache();
							NeedRedraw = true;
							unsavedChanges = true;
						}
					}
					break;
			}
		}

		private void modelInfoEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UI.ModelInfoEditor mtd = new UI.ModelInfoEditor(model, modelAuthor, modelDescription, currentFileName);
			if (mtd.ShowDialog() == DialogResult.OK)
			{
				modelAuthor = mtd.ModelAuthor;
				modelDescription = mtd.ModelDescription;
			}
		}

		private void setDefaultAnimationOrientationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AnimOrientation dlg = new AnimOrientation())
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					SetAnimOrientations(dlg.Position, dlg.AnimRotation, dlg.AnimScale);
				}
		}

		private void importLabelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string fn = !string.IsNullOrEmpty(currentFileName) ? Path.ChangeExtension(currentFileName, ".salabel") : "model.salabel";
			using (OpenFileDialog ofd = new OpenFileDialog() { Title = "Import Labels", DefaultExt = ".salabel", FileName = Path.GetFileName(fn), Filter = "Label Files|*.salabel|All Files|*.*" })
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					LabelOBJECT.ImportLabels(model, LabelOBJECT.Load(ofd.FileName));
					RebuildModelCache();
					unsavedChanges = true;
				}
			}
		}

		private void exportLabelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string fn = !string.IsNullOrEmpty(currentFileName) ? Path.ChangeExtension(currentFileName, ".salabel") : "model.salabel";
			using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Export Labels", DefaultExt = ".salabel", FileName = Path.GetFileName(fn), Filter = "Label Files|*.salabel|All Files|*.*" })
				if (sfd.ShowDialog() == DialogResult.OK)
					LabelOBJECT.Save(LabelOBJECT.ExportLabels(model), sfd.FileName);
		}

		private int ReadNJTL(byte[] file, ref bool basicModel, ref NJS_TEXLIST texList)
		{
			int ninjaDataOffset;

			//FUCK YOU Skys of Arcadia GC
			ByteConverter.BigEndian = HelperFunctions.CheckBigEndianInt32(file, 0x4);
			int POF0Offset = ByteConverter.ToInt32(file, 0x4) + 0x8;

			//POF0Size will use same endianness
			//Checking again because SOA compatibility requires it
			int POF0Size = ByteConverter.ToInt32(file, POF0Offset + 0x4);

			//Set to main endianness for general reading
			ByteConverter.BigEndian = HelperFunctions.CheckBigEndianInt32(file, 0x8);
			int texListOffset = POF0Offset + POF0Size + 0x8;
			ninjaDataOffset = texListOffset + 0x8;
			int texCount = ByteConverter.ToInt32(file, 0xC);
			int texOffset = 0;
			List<string> texNames = new List<string>();
			// Check if it's a basic model
			if (System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(BitConverter.ToInt32(file, texListOffset))) == "NJBM")
				basicModel = true;
			for (int i = 0; i < texCount; i++)
			{
				int textAddress = ByteConverter.ToInt32(file, texOffset + 0x10) + 0x8;
				// Read null terminated string
				List<byte> namestring = new List<byte>();
				byte namechar = (file[textAddress]);
				int j = 0;
				while (namechar != 0)
				{
					namestring.Add(namechar);
					j++;
					namechar = (file[textAddress + j]);
				}
				texNames.Add(Encoding.ASCII.GetString(namestring.ToArray()));
				texOffset += 0xC;
			}
			texList = new NJS_TEXLIST(texNames.ToArray());
			return ninjaDataOffset;
		}
	}
}