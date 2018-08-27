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
	public abstract class O_DOLPHIN : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] mesh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.Push();
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(model.DrawModelTree(dev, transform, ObjectHelper.GetTextures("OBJ_BEACH"), mesh));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, mesh));
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

	public class Dolphin : O_DOLPHIN
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			model = ObjectHelper.LoadModel("Objects/Levels/Emerald Coast/O DOLPHIN.sa1mdl");
			mesh = ObjectHelper.GetMeshes(model, dev);
		}

		public override string Name { get { return "Dolphin"; } }
	}

	public abstract class O_DOLSW : ObjectDefinition
	{
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			transform.Push();
			result = HitResult.Min(result, sphere.CheckHit(Near, Far, Viewport, Projection, View, transform, spheremsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			result.AddRange(sphere.DrawModelTree(dev, transform, null, spheremsh));
			if (item.Selected)
				result.AddRange(sphere.DrawModelTreeInvert(transform, spheremsh));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			return ObjectHelper.GetModelBounds(sphere, transform);
		}
	}

	public class Dolsw : O_DOLSW
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			sphere = ObjectHelper.LoadModel("Objects/Collision/C SPHERE.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere, dev);
		}

		public override string Name { get { return "Dolphin Trigger"; } }
	}
}