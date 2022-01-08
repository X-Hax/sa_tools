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
		Texture[] texs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
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
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs = ObjectHelper.GetTextures("SS_BG");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}