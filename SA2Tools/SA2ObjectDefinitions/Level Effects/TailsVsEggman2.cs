using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Level_Effects
{
	class TailsVsEggman2 : LevelDefinition
	{
		NJS_OBJECT spacemodel1;
		Mesh[] spacemesh1;
		NJS_TEXLIST spacetls1;
		Texture[] spacetexs1;
		NJS_OBJECT spacemodel2;
		Mesh[] spacemesh2;
		NJS_TEXLIST spacetls2;
		Texture[] spacetexs2;
		NJS_OBJECT earthmodel;
		Mesh[] earthmesh;
		NJS_TEXLIST earthtls;
		Texture[] earthtexs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			spacemodel1 = ObjectHelper.LoadModel("stg29_ewvstw2/models/Space.sa2mdl");
			spacemesh1 = ObjectHelper.GetMeshes(spacemodel1);
			spacetls1 = NJS_TEXLIST.Load("stg29_ewvstw2/tls/Space.satex");
			spacemodel2 = ObjectHelper.LoadModel("stg29_ewvstw2/models/TransparentSpace.sa2mdl");
			spacemesh2 = ObjectHelper.GetMeshes(spacemodel2);
			spacetls2 = NJS_TEXLIST.Load("stg29_ewvstw2/tls/TransparentSpace.satex");
			earthmodel = ObjectHelper.LoadModel("stg29_ewvstw2/models/Earth.sa2mdl");
			earthmesh = ObjectHelper.GetMeshes(earthmodel);
			earthtls = NJS_TEXLIST.Load("stg29_ewvstw2/tls/Earth.satex");
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (earthtexs == null)
				earthtexs = ObjectHelper.GetTextures("bgtex29", earthtls, dev);
			if (spacetexs1 == null)
				spacetexs1 = ObjectHelper.GetTextures("bgtex29", spacetls1, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y + 3700.0f, cam.Position.Z);
			result1.AddRange(earthmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, earthtexs, earthmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(cam.Position);
			result1.AddRange(spacemodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs1, spacemesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (spacetexs2 == null)
				spacetexs2 = ObjectHelper.GetTextures("bgtex29", spacetls2, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			result1.AddRange(spacemodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs2, spacemesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}