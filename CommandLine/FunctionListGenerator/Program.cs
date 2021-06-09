using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionListGenerator
{
	static class Program
	{
		static readonly Regex functiontype = new Regex(@"^(?<returntype>(?:const )?(?:signed |unsigned )?[A-Za-z_][A-Za-z_0-9]*(?: ?\*)?) ?(?<callconv>__cdecl|__stdcall|__fastcall|__thiscall|__usercall|__userpurge)?(?:@<(?<returnreg>[^>]+)>)?\((?<arguments>.*)\)$", RegexOptions.CultureInvariant);
		static readonly Regex argument     = new Regex(@"(?<name>[^<@]+)(?:@<(?<register>[^>]+)>)?",                                                                                                                                                                      RegexOptions.CultureInvariant);
		static readonly Regex functionptr  = new Regex(@"\((?<callconv>__cdecl|__stdcall|__fastcall|__thiscall)?\*(?<name>[A-Za-z_][A-Za-z_0-9]*)\)",                                                                                                                     RegexOptions.CultureInvariant);
		static readonly Regex objfunc      = new Regex(@"^ObjectMaster\s*\*\s*[^\s,]*$");

		static readonly Dictionary<string, string> boolregs = new Dictionary<string, string>
		{
			{ "eax", "al" },
			{ "ebx", "bl" },
			{ "ecx", "cl" },
			{ "edx", "dl" }
		};

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

			string[] lines = File.ReadAllLines(filename);
			var excludefuncs = new List<string>();

			if (args.Length > 1)
			{
				if (File.Exists(args[1]))
				{
					excludefuncs = new List<string>(File.ReadAllLines(args[1]));
				}
			}

			using (StreamWriter writer = File.CreateText(Path.ChangeExtension(filename, outputD ? "d" : "h")))
			{
				if (outputD)
				{
					outputDFunctions(lines, excludefuncs, writer);
				}
				else
				{
					writer.WriteLine("#define FunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__cdecl *const NAME)ARGS = (RETURN_TYPE (__cdecl *)ARGS)ADDRESS");
					writer.WriteLine("#define StdcallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__stdcall *const NAME)ARGS = (RETURN_TYPE (__stdcall *)ARGS)ADDRESS");
					writer.WriteLine("#define FastcallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__fastcall *const NAME)ARGS = (RETURN_TYPE (__fastcall *)ARGS)ADDRESS");
					writer.WriteLine("#define ThiscallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) static RETURN_TYPE (__thiscall *const NAME)ARGS = (RETURN_TYPE (__thiscall *)ARGS)ADDRESS");
					writer.WriteLine("#define VoidFunc(NAME, ADDRESS) FunctionPointer(void,NAME,(void),ADDRESS)");
					writer.WriteLine("#define ObjectFunc(NAME, ADDRESS) FunctionPointer(void,NAME,(ObjectMaster *obj),ADDRESS)");
					writer.WriteLine();

					outputCFunctions(lines, excludefuncs, writer);
				}
			}
		}

		static string toDType(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			string result = value;

			if (value.Contains("unsigned "))
			{
				result = result.Replace("unsigned ", "u");
			}

			if (result.Contains("signed "))
			{
				result = result.Replace("signed ", null);
			}

			if (result.Contains("__int8"))
			{
				result = result.Replace("__int8", "byte");
			}

			if (result.Contains("__int16"))
			{
				result = result.Replace("__int16", "short");
			}

			if (result.Contains("__int32"))
			{
				result = result.Replace("__int32", "int");
			}

			if (result.Contains("__int64"))
			{
				result = result.Replace("__int64", "long");
			}

			return result;
		}

		static void outputDFunctions(string[] lines, List<string> excludefuncs, StreamWriter writer)
		{
			foreach (string line in lines)
			{
				string[] split   = line.Split('|');
				string   address = split[0];

				if (excludefuncs.Contains(address))
				{
					continue;
				}

				string name = split[1];
				string type = split[2];

				Match match = functiontype.Match(type);

				string returntype = toDType(match.Groups["returntype"].Value);
				string returnreg  = match.Groups["returnreg"].Value;
				string callconv   = match.Groups["callconv"].Value;
				string arguments  = match.Groups["arguments"].Value;

				var arglist = new List<string>();
				getArgList(arguments, arglist);

				var argnames = new List<string>();
				var argdecls = new List<string>();
				var argregs  = new List<string>();

				giveMeTheThings(arglist, argdecls, argnames, argregs);

				var dArgs = new List<string>();

				bool gtfo = false;

				for (int i = 0; i < argnames.Count; i++)
				{
					string argName     = argnames[i];
					string argDecl     = argdecls[i];
					string argRegister = argregs[i];

					if (argName == argDecl)
					{
						argName = null;
					}
					else if (argName == "function")
					{
						argName = "function_";
					}

					if (i == 0 && callconv == "__thiscall" && string.IsNullOrEmpty(argName))
					{
						argName =  "_this";
						argDecl += " _this";
					}

					if (argDecl == "...")
					{
						dArgs.Add("makeArg(\"...\")");
						continue;
					}

					if (functionptr.IsMatch(argDecl))
					{
						gtfo = true;
						break;

						// TODO !!!
#if false
						string[] s = functionptr.Split(argDecl);

						string ptrReturnType = string.Empty;
						string ptrConention = string.Empty;
						string theRest = string.Empty;

						switch (s.Length)
						{
							case 3:
								ptrReturnType = s[0].Trim();
								ptrConention = "__cdecl";
								theRest = s[2].Trim();
								break;

							case 4:
								ptrReturnType = s[0].Trim();
								ptrConention = s[1].Trim();
								theRest = s[3];
								break;

							default:
								throw new Exception("idfk");
						}

						if (ptrConention == "__cdecl" || ptrConention == "__stdcall")
						{
							string extern_ = ptrConention == "__cdecl" ? "C" : "Windows";
							argDecl = $"extern ({extern_}) {ptrReturnType} function{theRest}"; // theRest includes '(' and ')'
						}
						else
						{
							argDecl = $"/* unsupported function pointer calling convention: {callconv} */ void*";
						}
#endif
					}
					else if (!string.IsNullOrEmpty(argName))
					{
						argDecl = argDecl.Replace(" *", "* ").Trim();
						string[] decl = argDecl.Split(' ');
						argDecl = string.Join(" ", decl, 0, decl.Length - 1).Trim();
					}

					argDecl = toDType(argDecl);

					if (string.IsNullOrEmpty(argRegister))
					{
						dArgs.Add($"makeArg!({argDecl})(\"{argName}\")");
					}
					else
					{
						dArgs.Add($"makeArg!({argDecl})(\"{argName}\", \"{argRegister.ToUpper()}\")");
					}
				}

				if (gtfo)
				{
					continue;
				}

				string dArgString = string.Join(", ", dArgs.ToArray());

				switch (callconv)
				{
					case "__cdecl":
					case "":
						if (returntype == "void")
						{
							if (string.IsNullOrEmpty(arguments) || arguments == "void")
							{
								writer.WriteLine("mixin VoidFunc!(\"{0}\", {1});", name, address);
								break;
							}

							// TODO !!!
							/*if (objfunc.IsMatch(arguments))
							{
								writer.WriteLine("ObjectFunc({0}, {1});", name, address);
								break;
							}*/
						}

						writer.WriteLine("mixin FunctionPointer!({0}, \"{1}\", [ {2} ], {3});",
						                 returntype, name, dArgString, address);
						break;

					case "__stdcall":
						// TODO !!!
						writer.WriteLine("mixin StdcallFunctionPointer!({0}, \"{1}\", [ {2} ], {3});",
						                 returntype, name, dArgString, address);
						break;

					case "__fastcall":
						writer.WriteLine("mixin FastcallFunctionPointer!({0}, \"{1}\", [ {2} ], {3});",
						                 returntype, name, dArgString, address);
						break;

					case "__thiscall":
						writer.WriteLine("mixin ThiscallFunctionPointer!({0}, \"{1}\", [ {2} ], {3});",
						                 returntype, name, dArgString, address);
						break;

					case "__usercall":
						// TODO !!!
						break;

					case "__userpurge":
						// TODO !!!
						break;
				}
			}
		}

		static void outputCFunctions(string[] lines, List<string> excludefuncs, StreamWriter writer)
		{
			List<string> nonStandard = lines.Where(x => x.Contains("__usercall") || x.Contains("__userpurge")).ToList();

			foreach (string line in lines.Except(nonStandard))
			{
				string[] split = line.Split('|');

				if (split.Length != 3)
				{
					continue;
				}

				string address = split[0];

				if (excludefuncs.Contains(address))
				{
					continue;
				}

				string name = split[1];
				string type = split[2];

				Match match = functiontype.Match(type);

				string returntype = match.Groups["returntype"].Value;
				string callconv   = match.Groups["callconv"].Value;
				string arguments  = match.Groups["arguments"].Value;

				switch (callconv)
				{
					case "__cdecl":
					case "":
						if (returntype == "void")
						{
							if (string.IsNullOrEmpty(arguments) || arguments == "void")
							{
								writer.WriteLine("VoidFunc({0}, {1});", name, address);
								break;
							}

							if (objfunc.IsMatch(arguments))
							{
								writer.WriteLine("ObjectFunc({0}, {1});", name, address);
								break;
							}
						}

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

			foreach (string line in nonStandard)
			{
				string[] split   = line.Split('|');
				string   address = split[0];

				if (excludefuncs.Contains(address))
				{
					continue;
				}

				writer.WriteLine();

				string name = split[1];
				string type = split[2];

				writer.WriteLine("// {0}", type);

				Match match = functiontype.Match(type);

				string returntype = match.Groups["returntype"].Value;
				string returnreg  = match.Groups["returnreg"].Value;
				string callconv   = match.Groups["callconv"].Value; // TODO: handle userpurge (clean up stack)
				string arguments  = match.Groups["arguments"].Value;

				var arglist = new List<string>();
				getArgList(arguments, arglist);

				var argnames = new List<string>();
				var argdecls = new List<string>();
				var argregs  = new List<string>();

				giveMeTheThings(arglist, argdecls, argnames, argregs);

				writer.WriteLine("static const void *const {0}Ptr = (void*){1};", name, address);
				writer.WriteLine("static inline {0} {1}({2})", returntype, name, string.Join(", ", argdecls.ToArray()));
				writer.WriteLine("{");

				if (returntype != "void")
				{
					writer.WriteLine("\t{0} result;", returntype);
				}

				writer.WriteLine("\t__asm");
				writer.WriteLine("\t{");

				for (int i = argnames.Count - 1; i >= 0; i--)
				{
					if (string.IsNullOrEmpty(argregs[i]))
					{
						bool isbyte = false;
						if (!argdecls[i].Contains("*"))
						{
							switch (argdecls[i].Remove(argdecls[i].LastIndexOf(' ')))
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
								case "__int8":
								case "signed __int8":
								case "unsigned __int8":
								case "_BYTE":
								case "short":
								case "unsigned short":
								case "signed short":
								case "Sint16":
								case "Uint16":
								case "uint16_t":
								case "int16_t":
								case "__int16":
								case "signed __int16":
								case "unsigned __int16":
								case "_WORD":
									isbyte = true;
									break;
							}
						}

						if (isbyte)
						{
							writer.WriteLine("\t\tmovzx eax, [{0}]", argnames[i]);
							writer.WriteLine("\t\tpush eax");
						}
						else
						{
							writer.WriteLine("\t\tpush [{0}]", argnames[i]);
						}
					}
					else
					{
						writer.WriteLine("\t\tmov {0}, [{1}]", argregs[i], argnames[i]);
					}
				}

				writer.WriteLine("\t\tcall {0}Ptr", name);

				int stackcnt = argregs.Count(string.IsNullOrEmpty);

				if (stackcnt > 0)
				{
					writer.WriteLine("\t\tadd esp, {0}", stackcnt * 4);
				}

				if (returntype == "bool")
				{
					writer.WriteLine("\t\tmov result, {0}", boolregs[returnreg]);
				}
				else if (returnreg == "st0")
				{
					writer.WriteLine("\t\tfstp result");
				}
				else if (returntype != "void")
				{
					writer.WriteLine("\t\tmov result, {0}", returnreg);
				}

				writer.WriteLine("\t}");

				if (returntype != "void")
				{
					writer.WriteLine("\treturn result;");
				}

				writer.WriteLine("}");
			}
		}

		static void giveMeTheThings(List<string> arglist, List<string> argdecls, List<string> argnames, List<string> argregs)
		{
			foreach (string arg in arglist)
			{
				Match match = argument.Match(arg);

				string argdecl = match.Groups[1].Value.Trim();
				string argname;

				if (functionptr.IsMatch(argdecl))
				{
					argname = functionptr.Match(argdecl).Groups["name"].Value;
				}
				else
				{
					argname = argdecl.Split(' ').Last().TrimStart('*');
				}

				switch (argname)
				{
					case "size":
						argname = "_size";
						argdecl = argdecl.Replace("size", "_size").Replace("_size_t", "size_t");
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
		}

		static void getArgList(string arguments, List<string> arglist)
		{
			int c = 0;
			while (c < arguments.Length)
			{
				if (arguments[c] == ' ')
				{
					c++;
				}

				int comma = arguments.IndexOf(',', c);

				if (comma == -1)
				{
					arglist.Add(arguments.Substring(c));
					break;
				}

				int paren = arguments.IndexOf('(', c);

				if (paren > -1 && paren < comma)
				{
					comma = arguments.IndexOf(',', arguments.IndexOf(')', arguments.IndexOf('(', paren + 1)));
				}

				if (comma == -1)
				{
					arglist.Add(arguments.Substring(c));
					break;
				}

				arglist.Add(arguments.Substring(c, comma - c));
				c = comma + 1;
			}
		}
	}
}