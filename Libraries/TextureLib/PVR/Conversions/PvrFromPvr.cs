using System;
using System.IO;

namespace TextureLib
{
	public partial class PvrTexture
	{
		/// <summary>Create a new PVR texture from an existing PVR texture, pixel and data formats determined automatically.</summary>
		public PvrTexture(PvrTexture pvr, bool forceMipmaps = false)
		{
			PvrDataFormat targetDataFormat = AutoPvrDataFormatFromImage(pvr.Image, forceMipmaps ? true : pvr.HasMipmaps);
			PvrPixelFormat targetPixelFormat = AutoPvrPixelFormatFromImage(pvr.Image);
			ConvertFromPvr(pvr, targetPixelFormat, targetDataFormat);
		}

		/// <summary>Create a new PVR texture from an existing PVR texture, data format determined automatically.</summary>
		public PvrTexture(PvrTexture pvr, PvrPixelFormat targetPixelFormat, bool forceMipmaps = false)
		{
			PvrDataFormat targetDataFormat = AutoPvrDataFormatFromImage(pvr.Image, forceMipmaps ? true : pvr.HasMipmaps);
			ConvertFromPvr(pvr, targetPixelFormat, targetDataFormat);
		}

		/// <summary>Create a new PVR texture from an existing PVR texture, pixel format determined automatically.</summary>
		public PvrTexture(PvrTexture pvr, PvrDataFormat targetDataFormat)
		{
			PvrPixelFormat targetPixelFormat = AutoPvrPixelFormatFromImage(pvr.Image);
			ConvertFromPvr(pvr, targetPixelFormat, targetDataFormat);
		}

		/// <summary>Create a new PVR texture from an existing PVR texture, pixel format specified manually.</summary>
		public PvrTexture(PvrTexture pvr, PvrPixelFormat targetPixelFormat, PvrDataFormat targetPvrDataFormat)
		{
			ConvertFromPvr(pvr, targetPixelFormat, targetPvrDataFormat);
		}

