using System;

namespace SAModel.GC
{
	/// <summary>
	/// The blendmode for the geometries surface
	/// </summary>
	public enum GCBlendModeControl
	{
		/// <summary>
		/// 0
		/// </summary>
		Zero = 0,
		/// <summary>
		/// 1
		/// </summary>
		One = 1,
		/// <summary>
		/// Source color
		/// </summary>
		SrcColor = 2,
		/// <summary>
		/// 1 - Source Color
		/// </summary>
		InverseSrcColor = 3,
		/// <summary>
		/// Source alpha
		/// </summary>
		SrcAlpha = 4,
		/// <summary>
		/// 1 - Source alpha
		/// </summary>
		InverseSrcAlpha = 5,
		/// <summary>
		/// Framebuffer alpha
		/// </summary>
		DstAlpha = 6,
		/// <summary>
		/// 1 - Framebuffer alpha
		/// </summary>
		InverseDstAlpha = 7
	}

	/// <summary>
	/// The component structure of the data
	/// </summary>
	public enum GCStructType
	{
		/// <summary>
		/// 2D Position (X, Y)
		/// </summary>
		Position_XY = 0,
		/// <summary>
		/// 3D Position (X, Y, Z)
		/// </summary>
		Position_XYZ = 1,

		/// <summary>
		/// XYZ Normal
		/// </summary>
		Normal_XYZ = 2,
		/// <summary>
		/// Normal
		/// </summary>
		Normal_NBT = 3,
		/// <summary>
		/// Normal
		/// </summary>
		Normal_NBT3 = 4,

		/// <summary>
		/// Color with 3 Channels
		/// </summary>
		Color_RGB = 5,
		/// <summary>
		/// Color with 4 Channels
		/// </summary>
		Color_RGBA = 6,

		/// <summary>
		/// 1D Texture Coordinates
		/// </summary>
		TexCoord_S = 7,
		/// <summary>
		/// 2D Texture Coordinates
		/// </summary>
		TexCoord_ST = 8
	}

	/// <summary>
	/// The component type of the data
	/// </summary>
	public enum GCDataType
	{
		/// <summary>
		/// Equal to <see cref="byte"/>
		/// </summary>
		Unsigned8 = 0,
		/// <summary>
		/// Equal to <see cref="char"/>
		/// </summary>
		Signed8 = 1,
		/// <summary>
		/// Equal to <see cref="ushort"/>
		/// </summary>
		Unsigned16 = 2,
		/// <summary>
		/// Equal to <see cref="short"/>
		/// </summary>
		Signed16 = 3,
		/// <summary>
		/// Equal to <see cref="float"/>
		/// </summary>
		Float32 = 4,
		/// <summary>
		/// RGB565 struct (Length: 2)
		/// </summary>
		RGB565 = 5,
		/// <summary>
		/// RGB8 struct (length: 4)
		/// </summary>
		RGB8 = 6,
		/// <summary>
		/// RGBX8 struct (length: 4)
		/// </summary>
		RGBX8 = 7,
		/// <summary>
		/// RGBA4 struct (length: 2)
		/// </summary>
		RGBA4 = 8,
		/// <summary>
		/// RGBA6 struct (length: 3)
		/// </summary>
		RGBA6 = 9,
		/// <summary>
		/// RGBA8 struct (length: 4)
		/// </summary>
		RGBA8 = 10
	}

	/// <summary>
	/// Primitive type on how faces are stored
	/// </summary>
	public enum GCPrimitiveType
	{
		/// <summary>
		/// Triangle list
		/// </summary>
		Triangles = 0x90,
		/// <summary>
		/// Triangle strip
		/// </summary>
		TriangleStrip = 0x98,
		/// <summary>
		/// Triangle fan
		/// </summary>
		TriangleFan = 0xA0,
		/// <summary>
		/// Edge list
		/// </summary>
		Lines = 0xA8,
		/// <summary>
		/// Edge strip
		/// </summary>
		LineStrip = 0xB0,
		/// <summary>
		/// Single points in space
		/// </summary>
		Points = 0xB8
	}

