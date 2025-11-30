using System;

namespace TextureLib
{
	internal class GvrRGB565DataCodec : GvrUncompressedDataCodec
	{
		protected override ByteType Type => ByteType.HalfPixel;

		protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			RGB565ToRGBA8(src, dst);
		}

		public static void RGB565ToRGBA8(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			byte r = (byte)(src[0] & 0xF8);
			dst[0] = (byte)(r | (r >> 5));

			byte g = (byte)((src[0] << 5) | ((src[1] & 0xE0) >> 3));
			dst[1] = (byte)(g | (g >> 6));

			byte b = (byte)(src[1] << 3);
			dst[2] = (byte)(b | (b >> 5));

			dst[3] = 0xFF;
		}

		protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			RGBA8ToRGB565(src, dst);
		}

		public static void RGBA8ToRGB565(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = (byte)((src[0] & 0xF8) | ((src[1] & 0xE0) >> 5));
			dst[1] = (byte)(((src[1] & 0x1C) << 3) | ((src[2] & 0xF8) >> 3));
		}
	}
}
