using SA_Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace Split
{
	class Program
	{
		static int CompareFiles(string file1, string file2)
		{
			int result = 0;
			int length1 = (int)new System.IO.FileInfo(file1).Length;
			int length2 = (int)new System.IO.FileInfo(file2).Length;
			result += Math.Abs(length1 - length2);
			if (Path.GetExtension(file1).ToLowerInvariant() != ".sa1lvl" && result > 64) return result;
			byte[] model1 = File.ReadAllBytes(file1);
			byte[] model2 = File.ReadAllBytes(file2);
			for (int i = 0; i < Math.Min(model1.Length, model2.Length); i++)
			{
				if (model1[i] != model2[i]) result++;
			}
			int abs = Math.Abs(model1.Length - model2.Length);
			if (abs != 0) result += abs;
			return result;
		}
		static void Main(string[] args)
		{
			bool nometa = false;
			if (args.Length > 0 && args[args.Length - 1] == "-m")
			{
				SplitM(args);
				return;
			}
			else if (args.Length > 0 && args[args.Length - 1] == "-l")
			{
				SplitL(args);
				return;
			}
			else if (args.Length > 0 && args[args.Length - 1] == "-c")
			{
				SplitC(args);
				return;
			}
			string mode;
			string fullpath_out;
			bool bigendian = false;
			List<string> mdlanimfiles;
			if (args.Length == 0)
			{
				Console.WriteLine("Split any binary files supported by SA Tools.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("-Splitting binary files with INI data-");
				Console.WriteLine("split binary <file> <inifile> [output path] [-nometa]\n");
				Console.WriteLine("-Splitting SA1/SADX NB files-");
				Console.WriteLine("split nb <file> [output path]\n");
				Console.WriteLine("-Splitting SA2 MDL files-");
				Console.WriteLine("split mdl <file> [output path] -anim [animation files]\n");
				Console.WriteLine("-Splitting SA2B MDL files-");
				Console.WriteLine("split mdl_b <file> [output path] -anim [animation files]\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			else
			{
				for (int u = 2; u < args.Length; u++)
				{
					if (args[u] == "-nometa") nometa = true;
				}
				mode = args[0];
				switch (mode.ToLowerInvariant())
				{
					case "binary":
						string fullpath_bin = Path.GetFullPath(args[1]);
						if (!File.Exists(fullpath_bin))
						{
							Console.WriteLine("File {0} doesn't exist.", fullpath_bin);
							return;
						}
						Console.WriteLine("File: {0}", fullpath_bin);
						string fullpath_ini = Path.GetFullPath(args[2]);
						if (!File.Exists(fullpath_ini))
						{
							Console.WriteLine("File {0} doesn't exist.", fullpath_ini);
							return;
						}
						Console.WriteLine("Data mapping: {0}", fullpath_ini);
						fullpath_out = Path.GetDirectoryName(fullpath_bin);
						if (args.Length > 3)
						{
							fullpath_out = args[3];
							if (fullpath_out[fullpath_out.Length - 1] != '/') fullpath_out = string.Concat(fullpath_out, '/');
							fullpath_out = Path.GetFullPath(fullpath_out);
						}
						Console.WriteLine("Output folder: {0}", fullpath_out);
						if (nometa) Console.WriteLine("Labels are disabled");
						if (Path.GetExtension(args[1]).ToLowerInvariant() == ".dll")
							SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(fullpath_bin, fullpath_ini, fullpath_out, nometa);
						else SA_Tools.Split.Split.SplitFile(fullpath_bin, fullpath_ini, fullpath_out, nometa);
						break;
					case "nb":
					case "nb_b":
						string fullpath_nb = Path.GetFullPath(args[1]);
						if (!File.Exists(fullpath_nb))
						{
							Console.WriteLine("File {0} doesn't exist.", fullpath_nb);
						}
						Console.WriteLine("File: {0}", fullpath_nb);
						fullpath_out = Path.GetDirectoryName(fullpath_nb);
						if (args.Length > 2)
						{
							fullpath_out = args[2];
							if (fullpath_out[fullpath_out.Length - 1] != '/') fullpath_out = string.Concat(fullpath_out, '/');
							fullpath_out = Path.GetFullPath(fullpath_out);
						}
						Console.WriteLine("Output folder: {0}", fullpath_out);
						SA_Tools.Split.SplitNB.SplitNBFile(fullpath_nb, false, fullpath_out, true);
						break;
					case "mdl":
					case "mdl_b":
						string fullpath_mdl = Path.GetFullPath(args[1]);
						if (!File.Exists(fullpath_mdl))
						{
							Console.WriteLine("File {0} doesn't exist.", fullpath_mdl);
						}
						Console.Write("File: {0}", fullpath_mdl);
						if (mode == "mdl_b")
						{
							bigendian = true;
							Console.Write(" (Big Endian)\n");
						}
						else
							Console.Write(System.Environment.NewLine);
						fullpath_out = Path.GetDirectoryName(fullpath_mdl);
						if (args.Length > 2)
						{
							fullpath_out = args[2];
							if (fullpath_out[fullpath_out.Length - 1] != '/') fullpath_out = string.Concat(fullpath_out, '/');
							fullpath_out = Path.GetFullPath(fullpath_out);
						}
						Console.WriteLine("Output path: {0}", fullpath_out);
						if (args.Length > 3)
						{
							mdlanimfiles = new List<string>();
							Console.WriteLine("Animation files:");
							for (int u = 3; u < args.Length; u++)
							{
								string animpath = Path.GetFullPath(args[u]);
								if (File.Exists(animpath))
								{
									mdlanimfiles.Add(animpath);
									Console.WriteLine(animpath);
								}
								else
									Console.WriteLine("File {0} doesn't exist.", animpath);
							}
							SA_Tools.SplitMDL.SplitMDL.Split(bigendian, fullpath_mdl, fullpath_out, mdlanimfiles.ToArray());
						}
						else
							SA_Tools.SplitMDL.SplitMDL.Split(bigendian, fullpath_mdl, fullpath_out, null);
						break;
					default:
						Console.WriteLine("Incorrect mode specified. Press ENTER to exit.");
						Console.ReadLine();
						return;
				}
			}
		}
		static void SplitM(string[] args)
		{
			Dictionary<string, string> matchstrings;
			string listname;
			string inifilename;
			string splitfilename;
			string srcext;
			List<string> fnd = new List<string>();
			bool match;
			if (args.Length == 1)
			{
				return;
			}
			else
			{
				TextWriter matchlist = File.CreateText("match.txt");
				TextWriter nomatchlist = File.CreateText("nomatch_source.txt");
				TextWriter nomatchlist_l = File.CreateText("nomatch_label.txt");
				listname = args[0];
				inifilename = args[1];
				IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
				matchstrings = new Dictionary<string, string>();
				string[] list = File.ReadAllLines(listname);
				for (int l = 0; l < list.Length; l++)
				{
					string filename = Path.GetFullPath(list[l]);
					match = false;
					foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo in inifile.Files)
					{
						if (fnd.Contains(fileinfo.Key)) continue;
						splitfilename = Path.GetFileNameWithoutExtension(fileinfo.Value.Filename);
						//Console.WriteLine("Processing split: {0}", splitfilename);
						//Try the label as-is first
						if (Path.GetFileNameWithoutExtension(splitfilename).ToLowerInvariant().StartsWith(Path.GetFileNameWithoutExtension(filename).ToLowerInvariant()))
						{
							//Console.WriteLine("Match full");
							match = true;
						}
						//Fix "_object", "_action" etc.
						else if (splitfilename.StartsWith("_"))
						{
							splitfilename = splitfilename.Substring(1, splitfilename.Length - 1);
							if (Path.GetFileNameWithoutExtension(splitfilename).ToLowerInvariant().StartsWith(Path.GetFileNameWithoutExtension(filename).ToLowerInvariant()))
							{
								//Console.WriteLine("Match without _");
								match = true;
							}
						}
						//Try without "object_", "action_" etc.
						else
						{
							switch (fileinfo.Value.Type)
							{
								case "landtable":
									splitfilename = Path.GetFileNameWithoutExtension(fileinfo.Value.Filename).Replace("obj", "").ToLowerInvariant();
									srcext = ".c";
									break;
								case "chunkmodel":
									splitfilename = Path.GetFileNameWithoutExtension(fileinfo.Value.Filename).Replace("object_", "");
									srcext = ".nja";
									break;
								case "action":
									splitfilename = Path.GetFileNameWithoutExtension(fileinfo.Value.Filename).Replace("action_", "");
									srcext = ".nam";
									break;
								case "animation":
									splitfilename = Path.GetFileNameWithoutExtension(fileinfo.Value.Filename).Replace("motion_", "");
									srcext = ".nam";
									break;
								case "basicdxmodel":
								case "basicmodel":
								case "model":
								default:
									splitfilename = Path.GetFileNameWithoutExtension(fileinfo.Value.Filename).Replace("object_", "");
									srcext = ".nja";
									break;
							}
							if ((Path.GetExtension(filename).ToLowerInvariant() == srcext || Path.GetExtension(filename).ToLowerInvariant() == ".dup") && Path.GetFileNameWithoutExtension(splitfilename).ToLowerInvariant().StartsWith(Path.GetFileNameWithoutExtension(filename).ToLowerInvariant()))
							{
								//Console.WriteLine("Match without object_");
								match = true;
							}
							else if (splitfilename.StartsWith("_"))
							{
								splitfilename = splitfilename.Substring(1, splitfilename.Length - 1);
								if ((Path.GetExtension(filename).ToLowerInvariant() == srcext || Path.GetExtension(filename).ToLowerInvariant() == ".dup") && Path.GetFileNameWithoutExtension(splitfilename).ToLowerInvariant().StartsWith(Path.GetFileNameWithoutExtension(filename).ToLowerInvariant()))
									match = true;
							}
						}
						if (match)
						{
							Console.WriteLine("{0}:{1}", fileinfo.Value.Filename, filename);
							matchlist.WriteLine("{0}={1}", fileinfo.Value.Filename, filename);
							if (!matchstrings.ContainsKey(fileinfo.Value.Filename)) matchstrings.Add(fileinfo.Value.Filename, filename);
							else Console.WriteLine("Duplicate of {0} detected:{1}:{2}", matchstrings[fileinfo.Value.Filename], fileinfo.Value.Filename, filename);
							matchlist.Flush();
							fnd.Add(fileinfo.Key);
							break;
						}
					}
					if (!match)
					{
						Console.WriteLine("No match for {0}!", Path.GetFileNameWithoutExtension(filename));
						nomatchlist.WriteLine(filename);
						nomatchlist.Flush();
					}
				}
				nomatchlist.Close();
				matchlist.Close();
				foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo in inifile.Files)
				{
					if (!matchstrings.ContainsKey(fileinfo.Value.Filename)) 
					{
						nomatchlist_l.WriteLine(fileinfo.Value.Filename);
						nomatchlist_l.Flush();
					}
				}
				nomatchlist_l.Close();
			}
		}
		static void SplitL(string[] args)
		{
			Dictionary<string, string> matchlist;
			List<string> duplicatecheck = new List<string>();
			string splitext = ".";
			string listname = args[0];
			string inifilename = args[1];
			matchlist = IniSerializer.Deserialize<Dictionary<string, string>>(listname);
			IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
			IniData newinifile = new IniData();
			newinifile.BigEndian = inifile.BigEndian;
			newinifile.Compressed = inifile.Compressed;
			newinifile.Game = inifile.Game;
			newinifile.ImageBase = inifile.ImageBase;
			newinifile.Reverse = inifile.Reverse;
			newinifile.StartOffset = inifile.StartOffset;
			newinifile.Files = new Dictionary<string, SA_Tools.FileInfo>();
			foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo in inifile.Files)
			{
				if (matchlist.ContainsKey(fileinfo.Value.Filename))
				{
					string newfilename = matchlist[fileinfo.Value.Filename];
					if (duplicatecheck.Contains(newfilename))
					{
						Console.WriteLine("Duplicate detected: {0}:{1}", newfilename, fileinfo.Value.Filename);
					}
					else duplicatecheck.Add(newfilename);
					splitext = Path.GetExtension(fileinfo.Value.Filename).ToLowerInvariant();
					SA_Tools.FileInfo newfileinfo = new SA_Tools.FileInfo();
					newfileinfo.Address = fileinfo.Value.Address;
					if (fileinfo.Value.Filename != null) newfileinfo.Filename = newfilename + splitext;
					if (fileinfo.Value.PointerList != null) newfileinfo.PointerList = fileinfo.Value.PointerList;
					if (fileinfo.Value.Type != null) newfileinfo.Type = fileinfo.Value.Type;
					if (fileinfo.Value.MD5Hash != null) newfileinfo.MD5Hash = fileinfo.Value.MD5Hash;
					newfileinfo.CustomProperties = fileinfo.Value.CustomProperties;
					//Fix animation names
					if (fileinfo.Value.CustomProperties != null && fileinfo.Value.CustomProperties.ContainsKey("animations"))
					{
						string[] animlist = fileinfo.Value.CustomProperties["animations"].Split(',');
						List<string> animlist_new = new List<string>();
						for (int u = 0; u < animlist.Length; u++)
						{
							if (matchlist.ContainsKey(animlist[u]))
							{
								//Console.WriteLine("Fixing animation {0}:{1}", animlist[u], Path.GetFileName(matchlist[animlist[u]]));
								animlist_new.Add(Path.GetFileName(matchlist[animlist[u]]) + ".saanim");
							}
							else
							{
								//Console.WriteLine("Adding animation {0}", animlist[u]);
								animlist_new.Add(Path.GetFileNameWithoutExtension(animlist[u]) + ".nam.saanim");
							}
						}
						newfileinfo.CustomProperties["animations"] = string.Join(",", animlist_new.ToArray());
					}
					newinifile.Files.Add(fileinfo.Key, newfileinfo);
				}
			}
			IniSerializer.Serialize(newinifile, Path.GetFileNameWithoutExtension(listname) + ".ini");
		}
		static void SplitC(string[] args)
		{
			if (args.Length < 3)
			{
				return;
			}
			List<string> foundmodels = new List<string>();
			string[] filenames_dir1 = Directory.GetFiles(args[0], "*", SearchOption.AllDirectories);
			string[] filenames_dir2 = Directory.GetFiles(args[1], "*", SearchOption.AllDirectories);
			for (int i = 0; i < filenames_dir1.Length; i++)
			{
				int match = 0;
				int threshold = 0;
				string ext1 = Path.GetExtension(filenames_dir1[i]).ToLowerInvariant();
				if (ext1 == ".sa1lvl") threshold = 128;
				for (int u = 0; u < filenames_dir2.Length; u++)
				{
					string ext2 = Path.GetExtension(filenames_dir2[u]).ToLowerInvariant();
					if (ext1 != ext2) continue;
					int comp = CompareFiles(filenames_dir1[i], filenames_dir2[u]);
					if (comp <= threshold)
					{
						if (match > 0) Console.Write("*");
						if (foundmodels.Contains(filenames_dir2[u])) Console.Write("$");
						Console.Write("{0}={1}", filenames_dir1[i].Replace("\\", "/"), filenames_dir2[u].Replace("\\", "/"));
						match++;
						foundmodels.Add(filenames_dir2[u]);
						Console.Write(System.Environment.NewLine);
					}
				}
				if (match == 0) Console.WriteLine("No match for {0}", filenames_dir1[i]);
			}
		}
	}
}