﻿using SharpDX;
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
	public class Dynamite : ObjectDefinition
	{
		private NJS_OBJECT dynamite;
		private Mesh[] meshesDynamite;
		private NJS_TEXLIST texarrDynamite;
		private Texture[] texsDynamite;
		
		private NJS_OBJECT dynamitecase;
		private Mesh[] meshesCase;
		private NJS_TEXLIST texarrCase;
		private Texture[] texsCase;
		
		private NJS_OBJECT arrow;
		private Mesh[] meshesArrow;

		public override void Init(ObjectData data, string name)
		{
			dynamite = ObjectHelper.LoadModel("object/OBJECT_DYNAMITE.sa2mdl");
			meshesDynamite = ObjectHelper.GetMeshes(dynamite);
			texarrDynamite = NJS_TEXLIST.Load("object/tls/DYNAMITE.satex");

			dynamitecase = ObjectHelper.LoadModel("object/OBJECT_DYNAMITE_CASE.sa2mdl");
			meshesCase = ObjectHelper.GetMeshes(dynamitecase);
			texarrCase = NJS_TEXLIST.Load("object/tls/DYNAMITE_CASE.satex");
			
			arrow = ObjectHelper.LoadModel("object/OBJECT_DYNAMITE_ARROW.sa2mdl");
			meshesArrow = ObjectHelper.GetMeshes(arrow);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			HitResult result = dynamite.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesDynamite);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texsDynamite == null)
				texsDynamite = ObjectHelper.GetTextures("objtex_common", texarrDynamite, dev);
			if (texsCase == null)
				texsCase = ObjectHelper.GetTextures("objtex_common", texarrCase, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.AddRange(dynamite.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsDynamite, meshesDynamite, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(dynamitecase.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsCase, meshesCase, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(arrow.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("objtex_common"), meshesArrow, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(dynamite.DrawModelTreeInvert(transform, meshesDynamite));
				result.AddRange(dynamitecase.DrawModelTreeInvert(transform, meshesCase));
				result.AddRange(arrow.DrawModelTreeInvert(transform, meshesArrow));
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
			result.Add(new ModelTransform(dynamite, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(dynamite, transform);
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

		public override string Name { get { return "Dynamite Pack"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}