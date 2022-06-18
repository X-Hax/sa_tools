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
	class WindyValley3Autodemo : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[3];
		readonly Mesh[][] meshes = new Mesh[3][];
		Texture[] texs_sora62;
		Texture[] texs_sora96;
		private NJS_TEXLIST texlist_sora62; // Bottom clouds
		private NJS_TEXLIST texlist_sora96; // Main sky

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			texlist_sora96 = NJS_TEXLIST.Load("stg02_windy/bg/models/windy_back3a.satex");
			texlist_sora62 = NJS_TEXLIST.Load("stg02_windy/bg/models/windy_back3b.satex");
			for (int i = 0; i < 3; i++)
			{
				models[i] = ObjectHelper.LoadModel("stg02_windy/bg/models/act3nbg_nbg" + (i + 1).ToString(NumberFormatInfo.InvariantInfo) + ".nja.sa1mdl");
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs_sora62 == null)
				texs_sora62 = ObjectHelper.GetTextures("WINDY_BACK3", texlist_sora62, dev);
			if (texs_sora96 == null)
				texs_sora96 = ObjectHelper.GetTextures("WINDY_BACK3", texlist_sora96, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			result.AddRange(models[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_sora96, meshes[0], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(models[1].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_sora62, meshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(models[2].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_sora62, meshes[2], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}