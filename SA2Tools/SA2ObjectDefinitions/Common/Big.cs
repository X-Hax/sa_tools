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
	public class Big : ObjectDefinition
	{
		private NJS_OBJECT object_big;
		private NJS_OBJECT object_big_lay;

		private Mesh[] meshes;

		private NJS_TEXLIST texlist_big;

		private Texture[] texs;

		private NJS_MOTION motion_big_normal;
		private NJS_MOTION motion_big_shake;
		private NJS_MOTION motion_big_clap;
		private NJS_MOTION motion_big_sit;
		private NJS_MOTION motion_big_rod;
		private NJS_MOTION motion_big_bellyscratch;
		private NJS_MOTION motion_big_lay;
		private NJS_MOTION motion_big_cheer;
		private NJS_MOTION motion_big_wait;
		private NJS_MOTION motion_big_hipsway;
		private NJS_MOTION motion_big_wave;
		private NJS_MOTION motion_big_chestscratch;
		private NJS_MOTION motion_big_buttscratch;
		private NJS_MOTION motion_big_fan;
		private NJS_MOTION motion_big_running;
		private NJS_MOTION motion_big_runningfish;
		private NJS_MOTION motion_big_lookup;
		private NJS_MOTION motion_big_lookdown;
		private NJS_MOTION motion_big_layback;
		private NJS_MOTION motion_big_sitlookleft;
		private NJS_MOTION motion_big_holdfroggy;
		private NJS_MOTION motion_big_holdrod;

		public override void Init(ObjectData data, string name)
		{
			object_big = ObjectHelper.LoadModel("enemy/big/E_BIG_THE_CAT.sa2mdl");
			object_big_lay = ObjectHelper.LoadModel("enemy/big/E_LAYING_BIG_THE_CAT.sa2mdl");
			meshes = ObjectHelper.GetMeshes(object_big);
			texlist_big = NJS_TEXLIST.Load("enemy/big/tls/BIG.satex");

			motion_big_normal = NJS_MOTION.Load("enemy/big/Big1.saanim");
			motion_big_shake = NJS_MOTION.Load("enemy/big/Big2.saanim");
			motion_big_clap = NJS_MOTION.Load("enemy/big/Big3.saanim");
			motion_big_sit = NJS_MOTION.Load("enemy/big/Big4.saanim");
			motion_big_rod = NJS_MOTION.Load("enemy/big/Big5.saanim");
			motion_big_bellyscratch = NJS_MOTION.Load("enemy/big/Big6.saanim");
			motion_big_lay = NJS_MOTION.Load("enemy/big/Big7.saanim");
			motion_big_cheer = NJS_MOTION.Load("enemy/big/Big8.saanim");
			motion_big_wait = NJS_MOTION.Load("enemy/big/Big9.saanim");
			motion_big_hipsway = NJS_MOTION.Load("enemy/big/Big10.saanim");
			motion_big_wave = NJS_MOTION.Load("enemy/big/Big11.saanim");
			motion_big_chestscratch = NJS_MOTION.Load("enemy/big/Big12.saanim");
			motion_big_buttscratch = NJS_MOTION.Load("enemy/big/Big13.saanim");
			motion_big_fan = NJS_MOTION.Load("enemy/big/Big14.saanim");
			motion_big_running = NJS_MOTION.Load("enemy/big/Big15.saanim");
			motion_big_runningfish = NJS_MOTION.Load("enemy/big/Big16.saanim");
			motion_big_lookup = NJS_MOTION.Load("enemy/big/Big17.saanim");
			motion_big_lookdown = NJS_MOTION.Load("enemy/big/Big18.saanim");
			motion_big_layback = NJS_MOTION.Load("enemy/big/Big19.saanim");
			motion_big_sitlookleft = NJS_MOTION.Load("enemy/big/Big20.saanim");
			motion_big_holdfroggy = NJS_MOTION.Load("enemy/big/Big21.saanim");
			motion_big_holdrod = NJS_MOTION.Load("enemy/big/Big22.saanim");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
				result = object_big.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			}
			transform.Pop();

			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int bigAnim = Math.Min((int)item.Scale.X, 23);

			if (texs == null)
				texs = ObjectHelper.GetTextures("e_bigtex", texlist_big, dev);

			NJS_OBJECT pObject = object_big;

			NJS_MOTION pMotion;

			switch (bigAnim)
			{
				case 1:
					pMotion = motion_big_shake;
					break;
				case 2:
					pMotion = motion_big_clap;
					break;
				case 3:
				case 19:
					pMotion = motion_big_sit;
					break;
				case 4:
					pMotion = motion_big_rod;
					break;
				case 5:
					pMotion = motion_big_bellyscratch;
					break;
				case 6:
				case 23:
					pMotion = motion_big_lay;
					break;
				case 7:
					pMotion = motion_big_cheer;
					break;
				case 8:
					pMotion = motion_big_wait;
					break;
				case 9:
					pMotion = motion_big_hipsway;
					break;
				case 10:
					pMotion = motion_big_wave;
					break;
				case 11:
					pMotion = motion_big_chestscratch;
					break;
				case 12:
					pMotion = motion_big_buttscratch;
					break;
				case 13:
					pMotion = motion_big_fan;
					break;
				case 14:
					pMotion = motion_big_running;
					break;
				case 15:
					pMotion = motion_big_runningfish;
					break;
				case 16:
					pMotion = motion_big_lookup;
					break;
				case 17:
					pMotion = motion_big_lookdown;
					break;
				case 18:
					pMotion = motion_big_layback;
					break;
				case 20:
					pMotion = motion_big_sitlookleft;
					break;
				case 21:
					pMotion = motion_big_holdfroggy;
					break;
				case 22:
					pMotion = motion_big_holdrod;
					break;
				case 0:
				default:
					pMotion = motion_big_normal;
					break;
			}

			Mesh[] pMesh = ObjectHelper.GetMeshes(pObject);

			List<RenderInfo> result = new List<RenderInfo>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
				result.AddRange(object_big.DrawModelTreeAnimated(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, pMesh, pMotion, 0.0f, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
					result.AddRange(object_big.DrawModelTreeAnimatedInvert(transform, pMesh, pMotion, 0.0f));
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
				transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
				result.Add(new ModelTransform(object_big, transform.Top));
			}
			transform.Pop();
			
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();

			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);

			return ObjectHelper.GetModelBounds(object_big, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + ObjectHelper.DegToBAMS(90);
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animation", typeof(BigAnimType), "Extended", null, null, (o) => (BigAnimType)o.Scale.X, (o, v) => o.Scale.X = (int)v)
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Big the Cat"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }

		public enum BigAnimType
		{
			Normal,
			TreeShake,
			Victory,
			Fishing,
			LookAtRod,
			BellyScratch,
			Laying,
			Cheering,
			HoldObject,
			HipSway,
			Wave,
			ChestScratch,
			ButtScratch,
			Fanning,
			PanicRun,
			WalkWhileHolding,
			LookUp,
			LookDown,
			LayingBack,
			FishAndLookToSide,
			HoldingFroggy,
			HoldingFish,
		}
	}
}