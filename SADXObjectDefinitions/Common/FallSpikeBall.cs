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
    public class FallSpikeBall : ObjectDefinition
    {
        private SonicRetro.SAModel.Object ballmodel;
        private Mesh[] ballmeshes;
        private SonicRetro.SAModel.Object cylindermodel;
        private Mesh[] cylindermeshes;
        private SonicRetro.SAModel.Object spheremodel;
        private Mesh[] spheremeshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            ballmodel = ObjectHelper.LoadModel("Objects/FallBall/Model.sa1mdl");
            ballmeshes = ObjectHelper.GetMeshes(ballmodel, dev);
            cylindermodel = ObjectHelper.LoadModel("Objects/Collision/Cylinder Model.sa1mdl");
            cylindermeshes = ObjectHelper.GetMeshes(cylindermodel, dev);
            spheremodel = ObjectHelper.LoadModel("Objects/Collision/Sphere Model.sa1mdl");
            spheremeshes = ObjectHelper.GetMeshes(spheremodel, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            HitResult result = HitResult.NoHit;
            transform.Push();
            transform.NJTranslate(item.Position);
            transform.NJRotateY(item.Rotation.Y);
            result = HitResult.Min(result, ballmodel.CheckHit(Near, Far, Viewport, Projection, View, transform, ballmeshes));
            transform.Pop();
            double v24 = item.Scale.X * 0.05000000074505806;
            transform.Push();
            double v22 = item.Scale.X * 0.5 + item.Position.Y;
            transform.NJTranslate(item.Position.X, (float)v22, item.Position.Z);
            transform.NJScale(1.0f, (float)v24, 1.0f);
            result = HitResult.Min(result, cylindermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, cylindermeshes));
            transform.Pop();
            transform.Push();
            transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.Z, item.Position.Z);
            result = HitResult.Min(result, spheremodel.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremeshes));
            transform.Pop();
            return result;
        }

		public override RenderInfo[] Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.NJTranslate(item.Position);
            transform.NJRotateY(item.Rotation.Y);
            result.AddRange(ballmodel.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), ballmeshes));
            if (selected)
                result.AddRange(ballmodel.DrawModelTreeInvert(dev, transform, ballmeshes));
            transform.Pop();
            double v24 = item.Scale.X * 0.05000000074505806;
            transform.Push();
            double v22 = item.Scale.X * 0.5 + item.Position.Y;
            transform.NJTranslate(item.Position.X, (float)v22, item.Position.Z);
            transform.NJScale(1.0f, (float)v24, 1.0f);
            result.AddRange(cylindermodel.DrawModelTree(dev, transform, null, cylindermeshes));
            if (selected)
                result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
            transform.Pop();
            transform.Push();
            transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.Z, item.Position.Z);
            result.AddRange(spheremodel.DrawModelTree(dev, transform, null, spheremeshes));
            if (selected)
                result.AddRange(spheremodel.DrawModelTreeInvert(dev, transform, spheremeshes));
            transform.Pop();
            return result.ToArray();
        }

		public override SonicRetro.SAModel.BoundingSphere GetBounds(SETItem item)
		{
			SonicRetro.SAModel.BoundingSphere bounds = new SonicRetro.SAModel.BoundingSphere(item.Position, SonicRetro.SAModel.Direct3D.Extensions.GetLargestRadius(ballmeshes));

			return bounds;
		}

        public override string Name { get { return "Falling Spike Ball"; } }

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Distance", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}