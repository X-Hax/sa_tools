using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using static ArchiveLib.GenericArchive;

// Ninja Binary (.nj) and its Gamecube (.gj) and Xbox variants (.xj).
// Technically this is not an archive but its chunk structure can be parsed to retrieve individual entries.
// Treating NJ files as archives makes it easier to modify individual items contained within.

namespace ArchiveLib
{
	public class NinjaBinaryFile : GenericArchive
	{
		public bool IsGinja;

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

		public static bool Identify(byte[] data)
		{
			if (data == null || data.Length < 8)
				return false;
			string magic = System.Text.Encoding.ASCII.GetString(data, 0, 4);
			return Regex.IsMatch(magic, @"^[A-Z]+$");
		}

		public static bool Identify(byte[] data, int offset)
		{
			if (data == null || (data.Length - offset) < 8)
				return false;
			string magic = System.Text.Encoding.ASCII.GetString(data, offset, 4);
			return Regex.IsMatch(magic, @"^[A-Z]+$");
		}

		public NinjaBinaryFile()
		{
			Entries = new List<GenericArchiveEntry>();
		}

		public NinjaBinaryFile(byte[] file)
		{
			Entries = new List<GenericArchiveEntry>();
			int count = 0;
			int i = 0;
			while (i < file.Length - 8)
			{
				if (Identify(file, i))
				{
					Console.WriteLine("Trying chunk at " + i.ToString("X"));
					NinjaBinaryEntry entry = new NinjaBinaryEntry(file, i);
					string magic = System.Text.Encoding.ASCII.GetString(entry.Data, 0, 4);
					switch (magic)
					{
						case "NJBM":
						case "NJCM":
							magic = "NJ";
							IsGinja = false;
							break;
						case "NJTL":
							IsGinja = false;
							break;
						case "GJBM":
						case "GJCM":
							magic = "GJ";
							IsGinja = true;
							break;
						case "GJTL":
							IsGinja = true;
							break;
						case "NMDM":
							magic = "NJM";
							break;
					}
					Console.WriteLine("\tAdded " + magic + " chunk at " + i.ToString("X"));
					entry.Name = count.ToString("D3") + "." + magic;
					Entries.Add(entry);
					i += entry.Data.Length;
					count++;
				}
			}
		}

		public override byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			foreach (GenericArchiveEntry entry in Entries)
			{
				result.AddRange(entry.Data);
			}
			result.AddRange(new byte[result.Count % 16]);
			return result.ToArray();
		}
	}

	public class NinjaBinaryEntry : GenericArchiveEntry
	{
		public NinjaBinaryEntry(byte[] data, int offset)
		{
			List<byte> result = new List<byte>();
			// Back up Big Endian mode
			bool bigEndianBk = ByteConverter.BigEndian;
			bool sizeIsLittleEndian = true; // In Gamecube games, size can be either Big or Little Endian.
			// This check is done because in PSO GC chunk size is in Little Endian despite the rest of the data being Big Endian.
			// First, determine whether size is Big Endian or not.
			ByteConverter.BigEndian = true;
			sizeIsLittleEndian = BitConverter.ToUInt32(data, offset + 4) < ByteConverter.ToUInt32(data, offset + 4);
			// Then, check if the actual data is Big Endian. Works in NJBM, NJCM and NJTL.
			ByteConverter.BigEndian = BitConverter.ToUInt32(data, offset + 8) > ByteConverter.ToUInt32(data, offset + 8);
			// Get chunk size
			int size = sizeIsLittleEndian ? BitConverter.ToInt32(data, offset + 4) : ByteConverter.ToInt32(data, offset + 4);
			// Get chunk data
			Console.WriteLine("\tChunk size: {0}", size.ToString());
			byte[] chunk = new byte[size + 8];
			Array.Copy(data, offset, chunk, 0, size + 8);
			result.AddRange(chunk);
			// Check if there's a POF0 chunk
			int offset_pofmg = offset + 8 + size;
			// Align by 4
			offset_pofmg += offset_pofmg % 4;
			// If there's a POF0 chunk, retrieve it
			if (offset_pofmg < data.Length - 8)
			{
				string magic = System.Text.Encoding.ASCII.GetString(data, offset_pofmg, 4);
				if (magic == "POF0")
				{
					// Get chunk size
					int pofsize = sizeIsLittleEndian ? BitConverter.ToInt32(data, offset_pofmg + 4) : ByteConverter.ToInt32(data, offset_pofmg + 4);
					// Get chunk data
					Console.WriteLine("\tPOF size: {0}", pofsize.ToString());
					byte[] pofchunk = new byte[pofsize + 8];
					Array.Copy(data, offset_pofmg, pofchunk, 0, pofsize + 8);
					result.AddRange(pofchunk);
					// Align by 4
					result.AddRange(new byte[result.Count % 4]);
				}
			}
			Data = result.ToArray();
		}

		public NinjaBinaryEntry(string filename)
		{
			Name = Path.GetFileName(filename);
			Data = File.ReadAllBytes(filename);
		}

		public override Bitmap GetBitmap()
		{
			throw new NotImplementedException();
		}
	}
}