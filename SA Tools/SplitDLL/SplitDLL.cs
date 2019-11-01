using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using SA_Tools;
using SonicRetro.SAModel;

namespace SA_Tools.SplitDLL
{
	public static class SplitDLL
	{
		public static int SplitDLLFile(string datafilename, string inifilename, string projectFolderName)
		{
#if !DEBUG
			try
			{
#endif
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
						landfmt = LandTableFormat.SA2;
						modelext = ".sa2mdl";
						landext = ".sa2lvl";
						break;
				}
				int itemcount = 0;
				List<string> labels = new List<string>();
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
						fileOutputPath = string.Concat(projectFolderName, data.Filename);

						Console.WriteLine(name + " -> " + fileOutputPath);
						Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
					}
					else
						Console.WriteLine(name);
					switch (type)
					{
						case "landtable":
							{
								LandTable land = new LandTable(datafile, address, imageBase, landfmt) { Description = name };
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = land.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(land.Name))
								{
									land.SaveToFile(fileOutputPath, landfmt);
									output.Files[data.Filename] = new FileTypeHash("landtable", HelperFunctions.FileHash(fileOutputPath));
									labels.AddRange(land.GetLabels());
								}
							}
							break;
						case "battlelandtable":
							{
								LandTable land = new LandTable(datafile, address, imageBase, LandTableFormat.SA2B) { Description = name };
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = land.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(land.Name))
								{
									land.SaveToFile(fileOutputPath, LandTableFormat.SA2B);
									output.Files[data.Filename] = new FileTypeHash("landtable", HelperFunctions.FileHash(fileOutputPath));
									labels.AddRange(land.GetLabels());
								}
							}
							break;
						case "landtablearray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									LandTable land = new LandTable(datafile, ptr, imageBase, landfmt) { Description = idx };
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = land.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(land.Name))
									{
										string outputFN = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + landext);
										string fileName = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + landext);
										
										land.SaveToFile(outputFN, landfmt);
										output.Files[fileName] = new FileTypeHash("landtable", HelperFunctions.FileHash(outputFN));
										labels.AddRange(land.GetLabels());
									}
								}
								address += 4;
							}
							break;
						case "model":
							{
								NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, modelfmt);
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = mdl.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(mdl.Name))
								{
									models.Add(new ModelAnimations(data.Filename, name, mdl, modelfmt));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;
						case "morph":
							{
								BasicAttach dummy = new BasicAttach(datafile, address, imageBase, modelfmt == ModelFormat.BasicDX);
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
									models.Add(new ModelAnimations(data.Filename, name, mdl, modelfmt));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;
						case "modelarray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									NJS_OBJECT mdl = new NJS_OBJECT(datafile, ptr, imageBase, modelfmt);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = mdl.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(mdl.Name))
									{
										string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
										models.Add(new ModelAnimations(fn, idx, mdl, modelfmt));
										labels.AddRange(mdl.GetLabels());
									}
								}
								address += 4;
							}
							break;
						case "modelsarray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									BasicAttach dummy = new BasicAttach(datafile, ptr, imageBase, modelfmt == ModelFormat.BasicDX);
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
									if (!labels.Contains(dummy.Name))
									{
										string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + modelext);
										models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.BasicDX));
										labels.AddRange(mdl.GetLabels());
									}
								}
								address += 4;
							}
							break;
						case "basicmodel":
							{
								NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.Basic);
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = mdl.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(mdl.Name))
								{
									models.Add(new ModelAnimations(data.Filename, name, mdl, ModelFormat.Basic));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;
						case "basicmodelarray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									NJS_OBJECT mdl = new NJS_OBJECT(datafile, ptr, imageBase, ModelFormat.Basic);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = mdl.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(mdl.Name))
									{
										string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl");
										models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.Basic));
										labels.AddRange(mdl.GetLabels());
									}
								}
								address += 4;
							}
							break;
						case "basicdxmodel":
							{
								NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.BasicDX);
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = mdl.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(mdl.Name))
								{
									models.Add(new ModelAnimations(data.Filename, name, mdl, ModelFormat.BasicDX));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;
						case "basicdxmodelarray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									NJS_OBJECT mdl = new NJS_OBJECT(datafile, ptr, imageBase, ModelFormat.BasicDX);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = mdl.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(mdl.Name))
									{
										string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl");
										models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.BasicDX));
										labels.AddRange(mdl.GetLabels());
									}
								}
								address += 4;
							}
							break;
						case "chunkmodel":
							{
								NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, ModelFormat.Chunk);
								DllItemInfo info = new DllItemInfo()
								{
									Export = name,
									Label = mdl.Name
								};
								output.Items.Add(info);
								if (!labels.Contains(mdl.Name))
								{
									models.Add(new ModelAnimations(data.Filename, name, mdl, ModelFormat.Chunk));
									labels.AddRange(mdl.GetLabels());
								}
							}
							break;
						case "chunkmodelarray":
							for (int i = 0; i < data.Length; i++)
							{
								int ptr = BitConverter.ToInt32(datafile, address);
								if (ptr != 0)
								{
									ptr = (int)(ptr - imageBase);
									NJS_OBJECT mdl = new NJS_OBJECT(datafile, ptr, imageBase, ModelFormat.Chunk);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = mdl.Name
									};
									output.Items.Add(info);
									if (!labels.Contains(mdl.Name))
									{
										string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl");
										models.Add(new ModelAnimations(fn, idx, mdl, ModelFormat.Chunk));
										labels.AddRange(mdl.GetLabels());
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
									NJS_ACTION ani = new NJS_ACTION(datafile, ptr, imageBase, modelfmt);
									string idx = name + "[" + i.ToString(NumberFormatInfo.InvariantInfo) + "]";
									ani.Animation.Name = item.Key + "_" + i;
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = i,
										Label = ani.Animation.Name,
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
									string outputFN = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
									string fn = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
									ani.Animation.Save(outputFN);
									output.Files[fn] = new FileTypeHash("animation", HelperFunctions.FileHash(outputFN));
									if (models.Contains(ani.Model.Name))
									{
										ModelAnimations mdl = models[ani.Model.Name];
										System.Text.StringBuilder sb = new System.Text.StringBuilder(260);
										PathRelativePathTo(sb, Path.GetFullPath(Path.Combine(projectFolderName, mdl.Filename)), 0, Path.GetFullPath(outputFN), 0);						 
										mdl.Animations.Add(sb.ToString()); // this is where the problem is
									}
									else
									{
										string mfn = Path.ChangeExtension(fn, modelext);
										string outputmfn = Path.Combine(projectFolderName, mfn);
										string animationName = Path.GetFileName(outputFN);

										ModelFile.CreateFile(outputmfn, ani.Model, new[] { animationName }, null, idx + "->object",
											null, modelfmt);
										output.Files[mfn] = new FileTypeHash("model", HelperFunctions.FileHash(outputmfn));
									}
								}
								address += 4;
							}
							break;
						case "texlist":
							if (output.TexLists == null)
								output.TexLists = new TexListContainer();
							output.TexLists.Add((uint)(address + imageBase), new DllTexListInfo(name, null));
							break;
						case "texlistarray":
							if (output.TexLists == null)
								output.TexLists = new TexListContainer();
							for (int i = 0; i < data.Length; i++)
							{
								uint ptr = BitConverter.ToUInt32(datafile, address);
								if (ptr != 0 && !output.TexLists.ContainsKey(ptr))
									output.TexLists.Add(ptr, new DllTexListInfo(name, i));
								address += 4;
							}
							break;
						case "animindexlist":
							{
								int c = 0;
								int i = ByteConverter.ToInt16(datafile, address);
								while (i != -1)
								{
									string outputFN = Path.Combine(fileOutputPath, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
									string fileName = Path.Combine(data.Filename, i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");

									NJS_MOTION anim = new NJS_MOTION(datafile, datafile.GetPointer(address + 4, imageBase), imageBase, ByteConverter.ToInt16(datafile, address + 2));
									DllItemInfo info = new DllItemInfo()
									{
										Export = name,
										Index = c,
										Field = "Animation",
										Label = anim.Name
									};
									anim.Save(outputFN);
									output.Files[fileName] = new FileTypeHash("animindex", HelperFunctions.FileHash(outputFN));
									address += 8;
									i = ByteConverter.ToInt16(datafile, address);
									++c;
								}
							}
							break;
					}
					itemcount++;
				}
				foreach (ModelAnimations item in models)
				{
					string modelOutputPath = string.Concat(projectFolderName, item.Filename);
					//string modelOutputPath = item.Filename;

					ModelFile.CreateFile(modelOutputPath, item.Model, item.Animations.ToArray(), null, item.Name, null, item.Format);
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
					}
					output.Files[item.Filename] = new FileTypeHash(type, HelperFunctions.FileHash(modelOutputPath));
				}
				IniSerializer.Serialize(output, Path.Combine(projectFolderName, Path.GetFileNameWithoutExtension(datafilename))
					+ "_data.ini");
				timer.Stop();
				Console.WriteLine("Split " + itemcount + " items in " + timer.Elapsed.TotalSeconds + " seconds.");
				Console.WriteLine();
#if !DEBUG
		}
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
