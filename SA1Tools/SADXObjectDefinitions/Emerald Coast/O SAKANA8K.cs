using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public class Fish2D : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_fish01.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override string Name { get { return "2D Fish"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{

			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			float ScaleX = item.Scale.X;
			float ScaleY = item.Scale.Y;
			if (ScaleX < ScaleY )
			{
				ScaleX = ScaleY;
			}
			transform.NJScale(ScaleX, ScaleY, item.Scale.Z);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			float ScaleX = item.Scale.X;
			float ScaleY = item.Scale.Y;
			if (ScaleX < ScaleY)
			{
				ScaleX = ScaleY;
			}
			transform.NJScale(ScaleX, ScaleY, item.Scale.Z);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			float ScaleX = item.Scale.X;
			float ScaleY = item.Scale.Y;
			if (ScaleX < ScaleY)
			{
				ScaleX = ScaleY;
			}
			transform.NJScale(ScaleX, ScaleY, item.Scale.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			float ScaleX = item.Scale.X;
			float ScaleY = item.Scale.Y;
			if (ScaleX < ScaleY)
			{
				ScaleX = ScaleY;
			}
			transform.NJScale(ScaleX, ScaleY, item.Scale.Z);
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
}