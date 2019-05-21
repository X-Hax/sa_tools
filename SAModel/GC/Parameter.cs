using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public enum ParameterType
	{
		VtxAttrFmt = 0,
		IndexAttributeFlags = 1,
		Lighting = 2,
		BlendAlpha = 4,
		AmbientColor = 5,
		Texture = 8,
		MipMap = 10,
	}

	public abstract class Parameter
	{
		public ParameterType ParameterType { get; protected set; }

		public abstract void Read(byte[] file, int address);
	}

	public class VtxAttrFmtParameter : Parameter
	{
		public GXVertexAttribute VertexAttribute { get; private set; }
		public GXDataType DataType { get; private set; }
		public byte FractionBits { get; private set; }
		public VtxAttrFmtParameter()
		{
			ParameterType = ParameterType.VtxAttrFmt;
		}
		public override void Read(byte[] file, int address)
		{
			FractionBits = file[0];
			DataType = (GXDataType)();
			VertexAttribute = (GXVertexAttribute)file[2];
		}
	}
	public class IndexAttributeParameter : Parameter
	{
		[Flags]
		public enum IndexAttributeFlags : ushort
		{
			Bit0 = 1 << 0, // Unused
			Bit1 = 1 << 1, // Unused
			Position16BitIndex = 1 << 2,
			HasPosition = 1 << 3,
			Normal16BitIndex = 1 << 4,
			HasNormal = 1 << 5,
			Color16BitIndex = 1 << 6,
			HasColor = 1 << 7,
			Bit8 = 1 << 8, // Unused
			Bit9 = 1 << 9, // Unused
			UV16BitIndex = 1 << 10,
			HasUV = 1 << 11,
			Bit12 = 1 << 12, // Unused
			Bit13 = 1 << 13, // Unused
			Bit14 = 1 << 14, // Unused
			Bit15 = 1 << 15, // Unused
		}

		public IndexAttributeFlags IndexAttributes { get; private set; }

		public IndexAttributeParameter()
		{
			ParameterType = ParameterType.IndexAttributeFlags;
		}

		public override void Read(byte[] file, int address)
		{
			IndexAttributes = (IndexAttributeFlags)ByteConverter.ToInt32(file, address);
		}
	}

	public class AmbientColorParameter : Parameter
	{
		public System.Drawing.Color AmbientColor { get; private set; }
		public AmbientColorParameter()
		{
			ParameterType = ParameterType.AmbientColor;
		}
		public override void Read(byte[] file, int address)
		{
			AmbientColor = System.Drawing.Color.FromArgb(ByteConverter.ToInt32(file, address));
		}
	}

	public class TextureParameter : Parameter
	{
		[Flags]
		public enum TileMode
		{
			WrapU = 1 << 0,
			MirrorU = 1 << 1,
			WrapV = 1 << 2,
			MirrorV = 1 << 3,
		}
		public ushort TextureID { get; private set; }
		public TileMode Tile { get; private set; }
		public TextureParameter()
		{
			ParameterType = ParameterType.Texture;
			TextureID = 0;
			Tile = TileMode.WrapU | TileMode.WrapV;
		}
		public override void Read(byte[] file, int address)
		{
			TextureID = ByteConverter.ToUInt16(file, address);
			Tile = (TileMode)ByteConverter.ToUInt16(file, address + 2);
		}
	}
}
