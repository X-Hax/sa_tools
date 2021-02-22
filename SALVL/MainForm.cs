using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;

using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.DLLModGenerator;
using SonicRetro.SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace SonicRetro.SAModel.SALVL
{
	public partial class MainForm : Form
	{
		SettingsFile settingsfile; //For user editable settings
		Properties.Settings AppConfig = Properties.Settings.Default; // For non-user editable settings in SALVL.config
		Logger log = new Logger(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SALVL.log");
		OnScreenDisplay osd;

		public MainForm()
		{
			InitializeComponent();
			AddMouseMoveHandler(this);
			Application.ThreadException += Application_ThreadException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		}
		private void AddMouseMoveHandler(Control c)
		{
			c.MouseMove += panel1_MouseMove;
			if (c.Controls.Count > 0)
			{
				foreach (Control ct in c.Controls)
					AddMouseMoveHandler(ct);
			}
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			log.Add(e.Exception.ToString());
			log.WriteLog();
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SALVL Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
				Close();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			log.Add(e.ExceptionObject.GetType().Name.ToString());
			log.WriteLog();
			MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SALVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		bool loaded;
		bool unsaved;
		int interval = 20;
		EditorItemSelection selectedItems = new EditorItemSelection();

		#region UI & Customization
		bool mouseWrapScreen = false;
		ushort mouseWrapThreshold = 2;
		bool mouseHide = false;
		Point mouseBackup;
		EditorOptionsEditor optionsEditor;
		bool lookKeyDown;
		bool zoomKeyDown;
		bool moveKeyDown;
		TransformGizmo transformGizmo;
		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;
		#endregion

		private void MainForm_Load(object sender, EventArgs e)
		{
			Assimp.Unmanaged.AssimpLibrary.Instance.LoadLibrary(Path.Combine(Application.StartupPath, "lib", "assimp.dll"));
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			d3ddevice = new Device(new SharpDX.Direct3D9.Direct3D(), 0, DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = Format.D24X8 } });
			settingsfile = SettingsFile.Load();
			EditorOptions.RenderDrawDistance = settingsfile.SALVL.DrawDistance_General;
			EditorOptions.Initialize(d3ddevice);
			osd = new OnScreenDisplay(d3ddevice, Color.Red.ToRawColorBGRA());
			AppConfig.Reload();
			if (settingsfile.SALVL.ShowWelcomeScreen)
			{
				ShowWelcomeScreen();
			}

			actionList = ActionMappingList.Load(Path.Combine(Application.StartupPath, "keybinds", "SALVL.ini"),
				DefaultActionList.DefaultActionMapping);

			actionInputCollector = new ActionInputCollector();
			actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			actionInputCollector.OnActionStart += ActionInputCollector_OnActionStart;
			actionInputCollector.OnActionRelease += ActionInputCollector_OnActionRelease;

			cam.ModifierKey = settingsfile.SALVL.CameraModifier;
			alternativeCameraModeToolStripMenuItem.Checked = settingsfile.SALVL.AlternativeCamera;
			optionsEditor = new EditorOptionsEditor(cam, actionList.ActionKeyMappings.ToArray(), DefaultActionList.DefaultActionMapping, false, false);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.FormUpdatedKeys += optionsEditor_FormUpdatedKeys;

			Gizmo.InitGizmo(d3ddevice);
			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);

			LevelData.StateChanged += LevelData_StateChanged;
			panel1.MouseWheel += panel1_MouseWheel;
		}

		void ShowWelcomeScreen()
		{
			WelcomeForm welcomeForm = new WelcomeForm();
			welcomeForm.showOnStartCheckbox.Checked = settingsfile.SALVL.ShowWelcomeScreen;

			// subscribe to our checkchanged event
			welcomeForm.showOnStartCheckbox.CheckedChanged += (object form, EventArgs eventArg) =>
			{
				settingsfile.SALVL.ShowWelcomeScreen = welcomeForm.showOnStartCheckbox.Checked;
			};

			welcomeForm.ThisToolLink.Text = "SALVL Documentation";
			welcomeForm.ThisToolLink.Visible = true;

			welcomeForm.ThisToolLink.LinkClicked += (object link, LinkLabelLinkClickedEventArgs linkEventArgs) =>
			{
				welcomeForm.GoToSite("https://github.com/sonicretro/sa_tools/wiki/SALVL");
			};

			welcomeForm.ShowDialog();

			welcomeForm.Dispose();
			welcomeForm = null;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded && unsaved)
				switch (MessageBox.Show(this, "Do you want to save?", "SALVL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1lvl",
				Filter = "Level Files|*.sa1lvl;*.sa2lvl;*.sa2blvl;*.exe;*.dll;*.bin;*.prs|All Files|*.*"
			};
			if (a.ShowDialog(this) == DialogResult.OK)
				LoadFile(a.FileName);
		}

		private void LoadFile(string filename)
		{
			loaded = false;
			selectedItems = new EditorItemSelection();
			toolStrip1.Enabled = loadTexturesToolStripMenuItem.Enabled = false;
			UseWaitCursor = true;
			Enabled = false;
			LevelData.leveltexs = null;
			cam = new EditorCamera(EditorOptions.RenderDrawDistance);
			if (LandTable.CheckLevelFile(filename))
				LevelData.geo = LandTable.LoadFromFile(filename);
			else
			{
				byte[] file = File.ReadAllBytes(filename);
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					file = FraGag.Compression.Prs.Decompress(file);
				using (LevelFileDialog dlg = new LevelFileDialog())
				{
					dlg.ShowDialog(this);
					if (dlg.DialogResult != DialogResult.OK)
					{
						Enabled = true;
						UseWaitCursor = false;
						return;
					}
					else LevelData.geo = new LandTable(file, (int)dlg.NumericUpDown1.Value, (uint)dlg.numericUpDown2.Value, (LandTableFormat)dlg.comboBox2.SelectedIndex);
				}
			}
			LevelData.ClearLevelItems();

			for (int i = 0; i < LevelData.geo.COL.Count; i++)
			{
				LevelData.AddLevelItem((new LevelItem(LevelData.geo.COL[i], i, selectedItems)));
			}

			LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
			LevelData.Textures = new Dictionary<string, Texture[]>();
			unloadTexturesToolStripMenuItem.Enabled = false;
			using (OpenFileDialog a = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs", Title = "Load textures" })
			{
				if (!string.IsNullOrEmpty(LevelData.geo.TextureFileName))
					a.FileName = LevelData.geo.TextureFileName + ".pvm";
				else
					a.FileName = string.Empty;
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					BMPInfo[] TexBmps = TextureArchive.GetTextures(a.FileName);
					Texture[] texs = new Texture[TexBmps.Length];
					for (int j = 0; j < TexBmps.Length; j++)
						texs[j] = TexBmps[j].Image.ToTexture(d3ddevice);
					string texname = Path.GetFileNameWithoutExtension(a.FileName);
					if (!LevelData.TextureBitmaps.ContainsKey(texname))
						LevelData.TextureBitmaps.Add(texname, TexBmps);
					if (!LevelData.Textures.ContainsKey(texname))
						LevelData.Textures.Add(texname, texs);
					LevelData.leveltexs = texname;
					unloadTexturesToolStripMenuItem.Enabled = true;
				}
			}
			loaded = true;
			unsaved = false;
			toolStrip1.Enabled = loadTexturesToolStripMenuItem.Enabled = true;
			 
			transformGizmo = new TransformGizmo();
			gizmoSpaceComboBox.Enabled = true;
			if (gizmoSpaceComboBox.SelectedIndex == -1) gizmoSpaceComboBox.SelectedIndex = 0;
			pivotComboBox.Enabled = true;
			if (pivotComboBox.SelectedIndex == -1) pivotComboBox.SelectedIndex = 0;

			clearLevelToolStripMenuItem.Enabled = LevelData.geo != null;
			calculateAllBoundsToolStripMenuItem.Enabled = LevelData.geo != null;
			statsToolStripMenuItem.Enabled = LevelData.geo != null;
			selectedItems.SelectionChanged += SelectionChanged;
			UseWaitCursor = false;
			Enabled = editInfoToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = true;

			DrawLevel();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loaded && unsaved)
			{
				switch (MessageBox.Show(this, "Do you want to save?", "SALVL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}

			try
			{
				settingsfile.SALVL.AlternativeCamera = alternativeCameraModeToolStripMenuItem.Checked;
				settingsfile.Save();
			}
			catch { };
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LandTableFormat outfmt = LevelData.geo.Format;
			if (outfmt == LandTableFormat.SADX)
				outfmt = LandTableFormat.SA1;
			using (SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = outfmt.ToString().ToLowerInvariant() + "lvl",
				Filter = outfmt.ToString().ToUpperInvariant() + "LVL Files|*." + outfmt.ToString().ToLowerInvariant() + "lvl|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					LevelData.geo.SaveToFile(a.FileName, outfmt);
					unsaved = false;
				}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		internal void DrawLevel()
		{
			if (!loaded) return;
			cam.FOV = (float)(Math.PI / 4);
			cam.Aspect = panel1.Width / (float)panel1.Height;
			cam.DrawDistance = 10000;
			Matrix projection = Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance);
			Matrix view = cam.ToMatrix();
			d3ddevice.SetTransform(TransformState.Projection, projection);
			d3ddevice.SetTransform(TransformState.View, view);
			Text = "X=" + cam.Position.X + " Y=" + cam.Position.Y + " Z=" + cam.Position.Z + " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X") + " Interval=" + interval + (cam.mode == 1 ? " Distance=" + cam.Distance : "");
			d3ddevice.SetRenderState(RenderState.FillMode, (int)EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderState.CullMode, (int)EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = Color.White.ToRawColor4() };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			d3ddevice.BeginScene();
			//all drawings after this line
			cam.DrawDistance = EditorOptions.RenderDrawDistance;
			cam.BuildFrustum(view, projection);

			EditorOptions.RenderStateCommonSetup(d3ddevice);

			MatrixStack transform = new MatrixStack();
			List<RenderInfo> renderlist = new List<RenderInfo>();
			if (LevelData.LevelItems != null)
				for (int i = 0; i < LevelData.LevelItemCount; i++)
				{
					bool display = false;
					if (visibleToolStripMenuItem.Checked && LevelData.GetLevelitemAtIndex(i).Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !LevelData.GetLevelitemAtIndex(i).Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;
					if (display)
						renderlist.AddRange(LevelData.GetLevelitemAtIndex(i).Render(d3ddevice, cam, transform));
				}
			RenderInfo.Draw(renderlist, d3ddevice, cam);
			osd.ProcessMessages();
			d3ddevice.EndScene(); // scene drawings go before this line

			transformGizmo.Draw(d3ddevice, cam);
			d3ddevice.Present();
		}

		private void UpdateTitlebar()
		{
			Text = "SALVL - " + "(" + cam.Position.X + ", " + cam.Position.Y + ", " + cam.Position.Z
				+ " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X")
				+ " Speed=" + cam.MoveSpeed + (cam.mode == 1 ? " Distance=" + cam.Distance : "") + ")";
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			DrawLevel();
		}

		#region User Keyboard / Mouse Methods
		void optionsEditor_FormUpdatedKeys()
		{

			// Keybinds
			actionList.ActionKeyMappings.Clear();
			ActionKeyMapping[] newMappings = optionsEditor.GetActionkeyMappings();
			foreach (ActionKeyMapping mapping in newMappings) 
				actionList.ActionKeyMappings.Add(mapping);
			actionInputCollector.SetActions(newMappings);
			string saveControlsPath = Path.Combine(Application.StartupPath, "keybinds", "SALVL.ini");
			actionList.Save(saveControlsPath);
			// Settings
			optionsEditor_FormUpdated();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.O:
					if (e.Control)
						openToolStripMenuItem_Click(sender, EventArgs.Empty);
					break;
				case Keys.S:
					if (!loaded) return;
					if (e.Control)
						saveToolStripMenuItem_Click(sender, EventArgs.Empty);
					break;
			}
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyDown(Keys.MButton);

			if (!loaded) return;

			switch (e.Button)
			{
				case MouseButtons.Left:
					float mindist = cam.DrawDistance;
					HitResult dist;
					Item item = null;
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
					Viewport viewport = d3ddevice.Viewport;
					viewport.Width = panel1.Width;
					viewport.Height = panel1.Height;
					Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
					Matrix view = d3ddevice.GetTransform(TransformState.View);
					Vector3 Near, Far;
					Near = mousepos;
					Near.Z = 0;
					Far = Near;
					Far.Z = -1;
					if (transformGizmo != null)
					{
						transformGizmo.SelectedAxes = transformGizmo.CheckHit(Near, Far, viewport, proj, view, cam);
						if ((transformGizmo.SelectedAxes == GizmoSelectedAxes.NONE))
						{
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
									if (display)
									{
										dist = LevelData.GetLevelitemAtIndex(i).CheckHit(Near, Far, viewport, proj, view);
										if (dist.IsHit & dist.Distance < mindist)
										{
											mindist = dist.Distance;
											item = LevelData.GetLevelitemAtIndex(i);
										}
									}
								}
							}

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
						}
					}
					break;
				case MouseButtons.Right:
					bool cancopy = false;
					foreach (Item obj in selectedItems.GetSelection())
						if (obj.CanCopy)
							cancopy = true;
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

			DrawLevel();
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyUp(Keys.MButton);

			propertyGrid1.Refresh();
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
			actionInputCollector.KeyUp(e.KeyCode);
		}

		private void panel1_KeyDown(object sender, KeyEventArgs e)
		{
			actionInputCollector.KeyDown(e.KeyCode);
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
						if (selectedItems.GetSelection().Count > 0)
							cam.FocalPoint = Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3();
						else
							cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
					}
					osd.UpdateOSDItem("Camera mode: " + cammode, panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
					osd.UpdateOSDItem("Camera zoomed to target", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
					osd.UpdateOSDItem("Render mode: " + rendermode, panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Decrease camera move speed"):
					cam.MoveSpeed = Math.Max(0.0625f, cam.MoveSpeed -= 0.0625f);
					UpdateTitlebar();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Reset camera move speed"):
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					UpdateTitlebar();
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Reset Camera Position"):
					if (cam.mode == 0)
					{
						cam.Position = new Vector3();
						osd.UpdateOSDItem("Reset camera position", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
						draw = true;
					}
					break;

				case ("Reset Camera Rotation"):
					if (cam.mode == 0)
					{
						cam.Pitch = 0;
						cam.Yaw = 0;
						osd.UpdateOSDItem("Reset camera rotation", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
						draw = true;
					}
					break;

				case ("Camera Move"):
					moveKeyDown = false;
					break;

				case ("Camera Zoom"):
					zoomKeyDown = false;
					break;

				case ("Camera Look"):
					lookKeyDown = false;
					break;

				default:
					break;
			}

			if (draw)
			{
				DrawLevel();
			}
		}

		private void ActionInputCollector_OnActionStart(ActionInputCollector sender, string actionName)
		{
			switch (actionName)
			{
				case ("Camera Move"):
					moveKeyDown = true;
					osd.UpdateOSDItem("Camera mode: Move", panel1.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					break;

				case ("Camera Zoom"):
					zoomKeyDown = true;
					osd.UpdateOSDItem("Camera mode: Zoom", panel1.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					break;

				case ("Camera Look"):
					lookKeyDown = true;
					osd.UpdateOSDItem("Camera mode: Look", panel1.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
					break;

				default:
					break;
			}
		}

		private void panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			bool draw = false;
			bool gizmo = false;
			bool mouseWrapScreen = false;
			
			switch (e.Button)
			{
				case MouseButtons.Middle:
					break;

				case MouseButtons.Left:
					foreach (PointHelper pointHelper in PointHelper.Instances)
					{
						pointHelper.TransformAffected(cam.mouseDelta.X / 2 * cam.MoveSpeed, cam.mouseDelta.Y / 2 * cam.MoveSpeed, cam);
					}
					transformGizmo.TransformGizmoMove(cam.mouseDelta, cam, selectedItems);
					if (selectedItems.ItemCount > 0 && (cam.mouseDelta.X != 0 || cam.mouseDelta.Y != 0))
					{
						if (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE) gizmo = true;
						unsaved = true;
					}
					draw = true;
					break;

				case MouseButtons.None:
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
					Viewport viewport = d3ddevice.Viewport;
					viewport.Width = panel1.Width;
					viewport.Height = panel1.Height;
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

			System.Drawing.Rectangle mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : panel1.RectangleToScreen(panel1.Bounds);
			int camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, moveKeyDown, alternativeCameraModeToolStripMenuItem.Checked, gizmo);

			if (camresult >= 2 && selectedItems != null && selectedItems.ItemCount > 0 && propertyGrid1.ActiveControl == null) propertyGrid1.Refresh();

			if (camresult >= 1 || draw)
			{
				DrawLevel();
			}
		}
	
		void panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (!panel1.Focused) return;

			float detentValue = -1;

			if (e.Delta < 0) detentValue = 1;

			if (cam.mode == 0) cam.Position += cam.Look * (detentValue * cam.MoveSpeed);
			else if (cam.mode == 1) cam.Distance += (detentValue * cam.MoveSpeed);
			DrawLevel();
		}
		#endregion

		void SelectionChanged(EditorItemSelection sender)
		{
			propertyGrid1.SelectedObjects = sender.GetSelection().ToArray();

			if (cam.mode == 1)
			{
				cam.FocalPoint = Item.CenterFromSelection(sender.GetSelection()).ToVector3();
			}

			if (sender.ItemCount > 0) // set up gizmo
			{
				transformGizmo.Enabled = true;
				transformGizmo.SetGizmo(Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3(),
					selectedItems.Get(0).TransformMatrix);
				transformGizmo.TransformGizmoMove(cam.mouseDelta, cam, selectedItems);
			}
			else
			{
				if (transformGizmo != null)
				{
					transformGizmo.Enabled = false;
				}
			}
			duplicateToolStripMenuItem.Enabled = selectedItems.ItemCount > 0;
			deleteToolStripMenuItem.Enabled = selectedItems.ItemCount > 0;
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in selectedItems.GetSelection())
				if (item.CanCopy)
				{
					item.Delete(selectedItems);
					selitems.Add(item);
				}
			selectedItems.Clear();
			DrawLevel();
			if (selitems.Count == 0) return;
			Clipboard.SetData("SADXLVLObjectList", selitems);
			unsaved = true;
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in selectedItems.GetSelection())
				if (item.CanCopy)
					selitems.Add(item);
			if (selitems.Count == 0) return;
			Clipboard.SetData("SADXLVLObjectList", selitems);
			unsaved = true;
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> objs = Clipboard.GetData("SADXLVLObjectList") as List<Item>;

			if (objs == null)
			{
				osd.AddMessage("Paste operation failed - feature not implemented.", 180);
				return; // todo: finish implementing proper copy/paste
			}

			Vector3 center = new Vector3();
			foreach (Item item in objs)
				center += item.Position.ToVector3();
			center = new Vector3(center.X / objs.Count, center.Y / objs.Count, center.Z / objs.Count);
			foreach (Item item in objs)
			{
				item.Position = new Vertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
				item.Paste();
			}
			selectedItems.Add(new List<Item>(objs)); 
			unsaved = true;
			DrawLevel();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in selectedItems.GetSelection())
				if (item.CanCopy)
					item.Delete(selectedItems);
			selectedItems.Clear();
			unsaved = true;
			DrawLevel();
		}

		private void levelToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in levelToolStripMenuItem.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			DrawLevel();
		}

		private void levelPieceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = importFileDialog.FileName;


				LevelData.ImportFromFile(filePath, cam, out bool errorFlag, out string errorMsg, selectedItems);

				if (errorFlag)
				{
					osd.AddMessage(errorMsg, 300);
				}
				unsaved = true;
			}
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = importFileDialog.FileName;
				DialogResult userClearLevelResult = MessageBox.Show("Do you want to clear the level models first?", "Clear Level?", MessageBoxButtons.YesNoCancel);

				if (userClearLevelResult == DialogResult.Cancel) return;
				else if (userClearLevelResult == DialogResult.Yes)
				{
					DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNo);

					LevelData.ClearLevelGeometry();

					if (clearAnimsResult == DialogResult.Yes)
					{
						LevelData.ClearLevelGeoAnims();
					}
				}


				LevelData.ImportFromFile(filePath, cam, out bool errorFlag, out string errorMsg, selectedItems);

				if (errorFlag)
				{
					osd.AddMessage(errorMsg, 300);
				}
				unsaved = true;
			}
		}

		private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			#region Old Code
			/*SaveFileDialog a = new SaveFileDialog
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

					// This is admittedly not an accurate representation of the materials used in the model - HOWEVER, it makes the materials more managable in MAX
					// So we're doing it this way. In the future we should come back and add an option to do it this way or the original way.
					for (int texIndx = 0; texIndx < LevelData.TextureBitmaps[LevelData.leveltexs].Length; texIndx++)
					{
						mtlstream.WriteLine(String.Format("newmtl {0}_material_{1}", materialPrefix, texIndx));
						mtlstream.WriteLine("Ka 1 1 1");
						mtlstream.WriteLine("Kd 1 1 1");
						mtlstream.WriteLine("Ks 0 0 0");
						mtlstream.WriteLine("illum 1");

						if (!string.IsNullOrEmpty(LevelData.leveltexs))
						{
							mtlstream.WriteLine("Map_Kd " + LevelData.TextureBitmaps[LevelData.leveltexs][texIndx].Name + ".png");

							// save texture
							string mypath = Path.GetDirectoryName(a.FileName);
							BMPInfo item = LevelData.TextureBitmaps[LevelData.leveltexs][texIndx];
							item.Image.Save(Path.Combine(mypath, item.Name + ".png"));
						}
					}
					#endregion

					int totalVerts = 0;
					int totalNorms = 0;
					int totalUVs = 0;

					bool errorFlag = false;

					for (int i = 0; i < LevelData.geo.COL.Count; i++)
						Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, materialPrefix, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
					if (LevelData.geo.Anim != null)
						for (int i = 0; i < LevelData.geo.Anim.Count; i++)
							Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.Anim[i].Model, materialPrefix, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

					if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");
				}
			}*/
			#endregion

			ExportObj();
		}

		private void ExportObj()
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
					int stepCount = LevelData.geo.COL.Count;
					if (LevelData.TextureBitmaps.Count > 0) stepCount += LevelData.TextureBitmaps[LevelData.leveltexs].Length;
					if (LevelData.geo.Anim != null)
						stepCount += LevelData.geo.Anim.Count;

					List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();

					ProgressDialog progress = new ProgressDialog("Exporting stage", stepCount, true, false);
					progress.Show(this);
					progress.SetTaskAndStep("Exporting...");

					int totalVerts = 0;
					int totalNorms = 0;
					int totalUVs = 0;

					bool errorFlag = false;

					for (int i = 0; i < LevelData.geo.COL.Count; i++)
					{
						Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, ref materials, new MatrixStack(),
							ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

						progress.Step = String.Format("Mesh {0}/{1}", i + 1, LevelData.geo.COL.Count);
						progress.StepProgress();
						Application.DoEvents();
					}
					if (LevelData.geo.Anim != null)
					{
						for (int i = 0; i < LevelData.geo.Anim.Count; i++)
						{
							Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.Anim[i].Model, ref materials, new MatrixStack(),
								ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

							progress.Step = String.Format("Animation {0}/{1}", i + 1, LevelData.geo.Anim.Count);
							progress.StepProgress();
							Application.DoEvents();
						}
					}

					if (errorFlag)
					{
						osd.AddMessage("Error(s) encountered during export. Inspect the output file for more details.", 180);
					}

					#region Material Exporting
					string materialPrefix = LevelData.leveltexs;

					objstream.WriteLine("mtllib " + Path.GetFileNameWithoutExtension(a.FileName) + ".mtl");

					for (int i = 0; i < materials.Count; i++)
					{
						NJS_MATERIAL material = materials[i];
						mtlstream.WriteLine("newmtl material_{0}", i);
						mtlstream.WriteLine("Ka 1 1 1");
						mtlstream.WriteLine(string.Format("Kd {0} {1} {2}",
							material.DiffuseColor.R / 255,
							material.DiffuseColor.G / 255,
							material.DiffuseColor.B / 255));

						mtlstream.WriteLine(string.Format("Ks {0} {1} {2}",
							material.SpecularColor.R / 255,
							material.SpecularColor.G / 255,
							material.SpecularColor.B / 255));
						mtlstream.WriteLine("illum 1");

						if (!string.IsNullOrEmpty(LevelData.leveltexs) && material.UseTexture)
						{
							mtlstream.WriteLine("Map_Kd " + LevelData.TextureBitmaps[LevelData.leveltexs][material.TextureID].Name + ".png");

							// save texture
							string mypath = Path.GetDirectoryName(a.FileName);
							BMPInfo item = LevelData.TextureBitmaps[LevelData.leveltexs][material.TextureID];
							item.Image.Save(Path.Combine(mypath, item.Name + ".png"));
						}

						//progress.Step = String.Format("Texture {0}/{1}", material.TextureID + 1, LevelData.TextureBitmaps[LevelData.leveltexs].Length);
						//progress.StepProgress();
						Application.DoEvents();
					}
					#endregion

					progress.SetTaskAndStep("Export complete!");
				}
			}
		}

		private void editInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AdvancedInfoDialog dlg = new AdvancedInfoDialog())
			{
				dlg.ShowDialog(this);
				if (dlg.DialogResult == DialogResult.OK) unsaved = true;
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			DrawLevel();
			unsaved = true;
		}

		private void cStructsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "c", Filter = "C Files|*.c" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					LandTableFormat fmt = LevelData.geo.Format;
					switch (fmt)
					{
						case LandTableFormat.SA1:
						case LandTableFormat.SADX:
							using (StructExportDialog ed = new StructExportDialog() { Format = LevelData.geo.Format })
								if (ed.ShowDialog(this) == DialogResult.OK)
									fmt = ed.Format;
								else
									return;
							break;
					}
					List<string> labels = new List<string>() { LevelData.geo.Name };
					using (StreamWriter sw = File.CreateText(sd.FileName))
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
						}
						sw.WriteLine(" LandTable");
						sw.WriteLine(" * ");
						sw.WriteLine(" * Generated by SALVL");
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
						string[] texnames = null;
						if (LevelData.leveltexs != null)
						{
							texnames = new string[LevelData.TextureBitmaps[LevelData.leveltexs].Length];
							for (int i = 0; i < LevelData.TextureBitmaps[LevelData.leveltexs].Length; i++)
								texnames[i] = string.Format("{0}TexName_{1}", LevelData.leveltexs,
									LevelData.TextureBitmaps[LevelData.leveltexs][i].Name);
							sw.Write("enum {0}TexName", LevelData.leveltexs);
							sw.WriteLine();
							sw.WriteLine("{");
							sw.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texnames));
							sw.WriteLine("};");
							sw.WriteLine();
						}
						LevelData.geo.ToStructVariables(sw, fmt, labels, texnames);
					}
				}
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo != null) transformGizmo.Enabled = selectedItems.ItemCount > 0;
			DrawLevel();
			unsaved = true;
		}

		private void clearLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNoCancel);

			if (clearAnimsResult == DialogResult.Cancel) return;

			LevelData.ClearLevelGeometry();

			if (clearAnimsResult == DialogResult.Yes)
			{
				LevelData.ClearLevelGeoAnims();
			}
			unsaved = true;
		}

		private void statsToolStripMenuItem_Click_1(object sender, EventArgs e)
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

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.DuplicateSelection(selectedItems, out bool errorFlag, out string errorMsg);

			if (errorFlag) osd.AddMessage(errorMsg, 180);
			unsaved = true;
		}

		private void debugLightingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DefaultLightEditor lightEditor = new DefaultLightEditor();

			lightEditor.Show();
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
		}

		void optionsEditor_FormUpdated()
		{
			settingsfile.SALVL.DrawDistance_General = EditorOptions.RenderDrawDistance;
			settingsfile.SALVL.CameraModifier = cam.ModifierKey;
			DrawLevel();
		}

		#region Gizmo Button Event Methods
		private void selectModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.NONE;
				gizmoSpaceComboBox.Enabled = true;
				pivotComboBox.Enabled = true;
				selectModeButton.Checked = true;
				moveModeButton.Checked = false;
				rotateModeButton.Checked = false;
				scaleModeButton.Checked = false;
				SetGizmoPivotAndLocality();
				osd.UpdateOSDItem("Transform mode: Select", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
				osd.UpdateOSDItem("Transform mode: Move", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
				pivotComboBox.Enabled = false;
				pivotComboBox.SelectedIndex = 0;
				selectModeButton.Checked = false;
				rotateModeButton.Checked = true;
				moveModeButton.Checked = false;
				scaleModeButton.Checked = false;
				SetGizmoPivotAndLocality();
				osd.UpdateOSDItem("Transform mode: Rotate", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
				string pivotmode = "Origin";
				string globalorlocal = "Global";
				transformGizmo.LocalTransform = (gizmoSpaceComboBox.SelectedIndex != 0);
				transformGizmo.Pivot = (pivotComboBox.SelectedIndex != 0) ? Pivot.Origin : Pivot.CenterOfMass;
				if (selectedItems.ItemCount > 0)
				{
					Item firstItem = selectedItems.Get(0);
					transformGizmo.SetGizmo(
						((transformGizmo.Pivot == Pivot.CenterOfMass) ? firstItem.Bounds.Center : firstItem.Position).ToVector3(),
						firstItem.TransformMatrix);
					transformGizmo.TransformGizmoMove(cam.mouseDelta, cam, selectedItems);
				}
				if (transformGizmo.Pivot == Pivot.CenterOfMass) pivotmode = "Center";
				if (transformGizmo.LocalTransform == true) globalorlocal = "Local";
				if (!silent) osd.UpdateOSDItem("Transform: " + globalorlocal + ", " + pivotmode, panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
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
				scaleModeButton.Checked = true;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				rotateModeButton.Checked = false;
				SetGizmoPivotAndLocality();
				osd.UpdateOSDItem("Transform mode: Scale", panel1.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				DrawLevel();
			}
		}
		#endregion

		private void calculateAllBoundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (LevelItem item in LevelData.LevelItems)
				item.CalculateBounds();
			osd.UpdateOSDItem("Calculated all bounds", panel1.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
			DrawLevel();
			unsaved = true;
		}

		private void ASSIMPExportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.obj;*.fbx;*.dae",
				FileName = "test"
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					Assimp.AssimpContext context = new Assimp.AssimpContext();
					Assimp.Scene scene = new Assimp.Scene();
					scene.Materials.Add(new Assimp.Material());
					Assimp.Node n = new Assimp.Node();
					n.Name = "RootNode";
					scene.RootNode = n;
					string rootPath = Path.GetDirectoryName(a.FileName);
					List<string> texturePaths = new List<string>();

					if (LevelData.TextureBitmaps != null)
					{
						foreach (BMPInfo[] bmp_ in LevelData.TextureBitmaps.Values) //???????
						{
							foreach (BMPInfo bmp in bmp_)
							{
								texturePaths.Add(Path.Combine(rootPath, bmp.Name + ".png"));
								bmp.Image.Save(Path.Combine(rootPath, bmp.Name + ".png"));
							}
						}
					}
					
					foreach (COL col in LevelData.geo.COL)
					{
						SAEditorCommon.Import.AssimpStuff.AssimpExport(col.Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
					}
						
					context.ExportFile(scene, a.FileName, "collada", Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//
				}
			}
		}

		private void welcomeTutorialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowWelcomeScreen();
		}

		private void loadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs" })
			{
				if (!string.IsNullOrEmpty(LevelData.geo.TextureFileName))
					a.FileName = LevelData.geo.TextureFileName + ".pvm";
				else
					a.FileName = string.Empty;
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					BMPInfo[] TexBmps = TextureArchive.GetTextures(a.FileName);
					Texture[] texs = new Texture[TexBmps.Length];
					for (int j = 0; j < TexBmps.Length; j++)
						texs[j] = TexBmps[j].Image.ToTexture(d3ddevice);
					string texname = Path.GetFileNameWithoutExtension(a.FileName);
					if (!LevelData.TextureBitmaps.ContainsKey(texname))
						LevelData.TextureBitmaps.Add(texname, TexBmps);
					if (!LevelData.Textures.ContainsKey(texname))
						LevelData.Textures.Add(texname, texs);
					LevelData.leveltexs = texname;
					unloadTexturesToolStripMenuItem.Enabled = true;
				}
				DrawLevel();
			}
		}

		private void unloadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.leveltexs = null;
			LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
			LevelData.Textures = new Dictionary<string, Texture[]>();
			DrawLevel();
		}

		private void MessageTimer_Tick(object sender, EventArgs e)
		{
			if (d3ddevice != null && osd != null)
				if (osd.UpdateTimer() == true) DrawLevel();
		}

		private void showHintsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			osd.show_osd = showHintsToolStripMenuItem.Checked;
			hintsButton.Checked = showHintsToolStripMenuItem.Checked;
		}

		private void hintsButton_Click(object sender, EventArgs e)
		{
			showHintsToolStripMenuItem.Checked = !showHintsToolStripMenuItem.Checked;
			hintsButton.Checked = showHintsToolStripMenuItem.Checked;
		}

		private void preferencesButton_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
		}

		private void Resize()
		{
			// Causes a memory leak so not used for now
			LevelData.Textures = new Dictionary<string, Texture[]>();
			SharpDX.Direct3D9.Direct3D d3d = new SharpDX.Direct3D9.Direct3D();
			d3ddevice = new Device(d3d, 0, DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing,
			new PresentParameters
			{
				Windowed = true,
				SwapEffect = SwapEffect.Discard,
				EnableAutoDepthStencil = true,
				AutoDepthStencilFormat = Format.D24X8
			});		
			EditorOptions.Initialize(d3ddevice);
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
			LevelData.InvalidateRenderState();
		}

		private void alternativeCameraModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			alternativeCameraModeToolStripMenuItem.Checked = !alternativeCameraModeToolStripMenuItem.Checked;
		}

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
		}
	}
}