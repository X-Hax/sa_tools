using System;

namespace TextureLib
{
    internal class BitmapDataCodec : RectangleDataCodec
    {
        public BitmapDataCodec(PixelCodec pixelCodec) : base(new ARGB8888PixelCodec() { ColorOrder = ARGB8888PixelCodec.ColorOrderType.ABGRBitmap }) { }

        // Like Rectangle but with Bitmap ARGB8888
        protected override void InternalDecode(ReadOnlySpan<byte> source, int width, int height, ReadOnlySpan<byte> palette, Span<byte> destination)
        {
            int srcAddress = 0;
            int dstAddress = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x += PixelCodec.Pixels)
                {
                    PixelCodec.DecodePixel(
                        source[srcAddress..],
                        destination[dstAddress..]);

                    srcAddress += PixelCodec.BytesPerPixel;
                    dstAddress += 4 * PixelCodec.Pixels;
                }
            }
        }
    }
}