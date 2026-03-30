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

namespace SA2ObjectDefinitions.CityEscape
{
	public abstract class TreesBase : ObjectDefinition
	{
		protected NJS_OBJECT model;

		protected NJS_TEXLIST texlist;

		protected Texture[] texs;

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			NJS_OBJECT curModel = model;

			texs = ObjectHelper.GetTextures(new List<string> { "landtx13", "objtex_stg13" }, texlist, dev);

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				
				curModel.Children[0].Scale.X = 0;
				curModel.Children[0].Scale.Y = 0;
				curModel.Children[0].Scale.Z = 0;

				result.AddRange(
					curModel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, ObjectHelper.GetMeshes(curModel), EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting)
				);

				if (item.Selected)
				{
					result.AddRange(curModel.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(curModel)));
				}

				curModel.Children[0].Scale.X = 1;
				curModel.Children[0].Scale.Y = 1;
				curModel.Children[0].Scale.Z = 1;

				transform.NJRotateY(camera.Yaw - item.Rotation.Y);

				result.AddRange(
					curModel.Children[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, ObjectHelper.GetMeshes(curModel.Children[0]), EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting)
				);

				if (item.Selected)
				{
					result.AddRange(curModel.Children[0].DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(curModel.Children[0])));
				}
			}
			transform.Pop();

			return result;
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(model));
			}
			transform.Pop();

			return result;
		}
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				result.Add(new ModelTransform(model, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + ObjectHelper.DegToBAMS(90);
			item.Rotation.Z = -z;
		}

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class StreetTree : TreesBase
	{
		public override string Name { get { return "City Escape Street Tree"; } }
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg13_cityescape/models/GC/TREEST.sa2bmdl");
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/TREEST.satex");
		}
	}
	public class GroundTree : TreesBase
	{
		public override string Name { get { return "City Escape Ground Tree"; } }
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg13_cityescape/models/GC/TREESTNB.sa2bmdl");
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/TREESTNB.satex");
		}
	}
}
