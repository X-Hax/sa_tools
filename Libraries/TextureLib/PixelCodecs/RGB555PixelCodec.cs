using System;

namespace TextureLib
{
    public class RGB555PixelCodec : PixelCodec
    {
        public override int BytesPerPixel => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            byte r = (byte)((src[BigEndian ? 0 : 1] & 0x7C) << 1);
            dst[0] = (byte)(r | (r >> 5));

            byte g = (byte)((src[BigEndian ? 0 : 1] << 6) | ((src[BigEndian ? 1 : 0] & 0xE0) >> 2));
            dst[1] = (byte)(g | (g >> 5));

            byte b = (byte)(src[BigEndian ? 1 : 0] << 3);
            dst[2] = (byte)(b | (b >> 5));

            dst[3] = 0xFF;
        }

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
            dst[BigEndian ? 0 : 1] = (byte)(((src[0] & 0xF8) >> 1) | (src[1] >> 6));
            dst[BigEndian ? 1 : 0] = (byte)(((src[1] & 0xF8) << 2) | ((src[2] & 0xF8) >> 3));
        }

		public override string Info()
		{
			return "RGB555, 16 bit (2 bytes per pixel)" + (BigEndian ? ", Big Endian" : "");
		}
	}
}