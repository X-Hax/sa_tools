using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PAKLib;

namespace PAKtool
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			if (args.Length == 0)
				args = new string[] { "/?" };
			switch (args[0].ToLowerInvariant())
			{
				case "/t":
					Main(ParseCommandLine(File.ReadAllText(args[1])));
					break;
				case "/u":
					try
					{
						string fn = Path.Combine(Environment.CurrentDirectory, args[1]);
						Environment.CurrentDirectory = Path.GetDirectoryName(fn);
						PAKFile pak = new PAKFile(fn);
						Dictionary<string, PAKInfo> list = new Dictionary<string, PAKInfo>(pak.Files.Count);
						foreach (PAKFile.File item in pak.Files)
						{
							Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(Environment.CurrentDirectory, item.Name)));
							File.WriteAllBytes(item.Name, item.Data);
							list.Add(item.Name, new PAKInfo(item.LongPath));
						}
						IniFile.Serialize(list, Path.ChangeExtension(fn, "ini"));
					}
					catch (Exception ex) { Console.WriteLine(ex.ToString()); }
					break;
				case "/p":
					try
					{
						string fn = Path.Combine(Environment.CurrentDirectory, args[1]);
						Environment.CurrentDirectory = Path.GetDirectoryName(fn);
						Dictionary<string, PAKInfo> list = IniFile.Deserialize<Dictionary<string, PAKInfo>>(Path.ChangeExtension(fn, "ini"));
						PAKFile pak = new PAKFile();
						foreach (KeyValuePair<string, PAKInfo> item in list)
							pak.Files.Add(new PAKFile.File(item.Key, item.Value.LongPath, File.ReadAllBytes(item.Key)));
						pak.Save(Path.ChangeExtension(fn, "pak"));
					}
					catch (Exception ex) { Console.WriteLine(ex.ToString()); }
					break;
				case "/?":
					Console.WriteLine("Arguments:");
					Console.WriteLine();
					Console.WriteLine("/?\tShow this help.");
					Console.WriteLine();
					Console.WriteLine("/t filename\tReads text file filename as a commandline.");
					Console.WriteLine();
					Console.WriteLine("/u filename\tExtracts files from an archive.");
					Console.WriteLine();
					Console.WriteLine("/p filename\tPacks files into an archive.");
					Console.WriteLine();
					break;
				default:
					if (args.Length == 0) goto case "/?";
					char arg = '\0';
					while (arg != 'u' & arg != 'p')
					{
						Console.Write("Type u to unpack or p to pack: ");
						arg = Console.ReadKey().KeyChar;
						Console.WriteLine();
					}
					Main(new string[] { "/" + arg, args[0] });
					break;
			}
		}

		public static string[] ParseCommandLine(string commandLine)
		{
			List<string> args2 = new List<string>();
			string curcmd = string.Empty;
			bool quotes = false;
			foreach (char item in commandLine)
			{
				switch (item)
				{
					case ' ':
						if (!quotes)
						{
							if (!string.IsNullOrEmpty(curcmd))
								args2.Add(curcmd);
							curcmd = string.Empty;
						}
						else
							goto default;
						break;
					case '"':
						if (quotes)
						{
							args2.Add(curcmd);
							curcmd = string.Empty;
							quotes = false;
						}
						else
							quotes = true;
						break;
					default:
						curcmd += item;
						break;
				}
			}
			if (!string.IsNullOrEmpty(curcmd))
				args2.Add(curcmd);
			return args2.ToArray();
		}
	}

	public class PAKInfo
	{
		public PAKInfo() { }

		public PAKInfo(string longpath) { LongPath = longpath; }

		public string LongPath { get; set; }
	}
}