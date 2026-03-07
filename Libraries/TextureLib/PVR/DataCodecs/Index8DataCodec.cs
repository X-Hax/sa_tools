using System;

namespace TextureLib
{
	internal class Index8DataCodec : RectangleDataCodec
	{
		public override bool NeedsExternalPalette => true;

		public Index8DataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int GetPaletteEntries(int? width)
		{
			return 256;
		}

		public override int CalculateTextureSize(int width, int height)
		{
			return width * height;
		}

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			int size = Math.Min(width, height);
			TwiddleMap twiddleMap = new(width);
			int sourceBlockIndex = 0;

			for(int yStart = 0; yStart < height; yStart += size)
			{
				for(int xStart = 0; xStart < width; xStart += size)
				{
					for(int y = 0; y < size; y++)
					{
						for(int x = 0; x < size; x++)
						{
							int sourceIndex = sourceBlockIndex + twiddleMap[x, y];
							int destinationIndex = ((yStart + y) * width) + xStart + x;

							destination[destinationIndex] = source[sourceIndex];
						}
					}

					sourceBlockIndex += size * size;
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			int size = Math.Min(width, height);
			TwiddleMap twiddleMap = new(width);
			int destinationBlockIndex = 0;

			for(int yStart = 0; yStart < height; yStart += size)
			{
				for(int xStart = 0; xStart < width; xStart += size)
				{
					for(int y = 0; y < size; y++)
					{
						for(int x = 0; x < size; x++)
						{
							int sourceIndex = ((y + yStart) * width) + xStart + x;
							int destinationIndex = destinationBlockIndex + twiddleMap[x, y];

							destination[destinationIndex] = source[sourceIndex];
						}
					}

					destinationBlockIndex += size * size;
				}
			}
		}

	}
}