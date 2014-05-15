using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SADXObjectDefinitions.Common
{
    public class ItemBox : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Item Box/Normal Model.sa1mdl");
            meshes = ObjectHelper.GetMeshes(model, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
            transform.Pop();
            return result;
        }

        public override RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            ((SonicRetro.SAModel.BasicAttach)model.Children[2].Attach).Material[0].TextureID = itemTexs[Math.Min(Math.Max((int)item.Scale.X, 0), 8)];
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
            if (selected)
                result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
            transform.Pop();
            return result.ToArray();
        }

        internal int[] itemTexs = { 35, 72, 33, 32, 34, 71, 31, 73, 70 };

        internal int[] charTexs = { 31, 0, 4, 0, 0, 1, 3, 2 };

        public override string Name { get { return "Item Box"; } }

        public override Type ObjectType { get { return typeof(ItemBoxSETItem); } }
    }

    public class FloatingItemBox : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Item Box/Floating Model.sa1mdl");
            meshes = ObjectHelper.GetMeshes(model, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
            transform.Pop();
            return result;
        }

        public override RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            ((SonicRetro.SAModel.BasicAttach)model.Children[1].Attach).Material[0].TextureID = itemTexs[Math.Min(Math.Max((int)item.Scale.X, 0), 8)];
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
            if (selected)
                result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
            transform.Pop();
            return result.ToArray();
        }

        private int[] itemTexs = { 35, 72, 33, 32, 34, 71, 31, 73, 70 };

        private int[] charTexs = { 31, 0, 4, 0, 0, 1, 3, 2 };

        public override string Name { get { return "Floating Item Box"; } }

        public override Type ObjectType { get { return typeof(ItemBoxSETItem); } }
    }

    public class ItemBoxSETItem : SETItem
    {
        public ItemBoxSETItem() : base() { }
        public ItemBoxSETItem(byte[] file, int address) : base(file, address) { }

        public Items Item
        {
            get
            {
                return (Items)Math.Min(Math.Max((int)Scale.X, 0), 8);
            }
            set
            {
                Scale.X = (int)value;
            }
        }
    }

    public enum Items
    {
        SpeedUp,
        Invincibility,
        FiveRings,
        TenRings,
        RandomRings,
        Barrier,
        ExtraLife,
        Bomb,
        MagneticBarrier
    }
}