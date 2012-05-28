using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SADXPCTools;
using SonicRetro.SAModel.Direct3D;

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
            ImportModel();
            Paste();
        }

        public LevelItem(COL col, Device dev)
        {
            COL = col;
            Mesh = col.Model.Attach.CreateD3DMesh(dev);
            this.dev = dev;
        }

        public override Vertex Position { get { return COL.Model.Position; } set { COL.Model.Position = value; } }

        public override Rotation Rotation { get { return COL.Model.Rotation; } set { COL.Model.Rotation = value; } }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return COL.Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
        }

        public override RenderInfo[] Render(Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            result.AddRange(COL.Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible));
            if (selected)
                result.AddRange(COL.Model.DrawModelInvert(dev, transform, Mesh, (COL.SurfaceFlags & SurfaceFlags.Visible) == SurfaceFlags.Visible));
            return result.ToArray();
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
    }

    public class SETItem : Item
    {
        public SETItem()
        {
            Position = new Vertex();
            Scale = new Vertex();
        }

        public SETItem(byte[] file, int address)
        {
            ID = BitConverter.ToUInt16(file, address);
            ushort xrot = BitConverter.ToUInt16(file, address + 2);
            ushort yrot = BitConverter.ToUInt16(file, address + 4);
            ushort zrot = BitConverter.ToUInt16(file, address + 6);
            Rotation = new Rotation(xrot, yrot, zrot);
            Position = new Vertex(file, address + 8);
            Scale = new Vertex(file, address + 0x14);
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

        public override Vertex Position { get; set; }

        public override Rotation Rotation { get; set; }

        public Vertex Scale { get; set; }

        public override void Paste()
        {
            LevelData.SETItems[LevelData.Character].Add(this);
        }

        public override void Delete()
        {
            LevelData.SETItems[LevelData.Character].Remove(this);
        }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return LevelData.ObjDefs[ID].CheckHit(this, Near, Far, Viewport, Projection, View, new MatrixStack());
        }

        public override RenderInfo[] Render(Device dev, MatrixStack transform, bool selected)
        {
            return LevelData.ObjDefs[ID].Render(this, dev, transform, selected);
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
            Position = position;
            YRotation = yrot;
        }

        public override Vertex Position { get; set; }

        [Browsable(false)]
        public int YRotation { get; set; }

        [DisplayName("Y Rotation")]
        public float YRotDeg
        {
            get { return ObjectHelper.BAMSToDeg(YRotation); }
            set { YRotation = ObjectHelper.DegToBAMS(value); }
        }

        [Browsable(false)]
        public override Rotation Rotation { get { return new Rotation(0, YRotation, 0); } set { YRotation = value.Y; } }

        public override bool CanCopy { get { return false; } }

        public override void Paste()
        {
            throw new System.NotImplementedException();
        }

        public override void Delete()
        {
            throw new System.NotImplementedException();
        }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            MatrixStack transform = new MatrixStack();
            transform.Push();
            transform.TranslateLocal(0, offset, 0);
            transform.TranslateLocal(Position.ToVector3());
            transform.RotateXYZLocal(0, YRotation, 0);
            return Model.CheckHit(Near, Far, Viewport, Projection, View, transform, Meshes);
        }

        public override RenderInfo[] Render(Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.TranslateLocal(0, offset, 0);
            transform.TranslateLocal(Position.ToVector3());
            transform.RotateXYZLocal(0, YRotation, 0);
            result.AddRange(Model.DrawModelTree(dev, transform, LevelData.Textures[texture], Meshes));
            if (selected)
                result.AddRange(Model.DrawModelTreeInvert(dev, transform, Meshes));
            transform.Pop();
            return result.ToArray();
        }
    }

    public class DeathZoneItem : Item
    {
        [Browsable(false)]
        private Object Model { get; set; }
        [Browsable(false)]
        private Microsoft.DirectX.Direct3D.Mesh Mesh { get; set; }

        private Device dev;

        public DeathZoneItem(Device dev)
        {
            this.dev = dev;
            Model = new Object();
            ImportModel();
            Paste();
        }

        public DeathZoneItem(Object model, CharacterFlags flags, Device dev)
        {
            Model = model;
            Flags = flags;
            Mesh = Model.Attach.CreateD3DMesh(dev);
            this.dev = dev;
        }

        public override Vertex Position { get { return Model.Position; } set { Model.Position = value; } }

        public override Rotation Rotation { get { return Model.Rotation; } set { Model.Rotation = value; } }

        public override HitResult CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            return Model.CheckHit(Near, Far, Viewport, Projection, View, Mesh);
        }

        public override RenderInfo[] Render(Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            result.AddRange(Model.DrawModel(dev, transform, LevelData.Textures[LevelData.leveltexs], Mesh, false));
            if (selected)
                result.AddRange(Model.DrawModelInvert(dev, transform, Mesh, false));
            return result.ToArray();
        }

        public override void Paste()
        {
            if (LevelData.DeathZones != null)
                LevelData.DeathZones.Add(this);
        }

        public override void Delete()
        {
            LevelData.DeathZones.Remove(this);
        }

        [Browsable(true)]
        [DisplayName("Import Model")]
        public void ImportModel()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog() { DefaultExt = "obj", Filter = "OBJ Files|*.obj;*.objf", RestoreDirectory = true };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Model.Attach = SonicRetro.SAModel.Direct3D.Extensions.obj2nj(dlg.FileName);
                Mesh = Model.Attach.CreateD3DMesh(dev);
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
            using (MaterialEditor pw = new MaterialEditor(Model.Attach.Material.ToArray(), LevelData.TextureBitmaps[LevelData.leveltexs]))
                pw.ShowDialog();
        }

        public CharacterFlags Flags { get; set; }

        [Browsable(false)]
        public bool Visible
        {
            get
            {
                switch (LevelData.Character)
                {
                    case 0:
                        return (Flags & CharacterFlags.Sonic) == CharacterFlags.Sonic;
                    case 1:
                        return (Flags & CharacterFlags.Tails) == CharacterFlags.Tails;
                    case 2:
                        return (Flags & CharacterFlags.Knuckles) == CharacterFlags.Knuckles;
                    case 3:
                        return (Flags & CharacterFlags.Amy) == CharacterFlags.Amy;
                    case 4:
                        return (Flags & CharacterFlags.Gamma) == CharacterFlags.Gamma;
                    case 5:
                        return (Flags & CharacterFlags.Big) == CharacterFlags.Big;
                }
                return false;
            }
        }

        public bool Sonic
        {
            get
            {
                return (Flags & CharacterFlags.Sonic) == CharacterFlags.Sonic;
            }
            set
            {
                Flags = (Flags & ~CharacterFlags.Sonic) | (value ? CharacterFlags.Sonic : 0);
            }
        }

        public bool Tails
        {
            get
            {
                return (Flags & CharacterFlags.Tails) == CharacterFlags.Tails;
            }
            set
            {
                Flags = (Flags & ~CharacterFlags.Tails) | (value ? CharacterFlags.Tails : 0);
            }
        }

        public bool Knuckles
        {
            get
            {
                return (Flags & CharacterFlags.Knuckles) == CharacterFlags.Knuckles;
            }
            set
            {
                Flags = (Flags & ~CharacterFlags.Knuckles) | (value ? CharacterFlags.Knuckles : 0);
            }
        }

        public bool Amy
        {
            get
            {
                return (Flags & CharacterFlags.Amy) == CharacterFlags.Amy;
            }
            set
            {
                Flags = (Flags & ~CharacterFlags.Amy) | (value ? CharacterFlags.Amy : 0);
            }
        }

        public bool Gamma
        {
            get
            {
                return (Flags & CharacterFlags.Gamma) == CharacterFlags.Gamma;
            }
            set
            {
                Flags = (Flags & ~CharacterFlags.Gamma) | (value ? CharacterFlags.Gamma : 0);
            }
        }

        public bool Big
        {
            get
            {
                return (Flags & CharacterFlags.Big) == CharacterFlags.Big;
            }
            set
            {
                Flags = (Flags & ~CharacterFlags.Big) | (value ? CharacterFlags.Big : 0);
            }
        }

        public DeathZoneFlags Save(string path, int i)
        {
            ModelFile.CreateFile(System.IO.Path.Combine(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl"), Model, ModelFormat.SA1);
            return new DeathZoneFlags() { Flags = Flags };
        }
    }
}