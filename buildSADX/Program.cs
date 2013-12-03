using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using SADXPCTools;
using SonicRetro.SAModel;

namespace buildPE
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
            uint imageBase = HelperFunctions.SetupEXE(ref exefile).Value;
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(exefilename));
            Dictionary<string, uint> addresses = new Dictionary<string, uint>();
            string inistartpath = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(exefilename));
            IniData inifile = IniFile.Deserialize<IniData>(inistartpath + "_data.ini");
            bool SA2 = inifile.Game == Game.SA2 | inifile.Game == Game.SA2B;
            ModelFormat modelfmt = 0;
            LandTableFormat landfmt = 0;
            switch (inifile.Game)
            {
                case Game.SA1:
                    modelfmt = ModelFormat.Basic;
                    landfmt = LandTableFormat.SA1;
                    break;
                case Game.SADX:
                    modelfmt = ModelFormat.BasicDX;
                    landfmt = LandTableFormat.SADX;
                    break;
                case Game.SA2:
                    modelfmt = ModelFormat.Chunk;
                    landfmt = LandTableFormat.SA2;
                    break;
                case Game.SA2B:
                    modelfmt = ModelFormat.Chunk;
                    landfmt = LandTableFormat.SA2B;
                    break;
            }
            uint startaddress = HelperFunctions.GetNewSectionAddress(exefile);
            uint curaddr = startaddress;
            int itemcount = 0;
            TextureCollection textures = new TextureCollection();
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
                            int staddr = int.Parse(item.Split('-')[0], NumberStyles.HexNumber);
                            int endaddr = int.Parse(item.Split('-')[1], NumberStyles.HexNumber);
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
                            int i = int.Parse(item, NumberStyles.HexNumber);
                            exefile[i] = 0xE8;
                            int disp = (int)curaddr - (i + 5);
                            BitConverter.GetBytes(disp).CopyTo(exefile, i + 1);
                        }
                    if (data.ContainsKey("jmp"))
                        foreach (string item in data["jmp"].Split(','))
                        {
                            int i = int.Parse(item, NumberStyles.HexNumber);
                            exefile[i] = 0xE9;
                            int disp = (int)curaddr - (i + 5);
                            BitConverter.GetBytes(disp).CopyTo(exefile, i + 1);
                        }
                    if (data.ContainsKey("pointer"))
                        foreach (string item in data["pointer"].Split(','))
                        {
                            int i = int.Parse(item, NumberStyles.HexNumber);
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
                            datasection.AddRange(tbl.GetBytes(curaddr, landfmt, out dataaddr));
                            //tbl = new LandTable(datasection.ToArray(), (int)dataaddr, curaddr, ModelFormat.SADX); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "model":
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                            datasection.AddRange(mdl.GetBytes(curaddr, modelfmt == ModelFormat.BasicDX, out dataaddr));
                            //mdl = new SonicRetro.SAModel.Object(datasection.ToArray(), (int)dataaddr, curaddr, ModelFormat.SADX); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "basicmodel":
                            mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                            datasection.AddRange(mdl.GetBytes(curaddr, false, out dataaddr));
                            //mdl = new SonicRetro.SAModel.Object(datasection.ToArray(), (int)dataaddr, curaddr, ModelFormat.SA1); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "basicdxmodel":
                            mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                            datasection.AddRange(mdl.GetBytes(curaddr, true, out dataaddr));
                            //mdl = new SonicRetro.SAModel.Object(datasection.ToArray(), (int)dataaddr, curaddr, ModelFormat.SA1); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "chunkmodel":
                            mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                            datasection.AddRange(mdl.GetBytes(curaddr, false, out dataaddr));
                            //mdl = new SonicRetro.SAModel.Object(datasection.ToArray(), (int)dataaddr, curaddr, ModelFormat.SA2); //sanity check
                            dataaddr += curaddr;
                            break;
                        case "action":
                            Animation ani = Animation.Load(data.Filename);
                            datasection.AddRange(ani.WriteHeader(curaddr, addresses[data.CustomProperties["model"]], out dataaddr));
                            dataaddr += curaddr;
                            break;
                        case "animation":
                            ani = Animation.Load(data.Filename);
                            datasection.AddRange(ani.GetBytes(curaddr, out dataaddr));
                            dataaddr += curaddr;
                            break;
                        case "objlist":
                            datasection.AddRange(ObjectList.Load(data.Filename, SA2).GetBytes(startaddress + imageBase, addresses, out dataaddr));
                            break;
                        case "startpos":
                            if (SA2)
                                datasection.AddRange(SA2StartPosList.Load(data.Filename).GetBytes());
                            else
                                datasection.AddRange(SA1StartPosList.Load(data.Filename).GetBytes());
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
                                SA1LevelAct[] levels = TrialLevelList.Load(data.Filename);
                                foreach (SA1LevelAct line in levels)
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
                                        int textaddr = exefile.GetPointer(headaddr + (i * 4), imageBase);
                                        string[] strs = texts.Text[i];
                                        for (int j = 0; j < cnt; j++)
                                        {
                                            if (j < strs.Length && !string.IsNullOrEmpty(strs[j]))
                                            {
                                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, textaddr);
                                                datasection.AddRange(HelperFunctions.GetEncoding((Languages)i).GetBytes(strs[j]));
                                                datasection.Add(0);
                                                datasection.Align(4);
                                            }
                                            else
                                                Array.Clear(exefile, textaddr, 4);
                                            textaddr += 4;
                                        }
                                    }
                                }
                            }
                            break;
                        case "recapscreen":
                            {
                                RecapScreen[][] texts = RecapScreenList.Load(data.Filename, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                string[] hash2 = data.MD5Hash.Split(':');
                                string[][] hashes = new string[hash2.Length][];
                                for (int i = 0; i < hash2.Length; i++)
                                    hashes[i] = hash2[i].Split(',');
                                for (int i = 0; i < texts.Length; i++)
                                    for (int l = 0; l < 5; l++)
                                    {
                                        int tmpaddr = exefile.GetPointer(headaddr + (l * 4), imageBase);
                                        tmpaddr += i * 8;
                                        string textname = Path.Combine(Path.Combine(data.Filename, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
                                        if (HelperFunctions.FileHash(textname) != hashes[i][l])
                                        {
                                            string[] strs = texts[i][l].Text.Split('\n');
                                            BitConverter.GetBytes(texts[i][l].Speed).CopyTo(exefile, tmpaddr);
                                            if (BitConverter.ToInt32(exefile, tmpaddr + 4) >= strs.Length)
                                            {
                                                BitConverter.GetBytes(strs.Length).CopyTo(exefile, tmpaddr + 4);
                                                tmpaddr = exefile.GetPointer(tmpaddr + 8, imageBase);
                                                for (int j = 0; j < strs.Length; j++)
                                                {
                                                    if (!string.IsNullOrEmpty(strs[j]))
                                                    {
                                                        BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, tmpaddr);
                                                        datasection.AddRange(HelperFunctions.GetEncoding((Languages)l).GetBytes(strs[j]));
                                                        datasection.Add(0);
                                                        datasection.Align(4);
                                                    }
                                                    else
                                                        BitConverter.GetBytes(0x7DDD97u).CopyTo(exefile, tmpaddr);
                                                    tmpaddr += 4;
                                                }
                                            }
                                            else
                                            {
                                                BitConverter.GetBytes(strs.Length).CopyTo(exefile, tmpaddr + 4);
                                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, tmpaddr + 8);
                                                uint straddr = startaddress + imageBase + (uint)datasection.Count + (uint)(strs.Length * 4);
                                                List<byte> strbytes = new List<byte>();
                                                for (int j = 0; j < strs.Length; j++)
                                                {
                                                    if (!string.IsNullOrEmpty(strs[j]))
                                                    {
                                                        datasection.AddRange(BitConverter.GetBytes(straddr + (uint)strbytes.Count));
                                                        strbytes.AddRange(HelperFunctions.GetEncoding((Languages)l).GetBytes(strs[j]));
                                                        strbytes.Add(0);
                                                        strbytes.Align(4);
                                                    }
                                                    else
                                                        datasection.AddRange(BitConverter.GetBytes(0x7DDD97u));
                                                }
                                                datasection.AddRange(strbytes);
                                            }
                                        }
                                    }
                            }
                            break;
                        case "npctext":
                            {
                                NPCText[][] texts = NPCTextList.Load(data.Filename, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                string[] hash2 = data.MD5Hash.Split(':');
                                string[][] hashes = new string[hash2.Length][];
                                for (int i = 0; i < hash2.Length; i++)
                                    hashes[i] = hash2[i].Split(',');
                                for (int l = 0; l < 5; l++)
                                    for (int i = 0; i < texts[l].Length; i++)
                                    {
                                        int tmpaddr = exefile.GetPointer(headaddr + (l * 4), imageBase);
                                        tmpaddr += i * 8;
                                        string textname = Path.Combine(Path.Combine(data.Filename, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
                                        if (HelperFunctions.FileHash(textname) != hashes[l][i])
                                        {
                                            if (texts[l][i].Groups.Count == 0)
                                            {
                                                Array.Clear(exefile, tmpaddr, 8);
                                                continue;
                                            }
                                            if (texts[l][i].HasControl)
                                            {
                                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, tmpaddr);
                                                bool first = true;
                                                foreach (NPCTextGroup group in texts[l][i].Groups)
                                                {
                                                    if (!first)
                                                        datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.NewGroup));
                                                    else
                                                        first = false;
                                                    foreach (ushort flag in group.EventFlags)
                                                    {
                                                        datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.EventFlag));
                                                        datasection.AddRange(BitConverter.GetBytes(flag));
                                                    }
                                                    foreach (ushort flag in group.NPCFlags)
                                                    {
                                                        datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.NPCFlag));
                                                        datasection.AddRange(BitConverter.GetBytes(flag));
                                                    }
                                                    if (group.Character != (SA1CharacterFlags)0xFF)
                                                    {
                                                        datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.Character));
                                                        datasection.AddRange(BitConverter.GetBytes((ushort)group.Character));
                                                    }
                                                    if (group.Voice.HasValue)
                                                    {
                                                        datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.Voice));
                                                        datasection.AddRange(BitConverter.GetBytes(group.Voice.Value));
                                                    }
                                                    if (group.SetEventFlag.HasValue)
                                                    {
                                                        datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.SetEventFlag));
                                                        datasection.AddRange(BitConverter.GetBytes(group.SetEventFlag.Value));
                                                    }
                                                }
                                                datasection.AddRange(BitConverter.GetBytes((short)NPCTextControl.End));
                                            }
                                            else
                                                Array.Clear(exefile, tmpaddr, 4);
                                            if (texts[l][i].HasText)
                                            {
                                                List<byte> textptrs = new List<byte>();
                                                foreach (NPCTextGroup group in texts[l][i].Groups)
                                                {
                                                    foreach (NPCTextLine line in group.Lines)
                                                    {
                                                        textptrs.AddRange(BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count));
                                                        textptrs.AddRange(BitConverter.GetBytes(line.Time));
                                                        datasection.AddRange(HelperFunctions.GetEncoding((Languages)l).GetBytes(line.Line));
                                                        datasection.Add(0);
                                                        int off = datasection.Count % 4;
                                                        if (off != 0)
                                                            datasection.AddRange(new byte[4 - off]);
                                                    }
                                                    textptrs.AddRange(new byte[8]);
                                                }
                                                BitConverter.GetBytes(startaddress + imageBase + (uint)datasection.Count).CopyTo(exefile, tmpaddr + 4);
                                                datasection.AddRange(textptrs);
                                            }
                                            else
                                                Array.Clear(exefile, tmpaddr + 4, 4);
                                        }
                                    }
                            }
                            break;
                        case "levelclearflags":
                            datasection.AddRange(LevelClearFlagList.Load(data.Filename).GetBytes());
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
                                    datasection.AddRange(new ModelFile(Path.Combine(path, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl")).Model.GetBytes(curaddr, true, out mdladdr));
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
                                int cnt = int.Parse(data.CustomProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                SkyboxScale[] sclini = SkyboxScaleList.Load(data.Filename);
                                for (int i = 0; i < cnt; i++)
                                {
                                    if (BitConverter.ToUInt32(exefile, (int)headaddr) != 0)
                                        sclini[i].GetBytes().CopyTo(exefile, exefile.GetPointer((int)headaddr, imageBase));
                                    headaddr += 4;
                                }
                            }
                            break;
                        case "stageselectlist":
                            {
                                int headaddr = data.Address;
                                dataaddr = (uint)(headaddr + imageBase);
                                int cnt = int.Parse(data.CustomProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                StageSelectLevel[] sclini = StageSelectLevelList.Load(data.Filename);
                                for (int i = 0; i < cnt; i++)
                                {
                                    sclini[i].GetBytes().CopyTo(exefile, (int)headaddr);
                                    headaddr += StageSelectLevel.Size;
                                }
                            }
                            break;
                        case "levelrankscores":
                            datasection.AddRange(LevelRankScoresList.Load(data.Filename).GetBytes());
                            break;
                        case "levelranktimes":
                            datasection.AddRange(LevelRankTimesList.Load(data.Filename).GetBytes());
                            break;
                        case "endpos":
                            datasection.AddRange(SA2EndPosList.Load(data.Filename).GetBytes());
                            break;
                        case "animationlist":
                            datasection.AddRange(SA2AnimationInfoList.Load(data.Filename).GetBytes());
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
                switch (data.Type)
                {
                    case "texlist":
                        foreach (TextureListEntry item in TextureList.Load(data.Filename))
                            if (!textures.Contains(item.Textures))
                                textures.Add(item);
                        break;
                    case "leveltexlist":
                        foreach (TextureListEntry item in LevelTextureList.Load(data.Filename).TextureList)
                            if (!textures.Contains(item.Textures))
                                textures.Add(item);
                        break;
                }
            }
            string systemfolder = Path.Combine(Environment.CurrentDirectory, inifile.SystemFolder);
            foreach (TextureListEntry item in textures)
            {
                if (item.Textures == 0 | string.IsNullOrEmpty(item.Name)) continue;
                curaddr = startaddress + imageBase + (uint)datasection.Count;
                switch (inifile.Game)
                {
                    case Game.SADX:
                        string namebase = Path.Combine(systemfolder, item.Name);
                        Stream pvmdata = new MemoryStream(File.ReadAllBytes(namebase + ".pvm"));
                        PuyoTools.ArchiveModule pvmfile = new PuyoTools.PVM();
                        pvmdata = pvmfile.TranslateData(pvmdata);
                        PuyoTools.ArchiveFileList pvmentries = pvmfile.GetFileList(pvmdata);
                        if (pvmentries.Entries.Length != item.Count)
                        {
                            Console.WriteLine(item.Name + ": " + (curaddr - imageBase).ToString("X"));
                            itemcount++;
                            int headaddr = (int)(item.Textures - imageBase);
                            BitConverter.GetBytes(curaddr).CopyTo(exefile, headaddr);
                            BitConverter.GetBytes(pvmentries.Entries.Length).CopyTo(exefile, headaddr + 4);
                            datasection.AddRange(new byte[12 * pvmentries.Entries.Length]);
                        }
                        break;
                    case Game.SA2B:
                        if (File.Exists(Path.Combine(Path.Combine(systemfolder, "PRS"), item.Name) + ".pak"))
                        {
                            PAKLib.PAKFile pak = new PAKLib.PAKFile(Path.Combine(Path.Combine(systemfolder, "PRS"), item.Name) + ".pak");
                            foreach (PAKLib.PAKFile.File file in pak.Files)
                                if (file.Name == item.Name + "\\" + item.Name + ".inf")
                                    if (file.Data.Length / 0x3C != item.Count)
                                    {
                                        Console.WriteLine(item.Name + ": " + (curaddr - imageBase).ToString("X"));
                                        itemcount++;
                                        int headaddr = (int)(item.Textures - imageBase);
                                        BitConverter.GetBytes(curaddr).CopyTo(exefile, headaddr);
                                        BitConverter.GetBytes(file.Data.Length / 0x3C).CopyTo(exefile, headaddr + 4);
                                        datasection.AddRange(new byte[12 * (file.Data.Length / 0x3C)]);
                                    }
                        }
                        else
                        {
                            namebase = Path.Combine(systemfolder, item.Name);
                            pvmdata = new MemoryStream(FraGag.Compression.Prs.Decompress(namebase + ".prs"));
                            pvmfile = new PuyoTools.GVM();
                            pvmdata = pvmfile.TranslateData(pvmdata);
                            pvmentries = pvmfile.GetFileList(pvmdata);
                            if (pvmentries.Entries.Length != item.Count)
                            {
                                Console.WriteLine(item.Name + ": " + (curaddr - imageBase).ToString("X"));
                                itemcount++;
                                int headaddr = (int)(item.Textures - imageBase);
                                BitConverter.GetBytes(curaddr).CopyTo(exefile, headaddr);
                                BitConverter.GetBytes(pvmentries.Entries.Length).CopyTo(exefile, headaddr + 4);
                                datasection.AddRange(new byte[12 * pvmentries.Entries.Length]);
                            }
                        }
                        break;
                }
            }
            if (datasection.Count > 0)
                HelperFunctions.CreateNewSection(ref exefile, ".data2", datasection.ToArray(), false);
            if (File.Exists("postbuild.cs"))
            {
                CodeDomProvider pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetAssembly(typeof(LandTable)).Location });
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
            HelperFunctions.CompactEXE(ref exefile);
            File.WriteAllBytes(inistartpath + "_edit.exe", exefile);
            timer.Stop();
            Console.WriteLine("Built " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
            Console.Write("Press a key to continue...");
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }

    internal class TextureCollection : System.Collections.ObjectModel.KeyedCollection<uint, TextureListEntry>
    {
        protected override uint GetKeyForItem(TextureListEntry item)
        {
            return item.Textures;
        }
    }
}