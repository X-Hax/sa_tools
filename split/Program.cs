using SA_Tools.Split;
using SA_Tools.SplitDLL;
using System;
using System.Collections.Generic;
using System.IO;

namespace Split
{
	class Program
	{
		static void Main(string[] args)
		{
			string mode;
			string fullpath_out;
			bool bigendian = false;
			List<string> mdlanimfiles;
			if (args.Length == 0)
			{
				Console.WriteLine("Split any binary files supported by SA Tools.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("-Splitting binary files with INI data-");
				Console.WriteLine("split binary <file> <inifile> [output path]\n");
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
						if (Path.GetExtension(args[1]).ToLowerInvariant() == ".dll")
							SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(fullpath_bin, fullpath_ini, fullpath_out);
						else SA_Tools.Split.Split.SplitFile(fullpath_bin, fullpath_ini, fullpath_out);
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
	}
}