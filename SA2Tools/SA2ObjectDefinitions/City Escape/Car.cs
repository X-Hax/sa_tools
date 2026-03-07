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
		protected Mesh[] mesh1;
		protected NJS_OBJECT model2;
		protected Mesh[] mesh2;
		protected NJS_OBJECT model3;
		protected Mesh[] mesh3;
		protected NJS_OBJECT model4;
		protected Mesh[] mesh4;
		protected NJS_OBJECT model5;
		protected Mesh[] mesh5;
		protected NJS_OBJECT model6;
		protected Mesh[] mesh6;
		protected NJS_OBJECT model7;
		protected Mesh[] mesh7;
		protected NJS_OBJECT model8;
		protected Mesh[] mesh8;
		protected NJS_OBJECT model9;
		protected Mesh[] mesh9;
		protected NJS_OBJECT model10;
		protected Mesh[] mesh10;
		protected NJS_OBJECT model11;
		protected Mesh[] mesh11;
		protected NJS_OBJECT model12;
		protected Mesh[] mesh12;
		protected NJS_OBJECT model13;
		protected Mesh[] mesh13;
		protected NJS_TEXLIST texarr1;
		protected NJS_TEXLIST texarr2;
		protected NJS_TEXLIST texarr3;
		protected NJS_TEXLIST texarr4;
		protected NJS_TEXLIST texarr5;
		protected NJS_TEXLIST texarr6;
		protected NJS_TEXLIST texarr7;
		protected Texture[] texs1;
		protected Texture[] texs2;
		protected Texture[] texs3;
		protected Texture[] texs4;
		protected Texture[] texs5;
		protected Texture[] texs6;
		protected Texture[] texs7;
		protected List<NJS_OBJECT> carmodels;
		protected NJS_OBJECT[] carmodelarray;
		protected List<Mesh[]> carmeshes;
		protected Mesh[][] carmesharray;

		public override void Init(ObjectData data, string name)
		{
			model1 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RND.sa2bmdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			texarr1 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/RND.satex");
			
			model2 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/TROLLEY.sa2bmdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			texarr2 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/TROLLEY.satex");
			
			model3 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/WHITE.sa2bmdl");
			mesh3 = ObjectHelper.GetMeshes(model3);
			texarr3 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/WHITE.satex");
			
			model4 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLACK.sa2bmdl");
			mesh4 = ObjectHelper.GetMeshes(model4);
			texarr4 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/BLACK.satex");
			
			model5 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLUE.sa2bmdl");
			mesh5 = ObjectHelper.GetMeshes(model5);
			texarr5 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/BLUE.satex");
			
			model6 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RED.sa2bmdl");
			mesh6 = ObjectHelper.GetMeshes(model6);
			texarr6 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/RED.satex");
			
			model7 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/CIVIC.sa2bmdl");
			mesh7 = ObjectHelper.GetMeshes(model7);
			texarr7 = NJS_TEXLIST.Load("stg13_cityescape/tls/CARKAZ/CIVIC.satex");
			
			model8 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RND_P.sa2bmdl");
			mesh8 = ObjectHelper.GetMeshes(model8);
			
			model9 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/WHITE_P.sa2bmdl");
			mesh9 = ObjectHelper.GetMeshes(model9);
			
			model10 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLACK_P.sa2bmdl");
			mesh10 = ObjectHelper.GetMeshes(model10);
			
			model11 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/BLUE_P.sa2bmdl");
			mesh11 = ObjectHelper.GetMeshes(model11);
			
			model12 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/RED_P.sa2bmdl");
			mesh12 = ObjectHelper.GetMeshes(model12);
			
			model13 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/CARKAZ/CIVIC_P.sa2bmdl");
			mesh13 = ObjectHelper.GetMeshes(model13);
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
			if (carID == 2 || carID == 3 || carID == 4 || carID == 5)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model2.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh2);
				transform.Pop();
				return result;
			}
			else if (carID == 7 || carID == 13)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model3.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh3);
				transform.Pop();
				return result;
			}
			else if (carID == 8 || carID == 14)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model4.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh4);
				transform.Pop();
				return result;
			}
			else if (carID == 9 || carID == 14)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model5.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh5);
				transform.Pop();
				return result;
			}
			else if (carID == 10 || carID == 16)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model6.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh6);
				transform.Pop();
				return result;
			}
			else if (carID == 11 || carID == 17)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model7.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh7);
				transform.Pop();
				return result;
			}
			else if (carID == 18 || carID == 24)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model8.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh8);
				transform.Pop();
				return result;
			}
			else if (carID == 19 || carID == 25)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model9.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh9);
				transform.Pop();
				return result;
			}
			else if (carID == 20 || carID == 26)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model10.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh10);
				transform.Pop();
				return result;
			}
			else if (carID == 21 || carID == 27)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model11.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh11);
				transform.Pop();
				return result;
			}
			else if (carID == 22 || carID == 28)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model12.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh12);
				transform.Pop();
				return result;
			}
			else if (carID == 23 || carID == 29)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model13.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh13);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh1);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("objtex_stg13", texarr1, dev);
			if (texs2 == null)
				texs2 = ObjectHelper.GetTextures("objtex_stg13", texarr2, dev);
			if (texs3 == null)
				texs3 = ObjectHelper.GetTextures("objtex_stg13", texarr3, dev);
			if (texs4 == null)
				texs4 = ObjectHelper.GetTextures("objtex_stg13", texarr4, dev);
			if (texs5 == null)
				texs5 = ObjectHelper.GetTextures("objtex_stg13", texarr5, dev);
			if (texs6 == null)
				texs6 = ObjectHelper.GetTextures("objtex_stg13", texarr6, dev);
			if (texs7 == null)
				texs7 = ObjectHelper.GetTextures("objtex_stg13", texarr7, dev);
			int carID = Math.Max((int)item.Scale.Y, 0);
			if (carID == 2 || carID == 3 || carID == 4 || carID == 5)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model2.DrawModelTreeInvert(transform, mesh2));
				transform.Pop();
				return result;
			}
			else if (carID == 7 || carID == 13)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs3, mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model3.DrawModelTreeInvert(transform, mesh3));
				transform.Pop();
				return result;
			}
			else if (carID == 8 || carID == 14)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs4, mesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model4.DrawModelTreeInvert(transform, mesh4));
				transform.Pop();
				return result;
			}
			else if (carID == 9 || carID == 15)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs5, mesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model5.DrawModelTreeInvert(transform, mesh5));
				transform.Pop();
				return result;
			}
			else if (carID == 10 || carID == 16)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model6.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs6, mesh6, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model6.DrawModelTreeInvert(transform, mesh6));
				transform.Pop();
				return result;
			}
			else if (carID == 11 || carID == 17)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model7.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs7, mesh7, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model7.DrawModelTreeInvert(transform, mesh7));
				transform.Pop();
				return result;
			}
			if (carID == 18 || carID == 24)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model8.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh8, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model8.DrawModelTreeInvert(transform, mesh8));
				transform.Pop();
				return result;
			}
			else if (carID == 19 || carID == 25)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model9.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs3, mesh9, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model9.DrawModelTreeInvert(transform, mesh9));
				transform.Pop();
				return result;
			}
			else if (carID == 20 || carID == 26)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model10.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs4, mesh10, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model10.DrawModelTreeInvert(transform, mesh10));
				transform.Pop();
				return result;
			}
			else if (carID == 21 || carID == 27)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model11.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs5, mesh11, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model11.DrawModelTreeInvert(transform, mesh11));
				transform.Pop();
				return result;
			}
			else if (carID == 22 || carID == 28)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model12.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs6, mesh12, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model12.DrawModelTreeInvert(transform, mesh12));
				transform.Pop();
				return result;
			}
			else if (carID == 23 || carID == 29)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model13.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs7, mesh13, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model13.DrawModelTreeInvert(transform, mesh13));
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model1.DrawModelTreeInvert(transform, mesh1));
				transform.Pop();
				return result;
			}
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
			if (carID == 2 || carID == 3 || carID == 4 || carID == 5)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model2.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh2);
				transform.Pop();
				return result;
			}
			else if (carID == 7 || carID == 13)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model3.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh3);
				transform.Pop();
				return result;
			}
			else if (carID == 8 || carID == 14)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model4.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh4);
				transform.Pop();
				return result;
			}
			else if (carID == 9 || carID == 14)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model5.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh5);
				transform.Pop();
				return result;
			}
			else if (carID == 10 || carID == 16)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model6.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh6);
				transform.Pop();
				return result;
			}
			else if (carID == 11 || carID == 17)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model7.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh7);
				transform.Pop();
				return result;
			}
			else if (carID == 18 || carID == 24)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model8.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh8);
				transform.Pop();
				return result;
			}
			else if (carID == 19 || carID == 25)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model9.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh9);
				transform.Pop();
				return result;
			}
			else if (carID == 20 || carID == 26)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model10.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh10);
				transform.Pop();
				return result;
			}
			else if (carID == 21 || carID == 27)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model11.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh11);
				transform.Pop();
				return result;
			}
			else if (carID == 22 || carID == 28)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model12.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh12);
				transform.Pop();
				return result;
			}
			else if (carID == 23 || carID == 29)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model13.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh13);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh1);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("objtex_stg13", texarr1, dev);
			if (texs2 == null)
				texs2 = ObjectHelper.GetTextures("objtex_stg13", texarr2, dev);
			if (texs3 == null)
				texs3 = ObjectHelper.GetTextures("objtex_stg13", texarr3, dev);
			if (texs4 == null)
				texs4 = ObjectHelper.GetTextures("objtex_stg13", texarr4, dev);
			if (texs5 == null)
				texs5 = ObjectHelper.GetTextures("objtex_stg13", texarr5, dev);
			if (texs6 == null)
				texs6 = ObjectHelper.GetTextures("objtex_stg13", texarr6, dev);
			if (texs7 == null)
				texs7 = ObjectHelper.GetTextures("objtex_stg13", texarr7, dev);
			int carID = Math.Max((int)item.Scale.Y, 0);
			if (carID == 2 || carID == 3 || carID == 4 || carID == 5)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model2.DrawModelTreeInvert(transform, mesh2));
				transform.Pop();
				return result;
			}
			else if (carID == 7 || carID == 13)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs3, mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model3.DrawModelTreeInvert(transform, mesh3));
				transform.Pop();
				return result;
			}
			else if (carID == 8 || carID == 14)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs4, mesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model4.DrawModelTreeInvert(transform, mesh4));
				transform.Pop();
				return result;
			}
			else if (carID == 9 || carID == 15)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs5, mesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model5.DrawModelTreeInvert(transform, mesh5));
				transform.Pop();
				return result;
			}
			else if (carID == 10 || carID == 16)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model6.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs6, mesh6, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model6.DrawModelTreeInvert(transform, mesh6));
				transform.Pop();
				return result;
			}
			else if (carID == 11 || carID == 17)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model7.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs7, mesh7, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model7.DrawModelTreeInvert(transform, mesh7));
				transform.Pop();
				return result;
			}
			if (carID == 18 || carID == 24)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model8.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh8, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model8.DrawModelTreeInvert(transform, mesh8));
				transform.Pop();
				return result;
			}
			else if (carID == 19 || carID == 25)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model9.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs3, mesh9, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model9.DrawModelTreeInvert(transform, mesh9));
				transform.Pop();
				return result;
			}
			else if (carID == 20 || carID == 26)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model10.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs4, mesh10, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model10.DrawModelTreeInvert(transform, mesh10));
				transform.Pop();
				return result;
			}
			else if (carID == 21 || carID == 27)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model11.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs5, mesh11, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model11.DrawModelTreeInvert(transform, mesh11));
				transform.Pop();
				return result;
			}
			else if (carID == 22 || carID == 28)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model12.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs6, mesh12, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model12.DrawModelTreeInvert(transform, mesh12));
				transform.Pop();
				return result;
			}
			else if (carID == 23 || carID == 29)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model13.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs7, mesh13, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model13.DrawModelTreeInvert(transform, mesh13));
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(item.Rotation);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model1.DrawModelTreeInvert(transform, mesh1));
				transform.Pop();
				return result;
			}
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