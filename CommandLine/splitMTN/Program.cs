using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace splitMTN
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0 || (args.Length > 0 && (args[0] == "-h" || args[0] == "--help")))
			{
				Console.WriteLine("SplitMTN is a command line tool for extracting SA2 MTN files into a new folder.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("SplitMTN <file>\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}

			var argQueue = new Queue<string>(args);
			
			string filename;
			
			if (argQueue.Count > 0)
			{
				filename = argQueue.Dequeue();
				Console.WriteLine("File: {0}", filename);
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}

			SA2MTN.Split(filename);
		}
	}
}