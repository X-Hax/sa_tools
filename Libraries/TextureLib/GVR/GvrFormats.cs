using System;

namespace TextureLib
{
    public enum GvrPaletteFormat : byte
    {
		/// <summary>This format is normally Intensity8A8 but SADX GC and SA2B use ARGB1555 instead.</summary>
		IntensityA8orArgb1555 = 0x00,
		/// <summary>Same as PVR RGB565 except Endianness.</summary>
		Rgb565 = 0x01,
		/// <summary>This format is normally RGB5A3 but SADX GC and SA2B use ARGB4444 instead.</summary>
		Rgb5A3orArgb4444 = 0x02,
		/// <summary>This format is not standard but may work in SADX GC or SA2B.</summary>
		Argb8888 = 0x06,
		/// <summary>This format is only used internally by Texture Editor.</summary>
		Invalid = 0xFF
	}

    public enum GvrDataFormat : byte
    {
		/// <summary>Grayscale format without alpha, 2 pixels per byte.</summary>
        Intensity4 = 0x00,
		/// <summary>Grayscale format without alpha, 1 byte per pixel.</summary>
		Intensity8 = 0x01,
		/// <summary>Grayscale format with alpha, 1 byte per pixel.</summary>
		IntensityA44 = 0x02,
		/// <summary>Grayscale format with alpha, 2 bytes per pixel.</summary>
		IntensityA88 = 0x03,
		/// <summary>5 bits for Red and Blue, 6 bytes for Green, no alpha.</summary>
		Rgb565 = 0x04,
		/// <summary>1 bit reserved, the rest is ARGB3444 or RGB555.</summary>
		Rgb5a3 = 0x05,
		/// <summary>Lossless format with alpha.</summary>
		Argb8888 = 0x06,
		/// <summary>Paletted format, 2 pixels per byte.</summary>
		Index4 = 0x08,
		/// <summary>Paletted format, 1 byte per pixel.</summary>
		Index8 = 0x09,
		/// <summary>Compressed blocks, 8 bytes per 16 pixels.</summary>
		Dxt1 = 0x0E,
    }

    [Flags]
    public enum GvrDataFlags : byte
    {
        None = 0x0,
		/// <summary>Indicates that the texture has mipmaps.</summary>
		Mipmaps = 0x1,
		/// <summary>Indicates that the texture uses an external palette file.</summary>
		ExternalPalette = 0x2,
		/// <summary>Indicates that the texture's palette (CLUT) is embedded.</summary>
		InternalPalette = 0x8,
    }
}