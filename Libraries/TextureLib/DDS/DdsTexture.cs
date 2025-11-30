using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Drawing;
using System.IO;

namespace TextureLib
{
	// Class for DDS textures
	public class DdsTexture : GenericTexture
	{
		public DdsPixelFormat DdsPixelFormat;
		public DdsHeaderFlags DdsHeaderFlags;
		public DdsPixelBitFormat DdsDataFormat;
		private PixelCodec pixelCodec;

		public DdsTexture(byte[] data, int offset = 0, string name = null)
		{
			InitTexture(data, offset, name);
			// Read information about the texture
			Height = (ushort)BitConverter.ToUInt32(RawData, 0xC);
			Width = (ushort)BitConverter.ToUInt32(RawData, 0x10);

			DdsPixelFormat = (DdsPixelFormat)(RawData[0x50]);
			DdsHeaderFlags = (DdsHeaderFlags)BitConverter.ToUInt32(RawData, 0x8);
			uint amask = BitConverter.ToUInt32(RawData, 0x68);
			uint rmask = BitConverter.ToUInt32(RawData, 0x5C);
			uint gmask = BitConverter.ToUInt32(RawData, 0x60);
			uint bmask = BitConverter.ToUInt32(RawData, 0x64);
			if (amask == 0)
				DdsDataFormat = DdsPixelBitFormat.RGB565;
			else if (amask == 32768 && rmask == 31744)
				DdsDataFormat = DdsPixelBitFormat.ARGB1555;
			else if (amask == 61440 && rmask == 3840)
				DdsDataFormat = DdsPixelBitFormat.ARGB4444;
			else if (amask == 4278190080 && rmask == 16711680)
				DdsDataFormat = DdsPixelBitFormat.ARGB8888;

			MemoryStream rawStream = new MemoryStream(RawData);
			DdsFile ddsFile = DdsFile.Load(rawStream);
			bool mipmaps = ddsFile.Faces[0].MipMaps.Length > 1;
			rawStream.Seek(0, SeekOrigin.Begin);

			BcDecoder decoder = new BcDecoder();			
			Image<Rgba32> image = decoder.DecodeToImageRgba32(rawStream);

			// Decode main image
			MemoryStream imageStream = new();
			image.SaveAsPng(imageStream);
			Image = new Bitmap(imageStream);

			// Decode mipmaps
			if (mipmaps)
			{
				HasMipmaps = true;
				rawStream.Seek(0, SeekOrigin.Begin);
				Image<Rgba32>[] mips = decoder.DecodeAllMipMapsToImageRgba32(rawStream);
				MipmapImages = new Bitmap[mips.Length];
				for (int i = 0; i < mips.Length; i++)
				{
					MemoryStream mipStream = new();
					mips[i].SaveAsPng(mipStream);
					MipmapImages[i] = new Bitmap(mipStream);
				}
			}
			// Get the codecs and make sure we can decode using them
			//dataCodec = DDSDataCodec.GetDataCodec(dataFormat);
			// We need a pixel codec if this is a palettized texture
			// Placeholder because palette data doesn't work properly yet
			//if (dataCodec != null && dataCodec.PaletteEntries != 0)
			/*
			pixelCodec = PixelCodec.GetPixelCodec(DdsPixelFormat);

			if (pixelCodec != null)
			{
				dataCodec.PixelCodec = pixelCodec;
				canDecode = true;
			}
			//}
			//else
			//{
			pixelFormat = DDSPixelFormat.Invalid;

			if (dataCodec != null)
			{
				canDecode = true;
			}
			//}

			// If the texture contains mipmaps, gets the offsets of them
			if (canDecode && paletteEntries == 0 && (dataFlags & DDSHeaderFlags.MipmapCount - 1) != 0)
			{
				//mipmapOffsets = new int[(int)Math.Log(textureWidth, 2)];
				mipmapOffsets = new int[2];
				int mipmapOffset = 0;
				for (int i = 0, size = textureWidth; i < mipmapOffsets.Length; i++, size >>= 1)
				{
					mipmapOffsets[i] = mipmapOffset;
					mipmapOffset += Math.Max(size * size * (dataCodec.Bpp >> 3), 32);
				}
			}
			dataOffset = 0x80;
			*/
		}

		private PixelCodec GetPixelCodecForDds(DdsPixelFormat pixelFormat)
		{
			switch (pixelFormat)
			{
				case DdsPixelFormat.RGB:
					return new RGB565PixelCodec();
				case DdsPixelFormat.RGBA:
					return new RGB5A3PixelCodec();
				case DdsPixelFormat.YUV:
					throw new NotImplementedException(pixelFormat.ToString());
				default:
					return null;
			}
		}


		public override byte[] GetBytes()
		{
			MemoryStream ms = new();

			return ms.ToArray();
		}
	}
}