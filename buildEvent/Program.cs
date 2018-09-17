using FraGag.Compression;
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
				EventIniData ini = IniSerializer.Deserialize<EventIniData>(Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".ini")));
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
				List<byte> modelbytes = new List<byte>(fc);
				Dictionary<string, uint> labels = new Dictionary<string, uint>();
				foreach (string file in ini.Files.Where(a => HelperFunctions.FileHash(a.Key) != a.Value).Select(a => a.Key))
					modelbytes.AddRange(new ModelFile(Path.Combine(path, file)).Model.GetBytes((uint)(key + modelbytes.Count), false, labels, out uint address));
				fc = modelbytes.ToArray();
				int ptr = fc.GetPointer(0x20, key);
				if (ptr != 0)
					for (int i = 0; i < 18; i++)
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
						GroupInfo info = ini.Groups[gn];
						int ptr2 = fc.GetPointer(ptr, key);
						int ecnt = ByteConverter.ToInt32(fc, ptr + 4);
						if (ptr2 != 0)
							for (int en = 0; en < ecnt; en++)
							{
								if (labels.ContainsKey(info.Entities[en]))
									ByteConverter.GetBytes(labels[info.Entities[en]]).CopyTo(fc, ptr2);
								ptr2 += 0x2C;
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
				if (ptr != 0)
					if (labels.ContainsKey(ini.TailsTails))
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
		[IniAlwaysInclude]
		public Game Game { get; set; }
		public DictionaryContainer<string, string> Files { get; set; }
		[IniName("Upgrade")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<UpgradeInfo> Upgrades { get; set; }
		[IniName("Group")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<GroupInfo> Groups { get; set; }
		public DictionaryContainer<int, string> MechParts { get; set; }
		public string TailsTails { get; set; }

		public EventIniData()
		{
			Files = new DictionaryContainer<string, string>();
			Upgrades = new List<UpgradeInfo>();
			Groups = new List<GroupInfo>();
			MechParts = new DictionaryContainer<int, string>();
		}
	}

	public class DictionaryContainer<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<TKey, TValue> Items { get; set; }

		public DictionaryContainer()
		{
			Items = new Dictionary<TKey, TValue>();
		}

		public void Add(TKey key, TValue value)
		{
			Items.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return Items.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			return Items.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return Items.TryGetValue(key, out value);
		}

		public TValue this[TKey key]
		{
			get
			{
				return Items[key];
			}
			set
			{
				Items[key] = value;
			}
		}

		public void Clear()
		{
			Items.Clear();
		}

		[IniIgnore]
		public int Count
		{
			get { return Items.Count; }
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class UpgradeInfo
	{
		public string RootNode { get; set; }
		public string AttachNode1 { get; set; }
		public string Model1 { get; set; }
		public string AttachNode2 { get; set; }
		public string Model2 { get; set; }
	}

	public class GroupInfo
	{
		[IniName("Entity")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<string> Entities { get; set; }

		public GroupInfo()
		{
			Entities = new List<string>();
		}
	}
}
