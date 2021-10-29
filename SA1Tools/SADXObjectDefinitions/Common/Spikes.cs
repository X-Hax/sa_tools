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

namespace SADXObjectDefinitions.Common
{
	public class Spikes : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/toge_togebody.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int rows = (int)Math.Max(item.Scale.X, 1);
			int cols = (int)Math.Max(item.Scale.Z, 1);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
			transform.NJTranslate((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
			for (int i = 0; i < rows; ++i)
			{
				transform.Push();
				for (int j = 0; j < cols; ++j)
				{
					result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
					transform.NJTranslate(0, 0, 15);
				}
				transform.Pop();
				transform.NJTranslate(15, 0, 0);
			}
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			int rows = (int)Math.Max(item.Scale.X, 1);
			int cols = (int)Math.Max(item.Scale.Z, 1);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
			transform.NJTranslate((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
			for (int i = 0; i < rows; ++i)
			{
				transform.Push();
				for (int j = 0; j < cols; ++j)
				{
					result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
					if (item.Selected)
						result.AddRange(model.DrawModelTreeInvert(transform, meshes));
					transform.NJTranslate(0, 0, 15);
				}
				transform.Pop();
				transform.NJTranslate(15, 0, 0);
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			int rows = (int)Math.Max(item.Scale.X, 1);
			int cols = (int)Math.Max(item.Scale.Z, 1);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
			transform.NJTranslate((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
			for (int i = 0; i < rows; ++i)
			{
				transform.Push();
				for (int j = 0; j < cols; ++j)
				{
					result.Add(new ModelTransform(model, transform.Top));
					transform.NJTranslate(0, 0, 15);
				}
				transform.Pop();
				transform.NJTranslate(15, 0, 0);
			}
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			int rows = (int)Math.Max(item.Scale.X, 1);
			int cols = (int)Math.Max(item.Scale.Z, 1);
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X & 0xC000, item.Rotation.Y, 0);
			transform.NJTranslate((1 - rows) * 7.5f, 0, (1 - cols) * 7.5f);
			for (int i = 0; i < rows; ++i)
			{
				transform.Push();
				for (int j = 0; j < cols; ++j)
				{
					result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
					transform.NJTranslate(0, 0, 15);
				}
				transform.Pop();
				transform.NJTranslate(15, 0, 0);
			}
			return result;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X & 0xC000, item.Rotation.Y, 0);

			return matrix;
		}

		public override string Name { get { return "Spikes"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Rows", typeof(int), "Extended", null, 1, (o) => Math.Max((int)o.Scale.X, 1), (o, v) => o.Scale.X = Math.Max((int)v, 1)),
			new PropertySpec("Columns", typeof(int), "Extended", null, 1, (o) => Math.Max((int)o.Scale.Z, 1), (o, v) => o.Scale.Z = Math.Max((int)v, 1))
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultZScale { get { return 0; } }
	}
}