using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using SonicRetro.SAModel.Direct3D;
using puyo_tools;
using VrSharp.PvrTexture;

namespace SonicRetro.SAModel.SADXLVL
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
            File.WriteAllText("SADXLVL.log", e.Exception.ToString());
            if (MessageBox.Show("Unhandled " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SADXLVL Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
                Close();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("SADXLVL.log", e.ExceptionObject.ToString());
            MessageBox.Show("Unhandled Exception: " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXLVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        Device d3ddevice;
        Dictionary<string, Dictionary<string, string>> ini;
        Camera cam = new Camera();
        string level;
        bool loaded;
        int interval = 20;
        FillMode rendermode;
        Cull cullmode = Cull.None;
        byte[] file;
        LandTable geo;
        Dictionary<string, Bitmap[]> TextureBitmaps;
        Dictionary<string, Texture[]> Textures;
        List<Microsoft.DirectX.Direct3D.Mesh> meshes;

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
                file = File.ReadAllBytes(ini[string.Empty]["file"]);
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
            Text = "SADXLVL - Loading " + level + "...";
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
                string tmpfile = Path.GetTempFileName();
                Dictionary<string, string> group = ini[level];
                string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
                string levelact = group.GetValueOrDefault("LevelID", "0000");
                byte levelnum = byte.Parse(levelact.Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                byte actnum = byte.Parse(levelact.Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                cam = new Camera();
                int geoaddr = int.Parse(group["LevelGeo"], System.Globalization.NumberStyles.HexNumber);
                if (geoaddr == 0)
                    geo = null;
                else
                {
                    geo = new LandTable(file, geoaddr, 0x400000, true);
                    meshes = new List<Microsoft.DirectX.Direct3D.Mesh>();
                    foreach (COL item in geo.COL)
                        meshes.Add(item.Object.Attach.CreateD3DMesh(d3ddevice));
                }
                TextureBitmaps = new Dictionary<string, Bitmap[]>();
                Textures = new Dictionary<string, Texture[]>();
                if (geo != null && !string.IsNullOrEmpty(geo.TextureFileName))
                {
                    Bitmap[] TexBmps = GetTextures(System.IO.Path.Combine(syspath, geo.TextureFileName) + ".PVM");
                    Texture[] texs = new Texture[TexBmps.Length];
                    for (int j = 0; j < TexBmps.Length - 1; j++)
                        texs[j] = new Texture(d3ddevice, TexBmps[j], Usage.SoftwareProcessing, Pool.Managed);
                    if (!TextureBitmaps.ContainsKey(geo.TextureFileName))
                        TextureBitmaps.Add(geo.TextureFileName, TexBmps);
                    if (!Textures.ContainsKey(geo.TextureFileName))
                        Textures.Add(geo.TextureFileName, texs);
                }
                int ptraddr = 0x50f010;
                int lvladdress = 0;
                uint listaddress = 0;
                while (BitConverter.ToUInt32(file, ptraddr) != 0)
                {
                    lvladdress = (int)(BitConverter.ToUInt32(file, ptraddr) - 0x400000);
                    if (BitConverter.ToUInt16(file, lvladdress) == ((levelnum << 8) | actnum))
                        break;
                    ptraddr += 4;
                }
                listaddress = BitConverter.ToUInt32(file, lvladdress + 4);
                if (listaddress != 0)
                    listaddress -= 0x400000;
                int numentries = BitConverter.ToUInt16(file, lvladdress + 2);
                if (numentries > 0 & listaddress > 0)
                {
                    for (int objaddress = (int)listaddress; objaddress < listaddress + (numentries * 8); objaddress += 8)
                    {
                        if (BitConverter.ToUInt32(file, objaddress) == 0 | BitConverter.ToUInt32(file, objaddress + 4) == 0)
                            continue;
                        string texname = GetCString(file, (int)(BitConverter.ToUInt32(file, objaddress) - 0x400000));
                        Bitmap[] TexBmps = GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
                        Texture[] texs = new Texture[TexBmps.Length];
                        for (int j = 0; j < TexBmps.Length - 1; j++)
                            texs[j] = new Texture(d3ddevice, TexBmps[j], Usage.SoftwareProcessing, Pool.Managed);
                        if (BitConverter.ToUInt32(file, objaddress + 4) == geo.TextureList & string.IsNullOrEmpty(geo.TextureFileName)) geo.TextureFileName = texname;
                        if (!TextureBitmaps.ContainsKey(texname))
                            TextureBitmaps.Add(texname, TexBmps);
                        if (!Textures.ContainsKey(texname))
                            Textures.Add(texname, texs);
                    }
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
            loaded = true;
            UseWaitCursor = false;
            Enabled = true;
            Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Not implemented.");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public Bitmap[] GetTextures(string filename)
        {
            List<Bitmap> functionReturnValue = new List<Bitmap>();
            PVM pvmfile = new PVM();
            Stream pvmdata = new MemoryStream(File.ReadAllBytes(filename));
            pvmdata = pvmfile.TranslateData(ref pvmdata);
            ArchiveFileList pvmentries = pvmfile.GetFileList(ref pvmdata);
            foreach (ArchiveFileList.Entry file in pvmentries.Entries)
            {
                byte[] data = new byte[file.Length];
                pvmdata.Seek(file.Offset, SeekOrigin.Begin);
                pvmdata.Read(data, 0, (int)file.Length);
                PvrTexture vrfile = new PvrTexture(data);
                functionReturnValue.Add(vrfile.GetTextureAsBitmap());
            }
            return functionReturnValue.ToArray();
        }

        public string GetCString(byte[] file, int address)
        {
            int textsize = 0;
            while (file[address + textsize] > 0)
                textsize += 1;

            return System.Text.Encoding.ASCII.GetString(file, address, textsize);
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
            if (geo != null)
            {
                for (int i = 0; i < geo.COL.Length; i++)
                {
                    if (geo.COL[i].Object.Attach == null)
                        continue;
                    geo.COL[i].Object.DrawModel(d3ddevice, transform, Textures[geo.TextureFileName], meshes[i]);
                }
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
            DrawLevel();
        }
    }
}