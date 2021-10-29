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
	public class AOSummon : ObjectDefinition
	{
		protected NJS_OBJECT whale;
		protected Mesh[] whalemsh;
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override void Init(ObjectData data, string name)
		{
			whale = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_oruka.nja.sa1mdl");
			whalemsh = ObjectHelper.GetMeshes(whale);
			sphere = ObjectHelper.LoadModel("nondisp/sphere01.nja.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere);
		}

		public override string Name { get { return "Whale Spawner"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			transform.Push();
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position + item.Scale);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			transform.Push();
			result = HitResult.Min(result, whale.CheckHit(Near, Far, Viewport, Projection, View, transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			result.AddRange(sphere.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, spheremsh, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh, boundsByMesh: true));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position + item.Scale);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			result.AddRange(whale.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), whalemsh));
			if (item.Selected)
				result.AddRange(whale.DrawModelTreeInvert(transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position + item.Scale);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			result.Add(new ModelTransform(whale, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			return ObjectHelper.GetModelBounds(sphere, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
	}

	public class AOKill : ObjectDefinition
	{
		protected NJS_OBJECT whale;
		protected Mesh[] whalemsh;
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override void Init(ObjectData data, string name)
		{
			whale = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_oruka.nja.sa1mdl");
			whalemsh = ObjectHelper.GetMeshes(whale);
			sphere = ObjectHelper.LoadModel("nondisp/sphere01.nja.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere);
		}

		public override string Name { get { return "Whale Despawner"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			transform.Push();
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZ(0x8000);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			transform.Push();
			result = HitResult.Min(result, whale.CheckHit(Near, Far, Viewport, Projection, View, transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			result.AddRange(sphere.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, spheremsh));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateZ(0x8000);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			result.AddRange(whale.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), whalemsh));
			if (item.Selected)
				result.AddRange(whale.DrawModelTreeInvert(transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			return ObjectHelper.GetModelBounds(sphere, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}
	}

	public class POSummon : ObjectDefinition
	{
		protected NJS_OBJECT whale;
		protected Mesh[] whalemsh;
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override void Init(ObjectData data, string name)
		{
			whale = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_oruka.nja.sa1mdl");
			whalemsh = ObjectHelper.GetMeshes(whale);
			sphere = ObjectHelper.LoadModel("nondisp/sphere01.nja.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere);
		}

		public override string Name { get { return "PO Whale Spawner"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			transform.Push();
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(0x2000);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			transform.Push();
			result = HitResult.Min(result, whale.CheckHit(Near, Far, Viewport, Projection, View, transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			result.AddRange(sphere.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, spheremsh));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(0x2000);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			result.AddRange(whale.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), whalemsh));
			if (item.Selected)
				result.AddRange(whale.DrawModelTreeInvert(transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(0x2000);
			transform.NJScale(0.40000001f, 0.40000001f, 0.40000001f);
			result.Add(new ModelTransform(whale, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJScale(4.5f, 4.5f, 4.5f);
			return ObjectHelper.GetModelBounds(sphere, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}
	}
}