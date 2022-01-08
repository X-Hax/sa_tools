using SplitTools;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.Level_Effects
{
	class IceCap : LevelDefinition
	{
		byte Act;
		readonly NJS_OBJECT[] models = new NJS_OBJECT[3];
		readonly Mesh[][] meshes = new Mesh[3][];
		Texture[] texs;
		Vector3 Skybox_Scale;
		
		public override void Init(IniLevelData data, byte act, byte timeofday)
		{
			Act = act;
			SkyboxScale[] skyboxdata = SkyboxScaleList.Load("stg08_icecap/bg/bgScale.ini");
			if (skyboxdata.Length > act)
				Skybox_Scale = skyboxdata[act].Far.ToVector3();
			models[0] = ObjectHelper.LoadModel("stg08_icecap/bg/models/sora98_oya.nja.sa1mdl");
			models[1] = ObjectHelper.LoadModel("stg08_icecap/bg/models/sora55_aida01a.nja.sa1mdl");
			models[2] = ObjectHelper.LoadModel("stg08_icecap/bg/models/sora98_yuki_03kumo.nja.sa1mdl");
			for (int i = 0; i < 3; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override void Render(Device dev, EditorCamera cam)
		{
			if (Act == 1 || Act == 3) 
				return;
			texs = ObjectHelper.GetTextures("BG_ICECAP");
			List<RenderInfo> result = new List<RenderInfo>();
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(cam.Position.X, cam.Position.Y, cam.Position.Z);
			switch (Act)
			{
				case 0:
				default:
					for (int i = 0; i < 3; i++)
						result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i]));
					break;
				case 2:
					transform.NJScale(Skybox_Scale);
					if (cam.Position.Y > -4000.0f || cam.Position.Y < -18500.0f)
					{
						for (int i = 0; i < 2; i++)
							result.AddRange(models[i].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[i]));
						if (cam.Position.Y > -4000.0f)
						{
							transform.NJRotateY(0xC000);
							result.AddRange(models[2].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes[2]));
						}
					}
					break;
			}
			transform.Pop();
			RenderInfo.Draw(result, dev, cam);
		}
	}
}