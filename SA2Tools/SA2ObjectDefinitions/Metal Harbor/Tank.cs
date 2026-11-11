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

namespace SA2ObjectDefinitions.MetalHarbor
{
	public class Tank : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_TEXLIST texarr;
		private Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/Tank.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/Tank.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
			transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}
		
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[0], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelInvert(transform, meshes[0], false));
			transform.NJTranslate(0, item.Scale.Y, 0);
			result.AddRange(model.Children[0].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[0], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.Children[0].DrawModelInvert(transform, meshes[0], false));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(model, transform);
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
			new PropertySpec("Tank Height", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Tank"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}