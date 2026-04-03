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
	public class Hammer : ObjectDefinition
	{
		protected NJS_OBJECT object_hammer;
		protected NJS_TEXLIST texlist_hammer;

		public override void Init(ObjectData data, string name)
		{
			object_hammer = ObjectHelper.LoadModel("stg13_cityescape/models/HAMMER.sa2mdl");

			texlist_hammer = NJS_TEXLIST.Load("stg13_cityescape/tls/HAMMER.satex");
		}

		public override string Name { get { return "Moving Pillar"; } }

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			float oscInten;

			if ((item.Rotation.X & 0xFF00) != 0)
			{
				oscInten = item.Scale.Z;
			}
			else
			{
				oscInten = -item.Scale.Z;
			}

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.X + 1.0f));

				result.AddRange(
					object_hammer.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode),
					transform,
					ObjectHelper.GetTextures(new List<string> { "landtx13", "objtex_stg13" }, texlist_hammer, dev),
					ObjectHelper.GetMeshes(object_hammer),
					EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));

				if (item.Selected)
				{
					result.AddRange(object_hammer.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(object_hammer)));
				}
			}
			transform.Pop();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJTranslate(0.0f, oscInten, 0.0f);

				transform.NJRotateY(item.Rotation.Y);

				transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.X + 1.0f));

				result.AddRange(object_hammer.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(object_hammer)));
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

				transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.X + 1.0f));

				result = object_hammer.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(object_hammer));
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

				transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.X + 1.0f));

				result.Add(new ModelTransform(object_hammer, transform.Top));
			}
			transform.Pop();

			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);

			transform.NJScale((item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.X + 1.0f));

			return ObjectHelper.GetModelBounds(object_hammer, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);
			MatrixFunctions.Scale(ref matrix, (item.Scale.X + 1.0f), (item.Scale.Y + 1.0f), (item.Scale.X + 1.0f));

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Rest Time (in frames)", typeof(int), "Movement Stats", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Oscillation Speed", typeof(int), "Movement Stats", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Oscillation Intensity", typeof(float), "Movement Stats", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Oscillation Direction", typeof(HammerDirection), "Movement Stats", null, null,
			(o) => ((o.Rotation.X & 0x100) != 0) ? 0 : 1,
			(o, v) => {o.Rotation.X &= ~0x100; if ((byte)v == 0) o.Rotation.X |= 0x100;}),
			new PropertySpec("Width", typeof(float), "Size", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Height", typeof(float), "Size", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum HammerDirection
		{
			Up,
			Down
		}
	}
}