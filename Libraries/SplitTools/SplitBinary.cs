using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using SplitTools;
using SAModel;
using SAModel.GC;

namespace SplitTools.Split
{
	[Flags]
	public enum SplitFlags
	{
		NoMeta = 0x1, // Disable SAModel metadata
		NoLabels = 0x2, // Disable label loading from the txt file
		Overwrite = 0x4, // Overwrite existing files
		Log = 0x8, // Enable output log
		NJA = 0x10, // Output NJA instead of SAModel
	}

	public static class SplitBinary
	{
		public static int SplitFile(string datafilename, string inifilename, string projectFolderName, SplitFlags splitFlags)
		{
			string errname = "";
#if !DEBUG
			try
#endif
			{
				// Load data file
				byte[] datafile;
				byte[] datafile_temp = File.ReadAllBytes(datafilename);
				// Load split INI
				IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
				HelperFunctions.KoreanMode = inifile.KoreanMode;
				// Load labels list
				string listfile = Path.Combine(Path.GetDirectoryName(inifilename), Path.GetFileNameWithoutExtension(datafilename) + "_labels.txt");
				Dictionary<int, string> labels = new Dictionary<int, string>();
				if (File.Exists(listfile) && !splitFlags.HasFlag(SplitFlags.NoLabels))
					labels = IniSerializer.Deserialize<Dictionary<int, string>>(listfile);
				// Trim data file if it has a start offset
				if (inifile.StartOffset != 0)
				{
					byte[] datafile_new = new byte[inifile.StartOffset + datafile_temp.Length];
					datafile_temp.CopyTo(datafile_new, inifile.StartOffset);
					datafile = datafile_new;
				}
				else datafile = datafile_temp;
				// Check the file's MD5 hash if it exists
				if (inifile.MD5 != null && inifile.MD5.Count > 0)
				{
					string datahash = HelperFunctions.FileHash(datafile);
					if (!inifile.MD5.Any(h => h.Equals(datahash, StringComparison.OrdinalIgnoreCase)))
					{
						Console.WriteLine("The file {0} is not valid for use with the INI {1}.", datafilename, inifilename);
						return (int)SplitERRORVALUE.InvalidDataMapping;
					}
				}
				// Set environment
				ByteConverter.BigEndian = inifile.BigEndian;
				ByteConverter.Reverse = inifile.Reverse;
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(datafilename));
				// Decompress PRS
				if (Path.GetExtension(datafilename).ToLowerInvariant() == ".prs" || (inifile.Compressed && Path.GetExtension(datafilename).ToLowerInvariant() != ".bin"))
					datafile = FraGag.Compression.Prs.Decompress(datafile);
				// Get binary key
				uint imageBase = HelperFunctions.SetupEXE(ref datafile) ?? inifile.ImageBase.Value;
				// Decompress REL
				if (Path.GetExtension(datafilename).Equals(".rel", StringComparison.OrdinalIgnoreCase) || (inifile.CompressedREL && Path.GetExtension(datafilename).ToLowerInvariant() == ".prs"))
				{
					datafile = HelperFunctions.DecompressREL(datafile);
					HelperFunctions.FixRELPointers(datafile, imageBase);
				}
				// Start split
				int itemcount = 0;
				Dictionary<string, MasterObjectListEntry> masterobjlist = new Dictionary<string, MasterObjectListEntry>();
				string molpath = null;
				if (inifile.MasterObjectList != null)
				{
					molpath = Path.Combine(projectFolderName, inifile.MasterObjectList);
					if (File.Exists(molpath))
						masterobjlist = IniSerializer.Deserialize<Dictionary<string, MasterObjectListEntry>>(molpath);
				}
				Stopwatch timer = new Stopwatch();
				timer.Start();
				// Loop through all items
				foreach (KeyValuePair<string, SplitTools.FileInfo> item in new List<KeyValuePair<string, SplitTools.FileInfo>>(inifile.Files))
				{
					if (string.IsNullOrEmpty(item.Key))
						continue;
					// Set up split FileInfo
					string filedesc = item.Key;
					errname = item.Key;
					SplitTools.FileInfo data = item.Value;
					Dictionary<string, string> customProperties = data.CustomProperties;
					string type = data.Type;
					int address = data.Address;
					// Print output path
					string fileOutputPath = Path.Combine(projectFolderName, data.Filename);
					Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
					Console.WriteLine("Item {0}", type);
					// Split out each item
					switch (type)
					{
						// This is the only thing that couldn't be moved out to SplitSingle because it retroactively writes to inifile
						case "masterstringlist":
							{
								if (splitFlags.HasFlag(SplitFlags.NJA))
									continue;
								Console.WriteLine(item.Key + ": " + data.Address.ToString("X") + " -> " + fileOutputPath);
								int lngcnt = 5;
								if (inifile.Game == Game.SA2B && !inifile.BigEndian)
									lngcnt = 6;
								for (int l = 0; l < lngcnt; l++)
								{
									Languages lng = (Languages)l;
									System.Text.Encoding enc = HelperFunctions.GetEncoding(inifile.Game, lng);
									string ld = Path.Combine(fileOutputPath, lng.ToString());
									Directory.CreateDirectory(ld);
									int ptr = datafile.GetPointer(address, imageBase);
									for (int i = 0; i < data.Length; i++)
									{
										int ptr2 = datafile.GetPointer(ptr, imageBase);
										if (ptr2 != 0)
										{
											string fn = Path.Combine(ld, $"{i}.txt");
											File.WriteAllText(fn, datafile.GetCString(ptr2, enc).Replace("\n", "\r\n"));
											inifile.Files.Add($"{filedesc} {lng} {i}", new FileInfo() { Type = "string", Filename = fn.Substring(projectFolderName.Length), PointerList = new int[] { ptr }, MD5Hash = HelperFunctions.FileHash(fn), CustomProperties = new Dictionary<string, string>() { { "language", lng.ToString() } } });
										}
										ptr += 4;
									}
									address += 4;
								}
								inifile.Files.Remove(filedesc);
								itemcount++;
							}
							break;
						// Single split mode for everything else
						default:
							try
							{
								itemcount += SplitSingle(item.Key, item.Value, fileOutputPath, datafile, imageBase, labels, inifile.Game, masterobjlist, splitFlags, inifile.StartOffset);
							}
							catch (Exception ex)
							{
								if (splitFlags.HasFlag(SplitFlags.Log))
								{
									TextWriter log = File.CreateText(Path.Combine(projectFolderName, "SplitLog.log"));
									log.WriteLine("Failed to split {0} in {1}.\n", errname, Path.GetFileName(inifilename));
									log.WriteLine(ex.Message);
									log.WriteLine(ex.StackTrace);
									log.Close();
								}
								return (int)SplitERRORVALUE.UnhandledException;
							}
							break;
					}
				}
				// Deal with the master object list
				if (!splitFlags.HasFlag(SplitFlags.NJA) && inifile.MasterObjectList != null)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(molpath));
					IniSerializer.Serialize(masterobjlist, molpath);
				}
				// Save _data INI file
				if (!splitFlags.HasFlag(SplitFlags.NJA))
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
				if (splitFlags.HasFlag(SplitFlags.Log))
				{
					TextWriter log = File.CreateText(Path.Combine(projectFolderName, "SplitLog.log"));
					log.WriteLine("Failed to split {0} in {1}.\n", errname, Path.GetFileName(inifilename));
					log.WriteLine(e.Message);
					log.WriteLine(e.StackTrace);
					log.Close();
				}
				return (int)SplitERRORVALUE.UnhandledException;
			}
