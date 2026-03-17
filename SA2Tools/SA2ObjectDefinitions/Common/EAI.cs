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
		private NJS_OBJECT object_eai_gun;
		private NJS_OBJECT object_eai_laser;
		private NJS_OBJECT object_eai_shield;

		private NJS_MOTION motion_eai_activate;
		private NJS_MOTION motion_eai_idle;
		private NJS_MOTION motion_eai_move;
		private NJS_MOTION motion_eai_drop;

		public override void Init(ObjectData data, string name)
		{
			object_eai_gun = ObjectHelper.LoadModel("enemy/ai/E_AI_GUN.sa2mdl");
			object_eai_laser = ObjectHelper.LoadModel("enemy/ai/E_AI_LASER.sa2mdl");
			object_eai_shield = ObjectHelper.LoadModel("enemy/ai/E_AI_SHIELDER.sa2mdl");

			motion_eai_activate = ObjectHelper.LoadMotion("enemy/ai/Hunter11.saanim");
			motion_eai_idle = ObjectHelper.LoadMotion("enemy/ai/Hunter2.saanim");
			motion_eai_move = ObjectHelper.LoadMotion("enemy/ai/Hunter3.saanim");
			motion_eai_drop = ObjectHelper.LoadMotion("enemy/ai/Hunter1.saanim");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);

			HitResult result;

			NJS_OBJECT model;

			switch (hunterID)
			{
				case 2:
				case 3:
				case 7:
					model = object_eai_laser;
					break;
				case 5:
					model = object_eai_shield;
					break;
				default:
					model = object_eai_gun;
					break;
			}

			Mesh[] mesh = ObjectHelper.GetMeshes(model);

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), 0);
				result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh);
			}
			transform.Pop();

			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);

			List<RenderInfo> result = new List<RenderInfo>();

			NJS_OBJECT model;

			NJS_MOTION motion = motion_eai_idle;

			switch (hunterID)
			{
				case 0:
					model = object_eai_gun;
					break;
				case 2:
				case 3:
				case 7:
					model = object_eai_laser;
					break;
				case 5:
					model = object_eai_shield;
					break;
				default:
					model = object_eai_gun;
					break;
			}


			switch (hunterID)
			{
				case 0:
					motion = motion_eai_activate;
					break;
				case 1:
				case 3:
					motion = motion_eai_move;
					break;
				case 6:
				case 7:
					motion = motion_eai_drop;
					break;
				default:
					motion = motion_eai_idle;
					break;
			}

			Mesh[] mesh = ObjectHelper.GetMeshes(model);

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
//				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_aitex"), mesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				result.AddRange(model.DrawModelTreeAnimated(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_aitex"), mesh, motion, 0.0f, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(model.DrawModelTreeAnimatedInvert(transform, mesh, motion, 0.0f));
			}
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);

			List<ModelTransform> result = new List<ModelTransform>();

			NJS_OBJECT model;

			switch (hunterID)
			{
				case 2:
				case 3:
				case 7:
					model = object_eai_laser;
					break;
				case 5:
					model = object_eai_shield;
					break;
				default:
					model = object_eai_gun;
					break;
			}

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
				result.Add(new ModelTransform(model, transform.Top));
			}
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int hunterID = Math.Max((int)item.Scale.X, 0);

			MatrixStack transform = new MatrixStack();

			NJS_OBJECT model;

			switch (hunterID)
			{
				case 2:
				case 3:
				case 7:
					model = object_eai_laser;
					break;
				case 5:
					model = object_eai_shield;
					break;
				default:
					model = object_eai_gun;
					break;
			}

			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + ObjectHelper.DegToBAMS(90);
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