using System;

namespace VrSharp.DDS
{
	// DDS Pixel Formats (Limited. Expand with time.)
	public enum DDSPixelFormat
	{
		Invalid,
		AlphaPixels,
		Alpha,
		FourCC = 0x4, // Compressed RGB
		RGB = 0x40,   // Uncompressed RGB
		RGBA = 0x41,
		YUV = 0x200,
	}

	public enum DDSPixelBitFormat
	{
		Invalid,
		RGB444,   //RGB444
		ARGB4444, //ARGB4444
		RGB565,   //RGB565
		ARGB1555, //ARGB1555
		ARGB8888, //ARGB8888
		BGRA8888, //BGRA8888
		DXT1,
		DXT2,
		DXT3,
		DXT4,
		DXT5
	}
	public enum DDSHeaderFlags
	{
		Caps = 0x1,
		Height = 0x2,
		Width = 0x4,
		Pitch = 0x8,
		PixelType = 0x1000,
		MipmapCount = 0x20000,
		LinearSize = 0x80000,
		Depth = 0x800000
	}
	public enum DDSCaps
	{
		Complex = 0x00000008,
		Texture = 0x00001000,
		Mipmap = 0x00400000
	}
}