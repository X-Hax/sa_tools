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
            U = ByteConverter.ToInt16(file, address);
            V = ByteConverter.ToInt16(file, address + 2);
        }

        public UV(string data)
        {
            string[] uv = data.Split(',');
            U = short.Parse(uv[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            V = short.Parse(uv[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(ByteConverter.GetBytes(U));
            result.AddRange(ByteConverter.GetBytes(V));
            return result.ToArray();
        }

        public override string ToString()
        {
            return U.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "," + V.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }
    }
}
