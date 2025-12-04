using System;
using System.Drawing;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

// PVR encoding methods that were split out of the main .cs file.

namespace TextureLib
{
    public partial class PvrTexture
    {
		public override void Encode()
		{
			// Determine the pixel and data codecs
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(PvrPixelFormat);
			PvrDataCodec dataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);

			// Check texture dimensions
			if (!dataCodec.CheckDimensionsValid(Width, Height))
			{
				throw new InvalidOperationException($"The dimensions ({Width}x{Height}) of the specified image are not valid for data format {PvrDataFormat}.");
			}

			MemoryStream outputStream = new();

			// Check whether to use a generated or specified palette
			bool autoPalette = Palette == null;
			
			// VQ encoder
			if (dataCodec is VqDataCodec)
			{
				EncodeVQ(Image, dataCodec, outputStream);
			}
			// Regular encoder
			else if (dataCodec.GetPaletteEntries(Width) == 0)
			{
				EncodeColored(Image, dataCodec, outputStream);
			}
			// Indexed encoder
			else
			{
				int indexRange = dataCodec.GetPaletteEntries(Image.Width);
				bool index8 = dataCodec is Index8DataCodec or Index8MipmapsDataCodec;
				PvrDataCodec qDataCodec = PvrDataCodec.Create(PvrDataFormat.Rectangle, pixelCodec);
				// If the user hasn't specified a palette, get one from the quantized bitmap
				if (autoPalette)
				{
					// Quantize the image using the default quantizer to get a palette (ignore the output bitmap data because only the palette is needed)
					TextureFunctions.QuantizeImage(TextureFunctions.CalculateLossyForPaletteOrVq(Image, qDataCodec), index8, out Palette, useDithering);
					// Sort the palette by luminance
					Palette.SortByLuminance();
				}
				// Quantize the image using the specific quantizer created from the palette (ignore the output palette data because there's a palette already)
				byte[] indexedBitmapData = TextureFunctions.QuantizeImage(TextureFunctions.CalculateLossyForPaletteOrVq(Image, qDataCodec), index8, out _, useDithering, Palette);
				// If the palette was created from the image (as opposed to user specified), encode it with the specified pixel codec
				if (autoPalette)
					Palette.Encode(pixelCodec, false);
				// If the palette is embedded, write it to texture data
				if (!dataCodec.NeedsExternalPalette)
				{
					EncodePalette(Palette, indexRange, dataCodec.PixelCodec, outputStream);
				}
				// If the texture has mipmaps, write them first
				if (dataCodec.HasMipmaps)
				{
					PaletteQuantizer quantizer = TexturePalette.CreatePaletteQuantizer(Palette, Palette.GetNumColors(), 0, useDithering);
					// PVR mipmap order: from smallest to largest
					for (int size = 1; size < Image.Width; size <<= 1)
					{
						TextureFunctions.EncodeMipMap(TextureFunctions.BitmapToImageSharp(Image), quantizer.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default), dataCodec, size, outputStream);
					}
				}
				// Encode the indexed texture itself
				outputStream.Write(dataCodec.Encode(indexedBitmapData, Image.Width, Image.Height));
			}
			// Set the texture's raw data
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		private static void EncodeColored(Bitmap texture, PvrDataCodec dataCodec, MemoryStream writer)
        {
            if (dataCodec.HasMipmaps)
            {
                Image<Rgba32> image = TextureFunctions.BitmapToImageSharp(texture);
				// PVR mipmap order: from smallest to largest
				for (int size = 1; size < texture.Width; size <<= 1)
				{
					TextureFunctions.EncodeMipMap(image, null, dataCodec, size, writer);
				}
            }

            byte[] mainImageData = dataCodec.Encode(TextureFunctions.BitmapToRaw(texture), texture.Width, texture.Height);
            writer.Write(mainImageData);
        }

        private static void EncodePalette(TexturePalette palette, int paletteEntries, PixelCodec pixelCodec, MemoryStream writer)
        {
            ReadOnlySpan<byte> paletteColors = palette.RawData;

            Span<byte> destination = new byte[pixelCodec.BytesPerPixel * paletteEntries / pixelCodec.Pixels];
            for (int i = 0; i < paletteEntries; i += pixelCodec.Pixels)
            {
                pixelCodec.EncodePixel(paletteColors[(i * 4)..], destination[(i * pixelCodec.BytesPerPixel)..], false);
            }

            writer.Write(destination);
        }

