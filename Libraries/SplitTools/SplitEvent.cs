using FraGag.Compression;
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
		static Dictionary<string, CameraInfo> camarrayfiles = new Dictionary<string, CameraInfo>();
		static Dictionary<string, TexAnimFileInfo> texanimfiles = new Dictionary<string, TexAnimFileInfo>();

		public static void Split(string filename, string outputPath)
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

				Console.WriteLine("Splitting file {0}...", evfilename);
				byte[] fc;
				if (Path.GetExtension(evfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(evfilename);
				else
					fc = File.ReadAllBytes(evfilename);
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
				uint key;
				List<NJS_MOTION> motions = null;
				List<NJS_CAMERA> ncams = null;
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
						ncams = ReadMotionFileCams(Path.ChangeExtension(evfilename, null) + "motion.bin");
						ini.Motions = motions.Select(a => a?.Name).ToList();
						foreach (var mtn in motions.Where(a => a != null))
							motionfiles[mtn.Name] = new MotionInfo(null, mtn);
						ini.NinjaCameras = ncams.Select(a => a?.Name).ToList();
						foreach (var camdata in ncams.Where(a => a != null))
							camarrayfiles[camdata.Name] = new CameraInfo(null, camdata);
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
							UpgradeInfo info = new UpgradeInfo();
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
						{
							name = $"object_{ptr2:X8}";
							ini.MechParts.Add(i, name);
							namecount++;
						}
						else
							ini.MechParts.Add(i, null);
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
							int camaddr = ByteConverter.ToInt32(fc, ptr2 + 0xC);
							int cnt = ByteConverter.ToInt32(fc, ptr + 12);
							for (int i = 0; i < cnt; i++)
							{
								scn.CameraMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn + 1}\\Camera Motion {i + 1}.saanim", motions, 1));
								if (battle)
									scn.NinjaCameras.Add(GetGCCamData(fc, ptr2, key, $"Scene {gn + 1}\\Camera Motion {i + 1} Attributes.ini", ncams));
								else
									scn.NinjaCameras.Add(GetCamData(fc, ptr2, key, $"Scene {gn + 1}\\Camera Motion {i + 1} Attributes.ini", ncams));
								ptr2 += sizeof(int);
							}
						}
						ptr2 = fc.GetPointer(ptr + 0x10, key);
						if (ptr2 != 0)
						{
							int cnt = ByteConverter.ToInt32(fc, ptr + 0x14);
							for (int i = 0; i < cnt; i++)
							{
								scn.StaticMotions.Add(GetMotion(fc, ptr2, key, $"Scene {gn + 1}\\Static Motion {i + 1}.saanim", motions, 1));
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
					if (ptr2 != 0)
					{
						Console.WriteLine("Event contains an internal texture list.");
						NJS_TEXLIST tls = new NJS_TEXLIST(fc, ptr, key);
						ini.Texlist = GetTexlist(fc, 4, key, "InternalTexlist.satex");
						string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "InternalTexlist.satex");
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
						string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "TextureSizes.ini");
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
						int rptr = fc.GetPointer(ptr + 0x84, key);
						ReflectionInfo refl = new ReflectionInfo();
						refl.Unk1 = ByteConverter.ToInt32(fc, ptr);
						refl.Unk2 = ByteConverter.ToInt32(fc, ptr + 4);
						//There's a huge gap here of 0x84. Investigate further.
						ReflectionMatrixData rmx = new ReflectionMatrixData(fc, rptr);
						refl.ReflectData = GetReflectData(fc, ptr + 0x84, key, "ReflectionData.ini");
						string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "ReflectionData.ini");
						rmx.Save(fp);
						ini.Files.Add("ReflectionData.ini", HelperFunctions.FileHash(fp));
						ini.ReflectionInfo.Add(refl);
						Console.WriteLine("Event contains character reflection info.");
					}
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
						{
							name = $"object_{ptr2:X8}";
							ini.UnknownModelData.Add(i, name);
							namecount++;
						}
						else
							ini.UnknownModelData.Add(i, null);
						ptr += 4;
					}
					Console.WriteLine("Event contains {0} unknown model pointe{1}.", namecount == 0 ? "no" : $"{namecount}", namecount == 1 ? "r" : "rs");
				}
				ptr = fc.GetPointer(0x24, key);
				if (ptr == 0 || dcbeta && ((fc[37] == 0x25) || (fc[38] == 0x22)))
					Console.WriteLine("Event does not contain texture animation data.");
				else
				{
					Directory.CreateDirectory(Path.Combine(Path.GetFileNameWithoutExtension(evfilename), "Texture Animations"));
					TexAnimInfo tanim = new TexAnimInfo();
					int ptr2 = fc.GetPointer(ptr, key);
					if (battle)
					{
						int tn = ByteConverter.ToInt32(fc, ptr + 8);
						int tanimcount = 0;
						for (int d = 0; d < 3; d++)
						{
							TexAnimMain main = new TexAnimMain();
							int mptr = fc.GetPointer(ptr2, key);
							int taptr = fc.GetPointer(ptr2 + 8, key);
							int tanims = ByteConverter.ToInt32(fc, ptr2 + 4);
							if (mptr != 0)
							{
								tanimcount++;
								main.Model = $"object_{mptr:X8}";
								main.TexAnimDataEntries = ByteConverter.ToInt32(fc, ptr2 + 4);
								for (int t = 0; t < tanims; t++)
								{
									main.TexAnimPointers.Add(GetTexanim(fc, taptr, key, $"Texture Animations\\TexanimInfo {d + 1} Part {t + 1}.ini"));
									taptr += 0x4;
								}
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
						tanim.TexAnimMainDataEntries = ByteConverter.ToInt32(fc, ptr + 8);
						ini.TextureAnimations.Add(tanim);
						if (tn != tanimcount)
							Console.WriteLine("Event contains {0} mode{1} with texture animation data. {2} data se{3} {4} used.", $"{tanimcount}", tanimcount == 1 ? "l" : "ls", tn == 1 ? "Only the first" : $"The first {tn}", tn == 1 ? "t" : "ts", tn == 1 ? "is" : "are");
						else
							Console.WriteLine("Event contains {0} mode{1} with texture animation data.", $"{tanimcount}", tanimcount == 1 ? "l" : "ls");
					}
					else
					{
						int tanimcount = 0;
						for (int d = 0; d < 3; d++)
						{
							TexAnimMain main = new TexAnimMain();
							int mptr = fc.GetPointer(ptr2, key);
							int taptr = fc.GetPointer(ptr2 + 8, key);
							int tanims = ByteConverter.ToInt32(fc, ptr2 + 4);
							if (mptr != 0)
							{
								tanimcount++;
								main.Model = $"object_{mptr:X8}";
								main.TexAnimDataEntries = ByteConverter.ToInt32(fc, ptr2 + 4);
								for (int t = 0; t < tanims; t++)
								{
									main.TexAnimPointers.Add(GetTexanim(fc, taptr, key, $"Texture Animations\\TexanimInfo {d + 1} Part {t + 1}.ini"));
									taptr += 0x4;
								}
							}
							else
								break;
							tanim.MainData.Add(main);
							ptr2 += 0xC;
						}
						tanim.TexID = ByteConverter.ToInt32(fc, ptr + 4);
						tanim.TexLoopNumber = ByteConverter.ToInt32(fc, ptr + 8);
						ini.TextureAnimations.Add(tanim);
						if (tanimcount != 1)
							Console.WriteLine("Event contains {0} models with texture animation data. Only the first data set is used.", $"{tanimcount}");
						else
							Console.WriteLine("Event contains 1 model with texture animation data.");
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
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in camarrayfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.CamData.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in texanimfiles.Values)
				{
					string fn = item.Filename;
					string fp = Path.Combine(Path.GetFileNameWithoutExtension(evfilename), fn);
					item.TextureAnimation.Save(fp);
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
							ptr2 = fc.GetPointer(ptr + 0x10, key);
							if (ptr2 != 0)
							{
								int cnt = ByteConverter.ToInt32(fc, ptr + 0x14);
								for (int i = 0; i < cnt; i++)
								{
									if (labels.ContainsKeySafe(info.StaticMotions[i]))
										ByteConverter.GetBytes(labels[info.StaticMotions[i]]).CopyTo(fc, ptr2);
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
				name = $"matrix_{ptr3:X8}";
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
					motions.Add(new NJS_MOTION(fc, ptr, 0, nummdl));
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
			get { return Game.ToString(); }
			set { Game = (Game)Enum.Parse(typeof(Game), value); }
		}
		public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
		public List<UpgradeInfo> Upgrades { get; set; } = new List<UpgradeInfo>();
		public Dictionary<int, string> UnknownModelData { get; set; } = new Dictionary<int, string>();
		public Dictionary<int, string> MechParts { get; set; } = new Dictionary<int, string>();
		public string TailsTails { get; set; }
		public string Texlist { get; set; }
		public string TexSizes { get; set; }
		public bool GCShadowMaps { get; set; }
		public List<SceneInfo> Scenes { get; set; } = new List<SceneInfo>();
		public List<TexAnimInfo> TextureAnimations { get; set; } = new List<TexAnimInfo>();
		public List<ReflectionInfo> ReflectionInfo { get; set; } = new List<ReflectionInfo>();
		public List<string> Motions { get; set; }
		public List<string> NinjaCameras { get; set; }
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
		public List<string> NinjaCameras { get; set; } = new List<string>();
		public List<string> StaticMotions { get; set; } = new List<string>();
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

	[Serializable]
	public class TexSizes
	{
		public string Name { get; set; }
		public List<TexSizeArray> TextureSize { get; set; } = new List<TexSizeArray>();

		public TexSizes(byte[] file, int address, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "texsizes_" + address.ToString("X8");
			TextureSize = new List<TexSizeArray>();
			int numtex = ByteConverter.ToInt32(file, address - 4);
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
	}

	[Serializable]
	public class TexSizeArray
	{
		[IniAlwaysInclude]
		public short H { get; set; }
		[IniAlwaysInclude]
		public short V { get; set; }

		public TexSizeArray(byte[] file, int address)
		{
			H = ByteConverter.ToInt16(file, address);
			V = ByteConverter.ToInt16(file, address + 2);

		}
	}
		public class TexAnimInfo
	{
		public List<TexAnimMain> MainData { get; set; } = new List<TexAnimMain>();
		public List<TexAnimGCIDs> TexAnimGCIDs { get; set; } = new List<TexAnimGCIDs>();
		public int TexID { get; set; }
		public int TexLoopNumber { get; set; }
		public int TexAnimMainDataEntries { get; set; }

	}
	public class TexAnimGCIDs
	{
		public int TexID { get; set; }
		public int TexLoopNumber { get; set; }
	}

	public class TexAnimMain
	{
		public string Model { get; set; }
		public int TexAnimDataEntries { get; set; }
		public List<string> TexAnimPointers { get; set; } = new List<string>();
	}

	public class ReflectionInfo
	{
		public int Unk1 { get; set; }
		public int Unk2 { get; set; }
		public string ReflectData { get; set; }
	}

	[Serializable]
	public class ReflectionMatrixData
	{
		public string Name { get; set; }
		public Vertex Data1 { get; set; }
		public Vertex Data2 { get; set; }
		public Vertex Data3 { get; set; }
		public Vertex Data4 { get; set; }

		public ReflectionMatrixData(byte[] file, int address, Dictionary<int, string> labels = null, uint offset = 0)
		{
			if (labels != null && labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "matrix_" + address.ToString("X8");
			Data1 = new Vertex(file, address);
			Data2 = new Vertex(file, address + 0xC);
			Data3 = new Vertex(file, address + 0x18);
			Data4 = new Vertex(file, address + 0x24);
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

	}
	[Serializable]
	public class UVData
	{
		public string UVAddress { get; set; }
		[IniAlwaysInclude]
		public short U { get; set; }
		[IniAlwaysInclude]
		public short V { get; set; }

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
			uint ncamaddr = ByteConverter.ToUInt32(file, address);
			uint animaddr = ByteConverter.ToUInt32(file, address + 4);
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
		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}
	}
}
