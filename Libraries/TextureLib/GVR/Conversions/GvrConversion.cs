using System;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
	public partial class GvrTexture
	{
		public static GvrDataFormat AutoGvrDataFormatFromPvr(PvrPixelFormat pixelFormat, bool maxQuality = false)
		{
			switch (pixelFormat)
			{
				case PvrPixelFormat.Argb1555:
				case PvrPixelFormat.Argb4444:
					return maxQuality ? GvrDataFormat.Argb8888 : GvrDataFormat.Rgb5a3;
				case PvrPixelFormat.Rgb555:
				case PvrPixelFormat.Rgb565:
					return GvrDataFormat.Rgb565;
				case PvrPixelFormat.Argb8888Alt:
				case PvrPixelFormat.Argb8888orYUV420:
					return GvrDataFormat.Argb8888;
				case PvrPixelFormat.Yuv422:
				case PvrPixelFormat.Bump88:
				default:
					return maxQuality ? GvrDataFormat.Argb8888 : GvrDataFormat.Rgb5a3;
			}
		}

		public static GvrDataFormat AutoGvrDataFormatFromDds(DdsFormat ddsFormat, bool maxQuality = false)
		{
			switch (ddsFormat)
			{
				case DdsFormat.Argb1555:
				case DdsFormat.Argb4444:
					return maxQuality ? GvrDataFormat.Argb8888 : GvrDataFormat.Rgb5a3;
				case DdsFormat.Dxt1:
					return GvrDataFormat.Dxt1;
				case DdsFormat.Rgb565:
					return GvrDataFormat.Rgb565;
				case DdsFormat.Argb8888:
					return GvrDataFormat.Argb8888;
				case DdsFormat.Rgb888:
					return maxQuality ? GvrDataFormat.Argb8888 : GvrDataFormat.Rgb565;
				case DdsFormat.Dxt3:
				case DdsFormat.Dxt5:
				default:
					return maxQuality ? GvrDataFormat.Argb8888 : GvrDataFormat.Rgb5a3;
			}
		}

		public static GvrDataFormat AutoGvrDataFormatFromImage(System.Drawing.Bitmap image, bool useDxt = false, bool maxQuality = false)
		{
			GvrDataFormat targetDataFormat;
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(image);
			switch (alphaLevel)
			{
				case BitmapAlphaLevel.None:
					if (maxQuality)
						targetDataFormat = GvrDataFormat.Argb8888;
					else
						targetDataFormat = useDxt ? GvrDataFormat.Dxt1 : GvrDataFormat.Rgb565;
					break;
				case BitmapAlphaLevel.OneBitAlpha:
					if (maxQuality)
						targetDataFormat = GvrDataFormat.Argb8888;
					else
						targetDataFormat = useDxt ? GvrDataFormat.Dxt1 : GvrDataFormat.Rgb5a3;
					break;
				case BitmapAlphaLevel.FullAlpha:
				default:
					targetDataFormat = maxQuality ? GvrDataFormat.Argb8888 : GvrDataFormat.Rgb5a3;
					break;
			}
			return targetDataFormat;
		}

		public override void AddMipmaps()
		{
			// If the texture already has mipmaps, don't do anything
			if (HasMipmaps)
				return;
			int currentOffset = 0;
			MemoryStream outputStream = new MemoryStream();
			// Set data codec
			GvrDataCodec dataCodec = GvrDataCodec.GetDataCodec(GvrDataFormat);
			// Write the texture's CLUT (internal palette) if it exists
			if (dataCodec.PaletteEntries != 0 && GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
			{
				PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, false);
				int paletteSize = paletteCodec.BytesPerPixel * dataCodec.PaletteEntries;
				byte[] paletteData = new byte[paletteSize];
				Array.Copy(HeaderlessData, currentOffset, paletteData, 0, paletteSize);
				currentOffset += paletteSize;
				outputStream.Write(paletteData);
			}
			// Find the original texture
			byte[] originalTexture = new byte[dataCodec.CalculateTextureSize(Width, Height)];
			Array.Copy(HeaderlessData, currentOffset, originalTexture, 0, originalTexture.Length);
			// Write the original texture
			outputStream.Write(originalTexture);
			// Get original texture raw data
			byte[] encodedBytes = new byte[Image.Width * Image.Height * 4];
			TextureFunctions.BitmapToRaw(Image, encodedBytes);
			// Write mipmaps for regular textures
			if (!Indexed)
			{
				// GVR mipmap order: from largest to smallest
				for (int size = Image.Width >> 1; size > 0; size >>= 1)
				{
					TextureFunctions.EncodeMipMap(SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(encodedBytes, Image.Width, Image.Height), null, dataCodec, size, outputStream);
				}
			}
			// Write mipmaps for Indexed texture
			else
			{
				PaletteQuantizer quantizer = TexturePalette.CreatePaletteQuantizer(Palette, Palette.GetNumColors(), 0, useDithering);
				// GVR mipmap order: from largest to smallest
				for (int size = Image.Width >> 1; size > 0; size >>= 1)
				{
					TextureFunctions.EncodeMipMap(TextureFunctions.BitmapToImageSharp(Image), quantizer.CreatePixelSpecificQuantizer<Rgba32>(SixLabors.ImageSharp.Configuration.Default), dataCodec, size, outputStream);
				}
			}
			// Update flags
			HasMipmaps = true;
			if (!GvrDataFlags.HasFlag(GvrDataFlags.Mipmaps))
				GvrDataFlags |= GvrDataFlags.Mipmaps;
			// Update raw data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		public override void RemoveMipmaps()
		{
			// If the texture doesn't have mipmaps, don't do anything
			if (!HasMipmaps)
				return;
			int currentOffset = 0;
			MemoryStream outputStream = new MemoryStream();
			// Set data codec
			GvrDataCodec dataCodec = GvrDataCodec.GetDataCodec(GvrDataFormat);
			// Write the texture's CLUT (internal palette) if it exists
			if (dataCodec.PaletteEntries != 0 && GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
			{
				PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, false);
				int paletteSize = paletteCodec.BytesPerPixel * dataCodec.PaletteEntries;
				byte[] paletteData = new byte[paletteSize];
				Array.Copy(HeaderlessData, currentOffset, paletteData, 0, paletteSize);
				currentOffset += paletteSize;
				outputStream.Write(paletteData);
			}	 
			// Find the original texture
			byte[] originalTexture = new byte[dataCodec.CalculateTextureSize(Width, Height)];
			Array.Copy(HeaderlessData, currentOffset, originalTexture, 0, originalTexture.Length);
			// Write the original texture
			outputStream.Write(originalTexture);
			// Update flags
			HasMipmaps = false;
			if (GvrDataFlags.HasFlag(GvrDataFlags.Mipmaps))
				GvrDataFlags &= ~GvrDataFlags.Mipmaps;
			// Update raw data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		/// <summary>Removes an Indexed GVR texture's CLUT (internal palette), outputting it as a TexturePalette.</summary>
		/// <param name="palette">CLUT output as a palette.</param>
		public void RemoveClut(out TexturePalette palette)
		{
			palette = Palette;
			if (!Indexed || !GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
				return;
			int currentOffset = 0;
			// Set data codec
			GvrDataCodec dataCodec = GvrDataCodec.GetDataCodec(GvrDataFormat);
			// Get around the texture's CLUT
			PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, false);
			int paletteSize = paletteCodec.BytesPerPixel * dataCodec.PaletteEntries;
			currentOffset += paletteSize;
			// Get data without CLUT
			byte[] dataWithoutClut = new byte[HeaderlessData.Length - paletteSize];
			Array.Copy(HeaderlessData, currentOffset, dataWithoutClut, 0, dataWithoutClut.Length);
			HeaderlessData = dataWithoutClut;
			GvrDataFlags &= ~GvrDataFlags.InternalPalette;
			GvrDataFlags |= GvrDataFlags.ExternalPalette;
			RawData = GetBytes();
		}

		/// <summary>Converts an Indexed GVR texture with an external palette to a texture with a built-in CLUT (internal palette).</summary>
		/// <param name="palette">Palette to convert to CLUT.</param>
		public void AddClut(TexturePalette palette)
		{
			if (!Indexed || !GvrDataFlags.HasFlag(GvrDataFlags.ExternalPalette))
				return;
			MemoryStream outputStream = new();
			Palette = palette;
			GvrDataCodec dataCodec = GvrDataCodec.GetDataCodec(GvrDataFormat);
			PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, useSACompatiblePalette);
			int paletteSize = paletteCodec.BytesPerPixel * dataCodec.PaletteEntries;
			byte[] paletteData = new byte[paletteSize];
			Array.Copy(Palette.RawData, 0, paletteData, 0, Palette.RawData.Length);
			GvrDataFlags &= ~GvrDataFlags.ExternalPalette;
			GvrDataFlags |= GvrDataFlags.InternalPalette;
			outputStream.Write(paletteData);
			outputStream.Write(HeaderlessData);
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}
	}
}