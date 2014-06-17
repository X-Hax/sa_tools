using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SAModel
{
	[Serializable]
    public class BoundingSphere
    {
        public Vertex Center { get; set; }
        public float Radius { get; set; }

        public BoundingSphere()
        {
            Center = new Vertex();
        }

        public BoundingSphere(byte[] file, int address)
        {
            Center = new Vertex(file, address);
            Radius = ByteConverter.ToSingle(file, address + Vertex.Size);
        }

        public BoundingSphere(string data)
        {
            int i = data.LastIndexOf(',');
            Center = new Vertex(data.Substring(0, i));
            Radius = float.Parse(data.Substring(i + 1), System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public BoundingSphere(Vertex center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public BoundingSphere(float x, float y, float z, float radius)
        {
            Center = new Vertex(x, y, z);
            Radius = radius;
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(Center.GetBytes());
            result.AddRange(ByteConverter.GetBytes(Radius));
            return result.ToArray();
        }

        public override string ToString()
        {
            return Center.ToString() + ", " + Radius.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public string ToStruct()
        {
            System.Text.StringBuilder result = new StringBuilder();
            result.Append(Center.ToStruct());
            result.Append(", ");
            result.Append(Radius.ToC());
            return result.ToString();
        }

        public float[] ToArray()
        {
            float[] result = new float[4];
            result[0] = Center.X;
            result[1] = Center.Y;
            result[2] = Center.Z;
            result[3] = Radius;
            return result;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Center.X;
                    case 1:
                        return Center.Y;
                    case 2:
                        return Center.Z;
                    case 3:
                        return Radius;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Center.X = value;
                        return;
                    case 1:
                        Center.Y = value;
                        return;
                    case 2:
                        Center.Z = value;
                        return;
                    case 3:
                        Radius = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }
}