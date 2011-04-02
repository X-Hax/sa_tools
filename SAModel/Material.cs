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

        public static int Size { get { return 0x14; } }

        public Material() { }

        public Material(byte[] file, int address)
        {
            DiffuseColor = Color.FromArgb(BitConverter.ToInt32(file, address));
            SpecularColor = Color.FromArgb(BitConverter.ToInt32(file, address + 4));
            Unknown1 = BitConverter.ToSingle(file, address + 8);
            TextureID = BitConverter.ToInt32(file, address + 0xC);
            Unknown2 = file[address + 0x10];
            Unknown3 = file[address + 0x11];
            Flags = file[address + 0x12];
            EndOfMat = file[address + 0x13];
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(DiffuseColor.ToArgb()));
            result.AddRange(BitConverter.GetBytes(SpecularColor.ToArgb()));
            result.AddRange(BitConverter.GetBytes(Unknown1));
            result.AddRange(BitConverter.GetBytes(TextureID));
            result.Add(Unknown2);
            result.Add(Unknown3);
            result.Add(Flags);
            result.Add(EndOfMat);
            return result.ToArray();
        }
    }
}
