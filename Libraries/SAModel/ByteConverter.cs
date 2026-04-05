using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SAModel
{
    [DebuggerNonUserCode]
    public static class ByteConverter
    {
        public static bool BigEndian { get; set; }
        public static bool Reverse { get; set; }

		private static bool Locked;

        private static readonly List<bool> bigEndianStack = [];

		public static void SetBigEndian(bool value)
		{
			BackupBigEndian();
			BigEndian = value;
		}

        public static void BackupBigEndian()
        {
			// Because ByteConverter is a static class, loading multiple models or motions that require different Endianness in parallel can break it.
			// Below is probably one of the dumbest hacks but it's meant to prevent errors during multi-threaded operations, such as loading models in object definitions.
			// Whenever the Big Endian setting is modified via this method, the Locked variable is set to true until Endianness is reset.
			// If another attempt is made to set the Big Endian value while Locked is still true, it will wait until Locked is false.
			// The real solution would be to replace the way we deal with Endianness and/or read and write binary data.
			if (Locked)
			{
				while (Locked)
					System.Threading.Thread.Sleep(1);
			}
			Locked = true;
			bigEndianStack.Add(BigEndian);
        }

        public static void RestoreBigEndian()
        {
			Locked = false;
            if (bigEndianStack.Count == 0)
                return;
            BigEndian = bigEndianStack[bigEndianStack.Count - 1];
            bigEndianStack.RemoveAt(bigEndianStack.Count - 1);
        }

        public static void SwapEndianArray(byte[] array, int unitSize)
        {
            int numItems = array.Length / unitSize;
            if (numItems * unitSize != array.Length)
                throw new Exception(string.Format("ByteConverter: unit size of {0} cannot be used in an array with the length of {1}",
                    unitSize, array.Length));
            for (int i = 0; i < numItems; i++)
            {
                Span<byte> item = array.AsSpan().Slice(i * unitSize, unitSize);
                item.Reverse();
            }
        }

        #region Endianness based on BigEndian value
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
            byte[] y = new byte[2];
            Array.Copy(value, startIndex, y, 0, 2);
            SwapEndian(y);
            return BitConverter.ToUInt16(y, 0);
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            byte[] y = new byte[2];
            Array.Copy(value, startIndex, y, 0, 2);
            SwapEndian(y);
            return BitConverter.ToInt16(y, 0);
        }

        public static uint ToUInt32(byte[] value, int startIndex)
        {
            byte[] y = new byte[4];
            Array.Copy(value, startIndex, y, 0, 4);
            SwapEndian(y);
            return BitConverter.ToUInt32(y, 0);
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            byte[] y = new byte[4];
            Array.Copy(value, startIndex, y, 0, 4);
            SwapEndian(y);
            return BitConverter.ToInt32(y, 0);
        }

        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            byte[] y = new byte[8];
            Array.Copy(value, startIndex, y, 0, 8);
            SwapEndian(y);
            return BitConverter.ToUInt64(y, 0);
        }

        public static long ToInt64(byte[] value, int startIndex)
        {
            byte[] y = new byte[8];
            Array.Copy(value, startIndex, y, 0, 8);
            SwapEndian(y);
            return BitConverter.ToInt64(y, 0);
        }

        public static float ToSingle(byte[] value, int startIndex)
        {
            byte[] y = new byte[4];
            Array.Copy(value, startIndex, y, 0, 4);
            SwapEndian(y);
            float res = BitConverter.ToSingle(y, 0);
            if (res == -0.0f) res = 0.0f;
            return res;
        }

        public static double ToDouble(byte[] value, int startIndex)
        {
            byte[] y = new byte[8];
            Array.Copy(value, startIndex, y, 0, 8);
            SwapEndian(y);
            return BitConverter.ToDouble(y, 0);
        }

        private static void SwapEndian(byte[] value)
        {
            if (!BigEndian & !BitConverter.IsLittleEndian)
                Array.Reverse(value);
            if (BigEndian & BitConverter.IsLittleEndian)
                Array.Reverse(value);
        }
        #endregion

        #region Big Endian only

        public static byte[] GetBytesBE(ushort value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(short value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(uint value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(int value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(ulong value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(long value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(float value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static byte[] GetBytesBE(double value)
        {
            byte[] y = BitConverter.GetBytes(value);
            Array.Reverse(y);
            return y;
        }

        public static ushort ToUInt16BE(byte[] value, int startIndex)
        {
            byte[] y = new byte[2];
            Array.Copy(value, startIndex, y, 0, 2);
            Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }

        public static short ToInt16BE(byte[] value, int startIndex)
        {
            byte[] y = new byte[2];
            Array.Copy(value, startIndex, y, 0, 2);
            Array.Reverse(y);
            return BitConverter.ToInt16(y, 0);
        }

        public static uint ToUInt32BE(byte[] value, int startIndex)
        {
            byte[] y = new byte[4];
            Array.Copy(value, startIndex, y, 0, 4);
            Array.Reverse(y);
            return BitConverter.ToUInt32(y, 0);
        }

        public static int ToInt32BE(byte[] value, int startIndex)
        {
            byte[] y = new byte[4];
            Array.Copy(value, startIndex, y, 0, 4);
            Array.Reverse(y);
            return BitConverter.ToInt32(y, 0);
        }

        public static ulong ToUInt64BE(byte[] value, int startIndex)
        {
            byte[] y = new byte[8];
            Array.Copy(value, startIndex, y, 0, 8);
            Array.Reverse(y);
            return BitConverter.ToUInt64(y, 0);
        }

        public static long ToInt64BE(byte[] value, int startIndex)
        {
            byte[] y = new byte[8];
            Array.Copy(value, startIndex, y, 0, 8);
            Array.Reverse(y);
            return BitConverter.ToInt64(y, 0);
        }

        public static float ToSingleBE(byte[] value, int startIndex)
        {
            byte[] y = new byte[4];
            Array.Copy(value, startIndex, y, 0, 4);
            Array.Reverse(y);
            float res = BitConverter.ToSingle(y, 0);
            if (res == -0.0f) res = 0.0f;
            return res;
        }

        public static double ToDoubleBE(byte[] value, int startIndex)
        {
            byte[] y = new byte[8];
            Array.Copy(value, startIndex, y, 0, 8);
            Array.Reverse(y);
            return BitConverter.ToDouble(y, 0);
        }
        #endregion
    }
}