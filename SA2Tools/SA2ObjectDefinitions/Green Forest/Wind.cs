using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.GreenForest
{
	public abstract class WindBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, (item.Scale.Y + 10F) * 2F, (item.Scale.Z + 10F) * 2F);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, (item.Scale.Y + 10F) * 2F, (item.Scale.Z + 10F) * 2F);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, (item.Scale.Y + 10F) * 2F, (item.Scale.Z + 10F) * 2F);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, (item.Scale.Y + 10F) * 2F, (item.Scale.Z + 10F) * 2F);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
	}
	public class Wind : WindBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, 40F, (item.Scale.Z + 10F) * 2F);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, 40F, (item.Scale.Z + 10F) * 2F);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, 40F, (item.Scale.Z + 10F) * 2F);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale((item.Scale.X + 10F) * 2F, 40F, (item.Scale.Z + 10F) * 2F);
			return ObjectHelper.GetModelBounds(model, transform);
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("X Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Z Scale", typeof(float), "Extended", null , null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Wind Generator (Set Height)"; } }
	}
	public class Wind2 : WindBase 
	{
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("X Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Z Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Wind Generator"; } }
	}
}