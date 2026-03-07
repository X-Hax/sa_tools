using System;

namespace TextureLib
{
    internal class GvrRGBA1555DataCodec : GvrUncompressedDataCodec
    {
        protected override ByteType Type => ByteType.HalfPixel;

        protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            RGB1555ToRGBA8(src, dst);
        }

        public static void RGB1555ToRGBA8(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian = true)
        {
            byte r = (byte)((src[bigEndian ? 0 : 1] & 0x7C) << 1);
            dst[0] = (byte)(r | (r >> 5));

            byte g = (byte)((src[bigEndian ? 0 : 1] << 6) | ((src[bigEndian ? 1 : 0] & 0xE0) >> 2));
            dst[1] = (byte)(g | (g >> 5));

            byte b = (byte)(src[bigEndian ? 1 : 0] << 3);
            dst[2] = (byte)(b | (b >> 5));

            dst[3] = (byte)((src[bigEndian ? 0 : 1] & 0x80) == 0x80 ? 0xFF : 0);
        }

        protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            RGBA8ToRGBA1555(src, dst);
        }

        public static void RGBA8ToRGBA1555(ReadOnlySpan<byte> src, Span<byte> dst, bool bigEndian = true)
        {
            dst[bigEndian ? 0 : 1] = (byte)((src[3] & 0x80) | ((src[0] & 0xF8) >> 1) | (src[1] >> 6));
            dst[bigEndian ? 1 : 0] = (byte)(((src[1] & 0xF8) << 2) | ((src[2] & 0xF8) >> 3));
        }
    }
}