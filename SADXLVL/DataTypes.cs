using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel.SADXLVL2
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
            COL.Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh);
            if (selected)
                COL.Model.DrawModelInvert(dev, transform, Mesh);
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

    public class SETItem : Item
    {
        public SETItem()
        {
            Position = new EditableVertex();
            Scale = new EditableVertex();
        }

        public SETItem(byte[] file, int address)
        {
            ID = BitConverter.ToUInt16(file, address);
            ushort xrot = BitConverter.ToUInt16(file, address + 2);
            ushort yrot = BitConverter.ToUInt16(file, address + 4);
            ushort zrot = BitConverter.ToUInt16(file, address + 6);
            Rotation = new EditableRotation(xrot, yrot, zrot);
            Position = new EditableVertex(file, address + 8);
            Scale = new EditableVertex(file, address + 0x14);
            isLoaded = true;
        }

        [ParenthesizePropertyName(true)]
        public string Name { get { return LevelData.ObjDefs[id].Name; } }

        protected bool isLoaded = false;
        private ushort id;
        [Editor(typeof(IDEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ushort ID
        {
            get
            {
                return id;
            }
            set
            {
                id = (ushort)(value & 0xFFF);
                if (isLoaded) LevelData.ChangeObjectType(this);
            }
        }

        public override EditableVertex Position { get; set; }

        public override EditableRotation Rotation { get; set; }

        public EditableVertex Scale { get; set; }

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
            return LevelData.ObjDefs[ID].CheckHit(this, Near, Far, Viewport, Projection, View, new MatrixStack());
        }

        public override void Render(Device dev, MatrixStack transform, bool selected)
        {
            LevelData.ObjDefs[ID].Render(this, dev, transform, selected);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(0x20);
            bytes.AddRange(BitConverter.GetBytes(ID));
            unchecked
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.X));
                bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Y));
                bytes.AddRange(BitConverter.GetBytes((ushort)Rotation.Z));
            }
            bytes.AddRange(Position.GetBytes());
            bytes.AddRange(Scale.GetBytes());
            return bytes.ToArray();
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
            Position = new EditableVertex(position);
            YRotation = yrot;
        }

        public override EditableVertex Position { get; set; }

        [Browsable(false)]
        public int YRotation { get; set; }

        [DisplayName("Y Rotation")]
        public float YRotDeg
        {
            get { return ObjectHelper.BAMSToDeg(YRotation); }
            set { YRotation = ObjectHelper.DegToBAMS(value); }
        }

        [Browsable(false)]
        public override EditableRotation Rotation { get { return new EditableRotation(0, YRotation, 0); } set { YRotation = value.Y; } }

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
            transform.Push();
            transform.TranslateLocal(0, offset, 0);
            transform.TranslateLocal(Position.ToVector3());
            transform.RotateXYZLocal(0, YRotation, 0);
            return Model.CheckHit(Near, Far, Viewport, Projection, View, transform, Meshes);
        }

        public override void Render(Device dev, MatrixStack transform, bool selected)
        {
            transform.Push();
            transform.TranslateLocal(0, offset, 0);
            transform.TranslateLocal(Position.ToVector3());
            transform.RotateXYZLocal(0, YRotation, 0);
            Model.DrawModelTree(dev, transform, LevelData.Textures[texture], Meshes);
            if (selected)
                Model.DrawModelTreeInvert(dev, transform, Meshes);
            transform.Pop();
        }
    }
}