using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class UV
    {
        public short U { get; set; }
        public short V { get; set; }

        public static int Size { get { return 4; } }

        public UV() { }

        public UV(byte[] file, int address)
        {
            U = BitConverter.ToInt16(file, address);
            V = BitConverter.ToInt16(file, address + 2);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(U));
            result.AddRange(BitConverter.GetBytes(V));
            return result.ToArray();
        }
    }
}
