using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;

namespace SA2ObjectDefinitions.Common
{
	public class LevelUp : ObjectDefinition
	{
		private NJS_OBJECT ring;
		private Mesh[] meshesRing;
		private NJS_TEXLIST texarrRing;
		private Texture[] texsRing;
		
		private NJS_OBJECT grndglow;
		private Mesh[] meshesGrnd;
		private NJS_TEXLIST texarrGrnd;
		private Texture[] texsGrnd;
		
		private NJS_OBJECT glow;
		private Mesh[] meshesGlow;
		private NJS_TEXLIST texarrGlow;
		private Texture[] texsGlow;

		public override void Init(ObjectData data, string name)
		{
			ring = ObjectHelper.LoadModel("object/OBJECT_LEVUPDAI_RING.sa2mdl");
			meshesRing = ObjectHelper.GetMeshes(ring);
			texarrRing = NJS_TEXLIST.Load("object/tls/LEVUPDAI_RING.satex");

			grndglow = ObjectHelper.LoadModel("object/OBJECT_LEVUPDAI_GROUNDGLOW.sa2mdl");
			meshesGrnd = ObjectHelper.GetMeshes(grndglow);
			texarrGrnd = NJS_TEXLIST.Load("object/tls/LEVUPDAI_GROUNDGLOW.satex");
			
			glow = ObjectHelper.LoadModel("object/OBJECT_LEVUPDAI_GLOW.sa2mdl");
			meshesGlow = ObjectHelper.GetMeshes(glow);
			texarrGlow = NJS_TEXLIST.Load("object/tls/LEVUPDAI_GLOW.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			HitResult result = ring.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesRing);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texsRing == null)
				texsRing = ObjectHelper.GetTextures("objtex_common", texarrRing, dev);
			if (texsGrnd == null)
				texsGrnd = ObjectHelper.GetTextures("objtex_common", texarrGrnd, dev);
			if (texsGlow == null)
				texsGlow = ObjectHelper.GetTextures("objtex_common", texarrGlow, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.AddRange(ring.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsRing, meshesRing, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(grndglow.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsGrnd, meshesGrnd, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(glow.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsGlow, meshesGlow, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(ring.DrawModelTreeInvert(transform, meshesRing));
				result.AddRange(grndglow.DrawModelTreeInvert(transform, meshesGrnd));
				result.AddRange(glow.DrawModelTreeInvert(transform, meshesGlow));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.Add(new ModelTransform(ring, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(ring, transform);
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

		public override string Name { get { return "Level-Up Item Location"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}