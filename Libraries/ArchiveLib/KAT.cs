using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// Very early support for KAT files (multimedia archives in Katana SDK)

namespace ArchiveLib
{
	public class KATFile : GenericArchive
	{
		public override void CreateIndexFile(string path)
		{
			using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
			{
				foreach (GenericArchiveEntry entry in Entries)
				{
					texList.WriteLine(entry.Name);
				}
				texList.Flush();
				texList.Close();
			}
		}
	
		public class KATArchiveEntry : GenericArchiveEntry
		{

			public override Bitmap GetBitmap()
			{
				throw new NotImplementedException();
			}

			public KATArchiveEntry(byte[] data, int index, int numch, int freq, int loop, int bits, int bankID, int sampleID)
			{
				Name = string.Format("{0}_{1}_{2}.wav", index.ToString("D3"), bankID.ToString("D3"), sampleID.ToString("D3"));
				List<byte> wave = new List<byte>();
				wave.AddRange(System.Text.Encoding.ASCII.GetBytes("RIFF"));
				int length = 4 + 24 + 8 + data.Length;
				wave.AddRange(BitConverter.GetBytes(length));
				wave.AddRange(System.Text.Encoding.ASCII.GetBytes("WAVE"));
				wave.AddRange(System.Text.Encoding.ASCII.GetBytes("fmt "));
				wave.AddRange(BitConverter.GetBytes((int)0x10));
				wave.AddRange(BitConverter.GetBytes((short)1));
				wave.AddRange(BitConverter.GetBytes((short)numch));
				wave.AddRange(BitConverter.GetBytes(freq));
				int value = (freq * numch * bits) / 8;
				wave.AddRange(BitConverter.GetBytes(value));
				int value2 = bits * numch / 8;
				wave.AddRange(BitConverter.GetBytes((short)value2));
				wave.AddRange(BitConverter.GetBytes((short)bits));
				wave.AddRange(System.Text.Encoding.ASCII.GetBytes("data"));
				wave.AddRange(BitConverter.GetBytes(data.Length));
				wave.AddRange(data);
				Data = wave.ToArray();
			}
		}

		public KATFile(byte[] file)
		{
			Entries = new List<GenericArchiveEntry>();
			int count = ByteConverter.ToInt32(file, 0);
			List<int> sizehdrs = new List<int>();
			for (int i = 0; i < count; i++)
			{
				int numchannel = ByteConverter.ToInt32(file, 4 + i * 44);
				int entryoff = ByteConverter.ToInt32(file, 4 + i * 44 + 4);
				int size = ByteConverter.ToInt32(file, 4 + i * 44 + 8);
				int frequency = ByteConverter.ToInt32(file, 4 + i * 44 + 12);
				int loop = ByteConverter.ToInt32(file, 4 + i * 44 + 16);
				int bits = ByteConverter.ToInt32(file, 4 + i * 44 + 20);
				int index = ByteConverter.ToInt32(file, 4 + i * 44 + 24);
				int bankIndex = index / 1000;
				int sampleIndex = index % 1000;
				Console.WriteLine("Entry size data {0} at offset {1}: size {2}", i, entryoff, size);
				byte[] data = new byte[size];
				Array.Copy(file, entryoff, data, 0, size);
				Entries.Add(new KATArchiveEntry(data, i, numchannel, frequency, loop, bits, bankIndex, sampleIndex));
			}
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}
	}
}