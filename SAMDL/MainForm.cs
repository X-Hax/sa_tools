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
using SonicRetro.SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;
using Point = System.Drawing.Point;

namespace SonicRetro.SAModel.SAMDL
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
			File.WriteAllText("SAMDL.log", e.Exception.ToString());
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SAMDL Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
				Close();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("SAMDL.log", e.ExceptionObject.ToString());
			MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SAMDL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		EditorOptionsEditor optionsEditor;

		bool loaded;
		string currentFileName = "";
		NJS_OBJECT model;
		NJS_MOTION[] animations;
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
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
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

			modelLibraryWindow = new ModelLibraryWindow();
			modelLibrary = modelLibraryWindow.modelLibraryControl1;
			modelLibrary.InitRenderer();
			modelLibrary.SelectionChanged += modelLibrary_SelectionChanged;

			sphereMesh = Mesh.Sphere(0.0625f, 10, 10);
			selectedSphereMesh = Mesh.Sphere(0.0625f, 10, 10, Color.Lime);
			modelSphereMesh = Mesh.Sphere(0.0625f, 10, 10, Color.Red);
			selectedModelSphereMesh = Mesh.Sphere(0.0625f, 10, 10, Color.Yellow);
			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded)
				switch (MessageBox.Show(this, "Do you want to save?", "SAMDL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveAsToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "sa1mdl",
				Filter = "Model Files|*.sa1mdl;*.sa2mdl;*.sa2bmdl;*.exe;*.dll;*.bin;*.prs;*.rel|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
					LoadFile(a.FileName);
		}

		private void LoadFile(string filename)
		{
			loaded = false;
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			timer1.Stop();
			modelFile = null;
			animation = null;
			animations = null;
			animnum = -1;
			animframe = 0;
			if (ModelFile.CheckModelFile(filename))
			{
				modelFile = new ModelFile(filename);
				outfmt = modelFile.Format;
				model = modelFile.Model;
				animations = new NJS_MOTION[modelFile.Animations.Count];
				modelFile.Animations.CopyTo(animations, 0);
			}
			else
			{
				byte[] file = File.ReadAllBytes(filename);
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					file = FraGag.Compression.Prs.Decompress(file);
				ByteConverter.BigEndian = false;
				uint? baseaddr = SA_Tools.HelperFunctions.SetupEXE(ref file);
				if (baseaddr.HasValue)
				{
					modelinfo.numericUpDown2.Value = baseaddr.Value;
					modelinfo.numericUpDown2.Enabled = false;
					modelinfo.ComboBox1.Enabled = false;
					modelinfo.checkBox2.Checked = modelinfo.checkBox2.Enabled = false;
					LoadBinFile(file);
				}
				else if (Path.GetExtension(filename).Equals(".rel", StringComparison.OrdinalIgnoreCase))
				{
					ByteConverter.BigEndian = true;
					SA_Tools.HelperFunctions.FixRELPointers(file);
					modelinfo.numericUpDown2.Value = 0;
					modelinfo.numericUpDown2.Enabled = false;
					modelinfo.ComboBox1.Enabled = false;
					modelinfo.checkBox2.Enabled = false;
					modelinfo.checkBox2.Checked = true;
					LoadBinFile(file);
				}
				else
					using (FileTypeDialog ftd = new FileTypeDialog())
					{
						if (ftd.ShowDialog(this) != DialogResult.OK)
							return;
						if (ftd.typBinary.Checked)
						{
							modelinfo.numericUpDown2.Enabled = true;
							modelinfo.ComboBox1.Enabled = true;
							modelinfo.checkBox2.Enabled = true;
							LoadBinFile(file);
						}
						else if (ftd.typSA2MDL.Checked | ftd.typSA2BMDL.Checked)
						{
							ModelFormat fmt = outfmt = ModelFormat.Chunk;
							ByteConverter.BigEndian = ftd.typSA2BMDL.Checked;
							using (SA2MDLDialog dlg = new SA2MDLDialog())
							{
								int address = 0;
								SortedDictionary<int, NJS_OBJECT> sa2models = new SortedDictionary<int, NJS_OBJECT>();
								int i = ByteConverter.ToInt32(file, address);
								while (i != -1)
								{
									sa2models.Add(i, new NJS_OBJECT(file, ByteConverter.ToInt32(file, address + 4), 0, fmt));
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
											animations = new List<NJS_MOTION>(anis.Values).ToArray();
										}
									}
								}
							}
						}
					}
			}
			if (outfmt == ModelFormat.Chunk)
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
			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
			AddTreeNode(model, treeView1.Nodes);
			loaded = saveMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = true;
			textureRemappingToolStripMenuItem.Enabled = TextureInfo != null;
			showWeightsToolStripMenuItem.Enabled = model.HasWeight;
			selectedObject = model;
			SelectedItemChanged();

			currentFileName = filename;

			AddModelToLibrary(model, false);
		}

		private void LoadBinFile(byte[] file)
		{
			modelinfo.ShowDialog(this);
			ByteConverter.BigEndian = modelinfo.checkBox2.Checked;
			if (modelinfo.checkBox1.Checked)
				animations = new NJS_MOTION[] { NJS_MOTION.ReadHeader(file, (int)modelinfo.numericUpDown3.Value, (uint)modelinfo.numericUpDown2.Value, (ModelFormat)modelinfo.comboBox2.SelectedIndex) };
			model = new NJS_OBJECT(file, (int)modelinfo.NumericUpDown1.Value, (uint)modelinfo.numericUpDown2.Value, (ModelFormat)modelinfo.comboBox2.SelectedIndex);
			switch ((ModelFormat)modelinfo.comboBox2.SelectedIndex)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					outfmt = ModelFormat.Basic;
					break;
				case ModelFormat.Chunk:
					outfmt = ModelFormat.Chunk;
					break;
				case ModelFormat.GC:
					outfmt = ModelFormat.GC;
					break;
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
			if (loaded)
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

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void SaveAs()
		{
			using (SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = (outfmt == ModelFormat.GC ? "sa2b" : (outfmt == ModelFormat.Chunk ? "sa2" : "sa1")) + "mdl",
				Filter = (outfmt == ModelFormat.GC ? "SA2B" : (outfmt == ModelFormat.Chunk ? "SA2" : "SA1")) + "MDL Files|*." + (outfmt == ModelFormat.GC ? "sa2b" : (outfmt == ModelFormat.Chunk ? "sa2" : "sa1")) + "mdl|All Files|*.*"
			})
			{
				if (currentFileName.Length > 0) a.InitialDirectory = currentFileName;

				if (a.ShowDialog(this) == DialogResult.OK)
				{
					Save(a.FileName);
				}
			}
		}

		private void Save(string fileName)
		{
			if (modelFile != null)
				modelFile.SaveToFile(fileName);
			else
			{
				ModelFile.CreateFile(fileName, model, null, null, null, null, outfmt);
				modelFile = new ModelFile(fileName);
			}

			currentFileName = fileName;
			UpdateStatusString();
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
					DialogResult dialogResult = MessageBox.Show("Are you sure? You will lose any unsaved work.", "Are you sure?", MessageBoxButtons.YesNo);

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

		private void NewFile(ModelFormat modelFormat)
		{
			loaded = false;
			//Environment.CurrentDirectory = Path.GetDirectoryName(filename); // might not need this for now?
			timer1.Stop();
			modelFile = null;
			animation = null;
			animations = null;
			animnum = -1;
			animframe = 0;

			outfmt = modelFormat;
			animations = new NJS_MOTION[0];

			model = new NJS_OBJECT();
			model.Morph = false;
			model.ProcessVertexData();
			meshes = new Mesh[1];
			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
			AddTreeNode(model, treeView1.Nodes);
			selectedObject = model;

			loaded = saveMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = true;
			textureRemappingToolStripMenuItem.Enabled = TextureInfo != null;
			SelectedItemChanged();

			currentFileName = "";
			UpdateStatusString();
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
			Text = "SAMDL: " + currentFileName;
			cameraPosLabel.Text = $"Camera Pos: {cam.Position}";
			animNameLabel.Text = $"Animation: {animation?.Name ?? "None"}";
			animFrameLabel.Text = $"Frame: {animframe}";
			// + " X=" + cam.Position.X + " Y=" + cam.Position.Y + " Z=" + cam.Position.Z + " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X") + " Interval=" + cameraMotionInterval + (cam.mode == 1 ? " Distance=" + cam.Distance : "") + (animation != null ? " Animation=" + animation.Name + " Frame=" + animframe : "");
		}

		#region Rendering Methods
		internal void DrawEntireModel()
		{
			if (!loaded) return;
			d3ddevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), panel1.Width / (float)panel1.Height, 1, cam.DrawDistance));
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
				if (outfmt == ModelFormat.Chunk)
					RenderInfo.Draw(model.DrawModelTreeWeighted(EditorOptions.RenderFillMode, transform.Top, Textures, meshes), d3ddevice, cam);
				else if (animation != null)
					RenderInfo.Draw(model.DrawModelTreeAnimated(EditorOptions.RenderFillMode, transform, Textures, meshes, animation, animframe), d3ddevice, cam);
				else
					RenderInfo.Draw(model.DrawModelTree(EditorOptions.RenderFillMode, transform, Textures, meshes), d3ddevice, cam);

				if (selectedObject != null)
				{
					if (outfmt == ModelFormat.Chunk)
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
			if (outfmt == ModelFormat.Chunk)
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

				case ("Next Animation"):
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

		private void loadTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs" })
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					TextureInfo = TextureArchive.GetTextures(a.FileName);

					TexturePackName = Path.GetFileNameWithoutExtension(a.FileName);
					Textures = new Texture[TextureInfo.Length];
					for (int j = 0; j < TextureInfo.Length; j++)
						Textures[j] = TextureInfo[j].Image.ToTexture(d3ddevice);

					textureRemappingToolStripMenuItem.Enabled = loaded;
					DrawEntireModel();
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

		private void colladaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "dae", Filter = "DAE Files|*.dae" })
				if (sd.ShowDialog(this) == DialogResult.OK)
				{
					model.ToCollada(TextureInfo?.Select((item) => item.Name).ToArray()).Save(sd.FileName);
					string p = Path.GetDirectoryName(sd.FileName);
					if (TextureInfo != null)
						foreach (BMPInfo img in TextureInfo)
							img.Image.Save(Path.Combine(p, img.Name + ".png"));
				}
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
						model.ToStructVariables(sw, dx, labels, texnames);
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
				Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
				Matrix view = d3ddevice.GetTransform(TransformState.View);
				Vector3 Near, Far;
				Near = mousepos;
				Near.Z = 0;
				Far = Near;
				Far.Z = -1;
				if (outfmt == ModelFormat.Chunk)
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
				contextMenuStrip1.Show(panel1, e.Location);
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
				editMaterialsToolStripMenuItem.Enabled = selectedObject.Attach is BasicAttach || (selectedObject.Attach is ChunkAttach ca && ca.Poly?.Count > 0);
				importOBJToolStripMenuItem.Enabled = outfmt == ModelFormat.Basic;
				//importOBJToolstripitem.Enabled = outfmt == ModelFormat.Basic;
				exportOBJToolStripMenuItem.Enabled = selectedObject.Attach != null;
			}
			else
			{
				treeView1.SelectedNode = null;
				suppressTreeEvent = false;
				propertyGrid1.SelectedObject = null;
				copyModelToolStripMenuItem.Enabled = false;
				pasteModelToolStripMenuItem.Enabled = Clipboard.ContainsData(GetAttachType().AssemblyQualifiedName);
				editMaterialsToolStripMenuItem.Enabled = false;
				importOBJToolStripMenuItem.Enabled = outfmt == ModelFormat.Basic;
				//importOBJToolstripitem.Enabled = outfmt == ModelFormat.Basic;
				exportOBJToolStripMenuItem.Enabled = false;
			}
			if (showWeightsToolStripMenuItem.Checked && model.HasWeight)
				model.UpdateWeightedModelSelection(selectedObject, meshes);

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

						Direct3D.Extensions.WriteModelAsObj(objstream, model, ref materials, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

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

							if (TextureInfo != null && !string.IsNullOrEmpty(TextureInfo[texIndx].Name) && material.UseTexture)
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
					foreach (NJS_OBJECT obj in model.GetObjects().Where(a => a.Attach != null))
					{
						string objFileName = Path.Combine(dlg.SelectedPath, obj.Name + ".obj");
						using (StreamWriter objstream = new StreamWriter(objFileName, false))
						{
							List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();

							objstream.WriteLine("mtllib " + TexturePackName + ".mtl");
							bool errorFlag = false;

							Direct3D.Extensions.WriteSingleModelAsObj(objstream, obj, ref materials, ref errorFlag);

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
					}
				}
			}
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
			if (attach is BasicAttach batt)
			{
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
			}
			else if (attach is ChunkAttach catt)
			{
				catt.VertexName = "vertex_" + Extensions.GenerateIdentifier();
				catt.PolyName = "poly_" + Extensions.GenerateIdentifier();
			}
			selectedObject.Attach = attach;
			attach.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			try { meshes[Array.IndexOf(models, selectedObject)] = attach.CreateD3DMesh(); }
			catch { }
			DrawEntireModel();
		}

		private void editMaterialsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<NJS_MATERIAL> mats;
			switch (selectedObject.Attach)
			{
				case BasicAttach bscatt:
					mats = bscatt.Material;
					break;
				case ChunkAttach cnkatt:
					mats = cnkatt.MeshInfo.Select(a => a.Material).ToList();
					break;
				default:
					return;
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
			}
		}

		private void importOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj"
			})
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Attach newattach = Direct3D.Extensions.obj2nj(dlg.FileName, TextureInfo?.Select(a => a.Name).ToArray());

					editMaterialsToolStripMenuItem.Enabled = selectedObject.Attach is BasicAttach;

					modelLibrary.Add(newattach);

					if (selectedObject.Attach != null)
					{
						newattach.Name = selectedObject.Attach.Name;
					}

					meshes[Array.IndexOf(model.GetObjects(), selectedObject)] = newattach.CreateD3DMesh();
					selectedObject.Attach = newattach;

					Matrix m = Matrix.Identity;
					selectedObject.ProcessTransforms(m);
					float scale = Math.Max(Math.Max(selectedObject.Scale.X, selectedObject.Scale.Y), selectedObject.Scale.Z);
					BoundingSphere bounds = new BoundingSphere(Vector3.TransformCoordinate(selectedObject.Attach.Bounds.Center.ToVector3(), m).ToVertex(), selectedObject.Attach.Bounds.Radius * scale);

					bool boundsVisible = !cam.SphereInFrustum(bounds);

					if (!boundsVisible)
					{
						cam.MoveToShowBounds(bounds);
					}

					DrawEntireModel();
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

						Direct3D.Extensions.WriteSingleModelAsObj(objstream, obj, ref materials, ref errorFlag);

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
						}
					if (outfmt == ModelFormat.Chunk)
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
			DrawEntireModel();
		}

		private void showNodesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
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
						model.ToNJA(sw, dx, labels, texnames);
					}
				}
		}

		private void aSSIMPImportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NJS_OBJECT obj = selectedObject;
			Assimp.AssimpContext context = new Assimp.AssimpContext();
			context.SetConfig(new Assimp.Configs.FBXPreservePivotsConfig(false));

			using (OpenFileDialog a = new OpenFileDialog
			{
				DefaultExt = "fbx",
				Filter = "Model Files|*.obj;*.fbx;*.dae;|All Files|*.*"
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					string objFileName = a.FileName;
					Assimp.Scene scene = context.ImportFile(objFileName, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.JoinIdenticalVertices | Assimp.PostProcessSteps.FlipUVs);
					loaded = false;
					//Environment.CurrentDirectory = Path.GetDirectoryName(filename); // might not need this for now?
					timer1.Stop();
					modelFile = null;
					animation = null;
					animations = null;
					animnum = -1;
					animframe = 0;

					//outfmt = ModelFormat.GC;
					animations = new NJS_MOTION[0];
					treeView1.Nodes.Clear();
					nodeDict = new Dictionary<NJS_OBJECT, TreeNode>();
					//model = new NJS_OBJECT(scene, scene.RootNode, TextureInfo?.Select(t => t.Name).ToArray(), outfmt);
					model = SAEditorCommon.Import.AssimpStuff.AssimpImport(scene, /* ? */ scene.RootNode.Children[0], outfmt, TextureInfo?.Select(t => t.Name).ToArray());
					editMaterialsToolStripMenuItem.Enabled = true;

					if (outfmt == ModelFormat.Chunk)
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

					showWeightsToolStripMenuItem.Enabled = model.HasWeight;

					AddTreeNode(model, treeView1.Nodes);
					loaded = saveMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = findToolStripMenuItem.Enabled = true;
					textureRemappingToolStripMenuItem.Enabled = TextureInfo != null;
					selectedObject = model;
					SelectedItemChanged();

					AddModelToLibrary(model, false);
				}
			}
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
			DrawEntireModel();
		}

		private void aSSIMPExportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "dae",
				Filter = "Model Files|*.fbx;*.dae",
				FileName = model.Name
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
					if (TextureInfo != null)
					{
						foreach (BMPInfo bmp in TextureInfo)
						{
							texturePaths.Add(Path.Combine(rootPath, bmp.Name + ".png"));
							bmp.Image.Save(Path.Combine(rootPath, bmp.Name + ".png"));
						}
					}
					SAEditorCommon.Import.AssimpStuff.AssimpExport(model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
					/*
					if (animation != null)
					{
						Assimp.Animation asAnim = new Assimp.Animation() { Name = animation.Name };
						asAnim.DurationInTicks = animation.Frames;
						asAnim.TicksPerSecond = 0.1f;
						string[] names = model.GetObjects().Select(l => l.Name).ToArray();
						List<Assimp.NodeAnimationChannel> nodeAnimChannels = new List<Assimp.NodeAnimationChannel>();
						foreach (KeyValuePair<int, AnimModelData> pair in animation.Models)
						{
							Assimp.NodeAnimationChannel channel = new Assimp.NodeAnimationChannel();
							channel.NodeName = names[pair.Key];
							AnimModelData data = pair.Value;
							if(data.Position.Count > 0)
							{
								foreach(KeyValuePair<int, Vertex> ver in data.Position)
									channel.PositionKeys.Add(new Assimp.VectorKey() { Time = ver.Key, Value = new Assimp.Vector3D(ver.Value.X, ver.Value.Y, ver.Value.Z) });
							}
							if (data.Rotation.Count > 0)
							{
								foreach (KeyValuePair<int, Rotation> ver in data.Rotation)
									channel.RotationKeys.Add(new Assimp.QuaternionKey()
									{
										Time = ver.Key,
										Value = new Assimp.Quaternion((ver.Value.YDeg) * (float)Math.PI / 180.0f, (ver.Value.ZDeg) * (float)Math.PI / 180.0f, (ver.Value.XDeg) * (float)Math.PI / 180.0f) //umm yeah
									});
							}
							if (data.Scale.Count > 0)
							{
								foreach (KeyValuePair<int, Vertex> ver in data.Scale)
									channel.ScalingKeys.Add(new Assimp.VectorKey() { Time = ver.Key, Value = new Assimp.Vector3D(ver.Value.X, ver.Value.Y, ver.Value.Z) });
							}
						}
						asAnim.NodeAnimationChannels.AddRange(nodeAnimChannels);
						scene.Animations.Add(asAnim);
					}
					*/
					context.ExportFile(scene, a.FileName, a.FileName.EndsWith("dae") ? "collada" : "fbx", Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//
				}
			}
		}

		private void showNodeConnectionsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawEntireModel();
		}

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
	}
}