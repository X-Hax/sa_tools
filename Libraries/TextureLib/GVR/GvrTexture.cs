using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
	// Class for Gamecube and Wii GVR textures
	public class GvrTexture : GenericTexture
	{
		const uint Magic_GBIX = 0x58494247;
		const uint Magic_GCIX = 0x58494347;
		const uint Magic_GVRT = 0x54525647;

		public GvrPaletteFormat GvrPaletteFormat;
		public GvrDataFormat GvrDataFormat;
		private GvrDataFlags GvrDataFlags;
		private byte[] HeaderlessData;

		public override byte[] GetBytes()
		{
			ByteConverter.SetBigEndian(true);
			List<byte> result = new();
			if (Gbix != 0)
			{
				result.AddRange(BitConverter.GetBytes(Magic_GBIX));
				result.AddRange(BitConverter.GetBytes((uint)8));
				result.AddRange(ByteConverter.GetBytes(Gbix));
				result.AddRange(ByteConverter.GetBytes((uint)0));
			}
			result.AddRange(BitConverter.GetBytes(Magic_GVRT));
			result.AddRange(BitConverter.GetBytes((uint)(RawData.Length + 8)));
			result.Add((byte)PaletteBank);
			result.Add((byte)PaletteStartIndex);
			int flag = ((byte)GvrPaletteFormat << 4) | (byte)GvrDataFlags;
			result.Add((byte)flag);
			result.Add((byte)GvrDataFormat);
			result.AddRange(ByteConverter.GetBytes((ushort)Width));
			result.AddRange(ByteConverter.GetBytes((ushort)Height));
			result.AddRange(RawData);
			ByteConverter.RestoreBigEndian();
			return result.ToArray();
		}

		/// <summary>
		/// Initializes a GVR texture from a byte array that contains GVR texture header and data.
		/// </summary>
		/// <param name="data">Byte array containing GVR texture header and data.</param>
		/// <param name="offset">Offset to the beginning of the GVR texture header.</param>
		/// <param name="name">Texture name, if applicable.</param>
		/// <param name="extPalette">Texture palette for decoding indexed textures, if applicable</param>
		public GvrTexture(byte[] data, int offset = 0, string name = null, TexturePalette extPalette = null)
		{
			ByteConverter.SetBigEndian(true);
			InitTexture(data, offset, name);
			int currentOffset = offset;

			// Get global index
			if (BitConverter.ToUInt32(RawData, offset) == Magic_GBIX || BitConverter.ToUInt32(RawData, offset) == Magic_GCIX)
			{
				Gbix = ByteConverter.ToUInt32(RawData, currentOffset + 0x8);
				currentOffset += 0x10;
			}

			// Parse header
			if (BitConverter.ToUInt32(RawData, currentOffset) == Magic_GVRT)
			{
				int chunksize = ByteConverter.ToInt32(RawData, currentOffset + 0x4);
				PaletteBank = RawData[currentOffset + 0x8]; // Not confirmed
				PaletteStartIndex = RawData[currentOffset + 0x9]; // Not confirmed
				GvrPaletteFormat = (GvrPaletteFormat)(RawData[currentOffset + 0x0A] >> 4); // Only the first 4 bits matter
				GvrDataFlags = (GvrDataFlags)(RawData[currentOffset + 0x0A] & 0x0F); // Only the last 4 bits matter
				GvrDataFormat = (GvrDataFormat)RawData[currentOffset + 0x0B];
				Width = ByteConverter.ToUInt16(RawData, currentOffset + 0xC);
				Height = ByteConverter.ToUInt16(RawData, currentOffset + 0xE);
			}

			// Set general texture properties
			switch (GvrDataFormat)
			{
				case GvrDataFormat.Index4:
					Indexed = true;
					break;
				case GvrDataFormat.Index8:
					Indexed = true;
					break;
				default:
					break;
			}

			// Parse GVR data flags and set general texture properties
			if (GvrDataFlags.HasFlag(GvrDataFlags.Mipmaps))
				HasMipmaps = true;
			if (GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette) && (GvrDataFormat == GvrDataFormat.Index4 || GvrDataFormat == GvrDataFormat.Index8 || GvrDataFormat == GvrDataFormat.Index14))
			{
				// Sometimes the "internal palette" flag is there even when the texture isn't indexed...
				Indexed = true;
			}
			if (GvrDataFlags.HasFlag(GvrDataFlags.ExternalPalette))
			{
				Indexed = true;
				RequiresPaletteFile = true;
			}

			// Get codec
			currentOffset += 0x10;
			GvrPixelCodec pixelCodec = GvrPixelCodec.GetPixelCodec(GvrDataFormat);
#if DEBUG
			Console.WriteLine("\nTEXTURE INFO");
			Console.WriteLine("Width: " + Width.ToString());
			Console.WriteLine("Height: " + Height.ToString());
			Console.WriteLine("Gbix: " + Gbix.ToString());
			Console.WriteLine("Data format: " + GvrDataFormat.ToString());
			Console.WriteLine("Palette format: " + (RequiresPaletteFile ? "External" : GvrPaletteFormat.ToString()));
			Console.WriteLine("Mipmaps: " + HasMipmaps.ToString());
			Console.WriteLine("Indexed: " + Indexed.ToString());
			Console.WriteLine("Requires palette file: " + RequiresPaletteFile.ToString());
#endif
			// Calculate the expected size of the texture data chunk
			int dataSize = pixelCodec.CalculateTextureSize(Width, Height);

			// Decode the internal palette
			if (pixelCodec.PaletteEntries != 0 && GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
			{
				PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, false);
				int paletteSize = paletteCodec.BytesPerPixel * pixelCodec.PaletteEntries;
				byte[] paletteData = new byte[paletteSize];
				Array.Copy(data, currentOffset, paletteData, 0, paletteSize);
				currentOffset += paletteSize;
				Palette = new TexturePalette(paletteData, paletteCodec, pixelCodec.PaletteEntries, bigEndian: true);
			}

			// Set external palette
			if (RequiresPaletteFile && extPalette != null)
				Palette = extPalette;

			// Adjust data size for mipmaps
			if (HasMipmaps)
			{
				for (int size = Width >> 1; size > 0; size >>= 1)
				{
					//Console.WriteLine("Mip size {0}", size);
					dataSize += pixelCodec.CalculateTextureSize(size, size);
				}
			}

			// Set up data without header
			HeaderlessData = new byte[dataSize];
			Array.Copy(data, currentOffset, HeaderlessData, 0, dataSize);

			// Decode data
			int textureAddress = HeaderlessData.Length - pixelCodec.CalculateTextureSize(Width, Height);
			ReadOnlySpan<byte> textureData = HeaderlessData[textureAddress..];
			byte[] result = pixelCodec.Decode(HeaderlessData, Width, Height);

			// Apply palette if the texture is indexed
			if (Indexed)
			{
				result = ApplyPalette(result, Width, Height).ToArray();
			}

			// Create preview image
			Image = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
			TextureFunctions.RawToBitmap(Image, result);

			// Create mipmap preview images
			if (HasMipmaps)
			{
				int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
				MipmapImages = new Bitmap[mipLevels];
				for (int mipmapIndex = 0; mipmapIndex < mipLevels; mipmapIndex++)
				{
					int mipOffset = 0;
					int size = Width;
					for (int i = 0; i < mipmapIndex; i++, size >>= 1)
					{
						mipOffset += pixelCodec.CalculateTextureSize(size, size);
					}
					byte[] mipData = HeaderlessData[mipOffset..].ToArray();
					byte[] mipRawData = pixelCodec.Decode(mipData, size, size);
					if (Indexed)
						mipRawData = ApplyPalette(mipRawData, size, size).ToArray();
					Bitmap mipBitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
					TextureFunctions.RawToBitmap(mipBitmap, mipRawData);
					MipmapImages[mipmapIndex] = mipBitmap;
				}
			}

			// Important!
			ByteConverter.RestoreBigEndian();
		}

		/// <summary>
		/// Encodes a GVR texture from Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="dataFormat">Target GVR data format.</param>
		/// <param name="mipmaps">Encode mipmaps.</param>
		/// <param name="outputPalette">Output palette for indexed images.</param>
		/// <param name="inputPalette">Input palette for indexed images. Used to create indexed textures with a user-specified palette.</param>
		/// <param name="gbix">Global index.</param>
		/// <param name="paletteFormat">Pixel format for indexed images.</param>
		/// <param name="dither">Use dithering for encoding indexed images.</param>
		/// <param name="paletteExternal">Save the palette to an external file.</param>
		/// <param name="paletteSACompatible">Use SADX and SA2B compatible palette formats: ARGB1555 instead of IntensityA8 and ARGB4444 instead of RGB5A3.</param>
		public GvrTexture(Bitmap texture, GvrDataFormat dataFormat, bool mipmaps, out TexturePalette? outputPalette, TexturePalette inputPalette = null, uint gbix = 0, GvrPaletteFormat paletteFormat = GvrPaletteFormat.Rgb5A3orArgb4444, bool dither = false, bool paletteExternal = false, bool paletteSACompatible = true)
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
				if (dataFormat == GvrDataFormat.Index4 || dataFormat == GvrDataFormat.Index8 || dataFormat == GvrDataFormat.Index14)
				{
					// TODO: Remove
					Console.WriteLine("Mipmaps disabled because the texture is indexed");
					mipmaps = false;
				}
			}

			// Set common texture data
			Gbix = gbix;
			GvrDataFormat = dataFormat;
			Width = texture.Width;
			Height = texture.Height;
			if (mipmaps)
			{
				HasMipmaps = true;
				GvrDataFlags |= GvrDataFlags.Mipmaps;
			}
			if (dataFormat == GvrDataFormat.Index8 || dataFormat == GvrDataFormat.Index4 || dataFormat == GvrDataFormat.Index14)
			{
				Indexed = true;
				GvrDataFlags |= paletteExternal ? GvrDataFlags.ExternalPalette : GvrDataFlags.InternalPalette;
				RequiresPaletteFile = paletteExternal;
			}
			GvrPaletteFormat = paletteFormat;

			// Determine the pixel codec
			GvrPixelCodec pixelCodec = GvrPixelCodec.GetPixelCodec(dataFormat);

			MemoryStream outputStream = new();
			outputPalette = null;

			// If the user has specified a palette, use it
			if (inputPalette != null)
				outputPalette = inputPalette;

			// Encoding to an indexed format
			if (pixelCodec.PaletteEntries > 0)
			{
				bool index8 = dataFormat == GvrDataFormat.Index8;
				// If the user hasn't specified a palette, get one from the quantized bitmap
				if (outputPalette == null)
				{
					// Quantize the image using the default quantizer to get a palette (ignore the output bitmap data because only the palette is needed)
					TextureFunctions.QuantizeImage(CalculateLossyForPalette(texture, GvrPixelCodec.GetPixelCodecForPalette(paletteFormat)), index8, out outputPalette, dither);
					// Sort the palette by luminance
					outputPalette.SortByLuminance();
				}
				// Quantize the image using the specific quantizer created from the palette (ignore the output palette data because there's a palette already)
				byte[] indexedBitmapData = TextureFunctions.QuantizeImage(CalculateLossyForPalette(texture, GvrPixelCodec.GetPixelCodecForPalette(paletteFormat)), index8, out _, dither, outputPalette);
				// If the palette was created from the image (as opposed to user specified), encode it with the specified pixel codec
				if (inputPalette == null)
					outputPalette.Encode(PixelCodec.GetPixelCodec(paletteFormat, paletteSACompatible), true);
				// If the palette is embedded, write it to texture data
				if (!paletteExternal)
				{
					PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, paletteSACompatible);
					int paletteSize = paletteCodec.BytesPerPixel * pixelCodec.PaletteEntries;
					byte[] paletteData = new byte[paletteSize];
					Array.Copy(outputPalette.RawData, 0, paletteData, 0, outputPalette.RawData.Length);
					outputStream.Write(paletteData);
				}
				// Encode the indexed texture itself
				outputStream.Write(pixelCodec.Encode(indexedBitmapData, texture.Width, texture.Height));
				// Encode mipmaps if specified
				if (mipmaps)
				{
					PaletteQuantizer quantizer = TexturePalette.CreatePaletteQuantizer(outputPalette, outputPalette.GetNumColors(), 0, dither);
					EncodeMipMaps(SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(indexedBitmapData, texture.Width, texture.Height), quantizer.CreatePixelSpecificQuantizer<Rgba32>(SixLabors.ImageSharp.Configuration.Default), pixelCodec, outputStream);
				}
			}
			// Encoding to a non-indexed format
			else
			{
				byte[] encodedBytes = new byte[texture.Width * texture.Height * 4];
				TextureFunctions.BitmapToRaw(texture, encodedBytes);
				outputStream.Write(pixelCodec.Encode(encodedBytes, texture.Width, texture.Height));
				// Encode mipmaps if specified
				if (mipmaps)
				{
					EncodeMipMaps(SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(encodedBytes, texture.Width, texture.Height), null, pixelCodec, outputStream);
				}
			}
			// Set the texture's raw data
			RawData = outputStream.ToArray();
		}

		/// <summary>
		/// Encodes a bitmap with the specified pixel codec, then decodes it back with degraded colors for the base of the indexed image.
		/// </summary>
		/// <param name="texture">Bitmap to encode.</param>
		/// <param name="gvrPaletteCodec">GVR codec to use.</param>
		/// <returns></returns>
		internal static Bitmap CalculateLossyForPalette(Bitmap texture, GvrPixelCodec gvrPaletteCodec)
		{
			byte[] encoded = gvrPaletteCodec.Encode(TextureFunctions.BitmapToRaw(texture), texture.Width, texture.Height);
			byte[] decoded = gvrPaletteCodec.Decode(encoded, texture.Width, texture.Height);
			Bitmap output = new Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			TextureFunctions.RawToBitmap(output, decoded);
			return output;
		}

		/// <summary>
		/// Encodes mipmaps, optionally with a quantizer.
		/// </summary>
		/// <param name="image">ImageSharp image to encode.</param>
		/// <param name="quantizer">Quantizer to use (optional).</param>
		/// <param name="pixelCodec">GVR codec to use.</param>
		/// <param name="writer">Output memory stream.</param>
		internal static void EncodeMipMaps(Image<Rgba32> image, IQuantizer<Rgba32>? quantizer, GvrPixelCodec pixelCodec, MemoryStream writer)
		{
			for (int size = image.Width >> 1; size > 0; size >>= 1)
			{
				byte[] mipMapPixels;
				Image<Rgba32> mipMapImage = image.Clone();
				mipMapImage.Mutate(x => x.Resize(size, size));
				// If there is a quantizer, use it.
				// Since indexed GVR apparently can't have mipmaps, this whole thing is unnecessary. Also it seems to be broken for Index4's 1x1 mipmap.
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
				// If no quantizer is specified, copy image data directly.
				else
				{
					mipMapPixels = new byte[size * size * 4];
					mipMapImage.CopyPixelDataTo(mipMapPixels);
				}
				writer.Write(pixelCodec.Encode(mipMapPixels, size, size));
			}
		}
	}
}