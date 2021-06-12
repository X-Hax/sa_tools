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
	public class Pier : ObjectDefinition
	{
		protected NJS_OBJECT model1;
		protected Mesh[] meshes1;
		protected NJS_OBJECT model2;
		protected Mesh[] meshes2;

		public override void Init(ObjectData data, string name)
		{
			model2 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_pier_a01.nja.sa1mdl");
			meshes2 = ObjectHelper.GetMeshes(model2);
			model1 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_pier_b.nja.sa1mdl");
			meshes1 = ObjectHelper.GetMeshes(model1);
		}

		public override string Name { get { return "Pier"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			if (item.Scale.Y % 2 == 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				HitResult result = model2.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes2);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes1);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (item.Scale.Y % 2 == 0)
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
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes1));
				if (item.Selected)
					result.AddRange(model1.DrawModelTreeInvert(transform, meshes1));
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			if (item.Scale.Y % 2 == 0)
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
				result.Add(new ModelTransform(model1, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			if (item.Scale.Y % 2 == 0)
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
				return ObjectHelper.GetModelBounds(model1, transform);
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Variant", typeof(Item), "Extended", null, null, (o) => (PierVariants)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}

	public enum PierVariants
	{
		PierStraight,
		PierCorner
	}
}