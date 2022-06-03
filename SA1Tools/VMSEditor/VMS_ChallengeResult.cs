using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}

	public class PDATA_US: PDATA
	{
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

	public class EventResultDataChild
	{
		public uint EventID { get; set; }
		public uint EventTime { get; set; }
		public uint Character { get; set; }

		public EventResultDataChild(byte[] file, int startindex)
		{
			EventID = BitConverter.ToUInt32(file, startindex);
			EventTime = BitConverter.ToUInt32(file, startindex+4);
			Character = BitConverter.ToUInt32(file, startindex+8);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(BitConverter.GetBytes(EventID));
			result.AddRange(BitConverter.GetBytes(EventTime));
			result.AddRange(BitConverter.GetBytes(Character));
			return result.ToArray();
		}
	}

	public class VMSChallengeResult
	{
		public uint Checksum { get; set; }
		
		public const uint EventResultChecksum = 0xB5D8B4DD;

		public PDATA UserData { get; set; }
		public EventResultDataChild ResultData { get; set; }

		public VMSChallengeResult(byte[] rawdata, int startIndex)
		{
			UserData = new PDATA(rawdata, 0x08);
			ResultData = new EventResultDataChild(rawdata, 0x44);
		}

		public VMSChallengeResult(byte[] htmlpage)
		{
			byte[] decr = VMSFile.GetDataFromHTML(htmlpage);
			UserData = new PDATA(decr, 0x08);
			ResultData = new EventResultDataChild(decr, 0x44);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(BitConverter.GetBytes((uint)0)); // Checksum
			result.AddRange(BitConverter.GetBytes(EventResultChecksum)); // Hardcoded checksum
			result.AddRange(UserData.GetBytes());
			result.AddRange(ResultData.GetBytes());
			byte[] end = result.ToArray();
			int checksum = VMSFile.CalculateUploadCRC(ref end);
			result.RemoveRange(0, 4);
			result.InsertRange(0, BitConverter.GetBytes(checksum));
			return result.ToArray();
		}
	}
}
