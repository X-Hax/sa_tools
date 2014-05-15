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
    public class SwingSpikeBall : ObjectDefinition
    {
        private SonicRetro.SAModel.Object centermodel;
        private Microsoft.DirectX.Direct3D.Mesh[] centermeshes;
        private SonicRetro.SAModel.Object cylindermodel;
        private Microsoft.DirectX.Direct3D.Mesh[] cylindermeshes;
        private SonicRetro.SAModel.Object ballmodel;
        private Microsoft.DirectX.Direct3D.Mesh[] ballmeshes;

        public override void Init(ObjectData data, string name, Device dev)
        {
            centermodel = ObjectHelper.LoadModel("Objects/SwingBall/Center Model.sa1mdl");
            centermeshes = ObjectHelper.GetMeshes(centermodel, dev);
            cylindermodel = ObjectHelper.LoadModel("Objects/Collision/Cylinder Model.sa1mdl");
            cylindermeshes = ObjectHelper.GetMeshes(cylindermodel, dev);
            ballmodel = ObjectHelper.LoadModel("Objects/FallBall/Model.sa1mdl");
            ballmeshes = ObjectHelper.GetMeshes(ballmodel, dev);
        }

        public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
        {
            HitResult result = HitResult.NoHit;
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            result = HitResult.Min(result, centermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, centermeshes));
            transform.Pop();
            /*double v14 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v8 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v8, item.Position.Z);
            double v9 = item.Scale.Y * 0.05000000074505806;
            transform.ScaleLocal((float)v14, (float)v9, (float)v14);
            result = HitResult.Min(result, cylindermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, cylindermeshes));
            transform.Pop();
            double v15 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v13 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v13, item.Position.Z);
            transform.ScaleLocal((float)v15, 0.1000000014901161f, (float)v15);
            result = HitResult.Min(result, cylindermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, cylindermeshes));
            transform.Pop();*/
            return result;
        }

        public override RenderInfo[] Render(SETItem item, Device dev, MatrixStack transform, bool selected)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            transform.Push();
            transform.TranslateLocal(item.Position.ToVector3());
            transform.RotateXYZLocal(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
            result.AddRange(centermodel.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), centermeshes));
            if (selected)
                result.AddRange(centermodel.DrawModelTreeInvert(dev, transform, centermeshes));
            transform.Pop();
            /*double v14 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v8 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v8, item.Position.Z);
            double v9 = item.Scale.Y * 0.05000000074505806;
            transform.ScaleLocal((float)v14, (float)v9, (float)v14);
            result.AddRange(cylindermodel.DrawModelTree(dev, transform, null, cylindermeshes));
            if (selected)
                result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
            transform.Pop();
            double v15 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
            transform.Push();
            double v13 = item.Scale.Y * 0.5;
            transform.TranslateLocal(item.Position.X, (float)v13, item.Position.Z);
            transform.ScaleLocal((float)v15, 0.1000000014901161f, (float)v15);
            result.AddRange(cylindermodel.DrawModelTree(dev, transform, null, cylindermeshes));
            if (selected)
                result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
            transform.Pop();*/
            return result.ToArray();
        }

        public override string Name { get { return "Swinging Spike Ball"; } }

		public static object GetOneBall(SETItem item)
		{
			return (long)item.Scale.Z % 10 >= 1;
		}

		public static object GetShadow(SETItem item)
		{
			long v5 = Math.Abs((long)item.Scale.Z % 100);
			if (v5 >= 10)
			{
				if (v5 < 90)
					return ShadowType.Off;
				else
					return ShadowType.Heavy;
			}
			else
				return ShadowType.Light;
		}

		public static object GetChain(SETItem item)
		{
			return Math.Abs((long)item.Scale.Z % 1000) >= 100;
		}

		public static object GetVerticalSpeed(SETItem item)
		{
			return (long)(item.Scale.Z / 1000);
		}

		public static void UpdateZScale(SETItem item, bool oneBall, ShadowType shadow, bool chain, long yspeed)
		{
			float value = oneBall ? 1 : 0;
			if (shadow == ShadowType.Off)
				value += 10;
			else if (shadow == ShadowType.Heavy)
				value += 90;
			if (!chain)
				value += 100;
			value += (float)yspeed * 1000;
			item.Scale.Z = value;
		}

		public static void SetOneBall(SETItem item, object value)
		{
			UpdateZScale(item, (bool)value, (ShadowType)GetShadow(item), (bool)GetChain(item), (long)GetVerticalSpeed(item));
		}

		public static void SetShadow(SETItem item, object value)
		{
			UpdateZScale(item, (bool)GetOneBall(item), (ShadowType)value, (bool)GetChain(item), (long)GetVerticalSpeed(item));
		}

		public static void SetChain(SETItem item, object value)
		{
			UpdateZScale(item, (bool)GetOneBall(item), (ShadowType)GetShadow(item), (bool)value, (long)GetVerticalSpeed(item));
		}

		public static void SetVerticalSpeed(SETItem item, object value)
		{
			UpdateZScale(item, (bool)GetOneBall(item), (ShadowType)GetShadow(item), (bool)GetChain(item), (long)value);
		}

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Chain Length", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X =  (float)v),
			new PropertySpec("Vertical Distance", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("One Ball", typeof(bool), "Extended", null, null, GetOneBall, SetOneBall),
			new PropertySpec("Shadow", typeof(ShadowType), "Extended", null, null, GetShadow, SetShadow),
			new PropertySpec("Chain", typeof(bool), "Extended", null, null, GetChain, SetChain),
			new PropertySpec("Vertical Speed", typeof(long), "Extended", null, null, GetVerticalSpeed, SetVerticalSpeed)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}

    public enum ShadowType
    {
        Off,
        Heavy,
        Light
    }
}