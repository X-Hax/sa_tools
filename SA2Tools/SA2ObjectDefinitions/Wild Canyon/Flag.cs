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
	public abstract class FlagBase : ObjectDefinition
	{
		protected NJS_OBJECT[] flagmodels;
		protected Mesh[][] flagmeshes;
		protected NJS_TEXLIST[] texarrs;
		protected Texture[][] texs;
		protected List<string> texpacks = [];

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int colorID = item.Rotation.Z % 2;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result = HitResult.Min(result, flagmodels[colorID].CheckHit(Near, Far, Viewport, Projection, View, transform, flagmeshes[colorID]));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int colorID = item.Rotation.Z % 2;
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
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result.AddRange(flagmodels[colorID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[colorID], flagmeshes[colorID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(flagmodels[colorID].DrawModelTreeInvert(transform, flagmeshes[colorID]));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int colorID = item.Rotation.Z % 2;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result.Add(new ModelTransform(flagmodels[colorID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int colorID = item.Rotation.Z % 2;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			return ObjectHelper.GetModelBounds(flagmodels[colorID], transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

	}
	public class UpperFlag : FlagBase
	{
		public override void Init(ObjectData data, string name)
		{
			flagmodels = new NJS_OBJECT[2];
			flagmeshes = new Mesh[2][];
			texarrs = new NJS_TEXLIST[2];
			texs = new Texture[2][];

			flagmodels[0] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FLAG2/00.sa2mdl");
			flagmodels[1] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FLAG/00.sa2mdl");

			texarrs[0] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FLAG2/00.satex");
			texarrs[1] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FLAG/00.satex");

			for (int i = 0; i < 2; i++)
			{
				flagmeshes[i] = ObjectHelper.GetMeshes(flagmodels[i]);
			}

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Flag Color", typeof(FlagTypeUpper), "Extended", null, null, (o) => (FlagTypeUpper)(o.Rotation.Z % 2), (o, v) => o.Rotation.Z = (int)(FlagTypeUpper)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Wind Altar Flag (Upper)"; } }
	}
	public class LowerFlag : FlagBase
	{
		public override void Init(ObjectData data, string name)
		{
			flagmodels = new NJS_OBJECT[2];
			flagmeshes = new Mesh[2][];
			texarrs = new NJS_TEXLIST[2];
			texs = new Texture[2][];

			flagmodels[0] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FLAGLOW2/00.sa2mdl");
			flagmodels[1] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FLAGLOW/00.sa2mdl");

			texarrs[0] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FLAGLOW2/00.satex");
			texarrs[1] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FLAGLOW/00.satex");

			for (int i = 0; i < 2; i++)
			{
				flagmeshes[i] = ObjectHelper.GetMeshes(flagmodels[i]);
			}

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Flag Color", typeof(FlagTypeLower), "Extended", null, null, (o) => (FlagTypeLower)(o.Rotation.Z % 2), (o, v) => o.Rotation.Z = (int)(FlagTypeLower)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Wind Altar Flag (Lower)"; } }
	}
	public enum FlagTypeUpper
	{
		Red,
		Purple,
	}
	public enum FlagTypeLower
	{
		Purple,
		Blue,
	}
}