using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.Direct3D
{
    public class RenderInfo
    {
        public Microsoft.DirectX.Direct3D.Mesh Mesh { get; private set; }
        public int Subset { get; private set; }
        public Matrix Transform { get; private set; }
        public Material Material { get; private set; }
        public Texture Texture { get; private set; }
        public FillMode FillMode { get; private set; }
        public BoundingSphere Bounds { get; private set; }

        public RenderInfo(Microsoft.DirectX.Direct3D.Mesh mesh, int subset, Matrix transform, Material material, Texture texture, FillMode fillMode, BoundingSphere bounds)
        {
            Mesh = mesh;
            Subset = subset;
            Transform = transform;
            Material = material;
            Texture = texture;
            FillMode = fillMode;
            Bounds = bounds;
        }

        public void Draw(Device device)
        {
            FillMode mode = device.RenderState.FillMode;
            TextureFilter magfilter = device.SamplerState[0].MagFilter;
            TextureFilter minfilter = device.SamplerState[0].MinFilter;
            TextureFilter mipfilter = device.SamplerState[0].MipFilter;

            /*if (!Material.SuperSample)
            {
                device.SamplerState[0].MagFilter = TextureFilter.None;
                device.SamplerState[0].MinFilter = TextureFilter.None;
                device.SamplerState[0].MipFilter = TextureFilter.None;
            }
            device.RenderState.FillMode = FillMode;
            device.SetTransform(TransformType.World, Transform);
            device.Material = new Microsoft.DirectX.Direct3D.Material
            {
                Diffuse = Material.DiffuseColor,
                Ambient = Material.DiffuseColor,
                Specular = Material.IgnoreSpecular ? System.Drawing.Color.Transparent : Material.SpecularColor,
                SpecularSharpness = Material.Exponent * Material.Exponent
            };
            device.SetTexture(0, Material.UseTexture ? Texture : null);
            device.RenderState.Ambient = (Material.IgnoreLighting) ? System.Drawing.Color.White : System.Drawing.Color.Black;
            device.RenderState.AlphaBlendEnable = Material.UseAlpha; if(Material.UseAlpha) device.RenderState.Ambient = Material.DiffuseColor;
            switch (Material.DestinationAlpha)
            {
                case AlphaInstruction.Zero:
                    device.RenderState.AlphaDestinationBlend = Blend.Zero;
                    break;
                case AlphaInstruction.One:
                    device.RenderState.AlphaDestinationBlend = Blend.One;
                    break;
                case AlphaInstruction.OtherColor:
                    break;
                case AlphaInstruction.InverseOtherColor:
                    break;
                case AlphaInstruction.SourceAlpha:
                    device.RenderState.AlphaDestinationBlend = Blend.SourceAlpha;
                    break;
                case AlphaInstruction.InverseSourceAlpha:
                    device.RenderState.AlphaDestinationBlend = Blend.InvSourceAlpha;
                    break;
                case AlphaInstruction.DestinationAlpha:
                    device.RenderState.AlphaDestinationBlend = Blend.DestinationAlpha;
                    break;
                case AlphaInstruction.InverseDestinationAlpha:
                    device.RenderState.AlphaDestinationBlend = Blend.InvDestinationAlpha;
                    break;
            }
            switch (Material.SourceAlpha)
            {
                case AlphaInstruction.Zero:
                    device.RenderState.AlphaSourceBlend = Blend.Zero;
                    break;
                case AlphaInstruction.One:
                    device.RenderState.AlphaSourceBlend = Blend.One;
                    break;
                case AlphaInstruction.OtherColor:
                    break;
                case AlphaInstruction.InverseOtherColor:
                    break;
                case AlphaInstruction.SourceAlpha:
                    device.RenderState.AlphaSourceBlend = Blend.SourceAlpha;
                    break;
                case AlphaInstruction.InverseSourceAlpha:
                    device.RenderState.AlphaSourceBlend = Blend.InvSourceAlpha;
                    break;
                case AlphaInstruction.DestinationAlpha:
                    device.RenderState.AlphaSourceBlend = Blend.DestinationAlpha;
                    break;
                case AlphaInstruction.InverseDestinationAlpha:
                    device.RenderState.AlphaSourceBlend = Blend.InvDestinationAlpha;
                    break;
            }
            device.TextureState[0].TextureCoordinateIndex = Material.EnvironmentMap ? (int)TextureCoordinateIndex.SphereMap : 0;*/

			Material.SetDeviceStates(device, Texture, Transform, FillMode);

            if (Mesh != null)
                Mesh.DrawSubset(Subset);
            device.RenderState.Ambient = System.Drawing.Color.Black;
            device.RenderState.FillMode = mode;
            device.SamplerState[0].MagFilter = magfilter;
            device.SamplerState[0].MinFilter = minfilter;
            device.SamplerState[0].MipFilter = mipfilter;
        }

        public static void Draw(IEnumerable<RenderInfo> items, Device device, EditorCamera camera)
        {
            List<KeyValuePair<float, RenderInfo>> drawList = new List<KeyValuePair<float, RenderInfo>>();
            foreach (RenderInfo item in items)
            {
                float dist = Extensions.Distance(camera.Position, item.Bounds.Center.ToVector3());

                if (dist > camera.DrawDistance) continue;

                if (item.Material != null && item.Material.UseAlpha)
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