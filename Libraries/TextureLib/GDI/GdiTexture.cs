using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace TextureLib
{
	// Class for BMP, GIF, JPG and PNG textures
	public class GdiTexture : GenericTexture
	{
		/// <summary>Pixel format of the Image (GDI).</summary>
		public PixelFormat GdiPixelFormat;

		/// <summary>
		/// Initializes a GDI texture from a byte array that contains a BMP/JPG/GIF/PNG header and data.
		/// </summary>
		/// <param name="data">Byte array containing texture data.</param>
		/// <param name="offset">Offset where texture data begins.</param>
		/// <param name="mipmaps">Whether to generate mipmaps or not.</param>
		/// <param name="gbix">Global Index (optional).</param>
		/// <param name="name">Texture name (optional).</param>
		public GdiTexture(byte[] data, int offset = 0, bool mipmaps = false, uint gbix = 0, string name = null)
		{
			InitTexture(data, offset, name);
			Decode();
			Gbix = gbix;
			SetTextureProperties();
			if (mipmaps)
				AddMipmaps();
		}

		/// <summary>
		/// Creates a GDI texture from a Bitmap.
		/// </summary>
		/// <param name="texture">Source Bitmap.</param>
		/// <param name="mipmaps">Whether to generate mipmaps or not.</param>
		/// <param name="gbix">Global Index (optional).</param>
		/// <param name="name">Texture name (optional).</param>
		public GdiTexture(Bitmap texture, bool mipmaps = false, uint gbix = 0, string name = null)
		{
			Image = new Bitmap(texture);
			Gbix = gbix;
			Name = name;
			HasMipmaps = mipmaps;
			SetTextureProperties();
			Encode();
		}

		public override byte[] GetBytes()
		{
			MemoryStream ms = new();
			Image.Save(ms, ImageFormat.Png);
			return ms.ToArray();
		}

		public override void Decode()
		{
			LoadBitmapFromRawData();
			SetTextureProperties();
			if (HasMipmaps)
				AddMipmaps();
		}

		public override void Encode()
		{
			Width = Image.Width;
			Height = Image.Height;
			if (HasMipmaps)
				AddMipmaps();
			RawData = GetBytes();
		}

		/// <summary>Sets texture properties such as width etc. based on the Image parameters.</summary>
		private void SetTextureProperties()
		{
			Width = Image.Width;
			Height = Image.Height;
			switch (Image.PixelFormat)
			{
				case PixelFormat.Format1bppIndexed: // This probably won't work so it should be converted to ARGB8888
					Bitmap newImage = new Bitmap(Image.Width, Image.Height, PixelFormat.Format32bppArgb);
					using (Graphics g = Graphics.FromImage(newImage))
						g.DrawImage(Image, 0, 0, Image.Width, Image.Height);
					Indexed = false;
					Image = newImage;
					break;
				case PixelFormat.Format4bppIndexed:
					Indexed = true;
					Palette = TexturePalette.FromIndexedBitmap(Image);
					break;
				case PixelFormat.Format8bppIndexed:
					Indexed = true;
					Palette = TexturePalette.FromIndexedBitmap(Image);
					break;
				default:
					Indexed = false;
					break;
			}
			GdiPixelFormat = Image.PixelFormat;
		}

		/// <summary>Retrieves the Image from raw data.</summary>
		private void LoadBitmapFromRawData()
		{
			using (var ms = new MemoryStream(RawData))
			{
				Bitmap b = new Bitmap(ms);
				Image = (Bitmap)b.Clone();
				b.Dispose();
			}
		}

		public override void AddMipmaps()
		{
			HasMipmaps = true;
			// Calculate mipmap levels based on texture dimensions
			int levels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
			MipmapImages = new Bitmap[levels];
			// Set initial width before division
			int mipWidth = Width;
			int mipHeight = Height;
			// Generate individual mipmaps
			for (int m = 0; m < levels - 1; m++)
			{
				// Divide original or previous dimensions by two for each mipmap
				mipWidth = Math.Max(1, mipWidth >>= 1); // The Max check is necessary for rectangular mipmaps
				mipHeight = Math.Max(1, mipHeight >>= 1);
				Bitmap mip = new Bitmap(mipWidth, mipHeight);
				// Write mipmap image onto the bitmap from largest to smallest
				using (Graphics gfx = Graphics.FromImage(mip))
				{
					gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
					gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
					gfx.DrawImage(Image, 0, 0, mipWidth, mipHeight);
				}
				// Save bitmap to the dictionary
				MipmapImages[m] = mip;
			}
		}

		public override void RemoveMipmaps()
		{
			HasMipmaps = false;
			MipmapImages = null;
		}

		public GdiTexture Clone()
		{
			return new GdiTexture(RawData, 0, HasMipmaps, Gbix, Name) { PakMetadata = PakMetadata, PvmxOriginalDimensions = PvmxOriginalDimensions };
		}

		public override bool CanHaveMipmaps()
		{
			return false;
		}

		public static bool Identify(byte[] data, int offset = 0)
		{
			const ushort MagicBMP = 0x4D42;
			const uint MagicJPG = 0xE0FFD8FF;
			const uint MagicGIF = 0x38464947;
			const uint MagicPNG = 0x474E5089;
			
			if (BitConverter.ToUInt16(data, 0) == MagicBMP)
				return true;
			else if (BitConverter.ToUInt32(data, offset) == MagicPNG)
				return true;
			else if (BitConverter.ToUInt32(data, offset) == MagicJPG)
				return true;
			else if (BitConverter.ToUInt32(data, offset) == MagicGIF)
				return true;
			return false;
		}

		public override string Info()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("GDI TEXTURE INFO");
			sb.AppendLine("Width: " + Width.ToString());
			sb.AppendLine("Height: " + Height.ToString());
			sb.AppendLine("Pixel format: " + GdiPixelFormat.ToString());
			sb.AppendLine("Mipmaps: " + HasMipmaps.ToString());			
			return sb.ToString();
		}
	}
}