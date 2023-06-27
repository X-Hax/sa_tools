using SplitTools.SAArc;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace buildEvent
{
	static class Program
	{
		public class Wildcard : Regex
		{
			/// <summary>
			/// Initializes a wildcard with the given search pattern.
			/// </summary>
			/// <param name="pattern">The wildcard pattern to match.</param>
			public Wildcard(string pattern)
			 : base(WildcardToRegex(pattern))
			{
			}

			/// <summary>
			/// Initializes a wildcard with the given search pattern and options.
			/// </summary>
			/// <param name="pattern">The wildcard pattern to match.</param>
			/// <param name="options">A combination of one or more
			/// <see cref="System.Text.RegexOptions"/>.</param>
			public Wildcard(string pattern, RegexOptions options)
			 : base(WildcardToRegex(pattern), options)
			{
			}

			/// <summary>
			/// Converts a wildcard to a regex.
			/// </summary>
			/// <param name="pattern">The wildcard pattern to convert.</param>
			/// <returns>A regex equivalent of the given wildcard.</returns>
			public static string WildcardToRegex(string pattern)
			{
				return "^" + Regex.Escape(pattern).
				 Replace("\\*", ".*").
				 Replace("\\?", ".") + "$";
			}
		}
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("This program builds SA2 Event files with optional settings.\n");
				Console.WriteLine("File types are automatically detected if they match the names that the game normally loads.");
				Console.WriteLine("\te.g E0127.PRS is a main Event file\n\tE0127_0.PRS is an Event Extra file");
				Console.WriteLine("\tME0127.PRS is a Mini-Event file\n\tME0127_0.SCR is a Mini-Event Extra File");
				Console.WriteLine("\tME0127TEXLIST.PRS is an Event Texlist file\n\tTAILSPLAIN.PRS is the Tails' Cyclone file");
				Console.WriteLine("Folders created via SplitEvent or other means can be used as well,");
				Console.WriteLine("so long as they have all necessary data with a name that matches the event type.\n");
				Console.WriteLine("Endianness:\n\tbe = Big Endian\n\tle = Little Endian");
				Console.WriteLine("Dreamcast files are in Little Endian; all other versions are in Big Endian.\n");
				Console.WriteLine("-Building Mini-Event, Mini-Event Extra, Tails' Cyclone, and Texlist files-");
				Console.WriteLine("buildevent <file> [-endian Endianness] [-output Folder Path]\n");
				Console.WriteLine("-Building Event Extra files-");
				Console.WriteLine("Data Type:\n\tfull = Complete file with all data\n\tlanguage = Only audio/subtitle timings");
				Console.WriteLine("buildevent <file> [-endian Endianness] [-datatype Data Type] [-output Folder Path]\n");
				Console.WriteLine("-Building Main Event files-");
				Console.WriteLine("File Format:\n\tdc = Dreamcast (Everything is compressed into one file)");
				Console.WriteLine("\tbattle = GameCube/PS3/X360/PC (Motions are split into their own file)");
				Console.WriteLine("buildevent <file> [-format File Format] [-output Folder Path]\n");
				Console.WriteLine("Note: When specifying an output folder path that isn't complete,");
				Console.WriteLine("the new folder or partial path will be created in the same directory as this program.");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			Queue<string> argq = new Queue<string>(args);
			string fullpath_bin = Path.GetFullPath(args[0]);
			string fullpath_out = Path.GetDirectoryName(fullpath_bin);
			string name = Path.GetFileName(fullpath_bin);
			Wildcard evwcard = new Wildcard("e*", RegexOptions.IgnoreCase);
			Wildcard mevwcard = new Wildcard("me*", RegexOptions.IgnoreCase);
			Wildcard evxwcard = new Wildcard("e*_*", RegexOptions.IgnoreCase);
			Wildcard mevxwcard = new Wildcard("me*_*", RegexOptions.IgnoreCase);
			Wildcard exfwcard = new Wildcard("e*_*.*", RegexOptions.IgnoreCase);
			Wildcard mexfwcard = new Wildcard("me*_*.*", RegexOptions.IgnoreCase);
			if (!name.EndsWith("texlist.prs", StringComparison.OrdinalIgnoreCase))
			{
				if (mevwcard.IsMatch(name))
				{
					if (mevxwcard.IsMatch(name))
					{
						if (!name.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
							name += ".scr";
					}
					else
					{
						if (!name.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
							name += ".prs";
					}
				}
				else if (evwcard.IsMatch(name))
				{
					if (evxwcard.IsMatch(name))
					{
						if (!name.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						&& (!name.EndsWith(".scr", StringComparison.OrdinalIgnoreCase)))
							name += ".prs";
					}
					else
					{
						if (!name.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
							&& (!name.EndsWith(".bin", StringComparison.OrdinalIgnoreCase)))
							name += ".prs";
					}
				}
			}
			bool? be = null;
			string endian = "";
			bool? lang = null;
			string langfile;
			string format = "default";
			string formattype;
			int formatid = 0;
			string compress;
			bool? compression = null;
			if (args.Length > 1)
			{
				for (int a = 1; a < args.Length; a++)
				{
					switch (args[a])
					{
						case "-endian":
							endian = args[a + 1];
							if (endian.Equals("be", StringComparison.OrdinalIgnoreCase))
								be = true;
							else if (endian.Equals("le", StringComparison.OrdinalIgnoreCase))
								be = false;
							else
								Console.WriteLine("Invalid endian setting. Using file's default setting.");
							a++;
							break;
						case "-datatype":
							langfile = args[a + 1];
							if (langfile.Equals("full", StringComparison.OrdinalIgnoreCase))
								lang = false;
							else if (langfile.Equals("language", StringComparison.OrdinalIgnoreCase))
								lang = true;
							else
								Console.WriteLine("Invalid Event Extra data type setting. Using file's default setting.");
							a++;
							break;
						case "-format":
							formattype = args[a + 1];
							if (formattype.Equals("dc", StringComparison.OrdinalIgnoreCase))
							{
								format = formattype.ToLowerInvariant();
								formatid = 1;
							}
							else if (formattype.Equals("battle", StringComparison.OrdinalIgnoreCase))
							{
								format = formattype.ToLowerInvariant();
								formatid = 2;
							}
							else if (formattype.Equals("dcbeta", StringComparison.OrdinalIgnoreCase))
							{
								format = formattype.ToLowerInvariant();
								formatid = 3;
							}
							else if (formattype.Equals("battlebeta", StringComparison.OrdinalIgnoreCase))
							{
								format = formattype.ToLowerInvariant();
								formatid = 4;
							}
							else
								Console.WriteLine("Invalid event format setting. Using file's default setting.");
							a++;
							break;
						case "-output":
							fullpath_out = args[a + 1];
							if (fullpath_out[fullpath_out.Length - 1] != '/') fullpath_out = string.Concat(fullpath_out, '/');
								fullpath_out = Path.GetFullPath(fullpath_out);
							a++;
							break;
						case "-compression":
							compress = args[a + 1];
							if (compress.Equals("bin", StringComparison.OrdinalIgnoreCase))
								compression = false;
							else if (compress.Equals("prs", StringComparison.OrdinalIgnoreCase))
								compression = true;
							a++;
							break;
					}
				}
			}
			string evfilename;
			if (argq.Count > 0)
			{
				evfilename = fullpath_bin;
				Console.WriteLine("File: {0}", evfilename);
				Console.WriteLine("Output path: {0}", fullpath_out);
				System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
				string endiangame = "";
				if (be.HasValue)
				{
					if (be == false)
						endiangame = "Dreamcast";
					else
						endiangame = "GameCube/PS3/X360/PC";
				}
				if (name.Contains("TAILSPLAIN", StringComparison.OrdinalIgnoreCase))
				{
					if (be.HasValue)
						Console.WriteLine("Building Tails' Cyclone File for the {0} version.", be == false ? "Dreamcast" : "GameCube/PS3/X360/PC");
					else
						Console.WriteLine("Building Tails' Cyclone file");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
						sa2EventTailsPlane.Build(be, evfilename, fullpath_out);
					else
						sa2EventTailsPlane.Build(be, evfilename + ".prs", fullpath_out);
				}
				else if (name.EndsWith("TEXLIST.PRS", StringComparison.OrdinalIgnoreCase)
					|| name.EndsWith("TEXLIST", StringComparison.OrdinalIgnoreCase))
				{
					if (be.HasValue)
						Console.WriteLine($"Building Event Texlist File {name} for the {endiangame} version.");
					else
						Console.WriteLine($"Building Event Texlist file {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
						sa2Event.BuildTexlist(be, evfilename, fullpath_out);
					else
						sa2Event.BuildTexlist (be, evfilename + ".prs", fullpath_out);
				}
				else if (mexfwcard.IsMatch(name))
				{
					if (be.HasValue)
						Console.WriteLine($"Building Mini-Event Extra file {name} for the {endiangame} version.");
					else
						Console.WriteLine($"Building Mini-Event Extra file {name}");
					if (fullpath_bin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						sa2EventExtra.BuildMini(be, evfilename, fullpath_out);
					else
						sa2EventExtra.BuildMini(be, evfilename + ".scr", fullpath_out);
				}
				else if (exfwcard.IsMatch(name))
				{
					string datagame = "";
					if (lang.HasValue)
					{
						if (lang == false)
							datagame = "This is a complete file.";
						else
							datagame = "File will only contain subtitle and audio timings.";
					}
					if (be.HasValue && lang.HasValue)
						Console.WriteLine($"Building Event Extra file {name} for the {endiangame} version. {datagame}");
					else if (be.HasValue)
						Console.WriteLine($"Building Event Extra file {name} for the {endiangame} version.");
					else if (lang.HasValue)
						Console.WriteLine($"Building Event Extra file {name}. {datagame}");
					else
						Console.WriteLine($"Building Event Extra file {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						|| fullpath_bin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						sa2EventExtra.Build(be, lang, evfilename, fullpath_out);
					else
						sa2EventExtra.Build(be, lang, evfilename + ".prs", fullpath_out);
				}
				else if (name.StartsWith("me", StringComparison.OrdinalIgnoreCase))
				{
					if (be.HasValue)
						Console.WriteLine($"Building Mini-Event {name} for the {endiangame} version.");
					else
						Console.WriteLine($"Building Mini-Event {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
						SA2MiniEvent.Build(be, evfilename, fullpath_out);
					else
						SA2MiniEvent.Build(be, evfilename + ".prs", fullpath_out);
				}
				else
				{
					string ext;
					if (compression.HasValue)
					{
						if (compression == true)
							ext = ".prs";
						else
							ext = ".bin";
					}
					else
						ext = ".prs";
					string mainevname = Path.GetFileNameWithoutExtension(name) + ext;

					if (format != "default")
					{
						string gametype = "version specified in the json file.";
						switch (formatid)
						{
							case 1:
								gametype = "Dreamcast version.";
								break;
							case 2:
								gametype = "GameCube/PS3/X360/PC version.";
								break;
							case 3:
								gametype = "Dreamcast (Beta) version. This file is only compatible with SA2: The Trial and SA2: Preview.";
								break;
							case 4:
								gametype = "GameCube (Beta) version. This file will not work with any available version of SA2.";
								break;
						}
						Console.WriteLine($"Building Event {mainevname} for the {gametype}");
					}
					else
						Console.WriteLine($"Building Event {mainevname}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						|| fullpath_bin.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
						sa2Event.Build(evfilename, format, fullpath_out);
					else
						sa2Event.Build(evfilename + ext, format, fullpath_out);
				}
			}
		}
	}
}
