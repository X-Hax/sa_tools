using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SA_Tools;
using SA_Tools.SplitDLL;
using System.Globalization;

namespace SonicRetro.SAModel.SAEditorCommon.DLLModGenerator
{
	public static class DLLModGen
	{
		static readonly Dictionary<string, string> typemap = new Dictionary<string, string>() {
			{ "landtable", "LandTable *" },
			{ "landtablearray", "LandTable **" },
			{ "model", "NJS_OBJECT *" },
			{ "modelarray", "NJS_OBJECT **" },
			{ "basicmodel", "NJS_OBJECT *" },
			{ "basicmodelarray", "NJS_OBJECT **" },
			{ "basicdxmodel", "NJS_OBJECT *" },
			{ "basicdxmodelarray", "NJS_OBJECT **" },
			{ "chunkmodel", "NJS_OBJECT *" },
			{ "chunkmodelarray", "NJS_OBJECT **" },
			{ "chunkattach", "NJS_CNK_MODEL **" },
			{ "gcmodel", "NJS_OBJECT *" },
			{ "gcmodelarray", "NJS_OBJECT **" },
			{ "gcattach", "SA2B_MODEL **" },
			{ "actionarray", "NJS_ACTION **" },
			{ "motionarray", "NJS_MOTION **" },
			{ "morph", "NJS_MODEL_SADX *" },
			{ "modelsarray", "NJS_MODEL_SADX **" },
			{ "texlist", "NJS_TEXLIST *" },
			{ "texlistarray", "NJS_TEXLIST **" },
			{ "animindexlist", "AnimationIndex *" }
		};

		private static void CheckItems(DllDataItemInfo item, DllIniData iniData, ref Dictionary<string, bool> defaultExportState)
		{
			bool modified = false;
			switch (item.Type)
			{
				case "animindexlist":
					{
						Dictionary<int, string> hashes = new Dictionary<int, string>();
						foreach (var hash in item.MD5Hash.Split('|').Select(a =>
						{
							string[] b = a.Split(':');
							return (int.Parse(b[0], NumberFormatInfo.InvariantInfo), b[1]);
						}))
							hashes.Add(hash.Item1, hash.Item2);
						foreach (var fn in Directory.GetFiles(item.Filename, "*.saanim"))
							if (int.TryParse(Path.GetFileNameWithoutExtension(fn), out int i))
							{
								if (!hashes.ContainsKey(i) || HelperFunctions.FileHash(fn) != hashes[i])
								{
									modified = true;
									break;
								}
								hashes.Remove(i);
							}
						if (hashes.Count > 0)
							modified = true;
					}
					break;
				case "charaobjectdatalist":
					{
						Dictionary<string, string> hashes = new Dictionary<string, string>();
						foreach (var hash in item.MD5Hash.Split('|').Select(a =>
						{
							string[] b = a.Split(':');
							return (b[0], b[1]);
						}))
							hashes.Add(hash.Item1, hash.Item2);
						foreach (var fp in Directory.GetFiles(item.Filename, "*.sa2mdl").Concat(Directory.GetFiles(item.Filename, "*.saanim")).Append(Path.Combine(item.Filename, "info.ini")))
						{
							string fn = Path.GetFileName(fp);
							if (!hashes.ContainsKey(fn) || HelperFunctions.FileHash(fp) != hashes[fn])
							{
								modified = true;
								break;
							}
							hashes.Remove(fn);
						}
						if (hashes.Count > 0)
							modified = true;
					}
					break;
				case "kartspecialinfolist":
					{
						Dictionary<string, string> hashes = new Dictionary<string, string>();
						foreach (var hash in item.MD5Hash.Split('|').Select(a =>
						{
							string[] b = a.Split(':');
							return (b[0], b[1]);
						}))
							hashes.Add(hash.Item1, hash.Item2);
						foreach (var fp in Directory.GetFiles(item.Filename, "*.sa2mdl").Append(Path.Combine(item.Filename, "info.ini")))
						{
							string fn = Path.GetFileName(fp);
							if (!hashes.ContainsKey(fn) || HelperFunctions.FileHash(fp) != hashes[fn])
							{
								modified = true;
								break;
							}
							hashes.Remove(fn);
						}
						if (hashes.Count > 0)
							modified = true;
					}
					break;
				case "kartmodelsarray":
					{
						Dictionary<string, string> hashes = new Dictionary<string, string>();
						foreach (var hash in item.MD5Hash.Split('|').Select(a =>
						{
							string[] b = a.Split(':');
							return (b[0], b[1]);
						}))
							hashes.Add(hash.Item1, hash.Item2);
						foreach (var fp in Directory.GetFiles(item.Filename, "*.sa2bmdl").Concat(Directory.GetFiles(item.Filename, "*.sa1mdl")).Append(Path.Combine(item.Filename, "info.ini")))
						{
							string fn = Path.GetFileName(fp);
							if (!hashes.ContainsKey(fn) || HelperFunctions.FileHash(fp) != hashes[fn])
							{
								modified = true;
								break;
							}
							hashes.Remove(fn);
						}
						if (hashes.Count > 0)
							modified = true;
					}
					break;
				case "motiontable":
					{
						Dictionary<string, string> hashes = new Dictionary<string, string>();
						foreach (var hash in item.MD5Hash.Split('|').Select(a =>
						{
							string[] b = a.Split(':');
							return (b[0], b[1]);
						}))
							hashes.Add(hash.Item1, hash.Item2);
						foreach (var fp in Directory.GetFiles(item.Filename, "*.saanim").Append(Path.Combine(item.Filename, "info.ini")))
						{
							string fn = Path.GetFileName(fp);
							if (!hashes.ContainsKey(fn) || HelperFunctions.FileHash(fp) != hashes[fn])
							{
								modified = true;
								break;
							}
							hashes.Remove(fn);
						}
						if (hashes.Count > 0)
							modified = true;
					}
					break;
			}
			defaultExportState.Add(item.Filename, modified);
		}

