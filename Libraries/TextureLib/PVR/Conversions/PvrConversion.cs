using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.IO;

namespace TextureLib
{
	public partial class PvrTexture
	{
		/// <summary>Analyze a Bitmap and determine the optimal (lossy) PVR pixel format based on the bitmap's transparency levels.</summary>
		public static PvrPixelFormat AutoPvrPixelFormatFromImage(System.Drawing.Bitmap image)
		{
			PvrPixelFormat targetPixelFormat;
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(image);
			switch (alphaLevel)
			{
				case BitmapAlphaLevel.None:
					targetPixelFormat = PvrPixelFormat.Rgb565;
					break;
				case BitmapAlphaLevel.OneBitAlpha:
					targetPixelFormat = PvrPixelFormat.Argb1555;
					break;
				case BitmapAlphaLevel.FullAlpha:
				default:
					targetPixelFormat = PvrPixelFormat.Argb4444;
					break;
			}
			return targetPixelFormat;
		}

		/// <summary>Analyze a Bitmap and determine the optimal PVR data format based on the bitmap's dimensions, optionally with VQ and mipmapped formats.</summary>
		public static PvrDataFormat AutoPvrDataFormatFromImage(System.Drawing.Bitmap image, bool mipmaps, bool vq = false)
		{
			PvrDataFormat targetDataFormat;
			// Rectangular
			if (image.Width != image.Height)
				targetDataFormat = PvrDataFormat.RectangleTwiddled;
			// Square
			else
			{
				// Using VQ
				if (vq)
				{
					// If image is smaller than 32x32, use SmallVQ
					if (image.Width <= 32)
						targetDataFormat = mipmaps ? PvrDataFormat.SmallVqMipmaps : PvrDataFormat.SmallVq;
					// Otherwise use regular VQ
					else
						targetDataFormat = mipmaps ? PvrDataFormat.VqMipmaps : PvrDataFormat.Vq;
				}
				// Not using VQ
				else
					targetDataFormat = mipmaps ? PvrDataFormat.SquareTwiddledMipmaps : PvrDataFormat.SquareTwiddled;
			}
			return targetDataFormat;
		}

