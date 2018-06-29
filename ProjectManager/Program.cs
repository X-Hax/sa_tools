using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SA_Tools;
using SonicRetro.SAModel;
using SonicRetro.SAModel.SAEditorCommon;

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

        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ProjectManager projectSelect;

            //Properties.Settings.Default.Upgrade();
            settings = ProjectManagerSettings.Load();

            if(args != null && args.Length > 0  && args[0] == "build")
            {
                // not yet implemented because not enough info to know which game is being targeted
                // args[1] should be mod name
                // args[2] should be game
                throw new System.NotImplementedException();
                
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
                Application.Run(projectSelect);
            }

            #region Old Code
            //#region Getting Input Arguments
            //string datafilename, inifilename, projectFolderName;
            //if (args.Length > 0)
            //{
            //    datafilename = args[0];
            //    Console.WriteLine("File: {0}", datafilename);
            //}
            //else
            //{
            //    Console.WriteLine("No source file supplied. Aborting.");
            //    PrintHelp();
            //    Console.WriteLine("Press any key to exit.");
            //    Console.ReadLine();
            //    return (int)ERRORVALUE.NoSourceFile;
            //}
            //if (args.Length > 1)
            //{
            //    inifilename = args[1];
            //    Console.WriteLine("INI File: {0}", inifilename);
            //}
            //else
            //{
            //    Console.WriteLine("No data mapping file supplied (expected ini). Aborting.");
            //    Console.WriteLine("Press any key to exit.");
            //    Console.ReadLine();
            //    return (int)ERRORVALUE.NoDataMapping;
            //}
            //if (args.Length > 2)
            //{
            //    projectFolderName = args[2];
            //    Console.WriteLine("Project Folder: {0}", projectFolderName);
            //}
            //else
            //{
            //    Console.WriteLine("No project folder supplied. Aborting.");
            //    Console.WriteLine("Press any key to exit.");
            //    Console.ReadLine();
            //    return (int)ERRORVALUE.NoProject;
            //}
            //#endregion
            #endregion

            return 0;
        }
	}
}