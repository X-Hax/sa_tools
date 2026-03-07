using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
	public enum BitmapAlphaLevel
	{
		None = 0,
		OneBitAlpha = 1,
		FullAlpha = 2
	}

	public static partial class TextureFunctions
    {

		/// <summary>
		/// Checks how many levels of transparency a Bitmap has.
		/// </summary>
		/// <returns>
		/// 0 if the Bitmap has only opaque pixels,
		/// 1 if the Bitmap has fully transparent and fully opaque pixels,
		/// 2 if the Bitmap contains partially transparent pixels.
		/// </returns>
		public static BitmapAlphaLevel GetAlphaLevelFromBitmap(Bitmap img)
		{
			Bitmap argb = (img.PixelFormat == PixelFormat.Format32bppArgb) ? img : new Bitmap(img);
			BitmapData bmpd = argb.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, argb.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			int stride = bmpd.Stride;
			byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
			Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
			argb.UnlockBits(bmpd);
			BitmapAlphaLevel tlevel = BitmapAlphaLevel.None;
			for (int y = 0; y < argb.Height; y++)
			{
				int srcaddr = y * Math.Abs(stride);
				for (int x = 0; x < argb.Width; x++)
				{
					System.Drawing.Color c = System.Drawing.Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
					if (c.A == 0)
						tlevel = BitmapAlphaLevel.OneBitAlpha;
					else if (c.A < 255)
					{
						tlevel = BitmapAlphaLevel.FullAlpha;
						break;
					}
				}
				if (tlevel == BitmapAlphaLevel.FullAlpha)
					break;
			}
			return tlevel;
		}

		/// <summary>
		/// Encodes a bitmap with the specified data/pixel codec, then decodes it back with degraded colors to use as the base of an indexed image.
		/// </summary>
		/// <param name="texture">Bitmap to encode.</param>
		/// <param name="codec">PVR or GVR data codec to use.</param>
		/// <returns>Reencoded Bitmap.</returns>
		public static Bitmap CalculateLossyForPaletteOrVq(Bitmap texture, DataCodec codec)
		{
			byte[] encoded = codec.Encode(TextureFunctions.BitmapToRaw(texture), texture.Width, texture.Height);
			byte[] decoded = codec.Decode(encoded, texture.Width, texture.Height, null);
			Bitmap output = new Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			TextureFunctions.RawToBitmap(output, decoded);
			return output;
		}

		/// <summary>
		/// Encodes a mipmap, optionally with a quantizer.
		/// </summary>
		/// <param name="image">ImageSharp image to encode.</param>
		/// <param name="quantizer">Quantizer to use (optional).</param>
		/// <param name="codec">Data codec to use.</param>
		/// <param name="size">Size of the mipmap.</param>
		/// <param name="writer">Output memory stream.</param>
		public static void EncodeMipMap(Image<Rgba32> image, IQuantizer<Rgba32>? quantizer, DataCodec codec, int size, MemoryStream writer)
		{
			byte[] mipMapPixels;
			Image<Rgba32> mipMapImage = image.Clone();
			mipMapImage.Mutate(x => x.Resize(size, size));
			// If there is a quantizer, use it.
			if (quantizer != null)
			{
				IndexedImageFrame<Rgba32> mipMapFrame = quantizer.QuantizeFrame(mipMapImage.Frames[0], new(0, 0, size, size));
				mipMapPixels = new byte[Math.Max(size * size, 2)];
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
			writer.Write(codec.Encode(mipMapPixels, size, size));
		}

		/// <summary>Converts a Bitmap to an ImageSharp image.</summary>
		public static SixLabors.ImageSharp.Image<Rgba32> BitmapToImageSharp(Bitmap bitmap)
		{
			// Create raw bitmap data array compatible with ImageSharp
			byte[] rawBitmap = BitmapToRaw(bitmap);
			// Load ImageSharp image
			return SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(rawBitmap, bitmap.Width, bitmap.Height);
		}

		/// <summary>Quantizes a Bitmap using a specified palette or WuQuantizer, outputs raw indexed bytes and a TexturePalette.</summary>
		public static byte[] QuantizeImage(Bitmap bitmap, bool index8, out TexturePalette outputPalette, bool dither = false, TexturePalette inputPalette = null)
        {
			// Convert Bitmap to ImageSharp
			SixLabors.ImageSharp.Image<Rgba32> image = BitmapToImageSharp(bitmap);
			// Set the quantizer
			IQuantizer quantizer = inputPalette != null ? TexturePalette.CreatePaletteQuantizer(inputPalette, inputPalette.GetNumColors(), 0, dither):
				new WuQuantizer(new QuantizerOptions { Dither = dither ? QuantizerConstants.DefaultDither : null, MaxColors = index8 ? 256 : 16 });
			// Create the specific quantizer
			IQuantizer<Rgba32> iquant = quantizer.CreatePixelSpecificQuantizer<Rgba32>(SixLabors.ImageSharp.Configuration.Default);
			// Quantize the image frame
			IndexedImageFrame<Rgba32> imageFrame = iquant.BuildPaletteAndQuantizeFrame(image.Frames[0], new(0, 0, image.Width, image.Height));
			byte[] quantizedPixels = new byte[image.Width * image.Height];
			// Transfer pizels
			Span<byte> pixelData = quantizedPixels;
			for (int y = 0; y < image.Height; y++)
				imageFrame.DangerousGetRowSpan(y).CopyTo(pixelData[(y * image.Width)..]);
			// Get palette bytes
			byte[] copyPalette = MemoryMarshal.Cast<Rgba32, byte>(iquant.Palette.Span).ToArray();
			// Expand the palette to 16 or 256 colors
			byte[] fillPalette = new byte[4 * (index8 ? 256 : 16)];
			Array.Copy(copyPalette, 0, fillPalette, 0, copyPalette.Length);
			// Create the output palette
			outputPalette = new TexturePalette(fillPalette, new ARGB8888PixelCodec(), index8 ? 256:16, bigEndian: false);
			return quantizedPixels;
		}

		/// <summary>Writes raw pixels from a byte array into a Bitmap with conversion.</summary>
		public static void RawToBitmap(Bitmap image, byte[] rawData)
		{
			// Copy the original array
			byte[] targetData = new byte[rawData.Length];
			Array.Copy(rawData, targetData, rawData.Length);
			// Convert for Windows bitmap byte order
			for (int i = 0; i < targetData.Length; i += 4)
			{
				// Swap R and B bytes
				byte temp = targetData[i];     // Store R
				targetData[i] = targetData[i + 2]; // R becomes B
				targetData[i + 2] = temp;     // B becomes original R
			}
			// Write to bitmap data
			BitmapData bitmapData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);
			Marshal.Copy(targetData, 0, bitmapData.Scan0, targetData.Length);
			image.UnlockBits(bitmapData);
		}

		public static void BitmapToRaw(Bitmap img, byte[] destination)
		{
			// If this is not a 32-bit ARGB bitmap, convert it to one
			if (img.PixelFormat != PixelFormat.Format32bppArgb)
			{
				Bitmap newImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
				using (Graphics g = Graphics.FromImage(newImage))
				{
					g.DrawImage(img, 0, 0, img.Width, img.Height);
				}
				img = newImage;
			}
			// Copy over the data to the destination. It's ok to do it without utilizing Stride
			// since each pixel takes up 4 bytes (aka Stride will always be equal to Width)
			BitmapData bitmapData = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
			Marshal.Copy(bitmapData.Scan0, destination, 0, destination.Length);
			img.UnlockBits(bitmapData);
			// Convert from Windows byte order
			for (int i = 0; i < destination.Length; i += 4)
			{
				// Swap R and B bytes
				byte temp = destination[i];     // Store R
				destination[i] = destination[i + 2]; // R becomes B
				destination[i + 2] = temp;     // B becomes original R
			}
		}

		public static byte[] BitmapToRaw(Bitmap source)
		{
			byte[] destination = new byte[source.Width * source.Height * 4];
			BitmapToRaw(source, destination);
			return destination;
		}

		/// <summary>Gets the luminance value of a pixel./// </summary>
		public static byte GetLuminance(byte red, byte green, byte blue)
		{
			return (byte)((0.2126f * red) + (0.7152f * green) + (0.0722f * blue));
		}

		/// <summary>Gets the luminance value of a pixel from a 3-byte array./// </summary>
		public static byte GetLuminance(ReadOnlySpan<byte> color)
		{
			return GetLuminance(color[0], color[1], color[2]);
		}

		/// <summary>Checks if a signed integer is a power of 2./// </summary>
		public static bool IsPow2(int number)
		{
			return (number & (number - 1)) == 0 && number > 0;
		}

		/// <summary>
		/// Manipulates pixel data in indexed Bitmaps. 
		/// <param name="bmp">Indexed Bitmap to modify.</param>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="pixelIndex">Palette color ID to set.</param>
		/// </summary>
		public static void SetPixelIndex(Bitmap bmp, int x, int y, int pixelIndex)
		{
			switch (bmp.PixelFormat)
			{
				case PixelFormat.Format8bppIndexed:
					BitmapData data8 = bmp.LockBits(new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), new System.Drawing.Size(bmp.Width, bmp.Height)), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
					int offset = y * data8.Stride + (x);
					Marshal.WriteByte(data8.Scan0, offset, (byte)pixelIndex);
					bmp.UnlockBits(data8);
					return;
				case PixelFormat.Format4bppIndexed:
					BitmapData data4 = bmp.LockBits(new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), new System.Drawing.Size(bmp.Width, bmp.Height)), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);
					// Bit index
					int biti = (data4.Stride > 0 ? y : y - bmp.Height + 1) * data4.Stride * 8 + x * 4;
					// Pixel index
					int i = biti / 8;
					// Retrieve byte
					byte b = Marshal.ReadByte(data4.Scan0, i);
					// Write byte
					if (biti % 8 == 0)
					{
						Marshal.WriteByte(data4.Scan0, i, (byte)(b & 0xf | (pixelIndex << 4)));
					}
					else
					{
						Marshal.WriteByte(data4.Scan0, i, (byte)(b & 0xf0 | pixelIndex));
					}
					bmp.UnlockBits(data4);
					return;
				default:
					return;
			}
		}
	}
}