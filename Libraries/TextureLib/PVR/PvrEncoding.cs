using System;
using System.Drawing;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
    public partial class PvrTexture
    {
		/// <summary>
		/// Encodes a PVR texture from Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="dataFormat">Target PVR data format.</param>
		/// <param name="pixelFormat">Target PVR pixel format or pixel format for indexed images.</param>
		/// <param name="mipmaps">Encode mipmaps.</param>
		/// <param name="outputPalette">Output palette for indexed images.</param>
		/// <param name="inputPalette">Input palette for indexed images. Used to create indexed textures with a user-specified palette.</param>
		/// <param name="gbix">Global index.</param>
		/// <param name="dither">Use dithering for encoding indexed images.</param>
		/// <param name="paletteExternal">Save the palette to an external file.</param>
		public PvrTexture(Bitmap texture, PvrDataFormat dataFormat, PvrPixelFormat pixelFormat, bool mipmaps, out TexturePalette? outputPalette, TexturePalette inputPalette = null, uint gbix = 0, bool dither = false, bool paletteExternal = false)
		{
			// Disable mipmaps if using incompatible texture encoder settings
			if (mipmaps)
			{
				if (texture.Width != texture.Height)
				{
					// TODO: Remove
					Console.WriteLine("Mipmaps disabled because the texture is rectangular");
					mipmaps = false;
				}
			}

			// Set common texture data
			Gbix = gbix;
			PvrDataFormat = dataFormat;
			PvrPixelFormat = pixelFormat;
			Width = texture.Width;
			Height = texture.Height;
			if (mipmaps)
			{
				HasMipmaps = true;
			}
			if (dataFormat == PvrDataFormat.Index8 || dataFormat == PvrDataFormat.Index4 || dataFormat == PvrDataFormat.Index4Mipmaps || dataFormat == PvrDataFormat.Index8Mipmaps)
			{
				Indexed = true;
				RequiresPaletteFile = paletteExternal;
			}

			// Determine the pixel and data codecs
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(pixelFormat);
			PvrDataCodec dataCodec = PvrDataCodec.Create(dataFormat, pixelCodec);

			// Check texture dimensions
			if (!dataCodec.CheckDimensionsValid(Width, Height))
			{
				throw new InvalidOperationException($"The dimensions ({Width}x{Height}) of the specified image are not valid for data format {PvrDataFormat}.");
			}

			MemoryStream outputStream = new();
			outputPalette = null;

			// If the user has specified a palette, use it
			if (inputPalette != null)
				outputPalette = inputPalette;

			if (dataCodec is VqDataCodec)
			{
				EncodeVQ(texture, dataCodec, outputStream);
			}
			else if (dataCodec.GetPaletteEntries(Width) == 0)
			{
				EncodeColored(texture, dataCodec, outputStream);
			}
			else
			{
				EncodeIndexed(texture, dataCodec, dither, outputStream, out outputPalette);
			}
			// Set the texture's raw data
			RawData = outputStream.ToArray();
		}

		internal static Bitmap CalculateLossyForPaletteOrVq(Bitmap texture, PixelCodec pixelCodec)
		{
			PvrDataCodec dataCodec = PvrDataCodec.Create(PvrDataFormat.Rectangle, pixelCodec);
			byte[] encoded = dataCodec.Encode(TextureFunctions.BitmapToRaw(texture), texture.Width, texture.Height);
			byte[] decoded = dataCodec.Decode(encoded, texture.Width, texture.Height, null);
			Bitmap output = new Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			TextureFunctions.RawToBitmap(output, decoded);
			return output;
		}

        private static void EncodeIndexed(Bitmap texture, PvrDataCodec dataCodec, bool dither, MemoryStream writer, out TexturePalette palette)
        {
			palette = null;
			/*
            bool index8 = dataCodec is Index8DataCodec or Index8MipmapsDataCodec;

            int indexRange = dataCodec.GetPaletteEntries(texture.Width);

            if (texture is not IndexTexture indexTexture)
            {
                indexTexture = CalculateLossyForPaletteOrVq(texture, dataCodec.PixelCodec).Palettize(index8, dither);
            }

            palette = indexTexture.Palette ?? TexturePalette.CreateDefaultPalette(index8);

            if (!dataCodec.NeedsExternalPalette)
            {
                EncodePalette(palette, indexRange, dataCodec.PixelCodec, writer);
            }

            if (dataCodec.HasMipmaps)
            {
                PaletteQuantizer quantizer = palette.CreatePaletteQuantizer(indexRange, indexRange * indexTexture.PaletteRow, dither);
                EncodeMipMaps(indexTexture.ToImageSharp(), quantizer.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default), dataCodec, writer);
            }

            writer.Write(dataCodec.Encode(indexTexture.Data, texture.Width, texture.Height));
			*/
        }

        private static void EncodeColored(Bitmap texture, PvrDataCodec dataCodec, MemoryStream writer)
        {
            if (dataCodec.HasMipmaps)
            {
                Image<Rgba32> image = TextureFunctions.BitmapToImageSharp(texture);
                EncodeMipMaps(image, null, dataCodec, writer);
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

        private static void EncodeMipMaps(Image<Rgba32> image, IQuantizer<Rgba32>? quantizer, PvrDataCodec dataCodec, MemoryStream writer)
        {
            for (int size = 1; size < image.Width; size <<= 1)
            {
                Image<Rgba32> mipMapImage = image.Clone();
                mipMapImage.Mutate(x => x.Resize(size, size));

                byte[] mipMapPixels;
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
                else
                {
                    mipMapPixels = new byte[size * size * 4];
                    mipMapImage.CopyPixelDataTo(mipMapPixels);
                }

                writer.Write(dataCodec.Encode(mipMapPixels, size, size));
            }
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
                texture = CalculateLossyForPaletteOrVq(texture, pixelCodec);
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