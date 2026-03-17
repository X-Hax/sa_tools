using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.TwinklePark
{
	public class EnemyCart : ObjectDefinition
	{
		private NJS_OBJECT[] models;
		private NJS_OBJECT saru_model;
		private Mesh[][] meshes;
		private Mesh[] saru_meshes;
		
		private Dictionary<CartColor, int> cartTextureIDs = new()
		{
			{  CartColor.Black, 68 },
			{  CartColor.Blue, 70 },
			{  CartColor.Green, 80 },
			{  CartColor.LightBlue, 85 },
			{  CartColor.Orange, 90 },
			{  CartColor.Pink, 92 },
			{  CartColor.Red, 93 },
		};

		enum CartType
		{
			RegularEnemy,
			Big,
			Gamma
		}

		public enum CartColor
		{
			Black,
			Blue,
			Green,
			LightBlue,
			Orange,
			Pink,
			Red
		}

		float[] offsets = { 0.0f, 8.2f, 13.5f };

		public override void Init(ObjectData data, string name)
		{
			models = new NJS_OBJECT[3];
			meshes = new Mesh[3][];
			models[0] = ObjectHelper.LoadModel("shareobj/common/models/sarucart_sarucart.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("shareobj/common/models/b_cart_cart.nja.sa1mdl");
			models[2] = ObjectHelper.LoadModel("shareobj/common/models/e_cart_cart.nja.sa1mdl");
			saru_model = ObjectHelper.LoadModel("shareobj/common/models/sarucart_saru_body.nja.sa1mdl");
			for (int i = 0; i < 3; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			saru_meshes = ObjectHelper.GetMeshes(saru_model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int type = GetCartType(item);
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + offsets[type], item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			HitResult result = models[type].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[type]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int type = GetCartType(item);
			CartColor color = (CartColor)Math.Max(0, Math.Min(6, (int)item.Scale.X));
			bool hasSaru = item.Scale.Y != 1.0f;
			((BasicAttach)models[type].Children[0].Attach).Material[0].TextureID = cartTextureIDs[color];
			((BasicAttach)models[type].Children[0].Children[0].Sibling.Attach).Material[1].TextureID = cartTextureIDs[color];
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + offsets[type], item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			result.AddRange(models[type].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_SHAREOBJ"), meshes[type], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(models[type].DrawModelTreeInvert(transform, meshes[type]));
			if (hasSaru)
			{
				transform.Push();
				if (type > 0)
					transform.NJTranslate(0, -5.0f, 0);
				result.AddRange(saru_model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_SHAREOBJ"), saru_meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(saru_model.DrawModelTreeInvert(transform, saru_meshes));
				transform.Pop();
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int type = GetCartType(item);
			CartColor color = (CartColor)Math.Max(0, Math.Min(6, (int)item.Scale.X));
			bool hasSaru = item.Scale.Y != 1.0f;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + offsets[type], item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			result.Add(new ModelTransform(models[type], transform.Top));
			if (hasSaru)
			{
				transform.Push();
				if (type > 0)
					transform.NJTranslate(0, -5.0f, 0);
				result.Add(new ModelTransform(saru_model, transform.Top));
				transform.Pop();
			}
			transform.Pop();
			return result;
		}

		private int GetCartType(SETItem item)
		{
			int type = (int)item.Scale.Z;
			if (type > 2 || type < 0)
				type = 0;
			return type;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int type = GetCartType(item);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, item.Position.Y + offsets[type], item.Position.Z);
			transform.NJRotateObject(item.Rotation);
			return ObjectHelper.GetModelBounds(models[type], transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);

			return matrix;
		}

		public static object GetCartEmpty(SETItem item)
		{
			return (bool)(item.Scale.Y == 1.0f);
		}

		public static void SetCartEmpty(SETItem item, object value)
		{
			if ((bool)value == true)
				item.Scale.Y = 1.0f;
			else
				item.Scale.Y = 20.0f;
		}

		public static object GetCartVariant(SETItem item)
		{
			return item.Scale.Z switch
			{
				2 => CartType.Gamma,
				1 => CartType.Big,
				_ => (object)CartType.RegularEnemy,
			};
		}

		public static void SetCartVariant(SETItem item, object value)
		{
			switch ((CartType)value)
			{
				case CartType.Big:
					item.Scale.Z = 1.0f;
					return;
				case CartType.Gamma:
					item.Scale.Z = 2.0f;
					return;
				case CartType.RegularEnemy:
				default:
					item.Scale.Z = 0.0f;
					return;
			}
		}

		public override string Name { get { return "Enemy Cart"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Cart Color", typeof(CartColor), "Extended", null, null, (o) => (CartColor)Math.Min(Math.Max((int)o.Scale.X, 0), 6), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Empty", typeof(bool), "Extended", "Whether the cart has an enemy riding it or not.", false, GetCartEmpty, SetCartEmpty),
			new PropertySpec("Model Variant", typeof(CartType), "Extended", null, null, GetCartVariant, SetCartVariant)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}