using System;
using System.Collections.Generic;
using static VMSEditor.SA1DLC;

namespace VMSEditor
{
    public class VMIFile
    {
        public enum VMIFlags
        {
            Protected = 0x1,
            Game = 0x2
        }
        public string Description; // 32 bytes
        public string Copyright; // 32 bytes
        public ushort Year;
        public byte Month;
        public byte Day;
        public byte Hour;
        public byte Minute;
        public byte Second;
        public byte Weekday;
        public ushort Version; // Always 0
        public ushort FileID; // Always 1
        public string ResourceName; // 8 bytes
        public string FileName; // 12 bytes
        public VMIFlags Flags; // 2 bytes (irrelevant for SA1)
        public uint Size;

		public VMIFile(VMSFile data, string ResName, bool ignoreresname = false)
        {
            if (!ignoreresname && ResName.Length > 8)
                System.Windows.Forms.MessageBox.Show("For the VMI file to work correctly, VMS filename should be 8 characters or less.", "DLC Tool Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            Description = data.Description;
            Copyright = data.AppName;
            Year = (ushort)DateTime.Now.Year;
            Month = (byte)DateTime.Now.Month;
            Day = (byte)DateTime.Now.Day;
            Hour = (byte)DateTime.Now.Hour;
            Minute = (byte)DateTime.Now.Minute;
            Second = (byte)DateTime.Now.Second;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    Weekday = 0;
                    break;
                case DayOfWeek.Monday:
                    Weekday = 1;
                    break;
                case DayOfWeek.Tuesday:
                    Weekday = 2;
                    break;
                case DayOfWeek.Wednesday:
                    Weekday = 3;
                    break;
                case DayOfWeek.Thursday:
                    Weekday = 4;
                    break;
                case DayOfWeek.Friday:
                    Weekday = 5;
                    break;
                case DayOfWeek.Saturday:
                    Weekday = 6;
                    break;
            }
            FileID = 1;
            ResourceName = ResName;
            if (data is VMS_DLC dlc)
            {
                FileName = "SONICADV_" + dlc.Identifier.ToString("D3");
                Size = (uint)dlc.GetBytes().Length;
            }
            else
            {
                FileName = "VMSDATA";
                Size = data.GetLength();
            }
        }

        public VMIFile() { }

        public VMIFile(byte[] vmidata)
        {
            byte[] description_b = new byte[32];
            Array.Copy(vmidata, 0x4, description_b, 0, 32);
            byte[] copyright_b = new byte[32];
            Array.Copy(vmidata, 0x4, copyright_b, 0, 32);
            Description = System.Text.Encoding.GetEncoding(932).GetString(description_b);
            Copyright = System.Text.Encoding.GetEncoding(932).GetString(copyright_b);
            Year = BitConverter.ToUInt16(vmidata, 0x44);
            Month = vmidata[0x46];
            Day = vmidata[0x47];
            Hour = vmidata[0x48];
            Minute = vmidata[0x49];
            Second = vmidata[0x4A];
            Weekday = vmidata[0x4B];
            Version = BitConverter.ToUInt16(vmidata, 0x4C);
            FileID = BitConverter.ToUInt16(vmidata, 0x4E);
            byte[] resource_b = new byte[8];
            Array.Copy(vmidata, 0x50, resource_b, 0, 8);
            byte[] filename_b = new byte[12];
            Array.Copy(vmidata, 0x58, filename_b, 0, 12);
            ResourceName = System.Text.Encoding.GetEncoding(932).GetString(resource_b);
            FileName = System.Text.Encoding.GetEncoding(932).GetString(filename_b);
            Flags = (VMIFlags)BitConverter.ToUInt16(vmidata, 0x64);
            Size = BitConverter.ToUInt32(vmidata, 0x68);
        }

        public static byte[] GetString(string str, int size)
        {
            List<byte> result = new List<byte>();
            result.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes(str));
            if (result.Count > size)
            {
                do
                    result.RemoveAt(result.Count - 1);
                while (result.Count > size);
            }
            if (result.Count < size)
            {
                do
                    result.Add(0);
                while (result.Count < size);
            }
            return result.ToArray();
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            byte[] resname = GetString(ResourceName, 8);
            byte[] checksum = new byte[4];
            checksum[0] = (byte)(resname[0] & 0x53); // S
            checksum[1] = (byte)(resname[1] & 0x45); // E
            checksum[2] = (byte)(resname[2] & 0x47); // G
            checksum[3] = (byte)(resname[3] & 0x41); // A
            result.AddRange(checksum);
            result.AddRange(GetString(Description, 32));
            result.AddRange(GetString(Copyright, 32));
            result.AddRange(BitConverter.GetBytes(Year));
            result.Add(Month);
            result.Add(Day);
            result.Add(Hour);
            result.Add(Minute);
            result.Add(Second);
            result.Add(Weekday);
            result.AddRange(BitConverter.GetBytes(Version));
            result.AddRange(BitConverter.GetBytes(FileID));
            result.AddRange(resname);
            result.AddRange(GetString(FileName, 12));
            result.AddRange(BitConverter.GetBytes((ushort)Flags));
            result.AddRange(BitConverter.GetBytes((ushort)0));
            result.AddRange(BitConverter.GetBytes(Size));
            return result.ToArray();
        }

		public static VMIFile GetVMIFromDCI(byte[] dcifile)
		{
			VMIFile result = new();
			if (dcifile[0] == 0xCC)
				result.Flags |= VMIFlags.Game;
			if (dcifile[1] == 0xFF)
				result.Flags |= VMIFlags.Protected;
			result.FileName = System.Text.Encoding.GetEncoding(932).GetString(dcifile, 0x04, 12);
			result.Year = ushort.Parse((dcifile[0x10] << 8 | dcifile[0x11]).ToString());
			result.Month = byte.Parse(dcifile[0x12].ToString());
			result.Day = byte.Parse(dcifile[0x13].ToString());
			result.Hour = byte.Parse(dcifile[0x14].ToString());
			result.Minute = byte.Parse(dcifile[0x15].ToString());
			result.Second = byte.Parse(dcifile[0x16].ToString());
			result.Weekday = byte.Parse(dcifile[0x17].ToString());
			result.Size = (ushort)(512 * BitConverter.ToUInt16(dcifile, 0x18));
			return result;
		}
	}
}
