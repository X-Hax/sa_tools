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
	public class ECHAOS : ObjectDefinition
	{
		NJS_OBJECT object_chaos_p1;
		NJS_OBJECT object_chaos_p1_arms;
		NJS_OBJECT object_chaos_p100;
		NJS_OBJECT object_chaos_float;
		NJS_OBJECT object_chaos_float_arms;
		NJS_OBJECT object_chaos_guard;
		public override void Init(ObjectData data, string name)
		{
			object_chaos_p1 = ObjectHelper.LoadModel("ENEMY/chaos/E_CHAOS_NORMAL.sa2mdl");
			object_chaos_p1_arms = ObjectHelper.LoadModel("ENEMY/chaos/E_CHAOS_NORMAL_ATTACK.sa2mdl");

			object_chaos_float = ObjectHelper.LoadModel("ENEMY/chaos/E_CHAOS_ROUND.sa2mdl");
			object_chaos_float_arms = ObjectHelper.LoadModel("ENEMY/chaos/E_CHAOS_ROUND_ATTACK.sa2mdl");

			object_chaos_p100 = ObjectHelper.LoadModel("ENEMY/chaos/E_CHAOS_100.sa2mdl");

			object_chaos_guard = ObjectHelper.LoadModel("ENEMY/chaos/E_CHAOS_GUARD.sa2mdl");
		}

		public override string Name { get { return "Artificial Chaos"; } }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int chaosType = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (chaosType)
			{
				default:
				case 0:
				case 7:
				case 1:
				case 2:
				case 3:
				case 4:
					pObject = object_chaos_p1;
					break;
				case 5:
					pObject = object_chaos_float;
					break;
				case 6:
				case 9:
					pObject = object_chaos_p100;
					break;
				case 8:
					pObject = object_chaos_guard;
					break;
			}

			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			HitResult result = pObject.CheckHit(Near, Far, Viewport, Projection, View, transform, ObjectHelper.GetMeshes(pObject));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int chaosType = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (chaosType)
			{
				default:
				case 0:
				case 7:
				case 1:
				case 2:
				case 3:
				case 4:
					pObject = object_chaos_p1;
					break;
				case 5:
					pObject = object_chaos_float;
					break;
				case 6:
				case 9:
					pObject = object_chaos_p100;
					break;
				case 8:
					pObject = object_chaos_guard;
					break;
			}

			List<RenderInfo> result = new List<RenderInfo>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y + ObjectHelper.DegToBAMS(180));
				result.AddRange(pObject.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("e_chaostex"), ObjectHelper.GetMeshes(pObject), EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(pObject.DrawModelTreeInvert(transform, ObjectHelper.GetMeshes(pObject)));
				}
			}
			transform.Pop();

			return result;
		}
		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int chaosType = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (chaosType)
			{
				default:
				case 0:
				case 7:
				case 1:
				case 2:
				case 3:
				case 4:
					pObject = object_chaos_p1;
					break;
				case 5:
					pObject = object_chaos_float_arms;
					break;
				case 6:
				case 9:
					pObject = object_chaos_p100;
					break;
				case 8:
					pObject = object_chaos_guard;
					break;
			}

			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateObject(0, item.Rotation.Y, 0);
				result.Add(new ModelTransform(pObject, transform.Top));
			}
			transform.Pop();

			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int chaosType = Math.Max((int)item.Scale.X, 0);

			NJS_OBJECT pObject;

			switch (chaosType)
			{
				default:
				case 0:
				case 7:
				case 1:
				case 2:
				case 3:
				case 4:
					pObject = object_chaos_p1;
					break;
				case 5:
					pObject = object_chaos_float;
					break;
				case 6:
				case 9:
					pObject = object_chaos_p100;
					break;
				case 8:
					pObject = object_chaos_guard;
					break;
			}

			MatrixStack transform = new MatrixStack();

			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(0, item.Rotation.Y, 0);

			return ObjectHelper.GetModelBounds(pObject, transform);
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
			item.Rotation.X = x + ObjectHelper.DegToBAMS(90);
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Artificial Chaos Type", typeof(ChaosType), "Extended", null, null, (o) => (ChaosType)Math.Min(Math.Max((int)o.Scale.X, 0), 8), (o, v) => o.Scale.X = (int)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public enum ChaosType
		{
			P1,
			P1LASER,
			P1REVERSELASER,
			P1SPIN,
			P1ROOFSPIN,
			FLOAT,
			P100,
			P1REVERSE,
			GUARD,
			P100BRIGHT
		}
	}
}
