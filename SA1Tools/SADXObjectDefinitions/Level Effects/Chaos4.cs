using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class Chaos4 : LevelDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] mesh;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model = ObjectHelper.LoadModel("boss_chaos4/common/models/obj/c4_s_sora_hare.nja.sa1mdl");
			mesh = ObjectHelper.GetMeshes(model);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("CHAOS4_OBJECT"), mesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}