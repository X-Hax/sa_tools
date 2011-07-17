using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SAModel
{
    public static class Extensions
    {
        public static void Align(this List<byte> me, int alignment)
        {
            me.AddRange(new byte[me.Count % alignment]);
        }
    }
}