	/// <summary>
	/// The output location for the generated texture coordinates
	/// </summary>
	public enum GCTexCoordID
	{
		TexCoord0 = 0x0,
		TexCoord1 = 0x1,
		TexCoord2 = 0x2,
		TexCoord3 = 0x3,
		TexCoord4 = 0x4,
		TexCoord5 = 0x5,
		TexCoord6 = 0x6,
		TexCoord7 = 0x7,
		TexCoordMax = 0x8,
		TexCoordNull = 0xFF,
	}

	/// <summary>
	/// The function used to generate the texture coordinates
	/// </summary>
	public enum GCTexGenType
	{
		Matrix3x4 = 0x0,
		Matrix2x4 = 0x1,
		Bump0 = 0x2,
		Bump1 = 0x3,
		Bump2 = 0x4,
		Bump3 = 0x5,
		Bump4 = 0x6,
		Bump5 = 0x7,
		Bump6 = 0x8,
		Bump7 = 0x9,
		SRTG = 0xA
	}

	/// <summary>
	/// The source, which should be used to generate the texture coordinates
	/// </summary>
	public enum GCTexGenSrc
	{
		Position = 0x0,
		Normal = 0x1,
		Binormal = 0x2,
		Tangent = 0x3,
		Tex0 = 0x4,
		Tex1 = 0x5,
		Tex2 = 0x6,
		Tex3 = 0x7,
		Tex4 = 0x8,
		Tex5 = 0x9,
		Tex6 = 0xA,
		Tex7 = 0xB,
		TexCoord0 = 0xC,
		TexCoord1 = 0xD,
		TexCoord2 = 0xE,
		TexCoord3 = 0xF,
		TexCoord4 = 0x10,
		TexCoord5 = 0x11,
		TexCoord6 = 0x12,
		Color0 = 0x13,
		Color1 = 0x14,
	}

	/// <summary>
	/// The generation matrix to use when generating the texture coordinates
	/// </summary>
	public enum GCTexGenMatrix
	{
		Matrix0 = 0,
		Matrix1 = 1,
		Matrix2 = 2,
		Matrix3 = 3,
		Matrix4 = 4,
		Matrix5 = 5,
		Matrix6 = 6,
		Matrix7 = 7,
		Matrix8 = 8,
		Matrix9 = 9,
		Identity = 10
	}

	/// <summary>
	/// Vertex attribute type
	/// </summary>
	public enum GCVertexAttribute
	{
		PositionMatrixIdx = 0,
		Position = 1,
		Normal = 2,
		Color0 = 3,
		Color1 = 4,
		Tex0 = 5,
		Tex1 = 6,
		Tex2 = 7,
		Tex3 = 8,
		Tex4 = 9,
		Tex5 = 10,
		Tex6 = 11,
		Tex7 = 12,

		Null = 255
	}

	/// <summary>
	/// Holds information about the vertex data thats stored in the geometry
	/// </summary>
	[Flags]
	public enum GCIndexAttributeFlags : ushort
	{
		/// <summary>
		/// Unused
		/// </summary>
		Bit0 = 1 << 0,
		/// <summary>
		/// Unused
		/// </summary>
		Bit1 = 1 << 1,
		/// <summary>
		/// Whether the position indices are 16 bit rather than 8 bit
		/// </summary>
		Position16BitIndex = 1 << 2,
		/// <summary>
		/// Whether the Geometry contains position data
		/// </summary>
		HasPosition = 1 << 3,
		/// <summary>
		/// Whether the normal indices are 16 bit rather than 8 bit
		/// </summary>
		Normal16BitIndex = 1 << 4,
		/// <summary>
		/// Whether the Geometry contains normal data
		/// </summary>
		HasNormal = 1 << 5,
		/// <summary>
		/// Whether the color indices are 16 bit rather than 8 bit
		/// </summary>
		Color16BitIndex = 1 << 6,
		/// <summary>
		/// Whether the Geometry contains color data
		/// </summary>
		HasColor = 1 << 7,
		/// <summary>
		/// Unused
		/// </summary>
		Bit8 = 1 << 8,
		/// <summary>
		/// Unused
		/// </summary>
		Bit9 = 1 << 9,
		/// <summary>
		/// Whether the uv indices are 16 bit rather than 8 bit
		/// </summary>
		UV16BitIndex = 1 << 10,
		/// <summary>
		/// Whether the Geometry contains uv data
		/// </summary>
		HasUV = 1 << 11,
		/// <summary>
		/// Unused
		/// </summary>
		Bit12 = 1 << 12,
		/// <summary>
		/// Unused
		/// </summary>
		Bit13 = 1 << 13,
		/// <summary>
		/// Unused
		/// </summary>
		Bit14 = 1 << 14,
		/// <summary>
		/// Unused
		/// </summary>
		Bit15 = 1 << 15,
	}

