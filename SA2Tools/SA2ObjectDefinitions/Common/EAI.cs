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

namespace SA2ObjectDefinitions.Common
{
	public class EAI : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("enemy/ai/E_AI_GUN.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + 0x8000, item.Rotation.Z);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + 0x8000, item.Rotation.Z);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_aitex"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + 0x8000, item.Rotation.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x4000, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x4000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Bullets Laser Type", typeof(float), "Extended", null, 1.0f, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v > 0 ? (int)v : 99),
			new PropertySpec("Hunter Type", typeof(HunterType), "Extended", null, null, (o) => (HunterType)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Bullet laser glue Speed", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 60.0f),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Gun Hunter"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum HunterType
		{
			Activating,
			Moving,
			LaserGun,
			MovingLaserGun,
			GlueGun,
			ShieldLaserGun,
			Dropping,
			DroppingLaserGun,
		}
	}
}