using System;

namespace TextureLib
{
    internal class GvrIndex14DataCodec : GvrUncompressedDataCodec
    {
        public override int PaletteEntries => 16384;
        protected override ByteType Type => ByteType.Pixel;

        protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            dst[0] = src[0];
        }

        protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            dst[0] = src[0];
        }
    }
}
