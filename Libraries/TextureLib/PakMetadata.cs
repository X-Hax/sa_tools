using System;
using System.Collections.Generic;
using System.Text;

namespace TextureLib
{
	///<summary>PVR surface flags specified in INF metadata of PAK archives.</summary>
	public enum NinjaSurfaceFlags : uint
	{
		/// <summary>Texture has mipmaps (NJD_SURFACEFLAGS_MIPMAPED).</summary>
		Mipmapped = 0x80000000,

		/// <summary>Texture was VQ compressed (NJD_SURFACEFLAGS_VQ).</summary>
		VQ = 0x40000000,

		/// <summary>Texture is not twiddled (NJD_SURFACEFLAGS_NOTWIDDLED).</summary>
		NotTwiddled = 0x04000000,

		/// <summary>Texture is twiddled (NJD_SURFACEFLAGS_TWIDDLED).</summary>
		Twiddled = 0x00000000,

		/// <summary>Texture is in Stride format (NJD_SURFACEFLAGS_STRIDE).</summary>
		Stride = 0x02000000,

		/// <summary>Texture is Indexed (NJD_SURFACEFLAGS_PALETTIZED).</summary>
		Palettized = 0x00008000
	}

	/// <summary>
	/// Extra fields for entries in PAK archives stored in the .inf file. 
	/// This is not the entire contents of the .inf entry, only the essential fields.
	/// </summary>
	public class PakMetadata
	{
		/// <summary>If this value is true, the texture is excluded from the INF file.</summary>
		public bool BlacklistInf;

		/// <summary>Data format of the GVR texture.</summary>
		public GvrDataFormat PakGvrFormat;

		///<summary>Ninja PVR surface flags.</summary>
		public NinjaSurfaceFlags PakNinjaFlags;

		/// <summary>PAK folder name, matches the PAK's filename without extension.</summary>
		public string PakFolderName;

		/// <summary>
		/// PAK relative path name, includes folder name, file name and extension. Backslash is escaped.
		/// Example: `..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\stg_title08\\stg_title08.dds`
		/// </summary>
		public string PakLongPath;

		public PakMetadata()
		{
			PakGvrFormat = GvrDataFormat.Rgb5a3;
			PakNinjaFlags = NinjaSurfaceFlags.NotTwiddled;
		}
	}

	/// <summary>Class for entries in the INF file inside SA2 PAK files.</summary>
	public class PAKInfEntry
	{
		/// <summary>Entry filename without extension, 28 ASCII characters, can be padded with spaces.</summary>
		public byte[] Filename;

		/// <summary>Global Index.</summary>
		public uint GlobalIndex;

		/// <summary>GVR texture data format.</summary>
		public GvrDataFormat TypeInf;

		/// <summary>Bit Depth (unused, usually 0).</summary>
		public uint BitDepth;

		/// <summary>GVR texture data format (duplicate).</summary>
		public GvrDataFormat PixelFormatInf;

		/// <summary>Texture width.</summary>
		public uint nWidth;

		/// <summary>Texture height.</summary>
		public uint nHeight;

		/// <summary>Texture size in bytes (unused?).</summary>
		public uint TextureSize;

		/// <summary>Ninja surface flags.</summary>
		public NinjaSurfaceFlags fSurfaceFlags;

		/// <summary>Creates an empty PAK INF entry.</summary>
		public PAKInfEntry()
		{
			Filename = new byte[28];
		}

		/// <summary>Reads a PAK INF entry from an offset in the PAK INF file.</summary>
		/// <param name="data">Byte array to read from.</param>
		/// <param name="offset">Entry start offset (optional).</param>
		public PAKInfEntry(byte[] data, int offset = 0)
		{
			Filename = new byte[28];
			Array.Copy(data, offset, Filename, 0, 28);
			GlobalIndex = BitConverter.ToUInt32(data, offset + 0x1C);
			TypeInf = (GvrDataFormat)BitConverter.ToUInt32(data, offset + 0x20);
			BitDepth = BitConverter.ToUInt32(data, offset + 0x24);
			PixelFormatInf = (GvrDataFormat)BitConverter.ToUInt32(data, offset + 0x28);
			nWidth = BitConverter.ToUInt32(data, offset + 0x2C);
			nHeight = BitConverter.ToUInt32(data, offset + 0x30);
			TextureSize = BitConverter.ToUInt32(data, offset + 0x34);
			fSurfaceFlags = (NinjaSurfaceFlags)BitConverter.ToUInt32(data, offset + 0x38);
		}

		/// <summary>Creates a PAK INF entry from any texture supported by TextureLib.</summary>
		/// <param name="texture">Source texture.</param>
		public PAKInfEntry(GenericTexture texture)
		{
			byte[] name = System.Text.Encoding.ASCII.GetBytes(texture.Name);
			Array.Copy(name, 0, Filename, 0, Math.Min(28, name.Length));
			GlobalIndex = texture.Gbix;
			PixelFormatInf = TypeInf = texture.PakMetadata.PakGvrFormat;
			nWidth = (uint)texture.Width;
			nHeight = (uint)texture.Height;
			TextureSize = (uint)texture.GetBytes().Length;
			fSurfaceFlags = texture.PakMetadata.PakNinjaFlags;
		}

		/// <summary>Retrieves the filename of a PAK INF entry as a string.</summary>
		public string GetFilename()
		{
			StringBuilder sb = new StringBuilder(0x1C);
			for (int j = 0; j < 28; j++)
				if (Filename[j] != 0)
					sb.Append((char)Filename[j]);
				else
					break;
			return sb.ToString();
		}

		/// <summary>Converts the PAK INF entry data into a byte array.</summary>
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(Filename);
			result.AddRange(BitConverter.GetBytes(GlobalIndex));
			result.AddRange(BitConverter.GetBytes((uint)TypeInf));
			result.AddRange(BitConverter.GetBytes(BitDepth));
			result.AddRange(BitConverter.GetBytes((uint)PixelFormatInf));
			result.AddRange(BitConverter.GetBytes(nWidth));
			result.AddRange(BitConverter.GetBytes(nHeight));
			result.AddRange(BitConverter.GetBytes(TextureSize));
			result.AddRange(BitConverter.GetBytes((uint)fSurfaceFlags));
			return result.ToArray();
		}
	};
}