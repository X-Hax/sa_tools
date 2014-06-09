using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.Common
{
    public class JumpPanel : ObjectDefinition
    {
        private SonicRetro.SAModel.Object model;
        private Microsoft.DirectX.Direct3D.Mesh[] meshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            model = ObjectHelper.LoadModel("Objects/Jump Panel/Jump Panel.sa1mdl");
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
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
            result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
            if (selected)
                result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
            transform.Pop();
            return result.ToArray();
        }

        public override string Name { get { return "Jump Panel"; } }

        private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Panel Number", typeof(int), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (int)v),
            new PropertySpec("Next Panel", typeof(int), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (int)v)
		};

        public override PropertySpec[] CustomProperties { get { return customProperties; } }
    }
}
