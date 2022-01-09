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
	class SkyDeck : LevelDefinition
	{
		readonly NJS_OBJECT[] modelsNormal = new NJS_OBJECT[2];
		readonly NJS_OBJECT[] modelsDark = new NJS_OBJECT[2];
		readonly Mesh[][] meshesNormal = new Mesh[2][];
		readonly Mesh[][] meshesDark = new Mesh[2][];
		Vector3 Skybox_Scale;
		byte Act;
		byte timeOfDay;
		Texture[] texs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			timeOfDay = timeofday;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg06_skydeck/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			modelsNormal[0] = ObjectHelper.LoadModel("stg06_skydeck/common/models/sky_bg_sora.nja.sa1mdl"); // Normal
			modelsNormal[1] = ObjectHelper.LoadModel("stg06_skydeck/common/models/sky_bg_scroal.nja.sa1mdl"); // Normal bottom
			modelsDark[0] = ObjectHelper.LoadModel("stg06_skydeck/common/models/ecsc_sky.nja.sa1mdl"); // Dark
			modelsDark[1] = ObjectHelper.LoadModel("stg06_skydeck/common/models/ecsc_skykumo.nja.sa1mdl"); // Dark bottom
			for (int i = 0; i < 2; i++)
			{
				meshesNormal[i] = ObjectHelper.GetMeshes(modelsNormal[i]);
				meshesDark[i] = ObjectHelper.GetMeshes(modelsDark[i]);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			float x, y, z;
			switch (Act)
			{
				case 0:
				default:
					x = 0;
					y = 600;
					z = 2500;
					break;
				case 1:
					x = 0;
					y = 1050;
					z = -2500;
					break;
				case 2:
					x = 0;
					y = 750;
					z = 0;
					break;
			}
			NJS_OBJECT modelSky = (timeOfDay != 0 && Act != 2) ? modelsDark[0] : modelsNormal[0];
			NJS_OBJECT modelBottom = (timeOfDay != 0 && Act != 2) ? modelsDark[1] : modelsNormal[1];
			Mesh[] meshSky = (timeOfDay != 0 && Act != 2) ? meshesDark[0] : meshesNormal[0];
			Mesh[] meshBottom = (timeOfDay != 0 && Act != 2) ? meshesDark[1] : meshesNormal[1];
			texs = ObjectHelper.GetTextures("OBJ_SKYDECK");
			List<RenderInfo> result1 = new List<RenderInfo>();
			List<RenderInfo> result2 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(x, y, z);
			transform.NJScale(1.7f, 1.0f, 1.7f);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(modelSky.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshSky));
			RenderInfo.Draw(result1, dev, cam);
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			transform.NJScale(1.0f, 1.0f, 1.0f);
			result2.AddRange(modelBottom.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshBottom));
			transform.NJTranslate(0.0f, -100.0f, 0.0f);
			result2.AddRange(modelBottom.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshBottom));
			if (timeOfDay != 0 && Act != 2)
			{
				transform.NJTranslate(0.0f, -600.0f, 0.0f);
				transform.NJScale(Skybox_Scale);
				result2.AddRange(modelBottom.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshBottom));
				transform.NJScale(1.0f, 1.0f, 1.0f);
				transform.NJTranslate(0.0f, -100.0f, 0.0f);
				result2.AddRange(modelBottom.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshBottom));
			}
			transform.Pop();
			RenderInfo.Draw(result2, dev, cam);
		}
	}
}