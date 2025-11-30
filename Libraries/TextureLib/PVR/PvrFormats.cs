namespace TextureLib
{
    public enum PvrPixelFormat : byte
    {
        Argb1555 = 0x00, // NJD_TEXFMT_ARGB_1555
        Rgb565 = 0x01, // NJD_TEXFMT_RGB_565
        Argb4444 = 0x02, // NJD_TEXFMT_ARGB_4444
        Yuv422 = 0x03, // NJD_TEXFMT_YUV_422
        Bump88 = 0x04, // NJD_TEXFMT_BUMP
        Rgb555 = 0x05, // NJD_TEXFMT_RGB_555
        Argb8888orYUV420 = 0x06, // NJD_TEXFMT_ARGB_8888 or NJD_TEXFMT_YUV_420 (Palletized ARGB8888 or YUV420).
        Argb8888 = 0x07, // DCSDK8\Shinobi\Sample\Kamui1\TestSamples\Txtest13\P4S256T1.C
    }

    // Format 06 is not found in PVR files, but can be used in palettes as NJD_TEXFMT_ARGB_8888 can be used in palettes.
    // YUV420 can be used in textures that are set up manually in memory and loaded with raw data.
    // Format 07 doesn't look standard to the SDK, however some PVR files in this format do exist in SDK samples and Naomi games (Initial D).

    public enum PvrDataFormat : byte
    {
        None = 0x00, // Doesn't exist?
        SquareTwiddled = 0x01, // NJD_TEXFMT_TWIDDLED
        SquareTwiddledMipmaps = 0x02, // NJD_TEXFMT_TWIDDLED_MM
        Vq = 0x03, // NJD_TEXFMT_VQ
        VqMipmaps = 0x04, // NJD_TEXFMT_VQ_MM
        Index4 = 0x05, // NJD_TEXFMT_PALETTIZE4
        Index4Mipmaps = 0x06, // NJD_TEXFMT_PALETTIZE4_MM
        Index8 = 0x07, // NJD_TEXFMT_PALETTIZE8
        Index8Mipmaps = 0x08, // NJD_TEXFMT_PALETTIZE8_MM
        Rectangle = 0x09, // NJD_TEXFMT_RECTANGLE
        RectangleMipmaps = 0x0A, // Same as Rectangle (doesn't support mipmaps)
        RectangleStride = 0x0B, // NJD_TEXFMT_STRIDE
        RectangleStrideMipmaps = 0x0C, // Same as Stride (doesn't support mipmaps)
        RectangleTwiddled = 0x0D, // NJD_TEXFMT_TWIDDLED_RECTANGLE
        Bitmap = 0x0E, // NJD_TEXFMT_ABGR
        BitmapMipmaps = 0x0F, // NJD_TEXFMT_ABGR_MM
        SmallVq = 0x10, // NJD_TEXFMT_SMALLVQ
        SmallVqMipmaps = 0x11, // NJD_TEXFMT_SMALLVQ_MM
        SquareTwiddledMipmapsAlt = 0x12, // NJD_TEXFMT_TWIDDLED_MM_DMA Twiddled mipmapped DMA texture
    }
}