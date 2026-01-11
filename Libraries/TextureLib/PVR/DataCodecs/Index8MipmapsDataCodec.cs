using System;

namespace TextureLib
{
	internal class Index8MipmapsDataCodec : PvrDataCodec
	{
		public override bool NeedsExternalPalette => true;
		public override bool HasMipmaps => true;

		public Index8MipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int GetPaletteEntries(int? width)
		{
			return 256;
		}

		public override int CalculateTextureSize(int width, int height)
		{
			// A 1x1 mipmap takes up as much space as a 2x2 mipmap.
			// i guess this logic was just kind of copy pasted from the Index4 codec
			if(width == 1)
			{
				width = 2;
			}

			return width * width;
		}

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			TwiddleMap twiddleMap = new(width);
			int destinationIndex = 0;

			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					int sourceIndex = twiddleMap[x, y];

					destination[destinationIndex] = source[sourceIndex];
					destinationIndex++;
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			TwiddleMap twiddleMap = new(width);
			int sourceIndex = 0;

			if(width == 1 && height == 1)
			{
				destination[3] = source[0];
			}
			else
			{
				for(int y = 0; y < height; y++)
				{
					for(int x = 0; x < width; x++)
					{
						int destinationIndex = twiddleMap[x, y];
						destination[destinationIndex] = source[sourceIndex];
						sourceIndex++;
					}
				}

			}

		}

	}
}