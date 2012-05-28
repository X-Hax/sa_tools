using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SADXPCTools
{
    [Serializable]
    [TypeConverter(typeof(VertexConverter))]
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

        public Vertex(string data)
        {
            string[] a = data.Split(',');
            X = float.Parse(a[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            Y = float.Parse(a[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
            Z = float.Parse(a[2], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
        }

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vertex(float[] data)
        {
            X = data[0];
            Y = data[1];
            Z = data[2];
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
            return X.ToString(NumberFormatInfo.InvariantInfo) + ", " + Y.ToString(NumberFormatInfo.InvariantInfo) + ", " + Z.ToString(NumberFormatInfo.InvariantInfo);
        }

        public float[] ToArray()
        {
            float[] result = new float[3];
            result[0] = X;
            result[1] = Y;
            result[2] = Z;
            return result;
        }

        public float this[int index]
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

    public class VertexConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Vertex))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Vertex)
                return ((Vertex)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return new Vertex((string)value);
            return base.ConvertFrom(context, culture, value);
        }
    }
}