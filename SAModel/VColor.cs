using System;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public static class VColor
    {
        public static Color FromBytes(byte[] file, int address) { return Color.FromArgb(ByteConverter.ToInt32(file, address)); }
        public static byte[] GetBytes(Color Color) { return ByteConverter.GetBytes(Color.ToArgb()); }
        public static int Size { get { return 4; } }
    }
}
