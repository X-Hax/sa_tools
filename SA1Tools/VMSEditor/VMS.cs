using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VMSEditor
{
	public class VMSFile
	{
		[IniAlwaysInclude]
		public string Title;
		[IniAlwaysInclude]
		public string Description;
		[IniAlwaysInclude]
		[IniName("Author")]
		public string AppName;
		private uint Length;

		// Icon should be here too, maybe later

		public static void DecryptData(ref byte[] input)
		{
			// C# code by Sappharad
			// Original Perl code by Darksecond
			// Original post here: http://assemblergames.com/l/threads/decoding-decrypting-sonic-adventure-ranking-data.60036/#post-863289
			byte[] xor_code = new byte[] { 0x41, 0x54, 0x45, 0x5A };
			byte[] plus_val = new byte[] { 0x41, 0x4E, 0x41, 0x4E };
			byte[] tmp_val = new byte[] { 0, 0, 0, 0 };

			for (int i = 0; i < input.Length / 4; i++)
			{
				int tmp_val2 = 0;
				for (int j = 0; j < 4; j++)
				{
					input[i * 4 + j] = (byte)(input[i * 4 + j] ^ tmp_val[j] ^ xor_code[j]);
					int next = (tmp_val[j] + plus_val[j] + tmp_val2);
					tmp_val2 = (next >> 8);
					tmp_val[j] = (byte)(next & 0xFF);
				}
			}
		}

		public static byte[] GetTextItem(ref byte[] text, int start, int maxlength)
		{
			List<byte> result = new List<byte>();
			for (int a = start; a < start + maxlength; a++)
			{
				if (text[a] == 0) break;
				result.Add(text[a]);
			}
			return result.ToArray();
		}

		public static uint CalculateChecksum(ref byte[] buf, int start, int end)
		{
			// Code by Sappharad
			uint result = 0;
			for (int i = start; i < end; i++)
			{
				int notByte = buf[i];
				if (notByte >= 128)
				{
					notByte -= 256;
				}
				result = (uint)(result + notByte);
			}
			return ~result;
		}

		public static int CalculateUploadCRC(ref byte[] bindata)
		{
			int val = 65535;
			ushort CRCPOLY1 = 4129; // 0x1021

			int maxbyte = 0x50; // 80 bytes

			for (int i = 4; i < maxbyte; i++)
			{
				val ^= (bindata[i] << 8);
				for (int k = 0; k < 8; k++)
				{
					if ((val & 32768) != 0)
					{
						val = (val << 1) ^ CRCPOLY1;
					}
					else
					{
						val <<= 1;
					}
					val &= 65535;
				}
			}
			val ^= 65535;   // Bit turnover
			val &= 65535;

			return val;
		}

		public static byte[] GetDataFromHTML(byte[] file)
		{
			for (int u = 0; u < file.Length - 15; u++)
			{
				if (System.Text.Encoding.GetEncoding(932).GetString(file, u, 15) == "NAME=\"chaodata\"")
				{
					// Load Base64
					StringBuilder b64str = new StringBuilder();
					for (int k = u + 23; k < file.Length - 2; k++)
					{
						if (file[k] == 0x22 && file[k + 1] == 0x3E)
							break;
						else
							b64str.Append((Convert.ToChar(file[k])).ToString());
					}
					// Decode Base64, decrypt
					byte[] data = Convert.FromBase64String(b64str.ToString());
					VMSFile.DecryptData(ref data);
					return data;
				}
			}
			return null;
		}

		private void SaveDecodedData(string source)
		{
			byte[] decr = VMSFile.GetDataFromHTML(File.ReadAllBytes(source));
			string filename = Path.Combine(Path.GetDirectoryName(source), Path.GetFileNameWithoutExtension(source + "_data.bin"));
			File.WriteAllBytes(filename, decr);
			System.Windows.Forms.MessageBox.Show("Decrypted data saved as " + filename + ".");
		}

		public VMSFile() { }

        public VMSFile(byte[] file)
        {
            // In VMU game files, header data starts at 0x200. In regular VMS files such as savegames it starts at 0x0.
            // The "game" flag is stored in the VMU's filesystem and the VMI file.
            // There isn't a good way to tell if the VMS file is a game file or not if the only reference is the VMS file itself.
            // However, the game files I've tried so far all have '(' at certain addresses in the beginning.
            int gameFileOffset = (file[0x3] == 0x28 && file[0xB] == 0x28) ? 0x200 : 0;
            byte[] title_b = GetTextItem(ref file, gameFileOffset, 16);
            byte[] description_b = GetTextItem(ref file, gameFileOffset + 0x10, 32);
            byte[] appname_b = GetTextItem(ref file, gameFileOffset + 0x30, 16);
            Description = System.Text.Encoding.GetEncoding(932).GetString(description_b);
            Title = System.Text.Encoding.GetEncoding(932).GetString(title_b);
            AppName = System.Text.Encoding.GetEncoding(932).GetString(appname_b);
            Length = (uint)file.Length;
        }

        public uint GetLength()
        {
            return Length;
        }
    }
}
