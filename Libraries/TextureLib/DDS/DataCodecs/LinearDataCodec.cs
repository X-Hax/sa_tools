using System;

namespace TextureLib
{
	// Basically the same as PVR Rectangular
	internal class LinearDataCodec : DdsDataCodec
	{
		public override int CalculateTextureSize(int width, int height)
		{
			return width / PixelCodec.Pixels * height * PixelCodec.BytesPerPixel;
		}

		public override bool CheckDimensionsValid(int width, int height)
		{
			return width is >= 4 and <= 8192
				&& height is >= 4 and <= 8192
				&& TextureFunctions.IsPow2(width)
				&& TextureFunctions.IsPow2(height);
		}

		public LinearDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			int srcAddress = 0;
			int dstAddress = 0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x += PixelCodec.Pixels)
				{
					PixelCodec.DecodePixel(
						source[srcAddress..],
						destination[dstAddress..]);

					srcAddress += PixelCodec.BytesPerPixel;
					dstAddress += 4 * PixelCodec.Pixels;
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			int srcAddress = 0;
			int dstAddress = 0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x += PixelCodec.Pixels)
				{
					PixelCodec.EncodePixel(
						source[srcAddress..],
						destination[dstAddress..]);

					srcAddress += 4 * PixelCodec.Pixels;
					dstAddress += PixelCodec.BytesPerPixel;
				}
			}
		}
	}
}