using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class Rotation
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public static int Size { get { return 12; } }

        public Rotation() { }

        public Rotation(byte[] file, int address)
        {
            X = BitConverter.ToInt32(file, address);
            Y = BitConverter.ToInt32(file, address + 4);
            Z = BitConverter.ToInt32(file, address + 8);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(X));
            result.AddRange(BitConverter.GetBytes(Y));
            result.AddRange(BitConverter.GetBytes(Z));
            return result.ToArray();
        }
    }
}
