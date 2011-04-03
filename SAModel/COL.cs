using System;

namespace SonicRetro.SAModel
{
    public class COL
    {
        public Vertex Center { get; set; }
        public float Radius { get; set; }
        public byte[] Unknown1 { get; private set; }
        public Object Object { get; set; }
        public int Unknown2 { get; set; }
        public SurfaceFlags Flags { get; set; }

        public static int Size { get { return 0x24; } }

        public COL(byte[] file, int address, uint imageBase, bool DX)
        {
            Center = new Vertex(file, address);
            Radius = BitConverter.ToSingle(file, address + 0xC);
            Unknown1 = new byte[8];
            Array.Copy(file, address + 0x10, Unknown1, 0, 8);
            int tmpaddr = BitConverter.ToInt32(file, address + 0x18);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Object = new Object(file, tmpaddr, imageBase, DX);
            }
            Unknown2 = BitConverter.ToInt32(file, address + 0x1C);
            Flags = (SurfaceFlags)BitConverter.ToInt32(file, 0x20);
        }
    }
}
