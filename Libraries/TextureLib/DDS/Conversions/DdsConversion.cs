using System;
using System.Drawing;
using System.IO;

namespace TextureLib
{
	public partial class DdsTexture
	{
		public static DdsFormat AutoDdsFormatFromImage(Bitmap image, bool maxQuality = false, bool useDxt = true)
		{
			DdsFormat targetFormat;
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(image);
			switch (alphaLevel)
			{
				case BitmapAlphaLevel.None:
					if (useDxt)
						targetFormat = DdsFormat.Dxt1;
					else
						targetFormat = maxQuality ? DdsFormat.Rgb888 : DdsFormat.Rgb565;
					break;
				case BitmapAlphaLevel.OneBitAlpha:
					if (useDxt)
						targetFormat = maxQuality ? DdsFormat.Dxt3 : DdsFormat.Dxt1;
					else
						targetFormat = maxQuality ? DdsFormat.Argb8888 : DdsFormat.Argb1555;
					break;
				case BitmapAlphaLevel.FullAlpha:
				default:
					if (useDxt)
						targetFormat = maxQuality ? DdsFormat.Dxt5 : DdsFormat.Dxt3;
					targetFormat = maxQuality ? DdsFormat.Argb8888 : DdsFormat.Argb4444;
					break;
			}
			return targetFormat;
		}

		public override void AddMipmaps()
		{
			// If the texture already has mipmaps, do nothing
			if (HasMipmaps)
				return;
			MemoryStream outputStream = new MemoryStream();
			// Set pixel and data codec
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(DdsFormat);
			DdsDataCodec dataCodec = DdsDataCodec.GetDataCodec(DdsFormat, pixelCodec, true);
			// Copy main texture
			int textureSize = dataCodec.CalculateTextureSize(Width, Height);
			byte[] originalTexture = new byte[textureSize];
			Array.Copy(HeaderlessData, 0, originalTexture, 0, textureSize);
			outputStream.Write(originalTexture);
			// Encode mipmaps
			// Calculate the number of mip levels
			int mipLevels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
			MipmapImages = new Bitmap[mipLevels];
			MipmapImages[0] = new Bitmap(Image);
			// DDS mipmap order: from largest to smallest
			int mipLevel = 1;
			for (int size = Image.Width >> 1; size > 0; size >>= 1)
			{
				MipmapImages[mipLevel] = new Bitmap(Image, size, size);
				outputStream.Write(dataCodec.Encode(TextureFunctions.BitmapToRaw(MipmapImages[mipLevel]), size, size));
			}
			HasMipmaps = true;
			// Update raw data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}

		public override void RemoveMipmaps()
		{
			// If the texture already doesn't have mipmaps, do nothing
			if (!HasMipmaps)
				return;
			MemoryStream outputStream = new MemoryStream();
			// Set pixel and data codec
			PixelCodec pixelCodec = PixelCodec.GetPixelCodec(DdsFormat);
			DdsDataCodec dataCodec = DdsDataCodec.GetDataCodec(DdsFormat, pixelCodec, true);
			// Copy main texture
			int textureSize = dataCodec.CalculateTextureSize(Width, Height);
			byte[] originalTexture = new byte[textureSize];
			Array.Copy(HeaderlessData, 0, originalTexture, 0, textureSize);
			outputStream.Write(originalTexture);
			// Remove mipmaps
			MipmapImages = null;
			HasMipmaps = false;
			// Update raw data arrays
			HeaderlessData = outputStream.ToArray();
			RawData = GetBytes();
		}
	}
}