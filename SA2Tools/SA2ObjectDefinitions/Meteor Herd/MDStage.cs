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

namespace SA2ObjectDefinitions.Common
{
	public class MeteorHerdStage : ObjectDefinition
	{
		protected NJS_OBJECT object_stage_a;
		protected Mesh[] mesh_stage_a;
		protected NJS_OBJECT object_stage_b;
		protected Mesh[] mesh_stage_b;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		public override void Init(ObjectData data, string name)
		{
			object_stage_a = ObjectHelper.LoadModel("stg32_MeteorHerd/models/GC/STAGE_A.sa2bmdl");
			object_stage_b = ObjectHelper.LoadModel("stg32_MeteorHerd/models/GC/STAGE_B.sa2bmdl");

			mesh_stage_a = ObjectHelper.GetMeshes(object_stage_a);
			mesh_stage_b = ObjectHelper.GetMeshes(object_stage_b);

			texarr = NJS_TEXLIST.Load("stg32_MeteorHerd/tls/STAGE_A.satex");
		}

		public override string Name { get { return "Meteor Herd Platform"; } }
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - ObjectHelper.DegToBAMS(180), 0);
			HitResult result = object_stage_a.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh_stage_a);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - ObjectHelper.DegToBAMS(180), 0);

			NJS_TEXLIST tls = texarr;

			NJS_OBJECT obj = object_stage_a;
			Mesh[] mesh = mesh_stage_a;
			if ((int)item.Scale.X == 1)
			{
				obj = object_stage_b;
				mesh = mesh_stage_b;
			}

			if (texs == null)
			{
				List<string> textures = ["landtx32", "objtex_stg32"];

				texs = ObjectHelper.GetTexturesMultiSource(textures, tls, dev);
			}

			result.AddRange(obj.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, mesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(obj.DrawModelTreeInvert(transform, mesh));
			}

			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y - ObjectHelper.DegToBAMS(180), 0);
			result.Add(new ModelTransform(object_stage_a, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y - ObjectHelper.DegToBAMS(180), 0);
			return ObjectHelper.GetModelBounds(object_stage_a, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - ObjectHelper.DegToBAMS(180), 0);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}