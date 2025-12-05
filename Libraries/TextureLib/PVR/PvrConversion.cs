using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Various functions to convert PVR to/from GVR or DDS losslessly

namespace TextureLib
{
	public partial class PvrTexture
	{
		/// <summary>Dictionary of PVR and GVR formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		public static Dictionary<PvrPixelFormat, GvrDataFormat> CompatibleFormatsPvrGvr = new Dictionary<PvrPixelFormat, GvrDataFormat>()
		{
			{ PvrPixelFormat.Rgb565, GvrDataFormat.Rgb565 }
			// Bump88 could be converted to IntensityA8 I guess?
		};

		/// <summary>Dictionary of PVR and DDS formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		public static Dictionary<PvrPixelFormat, DdsFormat> CompatibleFormatsPvrDds = new Dictionary<PvrPixelFormat, DdsFormat>()
		{
			{ PvrPixelFormat.Rgb565, DdsFormat.Rgb565 },
			{ PvrPixelFormat.Argb1555, DdsFormat.Argb1555 },
			{ PvrPixelFormat.Argb4444, DdsFormat.Argb4444 },
			// Yuv422 could be converted to D3DFMT_UYVY or D3DFMT_YUY2
			// Bump88 could be converted to D3DFMT_V8U8
			// Rgb555 could be converted to D3DFMT_X1R5G5B5
		};

