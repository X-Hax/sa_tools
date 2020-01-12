using FraGag.Compression;
using Newtonsoft.Json;
using SA_Tools;
using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteConverter = SonicRetro.SAModel.ByteConverter;

namespace splitEvent
{
	static class Program
	{
		static readonly string[] upgradenames = { "Sonic's Light Shoes", "Sonic's Flame Ring", "Sonic's Bounce Bracelet", "Sonic's Magic Gloves", "Shadow's Air Shoes", "Shadow's Flame Ring", "Knuckles' Shovel Claw L", "Knuckles' Shovel Claw R", "Knuckles' Hammer Glove L", "Knuckles' Hammer Glove R", "Knuckles' Sunglasses", "Knuckles' Air Necklace", "Rouge's Pick Nails", "Rouge's Treasure Scope", "Rouge's Iron Boots", "Rouge's Heart Plates", "Mech Tails' Window", "Mech Eggman's Window" };
		static List<string> nodenames = new List<string>();
		static Dictionary<string, ModelInfo> modelfiles = new Dictionary<string, ModelInfo>();
		static Dictionary<string, MotionInfo> motionfiles = new Dictionary<string, MotionInfo>();

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Write("Filename: ");
				args = new string[] { Console.ReadLine().Trim('"') };
			}
			foreach (string filename in args)
			{
				Console.WriteLine("Splitting file {0}...", filename);
				byte[] fc;
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					fc = Prs.Decompress(filename);
				else
					fc = File.ReadAllBytes(filename);
				EventIniData ini = new EventIniData() { Name = Path.GetFileNameWithoutExtension(filename) };
				string path = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename))).FullName;
				uint key;
				List<NJS_MOTION> motions = null;
				bool battle;
				if (fc[0] == 0x81)
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					key = 0x8125FE60;
					ini.Game = Game.SA2B;
					battle = true;
					motions = ReadMotionFile(Path.ChangeExtension(filename, null) + "motion.bin");
					ini.Motions = motions.Select(a => a?.Name).ToList();
					foreach (var mtn in motions.Where(a => a != null))
						motionfiles[mtn.Name] = new MotionInfo(null, mtn);
				}
				else
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					key = 0xC600000;
					ini.Game = Game.SA2;
					battle = false;
				}
				int ptr = fc.GetPointer(0x20, key);
				if (ptr != 0)
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
							info.Model1 = GetModel(fc, ptr + 8, key, $"{upnam} Model 1.sa2mdl");
							ptr2 = fc.GetPointer(ptr + 0xC, key);
							if (ptr2 != 0)
								info.AttachNode2 = $"object_{ptr2:X8}";
							info.Model2 = GetModel(fc, ptr + 0x10, key, $"{upnam} Model 2.sa2mdl");
						}
						ini.Upgrades.Add(info);
						ptr += 0x14;
					}
				else
					Console.WriteLine("Event contains no character upgrades.");
				ptr = fc.GetPointer(0x18, key);
				if (ptr != 0)
					for (int i = 0; i < 93; i++)
					{
						string name = GetModel(fc, ptr, key, $"Mech Part {i + 1}.sa2mdl");
						if (name != null)
							ini.MechParts.Add(i, name);
						ptr += 4;
					}
				else
					Console.WriteLine("Event contains no mech parts.");
				int gcnt = ByteConverter.ToInt32(fc, 8);
				ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					Console.WriteLine("Event contains {0} scene(s).", gcnt + 1);
					for (int gn = 0; gn <= gcnt; gn++)
					{
						Directory.CreateDirectory(Path.Combine(path, $"Scene {gn + 1}"));
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
								ent.Motion = GetMotion(fc, ptr2 + 4, key, $"Scene {gn + 1}\\Entity {en + 1} Motion.saanim", motions, modelfiles[ent.Model].Model.CountAnimated());
								if (ent.Motion != null)
									modelfiles[ent.Model].Motions.Add(motionfiles[ent.Motion].Filename);
								ent.ShapeMotion = GetMotion(fc, ptr2 + 8, key, $"Scene {gn + 1}\\Entity {en + 1} Shape Motion.saanim", motions, modelfiles[ent.Model].Model.CountMorph());
								if (ent.ShapeMotion != null)
									modelfiles[ent.Model].Motions.Add(motionfiles[ent.ShapeMotion].Filename);
								if (battle)
								{
									ent.ShadowModel = GetModel(fc, ptr2 + 16, key, $"Scene {gn + 1}\\Entity {en + 1} Shadow Model.sa2mdl");
									ent.Position = new Vertex(fc, ptr2 + 24);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 40);
								}
								else
								{
									ent.Position = new Vertex(fc, ptr2 + 16);
									ent.Flags = ByteConverter.ToUInt32(fc, ptr2 + 28);
								}
								scn.Entities.Add(ent);
								ptr2 += ini.Game == Game.SA2B ? 0x2C : 0x20;
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
					ini.TailsTails = GetModel(fc, ptr, key, $"Tails' tails.sa2mdl");
				else
					Console.WriteLine("Event does not contain Tails' tails.");
				foreach (var item in motionfiles.Values)
				{
					string fn = item.Filename ?? $"Unknown Motion {motions.IndexOf(item.Motion)}.saanim";
					string fp = Path.Combine(path, fn);
					item.Motion.Save(fp);
					ini.Files.Add(fn, HelperFunctions.FileHash(fp));
				}
				foreach (var item in modelfiles.Values)
				{
					string fp = Path.Combine(path, item.Filename);
					ModelFile.CreateFile(fp, item.Model, item.Motions.ToArray(), null, null, null, ModelFormat.Chunk);
					ini.Files.Add(item.Filename, HelperFunctions.FileHash(fp));
				}
				JsonSerializer js = new JsonSerializer
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				};
				using (var tw = File.CreateText(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".json"))))
					js.Serialize(tw, ini);
			}
		}

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
					modelfiles.Add(obj.Name, new ModelInfo(fn, obj));
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
	}

	public class ModelInfo
	{
		public string Filename { get; set; }
		public NJS_OBJECT Model { get; set; }
		public List<string> Motions { get; set; } = new List<string>();

		public ModelInfo(string fn, NJS_OBJECT obj)
		{
			Filename = fn;
			Model = obj;
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
		public string ShadowModel { get; set; }
		public Vertex Position { get; set; }
		public uint Flags { get; set; }
	}

	public class BigInfo
	{
		public string Model { get; set; }
		public List<string[]> Motions { get; set; } = new List<string[]>();
		public int Unknown { get; set; }
	}
}
