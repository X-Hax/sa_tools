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

namespace SA2ObjectDefinitions.PumpkinHill
{
	public abstract class PumCol : ObjectDefinition
	{
		protected NJS_OBJECT models;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texlist;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			HitResult result = models.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texlist, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			result.AddRange(models.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateZ(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateZ(item.Rotation.X);
			result.Add(new ModelTransform(models, transform.Top));
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			return ObjectHelper.GetModelBounds(models, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
	}
	public class PumWall : PumCol
	{
		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("stg05_pumpkin/models/GC/PUMWALL.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(models);
			texlist = NJS_TEXLIST.Load("stg05_pumpkin/tls/PUMWALL.satex");

			texpacks.Add("objtex_stg05");
			texpacks.Add("landtx05");
		}
		public override string Name { get { return "Metal Platform/Wall"; } }
	}
	public class PumWallS : PumCol
	{
		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("stg05_pumpkin/models/GC/PUMWALL.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(models);
			texlist = NJS_TEXLIST.Load("stg05_pumpkin/tls/PUMWALL.satex");

			texpacks.Add("objtex_stg05");
			texpacks.Add("landtx05");
		}
		public override string Name { get { return "Metal Platform/Wall (Type S)"; } }
	}
	public class PumSasi : PumCol
	{
		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("stg05_pumpkin/models/GC/PUMSASI.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(models);
			texlist = NJS_TEXLIST.Load("stg05_pumpkin/tls/PUMSASI.satex");

			texpacks.Add("objtex_stg05");
			texpacks.Add("landtx05");
		}
		public override string Name { get { return "Floating Platform with Bridge"; } }
	}
}