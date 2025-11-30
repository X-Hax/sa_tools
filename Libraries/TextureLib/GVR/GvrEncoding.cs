using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Drawing;
using System.IO;

namespace TextureLib
{
    internal class GvrEncoding
    {
        // Encodes the bitmap with the specified pixel codec, then decodes it back with degraded colors for the base of the indexed image.
        public static Bitmap CalculateLossyForPalette(Bitmap texture, GvrPixelCodec gvrPaletteCodec)
        {
            byte[] encoded = gvrPaletteCodec.Encode(TextureFunctions.BitmapToRaw(texture), texture.Width, texture.Height);
            byte[] decoded = gvrPaletteCodec.Decode(encoded, texture.Width, texture.Height);
            Bitmap bitmap = new Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            TextureFunctions.RawToBitmap(bitmap, decoded);
            return bitmap;
        }

        public static void EncodeMipMaps(Image<Rgba32> image, IQuantizer<Rgba32>? quantizer, GvrPixelCodec pixelCodec, MemoryStream writer)
        {
            for (int size = image.Width >> 1; size > 0; size >>= 1)
            {
                Image<Rgba32> mipMapImage = image.Clone();
                mipMapImage.Mutate(x => x.Resize(size, size));

                byte[] mipMapPixels;
                // If there's a quantizer, use it
                if (quantizer != null)
                {
                    IndexedImageFrame<Rgba32> mipMapFrame = quantizer.QuantizeFrame(mipMapImage.Frames[0], new(0, 0, size, size));

                    mipMapPixels = new byte[size * size];
                    Span<byte> pixelData = mipMapPixels;

                    for (int y = 0; y < size; y++)
                    {
                        mipMapFrame.DangerousGetRowSpan(y).CopyTo(pixelData[(y * size)..]);
                    }
                }
                // Otherwise just copy
                else
                {
                    mipMapPixels = new byte[size * size * 4];
                    Span<byte> pixelData = mipMapPixels;
                    for (int y = 0; y < size; y++)
                    {
                        mipMapImage.Frames[0].CopyPixelDataTo(pixelData);
                    }
                    TextureFunctions.RGBAtoBGRA(mipMapPixels);
                }
                writer.Write(pixelCodec.Encode(mipMapPixels, size, size));
            }
        }
    }
}