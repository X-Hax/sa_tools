using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
    public enum GXPrimitiveType
    {
        Triangles     = 0x90,
        TriangleStrip = 0x98,
        TriangleFan   = 0xA0,
        Lines         = 0xA8,
        LineStrip     = 0xB0,
        Points        = 0xB8
    }
}
