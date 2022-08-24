using System;
using System.Collections.Generic;
using System.Text;

namespace VMSEditor
{
	public class VMS_ChaoGardenSave
	{
		// VMS Header up to 0x680
		// 8C AD B9 A3 at 0x680, doesn't seem to change
		// 70 A6 7C 8B at 0x684, doesn't seem to change
		// Zeroes at 0x688-0x68B
		// Data begins at 0x68C
		public string[] PearlCourseNames; // 8x3 = 24 bytes each, total 120 bytes
		public string[] AmethystCourseNames;
		public string[] SapphireCourseNames;
		public string[] RubyCourseNames;
		public string[] EmeraldCourseNames;
		public byte[] PearlCourseTimes; // 3x3 = 9 bytes, total 45 bytes
		public byte[] AmethystCourseTimes;
		public byte[] SapphireCourseTimes;
		public byte[] RubyCourseTimes;
		public byte[] EmeraldCourseTimes;
		// Unknown bytes 0x731-0x7FF, 0x782 to 0x788 seems to be used, the rest is zeroes
		public byte[] UnknownBytes731x77F; // 79 bytes
		public byte UnknownFlag0; // 0x780
		public byte UnknownFlag1; // 0x781
		public byte UnknownFlag2; // 0x782, can be 0 or 1
		public byte UnknownFlag3; // 0x783, can be 0 or 1
		public byte UnknownFlag4; // 0x784
		public byte UnknownFlag5; // 0x785
		public byte UnknownFlag6; // 0x786
		public byte UnknownFlag7; // 0x787
		public byte UnknownFlag8; // 0x788, can be 0 or 1
		public byte[] UnknownBytes789x7FF; // 119 bytes
		public List<VMS_Chao> GardenChao; // 24x512 = 12288 bytes

		public VMS_ChaoGardenSave()
		{
			GardenChao = new List<VMS_Chao>();
			GardenChao.Add(new VMS_Chao());
			PearlCourseNames = new string[3];
			AmethystCourseNames = new string[3];
			SapphireCourseNames = new string[3];
			RubyCourseNames = new string[3];
			EmeraldCourseNames = new string[3];
			PearlCourseTimes = new byte[9];
			AmethystCourseTimes = new byte[9];
			SapphireCourseTimes = new byte[9];
			RubyCourseTimes = new byte[9];
			EmeraldCourseTimes = new byte[9];
			UnknownFlag8 = 1;
			UnknownBytes731x77F = new byte[79];
			UnknownBytes789x7FF = new byte[119];
			PearlCourseNames[0] = AmethystCourseNames [0] = SapphireCourseNames[0] = RubyCourseNames[0] = EmeraldCourseNames [0] = "CHAOTI"; // チャオキチ
			PearlCourseNames[1] = AmethystCourseNames [1] = SapphireCourseNames[1] = RubyCourseNames[1] = EmeraldCourseNames [1] = "CHAOMA"; // チャオマル
			PearlCourseNames[2] = AmethystCourseNames [2] = SapphireCourseNames[2] = RubyCourseNames[2] = EmeraldCourseNames [2] = "CHAOCHA"; // チャオチャオ
			// 1
			PearlCourseTimes[0] = AmethystCourseTimes[0] = SapphireCourseTimes[0] = RubyCourseTimes[0] = EmeraldCourseTimes[0] = 0;
			PearlCourseTimes[1] = AmethystCourseTimes[1] = SapphireCourseTimes[1] = RubyCourseTimes[1] = EmeraldCourseTimes[1] = 0;
			PearlCourseTimes[2] = AmethystCourseTimes[2] = SapphireCourseTimes[2] = RubyCourseTimes[2] = EmeraldCourseTimes[2] = 8;
			// 2
			PearlCourseTimes[3] = AmethystCourseTimes[3] = SapphireCourseTimes[3] = RubyCourseTimes[3] = EmeraldCourseTimes[3] = 0;
			PearlCourseTimes[4] = AmethystCourseTimes[4] = SapphireCourseTimes[4] = RubyCourseTimes[4] = EmeraldCourseTimes[4] = 0;
			PearlCourseTimes[5] = AmethystCourseTimes[5] = SapphireCourseTimes[5] = RubyCourseTimes[5] = EmeraldCourseTimes[5] = 9;
			// 3
			PearlCourseTimes[6] = AmethystCourseTimes[6] = SapphireCourseTimes[6] = RubyCourseTimes[6] = EmeraldCourseTimes[6] = 0;
			PearlCourseTimes[7] = AmethystCourseTimes[7] = SapphireCourseTimes[7] = RubyCourseTimes[7] = EmeraldCourseTimes[7] = 0;
			PearlCourseTimes[8] = AmethystCourseTimes[8] = SapphireCourseTimes[8] = RubyCourseTimes[8] = EmeraldCourseTimes[8] = 10;
		}

