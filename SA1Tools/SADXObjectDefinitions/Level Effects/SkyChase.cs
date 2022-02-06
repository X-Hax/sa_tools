using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;
using SharpDX;
using SplitTools;

namespace SADXObjectDefinitions.Level_Effects
{
	class SkyChase : LevelDefinition
	{
		NJS_OBJECT carriermdl, skytop, skybottom;
		Mesh[] carriermesh, skytopmesh, skybottommesh;
		string texname;
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			if (data.LevelID == "3700")
			{
				skytop = ObjectHelper.LoadModel("shooting/act2/models/ecsc_s_sora.nja.sa1mdl");
				skybottom = ObjectHelper.LoadModel("shooting/act2/models/ecsc_s_sitakumo.nja.sa1mdl");
				texname = "SHOOTING2";
			}
			else
			{
				skytop = ObjectHelper.LoadModel("shooting/act1/models/ecsc_hare_s_sora_hare.nja.sa1mdl");
				skybottom = ObjectHelper.LoadModel("shooting/act1/models/ecsc_s_sitakumo2.nja.sa1mdl");
				texname = "SHOOTING1";
			}
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("shooting/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			carriermdl = ObjectHelper.LoadModel("shooting/common/models/shot_bf_s_bodya.nja.sa1mdl");
			carriermesh = ObjectHelper.GetMeshes(carriermdl);
			skytopmesh = ObjectHelper.GetMeshes(skytop);
			skybottommesh = ObjectHelper.GetMeshes(skybottom);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			Texture[] texs = ObjectHelper.GetTextures(texname);
			// Sky
			transform.Push();
			transform.NJTranslate(0.0f, -4000.0f, 0.0f);
			transform.NJScale(Skybox_Scale);
			result.AddRange(skytop.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, skytopmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Bottom cloud 1			
			transform.Push();
			transform.NJTranslate(0.0f, -4000.0f, 0.0f);
			transform.NJScale(24.0f, 1.0f, 26.0f);
			result.AddRange(skybottom.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, skybottommesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Bottom cloud 2
			transform.Push();
			transform.NJTranslate(0.0f, -2000.0f, 0.0f);
			transform.NJScale(24.0f, 1.0f, 26.0f);
			result.AddRange(skybottom.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, skybottommesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Egg Carrier
			transform.Push();
			carriermdl.ProcessTransforms(transform);
			carriermdl.ProcessVertexData();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			result.AddRange(carriermdl.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("SHOOTING0"), carriermesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting, boundsByMesh: true));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam, true);
		}
	}
}