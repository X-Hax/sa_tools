using System;
using System.Collections.Generic;
using System.Text;

// A 60-byte block of data referenced to as PDATA in website CGI scripts. It is embedded in uploadable VMS files in the region 0x08-0x44 of the decrypted data.
// This data seems to be created from various parts of DC flash memory and depends on the version of the game.

namespace VMSEditor
{
	public class PDATA
	{
		public byte[] Data { get; set; }

		public PDATA(byte[] file, int startindex)
		{
			Data = new byte[60];
			Array.Copy(file, startindex, Data, 0, Data.Length);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(Data);
			return result.ToArray();
		}

		public DateTime TimestampToDate(uint aica)
		{
			DateTime result = new DateTime(1950, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Unspecified); // AICA is 1950 based
			result = result.AddMilliseconds(aica).ToLocalTime();
			return result;
		}

		public long DateTimeToTimestamp(DateTime date)
		{
			TimeSpan result = date - new DateTime(1950, 1, 1, 0, 0, 0);
			return (long)result.TotalSeconds;
		}
	}

	// JP, EU and International
	public class PDATA_JP : PDATA
	{
		public DateTime TimestampFirstBoot; // AICA timestamp when the game was first booted
		public DateTime TimestampLastBoot; // AICA timestamp at the time of save
		public ushort BootedTotal; // Number of times the game booted
		public ushort Unknown;
		public ushort[] BootTimes = new ushort[24]; // Number of times the game booted in 1-hour intervals from 00:00-00:59 to 23:00-23:00

		public PDATA_JP(byte[] file, int startindex) : base(file, startindex)
		{
			TimestampFirstBoot = TimestampToDate(BitConverter.ToUInt32(file, startindex));
			TimestampLastBoot = TimestampToDate(BitConverter.ToUInt32(file, startindex + 4));
			BootedTotal = BitConverter.ToUInt16(file, startindex + 8);
			Unknown = BitConverter.ToUInt16(file, startindex + 10);
			for (int i = 0; i < 24; i++)
			{
				BootTimes[i] = BitConverter.ToUInt16(file, startindex + 12 + i * 2);
			}
		}
	}

	// US 1.004 and 1.005
	public class PDATA_US : PDATA
	{
		// This shit seems totally random and below is just a guess based on a couple of tests I did
		public string MailAccountName { get; set; } // 16 bytes
		public string MailAccountPassword { get; set; } // 16 bytes
		public string ProxyName { get; set; } // 4 bytes
		public uint Pointer1 { get; set; } // 4 bytes 0x8C000070
		public uint Value1 { get; set; } // 4 bytes 0x0001A000
		public uint Pointer2 { get; set; } // 4 bytes 0x8C00F250
		public uint Value2 { get; set; } // 4 bytes 0x0001A05E or 00000D3E
		public uint Pointer3 { get; set; } // 4 bytes 0x8C003CD0 or 8C7A7DF0
		public uint Pointer4 { get; set; } // 4 bytes 0x8C00F250 (same as 2?)

		public PDATA_US(byte[] file, int startindex) : base(file, startindex)
		{
			MailAccountName = Encoding.GetEncoding(932).GetString(file, startindex, 16);
			MailAccountPassword = Encoding.GetEncoding(932).GetString(file, startindex + 16, 16);
			ProxyName = Encoding.GetEncoding(932).GetString(file, startindex + 32, 4);
			Pointer1 = BitConverter.ToUInt32(file, startindex + 0x2C);
			Value1 = BitConverter.ToUInt32(file, startindex + 0x30);
			Pointer2 = BitConverter.ToUInt32(file, startindex + 0x34);
			Value2 = BitConverter.ToUInt32(file, startindex + 0x38);
			Pointer3 = BitConverter.ToUInt32(file, startindex + 0x3C);
			Pointer4 = BitConverter.ToUInt32(file, startindex + 0x40);
		}
	}
}
