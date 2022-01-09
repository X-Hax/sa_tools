using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public class Pier : ObjectDefinition
	{
		protected NJS_OBJECT[] models = new NJS_OBJECT[2];

		protected Mesh[][] meshes = new Mesh[2][];

		public override void Init(ObjectData data, string name)
		{
			models[0] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_pier_a.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_pier_b.nja.sa1mdl");
			meshes[0] = ObjectHelper.GetMeshes(models[0]);
			meshes[1] = ObjectHelper.GetMeshes(models[1]);
		}

		public override string Name { get { return "Pier"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int modelID = (int)item.Scale.Y & 1;
			transform.Push();
			transform.NJTranslate(item.Position);
			HitResult result = models[modelID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[modelID]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int modelID = (int)item.Scale.Y & 1;
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(models[modelID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes[modelID]));
			if (item.Selected)
				result.AddRange(models[modelID].DrawModelTreeInvert(transform, meshes[modelID]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int modelID = (int)item.Scale.Y & 1;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(models[modelID], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int modelID = (int)item.Scale.Y & 1;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(models[modelID], transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		private PropertySpec[] customProperties = new PropertySpec[]
		{
			new PropertySpec("Variant", typeof(PierVariants), "Extended", null, PierVariants.PierStraight, (o) => (PierVariants)Math.Min(Math.Max((int)o.Scale.Y & 1, 0), 1), (o, v) => o.Scale.Y = (int)o.Scale.Y - ((int)o.Scale.Y & 1) | (int)v),
			new PropertySpec("Debris Speed X", typeof(float), "Extended", null, 0, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Debris Speed Z", typeof(float), "Extended", null, 0, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}

	public enum PierVariants
	{
		PierStraight,
		PierCorner
	}
}