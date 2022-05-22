using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using BoundingSphere = SAModel.BoundingSphere;
using Color = System.Drawing.Color;
using Mesh = SAModel.Direct3D.Mesh;
using Point = System.Drawing.Point;
using static SAModel.SAEditorCommon.SettingsFile;

namespace SA2EventViewer
{
	public partial class MainForm : Form
	{
		Settings_SA2EventViewer settingsfile; // For user editable settings
		Logger log = new Logger();
		bool FormResizing;
		bool Playing;
		bool NeedRedraw;
		FormWindowState LastWindowState = FormWindowState.Minimized;
		public MainForm()
		{
			InitializeComponent();
			AddMouseMoveHandler(this);
			Application.ThreadException += Application_ThreadException;
			Application.Idle += HandleWaitLoop;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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
			string logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SA2EventViewer.log");
			File.WriteAllText(logPath, e.Exception.ToString());
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved to:\n" + logPath + "\n\nDo you want to try to continue running?", "SA2 Event Viewer Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
				Close();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			string logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SA2EventViewer.log");
			File.WriteAllText(logPath, e.ExceptionObject.ToString());
			MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "SA2 Event Viewer Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		EditorOptionsEditor optionsEditor;

		bool loaded;
		bool DeviceResizing;
		string currentFileName = "";
		Event @event;
		int scenenum = 0;
		float animframe = -1;
		float nextframe = -1;
		float animspeed = 0.5f;
		List<List<Mesh[]>> meshes;
		List<Mesh[]> bigmeshes;
		NJS_OBJECT cammodel;
		Mesh cammesh;
		Matrix cammatrix;
		string TexturePackName;
		BMPInfo[] TextureInfo;
		Texture[] Textures;
		EventEntity selectedObject;
		bool eventcamera = true;
		OnScreenDisplay osd;

		#region UI
		bool lookKeyDown;
		bool zoomKeyDown;
		bool cameraKeyDown;

		//int cameraMotionInterval = 1;

		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;
		#endregion

		private void MainForm_Load(object sender, EventArgs e)
		{
			AnimationTimer = new HiResTimer(16.667f);
			AnimationTimer.Elapsed += new EventHandler<HiResTimerElapsedEventArgs>(AdvanceAnimation);
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
			settingsfile = Settings_SA2EventViewer.Load();
			EditorOptions.FillColor = settingsfile.BackgroundColor;
			EditorOptions.Initialize(d3ddevice);
			EditorOptions.RenderDrawDistance = cam.DrawDistance = settingsfile.DrawDistanceGeneral;
			cam.ModifierKey = settingsfile.CameraModifier;
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
			actionList = ActionMappingList.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SA2EventViewer_keys.ini"),
				DefaultActionList.DefaultActionMapping);

			actionInputCollector = new ActionInputCollector();
			actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			actionInputCollector.OnActionStart += ActionInputCollector_OnActionStart;
			actionInputCollector.OnActionRelease += ActionInputCollector_OnActionRelease;

			optionsEditor = new EditorOptionsEditor(cam, actionList.ActionKeyMappings.ToArray(), DefaultActionList.DefaultActionMapping, false, false);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.FormUpdatedKeys += optionsEditor_FormUpdatedKeys;

			cammodel = new ModelFile(Properties.Resources.camera).Model;
			cammodel.Attach.ProcessVertexData();
			cammesh = cammodel.Attach.CreateD3DMesh();

			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);
		}

