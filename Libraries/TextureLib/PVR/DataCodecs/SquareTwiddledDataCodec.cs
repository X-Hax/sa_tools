namespace TextureLib
{
	internal class SquareTwiddledDataCodec : RectangleTwiddledDataCodec
	{
		public SquareTwiddledDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override bool CheckDimensionsValid(int width, int height)
		{
			// "Underiving" it
			return width == height
				&& width is >= 8 and <= 1024
				&& TextureFunctions.IsPow2(width);
		}
	}
}