		public PvrTexture(PvrTexture pvr, PvrPixelFormat targetPixelFormat, PvrDataFormat targetPvrDataFormat)
		{
			// Set common texture properties
			Image = pvr.Image;
			Gbix = pvr.Gbix;
			Name = pvr.Name;
			Width = pvr.Width;
			Height = pvr.Height;
			Palette = pvr.Palette;
			PvrPixelFormat = targetPixelFormat;
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = pvr.PaletteBank;
			PaletteStartIndex = pvr.PaletteStartIndex;
			PakMetadata = pvr.PakMetadata;
			bool lossless = true;
			// If pixel format doesn't match, perform a full conversion
			if (pvr.PvrPixelFormat != targetPixelFormat)
				lossless = false;
			// And don't fuck with VQ and SmallVQ textures!
			else if (pvr.PvrDataFormat == PvrDataFormat.Vq && PvrDataFormat == PvrDataFormat.SmallVq)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.Vq && PvrDataFormat == PvrDataFormat.SmallVqMipmaps)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.VqMipmaps && PvrDataFormat == PvrDataFormat.SmallVq)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.VqMipmaps && PvrDataFormat == PvrDataFormat.SmallVqMipmaps)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.SmallVq && PvrDataFormat == PvrDataFormat.Vq)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.SmallVq && PvrDataFormat == PvrDataFormat.VqMipmaps)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.SmallVqMipmaps && PvrDataFormat == PvrDataFormat.Vq)
				lossless = false;
			else if (pvr.PvrDataFormat == PvrDataFormat.SmallVqMipmaps && PvrDataFormat == PvrDataFormat.VqMipmaps)
				lossless = false;
			// Lossy encoding
			if (!lossless)
			{
				Encode();
				Decode();
			}
			// Lossless conversion
			else
			{
				// Set mipmap flag
				switch (PvrDataFormat)
				{
					case PvrDataFormat.Index4Mipmaps:
					case PvrDataFormat.Index8Mipmaps:
					case PvrDataFormat.SmallVqMipmaps:
					case PvrDataFormat.VqMipmaps:
					case PvrDataFormat.SquareTwiddledMipmaps:
					case PvrDataFormat.SquareTwiddledMipmapsAlt:
						HasMipmaps = true;
						break;
				}
				// For most formats, use lossless conversion
				switch (pvr.PvrPixelFormat)
				{
					case PvrPixelFormat.Rgb565:
					case PvrPixelFormat.Argb1555:
					case PvrPixelFormat.Argb4444:
					case PvrPixelFormat.Bump88:
					case PvrPixelFormat.Yuv422:
					case PvrPixelFormat.Rgb555:
						Console.WriteLine("Using lossless conversion mode");
						MemoryStream outputStream = new();
						PvrDataCodec inputDataCodec = PvrDataCodec.Create(pvr.PvrDataFormat, new Bypass16BitPixelCodec());
						PvrDataCodec outputDataCodec = PvrDataCodec.Create(PvrDataFormat, new Bypass16BitPixelCodec());
						ReadOnlySpan<byte> palette = pvr.DecodeInternalPalette(inputDataCodec, out int paletteSize);
						if (paletteSize != 0)
							outputStream.Write(palette);
						int textureAddress = pvr.HeaderlessData.Length - inputDataCodec.CalculateTextureSize(Width, Height);
						ReadOnlySpan<byte> textureData = pvr.HeaderlessData[textureAddress..];
						byte[] mainTexRaw = inputDataCodec.Decode(textureData, Width, Height, palette);
						// Mipmaps come first in PVR
						if (HasMipmaps)
						{
							// Calculate the number of mip levels
							int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
							int[] mipmapOffsets = new int[mipLevels];
							// Start offset for the first mipmap
							int mipmapOffset = paletteSize;
							for (int i = mipLevels - 1, sizex = 1; i >= 0; i--, sizex <<= 1)
							{
								mipmapOffsets[i] = mipmapOffset;
								byte[] src = pvr.HeaderlessData[mipmapOffsets[i]..];
								int mipDataSize = Math.Max(1, pvr.HeaderlessData.Length - paletteSize - src.Length);
								//Console.WriteLine("Mipmap {0} ({1}x{1}) : {2} (size {3})", i, sizex, mipmapOffsets[i].ToString("X"), mipDataSize.ToString());
								byte[] mipRawData = inputDataCodec.Decode(src, sizex, sizex, palette);
								if (HasMipmaps && sizex != Width)
									outputStream.Write(outputDataCodec.Encode(mipRawData, sizex, sizex));
								mipmapOffset += inputDataCodec.CalculateTextureSize(sizex, sizex);
							}
						}
						// Write texture
						outputStream.Write(outputDataCodec.Encode(mainTexRaw, Width, Height));
						// Update the texture's data arrays
						HeaderlessData = outputStream.ToArray();
						RawData = GetBytes();
						// Update the texture's mipmaps and preview image
						Decode();
						break;
					// For other formats, use lossy conversion
					default:
						Encode();
						Decode();
						break;
				}
			}
		}

		public PvrTexture(GvrTexture gvr)
		{
			PvrDataFormat targetPvrDataFormat;
			bool rectangular = gvr.Width != gvr.Height;
			if (rectangular)
				targetPvrDataFormat = PvrDataFormat.RectangleTwiddled;
			else
				targetPvrDataFormat = gvr.HasMipmaps ? PvrDataFormat.SquareTwiddledMipmaps : PvrDataFormat.SquareTwiddled;
			ConvertFromGvr(gvr, targetPvrDataFormat);
		}

		public PvrTexture(DdsTexture dds)
		{
			PvrDataFormat targetPvrDataFormat;
			bool rectangular = dds.Width != dds.Height;
			if (rectangular)
				targetPvrDataFormat = PvrDataFormat.RectangleTwiddled;
			else
				targetPvrDataFormat = dds.HasMipmaps ? PvrDataFormat.SquareTwiddledMipmaps : PvrDataFormat.SquareTwiddled;
			ConvertFromDds(dds, targetPvrDataFormat);
		}

		public PvrTexture(GvrTexture gvr, PvrDataFormat targetPvrDataFormat)
		{
			ConvertFromGvr(gvr, targetPvrDataFormat);
		}

		public PvrTexture(DdsTexture dds, PvrDataFormat targetPvrDataFormat)
		{
			ConvertFromDds(dds, targetPvrDataFormat);
		}

		private void ConvertFromDds(DdsTexture dds, PvrDataFormat targetPvrDataFormat, bool maxQuality = false)
		{
			// Set common texture properties
			Image = dds.Image;
			Gbix = dds.Gbix;
			Name = dds.Name;
			Width = dds.Width;
			Height = dds.Height;
			HasMipmaps = dds.HasMipmaps;
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			// Get texture alpha level
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(dds.Image);
			// Select the appropriate pixel and data codecs
			switch (dds.DdsFormat)
			{
				case DdsFormat.Rgb565:
					PvrPixelFormat = PvrPixelFormat.Rgb565;
					break;
				case DdsFormat.Rgb888:
				case DdsFormat.Argb8888:
					PvrPixelFormat = PvrPixelFormat.Argb8888;
					break;
				case DdsFormat.Argb4444:
					PvrPixelFormat = PvrPixelFormat.Argb4444;
					break;
				case DdsFormat.Argb1555:
					PvrPixelFormat = PvrPixelFormat.Argb1555;
					break;
				case DdsFormat.Dxt1:
				case DdsFormat.Dxt3:
				case DdsFormat.Dxt5:
					if (maxQuality)
						PvrPixelFormat = PvrPixelFormat.Argb8888;
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
				default:
					throw new NotImplementedException(string.Format("Conversion from DDS Format {0} ({1}) to PVR is not implemented.", dds.DdsFormat.ToString(), dds.DdsFormat));
			}
			MemoryStream outputStream = new MemoryStream();
			// If the formats are compatible with lossless conversion, use the "bypass" codec.
			bool losslessConversion = false;
			foreach (var item in CompatibleFormatsPvrDds)
			{
				if (item.Key == PvrPixelFormat && item.Value == dds.DdsFormat)
					losslessConversion = true;
			}
			// Set pixel and data codecs
			PixelCodec pixelCodec = losslessConversion ? new Bypass16BitPixelCodec() : PixelCodec.GetPixelCodec(PvrPixelFormat);
			PvrDataCodec outputDataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
			DdsDataCodec inputDataCodec = losslessConversion ? new LinearDataCodec(pixelCodec) : DdsDataCodec.GetDataCodec(dds.DdsFormat, pixelCodec, alphaLevel != BitmapAlphaLevel.None);
			// If the conversion is lossless, unwrap the pixels using the "bypass" GVR data codec and wrap it using the PVR data codec and the "bypass" pixel codec.
			if (losslessConversion)
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion mode");
#endif
				// Mipmaps come first in PVR
				if (HasMipmaps)
				{
					List<byte[]> mipmaps = new List<byte[]>();
					int mipCount = dds.MipmapImages.Count();
					int mipmapOffset = 0;
					int mipSize = Width;
					for (int m = 0; m < mipCount; m++)
					{
						int mipDataSize = inputDataCodec.CalculateTextureSize(mipSize, mipSize);
						ReadOnlySpan<byte> mipData = dds.HeaderlessData[mipmapOffset..];
						byte[] mipmapResult = inputDataCodec.Decode(mipData, mipSize, mipSize, null);
						mipmapOffset += mipDataSize;
						mipmaps.Add(outputDataCodec.Encode(mipmapResult, mipSize, mipSize));
						mipSize >>= 1;
					}
					// Write mipmaps to PVR in the reverse order, skip 0
					for (int i = mipmaps.Count - 1; i > 0; i--)
					{
						outputStream.Write(mipmaps[i]);
					}
				}
				// Write the main texture
				byte[] maintex = inputDataCodec.Decode(dds.HeaderlessData, Width, Height, null);
				outputStream.Write(outputDataCodec.Encode(maintex, Width, Height));
				// Update the texture's data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Update the texture's mipmaps and preview image
				Decode();
			}
			// If a lossless conversion cannot be done, encode the texture from the preview Image.
			else
			{
				Encode();
			}
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
			HasMipmaps = gvr.HasMipmaps;
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = gvr.PaletteBank;
			PaletteStartIndex = gvr.PaletteStartIndex;
			PakMetadata = gvr.PakMetadata;
			// Get texture alpha level
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(gvr.Image);
			// Select the appropriate pixel and data codecs
			switch (gvr.GvrDataFormat)
			{
				case GvrDataFormat.Rgb565:
					PvrPixelFormat = PvrPixelFormat.Rgb565;
					break;
				case GvrDataFormat.Argb8888:
					PvrPixelFormat = PvrPixelFormat.Argb8888;
					break;
				case GvrDataFormat.Rgb5a3:
				case GvrDataFormat.Dxt1:
					if (maxQuality)
						PvrPixelFormat = PvrPixelFormat.Argb8888;
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
					PvrPixelFormat = PvrPixelFormat.Argb8888; // Could be Bump but who needs that?
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
								PvrPixelFormat = PvrPixelFormat.Argb8888;
								break;
							case GvrPaletteFormat.IntensityA8orArgb1555:
								PvrPixelFormat = saCompatible ? PvrPixelFormat.Argb1555 : PvrPixelFormat.Argb8888;
								break;
							case GvrPaletteFormat.Rgb5A3orArgb4444:
								if (saCompatible)
									PvrPixelFormat = PvrPixelFormat.Argb4444;
								else if (maxQuality)
									PvrPixelFormat = PvrPixelFormat.Argb8888;
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
			if (gvr.GvrDataFormat == GvrDataFormat.Rgb565 && PvrPixelFormat == PvrPixelFormat.Rgb565)
				losslessConversion = true;
			else if (gvr.GvrDataFormat == GvrDataFormat.Index4 || gvr.GvrDataFormat == GvrDataFormat.Index8)
			{
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
			// If the conversion is lossless, unwrap the pixels using the "bypass" GVR data codec and wrap it using the PVR data codec and the "bypass" pixel codec.
			if (losslessConversion)
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion mode");
#endif
				int currentOffset = 0;
				// Account for CLUT
				if (gvr.Indexed && !gvr.RequiresPaletteFile)
				{
					PixelCodec paletteCodec = PixelCodec.GetPixelCodec(gvr.GvrPaletteFormat, saCompatible);
					int paletteSize = paletteCodec.BytesPerPixel * GvrDataCodec.GetDataCodec(gvr.GvrDataFormat).PaletteEntries;
					currentOffset += paletteSize;
				}
				// Mipmaps come first in PVR
				if (HasMipmaps)
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
				// Write the main texture
				byte[] maintex = inputDataCodec.Decode(gvr.HeaderlessData[currentOffset..], Width, Height, null);
				Console.WriteLine(inputDataCodec.ToString());
				// Apply the palette if converting from an indexed GVR with a built-in CLUT
				if (gvr.Indexed && !RequiresPaletteFile)
				{
					maintex = ApplyPaletteRaw(maintex, Width, Height, index8, true).ToArray();
					Console.WriteLine(outputDataCodec.CalculateTextureSize(Width, Height).ToString());
				}
				outputStream.Write(outputDataCodec.Encode(maintex, Width, Height));
				// Update the texture's data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Update the texture's mipmaps and preview image
				Decode();
			}
			// If a lossless conversion cannot be done, encode the texture from the preview Image.
			else
			{
				Encode();
				Decode();
			}
		}

		private Span<byte> ApplyPaletteRaw(byte[] src, int width, int height, bool index8, bool bigEndian)
		{			
			byte[] result = new byte[src.Length * 4];
			if (Palette == null)
				Palette = TexturePalette.CreateDefaultPalette(index8);
			for (int pixelID = 0; pixelID < width * height; pixelID++)
			{
				//Console.WriteLine("P "+pixelID.ToString());
				byte decodedID = src[pixelID];
				if (!index8)
				{
					decodedID = (byte)((decodedID) >> 4);
				}
				//Console.WriteLine("A " + decodedID.ToString("X"));
				result[pixelID * 4] = Palette.RawData[bigEndian ? decodedID * 2 + 1 : decodedID * 2];
				result[pixelID * 4 + 1] = Palette.RawData[bigEndian ? decodedID * 2 : decodedID * 2 + 1];
			}
			return result;
		}
	}
}