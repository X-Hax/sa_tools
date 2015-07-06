using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SA_Tools;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.Level_Effects
{
	class EmeraldCoast : LevelDefinition
	{
		NJS_OBJECT model1, model2;
		Mesh[] mesh1, mesh2;
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act, Device dev)
		{
            string filePath = "Levels/Emerald Coast/Skybox Data.ini";

            Environment.CurrentDirectory = EditorOptions.ProjectPath; // look in our project folder first
            if (!File.Exists(filePath)) Environment.CurrentDirectory = EditorOptions.GamePath; // look in our fallback if it doesn't exist (probably won't happen in these cases, though.)

            SkyboxScale[] skyboxdata = SkyboxScaleList.Load(filePath);
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			model1 = ObjectHelper.LoadModel("Levels/Emerald Coast/Skybox model.sa1mdl");
			mesh1 = ObjectHelper.GetMeshes(model1, dev);
			model2 = ObjectHelper.LoadModel("Levels/Emerald Coast/Skybox bottom model.sa1mdl");
			mesh2 = ObjectHelper.GetMeshes(model2, dev);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			Texture[] texs = ObjectHelper.GetTextures("BG_BEACH");
			result.AddRange(model1.DrawModelTree(dev, transform, texs, mesh1));
			result.AddRange(model2.DrawModelTree(dev, transform, texs, mesh2));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}