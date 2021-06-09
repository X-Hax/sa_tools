using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SA_Tools;
using SonicRetro.SAModel;
using SonicRetro.SAModel.GC;


namespace SA_Tools.SplitDLL
{
	public static class SplitDLL
	{
		static readonly string[] charaobjectnames = new[] {
										"Sonic", "Shadow", "Tails", "Eggman", "Knuckles", "Rouge",
										"Amy", "Metal Sonic", "Tikal", "Chaos", "Chao Walker", "Dark Chao Walker",
										"Neutral Chao", "Hero Chao", "Dark Chao"
									};
		public static int SplitDLLFile(string datafilename, string inifilename, string projectFolderName, bool nometa = false, bool nolabel = false)
		{
#if !DEBUG
			try
#endif
			{
				byte[] datafile = File.ReadAllBytes(datafilename);
				IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
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
						string name = datafile.GetCString(BitConverter.ToInt32(datafile, nameaddr),
							System.Text.Encoding.ASCII);
						int addr = BitConverter.ToInt32(datafile,
							dir.AddressOfFunctions + (BitConverter.ToInt16(datafile, ordaddr) * 4));
						exports.Add(name, addr);
						nameaddr += 4;
						ordaddr += 2;
					}
				}
				ModelFormat modelfmt_def = 0;
				LandTableFormat landfmt_def = 0;
				string modelext_def = null;
				string landext_def = null;
				switch (inifile.Game)
				{
					case Game.SADX:
						modelfmt_def = ModelFormat.BasicDX;
						landfmt_def = LandTableFormat.SADX;
						modelext_def = ".sa1mdl";
						landext_def = ".sa1lvl";
						break;
					case Game.SA2B:
						modelfmt_def = ModelFormat.Chunk;
						landfmt_def = LandTableFormat.SA2;
						modelext_def = ".sa2mdl";
						landext_def = ".sa2lvl";
						break;
				}
				int itemcount = 0;
				List<string> labels = new List<string>();
				Dictionary<string, string> anilabels = new Dictionary<string, string>();
				Dictionary<string, string> anifiles = new Dictionary<string, string>();
				ModelAnimationsDictionary models = new ModelAnimationsDictionary();
				DllIniData output = new DllIniData()
				{
					Name = inifile.ModuleName,
					Game = inifile.Game
				};
				Stopwatch timer = new Stopwatch();
				timer.Start();
				foreach (KeyValuePair<string, FileInfo> item in inifile.Files)
				{
					if (string.IsNullOrEmpty(item.Key)) continue;
					FileInfo data = item.Value;
					string type = data.Type;
					string name = item.Key;
					output.Exports[name] = type;
					int address = exports[name];

					string fileOutputPath = "";
					if (data.Filename != null)
					{
						fileOutputPath = Path.Combine(projectFolderName, data.Filename);

						Console.WriteLine(name + " -> " + fileOutputPath);
					}
					else
						Console.WriteLine(name);
					switch (type)
					{
						// Landtables
						case "landtable":
						case "sa1landtable":
						case "sadxlandtable":
						case "sa2landtable":
						case "sa2blandtable":
						case "battlelandtable":
							{
								LandTableFormat landfmt_cur;
								switch (type)
								{
									case "sa1landtable":
										landfmt_cur = LandTableFormat.SA1;
										break;
									case "sadxlandtable":
										landfmt_cur = LandTableFormat.SADX;
										break;
									case "sa2landtable":
										landfmt_cur = LandTableFormat.SA2;
										break;
									case "sa2blandtable":
									case "battlelandtable":
										landfmt_cur = LandTableFormat.SA2B;
										break;
									case "landtable":
									default:
										landfmt_cur = landfmt_def;
										break;
								}
								if (data.CustomProperties.ContainsKey("format")) 
									landfmt_cur = (LandTableFormat)Enum.Parse(typeof(LandTableFormat), data.CustomProperties["format"]);
								LandTable land = new LandTable(datafile, address, imageBase, landfmt_cur) { Description = name };
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = land.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(land.Name))
								{
									if (!Directory.Exists(Path.GetDirectoryName(fileOutputPath)))
										Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
									land.SaveToFile(fileOutputPath, landfmt_cur, nometa);
									output.Files[data.Filename] = new FileTypeHash("landtable", HelperFunctions.FileHash(fileOutputPath));
									labels.AddRange(land.GetLabels());
								}
							}
							break;

						// Landtable array (SADX only)
						case "landtablearray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									LandTable land = new LandTable(datafile, ptr, imageBase, landfmt_def) { Description = idx };
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = land.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(land.Name))
									{
										string outputFN = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + landext_def);
										string fileName = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + landext_def);
										if (data.CustomProperties.ContainsKey("filename" + i.ToString()))
										{
											outputFN = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString()] + landext_def);
											fileName = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString()] + landext_def);
										}
										if (!Directory.Exists(Path.GetDirectoryName(outputFN)))
											Directory.CreateDirectory(Path.GetDirectoryName(outputFN));
										land.SaveToFile(outputFN, landfmt_def, nometa);
										output.Files[fileName] = new FileTypeHash("landtable", HelperFunctions.FileHash(outputFN));
										labels.AddRange(land.GetLabels());
									}
								}
								address += 4;
							}
							break;

						// NJS_OBJECT
						case "model":
						case "object":
						case "basicmodel":
						case "basicdxmodel":
						case "chunkmodel":
						case "gcmodel":
							{
								ModelFormat modelfmt_obj;
								switch (type)
								{
									case "basicmodel":
										modelfmt_obj = ModelFormat.Basic;
										break;
									case "basicdxmodel":
										modelfmt_obj = ModelFormat.BasicDX;
										break;
									case "chunkmodel":
										modelfmt_obj = ModelFormat.Chunk;
										break;
									case "gcmodel":
										modelfmt_obj = ModelFormat.GC;
										break;
									default:
										modelfmt_obj = modelfmt_def;
										break;
								}
								if (data.CustomProperties.ContainsKey("format"))
									modelfmt_obj = (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]);
								NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, modelfmt_obj, new Dictionary<int, Attach>());
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = mdl.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(mdl.Name))
								{
									models.Add(new ModelAnimations(data.Filename, name, mdl, modelfmt_obj));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;

						// NJS_MODEL
						case "morph":
						case "attach":
						case "basicattach":
						case "basicdxattach":
						case "chunkattach":
						case "gcattach":
							{
								Attach dummy;
								ModelFormat modelfmt_att;
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
									default:
										modelfmt_att = modelfmt_def;
										dummy = new BasicAttach(datafile, address, imageBase, true);
										break;
								}
								NJS_OBJECT mdl = new NJS_OBJECT()
								{
									Attach = dummy
								};
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = dummy.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(dummy.Name))
								{
									models.Add(new ModelAnimations(data.Filename, name, mdl, modelfmt_att));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;
						
						// NJS_OBJECT arrays
						case "modelarray":
						case "basicmodelarray":
						case "basicdxmodelarray":
						case "chunkmodelarray":
						case "gcmodelarray":
							// The code below was used for identifying models in DLLs using a source file list for SADX X360
							/*
							bool srclist_on = false; 
							/string[] srclist;
							int strid = 0;
							if (File.Exists(name + ".txt"))
							{
								srclist_on = true;
								srclist = File.ReadAllLines(name + ".txt");
							}
							else
								srclist = new string[data.Length];
							Dictionary<int, string> attachduplist = new Dictionary<int, string>();
							*/
							ModelFormat modelfmt_arr;
							string modelext_arr;
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
								default:
									modelfmt_arr = modelfmt_def;
									modelext_arr = modelext_def;
									break;
							}
							if (data.CustomProperties.ContainsKey("format"))
								modelfmt_arr = (ModelFormat)Enum.Parse(typeof(ModelFormat), data.CustomProperties["format"]);

							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									NJS_OBJECT mdl = new NJS_OBJECT(datafile, ptr, imageBase, modelfmt_arr, new Dictionary<int, Attach>());
									// The code below was used for identifying models in DLLs using a source file list for SADX X360
									/*
									bool dup = false;
									if (srclist_on)
									{
										foreach (var dupatt in attachduplist)
										{
											if (mdl.Attach != null && dupatt.Value == mdl.Attach.Name)
											{
												Console.WriteLine(";{0} is a duplicate of {1}", i, dupatt.Key);
												dup = true;
											}
										}
										if (!dup)
										{
											Console.WriteLine("filename{0}={1}", i, srclist[strid]);
											strid++;
										}
										if (mdl.Attach != null && !attachduplist.ContainsValue(mdl.Attach.Name))
											attachduplist.Add(i, mdl.Attach.Name);
									}
									*/
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = mdl.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(mdl.Name) || data.CustomProperties.ContainsKey("filename" + i.ToString()))
									{
										string fn = Path.Combine(data.Filename, i.ToString("D3", NumberFormatInfo.InvariantInfo) + modelext_arr);
										if (data.CustomProperties.ContainsKey("filename" + i.ToString()))
										{
											fn = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString()] + modelext_arr);
										}
										models.Add(new ModelAnimations(fn, idx, mdl, modelfmt_arr));
										if (!labels.Contains(mdl.Name))
											labels.AddRange(mdl.GetLabels());
									}
								}
								address += 4;
							}
							break;

						// NJS_MODEL array (SADX only)
						case "modelsarray":
						case "attacharray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									BasicAttach dummy = new BasicAttach(datafile, ptr, imageBase, modelfmt_def == ModelFormat.BasicDX);
									NJS_OBJECT mdl = new NJS_OBJECT()
									{
										Attach = dummy
									};
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = dummy.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(dummy.Name) || data.CustomProperties.ContainsKey("filename" + i.ToString()))
									{
										string fn = Path.Combine(data.Filename, i.ToString("D3", NumberFormatInfo.InvariantInfo) + modelext_def);
										if (data.CustomProperties.ContainsKey("filename" + i.ToString()))
										{
											fn = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString()] + modelext_def);
										}
										models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.BasicDX));
										if (!labels.Contains(mdl.Name))
											labels.AddRange(mdl.GetLabels());
									}
								}
								address += 4;
							}
							break;

						// NJS_ACTION array (SADX only?)
						case "actionarray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									NJS_ACTION ani = new NJS_ACTION(datafile, ptr, imageBase, modelfmt_def, new Dictionary<int, Attach>());
									string nm = item.Key + "_" + i;
									if (nolabel) 
										nm = ani.Animation.Name;
									bool saveani = false;
									if (!anilabels.ContainsKey(ani.Animation.Name))
									{
										if (!labels.Contains(ani.Animation.Name)) saveani = true;
										//else Console.WriteLine("Animation {0} already exists", ani.Animation.Name);
										anilabels.Add(ani.Animation.Name, nm);
										ani.Animation.Name = nm;
									}
									else
									{
										nm = anilabels[ani.Animation.Name];
										//Console.WriteLine("Animation {0} already exists", nm);
									}
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = nm,
										Field = "motion"
									};
									output.Items.Add(info);
									info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = ani.Model.Name,
										Field = "object"
									};
									output.Items.Add(info);
									string outputFN = Path.Combine(fileOutputPath, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim");
									string fn = Path.Combine(data.Filename, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim");
									if (anifiles.ContainsKey(nm))
										outputFN = anifiles[nm];
									if (data.CustomProperties.ContainsKey("filename" + i.ToString() + "_a"))
									{
										outputFN = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString() + "_a"] + ".saanim");
										fn = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString() + "_a"] + ".saanim");
										saveani = true;
									}
									if (saveani)
									{
										if (!Directory.Exists(Path.GetDirectoryName(outputFN)))
											Directory.CreateDirectory(Path.GetDirectoryName(outputFN));
										//Console.WriteLine("Saving animation: {0}", outputFN);
										ani.Animation.Save(outputFN, nometa);
										output.Files[fn] = new FileTypeHash("animation", HelperFunctions.FileHash(outputFN));
										if (!anifiles.ContainsKey(nm))
											anifiles.Add(nm, outputFN);
									}
									if (models.Contains(ani.Model.Name))
									{
										ModelAnimations mdl = models[ani.Model.Name];
										System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
										PathRelativePathTo(sb, Path.GetFullPath(Path.Combine(projectFolderName, mdl.Filename)), 0, Path.GetFullPath(outputFN), 0);
										mdl.Animations.Add(sb.ToString());
									}
									else
									{
										string mfn = Path.ChangeExtension(fn, modelext_def);
										if (data.CustomProperties.ContainsKey("filename" + i.ToString() + "_m"))
										{
											mfn = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString() + "_m"] + modelext_def);
										}
										string outputmfn = Path.Combine(projectFolderName, mfn);
										System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);

										PathRelativePathTo(sb, Path.GetFullPath(outputmfn), 0, Path.GetFullPath(outputFN), 0);
										string animationName = sb.ToString();
										if (!Directory.Exists(Path.GetDirectoryName(outputmfn)))
											Directory.CreateDirectory(Path.GetDirectoryName(outputmfn));
										if (!labels.Contains(ani.Model.Name) || data.CustomProperties.ContainsKey("filename" + i.ToString() + "_m"))
										{
											ModelFile.CreateFile(outputmfn, ani.Model, new[] { animationName }, null, $"{name}[{i}]->object",
												null, modelfmt_def, nometa);
											output.Files[mfn] = new FileTypeHash("model", HelperFunctions.FileHash(outputmfn));
											if (!labels.Contains(ani.Model.Name)) labels.AddRange(ani.Model.GetLabels());
										}
									}
								}
								address += 4;
							}
							break;

						// NJS_MOTION
						case "motion":
							{
								int nodeCount = int.Parse(data.CustomProperties["nodecount"]);
								NJS_MOTION ani = new NJS_MOTION(datafile, address, imageBase, nodeCount);
								string nm = item.Key;
								if (nolabel)
									nm = ani.Name;
								bool saveani = false;
								if (!anilabels.ContainsKey(ani.Name))
								{
									anilabels.Add(ani.Name, nm);
									ani.Name = nm;
									saveani = true;
								}
								else
									nm = anilabels[ani.Name];
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = nm
								};
								output.Items.Add(info);
								if (saveani)
								{
									if (!Directory.Exists(Path.GetDirectoryName(fileOutputPath)))
										Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
									ani.Save(fileOutputPath, nometa);
								}
							}
							break;

						// NJS_MOTION array
						case "motionarray":
							{
								int[] nodecounts = data.CustomProperties["nodecounts"].Split(',').Select(a => int.Parse(a)).ToArray();
								for (int i = 0; i < data.Length; i++)
								{
									int ptr = BitConverter.ToInt32(datafile, address);
									if (ptr != 0)
									{
										ptr = (int)(ptr - imageBase);
										NJS_MOTION ani = new NJS_MOTION(datafile, ptr, imageBase, nodecounts[i]);
										string nm = item.Key + "_" + i;
										if (nolabel)
											nm = ani.Name;
										bool saveani = false;
										if (!anilabels.ContainsKey(ani.Name))
										{
											anilabels.Add(ani.Name, nm);
											ani.Name = nm;
											saveani = true;
										}
										else
											nm = anilabels[ani.Name];
										DllItemInfo info = new DllItemInfo()
										{
											Export = name,
											Index = i,
											Label = nm
										};
										output.Items.Add(info);
										if (saveani)
										{
											string outputFN = Path.Combine(fileOutputPath, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim");
											string fn = Path.Combine(data.Filename, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim");
											if (data.CustomProperties.ContainsKey("filename" + i.ToString()))
											{
												outputFN = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString()] + ".saanim");
												fn = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString()] + ".saanim");
											}
											if (!Directory.Exists(Path.GetDirectoryName(outputFN)))
												Directory.CreateDirectory(Path.GetDirectoryName(outputFN));
											ani.Save(outputFN, nometa);
											output.Files[fn] = new FileTypeHash("animation", HelperFunctions.FileHash(outputFN));
										}
									}
									address += 4;
								}
							}
							break;

						// NJS_TEXLIST
						case "texlist":
							if (output.TexLists == null)
								output.TexLists = new TexListContainer();
							output.TexLists.Add((uint)(address + imageBase), new DllTexListInfo(name, null));
							if (data.Filename != null)
							{
								TexnameArray texarrs = new TexnameArray(datafile, address, imageBase);
								if (!Directory.Exists(Path.GetDirectoryName(fileOutputPath)))
									Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
								texarrs.Save(fileOutputPath + ".txt");
							}
							break;

						// NJS_TEXLIST array
						case "texlistarray":
							if (output.TexLists == null)
								output.TexLists = new TexListContainer();
							for (int i = 0; i < data.Length; i++)
							{
								uint ptr = BitConverter.ToUInt32(datafile, address);
								if (ptr != 0 && !output.TexLists.ContainsKey(ptr))
									output.TexLists.Add(ptr, new DllTexListInfo(name, i));
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
							break;

						// Other data
						case "animindexlist":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<string> hashes = new List<string>();
								int i = ByteConverter.ToInt16(datafile, address);
								while (i != -1)
								{
									new NJS_MOTION(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ByteConverter.ToInt16(datafile, address + 2))
										.Save(fileOutputPath + "/" + i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim", nometa);
									hashes.Add(i.ToString(NumberFormatInfo.InvariantInfo) + ":" + HelperFunctions.FileHash(fileOutputPath + "/" + i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim"));
									address += 8;
									i = ByteConverter.ToInt16(datafile, address);
								}
								output.DataItems.Add(new DllDataItemInfo() { Type = type, Export = name, Filename = data.Filename, MD5Hash = string.Join("|", hashes.ToArray()) });
							}
							break;
						case "charaobjectdatalist":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<CharaObjectData> result = new List<CharaObjectData>();
								List<string> hashes = new List<string>();
								for (int i = 0; i < data.Length; i++)
								{
									string chnm = charaobjectnames[i];
									CharaObjectData chara = new CharaObjectData();
									NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(BitConverter.ToInt32(datafile, address) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
									chara.MainModel = model.Name;
									NJS_MOTION anim = new NJS_MOTION(datafile, (int)(BitConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, model.CountAnimated());
									chara.Animation1 = anim.Name;
									anim.Save(Path.Combine(fileOutputPath, $"{chnm} Anim 1.saanim"), nometa);
									hashes.Add($"{chnm} Anim 1.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Anim 1.saanim")));
									anim = new NJS_MOTION(datafile, (int)(BitConverter.ToInt32(datafile, address + 8) - imageBase), imageBase, model.CountAnimated());
									chara.Animation2 = anim.Name;
									anim.Save(Path.Combine(fileOutputPath, $"{chnm} Anim 2.saanim"), nometa);
									hashes.Add($"{chnm} Anim 2.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Anim 2.saanim")));
									anim = new NJS_MOTION(datafile, (int)(BitConverter.ToInt32(datafile, address + 12) - imageBase), imageBase, model.CountAnimated());
									chara.Animation3 = anim.Name;
									anim.Save(Path.Combine(fileOutputPath, $"{chnm} Anim 3.saanim"), nometa);
									hashes.Add($"{chnm} Anim 3.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Anim 3.saanim")));
									ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{chnm}.sa2mdl"), model, new[] { $"{chnm} Anim 1.saanim", $"{chnm} Anim 2.saanim", $"{chnm} Anim 3.saanim" }, null, null, null, ModelFormat.Chunk, nometa);
									hashes.Add($"{chnm}.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm}.sa2mdl")));
									int ptr = BitConverter.ToInt32(datafile, address + 16);
									if (ptr != 0)
									{
										model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
										chara.AccessoryModel = model.Name;
										chara.AccessoryAttachNode = "object_" + (BitConverter.ToInt32(datafile, address + 20) - imageBase).ToString("X8");
										ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{chnm} Accessory.sa2mdl"), model, null, null, null, null, ModelFormat.Chunk, nometa);
										hashes.Add($"{chnm} Accessory.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{chnm} Accessory.sa2mdl")));
									}
									ptr = BitConverter.ToInt32(datafile, address + 24);
									if (ptr != 0)
									{
										model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
										chara.SuperModel = model.Name;
										anim = new NJS_MOTION(datafile, (int)(BitConverter.ToInt32(datafile, address + 28) - imageBase), imageBase, model.CountAnimated());
										chara.SuperAnimation1 = anim.Name;
										anim.Save(Path.Combine(fileOutputPath, $"Super {chnm} Anim 1.saanim"), nometa);
										hashes.Add($"Super {chnm} Anim 1.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm} Anim 1.saanim")));
										anim = new NJS_MOTION(datafile, (int)(BitConverter.ToInt32(datafile, address + 32) - imageBase), imageBase, model.CountAnimated());
										chara.SuperAnimation2 = anim.Name;
										anim.Save(Path.Combine(fileOutputPath, $"Super {chnm} Anim 2.saanim"), nometa);
										hashes.Add($"Super {chnm} Anim 2.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm} Anim 2.saanim")));
										anim = new NJS_MOTION(datafile, (int)(BitConverter.ToInt32(datafile, address + 36) - imageBase), imageBase, model.CountAnimated());
										chara.SuperAnimation3 = anim.Name;
										anim.Save(Path.Combine(fileOutputPath, $"Super {chnm} Anim 3.saanim"), nometa);
										hashes.Add($"Super {chnm} Anim 3.saanim:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm} Anim 3.saanim")));
										ModelFile.CreateFile(Path.Combine(fileOutputPath, $"Super {chnm}.sa2mdl"), model, new[] { $"Super {chnm} Anim 1.saanim", $"Super {chnm} Anim 2.saanim", $"Super {chnm} Anim 3.saanim" }, null, null, null, ModelFormat.Chunk, nometa);
										hashes.Add($"Super {chnm}.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"Super {chnm}.sa2mdl")));
									}
									chara.Unknown1 = BitConverter.ToInt32(datafile, address + 40);
									chara.Rating = BitConverter.ToInt32(datafile, address + 44);
									chara.DescriptionID = BitConverter.ToInt32(datafile, address + 48);
									chara.TextBackTexture = BitConverter.ToInt32(datafile, address + 52);
									chara.SelectionSize = BitConverter.ToSingle(datafile, address + 56);
									result.Add(chara);
									address += 60;
								}
								IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
								hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
								output.DataItems.Add(new DllDataItemInfo() { Type = type, Export = name, Filename = data.Filename, MD5Hash = string.Join("|", hashes.ToArray()) });
							}
							break;
						case "kartspecialinfolist":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<KartSpecialInfo> result = new List<KartSpecialInfo>();
								List<string> hashes = new List<string>();
								for (int i = 0; i < data.Length; i++)
								{
									KartSpecialInfo kart = new KartSpecialInfo
									{
										ID = ByteConverter.ToInt32(datafile, address)
									};
									NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(BitConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
									kart.Model = model.Name;
									ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{i}.sa2mdl"), model, null, null, null, null, ModelFormat.Chunk, nometa);
									hashes.Add($"{i}.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{i}.sa2mdl")));
									int ptr = BitConverter.ToInt32(datafile, address + 8);
									if (ptr != 0)
									{
										model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.Chunk, new Dictionary<int, Attach>());
										kart.LowModel = model.Name;
										ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{i} Low.sa2mdl"), model, null, null, null, null, ModelFormat.Chunk, nometa);
										hashes.Add($"{i} Low.sa2mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{i} Low.sa2mdl")));
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
								output.DataItems.Add(new DllDataItemInfo() { Type = type, Export = name, Filename = data.Filename, MD5Hash = string.Join("|", hashes.ToArray()) });
							}
							break;
						case "kartmodelsarray":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<KartModelsArray> result = new List<KartModelsArray>();
								List<string> hashes = new List<string>();
								for (int i = 0; i < data.Length; i++)
								{
									KartModelsArray kartobj = new KartModelsArray();
									int ptr = BitConverter.ToInt32(datafile, address);
									if (ptr != 0)
									{
										NJS_OBJECT model = new NJS_OBJECT(datafile, (int)(ptr - imageBase), imageBase, ModelFormat.GC, new Dictionary<int, Attach>());
										kartobj.Model = model.Name;
										ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{i}.sa2bmdl"), model, null, null, null, null, ModelFormat.GC, nometa);
										hashes.Add($"{i}.sa2bmdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{i}.sa2bmdl")));
										NJS_OBJECT collision = new NJS_OBJECT(datafile, (int)(BitConverter.ToInt32(datafile, address + 4) - imageBase), imageBase, ModelFormat.Basic, new Dictionary<int, Attach>());
										kartobj.Collision = collision.Name;
										ModelFile.CreateFile(Path.Combine(fileOutputPath, $"{i}.sa1mdl"), collision, null, null, null, null, ModelFormat.Basic, nometa);
										hashes.Add($"{i}.sa1mdl:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, $"{i}.sa1mdl")));
									}
									kartobj.Unknown1 = ByteConverter.ToUInt32(datafile, address + 8);
									kartobj.Unknown2 = ByteConverter.ToUInt32(datafile, address + 0xC);
									kartobj.Unknown3 = ByteConverter.ToUInt32(datafile, address + 0x10);
									kartobj.Unknown4 = ByteConverter.ToUInt32(datafile, address + 0x14);
									ptr = BitConverter.ToInt32(datafile, address + 0x64);
									if (ptr != 0)
									{
										kartobj.Unknown5 = ((uint)ptr - imageBase).ToCHex();
										kartobj.Unknown6 = ByteConverter.ToInt32(datafile, address + 0x68);
									}
									result.Add(kartobj);
									address += 0x6C;
								}
								IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
								hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
								output.DataItems.Add(new DllDataItemInfo() { Type = type, Export = name, Filename = data.Filename, MD5Hash = string.Join("|", hashes.ToArray()) });
							}
							break;
						case "motiontable":
							{
								Directory.CreateDirectory(fileOutputPath);
								List<MotionTableEntry> result = new List<MotionTableEntry>();
								List<string> hashes = new List<string>();
								int nodeCount = int.Parse(data.CustomProperties["nodecount"]);
								Dictionary<int, string> mtns = new Dictionary<int, string>();
								bool shortrot = false;
								if (data.CustomProperties.ContainsKey("shortrot"))
									shortrot = bool.Parse(data.CustomProperties["shortrot"]);
								for (int i = 0; i < data.Length; i++)
								{
									MotionTableEntry bmte = new MotionTableEntry();
									int mtnaddr = (int)(ByteConverter.ToInt32(datafile, address) - imageBase);
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
									bmte.TransitionSpeed = ByteConverter.ToUInt32(datafile, address + 12);
									bmte.StartFrame = ByteConverter.ToSingle(datafile, address + 16);
									bmte.EndFrame = ByteConverter.ToSingle(datafile, address + 20);
									bmte.PlaySpeed = ByteConverter.ToSingle(datafile, address + 24);
									result.Add(bmte);
									address += 0x1C;
								}
								IniSerializer.Serialize(result, Path.Combine(fileOutputPath, "info.ini"));
								hashes.Add("info.ini:" + HelperFunctions.FileHash(Path.Combine(fileOutputPath, "info.ini")));
								output.DataItems.Add(new DllDataItemInfo() { Type = type, Export = name, Filename = data.Filename, MD5Hash = string.Join("|", hashes.ToArray()) });
							}
							break;
						case "cactionarray":
							for (int i = 0; i < data.Length; i++)
							{
								uint ptr = BitConverter.ToUInt32(datafile, address);
								if (ptr != 0)
								{
									//NJS_CAMERA
									uint camaddr = BitConverter.ToUInt32(datafile, (int)(ptr - imageBase));
									NinjaCamera cam = new NinjaCamera(datafile, (int)(camaddr - imageBase));
									string outputFN = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".ini");
									string fileName = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".ini");
									if (data.CustomProperties.ContainsKey("filename" + i.ToString() + "_c"))
									{
										outputFN = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString() + "_c"] + ".ini");
										fileName = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString() + "_c"] + ".ini");
									}
									if (!Directory.Exists(Path.GetDirectoryName(outputFN)))
										Directory.CreateDirectory(Path.GetDirectoryName(outputFN));
									cam.Save(outputFN);
									//NJS_MOTION
									uint motptr = BitConverter.ToUInt32(datafile, (int)(ptr + 4 - imageBase));
									if (ptr != 0)
									{
										int motaddr = (int)(motptr - imageBase);
										NJS_MOTION ani = new NJS_MOTION(datafile, motaddr, imageBase, 1);
										string outputFN_m = Path.Combine(fileOutputPath, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim");
										string fn_m = Path.Combine(data.Filename, i.ToString("D3", NumberFormatInfo.InvariantInfo) + ".saanim");
										if (data.CustomProperties.ContainsKey("filename" + i.ToString() + "_m"))
										{
											outputFN_m = Path.Combine(fileOutputPath, data.CustomProperties["filename" + i.ToString() + "_m"] + ".saanim");
											fn_m = Path.Combine(data.Filename, data.CustomProperties["filename" + i.ToString() + "_m"] + ".saanim");
										}
										if (!Directory.Exists(Path.GetDirectoryName(outputFN)))
											Directory.CreateDirectory(Path.GetDirectoryName(outputFN));
										ani.Save(outputFN_m, nometa);
									}
								}
							}
							break;
					}
					itemcount++;
				}
				//Remove models that are included in other models split after them
				ModelAnimations[] arr = models.ToArray();
				List<ModelAnimations> modelanimsnew = new List<ModelAnimations>();
				foreach (ModelAnimations newitem in models)
				{
					modelanimsnew.Add(newitem);
				}
				for (int u = arr.Length - 1; u > 0; u--)
				{
					List<string> currlabels = arr[u].Model.GetLabels();
					foreach (ModelAnimations newitem in modelanimsnew)
					{
						if (newitem.Model != arr[u].Model && currlabels.Contains(newitem.Model.GetLabels().ToArray()[0]))
						{
							//Console.WriteLine("Removing item at {0}", modelanimsnew.IndexOf(newitem));
							models.Remove(newitem);
						}
					}
				}
				foreach (ModelAnimations item in models)
				{
					string modelOutputPath = Path.Combine(projectFolderName, item.Filename);
					//string modelOutputPath = item.Filename;
					if (!Directory.Exists(Path.GetDirectoryName(modelOutputPath)))
						Directory.CreateDirectory(Path.GetDirectoryName(modelOutputPath));
					ModelFile.CreateFile(modelOutputPath, item.Model, item.Animations.ToArray(), null, item.Name, null, item.Format, nometa);
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
						case ModelFormat.GC:
							type = "gcmodel";
							break;
					}
					output.Files[item.Filename] = new FileTypeHash(type, HelperFunctions.FileHash(modelOutputPath));
				}
				IniSerializer.Serialize(output, Path.Combine(projectFolderName, Path.GetFileNameWithoutExtension(datafilename))
					+ "_data.ini");
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
				return (int)SA_Tools.Split.SplitERRORVALUE.UnhandledException;
			}
