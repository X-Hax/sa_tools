using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TextureLib
{
    // Class for BMP, GIF, JPG and PNG textures
    public class GdiTexture : GenericTexture
    {
        public PixelFormat GdiPixelFormat;

        public GdiTexture(byte[] data, int offset = 0, string name = null)
        {
            InitTexture(data, offset, name);
            // Init Bitmap
            using (var ms = new MemoryStream(RawData))
            {
                Bitmap b = new Bitmap(ms);
                Image = (Bitmap)b.Clone();
                b.Dispose();
            }
            // Set texture properties
            Width = Image.Width;
            Height = Image.Height;
            HasMipmaps = false;
            RequiresPaletteFile = false;
            GdiPixelFormat = Image.PixelFormat;
            switch (Image.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed: // Probably won't work
                    Indexed = true;
                    Palette = TexturePalette.FromIndexedBitmap(Image);
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
        }

		// This function creates mipmaps from the original image
		private void GenerateMipmaps()
		{
			// Calculate mipmap levels based on texture dimensions
			int levels = (int)Math.Floor(Math.Log2(Math.Max(Width, Height))) + 1;
			MipmapImages = new Bitmap[levels];
			// Set initial width before division
			int mipWidth = Width;
			int mipHeight = Height;
			// Generate individual mipmaps
			for (int m = 0; m < levels; m++)
			{
				// Divide original or previous dimensions by two for each mipmap
				mipWidth = mipWidth / 2;
				mipHeight = mipHeight / 2;
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

		public override byte[] GetBytes()
		{
			MemoryStream ms = new();
			Image.Save(ms, ImageFormat.Png);
			return ms.ToArray();
		}
    }
}