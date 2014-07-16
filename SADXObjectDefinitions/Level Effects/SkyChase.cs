using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SA_Tools;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.Level_Effects
{
	class SkyChase : LevelDefinition
	{
		SonicRetro.SAModel.Object carriermdl;
		Mesh[] carriermesh;

		public override void Init(Dictionary<string, string> data, byte act, Device dev)
		{
			carriermdl = ObjectHelper.LoadModel("Levels/Sky Chase/Egg Carrier model.sa1mdl");
			carriermesh = ObjectHelper.GetMeshes(carriermdl, dev);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			Texture[] texs = ObjectHelper.GetTextures("SHOOTING0");
			result.AddRange(carriermdl.DrawModelTree(dev, transform, texs, carriermesh));
			RenderInfo.Draw(result, dev, cam);
		}
	}
}