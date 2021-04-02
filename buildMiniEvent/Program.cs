using SA_Tools.SAArc;
using System;

namespace buildMiniEvent
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Write("Filename: ");
				args = new string[] { Console.ReadLine().Trim('"') };
			}
			foreach (string filename in args)
			{
				SA2MiniEvent.Build(filename);
			}
		}
	}
}
