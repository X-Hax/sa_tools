using SplitTools.SAArc;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
			string fullpath_bin = Path.GetFullPath(args[0]);
			string name = Path.GetFileName(fullpath_bin);
			Queue<string> argq = new Queue<string>(args);
			bool? be = null;
			bool? lang = null;
			switch (argq.Count)
			{
				case 2:
					{
						if (argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
						{
							be = true;
							argq.Dequeue();
						}
						else if (argq.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
						{
							be = false;
							argq.Dequeue();
						}
					}
					break;
				case 3:
					{
						if (argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
						{
							be = true;
							argq.Dequeue();
							if (argq.Peek().Equals("/full", StringComparison.OrdinalIgnoreCase))
							{
								lang = false;
								argq.Dequeue();
							}
							else if (argq.Peek().Equals("/lang", StringComparison.OrdinalIgnoreCase))
							{
								lang = true;
								argq.Dequeue();
							}
						}
						else if (argq.Peek().Equals("/le", StringComparison.OrdinalIgnoreCase))
						{
							be = false;
							argq.Dequeue();
							if (argq.Peek().Equals("/full", StringComparison.OrdinalIgnoreCase))
							{
								lang = false;
								argq.Dequeue();
							}
							else if (argq.Peek().Equals("/lang", StringComparison.OrdinalIgnoreCase))
							{
								lang = true;
								argq.Dequeue();
							}
						}
					}
					break;
			}
			string evfilename;
			Wildcard exwcard = new Wildcard("e*_*.*", RegexOptions.IgnoreCase);
			Wildcard mexwcard = new Wildcard("me*_*.*", RegexOptions.IgnoreCase);
			if (argq.Count > 0)
			{
				evfilename = argq.Dequeue();
				Console.WriteLine("File: {0}", evfilename);
				if (mexwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Mini-Event Extra file {name}");
					sa2EventExtra.BuildMini(be, evfilename);
				}
				else if (exwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Event Extra file {name}");
					sa2EventExtra.Build(be, lang, evfilename);
				}
				else if (evfilename.StartsWith("me", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine($"Building Mini-Event {name}");
					SA2MiniEvent.Build(evfilename);
				}
				else
				{
					Console.WriteLine($"Building Event {name}");
					sa2Event.Build(evfilename);
				}

			}
			else
			{
				Console.WriteLine("File: ");
				evfilename = Console.ReadLine().Trim('"');
				if (mexwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Mini-Event Extra file {name}");
					sa2EventExtra.BuildMini(be, evfilename);
				}
				else if (exwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Event Extra file {name}");
					sa2EventExtra.Build(be, lang, evfilename);
				}
				else if (evfilename.StartsWith("me", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine($"Building Mini-Event {name}");
					SA2MiniEvent.Build(evfilename);
				}
				else
				{
					Console.WriteLine($"Building Event {name}");
					sa2Event.Build(evfilename);
				}
			}
		}
	}
}
