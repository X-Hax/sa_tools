using FraGag.Compression;
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
				if (fc[0] == 0x81)
				{
					Console.WriteLine("File is in GC/PC format.");
					ByteConverter.BigEndian = true;
					key = 0x8125FE60;
					ini.Game = Game.SA2B;
				}
				else
				{
					Console.WriteLine("File is in DC format.");
					ByteConverter.BigEndian = false;
					key = 0xC600000;
					ini.Game = Game.SA2;
				}
				List<string> nodenames = new List<string>();
				Dictionary<string, KeyValuePair<string, NJS_OBJECT>> modelfiles = new Dictionary<string, KeyValuePair<string, NJS_OBJECT>>();
				int ptr = fc.GetPointer(0x20, key);
				if (ptr != 0)
					for (int i = 0; i < 18; i++)
					{
						UpgradeInfo info = new UpgradeInfo();
						int ptr2 = fc.GetPointer(ptr, key);
						if (ptr2 != 0)
						{
							string name = $"object_{ptr2:X8}";
							if (!nodenames.Contains(name))
							{
								NJS_OBJECT obj = new NJS_OBJECT(fc, ptr2, key, ModelFormat.Chunk);
								info.RootNode = obj.Name;
								List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
								foreach (string s in names)
									if (modelfiles.ContainsKey(s))
										modelfiles.Remove(s);
								nodenames.AddRange(names);
								modelfiles.Add(obj.Name, new KeyValuePair<string, NJS_OBJECT>(Path.Combine(path, $"Upgrade {i + 1} Root.sa2mdl"), obj));
							}
							else
								info.RootNode = name;
							ptr2 = fc.GetPointer(ptr + 4, key);
							if (ptr2 != 0)
								info.AttachNode1 = $"object_{ptr2:X8}";
							ptr2 = fc.GetPointer(ptr + 8, key);
							if (ptr2 != 0)
							{
								name = $"object_{ptr2:X8}";
								if (!nodenames.Contains(name))
								{
									NJS_OBJECT obj = new NJS_OBJECT(fc, ptr2, key, ModelFormat.Chunk);
									info.Model1 = obj.Name;
									List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
									foreach (string s in names)
										if (modelfiles.ContainsKey(s))
											modelfiles.Remove(s);
									nodenames.AddRange(names);
									modelfiles.Add(obj.Name, new KeyValuePair<string, NJS_OBJECT>(Path.Combine(path, $"Upgrade {i + 1} Model 1.sa2mdl"), obj));
								}
								else
									info.Model1 = name;
							}
							ptr2 = fc.GetPointer(ptr + 12, key);
							if (ptr2 != 0)
								info.AttachNode2 = $"object_{ptr2:X8}";
							ptr2 = fc.GetPointer(ptr + 16, key);
							if (ptr2 != 0)
							{
								name = $"object_{ptr2:X8}";
								if (!nodenames.Contains(name))
								{
									NJS_OBJECT obj = new NJS_OBJECT(fc, ptr2, key, ModelFormat.Chunk);
									info.Model2 = obj.Name;
									List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
									foreach (string s in names)
										if (modelfiles.ContainsKey(s))
											modelfiles.Remove(s);
									nodenames.AddRange(names);
									modelfiles.Add(obj.Name, new KeyValuePair<string, NJS_OBJECT>(Path.Combine(path, $"Upgrade {i + 1} Model 2.sa2mdl"), obj));
								}
								else
									info.Model2 = name;
							}
						}
						ini.Upgrades.Add(info);
						ptr += 0x14;
					}
				else
					Console.WriteLine("Event contains no character upgrades.");
				int gcnt = ByteConverter.ToInt32(fc, 8);
				ptr = fc.GetPointer(0, key);
				if (ptr != 0)
				{
					Console.WriteLine("Event contains {0} group(s).", gcnt + 1);
					for (int gn = 0; gn <= gcnt; gn++)
					{
						GroupInfo info = new GroupInfo();
						int ptr2 = fc.GetPointer(ptr, key);
						int ecnt = ByteConverter.ToInt32(fc, ptr + 4);
						if (ptr2 != 0)
						{
							Console.WriteLine("Group {0} contains {1} entit{2}.", gn + 1, ecnt, ecnt == 1 ? "y" : "ies");
							for (int en = 0; en < ecnt; en++)
							{
								int ptr3 = fc.GetPointer(ptr2, key);
								if (ptr3 != 0)
								{
									string name = $"object_{ptr3:X8}";
									if (!nodenames.Contains(name))
									{
										NJS_OBJECT obj = new NJS_OBJECT(fc, ptr3, key, ModelFormat.Chunk);
										info.Entities.Add(obj.Name);
										List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
										foreach (string s in names)
											if (modelfiles.ContainsKey(s))
												modelfiles.Remove(s);
										nodenames.AddRange(names);
										modelfiles.Add(obj.Name, new KeyValuePair<string, NJS_OBJECT>(Path.Combine(path, $"Group {gn + 1} Entity {en + 1} Model.sa2mdl"), obj));
									}
									else
										info.Entities.Add(name);
								}
								ptr2 += 0x2C;
							}
						}
						else
							Console.WriteLine("Group {0} contains no entities.", gn + 1);
						ini.Groups.Add(info);
						ptr += 0x20;
					}
				}
				else
					Console.WriteLine("Event contains no groups.");
				foreach (var item in modelfiles)
				{
					ModelFile.CreateFile(item.Value.Key, item.Value.Value, null, null, null, null, null, ModelFormat.Chunk);
					ini.Files.Add(Path.GetFileName(item.Value.Key), HelperFunctions.FileHash(item.Value.Key));
				}
				IniSerializer.Serialize(ini, Path.Combine(path, Path.ChangeExtension(Path.GetFileName(filename), ".ini")));
			}
		}
	}

	public class EventIniData
	{
		public string Name { get; set; }
		[IniAlwaysInclude]
		public Game Game { get; set; }
		public DictionaryContainer<string> Files { get; set; }
		[IniName("Upgrade")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<UpgradeInfo> Upgrades { get; set; }
		[IniName("Group")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<GroupInfo> Groups { get; set; }

		public EventIniData()
		{
			Files = new DictionaryContainer<string>();
			Upgrades = new List<UpgradeInfo>();
			Groups = new List<GroupInfo>();
		}
	}

	public class DictionaryContainer<T> : IEnumerable<KeyValuePair<string, T>>
	{
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, T> Items { get; set; }

		public DictionaryContainer()
		{
			Items = new Dictionary<string, T>();
		}

		public void Add(string key, T value)
		{
			Items.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return Items.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return Items.Remove(key);
		}

		public bool TryGetValue(string key, out T value)
		{
			return Items.TryGetValue(key, out value);
		}

		public T this[string key]
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

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
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
		public List<String> Entities { get; set; }

		public GroupInfo()
		{
			Entities = new List<string>();
		}
	}
}
