using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class Emerald_Goal : ObjectDefinition
	{
		private NJS_OBJECT eme_a;
		private Mesh[] ememsh_a;
		private NJS_OBJECT eme_b;
		private Mesh[] ememsh_b;
		private NJS_OBJECT eme_c;
		private Mesh[] ememsh_c;

		public override void Init(ObjectData data, string name)
		{
			eme_a = ObjectHelper.LoadModel("stg02_windy/common/models/goaleme_blue.nja.sa1mdl");
			ememsh_a = ObjectHelper.GetMeshes(eme_a);
			eme_b = ObjectHelper.LoadModel("stg02_windy/common/models/goaleme_white.nja.sa1mdl");
			ememsh_b = ObjectHelper.GetMeshes(eme_b);
			eme_c = ObjectHelper.LoadModel("stg02_windy/common/models/goaleme_green.nja.sa1mdl");
			ememsh_c = ObjectHelper.GetMeshes(eme_c);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			if (item.Scale.Y != 0)
			{
				if ((item.Scale.Y - 1) == 0 && (item.Scale.Y - 1) == 1)
				{
					transform.Push();
					transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
					transform.NJRotateObject(item.Rotation);
					HitResult result = eme_b.CheckHit(Near, Far, Viewport, Projection, View, transform, ememsh_b);
					transform.Pop();
					return result;
				}
				else
				{
					transform.Push();
					transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
					transform.NJRotateObject(item.Rotation);
					HitResult result = eme_c.CheckHit(Near, Far, Viewport, Projection, View, transform, ememsh_c);
					transform.Pop();
					return result;
				}
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
				transform.NJRotateObject(item.Rotation);
				HitResult result = eme_a.CheckHit(Near, Far, Viewport, Projection, View, transform, ememsh_a);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (item.Scale.Y != 0)
			{
				if ((item.Scale.Y - 1) == 0 && (item.Scale.Y - 1) == 1)
				{
					transform.Push();
					transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
					transform.NJRotateObject(item.Rotation);
					result.AddRange(eme_b.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("KAOS_EME"), ememsh_b));
					if (item.Selected)
						result.AddRange(eme_b.DrawModelTreeInvert(transform, ememsh_b));
					transform.Pop();
					return result;
				}
				else
				{
					transform.Push();
					transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
					transform.NJRotateObject(item.Rotation);
					result.AddRange(eme_c.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("KAOS_EME"), ememsh_c));
					if (item.Selected)
						result.AddRange(eme_c.DrawModelTreeInvert(transform, ememsh_c));
					transform.Pop();
					return result;
				}
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
				transform.NJRotateObject(item.Rotation);
				result.AddRange(eme_a.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("KAOS_EME"), ememsh_a));
				if (item.Selected)
					result.AddRange(eme_a.DrawModelTreeInvert(transform, ememsh_a));
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			if (item.Scale.Y != 0)
			{
				if ((item.Scale.Y - 1) == 0 && (item.Scale.Y - 1) == 1)
				{
					transform.Push();
					transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
					transform.NJRotateObject(item.Rotation);
					result.Add(new ModelTransform(eme_b, transform.Top));
					transform.Pop();
					return result;
				}
				else
				{
					transform.Push();
					transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
					transform.NJRotateObject(item.Rotation);
					result.Add(new ModelTransform(eme_c, transform.Top));
					transform.Pop();
					return result;
				}
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
				transform.NJRotateObject(item.Rotation);
				result.Add(new ModelTransform(eme_a, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, (item.Position.Y + 10f), item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(eme_a, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		public override string Name { get { return "Chaos Emerald (Goal)"; } }
	}
}