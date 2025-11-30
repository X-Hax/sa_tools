using System;

namespace TextureLib
{
	internal class GvrDXT1DataCodec : GvrDataCodec
	{
		private const int _alphaThreshold = 16;

		protected override int InternalCalculateTextureSize(int width, int height)
		{
			return width * height / 2;
		}

		protected override void InternalDecode(ReadOnlySpan<byte> src, int width, int height, Span<byte> dst)
		{
			byte[] colors = new byte[16];
			Span<byte> colorDst = colors;
			colors[3] = 0xFF;
			colors[7] = 0xFF;
			colors[11] = 0xFF;

			void lerpByte3(int start, int end, int to)
			{
				colors[to] = (byte)(((colors[start] * 2) + colors[end]) / 3);
			}

			void lerpByte2(int start, int end, int to)
			{
				colors[to] = (byte)((colors[start] + colors[end]) / 2);
			}

			int sourceIndex = 0;

			int blockWidth = Math.Min(width, 8);
			int blockHeight = Math.Min(height, 8);
			int tileSize = Math.Min(width, 4);

			for(int yBlock = 0; yBlock < height; yBlock += blockHeight)
			{
				for(int xBlock = 0; xBlock < width; xBlock += blockWidth)
				{
					for(int y = 0; y < blockHeight; y += tileSize)
					{
						for(int x = 0; x < blockWidth; x += tileSize)
						{
							GvrRGB565DataCodec.RGB565ToRGBA8(src[sourceIndex..], colorDst);
                            GvrRGB565DataCodec.RGB565ToRGBA8(src[(sourceIndex + 2)..], colorDst[4..]);

							ushort col1 = (ushort)((src[sourceIndex] << 8) | src[sourceIndex + 1]);
							ushort col2 = (ushort)((src[sourceIndex + 2] << 8) | src[sourceIndex + 3]);

							if(col1 > col2)
							{
								lerpByte3(0, 4, 8);
								lerpByte3(1, 5, 9);
								lerpByte3(2, 6, 10);

								lerpByte3(4, 0, 12);
								lerpByte3(5, 1, 13);
								lerpByte3(6, 2, 14);
								colors[15] = 0xFF;
							}
							else
							{
								lerpByte2(0, 4, 8);
								lerpByte2(1, 5, 9);
								lerpByte2(2, 6, 10);

								colors[12] = 0;
								colors[13] = 0;
								colors[14] = 0;
								colors[15] = 0;
							}

							sourceIndex += 4;

							for(int y2 = 0; y2 < tileSize; y2++)
							{
								for(int x2 = 0; x2 < tileSize; x2++)
								{
									int colorIndex = ((src[sourceIndex + y2] >> (6 - (x2 * 2))) & 0x3) * 4;
									int destinationIndex = (((yBlock + y + y2) * width) + xBlock + x + x2) * 4;
									colorDst.Slice(colorIndex, 4).CopyTo(dst[destinationIndex..]);
								}
							}

							sourceIndex += 4;
						}
					}
				}
			}
		}

		protected override void InternalEncode(ReadOnlySpan<byte> src, int width, int height, Span<byte> dst)
		{
			Span<byte> subBlock = new byte[64];
			int destinationIndex = 0;

			int blockWidth = Math.Min(width, 8);
			int blockHeight = Math.Min(height, 8);
			int tileSize = Math.Min(width, 4);

			for(int yBlock = 0; yBlock < height; yBlock += blockHeight)
			{
				for(int xBlock = 0; xBlock < width; xBlock += blockWidth)
				{
					for(int y = 0; y < blockHeight; y += tileSize)
					{
						for(int x = 0; x < blockWidth; x += tileSize)
						{
							int subBlockStart = 0;

							for(int ySubBlock = 0; ySubBlock < tileSize; ySubBlock++)
							{
								for(int xSubBlock = 0; xSubBlock < tileSize; xSubBlock++)
								{
									int sourceIndex = (((y + yBlock + ySubBlock) * width) + x + xBlock + xSubBlock) * 4;

									src.Slice(sourceIndex, 4).CopyTo(subBlock[subBlockStart..]);
									subBlockStart += 4;
								}
							}

							ConvertBlockToQuaterCmpr(subBlock, dst[destinationIndex..]);
							destinationIndex += 8;
						}
					}
				}
			}
		}

