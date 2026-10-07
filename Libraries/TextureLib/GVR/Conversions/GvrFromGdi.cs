namespace TextureLib
{
	public partial class GvrTexture
	{
		/// <summary>Create a new GVR texture from an existing GDI texture, data format is determined automatically.</summary>
		public GvrTexture(GdiTexture gdi, bool useDxt = false, bool maxQuality = false, bool forceMipmaps = false, bool forceGCIX = false)
		{
			GvrDataFormat targetDataFormat = AutoGvrDataFormatFromImage(gdi.Image, useDxt, maxQuality);
			ConvertFromGdi(gdi, targetDataFormat, forceMipmaps, forceGCIX);
		}

		/// <summary>Create a new GVR texture from an existing GDI texture, data format is specified manually.</summary>
		public GvrTexture(GdiTexture gdi, GvrDataFormat targetDataFormat, bool forceMipmaps = false, bool forceGCIX = false)
		{
			ConvertFromGdi(gdi, targetDataFormat, forceMipmaps, forceGCIX);
		}

		private void ConvertFromGdi(GdiTexture gdi, GvrDataFormat targetDataFormat, bool forceMipmaps = false, bool forceGCIX = false)
		{
			// Set common texture properties
			Image = gdi.Image;
			Gbix = gdi.Gbix;
			Name = gdi.Name;
			Width = gdi.Width;
			Height = gdi.Height;
			GvrDataFormat = targetDataFormat;
			isGCIX = forceGCIX;
			PaletteBank = gdi.PaletteBank;
			PaletteStartIndex = gdi.PaletteStartIndex;
			PakMetadata = gdi.PakMetadata;
			PvmxOriginalDimensions = gdi.PvmxOriginalDimensions;
			HasMipmaps = forceMipmaps;
			Encode();
		}
	}
}