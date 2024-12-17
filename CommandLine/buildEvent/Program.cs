using SplitTools.SAArc;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace BuildEvent
{
	internal static class Program
	{
		private class Wildcard : Regex
		{
			/// <summary>
			/// Initializes a wildcard with the given search pattern and options.
			/// </summary>
			/// <param name="pattern">The wildcard pattern to match.</param>
			/// <param name="options">A combination of one or more
			/// <see cref="System.Text.RegularExpressions.RegexOptions"/>.</param>
			public Wildcard(string pattern, RegexOptions options) : base(WildcardToRegex(pattern), options) { }

			/// <summary>
			/// Converts a wildcard to a regex.
			/// </summary>
			/// <param name="pattern">The wildcard pattern to convert.</param>
			/// <returns>A regex equivalent of the given wildcard.</returns>
			private static string WildcardToRegex(string pattern)
			{
				return "^" + Escape(pattern)
					.Replace("\\*", ".*")
					.Replace("\\?", ".") + "$";
			}
		}

		private static void Main(string[] args)
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
			
			var argQueue = new Queue<string>(args);
			
			var fullPathBin = Path.GetFullPath(args[0]);
			var fullPathOut = Path.GetDirectoryName(fullPathBin);
			var name = Path.GetFileName(fullPathBin);
			
			var evWCard = new Wildcard("e*", RegexOptions.IgnoreCase);
			var mevWCard = new Wildcard("me*", RegexOptions.IgnoreCase);
			var evxWCard = new Wildcard("e*_*", RegexOptions.IgnoreCase);
			var mevxWCard = new Wildcard("me*_*", RegexOptions.IgnoreCase);
			var exfWCard = new Wildcard("e*_*.*", RegexOptions.IgnoreCase);
			var mexfWCard = new Wildcard("me*_*.*", RegexOptions.IgnoreCase);
			
			if (!name.EndsWith("texlist.prs", StringComparison.OrdinalIgnoreCase))
			{
				if (mevWCard.IsMatch(name))
				{
					if (mevxWCard.IsMatch(name))
					{
						if (!name.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						{
							name += ".scr";
						}
					}
					else
					{
						if (!name.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
						{
							name += ".prs";
						}
					}
				}
				else if (evWCard.IsMatch(name))
				{
					if (evxWCard.IsMatch(name))
					{
						if (!name.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						&& (!name.EndsWith(".scr", StringComparison.OrdinalIgnoreCase)))
						{
							name += ".prs";
						}
					}
					else
					{
						if (!name.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
							&& (!name.EndsWith(".bin", StringComparison.OrdinalIgnoreCase)))
						{
							name += ".prs";
						}
					}
				}
			}
			
			bool? bigEndian = null;
			bool? lang = null;
			bool? compression = null;

			var format = "default";
			var formatId = 0;
			
			if (args.Length > 1)
			{
				for (var a = 1; a < args.Length; a++)
				{
					switch (args[a])
					{
						case "-endian":
							var endian = args[a + 1];
							
							if (endian.Equals("be", StringComparison.OrdinalIgnoreCase))
							{
								bigEndian = true;
							}
							else if (endian.Equals("le", StringComparison.OrdinalIgnoreCase))
							{
								bigEndian = false;
							}
							else
							{
								Console.WriteLine("Invalid endian setting. Using file's default setting.");
							}

							a++;
							break;
						case "-datatype":
							var langFile = args[a + 1];
							
							if (langFile.Equals("full", StringComparison.OrdinalIgnoreCase))
							{
								lang = false;
							}
							else if (langFile.Equals("language", StringComparison.OrdinalIgnoreCase))
							{
								lang = true;
							}
							else
							{
								Console.WriteLine("Invalid Event Extra data type setting. Using file's default setting.");
							}

							a++;
							break;
						case "-format":
							var formatType = args[a + 1];
							
							if (formatType.Equals("dc", StringComparison.OrdinalIgnoreCase))
							{
								format = formatType.ToLowerInvariant();
								formatId = 1;
							}
							else if (formatType.Equals("battle", StringComparison.OrdinalIgnoreCase))
							{
								format = formatType.ToLowerInvariant();
								formatId = 2;
							}
							else if (formatType.Equals("dcbeta", StringComparison.OrdinalIgnoreCase))
							{
								format = formatType.ToLowerInvariant();
								formatId = 3;
							}
							else if (formatType.Equals("battlebeta", StringComparison.OrdinalIgnoreCase))
							{
								format = formatType.ToLowerInvariant();
								formatId = 4;
							}
							else
							{
								Console.WriteLine("Invalid event format setting. Using file's default setting.");
							}

							a++;
							break;
						case "-output":
							fullPathOut = args[a + 1];
							
							if (fullPathOut[^1] != '/')
							{
								fullPathOut = string.Concat(fullPathOut, '/');
							}

							fullPathOut = Path.GetFullPath(fullPathOut);
							a++;
							break;
						case "-compression":
							var compress = args[a + 1];
							if (compress.Equals("bin", StringComparison.OrdinalIgnoreCase))
							{
								compression = false;
							}
							else if (compress.Equals("prs", StringComparison.OrdinalIgnoreCase))
							{
								compression = true;
							}

							a++;
							break;
					}
				}
			}

			if (argQueue.Count <= 0)
			{
				return;
			}

			Console.WriteLine("File: {0}", fullPathBin);
			Console.WriteLine("Output path: {0}", fullPathOut);
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			
			var endianGame = "";
			
			if (bigEndian.HasValue)
			{
				if (bigEndian == false)
				{
					endianGame = "Dreamcast";
				}
				else
				{
					endianGame = "GameCube/PS3/X360/PC";
				}
			}
			if (name.Contains("TAILSPLAIN", StringComparison.OrdinalIgnoreCase))
			{
				if (bigEndian.HasValue)
				{
					Console.WriteLine("Building Tails' Cyclone File for the {0} version.", bigEndian == false ? "Dreamcast" : "GameCube/PS3/X360/PC");
				}
				else
				{
					Console.WriteLine("Building Tails' Cyclone file");
				}

				if (fullPathBin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
				{
					sa2EventTailsPlane.Build(bigEndian, fullPathBin, fullPathOut);
				}
				else
				{
					sa2EventTailsPlane.Build(bigEndian, fullPathBin + ".prs", fullPathOut);
				}
			}
			else if (name.EndsWith("TEXLIST.PRS", StringComparison.OrdinalIgnoreCase)
			         || name.EndsWith("TEXLIST", StringComparison.OrdinalIgnoreCase))
			{
				if (bigEndian.HasValue)
				{
					Console.WriteLine($"Building Event Texlist File {name} for the {endianGame} version.");
				}
				else
				{
					Console.WriteLine($"Building Event Texlist file {name}");
				}

				if (fullPathBin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
				{
					sa2Event.BuildTexlist(bigEndian, fullPathBin, fullPathOut);
				}
				else
				{
					sa2Event.BuildTexlist (bigEndian, fullPathBin + ".prs", fullPathOut);
				}
			}
			else if (mexfWCard.IsMatch(name))
			{
				if (bigEndian.HasValue)
				{
					Console.WriteLine($"Building Mini-Event Extra file {name} for the {endianGame} version.");
				}
				else
				{
					Console.WriteLine($"Building Mini-Event Extra file {name}");
				}

				if (fullPathBin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
				{
					sa2EventExtra.BuildMini(bigEndian, fullPathBin, fullPathOut);
				}
				else
				{
					sa2EventExtra.BuildMini(bigEndian, fullPathBin + ".scr", fullPathOut);
				}
			}
			else if (exfWCard.IsMatch(name))
			{
				var dataGame = "";
				
				if (lang.HasValue)
				{
					if (lang == false)
					{
						dataGame = "This is a complete file.";
					}
					else
					{
						dataGame = "File will only contain subtitle and audio timings.";
					}
				}
				
				if (bigEndian.HasValue && lang.HasValue)
				{
					Console.WriteLine($"Building Event Extra file {name} for the {endianGame} version. {dataGame}");
				}
				else if (bigEndian.HasValue)
				{
					Console.WriteLine($"Building Event Extra file {name} for the {endianGame} version.");
				}
				else if (lang.HasValue)
				{
					Console.WriteLine($"Building Event Extra file {name}. {dataGame}");
				}
				else
				{
					Console.WriteLine($"Building Event Extra file {name}");
				}

				if (fullPathBin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
				    || fullPathBin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
				{
					sa2EventExtra.Build(bigEndian, lang, fullPathBin, fullPathOut);
				}
				else
				{
					sa2EventExtra.Build(bigEndian, lang, fullPathBin + ".prs", fullPathOut);
				}
			}
			else if (name.StartsWith("me", StringComparison.OrdinalIgnoreCase))
			{
				if (bigEndian.HasValue)
				{
					Console.WriteLine($"Building Mini-Event {name} for the {endianGame} version.");
				}
				else
				{
					Console.WriteLine($"Building Mini-Event {name}");
				}

				if (fullPathBin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
				{
					SA2MiniEvent.Build(bigEndian, fullPathBin, fullPathOut);
				}
				else
				{
					SA2MiniEvent.Build(bigEndian, fullPathBin + ".prs", fullPathOut);
				}
			}
			else
			{
				string ext;
				if (compression.HasValue)
				{
					if (compression == true)
					{
						ext = ".prs";
					}
					else
					{
						ext = ".bin";
					}
				}
				else
				{
					ext = ".prs";
				}

				var mainEventName = Path.GetFileNameWithoutExtension(name) + ext;

				if (format != "default")
				{
					var gameType = formatId switch
					{
						1 => "Dreamcast version.",
						2 => "GameCube/PS3/X360/PC version.",
						3 => "Dreamcast (Beta) version. This file is only compatible with SA2: The Trial and SA2: Preview.",
						4 => "GameCube (Beta) version. This file will not work with any available version of SA2.",
						_ => "version specified in the json file."
					};
					
					Console.WriteLine($"Building Event {mainEventName} for the {gameType}");
				}
				else
				{
					Console.WriteLine($"Building Event {mainEventName}");
				}

				if (fullPathBin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
				    || fullPathBin.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
				{
					sa2Event.Build(fullPathBin, format, fullPathOut);
				}
				else
				{
					sa2Event.Build(fullPathBin + ext, format, fullPathOut);
				}
			}
		}
	}
}
