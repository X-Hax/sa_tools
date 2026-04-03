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
	class DowntownRace : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Texture[] texs1;
		NJS_TEXLIST skyboxtexlist;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg52_CityEscape2P/models/GC/Skybox.sa2bmdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtexlist = NJS_TEXLIST.Load("stg52_CityEscape2P/tls/Skybox.satex");
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("bgtex52"), mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}