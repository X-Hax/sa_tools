using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VariableListGenerator
{
	static class Program
	{
		static readonly Regex arrayLengthExp = new Regex(@"\[(\d+)?\]", RegexOptions.CultureInvariant);

		private static readonly HashSet<long> excludedAddresses = new HashSet<long>();

		static void Main(string[] args)
		{
			bool outputD = args.Any(x => x == "-d");

			string filename;

			if (args.Length > 0)
			{
				filename = args[0];
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}

			if (string.IsNullOrEmpty(filename))
			{
				Console.WriteLine("Filename is empty.");
				return;
			}

			if (args.Length > 1)
			{
				foreach (string address in File.ReadAllLines(args[1]).Where(x => !string.IsNullOrEmpty(x)))
				{
					excludedAddresses.Add(Convert.ToInt64(address, 16));
				}
			}

			string[] lines = File.ReadAllLines(filename);

			using (StreamWriter writer = File.CreateText(Path.ChangeExtension(filename, outputD ? "d" : "h")))
			{
				int lineNumber = 0;
				foreach (string line in lines.Select(x => x.Trim()))
				{
					++lineNumber;

					string[] fields = line.Split('|');

					if (fields.Length != 3)
					{
						Console.WriteLine($"Incorrect number of parameters on line {lineNumber}: Expected 3, got {fields.Length}");
						continue;
					}

					string address = fields[0];
					string name    = fields[1];
					string type    = fields[2];

					long addressValue = Convert.ToInt64(address, 16);

					if (excludedAddresses.Contains(addressValue))
					{
						continue;
					}

					MatchCollection matches = arrayLengthExp.Matches(type);
					bool isArray = matches.Count == 1;

					if (matches.Count > 1)
					{
						Console.WriteLine($"Unsupported multidimensional array on line {lineNumber}: {type}");
						continue;
					}

					if (isArray)
					{
						GroupCollection groups = matches[0].Groups;
						if (groups.Count == 2 && !string.IsNullOrEmpty(groups[1].Value))
						{
							int length = int.Parse(groups[1].Value);
							string baseType = arrayLengthExp.Split(type)[0];

							if (outputD)
							{
								baseType = toDType(baseType);
								writer.WriteLine($@"mixin DataArray!({baseType}, ""{name}"", {address}, {length});");
							}
							else
							{
								writer.WriteLine($"DataArray({baseType}, {name}, {address}, {length});");
							}

							continue;
						}

						// If the "array" does not have a specified size,
						// just make it a pointer.
						type = arrayLengthExp.Split(type)[0] + "*";
					}

					if (outputD)
					{
						type = toDType(type);
						writer.WriteLine($@"mixin DataPointer!({type}, ""{name}"", {address});");
					}
					else
					{
						writer.WriteLine($"DataPointer({type}, {name}, {address});");
					}
				}
			}
		}

		static string toDType(string value)
		{
			string result = value;

			if (value.Contains("unsigned "))
			{
				result = result.Replace("unsigned ", "u");
			}

			if (result.Contains("__int8"))
			{
				result = result.Replace("__int8", "byte");
			}

			if (result.Contains("__int16"))
			{
				result = result.Replace("__int16", "short");
			}

			if (result.Contains("__int64"))
			{
				result = result.Replace("__int64", "long");
			}

			if (result.Contains("signed "))
			{
				result = result.Replace("signed ", null);
			}

			return result;
		}
	}
}