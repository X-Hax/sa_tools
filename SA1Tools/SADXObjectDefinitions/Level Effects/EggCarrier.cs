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
	class EggCarrier : LevelDefinition
	{
		protected NJS_OBJECT model1, model2, model3;
		protected Mesh[] mesh1, mesh2, mesh3;
		protected Vector3 Skybox_Scale;
		byte Act;
		byte TimeOfDay;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			TimeOfDay = timeofday;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("adv01_eggcarrierab/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model1 = ObjectHelper.LoadModel("adv01_eggcarrierab/sky/ecsc_s_sora.nja.sa1mdl");
			model2 = ObjectHelper.LoadModel("adv01_eggcarrierab/sky/ecsc_s_sitakumo.nja.sa1mdl");
			model3 = ObjectHelper.LoadModel("adv01_eggcarrierab/sea/ecsc_s_sora_hare.nja.sa1mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			mesh2 = ObjectHelper.GetMeshes(model2);
			mesh3 = ObjectHelper.GetMeshes(model3);
			SetOceanData();
		}

		private void RenderEggCarrierSkyRegular(Device dev, EditorCamera cam, float yPos)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, yPos, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("EC_SKY"), mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.NJScale(1.0f, 1.0f, 1.0f);
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		private void RenderEggCarrierSkyCloud(Device dev, EditorCamera cam, float yPos)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			dev.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			dev.SetRenderState(RenderState.DestinationBlend, Blend.One);
			transform.NJTranslate(cam.Position.X, yPos, cam.Position.Z);
			transform.NJScale(3.0f, 1.0f, 3.0f);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("EC_SKY"), mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.NJScale(1.0f, 1.0f, 1.0f);
			dev.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			dev.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		private void RenderEggCarrierSkyClear(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("EC_SEA"), mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.NJScale(1.0f, 1.0f, 1.0f);
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (Act == 4 || Act == 5)
				return;
			float skyposY = Act == 6 ? -3000.0f : 0.0f;
			if (TimeOfDay == 0 && Act != 6)
				RenderEggCarrierSkyClear(dev, cam);
			else
			{
				RenderEggCarrierSkyRegular(dev, cam, skyposY);
				if (Act <= 2 || Act == 6)
					RenderEggCarrierSkyCloud(dev, cam, skyposY);
			}
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (TimeOfDay != 0 || Act > 2)
				return;
			Texture[] texs_ocean = ObjectHelper.GetTextures("ADV_EC0" + Act.ToString());
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			for (int o = 0; o < SADXOceanData.WaterSurfaces.Count; o++)
			{
				SADXOceanData.WaterSurfaceData water = SADXOceanData.WaterSurfaces[o];
				transform.Push();
				transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
				transform.NJScale(2.0f, 1.0f, 2.0f);
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
			if (Act > 2 || TimeOfDay != 0)
				return;
			ushort textureSea;
			ushort textureWaves;
			switch (Act)
			{
				case 0:
					textureWaves = 96;
					textureSea = 97;
					break;
				case 1:
					textureWaves = 89;
					textureSea = 90;
					break;
				case 2:
					textureWaves = 64;
					textureSea = 65;
					break;
				default:
					return;
			}
			SADXOceanData.Initialize();
			SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
			{
				Center = new Vertex(-4000.0f, 415.0f, -4000.0f),
				WrapX = 30,
				WrapZ = 30,
				WrapXZ = 512.0f,
				TextureSea = textureSea,
				TextureWaves = textureWaves
			});
			SADXOceanData.InitWaterSurface(0);
		}
	}
}