#endif
			return (int)SplitERRORVALUE.Success;
		}

		public static int SplitSingle(string itemName, SplitTools.FileInfo data, string fileOutputPath, byte[] datafile, uint imageBase, Dictionary<int, string> labels, Game game, Dictionary<string, MasterObjectListEntry> masterobjlist, SplitFlags splitFlags, uint offset = 0)
		{
			if (string.IsNullOrEmpty(itemName))
				return 0;
			if (File.Exists(fileOutputPath) && !splitFlags.HasFlag(SplitFlags.Overwrite))
				return 0;
			string filedesc = itemName;
			Dictionary<string, string> customProperties = data.CustomProperties;
			string type = data.Type;
			int address = data.Address;
			bool nohash = false;
			ModelFormat modelfmt_def = 0;
			LandTableFormat landfmt_def = 0;
			bool SA2 = false;
			Attach dummy;
			switch (game)
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
					SA2 = true;
					break;
				case Game.SA2B:
					modelfmt_def = ModelFormat.GC;
					landfmt_def = LandTableFormat.SA2B;
					SA2 = true;
					break;
			}
			Console.WriteLine(itemName + ": " + data.Address.ToString("X") + " -> " + fileOutputPath);
			Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
			switch (type)
			{
				case "landtable":
					if (data.CustomProperties.ContainsKey("format"))
						landfmt_def = (LandTableFormat)Enum.Parse(typeof(LandTableFormat), data.CustomProperties["format"]);
					LandTable lt = new LandTable(datafile, address, imageBase, landfmt_def, labels) { Description = itemName };
					if (splitFlags.HasFlag(SplitFlags.NoMeta))
					{
						lt.TextureList = 0;
						lt.TextureFileName = "";
					}
					lt.SaveToFile(fileOutputPath, landfmt_def, splitFlags.HasFlag(SplitFlags.NoMeta));
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
						bool rev = ByteConverter.Reverse;
						if (data.CustomProperties.ContainsKey("format"))
							mdlformat = (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]);
						if (data.CustomProperties.ContainsKey("reverse"))
							ByteConverter.Reverse = true;
						NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, mdlformat, labels, new Dictionary<int, Attach>());
						List<string> mdlanis = new List<string>();
						string[] mdlanisfiles;
						string[] mdlmorphs;
						string[] mdlanisfromfolder;
						string[] mdlanisfromfolderc;
						if (customProperties.ContainsKey("animationfolder"))
						{
							mdlanisfromfolder = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(fileOutputPath), customProperties["animationfolder"]), "*.saanim", SearchOption.TopDirectoryOnly);
							if (mdlanisfromfolder.Length > 0)
							{
								for (int s = 0; s < mdlanisfromfolder.Length; s++)
									mdlanisfromfolder[s] = Path.Combine(customProperties["animationfolder"], Path.GetFileName(mdlanisfromfolder[s]));
							}
							mdlanis.AddRange(mdlanisfromfolder);
						}
						// Custom animation folder setting for SADX Chao motions
						if (customProperties.ContainsKey("animationfolderc"))
						{
							string cname1 = Path.GetDirectoryName(Directory.GetParent(fileOutputPath).ToString());
							string[] cfoldernames = customProperties["animationfolderc"].Split('|');
							for (int f = 0; f < cfoldernames.Length; f++)
							{
								mdlanisfromfolderc = Directory.GetFiles(Path.Combine(cname1, cfoldernames[f]), "*.saanim", SearchOption.TopDirectoryOnly);
								if (mdlanisfromfolderc.Length > 0)
								{
									for (int s = 0; s < mdlanisfromfolderc.Length; s++)
										mdlanisfromfolderc[s] = Path.Combine(@"..\" + cfoldernames[f], Path.GetFileName(mdlanisfromfolderc[s]));
								}
								mdlanis.AddRange(mdlanisfromfolderc);
							}
						}
						if (customProperties.ContainsKey("animations"))
						{
							mdlanisfiles = customProperties["animations"].Split(',');
							mdlanis.AddRange(mdlanisfiles);
						}
						if (customProperties.ContainsKey("morphs"))
						{
							mdlmorphs = customProperties["morphs"].Split(',');
							mdlanis.AddRange(mdlmorphs);
						}
						ModelFile.CreateFile(fileOutputPath, mdl, mdlanis.ToArray(), null, itemName, null, mdlformat, splitFlags.HasFlag(SplitFlags.NoMeta));
						if (data.CustomProperties.ContainsKey("reverse")) 
							ByteConverter.Reverse = rev;
					}
					break;
				case "morph":
				case "attach":
				case "basicattach":
				case "basicdxattach":
				case "chunkattach":
				case "gcattach":
					{
						ModelFormat modelfmt_att;
						Attach attachfmt_def;
						switch (type)
						{
							case "basicattach":
								modelfmt_att = ModelFormat.Basic;
								dummy = new BasicAttach(datafile, address, imageBase, false);
								break;
							case "basicdxattach":
								modelfmt_att = ModelFormat.BasicDX;
								dummy = new BasicAttach(datafile, address, imageBase, true);
								break;
							case "chunkattach":
								modelfmt_att = ModelFormat.Chunk;
								dummy = new ChunkAttach(datafile, address, imageBase);
								break;
							case "gcattach":
								modelfmt_att = ModelFormat.GC;
								dummy = new GCAttach(datafile, address, imageBase);
								break;
							case "morph":
							case "attach":
							default:
								modelfmt_att = modelfmt_def;
								switch (game)
								{
									case Game.SA1:
										attachfmt_def = new BasicAttach(datafile, address, imageBase, false);
										break;
									case Game.SA2:
										attachfmt_def = new ChunkAttach(datafile, address, imageBase);
										break;
									case Game.SA2B:
										attachfmt_def = new GCAttach(datafile, address, imageBase);
										break;
									case Game.SADX:
									default:
										attachfmt_def = new BasicAttach(datafile, address, imageBase, true);
										break;
								}
								dummy = attachfmt_def;
								break;
						}
						NJS_OBJECT mdl = new NJS_OBJECT()
						{
							Attach = dummy
						};
						List<string> attanis = new List<string>();
						string[] attanisfiles;
						string[] attmorphs;
						string[] attanisfromfolder;
						string[] attanisfromfolderc;
						if (customProperties.ContainsKey("animationfolder"))
						{
							attanisfromfolder = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(fileOutputPath), customProperties["animationfolder"]), "*.saanim", SearchOption.TopDirectoryOnly);
							if (attanisfromfolder.Length > 0)
							{
								for (int s = 0; s < attanisfromfolder.Length; s++)
									attanisfromfolder[s] = Path.Combine(customProperties["animationfolder"], Path.GetFileName(attanisfromfolder[s]));
							}
							attanis.AddRange(attanisfromfolder);
						}
						if (customProperties.ContainsKey("animationfolderc"))
						{
							string cname1 = Path.GetDirectoryName(Directory.GetParent(fileOutputPath).ToString());
							string[] cfoldernames = customProperties["animationfolderc"].Split('|');
							for (int f = 0; f < cfoldernames.Length; f++)
							{
								attanisfromfolderc = Directory.GetFiles(Path.Combine(cname1, cfoldernames[f]), "*.saanim", SearchOption.TopDirectoryOnly);
								if (attanisfromfolderc.Length > 0)
								{
									for (int s = 0; s < attanisfromfolderc.Length; s++)
										attanisfromfolderc[s] = Path.Combine(@"..\" + cfoldernames[f], Path.GetFileName(attanisfromfolderc[s]));
								}
								attanis.AddRange(attanisfromfolderc);
							}
						}
						if (customProperties.ContainsKey("animations"))
						{
							attanisfiles = customProperties["animations"].Split(',');
							attanis.AddRange(attanisfiles);
						}
						if (customProperties.ContainsKey("morphs"))
						{
							attmorphs = customProperties["morphs"].Split(',');
							attanis.AddRange(attmorphs);
						}
						ModelFile.CreateFile(fileOutputPath, mdl, attanis.ToArray(), null, itemName, null, modelfmt_att, splitFlags.HasFlag(SplitFlags.NoMeta));
					}
					break;
				case "modelarray":
				case "basicmodelarray":
				case "basicdxmodelarray":
				case "chunkmodelarray":
				case "gcmodelarray":
					{
						ModelFormat modelfmt_arr;
						string modelext_arr;
						string modelext_def = null;
						string path = Path.GetDirectoryName(fileOutputPath);
						switch (type)
						{
							case "basicmodelarray":
								modelfmt_arr = ModelFormat.Basic;
								modelext_arr = ".sa1mdl";
								break;
							case "basicdxmodelarray":
								modelfmt_arr = ModelFormat.BasicDX;
								modelext_arr = ".sa1mdl";
								break;
							case "chunkmodelarray":
								modelfmt_arr = ModelFormat.Chunk;
								modelext_arr = ".sa2mdl";
								break;
							case "gcmodelarray":
								modelfmt_arr = ModelFormat.GC;
								modelext_arr = ".sa2bmdl";
								break;
							case "modelarray":
							default:
								modelfmt_arr = modelfmt_def;
								modelext_arr = modelext_def;
								break;
						}
						if (data.CustomProperties.ContainsKey("format"))
							modelfmt_arr = (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]);
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = ByteConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								string file_tosave;
								if (customProperties.ContainsKey("filename" + i.ToString()))
									file_tosave = customProperties["filename" + i.ToString()];
								else
									file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + modelext_arr;
								string file = Path.Combine(path, file_tosave);
								ModelFile.CreateFile(file, new NJS_OBJECT(datafile, datafile.GetPointer(address, imageBase), imageBase, modelfmt_arr, new Dictionary<int, Attach>()), null, null, null, null, modelfmt_arr, splitFlags.HasFlag(SplitFlags.NoMeta));
							}
							address += 4;
						}
						nohash = true;
					}
					break;
				case "attacharray":
				case "basicattacharray":
				case "basicdxattacharray":
				case "chunkattacharray":
				case "gcattacharray":
					{
						Attach attachfmt_def;
						ModelFormat modelfmt_att;
						string attachext_arr;
						string attachext_def = null;
						string path = Path.GetDirectoryName(fileOutputPath);
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = ByteConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								switch (type)
								{
									case "basicattacharray":
										modelfmt_att = ModelFormat.Basic;
										dummy = new BasicAttach(datafile, ptr, imageBase, false);
										attachext_arr = ".sa1mdl";
										break;
									case "basicdxattacharray":
										modelfmt_att = ModelFormat.BasicDX;
										dummy = new BasicAttach(datafile, ptr, imageBase, true);
										attachext_arr = ".sa1mdl";
										break;
									case "chunkattacharray":
										modelfmt_att = ModelFormat.Chunk;
										dummy = new ChunkAttach(datafile, ptr, imageBase);
										attachext_arr = ".sa2mdl";
										break;
									case "gcattacharray":
										modelfmt_att = ModelFormat.GC;
										dummy = new GCAttach(datafile, ptr, imageBase);
										attachext_arr = ".sa2bmdl";
										break;
									case "attacharray":
									default:
										modelfmt_att = modelfmt_def;
										switch (game)
										{
											case Game.SA1:
												attachfmt_def = new BasicAttach(datafile, ptr, imageBase, false);
												break;
											case Game.SA2:
												attachfmt_def = new ChunkAttach(datafile, ptr, imageBase);
												break;
											case Game.SA2B:
												attachfmt_def = new GCAttach(datafile, ptr, imageBase);
												break;
											case Game.SADX:
											default:
												attachfmt_def = new BasicAttach(datafile, ptr, imageBase, true);
												break;
										}
										dummy = attachfmt_def;
										attachext_arr = attachext_def;
										break;
								}
								NJS_OBJECT mdl = new NJS_OBJECT()
								{
									Attach = dummy
								};
								string file_tosave;
								if (customProperties.ContainsKey("filename" + i.ToString()))
									file_tosave = customProperties["filename" + i.ToString()];
								else
									file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + attachext_arr;
								string file = Path.Combine(path, file_tosave);
								ModelFile.CreateFile(file, mdl, null, null, null, null, modelfmt_att, splitFlags.HasFlag(SplitFlags.NoMeta));
							}
							address += 4;
						}
						nohash = true;
					}
					break;
				case "action":
					{
						ModelFormat modelfmt_act = data.CustomProperties.ContainsKey("format") ? (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]) : modelfmt_def;
						NJS_ACTION ani = new NJS_ACTION(datafile, address, imageBase, modelfmt_act, labels, new Dictionary<int, Attach>());
						if (!labels.ContainsValue(ani.Name) && !splitFlags.HasFlag(SplitFlags.NoLabels))
							ani.Animation.Description = itemName;
						if (customProperties.ContainsKey("numparts"))
							ani.Animation.ModelParts = int.Parse(customProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
						if (ani.Animation.ModelParts == 0)
						{
							Console.WriteLine("Action {0} has no model data!", ani.Name);
							return 0;
						}
						ani.Animation.Save(fileOutputPath, splitFlags.HasFlag(SplitFlags.NoMeta));
					}
					break;
				case "animation":
				case "motion":
					int[] numverts = null;
					int numparts = 0;
					if (customProperties.ContainsKey("refmodel"))
					{
						NJS_OBJECT refmdl = new ModelFile(Path.Combine(Path.GetDirectoryName(fileOutputPath), customProperties["refmodel"])).Model;
						numverts = refmdl.GetVertexCounts();
						numparts = refmdl.CountAnimated();
					}
					else
					{
						if (customProperties.ContainsKey("numparts"))
							numparts = int.Parse(customProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
						else
							Console.WriteLine("Number of parts not specified for {0}", filedesc);
					}
					if (customProperties.ContainsKey("shortrot"))
					{
						NJS_MOTION mot = new NJS_MOTION(datafile, address, imageBase, numparts, labels, bool.Parse(customProperties["shortrot"]), numverts) { Description = itemName };
						mot.Save(fileOutputPath, splitFlags.HasFlag(SplitFlags.NoMeta));
					}
					else
					{
						NJS_MOTION mot = new NJS_MOTION(datafile, address, imageBase, numparts, labels, false, numverts) { Description = itemName };
						mot.Save(fileOutputPath, splitFlags.HasFlag(SplitFlags.NoMeta));
					}
					break;
				case "objlist":
					{
						ObjectListEntry[] objs = ObjectList.Load(datafile, address, imageBase, SA2);
						foreach (ObjectListEntry obj in objs)
						{
							if (!masterobjlist.ContainsKey(obj.CodeString))
								masterobjlist.Add(obj.CodeString, new MasterObjectListEntry(obj));
							else
								masterobjlist[obj.CodeString].AddName(obj.Name);
						}
						objs.Save(fileOutputPath);
					}
					break;
				case "startpos":
					{
						switch (game)
						{
							case Game.SA2:
								SA2DCStartPosList.Load(datafile, address).Save(fileOutputPath);
								break;
							case Game.SA2B:
								SA2StartPosList.Load(datafile, address).Save(fileOutputPath);
								break;
							case Game.SA1:
							case Game.SADX:
							default:
								SA1StartPosList.Load(datafile, address).Save(fileOutputPath);
								break;
						}
					}
					break;
				case "texturedata":
					TextureList.Load(datafile, address, imageBase).Save(fileOutputPath);
					break;
				case "texnamearray":
				case "texlist":
					NJS_TEXLIST texnames = new NJS_TEXLIST(datafile, address, imageBase, labels, offset);
					texnames.Save(fileOutputPath);
					break;
				case "texlistarray":
					{
						for (int i = 0; i < data.Length; i++)
						{
							uint ptr = ByteConverter.ToUInt32(datafile, address);
							if (data.Filename != null && ptr != 0)
							{
								ptr -= imageBase;
								NJS_TEXLIST texarr = new NJS_TEXLIST(datafile, (int)ptr, imageBase, labels, offset);
								string fn = Path.Combine(fileOutputPath, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".satex");
								if (data.CustomProperties.ContainsKey("filename" + i.ToString()))
								{
									fn = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString()] + ".satex");
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
					MusicList.Load(datafile, address, imageBase, data.Length).Save(fileOutputPath);
					break;
				case "soundlist":
					if (SA2)
						SA2SoundList.Load(datafile, address, imageBase).Save(fileOutputPath);
					else
						SoundList.Load(datafile, address, imageBase).Save(fileOutputPath);
					break;
				case "charactersoundarray":
					CharaSoundArray.Load(datafile, address, imageBase, data.Length).Save(fileOutputPath);
					break;
				case "charactervoicearray":
					CharaVoiceArray.Load(datafile, address, imageBase, data.Length).Save(fileOutputPath);
					break;
				case "minieventarray":
					if (game == Game.SA2B)
						MiniEventArray.Load(datafile, address, data.Length).Save(fileOutputPath);
					else
						DCMiniEventArray.Load(datafile, address, data.Length).Save(fileOutputPath);
					break;
				case "cutscenevoicearray":
					if (data.CustomProperties.ContainsKey("beta"))
					{
						bool beta = bool.Parse(customProperties["beta"]);
						if (beta == true)
							SA2BetaCutsceneVoices.Load(datafile, address).Save(fileOutputPath);
						else
							SA2CutsceneVoices.Load(datafile, address).Save(fileOutputPath);
					}
					else
						SA2CutsceneVoices.Load(datafile, address).Save(fileOutputPath);
					break;
				case "stringarray":
					{
						Languages lang2 = Languages.Japanese;
						if (data.CustomProperties.ContainsKey("language"))
							lang2 = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
						StringArray.Load(datafile, address, imageBase, data.Length, lang2).Save(fileOutputPath);
					}
					break;
				case "nextlevellist":
					NextLevelList.Load(datafile, address).Save(fileOutputPath);
					break;
				case "cutscenetext":
					{
						new CutsceneText(datafile, address, imageBase, data.Length).Save(fileOutputPath, out string[] hashes);
						data.MD5Hash = string.Join(",", hashes);
						nohash = true;
					}
					break;
				case "recapscreen":
					{
						if (SA2)
						{
							bool PCLang = false;
							if (game == Game.SA2B && !ByteConverter.BigEndian)
								PCLang = true;
							if (data.CustomProperties.ContainsKey("beta"))
							{
								bool beta = bool.Parse(customProperties["beta"]);
								if (beta == true)
									SA2BetaRecapScreenList.Load(datafile, address, imageBase).Save(fileOutputPath);
								else
									SA2RecapScreenList.Load(datafile, address, imageBase, PCLang).Save(fileOutputPath);
							}
							else
								SA2RecapScreenList.Load(datafile, address, imageBase, PCLang).Save(fileOutputPath);
						}
						else
						{
							RecapScreenList.Load(datafile, address, imageBase, data.Length).Save(fileOutputPath, out string[][] hashes);
							string[] hash2 = new string[hashes.Length];
							for (int i = 0; i < hashes.Length; i++)
							{
								hash2[i] = string.Join(",", hashes[i]);
							}
							data.MD5Hash = string.Join(":", hash2);
							nohash = true;
						}
					}
					break;
				case "npctext":
					{
						NPCTextList.Load(datafile, address, imageBase, data.Length).Save(fileOutputPath, out string[][] hashes);
						string[] hash2 = new string[hashes.Length];
						for (int i = 0; i < hashes.Length; i++)
							hash2[i] = string.Join(",", hashes[i]);
						data.MD5Hash = string.Join(":", hash2);
						nohash = true;
					}
					break;
				case "tikalhintmulti":
					{
						bool dpointer2 = customProperties.ContainsKey("doublepointer");
						TikalHintMultiLanguage hints = new TikalHintMultiLanguage(datafile, address, imageBase, data.Length, dpointer2);
						hints.Save(fileOutputPath, out string[] hasheds);
						data.MD5Hash = string.Join(",", hasheds);
						nohash = true;
					}
					break;
				case "tikalhintsingle":
					{
						Languages lang4 = Languages.Japanese;
						lang4 = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
						TikalHintSingleLanguage hint = new TikalHintSingleLanguage(datafile, address, imageBase, data.Length, lang4);
						hint.Save(fileOutputPath);
					}
					break;
				case "levelclearflags":
					LevelClearFlagList.Save(LevelClearFlagList.Load(datafile, address), fileOutputPath);
					break;
				case "deathzone":
					{
						switch (game)
						{
							case Game.SA2:
								{
									List<SA2DeathZoneFlags> flags = new List<SA2DeathZoneFlags>();
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
										flags.Add(new SA2DeathZoneFlags(datafile, address, file_tosave));
										ModelFile.CreateFile(file, new NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ModelFormat.Basic, labels, new Dictionary<int, Attach>()), null, null, null, null, ModelFormat.Basic, splitFlags.HasFlag(SplitFlags.NoMeta));
										hashes.Add(HelperFunctions.FileHash(file));
										address += 8;

									}
									flags.ToArray().Save(fileOutputPath);
									hashes.Insert(0, HelperFunctions.FileHash(fileOutputPath));
									data.MD5Hash = string.Join(",", hashes.ToArray());
									nohash = true;
								}
								break;
							case Game.SA2B:
								{
									List<SA2BDeathZoneFlags> flags = new List<SA2BDeathZoneFlags>();
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
										flags.Add(new SA2BDeathZoneFlags(datafile, address, file_tosave));
										ModelFile.CreateFile(file, new NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ModelFormat.Basic, labels, new Dictionary<int, Attach>()), null, null, null, null, ModelFormat.Basic, splitFlags.HasFlag(SplitFlags.NoMeta));
										hashes.Add(HelperFunctions.FileHash(file));
										address += 8;

									}
									flags.ToArray().Save(fileOutputPath);
									hashes.Insert(0, HelperFunctions.FileHash(fileOutputPath));
									data.MD5Hash = string.Join(",", hashes.ToArray());
									nohash = true;
								}
								break;
							case Game.SADX:
							case Game.SA1:
							default:
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
										ModelFormat modelfmt_death = game == Game.SADX ? ModelFormat.BasicDX : ModelFormat.Basic; // Death zones in all games except SADXPC use Basic non-DX models
										ModelFile.CreateFile(file, new NJS_OBJECT(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, modelfmt_death, labels, new Dictionary<int, Attach>()), null, null, null, null, modelfmt_death, splitFlags.HasFlag(SplitFlags.NoMeta));
										hashes.Add(HelperFunctions.FileHash(file));
										address += 8;

									}
									flags.ToArray().Save(fileOutputPath);
									hashes.Insert(0, HelperFunctions.FileHash(fileOutputPath));
									data.MD5Hash = string.Join(",", hashes.ToArray());
									nohash = true;
								}
								break;
						}
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
				case "kartranktimes":
					KartRankTimesList.Load(datafile, address, data.Length).Save(fileOutputPath);
					break;
				case "endpos":
					SA2EndPosList.Load(datafile, address).Save(fileOutputPath);
					break;
				case "animationlist":
				case "sa1actionlist":
					{
						int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						switch (game)
						{
							case Game.SADX:
							case Game.SA1:
								SA1ActionInfoList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath);
								break;
							case Game.SA2B:
							case Game.SA2:
							default:
								SA2AnimationInfoList.Load(datafile, address, cnt).Save(fileOutputPath);
								break;
						}
					}
					break;
				case "enemyanimationlist":
					{
						int cnt = int.Parse(customProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						SA2EnemyAnimInfoList.Load(datafile, address, imageBase, cnt).Save(fileOutputPath);
					}
					break;
				case "motiontable":
					{
						Directory.CreateDirectory(fileOutputPath);
						List<MotionTableEntry> result = new List<MotionTableEntry>();
						List<string> hashes = new List<string>();
						bool shortrot = false;
						string animmeta = null;
						if (customProperties.ContainsKey("shortrot"))
							shortrot = bool.Parse(customProperties["shortrot"]);
						int nodeCount = int.Parse(data.CustomProperties["nodecount"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
						Dictionary<int, string> mtns = new Dictionary<int, string>();
						string path = Path.GetDirectoryName(fileOutputPath);
						for (int i = 0; i < data.Length; i++)
						{
							MotionTableEntry bmte = new MotionTableEntry();
							int mtnaddr = (int)(ByteConverter.ToUInt32(datafile, address) - imageBase);
							if (!mtns.ContainsKey(mtnaddr))
							{
								if (customProperties.ContainsKey("meta" + i.ToString() + "_a"))
									animmeta = customProperties["meta" + i.ToString() + "_a"];
								NJS_MOTION motion = new NJS_MOTION(datafile, mtnaddr, imageBase, nodeCount, null, shortrot);
								bmte.Motion = motion.Name;
								mtns.Add(mtnaddr, motion.Name);
								string file_tosave;
								if (customProperties.ContainsKey("filename" + i.ToString()))
									file_tosave = customProperties["filename" + i.ToString()];
								else
									file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim";
								string file = Path.Combine(fileOutputPath, file_tosave);
								motion.Description = animmeta;
								motion.Save(file, splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add(HelperFunctions.FileHash(file));
							}
							else
								bmte.Motion = mtns[mtnaddr];
							bmte.LoopProperty = ByteConverter.ToUInt16(datafile, address + 4);
							bmte.Pose = ByteConverter.ToUInt16(datafile, address + 6);
							int ptr = ByteConverter.ToInt32(datafile, address + 8);
							if (ptr != -1)
							{
								bmte.NextAnimation = ByteConverter.ToInt32(datafile, address + 8);
							}
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
				case "charaobjectdatalist":
					{
						Directory.CreateDirectory(fileOutputPath);
						List<CharaObjectData> result = new List<CharaObjectData>();
						List<string> hashes = new List<string>();
						for (int i = 0; i < data.Length; i++)
						{
							string chnm = null;
							switch (i)
							{
								case 0:
									chnm = "Sonic";
									break;
								case 1:
									chnm = "Shadow";
									break;
								case 2:
									chnm = "Mech Tails";
									break;
								case 3:
									chnm = "Mech Eggman";
									break;
								case 4:
									chnm = "Knuckles";
									break;
								case 5:
									chnm = "Rouge";
									break;
								case 6:
									chnm = "Amy";
									break;
								case 7:
									chnm = "Metal Sonic";
									break;
								case 8:
									chnm = "Tikal";
									break;
								case 9:
									chnm = "Chaos";
									break;
								case 10:
									chnm = "Chao Walker";
									break;
								case 11:
									chnm = "Dark Chao Walker";
									break;
								case 12:
									chnm = "Neutral Chao";
									break;
								case 13:
									chnm = "Hero Chao";
									break;
								case 14:
									chnm = "Dark Chao";
									break;
							}
							CharaObjectData chara = new CharaObjectData();
							NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ByteConverter.ToInt32(datafile, address) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
							chara.MainModel = model.Name;
							NJS_MOTION anim = new NJS_MOTION(datafile, (int)(ByteConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, model.CountAnimated());
							chara.Animation1 = anim.Name;
							anim.Description = $"{chnm} Default Pose";
							anim.Save(Path.Combine(fileOutputPath, $"{chnm} Anim 1.saanim"), splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add($"{chnm} Anim 1.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Anim 1.saanim")));
							anim = new NJS_MOTION(datafile, (int)(ByteConverter.ToInt32(datafile, address + 8) - imageBase), imageBase, model.CountAnimated());
							chara.Animation2 = anim.Name;
							anim.Description = $"{chnm} Selected Pose (Begin)";
							anim.Save(Path.Combine(fileOutputPath, $"{chnm} Anim 2.saanim"), splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add($"{chnm} Anim 2.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Anim 2.saanim")));
							anim = new NJS_MOTION(datafile, (int)(ByteConverter.ToInt32(datafile, address + 12) - imageBase), imageBase, model.CountAnimated());
							chara.Animation3 = anim.Name;
							anim.Description = $"{chnm} Selected Pose (Loop)";
							anim.Save(Path.Combine(fileOutputPath, $"{chnm} Anim 3.saanim"), splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add($"{chnm} Anim 3.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Anim 3.saanim")));
							ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{chnm}.sa2mdl"), model, new[] { $"{chnm} Anim 1.saanim", $"{chnm} Anim 2.saanim", $"{chnm} Anim 3.saanim" }, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add($"{chnm}.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm}.sa2mdl")));
							int ptr = ByteConverter.ToInt32(datafile, address + 16);
							if (ptr != 0)
							{
								model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
								chara.AccessoryModel = model.Name;
								chara.AccessoryAttachNode = "object_" + (ByteConverter.ToInt32(datafile, address + 20) - imageBase).ToString("X8");
								ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{chnm} Accessory.sa2mdl"), model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add($"{chnm} Accessory.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Accessory.sa2mdl")));
							}
							ptr = ByteConverter.ToInt32(datafile, address + 24);
							if (ptr != 0)
							{
								model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
								chara.SuperModel = model.Name;
								anim = new NJS_MOTION(datafile, (int)(ByteConverter.ToInt32(datafile, address + 28) - imageBase), imageBase, model.CountAnimated());
								chara.SuperAnimation1 = anim.Name;
									anim.Description = $"Super {chnm} Default Pose";
								anim.Save(Path.Combine(fileOutputPath, $"Super {chnm} Anim 1.saanim"), splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add($"Super {chnm} Anim 1.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm} Anim 1.saanim")));
								anim = new NJS_MOTION(datafile, (int)(ByteConverter.ToInt32(datafile, address + 32) - imageBase), imageBase, model.CountAnimated());
								chara.SuperAnimation2 = anim.Name;
								anim.Description = $"Super {chnm} Selected Pose (Begin)";
								anim.Save(Path.Combine(fileOutputPath, $"Super {chnm} Anim 2.saanim"), splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add($"Super {chnm} Anim 2.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm} Anim 2.saanim")));
								anim = new NJS_MOTION(datafile, (int)(ByteConverter.ToInt32(datafile, address + 36) - imageBase), imageBase, model.CountAnimated());
								chara.SuperAnimation3 = anim.Name;
								anim.Description = $"Super {chnm} Selected Pose (Loop)";
								anim.Save(Path.Combine(fileOutputPath, $"Super {chnm} Anim 3.saanim"), splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add($"Super {chnm} Anim 3.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm} Anim 3.saanim")));
								ModelFile.CreateFile(Path.Combine(fileOutputPath, $"Super {chnm}.sa2mdl"), model, new[] { $"Super {chnm} Anim 1.saanim", $"Super {chnm} Anim 2.saanim", $"Super {chnm} Anim 3.saanim" }, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add($"Super {chnm}.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm}.sa2mdl")));
							}
							chara.Unknown1 = ByteConverter.ToInt32(datafile, address + 40);
							chara.Rating = ByteConverter.ToInt32(datafile, address + 44);
							chara.DescriptionID = ByteConverter.ToInt32(datafile, address + 48);
							chara.TextBackTexture = ByteConverter.ToInt32(datafile, address + 52);
							chara.SelectionSize = ByteConverter.ToSingle(datafile, address + 56);
							result.Add(chara);
							address += 60;
						}
						IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
						hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
						data.MD5Hash = string.Join("|", hashes.ToArray());
						nohash = true;
					}
					break;
				case "kartmenu":
					{
						List<KartMenuElements> result = new List<KartMenuElements>();
						List<string> hashes = new List<string>();
						string path = Path.GetDirectoryName(fileOutputPath);
						for (int i = 0; i < data.Length; i++)
						{
							KartMenuElements menu = new KartMenuElements();
							menu.CharacterID = (SA2Characters)ByteConverter.ToUInt32(datafile, address);
							menu.PortraitID = ByteConverter.ToUInt32(datafile, address + 4);
							NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ByteConverter.ToUInt32(datafile, address + 8) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
							menu.KartModel = model.Name;
							string file_tosave;
							if (customProperties.ContainsKey("filename" + i.ToString()))
								file_tosave = customProperties["filename" + i.ToString()];
							else
								file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".sa2mdl";
							string file = Path.Combine(path, file_tosave);
							ModelFile.CreateFile(file, model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add(HelperFunctions.FileHash(file));
							menu.SPD = datafile[address + 0xC];
							menu.ACL = datafile[address + 0xD];
							menu.BRK = datafile[address + 0xE];
							menu.GRP = datafile[address + 0xF];
							result.Add(menu);
							address += 0x10;
						}
						IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
						hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
						data.MD5Hash = string.Join("|", hashes.ToArray());
						nohash = true;
					}
					break;
				case "kartsoundparameters":
					{
						List<KartSoundParameters> result = new List<KartSoundParameters>();
						List<string> hashes = new List<string>();
						string path = Path.GetDirectoryName(fileOutputPath);
						for (int i = 0; i < data.Length; i++)
						{
							KartSoundParameters para = new KartSoundParameters();
							para.EngineSFXID = ByteConverter.ToUInt32(datafile, address);
							para.BrakeSFXID = ByteConverter.ToUInt32(datafile, address + 4);
							uint voice = ByteConverter.ToUInt32(datafile, address + 8);
							if (voice != 0xFFFFFFFF)
							{
								para.FinishVoice = ByteConverter.ToUInt32(datafile, address + 8);
								para.FirstVoice = ByteConverter.ToUInt32(datafile, address + 0xC);
								para.LastVoice = ByteConverter.ToUInt32(datafile, address + 0x10);
							}
							NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ByteConverter.ToUInt32(datafile, address + 0x14) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
							para.ShadowModel = model.Name;
							string file_tosave;
							if (customProperties.ContainsKey("filename" + i.ToString()))
								file_tosave = customProperties["filename" + i.ToString()];
							else
								file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".sa2mdl";
							string file = Path.Combine(path, file_tosave);
							ModelFile.CreateFile(file, model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add(HelperFunctions.FileHash(file));
							result.Add(para);
							address += 0x18;
						}
						IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
						hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
						data.MD5Hash = string.Join("|", hashes.ToArray());
						nohash = true;
					}
					break;
				case "kartspecialinfolist":
					{
						Directory.CreateDirectory(fileOutputPath);
						List<string> hashes = new List<string>();
						string path = Path.GetDirectoryName(fileOutputPath);
						switch (game)
						{
							case Game.SA2:
								{
									List<DCKartSpecialInfo> result = new List<DCKartSpecialInfo>();
									for (int i = 0; i < data.Length; i++)
									{
										DCKartSpecialInfo kart = new DCKartSpecialInfo();
										kart.ID = (SA2KartCharacters)ByteConverter.ToInt32(datafile, address);
										NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ByteConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
										kart.Model = model.Name;
										string file_tosave;
										if (customProperties.ContainsKey("filename" + i.ToString()))
											file_tosave = customProperties["filename" + i.ToString()];
										else
											file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".sa2mdl";
										string file = Path.Combine(path, file_tosave);
										ModelFile.CreateFile(file, model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
										hashes.Add(HelperFunctions.FileHash(file));
										int ptr = ByteConverter.ToInt32(datafile, address + 8);
										if (ptr != 0)
										{
											model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
											kart.LowModel = model.Name;
											string file_tosave_l;
											if (customProperties.ContainsKey("filename" + i.ToString()))
												file_tosave_l = customProperties["filename" + i.ToString()];
											else
												file_tosave_l = i.ToString("D3", NumberFormatInfo.InvariantInfo) + " Low.sa2mdl";
											string file_l = Path.Combine(path, file_tosave_l);
											ModelFile.CreateFile(file_l, model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
											hashes.Add(HelperFunctions.FileHash(file_l));
										}
										kart.TexList = ByteConverter.ToUInt32(datafile, address + 12);
										kart.Unknown1 = ByteConverter.ToInt32(datafile, address + 16);
										kart.Unknown2 = ByteConverter.ToInt32(datafile, address + 20);
										result.Add(kart);
										address += 0x18;
									}
									IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
									hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
									data.MD5Hash = string.Join("|", hashes.ToArray());
									nohash = true;
								}
								break;
							case Game.SA2B:
							default:
								{
									List<KartSpecialInfo> result = new List<KartSpecialInfo>();
									for (int i = 0; i < data.Length; i++)
									{
										KartSpecialInfo kart = new KartSpecialInfo();
										kart.ID = (SA2KartCharacters)ByteConverter.ToInt32(datafile, address);
										NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ByteConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
										kart.Model = model.Name;
										string file_tosave;
										if (customProperties.ContainsKey("filename" + i.ToString()))
											file_tosave = customProperties["filename" + i.ToString()];
										else
											file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".sa2mdl";
										string file = Path.Combine(path, file_tosave);
										ModelFile.CreateFile(file, model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
										hashes.Add(HelperFunctions.FileHash(file));
										int ptr = ByteConverter.ToInt32(datafile, address + 8);
										if (ptr != 0)
										{
											model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
											kart.LowModel = model.Name;
											string file_tosave_l;
											if (customProperties.ContainsKey("filename" + i.ToString()))
												file_tosave_l = customProperties["filename" + i.ToString()];
											else
												file_tosave_l = i.ToString("D3", NumberFormatInfo.InvariantInfo) + " Low.sa2mdl";
											string file_l = Path.Combine(path, file_tosave_l);
											ModelFile.CreateFile(file_l, model, null, null, null, null, ModelFormat.Chunk, splitFlags.HasFlag(SplitFlags.NoMeta));
											hashes.Add(HelperFunctions.FileHash(file_l));
										}
										kart.TexList = ByteConverter.ToUInt32(datafile, address + 12);
										kart.Unknown1 = ByteConverter.ToInt32(datafile, address + 16);
										kart.Unknown2 = ByteConverter.ToInt32(datafile, address + 20);
										kart.Unknown3 = ByteConverter.ToInt32(datafile, address + 24);
										result.Add(kart);
										address += 0x1C;
									}
									IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
									hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
									data.MD5Hash = string.Join("|", hashes.ToArray());
									nohash = true;
								}
								break;
						}
					}
					break;
				case "kartmodelsarray":
					{
						Directory.CreateDirectory(fileOutputPath);
						List<KartModelInfo> result = new List<KartModelInfo>();
						List<string> hashes = new List<string>();
						string path = Path.GetDirectoryName(fileOutputPath);
						for (int i = 0; i < data.Length; i++)
						{
							KartModelInfo kartobj = new KartModelInfo();
							int ptr = ByteConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								string modelext_krt;
								switch (game)
								{
									case Game.SA2:
										modelext_krt = ".sa2mdl";
										break;
									case Game.SA2B:
									default:
										modelext_krt = ".sa2bmdl";
										break;
								}
								NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, modelfmt_def, new Dictionary<int, Attach>());
								kartobj.Model = model.Name;
								string file_tosave;
								if (customProperties.ContainsKey("filename" + i.ToString()))
									file_tosave = customProperties["filename" + i.ToString()];
								else
									file_tosave = i.ToString("D3", NumberFormatInfo.InvariantInfo) + modelext_krt;
								string file = Path.Combine(path, file_tosave);
								ModelFile.CreateFile(file, model, null, null, null, null, modelfmt_def, splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add(HelperFunctions.FileHash(file));
								NJS_OBJECT collision = new NJS_OBJECT(datafile, (int)(ByteConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, ModelFormat.Basic, new Dictionary<int, Attach>());
								kartobj.Collision = collision.Name;
								string file_tosave_col;
								if (customProperties.ContainsKey("filename" + i.ToString()))
									file_tosave_col = customProperties["filename" + i.ToString()];
								else
									file_tosave_col = i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".sa1mdl";
								string file_col = Path.Combine(path, file_tosave_col);
								ModelFile.CreateFile(file_col, collision, null, null, null, null, ModelFormat.Basic, splitFlags.HasFlag(SplitFlags.NoMeta));
								hashes.Add(HelperFunctions.FileHash(file_col));
							}
							kartobj.EndPoint = new Vertex(datafile, address + 8);
							kartobj.YRotation = ByteConverter.ToUInt32(datafile, address + 0x14);
							ptr = ByteConverter.ToInt32(datafile, address + 0x64);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								int cnt = ByteConverter.ToInt32(datafile, address + 0x68);
								kartobj.StreetLights = new List<KartStreetLightPos>(cnt);
								for (int j = 0; j < cnt; j++)
								{
									kartobj.StreetLights.Add(new KartStreetLightPos() { Position = new Vertex(datafile, ptr), YRotation = ByteConverter.ToUInt32(datafile, ptr + 0xC) });
									ptr += 0x10;
								}
							}
							result.Add(kartobj);
							address += 0x6C;
						}
						IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
						hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
						data.MD5Hash = string.Join("|", hashes.ToArray());
						nohash = true;
					}
					break;
				case "kartcourse":
					KartCourse.Save(KartCourse.ReadBinary(datafile, address, imageBase), fileOutputPath);
					break;
				case "kartphysics":
					KartPhysics kpm = new KartPhysics(datafile, address);
					kpm.Save(fileOutputPath);
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
				case "pathtag":
					{
						PathData path = new PathData(datafile, address, imageBase);
						path.Save(fileOutputPath);
					}
					break;
				case "stagelightdatalist":
					switch (game)
					{
						case Game.SA1:
						case Game.SADX:
						default:
							SADXStageLightDataList.Load(datafile, address).Save(fileOutputPath);
							break;
						case Game.SA2:
						case Game.SA2B:
							SA2DefaultObjectLights.Load(datafile, address, data.Length).Save(fileOutputPath);
							break;
					}
					break;
				case "weldlist":
					WeldList.Load(datafile, address, imageBase).Save(fileOutputPath);
					break;
				case "bmitemattrlist":
					BlackMarketItemAttributesList.Load(datafile, address, imageBase).Save(fileOutputPath);
					break;
				case "creditstextlist":
					switch (game)
					{
						case Game.SA1:
						case Game.SADX:
						default:
							CreditsTextList.Load(datafile, address, imageBase).Save(fileOutputPath);
							break;
						case Game.SA2:
						case Game.SA2B:
							SA2CreditsTextList.Load(datafile, address, imageBase, data.Length).Save(fileOutputPath);
							break;
					}
					break;
				case "animindexlist":
					{
						Directory.CreateDirectory(fileOutputPath);
						List<string> hashes = new List<string>();
						int i = ByteConverter.ToInt16(datafile, address);
						string animmeta = null;
						while (i != -1)
						{
							if (customProperties.ContainsKey("meta" + i.ToString() + "_a"))
								animmeta = customProperties["meta" + i.ToString() + "_a"];
							NJS_MOTION animdata = new NJS_MOTION(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ByteConverter.ToInt16(datafile, address + 2));
							animdata.Description = animmeta;
							animdata.Save(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim", splitFlags.HasFlag(SplitFlags.NoMeta));
							hashes.Add(i.ToString(NumberFormatInfo.InvariantInfo) + ":" + HelperFunctions.FileHash(fileOutputPath + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim"));
							address += 8;
							i = ByteConverter.ToInt16(datafile, address);
						}
						data.MD5Hash = string.Join("|", hashes.ToArray());
						nohash = true;
					}
					break;
				case "storysequence":
					if (game == Game.SA2B)
						SA2StoryList.Load(datafile, address).Save(fileOutputPath);
					else
						SA2DCStoryList.Load(datafile, address).Save(fileOutputPath);
					break;
				case "chaoitemstats":
					switch (game)
					{
						case Game.SA1:
						case Game.SA2:
							DCChaoItemStats.Load(datafile, address, data.Length).Save(fileOutputPath);
							break;
						case Game.SADX:
						case Game.SA2B:
						default:
							ChaoItemStats.Load(datafile, address, data.Length).Save(fileOutputPath);
							break;
					}
					break;
				case "camera":
					NinjaCamera cam = new NinjaCamera(datafile, address);
					cam.Save(fileOutputPath);
					break;
				case "missiontutorial":
					Languages lang = Languages.Japanese;
					if (data.CustomProperties.ContainsKey("language"))
						lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
					MissionTutorialMessage missionTutorialTable = new MissionTutorialMessage(datafile, address, imageBase, lang);
					missionTutorialTable.Save(fileOutputPath);
					break;
				case "missiondescription":
					Languages lang3 = Languages.Japanese;
					if (data.CustomProperties.ContainsKey("language"))
						lang3 = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
					MissionDescriptionList missionDescriptionList = new MissionDescriptionList(datafile, address, lang3);
					missionDescriptionList.Save(fileOutputPath);
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
					LSPaletteDataList pllist = new LSPaletteDataList(datafile, address, count);
					pllist.Save(fileOutputPath);
					break;
				case "physicsdata":
					PlayerParameter plpm = new PlayerParameter(datafile, address);
					plpm.Save(fileOutputPath);
					break;
				case "singlestring":
					Languages langs = Languages.Japanese;
					int countx = data.Length > 1 ? data.Length : 1;
					langs = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
					new SingleString(datafile, address, imageBase, countx, langs).Save(fileOutputPath, out string[] hashesz);
					data.MD5Hash = string.Join(",", hashesz);
					nohash = true;
					break;
				case "multistring":
					bool dpointer = customProperties.ContainsKey("doublepointer");
					int countz = data.Length > 1 ? data.Length : 1;
					new MultilingualString(datafile, address, imageBase, countz, dpointer).Save(fileOutputPath, out string[] hashess);
					data.MD5Hash = string.Join(",", hashess);
					nohash = true;
					break;
				default: // raw binary
					{
						byte[] bin = new byte[int.Parse(customProperties["size"], NumberStyles.HexNumber)];
						Array.Copy(datafile, address, bin, 0, bin.Length);
						File.WriteAllBytes(fileOutputPath, bin);
					}
					break;
			}
			if (!nohash && !splitFlags.HasFlag(SplitFlags.NJA))
				data.MD5Hash = HelperFunctions.FileHash(fileOutputPath);
			return 1;
		}

		public static void SplitManual(string game, string dataFileName, uint imageBase, int address, string itemType, string outputFilename, string customProperties, SplitFlags splitFlags, string itemname = "", int offset = 0)
		{
			Game gameBase;
			// Get game
			switch (game.ToLowerInvariant())
			{
				case "sa1":
					gameBase = Game.SA1;
					break;
				case "sa1_b":
					gameBase = Game.SA1;
					ByteConverter.BigEndian = true;
					break;
				case "sadx_b":
					gameBase = Game.SADX;
					ByteConverter.BigEndian = true;
					break;
				case "sadx_g":
					gameBase = Game.SADX;
					ByteConverter.BigEndian = ByteConverter.Reverse = true;
					break;
				case "sa2":
					gameBase = Game.SA2;
					break;
				case "sa2_b":
					gameBase = Game.SA2;
					ByteConverter.BigEndian = true;
					break;
				case "sa2b":
					gameBase = Game.SA2B;
					break;
				case "sa2b_b":
					gameBase = Game.SA2B;
					ByteConverter.BigEndian = true;
					break;
				case "sadx":
				default:
					gameBase = Game.SADX;
					break;
			}
			// Get custom properties
			Dictionary<string, string> props = new Dictionary<string, string>();
			if (customProperties != "")
			{

				string[] customs = customProperties.Split('+');
				for (int i = 0; i < customs.Length; i++)
				{
					string[] textdata = customs[i].Split('=');
					props.Add(textdata[0], textdata[1]);
				}
			}
			FileInfo info = new FileInfo();
			info.Address = address;
			info.Filename = outputFilename;
			info.Type = itemType;
			info.CustomProperties = props;
			if (info.CustomProperties.Count > 0)
			{
				Console.Write("Custom properties:");
				foreach (var entry in info.CustomProperties)
				{
					switch (entry.Key.ToLowerInvariant())
					{
						case "length":
							info.Length = int.Parse(entry.Value);
							break;
						case "pointer":
							string[] pointers = entry.Value.Split(',');
							List<int> pts = new List<int>();
							for (int p = 0; p < pointers.Length; p++)
								pts.Add(int.Parse(pointers[p], NumberStyles.HexNumber));
							info.PointerList = pts.ToArray();
							break;
						default:
							break;
					}
					Console.Write(" {0}={1}", entry.Key, entry.Value);
				}
				Console.WriteLine();
			}
			byte[] datafile = File.ReadAllBytes(dataFileName);
			uint imageBase_new = HelperFunctions.SetupEXE(ref datafile) ?? imageBase;
			// Trim file if a start offset is specified
			if (offset != 0)
			{
				byte[] datafile_new = new byte[offset + datafile.Length];
				datafile.CopyTo(datafile_new, offset);
				datafile = datafile_new;
			}
			SplitSingle(itemname == "" ? itemType + "_" + address.ToString("X8") : itemname, info, outputFilename, datafile, imageBase_new, new Dictionary<int, string>(), gameBase, null, splitFlags, offset: (uint)offset);
		}
	}

	public enum SplitERRORVALUE
	{
		Success = 0,
		NoProject = -1,
		InvalidProject = -2,
		NoSourceFile = -3,
		NoDataMapping = -4,
		InvalidDataMapping = -5,
		UnhandledException = -6,
		InvalidConfig = -7
	}
}
