using System;

namespace TextureLib
{
	public class Intensity8PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 1;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			byte value = src[0];

			dst[0] = value;
			dst[1] = value;
			dst[2] = value;
			dst[3] = 0xFF;
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			dst[0] = TextureFunctions.GetLuminance(src);
		}

		public override string Info()
		{
			return "Intensity8, 8 bit (1 byte per pixel)";
		}
	}
}