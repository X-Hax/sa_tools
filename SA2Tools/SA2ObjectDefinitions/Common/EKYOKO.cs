using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Common
{
	public abstract class EKYOKOBase : ObjectDefinition
	{
		private NJS_OBJECT object_kyoko;
		private Mesh[] meshes;
		private NJS_TEXLIST texarr;
		private Texture[] texs;

		public override void Init(ObjectData data, string name)
		{
			object_kyoko = ObjectHelper.LoadModel("enemy/kyoko/E_KYOKO.sa2mdl");
			meshes = ObjectHelper.GetMeshes(object_kyoko);
			texarr = NJS_TEXLIST.Load("enemy/kyoko/tls/KYOKO.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
				result = object_kyoko.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			}
			transform.Pop();

			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("E_KYOKOTEX", texarr, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(object_kyoko.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(object_kyoko.DrawModelTreeInvert(transform, meshes));
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(object_kyoko, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(object_kyoko, transform);
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
			new PropertySpec("Smoke Type", typeof(byte), "Extended", null, null, (o) => ((o.Rotation.X >> 4) & 0xF),
			(o, v) => { o.Rotation.X &= 0xFF0F; o.Rotation.X |= (byte)v << 4; }),
			new PropertySpec("Laser Length", typeof(byte), "Extended", null, null, (o) => ((o.Rotation.X >> 12) & 0xFF),
			(o, v) => { o.Rotation.X &= 0x00FF; o.Rotation.X |= (byte)v << 12; }),
			new PropertySpec("Hawk Type", typeof(HawkType), "Extended", null, null, (o) => (HawkType)(o.Rotation.X & 0xF),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Emerald/Item Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xFF,
			(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Bullet Frequency/Laser Cooldown", typeof(float), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Bullet/Laser Speed", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Chase Range", typeof(float), "Extended", "Determines the maximum Y distance in which the robot will be able to ascend/descend to chase the player.", null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class EKYOKO : EKYOKOBase
	{
		public override string Name { get { return "GUN Hawk"; } }
	}
	public class EKYOKON : EKYOKOBase
	{
		public override string Name { get { return "GUN Hawk (Near)"; } }
	}
	public class EKYOKOF : EKYOKOBase
	{
		public override string Name { get { return "GUN Hawk (Far)"; } }
	}

	public enum HawkType : byte
	{
		Move,
		Gun,
		GunMove,
		Laser,
		GlueGun,
		MoveAlt,
		GunAlt,
		GunMoveAlt,
		LaserAlt,
		GlueGunAlt,
	}
}