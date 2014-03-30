using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using SADXPCTools;
using SonicRetro.SAModel;
using System.Runtime.InteropServices;

namespace splitDLL
{
	static class Program
	{
		static void Main(string[] args)
		{
			string datafilename, inifilename;
			if (args.Length > 0)
			{
				datafilename = args[0];
				Console.WriteLine("File: {0}", datafilename);
			}
			else
			{
				Console.Write("File: ");
				datafilename = Console.ReadLine();
			}
			if (args.Length > 1)
			{
				inifilename = args[1];
				Console.WriteLine("INI File: {0}", inifilename);
			}
			else
			{
				Console.Write("INI File: ");
				inifilename = Console.ReadLine();
			}
			byte[] datafile = File.ReadAllBytes(datafilename);
			IniData inifile = IniFile.Deserialize<IniData>(inifilename);
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
					string name = HelperFunctions.GetCString(datafile, BitConverter.ToInt32(datafile, nameaddr),
						System.Text.Encoding.ASCII);
					int addr = BitConverter.ToInt32(datafile,
						dir.AddressOfFunctions + (BitConverter.ToInt16(datafile, ordaddr) * 4));
					exports.Add(name, addr);
					nameaddr += 4;
					ordaddr += 2;
				}
			}
			ModelFormat modelfmt = 0;
			LandTableFormat landfmt = 0;
			string modelext = null;
			string landext = null;
			switch (inifile.Game)
			{
				case Game.SADX:
					modelfmt = ModelFormat.BasicDX;
					landfmt = LandTableFormat.SADX;
					modelext = ".sa1mdl";
					landext = ".sa1lvl";
					break;
				case Game.SA2B:
					modelfmt = ModelFormat.Chunk;
					landfmt = LandTableFormat.SA2B;
					modelext = ".sa2mdl";
					landext = ".sa2blvl";
					break;
			}
			int itemcount = 0;
			List<string> labels = new List<string>();
			ModelAnimationsDictionary models = new ModelAnimationsDictionary();
			MyClass output = new MyClass();
			Stopwatch timer = new Stopwatch();
			timer.Start();
			foreach (KeyValuePair<string, FileInfo> item in inifile.Files)
			{
				if (string.IsNullOrEmpty(item.Key)) continue;
				FileInfo data = item.Value;
				string type = data.Type;
				string name = item.Key;
				output.ItemTypes.Items[name] = type;
				int address = exports[name];
				Console.WriteLine(name + " → " + data.Filename);
				Directory.CreateDirectory(Path.GetDirectoryName(data.Filename));
				Dictionary<int, string> tmplabels;
				switch (type)
				{
					case "landtable":
						{
							tmplabels = new Dictionary<int, string>();
							LandTable land = new LandTable(datafile, address, imageBase, landfmt, tmplabels) { Description = name, Tool = "splitDLL" };
							land.SaveToFile(data.Filename, landfmt);
							output.Labels.Items[name] = land.Name;
							output.Files.Items[data.Filename] = HelperFunctions.FileHash(data.Filename);
							labels.AddRange(tmplabels.Values);
						}
						break;
					case "landtablearray":
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = BitConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								tmplabels = new Dictionary<int, string>();
								string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
								LandTable land = new LandTable(datafile, ptr, imageBase, landfmt, tmplabels) { Description = idx, Tool = "splitDLL" };
								output.Labels.Items[idx] = land.Name;
								if (!labels.Contains(land.Name))
								{
									string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + landext);
									land.SaveToFile(fn, landfmt);
									output.Files.Items[fn] = HelperFunctions.FileHash(fn);
									labels.AddRange(tmplabels.Values);
								}
							}
							address += 4;
						}
						break;
					case "model":
						{
							tmplabels = new Dictionary<int, string>();
							SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, modelfmt, tmplabels);
							models.Add(new ModelAnimations(data.Filename, name, mdl, modelfmt));
							output.Labels.Items[name] = mdl.Name;
							labels.AddRange(tmplabels.Values);
						}
						break;
					case "modelarray":
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = BitConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								tmplabels = new Dictionary<int, string>();
								SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, ptr, imageBase, modelfmt, tmplabels);
								string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
								output.Labels.Items[idx] = mdl.Name;
								if (!labels.Contains(mdl.Name))
								{
									string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
									models.Add(new ModelAnimations(fn, idx, mdl, modelfmt));
									labels.AddRange(tmplabels.Values);
								}
							}
							address += 4;
						}
						break;
					case "basicmodel":
						{
							tmplabels = new Dictionary<int, string>();
							SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, ModelFormat.Basic, tmplabels);
							models.Add(new ModelAnimations(data.Filename, name, mdl, ModelFormat.Basic));
							output.Labels.Items[name] = mdl.Name;
							labels.AddRange(tmplabels.Values);
						}
						break;
					case "basicmodelarray":
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = BitConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								tmplabels = new Dictionary<int, string>();
								SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, ptr, imageBase, ModelFormat.Basic, tmplabels);
								string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
								output.Labels.Items[idx] = mdl.Name;
								if (!labels.Contains(mdl.Name))
								{
									string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
									models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.Basic));
									labels.AddRange(tmplabels.Values);
								}
							}
							address += 4;
						}
						break;
					case "basicdxmodel":
						{
							tmplabels = new Dictionary<int, string>();
							SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, ModelFormat.BasicDX, tmplabels);
							models.Add(new ModelAnimations(data.Filename, name, mdl, ModelFormat.BasicDX));
							output.Labels.Items[name] = mdl.Name;
							labels.AddRange(tmplabels.Values);
						}
						break;
					case "basicdxmodelarray":
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = BitConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								tmplabels = new Dictionary<int, string>();
								SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, ptr, imageBase, ModelFormat.BasicDX, tmplabels);
								string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
								output.Labels.Items[idx] = mdl.Name;
								if (!labels.Contains(mdl.Name))
								{
									string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
									models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.BasicDX));
									labels.AddRange(tmplabels.Values);
								}
							}
							address += 4;
						}
						break;
					case "chunkmodel":
						{
							tmplabels = new Dictionary<int, string>();
							SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, address, imageBase, ModelFormat.Chunk, tmplabels);
							models.Add(new ModelAnimations(data.Filename, name, mdl, ModelFormat.Chunk));
							output.Labels.Items[name] = mdl.Name;
							labels.AddRange(tmplabels.Values);
						}
						break;
					case "chunkmodelarray":
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = BitConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								tmplabels = new Dictionary<int, string>();
								SonicRetro.SAModel.Object mdl = new SonicRetro.SAModel.Object(datafile, ptr, imageBase, ModelFormat.Chunk, tmplabels);
								string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
								output.Labels.Items[idx] = mdl.Name;
								if (!labels.Contains(mdl.Name))
								{
									string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
									models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.Chunk));
									labels.AddRange(tmplabels.Values);
								}
							}
							address += 4;
						}
						break;
					case "actionarray":
						for (int i = 0; i < data.Length; i++)
						{
							int ptr = BitConverter.ToInt32(datafile, address);
							if (ptr != 0)
							{
								ptr = (int)(ptr - imageBase);
								tmplabels = new Dictionary<int, string>();
								AnimationHeader ani = new AnimationHeader(datafile, ptr, imageBase, modelfmt, tmplabels);
								string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
								output.Labels.Items[idx + ".motion"] = ani.Animation.Name;
								output.Labels.Items[idx + ".object"] = ani.Model.Name;
								string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
								ani.Animation.Save(fn);
								output.Files.Items[fn] = HelperFunctions.FileHash(fn);
								if (models.Contains(ani.Model.Name))
								{
									ModelAnimations mdl = models[ani.Model.Name];
									System.Text.StringBuilder sb = new System.Text.StringBuilder(260);
									PathRelativePathTo(sb, Path.GetFullPath(mdl.Filename), 0, Path.GetFullPath(fn), 0);
									mdl.Animations.Add(sb.ToString());
								}
								else
								{
									string mfn = Path.ChangeExtension(fn, modelext);
									ModelFile.CreateFile(mfn, ani.Model, new[] { Path.GetFileName(fn) }, null, null,
										idx + ".object", "splitDLL", null, modelfmt);
									output.Files.Items[mfn] = HelperFunctions.FileHash(mfn);
								}
							}
							address += 4;
						}
						break;
				}
				itemcount++;
			}
			foreach (ModelAnimations item in models)
				ModelFile.CreateFile(item.Filename, item.Model, item.Animations.ToArray(), null, null, item.Name, "splitDLL",
					null, item.Format);
			IniFile.Serialize(output, Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(datafilename))
				+ "_data.ini");
			timer.Stop();
			Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
			Console.WriteLine();
		}

		[DllImport("shlwapi.dll", SetLastError = true)]
		private static extern bool PathRelativePathTo(System.Text.StringBuilder pszPath,
			string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);
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
		public SonicRetro.SAModel.Object Model { get; private set; }
		public ModelFormat Format { get; private set; }
		public List<string> Animations { get; private set; }

		public ModelAnimations(string filename, string name, SonicRetro.SAModel.Object model, ModelFormat format)
		{
			Filename = filename;
			Name = name;
			Model = model;
			Format = format;
			Animations = new List<string>();
		}
	}

	public class MyClass
	{
		public DictionaryContainer ItemTypes { get; set; }
		public DictionaryContainer Files { get; set; }
		public DictionaryContainer Labels { get; set; }

		public MyClass()
		{
			ItemTypes = new DictionaryContainer();
			Files = new DictionaryContainer();
			Labels = new DictionaryContainer();
		}
	}

	public class DictionaryContainer
	{
		[IniCollection]
		public Dictionary<string, string> Items { get; set; }

		public DictionaryContainer()
		{
			Items = new Dictionary<string, string>();
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
		public int AddressOfFunctions;     // RVA from base of image
		public int AddressOfNames;         // RVA from base of image
		public int AddressOfNameOrdinals;  // RVA from base of image
	}
}