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
	public class ModMod : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private Mesh[][] meshes;

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[2];
			meshes = new Mesh[2][];
			models[0] = ObjectHelper.LoadModel("OBJECT/ModMod_Cylinder.sa2mdl");
			models[1] = ObjectHelper.LoadModel("OBJECT/ModMod_Cube.sa2mdl");

			for (int i = 0; i < 2; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			var coltype = (item.Rotation.Z & 0xF);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			HitResult result = models[coltype].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[coltype]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			var coltype = (item.Rotation.Z & 0xF);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result.AddRange(models[coltype].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes[coltype]));
			if (item.Selected)
				result.AddRange(models[coltype].DrawModelTreeInvert(transform, meshes[coltype]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			var coltype = (item.Rotation.Z & 0xF);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			return ObjectHelper.GetModelBounds(models[coltype], transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Modifier Shape", typeof(ModifierType), "Extended", null, null, (o) => (ModifierType)(o.Rotation.Z & 0xF),
			(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public enum ModifierType : byte
		{
			Cylinder,
			Cube
		}
		public override string Name { get { return "Modifier Volume"; } }
	}
}