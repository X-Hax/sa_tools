using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SplitTools.SAArc
{
	public static class SA2Event
	{
		private static readonly string[] upgradeNames = ["Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claw L", "Knuckles' Shovel Claw R", "Knuckles' Hammer Glove L", "Knuckles' Hammer Glove R", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates", "Mech Tails' Window", "Mech Eggman's Window"];
		private static readonly string[] upgradeBetaNames = ["Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claws", "Knuckles' Hammer Gloves", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates"];
		private static readonly List<string> nodeNames = [];
		private static readonly Dictionary<string, ModelInfo> modelFiles = new();
		private static readonly Dictionary<string, MotionInfo> motionFiles = new();
		private static readonly Dictionary<string, CameraInfo> camArrayFiles = new();
		private static readonly Dictionary<string, TexAnimFileInfo> texAnimFiles = new();

		public static void Split(string filename, string outputPath, string labelFile = null)
		{
			nodeNames.Clear();
			modelFiles.Clear();
			camArrayFiles.Clear();
			motionFiles.Clear();
			texAnimFiles.Clear();
			
			var dir = Environment.CurrentDirectory;
			
			try
			{
				if (outputPath[^1] != '/')
				{
					outputPath = string.Concat(outputPath, "/");
				}

				// Get file name, read it from the console if nothing
				var eventFilename = filename;

				eventFilename = Path.GetFullPath(eventFilename);
				var eventDirectory = Path.GetFileNameWithoutExtension(eventFilename);
				
				if (Path.GetExtension(eventFilename).Equals(".bin", StringComparison.OrdinalIgnoreCase))
				{
					eventDirectory += "_bin";
				}

				Console.WriteLine("Splitting file {0}...", eventFilename);
				byte[] fc;
				
				if (Path.GetExtension(eventFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					fc = Prs.Decompress(eventFilename);
				}
				else
				{
					fc = File.ReadAllBytes(eventFilename);
				}

				if (Path.GetExtension(eventFilename).Equals(".bin", StringComparison.OrdinalIgnoreCase) && fc[0] == 0x0F && fc[1] == 0x81)
				{
					fc = Prs.Decompress(eventFilename);
				}

				EventIniData ini = new() { Name = Path.GetFileNameWithoutExtension(eventFilename) };
				
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
					{
						Directory.CreateDirectory(outputPath);
					}

					Environment.CurrentDirectory = outputPath;
				}
				else
				{
					Environment.CurrentDirectory = Path.GetDirectoryName(eventFilename);
				}

				Directory.CreateDirectory(eventDirectory);

				// Metadata for SAMDL Project Mode
				byte[] mlength = null;
				Dictionary<string, string> evsectionlist = new();
				Dictionary<string, string> evsplitfilenames = new();
				
				if (labelFile != null)
				{
					labelFile = Path.GetFullPath(labelFile);
				}

				if (File.Exists(labelFile))
				{
					evsplitfilenames = IniSerializer.Deserialize<Dictionary<string, string>>(labelFile);
					mlength = File.ReadAllBytes(labelFile);
				}
				
				var evname = Path.GetFileNameWithoutExtension(eventFilename);
				var evtexname = Path.Combine("EVENT", evname);
				string[] evmetadata = [];

				uint key;
				List<NJS_MOTION> motions = null;
				Dictionary<int, string> motionnames = null;
				List<NJS_CAMERA> ncams = null;
				bool battle;
				bool dcbeta;

				// These are for establishing a Motions dictionary for non-Battle files to facilitiate converting between game versions
				var m = 1;
				Dictionary<int, string> dctogcmotions = null;

				if (fc[0] == 0x81)
				{
					ByteConverter.BigEndian = true;
					ini.BigEndian = true;
					ini.Game = Game.SA2B;
					ini.DCBeta = false;
					dcbeta = false;
					if (fc[0x2B] <= 0x01 && fc[0x2A] == 0)
					{
						Console.WriteLine("File is in GC/PC format.");
						key = 0x8125FE60;
						battle = true;
						ini.BattleFormat = true;
						motions = ReadMotionFile(Path.ChangeExtension(eventFilename, null) + "motion.bin");
						ncams = ReadMotionFileCams(Path.ChangeExtension(eventFilename, null) + "motion.bin");
						motionnames = ReadMotionFileNames(Path.ChangeExtension(eventFilename, null) + "motion.bin");
						ini.Motions = motionnames;
						foreach (var mtn in motions.Where(a => a != null))
							motionFiles[mtn.Name] = new(null, mtn, mtn.Description);
						foreach (var camdata in ncams.Where(a => a != null))
							camArrayFiles[camdata.Name] = new(null, camdata);
					}
					else
					{
						Console.WriteLine("File is in GC/PC Beta format.");
						key = 0x812FFE60;
						battle = false;
						ini.BattleFormat = false;
					}
				}
				else
				{
					ByteConverter.BigEndian = false;
					ini.BigEndian = false;
					key = 0xC600000;
					ini.Game = Game.SA2;
					battle = false;
					ini.BattleFormat = false;
					var upptr = fc.GetPointer(0x20, 0xC600000);
					int betacheck = fc[upptr + 0x14C];
					int betacheck2 = fc[upptr + 0x16C];
					if (betacheck != 0 || (fc[0x27] != 0xC && betacheck == 0 && betacheck2 != 0))
					{
						Console.WriteLine("File is in DC Beta format.");
						dcbeta = true;
						ini.DCBeta = true;
					}
					else
					{
						Console.WriteLine("File is in DC format.");
						dcbeta = false;
						ini.DCBeta = false;
					}

				}
				if (!battle)
				{
					ini.Motions.Add(0, null);
				}

				var ptr = fc.GetPointer(0x20, key);
				
				if (ptr != 0)
				{
					var upcount = 0;
					
					if (!dcbeta)
					{
						for (var i = 0; i < (battle ? 18 : 16); i++)
						{
							var upnam = upgradeNames[i];
							
							var chnam = i switch
							{
								0 => "Sonic",
								4 => "Shadow",
								6 => "Knuckles",
								12 => "Rouge",
								16 => "Mech Tails",
								17 => "Mech Eggman",
								_ => upnam
							};
							
							UpgradeInfo info = new()
							{
								UpgradeName = upnam
							};
							
							if (i >= 6 && i < 12)
							{
								var knucklesName = i switch
								{
									6 or 8 => "Knuckles' Left Hand",
									7 or 9 => "Knuckles' Right Hand",
									10 or 11 => "Knuckles Root",
									_ => null
								};
								
								info.RootNode = GetModel(fc, ptr, key, $"{knucklesName}.sa2mdl", $"{evname} {knucklesName}");
							}
							else
							{
								info.RootNode = GetModel(fc, ptr, key, $"{chnam} Root.sa2mdl", $"{evname} {chnam} Root");
							}

							if (info.RootNode != null)
							{
								// Populating metadata file
								string outResultRoot = null;
								string outResultU1 = null;
								string outResultU2 = null;
								
								// Checks if the source ini is a placeholder
								if (labelFile != null && mlength.Length != 0)
								{
									evmetadata = evsplitfilenames[modelFiles[info.RootNode].Filename].Split('|'); // Description|Texture file
									outResultRoot += evmetadata[0] + "|" + evmetadata[1];
									evsectionlist[modelFiles[info.RootNode].Filename] = outResultRoot;
								}
								else
								{
									outResultRoot += modelFiles[info.RootNode].MetaName + "|" + $"{evtexname}texture";
									evsectionlist[modelFiles[info.RootNode].Filename] = outResultRoot;
								}
								
								var ptr2 = fc.GetPointer(ptr + 4, key);
								
								if (ptr2 != 0)
								{
									info.AttachNode1 = $"object_{ptr2:X8}";
								}
								else
								{
									info.AttachNode1 = "null";
								}

								var ptr3 = fc.GetPointer(ptr + 8, key);
								
								if (ptr3 != 0)
								{
									info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl", $"{evname} {upnam} A");
									Console.WriteLine($"Event contains {upnam}.");
									upcount++;

									// Populating metadata file
									// Checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelFiles[info.Model1].Filename].Split('|'); // Description|Texture file
										outResultU1 += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelFiles[info.Model1].Filename] = outResultU1;
									}
									else
									{
										outResultU2 += modelFiles[info.Model1].MetaName + "|" + $"{evtexname}texture";
										evsectionlist.Add(modelFiles[info.Model1].Filename, outResultU2);
									}
								}
								else
								{
									info.Model1 = "null";
								}

								ptr2 = fc.GetPointer(ptr + 0xC, key);
								
								if (ptr2 != 0)
								{
									info.AttachNode2 = $"object_{ptr2:X8}";
								}
								else
								{
									info.AttachNode2 = "null";
								}

								ptr3 = fc.GetPointer(ptr + 0x10, key);
								
								if (ptr3 != 0)
								{
									info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl", $"{evname} {upnam} B");
									// Populating metadata file
									string outResult = null;
									
									// Checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelFiles[info.Model2].Filename].Split('|'); // Description|Texture file
										outResult += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelFiles[info.Model2].Filename] = outResult;
									}
									else
									{
										outResult += modelFiles[info.Model2].MetaName + "|" + $"{evtexname}texture";
										evsectionlist.Add(modelFiles[info.Model2].Filename, outResult);
									}
								}
								else
								{
									info.Model2 = "null";
								}
							}
							else
							{
								info.RootNode = "null";
								info.AttachNode1 = "null";
								info.Model1 = "null";
								info.AttachNode2 = "null";
								info.Model2 = "null";
							}
							ini.Upgrades.Add(info);
							ptr += 0x14;
						}
						
						// Padding in the event the user wants to convert a DC cutscene to Battle format
						if (!battle)
						{
							UpgradeInfo pad = new();
							pad.RootNode = "null";
							pad.AttachNode1 = "null";
							pad.Model1 = "null";
							pad.AttachNode2 = "null";
							pad.Model2 = "null";
							ini.Upgrades.Add(pad);
							ini.Upgrades.Add(pad);
						}
						
						ini.UpgradeCount = upcount;
						
						if (upcount == 0)
						{
							Console.WriteLine("Event contains no character upgrades.");
						}
					}
					else
					{
						for (var i = 0; i < 14; i++)
						{
							var upnam = upgradeBetaNames[i];
							
							var chnam = i switch
							{
								0 => "Sonic",
								4 => "Shadow",
								6 => "Knuckles",
								10 => "Rouge",
								_ => upnam
							};
							
							UpgradeInfo info = new();
							info.UpgradeName = upnam;
							info.RootNode = GetModel(fc, ptr, key, $"{chnam} Root.sa2mdl", $"{evname} {chnam} Root");
							if (info.RootNode != null)
							{
								// Populating metadata file
								string outResultRoot = null;
								
								// Checks if the source ini is a placeholder
								if (labelFile != null && mlength.Length != 0)
								{
									evmetadata = evsplitfilenames[modelFiles[info.RootNode].Filename].Split('|'); // Description|Texture file
									outResultRoot += evmetadata[0] + "|" + evmetadata[1];
									evsectionlist[modelFiles[info.RootNode].Filename] = outResultRoot;
								}
								else
								{
									outResultRoot += modelFiles[info.RootNode].MetaName + "|" + evtexname;
									evsectionlist[modelFiles[info.RootNode].Filename] = outResultRoot;
								}
								
								var ptr2 = fc.GetPointer(ptr + 4, key);
								
								if (ptr2 != 0)
								{
									info.AttachNode1 = $"object_{ptr2:X8}";
								}
								else
								{
									info.AttachNode1 = "null";
								}

								string outResultU1 = null;
								string outResultU2 = null;
								
								var ptr3 = fc.GetPointer(ptr + 8, key);
								
								if (ptr3 != 0)
								{
									info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl", $"{evname} {upnam} A");
									Console.WriteLine($"Event contains {upnam}.");
									upcount++;
									// populating metadata file
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelFiles[info.Model1].Filename].Split('|'); // Description|Texture file
										outResultU1 += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelFiles[info.Model1].Filename] = outResultU1;
									}
									else
									{
										outResultU1 = modelFiles[info.Model1].MetaName + "|" + evtexname;
										evsectionlist.Add(modelFiles[info.Model1].Filename, outResultU1);
									}
								}
								else
								{
									info.Model1 = "null";
								}

								ptr2 = fc.GetPointer(ptr + 0xC, key);
								
								if (ptr2 != 0)
								{
									info.AttachNode2 = $"object_{ptr2:X8}";
								}
								else
								{
									info.AttachNode2 = "null";
								}

								ptr3 = fc.GetPointer(ptr + 0x10, key);
								if (ptr3 != 0)
								{
									info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl", $"{evname} {upnam} B");
									// populating metadata file
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelFiles[info.Model2].Filename].Split('|'); // Description|Texture file
										outResultU2 += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelFiles[info.Model2].Filename] = outResultU2;
									}
									else
									{
										outResultU2 = modelFiles[info.Model2].MetaName + "|" + evtexname;
										evsectionlist.Add(modelFiles[info.Model2].Filename, outResultU2);
									}
								}
								else
								{
									info.Model2 = "null";
								}
							}
							else
							{
								info.RootNode = "null";
								info.AttachNode1 = "null";
								info.Model1 = "null";
								info.AttachNode2 = "null";
								info.Model2 = "null";
							}
							ini.Upgrades.Add(info);
							ptr += 0x14;
						}
						// Padding in the event the user wants to convert a DC Beta cutscene to other formats
						UpgradeInfo pad = new();
						pad.RootNode = "null";
						pad.AttachNode1 = "null";
						pad.Model1 = "null";
						pad.AttachNode2 = "null";
						pad.Model2 = "null";
						ini.Upgrades.Add(pad);
						ini.Upgrades.Add(pad);
						ini.Upgrades.Add(pad);
						ini.Upgrades.Add(pad);
						ini.UpgradeCount = upcount;
						
						if (upcount == 0)
						{
							Console.WriteLine("Event contains no character upgrades.");
						}
					}
				}
				
				ptr = fc.GetPointer(0x18, key);
				
				if (ptr != 0)
				{
					var namecount = 0;
					string name = null;
					
					for (var i = 0; i < 93; i++)
					{
						var ptr2 = fc.GetPointer(ptr, key);
						
						if (ptr2 != 0)
						{
							name = $"object_{ptr2:X8}";
							ini.UpgradeOverrideParts.Add(i, name);
							namecount++;
						}
						else
						{
							ini.UpgradeOverrideParts.Add(i, null);
						}

						ptr += 4;
					}
					ini.OverrideCount = namecount;
					Console.WriteLine("Event contains {0} upgrade override system pointe{1}.", namecount == 0 ? "no" : $"{namecount}", namecount == 1 ? "r" : "rs");
				}
				
				var gcnt = ByteConverter.ToInt32(fc, 8);
				ptr = fc.GetPointer(0, key);
				
				if (ptr != 0)
				{
					Console.WriteLine("Event contains {0} scen{1}.", gcnt + 1, gcnt + 1 == 1 ? "e" : "es");
					var scnnum = 0;
					
					for (var gn = 0; gn <= gcnt; gn++)
					{
						Directory.CreateDirectory(Path.Combine(eventDirectory, $"Scene {gn}"));
						SceneInfo scn = new();
						var ptr2 = fc.GetPointer(ptr, key);
						var ecnt = ByteConverter.ToInt32(fc, ptr + 4);
						
						scn.SceneNumber = scnnum;
						
						if (ptr2 != 0)
						{
							var entnum = 1;
							Console.WriteLine("Scene {0} contains {1} entit{2}.", gn, ecnt, ecnt == 1 ? "y" : "ies");
							
							for (var en = 0; en < ecnt; en++)
							{
								EntityInfo ent = new()
								{
									EntityNumber = entnum,
									Model = GetModel(fc, ptr2, key, $"Scene {gn}\\Entity {en + 1} Model.sa2mdl", $"{evname} Scene {gn} Entity {en + 1}")
								};
								
								string EntityName = null;

								if (ent.Model != null)
								{
									// Checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelFiles[ent.Model].Filename].Split('|');
										EntityName = evmetadata[0];
									}
									else
									{
										EntityName = modelFiles[ent.Model].MetaName;
									}

									ent.ModelFile = modelFiles[ent.Model].Filename;
									ent.Motion = GetMotion(fc, ptr2 + 4, key, $"Scene {gn}\\Entity {en + 1} Motion.saanim", motions, modelFiles[ent.Model].Model.CountAnimated(), $"{EntityName} Scene {gn} Animation");
									if (ent.Motion != null)
									{
										// Add metadata to animations found in motion.bin files
										if (battle)
										{
											motionFiles[ent.Motion].Description = $"{EntityName} Scene {gn} Animation";
										}

										if (modelFiles[ent.Model].Filename.EndsWith("Root.sa2mdl") || modelFiles[ent.Model].Filename.EndsWith("Hand.sa2mdl"))
										{
											modelFiles[ent.Model].Motions.Add(motionFiles[ent.Motion].Filename);
										}
										else
										{
											modelFiles[ent.Model].Motions.Add("../" + motionFiles[ent.Motion].Filename);
										}

										if (!battle)
										{
											ini.Motions.Add(m, ent.Motion);
											m++;
										}

									}
									else
									{
										ent.Motion = "null";
									}

									ent.ShapeMotion = GetMotion(fc, ptr2 + 8, key, $"Scene {gn}\\Entity {en + 1} Shape Motion.saanim", motions, modelFiles[ent.Model].Model.CountMorph(), $"{EntityName} Scene {gn} Shape Motion");
									if (ent.ShapeMotion != null)
									{
										// Add metadata to animations found in motion.bin files
										if (battle)
										{
											motionFiles[ent.ShapeMotion].Description = $"{EntityName} Scene {gn} Shape Motion";
										}

										if (modelFiles[ent.Model].Filename.EndsWith("Root.sa2mdl") || modelFiles[ent.Model].Filename.EndsWith("Hand.sa2mdl"))
										{
											modelFiles[ent.Model].Motions.Add(motionFiles[ent.ShapeMotion].Filename);
										}
										else
										{
											modelFiles[ent.Model].Motions.Add("../" + motionFiles[ent.ShapeMotion].Filename);
										}

										if (!battle)
										{
											ini.Motions.Add(m, ent.ShapeMotion);
											m++;
										}
									}
									else
									{
										ent.ShapeMotion = "null";
									}

									// Populating metadata file
									string outResult = null;
									// Checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelFiles[ent.Model].Filename].Split('|'); // Description|Texture file
										outResult += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelFiles[ent.Model].Filename] = outResult;
									}
									else
									{
										if (!dcbeta)
										{
											outResult += modelFiles[ent.Model].MetaName + "|" + $"{evtexname}texture";
										}
										else
										{
											outResult += modelFiles[ent.Model].MetaName + "|" + evtexname;
										}

										evsectionlist[modelFiles[ent.Model].Filename] = outResult;
									}
								}
								else
								{
									ent.Model = "null";
									ent.Motion = "null";
									ent.ShapeMotion = "null";
								}
								
								if (battle)
								{
									ent.GCModel = GetGCModel(fc, ptr2 + 12, key, $"Scene {gn}\\Entity {en + 1} GC Model.sa2bmdl", $"{evname} Scene {gn} Entity {en + 1} GC Model");
									ent.ShadowModel = GetModel(fc, ptr2 + 16, key, $"Scene {gn}\\Entity {en + 1} Shadow Model.sa2mdl", $"{EntityName} Shadow Model");
									ent.Position = new(fc, ptr2 + 24);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 36);
									ent.Layer = ByteConverter.ToUInt32(fc, ptr2 + 40);
									
									// Populating metadata file
									if (ent.GCModel != null)
									{
										string outResultGC = null;
										
										// Checks if the source ini is a placeholder
										if (labelFile != null && mlength.Length != 0)
										{
											evmetadata = evsplitfilenames[modelFiles[ent.GCModel].Filename].Split('|'); // Description|Texture file
											outResultGC += evmetadata[0] + "|" + evmetadata[1];
											evsectionlist[modelFiles[ent.GCModel].Filename] = outResultGC;
										}
										else
										{
											outResultGC = modelFiles[ent.GCModel].MetaName + "|" + $"{evtexname}texture";
											evsectionlist[modelFiles[ent.GCModel].Filename] = outResultGC;
										}
										ent.GCModelFile = modelFiles[ent.GCModel].Filename;
									}
									else
									{
										ent.GCModel = "null";
									}

									if (ent.ShadowModel != null)
									{
										string outResultShadow = null;
										
										// Checks if the source ini is a placeholder
										if (labelFile != null && mlength.Length != 0)
										{
											evmetadata = evsplitfilenames[modelFiles[ent.ShadowModel].Filename].Split('|'); // Description|Texture file
											outResultShadow += evmetadata[0];
											evsectionlist[modelFiles[ent.ShadowModel].Filename] = outResultShadow;
										}
										else
										{
											outResultShadow = modelFiles[ent.ShadowModel].MetaName;
											evsectionlist[modelFiles[ent.ShadowModel].Filename] = outResultShadow;
										}
										ent.ShadowModelFile = modelFiles[ent.ShadowModel].Filename;
									}
									else
									{
										ent.ShadowModel = "null";
									}
								}
								else
								{
									ent.GCModel = "null";
									ent.ShadowModel = "null";
									ent.Position = new(fc, ptr2 + 16);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 28);
								}
								scn.Entities.Add(ent);
								ptr2 += battle ? 0x2C : 0x20;
								entnum++;
							}
						}
						else
						{
							Console.WriteLine("Scene {0} contains no entities.", gn);
						}

						ptr2 = fc.GetPointer(ptr + 8, key);
						if (ptr2 != 0)
						{
							var camaddr = ByteConverter.ToInt32(fc, ptr2 + 0xC);
							var cnt = ByteConverter.ToInt32(fc, ptr + 12);
							
							for (var i = 0; i < cnt; i++)
							{
								scn.CameraMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn}\\Camera Motion {i + 1}.saanim", motions, 1, $"{evname} Scene {gn} Camera Animation {i + 1}"));
								if (battle)
								{
									// add metadata to animations found in motion.bin files
									motionFiles[scn.CameraMotions[i]].Description = $"{evname} Scene {gn} Camera Animation {i + 1}";
									scn.NinjaCameras.Add(GetGCCamData(fc, ptr2, key, $"Scene {gn}\\Camera Motion {i + 1} Attributes.ini", ncams));
								}
								else
								{
									scn.NinjaCameras.Add(GetCamData(fc, ptr2, key, $"Scene {gn}\\Camera Motion {i + 1} Attributes.ini", ncams));
									ini.Motions.Add(m, scn.CameraMotions[i]);
									m++;
								}
								ptr2 += sizeof(int);
							}
						}
						
						ptr2 = fc.GetPointer(ptr + 0x10, key);
						
						if (ptr2 != 0)
						{
							var cnt = ByteConverter.ToInt32(fc, ptr + 0x14);
							Console.WriteLine("Scene {0} contains {1} particle motio{2}.", gn, cnt == 0 ? "no" : cnt, cnt == 1 ? "n" : "ns");
							for (var i = 0; i < cnt; i++)
							{
								scn.ParticleMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn}\\Particle Motion {i + 1}.saanim", motions, 1, $"{evname} Scene {gn} Particle Motion {i + 1}"));
								// add metadata to animations found in motion.bin files
								if (battle)
								{
									motionFiles[scn.ParticleMotions[i]].Description = $"{evname} Scene {gn} Particle Motion {i + 1}";
								}
								else
								{
									ini.Motions.Add(m, scn.ParticleMotions[i]);
									m++;
								}
								ptr2 += sizeof(int);
							}
						}
						else
						{
							Console.WriteLine("Scene {0} contains no particle motions.", gn);
						}

						ptr2 = fc.GetPointer(ptr + 0x18, key);
						
						if (ptr2 != 0)
						{
							BigInfo big = new();
							big.Model = GetModel(fc, ptr2, key, $"Scene {gn}\\Big Model.sa2mdl", $"{evname} Scene {gn} Big Model");
							if (big.Model != null)
							{
								Console.WriteLine("Scene {0} contains Big the Cat cameo.", gn);
								var anicnt = modelFiles[big.Model].Model.CountAnimated();
								var ptr3 = fc.GetPointer(ptr2 + 4, key);
								if (ptr3 != 0)
								{
									var cnt = ByteConverter.ToInt32(fc, ptr2 + 8);
									for (var i = 0; i < cnt; i++)
									{
										big.Motions.Add([GetMotion(fc, ptr3, key, $"Scene {gn}/Big Motion {i + 1}a.saanim", motions, anicnt, $"{evname} Scene {gn} Big Motion {i + 1}A"), GetMotion(fc, ptr3 + 4, key, $"Scene {gn}\\Big Motion {i + 1}b.saanim", motions, anicnt, $"{evname} Scene {gn} Big Motion {i + 1}B")
										]);
										// add metadata to animations found in motion.bin files
										if (battle)
										{
											var ptr4 = fc.GetPointer(ptr3 + 4, key);
											motionFiles[big.Motions[i][0]].Description = $"{evname} Scene {gn} Big Motion {i + 1}A";
											modelFiles[big.Model].Motions.Add("../" + motionFiles[big.Motions[i][0]].Filename);
											if (ptr4 != 0)
											{
												motionFiles[big.Motions[i][1]].Description = $"{evname} Scene {gn} Big Motion {i + 1}B";
												modelFiles[big.Model].Motions.Add("../" + motionFiles[big.Motions[i][1]].Filename);
											}

										}
										else
										{
											ini.Motions.Add(m, big.Motions[i][0]);
											modelFiles[big.Model].Motions.Add("../" + motionFiles[big.Motions[i][0]].Filename);
											m++;
											var ptr4 = fc.GetPointer(ptr3 + 4, key);
											if (ptr4 != 0)
											{
												ini.Motions.Add(m, big.Motions[i][1]);
												modelFiles[big.Model].Motions.Add("../" + motionFiles[big.Motions[i][1]].Filename);
												m++;
											}
										}
										ptr3 += 8;
									}
								}
								
								// Populating metadata file
								string outResultBig = null;
								
								// Checks if the source ini is a placeholder
								if (labelFile != null && mlength.Length != 0)
								{
									evmetadata = evsplitfilenames[modelFiles[big.Model].Filename].Split('|'); // Description|Texture file
									outResultBig += evmetadata[0] + "|" + evmetadata[1];
									evsectionlist[modelFiles[big.Model].Filename] = outResultBig;
								}
								else
								{
									outResultBig = modelFiles[big.Model].MetaName + "|" + $"{evtexname}texture";
									evsectionlist.Add(modelFiles[big.Model].Filename, outResultBig);
								}
							}
							else
							{
								Console.WriteLine("Scene {0} does not contain Big the Cat cameo.", gn);
								big.Model = "null";
								big.Motions.Add(null);
							}
							
							big.Unknown = ByteConverter.ToInt32(fc, ptr2 + 12);
							scn.Big = big;
						}
						
						scn.FrameCount = ByteConverter.ToInt32(fc, ptr + 28);
						ini.Scenes.Add(scn);
						ptr += 0x20;
						scnnum++;
					}
				}
				else
				{
					Console.WriteLine("Event contains no scenes.");
				}

				ptr = fc.GetPointer(0x1C, key);
				if (ptr != 0)
				{
					ini.TailsTails = GetModel(fc, ptr, key, $"Tails' tails.sa2mdl", $"{evname} Tails' Tails");
					var ptr2 = fc.GetPointer(ptr, key);
					
					if (ptr2 == 0)
					{
						Console.WriteLine("Event does not contain Tails' tails.");
						ini.TailsTails = "null";
					}
					else
					{
						Console.WriteLine("Event contains Tails' tails.");
					}
				}
				
				ptr = fc.GetPointer(4, key);
				
				if (ptr != 0)
				{
					var ptr2 = fc.GetPointer(ptr, key);
					
					if (ptr2 != 0)
					{
						Console.WriteLine("Event contains an internal texture list.");
						NJS_TEXLIST tls = new(fc, ptr, key);
						ini.Texlist = GetTexlist(fc, 4, key, "InternalTexlist.satex");
						var fp = Path.Combine(eventDirectory, "InternalTexlist.satex");
						tls.Save(fp);
						ini.Files.Add("InternalTexlist.satex", HelperFunctions.FileHash(fp));
					}
					else
					{
						Console.WriteLine("Event does not contain an internal texture list.");
					}
				}
				
				ptr = fc.GetPointer(0xC, key);
				
				if (ptr != 0)
				{
					var start = ByteConverter.ToUInt16(fc, ptr);
					if (start > 0)
					{
						Console.WriteLine("Event contains internal texture sizes.");
						TexSizes tsz = new(fc, ptr);
						ini.TexSizes = GetTexSizes(fc, 0xC, key, "TextureSizes.ini");
						var fp = Path.Combine(eventDirectory, "TextureSizes.ini");
						tsz.Save(fp);
						ini.Files.Add("TextureSizes.ini", HelperFunctions.FileHash(fp));
					}
					else
					{
						Console.WriteLine("Event does not contain internal texture sizes.");
					}
				}
				
				ptr = fc.GetPointer(0x10, key);
				
				if (ptr != 0)
				{
					var start = ByteConverter.ToUInt32(fc, ptr);
					if (start > 0)
					{
						ReflectionInfo refl = new(fc, ptr, key);
						ini.ReflectionInfo = GetReflectData(fc, 0x10, key, "ReflectionData.ini");
						var fp = Path.Combine(eventDirectory, "ReflectionData.ini");
						refl.Save(fp);
						ini.Files.Add("ReflectionData.ini", HelperFunctions.FileHash(fp));
						Console.WriteLine("Event contains character reflection info.");
					}
					else
					{
						Console.WriteLine("Event does not contain character reflection info.");
						ini.ReflectionInfo = "null";
					}
				}
				
				ptr = fc.GetPointer(0x14, key);
				
				if (ptr != 0)
				{
					var namecount = 0;
					string name = null;
					
					for (var i = 0; i < 64; i++)
					{
						var ptr2 = fc.GetPointer(ptr, key);
						
						if (ptr2 != 0)
						{
							name = $"object_{ptr2:X8}";
							ini.BlurModels.Add(i, name);
							namecount++;
						}
						else
						{
							ini.BlurModels.Add(i, null);
						}

						ptr += 4;
					}
					ini.BlurCount = namecount;
					Console.WriteLine("Event contains {0} blur model pointe{1}.", namecount == 0 ? "no" : $"{namecount}", namecount == 1 ? "r" : "rs");
				}
				
				ptr = fc.GetPointer(0x24, key);
				
				if (ptr == 0 || dcbeta && ((fc[37] == 0x25) || (fc[38] == 0x22)))
				{
					Console.WriteLine("Event does not contain texture animation data.");
					ini.TextureAnimations = null;
					ini.TexAnimCount = 0;
					ini.UsedTexAnims = 0;
				}
				else
				{
					Directory.CreateDirectory(Path.Combine(eventDirectory, "Texture Animations"));
					TexAnimInfo tanim = new();
					var ptr2 = fc.GetPointer(ptr, key);
					if (battle)
					{
						var tn = ByteConverter.ToInt32(fc, ptr + 8);
						tanim.TexAnimGCIDCount = tn;
						var tanimcount = 0;
						var d = 1;
						while (ByteConverter.ToInt32(fc, ptr2) != 0)
						{
							TexAnimMain main = new();
							var mptr = fc.GetPointer(ptr2, key);
							var taptr = fc.GetPointer(ptr2 + 8, key);
							var tanims = ByteConverter.ToInt32(fc, ptr2 + 4);
							if (mptr != 0)
							{
								tanimcount++;
								main.Model = $"object_{mptr:X8}";
								main.ModelFile = modelFiles[main.Model].Filename;
								main.TexAnimDataEntries = ByteConverter.ToInt32(fc, ptr2 + 4);
								for (var t = 0; t < tanims; t++)
								{
									main.TexAnimPointers.Add(GetTexanim(fc, taptr, key, $"Texture Animations\\TexanimInfo {d} Part {t + 1}.ini"));
									taptr += 0x4;
								}
								d++;
							}
							else
							{
								break;
							}

							tanim.MainData.Add(main);
							ptr2 += 0xC;
						}
						var ptr3 = fc.GetPointer(ptr + 4, key);
						for (var i = 0; i < tn; i++)
						{
							TexAnimGCIDs tgcids = new();
							tgcids.TexID = ByteConverter.ToInt32(fc, ptr3);
							tgcids.TexLoopNumber = ByteConverter.ToInt32(fc, ptr3 + 4);
							tanim.TexAnimGCIDs.Add(tgcids);
							ptr3 += 0x8;
						}
						// This is for Battle -> DC conversions
						// Dreamcast only has one set of texture looping values
						tanim.TexID = tanim.TexAnimGCIDs[0].TexID;
						tanim.TexLoopNumber = tanim.TexAnimGCIDs[0].TexLoopNumber;

						tanim.TexAnimMainDataEntries = ByteConverter.ToInt32(fc, ptr + 8);
						ini.TextureAnimations = tanim;
						if (tn != tanimcount)
						{
							Console.WriteLine("Event contains {0} mode{1} with texture animation data. {2} data se{3} {4} used.", $"{tanimcount}", tanimcount == 1 ? "l" : "ls", tn == 1 ? "Only the first" : $"The first {tn}", tn == 1 ? "t" : "ts", tn == 1 ? "is" : "are");
						}
						else
						{
							Console.WriteLine("Event contains {0} mode{1} with texture animation data.", $"{tanimcount}", tanimcount == 1 ? "l" : "ls");
						}

						ini.UsedTexAnims = tn;
						ini.TexAnimCount = tanimcount;
					}
					else
					{
						var tanimcount = 0;
						var d = 1;
						while (ByteConverter.ToInt32(fc, ptr2) != 0)
						{
							TexAnimMain main = new();
							var mptr = fc.GetPointer(ptr2, key);
							var taptr = fc.GetPointer(ptr2 + 8, key);
							var tanims = ByteConverter.ToInt32(fc, ptr2 + 4);
							
							if (mptr != 0)
							{
								tanimcount++;
								main.Model = $"object_{mptr:X8}";
								main.ModelFile = modelFiles[main.Model].Filename;
								main.TexAnimDataEntries = ByteConverter.ToInt32(fc, ptr2 + 4);
								
								for (var t = 0; t < tanims; t++)
								{
									main.TexAnimPointers.Add(GetTexanim(fc, taptr, key, $"Texture Animations\\TexanimInfo {d} Part {t + 1}.ini"));
									taptr += 0x4;
								}
								d++;
							}
							else
							{
								break;
							}

							tanim.MainData.Add(main);
							ptr2 += 0xC;
						}
						
						// This is for DC -> Battle conversions
						// Dreamcast only has one set of texture looping values
						tanim.TexAnimGCIDCount = 1;
						TexAnimGCIDs tgcids = new();
						tgcids.TexID = ByteConverter.ToInt32(fc, ptr + 4);
						tgcids.TexLoopNumber = ByteConverter.ToInt32(fc, ptr + 8);
						tanim.TexAnimGCIDs.Add(tgcids);

						tanim.TexID = ByteConverter.ToInt32(fc, ptr + 4);
						tanim.TexLoopNumber = ByteConverter.ToInt32(fc, ptr + 8);
						tanim.TexAnimMainDataEntries = 1;
						ini.TextureAnimations = tanim;
						
						if (tanimcount != 1)
						{
							Console.WriteLine("Event contains {0} models with texture animation data. Only the first data set is used.", $"{tanimcount}");
						}
						else
						{
							Console.WriteLine("Event contains 1 model with texture animation data.");
						}

						ini.TexAnimCount = tanimcount;
						ini.UsedTexAnims = 1;
					}

				}
				
				var shmap = ByteConverter.ToInt32(fc, 0x28);
				
				if (battle && shmap == 0)
				{
					Console.WriteLine("Event does not use GC shadow maps.");
					ini.GCShadowMaps = false;
				}
				if (battle && shmap == 1)
				{
					Console.WriteLine("Event uses GC shadow maps.");
					ini.GCShadowMaps = true;
				}
				
				foreach (var item in motionFiles.Values)
				{
					var fn = item.Filename ?? $"Unknown Motion {motions.IndexOf(item.Motion)}.saanim";
					var fp = Path.Combine(eventDirectory, fn);
					// applies metadata to animations found in motion.bin files
					if (battle)
					{ 
						var meta = item.Description ?? $"Unassigned Motion {motions.IndexOf(item.Motion)}";
						item.Motion.Description = meta;
					}
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				
				foreach (var item in camArrayFiles.Values)
				{
					var fn = item.Filename;
					var fp = Path.Combine(eventDirectory, fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				
				foreach (var item in texAnimFiles.Values)
				{
					var fn = item.Filename;
					var fp = Path.Combine(eventDirectory, fn);
					item.TextureAnimation.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				
				foreach (var item in modelFiles.Values)
				{
					var fp = Path.Combine(eventDirectory, item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				
				// Creates metadata ini file for SAMDL Project Mode
				if (labelFile != null)
				{
					var evsectionListFilename = Path.GetFileNameWithoutExtension(labelFile) + "_data.ini";
					IniSerializer.Serialize(evsectionlist, Path.Combine(outputPath, evsectionListFilename));
				}
				JsonSerializer js = new()
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
				using var tw = File.CreateText(Path.Combine(eventDirectory, Path.ChangeExtension(Path.GetFileName(eventFilename), ".json")));
				js.Serialize(tw, ini);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void SplitExternalTexList(string filename, string outputPath)
		{
			var dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[^1] != '/')
				{
					outputPath = string.Concat(outputPath, "/");
				}

				// Get file name, read it from the console if nothing
				var evfilename = filename;

				evfilename = Path.GetFullPath(evfilename);

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					fc = Prs.Decompress(evfilename);
				}
				else
				{
					fc = File.ReadAllBytes(evfilename);
				}

				EventTexListData satx = new() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
					{
						Directory.CreateDirectory(outputPath);
					}

					Environment.CurrentDirectory = outputPath;
				}
				else
				{
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				}

				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				uint key;
				if (fc[3] == 0xC && fc[2] == 0xBC)
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					key = 0xCBC0000;
					satx.Game = Game.SA2;
					satx.BigEndian = false;
				}
				else
				{
					if (fc[0] == 0)
					{
						Console.WriteLine("File is in GC/PC format.");
						ByteConverter.BigEndian = true;
						key = 0;
						satx.Game = Game.SA2B;
						satx.BigEndian = true;
					}
					else
					{
						Console.WriteLine("File is in GC/PC Beta format.");
						ByteConverter.BigEndian = true;
						key = 0x818BFE60;
						satx.Game = Game.SA2B;
						satx.BigEndian = true;
					}
				}

				var ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					NJS_TEXLIST tls = new(fc, ptr, key);
					satx.TexList = GetTexlist(fc, 0, key, "ExternalTexlist.satex");
					var fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "ExternalTexlist.satex");
					tls.Save(fp);
					satx.Files.Add("ExternalTexlist.satex", HelperFunctions.FileHash(fp));
				}
				JsonSerializer js = new()
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
				using (var tw = File.CreateText(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), Path.ChangeExtension(Path.GetFileName(evfilename), ".json"))))
					js.Serialize(tw, satx);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}
		
		public static void Build(string filename, string format, string fileOutputPath)
		{
			nodeNames.Clear();
			modelFiles.Clear();
			motionFiles.Clear();
			var dir = Environment.CurrentDirectory;
			try
			{
				if (fileOutputPath[fileOutputPath.Length - 1] != '/')
				{
					fileOutputPath = string.Concat(fileOutputPath, "/");
				}

				var path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new();
				EventIniData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new(tr))
					evinfo = js.Deserialize<EventIniData>(jtr);

				uint gamekey;
				var battle = evinfo.BattleFormat;
				var dcbeta = evinfo.DCBeta;
				ByteConverter.BigEndian = evinfo.BigEndian;
				if (evinfo.Game == Game.SA2 || ((!evinfo.BattleFormat) && (!evinfo.BigEndian)))
				{
					gamekey = 0xC600000;
				}
				else if ((!evinfo.BattleFormat) && evinfo.BigEndian)
				{
					gamekey = 0x812FFE60;
				}
				else
				{
					gamekey = 0x8125FE60;
				}
				// This code will only write event data that's compatible with DC final and SA2B final
				// Beta compatibility is not implemented

				if (format != "default")
				{
					switch (format)
					{
						case "dc":
							battle = false;
							dcbeta = false;
							ByteConverter.BigEndian = false;
							gamekey = 0xC600000;
							break;
						case "dcbeta":
							battle = false;
							dcbeta = true;
							ByteConverter.BigEndian = false;
							gamekey = 0xC600000;
							break;
						case "battle":
							battle = true;
							dcbeta = false;
							ByteConverter.BigEndian = true;
							gamekey = 0x8125FE60;
							break;
						case "battlebeta":
							battle = false;
							dcbeta = false;
							ByteConverter.BigEndian = true;
							gamekey = 0x812FFE60;
							break;
						default:
							battle = evinfo.BattleFormat;
							dcbeta = evinfo.DCBeta;
							ByteConverter.BigEndian = evinfo.BigEndian;
							if (evinfo.Game == Game.SA2 || ((!evinfo.BattleFormat) && (!evinfo.BigEndian)))
							{
								gamekey = 0xC600000;
							}
							else if ((!evinfo.BattleFormat) && evinfo.BigEndian)
							{
								gamekey = 0x812FFE60;
							}
							else
							{
								gamekey = 0x8125FE60;
							}

							break;
					}
				}
				else
				{
					battle = evinfo.BattleFormat;
					dcbeta = evinfo.DCBeta;
					ByteConverter.BigEndian = evinfo.BigEndian;
					if (evinfo.Game == Game.SA2 || ((!evinfo.BattleFormat) && (!evinfo.BigEndian)))
					{
						gamekey = 0xC600000;
					}
					else if ((!evinfo.BattleFormat) && evinfo.BigEndian)
					{
						gamekey = 0x812FFE60;
					}
					else
					{
						gamekey = 0x8125FE60;
					}
				}
				var imageBase = gamekey;

				List<byte> evfile = [];
				List<byte> databytes = [];
				List<byte> exdatabytes = [];
				Dictionary<string, int> animaddrs = new();
				Dictionary<int, int> banimaddrs = new();
				Dictionary<int, uint> mdladdrs = new();
				Dictionary<int, int> panimaddrs = new();
				Dictionary<int, int> shapeaddrs = new();
				List<byte> modelbytes = [];
				List<byte> motionbytes = [];
				List<byte> ncameradata = [];
				Dictionary<string, uint> labels = new();
				Dictionary<string, int> motionIDs = new();

				if (battle)
				{
					// Done for accuracy to the original files. Whether or not this is necessary is unclear.
					if (evinfo.GCShadowMaps == true)
					{
						imageBase += 0x40;
					}
					else
					{
						imageBase += 0x2C;
					}

					// Reverses the motion dictionary so the motion IDs can be written to main files
					foreach (var mnames in evinfo.Motions.Where(a => a.Key != 0))
					{
						if (mnames.Value == null)
						{
							motionIDs.Add("null", mnames.Key);
						}
						else
						{
							motionIDs.Add(mnames.Value, mnames.Key);
						}
					}
				}
				else
				{
					imageBase += 0x28;
				}

				// Event models/animations. File acquisition code copied from original method, with some alterations.
				foreach (var file in evinfo.Files.Where(a => a.Key.EndsWith(".sa2mdl", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
				{
					if (battle)
					{
						if (file.EndsWith("Shadow Model.sa2mdl"))
						{
							modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels,
								[], out var shadowend));
						}
						else
						{
							modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels,
								[], out var modelend));
						}
					}
					else
					{
						if (!file.EndsWith("Shadow Model.sa2mdl"))
						{
							modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels,
								[], out var dcmodelend));
						}
					}
				}
				if (battle)
				{
					foreach (var file in evinfo.Files.Where(a => a.Key.EndsWith(".sa2bmdl", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels,
							[], out var gcmodelend));
					}
					// Motion file building
					var motionfilename = filename;
					List<byte> motiondatabytes = [];
					List<byte> motionfilebytes = [];
					List<byte> bincameradata = [];
					Dictionary<string, int> binanimaddrs = new();
					Dictionary<string, int> bincamaddrs = new();
					Dictionary<string, int> partcounts = new(evinfo.Motions.Count);
					var motionImageBase = (uint)(evinfo.Motions.Count + 1) * 8;
					foreach (var file in evinfo.Files.Where(a => a.Key.EndsWith("Attributes.ini", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						var camfile = NJS_CAMERA.Load(Path.Combine(path, file));
						List<byte> ncambytes = [];
						var ndata = camfile.NinjaCameraData;
						var ncamaddr = (int)motionImageBase;
						bincamaddrs[camfile.Name] = ncamaddr;
						ncambytes.AddRange(ndata.GetBytes());
						bincameradata.AddRange(ncambytes);
						motionImageBase += (uint)ncambytes.Count;
					}
					foreach (var file in evinfo.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						var motion = NJS_MOTION.Load(Path.Combine(path, file));
						motion.OptimizeShape();
						motiondatabytes.AddRange(motion.GetBytes(motionImageBase, labels, out var animend));
						var animaddr = (int)(animend + motionImageBase);
						binanimaddrs[motion.Name] = animaddr;
						if (file.Contains("Camera"))
						{
							var camfile = NJS_CAMERA.Load(Path.Combine(path, Path.ChangeExtension(Path.ChangeExtension(file, null) + " Attributes", ".ini")));
							// Sets up NJS_CAMERA pointers
							motiondatabytes.AddRange(ByteConverter.GetBytes(bincamaddrs[camfile.Name]));
							motiondatabytes.AddRange(ByteConverter.GetBytes(binanimaddrs[motion.Name]));
						}
						partcounts.Add(motion.Name, motion.ModelParts);
						if (file.Contains("Camera"))
						{
							motionImageBase += animend + 0x14;
						}
						else
						{
							motionImageBase += animend + 0xC;
						}
					}
					for (var i = 0; i < evinfo.Motions.Count; i++)
					{
						if (evinfo.Motions[i] == null)
						{
							motionfilebytes.AddRange([0xFF, 0xFF, 0xFF, 0xFF, 0, 0, 0, 0]);
						}
						else
						{
							motionfilebytes.AddRange(ByteConverter.GetBytes(binanimaddrs[evinfo.Motions[i]]));
							motionfilebytes.AddRange(ByteConverter.GetBytes(partcounts[evinfo.Motions[i]]));
						}
					}
					motionfilebytes.AddRange(new byte[8]);
					motionfilebytes.AddRange(bincameradata);
					motionfilebytes.AddRange(motiondatabytes);
					if (fileOutputPath.Length != 0)
					{
						if (!Directory.Exists(fileOutputPath))
						{
							Directory.CreateDirectory(fileOutputPath);
						}

						motionfilename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
					}
					File.WriteAllBytes(Path.ChangeExtension(Path.ChangeExtension(motionfilename, null) + "motion", ".bin"), motionfilebytes.ToArray());
				}
				else
				{
					var dcImageBase = imageBase;
					dcImageBase += (uint)modelbytes.Count;
					Dictionary<string, int> camaddrs = new();
					foreach (var file in evinfo.Files.Where(a => a.Key.EndsWith("Attributes.ini", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						var camfile = NJS_CAMERA.Load(Path.Combine(path, file));
						List<byte> ncambytes = [];
						var ndata = camfile.NinjaCameraData;
						var ncamaddr = (int)dcImageBase;
						camaddrs[camfile.Name] = ncamaddr;
						ncambytes.AddRange(ndata.GetBytes());
						ncameradata.AddRange(ncambytes);
						dcImageBase += (uint)ncambytes.Count;
					}
					foreach (var file in evinfo.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						var motion = NJS_MOTION.Load(Path.Combine(path, file));
						motion.OptimizeShape();
						motionbytes.AddRange(motion.GetBytes(dcImageBase, labels, out var animend));
						var animaddr = (int)(animend + dcImageBase);
						animaddrs[motion.Name] = animaddr;
						if (file.Contains("Camera"))
						{
							var camfile = NJS_CAMERA.Load(Path.Combine(path, Path.ChangeExtension(Path.ChangeExtension(file, null) + " Attributes", ".ini")));
							// Sets up NJS_CAMERA pointers
							motionbytes.AddRange(ByteConverter.GetBytes(camaddrs[camfile.Name]));
							motionbytes.AddRange(ByteConverter.GetBytes(animaddrs[motion.Name]));
						}
						if (file.Contains("Camera"))
						{
							dcImageBase += animend + 0x14;
						}
						else
						{
							dcImageBase += animend + 0xC;
						}
					}
					imageBase += (uint)motionbytes.Count;
					imageBase += (uint)ncameradata.Count;
				}
				databytes.AddRange(modelbytes);
				databytes.AddRange(ncameradata);
				databytes.AddRange(motionbytes);
				imageBase += (uint)modelbytes.Count;

				// Texlist information (Necessary for debug mode, always exists)
				var tlsstart = imageBase;
				var tex = NJS_TEXLIST.Load(Path.Combine(path, "InternalTexlist.satex"));
				List<byte> namebytes = [];
				Dictionary<int, int> texaddrs = new();
				for (var x = 0; x < tex.NumTextures; x++)
				{
					var names = tex.TextureNames[x];
					var texname = names.PadRight(28);
					namebytes.AddRange(Encoding.ASCII.GetBytes(texname));
					namebytes.AddRange(new byte[4]);
					var texaddr = 0x20 * x;
					texaddrs[x] = texaddr;
				}
				var texlistaddr = (int)(imageBase + (tex.NumTextures * 0xC));
				imageBase += (uint)((tex.NumTextures * 0xC) + 8);

				// Texture dimensions (Necessary for debug mode, always exists)
				var sizeptr = (int)imageBase;
				if (evinfo.TexSizes != null)
				{
					var size = TexSizes.Load(Path.Combine(path, "TextureSizes.ini"));
					List<byte> sizebytes = [];
					for (var s = 0; s < size.TextureCount; s++)
					{
						var texsize = size.TextureSize[s];
						sizebytes.AddRange(texsize.GetBytes());
					}
					exdatabytes.AddRange(sizebytes);
					imageBase += (uint)sizebytes.Count;
				}

				// Camera/Particle motion pointer array goes here
				Dictionary<int, int> camarrayptrs = new();
				Dictionary<int, int> particlearrayptrs = new();
				List<byte> camarraybytes = [];
				List<byte> particlearraybytes = [];
				var cammasterptr = (int)imageBase;
				// Scene 0 does not usually contain camera/particle animation data
				for (var i = 1; i < evinfo.Scenes.Count; i++)
				{
					var scn = evinfo.Scenes[i];
					// Cameras should always exist... right?
					for (var c = 0; c < scn.CameraMotions.Count; c++)
					{
						if (battle)
						{
							camarraybytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.CameraMotions[c]]));
						}
						else
						{
							if (labels.ContainsKeySafe(scn.CameraMotions[c]))
							{
								camarraybytes.AddRange(ByteConverter.GetBytes(labels[scn.CameraMotions[c]]));
							}
						}
					}
					camarrayptrs[i] = cammasterptr;
					cammasterptr += 4 * scn.CameraMotions.Count;
				}
				exdatabytes.AddRange(camarraybytes);
				imageBase += (uint)camarraybytes.Count;
				// Scene 0 does not usually contain particle animation data
				var particlemasterptr = (int)imageBase;
				for (var i = 1; i < evinfo.Scenes.Count; i++)
				{
					var scn = evinfo.Scenes[i];
					if (scn.ParticleMotions.Count != 0)
					{
						for (var p = 0; p < scn.ParticleMotions.Count; p++)
						{
							if (battle)
							{
								particlearraybytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.ParticleMotions[p]]));
							}
							else
							{
								if (labels.ContainsKeySafe(scn.ParticleMotions[p]))
								{
									particlearraybytes.AddRange(ByteConverter.GetBytes(labels[scn.ParticleMotions[p]]));
								}
							}
						}
					}
					else
					{
						particlearraybytes.AddRange(new byte[4]);
					}

					particlearrayptrs[i] = particlemasterptr;
					if (scn.ParticleMotions.Count != 0)
					{
						particlemasterptr += 4 * scn.ParticleMotions.Count;
					}
					else
					{
						particlemasterptr += 4;
					}
				}
				exdatabytes.AddRange(particlearraybytes);
				imageBase += (uint)particlearraybytes.Count;

				// Reflection array
				var reflectptr = (int)imageBase;
				List<byte> evmirrref = [];
				Dictionary<int, int> refaddrs = new();
				if (evinfo.ReflectionInfo != "null")
				{
					var refl = ReflectionInfo.Load(Path.Combine(path, "ReflectionData.ini"));
					List<byte> refbytes = [];
					for (var i = 0; i < refl.Instances; i++)
					{
						var refaddr = 0x30 * i;
						refbytes.AddRange(refl.ReflectData[i].GetBytes());
						refaddrs[i] = refaddr;
					}
					// The game adds the matrix data first, followed by the usage information.
					// Valid reflect data pointers point here.
					reflectptr += (int)refbytes.Count;
					evmirrref.AddRange(refbytes);
					evmirrref.AddRange(ByteConverter.GetBytes(refl.Instances));
					for (var t = 0; t < refl.Instances; t++)
					{
						evmirrref.AddRange(ByteConverter.GetBytes(refl.ReflectData[t].Opacity));
					}
					if (refl.Instances < 32)
					{
						var padding = (32 - (int)refl.Instances) * 4;
						evmirrref.AddRange(new byte[padding]);
					}
					for (var n = 0; n < refl.Instances; n++)
					{
						var matrixaddrs = (int)imageBase + refaddrs[n];
						evmirrref.AddRange(ByteConverter.GetBytes(matrixaddrs));
					}
				}
				else
				{
					evmirrref.AddRange(new byte[0x88]);
				}

				exdatabytes.AddRange(evmirrref);
				imageBase += (uint)evmirrref.Count;

				// Blur model array
				var blurptr = (int)imageBase;
				List<byte> evblurref = [];
				if (evinfo.BlurCount != 0)
				{
					for (var b = 0; b < 64; b++)
					{
						if (evinfo.BlurModels[b] != null && labels.ContainsKeySafe(evinfo.BlurModels[b]))
						{
							evblurref.AddRange(ByteConverter.GetBytes(labels[evinfo.BlurModels[b]]));
						}
						else
						{
							evblurref.AddRange(new byte[4]);
						}
					}
				}
				else
				{
					evblurref.AddRange(new byte[0x190]);
				}

				exdatabytes.AddRange(evblurref);
				imageBase += (uint)evblurref.Count;

				// Upgrade override array (AKA "Mech Parts")
				var overrideptr = (int)imageBase;
				List<byte> evovrref = [];
				if (evinfo.OverrideCount != 0)
				{
					for (var v = 0; v < 93; v++)
					{
						if (labels.ContainsKeySafe(evinfo.UpgradeOverrideParts[v]))
						{
							evovrref.AddRange(ByteConverter.GetBytes(labels[evinfo.UpgradeOverrideParts[v]]));
						}
						else
						{
							evovrref.AddRange(new byte[4]);
						}
					}
				}
				else
				{
					evovrref.AddRange(new byte[0x24C]);
				}

				exdatabytes.AddRange(evovrref);
				imageBase += (uint)evovrref.Count;

				// Tails' tails
				var tailsptr = (int)imageBase;
				List<byte> evtailref = [];
				if (evinfo.TailsTails != "null" && labels.ContainsKeySafe(evinfo.TailsTails))
				{
					evtailref.AddRange(ByteConverter.GetBytes(labels[evinfo.TailsTails]));
				}
				else
				{
					evtailref.AddRange(new byte[4]);
				}

				exdatabytes.AddRange(evtailref);
				imageBase += (uint)evtailref.Count;

				// Upgrade model array
				var upgradeptr = (int)imageBase;
				List<byte> evupref = [];
				if (evinfo.UpgradeCount != 0)
				{
					var upmodelnum = 0;
					if (battle)
					{
						upmodelnum = 18;
					}
					else if (dcbeta)
					{
						upmodelnum = 14;
					}
					else
					{
						upmodelnum = 16;
					}

					if (dcbeta)
					{
						if (evinfo.Upgrades[6].UpgradeName != "Knuckles' Shovel Claws")
						{
							var sclawL = evinfo.Upgrades[6];
							var sclawR = evinfo.Upgrades[7];
							var hgloveL = evinfo.Upgrades[8];
							var hgloveR = evinfo.Upgrades[9];
							evinfo.Upgrades[6].AttachNode2 = sclawR.AttachNode1;
							evinfo.Upgrades[6].Model2 = sclawR.Model1;
							evinfo.Upgrades[7].AttachNode1 = hgloveL.AttachNode1;
							evinfo.Upgrades[7].Model1 = hgloveL.Model1;
							evinfo.Upgrades[7].AttachNode2 = hgloveR.AttachNode1;
							evinfo.Upgrades[7].Model2 = hgloveR.Model1;
							evinfo.Upgrades[8] = evinfo.Upgrades[10];
							evinfo.Upgrades[9] = evinfo.Upgrades[11];
							evinfo.Upgrades[10] = evinfo.Upgrades[12];
							evinfo.Upgrades[11] = evinfo.Upgrades[13];
							evinfo.Upgrades[12] = evinfo.Upgrades[14];
							evinfo.Upgrades[13] = evinfo.Upgrades[15];
						}
					}
					else
					{
						if (evinfo.Upgrades[6].UpgradeName != "Knuckles' Shovel Claw L")
						{
							var sclaw = evinfo.Upgrades[6];
							var hglove = evinfo.Upgrades[7];
							var sun = evinfo.Upgrades[8];
							var air = evinfo.Upgrades[9];
							var nail = evinfo.Upgrades[10];
							var scope = evinfo.Upgrades[11];
							var boot = evinfo.Upgrades[12];
							var plate = evinfo.Upgrades[13];

							UpgradeInfo sclawL = new();
							UpgradeInfo sclawR = new();
							UpgradeInfo hgloveL = new();
							UpgradeInfo hgloveR = new();

							sclawL.RootNode = sclaw.RootNode;
							sclawL.AttachNode1 = sclaw.AttachNode1;
							sclawL.Model1 = sclaw.Model1;
							sclawL.AttachNode2 = null;
							sclawL.Model2 = null;

							sclawR.RootNode = sclaw.RootNode;
							sclawR.AttachNode1 = sclaw.AttachNode2;
							sclawR.Model1 = sclaw.Model2;
							sclawR.AttachNode2 = null;
							sclawR.Model2 = null;

							hgloveL.RootNode = hglove.RootNode;
							hgloveL.AttachNode1 = hglove.AttachNode1;
							hgloveL.Model1 = hglove.Model1;
							hgloveL.AttachNode2 = null;
							hgloveL.Model2 = null;

							hgloveR.RootNode = hglove.RootNode;
							hgloveR.AttachNode1 = hglove.AttachNode2;
							hgloveR.Model1 = hglove.Model2;
							hgloveR.AttachNode2 = null;
							hgloveR.Model2 = null;

							evinfo.Upgrades[6] = sclawL;
							evinfo.Upgrades[7] = sclawR;
							evinfo.Upgrades[8] = hgloveL;
							evinfo.Upgrades[9] = hgloveR;
							evinfo.Upgrades[10] = sun;
							evinfo.Upgrades[11] = air;
							evinfo.Upgrades[12] = nail;
							evinfo.Upgrades[13] = scope;
							evinfo.Upgrades[14] = boot;
							evinfo.Upgrades[15] = plate;
						}
					}
					
					for (var u = 0; u < upmodelnum; u++)
					{
						var upgrades = evinfo.Upgrades[u];
						if (upgrades.RootNode != "null")
						{
							if (labels.ContainsKeySafe(upgrades.RootNode))
							{
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.RootNode]));
							}

							if (upgrades.AttachNode1 != "null" && labels.ContainsKeySafe(upgrades.AttachNode1))
							{
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.AttachNode1]));
							}
							else
							{
								evupref.AddRange(new byte[4]);
							}

							if (upgrades.Model1 != "null" && labels.ContainsKeySafe(upgrades.Model1))
							{
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.Model1]));
							}
							else
							{
								evupref.AddRange(new byte[4]);
							}

							if (upgrades.AttachNode2 != "null" && labels.ContainsKeySafe(upgrades.AttachNode2))
							{
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.AttachNode2]));
							}
							else
							{
								evupref.AddRange(new byte[4]);
							}

							if (upgrades.Model2 != "null" && labels.ContainsKeySafe(upgrades.Model2))
							{
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.Model2]));
							}
							else
							{
								evupref.AddRange(new byte[4]);
							}
						}
						else
						{
							evupref.AddRange(new byte[0x14]);
						}
					}
				}
				else
				{
					if (battle)
					{
						evupref.AddRange(new byte[0x1E0]);
					}
					else
					{
						evupref.AddRange(new byte[0x1B8]);
					}
				}
				exdatabytes.AddRange(evupref);
				imageBase += (uint)evupref.Count;

				// Texture animation data
				var texanimptr = (int)imageBase;
				Dictionary<int, int> uvdatastartaddrs = new();
				Dictionary<int, int> uvdataaddrs = new();
				Dictionary<int, int> uvdataarrayaddrs = new();
				List<byte> evtexanimref = [];
				if (evinfo.UsedTexAnims != 0)
				{
					List<byte> uvdatabytes = [];
					var texmaster = evinfo.TextureAnimations;
					for (var q = 0; q < evinfo.TexAnimCount; q++)
					{
						var maindata = texmaster.MainData[q];
						for (var j = 0; j < maindata.TexAnimDataEntries; j++)
						{
							var materialsetup = 0;
							var materialaddr = 0;
							List<int> polysize = [];
							Dictionary<int, int> polyadd = new();
							var uvdata = EV_TEXANIM.Load(Path.Combine(path, $"Texture Animations\\TexanimInfo {q + 1} Part {j + 1}.ini"));
							// UV data start calculation
							var uvstart = (int)imageBase;

							for (var v = 0; v < uvdata.UVEditEntries; v++)
							{
								var uvaddr = 0;
								var uvaddrminus = 0;
								int uvsize;
								// Material/UV addresses. How the hell are we getting these?
								// Answer: Hacks!
								if (int.TryParse(uvdata.UVEditData[v].UVAddress.Substring(3), System.Globalization.NumberStyles.HexNumber, null, out var uv))
								{
									uvaddr = uv;
								}

								if (v != 0)
								{
									if (int.TryParse(uvdata.UVEditData[v - 1].UVAddress.Substring(3), System.Globalization.NumberStyles.HexNumber, null, out var uvminus))
									{
										uvaddrminus = uvminus;
									}
								}
								if (v != 0)
								{
									uvsize = uvaddr - uvaddrminus;
								}
								else
								{
									uvsize = 0;
								}

								polysize.Add(uvsize);
								if (v == 0)
								{
									polyadd.Add(v, 0);
								}
								else
								{
									polyadd.Add(v, polysize.Sum());
								}
							}
							var firstuv = 0;
							var mat = 0;
							if (int.TryParse(uvdata.UVEditData[0].UVAddress.Substring(3), System.Globalization.NumberStyles.HexNumber, null, out var uvstaddr))
							{
								firstuv = uvstaddr;
							}

							if (int.TryParse(uvdata.MaterialTexAddress.Substring(7), System.Globalization.NumberStyles.HexNumber, null, out var maddr))
							{
								mat = maddr;
							}

							var polysizeminus = polysize.Sum() + 6;
							if (labels.ContainsKeySafe(texmaster.MainData[q].Model))
							{
								var attachptr = labels[texmaster.MainData[q].Model];
								materialsetup = (int)attachptr - 0x1C - polysizeminus;
								materialaddr = materialsetup - (firstuv - mat);
							}
							// Actual UV edit data is not required for the texture animations to work
							for (var v = 0; v < uvdata.UVEditEntries; v++)
							{
								if (v == 0)
								{
									uvdatabytes.AddRange(ByteConverter.GetBytes(materialsetup));
								}
								else
								{
									uvdatabytes.AddRange(ByteConverter.GetBytes(materialsetup + polyadd[v]));
								}

								uvdatabytes.AddRange(ByteConverter.GetBytes(uvdata.UVEditData[v].U));
								uvdatabytes.AddRange(ByteConverter.GetBytes(uvdata.UVEditData[v].V));
							}
							// UV Array segment calculation
							uvdataarrayaddrs[(q * 100) + j] = uvstart + (uvdata.UVEditEntries * 0x8);

							uvdatabytes.AddRange(ByteConverter.GetBytes(uvdata.TextureID));
							uvdatabytes.AddRange(ByteConverter.GetBytes(materialaddr));
							uvdatabytes.AddRange(ByteConverter.GetBytes(uvdata.UVEditEntries));
							uvdatabytes.AddRange(ByteConverter.GetBytes(uvstart));
							imageBase += (uint)(uvdata.UVEditEntries * 0x8) + 0x10;
						}
						uvdataaddrs[q] = (int)imageBase;
						for (var j = 0; j < maindata.TexAnimDataEntries; j++)
						{
							uvdatabytes.AddRange(ByteConverter.GetBytes(uvdataarrayaddrs[(q * 100) + j]));
							imageBase += 4;
						}
					}
					evtexanimref.AddRange(uvdatabytes);
					imageBase = (uint)(texanimptr + uvdatabytes.Count);
					// Texture animation array (Model Data)
					List<byte> texanimarraybytes = [];
					var texanimmasterptr = (int)imageBase;
					for (var h = 0; h < evinfo.TexAnimCount; h++)
					{
						var mainarray = texmaster.MainData[h];
						texanimarraybytes.AddRange(ByteConverter.GetBytes(labels[mainarray.Model]));
						texanimarraybytes.AddRange(ByteConverter.GetBytes(mainarray.TexAnimDataEntries));
						texanimarraybytes.AddRange(ByteConverter.GetBytes(uvdataaddrs[h]));
					}
					texanimarraybytes.AddRange(new byte[0xC]);
					evtexanimref.AddRange(texanimarraybytes);
					imageBase += (uint)texanimarraybytes.Count;
					texanimptr = (int)imageBase;
					// Master array is different between versions because
					// Battle supports more than one model with texture animations
					// PC version only reads the first one because it sucks
					if (battle)
					{
						List<byte> gcidbytes = [];
						var gcidpointer = (int)imageBase;
						
						for (var u = 0; u < texmaster.TexAnimGCIDCount; u++)
						{
							var gcid = texmaster.TexAnimGCIDs[u];
							gcidbytes.AddRange(ByteConverter.GetBytes(gcid.TexID));
							gcidbytes.AddRange(ByteConverter.GetBytes(gcid.TexLoopNumber));
							imageBase += 8;
						}
						evtexanimref.AddRange(gcidbytes);
						texanimptr = (int)imageBase;
						evtexanimref.AddRange(ByteConverter.GetBytes(texanimmasterptr));
						evtexanimref.AddRange(ByteConverter.GetBytes(gcidpointer));
						evtexanimref.AddRange(ByteConverter.GetBytes(texmaster.TexAnimGCIDCount));					
					}
					else
					{
						evtexanimref.AddRange(ByteConverter.GetBytes(texanimmasterptr));
						evtexanimref.AddRange(ByteConverter.GetBytes(texmaster.TexID));
						evtexanimref.AddRange(ByteConverter.GetBytes(texmaster.TexLoopNumber));
					}
					exdatabytes.AddRange(evtexanimref);
					imageBase += 0xC;
				}

				// Scene Array data goes here
				// Assets always exist
				List<byte> assetbytes = [];
				List<byte> bigbytes = [];
				List<byte> assetarraybytes = [];
				Dictionary<int, int> assetptrs = new();
				Dictionary<int, int> bigptrs = new();
				Dictionary<int, int> biganimptrs = new();
				for (var s = 1; s < evinfo.Scenes.Count; s++)
				{
					var scn = evinfo.Scenes[s];
					assetptrs[s] = (int)imageBase;
					for (var a = 0; a < scn.Entities.Count; a++)
					{
						if (battle)
						{
							if (scn.Entities[a].Model != "null" && labels.ContainsKeySafe(scn.Entities[a].Model))
							{
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].Model]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							if (scn.Entities[a].Motion != "null")
							{
								assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Entities[a].Motion]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							if (scn.Entities[a].ShapeMotion != "null")
							{
								assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Entities[a].ShapeMotion]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							if (scn.Entities[a].GCModel != "null" && labels.ContainsKeySafe(scn.Entities[a].GCModel))
							{
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].GCModel]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							if (scn.Entities[a].ShadowModel != "null" && labels.ContainsKeySafe(scn.Entities[a].ShadowModel))
							{
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].ShadowModel]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							// This segment would point to individual texture animation functions
							// akin to what's used for models like the fire somersault aura.
							// Predictably, no cutscenes use this variation of the feature.
							assetbytes.AddRange(new byte[4]);
							assetbytes.AddRange(scn.Entities[a].Position.GetBytes());
							assetbytes.AddRange(ByteConverter.GetBytes(scn.Entities[a].Flags));
							assetbytes.AddRange(ByteConverter.GetBytes(scn.Entities[a].Layer));
							imageBase += 0x2C;
						}
						else
						{
							if (scn.Entities[a].Model != "null" && labels.ContainsKeySafe(scn.Entities[a].Model))
							{
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].Model]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							if (scn.Entities[a].Motion != "null" && labels.ContainsKeySafe(scn.Entities[a].Motion))
							{
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].Motion]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							if (scn.Entities[a].ShapeMotion != "null" &&labels.ContainsKeySafe(scn.Entities[a].ShapeMotion))
							{
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].ShapeMotion]));
							}
							else
							{
								assetbytes.AddRange(new byte[4]);
							}

							// This segment would point to individual texture animation functions
							// akin to what's used for models like the fire somersault aura.
							// Predictably, no cutscenes use this variation of the feature.
							assetbytes.AddRange(new byte[4]);
							assetbytes.AddRange(scn.Entities[a].Position.GetBytes());
							assetbytes.AddRange(ByteConverter.GetBytes(scn.Entities[a].Flags));
							imageBase += 0x20;
						}
					}
				}
				// Asset information puts "Scene 0" information below all other scenes
				assetptrs[0] = (int)imageBase;
				var scn0 = evinfo.Scenes[0];
				for (var a = 0; a < scn0.Entities.Count; a++)
				{
					if (battle)
					{
						if (scn0.Entities[a].Model != "null" && labels.ContainsKeySafe(scn0.Entities[a].Model))
						{
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].Model]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						if (scn0.Entities[a].Motion != "null")
						{
							assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn0.Entities[a].Motion]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						if (scn0.Entities[a].ShapeMotion != "null")
						{
							assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn0.Entities[a].ShapeMotion]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						if (scn0.Entities[a].GCModel != "null" && labels.ContainsKeySafe(scn0.Entities[a].GCModel))
						{
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].GCModel]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						if (scn0.Entities[a].ShadowModel != "null" && labels.ContainsKeySafe(scn0.Entities[a].ShadowModel))
						{
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].ShadowModel]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						assetbytes.AddRange(new byte[4]);
						assetbytes.AddRange(scn0.Entities[a].Position.GetBytes());
						assetbytes.AddRange(ByteConverter.GetBytes(scn0.Entities[a].Flags));
						assetbytes.AddRange(ByteConverter.GetBytes(scn0.Entities[a].Layer));
						imageBase += 0x2C;
					}
					else
					{
						if (scn0.Entities[a].Model != "null" && labels.ContainsKeySafe(scn0.Entities[a].Model))
						{
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].Model]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						if (scn0.Entities[a].Motion != "null" && labels.ContainsKeySafe(scn0.Entities[a].Motion))
						{
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].Motion]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						if (scn0.Entities[a].ShapeMotion != "null" && labels.ContainsKeySafe(scn0.Entities[a].ShapeMotion))
						{
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].ShapeMotion]));
						}
						else
						{
							assetbytes.AddRange(new byte[4]);
						}

						assetbytes.AddRange(new byte[4]);
						assetbytes.AddRange(scn0.Entities[a].Position.GetBytes());
						assetbytes.AddRange(ByteConverter.GetBytes(scn0.Entities[a].Flags));
						imageBase += 0x20;
					}
				}
				// Big cameo data. Scene 0 doesn't have a pointer to Big's data.
				bigptrs[0] = 0;
				biganimptrs[0] = 0;
				List<byte> biganimbytes = [];
				// Animation array pointers
				for (var s = 1; s < evinfo.Scenes.Count; s++)
				{
					var scn = evinfo.Scenes[s];
					biganimptrs[s] = (int)imageBase + ((s - 1) * 8);
					for (var t = 0; t < scn.Big.Motions.Count; t++)
					{
						if (scn.Big.Motions[t] != null)
						{
							if (scn.Big.Motions[t][0] != null)
							{
								if (battle)
								{
									biganimbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Big.Motions[t][0]]));
								}
								else
								{
									if (labels.ContainsKeySafe(scn.Big.Motions[t][0]))
									{
										biganimbytes.AddRange(ByteConverter.GetBytes(labels[scn.Big.Motions[t][0]]));
									}
								}
							}
							else
							{
								biganimbytes.AddRange(new byte[4]);
							}

							if (scn.Big.Motions[t][1] != null)
							{
								if (battle)
								{
									biganimbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Big.Motions[t][1]]));
								}
								else
								{
									if (labels.ContainsKeySafe(scn.Big.Motions[t][1]))
									{
										biganimbytes.AddRange(ByteConverter.GetBytes(labels[scn.Big.Motions[t][1]]));
									}
								}
							}
							else
							{
								biganimbytes.AddRange(new byte[4]);
							}
						}
						else
						{
							biganimbytes.AddRange(new byte[8]);
						}
					}
				}
				imageBase += (uint)biganimbytes.Count;
				for (var s = 1; s < evinfo.Scenes.Count; s++)
				{
					var scn = evinfo.Scenes[s];
					bigptrs[s] = (int)imageBase + ((s - 1) * 0x10);
					if (scn.Big.Model != "null")
					{
						if (labels.ContainsKeySafe(scn.Big.Model))
						{
							bigbytes.AddRange(ByteConverter.GetBytes(labels[scn.Big.Model]));
						}

						if (scn.Big.Motions != null)
						{
							bigbytes.AddRange(ByteConverter.GetBytes(biganimptrs[s]));
							bigbytes.AddRange(ByteConverter.GetBytes(scn.Big.Motions.Count));
						}
						else
						{
							bigbytes.AddRange(new byte[8]);
						}

						bigbytes.AddRange(new byte[4]);
					}
					else
					{
						bigbytes.AddRange(new byte[0x10]);
					}
				}
				exdatabytes.AddRange(assetbytes);
				exdatabytes.AddRange(biganimbytes);
				exdatabytes.AddRange(bigbytes);
				imageBase += (uint)bigbytes.Count;

				// Master Asset Array
				var assetarraymaster = (int)imageBase;
				List<byte> masterbytes = [];
				for (var m = 0; m < evinfo.Scenes.Count; m++)
				{
					var scn = evinfo.Scenes[m];
					// An asset will always exist
					masterbytes.AddRange(ByteConverter.GetBytes(assetptrs[m]));
					masterbytes.AddRange(ByteConverter.GetBytes(evinfo.Scenes[m].Entities.Count));
					if (m > 0)
					{
						masterbytes.AddRange(ByteConverter.GetBytes(camarrayptrs[m]));
						masterbytes.AddRange(ByteConverter.GetBytes(scn.CameraMotions.Count));
						masterbytes.AddRange(ByteConverter.GetBytes(particlearrayptrs[m]));
						masterbytes.AddRange(ByteConverter.GetBytes(scn.ParticleMotions.Count));
						masterbytes.AddRange(ByteConverter.GetBytes(bigptrs[m]));
					}
					else
					{
						masterbytes.AddRange(new byte[0x14]);
					}

					masterbytes.AddRange(ByteConverter.GetBytes(scn.FrameCount));
				}
				exdatabytes.AddRange(masterbytes);
				imageBase += (uint)masterbytes.Count;

				// Pointer Arrays
				// Scene Asset Master Array
				evfile.AddRange(ByteConverter.GetBytes(assetarraymaster));
				// Texlist
				evfile.AddRange(ByteConverter.GetBytes(texlistaddr));
				// Scene Count
				evfile.AddRange(ByteConverter.GetBytes(evinfo.Scenes.Count - 1));
				// Texture dimensions
				evfile.AddRange(ByteConverter.GetBytes(sizeptr));
				// Reflection data
				evfile.AddRange(ByteConverter.GetBytes(reflectptr));
				// Blur Array
				evfile.AddRange(ByteConverter.GetBytes(blurptr));
				// Upgrade Override Array
				evfile.AddRange(ByteConverter.GetBytes(overrideptr));
				// Tails' tails
				evfile.AddRange(ByteConverter.GetBytes(tailsptr));
				// Upgrade Model Array
				evfile.AddRange(ByteConverter.GetBytes(upgradeptr));
				// Texture Animations
				if (evinfo.TexAnimCount != 0)
				{
					evfile.AddRange(ByteConverter.GetBytes(texanimptr));
				}
				else
				{
					evfile.AddRange(new byte[4]);
				}

				// Battle Shadows
				if (battle)
				{
					evfile.AddRange(ByteConverter.GetBytes(Convert.ToInt32(evinfo.GCShadowMaps)));
					if (evinfo.GCShadowMaps == true)
					{
						evfile.AddRange(new byte[0x14]);
					}
				}
				// All model/animation data goes here
				evfile.AddRange(databytes);

				// Non-model data goes here. It was split from databytes because of the texlist data
				for (var n = 0; n < tex.NumTextures; n++)
				{
					var tlsaddrs = (int)imageBase + texaddrs[n];
					evfile.AddRange(ByteConverter.GetBytes(tlsaddrs));
					evfile.AddRange(new byte[8]);
				}
				evfile.AddRange(ByteConverter.GetBytes(tlsstart));
				evfile.AddRange(ByteConverter.GetBytes(tex.NumTextures));
				evfile.AddRange(exdatabytes);
				// Texlist names
				evfile.AddRange(namebytes);

				if (fileOutputPath.Length != 0)
				{
					if (!Directory.Exists(fileOutputPath))
					{
						Directory.CreateDirectory(fileOutputPath);
					}

					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(evfile.ToArray(), filename);
					if (!File.Exists(filename))
					{
						File.Create(filename);
					}
				}
				else
				{
					Console.WriteLine("Main file will be uncompressed");
					File.WriteAllBytes(filename, evfile.ToArray());
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}
		
		public static void BuildTexlist(bool? isBigEndian, string filename, string fileOutputPath)
		{
			var dir = Environment.CurrentDirectory;
			try
			{
				filename = Path.GetFullPath(filename);
				if (Directory.Exists(filename))
				{
					filename += ".prs";
				}

				Environment.CurrentDirectory = Path.GetDirectoryName(filename);
				var path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new();
				EventTexListData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new(tr))
					evinfo = js.Deserialize<EventTexListData>(jtr);
				uint gamekey;
				if (isBigEndian.HasValue)
				{
					ByteConverter.BigEndian = isBigEndian.Value;
					if (isBigEndian.Value == true)
					{
						gamekey = 0;
					}
					else
					{
						gamekey = 0xCBC0000;
					}
				}
				else
				{
					ByteConverter.BigEndian = evinfo.BigEndian;
					
					gamekey = evinfo.BigEndian ? 0 : (uint)0xCBC0000;
				}
				List<byte> evfile = [];
				List<byte> databytes = [];
				var imageBase = gamekey + 4;
				var tlsstart = gamekey + 4;
				var tex = NJS_TEXLIST.Load(Path.Combine(Path.GetFileNameWithoutExtension(filename), "ExternalTexlist.satex"));
				List<byte> namebytes = [];
				Dictionary<int, int> texaddrs = new();
				for (var t = 0; t < tex.NumTextures; t++)
				{
					var names = tex.TextureNames[t];
					var texname = names.PadRight(28);
					namebytes.AddRange(Encoding.ASCII.GetBytes(texname));
					namebytes.AddRange(new byte[4]);
					var texaddr = 0x20 * t;
					texaddrs[t] = texaddr;
				}
				databytes.AddRange(namebytes);
				var texlistaddr = (int)(imageBase + (tex.NumTextures * 0xC));
				imageBase += (uint)((tex.NumTextures * 0xC) + 8);
				evfile.AddRange(ByteConverter.GetBytes(texlistaddr));
				for (var n = 0; n < tex.NumTextures; n++)
				{
					var tlsaddrs = (int)imageBase + texaddrs[n];
					evfile.AddRange(ByteConverter.GetBytes(tlsaddrs));
					evfile.AddRange(new byte[8]);
				}
				evfile.AddRange(ByteConverter.GetBytes(tlsstart));
				evfile.AddRange(ByteConverter.GetBytes(tex.NumTextures));
				evfile.AddRange(databytes);
				if (fileOutputPath.Length != 0)
				{
					if (!Directory.Exists(fileOutputPath))
					{
						Directory.CreateDirectory(fileOutputPath);
					}

					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(evfile.ToArray(), filename);
					if (!File.Exists(filename))
					{
						File.Create(filename);
					}
				}
				else
				{
					File.WriteAllBytes(filename, evfile.ToArray());
				}
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		//Get Functions
		private static string GetModel(byte[] fc, int address, uint key, string fn, string meta = null)
			{
				string name = null;
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					name = $"object_{ptr3:X8}";
					if (!nodeNames.Contains(name))
					{
						NJS_OBJECT obj = new(fc, ptr3, key, ModelFormat.Chunk, null);
						name = obj.Name;
						List<string> names = [..obj.GetObjects().Select((o) => o.Name)];
						foreach (var s in names)
							if (modelFiles.ContainsKey(s))
							{
								modelFiles.Remove(s);
							}

						nodeNames.AddRange(names);
						modelFiles.Add(obj.Name, new(fn, obj, ModelFormat.Chunk, meta));
					}
				}
				return name;
			}

		private static string GetGCModel(byte[] fc, int address, uint key, string fn, string meta = null)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"object_{ptr3:X8}";
				if (!nodeNames.Contains(name))
				{
					NJS_OBJECT obj = new(fc, ptr3, key, ModelFormat.GC, null);
					name = obj.Name;
					List<string> names = [..obj.GetObjects().Select((o) => o.Name)];
					foreach (var s in names)
						if (modelFiles.ContainsKey(s))
						{
							modelFiles.Remove(s);
						}

					nodeNames.AddRange(names);
					modelFiles.Add(obj.Name, new(fn, obj, ModelFormat.GC, meta));
				}
			}
			return name;
		}

		private static string GetMotion(byte[] fc, int address, uint key, string fn, List<NJS_MOTION> motions, int cnt, string meta = null)
		{
			NJS_MOTION mtn = null;
			if (motions != null)
			{
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			}
			else
			{
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					mtn = new(fc, ptr3, key, cnt, shortrot: false, shortcheck: false);
					mtn.ShortRot = false;
					mtn.Description = meta;
					mtn.OptimizeShape();
				}
			}
			if (mtn == null)
			{
				return null;
			}

			if (!motionFiles.ContainsKey(mtn.Name) || motionFiles[mtn.Name].Filename == null)
			{
				motionFiles[mtn.Name] = new(fn, mtn);
			}

			return mtn.Name;
		}

		private static string GetTexlist(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texlist_{ptr3:X8}";
			}
			return name;
		}

		private static string GetTexSizes(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texsizes_{ptr3:X8}";
			}
			return name;
		}

		private static string GetReflectData(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"reflectdata_{ptr3:X8}";
			}
			return name;
		}

		private static string GetCamData(byte[] fc, int address, uint key, string fn, List<NJS_CAMERA> ncams)
		{
			NJS_CAMERA ncam = null;
			if (ncams != null)
			{
				ncam = ncams[ByteConverter.ToInt32(fc, address + 0xC)];
			}
			else
			{
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new(fc, ptr3 + 0xC, key);
				}
			}
			if (ncam == null)
			{
				return null;
			}

			if (!camArrayFiles.ContainsKey(ncam.Name) || camArrayFiles[ncam.Name].Filename == null)
			{
				camArrayFiles[ncam.Name] = new(fn, ncam);
			}

			return ncam.Name;
		}

		private static string GetGCCamData(byte[] fc, int address, uint key, string fn, List<NJS_CAMERA> ncams)
		{
			NJS_CAMERA ncam = null;
			if (ncams != null)
			{
				ncam = ncams[ByteConverter.ToInt32(fc, address)];
			}
			else
			{
				var ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new(fc, ptr3, key);
				}
			}
			if (ncam == null)
			{
				return null;
			}

			if (!camArrayFiles.ContainsKey(ncam.Name) || camArrayFiles[ncam.Name].Filename == null)
			{
				camArrayFiles[ncam.Name] = new(fn, ncam);
			}

			return ncam.Name;
		}

		private static string GetTexanim(byte[] fc, int address, uint key, string fn)
		{
			EV_TEXANIM texanim = null;
			string name = null;
			var ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texanim_{ptr3:X8}";
				texanim = new(fc, ptr3, key);
			}
			if (texanim == null)
			{
				return null;
			}

			if (!texAnimFiles.ContainsKey(name) || texAnimFiles[name].Filename == null)
			{
				texAnimFiles[name] = new(fn, texanim);
			}

			return name;
		}

		//Read Functions
		private static List<NJS_MOTION> ReadMotionFile(string filename)
		{
			List<NJS_MOTION> motions = [];
			List<NJS_CAMERA> cam = [];
			NJS_MOTION mtn;
			var fc = File.ReadAllBytes(filename);
			var addr = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				var ptr = ByteConverter.ToInt32(fc, addr);
				var nummdl = ByteConverter.ToInt32(fc, addr + 4);
				var camcheck1 = fc.GetPointer(addr, 0);
				var camcheck2 = ByteConverter.ToUInt32(fc, camcheck1 + 8);
				if (ptr == -1)
				{
					motions.Add(null);
				}
				else
				{
					mtn = new(fc, ptr, 0, nummdl, shortrot: false, shortcheck: false);
					mtn.OptimizeShape();
					motions.Add(mtn);
					if (nummdl == 1 && camcheck2 == 0x1C10004)
					{
						cam.Add(new(fc, ptr + 0xC, 0));
					}
				}
				addr += 8;
			}
			return motions;
		}

		private static List<NJS_CAMERA> ReadMotionFileCams(string filename)
		{
			List<NJS_CAMERA> cam = [];
			var fc = File.ReadAllBytes(filename);
			var addr = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				var ptr = ByteConverter.ToInt32(fc, addr);
				var nummdl = ByteConverter.ToInt32(fc, addr + 4);
				var camcheck1 = fc.GetPointer(addr, 0);
				var camcheck2 = ByteConverter.ToUInt32(fc, camcheck1 + 8);
				if (ptr == -1)
				{
					cam.Add(null);
				}
				else
				{
					if (nummdl == 1 && camcheck2 == 0x1C10004)
					{
						cam.Add(new(fc, ptr + 0xC, 0));
					}
					else
					{
						cam.Add(null);
					}
				}
			addr += 8;
			}
			return cam;
		}

		private static Dictionary<int, string> ReadMotionFileNames(string filename)
		{
			Dictionary<int, string> motions = new();
			List<NJS_CAMERA> cam = [];
			string mtn;
			var fc = File.ReadAllBytes(filename);
			var addr = 0;
			var i = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				var ptr = ByteConverter.ToInt32(fc, addr);
				var nummdl = ByteConverter.ToInt32(fc, addr + 4);
				var camcheck1 = fc.GetPointer(addr, 0);
				var camcheck2 = ByteConverter.ToUInt32(fc, camcheck1 + 8);
				if (ptr == -1)
				{
					motions.Add(i, null);
				}
				else
				{
					mtn = new NJS_MOTION(fc, ptr, 0, nummdl, shortrot: false, shortcheck: false).Name;
					motions.Add(i, mtn);
					if (nummdl == 1 && camcheck2 == 0x1C10004)
					{
						cam.Add(new(fc, ptr + 0xC, 0));
					}
				}
				addr += 8;
				i++;
			}
			return motions;
		}

		public static bool ContainsKeySafe<TValue>(this IDictionary<string, TValue> dict, string key)
		{
			return key != null && dict.ContainsKey(key);
		}
	}

	public class ModelInfo
	{
		public string Filename { get; set; }
		public NJS_OBJECT Model { get; set; }
		public ModelFormat Format { get; set; }
		public List<string> Motions { get; set; } = [];
		public string MetaName { get; set; }

		public ModelInfo(string fn, NJS_OBJECT obj, ModelFormat format, string desc = null)
		{
			Filename = fn;
			Model = obj;
			Format = format;
			MetaName = desc;
		}
	}

	public class MotionInfo
	{
		public string Filename { get; set; }
		public NJS_MOTION Motion { get; set; }
		public string Description { get; set; }

		public MotionInfo(string fn, NJS_MOTION mtn, string desc = null)
		{
			Filename = fn;
			Motion = mtn;
			Description = desc;
		}
	}

	public class CameraInfo
	{
		public string Filename { get; set; }
		public NJS_CAMERA CamData { get; set; }

		public CameraInfo(string fn, NJS_CAMERA ncam)
		{
			Filename = fn;
			CamData = ncam;
		}
	}

	public class TexAnimFileInfo
	{
		public string Filename { get; set; }
		public EV_TEXANIM TextureAnimation { get; set; }

		public TexAnimFileInfo(string fn, EV_TEXANIM texanim)
		{
			Filename = fn;
			TextureAnimation = texanim;
		}
	}

	public class EventIniData
	{
		public string Name { get; set; }
		[JsonIgnore]
		public Game Game { get; set; }
		[JsonProperty(PropertyName = "Game")]
		public string GameString
		{
			get => Game.ToString();
			set => Game = (Game)Enum.Parse(typeof(Game), value);
		}
		public bool BigEndian { get; set; }
		public bool BattleFormat { get; set; }
		public bool DCBeta { get; set; }
		public Dictionary<string, string> Files { get; set; } = new();
		public int UpgradeCount { get; set; }
		public List<UpgradeInfo> Upgrades { get; set; } = [];
		public int BlurCount { get; set; }
		public Dictionary<int, string> BlurModels { get; set; } = new();
		public int OverrideCount { get; set; }
		public Dictionary<int, string> UpgradeOverrideParts { get; set; } = new();
		public string TailsTails { get; set; }
		public string Texlist { get; set; }
		public string TexSizes { get; set; }
		public bool GCShadowMaps { get; set; }
		public List<SceneInfo> Scenes { get; set; } = [];
		public int TexAnimCount { get; set; }
		public int UsedTexAnims { get; set; }
		public TexAnimInfo TextureAnimations { get; set; }
		public string ReflectionInfo { get; set; }
		public Dictionary<int, string> Motions { get; set; } = new();
	}

	public class UpgradeInfo
	{
		public string UpgradeName { get; set; }
		public string RootNode { get; set; }
		public string AttachNode1 { get; set; }
		public string Model1 { get; set; }
		public string AttachNode2 { get; set; }
		public string Model2 { get; set; }
	}

	public class SceneInfo
	{
		public int SceneNumber { get; set; }
		public List<EntityInfo> Entities { get; set; } = [];
		public List<string> CameraMotions { get; set; } = [];
		public List<string> NinjaCameras { get; set; } = [];
		public List<string> ParticleMotions { get; set; } = [];
		public BigInfo Big { get; set; }
		public int FrameCount { get; set; }
	}

	public class EntityInfo
	{
		public int EntityNumber { get; set; }
		public string ModelFile { get; set; }
		public string GCModelFile { get; set; }
		public string ShadowModelFile { get; set; }
		public string Model { get; set; }
		public string Motion { get; set; }
		public string ShapeMotion { get; set; }
		public string GCModel { get; set; }
		public string ShadowModel { get; set; }
		public Vertex Position { get; set; }
		public uint Flags { get; set; }
		public uint Layer { get; set; }
	}

	public class BigInfo
	{
		public string Model { get; set; }
		public List<string[]> Motions { get; set; } = [];
		public int Unknown { get; set; }
	}

	[Serializable]
	public class TexSizes
	{
		public string Name { get; set; }
		public int TextureCount { get; set; }
		public List<TexSizeArray> TextureSize { get; set; } = [];

		public TexSizes() { }

		public TexSizes(byte[] file, int address, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.ContainsKey(address))
			{
				Name = labels[address];
			}
			else
			{
				Name = "texsizes_" + address.ToString("X8");
			}

			TextureSize = [];
			var numtex = ByteConverter.ToInt32(file, address - 4);
			TextureCount = numtex;
			for (var i = 0; i < numtex; i++)
			{
				TextureSize.Add(new(file, address));
				address += 0x4;
			}

		}
		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}
		public static TexSizes Load(string filename) => IniSerializer.Deserialize<TexSizes>(filename);
	}

	[Serializable]
	public class TexSizeArray
	{
		[IniAlwaysInclude]
		public short H { get; set; }
		[IniAlwaysInclude]
		public short V { get; set; }

		public TexSizeArray() { }
		public TexSizeArray(byte[] file, int address)
		{
			H = ByteConverter.ToInt16(file, address);
			V = ByteConverter.ToInt16(file, address + 2);
		}

		public static int Size => 0x4;

		public byte[] GetBytes()
		{
			List<byte> result = new(Size);
			result.AddRange(ByteConverter.GetBytes(H));
			result.AddRange(ByteConverter.GetBytes(V));
			result.Align(4);
			return result.ToArray();
		}
	}
	public class TexAnimInfo
	{
		public List<TexAnimMain> MainData { get; set; } = [];
		public List<TexAnimGCIDs> TexAnimGCIDs { get; set; } = [];
		public int TexAnimGCIDCount { get; set; }
		public int TexID { get; set; }
		public int TexLoopNumber { get; set; }
		public int TexAnimMainDataEntries { get; set; }

	}
	public class TexAnimGCIDs
	{
		public int TexID { get; set; }
		public int TexLoopNumber { get; set; }

		private static int Size => 0x8;

		public byte[] GetBytes()
		{
			List<byte> result = new(Size);
			result.AddRange(ByteConverter.GetBytes(TexID));
			result.AddRange(ByteConverter.GetBytes(TexLoopNumber));
			result.Align(8);
			return result.ToArray();
		}
	}

	public class TexAnimMain
	{
		public string ModelFile { get; set; }
		public string Model { get; set; }
		public int TexAnimDataEntries { get; set; }
		public List<string> TexAnimPointers { get; } = [];
	}

	public class ReflectionInfo
	{
		public string Name { get; set; }
		public int Instances { get; }
		public List<ReflectionMatrixData> ReflectData { get; }

		public ReflectionInfo(byte[] file, int address, uint imageBase, Dictionary<int, string> labels = null)
		{
			if (labels != null && labels.TryGetValue(address, out var name))
			{
				Name = name;
			}
			else
			{
				Name = "reflectdata_" + address.ToString("X8");
			}

			ReflectData = [];
			
			var reflectCount = ByteConverter.ToInt32(file, address);
			Instances = reflectCount;
			
			for (var i = 0; i < reflectCount; i++)
			{
				var reflectOffset = ByteConverter.ToUInt32(file, address + 0x84);
				var reflectAddr = (int)(reflectOffset - imageBase);
				
				ReflectData.Add(new ReflectionMatrixData(file, reflectAddr));
				ReflectData[i].Opacity = ByteConverter.ToInt32(file, address + 4);
				address += 0x4;
			}
		}
		
		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public static ReflectionInfo Load(string filename) => IniSerializer.Deserialize<ReflectionInfo>(filename);
	}

	[Serializable]
	public class ReflectionMatrixData
	{
		public string Name { get; set; }
		public int Opacity { get; set; }
		public Vertex Data1 { get; set; }
		public Vertex Data2 { get; set; }
		public Vertex Data3 { get; set; }
		public Vertex Data4 { get; set; }

		public ReflectionMatrixData() { }

		public ReflectionMatrixData(byte[] file, int address, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.TryGetValue(address, out var nameLabel))
			{
				Name = nameLabel;
			}
			else
			{
				Name = "matrix_" + address.ToString("X8");
			}

			// Placeholder
			Opacity = 0;
			Data1 = new Vertex(file, address);
			Data2 = new Vertex(file, address + 0xC);
			Data3 = new Vertex(file, address + 0x18);
			Data4 = new Vertex(file, address + 0x24);
		}

		public static int Size => 0x30;

		public byte[] GetBytes()
		{
			List<byte> result = new(Size);
			result.AddRange(Data1.GetBytes());
			result.AddRange(Data2.GetBytes());
			result.AddRange(Data3.GetBytes());
			result.AddRange(Data4.GetBytes());
			result.Align(0x30);
			return result.ToArray();
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}
	}

	[Serializable]
	public class EV_TEXANIM
	{
		public string Name { get; set; }
		[IniAlwaysInclude]
		public int TextureID { get; set; }
		public string MaterialTexAddress { get; set; }
		[IniAlwaysInclude]
		public int UVEditEntries { get; set; }
		public string TexAnimArrayName { get; set; }
		public List<UVData> UVEditData { get; set; }

		public EV_TEXANIM() { }
		public EV_TEXANIM(byte[] file, int address, uint imageBase, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.TryGetValue(address, out var nameLabel))
			{
				Name = nameLabel;
			}
			else
			{
				Name = "texanim_" + address.ToString("X8");
			}

			TextureID = ByteConverter.ToInt32(file, address);
			
			var materialOffset = ByteConverter.ToUInt32(file, address + 4);
			var materialAddr = (int)(materialOffset - imageBase);
			
			if (labels != null && labels.TryGetValue(materialAddr, out var texAddress))
			{
				MaterialTexAddress = texAddress;
			}
			else
			{
				MaterialTexAddress = "mattex_" + materialAddr.ToString("X8");
			}

			UVEditEntries = ByteConverter.ToInt32(file, address + 8);
			
			var texAnimArrayOffset = ByteConverter.ToUInt32(file, address + 0xC);
			var texAnimArrayAddr = (int)(texAnimArrayOffset - imageBase);
			
			if (labels != null && labels.TryGetValue(texAnimArrayAddr, out var animArrayName))
			{
				TexAnimArrayName = animArrayName;
			}
			else
			{
				TexAnimArrayName = "uvanim_" + texAnimArrayAddr.ToString("X8");
			}

			UVEditData = [];
			
			if (texAnimArrayOffset == 0)
			{
				return;
			}
			
			for (var i = 0; i < UVEditEntries; i++)
			{
				UVEditData.Add(new UVData(file, texAnimArrayAddr, imageBase));
				texAnimArrayAddr += 0x8;
			}
		}
		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public static EV_TEXANIM Load(string filename) => IniSerializer.Deserialize<EV_TEXANIM>(filename);

	}
	[Serializable]
	public class UVData
	{
		public string UVAddress { get; set; }
		[IniAlwaysInclude]
		public short U { get; set; }
		[IniAlwaysInclude]
		public short V { get; set; }

		public UVData() { }

		public UVData(byte[] file, int address, uint imageBase)
		{
			var uvAddr = ByteConverter.ToUInt32(file, address);
			var ptr = (int)(uvAddr - imageBase);
			
			UVAddress = "uv_" + ptr.ToString("X8");
			U = ByteConverter.ToInt16(file, address + 4);
			V = ByteConverter.ToInt16(file, address + 6);

		}
	}

	[Serializable]
	public class NJS_CAMERA
	{
		public string Name { get; set; }
		public string NinjaCameraName { get; set; }
		public NinjaCamera NinjaCameraData { get; set; }
		public string CameraAnimation { get; set; }

		public NJS_CAMERA(byte[] file, int address, uint imageBase, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.TryGetValue(address, out var nameLabel))
			{
				Name = nameLabel;
			}
			else
			{
				Name = "camdata_" + address.ToString("X8");
			}

			var ncamaddr = ByteConverter.ToInt32(file, address);
			var animaddr = ByteConverter.ToInt32(file, address + 4);
			var ncamptr = (int)(ncamaddr - imageBase);
			var animptr = (int)(animaddr - imageBase);
			if (labels != null && labels.TryGetValue(ncamptr, out var cameraNameLabel))
			{
				NinjaCameraName = cameraNameLabel;
			}
			else
			{
				NinjaCameraName = "ninjacam_" + ncamptr.ToString("X8");
			}

			NinjaCameraData = new(file, ncamptr);
			if (labels != null && labels.TryGetValue(animptr, out var animLabel))
			{
				CameraAnimation = animLabel;
			}
			else
			{
				CameraAnimation = "animation_" + animptr.ToString("X8");
			}
		}

		public NJS_CAMERA() { }

		public static NJS_CAMERA Load(string filename) => IniSerializer.Deserialize<NJS_CAMERA>(filename);

		public static int Size => 0x40;

		public byte[] GetBytes(Dictionary<string, uint> labels)
		{
			List<byte> result = new(Size);
			result.AddRange(NinjaCameraData.GetBytes());
			result.AddRange(ByteConverter.GetBytes(labels[NinjaCameraName]));
			result.AddRange(ByteConverter.GetBytes(labels[CameraAnimation]));
			result.Align(0x40);
			return result.ToArray();
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}
	}

	public class EventTexListData
	{
		public string Name { get; set; }
		[JsonIgnore]
		public Game Game { get; set; }
		[JsonProperty(PropertyName = "Game")]
		public string GameString
		{
			get => Game.ToString();
			set => Game = (Game)Enum.Parse(typeof(Game), value);
		}
		public bool BigEndian { get; set; }
		public Dictionary<string, string> Files { get; set; } = new();
		public string TexList { get; set; }
	}
}
