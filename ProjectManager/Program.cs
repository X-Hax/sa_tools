using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SA_Tools;
using SonicRetro.SAModel;


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
        private static Settings settings;
        public static Settings Settings { get { return settings; } } 

        private static void PrintHelp()
        {
            Console.WriteLine("Argument #1: Path to file to be split apart into data chunks.");
            Console.WriteLine("Argument #2: Path to data mapping file. This is usually an INI file.");
            Console.WriteLine("Argument #3: Project Directory. All files will be output to this directory.");
            //Console.WriteLine("Argument #3 must be an absolute path.");
        }

        public static bool CheckSADXPCValid(out string failReason)
        {
            bool sadxPCIsValid = true;

            try // get cast exceptions here
            {
                string sadxPCPath = settings.SADXPCPath;

                bool sadxPCPathStringExists = sadxPCPath != null && sadxPCPath.Length > 0;
                bool sadxPCPathExists = Directory.Exists(sadxPCPath);
                bool sonicExeExists = File.Exists(string.Concat(sadxPCPath + "\\", "sonic.exe")); // todo: maybe md5 check this so that we can tell it's the right version?
                bool modLoaderPresent = File.Exists(string.Concat(sadxPCPath + "\\", "SADXModManager.exe"));

                if(!sadxPCPathStringExists)
                {
                    failReason = "SADX PC path string did not exist in settings";
                    return false;
                }
                else if (!sadxPCPathExists)
                {
                    failReason = string.Format("SADX PC path {0} did not exist or was invalid.", sadxPCPath);
                    return false;
                }
                else if (!sonicExeExists)
                {
                    failReason = "SADX PC path did not contain sonic.exe";
                    return false;
                }
                else if (!modLoaderPresent)
                {
                    failReason = "SADX PC path did not contain mod loader";
                    return false;
                }
            }
            catch (System.InvalidCastException e)
            {
                failReason = "sadxpc path settings variable was not string type";
                sadxPCIsValid = false;
            }

            failReason = "";
            return sadxPCIsValid;
        }

        public static bool CheckSA2PCValid(out string failReason)
        {
            bool sa2PCIsValid = true;

            try // get cast exceptions here
            {
                string sa2PCPath = settings.SA2PCPath;

                bool sa2PCPathStringExists = sa2PCPath != null && sa2PCPath.Length > 0;
                bool sa2PCPathExists = Directory.Exists(sa2PCPath);
                bool sonic2ExeExists = File.Exists(string.Concat(sa2PCPath + "\\", "sonic2app.exe"));
                bool modLoaderPresent = File.Exists(string.Concat(sa2PCPath + "\\", "SA2ModManager.exe"));

                if (!sa2PCPathStringExists)
                {
                    failReason = "SA2 PC path string did not exist in settings";
                    return false;
                }
                else if (!sa2PCPathExists)
                {
                    failReason = string.Format("SA2 PC path {0} did not exist or was invalid.", sa2PCPath);
                    return false;
                }
                else if (!sonic2ExeExists)
                {
                    failReason = "SA2 PC path did not contain sonic2app.exe";
                    return false;
                }
                else if (!modLoaderPresent)
                {
                    failReason = "SA2 PC path did not contain mod loader";
                    return false;
                }
            }
            catch(System.InvalidCastException e)
            {
                sa2PCIsValid = false;
            }

            failReason = "";
            return sa2PCIsValid;
        }

        public static bool AnyGamesConfigured()
        {
            // sadx validity check
            string sadxpcfailreason = "";
            bool sadxPCIsValid = CheckSADXPCValid(out sadxpcfailreason);

            // sa2 valididty check next
            string sa2pcFailReason = "";
            bool sa2PCIsValid = CheckSA2PCValid(out sa2pcFailReason);

            return sadxPCIsValid || sa2PCIsValid;
        }

        [STAThread]
        static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ProjectManager projectSelect;

            //Properties.Settings.Default.Upgrade();
            settings = Settings.Load();

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