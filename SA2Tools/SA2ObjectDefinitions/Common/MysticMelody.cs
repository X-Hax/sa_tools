using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;

namespace SA2ObjectDefinitions.Common
{
	public abstract class MysticMelody : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, item.Rotation.Z);
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
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, item.Rotation.Z);
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
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Shrine ID", typeof(float), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	
	public class Shrine : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_KNUDAI.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/KNUDAI.satex");
		}
		
		public override string Name { get { return "Mystic Melody Shrine"; } }
	}
	
	public class Platform : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_KDASIBA.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/KDASIBA.satex");
		}
		
		public override string Name { get { return "Mystic Melody Platform"; } }
	}
	
	public class Door : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_KDDOOR.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/KDDOOR.satex");
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Shrine ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Door Orientation (Debug)", typeof(MysticDoorOpen), "Extended", null, null, (o) => (MysticDoorOpen)(o.Rotation.X >> 8),
			(o, v) => { o.Rotation.X &= 0xFF; o.Rotation.X |= (byte)v << 8; }),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Mystic Melody Door (Vertical)"; } }
		public enum MysticDoorOpen : byte
		{
			Upward,
			LeftToRight,
			Upward2,
			RightToLeft
		}
	}
}