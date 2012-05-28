using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Common
{
    public class Spikes : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Spikes/Model.sa1mdl");
            meshes = ObjectHelper.GetMeshes(model, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            HitResult result = HitResult.NoHit;
            int rows = (int)Math.Max(item.Scale.X, 1);
            int cols = (int)Math.Max(item.Scale.Z, 1);
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
            double v4 = (1 - cols) * 7.5;
            double v5 = (1 - rows) * 7.5;
            transform.TranslateLocal((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
            for (int i = 0; i < rows; ++i)
            {
                transform.Push();
                for (int j = 0; j < cols; ++j)
                {
                    result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
                    transform.TranslateLocal(0, 0, 15);
                }
                transform.Pop();
                transform.TranslateLocal(15, 0, 0);
            }
            transform.Pop();
            return result;
        }

        public override RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            int rows = (int)Math.Max(item.Scale.X, 1);
            int cols = (int)Math.Max(item.Scale.Z, 1);
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
            double v4 = (1 - cols) * 7.5;
            double v5 = (1 - rows) * 7.5;
            transform.TranslateLocal((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
            for (int i = 0; i < rows; ++i)
            {
                transform.Push();
                for (int j = 0; j < cols; ++j)
                {
                    result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
                    if (selected)
                        result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
                    transform.TranslateLocal(0, 0, 15);
                }
                transform.Pop();
                transform.TranslateLocal(15, 0, 0);
            }
            transform.Pop();
            return result.ToArray();
        }

        public override string Name { get { return "Spikes"; } }

        public override Type ObjectType { get { return typeof(SpikesSETItem); } }
    }

    public class SpikesSETItem : SETItem
    {
        public SpikesSETItem() : base() { }
        public SpikesSETItem(byte[] file, int address) : base(file, address) { }

        public int Rows
        {
            get
            {
                return Math.Max((int)Scale.X, 1);
            }
            set
            {
                Scale.X = Math.Max(value, 1);
            }
        }

        public int Columns
        {
            get
            {
                return Math.Max((int)Scale.Z, 1);
            }
            set
            {
                Scale.Z = Math.Max(value, 1);
            }
        }
    }
}