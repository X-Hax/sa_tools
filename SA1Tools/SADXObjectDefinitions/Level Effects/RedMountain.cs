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
	class RedMountain : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[4];
		readonly Mesh[][] meshes = new Mesh[4][];
		Vector3 Skybox_Scale;
		byte Act;
		Texture[] texs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg05_mountain/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			for (int i = 0; i < 4; i++)
			{
				models[i] = ObjectHelper.LoadModel("stg05_mountain/common/models/soto_fr_sora" + (i + 1).ToString() + "a.nja.sa1mdl");
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
			texs = ObjectHelper.GetTextures("OBJ_MOUNTAIN");
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			float x;
			float y;
			float z;
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			switch (Act)
			{
				case 0:
				default:
					x = 0;
					y = 0;
					z = 0;
					break;
				case 1:
					x = -500.0f;
					y = 700.0f;
					z = 3000.0f;
					break;
				case 2:
					x = 0;
					y = 0;
					z = 100;
					break;
			}
			transform.NJTranslate(x, y, z);
			transform.NJScale(Skybox_Scale);
			for (int i = 0; i < 4; i++)
				result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i]));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}