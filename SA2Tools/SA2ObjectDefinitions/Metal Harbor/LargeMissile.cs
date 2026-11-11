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
using SAModel.SAEditorCommon.UI;

namespace SA2ObjectDefinitions.MetalHarbor
{
	public class LargeMissile : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		protected NJS_OBJECT point;
		protected Mesh[] pointmesh;
		private NJS_TEXLIST texarr;
		private Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/LargeMissile.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			point = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointmesh = ObjectHelper.GetMeshes(point);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/Missile.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
			transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F);
			result = HitResult.Min(result, point.CheckHit(Near, Far, Viewport, Projection, View, transform, pointmesh));
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
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F);
			result.AddRange(point.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, pointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(point.DrawModelTreeInvert(transform, pointmesh));
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
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F);
			result.Add(new ModelTransform(point, transform.Top));
			transform.Pop();
			return result;
		}
		
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere mainbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F, (item.Scale.X + 100F) / 10F);
			BoundingSphere triggerbounds = ObjectHelper.GetModelBounds(point, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(mainbounds, triggerbounds);
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
			new PropertySpec("Countdown Trigger Scale", typeof(float), "Extended", "100 is added to this value in the game's code.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Large Missile"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}