using System;

namespace TextureLib
{
	internal class Bump88PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			const float n = 1f / byte.MaxValue; // normalizer
			const float azimuthCorrect = MathF.PI * 2f;
			const float elevationCorrect = MathF.PI * 0.5f;

			// converting to a normal map. hard to make use of it in any other way
			float azimuth = azimuthCorrect * ((src[BigEndian ? 1 : 0] * n) - 0.5f);
			float elevation = elevationCorrect * (src[BigEndian ? 0 : 1] * n);

			float cos = MathF.Cos(elevation);
			float x = 1 - ((MathF.Sin(azimuth) * cos * 0.5f) + 0.5f);
			float y = (MathF.Cos(azimuth) * cos * 0.5f) + 0.5f;
			float z = MathF.Sin(elevation);

			dst[0] = (byte)(x * byte.MaxValue);
			dst[1] = (byte)(y * byte.MaxValue);
			dst[2] = (byte)(z * byte.MaxValue);
			dst[3] = 0xFF;
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			const float n = 1f / byte.MaxValue; // normalizer
			const float azimuthCorrect = 1f / (MathF.PI * 2f);
			const float elevationCorrect = 1f / (MathF.PI * 0.5f);

			float x = ((1f - (src[0] * n)) * 2f) - 1f;
			float y = (src[1] * n * 2f) - 1f;
			float z = src[2] * n;

			float azimuth;
			float elevation;

			if(z > 0.99f || (x < 0.01f && y < 0.01f && z > 0))
			{
				azimuth = 0.5f;
				elevation = elevationCorrect;
			}
			else
			{
				azimuth = (MathF.Atan2(x, y) * azimuthCorrect) + 0.5f;
				elevation = MathF.Asin(z) * elevationCorrect;
			}

			dst[BigEndian ? 1 : 0] = (byte)(azimuth * byte.MaxValue);
			dst[BigEndian ? 0 : 1] = (byte)(elevation * byte.MaxValue);
		}

		public override string Info()
		{
			return "BUMP88, 16 bit (2 bytes per pixel)" + (BigEndian ? ", Big Endian" : "");
		}
	}
}