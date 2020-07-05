using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using System;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;

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
		private static float renderDrawDistance = 6000f;
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

		public static void Initialize(Device d3dDevice)
		{
			direct3DDevice = d3dDevice;

			SetDefaultLights(d3dDevice, false);
		}

		public static void SetDefaultLights(Device d3dDevice, bool reset)
		{
			for (int i = 0; i < 4; i++)
			{
				d3dDevice.EnableLight(i, false);
			}

			#region Key Light
			Light l0 = new Light()
			{
				Type = LightType.Directional,
				Diffuse = Color.FromArgb(255, 180, 172, 172).ToRawColor4(),
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(-0.245f, -1, 0.125f))
			};
			d3dDevice.SetLight(0, ref l0);
			d3dDevice.EnableLight(0, true);
			#endregion

			#region Fill Light
			Light l1 = new Light()
			{
				Type = LightType.Directional,
				Diffuse = Color.FromArgb(255, 132, 132, 132).ToRawColor4(),
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(0.245f, -0.4f, -0.125f))
			};
			d3dDevice.SetLight(1, ref l1);
			d3dDevice.EnableLight(1, true);
			#endregion

			#region Back Light
			Light l2 = new Light()
			{
				Type = LightType.Directional,
				Diffuse = Color.FromArgb(255, 130, 142, 130).ToRawColor4(),
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(-0.45f, 1f, 0.25f))
			};
			d3dDevice.SetLight(2, ref l2);
			d3dDevice.EnableLight(2, true);
			#endregion

			#region Font Setup
			onscreenFont = new Font(d3dDevice, new FontDescription
			{
				Height = 24,
				FaceName = "Verdana",
				Weight = FontWeight.Black,
				CharacterSet = FontCharacterSet.Oem,
				PitchAndFamily = FontPitchAndFamily.Default,
				OutputPrecision = FontPrecision.TrueType,
				Quality = FontQuality.ClearType
			});
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
			d3ddevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
			d3ddevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
			d3ddevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Anisotropic);
			d3ddevice.SetRenderState(RenderState.Lighting, !overrideLighting);
			d3ddevice.SetRenderState(RenderState.SpecularEnable, true);
			if (!OverrideLighting) d3ddevice.SetRenderState(RenderState.Ambient, Color.Black.ToArgb());
			else d3ddevice.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
			d3ddevice.SetRenderState(RenderState.AlphaBlendEnable, false);
			d3ddevice.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
			d3ddevice.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			d3ddevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			d3ddevice.SetRenderState(RenderState.AlphaTestEnable, true);
			d3ddevice.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
			d3ddevice.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Material);
			d3ddevice.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Material);
			d3ddevice.SetRenderState(RenderState.SpecularMaterialSource, ColorSource.Material);
			d3ddevice.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.BlendDiffuseAlpha);
			d3ddevice.SetRenderState(RenderState.ColorVertex, true);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
		}
	}
}