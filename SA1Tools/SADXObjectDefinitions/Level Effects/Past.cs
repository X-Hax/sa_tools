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
	class Past : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Vector3 Skybox_Scale;
		Texture[] texs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("adv03_past/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			switch (act)
			{
				case 0:
					model = ObjectHelper.LoadModel("adv03_past/bg/mrc_bf_s_skyhiru.nja.sa1mdl");
					texs = ObjectHelper.GetTextures("MR_SKY02");
					break;
				case 1:
					model = ObjectHelper.LoadModel("adv03_past/bg/mra_s_sora_hare.nja.sa1mdl");
					texs = ObjectHelper.GetTextures("MR_SKY00");
					break;
				case 2:
					model = ObjectHelper.LoadModel("adv03_past/bg/mra_s_sora_yoru.nja.sa1mdl");
					texs = ObjectHelper.GetTextures("MR_SKY00");
					break;
			}
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(2.0f, 2.0f, 2.0f);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}