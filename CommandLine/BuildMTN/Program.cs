using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace buildMTN
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0 || (args.Length > 0 && (args[0] == "-h" || args[0] == "--help")))
			{
				Console.WriteLine("BuildMTN is a command line tool for building SA2 MTN files from a folder of motion data.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("BuildMTN [/be|/le] <folder>\n");
				Console.WriteLine("/be: Use big-endian byte order");
				Console.WriteLine("/le: Use little-endian byte order (default)");
				Console.WriteLine("Note: Little-endian is used by default but big-endian will be used if specified in the input folder.");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}

			var argQueue = new Queue<string>(args);
			
			bool? bigEndian = null;
			
			if (argQueue.Count > 0)
			{
				if (argQueue.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
				{
					bigEndian = true;
					argQueue.Dequeue();
				}
				else if (argQueue.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
				{
					bigEndian = false;
					argQueue.Dequeue();
				}
			}

			string mtnFileName;
			if (argQueue.Count > 0)
			{
				mtnFileName = argQueue.Dequeue();
				Console.WriteLine("File: {0}", mtnFileName);
			}
			else
			{
				Console.Write("File: ");
				mtnFileName = Console.ReadLine().Trim('"');
			}

			SA2MTN.Build(bigEndian, mtnFileName);

			/*
			if (argq.Count > 0)
			{
				mtnFileName = argq.Dequeue();
				Console.WriteLine("File: {0}", mtnFileName);

				FileAttributes file = File.GetAttributes(mtnFileName);

				if (file.HasFlag(FileAttributes.Directory))
					SplitMTN.Build(be, mtnFileName);

				else
					SplitMTN.Split(be, mtnFileName);

			}
			else
			{
				Console.Write("File: ");
				mtnFileName = Console.ReadLine().Trim('"');

				FileAttributes file = File.GetAttributes(mtnFileName);

				if (file.HasFlag(FileAttributes.Directory))
					SplitMTN.Build(be, mtnFileName);

				else
					SplitMTN.Split(be, mtnFileName);
			}
			*/
		}
	}
}