	/// <summary>
	/// Texture tilemode
	/// </summary>
	[Flags]
	public enum GCTileMode
	{
		WrapV = 1 << 0,		// Enables Tile Wrapping/Repeating on V axis
		MirrorV = 1 << 1,	// Enables Tile Mirror on V axis (overrides clamping)
		WrapU = 1 << 2,		// Enables Tile Wrapping/Repeating on U axis
		MirrorU = 1 << 3,	// Enables Tile Mirror on U axis (overrides clamping)
		Unk_1 = 1 << 4,		
		Mask = (1 << 5) - 1
	}

	public enum GCUVScale
	{
		Default = 0,		// Normal Display
		NoUV1 = 1,          // UV Map set to 0
		NoUV2 = 2,			// Same as NoUV1	
		NoUV3 = 3,          // Same as NoUV1	
		NoUV4 = 4,          // Same as NoUV1	
		NoUV5 = 5,          // Same as NoUV1	
		NoUV6 = 6,          // Same as NoUV1	
		NoUV7 = 7,          // Same as NoUV1	
		Scale1 = 8,			// UV * 1.00
		Scale2 = 9,         // UV * 0.50
		Scale3 = 0xA,       // UV * 0.25
		Scale4 = 0xB,       // UV * 0.125
		Scale5 = 0xC,       // UV * 0.0625
		Scale6 = 0xD,       // UV * 0.03125
		Scale7 = 0xE,       // UV * 0.015625
		Scale8 = 0xF,       // UV * 0.0078125
		Scale9 = 0x10,		// Same as NoUV1

	}

	/// <summary>
	/// Used to convert between NJ and GC enums
	/// </summary>
	public static class GCEnumConverter
	{
		public static AlphaInstruction GXToNJAlphaInstruction(GCBlendModeControl gx)
		{
			switch (gx)
			{
				case GCBlendModeControl.SrcAlpha:
					return AlphaInstruction.SourceAlpha;
				case GCBlendModeControl.DstAlpha:
					return AlphaInstruction.DestinationAlpha;
				case GCBlendModeControl.InverseSrcAlpha:
					return AlphaInstruction.InverseSourceAlpha;
				case GCBlendModeControl.InverseDstAlpha:
					return AlphaInstruction.InverseDestinationAlpha;
				case GCBlendModeControl.SrcColor:
					return AlphaInstruction.OtherColor;
				case GCBlendModeControl.InverseSrcColor:
					return AlphaInstruction.InverseOtherColor;
				case GCBlendModeControl.One:
					return AlphaInstruction.One;
				case GCBlendModeControl.Zero:
					return AlphaInstruction.Zero;
			}

			return AlphaInstruction.Zero;
		}

		public static GCBlendModeControl NJtoGXBlendModeControl(AlphaInstruction nj)
		{
			switch (nj)
			{
				case AlphaInstruction.SourceAlpha:
					return GCBlendModeControl.SrcAlpha;
				case AlphaInstruction.DestinationAlpha:
					return GCBlendModeControl.DstAlpha;
				case AlphaInstruction.InverseSourceAlpha:
					return GCBlendModeControl.InverseSrcAlpha;
				case AlphaInstruction.InverseDestinationAlpha:
					return GCBlendModeControl.InverseDstAlpha;
				case AlphaInstruction.OtherColor:
					return GCBlendModeControl.SrcColor;
				case AlphaInstruction.InverseOtherColor:
					return GCBlendModeControl.InverseSrcColor;
				case AlphaInstruction.One:
					return GCBlendModeControl.One;
				case AlphaInstruction.Zero:
					return GCBlendModeControl.Zero;
			}

			return GCBlendModeControl.Zero;
		}

	}

}
