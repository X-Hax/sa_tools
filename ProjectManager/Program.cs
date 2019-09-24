using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon;
using Fclp;

namespace ProjectManager
{
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
			parser.Setup<CLIMode>('m', "mode").Callback((CLIMode mode) =>
			{
				startupArgs.mode = mode;
			});

			// parse split options
			parser.Setup<string>('f', "FileToSplit").Callback((string fileToSplit) => { startupArgs.filePath = fileToSplit; });
			parser.Setup<string>('o', "OutputFolder").Callback((string folder) => { startupArgs.outputFolder = folder; });
			parser.Setup<string>('d', "DataMappingFilePath").Callback((string mappingPath) => { startupArgs.dataMappingPath = mappingPath; });
			parser.Setup<bool>('b', "BigEndian").Callback((bool bigEndian) => { startupArgs.isBigEndian = bigEndian; });
			parser.Setup<List<string>>('a', "AnimationList").Callback((List<string> animationList) => { startupArgs.animationList = animationList.ToArray(); });
			//parser.Setup<bool>

			// parse build options
			parser.Setup<string>('p', "ProjectName").Callback((string projectName) => { startupArgs.projectName = projectName; });
			parser.Setup<SA_Tools.Game>('g', "Game").Callback((SA_Tools.Game game) => { startupArgs.game = game; });
			parser.Setup<bool>('r', "RunAfterBuild").Callback((bool runAfterbuild) => { startupArgs.runAfterBuild = runAfterbuild; });

			// do help
			parser.SetupHelp("?", "help").Callback((string text) =>
			{
				Console.WriteLine("Project Manager cmd line options:");
				Console.WriteLine(text);
				Console.WriteLine("NOTE: Not all of these options are valid in all contexts.");
				Console.WriteLine("You must set Mode. To either Split, SplitMDL, or Build.");
				Console.WriteLine("Split requires:");
				Console.WriteLine("   FileToSplit: This is the file with the data you'd like to extract.");
				Console.WriteLine("   DataMappingFilePath: This is the INI file that says where things are in the data file.");
				Console.WriteLine("   OutputFolder: This is where the split data will be placed.");
				Console.WriteLine();
				Console.WriteLine("SplitMDL requires everything Split does but also:");
				Console.WriteLine("   BigEndian: true/false. States whether or not the file is big endian.");
				Console.WriteLine("   AnimationList: (optional) a list of SAAnimation files to use. Paths are relative to the mdl file.");
				Console.WriteLine("Build requires:");
				Console.WriteLine("   ProjectName: name of the project to build.");
				Console.WriteLine("   Game: Game the project is for. Valid values are SADXPC and SA2B");
				Console.WriteLine("   RunAfterBuild: true/false. Whether or not to start the game and load the mod after build is complete");
			});

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

					if (configResult == DialogResult.Abort) return (int)SA_Tools.Split.SplitERRORVALUE.InvalidConfig;
					gameConfig.Dispose();
				}

				// todo: catch unhandled exceptions
				projectSelect = new ProjectManager();
				Application.ThreadException += Application_ThreadException;
				Application.Run(projectSelect);
			}

			return 0;
		}

		private static int CLISplit(StartupArgs startupArgs)
		{
			bool envSwitchError = false;
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(startupArgs.dataMappingPath);
			}
			catch(System.ArgumentException e)
			{
				envSwitchError = true;
				Console.WriteLine(string.Format("{0} was an invalid data mapping path", startupArgs.dataMappingPath));
			}
			catch(System.IO.DirectoryNotFoundException nullEx)
			{
				envSwitchError = true;
				Console.WriteLine(string.Format("Path to data mappilg file {0} did not exist", startupArgs.dataMappingPath));
			}

			if(envSwitchError)
			{
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();

				return (int)SA_Tools.Split.SplitERRORVALUE.InvalidDataMapping;
			}

			if (!File.Exists(startupArgs.filePath))
			{
				Console.WriteLine(startupArgs.filePath + " not found. Aborting.");
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();

				return (int)SA_Tools.Split.SplitERRORVALUE.NoSourceFile;
			}

			if (!File.Exists(startupArgs.dataMappingPath))
			{
				Console.WriteLine(startupArgs.dataMappingPath + " not found. Aborting.");
				Console.WriteLine("Press any key to exit.");
				Console.ReadLine();

				return (int)SA_Tools.Split.SplitERRORVALUE.NoDataMapping;
			}

			// check our output folder's last character for validity. Modify it if need be so that sub folders do not get created.
			char outputPathfinalChar = startupArgs.outputFolder[startupArgs.outputFolder.Length - 1];
			if (outputPathfinalChar != System.IO.Path.DirectorySeparatorChar && outputPathfinalChar != System.IO.Path.AltDirectorySeparatorChar)
			{
				startupArgs.outputFolder += System.IO.Path.DirectorySeparatorChar;
			}

			if (!Directory.Exists(startupArgs.outputFolder))
			{
				// try creating the directory
				bool created = true;

				try
				{
					// check to see if trailing charcter closes 
					Directory.CreateDirectory(startupArgs.outputFolder);
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

					return (int)SA_Tools.Split.SplitERRORVALUE.InvalidProject;
				}
			}

			System.IO.FileInfo fileInfo = new System.IO.FileInfo(startupArgs.filePath);

			return (fileInfo.Extension.ToLower().Contains("dll")) ? SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(startupArgs.filePath, startupArgs.dataMappingPath, startupArgs.outputFolder) :
				SA_Tools.Split.Split.SplitFile(startupArgs.filePath, startupArgs.dataMappingPath, startupArgs.outputFolder);
		}

		private static void CLISplitMDL(StartupArgs args)
		{
			SA_Tools.SplitMDL.SplitMDL.Split(args.isBigEndian, args.filePath, args.outputFolder, args.animationList);
		}

		private static void CLIBuild(StartupArgs args)
		{
			Console.WriteLine("CLI Build is not yet implemented");
			Console.WriteLine("Press any key to exit.");
			Console.ReadLine();
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