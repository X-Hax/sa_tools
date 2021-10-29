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
	public class Cylinder : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("nondisp/cylinder01.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);

			float X, Y;
			X = (item.Scale.X + 10) * 0.1f;
			Y = (item.Scale.Y + 10) * 0.1f;
			transform.NJScale(X, Y, X);

			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);

			float X, Y;
			X = (item.Scale.X + 10) * 0.1f;
			Y = (item.Scale.Y + 10) * 0.1f;
			transform.NJScale(X, Y, X);

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
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			float X, Y;
			X = (item.Scale.X + 10) * 0.1f;
			Y = (item.Scale.Y + 10) * 0.1f;
			transform.NJScale(X, Y, X);
			return ObjectHelper.GetModelBounds(model, transform, Math.Max(X, Y));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		public override string Name { get { return "Solid Cylinder"; } }
	}
}