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
	public abstract class WarpBase : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private Mesh[][] meshes;
		private NJS_TEXLIST[] texarrs;
		private Texture[][] texs;
		protected List<string> texpacks = [];
		protected float[] portraitSizes;
		protected NJS_OBJECT endpoint;
		protected Mesh[] meshendpoint;

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[2];
			meshes = new Mesh[2][];
			texarrs = new NJS_TEXLIST[2];
			texs = new Texture[2][];
			portraitSizes = new float[3];

			models[0] = ObjectHelper.LoadModel("stg16_WildCanyon/models/WARP_BRIGHT.sa2mdl");
			models[1] = ObjectHelper.LoadModel("stg16_WildCanyon/models/WARP_DARK.sa2mdl");

			texarrs[0] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/WARP_BRIGHT.satex");
			texarrs[1] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/WARP_DARK.satex");

			for (int i = 0; i < 2; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");

			portraitSizes[0] = 60.0F;
			portraitSizes[1] = 50.0F;
			portraitSizes[2] = 40.0F;

			endpoint = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			meshendpoint = ObjectHelper.GetMeshes(endpoint);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int shadeID = ((item.Rotation.X >> 4) & 0xF) % 2;
			int scaleconst = (item.Rotation.X & 0xF) % 3;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(portraitSizes[scaleconst] / 60.0f, portraitSizes[scaleconst] / 60.0f, 1F);
			result = HitResult.Min(result, models[shadeID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[shadeID]));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(10F, 10f, 10f);
			result = HitResult.Min(result, endpoint.CheckHit(Near, Far, Viewport, Projection, View, transform, meshendpoint));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int shadeID = ((item.Rotation.X >> 4) & 0xF) % 2;
			int scaleconst = (item.Rotation.X & 0xF) % 3;
			for (int i = 0; i < 2; i++)
			{
				if (texs[i] == null)
					texs[i] = ObjectHelper.GetTextures(texpacks, texarrs[i], dev);
			}
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(portraitSizes[scaleconst] / 60.0f, portraitSizes[scaleconst] / 60.0f, 1F);
			result.AddRange(models[shadeID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[shadeID], meshes[shadeID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[shadeID].DrawModelTreeInvert(transform, meshes[shadeID]));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(10F, 10f, 10f);
			result.AddRange(endpoint.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshendpoint, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(endpoint.DrawModelTreeInvert(transform, meshendpoint));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int shadeID = ((item.Rotation.X >> 4) & 0xF) % 2;
			int scaleconst = (item.Rotation.X & 0xF) % 3;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(portraitSizes[scaleconst] / 60.0f, portraitSizes[scaleconst] / 60.0f, 1F);
			result.Add(new ModelTransform(models[shadeID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int shadeID = ((item.Rotation.X >> 4) & 0xF) % 2;
			int scaleconst = (item.Rotation.X & 0xF) % 3;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(portraitSizes[scaleconst] / 60.0f, portraitSizes[scaleconst] / 60.0f, 1F);
			BoundingSphere modelbounds = ObjectHelper.GetModelBounds(models[shadeID], transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(10F, 10F, 10F);
			BoundingSphere endbounds = ObjectHelper.GetModelBounds(endpoint, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(modelbounds, endbounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Scale", typeof(PortraitScale), "Extended", null, null, (o) => (PortraitScale)((o.Rotation.X & 0xF) % 3),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)(PortraitScale)v; }),
			new PropertySpec("Shading Type", typeof(ShadeType), "Extended", null, null, (o) => (ShadeType)(((o.Rotation.X >> 4) & 0xF) % 2),
				(o, v) => { o.Rotation.X &= 0xFF0F; o.Rotation.X |= (byte)(ShadeType)v << 4; }),
			new PropertySpec("Warp X Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Warp Y Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Warp Z Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Player Z Rotation", typeof(int), "Extended", "This determines the player's Z Rotation when the warp is finalized.", null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
	public class Warp : WarpBase
	{
		public override string Name { get { return "Warp Wall Painting"; } }
	}
	public class WarpN : WarpBase
	{
		public override string Name { get { return "Warp Wall Painting (N Type)"; } }
	}
	public enum ShadeType
	{
		Light,
		Dark,
	}
	public enum PortraitScale
	{
		Large,
		Medium,
		Small
	}
}