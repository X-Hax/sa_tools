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

namespace SADXObjectDefinitions.WindyValleyAutodemo
{
	public class Bridge : ObjectDefinition
	{
		protected NJS_OBJECT modelA;
		protected Mesh[] meshesA;
		protected NJS_OBJECT modelB;
		protected Mesh[] meshesB;
		protected float v5;

		public override void Init(ObjectData data, string name)
		{
			modelA = ObjectHelper.LoadModel("stg02_windy/common/models/col_bridgeshort.sa1mdl");
			meshesA = ObjectHelper.GetMeshes(modelA);
			modelB = ObjectHelper.LoadModel("stg02_windy/common/models/col_bridgelong.sa1mdl");
			meshesB = ObjectHelper.GetMeshes(modelB);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			v5 = Math.Min(Math.Max((int)item.Scale.Z, -1), 0);
			if (v5 < 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result = HitResult.Min(result, modelB.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesB));
				transform.Pop();
				return result;
			}
			if (v5 >= 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result = HitResult.Min(result, modelA.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesA));
				transform.Pop();
				return result;
			}
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			v5 = Math.Min(Math.Max((int)item.Scale.Z, -1), 0);
			if (v5 < 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.AddRange(modelB.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesB));
				if (item.Selected)
					result.AddRange(modelB.DrawModelTreeInvert(transform, meshesB));
				transform.Pop();
				return result;
			}
			if (v5 >= 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.AddRange(modelA.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshesA));
				if (item.Selected)
					result.AddRange(modelA.DrawModelTreeInvert(transform, meshesA));
				transform.Pop();
				return result;
			}
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			v5 = Math.Min(Math.Max((int)item.Scale.Z, -1), 0);
			if (v5 < 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(modelB, transform.Top));
				transform.Pop();
				return result;
			}
			if (v5 >= 0)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(modelA, transform.Top));
				transform.Pop();
				return result;
			}
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			v5 = Math.Min(Math.Max((int)item.Scale.Z, -1), 0);
			if ( v5 < 0)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				return ObjectHelper.GetModelBounds(modelB, transform);
			}
			if (v5 >= 0)
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				return ObjectHelper.GetModelBounds(modelA, transform);
			}
			return ObjectHelper.GetModelBounds(null, transform);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Bridge Size", typeof(Bridges), "Extended", null, null, (o) => (Bridges)Math.Min(Math.Max((int)o.Scale.Z, -1), 0), (o, v) => o.Scale.Z = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public override string Name { get { return "Bridge"; } }
	}

	public enum Bridges
	{
		Short = 0,
		Long = -1
	}
}