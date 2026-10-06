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

namespace SA2ObjectDefinitions.Common
{
	public class LinkBonus : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private Mesh[][] meshes;

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[3];
			meshes = new Mesh[3][];
			models[0] = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			models[1] = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			models[2] = ObjectHelper.LoadModel("OBJECT/CylinderCollision.sa2mdl");

			for (int i = 0; i < 3; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			var coltype = (item.Rotation.Z & 0xF) % 2;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (coltype == 0)
				transform.NJScale(item.Scale.X * 2, item.Scale.Y * 2, item.Scale.Z * 2);
			if (coltype == 1)
				transform.NJScale(item.Scale.X / 10F, item.Scale.X / 10F, item.Scale.X / 10F);
			HitResult result = models[coltype].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[coltype]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			var coltype = (item.Rotation.Z & 0xF) % 2;
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (coltype == 0)
				transform.NJScale(item.Scale.X * 2, item.Scale.Y * 2, item.Scale.Z * 2);
			if (coltype == 1)
				transform.NJScale(item.Scale.X / 10F, item.Scale.X / 10F, item.Scale.X / 10F);
			result.AddRange(models[coltype].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes[coltype], boundsByMesh: true));
			if (item.Selected)
				result.AddRange(models[coltype].DrawModelTreeInvert(transform, meshes[coltype], boundsByMesh: true));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			var coltype = (item.Rotation.Z & 0xF) % 2;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (coltype == 0)
				transform.NJScale(item.Scale.X * 2, item.Scale.Y * 2, item.Scale.Z * 2);
			if (coltype == 1)
				transform.NJScale(item.Scale.X / 10F, item.Scale.X / 10F, item.Scale.X / 10F);
			return ObjectHelper.GetModelBounds(models[coltype], transform, Math.Max(Math.Max((item.Scale.X + 10), (item.Scale.Y + 10)), (item.Scale.Z + 10)));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Collision Type", typeof(CollisionType), "Extended", null, null, (o) => (CollisionType)(Math.Min(Math.Max(o.Rotation.Z & 0xF, 0), 1)),
			(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
			new PropertySpec("Active Time", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Bonus Score", typeof(LinkBonusScore), "Extended", null, null, (o) => (LinkBonusScore)(o.Rotation.Y & 0xF),
			(o, v) => { o.Rotation.Y &= 0xFFF0; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Link Order", typeof(byte), "Extended", null, null, (o) => (o.Rotation.Y >> 4) & 0xF,
			(o, v) => { o.Rotation.Y &= 0xFF0F; o.Rotation.Y |= (byte)v << 4; }),
			new PropertySpec("Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public enum CollisionType : byte
		{
			Cube,
			Sphere,
		}
		public enum LinkBonusScore : byte
		{
			NoPoints,
			Good100,
			Nice200,
			Great300,
			Jammin400,
			Cool500,
			Radical600,
			Tight800,
			Awesome1000,
			Extreme1500
		}
		public override string Name { get { return "Point Bonus Link"; } }
	}
}