using System;
using System.Diagnostics;

namespace SA_Tools
{
    [DebuggerNonUserCode]
    public static class ByteConverter
    {
        public static bool BigEndian { get; set; }

        public static byte[] GetBytes(ushort value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(short value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(uint value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(int value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(ulong value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(long value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(float value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static byte[] GetBytes(double value)
        {
            byte[] y = BitConverter.GetBytes(value);
            SwapEndian(y);
            return y;
        }

        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(ushort)];
            Array.Copy(value, startIndex, y, 0, sizeof(ushort));
            SwapEndian(y);
            return BitConverter.ToUInt16(y, 0);
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(short)];
            Array.Copy(value, startIndex, y, 0, sizeof(short));
            SwapEndian(y);
            return BitConverter.ToInt16(y, 0);
        }

        public static uint ToUInt32(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(uint)];
            Array.Copy(value, startIndex, y, 0, sizeof(uint));
            SwapEndian(y);
            return BitConverter.ToUInt32(y, 0);
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(int)];
            Array.Copy(value, startIndex, y, 0, sizeof(int));
            SwapEndian(y);
            return BitConverter.ToInt32(y, 0);
        }

        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(ulong)];
            Array.Copy(value, startIndex, y, 0, sizeof(ulong));
            SwapEndian(y);
            return BitConverter.ToUInt64(y, 0);
        }

        public static long ToInt64(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(long)];
            Array.Copy(value, startIndex, y, 0, sizeof(long));
            SwapEndian(y);
            return BitConverter.ToInt64(y, 0);
        }

        public static float ToSingle(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(float)];
            Array.Copy(value, startIndex, y, 0, sizeof(float));
            SwapEndian(y);
            return BitConverter.ToSingle(y, 0);
        }

        public static double ToDouble(byte[] value, int startIndex)
        {
            byte[] y = new byte[sizeof(double)];
            Array.Copy(value, startIndex, y, 0, sizeof(double));
            SwapEndian(y);
            return BitConverter.ToDouble(y, 0);
        }

        internal static void SwapEndian(byte[] value)
        {
            if (BigEndian == BitConverter.IsLittleEndian)
                Array.Reverse(value);
        }
    }
}