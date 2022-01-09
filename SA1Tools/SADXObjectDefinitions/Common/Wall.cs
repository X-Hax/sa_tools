using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using System;

namespace SADXObjectDefinitions.Common
{
	public class Wall : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("nondisp/cube01.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			transform.Push();
			transform.NJTranslate(0, 0, 10);
			transform.NJScale(0.1f, 0.1f, 2.0f);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(0, 0, 20);
			transform.NJRotateX(0x2000);
			transform.NJTranslate(0, 0, -3);
			transform.NJScale(0.1f, 0.1f, 0.7f);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(0, 0, 20);
			transform.NJRotateX(0xE000);
			transform.NJTranslate(0, 0, -3);
			transform.NJScale(0.1f, 0.1f, 0.7f);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.NJScale((item.Scale.X + 10) / 5f, (item.Scale.Y + 10) / 5f, 0.1f);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			transform.Push();
			transform.NJTranslate(0, 0, 10);
			transform.NJScale(0.1f, 0.1f, 2.0f);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes, boundsByMesh: true));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(0, 0, 20);
			transform.NJRotateX(0x2000);
			transform.NJTranslate(0, 0, -3);
			transform.NJScale(0.1f, 0.1f, 0.7f);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes, boundsByMesh: true));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(0, 0, 20);
			transform.NJRotateX(0xE000);
			transform.NJTranslate(0, 0, -3);
			transform.NJScale(0.1f, 0.1f, 0.7f);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes, boundsByMesh: true));
			transform.Pop();
			transform.NJScale((item.Scale.X + 10) / 5f, (item.Scale.Y + 10) / 5f, 0.1f);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes, boundsByMesh: true));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			return new BoundingSphere(new Vertex(item.Position.X, item.Position.Y, item.Position.Z), Math.Max(Math.Max(item.Scale.X, item.Scale.Y), item.Scale.Z) + 10.0f);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		public override string Name { get { return "Wall that pushes you"; } }
	}
}