using SA_Tools.SAArc;
using System;
using System.Collections.Generic;

namespace buildMTN
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
			string mtnFileName;
			if (argq.Count > 0)
			{
				mtnFileName = argq.Dequeue();
				Console.WriteLine("File: {0}", mtnFileName);
				sa2MTN.Build(be, mtnFileName);
			}
			else
			{
				Console.Write("File: ");
				mtnFileName = Console.ReadLine().Trim('"');
				sa2MTN.Build(be, mtnFileName);
			}
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