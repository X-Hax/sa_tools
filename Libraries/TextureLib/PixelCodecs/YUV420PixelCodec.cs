using System;
using System.Collections.Generic;
using System.IO;

namespace TextureLib
{
    internal class YUV420PixelCodec : PixelCodec
    {
        public override int BytesPerPixel => 4;

        public override int Pixels => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
            if (dst.Length > 4)
            {
                sbyte u = (sbyte)(src[bigEndian ? 3 : 0] - 128);
                byte y1 = src[bigEndian ? 2 : 1];
                sbyte v = (sbyte)(src[bigEndian ? 1 : 2] - 128);
                byte y2 = src[bigEndian ? 0 : 3];

                YUV2RGB(y1, u, v, dst);
                YUV2RGB(y2, u, v, dst[4..]);
            }
            else
            {
                sbyte u = (sbyte)(src[bigEndian ? 3 : 0] - 128);
                byte y1 = src[bigEndian ? 2 : 1];
                sbyte v = (sbyte)(src[bigEndian ? 1 : 2] - 128);
                byte y2 = src[bigEndian ? 0 : 3];

                YUV2RGB(y2, u, v, dst);
            }
        }

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
            if (src.Length > 4)
            {
                (byte y1, byte u1, byte v1) = RGB2YUV(src);
                (byte y2, byte u2, byte v2) = RGB2YUV(src[4..]);

                dst[bigEndian ? 3 : 0] = (byte)((u1 + u2) / 2);
                dst[bigEndian ? 2 : 1] = y1;
                dst[bigEndian ? 1 : 2] = (byte)((v1 + v2) / 2);
                dst[bigEndian ? 0 : 3] = y2;

            }
            else
            {
                (byte y, byte u, byte v) = RGB2YUV(src);

                dst[bigEndian ? 3 : 0] = u;
                dst[bigEndian ? 2 : 1] = y;
                dst[bigEndian ? 1 : 2] = v;
                dst[bigEndian ? 0 : 3] = y;
            }

        }

        private static void YUV2RGB(byte y, sbyte u, sbyte v, Span<byte> dst)
        {
            // Integer operation of ITU-R standard for YCbCr; Equal to PVR viewer
            static byte Clamp(int val)
            {
                return (byte)Math.Clamp(val, byte.MinValue, byte.MaxValue);
            }

            dst[0] = Clamp(y + v + (v >> 2) + (v >> 3) + (v >> 5));
            dst[1] = Clamp(y - ((u >> 2) + (u >> 4) + (u >> 5)) - ((v >> 1) + (v >> 3) + (v >> 4) + (v >> 5)));
            dst[2] = Clamp(y + u + (u >> 1) + (u >> 2) + (u >> 6));

            /* Y′UV to RGB (NTSC version); Higher contrasts
            int c = 298 * (y - 16) + 128;
            byte Calc(int val) => (byte)Math.Clamp((c + val) >> 8, byte.MinValue, byte.MaxValue);

            dst[0] = Calc(409 * v);
            dst[1] = Calc(-100 * u - 208 * v);
            dst[2] = Calc(516 * u);*/

            dst[3] = 0xFF;
        }

        private static (byte y, byte u, byte v) RGB2YUV(ReadOnlySpan<byte> src)
        {
            byte r = src[0];
            byte g = src[1];
            byte b = src[2];

            byte y = (byte)((r * 0.299) + (g * 0.587) + (b * 0.114));
            byte u = (byte)((r * -0.168) - (g * 0.331) + (b * 0.500) + 128);
            byte v = (byte)((r * 0.500) - (g * 0.418) - (b * 0.081) + 128);

            return (y, u, v);
        }


        public class Converter
        {
            public List<(byte, byte, byte)> Yuv420ToRgb(Stream f, int w, int h)
            {
                // precompute conversion coefficients
                int u_offset = -128;
                int v_offset = -128;
                double r_factor = 1.402;
                double g_u_factor = -0.344136;
                double g_v_factor = -0.714136;
                double b_factor = 1.772;

                // initialize RGB buffer
                byte[,,] rgb_data = new byte[h, w, 3];

                // calculate the number of macroblocks
                int mb_width = w / 16;
                int mb_height = h / 16;

                byte[] buffer64 = new byte[64];

                // loop over each macroblock (16x16 pixels)
                for (int mb_y = 0; mb_y < mb_height; mb_y++)
                {
                    for (int mb_x = 0; mb_x < mb_width; mb_x++)
                    {
                        // read U and V data for the 16x16 block (8x8 U and V values)
                        ReadFully(f, buffer64, 64);
                        byte[,] u_block = new byte[8, 8];
                        BufferTo2DArray(buffer64, u_block);

                        ReadFully(f, buffer64, 64);
                        byte[,] v_block = new byte[8, 8];
                        BufferTo2DArray(buffer64, v_block);

                        // read Y data for the four 8x8 blocks (Y0, Y1, Y2, Y3)
                        byte[] y_blocks_flat = new byte[64 * 4];
                        ReadFully(f, y_blocks_flat, 64 * 4);
                        byte[] y_block0_flat = new byte[64];
                        byte[] y_block1_flat = new byte[64];
                        byte[] y_block2_flat = new byte[64];
                        byte[] y_block3_flat = new byte[64];
                        Array.Copy(y_blocks_flat, 0, y_block0_flat, 0, 64);
                        Array.Copy(y_blocks_flat, 64, y_block1_flat, 0, 64);
                        Array.Copy(y_blocks_flat, 128, y_block2_flat, 0, 64);
                        Array.Copy(y_blocks_flat, 192, y_block3_flat, 0, 64);

                        byte[,] y_block0 = new byte[8, 8];
                        byte[,] y_block1 = new byte[8, 8];
                        byte[,] y_block2 = new byte[8, 8];
                        byte[,] y_block3 = new byte[8, 8];
                        BufferTo2DArray(y_block0_flat, y_block0);
                        BufferTo2DArray(y_block1_flat, y_block1);
                        BufferTo2DArray(y_block2_flat, y_block2);
                        BufferTo2DArray(y_block3_flat, y_block3);

                        // upscale U and V to 16x16 to match the 16x16 Y blocks using manual kron equivalent
                        byte[,] u_block_16 = Kron2D(u_block, 2);
                        byte[,] v_block_16 = Kron2D(v_block, 2);

                        // prepare Y data for the full 16x16 block
                        byte[,] full_y = new byte[16, 16];
                        CopyBlock(y_block0, full_y, 0, 0);
                        CopyBlock(y_block1, full_y, 0, 8);
                        CopyBlock(y_block2, full_y, 8, 0);
                        CopyBlock(y_block3, full_y, 8, 8);

                        // convert U, V, and Y to RGB
                        for (int y = 0; y < 16; y++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                int u = u_block_16[y, x] + u_offset;
                                int v = v_block_16[y, x] + v_offset;
                                int Y = full_y[y, x];

                                int r = ClampToByte(Y + (int)(r_factor * v));
                                int g = ClampToByte(Y + (int)(g_u_factor * u + g_v_factor * v));
                                int b = ClampToByte(Y + (int)(b_factor * u));

                                int global_y = mb_y * 16 + y;
                                int global_x = mb_x * 16 + x;

                                rgb_data[global_y, global_x, 0] = (byte)r;
                                rgb_data[global_y, global_x, 1] = (byte)g;
                                rgb_data[global_y, global_x, 2] = (byte)b;
                            }
                        }
                    }
                }

                // convert rgb_data to a list of RGB tuples
                List<(byte, byte, byte)> data = new List<(byte, byte, byte)>();
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        data.Add((
                            rgb_data[y, x, 0],
                            rgb_data[y, x, 1],
                            rgb_data[y, x, 2]
                        ));
                    }
                }

                return data;
            }

            private static void ReadFully(Stream stream, byte[] buffer, int count)
            {
                int offset = 0;
                while (count > 0)
                {
                    int read = stream.Read(buffer, offset, count);
                    if (read <= 0)
                        throw new EndOfStreamException();
                    offset += read;
                    count -= read;
                }
            }

            private static void BufferTo2DArray(byte[] buffer, byte[,] array)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        array[i, j] = buffer[i * cols + j];
            }

            private static byte[,] Kron2D(byte[,] input, int scale)
            {
                int rows = input.GetLength(0);
                int cols = input.GetLength(1);
                byte[,] output = new byte[rows * scale, cols * scale];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        byte val = input[i, j];
                        for (int di = 0; di < scale; di++)
                        {
                            for (int dj = 0; dj < scale; dj++)
                            {
                                output[i * scale + di, j * scale + dj] = val;
                            }
                        }
                    }
                }
                return output;
            }

            private static void CopyBlock(byte[,] src, byte[,] dst, int dstRow, int dstCol)
            {
                int rows = src.GetLength(0);
                int cols = src.GetLength(1);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        dst[dstRow + i, dstCol + j] = src[i, j];
            }

            private static int ClampToByte(int val)
            {
                if (val < 0) return 0;
                if (val > 255) return 255;
                return val;
            }
        }
    }
}