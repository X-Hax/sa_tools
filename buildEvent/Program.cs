using FraGag.Compression;
using Newtonsoft.Json;
using SA_Tools;
using SonicRetro.SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteConverter = SonicRetro.SAModel.ByteConverter;

namespace buildEvent
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Write("Filename: ");
				args = new string[] { Console.ReadLine().Trim('"') };
			}
			foreach (string filename in args)
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
				if (fc[0] == 0x81)
				{
					ByteConverter.BigEndian = true;
					key = 0x8125FE60;
				}
				else
				{
					ByteConverter.BigEndian = false;
					key = 0xC600000;
				}
				bool battle = ini.Game == Game.SA2B;
				List<byte> modelbytes = new List<byte>(fc);
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".sa2mdl", StringComparison.OrdinalIgnoreCase) && HelperFunctions.FileHash(Path.Combine(path, a.Key)) != a.Value).Select(a => a.Key))
					modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, out uint _));
				if (battle)
				{
					List<byte> motionbytes = new List<byte>(new byte[(ini.Motions.Count + 1) * 8]);
					Dictionary<string, int> partcounts = new Dictionary<string, int>(ini.Motions.Count);
					foreach (string file in ini.Files.Where(a => a.Key.EndsWith(".saanim", StringComparison.OrdinalIgnoreCase)).Select(a => a.Key))
					{
						NJS_MOTION motion = NJS_MOTION.Load(Path.Combine(path, file));
						motionbytes.AddRange(motion.GetBytes((uint)(key + motionbytes.Count), labels, out uint _));
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
					for (int i = 0; i < (battle ? 18 : 16); i++)
					{
						UpgradeInfo info = ini.Upgrades[i];
						if (info.RootNode != null)
						{
							if (labels.ContainsKey(info.RootNode))
								ByteConverter.GetBytes(labels[info.RootNode]).CopyTo(fc, ptr);
							if (labels.ContainsKey(info.AttachNode1))
								ByteConverter.GetBytes(labels[info.AttachNode1]).CopyTo(fc, ptr + 4);
							if (labels.ContainsKey(info.Model1))
								ByteConverter.GetBytes(labels[info.Model1]).CopyTo(fc, ptr + 8);
							if (info.AttachNode2 != null && labels.ContainsKey(info.AttachNode2))
								ByteConverter.GetBytes(labels[info.AttachNode2]).CopyTo(fc, ptr + 12);
							if (info.Model2 != null && labels.ContainsKey(info.Model2))
								ByteConverter.GetBytes(labels[info.Model2]).CopyTo(fc, ptr + 16);
						}
						ptr += 0x14;
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
								if (labels.ContainsKey(info.Entities[en].Model))
									ByteConverter.GetBytes(labels[info.Entities[en].Model]).CopyTo(fc, ptr2);
								if (!battle && labels.ContainsKey(info.Entities[en].Motion))
									ByteConverter.GetBytes(labels[info.Entities[en].Motion]).CopyTo(fc, ptr2 + 4);
								if (!battle && labels.ContainsKey(info.Entities[en].ShapeMotion))
									ByteConverter.GetBytes(labels[info.Entities[en].ShapeMotion]).CopyTo(fc, ptr2 + 8);
								if (battle && labels.ContainsKey(info.Entities[en].ShadowModel))
									ByteConverter.GetBytes(labels[info.Entities[en].ShadowModel]).CopyTo(fc, ptr2 + 16);
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
									if (labels.ContainsKey(info.CameraMotions[i]))
										ByteConverter.GetBytes(labels[info.CameraMotions[i]]).CopyTo(fc, ptr2);
									ptr2 += sizeof(int);
								}
							}
						}
						ptr2 = fc.GetPointer(ptr + 0x18, key);
						if (ptr2 != 0 && info.Big != null)
						{
							if (labels.ContainsKey(info.Big.Model))
								ByteConverter.GetBytes(labels[info.Big.Model]).CopyTo(fc, ptr2);
							if (!battle)
							{
								int ptr3 = fc.GetPointer(ptr2 + 4, key);
								if (ptr3 != 0)
								{
									int cnt = ByteConverter.ToInt32(fc, ptr2 + 8);
									for (int i = 0; i < cnt; i++)
									{
										if (labels.ContainsKey(info.Big.Motions[i][0]))
											ByteConverter.GetBytes(labels[info.Big.Motions[i][0]]).CopyTo(fc, ptr3);
										if (labels.ContainsKey(info.Big.Motions[i][1]))
											ByteConverter.GetBytes(labels[info.Big.Motions[i][1]]).CopyTo(fc, ptr3 + 4);
										ptr3 += 8;
									}
								}
							}
						}
						ptr += 0x20;
					}
				ptr = fc.GetPointer(0x18, key);
				if (ptr != 0)
					for (int i = 0; i < 18; i++)
					{
						if (ini.MechParts.ContainsKey(i) && labels.ContainsKey(ini.MechParts[i]))
							ByteConverter.GetBytes(labels[ini.MechParts[i]]).CopyTo(fc, ptr);
						ptr += 4;
					}
				ptr = fc.GetPointer(0x1C, key);
				if (ptr != 0 && ini.TailsTails != null && labels.ContainsKey(ini.TailsTails))
						ByteConverter.GetBytes(labels[ini.TailsTails]).CopyTo(fc, ptr);
				if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
					Prs.Compress(fc, filename);
				else
					File.WriteAllBytes(filename, fc);
			}
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
