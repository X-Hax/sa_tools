using SplitTools.SAArc;
using System;
using System.Collections.Generic;

namespace splitMTN
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
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