using System;

namespace TextureLib
{
    internal class GvrRGBA4444PixelCodec : UncompressedPixelCodec
    {
        protected override ByteType Type => ByteType.HalfPixel;

        protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            RGB4444ToRGBA8(src, dst);
        }

        public static void RGB4444ToRGBA8(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian = true)
        {
            byte r = (byte)(src[bigEndian ? 0 : 1] & 0x0F);
            dst[0] = (byte)(r | (r << 4));

            byte g = (byte)(src[bigEndian ? 1 : 0] & 0xF0);
            dst[1] = (byte)(g | (g >> 4));

            byte b = (byte)(src[bigEndian ? 1 : 0] & 0x0F);
            dst[2] = (byte)(b | (b << 4));

            byte a = (byte)(src[bigEndian ? 0 : 1] & 0xF0);
            dst[3] = (byte)(a | (a >> 4));
        }

        protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            RGBA8ToRGBA4444(src, dst);
        }

        public static void RGBA8ToRGBA4444(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian = true)
        {
            dst[bigEndian ? 0 : 1] = (byte)((src[3] & 0xF0) | (src[0] >> 4));
            dst[bigEndian ? 1 : 0] = (byte)((src[1] & 0xF0) | (src[2] >> 4));
        }
    }
}