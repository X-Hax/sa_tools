namespace TextureLib
{
	internal class StrideDataCodec : RectangleDataCodec
	{
		public StrideDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override bool CheckDimensionsValid(int width, int height)
		{
			return width is >= 32 and <= 992
				&& height is >= 8 and <= 512
				&& width % 32 == 0;
		}
	}
}