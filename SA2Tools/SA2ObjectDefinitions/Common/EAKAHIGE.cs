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
	public class EAKAHIGE : ObjectDefinition
	{
		private NJS_OBJECT object_akahige3;
		private Mesh[] mesh_akahige3;
		private NJS_OBJECT object_akahige6;
		private Mesh[] mesh_akahige6;
		private NJS_OBJECT object_akahige_bomb;
		private Mesh[] mesh_akahige_bomb;

		protected NJS_TEXLIST texlist_akahige;
		protected Texture[] texs;

		public override void Init(ObjectData data, string name)
		{
			object_akahige3 = ObjectHelper.LoadModel("enemy/akahige/E_AKAHIGE_3.sa2mdl");
			object_akahige6 = ObjectHelper.LoadModel("enemy/akahige/E_AKAHIGE_6.sa2mdl");
			object_akahige_bomb = ObjectHelper.LoadModel("enemy/akahige/E_AKAHIGE_BOMB.sa2mdl");

			mesh_akahige3 = ObjectHelper.GetMeshes(object_akahige3);
			mesh_akahige6 = ObjectHelper.GetMeshes(object_akahige6);
			mesh_akahige_bomb = ObjectHelper.GetMeshes(object_akahige_bomb);

			texlist_akahige = NJS_TEXLIST.Load("enemy/akahige/tls/AKAHIGE.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int phoenixType = Math.Max( (int)item.Scale.X, 0 );
			
			if ( phoenixType == 1 )
			{
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, 0);
				HitResult result = object_akahige6.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh_akahige6);
				transform.Pop();
				return result;
			}
			else {
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, 0);
				HitResult result = object_akahige3.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh_akahige3);
				transform.Pop();
				return result;
			}
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			texs = ObjectHelper.GetTextures("e_akahigetex", texlist_akahige, dev);

			int phoenixType = Math.Max((int)item.Scale.X, 0);
			int bombCount = 3;
			List<RenderInfo> result = new List<RenderInfo>();

			NJS_OBJECT pObject = object_akahige3;
			Mesh[] pMesh = mesh_akahige3;

			if ( phoenixType == 1 )
			{
				bombCount = 6;

				pObject = object_akahige6;
				pMesh = mesh_akahige6;
			}

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.AddRange(
					pObject.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode),
					transform,
					texs,
					pMesh,
					EditorOptions.IgnoreMaterialColors,
					EditorOptions.OverrideLighting));

				if (item.Selected)
				{
					result.AddRange(pObject.DrawModelTreeInvert(transform, pMesh));
				}

				for (int i = 0; i < bombCount; i++)
				{
					transform.Push();
					{
						transform.NJRotateObject(0, 0, ObjectHelper.DegToBAMS(120 / (bombCount / 3)) * i);
						result.AddRange(
							object_akahige_bomb.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode),
							transform,
							texs,
							mesh_akahige_bomb,
							EditorOptions.IgnoreMaterialColors,
							EditorOptions.OverrideLighting));
						if (item.Selected)
						{
							result.AddRange(object_akahige_bomb.DrawModelTreeInvert(transform, mesh_akahige_bomb));
						}
					}
					transform.Pop();
				}
			}
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int phoenixType = Math.Max( (int)item.Scale.X, 0 );
			if ( phoenixType == 1 )
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.Add(new ModelTransform(object_akahige6, transform.Top));
				transform.Pop();
				return result;
			}
			else
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				result.Add(new ModelTransform(object_akahige3, transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int phoenixType = Math.Max( (int)item.Scale.X, 0 );
			if ( phoenixType == 1 )
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(object_akahige6, transform);
			}
			else
			{
				MatrixStack transform = new MatrixStack();
				transform.NJTranslate(item.Position.ToVector3());
				transform.NJRotateObject(0, item.Rotation.Y + 0x4000, item.Rotation.Z);
				return ObjectHelper.GetModelBounds(object_akahige3, transform);
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y + 0x4000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			
			new PropertySpec(
				"Phoenix Type", 
				typeof(PhoenixType), 
				"Extended", 
				null, 
				PhoenixType.Phoenix_3, 
				(o) =>
				{
					int phoenixType = (int)o.Scale.X;
					if (phoenixType == 1)
						return PhoenixType.Phoenix_6;
					else
						return PhoenixType.Phoenix_3;
				}, 
				(o, v) => 
				{
					if ((PhoenixType)v == PhoenixType.Phoenix_6)
					{
						o.Scale.X = 1;
					}
					else
					{
						o.Scale.X = 0;
					}
				}),
			new PropertySpec("Wandering Distance", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 60.0f),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "GUN Phoenix"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum PhoenixType
		{
			Phoenix_3 = 0,
			Phoenix_6 = 1,
		}
	}
}