using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class ZeroE101R : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Texture[] texs_bg, texs_ocean;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			if (data.LevelID == "2300")
				model = ObjectHelper.LoadModel("bossrobo/common/bgmodels/ecsc_s_sora_hare.nja.sa1mdl");
			else
				model = ObjectHelper.LoadModel("boss_e101_r/common/bgmodels/ecsc_s_sora_hare.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			SetOceanData();
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			MatrixStack transform = new MatrixStack();
			texs_bg = ObjectHelper.GetTextures("E101R_BG");
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_bg, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			texs_ocean = ObjectHelper.GetTextures("E101R_TIKEI");
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			for (int o = 0; o < SADXOceanData.WaterSurfaces.Count; o++)
			{
				SADXOceanData.WaterSurfaceData water = SADXOceanData.WaterSurfaces[o];
				transform.Push();
				transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
				transform.NJScale(2.0f, 1.0f, 2.0f);
				transform.NJTranslate(water.Center);
				transform.NJTranslate(0, -40, 0);
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
			SADXOceanData.Initialize();
			SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
			{
				Center = new Vertex(-4000.0f, 415.0f, -4000.0f),
				WrapX = 30,
				WrapZ = 30,
				WrapXZ = 512.0f,
				TextureSea = 79,
				TextureWaves = 78
			});
			SADXOceanData.InitWaterSurface(0);
		}
	}
}