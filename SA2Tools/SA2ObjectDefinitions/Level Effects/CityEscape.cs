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
	class CityEscape : LevelDefinition
	{
		NJS_OBJECT model1;
		Mesh[] mesh1;
		Texture[] texs1;
		NJS_TEXLIST skyboxtexlist;
		NJS_OBJECT treemodel1;
		Mesh[] treemesh1;
		Vector3 treepos1 = new Vector3(6745.0f, -11102.9f, 4725.0f); 
		NJS_OBJECT treemodel2;
		Mesh[] treemesh2;
		Vector3 treepos2 = new Vector3(6905.0f, -11216.9f, 4865.0f);
		NJS_OBJECT treemodel3;
		Mesh[] treemesh3;
		Vector3 treepos3 = new Vector3(7325.0f, -11420.9f, 5079.0f);
		NJS_OBJECT treemodel4;
		Mesh[] treemesh4;
		Vector3 treepos4 = new Vector3(7384.0f, -11389.9f, 4668.0f);
		NJS_OBJECT treemodel5;
		Mesh[] treemesh5;
		Vector3 treepos5 = new Vector3(6825.0f, -11184.9f, 5115.0f);
		Texture[] treetexs;
		NJS_TEXLIST treetexlist;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			model1 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/Skybox.sa2bmdl");
			mesh1 = ObjectHelper.GetMeshes(model1);
			skyboxtexlist = NJS_TEXLIST.Load("stg13_cityescape/tls/Skybox.satex");
			treemodel1 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree1.sa2bmdl");
			treemesh1 = ObjectHelper.GetMeshes(treemodel1);
			treemodel2 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree2.sa2bmdl");
			treemesh2 = ObjectHelper.GetMeshes(treemodel2);
			treemodel3 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree3.sa2bmdl");
			treemesh3 = ObjectHelper.GetMeshes(treemodel3);
			treemodel4 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree4.sa2bmdl");
			treemesh4 = ObjectHelper.GetMeshes(treemodel4);
			treemodel5 = ObjectHelper.LoadModel("stg13_cityescape/models/GC/DownhillTree5.sa2bmdl");
			treemesh5 = ObjectHelper.GetMeshes(treemodel5);
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
			if (treetexs == null)
				treetexs = ObjectHelper.GetTextures("landtx13", treetexlist, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(treepos1.X, treepos1.Y + 1.0f, treepos1.Z);
			result1.AddRange(treemodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(treepos2.X, treepos2.Y + 1.0f, treepos2.Z);
			result1.AddRange(treemodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(treepos3.X, treepos3.Y + 1.0f, treepos3.Z);
			result1.AddRange(treemodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(treepos4.X, treepos4.Y + 1.0f, treepos4.Z);
			result1.AddRange(treemodel4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(treepos5.X, treepos5.Y + 1.0f, treepos5.Z);
			result1.AddRange(treemodel5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, treetexs, treemesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
	}
}