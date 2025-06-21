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
	class FinalHazard : LevelDefinition
	{
		NJS_OBJECT arkmodel;
		Mesh[] arkmesh;
		NJS_TEXLIST arktls;
		Texture[] arktexs;
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
			arkmodel = ObjectHelper.LoadModel("BossLast2/models/ARK.sa2mdl");
			arkmesh = ObjectHelper.GetMeshes(arkmodel);
			arktls = NJS_TEXLIST.Load("BossLast2/tls/ARK.satex");
			spacemodel1 = ObjectHelper.LoadModel("BossLast2/models/Space.sa2mdl");
			spacemesh1 = ObjectHelper.GetMeshes(spacemodel1);
			spacetls1 = NJS_TEXLIST.Load("BossLast2/tls/Space.satex");
			spacemodel2 = ObjectHelper.LoadModel("BossLast2/models/TransparentSpace.sa2mdl");
			spacemesh2 = ObjectHelper.GetMeshes(spacemodel2);
			spacetls2 = NJS_TEXLIST.Load("BossLast2/tls/TransparentSpace.satex");
			earthmodel = ObjectHelper.LoadModel("BossLast2/models/Earth.sa2mdl");
			earthmesh = ObjectHelper.GetMeshes(earthmodel);
			earthtls = NJS_TEXLIST.Load("BossLast2/tls/Earth.satex");
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (earthtexs == null)
				earthtexs = ObjectHelper.GetTextures("bgtex_space0", earthtls, dev);
			if (spacetexs1 == null)
				spacetexs1 = ObjectHelper.GetTextures("bgtex_space0", spacetls1, dev);
			if (arktexs == null)
				arktexs = ObjectHelper.GetTextures("bgtex_space0", arktls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJRotateZ(0x1555);
			result1.AddRange(arkmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, arktexs, arkmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(cam.Position);
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
				spacetexs2 = ObjectHelper.GetTextures("bgtex_space0", spacetls2, dev);
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