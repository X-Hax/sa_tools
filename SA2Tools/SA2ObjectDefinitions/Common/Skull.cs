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
	public abstract class SkullBase : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_TEXLIST texarr;
		private Texture[] texs;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_SKULL.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/SKULL.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class Skull : SkullBase
	{
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Spawn Animal", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.Z >> 4) == 10),
				(o, v) => { o.Rotation.Z &= 0xFF0F; o.Rotation.Z |= (Convert.ToByte((bool)v) * 10) << 4; }),
			new PropertySpec("Animal Group", typeof(byte), "Extended", null, null, (o) => o.Rotation.Z & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
			new PropertySpec("Has Collision", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean(o.Scale.X >= 1F),
				(o, v) => o.Scale.X = Convert.ToSingle(v)),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Skull"; } }
	}
	public class KSkull : SkullBase
	{
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Spawn Animal", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.Z >> 4) == 10),
				(o, v) => { o.Rotation.Z &= 0xFF0F; o.Rotation.Z |= (Convert.ToByte((bool)v) * 10) << 4; }),
			new PropertySpec("Animal Group", typeof(byte), "Extended", null, null, (o) => o.Rotation.Z & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
			new PropertySpec("Has Collision", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean(o.Scale.X >= 1F),
				(o, v) => o.Scale.X = Convert.ToSingle(!(bool)v))
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Skull (K Type)"; } }
	}
	public class PSkull : SkullBase
	{
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Spawn Animal", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.Z >> 4) == 10),
				(o, v) => { o.Rotation.Z &= 0xFF0F; o.Rotation.Z |= (Convert.ToByte((bool)v) * 10) << 4; }),
			new PropertySpec("Animal Group", typeof(byte), "Extended", null, null, (o) => o.Rotation.Z & 0xF,
				(o, v) => { o.Rotation.Z &= 0xFFF0; o.Rotation.Z |= (byte)v; }),
			new PropertySpec("Has Collision", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean(o.Scale.X < 1F),
				(o, v) => o.Scale.X = Convert.ToSingle(!(bool)v))
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Skull (P Type)"; } }
	}
	public class NoColSkull : SkullBase
	{
		public override string Name { get { return "Skull (Intangible)"; } }
	}
}