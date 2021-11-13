using System;
using System.Collections.Generic;

namespace VrSharp
{
    public static class PTMethods
    {
        /// <summary>
        /// Checks to see if the array contains the values stored in compareTo at the specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Array to check</param>
        /// <param name="sourceIndex">Position in the array to check.</param>
        /// <param name="compareTo">The array to compare to.</param>
        /// <returns>True if the values in compareTo are in the array at the specified index.</returns>
        public static bool Contains<T>(T[] source, int sourceIndex, T[] compareTo)
        {
            if (source == null || compareTo == null)
                return false;

            if (sourceIndex < 0 || sourceIndex + compareTo.Length > source.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (!comparer.Equals(source[sourceIndex + i], compareTo[i])) return false;
            }

            return true;
        }

        public static int RoundUp(int value, int roundUpTo)
        {
            // Return the same number if it is already a multiple
            if (value % roundUpTo == 0)
                return value;

            return value + (roundUpTo - (value % roundUpTo));
        }

        public static ushort ToUInt16BE(byte[] value, int startIndex)
        {
            return (ushort)(value[startIndex] << 8 | value[startIndex + 1]);
        }

        public static uint ToUInt32BE(byte[] value, int startIndex)
        {
            return (uint)(value[startIndex] << 24 | value[startIndex + 1] << 16 | value[startIndex + 2] << 8 | value[startIndex + 3]);
        }

        public static byte[] GetBytesBE(ushort value)
        {
            return new byte[] { (byte)(value >> 8), (byte)(value & 0xFF) };
        }
    }
}