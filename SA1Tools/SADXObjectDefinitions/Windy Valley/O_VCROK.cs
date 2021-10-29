using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.WindyValley
{
	public class VC_Rock : ObjectDefinition
	{
		private NJS_OBJECT modelA;
		private Mesh[] meshesA;
		private NJS_OBJECT modelB;
		private Mesh[] meshesB;

		public override void Init(ObjectData data, string name)
		{
			modelA = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_brock_moto.nja.sa1mdl");
			meshesA = ObjectHelper.GetMeshes(modelA);
			modelB = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_iwaita.nja.sa1mdl");
			meshesB = ObjectHelper.GetMeshes(modelB);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			if (item.Scale.X == 1)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				HitResult result = modelB.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesB);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				HitResult result = modelA.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesA);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (item.Scale.X == 1)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.AddRange(modelB.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesB));
				if (item.Selected)
					result.AddRange(modelB.DrawModelTreeInvert(transform, meshesB));
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.AddRange(modelA.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesA));
				if (item.Selected)
					result.AddRange(modelA.DrawModelTreeInvert(transform, meshesA));
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			if (item.Scale.X == 1)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.Add(new ModelTransform(modelB, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				result.Add(new ModelTransform(modelA, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			if (item.Scale.X == 1)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				return ObjectHelper.GetModelBounds(modelB, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation);
				return ObjectHelper.GetModelBounds(modelA, transform);
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		public override string Name { get { return "Animated Rock"; } }
	}
}