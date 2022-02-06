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
	class StationSquare : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Vector3 Skybox_Scale;
		Texture[] texs_bg, texs_advss03, texs_advss04;
		byte Act;
		byte TimeOfDay;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			TimeOfDay = timeofday;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("adv00_stationsquare/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			switch (timeofday)
			{
				case 0:
				default:
					model = ObjectHelper.LoadModel("adv00_stationsquare/object/no_unite/bg/ss_haikei_sky_d.nja.sa1mdl");
					break;
				case 1:
					model = ObjectHelper.LoadModel("adv00_stationsquare/object/no_unite/bg/ss_haikei_sky_e.nja.sa1mdl");
					break;
				case 2:
					model = ObjectHelper.LoadModel("adv00_stationsquare/object/no_unite/bg/ss_haikei_sky_n.nja.sa1mdl");
					break;
			}
			meshes = ObjectHelper.GetMeshes(model);
			SetOceanData();
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs_bg = ObjectHelper.GetTextures("SS_BG");
			texs_advss03 = ObjectHelper.GetTextures("ADVSS03");
			texs_advss04 = ObjectHelper.GetTextures("ADVSS04");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_bg, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			dev.SetRenderState(RenderState.ZWriteEnable, false);
			Texture[] watertex = Act == 3 ? texs_advss03 : texs_advss04;
			for (int o = 0; o < SADXOceanData.WaterSurfaces.Count; o++)
			{
				SADXOceanData.WaterSurfaceData water = SADXOceanData.WaterSurfaces[o];
				transform.Push();
				transform.NJTranslate(water.Center);
				transform.Push();
				transform.NJTranslate(0, -1, 0);
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, watertex?[water.TextureWaves], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
					}
					transform.NJTranslate(water.WrapXZ, 0, 0);
				}
				transform.Pop();
				transform.Push();
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, watertex?[water.TextureSea], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
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
			ushort textureWaves;
			ushort textureSea;
			ushort textureSewer = 175;
			SADXOceanData.Initialize();
			switch (Act)
			{
				case 3:
					textureWaves = 172;
					switch (TimeOfDay)
					{
						case 1:
							textureSea = 176;
							break;
						case 0:
						case 2:
						default:
							textureSea = 173;
							break;
					}
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(-1500.0f, -19.0f, 1660.0f),
						WrapX = 50,
						WrapZ = 35,
						WrapXZ = 64.0f,
						TextureSea = textureSea,
						TextureWaves = textureWaves
					});
					SADXOceanData.InitWaterSurface(0);
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(442.0f, -18.0f, 1152.0f),
						WrapX = 16,
						WrapZ = 16,
						WrapXZ = 32.0f,
						TextureWaves = textureWaves,
						TextureSea = textureSewer,
					});
					SADXOceanData.InitWaterSurface(1);
					break;
				case 4:
					textureWaves = 79;
					switch (TimeOfDay)
					{
						case 1:
							textureSea = 82;
							break;
						case 0:
						case 2:
						default:
							textureSea = 80;
							break;
					}
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(-2000.0f, -16.0f, 1500.0f),
						WrapX = 50,
						WrapZ = 35,
						WrapXZ = 64.0f,
						TextureSea = textureSea,
						TextureWaves = textureWaves
					});
					SADXOceanData.InitWaterSurface(0);
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(-446.0f, -2.5f, 1869.0f),
						WrapX = 20,
						WrapZ = 10,
						WrapXZ = 16.0f,
						TextureWaves = textureWaves,
						TextureSea = textureSea,
					});
					SADXOceanData.InitWaterSurface(1);
					break;
				default:
					break;
			}
		}
	}
}