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
	public class RoadObjects : ObjectDefinition
	{
		protected NJS_OBJECT[] models;
		protected Mesh[][] meshes;
		protected NJS_TEXLIST[] texlists;
		protected Texture[][] textures;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[16];
			meshes = new Mesh[16][];
			texlists = new NJS_TEXLIST[16];
			textures = new Texture[16][];

			models[0] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TRASH.sa2bmdl");
			texlists[0] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TRASH.satex");

			models[1] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/PPLANT.sa2bmdl");
			texlists[1] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/PPLANT.satex");

			models[2] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/BENCH_A.sa2bmdl");
			texlists[2] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/BENCH_A.satex");

			models[3] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/BENCH_B.sa2bmdl");
			texlists[3] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/BENCH_B.satex");

			models[4] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/LAMPPOST.sa2bmdl");
			texlists[4] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/LAMPPOST.satex");

			models[5] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/NEWS_A.sa2bmdl");
			texlists[5] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/NEWS_A.satex");

			models[6] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/NEWS_B.sa2bmdl");
			texlists[6] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/NEWS_B.satex");

			models[7] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TREEWALL_RS.sa2bmdl");
			texlists[7] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TREEWALL_RS.satex");

			models[8] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TREEWALL_RC.sa2bmdl");
			texlists[8] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TREEWALL_RC.satex");

			models[9] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TREEWALL_LS.sa2bmdl");
			texlists[9] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TREEWALL_LS.satex");

			models[10] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_NIGHTSA.sa2bmdl");
			texlists[10] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_NIGHTSA.satex");

			models[11] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_GUNA.sa2bmdl");
			texlists[11] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_GUNA.satex");

			models[12] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_NIGHTSC.sa2bmdl");
			texlists[12] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_NIGHTSC.satex");

			models[13] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_GUNC.sa2bmdl");
			texlists[13] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_GUNC.satex");

			models[14] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_NIGHTSB.sa2bmdl");
			texlists[14] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_NIGHTSB.satex");

			models[15] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_GUNB.sa2bmdl");
			texlists[15] = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_GUNB.satex");

			texpacks.Add("objtex_stg13");
			texpacks.Add("landtx13");

			for (int i = 0; i < 16; i++)
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
			new PropertySpec("Object", typeof(RoadObjectTypes), "Extended", null, null, (o) => (RoadObjectTypes)Math.Min(Math.Max((int)o.Rotation.X, 0), 15), (o, v) => o.Rotation.X = (int)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum RoadObjectTypes
		{
			RecycleBin,
			Plant,
			BenchA,
			BenchB,
			Lamppost,
			NewspaperStandA,
			NewspaperStandB,
			RightWallTree,
			CurvedWallTree,
			LeftWallTree,
			NiGHTSPosterA,
			GUNPosterA,
			NiGHTSPosterB,
			GUNPosterB,
			NiGHTSPosterC,
			GUNPosterC
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int objID = Math.Min((int)item.Rotation.X, 15);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (objID == 8)
				transform.NJTranslate(new Vertex(1.5f, 0.8f, 1.5f));
			transform.NJRotateY(item.Rotation.Y);
			HitResult result = models[objID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[objID]);
			transform.Pop();
			return result;
			
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			for (int i = 0; i < 16; i++)
			{
				if (textures[i] == null)
					textures[i] = ObjectHelper.GetTextures(texpacks, texlists[i]);
			}
			int objID = Math.Min((int)item.Rotation.X, 15);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (objID == 8)
				transform.NJTranslate(new Vertex(1.5f, 0.8f, 1.5f));
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models[objID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures[objID], meshes[objID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[objID].DrawModelTreeInvert(transform, meshes[objID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int objID = Math.Min((int)item.Rotation.X, 15);
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
			int objID = Math.Min((int)item.Rotation.X, 15);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(models[objID], transform);
		}
		public override string Name { get { return "Road Curb Object"; } }
	}
}