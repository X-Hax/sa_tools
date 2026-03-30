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
using System.Diagnostics;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.CityEscape
{
	public class Signboard : ObjectDefinition
	{
		protected NJS_OBJECT object_signboard;
		protected NJS_TEXLIST texlist_signboard;
		
		public override void Init(ObjectData data, string name)
		{
			object_signboard = ObjectHelper.LoadModel("stg13_cityescape/models/SIGNBOARD.sa2mdl");

			texlist_signboard = NJS_TEXLIST.Load("stg13_cityescape/tls/SIGNBOARD.satex");
		}

		public override string Name { get { return "City Escape Signboard"; } }

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			int posterType = item.Rotation.X;

			NJS_OBJECT pObject = object_signboard;

			int itemID = Math.Min(5 + item.Rotation.X, 9);

			texlist_signboard.TextureNames[2] = texlist_signboard.TextureNames[itemID];

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result.AddRange(
					pObject.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode),
					transform,
					ObjectHelper.GetTextures(new List<string> { "landtx13", "objtex_stg13" }, texlist_signboard, dev),
					ObjectHelper.GetMeshes(pObject),
					EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));

				if (item.Selected)
				{
					result.AddRange(pObject.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(pObject)));
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

				result = object_signboard.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(object_signboard));
			}
			transform.Pop();

			return result;
		}
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int posterType = item.Rotation.X;

			NJS_OBJECT pObject = object_signboard;

			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result.Add(new ModelTransform(pObject, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(object_signboard, transform);
		}
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Poster Content", typeof(CESignboard), "Extended", null, null, (o) => (CESignboard)o.Rotation.X, (o, v) => o.Rotation.X = (int)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum CESignboard
		{
			Subburger,
			DIGITALCHOKE,
			SonicTeam,
			ChaosCola,
			SOAP
		}
	}
}
