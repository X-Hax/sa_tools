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
	class CrazyGadget : LevelDefinition
	{
		NJS_OBJECT platemodel;
		Mesh[] platemesh;
		NJS_TEXLIST platetls;
		Texture[] platetexs;
		Vector3[] platepos = { new Vector3(-5455.0f, -1187.9f, -3083.5f), new Vector3(-5455.0f, -1187.9f, -3326.5f), new Vector3(-1050.0f, -80.0f, -277.5f), new Vector3(-1050.0f, -80.0f, -522.5f) };
		int[] plateyrot = { 0x8000, 0, 0x8000, 0 };

		NJS_OBJECT spacemodel;
		Mesh[] spacemesh;
		NJS_TEXLIST spacetls;
		Texture[] spacetexs;
		List<NJS_OBJECT> acidmodellist = [];
		List<Mesh[]> acidmesheslist = [];
		NJS_OBJECT[] acidmodels = [];
		Mesh[][] acidmeshes = [];

		NJS_TEXLIST acidtls;
		Texture[] acidtexs;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			platemodel = ObjectHelper.LoadModel("stg22_CrazyGadget/models/Plating.sa2mdl");
			platemesh = ObjectHelper.GetMeshes(platemodel);
			platetls = NJS_TEXLIST.Load("stg22_CrazyGadget/tls/Plating.satex");
			spacemodel = ObjectHelper.LoadModel("stg22_CrazyGadget/models/Space.sa2mdl");
			spacemesh = ObjectHelper.GetMeshes(spacemodel);
			spacetls = NJS_TEXLIST.Load("stg22_CrazyGadget/tls/Space.satex");
			for (int i = 0; i < 22; i++)
			{
				acidmodellist.Add(ObjectHelper.LoadModel($"stg22_CrazyGadget/models/Acid{i + 1}.sa2mdl"));
			}
			acidmodels = acidmodellist.ToArray();
			for (int i = 0; i < 22; i++)
			{
				acidmesheslist.Add(ObjectHelper.GetMeshes(acidmodels[i]));
			}
			acidmeshes = acidmesheslist.ToArray();
			acidtls = NJS_TEXLIST.Load("stg22_CrazyGadget/tls/Acid.satex");
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (spacetexs == null)
			spacetexs = ObjectHelper.GetTextures("bgtex22", spacetls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position);
			result1.AddRange(spacemodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, spacetexs, spacemesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}

		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (platetexs == null)
				platetexs = ObjectHelper.GetTextures("bgtex22", platetls, dev);
			if (acidtexs == null)
				acidtexs = ObjectHelper.GetTextures("bgtex22", acidtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			for (int i = 0; i < platepos.Length; i++)
			{
				transform.Push();
				transform.NJTranslate(platepos[i]);
				transform.NJRotateY(plateyrot[i]);
				result1.AddRange(platemodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, platetexs, platemesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			for (int i = 0; i < acidmodels.Length; i++)
			{
				transform.Push();
				result1.AddRange(acidmodels[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, acidtexs, acidmeshes[i], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}