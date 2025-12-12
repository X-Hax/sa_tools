using System;
using System.Collections.Generic;
using System.Drawing;

namespace TextureLib
{
    // Class for Dreamcast PVR textures
    public partial class PvrTexture : GenericTexture
    {
        const uint Magic_GBIX = 0x58494247;
        const uint Magic_PVRT = 0x54525650;

        public PvrPixelFormat PvrPixelFormat;
        public PvrDataFormat PvrDataFormat;

        public byte[] HeaderlessData; // Raw data without the header
		private bool useDithering;

		public override byte[] GetBytes()
		{
			List<byte> result = new();
			if (Gbix != 0)
			{
				result.AddRange(BitConverter.GetBytes(Magic_GBIX));
				result.AddRange(BitConverter.GetBytes((uint)8));
				result.AddRange(BitConverter.GetBytes(Gbix));
				result.AddRange(BitConverter.GetBytes((uint)0x20202020));
			}
			result.AddRange(BitConverter.GetBytes(Magic_PVRT));
			result.AddRange(BitConverter.GetBytes((uint)(HeaderlessData.Length + (PvrDataFormat == PvrDataFormat.SquareTwiddledMipmapsDma ? 12 : 8))));
			result.Add((byte)PvrPixelFormat);
			result.Add((byte)PvrDataFormat);
			result.Add((byte)PaletteBank);
			result.Add((byte)PaletteStartIndex);			
			result.AddRange(BitConverter.GetBytes((ushort)Width));
			result.AddRange(BitConverter.GetBytes((ushort)Height));
			if (PvrDataFormat == PvrDataFormat.SquareTwiddledMipmapsDma)
				result.AddRange(new byte[4]);
			result.AddRange(HeaderlessData);
			return result.ToArray();
		}

		/// <summary>
		/// Initializes a PVR texture from a byte array that contains PVR texture header and data.
		/// </summary>
		/// <param name="data">Byte array containing PVR texture header and data.</param>
		/// <param name="offset">Offset to the beginning of the GBIX or PVRT header.</param>
		/// <param name="name">Texture name, if applicable.</param>
		/// <param name="extPalette">Texture palette for decoding indexed textures, if applicable</param>
		public PvrTexture(byte[] data, int offset = 0, string name = null, TexturePalette extPalette = null)
        {
            InitTexture(data, offset, name);
            Palette = extPalette;
			Decode();
        }

		/// <summary>
		/// Encodes a PVR texture from Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="dataFormat">Target PVR data format.</param>
		/// <param name="pixelFormat">Target PVR pixel format or pixel format for indexed images.</param>
		/// <param name="mipmaps">Encode mipmaps.</param>
		/// <param name="inputPalette">Input palette for indexed images. Used to create indexed textures with a user-specified palette.</param>
		/// <param name="gbix">Global index.</param>
		/// <param name="dither">Use dithering for encoding indexed images.</param>
		/// <param name="paletteExternal">Save the palette to an external file.</param>
		public PvrTexture(Bitmap texture, PvrDataFormat dataFormat, PvrPixelFormat pixelFormat, bool mipmaps, TexturePalette inputPalette = null, uint gbix = 0, string name = null, bool dither = false, bool paletteExternal = false)
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
			// Set common texture properties
			Image = texture;
			Gbix = gbix;
			Name = name;
			PvrDataFormat = dataFormat;
			PvrPixelFormat = pixelFormat;
			Width = texture.Width;
			Height = texture.Height;
			Palette = inputPalette;
			// Set dithering flag
			useDithering = dither;
			// Prepare mipmaps
			if (mipmaps)
			{
				HasMipmaps = true;
				// Calculate the number of mip levels
				int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
				// Generate mipmaps for the preview version
				MipmapImages = new Bitmap[mipLevels];
				int mipWidth = Width;
				for (int m = 0; m < mipLevels; m++)
				{
					MipmapImages[m] = new Bitmap(Image, Width, Width);
					mipWidth >>= 1;
				}
			}
			if (dataFormat == PvrDataFormat.Index8 || dataFormat == PvrDataFormat.Index4 || dataFormat == PvrDataFormat.Index4Mipmaps || dataFormat == PvrDataFormat.Index8Mipmaps)
			{
				Indexed = true;
				RequiresPaletteFile = paletteExternal;
			}
			Encode();
		}

