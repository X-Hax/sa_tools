using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

using Microsoft.DirectX;

using SonicRetro.SAModel;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    [TypeConverter(typeof(EditableVertexConverter))]
    public class EditableVertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public EditableVertex() { }

        public EditableVertex(byte[] file, int address)
        {
            X = BitConverter.ToSingle(file, address);
            Y = BitConverter.ToSingle(file, address + 4);
            Z = BitConverter.ToSingle(file, address + 8);
        }

        public EditableVertex(string data)
        {
            string[] a = data.Split(',');
            X = float.Parse(a[0], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            Y = float.Parse(a[1], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            Z = float.Parse(a[2], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public EditableVertex(Vertex data)
        {
            X = data.X;
            Y = data.Y;
            Z = data.Z;
        }

        public EditableVertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
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
            return X.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ", " + Y.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ", " + Z.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public float[] ToArray()
        {
            float[] result = new float[3];
            result[0] = X;
            result[1] = Y;
            result[2] = Z;
            return result;
        }

        public Vertex ToVertex()
        {
            return new Vertex(X, Y, Z);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
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

    public class EditableVertexConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(EditableVertex))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is EditableVertex)
                return ((EditableVertex)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return new EditableVertex(new Vertex((string)value));
            return base.ConvertFrom(context, culture, value);
        }
    }
}
