using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SonicRetro.SAModel.BoundingSphere;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public abstract class OGrasa : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;

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
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(model, transform);
		}
	}

	public class GrasA : OGrasa
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O GRASA.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override string Name { get { return "Horizontal Palm Tree"; } }
	}

	public abstract class OGrasb : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{

			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.Z + 1.0f));
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
			transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.Z + 1.0f));
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.Z + 1.0f));
			return ObjectHelper.GetModelBounds(model, transform);
		}
	}

	public class GrasB : OGrasb
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O GRASB.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override string Name { get { return "Vines"; } }
	}

	public class GrasC : OGrasa
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O GRASC.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override string Name { get { return "Moss (Type A)"; } }
	}

	public class GrasD : OGrasa
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O GRASD.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
		}

		public override string Name { get { return "Vine Plant"; } }
	}
}