using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.UI;
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
			Application.ThreadException += Application_ThreadException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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
		EditorCamera cam = new EditorCamera(50000);
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
			optionsEditor = new EditorOptionsEditor(cam);
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
			optionsEditor.CustomizeKeybindsCommand += CustomizeControls;
			optionsEditor.ResetDefaultKeybindsCommand += () =>
			{
				actionList.ActionKeyMappings.Clear();

				foreach (ActionKeyMapping keymapping in DefaultActionList.DefaultActionMapping)
				{
					actionList.ActionKeyMappings.Add(keymapping);
				}

				actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			};

			actionList = ActionMappingList.Load(Path.Combine(Application.StartupPath, "keybinds.ini"),
				DefaultActionList.DefaultActionMapping);

			actionInputCollector = new ActionInputCollector();
			actionInputCollector.SetActions(actionList.ActionKeyMappings.ToArray());
			actionInputCollector.OnActionStart += ActionInputCollector_OnActionStart;
			actionInputCollector.OnActionRelease += ActionInputCollector_OnActionRelease;

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
					LoadFile(a.FileName);
		}

		private void LoadFile(string filename)
		{
			loaded = false;
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			timer1.Stop();
			scenenum = 0;
			animframe = -1;
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
			sceneNumLabel.Text = $"Scene: {scenenum}";
			animFrameLabel.Text = $"Frame: {animframe}";
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded) return;
			float fov = (float)(Math.PI / 4);
			if (eventcamera && animframe != -1 && @event.Scenes[scenenum].CameraMotions != null)
			{
				int an = 0;
				int fr = animframe;
				while (@event.Scenes[scenenum].CameraMotions[an].Frames < fr)
				{
					fr -= @event.Scenes[scenenum].CameraMotions[an].Frames;
					an++;
				}
				fov = SonicRetro.SAModel.Direct3D.Extensions.BAMSToRad(@event.Scenes[scenenum].CameraMotions[an].Models[0].GetAngle(fr));
			}
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH(fov, panel1.Width / (float)panel1.Height, 1, cam.DrawDistance));
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
				if (@event.Scenes[0].Entities[i].Model != null)
					if (@event.Scenes[0].Entities[i].Model.HasWeight)
					{
						renderList.AddRange(@event.Scenes[0].Entities[i].Model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[0][i]));
						if (@event.Scenes[0].Entities[i] == selectedObject)
							renderList.AddRange(@event.Scenes[0].Entities[i].Model.DrawModelTreeWeightedInvert(transform.Top, meshes[0][i]));
					}
					else
					{
						renderList.AddRange(@event.Scenes[0].Entities[i].Model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[0][i]));
						if (@event.Scenes[0].Entities[i] == selectedObject)
							renderList.AddRange(@event.Scenes[0].Entities[i].Model.DrawModelTreeInvert(transform, meshes[0][i]));
					}
			}
			if (scenenum > 0)
			{
				for (int i = 0; i < @event.Scenes[scenenum].Entities.Count; i++)
				{
					if (@event.Scenes[scenenum].Entities[i].Model != null)
						if (@event.Scenes[scenenum].Entities[i].Model.HasWeight)
						{
							renderList.AddRange(@event.Scenes[scenenum].Entities[i].Model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[scenenum][i]));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(@event.Scenes[scenenum].Entities[i].Model.DrawModelTreeWeightedInvert(transform.Top, meshes[scenenum][i]));
						}
						else if (animframe == -1 || @event.Scenes[scenenum].Entities[i].Motion == null)
						{
							renderList.AddRange(@event.Scenes[scenenum].Entities[i].Model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[scenenum][i]));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(@event.Scenes[scenenum].Entities[i].Model.DrawModelTreeInvert(transform, meshes[scenenum][i]));
						}
						else
						{
							renderList.AddRange(@event.Scenes[scenenum].Entities[i].Model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe));
							if (@event.Scenes[scenenum].Entities[i] == selectedObject)
								renderList.AddRange(@event.Scenes[scenenum].Entities[i].Model.DrawModelTreeAnimatedInvert(transform, meshes[scenenum][i], @event.Scenes[scenenum].Entities[i].Motion, animframe));
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

			RenderInfo.Draw(renderList, d3ddevice, cam);
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
								@event.Scenes[scenenum].Entities[i].Model.UpdateWeightedModelAnimated(new MatrixStack(), @event.Scenes[scenenum].Entities[i].Motion, animframe, meshes[scenenum][i]);
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
				}
				else
				{
					cam.mode = 0;
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
		void CustomizeControls()
		{
			ActionKeybindEditor editor = new ActionKeybindEditor(actionList.ActionKeyMappings.ToArray());

			editor.ShowDialog();

			// copy all our mappings back
			actionList.ActionKeyMappings.Clear();

			ActionKeyMapping[] newMappings = editor.GetActionkeyMappings();
			foreach (ActionKeyMapping mapping in newMappings) actionList.ActionKeyMappings.Add(mapping);

			actionInputCollector.SetActions(newMappings);

			// save our controls
			string saveControlsPath = Path.Combine(Application.StartupPath, "keybinds.ini");

			actionList.Save(saveControlsPath);

			this.BringToFront();
			optionsEditor.BringToFront();
			optionsEditor.Focus();
			//this.panel1.Focus();
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

		Point mouseLast;
		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;

			#region Motion Handling
			Point mouseEvent = e.Location;
			if (mouseLast == Point.Empty)
			{
				mouseLast = mouseEvent;
				return;
			}

			bool mouseWrapScreen = true;
			ushort mouseWrapThreshold = 2;

			Point mouseDelta = mouseEvent - (Size)mouseLast;
			bool performedWrap = false;

			if (e.Button != MouseButtons.None)
			{
				System.Drawing.Rectangle mouseBounds = (mouseWrapScreen) ? Screen.GetBounds(ClientRectangle) : panel1.RectangleToScreen(panel1.Bounds);

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
			#endregion

			if (cameraKeyDown && (!eventcamera || animframe == -1))
			{
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

				UpdateWeightedModels();
				DrawEntireModel();
			}

			if (performedWrap || Math.Abs(mouseDelta.X / 2) * cam.MoveSpeed > 0 || Math.Abs(mouseDelta.Y / 2) * cam.MoveSpeed > 0)
			{
				mouseLast = mouseEvent;
				if (e.Button != MouseButtons.None && selectedObject != null) propertyGrid1.Refresh();

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
				//exportOBJToolStripMenuItem.Enabled = selectedObject.Model != null;
			}
			else
			{
				propertyGrid1.SelectedObject = null;
				exportOBJToolStripMenuItem.Enabled = false;
			}

			DrawEntireModel();
		}

		private void objToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj"
			})
				if (a.ShowDialog() == DialogResult.OK)
				{
					using (StreamWriter objstream = new StreamWriter(a.FileName, false))
					using (StreamWriter mtlstream = new StreamWriter(Path.ChangeExtension(a.FileName, "mtl"), false))
					{
						List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();

						int totalVerts = 0;
						int totalNorms = 0;
						int totalUVs = 0;

						bool errorFlag = false;

						for (int i = 0; i < @event.Scenes[scenenum].Entities.Count; i++)
							if (@event.Scenes[scenenum].Entities[i].Model != null)
								SonicRetro.SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, @event.Scenes[scenenum].Entities[i].Model, ref materials, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
						

						#region Material Exporting
						objstream.WriteLine("mtllib " + Path.GetFileNameWithoutExtension(a.FileName) + ".mtl");

						for (int i = 0; i < materials.Count; i++)
						{
							int texIndx = materials[i].TextureID;

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

							if (!string.IsNullOrEmpty(TextureInfo[texIndx].Name) && material.UseTexture)
							{
								mtlstream.WriteLine("Map_Kd " + TextureInfo[texIndx].Name + ".png");

								// save texture
								string mypath = Path.GetDirectoryName(a.FileName);
								TextureInfo[texIndx].Image.Save(Path.Combine(mypath, TextureInfo[texIndx].Name + ".png"));
							}

							//progress.Step = String.Format("Texture {0}/{1}", material.TextureID + 1, LevelData.TextureBitmaps[LevelData.leveltexs].Length);
							//progress.StepProgress();
							Application.DoEvents();
						}
						#endregion

						if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");
					}
				}
		}

		private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj",
				FileName = selectedObject.Model.Name
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					string objFileName = a.FileName;
					using (StreamWriter objstream = new StreamWriter(objFileName, false))
					{
						List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();

						objstream.WriteLine("mtllib " + TexturePackName + ".mtl");
						int totalVerts = 0;
						int totalNorms = 0;
						int totalUVs = 0;
						bool errorFlag = false;

						SonicRetro.SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, selectedObject.Model, ref materials, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

						if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");

						string mypath = Path.GetDirectoryName(objFileName);
						using (StreamWriter mtlstream = new StreamWriter(Path.Combine(mypath, TexturePackName + ".mtl"), false))
						{
							for (int i = 0; i < materials.Count; i++)
							{
								int texIndx = materials[i].TextureID;

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

								if (!string.IsNullOrEmpty(TextureInfo[texIndx].Name) && material.UseTexture)
								{
									mtlstream.WriteLine("Map_Kd " + TextureInfo[texIndx].Name + ".png");

									// save texture

									TextureInfo[texIndx].Image.Save(Path.Combine(mypath, TextureInfo[texIndx].Name + ".png"));
								}
							}
						}
					}
				}
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
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
			DrawEntireModel();
		}

		private void showCameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawEntireModel();
		}
	}
}