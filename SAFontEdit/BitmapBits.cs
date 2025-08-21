using System.Drawing;

namespace SAFontEdit
{
	public abstract class BitmapBits
	{
		public int Width;
		public int Height;
		public Size Size
		{
			get
			{
				return new Size(Width, Height);
			}
		}

		public abstract byte[] GetBytes(bool invert);
		public abstract Bitmap ToBitmap();

		public abstract byte GetCharacterWidth();
	}
}