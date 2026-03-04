using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;

// Various archive formats from Sonic Shuffle.
namespace ArchiveLib
{
	#region Sonic Shuffle MDL
	/// <summary>MDL arhives from Sonic Shuffle.</summary>
	public class MDLArchive : GenericArchive
	{
		public override void CreateIndexFile(string path)
		{
			CreateDefaultIndexFile(path);
		}

		public class MDLArchiveEntry : GenericArchiveEntry
		{
			public MDLEntryType Type;

			public override Bitmap GetBitmap()
			{
				throw new NotImplementedException();
			}

			public MDLArchiveEntry(byte[] data, string name)
			{
				Name = name;
				Data = data;
			}

			public MDLArchiveEntry() { }
		}

		/// <summary>
		/// MDL entry type, such as PVM, Ninja Chunk Model, Ninja Motion etc.
		/// </summary>
		public enum MDLEntryType : uint
		{
			/// <summary>Entry is a PVM texture archive.</summary>
			PVM = 1,
			/// <summary>Entry is a Ninja Chunk model.</summary>
			ChunkModel = 2,
			/// <summary>Entry is a Ninja Motion.</summary>
			Motion = 4,
			/// <summary>Entry is a Ninja Shape Motion.</summary>
			ShapeMotion = 8,
			/// <summary>Entry is unknown binary data.</summary>
			Unknown = 10,
		}

		public MDLArchive(byte[] file)
		{
			bool bigendbk = ByteConverter.BigEndian;
			if (file[0] == 0)
				ByteConverter.BigEndian = true;
			int count = ByteConverter.ToUInt16(file, 2);
			Entries = new List<GenericArchiveEntry>(count);
			for (int i = 0; i < count; i++)
			{
				MDLEntryType type = (MDLEntryType)BitConverter.ToUInt32(file, 8 + i * 12);
				int size = BitConverter.ToInt32(file, 12 + i * 12);
				int offset = BitConverter.ToInt32(file, 16 + i * 12);
				Console.WriteLine("Entry {0} type {1} at offset {2}: size {3}", i, type.ToString(), offset, size);
				byte[] entrydata = new byte[size];
				Array.Copy(file, offset, entrydata, 0, size);
				string extension;
				switch (type)
				{
					case MDLEntryType.PVM:
						extension = ".pvm";
						break;
					case MDLEntryType.ShapeMotion:
					case MDLEntryType.Motion:
						extension = ".njm";
						break;
					case MDLEntryType.ChunkModel:
						extension = ".nj";
						break;
					case MDLEntryType.Unknown:
					default:
						extension = ".bin";
						break;
				}
				Entries.Add(new MDLArchiveEntry(entrydata, i.ToString("D3") + extension));
			}
			ByteConverter.BigEndian = bigendbk;
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		public override GenericArchiveEntry NewEntry()
		{
			return new MDLArchiveEntry();
		}
	}
	#endregion

	#region Sonic Shuffle MDT

	/// <summary>MDT arhives from Sonic Shuffle.</summary>
	public class MDTArchive : GenericArchive
	{
		const uint Magic_SMPB = 0x42504D53; // SMPB
		const uint Magic_SMSB = 0x42534D53; // SMPB
		const uint Magic_SFPB = 0x42504653; // SFPB
		const uint Magic_SDRV = 0x56524453; // SDRV
		const uint Magic_SFOB = 0x424F4653; // SFOB
		const uint Magic_SMLT = 0x544C4D53; // SMLT
		const uint Magic_SOSB = 0x42534F53; // SOSB

		public override void CreateIndexFile(string path)
		{
			CreateDefaultIndexFile(path);
		}

		public class MDTArchiveEntry : GenericArchiveEntry
		{
			public override Bitmap GetBitmap()
			{
				throw new NotImplementedException();
			}

			public MDTArchiveEntry(byte[] data, string name)
			{
				Name = name;
				Data = data;
			}

			public MDTArchiveEntry() { }
		}

