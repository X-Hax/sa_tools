namespace TextureLib
{
	public partial class GvrTexture
	{
		/// <summary>Create a new GVR texture from another GVR texture, data format determined automatically.</summary>
		public GvrTexture(GvrTexture gvr, bool forceMipmaps = false, bool useDxt = true, bool maxQuality = false)
		{
			GvrDataFormat targetGvrDataFormat = AutoGvrDataFormatFromImage(gvr.Image, useDxt, maxQuality);
			ConvertFromGvr(gvr, targetGvrDataFormat, forceMipmaps);
		}

		/// <summary>Create a new GVR texture from another GVR texture, data format specified manually.</summary>
		public GvrTexture(GvrTexture gvr, GvrDataFormat targetGvrDataFormat, bool forceMipmaps = false)
		{
			ConvertFromGvr(gvr, targetGvrDataFormat, forceMipmaps);
		}

		private void ConvertFromGvr(GvrTexture gvr, GvrDataFormat targetGvrDataFormat, bool forceMipmaps = false)
		{
			// Any data format that is different from the original results in a reencode
			// Set common texture properties
			Image = gvr.Image;
			Gbix = gvr.Gbix;
			Name = gvr.Name;
			Width = gvr.Width;
			Height = gvr.Height;
			GvrDataFormat = targetGvrDataFormat;
			PaletteBank = gvr.PaletteBank;
			PaletteStartIndex = gvr.PaletteStartIndex;
			PakMetadata = gvr.PakMetadata;
			PvmxOriginalDimensions = gvr.PvmxOriginalDimensions;
			HasMipmaps = gvr.HasMipmaps;
			Encode();
		}
	}
}