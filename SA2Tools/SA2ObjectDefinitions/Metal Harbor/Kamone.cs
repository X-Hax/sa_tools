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
using System.Net;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.MetalHarbor
{
	public class Kamone : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_OBJECT point;
		private Mesh[] pointmesh;
		private NJS_TEXLIST texarr;
		private Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/Seagull.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			point = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			pointmesh = ObjectHelper.GetMeshes(point);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/Seagull.satex");
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
			transform.NJScale(item.Scale.X / 10F, item.Scale.Y / 10F, item.Scale.Z / 10F);
			result = HitResult.Min(result, point.CheckHit(Near, Far, Viewport, Projection, View, transform, pointmesh));
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
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.NJScale(item.Scale.X / 10F, item.Scale.Y / 10F, item.Scale.Z / 10F);
			result.AddRange(point.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, pointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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
			transform.NJScale(item.Scale.X / 10F, item.Scale.Y / 10F, item.Scale.Z / 10F);
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
			transform.NJScale(item.Scale.X / 10F, item.Scale.Y / 10F, item.Scale.Z / 10F);
			BoundingSphere rangebounds = ObjectHelper.GetModelBounds(point, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(mainbounds, rangebounds);
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
			new PropertySpec("Flight Range X", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Flight Range Y", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Flight Range Z", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Seagull Count", typeof(int), "Extended", "This value caps at 20.", null, (o) => Math.Min(o.Rotation.X, 20), (o, v) => o.Rotation.X = Math.Min((int)v, 20)),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Seagull Flock"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}