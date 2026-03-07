using System;
using System.Drawing;
using System.IO;

namespace TextureLib
{
	public partial class DdsTexture
	{
		/// <summary>
		/// Determine the most optimal DDS format to store the image data of the specified Bitmap.
		/// </summary>
		/// <param name="image">Bitmap to analyze.</param>
		/// <param name="maxQuality">Whether to prefer RGB888, ARGB8888 and higher quality DXT compression to lossy codecs and DXT1/3.</param>
		/// <param name="useDxt">Whether to consider DXT1, DXT3 or DXT5 formats.</param>
		/// <returns>Recommended DDS texture format.</returns>
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
			int mipWidth = Math.Max(1, Width >> 1);
			int mipHeight = Math.Max(1, Height >> 1);
			// DDS mipmap order: from largest to smallest
			for (int mipLevel = 1; mipLevel < mipLevels; mipLevel++)
			{
				MipmapImages[mipLevel] = new Bitmap(Image, mipWidth, mipHeight);
				outputStream.Write(dataCodec.Encode(TextureFunctions.BitmapToRaw(MipmapImages[mipLevel]), mipWidth, mipHeight));
				mipWidth = Math.Max(1, mipWidth >>= 1);
				mipHeight = Math.Max(1, mipHeight >>= 1);
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