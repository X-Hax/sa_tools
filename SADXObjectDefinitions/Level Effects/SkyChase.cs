using SharpDX.Direct3D9;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SonicRetro.SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class SkyChase : LevelDefinition
	{
		NJS_OBJECT carriermdl;
		Mesh[] carriermesh;

		public override void Init(IniLevelData data, byte act)
		{
			carriermdl = ObjectHelper.LoadModel("Levels/Sky Chase/Egg Carrier model.sa1mdl");
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