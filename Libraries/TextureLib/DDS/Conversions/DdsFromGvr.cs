using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TextureLib
{
	public partial class DdsTexture
	{
		/// <summary>Dictionary of GVR and DDS formats that can be converted losslessly between each other. ARGB8888 and Indexed formats not included.</summary>
		public static Dictionary<GvrDataFormat, DdsFormat> CompatibleFormatsGvrDds = new Dictionary<GvrDataFormat, DdsFormat>()
		{
			{ GvrDataFormat.Rgb565, DdsFormat.Rgb565 },
			{ GvrDataFormat.Dxt1, DdsFormat.Dxt1 }
			// IntensityA4 could be converted to D3DFMT_A4L4
			// Intensity8 could be converted to D3DFMT_L8 or D3DFMT_A8
			// IntensityA8 could be converted to D3DFMT_A8L8	
		};

		/// <summary>
		/// Create a new DDS texture from a GVR texture. DDS data format is determined automatically.
		/// This conversion is lossless for RGB565 or DXT1 GVR textures.
		/// </summary>
		/// <param name="gvr">Source GVR texture.</param>
		/// <param name="forceMipmaps">Whether the target texture should have mipmaps even if the source texture doesn't.</param>
		/// <param name="maxQuality">Whether to prefer ARGB8888 format if a lossless conversion cannot be done with other formats.</param>
		public DdsTexture(GvrTexture gvr, bool forceMipmaps = false, bool maxQuality = false)
		{
			DdsFormat targetFormat;
			switch (gvr.GvrDataFormat)
			{
				case GvrDataFormat.Rgb565:
					targetFormat = DdsFormat.Rgb565;
					break;
				case GvrDataFormat.Dxt1:
					targetFormat = DdsFormat.Dxt1;
					break;
				case GvrDataFormat.Rgb5a3:
					targetFormat = maxQuality ? DdsFormat.Argb8888 : DdsFormat.Dxt5;
					break;
				case GvrDataFormat.Argb8888:
				default:
					targetFormat = DdsFormat.Argb8888;
					break;
			}
			ConvertFromGvr(gvr, targetFormat, forceMipmaps);
		}

		/// <summary>Create a new DDS texture from a GVR texture, data format is specified manually.
		/// This conversion is lossless for RGB565 or DXT1 GVR textures if the DDS format matches.</summary>
		/// <param name="gvr">Source GVR texture.</param>
		/// <param name="targetFormat">Target DDS format.</param>
		/// <param name="forceMipmaps">Whether the target texture should have mipmaps even if the source texture doesn't.</param>
		public DdsTexture(GvrTexture gvr, DdsFormat targetFormat, bool forceMipmaps = false)
		{
			ConvertFromGvr(gvr, targetFormat, forceMipmaps);
		}

		private void ConvertFromGvr(GvrTexture gvr, DdsFormat targetDddsFormat, bool forceMipmaps = false)
		{
			// Set common texture properties
			Image = gvr.Image;
			Gbix = gvr.Gbix;
			Name = gvr.Name;
			Width = gvr.Width;
			Height = gvr.Height;
			HasMipmaps = gvr.HasMipmaps;
			PaletteBank = gvr.PaletteBank;
			PaletteStartIndex = gvr.PaletteStartIndex;
			PakMetadata = gvr.PakMetadata;
			DdsFormat = targetDddsFormat;
			// Transfer mipmap images if they exist (to re-encode them directly rather than using the full size image)
			if (gvr.MipmapImages != null && HasMipmaps)
			{
				MipmapImages = new System.Drawing.Bitmap[gvr.MipmapImages.Length];
				for (int i = 0; i < gvr.MipmapImages.Length; i++)
				{
					MipmapImages[i] = new System.Drawing.Bitmap(gvr.MipmapImages[i]);
				}
			}
			// Check lossless
			bool lossless = false;
			foreach (var item in CompatibleFormatsGvrDds)
				if (item.Key == gvr.GvrDataFormat && item.Value == DdsFormat)
					lossless = true;
			// If the encoding is not lossless, perform a full conversion
			if (!lossless)
			{
				HasMipmaps = forceMipmaps ? true : gvr.HasMipmaps;
				Encode();
			}
			// Otherwise perform lossless conversion
			else
			{
#if DEBUG
				Console.WriteLine("Using lossless conversion");
#endif
				GvrDataCodec inputCodec = GvrDataCodec.GetDataCodec(gvr.GvrDataFormat);
				DdsDataCodec outputCodec = DdsDataCodec.GetDataCodec(DdsFormat, new Bypass16BitPixelCodec(), true);
				MemoryStream outputStream = new();
				// If it's DXT1, use the converter
				if (gvr.GvrDataFormat == GvrDataFormat.Dxt1)
				{
					// If there are mipmaps, write them along with the main texture (mip 0)
					if (HasMipmaps)
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
							byte[] convData = DxtConverter.ConvertDxt(gvr.HeaderlessData[mipOffset..], size, size, true);
							byte[] mipData = new byte[Math.Max(outputCodec.CalculateTextureSize(size, size), convData.Length)];
							Array.Copy(convData, 0, mipData, 0, convData.Length);
							outputStream.Write(mipData);
						}
					}
					// Otherwise find and write the main texture
					else
					{
						byte[] ddsdxt = new byte[inputCodec.CalculateTextureSize(Width, Height)];
						Array.Copy(gvr.HeaderlessData, 0, ddsdxt, 0, ddsdxt.Length);
						outputStream.Write(DxtConverter.ConvertDxt(ddsdxt, Width, Height, true));
					}
				}
				// If not, use the bypass codec
				else
				{
					inputCodec = new GvrBypass16BitDataCodec();
					int currentOffset = 0;
					// Account for CLUT
					if (gvr.Indexed && !gvr.RequiresPaletteFile)
					{
						PixelCodec paletteCodec = PixelCodec.GetPixelCodec(gvr.GvrPaletteFormat);
						int paletteSize = paletteCodec.BytesPerPixel * GvrDataCodec.GetDataCodec(gvr.GvrDataFormat).PaletteEntries;
						currentOffset += paletteSize;
					}
					// Mipmaps + main texture
					if (HasMipmaps)
					{
						int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
						MipmapImages = new Bitmap[mipLevels];
						for (int mipmapIndex = 0; mipmapIndex < mipLevels; mipmapIndex++)
						{
							int mipOffset = currentOffset;
							int size = Width;
							for (int i = 0; i < mipmapIndex; i++, size >>= 1)
							{
								mipOffset += inputCodec.CalculateTextureSize(size, size);
							}
							byte[] convData = inputCodec.Decode(gvr.HeaderlessData[mipOffset..], size, size, null);
							byte[] mipData = new byte[Math.Max(outputCodec.CalculateTextureSize(size, size), convData.Length)];
							Array.Copy(convData, 0, mipData, 0, convData.Length);
							outputStream.Write(mipData);
						}
					}
					// Just main texture
					byte[] maintex = inputCodec.Decode(gvr.HeaderlessData[currentOffset..], Width, Height, null);
					outputStream.Write(outputCodec.Encode(maintex, Width, Height));
				}
				// Update raw data arrays
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				// Check if mipmaps need to be force created
				if (forceMipmaps && !gvr.HasMipmaps)
				{
					HasMipmaps = false;
					AddMipmaps();
				}
			}
		}
	}
}