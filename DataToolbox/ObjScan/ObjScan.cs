using System;
using System.Collections.Generic;
using System.IO;
using SplitTools;
using System.Windows.Forms;

namespace SAModel.DataToolbox
{
	public partial class ObjScan
	{
		// Input parameters
		static public string SourceFilename;
		static public string OutputFolder;
		static public bool BigEndian;
		static public bool KeepLandtableModels;
		static public bool NoMeta;
		static public bool KeepChildModels;
		static public bool ReverseColors;
		static public bool SimpleSearch;
		static public bool SingleOutputFolder;
		static public uint StartAddress = 0;
		static public uint EndAddress = 0;
		static public uint ImageBase;
		static public uint DataOffset;
		static public int ModelParts;
		static public bool ShortRot;
		static public bool BasicModelsAreDX;

		// Scan parameters
		static public bool scan_sa1_land;
		static public bool scan_sa2_land;
		static public bool scan_sa2b_land;
		static public bool scan_sa1_model;
		static public bool scan_sa2_model;
		static public bool scan_sa2b_model;
		static public bool scan_action;
		static public bool scan_motion;
		static public string matchfile;

		// Variables
		static public int NumSteps;
		static public int CurrentStep;
		static public bool CancelScan;
		static public bool ConsoleMode;
		static public string CurrentScanData;
		static public uint CurrentAddress;
		static public List<uint> landtablelist;
		static public Dictionary<uint, string> addresslist;
		static public Dictionary<uint, uint[]> actionlist;
		static public byte[] datafile;

		// Variables: progress report
		static public int FoundBasicModels;
		static public int FoundChunkModels;
		static public int FoundGCModels;
		static public int FoundSA1Landtables;
		static public int FoundSADXLandtables;
		static public int FoundSA2Landtables;
		static public int FoundSA2BLandtables;
		static public int FoundActions;
		static public int FoundMotions;

