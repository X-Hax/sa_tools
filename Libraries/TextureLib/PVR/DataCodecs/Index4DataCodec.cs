using System;

namespace TextureLib
{
	internal class Index4DataCodec : RectangleDataCodec
	{
		public override bool NeedsExternalPalette => true;

		internal Index4DataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int GetPaletteEntries(int? width)
		{
			return 16;
		}

		public override int CalculateTextureSize(int width, int height)
		{
			return width * width / 2;
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
							int sourceIndex = sourceBlockIndex + (twiddleMap[x, y] / 2);
							int destinationIndex = ((yStart + y) * width) + xStart + x;

							byte index = (byte)((source[sourceIndex] >> ((y & 0x1) * 4)) & 0xF);
							destination[destinationIndex] = (byte)(index | (index << 4));
                        }
					}

					sourceBlockIndex += size * size / 2;
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
							int sourceIndex = (y * width) + x;
							int destinationIndex = destinationBlockIndex + (twiddleMap[x, y] / 2);
							
							destination[destinationIndex] |= (byte)((source[sourceIndex] << 4) >> ((~y & 0x1) * 4));
						}
					}

					destinationBlockIndex += size * size / 2;
				}
			}
		}

	}
}