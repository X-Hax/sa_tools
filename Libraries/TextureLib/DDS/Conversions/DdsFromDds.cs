namespace TextureLib
{
	public partial class DdsTexture
	{
		/// <summary>Create a new DDS texture from an existing DDS texture, data format is determined automatically.</summary>
		/// <param name="dds">Source DDS texture.</param>
		/// <param name="maxQuality">Prefer maximum quality (RGB888/RGBA8888) and higher quality DXT formats.</param>
		/// <param name="useDxt">Whether to consider DXT formats for encoding or not.</param>
		/// <param name="forceMipmaps">Force add mipmaps even if the source texture doesn't have them.</param>
		public DdsTexture(DdsTexture dds, bool maxQuality = false, bool useDxt = true, bool forceMipmaps = false)
		{
			// Set common texture properties
			Image = dds.Image;
			Gbix = dds.Gbix;
			Name = dds.Name;
			Width = dds.Width;
			Height = dds.Height;
			HasMipmaps = forceMipmaps ? true : dds.HasMipmaps;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			PvmxOriginalDimensions = dds.PvmxOriginalDimensions;
			DdsFormat = AutoDdsFormatFromImage(Image, maxQuality, useDxt);
			Encode();
			Decode();
		}

		/// <summary>Create a new DDS texture from an existing DDS texture, data format is specified manually.</summary>
		/// <param name="dds">Source DDS texture.</param>
		/// <param name="format">Target DDS format.</param>
		/// <param name="forceMipmaps">Force add mipmaps even if the source texture doesn't have them.</param>
		public DdsTexture(DdsTexture dds, DdsFormat format, bool forceMipmaps = false)
		{
			// Set common texture properties
			Image = dds.Image;
			Gbix = dds.Gbix;
			Name = dds.Name;
			Width = dds.Width;
			Height = dds.Height;
			HasMipmaps = forceMipmaps ? true : dds.HasMipmaps;
			PaletteBank = dds.PaletteBank;
			PaletteStartIndex = dds.PaletteStartIndex;
			PakMetadata = dds.PakMetadata;
			DdsFormat = format;
			Encode();
			Decode();
		}
	}
}