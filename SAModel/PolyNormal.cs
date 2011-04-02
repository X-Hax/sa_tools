using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class PolyNormal
    {
        public float Unknown1 { get; set; }
        public float Unknown2 { get; set; }

        public static int Size { get { return 8; } }

        public PolyNormal() { }

        public PolyNormal(byte[] file, int address)
        {
            Unknown1 = BitConverter.ToSingle(file, address);
            Unknown2 = BitConverter.ToSingle(file, address + 4);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Unknown1));
            result.AddRange(BitConverter.GetBytes(Unknown2));
            return result.ToArray();
        }
    }
}
