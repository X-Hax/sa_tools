using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon;

namespace ProjectManager
{
	static class Program
	{
		private static SAEditorCommon.ProjectManagement.ProjectSettings settings;
		public static SAEditorCommon.ProjectManagement.ProjectSettings Settings { get { return settings; } } 

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
			settings = SAEditorCommon.ProjectManagement.ProjectSettings.Load();

			// first check to see if we're configured properly.
			if (!AnyGamesConfigured())
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

			return 0;
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