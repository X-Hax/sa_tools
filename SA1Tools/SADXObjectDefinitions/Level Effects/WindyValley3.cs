using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using System.Globalization;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class WindyValley3 : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[4];
		readonly Mesh[][] meshes = new Mesh[4][];
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg02_windy/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			for (int i = 0; i < 4; i++)
			{
				models[i] = ObjectHelper.LoadModel("stg02_windy/bg/models/act3nbg_nbg" + (i + 1).ToString(NumberFormatInfo.InvariantInfo) + ".nja.sa1mdl");
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			transform.NJScale(Skybox_Scale);
			Texture[] texs = ObjectHelper.GetTextures("WINDY_BACK3");
			for (int i = 0; i < 4; i++)
				result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i]));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}
