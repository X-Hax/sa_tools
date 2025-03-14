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
	class WildCanyon : LevelDefinition
	{
		NJS_OBJECT model1;
		NJS_OBJECT model2;
		NJS_OBJECT model3;
		NJS_OBJECT model4;
		Mesh[] mesh1;
		Mesh[] mesh2;
		Mesh[] mesh3;
		Mesh[] mesh4;
		Vector3 Skybox_Scale;
		Texture[] texs1;
		Texture[] texs2;
		Texture[] texs3;
		NJS_TEXLIST skyboxtls;
		NJS_TEXLIST tunneltls;
		NJS_TEXLIST lighttls;
		
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg16_WildCanyon/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtls = NJS_TEXLIST.Load("stg16_WildCanyon/tls/Skybox.satex");
			model2 = ObjectHelper.LoadModel("stg16_WildCanyon/models/WindTunnel1.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			tunneltls = NJS_TEXLIST.Load("stg16_WildCanyon/tls/WindTunnel.satex");
			model3 = ObjectHelper.LoadModel("stg16_WildCanyon/models/WindTunnel2.sa2mdl");
			mesh3 = ObjectHelper.GetMeshes(model3);
			model4 = ObjectHelper.LoadModel("stg16_WildCanyon/models/LightPillar.sa2mdl");
			mesh4 = ObjectHelper.GetMeshes(model4);
			lighttls = NJS_TEXLIST.Load("stg16_WildCanyon/tls/LightPillar.satex");
			Skybox_Scale.X = 1.0f;
			Skybox_Scale.Y = 1.0f;
			Skybox_Scale.Z = 1.0f;
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			texs1 = ObjectHelper.GetTextures("landtx16", skyboxtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Skybox
			transform.Push();
			//transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			texs2 = ObjectHelper.GetTextures("stg16_wind", tunneltls, dev);
			texs3 = ObjectHelper.GetTextures("bgtex16", lighttls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Wind Tunnel 1
			transform.Push();
			transform.NJTranslate(200.0f, 250.0f, -4.0f);
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Wind Tunnel 2
			transform.Push();
			transform.NJTranslate(200.0f, 902.02002f, -4.0f);
			result1.AddRange(model3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Light Pillar
			transform.Push();
			transform.NJTranslate(220.0f, 320.0f, -30.0f);
			result1.AddRange(model4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs3, mesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}