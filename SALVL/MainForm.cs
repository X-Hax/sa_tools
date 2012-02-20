using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SALVL
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            LevelData.MainForm = this;
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
        FillMode rendermode;
        Cull cullmode = Cull.None;
        internal List<Item> SelectedItems;
        PropertyWindow PropertyWindow;

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
                Filter = "Level Files|*.sa1lvl;*.sa2lvl;*.exe;*.dll;*.bin|All Files|*.*"
            };
            if (a.ShowDialog(this) == DialogResult.OK)
            {
                loaded = false;
                UseWaitCursor = true;
                Enabled = false;
                LevelData.leveltexs = null;
                cam = new Camera();
                if (LandTable.CheckLevelFile(a.FileName))
                    LevelData.geo = LandTable.LoadFromFile(a.FileName);
                else
                {
                    byte[] file = File.ReadAllBytes(a.FileName);
                    using (LevelFileDialog dlg = new LevelFileDialog())
                    {
                        dlg.ShowDialog(this);
                        LevelData.geo = new LandTable(file, (int)dlg.NumericUpDown1.Value, (uint)dlg.numericUpDown2.Value, (ModelFormat)dlg.comboBox2.SelectedIndex);
                    }
                }
                LevelData.LevelItems = new List<LevelItem>();
                foreach (COL item in LevelData.geo.COL)
                    LevelData.LevelItems.Add(new LevelItem(item, d3ddevice));
                LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
                LevelData.Textures = new Dictionary<string, Texture[]>();
                a.DefaultExt = "pvm";
                a.Filter = "PVM Files|*.pvm|All Files|*.*";
                if (!string.IsNullOrEmpty(LevelData.geo.TextureFileName))
                    a.FileName = LevelData.geo.TextureFileName + ".pvm";
                else
                    a.FileName = string.Empty;
                if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    BMPInfo[] TexBmps = LevelData.GetTextures(a.FileName);
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
                if (PropertyWindow == null)
                {
                    PropertyWindow = new PropertyWindow();
                    PropertyWindow.Show(this);
                    Activate();
                }
                loaded = true;
                SelectedItems = new List<Item>();
                UseWaitCursor = false;
                Enabled = true;
                DrawLevel();
            }
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
            using (SaveFileDialog a = new SaveFileDialog() { DefaultExt = "sa1lvl", Filter = "SA1LVL Files|*.sa1lvl" })
                if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    LevelData.geo.SaveToFile(a.FileName, ModelFormat.SA1);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        internal void DrawLevel()
        {
            if (!loaded) return;
            d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), panel1.Width / (float)panel1.Height, 1, 10000));
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

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            float mindist = float.PositiveInfinity;
            float dist;
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
                        if (dist > 0 & dist < mindist)
                        {
                            mindist = dist;
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
            PropertyWindow.propertyGrid1.SelectedObjects = SelectedItems.ToArray();
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
            LevelItem item = new LevelItem(d3ddevice);
            Vector3 pos = cam.Position + (-20 * cam.Look);
            item.Position = new Vertex(pos.X, pos.Y, pos.Z);
        }

        public void Writeobj(System.IO.StreamWriter objstream, System.IO.StreamWriter mtlstream, SAModel.Object obj, List<string> usedmtls, MatrixStack transform, ref int totverts, ref int totuvs)
        {
            if (obj.Attach != null && (obj.Flags & SAModel.ObjectFlags.NoDisplay) == 0)
            {
                for (int j = 0; j < obj.Attach.Material.Count; j++)
                {
                    if (!usedmtls.Contains(obj.Attach.Material[j].Name))
                    {
                        mtlstream.WriteLine("newmtl " + obj.Attach.Material[j].Name);
                        mtlstream.WriteLine("Ka 1 1 1");
                        mtlstream.WriteLine("Kd " + obj.Attach.Material[j].DiffuseColor.R / 255f + " " + obj.Attach.Material[j].DiffuseColor.G / 255f + " " + obj.Attach.Material[j].DiffuseColor.B / 255f);
                        mtlstream.WriteLine("d " + obj.Attach.Material[j].DiffuseColor.A / 255f);
                        mtlstream.WriteLine("Ks " + obj.Attach.Material[j].SpecularColor.R / 255f + " " + obj.Attach.Material[j].SpecularColor.G / 255f + " " + obj.Attach.Material[j].SpecularColor.B / 255f);
                        mtlstream.WriteLine("illum 1");
                        mtlstream.WriteLine("texid " + obj.Attach.Material[j].TextureID);
                        mtlstream.WriteLine("Map_Ka " + LevelData.TextureBitmaps[LevelData.leveltexs][obj.Attach.Material[j].TextureID].Name + ".png");
                        mtlstream.WriteLine("Map_Kd " + LevelData.TextureBitmaps[LevelData.leveltexs][obj.Attach.Material[j].TextureID].Name + ".png");
                        usedmtls.Add(obj.Attach.Material[j].Name);
                    }
                }
                objstream.WriteLine("g " + obj.Name);
            }
            transform.Push();
            transform.TranslateLocal(obj.Position.ToVector3());
            transform.RotateYawPitchRollLocal(SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.Y), SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.X), SAModel.Direct3D.Extensions.BAMSToRad(obj.Rotation.Z));
            transform.ScaleLocal(obj.Scale.ToVector3());
            if (obj.Attach != null && (obj.Flags & SAModel.ObjectFlags.NoDisplay) == 0)
            {
                objstream.WriteLine("# " + obj.Attach.Name);
                objstream.WriteLine("# vertex_" + obj.Attach.Name);
                for (int j = 0; j < obj.Attach.Vertex.Length; j++)
                {
                    Vector3 x = Vector3.TransformCoordinate(obj.Attach.Vertex[j].ToVector3(), transform.Top);
                    objstream.WriteLine("v " + x.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + x.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + x.Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + " #" + j + totverts + 1);
                }
                objstream.WriteLine("# normal_" + obj.Attach.Name);
                for (int j = 0; j < obj.Attach.Vertex.Length; j++)
                {
                    Vector3 vect = Vector3.TransformNormal(obj.Attach.Normal[j].ToVector3(), Matrix.Invert(transform.Top));
                    objstream.WriteLine("vn " + vect.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + vect.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + vect.Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + " #" + j + totverts + 1);
                }
                int c = 0;
                for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                    if (obj.Attach.Mesh[j].VColor != null)
                    {
                        objstream.WriteLine("# vcolor_" + obj.Attach.Mesh[j].Name);
                        for (int k = 0; k < obj.Attach.Mesh[j].VColor.Length; k++)
                            objstream.WriteLine("vc " + obj.Attach.Mesh[j].VColor[k].A.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + obj.Attach.Mesh[j].VColor[k].R.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + obj.Attach.Mesh[j].VColor[k].G.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + obj.Attach.Mesh[j].VColor[k].B.ToString(System.Globalization.CultureInfo.InvariantCulture) + " #" + c + k + totuvs + 1);
                        c += obj.Attach.Mesh[j].VColor.Length;
                    }
                c = 0;
                for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                    if (obj.Attach.Mesh[j].UV != null)
                    {
                        objstream.WriteLine("# uv_" + obj.Attach.Mesh[j].Name);
                        for (int k = 0; k < obj.Attach.Mesh[j].UV.Length; k++)
                            objstream.WriteLine("vt " + (obj.Attach.Mesh[j].UV[k].U / 255f).ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + (-obj.Attach.Mesh[j].UV[k].V / 255f).ToString(System.Globalization.CultureInfo.InvariantCulture) + " #" + c + k + totuvs + 1);
                        c += obj.Attach.Mesh[j].UV.Length;
                    }
                for (int j = 0; j < obj.Attach.Mesh.Count; j++)
                {
                    objstream.WriteLine("# mesh_" + obj.Attach.Mesh[j].Name);
                    objstream.WriteLine("usemtl " + obj.Attach.Material[obj.Attach.Mesh[j].MaterialID].Name);
                    int currentstriptotal = 0;
                    for (int k = 0; k < obj.Attach.Mesh[j].Poly.Count; k++)
                    {
                        objstream.WriteLine("# poly " + k);
                        if (obj.Attach.Mesh[j].UV != null)
                        {
                            switch (obj.Attach.Mesh[j].PolyType)
                            {
                                case PolyType.Triangles:
                                case PolyType.Quads:
                                    objstream.Write("f");
                                    for (int l = 0; l < obj.Attach.Mesh[j].Poly[k].Indexes.Length; l++)
                                        objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1) + "/" + (currentstriptotal + l + totuvs + 1) + "/" + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1));
                                    objstream.WriteLine();
                                    currentstriptotal += obj.Attach.Mesh[j].Poly[k].Indexes.Length;
                                    break;
                                case PolyType.Strips:
                                case PolyType.Strips2:
                                    for (int l = 0; l <= obj.Attach.Mesh[j].Poly[k].Indexes.Length - 3; l++)
                                    {
                                        bool flip = ((Strip)obj.Attach.Mesh[j].Poly[k]).Reversed;
                                        if (l % 2 == 0)
                                            flip = !flip;
                                        if (flip)
                                        {
                                            objstream.Write("f");
                                            for (int m = 0; m <= 2; m++)
                                                objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l + m] + totverts + 1) + "/" + (currentstriptotal + m + totuvs + 1) + "/" + (obj.Attach.Mesh[j].Poly[k].Indexes[l + m] + totverts + 1));
                                        }
                                        else
                                        {
                                            objstream.Write("f");
                                            objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 1] + totverts + 1) + "/" + (currentstriptotal + 1 + totuvs + 1) + "/" + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 1] + totverts + 1));
                                            objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1) + "/" + (currentstriptotal + totuvs + 1) + "/" + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1));
                                            objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 2] + totverts + 1) + "/" + (currentstriptotal + 2 + totuvs + 1) + "/" + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 2] + totverts + 1));
                                        }
                                        objstream.WriteLine();
                                        currentstriptotal += 1;
                                    }
                                    currentstriptotal += 2;
                                    break;
                            }
                        }
                        else
                        {
                            switch (obj.Attach.Mesh[j].PolyType)
                            {
                                case PolyType.Triangles:
                                case PolyType.Quads:
                                    objstream.Write("f");
                                    for (int l = 0; l < obj.Attach.Mesh[j].Poly[k].Indexes.Length; l++)
                                        objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1) + "//" + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1));
                                    objstream.WriteLine();
                                    currentstriptotal += obj.Attach.Mesh[j].Poly[k].Indexes.Length;
                                    break;
                                case PolyType.Strips:
                                case PolyType.Strips2:
                                    for (int l = 0; l <= obj.Attach.Mesh[j].Poly[k].Indexes.Length - 3; l++)
                                    {
                                        bool flip = ((Strip)obj.Attach.Mesh[j].Poly[k]).Reversed;
                                        if (l % 2 == 0)
                                            flip = !flip;
                                        if (flip)
                                        {
                                            objstream.Write("f");
                                            for (int m = 0; m <= 2; m++)
                                                objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l + m] + totverts + 1) + "//" + (obj.Attach.Mesh[j].Poly[k].Indexes[l + m] + totverts + 1));
                                        }
                                        else
                                        {
                                            objstream.Write("f");
                                            objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 1] + totverts + 1) + "//" + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 1] + totverts + 1));
                                            objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1) + "//" + (obj.Attach.Mesh[j].Poly[k].Indexes[l] + totverts + 1));
                                            objstream.Write(" " + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 2] + totverts + 1) + "//" + (obj.Attach.Mesh[j].Poly[k].Indexes[l + 2] + totverts + 1));
                                        }
                                        objstream.WriteLine();
                                        currentstriptotal += 1;
                                    }
                                    currentstriptotal += 2;
                                    break;
                            }
                        }
                    }
                    if (obj.Attach.Mesh[j].UV != null)
                        totuvs += obj.Attach.Mesh[j].UV.Length;
                }
                totverts += obj.Attach.Vertex.Length;
            }
            foreach (Object item in obj.Children)
                Writeobj(objstream, mtlstream, item, usedmtls, transform, ref totverts, ref totuvs);
            transform.Pop();
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
                System.IO.StreamWriter objstream = new System.IO.StreamWriter(a.FileName, false);
                System.IO.StreamWriter mtlstream = new System.IO.StreamWriter(System.IO.Path.ChangeExtension(a.FileName, "mtl"), false);
                objstream.WriteLine("mtllib " + System.IO.Path.GetFileNameWithoutExtension(a.FileName) + ".mtl");
                mtlstream.WriteLine("newmtl material_0");
                mtlstream.WriteLine("Ka 1 1 1");
                mtlstream.WriteLine("Kd 1 1 1");
                mtlstream.WriteLine("Ks 0 0 0");
                mtlstream.WriteLine("illum 1");
                string mypath = System.IO.Path.GetDirectoryName(a.FileName);
                foreach (BMPInfo Item in LevelData.TextureBitmaps[LevelData.leveltexs])
                    Item.Image.Save(System.IO.Path.Combine(mypath, Item.Name + ".png"));
                int totverts = 0;
                int totuvs = 0;
                List<string> usedmtls = new List<string>();
                objstream.WriteLine("# geo_" + LevelData.geo.Name);
                for (int i = 0; i < LevelData.geo.COL.Count; i++)
                    Writeobj(objstream, mtlstream, LevelData.geo.COL[i].Model, usedmtls, new MatrixStack(), ref totverts, ref totuvs);
                for (int i = 0; i < LevelData.geo.Anim.Count; i++)
                    Writeobj(objstream, mtlstream, LevelData.geo.Anim[i].Model, usedmtls, new MatrixStack(), ref totverts, ref totuvs);
                mtlstream.Write("#EOF");
                mtlstream.Close();
                objstream.Write("#EOF");
                objstream.Close();
            }
        }
    }
}