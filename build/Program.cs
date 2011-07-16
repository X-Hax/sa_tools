using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace build
{
    static class Program
    {
        static void Main(string[] args)
        {
            string exefilename;
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
            byte[] exefile = File.ReadAllBytes(exefilename);
            SetupEXE(ref exefile);
            string inistartpath = Path.Combine(Path.GetDirectoryName(exefilename), Path.GetFileNameWithoutExtension(exefilename));
            if (File.Exists(inistartpath + "_code.ini"))
            {
                Dictionary<string, Dictionary<string, string>> codeINI = IniFile.Load(inistartpath + "_code.ini");
                uint startaddress = GetNewSectionAddress(exefile);
                List<byte> codesection = new List<byte>();
                foreach (KeyValuePair<string, Dictionary<string, string>> dictitem in codeINI)
                {
                    Dictionary<string, string> data = dictitem.Value;
                    if (data.ContainsKey("nop"))
                        foreach (string item in data["nop"].Split(','))
                        {
                            int startaddr = int.Parse(item.Split('-')[0], System.Globalization.NumberStyles.HexNumber);
                            int endaddr = int.Parse(item.Split('-')[1], System.Globalization.NumberStyles.HexNumber);
                            for (int i = startaddr; i <= endaddr; i++)
                                exefile[i] = 0x90;
                        }
                    string asm = File.ReadAllText(data["filename"]);
                    asm = "bits 32" + Environment.NewLine + "org 0x" + startaddress.ToString("X8") + Environment.NewLine + asm;
                    File.WriteAllText("asm.tmp", asm);
                    Process proc = Process.Start(new ProcessStartInfo("nasm.exe", "-o bin.tmp asm.tmp") { UseShellExecute = false, CreateNoWindow = true });
                    proc.WaitForExit();
                    codesection.AddRange(File.ReadAllBytes("bin.tmp"));
                    File.Delete("asm.tmp");
                    File.Delete("bin.tmp");
                    codesection.AlignCode();
                    if (data.ContainsKey("call"))
                        foreach (string item in data["call"].Split(','))
                        {
                            int i = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                            exefile[i] = 0xE8;
                            int disp = (int)startaddress - (i + 5);
                            BitConverter.GetBytes(disp).CopyTo(exefile, i + 1);
                        }
                    if (data.ContainsKey("pointer"))
                        foreach (string item in data["pointer"].Split(','))
                        {
                            int i = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                            BitConverter.GetBytes(startaddress).CopyTo(exefile, i);
                        }
                }
                CreateNewSection(ref exefile, ".text2", codesection.ToArray(), true);
            }
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(exefilename), Path.GetFileNameWithoutExtension(exefilename) + "_edit.exe"), exefile);
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
                    throw new Exception("Are you sure this is an SADX EXE?");
            }
            else
                throw new Exception("This doesn't seem to be a valid EXE file...");
        }

        static uint GetNewSectionAddress(byte[] exefile)
        {
            int ptr = BitConverter.ToInt32(exefile, 0x3c);
            ptr += 4;
            UInt16 numsects = BitConverter.ToUInt16(exefile, (int)ptr + 2);
            ptr += 0x14;
            ptr += 0xe0;
            ptr += (int)SectOffs.Size * (numsects - 1);
            return Align(BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FAddr) + BitConverter.ToUInt32(exefile, ptr + (int)SectOffs.FSize));
        }

        static void CreateNewSection(ref byte[] exefile, string name, byte[] data, bool isCode)
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
            System.Text.Encoding.ASCII.GetBytes(name).CopyTo(exefile, ptr);
            UInt32 myaddr = Align(BitConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.FAddr) + BitConverter.ToUInt32(exefile, ptr - (int)SectOffs.Size + (int)SectOffs.FSize));
            BitConverter.GetBytes(myaddr).CopyTo(exefile, ptr + (int)SectOffs.VAddr);
            BitConverter.GetBytes(myaddr).CopyTo(exefile, ptr + (int)SectOffs.FAddr);
            BitConverter.GetBytes(isCode ? 0x60000020 : 0xC0000040).CopyTo(exefile, ptr + (int)SectOffs.Flags);
            int diff = (int)Align((uint)data.Length);
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

        static uint Align(uint address)
        {
            if (address % 0x1000 == 0) return address;
            return ((address / 0x1000) + 1) * 0x1000;
        }

        static void Align(this List<byte> me, int bytes)
        {
            me.AddRange(new byte[me.Count % bytes]);
        }

        static void AlignCode(this List<byte> me)
        {
            while (me.Count % 0x10 > 0)
                me.Add(0x90);
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