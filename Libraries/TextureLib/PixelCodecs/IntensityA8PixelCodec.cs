using System;

namespace TextureLib
{
	public class IntensityA8PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			byte intensity = src[1];
			byte alpha = src[0];

			dst[0] = intensity;
			dst[1] = intensity;
			dst[2] = intensity;
			dst[3] = alpha;
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			dst[0] = src[3];
			dst[1] = TextureFunctions.GetLuminance(src);
		}

		public override string Info()
		{
			return "Intensity8A8, 16 bit (2 bytes per pixel)";
		}
	}
}
