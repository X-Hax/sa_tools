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

namespace SA2ObjectDefinitions.CityEscape
{
	public abstract class TreeBase : ObjectDefinition
	{
		protected NJS_OBJECT modelSidewalk;
		protected Mesh[] meshesSidewalk;
		protected NJS_OBJECT modelNormal;
		protected Mesh[] meshesNormal;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			modelSidewalk = ObjectHelper.LoadModel("stg13_cityescape/models/GC/TREEST.sa2bmdl");
			meshesSidewalk = ObjectHelper.GetMeshes(modelSidewalk);
			modelNormal = ObjectHelper.LoadModel("stg13_cityescape/models/GC/TREESTNB.sa2bmdl");
			meshesNormal = ObjectHelper.GetMeshes(modelNormal);
			texarr = NJS_TEXLIST.Load("stg13_cityescape/tls/TREEST.satex");
			texpacks.Add("landtx13");
			texpacks.Add("objtex_stg13");
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

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
		public class TreeSidewalk : TreeBase
		{
			public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJTranslate(0, 1.1f, 0);
				transform.NJRotateZYX(0, item.Rotation.Y, 0);
				HitResult result = modelSidewalk.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesSidewalk);
				transform.Pop();
				return result;
			}

			public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
			{
				var camerarot = camera.Yaw - item.Rotation.Y;
				List<RenderInfo> result = new List<RenderInfo>();
				if (texs == null)
					texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJTranslate(0, 1.1f, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.AddRange(modelSidewalk.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshesSidewalk[0], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(modelSidewalk.DrawModelInvert(transform, meshesSidewalk[0], true));
				transform.NJTranslate(modelSidewalk.Children[0].Position);
				if (camerarot != 0)
					transform.NJRotateY(camerarot);
				result.AddRange(modelSidewalk.Children[0].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshesSidewalk[1], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(modelSidewalk.Children[0].DrawModelInvert(transform, meshesSidewalk[1], true));
				transform.Pop();
				return result;
			}

			public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(modelSidewalk, transform.Top));
				transform.Pop();
				return result;
			}

			public override BoundingSphere GetBounds(SETItem item)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateZYX(0, item.Rotation.Y, 0);
				return ObjectHelper.GetModelBounds(modelSidewalk, transform);
			}
			public override string Name { get { return "Tree (Sidewalk)"; } }
		}
		public class TreeNormal : TreeBase
		{
			public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJTranslate(0, 1.1f, 0);
				transform.NJRotateZYX(0, item.Rotation.Y, 0);
				HitResult result = modelNormal.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesNormal);
				transform.Pop();
				return result;
			}

			public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
			{
				var camerarot = camera.Yaw - item.Rotation.Y;
				List<RenderInfo> result = new List<RenderInfo>();
				if (texs == null)
					texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJTranslate(0, 1.1f, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				result.AddRange(modelNormal.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshesNormal[0], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(modelNormal.DrawModelInvert(transform, meshesNormal[0], true));
				transform.NJTranslate(modelNormal.Children[0].Position);
				if (camerarot != 0)
					transform.NJRotateY(camerarot);
				result.AddRange(modelNormal.Children[0].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshesNormal[1], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(modelNormal.Children[0].DrawModelInvert(transform, meshesNormal[1], true));
				transform.Pop();
				return result;
			}

			public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateZYX(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(modelNormal, transform.Top));
				transform.Pop();
				return result;
			}

			public override BoundingSphere GetBounds(SETItem item)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateZYX(0, item.Rotation.Y, 0);
				return ObjectHelper.GetModelBounds(modelNormal, transform);
			}
			public override string Name { get { return "Tree (Flat Surface)"; } }
		}

}