using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;
using SonicRetro.SAModel;

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
            Dictionary<string, int> modelparts = new Dictionary<string, int>();
            string inistartpath = Path.Combine(Path.GetDirectoryName(exefilename), Path.GetFileNameWithoutExtension(exefilename));
            Dictionary<string, Dictionary<string, string>> inifile = IniFile.Load(inistartpath + "_data.ini");
            uint imageBase = uint.Parse(inifile[string.Empty]["key"], System.Globalization.NumberStyles.HexNumber);
            uint startaddress = GetNewSectionAddress(exefile);
            uint curaddr = startaddress;
            int itemcount = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            if (File.Exists(inistartpath + "_code.ini"))
            {
                Dictionary<string, Dictionary<string, string>> codeINI = IniFile.Load(inistartpath + "_code.ini");
                List<byte> codesection = new List<byte>();
                foreach (KeyValuePair<string, Dictionary<string, string>> dictitem in codeINI)
                {
                    if (string.IsNullOrEmpty(dictitem.Key)) continue;
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
                    itemcount++;
                    Console.WriteLine(dictitem.Key + ": " + curaddr.ToString("X"));
                }
                CreateNewSection(ref exefile, ".text2", codesection.ToArray(), true);
            }
            List<byte> datasection = new List<byte>();
            startaddress = GetNewSectionAddress(exefile);
            curaddr = startaddress + imageBase;
            uint dataaddr = curaddr;
            foreach (KeyValuePair<string, Dictionary<string, string>> dictitem in inifile)
            {
                if (string.IsNullOrEmpty(dictitem.Key)) continue;
                string filedesc = dictitem.Key;
                Dictionary<string, string> data = dictitem.Value;
                string type = string.Empty;
                if (data.ContainsKey("type"))
                    type = data["type"];
                bool nohash = false;
                if (data.ContainsKey("nohash"))
                    nohash = bool.Parse(data["nohash"]);
                if (nohash || FileHash(data["filename"]) != data["md5"])
                {
                    switch (type)
                    {
                        case "landtable":
                            LandTable tbl = LandTable.LoadFromFile(data["filename"]);
                            datasection.AddRange(tbl.GetBytes(curaddr, ModelFormat.SADX, out dataaddr));
                            //tbl = new LandTable(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "model":
                            SonicRetro.SAModel.Object mdl = SonicRetro.SAModel.Object.LoadFromFile(data["filename"]);
                            int modelp = 0;
                            foreach (SonicRetro.SAModel.Object o in mdl.GetObjects())
                                if ((mdl.Flags & ObjectFlags.NoAnimate) == 0)
                                    modelp++;
                            modelparts.Add(filedesc, modelp);
                            datasection.AddRange(mdl.GetBytes(curaddr, ModelFormat.SADX, out dataaddr));
                            //mdl = new Object(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "inimodel":
                            mdl = new ModelFile(IniFile.Load(data["filename"]), Path.GetDirectoryName(data["filename"])).Model;
                            modelp = 0;
                            foreach (SonicRetro.SAModel.Object o in mdl.GetObjects())
                                if ((mdl.Flags & ObjectFlags.NoAnimate) == 0)
                                    modelp++;
                            modelparts.Add(filedesc, modelp);
                            datasection.AddRange(mdl.GetBytes(curaddr, ModelFormat.SADX, out dataaddr));
                            //mdl = new Object(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "animation":
                            Animation ani = new Animation(data["filename"]);
                            datasection.AddRange(ani.GetBytes(curaddr, addresses[data["model"]], modelparts[data["model"]], out dataaddr));
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
                                ushort levelact = ParseLevelAct(item.Key);
                                datasection.AddRange(BitConverter.GetBytes((ushort)(levelact >> 8)));
                                datasection.AddRange(BitConverter.GetBytes((ushort)(levelact & 0xFF)));
                                datasection.AddRange(new Vertex(item.Value["Position"]).GetBytes());
                                datasection.AddRange(BitConverter.GetBytes(int.Parse(item.Value["YRotation"], System.Globalization.NumberStyles.HexNumber)));
                            }
                            datasection.AddRange(BitConverter.GetBytes((ushort)LevelIDs.Invalid));
                            datasection.AddRange(new byte[0x12]);
                            break;
                        case "texlist":
                            Dictionary<string, Dictionary<string, string>> texini = IniFile.Load(data["filename"]);
                            List<byte> texents = new List<byte>();
                            int texcnt = 0;
                            while (texini.ContainsKey(texcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = texini[texcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                if (string.IsNullOrEmpty(group["Name"]))
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
                            break;
                        case "leveltexlist":
                            Dictionary<string, Dictionary<string, string>> lvltxini = IniFile.Load(data["filename"]);
                            List<byte> lvltxents = new List<byte>();
                            int lvltxcnt = 0;
                            while (lvltxini.ContainsKey(lvltxcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = lvltxini[lvltxcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                if (string.IsNullOrEmpty(group["Name"]))
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
                            datasection.AddRange(BitConverter.GetBytes(ParseLevelAct(data["level"])));
                            datasection.AddRange(BitConverter.GetBytes((ushort)lvltxcnt));
                            datasection.AddRange(BitConverter.GetBytes(lvltxaddr));
                            break;
                        case "triallevellist":
                            uint headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                            string[] levels = File.ReadAllLines(data["filename"]);
                            int cnt = 0;
                            foreach (string line in levels)
                            {
                                if (string.IsNullOrEmpty(line)) continue;
                                ushort levelact = ParseLevelAct(line);
                                datasection.Add((byte)(levelact >> 8));
                                datasection.Add((byte)(levelact & 0xFF));
                                cnt++;
                            }
                            BitConverter.GetBytes(cnt).CopyTo(exefile, headaddr + 4);
                            break;
                        case "bosslevellist":
                            levels = File.ReadAllLines(data["filename"]);
                            foreach (string line in levels)
                            {
                                if (string.IsNullOrEmpty(line)) continue;
                                ushort levelact = ParseLevelAct(line);
                                datasection.AddRange(BitConverter.GetBytes((ushort)(levelact >> 8)));
                                datasection.AddRange(BitConverter.GetBytes((ushort)(levelact & 0xFF)));
                            }
                            datasection.AddRange(BitConverter.GetBytes((ushort)LevelIDs.Invalid));
                            datasection.AddRange(new byte[2]);
                            break;
                        case "fieldstartpos":
                            posini = IniFile.Load(data["filename"]);
                            foreach (KeyValuePair<string, Dictionary<string, string>> item in posini)
                            {
                                if (string.IsNullOrEmpty(item.Key)) continue;
                                datasection.AddRange(BitConverter.GetBytes((ushort)Enum.Parse(typeof(LevelIDs), item.Key)));
                                datasection.AddRange(BitConverter.GetBytes((ushort)Enum.Parse(typeof(LevelIDs), data["Field"])));
                                datasection.AddRange(new Vertex(data["Position"]).GetBytes());
                                datasection.AddRange(BitConverter.GetBytes(int.Parse(data["YRotation"], System.Globalization.NumberStyles.HexNumber)));
                            }
                            datasection.AddRange(BitConverter.GetBytes((ushort)LevelIDs.Invalid));
                            datasection.AddRange(new byte[0x12]);
                            break;
                        case "soundtestlist":
                            Dictionary<string, Dictionary<string, string>> soundini = new Dictionary<string, Dictionary<string, string>>();
                            headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            int soundcnt = 0;
                            List<byte> soundents = new List<byte>();
                            while (soundini.ContainsKey(soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = soundini[soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                if (string.IsNullOrEmpty(group["Name"]))
                                    soundents.AddRange(new byte[4]);
                                else
                                {
                                    soundents.AddRange(BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count));
                                    datasection.AddRange(jpenc.GetBytes(group["Name"]));
                                    datasection.Add(0);
                                    datasection.Align(4);
                                }
                                soundents.AddRange(BitConverter.GetBytes(uint.Parse(group["Track"], System.Globalization.NumberStyles.HexNumber)));
                                soundcnt++;
                            }
                            BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                            BitConverter.GetBytes(soundcnt).CopyTo(exefile, headaddr + 4);
                            datasection.AddRange(soundents.ToArray());
                            break;
                        case "musiclist":
                            Dictionary<string, Dictionary<string, string>> musini = new Dictionary<string, Dictionary<string, string>>();
                            headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            int muscnt = int.Parse(data["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            for (int i = 0; i < muscnt; i++)
                            {
                                if (musini.ContainsKey(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                                {
                                    Dictionary<string, string> group = musini[i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                    if (string.IsNullOrEmpty(group["Name"]))
                                        Array.Clear(exefile, (int)headaddr, 4);
                                    else
                                    {
                                        BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                        datasection.AddRange(jpenc.GetBytes(group["Name"]));
                                        datasection.Add(0);
                                        datasection.Align(4);
                                    }
                                    BitConverter.GetBytes(bool.Parse(group["Loop"]) ? 1 : 0).CopyTo(exefile, headaddr + 4);
                                }
                                else
                                    Array.Clear(exefile, (int)headaddr, 8);
                                headaddr += 8;
                            }
                            break;
                        case "soundlist":
                            soundini = new Dictionary<string, Dictionary<string, string>>();
                            headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            soundcnt = 0;
                            soundents = new List<byte>();
                            while (soundini.ContainsKey(soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = soundini[soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                soundents.AddRange(BitConverter.GetBytes(uint.Parse(group["Bank"], System.Globalization.NumberStyles.HexNumber)));
                                if (string.IsNullOrEmpty(group["Filename"]))
                                    soundents.AddRange(new byte[4]);
                                else
                                {
                                    soundents.AddRange(BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count));
                                    datasection.AddRange(jpenc.GetBytes(group["Filename"]));
                                    datasection.Add(0);
                                    datasection.Align(4);
                                }
                                soundcnt++;
                            }
                            BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                            BitConverter.GetBytes(soundcnt).CopyTo(exefile, headaddr + 4);
                            datasection.AddRange(soundents.ToArray());
                            break;
                        case "stringarray":
                            string[] strs = File.ReadAllLines(data["filename"]);
                            headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            cnt = int.Parse(data["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            for (int i = 0; i < cnt; i++)
                            {
                                if (i < strs.Length && !string.IsNullOrEmpty(strs[i]))
                                {
                                    BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                    datasection.AddRange(jpenc.GetBytes(strs[i].Replace("\\n", "\n")));
                                    datasection.Add(0);
                                    datasection.Align(4);
                                }
                                else
                                    Array.Clear(exefile, (int)headaddr, 4);
                                headaddr += 4;
                            }
                            break;
                        case "nextlevellist":
                            posini = IniFile.Load(data["filename"]);
                            soundcnt = 0;
                            while (posini.ContainsKey(soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = posini[soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                datasection.Add(byte.Parse(group["CGMovie"]));
                                datasection.Add((byte)Enum.Parse(typeof(LevelIDs), group["Level"]));
                                datasection.Add((byte)Enum.Parse(typeof(LevelIDs), group["NextLevel"]));
                                datasection.Add(byte.Parse(group["NextAct"]));
                                datasection.Add(byte.Parse(group["StartPos"]));
                                datasection.Add((byte)Enum.Parse(typeof(LevelIDs), group["AltNextLevel"]));
                                datasection.Add(byte.Parse(group["AltNextAct"]));
                                datasection.Add(byte.Parse(group["AltStartPos"]));
                                soundcnt++;
                            }
                            datasection.Add(0);
                            datasection.Add((byte)LevelIDs.Maximum);
                            datasection.AddRange(new byte[6]);
                            break;
                        case "cutscenetext":
                            headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            cnt = int.Parse(data["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            string[] hashes = data["md5"].Split(',');
                            for (int i = 0; i < 5; i++)
                            {
                                string textname = Path.Combine(data["filename"], ((Languages)i).ToString() + ".txt");
                                if (FileHash(textname) != hashes[i])
                                {
                                    strs = File.ReadAllLines(textname);
                                    for (int j = 0; j < cnt; j++)
                                    {
                                        if (j < strs.Length && !string.IsNullOrEmpty(strs[j]))
                                        {
                                            BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                            datasection.AddRange(jpenc.GetBytes(strs[j].Replace("\\n", "\n")));
                                            datasection.Add(0);
                                            datasection.Align(4);
                                        }
                                        else
                                            Array.Clear(exefile, (int)headaddr, 4);
                                        headaddr += 4;
                                    }
                                }
                            }
                            break;
                        case "levelclearflags":
                            levels = File.ReadAllLines(data["filename"]);
                            foreach (string line in levels)
                            {
                                if (string.IsNullOrEmpty(line)) continue;
                                datasection.AddRange(BitConverter.GetBytes((ushort)(LevelIDs)Enum.Parse(typeof(LevelIDs), line.Split(' ')[0])));
                                datasection.AddRange(BitConverter.GetBytes(ushort.Parse(line.Split(' ')[1], System.Globalization.NumberStyles.HexNumber)));
                            }
                            datasection.AddRange(BitConverter.GetBytes(uint.MaxValue));
                            break;
                        case "deathzone":
                            posini = IniFile.Load(data["filename"]);
                            string path = Path.GetDirectoryName(data["filename"]);
                            soundcnt = 0;
                            soundents = new List<byte>();
                            while (posini.ContainsKey(soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)))
                            {
                                Dictionary<string, string> group = posini[soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                curaddr = startaddress + imageBase + (uint)datasection.Count;
                                uint mdladdr;
                                datasection.AddRange(SonicRetro.SAModel.Object.LoadFromFile(Path.Combine(path, soundcnt.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl")).GetBytes(curaddr, ModelFormat.SADX, out mdladdr));
                                datasection.Align(4);
                                soundents.AddRange(BitConverter.GetBytes((int)Enum.Parse(typeof(CharacterFlags), group["Flags"])));
                                soundents.AddRange(BitConverter.GetBytes(curaddr + mdladdr));
                                soundcnt++;
                            }
                            dataaddr = startaddress + imageBase + (uint)datasection.Count;
                            datasection.AddRange(soundents.ToArray());
                            datasection.AddRange(new byte[8]);
                            break;
                        case "skyboxscale":
                            headaddr = uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber);
                            dataaddr = headaddr + imageBase;
                            cnt = int.Parse(data["count"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            posini = IniFile.Load(data["filename"]);
                            for (int i = 0; i < cnt; i++)
                            {
                                if (BitConverter.ToUInt32(exefile, (int)headaddr) != 0)
                                {
                                    int ptr = (int)(BitConverter.ToUInt32(exefile, (int)headaddr) - imageBase);
                                    Dictionary<string, string> objgrp = posini[i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo)];
                                    new Vertex(objgrp["Far"]).GetBytes().CopyTo(exefile, ptr);
                                    ptr += Vertex.Size;
                                    new Vertex(objgrp["Normal"]).GetBytes().CopyTo(exefile, ptr);
                                    ptr += Vertex.Size;
                                    new Vertex(objgrp["Near"]).GetBytes().CopyTo(exefile, ptr);
                                }
                                headaddr += 4;
                            }
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
                    Console.WriteLine(filedesc + ": " + (dataaddr - imageBase).ToString("X"));
                    datasection.Align(4);
                    curaddr = startaddress + imageBase + (uint)datasection.Count;
                    dataaddr = curaddr;
                    itemcount++;
                }
                else
                    addresses.Add(filedesc, uint.Parse(data["address"], System.Globalization.NumberStyles.HexNumber));
            }
            if (datasection.Count > 0)
                CreateNewSection(ref exefile, ".data2", datasection.ToArray(), false);
            if (File.Exists("postbuild.cs"))
            {
                CodeDomProvider pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", System.Reflection.Assembly.GetAssembly(typeof(LandTable)).Location });
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
            timer.Stop();
            Console.WriteLine("Built " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
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
            int off = me.Count % bytes;
            if (off == 0) return;
            me.AddRange(new byte[bytes - off]);
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

        static ushort ParseLevelAct(string levelact)
        {
            if (levelact.Contains(" "))
            {
                return (ushort)(((byte)Enum.Parse(typeof(LevelIDs), levelact.Split(' ')[0]) << 8) + byte.Parse(levelact.Split(' ')[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            else
                return (ushort)((byte.Parse(levelact.Substring(0, 2), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo) << 8) | byte.Parse(levelact.Substring(2, 2), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
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

    [Flags()]
    enum CharacterFlags
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

    enum Languages
    {
        Japanese = 0,
        English = 1,
        French = 2,
        Spanish = 3,
        German = 4
    }
}