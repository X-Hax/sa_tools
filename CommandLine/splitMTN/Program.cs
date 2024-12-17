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
			string filename;
			if (argq.Count > 0)
			{
				filename = argq.Dequeue();
				Console.WriteLine("File: {0}", filename);
				SA2MTN.Split(filename);
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
				SA2MTN.Split(filename);
			}
		}
	}
}