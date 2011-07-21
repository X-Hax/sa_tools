using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class PolyNormal
    {
        public float Unknown1 { get; set; }
        public float Unknown2 { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 8; } }

        public PolyNormal()
        {
            Name = "pnorm_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
        }

        public PolyNormal(byte[] file, int address)
        {
            Name = "pnorm_" + address.ToString("X8");
            Unknown1 = BitConverter.ToSingle(file, address);
            Unknown2 = BitConverter.ToSingle(file, address + 4);
        }

        public PolyNormal(Dictionary<string, string> group, string name)
        {
            Name = name;
            Unknown1 = float.Parse(group["Unknown1"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown2 = float.Parse(group["Unknown2"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Unknown1));
            result.AddRange(BitConverter.GetBytes(Unknown2));
            return result.ToArray();
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Unknown1", Unknown1.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown2", Unknown2.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }
    }
}