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
using System.Reflection;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Common
{
	public class IronBall2 : ObjectDefinition
	{
		protected NJS_OBJECT ballmodel;
		protected NJS_OBJECT stakemodel;
		protected Mesh[] ballmesh;
		protected Mesh[] stakemesh;
		protected NJS_TEXLIST balltexlist;
		protected Texture[] balltexs;
		protected NJS_TEXLIST staketexlist;
		protected Texture[] staketexs;

		public override void Init(ObjectData data, string name)
		{
			ballmodel = ObjectHelper.LoadModel("OBJECT/OBJECT_IRONBALL.sa2mdl");
			ballmesh = ObjectHelper.GetMeshes(ballmodel);
			stakemodel = ObjectHelper.LoadModel("OBJECT/OBJECT_IRONBALLSTAKE.sa2mdl");
			stakemesh = ObjectHelper.GetMeshes(stakemodel);
			balltexlist = NJS_TEXLIST.Load("OBJECT/tls/IRONBALL.satex");
			staketexlist = NJS_TEXLIST.Load("OBJECT/tls/IRONBALLSTAKE.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			float ironradius = (item.Scale.X + 1F) * 20;
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			result = HitResult.Min(result, stakemodel.CheckHit(Near, Far, Viewport, Projection, View, transform, stakemesh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(ironradius, 0F, 0F);
			result = HitResult.Min(result, ballmodel.CheckHit(Near, Far, Viewport, Projection, View, transform, ballmesh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(-ironradius, 0F, 0F);
			transform.NJRotateY(0x8000);
			result = HitResult.Min(result, ballmodel.CheckHit(Near, Far, Viewport, Projection, View, transform, ballmesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			float ironradius = (item.Scale.X + 1F) * 20;
			if (balltexs == null)
				balltexs = ObjectHelper.GetTextures("objtex_common", balltexlist, dev);
			if (staketexs == null)
				staketexs = ObjectHelper.GetTextures("objtex_common", staketexlist, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			result.AddRange(stakemodel.DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, staketexs, stakemesh[0], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(stakemodel.DrawModelInvert(transform, stakemesh[0], true));
			transform.NJScale(item.Scale.X + 1F, 1F, 1F);
			result.AddRange(stakemodel.Children[0].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, staketexs, stakemesh[1], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(stakemodel.Children[0].DrawModelInvert(transform, stakemesh[1], true));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJScale(-(item.Scale.X + 1F), 1F, 1F);
			result.AddRange(stakemodel.Children[0].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, staketexs, stakemesh[1], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(stakemodel.Children[0].DrawModelInvert(transform, stakemesh[1], true));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(ironradius, 0F, 0F);
			result.AddRange(ballmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, balltexs, ballmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(ballmodel.DrawModelTreeInvert(transform, ballmesh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(-ironradius, 0F, 0F);
			transform.NJRotateY(0x8000);
			result.AddRange(ballmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, balltexs, ballmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(ballmodel.DrawModelTreeInvert(transform, ballmesh));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			float ironradius = (item.Scale.X + 1F) * 20;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			result.Add(new ModelTransform(stakemodel, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(ironradius, 0F, 0F);
			result.Add(new ModelTransform(ballmodel, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(-ironradius, 0F, 0F);
			transform.NJRotateY(0x8000);
			result.Add(new ModelTransform(ballmodel, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			float ironradius = (item.Scale.X + 1F) * 20;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			BoundingSphere stakebounds = ObjectHelper.GetModelBounds(stakemodel, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(ironradius, 0F, 0F);
			BoundingSphere spikebounds = ObjectHelper.GetModelBounds(ballmodel, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 10F, item.Position.Z);
			transform.NJTranslate(ironradius, 0F, 0F);
			transform.NJRotateY(0x8000);
			spikebounds = SAModel.Direct3D.Extensions.Merge(spikebounds, ObjectHelper.GetModelBounds(ballmodel, transform));
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(stakebounds, spikebounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Rotation Radius", typeof(float), "Extended", "1 is added to this value and then multiplied by 20 in the game's code. To spawn only a single spike ball in the center, set this to -1.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Rotation Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Oscillation Strength", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Oscillation Speed", typeof(float), "Extended", null, null, (o) => o.Rotation.Y, (o, v) => o.Rotation.Y = (int)v),
			new PropertySpec("Spike Ball Type", typeof(SpikeBallType), "Extended", null, null, (o) => o.Rotation.Z % 3,(o, v) => o.Rotation.Z = (int)(SpikeBallType)v),

		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Spike Ball Trap"; } }
		public enum SpikeBallType
		{
			GenerateDust,
			FloatOscillate,
			FloatOscillateDust,
		}
	}
}