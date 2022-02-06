using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class PerfectChaos : LevelDefinition
	{
		readonly NJS_OBJECT[] models = new NJS_OBJECT[2];
		readonly Mesh[][] meshes = new Mesh[2][];
		Texture[] texs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			models[0] = ObjectHelper.LoadModel("boss_chaos7/object/models/last1a_nc_sky.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("boss_chaos7/object/models/lastmap_bf_watera.nja.sa1mdl");
			for (int i = 0; i < 2; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			texs = ObjectHelper.GetTextures("OBJ_CHAOS7");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, 0, cam.Position.Z);
			result.AddRange(models[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[0], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			texs = ObjectHelper.GetTextures("OBJ_CHAOS7");
			transform.Push();
			result.AddRange(models[1].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}