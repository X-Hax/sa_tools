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
using SplitTools;

namespace SA2ObjectDefinitions.Common
{
	public class EKUMI : ObjectDefinition
	{
		private NJS_OBJECT object_kumi;
		private Mesh[] mesh1;
		private NJS_OBJECT object_kumi_elec;
		private Mesh[] mesh2;
		private NJS_OBJECT object_kumi_gun;
		private Mesh[] mesh3;
		private NJS_OBJECT object_kump_spring;
		private Mesh[] mesh4;
		private NJS_OBJECT object_kumi_bomb;
		private Mesh[] mesh5;

		public override void Init(ObjectData data, string name)
		{
			object_kumi = ObjectHelper.LoadModel("enemy/kumi/E_KUMI.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(object_kumi);
			object_kumi_elec = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_ELEC_DISCHARGE.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(object_kumi_elec);
			object_kumi_gun = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_GUNNER.sa2mdl");
			mesh3 = ObjectHelper.GetMeshes(object_kumi_gun);
			object_kump_spring = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_BUMPER.sa2mdl");
			mesh4 = ObjectHelper.GetMeshes(object_kump_spring);
			object_kumi_bomb = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_BOMBER.sa2mdl");
			mesh5 = ObjectHelper.GetMeshes(object_kumi_bomb);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (beetleID)
			{
				default:
					pObject = object_kumi;
					break;
				case 4:
					pObject = object_kumi_gun;
					break;
				case 6:
					pObject = object_kump_spring;
					break;
				case 7:
					pObject = object_kumi_bomb;
					break;
			}

			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			HitResult result = pObject.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(pObject));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;
			Texture[] pTexs;

			bool eKumi = false;

			switch (beetleID)
			{
				default:
					pObject = object_kumi;
					pTexs = ObjectHelper.GetTextures("e_kumitex");
					break;
				case 4:
					pObject = object_kumi_gun;
					pTexs = ObjectHelper.GetTextures("e_g_kumitex");
					break;
				case 6:
					pObject = object_kump_spring;
					pTexs = ObjectHelper.GetTextures("e_s_kumitex");
					break;
				case 7:
					pObject = object_kumi_bomb;
					pTexs = ObjectHelper.GetTextures("e_b_kumitex");
					break;
			}

			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.AddRange(pObject.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, pTexs, ObjectHelper.GetMeshes(pObject), EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				
				if (eKumi)
					result.AddRange(object_kumi_elec.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_e_kumitex"), ObjectHelper.GetMeshes(object_kumi_elec), EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				
				if (item.Selected)
				{
					result.AddRange(pObject.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(pObject)));

					if (eKumi)
						result.AddRange(object_kumi_elec.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(object_kumi_elec)));
				}
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (beetleID)
			{
				default:
					pObject = object_kumi;
					break;
				case 4:
					pObject = object_kumi_gun;
					break;
				case 6:
					pObject = object_kump_spring;
					break;
				case 7:
					pObject = object_kumi_bomb;
					break;
			}

			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(pObject, transform.Top));
			}
			transform.Pop();

			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (beetleID)
			{
				default:
					pObject = object_kumi;
					break;
				case 4:
					pObject = object_kumi_gun;
					break;
				case 6:
					pObject = object_kump_spring;
					break;
				case 7:
					pObject = object_kumi_bomb;
					break;
			}

			MatrixStack transform = new MatrixStack();

			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y, 0);

			return ObjectHelper.GetModelBounds(pObject, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y, 0);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Speed (Mono Beetle, Moving)", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Bullets Fired (Gun Beetle)", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xF,
			(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Bomb Type (Bomb Beetle)", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xF,
			(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Spring Power (Spring Beetle)", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xF,
			(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Emerald/Item Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xFF,
			(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Oscillation/Rotation Speed", typeof(int), "Extended", null, 1, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v > 0 ? (int)v : 999999),
			new PropertySpec("Draw Type", typeof(DrawType), "Extended", null, null, (o) => o.Rotation.Z & 0xF,
			(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
			new PropertySpec("Beetle Type", typeof(BeetleType), "Extended", null, null, (o) => (BeetleType)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Bullet Speed/Electric Time", typeof(float), "Extended", null, 1.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 999.0f),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 10.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "GUN Beetle"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum BeetleType
		{
			Normal,
			Moving,
			Electric,
			ElectricAndMoving,
			Gunned,
			Appearing,
			Spring,
			Bombing,
		}
		public enum DrawType
		{
			Simple,
			Direct
		}
	}
}