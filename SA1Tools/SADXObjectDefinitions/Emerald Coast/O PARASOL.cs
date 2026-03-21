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
using SAModel.SAEditorCommon;

namespace SADXObjectDefinitions.EmeraldCoast
{
	public class Parasol : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private Mesh[][] meshes;

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[5];
			meshes = new Mesh[5][];
			models[0] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_parasol01.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_parasol02.nja.sa1mdl");
			models[2] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_chair_a.nja.sa1mdl");
			models[3] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_chair_b.nja.sa1mdl");
			models[4] = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_table.nja.sa1mdl");
			for (int i = 0; i < 5; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override string Name { get { return "Breakable Parasol, Chair, or Table"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int modelId = Math.Abs((int)item.Scale.X) % 5;
			transform.Push();
			transform.NJTranslate(item.Position);
			HitResult result = models[modelId].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[modelId]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int modelId = Math.Abs((int)item.Scale.X) % 5;
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(models[modelId].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_BEACH"), meshes[modelId], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[modelId].DrawModelTreeInvert(transform, meshes[modelId]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int modelId = Math.Abs((int)item.Scale.X) % 5;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(models[modelId], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int modelId = Math.Abs((int)item.Scale.X) % 5;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(models[modelId], transform);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Variant", typeof(ParasolVariants), "Extended", "Variation of the breakable parasol or chair.", ParasolVariants.Parasol_A, (o) => (ParasolVariants)(Math.Abs((int)o.Scale.X) % 5), (o, v) => o.Scale.X = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}
	}

	public enum ParasolVariants
	{
		Parasol_A,
		Parasol_B,
		Chair_A,
		Chair_B,
		Table
	}
}