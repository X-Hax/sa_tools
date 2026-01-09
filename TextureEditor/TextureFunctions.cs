using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TextureLib;

// TODO: Move to TextureLib, MainForm or remove

namespace TextureEditor
{
	public static class TextureFunctions
	{
		public enum PalettedTextureFormat { NotIndexed, Index4, Index8, Index14 };

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

			if (PvrTexture.Identify(file))
				return TextureFileFormat.PVR;
			if (GvrTexture.Identify(file))
				return TextureFileFormat.GVR;
			if (XvrTexture.Identify(file))
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

		public static string GetSurfaceFlagsString(NinjaSurfaceFlags njflags)
		{
			List<string> flags = new List<string>();
			if ((njflags & NinjaSurfaceFlags.NotTwiddled) != 0)
				flags.Add("Not Twiddled");
			else
				flags.Add("Twiddled");
			if ((njflags & NinjaSurfaceFlags.Mipmapped) != 0)
				flags.Add("Mipmapped");
			if ((njflags & NinjaSurfaceFlags.Palettized) != 0)
				flags.Add("Palettized");
			if ((njflags & NinjaSurfaceFlags.Stride) != 0)
				flags.Add("Stride");
			if ((njflags & NinjaSurfaceFlags.VQ) != 0)
				flags.Add("VQ");
			return string.Join(", ", flags);
		}

	}
}