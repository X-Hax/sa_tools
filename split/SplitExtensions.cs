// Various additional functionality related to the split tool and data mapping available in Debug builds.

using System;
using System.Collections.Generic;
using SA_Tools;
using SonicRetro.SAModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Split
{
	partial class Program
	{
		[DllImport("shlwapi.dll", SetLastError = true)]
		private static extern bool PathRelativePathTo(System.Text.StringBuilder pszPath, string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);
		// Parses command line arguments for advanced split mode
		static bool SplitExtensions(string[] args)
		{
			switch (args[args.Length - 1])
			{
				case "-m":
					SplitM(args);
					return true;
				case "-l":
					SplitL(args);
					return true;
				case "-la":
					SplitL(args, true);
					return true;
				case "-g":
					SplitG(args);
					return true;
				case "-c":
					SplitC(args);
					return true;
				case "-f":
					SplitF(args);
					return true;
				case "-lf":
					SplitLF(args);
					return true;
				case "-rf":
					SplitRF(args);
					return true;
				case "-ld":
					SplitLD(args);
					return true;
				default:
					return false;
			}
		}

		// Compares two files and returns the difference in the number of bytes
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

		// Loads a split INI file and a list of filenames and replaces INI filenames for models and animations with the names specified in the list
		static void SplitRF(string[] args)
		{
			string inifilename = args[0];
			string listfilename = args[1];
			IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
			string[] list = File.ReadAllLines(listfilename);
			int n = 0;
			Dictionary<int, string> filenames = new Dictionary<int, string>();
			foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo in inifile.Files)
			{
				Console.WriteLine("{0}", list[n]);
				fileinfo.Value.Filename = list[n];
				filenames.Add(fileinfo.Value.Address, list[n]);
				n++;
			}
			// Relink animations
			foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo in inifile.Files)
			{
				if (fileinfo.Value.CustomProperties.ContainsKey("animations"))
				{
					string[] anims = fileinfo.Value.CustomProperties["animations"].Split(',');
					for (int a = 0; a < anims.Length; a++)
					{
						string newfilename = filenames[int.Parse(Path.GetFileNameWithoutExtension(anims[a]), System.Globalization.NumberStyles.AllowHexSpecifier)];
						System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
						PathRelativePathTo(sb, Path.GetFullPath(fileinfo.Value.Filename), 0, Path.GetFullPath(newfilename), 0);
						anims[a] = sb.ToString();
					}
					fileinfo.Value.CustomProperties["animations"] = string.Join(",", anims);
				}
			}
			IniSerializer.Serialize(inifile, Path.GetFileNameWithoutExtension(inifilename) + "_replaced.ini");
		}

		// Loads an IniData and returns a sorted version of it, as well as outputs a sorted list of filenames to console
		static Dictionary<string, SA_Tools.FileInfo> SortIniData(Dictionary<string, SA_Tools.FileInfo> fileinfodicc)
		{
			Dictionary<string, SA_Tools.FileInfo> fileinfo_new = new Dictionary<string, SA_Tools.FileInfo>();
			uint lowest = 0xFFFFFFFF;
			List<uint> found = new List<uint>();
			for (int n = 0; n < fileinfodicc.Count; n++)
			{
				//Console.WriteLine(n);
				foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo in fileinfodicc)
				{
					//Console.WriteLine("testing");
					if ((uint)fileinfo.Value.Address < lowest && !found.Contains((uint)fileinfo.Value.Address))
					{
						string ext = Path.GetExtension(fileinfo.Value.Filename).ToLowerInvariant();
						if (ext == ".sa1mdl" || ext == ".saanim" || ext == ".sa1lvl")
							lowest = (uint)fileinfo.Value.Address;
						else if (!fileinfo_new.ContainsKey(fileinfo.Key))
							fileinfo_new.Add(fileinfo.Key, fileinfo.Value);
					}
					//else Console.WriteLine("{0} is not smaller than {1}", fileinfo.Value.Address.ToString("X"), lowest.ToString("X"));
				}
				if (lowest != 0xFFFFFFFF)
				{
					found.Add(lowest);
					//Console.WriteLine(lowest.ToString("X"));
					foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo_x in fileinfodicc)
					{
						if (fileinfo_x.Value.Address == lowest)
						{
							Console.WriteLine(fileinfo_x.Value.Filename);
							if (!fileinfo_new.ContainsKey(fileinfo_x.Value.Address.ToString("X8")))
								fileinfo_new.Add(fileinfo_x.Value.Address.ToString("X8"), fileinfo_x.Value);
							else Console.WriteLine("Duplicate detected!");
						}
					}
					lowest = 0xFFFFFFFF;
				}
			}
			return fileinfo_new;
		}

		// Loads a split INI file and lists all filenames by address in ascending order, as well as saves a sorted INI file
		static void SplitLF(string[] args)
		{
			string inifilename = args[0];
			IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
			inifile.Files = SortIniData(inifile.Files);
			IniSerializer.Serialize(inifile, Path.GetFileNameWithoutExtension(inifilename) + "_sorted.ini");
		}

		// Scans a folder for .sa1lvl, .sa1mdl and .saanim files and creates a sorted split INI file based on their labels and filenames
		static void SplitLD(string[] args)
		{
			string dirpath = args[0];
			string[] modelfiles = System.IO.Directory.GetFiles(dirpath, "*.sa1mdl", SearchOption.AllDirectories);
			string[] levelfiles = System.IO.Directory.GetFiles(dirpath, "*.sa1lvl", SearchOption.AllDirectories);
			string[] animfiles = System.IO.Directory.GetFiles(dirpath, "*.saanim", SearchOption.AllDirectories);
			Dictionary<string, SA_Tools.FileInfo> files_ini = new Dictionary<string, SA_Tools.FileInfo>();
			for (int u = 0; u < modelfiles.Length; u++)
			{
				ModelFile mdl = new ModelFile(modelfiles[u]);
				SA_Tools.FileInfo fileinfo = new SA_Tools.FileInfo();
				fileinfo.Type = "model";
				fileinfo.Filename = modelfiles[u];
				string mdlname = mdl.Model.Name;
				fileinfo.Address = int.Parse(mdlname.Substring(mdlname.Length - 8, 8), System.Globalization.NumberStyles.AllowHexSpecifier);
				//Console.WriteLine("{0}={1}", fileinfo.Address.ToString("X8"), modelfiles[u]);
				files_ini.Add(mdlname, fileinfo);
			}
			for (int u = 0; u < levelfiles.Length; u++)
			{
				LandTable lvl = LandTable.LoadFromFile(levelfiles[u]);
				SA_Tools.FileInfo fileinfo = new SA_Tools.FileInfo();
				fileinfo.Type = "landtable";
				fileinfo.Filename = levelfiles[u];
				string lvlname = lvl.Name;
				fileinfo.Address = int.Parse(lvlname.Substring(lvlname.Length - 8, 8), System.Globalization.NumberStyles.AllowHexSpecifier);
				//Console.WriteLine("{0}={1}", fileinfo.Address.ToString("X8"), levelfiles[u]);
				files_ini.Add(lvlname, fileinfo);
			}
			for (int u = 0; u < animfiles.Length; u++)
			{
				NJS_MOTION mtn = NJS_MOTION.Load(animfiles[u]);
				SA_Tools.FileInfo fileinfo = new SA_Tools.FileInfo();
				fileinfo.Type = "animation";
				fileinfo.Filename = animfiles[u];
				string mtnname = mtn.Name;
				fileinfo.Address = int.Parse(mtnname.Substring(mtnname.Length - 8, 8), System.Globalization.NumberStyles.AllowHexSpecifier);
				//Console.WriteLine("{0}={1}", fileinfo.Address.ToString("X8"), animfiles[u]);
				files_ini.Add(mtnname, fileinfo);
			}
			IniData inidata_new = new SA_Tools.IniData();
			inidata_new.Files = SortIniData(files_ini);
			IniSerializer.Serialize(inidata_new, dirpath + "_sorted.ini");
		}

		// Something... I forgot
		static void SplitF(string[] args)
		{
			//This code does the following:
			//1) Scan through a split INI file and find identically labelled models and motions
			//2) Add node count to the motions based on their models
			//3) Assign motions to matched models

			string inifilename;
			inifilename = args[0];
			byte[] datafile = File.ReadAllBytes(args[1]);
			uint imageBase = uint.Parse(args[2], System.Globalization.NumberStyles.AllowHexSpecifier);
			IniData inifile = IniSerializer.Deserialize<IniData>(inifilename);
			foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo_mot in inifile.Files)
			{
				if (fileinfo_mot.Value.Type == "animation")
				{
					string motionname = Path.GetFileNameWithoutExtension(fileinfo_mot.Value.Filename.Replace(".saanim", ""));
					bool sucksess = false;
					foreach (KeyValuePair<string, SA_Tools.FileInfo> fileinfo_mdl in inifile.Files)
					{
						string modelname = Path.GetFileNameWithoutExtension(fileinfo_mdl.Value.Filename.Replace(".sa1mdl", ""));
						if (modelname == motionname)
						{
							//Console.WriteLine("Motion {0} fond", motionname);
							NJS_OBJECT obj = new NJS_OBJECT(datafile, fileinfo_mdl.Value.Address, imageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
							//Console.WriteLine("Setting {0} model parts for motion {1}", obj.CountAnimated(), motionfilename);
							fileinfo_mot.Value.CustomProperties.Remove("numparts");
							fileinfo_mot.Value.CustomProperties.Add("numparts", obj.CountAnimated().ToString());
							fileinfo_mdl.Value.CustomProperties.Remove("animations");
							fileinfo_mdl.Value.CustomProperties.Add("animations", Path.GetFileName(fileinfo_mot.Value.Filename));
							sucksess = true;
						}
					}
					if (!sucksess) Console.WriteLine("Model not found for {0}", fileinfo_mot.Value.Filename);
				}
			}
			IniSerializer.Serialize(inifile, "motions.ini");
			return;
		}

		// Loads list of source filenames and a split INI file and outputs a list of matches
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

		// I forgot what this does
		static void SplitL(string[] args, bool addressmatch = false)
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
				if ((!addressmatch && matchlist.ContainsKey(fileinfo.Value.Filename)) || (addressmatch && matchlist.ContainsKey(fileinfo.Value.Address.ToString("X8"))))
				{
					string newfilename;
					int newaddress;
					if (!addressmatch)
					{
						newfilename = matchlist[fileinfo.Value.Filename];
						newaddress = fileinfo.Value.Address;
					}
					else
					{
						newfilename = fileinfo.Value.Filename;
						newaddress = int.Parse(matchlist[fileinfo.Value.Address.ToString("X8")], System.Globalization.NumberStyles.AllowHexSpecifier);
					}
					if (duplicatecheck.Contains(newfilename))
					{
						Console.WriteLine("Duplicate detected: {0}:{1}", newfilename, fileinfo.Value.Filename);
					}
					else duplicatecheck.Add(newfilename);
					splitext = Path.GetExtension(fileinfo.Value.Filename).ToLowerInvariant();
					SA_Tools.FileInfo newfileinfo = new SA_Tools.FileInfo();
					newfileinfo.Address = newaddress;
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
					newinifile.Files.Add(Path.GetFileNameWithoutExtension(newfilename), newfileinfo);
				}
			}
			IniSerializer.Serialize(newinifile, Path.GetFileNameWithoutExtension(listname) + ".ini");
		}

		// Compares levels, models and motions between two folders and outputs a list of identical files
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
				if (ext1 == ".saanim") threshold = 16;
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

		//Generates a split INI file from a list of items in the format 'address=filename.extension'
		static void SplitG(string[] args)
		{
			int key = 0;
			if (args.Length > 2 && args[1] == "-k")
			{
				key = int.Parse(args[2], System.Globalization.NumberStyles.AllowHexSpecifier);
			}
			Dictionary<string, string> matchlist;
			List<string> duplicatecheck = new List<string>();
			string splitext = ".";
			string listname = args[0];
			matchlist = IniSerializer.Deserialize<Dictionary<string, string>>(listname);
			IniData newinifile = new IniData();
			newinifile.Files = new Dictionary<string, SA_Tools.FileInfo>();
			foreach (KeyValuePair<string, string> item in matchlist)
			{
				int addr = int.Parse(item.Key, System.Globalization.NumberStyles.AllowHexSpecifier);
				string newfilename = item.Value;
				if (duplicatecheck.Contains(newfilename))
				{
					Console.WriteLine("Duplicate detected: {0}", newfilename);
				}
				else duplicatecheck.Add(newfilename);
				splitext = Path.GetExtension(newfilename).ToLowerInvariant();
				SA_Tools.FileInfo newfileinfo = new SA_Tools.FileInfo();
				string tp = "unk";
				string finalext = ".";
				switch (splitext)
				{
					case ".c":
						tp = "landtable";
						finalext = ".sa1lvl";
						break;
					case ".nam":
						tp = "animation";
						finalext = ".saanim";
						break;
					case ".tls":
						tp = "texnamearray";
						finalext = ".txt";
						break;
					case ".nac":
						tp = "camera";
						finalext = ".ini";
						break;
					case ".nja":
					default:
						tp = "basicdxmodel";
						finalext = ".sa1mdl";
						break;
				}
				newfileinfo.Filename = newfilename + finalext;
				newfileinfo.Type = tp;
				newfileinfo.Address = addr - key;
				string newdesc = Path.GetFileName(newfilename);
				if (newinifile.Files.ContainsKey(newdesc))
				{
					do
					{
						newdesc += "_";
					}
					while (newinifile.Files.ContainsKey(newdesc));
				}
				Console.WriteLine("{0}:{1}:{2}", newfilename, splitext, newfileinfo.Address.ToString("X8"));
				newinifile.Files.Add(newdesc, newfileinfo);
			}
			IniSerializer.Serialize(newinifile, Path.GetFileNameWithoutExtension(listname) + ".ini");
		}
	}
}