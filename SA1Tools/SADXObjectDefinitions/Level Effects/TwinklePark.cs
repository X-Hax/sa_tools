using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class TwinklePark : LevelDefinition
	{
		NJS_OBJECT model;
		Mesh[] meshes;
		Vector3 Skybox_Scale;
		bool NoRender;

		public override void Init(IniLevelData data, byte act)
		{
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("shareobj/bg/bgScale_tp.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model = ObjectHelper.LoadModel("shareobj/bg/models/tp_nbg2.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
			NoRender = act == 1;
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (NoRender) return;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			transform.NJScale(Skybox_Scale);
			RenderInfo.Draw(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("BG_SHAREOBJ"), meshes), dev, cam);
			transform.Pop();
		}
	}
}