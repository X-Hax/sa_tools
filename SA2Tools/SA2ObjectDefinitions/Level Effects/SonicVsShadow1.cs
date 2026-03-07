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
	class SonicVsShadow1 : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		NJS_TEXLIST skyboxtls;
		Texture[] texs1;
		NJS_OBJECT model2;
		Mesh[] mesh2;
		NJS_TEXLIST watertls;
		Texture[] texs2;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg19_sonicvsshadow/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtls = NJS_TEXLIST.Load("stg19_sonicvsshadow/tls/Skybox.satex");
			model2 = ObjectHelper.LoadModel("stg19_sonicvsshadow/models/Water.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			watertls = NJS_TEXLIST.Load("stg19_sonicvsshadow/tls/Water.satex");
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("bgtex03", skyboxtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 920.0f, cam.Position.Z);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (texs2 == null)
				texs2 = ObjectHelper.GetTextures("landtx19_suimen", watertls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 920.0f, cam.Position.Z);
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}