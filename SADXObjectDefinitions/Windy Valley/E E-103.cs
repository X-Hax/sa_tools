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
	public class E103 : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/E-103 Boss.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.Z, item.Rotation.Y, item.Rotation.X);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.Z, item.Rotation.Y, item.Rotation.X);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("WINDY_E103"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

        public override Matrix GetHandleMatrix(SETItem item)
        {
            Matrix matrix = Matrix.Identity;

            MatrixFunctions.Translate(ref matrix, item.Position);
            MatrixFunctions.RotateObject(ref matrix, item.Rotation.Z, item.Rotation.Y, item.Rotation.X);

            return matrix;
        }

        public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.Z, item.Rotation.Y, item.Rotation.X);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override string Name { get { return "Enemy E-103"; } }
	}
}