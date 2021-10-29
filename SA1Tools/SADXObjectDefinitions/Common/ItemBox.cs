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
	public abstract class ItemBoxBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected int childindex;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			((BasicAttach)model.Children[childindex].Attach).Material[0].TextureID = itemTexs[Math.Min(Math.Max((int)item.Scale.X, 0), 8)];
			transform.Push();
			transform.NJTranslate(item.Position);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			((BasicAttach)model.Children[childindex].Attach).Material[0].TextureID = itemTexs[Math.Min(Math.Max((int)item.Scale.X, 0), 8)];
			transform.Push();
			transform.NJTranslate(item.Position);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		internal int[] itemTexs = { 35, 72, 33, 32, 34, 71, 31, 73, 70 };

		internal int[] charTexs = { 31, 0, 4, 0, 0, 1, 3, 2 };

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Item", typeof(Items), "Extended", null, null, (o) => (Items)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v)
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
			model = ObjectHelper.LoadModel("object/itembox_boxbody.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			childindex = 2;
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
			model = ObjectHelper.LoadModel("object/itemboxair_boxbody.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			childindex = 1;
		}

		public override string Name { get { return "Floating Item Box"; } }
	}

	public enum Items
	{
		SpeedUp,
		Invincibility,
		FiveRings,
		TenRings,
		RandomRings,
		Barrier,
		ExtraLife,
		Bomb,
		MagneticBarrier
	}
}
