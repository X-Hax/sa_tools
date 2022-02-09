using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SAModel.SAEditorCommon;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public class Whale : ObjectDefinition
	{
		protected NJS_OBJECT whale;
		protected NJS_OBJECT sphere;
		protected Mesh[] whalemsh;
		protected Mesh[] spheremsh;
		protected WhaleDefType type;

		public override string Name
		{
			get
			{
				switch (type)
				{
					case WhaleDefType.AOSummon:
					default:
						return "Whale Spawner";
					case WhaleDefType.AOKill:
						return "Whale Despawner";
					case WhaleDefType.POSummon:
						return "PO Whale Spawner";
				}
			}
		}

		public void InitModels()
		{
			whale = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_oruka.nja.sa1mdl");
			sphere = ObjectHelper.LoadModel("nondisp/sphere01.nja.sa1mdl");
			whalemsh = ObjectHelper.GetMeshes(whale);
			spheremsh = ObjectHelper.GetMeshes(sphere);
		}

		public override void Init(ObjectData data, string name) { }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.Push();
			transform.NJScale(4.5f, 4.5f, 4.5f);
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			switch (type)
			{
				case WhaleDefType.AOSummon:
				default:
					transform.NJTranslate(item.Scale);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case WhaleDefType.AOKill:
					transform.NJRotateZ(0x8000);
					break;
				case WhaleDefType.POSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateX(0x2000);
					transform.NJRotateY(item.Rotation.Y);
					break;
			}
			transform.NJScale(0.4f, 0.4f, 0.4f);
			result = HitResult.Min(result, whale.CheckHit(Near, Far, Viewport, Projection, View, transform, whalemsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.Push();
			transform.NJScale(4.5f, 4.5f, 4.5f);
			result.AddRange(sphere.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, spheremsh, boundsByMesh: true));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh, boundsByMesh: true));
			transform.Pop();
			switch (type)
			{
				case WhaleDefType.AOSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case WhaleDefType.AOKill:
					transform.NJRotateZ(0x8000);
					break;
				case WhaleDefType.POSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateX(0x2000);
					transform.NJRotateY(item.Rotation.Y);
					break;
			}
			transform.NJScale(0.4f, 0.4f, 0.4f);
			result.AddRange(whale.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), whalemsh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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
			switch (type)
			{
				case WhaleDefType.AOSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case WhaleDefType.AOKill:
					transform.NJRotateZ(0x8000);
					break;
				case WhaleDefType.POSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateX(0x2000);
					transform.NJRotateY(item.Rotation.Y);
					break;
			}
			transform.NJScale(0.4f, 0.4f, 0.4f);
			result.Add(new ModelTransform(whale, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.Push();
			transform.NJScale(4.5f, 4.5f, 4.5f);
			BoundingSphere bounds_sphere = ObjectHelper.GetModelBounds(sphere, transform);
			transform.Pop();
			switch (type)
			{
				case WhaleDefType.AOSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case WhaleDefType.AOKill:
					transform.NJRotateZ(0x8000);
					break;
				case WhaleDefType.POSummon:
					transform.NJTranslate(item.Scale);
					transform.NJRotateX(0x2000);
					transform.NJRotateY(item.Rotation.Y);
					break;
			}
			transform.NJScale(0.4f, 0.4f, 0.4f);
			BoundingSphere bounds_whale = ObjectHelper.GetModelBounds(whale, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(bounds_whale, bounds_sphere);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;
			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);
			return matrix;
		}

		public enum WhaleDefType
		{
			AOSummon,
			AOKill,
			POSummon,
		}
	}

	public class AOSummon : Whale 
	{
		public override void Init(ObjectData data, string name) 
		{
			type = WhaleDefType.AOSummon;
			InitModels();
		}
	}

	public class AOKill : Whale
	{
		public override void Init(ObjectData data, string name)
		{
			type = WhaleDefType.AOKill;
			InitModels();
		}
	}

	public class POSummon : Whale
	{
		public override void Init(ObjectData data, string name)
		{
			type = WhaleDefType.POSummon;
			InitModels();
		}
	}
}