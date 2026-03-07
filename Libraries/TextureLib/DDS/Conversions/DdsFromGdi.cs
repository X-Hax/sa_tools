namespace TextureLib
{
	public partial class DdsTexture
	{
		/// <summary>Create a new DDS texture from an existing GDI texture, data format determined automatically.</summary>
		public DdsTexture(GdiTexture gdi, bool forceMipmaps = false, bool maxQuality = false, bool useDxt = true)
		{
			DdsFormat targetDataFormat = AutoDdsFormatFromImage(gdi.Image, maxQuality, useDxt);
			ConvertFromGdi(gdi, targetDataFormat, forceMipmaps);
		}

		/// <summary>Create a new DDS texture from an existing GDI texture, data format specified manually.</summary>
		public DdsTexture(GdiTexture gdi, DdsFormat targetDataFormat, bool forceMipmaps = false)
		{
			ConvertFromGdi(gdi, targetDataFormat, forceMipmaps);
		}

		private void ConvertFromGdi(GdiTexture gdi, DdsFormat targetDataFormat, bool forceMipmaps)
		{
			// Set common texture properties
			Image = gdi.Image;
			Gbix = gdi.Gbix;
			Name = gdi.Name;
			Width = gdi.Width;
			Height = gdi.Height;
			DdsFormat = targetDataFormat;
			PaletteBank = gdi.PaletteBank;
			PaletteStartIndex = gdi.PaletteStartIndex;
			PakMetadata = gdi.PakMetadata;
			PvmxOriginalDimensions = gdi.PvmxOriginalDimensions;
			HasMipmaps = forceMipmaps;
			Encode();
		}
	}
}