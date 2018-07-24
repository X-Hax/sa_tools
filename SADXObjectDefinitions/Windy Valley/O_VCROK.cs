using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System;

namespace SADXObjectDefinitions.WindyValley
{
	public class VC_Rock : ObjectDefinition
	{
		private NJS_OBJECT modelA;
		private Mesh[] meshesA;
		private NJS_OBJECT modelB;
		private Mesh[] meshesB;

		public override void Init(ObjectData data, string name, Device dev)
		{
			modelA = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O_VCROK_A.sa1mdl");
			meshesA = ObjectHelper.GetMeshes(modelA, dev);
			modelB = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O_VCROK_B.sa1mdl");
			meshesB = ObjectHelper.GetMeshes(modelB, dev);
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
				result.AddRange(modelB.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesB));
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
				result.AddRange(modelA.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesA));
				if (item.Selected)
					result.AddRange(modelA.DrawModelTreeInvert(transform, meshesA));
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