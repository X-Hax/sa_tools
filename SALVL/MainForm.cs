using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;

using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SALVL
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			File.WriteAllText("SALVL.log", e.Exception.ToString());
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SALVL Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
				Close();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("SALVL.log", e.ExceptionObject.ToString());
			MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SALVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		internal Device d3ddevice;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		bool loaded;
		int interval = 20;
		internal List<Item> SelectedItems;
		bool lookKeyDown;
		bool zoomKeyDown;
		TransformGizmo transformGizmo;

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			d3ddevice = new Device(0, DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } });
			EditorOptions.InitializeDefaultLights(d3ddevice);
			Gizmo.InitGizmo(d3ddevice);
			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);

			LevelData.StateChanged += new LevelData.LevelStateChangeHandler(LevelData_StateChanged);
			panel1.MouseWheel += new MouseEventHandler(panel1_MouseWheel);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded)
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
				Filter = "Level Files|*.sa1lvl;*.sa2lvl;*.exe;*.dll;*.bin;*.prs|All Files|*.*"
			};
			if (a.ShowDialog(this) == DialogResult.OK)
				LoadFile(a.FileName);
		}

		private void LoadFile(string filename)
		{
			loaded = false;
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
					LevelData.geo = new LandTable(file, (int)dlg.NumericUpDown1.Value, (uint)dlg.numericUpDown2.Value, (LandTableFormat)dlg.comboBox2.SelectedIndex);
				}
			}
			LevelData.LevelItems = new List<LevelItem>();

			for (int i = 0; i < LevelData.geo.COL.Count; i++)
				LevelData.LevelItems.Add(new LevelItem(LevelData.geo.COL[i], d3ddevice, i));

			LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
			LevelData.Textures = new Dictionary<string, Texture[]>();
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
						texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.None, Pool.Managed);
					string texname = Path.GetFileNameWithoutExtension(a.FileName);
					if (!LevelData.TextureBitmaps.ContainsKey(texname))
						LevelData.TextureBitmaps.Add(texname, TexBmps);
					if (!LevelData.Textures.ContainsKey(texname))
						LevelData.Textures.Add(texname, texs);
					LevelData.leveltexs = texname;
				}
			}
			loaded = true;
			transformGizmo = new TransformGizmo();
			gizmoSpaceComboBox.Enabled = false;
			gizmoSpaceComboBox.SelectedIndex = 0;

			clearLevelToolStripMenuItem.Enabled = LevelData.geo != null;
			statsToolStripMenuItem.Enabled = LevelData.geo != null;
			SelectedItems = new List<Item>();
			UseWaitCursor = false;
			Enabled = editInfoToolStripMenuItem.Enabled = saveToolStripMenuItem.Enabled = exportToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled = true;

			DrawLevel();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loaded)
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
				if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					LevelData.geo.Tool = "SALVL";
					LevelData.geo.SaveToFile(a.FileName, outfmt);
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
			d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
			Text = "X=" + cam.Position.X + " Y=" + cam.Position.Y + " Z=" + cam.Position.Z + " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X") + " Interval=" + interval + (cam.mode == 1 ? " Distance=" + cam.Distance : "");
			d3ddevice.SetRenderState(RenderStates.FillMode, (int)EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderStates.CullMode, (int)EditorOptions.RenderCullMode);
			d3ddevice.Material = new Microsoft.DirectX.Direct3D.Material { Ambient = Color.White };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 1, 0);
			d3ddevice.RenderState.ZBufferEnable = true;
			d3ddevice.BeginScene();
			//all drawings after this line
			cam.DrawDistance = EditorOptions.RenderDrawDistance;
			d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
			cam.BuildFrustum(d3ddevice.Transform.View, d3ddevice.Transform.Projection);

			EditorOptions.RenderStateCommonSetup(d3ddevice);

			MatrixStack transform = new MatrixStack();
			List<RenderInfo> renderlist = new List<RenderInfo>();
			if (LevelData.LevelItems != null)
				for (int i = 0; i < LevelData.LevelItems.Count; i++)
				{
					bool display = false;
					if (visibleToolStripMenuItem.Checked && LevelData.LevelItems[i].Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !LevelData.LevelItems[i].Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;
					if (display)
						renderlist.AddRange(LevelData.LevelItems[i].Render(d3ddevice, cam, transform, SelectedItems.Contains(LevelData.LevelItems[i])));
				}
			RenderInfo.Draw(renderlist, d3ddevice, cam);

			d3ddevice.EndScene(); // scene drawings go before this line

			transformGizmo.Draw(d3ddevice, cam);
			d3ddevice.Present();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			DrawLevel();
		}

		#region User Keyboard / Mouse Methods
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
			if (!loaded) return;

			switch (e.Button)
			{
				case MouseButtons.Left:
					float mindist = cam.DrawDistance;
					HitResult dist;
					Item item = null;
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
					Viewport viewport = d3ddevice.Viewport;
					Matrix proj = d3ddevice.Transform.Projection;
					Matrix view = d3ddevice.Transform.View;
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
								for (int i = 0; i < LevelData.LevelItems.Count; i++)
								{
									bool display = false;
									if (visibleToolStripMenuItem.Checked && LevelData.LevelItems[i].Visible)
										display = true;
									else if (invisibleToolStripMenuItem.Checked && !LevelData.LevelItems[i].Visible)
										display = true;
									else if (allToolStripMenuItem.Checked)
										display = true;
									if (display)
									{
										dist = LevelData.LevelItems[i].CheckHit(Near, Far, viewport, proj, view);
										if (dist.IsHit & dist.Distance < mindist)
										{
											mindist = dist.Distance;
											item = LevelData.LevelItems[i];
										}
									}
								}
							}

							if (item != null)
							{
								if (ModifierKeys == Keys.Control)
								{
									if (SelectedItems.Contains(item))
										SelectedItems.Remove(item);
									else
										SelectedItems.Add(item);
								}
								else if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
							}
							else if ((ModifierKeys & Keys.Control) == 0)
							{
								SelectedItems.Clear();
							}
						}
					}
					break;
				case MouseButtons.Right:
					bool cancopy = false;
					foreach (Item obj in SelectedItems)
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
			SelectedItemChanged();
			DrawLevel();
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
			if (!e.Alt) lookKeyDown = false;
			if (!e.Control) zoomKeyDown = false;
		}

		private void panel1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!loaded) return;
			if (cam.mode == 0)
			{
				if (e.KeyCode == Keys.E)
				{
					cam.Position = new Vector3();
					DrawLevel();
				}

				if (e.KeyCode == Keys.R)
				{
					cam.Pitch = 0;
					cam.Yaw = 0;
					DrawLevel();
				}
			}

			if (e.Alt) { lookKeyDown = true; if (panel1.ContainsFocus) e.Handled = false; }
			if (e.Control) zoomKeyDown = true;

			if (e.KeyCode == Keys.X)
			{
				cam.mode = (cam.mode + 1) % 2;

				if (cam.mode == 1)
				{
					if (SelectedItems.Count > 0) cam.FocalPoint = Item.CenterFromSelection(SelectedItems).ToVector3();
					else
					{
						cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
					}
				}

				DrawLevel();
			}
			if (e.KeyCode == Keys.N)
			{
				if (EditorOptions.RenderFillMode == FillMode.Solid)
					EditorOptions.RenderFillMode = FillMode.Point;
				else
					EditorOptions.RenderFillMode += 1;

				DrawLevel();
			}
			if (e.KeyCode == Keys.Delete)
			{
				foreach (Item item in SelectedItems)
					item.Delete();
				SelectedItems.Clear();
				SelectedItemChanged();
				DrawLevel();
			}
		}

		Point lastmouse;
		private void Panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (!loaded) return;
			Point evloc = e.Location;
			if (lastmouse == Point.Empty)
			{
				lastmouse = evloc;
				return;
			}
			Point chg = evloc - (Size)lastmouse;
			if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				// all cam controls are now bound to the middle mouse button
				if (cam.mode == 0)
				{
					if (zoomKeyDown)
					{
						cam.Position += cam.Look * (chg.Y * cam.MoveSpeed);
					}
					else if (lookKeyDown)
					{
						cam.Yaw = unchecked((ushort)(cam.Yaw - chg.X * 0x10));
						cam.Pitch = unchecked((ushort)(cam.Pitch - chg.Y * 0x10));
					}
					else if (!lookKeyDown && !zoomKeyDown) // pan
					{
						cam.Position += cam.Up * (chg.Y * cam.MoveSpeed);
						cam.Position += cam.Right * (chg.X * cam.MoveSpeed) * -1;
					}
				}
				else if (cam.mode == 1)
				{
					if (zoomKeyDown)
					{
						cam.Distance += (chg.Y * cam.MoveSpeed) * 3;
					}
					else if (lookKeyDown)
					{
						cam.Yaw = unchecked((ushort)(cam.Yaw - chg.X * 0x10));
						cam.Pitch = unchecked((ushort)(cam.Pitch - chg.Y * 0x10));
					}
					else if (!lookKeyDown && !zoomKeyDown) // pan
					{
						cam.FocalPoint += cam.Up * (chg.Y * cam.MoveSpeed);
						cam.FocalPoint += cam.Right * (chg.X * cam.MoveSpeed) * -1;
					}
				}

				DrawLevel();
			}
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (transformGizmo != null) transformGizmo.TransformAffected(chg.X / 2, chg.Y / 2, cam);
				DrawLevel();

				Rectangle scrbnds = Screen.GetBounds(Cursor.Position);
				if (Cursor.Position.X == scrbnds.Left)
				{
					Cursor.Position = new Point(scrbnds.Right - 2, Cursor.Position.Y);
					evloc = new Point(evloc.X + scrbnds.Width - 2, evloc.Y);
				}
				else if (Cursor.Position.X == scrbnds.Right - 1)
				{
					Cursor.Position = new Point(scrbnds.Left + 1, Cursor.Position.Y);
					evloc = new Point(evloc.X - scrbnds.Width + 1, evloc.Y);
				}
				if (Cursor.Position.Y == scrbnds.Top)
				{
					Cursor.Position = new Point(Cursor.Position.X, scrbnds.Bottom - 2);
					evloc = new Point(evloc.X, evloc.Y + scrbnds.Height - 2);
				}
				else if (Cursor.Position.Y == scrbnds.Bottom - 1)
				{
					Cursor.Position = new Point(Cursor.Position.X, scrbnds.Top + 1);
					evloc = new Point(evloc.X, evloc.Y - scrbnds.Height + 1);
				}
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.None)
			{
				float mindist = cam.DrawDistance; // initialize to max distance, because it will get smaller on each check
				Vector3 mousepos = new Vector3(e.X, e.Y, 0);
				Viewport viewport = d3ddevice.Viewport;
				Matrix proj = d3ddevice.Transform.Projection;
				Matrix view = d3ddevice.Transform.View;
				Vector3 Near, Far;
				Near = mousepos;
				Near.Z = 0;
				Far = Near;
				Far.Z = -1;

				if (transformGizmo != null)
				{
					GizmoSelectedAxes oldSelection = transformGizmo.SelectedAxes;
					transformGizmo.SelectedAxes = transformGizmo.CheckHit(Near, Far, viewport, proj, view, cam);

					if (oldSelection != transformGizmo.SelectedAxes) transformGizmo.Draw(d3ddevice, cam);
				}
			}
			lastmouse = evloc;
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

		internal void SelectedItemChanged()
		{
			propertyGrid1.SelectedObjects = SelectedItems.ToArray();

			if (cam.mode == 1)
			{
				cam.FocalPoint = Item.CenterFromSelection(SelectedItems).ToVector3();
			}

			if (SelectedItems.Count > 0) // set up gizmo
			{
				transformGizmo.Enabled = true;
				transformGizmo.AffectedItems = SelectedItems;
			}
			else
			{
				if (transformGizmo != null)
				{
					transformGizmo.AffectedItems = new List<Item>();
					transformGizmo.Enabled = false;
				}
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in SelectedItems)
				if (item.CanCopy)
				{
					item.Delete();
					selitems.Add(item);
				}
			SelectedItems.Clear();
			SelectedItemChanged();
			DrawLevel();
			if (selitems.Count == 0) return;
			Clipboard.SetData("SADXLVLObjectList", selitems);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in SelectedItems)
				if (item.CanCopy)
					selitems.Add(item);
			if (selitems.Count == 0) return;
			Clipboard.SetData("SADXLVLObjectList", selitems);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> objs = Clipboard.GetData("SADXLVLObjectList") as List<Item>;

			if (objs == null)
			{
				MessageBox.Show("Paste operation failed - this is a known issue and is being worked on.");
				return; // todo: finish implementing proper copy/paste
			}

			Vector3 center = new Vector3();
			foreach (Item item in objs)
				center.Add(item.Position.ToVector3());
			center = new Vector3(center.X / objs.Count, center.Y / objs.Count, center.Z / objs.Count);
			foreach (Item item in objs)
			{
				item.Position = new Vertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
				item.Paste();
			}
			SelectedItems = new List<Item>(objs);
			SelectedItemChanged();
			DrawLevel();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in SelectedItems)
				if (item.CanCopy)
					item.Delete();
			SelectedItems.Clear();
			SelectedItemChanged();
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
			if (importFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string filePath = importFileDialog.FileName;

				bool errorFlag = false;
				string errorMsg = "";

				LevelData.ImportFromFile(filePath, d3ddevice, cam, out errorFlag, out errorMsg);

				if (errorFlag)
				{
					MessageBox.Show(errorMsg);
				}
			}
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string filePath = importFileDialog.FileName;
				DialogResult userClearLevelResult = MessageBox.Show("Do you want to clear the level models first?", "Clear Level?", MessageBoxButtons.YesNoCancel);

				if (userClearLevelResult == System.Windows.Forms.DialogResult.Cancel) return;
				else if (userClearLevelResult == DialogResult.Yes)
				{
					DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNo);

					LevelData.ClearLevelGeometry();

					if (clearAnimsResult == System.Windows.Forms.DialogResult.Yes)
					{
						LevelData.ClearLevelGeoAnims();
					}
				}

				bool errorFlag = false;
				string errorMsg = "";

				LevelData.ImportFromFile(filePath, d3ddevice, cam, out errorFlag, out errorMsg);

				if (errorFlag)
				{
					MessageBox.Show(errorMsg);
				}
			}
		}

		private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj"
			};
			if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
							string mypath = System.IO.Path.GetDirectoryName(a.FileName);
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
						SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, materialPrefix, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
					if (LevelData.geo.Anim != null)
						for (int i = 0; i < LevelData.geo.Anim.Count; i++)
							SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.Anim[i].Model, materialPrefix, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

					if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");
				}
			}
		}

		private void editInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AdvancedInfoDialog dlg = new AdvancedInfoDialog())
				dlg.ShowDialog(this);
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			DrawLevel();
		}

		private void cStructsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog sd = new SaveFileDialog() { DefaultExt = "c", Filter = "C Files|*.c" })
				if (sd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					LandTableFormat fmt = LevelData.geo.Format;
					switch (fmt)
					{
						case LandTableFormat.SA1:
						case LandTableFormat.SADX:
							using (StructExportDialog ed = new StructExportDialog() { Format = LevelData.geo.Format })
								if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
									fmt = ed.Format;
								else
									return;
							break;
					}
					List<string> labels = new List<string>() { LevelData.geo.Name };
					System.Text.StringBuilder result = new System.Text.StringBuilder("/* Sonic Adventure ");
					switch (fmt)
					{
						case LandTableFormat.SA1:
							result.Append("1");
							break;
						case LandTableFormat.SADX:
							result.Append("DX");
							break;
						case LandTableFormat.SA2:
							result.Append("2");
							break;
					}
					result.AppendLine(" LandTable");
					result.AppendLine(" * ");
					result.AppendLine(" * Generated by SALVL");
					result.AppendLine(" * ");
					if (!string.IsNullOrEmpty(LevelData.geo.Description))
					{
						result.Append(" * Description: ");
						result.AppendLine(LevelData.geo.Description);
						result.AppendLine(" * ");
					}
					if (!string.IsNullOrEmpty(LevelData.geo.Author))
					{
						result.Append(" * Author: ");
						result.AppendLine(LevelData.geo.Author);
						result.AppendLine(" * ");
					}
					result.AppendLine(" */");
					result.AppendLine();
					string[] texnames = null;
					if (LevelData.leveltexs != null)
					{
						texnames = new string[LevelData.TextureBitmaps[LevelData.leveltexs].Length];
						for (int i = 0; i < LevelData.TextureBitmaps[LevelData.leveltexs].Length; i++)
							texnames[i] = string.Format("{0}TexName_{1}", LevelData.leveltexs,
								LevelData.TextureBitmaps[LevelData.leveltexs][i].Name);
						result.AppendFormat("enum {0}TexName", LevelData.leveltexs);
						result.AppendLine();
						result.AppendLine("{");
						result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", texnames));
						result.AppendLine("};");
						result.AppendLine();
					}
					result.Append(LevelData.geo.ToStructVariables(fmt, labels, texnames));
					File.WriteAllText(sd.FileName, result.ToString());
				}
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo != null) transformGizmo.AffectedItems = SelectedItems;
			SelectedItemChanged();
			DrawLevel();
		}

		private void clearLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNoCancel);

			if (clearAnimsResult == System.Windows.Forms.DialogResult.Cancel) return;

			LevelData.ClearLevelGeometry();

			if (clearAnimsResult == System.Windows.Forms.DialogResult.Yes)
			{
				LevelData.ClearLevelGeoAnims();
			}
		}

		private void statsToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			MessageBox.Show(LevelData.GetStats());
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool errorFlag = false;
			string errorMsg = "";
			LevelData.DuplicateSelection(d3ddevice, ref SelectedItems, out errorFlag, out errorMsg);

			if (errorFlag) MessageBox.Show(errorMsg);
		}

		private void debugLightingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DefaultLightEditor lightEditor = new DefaultLightEditor();

			lightEditor.Show();
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditorOptionsEditor optionsEditor = new EditorOptionsEditor(cam);
			optionsEditor.FormUpdated += new EditorOptionsEditor.FormUpdatedHandler(optionsEditor_FormUpdated);
			optionsEditor.Show();
		}

		void optionsEditor_FormUpdated()
		{
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
				rotateModeButton.Checked = false;
				DrawLevel(); // possibly find a better way of doing this than re-drawing the entire scene? Possibly keep a copy of the last render w/o gizmo in memory?
			}
		}

		private void moveModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANFORM_MOVE;
				gizmoSpaceComboBox.Enabled = true;
				selectModeButton.Checked = false;
				rotateModeButton.Checked = false;
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
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				DrawLevel();
			}
		}

		private void gizmoSpaceComboBox_DropDownClosed(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.LocalTransform = (gizmoSpaceComboBox.SelectedIndex == 0) ? false : true;
				DrawLevel();
			}
		}

		private void rotateMode_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANSFORM_SCALE;
				transformGizmo.LocalTransform = true;
				gizmoSpaceComboBox.SelectedIndex = 1;
				gizmoSpaceComboBox.Enabled = false;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				DrawLevel();
			}
		}
		#endregion
	}
}