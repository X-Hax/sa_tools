using System;

namespace VrSharp.Pvr
{
    // Pvr Pixel Formats
    public enum PvrPixelFormat : byte
    {
		Argb1555 = 0x00, // NJD_TEXFMT_ARGB_1555
		Rgb565   = 0x01, // NJD_TEXFMT_RGB_565
		Argb4444 = 0x02, // NJD_TEXFMT_ARGB_4444
		Yuv422   = 0x03, // NJD_TEXFMT_YUV_422
		Bump88   = 0x04, // NJD_TEXFMT_BUMP
		Rgb555   = 0x05, // NJD_TEXFMT_RGB_555
		Argb8888 = 0x06, // NJD_TEXFMT_ARGB_8888 or NJD_TEXFMT_YUV_420 (Palletized ARGB8888 or YUV420)
		DdsDxt1  = 0x80, // DDS DXT1 RGB, no transparency (not in DC Ninja)
		DdsDxt3  = 0x81, // DDS DXT3 RGBA, transparency (not in DC Ninja)
		Unknown  = 0xFF, // NJD_TEXFMT_COLOR_MASK
	}

    // Pvr Data Formats
    public enum PvrDataFormat : byte
    {
		Raw						 = 0x00, // Doesn't exist?
        SquareTwiddled           = 0x01, // NJD_TEXFMT_TWIDDLED
		SquareTwiddledMipmaps    = 0x02, // NJD_TEXFMT_TWIDDLED_MM
		Vq                       = 0x03, // NJD_TEXFMT_VQ
		VqMipmaps                = 0x04, // NJD_TEXFMT_VQ_MM
		Index4                   = 0x05, // NJD_TEXFMT_PALETTIZE4
		Index4Mipmaps			 = 0x06, // NJD_TEXFMT_PALETTIZE4_MM
		Index8                   = 0x07, // NJD_TEXFMT_PALETTIZE8
		Index8Mipmaps	         = 0x08, // NJD_TEXFMT_PALETTIZE8_MM
		Rectangle                = 0x09, // NJD_TEXFMT_RECTANGLE
		RectangleMipmap			 = 0x0A, // Doesn't exist?
		RectangleStride			 = 0x0B, // NJD_TEXFMT_STRIDE
		RectangleStrideMipmap	 = 0x0C, // Doesn't exist?
		RectangleTwiddled        = 0x0D, // NJD_TEXFMT_TWIDDLED_RECTANGLE
		Bitmap					 = 0x0E, // NJD_TEXFMT_ABGR, doesn't exist?
		BitmapMipmap			 = 0x0F, // NJD_TEXFMT_ABGR_MM, doesn't exist?
		SmallVq                  = 0x10, // NJD_TEXFMT_SMALLVQ
		SmallVqMipmaps           = 0x11, // NJD_TEXFMT_SMALLVQ_MM
		SquareTwiddledMipmapsAlt = 0x12, // NJD_TEXFMT_TWIDDLED_MM_DMA Twiddled mipmapped DMA texture
		DDS						 = 0x80, // Not in DC Ninja
		DDS_2					 = 0x87, // Not in DC Ninja
		Unknown                  = 0xFF, // NJD_TEXFMT_TYPE_MASK
	}

    // Pvr Compression Formats
    public enum PvrCompressionFormat
    {
        None,
        Rle,
    }
}