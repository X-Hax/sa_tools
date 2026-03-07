using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;

namespace SA2ObjectDefinitions.Common
{
	public class GoalRing : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_OBJECT modelChao;
		private Mesh[] meshesChao;
		private NJS_OBJECT childGoal;
		private Mesh[] meshesChildGoal;
		private NJS_OBJECT childBack;
		private Mesh[] meshesChildBack;

		private NJS_TEXLIST texarr;
		private NJS_TEXLIST texarrChao;
		private NJS_TEXLIST texarrChildGoal;
		private NJS_TEXLIST texarrChildBack;
		private Texture[] texs;
		private Texture[] texsChao;
		private Texture[] texsChildGoal;
		private Texture[] texsChildBack;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_GOALRING.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/GOALRING.satex");

			modelChao = ObjectHelper.LoadModel("object/OBJECT_CHAO_LOST.sa2mdl");
			meshesChao = ObjectHelper.GetMeshes(modelChao);
			texarrChao = NJS_TEXLIST.Load("object/tls/CHAO_LOST.satex");

			childGoal = ObjectHelper.LoadModel("object/OBJECT_GOALRING_GOAL.sa2mdl");
			meshesChildGoal = ObjectHelper.GetMeshes(childGoal);
			texarrChildGoal = NJS_TEXLIST.Load("object/tls/GOALRING_GOAL.satex");

			childBack = ObjectHelper.LoadModel("object/OBJECT_GOALRING_BACK.sa2mdl");
			meshesChildBack = ObjectHelper.GetMeshes(childBack);
			texarrChildBack = NJS_TEXLIST.Load("object/tls/GOALRING_BACK.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			if (item.Rotation.X == 1)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				HitResult result = modelChao.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesChao);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
				HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			if (texsChao == null)
				texsChao = ObjectHelper.GetTextures("objtex_common", texarrChao, dev);
			if (texsChildGoal == null)
				texsChildGoal = ObjectHelper.GetTextures("objtex_common", texarrChildGoal, dev);
			if (texsChildBack == null)
				texsChildBack = ObjectHelper.GetTextures("objtex_common", texarrChildBack, dev);
			if (item.Rotation.X == 1)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.AddRange(modelChao.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsChao, meshesChao, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(modelChao.DrawModelTreeInvert(transform, meshesChao));
				}
				transform.Pop();
				return result;
			}
			else if (item.Rotation.X == 2)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				result.AddRange(childBack.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsChildBack, meshesChildBack, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
					result.AddRange(childBack.DrawModelTreeInvert(transform, meshesChildBack));
				}
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				result.AddRange(childGoal.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsChildGoal, meshesChildGoal, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
					result.AddRange(childGoal.DrawModelTreeInvert(transform, meshesChildGoal));
				}
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			if (item.Rotation.X == 1)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(modelChao, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(model, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			if (item.Rotation.X == 1)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				return ObjectHelper.GetModelBounds(modelChao, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				return ObjectHelper.GetModelBounds(model, transform);
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Goal Type", typeof(GoalType), "Extended", null, null, (o) => (GoalType)o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Goal Ring"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public enum GoalType
		{
			GoalRing,
			LostChao,
			BackRing
		}
	}
}