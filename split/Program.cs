using System;
using System.Collections.Generic;
using System.IO;

namespace split
{
    static class Program
    {
        static void Main(string[] args)
        {
            string exefilename, inifilename;
            if (args.Length > 0)
            {
                exefilename = args[0];
                Console.WriteLine("EXE File: {0}", exefilename);
            }
            else
            {
                Console.Write("EXE File: ");
                exefilename = Console.ReadLine();
            }
            if (args.Length > 1)
            {
                inifilename = args[1];
                Console.WriteLine("INI File: {0}", inifilename);
            }
            else
            {
                Console.Write("INI File: ");
                inifilename = Console.ReadLine();
            }
            byte[] exefile = File.ReadAllBytes(exefilename);
            SetupEXE(ref exefile);
            Dictionary<string, Dictionary<string, string>> inifile = IniFile.Load(inifilename);
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(exefilename));
            uint imageBase = uint.Parse(inifile[string.Empty]["key"], System.Globalization.NumberStyles.HexNumber);
            foreach (KeyValuePair<string, Dictionary<string, string>> item in inifile)
            {
                if (string.IsNullOrEmpty(item.Key)) continue;
                string filedesc = item.Key;
                Dictionary<string, string> data = item.Value;
                string type = string.Empty;
                if (data.ContainsKey("type"))
                    type = data["type"];
                int address = int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                Console.WriteLine(item.Key + ": " + data["address"] + " → " + data["filename"]);
                Directory.CreateDirectory(Path.GetDirectoryName(data["filename"]));
                switch (type)
                {
                    case "landtable":
                        new SonicRetro.SAModel.LandTable(exefile, address, imageBase, SonicRetro.SAModel.ModelFormat.SADX).SaveToFile(data["filename"], SonicRetro.SAModel.ModelFormat.SA1);
                        break;
                    case "model":
                        SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(exefile, address, imageBase, SonicRetro.SAModel.ModelFormat.SADX);
                        Dictionary<string, Dictionary<string, string>> mdlini = new Dictionary<string, Dictionary<string, string>>();
                        mdlini.Add(string.Empty, new Dictionary<string, string>());
                        mdlini[string.Empty].Add("Root", mdl.Name);
                        if (data.ContainsKey("animations"))
                            mdlini[string.Empty].Add("Animations", data["animations"]);
                        mdl.Save(mdlini);
                        IniFile.Save(mdlini, data["filename"]);
                        break;
                    case "animation":
                        SonicRetro.SAModel.Animation ani = new SonicRetro.SAModel.Animation(exefile, address, imageBase, SonicRetro.SAModel.ModelFormat.SADX);
                        ani.Save(data["filename"]);
                        break;
                    case "objlist":
                        int numobjs = BitConverter.ToInt32(exefile, address);
                        address = (int)(BitConverter.ToUInt32(exefile, address + 4) - imageBase);
                        Dictionary<string, Dictionary<string, string>> objini = new Dictionary<string, Dictionary<string, string>>();
                        for (int i = 0; i < numobjs; i++)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            objgrp.Add("Arg1", exefile[address].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("Arg2", exefile[address + 1].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("Flags", BitConverter.ToUInt16(exefile, address + 2).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("Distance", BitConverter.ToSingle(exefile, address + 4).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("Unknown", BitConverter.ToInt32(exefile, address + 8).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("Code", BitConverter.ToUInt32(exefile, address + 12).ToString("X8"));
                            objgrp.Add("Name", GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address + 16) - imageBase)));
                            objini.Add(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), objgrp);
                            address += 0x14;
                        }
                        IniFile.Save(objini, data["filename"]);
                        break;
                    case "startpos":
                        int numpos = 0;
                        Dictionary<string, Dictionary<string, string>> posini = new Dictionary<string, Dictionary<string, string>>();
                        while (BitConverter.ToUInt16(exefile, address) != (ushort)LevelIDs.Invalid)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            objgrp.Add("Position", new SonicRetro.SAModel.Vertex(exefile, address + 4).ToString());
                            objgrp.Add("YRotation", BitConverter.ToInt32(exefile, address + 0x10).ToString("X8"));
                            posini.Add(((LevelIDs)BitConverter.ToUInt16(exefile, address)).ToString() + " " + BitConverter.ToUInt16(exefile, address + 2).ToString(System.Globalization.NumberFormatInfo.InvariantInfo), objgrp);
                            numpos++;
                            address += 0x14;
                        }
                        IniFile.Save(posini, data["filename"]);
                        break;
                    case "texlist":
                        Dictionary<string, Dictionary<string, string>> texini = new Dictionary<string, Dictionary<string, string>>();
                        int txi = 0;
                        while (BitConverter.ToUInt64(exefile, address) != 0)
                        {
                            Dictionary<string, string> group = new Dictionary<string, string>();
                            if (BitConverter.ToUInt32(exefile, address) == 0)
                                group.Add("Name", string.Empty);
                            else
                                group.Add("Name", GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address) - imageBase)));
                            group.Add("Textures", BitConverter.ToUInt32(exefile, address + 4).ToString("X8"));
                            texini.Add(txi.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), group);
                            address += 8;
                            txi++;
                        }
                        IniFile.Save(texini, data["filename"]);
                        break;
                    case "leveltexlist":
                        Dictionary<string, Dictionary<string, string>> lvltxini = new Dictionary<string, Dictionary<string, string>>() { { string.Empty, new Dictionary<string, string>() } };
                        ushort levelnum = BitConverter.ToUInt16(exefile, address);
                        lvltxini[string.Empty].Add("Level", ((LevelIDs)((levelnum >> 8) & 0xFF)).ToString() + " " + (levelnum & 0xFF).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                        ushort lvltxnum = BitConverter.ToUInt16(exefile, address + 2);
                        address = (int)(BitConverter.ToUInt32(exefile, address + 4) - imageBase);
                        for (int i = 0; i < lvltxnum; i++)
                        {
                            Dictionary<string, string> group = new Dictionary<string, string>();
                            if (BitConverter.ToUInt32(exefile, address) == 0)
                                group.Add("Name", string.Empty);
                            else
                                group.Add("Name", GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address) - imageBase)));
                            group.Add("Textures", BitConverter.ToUInt32(exefile, address + 4).ToString("X8"));
                            lvltxini.Add(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), group);
                            address += 8;
                        }
                        IniFile.Save(lvltxini, data["filename"]);
                        break;
                    case "triallevellist":
                        List<string> lvllist = new List<string>();
                        uint lvlcnt = BitConverter.ToUInt32(exefile, address + 4);
                        address = (int)(BitConverter.ToUInt32(exefile, address) - imageBase);
                        for (int i = 0; i < lvlcnt; i++)
                        {
                            lvllist.Add(((LevelIDs)exefile[address]).ToString() + " " + exefile[address + 1].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            address += 2;
                        }
                        File.WriteAllLines(data["filename"], lvllist.ToArray());
                        break;
                    case "bosslevellist":
                        lvllist = new List<string>();
                        while (BitConverter.ToUInt16(exefile, address) != (ushort)LevelIDs.Invalid)
                        {
                            lvllist.Add(((LevelIDs)BitConverter.ToUInt16(exefile, address)).ToString() + " " + BitConverter.ToUInt16(exefile, address + 2).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            address += 4;
                        }
                        File.WriteAllLines(data["filename"], lvllist.ToArray());
                        break;
                    case "fieldstartpos":
                        numpos = 0;
                        posini = new Dictionary<string, Dictionary<string, string>>();
                        while (exefile[address] != (byte)LevelIDs.Invalid)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            objgrp.Add("Field", ((LevelIDs)exefile[address + 2]).ToString());
                            objgrp.Add("Position", new SonicRetro.SAModel.Vertex(exefile, address + 4).ToString());
                            objgrp.Add("YRotation", BitConverter.ToInt32(exefile, address + 0x10).ToString("X8"));
                            posini.Add(((LevelIDs)exefile[address]).ToString(), objgrp);
                            numpos++;
                            address += 0x14;
                        }
                        IniFile.Save(posini, data["filename"]);
                        break;
                    case "soundtestlist":
                        Dictionary<string, Dictionary<string,string>> soundini = new Dictionary<string, Dictionary<string, string>>();
                        uint soundcnt = BitConverter.ToUInt32(exefile, address + 4);
                        address = (int)(BitConverter.ToUInt32(exefile, address) - imageBase);
                        for (int i = 0; i < soundcnt; i++)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            if (BitConverter.ToUInt32(exefile, address) == 0)
                                objgrp.Add("Title", string.Empty);
                            else
                                objgrp.Add("Title", GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address) - imageBase)));
                            objgrp.Add("Track", BitConverter.ToInt32(exefile, address + 4).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            soundini.Add(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), objgrp);
                            address += 8;
                        }
                        IniFile.Save(soundini, data["filename"]);
                        break;
                    case "musiclist":
                        Dictionary<string, Dictionary<string, string>> musini = new Dictionary<string, Dictionary<string, string>>();
                        int muscnt = int.Parse(data["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        for (int i = 0; i < muscnt; i++)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            if (BitConverter.ToUInt32(exefile, address) == 0)
                                objgrp.Add("Filename", string.Empty);
                            else
                                objgrp.Add("Filename", GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address) - imageBase)));
                            objgrp.Add("Loop", (BitConverter.ToInt32(exefile, address + 4) != 0).ToString());
                            musini.Add(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), objgrp);
                            address += 8;
                        }
                        IniFile.Save(musini, data["filename"]);
                        break;
                    case "soundlist":
                        soundini = new Dictionary<string, Dictionary<string, string>>();
                        soundcnt = BitConverter.ToUInt32(exefile, address);
                        address = (int)(BitConverter.ToUInt32(exefile, address + 4) - imageBase);
                        for (int i = 0; i < soundcnt; i++)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            objgrp.Add("Bank", BitConverter.ToInt32(exefile, address).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            if (BitConverter.ToUInt32(exefile, address + 4) == 0)
                                objgrp.Add("Filename", string.Empty);
                            else
                                objgrp.Add("Filename", GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address + 4) - imageBase)));
                            soundini.Add(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), objgrp);
                            address += 8;
                        }
                        IniFile.Save(soundini, data["filename"]);
                        break;
                    case "stringarray":
                        int cnt = int.Parse(data["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        string[] strs = new string[cnt];
                        for (int i = 0; i < cnt; i++)
                        {
                            if (BitConverter.ToUInt32(exefile, address) == 0)
                                strs[i] = string.Empty;
                            else
                                strs[i] = GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, address) - imageBase)).Replace("\n", "\\n");
                            address += 4;
                        }
                        File.WriteAllLines(data["filename"], strs);
                        break;
                    case "nextlevellist":
                        numpos = 0;
                        posini = new Dictionary<string, Dictionary<string, string>>();
                        while (exefile[address + 1] != (byte)LevelIDs.Maximum)
                        {
                            Dictionary<string, string> objgrp = new Dictionary<string, string>();
                            objgrp.Add("CGMovie", exefile[address].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("Level", ((LevelIDs)exefile[address + 1]).ToString());
                            objgrp.Add("NextLevel", ((LevelIDs)exefile[address + 2]).ToString());
                            objgrp.Add("NextAct", exefile[address + 3].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("StartPos", exefile[address + 4].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("AltNextLevel", ((LevelIDs)exefile[address + 5]).ToString());
                            objgrp.Add("AltNextAct", exefile[address + 6].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            objgrp.Add("AltStartPos", exefile[address + 7].ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                            posini.Add(numpos.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), objgrp);
                            numpos++;
                            address += 8;
                        }
                        IniFile.Save(posini, data["filename"]);
                        break;
                    case "cutscenetext":
                        Directory.CreateDirectory(data["filename"]);
                        cnt = int.Parse(data["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        string[] hashes = new string[5];
                        for (int i = 0; i < 5; i++)
                        {
                            int ptr = (int)(BitConverter.ToUInt32(exefile, address) - imageBase);
                            strs = new string[cnt];
                            for (int j = 0; j < cnt; j++)
                            {
                                if (BitConverter.ToUInt32(exefile, ptr) == 0)
                                    strs[j] = string.Empty;
                                else
                                    strs[j] = GetCString(exefile, (int)(BitConverter.ToUInt32(exefile, ptr) - imageBase), i < 2 ? jpenc : euenc).Replace("\n", "\\n");
                                ptr += 4;
                            }
                            string textname = Path.Combine(data["filename"], ((Languages)i).ToString() + ".txt");
                            File.WriteAllLines(textname, strs);
                            hashes[i] = FileHash(textname);
                            address += 4;
                        }
                        data.Add("md5", string.Join(",", hashes));
                        break;
                    default: // raw binary
                        byte[] bin = new byte[int.Parse(data["size"], System.Globalization.NumberStyles.HexNumber)];
                        Array.Copy(exefile, int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber), bin, 0, bin.Length);
                        File.WriteAllBytes(data["filename"], bin);
                        break;
                }
                bool nohash = false;
                if (data.ContainsKey("nohash"))
                    nohash = bool.Parse(data["nohash"]);
                if (!nohash)
                    data.Add("md5", FileHash(data["filename"]));
            }
            IniFile.Save(inifile, Path.Combine(Path.GetDirectoryName(exefilename), Path.GetFileNameWithoutExtension(exefilename)) + "_data.ini");
            Console.Write("Press a key to continue...");
            Console.ReadKey(true);
            Console.WriteLine();
        }

        static void SetupEXE(ref byte[] exefile)
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
                if (System.Text.Encoding.ASCII.GetString(exefile, ptr, strlen) == ".data")
                    if (Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize)) > Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FSize)))
                    {
                        UInt32 dif = Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize)) - Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FSize));
                        BitConverter.GetBytes(Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.VSize))).CopyTo(exefile, ptr + (int)SectOffs.FSize);
                        UInt32 splitaddr = BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.Size + (int)SectOffs.FAddr);
                        for (int i = 1; i <= numsects - 3; i++)
                            BitConverter.GetBytes(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FAddr + (i * (int)SectOffs.Size)) + dif).CopyTo(exefile, ptr + (int)SectOffs.FAddr + (i * (int)SectOffs.Size));
                        byte[] newfile = new byte[exefile.Length + dif];
                        System.IO.MemoryStream mystream = new System.IO.MemoryStream(newfile, true);
                        mystream.Write(exefile, 0, (int)splitaddr);
                        mystream.Seek(dif, System.IO.SeekOrigin.Current);
                        mystream.Write(exefile, (int)splitaddr, exefile.Length - (int)splitaddr);
                        exefile = newfile;
                    }
                    else
                        Console.WriteLine("Are you sure this is an SADX EXE?");
            }
            else
                Console.WriteLine("This doesn't seem to be a valid EXE file...");
        }

        static uint Align(uint address)
        {
            if (address % 0x1000 == 0) return address;
            return ((address / 0x1000) + 1) * 0x1000;
        }

        static System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        static string FileHash(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            file = md5.ComputeHash(file);
            string result = string.Empty;
            foreach (byte item in file)
                result += item.ToString("x2");
            return result;
        }

        static System.Text.Encoding jpenc = System.Text.Encoding.GetEncoding(932);
        static System.Text.Encoding euenc = System.Text.Encoding.GetEncoding(1252);

        static string GetCString(byte[] file, int address, System.Text.Encoding encoding)
        {
            int count = 0;
            while (file[address + count] != 0)
                count++;
            return encoding.GetString(file, address, count);
        }

        static string GetCString(byte[] file, int address)
        {
            return GetCString(file, address, jpenc);
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

    enum LevelIDs : byte
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
        Invalid = 0x2B,
        Maximum = 0xFF
    }

    enum Characters : byte
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

    enum Languages
    {
        Japanese = 0,
        English = 1,
        French = 2,
        Spanish = 3,
        German = 4
    }
}