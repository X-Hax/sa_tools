using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class OFrog : ObjectDefinition
	{
		protected NJS_OBJECT frog;
		protected Mesh[] frogmsh;
		protected NJS_OBJECT bubble;
		protected Mesh[] bubblemsh;
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override void Init(ObjectData data, string name)
		{
			frog = ObjectHelper.LoadModel("figure/big/models/big_kaeru.nja.sa1mdl");
			frogmsh = ObjectHelper.GetMeshes(frog);
			bubble = ObjectHelper.LoadModel("a_life/barria/al_barria.nja.sa1mdl");
			bubblemsh = ObjectHelper.GetMeshes(bubble);
			sphere = ObjectHelper.LoadModel("nondisp/sphere01.nja.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere);
		}

		public override string Name { get { return "Froggy (Bubble)"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			transform.Push();
			result = HitResult.Min(result, frog.CheckHit(Near, Far, Viewport, Projection, View, transform, frogmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.Push();
			result = HitResult.Min(result, bubble.CheckHit(Near, Far, Viewport, Projection, View, transform, bubblemsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			transform.Push();
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(frog.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("BIG_KAERU"), frogmsh));
			if (item.Selected)
				result.AddRange(frog.DrawModelTreeInvert(transform, frogmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(bubble.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, bubblemsh));
			if (item.Selected)
				result.AddRange(bubble.DrawModelTreeInvert(transform, bubblemsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			result.AddRange(sphere.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, spheremsh, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh, boundsByMesh: true));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(frog, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(bubble, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			return ObjectHelper.GetModelBounds(sphere, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position.X, item.Position.Y + item.Scale.Y, + item.Position.Z);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
	}

	public class Froggy : ObjectDefinition
	{
		protected NJS_OBJECT frog;
		protected Mesh[] frogmsh;

		public override void Init(ObjectData data, string name)
		{
			frog = ObjectHelper.LoadModel("figure/big/models/big_kaeru.nja.sa1mdl");
			frogmsh = ObjectHelper.GetMeshes(frog);
		}

		public override string Name { get { return "Froggy"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			transform.Push();
			result = HitResult.Min(result, frog.CheckHit(Near, Far, Viewport, Projection, View, transform, frogmsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			result.AddRange(frog.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("BIG_KAERU"), frogmsh));
			if (item.Selected)
				result.AddRange(frog.DrawModelTreeInvert(transform, frogmsh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			result.Add(new ModelTransform(frog, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			return ObjectHelper.GetModelBounds(frog, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}
	}
}