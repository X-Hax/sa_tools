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
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SA2EventViewer Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
				Close();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("SA2EventViewer.log", e.ExceptionObject.ToString());
			MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SA2EventViewer Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		EditorOptionsEditor optionsEditor;

		bool loaded;
		string currentFileName = "";
		Event @event;
		NJS_MOTION[] animations;
		int animframe = 0;
		List<List<Mesh[]>> meshes;
		string TexturePackName;
		BMPInfo[] TextureInfo;
		Texture[] Textures;
		NJS_OBJECT selectedObject;

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

			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "Event Files|e????.prs;e????.bin|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
					LoadFile(a.FileName);
		}

		private void LoadFile(string filename)
		{
			loaded = false;
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			timer1.Stop();
			animations = null;
			animframe = 0;
			@event = new Event(filename);
			meshes = new List<List<Mesh[]>>();
			foreach (EventScene scene in @event.Scenes)
			{
				List<Mesh[]> scenemeshes = new List<Mesh[]>();
				foreach (EventEntity entity in scene.Entities)
				{
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
				}
				meshes.Add(scenemeshes);
			}
			loaded = exportToolStripMenuItem.Enabled = true;
			selectedObject = null;
			SelectedItemChanged();

			currentFileName = filename;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		string GetStatusString()
		{
			return "SA2EventViewer: " + currentFileName;
			// + " X=" + cam.Position.X + " Y=" + cam.Position.Y + " Z=" + cam.Position.Z + " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X") + " Interval=" + cameraMotionInterval + (cam.mode == 1 ? " Distance=" + cam.Distance : "") + (animation != null ? " Animation=" + animation.Name + " Frame=" + animframe : "");
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded) return;
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), panel1.Width / (float)panel1.Height, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformState.View, cam.ToMatrix());
			Text = GetStatusString();
			d3ddevice.SetRenderState(RenderState.FillMode, EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderState.CullMode, EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = Color.White.ToRawColor4() };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToRawColorBGRA(), 1, 0);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			d3ddevice.BeginScene();

			//all drawings after this line
			EditorOptions.RenderStateCommonSetup(d3ddevice);

			MatrixStack transform = new MatrixStack();
			for (int i = 0; i < @event.Scenes[0].Entities.Count; i++)
			{
				transform.Push();
				transform.NJTranslate(@event.Scenes[0].Entities[i].Position);
				if (@event.Scenes[0].Entities[i].Model.HasWeight)
					RenderInfo.Draw(@event.Scenes[0].Entities[i].Model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes[0][i]), d3ddevice, cam);
				else
					RenderInfo.Draw(@event.Scenes[0].Entities[i].Model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes[0][i]), d3ddevice, cam);
				transform.Pop();
			}


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
			/*transform.Push();
			modelindex++;
			if (obj.Animate) animindex++;
			if (animation != null && animation.Models.ContainsKey(animindex))
				obj.ProcessTransforms(animation.Models[animindex], animframe, transform);
			else
				obj.ProcessTransforms(transform);
			if (obj == selectedObject)
			{
				if (obj.Attach != null)
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
				transform.Pop();
				return true;
			}
			foreach (NJS_OBJECT child in obj.Children)
				if (DrawSelectedObject(child, transform, ref modelindex, ref animindex))
				{
					transform.Pop();
					return true;
				}
			transform.Pop();*/
			return false;
		}

		private void UpdateWeightedModel()
		{
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
					cam.mode = (cam.mode + 1) % 2;

					if (cam.mode == 1)
					{
						if (selectedObject != null)
						{
							cam.FocalPoint = selectedObject.Position.ToVector3();
						}
						else
						{
							cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
						}
					}

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

					draw = true;
					break;

				case ("Change Render Mode"):
					if (EditorOptions.RenderFillMode == FillMode.Solid)
						EditorOptions.RenderFillMode = FillMode.Point;
					else
						EditorOptions.RenderFillMode += 1;

					draw = true;
					break;

				case ("Delete"):
					/*foreach (Item item in selectedItems.GetSelection())
						item.Delete();
					selectedItems.Clear();*/
					throw new System.NotImplementedException();
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
					if (cam.mode == 0)
					{
						cam.Position = new Vector3();
						draw = true;
					}
					break;

				case ("Reset Camera Rotation"):
					if (cam.mode == 0)
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

				/*case ("Next Animation"):
					if (animations != null)
					{
						animnum++;
						animframe = 0;
						if (animnum == animations.Length) animnum = -1;
						if (animnum > -1)
							animation = animations[animnum];
						else
							animation = null;

						draw = true;
					}
					break;

				case ("Previous Animation"):
					if (animations != null)
					{
						animnum--;
						animframe = 0;
						if (animnum == -2) animnum = animations.Length - 1;
						if (animnum > -1)
							animation = animations[animnum];
						else
							animation = null;

						draw = true;
					}
					break;

				case ("Previous Frame"):
					if (animation != null)
					{
						animframe--;
						if (animframe < 0) animframe = animation.Frames - 1;
						draw = true;
					}
					break;

				case ("Next Frame"):
					if (animation != null)
					{
						animframe++;
						if (animframe == animation.Frames) animframe = 0;
						draw = true;
					}
					break;

				case ("Play/Pause Animation"):
					if (animation != null)
					{
						timer1.Enabled = !timer1.Enabled;
						draw = true;
					}
					break;*/

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

			if (cameraKeyDown)
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

				UpdateWeightedModel();
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

		private void timer1_Tick(object sender, EventArgs e)
		{
			/*if (animation == null) return;
			animframe++;
			if (animframe == animation.Frames) animframe = 0;
			UpdateWeightedModel();
			DrawEntireModel();*/
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			/*if (!loaded) return;

			if (e.Button == MouseButtons.Middle) actionInputCollector.KeyDown(Keys.MButton);

			if (e.Button == MouseButtons.Left)
			{
				HitResult dist;
				Vector3 mousepos = new Vector3(e.X, e.Y, 0);
				Viewport viewport = d3ddevice.Viewport;
				Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
				Matrix view = d3ddevice.GetTransform(TransformState.View);
				Vector3 Near, Far;
				Near = mousepos;
				Near.Z = 0;
				Far = Near;
				Far.Z = -1;
				if (model.HasWeight)
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
				contextMenuStrip1.Show(panel1, e.Location);*/
		}

		internal void SelectedItemChanged()
		{
			if (selectedObject != null)
			{
				propertyGrid1.SelectedObject = selectedObject;
				exportOBJToolStripMenuItem.Enabled = selectedObject.Attach != null;
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

						//SonicRetro.SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, model, ref materials, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

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

		private void multiObjToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog()
			{
				SelectedPath = Environment.CurrentDirectory,
				ShowNewFolderButton = true
			})
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					/*foreach (NJS_OBJECT obj in model.GetObjects().Where(a => a.Attach != null))
					{
						string objFileName = Path.Combine(dlg.SelectedPath, obj.Name + ".obj");
						using (StreamWriter objstream = new StreamWriter(objFileName, false))
						{
							List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();

							objstream.WriteLine("mtllib " + TexturePackName + ".mtl");
							bool errorFlag = false;

							SonicRetro.SAModel.Direct3D.Extensions.WriteSingleModelAsObj(objstream, obj, ref materials, ref errorFlag);

							if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");

							using (StreamWriter mtlstream = new StreamWriter(Path.Combine(dlg.SelectedPath, TexturePackName + ".mtl"), false))
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
										string mypath = Path.GetDirectoryName(objFileName);
										TextureInfo[texIndx].Image.Save(Path.Combine(mypath, TextureInfo[texIndx].Name + ".png"));
									}
								}
							}
						}
					}*/
				}
			}
		}

		private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj",
				FileName = selectedObject.Name
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					string objFileName = a.FileName;
					NJS_OBJECT obj = selectedObject;
					using (StreamWriter objstream = new StreamWriter(objFileName, false))
					{
						List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();

						objstream.WriteLine("mtllib " + TexturePackName + ".mtl");
						bool errorFlag = false;

						SonicRetro.SAModel.Direct3D.Extensions.WriteSingleModelAsObj(objstream, obj, ref materials, ref errorFlag);

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
			UpdateWeightedModel();
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
	}
}