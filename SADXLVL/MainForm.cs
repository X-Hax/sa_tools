using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SADXLVL2
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
            File.WriteAllText("SADXLVL2.log", e.Exception.ToString());
            if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SADXLVL2 Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
                Close();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("SADXLVL2.log", e.ExceptionObject.ToString());
            MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal Device d3ddevice;
        Dictionary<string, Dictionary<string, string>> ini;
        Camera cam = new Camera();
        string level;
        bool loaded;
        int interval = 20;
        FillMode rendermode;
        Cull cullmode = Cull.None;
        List<Item> SelectedItems;
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
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog a = new OpenFileDialog()
            {
                DefaultExt = "ini",
                Filter = "INI Files|*.ini|All Files|*.*"
            };
            if (a.ShowDialog(this) == DialogResult.OK)
            {
                loaded = false;
                ini = IniFile.Load(a.FileName);
                Environment.CurrentDirectory = Path.GetDirectoryName(a.FileName);
                changeLevelToolStripMenuItem.DropDownItems.Clear();
                foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
                    if (!string.IsNullOrEmpty(item.Key))
                        changeLevelToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(item.Key));
            }
        }

        private void changeLevelToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            loaded = false;
            foreach (ToolStripMenuItem item in changeLevelToolStripMenuItem.DropDownItems)
                item.Checked = false;
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            fileToolStripMenuItem.HideDropDown();
            level = e.ClickedItem.Text;
            UseWaitCursor = true;
            Enabled = false;
            Text = "SADXLVL2 - Loading " + level + "...";
#if !DEBUG
            backgroundWorker1.RunWorkerAsync();
#else
            backgroundWorker1_DoWork(null, null);
            backgroundWorker1_RunWorkerCompleted(null, null);
#endif
        }

        bool initerror = false;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
#if !DEBUG
            try
            {
#endif
                Dictionary<string, string> group = ini[level];
                string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
                string levelact = group.GetValueOrDefault("LevelID", "0000");
                byte levelnum = byte.Parse(levelact.Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                byte actnum = byte.Parse(levelact.Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                cam = new Camera();
                if (!group.ContainsKey("LevelGeo"))
                    LevelData.geo = null;
                else
                {
                    Dictionary<string, Dictionary<string, string>> geoini = IniFile.Load(group["LevelGeo"]);
                    LevelData.geo = new LandTable(geoini, geoini[string.Empty]["LandTable"]);
                    LevelData.LevelItems = new List<LevelItem>();
                    foreach (COL item in LevelData.geo.COL)
                        LevelData.LevelItems.Add(new LevelItem(item, d3ddevice));
                }
                LevelData.TextureBitmaps = new Dictionary<string, Bitmap[]>();
                LevelData.Textures = new Dictionary<string, Texture[]>();
                if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName))
                {
                    Bitmap[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, LevelData.geo.TextureFileName) + ".PVM");
                    Texture[] texs = new Texture[TexBmps.Length];
                    for (int j = 0; j < TexBmps.Length - 1; j++)
                        texs[j] = new Texture(d3ddevice, TexBmps[j], Usage.SoftwareProcessing, Pool.Managed);
                    if (!LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
                        LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
                    if (!LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
                        LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
                    LevelData.leveltexs = LevelData.geo.TextureFileName;
                }
                string[] textures = group["Textures"].Split(',');
                foreach (string tex in textures)
                {
                    Bitmap[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, tex) + ".PVM");
                    Texture[] texs = new Texture[TexBmps.Length];
                    for (int j = 0; j < TexBmps.Length - 1; j++)
                        texs[j] = new Texture(d3ddevice, TexBmps[j], Usage.SoftwareProcessing, Pool.Managed);
                    if (!LevelData.TextureBitmaps.ContainsKey(tex))
                        LevelData.TextureBitmaps.Add(tex, TexBmps);
                    if (!LevelData.Textures.ContainsKey(tex))
                        LevelData.Textures.Add(tex, texs);
                    if (string.IsNullOrEmpty(LevelData.leveltexs))
                        LevelData.leveltexs = tex;
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.GetType().Name + ": " + ex.Message + "\nLog file has been saved to " + System.IO.Path.Combine(Environment.CurrentDirectory, "SADXLVL.log") + ".\nSend this to MainMemory on the Sonic Retro forums.",
                    "SADXLVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.WriteAllText("SADXLVL.log", ex.ToString());
                initerror = true;
            }
#endif
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (initerror)
            {
                Close();
                return;
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> group = ini[level];
            if (LevelData.geo !=null)
            {
                Dictionary<string, Dictionary<string, string>> geoini = new Dictionary<string,Dictionary<string,string>>();
                geoini.Add(string.Empty, new Dictionary<string, string>() { { "LandTable", LevelData.geo.Name } });
                LevelData.geo.Save(geoini, Path.Combine(Path.GetDirectoryName(group["LevelGeo"]), "Animations"));
                IniFile.Save(geoini, group["LevelGeo"]);
            }
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
            if (LevelData.LevelItems != null)
            {
                for (int i = 0; i < LevelData.LevelItems.Count; i++)
                    LevelData.LevelItems[i].Render(d3ddevice, transform, LevelData.Textures[LevelData.leveltexs], SelectedItems.Contains(LevelData.LevelItems[i]));
            }
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
                    float dist = LevelData.LevelItems[i].CheckHit(Near, Far, viewport, proj, view);
                    if (dist > 0 & dist < mindist)
                    {
                        mindist = dist;
                        item = LevelData.LevelItems[i];
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
                    if (SelectedItems.Count > 0)
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
                    //pasteToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SADXLVLObjectList");
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
                    item.Position.X += horiz.X + verti.X;
                    item.Position.Y += horiz.Y + verti.Y;
                    item.Position.Z += horiz.Z + verti.Z;
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

        private void SelectedItemChanged()
        {
            PropertyWindow.propertyGrid1.SelectedObjects = SelectedItems.ToArray();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loaded)
            {
                switch (MessageBox.Show(this, "Do you want to save?", "SADXLVL2", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Item> selitems = new List<Item>();
            foreach (Item item in SelectedItems)
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
                item.Delete();
            SelectedItems.Clear();
            SelectedItemChanged();
            DrawLevel();
        }
    }
}