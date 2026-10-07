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
	public class BoardCol : ObjectDefinition
	{
		private NJS_OBJECT models;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			meshes = ObjectHelper.GetMeshes(models);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 10) / 10F, (item.Scale.X + 10) / 10F, (item.Scale.X + 10) / 10F);
			HitResult result = models.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 10) / 10F, (item.Scale.X + 10) / 10F, (item.Scale.X + 10) / 10F);
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
			transform.NJScale((item.Scale.X + 10) / 10F, (item.Scale.X + 10) / 10F, (item.Scale.X + 10) / 10F);
			return ObjectHelper.GetModelBounds(models, transform, Math.Max(Math.Max((item.Scale.X + 10), (item.Scale.Y + 10)), (item.Scale.Z + 10)));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Trigger Type", typeof(BoardCollisionType), "Extended", null, null, (o) => (BoardCollisionType)(o.Rotation.X % 2), (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public enum BoardCollisionType
		{
			RemoveBoard,
			AddBoard
			
		}
		public override string Name { get { return "Board Trigger"; } }
	}
}