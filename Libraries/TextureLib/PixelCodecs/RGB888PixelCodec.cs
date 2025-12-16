using System;

// DDS variation

namespace TextureLib
{
	internal class RGB888PixelCodec : PixelCodec
	{
		public override int BytesPerPixel => 3;

		public override int Pixels => 1;

		public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{			
			dst[0] = src[BigEndian ? 0 : 2]; // R
			dst[1] = src[BigEndian ? 1 : 1]; // G
			dst[2] = src[BigEndian ? 2 : 0]; // B
			dst[3] = 0xFF; // A
		}

		public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[BigEndian ? 0 : 2] = src[0]; // R
			dst[BigEndian ? 1 : 1] = src[1]; // G
			dst[BigEndian ? 2 : 0] = src[2]; // B
		}

		public override string Info()
		{
			return "RGB888, 24 bit (3 bytes per pixel)" + (BigEndian ? ", Big Endian" : "");
		}
	}
}