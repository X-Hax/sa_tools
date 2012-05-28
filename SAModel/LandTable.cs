using System;
using System.Collections.Generic;
using System.Text;

namespace SonicRetro.SAModel
{
    public class LandTable
    {
        public const ulong SA1LVL = 0x4C564C314153u;
        public const ulong SA2LVL = 0x4C564C324153u;
        public const ulong FormatMask = 0xFFFFFFFFFFFFu;
        public const ulong CurrentVersion = 1;
        public const ulong SA1LVLVer = SA1LVL | (CurrentVersion << 56);
        public const ulong SA2LVLVer = SA2LVL | (CurrentVersion << 56);

        public List<COL> COL { get; set; }
        public string COLName { get; set; }
        public List<GeoAnimData> Anim { get; set; }
        public string AnimName { get; set; }
        public int Flags { get; set; }
        public float Unknown1 { get; set; }
        public string TextureFileName { get; set; }
        public uint TextureList { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 0x24; } }

        public LandTable(byte[] file, int address, uint imageBase, ModelFormat format)
            : this(file, address, imageBase, format, new Dictionary<int, string>()) { }

        public LandTable(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
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
                    int tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        if (labels.ContainsKey(tmpaddr))
                            COLName = labels[tmpaddr];
                        else
                            COLName = "collist_" + tmpaddr.ToString("X8");
                        for (int i = 0; i < colcnt; i++)
                        {
                            COL.Add(new COL(file, tmpaddr, imageBase, format, labels));
                            tmpaddr += SAModel.COL.Size(format);
                        }
                    }
                    else
                        COLName = "collist_" + Object.GenerateIdentifier();
                    Anim = new List<GeoAnimData>();
                    tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        if (labels.ContainsKey(tmpaddr))
                            AnimName = labels[tmpaddr];
                        else
                            AnimName = "animlist_" + tmpaddr.ToString("X8");
                        for (int i = 0; i < anicnt; i++)
                        {
                            Anim.Add(new GeoAnimData(file, tmpaddr, imageBase, format, labels));
                            tmpaddr += GeoAnimData.Size;
                        }
                    }
                    else
                        AnimName = "animlist_" + Object.GenerateIdentifier();
                    tmpaddr = ByteConverter.ToInt32(file, address + 0x14);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        TextureFileName = file.GetCString((int)tmpaddr, Encoding.ASCII);
                    }
                    TextureList = ByteConverter.ToUInt32(file, address + 0x18);
                    Unknown2 = ByteConverter.ToInt32(file, address + 0x1C);
                    Unknown3 = ByteConverter.ToInt32(file, address + 0x20);
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    Unknown1 = ByteConverter.ToSingle(file, address + 0xC);
                    COL = new List<COL>();
                    tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        if (labels.ContainsKey(tmpaddr))
                            COLName = labels[tmpaddr];
                        else
                            COLName = "collist_" + tmpaddr.ToString("X8");
                        for (int i = 0; i < colcnt; i++)
                        {
                            COL.Add(new COL(file, tmpaddr, imageBase, format, labels));
                            tmpaddr += SAModel.COL.Size(format);
                        }
                    }
                    else
                        COLName = "collist_" + Object.GenerateIdentifier();
                    Anim = new List<GeoAnimData>();
                    tmpaddr = ByteConverter.ToInt32(file, address + 0x14);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        if (labels.ContainsKey(tmpaddr))
                            AnimName = labels[tmpaddr];
                        else
                            AnimName = "animlist_" + tmpaddr.ToString("X8");
                        for (int i = 0; i < anicnt; i++)
                        {
                            Anim.Add(new GeoAnimData(file, tmpaddr, imageBase, format, labels));
                            tmpaddr += GeoAnimData.Size;
                        }
                    }
                    else
                        AnimName = "animlist_" + Object.GenerateIdentifier();
                    tmpaddr = ByteConverter.ToInt32(file, address + 0x18);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        TextureFileName = file.GetCString((int)tmpaddr, Encoding.ASCII);
                    }
                    TextureList = ByteConverter.ToUInt32(file, address + 0x1C);
                    break;
            }
        }

        public static LandTable LoadFromFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
            byte version = file[7];
            if (version > CurrentVersion)
                throw new FormatException("Not a valid SA1LVL/SA2LVL file.");
            Dictionary<int, string> labels = new Dictionary<int, string>();
            if (version == 1)
            {
                int tmpaddr = BitConverter.ToInt32(file, 0xC);
                if (tmpaddr != 0)
                {
                    int addr = BitConverter.ToInt32(file, tmpaddr);
                    while (addr != -1)
                    {
                        labels.Add(addr, file.GetCString(BitConverter.ToInt32(file, tmpaddr + 4)));
                        tmpaddr += 8;
                        addr = BitConverter.ToInt32(file, tmpaddr);
                    }
                }
            }
            if (magic == SA1LVL)
                return new LandTable(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA1, labels);
            else if (magic == SA2LVL)
                return new LandTable(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA2, labels);
            else
                throw new FormatException("Not a valid SA1LVL/SA2LVL file.");
        }

        public static bool CheckLevelFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            switch (ByteConverter.ToUInt64(file, 0) & FormatMask)
            {
                case SA1LVL:
                case SA2LVL:
                    return file[7] <= CurrentVersion;
                default:
                    return false;
            }
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, Dictionary<string, uint> labels, out uint address)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            List<byte> result = new List<byte>();
            byte[] tmpbyte;
            uint[] colmdladdrs = new uint[COL.Count];
            uint tmpaddr = 0;
            for (int i = 0; i < COL.Count; i++)
            {
                if (labels.ContainsKey(COL[i].Model.Name))
                    colmdladdrs[i] = labels[COL[i].Model.Name];
                else
                {
                    result.Align(4);
                    tmpbyte = COL[i].Model.GetBytes(imageBase + (uint)result.Count, format, labels, out tmpaddr);
                    colmdladdrs[i] = tmpaddr + (uint)result.Count + imageBase;
                    result.AddRange(tmpbyte);
                }
            }
            uint[] animmdladdrs = new uint[Anim.Count];
            uint[] animaniaddrs = new uint[Anim.Count];
            for (int i = 0; i < Anim.Count; i++)
            {
                if (labels.ContainsKey(Anim[i].Model.Name))
                    animmdladdrs[i] = labels[Anim[i].Model.Name];
                else
                {
                    result.Align(4);
                    tmpbyte = Anim[i].Model.GetBytes(imageBase + (uint)result.Count, format, labels, out tmpaddr);
                    animmdladdrs[i] = tmpaddr + (uint)result.Count + imageBase;
                    result.AddRange(tmpbyte);
                }
                if (labels.ContainsKey(Anim[i].Animation.Name))
                    animaniaddrs[i] = labels[Anim[i].Animation.Name];
                else
                {
                    result.Align(4);
                    tmpbyte = Anim[i].Animation.WriteHeader(imageBase + (uint)result.Count, animmdladdrs[i], labels, out tmpaddr);
                    animaniaddrs[i] = tmpaddr + (uint)result.Count + imageBase;
                    result.AddRange(tmpbyte);
                }
            }
            uint coladdr;
            if (labels.ContainsKey(COLName))
                coladdr = labels[COLName];
            else
            {
                coladdr = imageBase + (uint)result.Count;
                labels.Add(COLName, coladdr);
                for (int i = 0; i < COL.Count; i++)
                {
                    result.Align(4);
                    labels.Add(COL[i].Name, imageBase + (uint)result.Count);
                    result.AddRange(COL[i].GetBytes(imageBase + (uint)result.Count, colmdladdrs[i], format));
                }
            }
            uint animaddr;
            if (Anim.Count > 0)
            {
                if (labels.ContainsKey(AnimName))
                    animaddr = labels[AnimName];
                else
                {
                    animaddr = imageBase + (uint)result.Count;
                    labels.Add(AnimName, animaddr);
                    for (int i = 0; i < Anim.Count; i++)
                    {
                        result.Align(4);
                        labels.Add(Anim[i].Name, imageBase + (uint)result.Count);
                        result.AddRange(Anim[i].GetBytes(imageBase + (uint)result.Count, animmdladdrs[i], animaniaddrs[i]));
                    }
                }
            }
            else
                animaddr = 0;
            result.Align(4);
            uint texnameaddr = 0;
            if (TextureFileName != null)
            {
                texnameaddr = imageBase + (uint)result.Count;
                result.AddRange(Encoding.ASCII.GetBytes(TextureFileName));
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
            labels.Add(Name, address + imageBase);
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, out uint address)
        {
            return GetBytes(imageBase, format, new Dictionary<string, uint>(), out address);
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format)
        {
            uint address;
            return GetBytes(imageBase, format, out address);
        }

        public void SaveToFile(string filename, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            ulong magic;
            switch (format)
            {
                case ModelFormat.SA1:
                    magic = SA1LVLVer;
                    break;
                /*case ModelFormat.SA2:
                    magic = SA2LVLVer;
                    break;*/
                default:
                    throw new ArgumentException("Cannot save " + format.ToString() + " format levels to file!", "format");
            }
            file.AddRange(ByteConverter.GetBytes(magic));
            uint addr = 0;
            Dictionary<string, uint> labels = new Dictionary<string, uint>();
            byte[] lvl = GetBytes(0x10, ModelFormat.SA1, labels, out addr);
            file.AddRange(ByteConverter.GetBytes(addr + 0x10));
            file.Align(0x10);
            file.AddRange(lvl);
            file.Align(4);
            if (labels.Count > 0)
            {
                file.RemoveRange(0xC, 4);
                file.InsertRange(0xC, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (labels.Count * 8) + 8;
                List<byte> strbytes = new List<byte>();
                foreach (KeyValuePair<string, uint> label in labels)
                {
                    file.AddRange(ByteConverter.GetBytes(label.Value));
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(label.Key));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1L));
                file.AddRange(strbytes);
                file.Align(4);
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }
    }
}