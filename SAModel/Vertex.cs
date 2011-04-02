using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static int Size { get { return 12; } }

        public Vertex() { }

        public Vertex(byte[] file, int address)
        {
            X = BitConverter.ToSingle(file, address);
            Y = BitConverter.ToSingle(file, address + 4);
            Z = BitConverter.ToSingle(file, address + 8);
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
