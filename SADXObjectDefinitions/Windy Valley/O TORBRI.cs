using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SonicRetro.SAModel.BoundingSphere;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.WindyValley
{
	public class OTorbri : ObjectDefinition
	{
		private NJS_OBJECT modelA;
		private Mesh[] meshesA;
		private NJS_OBJECT modelB;
		private Mesh[] meshesB;
		private NJS_OBJECT modelC;
		private Mesh[] meshesC;
		private NJS_OBJECT modelD;
		private Mesh[] meshesD;

		public override void Init(ObjectData data, string name)
		{
			modelA = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O TORBRI_A.sa1mdl");
			meshesA = ObjectHelper.GetMeshes(modelA);
			modelB = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O TORBRI_B.sa1mdl");
			meshesB = ObjectHelper.GetMeshes(modelB);
			modelC = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O TORBRI_C.sa1mdl");
			meshesC = ObjectHelper.GetMeshes(modelC);
			modelD = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O TORBRI_D.sa1mdl");
			meshesD = ObjectHelper.GetMeshes(modelD);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			int ScaleX = (int)item.Scale.X;
			switch (ScaleX)
			{
				case 1:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result = HitResult.Min(result, modelB.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesB));
					transform.Pop();
					break;
				case 2:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result = HitResult.Min(result, modelC.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesC));
					transform.Pop();
					break;
				case 3:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result = HitResult.Min(result, modelD.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesD));
					transform.Pop();
					break;
				default:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result = HitResult.Min(result, modelA.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesA));
					transform.Pop();
					break;
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			int ScaleX = (int)item.Scale.X;
			switch (ScaleX)
			{
				case 1:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result.AddRange(modelB.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesB));
					if (item.Selected)
						result.AddRange(modelB.DrawModelTreeInvert(transform, meshesB));
					transform.Pop();
					break;
				case 2:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result.AddRange(modelC.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesC));
					if (item.Selected)
						result.AddRange(modelC.DrawModelTreeInvert(transform, meshesC));
					transform.Pop();
					break;
				case 3:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result.AddRange(modelD.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesD));
					if (item.Selected)
						result.AddRange(modelD.DrawModelTreeInvert(transform, meshesD));
					transform.Pop();
					break;
				default:
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					result.AddRange(modelA.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesA));
					if (item.Selected)
						result.AddRange(modelA.DrawModelTreeInvert(transform, meshesA));
					transform.Pop();
					break;
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			BoundingSphere boxSphere = new BoundingSphere() { Center = new Vertex((item.Position.X), (item.Position.Y), item.Position.Z), Radius = 10f };

			return boxSphere;
		}

        public override Matrix GetHandleMatrix(SETItem item)
        {
            Matrix matrix = Matrix.Identity;

            MatrixFunctions.Translate(ref matrix, item.Position);
            MatrixFunctions.RotateObject(ref matrix, item.Rotation);

            return matrix;
        }

        public override string Name { get { return "Torbri"; } }
	}
}