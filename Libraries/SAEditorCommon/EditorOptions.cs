using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using SplitTools;
using SharpDX.Mathematics.Interop;
using System;
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
		private static bool disableTextures = false;
		private static Device direct3DDevice;
		private static Font onscreenFont;
		private static Light keyLight;
		private static Light backLight;
		private static Light fillLight;
		private static Light currentLight;
		private static bool enableSpecular;
		private static List<SADXStageLightData> stageLights;
		private static List<LSPaletteData> characterLights;
		private static EditorFogSettings fogSettings;

		public static FillMode RenderFillMode { get { return renderFillMode; } set { renderFillMode = value; } }
		public static Cull RenderCullMode { get { return renderCullMode; } set { renderCullMode = value; } }
		public static float RenderDrawDistance { get { return renderDrawDistance; } set { renderDrawDistance = value; } }
		public static float LevelDrawDistance { get { return levelDrawDistance; } set { levelDrawDistance = value; } }
		public static float SetItemDrawDistance { get { return setDrawDistance; } set { setDrawDistance = value; } }
		public static bool OverrideLighting { get { return overrideLighting; } set { overrideLighting = value; } }
		public static bool IgnoreMaterialColors { get { return ignoreMaterialColors; } set { ignoreMaterialColors = value; } }
		public static bool DisableTextures { get { return disableTextures; } set { disableTextures = value; } }
		public static Device Direct3DDevice { get { return direct3DDevice; } set { direct3DDevice = value; } }
		public static Font OnscreenFont { get { return onscreenFont; } set { onscreenFont = value; } }
		public static Color FillColor { get { return fillColor; } set { fillColor = value; } }
		public static List<SADXStageLightData> StageLights { get { return stageLights; } set { stageLights = value; } }
		public static List<LSPaletteData> CharacterLights { get { return characterLights; } set { characterLights = value; } }
		public static Light FillLight { get { return fillLight; } set { fillLight = value; } }
		public static Light KeyLight { get { return keyLight; } set { keyLight = value; } }
		public static Light BackLight { get { return backLight; } set { backLight = value; } }
		public static bool EnableSpecular { get { return enableSpecular; } set { enableSpecular = value; } }
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

			#region Fog setup
			fogSettings = new EditorFogSettings(d3dDevice);
			#endregion
		}

		public static void RenderStateCommonSetup(Device d3ddevice)
		{
			d3ddevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
			d3ddevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
			d3ddevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Anisotropic);
			d3ddevice.SetRenderState(RenderState.Lighting, !overrideLighting);
			d3ddevice.SetRenderState(RenderState.SpecularEnable, enableSpecular);
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
			d3ddevice.SetRenderState(RenderState.FogEnable, false);
		}

		#region Lighting
		/// <summary>
		/// Replaces current lighting with editor lights.
		/// </summary>
		private static void SetDefaultLights(Device d3dDevice)
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

		/// <summary>
		/// Resets editor lights to default values.
		/// </summary>
		public static void ResetDefaultLights()
		{
			enableSpecular = false;
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
				Ambient = new SharpDX.Mathematics.Interop.RawColor4(0.15f, 0.15f, 0.15f, 1),
				Specular = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1),
				Range = 0,
				Direction = Vector3.Normalize(new Vector3(-0.45f, 1f, 0.25f))
			};
			#endregion
		}

		/// <summary>
		/// Increases or decreases default Back Light brightness.
		/// </summary>
		public static void AdjustBackLightBrightness(float value)
		{
			backLight.Ambient.R = Math.Max(0, Math.Min(1.0f, backLight.Ambient.R + value));
			backLight.Ambient.G = Math.Max(0, Math.Min(1.0f, backLight.Ambient.G + value));
			backLight.Ambient.B = Math.Max(0, Math.Min(1.0f, backLight.Ambient.B + value));
		}

		/// <summary>
		/// Sets up lighting using SADX Stage Lights data.
		/// </summary>
		private static void SetStageLights(Device d3ddevice)
		{
			for (int i = 0; i < 4; i++)
				d3ddevice.EnableLight(i, false);
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

		/// <summary>
		/// Sets up lighting using SADX LSPalette data.
		/// </summary>
		private static void SetCharacterLight(Device d3ddevice, Vertex stageLightDirection, LSPaletteData lspalette, bool casino = false, bool icecap = false)
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

		/// <summary>
		/// Sets up lighting according to the SADX light type.
		/// </summary>
		public static void SetLightType(Device d3ddevice, SADXLightTypes lightType)
		{
			switch (lightType)
			{
				case SADXLightTypes.Default:
				default:
					SetDefaultLights(d3ddevice);
					return;
				case SADXLightTypes.Level:
					if (stageLights == null || stageLights.Count == 0)
						SetDefaultLights(d3ddevice);
					else
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

		/// <summary>
		/// Retrieves LSPaletteData entries of the specified type.
		/// </summary>
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

		#region Fog
		public static void SetStageFogData(FogData data)
		{
			fogSettings.UpdateFogData(data);
		}

		public class EditorFogSettings
		{
			private Device D3DDevice;
			private static float[] FogRangeBaseTable; // OtherFogTableA / st_fog_range_base_
			private static float[] FogRangeTable;  // OtherFogTableB / _st_fog_range_
			private static float[] FogTable; // FogTable / fogtable
			private static int FogRangeStart; // st_fog_range_start_
			private static int FogRangeEnd; // st_fog_range_end_
			private static float FogDensityStart; // st_fog_density_start_
			private static float FogDensityEnd; // st_fog_density_end_

			// Crap maths stuff
			private const int FLT_EXP_MASK = 0x7f800000;
			private const int FLT_MANT_BITS = 23;
			private const int FLT_SGN_MASK = -1 - 0x7fffffff;
			private const int FLT_MANT_MASK = 0x007fffff;
			private const int FLT_EXP_CLR_MASK = FLT_SGN_MASK | FLT_MANT_MASK;

			/// <summary>
			/// Initializes editor fog and creates a base table used for exponential fog.
			/// </summary>
			public EditorFogSettings(Device device)
			{
				D3DDevice = device;
				float v1; // st7
				int v2; // [esp+30h] [ebp-4h]
				FogRangeBaseTable = new float[128];
				for (int v0 = 0; v0 < 128; v0++)
				{
					v1 = (float)System.Math.Pow(2.0f, (double)((v0 >> 4) & 0xF));
					v2 = v0 & 0xF;
					FogRangeBaseTable[v0] = (float)(v1 * ((double)v2 + 16.0f) * 0.0625f);
				}
				SetFogDensity(0x8010);
			}

			/// <summary>
			/// Main fog update function that should be called in the level. Corrects the level fog data values, generates and then applies the fog table.
			/// </summary>
			public void UpdateFogData(FogData LevelFogData)
			{
				float fogstart; // [esp+0h] [ebp-8h]
				float fogend; // [esp+4h] [ebp-4h]

				fogstart = LevelFogData.FogStart;
				fogend = LevelFogData.FogEnd;

				if (LevelFogData.FogStart > 0.0)
				{
					fogstart = -LevelFogData.FogStart;
				}
				if (LevelFogData.FogEnd > 0.0)
				{
					fogend = -LevelFogData.FogEnd;
				}
				njGenerateFogTable3(fogstart, fogend);
				njSetFogTable(FogTable);
			}

			/// <summary>
			/// Fog table generation, decompiled by Exant.
			/// </summary>
			private void njGenerateFogTable3(float n, float f)
			{
				FogTable = new float[128];
				int v4; // ebx
				ushort v5; // cx
				uint significant; // rax
				double v7; // st6
				int v21; // ecx
				uint v23; // eax
				int exponent = 0; // [esp+14h] [ebp-4h] BYREF

				v4 = 0;
				if (n >= -65535.0)
				{
					significant = (uint)(frexp(-f, ref exponent) * 65536.0);
					v5 = (ushort)(significant & 0xFF00 | (char)--exponent);
				}
				else
				{
					v5 = 0x8010;
				}
				SetFogDensity(v5);
				if (f / 256.0f <= n)
				{
					v7 = 1.0;
				}
				else
				{
					v7 = -f;
				}
				for (int i = 0; i < 128; i++)
				{
					FogTable[i] = (float)(v7 / ((double)(1 << (i >> 4)) * ((i & 0xF) + 16.0) / 16.0f));
					if (FogTable[i] > -n)
					{
						++v4;
					}
				}
				if (v7 == -f)
				{
					v21 = 0;
					v23 = ((uint)(v4 - 4) / 4) + 1;
					if (v4 >= 4)
					{
						v21 = 4 * (int)v23;
						for (int i = 0; i < v23; i++)
						{
							FogTable[i * 4] = (float)(1.0 - (f + FogTable[i * 4]) / (f + FogTable[v4]));
							FogTable[i * 4 + 1] = (float)(1.0 - (f + FogTable[i * 4 + 1]) / (f + FogTable[v4]));
							FogTable[i * 4 + 2] = (float)(1.0 - (f + FogTable[i * 4 + 2]) / (f + FogTable[v4]));
							FogTable[i * 4 + 3] = (float)(1.0 - (f + FogTable[i * 4 + 3]) / (f + FogTable[v4]));
						}
					}
					if (v21 < v4)
					{
						for (int i = v21; i < v4; i++)
							FogTable[i] = (float)(1.0 - (f + FogTable[i]) / (f + FogTable[v4]));
					}
					if (v4 < 128)
					{
						for (int i = v4; i < 128; i++) FogTable[i] = 0.0f;
					}
				}
			}

			/// <summary>
			/// Sets main values for range and density start/end, then applies the fog table.
			/// </summary>
			private void njSetFogTable(float[] fogtable)
			{
				int index_range_start; // esi
				int index_range_end; // edx
				int b; // eax
				int c; // ecx

				index_range_end = 126;
				b = 0;
				while (fogtable[b] == 1.0f)
				{
					if (++b == 126)
					{
						goto LABEL_6;
					}
				}
				index_range_end = b;
			LABEL_6:
				c = 126;
				index_range_start = index_range_end + 1;
				if (index_range_end != 126)
				{
					while (0.0 == fogtable[c])
					{
						if (--c == index_range_end)
						{
							goto LABEL_11;
						}
					}
					index_range_start = c + 1;
				}
			LABEL_11:
				FogRangeStart = index_range_start;
				FogRangeEnd = index_range_end;
				FogDensityStart = fogtable[index_range_start];
				FogDensityEnd = fogtable[index_range_end];
				SetFogTable();
			}

			/// <summary>
			/// Sets fog density, then applies the fog table.
			/// </summary>
			private void SetFogDensity(ushort n)
			{
				int i; // ecx
				double v2; // st6
				FogRangeTable = new float[128];
				for (i = 0; i != 128; ++i)
				{
					v2 = 1.0 / ((double)(1 << n) * ((double)(n & 0xFF00) * 0.000015258789)) * FogRangeBaseTable[i];
					if (v2 <= 0.000015258789)
					{
						FogRangeTable[i] = 65536.0f;
					}
					else
					{
						FogRangeTable[i] = 1.0f / (float)v2;
					}
				}
				SetFogTable();
			}

			/// <summary>
			/// Applies the fog table on the Direct3D side of things.
			/// </summary>
			private void SetFogTable()
			{
				double v0; // fp13
				double v1; // fp0
				double v2; // fp12
				double v3; // fp31
				double v4; // fp31
				double v5; // fp30
				double v6; // fp30
				double v7; // fp31

				v0 = FogDensityEnd;
				v1 = FogDensityStart;
				v2 = (float)(FogDensityEnd - FogDensityStart);
				if (v0 >= 0.9)
				{
					if (v2 <= 0.0)
					{
						if (v1 >= 0.3)
						{
							if (v1 <= 0.3 || v1 >= 0.6)
							{
								v6 = 0.0;
								v7 = 100.0;
							}
							else
							{
								v6 = 0.0;
								v7 = 32767.0;
							}
						}
						else
						{
							v6 = 65534.0;
							v7 = 65535.0;
						}
					}
					else
					{
						v6 = (float)((float)((float)-v1
										   * (float)((float)(FogRangeTable[FogRangeEnd] - FogRangeTable[FogRangeStart])
												   / (float)(FogDensityEnd - FogDensityStart)))
								   + FogRangeTable[FogRangeStart]);
						v7 = (float)((float)((float)((float)1.0 - FogDensityEnd)
										   * (float)((float)(FogRangeTable[FogRangeEnd] - FogRangeTable[FogRangeStart])
												   / (float)(FogDensityEnd - FogDensityStart)))
								   + FogRangeTable[FogRangeEnd]);
						if (v6 < 0.0)
							v6 = 0.0;
						if (v7 > 65535.0)
							v7 = 65535.0;
					}
					D3DDevice.SetRenderState(RenderState.FogTableMode, 3);
					D3DDevice.SetRenderState(RenderState.FogStart, (float)v6);
					D3DDevice.SetRenderState(RenderState.FogEnd, (float)v7);
				}
				else if (v2 <= 1.0)
				{
					if (v1 >= 0.30000001)
					{
						if (v1 <= 0.30000001 || v1 >= 0.60000002)
							v5 = 100.0;
						else
							v5 = 32767.0;
						v4 = 0.0;
					}
					else
					{
						v4 = 65534.0;
						v5 = 65535.0;
					}
					D3DDevice.SetRenderState(RenderState.FogTableMode, 3);
					D3DDevice.SetRenderState(RenderState.FogStart, (float)v4);
					D3DDevice.SetRenderState(RenderState.FogEnd, (float)v5);
				}
				else if (v1 < 0.5)
				{
					v3 = (float)(Math.Sqrt(
								   Math.Log((float)-(float)((float)((float)(FogDensityEnd + FogDensityStart) * (float)0.5)
													 - (float)1.0))
								 * -1.0)
							   * (1.0
								/ (float)((float)(FogRangeTable[FogRangeEnd] + FogRangeTable[FogRangeStart]) * (float)0.5)));
					D3DDevice.SetRenderState(RenderState.FogTableMode, 2);
					D3DDevice.SetRenderState(RenderState.FogDensity, (float)v3);
				}
				else
				{
					D3DDevice.SetRenderState(RenderState.FogTableMode, 3);
					D3DDevice.SetRenderState(RenderState.FogStart, 0.0f);
					D3DDevice.SetRenderState(RenderState.FogEnd, 100.0f);
				}
			}

			private static void LogFogTables()
			{
				string logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SALVL Fog.log");
				if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logPath)))
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));
				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				builder.AppendLine("FogA");
				for (int i = 0; i < 128; i++)
					builder.AppendLine(i.ToString() + ": " + FogRangeBaseTable[i].ToString());
				builder.AppendLine("FogB");
				for (int i = 0; i < 128; i++)
					builder.AppendLine(i.ToString() + ": " + FogRangeTable[i].ToString());
				builder.AppendLine("FogC");
				for (int i = 0; i < 128; i++)
					builder.AppendLine(i.ToString() + ": " + FogTable[i].ToString());
				System.IO.File.WriteAllText(logPath, builder.ToString());
			}

			/// <summary>
			/// Extracts exponent and significand from a float.
			/// </summary>
			private static float frexp(float number, ref int exponent)
			{
				// From https://github.com/MachineCognitis/C.math.NET/blob/master/C.math/math.cs#L644
				int bits = SingleToInt32Bits(number);
				int exp = (int)((bits & FLT_EXP_MASK) >> FLT_MANT_BITS);
				exponent = 0;

				if (exp == 0xff || number == 0F)
					number += number;
				else
				{
					// Not zero and finite.
					exponent = exp - 126;
					if (exp == 0)
					{
						// Subnormal, scale number so that it is in [1, 2).
						number *= Int32BitsToSingle(0x4c000000); // 2^25
						bits = SingleToInt32Bits(number);
						exp = (int)((bits & FLT_EXP_MASK) >> FLT_MANT_BITS);
						exponent = exp - 126 - 25;
					}
					// Set exponent to -1 so that number is in [0.5, 1).
					number = Int32BitsToSingle((bits & FLT_EXP_CLR_MASK) | 0x3f000000);
				}
				return number;
			}

			private static int SingleToInt32Bits(float value)
			{
				return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
			}

			private static float Int32BitsToSingle(int value)
			{
				return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
			}
			#endregion
		}
	}
}