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
	class MysticRuins : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Vector3 Skybox_Scale;
		Texture[] texs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			if (act == 3)
				return;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("adv02_mysticruin/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			switch (timeofday)
			{
				case 0:
				default:
					if (act == 0)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mra_s_sora_hare.nja.sa1mdl");
					else if (act == 1)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mrb_s_sora_hare.nja.sa1mdl");
					else if (act == 2)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mrc_bf_s_skyhiru.nja.sa1mdl");
					break;
				case 1:
					if (act == 0)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mra_s_sora_yuu.nja.sa1mdl");
					else if (act == 1)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mrb_s_sora_yuu.nja.sa1mdl");
					else if (act == 2)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mrc_bf_s_skyyuu.nja.sa1mdl");
					break;
				case 2:
					if (act == 0)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mra_s_sora_yoru.nja.sa1mdl");
					else if (act == 1)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mrb_s_sora_yoru.nja.sa1mdl");
					else if (act == 2)
						model = ObjectHelper.LoadModel("adv02_mysticruin/sky/mrc_bf_s_skyyoru.nja.sa1mdl");
					break;
			}
			texs = ObjectHelper.GetTextures("MR_SKY0" + act.ToString());
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (model == null)
				return;
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}