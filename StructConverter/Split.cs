using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using SA_Tools;
using SonicRetro.SAModel;

namespace ModGenerator
{
    public static class Split
    {
        enum ERRORVALUE { Success = 0, NoProject = -1, NoSourceFile = -1, NoDataMapping = -3, UnhandledException = -4 }

        public static bool EXEFile(string[] args, out string errorMessage)
        {
            string datafilename, inifilename, projectFolderName;
            if (args.Length > 0)
            {
                datafilename = args[0];
                Console.WriteLine("File: {0}", datafilename);
            }
            else
            {
                errorMessage = "No source file supplied. Aborting.";
                return false;
            }
            if (args.Length > 1)
            {
                inifilename = args[1];
                Console.WriteLine("INI File: {0}", inifilename);
            }
            else
            {
                errorMessage = "No data mapping file supplied (expected ini). Aborting.";
                return false;
            }
            if (args.Length > 2)
            {
                projectFolderName = args[2];
                Console.WriteLine("Project Folder: {0}", projectFolderName);
            }
            else
            {
                errorMessage = "No project folder supplied. Aborting.";
                return false;
            }

//#if !DEBUG 
            try
            {
//#endif
                byte[] datafile = File.ReadAllBytes(datafilename);
                IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
                SA_Tools.ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = inifile.BigEndian;
                Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(datafilename));
                if (inifile.Compressed) datafile = FraGag.Compression.Prs.Decompress(datafile);
                uint imageBase = HelperFunctions.SetupEXE(ref datafile) ?? inifile.ImageBase.Value;
                if (Path.GetExtension(datafilename).Equals(".rel", StringComparison.OrdinalIgnoreCase)) HelperFunctions.FixRELPointers(datafile);
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
                    case Game.SA2B:
                        modelfmt = ModelFormat.Chunk;
                        landfmt = LandTableFormat.SA2;
                        break;
                }
                int itemcount = 0;
                Dictionary<string, MasterObjectListEntry> masterobjlist = new Dictionary<string, MasterObjectListEntry>();
                Dictionary<string, Dictionary<string, int>> objnamecounts = new Dictionary<string, Dictionary<string, int>>();
                Stopwatch timer = new Stopwatch();
                timer.Start();
                foreach (KeyValuePair<string, SA_Tools.FileInfo> item in inifile.Files)
                {
                    if (string.IsNullOrEmpty(item.Key)) continue;
                    string filedesc = item.Key;
                    SA_Tools.FileInfo data = item.Value;
                    Dictionary<string, string> customProperties = data.CustomProperties;
                    string type = data.Type;
                    int address = data.Address;
                    bool nohash = false;
                    string fileOutputPath = string.Concat(projectFolderName, data.Filename);
                    Console.WriteLine(item.Key + ": " + data.Address.ToString("X") + " ? " + fileOutputPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
                    switch (type)
                    {
                        case "landtable":
                            new LandTable(datafile, address, imageBase, landfmt) { Description = item.Key, Tool = "split" }.SaveToFile(fileOutputPath, landfmt);
                            break;
                        case "model":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, modelfmt);
                                string[] mdlanis = new string[0];
                                if (customProperties.ContainsKey("animations"))
                                    mdlanis = customProperties["animations"].Split(',');
                                string[] mdlmorphs = new string[0];
                                if (customProperties.ContainsKey("morphs"))
                                    mdlmorphs = customProperties["morphs"].Split(',');
                                ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, modelfmt);
                            }
                            break;
                        case "basicmodel":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, ModelFormat.Basic);
                                string[] mdlanis = new string[0];
                                if (customProperties.ContainsKey("animations"))
                                    mdlanis = customProperties["animations"].Split(',');
                                string[] mdlmorphs = new string[0];
                                if (customProperties.ContainsKey("morphs"))
                                    mdlmorphs = customProperties["morphs"].Split(',');
                                ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, ModelFormat.Basic);
                            }
                            break;
                        case "basicdxmodel":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, ModelFormat.BasicDX);
                                string[] mdlanis = new string[0];
                                if (customProperties.ContainsKey("animations"))
                                    mdlanis = customProperties["animations"].Split(',');
                                string[] mdlmorphs = new string[0];
                                if (customProperties.ContainsKey("morphs"))
                                    mdlmorphs = customProperties["morphs"].Split(',');
                                ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, ModelFormat.BasicDX);
                            }
                            break;
                        case "chunkmodel":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, ModelFormat.Chunk);
                                string[] mdlanis = new string[0];
                                if (customProperties.ContainsKey("animations"))
                                    mdlanis = customProperties["animations"].Split(',');
                                string[] mdlmorphs = new string[0];
                                if (customProperties.ContainsKey("morphs"))
                                    mdlmorphs = customProperties["morphs"].Split(',');
                                ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, ModelFormat.Chunk);
                            }
                            break;
                        case "action":
                            {
                                AnimationHeader ani = new AnimationHeader(datafile, address, imageBase, modelfmt);
                                ani.Animation.Name = filedesc;
                                ani.Animation.Save(fileOutputPath);
                            }
                            break;
                        case "animation":
                            new Animation(datafile, address, imageBase, int.Parse(customProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo)) { Name = filedesc }
                                .Save(fileOutputPath);
                            break;
                        case "objlist":
                            {
                                ObjectListEntry[] objs = ObjectList.Load(datafile, address, imageBase, SA2);
                                if (inifile.MasterObjectList != null)
                                    foreach (ObjectListEntry obj in objs)
                                    {
                                        if (!masterobjlist.ContainsKey(obj.CodeString))
                                            masterobjlist.Add(obj.CodeString, new MasterObjectListEntry(obj));
                                        if (!objnamecounts.ContainsKey(obj.CodeString))
                                            objnamecounts.Add(obj.CodeString, new Dictionary<string, int>() { { obj.Name, 1 } });
                                        else if (!objnamecounts[obj.CodeString].ContainsKey(obj.Name))
                                            objnamecounts[obj.CodeString].Add(obj.Name, 1);
                                        else
                                            objnamecounts[obj.CodeString][obj.Name]++;
                                    }
                                objs.Save(fileOutputPath);
                            }
                            break;
                        case "startpos":
                            if (SA2)
                                SA2StartPosList.Load(datafile, address).Save(fileOutputPath);
                            else
                                SA1StartPosList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        case "texlist":
                            TextureList.Load(datafile, address, imageBase).Save(fileOutputPath);
                            break;
                        case "leveltexlist":
                            new LevelTextureList(datafile, address, imageBase).Save(fileOutputPath);
                            break;
                        case "triallevellist":
                            TrialLevelList.Save(TrialLevelList.Load(datafile, address, imageBase), fileOutputPath);
                            break;
                        case "bosslevellist":
                            BossLevelList.Save(BossLevelList.Load(datafile, address), fileOutputPath);
                            break;
                        case "fieldstartpos":
                            FieldStartPosList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        case "soundtestlist":
                            SoundTestList.Load(datafile, address, imageBase).Save(fileOutputPath);
                            break;
                        case "musiclist":
                            {
                                int muscnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                MusicList.Load(datafile, address, imageBase, muscnt).Save(fileOutputPath);
                            }
                            break;
                        case "soundlist":
                            SoundList.Load(datafile, address, imageBase).Save(fileOutputPath);
                            break;
                        case "stringarray":
                            {
                                int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                Languages lang = Languages.Japanese;
                                if (data.CustomProperties.ContainsKey("language"))
                                    lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
                                StringArray.Load(datafile, address, imageBase, cnt, lang).Save(fileOutputPath);
                            }
                            break;
                        case "nextlevellist":
                            NextLevelList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        case "cutscenetext":
                            {
                                int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                string[] hashes;
                                new CutsceneText(datafile, address, imageBase, cnt).Save(fileOutputPath, out hashes);
                                data.MD5Hash = string.Join(",", hashes);
                                nohash = true;
                            }
                            break;
                        case "recapscreen":
                            {
                                int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                string[][] hashes;
                                RecapScreenList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath, out hashes);
                                string[] hash2 = new string[hashes.Length];
                                for (int i = 0; i < hashes.Length; i++)
                                {
                                    hash2[i] = string.Join(",", hashes[i]);
                                }
                                data.MD5Hash = string.Join(":", hash2);
                                nohash = true;
                            }
                            break;
                        case "npctext":
                            {
                                int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                string[][] hashes;
                                NPCTextList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath, out hashes);
                                string[] hash2 = new string[hashes.Length];
                                for (int i = 0; i < hashes.Length; i++)
                                    hash2[i] = string.Join(",", hashes[i]);
                                data.MD5Hash = string.Join(":", hash2);
                                nohash = true;
                            }
                            break;
                        case "levelclearflags":
                            LevelClearFlagList.Save(LevelClearFlagList.Load(datafile, address), fileOutputPath);
                            break;
                        case "deathzone":
                            {
                                List<DeathZoneFlags> flags = new List<DeathZoneFlags>();
                                string path = Path.GetDirectoryName(fileOutputPath);
                                List<string> hashes = new List<string>();
                                int num = 0;
                                while (SA_Tools.ByteConverter.ToUInt32(datafile, address + 4) != 0)
                                {
                                    flags.Add(new DeathZoneFlags(datafile, address));
                                    string file = Path.Combine(path, num++.ToString(NumberFormatInfo.InvariantInfo) + (modelfmt == ModelFormat.Chunk ? ".sa2mdl" : ".sa1mdl"));
                                    ModelFile.CreateFile(file, new SonicRetro.SAModel.NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, modelfmt), null, null, null, null, "split", null, modelfmt);
                                    hashes.Add(HelperFunctions.FileHash(file));
                                    address += 8;
                                }
                                flags.ToArray().Save(fileOutputPath);
                                hashes.Insert(0, HelperFunctions.FileHash(fileOutputPath));
                                data.MD5Hash = string.Join(",", hashes.ToArray());
                                nohash = true;
                            }
                            break;
                        case "skyboxscale":
                            {
                                int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                SkyboxScaleList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath);
                            }
                            break;
                        case "stageselectlist":
                            {
                                int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                StageSelectLevelList.Load(datafile, address, cnt).Save(fileOutputPath);
                            }
                            break;
                        case "levelrankscores":
                            LevelRankScoresList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        case "levelranktimes":
                            LevelRankTimesList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        case "endpos":
                            SA2EndPosList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        case "animationlist":
                            {
                                int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                SA2AnimationInfoList.Load(datafile, address, cnt).Save(fileOutputPath);
                            }
                            break;
                        case "levelpathlist":
                            {
                                List<string> hashes = new List<string>();
                                ushort lvlnum = (ushort)SA_Tools.ByteConverter.ToUInt32(datafile, address);
                                while (lvlnum != 0xFFFF)
                                {
                                    int ptr = SA_Tools.ByteConverter.ToInt32(datafile, address + 4);
                                    if (ptr != 0)
                                    {
                                        ptr = (int)((uint)ptr - imageBase);
                                        SA1LevelAct level = new SA1LevelAct(lvlnum);
                                        string lvldir = Path.Combine(fileOutputPath, level.ToString());
                                        string[] lvlhashes;
                                        PathList.Load(datafile, ptr, imageBase).Save(lvldir, out lvlhashes);
                                        hashes.Add(level.ToString() + ":" + string.Join(",", lvlhashes));
                                    }
                                    address += 8;
                                    lvlnum = (ushort)SA_Tools.ByteConverter.ToUInt32(datafile, address);
                                }
                                data.MD5Hash = string.Join("|", hashes.ToArray());
                                nohash = true;
                            }
                            break;
                        case "stagelightdatalist":
                            SA1StageLightDataList.Load(datafile, address).Save(fileOutputPath);
                            break;
                        default: // raw binary
                            {
                                byte[] bin = new byte[int.Parse(customProperties["size"], NumberStyles.HexNumber)];
                                Array.Copy(datafile, address, bin, 0, bin.Length);
                                File.WriteAllBytes(fileOutputPath, bin);
                            }
                            break;
                    }
                    if (!nohash)
                        data.MD5Hash = HelperFunctions.FileHash(fileOutputPath);
                    itemcount++;
                }
                if (inifile.MasterObjectList != null)
                {
                    foreach (KeyValuePair<string, MasterObjectListEntry> obj in masterobjlist)
                    {
                        KeyValuePair<string, int> name = new KeyValuePair<string, int>();
                        foreach (KeyValuePair<string, int> it in objnamecounts[obj.Key])
                            if (it.Value > name.Value)
                                name = it;
                        obj.Value.Name = name.Key;
                        obj.Value.Names = objnamecounts[obj.Key].Select((it) => it.Key).ToArray();
                    }
                    IniSerializer.Serialize(masterobjlist, string.Concat(projectFolderName, inifile.MasterObjectList));
                }

                string dataMappingFolder = string.Concat(projectFolderName, "\\DataMappings\\");
                if (!Directory.Exists(dataMappingFolder)) Directory.CreateDirectory(dataMappingFolder);
                IniSerializer.Serialize(inifile, Path.Combine(dataMappingFolder, Path.GetFileNameWithoutExtension(datafilename)) + "_data.ini");
                timer.Stop();
                Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
                Console.WriteLine();

            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            errorMessage = "";
            return true;
        }

        public static bool DLL(string[] args, out string errorMessage)
        {
            string datafilename, inifilename, projectFolderName;
            if (args.Length > 0)
            {
                datafilename = args[0];
                Console.WriteLine("File: {0}", datafilename);
            }
            else
            {
                errorMessage = "No source file supplied. Aborting.";
                return false;
            }
            if (args.Length > 1)
            {
                inifilename = args[1];
                Console.WriteLine("INI File: {0}", inifilename);
            }
            else
            {
                errorMessage = "No data mapping file supplied (expected ini). Aborting.";
                return false;
            }
            if (args.Length > 2)
            {
                projectFolderName = args[2];
                Console.WriteLine("Project Folder: {0}", projectFolderName);
            }
            else
            {
                errorMessage = "No project folder supplied. Aborting.";
                return false;
            }

            try
            {
                byte[] datafile = File.ReadAllBytes(datafilename);
                Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(inifilename));
                DLLIniData inifile = IniSerializer.Deserialize<DLLIniData>(inifilename);
                uint imageBase = HelperFunctions.SetupEXE(ref datafile).Value;
                Dictionary<string, int> exports;
                {
                    int ptr = BitConverter.ToInt32(datafile, BitConverter.ToInt32(datafile, 0x3c) + 4 + 20 + 96);
                    GCHandle handle = GCHandle.Alloc(datafile, GCHandleType.Pinned);
                    IMAGE_EXPORT_DIRECTORY dir = (IMAGE_EXPORT_DIRECTORY)Marshal.PtrToStructure(
                        Marshal.UnsafeAddrOfPinnedArrayElement(datafile, ptr), typeof(IMAGE_EXPORT_DIRECTORY));
                    handle.Free();
                    exports = new Dictionary<string, int>(dir.NumberOfFunctions);
                    int nameaddr = dir.AddressOfNames;
                    int ordaddr = dir.AddressOfNameOrdinals;
                    for (int i = 0; i < dir.NumberOfNames; i++)
                    {
                        string name = HelperFunctions.GetCString(datafile, BitConverter.ToInt32(datafile, nameaddr),
                            System.Text.Encoding.ASCII);
                        int addr = BitConverter.ToInt32(datafile,
                            dir.AddressOfFunctions + (BitConverter.ToInt16(datafile, ordaddr) * 4));
                        exports.Add(name, addr);
                        nameaddr += 4;
                        ordaddr += 2;
                    }
                }
                ModelFormat modelfmt = 0;
                LandTableFormat landfmt = 0;
                string modelext = null;
                string landext = null;
                switch (inifile.Game)
                {
                    case Game.SADX:
                        modelfmt = ModelFormat.BasicDX;
                        landfmt = LandTableFormat.SADX;
                        modelext = ".sa1mdl";
                        landext = ".sa1lvl";
                        break;
                    case Game.SA2B:
                        modelfmt = ModelFormat.Chunk;
                        landfmt = LandTableFormat.SA2;
                        modelext = ".sa2mdl";
                        landext = ".sa2lvl";
                        break;
                }
                int itemcount = 0;
                List<string> labels = new List<string>();
                ModelAnimationsDictionary models = new ModelAnimationsDictionary();
                DllIniData output = new DllIniData();
                output.Name = inifile.ModuleName;
                output.Game = inifile.Game;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                foreach (KeyValuePair<string, DLLFileInfo> item in inifile.Files)
                {
                    if (string.IsNullOrEmpty(item.Key)) continue;
                    DLLFileInfo data = item.Value;
                    string type = data.Type;
                    string name = item.Key;
                    output.Exports[name] = type;
                    int address = exports[name];
                    string fileOutputPath = string.Concat(projectFolderName, data.Filename);
                    Console.WriteLine(name + " → " + fileOutputPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
                    switch (type)
                    {
                        case "landtable":
                            {
                                LandTable land = new LandTable(datafile, address, imageBase, landfmt) { Description = name, Tool = "splitDLL" };
                                DllItemInfo info = new DllItemInfo();
                                info.Export = name;
                                info.Label = land.Name;
                                output.Items.Add(info);
                                if (!labels.Contains(land.Name))
                                {
                                    land.SaveToFile(fileOutputPath, landfmt);
                                    output.Files[data.Filename] = new FileTypeHash("landtable", HelperFunctions.FileHash(fileOutputPath));
                                    labels.AddRange(land.GetLabels());
                                }
                            }
                            break;
                        case "landtablearray":
                            for (int i = 0; i < data.Length; i++)
                            {
                                int ptr = BitConverter.ToInt32(datafile, address);
                                if (ptr != 0)
                                {
                                    ptr = (int)(ptr - imageBase);
                                    string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
                                    LandTable land = new LandTable(datafile, ptr, imageBase, landfmt) { Description = idx, Tool = "splitDLL" };
                                    DllItemInfo info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = land.Name;
                                    output.Items.Add(info);
                                    if (!labels.Contains(land.Name))
                                    {
                                        string fn = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + landext);
                                        land.SaveToFile(fn, landfmt);
                                        output.Files[fn] = new FileTypeHash("landtable", HelperFunctions.FileHash(fn));
                                        labels.AddRange(land.GetLabels());
                                    }
                                }
                                address += 4;
                            }
                            break;
                        case "model":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, modelfmt);
                                DllItemInfo info = new DllItemInfo();
                                info.Export = name;
                                info.Label = mdl.Name;
                                output.Items.Add(info);
                                if (!labels.Contains(mdl.Name))
                                {
                                    models.Add(new ModelAnimations(fileOutputPath, name, mdl, modelfmt));
                                    labels.AddRange(mdl.GetLabels());
                                }
                            }
                            break;
                        case "modelarray":
                            for (int i = 0; i < data.Length; i++)
                            {
                                int ptr = BitConverter.ToInt32(datafile, address);
                                if (ptr != 0)
                                {
                                    ptr = (int)(ptr - imageBase);
                                    SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, ptr, imageBase, modelfmt);
                                    string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
                                    DllItemInfo info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = mdl.Name;
                                    output.Items.Add(info);
                                    if (!labels.Contains(mdl.Name))
                                    {
                                        string fn = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
                                        models.Add(new ModelAnimations(fn, idx, mdl, modelfmt));
                                        labels.AddRange(mdl.GetLabels());
                                    }
                                }
                                address += 4;
                            }
                            break;
                        case "basicmodel":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, ModelFormat.Basic);
                                DllItemInfo info = new DllItemInfo();
                                info.Export = name;
                                info.Label = mdl.Name;
                                output.Items.Add(info);
                                if (!labels.Contains(mdl.Name))
                                {
                                    models.Add(new ModelAnimations(fileOutputPath, name, mdl, ModelFormat.Basic));
                                    labels.AddRange(mdl.GetLabels());
                                }
                            }
                            break;
                        case "basicmodelarray":
                            for (int i = 0; i < data.Length; i++)
                            {
                                int ptr = BitConverter.ToInt32(datafile, address);
                                if (ptr != 0)
                                {
                                    ptr = (int)(ptr - imageBase);
                                    SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, ptr, imageBase, ModelFormat.Basic);
                                    string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
                                    DllItemInfo info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = mdl.Name;
                                    output.Items.Add(info);
                                    if (!labels.Contains(mdl.Name))
                                    {
                                        string fn = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl");
                                        models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.Basic));
                                        labels.AddRange(mdl.GetLabels());
                                    }
                                }
                                address += 4;
                            }
                            break;
                        case "basicdxmodel":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, ModelFormat.BasicDX);
                                DllItemInfo info = new DllItemInfo();
                                info.Export = name;
                                info.Label = mdl.Name;
                                output.Items.Add(info);
                                if (!labels.Contains(mdl.Name))
                                {
                                    models.Add(new ModelAnimations(fileOutputPath, name, mdl, ModelFormat.BasicDX));
                                    labels.AddRange(mdl.GetLabels());
                                }
                            }
                            break;
                        case "basicdxmodelarray":
                            for (int i = 0; i < data.Length; i++)
                            {
                                int ptr = BitConverter.ToInt32(datafile, address);
                                if (ptr != 0)
                                {
                                    ptr = (int)(ptr - imageBase);
                                    SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, ptr, imageBase, ModelFormat.BasicDX);
                                    string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
                                    DllItemInfo info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = mdl.Name;
                                    output.Items.Add(info);
                                    if (!labels.Contains(mdl.Name))
                                    {
                                        string fn = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl");
                                        models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.BasicDX));
                                        labels.AddRange(mdl.GetLabels());
                                    }
                                }
                                address += 4;
                            }
                            break;
                        case "chunkmodel":
                            {
                                SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, address, imageBase, ModelFormat.Chunk);
                                DllItemInfo info = new DllItemInfo();
                                info.Export = name;
                                info.Label = mdl.Name;
                                output.Items.Add(info);
                                if (!labels.Contains(mdl.Name))
                                {
                                    models.Add(new ModelAnimations(fileOutputPath, name, mdl, ModelFormat.Chunk));
                                    labels.AddRange(mdl.GetLabels());
                                }
                            }
                            break;
                        case "chunkmodelarray":
                            for (int i = 0; i < data.Length; i++)
                            {
                                int ptr = BitConverter.ToInt32(datafile, address);
                                if (ptr != 0)
                                {
                                    ptr = (int)(ptr - imageBase);
                                    SonicRetro.SAModel.NJS_OBJECT mdl = new SonicRetro.SAModel.NJS_OBJECT(datafile, ptr, imageBase, ModelFormat.Chunk);
                                    string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
                                    DllItemInfo info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = mdl.Name;
                                    output.Items.Add(info);
                                    if (!labels.Contains(mdl.Name))
                                    {
                                        string fn = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl");
                                        models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.Chunk));
                                        labels.AddRange(mdl.GetLabels());
                                    }
                                }
                                address += 4;
                            }
                            break;
                        case "actionarray":
                            for (int i = 0; i < data.Length; i++)
                            {
                                int ptr = BitConverter.ToInt32(datafile, address);
                                if (ptr != 0)
                                {
                                    ptr = (int)(ptr - imageBase);
                                    AnimationHeader ani = new AnimationHeader(datafile, ptr, imageBase, modelfmt);
                                    string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
                                    DllItemInfo info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = ani.Animation.Name;
                                    info.Field = "motion";
                                    output.Items.Add(info);
                                    info = new DllItemInfo();
                                    info.Export = name;
                                    info.Index = i;
                                    info.Label = ani.Model.Name;
                                    info.Field = "object";
                                    output.Items.Add(info);
                                    string fn = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
                                    ani.Animation.Save(fn);
                                    output.Files[fn] = new FileTypeHash("animation", HelperFunctions.FileHash(fn));
                                    if (models.Contains(ani.Model.Name))
                                    {
                                        ModelAnimations mdl = models[ani.Model.Name];
                                        System.Text.StringBuilder sb = new System.Text.StringBuilder(260);
                                        PathRelativePathTo(sb, Path.GetFullPath(mdl.Filename), 0, Path.GetFullPath(fn), 0);
                                        mdl.Animations.Add(sb.ToString());
                                    }
                                    else
                                    {
                                        string mfn = Path.ChangeExtension(fn, modelext);
                                        ModelFile.CreateFile(mfn, ani.Model, new[] { Path.GetFileName(fn) }, null, null,
                                            idx + "->object", "splitDLL", null, modelfmt);
                                        output.Files[mfn] = new FileTypeHash("model", HelperFunctions.FileHash(mfn));
                                    }
                                }
                                address += 4;
                            }
                            break;
                    }
                    itemcount++;
                }
                foreach (ModelAnimations item in models)
                {
                    ModelFile.CreateFile(item.Filename, item.Model, item.Animations.ToArray(), null, null, item.Name, "splitDLL",
                        null, item.Format);
                    string type = "model";
                    switch (item.Format)
                    {
                        case ModelFormat.Basic:
                            type = "basicmodel";
                            break;
                        case ModelFormat.BasicDX:
                            type = "basicdxmodel";
                            break;
                        case ModelFormat.Chunk:
                            type = "chunkmodel";
                            break;
                    }
                    output.Files[item.Filename] = new FileTypeHash(type, HelperFunctions.FileHash(item.Filename));
                }

                string dataMappingFolder = string.Concat(projectFolderName, "\\DataMappings\\");
                if (!Directory.Exists(dataMappingFolder)) Directory.CreateDirectory(dataMappingFolder);
                IniSerializer.Serialize(output, Path.Combine(dataMappingFolder, Path.GetFileNameWithoutExtension(datafilename)) + "_data.ini");
                timer.Stop();
                Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            errorMessage = "";
            return true;
        }

        [DllImport("shlwapi.dll", SetLastError = true)]
        private static extern bool PathRelativePathTo(System.Text.StringBuilder pszPath,
            string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);

        static List<string> GetLabels(this LandTable land)
        {
            List<string> labels = new List<string>() { land.Name };
            if (land.COLName != null)
            {
                labels.Add(land.COLName);
                foreach (COL col in land.COL)
                    if (col.Model != null)
                        labels.AddRange(col.Model.GetLabels());
            }
            if (land.AnimName != null)
            {
                labels.Add(land.AnimName);
                foreach (GeoAnimData gan in land.Anim)
                {
                    if (gan.Model != null)
                        labels.AddRange(gan.Model.GetLabels());
                    if (gan.Animation != null)
                        labels.Add(gan.Animation.Name);
                }
            }
            return labels;
        }

        static List<string> GetLabels(this SonicRetro.SAModel.NJS_OBJECT obj)
        {
            List<string> labels = new List<string>() { obj.Name };
            if (obj.Attach != null)
                labels.AddRange(obj.Attach.GetLabels());
            if (obj.Children != null)
                foreach (SonicRetro.SAModel.NJS_OBJECT o in obj.Children)
                    labels.AddRange(o.GetLabels());
            return labels;
        }

        static List<string> GetLabels(this Attach att)
        {
            List<string> labels = new List<string>() { att.Name };
            if (att is BasicAttach)
            {
                BasicAttach bas = (BasicAttach)att;
                if (bas.VertexName != null)
                    labels.Add(bas.VertexName);
                if (bas.NormalName != null)
                    labels.Add(bas.NormalName);
                if (bas.MaterialName != null)
                    labels.Add(bas.MaterialName);
                if (bas.MeshName != null)
                    labels.Add(bas.MeshName);
            }
            else if (att is ChunkAttach)
            {
                ChunkAttach cnk = (ChunkAttach)att;
                if (cnk.VertexName != null)
                    labels.Add(cnk.VertexName);
                if (cnk.PolyName != null)
                    labels.Add(cnk.PolyName);
            }
            return labels;
        }
    }

    class ModelAnimationsDictionary : System.Collections.ObjectModel.KeyedCollection<string, ModelAnimations>
    {
        protected override string GetKeyForItem(ModelAnimations item)
        {
            return item.Model.Name;
        }
    }

    class ModelAnimations
    {
        public string Filename { get; private set; }
        public string Name { get; private set; }
        public SonicRetro.SAModel.NJS_OBJECT Model { get; private set; }
        public ModelFormat Format { get; private set; }
        public List<string> Animations { get; private set; }

        public ModelAnimations(string filename, string name, SonicRetro.SAModel.NJS_OBJECT model, ModelFormat format)
        {
            Filename = filename;
            Name = name;
            Model = model;
            Format = format;
            Animations = new List<string>();
        }
    }

    struct IMAGE_EXPORT_DIRECTORY
    {
        public int Characteristics;
        public int TimeDateStamp;
        public short MajorVersion;
        public short MinorVersion;
        public int Name;
        public int Base;
        public int NumberOfFunctions;
        public int NumberOfNames;
        public int AddressOfFunctions;     // RVA from base of image
        public int AddressOfNames;         // RVA from base of image
        public int AddressOfNameOrdinals;  // RVA from base of image
    }
}