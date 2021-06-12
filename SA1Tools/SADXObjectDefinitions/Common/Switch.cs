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
	class Switch : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/switch_body.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(item.Rotation.X);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(item.Rotation.Z);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(item.Rotation.X);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(item.Rotation.Z);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(item.Rotation.X);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(item.Rotation.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(item.Rotation.X);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJRotateZ(item.Rotation.Z);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateX(ref matrix, item.Rotation.X);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);
			MatrixFunctions.RotateZ(ref matrix, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override string Name { get { return "Switch"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Type", typeof(SwitchType), "Extended", null, SwitchType.Toggle, (o) => (SwitchType)o.Scale.X, (o, v) => o.Scale.X = (float)(SwitchType)v),
			new PropertySpec("Switch ID", typeof(byte), "Extended", null, 0, o => (byte)o.Scale.Y, (o, v) => o.Scale.Y = (byte)v),
			new PropertySpec("Active Time", typeof(ushort), "Extended", null, 5, o => (ushort)o.Scale.Z, (o, v) => o.Scale.Z = (ushort)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		private readonly PropertySpec[] missionProperties = new PropertySpec[] {
			new PropertySpec("Switch ID", typeof(byte), null, "Overrides regular Switch ID setting for mission mode.", 0, (o) => ((MissionSETItem)o).PRMBytes[4], (o, v) => ((MissionSETItem)o).PRMBytes[4] = (byte)v)
		};

		public override PropertySpec[] MissionProperties { get { return missionProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		enum SwitchType
		{
			Toggle,
			Push,
			Timed,
			Permanent
		}
	}
}
