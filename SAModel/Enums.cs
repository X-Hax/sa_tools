using System;

namespace SonicRetro.SAModel
{
    public enum PolyType
    {
        Triangles,
        Quads,
        Strips,
        Strips2
    }

    [Flags()]
    public enum ObjectFlags
    {
        NoPosition = 0x01,
        NoRotate = 0x02,
        NoScale = 0x04,
        NoDisplay = 0x08,
        NoChildren = 0x10,
        RotateZYX = 0x20,
        NoAnimate = 0x40
    }
}