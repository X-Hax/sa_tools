using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

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
    }
}