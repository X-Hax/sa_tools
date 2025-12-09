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

		public DdsTexture(PvrTexture pvr)
		{
			// Set common texture properties
			Image = pvr.Image;
			Gbix = pvr.Gbix;
			Name = pvr.Name;
			Width = pvr.Width;
			Height = pvr.Height;
			HasMipmaps = pvr.HasMipmaps;
			PaletteBank = pvr.PaletteBank;
			PaletteStartIndex = pvr.PaletteStartIndex;
			PakMetadata = pvr.PakMetadata;
			switch (pvr.PvrPixelFormat)
			{
				case PvrPixelFormat.Argb8888:
				case PvrPixelFormat.Argb8888orYUV420:
					DdsFormat = DdsFormat.Argb8888;
					break;
				case PvrPixelFormat.Rgb565:
					DdsFormat = DdsFormat.Rgb565;
					break;
				case PvrPixelFormat.Argb1555:
				case PvrPixelFormat.Rgb555:
					DdsFormat = DdsFormat.Argb1555;
					break;
				case PvrPixelFormat.Argb4444:
					DdsFormat = DdsFormat.Argb4444;
					break;
				default:
					DdsFormat = DdsFormat.Argb8888;
					break;
			}
			// Check lossless
			bool lossless = false;
			foreach (var item in CompatibleFormatsPvrDds)
				if (item.Key == pvr.PvrPixelFormat && item.Value == DdsFormat)
					lossless = true;
			if (lossless)
			{
				Console.WriteLine("Using lossless conversion");
				PvrDataCodec inputCodec = PvrDataCodec.Create(pvr.PvrDataFormat, new Bypass16BitPixelCodec());
				DdsDataCodec outputCodec = DdsDataCodec.GetDataCodec(DdsFormat, null, true);
				MemoryStream outputStream = new();
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