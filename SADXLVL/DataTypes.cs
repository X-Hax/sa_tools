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
            return COL.Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
        }

        public override void Render(Device dev, MatrixStack transform, bool selected)
        {
            COL.Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh);
            if (selected)
                COL.Model.DrawModelInvert(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh);
        }

        public override void Paste()
        {
            LevelData.LevelItems.Add(this);
            LevelData.geo.COL.Add(COL);
        }

        public override void Delete()
        {
            LevelData.geo.COL.Remove(COL);
            LevelData.LevelItems.Remove(this);
        }

        [Browsable(true)]
        [DisplayName("Import Model")]
        public void ImportModel()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog() { DefaultExt = "obj", Filter = "OBJ Files|*.obj;*.objf", RestoreDirectory = true };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                COL.Model.Attach = SonicRetro.SAModel.Direct3D.Extensions.obj2nj(dlg.FileName);
                COL.CalculateBounds();
                Mesh = COL.Model.Attach.CreateD3DMesh(LevelData.MainForm.d3ddevice);
            }
        }

        //[Browsable(true)]
        [DisplayName("Export Model")]
        public void ExportModel()
        {

        }

        [Browsable(true)]
        [DisplayName("Edit Materials")]
        public void EditMaterials()
        {
            using (MaterialEditor pw = new MaterialEditor(COL.Model.Attach.Material, LevelData.TextureBitmaps[LevelData.leveltexs]))
            {
                pw.ShowDialog();
            }
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

    public class SETItem : Item
    {
        public override Vertex Position
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override Rotation Rotation
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override void Paste()
        {
            LevelData.SETItems[LevelData.Character].Add(this);
        }

        public override void Delete()
        {
            LevelData.SETItems[LevelData.Character].Remove(this);
        }

        public override float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(Device dev, MatrixStack transform, bool selected)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StartPosItem : Item
    {
        private Object Model;
        private Microsoft.DirectX.Direct3D.Mesh[] Meshes;
        private string texture;
        private float offset;

        public StartPosItem(Object model, string textures, float offset, Vertex position, int yrot, Device dev)
        {
            Model = model;
            Object[] models = model.GetObjects();
            Meshes = new Microsoft.DirectX.Direct3D.Mesh[models.Length];
            for (int i = 0; i < models.Length; i++)
                if (models[i].Attach != null)
                    Meshes[i] = models[i].Attach.CreateD3DMesh(dev);
            texture = textures;
            this.offset = offset;
            Position = position;
            YRot = yrot;
        }

        public override Vertex Position { get; set; }

        private int YRot;
        public override Rotation Rotation
        {
            get
            {
                return new Rotation(0, YRot, 0);
            }
            set
            {
                YRot = value.Y;
            }
        }

        public override bool CanCopy { get { return false; } }

        public override void Paste()
        {
            throw new System.NotImplementedException();
        }

        public override void Delete()
        {
            throw new System.NotImplementedException();
        }

        public override float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            MatrixStack transform = new MatrixStack();
            transform.TranslateLocal(0, offset, 0);
            transform.TranslateLocal(Position.ToVector3());
            transform.RotateYawPitchRollLocal(LevelData.BAMSToRad(YRot), 0, 0);
            return Model.CheckHit(Near, Far, Viewport, Projection, View, transform, Meshes);
        }

        public override void Render(Device dev, MatrixStack transform, bool selected)
        {
            transform.Push();
            transform.TranslateLocal(0, offset, 0);
            transform.TranslateLocal(Position.ToVector3());
            transform.RotateYawPitchRollLocal(LevelData.BAMSToRad(YRot), 0, 0);
            Model.DrawModelTree(dev, transform, LevelData.Textures[texture], Meshes);
            if (selected)
                Model.DrawModelTreeInvert(dev, transform, LevelData.Textures[texture], Meshes);
            transform.Pop();
        }
    }
}