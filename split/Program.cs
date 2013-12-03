using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using SADXPCTools;
using SonicRetro.SAModel;

namespace split
{
    static class Program
    {
        static void Main(string[] args)
        {
            string datafilename, inifilename;
            if (args.Length > 0)
            {
                datafilename = args[0];
                Console.WriteLine("File: {0}", datafilename);
            }
            else
            {
                Console.Write("File: ");
                datafilename = Console.ReadLine();
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
            byte[] datafile = File.ReadAllBytes(datafilename);
            IniData inifile = IniFile.Deserialize<IniData>(inifilename);
            SADXPCTools.ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = inifile.BigEndian;
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
                    modelfmt = ModelFormat.Chunk;
                    landfmt = LandTableFormat.SA2;
                    break;
                case Game.SA2B:
                    modelfmt = ModelFormat.Chunk;
                    landfmt = LandTableFormat.SA2B;
                    break;
            }
            int itemcount = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            foreach (KeyValuePair<string, SADXPCTools.FileInfo> item in inifile.Files)
            {
                if (string.IsNullOrEmpty(item.Key)) continue;
                string filedesc = item.Key;
                SADXPCTools.FileInfo data = item.Value;
                Dictionary<string, string> customProperties = data.CustomProperties;
                string type = data.Type;
                int address = data.Address;
                bool nohash = false;
                Console.WriteLine(item.Key + ": " + data.AddressString + " → " + data.Filename);
                Directory.CreateDirectory(Path.GetDirectoryName(data.Filename));
                switch (type)
                {
                    case "landtable":
                        new LandTable(datafile, address, imageBase, landfmt) { Description = item.Key, Tool = "split" }.SaveToFile(data.Filename, landfmt);
                        break;
                    case "model":
                        {
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, modelfmt);
                            string[] mdlanis = new string[0];
                            if (customProperties.ContainsKey("animations"))
                                mdlanis = customProperties["animations"].Split(',');
                            string[] mdlmorphs = new string[0];
                            if (customProperties.ContainsKey("morphs"))
                                mdlmorphs = customProperties["morphs"].Split(',');
                            ModelFile.CreateFile(data.Filename, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, modelfmt);
                        }
                        break;
                    case "basicmodel":
                        {
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, ModelFormat.Basic);
                            string[] mdlanis = new string[0];
                            if (customProperties.ContainsKey("animations"))
                                mdlanis = customProperties["animations"].Split(',');
                            string[] mdlmorphs = new string[0];
                            if (customProperties.ContainsKey("morphs"))
                                mdlmorphs = customProperties["morphs"].Split(',');
                            ModelFile.CreateFile(data.Filename, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, ModelFormat.Basic);
                        }
                        break;
                    case "basicdxmodel":
                        {
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, ModelFormat.BasicDX);
                            string[] mdlanis = new string[0];
                            if (customProperties.ContainsKey("animations"))
                                mdlanis = customProperties["animations"].Split(',');
                            string[] mdlmorphs = new string[0];
                            if (customProperties.ContainsKey("morphs"))
                                mdlmorphs = customProperties["morphs"].Split(',');
                            ModelFile.CreateFile(data.Filename, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, ModelFormat.BasicDX);
                        }
                        break;
                    case "chunkmodel":
                        {
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, ModelFormat.Chunk);
                            string[] mdlanis = new string[0];
                            if (customProperties.ContainsKey("animations"))
                                mdlanis = customProperties["animations"].Split(',');
                            string[] mdlmorphs = new string[0];
                            if (customProperties.ContainsKey("morphs"))
                                mdlmorphs = customProperties["morphs"].Split(',');
                            ModelFile.CreateFile(data.Filename, mdl, mdlanis, mdlmorphs, null, item.Key, "split", null, ModelFormat.Chunk);
                        }
                        break;
                    case "action":
                        {
                            AnimationHeader ani = new AnimationHeader(datafile, address, imageBase, modelfmt);
                            ani.Animation.Name = filedesc;
                            ani.Animation.Save(data.Filename);
                        }
                        break;
                    case "animation":
                            new Animation(datafile, address, imageBase, int.Parse(customProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo)) { Name = filedesc }
                                .Save(data.Filename);
                            break;
                    case "objlist":
                        ObjectList.Load(datafile, address, imageBase, SA2).Save(data.Filename);
                        break;
                    case "startpos":
                        if (SA2)
                            SA2StartPosList.Load(datafile, address).Save(data.Filename);
                        else
                            SA1StartPosList.Load(datafile, address).Save(data.Filename);
                        break;
                    case "texlist":
                        TextureList.Load(datafile, address, imageBase).Save(data.Filename);
                        break;
                    case "leveltexlist":
                        new LevelTextureList(datafile, address, imageBase).Save(data.Filename);
                        break;
                    case "triallevellist":
                        TrialLevelList.Save(TrialLevelList.Load(datafile, address, imageBase), data.Filename);
                        break;
                    case "bosslevellist":
                        BossLevelList.Save(BossLevelList.Load(datafile, address), data.Filename);
                        break;
                    case "fieldstartpos":
                        FieldStartPosList.Load(datafile, address).Save(data.Filename);
                        break;
                    case "soundtestlist":
                        SoundTestList.Load(datafile, address, imageBase).Save(data.Filename);
                        break;
                    case "musiclist":
                        {
                            int muscnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            MusicList.Load(datafile, address, imageBase, muscnt).Save(data.Filename);
                        }
                        break;
                    case "soundlist":
                        SoundList.Load(datafile, address, imageBase).Save(data.Filename);
                        break;
                    case "stringarray":
                        {
                            int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            Languages lang = Languages.Japanese;
                            if (data.CustomProperties.ContainsKey("language"))
                                lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
                            StringArray.Load(datafile, address, imageBase, cnt, lang).Save(data.Filename);
                        }
                        break;
                    case "nextlevellist":
                        NextLevelList.Load(datafile, address).Save(data.Filename);
                        break;
                    case "cutscenetext":
                        {
                            int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            string[] hashes;
                            new CutsceneText(datafile, address, imageBase, cnt).Save(data.Filename, out hashes);
                            data.MD5Hash = string.Join(",", hashes);
                            nohash = true;
                        }
                        break;
                    case "recapscreen":
                        {
                            int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            string[][] hashes;
                            RecapScreenList.Load(datafile, address, imageBase, cnt).Save(data.Filename, out hashes);
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
                            NPCTextList.Load(datafile, address, imageBase, cnt).Save(data.Filename, out hashes);
                            string[] hash2 = new string[hashes.Length];
                            for (int i = 0; i < hashes.Length; i++)
                                hash2[i] = string.Join(",", hashes[i]);
                            data.MD5Hash = string.Join(":", hash2);
                            nohash = true;
                        }
                        break;
                    case "levelclearflags":
                        LevelClearFlagList.Save(LevelClearFlagList.Load(datafile, address), data.Filename);
                        break;
                    case "deathzone":
                        {
                            List<DeathZoneFlags> flags = new List<DeathZoneFlags>();
                            string path = Path.GetDirectoryName(data.Filename);
                            int num = 0;
                            while (SADXPCTools.ByteConverter.ToUInt32(datafile, address + 4) != 0)
                            {
                                flags.Add(new DeathZoneFlags(datafile, address));
                                ModelFile.CreateFile(Path.Combine(path, num++.ToString(NumberFormatInfo.InvariantInfo) + (modelfmt == ModelFormat.Chunk ? ".sa2mdl" : ".sa1mdl")), new SonicRetro.SAModel.Object(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, modelfmt), null, null, null, null, "split", null, modelfmt);
                                address += 8;
                            }
                            flags.ToArray().Save(data.Filename);
                            nohash = true;
                        }
                        break;
                    case "skyboxscale":
                        {
                            int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            SkyboxScaleList.Load(datafile, address, imageBase, cnt).Save(data.Filename);
                        }
                        break;
                    case "stageselectlist":
                        {
                            int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            StageSelectLevelList.Load(datafile, address, cnt).Save(data.Filename);
                        }
                        break;
                    case "levelrankscores":
                        LevelRankScoresList.Load(datafile, address).Save(data.Filename);
                        break;
                    case "levelranktimes":
                        LevelRankTimesList.Load(datafile, address).Save(data.Filename);
                        break;
                    case "endpos":
                        SA2EndPosList.Load(datafile, address).Save(data.Filename);
                        break;
                    case "animationlist":
                        {
                            int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            SA2AnimationInfoList.Load(datafile, address, cnt).Save(data.Filename);
                        }
                        break;
                    default: // raw binary
                        {
                            byte[] bin = new byte[int.Parse(customProperties["size"], NumberStyles.HexNumber)];
                            Array.Copy(datafile, address, bin, 0, bin.Length);
                            File.WriteAllBytes(data.Filename, bin);
                        }
                        break;
                }
                if (!nohash)
                    data.MD5Hash = HelperFunctions.FileHash(data.Filename);
                data.NoHash = nohash;
                itemcount++;
            }
            IniFile.Serialize(inifile, Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(datafilename)) + "_data.ini");
            timer.Stop();
            Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
            Console.WriteLine();
        }
    }
}