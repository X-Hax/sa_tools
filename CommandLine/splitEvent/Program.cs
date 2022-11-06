using SplitTools.SAArc;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace splitEvent
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
			Wildcard exwcard = new Wildcard("e*_*.*", RegexOptions.IgnoreCase);
			Wildcard mexwcard = new Wildcard("me*_*.*", RegexOptions.IgnoreCase);
			if (name.EndsWith("texlist.prs", StringComparison.OrdinalIgnoreCase))
				sa2Event.SplitExternalTexlist(fullpath_bin, fullpath_out);
			else if (mexwcard.IsMatch(name))
				sa2EventExtra.SplitMini(fullpath_bin, fullpath_out);
			else if (exwcard.IsMatch(name))
				sa2EventExtra.Split(fullpath_bin, fullpath_out);
			else if (name.StartsWith("me", StringComparison.OrdinalIgnoreCase))
				SA2MiniEvent.Split(fullpath_bin, fullpath_out);
			else
				sa2Event.Split(fullpath_bin, fullpath_out);
		}
	}
}