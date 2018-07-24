using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public abstract class OBZ : ObjectDefinition
	{
		protected NJS_OBJECT plane;
		protected Mesh[] planemsh;
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate((item.Position.X + 50f), item.Position.Y, item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			transform.NJScale(1f, 1f, 1f);
			transform.Push();
			result = HitResult.Min(result, plane.CheckHit(Near, Far, Viewport, Projection, View, transform, planemsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJScale(item.Scale.X, item.Scale.X, item.Scale.X);
			transform.Push();
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate((item.Position.X + 50f), item.Position.Y, item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(plane.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), planemsh));
			if (item.Selected)
				result.AddRange(plane.DrawModelTreeInvert(transform, planemsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJScale(item.Scale.X, item.Scale.X, item.Scale.X);
			result.AddRange(sphere.DrawModelTree(dev, transform, null, spheremsh));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform1 = new MatrixStack();
			transform1.NJTranslate(item.Position);
			transform1.NJScale(item.Scale.X, item.Scale.X, item.Scale.X);
			return ObjectHelper.GetModelBounds(sphere, transform1);
		}

        public override Matrix GetHandleMatrix(SETItem item)
        {
            Matrix matrix = Matrix.Identity;

            MatrixFunctions.Translate(ref matrix, item.Position);
            MatrixFunctions.RotateObject(ref matrix, item.Rotation);

            return matrix;
        }
    }

	public class Plane : OBZ
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			plane = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O BZ.sa1mdl");
			planemsh = ObjectHelper.GetMeshes(plane, dev);
			sphere = ObjectHelper.LoadModel("Objects/Collision/C SPHERE.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere, dev);
		}

		public override string Name { get { return "Tails' Plane"; } }
	}
}