		public static DllIniData LoadINI(string fileName,
			ref Dictionary<string, bool> defaultExportState)
		{
			defaultExportState.Clear();

			DllIniData IniData = IniSerializer.Deserialize<DllIniData>(fileName);

			Environment.CurrentDirectory = Path.GetDirectoryName(fileName);

			foreach (KeyValuePair<string, FileTypeHash> item in IniData.Files)
			{
				bool modified = HelperFunctions.FileHash(item.Key) != item.Value.Hash;
				defaultExportState.Add(item.Key, modified);
			}

			foreach (DllDataItemInfo item in IniData.DataItems)
			{
				CheckItems(item, IniData, ref defaultExportState);
			}

			return IniData;
		}

		public static DllIniData LoadMultiINI(List<string> fileName,
			ref Dictionary<string, bool> defaultExportState)
		{
			defaultExportState.Clear();
			DllIniData newIniData = new DllIniData();
			List<DllDataItemInfo> curItems = new List<DllDataItemInfo>();

			foreach (string arrFile in fileName)
			{
				DllIniData IniData = IniSerializer.Deserialize<DllIniData>(arrFile);

				Environment.CurrentDirectory = Path.GetDirectoryName(arrFile);

				foreach (KeyValuePair<string, FileTypeHash> item in IniData.Files)
				{
					bool modified = HelperFunctions.FileHash(item.Key) != item.Value.Hash;
					defaultExportState.Add(item.Key, modified);
				}

				foreach (DllDataItemInfo item in IniData.DataItems)
				{
					CheckItems(item, IniData, ref defaultExportState);

					curItems.Add(item);
				}
			}
			newIniData.DataItems = curItems;

			return newIniData;
		}

		private static void CopyDirectory(DirectoryInfo srcdir, string dstdir)
		{
			foreach (var d in srcdir.GetDirectories())
			{
				string nd = Path.Combine(dstdir, d.Name);
				Directory.CreateDirectory(nd);
				CopyDirectory(d, nd);
			}
			foreach (var f in srcdir.GetFiles())
			{
				f.CopyTo(Path.Combine(dstdir, f.Name), true);
			}
		}

