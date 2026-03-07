using System;

namespace TextureLib
{
	internal class Index4MipmapsDataCodec : PvrDataCodec
	{
		public override bool NeedsExternalPalette => true;
		public override bool HasMipmaps => true;

		internal Index4MipmapsDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int GetPaletteEntries(int? width)
		{
			return 16;
		}

		public override int CalculateTextureSize(int width, int height)
		{
			// A 1x1 mipmap takes up as much space as a 2x2 mipmap. The pixel is stored in the upper 4 bits of the final byte.
			if(width == 1)
			{
				width = 2;
			}

			return width * width / 2;
		}

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			TwiddleMap twiddleMap = new(width);
			int destinationIndex = 0;

			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					int sourceIndex = twiddleMap[x, y] / 2;

					byte index = (byte)((source[sourceIndex] >> ((y & 0x1) * 4)) & 0xF);
					destination[destinationIndex] = (byte)(index | (index << 4));

                    destinationIndex++;
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			if(width == 1 && height == 1)
			{
				destination[1] = (byte)((source[0] & 0xF0) >> 4);
			}
			else
			{
				TwiddleMap twiddleMap = new(width);
				int sourceIndex = 0;

				for(int y = 0; y < height; y++)
				{
					for(int x = 0; x < width; x++)
					{
						int destinationIndex = twiddleMap[x, y] / 2;

						destination[destinationIndex] |= (byte)((source[sourceIndex] << 4) >> ((~y & 0x1) * 4));

						sourceIndex++;
					}
				}
			}
		}

	}
}