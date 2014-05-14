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
        Camera cam = new Camera();
        bool loaded;
        int interval = 20;
        FillMode rendermode = FillMode.Solid;
        Cull cullmode = Cull.None;
        internal List<Item> SelectedItems;

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            d3ddevice = new Device(0, DeviceType.Hardware, panel1.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } });
            d3ddevice.Lights[0].Type = LightType.Directional;
            d3ddevice.Lights[0].Diffuse = Color.White;
            d3ddevice.Lights[0].Ambient = Color.White;
            d3ddevice.Lights[0].Specular = Color.White;
            d3ddevice.Lights[0].Range = 100000;
            d3ddevice.Lights[0].Direction = Vector3.Normalize(new Vector3(0, -1, 0));
            d3ddevice.Lights[0].Enabled = true;
            if (Program.Arguments.Length > 0)
                LoadFile(Program.Arguments[0]);

            LevelData.StateChanged += new LevelData.LevelStateChangeHandler(LevelData_StateChanged);
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
            cam = new Camera();
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
            foreach (COL item in LevelData.geo.COL)
                LevelData.LevelItems.Add(new LevelItem(item, d3ddevice));
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
                        texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
                    string texname = Path.GetFileName(a.FileName);
                    if (!LevelData.TextureBitmaps.ContainsKey(texname))
                        LevelData.TextureBitmaps.Add(texname, TexBmps);
                    if (!LevelData.Textures.ContainsKey(texname))
                        LevelData.Textures.Add(texname, texs);
                    LevelData.leveltexs = texname;
                }
            }
            loaded = true;
            clearLevelToolStripMenuItem.Enabled = LevelData.geo != null;
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
            d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), panel1.Width / (float)panel1.Height, 1, cam.DrawDistance));
            d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
            Text = "X=" + cam.Position.X + " Y=" + cam.Position.Y + " Z=" + cam.Position.Z + " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X") + " Interval=" + interval + (cam.mode == 1 ? " Distance=" + cam.Distance : "");
            d3ddevice.SetRenderState(RenderStates.FillMode, (int)rendermode);
            d3ddevice.SetRenderState(RenderStates.CullMode, (int)cullmode);
            d3ddevice.Material = new Microsoft.DirectX.Direct3D.Material { Ambient = Color.White };
            d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 1, 0);
            d3ddevice.RenderState.ZBufferEnable = true;
            d3ddevice.BeginScene();
            //all drawings after this line
            d3ddevice.SetSamplerState(0, SamplerStageStates.MinFilter, (int)TextureFilter.Anisotropic);
            d3ddevice.SetSamplerState(0, SamplerStageStates.MagFilter, (int)TextureFilter.Anisotropic);
            d3ddevice.SetSamplerState(0, SamplerStageStates.MipFilter, (int)TextureFilter.Anisotropic);
            d3ddevice.SetRenderState(RenderStates.Lighting, true);
            d3ddevice.SetRenderState(RenderStates.SpecularEnable, false);
            d3ddevice.SetRenderState(RenderStates.Ambient, Color.White.ToArgb());
            d3ddevice.SetRenderState(RenderStates.AlphaBlendEnable, true);
            d3ddevice.SetRenderState(RenderStates.BlendOperation, (int)BlendOperation.Add);
            d3ddevice.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            d3ddevice.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            d3ddevice.SetRenderState(RenderStates.AlphaTestEnable, true);
            d3ddevice.SetRenderState(RenderStates.AlphaFunction, (int)Compare.Greater);
            d3ddevice.SetRenderState(RenderStates.AmbientMaterialSource, (int)ColorSource.Material);
            d3ddevice.SetRenderState(RenderStates.DiffuseMaterialSource, (int)ColorSource.Material);
            d3ddevice.SetRenderState(RenderStates.SpecularMaterialSource, (int)ColorSource.Material);
            d3ddevice.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.BlendDiffuseAlpha);
            d3ddevice.SetRenderState(RenderStates.ColorVertex, true);
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
                        renderlist.AddRange(LevelData.LevelItems[i].Render(d3ddevice, transform, SelectedItems.Contains(LevelData.LevelItems[i])));
                }
            RenderInfo.Draw(renderlist, d3ddevice, cam);
            d3ddevice.EndScene(); //all drawings before this line
            d3ddevice.Present();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            DrawLevel();
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
            if (!loaded) return;
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
            switch (e.Button)
            {
                case MouseButtons.Left:
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
                    break;
                case MouseButtons.Right:
                    if (item != null)
                    {
                        if (!SelectedItems.Contains(item))
                        {
                            SelectedItems.Clear();
                            SelectedItems.Add(item);
                        }
                    }
                    else
                    {
                        SelectedItems.Clear();
                    }
                    bool cancopy = false;
                    foreach (Item obj in SelectedItems)
                        if (obj.CanCopy)
                            cancopy = true;
                    if (cancopy)
                    {
                        cutToolStripMenuItem.Enabled = true;
                        copyToolStripMenuItem.Enabled = true;
                        deleteToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        cutToolStripMenuItem.Enabled = false;
                        copyToolStripMenuItem.Enabled = false;
                        deleteToolStripMenuItem.Enabled = false;
                    }
                    pasteToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SADXLVLObjectList");
                    contextMenuStrip1.Show(panel1, e.Location);
                    break;
            }
            SelectedItemChanged();
            DrawLevel();
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
                cam.Yaw = unchecked((ushort)(cam.Yaw - chg.X * 0x10));
                cam.Pitch = unchecked((ushort)(cam.Pitch - chg.Y * 0x10));
                DrawLevel();
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Vector3 horivect = cam.Right;
                Vector3 vertvect = cam.Up;
                if (Math.Abs(horivect.X) > Math.Abs(horivect.Y) & Math.Abs(horivect.X) > Math.Abs(horivect.Z))
                    horivect = new Vector3(Math.Sign(horivect.X), 0, 0);
                else if (Math.Abs(horivect.Y) > Math.Abs(horivect.X) & Math.Abs(horivect.Y) > Math.Abs(horivect.Z))
                    horivect = new Vector3(0, Math.Sign(horivect.Y), 0);
                else if (Math.Abs(horivect.Z) > Math.Abs(horivect.X) & Math.Abs(horivect.Z) > Math.Abs(horivect.Y))
                    horivect = new Vector3(0, 0, Math.Sign(horivect.Z));
                if (Math.Abs(vertvect.X) > Math.Abs(vertvect.Y) & Math.Abs(vertvect.X) > Math.Abs(vertvect.Z))
                    vertvect = new Vector3(Math.Sign(vertvect.X), 0, 0);
                else if (Math.Abs(vertvect.Y) > Math.Abs(vertvect.X) & Math.Abs(vertvect.Y) > Math.Abs(vertvect.Z))
                    vertvect = new Vector3(0, Math.Sign(vertvect.Y), 0);
                else if (Math.Abs(vertvect.Z) > Math.Abs(vertvect.X) & Math.Abs(vertvect.Z) > Math.Abs(vertvect.Y))
                    vertvect = new Vector3(0, 0, Math.Sign(vertvect.Z));
                Vector3 horiz = horivect * (chg.X / 2);
                Vector3 verti = vertvect * (-chg.Y / 2);
                foreach (Item item in SelectedItems)
                {
                    item.Position = new Vertex(
                        item.Position.X + horiz.X + verti.X,
                        item.Position.Y + horiz.Y + verti.Y,
                        item.Position.Z + horiz.Z + verti.Z);
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
            lastmouse = evloc;
        }

        internal void SelectedItemChanged()
        {
            propertyGrid1.SelectedObjects = SelectedItems.ToArray();
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
                    objstream.WriteLine("mtllib " + Path.GetFileNameWithoutExtension(a.FileName) + ".mtl");

                    // This is admittedly not an accurate representation of the materials used in the model - HOWEVER, it makes the materials more managable in MAX
                    // So we're doing it this way. In the future we should come back and add an option to do it this way or the original way.
                    for (int texIndx = 0; texIndx < LevelData.TextureBitmaps[LevelData.leveltexs].Length; texIndx++)
                    {
                        mtlstream.WriteLine(String.Format("newmtl material_{0}", texIndx));
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
                        SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
                    if (LevelData.geo.Anim != null)
                        for (int i = 0; i < LevelData.geo.Anim.Count; i++)
                            SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.Anim[i].Model, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

                    if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");
                }
            }
        }

        private void editInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AdvancedInfoDialog dlg = new AdvancedInfoDialog())
                dlg.ShowDialog(this);
        }

        private void panel1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!loaded) return;
            if (cam.mode == 0)
            {
                if (e.KeyCode == Keys.Down)
                    if (e.Shift)
                        cam.Position += cam.Up * -interval;
                    else
                        cam.Position += cam.Look * interval;
                if (e.KeyCode == Keys.Up)
                    if (e.Shift)
                        cam.Position += cam.Up * interval;
                    else
                        cam.Position += cam.Look * -interval;
                if (e.KeyCode == Keys.Left)
                    cam.Position += cam.Right * -interval;
                if (e.KeyCode == Keys.Right)
                    cam.Position += cam.Right * interval;
                if (e.KeyCode == Keys.K)
                    cam.Yaw = unchecked((ushort)(cam.Yaw - 0x100));
                if (e.KeyCode == Keys.J)
                    cam.Yaw = unchecked((ushort)(cam.Yaw + 0x100));
                if (e.KeyCode == Keys.H)
                    cam.Yaw = unchecked((ushort)(cam.Yaw + 0x4000));
                if (e.KeyCode == Keys.L)
                    cam.Yaw = unchecked((ushort)(cam.Yaw - 0x4000));
                if (e.KeyCode == Keys.M)
                    cam.Pitch = unchecked((ushort)(cam.Pitch - 0x100));
                if (e.KeyCode == Keys.I)
                    cam.Pitch = unchecked((ushort)(cam.Pitch + 0x100));
                if (e.KeyCode == Keys.E)
                    cam.Position = new Vector3();
                if (e.KeyCode == Keys.R)
                {
                    cam.Pitch = 0;
                    cam.Yaw = 0;
                }
            }
            else
            {
                if (e.KeyCode == Keys.Down)
                    if (e.Shift)
                        cam.Pitch = unchecked((ushort)(cam.Pitch - 0x100));
                    else
                        cam.Distance += interval;
                if (e.KeyCode == Keys.Up)
                    if (e.Shift)
                        cam.Pitch = unchecked((ushort)(cam.Pitch + 0x100));
                    else
                    {
                        cam.Distance -= interval;
                        cam.Distance = Math.Max(cam.Distance, interval);
                    }
                if (e.KeyCode == Keys.Left)
                    cam.Yaw = unchecked((ushort)(cam.Yaw + 0x100));
                if (e.KeyCode == Keys.Right)
                    cam.Yaw = unchecked((ushort)(cam.Yaw - 0x100));
                if (e.KeyCode == Keys.K)
                    cam.Yaw = unchecked((ushort)(cam.Yaw - 0x100));
                if (e.KeyCode == Keys.J)
                    cam.Yaw = unchecked((ushort)(cam.Yaw + 0x100));
                if (e.KeyCode == Keys.H)
                    cam.Yaw = unchecked((ushort)(cam.Yaw + 0x4000));
                if (e.KeyCode == Keys.L)
                    cam.Yaw = unchecked((ushort)(cam.Yaw - 0x4000));
                if (e.KeyCode == Keys.M)
                    cam.Pitch = unchecked((ushort)(cam.Pitch - 0x100));
                if (e.KeyCode == Keys.I)
                    cam.Pitch = unchecked((ushort)(cam.Pitch + 0x100));
                if (e.KeyCode == Keys.R)
                {
                    cam.Pitch = 0;
                    cam.Yaw = 0;
                }
            }
            if (e.KeyCode == Keys.X)
                cam.mode = (cam.mode + 1) % 2;
            if (e.KeyCode == Keys.Q)
                interval += 1;
            if (e.KeyCode == Keys.W)
                interval -= 1;
            if (e.KeyCode == Keys.N)
                if (rendermode == FillMode.Solid)
                    rendermode = FillMode.Point;
                else
                    rendermode += 1;
            if (e.KeyCode == Keys.Delete)
            {
                foreach (Item item in SelectedItems)
                    item.Delete();
                SelectedItems.Clear();
                SelectedItemChanged();
                DrawLevel();
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
    }
}