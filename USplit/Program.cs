using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;
using SA_Tools;

namespace USplit
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] arguments = Environment.GetCommandLineArgs();
			Game game;
			string filename;
			string dir = Environment.CurrentDirectory;
			bool bigendian = false;
			if (args.Length == 0)
			{
				Console.WriteLine("USplit is a tool that lets you extract any data supported by SA Tools from any binary file.");
				Console.WriteLine("Usage: usplit <GAME> <FILENAME> <KEY> <TYPE> <ADDRESS> <PARAMETER1> <PARAMETER2> [-name <NAME>]\n");
				Console.WriteLine("Argument description:");
				Console.WriteLine("<GAME>: SA1, SADX, SA2, SA2B. Add '_b' (e.g. SADX_b) to switch to Big Endian.\n");
				Console.WriteLine("<FILENAME>: The name of the binary file, e.g. sonic.exe.\n");
				Console.WriteLine("<KEY>: Binary key, e.g. 400000 for sonic.exe or C900000 for SA1 STG file.\n");
				Console.WriteLine("<TYPE>: One of the following:\n" +
					"binary <length> [hex],\nlandtable, model, basicmodel, basicdxmodel, chunkmodel, gcmodel, action, animation <NJS_OBJECT address> [shortrot],\n" +
					"objlist, startpos, texlist, leveltexlist, triallevellist, bosslevellist, fieldstartpos, soundlist, soundtestlist,\nnextlevellist, "+
					"levelclearflags, deathzone, levelrankscores, levelranktimes, endpos, levelpathlist, pathlist,\nstagelightdatalist, weldlist" +
					"bmitemattrlist, creditstextlist, animindexlist, storysequence, musiclist <count>,\n" +
					"stringarray <count> [language], skyboxscale <count>, stageselectlist <count>, animationlist <count>,\n" +
					"masterstringlist <count>, cutscenetext <count>, recapscreen <count>, npctext <count>\n");
				Console.WriteLine("<ADDRESS>: The location of data in the file.\n");
				Console.WriteLine("<PARAMETER1>: length, count, secondary address etc. depending on data type\n");
				Console.WriteLine("<PARAMETER2>: 'hex' for binary to read length as hexadecimal, 'shortrot' for animation to read rotation as short\n");
				Console.WriteLine("<NAME>: Output file name (optional)\n");
				Console.WriteLine("Press ENTER to exit");
				Console.ReadLine();
				return;
			}
			//Args list: game, filename, key, type, address, [address2/count], [language], [name]
			switch (args[0].ToLowerInvariant())
			{
				case "sa1":
					game = Game.SA1;
					break;
				case "sa1_b":
					game = Game.SA1;
					bigendian = true;
					break;
				case "sadx":
					game = Game.SADX;
					break;
				case "sadx_b":
					game = Game.SADX;
					bigendian = true;
					break;
				case "sa2":
					game = Game.SA2;
					break;
				case "sa2_b":
					game = Game.SA2;
					bigendian = true;
					break;
				case "sa2b":
					game = Game.SA2B;
					break;
				case "sa2b_b":
					game = Game.SA2B;
					bigendian = true;
					break;
				default:
					Console.WriteLine("Error parsing game type.\nCorrect game types are: SA1, SADX, SA2, SA2B.");
					Console.WriteLine("Press ENTER to exit.");
					Console.ReadLine();
					return;
			}
			string model_extension = ".sa1mdl";
			string landtable_extension = ".sa1lvl";
			ByteConverter.BigEndian = SonicRetro.SAModel.ByteConverter.BigEndian = bigendian;
			filename = args[1];
			byte[] datafile = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).ToLowerInvariant() == ".prs") datafile = FraGag.Compression.Prs.Decompress(datafile);
			Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
			uint imageBase = uint.Parse(args[2], NumberStyles.AllowHexSpecifier);
			string type = args[3];
			int address = int.Parse(args[4], NumberStyles.AllowHexSpecifier);
			bool SA2 = game == Game.SA2 | game == Game.SA2B;
			ModelFormat modelfmt = ModelFormat.BasicDX;
			LandTableFormat landfmt = LandTableFormat.SADX;
			switch (game)
			{
				case Game.SA1:
					modelfmt = ModelFormat.Basic;
					landfmt = LandTableFormat.SA1;
					model_extension = ".sa1mdl";
					landtable_extension = ".sa1lvl";
					break;
				case Game.SADX:
					modelfmt = ModelFormat.BasicDX;
					landfmt = LandTableFormat.SADX;
					model_extension = ".sa1mdl";
					landtable_extension = ".sa1lvl";
					break;
				case Game.SA2:
					modelfmt = ModelFormat.Chunk;
					landfmt = LandTableFormat.SA2;
					model_extension = ".sa2mdl";
					landtable_extension = ".sa2lvl";
					break;
				case Game.SA2B:
					modelfmt = ModelFormat.Chunk;
					landfmt = LandTableFormat.SA2B;
					model_extension = ".sa2mdl";
					landtable_extension = ".sa2blvl";
					break;
			}
			Dictionary<string, MasterObjectListEntry> masterobjlist = new Dictionary<string, MasterObjectListEntry>();
			Dictionary<string, Dictionary<string, int>> objnamecounts = new Dictionary<string, Dictionary<string, int>>();
			string fileOutputPath = dir + "\\" + address.ToString("X");
			Console.WriteLine("Game: {0}, file: {1}, key: 0x{2}, splitting {3} at 0x{4}", game.ToString(), filename, imageBase.ToString("X"), type, address.ToString("X"));
			if (args[args.Length - 2] == "-name")
			{
				fileOutputPath = dir + "\\" + args[args.Length - 1];
				Console.WriteLine("Name: {0}", args[args.Length - 1]);
			}
			switch (type.ToLowerInvariant())
			{
				case "landtable":
					new LandTable(datafile, address, imageBase, landfmt).SaveToFile(fileOutputPath + landtable_extension, landfmt);
					break;
				case "model":
					{
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, modelfmt, new Dictionary<int, Attach>());
						ModelFile.CreateFile(fileOutputPath + model_extension, mdl, null, null, null, null, modelfmt);
					}
					break;
				case "basicmodel":
					{
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.Basic, new Dictionary<int, Attach>());
						ModelFile.CreateFile(fileOutputPath + ".sa1mdl", mdl, null, null, null, null, ModelFormat.Basic);
					}
					break;
				case "basicdxmodel":
					{
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
						ModelFile.CreateFile(fileOutputPath + ".sa1mdl", mdl, null, null, null, null, ModelFormat.BasicDX);
					}
					break;
				case "chunkmodel":
					{
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
						ModelFile.CreateFile(fileOutputPath + ".sa2mdl", mdl, null, null, null, null, ModelFormat.Chunk);
					}
					break;
				case "gcmodel":
					{
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.GC, new Dictionary<int, Attach>());
						ModelFile.CreateFile(fileOutputPath + ".sa2mdl", mdl, null, null, null, null, ModelFormat.GC);
					}
					break;
				case "action":
					{
						NJS_ACTION ani = new NJS_ACTION(datafile, address, imageBase, modelfmt, new Dictionary<int, Attach>());
						ani.Animation.Save(fileOutputPath + ".saanim");
						string[] mdlanis = new string[0];
						NJS_OBJECT mdl = ani.Model;
						mdlanis = (fileOutputPath + ".saanim").Split(',');
						ModelFile.CreateFile(fileOutputPath + "_model" + model_extension, mdl, mdlanis, null, null, null, modelfmt);
					}
					break;
				case "animation":
					{
						bool shortrot_enabled = false;
						if (args.Length > 6 && args[6] == "shortrot") shortrot_enabled = true;
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, int.Parse(args[5], NumberStyles.AllowHexSpecifier), imageBase, modelfmt, new Dictionary<int, Attach>());
						new NJS_MOTION(datafile, address, imageBase, mdl.CountAnimated(), shortrot: shortrot_enabled).Save(fileOutputPath + ".saanim");
						string[] mdlanis = new string[0];
						mdlanis = (fileOutputPath + ".saanim").Split(',');
						ModelFile.CreateFile(fileOutputPath + "_model" + model_extension, mdl, mdlanis, null, null, null, modelfmt);
					}
					break;
				case "objlist":
					{
						ObjectListEntry[] objs = ObjectList.Load(datafile, address, imageBase, SA2);
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
						objs.Save(fileOutputPath+".ini");
					}
					break;
				case "startpos":
					if (SA2)
						SA2StartPosList.Load(datafile, address).Save(fileOutputPath + ".ini");
					else
						SA1StartPosList.Load(datafile, address).Save(fileOutputPath + ".ini");
					break;
				case "texlist":
					TextureList.Load(datafile, address, imageBase).Save(fileOutputPath + ".ini");
					break;
				case "leveltexlist":
					new LevelTextureList(datafile, address, imageBase).Save(fileOutputPath + ".ini");
					break;
				case "triallevellist":
					TrialLevelList.Save(TrialLevelList.Load(datafile, address, imageBase), fileOutputPath + ".ini");
					break;
				case "bosslevellist":
					BossLevelList.Save(BossLevelList.Load(datafile, address), fileOutputPath + ".ini");
					break;
				case "fieldstartpos":
					FieldStartPosList.Load(datafile, address).Save(fileOutputPath + ".ini");
					break;
				case "soundtestlist":
					SoundTestList.Load(datafile, address, imageBase).Save(fileOutputPath + ".ini");
					break;
				case "musiclist":
					{
						int muscnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						MusicList.Load(datafile, address, imageBase, muscnt).Save(fileOutputPath + ".ini");
					}
					break;
				case "soundlist":
					SoundList.Load(datafile, address, imageBase).Save(fileOutputPath + ".ini");
					break;
				case "stringarray":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						Languages lang = Languages.Japanese;
						if (args.Length > 6) 
							lang = (Languages)Enum.Parse(typeof(Languages), args[6], true);
						StringArray.Load(datafile, address, imageBase, cnt, lang).Save(fileOutputPath + ".txt");
					}
					break;
				case "nextlevellist":
					NextLevelList.Load(datafile, address).Save(fileOutputPath + ".ini");
					break;
				case "cutscenetext":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						new CutsceneText(datafile, address, imageBase, cnt).Save(fileOutputPath + ".txt", out string[] hashes);
					}
					break;
				case "recapscreen":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						RecapScreenList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath + ".txt", out string[][] hashes);
					}
					break;
				case "npctext":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						NPCTextList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath + ".txt", out string[][] hashes);
					}
					break;
				case "levelclearflags":
					LevelClearFlagList.Save(LevelClearFlagList.Load(datafile, address), fileOutputPath + ".ini");
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
							ModelFile.CreateFile(file, new NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, modelfmt, new Dictionary<int, Attach>()), null, null, null, null, modelfmt);
							address += 8;
						}
						flags.ToArray().Save(fileOutputPath + ".ini");
					}
					break;
				case "skyboxscale":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						SkyboxScaleList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath + ".ini");
					}
					break;
				case "stageselectlist":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						StageSelectLevelList.Load(datafile, address, cnt).Save(fileOutputPath + ".ini");
					}
					break;
				case "levelrankscores":
					LevelRankScoresList.Load(datafile, address).Save(fileOutputPath + ".ini");
					break;
				case "levelranktimes":
					LevelRankTimesList.Load(datafile, address).Save(fileOutputPath + ".ini");
					break;
				case "endpos":
					SA2EndPosList.Load(datafile, address).Save(fileOutputPath + ".ini");
					break;
				case "animationlist":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						SA2AnimationInfoList.Load(datafile, address, cnt).Save(fileOutputPath + ".ini");
					}
					break;
				case "levelpathlist":
					{
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
							}
							address += 8;
							lvlnum = (ushort)ByteConverter.ToUInt32(datafile, address);
						}
					}
					break;
				case "pathlist":
					{
						PathList.Load(datafile, address, imageBase).Save(fileOutputPath, out string[] hashes);
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
								.Save(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
							hashes.Add(i.ToString(NumberFormatInfo.InvariantInfo) + ":" + HelperFunctions.FileHash(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim"));
							address += 8;
							i = ByteConverter.ToInt16(datafile, address);
						}
					}
					break;
				case "storysequence":
					SA2StoryList.Load(datafile, address).Save(fileOutputPath);
					break;
				case "masterstringlist":
					{
						int cnt = int.Parse(args[5], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						for (int l = 0; l < 5; l++)
						{
							Languages lng = (Languages)l;
							System.Text.Encoding enc = HelperFunctions.GetEncoding(game, lng);
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
								}
								ptr += 4;
							}
							address += 4;
						}
					}
					break;
				case "binary":
					{
						int length;
						if (args.Length > 6 && args[6] == "hex") length = int.Parse(args[5], NumberStyles.AllowHexSpecifier);
						else length = int.Parse(args[5]);
						byte[] bin = new byte[length];
						Array.Copy(datafile, address, bin, 0, bin.Length);
						File.WriteAllBytes(fileOutputPath + ".bin", bin);
						Console.WriteLine("Length: {0} (0x{1}) bytes", length.ToString(), length.ToString("X"));
					}
					break;
				default:
					{
						Console.WriteLine("Error parsing data type. Run the program without arguments for a list of usable data types.");
						Console.WriteLine("Press ENTER to exit.");
						Console.ReadLine();
						return;
					}
			}
		}
	}
}