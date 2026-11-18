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
	public abstract class JungleObjectsBase : ObjectDefinition
	{
		protected NJS_OBJECT[] models;
		protected Mesh[][] meshes;
		protected NJS_TEXLIST[] texlists;
		protected Texture[][] textures;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[3];
			meshes = new Mesh[3][];
			texlists = new NJS_TEXLIST[3];
			textures = new Texture[3][];

			models[0] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/LargeBump.sa2bmdl");
			texlists[0] = NJS_TEXLIST.Load("stg03_Jungle/tls/LargeBump.satex");

			models[1] = ObjectHelper.LoadModel("stg03_Jungle/models/GC/SmallBump.sa2bmdl");
			texlists[1] = NJS_TEXLIST.Load("stg03_Jungle/tls/SmallBump.satex");

			models[2] = ObjectHelper.LoadModel("stg04_FogJungle/models/GC/TREE_S.sa2bmdl");
			texlists[2] = NJS_TEXLIST.Load("stg04_FogJungle/tls/TREE_S.satex");

			texpacks.Add("objtex_stg03");
			texpacks.Add("landtx03");

			for (int i = 0; i < 3; i++)
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
			new PropertySpec("Object", typeof(JungleObjectTypes), "Extended", null, null, (o) => (JungleObjectTypes)((int)o.Scale.X > 3 ? 2 : (int)o.Scale.X), (o, v) => o.Scale.X = (float)(JungleObjectTypes)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum JungleObjectTypes
		{
			LargeBump,
			SmallBump,
			SmallTree
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int objID = (int)item.Scale.X > 2 ? 2 : (int)item.Scale.X;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJTranslate(0, 0.5F, 0);
			HitResult result = models[objID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[objID]);
			transform.Pop();
			return result;
			
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			for (int i = 0; i < 3; i++)
			{
				if (textures[i] == null)
					textures[i] = ObjectHelper.GetTextures(texpacks, texlists[i]);
			}
			int objID = (int)item.Scale.X > 2 ? 2 : (int)item.Scale.X;
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJTranslate(0, 0.5F, 0);
			result.AddRange(models[objID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures[objID], meshes[objID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[objID].DrawModelTreeInvert(transform, meshes[objID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int objID = (int)item.Scale.X > 2 ? 2 : (int)item.Scale.X;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJTranslate(0, 0.5F, 0);
			result.Add(new ModelTransform(models[objID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int objID = (int)item.Scale.X > 2 ? 2 : (int)item.Scale.X;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			transform.NJTranslate(0, 0.5F, 0);
			return ObjectHelper.GetModelBounds(models[objID], transform);
		}
	}
	public class JungleObjects : JungleObjectsBase 
	{
		public override string Name { get { return "Jungle Object"; } }
	}
	public class JungleObjectsFar : JungleObjectsBase
	{
		public override string Name { get { return "Jungle Object (Far)"; } }
	}
}