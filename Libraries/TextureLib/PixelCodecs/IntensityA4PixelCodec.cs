using System;

namespace TextureLib
{
	public class IntensityA4PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 1;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			byte value = src[0];

			byte intensity = (byte)((value & 0x0F) | (value << 4));
			byte alpha = (byte)((value & 0xF0) | (value >> 4));

			dst[0] = intensity;
			dst[1] = intensity;
			dst[2] = intensity;
			dst[3] = alpha;
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			byte intensity = TextureFunctions.GetLuminance(src);
			byte alpha = src[3];

			dst[0] = (byte)((intensity >> 4) | (alpha & 0xF0));
		}

		public override string Info()
		{
			return "Intensity4A4, 16 bit (2 bytes per pixel)";
		}
	}
}