using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using VrSharp.Gvr;
using VrSharp.Pvr;
using VrSharp.Xvr;
using static VrSharp.Xvr.DirectXTexUtility;

namespace TextureEditor
{
    enum TextureFormat { PVM, GVM, PVMX, PAK, XVM }
	enum TextureFiles { PVR, GVR, PNG, DDS, XVR, Default }

    public abstract class TextureInfo
    {
        public string Name { get; set; }
        public uint GlobalIndex { get; set; }
        public bool Mipmap { get; set; }
        public Bitmap Image { get; set; } // Preview image
		public MemoryStream TextureData { get; set; } // Texture data in target format
		public abstract bool CheckMipmap(); // Checks if the texture format allows mipmaps (the texture itself may or may not have them)
    }

    class PvrTextureInfo : TextureInfo
    {
        public PvrDataFormat DataFormat { get; set; }
        public PvrPixelFormat PixelFormat { get; set; }
    
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
			{
				TextureData = pv.TextureData;
				PixelFormat = pv.PixelFormat;
				DataFormat = pv.DataFormat;
			}
			else if (tex is GvrTextureInfo gv)
			{
				switch (gv.DataFormat)
				{
					case GvrDataFormat.Index4:
						DataFormat = PvrDataFormat.Index4;
						break;
					case GvrDataFormat.Index8:
						DataFormat = PvrDataFormat.Index8;
						break;
					default:
						DataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(tex.Image, tex.Mipmap, true);
						PixelFormat = TextureFunctions.GetPvrPixelFormatFromBitmap(tex.Image);
						break;
				}
			}
			else if (tex is PakTextureInfo pakv)
			{
				switch (pakv.PixelBitType)
				{
					case DDSPixelBitFormat.RGB565:
						PixelFormat = PvrPixelFormat.Rgb565;
						break;
					case DDSPixelBitFormat.ARGB1555:
						PixelFormat = PvrPixelFormat.Argb1555;
						break;
					case DDSPixelBitFormat.ARGB4444:
						PixelFormat = PvrPixelFormat.Argb4444;
						break;
					case DDSPixelBitFormat.ARGB8888:
						PixelFormat = PvrPixelFormat.Argb8888;
						break;
				}
			}
			else
			{
				DataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(tex.Image, tex.Mipmap, true);
				PixelFormat = TextureFunctions.GetPvrPixelFormatFromBitmap(tex.Image);
			}
        }

