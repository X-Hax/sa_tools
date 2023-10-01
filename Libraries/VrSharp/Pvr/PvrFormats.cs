using System;

namespace VrSharp.Pvr
{
    // Pvr Pixel Formats
    public enum PvrPixelFormat : byte
    {
		Argb1555 = 0x00,
		Rgb565   = 0x01,
		Argb4444 = 0x02,
		Yuv422   = 0x03,
		Bump88   = 0x04,
		Rgb555   = 0x05,
		Argb8888 = 0x06, // Palletized ARGB8888 or YUV420
		DdsDxt1  = 0x80, // DDS DXT1 RGB, no transparency (not in Ninja)
		DdsDxt3  = 0x81, // DDS DXT3 RGBA, transparency (not in Ninja)
		Unknown  = 0xFF,
	}

    // Pvr Data Formats
    public enum PvrDataFormat : byte
    {
		Raw						 = 0x00, // Doesn't exist?
        SquareTwiddled           = 0x01,
        SquareTwiddledMipmaps    = 0x02,
        Vq                       = 0x03,
        VqMipmaps                = 0x04,
        Index4                   = 0x05,
		Index4Mipmaps			 = 0x06,
        Index8                   = 0x07,
        Index8Mipmaps	         = 0x08,
        Rectangle                = 0x09,
		RectangleMipmap			 = 0x0A, // Doesn't exist?
		RectangleStride			 = 0x0B,
		RectangleStrideMipmap	 = 0x0C, // Doesn't exist?
		RectangleTwiddled        = 0x0D,
		Bitmap					 = 0x0E, // Doesn't exist?
		BitmapMipmap			 = 0x0F, // Doesn't exist?
		SmallVq                  = 0x10,
        SmallVqMipmaps           = 0x11,
        SquareTwiddledMipmapsAlt = 0x12, // Twiddled mipmapped DMA texture
		DDS						 = 0x80, // Not in Ninja
		DDS_2					 = 0x87, // Not in Ninja
		Unknown                  = 0xFF,
    }

    // Pvr Compression Formats
    public enum PvrCompressionFormat
    {
        None,
        Rle,
    }
}