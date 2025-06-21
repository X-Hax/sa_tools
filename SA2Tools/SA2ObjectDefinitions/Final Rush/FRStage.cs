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

namespace SA2ObjectDefinitions.FinalRush
{
	public abstract class PlatformBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected Mesh meshMain;
		protected Mesh meshMain2;
		protected Mesh meshBack;
		protected Mesh meshRight;
		protected Mesh meshFront;
		protected Mesh meshLeft;
		protected Mesh[] meshesCustom;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg30_FinalRush/models/GC/FRSTAGE.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			meshMain = meshes[0];
			meshMain2 = meshes[1];
			meshBack = meshes[2];
			meshRight = meshes[3];
			meshFront = meshes[4];
			meshLeft = meshes[5];
			texarr = NJS_TEXLIST.Load("stg30_FinalRush/tls/FRSTAGE.satex");
			texpacks.Add("objtex_stg30");
			texpacks.Add("landtx30");
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - 0x8000, 0);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Left Rail", typeof(bool), "Extended", null, 0, (o) => Convert.ToBoolean((o.Rotation.X >> 12) & 0xF),
				(o, v) => { o.Rotation.X &= 0x0FFF; o.Rotation.X |= Convert.ToByte((bool)v) << 12; }),
			new PropertySpec("Front Rail", typeof(bool), "Extended", null, 0, (o) => Convert.ToBoolean((o.Rotation.X >> 8) & 0xF),
				(o, v) => { o.Rotation.X &= 0xF0FF; o.Rotation.X |= Convert.ToByte((bool)v) << 8; }),
			new PropertySpec("Right Rail", typeof(bool), "Extended", null, 0, (o) => Convert.ToBoolean((o.Rotation.X >> 4) & 0xF),
				(o, v) => { o.Rotation.X &= 0xFF0F; o.Rotation.X |= Convert.ToByte((bool)v) << 4; }),
			new PropertySpec("Back Rail", typeof(bool), "Extended", null, 0, (o) => Convert.ToBoolean(o.Rotation.X & 0xF),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= Convert.ToByte((bool)v); }),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class FRStage : PlatformBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<Mesh> usedMeshes = [];
			usedMeshes.Add(meshMain);
			usedMeshes.Add(meshMain2);
			if (((item.Rotation.X >> 12) & 0xF) == 1)
				usedMeshes.Add(meshLeft);
			if (((item.Rotation.X >> 8) & 0xF) == 1)
				usedMeshes.Add(meshFront);
			if (((item.Rotation.X >> 4) & 0xF) == 1)
				usedMeshes.Add(meshRight);
			if ((item.Rotation.X & 0xF) == 1)
				usedMeshes.Add(meshBack);
			meshesCustom = usedMeshes.ToArray();
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTexturesMultiSource(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshesCustom, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshesCustom));
			transform.Pop();
			return result;
		}
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			return ObjectHelper.GetModelBounds(model, transform);
		}
		public override string Name { get { return "Railed Platform"; } }
	}
	public class FRStageS : PlatformBase
	{
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<Mesh> usedMeshes = [];
			usedMeshes.Add(meshMain);
			usedMeshes.Add(meshMain2);
			if (((item.Rotation.X >> 12) & 0xF) == 1)
				usedMeshes.Add(meshLeft);
			if (((item.Rotation.X >> 8) & 0xF) == 1)
				usedMeshes.Add(meshFront);
			if (((item.Rotation.X >> 4) & 0xF) == 1)
				usedMeshes.Add(meshRight);
			if ((item.Rotation.X & 0xF) == 1)
				usedMeshes.Add(meshBack);
			meshesCustom = usedMeshes.ToArray();
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTexturesMultiSource(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshesCustom, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshesCustom));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			return ObjectHelper.GetModelBounds(model, transform);
		}
		public override string Name { get { return "Railed Platform (Type S)"; } }
	}
}