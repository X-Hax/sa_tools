using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace BuildMDL
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
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

			sa2MDL.Build(bigEndian, mdlFilename);
			
			/*
			if (argQueue.Count > 0)
			{
				mdlFilename = argQueue.Dequeue();
				Console.WriteLine("File: {0}", mdlFilename);

				var file = File.GetAttributes(mdlFilename);

				if (file.HasFlag(FileAttributes.Directory))
				{
					sa2MDL.Build(bigEndian, mdlFilename);
				}
				else
				{
					sa2MDL.Split(mdlFilename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(mdlFilename)), argQueue.ToArray());
				}
			}
			else
			{
				Console.Write("File: ");
				mdlFilename = Console.ReadLine().Trim('"');

				var file = File.GetAttributes(mdlFilename);

				if (file.HasFlag(FileAttributes.Directory))
				{
					sa2MDL.Build(be, mdlFilename);
				}
				else
				{
					sa2MDL.Split(mdlFilename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(mdlFilename)), argQueue.ToArray());
				}
			}
			*/
		}
	}
}