﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace buildSATools
{
	partial class Program
	{
		private enum ProgramMode
		{
			Normal,
			NoBuildScript,
			NoPackage
		};

		static void Main(string[] args)
		{
			int retries = 0;
			ProgramMode mode = ProgramMode.Normal;
			string srcdir = Environment.CurrentDirectory;
			string outdir = Path.Combine(Environment.CurrentDirectory, "output");
			bool clean = false;
			// Process command line arguments
			if (args.Length == 0)
				Console.WriteLine("For usage, run buildsatools.exe -help\n");
			else
				for (int a = 0; a < args.Length; a++)
				{
					switch (args[a])
					{
						case "-help":
							Console.WriteLine("This program creates a ready to use SA Tools package.");
							Console.WriteLine("It requires a folder containing the SA Tools solution which has been built at least once.");
							Console.WriteLine("\nUsage: buildtools.exe [-root <folder>] [-clean] [-noscript] [-nopackage] [output folder]");
							Console.WriteLine("\nArguments:");
							Console.WriteLine("-root : Specify the solution's root folder (default is current folder)");
							Console.WriteLine("-clean : Empty the output folder before copying files to it");
							Console.WriteLine("-nopackage : Only process BuildScript.ini");
							Console.WriteLine("-noscript : Do not process BuildScript.ini");
							Console.WriteLine("output folder : Put the complete package in the specified folder (default is 'output' in the current folder)");
							Console.WriteLine("\nPress ENTER to exit.");
							Console.ReadLine();
							return;
						case "-root":
							srcdir = (args[a + 1]);
							a++;
							break;
						case "-clean":
							clean = true;
							break;
						case "-nopackage":
							mode = ProgramMode.NoPackage;
							break;
						case "-noscript":
							mode = ProgramMode.NoBuildScript;
							break;
						default:
							outdir = args[a];
							break;
					}
				}
			string builddir = Path.Combine(srcdir, "build");
			Console.WriteLine("Build folder: {0}", builddir);
			if (!Directory.Exists(builddir))
			{
				Console.WriteLine("Build directory doesn't exist!");
				return;
			}
			// Clean the output folder
			while (true)
				try
				{
					if (clean && Directory.Exists(outdir))
					{
						Directory.Delete(outdir, true);
						Directory.CreateDirectory(outdir);
						Console.WriteLine("Output folder cleaned.");
					}
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
			// Process build script
			if (mode != ProgramMode.NoBuildScript)
			{
				Console.WriteLine("\nProcessing build script...");
				while (true)
					try
					{
						ProcessBuildScript(srcdir, outdir);
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
			// Create package
			if (mode == ProgramMode.NoPackage)
			{
				Console.WriteLine("\nFinished!");
				return;
			}
			// Check if the build was already rearranged
			if (!File.Exists(Path.Combine(outdir, "SAToolsHub.deps.json")))
			{
				Console.WriteLine("Build is already packaged or output folder is missing files.");
				return;
			}
			while (true)
				try
				{
					// Clean up leftovers from previous build
					List<string> refdirlist = new List<string>();
					refdirlist.Add(Path.Combine(outdir, "bin", "lib"));
					refdirlist.Add(Path.Combine(outdir, "tools", "lib"));
					DeleteDirs(refdirlist);
					Console.WriteLine("\nPatching EXE files...");
					DirectoryInfo d = new DirectoryInfo(Path.Combine(outdir));
					FileInfo[] files = d.GetFiles("*.exe", SearchOption.AllDirectories);
					foreach (FileInfo exefile in files)
					{
						switch (exefile.Name.ToLowerInvariant())
						{
							case "7z.exe":
								break;
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
					Console.WriteLine("\nDeleting reference assemblies...");
					refdirlist = new List<string>()
					{
					Path.Combine(outdir, "ref"),
					Path.Combine(outdir, "lib", "ref"),
					Path.Combine(outdir, "tools", "ref"),
					Path.Combine(outdir, "bin", "ref")
					};
					DeleteDirs(refdirlist);
					Console.WriteLine("\nMoving all DLL files to the lib folder...");
					FileInfo[] dllfiles = d.GetFiles("*.dll", SearchOption.AllDirectories);
					foreach (FileInfo dllfile in dllfiles)
					{
						string fn = dllfile.FullName.ToLowerInvariant();
						if (fn.Contains("buildsatools"))
							continue;
						// Skip platform-specific runtimes in the runtimes folder
						if (fn.Contains("runtimes"))
							continue;
						Console.WriteLine("\tMoving: {0}", dllfile.FullName);
						File.Move(dllfile.FullName, Path.Combine(outdir, "lib", dllfile.Name), true);
					}
					Console.WriteLine("\nMoving all JSON files to the lib folder...");
					FileInfo[] jsonfiles = d.GetFiles("*.json", SearchOption.AllDirectories);
					foreach (FileInfo jsonfile in jsonfiles)
					{
						Console.WriteLine("\tMoving: {0}", jsonfile.FullName);
						File.Move(jsonfile.FullName, Path.Combine(outdir, "lib", jsonfile.Name), true);
					}
					Console.WriteLine("\nCopying lib folder to bin and tools...");
					DirectoryCopy(Path.Combine(outdir, "tools", "runtimes"), Path.Combine(outdir, "lib", "runtimes"), true);
					DirectoryCopy(Path.Combine(outdir, "lib"), Path.Combine(outdir, "bin", "lib"), true);
					DirectoryCopy(Path.Combine(outdir, "lib"), Path.Combine(outdir, "tools", "lib"), true);
					Console.WriteLine("\nDeleting runtimes folders...");
					refdirlist = new List<string>()
					{
					Path.Combine(outdir, "runtimes"),
					Path.Combine(outdir, "bin", "runtimes"),
					Path.Combine(outdir, "tools", "runtimes"),
					Path.Combine(outdir, "tools", "Properties"),
					};
					DeleteDirs(refdirlist);
					Console.WriteLine("\nDeleting original lib folder...");
					Directory.Delete(Path.Combine(outdir, "lib"), true);
					Console.WriteLine("\nDeleting runtimeconfig.dev.json files and irrelevant runtimes...");
					DirectoryInfo newd = new DirectoryInfo(Path.Combine(outdir));
					FileInfo[] devf = newd.GetFiles("*.*", SearchOption.AllDirectories);
					foreach (FileInfo devfile in devf)
					{
						string f = devfile.FullName.ToLowerInvariant();
						if (f.Contains("dev.json") || f.Contains("freebsd") || f.Contains("linux") || f.Contains("osx") || f.Contains("unix") || f.Contains("arm64") || f.Contains("ios") || f.Contains("solaris") || f.Contains("tvos") || f.Contains("illumos"))
							File.Delete(devfile.FullName);
						else if (Environment.Is64BitProcess && f.Contains("win-x86"))
							File.Delete(devfile.FullName);
						else if (!Environment.Is64BitProcess && f.Contains("win-x64"))
							File.Delete(devfile.FullName);
					}
					Console.WriteLine("\nCreating manifest...");
					CreateFolderManifest(Path.GetFullPath(outdir), Path.Combine(outdir, "satools.manifest"));
					Console.WriteLine("\nOutput folder: {0}", outdir);
					Console.WriteLine("Finished!");
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

		private static void CreateFolderManifest(string fullpath, string outputfile)
		{
			List<string> manifest = new List<string>();
			// Create a DirectoryInfo object representing the specified directory.
			var dir = new DirectoryInfo(fullpath);
			FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
			using (System.Security.Cryptography.SHA256 mySHA256 = System.Security.Cryptography.SHA256.Create())
			{
				foreach (FileInfo fInfo in files)
				{
					using (FileStream fileStream = fInfo.Open(FileMode.Open))
					{
						try
						{
							// Get relative path
							string shortname = Path.GetRelativePath(fullpath, fInfo.FullName);
							// Create a fileStream for the file.
							// Be sure it's positioned to the beginning of the stream.
							fileStream.Position = 0;
							// Compute the hash of the fileStream.
							byte[] hashValue = mySHA256.ComputeHash(fileStream);
							// Write the name and hash value of the file.
							manifest.Add(string.Format("{0}\t{1}\t{2}", shortname, fInfo.Length.ToString(), SHA256ToString(hashValue)));
						}
						catch (Exception e)
						{
							Console.WriteLine($"Exception: {e.Message}");
						}
					}
				}
			}
			File.WriteAllLines(outputfile, manifest.ToArray());
		}

		// Display the byte array in a readable format.
		private static string SHA256ToString(byte[] array)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
				stringBuilder.Append($"{array[i]:x2}");
			return stringBuilder.ToString();
		}

		private static void DeleteDirs(List<string> refdirlist)
		{
			foreach (string refd in refdirlist)
				if (Directory.Exists(refd))
					Directory.Delete(refd, true);
		}

		private static void ProcessBuildScript(string startdir, string outdir)
		{
			Console.WriteLine("Source folder: {0}", startdir);
			string[] script = File.ReadAllLines(Path.Combine(startdir, "BuildScript.ini"));
			Directory.CreateDirectory(outdir);
			for (int i = 0; i < script.Length; i++)
			{
				string[] srcdest = script[i].Split('=');
				Console.WriteLine("\tSource: {1}, Destination: {0}", srcdest[0], srcdest[1]);
				// Copy file
				if (File.Exists(Path.Combine(startdir, srcdest[1])))
					File.Copy(Path.Combine(startdir, srcdest[1]), Path.Combine(outdir, srcdest[0]), true);
				// Copy folder
				else if (Directory.Exists(Path.Combine(startdir, srcdest[1])))
					DirectoryCopy(Path.Combine(startdir, srcdest[1]), Path.Combine(outdir, srcdest[0]), true);
				// If neither exist
				else
					Console.WriteLine("\t\t{0} does not exist", srcdest[1]);
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
			Console.WriteLine("\tPatching {0} to use {1}", apphostExe, libDirPath);
			try
			{
				string origPath = Path.GetFileName(ChangeExecutableExtension(apphostExe));
				string newPath = libDirPath + GetPathSeparator(apphostExe) + origPath;
				if (!File.Exists(apphostExe))
				{
					Console.WriteLine($"\t\tApphost '{apphostExe}' does not exist");
					return 1;
				}
				if (origPath == string.Empty)
				{
					Console.WriteLine("\t\tOriginal path is empty");
					return 1;
				}
				var origPathBytes = Encoding.UTF8.GetBytes(origPath + "\0");
				Debug.Assert(origPathBytes.Length > 0);
				var newPathBytes = Encoding.UTF8.GetBytes(newPath + "\0");
				if (origPathBytes.Length > maxPathBytes)
				{
					Console.WriteLine($"\t\tOriginal path is too long");
					return 1;
				}
				if (newPathBytes.Length > maxPathBytes)
				{
					Console.WriteLine($"\t\tNew path is too long");
					return 1;
				}
				var apphostExeBytes = File.ReadAllBytes(apphostExe);
				int offset = GetOffset(apphostExeBytes, origPathBytes);
				if (offset < 0)
				{
					Console.WriteLine($"\t\tCould not find original path '{origPath}'");
					return 1;
				}

				// Don't patch if the string already has the lib folder in it
				string originalName = System.Text.Encoding.UTF8.GetString(apphostExeBytes, 0, apphostExeBytes.Length);
				if (originalName.Contains("lib\\"))
				{
					Console.WriteLine("\t\tFile is already patched");
					return 1;
				}

				if (offset + newPathBytes.Length > apphostExeBytes.Length)
				{
					Console.WriteLine($"\t\tNew path is too long: {newPath}");
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