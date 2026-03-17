using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class TikalHintKnuckles : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private Texture[] textures;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.GetFlatSquareModel();
			meshes = ObjectHelper.GetMeshes(model);
			textures = ObjectHelper.GetTextures("HINT");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y, item.Position.Z);
			transform.NJScale(0.4f, 0.4f, 0.4f);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y, item.Position.Z);
			transform.NJRotateXYZ(camera.Pitch, camera.Yaw, camera.Roll);
			transform.NJScale(0.4f, 0.4f, 0.4f);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures, meshes, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes, boundsByMesh: true));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			return new BoundingSphere(new Vertex(item.Position.X, item.Position.Y, item.Position.Z), 8.0f);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position.X, item.Position.Y, item.Position.Z);
			MatrixFunctions.Scale(ref matrix, 0.4f, 0.4f, 0.4f);

			return matrix;
		}

		public override string Name { get { return "Tikal Hint (This way!)"; } }
	}
}