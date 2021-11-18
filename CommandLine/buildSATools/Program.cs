using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace buildSATools
{
    partial class Program
	{
        static void Main(string[] args)
        {
            // Check if the build was already rearranged
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "build", "SAToolsHub.deps.json")))
                goto package;
            // Clean up leftovers from previous build
            List<string> refdirlist = new List<string>();
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "bin", "lib"));
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "tools", "lib"));
            DeleteDirs(refdirlist);
            Console.WriteLine("Patching EXE files...");
            DirectoryInfo d = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "build"));
            FileInfo[] files = d.GetFiles("*.exe", SearchOption.AllDirectories);
            foreach (FileInfo exefile in files)
            {
                switch (exefile.Name.ToLowerInvariant())
                {
                    case "buildsatools.exe":
                        break;
                    case "satoolshub.exe":
                        PatchExe(exefile.FullName, "tools\\lib");
                        break;
                    default:
                        PatchExe(exefile.FullName, "lib");
                        break;
                }
            }
            Console.WriteLine("Deleting reference assemblies");
            refdirlist = new List<string>();
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "ref"));
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "lib", "ref"));
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "tools", "ref"));
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "bin", "ref"));
            DeleteDirs(refdirlist);
            Console.WriteLine("Moving all DLL files to the lib folder");
            FileInfo[] dllfiles = d.GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (FileInfo dllfile in dllfiles)
            {
                string fn = dllfile.FullName.ToLowerInvariant();
                if (fn.Contains("buildsatools"))
                    continue;
                // Skip platform-specific runtimes in the runtimes folder
                if (fn.Contains("runtimes"))
                    continue;
                Console.WriteLine("Moving: {0}", dllfile.FullName);
                File.Move(dllfile.FullName, Path.Combine(Environment.CurrentDirectory, "build", "lib", dllfile.Name), true);
            }
            Console.WriteLine("Moving all JSON files to the lib folder");
            FileInfo[] jsonfiles = d.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (FileInfo jsonfile in jsonfiles)
            {
                Console.WriteLine("Moving: {0}", jsonfile.FullName);
                File.Move(jsonfile.FullName, Path.Combine(Environment.CurrentDirectory, "build", "lib", jsonfile.Name), true);
            }
            Console.WriteLine("Copying lib folder to bin and tools");
            DirectoryCopy(Path.Combine(Environment.CurrentDirectory, "build", "tools", "runtimes"), Path.Combine(Environment.CurrentDirectory, "build", "lib", "runtimes"), true);
            DirectoryCopy(Path.Combine(Environment.CurrentDirectory, "build", "lib"), Path.Combine(Environment.CurrentDirectory, "build", "bin", "lib"), true);
            DirectoryCopy(Path.Combine(Environment.CurrentDirectory, "build", "lib"), Path.Combine(Environment.CurrentDirectory, "build", "tools", "lib"), true);
            Console.WriteLine("Deleting runtimes folders");
            refdirlist = new List<string>();
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "runtimes"));
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "bin", "runtimes"));
            refdirlist.Add(Path.Combine(Environment.CurrentDirectory, "build", "tools", "runtimes"));
            DeleteDirs(refdirlist);
            Console.WriteLine("Deleting original lib folder");
            Directory.Delete(Path.Combine(Environment.CurrentDirectory, "build", "lib"), true);
            Console.WriteLine("Deleting runtimeconfig.dev.json files and irrelevant runtimes");
            DirectoryInfo newd = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "build"));
            FileInfo[] devf = newd.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo devfile in devf)
            {
                string f = devfile.FullName.ToLowerInvariant();
                if (f.Contains("dev.json") || f.Contains("freebsd") || f.Contains("linux") || f.Contains("osx") || f.Contains("unix") || f.Contains("arm64"))
                    File.Delete(devfile.FullName);
                else if (Environment.Is64BitProcess && f.Contains("win-x86"))
                    File.Delete(devfile.FullName);
                else if (!Environment.Is64BitProcess && f.Contains("win-x64"))
                    File.Delete(devfile.FullName);
            }
            // Proceed to create the end user package
            package:
            string outdir = "output";
            if (args.Length > 0)
            {
                if (args[0] == "-noout") return;
                outdir = args[0];
            }
            // Set output directory
            int retries = 0;
            Console.WriteLine("Output directory: {0}", Path.GetFullPath(outdir));
            while (true)
                try
                {
                    DoStuff(outdir);
                    break;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    if (retries < 1000)
                    {
                        retries++;
                        Console.Write(ex.Message + " Trying again...\n");
                    }
                    else
                        throw;
                }
        }

        private static void DeleteDirs(List<string> refdirlist)
        {
            foreach (string refd in refdirlist)
                if (Directory.Exists(refd))
                    Directory.Delete(refd, true);
        }

        private static void DoStuff(string outdir)
        {
            string[] script = File.ReadAllLines("BuildScript.ini");
            Directory.CreateDirectory(outdir);
            for (int i = 0; i < script.Length; i++)
            {
                string[] srcdest = script[i].Split('=');
                Console.WriteLine("Source: {1}, Destination: {0}", srcdest[0], srcdest[1]);
                if (File.Exists(srcdest[1]))
                {
                    File.Copy(srcdest[1], Path.Combine(outdir, srcdest[0]), true);
                }
                else if (Directory.Exists(srcdest[1]))
                {
                    DirectoryCopy(srcdest[1], Path.Combine(outdir, srcdest[0]), true);
                }
                else
                {
                    Console.WriteLine("{0} does not exist", srcdest[1]);
                }
            }
        }

		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			// If the source directory does not exist, throw an exception.
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory does not exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}


			// Get the file contents of the directory to copy.
			System.IO.FileInfo[] files = dir.GetFiles();

			foreach (System.IO.FileInfo file in files)
			{
				// Create the path to the new copy of the file.
				string temppath = Path.Combine(destDirName, file.Name);

				// Copy the file.
				file.CopyTo(temppath, true);
			}

			// If copySubDirs is true, copy the subdirectories.
			if (copySubDirs)
			{

				foreach (DirectoryInfo subdir in dirs)
				{
					// Create the subdirectory.
					string temppath = Path.Combine(destDirName, subdir.Name);

					// Copy the subdirectories.
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}

        // Patches an EXE file to load the assembly from the "lib" folder
        private static int PatchExe(string apphostExe, string libDirPath)
        {
            Console.WriteLine("Patching {0} to use {1}", Path.GetFileName(apphostExe), libDirPath);
            try
            {
                string origPath = Path.GetFileName(ChangeExecutableExtension(apphostExe));
                string newPath = libDirPath + GetPathSeparator(apphostExe) + origPath;
                if (!File.Exists(apphostExe))
                {
                    Console.WriteLine($"Apphost '{apphostExe}' does not exist");
                    return 1;
                }
                if (origPath == string.Empty)
                {
                    Console.WriteLine("Original path is empty");
                    return 1;
                }
                var origPathBytes = Encoding.UTF8.GetBytes(origPath + "\0");
                Debug.Assert(origPathBytes.Length > 0);
                var newPathBytes = Encoding.UTF8.GetBytes(newPath + "\0");
                if (origPathBytes.Length > maxPathBytes)
                {
                    Console.WriteLine($"Original path is too long");
                    return 1;
                }
                if (newPathBytes.Length > maxPathBytes)
                {
                    Console.WriteLine($"New path is too long");
                    return 1;
                }
                var apphostExeBytes = File.ReadAllBytes(apphostExe);
                int offset = GetOffset(apphostExeBytes, origPathBytes);
                if (offset < 0)
                {
                    Console.WriteLine($"Could not find original path '{origPath}'");
                    return 1;
                }

                // Don't patch if the string already has the lib folder in it
                string originalName = System.Text.Encoding.UTF8.GetString(apphostExeBytes, 0, apphostExeBytes.Length);
                if (originalName.Contains("lib\\"))
                {
                    Console.WriteLine("File is already patched");
                    return 1;
                }

                if (offset + newPathBytes.Length > apphostExeBytes.Length)
                {
                    Console.WriteLine($"New path is too long: {newPath}");
                    return 1;
                }
                for (int i = 0; i < newPathBytes.Length; i++)
                    apphostExeBytes[offset + i] = newPathBytes[i];
                File.WriteAllBytes(apphostExe, apphostExeBytes);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }

    }
}