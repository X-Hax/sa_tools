using System;

namespace TextureLib
{
	internal class ARGB8888PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 4;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
			dst[0] = src[bigEndian ? 3 : 0]; // A
			dst[1] = src[bigEndian ? 2 : 1]; // R
			dst[2] = src[bigEndian ? 1 : 2]; // G
			dst[3] = src[bigEndian ? 0 : 3]; // B
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
			dst[bigEndian ? 3 : 0] = src[0];
			dst[bigEndian ? 2 : 1] = src[1];
			dst[bigEndian ? 1 : 2] = src[2];
			dst[bigEndian ? 0 : 3] = src[3];
		}
	}
}