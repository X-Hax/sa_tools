using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;

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
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(exefilename));
            Dictionary<string, uint> addresses = new Dictionary<string, uint>();
            string inistartpath = Path.Combine(Path.GetDirectoryName(exefilename), Path.GetFileNameWithoutExtension(exefilename));
            Dictionary<string, Dictionary<string, string>> inifile = IniFile.Load(inistartpath + "_data.ini");
            uint imageBase = uint.Parse(inifile[string.Empty]["key"], System.Globalization.NumberStyles.HexNumber);
            uint startaddress = GetNewSectionAddress(exefile);
            uint curaddr = startaddress;
            if (File.Exists(inistartpath + "_code.ini"))
            {
                Dictionary<string, Dictionary<string, string>> codeINI = IniFile.Load(inistartpath + "_code.ini");
                List<byte> codesection = new List<byte>();
                foreach (KeyValuePair<string, Dictionary<string, string>> dictitem in codeINI)
                {
                    if (string.IsNullOrWhiteSpace(dictitem.Key)) continue;
                    Dictionary<string, string> data = dictitem.Value;
                    if (data.ContainsKey("nop"))
                        foreach (string item in data["nop"].Split(','))
                        {
                            int staddr = int.Parse(item.Split('-')[0], System.Globalization.NumberStyles.HexNumber);
                            int endaddr = int.Parse(item.Split('-')[1], System.Globalization.NumberStyles.HexNumber);
                            for (int i = staddr; i <= endaddr; i++)
                                exefile[i] = 0x90;
                        }
                    string asm = File.ReadAllText(data["filename"]);
                    asm = "bits 32" + Environment.NewLine + "org 0x" + curaddr.ToString("X8") + Environment.NewLine + asm;
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
                            int disp = (int)curaddr - (i + 5);
                            BitConverter.GetBytes(disp).CopyTo(exefile, i + 1);
                        }
                    if (data.ContainsKey("pointer"))
                        foreach (string item in data["pointer"].Split(','))
                        {
                            int i = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                            BitConverter.GetBytes(startaddress).CopyTo(exefile, i);
                        }
                    addresses.Add(dictitem.Key, curaddr + imageBase);
                    curaddr = startaddress + (uint)codesection.Count;
                }
                CreateNewSection(ref exefile, ".text2", codesection.ToArray(), true);
            }
            List<byte> datasection = new List<byte>();
            startaddress = GetNewSectionAddress(exefile);
            curaddr = startaddress + imageBase;
            uint dataaddr = curaddr;
            foreach (KeyValuePair<string, Dictionary<string, string>> dictitem in inifile)
            {
                if (string.IsNullOrWhiteSpace(dictitem.Key)) continue;
                string filedesc = dictitem.Key;
                Dictionary<string, string> data = dictitem.Value;
                string type = string.Empty;
                if (data.ContainsKey("type"))
                    type = data["type"];
                if (FileHash(data["filename"]) != data["md5"])
                {
                    switch (type)
                    {
                        case "landtable":
                            Dictionary<string, Dictionary<string, string>> tblini = IniFile.Load(data["filename"]);
                            SonicRetro.SAModel.LandTable tbl = new SonicRetro.SAModel.LandTable(tblini, tblini[string.Empty]["LandTable"]);
                            datasection.AddRange(tbl.GetBytes(curaddr, true, out dataaddr));
                            //tbl = new SonicRetro.SAModel.LandTable(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "model":
                            Dictionary<string, Dictionary<string, string>> mdlini = IniFile.Load(data["filename"]);
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(mdlini, mdlini[string.Empty]["Root"]);
                            datasection.AddRange(mdl.GetBytes(curaddr, true, out dataaddr));
                            //mdl = new SonicRetro.SAModel.Object(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "animation":
                            Dictionary<string, Dictionary<string, string>> mdlini2 = IniFile.Load(inifile[data["model]"]]["filename"]);
                            SonicRetro.SAModel.Animation ani = new SonicRetro.SAModel.Animation(data["filename"]);
                            datasection.AddRange(ani.GetBytes(curaddr, addresses[data["model"]], new SonicRetro.SAModel.Object(mdlini2, mdlini2[string.Empty]["Root"]).GetObjects().Length, out dataaddr));
                            dataaddr += curaddr;
                            break;
                        case "objlist":
                            Dictionary<string, Dictionary<string, string>> objini = IniFile.Load(data["filename"]);
                            List<byte> objents = new List<byte>();
                            int objcnt = 0;
                            while (objini.ContainsKey(objcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = objini[objcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                objents.Add(byte.Parse(group["Arg1"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                                objents.Add(byte.Parse(group["Arg2"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                                objents.AddRange(BitConverter.GetBytes(ushort.Parse(group["Flags"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                                objents.AddRange(BitConverter.GetBytes(float.Parse(group["Distance"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo)));
                                objents.AddRange(BitConverter.GetBytes(int.Parse(group["Unknown"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                                uint code = 0;
                                if (!uint.TryParse(group["Code"], System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo, out code))
                                    code = addresses[group["Code"]];
                                objents.AddRange(BitConverter.GetBytes(code));
                                objents.AddRange(BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count));
                                datasection.AddRange(jpenc.GetBytes(group["Name"]));
                                datasection.Add(0);
                                datasection.Align(4);
                                objcnt++;
                            }
                            uint objentaddr = startaddress + imageBase + (uint)datasection.Count;
                            datasection.AddRange(objents.ToArray());
                            datasection.Align(4);
                            dataaddr = startaddress + imageBase + (uint)datasection.Count;
                            datasection.AddRange(BitConverter.GetBytes(objcnt));
                            datasection.AddRange(BitConverter.GetBytes(objentaddr));
                            break;
                        case "startpos":
                            Dictionary<string, Dictionary<string, string>> posini = IniFile.Load(data["filename"]);
                            foreach (KeyValuePair<string, Dictionary<string,string>> item in posini)
                            {
                                if (string.IsNullOrEmpty(item.Key)) continue;
                                datasection.AddRange(BitConverter.GetBytes(ushort.Parse(item.Key.Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                                datasection.AddRange(BitConverter.GetBytes(ushort.Parse(item.Key.Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo)));
                                datasection.AddRange(new SonicRetro.SAModel.Vertex(item.Value["Position"]).GetBytes());
                                datasection.AddRange(BitConverter.GetBytes(int.Parse(item.Value["YRotation"], System.Globalization.NumberStyles.HexNumber)));
                            }
                            datasection.AddRange(BitConverter.GetBytes((ushort)0x2B));
                            datasection.AddRange(new byte[0x12]);
                            break;
                        case "texlist":
                            Dictionary<string, Dictionary<string, string>> texini = IniFile.Load(data["filename"]);
                            List<byte> texents = new List<byte>();
                            int texcnt = 0;
                            while (texini.ContainsKey(texcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = texini[texcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                if (string.IsNullOrWhiteSpace(group["Name"]))
                                    texents.AddRange(new byte[4]);
                                else
                                {
                                    texents.AddRange(BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count));
                                    datasection.AddRange(jpenc.GetBytes(group["Name"]));
                                    datasection.Add(0);
                                    datasection.Align(4);
                                }
                                texents.AddRange(BitConverter.GetBytes(uint.Parse(group["Textures"], System.Globalization.NumberStyles.HexNumber)));
                                texcnt++;
                            }
                            dataaddr = startaddress + imageBase + (uint)datasection.Count;
                            datasection.AddRange(texents.ToArray());
                            datasection.Align(4);
                            break;
                        case "leveltexlist":
                            Dictionary<string, Dictionary<string, string>> lvltxini = IniFile.Load(data["filename"]);
                            List<byte> lvltxents = new List<byte>();
                            int lvltxcnt = 0;
                            while (lvltxini.ContainsKey(lvltxcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = lvltxini[lvltxcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                if (string.IsNullOrWhiteSpace(group["Name"]))
                                    lvltxents.AddRange(new byte[4]);
                                else
                                {
                                    lvltxents.AddRange(BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count));
                                    datasection.AddRange(jpenc.GetBytes(group["Name"]));
                                    datasection.Add(0);
                                    datasection.Align(4);
                                }
                                lvltxents.AddRange(BitConverter.GetBytes(uint.Parse(group["Textures"], System.Globalization.NumberStyles.HexNumber)));
                                lvltxcnt++;
                            }
                            uint lvltxaddr = startaddress + imageBase + (uint)datasection.Count;
                            datasection.AddRange(lvltxents.ToArray());
                            datasection.Align(4);
                            dataaddr = startaddress + imageBase + (uint)datasection.Count;
                            datasection.AddRange(BitConverter.GetBytes((ushort)((byte.Parse(data["level"].Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo) << 8) | byte.Parse(data["level"].Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo))));
                            datasection.AddRange(BitConverter.GetBytes((ushort)lvltxcnt));
                            datasection.AddRange(BitConverter.GetBytes(lvltxaddr));
                            break;
                        default: // raw binary
                            bool reloc = true;
                            if (data.ContainsKey("dontrelocate"))
                                reloc = !bool.Parse(data["dontrelocate"]);
                            byte[] file = File.ReadAllBytes(data["filename"]);
                            if (reloc)
                                datasection.AddRange(file);
                            else
                                Array.Copy(file, 0, exefile, int.Parse(data["address"], System.Globalization.NumberStyles.HexNumber), file.Length);
                            break;
                    }
                    if (data.ContainsKey("pointer"))
                        foreach (string item in data["pointer"].Split(','))
                        {
                            int i = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                            BitConverter.GetBytes(dataaddr).CopyTo(exefile, i);
                        }
                    addresses.Add(filedesc, dataaddr);
                    datasection.Align(4);
                    curaddr = startaddress + imageBase + (uint)datasection.Count;
                    dataaddr = curaddr;
                }
                else
                    addresses.Add(filedesc, uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber));
            }
            if (datasection.Count > 0)
                CreateNewSection(ref exefile, ".data2", datasection.ToArray(), false);
            if (File.Exists("postbuild.cs"))
            {
                CodeDomProvider pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", System.Reflection.Assembly.GetAssembly(typeof(SonicRetro.SAModel.LandTable)).Location });
                para.GenerateExecutable = false;
                para.GenerateInMemory = false;
                para.IncludeDebugInformation = true;
                para.OutputAssembly = Path.Combine(Environment.CurrentDirectory, "postbuild.dll");
                CompilerResults res = pr.CompileAssemblyFromFile(para, Path.Combine(Environment.CurrentDirectory, "postbuild.cs"));
                if (res.Errors.HasErrors)
                {
                    foreach (CompilerError item in res.Errors)
                        if (!item.IsWarning)
                            Console.WriteLine(item.ToString());
                }
                else
                {
                    res.CompiledAssembly.GetType("PostBuild")
                        .GetMethod("PostBuild", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.InvokeMethod, null, System.Reflection.CallingConventions.Standard, new Type[] { typeof(uint), typeof(byte[]), typeof(Dictionary<string, uint>) }, null)
                        .Invoke(null, new object[] { imageBase, exefile, addresses });
                }
            }
            File.WriteAllBytes(inistartpath + "_edit.exe", exefile);
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