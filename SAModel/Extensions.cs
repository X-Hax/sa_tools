using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SonicRetro.SAModel
{
	public static class Extensions
	{
		public static void Align(this List<byte> me, int alignment)
		{
			int off = me.Count % alignment;
			if (off == 0)
				return;
			me.AddRange(new byte[alignment - off]);
		}

		public static string GetCString(this byte[] file, int address)
		{
			return GetCString(file, address, Encoding.UTF8);
		}

		public static string GetCString(this byte[] file, int address, Encoding encoding)
		{
			int count = 0;
			while (file[address + count] != 0)
				count++;
			return encoding.GetString(file, address, count);
		}

		public static string ToC(this float num)
		{
			string result = num.ToLongString();
			if (result.Contains("."))
				result += "f";
			return result;
		}

		public static string ToLongString(this float input)
		{
			string str = input.ToString(NumberFormatInfo.InvariantInfo);
			// if string representation was collapsed from scientific notation, just return it: 
			if (!str.Contains("E") & !str.Contains("e"))
				return str;
			str = str.ToUpper();
			char decSeparator = '.';
			string[] exponentParts = str.Split('E');
			string[] decimalParts = exponentParts[0].Split(decSeparator);
			// fix missing decimal point: 
			if (decimalParts.Length == 1)
			{
				decimalParts = new[]
				{
					exponentParts[0],
					"0"
				};
			}
			int exponentValue = int.Parse(exponentParts[1]);
			string newNumber = decimalParts[0] + decimalParts[1];
			string result;
			if (exponentValue > 0)
				result = newNumber + GetZeros(exponentValue - decimalParts[1].Length);
			else
			{
				// negative exponent 
				result = string.Empty;
				if (newNumber.StartsWith("-"))
				{
					result = "-";
					newNumber = newNumber.Substring(1);
				}
				result += "0" + decSeparator + GetZeros(exponentValue + decimalParts[0].Length) + newNumber;
				result = result.TrimEnd('0');
			}
			return result;
		}

		public static string GetZeros(int zeroCount)
		{
			if (zeroCount < 0)
				zeroCount = Math.Abs(zeroCount);
			return new string('0', zeroCount);
		}

		public static string ToCHex(this int i)
		{
			if (i < 10 && i > -1)
				return i.ToString(NumberFormatInfo.InvariantInfo);
			return "0x" + i.ToString("X");
		}

		public static string ToCHex(this uint i)
		{
			if (i < 10)
				return i.ToString(NumberFormatInfo.InvariantInfo);
			return "0x" + i.ToString("X");
		}

		public static string ToCHex(this ulong i)
		{
			if (i < 10)
				return i.ToString(NumberFormatInfo.InvariantInfo);
			return "0x" + i.ToString("X");
		}

		public static string ToC(this string str)
		{
			if (str == null)
				return "NULL";
			Encoding enc = Encoding.GetEncoding(932);
			StringBuilder result = new StringBuilder("\"");
			foreach (char item in str)
			{
				switch (item)
				{
					case '\0':
						result.Append(@"\0");
						break;
					case '\a':
						result.Append(@"\a");
						break;
					case '\b':
						result.Append(@"\b");
						break;
					case '\f':
						result.Append(@"\f");
						break;
					case '\n':
						result.Append(@"\n");
						break;
					case '\r':
						result.Append(@"\r");
						break;
					case '\t':
						result.Append(@"\t");
						break;
					case '\v':
						result.Append(@"\v");
						break;
					case '"':
						result.Append(@"\""");
						break;
					case '\\':
						result.Append(@"\\");
						break;
					default:
						if (item < ' ')
							result.AppendFormat(@"\{0}", Convert.ToString((short)item, 8).PadLeft(3, '0'));
						else if (item > '\x7F')
						{
							foreach (byte b in enc.GetBytes(item.ToString()))
								result.AppendFormat(@"\{0}", Convert.ToString(b, 8).PadLeft(3, '0'));
						}
						else
							result.Append(item);
						break;
				}
			}
			result.Append("\"");
			return result.ToString();
		}

		internal static string MakeIdentifier(this string s)
		{
			StringBuilder result = new StringBuilder(s.Length + 1);
			foreach (char item in s)
			{
				if ((item >= '0' & item <= '9') | (item >= 'A' & item <= 'Z') | (item >= 'a' & item <= 'z') | item == '_')
					result.Append(item);
			}
			if (result[0] >= '0' & result[0] <= '9')
				result.Insert(0, '_');
			return result.ToString();
		}

		private static readonly Random rand = new Random();

		public static string GenerateIdentifier()
		{
			return DateTime.Now.Ticks.ToString("X") + rand.Next(0, 65536).ToString("X4");
		}

		public static int AddUnique<T>(this List<T> list, T item)
		{
			if (list.Contains(item))
				return list.IndexOf(item);
			int val = list.Count;
			list.Add(item);
			return val;
		}

		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
		{
			key = pair.Key;
			value = pair.Value;
		}

#if modellog
		internal static void Log(string message)
		{
			System.IO.File.AppendAllText("modellog.log", message);
		}
#endif
	}
}