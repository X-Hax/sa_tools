using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using SA_Tools;
using SonicRetro.SAModel;

namespace ProjectManager.Split
{
	static class Split
	{
		public static int SplitFile(string datafilename, string inifilename, string projectFolderName)
		{
			try
			{
				byte[] datafile = File.ReadAllBytes(datafilename);
				IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
				if (inifile.MD5 != null && inifile.MD5.Count > 0)
				{
					string datahash = HelperFunctions.FileHash(datafile);
					if (!inifile.MD5.Any(h => h.Equals(datahash, StringComparison.OrdinalIgnoreCase)))
					{
						Console.WriteLine("The file {0} is not valid for use with the INI {1}.", datafilename, inifilename);
						return (int)ERRORVALUE.InvalidDataMapping;
					}
				}
				ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = inifile.BigEndian;
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
					Console.WriteLine(item.Key + ": " + data.Address.ToString("X") + " → " + fileOutputPath);
					Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
					switch (type)
					{
						case "landtable":
							new LandTable(datafile, address, imageBase, landfmt) { Description = item.Key }.SaveToFile(fileOutputPath, landfmt);
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
								ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, null, modelfmt);
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
								ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, null, ModelFormat.Basic);
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
								ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, null, ModelFormat.BasicDX);
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
								ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, mdlmorphs, null, item.Key, null, ModelFormat.Chunk);
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
								new CutsceneText(datafile, address, imageBase, cnt).Save(fileOutputPath, out string[] hashes);
								data.MD5Hash = string.Join(",", hashes);
								nohash = true;
							}
							break;
						case "recapscreen":
							{
								int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								RecapScreenList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath, out string[][] hashes);
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
								NPCTextList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath, out string[][] hashes);
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
								while (ByteConverter.ToUInt32(datafile, address + 4) != 0)
								{
									flags.Add(new DeathZoneFlags(datafile, address));
									string file = Path.Combine(path, num++.ToString(NumberFormatInfo.InvariantInfo) + (modelfmt == ModelFormat.Chunk ? ".sa2mdl" : ".sa1mdl"));
									ModelFile.CreateFile(file, new SonicRetro.SAModel.NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, modelfmt), null, null, null, null, null, modelfmt);
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
								ushort lvlnum = (ushort)ByteConverter.ToUInt32(datafile, address);
								while (lvlnum != 0xFFFF)
								{
									int ptr = ByteConverter.ToInt32(datafile, address + 4);
									if (ptr != 0)
									{
										ptr = (int)((uint)ptr - imageBase);
										SA1LevelAct level = new SA1LevelAct(lvlnum);
										string lvldir = Path.Combine(fileOutputPath, level.ToString());
										PathList.Load(datafile, ptr, imageBase).Save(lvldir, out string[] lvlhashes);
										hashes.Add(level.ToString() + ":" + string.Join(",", lvlhashes));
									}
									address += 8;
									lvlnum = (ushort)ByteConverter.ToUInt32(datafile, address);
								}
								data.MD5Hash = string.Join("|", hashes.ToArray());
								nohash = true;
							}
							break;
						case "pathlist":
							{
								PathList.Load(datafile, address, imageBase).Save(fileOutputPath, out string[] hashes);
								data.MD5Hash = string.Join(",", hashes.ToArray());
								nohash = true;
							}
							break;
						case "stagelightdatalist":
							SA1StageLightDataList.Load(datafile, address).Save(fileOutputPath);
							break;
						case "weldlist":
							WeldList.Load(datafile, address, imageBase).Save(fileOutputPath);
							break;
						case "bmitemattrlist":
							BlackMarketItemAttributesList.Load(datafile, address, imageBase).Save(fileOutputPath);
							break;
						case "creditstextlist":
							CreditsTextList.Load(datafile, address, imageBase).Save(fileOutputPath);
							break;
						case "animindexlist":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<string> hashes = new List<string>();
								int i = ByteConverter.ToInt16(datafile, address);
								while (i != -1)
								{
									new Animation(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ByteConverter.ToInt16(datafile, address + 2))
										.Save(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
									hashes.Add(i.ToString(NumberFormatInfo.InvariantInfo) + ":" + HelperFunctions.FileHash(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim"));
									address += 8;
									i = ByteConverter.ToInt16(datafile, address);
								}
								data.MD5Hash = string.Join("|", hashes.ToArray());
								nohash = true;
							}
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

					string masterObjectListOutputPath = string.Concat(projectFolderName, inifile.MasterObjectList);

					IniSerializer.Serialize(masterobjlist, masterObjectListOutputPath);
				}
				IniSerializer.Serialize(inifile, Path.Combine(projectFolderName, Path.GetFileNameWithoutExtension(datafilename) + "_data.ini"));
				timer.Stop();
				Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
				Console.WriteLine();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();
				return (int)ERRORVALUE.UnhandledException;
			}

			return (int)ERRORVALUE.Success;
		}
	}
}
