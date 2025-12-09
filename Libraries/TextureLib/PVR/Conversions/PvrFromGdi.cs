namespace TextureLib
{
	public partial class PvrTexture
	{
		/// <summary>Create a new PVR texture from an existing GDI texture, pixel and data formats determined automatically.</summary>
		public PvrTexture(GdiTexture gdi, bool forceMipmaps = false)
		{
			PvrDataFormat targetDataFormat = AutoPvrDataFormatFromImage(gdi.Image, forceMipmaps);
			PvrPixelFormat pvrPixelFormat = AutoPvrPixelFormatFromImage(gdi.Image);
			ConvertFromGdi(gdi, targetDataFormat, pvrPixelFormat);
		}

		/// <summary>Create a new PVR texture from an existing GDI texture, data format determined automatically.</summary>
		public PvrTexture(GdiTexture gdi, PvrPixelFormat pvrPixelFormat, bool forceMipmaps = false)
		{
			PvrDataFormat targetDataFormat = AutoPvrDataFormatFromImage(gdi.Image, forceMipmaps);
			ConvertFromGdi(gdi, targetDataFormat, pvrPixelFormat);
		}

		/// <summary>Create a new PVR texture from an existing GDI texture, pixel format determined automatically.</summary>
		public PvrTexture(GdiTexture gdi, PvrDataFormat targetDataFormat)
		{
			PvrPixelFormat pvrPixelFormat = AutoPvrPixelFormatFromImage(gdi.Image);
			ConvertFromGdi(gdi, targetDataFormat, pvrPixelFormat);
		}

		private void ConvertFromGdi(GdiTexture gdi, PvrDataFormat targetPvrDataFormat, PvrPixelFormat targetPixelFormat)
		{
			// Set common texture properties
			Image = gdi.Image;
			Gbix = gdi.Gbix;
			Name = gdi.Name;
			Width = gdi.Width;
			Height = gdi.Height;
			PvrDataFormat = targetPvrDataFormat;
			PaletteBank = gdi.PaletteBank;
			PaletteStartIndex = gdi.PaletteStartIndex;
			PakMetadata = gdi.PakMetadata;
			PvmxOriginalDimensions = gdi.PvmxOriginalDimensions;
			HasMipmaps = PvrDataCodec.Create(targetPvrDataFormat, PixelCodec.GetPixelCodec(targetPixelFormat)).HasMipmaps;
			Encode();
		}
	}
}