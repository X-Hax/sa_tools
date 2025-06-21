using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
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
		private NJS_OBJECT model1;
		private Mesh[] mesh1;
		private NJS_OBJECT model2;
		private Mesh[] mesh2;
		private NJS_OBJECT model3;
		private Mesh[] mesh3;

		public override void Init(ObjectData data, string name)
		{
			model1 = ObjectHelper.LoadModel("enemy/ai/E_AI_GUN.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("enemy/ai/E_AI_LASER.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			model3 = ObjectHelper.LoadModel("enemy/ai/E_AI_SHIELDER.sa2mdl");
			mesh3 = ObjectHelper.GetMeshes(model3);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);
			if (hunterID == 2 || hunterID == 3 || hunterID == 7)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, 0);
				HitResult result = model2.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh2);
				transform.Pop();
				return result;
			}
			else if (hunterID == 5)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, 0);
				HitResult result = model3.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh3);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, 0);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh1);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);
			if (hunterID == 2 || hunterID == 3 || hunterID == 7)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_aitex"), mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model2.DrawModelTreeInvert(transform, mesh2));
				transform.Pop();
				return result;
			}
			else if (hunterID == 5)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_aitex"), mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model3.DrawModelTreeInvert(transform, mesh3));
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_aitex"), mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model1.DrawModelTreeInvert(transform, mesh1));
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);
			if (hunterID == 2 || hunterID == 3 || hunterID == 7)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.Add(new ModelTransform(model2, transform.Top));
				transform.Pop();
				return result;
			}
			else if (hunterID == 5)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.Add(new ModelTransform(model3, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.Add(new ModelTransform(model1, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);
			if (hunterID == 2 || hunterID == 3 || hunterID == 7)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(model2, transform);
			}
			if (hunterID == 5)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(model3, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(model1, transform);
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y + 0x4000, item.Rotation.Z);

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
			new PropertySpec("Emerald/Item Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xFF,
			(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Hunter Type", typeof(HunterType), "Extended", null, null, (o) => (HunterType)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Bullet laser glue Speed", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 60.0f),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "GUN Hunter"; } }

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