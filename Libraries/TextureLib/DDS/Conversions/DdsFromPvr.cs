using System;
using System.Collections.Generic;
using System.IO;

namespace TextureLib
{
	public partial class DdsTexture
	{
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

		/// <summary>Create a new DDS texture from a PVR texture, data format determined automatically.</summary>
		public DdsTexture(PvrTexture pvr, bool forceMipmaps = false, bool maxQuality = false)
		{
			DdsFormat targetFormat;
			switch (pvr.PvrPixelFormat)
			{
				case PvrPixelFormat.Rgb565:
					targetFormat = DdsFormat.Rgb565;
					break;
				case PvrPixelFormat.Argb4444:
					targetFormat = DdsFormat.Argb4444;
					break;
				case PvrPixelFormat.Argb1555:
				case PvrPixelFormat.Rgb555:
					targetFormat = DdsFormat.Argb1555;
					break;
				case PvrPixelFormat.Argb8888Alt:
				case PvrPixelFormat.Argb8888:
				default:
					targetFormat = DdsFormat.Argb8888;
					break;
			}
			// VQ textures -> DXT textures?
			if (pvr.PvrDataFormat == PvrDataFormat.Vq || pvr.PvrDataFormat == PvrDataFormat.VqMipmaps ||
				pvr.PvrDataFormat == PvrDataFormat.SmallVq || pvr.PvrDataFormat == PvrDataFormat.SmallVqMipmaps)
				if (!maxQuality)
					targetFormat = AutoDdsFormatFromImage(pvr.Image, maxQuality, maxQuality ? false : true);
			ConvertFromPvr(pvr, targetFormat, forceMipmaps);
		}

		/// <summary>Create a new DDS texture from a PVR texture, data format specified manually.</summary>
		public DdsTexture(PvrTexture pvr, DdsFormat targetFormat, bool forceMipmaps = false)
		{
			ConvertFromPvr(pvr, targetFormat, forceMipmaps);
		}

		private void ConvertFromPvr(PvrTexture pvr, DdsFormat targetFormat, bool forceMipmaps = false)
		{
			// Set common texture properties
			Image = pvr.Image;
			Gbix = pvr.Gbix;
			Name = pvr.Name;
			Width = pvr.Width;
			Height = pvr.Height;
			HasMipmaps = forceMipmaps ? true : pvr.HasMipmaps;
			PaletteBank = pvr.PaletteBank;
			PaletteStartIndex = pvr.PaletteStartIndex;
			PakMetadata = pvr.PakMetadata;
			DdsFormat = targetFormat;
			PvmxOriginalDimensions = pvr.PvmxOriginalDimensions;
			// Check lossless
			bool lossless = false;
			foreach (var item in CompatibleFormatsPvrDds)
				if (item.Key == pvr.PvrPixelFormat && item.Value == DdsFormat)
					lossless = true;
			// But not VQ or Indexed or Bitmap
			switch (pvr.PvrDataFormat)
			{
				case PvrDataFormat.Bitmap:
				case PvrDataFormat.BitmapMipmaps:
				case PvrDataFormat.Index4:
				case PvrDataFormat.Index4Mipmaps:
				case PvrDataFormat.Index8:
				case PvrDataFormat.Index8Mipmaps:
				case PvrDataFormat.SmallVq:
				case PvrDataFormat.SmallVqMipmaps:
				case PvrDataFormat.Vq:
				case PvrDataFormat.VqMipmaps:
					lossless = false;
					break;
			}
			// Lossy conversion
			if (!lossless)
			{
				Encode();
			}
			// Lossless conversion
			else
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion");
#endif
				PvrDataCodec inputDataCodec = PvrDataCodec.Create(pvr.PvrDataFormat, new Bypass16BitPixelCodec());
				DdsDataCodec outputDataCodec = DdsDataCodec.GetDataCodec(DdsFormat, new Bypass16BitPixelCodec(), true);
				MemoryStream outputStream = new();
				// Original texture data
				int textureAddress = pvr.HeaderlessData.Length - inputDataCodec.CalculateTextureSize(Width, Height);
				ReadOnlySpan<byte> textureData = pvr.HeaderlessData[textureAddress..];
				byte[] mainTexRaw = inputDataCodec.Decode(textureData, Width, Height, null);
				// Write the original texture
				outputStream.Write(outputDataCodec.Encode(mainTexRaw, Width, Height));
				// Unwrap mipmaps, if present and required
				if (HasMipmaps && pvr.HasMipmaps)
				{
					// Calculate the number of mip levels
					int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
					// Convert mipmaps if they're already there
					for (int i = mipLevels - 1, sizex = 1; i >= 0; i--, sizex <<= 1)
					{
						int[] mipmapOffsets = new int[mipLevels];
						// Start offset for the first mipmap
						int mipmapOffset = 0;
						mipmapOffsets[i] = mipmapOffset;
						byte[] srcdata = pvr.HeaderlessData[mipmapOffsets[i]..];
						int mipDataSize = Math.Max(1, pvr.HeaderlessData.Length - srcdata.Length);
						byte[] mipRawData = inputDataCodec.Decode(srcdata, sizex, sizex, null);
						if (HasMipmaps && sizex != Width)
							outputStream.Write(outputDataCodec.Encode(mipRawData, sizex, sizex));
						mipmapOffset += inputDataCodec.CalculateTextureSize(sizex, sizex);
					}
				}
				// Update raw data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Force mipmaps if required
				if (HasMipmaps && !pvr.HasMipmaps)
				{
					HasMipmaps = false;
					AddMipmaps();
				}
			}
		}
	}
}