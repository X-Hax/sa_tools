using System;

namespace TextureLib
{
	// Endian flipped!
	internal class GvrBypass16BitDataCodec : GvrUncompressedDataCodec
	{
		protected override ByteType Type => ByteType.HalfPixel;

		protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = ByteConverter.BigEndian ? src[0] : src[1];
			dst[1] = ByteConverter.BigEndian ? src[1] : src[0];
		}

		protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
			dst[0] = ByteConverter.BigEndian ? src[0] : src[1];
			dst[1] = ByteConverter.BigEndian ? src[1] : src[0];
		}
	}
}