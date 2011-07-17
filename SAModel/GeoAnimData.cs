using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class GeoAnimData
    {
        public string Name { get; set; }
        public int Unknown1 { get; set; }
        public float Unknown2 { get; set; }
        public float Unknown3 { get; set; }
        public Object Model { get; set; }
        public Animation Animation { get; set; }
        public int Unknown4 { get; set; }

        public static int Size { get { return 0x18; } }

        public GeoAnimData(byte[] file, int address, uint imageBase, bool DX)
        {
            Name = "anim_" + address.ToString("X8");
            Unknown1 = BitConverter.ToInt32(file, address);
            Unknown2 = BitConverter.ToSingle(file, address + 4);
            Unknown3 = BitConverter.ToSingle(file, address + 8);
            Model = new Object(file, (int)(BitConverter.ToUInt32(file, address + 0xC) - imageBase), imageBase, DX);
            Animation = new Animation(file, (int)(BitConverter.ToUInt32(file, address + 0x10) - imageBase), imageBase, DX);
            Unknown4 = BitConverter.ToInt32(file, address + 0x14);
        }

        public GeoAnimData(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Name = groupname;
            Dictionary<string, string> group = INI[groupname];
            Unknown1 = int.Parse(group["Unknown1"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown2 = float.Parse(group["Unknown2"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown3 = float.Parse(group["Unknown3"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            Model = new Object(INI, group["Model"]);
            Animation = new Animation(group["Animation"]);
            Unknown4 = int.Parse(group["Unknown4"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public byte[] GetBytes(uint imageBase, uint modelptr, uint animptr)
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Unknown1));
            result.AddRange(BitConverter.GetBytes(Unknown2));
            result.AddRange(BitConverter.GetBytes(Unknown3));
            result.AddRange(BitConverter.GetBytes(modelptr));
            result.AddRange(BitConverter.GetBytes(animptr));
            result.AddRange(BitConverter.GetBytes(Unknown4));
            return result.ToArray();
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI, string animpath)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Unknown1", Unknown1.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown2", Unknown2.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown3", Unknown3.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Model", Model.Name);
            Model.Save(INI);
            group.Add("Animation", System.IO.Path.Combine(animpath, Animation.Name + ".xml"));
            group.Add("Unknown4", Unknown4.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            INI.Add(Name, group);
            Animation.Save(group["Animation"]);
        }
    }
}
