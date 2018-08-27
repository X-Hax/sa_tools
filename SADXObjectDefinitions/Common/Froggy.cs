using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SonicRetro.SAModel.BoundingSphere;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public abstract class O_Frog : ObjectDefinition
	{
		protected NJS_OBJECT frog;
		protected Mesh[] frogmsh;
		protected NJS_OBJECT bubble;
		protected Mesh[] bubblemsh;
		protected NJS_OBJECT sphere;
		protected Mesh[] spheremsh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			transform.Push();
			result = HitResult.Min(result, frog.CheckHit(Near, Far, Viewport, Projection, View, transform, frogmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.Push();
			result = HitResult.Min(result, bubble.CheckHit(Near, Far, Viewport, Projection, View, transform, bubblemsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
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
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(frog.DrawModelTree(dev, transform, ObjectHelper.GetTextures("BIG_KAERU"), frogmsh));
			if (item.Selected)
				result.AddRange(frog.DrawModelTreeInvert(transform, frogmsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(bubble.DrawModelTree(dev, transform, null, bubblemsh));
			if (item.Selected)
				result.AddRange(bubble.DrawModelTreeInvert(transform, bubblemsh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
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
			transform.NJTranslate(item.Position.X, (item.Position.Y + item.Scale.Y), item.Position.Z);
			transform.NJScale((item.Scale.X + 1f), (item.Scale.X + 1f), (item.Scale.X + 1f));
			return ObjectHelper.GetModelBounds(sphere, transform);
		}
	}

	public class OFrog : O_Frog
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			frog = ObjectHelper.LoadModel("Objects/Common/FROGGY.sa1mdl");
			frogmsh = ObjectHelper.GetMeshes(frog, dev);
			bubble = ObjectHelper.LoadModel("Objects/Common/Animals/AnimalBubble.sa1mdl");
			bubblemsh = ObjectHelper.GetMeshes(bubble, dev);
			sphere = ObjectHelper.LoadModel("Objects/Collision/C SPHERE.sa1mdl");
			spheremsh = ObjectHelper.GetMeshes(sphere, dev);
		}

		public override string Name { get { return "Froggy (Bubble)"; } }
	}

	public abstract class Kaeru : ObjectDefinition
	{
		protected NJS_OBJECT frog;
		protected Mesh[] frogmsh;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			transform.Push();
			result = HitResult.Min(result, frog.CheckHit(Near, Far, Viewport, Projection, View, transform, frogmsh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			result.AddRange(frog.DrawModelTree(dev, transform, ObjectHelper.GetTextures("BIG_KAERU"), frogmsh));
			if (item.Selected)
				result.AddRange(frog.DrawModelTreeInvert(transform, frogmsh));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			transform.NJTranslate(2f, 0f, 0f);
			return ObjectHelper.GetModelBounds(frog, transform);
		}
	}

	public class Froggy : Kaeru
	{
		public override void Init(ObjectData data, string name, Device dev)
		{
			frog = ObjectHelper.LoadModel("Objects/Common/FROGGY.sa1mdl");
			frogmsh = ObjectHelper.GetMeshes(frog, dev);
		}

		public override string Name { get { return "Froggy"; } }
	}
}