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
	class EggGolemS : LevelDefinition
	{
		NJS_OBJECT glyphmodel;
		Mesh[] glyphmesh;
		NJS_TEXLIST glyphtls;
		Texture[] texs1;
		NJS_OBJECT quicksandmodel;
		Mesh[] quicksandmesh;
		NJS_TEXLIST quicksandtls;
		Texture[] sandtexs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			glyphmodel = ObjectHelper.LoadModel("BossGolem/models/Hieroglyphs.sa2mdl");
			glyphmesh = ObjectHelper.GetMeshes(glyphmodel);
			glyphtls = NJS_TEXLIST.Load("BossGolem/tls/Hieroglyphs.satex");
			quicksandmodel = ObjectHelper.LoadModel("BossGolem/models/Quicksand.sa2mdl");
			quicksandmesh = ObjectHelper.GetMeshes(quicksandmodel);
			quicksandtls = NJS_TEXLIST.Load("BossGolem/tls/Quicksand.satex");
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			//lol
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("bgbossgolem", glyphtls, dev);
			if (sandtexs == null)
				sandtexs = ObjectHelper.GetTextures("bgbossgolem", quicksandtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			result1.AddRange(glyphmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, glyphmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(quicksandmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtexs, quicksandmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}