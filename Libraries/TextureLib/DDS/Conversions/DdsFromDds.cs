namespace TextureLib
{
	public partial class DdsTexture
	{
		public DdsTexture(DdsTexture dds, bool maxQuality = false, bool useDxt = false)
		{
			// Set common texture properties
			Image = dds.Image;
			Gbix = dds.Gbix;
			Name = dds.Name;
			Width = dds.Width;
			Height = dds.Height;
			HasMipmaps = dds.HasMipmaps;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			BitmapAlphaLevel alphaLevel = TextureFunctions.GetAlphaLevelFromBitmap(Image);
			switch (alphaLevel)
			{
				case BitmapAlphaLevel.None:
					if (useDxt)
						DdsFormat = DdsFormat.Dxt1;
					else
						DdsFormat = maxQuality ? DdsFormat.Rgb888 : DdsFormat.Rgb565;
					break;
				case BitmapAlphaLevel.OneBitAlpha:
					if (useDxt)
						DdsFormat = maxQuality ? DdsFormat.Dxt3 : DdsFormat.Dxt1;
					else
						DdsFormat = maxQuality ? DdsFormat.Argb8888 : DdsFormat.Argb1555;
					break;
				case BitmapAlphaLevel.FullAlpha:
					if (useDxt)
						DdsFormat = maxQuality ? DdsFormat.Dxt5 : DdsFormat.Dxt3;
					DdsFormat = maxQuality ? DdsFormat.Argb8888 : DdsFormat.Argb4444;
					break;
			}
			Encode();
			Decode();
		}

		public DdsTexture(DdsTexture dds, DdsFormat format)
		{
			// Set common texture properties
			Image = dds.Image;
			Gbix = dds.Gbix;
			Name = dds.Name;
			Width = dds.Width;
			Height = dds.Height;
			HasMipmaps = dds.HasMipmaps;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			DdsFormat = format;
			Encode();
			Decode();
		}
	}
}
