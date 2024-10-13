using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SplitTools.SAArc
{
	public static class sa2Event
	{
		static readonly string[] upgradenames = { "Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claw L", "Knuckles' Shovel Claw R", "Knuckles' Hammer Glove L", "Knuckles' Hammer Glove R", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates", "Mech Tails' Window", "Mech Eggman's Window" };
		static readonly string[] upgradebetanames = { "Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claws", "Knuckles' Hammer Gloves", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates" };
		static List<string> nodenames = new List<string>();
		static Dictionary<string, ModelInfo> modelfiles = new Dictionary<string, ModelInfo>();
		static Dictionary<string, MotionInfo> motionfiles = new Dictionary<string, MotionInfo>();
		static Dictionary<string, CameraInfo> camarrayfiles = new Dictionary<string, CameraInfo>();
		static Dictionary<string, TexAnimFileInfo> texanimfiles = new Dictionary<string, TexAnimFileInfo>();

		public static void Split(string filename, string outputPath, string labelFile = null)
		{
			nodenames.Clear();
			modelfiles.Clear();
			camarrayfiles.Clear();
			motionfiles.Clear();
			texanimfiles.Clear();
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[outputPath.Length - 1] != '/') outputPath = string.Concat(outputPath, "/");
				// get file name, read it from the console if nothing
				string evfilename = filename;

				evfilename = Path.GetFullPath(evfilename);
				string EventFileName = Path.GetFileNameWithoutExtension(evfilename);
				if (Path.GetExtension(evfilename).Equals(".bin", StringComparison.OrdinalIgnoreCase))
					EventFileName += "_bin";

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(evfilename);
				else
					fc = File.ReadAllBytes(evfilename);
				if (Path.GetExtension(evfilename).Equals(".bin", StringComparison.OrdinalIgnoreCase) && fc[0] == 0x0F && fc[1] == 0x81)
					fc = Prs.Decompress(evfilename);
				EventIniData ini = new EventIniData() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
					Directory.CreateDirectory(EventFileName);

				// Metadata for SAMDL Project Mode
				byte[] mlength = null;
				Dictionary<string, string> evsectionlist = new Dictionary<string, string>();
				Dictionary<string, string> evsplitfilenames = new Dictionary<string, string>();
				if (labelFile != null) labelFile = Path.GetFullPath(labelFile);
				if (File.Exists(labelFile))
				{
					evsplitfilenames = IniSerializer.Deserialize<Dictionary<string, string>>(labelFile);
					mlength = File.ReadAllBytes(labelFile);
				}
				string evname = Path.GetFileNameWithoutExtension(evfilename);
				string evtexname = Path.Combine("EVENT", evname);
				string[] evmetadata = new string[0];

				uint key;
				List<NJS_MOTION> motions = null;
				Dictionary<int, string> motionnames = null;
				List<NJS_CAMERA> ncams = null;
				bool battle;
				bool dcbeta;

				// These are for establishing a Motions dictionary for non-Battle files to facilitiate converting between game versions
				int m = 1;
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
						motions = ReadMotionFile(Path.ChangeExtension(evfilename, null) + "motion.bin");
						ncams = ReadMotionFileCams(Path.ChangeExtension(evfilename, null) + "motion.bin");
						motionnames = ReadMotionFileNames(Path.ChangeExtension(evfilename, null) + "motion.bin");
						ini.Motions = motionnames;
						foreach (var mtn in motions.Where(a => a != null))
							motionfiles[mtn.Name] = new MotionInfo(null, mtn, mtn.Description);
						foreach (var camdata in ncams.Where(a => a != null))
							camarrayfiles[camdata.Name] = new CameraInfo(null, camdata);
					}
					else
					{
						Console.WriteLine("File is in GC/PC Beta format.");
						key = 0x812FFE60;
						battle = false;
						ini.BattleFormat = false;
						motionnames = dctogcmotions;
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
					motionnames = dctogcmotions;
					int upptr = fc.GetPointer(0x20, 0xC600000);
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
						motionnames = dctogcmotions;
					}

				}
				if (!battle)
					ini.Motions.Add(0, null);
				int ptr = fc.GetPointer(0x20, key);
				if (ptr != 0)
				{
					int upcount = 0;
					if (!dcbeta)
					{
						for (int i = 0; i < (battle ? 18 : 16); i++)
						{
							string upnam = upgradenames[i];
							string chnam = upnam;
							switch (i)
							{
								case 0:
									chnam = "Sonic";
									break;
								case 4:
									chnam = "Shadow";
									break;
								case 6:
									chnam = "Knuckles";
									break;
								case 12:
									chnam = "Rouge";
									break;
								case 16:
									chnam = "Mech Tails";
									break;
								case 17:
									chnam = "Mech Eggman";
									break;
							}
							UpgradeInfo info = new UpgradeInfo();
							info.UpgradeName = upnam;
							if (i >= 6 && i < 12)
							{
								string knucklesName = null;
								switch (i)
								{
									case 6:
									case 8:
										knucklesName = "Knuckles' Left Hand";
										break;
									case 7:
									case 9:
										knucklesName = "Knuckles' Right Hand";
										break;
									case 10:
									case 11:
										knucklesName = "Knuckles Root";
										break;
								}
								info.RootNode = GetModel(fc, ptr, key, $"{knucklesName}.sa2mdl", $"{evname} {knucklesName}");
							}
							else
								info.RootNode = GetModel(fc, ptr, key, $"{chnam} Root.sa2mdl", $"{evname} {chnam} Root");
							if (info.RootNode != null)
							{
								// populating metadata file
								string outResultRoot = null;
								string outResultU1 = null;
								string outResultU2 = null;
								// checks if the source ini is a placeholder
								if (labelFile != null && mlength.Length != 0)
								{
									evmetadata = evsplitfilenames[modelfiles[info.RootNode].Filename].Split('|'); // Description|Texture file
									outResultRoot += evmetadata[0] + "|" + evmetadata[1];
									evsectionlist[modelfiles[info.RootNode].Filename] = outResultRoot;
								}
								else
								{
									outResultRoot += modelfiles[info.RootNode].MetaName + "|" + $"{evtexname}texture";
									evsectionlist[modelfiles[info.RootNode].Filename] = outResultRoot;
								}
								int ptr2 = fc.GetPointer(ptr + 4, key);
								if (ptr2 != 0)
									info.AttachNode1 = $"object_{ptr2:X8}";
								else
									info.AttachNode1 = "null";
								int ptr3 = fc.GetPointer(ptr + 8, key);
								if (ptr3 != 0)
								{
									info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl", $"{evname} {upnam} A");
									Console.WriteLine($"Event contains {upnam}.");
									upcount++;

									// populating metadata file
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[info.Model1].Filename].Split('|'); // Description|Texture file
										outResultU1 += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelfiles[info.Model1].Filename] = outResultU1;
									}
									else
									{
										outResultU2 += modelfiles[info.Model1].MetaName + "|" + $"{evtexname}texture";
										evsectionlist.Add(modelfiles[info.Model1].Filename, outResultU2);
									}
								}
								else
									info.Model1 = "null";
								ptr2 = fc.GetPointer(ptr + 0xC, key);
								if (ptr2 != 0)
									info.AttachNode2 = $"object_{ptr2:X8}";
								else
									info.AttachNode2 = "null";
								ptr3 = fc.GetPointer(ptr + 0x10, key);
								if (ptr3 != 0)
								{
									info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl", $"{evname} {upnam} B");
									// populating metadata file
									string outResult = null;
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[info.Model2].Filename].Split('|'); // Description|Texture file
										outResult += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelfiles[info.Model2].Filename] = outResult;
									}
									else
									{
										outResult += modelfiles[info.Model2].MetaName + "|" + $"{evtexname}texture";
										evsectionlist.Add(modelfiles[info.Model2].Filename, outResult);
									}
								}
								else
									info.Model2 = "null";
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
							UpgradeInfo pad = new UpgradeInfo();
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
							Console.WriteLine("Event contains no character upgrades.");
					}
					else
					{
						for (int i = 0; i < 14; i++)
						{
							string upnam = upgradebetanames[i];
							string chnam = upnam;
							switch (i)
							{
								case 0:
									chnam = "Sonic";
									break;
								case 4:
									chnam = "Shadow";
									break;
								case 6:
									chnam = "Knuckles";
									break;
								case 10:
									chnam = "Rouge";
									break;
							}
							UpgradeInfo info = new UpgradeInfo();
							info.UpgradeName = upnam;
							info.RootNode = GetModel(fc, ptr, key, $"{chnam} Root.sa2mdl", $"{evname} {chnam} Root");
							if (info.RootNode != null)
							{
								// populating metadata file
								string outResultRoot = null;
								// checks if the source ini is a placeholder
								if (labelFile != null && mlength.Length != 0)
								{
									evmetadata = evsplitfilenames[modelfiles[info.RootNode].Filename].Split('|'); // Description|Texture file
									outResultRoot += evmetadata[0] + "|" + evmetadata[1];
									evsectionlist[modelfiles[info.RootNode].Filename] = outResultRoot;
								}
								else
								{
									outResultRoot += modelfiles[info.RootNode].MetaName + "|" + evtexname;
									evsectionlist[modelfiles[info.RootNode].Filename] = outResultRoot;
								}
								int ptr2 = fc.GetPointer(ptr + 4, key);
								if (ptr2 != 0)
									info.AttachNode1 = $"object_{ptr2:X8}";
								else
									info.AttachNode1 = "null";
								string outResultU1 = null;
								string outResultU2 = null;
								int ptr3 = fc.GetPointer(ptr + 8, key);
								if (ptr3 != 0)
								{
									info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl", $"{evname} {upnam} A");
									Console.WriteLine($"Event contains {upnam}.");
									upcount++;
									// populating metadata file
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[info.Model1].Filename].Split('|'); // Description|Texture file
										outResultU1 += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelfiles[info.Model1].Filename] = outResultU1;
									}
									else
									{
										outResultU1 = modelfiles[info.Model1].MetaName + "|" + evtexname;
										evsectionlist.Add(modelfiles[info.Model1].Filename, outResultU1);
									}
								}
								else
									info.Model1 = "null";
								ptr2 = fc.GetPointer(ptr + 0xC, key);
								if (ptr2 != 0)
									info.AttachNode2 = $"object_{ptr2:X8}";
								else
									info.AttachNode2 = "null";
								ptr3 = fc.GetPointer(ptr + 0x10, key);
								if (ptr3 != 0)
								{
									info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl", $"{evname} {upnam} B");
									// populating metadata file
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[info.Model2].Filename].Split('|'); // Description|Texture file
										outResultU2 += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelfiles[info.Model2].Filename] = outResultU2;
									}
									else
									{
										outResultU2 = modelfiles[info.Model2].MetaName + "|" + evtexname;
										evsectionlist.Add(modelfiles[info.Model2].Filename, outResultU2);
									}
								}
								else
									info.Model2 = "null";
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
						UpgradeInfo pad = new UpgradeInfo();
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
							Console.WriteLine("Event contains no character upgrades.");
					}
				}
				ptr = fc.GetPointer(0x18, key);
				if (ptr != 0)
				{
					int namecount = 0;
					string name = null;
					for (int i = 0; i < 93; i++)
					{
						int ptr2 = fc.GetPointer(ptr, key);
						if (ptr2 != 0)
						{
							name = $"object_{ptr2:X8}";
							ini.UpgradeOverrideParts.Add(i, name);
							namecount++;
						}
						else
							ini.UpgradeOverrideParts.Add(i, null);
						ptr += 4;
					}
					ini.OverrideCount = namecount;
					Console.WriteLine("Event contains {0} upgrade override system pointe{1}.", namecount == 0 ? "no" : $"{namecount}", namecount == 1 ? "r" : "rs");
				}
				int gcnt = ByteConverter.ToInt32(fc, 8);
				ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					Console.WriteLine("Event contains {0} scen{1}.", gcnt + 1, gcnt + 1 == 1 ? "e" : "es");
					int scnnum = 0;
					for (int gn = 0; gn <= gcnt; gn++)
					{
						Directory.CreateDirectory(Path.Combine(EventFileName, $"Scene {gn}"));
						SceneInfo scn = new SceneInfo();
						int ptr2 = fc.GetPointer(ptr, key);
						int ecnt = ByteConverter.ToInt32(fc, ptr + 4);
						scn.SceneNumber = scnnum;
						if (ptr2 != 0)
						{
							int entnum = 1;
							Console.WriteLine("Scene {0} contains {1} entit{2}.", gn, ecnt, ecnt == 1 ? "y" : "ies");
							for (int en = 0; en < ecnt; en++)
							{
								EntityInfo ent = new EntityInfo();
								ent.EntityNumber = entnum;
								ent.Model = GetModel(fc, ptr2, key, $"Scene {gn}\\Entity {en + 1} Model.sa2mdl", $"{evname} Scene {gn} Entity {en + 1}");
								string EntityName = null;

								if (ent.Model != null)
								{
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[ent.Model].Filename].Split('|');
										EntityName = evmetadata[0];
									}
									else
										EntityName = modelfiles[ent.Model].MetaName;
									ent.ModelFile = modelfiles[ent.Model].Filename;
									ent.Motion = GetMotion(fc, ptr2 + 4, key, $"Scene {gn}\\Entity {en + 1} Motion.saanim", motions, modelfiles[ent.Model].Model.CountAnimated(), $"{EntityName} Scene {gn} Animation");
									if (ent.Motion != null)
									{
										// add metadata to animations found in motion.bin files
										if (battle)
											motionfiles[ent.Motion].Description = $"{EntityName} Scene {gn} Animation";

										if (modelfiles[ent.Model].Filename.EndsWith("Root.sa2mdl") || modelfiles[ent.Model].Filename.EndsWith("Hand.sa2mdl"))
											modelfiles[ent.Model].Motions.Add(motionfiles[ent.Motion].Filename);
										else
											modelfiles[ent.Model].Motions.Add("../" + motionfiles[ent.Motion].Filename);
										if (!battle)
										{
											ini.Motions.Add(m, ent.Motion);
											m++;
										}

									}
									else
										ent.Motion = "null";
									ent.ShapeMotion = GetMotion(fc, ptr2 + 8, key, $"Scene {gn}\\Entity {en + 1} Shape Motion.saanim", motions, modelfiles[ent.Model].Model.CountMorph(), $"{EntityName} Scene {gn} Shape Motion");
									if (ent.ShapeMotion != null)
									{
										// add metadata to animations found in motion.bin files
										if (battle)
											motionfiles[ent.ShapeMotion].Description = $"{EntityName} Scene {gn} Shape Motion";

										if (modelfiles[ent.Model].Filename.EndsWith("Root.sa2mdl") || modelfiles[ent.Model].Filename.EndsWith("Hand.sa2mdl"))
											modelfiles[ent.Model].Motions.Add(motionfiles[ent.ShapeMotion].Filename);
										else
											modelfiles[ent.Model].Motions.Add("../" + motionfiles[ent.ShapeMotion].Filename);
										if (!battle)
										{
											ini.Motions.Add(m, ent.ShapeMotion);
											m++;
										}
									}
									else
										ent.ShapeMotion = "null";
									// populating metadata file
									string outResult = null;
									// checks if the source ini is a placeholder
									if (labelFile != null && mlength.Length != 0)
									{
										evmetadata = evsplitfilenames[modelfiles[ent.Model].Filename].Split('|'); // Description|Texture file
										outResult += evmetadata[0] + "|" + evmetadata[1];
										evsectionlist[modelfiles[ent.Model].Filename] = outResult;
									}
									else
									{
										if (!dcbeta)
											outResult += modelfiles[ent.Model].MetaName + "|" + $"{evtexname}texture";
										else
											outResult += modelfiles[ent.Model].MetaName + "|" + evtexname;
										evsectionlist[modelfiles[ent.Model].Filename] = outResult;
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
									ent.Position = new Vertex(fc, ptr2 + 24);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 36);
									ent.Layer = ByteConverter.ToUInt32(fc, ptr2 + 40);
									// populating metadata file
									if (ent.GCModel != null)
									{
										string outResultGC = null;
										// checks if the source ini is a placeholder
										if (labelFile != null && mlength.Length != 0)
										{
											evmetadata = evsplitfilenames[modelfiles[ent.GCModel].Filename].Split('|'); // Description|Texture file
											outResultGC += evmetadata[0] + "|" + evmetadata[1];
											evsectionlist[modelfiles[ent.GCModel].Filename] = outResultGC;
										}
										else
										{
											outResultGC = modelfiles[ent.GCModel].MetaName + "|" + $"{evtexname}texture";
											evsectionlist[modelfiles[ent.GCModel].Filename] = outResultGC;
										}
										ent.GCModelFile = modelfiles[ent.GCModel].Filename;
									}
									else
										ent.GCModel = "null";
									if (ent.ShadowModel != null)
									{
										string outResultShadow = null;
										// checks if the source ini is a placeholder
										if (labelFile != null && mlength.Length != 0)
										{
											evmetadata = evsplitfilenames[modelfiles[ent.ShadowModel].Filename].Split('|'); // Description|Texture file
											outResultShadow += evmetadata[0];
											evsectionlist[modelfiles[ent.ShadowModel].Filename] = outResultShadow;
										}
										else
										{
											outResultShadow = modelfiles[ent.ShadowModel].MetaName;
											evsectionlist[modelfiles[ent.ShadowModel].Filename] = outResultShadow;
										}
										ent.ShadowModelFile = modelfiles[ent.ShadowModel].Filename;
									}
									else
										ent.ShadowModel = "null";
								}
								else
								{
									ent.GCModel = "null";
									ent.ShadowModel = "null";
									ent.Position = new Vertex(fc, ptr2 + 16);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 28);
								}
								scn.Entities.Add(ent);
								ptr2 += battle ? 0x2C : 0x20;
								entnum++;
							}
						}
						else
							Console.WriteLine("Scene {0} contains no entities.", gn);
						ptr2 = fc.GetPointer(ptr + 8, key);
						if (ptr2 != 0)
						{
							int camaddr = ByteConverter.ToInt32(fc, ptr2 + 0xC);
							int cnt = ByteConverter.ToInt32(fc, ptr + 12);
							for (int i = 0; i < cnt; i++)
							{
								scn.CameraMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn}\\Camera Motion {i + 1}.saanim", motions, 1, $"{evname} Scene {gn} Camera Animation {i + 1}"));
								if (battle)
								{
									// add metadata to animations found in motion.bin files
									motionfiles[scn.CameraMotions[i]].Description = $"{evname} Scene {gn} Camera Animation {i + 1}";
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
							int cnt = ByteConverter.ToInt32(fc, ptr + 0x14);
							Console.WriteLine("Scene {0} contains {1} particle motio{2}.", gn, cnt == 0 ? "no" : cnt, cnt == 1 ? "n" : "ns");
							for (int i = 0; i < cnt; i++)
							{
								scn.ParticleMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn}\\Particle Motion {i + 1}.saanim", motions, 1, $"{evname} Scene {gn} Particle Motion {i + 1}"));
								// add metadata to animations found in motion.bin files
								if (battle)
									motionfiles[scn.ParticleMotions[i]].Description = $"{evname} Scene {gn} Particle Motion {i + 1}";
								else
								{
									ini.Motions.Add(m, scn.ParticleMotions[i]);
									m++;
								}
								ptr2 += sizeof(int);
							}
						}
						else
							Console.WriteLine("Scene {0} contains no particle motions.", gn);
						ptr2 = fc.GetPointer(ptr + 0x18, key);
						if (ptr2 != 0)
						{
							BigInfo big = new BigInfo();
							big.Model = GetModel(fc, ptr2, key, $"Scene {gn}\\Big Model.sa2mdl", $"{evname} Scene {gn} Big Model");
							if (big.Model != null)
							{
								Console.WriteLine("Scene {0} contains Big the Cat cameo.", gn);
								int anicnt = modelfiles[big.Model].Model.CountAnimated();
								int ptr3 = fc.GetPointer(ptr2 + 4, key);
								if (ptr3 != 0)
								{
									int cnt = ByteConverter.ToInt32(fc, ptr2 + 8);
									for (int i = 0; i < cnt; i++)
									{
										big.Motions.Add(new string[] { GetMotion(fc, ptr3, key, $"Scene {gn}/Big Motion {i + 1}a.saanim", motions, anicnt, $"{evname} Scene {gn} Big Motion {i + 1}A"), GetMotion(fc, ptr3 + 4, key, $"Scene {gn}\\Big Motion {i + 1}b.saanim", motions, anicnt, $"{evname} Scene {gn} Big Motion {i + 1}B") });
										// add metadata to animations found in motion.bin files
										if (battle)
										{
											int ptr4 = fc.GetPointer(ptr3 + 4, key);
											motionfiles[big.Motions[i][0]].Description = $"{evname} Scene {gn} Big Motion {i + 1}A";
											modelfiles[big.Model].Motions.Add("../" + motionfiles[big.Motions[i][0]].Filename);
											if (ptr4 != 0)
											{
												motionfiles[big.Motions[i][1]].Description = $"{evname} Scene {gn} Big Motion {i + 1}B";
												modelfiles[big.Model].Motions.Add("../" + motionfiles[big.Motions[i][1]].Filename);
											}

										}
										else
										{
											ini.Motions.Add(m, big.Motions[i][0]);
											modelfiles[big.Model].Motions.Add("../" + motionfiles[big.Motions[i][0]].Filename);
											m++;
											int ptr4 = fc.GetPointer(ptr3 + 4, key);
											if (ptr4 != 0)
											{
												ini.Motions.Add(m, big.Motions[i][1]);
												modelfiles[big.Model].Motions.Add("../" + motionfiles[big.Motions[i][1]].Filename);
												m++;
											}
										}
										ptr3 += 8;
									}
								}
								// populating metadata file
								string outResultBig = null;
								// checks if the source ini is a placeholder
								if (labelFile != null && mlength.Length != 0)
								{
									evmetadata = evsplitfilenames[modelfiles[big.Model].Filename].Split('|'); // Description|Texture file
									outResultBig += evmetadata[0] + "|" + evmetadata[1];
									evsectionlist[modelfiles[big.Model].Filename] = outResultBig;
								}
								else
								{
									outResultBig = modelfiles[big.Model].MetaName + "|" + $"{evtexname}texture";
									evsectionlist.Add(modelfiles[big.Model].Filename, outResultBig);
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
					Console.WriteLine("Event contains no scenes.");
				ptr = fc.GetPointer(0x1C, key);
				if (ptr != 0)
				{
					ini.TailsTails = GetModel(fc, ptr, key, $"Tails' tails.sa2mdl", $"{evname} Tails' Tails");
					int ptr2 = fc.GetPointer(ptr, key);
					if (ptr2 == 0)
					{
						Console.WriteLine("Event does not contain Tails' tails.");
						ini.TailsTails = "null";
					}
					else
						Console.WriteLine("Event contains Tails' tails.");
				}
				ptr = fc.GetPointer(4, key);
				if (ptr != 0)
				{
					int ptr2 = fc.GetPointer(ptr, key);
					if (ptr2 != 0)
					{
						Console.WriteLine("Event contains an internal texture list.");
						NJS_TEXLIST tls = new NJS_TEXLIST(fc, ptr, key);
						ini.Texlist = GetTexlist(fc, 4, key, "InternalTexlist.satex");
						string fp = Path.Combine(EventFileName, "InternalTexlist.satex");
						tls.Save(fp);
						ini.Files.Add("InternalTexlist.satex", HelperFunctions.FileHash(fp));
					}
					else
						Console.WriteLine("Event does not contain an internal texture list.");
				}
				ptr = fc.GetPointer(0xC, key);
				if (ptr != 0)
				{
					ushort start = ByteConverter.ToUInt16(fc, ptr);
					if (start > 0)
					{
						Console.WriteLine("Event contains internal texture sizes.");
						TexSizes tsz = new TexSizes(fc, ptr);
						ini.TexSizes = GetTexSizes(fc, 0xC, key, "TextureSizes.ini");
						string fp = Path.Combine(EventFileName, "TextureSizes.ini");
						tsz.Save(fp);
						ini.Files.Add("TextureSizes.ini", HelperFunctions.FileHash(fp));
					}
					else
						Console.WriteLine("Event does not contain internal texture sizes.");
				}
				ptr = fc.GetPointer(0x10, key);
				if (ptr != 0)
				{
					uint start = ByteConverter.ToUInt32(fc, ptr);
					if (start > 0)
					{
						ReflectionInfo refl = new ReflectionInfo(fc, ptr, key);
						ini.ReflectionInfo = GetReflectData(fc, 0x10, key, "ReflectionData.ini");
						string fp = Path.Combine(EventFileName, "ReflectionData.ini");
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
					int namecount = 0;
					string name = null;
					for (int i = 0; i < 64; i++)
					{
						int ptr2 = fc.GetPointer(ptr, key);
						if (ptr2 != 0)
						{
							name = $"object_{ptr2:X8}";
							ini.BlurModels.Add(i, name);
							namecount++;
						}
						else
							ini.BlurModels.Add(i, null);
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
					Directory.CreateDirectory(Path.Combine(EventFileName, "Texture Animations"));
					TexAnimInfo tanim = new TexAnimInfo();
					int ptr2 = fc.GetPointer(ptr, key);
					if (battle)
					{
						int tn = ByteConverter.ToInt32(fc, ptr + 8);
						tanim.TexAnimGCIDCount = tn;
						int tanimcount = 0;
						int d = 1;
						while (ByteConverter.ToInt32(fc, ptr2) != 0)
						{
							TexAnimMain main = new TexAnimMain();
							int mptr = fc.GetPointer(ptr2, key);
							int taptr = fc.GetPointer(ptr2 + 8, key);
							int tanims = ByteConverter.ToInt32(fc, ptr2 + 4);
							if (mptr != 0)
							{
								tanimcount++;
								main.Model = $"object_{mptr:X8}";
								main.ModelFile = modelfiles[main.Model].Filename;
								main.TexAnimDataEntries = ByteConverter.ToInt32(fc, ptr2 + 4);
								for (int t = 0; t < tanims; t++)
								{
									main.TexAnimPointers.Add(GetTexanim(fc, taptr, key, $"Texture Animations\\TexanimInfo {d} Part {t + 1}.ini"));
									taptr += 0x4;
								}
								d++;
							}
							else
								break;
							tanim.MainData.Add(main);
							ptr2 += 0xC;
						}
						int ptr3 = fc.GetPointer(ptr + 4, key);
						for (int i = 0; i < tn; i++)
						{
							TexAnimGCIDs tgcids = new TexAnimGCIDs();
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
							Console.WriteLine("Event contains {0} mode{1} with texture animation data. {2} data se{3} {4} used.", $"{tanimcount}", tanimcount == 1 ? "l" : "ls", tn == 1 ? "Only the first" : $"The first {tn}", tn == 1 ? "t" : "ts", tn == 1 ? "is" : "are");
						else
							Console.WriteLine("Event contains {0} mode{1} with texture animation data.", $"{tanimcount}", tanimcount == 1 ? "l" : "ls");
						ini.UsedTexAnims = tn;
						ini.TexAnimCount = tanimcount;
					}
					else
					{
						int tanimcount = 0;
						int d = 1;
						while (ByteConverter.ToInt32(fc, ptr2) != 0)
						{
							TexAnimMain main = new TexAnimMain();
							int mptr = fc.GetPointer(ptr2, key);
							int taptr = fc.GetPointer(ptr2 + 8, key);
							int tanims = ByteConverter.ToInt32(fc, ptr2 + 4);
							if (mptr != 0)
							{
								tanimcount++;
								main.Model = $"object_{mptr:X8}";
								main.ModelFile = modelfiles[main.Model].Filename;
								main.TexAnimDataEntries = ByteConverter.ToInt32(fc, ptr2 + 4);
								for (int t = 0; t < tanims; t++)
								{
									main.TexAnimPointers.Add(GetTexanim(fc, taptr, key, $"Texture Animations\\TexanimInfo {d} Part {t + 1}.ini"));
									taptr += 0x4;
								}
								d++;
							}
							else
								break;
							tanim.MainData.Add(main);
							ptr2 += 0xC;
						}
						// This is for DC -> Battle conversions
						// Dreamcast only has one set of texture looping values
						tanim.TexAnimGCIDCount = 1;
						TexAnimGCIDs tgcids = new TexAnimGCIDs();
						tgcids.TexID = ByteConverter.ToInt32(fc, ptr + 4);
						tgcids.TexLoopNumber = ByteConverter.ToInt32(fc, ptr + 8);
						tanim.TexAnimGCIDs.Add(tgcids);

						tanim.TexID = ByteConverter.ToInt32(fc, ptr + 4);
						tanim.TexLoopNumber = ByteConverter.ToInt32(fc, ptr + 8);
						tanim.TexAnimMainDataEntries = 1;
						ini.TextureAnimations = tanim;
						if (tanimcount != 1)
							Console.WriteLine("Event contains {0} models with texture animation data. Only the first data set is used.", $"{tanimcount}");
						else
							Console.WriteLine("Event contains 1 model with texture animation data.");
						ini.TexAnimCount = tanimcount;
						ini.UsedTexAnims = 1;
					}

				}
				int shmap = ByteConverter.ToInt32(fc, 0x28);
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
				foreach (var item in motionfiles.Values)
				{
					string fn = item.Filename ?? $"Unknown Motion {motions.IndexOf(item.Motion)}.saanim";
					string fp = Path.Combine(EventFileName, fn);
					// applies metadata to animations found in motion.bin files
					if (battle)
					{ 
						string meta = item.Description ?? $"Unassigned Motion {motions.IndexOf(item.Motion)}";
						item.Motion.Description = meta;
					}
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in camarrayfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(EventFileName, fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in texanimfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(EventFileName, fn);
					item.TextureAnimation.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in modelfiles.Values)
				{
					string fp = Path.Combine(EventFileName, item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				// Creates metadata ini file for SAMDL Project Mode
				if (labelFile != null)
				{
					string evsectionListFilename = Path.GetFileNameWithoutExtension(labelFile) + "_data.ini";
					IniSerializer.Serialize(evsectionlist, Path.Combine(outputPath, evsectionListFilename));
				}
				JsonSerializer js = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
					using (var tw = File.CreateText(Path.Combine(EventFileName, Path.ChangeExtension(Path.GetFileName(evfilename), ".json"))))
						js.Serialize(tw, ini);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void SplitExternalTexlist(string filename, string outputPath)
		{
			string dir = Environment.CurrentDirectory;
			try
			{
				if (outputPath[outputPath.Length - 1] != '/') outputPath = string.Concat(outputPath, "/");
				// get file name, read it from the console if nothing
				string evfilename = filename;

				evfilename = Path.GetFullPath(evfilename);

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(evfilename);
				else
					fc = File.ReadAllBytes(evfilename);
				EventTexlistData satx = new EventTexlistData() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
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

				int ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					NJS_TEXLIST tls = new NJS_TEXLIST(fc, ptr, key);
					satx.Texlist = GetTexlist(fc, 0, key, "ExternalTexlist.satex");
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "ExternalTexlist.satex");
					tls.Save(fp);
					satx.Files.Add("ExternalTexlist.satex", HelperFunctions.FileHash(fp));
				}
				JsonSerializer js = new JsonSerializer
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
		public static void Build(string filename, string? format, string fileOutputPath)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			string dir = Environment.CurrentDirectory;
			try
			{
				if (fileOutputPath[fileOutputPath.Length - 1] != '/') fileOutputPath = string.Concat(fileOutputPath, "/");

				string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new JsonSerializer();
				EventIniData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new JsonTextReader(tr))
					evinfo = js.Deserialize<EventIniData>(jtr);

				uint gamekey;
				bool battle = evinfo.BattleFormat;
				bool dcbeta = evinfo.DCBeta;
				ByteConverter.BigEndian = evinfo.BigEndian;
				if (evinfo.Game == Game.SA2 || ((!evinfo.BattleFormat) && (!evinfo.BigEndian)))
					gamekey = 0xC600000;
				else if ((!evinfo.BattleFormat) && evinfo.BigEndian)
					gamekey = 0x812FFE60;
				else
					gamekey = 0x8125FE60;
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
								gamekey = 0xC600000;
							else if ((!evinfo.BattleFormat) && evinfo.BigEndian)
								gamekey = 0x812FFE60;
							else
								gamekey = 0x8125FE60;
							break;
					}
				}
				else
				{
					battle = evinfo.BattleFormat;
					dcbeta = evinfo.DCBeta;
					ByteConverter.BigEndian = evinfo.BigEndian;
					if (evinfo.Game == Game.SA2 || ((!evinfo.BattleFormat) && (!evinfo.BigEndian)))
						gamekey = 0xC600000;
					else if ((!evinfo.BattleFormat) && evinfo.BigEndian)
						gamekey = 0x812FFE60;
					else
						gamekey = 0x8125FE60;
				}
				uint imageBase = gamekey;

				List<byte> evfile = new List<byte>();
				List<byte> databytes = new List<byte>();
				List<byte> exdatabytes = new List<byte>();
				Dictionary<string, int> animaddrs = new Dictionary<string, int>();
				Dictionary<int, int> banimaddrs = new Dictionary<int, int>();
				Dictionary<int, uint> mdladdrs = new Dictionary<int, uint>();
				Dictionary<int, int> panimaddrs = new Dictionary<int, int>();
				Dictionary<int, int> shapeaddrs = new Dictionary<int, int>();
				List<byte> modelbytes = new List<byte>();
				List<byte> motionbytes = new List<byte>();
				List<byte> ncameradata = new List<byte>();
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				Dictionary<string, int> motionIDs = new Dictionary<string, int>();

				if (battle)
				{
					// Done for accuracy to the original files. Whether or not this is necessary is unclear.
					if (evinfo.GCShadowMaps == true)
						imageBase += 0x40;
					else
						imageBase += 0x2C;
					// Reverses the motion dictionary so the motion IDs can be written to main files
					foreach (KeyValuePair<int, string> mnames in evinfo.Motions.Where(a => a.Key != 0))
					{
						if (mnames.Value == null)
							motionIDs.Add("null", mnames.Key);
						else
							motionIDs.Add(mnames.Value, mnames.Key);
					}
				}
				else
					imageBase += 0x28;

				// Event models/animations. File acquisition code copied from original method, with some alterations.
				foreach (string file in evinfo.Files.Where(a => a.Key.EndsWith(".sa2mdl", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
				{
					if (battle)
					{
						if (file.EndsWith("Shadow Model.sa2mdl"))
							modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels, new List<uint>(), out uint shadowend));
						else
							modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels, new List<uint>(), out uint modelend));
					}
					else
					{
						if (!file.EndsWith("Shadow Model.sa2mdl"))
							modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels, new List<uint>(), out uint dcmodelend));
					}
				}
				if (battle)
				{
					foreach (string file in evinfo.Files.Where(a => a.Key.EndsWith(".sa2bmdl", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes(imageBase + (uint)modelbytes.Count, false, labels, new List<uint>(), out uint gcmodelend));
					}
					// Motion file building
					string motionfilename = filename;
					List<byte> motiondatabytes = new List<byte>();
					List<byte> motionfilebytes = new List<byte>();
					List<byte> bincameradata = new List<byte>();
					Dictionary<string, int> binanimaddrs = new Dictionary<string, int>();
					Dictionary<string, int> bincamaddrs = new Dictionary<string, int>();
					Dictionary<string, int> partcounts = new Dictionary<string, int>(evinfo.Motions.Count);
					uint motionImageBase = (uint)(evinfo.Motions.Count + 1) * 8;
					foreach (string file in evinfo.Files.Where(a => a.Key.EndsWith("Attributes.ini", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						NJS_CAMERA camfile = NJS_CAMERA.Load(Path.Combine(path, file));
						List<byte> ncambytes = new List<byte>();
						NinjaCamera ndata = camfile.NinjaCameraData;
						int ncamaddr = (int)motionImageBase;
						bincamaddrs[camfile.Name] = ncamaddr;
						ncambytes.AddRange(ndata.GetBytes());
						bincameradata.AddRange(ncambytes);
						motionImageBase += (uint)ncambytes.Count;
					}
					foreach (string file in evinfo.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						NJS_MOTION motion = NJS_MOTION.Load(Path.Combine(path, file));
						motion.OptimizeShape();
						motiondatabytes.AddRange(motion.GetBytes(motionImageBase, labels, out uint animend));
						int animaddr = (int)(animend + motionImageBase);
						binanimaddrs[motion.Name] = animaddr;
						if (file.Contains("Camera"))
						{
							NJS_CAMERA camfile = NJS_CAMERA.Load(Path.Combine(path, Path.ChangeExtension(Path.ChangeExtension(file, null) + " Attributes", ".ini")));
							// Sets up NJS_CAMERA pointers
							motiondatabytes.AddRange(ByteConverter.GetBytes(bincamaddrs[camfile.Name]));
							motiondatabytes.AddRange(ByteConverter.GetBytes(binanimaddrs[motion.Name]));
						}
						partcounts.Add(motion.Name, motion.ModelParts);
						if (file.Contains("Camera"))
							motionImageBase += animend + 0x14;
						else
							motionImageBase += animend + 0xC;
					}
					for (int i = 0; i < evinfo.Motions.Count; i++)
					{
						if (evinfo.Motions[i] == null)
							motionfilebytes.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0, 0, 0, 0 });
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
							Directory.CreateDirectory(fileOutputPath);
						motionfilename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
					}
					File.WriteAllBytes(Path.ChangeExtension(Path.ChangeExtension(motionfilename, null) + "motion", ".bin"), motionfilebytes.ToArray());
				}
				else
				{
					uint dcImageBase = imageBase;
					dcImageBase += (uint)modelbytes.Count;
					Dictionary<string, int> camaddrs = new Dictionary<string, int>();
					foreach (string file in evinfo.Files.Where(a => a.Key.EndsWith("Attributes.ini", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						NJS_CAMERA camfile = NJS_CAMERA.Load(Path.Combine(path, file));
						List<byte> ncambytes = new List<byte>();
						NinjaCamera ndata = camfile.NinjaCameraData;
						int ncamaddr = (int)dcImageBase;
						camaddrs[camfile.Name] = ncamaddr;
						ncambytes.AddRange(ndata.GetBytes());
						ncameradata.AddRange(ncambytes);
						dcImageBase += (uint)ncambytes.Count;
					}
					foreach (string file in evinfo.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						NJS_MOTION motion = NJS_MOTION.Load(Path.Combine(path, file));
						motion.OptimizeShape();
						motionbytes.AddRange(motion.GetBytes(dcImageBase, labels, out uint animend));
						int animaddr = (int)(animend + dcImageBase);
						animaddrs[motion.Name] = animaddr;
						if (file.Contains("Camera"))
						{
							NJS_CAMERA camfile = NJS_CAMERA.Load(Path.Combine(path, Path.ChangeExtension(Path.ChangeExtension(file, null) + " Attributes", ".ini")));
							// Sets up NJS_CAMERA pointers
							motionbytes.AddRange(ByteConverter.GetBytes(camaddrs[camfile.Name]));
							motionbytes.AddRange(ByteConverter.GetBytes(animaddrs[motion.Name]));
						}
						if (file.Contains("Camera"))
							dcImageBase += animend + 0x14;
						else
							dcImageBase += animend + 0xC;
					}
					imageBase += (uint)motionbytes.Count;
					imageBase += (uint)ncameradata.Count;
				}
				databytes.AddRange(modelbytes);
				databytes.AddRange(ncameradata);
				databytes.AddRange(motionbytes);
				imageBase += (uint)modelbytes.Count;

				// Texlist information (Necessary for debug mode, always exists)
				uint tlsstart = imageBase;
				NJS_TEXLIST tex = NJS_TEXLIST.Load(Path.Combine(path, "InternalTexlist.satex"));
				List<byte> namebytes = new List<byte>();
				Dictionary<int, int> texaddrs = new Dictionary<int, int>();
				for (int x = 0; x < tex.NumTextures; x++)
				{
					string names = tex.TextureNames[x];
					string texname = names.PadRight(28);
					namebytes.AddRange(Encoding.ASCII.GetBytes(texname));
					namebytes.AddRange(new byte[4]);
					int texaddr = 0x20 * x;
					texaddrs[x] = texaddr;
				}
				int texlistaddr = (int)(imageBase + (tex.NumTextures * 0xC));
				imageBase += (uint)((tex.NumTextures * 0xC) + 8);

				// Texture dimensions (Necessary for debug mode, always exists)
				int sizeptr = (int)imageBase;
				if (evinfo.TexSizes != null)
				{
					TexSizes size = TexSizes.Load(Path.Combine(path, "TextureSizes.ini"));
					List<byte> sizebytes = new List<byte>();
					for (int s = 0; s < size.TextureCount; s++)
					{
						TexSizeArray texsize = size.TextureSize[s];
						sizebytes.AddRange(texsize.GetBytes());
					}
					exdatabytes.AddRange(sizebytes);
					imageBase += (uint)sizebytes.Count;
				}

				// Camera/Particle motion pointer array goes here
				Dictionary<int, int> camarrayptrs = new Dictionary<int, int>();
				Dictionary<int, int> particlearrayptrs = new Dictionary<int, int>();
				List<byte> camarraybytes = new List<byte>();
				List<byte> particlearraybytes = new List<byte>();
				int cammasterptr = (int)imageBase;
				// Scene 0 does not usually contain camera/particle animation data
				for (int i = 1; i < evinfo.Scenes.Count; i++)
				{
					SceneInfo scn = evinfo.Scenes[i];
					// Cameras should always exist... right?
					for (int c = 0; c < scn.CameraMotions.Count; c++)
					{
						if (battle)
							camarraybytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.CameraMotions[c]]));
						else
						{
							if (labels.ContainsKeySafe(scn.CameraMotions[c]))
								camarraybytes.AddRange(ByteConverter.GetBytes(labels[scn.CameraMotions[c]]));
						}
					}
					camarrayptrs[i] = cammasterptr;
					cammasterptr += 4 * scn.CameraMotions.Count;
				}
				exdatabytes.AddRange(camarraybytes);
				imageBase += (uint)camarraybytes.Count;
				// Scene 0 does not usually contain particle animation data
				int particlemasterptr = (int)imageBase;
				for (int i = 1; i < evinfo.Scenes.Count; i++)
				{
					SceneInfo scn = evinfo.Scenes[i];
					if (scn.ParticleMotions.Count != 0)
					{
						for (int p = 0; p < scn.ParticleMotions.Count; p++)
						{
							if (battle)
								particlearraybytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.ParticleMotions[p]]));
							else
							{
								if (labels.ContainsKeySafe(scn.ParticleMotions[p]))
									particlearraybytes.AddRange(ByteConverter.GetBytes(labels[scn.ParticleMotions[p]]));
							}
						}
					}
					else
						particlearraybytes.AddRange(new byte[4]);

					particlearrayptrs[i] = particlemasterptr;
					if (scn.ParticleMotions.Count != 0)
						particlemasterptr += 4 * scn.ParticleMotions.Count;
					else
						particlemasterptr += 4;
				}
				exdatabytes.AddRange(particlearraybytes);
				imageBase += (uint)particlearraybytes.Count;

				// Reflection array
				int reflectptr = (int)imageBase;
				List<byte> evmirrref = new List<byte>();
				Dictionary<int, int> refaddrs = new Dictionary<int, int>();
				if (evinfo.ReflectionInfo != "null")
				{
					ReflectionInfo refl = ReflectionInfo.Load(Path.Combine(path, "ReflectionData.ini"));
					List<byte> refbytes = new List<byte>();
					for (int i = 0; i < refl.Instances; i++)
					{
						int refaddr = 0x30 * i;
						refbytes.AddRange(refl.ReflectData[i].GetBytes());
						refaddrs[i] = refaddr;
					}
					// The game adds the matrix data first, followed by the usage information.
					// Valid reflect data pointers point here.
					reflectptr += (int)refbytes.Count;
					evmirrref.AddRange(refbytes);
					evmirrref.AddRange(ByteConverter.GetBytes(refl.Instances));
					for (int t = 0; t < refl.Instances; t++)
					{
						evmirrref.AddRange(ByteConverter.GetBytes(refl.ReflectData[t].Opacity));
					}
					if (refl.Instances < 32)
					{
						int padding = (32 - (int)refl.Instances) * 4;
						evmirrref.AddRange(new byte[padding]);
					}
					for (int n = 0; n < refl.Instances; n++)
					{
						int matrixaddrs = (int)imageBase + refaddrs[n];
						evmirrref.AddRange(ByteConverter.GetBytes(matrixaddrs));
					}
				}
				else
					evmirrref.AddRange(new byte[0x88]);
				exdatabytes.AddRange(evmirrref);
				imageBase += (uint)evmirrref.Count;

				// Blur model array
				int blurptr = (int)imageBase;
				List<byte> evblurref = new List<byte>();
				if (evinfo.BlurCount != 0)
				{
					for (int b = 0; b < 64; b++)
					{
						if (evinfo.BlurModels[b] != null && labels.ContainsKeySafe(evinfo.BlurModels[b]))
						{
							evblurref.AddRange(ByteConverter.GetBytes(labels[evinfo.BlurModels[b]]));
						}
						else
							evblurref.AddRange(new byte[4]);
					}
				}
				else
					evblurref.AddRange(new byte[0x190]);
				exdatabytes.AddRange(evblurref);
				imageBase += (uint)evblurref.Count;

				// Upgrade override array (AKA "Mech Parts")
				int overrideptr = (int)imageBase;
				List<byte> evovrref = new List<byte>();
				if (evinfo.OverrideCount != 0)
				{
					for (int v = 0; v < 93; v++)
					{
						if (labels.ContainsKeySafe(evinfo.UpgradeOverrideParts[v]))
							evovrref.AddRange(ByteConverter.GetBytes(labels[evinfo.UpgradeOverrideParts[v]]));
						else
							evovrref.AddRange(new byte[4]);
					}
				}
				else
					evovrref.AddRange(new byte[0x24C]);
				exdatabytes.AddRange(evovrref);
				imageBase += (uint)evovrref.Count;

				// Tails' tails
				int tailsptr = (int)imageBase;
				List<byte> evtailref = new List<byte>();
				if (evinfo.TailsTails != "null" && labels.ContainsKeySafe(evinfo.TailsTails))
					evtailref.AddRange(ByteConverter.GetBytes(labels[evinfo.TailsTails]));
				else
					evtailref.AddRange(new byte[4]);
				exdatabytes.AddRange(evtailref);
				imageBase += (uint)evtailref.Count;

				// Upgrade model array
				int upgradeptr = (int)imageBase;
				List<byte> evupref = new List<byte>();
				if (evinfo.UpgradeCount != 0)
				{
					int upmodelnum = 0;
					if (battle)
						upmodelnum = 18;
					else if (dcbeta)
						upmodelnum = 14;
					else
						upmodelnum = 16;
					if (dcbeta)
					{
						if (evinfo.Upgrades[6].UpgradeName != "Knuckles' Shovel Claws")
						{
							UpgradeInfo sclawL = evinfo.Upgrades[6];
							UpgradeInfo sclawR = evinfo.Upgrades[7];
							UpgradeInfo hgloveL = evinfo.Upgrades[8];
							UpgradeInfo hgloveR = evinfo.Upgrades[9];
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
							UpgradeInfo sclaw = evinfo.Upgrades[6];
							UpgradeInfo hglove = evinfo.Upgrades[7];
							UpgradeInfo sun = evinfo.Upgrades[8];
							UpgradeInfo air = evinfo.Upgrades[9];
							UpgradeInfo nail = evinfo.Upgrades[10];
							UpgradeInfo scope = evinfo.Upgrades[11];
							UpgradeInfo boot = evinfo.Upgrades[12];
							UpgradeInfo plate = evinfo.Upgrades[13];

							UpgradeInfo sclawL = new UpgradeInfo();
							UpgradeInfo sclawR = new UpgradeInfo();
							UpgradeInfo hgloveL = new UpgradeInfo();
							UpgradeInfo hgloveR = new UpgradeInfo();

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
						for (int u = 0; u < upmodelnum; u++)
					{
						UpgradeInfo upgrades = evinfo.Upgrades[u];
						if (upgrades.RootNode != "null")
						{
							if (labels.ContainsKeySafe(upgrades.RootNode))
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.RootNode]));
							if (upgrades.AttachNode1 != "null" && labels.ContainsKeySafe(upgrades.AttachNode1))
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.AttachNode1]));
							else
								evupref.AddRange(new byte[4]);
							if (upgrades.Model1 != "null" && labels.ContainsKeySafe(upgrades.Model1))
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.Model1]));
							else
								evupref.AddRange(new byte[4]);
							if (upgrades.AttachNode2 != "null" && labels.ContainsKeySafe(upgrades.AttachNode2))
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.AttachNode2]));
							else
								evupref.AddRange(new byte[4]);
							if (upgrades.Model2 != "null" && labels.ContainsKeySafe(upgrades.Model2))
								evupref.AddRange(ByteConverter.GetBytes(labels[upgrades.Model2]));
							else
								evupref.AddRange(new byte[4]);
						}
						else
							evupref.AddRange(new byte[0x14]);
					}
				}
				else
				{
					if (battle)
						evupref.AddRange(new byte[0x1E0]);
					else
						evupref.AddRange(new byte[0x1B8]);
				}
				exdatabytes.AddRange(evupref);
				imageBase += (uint)evupref.Count;

				// Texture animation data
				int texanimptr = (int)imageBase;
				Dictionary<int, int> uvdatastartaddrs = new Dictionary<int, int>();
				Dictionary<int, int> uvdataaddrs = new Dictionary<int, int>();
				Dictionary<int, int> uvdataarrayaddrs = new Dictionary<int, int>();
				List<byte> evtexanimref = new List<byte>();
				if (evinfo.UsedTexAnims != 0)
				{
					List<byte> uvdatabytes = new List<byte>();
					TexAnimInfo texmaster = evinfo.TextureAnimations;
					for (int q = 0; q < evinfo.TexAnimCount; q++)
					{
						TexAnimMain maindata = texmaster.MainData[q];
						for (int j = 0; j < maindata.TexAnimDataEntries; j++)
						{
							int materialsetup = 0;
							int materialaddr = 0;
							List<int> polysize = new List<int>();
							Dictionary<int, int> polyadd = new Dictionary<int, int>();
							EV_TEXANIM uvdata = EV_TEXANIM.Load(Path.Combine(path, $"Texture Animations\\TexanimInfo {q + 1} Part {j + 1}.ini"));
							// UV data start calculation
							int uvstart = (int)imageBase;

							for (int v = 0; v < uvdata.UVEditEntries; v++)
							{
								int uvaddr = 0;
								int uvaddrminus = 0;
								int uvsize;
								// Material/UV addresses. How the hell are we getting these?
								// Answer: Hacks!
								if (int.TryParse(uvdata.UVEditData[v].UVAddress.Substring(3), System.Globalization.NumberStyles.HexNumber, null, out int uv))
									uvaddr = uv;
								if (v != 0)
								{
									if (int.TryParse(uvdata.UVEditData[v - 1].UVAddress.Substring(3), System.Globalization.NumberStyles.HexNumber, null, out int uvminus))
										uvaddrminus = uvminus;
								}
								if (v != 0)
									uvsize = uvaddr - uvaddrminus;
								else
									uvsize = 0;
								polysize.Add(uvsize);
								if (v == 0)
									polyadd.Add(v, 0);
								else
									polyadd.Add(v, polysize.Sum());
							}
							int firstuv = 0;
							int mat = 0;
							if (int.TryParse(uvdata.UVEditData[0].UVAddress.Substring(3), System.Globalization.NumberStyles.HexNumber, null, out int uvstaddr))
								firstuv = uvstaddr;
							if (int.TryParse(uvdata.MaterialTexAddress.Substring(7), System.Globalization.NumberStyles.HexNumber, null, out int maddr))
								mat = maddr;
							int polysizeminus = polysize.Sum() + 6;
							if (labels.ContainsKeySafe(texmaster.MainData[q].Model))
							{
								uint attachptr = labels[texmaster.MainData[q].Model];
								materialsetup = (int)attachptr - 0x1C - polysizeminus;
								materialaddr = materialsetup - (firstuv - mat);
							}
							// Actual UV edit data is not required for the texture animations to work
							for (int v = 0; v < uvdata.UVEditEntries; v++)
							{
								if (v == 0)
									uvdatabytes.AddRange(ByteConverter.GetBytes(materialsetup));
								else
									uvdatabytes.AddRange(ByteConverter.GetBytes(materialsetup + polyadd[v]));
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
						for (int j = 0; j < maindata.TexAnimDataEntries; j++)
						{
							uvdatabytes.AddRange(ByteConverter.GetBytes(uvdataarrayaddrs[(q * 100) + j]));
							imageBase += 4;
						}
					}
					evtexanimref.AddRange(uvdatabytes);
					imageBase = (uint)(texanimptr + uvdatabytes.Count);
					// Texture animation array (Model Data)
					List<byte> texanimarraybytes = new List<byte>();
					int texanimmasterptr = (int)imageBase;
					for (int h = 0; h < evinfo.TexAnimCount; h++)
					{
						TexAnimMain mainarray = texmaster.MainData[h];
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
						List<byte> gcidbytes = new List<byte>();
						int gcidpointer = (int)imageBase;
						
						for (int u = 0; u < texmaster.TexAnimGCIDCount; u++)
						{
							TexAnimGCIDs gcid = texmaster.TexAnimGCIDs[u];
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
				List<byte> assetbytes = new List<byte>();
				List<byte> bigbytes = new List<byte>();
				List<byte> assetarraybytes = new List<byte>();
				Dictionary<int, int> assetptrs = new Dictionary<int, int>();
				Dictionary<int, int> bigptrs = new Dictionary<int, int>();
				Dictionary<int, int> biganimptrs = new Dictionary<int, int>();
				for (int s = 1; s < evinfo.Scenes.Count; s++)
				{
					SceneInfo scn = evinfo.Scenes[s];
					assetptrs[s] = (int)imageBase;
					for (int a = 0; a < scn.Entities.Count; a++)
					{
						if (battle)
						{
							if (scn.Entities[a].Model != "null" && labels.ContainsKeySafe(scn.Entities[a].Model))
									assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].Model]));
							else
								assetbytes.AddRange(new byte[4]);
							if (scn.Entities[a].Motion != "null")
								assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Entities[a].Motion]));
							else
								assetbytes.AddRange(new byte[4]);
							if (scn.Entities[a].ShapeMotion != "null")
								assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Entities[a].ShapeMotion]));
							else
								assetbytes.AddRange(new byte[4]);
							if (scn.Entities[a].GCModel != "null" && labels.ContainsKeySafe(scn.Entities[a].GCModel))
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].GCModel]));
							else
								assetbytes.AddRange(new byte[4]);
							if (scn.Entities[a].ShadowModel != "null" && labels.ContainsKeySafe(scn.Entities[a].ShadowModel))
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].ShadowModel]));
							else
								assetbytes.AddRange(new byte[4]);
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
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].Model]));
							else
								assetbytes.AddRange(new byte[4]);
							if (scn.Entities[a].Motion != "null" && labels.ContainsKeySafe(scn.Entities[a].Motion))
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].Motion]));
							else
								assetbytes.AddRange(new byte[4]);
							if (scn.Entities[a].ShapeMotion != "null" &&labels.ContainsKeySafe(scn.Entities[a].ShapeMotion))
								assetbytes.AddRange(ByteConverter.GetBytes(labels[scn.Entities[a].ShapeMotion]));
							else
								assetbytes.AddRange(new byte[4]);
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
				SceneInfo scn0 = evinfo.Scenes[0];
				for (int a = 0; a < scn0.Entities.Count; a++)
				{
					if (battle)
					{
						if (scn0.Entities[a].Model != "null" && labels.ContainsKeySafe(scn0.Entities[a].Model))
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].Model]));
						else
							assetbytes.AddRange(new byte[4]);
						if (scn0.Entities[a].Motion != "null")
							assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn0.Entities[a].Motion]));
						else
							assetbytes.AddRange(new byte[4]);
						if (scn0.Entities[a].ShapeMotion != "null")
							assetbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn0.Entities[a].ShapeMotion]));
						else
							assetbytes.AddRange(new byte[4]);
						if (scn0.Entities[a].GCModel != "null" && labels.ContainsKeySafe(scn0.Entities[a].GCModel))
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].GCModel]));
						else
							assetbytes.AddRange(new byte[4]);
						if (scn0.Entities[a].ShadowModel != "null" && labels.ContainsKeySafe(scn0.Entities[a].ShadowModel))
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].ShadowModel]));
						else
							assetbytes.AddRange(new byte[4]);
						assetbytes.AddRange(new byte[4]);
						assetbytes.AddRange(scn0.Entities[a].Position.GetBytes());
						assetbytes.AddRange(ByteConverter.GetBytes(scn0.Entities[a].Flags));
						assetbytes.AddRange(ByteConverter.GetBytes(scn0.Entities[a].Layer));
						imageBase += 0x2C;
					}
					else
					{
						if (scn0.Entities[a].Model != "null" && labels.ContainsKeySafe(scn0.Entities[a].Model))
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].Model]));
						else
							assetbytes.AddRange(new byte[4]);
						if (scn0.Entities[a].Motion != "null" && labels.ContainsKeySafe(scn0.Entities[a].Motion))
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].Motion]));
						else
							assetbytes.AddRange(new byte[4]);
						if (scn0.Entities[a].ShapeMotion != "null" && labels.ContainsKeySafe(scn0.Entities[a].ShapeMotion))
							assetbytes.AddRange(ByteConverter.GetBytes(labels[scn0.Entities[a].ShapeMotion]));
						else
							assetbytes.AddRange(new byte[4]);
						assetbytes.AddRange(new byte[4]);
						assetbytes.AddRange(scn0.Entities[a].Position.GetBytes());
						assetbytes.AddRange(ByteConverter.GetBytes(scn0.Entities[a].Flags));
						imageBase += 0x20;
					}
				}
				// Big cameo data. Scene 0 doesn't have a pointer to Big's data.
				bigptrs[0] = 0;
				biganimptrs[0] = 0;
				List<byte> biganimbytes = new List<byte>();
				// Animation array pointers
				for (int s = 1; s < evinfo.Scenes.Count; s++)
				{
					SceneInfo scn = evinfo.Scenes[s];
					biganimptrs[s] = (int)imageBase + ((s - 1) * 8);
					for (int t = 0; t < scn.Big.Motions.Count; t++)
					{
						if (scn.Big.Motions[t] != null)
						{
							if (scn.Big.Motions[t][0] != null)
							{
								if (battle)
									biganimbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Big.Motions[t][0]]));
								else
								{
									if (labels.ContainsKeySafe(scn.Big.Motions[t][0]))
										biganimbytes.AddRange(ByteConverter.GetBytes(labels[scn.Big.Motions[t][0]]));
								}
							}
							else
								biganimbytes.AddRange(new byte[4]);
							if (scn.Big.Motions[t][1] != null)
							{
								if (battle)
									biganimbytes.AddRange(ByteConverter.GetBytes(motionIDs[scn.Big.Motions[t][1]]));
								else
								{
									if (labels.ContainsKeySafe(scn.Big.Motions[t][1]))
										biganimbytes.AddRange(ByteConverter.GetBytes(labels[scn.Big.Motions[t][1]]));
								}
							}
							else
								biganimbytes.AddRange(new byte[4]);
						}
						else
							biganimbytes.AddRange(new byte[8]);
					}
				}
				imageBase += (uint)biganimbytes.Count;
				for (int s = 1; s < evinfo.Scenes.Count; s++)
				{
					SceneInfo scn = evinfo.Scenes[s];
					bigptrs[s] = (int)imageBase + ((s - 1) * 0x10);
					if (scn.Big.Model != "null")
					{
						if (labels.ContainsKeySafe(scn.Big.Model))
							bigbytes.AddRange(ByteConverter.GetBytes(labels[scn.Big.Model]));
						if (scn.Big.Motions != null)
						{
							bigbytes.AddRange(ByteConverter.GetBytes(biganimptrs[s]));
							bigbytes.AddRange(ByteConverter.GetBytes(scn.Big.Motions.Count));
						}
						else
							bigbytes.AddRange(new byte[8]);
						bigbytes.AddRange(new byte[4]);
					}
					else
						bigbytes.AddRange(new byte[0x10]);
				}
				exdatabytes.AddRange(assetbytes);
				exdatabytes.AddRange(biganimbytes);
				exdatabytes.AddRange(bigbytes);
				imageBase += (uint)bigbytes.Count;

				// Master Asset Array
				int assetarraymaster = (int)imageBase;
				List<byte> masterbytes = new List<byte>();
				for (int m = 0; m < evinfo.Scenes.Count; m++)
				{
					SceneInfo scn = evinfo.Scenes[m];
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
						masterbytes.AddRange(new byte[0x14]);
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
					evfile.AddRange(ByteConverter.GetBytes(texanimptr));
				else
					evfile.AddRange(new byte[4]);
				// Battle Shadows
				if (battle)
				{
					evfile.AddRange(ByteConverter.GetBytes(Convert.ToInt32(evinfo.GCShadowMaps)));
					if (evinfo.GCShadowMaps == true)
						evfile.AddRange(new byte[0x14]);
				}
				// All model/animation data goes here
				evfile.AddRange(databytes);

				// Non-model data goes here. It was split from databytes because of the texlist data
				for (int n = 0; n < tex.NumTextures; n++)
				{
					int tlsaddrs = (int)imageBase + texaddrs[n];
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
						Directory.CreateDirectory(fileOutputPath);
					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(evfile.ToArray(), filename);
					if (!File.Exists(filename))
						File.Create(filename);
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
			string dir = Environment.CurrentDirectory;
			try
			{
				filename = Path.GetFullPath(filename);
				if (Directory.Exists(filename))
					filename += ".prs";
				Environment.CurrentDirectory = Path.GetDirectoryName(filename);
				string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new JsonSerializer();
				EventTexlistData evinfo;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new JsonTextReader(tr))
					evinfo = js.Deserialize<EventTexlistData>(jtr);
				uint gamekey;
				if (isBigEndian.HasValue)
				{
					ByteConverter.BigEndian = isBigEndian.Value;
					if (isBigEndian.Value == true)
						gamekey = 0;
					else
						gamekey = 0xCBC0000;
				}
				else
				{
					ByteConverter.BigEndian = evinfo.BigEndian;
					if (evinfo.BigEndian == true)
						gamekey = 0;
					else
						gamekey = 0xCBC0000;
				}
				List<byte> evfile = new List<byte>();
				List<byte> databytes = new List<byte>();
				uint imageBase = gamekey + 4;
				uint tlsstart = gamekey + 4;
				NJS_TEXLIST tex = NJS_TEXLIST.Load(Path.Combine(Path.GetFileNameWithoutExtension(filename), "ExternalTexlist.satex"));
				List<byte> namebytes = new List<byte>();
				Dictionary<int, int> texaddrs = new Dictionary<int, int>();
				for (int t = 0; t < tex.NumTextures; t++)
				{
					string names = tex.TextureNames[t];
					string texname = names.PadRight(28);
					namebytes.AddRange(Encoding.ASCII.GetBytes(texname));
					namebytes.AddRange(new byte[4]);
					int texaddr = 0x20 * t;
					texaddrs[t] = texaddr;
				}
				databytes.AddRange(namebytes);
				int texlistaddr = (int)(imageBase + (tex.NumTextures * 0xC));
				imageBase += (uint)((tex.NumTextures * 0xC) + 8);
				evfile.AddRange(ByteConverter.GetBytes(texlistaddr));
				for (int n = 0; n < tex.NumTextures; n++)
				{
					int tlsaddrs = (int)imageBase + texaddrs[n];
					evfile.AddRange(ByteConverter.GetBytes(tlsaddrs));
					evfile.AddRange(new byte[8]);
				}
				evfile.AddRange(ByteConverter.GetBytes(tlsstart));
				evfile.AddRange(ByteConverter.GetBytes(tex.NumTextures));
				evfile.AddRange(databytes);
				if (fileOutputPath.Length != 0)
				{
					if (!Directory.Exists(fileOutputPath))
						Directory.CreateDirectory(fileOutputPath);
					filename = Path.Combine(fileOutputPath, Path.GetFileName(filename));
				}
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				{
					FraGag.Compression.Prs.Compress(evfile.ToArray(), filename);
					if (!File.Exists(filename))
						File.Create(filename);
				}
				else
					File.WriteAllBytes(filename, evfile.ToArray());
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
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					name = $"object_{ptr3:X8}";
					if (!nodenames.Contains(name))
					{
						NJS_OBJECT obj = new NJS_OBJECT(fc, ptr3, key, ModelFormat.Chunk, null);
						name = obj.Name;
						List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
						foreach (string s in names)
							if (modelfiles.ContainsKey(s))
								modelfiles.Remove(s);
						nodenames.AddRange(names);
						modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.Chunk, meta));
					}
				}
				return name;
			}

		private static string GetGCModel(byte[] fc, int address, uint key, string fn, string meta = null)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"object_{ptr3:X8}";
				if (!nodenames.Contains(name))
				{
					NJS_OBJECT obj = new NJS_OBJECT(fc, ptr3, key, ModelFormat.GC, null);
					name = obj.Name;
					List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
					foreach (string s in names)
						if (modelfiles.ContainsKey(s))
							modelfiles.Remove(s);
					nodenames.AddRange(names);
					modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.GC, meta));
				}
			}
			return name;
		}

		private static string GetMotion(byte[] fc, int address, uint key, string fn, List<NJS_MOTION> motions, int cnt, string meta = null)
		{
			NJS_MOTION mtn = null;
			if (motions != null)
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					mtn = new NJS_MOTION(fc, ptr3, key, cnt, shortrot: false, shortcheck: false);
					mtn.ShortRot = false;
					mtn.Description = meta;
					mtn.OptimizeShape();
				}
			}
			if (mtn == null) return null;
			if (!motionfiles.ContainsKey(mtn.Name) || motionfiles[mtn.Name].Filename == null)
				motionfiles[mtn.Name] = new MotionInfo(fn, mtn);
			return mtn.Name;
		}

		private static string GetTexlist(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texlist_{ptr3:X8}";
			}
			return name;
		}

		private static string GetTexSizes(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texsizes_{ptr3:X8}";
			}
			return name;
		}

		private static string GetReflectData(byte[] fc, int address, uint key, string fn)
		{
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
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
				ncam = ncams[ByteConverter.ToInt32(fc, address + 0xC)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new NJS_CAMERA(fc, ptr3 + 0xC, key);
				}
			}
			if (ncam == null) return null;
			if (!camarrayfiles.ContainsKey(ncam.Name) || camarrayfiles[ncam.Name].Filename == null)
				camarrayfiles[ncam.Name] = new CameraInfo(fn, ncam);
			return ncam.Name;
		}

		private static string GetGCCamData(byte[] fc, int address, uint key, string fn, List<NJS_CAMERA> ncams)
		{
			NJS_CAMERA ncam = null;
			if (ncams != null)
				ncam = ncams[ByteConverter.ToInt32(fc, address)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
				{
					ncam = new NJS_CAMERA(fc, ptr3, key);
				}
			}
			if (ncam == null) return null;
			if (!camarrayfiles.ContainsKey(ncam.Name) || camarrayfiles[ncam.Name].Filename == null)
				camarrayfiles[ncam.Name] = new CameraInfo(fn, ncam);
			return ncam.Name;
		}

		private static string GetTexanim(byte[] fc, int address, uint key, string fn)
		{
			EV_TEXANIM texanim = null;
			string name = null;
			int ptr3 = fc.GetPointer(address, key);
			if (ptr3 != 0)
			{
				name = $"texanim_{ptr3:X8}";
				texanim = new EV_TEXANIM(fc, ptr3, key);
			}
			if (texanim == null) return null;
			if (!texanimfiles.ContainsKey(name) || texanimfiles[name].Filename == null)
				texanimfiles[name] = new TexAnimFileInfo(fn, texanim);
			return name;
		}

		//Read Functions
		private static List<NJS_MOTION> ReadMotionFile(string filename)
		{
			List<NJS_MOTION> motions = new List<NJS_MOTION>();
			List<NJS_CAMERA> cam = new List<NJS_CAMERA>();
			NJS_MOTION mtn;
			byte[] fc = File.ReadAllBytes(filename);
			int addr = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				int ptr = ByteConverter.ToInt32(fc, addr);
				int nummdl = ByteConverter.ToInt32(fc, addr + 4);
				int camcheck1 = fc.GetPointer(addr, 0);
				uint camcheck2 = ByteConverter.ToUInt32(fc, camcheck1 + 8);
				if (ptr == -1)
					motions.Add(null);
				else
				{
					mtn = new NJS_MOTION(fc, ptr, 0, nummdl, shortrot: false, shortcheck: false);
					mtn.OptimizeShape();
					motions.Add(mtn);
					if (nummdl == 1 && camcheck2 == 0x1C10004)
					{
						cam.Add(new NJS_CAMERA(fc, ptr + 0xC, 0));
					}
				}
				addr += 8;
			}
			return motions;
		}

		private static List<NJS_CAMERA> ReadMotionFileCams(string filename)
		{
			List<NJS_CAMERA> cam = new List<NJS_CAMERA>();
			byte[] fc = File.ReadAllBytes(filename);
			int addr = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				int ptr = ByteConverter.ToInt32(fc, addr);
				int nummdl = ByteConverter.ToInt32(fc, addr + 4);
				int camcheck1 = fc.GetPointer(addr, 0);
				uint camcheck2 = ByteConverter.ToUInt32(fc, camcheck1 + 8);
				if (ptr == -1)
					cam.Add(null);
				else
				{
					if (nummdl == 1 && camcheck2 == 0x1C10004)
					{
						cam.Add(new NJS_CAMERA(fc, ptr + 0xC, 0));
					}
					else
						cam.Add(null);
			}
			addr += 8;
			}
			return cam;
		}

		private static Dictionary<int, string> ReadMotionFileNames(string filename)
		{
			Dictionary<int, string> motions = new Dictionary<int, string>();
			List<NJS_CAMERA> cam = new List<NJS_CAMERA>();
			string mtn;
			byte[] fc = File.ReadAllBytes(filename);
			int addr = 0;
			int i = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				int ptr = ByteConverter.ToInt32(fc, addr);
				int nummdl = ByteConverter.ToInt32(fc, addr + 4);
				int camcheck1 = fc.GetPointer(addr, 0);
				uint camcheck2 = ByteConverter.ToUInt32(fc, camcheck1 + 8);
				if (ptr == -1)
					motions.Add(i, null);
				else
				{
					mtn = new NJS_MOTION(fc, ptr, 0, nummdl, shortrot: false, shortcheck: false).Name;
					motions.Add(i, mtn);
					if (nummdl == 1 && camcheck2 == 0x1C10004)
					{
						cam.Add(new NJS_CAMERA(fc, ptr + 0xC, 0));
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
		public List<string> Motions { get; set; } = new List<string>();
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
	public class EventMetaInfo
	{
		[IniName("type")]
		public string Type { get; set; }
		[IniName("filename")]
		public string Filename { get; set; }
		[IniName("meta")]
		public string Metadata { get; set; }
	}

	public class EventIniData
	{
		public string Name { get; set; }
		[JsonIgnore]
		public Game Game { get; set; }
		[JsonProperty(PropertyName = "Game")]
		public string GameString
		{
			get { return Game.ToString(); }
			set { Game = (Game)Enum.Parse(typeof(Game), value); }
		}
		public bool BigEndian { get; set; }
		public bool BattleFormat { get; set; }
		public bool DCBeta { get; set; }
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
		public int UpgradeCount { get; set; }
		public List<UpgradeInfo> Upgrades { get; set; } = new List<UpgradeInfo>();
		public int BlurCount { get; set; }
		public Dictionary<int, string> BlurModels { get; set; } = new Dictionary<int, string>();
		public int OverrideCount { get; set; }
		public Dictionary<int, string> UpgradeOverrideParts { get; set; } = new Dictionary<int, string>();
		public string TailsTails { get; set; }
		public string Texlist { get; set; }
		public string TexSizes { get; set; }
		public bool GCShadowMaps { get; set; }
		public List<SceneInfo> Scenes { get; set; } = new List<SceneInfo>();
		public int TexAnimCount { get; set; }
		public int UsedTexAnims { get; set; }
		public TexAnimInfo TextureAnimations { get; set; }
		public string ReflectionInfo { get; set; }
		public Dictionary<int, string> Motions { get; set; } = new Dictionary<int, string>();
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
		public List<EntityInfo> Entities { get; set; } = new List<EntityInfo>();
		public List<string> CameraMotions { get; set; } = new List<string>();
		public List<string> NinjaCameras { get; set; } = new List<string>();
		public List<string> ParticleMotions { get; set; } = new List<string>();
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
		public List<string[]> Motions { get; set; } = new List<string[]>();
		public int Unknown { get; set; }
	}

	[Serializable]
	public class TexSizes
	{
		public string Name { get; set; }
		public int TextureCount { get; set; }
		public List<TexSizeArray> TextureSize { get; set; } = new List<TexSizeArray>();

		public TexSizes() { }

		public TexSizes(byte[] file, int address, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "texsizes_" + address.ToString("X8");
			TextureSize = new List<TexSizeArray>();
			int numtex = ByteConverter.ToInt32(file, address - 4);
			TextureCount = numtex;
			for (int i = 0; i < numtex; i++)
			{
				TextureSize.Add(new TexSizeArray(file, address));
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

		public static int Size { get { return 0x4; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(H));
			result.AddRange(ByteConverter.GetBytes(V));
			result.Align(4);
			return result.ToArray();
		}
	}
	public class TexAnimInfo
	{
		public List<TexAnimMain> MainData { get; set; } = new List<TexAnimMain>();
		public List<TexAnimGCIDs> TexAnimGCIDs { get; set; } = new List<TexAnimGCIDs>();
		public int TexAnimGCIDCount { get; set; }
		public int TexID { get; set; }
		public int TexLoopNumber { get; set; }
		public int TexAnimMainDataEntries { get; set; }

	}
	public class TexAnimGCIDs
	{
		public int TexID { get; set; }
		public int TexLoopNumber { get; set; }

		public static int Size { get { return 0x8; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
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
		public List<string> TexAnimPointers { get; set; } = new List<string>();
	}

	public class ReflectionInfo
	{
		public string Name { get; set; }
		public int Instances { get; set; }
		public List<ReflectionMatrixData> ReflectData { get; set; } = new List<ReflectionMatrixData>();

		public ReflectionInfo() { }

		public ReflectionInfo(byte[] file, int address, uint imageBase, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "reflectdata_" + address.ToString("X8");
			ReflectData = new List<ReflectionMatrixData>();
			int reflectcount = ByteConverter.ToInt32(file, address);
			Instances = reflectcount;
			for (int i = 0; i < reflectcount; i++)
			{
				uint reflectOffset = ByteConverter.ToUInt32(file, address + 0x84);
				int reflectAddr = (int)(reflectOffset - imageBase);
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
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "matrix_" + address.ToString("X8");
			// placeholder
			Opacity = 0;
			Data1 = new Vertex(file, address);
			Data2 = new Vertex(file, address + 0xC);
			Data3 = new Vertex(file, address + 0x18);
			Data4 = new Vertex(file, address + 0x24);
		}

		public static int Size { get { return 0x30; } }

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
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
		public string TexanimArrayName { get; set; }
		public List<UVData> UVEditData { get; set; }

		public EV_TEXANIM() { }
		public EV_TEXANIM(byte[] file, int address, uint imageBase, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "texanim_" + address.ToString("X8");
			TextureID = ByteConverter.ToInt32(file, address);
			uint MaterialOffset = ByteConverter.ToUInt32(file, address + 4);
			int MaterialAddr = (int)(MaterialOffset - imageBase);
			if (labels != null && labels.ContainsKey(MaterialAddr))
				MaterialTexAddress = labels[MaterialAddr];
			else
				MaterialTexAddress = "mattex_" + MaterialAddr.ToString("X8");
			UVEditEntries = ByteConverter.ToInt32(file, address + 8);
			uint TexanimArrayOffset = ByteConverter.ToUInt32(file, address + 0xC);
			int TexanimArrayAddr = (int)(TexanimArrayOffset - imageBase);
			if (labels != null && labels.ContainsKey(TexanimArrayAddr))
				TexanimArrayName = labels[TexanimArrayAddr];
			else
				TexanimArrayName = "uvanim_" + TexanimArrayAddr.ToString("X8");
			UVEditData = new List<UVData>();
			if (TexanimArrayOffset != 0)
			{
				for (int i = 0; i < UVEditEntries; i++)
				{
					UVEditData.Add(new UVData(file, TexanimArrayAddr, imageBase));
					TexanimArrayAddr += 0x8;
				}
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
			uint uvaddr = ByteConverter.ToUInt32(file, address);
			int ptr = (int)(uvaddr - imageBase);
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
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "camdata_" + address.ToString("X8");
			int ncamaddr = ByteConverter.ToInt32(file, address);
			int animaddr = ByteConverter.ToInt32(file, address + 4);
			int ncamptr = (int)(ncamaddr - imageBase);
			int animptr = (int)(animaddr - imageBase);
			if (labels != null && labels.ContainsKey(ncamptr))
				NinjaCameraName = labels[ncamptr];
			else
				NinjaCameraName = "ninjacam_" + ncamptr.ToString("X8");
			NinjaCameraData = new NinjaCamera(file, ncamptr);
			if (labels != null && labels.ContainsKey(animptr))
				CameraAnimation = labels[animptr];
			else
				CameraAnimation = "animation_" + animptr.ToString("X8");
		}

		public NJS_CAMERA() { }

		public static NJS_CAMERA Load(string filename) => IniSerializer.Deserialize<NJS_CAMERA>(filename);

		public static int Size { get { return 0x40; } }

		public byte[] GetBytes(uint imageBase, Dictionary<string, uint> labels)
		{
			List<byte> result = new List<byte>(Size);
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

	public class EventTexlistData
	{
		public string Name { get; set; }
		[JsonIgnore]
		public Game Game { get; set; }
		[JsonProperty(PropertyName = "Game")]
		public string GameString
		{
			get { return Game.ToString(); }
			set { Game = (Game)Enum.Parse(typeof(Game), value); }
		}
		public bool BigEndian { get; set; }
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
		public string Texlist { get; set; }
	}
}
