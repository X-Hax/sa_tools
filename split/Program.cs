using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using SA_Tools;
using SonicRetro.SAModel;

namespace split
{
    enum ERRORVALUE
    {
        Success = 0,
        NoProject = -1,
        InvalidProject = -2,
        NoSourceFile = -3,
        NoDataMapping = -4,
        InvalidDataMapping = -5,
        UnhandledException = -6
    }

    static class Program
	{
        private static void PrintHelp()
        {
            Console.WriteLine("Argument #1: Path to file to be split apart into data chunks.");
            Console.WriteLine("Argument #2: Path to data mapping file. This is usually an INI file.");
            Console.WriteLine("Argument #3: Project Directory. All files will be output to this directory.");
            //Console.WriteLine("Argument #3 must be an absolute path.");
        }

        static int Main(string[] args)
        {
            #region Getting Input Arguments
            string datafilename, inifilename, projectFolderName;
            if (args.Length > 0)
            {
                datafilename = args[0];
                Console.WriteLine("File: {0}", datafilename);
            }
            else
            {
                Console.WriteLine("No source file supplied. Aborting.");
                PrintHelp();
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return (int)ERRORVALUE.NoSourceFile;
            }
            if (args.Length > 1)
            {
                inifilename = args[1];
                Console.WriteLine("INI File: {0}", inifilename);
            }
            else
            {
                Console.WriteLine("No data mapping file supplied (expected ini). Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return (int)ERRORVALUE.NoDataMapping;
            }
            if (args.Length > 2)
            {
                projectFolderName = args[2];
                Console.WriteLine("Project Folder: {0}", projectFolderName);
            }
            else
            {
                Console.WriteLine("No project folder supplied. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return (int)ERRORVALUE.NoProject;
            }
            #endregion

            #region Validating Inputs
            if (!File.Exists(datafilename))
            {
                Console.WriteLine("No source file supplied. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return (int)ERRORVALUE.NoSourceFile;
            }

            if(!File.Exists(inifilename))
            {
                Console.WriteLine("ini data mapping not found. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                return (int)ERRORVALUE.NoDataMapping;
            }

            if (!Directory.Exists(projectFolderName))
            {
                // try creating the directory
                bool created = true;

                try
                {
                    // check to see if trailing charcter closes 
                    Directory.CreateDirectory(projectFolderName);
                }
                catch
                {
                    created = false;
                }

                if(!created)
                {
                    // couldn't create directory.
                    Console.WriteLine("Output folder did not exist and couldn't be created.");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();
                    return (int)ERRORVALUE.InvalidProject;
                }
            }
            #endregion

            // switch on file extension - if dll, use dll splitter
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(datafilename);

            return (fileInfo.Extension.ToLower().Contains("dll")) ? SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName) :
                Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
        }
	}
}