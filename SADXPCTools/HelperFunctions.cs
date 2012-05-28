using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SADXPCTools
{
    public static class HelperFunctions
    {
        public static void SetupEXE(ref byte[] exefile)
        {
            int ptr = BitConverter.ToInt32(exefile, 0x3c);
            if (BitConverter.ToInt32(exefile, (int)ptr) == 0x4550) //PE\0\0
            {
                ptr += 4;
                UInt16 numsects = BitConverter.ToUInt16(exefile, (int)ptr + 2);
                int sectnumptr = ptr + 2;
                ptr += 0x14;
                int PEHead = ptr;
                ptr += 0xe0;
                int sectptr = ptr;
                ptr += 0x28 * 2;
                int strlen = 0;
                for (int i = 0; i <= 7; i++)
                    if (exefile[ptr + i] > 0)
                        strlen += 1;
                if (Encoding.ASCII.GetString(exefile, ptr, strlen) == ".data")
                    if (Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize)) > Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FSize)))
                    {
                        UInt32 dif = Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize)) - Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FSize));
                        BitConverter.GetBytes(Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize))).CopyTo(exefile, ptr + (int)SectOffs.FSize);
                        UInt32 splitaddr = BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.Size + (int)SectOffs.FAddr);
                        for (int i = 1; i <= numsects - 3; i++)
                            BitConverter.GetBytes(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FAddr + (i * (int)SectOffs.Size)) + dif).CopyTo(exefile, ptr + (int)SectOffs.FAddr + (i * (int)SectOffs.Size));
                        byte[] newfile = new byte[exefile.Length + dif];
                        MemoryStream mystream = new MemoryStream(newfile, true);
                        mystream.Write(exefile, 0, (int)splitaddr);
                        mystream.Seek(dif, SeekOrigin.Current);
                        mystream.Write(exefile, (int)splitaddr, exefile.Length - (int)splitaddr);
                        exefile = newfile;
                    }
                    else
                        Console.WriteLine("Are you sure this is an SADX EXE?");
            }
            else
                Console.WriteLine("This doesn't seem to be a valid EXE file...");
        }

        public static uint GetNewSectionAddress(byte[] exefile)
        {
            int ptr = BitConverter.ToInt32(exefile, 0x3c);
            ptr += 4;
            UInt16 numsects = BitConverter.ToUInt16(exefile, (int)ptr + 2);
            ptr += 0x14;
            ptr += 0xe0;
            ptr += (int)SectOffs.Size * (numsects - 1);
            return HelperFunctions.Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FAddr) + BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FSize));
        }

        public static void CreateNewSection(ref byte[] exefile, string name, byte[] data, bool isCode)
        {
            int ptr = BitConverter.ToInt32(exefile, 0x3c);
            ptr += 4;
            UInt16 numsects = BitConverter.ToUInt16(exefile, ptr + 2);
            int sectnumptr = ptr + 2;
            ptr += 0x14;
            int PEHead = ptr;
            ptr += 0xe0;
            int sectptr = ptr;
            ptr += (int)SectOffs.Size * numsects;
            BitConverter.GetBytes((ushort)(numsects + 1)).CopyTo(exefile, sectnumptr);
            Array.Clear(exefile, ptr, 8);
            Encoding.ASCII.GetBytes(name).CopyTo(exefile, ptr);
            UInt32 myaddr = HelperFunctions.Align(BitConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.FAddr) + BitConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.FSize));
            BitConverter.GetBytes(myaddr).CopyTo(exefile, ptr + (int)SectOffs.VAddr);
            BitConverter.GetBytes(myaddr).CopyTo(exefile, ptr + (int)SectOffs.FAddr);
            BitConverter.GetBytes(isCode ? 0x60000020 : 0xC0000040).CopyTo(exefile, ptr + (int)SectOffs.Flags);
            int diff = (int)HelperFunctions.Align((uint)data.Length);
            BitConverter.GetBytes(diff).CopyTo(exefile, ptr + (int)SectOffs.VSize);
            BitConverter.GetBytes(diff).CopyTo(exefile, ptr + (int)SectOffs.FSize);
            if (isCode)
                BitConverter.GetBytes(Convert.ToUInt32(BitConverter.ToUInt32(exefile, PEHead + 4) + diff)).CopyTo(exefile, PEHead + 4);
            else
                BitConverter.GetBytes(Convert.ToUInt32(BitConverter.ToUInt32(exefile, PEHead + 8) + diff)).CopyTo(exefile, PEHead + 8);
            BitConverter.GetBytes(Convert.ToUInt32(BitConverter.ToUInt32(exefile, PEHead + 0x38) + diff)).CopyTo(exefile, PEHead + 0x38);
            Array.Resize(ref exefile, exefile.Length + diff);
            data.CopyTo(exefile, myaddr);
        }

        public static void AlignCode(this List<byte> me)
        {
            while (me.Count % 0x10 > 0)
                me.Add(0x90);
        }

        public static uint Align(uint address)
        {
            if (address % 0x1000 == 0) return address;
            return ((address / 0x1000) + 1) * 0x1000;
        }

        static readonly System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        public static string FileHash(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            file = md5.ComputeHash(file);
            string result = string.Empty;
            foreach (byte item in file)
                result += item.ToString("x2");
            return result;
        }

        private static readonly Encoding jpenc = Encoding.GetEncoding(932);
        private static readonly Encoding euenc = Encoding.GetEncoding(1252);

        public static Encoding GetEncoding() { return jpenc; }

        public static Encoding GetEncoding(Languages language)
        {
            switch (language)
            {
                case Languages.Japanese:
                case Languages.English:
                    return jpenc;
                default:
                    return euenc;
            }
        }

        internal static void Align(this List<byte> me, int alignment)
        {
            int off = me.Count % alignment;
            if (off == 0) return;
            me.AddRange(new byte[alignment - off]);
        }

        public static string GetCString(this byte[] file, int address, Encoding encoding)
        {
            int count = 0;
            while (file[address + count] != 0)
                count++;
            return encoding.GetString(file, address, count);
        }

        public static string GetCString(this byte[] file, int address)
        {
            return GetCString(file, address, jpenc);
        }

        public static string UnescapeNewlines(this string line)
        {
            StringBuilder sb = new StringBuilder(line.Length);
            for (int c = 0; c < line.Length; c++)
                switch (line[c])
                {
                    case '\\': // escape character
                        if (c + 1 == line.Length) goto default;
                        c++;
                        switch (line[c])
                        {
                            case 'n': // line feed
                                sb.Append('\n');
                                break;
                            case 'r': // carriage return
                                sb.Append('\r');
                                break;
                            default: // literal character
                                sb.Append(line[c]);
                                break;
                        }
                        break;
                    default:
                        sb.Append(line[c]);
                        break;
                }
            return sb.ToString();
        }

        public static string EscapeNewlines(this string line)
        {
            return line.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r");
        }
    }

    enum SectOffs
    {
        VSize = 8,
        VAddr = 0xC,
        FSize = 0x10,
        FAddr = 0x14,
        Flags = 0x24,
        Size = 0x28
    }

    public enum LevelIDs : byte
    {
        HedgehogHammer = 0,
        EmeraldCoast = 1,
        WindyValley = 2,
        TwinklePark = 3,
        SpeedHighway = 4,
        RedMountain = 5,
        SkyDeck = 6,
        LostWorld = 7,
        IceCap = 8,
        Casinopolis = 9,
        FinalEgg = 0xA,
        HotShelter = 0xC,
        Chaos0 = 0xF,
        Chaos2 = 0x10,
        Chaos4 = 0x11,
        Chaos6 = 0x12,
        Chaos7 = 0x13,
        EggHornet = 0x14,
        EggWalker = 0x15,
        EggViper = 0x16,
        Zero = 0x17,
        E101 = 0x18,
        E101R = 0x19,
        StationSquare = 0x1A,
        EggCarrierOutside = 0x1D,
        EggCarrierInside = 0x20,
        MysticRuins = 0x21,
        Past = 0x22,
        TwinkleCircuit = 0x23,
        SkyChase1 = 0x24,
        SkyChase2 = 0x25,
        SandHill = 0x26,
        SSGarden = 0x27,
        ECGarden = 0x28,
        MRGarden = 0x29,
        ChaoRace = 0x2A,
        Invalid = 0x2B
    }

    public enum Characters : byte
    {
        Sonic = 0,
        Eggman = 1,
        Tails = 2,
        Knuckles = 3,
        Tikal = 4,
        Amy = 5,
        Gamma = 6,
        Big = 7,
        MetalSonic = 8
    }

    [Flags()]
    public enum CharacterFlags
    {
        Sonic = 1 << Characters.Sonic,
        Eggman = 1 << Characters.Eggman,
        Tails = 1 << Characters.Tails,
        Knuckles = 1 << Characters.Knuckles,
        Tikal = 1 << Characters.Tikal,
        Amy = 1 << Characters.Amy,
        Gamma = 1 << Characters.Gamma,
        Big = 1 << Characters.Big
    }

    public enum Languages
    {
        Japanese = 0,
        English = 1,
        French = 2,
        Spanish = 3,
        German = 4
    }
}