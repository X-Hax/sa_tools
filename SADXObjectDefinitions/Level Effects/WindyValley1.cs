using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Level_Effects
{
    class WindyValley1 : LevelDefinition
    {
        SonicRetro.SAModel.Object[] models = new SonicRetro.SAModel.Object[5];
        Mesh[][] meshes = new Mesh[5][];
        Vector3 Skybox_Scale;

        public override void Init(Dictionary<string, string> data, byte act, Device dev)
        {
            Dictionary<string, Dictionary<string, string>> skyboxdata = IniFile.Load("Levels/Windy Valley/Skybox Data.ini");
            if (skyboxdata.ContainsKey(act.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                Skybox_Scale = new SonicRetro.SAModel.Vertex(skyboxdata[act.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Far"]).ToVector3();
            for (int i = 0; i < 5; i++)
            {
                models[i] = SonicRetro.SAModel.Object.LoadFromFile("Levels/Windy Valley/Act 1/Skybox model " + (i + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl");
                meshes[i] = ObjectHelper.GetMeshes(models[i], dev);
            }
        }

        public override void Render(Device dev, Camera cam)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            MatrixStack transform = new MatrixStack();
            transform.Push();
            transform.TranslateLocal(cam.Position.X, 0, cam.Position.Z);
            transform.ScaleLocal(Skybox_Scale);
            Texture[] texs = ObjectHelper.GetTextures("WINDY_BACK");
            for (int i = 0; i < 5; i++)
                result.AddRange(models[i].DrawModelTree(dev, transform, texs, meshes[i]));
            transform.Pop();
            RenderInfo.Draw(result, dev, cam);
        }
    }
}
