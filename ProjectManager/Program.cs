using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon;
using Fclp;

namespace ProjectManager
{
	enum ERRORVALUE
	{
		Success = 0,
		NoProject = -1,
		InvalidProject = -2,
		NoSourceFile = -3,
		NoDataMapping = -4,
		InvalidDataMapping = -5,
		UnhandledException = -6,
		InvalidConfig = -7
	}

	static class Program
	{
		private static ProjectManagerSettings settings;
		public static ProjectManagerSettings Settings { get { return settings; } } 

		private static void PrintHelp()
		{
			Console.WriteLine("Argument #1: Path to file to be split apart into data chunks.");
			Console.WriteLine("Argument #2: Path to data mapping file. This is usually an INI file.");
			Console.WriteLine("Argument #3: Project Directory. All files will be output to this directory.");
			//Console.WriteLine("Argument #3 must be an absolute path.");
		}

		public static bool AnyGamesConfigured()
		{
			// sadx validity check
			string sadxpcfailreason = "";
			bool sadxPCIsValid = GamePathChecker.CheckSADXPCValid(settings.SADXPCPath, out sadxpcfailreason);

			// sa2 valididty check next
			string sa2pcFailReason = "";
			bool sa2PCIsValid = GamePathChecker.CheckSA2PCValid(settings.SA2PCPath, out sa2pcFailReason);

			return sadxPCIsValid || sa2PCIsValid;
		}

		enum CLIMode
		{
			None,
			Split,
			SplitMDL,
			Build
		}

		struct StartupArgs
		{
			public CLIMode mode;

			// split options
			public string filePath;
			public string dataMappingPath;
			public bool isBigEndian;
			public string outputFolder;
			public string[] animationList;
			public bool exportCStructs;

			// build options
			public string projectName;
			public SA_Tools.Game game;
			public bool runAfterBuild;
		}

		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ProjectManager projectSelect;

			//Properties.Settings.Default.Upgrade();
			settings = ProjectManagerSettings.Load();

			StartupArgs startupArgs = new StartupArgs();
			startupArgs.mode = CLIMode.None;

			FluentCommandLineParser parser = new FluentCommandLineParser();
			// parse main options
			parser.Setup<CLIMode>('m', "mode").Callback(mode => startupArgs.mode = mode);

			// parse split options
			parser.Setup<string>('f', "FileToSplit").Callback(fileToSplit => startupArgs.filePath = fileToSplit);
			parser.Setup<string>('d', "DataMappingFilePath").Callback(mappingPath => startupArgs.dataMappingPath = mappingPath);
			parser.Setup<bool>('b', "BigEndian").Callback(bigEndian => startupArgs.isBigEndian = bigEndian);
			parser.Setup<string>('o', "OutputFolder").Callback(folder => startupArgs.outputFolder = folder);
			parser.Setup<List<string>>('a', "AnimationList").Callback(animationList => startupArgs.animationList = animationList.ToArray());
			//parser.Setup<bool>

			// parse build options
			parser.Setup<string>('p', "ProjectName").Callback(projectName => startupArgs.projectName = projectName);
			parser.Setup<SA_Tools.Game>('g', "Game").Callback(game => startupArgs.game = game);
			parser.Setup<bool>('r', "RunAfterBuild").Callback(runAfterbuild => startupArgs.runAfterBuild = runAfterbuild);

			// do help
			parser.SetupHelp("?", "help").Callback(text => Console.WriteLine(text));

			parser.Parse(args);
				
			if(startupArgs.mode != CLIMode.None) // user wants to use the CLI
			{
				switch (startupArgs.mode)
				{
					case CLIMode.Split:
						CLISplit(startupArgs);
						break;
					case CLIMode.SplitMDL:
						CLISplitMDL(startupArgs);
						break;
					case CLIMode.Build:
						CLIBuild(startupArgs);
						break;
					default:
						break;
				}
			}
			else
			{
				// first check to see if we're configured properly.
				if(!AnyGamesConfigured())
				{
					GameConfig gameConfig = new GameConfig();
					DialogResult configResult = gameConfig.ShowDialog();

					if (configResult == DialogResult.Abort) return (int)ERRORVALUE.InvalidConfig;
					gameConfig.Dispose();
				}

				// todo: catch unhandled exceptions
				projectSelect = new ProjectManager();
				Application.ThreadException += Application_ThreadException;
				Application.Run(projectSelect);
			}

			return 0;
		}

		private static int CLISplit(StartupArgs args)
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(args.dataMappingPath);

			if (!File.Exists(args.filePath))
			{
				Console.WriteLine(args.filePath + " not found. Aborting.");
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();

				return (int)ERRORVALUE.NoSourceFile;
			}

			if (!File.Exists(args.dataMappingPath))
			{
				Console.WriteLine(args.dataMappingPath + " not found. Aborting.");
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();

				return (int)ERRORVALUE.NoDataMapping;
			}

			if (!Directory.Exists(args.outputFolder))
			{
				// try creating the directory
				bool created = true;

				try
				{
					// check to see if trailing charcter closes 
					Directory.CreateDirectory(args.outputFolder);
				}
				catch
				{
					created = false;
				}

				if (!created)
				{
					// couldn't create directory.
					Console.WriteLine("Output folder did not exist and couldn't be created.");
					Console.WriteLine("Press any key to exit.");
					Console.ReadLine();

					return (int)ERRORVALUE.InvalidProject;
				}
			}

			System.IO.FileInfo fileInfo = new System.IO.FileInfo(args.filePath);

			return (fileInfo.Extension.ToLower().Contains("dll")) ? SplitDLL.SplitDLL.SplitDLLFile(args.filePath, args.dataMappingPath, args.outputFolder) :
				Split.Split.SplitFile(args.filePath, args.dataMappingPath, args.outputFolder);
		}

		private static void CLISplitMDL(StartupArgs args)
		{
			SplitMDL.SplitMDL.Split(args.isBigEndian, args.filePath, args.outputFolder, args.animationList);
		}

		private static void CLIBuild(StartupArgs args)
		{

		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			string error = string.Format("{0}\n{1}", e.Exception.Message, e.Exception.TargetSite);
			MessageBox.Show(error, "Unhandled exception");

				//if (primaryForm != null)
				//	using (ErrorDialog ed = new ErrorDialog((Exception)e.ExceptionObject, false))
				//		ed.ShowDialog(primaryForm);
				//else
				//{
				//	System.IO.File.WriteAllText("SADXLVL2.log", e.ExceptionObject.ToString());
				//	MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//}
		}
	}
}