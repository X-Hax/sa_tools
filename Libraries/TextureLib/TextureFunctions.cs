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
    public static partial class TextureFunctions
    {
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

		/*

		public static byte[] GetPaletteColorsFromIndexedBitmap(Bitmap bitmap, bool bigEndian)
        {
            int numColors = bitmap.Palette.Entries.Length;
            bool index8 = numColors <= 16;
            byte[] colorData = new byte[(index8 ? 256 : 16) * 4];
            for (int i = 0; i < bitmap.Palette.Entries.Length; i++)
            {
                byte[] color = BitConverter.GetBytes(bitmap.Palette.Entries[i].ToArgb());
                if (bigEndian) color.Reverse();
                Array.Copy(color, 0, colorData, i * 4, 4);
            }
            return colorData;
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

        /// <summary>
        /// Checks how many levels of transparency a Bitmap has.
        /// </summary>
        /// <returns>
        /// 0 if the Bitmap has only opaque pixels,
        /// 1 if the Bitmap has fully transparent and fully opaque pixels,
        /// 2 if the Bitmap contains partially transparent pixels.
        /// </returns>
        public static int GetAlphaLevelFromBitmap(Bitmap img)
        {
            Bitmap argb = (img.PixelFormat == PixelFormat.Format32bppArgb) ? img : new Bitmap(img);
            BitmapData bmpd = argb.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, argb.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmpd.Stride;
            byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
            Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
            argb.UnlockBits(bmpd);
            int tlevels = 0;
            for (int y = 0; y < argb.Height; y++)
            {
                int srcaddr = y * Math.Abs(stride);
                for (int x = 0; x < argb.Width; x++)
                {
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
                    if (c.A == 0)
                        tlevels = 1;
                    else if (c.A < 255)
                    {
                        tlevels = 2;
                        break;
                    }
                }
                if (tlevels == 2)
                    break;
            }
            return tlevels;
        }

        public static PvrDataFormat GetPvrDataFormatFromBitmap(Bitmap image, bool mipmap, bool includeIndexed)
        {
            switch (image.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    if (includeIndexed)
                        return PvrDataFormat.Index8;
                    break;
                case PixelFormat.Format4bppIndexed:
                    if (includeIndexed)
                        return PvrDataFormat.Index4;
                    break;
                default:
                    break;
            }
            if (image.Width == image.Height)
                if (mipmap)
                    return PvrDataFormat.SquareTwiddledMipmaps;
                else
                    return PvrDataFormat.SquareTwiddled;
            else
                return PvrDataFormat.Rectangle;
        }

        public static PvrPixelFormat GetPvrPixelFormatFromBitmap(Bitmap image)
        {
            int tlevels = GetAlphaLevelFromBitmap(image);
            switch (tlevels)
            {
                case 0:
                    return PvrPixelFormat.Rgb565;
                case 1:
                    return PvrPixelFormat.Argb1555;
                case 2:
                default:
                    return PvrPixelFormat.Argb4444;
            }
        }

        public static GvrDataFormat GetGvrDataFormatFromBitmap(Bitmap image, bool hqGVM, bool includeIndexed)
        {
            switch (image.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    if (includeIndexed)
                        return GvrDataFormat.Index8;
                    break;
                case PixelFormat.Format4bppIndexed:
                    if (includeIndexed)
                        return GvrDataFormat.Index4;
                    break;
                default:
                    break;
            }
            if (!hqGVM)
            {
                int tlevels = GetAlphaLevelFromBitmap(image);
                if (tlevels < 2)
                    return GvrDataFormat.Dxt1;
                else
                    return GvrDataFormat.Rgb5a3;
            }
            else
                return GvrDataFormat.Argb8888;
        }

        public static GvrPixelFormat GetGvrPixelFormatFromBitmap(Bitmap bmp)
        {
            int tlevels = GetAlphaLevelFromBitmap(bmp);
            switch (tlevels)
            {
                case 1:
                case 2:
                    return GvrPixelFormat.Rgb5a3;
                case 0:
                default:
                    return GvrPixelFormat.NonIndexed;
            }
        }
        public static DDSPixelFormat GetDDSPixelTypeFromBitmap(Bitmap bmp, bool useHQ)
        {
            int tlevels = GetAlphaLevelFromBitmap(bmp);
            if (useHQ)
            {
                return DDSPixelFormat.RGBA;
            }
            else
            {
                if (tlevels < 1)
                    return DDSPixelFormat.RGB;
                else
                    return DDSPixelFormat.RGBA;
            }
        }
        public static DDSPixelBitFormat GetDDSPixelFormatFromBitmap(Bitmap bmp, bool useHQ)
        {
            int tlevels = GetAlphaLevelFromBitmap(bmp);
            if (useHQ)
            {
                return DDSPixelBitFormat.ARGB8888;
            }
            else
            {
                switch (tlevels)
                {
                    case 0:
                    default:
                        return DDSPixelBitFormat.RGB565;
                    case 1:
                        return DDSPixelBitFormat.ARGB1555;
                    case 2:
                        return DDSPixelBitFormat.ARGB4444;
                }
            }
        }

        /// <summary>
        /// Looks for the GBIX header in a MemoryStream and sets the GBIX to the specified value. 
        /// <param name="stream">MemoryStream with data.</param>
        /// <param name="gbix">Global Index to set.</param>
        /// <param name="bigendian">Big Endian.</param>
        /// </summary>
        public static MemoryStream UpdateGBIX(MemoryStream stream, uint gbix, bool bigendian = false, bool xvr = false)
        {
            byte[] arr = stream.ToArray();
            byte[] value = BitConverter.GetBytes(gbix);
            // In XVR, there's no GBIX header and the GBIX is always at 0x10
            if (xvr)
            {
                arr[0x10] = value[0];
                arr[0x11] = value[1];
                arr[0x12] = value[2];
                arr[0x13] = value[3];
                return new MemoryStream(arr);
            }
            // In PVR or GVR, the GBIX header is not always in the same place so we have to look for it first
            for (int u = 0; u < arr.Length - 4; u++)
            {
                if (BitConverter.ToUInt32(arr, u) == 0x58494247) // GBIX
                {
                    if (bigendian)
                    {
                        arr[u + 11] = value[0];
                        arr[u + 10] = value[1];
                        arr[u + 9] = value[2];
                        arr[u + 8] = value[3];
                    }
                    else
                    {
                        arr[u + 8] = value[0];
                        arr[u + 9] = value[1];
                        arr[u + 10] = value[2];
                        arr[u + 11] = value[3];
                    }
                    return new MemoryStream(arr);
                }
            }
            System.Windows.Forms.MessageBox.Show("Unable to set the global index. The texture may be corrupted.");
            return stream;
        }

        /// <summary>
        /// Checks if the specified dimensions meet the requirements for PVR and GVR textures.
        /// </summary>
        public static bool CheckTextureDimensions(int width, int height)
        {
            if ((width != 0) && ((width & (width - 1)) == 0) && (height != 0) && ((height & (height - 1)) == 0) && width <= 1024 && height <= 1024)
                return true;
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid texture dimensions: " + width.ToString() + "x" + height.ToString() + ".\nPVR/GVR texture dimensions must be power of 2 and less than or equal to 1024. If you need higher resolution textures, use PVMX or PAK.", "Texture Editor Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        /*
        /// <summary>
        /// Identifies the bitmap file format contained in a byte array.
        /// </summary>
        public static TextureFileFormat IdentifyTextureFileFormat(byte[] file)
        {
            const ushort MagicBMP = 0x4D42;

            const uint MagicJPG = 0xE0FFD8FF;
            const uint MagicGIF = 0x38464947;
            const uint MagicPNG = 0x474E5089;
            const uint MagicDDS = 0x20534444;

            if (file == null || file.Length < 4)
                return TextureFileFormat.Invalid;

            if (PvrTexture.Is(file))
                return TextureFileFormat.PVR;
            if (GvrTexture.Is(file))
                return TextureFileFormat.GVR;
            if (XvrTexture.Is(file))
                return TextureFileFormat.XVR;
            if (BitConverter.ToUInt16(file, 0) == MagicBMP)
                return TextureFileFormat.BMP;

            return BitConverter.ToUInt32(file, 0) switch
            {
                MagicJPG => TextureFileFormat.JPG,
                MagicGIF => TextureFileFormat.GIF,
                MagicPNG => TextureFileFormat.PNG,
                MagicDDS => TextureFileFormat.DDS,
                _ => TextureFileFormat.Unknown,
            };
        }

        public static DDSPixelFormat IdentifyPAKPixelFormat(byte[] file)
        {
            return (DDSPixelFormat)BitConverter.ToUInt32(file, 0x50);
        }

        public static TextureFileFormat IdentifyTextureFileFormat(MemoryStream ms)
        {
            return ms == null ? TextureFileFormat.Invalid : IdentifyTextureFileFormat(ms.ToArray());
        }

        public static DDSPixelFormat IdentifyPAKPixelFormat(MemoryStream ms)
        {
            return ms == null ? DDSPixelFormat.Invalid : IdentifyPAKPixelFormat(ms.ToArray());
        }
        public static bool IdentifyDDSMipmapUsage(MemoryStream ms)
        {
            return ms == null ? false : IdentifyDDSMipmapUsage(ms.ToArray());
        }

        public static DDSPixelBitFormat IdentifyPAKPixelSubFormat(MemoryStream ms)
        {
            return ms == null ? DDSPixelBitFormat.Invalid : IdentifyPAKPixelSubFormat(ms.ToArray());
        }
        public static DDSPixelBitFormat IdentifyPAKPixelSubFormat(byte[] file)
        {
            byte compression = file[0x57];
            int bitdepth = BitConverter.ToInt32(file, 0x58);
            int r = BitConverter.ToInt32(file, 0x5C);
            int g = BitConverter.ToInt32(file, 0x60);
            int b = BitConverter.ToInt32(file, 0x64);
            int a = BitConverter.ToInt32(file, 0x68);
            DDSPixelBitFormat fmt = DDSPixelBitFormat.Invalid;
            if (IdentifyPAKPixelFormat(file) == DDSPixelFormat.FourCC)
            {
                switch (compression)
                {
                    case 0x31:
                        fmt = DDSPixelBitFormat.DXT1;
                        break;
                    case 0x32:
                        fmt = DDSPixelBitFormat.DXT2;
                        break;
                    case 0x33:
                        fmt = DDSPixelBitFormat.DXT3;
                        break;
                    case 0x34:
                        fmt = DDSPixelBitFormat.DXT4;
                        break;
                    case 0x35:
                        fmt = DDSPixelBitFormat.DXT5;
                        break;
                }
            }
            else if (IdentifyPAKPixelFormat(file) == DDSPixelFormat.RGB || IdentifyPAKPixelFormat(file) == DDSPixelFormat.RGBA)
            {
                switch (bitdepth)
                {
                    case 0:
                    default:
                        break;
                    case 8:
                    case 16:
                        if (a == 0 && r == 63488 && g == 2016)
                            fmt = DDSPixelBitFormat.RGB565;
                        else if (r == 31744 && g == 992)
                            fmt = DDSPixelBitFormat.ARGB1555;
                        else if (r == 3840 && g == 240 && a == 61440)
                            fmt = DDSPixelBitFormat.ARGB4444;
                        break;
                    case 24:
                        break;
                    case 32:
                        if (g != 65280)
                            break;
                        else
                        {
                            if (r != 16711680)
                            {
                                fmt = DDSPixelBitFormat.BGRA8888;
                            }
                            else
                            {
                                fmt = DDSPixelBitFormat.ARGB8888;
                            }
                        }
                        break;
                }
            }
            return fmt;
        }
        public static bool IdentifyDDSMipmapUsage(byte[] file)
        {
            DDSHeaderFlags HeaderFlags = (DDSHeaderFlags)BitConverter.ToUInt32(file, 8);
            return (HeaderFlags & DDSHeaderFlags.MipmapCount) != 0;
        }

        /*
        /// <summary>
        /// Returns the extension string by identifying texture file format in a byte array.
        /// </summary>
        public static string IdentifyTextureFileExtension(byte[] file)
        {
            switch (IdentifyTextureFileFormat(file))
            {
                case TextureFileFormat.BMP:
                    return ".bmp";
                case TextureFileFormat.GIF:
                    return ".gif";
                case TextureFileFormat.JPG:
                    return ".jpg";
                case TextureFileFormat.PNG:
                    return ".png";
                case TextureFileFormat.DDS:
                    return ".dds";
                case TextureFileFormat.PVR:
                    return ".pvr";
                case TextureFileFormat.GVR:
                    return ".gvr";
                case TextureFileFormat.XVR:
                    return ".xvr";
            }
            throw new Exception("Unknown texture file format");
        }

        public static string IdentifyTextureFileExtension(MemoryStream ms)
        {
            if (ms == null)
                throw new Exception("Memory stream is null");
            return IdentifyTextureFileExtension(ms.ToArray());
        }

        public static bool CheckIfTextureIsDDS(byte[] file)
        {
            return IdentifyTextureFileFormat(file) == TextureFileFormat.DDS;
        }
        */
    }
}