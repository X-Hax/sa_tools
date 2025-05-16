using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace buildMTN
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