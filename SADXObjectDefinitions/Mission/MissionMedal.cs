using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;

namespace SADXObjectDefinitions.Mission
{
	class MissionMedal : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Mission/Mission Medal.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(0x4000);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateX(0x4000);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("Mission"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
			transform.Pop();
			return result;
		}

		public override string Name { get { return "Mission Medal"; } }

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Goal", typeof(GoalType), null, null, 0, (o) => (GoalType)((MissionSETItem)o).PRMBytes[4], (o, v) => ((MissionSETItem)o).PRMBytes[4] = (byte)(GoalType)v),
			new PropertySpec("Items Required/Order", typeof(byte), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[5], (o, v) => ((MissionSETItem)o).PRMBytes[5] = (byte)v),
			new PropertySpec("Last In Order", typeof(bool), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[6] != 0, (o, v) => ((MissionSETItem)o).PRMBytes[6] = (byte)((bool)v ? 1 : 0))
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
