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
		StripFlags1 = 2,
		StripFlags2 = 3,
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
				ParameterType.StripFlags1 => new StripFlagsParameter(),
				ParameterType.BlendAlpha => new BlendAlphaParameter(),
				ParameterType.DiffuseColor => new DiffuseColorParameter(),
				ParameterType.AmbientColor => new AmbientColorParameter(),
				ParameterType.SpecularColor => new SpecularColorParameter(),
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
			string type_str = Type switch
			{
				ParameterType.VtxAttrFmt => "GJ_MT_VTXATTR",
				ParameterType.IndexAttributeFlags => "GJ_MT_VCD",
				ParameterType.StripFlags1 => "GJ_MT_FST1",
				ParameterType.StripFlags2 => "GJ_MT_FST2",
				ParameterType.BlendAlpha => "GJ_MT_BLEND",
				ParameterType.DiffuseColor => "GJ_MT_DIFFUSE",
				ParameterType.AmbientColor => "GJ_MT_AMBIENT",
				ParameterType.SpecularColor => "GJ_MT_SPECULAR",
				ParameterType.Texture => "GJ_MT_TEXTURE",
				ParameterType.TextureTEVMode => "GJ_MT_TEVORDER",
				ParameterType.TexCoordGen => "GJ_MT_TEXGEN",
				_ => $"{(uint)Type}"
			};
			writer.WriteLine($"GjMaterial ( {type_str}, 0x{(uint)Data:X} ),");
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

			writer.WriteLine($"GjVtxAttr  ( {attr_str}, {comptype_str}, {compsize_str}, {(uint)UVScale} ),");
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

					list.Add($"GjVtxIdxAttr ( {attr_str}, {src_str} )");
				}
			}

			if (list.Count == 0)
			{
				writer.WriteLine("GjVtxDesc  ( NULL ),");
			}
			else
			{
				writer.WriteLine($"GjVtxDesc  ( {string.Join(" | ", list)} ),");
			}
		}
	}

	/// <summary>
	/// Holds strip information
	/// </summary>
	[Serializable]
	public class StripFlagsParameter : GCParameter
	{
		/// <summary>
		/// Strip flags. Many of these are analogous with Ninja strip flags
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
		public byte ChannelNum
		{
			get => (byte)(Data & 0x3);
			set
			{
				Data &= 0xFFFFFFF0;
				Data |= (uint)((value & 0x3));
			}
		}
		public byte TexGenCount
		{
			get => (byte)((Data >> 4) & 0xF);
			set
			{
				Data &= 0xFFFFFF0F;
				Data |= (uint)((value & 0xF) << 4);
			}
		}
		public bool IgnoreLight
		{
			get { return (LightingFlags & 0x100) == 0x100; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x100) | (value ? 0x100 : 0)); }
		}
		public bool IgnoreSpecular
		{
			get { return (LightingFlags & 0x200) == 0x200; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x200) | (value ? 0x200 : 0)); }
		}
		public bool IgnoreAmbient
		{
			get { return (LightingFlags & 0x400) == 0x400; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x400) | (value ? 0x400 : 0)); }
		}
		public bool VertexDiffuse
		{
			get { return (LightingFlags & 0x800) == 0x800; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x800) | (value ? 0x800 : 0)); }
		}
		public bool VertexAmbient
		{
			get { return (LightingFlags & 0x1000) == 0x1000; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x1000) | (value ? 0x1000 : 0)); }
		}
		public bool UseAlpha
		{
			get { return (LightingFlags & 0x2000) == 0x2000; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x2000) | (value ? 0x2000 : 0)); }
		}
		public bool NoPunchthrough
		{
			get { return (LightingFlags & 0x4000) == 0x4000; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x4000) | (value ? 0x4000 : 0)); }
		}
		public bool DoubleSided
		{
			get { return (LightingFlags & 0x8000) == 0x8000; }
			set { LightingFlags = (ushort)((LightingFlags & ~0x8000) | (value ? 0x8000 : 0)); }
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
		/// Creates a strip parameter with the default data
		/// </summary>
		public StripFlagsParameter() : base(ParameterType.StripFlags1)
		{
			// Default value: Ignore Light, Ignore Ambient, Ignore Specular, Vertex Material
			LightingFlags = 0xB11;
			ShadowStencil = 1;
		}

		public StripFlagsParameter(ushort lightingFlags, byte shadowStencil) : base(ParameterType.StripFlags1)
		{
			LightingFlags = lightingFlags;
			ShadowStencil = shadowStencil;
		}

		public override void ToNJA(TextWriter writer)
		{
			string flags = string.Empty;

			if (IgnoreLight)
				flags += "GJ_FST_IL|";
			if (IgnoreSpecular)
				flags += "GJ_FST_IS|";
			if (IgnoreAmbient)
				flags += "GJ_FST_IA|";
			if (VertexDiffuse)
				flags += "GJ_FST_VM|";
			if (VertexAmbient)
				flags += "GJ_FST_VA|";
			if (UseAlpha)
				flags += "GJ_FST_UA|";
			if (NoPunchthrough)
				flags += "GJ_FST_NPT|";
			if (DoubleSided)
				flags += "GJ_FST_DB|";
			if (flags == string.Empty)
				flags = "0x0";
			else
				flags = flags.Remove(flags.Length - 1);
			writer.WriteLine($"GjFst1     ( GJD_FST_CHAN( {ChannelNum} ), GJD_FST_TEXGEN( {TexGenCount} ), {flags}, GJD_FST_TEVSTG( {ShadowStencil} ) ),");
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
				_ => $"{(uint)DestAlpha}"
			};

			writer.WriteLine($"GjBlend    ( {mode_str}, {src_str}, {dst_str} ),");
		}
	}

	/// <summary>
	/// Diffuse color of the geometry
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
			writer.WriteLine($"GjDiffuse  ( {DiffuseColor.Alpha}, {DiffuseColor.Red}, {DiffuseColor.Green}, {DiffuseColor.Blue} ),");
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

		public AmbientColorParameter() : base(ParameterType.DiffuseColor)
		{
			Data = uint.MinValue;
		}
		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine($"GjAmbient  ( {AmbientColor.Alpha}, {AmbientColor.Red}, {AmbientColor.Green}, {AmbientColor.Blue} ),");
		}
	}

	/// <summary>
	/// Specular color of the geometry
	/// </summary>
	[Serializable]
	public class SpecularColorParameter : GCParameter
	{
		/// <summary>
		/// The Color of the geometry
		/// </summary>
		public Color SpecularColor
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

		public SpecularColorParameter() : base(ParameterType.DiffuseColor)
		{
			Data = uint.MinValue;
		}
		public override void ToNJA(TextWriter writer)
		{
			writer.WriteLine($"GjSpecular ( {SpecularColor.Alpha}, {SpecularColor.Red}, {SpecularColor.Green}, {SpecularColor.Blue} ),");
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

			writer.WriteLine($"GjTexture  ( {TextureId}, {wrap_s_str}, {wrap_t_str}, {(Data >> 13) & 3} ),");
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
			uint tevstage = Data >> 12;
			string tevstage_str = tevstage switch
			{
				0 => "GJ_TEVSTAGE0",
				1 => "GJ_TEVSTAGE1",
				2 => "GJ_TEVSTAGE2",
				3 => "GJ_TEVSTAGE3",
				4 => "GJ_TEVSTAGE4",
				5 => "GJ_TEVSTAGE5",
				6 => "GJ_TEVSTAGE6",
				7 => "GJ_TEVSTAGE7",
				8 => "GJ_TEVSTAGE8",
				9 => "GJ_TEVSTAGE9",
				10 => "GJ_TEVSTAGE10",
				11 => "GJ_TEVSTAGE11",
				12 => "GJ_TEVSTAGE12",
				13 => "GJ_TEVSTAGE13",
				14 => "GJ_TEVSTAGE14",
				15 => "GJ_TEVSTAGE15",
				_ => $"{tevstage}"
			};

			uint texcoord = (Data >> 8) & 0xF;
			string texcoord_str = texcoord switch
			{
				0 => "GJ_TEXCOORD0",
				1 => "GJ_TEXCOORD1",
				2 => "GJ_TEXCOORD2",
				3 => "GJ_TEXCOORD3",
				4 => "GJ_TEXCOORD4",
				5 => "GJ_TEXCOORD5",
				6 => "GJ_TEXCOORD6",
				7 => "GJ_TEXCOORD7",
				255 => "GJ_TEXCOORDNULL",
				_ => $"{texcoord}"
			};

			uint texmap = (Data >> 4) & 0xF;
			string texmap_str = texmap switch
			{
				0 => "GJ_TEXMAP0",
				1 => "GJ_TEXMAP1",
				2 => "GJ_TEXMAP2",
				3 => "GJ_TEXMAP3",
				4 => "GJ_TEXMAP4",
				5 => "GJ_TEXMAP5",
				6 => "GJ_TEXMAP6",
				7 => "GJ_TEXMAP7",
				255 => "GJ_TEXMAPNULL",
				_ => $"{texmap}"
			};

			uint unk = Data & 0xF;

			writer.WriteLine($"GjTevOrder ( {tevstage_str}, {texcoord_str}, {texmap_str}, {unk} ),");
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
				GCTexGenMatrix.Matrix1 => "GJ_TEXMTX1",
				GCTexGenMatrix.Matrix2 => "GJ_TEXMTX2",
				GCTexGenMatrix.Matrix3 => "GJ_TEXMTX3",
				GCTexGenMatrix.Matrix4 => "GJ_TEXMTX4",
				GCTexGenMatrix.Matrix5 => "GJ_TEXMTX5",
				GCTexGenMatrix.Matrix6 => "GJ_TEXMTX6",
				GCTexGenMatrix.Matrix7 => "GJ_TEXMTX7",
				GCTexGenMatrix.Matrix8 => "GJ_TEXMTX8",
				GCTexGenMatrix.Matrix9 => "GJ_TEXMTX9",
				GCTexGenMatrix.Identity => "GJ_IDENTITY",
				_ => $"{(uint)MatrixId}"
			};

			string texcoordid_str = TexCoordId switch
			{
				GCTexCoordID.TexCoord0 => "GJ_TEXCOORD0",
				GCTexCoordID.TexCoord1 => "GJ_TEXCOORD1",
				GCTexCoordID.TexCoord2 => "GJ_TEXCOORD2",
				GCTexCoordID.TexCoord3 => "GJ_TEXCOORD3",
				GCTexCoordID.TexCoord4 => "GJ_TEXCOORD4",
				GCTexCoordID.TexCoord5 => "GJ_TEXCOORD5",
				GCTexCoordID.TexCoord6 => "GJ_TEXCOORD6",
				GCTexCoordID.TexCoord7 => "GJ_TEXCOORD7",
				GCTexCoordID.TexCoordNull => "GJ_TEXCOORDNULL",
				_ => $"{(uint)TexCoordId}"
			};

			string texgensrc_str = TexGenSrc switch
			{
				GCTexGenSrc.Position => "GJ_TG_POS",
				GCTexGenSrc.Normal => "GJ_TG_NRM",
				GCTexGenSrc.Tex0 => "GJ_TG_TEX0",
				_ => $"{(uint)TexGenSrc}"
			};

			string texgentype_str = TexGenType switch
			{
				GCTexGenType.Matrix3x4 => "GJ_TG_MTX3x4",
				GCTexGenType.Matrix2x4 => "GJ_TG_MTX2x4",
				_ => $"{(uint)TexGenType}"
			};

			writer.WriteLine($"GjTexGen   ( {texcoordid_str}, {texgentype_str}, {texgensrc_str}, {matrixid_str} ),");
		}
	}
}
