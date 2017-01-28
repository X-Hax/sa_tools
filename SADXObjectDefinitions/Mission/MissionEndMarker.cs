using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel;

namespace SADXObjectDefinitions.Mission
{
	class MissionEndMarker : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Mission/Mission End Marker.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
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
			transform.NJTranslate(item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("Mission"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
			transform.Pop();
			return result;
		}

		public override string Name { get { return "Mission End Marker"; } }

		// incomplete, further investigation required
		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Visible", typeof(bool), null, null, true, (o) => ((MissionSETItem)o).PRMBytes[8] == 0, (o, v) => ((MissionSETItem)o).PRMBytes[8] = (byte)((bool)v ? 0 : 1)),
			new PropertySpec("Rings Required", typeof(byte), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[5], (o, v) => ((MissionSETItem)o).PRMBytes[5] = (byte)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}
