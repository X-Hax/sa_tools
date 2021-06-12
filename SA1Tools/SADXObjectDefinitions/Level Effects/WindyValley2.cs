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
	class WindyValley2 : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[3];
		readonly Mesh[][] meshes = new Mesh[3][];

		public override void Init(IniLevelData data, byte act)
		{
			for (char i = 'a'; i < 'c'; i++)
			{
				models[i] = ObjectHelper.LoadModel("stg02_windy/bg/models/newmind02_kaze" + (i) + ".nja.sa1mdl");
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			Texture[] texs = ObjectHelper.GetTextures("WINDY_BACK2");
			for (int i = 0; i < 3; i++)
				result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i]));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}
