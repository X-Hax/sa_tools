using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using VrSharp.Gvr;
using VrSharp.Pvr;

namespace TextureEditor
{
    enum TextureFormat { PVM, GVM, PVMX, PAK }

    abstract class TextureInfo
    {
        public string Name { get; set; }
        public uint GlobalIndex { get; set; }
        public bool Mipmap { get; set; }
        public Bitmap Image { get; set; }
        public abstract bool CheckMipmap();
    }

    class PvrTextureInfo : TextureInfo
    {
        public PvrDataFormat DataFormat { get; set; }
        public PvrPixelFormat PixelFormat { get; set; }
        public MemoryStream TextureData { get; set; }
        public PvrTextureInfo() { }

        public PvrTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            PixelFormat = PvrPixelFormat.Unknown;
            DataFormat = PvrDataFormat.Unknown;
            if (tex is PvrTextureInfo pv)
                TextureData = pv.TextureData;
        }

        public PvrTextureInfo(GvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            switch (tex.DataFormat)
            {
                case GvrDataFormat.Index4:
                    DataFormat = PvrDataFormat.Index4;
                    break;
                case GvrDataFormat.Index8:
                    DataFormat = PvrDataFormat.Index8;
                    break;
            }
        }

        public PvrTextureInfo(PvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            PixelFormat = tex.PixelFormat;
            DataFormat = tex.DataFormat;
            TextureData = tex.TextureData;
        }

        public PvrTextureInfo(string name, uint gbix, Bitmap bitmap)
        {
            Name = name;
            GlobalIndex = gbix;
            DataFormat = PvrDataFormat.Unknown;
            PixelFormat = PvrPixelFormat.Unknown;
            Image = bitmap;
        }

        public PvrTextureInfo(string name, MemoryStream str, PvpPalette pvp = null)
        {
            TextureData = str;
            PvrTexture texture = new PvrTexture(str);
            if (pvp != null)
                texture.SetPalette(pvp);
            Name = name;
            GlobalIndex = texture.GlobalIndex;
            DataFormat = texture.DataFormat;
            Mipmap = DataFormat == PvrDataFormat.SquareTwiddledMipmaps || DataFormat == PvrDataFormat.SquareTwiddledMipmapsAlt;
            PixelFormat = texture.PixelFormat;
            Image = texture.ToBitmap();
        }

