using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Font = Microsoft.DirectX.Direct3D.Font;

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
		private static float renderDrawDistance = 3500f;
		private static bool overrideLighting = false;
		private static Device direct3DDevice;
		private static Font onscreenFont;		

		public static FillMode RenderFillMode { get { return renderFillMode; } set { renderFillMode = value; } }
		public static Cull RenderCullMode { get { return renderCullMode; } set { renderCullMode = value; } }
		public static float RenderDrawDistance { get { return renderDrawDistance; } set { renderDrawDistance = value; } }
		public static bool OverrideLighting { get { return overrideLighting; } set { overrideLighting = value; } }
		public static Device Direct3DDevice { get { return direct3DDevice; } set { direct3DDevice = value; } }
		public static Font OnscreenFont { get { return onscreenFont; } set { onscreenFont = value; } }
		#endregion

        #region Paths
        private static string gamePath;
        private static string projectName;
        private static string projectPath;

        public static string GamePath { get { return EditorOptions.gamePath; } set { EditorOptions.gamePath = value; } }
        public static string ProjectName { get { return EditorOptions.projectName; } set { EditorOptions.projectName = value; } }
        public static string ProjectPath { get { return EditorOptions.projectPath; } set { EditorOptions.projectPath = value; } }
        #endregion

		public static void Initialize(Device d3dDevice)
		{
			direct3DDevice = d3dDevice;

			#region Key Light
			d3dDevice.Lights[0].Type = LightType.Directional;
			d3dDevice.Lights[0].DiffuseColor = new ColorValue(180, 172, 172, 255);
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

			#region Font Setup
			onscreenFont = new Font(d3dDevice, 14, 14, FontWeight.DoNotCare, 0, false, CharacterSet.Oem, Precision.Default, FontQuality.Default, PitchAndFamily.FamilyDoNotCare, "Verdana");
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
            d3ddevice.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
			d3ddevice.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
			d3ddevice.SamplerState[0].MipFilter = TextureFilter.Anisotropic;
			d3ddevice.RenderState.Lighting = !overrideLighting;
			d3ddevice.RenderState.SpecularEnable = true;
            if (!OverrideLighting) d3ddevice.RenderState.Ambient = Color.Black;
            else d3ddevice.RenderState.Ambient = Color.White;
			d3ddevice.RenderState.AlphaBlendEnable = false;
			d3ddevice.RenderState.BlendOperation = BlendOperation.Add;
			d3ddevice.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			d3ddevice.RenderState.SourceBlend = Blend.SourceAlpha;
			d3ddevice.RenderState.AlphaTestEnable = true;
			d3ddevice.RenderState.AlphaFunction = Compare.Greater;
			d3ddevice.RenderState.AmbientMaterialSource = ColorSource.Material;
			d3ddevice.RenderState.DiffuseMaterialSource = ColorSource.Material;
			d3ddevice.RenderState.SpecularMaterialSource = ColorSource.Material;
			d3ddevice.TextureState[0].AlphaOperation = TextureOperation.BlendDiffuseAlpha;
			d3ddevice.RenderState.ColorVertex = true;
			d3ddevice.RenderState.ZBufferEnable = true;
		}
	}
}