using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
    public enum GXDataType
    {
        Unsigned8  = 0x0,
        Signed8   = 0x1,
        Unsigned16  = 0x2,
        Signed16   = 0x3,
        Float32   = 0x4,
		RGB565,
		RGB8,
		RGBX8,
		RGBA4,
		RGBA6,
        RGBA8
    }
}
