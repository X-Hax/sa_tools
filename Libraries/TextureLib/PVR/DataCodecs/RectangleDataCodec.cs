using System;

namespace TextureLib
{
	internal class RectangleDataCodec : PvrDataCodec
    {
		public override int CalculateTextureSize(int width, int height)
		{
			return width / PixelCodec.Pixels * height * PixelCodec.BytesPerPixel;
		}

		public override bool CheckDimensionsValid(int width, int height)
		{
			return width is >= 8 and <= 1024
				&& height is >= 8 and <= 1024
				&& TextureFunctions.IsPow2(width)
				&& TextureFunctions.IsPow2(height);
		}

		public RectangleDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			int srcAddress = 0;
			int dstAddress = 0;

			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x += PixelCodec.Pixels)
				{
					PixelCodec.DecodePixel(
						source[srcAddress..],
						destination[dstAddress..], false);

					srcAddress += PixelCodec.BytesPerPixel;
					dstAddress += 4 * PixelCodec.Pixels;
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			int srcAddress = 0;
			int dstAddress = 0;

			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x += PixelCodec.Pixels)
				{
					PixelCodec.EncodePixel(
						source[srcAddress..],
						destination[dstAddress..], false);

					srcAddress += 4 * PixelCodec.Pixels;
					dstAddress += PixelCodec.BytesPerPixel;
				}
			}
		}
	}
}