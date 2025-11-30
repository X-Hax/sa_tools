using System;

namespace TextureLib
{
    public enum GvrPaletteFormat : byte
    {
        IntensityA8orArgb1555 = 0x00,
        Rgb565 = 0x01,
        Rgb5A3orArgb4444 = 0x02,
        Argb8888 = 0x06
    }

    public enum GvrPixelFormat : byte
    {
        IntensityA8 = 0x00,
        Rgb565 = 0x01,
        Rgb5a3 = 0x02,
        NonIndexed = 0xFF,
    }

    public enum GvrDataFormat : byte
    {
        Intensity4 = 0x00,
        Intensity8 = 0x01,
        IntensityA4 = 0x02,
        IntensityA8 = 0x03,
        Rgb565 = 0x04,
        Rgb5a3 = 0x05,
        Argb8888 = 0x06,
        Index4 = 0x08,
        Index8 = 0x09,
        Index14 = 0xA, // Palette index is 14 bit (from 0 to 16383), top 2 bits are ignored
        Dxt1 = 0x0E,
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