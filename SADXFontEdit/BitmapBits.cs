using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SADXFontEdit
{
    /// <summary>
    /// Represents the pixel data of a 1bpp Bitmap.
    /// </summary>
    public class BitmapBits
    {
        public bool[] Bits { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
        }

        public bool this[int x, int y]
        {
            get
            {
                return Bits[(y * Width) + x];
            }
            set
            {
                Bits[(y * Width) + x] = value;
            }
        }

        public BitmapBits(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new bool[width * height];
        }

        public BitmapBits(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format1bppIndexed)
                throw new ArgumentException();
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Width = bmpd.Width;
            Height = bmpd.Height;
            byte[] tmpbits = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
            Marshal.Copy(bmpd.Scan0, tmpbits, 0, tmpbits.Length);
            Bits = new bool[Width * Height];
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(bmpd.Stride);
                for (int x = 0; x < bmp.Width; x += 8)
                {
                    this[x + 0, y] = ((tmpbits[srcaddr + (x / 8)] >> 7) & 1) == 1;
                    this[x + 1, y] = ((tmpbits[srcaddr + (x / 8)] >> 6) & 1) == 1;
                    this[x + 2, y] = ((tmpbits[srcaddr + (x / 8)] >> 5) & 1) == 1;
                    this[x + 3, y] = ((tmpbits[srcaddr + (x / 8)] >> 4) & 1) == 1;
                    this[x + 4, y] = ((tmpbits[srcaddr + (x / 8)] >> 3) & 1) == 1;
                    this[x + 5, y] = ((tmpbits[srcaddr + (x / 8)] >> 2) & 1) == 1;
                    this[x + 6, y] = ((tmpbits[srcaddr + (x / 8)] >> 1) & 1) == 1;
                    this[x + 7, y] = (tmpbits[srcaddr + (x / 8)] & 1) == 1;
                }
            }
            bmp.UnlockBits(bmpd);
        }

        public BitmapBits(BitmapBits source)
        {
            Width = source.Width;
            Height = source.Height;
            Bits = new bool[source.Bits.Length];
            Array.Copy(source.Bits, Bits, Bits.Length);
        }

        public BitmapBits(byte[] file, int address, bool reverse)
        {
            Width = Height = 24;
            Bits = new bool[24 * 24];
            for (int y = 0; y < 24; y++)
            {
                int srcaddr = y * 3 + address;
                for (int x = 0; x < 24; x += 8)
                {
					if (reverse)
					{
						this[x + 7, y] = ((file[srcaddr + (x / 8)] >> 7) & 1) == 1;
						this[x + 6, y] = ((file[srcaddr + (x / 8)] >> 6) & 1) == 1;
						this[x + 5, y] = ((file[srcaddr + (x / 8)] >> 5) & 1) == 1;
						this[x + 4, y] = ((file[srcaddr + (x / 8)] >> 4) & 1) == 1;
						this[x + 3, y] = ((file[srcaddr + (x / 8)] >> 3) & 1) == 1;
						this[x + 2, y] = ((file[srcaddr + (x / 8)] >> 2) & 1) == 1;
						this[x + 1, y] = ((file[srcaddr + (x / 8)] >> 1) & 1) == 1;
						this[x + 0, y] = (file[srcaddr + (x / 8)] & 1) == 1;
					}
					else
					{
						this[x + 0, y] = ((file[srcaddr + (x / 8)] >> 7) & 1) == 1;
						this[x + 1, y] = ((file[srcaddr + (x / 8)] >> 6) & 1) == 1;
						this[x + 2, y] = ((file[srcaddr + (x / 8)] >> 5) & 1) == 1;
						this[x + 3, y] = ((file[srcaddr + (x / 8)] >> 4) & 1) == 1;
						this[x + 4, y] = ((file[srcaddr + (x / 8)] >> 3) & 1) == 1;
						this[x + 5, y] = ((file[srcaddr + (x / 8)] >> 2) & 1) == 1;
						this[x + 6, y] = ((file[srcaddr + (x / 8)] >> 1) & 1) == 1;
						this[x + 7, y] = (file[srcaddr + (x / 8)] & 1) == 1;
					}
                }
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap newbmp = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);
            BitmapData newbmpd = newbmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            byte[] bmpbits = new byte[Math.Abs(newbmpd.Stride) * newbmpd.Height];
            for (int y = 0; y < Height; y++)
            {
                int srcaddr = y * Math.Abs(newbmpd.Stride);
                for (int x = 0; x < Width; x += 8)
                {
                    byte pix = (byte)(this[x + 7, y] ? 1 : 0);
                    pix |= (byte)((this[x + 6, y] ? 1 : 0) << 1);
                    pix |= (byte)((this[x + 5, y] ? 1 : 0) << 2);
                    pix |= (byte)((this[x + 4, y] ? 1 : 0) << 3);
                    pix |= (byte)((this[x + 3, y] ? 1 : 0) << 4);
                    pix |= (byte)((this[x + 2, y] ? 1 : 0) << 5);
                    pix |= (byte)((this[x + 1, y] ? 1 : 0) << 6);
                    pix |= (byte)((this[x, y] ? 1 : 0) << 7);
                    bmpbits[srcaddr + (x / 8)] = pix;
                }
            }
            Marshal.Copy(bmpbits, 0, newbmpd.Scan0, bmpbits.Length);
            newbmp.UnlockBits(newbmpd);
            return newbmp;
        }

        public Bitmap ToBitmap(ColorPalette palette)
        {
            Bitmap newbmp = ToBitmap();
            newbmp.Palette = palette;
            return newbmp;
        }

        public Bitmap ToBitmap(params Color[] palette)
        {
            Bitmap newbmp = ToBitmap();
            ColorPalette pal = newbmp.Palette;
            for (int i = 0; i < Math.Min(palette.Length, 256); i++)
                pal.Entries[i] = palette[i];
            newbmp.Palette = pal;
            return newbmp;
        }

        public byte[] GetBytes()
        {
            byte[] bmpbits = new byte[3 * 24];
            for (int y = 0; y < Height; y++)
            {
                int srcaddr = y * 3;
                for (int x = 0; x < Width; x += 8)
                {
                    byte pix = (byte)(this[x + 7, y] ? 1 : 0);
                    pix |= (byte)((this[x + 6, y] ? 1 : 0) << 1);
                    pix |= (byte)((this[x + 5, y] ? 1 : 0) << 2);
                    pix |= (byte)((this[x + 4, y] ? 1 : 0) << 3);
                    pix |= (byte)((this[x + 3, y] ? 1 : 0) << 4);
                    pix |= (byte)((this[x + 2, y] ? 1 : 0) << 5);
                    pix |= (byte)((this[x + 1, y] ? 1 : 0) << 6);
                    pix |= (byte)((this[x, y] ? 1 : 0) << 7);
                    bmpbits[srcaddr + (x / 8)] = pix;
                }
            }
            return bmpbits;
        }

        public void DrawBitmap(BitmapBits source, Point location)
        {
            int dstx = location.X; int dsty = location.Y;
            for (int i = 0; i < source.Height; i++)
            {
                int di = ((dsty + i) * Width) + dstx;
                int si = i * source.Width;
                Array.Copy(source.Bits, si, Bits, di, source.Width);
            }
        }

        public void DrawBitmapComposited(BitmapBits source, Point location)
        {
            int srcl = 0;
            if (location.X < 0)
                srcl = -location.X;
            int srct = 0;
            if (location.Y < 0)
                srct = -location.Y;
            int srcr = source.Width;
            if (srcr > Width - location.X)
                srcr = Width - location.X;
            int srcb = source.Height;
            if (srcb > Height - location.Y)
                srcb = Height - location.Y;
            for (int i = srct; i < srcb; i++)
                for (int x = srcl; x < srcr; x++)
                    if (source[x, i])
                        this[location.X + x, location.Y + i] = source[x, i];
        }

        public void Flip(bool XFlip, bool YFlip)
        {
            if (!XFlip & !YFlip)
                return;
            if (XFlip)
            {
                for (int y = 0; y < Height; y++)
                {
                    int addr = y * Width;
                    Array.Reverse(Bits, addr, Width);
                }
            }
            if (YFlip)
            {
                bool[] tmppix = new bool[Bits.Length];
                for (int y = 0; y < Height; y++)
                {
                    int srcaddr = y * Width;
                    int dstaddr = (Height - y - 1) * Width;
                    Array.Copy(Bits, srcaddr, tmppix, dstaddr, Width);
                }
                Bits = tmppix;
            }
        }

        public void Clear()
        {
            Array.Clear(Bits, 0, Bits.Length);
        }

        public void Rotate(int R)
        {
            bool[] tmppix = new bool[Bits.Length];
            switch (R)
            {
                case 1:
                    for (int y = 0; y < Height; y++)
                    {
                        int srcaddr = y * Width;
                        int dstaddr = (Width * (Width - 1)) + y;
                        for (int x = 0; x < Width; x++)
                        {
                            tmppix[dstaddr] = Bits[srcaddr + x];
                            dstaddr -= Width;
                        }
                    }
                    Bits = tmppix;
                    int h = Height;
                    Height = Width;
                    Width = h;
                    break;
                case 2:
                    Flip(true, true);
                    break;
                case 3:
                    for (int y = 0; y < Height; y++)
                    {
                        int srcaddr = y * Width;
                        int dstaddr = Height - 1 - y;
                        for (int x = 0; x < Width; x++)
                        {
                            tmppix[dstaddr] = Bits[srcaddr + x];
                            dstaddr += Width;
                        }
                    }
                    Bits = tmppix;
                    h = Height;
                    Height = Width;
                    Width = h;
                    break;
            }
        }

        public BitmapBits Scale(int factor)
        {
            BitmapBits res = new BitmapBits(Width * factor, Height * factor);
            for (int y = 0; y < res.Height; y++)
                for (int x = 0; x < res.Width; x++)
                    res[x, y] = this[(x / factor), (y / factor)];
            return res;
        }

        private void DrawLine(bool index, int x1, int y1, int x2, int y2)
        {
            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            if (steep)
            {
                int tmp = x1;
                x1 = y1;
                y1 = tmp;
                tmp = x2;
                x2 = y2;
                y2 = tmp;
            }
            if (x1 > x2)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }
            int deltax = x2 - x1;
            int deltay = Math.Abs(y2 - y1);
            double error = 0;
            double deltaerr = (double)deltay / (double)deltax;
            int ystep;
            int y = y1;
            if (y1 < y2) ystep = 1; else ystep = -1;
            for (int x = x1; x <= x2; x++)
            {
                if (steep)
                {
                    if (x >= 0 & x < Height & y >= 0 & y < Width)
                        this[y, x] = index;
                }
                else
                {
                    if (y >= 0 & y < Height & x >= 0 & x < Width)
                        this[x, y] = index;
                }
                error = error + deltaerr;
                if (error >= 0.5)
                {
                    y = y + ystep;
                    error = error - 1.0;
                }
            }
        }

        public void DrawLine(bool index, Point p1, Point p2) { DrawLine(index, p1.X, p1.Y, p2.X, p2.Y); }

        public void DrawRectangle(bool index, int x, int y, int width, int height)
        {
            DrawLine(index, x, y, x + width, y);
            DrawLine(index, x, y, x, y + height);
            DrawLine(index, x + width, y, x + width, y + height);
            DrawLine(index, x, y + height, x + width, y + height);
        }

        public void DrawRectangle(bool index, Rectangle rect) { DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height); }

        public void FillRectangle(bool index, int x, int y, int width, int height)
        {
            int srcl = 0;
            if (x < 0)
                srcl = -x;
            int srct = 0;
            if (y < 0)
                srct = -y;
            int srcr = width;
            if (srcr > Width - x)
                srcr = Width - x;
            int srcb = height;
            if (srcb > Height - y)
                srcb = Height - y;
            for (int cy = srct; cy < srcb; cy++)
                for (int cx = srcl; cx < srcr; cx++)
                    this[cx, cy] = index;
        }

        public void FillRectangle(bool index, Rectangle rect) { DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height); }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;
            BitmapBits other = obj as BitmapBits;
            if (other == null) return false;
            if (Width != other.Width | Height != other.Height) return false;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}