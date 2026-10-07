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

namespace SA2ObjectDefinitions.Common
{
	public class Pulley : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_OBJECT points;
		private Mesh[] meshpoints;
		private NJS_TEXLIST texarr;
		private Texture[] texs;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/GC/OBJECT_UDREEL.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			points = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			meshpoints = ObjectHelper.GetMeshes(points);
			texarr = NJS_TEXLIST.Load("object/tls/UDREEL.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			float restA = ((item.Scale.X + 1F) * -20F) - 20F;
			float restB = ((item.Scale.Y + 1F) * -20F) - 20F;
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restA, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result = HitResult.Min(result, points.CheckHit(Near, Far, Viewport, Projection, View, transform, meshpoints));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restB, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result = HitResult.Min(result, points.CheckHit(Near, Far, Viewport, Projection, View, transform, meshpoints));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			float restA = ((item.Scale.X + 1F) * -20F) - 20F;
			float restB = ((item.Scale.Y + 1F) * -20F) - 20F;
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restA, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result.AddRange(points.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshpoints, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(points.DrawModelTreeInvert(transform, meshpoints));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restB, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result.AddRange(points.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshpoints, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(points.DrawModelTreeInvert(transform, meshpoints));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			float restA = ((item.Scale.X + 1F) * -20F) - 20F;
			float restB = ((item.Scale.Y + 1F) * -20F) - 20F;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restA, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result.Add(new ModelTransform(points, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restB, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result.Add(new ModelTransform(points, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			float restA = ((item.Scale.X + 1F) * -20F) - 20F;
			float restB = ((item.Scale.Y + 1F) * -20F) - 20F;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			BoundingSphere pulleybounds =  ObjectHelper.GetModelBounds(model, transform);
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restA, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			BoundingSphere topbounds = ObjectHelper.GetModelBounds(points, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + restB, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y - 0x8000, 0);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			BoundingSphere bottombounds = ObjectHelper.GetModelBounds(points, transform);
			transform.Pop();
			BoundingSphere pointbounds = SAModel.Direct3D.Extensions.Merge(topbounds, bottombounds);
			return SAModel.Direct3D.Extensions.Merge(pointbounds, pulleybounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - 0x8000, 0);
			return matrix;
		}

		//public override void SetOrientation(SETItem item, Vertex direction)
		//{
		//	int x; int z; direction.GetRotation(out x, out z);
		//	item.Rotation.X = x + 0x4000;
		//	item.Rotation.Z = -z;
		//}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Retractable", typeof(bool), "Extended", "Determines if the pulley returns to its original position when the player lets go. An even X Rotation is false, odd X Rotation is true.", null, (o) => Convert.ToBoolean(o.Rotation.X & 0xF),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= Convert.ToByte((bool)v); }),
			new PropertySpec("Upper Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Lower Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Release Jump Power", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Release Jump Angle", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Pulley"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}