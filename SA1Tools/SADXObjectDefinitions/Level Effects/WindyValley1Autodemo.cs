using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using System.Globalization;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class WindyValley1Autodemo : LevelDefinition
	{
		private NJS_TEXLIST texlist_main;
		private NJS_TEXLIST texlist_clouds;
		private NJS_OBJECT[] models = new NJS_OBJECT[3];
		private Mesh[][] meshes = new Mesh[3][];
		private Texture[] texs_main;
		private Texture[] texs_clouds;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			texlist_main = NJS_TEXLIST.Load("stg02_windy/bg/models/windy_back1a.satex");
			texlist_clouds = NJS_TEXLIST.Load("stg02_windy/bg/models/windy_back1b.satex");
			for (int i = 0; i < 3; i++)
			{
				models[i] = ObjectHelper.LoadModel("stg02_windy/bg/models/windy01_nbg" + (i + 1).ToString(NumberFormatInfo.InvariantInfo) + ".nja.sa1mdl");
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs_main == null)
				texs_main = ObjectHelper.GetTextures("WINDY_BACK", texlist_main, dev);
			if (texs_clouds == null)
				texs_clouds = ObjectHelper.GetTextures("WINDY_BACK", texlist_clouds, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			transform.NJScale(1.4f, 1.4f, 1.4f);
			// Bottom
			transform.Push();
			transform.NJScale(1.25f, 1.0f, 1.25f);
			transform.NJTranslate(0, models[2].Position.Y, 0);
			result.AddRange(models[2].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_main, meshes[2], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Main skybox
			result.AddRange(models[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_main, meshes[0], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			// Clouds
			transform.Push();
			transform.NJTranslate(0, models[1].Position.Y, 0);
			result.AddRange(models[1].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_clouds, meshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}