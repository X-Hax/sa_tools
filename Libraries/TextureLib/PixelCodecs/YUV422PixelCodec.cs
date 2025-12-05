using System;

namespace TextureLib
{
	internal class YUV422PixelCodec : PixelCodec
	{
        public override int BytesPerPixel => 4;

        public override int Pixels => 2;

        public override void DecodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			if(dst.Length > 4)
			{
				sbyte u = (sbyte)(src[BigEndian ? 3 : 0] - 128);
				byte y1 = src[BigEndian ? 2 : 1];
				sbyte v = (sbyte)(src[BigEndian ? 1 : 2] - 128);
				byte y2 = src[BigEndian ? 0 : 3];

				YUV2RGB(y1, u, v, dst);
				YUV2RGB(y2, u, v, dst[4..]);
			}
			else
			{
				sbyte u = (sbyte)(src[BigEndian ? 3 : 0] - 128);
				byte y1 = src[BigEndian ? 2 : 1];
				sbyte v = (sbyte)(src[BigEndian ? 1 : 2] - 128);
				byte y2 = src[BigEndian ? 0 : 3];

				YUV2RGB(y2, u, v, dst);
			}
		}

        public override void EncodePixel(ReadOnlySpan<byte> src, Span<byte> dst)
        {
			if(src.Length > 4)
			{
				(byte y1, byte u1, byte v1) = RGB2YUV(src);
				(byte y2, byte u2, byte v2) = RGB2YUV(src[4..]);

				dst[BigEndian ? 3 : 0] = (byte)((u1 + u2) / 2);
				dst[BigEndian ? 2 : 1] = y1;
				dst[BigEndian ? 1 : 2] = (byte)((v1 + v2) / 2);
				dst[BigEndian ? 0 : 3] = y2;

			}
			else
			{
				(byte y, byte u, byte v) = RGB2YUV(src);

				dst[BigEndian ? 3 : 0] = u;
				dst[BigEndian ? 2 : 1] = y;
				dst[BigEndian ? 1 : 2] = v;
				dst[BigEndian ? 0 : 3] = y;
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
    }
}