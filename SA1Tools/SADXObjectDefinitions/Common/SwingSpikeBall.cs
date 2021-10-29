// TODO: finish implementing this
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class SwingSpikeBall : ObjectDefinition
	{
		private NJS_OBJECT centermodel;
		private Mesh[] centermeshes;
		private NJS_OBJECT cylindermodel;
		private Mesh[] cylindermeshes;
		private NJS_OBJECT ballmodel;
		private Mesh[] ballmeshes;

		public override void Init(ObjectData data, string name)
		{
			centermodel = ObjectHelper.LoadModel("object/ironball_typeb_iron_joint.nja.sa1mdl");
			centermeshes = ObjectHelper.GetMeshes(centermodel);
			cylindermodel = ObjectHelper.LoadModel("nondisp/cylinder01.nja.sa1mdl");
			cylindermeshes = ObjectHelper.GetMeshes(cylindermodel);
			ballmodel = ObjectHelper.LoadModel("object/sikake_ironball.nja.sa1mdl");
			ballmeshes = ObjectHelper.GetMeshes(ballmodel);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result = HitResult.Min(result, centermodel.CheckHit(Near, Far, Viewport, Projection, View, transform, centermeshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(centermodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), centermeshes));
			if (item.Selected)
				result.AddRange(centermodel.DrawModelTreeInvert(transform, centermeshes));
			transform.Pop();
			/*double v14 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
			transform.Push();
			double v8 = item.Scale.Y * 0.5;
			transform.NJTranslate(item.Position.X, (float)v8, item.Position.Z);
			double v9 = item.Scale.Y * 0.05000000074505806;
			transform.NJScale((float)v14, (float)v9, (float)v14);
			result.AddRange(cylindermodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, cylindermeshes));
			if (selected)
				result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
			transform.Pop();
			double v15 = (item.Scale.X + 6.0) * 0.4000000059604645 + 0.6000000238418579;
			transform.Push();
			double v13 = item.Scale.Y * 0.5;
			transform.NJTranslate(item.Position.X, (float)v13, item.Position.Z);
			transform.NJScale((float)v15, 0.1000000014901161f, (float)v15);
			result.AddRange(cylindermodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, cylindermeshes));
			if (selected)
				result.AddRange(cylindermodel.DrawModelTreeInvert(dev, transform, cylindermeshes));
			transform.Pop();*/
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(centermodel, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(centermodel, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
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

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
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