using SplitTools.SAArc;
using System;

namespace splitMiniEvent
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
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			foreach (string filename in args)
			{
				SA2MiniEvent.Split(filename);
			}
		}
	}
}