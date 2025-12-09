using System;

namespace TextureTool
{
	internal class MiscFunctions
	{
		public static int FindArgument(string[] args, string arg)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == arg)
					return i;
			}
			return -1;
		}

		public static void ShowUsage()
		{
			Console.WriteLine("No arguments");
		}
	}
}
