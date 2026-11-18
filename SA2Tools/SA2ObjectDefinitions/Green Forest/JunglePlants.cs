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
using System;

namespace SA2ObjectDefinitions.GreenForest
{
	public abstract class JunglePlantsBase : ObjectDefinition
	{
		protected NJS_OBJECT[] models;
		protected Mesh[][] meshes;
		protected NJS_TEXLIST[] texlists;
		protected Texture[][] textures;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[18];
			meshes = new Mesh[18][];
			texlists = new NJS_TEXLIST[18];
			textures = new Texture[18][];

			models[0] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/HugeTree.sa2bmdl");
			texlists[0] = NJS_TEXLIST.Load("stg03_Jungle/tls/HugeTree.satex");

			models[1] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/HugeLeaves.sa2bmdl");
			texlists[1] = NJS_TEXLIST.Load("stg03_Jungle/tls/HugeLeaves.satex");

			models[2] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/SmallLeaves.sa2bmdl");
			texlists[2] = NJS_TEXLIST.Load("stg03_Jungle/tls/SmallLeaves.satex");

			models[3] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Shrubbery.sa2bmdl");
			texlists[3] = NJS_TEXLIST.Load("stg03_Jungle/tls/Shrubbery.satex");

			models[4] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/LeafPatch.sa2bmdl");
			texlists[4] = NJS_TEXLIST.Load("stg03_Jungle/tls/LeafPatch.satex");

			models[5] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Shrubbery2.sa2bmdl");
			texlists[5] = NJS_TEXLIST.Load("stg03_Jungle/tls/Shrubbery2.satex");

			models[6] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Shrubbery3.sa2bmdl");
			texlists[6] = NJS_TEXLIST.Load("stg03_Jungle/tls/Shrubbery3.satex");

			models[7] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Shrubbery4.sa2bmdl");
			texlists[7] = NJS_TEXLIST.Load("stg03_Jungle/tls/Shrubbery4.satex");

			models[8] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Shrubbery5.sa2bmdl");
			texlists[8] = NJS_TEXLIST.Load("stg03_Jungle/tls/Shrubbery5.satex");

			models[9] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Flowers.sa2bmdl");
			texlists[9] = NJS_TEXLIST.Load("stg03_Jungle/tls/Flowers.satex");

			models[10] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/SmallTree.sa2bmdl");
			texlists[10] = NJS_TEXLIST.Load("stg03_Jungle/tls/SmallTree.satex");

			models[11] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/TallTree.sa2bmdl");
			texlists[11] = NJS_TEXLIST.Load("stg03_Jungle/tls/TallTree.satex");

			models[12] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/Overgrowth.sa2bmdl");
			texlists[12] = NJS_TEXLIST.Load("stg03_Jungle/tls/Overgrowth.satex");

			models[13] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/TallTree2.sa2bmdl");
			texlists[13] = NJS_TEXLIST.Load("stg03_Jungle/tls/TallTree2.satex");

			models[14] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/HorizontalTree.sa2bmdl");
			texlists[14] = NJS_TEXLIST.Load("stg03_Jungle/tls/HorizontalTree.satex");

			models[15] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/SmallCurvedTree.sa2bmdl");
			texlists[15] = NJS_TEXLIST.Load("stg03_Jungle/tls/CurvedTree.satex");

			models[16] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/HangingGrowth.sa2bmdl");
			texlists[16] = NJS_TEXLIST.Load("stg03_Jungle/tls/HangingGrowth.satex");

			models[17] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/RootGrowth.sa2bmdl");
			texlists[17] = NJS_TEXLIST.Load("stg03_Jungle/tls/RootGrowth.satex");

			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");

			for (int i = 0; i < 18; i++)
			{
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
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
			new PropertySpec("Plant Type", typeof(JunglePlantTypes), "Extended", null, null, (o) => (JunglePlantTypes)((int)o.Scale.X % 18), (o, v) => o.Scale.X = (float)(JunglePlantTypes)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum JunglePlantTypes
		{
			LargeVines,
			RedLeaf,
			GreenLeaf,
			RedShrub,
			LeafPatch,
			VinesWithLeaves,
			FanShrub,
			PalmShrub,
			RedPalm,
			Bromeliad,
			SmallTree,
			TallTree,
			Overgrowth,
			TallPalm,
			SideTree,
			CurvedTree,
			Vines,
			RootGrowth
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int objID = (int)item.Scale.X % 18;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 0.5F, 0);
			HitResult result = models[objID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[objID]);
			transform.Pop();
			return result;
			
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			for (int i = 0; i < 18; i++)
			{
				if (textures[i] == null)
					textures[i] = ObjectHelper.GetTextures(texpacks, texlists[i]);
			}
			int objID = (int)item.Scale.X % 18;
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 0.5F, 0);
			result.AddRange(models[objID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures[objID], meshes[objID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[objID].DrawModelTreeInvert(transform, meshes[objID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int objID = (int)item.Scale.X % 18;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 0.5F, 0);
			result.Add(new ModelTransform(models[objID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int objID = (int)item.Scale.X % 18;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJTranslate(0, 0.5F, 0);
			return ObjectHelper.GetModelBounds(models[objID], transform);
		}
	}
	public class JunglePlants : JunglePlantsBase 
	{
		public override string Name { get { return "Jungle Plant"; } }
	}
	public class JunglePlantsFar : JunglePlantsBase
	{
		public override string Name { get { return "Jungle Plant (Far)"; } }
	}
}