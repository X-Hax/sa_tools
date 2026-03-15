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

		protected NJS_OBJECT object_gun;
		protected NJS_TEXLIST texlist_gun;

		protected NJS_OBJECT object_gun2;
		protected NJS_TEXLIST texlist_gun2;

		protected NJS_OBJECT object_gun3;
		protected NJS_TEXLIST texlist_gun3;

		protected NJS_OBJECT object_newsA;
		protected NJS_TEXLIST texlist_newsA;

		protected NJS_OBJECT object_newsB;
		protected NJS_TEXLIST texlist_newsB;

		protected Texture[] texs;


		public override void Init(ObjectData data, string name)
		{
			object_benchA = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/BENCH_A.sa2bmdl");
			mesh_benchA = ObjectHelper.GetMeshes(object_benchA);
			texlist_benchA = NJS_TEXLIST.Load("stg13_cityescape/tls/DOBJ/BENCH_A.satex");

			object_benchB = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DOBJ/BENCH_B.sa2bmdl");
			mesh_benchB = ObjectHelper.GetMeshes(object_benchB);
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

		}

		public override string Name { get { return "City Escape Road Object"; } }

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int objType = (int)item.Rotation.X;

			NJS_OBJECT pObject = object_benchA;
			NJS_TEXLIST pTexlist = texlist_benchA;

			switch (objType)
			{
				case 0x0:
					pObject = object_trash;
					pTexlist = texlist_trash;
					break;
				case 0x1:
					pObject = object_flower;
					pTexlist = texlist_flower;
					break;
				case 0x2:
					pObject = object_benchA;
					pTexlist = texlist_benchA;
					break;
				case 0x3:
					pObject = object_benchB;
					pTexlist = texlist_benchB;
					break;
				case 0x4:
					pObject = object_lamppost;
					pTexlist = texlist_lamppost;
					break;
				case 0x5:
					pObject = object_newsA;
					pTexlist = texlist_newsA;
					break;
				case 0x6:
					pObject = object_newsB;
					pTexlist = texlist_newsB;
					break;
				case 0x7:
					pObject = object_newsB;
					pTexlist = texlist_newsB;
					break;
				case 0x8:
					pObject = object_newsB;
					pTexlist = texlist_newsB;
					break;
				case 0x9:
					pObject = object_newsB;
					pTexlist = texlist_newsB;
					break;
				case 0xA:
					pObject = object_gun;
					pTexlist = texlist_gun;
					break;
				case 0xB:
					pObject = object_gun2;
					pTexlist = texlist_gun2;
					break;
				case 0xC:
					pObject = object_gun3;
					pTexlist = texlist_gun3;
					break;
				default:
					pObject = object_flower;
					pTexlist = texlist_flower;
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
					ObjectHelper.GetTexturesMultiSource(new List<string>(["landtx13", "objtex_stg13"]), pTexlist, dev),
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

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result = object_benchA.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh_benchA);
			}
			transform.Pop();

			return result;
		}
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(object_benchA, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(object_benchA, transform);
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
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}
