using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SADXLVL2;

namespace SADXObjectDefinitions.Level_Effects
{
    class WindyValley2 : LevelDefinition
    {
        SonicRetro.SAModel.Object[] models = new SonicRetro.SAModel.Object[3];
        Mesh[][] meshes = new Mesh[3][];

        public override void Init(Dictionary<string, string> data, byte act, Device dev)
        {
            for (int i = 0; i < 3; i++)
            {
                models[i] = SonicRetro.SAModel.Object.LoadFromFile("Levels/Windy Valley/Act 2/Skybox model " + (i + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl");
                meshes[i] = ObjectHelper.GetMeshes(models[i], dev);
            }
        }

        public override void Render(Device dev, Camera cam)
        {
            List<RenderInfo> result = new List<RenderInfo>();
            MatrixStack transform = new MatrixStack();
            transform.Push();
            Texture[] texs = ObjectHelper.GetTextures("WINDY_BACK2");
            for (int i = 0; i < 3; i++)
                result.AddRange(models[i].DrawModelTree(dev, transform, texs, meshes[i]));
            transform.Pop();
            RenderInfo.Draw(result, dev, cam);
        }
    }
}