		public byte[] GetBytes(bool jp)
		{
			List<byte> result = new List<byte>();
			result.AddRange(jp ? Properties.Resources.garden_jp : Properties.Resources.garden_us);
			result.AddRange(BitConverter.GetBytes(0x8B7CA670A3B9AD8C));
			result.AddRange(BitConverter.GetBytes((uint)0));
			// Names
			for (int i = 0; i < 3; i++)
				result.AddRange(VMS_Chao.GetChaoNameBytes(PearlCourseNames[i]));
			for (int i = 0; i < 3; i++)
				result.AddRange(VMS_Chao.GetChaoNameBytes(AmethystCourseNames[i]));
			for (int i = 0; i < 3; i++)
				result.AddRange(VMS_Chao.GetChaoNameBytes(SapphireCourseNames[i]));
			for (int i = 0; i < 3; i++)
				result.AddRange(VMS_Chao.GetChaoNameBytes(RubyCourseNames[i]));
			for (int i = 0; i < 3; i++)
				result.AddRange(VMS_Chao.GetChaoNameBytes(EmeraldCourseNames[i]));
			// Time
			for (int i = 0; i < 9; i++)
				result.Add(PearlCourseTimes[i]);
			for (int i = 0; i < 9; i++)
				result.Add(AmethystCourseTimes[i]);
			for (int i = 0; i < 9; i++)
				result.Add(SapphireCourseTimes[i]);
			for (int i = 0; i < 9; i++)
				result.Add(RubyCourseTimes[i]);
			for (int i = 0; i < 9; i++)
				result.Add(EmeraldCourseTimes[i]);
			// Unknown data
			result.AddRange(UnknownBytes731x77F);
			result.Add(UnknownFlag0);
			result.Add(UnknownFlag1);
			result.Add(UnknownFlag2);
			result.Add(UnknownFlag3);
			result.Add(UnknownFlag4);
			result.Add(UnknownFlag5);
			result.Add(UnknownFlag6);
			result.Add(UnknownFlag7);
			result.Add(UnknownFlag8);
			result.AddRange(UnknownBytes789x7FF);
			// Chao
			for (int c = 0; c < 24; c++)
			{
				if (c < GardenChao.Count)
					result.AddRange(GardenChao[c].GetBytes());
				else
					result.AddRange(new VMS_Chao().GetBytes());
			}
			return result.ToArray();
		}

		public VMS_ChaoGardenSave(byte[] file)
		{
			GardenChao = new List<VMS_Chao>();
			PearlCourseNames = new string[3];
			AmethystCourseNames = new string[3];
			SapphireCourseNames = new string[3];
			RubyCourseNames = new string[3];
			EmeraldCourseNames = new string[3];
			PearlCourseTimes = new byte[9];
			AmethystCourseTimes = new byte[9];
			SapphireCourseTimes = new byte[9];
			RubyCourseTimes = new byte[9];
			EmeraldCourseTimes = new byte[9];
			UnknownBytes731x77F = new byte[79];
			UnknownBytes789x7FF = new byte[119];
			Array.Copy(file, 0x731, UnknownBytes731x77F, 0, 79);
			Array.Copy(file, 0x789, UnknownBytes789x7FF, 0, 119);
			StringBuilder sb = new StringBuilder();
			// Check if the header is weird
			ulong headerid = BitConverter.ToUInt64(file, 0x680);
			if (headerid != 0x8B7CA670A3B9AD8C && headerid != 0)
				System.Windows.Forms.MessageBox.Show("Default data ID is different: " + headerid.ToString("X"), "Chao Editor Message", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
			// Check if any unknown data is set
			for (int i = 0; i < 79; i++)
				if (UnknownBytes731x77F[i] != 0)
					sb.Append("0x" + (0x731 + i).ToString("X") + " is " + UnknownBytes731x77F[i].ToString("X") + System.Environment.NewLine);
			for (int i = 0; i < 119; i++)
				if (UnknownBytes789x7FF[i] != 0)
					sb.Append("0x" + (0x731 + i).ToString("X") + " is " + UnknownBytes789x7FF[i].ToString("X") + System.Environment.NewLine);
			if (sb.Length > 0)
				System.Windows.Forms.MessageBox.Show("Unknown data detected:\n" + sb.ToString(), "Chao Editor Message", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
			// Names
			for (int i = 0; i < 3; i++)
			{
				PearlCourseNames[i] = VMS_Chao.ReadChaoName(file, 0x68C + i * 8);
				AmethystCourseNames[i] = VMS_Chao.ReadChaoName(file, 0x6A4 + i * 8);
				SapphireCourseNames[i] = VMS_Chao.ReadChaoName(file, 0x6BC + i * 8);
				RubyCourseNames[i] = VMS_Chao.ReadChaoName(file, 0x6D4 + i * 8);
				EmeraldCourseNames[i] = VMS_Chao.ReadChaoName(file, 0x6EC + i * 8);
			}
			// Time
			for (int t = 0; t < 9; t++)
			{
				PearlCourseTimes[t] = file[0x704 + t];
				AmethystCourseTimes[t] = file[0x70D + t];
				SapphireCourseTimes[t] = file[0x716 + t];
				RubyCourseTimes[t] = file[0x71F + t];
				EmeraldCourseTimes[t] = file[0x728 + t];
			}
			// Flags
			UnknownFlag0 = file[0x780];
			UnknownFlag1 = file[0x781];
			UnknownFlag2 = file[0x782];
			UnknownFlag3 = file[0x783];
			UnknownFlag4 = file[0x784];
			UnknownFlag5 = file[0x785];
			UnknownFlag6 = file[0x786];
			UnknownFlag7 = file[0x787];
			UnknownFlag8 = file[0x788];
			// Chao
			for (int c = 0; c < 24; c++)
			{
				GardenChao.Add(new VMS_Chao(file, 0x800 + c * 512));
			}
		}
	}
}
