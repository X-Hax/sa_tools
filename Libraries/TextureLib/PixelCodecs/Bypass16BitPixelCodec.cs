using System;

namespace TextureLib
{
	internal class Bypass16BitPixelCodec : PixelCodec
	{
		public override int BytesPerPixel => 2;

		public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = BigEndian ? src[1] : src[0];
			dst[1] = BigEndian ? src[0] : src[1];
		}

		public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = BigEndian ? src[1] : src[0];
			dst[1] = BigEndian ? src[0] : src[1];
		}

		public override string Info()
		{
			return "Bypass 16 bit (2 bytes per pixel)" + (BigEndian ? ", Big Endian" : "");
		}
	}
}