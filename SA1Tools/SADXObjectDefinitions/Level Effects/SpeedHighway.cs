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
	class SpeedHighway : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[2];
		readonly Mesh[][] meshes = new Mesh[2][];
		readonly Texture[][] textures = new Texture[2][];
		Vector3 Skybox_Scale;
		byte Act;

		public override void Init(IniLevelData data, byte act)
		{
			Act = act;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg04_highway/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			switch (act)
			{
				case 0:
				default:
					models[0] = ObjectHelper.LoadModel("stg04_highway/bg/models/s1_nbg1.nja.sa1mdl");
					models[1] = ObjectHelper.LoadModel("stg04_highway/bg/models/nbg1_yakeishita.nja.sa1mdl");
					break;
				case 1:
					models[0] = ObjectHelper.LoadModel("stg04_highway/bg/models/s1_nbg1.nja.sa1mdl");
					models[1] = ObjectHelper.LoadModel("stg04_highway/bg/models/s2_yakei.nja.sa1mdl");
					break;
				case 2:
					models[0] = ObjectHelper.LoadModel("stg04_highway/bg/models/s3_nbg3.nja.sa1mdl");
					models[1] = null;
					break;
			}
			for (int i = 0; i < 2; i++)
			{
				if (models[i] != null)
					meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (Act == 1 && cam.Position.Y <= -10400.0f)
				return;
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			switch (Act)
			{
				case 0:
				default:
					textures[0] = ObjectHelper.GetTextures("BG_HIGHWAY");
					textures[1] = ObjectHelper.GetTextures("BG_HIGHWAY01");
					break;
				case 1:
					textures[0] = ObjectHelper.GetTextures("BG_HIGHWAY");
					textures[1] = ObjectHelper.GetTextures("BG_HIGHWAY02");
					break;
				case 2:
					textures[0] = ObjectHelper.GetTextures("BG_HIGHWAY03");
					textures[1] = null;
					break;
			}
			for (int i = 0; i < 2; i++)
				if (models[i] != null)
					result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, textures[i], meshes[i]));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}