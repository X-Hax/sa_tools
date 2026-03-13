using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace BuildMDL
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length == 0 || (args.Length > 0 && (args[0] == "-h" || args[0] == "--help")))
			{
				Console.WriteLine("BuildMDL is a command line tool for building SA2 MDL files from a folder of model data.\n");
				Console.WriteLine("Usage:\n");
				Console.WriteLine("BuildMDL [/be|/le] <folder>\n");
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

			string mdlFilename;
			if (argQueue.Count > 0)
			{
				mdlFilename = argQueue.Dequeue();
				Console.WriteLine("File: {0}", mdlFilename);
			}
			else
			{
				Console.WriteLine("File: ");
				mdlFilename = Console.ReadLine().Trim('"');
			}

			SA2MDL.Build(bigEndian, mdlFilename);
			
			/*
			if (argQueue.Count > 0)
			{
				mdlFilename = argQueue.Dequeue();
				Console.WriteLine("File: {0}", mdlFilename);

				var file = File.GetAttributes(mdlFilename);

				if (file.HasFlag(FileAttributes.Directory))
				{
					SA2MDL.Build(bigEndian, mdlFilename);
				}
				else
				{
					SA2MDL.Split(mdlFilename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(mdlFilename)), argQueue.ToArray());
				}
			}
			else
			{
				Console.Write("File: ");
				mdlFilename = Console.ReadLine().Trim('"');

				var file = File.GetAttributes(mdlFilename);

				if (file.HasFlag(FileAttributes.Directory))
				{
					SA2MDL.Build(be, mdlFilename);
				}
				else
				{
					SA2MDL.Split(mdlFilename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(mdlFilename)), argQueue.ToArray());
				}
			}
			*/
		}
	}
}