		public PvrTexture Clone()
		{
			return new PvrTexture(RawData, 0, Name, Palette);
		}

		public override void Decode()
		{
			int currentOffset = 0;
			int gbixOffset = 0x00;
			int pvrtOffset = 0x00;
			if (BitConverter.ToUInt32(RawData, currentOffset) == Magic_GBIX)
			{
				gbixOffset = 0x00;
				pvrtOffset = 0x08 + BitConverter.ToInt32(RawData, gbixOffset + 4);
			}
			else if (BitConverter.ToUInt32(RawData, currentOffset + 0x04) == Magic_GBIX)
			{
				gbixOffset = 0x04;
				pvrtOffset = 0x0C + BitConverter.ToInt32(RawData, gbixOffset + 4);
			}
			else if (BitConverter.ToUInt32(RawData, currentOffset + 0x04) == Magic_PVRT)
			{
				gbixOffset = -1;
				pvrtOffset = 0x04;
			}
			else
			{
				gbixOffset = -1;
				pvrtOffset = 0x00;
			}
			// Get global index
			if (gbixOffset != -1)
				Gbix = BitConverter.ToUInt32(RawData, gbixOffset + 0x8);
			else
				Gbix = 0;
			currentOffset = pvrtOffset;
			// Parse header
			int chunksize = BitConverter.ToInt32(RawData, currentOffset + 0x4);
			PvrPixelFormat = (PvrPixelFormat)RawData[currentOffset + 0x8];
			PvrDataFormat = (PvrDataFormat)RawData[currentOffset + 0x9];
			PaletteBank = RawData[currentOffset + 0xA];
			PaletteStartIndex = RawData[currentOffset + 0xB];
			Width = ByteConverter.ToUInt16(RawData, currentOffset + 0xC);
			Height = ByteConverter.ToUInt16(RawData, currentOffset + 0xE);
			// Override pixel format for Bitmap (because Bitmap can only be in ARGB8888)
			if (PvrDataFormat is PvrDataFormat.Bitmap || PvrDataFormat is PvrDataFormat.BitmapMipmaps)
				PvrPixelFormat = PvrPixelFormat.Argb8888orYUV420;
			// Set texture properties
			switch (PvrDataFormat)
			{
				case PvrDataFormat.VqMipmaps:
				case PvrDataFormat.SmallVqMipmaps:
				case PvrDataFormat.SquareTwiddledMipmaps:
				case PvrDataFormat.RectangleMipmaps:
				case PvrDataFormat.RectangleStrideMipmaps:
				case PvrDataFormat.BitmapMipmaps:
				case PvrDataFormat.SquareTwiddledMipmapsDma:
					HasMipmaps = true;
					break;
				case PvrDataFormat.Index4:
				case PvrDataFormat.Index8:
					Indexed = true;
					RequiresPaletteFile = true;
					break;
				case PvrDataFormat.Index4Mipmaps:
				case PvrDataFormat.Index8Mipmaps:
					Indexed = true;
					HasMipmaps = true;
					RequiresPaletteFile = true;
					break;
			}

			currentOffset += 16;

			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(PvrPixelFormat);
			PvrDataCodec dataCodec = PvrDataCodec.Create(PvrDataFormat, pixelCodec);
			int dataSize = dataCodec.CalculateTextureSize(Width, Height);
			int paletteEntries = dataCodec.GetPaletteEntries(Width);

			if (PvrPixelFormat is PvrPixelFormat.Argb8888orYUV420 && dataSize > RawData.Length)
				throw new NotImplementedException("YUV420 support is not implemented");
#if DEBUG
			Console.WriteLine("\nPVR TEXTURE INFO");
			Console.WriteLine("Width: " + Width.ToString());
			Console.WriteLine("Height: " + Height.ToString());
			Console.WriteLine("Gbix: " + Gbix.ToString());
			Console.WriteLine("Pixel format: {0} ({1} bytes per {2} pixels)", PvrPixelFormat.ToString(), pixelCodec.BytesPerPixel.ToString(), pixelCodec.Pixels.ToString());
			Console.WriteLine("Data format: " + PvrDataFormat.ToString());
			Console.WriteLine("Mipmaps: " + HasMipmaps.ToString());
			Console.WriteLine("Indexed: " + Indexed.ToString());
#endif
			if (paletteEntries > 0 && !dataCodec.NeedsExternalPalette)
			{
				dataSize += paletteEntries / pixelCodec.Pixels * pixelCodec.BytesPerPixel;
			}
			if (dataCodec.HasMipmaps)
			{
				for (int size = 1; size < Width; size <<= 1)
				{
					dataSize += dataCodec.CalculateTextureSize(size, size);
				}
			}
			HeaderlessData = new byte[dataSize];
			// DMA offset
			if (PvrDataFormat == PvrDataFormat.SquareTwiddledMipmapsDma)
			{
				currentOffset += 4;
				dataSize -= 4;
			}
			Array.Copy(RawData, currentOffset, HeaderlessData, 0, dataSize);

			ReadOnlySpan<byte> palette = DecodeInternalPalette(dataCodec, out int paletteSize);
			int textureAddress = HeaderlessData.Length - dataCodec.CalculateTextureSize(Width, Height);
			ReadOnlySpan<byte> textureData = HeaderlessData[textureAddress..];
			byte[] result = dataCodec.Decode(textureData, Width, Height, palette);

			if (Indexed)
			{
				result = ApplyPalette(result, Width, Height).ToArray();
			}

			Image = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			TextureFunctions.RawToBitmap(Image, result);

			if (HasMipmaps)
			{
				// Calculate the number of mip levels
				int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
				MipmapImages = new Bitmap[mipLevels];
				int[] mipmapOffsets = new int[mipLevels];
				// Start offset for the first mipmap
				int mipmapOffset = paletteSize;
				for (int i = mipLevels - 1, sizex = 1; i >= 0; i--, sizex <<= 1)
				{
					mipmapOffsets[i] = sizex == 1 ? 2 : mipmapOffset;
					byte[] src = HeaderlessData[mipmapOffsets[i]..];
					//Console.WriteLine("Mipmap {0} ({1}x{1}) : 0x{2})", i, sizex, mipmapOffsets[i].ToString("X"));
					byte[] mipRawData = dataCodec.Decode(src, sizex, sizex, palette);
					// Workarounds for 1x1 mipmaps
					if (sizex == 1)
					{
						// In VQ Mipmap formats, the 1x1 mipmap is retrieved from the bottom right pixel of the 2x2 mipmap
						if (PvrDataFormat == PvrDataFormat.VqMipmaps || PvrDataFormat == PvrDataFormat.SmallVqMipmaps)
						{
							src = HeaderlessData[(mipmapOffset + 1)..];
							mipRawData = dataCodec.Decode(src, sizex * 2, sizex * 2, palette);
							byte[] newmipdata = new byte[4];
							Array.Copy(mipRawData, 12, newmipdata, 0, 4);
							mipRawData = newmipdata;
						}
						// In YUV422 textures, the 1x1 mipmap is stored as RGB565
						else if (PvrPixelFormat == PvrPixelFormat.Yuv422)
						{
							PvrDataCodec dataCodecTemp = PvrDataCodec.Create(PvrDataFormat, new RGB565PixelCodec());
							mipRawData = dataCodecTemp.Decode(src, sizex, sizex, palette);
						}					
					}
					if (Indexed)
						mipRawData = ApplyPalette(mipRawData, sizex, sizex).ToArray();
					Bitmap mipBitmap = new Bitmap(sizex, sizex, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					TextureFunctions.RawToBitmap(mipBitmap, mipRawData);
					MipmapImages[i] = mipBitmap;
					mipmapOffset += dataCodec.CalculateTextureSize(sizex, sizex);
				}
			}
		}

		private ReadOnlySpan<byte> DecodeInternalPalette(PvrDataCodec dataCodec, out int bytesRead)
        {
            Span<byte> result = null;
            int paletteEntries = dataCodec.GetPaletteEntries(Width);
            bytesRead = 0;

            if (paletteEntries > 0 && !dataCodec.NeedsExternalPalette)
            {
                PixelCodec pixelCodec = dataCodec.PixelCodec;

                int srcAddress = 0;
                result = new byte[paletteEntries * 4];

                for (int i = 0; i < paletteEntries; i += pixelCodec.Pixels)
                {
                    pixelCodec.DecodePixel(HeaderlessData[srcAddress..], result[(i * 4)..]);
                    srcAddress += pixelCodec.BytesPerPixel;
                }

                bytesRead = srcAddress;
            }

            return result;
        }
	}
}