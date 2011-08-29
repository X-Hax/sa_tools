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
            int off = me.Count % alignment;
            if (off == 0) return;
            me.AddRange(new byte[alignment - off]);
        }
    }
}