        private static void EncodeVQ(Bitmap texture, PvrDataCodec dataCodec, MemoryStream writer)
        {
            if (!dataCodec.CheckDimensionsValid(texture.Width, texture.Height))
            {
				throw new InvalidOperationException($"The dimensions ({texture.Width}x{texture.Height}) of the specified image are not valid for data codec {dataCodec}.");
			}

            PixelCodec pixelCodec = dataCodec.PixelCodec;
            if (pixelCodec is not ARGB8888PixelCodec)
            {
				PvrDataCodec qDataCodec = PvrDataCodec.Create(PvrDataFormat.Rectangle, pixelCodec);
				texture = TextureFunctions.CalculateLossyForPaletteOrVq(texture, qDataCodec);
            }

            int evalDataLength = texture.Width * texture.Height * 4;

            int textureCount = 1;
            if (dataCodec.HasMipmaps)
            {
                textureCount = (int)Math.Log(texture.Width, 2);

                int curLength = evalDataLength >> 2;
                for (int i = textureCount - 1; i > 0; i--, curLength >>= 2)
                {
                    evalDataLength += curLength;
                }
            }

            Span<byte> evalData = new byte[evalDataLength];
            Image<Rgba32> image = TextureFunctions.BitmapToImageSharp(texture);
            int destinationAddress = 0;

            for (int i = 0; i < textureCount; i++)
            {
                Span<byte> imageBuffer = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(imageBuffer);

                // next we remap it so that a VQ superpixel is a 16 byte row
                int rowSize = image.Width * 4;
                for (int y = 0; y < image.Height; y += 2)
                {
                    for (int x = 0; x < image.Width; x += 2)
                    {
                        int addr = (x * 4) + (y * rowSize);
                        imageBuffer.Slice(addr, 4).CopyTo(evalData[destinationAddress..]);
                        imageBuffer.Slice(addr + rowSize, 4).CopyTo(evalData[(destinationAddress + 4)..]);
                        imageBuffer.Slice(addr + 4, 4).CopyTo(evalData[(destinationAddress + 8)..]);
                        imageBuffer.Slice(addr + 4 + rowSize, 4).CopyTo(evalData[(destinationAddress + 12)..]);
                        destinationAddress += 16;
                    }
                }

                image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
            }

            // now quantize the data
            int superPixelCount = dataCodec.GetPaletteEntries(texture.Width) / 4;
            (int[] indices, byte[] clusters) = VectorQuantization.QuantizeByteData(evalData, 16, superPixelCount);

            // the palette
            uint paletteSize = (uint)(dataCodec.GetPaletteEntries(texture.Width) / pixelCodec.Pixels * pixelCodec.BytesPerPixel);

            Span<byte> palette = new byte[paletteSize];
            int paletteIndex = 0;
            for (int i = 0; i < clusters.Length; i += 4 * pixelCodec.Pixels)
            {
                pixelCodec.EncodePixel(clusters[i..], palette[paletteIndex..], false);
                paletteIndex += pixelCodec.BytesPerPixel;
            }

            writer.Write(palette);

            // The indices are basically our texture, so just write those.
            // But if we have mipmaps, then we need to write it in "reverse"

            Span<byte> textures = indices.Select(x => (byte)x).ToArray();

            if (textureCount > 1)
            {
                int mipmapAddr = textures.Length - 1;

                // the 1x1 mipmap is essentially just a 2x2 mipmap, which is at the last pixel
                writer.Write(dataCodec.Encode(textures[mipmapAddr..], 1, 1));

                int size = 2;
                for (; mipmapAddr > 0; mipmapAddr -= size * size, size <<= 1)
                {
                    writer.Write(dataCodec.Encode(textures[mipmapAddr..], size, size));
                }
            }

            writer.Write(dataCodec.Encode(textures, texture.Width, texture.Height));
        }
	}
}