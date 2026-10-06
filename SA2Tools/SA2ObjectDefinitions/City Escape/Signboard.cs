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
using SplitTools;
using SAModel.SAEditorCommon;

namespace SA2ObjectDefinitions.CityEscape
{
	public class Signboard : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];
		internal string[] itemnames =
		{
			"miu128_ce008",
			"miu128_ce009",
			"miu128_ce005",
			"miu128_ce010",
			"miu128_ce011"
		};
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg13_cityescape/models/SIGNBOARD.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNBOARD.satex");
			List<string> names = new List<string>();
			for (int i = 0; i < texarr.TextureNames.Length; i++)
			{
				names.Add(texarr.TextureNames[i]);
			}
			for (int n = 0; n < itemnames.Length; n++)
			{
				names.Add(itemnames[n]);
			}
			texarr.TextureNames = names.ToArray();

			texpacks.Add("objtex_stg13");
			texpacks.Add("landtx13");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			int posterID = Math.Min((int)item.Rotation.X, 5);
			switch (posterID)
			{
				case 0:
				default:
					texarr.TextureNames[2] = texarr.TextureNames[5];
					break;
				case 1:
					texarr.TextureNames[2] = texarr.TextureNames[6];
					break;
				case 2:
					texarr.TextureNames[2] = texarr.TextureNames[7];
					break;
				case 3:
					texarr.TextureNames[2] = texarr.TextureNames[8];
					break;
				case 4:
					texarr.TextureNames[2] = texarr.TextureNames[9];
					break;
			}
			texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0) 
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Poster Type", typeof(PosterAdvertisements), "Extended", null, null, (o) => (PosterAdvertisements)Math.Min((int)o.Rotation.X, 5), (o, v) => o.Rotation.X = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}
		public override string Name { get { return "Signboard Poster"; } }
	}

	public enum PosterAdvertisements
	{
		Burger,
		DigitalChoke,
		SonicTeam,
		ChaosCola,
		SOAPorSonicTeam,
	}
}