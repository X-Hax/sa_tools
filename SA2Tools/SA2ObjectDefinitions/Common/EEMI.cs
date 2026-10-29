using Assimp;
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
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Common
{
	public abstract class EEMIBase : ObjectDefinition
	{
		protected NJS_OBJECT[] rhinomodels;
		protected Mesh[][] rhinomeshes;
		protected NJS_TEXLIST[] rhinotexlists;
		protected Texture[][] rhinotexpacks;


		public override void Init(ObjectData data, string name)
		{
			rhinomodels = new NJS_OBJECT[4];
			rhinomeshes = new Mesh[4][];
			rhinotexlists = new NJS_TEXLIST[2];
			rhinotexpacks = new Texture[2][];


			rhinomodels[0] = ObjectHelper.LoadModel("enemy/emi/E_EMI_NORMAL.sa2mdl");
			rhinomodels[1] = ObjectHelper.LoadModel("enemy/emi/E_EMI_SPIKED.sa2mdl");
			rhinomodels[2] = ObjectHelper.LoadModel("enemy/emi/E_EMI_CANNON.sa2mdl");
			rhinomodels[3] = ObjectHelper.LoadModel("enemy/emi/E_EMI_ARMORED.sa2mdl");

			rhinotexlists[0] = NJS_TEXLIST.Load("enemy/emi/tls/EMI.satex");
			rhinotexlists[1] = NJS_TEXLIST.Load("enemy/emi/tls/EMI_SPIKED.satex");

			for (int i = 0; i < 4; i++)
			{
				rhinomeshes[i] = ObjectHelper.GetMeshes(rhinomodels[i]);
			}
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int rhinoID = (int)item.Scale.X > 4 ? 0 : (int)item.Scale.X;
			HitResult result = HitResult.NoHit;
			NJS_OBJECT currentmodel;
			Mesh[] currentmesh;

			transform.Push();
			transform.NJTranslate(item.Position);
			//transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), 0);
			switch (rhinoID)
			{
				case 0:
				case 2:
				default:
					currentmodel = rhinomodels[0];
					currentmesh = rhinomeshes[0];
					break;
				case 1:
					currentmodel = rhinomodels[1];
					currentmesh = rhinomeshes[1];
					break;
				case 3:
					currentmodel = rhinomodels[2];
					currentmesh = rhinomeshes[2];
					break;
				case 4:
					currentmodel = rhinomodels[3];
					currentmesh = rhinomeshes[3];
					break;
			}
			result = currentmodel.CheckHit(Near, Far, Viewport, Projection, View, transform, currentmesh);
			transform.Pop();

			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int rhinoID = (int)item.Scale.X > 4 ? 0 : (int)item.Scale.X;
			NJS_OBJECT currentmodel;
			Mesh[] currentmesh;
			Texture[] currenttextures;

			if (rhinotexpacks[0] == null)
				rhinotexpacks[0] = ObjectHelper.GetTextures("E_EMITEX", rhinotexlists[0], dev);
			if (rhinotexpacks[1] == null)
				rhinotexpacks[1] = ObjectHelper.GetTextures("E_T_EMITEX", rhinotexlists[1], dev);

			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			switch (rhinoID)
			{
				case 0:
				case 2:
				default:
					currentmodel = rhinomodels[0];
					currentmesh = rhinomeshes[0];
					currenttextures = rhinotexpacks[0];
					break;
				case 1:
					currentmodel = rhinomodels[1];
					currentmesh = rhinomeshes[1];
					currenttextures = rhinotexpacks[1];
					break;
				case 3:
					currentmodel = rhinomodels[2];
					currentmesh = rhinomeshes[2];
					currenttextures = rhinotexpacks[0];
					break;
				case 4:
					currentmodel = rhinomodels[3];
					currentmesh = rhinomeshes[3];
					currenttextures = rhinotexpacks[0];
					break;
			}
			result.AddRange(currentmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, currenttextures, currentmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(currentmodel.DrawModelTreeInvert(transform, currentmesh));
			transform.Pop();

			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int rhinoID = (int)item.Scale.X > 4 ? 0 : (int)item.Scale.X;
			NJS_OBJECT currentmodel;

			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			transform.NJTranslate(item.Position);
			//transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
			switch (rhinoID)
			{
				case 0:
				case 2:
				default:
					currentmodel = rhinomodels[0];
					break;
				case 1:
					currentmodel = rhinomodels[1];
					break;
				case 3:
					currentmodel = rhinomodels[2];
					break;
				case 4:
					currentmodel = rhinomodels[3];
					break;
			}
			result.Add(new ModelTransform(currentmodel, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int rhinoID = (int)item.Scale.X > 4 ? 0 : (int)item.Scale.X;
			NJS_OBJECT currentmodel;

			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			//transform.NJRotateObject(0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);
			switch (rhinoID)
			{
				case 0:
				case 2:
				default:
					currentmodel = rhinomodels[0];
					break;
				case 1:
					currentmodel = rhinomodels[1];
					break;
				case 3:
					currentmodel = rhinomodels[2];
					break;
				case 4:
					currentmodel = rhinomodels[3];
					break;
			}
			return ObjectHelper.GetModelBounds(currentmodel, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y + ObjectHelper.DegToBAMS(90), item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + ObjectHelper.DegToBAMS(90);
			item.Rotation.Z = -z;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Bomb Type", typeof(BombType), "Extended", null, null, (o) => (BombType)o.Rotation.X, (o, v) => o.Rotation.X = (int)(BombType)v),
			new PropertySpec("Emerald/Item Link ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.Y & 0xFF,
			(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Rhino Type", typeof(RhinoType), "Extended", null, null, (o) => (RhinoType)(o.Scale.X > 4 ? 0 : o.Scale.X), (o, v) => o.Scale.X = (int)v),
			new PropertySpec("Cannon Firing Rate", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Vision Radius", typeof(float), "Extended", null, 60.0f, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v > 0 ? (float)v : 999.0f)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class EEMI : EEMIBase
	{
		public override string Name { get { return "GUN Rhino"; } }
	}
	public class EEMIN : EEMIBase
	{
		public override string Name { get { return "GUN Rhino (Near)"; } }

	}
	public class EEMIF : EEMIBase
	{
		public override string Name { get { return "GUN Rhino (Far)"; } }
	}

	public enum RhinoType
	{
		Normal,
		Spike,
		NormalDebug,
		Cannon,
		Metal
	}
	public enum BombType
	{
		Delayed,
		Immediate,
		Large,
		DelayedAlt
	}
}