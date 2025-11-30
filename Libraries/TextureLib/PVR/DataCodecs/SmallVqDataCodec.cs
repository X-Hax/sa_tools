namespace TextureLib
{
	internal class SmallVqDataCodec : VqDataCodec
	{
		public SmallVqDataCodec(PixelCodec pixelCodec) : base(pixelCodec) { }

		public override int GetPaletteEntries(int? width)
		{
			if(width == null)
			{
				return 1024;
			}
			else if(width <= 16)
			{
				return 64;
			}
			else if(width <= 32)
			{
				return 128;
			}
			else if(width <= 64)
			{
				return 512;
			}
			else
			{
				return 1024;
			}
		}
	}
}
