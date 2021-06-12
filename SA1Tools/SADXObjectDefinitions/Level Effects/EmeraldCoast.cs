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
		NJS_OBJECT model1, model2;
		Mesh[] mesh1, mesh2;
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg01_beach/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model1 = ObjectHelper.LoadModel("stg01_beach/bg/models/sea_nbg.nja.sa1mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("stg01_beach/bg/models/sea_nbg3.nja.sa1mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			Texture[] texs = ObjectHelper.GetTextures("BG_BEACH");
			result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, mesh1));
			result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, mesh2));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}