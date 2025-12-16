using System;

namespace TextureLib
{
	internal class Intensity4PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 1;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			byte index = src[0];
			byte first = (byte)((index & 0xF0) | (index >> 4));
			byte second = (byte)((index & 0x0F) | (index << 4));

			dst[0] = first;
			dst[1] = first;
			dst[2] = first;
			dst[3] = 0xFF;

			dst[4] = second;
			dst[5] = second;
			dst[6] = second;
			dst[7] = 0xFF;
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			byte first = TextureFunctions.GetLuminance(src);
			byte second = TextureFunctions.GetLuminance(src[4..]);

			dst[0] = (byte)((first & 0xF0) | (second >> 4));
		}

		public override string Info()
		{
			return "Intensity4, 4 bit (2 pixels per byte)";
		}
	}
}