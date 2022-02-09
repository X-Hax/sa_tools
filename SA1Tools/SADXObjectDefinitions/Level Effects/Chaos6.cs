using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class Chaos6 : LevelDefinition
	{
		protected NJS_OBJECT model1, model2;
		protected Mesh[] mesh1, mesh2;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("boss_chaos6/common/objmodels/ecsc_s_sora.nja.sa1mdl");
			model2 = ObjectHelper.LoadModel("boss_chaos6/common/objmodels/ecsc_s_sitakumo.nja.sa1mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			mesh2 = ObjectHelper.GetMeshes(model2);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Sky
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJRotateY(0xE000);
			result.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("CHAOS6_BG"), mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Moving cloud
			transform.Push();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			dev.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			dev.SetRenderState(RenderState.DestinationBlend, Blend.One);
			transform.NJTranslate(cam.Position.X, 2300.0f, cam.Position.Z); // Absolutely wrong but accurate to SADX!
			transform.NJScale(3.0f, 1.0f, 3.0f);
			result.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("CHAOS6_BG"), mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			dev.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			dev.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}