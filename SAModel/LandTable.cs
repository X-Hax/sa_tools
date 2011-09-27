using System;
using System.Collections.Generic;
using System.Text;

namespace SonicRetro.SAModel
{
    public class LandTable
    {
        public List<COL> COL { get; set; }
        public List<GeoAnimData> Anim { get; set; }
        public int Flags { get; set; }
        public float Unknown1 { get; set; }
        public string TextureFileName { get; set; }
        public uint TextureList { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 0x24; } }

        public LandTable(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Name = "landtable_" + address.ToString("X8");
            short colcnt = ByteConverter.ToInt16(file, address);
            short anicnt = ByteConverter.ToInt16(file, address + 2);
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    Flags = ByteConverter.ToInt32(file, address + 4);
                    Unknown1 = ByteConverter.ToSingle(file, address + 8);
                    COL = new List<COL>();
                    uint tmpaddr = ByteConverter.ToUInt32(file, address + 0xC);
                    if (tmpaddr != 0)
                    {
                        tmpaddr -= imageBase;
                        for (int i = 0; i < colcnt; i++)
                        {
                            COL.Add(new COL(file, (int)tmpaddr, imageBase, format));
                            tmpaddr += (uint)SAModel.COL.Size(format);
                        }
                    }
                    Anim = new List<GeoAnimData>();
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x10);
                    if (tmpaddr != 0)
                    {
                        tmpaddr -= imageBase;
                        for (int i = 0; i < anicnt; i++)
                        {
                            Anim.Add(new GeoAnimData(file, (int)tmpaddr, imageBase, format));
                            tmpaddr += (uint)GeoAnimData.Size;
                        }
                    }
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x14);
                    if (tmpaddr != 0)
                    {
                        tmpaddr -= imageBase;
                        StringBuilder sb = new StringBuilder();
                        while (file[tmpaddr] != 0)
                            sb.Append((char)file[tmpaddr++]);
                        TextureFileName = sb.ToString();
                    }
                    TextureList = ByteConverter.ToUInt32(file, address + 0x18);
                    Unknown2 = ByteConverter.ToInt32(file, address + 0x1C);
                    Unknown3 = ByteConverter.ToInt32(file, address + 0x20);
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    Unknown1 = ByteConverter.ToSingle(file, address + 0xC);
                    COL = new List<COL>();
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x10);
                    if (tmpaddr != 0)
                    {
                        tmpaddr -= imageBase;
                        for (int i = 0; i < colcnt; i++)
                        {
                            COL.Add(new COL(file, (int)tmpaddr, imageBase, format));
                            tmpaddr += (uint)SAModel.COL.Size(format);
                        }
                    }
                    Anim = new List<GeoAnimData>();
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x14);
                    if (tmpaddr != 0)
                    {
                        tmpaddr -= imageBase;
                        for (int i = 0; i < anicnt; i++)
                        {
                            Anim.Add(new GeoAnimData(file, (int)tmpaddr, imageBase, format));
                            tmpaddr += (uint)GeoAnimData.Size;
                        }
                    }
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x18);
                    if (tmpaddr != 0)
                    {
                        tmpaddr -= imageBase;
                        StringBuilder sb = new StringBuilder();
                        while (file[tmpaddr] != 0)
                            sb.Append((char)file[tmpaddr++]);
                        TextureFileName = sb.ToString();
                    }
                    TextureList = ByteConverter.ToUInt32(file, address + 0x1C);
                    break;
            }
        }

        public LandTable(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Name = groupname;
            Dictionary<string, string> group = INI[groupname];
            COL = new List<COL>();
            if (group.ContainsKey("COL"))
            {
                string[] cols = group["COL"].Split(',');
                foreach (string item in cols)
                    COL.Add(new COL(INI, item));
            }
            Anim = new List<GeoAnimData>();
            if (group.ContainsKey("Anim"))
            {
                string[] cols = group["Anim"].Split(',');
                foreach (string item in cols)
                    Anim.Add(new GeoAnimData(INI, item));
            }
            Flags = int.Parse(group["Flags"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown1 = float.Parse(group["Unknown1"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
            if (group.ContainsKey("TextureFileName"))
                TextureFileName = group["TextureFileName"];
            TextureList = uint.Parse(group["TextureList"], System.Globalization.NumberStyles.HexNumber);
            Unknown2 = int.Parse(group["Unknown2"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Unknown3 = int.Parse(group["Unknown3"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public static LandTable LoadFromFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C564C314153u)
                return new LandTable(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA1);
            else if (magic == 0x00004C564C324153u)
                return new LandTable(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA2);
            else
                throw new FormatException("Not a valid SA1LVL/SA2LVL file.");
        }

        public static bool CheckLevelFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C564C314153u)
                return true;
            else if (magic == 0x00004C564C324153u)
                return true;
            else
                return false;
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, out uint address)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Dictionary<string, uint> attachaddrs = new Dictionary<string, uint>();
            List<byte> result = new List<byte>();
            byte[] tmpbyte;
            uint[] colmdladdrs = new uint[COL.Count];
            uint tmpaddr = 0;
            for (int i = 0; i < COL.Count; i++)
            {
                result.Align(4);
                tmpbyte = COL[i].Model.GetBytes(imageBase + (uint)result.Count, format, attachaddrs, out tmpaddr);
                colmdladdrs[i] = tmpaddr + (uint)result.Count + imageBase;
                result.AddRange(tmpbyte);
            }
            uint[] animmdladdrs = new uint[Anim.Count];
            uint[] animaniaddrs = new uint[Anim.Count];
            for (int i = 0; i < Anim.Count; i++)
            {
                result.Align(4);
                tmpbyte = Anim[i].Model.GetBytes(imageBase + (uint)result.Count, format, out tmpaddr);
                animmdladdrs[i] = tmpaddr + (uint)result.Count + imageBase;
                result.AddRange(tmpbyte);
                result.Align(4);
                tmpbyte = Anim[i].Animation.GetBytes(imageBase + (uint)result.Count, animmdladdrs[i], Anim[i].Model.GetObjects().Length, out tmpaddr);
                animaniaddrs[i] = tmpaddr + (uint)result.Count + imageBase;
                result.AddRange(tmpbyte);
            }
            uint coladdr = imageBase + (uint)result.Count;
            for (int i = 0; i < COL.Count; i++)
            {
                result.Align(4);
                result.AddRange(COL[i].GetBytes(imageBase + (uint)result.Count, colmdladdrs[i], format));
            }
            uint animaddr = imageBase + (uint)result.Count;
            for (int i = 0; i < Anim.Count; i++)
            {
                result.Align(4);
                result.AddRange(Anim[i].GetBytes(imageBase + (uint)result.Count, animmdladdrs[i], animaniaddrs[i]));
            }
            if (Anim.Count == 0)
                animaddr = 0;
            result.Align(4);
            uint texnameaddr = 0;
            if (TextureFileName != null)
            {
                texnameaddr = imageBase + (uint)result.Count;
                result.AddRange(System.Text.Encoding.ASCII.GetBytes(TextureFileName));
                result.Add(0);
            }
            result.Align(4);
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes((ushort)COL.Count));
            result.AddRange(ByteConverter.GetBytes((ushort)Anim.Count));
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    result.AddRange(ByteConverter.GetBytes(Flags));
                    result.AddRange(ByteConverter.GetBytes(Unknown1));
                    result.AddRange(ByteConverter.GetBytes(coladdr));
                    result.AddRange(ByteConverter.GetBytes(animaddr));
                    result.AddRange(ByteConverter.GetBytes(texnameaddr));
                    result.AddRange(ByteConverter.GetBytes(TextureList));
                    result.AddRange(ByteConverter.GetBytes(Unknown2));
                    result.AddRange(ByteConverter.GetBytes(Unknown3));
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    result.AddRange(new byte[8]); // TODO: figure out what these do
                    result.AddRange(ByteConverter.GetBytes(Unknown1));
                    result.AddRange(ByteConverter.GetBytes(coladdr));
                    result.AddRange(ByteConverter.GetBytes(animaddr));
                    result.AddRange(ByteConverter.GetBytes(texnameaddr));
                    result.AddRange(ByteConverter.GetBytes(TextureList));
                    break;
            }
            return result.ToArray();
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI, string animpath)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            if (COL.Count > 0)
            {
                List<string> cols = new List<string>();
                foreach (COL item in COL)
                {
                    item.Save(INI);
                    cols.Add(item.Name);
                }
                group.Add("COL", string.Join(",", cols.ToArray()));
            }
            if (Anim.Count > 0)
            {
                List<string> cols = new List<string>();
                foreach (GeoAnimData item in Anim)
                {
                    item.Save(INI, animpath);
                    cols.Add(item.Name);
                }
                group.Add("Anim", string.Join(",", cols.ToArray()));
            }
            group.Add("Flags", Flags.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown1", Unknown1.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            if (!string.IsNullOrEmpty(TextureFileName))
                group.Add("TextureFileName", TextureFileName);
            group.Add("TextureList", TextureList.ToString("X8"));
            group.Add("Unknown2", Unknown2.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Unknown3", Unknown3.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }

        public void SaveToFile(string filename, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            switch (format)
            {
                case ModelFormat.SA1:
                    file.AddRange(ByteConverter.GetBytes(0x00004C564C314153u));
                    uint addr = 0;
                    byte[] lvl = GetBytes(0x10, ModelFormat.SA1, out addr);
                    file.AddRange(ByteConverter.GetBytes(addr + 0x10));
                    file.Align(0x10);
                    file.AddRange(lvl);
                    break;
                case ModelFormat.SADX:
                    throw new ArgumentException("Cannot save SADX format levels to file!", "format");
                case ModelFormat.SA2:
                    file.AddRange(ByteConverter.GetBytes(0x00004C564C324153u));
                    addr = 0;
                    lvl = GetBytes(0x10, ModelFormat.SA2, out addr);
                    file.AddRange(ByteConverter.GetBytes(addr + 0x10));
                    file.Align(0x10);
                    file.AddRange(lvl);
                    break;
                case ModelFormat.SA2B:
                    throw new ArgumentException("Cannot save SA2B format levels to file!", "format");
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }
    }
}