		public static void ExportINI(DllIniData IniData,
			Dictionary<string, bool> itemsToExport, string fileName)
		{
			string dstfol = Path.GetDirectoryName(fileName);
			DllIniData output = new DllIniData()
			{
				Name = IniData.Name,
				Game = IniData.Game,
				Exports = IniData.Exports,
				TexLists = IniData.TexLists
			};
			List<string> labels = new List<string>();
			foreach (KeyValuePair<string, FileTypeHash> item in
				IniData.Files.Where(i => itemsToExport[i.Key]))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(dstfol, item.Key)));
				File.Copy(item.Key, Path.Combine(dstfol, item.Key), true);
				switch (item.Value.Type)
				{
					case "landtable":
						LandTable tbl = LandTable.LoadFromFile(item.Key);
						labels.AddRange(tbl.GetLabels());
						break;
					case "model":
					case "basicmodel":
					case "chunkmodel":
					case "gcmodel":
					case "basicdxmodel":
						NJS_OBJECT mdl = new ModelFile(item.Key).Model;
						labels.AddRange(mdl.GetLabels());
						break;
					case "animation":
						NJS_MOTION ani = NJS_MOTION.Load(item.Key);
						labels.Add(ani.Name);
						break;
				}
				output.Files.Add(item.Key, new FileTypeHash(item.Value.Type, null));
			}
			output.Items = new List<DllItemInfo>(IniData.Items.Where(a => labels.Contains(a.Label)));
			foreach (var item in IniData.DataItems.Where(i => itemsToExport[i.Filename]))
			{
				Directory.CreateDirectory(Path.Combine(dstfol, item.Filename));
				CopyDirectory(new DirectoryInfo(item.Filename), Path.Combine(dstfol, item.Filename));
				output.DataItems.Add(item);
			}
			IniSerializer.Serialize(output, fileName);
		}

		public static void ExportCPP(DllIniData IniData,
			Dictionary<string, bool> itemsToExport, string fileName)
		{
			using (TextWriter writer = File.CreateText(fileName))
			{
				bool SA2 = IniData.Game == SA_Tools.SplitDLL.Game.SA2B;
				ModelFormat modelfmt = SA2 ? ModelFormat.Chunk : ModelFormat.BasicDX;
				LandTableFormat landfmt = SA2 ? LandTableFormat.SA2 : LandTableFormat.SADX;
				writer.WriteLine("// Generated by SA Tools DLL Mod Generator");
				writer.WriteLine();
				if (SA2)
					writer.WriteLine("#include \"SA2ModLoader.h\"");
				else
					writer.WriteLine("#include \"SADXModLoader.h\"");
				writer.WriteLine();
				List<string> labels = new List<string>();
				Dictionary<string, uint> texlists = new Dictionary<string, uint>();
				foreach (KeyValuePair<string, FileTypeHash> item in
					IniData.Files.Where(i => itemsToExport[i.Key]))
				{
					switch (item.Value.Type)
					{
						case "landtable":
							LandTable tbl = LandTable.LoadFromFile(item.Key);
							texlists.Add(tbl.Name, tbl.TextureList);
							tbl.ToStructVariables(writer, landfmt, new List<string>());
							labels.AddRange(tbl.GetLabels());
							break;
						case "model":
							NJS_OBJECT mdl = new ModelFile(item.Key).Model;
							mdl.ToStructVariables(writer, modelfmt == ModelFormat.BasicDX, new List<string>());
							labels.AddRange(mdl.GetLabels());
							break;
						case "basicmodel":
						case "chunkmodel":
						case "gcmodel":
							mdl = new ModelFile(item.Key).Model;
							mdl.ToStructVariables(writer, false, new List<string>());
							labels.AddRange(mdl.GetLabels());
							break;
						case "basicdxmodel":
							mdl = new ModelFile(item.Key).Model;
							mdl.ToStructVariables(writer, true, new List<string>());
							labels.AddRange(mdl.GetLabels());
							break;
						case "animation":
							NJS_MOTION ani = NJS_MOTION.Load(item.Key);
							ani.ToStructVariables(writer);
							labels.Add(ani.Name);
							break;
					}
					writer.WriteLine();
				}
				foreach (var item in IniData.DataItems.Where(i => itemsToExport[i.Filename]))
					switch (item.Type)
					{
						case "animindexlist":
							{
								SortedDictionary<short, NJS_MOTION> anims = new SortedDictionary<short, NJS_MOTION>();
								foreach (string file in Directory.GetFiles(item.Filename, "*.saanim"))
									if (short.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out short i))
										anims.Add(i, NJS_MOTION.Load(file));
								foreach (KeyValuePair<short, NJS_MOTION> obj in anims)
								{
									obj.Value.ToStructVariables(writer);
									writer.WriteLine();
								}
								writer.WriteLine("AnimationIndex {0}[] = {{", item.Export);
								List<string> objs = new List<string>(anims.Count);
								foreach (KeyValuePair<short, NJS_MOTION> obj in anims)
									objs.Add($"{{ {obj.Key}, {obj.Value.ModelParts}, &{obj.Value.Name} }}");
								objs.Add("{ -1 }");
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "charaobjectdatalist":
							{
								foreach (string file in Directory.GetFiles(item.Filename, "*.sa2mdl"))
								{
									new ModelFile(file).Model.ToStructVariables(writer, false, new List<string>());
									writer.WriteLine();
								}
								foreach (string file in Directory.GetFiles(item.Filename, "*.saanim"))
								{
									NJS_MOTION.Load(file).ToStructVariables(writer);
									writer.WriteLine();
								}
								var data = IniSerializer.Deserialize<CharaObjectData[]>(Path.Combine(item.Filename, "info.ini"));
								writer.WriteLine("CharaObjectData {0}[] = {{", item.Export);
								List<string> objs = new List<string>(data.Length);
								foreach (var obj in data)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "kartspecialinfolist":
							{
								foreach (string file in Directory.GetFiles(item.Filename, "*.sa2mdl"))
								{
									new ModelFile(file).Model.ToStructVariables(writer, false, new List<string>());
									writer.WriteLine();
								}
								var data = IniSerializer.Deserialize<KartSpecialInfo[]>(Path.Combine(item.Filename, "info.ini"));
								writer.WriteLine("KartSpecialInfo {0}[] = {{", item.Export);
								List<string> objs = new List<string>(data.Length);
								for (int i = 0; i < data.Length; i++)
								{
									KartSpecialInfo obj = data[i];
									objs.Add(obj.ToStruct());
									texlists.Add($"{item.Export}[{i}]", obj.TexList);
								}
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "kartmodelsarray":
							{
								foreach (string file in Directory.GetFiles(item.Filename, "*.sa2bmdl"))
								{
									new ModelFile(file).Model.ToStructVariables(writer, false, new List<string>());
									writer.WriteLine();
								}
								foreach (string file in Directory.GetFiles(item.Filename, "*.sa1mdl"))
								{
									new ModelFile(file).Model.ToStructVariables(writer, false, new List<string>());
									writer.WriteLine();
								}
								var data = IniSerializer.Deserialize<CharaObjectData[]>(Path.Combine(item.Filename, "info.ini"));
								writer.WriteLine("KartModelsArray {0}[] = {{", item.Export);
								List<string> objs = new List<string>(data.Length);
								foreach (var obj in data)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
						case "motiontable":
							{
								foreach (string file in Directory.GetFiles(item.Filename, "*.saanim"))
								{
									NJS_MOTION.Load(file).ToStructVariables(writer);
									writer.WriteLine();
								}
								var data = IniSerializer.Deserialize<MotionTableEntry[]>(Path.Combine(item.Filename, "info.ini"));
								writer.WriteLine("MotionTableEntry {0}[] = {{", item.Export);
								List<string> objs = new List<string>(data.Length);
								foreach (var obj in data)
									objs.Add(obj.ToStruct());
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
							}
							break;
					}
				writer.WriteLine("extern \"C\" __declspec(dllexport) void __cdecl Init(const char *path, const HelperFunctions &helperFunctions)");
				writer.WriteLine("{");
				writer.WriteLine("\tHMODULE handle = GetModuleHandle(L\"{0}\");", IniData.Name);
				List<string> exports = new List<string>(IniData.Items.Where(item => labels.Contains(item.Label)).Select(item => item.Export).Distinct());
				foreach (KeyValuePair<string, string> item in IniData.Exports.Where(item => exports.Contains(item.Key)))
					writer.WriteLine("\t{0}{1} = ({0})GetProcAddress(handle, \"{1}\");", typemap[item.Value], item.Key);
				foreach (DllItemInfo item in IniData.Items.Where(item => labels.Contains(item.Label)))
					if (typemap[IniData.Exports[item.Export]].EndsWith("**"))
						writer.WriteLine("\t{0} = &{1};", item.ToString(), item.Label);
					else
						writer.WriteLine("\t*{0} = {1};", item.ToString(), item.Label);
				foreach (var item in IniData.DataItems.Where(item => itemsToExport[item.Filename]))
					switch (item.Type)
					{
						case "animindexlist":
						case "charaobjectdatalist":
						case "kartspecialinfolist":
						case "kartmodelsarray":
							writer.WriteLine("\tHookExport(handle, \"{0}\", {0});", item.Export);
							break;
						default:
							writer.WriteLine("\t{0}{1}_exp = ({0})GetProcAddress(handle, \"{1}\");", typemap[item.Type], item.Export);
							writer.WriteLine("\t*{0}_exp = {0};", item.Export);
							break;
					}
				if (texlists.Count > 0 && IniData.TexLists != null && IniData.TexLists.Items != null)
				{
					exports = new List<string>(IniData.TexLists.Where(item => texlists.Values.Contains(item.Key)).Select(item => item.Value.Export).Distinct());
					foreach (KeyValuePair<string, string> item in IniData.Exports.Where(item => exports.Contains(item.Key)))
						writer.WriteLine("\t{0}{1} = ({0})GetProcAddress(handle, \"{1}\");", typemap[item.Value], item.Key);
					foreach (KeyValuePair<string, uint> item in texlists.Where(item => IniData.TexLists.ContainsKey(item.Value)))
					{
						DllTexListInfo tex = IniData.TexLists[item.Value];
						string str;
						if (tex.Index.HasValue)
							str = $"{tex.Export}[{tex.Index.Value}]";
						else
							str = tex.Export;
						writer.WriteLine("\t{0}.TexList = {1};", item.Key, str);
					}
				}
				writer.WriteLine("}");
				writer.WriteLine();
				writer.WriteLine("extern \"C\" __declspec(dllexport) const ModInfo {0}ModInfo = {{ ModLoaderVer }};", SA2 ? "SA2" : "SADX");
			}
		}
	}
}
