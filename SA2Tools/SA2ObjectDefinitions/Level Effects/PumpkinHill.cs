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
	class PumpkinHill : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Vector3 Skybox_Scale;
		NJS_TEXLIST skyboxtls;
		Texture[] texs1;
		NJS_OBJECT model2;
		Mesh[] mesh2;
		NJS_TEXLIST abysstex1;
		Texture[] texs2;
		NJS_OBJECT model3;
		Mesh[] mesh3;
		NJS_TEXLIST abysstex2;
		Texture[] texs3;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg05_pumpkin/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtls = NJS_TEXLIST.Load("stg05_pumpkin/tls/Skybox.satex");
			model2 = ObjectHelper.LoadModel("stg05_pumpkin/models/AbyssFog.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			abysstex1 = NJS_TEXLIST.Load("stg05_pumpkin/tls/AbyssFog.satex");
			model3 = ObjectHelper.LoadModel("stg05_pumpkin/models/AbyssFog2.sa2mdl");
			mesh3 = ObjectHelper.GetMeshes(model3);
			abysstex2 = NJS_TEXLIST.Load("stg05_pumpkin/tls/AbyssFog2.satex");
			Skybox_Scale.X = 1.0f;
			Skybox_Scale.Y = 1.0f;
			Skybox_Scale.Z = 1.0f;
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
			texs1 = ObjectHelper.GetTextures("bgtex05", skyboxtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0.0f, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (texs2 == null)
			texs2 = ObjectHelper.GetTextures("bgtex05", abysstex1, dev);
			if (texs3 == null)
			texs3 = ObjectHelper.GetTextures("bgtex05", abysstex2, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0.0f, cam.Position.Z);
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result1.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs3, mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}