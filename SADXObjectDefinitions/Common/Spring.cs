using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.Common
{
	public abstract class SpringBase : ObjectDefinition
	{
		protected SonicRetro.SAModel.Object model;
		protected Microsoft.DirectX.Direct3D.Mesh[] meshes;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateXYZ(item.Rotation);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override RenderInfo[] Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform, bool selected)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateXYZ(item.Rotation);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (selected)
				result.AddRange(model.DrawModelTreeInvert(dev, transform, meshes));
			transform.Pop();
			return result.ToArray();
		}

		private PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Disable Timer", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}

	public class Spring : SpringBase
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Spring/Model.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override SonicRetro.SAModel.BoundingSphere GetBounds(SETItem item)
		{
			SonicRetro.SAModel.BoundingSphere bounds = new SonicRetro.SAModel.BoundingSphere(item.Position, SonicRetro.SAModel.Direct3D.Extensions.GetLargestRadius(meshes));

			return bounds;
		}

		public override string Name { get { return "Ground Spring"; } }
	}

	public class SpringB : SpringBase
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Spring/Air Model.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override SonicRetro.SAModel.BoundingSphere GetBounds(SETItem item)
		{
			SonicRetro.SAModel.BoundingSphere bounds = new SonicRetro.SAModel.BoundingSphere(item.Position, SonicRetro.SAModel.Direct3D.Extensions.GetLargestRadius(meshes));

			return bounds;
		}

		public override string Name { get { return "Air Spring"; } }
	}
}