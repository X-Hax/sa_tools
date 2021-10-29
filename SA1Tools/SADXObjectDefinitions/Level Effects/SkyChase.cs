using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class SkyChase : LevelDefinition
	{
		NJS_OBJECT carriermdl;
		Mesh[] carriermesh;

		public override void Init(IniLevelData data, byte act)
		{
			carriermdl = ObjectHelper.LoadModel("shooting/common/models/shot_bf_s_bodya.nja.sa1mdl");
			carriermesh = ObjectHelper.GetMeshes(carriermdl);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			Texture[] texs = ObjectHelper.GetTextures("SHOOTING0");
			carriermdl.ProcessTransforms(transform);
			carriermdl.ProcessVertexData();
			dev.SetRenderState(RenderState.ZWriteEnable, true); // Z write is disabled for skybox by default
			result.AddRange(carriermdl.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, carriermesh, boundsByMesh: true));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam, true);
		}
	}
}