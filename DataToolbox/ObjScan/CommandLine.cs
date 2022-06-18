using SAModel;
using SplitTools;
using System;
using System.Globalization;
using System.IO;

namespace SAModel.DataToolbox
{
	public static partial class ObjScan
	{
		public static void ProcessCommandLine(string[] args)
		{
			Game game = Game.SADX;
			ConsoleMode = true;
			string type;
			if (args.Length == 0)
			{
				PrintHelp();
				return;
			}
			switch (args[0].ToLowerInvariant())
			{
				case "-?":
				case "/?":
					PrintHelp();
					return;
				case "sa1":
					game = Game.SA1;
					break;
				case "sa1_b":
					BigEndian = true;
					game = Game.SA1;
					break;
				case "sadx":
					game = Game.SADX;
					BasicModelsAreDX = true;
					break;
				case "sadx_b":
					BigEndian = true;
					game = Game.SADX;
					BasicModelsAreDX = true;
					break;
				case "sadx_g":
					BigEndian = true;
					ReverseColors = true;
					game = Game.SA1;
					break;
				case "sa2":
					game = Game.SA2;
					break;
				case "sa2_b":
					BigEndian = true;
					game = Game.SA2;
					break;
				case "sa2b":
					game = Game.SA2B;
					break;
				case "sa2b_b":
					BigEndian = true;
					game = Game.SA2B;
					break;
				default:
					Console.WriteLine("Error parsing game type.\nCorrect game types are: SA1, SADX, SADX_b, SADX_g, SA2, SA2B, SA2_b, SA2B_b");
					Console.WriteLine("Press ENTER to exit.");
					Console.ReadLine();
					return;
			}
			SourceFilename = args[1];
			ImageBase = uint.Parse(args[2], NumberStyles.AllowHexSpecifier);
			type = args[3];
			for (int u = 2; u < args.Length; u++)
			{
				switch (args[u])
				{
					case "-offset":
						StartAddress = uint.Parse(args[u + 1], NumberStyles.AllowHexSpecifier);
						break;
					case "action":
						scan_action = true;
						break;
					case "-file":
						matchfile = args[u + 1];
						break;
					case "-findall":
						SimpleSearch = true;
						break;
					case "-keepland":
						KeepLandtableModels = true;
						break;
					case "-nometa":
						NoMeta = true;
						break;
					case "-keepchild":
						KeepChildModels = true;
						break;
					case "-start":
						StartAddress = uint.Parse(args[u + 1], NumberStyles.AllowHexSpecifier);
						break;
					case "-end":
						EndAddress = uint.Parse(args[u + 1], NumberStyles.AllowHexSpecifier);
						break;
					case "-parts":
						ModelParts = int.Parse(args[u + 1]);
						break;
					case "-shortrot":
						ShortRot = true;
						break;
					case "-nodir":
						SingleOutputFolder = true;
						break;
				}
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(SourceFilename));
				OutputFolder = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(SourceFilename));
			}
			switch (type.ToLowerInvariant())
			{
				case "match":
					matchfile = args[4];
					break;
				case "all":
					SetScanParametersCmdline(game, true, true, true);
					PerformScan();
					break;
				case "allmotion":
					SetScanParametersCmdline(game, true, true, true);
					scan_motion = true;
					break;
				case "landtable":
					SetScanParametersCmdline(game, true, false, false);
					break;
				case "model":
					SetScanParametersCmdline(game, false, true, true);
					break;
				case "basicmodel":
					SetScanParametersCmdline(Game.SA1, false, true, false);
					break;
				case "basicdxmodel":
					SetScanParametersCmdline(Game.SADX, false, true, false);
					break;
				case "chunkmodel":
					SetScanParametersCmdline(Game.SA2, false, true, false);
					break;
				case "gcmodel":
					SetScanParametersCmdline(Game.SA2B, false, false, true);
					break;
				case "motion":
					scan_motion = true;
					break;
			}
			PerformScan();
		}

		public static void SetScanParametersCmdline(Game game, bool landtable, bool model_main, bool model_additional)
		{
			switch (game)
			{
				case Game.SA1:
					BasicModelsAreDX = false;
					scan_sa1_land = landtable;
					scan_sa1_model = model_main;
					break;
				case Game.SADX:
					BasicModelsAreDX = true;
					scan_sa1_land = landtable;
					scan_sa1_model = model_main;
					scan_sa2_model = model_additional;
					break;
				case Game.SA2:
					scan_sa2_land = landtable;
					scan_sa2_model = model_main;
					break;
				case Game.SA2B:
					scan_sa2_land = landtable;
					scan_sa2b_land = landtable;
					scan_sa2_model = model_main;
					scan_sa2b_model = model_additional;
					break;
			}
		}

		public static void PrintHelp()
		{
			Console.WriteLine("Object Scanner scans a binary file or memory dump and extracts levels, models and/or motions from it.");
			Console.WriteLine("Usage: objscan <GAME> <FILENAME> <KEY> <TYPE> [-offset addr] [-file modelfile] [-start addr] [-end addr] [-findall]\n[-noaction] [-nometa] [-keepland] [-keepchild]\n");
			Console.WriteLine("Argument description:");
			Console.WriteLine("<GAME>: SA1, SADX, SA2, SA2B. Add '_b' (e.g. SADX_b) to set Big Endian, use SADX_g for the Gamecube version of SADX.");
			Console.WriteLine("<FILENAME>: The name of the binary file, e.g. sonic.exe.");
			Console.WriteLine("<KEY>: Binary key, e.g. 400000 for sonic.exe or C900000 for SA1 STG file. Use C900000 for Gamecube REL files.");
			Console.WriteLine("<TYPE>: model, basicmodel, basicdxmodel, chunkmodel, gcmodel, landtable, all, allmotion, match, motion");
			Console.WriteLine("-offset: Start offset (hexadecimal).");
			Console.WriteLine("-file: Path to .sa1mdl file to use in match mode.");
			Console.WriteLine("-action: Scan for actions for all found models.");
			Console.WriteLine("-findall: Less strict scan, try to find as many models as possible.");
			Console.WriteLine("-nometa: Don't save labels.");
			Console.WriteLine("-SingleOutputFolder: Don't create folders for file categories.");
			Console.WriteLine("-parts: Minimum number of model parts for motions.");
			Console.WriteLine("-shortrot: Use int16 rotations in motions.");
			Console.WriteLine("-start and -end: Range of addresses to scan.");
			Console.WriteLine("-keepland: Include landtable models in generated split INI.\n");
			Console.WriteLine("-keepchild: Don't clean up child and sibling models.\n");
			Console.WriteLine("Press ENTER to exit");
			Console.ReadLine();
		}
	}
}