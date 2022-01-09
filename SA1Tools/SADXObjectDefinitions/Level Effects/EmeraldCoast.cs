using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class EmeraldCoast : LevelDefinition
	{
		NJS_OBJECT model1, model2, modelbridge, modelbridge2, modelbridge3, modelbridge4;
		Mesh[] mesh1, mesh2, meshbridge, meshbridge2, meshbridge3, meshbridge4;
		Vector3 Skybox_Scale;
		Texture[] texs1;
		Texture[] texs2;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg01_beach/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model1 = ObjectHelper.LoadModel("stg01_beach/bg/models/sea_nbg.nja.sa1mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("stg01_beach/bg/models/sea_nbg3.nja.sa1mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			modelbridge = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_hasi_a.nja.sa1mdl");
			meshbridge = ObjectHelper.GetMeshes(modelbridge);
			modelbridge2 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_hasi_b.nja.sa1mdl");
			meshbridge2 = ObjectHelper.GetMeshes(modelbridge2);
			modelbridge3 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_hasi_c.nja.sa1mdl");
			meshbridge3 = ObjectHelper.GetMeshes(modelbridge3);
			modelbridge4 = ObjectHelper.LoadModel("stg01_beach/common/models/seaobj_hasi_a2.nja.sa1mdl");
			meshbridge4 = ObjectHelper.GetMeshes(modelbridge4);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs1 = ObjectHelper.GetTextures("BG_BEACH");
			texs2 = ObjectHelper.GetTextures("OBJ_BEACH");
			List<RenderInfo> result1 = new List<RenderInfo>();
			List<RenderInfo> result2 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Part 1 - skybox (no Z Write)
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1));
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh2));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
			// Part 2 - bridge (Z Write)
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			transform.Push();
			// Main part 1
			transform.NJTranslate(2803.0f, -1.0f, 365.0f);
			result2.AddRange(modelbridge.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, meshbridge));
			// Repeated parts
			transform.NJTranslate(75.0f, 9.5f, 0.0f);
			for (int i = 0; i < 25; i++)
			{
				// 1
				transform.NJTranslate(20.0f, 0.0f, 0.0f);
				result2.AddRange(modelbridge2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, meshbridge2));
				// 2
				transform.NJTranslate(20.0f, 0.0f, 0.0f);
				result2.AddRange(modelbridge3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, meshbridge3));
			}
			// Small chunk
			transform.NJTranslate(20.0f, 0.0f, 0.0f);
			result2.AddRange(modelbridge2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, meshbridge2));
			// Main part 2
			transform.NJTranslate(95.0f, -9.5f, 0.0f); // Y is 0.0 in original code at 0x501AD6, no idea why
			result2.AddRange(modelbridge4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, meshbridge4));
			transform.Pop();
			RenderInfo.Draw(result2, dev, cam);
		}
	}
}