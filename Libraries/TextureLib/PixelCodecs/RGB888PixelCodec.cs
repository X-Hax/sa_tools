using System;

// DDS variation

namespace TextureLib
{
	internal class RGB888PixelCodec : PixelCodec
	{
		public override int BytesPerPixel => 3;

		public override int Pixels => 1;

		public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
		{			
			dst[0] = src[bigEndian ? 0 : 2]; // R
			dst[1] = src[bigEndian ? 1 : 1]; // G
			dst[2] = src[bigEndian ? 2 : 0]; // B
			dst[3] = 0xFF; // A
		}

		public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
		{
			dst[bigEndian ? 0 : 2] = src[0]; // R
			dst[bigEndian ? 1 : 1] = src[1]; // G
			dst[bigEndian ? 2 : 0] = src[2]; // B
		}
	}
}