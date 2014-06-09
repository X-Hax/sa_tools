using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon
{
    /// <summary>
    /// This class will store all of the common editor options, from rendering to interface configuration.
    /// </summary>
    public static class EditorOptions
    {
        #region Render Options
        private static FillMode renderFillMode = FillMode.Solid;
        private static Cull renderCullMode = Cull.None;
        private static float renderDrawDistance = 5000f;
        private static bool overrideLighting = false;
        private static Device direct3DDevice;

        public static FillMode RenderFillMode { get { return renderFillMode; } set { renderFillMode = value; } }
        public static Cull RenderCullMode { get { return renderCullMode; } set { renderCullMode = value; } }
        public static float RenderDrawDistance { get { return renderDrawDistance; } set { renderDrawDistance = value; } }
        public static bool OverrideLighting { get { return overrideLighting; } set { overrideLighting = value; } }
        public static Device Direct3DDevice { get { return direct3DDevice; } set { direct3DDevice = value; } }
        #endregion

        public static void InitializeDefaultLights(Device d3dDevice)
        {
            direct3DDevice = d3dDevice;
            #region Key Light
            d3dDevice.Lights[0].Type = LightType.Directional;
            d3dDevice.Lights[0].DiffuseColor = new ColorValue(180,172,172,255);
            d3dDevice.Lights[0].Diffuse = Color.FromArgb(255, 180, 172, 172);
            d3dDevice.Lights[0].Ambient = Color.Black;
            d3dDevice.Lights[0].Specular = Color.White;            
            d3dDevice.Lights[0].Range = 0;
            d3dDevice.Lights[0].Direction = Vector3.Normalize(new Vector3(-0.245f, -1, 0.125f));
            d3dDevice.Lights[0].Enabled = true;
            #endregion

            #region Fill Light
            d3dDevice.Lights[1].Type = LightType.Directional;
            d3dDevice.Lights[1].DiffuseColor = new ColorValue(132, 132, 132, 255);
            d3dDevice.Lights[1].Diffuse = Color.FromArgb(255, 132, 132, 132);
            d3dDevice.Lights[1].Ambient = Color.Black;
            d3dDevice.Lights[1].Specular = Color.Gray;
            d3dDevice.Lights[1].Range = 0;
            d3dDevice.Lights[1].Direction = Vector3.Normalize(new Vector3(0.245f, -0.4f, -0.125f));
            d3dDevice.Lights[1].Enabled = true;
            #endregion

            #region Back Light
            d3dDevice.Lights[2].Type = LightType.Directional;
            d3dDevice.Lights[2].DiffuseColor = new ColorValue(100, 100, 100, 255);
            d3dDevice.Lights[2].Diffuse = Color.FromArgb(255, 130, 142, 130);
            d3dDevice.Lights[2].Ambient = Color.Black;
            d3dDevice.Lights[2].Specular = Color.Gray;
            d3dDevice.Lights[2].Range = 0;
            d3dDevice.Lights[2].Direction = Vector3.Normalize(new Vector3(-0.45f, 1f, 0.25f));
            d3dDevice.Lights[2].Enabled = true;
            #endregion
        }

        public static void Load()
        {
            throw new NotImplementedException();
        }

        public static void Save()
        {
            throw new NotImplementedException();
        }

        public static void RenderStateCommonSetup(Device d3ddevice)
        {
            d3ddevice.SetSamplerState(0, SamplerStageStates.MinFilter, (int)TextureFilter.Anisotropic);
            d3ddevice.SetSamplerState(0, SamplerStageStates.MagFilter, (int)TextureFilter.Anisotropic);
            d3ddevice.SetSamplerState(0, SamplerStageStates.MipFilter, (int)TextureFilter.Anisotropic);
            d3ddevice.SetRenderState(RenderStates.Lighting, true);
            d3ddevice.SetRenderState(RenderStates.SpecularEnable, false);
            if(!OverrideLighting) d3ddevice.SetRenderState(RenderStates.Ambient, Color.Black.ToArgb());
            else d3ddevice.SetRenderState(RenderStates.Ambient, Color.White.ToArgb());
            d3ddevice.SetRenderState(RenderStates.AlphaBlendEnable, false);
            d3ddevice.SetRenderState(RenderStates.BlendOperation, (int)BlendOperation.Add);
            d3ddevice.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            d3ddevice.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            d3ddevice.SetRenderState(RenderStates.AlphaTestEnable, true);
            d3ddevice.SetRenderState(RenderStates.AlphaFunction, (int)Compare.Greater);
            d3ddevice.SetRenderState(RenderStates.AmbientMaterialSource, (int)ColorSource.Material);
            d3ddevice.SetRenderState(RenderStates.DiffuseMaterialSource, (int)ColorSource.Material);
            d3ddevice.SetRenderState(RenderStates.SpecularMaterialSource, (int)ColorSource.Material);
            d3ddevice.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.BlendDiffuseAlpha);
            d3ddevice.SetRenderState(RenderStates.ColorVertex, true);
            d3ddevice.SetRenderState(RenderStates.ZEnable, true);
        }
    }
}
