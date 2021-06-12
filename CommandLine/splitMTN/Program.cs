using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace splitMTN
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
			string filename;
			if (argq.Count > 0)
			{
				filename = argq.Dequeue();
				Console.WriteLine("File: {0}", filename);
				sa2MTN.Split(be, filename);
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
				sa2MTN.Split(be, filename);
			}
		}
	}
}