#endif
			return (int)SA_Tools.Split.SplitERRORVALUE.Success;
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

		static List<string> GetLabels(this NJS_OBJECT obj)
		{
			List<string> labels = new List<string>() { obj.Name };
			if (obj.Attach != null)
				labels.AddRange(obj.Attach.GetLabels());
			if (obj.Children != null)
				foreach (NJS_OBJECT o in obj.Children)
					labels.AddRange(o.GetLabels());
			if (obj.Sibling != null)
				labels.AddRange(obj.Sibling.GetLabels());
			return labels;
		}

		static List<string> GetLabels(this Attach att)
		{
			List<string> labels = new List<string>() { att.Name };
			if (att is BasicAttach bas)
			{
				if (bas.VertexName != null)
					labels.Add(bas.VertexName);
				if (bas.NormalName != null)
					labels.Add(bas.NormalName);
				if (bas.MaterialName != null)
					labels.Add(bas.MaterialName);
				if (bas.MeshName != null)
					labels.Add(bas.MeshName);
			}
			else if (att is ChunkAttach cnk)
			{
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
		public NJS_OBJECT Model { get; private set; }
		public ModelFormat Format { get; private set; }
		public List<string> Animations { get; private set; }

		public ModelAnimations(string filename, string name, NJS_OBJECT model, ModelFormat format)
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
		public int AddressOfFunctions;	 // RVA from base of image
		public int AddressOfNames;		 // RVA from base of image
		public int AddressOfNameOrdinals;  // RVA from base of image

		/// <summary>
		/// Gets rid of warnings about fields never being written to.
		/// </summary>
		public IMAGE_EXPORT_DIRECTORY(bool dummy)
		{
			Characteristics = 0;
			TimeDateStamp = 0;
			MajorVersion = 0;
			MinorVersion = 0;
			Name = 0;
			Base = 0;
			NumberOfFunctions = 0;
			NumberOfNames = 0;
			AddressOfFunctions = 0;
			AddressOfNames = 0;
			AddressOfNameOrdinals = 0;
		}
	}
}
