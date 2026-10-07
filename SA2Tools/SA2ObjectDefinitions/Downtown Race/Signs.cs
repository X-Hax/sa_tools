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
	public class Signs : ObjectDefinition
	{
		protected NJS_OBJECT object_groundstop;
		protected NJS_OBJECT object_downstop;
		protected NJS_OBJECT object_upstop;
		protected NJS_OBJECT object_noparkA;
		protected NJS_OBJECT object_noparkB;
		protected NJS_OBJECT object_stopsign;
		protected NJS_OBJECT object_noentrysign;
		protected NJS_OBJECT object_pedestriancrossing;

		protected NJS_TEXLIST texlist_groundstop;
		protected NJS_TEXLIST texlist_downstop;
		protected NJS_TEXLIST texlist_upstop;
		protected NJS_TEXLIST texlist_noparkA;
		protected NJS_TEXLIST texlist_noparkB;
		protected NJS_TEXLIST texlist_stopsign;
		protected NJS_TEXLIST texlist_noentrysign;
		protected NJS_TEXLIST texlist_pedestriancrossing;

		protected Texture[] texs;


		public override void Init(ObjectData data, string name)
		{
			object_groundstop = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP_STREET.sa2bmdl");
			object_downstop = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP_STREETCI.sa2bmdl");
			object_upstop = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP_STREETCO.sa2bmdl");

			object_noparkA = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/PSIGN_A.sa2bmdl");
			object_noparkB = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/PSIGN_B.sa2bmdl");

			object_stopsign = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/STOP.sa2bmdl");
			object_noentrysign = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/DSIGN.sa2bmdl");

			object_pedestriancrossing = ObjectHelper.LoadModel("stg13_cityescape/models/GC/SIGNS/PEDXING.sa2bmdl");

			texlist_groundstop = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP_STREET.satex");
			texlist_downstop = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP_STREETCI.satex");
			texlist_upstop = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP_STREETCO.satex");

			texlist_noparkA = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/PSIGN_A.satex");
			texlist_noparkB = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/PSIGN_B.satex");

			texlist_stopsign = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/STOP.satex");
			texlist_noentrysign = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/DSIGN.satex");

			texlist_pedestriancrossing = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNS/PEDXING.satex");
		}

		public override string Name { get { return "City Escape Sign"; } }

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int objType = item.Rotation.X % 8;

			NJS_OBJECT pObject = object_groundstop;
			NJS_TEXLIST pTexlist = texlist_groundstop;

			switch (objType)
			{
				case 0:
				default:
					pObject = object_groundstop;
					pTexlist = texlist_groundstop;
					break;
				case 1:
					pObject = object_downstop;
					pTexlist = texlist_downstop;
					break;
				case 2:
					pObject = object_upstop;
					pTexlist = texlist_upstop;
					break;
				case 3:
					pObject = object_noparkA;
					pTexlist = texlist_noparkA;
					break;
				case 4:
					pObject = object_noparkB;
					pTexlist = texlist_noparkB;
					break;
				case 5:
					pObject = object_stopsign;
					pTexlist = texlist_stopsign;
					break;
				case 6:
					pObject = object_noentrysign;
					pTexlist = texlist_noentrysign;
					break;
				case 7:
					pObject = object_pedestriancrossing;
					pTexlist = texlist_pedestriancrossing;
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

			int objType = item.Rotation.X % 7;

			NJS_OBJECT pObject = object_groundstop;
			NJS_TEXLIST pTexlist = texlist_groundstop;

			switch (objType)
			{
				case 0:
				default:
					pObject = object_groundstop;
					pTexlist = texlist_groundstop;
					break;
				case 1:
					pObject = object_downstop;
					pTexlist = texlist_downstop;
					break;
				case 2:
					pObject = object_upstop;
					pTexlist = texlist_upstop;
					break;
				case 3:
					pObject = object_noparkA;
					pTexlist = texlist_noparkA;
					break;
				case 4:
					pObject = object_noparkB;
					pTexlist = texlist_noparkB;
					break;
				case 5:
					pObject = object_stopsign;
					pTexlist = texlist_stopsign;
					break;
				case 6:
					pObject = object_noentrysign;
					pTexlist = texlist_noentrysign;
					break;
				case 7:
					pObject = object_pedestriancrossing;
					pTexlist = texlist_pedestriancrossing;
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
			int objType = item.Rotation.X % 7;

			NJS_OBJECT pObject = object_groundstop;
			NJS_TEXLIST pTexlist = texlist_groundstop;

			switch (objType)
			{
				case 0:
				default:
					pObject = object_groundstop;
					pTexlist = texlist_groundstop;
					break;
				case 1:
					pObject = object_downstop;
					pTexlist = texlist_downstop;
					break;
				case 2:
					pObject = object_upstop;
					pTexlist = texlist_upstop;
					break;
				case 3:
					pObject = object_noparkA;
					pTexlist = texlist_noparkA;
					break;
				case 4:
					pObject = object_noparkB;
					pTexlist = texlist_noparkB;
					break;
				case 5:
					pObject = object_stopsign;
					pTexlist = texlist_stopsign;
					break;
				case 6:
					pObject = object_noentrysign;
					pTexlist = texlist_noentrysign;
					break;
				case 7:
					pObject = object_pedestriancrossing;
					pTexlist = texlist_pedestriancrossing;
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
			int objType = item.Rotation.X % 7;

			NJS_OBJECT pObject = object_groundstop;
			NJS_TEXLIST pTexlist = texlist_groundstop;

			switch (objType)
			{
				case 0:
				default:
					pObject = object_groundstop;
					pTexlist = texlist_groundstop;
					break;
				case 1:
					pObject = object_downstop;
					pTexlist = texlist_downstop;
					break;
				case 2:
					pObject = object_upstop;
					pTexlist = texlist_upstop;
					break;
				case 3:
					pObject = object_noparkA;
					pTexlist = texlist_noparkA;
					break;
				case 4:
					pObject = object_noparkB;
					pTexlist = texlist_noparkB;
					break;
				case 5:
					pObject = object_stopsign;
					pTexlist = texlist_stopsign;
					break;
				case 6:
					pObject = object_noentrysign;
					pTexlist = texlist_noentrysign;
					break;
				case 7:
					pObject = object_pedestriancrossing;
					pTexlist = texlist_pedestriancrossing;
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
			new PropertySpec("Sign", typeof(CESignsType), "Extended", null, null, (o) => (CESignsType)o.Rotation.X, (o, v) => o.Rotation.X = (int)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public enum CESignsType
		{
			GroundStop,
			DownStop,
			UpStop,
			NoPark,
			TwoHrPark,
			StopSign,
			NoEntry,
			PedestrianCrossing,
		}
	}
}
