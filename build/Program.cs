using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.CodeDom.Compiler;
using SonicRetro.SAModel;
using SADXPCTools;
using System.Globalization;

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
            HelperFunctions.SetupEXE(ref exefile);
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(exefilename));
            Dictionary<string, uint> addresses = new Dictionary<string, uint>();
            Dictionary<string, int> modelanims = new Dictionary<string, int>();
            string inistartpath = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(exefilename));
            IniData inifile = IniFile.Deserialize<IniData>(inistartpath + "_data.ini");
            uint imageBase = inifile.ImageBase;
            uint startaddress = HelperFunctions.GetNewSectionAddress(exefile);
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
                HelperFunctions.CreateNewSection(ref exefile, ".text2", codesection.ToArray(), true);
            }
            List<byte> datasection = new List<byte>();
            startaddress = HelperFunctions.GetNewSectionAddress(exefile);
            curaddr = startaddress + imageBase;
            uint dataaddr = curaddr;
            foreach (KeyValuePair<string, SADXPCTools.FileInfo> dictitem in inifile.Files)
            {
                string filedesc = dictitem.Key;
                SADXPCTools.FileInfo data = dictitem.Value;
                string type = data.Type;
                bool nohash = data.NoHash;
                if (nohash || HelperFunctions.FileHash(data.Filename) != data.MD5Hash)
                {
                    switch (type)
                    {
                        case "landtable":
                            LandTable tbl = LandTable.LoadFromFile(data.Filename);
                            datasection.AddRange(tbl.GetBytes(curaddr, ModelFormat.SADX, out dataaddr));
                            //tbl = new LandTable(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "model":
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                            modelanims.Add(filedesc, mdl.CountAnimated());
                            datasection.AddRange(mdl.GetBytes(curaddr, ModelFormat.SADX, out dataaddr));
                            //mdl = new Object(datasection.ToArray(), (int)dataaddr, curaddr, true); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "animation":
                            Animation ani = Animation.Load(data.Filename, modelanims[data.CustomProperties["model"]]);
                            datasection.AddRange(ani.WriteHeader(curaddr, addresses[data.CustomProperties["model"]], out dataaddr));
                            dataaddr += curaddr;
                            break;
                        case "objlist":
                            datasection.AddRange(ObjectList.Load(data.Filename).GetBytes(startaddress + imageBase, addresses, out dataaddr));
                            break;
                        case "startpos":
                            datasection.AddRange(StartPosList.Load(data.Filename).GetBytes());
                            break;
                        case "texlist":
                            datasection.AddRange(TextureList.Load(data.Filename).GetBytes(startaddress + imageBase, addresses, out dataaddr));
                            break;
                        case "leveltexlist":
                            datasection.AddRange(LevelTextureList.Load(data.Filename).GetBytes(startaddress + imageBase, out dataaddr));
                            break;
                        case "triallevellist":
                            {
                                uint headaddr = (uint)data.Address;
                                dataaddr = headaddr + imageBase;
                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                LevelAct[] levels = TrialLevelList.Load(data.Filename);
                                foreach (LevelAct line in levels)
                                {
                                    datasection.Add((byte)line.Level);
                                    datasection.Add(line.Act);
                                }
                                BitConverter.GetBytes(levels.Length).CopyTo(exefile, headaddr + 4);
                            }
                            break;
                        case "bosslevellist":
                            datasection.AddRange(BossLevelList.GetBytes(BossLevelList.Load(data.Filename)));
                            break;
                        case "fieldstartpos":
                            datasection.AddRange(FieldStartPosList.Load(data.Filename).GetBytes());
                            break;
                        case "soundtestlist":
                            {
                                SoundTestListEntry[] soundini = SoundTestList.Load(data.Filename);
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                int soundcnt = 0;
                                List<byte> soundents = new List<byte>();
                                for (int i = 0; i < soundini.Length; i++)
                                {
                                    uint titleaddr = 0;
                                    if (!string.IsNullOrEmpty(soundini[i].Title))
                                    {
                                        titleaddr = startaddress + imageBase + (uint)datasection.Count;
                                        datasection.AddRange(HelperFunctions.GetEncoding().GetBytes(soundini[i].Title));
                                        datasection.Add(0);
                                        datasection.Align(4);
                                    }
                                    soundents.AddRange(soundini[i].GetBytes(titleaddr));
                                }
                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                BitConverter.GetBytes(soundcnt).CopyTo(exefile, headaddr + 4);
                                datasection.AddRange(soundents.ToArray());
                            }
                            break;
                        case "musiclist":
                            {
                                MusicListEntry[] musini = MusicList.Load(data.Filename);
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                int muscnt = int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                for (int i = 0; i < muscnt; i++)
                                {
                                    if (string.IsNullOrEmpty(musini[i].Filename))
                                        Array.Clear(exefile, (int)headaddr, 4);
                                    else
                                    {
                                        BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                        datasection.AddRange(HelperFunctions.GetEncoding().GetBytes(musini[i].Filename));
                                        datasection.Add(0);
                                        datasection.Align(4);
                                    }
                                    BitConverter.GetBytes(musini[i].Loop ? 1 : 0).CopyTo(exefile, headaddr + 4);
                                    headaddr += 8;
                                }
                            }
                            break;
                        case "soundlist":
                            {
                                SoundListEntry[] soundini = SoundList.Load(data.Filename);
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                List<byte> soundents = new List<byte>();
                                for (int i = 0; i < soundini.Length; i++)
                                {
                                    uint nameaddr = 0;
                                    if (!string.IsNullOrEmpty(soundini[i].Filename))
                                    {
                                        nameaddr = startaddress + imageBase + (uint)datasection.Count;
                                        datasection.AddRange(HelperFunctions.GetEncoding().GetBytes(soundini[i].Filename));
                                        datasection.Add(0);
                                        datasection.Align(4);
                                    }
                                    soundents.AddRange(soundini[i].GetBytes(nameaddr));
                                }
                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                BitConverter.GetBytes(soundini.Length).CopyTo(exefile, headaddr + 4);
                                datasection.AddRange(soundents.ToArray());
                            }
                            break;
                        case "stringarray":
                            {
                                string[] strs = StringArray.Load(data.Filename);
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                int cnt = int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                Languages lang = Languages.Japanese;
                                if (data.CustomProperties.ContainsKey("language"))
                                    lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
                                for (int i = 0; i < cnt; i++)
                                {
                                    if (i < strs.Length && !string.IsNullOrEmpty(strs[i]))
                                    {
                                        BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                        datasection.AddRange(HelperFunctions.GetEncoding(lang).GetBytes(strs[i]));
                                        datasection.Add(0);
                                        datasection.Align(4);
                                    }
                                    else
                                        Array.Clear(exefile, (int)headaddr, 4);
                                    headaddr += 4;
                                }
                            }
                            break;
                        case "nextlevellist":
                            datasection.AddRange(NextLevelList.Load(data.Filename).GetBytes());
                            break;
                        case "cutscenetext":
                            {
                                CutsceneText texts = new CutsceneText(data.Filename);
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                int cnt = int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                string[] hashes = data.MD5Hash.Split(',');
                                for (int i = 0; i < 5; i++)
                                {
                                    string textname = Path.Combine(data.Filename, ((Languages)i).ToString() + ".txt");
                                    if (HelperFunctions.FileHash(textname) != hashes[i])
                                    {
                                        string[] strs = texts.Text[i];
                                        for (int j = 0; j < cnt; j++)
                                        {
                                            if (j < strs.Length && !string.IsNullOrEmpty(strs[j]))
                                            {
                                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, headaddr);
                                                datasection.AddRange(HelperFunctions.GetEncoding((Languages)i).GetBytes(strs[j]));
                                                datasection.Add(0);
                                                datasection.Align(4);
                                            }
                                            else
                                                Array.Clear(exefile, (int)headaddr, 4);
                                            headaddr += 4;
                                        }
                                    }
                                }
                            }
                            break;
                        case "levelclearflags":
                            datasection.AddRange(LevelClearFlagList.GetBytes(LevelClearFlagList.Load(data.Filename)));
                            break;
                        case "deathzone":
                            {
                                DeathZoneFlags[] dzs = DeathZoneFlagsList.Load(data.Filename);
                                string path = Path.GetDirectoryName(data.Filename);
                                List<byte> dzents = new List<byte>();
                                for (int i = 0; i < dzs.Length; i++)
                                {
                                    curaddr = startaddress + imageBase + (uint)datasection.Count;
                                    uint mdladdr;
                                    datasection.AddRange(new ModelFile(Path.Combine(path, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl")).Model.GetBytes(curaddr, ModelFormat.SADX, out mdladdr));
                                    datasection.Align(4);
                                    dzents.AddRange(dzs[i].GetBytes());
                                    dzents.AddRange(BitConverter.GetBytes(curaddr + mdladdr));
                                }
                                dataaddr = startaddress + imageBase + (uint)datasection.Count;
                                datasection.AddRange(dzents.ToArray());
                                datasection.AddRange(new byte[8]);
                            }
                            break;
                        case "skyboxscale":
                            {
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                int cnt = int.Parse(data.CustomProperties["count"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                                SkyboxScale[] sclini = SkyboxScaleList.Load(data.Filename);
                                for (int i = 0; i < cnt; i++)
                                {
                                    if (BitConverter.ToUInt32(exefile, (int)headaddr) != 0)
                                        sclini[i].GetBytes().CopyTo(exefile, (int)(BitConverter.ToUInt32(exefile, (int)headaddr) - imageBase));
                                    headaddr += 4;
                                }
                            }
                            break;
                        default: // raw binary
                            {
                                bool reloc = true;
                                if (data.CustomProperties.ContainsKey("dontrelocate"))
                                    reloc = !bool.Parse(data.CustomProperties["dontrelocate"]);
                                byte[] file = File.ReadAllBytes(data.Filename);
                                if (reloc)
                                    datasection.AddRange(file);
                                else
                                    Array.Copy(file, 0, exefile, data.Address, file.Length);
                            }
                            break;
                    }
                    if (data.PointerList != null)
                        foreach (int item in data.PointerList)
                            BitConverter.GetBytes(dataaddr).CopyTo(exefile, item);
                    addresses.Add(filedesc, dataaddr);
                    Console.WriteLine(filedesc + ": " + (dataaddr - imageBase).ToString("X"));
                    datasection.Align(4);
                    curaddr = startaddress + imageBase + (uint)datasection.Count;
                    dataaddr = curaddr;
                    itemcount++;
                }
                else
                    addresses.Add(filedesc, (uint)data.Address + imageBase);
            }
            if (datasection.Count > 0)
                HelperFunctions.CreateNewSection(ref exefile, ".data2", datasection.ToArray(), false);
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
                        .GetMethod("PostBuildMethod", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.InvokeMethod, null, System.Reflection.CallingConventions.Standard, new Type[] { typeof(uint), typeof(byte[]), typeof(Dictionary<string, uint>) }, null)
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
    }
}