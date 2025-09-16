using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

//to do: add WATER
namespace SA2ObjectDefinitions.Level_Effects
{
	class MetalHarbor : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Texture[] texs1;
		NJS_OBJECT model2;
		NJS_TEXLIST watertls;
		Mesh[] mesh2;
		Texture[] texs2;
		//float waterYPos = -329.65820002f;


		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg10_metalharbor/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			model2 = ObjectHelper.LoadModel("CartCommon/models/Water.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			// Temporary, until End User puts in the real texlist used for this level
			watertls = NJS_TEXLIST.Load("CartCommon/tls/Water.satex");
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("bgtex10");
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, -0.1f, cam.Position.Z);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (texs2 == null)
				texs2 = ObjectHelper.GetTextures("bgtex10", watertls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			//transform.NJScale(Skybox_Scale);
			result1.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}