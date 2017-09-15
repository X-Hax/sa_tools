using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public abstract class YasiMain : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected Mesh[] meshes2;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result = HitResult.Min(result, model.Sibling.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes2));
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
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(model.Sibling.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes2));
			if (item.Selected)
				result.AddRange(model.Sibling.DrawModelTreeInvert(transform, meshes2));
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

	public class Yasi0 : YasiMain
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/YASI0.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
			meshes2 = ObjectHelper.GetMeshes(model.Sibling, dev);
		}

		public override string Name { get { return "Palm Tree 1"; } }
	}

	public class Yasi1 : YasiMain
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/YASI1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
			meshes2 = ObjectHelper.GetMeshes(model.Sibling, dev);
		}

		public override string Name { get { return "Palm Tree 2"; } }
	}

	public class Yasi2 : YasiMain
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/YASI2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
			meshes2 = ObjectHelper.GetMeshes(model.Sibling, dev);
		}

		public override string Name { get { return "Palm Tree 3"; } }
	}

	public class Yasi3 : YasiMain
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/YASI3.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model, dev);
			meshes2 = ObjectHelper.GetMeshes(model.Sibling, dev);
		}

		public override string Name { get { return "Palm Tree 4"; } }
	}
}