using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;

namespace SADXObjectDefinitions.Level_Effects
{
	class ChaoGarden : LevelDefinition
	{
		bool IsMRGarden;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			if (data.LevelID == "4100")
				IsMRGarden = true;
			SetOceanData();
		}

		private void RenderWater(Device dev, EditorCamera cam)
		{
			Texture[] texs_ocean;
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			if (!IsMRGarden)
				texs_ocean = ObjectHelper.GetTextures("GARDEN00SSOBJ");
			else
			{
				texs_ocean = ObjectHelper.GetTextures("GARDEN02MR_DAYTIME");
				if (texs_ocean == null)
					texs_ocean = ObjectHelper.GetTextures("GARDEN02MR_EVENING");
				if (texs_ocean == null)
					texs_ocean = ObjectHelper.GetTextures("GARDEN02MR_NIGHT");
			}
			List<RenderInfo> result3 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
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
						result3.Add(new RenderInfo(water.Meshes[z], 0, transform.Top, SADXOceanData.Material, texs_ocean?[water.TextureSea], dev.GetRenderState<FillMode>(RenderState.FillMode), water.Bounds));
					}
					transform.NJTranslate(water.WrapXZ, 0, 0);
				}
				transform.Pop();
				transform.Pop();
			}
			RenderInfo.Draw(result3, dev, cam);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (IsMRGarden)
				RenderWater(dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (!IsMRGarden)
				RenderWater(dev, cam);
		}

		private void SetOceanData()
		{
			Vertex center = new Vertex(-260.0f, 0.6f, -160.0f);
			int textureSea = 11;
			if (IsMRGarden)
			{
				center = new Vertex(-180.0f, -0.4f, -130.0f);
				textureSea = 43;
			}
			SADXOceanData.Initialize();
			SADXOceanData.WaterSurfaces.Add(new SADXOceanData.WaterSurfaceData()
			{
				Center = center,
				WrapX = 10,
				WrapZ = 10,
				WrapXZ = 32.0f,
				TextureSea = (ushort)textureSea
			});
			SADXOceanData.InitWaterSurface(0);
		}
	}
}