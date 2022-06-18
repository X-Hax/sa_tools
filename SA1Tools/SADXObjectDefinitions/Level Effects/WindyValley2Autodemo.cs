using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;

namespace SADXObjectDefinitions.Level_Effects
{
	class WindyValley2Autodemo : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[3];
		readonly Mesh[][] meshes = new Mesh[3][];
		private NJS_TEXLIST texlist_main;
		private NJS_TEXLIST texlist_floating;
		Texture[] texs_main;
		Texture[] texs_floating;
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			texlist_main = NJS_TEXLIST.Load("stg02_windy/bg/models/newmind02_kazea.satex");
			texlist_floating = NJS_TEXLIST.Load("stg02_windy/bg/models/newmind02_kazeb.satex");
			models[0] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kazea.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kazeb.nja.sa1mdl");
			models[2] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kazec.nja.sa1mdl");
			for (int i = 0; i < models.Length; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs_main == null)
				texs_main = ObjectHelper.GetTextures("WINDY_BACK2", texlist_main, dev);
			if (texs_floating == null)
				texs_floating = ObjectHelper.GetTextures("WINDY_BACK2", texlist_floating, dev);
			dev.SetRenderState(RenderState.ZWriteEnable, true);
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(650, -360, -200);
			result.AddRange(models[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_main, meshes[0], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(models[1].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_floating, meshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(models[2].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs_floating, meshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			RenderInfo.Draw(result, dev, cam);
		}
	}
}