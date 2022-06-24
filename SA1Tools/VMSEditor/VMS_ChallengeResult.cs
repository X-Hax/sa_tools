using System;
using System.Collections.Generic;

namespace VMSEditor
{
	public enum DataIDs : uint
	{
		EventResultChecksum = 0xB5D8B4DD, // Treasure hunts
		CartResultChecksum = 0xB6B1C421, // Twinkle Circuit (Sampa GP)
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
		public DataIDs DataType { get; set; }

		public PDATA UserData { get; set; }
		public EventResultDataChild ResultData { get; set; }

		public VMSChallengeResult(byte[] rawdata, int startIndex)
		{
			DataType = (DataIDs)BitConverter.ToUInt32(rawdata, 0x04);
			UserData = new PDATA(rawdata, 0x08);
			ResultData = new EventResultDataChild(rawdata, 0x44);
		}

		public VMSChallengeResult(byte[] htmlpage)
		{
			byte[] decr = VMSFile.GetDataFromHTML(htmlpage);
			DataType = (DataIDs)BitConverter.ToUInt32(decr, 0x04);
			UserData = new PDATA(decr, 0x08);
			ResultData = new EventResultDataChild(decr, 0x44);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(BitConverter.GetBytes((uint)0)); // Checksum
			result.AddRange(BitConverter.GetBytes((uint)DataType)); // Hardcoded data ID
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
