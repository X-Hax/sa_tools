using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SAModel.SAMDL
{
	public static class IniFile
	{
		public static Dictionary<string, Dictionary<string, string>> Load(string filename)
		{
			Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
			Dictionary<string, string> curent = new Dictionary<string, string>();
			result.Add(string.Empty, curent);
			string curgroup = string.Empty;
			string[] fc = File.ReadAllLines(filename);
			for (int i = 0; i <= fc.Length - 1; i++)
			{
				string line = fc[i].Split(';')[0].Trim();
				if (line.StartsWith("[") & line.EndsWith("]"))
				{
					curgroup = line.Substring(1, line.Length - 2);
					curent = new Dictionary<string, string>();
					try
					{
						result.Add(curgroup, curent);
					}
					catch (ArgumentException ex)
					{
						throw new Exception("INI File error: Group \"" + line.Substring(1, line.Length - 2) + "\" already exists.\n" + filename + ":line " + i, ex);
					}
				}
				else if (!string.IsNullOrEmpty(line))
				{
					try
					{
						if (line.IndexOf('=') > -1)
							curent.Add(line.Substring(0, line.IndexOf('=')), line.Substring(line.IndexOf('=') + 1));
						else
							curent.Add(line, "");
					}
					catch (ArgumentException ex)
					{
						throw new Exception("INI File error: Value \"" + line.Split('=')[0] + "\" already exists in group \"" + curgroup + "\".\n" + filename + ":line " + i, ex);
					}
				}
			}
			return result;
		}

		public static void Save(Dictionary<string, Dictionary<string, string>> INI, string filename)
		{
			List<string> result = new List<string>();
			foreach (KeyValuePair<string, Dictionary<string, string>> group in INI)
			{
				if (!string.IsNullOrEmpty(group.Key))
					result.Add("[" + group.Key + "]");
				foreach (KeyValuePair<string, string> value in group.Value)
					result.Add(value.Key + "=" + value.Value);
			}
			File.WriteAllLines(filename, result.ToArray());
		}
	}
}