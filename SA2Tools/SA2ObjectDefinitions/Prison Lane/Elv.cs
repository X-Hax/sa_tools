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

namespace SA2ObjectDefinitions.PrisonLane
{
	public class Elv : ObjectDefinition
	{
		protected NJS_OBJECT models;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texlist;
		protected Texture[] texs;
		protected List<string> texpacks = [];
		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("stg09_tailsprison/models/GC/ELV.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(models);
			texlist = NJS_TEXLIST.Load("stg09_tailsprison/tls/ELV.satex");

			texpacks.Add("objtex_stg09");
			texpacks.Add("landtx09");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, models.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, models.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
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
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateZ(item.Rotation.Y);
			result.Add(new ModelTransform(models, transform.Top));
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere mainbounds = ObjectHelper.GetModelBounds(models, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere endbounds = ObjectHelper.GetModelBounds(models, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(mainbounds, endbounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("X Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Z Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Movement Type", typeof(ElevatorType), "Extended", null, null, (o) => (ElevatorType)(o.Rotation.X % 2), (o, v) => o.Rotation.X = (int)(ElevatorType)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Mini Lift"; } }
		public enum ElevatorType
		{
			TwoWay,
			OneWay
		}
	}
}