		private void ConvertFromPvr(PvrTexture src, PvrPixelFormat targetPixelFormat, PvrDataFormat targetPvrDataFormat)
		{
			// Set common texture properties
			Image = src.Image;
			Gbix = src.Gbix;
			Name = src.Name;
			Width = src.Width;
			Height = src.Height;
			Palette = src.Palette;
			PvrPixelFormat = targetPixelFormat;
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = src.PaletteBank;
			PaletteStartIndex = src.PaletteStartIndex;
			PakMetadata = src.PakMetadata;
			PvmxOriginalDimensions = src.PvmxOriginalDimensions;
			bool forceMipmaps = false;
			bool lossless = true;
			// Converting to a different pixel format -> Reencode
			if (src.PvrPixelFormat != PvrPixelFormat)
				lossless = false;
			// Conversion restrictions based on target format
			switch (PvrDataFormat)
			{
				// Converting to Bitmap -> Reencode
				case PvrDataFormat.Bitmap:
				case PvrDataFormat.BitmapMipmaps:
					lossless = false;
					break;
				// Converting to VQ -> Reencode + check width/height
				case PvrDataFormat.Vq:
				case PvrDataFormat.SmallVq:
					if (src.Width != src.Height)
					{
						Image = new System.Drawing.Bitmap(Math.Min(src.Width, src.Height), Math.Min(src.Width, src.Height));
					}
					lossless = false;
					break;
				// Converting to Indexed formats -> Reencode + check width/height
				case PvrDataFormat.Index4:
				case PvrDataFormat.Index4Mipmaps:
				case PvrDataFormat.Index8:
				case PvrDataFormat.Index8Mipmaps:
					if (src.Width != src.Height)
					{
						Image = new System.Drawing.Bitmap(Math.Min(src.Width, src.Height), Math.Min(src.Width, src.Height));
					}
					lossless = false;
					break;
				// Converting to Square format - check width/height
				case PvrDataFormat.SquareTwiddled:
				case PvrDataFormat.SquareTwiddledMipmaps:
				case PvrDataFormat.SquareTwiddledMipmapsDma:
					if (src.Width != src.Height)
					{
						Image = new System.Drawing.Bitmap(Math.Min(src.Width, src.Height), Math.Min(src.Width, src.Height));
						lossless = false;
					}
					break;
			}
			// Conversion restrictions based on source format
			switch (src.PvrDataFormat)
			{
				// Converting from Bitmap -> Reencode
				case PvrDataFormat.Bitmap:
				case PvrDataFormat.BitmapMipmaps:
					lossless = false;
					break;
				// Converting from VQ -> Reencode
				case PvrDataFormat.Vq:
				case PvrDataFormat.SmallVq:
					lossless = false;
					break;
				// Converting from Indexed formats -> Reencode
				case PvrDataFormat.Index4:
				case PvrDataFormat.Index4Mipmaps:
				case PvrDataFormat.Index8:
				case PvrDataFormat.Index8Mipmaps:
					lossless = false;
					break;
				default:
					break;
			}
			if (!src.HasMipmaps)
			{
				switch (PvrDataFormat)
				{
					case PvrDataFormat.SquareTwiddledMipmaps:
						PvrDataFormat = PvrDataFormat.SquareTwiddled;
						forceMipmaps = true;
						break;
					case PvrDataFormat.SquareTwiddledMipmapsDma:
						PvrDataFormat = PvrDataFormat.SquareTwiddled;
						forceMipmaps = true;
						break;
					case PvrDataFormat.RectangleMipmaps:
						PvrDataFormat = PvrDataFormat.Rectangle;
						forceMipmaps = true;
						break;
					case PvrDataFormat.RectangleStrideMipmaps:
						PvrDataFormat = PvrDataFormat.RectangleStride;
						forceMipmaps = true;
						break;
					case PvrDataFormat.SmallVqMipmaps:
						PvrDataFormat = PvrDataFormat.SmallVq;
						forceMipmaps = true;
						break;
					case PvrDataFormat.VqMipmaps:
						PvrDataFormat = PvrDataFormat.Vq;
						forceMipmaps = true;
						break;
					case PvrDataFormat.Index4Mipmaps:
						PvrDataFormat = PvrDataFormat.Index4;
						forceMipmaps = true;
						break;
					case PvrDataFormat.Index8Mipmaps:
						PvrDataFormat = PvrDataFormat.Index8;
						forceMipmaps = true;
						break;
					case PvrDataFormat.BitmapMipmaps:
						PvrDataFormat = PvrDataFormat.Bitmap;
						forceMipmaps = true;
						break;
				}
			}
			// Lossy encoding
			if (!lossless)
			{
				Encode();
				if (forceMipmaps)
					AddMipmaps();
			}
			// Lossless conversion
			else
			{
				// Set mipmap flag
				switch (PvrDataFormat)
				{
					case PvrDataFormat.Index4Mipmaps:
					case PvrDataFormat.Index8Mipmaps:
					case PvrDataFormat.SmallVqMipmaps:
					case PvrDataFormat.VqMipmaps:
					case PvrDataFormat.SquareTwiddledMipmaps:
					case PvrDataFormat.SquareTwiddledMipmapsDma:
						HasMipmaps = true;
						break;
				}
				// For most formats, use lossless conversion
				switch (src.PvrPixelFormat)
				{
					case PvrPixelFormat.Rgb565:
					case PvrPixelFormat.Argb1555:
					case PvrPixelFormat.Argb4444:
					case PvrPixelFormat.Bump88:
					case PvrPixelFormat.Yuv422:
					case PvrPixelFormat.Rgb555:
						MemoryStream outputStream = new();
						PvrDataCodec inputDataCodec = PvrDataCodec.Create(src.PvrDataFormat, new Bypass16BitPixelCodec());
						PvrDataCodec outputDataCodec = PvrDataCodec.Create(PvrDataFormat, new Bypass16BitPixelCodec());
						// VQ Codebook
						ReadOnlySpan<byte> palette = src.DecodeInternalPalette(inputDataCodec, out int paletteSize);
						if (paletteSize != 0)
							outputStream.Write(palette);
						// Original texture data
						int textureAddress = src.HeaderlessData.Length - inputDataCodec.CalculateTextureSize(Width, Height);
						ReadOnlySpan<byte> textureData = src.HeaderlessData[textureAddress..];
						byte[] mainTexRaw = inputDataCodec.Decode(textureData, Width, Height, palette);
						// Mipmaps come first in PVR
						if (HasMipmaps)
						{
							// Calculate the number of mip levels
							int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
							// Convert mipmaps if they're already there
							if (src.HasMipmaps)
							{
								for (int i = mipLevels - 1, sizex = 1; i >= 0; i--, sizex <<= 1)
								{
									int[] mipmapOffsets = new int[mipLevels];
									// Start offset for the first mipmap
									int mipmapOffset = paletteSize;
									mipmapOffsets[i] = mipmapOffset;
									byte[] srcdata = src.HeaderlessData[mipmapOffsets[i]..];
									int mipDataSize = Math.Max(1, src.HeaderlessData.Length - paletteSize - srcdata.Length);
									//Console.WriteLine("Mipmap {0} ({1}x{1}) : {2} (size {3})", i, sizex, mipmapOffsets[i].ToString("X"), mipDataSize.ToString());
									byte[] mipRawData = inputDataCodec.Decode(srcdata, sizex, sizex, palette);
									if (HasMipmaps && sizex != Width)
										outputStream.Write(outputDataCodec.Encode(mipRawData, sizex, sizex));
									mipmapOffset += inputDataCodec.CalculateTextureSize(sizex, sizex);
								}
							}
						}
						// Write texture
						outputStream.Write(outputDataCodec.Encode(mainTexRaw, Width, Height));
						// Update the texture's data arrays
						HeaderlessData = outputStream.ToArray();
						RawData = GetBytes();
						// Force add mipmaps, if necessary
						if (forceMipmaps)
						{
							HasMipmaps = false;
							AddMipmaps();
						}
						break;
					// For other formats, use lossy conversion
					default:
						Encode();
						break;
				}
			}
		}
	}
}