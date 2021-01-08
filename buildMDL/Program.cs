using SA_Tools.SAArc;
using System;
using System.Collections.Generic;

namespace buildMDL
{
	class Program
	{
		static void Main(string[] args)
		{
			Queue<string> argq = new Queue<string>(args);
			bool? be = null;
			if (argq.Count > 0)
				if (argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
				{
					be = true;
					argq.Dequeue();
				}
				else if (argq.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
				{
					be = false;
					argq.Dequeue();
				}
			string mdlfilename;
			if (argq.Count > 0)
			{
				mdlfilename = argq.Dequeue();
				Console.WriteLine("File: {0}", mdlfilename);
				sa2MDL.Build(be, mdlfilename);
			}
			else
			{
				Console.WriteLine("File: ");
				mdlfilename = Console.ReadLine().Trim('"');
				sa2MDL.Build(be, mdlfilename);
			}
			/*
			if (argq.Count > 0)
			{
				mdlfilename = argq.Dequeue();
				Console.WriteLine("File: {0}", mdlfilename);

				FileAttributes file = File.GetAttributes(mdlfilename);

				if (file.HasFlag(FileAttributes.Directory))
					SplitMDL.Build(be, mdlfilename);

				else
					SplitMDL.Split(be, mdlfilename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(mdlfilename)), argq.ToArray());

			}
			else
			{
				Console.Write("File: ");
				mdlfilename = Console.ReadLine().Trim('"');

				FileAttributes file = File.GetAttributes(mdlfilename);

				if (file.HasFlag(FileAttributes.Directory))
					SplitMDL.Build(be, mdlfilename);

				else
					SplitMDL.Split(be, mdlfilename, Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(mdlfilename)), argq.ToArray());
			}
			*/
		}
	}
}