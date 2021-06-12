using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Split
{
    partial class Program
	{

        static void Main(string[] args)
		{
			bool nometa = false;
			bool nolabel = false;
			string mode;
			string fullpath_out;
			bool bigendian = false;
			List<string> mdlanimfiles;
			if (args.Length == 0)
			{
				Console.WriteLine("Split any binary files supported by SA Tools.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("-Splitting binary files with INI data-");
				Console.WriteLine("split binary <file> <inifile> [output path] [-nometa] [-nolabel]\n");
				Console.WriteLine("-Splitting SA1/SADX NB files-");
				Console.WriteLine("split nb <file> [output path] -ini [split INI file]\n");
				Console.WriteLine("-Splitting SA2 MDL files-");
				Console.WriteLine("split mdl <file> [output path] -anim [animation files]\n");
				Console.WriteLine("-Splitting SA2B MDL files-");
				Console.WriteLine("split mdl_b <file> [output path] -anim [animation files]\n");
                Console.WriteLine("-Splitting dllexport entries from DLL files-");
                Console.WriteLine("split dllexport <file> <type> <name> [output path] [-p numparts]\n");
                Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
#if DEBUG
			if (SplitExtensions(args) == true)
				return;
#endif
			for (int u = 2; u < args.Length; u++)
			{
				if (args[u] == "-nometa") nometa = true;
				if (args[u] == "-nolabel") nolabel = true;
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
						SplitTools.SplitDLL.SplitDLL.SplitDLLFile(fullpath_bin, fullpath_ini, fullpath_out, nometa, nolabel);
					else SplitTools.Split.SplitBinary.SplitFile(fullpath_bin, fullpath_ini, fullpath_out, nometa, nolabel);
					break;
				case "nb":
				case "nb_b":
					string fullpath_nb = Path.GetFullPath(args[1]);
					string path_ini = null;
					if (args[args.Length - 2].ToLowerInvariant() == "-ini")
					{
						path_ini = Path.GetFullPath(args[args.Length - 1]);
					}
					if (!File.Exists(fullpath_nb))
					{
						Console.WriteLine("File {0} doesn't exist.", fullpath_nb);
						return;
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
					SplitTools.Split.SplitNB.SplitNBFile(fullpath_nb, false, fullpath_out, 1, path_ini);
					break;
				case "mdl":
				case "mdl_b":
					string fullpath_mdl = Path.GetFullPath(args[1]);
					if (!File.Exists(fullpath_mdl))
					{
						Console.WriteLine("File {0} doesn't exist.", fullpath_mdl);
						return;
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
						SplitTools.SAArc.sa2MDL.Split(bigendian, fullpath_mdl, fullpath_out, mdlanimfiles.ToArray());
					}
					else
						SplitTools.SAArc.sa2MDL.Split(bigendian, fullpath_mdl, fullpath_out, null);
					break;
                case "dllexport":
                    string fullpath_dllex = Path.GetFullPath(args[1]);
                    string type = args[2];
                    string name = args[3];
                    string fileOutputPath = "";
                    if (args.Length > 4)
                    {
                        for (int i = 4; i < args.Length; i++)
                            if (args[i].Substring(0, 1) != "-" && args[i-1].Substring(0, 1) != "-")
                        fileOutputPath = args[i];
                    }
                    if (!File.Exists(fullpath_dllex))
                    {
                        Console.WriteLine("File {0} doesn't exist.", fullpath_dllex);
                        return;
                    }
                    Console.Write("File: {0}", fullpath_dllex);
                    byte[] datafile = File.ReadAllBytes(fullpath_dllex);
                    uint imageBase = SplitTools.HelperFunctions.SetupEXE(ref datafile).Value;
                    Dictionary<string, int> exports;
                    Dictionary<int, string> labels = new Dictionary<int, string>();
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
                            string namex = datafile.GetCString(BitConverter.ToInt32(datafile, nameaddr),
                                System.Text.Encoding.ASCII);
                            int addr = BitConverter.ToInt32(datafile,
                                dir.AddressOfFunctions + (BitConverter.ToInt16(datafile, ordaddr) * 4));
                            exports.Add(namex, addr);
                            labels.Add(addr, namex);
                            nameaddr += 4;
                            ordaddr += 2;
                        }
                        Console.Write(" ({0} exports)\n", exports.Count);
                    }
                    if (!exports.ContainsKey(name))
                    {
                        Console.WriteLine("The export table has no item named {0}", name);
                        return;
                    }
                    int address = exports[name];
                    Console.WriteLine("{0} {1}:{2}", type, name, address.ToString("X8"));
                    switch (type)
                    {   
                        // Landtables
                        case "landtable":
                        case "sa1landtable":
                        case "sadxlandtable":
                        case "sa2landtable":
                        case "sa2blandtable":
                        case "battlelandtable":
                            LandTableFormat landfmt_cur;
                            string landext;
                            switch (type)
                            {
                                case "sa1landtable":
                                    landfmt_cur = LandTableFormat.SA1;
                                    landext = ".sa1lvl";
                                    break;
                                case "sadxlandtable":
                                    landfmt_cur = LandTableFormat.SADX;
                                    landext = ".sa1lvl";
                                    break;
                                case "sa2landtable":
                                    landfmt_cur = LandTableFormat.SA2;
                                    landext = ".sa2lvl";
                                    break;
                                case "sa2blandtable":
                                case "battlelandtable":
                                    landfmt_cur = LandTableFormat.SA2B;
                                    landext = ".sa2blvl";
                                    break;
                                case "landtable":
                                default:
                                    landfmt_cur = LandTableFormat.SADX;
                                    landext = ".sa1lvl";
                                    break;
                            }
                            LandTable land = new LandTable(datafile, address, imageBase, landfmt_cur, labels);
                            if (fileOutputPath == "") 
                                fileOutputPath = land.Name + landext;
                            if (!Directory.Exists(Path.GetDirectoryName(fileOutputPath)))
                                Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
                            land.SaveToFile(fileOutputPath, landfmt_cur, nometa);
                            break;
                        // NJS_OBJECT
                        case "model":
                        case "object":
                        case "basicmodel":
                        case "basicdxmodel":
                        case "chunkmodel":
                        case "gcmodel":
                            {
                                ModelFormat modelfmt_obj;
                                string modelext;
                                switch (type)
                                {
                                    case "basicmodel":
                                        modelfmt_obj = ModelFormat.Basic;
                                        modelext = ".sa1mdl";
                                        break;
                                    case "basicdxmodel":
                                        modelfmt_obj = ModelFormat.BasicDX;
                                        modelext = ".sa1mdl";
                                        break;
                                    case "chunkmodel":
                                        modelfmt_obj = ModelFormat.Chunk;
                                        modelext = ".sa2mdl";
                                        break;
                                    case "gcmodel":
                                        modelfmt_obj = ModelFormat.GC;
                                        modelext = ".sa2bmdl";
                                        break;
                                    default:
                                        modelfmt_obj = ModelFormat.BasicDX;
                                        modelext = ".sa1mdl";
                                        break;
                                }
                                NJS_OBJECT mdl = new NJS_OBJECT(datafile, address, imageBase, modelfmt_obj, labels, new Dictionary<int, Attach>());
                                if (fileOutputPath == "")
                                    fileOutputPath = mdl.Name + modelext;
                                if (!Directory.Exists(Path.GetDirectoryName(fileOutputPath)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
                                ModelFile.CreateFile(fileOutputPath, mdl, null, null, null, null, modelfmt_obj, nometa);
                            }
                            break;
                        // NJS_MOTION
                        case "animation":
                        case "motion":
                            int numparts = 0;
                            for (int a = 3; a < args.Length; a++)
                            {
                                if (args[a] == "-p")
                                    numparts = int.Parse(args[a + 1], System.Globalization.NumberStyles.Integer);
                            }
                            NJS_MOTION ani = new NJS_MOTION(datafile, address, imageBase, numparts, labels);
                            if (fileOutputPath == "")
                                fileOutputPath = ani.Name + ".saanim";
                            string outpath = Path.GetDirectoryName(Path.GetFullPath(fileOutputPath));
                            Console.WriteLine("Output file: {0}", Path.GetFullPath(fileOutputPath));
                            if (!Directory.Exists(outpath))
                                Directory.CreateDirectory(outpath);
                            ani.Save(fileOutputPath, nometa);
                            break;
                        default:
                            Console.WriteLine("Unrecognized export type {0}", type);
                            break;
                    }
                    break;
				default:
					Console.WriteLine("Incorrect mode specified. Press ENTER to exit.");
					Console.ReadLine();
					return;
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
            public int AddressOfFunctions;   // RVA from base of image
            public int AddressOfNames;       // RVA from base of image
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
}