using System;

namespace TextureLib
{
    // Various classes and enums used by TextureLib, will be moved to respective .cs files for PVR, GVR etc.

    public enum TextureFormat
    {
        Gdi,
        Pvr,
        Gvr,
        Xvr,
        Dds,
        Invalid
    }



    public enum NinjaSurfaceFlags : uint
    {
        Mipmapped = 0x80000000,
        VQ = 0x40000000,
        NotTwiddled = 0x04000000,
        Twiddled = 0x00000000,
        Stride = 0x02000000,
        Palettized = 0x00008000
    }

    // Extra fields for entries in PAK archives (stored in the .inf file)
    public class PakMetadata
    {
        public GvrDataFormat PakGvrFormat;
        public NinjaSurfaceFlags PakNinjaFlags;
        public string PakFolderName;
        public string PakLongPath;
    }

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