using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using VrSharp.Gvr;
using VrSharp.Pvr;
using VrSharp.Xvr;

// Various additional texture related functions to avoid cluttering the MainForm

namespace TextureEditor
{
	public static class TextureFunctions
	{
		public enum TextureFileFormat { BMP, JPG, GIF, PNG, DDS, PVR, GVR, XVR, Unknown, Invalid }

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
					BitmapData data8 = bmp.LockBits(new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
					int offset = y * data8.Stride + (x);
					Marshal.WriteByte(data8.Scan0, offset, (byte)pixelIndex);
					bmp.UnlockBits(data8);
					return;
				case PixelFormat.Format4bppIndexed:
					BitmapData data4 = bmp.LockBits(new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);
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
			BitmapData bmpd = argb.LockBits(new Rectangle(Point.Empty, argb.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
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
				if (tlevels == 0)
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

		public static TextureFileFormat IdentifyTextureFileFormat(MemoryStream ms)
		{
			return ms == null ? TextureFileFormat.Invalid : IdentifyTextureFileFormat(ms.ToArray());
		}

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
	}
}