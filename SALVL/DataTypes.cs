using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel.SALVL
{
    public class LevelItem : Item
    {
        [Browsable(false)]
        private COL COL { get; set; }
        [Browsable(false)]
        private Microsoft.DirectX.Direct3D.Mesh Mesh { get; set; }

        private Device dev;

        public LevelItem(Device dev)
        {
            this.dev = dev;
            COL = new COL();
            COL.Model = new Object();
            Position = new EditableVertex();
            Rotation = new EditableRotation();
            ImportModel();
            Paste();
        }

        public LevelItem(COL col, Device dev)
        {
            COL = col;
            Mesh = col.Model.Attach.CreateD3DMesh(dev);
            Position = new EditableVertex(COL.Model.Position);
            Rotation = new EditableRotation(COL.Model.Rotation);
            this.dev = dev;
        }

        private EditableVertex position;
        public override EditableVertex Position { get { return position; } set { position = value; COL.Model.Position = value.ToVertex(); } }

        private EditableRotation rotation;
        public override EditableRotation Rotation { get { return rotation; } set { rotation = value; COL.Model.Rotation = value.ToRotation(); } }

        public override float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return COL.Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
        }

        public override void Render(Device dev, MatrixStack transform, bool selected)
        {
            if (!string.IsNullOrEmpty(LevelData.leveltexs))
                COL.Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible);
            else
                COL.Model.DrawModel(dev, transform, null, Mesh, (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible);
            if (selected)
                COL.Model.DrawModelInvert(dev, transform, Mesh, (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible);
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
                Mesh = COL.Model.Attach.CreateD3DMesh(dev);
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
            using (MaterialEditor pw = new MaterialEditor(COL.Model.Attach.Material.ToArray(), LevelData.TextureBitmaps[LevelData.leveltexs]))
                pw.ShowDialog();
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
                COL.Model.Flags = (COL.Model.Flags & ~ObjectFlags.NoDisplay) | (value ? 0 : ObjectFlags.NoDisplay);
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

        public void Save()
        {
            COL.Model.Position = Position.ToVertex();
            COL.Model.Rotation = Rotation.ToRotation();
            COL.CalculateBounds();
        }
    }
}