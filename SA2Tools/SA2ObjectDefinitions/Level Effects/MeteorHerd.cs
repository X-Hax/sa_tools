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
	class MeteorHerd : LevelDefinition
	{
		NJS_OBJECT arkmodel;
		Mesh[] arkmesh;
		NJS_TEXLIST arktls;
		Texture[] arktexs;
		List<string> multitexpacks = [];
		NJS_OBJECT spacemodel1;
		Mesh[] spacemesh1;
		NJS_TEXLIST spacetls1;
		Texture[] spacetexs1;
		NJS_OBJECT spacemodel2;
		Mesh[] spacemesh2;
		NJS_TEXLIST spacetls2;
		Texture[] spacetexs2;
		NJS_OBJECT asteroidmodel;
		Mesh[] asteroidmesh;
		NJS_TEXLIST asteroidtls;
		Texture[] asteroidtexs;


		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			arkmodel = ObjectHelper.LoadModel("stg32_MeteorHerd/models/ARKStructures.sa2mdl");
			arkmesh = ObjectHelper.GetMeshes(arkmodel);
			arktls = NJS_TEXLIST.Load("stg32_MeteorHerd/tls/ARKStructures.satex");
			spacemodel1 = ObjectHelper.LoadModel("stg32_MeteorHerd/models/Space.sa2mdl");
			spacemesh1 = ObjectHelper.GetMeshes(spacemodel1);
			spacetls1 = NJS_TEXLIST.Load("stg32_MeteorHerd/tls/Space.satex");
			spacemodel2 = ObjectHelper.LoadModel("stg32_MeteorHerd/models/TransparentSpace.sa2mdl");
			spacemesh2 = ObjectHelper.GetMeshes(spacemodel2);
			spacetls2 = NJS_TEXLIST.Load("stg32_MeteorHerd/tls/TransparentSpace.satex");
			asteroidmodel = ObjectHelper.LoadModel("stg32_MeteorHerd/models/BGAsteroids.sa2mdl");
			asteroidmesh = ObjectHelper.GetMeshes(asteroidmodel);
			asteroidtls = NJS_TEXLIST.Load("stg32_MeteorHerd/tls/BGAsteroids.satex");
			multitexpacks.Add("bgtex32");
			multitexpacks.Add("objtex_stg32");
			multitexpacks.Add("landtx32");
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (arktexs == null)
				arktexs = ObjectHelper.GetTexturesMultiSource(multitexpacks, arktls, dev);
			if (asteroidtexs == null)
				asteroidtexs = ObjectHelper.GetTexturesMultiSource(multitexpacks, asteroidtls, dev);
			if (spacetexs1 == null)
				spacetexs1 = ObjectHelper.GetTextures("bgtex32", spacetls1, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(0, 3700.0f, 0);
			result1.AddRange(asteroidmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, asteroidtexs, asteroidmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(0, 3000.0f, 0);
			result1.AddRange(arkmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, arktexs, arkmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(0, 3000.0f, 0);
			result1.AddRange(spacemodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs1, spacemesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (spacetexs2 == null)
				spacetexs2 = ObjectHelper.GetTextures("bgtex32", spacetls2, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(0, 3000.0f, 0);
			result1.AddRange(spacemodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs2, spacemesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}