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
		Texture[] texs, texs_advmr00;
		byte Act;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
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
			meshes = ObjectHelper.GetMeshes(model);
			SetOceanData();
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (model == null)
				return;
			texs = ObjectHelper.GetTextures("MR_SKY0" + Act.ToString());
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (Act != 0)
				return;
			texs_advmr00 = ObjectHelper.GetTextures("ADV_MR00");
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			dev.SetRenderState(RenderState.ZWriteEnable, false);
			for (int o = 0; o < SADXOceanData.WaterSurfaces.Count; o++)
			{
				SADXOceanData.WaterSurfaceData water = SADXOceanData.WaterSurfaces[o];
				transform.Push();
				transform.NJTranslate(water.Center);
				transform.Push();
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_advmr00?[water.TextureWaves], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
					}
					transform.NJTranslate(water.WrapXZ, 0, 0);
				}
				transform.Pop();
				transform.Push();
				transform.NJTranslate(0, 1, 0);
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_advmr00?[water.TextureSea], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
					}
					transform.NJTranslate(water.WrapXZ, 0, 0);
				}
				transform.Pop();
				transform.Pop();
			}
			RenderInfo.Draw(result3, dev, cam);
		}

		private void SetOceanData()
		{
			SADXOceanData.Initialize();
			switch (Act)
			{
				case 0:
		
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(-4174.0f, -418.5f, -2166.0f),
						WrapX = 60,
						WrapZ = 30,
						WrapXZ = 256.0f,
						TextureSea = 128,
						TextureWaves = 127
					});
					SADXOceanData.InitWaterSurface(0);
					break;
				default:
					break;
			}
		}
	}
}