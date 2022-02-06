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
	class SandHill : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Texture[] texs;
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("sandboard/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model = ObjectHelper.LoadModel("sandboard/bg/models/sunabo_fr_tenkyu.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs = ObjectHelper.GetTextures("BG_SANDBOARD");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}