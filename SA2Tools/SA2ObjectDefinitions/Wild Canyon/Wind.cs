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

namespace SA2ObjectDefinitions.WildCanyon
{
	public class Wind : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_TEXLIST texarr;
		private Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("OBJECT/CylinderCollision.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);

			texarr = NJS_TEXLIST.Load("stg16_WildCanyon/tls/POLE.satex");

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.X + 10F) * 0.1F);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.Z + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.Z + 10F) * 0.1F);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.X + 10F) * 0.1F);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.Z + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.Z + 10F) * 0.1F);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.X + 10F) * 0.1F);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.Z + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.Z + 10F) * 0.1F);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.X + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.X + 10F) * 0.1F);
			BoundingSphere bounds = ObjectHelper.GetModelBounds(model, transform);
			transform.NJTranslate(item.Position);
			transform.NJScale((item.Scale.Z + 10F) * 0.1F, (item.Scale.Y + 10F) * 0.1F, (item.Scale.Z + 10F) * 0.1F);
			BoundingSphere innerbounds = ObjectHelper.GetModelBounds(model, transform);
			return SAModel.Direct3D.Extensions.Merge(bounds, innerbounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Spawn Rock Debris", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.X & 0xF) % 2 == 0),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= Convert.ToByte(!(bool)v); }),
			new PropertySpec("Outside Width", typeof(float), "Extended", "This determines the width of the wind tunnel that pulls the player inward.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Height", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Inside Width", typeof(float), "Extended", "This determines the width of the wind tunnel that pushes the player upward.", null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Wind Generator"; } }
	}
}