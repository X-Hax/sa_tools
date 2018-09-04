using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;

using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.UI;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace SonicRetro.SAModel.SALVL
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
			File.WriteAllText("SALVL.log", e.Exception.ToString());
			if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SALVL Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
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
		EditorItemSelection selectedItems = new EditorItemSelection();
		bool lookKeyDown;
		bool zoomKeyDown;
		TransformGizmo transformGizmo;

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			d3ddevice = new Device(new SharpDX.Direct3D9.Direct3D(), 0, DeviceType.Hardware, panel1.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = Format.D24X8 } });
			EditorOptions.Initialize(d3ddevice);
			Gizmo.InitGizmo(d3ddevice);
			if (Program.Arguments.Length > 0)
				LoadFile(Program.Arguments[0]);

			LevelData.StateChanged += LevelData_StateChanged;
			panel1.MouseWheel += panel1_MouseWheel;
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
            LevelData.ClearLevelItems();

            for (int i = 0; i < LevelData.geo.COL.Count; i++)
            {
                LevelData.AddLevelItem((new LevelItem(LevelData.geo.COL[i], d3ddevice, i, selectedItems)));
            }

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
						texs[j] = TexBmps[j].Image.ToTexture(d3ddevice);
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
			calculateAllBoundsToolStripMenuItem.Enabled = LevelData.geo != null;
			statsToolStripMenuItem.Enabled = LevelData.geo != null;
			selectedItems.SelectionChanged += SelectionChanged;
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
				if (a.ShowDialog(this) == DialogResult.OK)
					LevelData.geo.SaveToFile(a.FileName, outfmt);
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

		private void panel1_KeyDown(object sender, KeyEventArgs e)
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
					if (selectedItems.ItemCount > 0) cam.FocalPoint = Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3();
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
				foreach (Item item in selectedItems.GetSelection())
					item.Delete();
				selectedItems.Clear();
				DrawLevel();
			}
		}

		Point lastmouse;
		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Point evloc = e.Location;
			if (lastmouse == Point.Empty)
			{
				lastmouse = evloc;
				return;
			}
			Point chg = evloc - (Size)lastmouse;
			if (e.Button == MouseButtons.Middle)
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
			if (e.Button == MouseButtons.Left)
			{
                if (transformGizmo != null)
                {
                    //transformGizmo.TransformAffected(chg.X / 2, chg.Y / 2, cam);
                    throw new System.NotImplementedException();
                }

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
			else if (e.Button == MouseButtons.None)
			{
				float mindist = cam.DrawDistance; // initialize to max distance, because it will get smaller on each check
				Vector3 mousepos = new Vector3(e.X, e.Y, 0);
				Viewport viewport = d3ddevice.Viewport;
				Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
				Matrix view = d3ddevice.GetTransform(TransformState.View);
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
            }
            else
            {
                if (transformGizmo != null)
                {
                    transformGizmo.Enabled = false;
                }
            }
        }

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in selectedItems.GetSelection())
				if (item.CanCopy)
				{
					item.Delete();
					selitems.Add(item);
				}
			selectedItems.Clear();
			DrawLevel();
			if (selitems.Count == 0) return;
			Clipboard.SetData("SADXLVLObjectList", selitems);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in selectedItems.GetSelection())
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
				center += item.Position.ToVector3();
			center = new Vector3(center.X / objs.Count, center.Y / objs.Count, center.Z / objs.Count);
			foreach (Item item in objs)
			{
				item.Position = new Vertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
				item.Paste();
			}
			selectedItems.Add(new List<Item>(objs));
			DrawLevel();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in selectedItems.GetSelection())
				if (item.CanCopy)
					item.Delete();
			selectedItems.Clear();
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


				LevelData.ImportFromFile(filePath, d3ddevice, cam, out bool errorFlag, out string errorMsg, selectedItems);

				if (errorFlag)
				{
					MessageBox.Show(errorMsg);
				}
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


				LevelData.ImportFromFile(filePath, d3ddevice, cam, out bool errorFlag, out string errorMsg, selectedItems);

				if (errorFlag)
				{
					MessageBox.Show(errorMsg);
				}
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
                    int stepCount = LevelData.TextureBitmaps[LevelData.leveltexs].Length + LevelData.geo.COL.Count;
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
                        MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.", "Failure",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				dlg.ShowDialog(this);
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			DrawLevel();
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
		}

		private void statsToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			MessageBox.Show(LevelData.GetStats());
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.DuplicateSelection(d3ddevice, selectedItems, out bool errorFlag, out string errorMsg);

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
			optionsEditor.FormUpdated += optionsEditor_FormUpdated;
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

		private void calculateAllBoundsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (LevelItem item in LevelData.LevelItems)
				item.CalculateBounds();
		}
	}
}