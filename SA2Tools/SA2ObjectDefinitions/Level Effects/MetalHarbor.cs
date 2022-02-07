using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

//to do: add WATER
namespace SA2ObjectDefinitions.Level_Effects
{
	class MetalHarbor : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Vector3 Skybox_Scale;
		Texture[] texs1;
		NJS_OBJECT model2;
		Mesh[] mesh2;
		float waterPos = -329.0582f;


		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg10_metalharbor/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("stg10_metalharbor/models/Water.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			Skybox_Scale.X = 1.0f;
			Skybox_Scale.Y = 1.0f;
			Skybox_Scale.Z = 1.0f;
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs1 = ObjectHelper.GetTextures("bgtex10");
			List<RenderInfo> result1 = new List<RenderInfo>();
			List<RenderInfo> result2 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, -0.1f, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}