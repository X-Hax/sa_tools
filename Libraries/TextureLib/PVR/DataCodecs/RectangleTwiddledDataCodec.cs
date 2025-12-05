using System;

namespace TextureLib
{
	internal class RectangleTwiddledDataCodec : RectangleDataCodec
	{
		public override int CalculateTextureSize(int width, int height)
		{
			return width / PixelCodec.Pixels * height * PixelCodec.BytesPerPixel;
		}

		public RectangleTwiddledDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
		{
			int size = Math.Min(width, height);
			TwiddleMap twiddleMap = new(width);
			int sourceBlockIndex = 0;

			if(PixelCodec.Pixels > 1)
			{
				Span<byte> pixelBuffer = new byte[PixelCodec.BytesPerPixel];
				int pixelPart = PixelCodec.BytesPerPixel / PixelCodec.Pixels;

				for(int yStart = 0; yStart < height; yStart += size)
				{
					for(int xStart = 0; xStart < width; xStart += size)
					{
						for(int y = 0; y < size; y++)
						{
							for(int x = 0; x < size; x += PixelCodec.Pixels)
							{
								for(int px = 0; px < PixelCodec.Pixels; px++)
								{
									int srcAddress = sourceBlockIndex + (twiddleMap[x + px, y] * pixelPart);
									source.Slice(srcAddress, pixelPart).CopyTo(pixelBuffer[(px * pixelPart)..]);
								}

								int destinationIndex = (((yStart + y) * width) + xStart + x) * 4;

								PixelCodec.DecodePixel(pixelBuffer, destination[destinationIndex..]);
							}
						}

						sourceBlockIndex += size * size * PixelCodec.BytesPerPixel;
					}
				}
			}
			else
			{
				for(int yStart = 0; yStart < height; yStart += size)
				{
					for(int xStart = 0; xStart < width; xStart += size)
					{
						for(int y = 0; y < size; y++)
						{
							for(int x = 0; x < size; x++)
							{
								int sourceIndex = sourceBlockIndex + (twiddleMap[x, y] * PixelCodec.BytesPerPixel);
								int destinationIndex = (((yStart + y) * width) + xStart + x) * 4;

								PixelCodec.DecodePixel(source[sourceIndex..], destination[destinationIndex..]);
							}
						}

						sourceBlockIndex += size * size * PixelCodec.BytesPerPixel;
					}
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> source, int width, int height, Span<byte> destination)
		{
			int size = Math.Min(width, height);
			TwiddleMap twiddleMap = new(width);
			int destinationBlockIndex = 0;

			if(PixelCodec.Pixels > 1)
			{
				Span<byte> pixelBuffer = new byte[PixelCodec.BytesPerPixel];
				int pixelPart = PixelCodec.BytesPerPixel / PixelCodec.Pixels;

				for(int yStart = 0; yStart < height; yStart += size)
				{
					for(int xStart = 0; xStart < width; xStart += size)
					{
						for(int y = 0; y < size; y++)
						{
							for(int x = 0; x < size; x += PixelCodec.Pixels)
							{
								int sourceIndex = (((y + yStart) * width) + xStart + x) * 4;
								PixelCodec.EncodePixel(source[sourceIndex..], pixelBuffer);

								for(int px = 0; px < PixelCodec.Pixels; px++)
								{
									int dstAddress = destinationBlockIndex + (twiddleMap[x + px, y] * pixelPart);
									pixelBuffer.Slice(px * pixelPart, pixelPart).CopyTo(destination[dstAddress..]);
								}
							}
						}

						destinationBlockIndex += size * size * PixelCodec.BytesPerPixel;
					}
				}
			}
			else
			{
				for(int yStart = 0; yStart < height; yStart += size)
				{
					for(int xStart = 0; xStart < width; xStart += size)
					{
						for(int y = 0; y < size; y++)
						{
							for(int x = 0; x < size; x += PixelCodec.Pixels)
							{
								int sourceIndex = (((y + yStart) * width) + xStart + x) * 4;
								int destinationIndex = destinationBlockIndex + (twiddleMap[x / PixelCodec.Pixels, y] * PixelCodec.BytesPerPixel);

								PixelCodec.EncodePixel(source[sourceIndex..], destination[destinationIndex..]);
							}
						}

						destinationBlockIndex += size * size * PixelCodec.BytesPerPixel;
					}
				}
			}
		}
	}
}