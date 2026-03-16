using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Common
{
	public class HintMonitor : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/hintbox_body.nja.sa1mdl");
			BasicAttach att = (BasicAttach)model.Attach;
			att.Material[16].UseAlpha = false;
			att.Material[16].EnvironmentMap = false;
			att.Material[16].IgnoreLighting = true;
			att.Material[16].TextureID = 28;
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_REGULAR"), meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		public override string Name { get { return "Hint Monitor"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Camera Distance", typeof(float), "Extended", "Camera zoom-out in units (default is 80).", null, (o) => (float)Math.Min(Math.Max((float)o.Scale.X, 0.1), 999), (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Camera Height", typeof(float), "Extended", "Camera height in units.", null, (o) => (float)Math.Min(Math.Max((float)o.Scale.Y, -999), 999), (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Hint ID", typeof(int), "Extended", "NPC ID in SADXTweaker2's NPC Message Editor. It's 0-based here, so the ID in SADXTweaker2 will be this ID + 1.", null, (o) => (int)Math.Min(Math.Max((int)o.Scale.Z, 0), 999), (o, v) => o.Scale.Z = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}