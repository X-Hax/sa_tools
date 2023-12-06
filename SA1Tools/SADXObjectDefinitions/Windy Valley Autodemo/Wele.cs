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

namespace SADXObjectDefinitions.WindyValleyAutodemo
{
	public class WeleObj : ObjectDefinition
	{
		private NJS_OBJECT Wele1;
		private Mesh[] Wele1Msh;
		private NJS_OBJECT WeleWhite;
		private Mesh[] WeleWhiteMsh;
		private NJS_OBJECT WeleAlt;
		private Mesh[] WeleAltMsh;
		private NJS_OBJECT destWele;
		private Mesh[] destWeleMsh;
		private float eleType;

		public override void Init(ObjectData data, string name)
		{
			Wele1 = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_windyelevator.sa1mdl");
			Wele1Msh = ObjectHelper.GetMeshes(Wele1);
			WeleWhite = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_windyelevator3.sa1mdl");
			WeleWhiteMsh = ObjectHelper.GetMeshes(WeleWhite);
			WeleAlt = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_windyelevator2.sa1mdl");
			WeleAltMsh = ObjectHelper.GetMeshes(WeleAlt);

			destWele = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_windyelevator.sa1mdl");
			destWeleMsh = ObjectHelper.GetMeshes(destWele);
		}

		public override string Name { get { return "Windy Elevator"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			eleType = Math.Min(Math.Max((int)item.Scale.Z, -1), 1);
			HitResult result = HitResult.NoHit;

			if (item.Selected)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.X), item.Position.Z);
				transform.NJRotateY(item.Rotation.Z);
				result = destWele.CheckHit(Near, Far, Viewport, Projection, View, transform, destWeleMsh);
				transform.Pop();
			}

			if (eleType < 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result = WeleAlt.CheckHit(Near, Far, Viewport, Projection, View, transform, WeleAltMsh);
				transform.Pop();
				return result;
			}
			else if (eleType > 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result = WeleWhite.CheckHit(Near, Far, Viewport, Projection, View, transform, WeleWhiteMsh);
				transform.Pop();
				return result;
			}
			else 
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result = Wele1.CheckHit(Near, Far, Viewport, Projection, View, transform, Wele1Msh);
				transform.Pop();
				
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			eleType = Math.Min(Math.Max((int)item.Scale.Z, -1), 1);

			if (item.Selected)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.X), item.Position.Z);
				transform.NJRotateY(item.Rotation.Z);
				result.AddRange(destWele.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, destWeleMsh));
				if (item.Selected)
					result.AddRange(destWele.DrawModelTreeInvert(transform, destWeleMsh));
				transform.Pop();
			}
			

			if (eleType < 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.AddRange(WeleAlt.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), WeleAltMsh));
				if (item.Selected)
					result.AddRange(WeleAlt.DrawModelTreeInvert(transform, WeleAltMsh));
				transform.Pop();
				return result;
			}
			else if (eleType > 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.AddRange(WeleWhite.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), WeleWhiteMsh));
				if (item.Selected)
					result.AddRange(WeleWhite.DrawModelTreeInvert(transform, WeleWhiteMsh));
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.AddRange(Wele1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), Wele1Msh));
				if (item.Selected)
					result.AddRange(Wele1.DrawModelTreeInvert(transform, Wele1Msh));
				transform.Pop();
				
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			eleType = Math.Min(Math.Max((int)item.Scale.Z, -1), 1);

			if (item.Selected)
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.X), item.Position.Z);
				transform.NJRotateY(item.Rotation.Z);
				result.Add(new ModelTransform(destWele, transform.Top));
				transform.Pop();
			}
			

			if (eleType < 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(WeleAlt, transform.Top));
				transform.Pop();
				return result;
			}
			else if (eleType > 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(WeleWhite, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(Wele1, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();

			float middle = (item.Position.Y + (item.Scale.X / 2));
			float radius = (((Math.Abs(item.Scale.X)) / 2) + 50.0f);

			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex((item.Position.X), (middle), item.Position.Z), Radius = radius };

			return boxSphere;

			//transform.NJTranslate(item.Position);
			//transform.NJRotateY(item.Rotation.Y);
			//return ObjectHelper.GetModelBounds(Wele1, transform);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Elevator Type", typeof(Type), "Extended", null, null, (o) => (Type)Math.Min(Math.Max((int)o.Scale.Z, -1), 1), (o, v) => o.Scale.Z = (int)v),
			new PropertySpec("Y Destination (Local)", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Rot Destination", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 50.0f; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
	}

	public enum Type
	{
		Alternate = -1,
		Normal = 0,
		White = 1
	}
}