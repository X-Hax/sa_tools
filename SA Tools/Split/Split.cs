using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using SA_Tools;
using SonicRetro.SAModel;

namespace SA_Tools.Split
{
	public static class Split
	{
		public static int SplitFile(string datafilename, string inifilename, string projectFolderName, bool nometa = false, bool nolabel = false)
		{
#if !DEBUG
			try
#endif
			{
				byte[] datafile;
				byte[] datafile_temp = File.ReadAllBytes(datafilename);
				IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
				string listfile = Path.Combine(Path.GetDirectoryName(inifilename), Path.GetFileNameWithoutExtension(datafilename) + "_labels.txt");
				Dictionary<int, string> labels = new Dictionary<int, string>();
				if (File.Exists(listfile) && !nolabel)
					labels = IniSerializer.Deserialize<Dictionary<int, string>>(listfile);
				if (inifile.StartOffset != 0)
				{
					byte[] datafile_new = new byte[inifile.StartOffset + datafile_temp.Length];
					datafile_temp.CopyTo(datafile_new, inifile.StartOffset);
					datafile = datafile_new;
				}
				else datafile = datafile_temp;
				if (inifile.MD5 != null && inifile.MD5.Count > 0)
				{
					string datahash = HelperFunctions.FileHash(datafile);
					if (!inifile.MD5.Any(h => h.Equals(datahash, StringComparison.OrdinalIgnoreCase)))
					{
						Console.WriteLine("The file {0} is not valid for use with the INI {1}.", datafilename, inifilename);
						return (int)SplitERRORVALUE.InvalidDataMapping;
					}
				}
				ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = inifile.BigEndian;
				ByteConverter.Reverse = SonicRetro.SAModel.ByteConverter.Reverse = inifile.Reverse;
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(datafilename));
				if (Path.GetExtension(datafilename).ToLowerInvariant() == ".prs" || (inifile.Compressed && Path.GetExtension(datafilename).ToLowerInvariant() != ".bin")) datafile = FraGag.Compression.Prs.Decompress(datafile);
				uint imageBase = HelperFunctions.SetupEXE(ref datafile) ?? inifile.ImageBase.Value;
				if (Path.GetExtension(datafilename).Equals(".rel", StringComparison.OrdinalIgnoreCase))
				{
					datafile = HelperFunctions.DecompressREL(datafile);
					HelperFunctions.FixRELPointers(datafile, imageBase);
				}
				bool SA2 = inifile.Game == Game.SA2 | inifile.Game == Game.SA2B;
				ModelFormat modelfmt_def = 0;
				LandTableFormat landfmt_def = 0;
				switch (inifile.Game)
				{
					case Game.SA1:
						modelfmt_def = ModelFormat.Basic;
						landfmt_def = LandTableFormat.SA1;
						break;
					case Game.SADX:
						modelfmt_def = ModelFormat.BasicDX;
						landfmt_def = LandTableFormat.SADX;
						break;
					case Game.SA2:
						modelfmt_def = ModelFormat.Chunk;
						landfmt_def = LandTableFormat.SA2;
						break;
					case Game.SA2B:
						modelfmt_def = ModelFormat.GC;
						landfmt_def = LandTableFormat.SA2B;
						break;
				}
				int itemcount = 0;
				Dictionary<string, MasterObjectListEntry> masterobjlist = new Dictionary<string, MasterObjectListEntry>();
				Dictionary<string, Dictionary<string, int>> objnamecounts = new Dictionary<string, Dictionary<string, int>>();
				Stopwatch timer = new Stopwatch();
				timer.Start();
				foreach (KeyValuePair<string, SA_Tools.FileInfo> item in new List<KeyValuePair<string, SA_Tools.FileInfo>>( inifile.Files))
				{
					if (string.IsNullOrEmpty(item.Key)) continue;
					string filedesc = item.Key;
					SA_Tools.FileInfo data = item.Value;
					Dictionary<string, string> customProperties = data.CustomProperties;
					string type = data.Type;
					int address = data.Address;
					bool nohash = false;

					string fileOutputPath = Path.Combine(projectFolderName, data.Filename);
					Console.WriteLine(item.Key + ": " + data.Address.ToString("X") + " → " + fileOutputPath);
					Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
					switch (type)
					{
						case "landtable":
							LandTableFormat format = data.CustomProperties.ContainsKey("format") ? (LandTableFormat)Enum.Parse(typeof(LandTableFormat), data.CustomProperties["format"]) : landfmt_def;
							new LandTable(datafile, address, imageBase, format, labels) { Description = item.Key }.SaveToFile(fileOutputPath, landfmt_def, nometa);
							break;
						case "model":
						case "basicmodel":
						case "basicdxmodel":
						case "chunkmodel":
						case "gcmodel":
							{
								ModelFormat mdlformat;
								switch (type)
								{
									case "basicmodel":
										mdlformat = ModelFormat.Basic;
										break;
									case "basicdxmodel":
										mdlformat = ModelFormat.BasicDX;
										break;
									case "chunkmodel":
										mdlformat = ModelFormat.Chunk;
										break;
									case "gcmodel":
										mdlformat = ModelFormat.GC;
										break;
									case "model":
									default:
										mdlformat = modelfmt_def;
										break;
								}
								if (data.CustomProperties.ContainsKey("format"))
									mdlformat = (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]);
								NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, mdlformat, labels, new Dictionary<int, Attach>());
								string[] mdlanis = new string[0];
								if (customProperties.ContainsKey("animations"))
									mdlanis = customProperties["animations"].Split(',');
								string[] mdlmorphs = new string[0];
								if (customProperties.ContainsKey("morphs"))
									mdlmorphs = customProperties["morphs"].Split(',');
								ModelFile.CreateFile(fileOutputPath, mdl, mdlanis, null, item.Key, null, mdlformat, nometa);
							}
							break;
						case "action":
							{
								ModelFormat modelfmt_act = data.CustomProperties.ContainsKey("format") ? (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]) : modelfmt_def;
								NJS_ACTION ani = new NJS_ACTION(datafile, address, imageBase, modelfmt_act, labels, new Dictionary<int, Attach>());
								if (!labels.ContainsValue(ani.Name) && !nolabel) ani.Name = filedesc;
								if (customProperties.ContainsKey("numparts"))
									ani.Animation.ModelParts = int.Parse(customProperties["numparts"]);
								if (ani.Animation.ModelParts == 0)
								{
									Console.WriteLine("Action {0} has no model data!", ani.Name);
									continue;
								}
								ani.Animation.Save(fileOutputPath, nometa);
							}
							break;
						case "animation":
						case "motion":
							int numparts = 0;
							if (customProperties.ContainsKey("numparts"))
								numparts = int.Parse(customProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
							else
								Console.WriteLine("Number of parts not specified for {0}", filedesc);
							if (customProperties.ContainsKey("shortrot"))
							{
								NJS_MOTION mot = new NJS_MOTION(datafile, address, imageBase, numparts , labels, bool.Parse(customProperties["shortrot"]));
								if (!labels.ContainsKey(address) && !nolabel) mot.Name = filedesc;
								mot.Save(fileOutputPath, nometa);
							}
							else
							{
								NJS_MOTION mot = new NJS_MOTION(datafile, address, imageBase, numparts, labels);
								if (!labels.ContainsKey(address) && !nolabel) mot.Name = filedesc;
								mot.Save(fileOutputPath, nometa);
							}
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
						case "texnamearray":
							TexnameArray texnames = new TexnameArray(datafile, address, imageBase);
							texnames.Save(fileOutputPath);
							break;
						case "texlistarray":
							{
								int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								for (int i = 0; i < cnt; i++)
								{
									uint ptr = BitConverter.ToUInt32(datafile, address);
									if (data.Filename != null && ptr != 0)
									{
										ptr -= imageBase;
										TexnameArray texarr = new TexnameArray(datafile, (int)ptr, imageBase);
										string fn = Path.Combine(fileOutputPath, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".txt");
										if (data.CustomProperties.ContainsKey("filename" + i.ToString()))
										{
											fn = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString()] + ".txt");
										}
										if (!Directory.Exists(Path.GetDirectoryName(fn)))
											Directory.CreateDirectory(Path.GetDirectoryName(fn));
										texarr.Save(fn);
									}
									address += 4;
								}
								nohash = true;
							}
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
									string file_tosave;
									if (customProperties.ContainsKey("filename" + num.ToString()))
										file_tosave = customProperties["filename" + num++.ToString()];
									else
										file_tosave = num++.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl";
									string file = Path.Combine(path, file_tosave);
									flags.Add(new DeathZoneFlags(datafile, address, file_tosave));
									ModelFormat modelfmt_death = inifile.Game == Game.SADX ? ModelFormat.BasicDX : ModelFormat.Basic; // Death zones in all games except SADXPC use Basic non-DX models
									ModelFile.CreateFile(file, new NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, modelfmt_death, new Dictionary<int, Attach>()), null, null, null, null, modelfmt_death, nometa);
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
						case "enemyanimationlist":
							{
								int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								SA2EnemyAnimInfoList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath);
							}
							break;
						case "sa1actionlist":
							{
								int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								SA1ActionInfoList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath);
							}
							break;
						case "motiontable":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<MotionTableEntry> result = new List<MotionTableEntry>();
								List<string> hashes = new List<string>();
								bool shortrot = false;
								if (customProperties.ContainsKey("shortrot"))
									shortrot = bool.Parse(customProperties["shortrot"]);
								int nodeCount = int.Parse(data.CustomProperties["nodecount"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
								int Length = int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								Dictionary<int, string> mtns = new Dictionary<int, string>();
								for (int i = 0; i < Length; i++)
								{
									MotionTableEntry bmte = new MotionTableEntry();
									int mtnaddr = (int)(ByteConverter.ToUInt32(datafile, address) - imageBase);
									if (!mtns.ContainsKey(mtnaddr))
									{
										NJS_MOTION motion = new NJS_MOTION(datafile, mtnaddr, imageBase, nodeCount, null, shortrot);
										bmte.Motion = motion.Name;
										mtns.Add(mtnaddr, motion.Name);
										motion.Save(Path.Combine(fileOutputPath, $"{i}.saanim"), nometa);
										hashes.Add($"{i}.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{i}.saanim")));
									}
									else
									bmte.Motion = mtns[mtnaddr];
									bmte.LoopProperty = ByteConverter.ToUInt16(datafile, address + 4);
									bmte.Pose = ByteConverter.ToUInt16(datafile, address + 6);
									bmte.NextAnimation = ByteConverter.ToInt32(datafile, address + 8);
									bmte.TransitionSpeed = ByteConverter.ToUInt32(datafile, address + 0xC);
									bmte.StartFrame = ByteConverter.ToSingle(datafile, address + 0x10);
									bmte.EndFrame = ByteConverter.ToSingle(datafile, address + 0x14);
									bmte.PlaySpeed = ByteConverter.ToSingle(datafile, address + 0x18);
									result.Add(bmte);
									address += 0x1C;
								}
								IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
								hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
								data.MD5Hash = string.Join("|", hashes.ToArray());
								nohash = true;
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
									new NJS_MOTION(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ByteConverter.ToInt16(datafile, address + 2))
										.Save(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim", nometa);
									hashes.Add(i.ToString(NumberFormatInfo.InvariantInfo) + ":" + HelperFunctions.FileHash(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim"));
									address += 8;
									i = ByteConverter.ToInt16(datafile, address);
								}
								data.MD5Hash = string.Join("|", hashes.ToArray());
								nohash = true;
							}
							break;
						case "storysequence":
							SA2StoryList.Load(datafile, address).Save(fileOutputPath);
							break;
						case "masterstringlist":
							{
								int cnt = int.Parse(customProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								for (int l = 0; l < 5; l++)
								{
									Languages lng = (Languages)l;
									System.Text.Encoding enc = HelperFunctions.GetEncoding(inifile.Game, lng);
									string ld = Path.Combine(fileOutputPath, lng.ToString());
									Directory.CreateDirectory(ld);
									int ptr = datafile.GetPointer(address, imageBase);
									for (int i = 0; i < cnt; i++)
									{
										int ptr2 = datafile.GetPointer(ptr, imageBase);
										if (ptr2 != 0)
										{
											string fn = Path.Combine(ld, $"{i}.txt");
											File.WriteAllText(fn, datafile.GetCString(ptr2, enc).Replace("\n", "\r\n"));
											inifile.Files.Add($"{filedesc} {lng} {i}", new FileInfo() { Type = "string", Filename = fn, PointerList = new int[] { ptr }, MD5Hash = HelperFunctions.FileHash(fn), CustomProperties = new Dictionary<string, string>() { { "language", lng.ToString() } } });
										}
										ptr += 4;
									}
									address += 4;
								}
								inifile.Files.Remove(filedesc);
								nohash = true;
							}
							break;
						case "camera":
							NinjaCamera cam = new NinjaCamera(datafile, address);
							cam.Save(fileOutputPath);
							break;
						case "fogdatatable":
							int fcnt = 3;
							if (customProperties.ContainsKey("count"))
								fcnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
							FogDataTable fga = new FogDataTable(datafile, address, imageBase, fcnt);
							fga.Save(fileOutputPath);
							break;
						case "palettelightlist":
							int count = 255;
							if (customProperties.ContainsKey("count"))
								count = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
							PaletteLightList pllist = new PaletteLightList(datafile, address, count);
							pllist.Save(fileOutputPath);
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

					string masterObjectListOutputPath = Path.Combine(projectFolderName, inifile.MasterObjectList);

					IniSerializer.Serialize(masterobjlist, masterObjectListOutputPath);
				}
				IniSerializer.Serialize(inifile, Path.Combine(projectFolderName, Path.GetFileNameWithoutExtension(inifilename) + "_data.ini"));
				timer.Stop();
				Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
				Console.WriteLine();
			}
#if !DEBUG
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();
				return (int)SplitERRORVALUE.UnhandledException;
			}
#endif
			return (int)SplitERRORVALUE.Success;
		}
	}
}
