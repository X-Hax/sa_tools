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
                if (string.IsNullOrWhiteSpace(item.Key)) continue;
                string filedesc = item.Key;
                Dictionary<string, string> data = item.Value;
                string type = string.Empty;
                if (data.ContainsKey("type"))
                    type = data["type"];
                Console.WriteLine(item.Key + ": " + data["address"] + " - " + data["filename"]);
                Directory.CreateDirectory(Path.GetDirectoryName(data["filename"]));
                switch (type)
                {
                    case "landtable":
                        SonicRetro.SAModel.LandTable tbl = new SonicRetro.SAModel.LandTable(exefile, int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber), imageBase, true);
                        Dictionary<string, Dictionary<string, string>> tblini = new Dictionary<string, Dictionary<string, string>>();
                        tblini.Add(string.Empty, new Dictionary<string, string>() { { "LandTable", tbl.Name } });
                        tbl.Save(tblini, Path.Combine(Path.GetDirectoryName(data["filename"]), "Animations"));
                        IniFile.Save(tblini, data["filename"]);
                        break;
                    case "model":
                        SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(exefile, int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber), imageBase, true);
                        Dictionary<string, Dictionary<string, string>> mdlini = new Dictionary<string, Dictionary<string, string>>();
                        mdlini.Add(string.Empty, new Dictionary<string, string>());
                        mdlini[string.Empty].Add("Root", mdl.Name);
                        if (data.ContainsKey("animations"))
                            mdlini[string.Empty].Add("Animations", data["animations"]);
                        mdl.Save(mdlini);
                        IniFile.Save(mdlini, data["filename"]);
                        break;
                    case "animation":
                        SonicRetro.SAModel.Animation ani = new SonicRetro.SAModel.Animation(exefile, int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber), imageBase, true);
                        ani.Save(data["filename"]);
                        break;
                    default: // raw binary
                        byte[] bin = new byte[int.Parse(data["size"], System.Globalization.NumberStyles.HexNumber)];
                        Array.Copy(exefile, int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber), bin, 0, bin.Length);
                        File.WriteAllBytes(data["filename"], bin);
                        break;
                }
                data.Add("md5", FileHash(data["filename"]));
            }
            IniFile.Save(inifile, Path.Combine(Path.GetDirectoryName(exefilename), Path.GetFileNameWithoutExtension(exefilename)) + "_data.ini");
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
}