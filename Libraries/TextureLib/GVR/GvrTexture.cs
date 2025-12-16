using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
	// Class for Gamecube and Wii GVR textures
	public partial class GvrTexture : GenericTexture
	{
		const uint Magic_GBIX = 0x58494247;
		const uint Magic_GCIX = 0x58494347;
		const uint Magic_GVRT = 0x54525647;

		public GvrPaletteFormat GvrPaletteFormat;
		public GvrDataFormat GvrDataFormat;
		private GvrDataFlags GvrDataFlags;
		public byte[] HeaderlessData;

		// Encoder parameters
		private bool useDithering;
		private bool useSACompatiblePalette;

		public override byte[] GetBytes()
		{
			List<byte> result = new();
			if (Gbix != 0)
			{
				result.AddRange(BitConverter.GetBytes(Magic_GBIX));
				result.AddRange(BitConverter.GetBytes((uint)8));
				result.AddRange(ByteConverter.GetBytesBE(Gbix));
				result.AddRange(BitConverter.GetBytes((uint)0));
			}
			result.AddRange(BitConverter.GetBytes(Magic_GVRT));
			result.AddRange(BitConverter.GetBytes((uint)(HeaderlessData.Length + 8)));
			result.Add((byte)PaletteBank);
			result.Add((byte)PaletteStartIndex);
			int flag = ((byte)GvrPaletteFormat << 4) | (byte)GvrDataFlags;
			result.Add((byte)flag);
			result.Add((byte)GvrDataFormat);
			result.AddRange(ByteConverter.GetBytesBE((ushort)Width));
			result.AddRange(ByteConverter.GetBytesBE((ushort)Height));
			result.AddRange(HeaderlessData);
			return result.ToArray();
		}

		/// <summary>
		/// Initializes a GVR texture from a byte array that contains GVR texture header and data.
		/// </summary>
		/// <param name="data">Byte array containing GVR texture header and data.</param>
		/// <param name="offset">Offset to the beginning of the GVRT or GBIX/GCIX texture header.</param>
		/// <param name="name">Texture name, if applicable.</param>
		/// <param name="extPalette">Texture palette for decoding indexed textures, if applicable</param>
		public GvrTexture(byte[] data, int offset = 0, string name = null, TexturePalette extPalette = null)
		{
			InitTexture(data, offset, name);
			Palette = extPalette;
			Decode();
		}

		/// <summary>
		/// Encodes a GVR texture from Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="dataFormat">Target GVR data format.</param>
		/// <param name="mipmaps">Encode mipmaps.</param>
		/// <param name="inputPalette">Input palette for indexed images. Used to create indexed textures with a user-specified palette.</param>
		/// <param name="gbix">Global index.</param>
		/// <param name="paletteFormat">Pixel format for indexed images.</param>
		/// <param name="dither">Use dithering for encoding indexed images.</param>
		/// <param name="paletteExternal">Save the palette to an external file.</param>
		/// <param name="paletteSACompatible">Use SADX and SA2B compatible palette formats: ARGB1555 instead of IntensityA8 and ARGB4444 instead of RGB5A3.</param>
		public GvrTexture(Bitmap texture, GvrDataFormat dataFormat, bool mipmaps, TexturePalette inputPalette = null, uint gbix = 0, string name = null, GvrPaletteFormat paletteFormat = GvrPaletteFormat.Rgb5A3orArgb4444, bool dither = false, bool paletteExternal = false, bool paletteSACompatible = true)
		{
			// Disable mipmaps if using incompatible texture encoder settings
			if (mipmaps && texture.Width != texture.Height)
				mipmaps = false;
			// Set common texture data
			Name = name;
			Image = new Bitmap(texture);
			Gbix = gbix;
			GvrDataFormat = dataFormat;
			Width = texture.Width;
			Height = texture.Height;
			Palette = inputPalette;
			GvrPaletteFormat = paletteFormat;
			// Set flags for Indexed formats
			if (dataFormat == GvrDataFormat.Index8 || dataFormat == GvrDataFormat.Index4 || dataFormat == GvrDataFormat.Index14)
			{
				Indexed = true;
				GvrDataFlags |= paletteExternal ? GvrDataFlags.ExternalPalette : GvrDataFlags.InternalPalette;
				RequiresPaletteFile = paletteExternal;
			}
			// Set encoder parameters
			useDithering = dither;
			useSACompatiblePalette = paletteSACompatible; // Check whether to use SADX and SA2 compatible palette colors
			// Get mipmaps ready
			if (mipmaps)
			{
				HasMipmaps = true;
				GvrDataFlags |= GvrDataFlags.Mipmaps;
				// Calculate the number of mip levels
				int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
				// Generate mipmaps for the preview version
				MipmapImages = new Bitmap[mipLevels];
				int mipWidth = Width;
				for (int m = 0; m < mipLevels; m++)
				{
					MipmapImages[m] = new Bitmap(Image, mipWidth, mipWidth);
					mipWidth >>= 1;
				}
			}
			// Encode the texture
			Encode();
		}

		public override void Decode()
		{
			int currentOffset = 0;

			// Get global index
			if (BitConverter.ToUInt32(RawData, currentOffset) == Magic_GBIX || BitConverter.ToUInt32(RawData, currentOffset) == Magic_GCIX)
			{
				Gbix = ByteConverter.ToUInt32BE(RawData, currentOffset + 0x8);
				currentOffset += 0x10;
			}

			// Parse header
			if (BitConverter.ToUInt32(RawData, currentOffset) == Magic_GVRT)
			{
				int chunksize = ByteConverter.ToInt32BE(RawData, currentOffset + 0x4);
				PaletteBank = RawData[currentOffset + 0x8]; // Not confirmed
				PaletteStartIndex = RawData[currentOffset + 0x9]; // Not confirmed
				GvrPaletteFormat = (GvrPaletteFormat)(RawData[currentOffset + 0x0A] >> 4); // Only the first 4 bits matter
				GvrDataFlags = (GvrDataFlags)(RawData[currentOffset + 0x0A] & 0x0F); // Only the last 4 bits matter
				GvrDataFormat = (GvrDataFormat)RawData[currentOffset + 0x0B];
				Width = ByteConverter.ToUInt16BE(RawData, currentOffset + 0xC);
				Height = ByteConverter.ToUInt16BE(RawData, currentOffset + 0xE);
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
			GvrDataCodec dataCodec = GvrDataCodec.GetDataCodec(GvrDataFormat);
			// Calculate the expected size of the texture data chunk. However, this may not be the final size if the texture has a CLUT or mipmaps.
			int dataSize = dataCodec.CalculateTextureSize(Width, Height);
			int paletteSize = 0;
			// Manage the texture's CLUT (internal palette)
			if (dataCodec.PaletteEntries != 0 && GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
			{				
				PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, false);
				paletteSize = paletteCodec.BytesPerPixel * dataCodec.PaletteEntries;
				byte[] paletteData = new byte[paletteSize];
				Array.Copy(RawData, currentOffset, paletteData, 0, paletteSize);
				currentOffset += paletteSize;
				Palette = new TexturePalette(paletteData, paletteCodec, dataCodec.PaletteEntries, bigEndian: true);
			}

			// Adjust data size for mipmaps
			if (HasMipmaps)
			{
				for (int size = Width >> 1; size > 0; size >>= 1)
				{
					//Console.WriteLine("Mip size {0}", size);
					dataSize += dataCodec.CalculateTextureSize(size, size);
				}
			}

			// Set up data without header
			HeaderlessData = new byte[dataSize + paletteSize];
			Array.Copy(RawData, currentOffset - paletteSize, HeaderlessData, 0, dataSize + paletteSize);

			// Set up data to decode the main texture and mipmaps.
			// This is used instead of HeaderlessData because the data codec includes palette size in the calculation and that interferes with decoding.
			byte[] textureToDecode = new byte[dataSize];
			Array.Copy(RawData, currentOffset, textureToDecode, 0, dataSize);

			// Decode data
			int textureAddress = textureToDecode.Length - dataCodec.CalculateTextureSize(Width, Height);
			ReadOnlySpan<byte> textureData = textureToDecode[textureAddress..];
			byte[] result = dataCodec.Decode(textureToDecode, Width, Height, null);

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
						mipOffset += dataCodec.CalculateTextureSize(size, size);
					}
					byte[] mipData = textureToDecode[mipOffset..].ToArray();
					byte[] mipRawData = dataCodec.Decode(mipData, size, size, null);
					if (Indexed)
						mipRawData = ApplyPalette(mipRawData, size, size).ToArray();
					Bitmap mipBitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
					TextureFunctions.RawToBitmap(mipBitmap, mipRawData);
					MipmapImages[mipmapIndex] = mipBitmap;
				}
			}
		}

		public override void Encode()
		{
			// Check if the palette needs to be generated
			bool autoPalette = Palette == null;

			// Determine the data codec
			GvrDataCodec dataCodec = GvrDataCodec.GetDataCodec(GvrDataFormat);

			MemoryStream outputStream = new();

			// Encoding to an indexed format
			if (dataCodec.PaletteEntries > 0)
			{
				bool index8 = GvrDataFormat == GvrDataFormat.Index8;
				// If the user hasn't specified a palette, get one from the quantized bitmap
				if (autoPalette)
				{
					// Quantize the image using the default quantizer to get a palette (ignore the output bitmap data because only the palette is needed)
					TextureFunctions.QuantizeImage(TextureFunctions.CalculateLossyForPaletteOrVq(Image, GvrDataCodec.GetGvrDataCodecForPalette(GvrPaletteFormat)), index8, out Palette, useDithering);
					// Sort the palette by luminance
					Palette.SortByLuminance();
				}
				// Quantize the image using the specific quantizer created from the palette (ignore the output palette data because there's a palette already)
				byte[] indexedBitmapData = TextureFunctions.QuantizeImage(TextureFunctions.CalculateLossyForPaletteOrVq(Image, GvrDataCodec.GetGvrDataCodecForPalette(GvrPaletteFormat)), index8, out _, useDithering, Palette);
				// If the palette was created from the image (as opposed to user specified), encode it with the specified pixel codec
				if (autoPalette)
					Palette.Encode(PixelCodec.GetPixelCodec(GvrPaletteFormat, useSACompatiblePalette), true);
				// If the palette is embedded, write it to texture data
				if (!RequiresPaletteFile)
				{
					PixelCodec paletteCodec = PixelCodec.GetPixelCodec(GvrPaletteFormat, useSACompatiblePalette);
					int paletteSize = paletteCodec.BytesPerPixel * dataCodec.PaletteEntries;
					byte[] paletteData = new byte[paletteSize];
					Array.Copy(Palette.RawData, 0, paletteData, 0, Palette.RawData.Length);
					outputStream.Write(paletteData);
				}
				// Encode the indexed texture itself
				outputStream.Write(dataCodec.Encode(indexedBitmapData, Image.Width, Image.Height));
				// Encode mipmaps if specified
				if (HasMipmaps)
				{
					PaletteQuantizer quantizer = TexturePalette.CreatePaletteQuantizer(Palette, Palette.GetNumColors(), 0, useDithering);
					// GVR mipmap order: from largest to smallest
					for (int size = Image.Width >> 1; size > 0; size >>= 1)
					{
						TextureFunctions.EncodeMipMap(TextureFunctions.BitmapToImageSharp(Image), quantizer.CreatePixelSpecificQuantizer<Rgba32>(SixLabors.ImageSharp.Configuration.Default), dataCodec, size, outputStream);
					}
					GvrDataFlags |= GvrDataFlags.Mipmaps;
				}
			}
			// Encoding to a non-indexed format
			else
			{
				byte[] encodedBytes = new byte[Image.Width * Image.Height * 4];
				TextureFunctions.BitmapToRaw(Image, encodedBytes);
				outputStream.Write(dataCodec.Encode(encodedBytes, Image.Width, Image.Height));
				// Encode mipmaps if specified
				if (HasMipmaps)
				{
					// GVR mipmap order: from largest to smallest
					for (int size = Image.Width >> 1; size > 0; size >>= 1)
					{
						TextureFunctions.EncodeMipMap(SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(encodedBytes, Image.Width, Image.Height), null, dataCodec, size, outputStream);
					}
					GvrDataFlags |= GvrDataFlags.Mipmaps;
				}
			}
			// Set the texture's raw data
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		public GvrTexture Clone()
		{
			return new GvrTexture(RawData, 0, Name, Palette) { PakMetadata = PakMetadata, PvmxOriginalDimensions = PvmxOriginalDimensions };
		}

		public static bool Identify(byte[] data, int offset = 0)
		{
			int gbixOffset = 0x00;
			int pvrtOffset = 0x00;
			if (BitConverter.ToUInt32(data, offset) == Magic_GVRT)
				return true;
			else if (BitConverter.ToUInt32(data, offset + 4) == Magic_GVRT)
				return true;
			else if (BitConverter.ToUInt32(data, offset) == Magic_GBIX || BitConverter.ToUInt32(data, offset) == Magic_GCIX)
			{
				gbixOffset = offset;
				pvrtOffset = offset + 8 + BitConverter.ToInt32(data, gbixOffset + 4);
			}
			else if (BitConverter.ToUInt32(data, offset + 4) == Magic_GBIX || BitConverter.ToUInt32(data, offset + 4) == Magic_GCIX)
			{
				gbixOffset = offset + 4;
				pvrtOffset = offset + 0x0C + BitConverter.ToInt32(data, gbixOffset + 4);
			}
			return (BitConverter.ToUInt32(data, pvrtOffset) == Magic_GVRT);
		}

		public override bool CanHaveMipmaps()
		{
			return Width == Height;
		}

		public override string Info()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("GVR TEXTURE INFO");
			sb.AppendLine("Width: " + Width.ToString());
			sb.AppendLine("Height: " + Height.ToString());
			sb.AppendLine("Gbix: " + Gbix.ToString());
			sb.AppendLine(string.Format("Data format: {0} (0x{1})" + GvrDataFormat.ToString(), ((int)(GvrDataFormat)).ToString("X")));
			sb.AppendLine("Palette format: " + (RequiresPaletteFile ? "External" : GvrPaletteFormat.ToString()));
			sb.AppendLine("Mipmaps: " + HasMipmaps.ToString());
			sb.AppendLine("Indexed: " + Indexed.ToString());
			sb.AppendLine("Requires palette file: " + RequiresPaletteFile.ToString());
			sb.AppendLine("Flags: " + GvrDataFlags.ToString());
			if (GvrDataFlags.HasFlag(GvrDataFlags.Mipmaps))
				sb.Append("Mipmaps (0x1) ");
			if (GvrDataFlags.HasFlag(GvrDataFlags.InternalPalette))
				sb.Append("InternalPalette (0x8) ");
			if (GvrDataFlags.HasFlag(GvrDataFlags.ExternalPalette))
				sb.Append("ExternalPalette (0x2)");
			sb.Append(System.Environment.NewLine);
			return sb.ToString();
		}
	}
}