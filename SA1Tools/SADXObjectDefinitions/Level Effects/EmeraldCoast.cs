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
		Texture[] texs_bg_beach;
		Texture[] texs_obj_beach;
		Texture[] texs_beach_sea;
		byte Act;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
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
			SetOceanData();
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs_bg_beach = ObjectHelper.GetTextures("BG_BEACH");
			texs_obj_beach = ObjectHelper.GetTextures("OBJ_BEACH");
			List<RenderInfo> result1 = new List<RenderInfo>();
			List<RenderInfo> result2 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Part 1 - skybox (no Z Write)
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_bg_beach, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_bg_beach, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
			// Bridge in Act 1
			if (Act == 0)
			{
				dev.SetRenderState(RenderState.ZWriteEnable, true);
				transform.Push();
				// Main part 1
				transform.NJTranslate(2803.0f, -1.0f, 365.0f);
				result2.AddRange(modelbridge.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_obj_beach, meshbridge, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				// Repeated parts
				transform.NJTranslate(75.0f, 9.5f, 0.0f);
				for (int i = 0; i < 25; i++)
				{
					// 1
					transform.NJTranslate(20.0f, 0.0f, 0.0f);
					result2.AddRange(modelbridge2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_obj_beach, meshbridge2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
					// 2
					transform.NJTranslate(20.0f, 0.0f, 0.0f);
					result2.AddRange(modelbridge3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_obj_beach, meshbridge3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				}
				// Small chunk
				transform.NJTranslate(20.0f, 0.0f, 0.0f);
				result2.AddRange(modelbridge2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_obj_beach, meshbridge2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				// Main part 2
				transform.NJTranslate(95.0f, -9.5f, 0.0f); // Y is 0.0 in original code at 0x501AD6, no idea why
				result2.AddRange(modelbridge4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_obj_beach, meshbridge4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
				RenderInfo.Draw(result2, dev, cam);
			}
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			texs_beach_sea = ObjectHelper.GetTextures("BEACH_SEA");
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			dev.SetRenderState(RenderState.ZWriteEnable, false);
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
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_beach_sea?[water.TextureWaves], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
					}
					transform.NJTranslate(water.WrapXZ, 0, 0);
				}
				transform.Pop();
				transform.Push();
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_beach_sea?[water.TextureSea], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
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
						Center = new Vertex(-3000.0f, 0.0f, -1000.0f),
						WrapX = 90,
						WrapXZ = 128.0f,
						WrapZ = 35,
						TextureSea = 15,
					});
					SADXOceanData.InitWaterSurface(0);
					break;
				case 1:
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(700.0f, -1.5f, -4850.0f),
						WrapX = 90,
						WrapXZ = 128.0f,
						WrapZ = 35,
						TextureSea = 15,
					});
					SADXOceanData.InitWaterSurface(0);
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(-630.0f, 540.0f, -2160.0f),
						WrapX = 27,
						WrapXZ = 64.0f,
						WrapZ = 23,
						TextureSea = 16,
					});
					SADXOceanData.InitWaterSurface(1);
					break;
				case 2:
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(3500.0f, -1.5f, -500.0f),
						WrapX = 30,
						WrapXZ = 128.0f,
						WrapZ = 30,
						TextureSea = 15,
					});
					SADXOceanData.InitWaterSurface(0);
					SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
					{
						Center = new Vertex(5845.0f, -105.0f, 2050.0f),
						WrapX = 10,
						WrapXZ = 64.0f,
						WrapZ = 10,
						TextureSea = 16,
					});
					SADXOceanData.InitWaterSurface(1);
					break;
			}
		}
	}
}