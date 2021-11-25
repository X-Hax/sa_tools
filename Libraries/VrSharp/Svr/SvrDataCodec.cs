using System;

namespace VrSharp.Svr
{
    public abstract class SvrDataCodec : VrDataCodec
    {
        #region Rectangle
        // Rectangle
        public class Rectangle : SvrDataCodec
        {
            public override bool CanEncode
            {
                get { return true; }
            }

            public override int Bpp
            {
                get { return PixelCodec.Bpp; }
            }

            public override byte[] Decode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data & index
                byte[] destination = new byte[width * height * 4];
                int destinationIndex = 0;

                // Decode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.DecodePixel(source, sourceIndex + GetSwizzledOffset(x, y, width, height, PixelCodec.Bpp), destination, destinationIndex);
                        destinationIndex += 4;
                    }
                }

                return destination;
            }

            public override byte[] Encode(byte[] source, int sourceIndex, int width, int height)
            {
                // Destination data
                byte[] destination = new byte[width * height * (PixelCodec.Bpp >> 3)];

                // Encode texture data
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        PixelCodec.EncodePixel(source, sourceIndex, destination, GetSwizzledOffset(x, y, width, height, PixelCodec.Bpp));
                        sourceIndex += 4;
                    }
                }

                return destination;
            }
        }
        #endregion

        #region 4-bit Indexed with External Palette
        // 4-bit Indexed with External Palette
        public class Index4ExtClut : SvrDataCodec
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

            public override bool NeedsExternalPalette
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output   = new byte[width * height * 4];
                byte[][] clut    = palette;
                int StartOffset = offset;
                bool IsSwizzled = (width >= 128 && height >= 128);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // We use width = height for swizzling. This is for rectangle textures
                        // as they require that. Square textures already are width = height.
                        // Most significant bits first
                        byte entry = input[StartOffset + GetSwizzledOffset4(x, y, width, height)];
                        if (IsSwizzled)
                            entry = (byte)((entry >> ((y >> 1) & 0x01) * 4) & 0x0F);
                        else
                            entry = (byte)((entry >> (x & 0x01) * 4) & 0x0F);

                        output[(((y * width) + x) * 4) + 3] = clut[entry][3];
                        output[(((y * width) + x) * 4) + 2] = clut[entry][2];
                        output[(((y * width) + x) * 4) + 1] = clut[entry][1];
                        output[(((y * width) + x) * 4) + 0] = clut[entry][0];

                        if ((x & 0x01) != 0)
                            offset++;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[(width * height) / 2];
                bool IsSwizzled = (width >= 128 && height >= 128);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int offset = GetSwizzledOffset4(x, y, width, height);
                        byte entry = (byte)(input[(y * width) + x] & 0x0F);
                        if (IsSwizzled)
                            entry = (byte)((output[offset] & (0x0F << ((y >> 1) & 0x01) * 4)) | (entry << ((~(y >> 1) & 0x01) * 4)));
                        else
                            entry = (byte)((output[offset] & (0x0F << (x & 0x01) * 4)) | (entry << ((~x & 0x01) * 4)));

                        output[GetSwizzledOffset4(x, y, width, height)] = entry;
                    }
                }

                return output;
            }
        }
        #endregion

        #region 8-bit Indexed with External Palette
        // 8-bit Indexed with External Palette
        public class Index8ExtClut : SvrDataCodec
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

            public override bool NeedsExternalPalette
            {
                get { return true; }
            }

            public override byte[] Decode(byte[] input, int offset, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output   = new byte[width * height * 4];
                byte[][] clut    = palette;
                int StartOffset = offset;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte entry = input[StartOffset + GetSwizzledOffset8(x, y, width, height)];
                        entry      = (byte)((entry & 0xE7) | ((entry & 0x10) >> 1) | ((entry & 0x08) << 1));

                        output[(((y * width) + x) * 4) + 3] = clut[entry][3];
                        output[(((y * width) + x) * 4) + 2] = clut[entry][2];
                        output[(((y * width) + x) * 4) + 1] = clut[entry][1];
                        output[(((y * width) + x) * 4) + 0] = clut[entry][0];

                        offset++;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte entry = input[(y * width) + x];
                        entry      = (byte)((entry & 0xE7) | ((entry & 0x10) >> 1) | ((entry & 0x08) << 1));

                        output[GetSwizzledOffset8(x, y, width, height)] = entry;
                    }
                }

                return output;
            }
        }
        #endregion

        #region 4-bit Indexed
        // 4-bit Indexed
        public class Index4 : SvrDataCodec
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
                byte[] output   = new byte[width * height * 4];
                byte[][] clut    = palette;
                int StartOffset = offset;
                bool IsSwizzled = (width >= 128 && height >= 128);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // We use width = height for swizzling. This is for rectangle textures
                        // as they require that. Square textures already are width = height.
                        // Most significant bits first
                        byte entry = input[StartOffset + GetSwizzledOffset4(x, y, width, height)];
                        if (IsSwizzled)
                            entry = (byte)((entry >> ((y >> 1) & 0x01) * 4) & 0x0F);
                        else
                            entry = (byte)((entry >> (x & 0x01) * 4) & 0x0F);

                        output[(((y * width) + x) * 4) + 3] = clut[entry][3];
                        output[(((y * width) + x) * 4) + 2] = clut[entry][2];
                        output[(((y * width) + x) * 4) + 1] = clut[entry][1];
                        output[(((y * width) + x) * 4) + 0] = clut[entry][0];

                        if ((x & 0x01) != 0)
                            offset++;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[(width * height) / 2];
                bool IsSwizzled = (width >= 128 && height >= 128);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int offset = GetSwizzledOffset4(x, y, width, height);
                        byte entry = (byte)(input[(y * width) + x] & 0x0F);
                        if (IsSwizzled)
                            entry = (byte)((output[offset] & (0x0F << ((y >> 1) & 0x01) * 4)) | (entry << ((~(y >> 1) & 0x01) * 4)));
                        else
                            entry = (byte)((output[offset] & (0x0F << (x & 0x01) * 4)) | (entry << ((~x & 0x01) * 4)));

                        output[GetSwizzledOffset4(x, y, width, height)] = entry;
                    }
                }

                return output;
            }
        }
        #endregion

        #region 8-bit Indexed
        // 8-bit Indexed
        public class Index8 : SvrDataCodec
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
                byte[] output   = new byte[width * height * 4];
                byte[][] clut    = palette;
                int StartOffset = offset;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte entry = input[StartOffset + GetSwizzledOffset8(x, y, width, height)];
                        entry      = (byte)((entry & 0xE7) | ((entry & 0x10) >> 1) | ((entry & 0x08) << 1));

                        output[(((y * width) + x) * 4) + 3] = clut[entry][3];
                        output[(((y * width) + x) * 4) + 2] = clut[entry][2];
                        output[(((y * width) + x) * 4) + 1] = clut[entry][1];
                        output[(((y * width) + x) * 4) + 0] = clut[entry][0];

                        offset++;
                    }
                }

                return output;
            }

            public override byte[] Encode(byte[] input, int width, int height, VrPixelCodec PixelCodec)
            {
                byte[] output = new byte[width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte entry = input[(y * width) + x];
                        entry      = (byte)((entry & 0xE7) | ((entry & 0x10) >> 1) | ((entry & 0x08) << 1));

                        output[GetSwizzledOffset8(x, y, width, height)] = entry;
                    }
                }

                return output;
            }
        }
        #endregion

        #region Swizzle Code
        private int GetSwizzledOffset(int x, int y, int width, int height, int bpp)
        {
            switch (bpp)
            {
                case 4:  return GetSwizzledOffset4(x, y, width, height);
                case 8:  return GetSwizzledOffset8(x, y, width, height);
                case 16: return GetSwizzledOffset16(x, y, width, height); // If it is like 4-bit it needs width for height
                case 32: return (((y * width) + x) * 4); // 32-bit textures aren't swizzled
            }

            return ((y * width) + x); // Shouldn't ever reach here
        }

        private int GetSwizzledOffset4(int x, int y, int width, int height)
        {
            if (width < 128 || height < 128) // Texture is to small for it to be swizzled
                return ((y * width) + x) >> 1;

            height = width; // Set height = width

            int PageX   = x & (~0x7f);
            int PageY   = y & (~0x7f);
            int PagesH  = (width + 127) / 128;
            int PagesV  = (height + 127) / 128;
            int PageNum = (PageY / 128) * PagesH + (PageX / 128);

            int Page32Y = (PageNum / PagesV) * 32;
            int Page32X = (PageNum % PagesV) * 64;

            int PagePos = Page32Y * height * 2 + Page32X * 4;

            int LocX = x & 0x7f;
            int LocY = y & 0x7f;

            int BlockPos  = ((LocX & (~0x1f)) >> 1) * height + (LocY & (~0xf)) * 2;
            int SwapSel   = (((y + 2) >> 2) & 0x1) * 4;
            int YPos      = (((y & (~3)) >> 1) + (y & 1)) & 0x7;
            int ColoumPos = YPos * height * 2 + ((x + SwapSel) & 0x7) * 4;

            int ByteNum = (x >> 3) & 3;     // 0,1,2,3

            return PagePos + BlockPos + ColoumPos + ByteNum;
        }

        private int GetSwizzledOffset8(int x, int y, int width, int height)
        {
            if (width < 128 || height < 64) // Texture is to small for it to be swizzled
                return ((y * width) + x);

            int BlockPos  = (y & (~0xf)) * width + (x & (~0xf)) * 2;
            int SwapSel   = (((y + 2) >> 2) & 0x1) * 4;
            int YPos      = (((y & (~3)) >> 1) + (y & 1)) & 0x7;
            int ColoumPos = YPos * width * 2 + ((x + SwapSel) & 0x7) * 4;
            int ByteNum   = ((y >> 1) & 1) + ((x >> 2) & 2); // 0, 1, 2, 3

            return BlockPos + ColoumPos + ByteNum;
        }

        private int GetSwizzledOffset16(int x, int y, int width, int height)
        {
            if (width < 64 || height < 64) // Texture is to small for it to be swizzled
                return ((y * width) + x) << 1;

            height = width; // Set height = width

            int PageX   = x & (~0x3f);
            int PageY   = y & (~0x3f);
            int PagesH  = (width + 63) / 64;
            int PagesV  = (height + 63) / 64;
            int PageNum = (PageY / 64) * PagesH + (PageX / 64);

            int Page32Y = (PageNum / PagesV) * 32;
            int Page32X = (PageNum % PagesV) * 64;

            int PagePos = (Page32Y * height + Page32X) * 2;

            int LocX = x & 0x3f;
            int LocY = y & 0x3f;

            int BlockPos  = (LocX & (~0xf)) * height + (LocY & (~0x7)) * 2;
            int ColoumPos = ((y & 0x7) * height + (x & 0x7)) * 2;

            int ByteNum = (x >> 3) & 1;       // 0,1

            return (PagePos + BlockPos + ColoumPos + ByteNum) << 1;
        }
        #endregion

        #region Get Codec
        public static SvrDataCodec GetDataCodec(SvrDataFormat format)
        {
            switch (format)
            {
                case SvrDataFormat.Rectangle:
                    return new Rectangle();
                case SvrDataFormat.Index4ExternalPalette:
                    return new Index4ExtClut();
                case SvrDataFormat.Index8ExternalPalette:
                    return new Index8ExtClut();
                case SvrDataFormat.Index4Rgb5a3Rectangle:
                case SvrDataFormat.Index4Rgb5a3Square:
                case SvrDataFormat.Index4Argb8Rectangle:
                case SvrDataFormat.Index4Argb8Square:
                    return new Index4();
                case SvrDataFormat.Index8Rgb5a3Rectangle:
                case SvrDataFormat.Index8Rgb5a3Square:
                case SvrDataFormat.Index8Argb8Rectangle:
                case SvrDataFormat.Index8Argb8Square:
                    return new Index8();
            }

            return null;
        }
        #endregion
    }
}