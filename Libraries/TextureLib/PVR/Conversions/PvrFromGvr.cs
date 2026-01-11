using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextureLib
{
	public partial class PvrTexture
	{
		/// <summary>Dictionary of PVR and GVR formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		private static Dictionary<PvrPixelFormat, GvrDataFormat> CompatibleFormatsPvrGvr = new Dictionary<PvrPixelFormat, GvrDataFormat>()
		{
			{ PvrPixelFormat.Rgb565, GvrDataFormat.Rgb565 }
			// Bump88 could be converted to IntensityA8 I guess?
		};

		/// <summary>Create a new PVR texture from a GVR texture, data format determined automatically.</summary>
		/// <param name="gvr">Source GVR texture.</param>
		/// <param name="forceMipmaps">Add mipmaps if the source texture doesn't have them.</param>
		/// <param name="saCompatible">Use SADX/SA2B compatible palette formats: ARGB1555 instead of Intensity8A8 and ARGB4444 instead of RGB5A3.</param>
		/// <param name="maxQuality">Use the ARGB8888 pixel format when a lossless conversion is impossible.</param>
		public PvrTexture(GvrTexture gvr, bool forceMipmaps = false, bool saCompatible = true, bool maxQuality = false)
		{
			PvrDataFormat targetPvrDataFormat = AutoPvrDataFormatFromImage(gvr.Image, forceMipmaps ? true : gvr.HasMipmaps);
			ConvertFromGvr(gvr, targetPvrDataFormat, saCompatible, maxQuality);
		}

		/// <summary>Create a new PVR texture from a GVR texture, data format specified manually.</summary>
		/// <param name="targetPvrDataFormat">Target PVR data format.</param>
		/// <param name="forceMipmaps">Add mipmaps if the source texture doesn't have them.</param>
		/// <param name="saCompatible">Use SADX/SA2B compatible palette formats: ARGB1555 instead of Intensity8A8 and ARGB4444 instead of RGB5A3.</param>
		/// <param name="maxQuality">Use the ARGB8888 pixel format when a lossless conversion is impossible.</param>
		public PvrTexture(GvrTexture gvr, PvrDataFormat targetPvrDataFormat, bool saCompatible = true, bool maxQuality = false)
		{
			ConvertFromGvr(gvr, targetPvrDataFormat, saCompatible, maxQuality);
		}

		private void ConvertFromGvr(GvrTexture gvr, PvrDataFormat targetPvrDataFormat, bool saCompatible = true, bool maxQuality = false)
		{
			// Set common texture properties
			Image = gvr.Image;
			Gbix = gvr.Gbix;
			Name = gvr.Name;
			Width = gvr.Width;
			Height = gvr.Height;
			Palette = gvr.Palette;
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = gvr.PaletteBank;
			PaletteStartIndex = gvr.PaletteStartIndex;
			PakMetadata = gvr.PakMetadata;
			PvmxOriginalDimensions = gvr.PvmxOriginalDimensions;
			// Check if mipmap generation is necessary
			bool forceMipmaps = false;
			if (!gvr.HasMipmaps)
			{
				switch (PvrDataFormat)
				{
					case PvrDataFormat.SquareTwiddledMipmaps:
						PvrDataFormat = PvrDataFormat.SquareTwiddled;
						forceMipmaps = true;
						break;
					case PvrDataFormat.SquareTwiddledMipmapsDma:
						PvrDataFormat = PvrDataFormat.SquareTwiddled;
						forceMipmaps = true;
						break;
					case PvrDataFormat.RectangleMipmaps:
						PvrDataFormat = PvrDataFormat.Rectangle;
						forceMipmaps = true;
						break;
					case PvrDataFormat.RectangleStrideMipmaps:
						PvrDataFormat = PvrDataFormat.RectangleStride;
						forceMipmaps = true;
						break;
					case PvrDataFormat.SmallVqMipmaps:
						PvrDataFormat = PvrDataFormat.SmallVq;
						forceMipmaps = true;
						break;
					case PvrDataFormat.VqMipmaps:
						PvrDataFormat = PvrDataFormat.Vq;
						forceMipmaps = true;
						break;
					case PvrDataFormat.Index4Mipmaps:
						PvrDataFormat = PvrDataFormat.Index4;
						forceMipmaps = true;
						break;
					case PvrDataFormat.Index8Mipmaps:
						PvrDataFormat = PvrDataFormat.Index8;
						forceMipmaps = true;
						break;
					case PvrDataFormat.BitmapMipmaps:
						PvrDataFormat = PvrDataFormat.Bitmap;
						forceMipmaps = true;
						break;
				}
			}
			// Get texture alpha level
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(gvr.Image);
			// Select the appropriate pixel and data codecs
			switch (gvr.GvrDataFormat)
			{
				case GvrDataFormat.Rgb565:
					PvrPixelFormat = PvrPixelFormat.Rgb565;
					break;
				case GvrDataFormat.Argb8888:
					PvrPixelFormat = PvrPixelFormat.Argb8888Alt;
					break;
				case GvrDataFormat.Rgb5a3:
				case GvrDataFormat.Dxt1:
					if (maxQuality)
						PvrPixelFormat = PvrPixelFormat.Argb8888Alt;
					else
						switch (alphaLevel)
						{
							case BitmapAlphaLevel.None:
								PvrPixelFormat = PvrPixelFormat.Rgb565;
								break;
							case BitmapAlphaLevel.OneBitAlpha:
								PvrPixelFormat = PvrPixelFormat.Argb1555;
								break;
							case BitmapAlphaLevel.FullAlpha:
								PvrPixelFormat = PvrPixelFormat.Argb4444;
								break;
						}
					break;
				case GvrDataFormat.Intensity4:
				case GvrDataFormat.Intensity8:
				case GvrDataFormat.IntensityA44:
				case GvrDataFormat.IntensityA88:
					PvrPixelFormat = PvrPixelFormat.Argb8888Alt; // Could be Bump but who needs that?
					break;
				case GvrDataFormat.Index4:
				case GvrDataFormat.Index8:
					// Texture with an external palette
					if (gvr.RequiresPaletteFile)
					{
						RequiresPaletteFile = true;
						Indexed = true;
						if (gvr.GvrDataFormat == GvrDataFormat.Index4)
							PvrDataFormat = gvr.HasMipmaps ? PvrDataFormat.Index4Mipmaps : PvrDataFormat.Index4;
						else
							PvrDataFormat = gvr.HasMipmaps ? PvrDataFormat.Index8Mipmaps : PvrDataFormat.Index8;
					}
					// Texture with a built-in CLUT
					else
					{
						switch (gvr.GvrPaletteFormat)
						{
							case GvrPaletteFormat.Rgb565:
								PvrPixelFormat = PvrPixelFormat.Rgb565;
								break;
							case GvrPaletteFormat.Argb8888:
								PvrPixelFormat = PvrPixelFormat.Argb8888Alt;
								break;
							case GvrPaletteFormat.IntensityA8orArgb1555:
								PvrPixelFormat = saCompatible ? PvrPixelFormat.Argb1555 : PvrPixelFormat.Argb8888Alt;
								break;
							case GvrPaletteFormat.Rgb5A3orArgb4444:
								if (saCompatible)
									PvrPixelFormat = PvrPixelFormat.Argb4444;
								else if (maxQuality)
									PvrPixelFormat = PvrPixelFormat.Argb8888Alt;
								else
								{
									switch (alphaLevel)
									{
										case BitmapAlphaLevel.None:
											PvrPixelFormat = PvrPixelFormat.Rgb565;
											break;
										case BitmapAlphaLevel.OneBitAlpha:
											PvrPixelFormat = PvrPixelFormat.Argb1555;
											break;
										case BitmapAlphaLevel.FullAlpha:
											PvrPixelFormat = PvrPixelFormat.Argb4444;
											break;
									}
								}
								break;
						}
					}
					break;
				case GvrDataFormat.Index14:
				default:
					throw new NotImplementedException(string.Format("Conversion from GVR Format {0} ({1}) to PVR is not implemented.", gvr.GvrDataFormat.ToString(), gvr.GvrDataFormat));
			}
			bool index8 = false;
			MemoryStream outputStream = new MemoryStream();
			// If the formats are compatible with lossless conversion, use the "bypass" codec.
			bool losslessConversion = false;
			foreach (var item in CompatibleFormatsPvrGvr)
			{
				if (item.Key == PvrPixelFormat && item.Value == gvr.GvrDataFormat)
					losslessConversion = true;
			}
			// Also check Indexed formats which are eligible for lossless conversion
			if (gvr.GvrDataFormat == GvrDataFormat.Index4 || gvr.GvrDataFormat == GvrDataFormat.Index8)
			{
				// GVRs with internal CLUT can be converted losslessly to non-paletted PVRs
				if (!gvr.RequiresPaletteFile)
				{
					if (gvr.GvrPaletteFormat == GvrPaletteFormat.Rgb565)
						losslessConversion = true;
					else if (gvr.GvrPaletteFormat == GvrPaletteFormat.IntensityA8orArgb1555 && saCompatible)
						losslessConversion = true;
					else if (gvr.GvrPaletteFormat == GvrPaletteFormat.Rgb5A3orArgb4444 && saCompatible)
						losslessConversion = true;
				}
			}
			// If a lossless conversion cannot be done, encode the texture from the preview Image.
			if (!losslessConversion)
			{
				Encode();
				if (forceMipmaps)
					AddMipmaps();
			}
			// If the conversion is lossless, unwrap the pixels using the "bypass" GVR data codec and wrap it using the PVR data codec and the "bypass" pixel codec.
			else
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion mode");
#endif
				// Set pixel and data codecs
				PixelCodec pixelCodec = losslessConversion ? new Bypass16BitPixelCodec() : PixelCodec.GetPixelCodec(PvrPixelFormat);
				PvrDataCodec outputDataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
				GvrDataCodec inputDataCodec = GvrDataCodec.GetDataCodec(gvr.GvrDataFormat);
				if (losslessConversion)
				{
					switch (gvr.GvrDataFormat)
					{
						case GvrDataFormat.Index4:
							inputDataCodec = new GvrIndex4DataCodec();
							index8 = false;
							break;
						case GvrDataFormat.Index8:
							inputDataCodec = new GvrIndex8DataCodec();
							index8 = true;
							break;
						default:
							inputDataCodec = new GvrBypass16BitDataCodec();
							break;
					}
				}
				int currentOffset = 0;
				// Account for CLUT
				if (gvr.Indexed && !gvr.RequiresPaletteFile)
				{
					PixelCodec paletteCodec = PixelCodec.GetPixelCodec(gvr.GvrPaletteFormat, saCompatible);
					int paletteSize = paletteCodec.BytesPerPixel * GvrDataCodec.GetDataCodec(gvr.GvrDataFormat).PaletteEntries;
					currentOffset += paletteSize;
				}
				// If the source and target formats have mipmaps, unwrap them and write in the destination codec
				if (gvr.HasMipmaps && outputDataCodec.HasMipmaps)
				{
					List<byte[]> mipmaps = new List<byte[]>();
					int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
					for (int mipmapIndex = 0; mipmapIndex < mipLevels; mipmapIndex++)
					{
						int mipOffset = currentOffset;
						int size = Width;
						for (int i = 0; i < mipmapIndex; i++, size >>= 1)
						{
							mipOffset += inputDataCodec.CalculateTextureSize(size, size);
						}
						byte[] mipData = gvr.HeaderlessData[mipOffset..].ToArray();
						byte[] mipRawData = inputDataCodec.Decode(mipData, size, size, null);
						// Apply palette to mipmap if needed
						if (gvr.Indexed && !RequiresPaletteFile)
						{
							mipRawData = ApplyPaletteRaw(mipRawData, size, size, index8, true).ToArray();
							Console.WriteLine(outputDataCodec.CalculateTextureSize(Width, Height).ToString());
						}
						mipmaps.Add(outputDataCodec.Encode(mipRawData, size, size));
					}
					// Write mipmaps to PVR in the reverse order
					for (int i = mipmaps.Count - 1; i > 0; i--)
					{
						outputStream.Write(mipmaps[i]);
					}
				}
				// Main texture
				byte[] maintex = inputDataCodec.Decode(gvr.HeaderlessData[currentOffset..], Width, Height, null);
				// Apply the palette if converting from an indexed GVR with a built-in CLUT
				if (gvr.Indexed && !RequiresPaletteFile)
				{
					maintex = ApplyPaletteRaw(maintex, Width, Height, index8, true).ToArray();
				}
				// Encode the main texture
				outputStream.Write(outputDataCodec.Encode(maintex, Width, Height));
				// Update the texture's data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Force add mipmaps if necessary
				if (forceMipmaps)
					AddMipmaps();
				// Update the texture's mipmaps and preview image
				Decode();
			}			
		}
	}
}