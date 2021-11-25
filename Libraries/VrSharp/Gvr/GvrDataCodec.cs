using System;

namespace VrSharp.Gvr
{
    public abstract class GvrDataCodec : VrDataCodec
    {
        #region Intensity 4-bit
        // Intensity 4-bit
        public class Intensity4 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 4; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                byte entry = (byte)((input[offset] >> ((~x2 & 0x01) * 4)) & 0x0F);

                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = 0xFF;
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(entry * 0xFF / 0x0F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(entry * 0xFF / 0x0F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(entry * 0xFF / 0x0F);

                                if ((x2 & 0x01) != 0)
                                    offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[(width * height) / 2];

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                int loc    = (((y + y2) * width) + (x + x2)) * 4;
                                byte entry = (byte)(((0.30 * input[loc + 2]) + (0.59 * input[loc + 1]) + (0.11 * input[loc + 0])) * 0x0F / 0xFF);
                                entry      = (byte)((output[offset] & (0x0F << (x2 & 0x01) * 4)) | (entry << ((~x2 & 0x01) * 4)));

                                output[offset] = entry;

                                if ((x2 & 0x01) != 0)
                                    offset++;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Intensity 8-bit
        // Intensity 8-bit
        public class Intensity8 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 8; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = 0xFF;
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = input[offset];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = input[offset];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = input[offset];

                                offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[width * height];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                int loc    = ((y + y2) * width) + (x + x2);
                                byte entry = (byte)((0.30 * input[loc + 2]) + (0.59 * input[loc + 1]) + (0.11 * input[loc + 0]));

                                output[offset] = entry;
                                offset++;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Intensity 4-bit with Alpha
        // Intensity 4-bit with Alpha
        public class IntensityA4 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 8; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = (byte)(((input[offset] >> 4) & 0x0F) * 0xFF / 0x0F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)((input[offset] & 0x0F) * 0xFF / 0x0F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)((input[offset] & 0x0F) * 0xFF / 0x0F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)((input[offset] & 0x0F) * 0xFF / 0x0F);

                                offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[width * height];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                int loc    = ((y + y2) * width) + (x + x2);
                                byte entry = (byte)(((0.30 * input[loc + 2]) + (0.59 * input[loc + 1]) + (0.11 * input[loc + 0])) * 0x0F / 0xFF);
                                entry      = (byte)(((((input[loc + 3]) * 0x0F / 0xFF) & 0x0F) << 4) | (entry & 0x0F));

                                output[offset] = entry;
                                offset++;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Intensity 8-bit with Alpha
        // Intensity 8-bit with Alpha
        public class IntensityA8 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 16; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = input[offset];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = input[offset + 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = input[offset + 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = input[offset + 1];

                                offset += 2;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[width * height * 2];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                int loc    = ((y + y2) * width) + (x + x2);
                                byte entry = (byte)((0.30 * input[loc + 2]) + (0.59 * input[loc + 1]) + (0.11 * input[loc + 0]));

                                output[offset + 0] = input[loc + 3];
                                output[offset + 1] = entry;
                                offset += 2;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Rgb565
        // Rgb565
        public class Rgb565 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 16; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                ushort pixel = PTMethods.ToUInt16BE(input, offset);

                                //output[((((y + y2) * width) + (x + x2)) * 4) + 3] = 0xFF;
                                //output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(((pixel >> 11) & 0x1F) << 11);
                                //output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(((pixel >> 5) & 0x3F) << 10);
                                //output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(((pixel >> 0) & 0x1F) << 11);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = 0xFF;
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(((pixel >> 11) & 0x1F) * 0xFF / 0x1F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(((pixel >> 5)  & 0x3F) * 0xFF / 0x3F);
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);

                                offset += 2;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset    = 0;
                byte[] output = new byte[width * height * 2];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                ushort pixel = 0x0000;
                                pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 2] >> 3) << 11);
                                pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 1] >> 2) << 5);
                                pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 0] >> 3) << 0);
                                //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 2] * 0x1F / 0xFF) & 0x1F) << 11);
                                //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 1] * 0x3F / 0xFF) & 0x3F) << 5);
                                //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 0] * 0x1F / 0xFF) & 0x1F) << 0);

                                PTMethods.GetBytesBE(pixel).CopyTo(output, offset);
                                offset += 2;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Rgb5a3
        // Rgb5a3
        public class Rgb5a3 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 16; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                ushort pixel = PTMethods.ToUInt16BE(input, offset);

                                if ((pixel & 0x8000) != 0) // Rgb555
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 3] = 0xFF;
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(((pixel >> 10) & 0x1F) * 0xFF / 0x1F);
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(((pixel >> 5)  & 0x1F) * 0xFF / 0x1F);
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(((pixel >> 0)  & 0x1F) * 0xFF / 0x1F);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 3] = 0xFF;
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(((pixel >> 10) & 0x1F) << 11);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(((pixel >> 5) & 0x1F) << 11);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(((pixel >> 0) & 0x1F) << 11);
                                }
                                else // Argb3444
                                {
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 3] = (byte)(((pixel >> 12) & 0x07) * 0xFF / 0x07);
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(((pixel >> 8)  & 0x0F) * 0xFF / 0x0F);
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(((pixel >> 4)  & 0x0F) * 0xFF / 0x0F);
                                    output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(((pixel >> 0)  & 0x0F) * 0xFF / 0x0F);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 3] = (byte)(((pixel >> 12) & 0x07) << 13);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 2] = (byte)(((pixel >> 8) & 0x0F) << 12);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 1] = (byte)(((pixel >> 4) & 0x0F) << 12);
                                    //output[((((y + y2) * width) + (x + x2)) * 4) + 0] = (byte)(((pixel >> 0) & 0x0F) << 12);
                                }

                                offset += 2;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset    = 0;
                byte[] output = new byte[width * height * 2];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                ushort pixel = 0x0000;

                                if (input[((((y + y2) * width) + (x + x2)) * 4) + 3] <= 0xDA) // Argb3444
                                {
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 3] >> 5) << 12);
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 2] >> 4) << 8);
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 1] >> 4) << 4);
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 0] >> 4) << 0);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 3] * 0x07 / 0xFF) & 0x07) << 12);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 2] * 0x0F / 0xFF) & 0x0F) << 8);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 1] * 0x0F / 0xFF) & 0x0F) << 4);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 0] * 0x0F / 0xFF) & 0x0F) << 0);
                                }
                                else // Rgb555
                                {
                                    pixel |= 0x8000;
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 2] >> 3) << 10);
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 1] >> 3) << 5);
                                    pixel |= (ushort)((input[((((y + y2) * width) + (x + x2)) * 4) + 0] >> 3) << 0);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 2] * 0x1F / 0xFF) & 0x1F) << 10);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 1] * 0x1F / 0xFF) & 0x1F) << 5);
                                    //pixel |= (ushort)(((input[((((y + y2) * width) + (x + x2)) * 4) + 0] * 0x1F / 0xFF) & 0x1F) << 0);
                                }

                                PTMethods.GetBytesBE(pixel).CopyTo(output, offset);
                                offset += 2;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Argb8888
        // Argb8888
        public class Argb8888 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 32; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = input[offset + 0];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = input[offset + 1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = input[offset + 32];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = input[offset + 33];

                                offset += 2;
                            }
                        }

                        offset += 32;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset    = 0;
                byte[] output = new byte[width * height * 4];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 4)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 4; x2++)
                            {
                                output[offset + 0]  = input[((((y + y2) * width) + (x + x2)) * 4) + 3];
                                output[offset + 1]  = input[((((y + y2) * width) + (x + x2)) * 4) + 2];
                                output[offset + 32] = input[((((y + y2) * width) + (x + x2)) * 4) + 1];
                                output[offset + 33] = input[((((y + y2) * width) + (x + x2)) * 4) + 0];

                                offset += 2;
                            }
                        }

                        offset += 32;
                    }
                }

                return output;
            }
        }
        #endregion

        #region 4-bit Indexed
        // 4-bit Indexed
        public class Index4 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 4; }
            }

            public override int PaletteEntries
            {
                get { return 16; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                byte[][] clut  = palette;

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                byte entry = (byte)((input[offset] >> ((~x2 & 0x01) * 4)) & 0x0F);

                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[entry][3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[entry][2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[entry][1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[entry][0];

                                if ((x2 & 0x01) != 0)
                                    offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[(width * height) / 2];

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                byte entry = (byte)(input[((y + y2) * width) + (x + x2)] & 0x0F);
                                entry = (byte)((output[offset] & (0x0F << (x2 & 0x01) * 4)) | (entry << ((~x2 & 0x01) * 4)));

                                output[offset] = entry;

                                if ((x2 & 0x01) != 0)
                                    offset++;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region 8-bit Indexed
        // 8-bit Indexed
        public class Index8 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 8; }
            }

            public override int PaletteEntries
            {
                get { return 256; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];
                byte[][] clut  = palette;

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                output[((((y + y2) * width) + (x + x2)) * 4) + 3] = clut[input[offset]][3];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 2] = clut[input[offset]][2];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 1] = clut[input[offset]][1];
                                output[((((y + y2) * width) + (x + x2)) * 4) + 0] = clut[input[offset]][0];

                                offset++;
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[width * height];

                for (int y = 0; y < height; y += 4)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 4; y2++)
                        {
                            for (int x2 = 0; x2 < 8; x2++)
                            {
                                output[offset] = input[((y + y2) * width) + (x + x2)];
                                offset++;
                            }
                        }
                    }
                }

                return output;
            }
        }
        #endregion

        #region Dxt1 Texture Compression
        // Dxt1 Texture Compression
        public class Dxt1 : GvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return 4; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height * 4];

                // Palette for each 4x4 block
                byte[][] palette = new byte[4][];
                palette[0] = new byte[4];
                palette[1] = new byte[4];
                palette[2] = new byte[4];
                palette[3] = new byte[4];

                // The two colors that determine the palette
                ushort[] pixel = new ushort[2];

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2 += 4)
                        {
                            for (int x2 = 0; x2 < 8; x2 += 4)
                            {
                                // Get the first two colors
                                pixel[0] = PTMethods.ToUInt16BE(input, offset);
                                pixel[1] = PTMethods.ToUInt16BE(input, offset + 2);

                                palette[0][3] = 0xFF;
                                palette[0][2] = (byte)(((pixel[0] >> 11) & 0x1F) * 0xFF / 0x1F);
                                palette[0][1] = (byte)(((pixel[0] >> 5)  & 0x3F) * 0xFF / 0x3F);
                                palette[0][0] = (byte)(((pixel[0] >> 0)  & 0x1F) * 0xFF / 0x1F);

                                palette[1][3] = 0xFF;
                                palette[1][2] = (byte)(((pixel[1] >> 11) & 0x1F) * 0xFF / 0x1F);
                                palette[1][1] = (byte)(((pixel[1] >> 5)  & 0x3F) * 0xFF / 0x3F);
                                palette[1][0] = (byte)(((pixel[1] >> 0)  & 0x1F) * 0xFF / 0x1F);

                                // Determine the next two colors based on how the first two are stored
                                if (pixel[0] > pixel[1])
                                {
                                    palette[2][3] = 0xFF;
                                    palette[2][2] = (byte)(((palette[0][2] * 2) + palette[1][2]) / 3);
                                    palette[2][1] = (byte)(((palette[0][1] * 2) + palette[1][1]) / 3);
                                    palette[2][0] = (byte)(((palette[0][0] * 2) + palette[1][0]) / 3);

                                    palette[3][3] = 0xFF;
                                    palette[3][2] = (byte)(((palette[1][2] * 2) + palette[0][2]) / 3);
                                    palette[3][1] = (byte)(((palette[1][1] * 2) + palette[0][1]) / 3);
                                    palette[3][0] = (byte)(((palette[1][0] * 2) + palette[0][0]) / 3);
                                }
                                else
                                {
                                    palette[2][3] = 0xFF;
                                    palette[2][2] = (byte)((palette[0][2] + palette[1][2]) / 2);
                                    palette[2][1] = (byte)((palette[0][1] + palette[1][1]) / 2);
                                    palette[2][0] = (byte)((palette[0][0] + palette[1][0]) / 2);

                                    palette[3][3] = 0x00;
                                    palette[3][2] = 0x00;
                                    palette[3][1] = 0x00;
                                    palette[3][0] = 0x00;
                                }

                                offset += 4;

                                for (int y3 = 0; y3 < 4; y3++)
                                {
                                    for (int x3 = 0; x3 < 4; x3++)
                                    {
                                        output[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 3] = palette[((input[offset] >> (6 - (x3 * 2))) & 0x03)][3];
                                        output[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 2] = palette[((input[offset] >> (6 - (x3 * 2))) & 0x03)][2];
                                        output[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 1] = palette[((input[offset] >> (6 - (x3 * 2))) & 0x03)][1];
                                        output[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 0] = palette[((input[offset] >> (6 - (x3 * 2))) & 0x03)][0];
                                    }

                                    offset++;
                                }
                            }
                        }
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                int offset = 0;
                byte[] output = new byte[width * height / 2];

                byte[] subBlock;
                byte[] result;

                result = new byte[32];
                subBlock = new byte[64];

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        for (int y2 = 0; y2 < 8; y2 += 4)
                        {
                            for (int x2 = 0; x2 < 8; x2 += 4)
                            {
                                int i = 0;

                                for (int y3 = 0; y3 < 4; y3++)
                                {
                                    for (int x3 = 0; x3 < 4; x3++)
                                    {
                                        subBlock[i + 3] = input[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 3];
                                        subBlock[i + 2] = input[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 2];
                                        subBlock[i + 1] = input[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 1];
                                        subBlock[i + 0] = input[((((y + y2 + y3) * width) + (x + x2 + x3)) * 4) + 0];

                                        i += 4;
                                    }
                                }

                                ConvertBlockToQuaterCmpr(subBlock).CopyTo(output, offset);
                                offset += 8;
                            }
                        }
                    }
                }

                return output;
            }

            // Methods below from CTools Wii
            private static byte[] ConvertBlockToQuaterCmpr(byte[] block)
            {
                int col1, col2, dist, temp;
                bool alpha;
                byte[][] palette;
                byte[] result;

                dist = col1 = col2 = -1;
                alpha = false;
                result = new byte[8];

                for (int i = 0; i < 15; i++)
                {
                    if (block[i * 4 + 3] < 16)
                        alpha = true;
                    else
                    {
                        for (int j = i + 1; j < 16; j++)
                        {
                            temp = Distance(block, i * 4, block, j * 4);

                            if (temp > dist)
                            {
                                dist = temp;
                                col1 = i;
                                col2 = j;
                            }
                        }
                    }
                }

                if (dist == -1)
                {
                    palette = new byte[][] { new byte[] { 0, 0, 0, 0xff }, new byte[] { 0xff, 0xff, 0xff, 0xff }, null, null };
                }
                else
                {
                    palette = new byte[4][];
                    palette[0] = new byte[4];
                    palette[1] = new byte[4];

                    Array.Copy(block, col1 * 4, palette[0], 0, 3);
                    palette[0][3] = 0xff;
                    Array.Copy(block, col2 * 4, palette[1], 0, 3);
                    palette[1][3] = 0xff;

                    if (palette[0][0] >> 3 == palette[1][0] >> 3 && palette[0][1] >> 2 == palette[1][1] >> 2 && palette[0][2] >> 3 == palette[1][2] >> 3)
                        if (palette[0][0] >> 3 == 0 && palette[0][1] >> 2 == 0 && palette[0][2] >> 3 == 0)
                            palette[1][0] = palette[1][1] = palette[1][2] = 0xff;
                        else
                            palette[1][0] = palette[1][1] = palette[1][2] = 0x0;
                }

                result[0] = (byte)(palette[0][2] & 0xf8 | palette[0][1] >> 5);
                result[1] = (byte)(palette[0][1] << 3 & 0xe0 | palette[0][0] >> 3);
                result[2] = (byte)(palette[1][2] & 0xf8 | palette[1][1] >> 5);
                result[3] = (byte)(palette[1][1] << 3 & 0xe0 | palette[1][0] >> 3);

                if ((result[0] > result[2] || (result[0] == result[2] && result[1] >= result[3])) == alpha)
                {
                    Array.Copy(result, 0, result, 4, 2);
                    Array.Copy(result, 2, result, 0, 2);
                    Array.Copy(result, 4, result, 2, 2);

                    palette[2] = palette[0];
                    palette[0] = palette[1];
                    palette[1] = palette[2];
                }

                if (!alpha)
                {
                    palette[2] = new byte[] { (byte)(((palette[0][0] << 1) + palette[1][0]) / 3), (byte)(((palette[0][1] << 1) + palette[1][1]) / 3), (byte)(((palette[0][2] << 1) + palette[1][2]) / 3), 0xff };
                    palette[3] = new byte[] { (byte)((palette[0][0] + (palette[1][0] << 1)) / 3), (byte)((palette[0][1] + (palette[1][1] << 1)) / 3), (byte)((palette[0][2] + (palette[1][2] << 1)) / 3), 0xff };
                }
                else
                {
                    palette[2] = new byte[] { (byte)((palette[0][0] + palette[1][0]) >> 1), (byte)((palette[0][1] + palette[1][1]) >> 1), (byte)((palette[0][2] + palette[1][2]) >> 1), 0xff };
                    palette[3] = new byte[] { 0, 0, 0, 0 };
                }

                for (int i = 0; i < block.Length >> 4; i++)
                {
                    result[4 + i] = (byte)(LeastDistance(palette, block, i * 16 + 0) << 6 | LeastDistance(palette, block, i * 16 + 4) << 4 | LeastDistance(palette, block, i * 16 + 8) << 2 | LeastDistance(palette, block, i * 16 + 12));
                }

                return result;
            }
            private static int LeastDistance(byte[][] palette, byte[] colour, int offset)
            {
                int dist, best, temp;

                if (colour[offset + 3] < 8)
                    return 3;

                dist = int.MaxValue;
                best = 0;

                for (int i = 0; i < palette.Length; i++)
                {
                    if (palette[i][3] != 0xff)
                        break;

                    temp = Distance(palette[i], 0, colour, offset);

                    if (temp < dist)
                    {
                        if (temp == 0)
                            return i;

                        dist = temp;
                        best = i;
                    }
                }

                return best;
            }
            private static int Distance(byte[] colour1, int offset1, byte[] colour2, int offset2)
            {
                int temp, val;

                temp = 0;

                for (int i = 0; i < 3; i++)
                {
                    val = colour1[offset1 + i] - colour2[offset2 + i];
                    temp += val * val;
                }

                return temp;
            }
        }
        #endregion

        #region Get Codec
        public static GvrDataCodec GetDataCodec(GvrDataFormat format)
        {
            switch (format)
            {
                case GvrDataFormat.Intensity4:
                    return new Intensity4();
                case GvrDataFormat.Intensity8:
                    return new Intensity8();
                case GvrDataFormat.IntensityA4:
                    return new IntensityA4();
                case GvrDataFormat.IntensityA8:
                    return new IntensityA8();
                case GvrDataFormat.Rgb565:
                    return new Rgb565();
                case GvrDataFormat.Rgb5a3:
                    return new Rgb5a3();
                case GvrDataFormat.Argb8888:
                    return new Argb8888();
                case GvrDataFormat.Index4:
                    return new Index4();
                case GvrDataFormat.Index8:
                    return new Index8();
                case GvrDataFormat.Dxt1:
                    return new Dxt1();
            }

            return null;
        }
        #endregion
    }
}