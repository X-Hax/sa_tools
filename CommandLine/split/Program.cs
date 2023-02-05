using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SAModel.SAEditorCommon.ProjectManagement;

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
                Console.WriteLine("-Splitting using an XML template-");
                Console.WriteLine("split template <xmlfile> [-data sourcepath] [output path]\n");
                Console.WriteLine("-Splitting a binary file with INI data-");
                Console.WriteLine("split binary <file> <inifile> [output path]\n");
                Console.WriteLine("-Splitting a single item from a binary file without INI data-");
                Console.WriteLine("split single <game> <file> <key> <address> <type> [output filename] [-p custom properties] [-name entryName]\n");
                Console.WriteLine("-Splitting an NB file-");
                Console.WriteLine("split nb <file> [output path] [-ini split ini file]\n");
                Console.WriteLine("-Splitting SA2 MDL files-");
                Console.WriteLine("split mdl <file> [output path] [-anim animation files]\n");
                Console.WriteLine("-Splitting SA2B MDL files-");
                Console.WriteLine("split mdl_b <file> [output path] [-anim animation files]\n");
                Console.WriteLine("-Splitting dllexport entries from DLL files-");
                Console.WriteLine("split dllexport <file> <type> <name> [-id array ID] [output path] [-p numparts]\n");
                Console.WriteLine("Common switches: [-nometa], [-nolabel]");
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
                return;
            }
			for (int u = 2; u < args.Length; u++)
			{
				if (args[u] == "-nometa") nometa = true;
				if (args[u] == "-nolabel") nolabel = true;
			}
			mode = args[0];
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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
                case "template":
                    string dataFolder = "";
                    fullpath_out = "";
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Insufficient arguments");
                        return;
                    }
                    if (!File.Exists(Path.GetFullPath(args[1])))
                    {
                        Console.WriteLine("File {0} doesn't exist", Path.GetFullPath(args[1]));
                        return;
                    }
                    for (int i = 2; i < args.Length; i++)
                    {
						if (args[i] == "-nolabel")
							nolabel = true;
						else if (args[i] == "-nometa")
							nometa = true;
						else if (args[i] == "-data")
						{
							dataFolder = args[i + 1];
							i++;
						}
						else fullpath_out = args[i];
                    }
                    Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(Path.GetFullPath(args[1]));
                    if (template == null)
                    {
                        Console.WriteLine("Failed to open template: {0}", Path.GetFullPath(args[1]));
                        return;
                    }
                    if (dataFolder == "")
                        dataFolder = ProjectFunctions.GetGamePath(template.GameInfo.GameName);
                    Console.WriteLine("Data folder: {0}", dataFolder);
                    string iniFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "..\\GameConfig", template.GameInfo.DataFolder));
                    Console.WriteLine("Splitting using template for {0} located at {1}", template.GameInfo.GameName, Path.GetFullPath(args[1]));
                    if (!Directory.Exists(dataFolder))
                    {
                        Console.WriteLine("\nData folder does not exist: {0}", Path.GetFullPath(dataFolder));
                        Console.WriteLine("Put your game files in {0} and run split again.", Path.GetFullPath(dataFolder));
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    Console.WriteLine("INI folder: {0}", iniFolder);
                    if (fullpath_out == "")
                        fullpath_out = Path.Combine(Environment.CurrentDirectory, template.GameInfo.GameName);
                    Console.WriteLine("Output folder: {0}", fullpath_out);
                    foreach (Templates.SplitEntry splitEntry in template.SplitEntries)
                    {
                        if (!File.Exists(Path.Combine(dataFolder, splitEntry.SourceFile)))
                        {
                            Console.WriteLine("Split source file {0} doesn't exist", Path.Combine(dataFolder, splitEntry.SourceFile));
                            continue;
                        }
                        Console.WriteLine("\n{0}: {1}: {2}", splitEntry.CmnName == null ? "No description" : splitEntry.CmnName, splitEntry.SourceFile, splitEntry.IniFile+".ini");
                        ProjectFunctions.SplitTemplateEntry(splitEntry, null, dataFolder, iniFolder, fullpath_out, true, nometa, nolabel);
                    }
                    if (template.SplitMDLEntries != null)
                        foreach (Templates.SplitEntryMDL splitEntryMDL in template.SplitMDLEntries)
                        {
                            if (!File.Exists(Path.Combine(dataFolder, splitEntryMDL.ModelFile)))
                            {
                                Console.WriteLine("Split MDL source file {0} doesn't exist", Path.Combine(dataFolder, splitEntryMDL.ModelFile));
                                continue;
                            }
                            Console.Write("\nSplitting MDL file: {0}", splitEntryMDL.ModelFile);
                            ProjectFunctions.SplitTemplateMDLEntry(splitEntryMDL, null, dataFolder, fullpath_out);
                        }
					if (template.SplitEventEntries != null)
						foreach (Templates.SplitEntryEvent splitEntryEvent in template.SplitEventEntries)
						{
							if (!File.Exists(Path.Combine(dataFolder, splitEntryEvent.EventFile)))
							{
								Console.WriteLine("Split Event source file {0} doesn't exist", Path.Combine(dataFolder, splitEntryEvent.EventFile));
								continue;
							}
							Console.Write("\nSplitting Event file: {0}", splitEntryEvent.EventFile);
							ProjectFunctions.SplitTemplateEventEntry(splitEntryEvent, null, dataFolder, fullpath_out);
						}
					break;
                case "single":
					int startoffset = 0;
                    string game = args[1];
                    string filepath = args[2];
                    string outPath = "";
                    uint key = uint.Parse(args[3], System.Globalization.NumberStyles.HexNumber);
                    int eaddress=int.Parse(args[4], System.Globalization.NumberStyles.HexNumber);
                    string entryName = "";
                    string props = "";
                    string etype = args[5];
                    if (args.Length > 6)
                    {
                        for (int a = 6; a < args.Length; a++)
                            switch (args[a])
                            {
                                case "-name":
                                    entryName = args[a + 1];
                                    a++;
                                    break;
								case "-offset":
									startoffset = int.Parse(args[a + 1], System.Globalization.NumberStyles.HexNumber);
									a++;
									break;
								case "-p":
                                    props= args[a + 1];
                                    a++;
                                    break;
                                default:
                                    outPath = args[a];
                                    break;
                            }
                    }
					// If no output filename is specified
					if (outPath == "")
                        outPath = Path.Combine(Environment.CurrentDirectory, eaddress.ToString("X8"));
                    // If an output name is specified without a path
                    else if (Path.GetDirectoryName(outPath) == "")
                        outPath = Path.Combine(Environment.CurrentDirectory, outPath);
                    // If a path is specified without a filename
                    else if (Path.GetFileName(outPath) == "")
                        outPath = Path.Combine(outPath, eaddress.ToString("X8"));
                    Console.WriteLine("Splitting from {0} (key: {1}) in {2}: {3} at {4}, offset: {5}", Path.GetFileName(filepath), key.ToString("X"), game.ToUpperInvariant(), etype, eaddress.ToString("X"), startoffset.ToString("X"));
                    Console.WriteLine("Output path: {0}", Path.GetFullPath(outPath));
                    SplitTools.Split.SplitBinary.SplitManual(game, filepath, key, eaddress, etype, outPath, props, entryName, nometa, nolabel, startoffset);
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
					if (args.Length > 1)
					{
						fullpath_out = args[2];
						if (fullpath_out[fullpath_out.Length - 1] != '/') fullpath_out = string.Concat(fullpath_out, '/');
						fullpath_out = Path.GetFullPath(fullpath_out);
					}
					Console.WriteLine("Output path: {0}", fullpath_out);
					if (args.Length > 2)
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
                    int arrayid = -1;
                    string fullpath_dllex = Path.GetFullPath(args[1]);
                    string type = args[2];
                    string name = args[3];
                    string fileOutputPath = "";
                    if (args.Length > 4)
                        for (int u = 4; u < args.Length; u++)
                        {
                            if (args[u] == "-id")
                            {
                                arrayid = int.Parse(args[u + 1]);
                                u++;
                            }
                            else
                                fileOutputPath = args[u];
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
                    // If an array ID is specified, jump to the pointer needed and use it as the address to split
                    if (arrayid != -1)
                    {
                        uint newpointer = ByteConverter.ToUInt32(datafile, address + arrayid * 4);
                        address = (int)(newpointer - imageBase);
                    }
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
                            fileOutputPath = MakePathThatExists(fileOutputPath, land.Name + landext);
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
                                fileOutputPath = MakePathThatExists(fileOutputPath, mdl.Name + modelext);
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
                            fileOutputPath = MakePathThatExists(fileOutputPath, ani.Name + ".saanim");
                            string outpath = Path.GetDirectoryName(Path.GetFullPath(fileOutputPath));
                            Console.WriteLine("Output file: {0}", Path.GetFullPath(fileOutputPath));
                            if (!Directory.Exists(outpath))
                                Directory.CreateDirectory(outpath);
                            ani.Save(fileOutputPath, nometa);
                            break;
						// Attach
						case "attach":
						case "basicattach":
						case "basicdxattach":
						case "chunkattach":
						case "gcattach":
							{
								Attach dummy;
								ModelFormat modelfmt_att;
								string modelext = ".sa1mdl";
								switch (type)
								{
									case "basicattach":
										modelfmt_att = ModelFormat.Basic;
										modelext = ".sa1mdl";
										dummy = new BasicAttach(datafile, address, imageBase, false);
										break;
									case "basicdxattach":
										modelfmt_att = ModelFormat.BasicDX;
										modelext = ".sa1mdl";
										dummy = new BasicAttach(datafile, address, imageBase, true);
										break;
									case "chunkattach":
										modelfmt_att = ModelFormat.Chunk;
										modelext = ".sa2mdl";
										dummy = new ChunkAttach(datafile, address, imageBase);
										break;
									case "gcattach":
										modelfmt_att = ModelFormat.GC;
										dummy = new SAModel.GC.GCAttach(datafile, address, imageBase);
										modelext = ".sa2bmdl";
										break;
									default:
										modelfmt_att = ModelFormat.BasicDX;
										modelext = ".sa1mdl";
										dummy = new BasicAttach(datafile, address, imageBase, true);
										break;
								}
								NJS_OBJECT mdl = new NJS_OBJECT()
								{
									Attach = dummy
								};
								fileOutputPath = MakePathThatExists(fileOutputPath, mdl.Name + modelext);
								if (!Directory.Exists(Path.GetDirectoryName(fileOutputPath)))
									Directory.CreateDirectory(Path.GetDirectoryName(fileOutputPath));
								ModelFile.CreateFile(fileOutputPath, mdl, null, null, null, null, modelfmt_att, nometa);
							}
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

        static string MakePathThatExists(string originalPath, string objectName)
        {
            string result = originalPath;
            // If no output filename/path is specified
            if (originalPath == "")
                result = Path.Combine(Environment.CurrentDirectory, objectName);
            // If output filename is specified without a path
            else if (Path.GetDirectoryName(originalPath) == "")
                result = Path.Combine(Environment.CurrentDirectory, originalPath);
            // If output path is specified without a filename
            else if (Path.GetFileName(originalPath) == "")
                result = Path.Combine(originalPath, objectName);
            return result;
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