		private static void ConvertBlockToQuaterCmpr(ReadOnlySpan<byte> block, Span<byte> destination)
		{
			byte[] palette = new byte[16];
			Span<byte> paletteDst = palette;

			bool alpha = false;
			int col1Index = -1;
			int col2Index = -1;
			int greatestDistance = -1;
			for(int i = 0; i < 15; i++)
			{
				if(block[(i * 4) + 3] < _alphaThreshold)
				{
					alpha = true;
				}
				else
				{
					for(int j = i + 1; j < 16; j++)
					{
						int distance = Distance(block[(i * 4)..], block[(j * 4)..]);

						if(distance > greatestDistance)
						{
							greatestDistance = distance;
							col1Index = i;
							col2Index = j;
						}
					}
				}
			}

			// using the destination as temporary buffer in the next part
			if(greatestDistance == -1)
			{
				paletteDst[3] = 0xFF;

				// if the last pixel is not transparent, use it, else just write black and white
				if(block[63] < _alphaThreshold)
				{
					paletteDst[4] = 0xFF;
					paletteDst[5] = 0xFF;
					paletteDst[6] = 0xFF;
					paletteDst[7] = 0xFF;
				}
				else
				{
                    GvrRGB565DataCodec.RGBA8ToRGB565(block[60..], destination);
                    GvrRGB565DataCodec.RGB565ToRGBA8(destination, paletteDst[4..]);
				}
			}
			else
			{

                // Compressing the colors
                GvrRGB565DataCodec.RGBA8ToRGB565(block[(col1Index * 4)..], destination);
                GvrRGB565DataCodec.RGB565ToRGBA8(destination, paletteDst);
				ushort col1 = (ushort)((destination[0] << 8) | destination[1]);

                GvrRGB565DataCodec.RGBA8ToRGB565(block[(col2Index * 4)..], destination);
                GvrRGB565DataCodec.RGB565ToRGBA8(destination, paletteDst[4..]);
				ushort col2 = (ushort)((destination[0] << 8) | destination[1]);

				// checking if they ended up equal
				if(col1 == col2)
				{
					// if the first color is black, the second one should be white. else black.
					if(col1 == 0)
					{
						paletteDst[4] = paletteDst[5] = paletteDst[6] = 0xFF;
						col2 = 0xFFFF;
					}
					else
					{
						paletteDst[4] = paletteDst[5] = paletteDst[6] = 0;
						col2 = 0;
					}
				}

				// determine if the colors need to be flipped
				if((col1 > col2) == alpha)
				{
					paletteDst[..4].CopyTo(paletteDst[8..]);
					paletteDst.Slice(4, 4).CopyTo(paletteDst);
					paletteDst.Slice(8, 4).CopyTo(paletteDst[4..]);
				}

				// assemble the lerped colors
				if(alpha)
				{
					void lerpByte2(int start, int end, int to)
					{
						palette[to] = (byte)((palette[start] + palette[end]) / 2);
					}

					lerpByte2(0, 4, 8);
					lerpByte2(1, 5, 9);
					lerpByte2(2, 6, 10);
					palette[11] = 0xFF;

					paletteDst[12] = paletteDst[13] = paletteDst[14] = paletteDst[15] = 0;
				}
				else
				{
					void lerpByte3(int start, int end, int to)
					{
						palette[to] = (byte)(((palette[start] * 2) + palette[end]) / 3);
					}

					lerpByte3(0, 4, 8);
					lerpByte3(1, 5, 9);
					lerpByte3(2, 6, 10);
					palette[11] = 0xFF;

					lerpByte3(4, 0, 12);
					lerpByte3(5, 1, 13);
					lerpByte3(6, 2, 14);
					palette[15] = 0xFF;
				}
			}

            // write the colors to the destination
            GvrRGB565DataCodec.RGBA8ToRGB565(paletteDst, destination);
            GvrRGB565DataCodec.RGBA8ToRGB565(paletteDst[4..], destination[2..]);

			for(int i = 0; i < 4; i++)
			{
				int blockIndex = i * 16;
				int index1 = LeastDistance(palette, block[blockIndex..]);
				int index2 = LeastDistance(palette, block[(blockIndex + 4)..]);
				int index3 = LeastDistance(palette, block[(blockIndex + 8)..]);
				int index4 = LeastDistance(palette, block[(blockIndex + 12)..]);

				destination[4 + i] = (byte)((index1 << 6) | (index2 << 4) | (index3 << 2) | (index4));
			}
		}
		private static int LeastDistance(ReadOnlySpan<byte> palette, ReadOnlySpan<byte> color)
		{
			if(color[3] < _alphaThreshold)
			{
				return 3;
			}

			int dist = int.MaxValue;
			int best = 0;

			for(int i = 0; i < 3; i++)
			{
				if(palette[(i * 4) + 3] != 0xff)
				{
					break;
				}

				int distance = Distance(palette[(i * 4)..], color);

				if(distance < dist)
				{
					if(distance == 0)
					{
						return i;
					}

					dist = distance;
					best = i;
				}
			}

			return best;
		}
		private static int Distance(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
		{
			int result = 0;
			for(int i = 0; i < 3; i++)
			{
				int dif = a[i] - b[i];
				result += dif * dif;
			}

			return result;
		}
	}
}
