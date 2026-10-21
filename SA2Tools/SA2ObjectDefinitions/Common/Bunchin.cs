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
	public class Bunchin : ObjectDefinition
	{
		private NJS_OBJECT[] weightmodels;
		private Mesh[][] weightmeshes;
		private NJS_TEXLIST[] texarrs;
		private Texture[][] texs;

		public override void Init(ObjectData data, string name)
		{
			weightmodels = new NJS_OBJECT[4];
			weightmeshes = new Mesh[4][];
			texarrs = new NJS_TEXLIST[3];
			texs = new Texture[3][];


			weightmodels[0] = ObjectHelper.LoadModel("object/OBJECT_BUNCHIN.sa2mdl");
			weightmodels[1] = ObjectHelper.LoadModel("object/OBJECT_BUNCHIN_GLOW.sa2mdl");
			weightmodels[2] = ObjectHelper.LoadModel("object/OBJECT_BUNCHIN_HANDLE.sa2mdl");
			weightmodels[3] = ObjectHelper.LoadModel("object/OBJECT_BUNCHIN_BOTTOM.sa2mdl");

			for (int i = 0; i < 4; i++)
				weightmeshes[i] = ObjectHelper.GetMeshes(weightmodels[i]);
			texarrs[0] = NJS_TEXLIST.Load("object/tls/BUNCHIN.satex");
			texarrs[1] = NJS_TEXLIST.Load("object/tls/BUNCHIN_GLOW.satex");
			texarrs[2] = NJS_TEXLIST.Load("object/tls/BUNCHIN_HANDLE.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 50F + Math.Abs(item.Scale.Y), item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, weightmodels[2].CheckHit(Near, Far, Viewport, Projection, View, transform, weightmeshes[2]));
			transform.NJScale((item.Scale.X + 1F), 1F, (item.Scale.Z + 1F));
			result = HitResult.Min(result, weightmodels[0].CheckHit(Near, Far, Viewport, Projection, View, transform, weightmeshes[0]));
			result = HitResult.Min(result, weightmodels[1].CheckHit(Near, Far, Viewport, Projection, View, transform, weightmeshes[1]));
			result = HitResult.Min(result, weightmodels[3].CheckHit(Near, Far, Viewport, Projection, View, transform, weightmeshes[3]));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			for (int i = 0; i < 3; i++)
			{
				if (texs[i] == null)
					texs[i] = ObjectHelper.GetTextures("objtex_common", texarrs[i], dev);
			}
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 50F + Math.Abs(item.Scale.Y), item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(weightmodels[2].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[2], weightmeshes[2], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(weightmodels[2].DrawModelTreeInvert(transform, weightmeshes[2]));
			transform.NJScale((item.Scale.X + 1F), 1F, (item.Scale.Z + 1F));
			result.AddRange(weightmodels[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[0], weightmeshes[0], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(weightmodels[1].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[1], weightmeshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(weightmodels[3].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, weightmeshes[3], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(weightmodels[0].DrawModelTreeInvert(transform, weightmeshes[0]));
				result.AddRange(weightmodels[1].DrawModelTreeInvert(transform, weightmeshes[1]));
				result.AddRange(weightmodels[3].DrawModelTreeInvert(transform, weightmeshes[3]));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 50F + Math.Abs(item.Scale.Y), item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(weightmodels[2], transform.Top));
			transform.NJScale((item.Scale.X + 1), 1F, (item.Scale.Z + 1));
			result.Add(new ModelTransform(weightmodels[0], transform.Top));
			result.Add(new ModelTransform(weightmodels[1], transform.Top));
			result.Add(new ModelTransform(weightmodels[3], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, item.Position.Y + 50F + Math.Abs(item.Scale.Y), item.Position.Z);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere handlebounds = ObjectHelper.GetModelBounds(weightmodels[2], transform);
			transform.NJScale((item.Scale.X + 1), 1F, (item.Scale.Z + 1));
			BoundingSphere basebounds = ObjectHelper.GetModelBounds(weightmodels[0], transform);
			BoundingSphere glowbounds = ObjectHelper.GetModelBounds(weightmodels[1], transform);
			BoundingSphere bottombounds = ObjectHelper.GetModelBounds(weightmodels[3], transform);
			BoundingSphere weightbounds = SAModel.Direct3D.Extensions.Merge(basebounds, SAModel.Direct3D.Extensions.Merge(glowbounds, bottombounds));
			return SAModel.Direct3D.Extensions.Merge(weightbounds, handlebounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Weight Type", typeof(WeightType), "Extended", null, null, (o) => (WeightType)o.Rotation.X, (o, v) => o.Rotation.X = (int)(WeightType)v),
			new PropertySpec("Switch ID", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Weight Length", typeof(float), "Extended", "The game adds 1 to this value", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Rest Position", typeof(float), "Extended", "The game adds 50 to this value, which is always treated as positive", null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Weight Depth", typeof(float), "Extended", "The game adds 1 to this value.", null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Weight Trap"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public enum WeightType
	{
		Continuous,
		TrapNormal,
		TrapSwitch,
		ContinuousSwitch
	}
}