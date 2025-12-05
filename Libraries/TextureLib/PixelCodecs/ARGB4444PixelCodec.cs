using System;

namespace TextureLib
{
	internal class ARGB4444PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            byte r = (byte)(src[BigEndian ? 0 : 1] & 0x0F);
			dst[0] = (byte)(r | (r << 4));

			byte g = (byte)(src[BigEndian ? 1 : 0] & 0xF0);
			dst[1] = (byte)(g | (g >> 4));

			byte b = (byte)(src[BigEndian ? 1 : 0] & 0x0F);
			dst[2] = (byte)(b | (b << 4));

			byte a = (byte)(src[BigEndian ? 0 : 1] & 0xF0);
			dst[3] = (byte)(a | (a >> 4));
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			dst[BigEndian ? 0 : 1] = (byte)((src[3] & 0xF0) | (src[0] >> 4));
			dst[BigEndian ? 1 : 0] = (byte)((src[1] & 0xF0) | (src[2] >> 4));
		}
	}
}
