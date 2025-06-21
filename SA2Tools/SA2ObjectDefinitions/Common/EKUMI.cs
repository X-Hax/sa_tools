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
	public class EKUMI : ObjectDefinition
	{
		private NJS_OBJECT model1;
		private Mesh[] mesh1;
		private NJS_OBJECT model2;
		private Mesh[] mesh2;
		private NJS_OBJECT model3;
		private Mesh[] mesh3;
		private NJS_OBJECT model4;
		private Mesh[] mesh4;
		private NJS_OBJECT model5;
		private Mesh[] mesh5;

		public override void Init(ObjectData data, string name)
		{
			model1 = ObjectHelper.LoadModel("enemy/kumi/E_KUMI.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_ELEC_DISCHARGE.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			model3 = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_GUNNER.sa2mdl");
			mesh3 = ObjectHelper.GetMeshes(model3);
			model4 = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_BUMPER.sa2mdl");
			mesh4 = ObjectHelper.GetMeshes(model4);
			model5 = ObjectHelper.LoadModel("enemy/kumi/E_KUMI_BOMBER.sa2mdl");
			mesh5 = ObjectHelper.GetMeshes(model5);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);
			if (beetleID == 4)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				HitResult result = model3.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh3);
				transform.Pop();
				return result;
			}
			else if (beetleID == 6)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				HitResult result = model4.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh4);
				transform.Pop();
				return result;
			}
			else if (beetleID == 7)
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				HitResult result = model5.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh5);
				transform.Pop();
				return result;
			}
			else
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				HitResult result = model1.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh1);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);
			if (beetleID == 2 || beetleID == 3)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_kumitex"), mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_e_kumitex"), mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model1.DrawModelTreeInvert(transform, mesh1));
					result.AddRange(model2.DrawModelTreeInvert(transform, mesh2));
				}
				transform.Pop();
				return result;
			}
			else if (beetleID == 4)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_g_kumitex"), mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model3.DrawModelTreeInvert(transform, mesh3));
				}
				transform.Pop();
				return result;
			}
			else if (beetleID == 6)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.AddRange(model4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_s_kumitex"), mesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model4.DrawModelTreeInvert(transform, mesh4));
				}
				transform.Pop();
				return result;
			}
			else if (beetleID == 7)
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.AddRange(model5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_b_kumitex"), mesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model5.DrawModelTreeInvert(transform, mesh5));
				}
				transform.Pop();
				return result;
			}
			else
			{
				List<RenderInfo> result = new List<RenderInfo>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_kumitex"), mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(model1.DrawModelTreeInvert(transform, mesh1));
				}
				transform.Pop();
				return result;
			}
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);
			if (beetleID == 4)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(model3, transform.Top));
				transform.Pop();
				return result;
			}
			else if (beetleID == 6)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(model4, transform.Top));
				transform.Pop();
				return result;
			}
			else if (beetleID == 7)
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(model5, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(model1, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int beetleID = Math.Max((int)item.Scale.X, 0);
			if (beetleID == 4)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				return ObjectHelper.GetModelBounds(model3, transform);
			}
			else if (beetleID == 6)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				return ObjectHelper.GetModelBounds(model4, transform);
			}
			else if (beetleID == 7)
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				return ObjectHelper.GetModelBounds(model5, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				return ObjectHelper.GetModelBounds(model1, transform);
			}
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
			new PropertySpec("Speed", typeof(float), "Extended", null, 1.0f, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v > 0 ? (int)v : 99),
			new PropertySpec("Emerald/Item Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xFF,
			(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Beetle Type", typeof(BeetleType), "Extended", null, null, (o) => (BeetleType)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Bullet Speed Electric time", typeof(float), "Extended", null, 1.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 999.0f),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 10.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "GUN Beetle"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum BeetleType
		{
			Normal,
			Moving,
			Electric,
			ElectricAndMoving,
			Gunned,
			Appearing,
			Spring,
			Bombing,
		}
	}
}