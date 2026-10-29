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
	public class TreeShadows : ObjectDefinition
	{
		private NJS_OBJECT model;
		private NJS_OBJECT origin;
		private Mesh[] originmesh;
		private Mesh[] mesh;
		protected Vector3[] treepositions;

		public override void Init(ObjectData data, string name)
		{
			treepositions = new Vector3[6];
			treepositions[0] = new Vector3(70.0f, -165.0f, 240.0f);
			treepositions[1] = new Vector3(70.0f, 0.0f, 0.0f);
			treepositions[2] = new Vector3(70.0f, 165.0f, -240.0f);
			treepositions[3] = new Vector3(-70.0f, -165.0f, 240.0f);
			treepositions[4] = new Vector3(-70.0f, 0.0f, 0.0f);
			treepositions[5] = new Vector3(-70.0f, 165.0f, -240.0f);

			model = ObjectHelper.LoadModel("OBJECT/CylinderCollision.sa2mdl");
			mesh = ObjectHelper.GetMeshes(model);
			origin = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			originmesh = ObjectHelper.GetMeshes(origin);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			result = HitResult.Min(result, origin.CheckHit(Near, Far, Viewport, Projection, View, transform, originmesh));
			transform.Pop();
			for (int i = 0; i < 6; i++)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, item.Position.Y + 24.0F, item.Position.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJTranslate(treepositions[i]);
				transform.NJScale(24.0F * 0.1F, 32.0F * 0.1F, 24.0F * 0.1F);
				result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh));
				transform.Pop();
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			result.AddRange(origin.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, originmesh, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(origin.DrawModelTreeInvert(transform, originmesh, boundsByMesh: true));
			transform.Pop();
			for (int i = 0; i < 6; i++)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, item.Position.Y + 24.0F, item.Position.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJTranslate(treepositions[i]);
				transform.NJScale(24.0F * 0.1F, 32.0F * 0.1F, 24.0F * 0.1F);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, mesh, boundsByMesh: true));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, mesh, boundsByMesh: true));
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			BoundingSphere originbounds = ObjectHelper.GetModelBounds(origin, transform);
			transform.Pop();
			for (int i = 0; i < 6; i++)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, item.Position.Y + 24.0F, item.Position.Z);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJTranslate(treepositions[i]);
				transform.NJScale(24.0F * 0.1F, 32.0F * 0.1F, 24.0F * 0.1F);
				BoundingSphere shadowbounds = ObjectHelper.GetModelBounds(model, transform);
				transform.Pop();
				originbounds = SAModel.Direct3D.Extensions.Merge(shadowbounds, originbounds);
			}
			return originbounds;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Light ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Downhill Tree Shadows"; } }
	}
}