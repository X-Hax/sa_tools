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

namespace SA2ObjectDefinitions.Common
{
	public class EGOLD : ObjectDefinition
	{
		private NJS_OBJECT object_e_gold;

		public override void Init(ObjectData data, string name)
		{
			object_e_gold = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_ESCAPER.sa2mdl");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);

				result = object_e_gold.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(object_e_gold));
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
				transform.NJRotateY(item.Rotation.Y);

				result.AddRange(
					object_e_gold.DrawModelTree(
						dev.GetRenderState<FillMode>(RenderState.FillMode), 
						transform, 
						ObjectHelper.GetTextures("e_goldtex"), 
						ObjectHelper.GetMeshes(object_e_gold), 
						EditorOptions.IgnoreMaterialColors, 
						EditorOptions.OverrideLighting
					)
				);

				if (item.Selected)
				{
					result.AddRange(object_e_gold.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(object_e_gold)));
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
				transform.NJRotateY(item.Rotation.Y);
			
				result.Add(new ModelTransform(object_e_gold, transform.Top));
			}
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();

			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);

			return ObjectHelper.GetModelBounds(object_e_gold, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y, 0);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {

			new PropertySpec("Time (in frames) before disappearing", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Oscillation Amount", typeof(int), "Extended", null, 1, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (int)v > 0 ? (int)v : 999999),
			new PropertySpec("Oscillation Speed", typeof(int), "Extended", null, 1, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v > 0 ? (int)v : 999999),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 10.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "GUN Gold Beetle"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
}