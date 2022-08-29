using SplitTools.SAArc;
using System;
using System.IO;

namespace splitEventExtra
{
    class Program
    {
        static void Main(string[] args)
		{
			string fullpath_out;
			string fullpath_bin = Path.GetFullPath(args[0]);
			string name = Path.GetFileName(fullpath_bin);
			if (!File.Exists(fullpath_bin))
			{
				Console.WriteLine("File {0} doesn't exist.", fullpath_bin);
				return;
			}
			if (args.Length == 0)
			{
				Console.Write("Filename: ");
				args = new string[] { Console.ReadLine().Trim('"') };
			}
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			fullpath_out = Path.GetDirectoryName(fullpath_bin);
			if (args.Length > 1)
			{
				fullpath_out = args[1];
				if (fullpath_out[fullpath_out.Length - 1] != '/') fullpath_out = string.Concat(fullpath_out, '/');
				fullpath_out = Path.GetFullPath(fullpath_out);
			}
			Console.WriteLine("Output folder: {0}", fullpath_out);
			if (name.StartsWith("me") && name.EndsWith (".scr"))
				sa2MiniEventExtra.Split(fullpath_bin, fullpath_out);
			else if (name.StartsWith("e") && name.EndsWith("_0.prs"))
				sa2EventExtra.Split(fullpath_bin, fullpath_out);
			else
				sa2EventExtra.SplitLanguage(fullpath_bin, fullpath_out);
		}
	}
}
