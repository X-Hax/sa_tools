using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Assimp;
namespace SonicRetro.SAModel
{
	public static class Extensions
	{
		//https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
		static Vector3D NormalizeAngles(Vector3D angles)
		{
			angles.X = NormalizeAngle(angles.X);
			angles.Y = NormalizeAngle(angles.Y);
			angles.Z = NormalizeAngle(angles.Z);
			return angles;
		}

		static float NormalizeAngle(float angle)
		{
			while (angle > 360)
				angle -= 360;
			while (angle < 0)
				angle += 360;
			return angle;
		}

		public static Assimp.Vector3D ToEulerAngles(this Assimp.Quaternion q)
		{
			// https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
			// roll (x-axis rotation)
			var sinr = +2.0 * (q.W * q.X + q.Y * q.Z);
			var cosr = +1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
			var roll = Math.Atan2(sinr, cosr);

			// pitch (y-axis rotation)
			var sinp = +2.0 * (q.W * q.Y - q.Z * q.X);
			double pitch;
			if (Math.Abs(sinp) >= 1)
			{
				var sign = sinp < 0 ? -1f : 1f;
				pitch = (Math.PI / 2) * sign; // use 90 degrees if out of range
			}
			else
			{
				pitch = Math.Asin(sinp);
			}

			// yaw (z-axis rotation)
			var siny = +2.0 * (q.W * q.Z + q.X * q.Y);
			var cosy = +1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
			var yaw = Math.Atan2(siny, cosy);

			return new Assimp.Vector3D((float)(roll * 57.2958d), (float)(pitch * 57.2958d), (float)(yaw * 57.2958d));
		}

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

#if modellog
		internal static void Log(string message)
		{
			System.IO.File.AppendAllText("modellog.log", message);
		}
#endif
	}
}