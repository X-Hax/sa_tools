using System;
using System.Drawing;
using System.Windows.Forms;

namespace SAFontEdit
{
	public partial class CharacterTileControl : UserControl
	{
		public int ImgScale;
		public BitmapBits Bits;
		public bool Clear;
		public Color[] Palette;
		readonly TextureBrush Brush;

		public CharacterTileControl()
		{
			InitializeComponent();
			Brush = new TextureBrush(BackgroundImage);
			DoubleBuffered = true;
		}

		protected override void OnPaintBackground(PaintEventArgs e) { }

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics gfx = e.Graphics;
			// Set settings
			gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
			gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			// Draw grid
			gfx.FillRectangle(Brush, new Rectangle(0, 0, Width, Height));
			// Draw tiles
			if (!Clear)
			{
				BitmapBits bits = Bits;
				if (bits is BitmapBits1bpp bits1)
					gfx.DrawImage(bits1.Scale(ImgScale).ToBitmap(Palette), 0, 0, Width, Height);
				else if (bits is BitmapBits32bpp bits32)
					gfx.DrawImage(bits32.Scale(ImgScale).ToBitmap(), 0, 0, Width, Height);
			}
		}
	}
}
