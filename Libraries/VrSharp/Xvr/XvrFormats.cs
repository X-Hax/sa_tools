namespace VrSharp.Xvr
{
	public static class XvrFormats
	{
		/*
		 *  Xvr format list from PSO executable. 
		 * 
		    D3DFMT_UNKNOWN
            D3DFMT_A8R8G8B8
            D3DFMT_R5G6B5
            D3DFMT_A1R5G5B5
            D3DFMT_A4R4G4B4
            D3DFMT_P8
            D3DFMT_DXT1
            D3DFMT_DXT2
            D3DFMT_DXT3
            D3DFMT_DXT4
            D3DFMT_DXT5
            D3DFMT_A8R8G8B8
            D3DFMT_R5G6B5
            D3DFMT_A1R5G5B5
            D3DFMT_A4R4G4B4
            D3DFMT_YUY2
            D3DFMT_V8U8
            D3DFMT_A8
            D3DFMT_X1R5G5B5
            D3DFMT_X8R8G8B8
            D3DFMT_UNKNOWN
            D3DFMT_UNKNOWN
            D3DFMT_UNKNOWN
            D3DFMT_UNKNOWN
		*/

		public enum XvrFlags : byte
		{
			Mips = 1,
			Alpha = 2,
		}
	}
}
