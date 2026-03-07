using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TextureLib
{
	public partial class GvrTexture
	{
		/// <summary>Dictionary of GVR and DDS formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		private static Dictionary<DdsFormat, GvrDataFormat> CompatibleFormatsGvrDds = new Dictionary<DdsFormat, GvrDataFormat>()
		{
			{ DdsFormat.Rgb565, GvrDataFormat.Rgb565 },
			{ DdsFormat.Dxt1, GvrDataFormat.Dxt1 }
			// D3DFMT_A4L4 could be converted to IntensityA4
			// D3DFMT_L8 or D3DFMT_A8 could be converted to Intensity8
			// D3DFMT_A8L8 could be converted to IntensityA8
		};

		/// <summary>Create a new GVR texture from a DDS texture, data format determined automatically.</summary>
		public GvrTexture(DdsTexture dds, bool forceMipmaps = false, bool maxQuality = false)
		{
			GvrDataFormat targetGvrDataFormat = AutoGvrDataFormatFromDds(dds.DdsFormat, maxQuality);
			ConvertFromDds(dds, targetGvrDataFormat, forceMipmaps);
		}

		/// <summary>Create a new GVR texture from a DDS texture, data format specified manually.</summary>
		public GvrTexture(DdsTexture dds, GvrDataFormat targetGvrDataFormat, bool forceMipmaps = false)
		{
			ConvertFromDds(dds, targetGvrDataFormat, forceMipmaps);
		}

		private void ConvertFromDds(DdsTexture dds, GvrDataFormat targetGvrDataFormat, bool forceMipmaps = false)
		{
			// Set common texture properties
			Image = dds.Image;
			Gbix = dds.Gbix;
			Name = dds.Name;
			Width = dds.Width;
			Height = dds.Height;
			GvrDataFormat = targetGvrDataFormat;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			PvmxOriginalDimensions = dds.PvmxOriginalDimensions;
			bool lossless = false;
			foreach (var item in CompatibleFormatsGvrDds)
			{
				if (item.Key == dds.DdsFormat && item.Value == GvrDataFormat)
					lossless = true;
			}
			// If lossless conversion is impossible, perform a full encode
			if (!lossless)
			{
				HasMipmaps = forceMipmaps ? true : dds.HasMipmaps;
				Encode();
			}
			// Lossless conversion
			else
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion");
#endif
				MemoryStream outputStream = new MemoryStream();
				// If it's Dxt1, use the DXT converter
				if (dds.DdsFormat == DdsFormat.Dxt1)
				{
					DdsDataCodec inputCodec = new BCDataCodec(null) { CompressionFormat = BCnEncoder.Shared.CompressionFormat.Bc1WithAlpha };
					GvrDataCodec gvrDxtCodec = new GvrDXT1DataCodec();
					// If there are mipmaps, write them along with the main texture (mip 0)
					if (dds.HasMipmaps)
					{
						int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
						MipmapImages = new Bitmap[mipLevels];
						for (int mipmapIndex = 0; mipmapIndex < mipLevels; mipmapIndex++)
						{
							int mipOffset = 0;
							int size = Width;
							for (int i = 0; i < mipmapIndex; i++, size >>= 1)
							{
								mipOffset += inputCodec.CalculateTextureSize(size, size);
							}
							byte[] convData = DxtConverter.ConvertDxt(dds.HeaderlessData[mipOffset..], size, size, false);
							byte[] mipData = new byte[Math.Max(gvrDxtCodec.CalculateTextureSize(size, size), convData.Length)];
							Array.Copy(convData, 0, mipData, 0, convData.Length);
							outputStream.Write(mipData);
						}
						HasMipmaps = true;
						GvrDataFlags |= GvrDataFlags.Mipmaps;
					}
					// Otherwise find and write the main texture
					else
					{
						byte[] ddsdxt = new byte[inputCodec.CalculateTextureSize(Width, Height)];
						Array.Copy(dds.HeaderlessData, 0, ddsdxt, 0, ddsdxt.Length);
						outputStream.Write(DxtConverter.ConvertDxt(ddsdxt, Width, Height, false));
					}
				}
				else
				{
					DdsDataCodec inputCodec = DdsDataCodec.GetDataCodec(dds.DdsFormat, new Bypass16BitPixelCodec(), true);
					GvrDataCodec outputCodec = new GvrBypass16BitDataCodec();
					// If there are mipmaps, write them along with the main texture (mip 0)
					if (dds.HasMipmaps)
					{
						int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
						MipmapImages = new Bitmap[mipLevels];
						for (int mipmapIndex = 0; mipmapIndex < mipLevels; mipmapIndex++)
						{
							int mipOffset = 0;
							int size = Width;
							for (int i = 0; i < mipmapIndex; i++, size >>= 1)
							{
								mipOffset += inputCodec.CalculateTextureSize(size, size);
							}
							byte[] convData = inputCodec.Decode(dds.HeaderlessData[mipOffset..], size, size, null);
							byte[] mipData = new byte[Math.Max(outputCodec.CalculateTextureSize(size, size), convData.Length)];
							outputStream.Write(outputCodec.Encode(mipData, size, size));
						}
						HasMipmaps = true;
						GvrDataFlags |= GvrDataFlags.Mipmaps;
					}
					// Otherwise find and write the main texture
					else
					{
						byte[] maintex = inputCodec.Decode(dds.HeaderlessData[0..], Width, Height, null);
						outputStream.Write(outputCodec.Encode(maintex, Width, Height));
					}
				}
				// Update raw data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Add mipmaps if necessary
				if (!dds.HasMipmaps && forceMipmaps)
				{
					HasMipmaps = false;
					AddMipmaps();
				}
			}
		}
	}
}