using System;

namespace TextureLib
{
	internal class SquareTwiddledMipmapsDataCodec : SquareTwiddledDataCodec
	{
		public override bool HasMipmaps => true;

		public SquareTwiddledMipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int CalculateTextureSize(int width, int height)
		{
			// A 1x1 mipmap takes up as much space as a 2x1 texture (old twiddle method, idk).
			// Probably because YUV encodes for pixel pairs
			if(width == 1)
			{
				width = 2;
			}

			return width / PixelCodec.Pixels * height * PixelCodec.BytesPerPixel;
		}

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			if(width == 1)
			{
				PixelCodec.DecodePixel(
					source[0..],
					destination[0..4]);
			}
			else
			{
				base.InternalDecode(source, width, height, palette, destination);
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			if(width == 1)
			{
				PixelCodec.EncodePixel(
					source[0..],
					destination.Slice(destination.Length - PixelCodec.BytesPerPixel, PixelCodec.BytesPerPixel));
			}
			else
			{
				base.InternalEncode(source, width, height, destination);
			}
		}
	}
}