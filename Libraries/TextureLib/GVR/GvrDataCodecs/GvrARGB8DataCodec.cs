using System;

namespace TextureLib
{
    /*
       The RGBA32 format (also known as RGBA8) is used to store 24 bit depth true color (1 byte per color), with an 8 bit alpha channel. Although the pixel data does follow the block order as seen in other formats, the data is separated into 2 groups. A and R are encoded in this order in the first group, and G and B in the second group.

       So one block in this format (4x4 pixels), as 64 bytes, appears in this order:

       ARARARARARARARAR
       ARARARARARARARAR
       GBGBGBGBGBGBGBGB
       GBGBGBGBGBGBGBGB

       From: https://wiki.tockdom.com/wiki/Image_Formats#RGBA32_(RGBA8)
    */

    internal class GvrARGB8DataCodec : GvrUncompressedDataCodec
	{
		protected override ByteType Type => ByteType.QuarterPixel;

		protected override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
            dst[0] = src[1];
            dst[1] = src[32];
            dst[2] = src[33];
            dst[3] = src[0];
        }

		protected override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
		{
            dst[0] = src[3];
            dst[1] = src[0];
            dst[32] = src[1];
            dst[33] = src[2];
        }
	}
}
