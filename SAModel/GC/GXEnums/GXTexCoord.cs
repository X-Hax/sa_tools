using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public enum GXTexCoordID
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

	public enum GXTexGenType
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
		SRTG = 0xA //idk?
	}

	public enum GXTexGenSrc
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
}
