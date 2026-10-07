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
	public class Minimal : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		protected Texture[] texs;
		protected NJS_TEXLIST texarr;
		
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("chao/animals/MINIMAL_ARA.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("AL_MINIMAL_TEX", null, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
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
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
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

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animal Condition", typeof(AnimalCondition), "Extended", null, null, (o) => (AnimalCondition)((o.Rotation.X >> 8) & 0xF),
				(o, v) => { o.Rotation.X &= 0xF0FF; o.Rotation.X |= ((byte)v) << 8; }),
			new PropertySpec("Animal Type", typeof(AnimalSet), "Extended", null, null, (o) => (AnimalSet)(o.Rotation.X & 0xFF),
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Movement Radius", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Speed", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),

		};
		
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Small Animal"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public enum AnimalCondition
		{
			Roam,
			Stationary,
			Whistle
		}
		
		public enum AnimalSet
		{
			Rare,
			RandomSetA1 = 0x10,
			RandomSetA2,
			RandomSetAAll0,
			RandomSetAAll1,
			RandomSetAAll2,
			RandomSetAAll3,
			RandomSetAAll4,
			RandomSetAAll5,
			RandomSetAAll6,
			RandomSetAAll7,
			RandomSetAAll8,
			RandomSetAAll9,
			RandomSetAAll10,
			RandomSetAAll11,
			RandomSetAAll12,
			RandomSetAAll13,
			RandomSetB1,
			RandomSetB2,
			RandomSetBAll0,
			RandomSetBAll1,
			RandomSetBAll2,
			RandomSetBAll3,
			RandomSetBAll4,
			RandomSetBAll5,
			RandomSetBAll6,
			RandomSetBAll7,
			RandomSetBAll8,
			RandomSetBAll9,
			RandomSetBAll10,
			RandomSetBAll11,
			RandomSetBAll12,
			RandomSetBAll13,
			Random
		}
		/*public enum Animal
		{
			Phoenix,
			Unicorn,
			Dragon,
			Bat,
			SkeletonDog,
			HalfFish,
			Raccoon,
			Sheep,
			Skunk,
			Condor,
			Parrot,
			Peacock,
			Gorilla,
			Tiger,
			Bear,
			Boar,
			Cheetah,
			Rabbit,
			Otter,
			Seal,
			Penguin,
		}*/
	}
}