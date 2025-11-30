using System;

namespace TextureLib
{
	internal abstract class GvrUncompressedDataCodec : GvrDataCodec
	{
		protected enum ByteType
		{
			/// <summary>
			/// One byte contains two pixels
			/// </summary>
			TwoPixels,

			/// <summary>
			/// One pixel, one byte
			/// </summary>
			Pixel,

			/// <summary>
			/// A pixel consists of two bytes
			/// </summary>
			HalfPixel,

			/// <summary>
			/// A Pixel consists of 4 bytes 
			/// </summary>
			QuarterPixel,
		}

		protected abstract ByteType Type { get; }

		protected override int InternalCalculateTextureSize(int width, int height)
		{
			int size = width * height;
			switch(Type)
			{
				case ByteType.TwoPixels:
					size /= 2;
					break;
				case ByteType.HalfPixel:
					size *= 2;
					break;
				case ByteType.QuarterPixel:
					size *= 4;
					size = Math.Max(size, 64);
					break;
				case ByteType.Pixel:
					break;
				default:
					break;
			}

			return size;
		}

		protected override void InternalDecode(ReadOnlySpan<byte> src, int width, int height, Span<byte> dst)
		{
			int sourceIndex = 0;
			int blockHeight = Math.Min(height, Type == ByteType.TwoPixels ? 8 : 4);
			int blockWidth = Math.Min(width, Type >= ByteType.HalfPixel ? 4 : 8);
			int xStep = Type == ByteType.TwoPixels ? 2 : 1;
			int srcStep = Type >= ByteType.HalfPixel ? 2 : 1;
			int dstStep = PaletteEntries > 0 ? 1 : 4;

			for(int yBlock = 0; yBlock < height; yBlock += blockHeight)
			{
				for(int xBlock = 0; xBlock < width; xBlock += blockWidth)
				{
					for(int y = 0; y < blockHeight; y++)
					{
						for(int x = 0; x < blockWidth; x += xStep)
						{
							int destinationIndex = (((yBlock + y) * width) + xBlock + x) * dstStep;

							DecodePixel(src[sourceIndex..], dst[destinationIndex..]);
							sourceIndex += srcStep;
						}
					}

					if(Type == ByteType.QuarterPixel)
					{
						sourceIndex += 32;
					}
				}
			}
		}

		protected abstract void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst);

		protected override void InternalEncode(ReadOnlySpan<byte> src, int width, int height, Span<byte> dst)
		{
			int destinationIndex = 0;
			int blockHeight = Math.Min(height, Type == ByteType.TwoPixels ? 8 : 4);
			int blockWidth = Math.Min(width, Type >= ByteType.HalfPixel ? 4 : 8);
			int xStep = Type == ByteType.TwoPixels ? 2 : 1;
			int dstStep = Type >= ByteType.HalfPixel ? 2 : 1;
			int srcStep = PaletteEntries > 0 ? 1 : 4;

			for(int yBlock = 0; yBlock < height; yBlock += blockHeight)
			{
				for(int xBlock = 0; xBlock < width; xBlock += blockWidth)
				{
					for(int y = 0; y < blockHeight; y++)
					{
						for(int x = 0; x < blockWidth; x += xStep)
						{
							int sourceIndex = (((yBlock + y) * width) + xBlock + x) * srcStep;

							EncodePixel(src[sourceIndex..], dst[destinationIndex..]);
							destinationIndex += dstStep;
						}
					}

					if(Type == ByteType.QuarterPixel)
					{
						destinationIndex += 32;
					}
				}
			}
		}

		protected abstract void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst);
	}
}
