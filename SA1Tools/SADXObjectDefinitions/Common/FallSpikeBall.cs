using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class FallSpikeBall : ObjectDefinition
	{
		private NJS_OBJECT ballmodel;
		private Mesh[] ballmeshes;
		private NJS_OBJECT cylindermodel;
		private Mesh[] cylindermeshes;
		private NJS_OBJECT spheremodel;
		private Mesh[] spheremeshes;

		public override void Init(ObjectData data, string name)
		{
			ballmodel = ObjectHelper.LoadModel("object/sikake_ironball.nja.sa1mdl");
			ballmeshes = ObjectHelper.GetMeshes(ballmodel);
			cylindermodel = ObjectHelper.LoadModel("nondisp/cylinder01.nja.sa1mdl");
			cylindermeshes = ObjectHelper.GetMeshes(cylindermodel);
			spheremodel = ObjectHelper.LoadModel("nondisp/sphere01.nja.sa1mdl");
			spheremeshes = ObjectHelper.GetMeshes(spheremodel);
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

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(ballmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), ballmeshes));
			if (item.Selected)
				result.AddRange(ballmodel.DrawModelTreeInvert(transform, ballmeshes));
			transform.Pop();
			double v24 = item.Scale.X * 0.05000000074505806;
			transform.Push();
			double v22 = item.Scale.X * 0.5 + item.Position.Y;
			transform.NJTranslate(item.Position.X, (float)v22, item.Position.Z);
			transform.NJScale(1.0f, (float)v24, 1.0f);
			result.AddRange(cylindermodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, cylindermeshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(cylindermodel.DrawModelTreeInvert(transform, cylindermeshes, boundsByMesh: true));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.Z, item.Position.Z);
			result.AddRange(spheremodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, spheremeshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(spheremodel.DrawModelTreeInvert(transform, spheremeshes, boundsByMesh: true));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(ballmodel, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			BoundingSphere bounds = new BoundingSphere(item.Position, item.Scale.X);	

			return bounds;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		public override string Name { get { return "Falling Spike Ball"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Distance", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}