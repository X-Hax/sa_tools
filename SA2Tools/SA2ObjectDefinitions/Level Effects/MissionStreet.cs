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
	class MissionStreet : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		NJS_TEXLIST skyboxtls;
		Vector3 Skybox_Scale;
		Texture[] texs1;
		
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg17_Mission/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtls = NJS_TEXLIST.Load("stg17_Mission/tls/Skybox.satex");
			Skybox_Scale.X = 1.0f;
			Skybox_Scale.Y = 1.0f;
			Skybox_Scale.Z = 1.0f;
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
			texs1 = ObjectHelper.GetTextures("bgtex17", skyboxtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y + 886.5248f, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}