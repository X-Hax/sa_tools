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
        Intensity4 = 0x00,
        Intensity8 = 0x01,
        IntensityA44 = 0x02,
        IntensityA88 = 0x03,
        Rgb565 = 0x04,
        Rgb5a3 = 0x05,
        Argb8888 = 0x06,
        Index4 = 0x08,
        Index8 = 0x09,
		/// <summary>Palette index is 14 bit (from 0 to 16383), top 2 bits are ignored. Currently not implemented.</summary>
		Index14 = 0xA,
        Dxt1 = 0x0E,
		/// <summary>This format is only used internally by Texture Editor.</summary>
		Invalid = 0xFF
    }

    [Flags]
    public enum GvrDataFlags : byte
    {
        None = 0x0,
        Mipmaps = 0x1,
        ExternalPalette = 0x2,
        InternalPalette = 0x8,
    }
}