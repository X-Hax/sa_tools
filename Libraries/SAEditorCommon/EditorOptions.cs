using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using SplitTools;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;

namespace SAModel.SAEditorCommon
{
	/// <summary>
	/// This class will store all of the common editor options, from rendering to interface configuration.
	/// </summary>
	public static class EditorOptions
	{
		#region Render Options
		private static FillMode renderFillMode = FillMode.Solid;
		private static Cull renderCullMode = Cull.None;
		private static Color fillColor = Color.Black;
		private static float renderDrawDistance = 10000f;
		private static float levelDrawDistance = 6000f;
		private static float setDrawDistance = 6000f;
		private static bool overrideLighting = false;
		private static bool ignoreMaterialColors = false;
		private static Device direct3DDevice;
		private static Font onscreenFont;
		private static Light keyLight;
		private static Light backLight;
		private static Light fillLight;
		private static Light currentLight;
		private static List<SADXStageLightData> stageLights;
		private static List<LSPaletteData> characterLights;

		public static FillMode RenderFillMode { get { return renderFillMode; } set { renderFillMode = value; } }
		public static Cull RenderCullMode { get { return renderCullMode; } set { renderCullMode = value; } }
		public static float RenderDrawDistance { get { return renderDrawDistance; } set { renderDrawDistance = value; } }
		public static float LevelDrawDistance { get { return levelDrawDistance; } set { levelDrawDistance = value; } }
		public static float SetItemDrawDistance { get { return setDrawDistance; } set { setDrawDistance = value; } }
		public static bool OverrideLighting { get { return overrideLighting; } set { overrideLighting = value; } }
		public static bool IgnoreMaterialColors { get { return ignoreMaterialColors; } set { ignoreMaterialColors = value; } }
		public static Device Direct3DDevice { get { return direct3DDevice; } set { direct3DDevice = value; } }
		public static Font OnscreenFont { get { return onscreenFont; } set { onscreenFont = value; } }
		public static Color FillColor { get { return fillColor; } set { fillColor = value; } }
		public static List<SADXStageLightData> StageLights { get { return stageLights; } set { stageLights = value; } }
		public static List<LSPaletteData> CharacterLights { get { return characterLights; } set { characterLights = value; } }
		public static float BackLightR { get { return backLight.Ambient.R; } set { backLight.Ambient.R = value; } }
		public static float BackLightG { get { return backLight.Ambient.G; } set { backLight.Ambient.G = value; } }
		public static float BackLightB { get { return backLight.Ambient.B; } set { backLight.Ambient.B = value; } }
		#endregion

		public static void Initialize(Device d3dDevice)
		{
			direct3DDevice = d3dDevice;

			ResetDefaultLights();
			SetDefaultLights(d3dDevice);

			#region Font Setup
			onscreenFont = new Font(d3dDevice, new FontDescription
			{
				Height = 24,
				FaceName = "Verdana",
				Weight = FontWeight.Black,
				CharacterSet = FontCharacterSet.Default,
				PitchAndFamily = FontPitchAndFamily.Default,
				OutputPrecision = FontPrecision.Default,
				Quality = FontQuality.Default
			});
			#endregion
		}

