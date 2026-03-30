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
using SplitTools;
using SAModel.SAEditorCommon;

namespace SA2ObjectDefinitions.CityEscape
{
	public abstract class CityEscapePosterBase : ObjectDefinition
	{
		protected NJS_OBJECT object_poster;

		protected NJS_TEXLIST texlist;
		public override string Name { get { return "City Escape Poster"; } }
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateXYZ(item.Rotation);

				transform.NJScale(item.Scale);

				result = object_poster.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(object_poster));
			}
			transform.Pop();

			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateXYZ(item.Rotation);

				transform.NJScale(item.Scale);

				result.AddRange(
					object_poster.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode),
					transform,
					ObjectHelper.GetTexturesMultiSource(new List<string>(["landtx13", "objtex_stg13"]), texlist, dev),
					ObjectHelper.GetMeshes(object_poster),
					EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));

				if (item.Selected)
				{
					result.AddRange(object_poster.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(object_poster)));
				}
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
				transform.NJRotateXYZ(item.Rotation);

				result.Add(new ModelTransform(object_poster, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateXYZ(item.Rotation);
			return ObjectHelper.GetModelBounds(object_poster, transform);
		}
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateXYZ(ref matrix, item.Rotation);

			return matrix;
		}
	}

	public class Poster : CityEscapePosterBase
	{
		public override void Init(ObjectData data, string name)
		{
			object_poster = ObjectHelper.GetFlatSquareModel();
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/POSTER.satex");
		}
	}

	public class Poster3 : CityEscapePosterBase
	{
		public override void Init(ObjectData data, string name)
		{
			object_poster = ObjectHelper.GetFlatSquareModel();
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/POSTER3.satex");
		}
	}
}
