using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SAModel.SAEditorCommon;

namespace SA2ObjectDefinitions.CityEscape
{
	public class IronBar : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_OBJECT endpoint;
		protected Mesh[] endpointmesh;
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg13_cityescape/models/GC/IRONBAR_JUMP.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);

			endpoint = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			endpointmesh = ObjectHelper.GetMeshes(endpoint);
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
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(10F, 10F, 10F);
			result = HitResult.Min(result, endpoint.CheckHit(Near, Far, Viewport, Projection, View, transform, endpointmesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0) 
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(10F, 10F, 10F);
			result.AddRange(endpoint.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, endpointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(endpoint.DrawModelTreeInvert(transform, endpointmesh));
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
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(10F, 10F, 10F);
			result.Add(new ModelTransform(endpoint, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere barbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(10F, 10F, 10F);
			BoundingSphere pointbounds = ObjectHelper.GetModelBounds(endpoint, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(barbounds, pointbounds);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Collision Size", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Bar Collision Length", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Jump X Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Jump Y Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Jump Z Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}
		public override string Name { get { return "Horizontal Bar Jump Data"; } }
	}

	public enum SwingDirection
	{
		Forward,
		Backward,
	}
}