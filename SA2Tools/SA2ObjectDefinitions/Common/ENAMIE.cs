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
	public abstract class ENAMIEBase : ObjectDefinition
	{
		protected NJS_OBJECT[] namiemodels;
		protected Mesh[][] namiemeshes;
		protected NJS_OBJECT object_namie_bomb;
		protected Mesh[] mesh_namie_bomb;

		protected NJS_TEXLIST texlist_namie;
		protected Texture[] texs;

		protected Vector3[] BombPositions;
		protected int[] BombZAngles;

		public override void Init(ObjectData data, string name)
		{
			namiemodels = new NJS_OBJECT[4];
			namiemeshes = new Mesh[4][];
			BombPositions = new Vector3[9];
			BombZAngles = new int[9];
			namiemodels[0] = ObjectHelper.LoadModel("enemy/namie/E_NAMIE_3.sa2mdl");
			namiemodels[1] = ObjectHelper.LoadModel("enemy/namie/E_NAMIE_6.sa2mdl");
			namiemodels[2] = ObjectHelper.LoadModel("enemy/namie/E_NAMIE_9.sa2mdl");
			namiemodels[3] = ObjectHelper.LoadModel("enemy/namie/E_NAMIE_LASER.sa2mdl");
			object_namie_bomb = ObjectHelper.LoadModel("enemy/namie/E_NAMIE_BOMB.sa2mdl");

			for (int i = 0; i < 4; i++)
			{
				namiemeshes[i] = ObjectHelper.GetMeshes(namiemodels[i]);
			}
			mesh_namie_bomb = ObjectHelper.GetMeshes(object_namie_bomb);

			texlist_namie = NJS_TEXLIST.Load("enemy/namie/tls/NAMIE.satex");

			BombPositions[0] = new Vector3(-16.1F, -13.9F, 0F);
			BombPositions[1] = new Vector3(-16.1F,  6.95F, 12.04F);
			BombPositions[2] = new Vector3(-16.1F, 6.95F, -12.04F);
			BombPositions[3] = new Vector3(-16.1F, 13.9F, 0F);
			BombPositions[4] = new Vector3(-16.1F, -6.95F, 12.04F);
			BombPositions[5] = new Vector3(-16.1F, -6.95F, -12.04F);

			//Modified from the sources, since the bombs are at the correct position already
			//BombPositions[6] = new Vector3(-33.1F, -12.1F, 0F);
			//BombPositions[7] = new Vector3(-33.1F, 0.05F, 12.04F);
			//BombPositions[8] = new Vector3(-33.1F, -0.05F, -12.04F);
			BombPositions[6] = new Vector3(0F, -1.8F, -17.0F);
			BombPositions[7] = new Vector3(-1.8F, 0F, -17.0F);
			BombPositions[8] = new Vector3(1.8F, 0F, -17.0F);

			BombZAngles[0] = 0;
			BombZAngles[1] = 0xAAAB;
			BombZAngles[2] = 0x5555;
			BombZAngles[3] = BombZAngles[6] = 0x8000;
			BombZAngles[4] = 0x2AAB;
			BombZAngles[5] = 0xD555;
			BombZAngles[7] = 0x4000;
			BombZAngles[8] = 0xC000;

		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int hornetType = (int)item.Scale.X > 3 ? 0 : (int)item.Scale.X;
			int bombcount;
			bombcount = hornetType switch
			{
				1 => 6,
				2 => 9,
				_ => 3,
			};
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			result = HitResult.Min(result, namiemodels[hornetType].CheckHit(Near, Far, Viewport, Projection, View, transform, namiemeshes[hornetType]));
			for (int i = 0; i < bombcount; i++)
			{
				transform.Push();
				if (i >= 6)
					transform.NJTranslate(BombPositions[i]);
				transform.NJRotateZ(BombZAngles[i]);
				result = HitResult.Min(result, object_namie_bomb.CheckHit(Near, Far, Viewport, Projection, View, transform, mesh_namie_bomb));
				transform.Pop();
			}
			transform.Pop();
			
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("e_namitex");

			int hornetType = (int)item.Scale.X > 3 ? 0 : (int)item.Scale.X;
			int bombcount;
			bombcount = hornetType switch
			{
				1 => 6,
				2 => 9,
				_ => 3,
			};
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			result.AddRange(namiemodels[hornetType].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform,texs,namiemeshes[hornetType], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(namiemodels[hornetType].DrawModelTreeInvert(transform, namiemeshes[hornetType]));
			}

			for (int i = 0; i < bombcount; i++)
			{
				transform.Push();
				if (i >= 6)
					transform.NJTranslate(BombPositions[i]);
				transform.NJRotateZ(BombZAngles[i]);
				result.AddRange(object_namie_bomb.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, mesh_namie_bomb, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(object_namie_bomb.DrawModelTreeInvert(transform, mesh_namie_bomb));
				}
				transform.Pop();
			}

			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int hornetType = (int)item.Scale.X > 3 ? 0 : (int)item.Scale.X;
			int bombcount;
			bombcount = hornetType switch
			{
				1 => 6,
				2 => 9,
				_ => 3,
			};
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y + 0x4000);
				result.Add(new ModelTransform(namiemodels[hornetType], transform.Top));
				for (int i = 0; i < bombcount; i++)
				{
					transform.Push();
					if (i >= 6)
						transform.NJTranslate(BombPositions[i]);
					transform.NJRotateZ(BombZAngles[i]);
					result.Add(new ModelTransform(object_namie_bomb, transform.Top));
					transform.Pop();
				}
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int hornetType = (int)item.Scale.X > 3 ? 0 : (int)item.Scale.X;
			int bombcount;
			bombcount = hornetType switch
			{
				1 => 6,
				2 => 9,
				_ => 3,
			};
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			BoundingSphere hornetbounds = ObjectHelper.GetModelBounds(namiemodels[hornetType], transform);
			for (int i = 0; i < bombcount; i++)
			{
				transform.Push();
				if (i >= 6)
					transform.NJTranslate(BombPositions[i]);
				transform.NJRotateZ(BombZAngles[i]);
				BoundingSphere bombbounds = ObjectHelper.GetModelBounds(object_namie_bomb, transform);
				transform.Pop();
				hornetbounds = SAModel.Direct3D.Extensions.Merge(hornetbounds, bombbounds);

			}
			transform.Pop();
			return hornetbounds;
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
			
			new PropertySpec("Hornet Type", typeof(HornetType), "Extended", null, null, (o) => (HornetType)((int)o.Scale.X > 3 ? 0 : (int)o.Scale.X), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Wandering Distance/Laser Speed", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v > 0 ? (float)v : 60.0f),
			new PropertySpec("Bomb Duration/Laser Length", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Laser Firing Rate", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class ENAMIE : ENAMIEBase
	{
		public override string Name { get { return "GUN Hornet"; } }
	}
	public class ENAMIEN : ENAMIEBase
	{
		public override string Name { get { return "GUN Hornet (Near)"; } }
	}
	public class ENAMIEF : ENAMIEBase
	{
		public override string Name { get { return "GUN Hornet (Far)"; } }
	}

	public enum HornetType
	{
		ThreeBombs,
		SixBombs,
		NineBombs,
		Laser
	}
}