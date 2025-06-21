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
	class HiddenBase : LevelDefinition
	{
		NJS_OBJECT pyramid;
		Mesh[] pyramidmesh;
		NJS_TEXLIST pyramidtexlist;
		Vector3 pyramidpos = new Vector3(0, 0, -4700.0f);
		Texture[] pyramidtexs;
		NJS_OBJECT skybox1;
		Mesh[] skymesh1;
		NJS_TEXLIST skytexlist1;
	//	Vector3 skypos1 = new Vector3(0, 0, -2250.0f);
		Texture[] skytexs1;
		NJS_OBJECT skybox2;
		Mesh[] skymesh2;
		NJS_TEXLIST skytexlist2;
		Vector3 skypos2 = new Vector3(-3000.0f, 0, -5000.0f);
		Texture[] skytexs2;

		Vector3 quicksandpos1 = new Vector3(-3830.0f, 900.0f, -5310.0f);
		Vector3 quicksandpos2 = new Vector3(-3580.0f, 900.0f, -5785.0f);
		Vector3 quicksandpos3 = new Vector3(-4095.0f, 220.0f, -3085.0f);

		// Rotate by 0xFFFFC000 Y
		Vector3 quicksandpos4 = new Vector3(-4085.0f, 580.0f, -4745.0f);
		Vector3 quicksandpos4alt = new Vector3(10.0f, 360.0f, -1660.0f);
		// Rotate by 0x8000 Y
		Vector3 quicksandpos5 = new Vector3(-3025.0f, 940.0f, -5785.0f);
		Vector3 quicksandpos5alt = new Vector3(1070.0f, 720.0f, -2700.0f);
		// Rotate by 0xFFFFC000 Y
		Vector3 quicksandpos6 = new Vector3(-3625.0f, 580.0f, -4640.0f);
		Vector3 quicksandpos6alt = new Vector3(470.0f, 360.0f, -1555.0f);
		// Rotate by 0xFFFFC000 Y
		Vector3 quicksandpos7 = new Vector3(-3625.0f, 580.0f, -4940.0f);
		Vector3 quicksandpos7alt = new Vector3(470.0f, 360.0f, -1855.0f);


		Vector3 quicksandpos8 = new Vector3(0.0f, 0.1f, -1000.0f);
		Vector3 quicksandpos9 = new Vector3(-2000.0f, 0.1f, -1500.0f);

		NJS_OBJECT qsandmodel1;
		Mesh[] qsandmesh1;
		NJS_OBJECT qsandgritmodel1;
		Mesh[] qsandgritmesh1;
		NJS_OBJECT qsandmodel2;
		Mesh[] qsandmesh2;
		NJS_OBJECT qsandgritmodel2;
		Mesh[] qsandgritmesh2;

		// Use with pos 3-7
		NJS_OBJECT qsandmodel3;
		Mesh[] qsandmesh3;
		NJS_OBJECT qsandgritmodel3;
		Mesh[] qsandgritmesh3;
		NJS_OBJECT qsandmodel3A;
		Mesh[] qsandmesh3A;
		NJS_OBJECT qsandgritmodel3A;
		Mesh[] qsandgritmesh3A;
		NJS_OBJECT qsandmodel3B;
		Mesh[] qsandmesh3B;
		NJS_OBJECT qsandgritmodel3B;
		Mesh[] qsandgritmesh3B;
		NJS_OBJECT qsandmodel3C;
		Mesh[] qsandmesh3C;
		NJS_OBJECT qsandgritmodel3C;
		Mesh[] qsandgritmesh3C;
		NJS_OBJECT qsandmodel3D;
		Mesh[] qsandmesh3D;
		NJS_OBJECT qsandgritmodel3D;
		Mesh[] qsandgritmesh3D;

		NJS_OBJECT qsandmodel4;
		Mesh[] qsandmesh4;
		NJS_OBJECT qsandgritmodel4;
		Mesh[] qsandgritmesh4;
		NJS_OBJECT qsandmodel5;
		Mesh[] qsandmesh5;
		NJS_OBJECT qsandgritmodel5;
		Mesh[] qsandgritmesh5;

		NJS_OBJECT qsandgritmodel6;
		Mesh[] qsandgritmesh6;
		NJS_OBJECT qsandgritmodel7;
		Mesh[] qsandgritmesh7;

		NJS_TEXLIST quicksandtexlist1;
		Texture[] sandtex1;
		NJS_TEXLIST quicksandtexlist2;
		Texture[] sandtex2;

		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			quicksandtexlist1 = NJS_TEXLIST.Load("stg21_SandOcean/tls/Quicksand.satex");
			quicksandtexlist2 = NJS_TEXLIST.Load("stg23_HiddenBase/tls/QuicksandGrit1.satex");

			pyramid = ObjectHelper.LoadModel("stg23_HiddenBase/models/Pyramid.sa2mdl");
			pyramidmesh = ObjectHelper.GetMeshes(pyramid);
			pyramidtexlist = NJS_TEXLIST.Load("stg23_HiddenBase/tls/Pyramid.satex");
			skybox1 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Skybox1.sa2mdl");
			skymesh1 = ObjectHelper.GetMeshes(skybox1);
			skytexlist1 = NJS_TEXLIST.Load("stg23_HiddenBase/tls/Skybox1.satex");
			skybox2 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Skybox2.sa2mdl");
			skymesh2 = ObjectHelper.GetMeshes(skybox2);
			skytexlist2 = NJS_TEXLIST.Load("stg23_HiddenBase/tls/Skybox2.satex");

			qsandmodel1 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Quicksand1.sa2mdl");
			qsandmesh1 = ObjectHelper.GetMeshes(qsandmodel1);
			qsandgritmodel1 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit1.sa2mdl");
			qsandgritmesh1 = ObjectHelper.GetMeshes(qsandgritmodel1);
			qsandmodel2 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Quicksand2.sa2mdl");
			qsandmesh2 = ObjectHelper.GetMeshes(qsandmodel2);
			qsandgritmodel2 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit2.sa2mdl");
			qsandgritmesh2 = ObjectHelper.GetMeshes(qsandgritmodel2);
			qsandmodel3 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Quicksand3.sa2mdl");
			//Necessary to apply transforms without further complications
			qsandmodel3.Position = new Vertex(0, 0, 0);
			qsandmesh3 = ObjectHelper.GetMeshes(qsandmodel3);
			qsandgritmodel3 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit3.sa2mdl");
			qsandgritmodel3.Position = new Vertex(0, 0, 0);
			qsandgritmesh3 = ObjectHelper.GetMeshes(qsandgritmodel3);

			qsandmodel4 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Quicksand4.sa2mdl");
			qsandmesh4 = ObjectHelper.GetMeshes(qsandmodel4);
			qsandgritmodel4 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit4.sa2mdl");
			qsandgritmesh4 = ObjectHelper.GetMeshes(qsandgritmodel4);
			qsandmodel5 = ObjectHelper.LoadModel("stg23_HiddenBase/models/Quicksand5.sa2mdl");
			qsandmesh5 = ObjectHelper.GetMeshes(qsandmodel5);
			qsandgritmodel5 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit5.sa2mdl");
			qsandgritmesh5 = ObjectHelper.GetMeshes(qsandgritmodel5);
			
			qsandgritmodel6 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit6.sa2mdl");
			qsandgritmesh6 = ObjectHelper.GetMeshes(qsandgritmodel6);
			qsandgritmodel7 = ObjectHelper.LoadModel("stg23_HiddenBase/models/QuicksandGrit7.sa2mdl");
			qsandgritmesh7 = ObjectHelper.GetMeshes(qsandgritmodel7);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (pyramidtexs == null)
				pyramidtexs = ObjectHelper.GetTextures("bgtex23", pyramidtexlist, dev);
			if (skytexs1 == null)
				skytexs1 = ObjectHelper.GetTextures("bgtex23", skytexlist1, dev);
			if (skytexs2 == null)
				skytexs2 = ObjectHelper.GetTextures("bgtex23", skytexlist2, dev);
			List<RenderInfo> result1 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Distant Pyramid
			transform.Push();
			transform.NJTranslate(pyramidpos);
			result1.AddRange(pyramid.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, pyramidtexs, pyramidmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Skybox 1
			transform.Push();
			//transform.NJTranslate(skypos1);
			result1.AddRange(skybox1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, skytexs1, skymesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Skybox 2
			transform.Push();
			transform.NJTranslate(skypos2);
			result1.AddRange(skybox2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, skytexs2, skymesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result1, dev, cam);
		}
		public override void RenderLate(Device dev, EditorCamera cam)
		{
			if (sandtex1 == null)
				sandtex1 = ObjectHelper.GetTextures("stg21_ryuusa", quicksandtexlist1, dev);
			if (sandtex2 == null)
				sandtex2 = ObjectHelper.GetTextures("objtex_stg23", quicksandtexlist2, dev);
			//For all of the quicksand meshses
			List<RenderInfo> result2 = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			// Quicksand Area 1
			transform.Push();
			//transform.NJTranslate(quicksandpos1);
			result2.AddRange(qsandmodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel1.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh1, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 2
			transform.Push();
			//transform.NJTranslate(quicksandpos2);
			result2.AddRange(qsandmodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel2.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh2, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 3
			transform.Push();
			transform.NJTranslate(quicksandpos3);
			result2.AddRange(qsandmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 4
			transform.Push();
			transform.NJTranslate(quicksandpos4);
			transform.NJRotateY(-0x4000);
			result2.AddRange(qsandmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 5
			transform.Push();
			transform.NJTranslate(quicksandpos5);
			transform.NJRotateY(0x8000);
			result2.AddRange(qsandmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 6
			transform.Push();
			transform.NJTranslate(quicksandpos6);
			transform.NJRotateY(-0x4000);
			result2.AddRange(qsandmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 7
			transform.Push();
			transform.NJTranslate(quicksandpos7);
			transform.NJRotateY(-0x4000);
			result2.AddRange(qsandmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel3.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh3, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 8
			transform.Push();
			//transform.NJTranslate(quicksandpos8);
			result2.AddRange(qsandmodel4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel4.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh4, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Area 9
			transform.Push();
			//transform.NJTranslate(quicksandpos9);
			result2.AddRange(qsandmodel5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex1, qsandmesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel5.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh5, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			// Quicksand Miscellaneous Grit
			transform.Push();
			result2.AddRange(qsandgritmodel6.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh6, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result2.AddRange(qsandgritmodel7.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, sandtex2, qsandgritmesh7, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			transform.Pop();
			RenderInfo.Draw(result2, dev, cam);
		}
	}
}