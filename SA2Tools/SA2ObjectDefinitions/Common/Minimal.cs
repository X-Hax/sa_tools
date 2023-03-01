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
	public class Minimal : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		
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

		//textures don't work at the moment - no texlists
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("al_minimal_tex"), meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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

		//doesn't work either - seems to just return a random animal
		/*private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animal Type", typeof(Animal), "Extended", null, null, (o) => (Animal)Math.Min(Math.Max((int)o.Scale.X, 0), 21), (o, v) => o.Scale.X = (int)v)
		};
		
		public override PropertySpec[] CustomProperties { get { return customProperties; } }*/

		public override string Name { get { return "Small Animal"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		
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