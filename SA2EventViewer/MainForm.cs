using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BoundingSphere = SonicRetro.SAModel.BoundingSphere;
using Color = System.Drawing.Color;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;
using Point = System.Drawing.Point;

namespace SA2EventViewer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			AddMouseMoveHandler(this);
			Application.ThreadException += Application_ThreadException;
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
			File.WriteAllText("SA2EventViewer.log", e.Exception.ToString());
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SA2 Event Viewer Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
				Close();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("SA2EventViewer.log", e.ExceptionObject.ToString());
			MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SA2 Event Viewer Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(100000);
		EditorOptionsEditor optionsEditor;

		bool loaded;
		string currentFileName = "";
		Event @event;
		int scenenum = 0;
		int animframe = -1;
		decimal decframe = -1;
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

		#region UI
		bool lookKeyDown;
		bool zoomKeyDown;
		bool cameraKeyDown;

		int cameraMotionInterval = 1;

		ActionMappingList actionList;
		ActionInputCollector actionInputCollector;
		#endregion

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			Direct3D d3d = new Direct3D();
			d3ddevice = new Device(d3d, 0, DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing,
				new PresentParameters
				{
					Windowed = true,
					SwapEffect = SwapEffect.Discard,
					EnableAutoDepthStencil = true,
					AutoDepthStencilFormat = Format.D24X8
				});

			EditorOptions.Initialize(d3ddevice);
			EditorOptions.OverrideLighting = true;
			EditorOptions.RenderDrawDistance = 10000;

			actionList = ActionMappingList.Load(Path.Combine(Application.StartupPath, "keybinds", "SA2EventViewer.ini"),
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

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "Event Files|e????.prs|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					timer1.Stop();
					timer1.Enabled = false;
					scenenum = 0;
					decframe = 0;
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
			TextureInfo = TextureArchive.GetTextures(TexturePackName);

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
			Text = "SA2 Event Viewer: " + currentFileName;
			camModeLabel.Text = eventcamera ? "Event Cam" : "Free Cam";
			cameraPosLabel.Text = $"Camera Pos: {cam.Position}";
			cameraFOVLabel.Text = $"FOV: {cam.FOV}";
			sceneNumLabel.Text = $"Scene: {scenenum}";
			animFrameLabel.Text = $"Frame: {animframe}";
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded) return;
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH(cam.FOV, panel1.Width / (float)panel1.Height, 1, cam.DrawDistance));
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
			List<RenderInfo> renderList = new List<RenderInfo>();
			for (int i = 0; i < @event.Scenes[0].Entities.Count; i++)
			{
				NJS_OBJECT model = @event.Scenes[0].Entities[i].Model;
				if (model == null)
					model = @event.Scenes[0].Entities[i].GCModel;
				if (model != null)
					if (model.HasWeight)
					{
						renderList.AddRange(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[0][i]));
						if (@event.Scenes[0].Entities[i] == selectedObject)
							renderList.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes[0][i]));
					}
					else
					{
						renderList.AddRange(model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[0][i]));
						if (@event.Scenes[0].Entities[i] == selectedObject)
							renderList.AddRange(model.DrawModelTreeInvert(transform, meshes[0][i]));
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
							renderList.AddRange(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[scenenum][i]));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(model.DrawModelTreeWeightedInvert(transform.Top, meshes[scenenum][i]));
							if (animframe != -1 && @event.Scenes[scenenum].Entities[i].Motion != null)
								transform.Pop();
						}
						else if (animframe == -1 || @event.Scenes[scenenum].Entities[i].Motion == null)
						{
							renderList.AddRange(model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[scenenum][i]));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(model.DrawModelTreeInvert(transform, meshes[scenenum][i]));
						}
						else
						{
							renderList.AddRange(model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(model.DrawModelTreeAnimatedInvert(transform, meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe));
						}
				}
				if (@event.Scenes[scenenum].Big?.Model != null)
					if (@event.Scenes[scenenum].Big.Model.HasWeight)
						renderList.AddRange(@event.Scenes[scenenum].Big.Model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, bigmeshes[scenenum]));
					else if (animframe == -1)
						renderList.AddRange(@event.Scenes[scenenum].Big.Model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, bigmeshes[scenenum]));
					else
					{
						int an = 0;
						int fr = animframe;
						while (an < @event.Scenes[scenenum].Big.Motions.Count && @event.Scenes[scenenum].Big.Motions[an].a.Frames < fr)
						{
							fr -= @event.Scenes[scenenum].Big.Motions[an].a.Frames;
							an++;
						}
						if (an < @event.Scenes[scenenum].Big.Motions.Count)
							renderList.AddRange(@event.Scenes[scenenum].Big.Model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, bigmeshes[scenenum], @event.Scenes[scenenum].Big.Motions[an].a, fr));
					}
				if (!eventcamera && animframe != -1 && showCameraToolStripMenuItem.Checked)
				{
					transform.Push();
					transform.LoadMatrix(cammatrix);
					renderList.AddRange(cammodel.DrawModel(EditorOptions.RenderFillMode, transform, null, cammesh, true));
					transform.Pop();
				}
			}

			RenderInfo.Draw(renderList, d3ddevice, cam, true);
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
							int fr = animframe;
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
					int fr = animframe;
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
					cam.FOV = SonicRetro.SAModel.Direct3D.Extensions.BAMSToRad(@event.Scenes[scenenum].CameraMotions[an].Models[0].GetAngle(fr));
				}
				else
				{
					cam.mode = 0;
					cam.FOV = (float)(Math.PI / 4);
					if (animframe != -1 && @event.Scenes[scenenum].CameraMotions != null)
					{
						int an = 0;
						int fr = animframe;
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
						float bams_sin = SonicRetro.SAModel.Direct3D.Extensions.NJSin(roll);
						float bams_cos = SonicRetro.SAModel.Direct3D.Extensions.NJCos(-roll);
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

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			DrawEntireModel();
		}
		#endregion

		#region Keyboard/Mouse Methods

		private void ActionInputCollector_OnActionRelease(ActionInputCollector sender, string actionName)
		{
			if (!loaded)
				return;

			bool draw = false; // should the scene redraw after this action

			switch (actionName)
			{
				case ("Camera Mode"):
					eventcamera = !eventcamera;

					draw = true;
					break;

				case ("Zoom to target"):
					if (selectedObject != null)
					{
						BoundingSphere bounds = (selectedObject.Model?.Attach != null) ? selectedObject.Model.Attach.Bounds :
							new BoundingSphere(selectedObject.Position.X, selectedObject.Position.Y, selectedObject.Position.Z, 10);

						bounds.Center += selectedObject.Position;
						cam.MoveToShowBounds(bounds);
					}

					draw = true;
					break;

				case ("Change Render Mode"):
					if (EditorOptions.RenderFillMode == FillMode.Solid)
						EditorOptions.RenderFillMode = FillMode.Point;
					else
						EditorOptions.RenderFillMode += 1;

					draw = true;
					break;

				case ("Increase camera move speed"):
					cam.MoveSpeed += 0.0625f;
					//UpdateTitlebar();
					break;

				case ("Decrease camera move speed"):
					cam.MoveSpeed -= 0.0625f;
					//UpdateTitlebar();
					break;

				case ("Reset camera move speed"):
					cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
					//UpdateTitlebar();
					break;

				case ("Reset Camera Position"):
					if (!eventcamera)
					{
						cam.Position = new Vector3();
						draw = true;
					}
					break;

				case ("Reset Camera Rotation"):
					if (!eventcamera)
					{
						cam.Pitch = 0;
						cam.Yaw = 0;
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
					scenenum++;
					animframe = (timer1.Enabled ? 0 : -1);
					decframe = animframe;
					if (scenenum == @event.Scenes.Count)
					{
						if (timer1.Enabled)
							scenenum = 1;
						else
							scenenum = 0;
					}

					draw = true;
					break;

				case ("Previous Scene"):
					scenenum--;
					animframe = (timer1.Enabled ? 0 : -1);
					decframe = animframe;
					if (scenenum == -1 || (timer1.Enabled && scenenum == 0)) scenenum = @event.Scenes.Count - 1;

					draw = true;
					break;

				case ("Previous Frame"):
					if (scenenum > 0 && !timer1.Enabled)
					{
						animframe--;
						if (animframe < -1)
						{
							scenenum--;
							if (scenenum == 0)
								scenenum = @event.Scenes.Count - 1;
							animframe = @event.Scenes[scenenum].FrameCount - 1;
						}
						decframe = animframe;

						draw = true;
					}
					break;

				case ("Next Frame"):
					if (scenenum > 0 && !timer1.Enabled)
					{
						animframe++;
						if (animframe == @event.Scenes[scenenum].FrameCount)
						{
							scenenum++;
							if (scenenum == @event.Scenes.Count)
								scenenum = 1;
							animframe = -1;
						}
						decframe = animframe;

						draw = true;
					}
					break;

				case ("Play/Pause Animation"):
					if (!timer1.Enabled)
					{
						if (scenenum == 0)
							scenenum = 1;
						if (animframe == -1)
							decframe = animframe = 0;
					}
					timer1.Enabled = !timer1.Enabled;
					draw = true;
					break;

				default:
					break;
			}

			if (draw)
			{
				UpdateWeightedModels();
				DrawEntireModel();
			}
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
			bool mouseWrapScreen = false;
			int camresult = 0;
			if (!eventcamera || animframe == -1)
			{
				System.Drawing.Rectangle mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : panel1.RectangleToScreen(panel1.Bounds);
				camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, cameraKeyDown);
			}
			if (camresult >= 2 && selectedObject != null) propertyGrid1.Refresh();
			if (camresult >= 1)
			{
				UpdateWeightedModels();
				DrawEntireModel();
			}
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyUp(Keys.MButton);
		}
		#endregion

		private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			decframe += numericUpDown1.Value;
			int oldanimframe = animframe;
			animframe = (int)decframe;
			if (animframe != oldanimframe)
			{
				if (animframe < 0)
				{
					scenenum--;
					if (scenenum == 0)
						scenenum = @event.Scenes.Count - 1;
					animframe = @event.Scenes[scenenum].FrameCount - 1;
					decframe = animframe + 0.99m;
				}
				else if (animframe >= @event.Scenes[scenenum].FrameCount)
				{
					scenenum++;
					if (scenenum == @event.Scenes.Count)
						scenenum = 1;
					decframe = animframe = 0;
				}
				UpdateWeightedModels();
				DrawEntireModel();
			}
		}

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
				contextMenuStrip1.Show(panel1, e.Location);
		}

		internal void SelectedItemChanged()
		{
			if (selectedObject != null)
			{
				propertyGrid1.SelectedObject = selectedObject;
				exportSA2MDLToolStripMenuItem.Enabled = selectedObject.Model != null;
			}
			else
			{
				propertyGrid1.SelectedObject = null;
				exportSA2MDLToolStripMenuItem.Enabled = false;
			}

			DrawEntireModel();
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
			DrawEntireModel();
		}

		void optionsEditor_FormUpdatedKeys()
		{
			// Keybinds
			actionList.ActionKeyMappings.Clear();
			ActionKeyMapping[] newMappings = optionsEditor.GetActionkeyMappings();
			foreach (ActionKeyMapping mapping in newMappings) 
				actionList.ActionKeyMappings.Add(mapping);
			actionInputCollector.SetActions(newMappings);
			string saveControlsPath = Path.Combine(Application.StartupPath, "keybinds", "SA2EventViewer.ini");
			actionList.Save(saveControlsPath);
			// Other settings
			optionsEditor_FormUpdated();
		}

		private void showCameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawEntireModel();
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

		private void MainForm_Deactivate(object sender, EventArgs e)
		{
			if (actionInputCollector != null) actionInputCollector.ReleaseKeys();
		}
	}
}