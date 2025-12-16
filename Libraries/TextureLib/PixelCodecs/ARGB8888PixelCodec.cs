using System;

namespace TextureLib
{
	internal class ARGB8888PixelCodec : PixelCodec
	{
		public enum ColorOrderType
		{
			/// <summary>Color order used in PVR palettes and DDS textures.</summary>
			ARGBNormal,
			/// <summary>Color order used in PVR Bitmap data format.</summary>
			ABGRBitmap
		}

		public ColorOrderType ColorOrder;

		public override int BytesPerPixel => 4;

		public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			switch (ColorOrder)
			{
				case ColorOrderType.ABGRBitmap:
					dst[0] = src[BigEndian ? 0 : 3];
					dst[1] = src[BigEndian ? 1 : 2];
					dst[2] = src[BigEndian ? 2 : 1];
					dst[3] = src[BigEndian ? 3 : 0];
					break;
				case ColorOrderType.ARGBNormal:
				default:
					dst[0] = src[BigEndian ? 3 : 2];
					dst[1] = src[BigEndian ? 0 : 1];
					dst[2] = src[BigEndian ? 1 : 0];
					dst[3] = src[BigEndian ? 2 : 3];
					break;
			}
		}

		public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			switch (ColorOrder)
			{
				case ColorOrderType.ABGRBitmap:
					dst[BigEndian ? 0 : 3] = src[0];
					dst[BigEndian ? 1 : 2] = src[1];
					dst[BigEndian ? 2 : 1] = src[2];
					dst[BigEndian ? 3 : 0] = src[3];
					break;
				case ColorOrderType.ARGBNormal:
				default:
					dst[BigEndian ? 3 : 2] = src[2];
					dst[BigEndian ? 0 : 1] = src[1];
					dst[BigEndian ? 1 : 0] = src[0];
					dst[BigEndian ? 2 : 3] = src[3];
					break;
			}
		}

		public override string Info()
		{
			return "ARGB8888, 32 bit (4 bytes per pixel), color order: " + (ColorOrder == ColorOrderType.ARGBNormal ? "ARGB" : "ABGR (Bitmap)") + (BigEndian ? ", Big Endian" : "");
		}
	}
}