using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SAModel.GC
{
	/// <summary>
	/// The types of parameter that exist
	/// </summary>
	public enum ParameterType : uint
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
	
	/// <summary>
	/// Base class for all GC parameter types. <br/>
	/// Used to store geometry information (like materials).
	/// </summary>
	[Serializable]
	public abstract class GCParameter
	{
		/// <summary>
		/// The type of parameter
		/// </summary>
		public readonly ParameterType Type;

		/// <summary>
		/// All parameter data is stored in these 4 bytes
		/// </summary>
		protected uint Data;

		/// <summary>
		/// Base constructor for an empty parameter. <br/>
		/// Used only in child classes.
		/// </summary>
		/// <param name="type">The type of parameter to create</param>
		protected GCParameter(ParameterType type)
		{
			Type = type;
			Data = 0;
		}

		/// <summary>
		/// Create a parameter object from a file and address
		/// </summary>
		/// <param name="file">The file contents</param>
		/// <param name="address">The address at which the parameter is located</param>
		/// <returns>Any of the parameter types</returns>
		public static GCParameter Read(byte[] file, int address)
		{
			var paramType = (ParameterType)file[address];

			GCParameter result = paramType switch
			{
				ParameterType.VtxAttrFmt => new VtxAttrFmtParameter(GCVertexAttribute.Null),
				ParameterType.IndexAttributeFlags => new IndexAttributeParameter(),
				ParameterType.Lighting => new LightingParameter(),
				ParameterType.BlendAlpha => new BlendAlphaParameter(),
				ParameterType.AmbientColor => new AmbientColorParameter(),
				ParameterType.Texture => new TextureParameter(),
				ParameterType.Unknown_9 => new Unknown9Parameter(),
				ParameterType.TexCoordGen => new TexCoordGenParameter(),
				_ => null
			};

			result.Data = ByteConverter.ToUInt32(file, address + 4);

			return result;
		}

		/// <summary>
		/// Writes the parameter contents to a stream
		/// </summary>
		/// <param name="writer">The stream writer</param>
		public void Write(BinaryWriter writer)
		{
			writer.Write((byte)Type);
			writer.Write(Data);
		}

		public byte[] GetBytes()
		{
			List<byte> result = [];
			
			result.Add((byte)Type);
			result.AddRange(new byte[3]);
			result.AddRange(ByteConverter.GetBytes(Data));
			
			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");
			
			result.Append((byte)Type);
			result.Append(", ");
			result.Append("0, 0, 0");
			result.Append(", ");
			result.AppendFormat(Data.ToCHex());
			result.Append(" }");
			
			return result.ToString();
		}

		public void ToNJA(TextWriter writer)
		{
			switch (Type)
			{
				case ParameterType.VtxAttrFmt:
					if (Data >> 16 != 0x5)
					{
						writer.WriteLine($"\tGJD_PARAM_IDX      ( {(GCVertexAttribute)(Data >> 16)}, {(byte)(Data >> 8)}, {(byte)Data} ),");
					}
					else
					{
						writer.WriteLine($"\tGJD_PARAM_IDX      ( {(GCVertexAttribute)(Data >> 16)}, {(byte)(Data >> 8)}, {(GCUVScale)(byte)Data} ),");
					}
					break;
				case ParameterType.IndexAttributeFlags:
					writer.WriteLine($"\tGJD_PARAM_VFLAGS   ( {((GCIndexAttributeFlags)Data).ToString().Replace(", ", " | ")} ),");
					break;
				case ParameterType.Lighting:
					writer.WriteLine($"\tGJD_PARAM_LIGHT    ( {(short)Data}, {(byte)((Data >> 16) & 0xF)}, {(byte)((Data >> 20) & 0xF)}, {(byte)((Data >> 24) & 0xFF)} ),");
					break;
				case ParameterType.BlendAlpha: 
					writer.WriteLine($"\tGJD_PARAM_BLEND    ( {(GCBlendModeControl)((Data >> 11) & 7)}, {(GCBlendModeControl)((Data >> 8) & 7)} ),");
					break;
				case ParameterType.AmbientColor:
					writer.WriteLine($"\tGJD_PARAM_ACOLOR   ( {(byte)Data}, {(byte)(Data >> 8)}, {(byte)(Data >> 16)}, {(byte)(Data >> 24)} ),");
					break;
				case ParameterType.Texture:
					writer.WriteLine($"\tGJD_PARAM_TEX      ( {(short)Data}, {((GCTileMode)(short)(Data >> 16)).ToString().Replace(", ", " | ")} ),");
					break;
				case ParameterType.Unknown_9: writer.WriteLine($"\tGJD_PARAM_UNK      ( {(short)Data}, {(short)(Data >> 16)} ),");
					break;
				case ParameterType.TexCoordGen:
					writer.WriteLine($"\tGJD_PARAM_TEXCOORD ( {(GCTexCoordID)((Data >> 16) & 0xFF)}, {(GCTexGenType)((Data >> 12) & 0xF)}, {(GCTexGenSrc)((Data >> 4) & 0xFF)}, {(GCTexGenMatrix)(Data & 0xF)} ),");
					break;
			}
		}
	}

	/// <summary>
	/// Parameter that is relevant for Vertex data. <br/>
	/// A geometry object needs to have one for each 
	/// </summary>
	[Serializable]
	public class VtxAttrFmtParameter : GCParameter
	{
		/// <summary>
		/// The attribute type that this parameter applies for
		/// </summary>
		public GCVertexAttribute VertexAttribute
		{
			get => (GCVertexAttribute)(Data >> 16);
			set
			{
				Data &= 0xFFFF;
				Data |= (uint)value << 16;
			}
		}

		/// <summary>
		/// Seems to be some type of address of buffer length. <br/>
		/// Sa2 only uses a specific value for each attribute type either way
		/// </summary>
		public ushort Unknown
		{
			get => (ushort)(Data & 0xFFFF);
			set
			{
				Data &= 0xFFFF0000;
				Data |= value;
			}
		}

		/// <summary>
		/// Creates a new parameter with the default value according to each attribute type <br/> (which are the only ones that work ingame)
		/// </summary>
		/// <param name="vertexAttrib">The vertex attribute type that the parameter is for</param>
		public VtxAttrFmtParameter(GCVertexAttribute vertexAttrib) : base(ParameterType.VtxAttrFmt)
		{
			VertexAttribute = vertexAttrib;

			// Setting the default values
			switch (vertexAttrib)
			{
				case GCVertexAttribute.Position:
					Unknown = 5120;
					break;
				case GCVertexAttribute.Normal:
					Unknown = 9216;
					break;
				case GCVertexAttribute.Color0:
					Unknown = 27136;
					break;
				case GCVertexAttribute.Tex0:
					Unknown = 33544;
					break;
			}
		}

		/// <summary>
		/// Allows to manually create a Vertex attribute parameter
		/// </summary>
		/// <param name="unknown"></param>
		/// <param name="vertexAttrib">The vertex attribute type that the parameter is for</param>
		public VtxAttrFmtParameter(ushort unknown, GCVertexAttribute vertexAttrib) : base(ParameterType.VtxAttrFmt)
		{
			Unknown = unknown;
			VertexAttribute = vertexAttrib;
		}
	}

	/// <summary>
	/// Holds information about the vertex data that's stored in the geometry
	/// </summary>
	[Serializable]
	public class IndexAttributeParameter : GCParameter
	{
		/// <summary>
		/// Holds information about the vertex data that's stored in the geometry 
		/// </summary>
		public GCIndexAttributeFlags IndexAttributes
		{
			get => (GCIndexAttributeFlags)Data;
			set => Data = (uint)value;
		}

		/// <summary>
		/// Creates an empty index attribute parameter
		/// </summary>
		public IndexAttributeParameter() : base(ParameterType.IndexAttributeFlags)
		{
			// This always exists
			IndexAttributes &= GCIndexAttributeFlags.HasPosition;
		}

		/// <summary>
		/// Creates an index attribute parameter based on existing flags
		/// </summary>
		/// <param name="flags"></param>
		public IndexAttributeParameter(GCIndexAttributeFlags flags) : base(ParameterType.IndexAttributeFlags)
		{
			IndexAttributes = flags;
		}

	}

	/// <summary>
	/// Holds lighting information
	/// </summary>
	[Serializable]
	public class LightingParameter : GCParameter
	{
		/// <summary>
		/// Lighting flags. Pretty much unknown how they work
		/// </summary>
		public ushort LightingFlags
		{
			get => (ushort)(Data & 0xFFFF);
			set
			{
				Data &= 0xFFFF0000;
				Data |= value;
			}
		}

		/// <summary>
		/// Which shadow stencil the geometry should use. <br/>
		/// Ranges from 0 to 15
		/// </summary>
		public byte ShadowStencil
		{
			get => (byte)((Data >> 16) & 0xF);
			set
			{
				Data &= 0xFFF0FFFF;
				Data |= (uint)((value & 0xF) << 16);
			}
		}

		public byte Unknown1
		{
			get => (byte)((Data >> 20) & 0xF);
			set
			{
				Data &= 0xFFF0FFFF;
				Data |= (uint)((value & 0xF) << 20);
			}
		}

		public byte Unknown2
		{
			get => (byte)((Data >> 24) & 0xFF);
			set
			{
				Data &= 0xFFF0FFFF;
				Data |= (uint)(value << 24);
			}
		} 

		/// <summary>
		/// Creates a lighting parameter with the default data
		/// </summary>
		public LightingParameter() : base(ParameterType.Lighting)
		{
			// Default value
			LightingFlags = 0xB11;
			ShadowStencil = 1;
		}

		public LightingParameter(ushort lightingFlags, byte shadowStencil) : base(ParameterType.Lighting)
		{
			LightingFlags = lightingFlags;
			ShadowStencil = shadowStencil;
		}
	}

	/// <summary>
	/// The blending information for the surface of the geometry
	/// </summary>
	[Serializable]
	public class BlendAlphaParameter() : GCParameter(ParameterType.BlendAlpha)
	{
		/// <summary>
		/// NJ Blend mode for the source alpha
		/// </summary>
		public AlphaInstruction NJSourceAlpha
		{
			get => GCEnumConverter.GXToNJAlphaInstruction((GCBlendModeControl)((Data >> 11) & 7));
			set
			{
				var inst = (uint)GCEnumConverter.NJtoGXBlendModeControl(value);
				Data &= 0xFFFFC7FF; // ~(7 << 11)
				Data |= (inst & 7) << 11;
			}
		}

		/// <summary>
		/// NJ Blend mode for the destination alpha
		/// </summary>
		public AlphaInstruction NJDestAlpha
		{
			get => GCEnumConverter.GXToNJAlphaInstruction((GCBlendModeControl)((Data >> 8) & 7));
			set
			{
				var inst = (uint)GCEnumConverter.NJtoGXBlendModeControl(value);
				Data &= 0xFFFFF8FF; // ~(7 << 8)
				Data |= (inst & 7) << 8;
			}
		}

		/// <summary>
		/// Blend mode for the source alpha
		/// </summary>
		public GCBlendModeControl SourceAlpha
		{
			get => (GCBlendModeControl)((Data >> 11) & 7);
			set
			{
				var inst = (uint)value;
				Data &= 0xFFFFC7FF; // ~(7 << 11)
				Data |= (inst & 7) << 11;
			}
		}

		/// <summary>
		/// Blend mode for the destination alpha
		/// </summary>
		public GCBlendModeControl DestAlpha
		{
			get => (GCBlendModeControl)((Data >> 8) & 7);
			set
			{
				var inst = (uint)value;
				Data &= 0xFFFFF8FF; // ~(7 << 8)
				Data |= (inst & 7) << 8;
			}
		}
	}

	/// <summary>
	/// Ambient color of the geometry
	/// </summary>
	[Serializable]
	public class AmbientColorParameter : GCParameter
	{
		/// <summary>
		/// The Color of the geometry
		/// </summary>
		public Color AmbientColor
		{
			get
			{
				var col = new Color
				{
					ARGB = Data
				};
				
				return col;
			}
			set => Data = value.ARGB;
		}

		public AmbientColorParameter() : base(ParameterType.AmbientColor)
		{
			Data = uint.MaxValue; // White is default
		}
	}

	/// <summary>
	/// Texture information for the geometry
	/// </summary>
	[Serializable]
	public class TextureParameter : GCParameter
	{
		/// <summary>
		/// The id of the texture
		/// </summary>
		public ushort TextureId
		{
			get => (ushort)(Data & 0xFFFF);
			set
			{
				Data &= 0xFFFF0000;
				Data |= value;
			}
		}

		/// <summary>
		/// Texture Tiling properties
		/// </summary>
		public GCTileMode Tile
		{
			get => (GCTileMode)(Data >> 16);
			set
			{
				Data &= 0xFFFF;
				Data |= (uint)value << 16;
			}
		}

		public TextureParameter() : base(ParameterType.Texture)
		{
			TextureId = 0;
			Tile = GCTileMode.WrapU | GCTileMode.WrapV;
		}

		public TextureParameter(ushort textureId, GCTileMode tileMode) : base(ParameterType.Texture)
		{
			TextureId = textureId;
			Tile = tileMode;
		}
	}

	/// <summary>
	/// No idea what this is for, but its needed
	/// </summary>
	[Serializable]
	public class Unknown9Parameter : GCParameter
	{
		/// <summary>
		/// No idea what this does. Default is 4
		/// </summary>
		public ushort Unknown1
		{
			get => (ushort)(Data & 0xFFFF);
			set
			{
				Data &= 0xFFFF0000;
				Data |= value;
			}
		}

		/// <summary>
		/// No idea what this does. Default is 0
		/// </summary>
		public ushort Unknown2
		{
			get => (ushort)(Data >> 16);
			set
			{
				Data &= 0xFFFF;
				Data |= (uint)value << 16;
			}
		}

		public Unknown9Parameter() : base(ParameterType.Unknown_9)
		{
			// Default values
			Unknown1 = 4;
			Unknown2 = 0;
		}
	}

	/// <summary>
	/// Determines where or how the geometry gets the texture coordinates
	/// </summary>
	[Serializable]
	public class TexCoordGenParameter : GCParameter
	{
		/// <summary>
		/// The output location of the generated texture coordinates
		/// </summary>
		public GCTexCoordID TexCoordId
		{
			get => (GCTexCoordID)((Data >> 16) & 0xFF);
			set
			{
				Data &= 0xFF00FFFF;
				Data |= (uint)value << 16;
			}
		}

		/// <summary>
		/// The function to use for generating the texture coordinates
		/// </summary>
		public GCTexGenType TexGenType
		{
			get => (GCTexGenType)((Data >> 12) & 0xF);
			set
			{
				Data &= 0xFFFF0FFF;
				Data |= (uint)value << 12;
			}
		}

		/// <summary>
		/// The source which should be used to generate the texture coordinates
		/// </summary>
		public GCTexGenSrc TexGenSrc
		{
			get => (GCTexGenSrc)((Data >> 4) & 0xFF);
			set
			{
				Data &= 0xFFFFF00F;
				Data |= (uint)value << 4;
			}
		}

		/// <summary>
		/// The id of the matrix to use for generating the texture coordinates
		/// </summary>
		public GCTexGenMatrix MatrixId
		{
			get => (GCTexGenMatrix)(Data & 0xF);
			set
			{
				Data &= 0xFFFFFFF0;
				Data |= (uint)value;
			}
		}

		public TexCoordGenParameter() : base(ParameterType.TexCoordGen)
		{

		}

		/// <summary>
		/// Create a custom Texture coordinate generation parameter
		/// </summary>
		/// <param name="texCoordId">The output location of the generated texture coordinates</param>
		/// <param name="texGenType">The function to use for generating the texture coordinates</param>
		/// <param name="texGenSrc">The source which should be used to generate the texture coordinates</param>
		/// <param name="matrixId">The id of the matrix to use for generating the texture coordinates</param>
		public TexCoordGenParameter(GCTexCoordID texCoordId, GCTexGenType texGenType, GCTexGenSrc texGenSrc, GCTexGenMatrix matrixId) : base(ParameterType.TexCoordGen)
		{
			TexCoordId = texCoordId;
			TexGenType = texGenType;
			TexGenSrc = texGenSrc;
			MatrixId = matrixId;
		}
	}
}
