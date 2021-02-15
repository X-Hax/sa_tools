using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;

using SA_Tools;

namespace SonicRetro.SAModel.SAEditorCommon.StructConverter
{
	public static class StructConverter
	{
		public static readonly Dictionary<string, string> DataTypeList = new Dictionary<string, string>()
		{
			{ "landtable", "LandTable" },
			{ "model", "Model" },
			{ "basicmodel", "Basic Model" },
			{ "basicdxmodel", "Basic Model (SADX)" },
			{ "chunkmodel", "Chunk Model" },
			{ "gcmodel", "SA2B Model" },
			{ "action", "Action (animation+model)" },
			{ "animation", "Animation" },
			{ "objlist", "Object List" },
			{ "startpos", "Start Positions" },
			{ "texlist", "Texture List" },
			{ "leveltexlist", "Level Texture List" },
			{ "triallevellist", "Trial Level List" },
			{ "bosslevellist", "Boss Level List" },
			{ "fieldstartpos", "Field Start Positions" },
			{ "soundtestlist", "Sound Test List" },
			{ "musiclist", "Music List" },
			{ "soundlist", "Sound List" },
			{ "stringarray", "String Array" },
			{ "nextlevellist", "Next Level List" },
			{ "cutscenetext", "Cutscene Text" },
			{ "recapscreen", "Recap Screen" },
			{ "npctext", "NPC Text" },
			{ "levelclearflags", "Level Clear Flags" },
			{ "deathzone", "Death Zones" },
			{ "skyboxscale", "Skybox Scale" },
			{ "stageselectlist", "Stage Select List" },
			{ "levelrankscores", "Level Rank Scores" },
			{ "levelranktimes", "Level Rank Times" },
			{ "endpos", "End Positions" },
			{ "animationlist", "Animation List" },
			{ "enemyanimationlist", "Enemy Animation List" },
			{ "sa1actionlist", "Action List" },
			{ "motiontable", "Motion Table" },
			{ "levelpathlist", "Path List" },
			{ "pathlist", "Path List" },
			{ "stagelightdatalist", "Stage Light Data List" },
			{ "weldlist", "Weld List" },
			{ "bmitemattrlist", "BM Item Attributes List" },
			{ "creditstextlist", "Credits Text List" },
			{ "animindexlist", "Animation Index List" },
			{ "storysequence", "Story Sequence" },
			{ "string", "String" },
			{ "texnamearray", "Texture Name Array" },
			{ "texlistarray", "Texture List Array" },
			{ "physicsdata", "Player Parameters" }
		};

