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
using SplitTools;
using SAModel.SAEditorCommon;

namespace SA2ObjectDefinitions.Common
{
	public abstract class ItemBoxBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected int childindex;
		protected int polyindex;
		protected UInt16 ItemBoxLength = 11;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		internal string[] itemnames =
		{
			"itemp_speed",
			"itemp_5ring",
			"itemp_1up",
			"itemp_10ring",
			"itemp_20ring",
			"itemp_barrier",
			"itemp_bomb",
			"itemp_life",
			"itemp_magnet",
			"itemp_super",
		};

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			int itemID = Math.Min(Math.Max((int)item.Scale.X, 0), 10);
			switch (itemID)
			{
				case 0:
				default:
					texarr.TextureNames[0] = texarr.TextureNames[3];
					break;
				case 1:
					texarr.TextureNames[0] = texarr.TextureNames[4];
					break;
				case 2:
					texarr.TextureNames[0] = texarr.TextureNames[5];
					break;
				case 3:
					texarr.TextureNames[0] = texarr.TextureNames[6];
					break;
				case 4:
					texarr.TextureNames[0] = texarr.TextureNames[7];
					break;
				case 5:
					texarr.TextureNames[0] = texarr.TextureNames[8];
					break;
				case 6:
					texarr.TextureNames[0] = texarr.TextureNames[9];
					break;
				case 7:
					texarr.TextureNames[0] = texarr.TextureNames[10];
					break;
				case 8:
					texarr.TextureNames[0] = texarr.TextureNames[11];
					break;
				case 10:
					texarr.TextureNames[0] = texarr.TextureNames[12];
					break;
			}
			texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0) 
			transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.Z != 0)
			transform.NJRotateZ(item.Rotation.Z);
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
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		internal int[] itemTexs = { 15, 9, 2, 1, 8, 10, 11, 12, 13, 18, 16 };

		internal int[] charTexs = { 2, 5, 3, 6, 4, 7};

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Item", typeof(Items), "Extended", null, null, (o) => (Items)Math.Min(Math.Max((int)o.Scale.X, 0), 10), (o, v) => o.Scale.X = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}
	}

	public class ItemBox : ItemBoxBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_ITEMBOX.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			childindex = 2;
			polyindex = 7;
			texarr = NJS_TEXLIST.Load("object/tls/itembox.satex");
			List<string> names = new List<string>();
			for (int i = 0; i < texarr.TextureNames.Length; i++)
			{
				names.Add(texarr.TextureNames[i]);
			}
			for (int n = 0; n < itemnames.Length; n++)
			{
				names.Add(itemnames[n]);
			}
			texarr.TextureNames = names.ToArray();
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x;
			int z;
			direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override string Name { get { return "Item Box"; } }
	}

	public class FloatingItemBox : ItemBoxBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_ITEMBOXAIR.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			childindex = 1;
			polyindex = 7;
			texarr = NJS_TEXLIST.Load("object/tls/itemboxair.satex");
			List<string> names = new List<string>();
			for (int i = 0; i < texarr.TextureNames.Length; i++)
			{
				names.Add(texarr.TextureNames[i]);
			}
			for (int n = 0; n < itemnames.Length; n++) 
			{
				names.Add(itemnames[n]);
			}
			texarr.TextureNames = names.ToArray();
		}

		public override string Name { get { return "Floating Item Box"; } }
	}

	public class ItemBalloon : ItemBoxBase
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_ITEMBOXBALLOON.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/ITEMBOXBALLOON.satex");
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
			texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}
		public override string Name { get { return "Item Balloon"; } }
	}
	
	public class MstItemBox : ItemBox
	{
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Item", typeof(Items), "Extended", null, null, (o) => (Items)Math.Min(Math.Max((int)o.Scale.X, 0), 10), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Shrine ID", typeof(float), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Item Box (Mystic Melody)"; } }
	}
	
	public class HidItemBox : ItemBox
	{
		public override string Name { get { return "Item Box (Hidden)"; } }
	}

	public enum Items
	{
		SpeedUp,
		FiveRings,
		ExtraLife,
		TenRings,
		TwentyRings,
		Barrier,
		Bomb,
		HealthPack,
		MagneticBarrier,
		Empty,
		Invincibility,
	}
}