		public override void AddMipmaps()
		{
			// If the texture already has mipmaps, don't do anything
			if (HasMipmaps)
				return;
			// If the texture is not square, don't do anything
			if (Width != Height)
				return;
			bool reencode = false; // Whether the texture should be reencoded or not
			 // Set the format to the mipmapped version
			switch (PvrDataFormat)
			{
				// Some "mipmapped" formats don't actually support mipmaps, so you can't add to them
				case PvrDataFormat.Bitmap:
					return;
				// Rectangular formats can be converted to SquareTwiddledMipmaps if the actual texture is square
				case PvrDataFormat.Rectangle:
				case PvrDataFormat.RectangleStride:
				case PvrDataFormat.RectangleTwiddled:
					PvrDataFormat = PvrDataFormat.SquareTwiddledMipmaps;
					reencode = true;
					break;
				case PvrDataFormat.SquareTwiddled:
					PvrDataFormat = PvrDataFormat.SquareTwiddledMipmaps;
					break;
				case PvrDataFormat.Index4:
					PvrDataFormat = PvrDataFormat.Index4Mipmaps;
					break;
				case PvrDataFormat.Index8:
					PvrDataFormat = PvrDataFormat.Index8Mipmaps;
					break;
				case PvrDataFormat.Vq:
					PvrDataFormat = PvrDataFormat.VqMipmaps;
					reencode = true; // Adding mipmaps to a VQ texture triggers a reencoding, sorry :(
					break;
				case PvrDataFormat.SmallVq:
					PvrDataFormat = PvrDataFormat.SmallVqMipmaps;
					reencode = true;
					break;
			}
			HasMipmaps = true;
			MemoryStream outputStream = new();
			// Set codecs
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(PvrPixelFormat);
			PvrDataCodec dataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
			// Calculate texture data size
			int dataSize = dataCodec.CalculateTextureSize(Width, Height);
			// If the texture already has mipmaps, add their offsets to get the correct location of the original texture (this is probably also unnecessary)
			if (dataCodec.HasMipmaps)
			{
				for (int size = 1; size < Width; size <<= 1)
				{
					dataSize += dataCodec.CalculateTextureSize(size, size);
				}
			}
			int mipLevels = 1; // Keep track of how many mipmaps were generated
			// Get the original texture address
			int textureAddress = HeaderlessData.Length - dataCodec.CalculateTextureSize(Width, Height);
			// Retrieve the original texture data
			byte[] originalTexture = new byte[dataCodec.CalculateTextureSize(Width, Height)];
			Array.Copy(HeaderlessData, textureAddress, originalTexture, 0, originalTexture.Length);
			// If the texture needs to be re-encoded, that's it
			if (reencode)
			{
				Encode();
				Decode(); // To regenerate MipmapImages
				return;
			}
			// Add mipmaps to an Indexed texture
			else if (Indexed)
			{
				PaletteQuantizer quantizer = TexturePalette.CreatePaletteQuantizer(Palette, Palette.GetNumColors(), 0, useDithering);
				// PVR mipmap order: from smallest to largest
				for (int size = 1; size < Image.Width; size <<= 1)
				{
					TextureFunctions.EncodeMipMap(TextureFunctions.BitmapToImageSharp(Image), quantizer.CreatePixelSpecificQuantizer<Rgba32>(Configuration.Default), dataCodec, size, outputStream);
					mipLevels++;
				}
			}
			// Add mipmaps to a regular texture
			else
			{
				// First, decode the original texture
				ReadOnlySpan<byte> textureData = HeaderlessData[textureAddress..];
				byte[] decodedTexture = dataCodec.Decode(textureData, Width, Height, null);
				SixLabors.ImageSharp.Image<Rgba32> orginalTextureImage = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(decodedTexture, Width, Height);
				// Encode mipmaps. PVR mipmap order: from smallest to largest
				for (int size = 1; size < orginalTextureImage.Width; size <<= 1)
				{
					TextureFunctions.EncodeMipMap(orginalTextureImage, null, dataCodec, size, outputStream);
					mipLevels++;
				}
			}
			// Update mipmap preview images (for more accurate images need to call Decode() again)
			MipmapImages = new System.Drawing.Bitmap[mipLevels];
			int mipSize = Image.Width;
			for (int i = 0; i < mipLevels; i++)
			{
				MipmapImages[i] = new System.Drawing.Bitmap(Image, mipSize, mipSize);
				mipSize >>= 1;
			}
			// Write the original texture
			outputStream.Write(originalTexture);
			// Update raw data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		public override void RemoveMipmaps()
		{
			// If the texture already doesn't have mipmaps, don't do anything
			if (!HasMipmaps)
				return;
			// Set the format to the non-mipmapped version
			switch (PvrDataFormat)
			{
				// Some "mipmapped" formats don't actually support mipmaps, so you can't remove them
				case PvrDataFormat.RectangleMipmaps:
					PvrDataFormat = PvrDataFormat.Rectangle;
					return;
				case PvrDataFormat.RectangleStrideMipmaps:
					PvrDataFormat = PvrDataFormat.RectangleStride;
					return;
				case PvrDataFormat.BitmapMipmaps:
					PvrDataFormat = PvrDataFormat.Bitmap;
					return;
				case PvrDataFormat.SquareTwiddledMipmaps:
				case PvrDataFormat.SquareTwiddledMipmapsDma:
					PvrDataFormat = PvrDataFormat.SquareTwiddled;
					break;
				case PvrDataFormat.Index4Mipmaps:
					PvrDataFormat = PvrDataFormat.Index4;
					break;
				case PvrDataFormat.Index8Mipmaps:
					PvrDataFormat = PvrDataFormat.Index8;
					break;
				case PvrDataFormat.VqMipmaps:
					PvrDataFormat = PvrDataFormat.Vq;
					break;
				case PvrDataFormat.SmallVqMipmaps:
					PvrDataFormat = PvrDataFormat.SmallVq;
					break;
			}
			HasMipmaps = false;
			MemoryStream outputStream = new();
			// Set codecs
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(PvrPixelFormat);
			PvrDataCodec dataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
			// Calculate texture data size
			int dataSize = dataCodec.CalculateTextureSize(Width, Height);
			// Calculate VQ codebook size, if present
			byte[] originalCodebook = new byte[0];
			int paletteEntries = dataCodec.GetPaletteEntries(Width);
			if (paletteEntries > 0 && !dataCodec.NeedsExternalPalette)
			{
				dataSize += paletteEntries / pixelCodec.Pixels * pixelCodec.BytesPerPixel;
				originalCodebook = new byte[paletteEntries / pixelCodec.Pixels * pixelCodec.BytesPerPixel];
				Array.Copy(HeaderlessData, 0, originalCodebook, 0, originalCodebook.Length);
			}				
			// Add mipmap offsets to get the correct location of the original texture
			if (dataCodec.HasMipmaps)
			{
				for (int size = 1; size < Width; size <<= 1)
				{
					dataSize += dataCodec.CalculateTextureSize(size, size);
				}
			}
			// Get the original texture address
			int textureAddress = HeaderlessData.Length - dataCodec.CalculateTextureSize(Width, Height);
			// Retrieve the original texture data
			byte[] originalTexture = new byte[dataCodec.CalculateTextureSize(Width, Height)];
			Array.Copy(HeaderlessData, textureAddress, originalTexture, 0, originalTexture.Length);
			// Write the original codebook, if present
			if (originalCodebook.Length > 0)
				outputStream.Write(originalCodebook);
			// Write the original texture
			outputStream.Write(originalTexture);
			// Update raw data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		private Span<byte> ApplyPaletteRaw(byte[] src, int width, int height, bool index8, bool bigEndian)
		{			
			byte[] result = new byte[src.Length * 4];
			if (Palette == null)
				Palette = TexturePalette.CreateDefaultPalette(index8);
			for (int pixelID = 0; pixelID < width * height; pixelID++)
			{
				//Console.WriteLine("P "+pixelID.ToString());
				byte decodedID = src[pixelID];
				if (!index8)
				{
					decodedID = (byte)((decodedID) >> 4);
				}
				//Console.WriteLine("A " + decodedID.ToString("X"));
				result[pixelID * 4] = Palette.RawData[bigEndian ? decodedID * 2 + 1 : decodedID * 2];
				result[pixelID * 4 + 1] = Palette.RawData[bigEndian ? decodedID * 2 : decodedID * 2 + 1];
			}
			return result;
		}
	}
}