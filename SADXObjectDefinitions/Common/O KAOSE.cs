using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System;

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

		public override void Init(ObjectData data, string name, Device dev)
		{
			eme_a = ObjectHelper.LoadModel("Objects/Common/O KAOSE_A.sa1mdl");
			ememsh_a = ObjectHelper.GetMeshes(eme_a, dev);
			eme_b = ObjectHelper.LoadModel("Objects/Common/O KAOSE_B.sa1mdl");
			ememsh_b = ObjectHelper.GetMeshes(eme_b, dev);
			eme_c = ObjectHelper.LoadModel("Objects/Common/O KAOSE_C.sa1mdl");
			ememsh_c = ObjectHelper.GetMeshes(eme_c, dev);
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
					result.AddRange(eme_b.DrawModelTree(dev, transform, ObjectHelper.GetTextures("KAOS_EME"), ememsh_b));
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
					result.AddRange(eme_c.DrawModelTree(dev, transform, ObjectHelper.GetTextures("KAOS_EME"), ememsh_c));
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
				result.AddRange(eme_a.DrawModelTree(dev, transform, ObjectHelper.GetTextures("KAOS_EME"), ememsh_a));
				if (item.Selected)
					result.AddRange(eme_a.DrawModelTreeInvert(transform, ememsh_a));
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

		public override string Name { get { return "Chaos Emerald (Goal)"; } }
	}
}