		public static void RenderStateCommonSetup(Device d3ddevice)
		{
			d3ddevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
			d3ddevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
			d3ddevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Anisotropic);
			d3ddevice.SetRenderState(RenderState.Lighting, !overrideLighting);
			d3ddevice.SetRenderState(RenderState.SpecularEnable, false);
			if (!overrideLighting) d3ddevice.SetRenderState(RenderState.Ambient, Color.Black.ToArgb());
			else d3ddevice.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
			d3ddevice.SetRenderState(RenderState.AlphaBlendEnable, false);
			d3ddevice.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
			d3ddevice.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			d3ddevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			d3ddevice.SetRenderState(RenderState.AlphaTestEnable, true);
			d3ddevice.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
			d3ddevice.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Color1);
			d3ddevice.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Color1);
			d3ddevice.SetRenderState(RenderState.SpecularMaterialSource, ColorSource.Color2);
			d3ddevice.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.BlendDiffuseAlpha);
			d3ddevice.SetRenderState(RenderState.ColorVertex, true);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
		}

		#region Lighting
		public static void SetDefaultLights(Device d3dDevice)
		{
			for (int i = 0; i < 4; i++)
			{
				d3dDevice.EnableLight(i, false);
			}
			d3dDevice.SetLight(0, ref keyLight);
			d3dDevice.EnableLight(0, true);

			d3dDevice.SetLight(1, ref fillLight);
			d3dDevice.EnableLight(1, true);

			d3dDevice.SetLight(2, ref backLight);
			d3dDevice.EnableLight(2, true);
		}

		public static void ResetDefaultLights()
		{
			#region Key Light
			keyLight = new Light()
			{
				Type = LightType.Directional,
				Diffuse = Color.FromArgb(255, 180, 172, 172).ToRawColor4(),
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(-0.245f, -1, 0.125f))
			};
			#endregion

			#region Fill Light
			fillLight = new Light()
			{
				Type = LightType.Directional,
				Diffuse = Color.FromArgb(255, 132, 132, 132).ToRawColor4(),
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(0.245f, -0.4f, -0.125f))
			};
			#endregion

			#region Back Light
			backLight = new Light()
			{
				Type = LightType.Directional,
				Diffuse = Color.FromArgb(255, 130, 142, 130).ToRawColor4(),
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(-0.45f, 1f, 0.25f))
			};
			#endregion
		}

		public static void SetStageLights(Device d3ddevice)
		{
			for (int i = 0; i < 4; i++)
			{
				d3ddevice.EnableLight(i, false);
			}
			if (stageLights == null || stageLights.Count == 0)
				SetDefaultLights(d3ddevice);
			for (int i = 0; i < stageLights.Count; i++)
			{
				SADXStageLightData lightData = stageLights[i];
				currentLight.Type = LightType.Directional;
				currentLight.Direction = lightData.Direction.ToVector3();
				currentLight.Specular = new RawColor4(lightData.Specular, lightData.Specular, lightData.Specular, 1.0f);
				// SADXPC reuses the first light's ambient color for other lights
				currentLight.Ambient = new RawColor4(
					stageLights[0].AmbientRGB.X,
					stageLights[0].AmbientRGB.Y,
					stageLights[0].AmbientRGB.Z,
					1.0f);
				currentLight.Diffuse = new RawColor4(
					lightData.RGB.X * lightData.Diffuse,
					lightData.RGB.Y * lightData.Diffuse,
					lightData.RGB.Z * lightData.Diffuse,
					1.0f);
				d3ddevice.SetRenderState(RenderState.SpecularEnable, false);
				d3ddevice.SetLight(i, ref currentLight);
				d3ddevice.EnableLight(i, lightData.UseDirection);
			}
		}

		public static void SetCharacterLight(Device d3ddevice, Vertex stageLightDirection, LSPaletteData lspalette, bool casino = false, bool icecap = false)
		{
			float dir_x;
			float dir_y;
			float dir_z;
			RawVector3 lightDirection;
			currentLight.Ambient.R = lspalette.Ambient.X;
			currentLight.Ambient.G = lspalette.Ambient.Y;
			currentLight.Ambient.B = lspalette.Ambient.Z;
			currentLight.Diffuse.A = 1.0f;
			currentLight.Diffuse.R = lspalette.Diffuse;
			currentLight.Diffuse.G = lspalette.Diffuse;
			currentLight.Diffuse.B = lspalette.Diffuse;
			currentLight.Specular.R = lspalette.Specular1.X;
			currentLight.Specular.G = lspalette.Specular1.Y;
			currentLight.Specular.B = lspalette.Specular1.Z;
			currentLight.Type = LightType.Directional;
			if (lspalette.Flags.HasFlag(LSPaletteData.LSPaletteFlags.UseLSLightDirection))
				lightDirection = lspalette.Direction.ToVector3();
			else
				lightDirection = stageLightDirection.ToVector3();
			if (casino)
			{
				dir_x = stageLightDirection.X;
				dir_y = stageLightDirection.Y;
				dir_z = stageLightDirection.Z;
				currentLight.Direction.X = dir_x;
				currentLight.Direction.Y = dir_y;
				currentLight.Direction.Z = dir_z;
				goto skipdir;
			}
			if (!lspalette.Flags.HasFlag(LSPaletteData.LSPaletteFlags.IgnoreDirection))
			{
				currentLight.Direction.X = lightDirection.X;
				currentLight.Direction.Y = lightDirection.Y;
				currentLight.Direction.Z = lightDirection.Z;
				goto skipdir;
			}
			currentLight.Direction.X = -lightDirection.X;
			currentLight.Direction.Y = -lightDirection.Y;
			currentLight.Direction.Z = -lightDirection.Z;
		skipdir:
			// Set light 0
			d3ddevice.SetLight(0, ref currentLight);
			d3ddevice.EnableLight(0, true);
			// Set render states
			d3ddevice.SetRenderState(RenderState.Lighting, true);
			d3ddevice.SetRenderState(RenderState.SpecularEnable, true);
			d3ddevice.SetRenderState(RenderState.AlphaBlendEnable, true);
			d3ddevice.SetRenderState(RenderState.ShadeMode, 2);
			// Disable lights 1-2 and enable light 3
			for (int i = 1; i < 4; i++)
				if ((icecap || lspalette.Flags.HasFlag(LSPaletteData.LSPaletteFlags.OverrideLastLight) && i == 3))
				{
					if (icecap)
					{
						currentLight.Specular.R = 0.2f;
						currentLight.Specular.G = 0.2f;
						currentLight.Specular.B = 0.2f;
					}
					currentLight.Direction.X = -lightDirection.X;
					currentLight.Direction.Y = -lightDirection.Y;
					currentLight.Direction.Z = -lightDirection.Z;
					d3ddevice.SetLight(3, ref currentLight);
					d3ddevice.EnableLight(3, true);
				}
				else
					d3ddevice.EnableLight(i, false);
		}

		public static void SetLightType(Device d3ddevice, SADXLightTypes lightType)
		{
			switch (lightType)
			{
				case SADXLightTypes.Default:
				default:
					SetDefaultLights(d3ddevice);
					return;
				case SADXLightTypes.Level:
					SetStageLights(d3ddevice);
					return;
				case SADXLightTypes.Character:
					if (characterLights == null || characterLights.Count == 0)
						SetDefaultLights(d3ddevice);
					else
						SetCharacterLight(d3ddevice, stageLights[0].Direction, GetCharacterLight(LSPaletteData.LSPaletteTypes.Character), (characterLights[0].Level == SA1LevelIDs.Casinopolis && characterLights[0].Act == 0), characterLights[0].Level == SA1LevelIDs.IceCap);
					return;
				case SADXLightTypes.CharacterAlt:
					if (characterLights == null || characterLights.Count == 0)
						SetDefaultLights(d3ddevice);
					else
						SetCharacterLight(d3ddevice, stageLights[0].Direction, GetCharacterLight(LSPaletteData.LSPaletteTypes.CharacterAlt), (characterLights[0].Level == SA1LevelIDs.Casinopolis && characterLights[0].Act == 0), characterLights[0].Level == SA1LevelIDs.IceCap);
					return;
				case SADXLightTypes.Boss:
					if (characterLights == null || characterLights.Count == 0)
						SetDefaultLights(d3ddevice);
					else
						SetCharacterLight(d3ddevice, stageLights[0].Direction, GetCharacterLight(LSPaletteData.LSPaletteTypes.Boss), (characterLights[0].Level == SA1LevelIDs.Casinopolis && characterLights[0].Act == 0), characterLights[0].Level == SA1LevelIDs.IceCap);
					return;
			}
		}

		private static LSPaletteData GetCharacterLight(LSPaletteData.LSPaletteTypes type)
		{
			foreach (LSPaletteData charlight in characterLights)
			{
				if ((LSPaletteData.LSPaletteTypes)charlight.Type == type)
					return charlight;
			}
			return characterLights[0];
		}

		public enum SADXLightTypes
		{
			Level = 0, // Use Stage Lights
			Character = 2, // Use LS Palette Type 0
			CharacterAlt = 4, // Use LS Palette Type 6
			Boss = 6, // Use LS Palette Type 8
			Default = -1 // Use default editor lights
		}
		#endregion
	}
}