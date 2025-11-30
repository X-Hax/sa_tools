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

}