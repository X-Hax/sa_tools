using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
    public enum GXComponentCount
    {
        Position_XY = 0,
        Position_XYZ,

        Normal_XYZ,
        Normal_NBT,
        Normal_NBT3,

        Color_RGB,
        Color_RGBA,

        TexCoord_S,
        TexCoord_ST
    }
}
