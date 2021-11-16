using System;

namespace VrSharp.Svr
{
    // Svr Pixel Formats
    public enum SvrPixelFormat : byte
    {
        Rgb5a3   = 0x08,
        Argb8888 = 0x09,
        Unknown  = 0xFF,
    }

    // Svr Data Formats
    public enum SvrDataFormat : byte
    {
        Rectangle             = 0x60,
        Index4ExternalPalette = 0x62,
        Index8ExternalPalette = 0x64,
        Index4Rgb5a3Rectangle = 0x66,
        Index4Rgb5a3Square    = 0x67,
        Index4Argb8Rectangle  = 0x68,
        Index4Argb8Square     = 0x69,
        Index8Rgb5a3Rectangle = 0x6A,
        Index8Rgb5a3Square    = 0x6B,
        Index8Argb8Rectangle  = 0x6C,
        Index8Argb8Square     = 0x6D,
        Index4                = Index4Rgb5a3Rectangle,
        Index8                = Index8Rgb5a3Rectangle,
        Unknown               = 0xFF,
    }
}