using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using Extensions = SonicRetro.SAModel.Direct3D.Extensions;
using Mesh = Microsoft.DirectX.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class JumpPanel : ObjectDefinition
	{
		private Object model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Common/Jump Panel.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
			transform.Pop();
			return result;
		}

		public override string Name { get { return "Jump Panel"; } }

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Panel Number", typeof(int), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (int)v),
            new PropertySpec("Next Panel", typeof(int), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}
