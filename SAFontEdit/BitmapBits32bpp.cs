using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SAFontEdit
{
	/// <summary>
	/// Represents the pixel data of a 32bpp Bitmap.
	/// </summary>
	public class BitmapBits32bpp : BitmapBits
	{
		public Color[] Colors;

		public Color this[int x, int y]
		{
			get
			{
				return Colors[(y * Width) + x];
			}
			set
			{
				Colors[(y * Width) + x] = value;
			}
		}

		public BitmapBits32bpp(int width, int height)
		{
			Width = width;
			Height = height;
			Colors = new Color[width * height];
		}

		public BitmapBits32bpp(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
			{
				bmp = new Bitmap(bmp);
				// Convert black to transparent
				for (int y = 0; y < bmp.Height; y++)
				{
					for (int x = 0; x < bmp.Width; x++)
						if (bmp.GetPixel(x, y).R == 0 && bmp.GetPixel(x, y).G == 0 && bmp.GetPixel(x, y).B == 0)
							bmp.SetPixel(x, y, Color.Transparent);
				}
			}
			Width = bmp.Width;
			Height = bmp.Height;
			Colors = new Color[Width * Height];
			for (int y = 0; y < bmp.Height; y++)
			{
				for (int x = 0; x < bmp.Width; x++)
				{
					this[x, y] = bmp.GetPixel(x, y);
				}
			}
		}

		public BitmapBits32bpp(BitmapBits32bpp source)
		{
			Width = source.Width;
			Height = source.Height;
			Colors = new Color[source.Colors.Length];
			Array.Copy(source.Colors, Colors, Colors.Length);
		}

		public BitmapBits32bpp(byte[] file, int address, bool reverse)
		{
			Width = Height = 24;
			Colors = new Color[24 * 24];
			for (int y = 0; y < 24; y++)
			{
				for (int x = 0; x < 24; x++)
				{
					uint color = BitConverter.ToUInt32(file, address + 4 * (y * Width + x));
					if (reverse)
					{
						color = (color & 0x000000FFU) << 24 | (color & 0x0000FF00U) << 8 | (color & 0x00FF0000U) >> 8 | (color & 0xFF000000U) >> 24;
					}
					this[x, y] = Color.FromArgb((int)color);
				}
			}
		}

		public override Bitmap ToBitmap()
		{
			Bitmap newbmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					newbmp.SetPixel(x, y, this[x, y]);
				}
			}
			return newbmp;
		}

		public override byte[] GetBytes(bool invert)
		{
			List<byte> bmpbits = new List<byte>();
			for (int y = 0; y < Height; y++)
			{
				{
					for (int x = 0; x < Width; x++)
					{
						uint color = (uint)this[x, y].ToArgb();
						if (invert)
							color = color = (color & 0x000000FFU) << 24 | (color & 0x0000FF00U) << 8 | (color & 0x00FF0000U) >> 8 | (color & 0xFF000000U) >> 24;
						bmpbits.AddRange(BitConverter.GetBytes(color));
					}
				}
			}
			return bmpbits.ToArray();
		}
		public void DrawBitmap(BitmapBits32bpp source, Point location)
		{
			int dstx = location.X; int dsty = location.Y;
			for (int i = 0; i < source.Height; i++)
			{
				int di = ((dsty + i) * Width) + dstx;
				int si = i * source.Width;
				Array.Copy(source.Colors, si, Colors, di, source.Width);
			}
		}

		public void DrawBitmapComposited(BitmapBits32bpp source, Point location)
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
					if (source[x, i] != Color.Transparent)
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
					Array.Reverse(Colors, addr, Width);
				}
			}
			if (YFlip)
			{
				Color[] tmppix = new Color[Colors.Length];
				for (int y = 0; y < Height; y++)
				{
					int srcaddr = y * Width;
					int dstaddr = (Height - y - 1) * Width;
					Array.Copy(Colors, srcaddr, tmppix, dstaddr, Width);
				}
				Colors = tmppix;
			}
		}

		public void Clear()
		{
			Array.Clear(Colors, 0, Colors.Length);
		}

		public void Rotate(int R)
		{
			Color[] tmppix = new Color[Colors.Length];
			switch (R)
			{
				case 1:
					for (int y = 0; y < Height; y++)
					{
						int srcaddr = y * Width;
						int dstaddr = (Width * (Width - 1)) + y;
						for (int x = 0; x < Width; x++)
						{
							tmppix[dstaddr] = Colors[srcaddr + x];
							dstaddr -= Width;
						}
					}
					Colors = tmppix;
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
							tmppix[dstaddr] = Colors[srcaddr + x];
							dstaddr += Width;
						}
					}
					Colors = tmppix;
					h = Height;
					Height = Width;
					Width = h;
					break;
			}
		}

		public BitmapBits32bpp Scale(int factor)
		{
			BitmapBits32bpp res = new BitmapBits32bpp(Width * factor, Height * factor);
			for (int y = 0; y < res.Height; y++)
				for (int x = 0; x < res.Width; x++)
					res[x, y] = this[(x / factor), (y / factor)];
			return res;
		}

		private void DrawLine(Color color, int x1, int y1, int x2, int y2)
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
						this[y, x] = color;
				}
				else
				{
					if (y >= 0 & y < Height & x >= 0 & x < Width)
						this[x, y] = color;
				}
				error = error + deltaerr;
				if (error >= 0.5)
				{
					y = y + ystep;
					error = error - 1.0;
				}
			}
		}

		public void DrawLine(Color color, Point p1, Point p2) { DrawLine(color, p1.X, p1.Y, p2.X, p2.Y); }

		public void DrawRectangle(Color index, int x, int y, int width, int height)
		{
			DrawLine(index, x, y, x + width, y);
			DrawLine(index, x, y, x, y + height);
			DrawLine(index, x + width, y, x + width, y + height);
			DrawLine(index, x, y + height, x + width, y + height);
		}

		public void DrawRectangle(Color index, Rectangle rect) { DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height); }

		public void FillRectangle(Color index, int x, int y, int width, int height)
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

		public void FillRectangle(Color index, Rectangle rect) { DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height); }

		public override bool Equals(object obj)
		{
			if (base.Equals(obj)) return true;
			BitmapBits32bpp other = obj as BitmapBits32bpp;
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

		public override byte GetCharacterWidth()
		{
			int result = 0;
			for (int y = 0; y < Height; y++)
				for (int x = 0; x < Width; x++)
				{
					if (this[x, y].A != 0 && x > result)
						result = x;
				}
			return (byte)(result);
		}
	}
}