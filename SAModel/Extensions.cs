using System.Collections.Generic;
using System.Text;

namespace SonicRetro.SAModel
{
    public static class Extensions
    {
        public static void Align(this List<byte> me, int alignment)
        {
            int off = me.Count % alignment;
            if (off == 0) return;
            me.AddRange(new byte[alignment - off]);
        }

        public static string GetCString(this byte[] file, int address)
        {
            return GetCString(file, address, Encoding.UTF8);
        }

        public static string GetCString(this byte[] file, int address, Encoding encoding)
        {
            int count = 0;
            while (file[address + count] != 0)
                count++;
            return encoding.GetString(file, address, count);
        }
    }
}