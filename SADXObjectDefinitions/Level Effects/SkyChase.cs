using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using Mesh = Microsoft.DirectX.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class SkyChase : LevelDefinition
	{
		NJS_OBJECT carriermdl;
		Mesh[] carriermesh;

		public override void Init(IniLevelData data, byte act, Device dev)
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