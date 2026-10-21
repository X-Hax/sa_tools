using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.WildCanyon
{
	public class Board2 : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private Mesh[][] meshes;
		private NJS_TEXLIST[] texarrs;
		private Texture[][] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[2];
			meshes = new Mesh[2][];
			texarrs = new NJS_TEXLIST[2];
			texs = new Texture[2][];

			models[0] = ObjectHelper.LoadModel("stg16_WildCanyon/models/BOARD2_BRIGHT.sa2mdl");
			models[1] = ObjectHelper.LoadModel("stg16_WildCanyon/models/BOARD2_DARK.sa2mdl");

			texarrs[0] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/BOARD2_BRIGHT.satex");
			texarrs[1] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/BOARD2_DARK.satex");

			for (int i = 0; i < 2; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int shadeID = (int)item.Scale.Z % 2;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.X + 1F, 1F);
			HitResult result = models[shadeID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[shadeID]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int shadeID = (int)item.Scale.Z % 2;
			for (int i = 0; i < 2; i++)
			{
				if (texs[i] == null)
					texs[i] = ObjectHelper.GetTextures(texpacks, texarrs[i], dev);
			}
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.X + 1F, 1F);
			result.AddRange(models[shadeID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[shadeID], meshes[shadeID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[shadeID].DrawModelTreeInvert(transform, meshes[shadeID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int shadeID = (int)item.Scale.Z % 2;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.X + 1F, 1F);
			result.Add(new ModelTransform(models[shadeID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int shadeID = (int)item.Scale.Z % 2;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X + 1F, item.Scale.X + 1F, 1F);
			BoundingSphere modelbounds = ObjectHelper.GetModelBounds(models[shadeID], transform);
			transform.Pop();
			return modelbounds;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Shading Type", typeof(ShadeType), "Extended", null, null, (o) => (ShadeType)(Math.Abs((int)o.Scale.Z) % 2), (o, v) => o.Scale.Z = (float)(ShadeType)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public enum ShadeType
		{
			Light,
			Dark,
		}
		public override string Name { get { return "Wall Board Glyph (Large)"; } }
	}
}