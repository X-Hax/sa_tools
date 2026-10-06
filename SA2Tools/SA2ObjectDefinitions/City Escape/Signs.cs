using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;
using System;

namespace SA2ObjectDefinitions.CityEscape
{
	public abstract class SignsBase : ObjectDefinition
	{
		protected NJS_OBJECT[] models;
		protected Mesh[][] meshes;
		protected NJS_TEXLIST[] texlists;
		protected Texture[][] textures;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[8];
			meshes = new Mesh[8][];
			texlists = new NJS_TEXLIST[8];
			textures = new Texture[8][];

			models[0] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP_STREET.sa2bmdl");
			texlists[0] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP_STREET.satex");

			models[1] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP_STREETCI.sa2bmdl");
			texlists[1] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP_STREETCI.satex");

			models[2] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP_STREETCO.sa2bmdl");
			texlists[2] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP_STREETCO.satex");

			models[3] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/PSIGN_A.sa2bmdl");
			texlists[3] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/PSIGN_A.satex");

			models[4] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/PSIGN_B.sa2bmdl");
			texlists[4] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/PSIGN_B.satex");

			models[5] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP.sa2bmdl");
			texlists[5] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP.satex");

			models[6] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/DSIGN.sa2bmdl");
			texlists[6] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/DSIGN.satex");

			models[7] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/PEDXING.sa2bmdl");
			texlists[7] = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/PEDXING.satex");

			texpacks.Add("objtex_stg13");
			texpacks.Add("landtx13");

			for (int i = 0; i < 8; i++)
			{
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Object", typeof(RoadSignTypes), "Extended", null, null, (o) => (RoadSignTypes)o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum RoadSignTypes
		{
			StopMark,
			StopMarkIn,
			StopMarkOut,
			ParkingA,
			ParkingB,
			StopSign,
			DoNotEnter,
			PedXing,
		}
	}
	public class NormalSigns : SignsBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int objID = Math.Min((int)item.Rotation.X, 7);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			HitResult result = models[objID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[objID]);
			transform.Pop();
			return result;

		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			for (int i = 0; i < 8; i++)
			{
				if (textures[i] == null)
					textures[i] = ObjectHelper.GetTextures(texpacks, texlists[i]);
			}
			int objID = Math.Min((int)item.Rotation.X, 7);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models[objID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures[objID], meshes[objID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[objID].DrawModelTreeInvert(transform, meshes[objID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int objID = Math.Min((int)item.Rotation.X, 7);
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(models[objID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int objID = Math.Min((int)item.Rotation.X, 7);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(models[objID], transform);
		}
		public override string Name { get { return "Road Sign Object"; } }
	}

	public class FarSigns : SignsBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int objID = Math.Min((int)item.Rotation.X, 7);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			HitResult result = models[objID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[objID]);
			transform.Pop();
			return result;

		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			for (int i = 0; i < 8; i++)
			{
				if (textures[i] == null)
					textures[i] = ObjectHelper.GetTextures(texpacks, texlists[i]);
			}
			int objID = Math.Min((int)item.Rotation.X, 7);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models[objID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures[objID], meshes[objID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[objID].DrawModelTreeInvert(transform, meshes[objID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int objID = Math.Min((int)item.Rotation.X, 7);
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(models[objID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int objID = Math.Min((int)item.Rotation.X, 7);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(models[objID], transform);
		}
		public override string Name { get { return "Road Sign Object (Far)"; } }
	}
}