		private static void CheckItems(KeyValuePair<string, SA_Tools.FileInfo> item, SA_Tools.IniData iniData, ref Dictionary<string, bool> defaultExportState)
		{
			bool? modified = null;
			switch (item.Value.Type)
			{
				case "cutscenetext":
					{
						modified = false;
						string[] hashes = item.Value.MD5Hash.Split(',');
						for (int i = 0; i < 5; i++)
						{
							string textname = Path.Combine(item.Value.Filename, ((Languages)i).ToString() + ".txt");
							if (HelperFunctions.FileHash(textname) != hashes[i])
							{
								modified = true;
								break;
							}
						}
					}
					break;
				case "recapscreen":
					{
						modified = false;
						int count = int.Parse(item.Value.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						string[] hash2 = item.Value.MD5Hash.Split(':');
						string[][] hashes = new string[hash2.Length][];
						for (int i = 0; i < hash2.Length; i++)
							hashes[i] = hash2[i].Split(',');
						for (int i = 0; i < count; i++)
							for (int l = 0; l < 5; l++)
							{
								string textname = Path.Combine(Path.Combine(item.Value.Filename, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
								if (HelperFunctions.FileHash(textname) != hashes[i][l])
								{
									modified = true;
									break;
								}
							}
					}
					break;
				case "npctext":
					{
						modified = false;
						int count = int.Parse(item.Value.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						string[] hash2 = item.Value.MD5Hash.Split(':');
						string[][] hashes = new string[hash2.Length][];
						for (int i = 0; i < hash2.Length; i++)
							hashes[i] = hash2[i].Split(',');
						for (int l = 0; l < 5; l++)
							for (int i = 0; i < count; i++)
							{
								string textname = Path.Combine(Path.Combine(item.Value.Filename, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
								if (HelperFunctions.FileHash(textname) != hashes[l][i])
								{
									modified = true;
									break;
								}
							}
					}
					break;
				case "deathzone":
					{
						modified = false;
						string[] hashes = item.Value.MD5Hash.Split(',');
						if (HelperFunctions.FileHash(item.Value.Filename) != hashes[0])
						{
							modified = true;
							break;
						}
						DeathZoneFlags[] flags = DeathZoneFlagsList.Load(item.Value.Filename);
						if (flags.Length != hashes.Length - 1)
						{
							modified = true;
							break;
						}
						string path = Path.GetDirectoryName(item.Value.Filename);
						for (int i = 0; i < flags.Length; i++)
							if (HelperFunctions.FileHash(Path.Combine(path, i.ToString(NumberFormatInfo.InvariantInfo) + (iniData.Game == Game.SA2 || iniData.Game == Game.SA2B ? ".sa2mdl" : ".sa1mdl")))
								!= hashes[i + 1])
							{
								modified = true;
								break;
							}
					}
					break;
				case "levelpathlist":
					{
						modified = false;
						Dictionary<string, string[]> hashes = new Dictionary<string, string[]>();
						string[] hash1 = item.Value.MD5Hash.Split('|');
						foreach (string hash in hash1)
						{
							string[] hash2 = hash.Split(':');
							hashes.Add(hash2[0], hash2[1].Split(','));
						}
						foreach (string dir in Directory.GetDirectories(item.Value.Filename))
						{
							string name = new DirectoryInfo(dir).Name;
							if (!hashes.ContainsKey(name))
							{
								modified = true;
								break;
							}
						}
						if (modified.Value)
							break;
						foreach (KeyValuePair<string, string[]> dirinfo in hashes)
						{
							string dir = Path.Combine(item.Value.Filename, dirinfo.Key);
							if (!Directory.Exists(dir))
							{
								modified = true;
								break;
							}
							if (Directory.GetFiles(dir, "*.ini").Length != dirinfo.Value.Length)
							{
								modified = true;
								break;
							}
							for (int i = 0; i < dirinfo.Value.Length; i++)
							{
								string file = Path.Combine(dir, i.ToString(NumberFormatInfo.InvariantInfo) + ".ini");
								if (!File.Exists(file))
								{
									modified = true;
									break;
								}
								else if (HelperFunctions.FileHash(file) != dirinfo.Value[i])
								{
									modified = true;
									break;
								}
							}
							if (modified.Value)
								break;
						}
					}
					break;
				case "pathlist":
					{
						modified = false;
						string[] hashes = item.Value.MD5Hash.Split(',');
						if (Directory.GetFiles(item.Value.Filename, "*.ini").Length != hashes.Length)
						{
							modified = true;
							break;
						}
						for (int i = 0; i < hashes.Length; i++)
						{
							string file = Path.Combine(item.Value.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".ini");
							if (!File.Exists(file))
							{
								modified = true;
								break;
							}
							else if (HelperFunctions.FileHash(file) != hashes[i])
							{
								modified = true;
								break;
							}
						}
					}
					break;
				case "animindexlist":
					{
						modified = false;
						string[] md5KeyvaluePairs = item.Value.MD5Hash.Split('|');
						foreach (string md5KeyValuePair in md5KeyvaluePairs)
						{
							string[] keySplit = md5KeyValuePair.Split(':');

							string filePath = Path.Combine(item.Value.Filename, keySplit[0] + ".saanim");

							if (File.Exists(filePath))
							{
								if (HelperFunctions.FileHash(filePath) != keySplit[1])
								{
									modified = true;
									break;
								}
							}
						}
					}
						break;
				case "motiontable":
					{
						modified = false;
						string[] md5KeyvaluePairs = item.Value.MD5Hash.Split('|');
						foreach (string md5KeyValuePair in md5KeyvaluePairs)
						{
							string[] keySplit = md5KeyValuePair.Split(':');

							string filePath = Path.Combine(item.Value.Filename, keySplit[0] + ".saanim");
							string filePathIni = Path.Combine(item.Value.Filename, keySplit[0] + ".ini");

							if (File.Exists(filePath))
							{
								if (HelperFunctions.FileHash(filePath) != keySplit[1] || HelperFunctions.FileHash(filePathIni) != keySplit[1])
								{
									modified = true;
									break;
								}
							}
						}
						break;
					}
				default:
					if (!string.IsNullOrEmpty(item.Value.MD5Hash))
						modified = HelperFunctions.FileHash(item.Value.Filename) != item.Value.MD5Hash;
					break;
			}
			defaultExportState.Add(item.Key, modified ?? true);
		}

		public static SA_Tools.IniData LoadINI(string filename,
			ref Dictionary<string, bool> defaultExportState)
		{
			SA_Tools.IniData iniData = IniSerializer.Deserialize<SA_Tools.IniData>(filename);
			defaultExportState.Clear();

			Environment.CurrentDirectory = Path.GetDirectoryName(filename);

			foreach (KeyValuePair<string, SA_Tools.FileInfo> item in iniData.Files)
			{
				CheckItems(item, iniData, ref defaultExportState);
			}

			return iniData;
		}

		public static SA_Tools.IniData LoadMultiINI(List<string> filename,
			ref Dictionary<string, bool> defaultExportState)
		{
			defaultExportState.Clear();
			SA_Tools.IniData newiniData = new SA_Tools.IniData();
			Dictionary<string, SA_Tools.FileInfo> curItems = new Dictionary<string, SA_Tools.FileInfo>();

			foreach (string arrFile in filename)
			{
				SA_Tools.IniData iniData = IniSerializer.Deserialize<SA_Tools.IniData>(arrFile);

				Environment.CurrentDirectory = Path.GetDirectoryName(arrFile);

				foreach (KeyValuePair<string, SA_Tools.FileInfo> item in iniData.Files)
				{
					CheckItems(item, iniData, ref defaultExportState);

					curItems.Add(item.Key, item.Value);
				}
			}
			newiniData.Files = curItems;
			return newiniData;
		}

		public static void ExportINI(SA_Tools.IniData iniData,
			Dictionary<string, bool> itemsToExport, string fileName)
		{
			string dstfol = Path.GetDirectoryName(fileName);

			SA_Tools.IniData output = new SA_Tools.IniData
			{
				Files = new Dictionary<string, SA_Tools.FileInfo>()
			};

			foreach (KeyValuePair<string, SA_Tools.FileInfo> item in
				iniData.Files.Where(i => itemsToExport[i.Key]))
			{
				if (Directory.Exists(item.Value.Filename))
				{
					Directory.CreateDirectory(Path.Combine(dstfol, item.Value.Filename));
				}
				else
				{
					Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(dstfol, item.Value.Filename)));
				}

				switch (item.Value.Type)
				{
					case "deathzone":
						DeathZoneFlags[] list = DeathZoneFlagsList.Load(item.Value.Filename);
						string path = Path.GetDirectoryName(item.Value.Filename);
						for (int j = 0; j < list.Length; j++)
						{
							System.IO.FileInfo fil = new System.IO.FileInfo(Path.Combine(path, j.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl"));
							fil.CopyTo(Path.Combine(Path.Combine(dstfol, path), j.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl"), true);
						}
						File.Copy(item.Value.Filename, Path.Combine(dstfol, item.Value.Filename), true);
						break;
					default:
						if (Directory.Exists(item.Value.Filename))
							CopyDirectory(new DirectoryInfo(item.Value.Filename), Path.Combine(dstfol, item.Value.Filename));
						else
							File.Copy(item.Value.Filename, Path.Combine(dstfol, item.Value.Filename), true);
						break;
				}
				item.Value.MD5Hash = null;
				output.Files.Add(item.Key, item.Value);
			}
			IniSerializer.Serialize(output, fileName);
		}

		public static void ExportCPP(SA_Tools.IniData iniData,
			Dictionary<string, bool> itemsToExport, string fileName)
		{
			using (TextWriter writer = File.CreateText(fileName))
			{
				bool SA2 = iniData.Game == Game.SA2 || iniData.Game == Game.SA2B;
				Dictionary<uint, string> pointers = new Dictionary<uint, string>();
				List<string> initlines = new List<string>();
				uint imagebase = iniData.ImageBase ?? 0x400000;
				ModelFormat modelfmt = 0;
				LandTableFormat landfmt = 0;
				switch (iniData.Game)
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
				writer.WriteLine("// Generated by SA Tools Struct Converter");
				writer.WriteLine();
				if (SA2)
					writer.WriteLine("#include \"SA2ModLoader.h\"");
				else
					writer.WriteLine("#include \"SADXModLoader.h\"");
				writer.WriteLine();
				Dictionary<string, string> models = new Dictionary<string, string>();
				//foreach (KeyValuePair<string, SA_Tools.FileInfo> item in iniData.Files.Where((a, i) => listView1.CheckedIndices.Contains(i)))
				foreach (KeyValuePair<string, SA_Tools.FileInfo> item in
					iniData.Files.Where(i => itemsToExport[i.Key]))
				{
					string name = item.Key.MakeIdentifier();
					SA_Tools.FileInfo data = item.Value;
					switch (data.Type)
					{
						case "landtable":
							LandTable tbl = LandTable.LoadFromFile(data.Filename);
							name = tbl.Name;
							tbl.ToStructVariables(writer, landfmt, new List<string>());
							break;
						case "model":
							NJS_OBJECT mdl = new ModelFile(data.Filename).Model;
							name = mdl.Name;
							mdl.ToStructVariables(writer, modelfmt == ModelFormat.BasicDX, new List<string>());
							models.Add(item.Key, mdl.Name);
							break;
						case "basicmodel":
							mdl = new ModelFile(data.Filename).Model;
							name = mdl.Name;
							mdl.ToStructVariables(writer, false, new List<string>());
							models.Add(item.Key, mdl.Name);
							break;
						case "basicdxmodel":
							mdl = new ModelFile(data.Filename).Model;
							name = mdl.Name;
							mdl.ToStructVariables(writer, true, new List<string>());
							models.Add(item.Key, mdl.Name);
							break;
						case "chunkmodel":
							mdl = new ModelFile(data.Filename).Model;
							name = mdl.Name;
							mdl.ToStructVariables(writer, false, new List<string>());
							models.Add(item.Key, mdl.Name);
							break;
						case "gcmodel":
							mdl = new ModelFile(data.Filename).Model;
							name = mdl.Name;
							mdl.ToStructVariables(writer, false, new List<string>());
							models.Add(item.Key, mdl.Name);
							break;
						case "action":
							NJS_MOTION ani = NJS_MOTION.Load(data.Filename);
							name = "action_" + ani.Name.MakeIdentifier();
							ani.ToStructVariables(writer);
							writer.WriteLine();
							writer.WriteLine("NJS_ACTION {0} = {{ &{1}, &{2} }};", name, models[data.CustomProperties["model"]], ani.Name.MakeIdentifier());
							break;
						case "animation":
							ani = NJS_MOTION.Load(data.Filename);
							name = ani.Name.MakeIdentifier();
							writer.WriteLine(ani.ToStructVariables());
							break;
						case "objlist":
							{
								ObjectListEntry[] list = ObjectList.Load(data.Filename, SA2);
								writer.WriteLine("ObjectListEntry {0}_list[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (ObjectListEntry obj in list)
									objs.Add(obj.ToStruct() + " " + obj.Name.ToComment());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								writer.WriteLine("ObjectList {0} = {{ arraylengthandptrT({0}_list, int) }};", name);
							}
							break;
						case "startpos":
							if (SA2)
							{
								Dictionary<SA2LevelIDs, SA2StartPosInfo> list = SA2StartPosList.Load(data.Filename);
								writer.WriteLine("StartPosition {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count + 1);
								foreach (KeyValuePair<SA2LevelIDs, SA2StartPosInfo> obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ LevelIDs_Invalid }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							else
							{
								Dictionary<SA1LevelAct, SA1StartPosInfo> list = SA1StartPosList.Load(data.Filename);
								writer.WriteLine("StartPosition {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count + 1);
								foreach (KeyValuePair<SA1LevelAct, SA1StartPosInfo> obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ LevelIDs_Invalid }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "texlist":
							{
								TextureListEntry[] list = TextureList.Load(data.Filename);
								writer.WriteLine("PVMEntry {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length + 1);
								foreach (TextureListEntry obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ 0 }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "leveltexlist":
							{
								LevelTextureList list = LevelTextureList.Load(data.Filename);
								writer.WriteLine("PVMEntry {0}_list[] = {{", name);
								List<string> objs = new List<string>(list.TextureList.Length);
								foreach (TextureListEntry obj in list.TextureList)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								writer.WriteLine("LevelPVMList {0} = {{ {1}, arraylengthandptrT({0}_list, int16_t) }};", name, list.Level.ToC());
							}
							break;
						case "triallevellist":
							{
								SA1LevelAct[] list = TrialLevelList.Load(data.Filename);
								writer.WriteLine("TrialLevelListEntry {0}_list[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (SA1LevelAct obj in list)
									objs.Add(string.Format("{{ {0}, {1} }}", obj.Level.ToC("LevelIDs"), obj.Act));
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								writer.WriteLine("TrialLevelList {0} = {{ arrayptrandlengthT({0}_list, int) }};", name);
								initlines.Add(string.Format("*(TrialLevelList*)0x{0:X} = {1};", data.Address + imagebase, name));
							}
							break;
						case "bosslevellist":
							{
								SA1LevelAct[] list = BossLevelList.Load(data.Filename);
								writer.WriteLine("__int16 {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length + 1);
								foreach (SA1LevelAct obj in list)
									objs.Add(obj.ToC());
								objs.Add("levelact(LevelIDs_Invalid, 0)");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "fieldstartpos":
							{
								Dictionary<SA1LevelIDs, FieldStartPosInfo> list = FieldStartPosList.Load(data.Filename);
								writer.WriteLine("FieldStartPosition {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count + 1);
								foreach (KeyValuePair<SA1LevelIDs, FieldStartPosInfo> obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ LevelIDs_Invalid }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "soundtestlist":
							{
								SoundTestListEntry[] list = SoundTestList.Load(data.Filename);
								writer.WriteLine("SoundTestEntry {0}_list[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (SoundTestListEntry obj in list)
									objs.Add(obj.ToStruct() + " " + obj.Title.ToComment());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								writer.WriteLine("SoundTestCategory {0} = {{ arrayptrandlengthT({0}_list, int) }};", name);
								initlines.Add(string.Format("*(SoundTestCategory*)0x{0:X} = {1};", data.Address + imagebase, name));
							}
							break;
						case "musiclist":
							{
								MusicListEntry[] list = MusicList.Load(data.Filename);
								writer.WriteLine("MusicInfo {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (MusicListEntry obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								initlines.Add(string.Format("memcpy((MusicInfo*)0x{0:X}, arrayptrandsize({1}));", data.Address + imagebase, name));
							}
							break;
						case "soundlist":
							{
								SoundListEntry[] list = SoundList.Load(data.Filename);
								writer.WriteLine("SoundFileInfo {0}_list[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (SoundListEntry obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								writer.WriteLine("SoundList {0} = {{ arraylengthandptrT({0}_list, int) }};", name);
								initlines.Add(string.Format("*(SoundList*)0x{0:X} = {1};", data.Address + imagebase, name));
							}
							break;
						case "stringarray":
							{
								string[] strs = StringArray.Load(data.Filename);
								Languages lang = Languages.Japanese;
								if (data.CustomProperties.ContainsKey("language"))
									lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
								writer.WriteLine("char *{0}[] = {{", name);
								List<string> objs = new List<string>(strs.Length);
								foreach (string obj in strs)
									objs.Add(obj.ToC(lang) + " " + obj.ToComment());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								initlines.Add(string.Format("memcpy((char**)0x{0:X}, arrayptrandsize({1}));", data.Address + imagebase, name));
							}
							break;
						case "nextlevellist":
							{
								NextLevelListEntry[] list = NextLevelList.Load(data.Filename);
								writer.WriteLine("NextLevelData {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length + 1);
								foreach (NextLevelListEntry obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ 0, -1 }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "cutscenetext":
							{
								CutsceneText texts = new CutsceneText(data.Filename);
								uint addr = (uint)(data.Address + imagebase);
								for (int j = 0; j < 5; j++)
								{
									string[] strs = texts.Text[j];
									Languages lang = (Languages)j;
									writer.WriteLine("char *{0}_{1}[] = {{", name, lang);
									writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", strs.Select((a) => a.ToC(lang) + " " + a.ToComment()).ToArray()));
									writer.WriteLine("};");
									writer.WriteLine();
									pointers.Add(addr, string.Format("{0}_{1}", name, lang));
									addr += 4;
								}
							}
							break;
						case "recapscreen":
							{
								uint addr = (uint)(data.Address + imagebase);
								RecapScreen[][] texts = RecapScreenList.Load(data.Filename, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
								for (int l = 0; l < 5; l++)
									for (int j = 0; j < texts.Length; j++)
									{
										writer.WriteLine("char *{0}_{1}_{2}_Text[] = {{", name, (Languages)l, j);
										writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texts[j][l].Text.Split('\n').Select((a) => a.ToC((Languages)l) + " " + a.ToComment()).ToArray()));
										writer.WriteLine("};");
										writer.WriteLine();
									}
								for (int l = 0; l < 5; l++)
								{
									writer.WriteLine("RecapScreen {0}_{1}[] = {{", name, (Languages)l);
									List<string> objs = new List<string>(texts.Length);
									for (int j = 0; j < texts.Length; j++)
									{
										RecapScreen scr = texts[j][l];
										objs.Add(string.Format("{{ {0}, arraylengthandptrT({1}_{2}_{3}_Text, int) }}",
											scr.Speed.ToC(), name, (Languages)l, j));
									}
									writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
									writer.WriteLine("};");
									writer.WriteLine();
									pointers.Add(addr, string.Format("{0}_{1}", name, (Languages)l));
									addr += 4;
								}
							}
							break;
						case "npctext":
							{
								NPCText[][] texts = NPCTextList.Load(data.Filename, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
								uint headaddr = (uint)(data.Address + imagebase);
								for (int l = 0; l < 5; l++)
								{
									for (int j = 0; j < texts[l].Length; j++)
									{
										if (texts[l][j].Groups.Count == 0)
											continue;
										if (texts[l][j].HasControl)
										{
											writer.WriteLine("__int16 {0}_{1}_{2}_Control[] = {{", name, (Languages)l, j);
											bool first = true;
											List<string> objs = new List<string>();
											foreach (NPCTextGroup group in texts[l][j].Groups)
											{
												if (!first)
													objs.Add(NPCTextControl.NewGroup.ToC());
												else
													first = false;
												foreach (ushort flag in group.EventFlags)
												{
													objs.Add(NPCTextControl.EventFlag.ToC());
													objs.Add(flag.ToCHex());
												}
												foreach (ushort flag in group.NPCFlags)
												{
													objs.Add(NPCTextControl.NPCFlag.ToC());
													objs.Add(flag.ToCHex());
												}
												if (group.Character != (SA1CharacterFlags)0xFF)
												{
													objs.Add(NPCTextControl.Character.ToC());
													objs.Add(group.Character.ToC("CharacterFlags"));
												}
												if (group.Voice.HasValue)
												{
													objs.Add(NPCTextControl.Voice.ToC());
													objs.Add(group.Voice.Value.ToString());
												}
												if (group.SetEventFlag.HasValue)
												{
													objs.Add(NPCTextControl.SetEventFlag.ToC());
													objs.Add(group.SetEventFlag.Value.ToCHex());
												}
											}
											objs.Add(NPCTextControl.End.ToC());
											writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
											writer.WriteLine("};");
											writer.WriteLine();
										}
										if (texts[l][j].HasText)
										{
											writer.WriteLine("HintText_Text {0}_{1}_{2}_Text[] = {{", name, (Languages)l, j);
											List<string> objs = new List<string>();
											foreach (NPCTextGroup group in texts[l][j].Groups)
											{
												foreach (NPCTextLine line in group.Lines)
													objs.Add(line.ToStruct((Languages)l) + " " + line.Line.ToComment());
												objs.Add("{ 0 }");
											}
											writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
											writer.WriteLine("};");
											writer.WriteLine();
										}
									}
								}
								for (int l = 0; l < 5; l++)
								{
									if (l > 0)
										writer.WriteLine();
									writer.WriteLine("HintText_Entry {0}_{1}[] = {{", name, (Languages)l);
									List<string> objs = new List<string>();
									for (int j = 0; j < texts[l].Length; j++)
									{
										if (texts[l][j].Groups.Count == 0)
										{
											objs.Add("{ 0 }");
											continue;
										}
										StringBuilder line = new StringBuilder("{ ");
										if (texts[l][j].HasControl)
											line.AppendFormat("{0}_{1}_{2}_Control", name, (Languages)l, j);
										else
											line.Append("NULL");
										line.Append(", ");
										if (texts[l][j].HasText)
											line.AppendFormat("{0}_{1}_{2}_Text", name, (Languages)l, j);
										else
											line.Append("NULL");
										line.Append(" }");
										objs.Add(line.ToString());
									}
									writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
									writer.WriteLine("};");
									pointers.Add(headaddr, string.Format("{0}_{1}", name, (Languages)l));
									headaddr += 4;
								}
							}
							break;
						case "levelclearflags":
							{
								LevelClearFlag[] list = LevelClearFlagList.Load(data.Filename);
								writer.WriteLine("LevelClearFlagData {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (LevelClearFlag obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ -1 }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "deathzone":
							{
								DeathZoneFlags[] list = DeathZoneFlagsList.Load(data.Filename);
								string path = Path.GetDirectoryName(data.Filename);
								List<string> mdls = new List<string>(list.Length);
								List<string> objs = new List<string>();
								for (int j = 0; j < list.Length; j++)
								{
									NJS_OBJECT obj = new ModelFile(Path.Combine(path,
										j.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl")).Model;
									obj.ToStructVariables(writer, modelfmt == ModelFormat.BasicDX, objs);
									writer.WriteLine();
									mdls.Add(obj.Name);
									objs.Clear();
								}
								writer.WriteLine("DeathZone {0}[] = {{", name);
								for (int j = 0; j < list.Length; j++)
									objs.Add(string.Format("{{ {0}, &{1} }}", list[j].Flags.ToC("CharacterFlags"), mdls[j]));
								objs.Add("{ 0 }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "skyboxscale":
							{
								uint headaddr = (uint)(data.Address + imagebase);
								int cnt = int.Parse(data.CustomProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
								SkyboxScale[] sclini = SkyboxScaleList.Load(data.Filename);
								for (int j = 0; j < cnt; j++)
								{
									writer.WriteLine("SkyboxScale {0}_{1} = {2};", name, j, sclini[j].ToStruct());
									pointers.Add(headaddr, string.Format("{0}_{1}", name, j));
									headaddr += 4;
								}
							}
							break;
						case "stageselectlist":
							{
								StageSelectLevel[] list = StageSelectLevelList.Load(data.Filename);
								writer.WriteLine("StageSelectLevel {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (StageSelectLevel obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "levelrankscores":
							{
								Dictionary<SA2LevelIDs, LevelRankScores> list = LevelRankScoresList.Load(data.Filename);
								writer.WriteLine("LevelRankScores {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count);
								foreach (KeyValuePair<SA2LevelIDs, LevelRankScores> obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ LevelIDs_Invalid }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "levelranktimes":
							{
								Dictionary<SA2LevelIDs, LevelRankTimes> list = LevelRankTimesList.Load(data.Filename);
								writer.WriteLine("LevelRankTimes {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count);
								foreach (KeyValuePair<SA2LevelIDs, LevelRankTimes> obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ LevelIDs_Invalid }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "endpos":
							{
								Dictionary<SA2LevelIDs, SA2EndPosInfo> list = SA2EndPosList.Load(data.Filename);
								writer.WriteLine("LevelEndPosition {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count);
								foreach (KeyValuePair<SA2LevelIDs, SA2EndPosInfo> obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ LevelIDs_Invalid }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "animationlist":
							{
								SA2AnimationInfo[] list = SA2AnimationInfoList.Load(data.Filename);
								writer.WriteLine("AnimationInfo {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (SA2AnimationInfo obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "enemyanimationlist":
							{
								SA2EnemyAnimInfo[] list = SA2EnemyAnimInfoList.Load(data.Filename);
								writer.WriteLine("AnimationInfo {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (SA2EnemyAnimInfo obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "sa1actionlist":
							{
								SA1ActionInfo[] list = SA1ActionInfoList.Load(data.Filename);
								writer.WriteLine("AnimationInfo {0}[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (SA1ActionInfo obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "levelpathlist":
							{
								List<SA1LevelAct> levels = new List<SA1LevelAct>();
								foreach (string dir in Directory.GetDirectories(data.Filename))
								{
									SA1LevelAct level;
									try { level = new SA1LevelAct(new DirectoryInfo(dir).Name); }
									catch { continue; }
									levels.Add(level);
									List<PathData> paths = PathList.Load(dir);
									for (int i = 0; i < paths.Count; i++)
									{
										writer.WriteLine("Loop {0}_{1}_{2}_Entries[] = {{", name, level.ToString().MakeIdentifier(), i);
										List<string> objs = new List<string>(paths[i].Path.Count);
										foreach (PathDataEntry entry in paths[i].Path)
											objs.Add(entry.ToStruct());
										writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
										writer.WriteLine("};");
										writer.WriteLine();
										writer.WriteLine("LoopHead {0}_{1}_{2} = {{ {3}, LengthOfArray<int16_t>({0}_{1}_{2}_Entries), {4}, {0}_{1}_{2}_Entries, (ObjectFuncPtr){5} }};",
											name, level.ToString().MakeIdentifier(), i, paths[i].Unknown,
											paths[i].TotalDistance.ToC(),
											paths[i].Code.ToCHex());
										writer.WriteLine();
									}
									writer.WriteLine("LoopHead *{0}_{1}[] = {{", name, level.ToString().MakeIdentifier());
									for (int i = 0; i < paths.Count; i++)
										writer.WriteLine("\t&{0}_{1}_{2},", name, level.ToString().MakeIdentifier(), i);
									writer.WriteLine("\tNULL");
									writer.WriteLine("};");
									writer.WriteLine();
								}
								writer.WriteLine("PathDataPtr {0}[] = {{", name);
								foreach (SA1LevelAct level in levels)
									writer.WriteLine("\t{{ {0}, {1}_{2} }},", level.ToC(), name,
										level.ToString().MakeIdentifier());
								writer.WriteLine("\t{ 0xFFFF }");
								writer.WriteLine("};");
								writer.WriteLine();
							}
							break;
						case "pathlist":
							{
								List<PathData> paths = PathList.Load(data.Filename);
								for (int i = 0; i < paths.Count; i++)
								{
									writer.WriteLine("Loop {0}_{1}_Entries[] = {{", name, i);
									List<string> objs = new List<string>(paths[i].Path.Count);
									foreach (PathDataEntry entry in paths[i].Path)
										objs.Add(entry.ToStruct());
									writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
									writer.WriteLine("};");
									writer.WriteLine();
									writer.WriteLine("LoopHead {0}_{1} = {{ {2}, LengthOfArray<int16_t>({0}_{1}_Entries), {3}, {0}_{1}_Entries, (ObjectFuncPtr){4} }};",
										name, i, paths[i].Unknown,
										paths[i].TotalDistance.ToC(),
										paths[i].Code.ToCHex());
									writer.WriteLine();
								}
								writer.WriteLine("LoopHead *{0}[] = {{", name);
								for (int i = 0; i < paths.Count; i++)
									writer.WriteLine("\t&{0}_{1},", name, i);
								writer.WriteLine("\tNULL");
								writer.WriteLine("};");
								writer.WriteLine();
							}
							break;
						case "stagelightdatalist":
							{
								List<SA1StageLightData> list = SA1StageLightDataList.Load(data.Filename);
								writer.WriteLine("StageLightData {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count + 1);
								foreach (SA1StageLightData obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ 0xFFu }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "weldlist":
							{
								List<WeldInfo> list = WeldList.Load(data.Filename);
								List<string> labels = new List<string>();
								foreach (WeldInfo weld in list)
									if (weld.VertIndexes != null && weld.VertIndexes.Count > 0 && !labels.Contains(weld.VertIndexName))
									{
										writer.WriteLine("uint16_t {0}[] = {{", weld.VertIndexName);
										for (int i = 0; i < weld.VertIndexes.Count; i += 2)
											writer.WriteLine("\t{0}, {1},", weld.VertIndexes[i], weld.VertIndexes[i + 1]);
										writer.WriteLine("};");
										writer.WriteLine();
										labels.Add(weld.VertIndexName);
									}
								writer.WriteLine("WeldInfo {0}[] = {{", name);
								foreach (WeldInfo weld in list)
									writer.WriteLine("\t{0},", weld.ToStruct());
								writer.WriteLine("\t{ 0 }");
								writer.WriteLine("};");
								writer.WriteLine();
							}
							break;
						case "bmitemattrlist":
							{
								Dictionary<ChaoItemCategory, List<BlackMarketItemAttributes>> list = BlackMarketItemAttributesList.Load(data.Filename);
								foreach (var attrlist in list)
								{
									writer.WriteLine("BlackMarketItemAttributes {0}_{1}[] = {{", name, attrlist.Key);
									foreach (var attr in attrlist.Value)
										writer.WriteLine("\t{0},", attr.ToStruct());
									writer.WriteLine("};");
									writer.WriteLine();
								}
								writer.WriteLine("BlackMarketItemAttributesList {0}[] = {{");
								for (int i = 0; i < 11; i++)
									if (list.ContainsKey((ChaoItemCategory)i))
										writer.WriteLine("\t{{ arrayptrandlengthT({0}_{1}, int) }},", name, (ChaoItemCategory)i);
									else
										writer.WriteLine("\t{ 0 },");
								writer.WriteLine("};");
							}
							break;
						case "creditstextlist":
							{
								CreditsTextListEntry[] list = CreditsTextList.Load(data.Filename);
								writer.WriteLine("CreditsEntry {0}_list[] = {{", name);
								List<string> objs = new List<string>(list.Length);
								foreach (CreditsTextListEntry obj in list)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								writer.WriteLine("CreditsList {0} = {{ arrayptrandlengthT({0}_list, int) }};", name);
								initlines.Add(string.Format("*(CreditsList*)0x{0:X} = {1};", data.Address + imagebase, name));
							}
							break;
						case "animindexlist":
							{
								SortedDictionary<short, NJS_MOTION> anims = new SortedDictionary<short, NJS_MOTION>();
								foreach (string file in Directory.GetFiles(data.Filename, "*.saanim"))
									if (short.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out short i))
										anims.Add(i, NJS_MOTION.Load(file));
								foreach (KeyValuePair<short, NJS_MOTION> obj in anims)
								{
									obj.Value.ToStructVariables(writer);
									writer.WriteLine();
								}
								writer.WriteLine("AnimationIndex {0}[] = {{", name);
								List<string> objs = new List<string>(anims.Count);
								foreach (KeyValuePair<short, NJS_MOTION> obj in anims)
									objs.Add($"{{ {obj.Key}, {obj.Value.ModelParts}, &{obj.Value.Name} }}");
								objs.Add("{ -1 }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "motiontable":
							{
								foreach (string file in Directory.GetFiles(data.Filename, "*.saanim"))
								{
									NJS_MOTION.Load(file).ToStructVariables(writer);
									writer.WriteLine();
								}
								var table = IniSerializer.Deserialize<MotionTableEntry[]>(Path.Combine(data.Filename, "info.ini"));
								writer.WriteLine("MotionTableEntry {0}[] = {{", name);
								List<string> objs = new List<string>(table.Length);
								foreach (var obj in table)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "storysequence":
							{
								List<SA2StoryEntry> list = SA2StoryList.Load(data.Filename);
								writer.WriteLine("StoryEntry {0}[] = {{", name);
								List<string> objs = new List<string>(list.Count + 1);
								foreach (SA2StoryEntry obj in list)
									objs.Add(obj.ToStruct());
								objs.Add("{ StoryEntryType_End }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "string":
							{
								string str = File.ReadAllText(data.Filename).Replace("\r\n", "\n");
								Languages lang = Languages.Japanese;
								if (data.CustomProperties.ContainsKey("language"))
									lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
								writer.WriteLine("const char *{0} = {1}; {2}", name, str.ToC(lang), str.ToComment());
							}
							break;
						case "physicsdata":
							{
								PlayerParameter plpm = PlayerParameter.Load(data.Filename);
								writer.WriteLine("player_parameter {0} = {1};", name, plpm.ToStruct());
								initlines.Add(string.Format("*(player_parameter*)0x{0:X} = {1};", data.Address + imagebase, name));
							}
							break;
					}
					writer.WriteLine();
					if (data.PointerList != null && data.PointerList.Length > 0)
						foreach (int ptr in data.PointerList)
							pointers.Add((uint)(ptr + imagebase), name);
				}

				if (pointers.Count > 0)
				{
					writer.WriteLine("PointerInfo pointers[] = {");
					List<string> ptrs = new List<string>(pointers.Count);
					foreach (KeyValuePair<uint, string> ptr in pointers)
						ptrs.Add(string.Format("ptrdecl({0}, &{1})", ptr.Key.ToCHex(), ptr.Value));
					writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", ptrs.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}

				writer.WriteLine("extern \"C\"");
				writer.WriteLine("{");

				if (pointers.Count > 0)
				{
					writer.WriteLine("\t__declspec(dllexport) PointerList Pointers = { arrayptrandlengthT(pointers, int) };");
					writer.WriteLine();
				}

				if (initlines.Count > 0)
				{
					writer.WriteLine("\t__declspec(dllexport) void __cdecl Init(const char *path, const HelperFunctions &helperFunctions)");
					writer.WriteLine("\t{");
					foreach (string line in initlines)
						writer.WriteLine("\t\t{0}", line);
					writer.WriteLine("\t}");
					writer.WriteLine();
				}

				writer.WriteLine("\t__declspec(dllexport) ModInfo {0}ModInfo = {{ ModLoaderVer }};", SA2 ? "SA2" : "SADX");
				writer.WriteLine("}");
			}
		}

		public static void CopyDirectory(DirectoryInfo src, string dst)
		{
			if (!Directory.Exists(dst))
				Directory.CreateDirectory(dst);
			foreach (DirectoryInfo dir in src.GetDirectories())
				CopyDirectory(dir, Path.Combine(dst, dir.Name));
			foreach (System.IO.FileInfo fil in src.GetFiles())
				fil.CopyTo(Path.Combine(dst, fil.Name), true);
		}
	}
}
