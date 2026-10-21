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
	public class Fan : ObjectDefinition
	{
		private NJS_OBJECT[] fanmodels;
		private Mesh[][] fanmeshes;
		private NJS_OBJECT[] basemodels;
		private Mesh[][] basemeshes;
		private NJS_TEXLIST[] texarrsfan;
		private Texture[][] texsfan;
		private NJS_TEXLIST[] texarrsbase;
		private Texture[][] texsbase;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			fanmodels = new NJS_OBJECT[2];
			fanmeshes = new Mesh[2][];
			basemodels = new NJS_OBJECT[2];
			basemeshes = new Mesh[2][];
			texarrsbase = new NJS_TEXLIST[2];
			texsbase = new Texture[2][];
			texarrsfan = new NJS_TEXLIST[2];
			texsfan = new Texture[2][];

			fanmodels[0] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FAN_BRIGHT.sa2mdl");
			fanmodels[1] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FAN_DARK.sa2mdl");
			basemodels[0] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FANBASE_BRIGHT.sa2mdl");
			basemodels[1] = ObjectHelper.LoadModel("stg16_WildCanyon/models/FANBASE_DARK.sa2mdl");

			texarrsfan[0] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FAN_BRIGHT.satex");
			texarrsfan[1] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FAN_DARK.satex");
			texarrsbase[0] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FANBASE_BRIGHT.satex");
			texarrsbase[1] = NJS_TEXLIST.Load("stg16_WildCanyon/tls/FANBASE_DARK.satex");

			for (int i = 0; i < 2; i++)
			{
				fanmeshes[i] = ObjectHelper.GetMeshes(fanmodels[i]);
				basemeshes[i] = ObjectHelper.GetMeshes(basemodels[i]);
			}

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int shadeID = (int)(Math.Abs(item.Scale.Z) % 2);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result = HitResult.Min(result, fanmodels[shadeID].CheckHit(Near, Far, Viewport, Projection, View, transform, fanmeshes[shadeID]));
			result = HitResult.Min(result, basemodels[shadeID].CheckHit(Near, Far, Viewport, Projection, View, transform, basemeshes[shadeID]));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int shadeID = (int)(Math.Abs(item.Scale.X) % 2);
			for (int i = 0; i < 2; i++)
			{
				if (texsbase[i] == null)
					texsbase[i] = ObjectHelper.GetTextures(texpacks, texarrsbase[i], dev);
				if (texsfan[i] == null)
					texsfan[i] = ObjectHelper.GetTextures(texpacks, texarrsfan[i], dev);
			}
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result.AddRange(basemodels[shadeID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsbase[shadeID], basemeshes[shadeID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(fanmodels[shadeID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsfan[shadeID], fanmeshes[shadeID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(basemodels[shadeID].DrawModelTreeInvert(transform, basemeshes[shadeID]));
				result.AddRange(fanmodels[shadeID].DrawModelTreeInvert(transform, fanmeshes[shadeID]));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int shadeID = (int)(Math.Abs(item.Scale.Z) % 2);
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			result.Add(new ModelTransform(fanmodels[shadeID], transform.Top));
			result.Add(new ModelTransform(basemodels[shadeID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int shadeID = (int)(Math.Abs(item.Scale.Z) % 2);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJScale(item.Scale.Y + 1F, item.Scale.Y + 1F, item.Scale.Z + 1F);
			BoundingSphere fanbounds = ObjectHelper.GetModelBounds(fanmodels[shadeID], transform);
			return SAModel.Direct3D.Extensions.Merge(fanbounds, ObjectHelper.GetModelBounds(basemodels[shadeID], transform));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("XY Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Z Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Spin Speed", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Shading Type", typeof(ShadeType), "Extended", null, null, (o) => (ShadeType)(Math.Abs((int)o.Scale.X) % 2), (o, v) => o.Scale.X = (float)(ShadeType)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public enum ShadeType
		{
			Light,
			Dark,
		}
		public override string Name { get { return "Wall Fan"; } }
	}
}