using System;
using System.Collections.Generic;
using System.IO;
using ArchiveLib;
using SA_Tools;

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
                        string fn = args[1];
                        Console.WriteLine("Extracting PAK file: {0}", Path.GetFullPath(fn));
                        string outputPath = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn));
                        Console.WriteLine("Output folder: {0}", Path.GetFullPath(outputPath));
                        PAKFile pak = new PAKFile(fn);
                        foreach (PAKFile.PAKEntry entry in pak.Entries)
                        {
                            Console.WriteLine("Extracting file: {0}", entry.Name);
                            File.WriteAllBytes(Path.Combine(outputPath, entry.Name), entry.GetBytes());
                        }
                        pak.CreateIndexFile(outputPath);
                        Console.WriteLine("Archive extracted!");
                    }
					catch (Exception ex) { Console.WriteLine(ex.ToString()); }
					break;
				case "/p":
					try
					{
                        Console.WriteLine("Building PAK from folder: {0}", Path.GetFullPath(args[1]));
                        string outputPath = Path.Combine(Environment.CurrentDirectory, args[1]);
                        Environment.CurrentDirectory = Path.GetDirectoryName(outputPath);
                        Dictionary<string, PAKFile.PAKIniItem> list = IniSerializer.Deserialize<Dictionary<string, PAKFile.PAKIniItem>>(Path.Combine(Path.GetFileNameWithoutExtension(outputPath), Path.GetFileNameWithoutExtension(outputPath) + ".ini"));
                        PAKFile pak = new PAKFile();
                        foreach (KeyValuePair<string, PAKFile.PAKIniItem> item in list)
                        {
                            Console.WriteLine("Adding file: {0}", item.Key);
                            pak.Entries.Add(new PAKFile.PAKEntry(item.Key, item.Value.LongPath, File.ReadAllBytes(item.Key)));
                        }
                        Console.WriteLine("Output file: {0}", Path.ChangeExtension(outputPath, "pak"));
                        pak.Save(Path.ChangeExtension(outputPath, "pak"));
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