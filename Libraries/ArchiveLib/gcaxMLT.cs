using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

// Gamecube Multi-Unit (gcaxMLT) container format used for soundbanks in SADX, SA2B, Heroes, Shadow etc.
// This class is for the base archive only, not the invidivual banks (gcaxMPB, gcaxMSB) contained within.
namespace ArchiveLib
{
	#region gcaxMLT
	public class gcaxMLTFile : GenericArchive
	{
		public gcaxMLTFile(byte[] file, string filename = "")
		{
			bool bigend = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;
			// Get number of MLTM entrues
			int size = ByteConverter.ToInt32(file, 0x1C);
			int numfiles = size / 16;
			int fileoffset = 0x20;
			List<int> entrypointers = new List<int>();
			for (int u = 0; u < numfiles; u++)
			{
				if(file[fileoffset + u * 16] != 0xFF)
					entrypointers.Add(ByteConverter.ToInt32(file, fileoffset + u * 16 + 8));
			}
			numfiles = entrypointers.Count;
			//Console.WriteLine("Num files: {0}", numfiles);
			for (int e = 0; e < numfiles; e++)
			{
				int entrysize;
				if (e < numfiles - 1)
					entrysize = entrypointers[e + 1] - entrypointers[e];
				else
					entrysize = file.Length - entrypointers[e] - 32;
				//Console.WriteLine("Entry size: {0}", file.Length - entrypointers[e] - 32);
				Entries.Add(new gcaxMLTEntry(file, fileoffset + e * 16, entrysize, filename));
			}
			ByteConverter.BigEndian = bigend;
		}

		public override byte[] GetBytes()
		{
			byte[] resultData;
			bool bigend = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;
			List<byte> result = new List<byte>();
			// gcaxMLT header
			result.AddRange(System.Text.Encoding.ASCII.GetBytes("gcaxMLT "));
			result.AddRange(ByteConverter.GetBytes((int)0));
			result.AddRange(ByteConverter.GetBytes((int)0)); // Total size at 0xC
			// gcaxMLTM header
			result.AddRange(System.Text.Encoding.ASCII.GetBytes("gcaxMLTM"));
			result.AddRange(ByteConverter.GetBytes((int)0));
			int mltmsize = Math.Max(2, Entries.Count) * 16;
			result.AddRange(ByteConverter.GetBytes(mltmsize));
			// Create array for all data items before adding pointers to individual headers
			Dictionary<gcaxMLTEntry, int> itemArray = new Dictionary<gcaxMLTEntry, int>();
			int pointer = Math.Max(2, Entries.Count) * 16;
			foreach (gcaxMLTEntry entry in Entries)
			{
				itemArray.Add(entry, pointer);
				pointer += entry.Data.Length;
			}
			// Add MLTM items
			foreach (var item in itemArray)
			{
				result.Add((byte)item.Key.Type);
				result.Add(0);
				result.Add(0);
				result.Add(0);
				result.Add((byte)item.Key.BankID);
				result.Add(0);
				result.Add(0);
				result.Add(0);
				result.AddRange(ByteConverter.GetBytes(item.Value));
				result.AddRange(ByteConverter.GetBytes((int)0));
			}
			// Add a dummy item if the count is less than 2 (like in casino_sonictailsvoice.mlt)
			if (itemArray.Count == 1)
			{
				result.AddRange(ByteConverter.GetBytes((int)-1));
				result.AddRange(ByteConverter.GetBytes((int)-1));
				result.AddRange(ByteConverter.GetBytes((int)0));
				result.AddRange(ByteConverter.GetBytes((int)0));
			}
			// Add item data
			foreach (var item in itemArray)
			{
				result.AddRange(item.Key.GetBytes());
			}
			resultData = result.ToArray();
			byte[] sizedata = ByteConverter.GetBytes(resultData.Length);
			Array.Copy(sizedata, 0, resultData, 0xC, 4);
			ByteConverter.BigEndian = bigend;
			return resultData; 
		}

		public override void CreateIndexFile(string path)
		{
			using (System.IO.TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
			{
				foreach (gcaxMLTEntry entry in Entries)
					texList.WriteLine("{0},{1}", entry.Name, entry.BankID.ToString("D2"));
				texList.Flush();
				texList.Close();
			}
		}

		public class gcaxMLTEntry : GenericArchiveEntry
		{
			public gcaxMLTEntryType Type;
			public int BankID;

			public gcaxMLTEntry(byte[] file, int offset, int size, string filename = "")
			{
				bool bigend = ByteConverter.BigEndian;
				ByteConverter.BigEndian = true;
				Type = (gcaxMLTEntryType)file[offset];
				BankID = file[offset + 4];
				Name = (filename == "" ? "BANK" : filename + "_BANK") + BankID.ToString("D2") + GetgcaxMLTItemExtension(file, offset);
				int pointer = ByteConverter.ToInt32(file, offset + 8) + 32;
				//Console.WriteLine("Size: {0}", size);
				Data = new byte[size];
				Array.Copy(file, pointer, Data, 0, size);
				//Console.WriteLine("Entry {0}, Bank {1}, Address {2}, Size {3}, Name {4}", Type.ToString(), BankID.ToString(), pointer.ToString("X"), Data.Length.ToString(), Name);
				ByteConverter.BigEndian = bigend;
			}

			public gcaxMLTEntry(string filename, int bankID)
			{
				Type = GetgcaxMLTEntryTypeFromFilename(filename);
				BankID = bankID;
				if (File.Exists(filename))
					Data = File.ReadAllBytes(filename);
			}

			public gcaxMLTEntry(string filename)
			{ }

			public byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				if (Data != null)
					result.AddRange(Data);
				return result.ToArray();
			}

			public override Bitmap GetBitmap()
			{
				throw new NotImplementedException();
			}
		}

		public gcaxMLTFile()
		{
		}

		public enum gcaxMLTEntryType : byte
		{
			gcaxMSB = 4, // MIDI Sequence Bank
			gcaxMPB = 1, // MIDI Program Bank
		}

		// Related methods
		public static string GetgcaxMLTItemExtension(byte[] file, int offset = 0)
		{
			switch ((gcaxMLTEntryType)file[offset])
			{
				case gcaxMLTEntryType.gcaxMSB:
					return ".gcaxMSB";
				case gcaxMLTEntryType.gcaxMPB:
					return ".gcaxMPB";
				default:
					return ".gcaxWTF";
			}
		}

		public static gcaxMLTEntryType GetgcaxMLTEntryTypeFromFilename(string filename)
		{
			switch (Path.GetExtension(filename).ToLowerInvariant())
			{
				case ".mpb":
				case ".gcaxmpb":
					return gcaxMLTEntryType.gcaxMPB;
				case ".msb":
				case ".gcaxmsb":
					return gcaxMLTEntryType.gcaxMSB;
				default:
					throw new Exception("Unknown GCAX entry extension: " + Path.GetExtension(filename).ToLowerInvariant());
			}
		}
	}
	#endregion
}