using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class COL
    {
        public Vertex Center { get; set; }
        public float Radius { get; set; }
        public ulong Unknown1 { get; private set; }
        public Object Model { get; set; }
        public int Unknown2 { get; set; }
        public int Flags { get; set; }
        public SurfaceFlags SurfaceFlags
        {
            get
            {
                return (SurfaceFlags)Flags;
            }
            set
            {
                Flags = (int)value;
            }
        }
        public string Name { get; set; }

        public static int Size { get { return 0x24; } }

        public COL()
        {
            Name = "col_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Center = new Vertex();
        }

        public COL(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            Name = "col_" + address.ToString("X8");
            Center = new Vertex(file, address);
            Radius = BitConverter.ToSingle(file, address + 0xC);
            Unknown1 = BitConverter.ToUInt64(file, 0x10);
            uint tmpaddr = BitConverter.ToUInt32(file, address + 0x18) - imageBase;
            Model = new Object(file, (int)tmpaddr, imageBase, format);
            Unknown2 = BitConverter.ToInt32(file, address + 0x1C);
            Flags = BitConverter.ToInt32(file, address + 0x20);
        }

        public COL(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Name = groupname;
            Dictionary<string, string> group = INI[groupname];
            Center = new Vertex(group["Center"]);
            Radius = float.Parse(group["Radius"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown1 = ulong.Parse(group["Unknown1"], System.Globalization.NumberStyles.HexNumber);
            Model = new Object(INI, group["Model"]);
            Unknown2 = int.Parse(group["Unknown2"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Flags = int.Parse(group["Flags"], System.Globalization.NumberStyles.HexNumber);
        }

        public byte[] GetBytes(uint imageBase, uint modelptr)
        {
            List<byte> result = new List<byte>();
            result.AddRange(Center.GetBytes());
            result.AddRange(BitConverter.GetBytes(Radius));
            result.AddRange(BitConverter.GetBytes(Unknown1));
            result.AddRange(BitConverter.GetBytes(modelptr));
            result.AddRange(BitConverter.GetBytes(Unknown2));
            result.AddRange(BitConverter.GetBytes(Flags));
            return result.ToArray();
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Center", Center.ToString());
            group.Add("Radius", Radius.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown1", Unknown1.ToString("X16"));
            group.Add("Model", Model.Name);
            Model.Save(INI);
            group.Add("Unknown2", Unknown2.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Flags", Flags.ToString("X8"));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }
    }
}