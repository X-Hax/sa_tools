using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.CityEscape
{
	public class Guidance : ObjectDefinition
	{
		private NJS_OBJECT models;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			meshes = ObjectHelper.GetMeshes(models);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Rotation.X * 2) * 2, (item.Rotation.Y * 2) * 2, (item.Rotation.Z * 2) * 2);
			HitResult result = models.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Rotation.X * 2) * 2, (item.Rotation.Y * 2) * 2, (item.Rotation.Z * 2) * 2);
			result.AddRange(models.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(models.DrawModelTreeInvert(transform, meshes, boundsByMesh: true));
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
			transform.NJScale((item.Rotation.X * 2) * 2, (item.Rotation.Y * 2) * 2, (item.Rotation.Z * 2) * 2);
			return ObjectHelper.GetModelBounds(models, transform, Math.Max(Math.Max((float)((item.Rotation.X * 2F * 2F)), (float)(item.Rotation.Y * 2F * 2F)), (float)(item.Rotation.Z * 2F * 2F)));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("X Scale", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Y Scale", typeof(int), "Extended", null, null, (o) => o.Rotation.Y, (o, v) => o.Rotation.Y = (int)v),
			new PropertySpec("Z Scale", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("X Axis Adjustment", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Z Axis Adjustment", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public enum BoardCollisionType
		{
			RemoveBoard,
			AddBoard
			
		}
		public override string Name { get { return "Directional Input Guidance"; } }
	}
}