		public PvrTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			DataFormat = PvrDataFormat.Unknown;
			PixelFormat = PvrPixelFormat.Unknown;
			if (!TextureFunctions.CheckTextureDimensions(bitmap.Width, bitmap.Height))
				Image = new Bitmap(TextureEditor.Properties.Resources.error);
			else
			{
				Bitmap clone = (Bitmap)bitmap.Clone();
				Image = clone;
			}
		}
	
        public PvrTextureInfo(string name, MemoryStream str)
        {
            TextureData = str;
            PvrTexture texture = new PvrTexture(str);
            Name = name;
            GlobalIndex = texture.GlobalIndex;
            DataFormat = texture.DataFormat;
			Mipmap = texture.HasMipmaps;
            PixelFormat = texture.PixelFormat;
			if (texture.NeedsExternalPalette)
				Image = new Bitmap(Properties.Resources.error);
			else
				Image = texture.ToBitmap();
        }

        public override bool CheckMipmap()
        {
			// Puyo Tools doesn't allow encoding to Indexed formats with a custom palette so mipmap toggle is disabled for now.
			switch (DataFormat)
			{
				case PvrDataFormat.Index4:
				case PvrDataFormat.Index4Mipmaps:
				case PvrDataFormat.Index8:
				case PvrDataFormat.Index8Mipmaps:
					return false;
				default:
					return Image.Width == Image.Height;
			}
		}
    }

    class GvrTextureInfo : TextureInfo
    {
        public GvrDataFormat DataFormat { get; set; }
        public GvrPixelFormat PixelFormat { get; set; }

        public GvrTextureInfo() { }

        public GvrTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            PixelFormat = GvrPixelFormat.NonIndexed;
            DataFormat = GvrDataFormat.Unknown;
            if (tex is GvrTextureInfo gvrt)
            {
                PixelFormat = gvrt.PixelFormat;
                DataFormat = gvrt.DataFormat;
                TextureData = gvrt.TextureData;
            }
            else if (tex is PvrTextureInfo pvrt)
            {
                switch (pvrt.DataFormat)
                {
                    case PvrDataFormat.Index4:
                        DataFormat = GvrDataFormat.Index4;
                        break;
                    case PvrDataFormat.Index8:
                        DataFormat = GvrDataFormat.Index8;
                        break;
                    default:
                        DataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(pvrt.Image, false, true);
                        break;
                }
            }
            else
                DataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(Image, false, true);
        }

		public GvrTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			DataFormat = GvrDataFormat.Unknown;
			PixelFormat = GvrPixelFormat.NonIndexed;
			if (!TextureFunctions.CheckTextureDimensions(bitmap.Width, bitmap.Height))
				Image = new Bitmap(TextureEditor.Properties.Resources.error);
			else
			{
				Bitmap clone = (Bitmap)bitmap.Clone();
				Image = clone;
			}
		}

        public GvrTextureInfo(string name, MemoryStream str)
        {
            Name = name;
            TextureData = str;
            GvrTexture texture = new GvrTexture(str);
            GlobalIndex = texture.GlobalIndex;
            DataFormat = texture.DataFormat;
            Mipmap = texture.HasMipmaps;
            PixelFormat = texture.PixelFormat;
            if (texture.NeedsExternalPalette)
                Image = new Bitmap(Properties.Resources.error);
            else
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
			TextureData = null;
		}

        public PvmxTextureInfo(PvmxTextureInfo tex)
            : this((TextureInfo)tex)
        {
            Dimensions = tex.Dimensions;
        }

        public PvmxTextureInfo(string name, uint gbix, Bitmap bitmap, MemoryStream stream = null)
        {
            Name = name;
            GlobalIndex = gbix;
            Image = bitmap;
			TextureData = stream;
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

	public enum DDSPixelFormat
	{
		Invalid,
		AlphaPixels,
		Alpha,
		FourCC = 0x4, //Compressed RGB
		RGB = 0x40, //Uncompressed RGB
		RGBA = 0x41,
		YUV = 0x200,
	}
	
	public enum DDSPixelBitFormat
	{
		Invalid,
		RGB444, //RGB444
		ARGB4444, //ARGB4444
		RGB565, //RGB565
		ARGB1555, //ARGB1555
		ARGB8888, //ARGB8888
		BGRA8888, //BGRA8888
		DXT1,
		DXT2,
		DXT3,
		DXT4,
		DXT5
	}

	class PAKInfEntry
    {
        public byte[] filename; // 28
        public uint globalindex;
        public GvrDataFormat TypeInf;
        public uint BitDepth; // Unused
        public GvrDataFormat PixelFormatInf; // Duplicate of Type
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
            TypeInf = (GvrDataFormat)BitConverter.ToUInt32(data, 0x20);
            BitDepth = BitConverter.ToUInt32(data, 0x24);
            PixelFormatInf = (GvrDataFormat)BitConverter.ToUInt32(data, 0x28);
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

    class PakTextureInfo : TextureInfo
    {
		public GvrDataFormat DataFormatInf { get; set; }
        public DDSPixelFormat DataFormat { get; set; }
		public DDSPixelBitFormat PixelBitType { get; set; }
        public NinjaSurfaceFlags SurfaceFlags { get; set; }
		public bool IsPAK { get; set; }
        public PakTextureInfo() { }
		public TextureFunctions.TextureFileFormat FileFormat { get; set; }
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
		public string OriginalFileExtension { get; set; }
		public string GetPixelFormatInf()
		{
			return DataFormatInf.ToString();
		}
		public string GetPixelFormat()
		{
			return DataFormat.ToString();
		}
		public string GetPixelSubType()
		{
			return PixelBitType.ToString();
		}

		public PakTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
			if (tex is GvrTextureInfo gvrt)
			{
				IsPAK = false;
				DataFormatInf = gvrt.DataFormat;
				if (gvrt.DataFormat == GvrDataFormat.Index4 || gvrt.DataFormat == GvrDataFormat.Index8)
					SurfaceFlags |= NinjaSurfaceFlags.Palettized;
				switch (gvrt.DataFormat)
				{
					default:
					case GvrDataFormat.Rgb565:
						DataFormat = DDSPixelFormat.RGB;
						PixelBitType = DDSPixelBitFormat.RGB565;
						DataFormatInf = GvrDataFormat.Dxt1;
						break;
					case GvrDataFormat.Dxt1:
						DataFormat = DDSPixelFormat.RGBA;
						PixelBitType = DDSPixelBitFormat.ARGB1555;
						DataFormatInf = GvrDataFormat.Dxt1;
						break;
					case GvrDataFormat.Rgb5a3:
						DataFormat = DDSPixelFormat.RGBA;
						PixelBitType = DDSPixelBitFormat.ARGB8888;
						DataFormatInf = GvrDataFormat.Rgb5a3;
						break;
					case GvrDataFormat.Argb8888:
						DataFormat = DDSPixelFormat.RGBA;
						PixelBitType = DDSPixelBitFormat.ARGB8888;
						DataFormatInf = GvrDataFormat.Rgb5a3;
						break;
				}
			}
			else if (tex is PvrTextureInfo pvrt)
			{
				IsPAK = false;
				switch (pvrt.PixelFormat)
				{
					default:
					case PvrPixelFormat.Rgb565:
						DataFormat = DDSPixelFormat.RGB;
						PixelBitType = DDSPixelBitFormat.RGB565;
						DataFormatInf = GvrDataFormat.Dxt1;
						break;
					case PvrPixelFormat.Argb1555:
						DataFormat = DDSPixelFormat.RGBA;
						PixelBitType = DDSPixelBitFormat.ARGB1555;
						DataFormatInf = GvrDataFormat.Dxt1;
						break;
					case PvrPixelFormat.Argb4444:
						DataFormat = DDSPixelFormat.RGBA;
						PixelBitType = DDSPixelBitFormat.ARGB4444;
						DataFormatInf = GvrDataFormat.Rgb5a3;
						break;
					case PvrPixelFormat.Argb8888:
						DataFormat = DDSPixelFormat.RGBA;
						PixelBitType = DDSPixelBitFormat.ARGB8888;
						DataFormatInf = GvrDataFormat.Rgb5a3;
						break;
				}
				if (pvrt.DataFormat == PvrDataFormat.Index4)
				{
					DataFormatInf = GvrDataFormat.Index4;
					SurfaceFlags |= NinjaSurfaceFlags.Palettized;
				}
				if (pvrt.DataFormat == PvrDataFormat.Index8)
				{
					DataFormatInf = GvrDataFormat.Index8;
					SurfaceFlags |= NinjaSurfaceFlags.Palettized;
				}
				TextureData = pvrt.TextureData;
			}
			else
			{
				DataFormatInf = GvrDataFormat.Dxt1;
			}
			if (tex is PakTextureInfo pk)
			{
				IsPAK = true;
				OriginalFileExtension = pk.OriginalFileExtension;
				DataFormat = TextureFunctions.IdentifyPAKPixelFormat(TextureData);
			}
			else
			{
				FileFormat = TextureFunctions.IdentifyTextureFileFormat(TextureData);
				if (FileFormat != TextureFunctions.TextureFileFormat.PNG)
					OriginalFileExtension = ".dds";
				else
					OriginalFileExtension = ".png";
			}
			Image = tex.Image;
            Mipmap = tex.Mipmap;
            if (tex.Mipmap)
                SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
			TextureData = null;
        }

        public PakTextureInfo(string name, uint gbix, Bitmap bitmap, GvrDataFormat format = GvrDataFormat.Dxt1, DDSPixelFormat ppx = DDSPixelFormat.RGB, DDSPixelBitFormat pbx = DDSPixelBitFormat.Invalid, NinjaSurfaceFlags flags = NinjaSurfaceFlags.Mipmapped, MemoryStream str = null, string origExt = ".dds")
        {
            Name = name;
            GlobalIndex = gbix;
            Image = bitmap;
            DataFormatInf = format;
			DataFormat = ppx;
			PixelBitType = pbx;
            SurfaceFlags = flags;
            Mipmap = (SurfaceFlags & NinjaSurfaceFlags.Mipmapped) != 0;
			TextureData = str;
			OriginalFileExtension = origExt;
        }

        public override bool CheckMipmap()
        {
            return true;
        }
    }

	class XvrTextureInfo : TextureInfo
	{
		public DXGIFormat DataFormat { get; set; }
		public DXGIFormat PixelFormat { get; set; }
		public bool useAlpha { get; set; }
		public XvrTextureInfo() { }

		public XvrTextureInfo(TextureInfo tex)
		{
			Name = tex.Name;
			GlobalIndex = tex.GlobalIndex;
			Image = tex.Image;
			Mipmap = tex.Mipmap;
			if (tex is PvrTextureInfo pv)
			{
				TextureData = pv.TextureData;
				PixelFormat = DXGIFormat.BC3UNORM;
				DataFormat = DXGIFormat.BC3UNORM;
			}
			else if (tex is GvrTextureInfo gv)
			{
				PixelFormat = DXGIFormat.BC3UNORM;
				DataFormat = DXGIFormat.BC3UNORM;
			}
			else
			{
				PixelFormat = DXGIFormat.BC3UNORM;
				DataFormat = DXGIFormat.BC3UNORM;
			}
		}

		public XvrTextureInfo(string name, uint gbix, Bitmap bitmap)
		{
			Name = name;
			GlobalIndex = gbix;
			DataFormat = DXGIFormat.BC3UNORM;
			PixelFormat = DXGIFormat.BC3UNORM;
			if (!TextureFunctions.CheckTextureDimensions(bitmap.Width, bitmap.Height))
				Image = new Bitmap(TextureEditor.Properties.Resources.error);
			else
				Image = bitmap;
		}

		public XvrTextureInfo(string name, MemoryStream str)
		{
			TextureData = str;
			XvrTexture texture = new XvrTexture(str);
			Name = name;
			useAlpha = texture.UseAlpha;
			GlobalIndex = texture.GlobalIndex;
			DataFormat = texture.DXGIPixelFormat;
			Mipmap = texture.HasMipmaps;
			PixelFormat = texture.DXGIPixelFormat;
			Image = texture.ToBitmap();
		}

		public override bool CheckMipmap()
		{
			return true;
		}
	}
}
