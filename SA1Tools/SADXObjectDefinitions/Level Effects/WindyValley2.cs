using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class WindyValley2 : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[3];
		readonly Mesh[][] meshes = new Mesh[3][];
		Texture[] texs;
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			models[0] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kazea.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kazeb.nja.sa1mdl");
			models[2] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kazec.nja.sa1mdl");
			for (int i = 0; i < models.Length; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs = ObjectHelper.GetTextures("WINDY_BACK2");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			for (int i = 0; i < 3; i++)
				result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}