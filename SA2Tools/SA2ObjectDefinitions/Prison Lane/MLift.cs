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
	public class MLift : ObjectDefinition
	{
		protected NJS_OBJECT models;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texlist;
		protected Texture[] texs;
		protected List<string> texpacks = [];
		public override void Init(ObjectData data, string name)
		{
			models = ObjectHelper.LoadModel("stg09_tailsprison/models/GC/MLIFT.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(models);
			texlist = NJS_TEXLIST.Load("stg09_tailsprison/tls/MLIFT.satex");

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
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 20F, item.Position.Z);
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
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 20F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models.DrawModelInvert(transform, meshes[1], EditorOptions.IgnoreMaterialColors));
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
			transform.NJTranslate(item.Position.X, item.Position.Y + item.Scale.X + 20F, item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			mainbounds = SAModel.Direct3D.Extensions.Merge(mainbounds, ObjectHelper.GetModelBounds(models, transform));
			transform.Pop();
			return mainbounds;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Lift Height", typeof(float), "Extended", "This value is added to 20 in the game's code, meaning that the lift will always ascend at least 20 units.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Elevator Platform"; } }
		public enum ElevatorType
		{
			TwoWay,
			OneWay
		}
	}
}