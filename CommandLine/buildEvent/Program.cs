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
			if (argq.Count > 0)
			{
				evfilename = argq.Dequeue();
				Console.WriteLine("File: {0}", evfilename);
				System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
				if (mexfwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Mini-Event Extra file {name}");
					if (fullpath_bin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						sa2EventExtra.BuildMini(be, evfilename);
					else
						sa2EventExtra.BuildMini(be, evfilename + ".scr");
				}
				else if (exfwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Event Extra file {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						|| fullpath_bin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						sa2EventExtra.Build(be, lang, evfilename);
					else
						sa2EventExtra.Build(be, lang, evfilename + ".prs");
				}
				else if (name.StartsWith("me", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine($"Building Mini-Event {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
						SA2MiniEvent.Build(be, evfilename);
					else
						SA2MiniEvent.Build(be, evfilename + ".prs");
				}
				else
				{
					Console.WriteLine($"Building Event {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						|| fullpath_bin.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
						sa2Event.Build(evfilename);
					else
						sa2Event.Build(evfilename + ".prs");
				}

			}
			else
			{
				Console.WriteLine("File: ");
				evfilename = Console.ReadLine().Trim('"');
				if (mexfwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Mini-Event Extra file {name}");
					if (fullpath_bin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						sa2EventExtra.BuildMini(be, evfilename);
					else
						sa2EventExtra.BuildMini(be, evfilename + ".scr");
				}
				else if (exfwcard.IsMatch(name))
				{
					Console.WriteLine($"Building Event Extra file {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						|| fullpath_bin.EndsWith(".scr", StringComparison.OrdinalIgnoreCase))
						sa2EventExtra.Build(be, lang, evfilename);
					else
						sa2EventExtra.Build(be, lang, evfilename + ".prs");
				}
				else if (name.StartsWith("me", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine($"Building Mini-Event {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase))
						SA2MiniEvent.Build(be, evfilename);
					else
						SA2MiniEvent.Build(be, evfilename + ".prs");
				}
				else
				{
					Console.WriteLine($"Building Event {name}");
					if (fullpath_bin.EndsWith(".prs", StringComparison.OrdinalIgnoreCase)
						|| fullpath_bin.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
						sa2Event.Build(evfilename);
					else
						sa2Event.Build(evfilename + ".prs");
				}
			}
		}
	}
}
