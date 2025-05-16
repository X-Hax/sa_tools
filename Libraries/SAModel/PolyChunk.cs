using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace SAModel
{
	[Serializable]
	public abstract class PolyChunk : ICloneable
	{
		public ushort Header { get; set; }

		public ChunkType Type
		{
			get { return (ChunkType)(Header & 0xFF); }
			protected set { Header = (ushort)((Header & 0xFF00) | (byte)value); }
		}

		public byte Flags
		{
			get { return (byte)(Header >> 8); }
			set { Header = (ushort)((Header & 0xFF) | (ushort)(value << 8)); }
		}

		public abstract int ByteSize { get; }

		public static PolyChunk Load(byte[] file, int address)
		{
			ChunkType type = (ChunkType)(ByteConverter.ToUInt16(file, address) & 0xFF);
			switch (type)
			{
				case ChunkType.Null:
					return new PolyChunkNull(file, address);
				case ChunkType.Bits_BlendAlpha:
					return new PolyChunkBitsBlendAlpha(file, address);
				case ChunkType.Bits_MipmapDAdjust:
					return new PolyChunkBitsMipmapDAdjust(file, address);
				case ChunkType.Bits_SpecularExponent:
					return new PolyChunkBitsSpecularExponent(file, address);
				case ChunkType.Bits_CachePolygonList:
					return new PolyChunkBitsCachePolygonList(file, address);
				case ChunkType.Bits_DrawPolygonList:
					return new PolyChunkBitsDrawPolygonList(file, address);
				case ChunkType.Tiny_TextureID:
				case ChunkType.Tiny_TextureID2:
					return new PolyChunkTinyTextureID(file, address);
				case ChunkType.Material_Diffuse:
				case ChunkType.Material_Ambient:
				case ChunkType.Material_DiffuseAmbient:
				case ChunkType.Material_Specular:
				case ChunkType.Material_DiffuseSpecular:
				case ChunkType.Material_AmbientSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Diffuse2:
				case ChunkType.Material_Ambient2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_Specular2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					return new PolyChunkMaterial(file, address);
				case ChunkType.Material_Bump:
					return new PolyChunkMaterialBump(file, address);
				case ChunkType.Volume_Polygon3:
				case ChunkType.Volume_Polygon4:
				case ChunkType.Volume_Strip:
					return new PolyChunkVolume(file, address);
				case ChunkType.Strip_Strip:
				case ChunkType.Strip_StripUVN:
				case ChunkType.Strip_StripUVH:
				case ChunkType.Strip_StripColor:
				case ChunkType.Strip_StripUVNColor:
				case ChunkType.Strip_StripUVHColor:
				case ChunkType.Strip_Strip2:
				case ChunkType.Strip_StripUVN2:
				case ChunkType.Strip_StripUVH2:
					return new PolyChunkStrip(file, address);
				case ChunkType.End:
					return new PolyChunkEnd(file, address);
				default:
					throw new NotSupportedException("Unsupported chunk type " + type + " at " + address.ToString("X8") + ".");
			}
		}

		public abstract byte[] GetBytes();

		public abstract void ToNJA(TextWriter writer);

		object ICloneable.Clone() => Clone();

		public virtual PolyChunk Clone() => (PolyChunk)MemberwiseClone();

		public static List<PolyChunk> Merge(List<PolyChunk> source)
		{
			var strips = new List<(NJS_MATERIAL mat, List<PolyChunk> pre, PolyChunkStrip str)>();
			NJS_MATERIAL curmat = new NJS_MATERIAL();
			var cnks = new List<PolyChunk>();
			foreach (var chunk in source)
			{
				curmat.UpdateFromPolyChunk(chunk);
				if (chunk is PolyChunkStrip str)
				{
					bool found = false;
					foreach (var set in strips)
						if (set.mat.Equals(curmat) && set.str.Type == str.Type)
						{
							found = true;
							set.str.Strips.AddRange(str.Strips);
							break;
						}
					if (!found)
						strips.Add((curmat, cnks, str));
					curmat = new NJS_MATERIAL(curmat);
					cnks = new List<PolyChunk>();
				}
				else
					cnks.Add(chunk);
			}
			return strips.SelectMany(a => a.pre.Append(a.str)).ToList();
		}
	}

	[Serializable]
	public abstract class PolyChunkBits : PolyChunk
	{
		public override int ByteSize
		{
			get { return 2; }
		}

		public override byte[] GetBytes()
		{
			return ByteConverter.GetBytes(Header);
		}
	}

	[Serializable]
	public class PolyChunkNull : PolyChunkBits
	{
		public PolyChunkNull()
		{
			Type = ChunkType.Null;
		}

		public PolyChunkNull(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine("\tCnkNull(),");
		}
	}

	[Serializable]
	internal class PolyChunkEnd : PolyChunkBits
	{
		public PolyChunkEnd()
		{
			Type = ChunkType.End;
		}

		public PolyChunkEnd(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine("\tCnkEnd()");
		}
	}

	[Serializable]
	public class PolyChunkBitsBlendAlpha : PolyChunkBits
	{
		public AlphaInstruction SourceAlpha
		{
			get { return (AlphaInstruction)((Flags >> 3) & 7); }
			set { Flags = (byte)((Flags & ~0x38) | ((byte)value << 3)); }
		}

		public AlphaInstruction DestinationAlpha
		{
			get { return (AlphaInstruction)(Flags & 7); }
			set { Flags = (byte)((Flags & ~7) | (byte)value); }
		}

		public PolyChunkBitsBlendAlpha()
		{
			Type = ChunkType.Bits_BlendAlpha;
		}

		public PolyChunkBitsBlendAlpha(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			string blendSrcStr = "FBS_SA";
			switch (SourceAlpha)
			{
				case AlphaInstruction.Zero:
					blendSrcStr = "FBS_ZER";
					break;
				case AlphaInstruction.One:
					blendSrcStr = "FBS_ONE";
					break;
				case AlphaInstruction.OtherColor:
					blendSrcStr = "FBS_OC";
					break;
				case AlphaInstruction.InverseOtherColor:
					blendSrcStr = "FBS_IOC";
					break;
				case AlphaInstruction.SourceAlpha:
					blendSrcStr = "FBS_SA";
					break;
				case AlphaInstruction.InverseSourceAlpha:
					blendSrcStr = "FBS_ISA";
					break;
				case AlphaInstruction.DestinationAlpha:
					blendSrcStr = "FBS_DA";
					break;
				case AlphaInstruction.InverseDestinationAlpha:
					blendSrcStr = "FBS_IDA";
					break;
			}

			string blendDstStr = "FBD_ISA";
			switch (DestinationAlpha)
			{
				case AlphaInstruction.Zero:
					blendDstStr = "FBD_ZER";
					break;
				case AlphaInstruction.One:
					blendDstStr = "FBD_ONE";
					break;
				case AlphaInstruction.OtherColor:
					blendDstStr = "FBD_OC";
					break;
				case AlphaInstruction.InverseOtherColor:
					blendDstStr = "FBD_IOC";
					break;
				case AlphaInstruction.SourceAlpha:
					blendDstStr = "FBD_SA";
					break;
				case AlphaInstruction.InverseSourceAlpha:
					blendDstStr = "FBD_ISA";
					break;
				case AlphaInstruction.DestinationAlpha:
					blendDstStr = "FBD_DA";
					break;
				case AlphaInstruction.InverseDestinationAlpha:
					blendDstStr = "FBD_IDA";
					break;
			}

			writer.WriteLine("\tCnkB_BA( " + blendSrcStr.ToString() + "|" + blendDstStr.ToString() + " ),");
		}
	}

	[Serializable]
	public class PolyChunkBitsMipmapDAdjust : PolyChunkBits
	{
		public float MipmapDAdjust
		{
			get { return (Flags & 0xF) * 0.25f; }
			set
			{
				Flags = (byte)((Flags & 0xF0) | (byte)Math.Max(0, Math.Min(0xF, Math.Round(value / 0.25, MidpointRounding.AwayFromZero))));
			}
		}

		public PolyChunkBitsMipmapDAdjust()
		{
			Type = ChunkType.Bits_MipmapDAdjust;
		}

		public PolyChunkBitsMipmapDAdjust(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			string bits = string.Empty;
			switch (Flags & 0xF)
			{
				case 0:
					return;
				case 1:
					bits = "FDA_025";
					break;
				case 2:
					bits = "FDA_050";
					break;
				case 3:
					bits = "FDA_075";
					break;
				case 4:
					bits = "FDA_100";
					break;
				case 5:
					bits = "FDA_125";
					break;
				case 6:
					bits = "FDA_150";
					break;
				case 7:
					bits = "FDA_175";
					break;
				case 8:
					bits = "FDA_200";
					break;
				case 9:
					bits = "FDA_225";
					break;
				case 10:
					bits = "FDA_250";
					break;
				case 11:
					bits = "FDA_275";
					break;
				case 12:
					bits = "FDA_300";
					break;
				case 13:
					bits = "FDA_325";
					break;
				case 14:
					bits = "FDA_350";
					break;
				case 15:
					bits = "FDA_375";
					break;
			}
			writer.WriteLine("\tCnkB_DA( " + bits + " ),");
		}
	}

	[Serializable]
	public class PolyChunkBitsSpecularExponent : PolyChunkBits
	{
		public byte SpecularExponent
		{
			get { return (byte)(Flags & 0x1F); }
			set { Flags = (byte)((Flags & ~0x1F) | Math.Min(value, (byte)16)); }
		}

		public PolyChunkBitsSpecularExponent()
		{
			Type = ChunkType.Bits_SpecularExponent;
		}

		public PolyChunkBitsSpecularExponent(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			throw new NotSupportedException("Unsupported chunk type " + Type + ".");
		}
	}

	[Serializable]
	public class PolyChunkBitsCachePolygonList : PolyChunkBits
	{
		public byte List
		{
			get { return Flags; }
			set { Flags = value; }
		}

		public PolyChunkBitsCachePolygonList()
		{
			Type = ChunkType.Bits_CachePolygonList;
		}

		public PolyChunkBitsCachePolygonList(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine("\tCnkB_CP( " + List + " ),");
		}
	}

	[Serializable]
	public class PolyChunkBitsDrawPolygonList : PolyChunkBits
	{
		public byte List
		{
			get { return Flags; }
			set { Flags = value; }
		}

		public PolyChunkBitsDrawPolygonList()
		{
			Type = ChunkType.Bits_DrawPolygonList;
		}

		public PolyChunkBitsDrawPolygonList(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
		}

		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine("\tCnkB_DP( " + List + " ),");
		}
	}

	[Serializable]
	public class PolyChunkTinyTextureID : PolyChunk
	{
		public bool Second { get; set; }

		public float MipmapDAdjust
		{
			get { return (Flags & 0xF) * 0.25f; }
			set
			{
				Flags = (byte)((Flags & 0xF0) | (byte)Math.Max(0, Math.Min(0xF, Math.Round(value / 0.25, MidpointRounding.AwayFromZero))));
			}
		}

		public bool ClampV
		{
			get { return (Flags & 0x10) == 0x10; }
			set { Flags = (byte)((Flags & ~0x10) | (value ? 0x10 : 0)); }
		}

		public bool ClampU
		{
			get { return (Flags & 0x20) == 0x20; }
			set { Flags = (byte)((Flags & ~0x20) | (value ? 0x20 : 0)); }
		}

		public bool FlipV
		{
			get { return (Flags & 0x40) == 0x40; }
			set { Flags = (byte)((Flags & ~0x40) | (value ? 0x40 : 0)); }
		}

		public bool FlipU
		{
			get { return (Flags & 0x80) == 0x80; }
			set { Flags = (byte)((Flags & ~0x80) | (value ? 0x80 : 0)); }
		}

		public ushort Data { get; set; }

		public ushort TextureID
		{
			get { return (ushort)(Data & 0x1FFF); }
			set { Data = (ushort)((Data & ~0x1FFF) | Math.Min(value, (ushort)0x1FFF)); }
		}

		public bool SuperSample
		{
			get { return (Data & 0x2000) == 0x2000; }
			set { Data = (ushort)((Data & ~0x2000) | (value ? 0x2000 : 0)); }
		}

		public FilterMode FilterMode
		{
			get { return (FilterMode)(Data >> 14); }
			set { Data = (ushort)((Data & ~0xC000) | ((ushort)value << 14)); }
		}

		public override int ByteSize
		{
			get { return 4; }
		}

		public PolyChunkTinyTextureID()
		{
			Type = ChunkType.Tiny_TextureID;
		}

		public PolyChunkTinyTextureID(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			Second = Type == ChunkType.Tiny_TextureID2;
			Data = ByteConverter.ToUInt16(file, address + 2);
		}

		public PolyChunkTinyTextureID(NJS_MATERIAL mat)
			: this()
		{
			MipmapDAdjust = mat.MipmapDAdjust;
			ClampV = mat.ClampV;
			ClampU = mat.ClampU;
			FlipV = mat.FlipV;
			FlipU = mat.FlipU;
			TextureID = (ushort)mat.TextureID;
			SuperSample = mat.SuperSample;
			FilterMode = mat.FilterMode;
		}

		public override byte[] GetBytes()
		{
			Type = Second ? ChunkType.Tiny_TextureID2 : ChunkType.Tiny_TextureID;
			List<byte> result = new List<byte>(4);
			result.AddRange(ByteConverter.GetBytes(Header));
			result.AddRange(ByteConverter.GetBytes(Data));
			return result.ToArray();
		}

		public override void ToNJA(TextWriter writer)
		{
			string headbits = string.Empty;
			if (FlipU)
				headbits += "FFL_U|";
			if (FlipV)
				headbits += "FFL_V|";
			if (ClampU)
				headbits += "FCL_U|";
			if (ClampV)
				headbits += "FCL_V|";

			switch(Flags & 0xF)
			{
				case 0:
				default:
					if (headbits != string.Empty)
						headbits = headbits.Remove(headbits.Length - 1);
					break;
				case 1:
					headbits += "FDA_025";
					break;
				case 2:
					headbits += "FDA_050";
					break;
				case 3:
					headbits += "FDA_075";
					break;
				case 4:
					headbits += "FDA_100";
					break;
				case 5:
					headbits += "FDA_125";
					break;
				case 6:
					headbits += "FDA_150";
					break;
				case 7:
					headbits += "FDA_175";
					break;
				case 8:
					headbits += "FDA_200";
					break;
				case 9:
					headbits += "FDA_225";
					break;
				case 10:
					headbits += "FDA_250";
					break;
				case 11:
					headbits += "FDA_275";
					break;
				case 12:
					headbits += "FDA_300";
					break;
				case 13:
					headbits += "FDA_325";
					break;
				case 14:
					headbits += "FDA_350";
					break;
				case 15:
					headbits += "FDA_375";
					break;
			}

			if (headbits == string.Empty)
				headbits = "0x0";

			writer.Write("\tCnkT_TID( " + headbits + " ), _TID( ");

			switch (FilterMode)
			{
			case FilterMode.PointSampled:
				writer.Write("FFM_PS");
				break;
			case FilterMode.Bilinear:
				writer.Write("FFM_BF");
				break;
			case FilterMode.Trilinear:
				writer.Write("FFM_TF");
				break;
			}

			if (SuperSample)
				writer.Write("|FSS");

			writer.WriteLine(", " + TextureID.ToString() + " ),");
		}
	}

	[Serializable]
	public abstract class PolyChunkSize : PolyChunk
	{
		public ushort Size { get; protected set; }

		public override int ByteSize
		{
			get { return (Size * 2) + 4; }
		}

		public override byte[] GetBytes()
		{
			List<byte> result = new List<byte>(4);
			result.AddRange(ByteConverter.GetBytes(Header));
			result.AddRange(ByteConverter.GetBytes(Size));
			return result.ToArray();
		}
	}

	[Serializable]
	public class PolyChunkMaterial : PolyChunkSize
	{
		public AlphaInstruction SourceAlpha
		{
			get { return (AlphaInstruction)((Flags >> 3) & 7); }
			set { Flags = (byte)((Flags & ~0x38) | ((byte)value << 3)); }
		}

		public AlphaInstruction DestinationAlpha
		{
			get { return (AlphaInstruction)(Flags & 7); }
			set { Flags = (byte)((Flags & ~7) | (byte)value); }
		}

		public Color? Diffuse { get; set; }
		public Color? Ambient { get; set; }
		public Color? Specular { get; set; }
		public byte SpecularExponent { get; set; }
		public bool Second { get; set; }

		public PolyChunkMaterial()
		{
			Type = ChunkType.Material_Diffuse;
			Diffuse = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
		}

		public PolyChunkMaterial(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			switch (Type)
			{
				case ChunkType.Material_Diffuse:
				case ChunkType.Material_DiffuseAmbient:
				case ChunkType.Material_DiffuseSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Diffuse2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Diffuse = VColor.FromBytes(file, address, ColorType.ARGB8888_16);
					address += VColor.Size(ColorType.ARGB8888_16);
					break;
			}
			switch (Type)
			{
				case ChunkType.Material_Ambient:
				case ChunkType.Material_DiffuseAmbient:
				case ChunkType.Material_AmbientSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Ambient2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Ambient = VColor.FromBytes(file, address, ColorType.XRGB8888_16);
					address += VColor.Size(ColorType.XRGB8888_16);
					break;
			}
			switch (Type)
			{
				case ChunkType.Material_Specular:
				case ChunkType.Material_DiffuseSpecular:
				case ChunkType.Material_AmbientSpecular:
				case ChunkType.Material_DiffuseAmbientSpecular:
				case ChunkType.Material_Specular2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Specular = VColor.FromBytes(file, address, ColorType.XRGB8888_16);
					SpecularExponent = (byte)(ByteConverter.ToUInt16(file, address + 2) >> 8);
					address += VColor.Size(ColorType.XRGB8888_16);
					break;
			}
			switch (Type)
			{
				case ChunkType.Material_Diffuse2:
				case ChunkType.Material_Ambient2:
				case ChunkType.Material_DiffuseAmbient2:
				case ChunkType.Material_Specular2:
				case ChunkType.Material_DiffuseSpecular2:
				case ChunkType.Material_AmbientSpecular2:
				case ChunkType.Material_DiffuseAmbientSpecular2:
					Second = true;
					break;
			}
		}

		public PolyChunkMaterial(NJS_MATERIAL mat)
		{
			SourceAlpha = mat.SourceAlpha;
			DestinationAlpha = mat.DestinationAlpha;
			Diffuse = mat.DiffuseColor;
			if (mat.AmbientColor != Color.White)
				Ambient = mat.AmbientColor;
			if (mat.SpecularColor != Color.Transparent)
			{
				Specular = mat.SpecularColor;
				SpecularExponent = (byte)mat.Exponent;
			}
		}

		public override byte[] GetBytes()
		{
			byte t = (byte)ChunkType.Material;
			Size = 0;
			if (Diffuse.HasValue)
			{
				t |= 1;
				Size += 2;
			}
			if (Ambient.HasValue)
			{
				t |= 2;
				Size += 2;
			}
			if (Specular.HasValue)
			{
				t |= 4;
				Size += 2;
			}
			if (Second)
				t |= 8;
			Type = (ChunkType)t;
			List<byte> result = new List<byte>(base.GetBytes());
			if (Diffuse.HasValue)
				result.AddRange(VColor.GetBytes(Diffuse.Value, ColorType.ARGB8888_16));
			if (Ambient.HasValue)
				result.AddRange(VColor.GetBytes(Ambient.Value, ColorType.XRGB8888_16));
			if (Specular.HasValue)
			{
				int i = Specular.Value.ToArgb();
				result.AddRange(ByteConverter.GetBytes((ushort)(i & 0xFFFF)));
				result.AddRange(ByteConverter.GetBytes((ushort)(((i >> 16) & 0xFF) | SpecularExponent << 8)));
			}
			return result.ToArray();
		}

		public override void ToNJA(TextWriter writer)
		{
			string letters = string.Empty;
			int size = 0;

			if (Diffuse.HasValue)
			{
				letters += 'D';
				size += 2;
			}
			if (Ambient.HasValue)
			{
				letters += 'A';
				size += 2;
			}
			if (Specular.HasValue)
			{
				letters += 'S';
				size += 2;
			}
			if (Second)
			{
				letters += '2';
			}

			string blendSrcStr = "FBS_SA";
			switch (SourceAlpha)
			{
				case AlphaInstruction.Zero:
					blendSrcStr = "FBS_ZER";
					break;
				case AlphaInstruction.One:
					blendSrcStr = "FBS_ONE";
					break;
				case AlphaInstruction.OtherColor:
					blendSrcStr = "FBS_OC";
					break;
				case AlphaInstruction.InverseOtherColor:
					blendSrcStr = "FBS_IOC";
					break;
				case AlphaInstruction.SourceAlpha:
					blendSrcStr = "FBS_SA";
					break;
				case AlphaInstruction.InverseSourceAlpha:
					blendSrcStr = "FBS_ISA";
					break;
				case AlphaInstruction.DestinationAlpha:
					blendSrcStr = "FBS_DA";
					break;
				case AlphaInstruction.InverseDestinationAlpha:
					blendSrcStr = "FBS_IDA";
					break;
			}

			string blendDstStr = "FBD_ISA";
			switch (DestinationAlpha)
			{
				case AlphaInstruction.Zero:
					blendDstStr = "FBD_ZER";
					break;
				case AlphaInstruction.One:
					blendDstStr = "FBD_ONE";
					break;
				case AlphaInstruction.OtherColor:
					blendDstStr = "FBD_OC";
					break;
				case AlphaInstruction.InverseOtherColor:
					blendDstStr = "FBD_IOC";
					break;
				case AlphaInstruction.SourceAlpha:
					blendDstStr = "FBD_SA";
					break;
				case AlphaInstruction.InverseSourceAlpha:
					blendDstStr = "FBD_ISA";
					break;
				case AlphaInstruction.DestinationAlpha:
					blendDstStr = "FBD_DA";
					break;
				case AlphaInstruction.InverseDestinationAlpha:
					blendDstStr = "FBD_IDA";
					break;
			}

			writer.WriteLine("\tCnkM_" + letters + "( " + blendSrcStr.ToString() + "|" + blendDstStr.ToString() + " ), " + size.ToString() +",");
			if (Diffuse.HasValue)
				writer.WriteLine("\tMDiff( " + Diffuse.Value.A.ToString() + ", " + Diffuse.Value.R.ToString() + ", " + Diffuse.Value.G.ToString() + ", " + Diffuse.Value.B.ToString() + " ),");
			if (Ambient.HasValue)
				writer.WriteLine("\tMAmbi( " + Ambient.Value.A.ToString() + ", " + Ambient.Value.R.ToString() + ", " + Ambient.Value.G.ToString() + ", " + Ambient.Value.B.ToString() + " ),");
			if (Specular.HasValue)
				writer.WriteLine("\tMSpec( " + SpecularExponent.ToString() + ", " + Specular.Value.R.ToString() + ", " + Specular.Value.G.ToString() + ", " + Specular.Value.B.ToString() + " ),");
		}
	}

	[Serializable]
	public class PolyChunkMaterialBump : PolyChunkSize
	{
		public ushort DX { get; set; }
		public ushort DY { get; set; }
		public ushort DZ { get; set; }
		public ushort UX { get; set; }
		public ushort UY { get; set; }
		public ushort UZ { get; set; }

		public PolyChunkMaterialBump()
		{
			Type = ChunkType.Material_Bump;
			Size = 6;
		}

		public PolyChunkMaterialBump(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			DX = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			DY = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			DZ = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			UX = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			UY = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			UZ = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
		}

		public override byte[] GetBytes()
		{
			List<byte> result = new List<byte>(base.GetBytes());
			result.AddRange(ByteConverter.GetBytes(DX));
			result.AddRange(ByteConverter.GetBytes(DY));
			result.AddRange(ByteConverter.GetBytes(DZ));
			result.AddRange(ByteConverter.GetBytes(UX));
			result.AddRange(ByteConverter.GetBytes(UY));
			result.AddRange(ByteConverter.GetBytes(UZ));
			return result.ToArray();
		}

		public override void ToNJA(TextWriter writer)
		{
			throw new NotSupportedException("Unsupported chunk type " + Type + ".");
		}
	}

	[Serializable]
	public class PolyChunkVolume : PolyChunkSize
	{
		[Serializable]
		public sealed class Triangle : Poly
		{
			public ushort? UserFlags1 { get; private set; }
			public ushort? UserFlags2 { get; private set; }
			public ushort? UserFlags3 { get; private set; }

			public Triangle()
			{
				Indexes = new ushort[3];
			}

			public Triangle(byte[] file, int address, byte userFlags)
				: this()
			{
				Indexes[0] = ByteConverter.ToUInt16(file, address);
				Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
				Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
				if (userFlags > 0)
				{
					UserFlags1 = ByteConverter.ToUInt16(file, address + 6);
					if (userFlags > 1)
					{
						UserFlags2 = ByteConverter.ToUInt16(file, address + 8);
						if (userFlags > 2)
							UserFlags3 = ByteConverter.ToUInt16(file, address + 10);
					}
				}
			}

			public override byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				foreach (ushort item in Indexes)
					result.AddRange(ByteConverter.GetBytes(item));
				if (UserFlags1.HasValue)
				{
					result.AddRange(ByteConverter.GetBytes(UserFlags1.Value));
					if (UserFlags2.HasValue)
					{
						result.AddRange(ByteConverter.GetBytes(UserFlags2.Value));
						if (UserFlags3.HasValue)
							result.AddRange(ByteConverter.GetBytes(UserFlags3.Value));
					}
				}
				return result.ToArray();
			}

			public void ToNJA(TextWriter writer)
			{
				if (UserFlags1 != null)
				{
					writer.Write(Environment.NewLine);
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write("\t" + Indexes[i].ToString() + ",");

						if (UserFlags1 != null && i > 1)
						{
							if (UserFlags3 != null)
							{
								writer.Write(" \tUf3( " + UserFlags1.ToString() + ", " + UserFlags2.ToString() + ", " + UserFlags3.ToString() + "),");
							}
							else if (UserFlags2 != null)
							{
								writer.Write(" \tUf2( " + UserFlags1.ToString() + ", " + UserFlags2.ToString() + "),");
							}
							else
							{
								writer.Write(" \tUf1( " + UserFlags1.ToString() + "),");
							}
						}

						writer.Write(Environment.NewLine);
					}
				}
				else
				{
					writer.Write("  ");
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write(Indexes[i].ToString() + ", ");
						if (((i + 1) % 10) == 0 && i != Indexes.Length - 1)
							writer.Write(Environment.NewLine + "                   ");
					}
					writer.Write(Environment.NewLine);
				}
			}

			public override int Size
			{
				get
				{
					int size = 6;
					if (UserFlags1.HasValue)
					{
						size += 2;
						if (UserFlags2.HasValue)
						{
							size += 2;
							if (UserFlags3.HasValue)
								size += 2;
						}
					}
					return size;
				}
			}
		}

		[Serializable]
		public sealed class Quad : Poly
		{
			public ushort? UserFlags1 { get; private set; }
			public ushort? UserFlags2 { get; private set; }
			public ushort? UserFlags3 { get; private set; }

			public Quad()
			{
				Indexes = new ushort[4];
			}

			public Quad(byte[] file, int address, byte userFlags)
				: this()
			{
				Indexes[0] = ByteConverter.ToUInt16(file, address);
				Indexes[1] = ByteConverter.ToUInt16(file, address + 2);
				Indexes[2] = ByteConverter.ToUInt16(file, address + 4);
				Indexes[3] = ByteConverter.ToUInt16(file, address + 6);
				if (userFlags > 0)
				{
					UserFlags1 = ByteConverter.ToUInt16(file, address + 8);
					if (userFlags > 1)
					{
						UserFlags2 = ByteConverter.ToUInt16(file, address + 10);
						if (userFlags > 2)
							UserFlags3 = ByteConverter.ToUInt16(file, address + 12);
					}
				}
			}

			public override byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				foreach (ushort item in Indexes)
					result.AddRange(ByteConverter.GetBytes(item));
				if (UserFlags1.HasValue)
				{
					result.AddRange(ByteConverter.GetBytes(UserFlags1.Value));
					if (UserFlags2.HasValue)
					{
						result.AddRange(ByteConverter.GetBytes(UserFlags2.Value));
						if (UserFlags3.HasValue)
							result.AddRange(ByteConverter.GetBytes(UserFlags3.Value));
					}
				}
				return result.ToArray();
			}

			public void ToNJA(TextWriter writer)
			{
				if (UserFlags1 != null)
				{
					writer.Write(Environment.NewLine);
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write("\t" + Indexes[i].ToString() + ",");

						if (UserFlags1 != null && i > 1)
						{
							if (UserFlags3 != null)
							{
								writer.Write(" \tUf3( " + UserFlags1.ToString() + ", " + UserFlags2.ToString() + ", " + UserFlags3.ToString() + "),");
							}
							else if (UserFlags2 != null)
							{
								writer.Write(" \tUf2( " + UserFlags1.ToString() + ", " + UserFlags2.ToString() + "),");
							}
							else
							{
								writer.Write(" \tUf1( " + UserFlags1.ToString() + "),");
							}
						}

						writer.Write(Environment.NewLine);
					}
				}
				else
				{
					writer.Write("  ");
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write(Indexes[i].ToString() + ", ");
						if (((i + 1) % 10) == 0 && i != Indexes.Length - 1)
							writer.Write(Environment.NewLine + "                   ");
					}
					writer.Write(Environment.NewLine);
				}
			}

			public override int Size
			{
				get
				{
					int size = 8;
					if (UserFlags1.HasValue)
					{
						size += 2;
						if (UserFlags2.HasValue)
						{
							size += 2;
							if (UserFlags3.HasValue)
								size += 2;
						}
					}
					return size;
				}
			}
		}

		[Serializable]
		public sealed class Strip : Poly
		{
			public bool Reversed { get; private set; }
			public ushort[] UserFlags1 { get; private set; }
			public ushort[] UserFlags2 { get; private set; }
			public ushort[] UserFlags3 { get; private set; }

			public Strip(int NumVerts, bool Reverse)
			{
				Indexes = new ushort[NumVerts];
				Reversed = Reverse;
			}

			public Strip(ushort[] Verts, bool Reverse)
			{
				Indexes = Verts;
				Reversed = Reverse;
			}

			public Strip(byte[] file, int address, byte userFlags)
			{
				Indexes = new ushort[ByteConverter.ToUInt16(file, address) & 0x7FFF];
				Reversed = (ByteConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
				if (userFlags > 0)
					UserFlags1 = new ushort[Indexes.Length - 2];
				if (userFlags > 1)
					UserFlags2 = new ushort[Indexes.Length - 2];
				if (userFlags > 2)
					UserFlags3 = new ushort[Indexes.Length - 2];
				address += 2;
				for (int i = 0; i < Indexes.Length; i++)
				{
					Indexes[i] = ByteConverter.ToUInt16(file, address);
					address += 2;
					if (i > 1)
					{
						if (userFlags > 0)
						{
							UserFlags1[i - 2] = ByteConverter.ToUInt16(file, address);
							address += 2;
							if (userFlags > 1)
							{
								UserFlags2[i - 2] = ByteConverter.ToUInt16(file, address);
								address += 2;
								if (userFlags > 2)
								{
									UserFlags3[i - 2] = ByteConverter.ToUInt16(file, address);
									address += 2;
								}
							}
						}
					}
				}
			}

			public void ToNJA(TextWriter writer)
			{
				if (UserFlags1 != null)
				{
					writer.Write(Environment.NewLine);
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write("\t" + Indexes[i].ToString() + ",");

						if (UserFlags1 != null && i > 1)
						{
							if (UserFlags3 != null)
							{
								writer.Write(" \tUf3( " + UserFlags1[i].ToString() + ", " + UserFlags2[i].ToString() + ", " + UserFlags3[i].ToString() + "),");
							}
							else if (UserFlags2 != null)
							{
								writer.Write(" \tUf2( " + UserFlags1[i].ToString() + ", " + UserFlags2[i].ToString() + "),");
							}
							else
							{
								writer.Write(" \tUf1( " + UserFlags1[i].ToString() + "),");
							}
						}

						writer.Write(Environment.NewLine);
					}
				}
				else
				{
					writer.Write("  ");
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write(Indexes[i].ToString() + ", ");
						if (((i + 1) % 10) == 0 && i != Indexes.Length - 1)
							writer.Write(Environment.NewLine + "                   ");
					}
					writer.Write(Environment.NewLine);
				}
			}

			public override int Size
			{
				get
				{
					int size = 2;
					size += Indexes.Length * 2;
					if (UserFlags1 != null)
					{
						size += UserFlags1.Length * 2;
						if (UserFlags2 != null)
						{
							size += UserFlags2.Length * 2;
							if (UserFlags3 != null)
								size += UserFlags3.Length * 2;
						}
					}
					return size;
				}
			}

			public override byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				int ind = Indexes.Length;
				if (Reversed)
					ind = -ind;
				result.AddRange(ByteConverter.GetBytes((short)(ind)));
				for (int i = 0; i < Indexes.Length; i++)
				{
					result.AddRange(ByteConverter.GetBytes(Indexes[i]));
					if (i > 1)
					{
						if (UserFlags1 != null)
						{
							result.AddRange(ByteConverter.GetBytes(UserFlags1[i - 2]));
							if (UserFlags2 != null)
							{
								result.AddRange(ByteConverter.GetBytes(UserFlags2[i - 2]));
								if (UserFlags3 != null)
									result.AddRange(ByteConverter.GetBytes(UserFlags3[i - 2]));
							}
						}
					}
				}
				return result.ToArray();
			}

			public override Poly Clone()
			{
				Strip result = (Strip)base.Clone();
				if (result.UserFlags1 != null)
					result.UserFlags1 = (ushort[])UserFlags1.Clone();
				if (result.UserFlags2 != null)
					result.UserFlags2 = (ushort[])UserFlags2.Clone();
				if (result.UserFlags3 != null)
					result.UserFlags3 = (ushort[])UserFlags3.Clone();
				return result;
			}
		}

		[Serializable]
		public abstract class Poly : ICloneable
		{
			public ushort[] Indexes { get; protected set; }

			internal Poly()
			{
			}

			public static Poly CreatePoly(ChunkType type)
			{
				switch (type)
				{
					case ChunkType.Volume_Polygon3:
						return new Triangle();
					case ChunkType.Volume_Polygon4:
						return new Quad();
					case ChunkType.Volume_Strip:
						throw new ArgumentException(
							"Cannot create strip-type poly without additional information.\nUse Strip.Strip(int NumVerts, bool Reverse) instead.",
							"type");
				}
				throw new ArgumentException("Unknown poly type!", "type");
			}

			public static Poly CreatePoly(ChunkType type, byte[] file, int address, byte userFlags)
			{
				switch (type)
				{
					case ChunkType.Volume_Polygon3:
						return new Triangle(file, address, userFlags);
					case ChunkType.Volume_Polygon4:
						return new Quad(file, address, userFlags);
					case ChunkType.Volume_Strip:
						return new Strip(file, address, userFlags);
				}
				throw new ArgumentException("Unknown poly type!", "type");
			}

			public abstract int Size { get; }

			public abstract byte[] GetBytes();

			object ICloneable.Clone() => Clone();

			public virtual Poly Clone()
			{
				Poly result = (Poly)MemberwiseClone();
				Indexes = (ushort[])Indexes.Clone();
				return result;
			}
		}

		public ushort Header2 { get; private set; }

		public byte UserFlags
		{
			get { return (byte)(Header2 >> 14); }
			private set { Header2 = (ushort)((Header2 & 0x3FFF) | ((value & 3) << 14)); }
		}

		public ushort PolyCount
		{
			get { return (ushort)Polys.Count; }
			private set { Header2 = (ushort)((Header2 & 0xC000) | (value & 0x3FFF)); }
		}

		public List<Poly> Polys { get; private set; }

		public PolyChunkVolume(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Header2 = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			int polyCount = Header2 & 0x3FFF;
			Polys = new List<Poly>(polyCount);
			for (int i = 0; i < polyCount; i++)
			{
				Poly str = Poly.CreatePoly(Type, file, address, UserFlags);
				Polys.Add(str);
				address += str.Size;
			}
		}

		public override byte[] GetBytes()
		{
			PolyCount = (ushort)Polys.Count;
			Size = 1;
			foreach (Poly str in Polys)
				Size += (ushort)(str.Size / 2);
			List<byte> result = new List<byte>(base.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Header2));
			foreach (Poly str in Polys)
				result.AddRange(str.GetBytes());
			return result.ToArray();
		}

		public override PolyChunk Clone()
		{
			PolyChunkVolume result = (PolyChunkVolume)base.Clone();
			result.Polys = new List<Poly>(Polys.Count);
			foreach (Poly item in Polys)
				result.Polys.Add(item.Clone());
			return result;
		}

		public override void ToNJA(TextWriter writer)
		{
			PolyCount = (ushort)Polys.Count;
			Size = 1;
			foreach (Poly ply in Polys)
				Size += (ushort)(ply.Size / 2);
			int alignmentPadding = Size % 2;
			Size += (ushort)alignmentPadding;
			switch (Type)
			{
				case ChunkType.Volume_Polygon3:
					writer.WriteLine("\tCnkO_P3, " + Size.ToString() + ", _NB( UFO_" + UserFlags.ToString() + ", " + Polys.Count + " ),");
					foreach (Triangle item in Polys)
					{
						item.ToNJA(writer);
					}
					break;
				case ChunkType.Volume_Polygon4:
					writer.WriteLine("\tCnkO_P4, " + Size.ToString() + ", _NB( UFO_" + UserFlags.ToString() + ", " + Polys.Count + " ),");
					foreach (Quad item in Polys)
					{
						item.ToNJA(writer);
					}
					break;
				case ChunkType.Volume_Strip:
					writer.WriteLine("\tCnkO_ST, " + Size.ToString() + ", _NB( UFO_" + UserFlags.ToString() + ", " + Polys.Count + " ),");
					foreach (Strip item in Polys)
					{
						item.ToNJA(writer);
					}
					break;
			}
			if (alignmentPadding > 0)
				writer.WriteLine("\tCnkNull(),");
		}
	}

	[Serializable]
	public class PolyChunkStrip : PolyChunkSize
	{
		[Serializable]
		public class Strip : ICloneable
		{
			public bool Reversed { get; private set; }
			public bool CMDEReversed
			{
				get { return Reversed; }
				set
				{
					bool oldsettype = Reversed;
					Reversed = value;
				}
			}
			public ushort[] Indexes { get; private set; }
			public UV[] UVs { get; private set; }
			public UV[] UVs2 { get; private set; }
			public Color[] VColors { get; private set; }
			public ushort[] UserFlags1 { get; private set; }
			public ushort[] UserFlags2 { get; private set; }
			public ushort[] UserFlags3 { get; private set; }

			public Strip(bool reversed, ushort[] indexes, UV[] uvs, Color[] vcolors)
			{
				Reversed = reversed;
				Indexes = indexes;
				UVs = uvs;
				VColors = vcolors;
			}

			public Strip(bool reversed, ushort[] indexes, UV[] uvs, UV[] uvs2, Color[] vcolors)
			{
				Reversed = reversed;
				Indexes = indexes;
				UVs = uvs;
				UVs2 = uvs2;
				VColors = vcolors;
			}

			public Strip(byte[] file, int address, ChunkType type, byte userFlags)
			{
				Indexes = new ushort[Math.Abs(ByteConverter.ToInt16(file, address))];
				Reversed = (ByteConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
				address += 2;
				switch (type)
				{
					case ChunkType.Strip_StripUVN:
					case ChunkType.Strip_StripUVH:
						UVs = new UV[Indexes.Length];
						break;
					case ChunkType.Strip_StripUVN2:
					case ChunkType.Strip_StripUVH2:
						UVs = new UV[Indexes.Length];
						UVs2 = new UV[Indexes.Length];
						break;
					case ChunkType.Strip_StripColor:
						VColors = new Color[Indexes.Length];
						break;
					case ChunkType.Strip_StripUVNColor:
					case ChunkType.Strip_StripUVHColor:
						UVs = new UV[Indexes.Length];
						VColors = new Color[Indexes.Length];
						break;
				}
				if (userFlags > 0)
					UserFlags1 = new ushort[Indexes.Length - 2];
				if (userFlags > 1)
					UserFlags2 = new ushort[Indexes.Length - 2];
				if (userFlags > 2)
					UserFlags3 = new ushort[Indexes.Length - 2];
				for (int i = 0; i < Indexes.Length; i++)
				{
					Indexes[i] = ByteConverter.ToUInt16(file, address);
					address += 2;
					switch (type)
					{
						case ChunkType.Strip_StripUVN:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVN2:
							UVs[i] = new UV(file, address, false, true);
							address += UV.Size;
							break;
						case ChunkType.Strip_StripUVH:
						case ChunkType.Strip_StripUVHColor:
						case ChunkType.Strip_StripUVH2:
							UVs[i] = new UV(file, address, true, true);
							address += UV.Size;
							break;
					}
					switch (type)
					{
						case ChunkType.Strip_StripColor:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVHColor:
							VColors[i] = VColor.FromBytes(file, address, ColorType.ARGB8888_16);
							address += VColor.Size(ColorType.ARGB8888_16);
							break;
					}
					switch (type)
					{
						case ChunkType.Strip_StripUVN2:
							UVs2[i] = new UV(file, address, false, true);
							address += UV.Size;
							break;
						case ChunkType.Strip_StripUVH2:
							UVs2[i] = new UV(file, address, true, true);
							address += UV.Size;
							break;
					}
					if (i > 1)
					{
						if (userFlags > 0)
						{
							UserFlags1[i - 2] = ByteConverter.ToUInt16(file, address);
							address += 2;
							if (userFlags > 1)
							{
								UserFlags2[i - 2] = ByteConverter.ToUInt16(file, address);
								address += 2;
								if (userFlags > 2)
								{
									UserFlags3[i - 2] = ByteConverter.ToUInt16(file, address);
									address += 2;
								}
							}
						}
					}
				}
			}

			public byte[] GetBytes(ChunkType type)
			{
				List<byte> result = new List<byte>();
				int ind = Indexes.Length;
				if (Reversed)
					ind = -ind;
				result.AddRange(ByteConverter.GetBytes((short)(ind)));
				for (int i = 0; i < Indexes.Length; i++)
				{
					result.AddRange(ByteConverter.GetBytes(Indexes[i]));
					switch (type)
					{
						case ChunkType.Strip_StripUVN:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVN2:
							result.AddRange(UVs[i].GetBytes(false, true));
							break;
						case ChunkType.Strip_StripUVH:
						case ChunkType.Strip_StripUVHColor:
						case ChunkType.Strip_StripUVH2:
							result.AddRange(UVs[i].GetBytes(true, true));
							break;
					}
					switch (type)
					{
						case ChunkType.Strip_StripColor:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVHColor:
							result.AddRange(VColor.GetBytes(VColors[i], ColorType.ARGB8888_16));
							break;
					}
					switch (type)
					{
						case ChunkType.Strip_StripUVN2:
							result.AddRange(UVs2[i].GetBytes(false, true));
							break;
						case ChunkType.Strip_StripUVH2:
							result.AddRange(UVs2[i].GetBytes(true, true));
							break;
					}
					if (i > 1)
					{
						if (UserFlags1 != null)
						{
							result.AddRange(ByteConverter.GetBytes(UserFlags1[i - 2]));
							if (UserFlags2 != null)
							{
								result.AddRange(ByteConverter.GetBytes(UserFlags2[i - 2]));
								if (UserFlags3 != null)
									result.AddRange(ByteConverter.GetBytes(UserFlags3[i - 2]));
							}
						}
					}
				}
				return result.ToArray();
			}

			public void ToNJA(TextWriter writer, bool UVH)
			{
				if (Reversed)
					writer.Write("\tStripR(" + Indexes.Length + "),");
				else
					writer.Write("\tStripL(" + Indexes.Length + "),");

				if (UVs != null || VColors != null || UserFlags1 != null)
				{
					writer.Write(Environment.NewLine);
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write("\t" + Indexes[i].ToString() + ",");

						if (UVs != null)
							writer.Write(" \tUvn( " + ((short)(UVs[i].U * (UVH ? 1024.0 : 256.0))).ToString() + ", " + ((short)(UVs[i].V * (UVH ? 1024.0 : 256.0))).ToString() + " ),");

						if (VColors != null)
							writer.Write(" \tD8888(" + VColors[i].A.ToString() + ", " + VColors[i].R.ToString() + ", " + VColors[i].G.ToString() + ", " + VColors[i].B.ToString() + "),");

						if (UVs2 != null)
							writer.Write(" \tUvn( " + ((short)(UVs2[i].U * (UVH ? 1024.0 : 256.0))).ToString() + ", " + ((short)(UVs2[i].V * (UVH ? 1024.0 : 256.0))).ToString() + " ),");

						if (UserFlags1 != null && i > 1)
						{
							if (UserFlags3 != null)
							{
								writer.Write(" \tUf3( " + UserFlags1[i].ToString() + ", " + UserFlags2[i].ToString() + ", " + UserFlags3[i].ToString() + "),");
							}
							else if (UserFlags2 != null)
							{
								writer.Write(" \tUf2( " + UserFlags1[i].ToString() + ", " + UserFlags2[i].ToString() + "),");
							}
							else
							{
								writer.Write(" \tUf1( " + UserFlags1[i].ToString() + "),");
							}
						}

						writer.Write(Environment.NewLine);
					}
				}
				else
				{
					writer.Write("  ");
					for (int i = 0; i < Indexes.Length; ++i)
					{
						writer.Write(Indexes[i].ToString() + ", ");
						if (((i + 1) % 10) == 0 && i != Indexes.Length - 1)
							writer.Write(Environment.NewLine + "                   ");
					}
					writer.Write(Environment.NewLine);
				}
			}

			public int Size
			{
				get
				{
					int size = 2;
					size += Indexes.Length * 2;
					if (UVs != null)
						size += UVs.Length * UV.Size;
					if (VColors != null)
						size += VColors.Length * VColor.Size(ColorType.ARGB8888_16);
					if (UVs2 != null)
						size += UVs2.Length * UV.Size;
					if (UserFlags1 != null)
					{
						size += UserFlags1.Length * 2;
						if (UserFlags2 != null)
						{
							size += UserFlags2.Length * 2;
							if (UserFlags3 != null)
								size += UserFlags3.Length * 2;
						}
					}
					return size;
				}
			}

			object ICloneable.Clone() => Clone();

			public Strip Clone()
			{
				Strip result = (Strip)MemberwiseClone();
				result.Indexes = (ushort[])Indexes.Clone();
				if (UVs != null)
				{
					result.UVs = new UV[UVs.Length];
					for (int i = 0; i < UVs.Length; i++)
						result.UVs[i] = UVs[i].Clone();
				}
				if (VColors != null) result.VColors = (Color[])VColors.Clone();
				if (UVs2 != null)
				{
					result.UVs2 = new UV[UVs.Length];
					for (int i = 0; i < UVs2.Length; i++)
						result.UVs2[i] = UVs2[i].Clone();
				}
				if (UserFlags1 != null) result.UserFlags1 = (ushort[])UserFlags1.Clone();
				if (UserFlags2 != null) result.UserFlags2 = (ushort[])UserFlags2.Clone();
				if (UserFlags3 != null) result.UserFlags3 = (ushort[])UserFlags3.Clone();
				return result;
			}
		}

		public bool IgnoreLight
		{
			get { return (Flags & 1) == 1; }
			set { Flags = (byte)((Flags & ~1) | (value ? 1 : 0)); }
		}

		public bool IgnoreSpecular
		{
			get { return (Flags & 2) == 2; }
			set { Flags = (byte)((Flags & ~2) | (value ? 2 : 0)); }
		}

		public bool IgnoreAmbient
		{
			get { return (Flags & 4) == 4; }
			set { Flags = (byte)((Flags & ~4) | (value ? 4 : 0)); }
		}

		public bool UseAlpha
		{
			get { return (Flags & 8) == 8; }
			set { Flags = (byte)((Flags & ~8) | (value ? 8 : 0)); }
		}

		public bool DoubleSide
		{
			get { return (Flags & 0x10) == 0x10; }
			set { Flags = (byte)((Flags & ~0x10) | (value ? 0x10 : 0)); }
		}

		public bool FlatShading
		{
			get { return (Flags & 0x20) == 0x20; }
			set { Flags = (byte)((Flags & ~0x20) | (value ? 0x20 : 0)); }
		}

		public bool EnvironmentMapping
		{
			get { return (Flags & 0x40) == 0x40; }
			set { Flags = (byte)((Flags & ~0x40) | (value ? 0x40 : 0)); }
		}

		public bool NoAlphaTest
		{
			get { return (Flags & 0x80) == 0x80; }
			set { Flags = (byte)((Flags & ~0x80) | (value ? 0x80 : 0)); }
		}

		public ushort Header2 { get; private set; }

		public byte UserFlags
		{
			get { return (byte)(Header2 >> 14); }
			private set { Header2 = (ushort)((Header2 & 0x3FFF) | ((value & 3) << 14)); }
		}

		public ushort StripCount
		{
			get { return (ushort)Strips.Count; }
			private set { Header2 = (ushort)((Header2 & 0xC000) | (value & 0x3FFF)); }
		}

		public List<Strip> Strips { get; private set; }

		public PolyChunkStrip(ChunkType type)
		{
			Type = type;
			Strips = new List<Strip>();
		}

		public PolyChunkStrip(byte[] file, int address)
		{
			Header = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Size = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			Header2 = ByteConverter.ToUInt16(file, address);
			address += sizeof(ushort);
			int stripCount = Header2 & 0x3FFF;
			Strips = new List<Strip>(stripCount);
			for (int i = 0; i < stripCount; i++)
			{
				Strip str = new Strip(file, address, Type, UserFlags);
				Strips.Add(str);
				address += str.Size;
			}
		}

		public override byte[] GetBytes()
		{
			StripCount = (ushort)Strips.Count;
			Size = 1;
			foreach (Strip str in Strips)
				Size += (ushort)(str.Size / 2);

			int alignmentPadding = (Size % 2);
			Size += (ushort)alignmentPadding;

			List<byte> result = new List<byte>(base.GetBytes());
			result.AddRange(ByteConverter.GetBytes(Header2));
			foreach (Strip str in Strips)
				result.AddRange(str.GetBytes(Type));

			if (alignmentPadding > 0)
			{
				result.AddRange(new byte[2]);
			}

			return result.ToArray();
		}

		public override PolyChunk Clone()
		{
			PolyChunkStrip result = (PolyChunkStrip)base.Clone();
			result.Strips = new List<Strip>(StripCount);
			foreach (Strip item in Strips)
				result.Strips.Add(item.Clone());
			return result;
		}

		public override void ToNJA(TextWriter writer)
		{
			string flags = string.Empty;

			if (IgnoreLight)
				flags += "FST_IL|";
			if (IgnoreSpecular)
				flags += "FST_IS|";
			if (IgnoreAmbient)
				flags += "FST_IA|";
			if (UseAlpha)
				flags += "FST_UA|";
			if (DoubleSide)
				flags += "FST_DB|";
			if (FlatShading)
				flags += "FST_FL|";
			if (EnvironmentMapping)
				flags += "FST_ENV|";
			if (NoAlphaTest)
				flags += "FST_NAT|";
			if (flags == string.Empty)
				flags = "0x0";
			else
				flags = flags.Remove(flags.Length - 1);

			string chunkname = string.Empty;
			bool UVH = false;

			switch (Type)
			{
				case ChunkType.Strip_Strip:
					chunkname = "CnkS";
					break;
				case ChunkType.Strip_StripUVN:
					chunkname = "CnkS_UVN";
					break;
				case ChunkType.Strip_StripUVH:
					chunkname = "CnkS_UVH";
					UVH = true;
					break;
				case ChunkType.Strip_StripNormal:
					chunkname = "CnkS_VN";
					break;
				case ChunkType.Strip_StripUVNNormal:
					chunkname = "CnkS_UVN_VN";
					break;
				case ChunkType.Strip_StripUVHNormal:
					chunkname = "CnkS_UVH_VN";
					UVH = true;
					break;
				case ChunkType.Strip_StripColor:
					chunkname = "CnkS_D8";
					break;
				case ChunkType.Strip_StripUVNColor:
					chunkname = "CnkS_UVN_D8";
					break;
				case ChunkType.Strip_StripUVHColor:
					chunkname = "CnkS_UVH_D8";
					UVH = true;
					break;
				case ChunkType.Strip_Strip2:
					chunkname = "CnkS_2";
					break;
				case ChunkType.Strip_StripUVN2:
					chunkname = "CnkS_UVN2";
					break;
				case ChunkType.Strip_StripUVH2:
					chunkname = "CnkS_UVH2";
					UVH = true;
					break;
			}

			StripCount = (ushort)Strips.Count;
			Size = 1;
			foreach (Strip str in Strips)
				Size += (ushort)(str.Size / 2);

			int alignmentPadding = (Size % 2);
			Size += (ushort)alignmentPadding;

			writer.WriteLine("\t" + chunkname + "( " + flags + " ), " + Size.ToString() + ", _NB( UFO_" + UserFlags.ToString() + ", " + StripCount + " ),");

			foreach(Strip item in Strips)
			{
				item.ToNJA(writer, UVH);
			}

			if (alignmentPadding > 0)
				writer.WriteLine("\tCnkNull(),");
		}

		public void UpdateFlags(NJS_MATERIAL mat)
		{
			IgnoreLight = mat.IgnoreLighting;
			IgnoreSpecular = mat.IgnoreSpecular;
			IgnoreAmbient = mat.IgnoreAmbient;
			UseAlpha = mat.UseAlpha;
			DoubleSide = mat.DoubleSided;
			FlatShading = mat.FlatShading;
			EnvironmentMapping = mat.EnvironmentMap;
			NoAlphaTest = mat.NoAlphaTest;
		}
	}
}