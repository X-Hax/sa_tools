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
	class CityEscape : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Vector3 Skybox_Scale;
		Texture[] texs1;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/Skybox.sa2bmdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			Skybox_Scale.X = 3.5f;
			Skybox_Scale.Y = 3.5f;
			Skybox_Scale.Z = 3.5f;
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs1 = ObjectHelper.GetTextures("bgtex13");
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}