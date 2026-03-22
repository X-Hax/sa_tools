using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		StripFlags = 3,
		BlendAlpha = 4,
		DiffuseColor = 5,
		AmbientColor = 6,
		SpecularColor = 7,
		Texture = 8,
		TextureTEVMode = 9,
		TexCoordGen = 10,
	}
	
	/// <summary>
	/// Base class for all GC parameter types. <br/>
	/// Used to store geometry information (like materials).
	/// </summary>
	[Serializable]
	public abstract class GCParameter : ICloneable
	{
		/// <summary>
		/// The type of parameter
		/// </summary>
		public readonly ParameterType Type;

		/// <summary>
		/// All parameter data is stored in these 4 bytes
		/// </summary>
		public uint Data;

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
				ParameterType.DiffuseColor => new DiffuseColorParameter(),
				ParameterType.Texture => new TextureParameter(),
				ParameterType.TextureTEVMode => new TexTEVModeParameter(),
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
		object ICloneable.Clone() => Clone();

		public GCParameter Clone()
		{
			GCParameter result = (GCParameter)MemberwiseClone();
			return result;
		}

		public virtual void ToNJA(TextWriter writer)
		{
			writer.WriteLine($"GjMat     ( {(uint)Type}, {(uint)Data} ),");
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
		/// An enum that determines the scaling for the mesh's UVs. <br/>
		/// This is necessary for meshes that use UVs.
		/// </summary>
		public GCUVScale UVScale
		{
			get => (GCUVScale)(Data & 0xFF);
			set
			{
				Data &= 0xFFFFFF00;
				Data |= (byte)value;
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

		public override void ToNJA(TextWriter writer)
		{
			string attr_str = VertexAttribute switch
			{
				GCVertexAttribute.PositionMatrixIdx => "GJ_VA_NULL",
				GCVertexAttribute.Position => "GJ_VA_POS",
				GCVertexAttribute.Normal => "GJ_VA_NRM",
				GCVertexAttribute.Color0 => "GJ_VA_CLR0",
				GCVertexAttribute.Color1 => "GJ_VA_CLR1",
				GCVertexAttribute.Tex0 => "GJ_VA_TEX0",
				GCVertexAttribute.Tex1 => "GJ_VA_TEX1",
				GCVertexAttribute.Tex2 => "GJ_VA_TEX2",
				GCVertexAttribute.Tex3 => "GJ_VA_TEX3",
				GCVertexAttribute.Tex4 => "GJ_VA_TEX4",
				GCVertexAttribute.Tex5 => "GJ_VA_TEX5",
				GCVertexAttribute.Tex6 => "GJ_VA_TEX6",
				GCVertexAttribute.Tex7 => "GJ_VA_TEX7",
				_ => $"{(uint)VertexAttribute}"
			};

			uint comptype = (Data >> 12) & 0xF;
			string comptype_str = comptype switch
			{
				0 => "GJ_POS_XY",
				1 => "GJ_POS_XYZ",
				2 => "GJ_NRM_XYZ",
				3 => "GJ_NRM_NBT",
				4 => "GJ_NRM_NBT3",
				5 => "GJ_CLR_RGB",
				6 => "GJ_CLR_RGBA",
				7 => "GJ_TEX_S",
				8 => "GJ_TEX_ST",
				_ => $"{comptype}"
			};

			uint compsize = (Data >> 8) & 0xF;
			string compsize_str = compsize switch
			{
				0 => "GJ_U8",
				1 => "GJ_S8",
				2 => "GJ_U16",
				3 => "GJ_S16",
				4 => "GJ_F32",
				5 => "GJ_RGB565",
				6 => "GJ_RGB8",
				7 => "GJ_RGBX8",
				8 => "GJ_RGBA4",
				9 => "GJ_RGBA6",
				10 => "GJ_RGBA8",
				_ => $"{compsize}"
			};

			writer.WriteLine($"GjVtxAttr ( {attr_str}, {comptype_str}, {compsize_str}, {(uint)UVScale} ),");
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

		public override void ToNJA(TextWriter writer)
		{
			var list = new List<string>();
			for (int i = 0; i < 16; i++)
			{
				uint src = (Data >> (i * 2)) & 0b11;
				if (src != 0)
				{
					string attr_str = i switch
					{
						0 => "GJ_VA_NULL",
						1 => "GJ_VA_POS",
						2 => "GJ_VA_NRM",
						3 => "GJ_VA_CLR0",
						4 => "GJ_VA_CLR1",
						5 => "GJ_VA_TEX0",
						6 => "GJ_VA_TEX1",
						7 => "GJ_VA_TEX2",
						8 => "GJ_VA_TEX3",
						9 => "GJ_VA_TEX4",
						10 => "GJ_VA_TEX5",
						11 => "GJ_VA_TEX6",
						12 => "GJ_VA_TEX7",
						_ => $"{i}"
					};

					string src_str = src switch
					{
						1 => "GJ_DIRECT",
						2 => "GJ_INDEX8",
						3 => "GJ_INDEX16",
						_ => $"{src}"
					};

					list.Add($"GjVtxDescAttr ( {attr_str}, {src_str} )");
				}
			}

			if (list.Count == 0)
			{
				writer.WriteLine("GjVtxDesc ( NULL ),");
			}
			else
			{
				writer.WriteLine($"GjVtxDesc ( {string.Join(" | ", list)} ),");
			}
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

		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine($"GjLight   ( {Data} ),");
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

		public override void ToNJA(TextWriter writer)
		{
			uint mode = Data >> 14;

			string mode_str = mode switch
			{
				0 => "GJ_BM_NONE",
				1 => "GJ_BM_BLEND",
				_ => $"{mode}"
			};

			string src_str = SourceAlpha switch
			{
				GCBlendModeControl.Zero => "GJ_BL_ZERO",
				GCBlendModeControl.One => "GJ_BL_ONE",
				GCBlendModeControl.SrcColor => "GJ_BL_SRCCLR",
				GCBlendModeControl.InverseSrcColor => "GJ_BL_INVSRCCLR",
				GCBlendModeControl.SrcAlpha => "GJ_BL_SRCALPHA",
				GCBlendModeControl.InverseSrcAlpha => "GJ_BL_INVSRCALPHA",
				GCBlendModeControl.DstAlpha => "GJ_BL_DSTALPHA",
				GCBlendModeControl.InverseDstAlpha => "GJ_BL_INVDSTALPHA",
				_ => $"{(uint)SourceAlpha}"
			};

			string dst_str = DestAlpha switch
			{
				GCBlendModeControl.Zero => "GJ_BL_ZERO",
				GCBlendModeControl.One => "GJ_BL_ONE",
				GCBlendModeControl.SrcColor => "GJ_BL_DSTCLR",
				GCBlendModeControl.InverseSrcColor => "GJ_BL_INVDSTCLR",
				GCBlendModeControl.SrcAlpha => "GJ_BL_SRCALPHA",
				GCBlendModeControl.InverseSrcAlpha => "GJ_BL_INVSRCALPHA",
				GCBlendModeControl.DstAlpha => "GJ_BL_DSTALPHA",
				GCBlendModeControl.InverseDstAlpha => "GJ_BL_INVDSTALPHA",
				_ => $"{(uint)SourceAlpha}"
			};

			writer.WriteLine($"GjBlend   ( {mode_str}, {src_str}, {dst_str} ),");
		}
	}

	/// <summary>
	/// Ambient color of the geometry
	/// </summary>
	[Serializable]
	public class DiffuseColorParameter : GCParameter
	{
		/// <summary>
		/// The Color of the geometry
		/// </summary>
		public Color DiffuseColor
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

		public DiffuseColorParameter() : base(ParameterType.DiffuseColor)
		{
			Data = uint.MaxValue; // White is default
		}
		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine($"GjDiffuse ( {DiffuseColor.Alpha}, {DiffuseColor.Red}, {DiffuseColor.Green}, {DiffuseColor.Blue} ),");
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

		public override void ToNJA(TextWriter writer)
		{
			uint wrap_s = (Data >> 16) & 3;
			string wrap_s_str = wrap_s switch
			{
				0 => "GJ_CLAMP",
				1 => "GJ_REPEAT",
				2 => "GJ_MIRROR",
				_ => $"{wrap_s}"
			};

			uint wrap_t = (Data >> 18) & 3;
			string wrap_t_str = wrap_t switch
			{
				0 => "GJ_CLAMP",
				1 => "GJ_REPEAT",
				2 => "GJ_MIRROR",
				_ => $"{wrap_t}"
			};

			writer.WriteLine($"GjTexID   ( {TextureId}, {wrap_s_str}, {wrap_t_str}, {(Data >> 13) & 3} ),");
		}
	}

	/// <summary>
	/// No idea what this is for, but its needed
	/// </summary>
	[Serializable]
	public class TexTEVModeParameter : GCParameter
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

		public TexTEVModeParameter() : base(ParameterType.TextureTEVMode)
		{
			// Default values
			Unknown1 = 4;
			Unknown2 = 0;
		}

		public override void ToNJA(TextWriter writer)
		{
			uint type = (Data >> 4) >> 0xF;
			string type_str = type switch
			{
				0 => "GJ_TG_MTX3x4",
				_ => $"{type}"
			};

			uint src = Data & 0xF;
			string src_str = src switch
			{
				4 => "GJ_TG_TEX0",
				_ => $"{src}"
			};

			uint texcoord = Data >> 12;
			string texcoord_str = texcoord switch
			{
				0 => "GJ_TEXCOORD0",
				_ => $"{texcoord}"
			};

			uint mtxsrc = Data >> 12;
			string mtxsrc_str = mtxsrc switch
			{
				0 => "GJ_TEXMTX0",
				10 => "GJ_IDENTITY",
				_ => $"{mtxsrc}"
			};

			writer.WriteLine($"GjTexGen  ( {texcoord_str}, {type_str}, {src_str}, {mtxsrc_str} ),");
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
		
		public override void ToNJA(TextWriter writer)
		{
			string matrixid_str = MatrixId switch
			{
				GCTexGenMatrix.Matrix0 => "GJ_TEXMTX0",
				GCTexGenMatrix.Identity => "GJ_IDENTITY",
				_ => $"{(uint)MatrixId}"
			};

			string texcoordid_str = TexCoordId switch
			{
				GCTexCoordID.TexCoord0 => "GJ_TEXCOORD0",
				_ => $"{(uint)TexCoordId}"
			};

			string texgensrc_str = TexGenSrc switch
			{
				GCTexGenSrc.Tex0 => "GJ_TG_TEX0",
				_ => $"{(uint)TexGenSrc}"
			};

			string texgentype_str = TexGenType switch
			{
				GCTexGenType.Matrix3x4 => "GJ_MTX3x4",
				GCTexGenType.Matrix2x4 => "GJ_MTX2x4",
				_ => $"{(uint)TexGenType}"
			};

			writer.WriteLine($"GjTexMtx  ( {matrixid_str}, {texcoordid_str}, {texgensrc_str}, {texgentype_str} ),");
		}
	}
}
