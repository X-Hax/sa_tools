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
		Texture[] texs_bg, texs_ocean;
		byte Act;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("adv03_past/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			switch (act)
			{
				case 0:
					model = ObjectHelper.LoadModel("adv03_past/bg/mrc_bf_s_skyhiru.nja.sa1mdl");
					break;
				case 1:
					model = ObjectHelper.LoadModel("adv03_past/bg/mra_s_sora_hare.nja.sa1mdl");
					break;
				case 2:
					model = ObjectHelper.LoadModel("adv03_past/bg/mra_s_sora_yoru.nja.sa1mdl");
					break;
			}
			meshes = ObjectHelper.GetMeshes(model);
			SetOceanData();
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			switch (Act)
			{
				case 0:
					texs_bg = ObjectHelper.GetTextures("MR_SKY02");
					break;
				case 1:
					texs_bg = ObjectHelper.GetTextures("MR_SKY00");
					break;
				case 2:
					texs_bg = ObjectHelper.GetTextures("MR_SKY00");
					break;
			}
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(2.0f, 2.0f, 2.0f);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_bg, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			switch (Act)
			{
				case 0:
				default:
					return;
				case 1:
					texs_ocean = ObjectHelper.GetTextures("PAST01");
					break;
				case 2:
					texs_ocean = ObjectHelper.GetTextures("PAST02");
					break;
			}
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
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_ocean?[water.TextureWaves], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
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
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_ocean?[water.TextureSea], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
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
			ushort texsea = 71;
			ushort texwaves = 70;
			SADXOceanData.Initialize();
			switch (Act)
			{
				case 0:
				default:
					return;
				case 1:
					texsea = 71;
					texwaves = 70;
					break;
				case 2:
					texsea = 86;
					texwaves = 85;
					break;
			}
			SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
			{
				Center = new Vertex(-2800.0f, -220.0f, -500.0f),
				WrapX = 40,
				WrapZ = 23,
				WrapXZ = 128.0f,
				TextureSea = texsea,
				TextureWaves = texwaves
			});
			SADXOceanData.InitWaterSurface(0);
		}
	}
}