		/// <summary>Retrieves the file extension for the MDT entry.</summary>
		/// <param name="data">Byte array to analyze.</param>
		/// <returns>Extension string with the leading dot, such as ".mpb".</returns>
		public static string GetEntryExtension(byte[] data)
		{
			// TODO: Maybe use a generic method?
			if (data.Length < 4)
				return ".bin";
			switch (BitConverter.ToUInt32(data, 0))
			{
				case Magic_SMLT:
					return ".mlt";
				case Magic_SMPB:
					return ".mpb";
				case Magic_SMSB:
					return ".msb";
				case Magic_SFPB:
					return ".fpb";
				case Magic_SFOB:
					return ".fob";
				case Magic_SDRV:
					return ".drv";
				case Magic_SOSB:
					return ".osb";
				default:
					if (data.Length < 20)
						return ".bin";
					if (BitConverter.ToUInt16(data, 0) == 0x0080 && data[4] == 0x03 && (data[18] == 0x03 || data[18] == 0x04))
						return ".adx";
					else return ".bin";
			}
		}

		/// <summary>Variations of the MDT archive.</summary>
		public enum MDTArchiveType
		{
			/// <summary>MDT type 0 stores data chunk lengths 4 bytes before the data begins. Usually it contains Manatee (MPB, MSB etc.) files.</summary>
			Manatee = 0,
			/// <summary>MDT type 1 doesn't store data chunk lengths. Usually it contains ADX file.</summary>
			CRI = 1,
			/// <summary>MDT type 2 is the same as MDT type 1 but the offsets are Big Endian.</summary>
			CRIBigEndian = 2
		}

		/// <summary>Checks the type of the MDT archive.</summary>
		/// <param name="file">Byte array to analyze.</param>
		/// <returns>MDT archive type.</returns>
		public MDTArchiveType Identify(byte[] file)
		{
			if (file[0] == 0)
				return MDTArchiveType.CRIBigEndian;
			// Check if there is ADX
			if (file.Length > 20)
				for (int i = 0; i < file.Length; i += 8)
				{
					if (BitConverter.ToUInt16(file, 0) == 0x0080 && file[4] == 0x03 && (file[18] == 0x03 || file[18] == 0x04))
						return MDTArchiveType.CRI;
				}
			return MDTArchiveType.Manatee;
		}

		public MDTArchive(byte[] file)
		{
			bool bigendbk = ByteConverter.BigEndian;
			MDTArchiveType type = Identify(file);
			if (type == MDTArchiveType.CRIBigEndian)
				ByteConverter.BigEndian = true;
			int firstoffset = ByteConverter.ToInt32(file, 0);
			int count = firstoffset / 4;
			Console.WriteLine("Number of entries: {0}", count);
			Entries = new List<GenericArchiveEntry>(count);
			List<int> offsets = new List<int>(count);
			for (int i = 0; i < count; i++)
			{
				int offset = ByteConverter.ToInt32(file, i * 4);
				Console.WriteLine("Entry {0} at header offset {1}", i, offset.ToString("X"));
				offsets.Add(offset);
			}
			for (int u = 0; u < count; u++)
			{
				//Type 0 archives store chunk length in the 4 bytes before the data begins
				int type1_offset = type == MDTArchiveType.Manatee ? 4 : 0;
				int end = file.Length - 1;
				if (u < count - 1)
					end = offsets[u + 1];
				int size = end - offsets[u];
				// Get size for Type 0 archives
				if (type == MDTArchiveType.Manatee)
					size = BitConverter.ToInt32(file, offsets[u]);
				byte[] entrydata = new byte[size];
				Array.Copy(file, offsets[u] + type1_offset, entrydata, 0, size);
				string extension = GetEntryExtension(entrydata);
				Entries.Add(new MDTArchiveEntry(entrydata, u.ToString("D3") + extension));
			}
			ByteConverter.BigEndian = bigendbk;
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		public override GenericArchiveEntry NewEntry()
		{
			return new MDTArchiveEntry();
		}
	}
	#endregion
}