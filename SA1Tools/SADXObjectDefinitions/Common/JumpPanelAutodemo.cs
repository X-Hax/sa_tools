using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using System;
using SAModel.SAEditorCommon;

namespace SADXObjectDefinitions.Common
{
	public class JumpPanelAutodemo : ObjectDefinition
	{
		private NJS_OBJECT model;
		private NJS_OBJECT numbermodel;
		private Mesh[] meshes;
		private Mesh[] numbermeshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/jumppanel_base.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			numbermodel = ObjectHelper.LoadModel("object/no_unite/jumppanel_numbers.nja.sa1mdl");
			numbermeshes = ObjectHelper.GetMeshes(numbermodel);
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
			((BasicAttach)numbermodel.Attach).Material[0].TextureID =
				62 + Math.Min(Math.Max((int)item.Scale.X, 0), 9);
			result.AddRange(numbermodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("CON_REGULAR"), numbermeshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
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

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override string Name { get { return "Jump Panel Beta"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Panel Number", typeof(int), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Next Panel", typeof(int), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}