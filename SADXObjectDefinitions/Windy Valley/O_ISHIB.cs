using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using Extensions = SonicRetro.SAModel.Direct3D.Extensions;
using Mesh = Microsoft.DirectX.Direct3D.Mesh;

namespace SADXObjectDefinitions.Windy_Valley
{
	class O_ISHIB : ObjectDefinition
	{
		private Object model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Windy Valley/O_ISHIB.sa1mdl");
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

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform, bool selected)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshes));
			if (selected)
				result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			BoundingSphere bounds = new BoundingSphere(item.Position, Extensions.GetLargestRadius(meshes));

			return bounds;
		}

		public override string Name { get { return "O ISHIB"; } }
	}
}