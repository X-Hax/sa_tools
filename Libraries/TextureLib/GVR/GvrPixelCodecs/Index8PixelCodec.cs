using System;

namespace TextureLib
{
	internal class GvrIndex8PixelCodec : UncompressedPixelCodec
	{
		public override int PaletteEntries => 256;
		protected override ByteType Type => ByteType.Pixel;

		protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = src[0];
		}

		protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = src[0];
		}
	}
}
