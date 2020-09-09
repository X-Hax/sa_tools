using System;
using System.IO;

namespace SplitNB
{
	class Program
	{
		static void Main(string[] args)
		{
			bool verbose = false;
			bool extractchunks = false;
			string filename;
			if (args.Length > 0)
			{
				filename = args[0];
				Console.WriteLine("Input: {0}", filename);
				for (int a = 0; a < args.Length; a++)
				{
					if (args[a] == "-b") extractchunks = true;
					if (args[a] == "-v") verbose = true;
				}
			}
			else
			{
				Console.WriteLine("SplitNB extracts and builds NB files.\n");
				Console.WriteLine("Usage:");
				Console.WriteLine("Extract an NB file: SplitNB <NB file> [-b] [-v]");
				Console.WriteLine("Build an NB file: SplitNB <INI file> [-v]\n");
				Console.WriteLine("Arguments:");
				Console.WriteLine("-b: extract binary chunks from the NB file");
				Console.WriteLine("-v: verbose\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			if (!File.Exists(filename))
			{
				Console.WriteLine("File {0} doesn't exist.", filename);
				Console.ReadLine();
				return;
			}
			Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(filename));
			string ext = Path.GetExtension(filename.ToLowerInvariant());
			switch (ext)
			{
				case ".nb":
					if (!Directory.Exists(Path.GetFileNameWithoutExtension(filename)))
						Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
					SA_Tools.Split.SplitNB.SplitNBFile(filename, extractchunks, Path.GetFileNameWithoutExtension(filename), verbose);
					return;
				case ".ini":
					SA_Tools.Split.SplitNB.BuildNBFile(filename, Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(filename) + "_new.NB"), verbose);
					return;
			}
		}
	}
}