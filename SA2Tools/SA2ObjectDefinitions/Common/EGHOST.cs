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
	public abstract class EGHOSTBase : ObjectDefinition
	{
		protected NJS_OBJECT[] ghostmodels;
		protected Mesh[][] ghostmeshes;
		protected NJS_TEXLIST ghosttexlist;
		protected Texture[] texs;

		public override void Init(ObjectData data, string name)
		{
			ghostmodels = new NJS_OBJECT[4];
			ghostmeshes = new Mesh[4][];
			ghostmodels[0] = ObjectHelper.LoadModel("enemy/ghost/E_GHOST_MINI.sa2mdl");
			ghostmodels[1] = ObjectHelper.LoadModel("enemy/ghost/E_GHOST_HIDE.sa2mdl");
			ghostmodels[2] = ObjectHelper.LoadModel("enemy/ghost/E_GHOST_NORMAL.sa2mdl");
			ghostmodels[3] = ObjectHelper.LoadModel("enemy/ghost/E_GHOST_BIG.sa2mdl");

			for (int i = 0; i < 4; i++)
			{
				ghostmeshes[i] = ObjectHelper.GetMeshes(ghostmodels[i]);
			}

			ghosttexlist = NJS_TEXLIST.Load("enemy/ghost/tls/GHOST1.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int ghostType = (int)item.Scale.X > 7 ? 0 : (int)item.Scale.X;
			int modelID;
			switch (ghostType)
			{
				case 0:
				default:
					modelID = 0;
					break;
				case 1:
					modelID = 1;
					break;
				case 2:
				case 3:
				case 5:
				case 6:
					modelID = 2;
					break;
				case 4:
					modelID = 3;
					break;
			}
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			result = HitResult.Min(result, ghostmodels[modelID].CheckHit(Near, Far, Viewport, Projection, View, transform, ghostmeshes[modelID]));
			transform.Pop();
			
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures("e_ghostex", ghosttexlist, dev);

			int ghostType = (int)item.Scale.X > 7 ? 0 : (int)item.Scale.X;
			int modelID;
			switch (ghostType)
			{
				case 0:
				default:
					modelID = 0;
					break;
				case 1:
					modelID = 1;
					break;
				case 2:
				case 3:
				case 5:
				case 6:
					modelID = 2;
					break;
				case 4:
					modelID = 3;
					break;
			}
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			result.AddRange(ghostmodels[modelID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, ghostmeshes[modelID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(ghostmodels[modelID].DrawModelTreeInvert(transform, ghostmeshes[modelID]));
			}
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int ghostType = (int)item.Scale.X > 7 ? 0 : (int)item.Scale.X;
			int modelID;
			switch (ghostType)
			{
				case 0:
				default:
					modelID = 0;
					break;
				case 1:
					modelID = 1;
					break;
				case 2:
				case 3:
				case 5:
				case 6:
					modelID = 2;
					break;
				case 4:
					modelID = 3;
					break;
			}
			{
				List<ModelTransform> result = new List<ModelTransform>();
				transform.Push();
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y + 0x4000);
				result.Add(new ModelTransform(ghostmodels[modelID], transform.Top));
				transform.Pop();
				return result;
			}
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int ghostType = (int)item.Scale.X > 7 ? 0 : (int)item.Scale.X;
			int modelID;
			switch (ghostType)
			{
				case 0:
				default:
					modelID = 0;
					break;
				case 1:
					modelID = 1;
					break;
				case 2:
				case 3:
				case 5:
				case 6:
					modelID = 2;
					break;
				case 4:
					modelID = 3;
					break;
			}
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y + 0x4000);
			BoundingSphere ghostbounds = ObjectHelper.GetModelBounds(ghostmodels[modelID], transform);
			transform.Pop();
			return ghostbounds;
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
			
			new PropertySpec("Ghost Type", typeof(GhostType), "Extended", "'Roam' and 'Boom Boo' are the only ghost types that can reward hunter items upon defeat.", null, (o) => (GhostType)((int)o.Scale.X > 7 ? 0 : (int)o.Scale.X), (o, v) => o.Scale.X = (float)(GhostType)v),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Wandering Distance", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class EGHOST : EGHOSTBase
	{
		public override string Name { get { return "Boo"; } }
	}
	public class EGHOSTN : EGHOSTBase
	{
		public override string Name { get { return "Boo (Near)"; } }
	}
	public class EGHOSTF : EGHOSTBase
	{
		public override string Name { get { return "Boo (Far)"; } }
	}

	public enum GhostType
	{
		RoamScare,
		Scare,
		Roam,
		Grab,
		BoomBoo,
		Stationary,
		Stationary2
	}
}