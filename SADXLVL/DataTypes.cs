using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using System.ComponentModel;

namespace SonicRetro.SAModel.SADXLVL2
{
    public class LevelItem : Item
    {
        [Browsable(false)]
        private COL COL { get; set; }
        [Browsable(false)]
        private Microsoft.DirectX.Direct3D.Mesh Mesh { get; set; }

        public LevelItem(COL col, Device dev)
        {
            COL = col;
            Mesh = col.Model.Attach.CreateD3DMesh(dev);
        }

        public override Vertex Position
        {
            get
            {
                return COL.Model.Position;
            }
            set
            {
                COL.Model.Position = value;
                COL.CalculateBounds();
            }
        }

        public override Rotation Rotation
        {
            get
            {
                return COL.Model.Rotation;
            }
            set
            {
                COL.Model.Rotation = value;
                COL.CalculateBounds();
            }
        }

        public override float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            Matrix transform = Matrix.Identity;
            transform.Multiply(Matrix.RotationYawPitchRoll(LevelData.BAMSToRad(Rotation.Y), LevelData.BAMSToRad(Rotation.X), LevelData.BAMSToRad(Rotation.Z)));
            transform.Multiply(Matrix.Translation(Position.ToVector3()));
            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform));
            IntersectInformation info;
            if (!Mesh.Intersect(pos, dir, out info)) return -1;
            return info.Dist;
        }

        public override void Render(Device dev, MatrixStack transform, Texture[] textures, bool selected)
        {
            COL.Model.DrawModel(dev, transform, textures, Mesh);
            if (selected)
                COL.Model.DrawModelInvert(dev, transform, textures, Mesh);
        }

        [Browsable(true)]
        [DisplayName("Import Model")]
        public void ImportModel()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog() { DefaultExt = "obj", Filter = "OBJ Files|*.obj;*.objf", RestoreDirectory = true };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                COL.Model.Attach = SonicRetro.SAModel.Direct3D.Extensions.obj2nj(dlg.FileName);
                COL.Model.Attach.CalculateBounds();
                COL.CalculateBounds();
                Mesh = COL.Model.Attach.CreateD3DMesh(LevelData.MainForm.d3ddevice);
            }
        }

        //[Browsable(true)]
        [DisplayName("Export Model")]
        public void ExportModel()
        {

        }

        public string Flags
        {
            get
            {
                return COL.Flags.ToString("X8");
            }
            set
            {
                COL.Flags = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }

        public bool Visible
        {
            get
            {
                return (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible;
            }
            set
            {
                COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Visible) | (value ? SurfaceFlags.Visible : 0);
            }
        }

        public bool Solid
        {
            get
            {
                return (COL.SurfaceFlags & SurfaceFlags.Solid) == SurfaceFlags.Solid;
            }
            set
            {
                COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Solid) | (value ? SurfaceFlags.Solid : 0);
            }
        }

        public bool Water
        {
            get
            {
                return (COL.SurfaceFlags & SurfaceFlags.Water) == SurfaceFlags.Water;
            }
            set
            {
                COL.SurfaceFlags = (COL.SurfaceFlags & ~SurfaceFlags.Water) | (value ? SurfaceFlags.Water : 0);
            }
        }
    }
}
