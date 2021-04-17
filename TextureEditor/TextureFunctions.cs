using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using VrSharp.Gvr;
using VrSharp.Pvr;

namespace TextureEditor
{
    public class TextureFunctions
    {

        private static void SetPixelIndex(Bitmap bmp, int x, int y, int paletteEntry)
        {
            BitmapData data = bmp.LockBits(new Rectangle(new Point(x, y), new Size(1, 1)), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format4bppIndexed);
            byte b = Marshal.ReadByte(data.Scan0);
            Marshal.WriteByte(data.Scan0, (byte)(b & 0xf | (paletteEntry << 4)));
            bmp.UnlockBits(data);
        }

        public static Color[] GenerateDefaultPalette()
        {
            List<Color> colors = new List<Color>();
            colors.Add(Color.FromArgb(0, 0, 0));
            colors.Add(Color.FromArgb(16, 16, 16));
            colors.Add(Color.FromArgb(32, 32, 32));
            colors.Add(Color.FromArgb(49, 48, 49));
            colors.Add(Color.FromArgb(65, 68, 65));
            colors.Add(Color.FromArgb(82, 85, 82));
            colors.Add(Color.FromArgb(98, 101, 98));
            colors.Add(Color.FromArgb(115, 117, 115));
            colors.Add(Color.FromArgb(139, 137, 139));
            colors.Add(Color.FromArgb(156, 153, 156));
            colors.Add(Color.FromArgb(172, 170, 172));
            colors.Add(Color.FromArgb(189, 186, 189));
            colors.Add(Color.FromArgb(205, 206, 205));
            colors.Add(Color.FromArgb(222, 222, 222));
            colors.Add(Color.FromArgb(238, 238, 238));
            colors.Add(Color.FromArgb(255, 255, 255));
            return colors.ToArray();
        }

        public static Bitmap ExportPalettedTexture(Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format4bppIndexed);
            Color[] colors = GenerateDefaultPalette();
            var newAliasForPalette = result.Palette; // Palette loaded from graphic device
            for (int i = 0; i < colors.Length; i++)
            {
                newAliasForPalette.Entries[i] = colors[i];
            }
            result.Palette = newAliasForPalette; // Palette data wrote back to the graphic device
            int remaining = bitmap.Height * bitmap.Width;
            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int p = 0; p < 16; p++)
                    {
                        if (bitmap.GetPixel(x, y).R == result.Palette.Entries[p].R)
                        {
                            remaining--;
                            SetPixelIndex(result, x, y, p);
                            break;
                        }
                    }
                }
            if (remaining > 0)
                System.Windows.Forms.MessageBox.Show(remaining.ToString() + " pixels were not found in the palette.", 
                    "Texture Editor Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            return result;
        }

        public static int GetAlphaLevelFromBitmap(Bitmap img)
        {
            BitmapData bmpd = img.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmpd.Stride;
            byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
            Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
            img.UnlockBits(bmpd);
            int tlevels = 0;
            for (int y = 0; y < img.Height; y++)
            {
                int srcaddr = y * Math.Abs(stride);
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
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

        public static PvrDataFormat GetPvrDataFormatFromBitmap(Bitmap image, bool mipmap)
        {
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

        public static GvrDataFormat GetGvrDataFormatFromBitmap(Bitmap image, bool hqGVM)
        {
            if (!hqGVM)
            {
                int tlevels = GetAlphaLevelFromBitmap(image);
                if (tlevels == 0)
                    return GvrDataFormat.Dxt1;
                else
                    return GvrDataFormat.Rgb5a3;
            }
            else
                return GvrDataFormat.Argb8888;
        }

        public static MemoryStream UpdateGBIX(MemoryStream stream, uint gbix, bool bigendian = false)
        {
            byte[] arr = stream.ToArray();
            for (int u = 0; u < arr.Length - 4; u++)
            {
                if (BitConverter.ToUInt32(arr, u) == 0x58494247) // GBIX
                {
                    byte[] value = BitConverter.GetBytes(gbix);
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

        public static bool CheckTextureDimensions(int width, int height)
        {
            if ((width != 0) && ((width & (width - 1)) == 0) && (height != 0) && ((height & (height - 1)) == 0) && width <= 1024 && height <= 1024)
                return true;
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid texture dimensions: " + width.ToString() + "x" + height.ToString() + ".\nPVR/GVR texture dimensions must be power of 2 and less than 1024. If you need higher resolution textures, use PVMX or PAK.", "Texture Editor Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
