using System;

namespace TextureLib
{
	internal class ARGB8888PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 4;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			dst[0] = src[BigEndian ? 3 : 0]; // A
			dst[1] = src[BigEndian ? 2 : 1]; // R
			dst[2] = src[BigEndian ? 1 : 2]; // G
			dst[3] = src[BigEndian ? 0 : 3]; // B
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			dst[BigEndian ? 3 : 0] = src[0];
			dst[BigEndian ? 2 : 1] = src[1];
			dst[BigEndian ? 1 : 2] = src[2];
			dst[BigEndian ? 0 : 3] = src[3];
		}
	}
}