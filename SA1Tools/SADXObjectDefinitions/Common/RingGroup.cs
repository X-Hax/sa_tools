using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class RingGroup : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/obj_ring.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
			{
				transform.Push();
				if (item.Scale.Z == 1) // circle
				{
					double v4 = i * 360.0;
					Vector3 v7 = new Vector3(
						ObjectHelper.NJSin((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y,
						0,
						ObjectHelper.NJCos((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y);
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
					result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				}
				else // line
				{
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					double v5;
					if (i % 2 == 1)
						v5 = i * item.Scale.Y * -0.5;
					else
						v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
					Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
					result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
				}
				transform.Pop();
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
			{
				transform.Push();
				if (item.Scale.Z == 1) // circle
				{
					double v4 = i * 360.0;
					Vector3 v7 = new Vector3(
						ObjectHelper.NJSin((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y,
						0,
						ObjectHelper.NJCos((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y);
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
					result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
					if (item.Selected)
						result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				}
				else // line
				{
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					double v5;
					if (i % 2 == 1)
						v5 = i * item.Scale.Y * -0.5;
					else
						v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
					Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
					result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
					if (item.Selected)
						result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				}
				transform.Pop();
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
			{
				transform.Push();
				if (item.Scale.Z == 1) // circle
				{
					double v4 = i * 360.0;
					Vector3 v7 = new Vector3(
						ObjectHelper.NJSin((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y,
						0,
						ObjectHelper.NJCos((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y);
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
					result.Add(new ModelTransform(model, transform.Top));
				}
				else // line
				{
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					double v5;
					if (i % 2 == 1)
						v5 = i * item.Scale.Y * -0.5;
					else
						v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
					Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
					result.Add(new ModelTransform(model, transform.Top));
				}
				transform.Pop();
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			BoundingSphere result = new BoundingSphere();
			for (int i = 0; i < Math.Min(item.Scale.X + 1, 8); i++)
			{
				transform.Push();
				if (item.Scale.Z == 1) // circle
				{
					double v4 = i * 360.0;
					Vector3 v7 = new Vector3(
						ObjectHelper.NJSin((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y,
						0,
						ObjectHelper.NJCos((int)(v4 / item.Scale.X * 65536.0 * 0.002777777777777778)) * item.Scale.Y);
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					Vector3 pos = Vector3.TransformCoordinate(v7, transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
				}
				else // line
				{
					transform.Push();
					transform.NJTranslate(item.Position);
					transform.NJRotateObject(item.Rotation);
					double v5;
					if (i % 2 == 1)
						v5 = i * item.Scale.Y * -0.5;
					else
						v5 = Math.Ceiling(i * 0.5) * item.Scale.Y;
					Vector3 pos = Vector3.TransformCoordinate(new Vector3(0, 0, (float)v5), transform.Top);
					transform.Pop();
					transform.NJTranslate(pos);
				}
				result = SAModel.Direct3D.Extensions.Merge(result, ObjectHelper.GetModelBounds(model, transform));
				transform.Pop();
			}
			return result;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		public override string Name { get { return "Ring Group"; } }

		public override float DistanceFromGround
		{
			get
			{
				return 7;
			}
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Number of Rings", typeof(byte), "Extended", null, 1, (o) => (byte)Math.Min(o.Scale.X + 1, 8), (o, v) => o.Scale.X = Math.Max(Math.Min((byte)v - 1, 8), 0)),
			new PropertySpec("Size", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Group Type", typeof(RingGroupType), "Extended", null, null, (o) => o.Scale.Z == 1 ? RingGroupType.Circle : RingGroupType.Line, (o, v) => o.Scale.Z = (RingGroupType)v == RingGroupType.Circle ? 1 : 0)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}

	public enum RingGroupType
	{
		Line,
		Circle
	}
}