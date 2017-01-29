using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;

namespace SADXObjectDefinitions.Common
{
	class Switch : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Common/Switch.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
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
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
			transform.Pop();
			return result;
		}

		public override string Name { get { return "Switch"; } }

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Type", typeof(SwitchType), "Extended", null, SwitchType.Toggle, (o) => (SwitchType)o.Scale.X, (o, v) => o.Scale.X = (float)(SwitchType)v),
			new PropertySpec("ID", typeof(byte), "Extended", null, 0, o => (byte)o.Scale.Y, (o, v) => o.Scale.Y = (byte)v),
			new PropertySpec("Active Time", typeof(ushort), "Extended", null, 5, o => (ushort)o.Scale.Z, (o, v) => o.Scale.Z = (ushort)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		private PropertySpec[] missionProperties = new PropertySpec[] {
			new PropertySpec("Switch ID", typeof(byte), null, "Overrides regular ID setting for mission mode.", 0, (o) => ((MissionSETItem)o).PRMBytes[4], (o, v) => ((MissionSETItem)o).PRMBytes[4] = (byte)v)
		};

		public override PropertySpec[] MissionProperties { get { return missionProperties; } }

		enum SwitchType
		{
			Toggle,
			Push,
			Timed,
			Permanent
		}
	}
}
