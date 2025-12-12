namespace TextureLib
{
	/// <summary>Texture file types used in texture identification.</summary>
	public enum TextureFileType	
	{
		Pvr,
		Gvr,
		Dds,
		Xvr,
		Gdi,
		Unknown
	}

	///<summary>Flags specified in the metadata in PAK archives</summary>
    public enum NinjaSurfaceFlags : uint
    {
        Mipmapped = 0x80000000,
        VQ = 0x40000000,
        NotTwiddled = 0x04000000,
        Twiddled = 0x00000000,
        Stride = 0x02000000,
        Palettized = 0x00008000
    }

	///<summary>Extra fields for entries in PAK archives stored in the .inf file</summary>
	public class PakMetadata
    {
        public GvrDataFormat PakGvrFormat;
        public NinjaSurfaceFlags PakNinjaFlags;
        public string PakFolderName;
        public string PakLongPath;
    }
}