using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SA_Tools;
using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SADXObjectDefinitions.Level_Effects
{
	class WindyValley1 : LevelDefinition
	{
		NJS_OBJECT[] models = new NJS_OBJECT[5];
		Mesh[][] meshes = new Mesh[5][];
		Vector3 Skybox_Scale;

		public override void Init(IniLevelData data, byte act, Device dev)
		{
            string filePath = "Levels/Windy Valley/Skybox Data.ini";

            Environment.CurrentDirectory = EditorOptions.ProjectPath; // look in our project folder first
            if (!File.Exists(filePath)) Environment.CurrentDirectory = EditorOptions.GamePath; // look in our fallback if it doesn't exist (probably won't happen in these cases, though.)

            SkyboxScale[] skyboxdata = SkyboxScaleList.Load(filePath);
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			for (int i = 0; i < 5; i++)
			{
				models[i] = ObjectHelper.LoadModel("Levels/Windy Valley/Act 1/Skybox model " + (i + 1).ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl");
				meshes[i] = ObjectHelper.GetMeshes(models[i], dev);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(Skybox_Scale);
			Texture[] texs = ObjectHelper.GetTextures("WINDY_BACK");
			for (int i = 0; i < 5; i++)
				result.AddRange(models[i].DrawModelTree(dev, transform, texs, meshes[i]));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}
