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
	class PyramidCave : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		NJS_TEXLIST skyboxtls;
		Texture[] texs1;
		NJS_OBJECT model2;
		Mesh[] mesh2;
		NJS_TEXLIST neontls;
		Texture[] texs2;
		// Neon Light positions
		Vector3[] neonpositions = {
			new Vector3(-685.0f, -4200.0f, -19700.0f),
			new Vector3(-685.0f, -4230.0f, -19840.0f),
			new Vector3(-685.0f, -4260.0f, -19980.0f),
			new Vector3(-685.0f, -4290.0f, -20130.0f),
			new Vector3(-685.0f, -4240.0f, -21070.0f),
			new Vector3(-685.0f, -4230.0f, -21220.0f),
			new Vector3(-685.0f, -4140.0f, -21410.0f),
			new Vector3(-685.0f, -4040.0f, -21600.0f),
		};

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg28_PyramidCave/models/Skybox.sa2mdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtls = NJS_TEXLIST.Load("stg28_PyramidCave/tls/Skybox.satex");
			model2 = ObjectHelper.LoadModel("stg28_PyramidCave/models/NeonWalls.sa2mdl");
			mesh2 = ObjectHelper.GetMeshes(model2);
			neontls = NJS_TEXLIST.Load("stg28_PyramidCave/tls/NeonWalls.satex");
		}
		
		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("bgtex28", skyboxtls, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (texs2 == null)
				texs2 = ObjectHelper.GetTextures("bgtex28", neontls, dev);
			List<RenderInfo> result2 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			for (int i = 0; i < neonpositions.Length; i++)
			{
				transform.Push();
				transform.NJTranslate(neonpositions[i]);
				result2.AddRange(model2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs2, mesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			RenderInfo.Draw(result2, dev, cam);
		}
	}
}