        public override bool CheckMipmap()
        {
            return DataFormat != PvrDataFormat.Index4 && DataFormat != PvrDataFormat.Index8 && Image.Width == Image.Height;
        }
    }

    class GvrTextureInfo : TextureInfo
    {
        public GvrDataFormat DataFormat { get; set; }
        public GvrPixelFormat PixelFormat { get; set; }
        public MemoryStream TextureData { get; set; }

        public GvrTextureInfo() { }

        public GvrTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            PixelFormat = GvrPixelFormat.Unknown;
            DataFormat = GvrDataFormat.Unknown;
            if (tex is GvrTextureInfo gvrt)
                TextureData = gvrt.TextureData;
        }

        public GvrTextureInfo(PvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            switch (tex.DataFormat)
            {
                case PvrDataFormat.Index4:
                    DataFormat = GvrDataFormat.Index4;
                    break;
                case PvrDataFormat.Index8:
                    DataFormat = GvrDataFormat.Index8;
                    break;
            }
        }

        public GvrTextureInfo(GvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            PixelFormat = tex.PixelFormat;
            DataFormat = tex.DataFormat;
            TextureData = tex.TextureData;
        }

        public GvrTextureInfo(string name, uint gbix, Bitmap bitmap)
        {
            Name = name;
            GlobalIndex = gbix;
            DataFormat = GvrDataFormat.Unknown;
            PixelFormat = GvrPixelFormat.Unknown;
            Image = bitmap;
        }

        public GvrTextureInfo(string name, MemoryStream str, GvpPalette gvp = null)
        {
            Name = name;
            TextureData = str;
            GvrTexture texture = new GvrTexture(str);
            if (gvp != null)
                texture.SetPalette(gvp);
            GlobalIndex = texture.GlobalIndex;
            DataFormat = texture.DataFormat;
            Mipmap = texture.HasMipmaps;
            PixelFormat = texture.PixelFormat;
            Image = texture.ToBitmap();
        }

        public override bool CheckMipmap()
        {
            return DataFormat != GvrDataFormat.Index4 && DataFormat != GvrDataFormat.Index8;
        }
    }

    class PvmxTextureInfo : TextureInfo
    {
        public Size? Dimensions { get; set; }

        public PvmxTextureInfo() { }

        public PvmxTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
        }

        public PvmxTextureInfo(PvmxTextureInfo tex)
            : this((TextureInfo)tex)
        {
            Dimensions = tex.Dimensions;
        }

        public PvmxTextureInfo(string name, uint gbix, Bitmap bitmap)
        {
            Name = name;
            GlobalIndex = gbix;
            Image = bitmap;
        }

        public override bool CheckMipmap()
        {
            return false;
        }
    }

    enum NinjaSurfaceFlags : uint
    {
        Mipmapped = 0x80000000,
        VQ = 0x40000000,
        NotTwiddled = 0x04000000,
        Twiddled = 0x00000000,
        Stride = 0x02000000,
        Palettized = 0x00008000
    }

    class PAKInfEntry
    {
        public byte[] filename; // 28
        public uint globalindex;
        public GvrDataFormat Type;
        public uint BitDepth; // Unused
        public GvrDataFormat PixelFormat; // Duplicate of Type
        public uint nWidth;
        public uint nHeight;
        public uint TextureSize; // Unused
        public NinjaSurfaceFlags fSurfaceFlags;
        public PAKInfEntry()
        {
            filename = new byte[28];
        }
        public PAKInfEntry(byte[] data)
        {
            filename = new byte[28];
            Array.Copy(data, filename, 0x1C);
            globalindex = BitConverter.ToUInt32(data, 0x1C);
            Type = (GvrDataFormat)BitConverter.ToUInt32(data, 0x20);
            BitDepth = BitConverter.ToUInt32(data, 0x24);
            PixelFormat = (GvrDataFormat)BitConverter.ToUInt32(data, 0x28);
            nWidth = BitConverter.ToUInt32(data, 0x2C);
            nHeight = BitConverter.ToUInt32(data, 0x30);
            TextureSize = BitConverter.ToUInt32(data, 0x34);
            fSurfaceFlags = (NinjaSurfaceFlags)BitConverter.ToUInt32(data, 0x38);
        }

        public string GetFilename()
        {
            StringBuilder sb = new StringBuilder(0x1C);
            for (int j = 0; j < 0x1C; j++)
                if (filename[j] != 0)
                    sb.Append((char)filename[j]);
                else
                    break;
            return sb.ToString();
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(filename);
            result.AddRange(BitConverter.GetBytes(globalindex));
            result.AddRange(BitConverter.GetBytes((uint)Type));
            result.AddRange(BitConverter.GetBytes(BitDepth));
            result.AddRange(BitConverter.GetBytes((uint)PixelFormat));
            result.AddRange(BitConverter.GetBytes(nWidth));
            result.AddRange(BitConverter.GetBytes(nHeight));
            result.AddRange(BitConverter.GetBytes(TextureSize));
            result.AddRange(BitConverter.GetBytes((uint)fSurfaceFlags));
            return result.ToArray();
        }
    };

    class PakTextureInfo : TextureInfo
    {
        public GvrDataFormat DataFormat { get; set; }
        public NinjaSurfaceFlags SurfaceFlags { get; set; }
        public PakTextureInfo() { }
        public string GetSurfaceFlags()
        {
            List<string> flags = new List<string>();
            if ((SurfaceFlags & NinjaSurfaceFlags.NotTwiddled) != 0)
                flags.Add("Not Twiddled");
            else
                flags.Add("Twiddled");
            if ((SurfaceFlags & NinjaSurfaceFlags.Mipmapped) != 0)
                flags.Add("Mipmapped");
            if ((SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
                flags.Add("Palettized");
            if ((SurfaceFlags & NinjaSurfaceFlags.Stride) != 0)
                flags.Add("Stride");
            if ((SurfaceFlags & NinjaSurfaceFlags.VQ) != 0)
                flags.Add("VQ");
            return string.Join(", ", flags);
        }
        public PakTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            if (tex is GvrTextureInfo gvrt)
            {
                DataFormat = gvrt.DataFormat;
                if (gvrt.DataFormat == GvrDataFormat.Index4 || gvrt.DataFormat == GvrDataFormat.Index8)
                    SurfaceFlags |= NinjaSurfaceFlags.Palettized;
            }
            else
                DataFormat = GvrDataFormat.Dxt1;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            if (tex.Mipmap)
                SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
        }

        public PakTextureInfo(string name, uint gbix, Bitmap bitmap, GvrDataFormat format = GvrDataFormat.Dxt1, NinjaSurfaceFlags flags = NinjaSurfaceFlags.Mipmapped)
        {
            Name = name;
            GlobalIndex = gbix;
            Image = bitmap;
            DataFormat = format;
            SurfaceFlags = flags;
            Mipmap = (SurfaceFlags & NinjaSurfaceFlags.Mipmapped) != 0;
        }

        public override bool CheckMipmap()
        {
            return true;
        }
    }

    static class PvmxArchive
	{
		const int FourCC = 0x584D5650; // 'PVMX'
		const byte Version = 1;

		enum dictionary_field : byte
		{
			none,
			/// <summary>
			/// 32-bit integer global index
			/// </summary>
			global_index,
			/// <summary>
			/// Null-terminated file name
			/// </summary>
			name,
			/// <summary>
			/// Two 32-bit integers defining width and height
			/// </summary>
			dimensions,
		}

		public static bool Is(byte[] file)
		{
			return file.Length > 4 && BitConverter.ToInt32(file, 0) == FourCC;
		}

		public static List<PvmxTextureInfo> GetTextures(byte[] file)
		{
			if (!Is(file)) throw new FormatException("File is not a PVMX archive.");
			if (file[4] != Version) throw new FormatException("Incorrect PVMX archive version.");
			int off = 5;
			List<PvmxTextureInfo> textures = new List<PvmxTextureInfo>();
			dictionary_field type;
			for (type = (dictionary_field)file[off++]; type != dictionary_field.none; type = (dictionary_field)file[off++])
			{
				PvmxTextureInfo tex = new PvmxTextureInfo();
				while (type != dictionary_field.none)
				{
					switch (type)
					{
						case dictionary_field.global_index:
							tex.GlobalIndex = BitConverter.ToUInt32(file, off);
							off += sizeof(uint);
							break;

						case dictionary_field.name:
							int count = 0;
							while (file[off + count] != 0)
								count++;
							tex.Name = Path.ChangeExtension(Encoding.UTF8.GetString(file, off, count), null);
							off += count + 1;
							break;

						case dictionary_field.dimensions:
							System.Drawing.Size size = new System.Drawing.Size();
							size.Width = BitConverter.ToInt32(file, off);
							off += sizeof(int);
							size.Height = BitConverter.ToInt32(file, off);
							off += sizeof(int);
							tex.Dimensions = size;
							break;

						default:
							break;
					}

					type = (dictionary_field)file[off++];
				}

				ulong offset = BitConverter.ToUInt64(file, off);
				off += sizeof(ulong);
				ulong length = BitConverter.ToUInt64(file, off);
				off += sizeof(ulong);

				using (MemoryStream ms = new MemoryStream(file, (int)offset, (int)length))
					tex.Image = new System.Drawing.Bitmap(ms);

				textures.Add(tex);
			}
			return textures;
		}

		struct OffData
		{
			public long off;
			public byte[] data;

			public OffData(long o, byte[] d)
			{
				off = o;
				data = d;
			}
		}

		public static void Save(Stream str, IEnumerable<PvmxTextureInfo> textures)
		{
			BinaryWriter bw = new BinaryWriter(str);
			bw.Write(FourCC);
			bw.Write(Version);
			List<OffData> texdata = new List<OffData>();
			foreach (PvmxTextureInfo tex in textures)
			{
				bw.Write((byte)dictionary_field.global_index);
				bw.Write(tex.GlobalIndex);
				bw.Write((byte)dictionary_field.name);
				bw.Write(tex.Name.ToCharArray());
				bw.Write(new[] { '.', 'p', 'n', 'g' });
				bw.Write((byte)0);
				if (tex.Dimensions.HasValue)
				{
					bw.Write((byte)dictionary_field.dimensions);
					bw.Write(tex.Dimensions.Value.Width);
					bw.Write(tex.Dimensions.Value.Height);
				}
				bw.Write((byte)dictionary_field.none);
				long size;
				using (MemoryStream ms = new MemoryStream())
				{
					System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(tex.Image);
					bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					texdata.Add(new OffData(str.Position, ms.ToArray()));
					size = ms.Length;
				}
				bw.Write(0ul);
				bw.Write(size);
			}
			bw.Write((byte)dictionary_field.none);
			foreach (OffData od in texdata)
			{
				long pos = str.Position;
				str.Position = od.off;
				bw.Write(pos);
				str.Position = pos;
				bw.Write(od.data);
			}
		}
	}
}
