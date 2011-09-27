using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class Material
    {
        public Color DiffuseColor { get; set; }
        public Color SpecularColor { get; set; }
        public float Unknown1 { get; set; }
        public int TextureID { get; set; }
        public byte Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public byte Flags { get; set; }
        public byte EndOfMat { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 0x14; } }

        public Material()
        {
            Name = "material_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            DiffuseColor = Color.FromArgb(0xFF, 0xB2, 0xB2, 0xB2);
            SpecularColor = Color.Transparent;
        }

        public Material(byte[] file, int address)
        {
            DiffuseColor = Color.FromArgb(ByteConverter.ToInt32(file, address));
            SpecularColor = Color.FromArgb(ByteConverter.ToInt32(file, address + 4));
            Unknown1 = ByteConverter.ToSingle(file, address + 8);
            TextureID = ByteConverter.ToInt32(file, address + 0xC);
            Unknown2 = file[address + 0x10];
            Unknown3 = file[address + 0x11];
            Flags = file[address + 0x12];
            EndOfMat = file[address + 0x13];
            Name = "material_" + address.ToString("X8");
        }

        public Material(Dictionary<string, string> group, string name)
        {
            Name = name;
            DiffuseColor = Color.FromArgb(int.Parse(group["Diffuse"], System.Globalization.NumberStyles.HexNumber));
            SpecularColor = Color.FromArgb(int.Parse(group["Specular"], System.Globalization.NumberStyles.HexNumber));
            Unknown1 = float.Parse(group["Unknown1"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            TextureID = int.Parse(group["Texture"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown2 = byte.Parse(group["Unknown2"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown3 = byte.Parse(group["Unknown3"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Flags = byte.Parse(group["Flags"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            EndOfMat = byte.Parse(group["EndOfMat"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(ByteConverter.GetBytes(DiffuseColor.ToArgb()));
            result.AddRange(ByteConverter.GetBytes(SpecularColor.ToArgb()));
            result.AddRange(ByteConverter.GetBytes(Unknown1));
            result.AddRange(ByteConverter.GetBytes(TextureID));
            result.Add(Unknown2);
            result.Add(Unknown3);
            result.Add(Flags);
            result.Add(EndOfMat);
            return result.ToArray();
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Diffuse", DiffuseColor.ToArgb().ToString("X8"));
            group.Add("Specular", SpecularColor.ToArgb().ToString("X8"));
            group.Add("Unknown1", Unknown1.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Texture", TextureID.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown2", Unknown2.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown3", Unknown3.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Flags", Flags.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("EndOfMat", EndOfMat.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }
    }
}