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

		public DdsTexture(GvrTexture gvr)
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
			switch (gvr.GvrDataFormat)
			{
				case GvrDataFormat.Argb8888:
					DdsFormat = DdsFormat.Argb8888;
					break;
				case GvrDataFormat.Rgb565:
					DdsFormat = DdsFormat.Rgb565;
					break;
				case GvrDataFormat.Dxt1:
					DdsFormat = DdsFormat.Dxt1;
					break;
				case GvrDataFormat.Rgb5a3:
					DdsFormat = DdsFormat.Dxt5;
					break;
				default:
					DdsFormat = DdsFormat.Argb8888;
					break;
			}
			// Check lossless
			bool lossless = false;
			foreach (var item in CompatibleFormatsGvrDds)
				if (item.Key == gvr.GvrDataFormat && item.Value == DdsFormat)
					lossless = true;
			if (lossless)
			{
				Console.WriteLine("Using lossless conversion");
				GvrDataCodec inputCodec = GvrDataCodec.GetDataCodec(gvr.GvrDataFormat);
				DdsDataCodec outputCodec = DdsDataCodec.GetDataCodec(DdsFormat, null, true);
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
					// Mipmaps + main texture
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
							byte[] convData = inputCodec.Decode(gvr.HeaderlessData[mipOffset..], size, size, null);
							byte[] mipData = new byte[Math.Max(outputCodec.CalculateTextureSize(size, size), convData.Length)];
							Array.Copy(convData, 0, mipData, 0, convData.Length);
							outputStream.Write(mipData);
						}
					}
					// Just main texture

					byte[] ddsdxt = new byte[inputCodec.CalculateTextureSize(Width, Height)];
					Array.Copy(gvr.HeaderlessData, 0, ddsdxt, 0, ddsdxt.Length);
					outputStream.Write(DxtConverter.ConvertDxt(ddsdxt, Width, Height, true));
				}
				HeaderlessData = outputStream.ToArray();
				RawData = GetBytes();
				Decode();
			}
			// Otherwise use full encoding
			else
			{
				Encode();
				Decode();
			}
		}
	}
}