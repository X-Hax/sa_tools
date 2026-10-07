using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Level_Effects
{
	class CityEscape : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Texture[] texs1;
		NJS_TEXLIST skyboxtexlist;
		protected NJS_OBJECT[] treemodels;
		protected Mesh[][] treemeshes;
		Vector3[] treepositions;
		Texture[] treetexs;
		NJS_TEXLIST treetexlist;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/Skybox.sa2bmdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtexlist = NJS_TEXLIST.Load("stg13_cityescape/tls/Skybox.satex");

			treemodels = new NJS_OBJECT[5];
			treemeshes = new Mesh[5][];
			treepositions = new Vector3[5];

			treemodels[0] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree1.sa2bmdl");
			treemodels[1] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree2.sa2bmdl");
			treemodels[2] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree3.sa2bmdl");
			treemodels[3] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree4.sa2bmdl");
			treemodels[4] = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree5.sa2bmdl");

			treepositions[0] = new Vector3(6745.0f, -11102.9f, 4725.0f);
			treepositions[1] = new Vector3(6905.0f, -11216.9f, 4865.0f);
			treepositions[2] = new Vector3(7325.0f, -11420.9f, 5079.0f);
			treepositions[3] = new Vector3(7384.0f, -11389.9f, 4668.0f);
			treepositions[4] = new Vector3(6825.0f, -11184.9f, 5115.0f);
			for (int i = 0; i < 5; i++)
				treemeshes[i] = ObjectHelper.GetMeshes(treemodels[i]);

			treetexlist = NJS_TEXLIST.Load("stg13_cityescape/tls/DownhillTree1.satex");
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (texs1 == null)
				texs1 = ObjectHelper.GetTextures("bgtex13", skyboxtexlist, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			result1.AddRange(model1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs1, mesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			var camerarot = cam.Yaw;
			if (treetexs == null)
				treetexs = ObjectHelper.GetTextures("landtx13", treetexlist, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			for (int i = 0; i < 5; i++)
			{
				transform.Push();
				transform.NJTranslate(treepositions[i].X, treepositions[i].Y + 1.0f, treepositions[i].Z);
				result1.AddRange(treemodels[i].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemeshes[i][0], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (camerarot != 0)
					transform.NJRotateY(camerarot);
				result1.AddRange(treemodels[i].Children[0].DrawModel(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemeshes[i][1], true, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				transform.Pop();
			}
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}