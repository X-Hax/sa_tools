using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using PAKLib;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SA_Tools;
using System.Linq;

namespace SA2StageSelEdit
{
    public partial class MainForm : Form
    {
        static readonly int chaoicon = 6;
        static readonly int questionicon = 9;
        static readonly int selecticon = 15;
		static readonly Dictionary<SA2Characters, int> charicons = new Dictionary<SA2Characters, int>() {
			{ SA2Characters.Sonic, 12 },
			{ SA2Characters.Shadow, 11 },
			{ SA2Characters.Tails, 13 },
			{ SA2Characters.Eggman, 7 },
			{ SA2Characters.Knuckles, 8 },
			{ SA2Characters.Rouge, 10 },
			{ SA2Characters.MechTails, 13 },
			{ SA2Characters.MechEggman, 7 }
		};

        public MainForm()
        {
            InitializeComponent();
        }

        Device d3ddevice;
        Camera cam = new Camera();
        static readonly CustomVertex.PositionTextured[] SquareVerts = {
        new CustomVertex.PositionTextured(0, 48, 0, 0, 1),
        new CustomVertex.PositionTextured(0, 0, 0, 0, 0),
        new CustomVertex.PositionTextured(48, 48, 0, 1, 1),
        new CustomVertex.PositionTextured(0, 0, 0, 0, 0),
        new CustomVertex.PositionTextured(48, 0, 0, 1, 0),
        new CustomVertex.PositionTextured(48, 48, 0, 1, 1)};
        static Microsoft.DirectX.Direct3D.Mesh SquareMesh;
        string filename;
        List<StageSelectLevel> levels;
        Texture bgtex;
        Texture[] uitexs;
        int selected;

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
            SquareMesh = new Mesh(2, 6, MeshFlags.Managed, CustomVertex.PositionTextured.Format, d3ddevice);
            List<short> ib = new List<short>();
            for (int i = 0; i < SquareVerts.Length; i++)
                ib.Add((short)(i));
            SquareMesh.SetVertexBufferData(SquareVerts, LockFlags.None);
            SquareMesh.SetIndexBufferData(ib.ToArray(), LockFlags.None);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog() { DefaultExt = "ini", Filter = "INI Files|*.ini" })
                if (fd.ShowDialog(this) == DialogResult.OK)
                {
					IniData ini = IniSerializer.Deserialize<IniData>(fd.FileName);
					filename = Path.Combine(Path.GetDirectoryName(fd.FileName), 
						ini.Files.First((item) => item.Value.Type == "stageselectlist").Value.Filename);
					levels = new List<StageSelectLevel>(StageSelectLevelList.Load(filename));
                    string resdir = Path.Combine(Path.GetDirectoryName(fd.FileName), ini.SystemFolder);
                    using (Stream str = new MemoryStream(
                        new PAKFile(Path.Combine(resdir, @"SOC\stageMapBG.pak")).Files.Find(
                        (a) => a.Name.Equals(@"stagemapbg\stagemap.dds")).Data))
                        bgtex = TextureLoader.FromStream(d3ddevice, str);
                    if (File.Exists(Path.Combine(resdir, @"PRS\stageMap.pak")))
                    {
                        List<PAKFile.File> files = new PAKFile(Path.Combine(resdir, @"PRS\stageMap.pak")).Files;
                        byte[] inf = files.Find((a) => a.Name.Equals(@"stagemap\stagemap.inf")).Data;
                        uitexs = new Texture[inf.Length / 0x3C];
                        for (int i = 0; i < uitexs.Length; i++)
                            using (Stream str = new MemoryStream(files.Find(
                                (a) => a.Name.Equals(@"stagemap\" + Encoding.ASCII.GetString(inf, i * 0x3C, 0x1c).TrimEnd('\0') + ".dds")).Data))
                                uitexs[i] = TextureLoader.FromStream(d3ddevice, str);
                    }
                    else
                    {
						BMPInfo[] bmps = TextureArchive.GetTextures(Path.Combine(resdir, "stageMap.prs"));
                        uitexs = new Texture[bmps.Length];
						for (int i = 0; i < bmps.Length; i++)
							uitexs[i] = new Texture(d3ddevice, bmps[i].Image, Usage.SoftwareProcessing, Pool.Managed);
                    }
                    saveToolStripMenuItem.Enabled = panel1.Enabled = panel2.Enabled = true;
					selected = 0;
                    level.SelectedIndex = (int)levels[selected].Level;
                    character.SelectedIndex = (int)levels[selected].Character;
                    column.Value = levels[selected].Column;
                    row.Value = levels[selected].Row;
                    DrawLevel();
                }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
			levels.ToArray().Save(filename);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { DrawLevel(); }

        void DrawLevel()
        {
            if (string.IsNullOrEmpty(filename)) return;
            d3ddevice.SetTransform(TransformType.Projection, Matrix.OrthoOffCenterRH(0, panel1.Width, panel1.Height, 0, 1, 10000));
            d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
            d3ddevice.Transform.World = Matrix.Identity;
            d3ddevice.Material = new Material() { Ambient = Color.White, Diffuse = Color.White };
            d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Magenta.ToArgb(), 1, 0);
            d3ddevice.RenderState.ZBufferEnable = true;
            d3ddevice.VertexFormat = CustomVertex.PositionTextured.Format;
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
            using (Sprite spr = new Sprite(d3ddevice))
            {
                spr.Begin(SpriteFlags.None);
                spr.Draw2D(bgtex, PointF.Empty, 0, PointF.Empty, Color.White);
                spr.Flush();
                spr.End();
            }
            d3ddevice.SetTexture(0, bgtex);
            SquareMesh.DrawSubset(0);
            foreach (StageSelectLevel item in levels)
            {
                int tex = 0;
                switch (item.Level)
                {
                    case SA2LevelIDs.GreenHill: // Green Hill
                        tex = questionicon;
                        break;
                    case SA2LevelIDs.ChaoWorld: // Chao World
                        tex = chaoicon;
                        break;
                    default:
                        tex = charicons[item.Character];
                        break;
                }
                d3ddevice.Transform.World = Matrix.Translation(item.Column * 60 + 110, item.Row * 60 + 10, -1);
                d3ddevice.SetTexture(0, uitexs[tex]);
                SquareMesh.DrawSubset(0);
            }
            d3ddevice.Transform.World = Matrix.Scaling(2, 2, 1) * Matrix.Translation(levels[selected].Column * 60 + 104, levels[selected].Row * 60 + 4, -1);
            d3ddevice.SetTexture(0, uitexs[selecticon]);
            SquareMesh.DrawSubset(0);
            d3ddevice.EndScene(); //all drawings before this line
            d3ddevice.Present();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(filename)) return;
            bool hit = false;
            foreach (StageSelectLevel item in levels)
                if (new Rectangle(item.Column * 60 + 110, item.Row * 60 + 10, 48, 48).Contains(e.Location))
                    hit = true;
            panel1.Cursor = hit ? Cursors.Hand : Cursors.Default;
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(filename)) return;
            foreach (StageSelectLevel item in levels)
                if (new Rectangle(item.Column * 60 + 110, item.Row * 60 + 10, 48, 48).Contains(e.Location))
                {
                    selected = levels.IndexOf(item);
                    DrawLevel();
                    level.SelectedIndex = (int)item.Level;
                    character.SelectedIndex = (int)item.Character;
                    column.Value = item.Column;
                    row.Value = item.Row;
                    return;
                }
        }

        private void level_SelectedIndexChanged(object sender, EventArgs e)
        {
            levels[selected].Level = (SA2LevelIDs)level.SelectedIndex;
            DrawLevel();
        }

        private void character_SelectedIndexChanged(object sender, EventArgs e)
        {
            levels[selected].Character = (SA2Characters)character.SelectedIndex;
            DrawLevel();
        }

        private void column_ValueChanged(object sender, EventArgs e)
        {
            levels[selected].Column = (int)column.Value;
            DrawLevel();
        }

        private void row_ValueChanged(object sender, EventArgs e)
        {
            levels[selected].Row = (int)row.Value;
            DrawLevel();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    saveToolStripMenuItem_Click(this, EventArgs.Empty);
                    break;
            }
        }
    }

    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Up { get; private set; }
        public Vector3 Look { get; private set; }
        public Vector3 Right { get; private set; }

        public Camera() { }

        public Matrix ToMatrix()
        {
            Up = new Vector3(0, 1, 0);
            Look = new Vector3(0, 0, 1);
            Right = new Vector3(1, 0, 0);
            Matrix ViewMatrix = Matrix.Identity;
            ViewMatrix.M11 = Right.X;
            ViewMatrix.M12 = Up.X;
            ViewMatrix.M13 = Look.X;
            ViewMatrix.M21 = Right.Y;
            ViewMatrix.M22 = Up.Y;
            ViewMatrix.M23 = Look.Y;
            ViewMatrix.M31 = Right.Z;
            ViewMatrix.M32 = Up.Z;
            ViewMatrix.M33 = Look.Z;
            ViewMatrix.M41 = -Vector3.Dot(Position, Right);
            ViewMatrix.M42 = -Vector3.Dot(Position, Up);
            ViewMatrix.M43 = -Vector3.Dot(Position, Look);
            return ViewMatrix;
        }
    }
}