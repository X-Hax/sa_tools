﻿using FraGag.Compression;
using Newtonsoft.Json;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SplitTools.SAArc
{
	public static class sa2Event
	{
		static readonly string[] upgradenames = { "Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claw L", "Knuckles' Shovel Claw R", "Knuckles' Hammer Glove L", "Knuckles' Hammer Glove R", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates", "Mech Tails' Window", "Mech Eggman's Window" };
		static readonly string[] upgradebetanames = { "Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claws", "Knuckles' Hammer Gloves", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates" };
		static List<string> nodenames = new List<string>();
		static Dictionary<string, ModelInfo> modelfiles = new Dictionary<string, ModelInfo>();
		static Dictionary<string, MotionInfo> motionfiles = new Dictionary<string, MotionInfo>();

		public static void Split(string filename, string outputPath)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
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
				//Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
				EventIniData ini = new EventIniData() { Name = Path.GetFileNameWithoutExtension(evfilename) };
				if (outputPath.Length != 0)
				{
					if (!Directory.Exists(outputPath))
						Directory.CreateDirectory(outputPath);
					Environment.CurrentDirectory = outputPath;
				}
				else
					Environment.CurrentDirectory = Path.GetDirectoryName(evfilename);
					Directory.CreateDirectory(Path.GetFileNameWithoutExtension(evfilename));
				//string path = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(evfilename)), Path.GetFileNameWithoutExtension(evfilename))).FullName;
				uint key;
				List<NJS_MOTION> motions = null;
				bool battle;
				bool dcbeta;
				if (fc[0] == 0x81)
				{
					if (fc[0x2B] <= 0x01 && fc[0x2A] == 0)
					{
						Console.WriteLine("File is in GC/PC format.");
						ByteConverter.BigEndian = true;
						key = 0x8125FE60;
						ini.Game = Game.SA2B;
						battle = true;
						dcbeta = false;
						motions = ReadMotionFile(Path.ChangeExtension(evfilename, null) + "motion.bin");
						ini.Motions = motions.Select(a => a?.Name).ToList();
						foreach (var mtn in motions.Where(a => a != null))
							motionfiles[mtn.Name] = new MotionInfo(null, mtn);
					}
					else
					{
						Console.WriteLine("File is in GC/PC Beta format.");
						ByteConverter.BigEndian = true;
						key = 0x812FFE60;
						ini.Game = Game.SA2B;
						battle = false;
						dcbeta = false;
					}
				}
				else
				{
					if ((fc[37] == 0x25 && (fc[36] == 0x13 || fc[36] == 0x11)) || (fc[38] == 0x22) || ((fc[36] == 0) && ((fc[1] == 0xFE) || (fc[1] == 0xF2) || ((fc[1] == 0x27) && fc[2] == 0x9F))))
					{
						Console.WriteLine("File is in DC Beta format.");
						ByteConverter.BigEndian = false;
						key = 0xC600000;
						ini.Game = Game.SA2;
						battle = false;
						dcbeta = true;
					}
					else
					{
						Console.WriteLine("File is in DC format.");
						ByteConverter.BigEndian = false;
						key = 0xC600000;
						ini.Game = Game.SA2;
						battle = false;
						dcbeta = false;
					}

				}
				int ptr = fc.GetPointer(0x20, key);
				if (ptr != 0)
				{
					UpgradeInfo info = new UpgradeInfo();
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
							info.RootNode = GetModel(fc, ptr, key, $"{chnam} Root.sa2mdl");
							if (info.RootNode != null)
							{
								int ptr2 = fc.GetPointer(ptr + 4, key);
								if (ptr2 != 0)
									info.AttachNode1 = $"object_{ptr2:X8}";
								int ptr3 = fc.GetPointer(ptr + 8, key);
								if (ptr3 != 0)
								{
									info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl");
									Console.WriteLine($"Event contains {upnam}.");
									upcount++;
								}
								ptr2 = fc.GetPointer(ptr + 0xC, key);
								if (ptr2 != 0)
									info.AttachNode2 = $"object_{ptr2:X8}";
								ptr3 = fc.GetPointer(ptr + 0x10, key);
								if (ptr3 != 0)
									info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl");
							}
							ini.Upgrades.Add(info);
							ptr += 0x14;
						}
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
							info.RootNode = GetModel(fc, ptr, key, $"{chnam} Root.sa2mdl");
							if (info.RootNode != null)
							{
								int ptr2 = fc.GetPointer(ptr + 4, key);
								if (ptr2 != 0)
									info.AttachNode1 = $"object_{ptr2:X8}";
								int ptr3 = fc.GetPointer(ptr + 8, key);
								if (ptr3 != 0)
								{
									info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl");
									Console.WriteLine($"Event contains {upnam}.");
									upcount++;
								}
								ptr2 = fc.GetPointer(ptr + 0xC, key);
								if (ptr2 != 0)
									info.AttachNode2 = $"object_{ptr2:X8}";
								ptr3 = fc.GetPointer(ptr + 0x10, key);
								if (ptr3 != 0)
									info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl");
							}
							ini.Upgrades.Add(info);
							ptr += 0x14;
						}
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
							namecount++;
						name = $"object_{ptr:X8}";
						if (name != null)
							ini.MechParts.Add(i, name);
						ptr += 4;
					}
					Console.WriteLine("Event contains {0} mech par{1}.", namecount == 0 ? "no" : $"{namecount}", namecount == 1 ? "t" : "ts");
				}
				int gcnt = ByteConverter.ToInt32(fc, 8);
				ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					Console.WriteLine("Event contains {0} scen{1}.", gcnt + 1, gcnt + 1 == 1 ? "e" : "es");
					for (int gn = 0; gn <= gcnt; gn++)
					{
						Directory.CreateDirectory(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), $"Scene {gn + 1}"));
						SceneInfo scn = new SceneInfo();
						int ptr2 = fc.GetPointer(ptr, key);
						int ecnt = ByteConverter.ToInt32(fc, ptr + 4);
						if (ptr2 != 0)
						{
							Console.WriteLine("Scene {0} contains {1} entit{2}.", gn + 1, ecnt, ecnt == 1 ? "y" : "ies");
							for (int en = 0; en < ecnt; en++)
							{
								EntityInfo ent = new EntityInfo();
								ent.Model = GetModel(fc, ptr2, key, $"Scene {gn + 1}\\Entity {en + 1} Model.sa2mdl");
								if (ent.Model != null)
								{
									ent.Motion = GetMotion(fc, ptr2 + 4, key, $"Scene {gn + 1}\\Entity {en + 1} Motion.saanim", motions, modelfiles[ent.Model].Model.CountAnimated());
									if (ent.Motion != null)
										modelfiles[ent.Model].Motions.Add(motionfiles[ent.Motion].Filename);
									ent.ShapeMotion = GetMotion(fc, ptr2 + 8, key, $"Scene {gn + 1}\\Entity {en + 1} Shape Motion.saanim", motions, modelfiles[ent.Model].Model.CountMorph());
									if (ent.ShapeMotion != null)
										modelfiles[ent.Model].Motions.Add(motionfiles[ent.ShapeMotion].Filename);
								}
								if (battle)
								{
									ent.GCModel = GetGCModel(fc, ptr2 + 12, key, $"Scene {gn + 1}\\Entity {en + 1} GC Model.sa2bmdl");
									ent.ShadowModel = GetModel(fc, ptr2 + 16, key, $"Scene {gn + 1}\\Entity {en + 1} Shadow Model.sa2mdl");
									ent.Position = new Vertex(fc, ptr2 + 24);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 36);
									ent.Layer = ByteConverter.ToUInt32(fc, ptr2 + 40);
								}
								else
								{
									ent.Position = new Vertex(fc, ptr2 + 16);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 28);
								}
								scn.Entities.Add(ent);
								ptr2 += battle ? 0x2C : 0x20;
							}
						}
						else
							Console.WriteLine("Scene {0} contains no entities.", gn + 1);
						ptr2 = fc.GetPointer(ptr + 8, key);
						if (ptr2 != 0)
						{
							int cnt = ByteConverter.ToInt32(fc, ptr + 12);
							for (int i = 0; i < cnt; i++)
							{
								scn.CameraMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn + 1}\\Camera Motion {i + 1}.saanim", motions, 1));
								ptr2 += sizeof(int);
							}
						}
						ptr2 = fc.GetPointer(ptr + 0x18, key);
						if (ptr2 != 0)
						{
							BigInfo big = new BigInfo();
							big.Model = GetModel(fc, ptr2, key, $"Scene {gn + 1}\\Big Model.sa2mdl");
							if (big.Model != null)
							{
								Console.WriteLine("Scene {0} contains Big the Cat cameo.", gn + 1);
								int anicnt = modelfiles[big.Model].Model.CountAnimated();
								int ptr3 = fc.GetPointer(ptr2 + 4, key);
								if (ptr3 != 0)
								{
									int cnt = ByteConverter.ToInt32(fc, ptr2 + 8);
									for (int i = 0; i < cnt; i++)
									{
										big.Motions.Add(new string[] { GetMotion(fc, ptr3, key, $"Scene {gn + 1}\\Big Motion {i + 1}a.saanim", motions, anicnt), GetMotion(fc, ptr3 + 4, key, $"Scene {gn + 1}\\Big Motion {i + 1}b.saanim", motions, anicnt) });
										ptr3 += 8;
									}
								}
							}
							else
								Console.WriteLine("Scene {0} does not contain Big the Cat cameo.", gn + 1);
							big.Unknown = ByteConverter.ToInt32(fc, ptr2 + 12);
							scn.Big = big;
						}
						scn.FrameCount = ByteConverter.ToInt32(fc, ptr + 28);
						ini.Scenes.Add(scn);
						ptr += 0x20;
					}
				}
				else
					Console.WriteLine("Event contains no scenes.");
				ptr = fc.GetPointer(0x1C, key);
				if (ptr != 0)
				{
					ini.TailsTails = GetModel(fc, ptr, key, $"Tails' tails.sa2mdl");
					int ptr2 = fc.GetPointer(ptr, key);
					if (ptr2 == 0)
						Console.WriteLine("Event does not contain Tails' tails.");
					else
						Console.WriteLine("Event contains Tails' tails.");
				}
				ptr = fc.GetPointer(4, key);
				if (ptr != 0)
				{
					int ptr2 = fc.GetPointer(ptr, key);
					if (ptr2 == 0)
						Console.WriteLine("Event does not contain an internal texture list.");
					else
						Console.WriteLine("Event contains an internal texture list.");
				}
				ptr = fc.GetPointer(0xC, key);
				if (ptr != 0)
				{
					ushort start = (ushort)fc.GetPointer(ptr, 0);
					if (start > 0)
						Console.WriteLine("Event contains internal texture sizes.");
					else
						Console.WriteLine("Event does not contain internal texture sizes.");
				}
				ptr = fc.GetPointer(0x10, key);
				if (ptr != 0)
				{
					int start = fc.GetPointer(ptr, 0);
					if (start > 0)
						Console.WriteLine("Event contains character reflection info.");
					else
						Console.WriteLine("Event does not contain character reflection info.");
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
							namecount++;
						name = $"object_{ptr:X8}";
						if (name != null)
							ini.UnknownModelData.Add(i, name);
						ptr += 4;
					}
					Console.WriteLine("Event contains {0} unknown model pointe{1}.", namecount == 0 ? "no" : $"{namecount}", namecount == 1 ? "r" : "rs");
				}
				ptr = fc.GetPointer(0x24, key);
				if (ptr == 0 || dcbeta && ((fc[37] == 0x25) || (fc[38] == 0x22)))
					Console.WriteLine("Event does not contain texture animation data.");
				else
					Console.WriteLine("Event contains texture animation data.");
				if (battle && fc[0x2B] == 0)
					Console.WriteLine("Event does not use GC shadow maps.");
				if (battle && fc[0x2B] == 1)
					Console.WriteLine("Event uses GC shadow maps.");
				foreach (var item in motionfiles.Values)
				{
					string fn = item.Filename ?? $"Unknown Motion {motions.IndexOf(item.Motion)}.saanim";
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in modelfiles.Values)
				{
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, item.Format);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				JsonSerializer js = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
				using (var tw = File.CreateText(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), Path.ChangeExtension(Path.GetFileName(evfilename), ".json"))))
					js.Serialize(tw, ini);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}

		public static void Build(string filename)
		{
			nodenames.Clear();
			modelfiles.Clear();
			motionfiles.Clear();
			string dir = Environment.CurrentDirectory;
			try
			{
				byte[] fc;
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(filename);
				else
					fc = File.ReadAllBytes(filename);
				string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
				JsonSerializer js = new JsonSerializer();
				EventIniData ini;
				using (TextReader tr = File.OpenText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
				using (JsonTextReader jtr = new JsonTextReader(tr))
					ini = js.Deserialize<EventIniData>(jtr);
				uint key;
				bool battle = ini.Game == Game.SA2B;
				bool battlebeta = ini.Game == Game.SA2B;
				bool dcbeta = ini.Game == Game.SA2;
				if (fc[0] == 0x81)
				{
					if (fc[0x2B] <= 1 && fc[0x2A] == 0)
					{
						ByteConverter.BigEndian = true;
						key = 0x8125FE60;
						battle = true;
					}
					else
					{
						ByteConverter.BigEndian = true;
						key = 0x812FFE60;
						battlebeta = true;
					}
				}
				else
				{
					if ((fc[37] == 0x25) || (fc[38] == 0x22) || ((fc[36] == 0) && ((fc[1] == 0xFE) || (fc[1] == 0xF2) || ((fc[1] == 0x27) && fc[2] == 0x9F))))
					{
						ByteConverter.BigEndian = false;
						key = 0xC600000;
						dcbeta = true;
					}
					else
					{
						ByteConverter.BigEndian = false;
						key = 0xC600000;
					}
				}
				List<byte> modelbytes = new List<byte>(fc);
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".sa2mdl", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
					modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, new List<uint>(), out uint _));
				if (battle)
				{
					foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".sa2bmdl", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
						modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, new List<uint>(), out uint _));
					List<byte> motionbytes = new List<byte>(new byte[(ini.Motions.Count + 1) * 8]);
					Dictionary<string, int> partcounts = new Dictionary<string, int>(ini.Motions.Count);
					foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						NJS_MOTION motion = NJS_MOTION.Load(Path.Combine(path, file));
						motionbytes.AddRange(motion.GetBytes((uint)motionbytes.Count, labels, out uint _));
						partcounts.Add(motion.Name, motion.ModelParts);
					}
					byte[] mfc = motionbytes.ToArray();
					for (int i = 0; i < ini.Motions.Count; i++)
					{
						if (ini.Motions[i] == null)
							new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }.CopyTo(mfc, i * 8);
						else
						{
							ByteConverter.GetBytes(labels[ini.Motions[i]]).CopyTo(mfc, i * 8);
							ByteConverter.GetBytes(partcounts[ini.Motions[i]]).CopyTo(mfc, i * 8 + 4);
						}
					}
					File.WriteAllBytes(Path.ChangeExtension(Path.ChangeExtension(filename, null) + "motion", ".bin"), mfc);
				}
				else
					foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
						modelbytes.AddRange(NJS_MOTION.Load(Path.Combine(path, file)).GetBytes((uint)(key + modelbytes.Count), labels, out uint _));
				fc = modelbytes.ToArray();
				int ptr = fc.GetPointer(0x20, key);
				if (ptr != 0)
					if (!dcbeta)
					{
						for (int i = 0; i < (battle ? 18 : 16); i++)
						{
							UpgradeInfo info = ini.Upgrades[i];
							if (info.RootNode != null)
							{
								if (labels.ContainsKeySafe(info.RootNode))
									ByteConverter.GetBytes(labels[info.RootNode]).CopyTo(fc, ptr);
								if (labels.ContainsKeySafe(info.AttachNode1))
									ByteConverter.GetBytes(labels[info.AttachNode1]).CopyTo(fc, ptr + 4);
								if (labels.ContainsKeySafe(info.Model1))
									ByteConverter.GetBytes(labels[info.Model1]).CopyTo(fc, ptr + 8);
								if (labels.ContainsKeySafe(info.AttachNode2))
									ByteConverter.GetBytes(labels[info.AttachNode2]).CopyTo(fc, ptr + 12);
								if (labels.ContainsKeySafe(info.Model2))
									ByteConverter.GetBytes(labels[info.Model2]).CopyTo(fc, ptr + 16);
							}
							ptr += 0x14;
						}
					}
					else
					{
						for (int i = 0; i < 14; i++)
						{
							UpgradeInfo info = ini.Upgrades[i];
							if (info.RootNode != null)
							{
								if (labels.ContainsKeySafe(info.RootNode))
									ByteConverter.GetBytes(labels[info.RootNode]).CopyTo(fc, ptr);
								if (labels.ContainsKeySafe(info.AttachNode1))
									ByteConverter.GetBytes(labels[info.AttachNode1]).CopyTo(fc, ptr + 4);
								if (labels.ContainsKeySafe(info.Model1))
									ByteConverter.GetBytes(labels[info.Model1]).CopyTo(fc, ptr + 8);
								if (labels.ContainsKeySafe(info.AttachNode2))
									ByteConverter.GetBytes(labels[info.AttachNode2]).CopyTo(fc, ptr + 12);
								if (labels.ContainsKeySafe(info.Model2))
									ByteConverter.GetBytes(labels[info.Model2]).CopyTo(fc, ptr + 16);
							}
							ptr += 0x14;
						}
					}
				int gcnt = ByteConverter.ToInt32(fc, 8);
				ptr = fc.GetPointer(0, key);
				if (ptr != 0)
					for (int gn = 0; gn <= gcnt; gn++)
					{
						SceneInfo info = ini.Scenes[gn];
						int ptr2 = fc.GetPointer(ptr, key);
						int ecnt = Math.Min(ByteConverter.ToInt32(fc, ptr + 4), info.Entities?.Count ?? 0);
						if (ptr2 != 0)
							for (int en = 0; en < ecnt; en++)
							{
								if (labels.ContainsKeySafe(info.Entities[en].Model))
									ByteConverter.GetBytes(labels[info.Entities[en].Model]).CopyTo(fc, ptr2);
								if (!battle)
								{
									if (labels.ContainsKeySafe(info.Entities[en].Motion))
										ByteConverter.GetBytes(labels[info.Entities[en].Motion]).CopyTo(fc, ptr2 + 4);
									if (labels.ContainsKeySafe(info.Entities[en].ShapeMotion))
										ByteConverter.GetBytes(labels[info.Entities[en].ShapeMotion]).CopyTo(fc, ptr2 + 8);
								}
								else
								{
									if (labels.ContainsKeySafe(info.Entities[en].GCModel))
										ByteConverter.GetBytes(labels[info.Entities[en].GCModel]).CopyTo(fc, ptr2 + 12);
									if (labels.ContainsKeySafe(info.Entities[en].ShadowModel))
										ByteConverter.GetBytes(labels[info.Entities[en].ShadowModel]).CopyTo(fc, ptr2 + 16);
								}
								ptr2 += battle ? 0x2C : 0x20;
							}
						if (!battle)
						{
							ptr2 = fc.GetPointer(ptr + 8, key);
							if (ptr2 != 0)
							{
								int cnt = ByteConverter.ToInt32(fc, ptr + 12);
								for (int i = 0; i < cnt; i++)
								{
									if (labels.ContainsKeySafe(info.CameraMotions[i]))
										ByteConverter.GetBytes(labels[info.CameraMotions[i]]).CopyTo(fc, ptr2);
									ptr2 += sizeof(int);
								}
							}
						}
						ptr2 = fc.GetPointer(ptr + 0x18, key);
						if (ptr2 != 0 && info.Big != null)
						{
							if (labels.ContainsKeySafe(info.Big.Model))
								ByteConverter.GetBytes(labels[info.Big.Model]).CopyTo(fc, ptr2);
							if (!battle)
							{
								int ptr3 = fc.GetPointer(ptr2 + 4, key);
								if (ptr3 != 0)
								{
									int cnt = ByteConverter.ToInt32(fc, ptr2 + 8);
									for (int i = 0; i < cnt; i++)
									{
										if (labels.ContainsKeySafe(info.Big.Motions[i][0]))
											ByteConverter.GetBytes(labels[info.Big.Motions[i][0]]).CopyTo(fc, ptr3);
										if (labels.ContainsKeySafe(info.Big.Motions[i][1]))
											ByteConverter.GetBytes(labels[info.Big.Motions[i][1]]).CopyTo(fc, ptr3 + 4);
										ptr3 += 8;
									}
								}
							}
						}
						ptr += 0x20;
					}

				ptr = fc.GetPointer(0x14, key);
				if (ptr != 0)
					for (int i = 0; i < 64; i++)
					{
						if (ini.UnknownModelData.ContainsKey(i) && labels.ContainsKeySafe(ini.UnknownModelData[i]))
							ByteConverter.GetBytes(labels[ini.UnknownModelData[i]]).CopyTo(fc, ptr);
						ptr += 4;
					}
				ptr = fc.GetPointer(0x18, key);
				if (ptr != 0)
					for (int i = 0; i < 93; i++)
					{
						if (ini.MechParts.ContainsKey(i) && labels.ContainsKeySafe(ini.MechParts[i]))
							ByteConverter.GetBytes(labels[ini.MechParts[i]]).CopyTo(fc, ptr);
						ptr += 4;
					}
				ptr = fc.GetPointer(0x1C, key);
				if (ptr != 0 && labels.ContainsKeySafe(ini.TailsTails))
					ByteConverter.GetBytes(labels[ini.TailsTails]).CopyTo(fc, ptr);
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					Prs.Compress(fc, filename);
				else
					File.WriteAllBytes(filename, fc);
			}
			finally
			{
				Environment.CurrentDirectory = dir;
			}
		}


			//Get Functions
			private static string GetModel(byte[] fc, int address, uint key, string fn)
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
						modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.Chunk));
					}
				}
				return name;
			}

		private static string GetGCModel(byte[] fc, int address, uint key, string fn)
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
					modelfiles.Add(obj.Name, new ModelInfo(fn, obj, ModelFormat.GC));
				}
			}
			return name;
		}

		private static string GetMotion(byte[] fc, int address, uint key, string fn, List<NJS_MOTION> motions, int cnt)
		{
			NJS_MOTION mtn = null;
			if (motions != null)
				mtn = motions[ByteConverter.ToInt32(fc, address)];
			else
			{
				int ptr3 = fc.GetPointer(address, key);
				if (ptr3 != 0)
					mtn = new NJS_MOTION(fc, ptr3, key, cnt);
			}
			if (mtn == null) return null;
			if (!motionfiles.ContainsKey(mtn.Name) || motionfiles[mtn.Name].Filename == null)
				motionfiles[mtn.Name] = new MotionInfo(fn, mtn);
			return mtn.Name;
		}

		//Read Functions
		private static List<NJS_MOTION> ReadMotionFile(string filename)
		{
			List<NJS_MOTION> motions = new List<NJS_MOTION>();
			byte[] fc = File.ReadAllBytes(filename);
			int addr = 0;
			while (ByteConverter.ToInt64(fc, addr) != 0)
			{
				int ptr = ByteConverter.ToInt32(fc, addr);
				if (ptr == -1)
					motions.Add(null);
				else
					motions.Add(new NJS_MOTION(fc, ptr, 0, ByteConverter.ToInt32(fc, addr + 4)));
				addr += 8;
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

		public ModelInfo(string fn, NJS_OBJECT obj, ModelFormat format)
		{
			Filename = fn;
			Model = obj;
			Format = format;
		}
	}

	public class MotionInfo
	{
		public string Filename { get; set; }
		public NJS_MOTION Motion { get; set; }

		public MotionInfo(string fn, NJS_MOTION mtn)
		{
			Filename = fn;
			Motion = mtn;
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
			get { return Game.ToString(); }
			set { Game = (Game)Enum.Parse(typeof(Game), value); }
		}
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
		public List<UpgradeInfo> Upgrades { get; set; } = new List<UpgradeInfo>();
		public Dictionary<int, string> UnknownModelData { get; set; } = new Dictionary<int, string>();
		public Dictionary<int, string> MechParts { get; set; } = new Dictionary<int, string>();
		public string TailsTails { get; set; }
		public List<SceneInfo> Scenes { get; set; } = new List<SceneInfo>();
		public List<string> Motions { get; set; }
	}

	public class UpgradeInfo
	{
		public string RootNode { get; set; }
		public string AttachNode1 { get; set; }
		public string Model1 { get; set; }
		public string AttachNode2 { get; set; }
		public string Model2 { get; set; }
	}

	public class SceneInfo
	{
		public List<EntityInfo> Entities { get; set; } = new List<EntityInfo>();
		public List<string> CameraMotions { get; set; } = new List<string>();
		public BigInfo Big { get; set; }
		public int FrameCount { get; set; }
	}

	public class EntityInfo
	{
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
}
