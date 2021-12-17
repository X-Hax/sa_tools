using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using System;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using SplitTools;
using SharpDX.Mathematics.Interop;

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
		private static SADXStageLightData[] stageLights;
		private static LSPaletteData[] characterLights;
		public static Light keyLight;
		public static Light backLight;
		public static Light fillLight;

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
		public static SADXStageLightData[] StageLights { get { return stageLights; } set { stageLights = value; } }
		public static LSPaletteData[] CharacterLights { get { return characterLights; } set { characterLights = value; } }
		#endregion

		public static void Initialize(Device d3dDevice)
		{
			direct3DDevice = d3dDevice;

			SetDefaultLights(d3dDevice, false);

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
		public static void SetStageLights(Device d3ddevice, SADXStageLightData[] lightList)
		{
			for (int i = 0; i < 4; i++)
			{
				d3ddevice.EnableLight(i, false);
			}
			if (lightList == null || lightList.Length == 0)
				SetDefaultLights(d3ddevice, false);
			for (int i = 0; i < lightList.Length; i++)
			{
				SADXStageLightData lightData = lightList[i];
				Light light = new Light
				{
					Type = LightType.Directional,
					Direction = lightData.Direction.ToVector3(),
				};
				light.Specular = new RawColor4(lightData.Dif, lightData.Dif, lightData.Dif, 1.0f);
				// SADXPC reuses the first light's ambient color for other lights
				light.Ambient = new RawColor4(
					lightList[0].AmbientRGB.X,
					lightList[0].AmbientRGB.Y,
					lightList[0].AmbientRGB.Z,
					1.0f);
				light.Diffuse = new RawColor4(
					lightData.RGB.X * lightData.Multiplier,
					lightData.RGB.Y * lightData.Multiplier,
					lightData.RGB.Z * lightData.Multiplier,
					1.0f);
				d3ddevice.SetRenderState(RenderState.SpecularEnable, false);
				d3ddevice.SetLight(i, ref light);
				d3ddevice.EnableLight(i, lightData.UseDirection);
			}
		}

		public static void SetCharacterLight(Device d3ddevice, Vertex stageLightDirection, LSPaletteData lspalette, bool casino = false, bool icecap = false)
		{
			float dir_x;
			float dir_y;
			float dir_z;
			Light light = new Light();
			RawVector3 lightDirection;
			light.Ambient.R = lspalette.Ambient.X;
			light.Ambient.G = lspalette.Ambient.Y;
			light.Ambient.B = lspalette.Ambient.Z;
			light.Diffuse.A = 1.0f;
			light.Diffuse.R = lspalette.Diffuse;
			light.Diffuse.G = lspalette.Diffuse;
			light.Diffuse.B = lspalette.Diffuse;
			light.Specular.R = lspalette.Specular1.X;
			light.Specular.G = lspalette.Specular1.Y;
			light.Specular.B = lspalette.Specular1.Z;
			light.Type = LightType.Directional;
			if (lspalette.Flags.HasFlag(LSPaletteData.LSPaletteFlags.UseLSLightDirection))
				lightDirection = lspalette.Direction.ToVector3();
			else
				lightDirection = stageLightDirection.ToVector3();
			if (casino)
			{
				dir_x = stageLightDirection.X;
				dir_y = stageLightDirection.Y;
				dir_z = stageLightDirection.Z;
				light.Direction.X = dir_x;
				light.Direction.Y = dir_y;
				light.Direction.Z = dir_z;
				goto skipdir;
			}
			if (!lspalette.Flags.HasFlag(LSPaletteData.LSPaletteFlags.IgnoreDirection))
			{
				light.Direction.X = lightDirection.X;
				light.Direction.Y = lightDirection.Y;
				light.Direction.Z = lightDirection.Z;
				goto skipdir;
			}
			light.Direction.X = -lightDirection.X;
			light.Direction.Y = -lightDirection.Y;
			light.Direction.Z = -lightDirection.Z;
		skipdir:
			// Set light 0
			d3ddevice.SetLight(0, ref light);
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
						light.Specular.R = 0.2f;
						light.Specular.G = 0.2f;
						light.Specular.B = 0.2f;
					}
					light.Direction.X = -lightDirection.X;
					light.Direction.Y = -lightDirection.Y;
					light.Direction.Z = -lightDirection.Z;
					d3ddevice.SetLight(3, ref light);
					d3ddevice.EnableLight(3, true);
				}
				else
					d3ddevice.EnableLight(i, false);
		}

		public static void SetDefaultLights(Device d3dDevice, bool reset)
		{
			for (int i = 0; i < 4; i++)
			{
				d3dDevice.EnableLight(i, false);
			}

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
			d3dDevice.SetLight(0, ref keyLight);
			d3dDevice.EnableLight(0, true);
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
			d3dDevice.SetLight(1, ref fillLight);
			d3dDevice.EnableLight(1, true);
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
			d3dDevice.SetLight(2, ref backLight);
			d3dDevice.EnableLight(2, true);
			#endregion
		}

		public static void UpdateDefaultLights(Device d3dDevice)
		{
			d3dDevice.SetLight(0, ref keyLight);
			d3dDevice.EnableLight(0, true);
			d3dDevice.SetLight(1, ref fillLight);
			d3dDevice.EnableLight(1, true);
			d3dDevice.SetLight(2, ref backLight);
			d3dDevice.EnableLight(2, true);
		}

		public static void SetLightType(Device d3ddevice, SADXLightTypes lightType)
		{
			switch (lightType)
			{
				case SADXLightTypes.Level:
					SetStageLights(d3ddevice, stageLights);
					break;
				case SADXLightTypes.Character:
					if (characterLights == null || characterLights.Length == 0)
						SetDefaultLights(d3ddevice, false);
					else
						SetCharacterLight(d3ddevice, stageLights[0].Direction, GetCharacterLight(LSPaletteTypes.Character), (characterLights[0].Level == SA1LevelIDs.Casinopolis && characterLights[0].Act == 0), characterLights[0].Level == SA1LevelIDs.IceCap);
					break;
				case SADXLightTypes.CharacterAlt:
					if (characterLights == null || characterLights.Length == 0)
						SetDefaultLights(d3ddevice, false);
					else
						SetCharacterLight(d3ddevice, stageLights[0].Direction, GetCharacterLight(LSPaletteTypes.CharacterAlt), (characterLights[0].Level == SA1LevelIDs.Casinopolis && characterLights[0].Act == 0), characterLights[0].Level == SA1LevelIDs.IceCap);
					break;
				case SADXLightTypes.Boss:
					if (characterLights == null || characterLights.Length == 0)
						SetDefaultLights(d3ddevice, false);
					else
						SetCharacterLight(d3ddevice, stageLights[0].Direction, GetCharacterLight(LSPaletteTypes.Boss), (characterLights[0].Level == SA1LevelIDs.Casinopolis && characterLights[0].Act == 0), characterLights[0].Level == SA1LevelIDs.IceCap);
					break;
			}
		}

		private static LSPaletteData GetCharacterLight(LSPaletteTypes type)
		{
			foreach (LSPaletteData charlight in characterLights)
			{
				if ((LSPaletteTypes)charlight.Type == type)
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

		public enum LSPaletteTypes
		{
			Character = 0, // Characters, NPCs, Leon, some other enemies
			CharacterAlt = 6, // Gamma, Super Sonic, some Egg Carrier NPCs
			Boss = 8
		}
		#endregion
	}
}