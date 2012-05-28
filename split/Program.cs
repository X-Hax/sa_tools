using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using SonicRetro.SAModel;
using SADXPCTools;

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
            HelperFunctions.SetupEXE(ref exefile);
            IniData inifile = IniFile.Deserialize<IniData>(inifilename);
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(exefilename));
            uint imageBase = inifile.ImageBase;
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
                        new LandTable(exefile, address, imageBase, ModelFormat.SADX).SaveToFile(data.Filename, ModelFormat.SA1);
                        break;
                    case "model":
                        {
                            SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(exefile, address, imageBase, ModelFormat.SADX);
                            string[] mdlanis = new string[0];
                            if (customProperties.ContainsKey("animations"))
                                mdlanis = customProperties["animations"].Split(',');
                            string[] mdlmorphs = new string[0];
                            if (customProperties.ContainsKey("morphs"))
                                mdlmorphs = customProperties["morphs"].Split(',');
                            ModelFile.CreateFile(data.Filename, mdl, mdlanis, mdlmorphs, ModelFormat.SA1);
                        }
                        break;
                    case "animation":
                        {
                            AnimationHeader ani = new AnimationHeader(exefile, address, imageBase, ModelFormat.SADX);
                            ani.Animation.Name = filedesc;
                            ani.Animation.Save(data.Filename);
                        }
                        break;
                    case "objlist":
                        ObjectList.Load(exefile, address, imageBase).Save(data.Filename);
                        break;
                    case "startpos":
                        StartPosList.Load(exefile, address).Save(data.Filename);
                        break;
                    case "texlist":
                        TextureList.Load(exefile, address, imageBase).Save(data.Filename);
                        break;
                    case "leveltexlist":
                        new LevelTextureList(exefile, address, imageBase).Save(data.Filename);
                        break;
                    case "triallevellist":
                        TrialLevelList.Save(TrialLevelList.Load(exefile, address, imageBase), data.Filename);
                        break;
                    case "bosslevellist":
                        BossLevelList.Save(BossLevelList.Load(exefile, address), data.Filename);
                        break;
                    case "fieldstartpos":
                        FieldStartPosList.Load(exefile, address).Save(data.Filename);
                        break;
                    case "soundtestlist":
                        SoundTestList.Load(exefile, address, imageBase).Save(data.Filename);
                        break;
                    case "musiclist":
                        {
                            int muscnt = int.Parse(customProperties["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            MusicList.Load(exefile, address, imageBase, muscnt).Save(data.Filename);
                        }
                        break;
                    case "soundlist":
                        SoundList.Load(exefile, address, imageBase).Save(data.Filename);
                        break;
                    case "stringarray":
                        {
                            int cnt = int.Parse(customProperties["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            Languages lang = Languages.Japanese;
                            if (data.CustomProperties.ContainsKey("language"))
                                lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
                            StringArray.Load(exefile, address, imageBase, cnt, HelperFunctions.GetEncoding(lang)).Save(data.Filename);
                        }
                        break;
                    case "nextlevellist":
                        NextLevelList.Load(exefile, address).Save(data.Filename);
                        break;
                    case "cutscenetext":
                        {
                            int cnt = int.Parse(customProperties["length"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            string[] hashes;
                            new CutsceneText(exefile, address, imageBase, cnt).Save(data.Filename, out hashes);
                            data.MD5Hash = string.Join(",", hashes);
                            nohash = true;
                        }
                        break;
                    case "levelclearflags":
                        LevelClearFlagList.Save(LevelClearFlagList.Load(exefile, address), data.Filename);
                        break;
                    case "deathzone":
                        {
                            List<DeathZoneFlags> flags = new List<DeathZoneFlags>();
                            string path = Path.GetDirectoryName(data.Filename);
                            int num = 0;
                            while (BitConverter.ToUInt32(exefile, address + 4) != 0)
                            {
                                flags.Add(new DeathZoneFlags(exefile, address));
                                ModelFile.CreateFile(Path.Combine(path, num++.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl"), new SonicRetro.SAModel.Object(exefile, (int)(BitConverter.ToUInt32(exefile, address + 4) - imageBase), imageBase, ModelFormat.SADX), ModelFormat.SA1);
                                address += 8;
                            }
                            flags.ToArray().Save(data.Filename);
                            nohash = true;
                        }
                        break;
                    case "skyboxscale":
                        {
                            int cnt = int.Parse(customProperties["count"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                            SkyboxScaleList.Load(exefile, address, imageBase, cnt).Save(data.Filename);
                        }
                        break;
                    default: // raw binary
                        {
                            byte[] bin = new byte[int.Parse(customProperties["size"], System.Globalization.NumberStyles.HexNumber)];
                            Array.Copy(exefile, address, bin, 0, bin.Length);
                            File.WriteAllBytes(data.Filename, bin);
                        }
                        break;
                }
                if (!nohash)
                    data.MD5Hash = HelperFunctions.FileHash(data.Filename);
                data.NoHash = nohash;
                itemcount++;
            }
            IniFile.Serialize(inifile, Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(exefilename)) + "_data.ini");
            timer.Stop();
            Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
            Console.Write("Press a key to continue...");
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}