		protected override void WndProc(ref Message m)
		{
			// Suppress the WM_UPDATEUISTATE message to remove rendering flicker
			if (m.Msg == 0x128) return;
			base.WndProc(ref m);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "Event Files|e????.prs;e????.bin|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					Playing = false;
					scenenum = 0;
					nextframe = 0;
					AnimationTimer.Stop();
					animframe = -1;
					LoadFile(a.FileName);
				}
		}

		private void LoadFile(string filename)
		{
			loaded = false;
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			@event = new Event(filename);
			meshes = new List<List<Mesh[]>>();
			bigmeshes = new List<Mesh[]>();
			buttonNextFrame.Enabled = buttonPreviousFrame.Enabled = buttonNextScene.Enabled = buttonPrevScene.Enabled = buttonPlayScene.Enabled = true;
			foreach (EventScene scene in @event.Scenes)
			{
				List<Mesh[]> scenemeshes = new List<Mesh[]>();
				foreach (EventEntity entity in scene.Entities)
				{
					if (entity.Model != null)
						if (entity.Model.HasWeight)
							scenemeshes.Add(entity.Model.ProcessWeightedModel().ToArray());
						else
						{
							entity.Model.ProcessVertexData();
							NJS_OBJECT[] models = entity.Model.GetObjects();
							Mesh[] entmesh = new Mesh[models.Length];
							for (int i = 0; i < models.Length; i++)
								if (models[i].Attach != null)
									try { entmesh[i] = models[i].Attach.CreateD3DMesh(); }
									catch { }
							scenemeshes.Add(entmesh);
						}
					else if (entity.GCModel != null)
					{
						entity.GCModel.ProcessVertexData();
						NJS_OBJECT[] models = entity.GCModel.GetObjects();
						Mesh[] entmesh = new Mesh[models.Length];
						for (int i = 0; i < models.Length; i++)
							if (models[i].Attach != null)
								try { entmesh[i] = models[i].Attach.CreateD3DMesh(); }
								catch { }
						scenemeshes.Add(entmesh);
					}
					else
						scenemeshes.Add(null);
				}
				meshes.Add(scenemeshes);
				if (scene.Big?.Model != null)
				{
					if (scene.Big.Model.HasWeight)
						bigmeshes.Add(scene.Big.Model.ProcessWeightedModel().ToArray());
					else
					{
						scene.Big.Model.ProcessVertexData();
						NJS_OBJECT[] models = scene.Big.Model.GetObjects();
						Mesh[] entmesh = new Mesh[models.Length];
						for (int i = 0; i < models.Length; i++)
							if (models[i].Attach != null)
								try { entmesh[i] = models[i].Attach.CreateD3DMesh(); }
								catch { }
						bigmeshes.Add(entmesh);
					}
				}
				else
					bigmeshes.Add(null);
			}
			TexturePackName = Path.GetFileNameWithoutExtension(filename) + "texture.prs";
			if (!File.Exists(TexturePackName))
				TexturePackName = Path.GetFileNameWithoutExtension(filename) + ".pvm";
			TextureInfo = TextureArchive.GetTextures(TexturePackName, out bool hasNames);

			Textures = new Texture[TextureInfo.Length];
			for (int j = 0; j < TextureInfo.Length; j++)
				Textures[j] = TextureInfo[j].Image.ToTexture(d3ddevice);

			loaded = true;
			selectedObject = null;
			SelectedItemChanged();

			currentFileName = filename;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void UpdateStatusString()
		{
			string cameramode = "Move";
			string look = "Look";
			string zoom = "Zoom";
			Text = "SA2 Event Viewer: " + currentFileName;
			camModeLabel.Text = eventcamera ? "Event Cam" : "Free Cam";
			cameraPosLabel.Text = $"Camera Pos: {cam.Position}";
			cameraFOVLabel.Text = $"FOV: {cam.FOV}";
			sceneNumLabel.Text = $"Scene: {scenenum}";
			animFrameLabel.Text = $"Frame: {animframe}";
			if (showAdvancedCameraInfoToolStripMenuItem.Checked)
			{
				if (lookKeyDown) cameramode = look;
				if (zoomKeyDown) cameramode = zoom;
				cameraAngleLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
				camModeLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
				cameraAngleLabel.Text = $"Pitch: " + cam.Pitch.ToString("X5") + " Yaw: " + cam.Yaw.ToString("X5") + (cam.mode == 1 ? " Distance: " + cam.Distance : "");
				camModeLabel.Text = $"Mode: " + cameramode + ", Speed: " + cam.MoveSpeed;
			}
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded || DeviceResizing) return;
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH(cam.FOV, RenderPanel.Width / (float)RenderPanel.Height, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformState.View, cam.ToMatrix());
			UpdateStatusString();
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
			List<RenderInfo> renderList = new List<RenderInfo>();
			for (int i = 0; i < @event.Scenes[0].Entities.Count; i++)
			{
				NJS_OBJECT model = @event.Scenes[0].Entities[i].Model;
				if (model == null)
					model = @event.Scenes[0].Entities[i].GCModel;
				if (model != null)
					if (model.HasWeight)
					{
						renderList.AddRange(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[0][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
						if (@event.Scenes[0].Entities[i] == selectedObject)
							renderList.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes[0][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
					}
					else
					{
						renderList.AddRange(model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[0][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
						if (@event.Scenes[0].Entities[i] == selectedObject)
							renderList.AddRange(model.DrawModelTreeInvert(transform, meshes[0][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
					}
			}
			if (scenenum > 0)
			{
				for (int i = 0; i < @event.Scenes[scenenum].Entities.Count; i++)
				{
					NJS_OBJECT model = @event.Scenes[scenenum].Entities[i].Model;
					if (model == null)
						model = @event.Scenes[scenenum].Entities[i].GCModel;
					if (model != null)
						if (model.HasWeight)
						{
							if (animframe != -1 && @event.Scenes[scenenum].Entities[i].Motion != null)
							{
								transform.Push();
								transform.NJTranslate(@event.Scenes[scenenum].Entities[i].Motion.Models[0].GetPosition(animframe));
							}
							renderList.AddRange(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[scenenum][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes[scenenum][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
							if (animframe != -1 && @event.Scenes[scenenum].Entities[i].Motion != null)
								transform.Pop();
						}
						else if (animframe == -1 || @event.Scenes[scenenum].Entities[i].Motion == null)
						{
							renderList.AddRange(model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[scenenum][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(model.DrawModelTreeInvert(transform, meshes[scenenum][i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
						}
						else
						{
							renderList.AddRange(model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(model.DrawModelTreeAnimatedInvert(transform, meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe));
						}
				}
				if (@event.Scenes[scenenum].Big?.Model != null)
					if (@event.Scenes[scenenum].Big.Model.HasWeight)
						renderList.AddRange(@event.Scenes[scenenum].Big.Model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, bigmeshes[scenenum], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
					else if (animframe == -1)
						renderList.AddRange(@event.Scenes[scenenum].Big.Model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, bigmeshes[scenenum], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
					else
					{
						int an = 0;
						float fr = animframe;
						while (an < @event.Scenes[scenenum].Big.Motions.Count && @event.Scenes[scenenum].Big.Motions[an].a.Frames < fr)
						{
							fr -= @event.Scenes[scenenum].Big.Motions[an].a.Frames;
							an++;
						}
						if (an < @event.Scenes[scenenum].Big.Motions.Count)
							renderList.AddRange(@event.Scenes[scenenum].Big.Model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, bigmeshes[scenenum], @event.Scenes[scenenum].Big.Motions[an].a, fr, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
					}
				if (!eventcamera && animframe != -1 && showCameraToolStripMenuItem.Checked)
				{
					transform.Push();
					transform.LoadMatrix(cammatrix);
					renderList.AddRange(cammodel.DrawModel(EditorOptions.RenderFillMode, transform, null, cammesh, true, false, true));
					transform.Pop();
				}
			}
			RenderInfo.Draw(renderList, d3ddevice, cam, true);
			osd.ProcessMessages();
			d3ddevice.EndScene(); //all drawings before this line
			d3ddevice.Present();
		}

		private void UpdateWeightedModels()
		{
			if (scenenum > 0)
			{
				for (int i = 0; i < @event.Scenes[scenenum].Entities.Count; i++)
					if (@event.Scenes[scenenum].Entities[i].Model != null)
						if (@event.Scenes[scenenum].Entities[i].Model.HasWeight)
						{
							if (animframe == -1 || @event.Scenes[scenenum].Entities[i].Motion == null)
								@event.Scenes[scenenum].Entities[i].Model.UpdateWeightedModel(new MatrixStack(), meshes[scenenum][i]);
							else
							{
								MatrixStack m = new MatrixStack();
								m.NJTranslate(-@event.Scenes[scenenum].Entities[i].Motion.Models[0].GetPosition(animframe));
								@event.Scenes[scenenum].Entities[i].Model.UpdateWeightedModelAnimated(m, @event.Scenes[scenenum].Entities[i].Motion, animframe, meshes[scenenum][i]);
							}
						}
						else if (@event.Scenes[scenenum].Entities[i].ShapeMotion != null)
						{
							if (animframe == -1)
								@event.Scenes[scenenum].Entities[i].Model.ProcessVertexData();
							else
								@event.Scenes[scenenum].Entities[i].Model.ProcessShapeMotionVertexData(@event.Scenes[scenenum].Entities[i].ShapeMotion, animframe);
							NJS_OBJECT[] models = @event.Scenes[scenenum].Entities[i].Model.GetObjects();
							for (int j = 0; j < models.Length; j++)
								if (models[j].Attach != null)
									try { meshes[scenenum][i][j] = models[j].Attach.CreateD3DMesh(); }
									catch { }
						}
				if (@event.Scenes[scenenum].Big?.Model != null)
					if (@event.Scenes[scenenum].Big.Model.HasWeight)
					{
						if (animframe == -1)
							@event.Scenes[scenenum].Big.Model.UpdateWeightedModel(new MatrixStack(), bigmeshes[scenenum]);
						else
						{
							int an = 0;
							float fr = animframe;
							while (an < @event.Scenes[scenenum].Big.Motions.Count && @event.Scenes[scenenum].Big.Motions[an].a.Frames < fr)
							{
								fr -= @event.Scenes[scenenum].Big.Motions[an].a.Frames;
								an++;
							}
							if (an < @event.Scenes[scenenum].Big.Motions.Count)
								@event.Scenes[scenenum].Big.Model.UpdateWeightedModelAnimated(new MatrixStack(), @event.Scenes[scenenum].Big.Motions[an].a, fr, bigmeshes[scenenum]);
						}
					}
				if (eventcamera && animframe != -1 && @event.Scenes[scenenum].CameraMotions != null)
				{
					cam.mode = 2;
					int an = 0;
					float fr = animframe;
					while (@event.Scenes[scenenum].CameraMotions[an].Frames < fr)
					{
						fr -= @event.Scenes[scenenum].CameraMotions[an].Frames;
						an++;
					}
					AnimModelData data = @event.Scenes[scenenum].CameraMotions[an].Models[0];
					cam.Position = data.GetPosition(fr).ToVector3();
					Vector3 dir;
					if (data.Vector.Count > 0)
						dir = data.GetVector(fr).ToVector3();
					else
						dir = Vector3.Normalize(cam.Position - data.GetTarget(fr).ToVector3());
					cam.Direction = dir;
					cam.Roll = data.GetRoll(fr);
					cam.FOV = SAModel.Direct3D.Extensions.BAMSToRad(@event.Scenes[scenenum].CameraMotions[an].Models[0].GetAngle(fr));
				}
				else
				{
					cam.mode = 0;
					cam.FOV = (float)(Math.PI / 4);
					if (animframe != -1 && @event.Scenes[scenenum].CameraMotions != null)
					{
						int an = 0;
						float fr = animframe;
						while (@event.Scenes[scenenum].CameraMotions[an].Frames < fr)
						{
							fr -= @event.Scenes[scenenum].CameraMotions[an].Frames;
							an++;
						}
						AnimModelData data = @event.Scenes[scenenum].CameraMotions[an].Models[0];
						Vector3 pos = data.GetPosition(fr).ToVector3();
						Vector3 dir;
						if (data.Vector.Count > 0)
							dir = data.GetVector(fr).ToVector3();
						else
							dir = Vector3.Normalize(pos - data.GetTarget(fr).ToVector3());
						int roll = data.GetRoll(fr);
						float bams_sin = SAModel.Direct3D.Extensions.NJSin(roll);
						float bams_cos = SAModel.Direct3D.Extensions.NJCos(-roll);
						float thing = dir.X * dir.X + dir.Z * dir.Z;
						double sqrt = Math.Sqrt(thing);
						float v3 = dir.Y * dir.Y + thing;
						double v4 = 1.0 / Math.Sqrt(v3);
						double sqrt__ = sqrt * v4;
						double sqrt___ = v4 * dir.Y;
						double v7, v8;
						if (thing <= 0.000001)
						{
							v7 = 1.0;
							v8 = 0.0;
						}
						else
						{
							double v5 = 1.0 / Math.Sqrt(thing);
							double v6 = v5;
							v7 = v5 * dir.Z;
							v8 = -(v6 * dir.X);
						}
						double v9 = sqrt___ * v8;
						cammatrix.M14 = 0;
						cammatrix.M23 = (float)sqrt___;
						cammatrix.M24 = 0;
						cammatrix.M34 = 0;
						cammatrix.M11 = (float)(v7 * bams_cos - v9 * bams_sin);
						cammatrix.M12 = (float)(v9 * bams_cos + v7 * bams_sin);
						cammatrix.M13 = -(float)(sqrt__ * v8);
						cammatrix.M21 = -(float)(sqrt__ * bams_sin);
						cammatrix.M22 = (float)(sqrt__ * bams_cos);
						double v10 = v7 * sqrt___;
						cammatrix.M31 = (float)(bams_sin * v10 + v8 * bams_cos);
						cammatrix.M32 = (float)(v8 * bams_sin - v10 * bams_cos);
						cammatrix.M33 = (float)(v7 * sqrt__);
						cammatrix.M41 = -(cammatrix.M31 * pos.Z) - cammatrix.M11 * pos.X - cammatrix.M21 * pos.Y;
						cammatrix.M42 = -(cammatrix.M32 * pos.Z) - cammatrix.M12 * pos.X - cammatrix.M22 * pos.Y;
						float v12 = -(cammatrix.M33 * pos.Z) - cammatrix.M13 * pos.X;
						double v13 = sqrt___ * pos.Y;
						cammatrix.M44 = 1;
						cammatrix.M43 = (float)(v12 - v13);
						cammatrix.Invert();
					}
				}
			}
		}

		#endregion

		#region Keyboard/Mouse Methods
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
			bool mouseWrapScreen = false;
			EditorCamera.CameraUpdateFlags camresult = EditorCamera.CameraUpdateFlags.None;
			if (!eventcamera || animframe == -1)
			{
				System.Drawing.Rectangle mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
				camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, cameraKeyDown);
			}
			if (camresult.HasFlag(EditorCamera.CameraUpdateFlags.RefreshControls) && selectedObject != null)
				propertyGrid1.Refresh();
			if (camresult.HasFlag(EditorCamera.CameraUpdateFlags.Redraw))
				NeedRedraw = true;
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) 
				actionInputCollector.KeyUp(Keys.MButton);
		}
		#endregion

		private void NextAnimation()
		{
			scenenum++;
			animframe = (Playing ? 0 : -1);
			nextframe = animframe;
			if (scenenum == @event.Scenes.Count)
			{
				if (Playing)
					scenenum = 1;
				else
					scenenum = 0;
			}
			if (scenenum == 0)
			{
				osd.UpdateOSDItem("Default Scene", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonNextFrame.Enabled = false;
				buttonPreviousFrame.Enabled = false;
			}
			else
			{
				osd.UpdateOSDItem("Scene " + scenenum.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonNextFrame.Enabled = true;
				buttonPreviousFrame.Enabled = true;
			}
			NeedRedraw = true;
		}

		private void PreviousAnimation()
		{
			scenenum--;
			animframe = (Playing ? 0 : -1);
			nextframe = animframe;
			if (scenenum == -1 || (Playing && scenenum == 0)) scenenum = @event.Scenes.Count - 1;
			if (scenenum == 0)
			{
				osd.UpdateOSDItem("Default Scene", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonNextFrame.Enabled = false;
				buttonPreviousFrame.Enabled = false;
			}
			else
			{
				osd.UpdateOSDItem("Scene " + scenenum.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonNextFrame.Enabled = true;
				buttonPreviousFrame.Enabled = true;
			}
			NeedRedraw = true;
		}

		private void PreviousFrame()
		{
			if (scenenum > 0)
			{
				animframe = (float)Math.Floor(animframe - 1);
				if (animframe <= -1)
				{
					scenenum--;
					if (scenenum == 0)
						scenenum = @event.Scenes.Count - 1;
					animframe = @event.Scenes[scenenum].FrameCount - 1;
				}
				nextframe = animframe;
				AnimationTimer.Stop();
				Playing = false;
				osd.UpdateOSDItem("Animation frame: " + animframe.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				NeedRedraw = true;
			}
		}

		private void NextFrame()
		{
			if (scenenum > 0)
			{
				animframe = (float)Math.Floor(animframe + 1);
				if (animframe == @event.Scenes[scenenum].FrameCount)
				{
					scenenum++;
					if (scenenum == @event.Scenes.Count)
						scenenum = 1;
					animframe = -1;
				}
				nextframe = animframe;
				AnimationTimer.Stop();
				Playing = false;
				osd.UpdateOSDItem("Animation frame: " + animframe.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				NeedRedraw = true;	
			}
		}

		private void PlayPause()
		{
			if (!Playing)
			{
				if (scenenum == 0)
					scenenum = 1;
				if (animframe == -1)
					nextframe = animframe = 0;
			}
			Playing = !Playing;
			if (Playing)
			{
				osd.UpdateOSDItem("Play animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayScene.Checked = true;
				AnimationTimer.Start();
			}
			else
			{
				osd.UpdateOSDItem("Stop animation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
				buttonPlayScene.Checked = false;
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
					eventcamera = !eventcamera;
					string cammode = "Event Cam";
					if (!eventcamera) cammode = "Free Cam";
					osd.UpdateOSDItem("Camera mode: " + cammode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					draw = true;
					break;

				case ("Zoom to Target"):
					if (selectedObject != null)
					{
						BoundingSphere bounds = (selectedObject.Model?.Attach != null) ? selectedObject.Model.Attach.Bounds :
							new BoundingSphere(selectedObject.Position.X, selectedObject.Position.Y, selectedObject.Position.Z, 10);

						bounds.Center += selectedObject.Position;
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

				case ("Increase Camera Speed"):
					cam.MoveSpeed += 0.0625f;
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					//UpdateTitlebar();
					UpdateStatusString();
					break;

				case ("Decrease Camera Speed"):
					cam.MoveSpeed = Math.Max(cam.MoveSpeed - 0.0625f, 0.0625f);
					osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					//UpdateTitlebar();
					UpdateStatusString();
					break;

				case ("Reset Camera Speed"):
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					osd.UpdateOSDItem("Reset camera speed", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
					//UpdateTitlebar();
					UpdateStatusString();
					break;

				case ("Reset Camera Position"):
					if (!eventcamera)
					{
						cam.Position = new Vector3();
						osd.UpdateOSDItem("Reset camera position", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
						draw = true;
					}
					break;

				case ("Reset Camera Rotation"):
					if (!eventcamera)
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

				case ("Next Scene"):
					NextAnimation();
					break;

				case ("Previous Scene"):
					PreviousAnimation();
					break;

				case ("Previous Frame"):
					PreviousFrame();
					break;

				case ("Next Frame"):
					NextFrame();
					break;

				case ("Play/Pause Animation"):
					PlayPause();
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
					break;

				case ("Camera Zoom"):
					zoomKeyDown = true;
					break;

				case ("Camera Look"):
					lookKeyDown = true;
					break;

				case ("Play Animation (Hold)"):
					if (scenenum == 0)
						scenenum = 1;
					PlayAnimationHold();
					break;

				case ("Play Animation in Reverse (Hold)"):
					PlayAnimationHold(true);
					if (scenenum == 0)
						scenenum = 1;
					break;

				default:
					break;
			}
		}

		// Increase or decrease animation frame on key held
		private void PlayAnimationHold(bool negative = false)
		{
			if (!loaded)
				return;
			if (negative)
				nextframe -= animspeed;
			else
				nextframe += animspeed;
			NeedUpdateAnimation = true;
			NeedRedraw = true;
		}

		// Increate or decrease animation frame using the timer

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;

			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyDown(Keys.MButton);

			if (e.Button == MouseButtons.Left)
			{
				HitResult dist = HitResult.NoHit;
				EventEntity entity = null;
				Vector3 mousepos = new Vector3(e.X, e.Y, 0);
				Viewport viewport = d3ddevice.Viewport;
				Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
				Matrix view = d3ddevice.GetTransform(TransformState.View);
				Vector3 Near, Far;
				Near = mousepos;
				Near.Z = 0;
				Far = Near;
				Far.Z = -1;
				for (int i = 0; i < @event.Scenes[0].Entities.Count; i++)
				{
					if (@event.Scenes[0].Entities[i].Model != null)
					{
						HitResult hit;
						if (@event.Scenes[0].Entities[i].Model.HasWeight)
							hit = @event.Scenes[0].Entities[i].Model.CheckHitWeighted(Near, Far, viewport, proj, view, Matrix.Identity, meshes[0][i]);
						else
							hit = @event.Scenes[0].Entities[i].Model.CheckHit(Near, Far, viewport, proj, view, new MatrixStack(), meshes[0][i]);
						if (hit < dist)
						{
							dist = hit;
							entity = @event.Scenes[0].Entities[i];
						}
					}
					if (@event.Scenes[0].Entities[i].GCModel != null)
					{
						HitResult hit;
						hit = @event.Scenes[0].Entities[i].GCModel.CheckHit(Near, Far, viewport, proj, view, new MatrixStack(), meshes[0][i]);
						if (hit < dist)
						{
							dist = hit;
							entity = @event.Scenes[0].Entities[i];
						}
					}
				}
				if (scenenum > 0)
					for (int i = 0; i < @event.Scenes[scenenum].Entities.Count; i++)
					{
						if (@event.Scenes[scenenum].Entities[i].Model != null)
						{
							HitResult hit;
							if (@event.Scenes[scenenum].Entities[i].Model.HasWeight)
								hit = @event.Scenes[scenenum].Entities[i].Model.CheckHitWeighted(Near, Far, viewport, proj, view, Matrix.Identity, meshes[scenenum][i]);
							else if (animframe == -1 || @event.Scenes[scenenum].Entities[i].Motion == null)
								hit = @event.Scenes[scenenum].Entities[i].Model.CheckHit(Near, Far, viewport, proj, view, new MatrixStack(), meshes[scenenum][i]);
							else
								hit = @event.Scenes[scenenum].Entities[i].Model.CheckHitAnimated(Near, Far, viewport, proj, view, new MatrixStack(), meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe);
							if (hit < dist)
							{
								dist = hit;
								entity = @event.Scenes[scenenum].Entities[i];
							}
						}
					}
				if (dist.IsHit)
				{
					selectedObject = entity;
					SelectedItemChanged();
				}
				else
				{
					selectedObject = null;
					SelectedItemChanged();
				}
			}

			if (e.Button == MouseButtons.Right)
				contextMenuStrip1.Show(RenderPanel, e.Location);
		}

		internal void SelectedItemChanged()
		{
			if (selectedObject != null)
			{
				propertyGrid1.SelectedObject = selectedObject;
				exportSA2MDLToolStripMenuItem.Enabled = selectedObject.Model != null;
				exportSA2BMDLToolStripMenuItem.Enabled = selectedObject.GCModel != null;
			}
			else
			{
				propertyGrid1.SelectedObject = null;
				exportSA2MDLToolStripMenuItem.Enabled = false;
				exportSA2BMDLToolStripMenuItem.Enabled = false;
			}

			NeedRedraw = true;
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			optionsEditor.Show();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
		}

		void optionsEditor_FormUpdated()
		{
			settingsfile.CameraModifier = cam.ModifierKey;
			settingsfile.DrawDistanceGeneral = EditorOptions.RenderDrawDistance;
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

		void optionsEditor_FormUpdatedKeys()
		{
			// Keybinds
			actionList.ActionKeyMappings.Clear();
			ActionKeyMapping[] newMappings = optionsEditor.GetActionkeyMappings();
			foreach (ActionKeyMapping mapping in newMappings)
				actionList.ActionKeyMappings.Add(mapping);
			actionInputCollector.SetActions(newMappings);
			string saveControlsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SA2EventViewer_keys.ini");
			actionList.Save(saveControlsPath);
			// Other settings
			optionsEditor_FormUpdated();
		}

		private void showCameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NeedRedraw = true;
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

		private void buttonOpen_Click(object sender, EventArgs e)
		{
			openToolStripMenuItem_Click(sender, e);
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

		private void buttonPrevScene_Click(object sender, EventArgs e)
		{
			PreviousAnimation();
		}

		private void buttonNextScene_Click(object sender, EventArgs e)
		{
			NextAnimation();
		}

		private void buttonPrevFrame_Click(object sender, EventArgs e)
		{
			PreviousFrame();
		}

		private void buttonPlayScene_Click(object sender, EventArgs e)
		{
			PlayPause();
		}

		private void buttonNextFrame_Click(object sender, EventArgs e)
		{
			NextFrame();
		}

		private void buttonMaterialColors_CheckedChanged(object sender, EventArgs e)
		{
			string showmatcolors = "On";
			EditorOptions.IgnoreMaterialColors = !buttonMaterialColors.Checked;
			if (EditorOptions.IgnoreMaterialColors) showmatcolors = "Off";
			osd.UpdateOSDItem("Material Colors: " + showmatcolors, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
			NeedRedraw = true;
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

		private void exportSA2MDLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "sa2mdl", Filter = "SA2MDL files|*.sa2mdl" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					List<string> anims = new List<string>();
					if (selectedObject.Motion != null)
					{
						string animname = Path.GetFileNameWithoutExtension(dlg.FileName) + "_sklmtn.saanim";
						selectedObject.Motion.Save(Path.Combine(Path.GetDirectoryName(dlg.FileName), animname));
						anims.Add(animname);
					}
					if (selectedObject.ShapeMotion != null)
					{
						string animname = Path.GetFileNameWithoutExtension(dlg.FileName) + "_shpmtn.saanim";
						selectedObject.ShapeMotion.Save(Path.Combine(Path.GetDirectoryName(dlg.FileName), animname));
						anims.Add(animname);
					}
					ModelFile.CreateFile(dlg.FileName, selectedObject.Model, anims.ToArray(), null, null, null, ModelFormat.Chunk);
				}
		}
		private void exportSA2BMDLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "sa2bmdl", Filter = "SA2BMDL files|*.sa2bmdl" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					ModelFile.CreateFile(dlg.FileName, selectedObject.GCModel, null, null, null, null, ModelFormat.GC);
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			FormResizing = false;
			DeviceReset();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			NeedRedraw = true;
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
		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				settingsfile.Save();
			}
			catch { };
			AnimationTimer.Stop();
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

		private void numericUpDown1_ValueChanged(Object sender, EventArgs e)
		{
			animspeed = (float)numericUpDown1.Value / 2.0f;
		}

		private void MessageTimer_Tick(object sender, EventArgs e)
		{
			if (osd != null && osd.UpdateTimer()) NeedRedraw = true;
		}
	}
}