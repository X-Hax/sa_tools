using System;

namespace TextureLib
{
	internal class GvrRGB5A3DataCodec : GvrUncompressedDataCodec
	{
		protected override ByteType Type => ByteType.HalfPixel;

		protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			RGB5A3ToRGBA8(src, dst);
		}

		public static void RGB5A3ToRGBA8(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			if((src[0] & 0x80) != 0) // RGB555
			{
				byte r = (byte)((src[0] & 0x7C) << 1);
				dst[0] = (byte)(r | (r >> 5));

				byte g = (byte)((src[0] << 6) | ((src[1] & 0xE0) >> 2));
				dst[1] = (byte)(g | (g >> 5));

				byte b = (byte)(src[1] << 3);
				dst[2] = (byte)(b | (b >> 5));

				dst[3] = 0xFF;
			}
			else // ARGB3444
			{
				byte r = (byte)(src[0] & 0x0F);
				dst[0] = (byte)(r | (r << 4));

				byte g = (byte)(src[1] & 0xF0);
				dst[1] = (byte)(g | (g >> 4));

				byte b = (byte)(src[1] & 0x0F);
				dst[2] = (byte)(b | (b << 4));

				byte a = (byte)((src[0] & 0x70) << 1);
				dst[3] = (byte)(a | (a >> 3) | (a >> 6));
			}
		}

		protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			RGBA8ToRGB5A3(src, dst);
		}

		public static void RGBA8ToRGB5A3(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			if(src[3] >= 0xE0) // RGB555
			{
				dst[0] = (byte)(0x80 | ((src[0] & 0xF8) >> 1) | (src[1] >> 6));
				dst[1] = (byte)(((src[1] & 0xF8) << 2) | ((src[2] & 0xF8) >> 3));
			}
			else // ARGB3444
			{
				dst[0] = (byte)(((src[3] & 0xE0) >> 1) | (src[0] >> 4));
				dst[1] = (byte)((src[1] & 0xF0) | (src[2] >> 4));
			}
		}
	}
}
