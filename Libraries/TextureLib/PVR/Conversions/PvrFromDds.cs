using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextureLib
{
	public partial class PvrTexture
	{
		/// <summary>Dictionary of PVR and DDS formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		private static Dictionary<PvrPixelFormat, DdsFormat> CompatibleFormatsPvrDds = new Dictionary<PvrPixelFormat, DdsFormat>()
		{
			{ PvrPixelFormat.Rgb565, DdsFormat.Rgb565 },
			{ PvrPixelFormat.Argb1555, DdsFormat.Argb1555 },
			{ PvrPixelFormat.Argb4444, DdsFormat.Argb4444 },
			// Yuv422 could be converted to D3DFMT_UYVY or D3DFMT_YUY2
			// Bump88 could be converted to D3DFMT_V8U8
			// Rgb555 could be converted to D3DFMT_X1R5G5B5
		};

		/// <summary>Create a new PVR texture from a DDS texture, data format determined automatically.</summary>
		public PvrTexture(DdsTexture dds, bool forceMipmaps = false)
		{
			PvrDataFormat targetPvrDataFormat = AutoPvrDataFormatFromImage(dds.Image, forceMipmaps ? true : dds.HasMipmaps);
			ConvertFromDds(dds, targetPvrDataFormat);
		}

		/// <summary>Create a new PVR texture from a DDS texture, data format specified manually.</summary>
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
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			PvmxOriginalDimensions = dds.PvmxOriginalDimensions;
			// Get texture alpha level
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(dds.Image);
			// Check if mipmap generation is necessary
			bool forceMipmaps = false;
			if (!dds.HasMipmaps)
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
			// Select the appropriate pixel codecs
			switch (dds.DdsFormat)
			{
				case DdsFormat.Rgb565:
					PvrPixelFormat = PvrPixelFormat.Rgb565;
					break;
				case DdsFormat.Rgb888:
				case DdsFormat.Argb8888:
					PvrPixelFormat = PvrPixelFormat.Argb8888Alt;
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
			// VQ textures have to be encoded
			if (PvrDataFormat == PvrDataFormat.Vq || PvrDataFormat == PvrDataFormat.VqMipmaps || PvrDataFormat == PvrDataFormat.SmallVq || PvrDataFormat == PvrDataFormat.SmallVqMipmaps)
				losslessConversion = false;
			// Set pixel and data codecs
			PixelCodec pixelCodec = losslessConversion ? new Bypass16BitPixelCodec() : PixelCodec.GetPixelCodec(PvrPixelFormat);
			PvrDataCodec outputDataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
			DdsDataCodec inputDataCodec = losslessConversion ? new LinearDataCodec(pixelCodec) : DdsDataCodec.GetDataCodec(dds.DdsFormat, pixelCodec, alphaLevel != BitmapAlphaLevel.None);
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
				// Copy mipmaps from the original texture if they exist
				if (HasMipmaps && dds.HasMipmaps)
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
				// If the source texture doesn't have mipmaps but the target format does, add them
				if (forceMipmaps)
					AddMipmaps();
				// Update the texture's mipmap and preview images
				Decode();
			}
		}
	}
}