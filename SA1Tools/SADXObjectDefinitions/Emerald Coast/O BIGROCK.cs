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
	public class BigRock : ObjectDefinition
	{
		protected NJS_OBJECT model1;
		protected Mesh[] meshes1;
		protected NJS_OBJECT model2;
		protected Mesh[] meshes2;

		public override void Init(ObjectData data, string name)
		{
			model1 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kowareiwa_a.nja.sa1mdl");
			meshes1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_kowareiwa_c.nja.sa1mdl");
			meshes2 = ObjectHelper.GetMeshes(model2);
		}

		public override string Name { get { return "Big Rock"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			if (item.Scale.Y % 2 == 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes1);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				HitResult result = model2.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes2);
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
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes1));
				if (item.Selected)
					result.AddRange(model1.DrawModelTreeInvert(transform, meshes1));
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes2));
				if (item.Selected)
					result.AddRange(model2.DrawModelTreeInvert(transform, meshes2));
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
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				result.Add(new ModelTransform(model1, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				result.Add(new ModelTransform(model2, transform.Top));
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
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(model1, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(model2, transform);
			}
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Variant", typeof(Item), "Extended", null, null, (o) => (BigRockVars)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y, item.Rotation.Z);

			return matrix;
		}
	}

	public enum BigRockVars
	{
		BigRock_A,
		BigRock_B
	}
}