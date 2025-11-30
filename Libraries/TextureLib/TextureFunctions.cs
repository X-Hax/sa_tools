using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace TextureLib
{
    public static partial class TextureFunctions
    {
        public static unsafe byte[] BitmapToRawIndexed(Bitmap img)
        {
            byte[] destination = new byte[img.Width * img.Height];

            int bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            //Console.WriteLine("Bpp: {0}", bitsPerPixel);

            // Copy over the data to the destination. We need to use Stride in this case, as it may not
            // always be equal to Width.
            BitmapData bitmapData = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

            byte* pointer = (byte*)bitmapData.Scan0;

            if (bitsPerPixel == 1)
            {
                for (int y = 0; y < bitmapData.Height; y++)
                {
                    for (int x = 0; x < bitmapData.Width; x++)
                    {
                        destination[(y * img.Width) + x] = (byte)((pointer[(y * bitmapData.Stride) + (x >> 3)] >> (7 - (x % 8))) & 0x1);
                    }
                }
            }
            else if (bitsPerPixel == 4)
            {
                for (int y = 0; y < bitmapData.Height; y++)
                {
                    for (int x = 0; x < bitmapData.Width; x++)
                    {
                        byte paletteIndex;
                        if (x % 2 == 0)
                        {
                            paletteIndex = (byte)(pointer[(y * bitmapData.Stride) + (x >> 1)] >> 4);
                        }
                        else
                        {
                            paletteIndex = (byte)(pointer[(y * bitmapData.Stride) + (x >> 1)] & 0xF);
                        }

                        destination[(y * img.Width) + x] = (byte)((paletteIndex >> 4) | (paletteIndex << 4)); // Hotfixed for GVR Index4 codec expecting the whole byte
                    }
                }
            }
            else
            {
                for (int y = 0; y < bitmapData.Height; y++)
                {
                    for (int x = 0; x < bitmapData.Width; x++)
                    {
                        destination[(y * img.Width) + x] = pointer[(y * bitmapData.Stride) + x];
                    }
                }
            }

            img.UnlockBits(bitmapData);
            return destination;
        }

        public static Bitmap QuantizeImage(Bitmap bitmap, bool index8, bool dither = false, int mipWidth = 0)
        {
            // Save bitmap to stream and reset the stream
            MemoryStream bitmapStream = new MemoryStream();
            bitmap.Save(bitmapStream, ImageFormat.Png);
            bitmapStream.Seek(0, SeekOrigin.Begin);
            // Load ImageSharp image
            SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(bitmapStream);
            // Set up the quantizer
            var quantizer = new WuQuantizer(new QuantizerOptions
            {
                Dither = dither ? QuantizerConstants.DefaultDither : null,
                MaxColors = index8 ? 256 : 16,
            });
            // Resize if mipmap
            if (mipWidth != 0)
                image.Mutate(x => x.Resize(mipWidth, mipWidth, KnownResamplers.NearestNeighbor));
            // Quantize the ImageSharp image
            image.Mutate(x => x.Quantize(quantizer));
            // Save the ImageSharp image as a PNG into a stream and reset the stream
            MemoryStream endStream = new MemoryStream();
            image.Save(endStream, new PngEncoder() { ColorType = PngColorType.Palette, BitDepth = index8 ? PngBitDepth.Bit8 : PngBitDepth.Bit4 });
            endStream.Seek(0, SeekOrigin.Begin);
            // Create the Bitmap from the stream, but not using the regular functions because of .NET bugs (sigh)
            return BitmapHandler.LoadBitmap(endStream.ToArray());
        }
        /// <summary>
        /// Sorts the colors in a palette by luminance into a new palette.
        /// </summary>
        /// <param name="palette">The palette to sort the colors of.</param>
        /// <returns>A new palette with the sorted colors.</returns>
        public static TexturePalette SortByLuminance(this TexturePalette palette)
        {
            (int, byte)[] luminanceLUT = new (int, byte)[palette.GetNumColors()];
            ReadOnlySpan<byte> data = palette.DecodedData;

            for (int i = 0; i < luminanceLUT.Length; i++)
            {
                ReadOnlySpan<byte> color = data[(i * 4)..];
                luminanceLUT[i] = (i, GetLuminance(color));
            }

            Array.Sort(luminanceLUT, (a, b) => a.Item2.CompareTo(b.Item2));

            byte[] newPalette = new byte[data.Length];
            Span<byte> destination = newPalette;
            for (int i = 0; i < luminanceLUT.Length; i++)
            {
                int dstIndex = luminanceLUT[i].Item1;
                data.Slice(luminanceLUT[i].Item1 * 4, 4).CopyTo(destination[(i * 4)..]);
            }

            return new(newPalette, new ARGB8888PixelCodec(), palette.GetNumColors());
        }

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

        public static void RawToBitmap(Bitmap image, byte[] rawData)
        {
            BitmapData bitmapData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);
            Marshal.Copy(rawData, 0, bitmapData.Scan0, rawData.Length);
            image.UnlockBits(bitmapData);
        }

        public static byte[] BitmapToRaw(Bitmap source)
        {
            byte[] destination = new byte[source.Width * source.Height * 4];
            BitmapToRaw(source, destination);
            return destination;
        }

        public static void BitmapToRaw(Bitmap img, byte[] destination)
        {
            // If this is not a 32-bit ARGB bitmap, convert it to one
            if (img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                //Console.WriteLine("Bitmap to raw 32");
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
        }
        
        public static void RGBAtoBGRA(byte[] pixelData)
        {
            // Assuming 4 bytes per pixel (R, G, B, A)
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                // Swap R and B bytes
                byte temp = pixelData[i];     // Store R
                pixelData[i] = pixelData[i + 2]; // R becomes B
                pixelData[i + 2] = temp;     // B becomes original R
            }
        }

        public static byte GetLuminance(byte red, byte green, byte blue)
        {
            return (byte)((0.2126f * red) + (0.7152f * green) + (0.0722f * blue));
        }

        public static byte GetLuminance(ReadOnlySpan<byte> color)
        {
            return GetLuminance(color[0], color[1], color[2]);
        }

        /// <summary>
		/// Checks if a signed integer is a power of 2.
		/// </summary>
		/// <param name="number">Number to check.</param>
		/// <returns>Whether the number is a power of 2.</returns>
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
        */

        public static DDSPixelFormat IdentifyPAKPixelFormat(byte[] file)
        {
            return (DDSPixelFormat)BitConverter.ToUInt32(file, 0x50);
        }

        /*
        public static TextureFileFormat IdentifyTextureFileFormat(MemoryStream ms)
        {
            return ms == null ? TextureFileFormat.Invalid : IdentifyTextureFileFormat(ms.ToArray());
        }
        */

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
