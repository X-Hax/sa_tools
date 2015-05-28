using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    [TypeConverter(typeof(EditableRotationConverter))]
    public class EditableRotation
    {
        [Browsable(false)]
        public int X { get; set; }
        [Browsable(false)]
        public int Y { get; set; }
        [Browsable(false)]
        public int Z { get; set; }

        [DisplayName("X")]
        public float XDeg { get { return Rotation.BAMSToDeg(X); } set { X = Rotation.DegToBAMS(value); } }
        [DisplayName("Y")]
        public float YDeg { get { return Rotation.BAMSToDeg(Y); } set { Y = Rotation.DegToBAMS(value); } }
        [DisplayName("Z")]
        public float ZDeg { get { return Rotation.BAMSToDeg(Z); } set { Z = Rotation.DegToBAMS(value); } }

        public EditableRotation() { }

        public EditableRotation(byte[] file, int address)
        {
            X = BitConverter.ToInt32(file, address);
            Y = BitConverter.ToInt32(file, address + 4);
            Z = BitConverter.ToInt32(file, address + 8);
        }

        public EditableRotation(Rotation data)
        {
            X = data.X;
            Y = data.Y;
            Z = data.Z;
        }

        public EditableRotation(string data)
        {
            string[] a = data.Split(',');
            X = Rotation.DegToBAMS(float.Parse(a[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo));
            Y = Rotation.DegToBAMS(float.Parse(a[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo));
            Z = Rotation.DegToBAMS(float.Parse(a[2], NumberStyles.Float, NumberFormatInfo.InvariantInfo));
        }

        public EditableRotation(int x, int y, int z)
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
            return Rotation.BAMSToDeg(X).ToString(NumberFormatInfo.InvariantInfo) + ", " + Rotation.BAMSToDeg(Y).ToString(NumberFormatInfo.InvariantInfo) + ", " + Rotation.BAMSToDeg(Z).ToString(NumberFormatInfo.InvariantInfo);
        }

        public int[] ToArray()
        {
            int[] result = new int[3];
            result[0] = X;
            result[1] = Y;
            result[2] = Z;
            return result;
        }

        public Rotation ToRotation()
        {
            return new Rotation(X, Y, Z);
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

    public class EditableRotationConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(EditableRotation))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is EditableRotation)
                return ((EditableRotation)value).ToString();
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
                return new EditableRotation(new Rotation((string)value));
            return base.ConvertFrom(context, culture, value);
        }
    }
}
