using System;
using System.Text;

namespace SonicRetro.SAModel
{
    public class LandTable
    {
        public short AnimCount { get; set; }
        public COL[] COL { get; set; }
        public uint Anim { get; set; }
        public int Flags { get; set; }
        public float Unknown1 { get; set; }
        public string TextureFileName { get; set; }
        public uint TextureList { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }

        public static int Size { get { return 0x24; } }

        public LandTable(byte[] file, int address, uint imageBase, bool DX)
        {
            COL = new COL[BitConverter.ToInt16(file, address)];
            AnimCount = BitConverter.ToInt16(file, address + 2);
            Flags = BitConverter.ToInt32(file, address + 4);
            Unknown1 = BitConverter.ToSingle(file, address + 8);
            int tmpaddr = BitConverter.ToInt32(file, address + 0xC);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                for (int i = 0; i < COL.Length; i++)
                {
                    COL[i] = new COL(file, tmpaddr, imageBase, DX);
                    tmpaddr += SAModel.COL.Size;
                }
            }
            Anim = BitConverter.ToUInt32(file, address + 0x10);
            tmpaddr = BitConverter.ToInt32(file, address + 0x14);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                StringBuilder sb = new StringBuilder();
                while (file[tmpaddr] != 0)
                {
                    sb.Append((char)file[tmpaddr]);
                }
                TextureFileName = sb.ToString();
            }
            TextureList = BitConverter.ToUInt32(file, address + 0x18);
            Unknown2 = BitConverter.ToInt32(file, address + 0x1C);
            Unknown3 = BitConverter.ToInt32(file, address + 0x20);
        }
    }
}
