using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Level_Effects
{
    class EmeraldCoast : LevelDefinition
    {
        SonicRetro.SAModel.Object model1, model2;
        Mesh[] mesh1, mesh2;
        Vector3 Skybox_Scale;

        public override void Init(Dictionary<string, string> data, byte act, Device dev)
        {
            Dictionary<string, Dictionary<string, string>> skyboxdata = IniFile.Load("Levels/Emerald Coast/Skybox Data.ini");
            if (skyboxdata.ContainsKey(act.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                Skybox_Scale = new SonicRetro.SAModel.Vertex(skyboxdata[act.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)]["Far"]).ToVector3();
            model1 = SonicRetro.SAModel.Object.LoadFromFile("Levels/Emerald Coast/Skybox model.sa1mdl");
            mesh1 = ObjectHelper.GetMeshes(model1, dev);
            model2 = SonicRetro.SAModel.Object.LoadFromFile("Levels/Emerald Coast/Skybox bottom model.sa1mdl");
            mesh2 = ObjectHelper.GetMeshes(model2, dev);
        }

        public override void Render(Device dev, Camera cam)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            MatrixStack transform = new MatrixStack();
            transform.Push();
            transform.TranslateLocal(cam.Position.X, 0, cam.Position.Z);
            transform.ScaleLocal(Skybox_Scale);
            Texture[] texs = ObjectHelper.GetTextures("BG_BEACH");
            result.AddRange(model1.DrawModelTree(dev, transform, texs, mesh1));
            result.AddRange(model2.DrawModelTree(dev, transform, texs, mesh2));
            transform.Pop();
            RenderInfo.Draw(result, dev, cam);
        }
    }
}
