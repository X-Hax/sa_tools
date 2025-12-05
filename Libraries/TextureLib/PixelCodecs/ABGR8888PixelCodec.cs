using System;

// DDS variation

namespace TextureLib
{
	internal class ABGR8888PixelCodec : PixelCodec
	{
		public override int BytesPerPixel => 4;

		public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = src[BigEndian ? 3 : 2]; // R
			dst[1] = src[BigEndian ? 0 : 1]; // G
			dst[2] = src[BigEndian ? 1 : 0]; // B
			dst[3] = src[BigEndian ? 2 : 3]; // A
		}

		public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[BigEndian ? 3 : 2] = src[2]; // R
			dst[BigEndian ? 0 : 1] = src[1]; // G
			dst[BigEndian ? 1 : 0] = src[0]; // B
			dst[BigEndian ? 2 : 3] = src[3]; // A
		}
	}
}