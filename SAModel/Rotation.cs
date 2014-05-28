using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SAModel
{
    [TypeConverter(typeof(RotationConverter))]
    public class Rotation
    {
        [Browsable(false)]
        public int X { get; set; }
        [Browsable(false)]
        public int Y { get; set; }
        [Browsable(false)]
        public int Z { get; set; }

        [DisplayName("X")]
        public float XDeg { get { return BAMSToDeg(X); } set { X = DegToBAMS(value); } }
        [DisplayName("Y")]
        public float YDeg { get { return BAMSToDeg(Y); } set { Y = DegToBAMS(value); } }
        [DisplayName("Z")]
        public float ZDeg { get { return BAMSToDeg(Z); } set { Z = DegToBAMS(value); } }

        [Browsable(false)]
        public static int Size { get { return 12; } }

        public Rotation() { }

        public Rotation(byte[] file, int address)
        {
            X = ByteConverter.ToInt32(file, address);
            Y = ByteConverter.ToInt32(file, address + 4);
            Z = ByteConverter.ToInt32(file, address + 8);
        }

        public Rotation(string data)
        {
            string[] a = data.Split(',');
            X = DegToBAMS(float.Parse(a[0], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
            Y = DegToBAMS(float.Parse(a[1], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
            Z = DegToBAMS(float.Parse(a[2], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo));
        }

        public Rotation(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(ByteConverter.GetBytes(X));
            result.AddRange(ByteConverter.GetBytes(Y));
            result.AddRange(ByteConverter.GetBytes(Z));
            return result.ToArray();
        }

        public override string ToString()
        {
            return BAMSToDeg(X).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ", " + BAMSToDeg(Y).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ", " + BAMSToDeg(Z).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
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

        public static float BAMSToDeg(int BAMS)
        {
            return (float)(BAMS / (65536 / 360.0));
        }

        public static int DegToBAMS(float deg)
        {
            return (int)(deg * (65536 / 360.0));
        }

		public bool IsEmpty { get { return X == 0 && Y == 0 && Z == 0; } }
	}

    public class RotationConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Rotation))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Rotation)
                return ((Rotation)value).ToString();
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
                return new Rotation((string)value);
            return base.ConvertFrom(context, culture, value);
        }
    }
}