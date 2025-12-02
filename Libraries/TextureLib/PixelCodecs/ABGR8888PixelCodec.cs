using System;

// DDS variation

namespace TextureLib
{
	internal class ABGR8888PixelCodec : PixelCodec
	{
		public override int BytesPerPixel => 4;

		public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
		{
			dst[0] = src[bigEndian ? 3 : 2]; // R
			dst[1] = src[bigEndian ? 0 : 1]; // G
			dst[2] = src[bigEndian ? 1 : 0]; // B
			dst[3] = src[bigEndian ? 2 : 3]; // A
		}

		public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
		{
			dst[bigEndian ? 3 : 2] = src[2]; // R
			dst[bigEndian ? 0 : 1] = src[1]; // G
			dst[bigEndian ? 1 : 0] = src[0]; // B
			dst[bigEndian ? 2 : 3] = src[3]; // A
		}
	}
}