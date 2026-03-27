using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IniDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;
using IniGroup = System.Collections.Generic.Dictionary<string, string>;

namespace SplitTools
{
	public static class IniFile
	{
		public static IniDictionary Load(params string[] data)
		{
			var result = new IniDictionary();
			var curent = new IniGroup();
			result.Add(string.Empty, curent);
			var curgroup = string.Empty;
			for (var i = 0; i < data.Length; i++)
			{
				var line = data[i];
				var sb = new StringBuilder(line.Length);
				var startswithbracket = false;
				var firstequals = -1;
				var endbracket = -1;
				for (var c = 0; c < line.Length; c++)
					switch (line[c])
					{
						case '\\': // escape character
							if (c + 1 == line.Length) goto default;
							c++;
							switch (line[c])
							{
								case 'n': // line feed
									sb.Append('\n');
									break;
								case 'r': // carriage return
									sb.Append('\r');
									break;
								default: // literal character
									sb.Append(line[c]);
									break;
							}
							break;
						case '=':
							if (firstequals == -1)
								firstequals = sb.Length;
							goto default;
						case '[':
							if (c == 0)
								startswithbracket = true;
							goto default;
						case ']':
							endbracket = sb.Length;
							goto default;
						case ';': // comment character, stop processing this line
							c = line.Length;
							break;
						default:
							sb.Append(line[c]);
							break;
					}
				line = sb.ToString();
				if (startswithbracket & endbracket != -1)
				{
					curgroup = line.Substring(1, endbracket - 1);
					curent = new IniGroup();
					try
					{
						result.Add(curgroup, curent);
					}
					catch (ArgumentException ex)
					{
						throw new Exception("INI File error: Group \"" + curgroup + "\" already exists.\nline " + (i + 1), ex);
					}
				}
				else if (!IsNullOrWhiteSpace(line))
				{
					string key;
					var value = string.Empty;
					if (firstequals > -1)
					{
						key = line.Substring(0, firstequals);
						value = line.Substring(firstequals + 1);
					}
					else
						key = line;
					try
					{
						curent.Add(key, value);
					}
					catch (ArgumentException ex)
					{
						throw new Exception("INI File error: Value \"" + key + "\" already exists in group \"" + curgroup + "\".\nline " + (i + 1), ex);
					}
				}
			}
			return result;
		}

		public static IniDictionary Load(string filename) { return Load(File.ReadAllLines(filename)); }

		public static IniDictionary Load(IEnumerable<string> data) { return Load(data.ToArray()); }

		public static IniDictionary Load(Stream stream)
		{
			var data = new List<string>();
			var reader = new StreamReader(stream);
			string line;
			while ((line = reader.ReadLine()) != null)
				data.Add(line);
			return Load(data.ToArray());
		}

		public static string[] Save(IniDictionary INI)
		{
			var first = true;
			var result = new List<string>();
			foreach (var group in INI)
			{
				var add = "";
				if (!first) 
					add += Environment.NewLine;
				else 
					first = false;
				if (!string.IsNullOrEmpty(group.Key))
				{
					add += "[" + group.Key.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r").Replace(";", @"\;") + "]";
					result.Add(add);
				}
				foreach (var value in group.Value)
				{
					var escapedkey = value.Key.Replace(@"\", @"\\").Replace("=", @"\=").Replace("\n", @"\n").Replace("\r", @"\r").Replace(";", @"\;");
					if (escapedkey.StartsWith("["))
						escapedkey = escapedkey.Insert(0, @"\");
					result.Add(escapedkey + "=" + value.Value.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r").Replace(";", @"\;"));
				}
			}
			return result.ToArray();
		}

		public static void Save(IniDictionary INI, string filename) { File.WriteAllLines(filename, Save(INI)); }

		public static void Save(IniDictionary INI, Stream stream)
		{
			var writer = new StreamWriter(stream);
			foreach (var line in Save(INI))
				writer.WriteLine(line);
		}

		public static IniDictionary Combine(IniDictionary dictA, IniDictionary dictB)
		{
			var result = new IniDictionary();
			foreach (var group in dictA)
				result.Add(group.Key, new IniGroup(group.Value));
			foreach (var group in dictB)
				if (result.ContainsKey(group.Key))
					foreach (var item in group.Value)
						result[group.Key][item.Key] = item.Value;
				else
					result.Add(group.Key, new IniGroup(group.Value));
			return result;
		}

		internal static bool IsNullOrWhiteSpace(string value)
		{
			if (string.IsNullOrEmpty(value))
				return true;
			for (var i = 0; i < value.Length; i++)
				if (!char.IsWhiteSpace(value[i]))
					return false;
			return true;
		}
	}
}