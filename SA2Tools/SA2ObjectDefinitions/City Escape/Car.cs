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
	public abstract class CarsBase : ObjectDefinition
	{
		protected NJS_OBJECT model1;
		protected NJS_OBJECT model2;
		protected NJS_OBJECT model3;
		protected NJS_OBJECT model4;
		protected NJS_OBJECT model5;
		protected NJS_OBJECT model6;
		protected NJS_OBJECT model7;
		protected NJS_OBJECT model8;
		protected NJS_OBJECT model9;
		protected NJS_OBJECT model10;
		protected NJS_OBJECT model11;
		protected NJS_OBJECT model12;
		protected NJS_OBJECT model13;
		protected NJS_TEXLIST texarr1;
		protected NJS_TEXLIST texarr2;
		protected NJS_TEXLIST texarr3;
		protected NJS_TEXLIST texarr4;
		protected NJS_TEXLIST texarr5;
		protected NJS_TEXLIST texarr6;
		protected NJS_TEXLIST texarr7;
		protected List<NJS_OBJECT> carmodels;
		protected NJS_OBJECT[] carmodelarray;
		protected List<Mesh[]> carmeshes;
		protected Mesh[][] carmesharray;

		public override void Init(ObjectData data, string name)
		{
			model1 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RND.sa2bmdl");
			texarr1 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/RND.satex");
			
			model2 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/TROLLEY.sa2bmdl");
			texarr2 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/TROLLEY.satex");
			
			model3 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/WHITE.sa2bmdl");
			texarr3 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/WHITE.satex");
			
			model4 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLACK.sa2bmdl");
			texarr4 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/BLACK.satex");
			
			model5 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLUE.sa2bmdl");
			texarr5 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/BLUE.satex");
			
			model6 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RED.sa2bmdl");
			texarr6 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/RED.satex");
			
			model7 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/CIVIC.sa2bmdl");
			texarr7 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/CIVIC.satex");
			
			model8 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RND_P.sa2bmdl");
			
			model9 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/WHITE_P.sa2bmdl");
			
			model10 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLACK_P.sa2bmdl");
			
			model11 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLUE_P.sa2bmdl");
			
			model12 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RED_P.sa2bmdl");
			
			model13 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/CIVIC_P.sa2bmdl");
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
			new PropertySpec("Car Type", typeof(CityEscapeCars), "Extended", null, null, (o) => (CityEscapeCars)o.Scale.Y, (o, v) => o.Scale.Y = (int)v),
			new PropertySpec("Car Strength", typeof(CityEscapeCarStrength), "Extended", null, null, (o) => (CityEscapeCarStrength)o.Scale.X, (o, v) => o.Scale.X = (int)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum CityEscapeCars
		{
			TaxiRND,
			TaxiRNDNOPUSH,
			Trolley,
			TrollyREV,
			Trolley2,
			Trolley2REVNOPUSH,
			Taxi,
			WhiteCar,
			BlackCar,
			BlueSUV,
			RedMinivan,
			BlueCivic,
			STaxi,
			SWhiteCar,
			SBlackCar,
			SBlueSUV,
			SRedMinivan,
			SBlueCivic,
			S2Taxi,
			S2WhiteCar,
			S2BlackCar,
			S2BlueSUV,
			S2RedMinivan,
			S2BlueCivic,
			S3Taxi,
			S3WhiteCar,
			S3BlackCar,
			S3BlueSUV,
			S3RedMinivan,
			S3BlueCivic
		}
		public enum CityEscapeCarStrength
		{
			Strong,
			TapDestruction
		}
	}
	public class ParkedCar : CarsBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int carID = Math.Max((int)item.Scale.Y, 0);

			NJS_OBJECT pObject;
			NJS_TEXLIST pTexlist;

			switch (carID)
			{
				case 0:
				case 1:
				default:
					pObject = model1;
					pTexlist = texarr1;
					break;
				case 2:
				case 3:
				case 4:
				case 5:
					pObject = model2;
					pTexlist = texarr2;
					break;
				case 7:
				case 13:
					pObject = model3;
					pTexlist = texarr3;
					break;
				case 8:
				case 14:
					pObject = model4;
					pTexlist = texarr4;
					break;
				case 9:
				case 15:
					pObject = model5;
					pTexlist = texarr5;
					break;
				case 10:
				case 16:
					pObject = model6;
					pTexlist = texarr6;
					break;
				case 11:
				case 17:
					pObject = model7;
					pTexlist = texarr7;
					break;
				case 18:
				case 24:
					pObject = model8;
					pTexlist = texarr1;
					break;
				case 19:
				case 25:
					pObject = model9;
					pTexlist = texarr3;
					break;
				case 20:
				case 26:
					pObject = model10;
					pTexlist = texarr4;
					break;
				case 21:
				case 27:
					pObject = model11;
					pTexlist = texarr5;
					break;
				case 22:
				case 28:
					pObject = model12;
					pTexlist = texarr6;
					break;
				case 23:
				case 29:
					pObject = model13;
					pTexlist = texarr7;
					break;
			}

			HitResult result;
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result = pObject.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(pObject));
			}
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int carID = Math.Max((int)item.Scale.Y, 0);

			NJS_OBJECT pObject;
			NJS_TEXLIST pTexlist;

			switch (carID)
			{
				case 0:
				case 1:
				default:
					pObject = model1;
					pTexlist = texarr1;
					break;
				case 2:
				case 3:
				case 4:
				case 5:
					pObject = model2;
					pTexlist = texarr2;
					break;
				case 7:
				case 13:
					pObject = model3;
					pTexlist = texarr3;
					break;
				case 8:
				case 14:
					pObject = model4;
					pTexlist = texarr4;
					break;
				case 9:
				case 15:
					pObject = model5;
					pTexlist = texarr5;
					break;
				case 10:
				case 16:
					pObject = model6;
					pTexlist = texarr6;
					break;
				case 11:
				case 17:
					pObject = model7;
					pTexlist = texarr7;
					break;
				case 18:
				case 24:
					pObject = model8;
					pTexlist = texarr1;
					break;
				case 19:
				case 25:
					pObject = model9;
					pTexlist = texarr3;
					break;
				case 20:
				case 26:
					pObject = model10;
					pTexlist = texarr4;
					break;
				case 21:
				case 27:
					pObject = model11;
					pTexlist = texarr5;
					break;
				case 22:
				case 28:
					pObject = model12;
					pTexlist = texarr6;
					break;
				case 23:
				case 29:
					pObject = model13;
					pTexlist = texarr7;
					break;
			}

			List<RenderInfo> result = new List<RenderInfo>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);

				result.AddRange(
					pObject.DrawModelTree(
						dev.GetRenderState<FillMode>(RenderState.FillMode),
						transform,
						ObjectHelper.GetTextures(new List<string> { "landtx13", "objtex_stg13" }, pTexlist, dev),
						ObjectHelper.GetMeshes(pObject),
						EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting)
					);

				if (item.Selected)
					result.AddRange(pObject.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(pObject)));
			}
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			result.Add(new ModelTransform(model1, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateZYX(item.Rotation);
			return ObjectHelper.GetModelBounds(model1, transform);
		}
		public override string Name { get { return "Parked Car"; } }
	}

	public class ParkedCarS : CarsBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int carID = Math.Max((int)item.Scale.Y, 0);

			NJS_OBJECT pObject;
			NJS_TEXLIST pTexlist;

			switch (carID)
			{
				case 0:
				case 1:
				default:
					pObject = model1;
					pTexlist = texarr1;
					break;
				case 2:
				case 3:
				case 4:
				case 5:
					pObject = model2;
					pTexlist = texarr2;
					break;
				case 7:
				case 13:
					pObject = model3;
					pTexlist = texarr3;
					break;
				case 8:
				case 14:
					pObject = model4;
					pTexlist = texarr4;
					break;
				case 9:
				case 15:
					pObject = model5;
					pTexlist = texarr5;
					break;
				case 10:
				case 16:
					pObject = model6;
					pTexlist = texarr6;
					break;
				case 11:
				case 17:
					pObject = model7;
					pTexlist = texarr7;
					break;
				case 18:
				case 24:
					pObject = model8;
					pTexlist = texarr1;
					break;
				case 19:
				case 25:
					pObject = model9;
					pTexlist = texarr3;
					break;
				case 20:
				case 26:
					pObject = model10;
					pTexlist = texarr4;
					break;
				case 21:
				case 27:
					pObject = model11;
					pTexlist = texarr5;
					break;
				case 22:
				case 28:
					pObject = model12;
					pTexlist = texarr6;
					break;
				case 23:
				case 29:
					pObject = model13;
					pTexlist = texarr7;
					break;
			}

			HitResult result;

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);

				result = pObject.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(pObject));
			}
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int carID = Math.Max((int)item.Scale.Y, 0);

			NJS_OBJECT pObject;
			NJS_TEXLIST pTexlist;

			switch (carID)
			{
				case 0:
				case 1:
				default:
					pObject = model1;
					pTexlist = texarr1;
					break;
				case 2:
				case 3:
				case 4:
				case 5:
					pObject = model2;
					pTexlist = texarr2;
					break;
				case 7:
				case 13:
					pObject = model3;
					pTexlist = texarr3;
					break;
				case 8:
				case 14:
					pObject = model4;
					pTexlist = texarr4;
					break;
				case 9:
				case 15:
					pObject = model5;
					pTexlist = texarr5;
					break;
				case 10:
				case 16:
					pObject = model6;
					pTexlist = texarr6;
					break;
				case 11:
				case 17:
					pObject = model7;
					pTexlist = texarr7;
					break;
				case 18:
				case 24:
					pObject = model8;
					pTexlist = texarr1;
					break;
				case 19:
				case 25:
					pObject = model9;
					pTexlist = texarr3;
					break;
				case 20:
				case 26:
					pObject = model10;
					pTexlist = texarr4;
					break;
				case 21:
				case 27:
					pObject = model11;
					pTexlist = texarr5;
					break;
				case 22:
				case 28:
					pObject = model12;
					pTexlist = texarr6;
					break;
				case 23:
				case 29:
					pObject = model13;
					pTexlist = texarr7;
					break;
			}

			List<RenderInfo> result = new List<RenderInfo>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);

				result.AddRange(
					pObject.DrawModelTree(
						dev.GetRenderState<FillMode>(RenderState.FillMode),
						transform,
						ObjectHelper.GetTextures(new List<string> { "landtx13", "objtex_stg13" }, pTexlist, dev),
						ObjectHelper.GetMeshes(pObject),
						EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting)
					);

				if (item.Selected)
					result.AddRange(pObject.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(pObject)));
			}
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZYX(item.Rotation);
			result.Add(new ModelTransform(model1, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateZYX(item.Rotation);
			return ObjectHelper.GetModelBounds(model1, transform);
		}
		public override string Name { get { return "Parked Car (Type S)"; } }
	}
}