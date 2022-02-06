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
	class EggHornet : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Vector3 Skybox_Scale;
		Texture[] texs, texs_egm1land;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("bossegm1/bg/bgScale.ini");
			Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model = ObjectHelper.LoadModel("bossegm1/models/xmra_s_sora_hare.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			SetOceanData();
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (model == null)
				return;
			texs = ObjectHelper.GetTextures("EGM1SORA");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			texs_egm1land = ObjectHelper.GetTextures("EGM1LAND");
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			dev.SetRenderState(RenderState.ZWriteEnable, false);
			for (int o = 0; o < SADXOceanData.WaterSurfaces.Count; o++)
			{
				SADXOceanData.WaterSurfaceData water = SADXOceanData.WaterSurfaces[o];
				transform.Push();
				transform.NJTranslate(water.Center);
				transform.NJTranslate(0, -40, 0);
				transform.Push();
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_egm1land?[water.TextureWaves], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
					}
					transform.NJTranslate(water.WrapXZ, 0, 0);
				}
				transform.Pop();
				transform.Push();
				for (int i = 0; i < water.WrapX; i++)
				{
					for (int z = 0; z < water.WrapZ; z++)
					{
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_egm1land?[water.TextureSea], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
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
				Center = new Vertex(-4000.0f, -418.5f, 0.0f),
				WrapX = 90,
				WrapZ = 20,
				WrapXZ = 256.0f,
				TextureSea = 101,
				TextureWaves = 100
			});
			SADXOceanData.InitWaterSurface(0);
		}
	}
}