		public static void PerformScan()
		{ 
			addresslist = new Dictionary<uint, string>();
			landtablelist = new List<uint>();
			actionlist = new Dictionary<uint, uint[]>();
			Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(SourceFilename));
			ByteConverter.BigEndian = BigEndian;
			ByteConverter.Reverse = ReverseColors;
			byte[] datafile_temp = File.ReadAllBytes(SourceFilename);
			// Decompress PRS
			if (Path.GetExtension(SourceFilename).ToLowerInvariant() == ".prs") 
				datafile_temp = FraGag.Compression.Prs.Decompress(datafile_temp);
			// Decompress REL
			if (Path.GetExtension(SourceFilename).ToLowerInvariant() == ".rel")
			{
				datafile_temp = HelperFunctions.DecompressREL(datafile_temp);
				HelperFunctions.FixRELPointers(datafile_temp, 0xC900000);
			}
			// Data offset
			if (DataOffset != 0)
			{
				byte[] datafile_new = new byte[DataOffset + datafile_temp.Length];
				datafile_temp.CopyTo(datafile_new, DataOffset);
				datafile = datafile_new;
			}
			else 
				datafile = datafile_temp;
			// Set defaults
			if (EndAddress == 0)
				EndAddress = (uint)datafile.Length - 4;
			if (ImageBase == 0)
				ImageBase = HelperFunctions.SetupEXE(ref datafile) ?? 0;
			// Output folder
			OutputFolder = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(SourceFilename));
			if (Directory.Exists(OutputFolder)) 
				Directory.Delete(OutputFolder, true);
			Directory.CreateDirectory(OutputFolder);
			// Print info
			Console.WriteLine("Source filename: {0} (key: 0x{1})", Path.GetFileName(SourceFilename), ImageBase.ToString("X"));
			// Find a model
			if (!string.IsNullOrEmpty(matchfile))
			{
				FindModel(matchfile);
				return;
			}
			Console.Write("Scanning for: ");
			List<string> datatypes = new List<string>();
			if (scan_sa1_land)
				datatypes.Add(BasicModelsAreDX ? "Basic+ Landtables" : "Basic Landtables");
			if (scan_sa2_land)
				datatypes.Add("Chunk Landtables");
			if (scan_sa2b_land)
				datatypes.Add("SA2B Landtables");
			if (scan_sa1_model)
				datatypes.Add(BasicModelsAreDX ? "Basic+ Models" : "Basic Models");
			if (scan_sa2_model)
				datatypes.Add("Chunk Models");
			if (scan_sa2b_model)
				datatypes.Add("Ginja Models");
			if (scan_action)
				datatypes.Add("Actions");
			if (scan_motion)
				datatypes.Add("Motions" + (ModelParts != 0 ? " with " + ModelParts.ToString() + " nodes" : ""));
			Console.WriteLine(string.Join(", ", datatypes));
			Console.Write("Scanning from {0} to {1}", StartAddress.ToString("X"), EndAddress.ToString("X"));
			if (DataOffset != 0)
				Console.Write(", Data Offset: {0}", DataOffset);
			if (BigEndian)
				Console.Write(", Big Endian");
			if (ReverseColors)
				Console.Write(", Reversed colors/UVs");
			Console.Write(System.Environment.NewLine);
			// Perform scan
			if (scan_sa1_land)
				ScanLandtable(BasicModelsAreDX ? LandTableFormat.SADX : LandTableFormat.SA1);
			if (scan_sa2_land)
				ScanLandtable(LandTableFormat.SA2);
			if (scan_sa2b_land)
				ScanLandtable(LandTableFormat.SA2B);
			if (scan_sa1_model)
				ScanModel(BasicModelsAreDX ? ModelFormat.BasicDX : ModelFormat.Basic);
			if (scan_sa2_model)
				ScanModel(ModelFormat.Chunk);
			if (scan_sa2b_model)
				ScanModel(ModelFormat.GC);
			if (scan_action)
			{
				if (scan_sa1_model)
					ScanAnimations(BasicModelsAreDX ? ModelFormat.BasicDX : ModelFormat.Basic);
				if (scan_sa2_model)
					ScanAnimations(ModelFormat.Chunk);
				if (scan_sa2b_model)
					ScanAnimations(ModelFormat.GC);
			}
			if (scan_motion)
				ScanMotions();
			// Move landtable models to their respective folders and optionally delete them
			CleanUpLandtable();
			CreateSplitIni(Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(SourceFilename) + ".ini"));
			// Clean up empty folders
			bool land = false;
			bool basicmodel = false;
			bool chunkmodel = false;
			bool gcmodel = false;
			bool motion = false;
			foreach (var item in addresslist)
			{
				switch (item.Value)
				{
					case "landtable_SA1":
					case "landtable_SADX":
					case "landtable_SA2":
					case "landtable_SA2B":
						land = true;
						break;
					case "NJS_OBJECT":
					case "NJS_OBJECT_OLD":
						basicmodel = true;
						break;
					case "NJS_CNK_OBJECT":
					case "cnkobj":
						chunkmodel = true;
						break;
					case "NJS_GC_OBJECT":
					case "gcobj":
						gcmodel = true;
						break;
					case "NJS_MOTION":
						motion = true;
						break;
					default:
						break;
				}
			}
			if (!SingleOutputFolder)
			{
				if (!motion && Directory.Exists(Path.Combine(OutputFolder, "actions"))) 
					Directory.Delete(Path.Combine(OutputFolder, "actions"), true);
				if (!land && Directory.Exists(Path.Combine(OutputFolder, "levels"))) 
					Directory.Delete(Path.Combine(OutputFolder, "levels"), true);
				if (!basicmodel && Directory.Exists(Path.Combine(OutputFolder, "basicmodels"))) 
					Directory.Delete(Path.Combine(OutputFolder, "basicmodels"), true);
				if (!chunkmodel && Directory.Exists(Path.Combine(OutputFolder, "chunkmodels"))) 
					Directory.Delete(Path.Combine(OutputFolder, "chunkmodels"), true);
				if (!gcmodel && Directory.Exists(Path.Combine(OutputFolder, "gcmodels"))) 
					Directory.Delete(Path.Combine(OutputFolder, "gcmodels"), true);
				if (!land && !basicmodel && !motion && !basicmodel && !chunkmodel && !gcmodel && Directory.Exists(OutputFolder)) 
					Directory.Delete(OutputFolder, true);
			}
			if (addresslist.Count == 0 && Directory.Exists(OutputFolder)) 
				Directory.Delete(OutputFolder, true);
		}

		static void ObjScanMain(string[] args)
		{
			if (args.Length == 0)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
				Application.Run(new MainForm());
			}
			else
			{
				ProcessCommandLine(args);
			}
		}
	}
}
