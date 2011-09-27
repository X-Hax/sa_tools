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
            ObjectHelper.QuestionMark = new Texture(d3ddevice, Properties.Resources.UnknownImg, Usage.SoftwareProcessing, Pool.Managed);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loaded)
                switch (MessageBox.Show(this, "Do you want to save?", "SADXLVL2", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:
                        return;
                }
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
            if (loaded)
                switch (MessageBox.Show(this, "Do you want to save?", "SADXLVL2", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:
                        return;
                }
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
                LevelData.Character = 0;
                Dictionary<string, string> group = ini[level];
                string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
                string levelact = group.GetValueOrDefault("LevelID", "0000");
                byte levelnum = byte.Parse(levelact.Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                byte actnum = byte.Parse(levelact.Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                LevelData.leveltexs = null;
                cam = new Camera();
                if (!group.ContainsKey("LevelGeo"))
                    LevelData.geo = null;
                else
                {
                    LevelData.geo = LandTable.LoadFromFile(group["LevelGeo"]);
                    LevelData.LevelItems = new List<LevelItem>();
                    foreach (COL item in LevelData.geo.COL)
                        LevelData.LevelItems.Add(new LevelItem(item, d3ddevice));
                }
                LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
                LevelData.Textures = new Dictionary<string, Texture[]>();
                if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName))
                {
                    BMPInfo[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, LevelData.geo.TextureFileName) + ".PVM");
                    Texture[] texs = new Texture[TexBmps.Length];
                    for (int j = 0; j < TexBmps.Length; j++)
                        texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
                    if (!LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
                        LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
                    if (!LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
                        LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
                    LevelData.leveltexs = LevelData.geo.TextureFileName;
                }
                LevelData.StartPositions = new StartPosItem[LevelData.Characters.Length];
                for (int i = 0; i < LevelData.StartPositions.Length; i++)
                {
                    Dictionary<string, Dictionary<string, string>> posini = IniFile.Load(ini[string.Empty][LevelData.Characters[i] + "start"]);
                    Vertex pos = new Vertex();
                    int rot = 0;
                    if (posini.ContainsKey(levelact))
                    {
                        pos = new Vertex(posini[levelact]["Position"]);
                        rot = int.Parse(posini[levelact]["YRotation"], System.Globalization.NumberStyles.HexNumber);
                    }
                    Dictionary<string, Dictionary<string, string>> mdlini = IniFile.Load(ini[string.Empty][LevelData.Characters[i] + "mdl"]);
                    LevelData.StartPositions[i] = new StartPosItem(new Object(mdlini, mdlini[string.Empty]["Root"]), ini[string.Empty][LevelData.Characters[i] + "tex"], float.Parse(ini[string.Empty][LevelData.Characters[i] + "height"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo), pos, rot, d3ddevice);
                    Dictionary<string, Dictionary<string, string>> texini = IniFile.Load(ini[string.Empty][LevelData.Characters[i] + "texlist"]);
                    int ti = 0;
                    while (texini.ContainsKey(ti.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                    {
                        string texname = texini[ti.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Name"];
                        if (!LevelData.TextureBitmaps.ContainsKey(texname))
                        {
                            BMPInfo[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
                            Texture[] texs = new Texture[TexBmps.Length];
                            for (int j = 0; j < TexBmps.Length; j++)
                                texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
                            LevelData.TextureBitmaps.Add(texname, TexBmps);
                            LevelData.Textures.Add(texname, texs);
                        }
                        ti++;
                    }
                    if (i == 0 & levelnum == 19)
                    {
                        mdlini = IniFile.Load(ini[string.Empty]["supermdl"]);
                        LevelData.StartPositions[i] = new StartPosItem(new Object(mdlini, mdlini[string.Empty]["Root"]), ini[string.Empty]["supertex"], float.Parse(ini[string.Empty]["superheight"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo), pos, rot, d3ddevice);
                    }
                }
                Dictionary<string, Dictionary<string, string>> objtexini = IniFile.Load(ini[string.Empty]["objtexlist"]);
                int oti = 0;
                while (objtexini.ContainsKey(oti.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                {
                    string texname = objtexini[oti.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Name"];
                    if (!string.IsNullOrEmpty(texname) & !LevelData.TextureBitmaps.ContainsKey(texname))
                    {
                        BMPInfo[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
                        Texture[] texs = new Texture[TexBmps.Length];
                        for (int j = 0; j < TexBmps.Length; j++)
                            texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
                        LevelData.TextureBitmaps.Add(texname, TexBmps);
                        LevelData.Textures.Add(texname, texs);
                    }
                    oti++;
                }
                foreach (string file in Directory.GetFiles(ini[string.Empty]["leveltexlists"]))
                {
                    Dictionary<string, Dictionary<string, string>> texini = IniFile.Load(file);
                    if (texini[string.Empty]["Level"] != levelact) continue;
                    int ti = 0;
                    while (texini.ContainsKey(ti.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                    {
                        string texname = texini[ti.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Name"];
                        if (!LevelData.TextureBitmaps.ContainsKey(texname))
                        {
                            BMPInfo[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
                            Texture[] texs = new Texture[TexBmps.Length];
                            for (int j = 0; j < TexBmps.Length; j++)
                                texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
                            LevelData.TextureBitmaps.Add(texname, TexBmps);
                            LevelData.Textures.Add(texname, texs);
                        }
                        ti++;
                    }
                }
                if (group.ContainsKey("Textures"))
                {
                    string[] textures = group["Textures"].Split(',');
                    foreach (string tex in textures)
                    {
                        if (!LevelData.TextureBitmaps.ContainsKey(tex))
                        {
                            BMPInfo[] TexBmps = LevelData.GetTextures(System.IO.Path.Combine(syspath, tex) + ".PVM");
                            Texture[] texs = new Texture[TexBmps.Length];
                            for (int j = 0; j < TexBmps.Length; j++)
                                texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
                            LevelData.TextureBitmaps.Add(tex, TexBmps);
                            LevelData.Textures.Add(tex, texs);
                        }
                        if (string.IsNullOrEmpty(LevelData.leveltexs))
                            LevelData.leveltexs = tex;
                    }
                }
                LevelData.ObjDefs = new List<ObjectDefinition>();
                Dictionary<string, Dictionary<string, string>> objdefini = IniFile.Load(ini[string.Empty]["objdefs"]);
                if (File.Exists(group.GetValueOrDefault("ObjList", string.Empty)))
                {
                    Dictionary<string, Dictionary<string, string>> objlstini = IniFile.Load(group["ObjList"]);
                    if (!Directory.Exists("dllcache"))
                        Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;
                    int ID = 0;
                    while (objlstini.ContainsKey(ID.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                    {
                        string codeaddr = objlstini[ID.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Code"];
                        if (!objdefini.ContainsKey(codeaddr))
                            codeaddr = "0";
                        Dictionary<string, string> defgroup = objdefini[codeaddr];
                        ObjectDefinition def = null;
                        if (defgroup.ContainsKey("codefile"))
                        {
                            string ty = defgroup["codetype"];
                            string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                            DateTime modDate = DateTime.MinValue;
                            if (System.IO.File.Exists(dllfile))
                                modDate = System.IO.File.GetLastWriteTime(dllfile);
                            string fp = defgroup["codefile"].Replace('/', System.IO.Path.DirectorySeparatorChar);
                            if (modDate >= System.IO.File.GetLastWriteTime(fp))
                                def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                            else
                            {
                                string ext = System.IO.Path.GetExtension(fp);
                                CodeDomProvider pr = null;
                                switch (ext.ToLowerInvariant())
                                {
                                    case ".cs":
                                        pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                        break;
                                    case ".vb":
                                        pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                        break;
                                }
                                if (pr != null)
                                {
                                    CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof(Vector3)).Location, Assembly.GetAssembly(typeof(Texture)).Location, Assembly.GetAssembly(typeof(D3DX)).Location, Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof(LandTable)).Location, Assembly.GetAssembly(typeof(Camera)).Location });
                                    para.GenerateExecutable = false;
                                    para.GenerateInMemory = false;
                                    para.IncludeDebugInformation = true;
                                    para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
                                    CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
                                    if (res.Errors.HasErrors)
                                        def = new DefaultObjectDefinition();
                                    else
                                        def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
                                }
                                else
                                    def = new DefaultObjectDefinition();
                            }
                        }
                        else
                            def = new DefaultObjectDefinition();
                        LevelData.ObjDefs.Add(def);
                        def.Init(defgroup, objlstini[ID.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Name"], d3ddevice);
                        ID++;
                    }
                    if (LevelData.ObjDefs.Count > 0)
                    {
                        LevelData.SETName = group.GetValueOrDefault("SETName", levelact);
                        string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + "{0}.bin");
                        LevelData.SETItems = new List<SETItem>[LevelData.SETChars.Length];
                        for (int i = 0; i < LevelData.SETChars.Length; i++)
                        {
                            List<SETItem> list = new List<SETItem>();
                            if (File.Exists(string.Format(setstr, LevelData.SETChars[i])))
                            {
                                byte[] setfile = File.ReadAllBytes(string.Format(setstr, LevelData.SETChars[i]));
                                int count = BitConverter.ToInt32(setfile, 0);
                                for (int j = 0; j < count; j++)
                                {
                                    SETItem ent = new SETItem(setfile, (j * 0x20) + 0x20);
                                    Type t = LevelData.ObjDefs[ent.ID].ObjectType;
                                    if (ent.GetType() != t)
                                        ent = (SETItem)Activator.CreateInstance(t, new object[] { ent.GetBytes(), 0 });
                                    list.Add(ent);
                                }
                            }
                            LevelData.SETItems[i] = list;
                        }
                    }
                    else
                        LevelData.SETItems = null;
                }
                else
                    LevelData.SETItems = null;
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
            levelPieceToolStripMenuItem.Enabled = LevelData.geo != null;
            objectToolStripMenuItem.Enabled = LevelData.SETItems != null;
            loaded = true;
            SelectedItems = new List<Item>();
            UseWaitCursor = false;
            Enabled = true;
            DrawLevel();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loaded)
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> group = ini[level];
            string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
            string levelact = group.GetValueOrDefault("LevelID", "0000");
            byte levelnum = byte.Parse(levelact.Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            byte actnum = byte.Parse(levelact.Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            if (LevelData.geo != null)
                LevelData.geo.SaveToFile(group["LevelGeo"], ModelFormat.SA1);
            for (int i = 0; i < LevelData.StartPositions.Length; i++)
            {
                Dictionary<string, Dictionary<string, string>> posini = IniFile.Load(ini[string.Empty][LevelData.Characters[i] + "start"]);
                if (posini.ContainsKey(levelact))
                    posini.Remove(levelact);
                if (LevelData.StartPositions[i].Position.X != 0 & LevelData.StartPositions[i].Position.Y != 0 & LevelData.StartPositions[i].Position.Z != 0 & LevelData.StartPositions[i].Rotation.Y != 0)
                {
                    posini.Add(levelact, new Dictionary<string, string>()
                    { { "Position", LevelData.StartPositions[i].Position.ToString() },
                    { "YRotation", LevelData.StartPositions[i].Rotation.Y.ToString("X8") } });
                }
                IniFile.Save(posini, ini[string.Empty][LevelData.Characters[i] + "start"]);
            }
            if (LevelData.SETItems != null)
            {
                for (int i = 0; i < LevelData.SETItems.Length; i++)
                {
                    string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
                    if (File.Exists(setstr))
                        File.Delete(setstr);
                    if (LevelData.SETItems[i].Count == 0)
                        continue;
                    List<byte> file = new List<byte>(LevelData.SETItems[i].Count * 0x20 + 0x20);
                    file.AddRange(BitConverter.GetBytes(LevelData.SETItems[i].Count));
                    file.Align(0x20);
                    foreach (SETItem item in LevelData.SETItems[i])
                        file.AddRange(item.GetBytes());
                    File.WriteAllBytes(setstr, file.ToArray());
                }
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
                        LevelData.LevelItems[i].Render(d3ddevice, transform, SelectedItems.Contains(LevelData.LevelItems[i]));
                }
            LevelData.StartPositions[LevelData.Character].Render(d3ddevice, transform, SelectedItems.Contains(LevelData.StartPositions[LevelData.Character]));
            if (LevelData.SETItems != null)
                foreach (SETItem item in LevelData.SETItems[LevelData.Character])
                    item.Render(d3ddevice, transform, SelectedItems.Contains(item));
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
            dist = LevelData.StartPositions[LevelData.Character].CheckHit(Near, Far, viewport, proj, view);
            if (dist > 0 & dist < mindist)
            {
                mindist = dist;
                item = LevelData.StartPositions[LevelData.Character];
            }
            if (LevelData.SETItems != null)
                foreach (SETItem setitem in LevelData.SETItems[LevelData.Character])
                {
                    dist = setitem.CheckHit(Near, Far, viewport, proj, view);
                    if (dist > 0 & dist < mindist)
                    {
                        mindist = dist;
                        item = setitem;
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
                    item.Position = new EditableVertex(
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
                item.Position = new EditableVertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
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

        private void characterToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            LevelData.Character = characterToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
            foreach (ToolStripMenuItem item in characterToolStripMenuItem.DropDownItems)
                item.Checked = false;
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
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
            item.Position = new EditableVertex(pos.X, pos.Y, pos.Z);
        }

        private void objectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SETItem item = new SETItem();
            Vector3 pos = cam.Position + (-20 * cam.Look);
            item.Position = new EditableVertex(pos.X, pos.Y, pos.Z);
            LevelData.SETItems[LevelData.Character].Add(item);
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
                        mtlstream.WriteLine("Tr " + obj.Attach.Material[j].DiffuseColor.A / 255f);
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
                    objstream.WriteLine("usemtl material_" + obj.Attach.Material[obj.Attach.Mesh[j].MaterialID].Name);
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