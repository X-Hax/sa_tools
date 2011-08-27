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
    public enum ObjectFlags : int
    {
        NoPosition = 0x01,
        NoRotate = 0x02,
        NoScale = 0x04,
        NoDisplay = 0x08,
        NoChildren = 0x10,
        RotateZYX = 0x20,
        NoAnimate = 0x40,
        ObjectFlags_80 = 0x80
    }

    [Flags()]
    public enum AnimFlags : ushort
    {
        Translate = 0x1,
        Rotate = 0x2,
        Scale = 0x4
    }

    [Flags()]
    public enum SurfaceFlags : int
    {
        Solid = 0x1,
        Water = 0x2,
        NoFriction = 0x4,
        NoAcceleration = 0x8,
        IncreasedAcceleration = 0x80,
        Diggable = 0x100,
        Unclimbable = 0x1000,
        Hurt = 0x10000,
        Footprints = 0x100000,
        Visible = unchecked((int)0x80000000)
    }

    public enum ModelFormat
    {
        SA1,
        SADX,
        SA2
    }
}