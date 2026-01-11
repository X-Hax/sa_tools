using System;

namespace TextureLib
{
	internal class VqDataCodec : PvrDataCodec
	{
		public VqDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int GetPaletteEntries(int? width)
		{
			return 1024; // 256 blocks
		}

		public override int CalculateTextureSize(int width, int height)
		{
			return Math.Max(width * height / 4, 1);
		}

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			TwiddleMap twiddleMap = new(width);

			for(int y = 0; y < height; y += 2)
			{
				for(int x = 0; x < width; x += 2)
				{
					int sourceIndex = twiddleMap[x / 2, y / 2];
					int paletteIndex = source[sourceIndex] * 4;

					for(int x2 = 0; x2 < 2; x2++)
					{
						for(int y2 = 0; y2 < 2; y2++)
						{
							int destinationIndex = (((y + y2) * width) + x + x2) * 4;
							palette.Slice(paletteIndex * 4, 4).CopyTo(destination[destinationIndex..]);
							paletteIndex++;
						}
					}
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			TwiddleMap twiddleMap = new(width);

			width /= 2;
			height /= 2;

			int sourceIndex = 0;
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