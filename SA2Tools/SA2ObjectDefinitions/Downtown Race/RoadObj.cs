using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.DowntownRace
{
	public class RoadObj : ObjectDefinition
	{
		protected NJS_OBJECT object_benchA;
		protected Mesh[] mesh_benchA;
		protected NJS_TEXLIST texlist_benchA;

		protected NJS_OBJECT object_benchB;
		protected Mesh[] mesh_benchB;
		protected NJS_TEXLIST texlist_benchB;

		protected NJS_OBJECT object_lamppost;
		protected NJS_TEXLIST texlist_lamppost;

		protected NJS_OBJECT object_trash;
		protected NJS_TEXLIST texlist_trash;

		protected NJS_OBJECT object_flower;
		protected NJS_TEXLIST texlist_flower;

		protected NJS_OBJECT object_newsA;
		protected NJS_TEXLIST texlist_newsA;

		protected NJS_OBJECT object_newsB;
		protected NJS_TEXLIST texlist_newsB;

		protected NJS_OBJECT object_nights;
		protected NJS_TEXLIST texlist_nights;

		protected NJS_OBJECT object_nights2;
		protected NJS_TEXLIST texlist_nights2;

		protected NJS_OBJECT object_nights3;
		protected NJS_TEXLIST texlist_nights3;

		protected NJS_OBJECT object_treers;
		protected NJS_TEXLIST texlist_treers;

		protected NJS_OBJECT object_treels;
		protected NJS_TEXLIST texlist_treels;

		protected NJS_OBJECT object_treerc;
		protected NJS_TEXLIST texlist_treerc;

		protected NJS_OBJECT object_gun;
		protected NJS_TEXLIST texlist_gun;

		protected NJS_OBJECT object_gun2;
		protected NJS_TEXLIST texlist_gun2;

		protected NJS_OBJECT object_gun3;
		protected NJS_TEXLIST texlist_gun3;

		protected Texture[] texs;


		public override void Init(ObjectData data, string name)
		{
			object_benchA = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/BENCH_A.sa2bmdl");
			texlist_benchA = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/BENCH_A.satex");

			object_benchB = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/BENCH_B.sa2bmdl");
			texlist_benchB = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/BENCH_B.satex");

			object_lamppost = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/LAMPPOST.sa2bmdl");
			texlist_lamppost = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/LAMPPOST.satex");

			object_trash = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TRASH.sa2bmdl");
			texlist_trash = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TRASH.satex");

			object_flower = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/PPLANT.sa2bmdl");
			texlist_flower = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/PPLANT.satex");

			object_newsA = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/NEWS_A.sa2bmdl");
			texlist_newsA = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/NEWS_A.satex");

			object_newsB = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/NEWS_B.sa2bmdl");
			texlist_newsB = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/NEWS_B.satex");

			object_gun = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_GUNA.sa2bmdl");
			texlist_gun = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_GUNA.satex");

			object_gun2 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_GUNB.sa2bmdl");
			texlist_gun2 = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_GUNB.satex");

			object_gun3 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_GUNC.sa2bmdl");
			texlist_gun3 = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_GUNC.satex");

			object_nights = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_NIGHTSA.sa2bmdl");
			texlist_nights = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_NIGHTSA.satex");

			object_nights2 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_NIGHTSB.sa2bmdl");
			texlist_nights2 = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_NIGHTSB.satex");

			object_nights3 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/POSTER_NIGHTSC.sa2bmdl");
			texlist_nights3 = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/POSTER_NIGHTSC.satex");

			object_treers = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TREEWALL_RS.sa2bmdl");
			texlist_treers = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TREEWALL_RS.satex");

			object_treerc = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TREEWALL_RC.sa2bmdl");
			texlist_treerc = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TREEWALL_RC.satex");

			object_treels = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/TREEWALL_LS.sa2bmdl");
			texlist_treels = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/TREEWALL_LS.satex");
		}

		public override string Name { get { return "City Escape Road Object"; } }

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int objType = item.Rotation.X % 17;

			NJS_OBJECT pObject = object_benchA;
			NJS_TEXLIST pTexlist = texlist_benchA;

			switch (objType)
			{
				case 0:
					pObject = object_trash;
					pTexlist = texlist_trash;
					break;
				case 1:
					pObject = object_flower;
					pTexlist = texlist_flower;
					break;
				case 2:
					pObject = object_benchA;
					pTexlist = texlist_benchA;
					break;
				case 3:
					pObject = object_benchB;
					pTexlist = texlist_benchB;
					break;
				case 4:
					pObject = object_lamppost;
					pTexlist = texlist_lamppost;
					break;
				case 5:
					pObject = object_newsA;
					pTexlist = texlist_newsA;
					break;
				case 6:
					pObject = object_newsB;
					pTexlist = texlist_newsB;
					break;
				case 7:
					pObject = object_treers;
					pTexlist = texlist_treers;
					break;
				case 8:
					pObject = object_treerc;
					pTexlist = texlist_treerc;
					break;
				case 9:
				case 10:
					pObject = object_treels;
					pTexlist = texlist_treels;
					break;
				case 11:
					pObject = object_nights;
					pTexlist = texlist_nights;
					break;
				case 12:
					pObject = object_nights2;
					pTexlist = texlist_nights2;
					break;
				case 13:
					pObject = object_nights3;
					pTexlist = texlist_nights3;
					break;
				case 14:
					pObject = object_gun;
					pTexlist = texlist_gun;
					break;
				case 15:
					pObject = object_gun2;
					pTexlist = texlist_gun2;
					break;
				case 16:
					pObject = object_gun3;
					pTexlist = texlist_gun3;
					break;
				default:
					break;
			}

			Mesh[] pMesh = ObjectHelper.GetMeshes(pObject);

			List<RenderInfo> result = new List<RenderInfo>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result.AddRange(
					pObject.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode),
					transform,
					ObjectHelper.GetTextures(new List<string> { "landtx52", "objtex_stg13" }, pTexlist, dev),
					pMesh,
					EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));

				if (item.Selected)
				{
					result.AddRange(pObject.DrawModelTreeInvert(transform, pMesh));
				}
			}
			transform.Pop();

			return result;
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;

			int objType = item.Rotation.X % 17;

			NJS_OBJECT pObject = object_benchA;

			switch (objType)
			{
				case 0:
					pObject = object_trash;
					break;
				case 1:
					pObject = object_flower;
					break;
				case 2:
					pObject = object_benchA;
					break;
				case 3:
					pObject = object_benchB;
					break;
				case 4:
					pObject = object_lamppost;
					break;
				case 5:
					pObject = object_newsA;
					break;
				case 6:
					pObject = object_newsB;
					break;
				case 7:
					pObject = object_treers;
					break;
				case 8:
					pObject = object_treerc;
					break;
				case 9:
				case 10:
					pObject = object_treels;
					break;
				case 11:
					pObject = object_nights;
					break;
				case 12:
					pObject = object_nights2;
					break;
				case 13:
					pObject = object_nights3;
					break;
				case 14:
					pObject = object_gun;
					break;
				case 15:
					pObject = object_gun2;
					break;
				case 16:
					pObject = object_gun3;
					break;
				default:
					break;
			}

			Mesh[] pMesh = ObjectHelper.GetMeshes(pObject);

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result = pObject.CheckHit(Near, Far, Viewport, Projection, View, transform, pMesh);
			}
			transform.Pop();

			return result;
		}
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int objType = item.Rotation.X % 17;

			NJS_OBJECT pObject = object_benchA;

			switch (objType)
			{
				case 0:
					pObject = object_trash;
					break;
				case 1:
					pObject = object_flower;
					break;
				case 2:
					pObject = object_benchA;
					break;
				case 3:
					pObject = object_benchB;
					break;
				case 4:
					pObject = object_lamppost;
					break;
				case 5:
					pObject = object_newsA;
					break;
				case 6:
					pObject = object_newsB;
					break;
				case 7:
					pObject = object_treers;
					break;
				case 8:
					pObject = object_treerc;
					break;
				case 9:
				case 10:
					pObject = object_treels;
					break;
				case 11:
					pObject = object_nights;
					break;
				case 12:
					pObject = object_nights2;
					break;
				case 13:
					pObject = object_nights3;
					break;
				case 14:
					pObject = object_gun;
					break;
				case 15:
					pObject = object_gun2;
					break;
				case 16:
					pObject = object_gun3;
					break;
				default:
					break;
			}

			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(pObject, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			int objType = item.Rotation.X % 17;

			NJS_OBJECT pObject = object_benchA;

			switch (objType)
			{
				case 0:
					pObject = object_trash;
					break;
				case 1:
					pObject = object_flower;
					break;
				case 2:
					pObject = object_benchA;
					break;
				case 3:
					pObject = object_benchB;
					break;
				case 4:
					pObject = object_lamppost;
					break;
				case 5:
					pObject = object_newsA;
					break;
				case 6:
					pObject = object_newsB;
					break;
				case 7:
					pObject = object_treers;
					break;
				case 8:
					pObject = object_treerc;
					break;
				case 9:
				case 10:
					pObject = object_treels;
					break;
				case 11:
					pObject = object_nights;
					break;
				case 12:
					pObject = object_nights2;
					break;
				case 13:
					pObject = object_nights3;
					break;
				case 14:
					pObject = object_gun;
					break;
				case 15:
					pObject = object_gun2;
					break;
				case 16:
					pObject = object_gun3;
					break;
				default:
					break;
			}

			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(pObject, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + ObjectHelper.DegToBAMS(90);
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Prop", typeof(CERoadObjType), "Extended", null, null, (o) => (CERoadObjType)o.Rotation.X, (o, v) => o.Rotation.X = (int)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public enum CERoadObjType
		{
			TrashBin,
			Flower,
			EmeraldNotworkBench,
			SOAPBench,
			StreetLampPost,
			NewsBins1,
			NewsBins2,
			TreeWall1,
			TreeWall2,
			TreeWall3,
			NightsPosterLight,
			NightsPosterDark,
			NightsPosterMedium,
			GunPosterClusterLight,
			GunPosterClusterDark,
			GunPosterClusterMedium
		}
	}
}
