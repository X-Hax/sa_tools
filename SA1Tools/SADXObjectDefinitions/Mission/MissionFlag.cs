using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Mission
{
	class MissionFlag : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("mission/model/mi_flag.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			((BasicAttach)model.Attach).Material[0].TextureID = ((MissionSETItem)item).PRMBytes[8] % 7;
			((BasicAttach)model.Children[0].Attach).Material[0].TextureID = ((MissionSETItem)item).PRMBytes[8] % 7;
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("Mission"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			((BasicAttach)model.Attach).Material[0].TextureID = ((MissionSETItem)item).PRMBytes[8] % 7;
			((BasicAttach)model.Children[0].Attach).Material[0].TextureID = ((MissionSETItem)item).PRMBytes[8] % 7;
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale);
			return ObjectHelper.GetModelBounds(model, transform, Math.Max(Math.Max(item.Scale.X, item.Scale.Y), item.Scale.Z));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		public override string Name { get { return "Mission Flag"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Goal", typeof(GoalType), null, null, 0, (o) => (GoalType)((MissionSETItem)o).PRMBytes[4], (o, v) => ((MissionSETItem)o).PRMBytes[4] = (byte)(GoalType)v),
			new PropertySpec("Items Required/Order", typeof(byte), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[5], (o, v) => ((MissionSETItem)o).PRMBytes[5] = (byte)v),
			new PropertySpec("Last In Order", typeof(bool), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[6] != 0, (o, v) => ((MissionSETItem)o).PRMBytes[6] = (byte)((bool)v ? 1 : 0)),
			new PropertySpec("Texture", typeof(byte), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[8], (o, v) => ((MissionSETItem)o).PRMBytes[8] = (byte)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		enum GoalType
		{
			CollectOne,
			CollectMultiple,
			CollectInOrder
		}
	}
}
