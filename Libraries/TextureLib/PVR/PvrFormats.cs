namespace TextureLib
{
    public enum PvrPixelFormat : byte
    {
		/// <summary>NJD_TEXFMT_ARGB_1555.</summary>
		Argb1555 = 0x00,
		/// <summary>NJD_TEXFMT_RGB_565.</summary>
		Rgb565 = 0x01,
		/// <summary>NJD_TEXFMT_ARGB_4444.</summary>
		Argb4444 = 0x02,
		/// <summary>NJD_TEXFMT_YUV_422.</summary>
		Yuv422 = 0x03, 
		/// <summary>NJD_TEXFMT_BUMP.</summary>
		Bump88 = 0x04, 
		/// <summary>NJD_TEXFMT_RGB_555. Stored like ARGB1555 but with the alpha bit ignored. Haven't seen any PVR files with it.</summary>
		Rgb555 = 0x05,
		/// <summary>
		/// NJD_TEXFMT_ARGB_8888 or NJD_TEXFMT_YUV_420. ARGB_8888 is used in palette PVP files. Neither format is seen in PVR files.
		/// This format ID is used in code when building a texture manually in memory from raw headerless data.
		/// </summary>
		Argb8888orYUV420 = 0x06,
		/// <summary>
		/// Same as NJD_TEXFMT_ARGB_8888.
		/// This ID is not defined in official Ninja headers, but there are several textures in Kamui samples and Naomi games (Initial D) using it. 
		/// Palette only.
		/// Example: DCSDK8\Shinobi\Sample\Kamui1\TestSamples\Txtest13\P4S256T1.C
		/// </summary>
		Argb8888Alt = 0x07,
    }

    public enum PvrDataFormat : byte
    {
        None = 0x00, // Doesn't exist?

		/// <summary>NJD_TEXFMT_TWIDDLED.</summary>
		SquareTwiddled = 0x01,
		/// <summary>NJD_TEXFMT_TWIDDLED_MM.</summary>
		SquareTwiddledMipmaps = 0x02,
		/// <summary>NJD_TEXFMT_VQ.</summary>
		Vq = 0x03,
		/// <summary>NJD_TEXFMT_VQ_MM.</summary>
		VqMipmaps = 0x04,
		/// <summary>NJD_TEXFMT_PALETTIZE4.</summary>
		Index4 = 0x05,
		/// <summary>NJD_TEXFMT_PALETTIZE4_MM</summary>
		Index4Mipmaps = 0x06,
		/// <summary>NJD_TEXFMT_PALETTIZE8.</summary>
		Index8 = 0x07,
		/// <summary>NJD_TEXFMT_PALETTIZE8_MM.</summary>
		Index8Mipmaps = 0x08,
		/// <summary>NJD_TEXFMT_RECTANGLE.</summary>
		Rectangle = 0x09,
		/// <summary>NJD_TEXFMT_RECTANGLE_MM. Same as NJD_TEXFMT_RECTANGLE because Rectangle doesn't support mipmaps.</summary>
		RectangleMipmaps = 0x0A,
		/// <summary>NJD_TEXFMT_STRIDE.</summary>
		RectangleStride = 0x0B,
		/// <summary>NJD_TEXFMT_STRIDE_MM. Same as NJD_TEXFMT_STRIDE because Stride doesn't support mipmaps.</summary>
		RectangleStrideMipmaps = 0x0C,
		/// <summary>NJD_TEXFMT_TWIDDLED_RECTANGLE.</summary>
		RectangleTwiddled = 0x0D,
		/// <summary>
		/// NJD_TEXFMT_ABGR. This data format forces pixel format to ARGB8888 (flipped to BGRA8888?). Otherwise identical to Rectangle.
		/// There is a possibility that the image is supposed to be stored in a flipped state, but this is unconfirmed.
		/// </summary>
		Bitmap = 0x0E, 
		/// <summary>NJD_TEXFMT_ABGR_MM. Supposedly the same as ABGR. No known PVR files in this format.</summary>
		BitmapMipmaps = 0x0F,
		/// <summary>NJD_TEXFMT_SMALLVQ.</summary>
		SmallVq = 0x10,
		/// <summary>NJD_TEXFMT_SMALLVQ_MM.</summary>
		SmallVqMipmaps = 0x11,
		/// <summary>NJD_TEXFMT_TWIDDLED_MM_DMA. Twiddled mipmapped DMA texture, contents identical to NJD_TEXFMT_TWIDDLED_MM except 4 dummy bytes at the start.</summary>
		SquareTwiddledMipmapsDma = 0x12,
	}
}