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
	class DeathChamber : LevelDefinition
	{
		NJS_OBJECT glyphmodel1;
		Mesh[] glyphmesh1;
		Vector3 glyphpos1 = new Vector3(0.0f, 200.0f, 0.0f);
		NJS_OBJECT glyphmodel2;
		Mesh[] glyphmesh2;
		Vector3 glyphpos2 = new Vector3(0.0f, 80.0f, 0.0f);
		NJS_OBJECT glyphmodel3;
		Mesh[] glyphmesh3;

		NJS_OBJECT glyphmodel4;
		Mesh[] glyphmesh4;
		Vector3[] glyphpos4 = { new Vector3(0.0f, -130.0f, 785.0f), new Vector3(1400.0f, -30.0f, 0.0f) };
		int[] glyph4yrot = { 0, 0x4000 };

		NJS_OBJECT glyphmodel5;
		Mesh[] glyphmesh5;
		Vector3[] glyphpos5 = { new Vector3(-470.0f, -30.0f, 860.0f), new Vector3(470.0f, -30.0f, 860.0f) };
		int[] glyph5yrot = { 0, -0x4000 };

		NJS_OBJECT glyphmodel6;
		Mesh[] glyphmesh6;
		Vector3 glyphpos6 = new Vector3(-160.0f, 100.0f, 1330.0f);

		NJS_OBJECT glyphmodel7;
		Mesh[] glyphmesh7;
		Vector3 glyphpos7 = new Vector3(1400.0f, -30.0f, 480.0f);

		NJS_OBJECT glyphmodel8;
		Mesh[] glyphmesh8;
		Vector3 glyphpos8 = new Vector3(1400.0f, -30.0f, -480.0f);

		NJS_OBJECT watermodel1;
		Mesh[] watermesh1;
		NJS_OBJECT watermodel2;
		Mesh[] watermesh2;
		NJS_OBJECT watermodel3;
		Mesh[] watermesh3;

		NJS_OBJECT sparkmodel;
		Mesh[] sparkmesh;
		Vector3[] sparkpos = { new Vector3(-35.5f, 160.0f, 35.5f), new Vector3(35.5f, 160.0f, -35.5f), new Vector3(-35.5f, 160.0f, -35.5f), new Vector3(35.5f, 160.0f, 35.5f) };
		NJS_OBJECT pcoremodel;
		Mesh[] pcoremesh;
		NJS_OBJECT pcorepistonmodel;
		Mesh[] pcorepistonmesh;

		NJS_TEXLIST glyphtls;
		Texture[] glyphtexs;
		NJS_TEXLIST pcoretls;
		Texture[] pcoretexs;
		NJS_TEXLIST sparktls;
		Texture[] sparktexs;
		NJS_TEXLIST watertls;
		Texture[] watertexs;

		List<string> multitexs = [];

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			glyphmodel1 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs1.sa2mdl");
			glyphmesh1 = ObjectHelper.GetMeshes(glyphmodel1);
			glyphmodel2 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs2.sa2mdl");
			glyphmesh2 = ObjectHelper.GetMeshes(glyphmodel2);
			glyphmodel3 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs3.sa2mdl");
			glyphmesh3 = ObjectHelper.GetMeshes(glyphmodel3);
			glyphmodel4 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs4.sa2mdl");
			glyphmesh4 = ObjectHelper.GetMeshes(glyphmodel4);
			glyphmodel5 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs5.sa2mdl");
			glyphmesh5 = ObjectHelper.GetMeshes(glyphmodel5);
			glyphmodel6 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs6.sa2mdl");
			glyphmesh6 = ObjectHelper.GetMeshes(glyphmodel6);
			glyphmodel7 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs7.sa2mdl");
			glyphmesh7 = ObjectHelper.GetMeshes(glyphmodel7);
			glyphmodel8 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Hieroglyphs8.sa2mdl");
			glyphmesh8 = ObjectHelper.GetMeshes(glyphmodel8);
			glyphtls = NJS_TEXLIST.Load("stg25_DeathChamber/tls/Hieroglyphs.satex");

			watermodel1 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Water1.sa2mdl");
			watermesh1 = ObjectHelper.GetMeshes(watermodel1);
			watermodel2 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Water2.sa2mdl");
			watermesh2 = ObjectHelper.GetMeshes(watermodel2);
			watermodel3 = ObjectHelper.LoadModel("stg25_DeathChamber/models/Water3.sa2mdl");
			watermesh3 = ObjectHelper.GetMeshes(watermodel3);
			watertls = NJS_TEXLIST.Load("stg25_DeathChamber/tls/Water.satex");

			pcoremodel = ObjectHelper.LoadModel("stg25_DeathChamber/models/PyramidCore.sa2mdl");
			pcoremesh = ObjectHelper.GetMeshes(pcoremodel);
			pcorepistonmodel = ObjectHelper.LoadModel("stg25_DeathChamber/models/Pistons.sa2mdl");
			pcorepistonmesh = ObjectHelper.GetMeshes(pcorepistonmodel);
			pcoretls = NJS_TEXLIST.Load("stg25_DeathChamber/tls/PyramidCore.satex");

			sparkmodel = ObjectHelper.LoadModel("stg25_DeathChamber/models/BIRIBIRI/00.sa2mdl");
			sparkmesh = ObjectHelper.GetMeshes(sparkmodel);
			sparktls = NJS_TEXLIST.Load("stg25_DeathChamber/tls/BIRIBIRI/00.satex");

			multitexs.Add("objtex_stg25");
			multitexs.Add("landtx25");
		}
		public override void Render(Device dev, EditorCamera cam)
		{
			//lol
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (glyphtexs == null)
				glyphtexs = ObjectHelper.GetTextures("bgtex25", glyphtls, dev);
			if (watertexs == null)
				watertexs = ObjectHelper.GetTextures("stg25_water", watertls, dev);
			if (pcoretexs == null)
				pcoretexs = ObjectHelper.GetTexturesMultiSource(multitexs, pcoretls, dev);
			if (sparktexs == null)
				sparktexs = ObjectHelper.GetTexturesMultiSource(multitexs, sparktls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			result1.AddRange(pcoremodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, pcoretexs, pcoremesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(pcorepistonmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, pcoretexs, pcorepistonmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(watermodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, watertexs, watermesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(watermodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, watertexs, watermesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(watermodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, watertexs, watermesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(glyphpos1);
			result1.AddRange(glyphmodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(glyphpos2);
			result1.AddRange(glyphmodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			result1.AddRange(glyphmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			for (int i = 0; i < glyphpos4.Length; i++)
			{
				transform.Push();
				transform.NJTranslate(glyphpos4[i]);
				transform.NJRotateY(glyph4yrot[i]);
				result1.AddRange(glyphmodel4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			for (int i = 0; i < glyphpos5.Length; i++)
			{
				transform.Push();
				transform.NJTranslate(glyphpos5[i]);
				transform.NJRotateY(glyph5yrot[i]);
				result1.AddRange(glyphmodel5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			transform.Push();
			transform.NJTranslate(glyphpos6);
			result1.AddRange(glyphmodel6.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh6, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(glyphpos7);
			result1.AddRange(glyphmodel7.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh7, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(glyphpos8);
			result1.AddRange(glyphmodel8.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, glyphtexs, glyphmesh8, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			for (int i = 0; i < sparkpos.Length; i++)
			{
				transform.Push();
				transform.NJTranslate(sparkpos[i]);
				result1.AddRange(sparkmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sparktexs, sparkmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}