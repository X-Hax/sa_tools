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

namespace SADXObjectDefinitions.EmeraldCoast
{
	public class Plants : ObjectDefinition
	{
		protected NJS_OBJECT model1;
		protected Mesh[] meshes1;
		protected NJS_OBJECT model2;
		protected Mesh[] meshes2;
		protected NJS_OBJECT model3;
		protected Mesh[] meshes3;

		public override void Init(ObjectData data, string name)
		{
			model1 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kusa5.nja.sa1mdl");
			meshes1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kusa8.nja.sa1mdl");
			meshes2 = ObjectHelper.GetMeshes(model2);
			model3 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kusa1.nja.sa1mdl");
			meshes3 = ObjectHelper.GetMeshes(model3);
		}

		public override string Name { get { return "Plants"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{

			if (item.Scale.Z == 0.0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes1);
				transform.Pop();
				return result;
			}
			else if (item.Scale.Z <= 5.0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				HitResult result = model2.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes2);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				HitResult result = model3.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes3);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (item.Scale.Z == 0.0)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes1));
				if (item.Selected)
					result.AddRange(model1.DrawModelTreeInvert(transform, meshes1));
				transform.Pop();
				return result;
			}
			else if (item.Scale.Z <= 5.0)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes2));
				if (item.Selected)
					result.AddRange(model2.DrawModelTreeInvert(transform, meshes2));
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes3));
				if (item.Selected)
					result.AddRange(model3.DrawModelTreeInvert(transform, meshes3));
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			if (item.Scale.Z == 0.0)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.Add(new ModelTransform(model1, transform.Top));
				transform.Pop();
				return result;
			}
			else if (item.Scale.Z <= 5.0)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.Add(new ModelTransform(model2, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.Add(new ModelTransform(model3, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			if (item.Scale.Z == 0.0)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				return ObjectHelper.GetModelBounds(model1, transform);
			}
			else if (item.Scale.Z <= 5.0)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				return ObjectHelper.GetModelBounds(model2, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				return ObjectHelper.GetModelBounds(model3, transform);
			}
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Variant", typeof(Item), "Extended", null, null, (o) => (PlantVars)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}
	}

	public enum PlantVars
	{
		Plant_A,
		Plant_B,
		Plant_C
	}
}