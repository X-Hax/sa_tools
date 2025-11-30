using System;

namespace TextureLib
{
    public class RGB555PixelCodec : PixelCodec
    {
        public override int BytesPerPixel => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
            byte r = (byte)((src[bigEndian ? 0 : 1] & 0x7C) << 1);
            dst[0] = (byte)(r | (r >> 5));

            byte g = (byte)((src[bigEndian ? 0 : 1] << 6) | ((src[bigEndian ? 1 : 0] & 0xE0) >> 2));
            dst[1] = (byte)(g | (g >> 5));

            byte b = (byte)(src[bigEndian ? 1 : 0] << 3);
            dst[2] = (byte)(b | (b >> 5));

            dst[3] = 0xFF;
        }

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian)
        {
            dst[bigEndian ? 0 : 1] = (byte)(((src[0] & 0xF8) >> 1) | (src[1] >> 6));
            dst[bigEndian ? 1 : 0] = (byte)(((src[1] & 0xF8) << 2) | ((src[2] & 0xF8) >> 3));
        }
    }
}