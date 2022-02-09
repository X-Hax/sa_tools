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
	class LostWorld : LevelDefinition
	{
		byte Act;
		readonly NJS_OBJECT[] models = new NJS_OBJECT[2];
		readonly Mesh[][] meshes = new Mesh[2][];
		Texture[] texs;
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg07_ruin/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			models[0] = ObjectHelper.LoadModel("stg07_ruin/bg/models/lost2_nbg_1.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("stg07_ruin/bg/models/lost2_nbg2_1.nja.sa1mdl");
			for (int i = 0; i < 2; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (Act == 0 || Act == 2)
				return;
			texs = ObjectHelper.GetTextures("BG_RUIN");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			for (int i = 0; i < 2; i++)
				result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}