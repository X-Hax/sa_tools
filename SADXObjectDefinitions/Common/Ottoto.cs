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
    class Ottoto : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Collision/Cube Model.sa1mdl");
            meshes = ObjectHelper.GetMeshes(model, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(0, item.Rotation.Y, 0);
            transform.ScaleLocal((item.Scale.X + 10) / 5f, (item.Scale.Y + 10) / 5f, (item.Scale.Z + 10) / 5f);
            HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
            transform.Pop();
            return result;
        }

        public override RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(0, item.Rotation.Y, 0);
            transform.ScaleLocal((item.Scale.X + 10) / 5f, (item.Scale.Y + 10) / 5f, (item.Scale.Z + 10) / 5f);
            result.AddRange(model.DrawModelTree(dev, transform, null, meshes));
            if (selected)
                result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
            transform.Pop();
            return result.ToArray();
        }

        public override string Name { get { return "Ottoto"; } }
    }
}
