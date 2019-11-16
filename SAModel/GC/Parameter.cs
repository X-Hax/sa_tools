using System;
using System.IO;
using SonicRetro.SAModel;
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
		Unknown_9 = 9,
		TexCoordGen = 10,
	}

	public abstract class Parameter
	{
		public ParameterType ParameterType { get; protected set; }

		public abstract void Read(byte[] file, int address);
		public abstract void Write(BinaryWriter writer);
	}

	public class VtxAttrFmtParameter : Parameter
	{
		public GXVertexAttribute VertexAttribute { get; private set; }
		public ushort Unknown_1 { get; private set; }
		public VtxAttrFmtParameter()
		{
			ParameterType = ParameterType.VtxAttrFmt;
		}
		public VtxAttrFmtParameter(ushort Unknown, GXVertexAttribute vertexAttrib)
		{
			ParameterType = ParameterType.VtxAttrFmt;
			Unknown_1 = Unknown;
			VertexAttribute = vertexAttrib;
		}
		public override void Read(byte[] file, int address)
		{
			Unknown_1 = ByteConverter.ToUInt16(file, address);
			VertexAttribute = (GXVertexAttribute)(ByteConverter.ToUInt16(file, address + 2) + 8);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write((ushort)Unknown_1);
			writer.Write((ushort)(VertexAttribute - 8));
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
		public IndexAttributeParameter(IndexAttributeFlags flags)
		{
			ParameterType = ParameterType.IndexAttributeFlags;
			IndexAttributes = flags;
		}

		public override void Read(byte[] file, int address)
		{
			IndexAttributes = (IndexAttributeFlags)ByteConverter.ToInt32(file, address);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write((int)IndexAttributes);
		}
	}

	public class LightingParameter : Parameter
	{
		public ushort Unknown1 { get; private set; }
		public ushort Unknown2 { get; private set; }

		public LightingParameter()
		{
			ParameterType = ParameterType.Lighting;
		}

		public LightingParameter(ushort Unk1, ushort Unk2)
		{
			ParameterType = ParameterType.Lighting;
			Unknown1 = Unk1;
			Unknown2 = Unk2;
		}

		public override void Read(byte[] file, int address)
		{
			Unknown1 = ByteConverter.ToUInt16(file, address);
			Unknown2 = ByteConverter.ToUInt16(file, address + 2);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write(Unknown1);
			writer.Write(Unknown2);
		}
	}

	public class BlendAlphaParameter : Parameter
	{
		private int m_Data;

		public AlphaInstruction SourceAlpha
		{
			get
			{
				return GXToNJAlphaInstruction((GXBlendModeControl)((m_Data >> 11) & 7));
			}
			set
			{
				int inst = (int)NJtoGXBlendModeControl(value);
				m_Data &= ~(7 << 11);
				m_Data |= (inst & 7) << 11;
			}
		}

		public AlphaInstruction DestinationAlpha
		{
			get
			{
				return GXToNJAlphaInstruction((GXBlendModeControl)((m_Data >> 8) & 7));
			}
			set
			{
				int inst = (int)NJtoGXBlendModeControl(value);
				m_Data &= ~(7 << 8);
				m_Data |= (inst & 7) << 8;
			}
		}

		public BlendAlphaParameter()
		{
			ParameterType = ParameterType.BlendAlpha;
			m_Data = 0;
		}

		public override void Read(byte[] file, int address)
		{
			m_Data = ByteConverter.ToInt32(file, address);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write(m_Data);
		}

		public static AlphaInstruction GXToNJAlphaInstruction(GXBlendModeControl gx)
		{
			switch (gx)
			{
				case GXBlendModeControl.SrcAlpha:
					return AlphaInstruction.SourceAlpha;
				case GXBlendModeControl.DstAlpha:
					return AlphaInstruction.DestinationAlpha;
				case GXBlendModeControl.InverseSrcAlpha:
					return AlphaInstruction.InverseSourceAlpha;
				case GXBlendModeControl.InverseDstAlpha:
					return AlphaInstruction.InverseDestinationAlpha;
				case GXBlendModeControl.SrcColor:
					return AlphaInstruction.OtherColor;
				case GXBlendModeControl.InverseSrcColor:
					return AlphaInstruction.InverseOtherColor;
				case GXBlendModeControl.One:
					return AlphaInstruction.One;
				case GXBlendModeControl.Zero:
					return AlphaInstruction.Zero;
			}

			return AlphaInstruction.Zero;
		}

		public static GXBlendModeControl NJtoGXBlendModeControl(AlphaInstruction nj)
		{
			switch (nj)
			{
				case AlphaInstruction.SourceAlpha:
					return GXBlendModeControl.SrcAlpha;
				case AlphaInstruction.DestinationAlpha:
					return GXBlendModeControl.DstAlpha;
				case AlphaInstruction.InverseSourceAlpha:
					return GXBlendModeControl.InverseSrcAlpha;
				case AlphaInstruction.InverseDestinationAlpha:
					return GXBlendModeControl.InverseDstAlpha;
				case AlphaInstruction.OtherColor:
					return GXBlendModeControl.SrcColor;
				case AlphaInstruction.InverseOtherColor:
					return GXBlendModeControl.InverseSrcColor;
				case AlphaInstruction.One:
					return GXBlendModeControl.One;
				case AlphaInstruction.Zero:
					return GXBlendModeControl.Zero;
			}

			return GXBlendModeControl.Zero;
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

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write(AmbientColor.B);
			writer.Write(AmbientColor.G);
			writer.Write(AmbientColor.R);
			writer.Write(AmbientColor.A);
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
			Unk_1 = 1 << 4,
			Mask = (1 << 5) - 1
		}
		public ushort TextureID { get; private set; }
		public TileMode Tile { get; private set; }
		public TextureParameter()
		{
			ParameterType = ParameterType.Texture;
			TextureID = 0;
			Tile = TileMode.WrapU | TileMode.WrapV;
		}
		public TextureParameter(ushort TexID, TileMode tileMode)
		{
			ParameterType = ParameterType.Texture;
			TextureID = TexID;
			Tile = tileMode;
		}

		public override void Read(byte[] file, int address)
		{
			TextureID = ByteConverter.ToUInt16(file, address);
			Tile = (TileMode)ByteConverter.ToUInt16(file, address + 2);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write((ushort)TextureID);
			writer.Write((ushort)Tile);
		}
	}

	public class Unknown9Parameter : Parameter
	{
		public ushort Unknown1 { get; private set; }
		public ushort Unknown2 { get; private set; }

		public Unknown9Parameter()
		{
			ParameterType = ParameterType.Unknown_9;
		}

		public override void Read(byte[] file, int address)
		{
			Unknown1 = ByteConverter.ToUInt16(file, address);
			Unknown2 = ByteConverter.ToUInt16(file, address + 2);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write(Unknown1);
			writer.Write(Unknown2);
		}
	}

	public class TexCoordGenParameter : Parameter
	{
		public ushort Unknown1 { get; private set; }
		public ushort Unknown2 { get; private set; }
		public GXTexCoordID TexCoordID{ get; private set; }
		public GXTexGenType TexGenType { get; private set; }
		public GXTexGenSrc TexGenSrc { get; private set; }
		public int MatrixIndex { get; private set; }
		public TexCoordGenParameter()
		{
			ParameterType = ParameterType.TexCoordGen;
		}

		public override void Read(byte[] file, int address)
		{
			TexCoordID = (GXTexCoordID)file[address + 2];
			TexGenType = (GXTexGenType)(ByteConverter.ToUInt16(file, address) >> 12);
			TexGenSrc = (GXTexGenSrc)((ByteConverter.ToUInt32(file, address) >> 4) & 0xFF);
			MatrixIndex = 0x1E + (file[address] & 0xF) * 3;
			//im just gonna keep this for now
			Unknown1 = ByteConverter.ToUInt16(file, address);
			Unknown2 = ByteConverter.ToUInt16(file, address + 2);
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write((int)ParameterType);
			writer.Write(Unknown1);
			writer.Write(Unknown2);
		}
	}
}
