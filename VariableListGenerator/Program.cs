using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VariableListGenerator
{
	class Program
	{
		static readonly Regex arrayLengthExp = new Regex(@"\[(\d+)?\]", RegexOptions.CultureInvariant);

		// TODO: exclusions
		static void Main(string[] args)
		{
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

			string[] lines = File.ReadAllLines(filename);

			using (StreamWriter writer = File.CreateText(Path.ChangeExtension(filename, "h")))
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

					var  matches = arrayLengthExp.Matches(type);
					bool isArray = matches.Count == 1;

					if (matches.Count > 1)
					{
						Console.WriteLine($"Unsupported multidimensional array on line {lineNumber}: {type}");
						continue;
					}

					if (isArray)
					{
						var groups = matches[0].Groups;
						if (groups.Count == 2 && !string.IsNullOrEmpty(groups[1].Value))
						{
							int length = int.Parse(groups[1].Value);
							var baseType = arrayLengthExp.Split(type)[0];

							writer.WriteLine($"DataArray({baseType}, {name}, {address}, {length});");
							continue;
						}

						// If the "array" does not have a specified size,
						// just make it a pointer.
						type = arrayLengthExp.Split(type)[0] + "*";
					}

					writer.WriteLine($"DataPointer({type}, {name}, {address});");
				}
			}
		}
	}
}