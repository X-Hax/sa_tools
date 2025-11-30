using System;

namespace TextureLib
{
    internal class RGB5A3PixelCodec : PixelCodec
    {
        public override int BytesPerPixel => 2;

        // Little Endian decoding not tested
        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
            if (!bigEndian && (src[1] & 0x80) != 0 || (bigEndian && (src[0] & 0x80) != 0)) // RGB555
            {
                byte r = (byte)((src[bigEndian ? 0 : 1] & 0x7C) << 1);
                dst[0] = (byte)(r | (r >> 5));

                byte g = (byte)((src[bigEndian ? 0 : 1] << 6) | ((src[bigEndian ? 1 : 0] & 0xE0) >> 2));
                dst[1] = (byte)(g | (g >> 5));

                byte b = (byte)(src[bigEndian ? 1 : 0] << 3);
                dst[2] = (byte)(b | (b >> 5));

                dst[3] = 0xFF;
            }
            else // ARGB3444
            {
                byte r = (byte)(src[bigEndian ? 0 : 1] & 0x0F);
                dst[0] = (byte)(r | (r << 4));

                byte g = (byte)(src[bigEndian ? 1 : 0] & 0xF0);
                dst[1] = (byte)(g | (g >> 4));

                byte b = (byte)(src[bigEndian ? 1 : 0] & 0x0F);
                dst[2] = (byte)(b | (b << 4));

                byte a = (byte)((src[bigEndian ? 0 : 1] & 0x70) << 1);
                dst[3] = (byte)(a | (a >> 3) | (a >> 6));
            }
        }

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
            if (src[3] >= 0xE0) // RGB555
            {
                dst[bigEndian ? 0 : 1] = (byte)(0x80 | ((src[0] & 0xF8) >> 1) | (src[1] >> 6));
                dst[bigEndian ? 1 : 0] = (byte)(((src[1] & 0xF8) << 2) | ((src[2] & 0xF8) >> 3));
            }
            else // ARGB3444
            {
                dst[bigEndian ? 0 : 1] = (byte)(((src[3] & 0xE0) >> 1) | (src[0] >> 4));
                dst[bigEndian ? 1 : 0] = (byte)((src[1] & 0xF0) | (src[2] >> 4));
            }
        }
    }
}