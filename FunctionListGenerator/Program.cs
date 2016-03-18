using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FunctionListGenerator
{
	static class Program
	{
		static readonly Regex functiontype = new Regex(@"^(?<returntype>(?:const )?(?:signed |unsigned )?[A-Za-z_][A-Za-z_0-9]*(?: ?\*)?) ?(?<callconv>__cdecl|__stdcall|__fastcall|__thiscall|__usercall)?(?:@<(?<returnreg>[^>]+)>)?\((?<arguments>.*)\)$", RegexOptions.CultureInvariant);
		static readonly Regex argument = new Regex(@"(?<name>[^<@]+)(?:@<(?<register>[^>]+)>)?", RegexOptions.CultureInvariant);
		static readonly Regex functionptr = new Regex(@"\((?:__cdecl|__stdcall|__fastcall|__thiscall)? \*(?<name>[A-Za-z_][A-Za-z_0-9]*)\)", RegexOptions.CultureInvariant);

		static readonly Dictionary<string, string> boolregs = new Dictionary<string, string>()
		{
			{ "eax", "al" },
			{ "ebx", "bl" },
			{ "ecx", "cl" },
			{ "edx", "dl" }
		};

		static void Main(string[] args)
		{
			string filename;
			if (args.Length > 0)
				filename = args[0];
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}
			string[] lines = File.ReadAllLines(filename);
			List<string> excludefuncs = new List<string>();
			if (args.Length > 1)
				excludefuncs = new List<string>(File.ReadAllLines(args[1]));
			using (StreamWriter writer = File.CreateText(Path.ChangeExtension(filename, "h")))
			{
				writer.WriteLine("#define FunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__cdecl *const NAME)ARGS = (RETURN_TYPE (__cdecl *)ARGS)ADDRESS");
				writer.WriteLine("#define StdcallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__stdcall *const NAME)ARGS = (RETURN_TYPE (__stdcall *)ARGS)ADDRESS");
				writer.WriteLine("#define FastcallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__fastcall *const NAME)ARGS = (RETURN_TYPE (__fastcall *)ARGS)ADDRESS");
				writer.WriteLine("#define ThiscallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__thiscall *const NAME)ARGS = (RETURN_TYPE (__thiscall *)ARGS)ADDRESS");
				writer.WriteLine("#define VoidFunc(NAME, ADDRESS) FunctionPointer(void,NAME,(void),ADDRESS)");
				writer.WriteLine("#define ObjectFunc(NAME, ADDRESS) FunctionPointer(void,NAME,(ObjectMaster *obj),ADDRESS)");
				writer.WriteLine();
				foreach (string line in lines.Where((line) => !line.Contains("__usercall")))
				{
					string[] split = line.Split('|');
					if (split.Length != 3) continue;
					string address = split[0];
					if (excludefuncs.Contains(address)) continue;
					string name = split[1];
					string type = split[2];
					Match match = functiontype.Match(type);
					string returntype = match.Groups["returntype"].Value;
					string callconv = match.Groups["callconv"].Value;
					string arguments = match.Groups["arguments"].Value;
					switch (match.Groups[2].Value)
					{
						case "__cdecl":
						case "":
							if (returntype == "void" && (arguments == string.Empty || arguments == "void"))
								writer.WriteLine("VoidFunc({0}, {1});", name, address);
							else if (returntype == "void" && arguments == "ObjectMaster *this")
								writer.WriteLine("ObjectFunc({0}, {1});", name, address);
							else
								writer.WriteLine("FunctionPointer({0}, {1}, ({2}), {3});", returntype, name, arguments.Replace("this", "_this"), address);
							break;
						case "__stdcall":
							writer.WriteLine("StdcallFunctionPointer({0}, {1}, ({2}), {3});", returntype, name, arguments.Replace("this", "_this"), address);
							break;
						case "__fastcall":
							writer.WriteLine("FastcallFunctionPointer({0}, {1}, ({2}), {3});", returntype, name, arguments.Replace("this", "_this"), address);
							break;
						case "__thiscall":
							writer.WriteLine("ThiscallFunctionPointer({0}, {1}, ({2}), {3});", returntype, name, arguments.Replace("this", "_this"), address);
							break;
					}
				}
				foreach (string line in lines.Where((line) => line.Contains("__usercall")))
				{
					writer.WriteLine();
					string[] split = line.Split('|');
					string address = split[0];
					if (excludefuncs.Contains(address)) continue;
					string name = split[1];
					string type = split[2];
					writer.WriteLine("// {0}", type);
					Match match = functiontype.Match(type);
					string returntype = match.Groups["returntype"].Value;
					string returnreg = match.Groups["returnreg"].Value;
					string callconv = match.Groups["callconv"].Value;
					string arguments = match.Groups["arguments"].Value;
					List<string> arglist = new List<string>();
					int c = 0;
					while (c < arguments.Length)
					{
						if (arguments[c] == ' ')
							c++;
						int comma = arguments.IndexOf(',', c);
						if (comma == -1)
						{
							arglist.Add(arguments.Substring(c));
							break;
						}
						int paren = arguments.IndexOf('(', c);
						if (paren > -1 && paren < comma)
							comma = arguments.IndexOf(')', arguments.IndexOf('(', paren + 1)) + 1;
						arglist.Add(arguments.Substring(c, comma - c));
						c = comma + 1;
					}
					List<string> argnames = new List<string>();
					List<string> argdecls = new List<string>();
					List<string> argregs = new List<string>();
					foreach (string arg in arglist)
					{
						match = argument.Match(arg);
						string argdecl = match.Groups[1].Value.Trim();
						string argname;
						if (functionptr.IsMatch(argdecl))
							argname = functionptr.Match(argdecl).Groups["name"].Value;
						else
							argname = argdecl.Split(' ').Last().TrimStart('*');
						switch (argname)
						{
							case "size":
								argname = "_size";
								argdecl = argdecl.Replace("size", "_size");
								break;
							case "type":
								argname = "_type";
								argdecl = argdecl.Replace("type", "_type");
								break;
							case "this":
								argname = "_this";
								argdecl = argdecl.Replace("this", "_this");
								break;
							case "sp":
								argname = "_sp";
								argdecl = argdecl.Replace("sp", "_sp");
								break;
						}
						argdecls.Add(argdecl);
						argnames.Add(argname);
						argregs.Add(match.Groups[2].Value);
					}
					writer.WriteLine("static const void *const {0}Ptr = (void*){1};", name, address);
					writer.WriteLine("static inline {0} {1}({2})", returntype, name, string.Join(", ", argdecls.ToArray()));
					writer.WriteLine("{");
					if (returntype != "void")
						writer.WriteLine("\t{0} result;", returntype);
					writer.WriteLine("\t__asm");
					writer.WriteLine("\t{");
					for (int i = argnames.Count - 1; i >= 0; i--)
					{
						if (string.IsNullOrEmpty(argregs[i]))
						{
							bool isbyte = false;
							if (!argdecls[i].Contains("*"))
								switch (argdecls[i].Substring(argdecls[i].LastIndexOf(' ')))
								{
									case "char":
									case "unsigned char":
									case "signed char":
									case "bool":
									case "Sint8":
									case "Uint8":
									case "byte":
									case "BOOL1":
									case "uint8_t":
									case "int8_t":
										isbyte = true;
										break;
								}
							if (isbyte)
							{
								writer.WriteLine("\t\tmovzx eax, [{0}]", argnames[i]);
								writer.WriteLine("\t\tpush eax");
							}
							else
								writer.WriteLine("\t\tpush [{0}]", argnames[i]);
						}
						else
							writer.WriteLine("\t\tmov {0}, [{1}]", argregs[i], argnames[i]);
					}
					writer.WriteLine("\t\tcall {0}Ptr", name);
					int stackcnt = argregs.Count((item) => string.IsNullOrEmpty(item));
					if (stackcnt > 0)
						writer.WriteLine("\t\tadd esp, {0}", stackcnt * 4);
					if (returntype == "bool")
						writer.WriteLine("\t\tmov result, {0}", boolregs[returnreg]);
					else if (returnreg == "st0")
						writer.WriteLine("\t\tfstp result");
					else if (returntype != "void")
						writer.WriteLine("\t\tmov result, {0}", returnreg);
					writer.WriteLine("\t}");
					if (returntype != "void")
						writer.WriteLine("\treturn result;");
					writer.WriteLine("}");
				}
			}
		}
	}
}