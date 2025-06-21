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
	class EternalEngine : LevelDefinition
	{
		NJS_OBJECT spacemodel1;
		Mesh[] spacemesh1;
		NJS_TEXLIST spacetls;
		NJS_OBJECT spacemodel2;
		Mesh[] spacemesh2;
		Texture[] spacetexs;
		
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			spacemodel1 = ObjectHelper.LoadModel("stg24_EternalEnginE/models/Space.sa2mdl");
			spacemesh1 = ObjectHelper.GetMeshes(spacemodel1);
			spacemodel2 = ObjectHelper.LoadModel("stg24_EternalEnginE/models/TransparentSpace.sa2mdl");
			spacemesh2 = ObjectHelper.GetMeshes(spacemodel2);
			spacetls = NJS_TEXLIST.Load("stg24_EternalEnginE/tls/Space.satex");
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (spacetexs == null)
			spacetexs = ObjectHelper.GetTextures("bgtex24", spacetls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			result1.AddRange(spacemodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs, spacemesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (spacetexs == null)
				spacetexs = ObjectHelper.GetTextures("bgtex24", spacetls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			result1.AddRange(spacemodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs, spacemesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}