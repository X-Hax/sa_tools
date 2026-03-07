using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Level_Effects
{
	class GreenForest : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Vector3 Skybox_Scale;
		Texture[] texs1;
		NJS_OBJECT model2;
		Mesh[] mesh2;
		NJS_OBJECT model3;
		Mesh[] mesh3;
		NJS_TEXLIST watertls;
		Texture[] texs2;
		float waterPos = -1356.910034179688f;
		
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg03_Jungle/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("stg03_Jungle/models/Water.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			model3 = ObjectHelper.LoadModel("stg03_Jungle/models/Puddles.sa2mdl");
			mesh3 = ObjectHelper.GetMeshes(model3);
			watertls = NJS_TEXLIST.Load("stg03_Jungle/texlist_landtx03_suimen.satex");
			Skybox_Scale.X = 1.0f;
			Skybox_Scale.Y = 1.0f;
			Skybox_Scale.Z = 1.0f;
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
			texs1 = ObjectHelper.GetTextures("bgtex03");
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (texs2 == null)
				texs2 = ObjectHelper.GetTextures("landtx03_suimen", watertls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, waterPos, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			result1.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}