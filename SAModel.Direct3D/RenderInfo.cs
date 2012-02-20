using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SonicRetro.SAModel.Direct3D
{
    public class RenderInfo
    {
        public Microsoft.DirectX.Direct3D.Mesh Mesh { get; private set; }
        public int Subset { get; private set; }
        public Matrix Transform { get; private set; }
        public Microsoft.DirectX.Direct3D.Material Material { get; private set; }
        public Texture Texture { get; private set; }
        public bool AlphaBlend { get; private set; }
        public bool SphereMap { get; private set; }
        public bool UseFilter { get; private set; }
        public FillMode FillMode { get; private set; }
        public BoundingSphere Bounds { get; private set; }

        public RenderInfo(Microsoft.DirectX.Direct3D.Mesh mesh, int subset, Matrix transform, Microsoft.DirectX.Direct3D.Material material, Texture texture, bool alphaBlend, bool sphereMap, bool useFilter, FillMode fillMode, BoundingSphere bounds)
        {
            Mesh = mesh;
            Subset = subset;
            Transform = transform;
            Material = material;
            Texture = texture;
            AlphaBlend = alphaBlend;
            SphereMap = sphereMap;
            UseFilter = useFilter;
            FillMode = fillMode;
            Bounds = bounds;
        }

        public void Draw(Device device)
        {
            FillMode mode = device.RenderState.FillMode;
            TextureFilter magfilter = device.SamplerState[0].MagFilter;
            TextureFilter minfilter = device.SamplerState[0].MinFilter;
            TextureFilter mipfilter = device.SamplerState[0].MipFilter;
            if (!UseFilter)
            {
                device.SamplerState[0].MagFilter = TextureFilter.None;
                device.SamplerState[0].MinFilter = TextureFilter.None;
                device.SamplerState[0].MipFilter = TextureFilter.None;
            }
            device.RenderState.FillMode = FillMode;
            device.SetTransform(TransformType.World, Transform);
            device.Material = Material;
            device.SetTexture(0, Texture);
            device.SetRenderState(RenderStates.AlphaBlendEnable, AlphaBlend);
            if (SphereMap)
                device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0x40000);
            else
                device.SetTextureStageState(0, TextureStageStates.TextureCoordinateIndex, 0);
            if (Mesh != null)
                Mesh.DrawSubset(Subset);
            device.RenderState.FillMode = mode;
            device.SamplerState[0].MagFilter = magfilter;
            device.SamplerState[0].MinFilter = minfilter;
            device.SamplerState[0].MipFilter = mipfilter;
        }

        public static void Draw(IEnumerable<RenderInfo> items, Device device, Camera camera)
        {
            List<KeyValuePair<float, RenderInfo>> drawList = new List<KeyValuePair<float, RenderInfo>>();
            foreach (RenderInfo item in items)
            {
                float dist = Vector3.Dot(camera.Position - item.Bounds.Center.ToVector3(), camera.Look);
                if (dist + item.Bounds.Radius < 0 | dist - item.Bounds.Radius > 5000)
                    continue;
                if (item.AlphaBlend)
                {
                    bool ins = false;
                    for (int i = 0; i < drawList.Count; i++)
                    {
                        if (drawList[i].Key < dist)
                        {
                            drawList.Insert(i, new KeyValuePair<float, RenderInfo>(dist, item));
                            ins = true;
                            break;
                        }
                    }
                    if (!ins)
                        drawList.Add(new KeyValuePair<float, RenderInfo>(dist, item));
                }
                else
                    drawList.Insert(0, new KeyValuePair<float, RenderInfo>(float.MaxValue, item));
            }
            foreach (KeyValuePair<float, RenderInfo> item in drawList)
                item.Value.Draw(device);
        }
    }
}