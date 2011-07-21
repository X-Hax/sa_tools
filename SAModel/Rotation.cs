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

        public Rotation(string data)
        {
            string[] a = data.Split(',');
            X = int.Parse(a[0], System.Globalization.NumberStyles.HexNumber);
            Y = int.Parse(a[1], System.Globalization.NumberStyles.HexNumber);
            Z = int.Parse(a[2], System.Globalization.NumberStyles.HexNumber);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(X));
            result.AddRange(BitConverter.GetBytes(Y));
            result.AddRange(BitConverter.GetBytes(Z));
            return result.ToArray();
        }

        public override string ToString()
        {
            return X.ToString("X8") + ", " + Y.ToString("X8") + ", " + Z.ToString("X8");
        }

        public int[] ToArray()
        {
            int[] result = new int[3];
            result[0] = X;
            result[1] = Y;
            result[2] = Z;
            return result;
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        return;
                    case 1:
                        Y = value;
                        return;
                    case 2:
                        Z = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }
}
