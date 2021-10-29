using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;

// Skies of Arcadia (Dreamcast) MLD archives.
namespace ArchiveLib
{
	public class nmldObjectInfo
	{
		public int ObjectPointer;
		public int MotionPointer;
		public int TexlistPointer;
		public int Unknown;
		public byte[] Data;

		public nmldObjectInfo(byte[] srcdata, int start, int end)
		{
			int size = end - start;
			Data = new byte[size - 16];
			Array.Copy(srcdata, start + 16, Data, 0, size - 16);
			ObjectPointer = ByteConverter.ToInt32(Data, 0);
			MotionPointer = ByteConverter.ToInt32(Data, 4);
			TexlistPointer = ByteConverter.ToInt32(Data, 8);
			Unknown = ByteConverter.ToInt32(Data, 12);
			Console.WriteLine("NMLD Entry Info at {0}: NJCM at {1}, NMDM at {2}, NJTL at {3}, Unknown: {4}", start.ToString("X"), ObjectPointer.ToString("X"), MotionPointer.ToString("X"), TexlistPointer.ToString("X"), Unknown.ToString("X"));
		}
	}

	public class nmldObjectTable
	{
		public int PointerInfo;
		public string Name;

		public nmldObjectTable(byte[] data, int start, int index, string name)
		{
			Name = name;
			int Count = ByteConverter.ToInt32(data, start);
			PointerInfo = ByteConverter.ToInt32(data, start + 4);
			int EndPointer = ByteConverter.ToInt32(data, start + 24);
			Console.WriteLine("NMLD Entry at {0}, End at {1}, Count {2}, Data at {3}", start.ToString("X"), EndPointer.ToString("X"), Count, PointerInfo.ToString("X"));
		}

		public byte[] GetBytes(byte[] src, int nextEntry)
		{
			nmldObjectInfo info = new nmldObjectInfo(src, (int)PointerInfo, nextEntry);
			return info.Data;
		}
	}

	public class nmldEntry
	{
		public int idx; // 0x00
		public int unk; // 0x04
		public int ptr_unk1; // 0x08
		public int ptr_unk2; // 0x0C
		public int ptr_unk3; // 0x10
		public int ptr_objTable; // 0x14
		public int ptr_grndTable; // 0x18
		public int ptr_motTable; // 0x1C
		public int ptr_texTable; // 0x20
		public string name; // 0x24 (32 bytes)
		public Vertex pos; // 0x44
		public Vertex rot; // 0x50
		public Vertex scl; // 0x5C

		public nmldEntry(byte[] data, int start)
		{
			idx = ByteConverter.ToInt32(data, start);
			unk = ByteConverter.ToInt32(data, start + 0x04);
			ptr_unk1 = ByteConverter.ToInt32(data, start + 0x08);
			ptr_unk2 = ByteConverter.ToInt32(data, start + 0x0C);
			ptr_unk3 = ByteConverter.ToInt32(data, start + 0x10);
			ptr_objTable = ByteConverter.ToInt32(data, start + 0x14);
			ptr_grndTable = ByteConverter.ToInt32(data, start + 0x18);
			ptr_motTable = ByteConverter.ToInt32(data, start + 0x1C);
			ptr_texTable = ByteConverter.ToInt32(data, start + 0x20);
			int namesize = 0;
			for (int s = 0; s < 32; s++)
			{
				if (data[start + 0x24 + s] != 0)
					namesize++;
				else
					break;
			}
			byte[] namechunk = new byte[namesize];
			Array.Copy(data, start + 0x24, namechunk, 0, namesize);
			name = System.Text.Encoding.ASCII.GetString(namechunk);
			pos = new Vertex(ByteConverter.ToSingle(data, start + 0x44), ByteConverter.ToSingle(data, start + 0x48), ByteConverter.ToSingle(data, start + 0x4C));
			rot = new Vertex(ByteConverter.ToSingle(data, start + 0x50), ByteConverter.ToSingle(data, start + 0x54), ByteConverter.ToSingle(data, start + 0x58));
			scl = new Vertex(ByteConverter.ToSingle(data, start + 0x5C), ByteConverter.ToSingle(data, start + 0x60), ByteConverter.ToSingle(data, start + 0x64));
			Console.WriteLine("NMLD at {0}, ID: {1}, Name: {2}, Entry at: {3}, GRND at: {4}, Motion at: {5}, Texlist at: {6}", start.ToString("X"),idx.ToString(), name, ptr_objTable.ToString("X"), ptr_grndTable.ToString("X"), ptr_motTable.ToString("X"), ptr_texTable.ToString("X"));
		}
	}

	public class MLDArchive : GenericArchive
	{
		public override void CreateIndexFile(string path)
		{
			return;
		}

		public class MLDArchiveEntry : GenericArchiveEntry
		{

			public override Bitmap GetBitmap()
			{
				throw new NotImplementedException();
			}

			public MLDArchiveEntry(byte[] data, string name)
			{
				Name = name;
				Data = data;
			}
		}

		public MLDArchive(byte[] file)
		{
			List<nmldEntry> nmldEntries = new List<nmldEntry>();
			List<nmldObjectTable> nmldObjectEntries = new List<nmldObjectTable>();
			ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0);
			string nmldInfo = "";

			// Read MLD Header
			int nmldCount = ByteConverter.ToInt32(file, 0);
			int ptr_nmldTable = ByteConverter.ToInt32(file, 0x04);
			int eof_nmldTable = ByteConverter.ToInt32(file, 0x08);
			int realdatapointer = ByteConverter.ToInt32(file, 0x0C);
			int textablepointer = ByteConverter.ToInt32(file, 0x10);
			Console.WriteLine("Number of NMLD entries: {0}, NMLD data starts at {1}, real data starts at {2}", nmldCount, ptr_nmldTable.ToString("X"), realdatapointer.ToString("X"));
			int sizereal = textablepointer - realdatapointer;
			int sizenmld = realdatapointer - ptr_nmldTable;
			Console.WriteLine("First entry: {0} size {1}", realdatapointer.ToString("X"), sizereal);
			
			// Extract NMLD stuff
			for (int m = 0; m < nmldCount; m++)
			{
				nmldEntry nmld = new nmldEntry(file, (int)ptr_nmldTable + 104 * m);
				nmldEntries.Add(nmld);

				/*
				if (nmld.ptr_objTable != 0)
				{
					nmldObjectEntries.Add(new nmldObjectTable(file, nmld.ptr_objTable, nmld.idx, nmld.name));
					Console.WriteLine("Create nmldEntry {0} at {1}", m, nmld.ptr_objTable.ToString("X"));
				}
				*/
			}

			for (int e = 0; e < nmldEntries.Count; e++)
			{
				List<int> objTablePointers = new List<int>();
				List<string> motTablePointers = new List<string>();

				//Object Table
				int objTableCount = ByteConverter.ToInt32(file, nmldEntries[e].ptr_objTable);
				int objTableCheck = ByteConverter.ToInt32(file, nmldEntries[e].ptr_objTable + 0x04);
				if (objTableCheck != 0 || objTableCount > 1)
				{
					int objTableStart = nmldEntries[e].ptr_objTable + 0x04;
					Console.WriteLine("Object Table Count: {0}", objTableCount.ToString());
					for (int obj = 0; obj < objTableCount; obj++)
					{
						int objTablePointer = ByteConverter.ToInt32(file, objTableStart + 4 * obj);
						objTablePointers.Add(objTablePointer);
						Console.WriteLine("Object Table at {0}", objTablePointer.ToString("X"));
					}
				}

				//Motion Table
				int motTableCount = ByteConverter.ToInt32(file, nmldEntries[e].ptr_motTable);
				int motTableCheck = ByteConverter.ToInt32(file, nmldEntries[e].ptr_motTable + 0x04);
				if (motTableCheck != 0 || motTableCount > 1)
				{
					int motTableStart = nmldEntries[e].ptr_motTable + 4;
					Console.WriteLine("Object {0} has {1} motions.", nmldEntries[e].name, motTableCount);
					for (int mot = 0; mot < motTableCount; mot++)
					{
						int motTablePointer = ByteConverter.ToInt32(file, motTableStart + 4 * mot);
						if (motTablePointer != 0 && !motTablePointers.Contains("motion_" + motTablePointer.ToString("X") + ".njm\n"))
							motTablePointers.Add("motion_" + motTablePointer.ToString("X") + ".njm\n");
					}
				}

				//Add NJCM Entries to Entries List.
				for (int mdl = 0; mdl < objTablePointers.Count; mdl++)
				{
					string mdlName = Path.ChangeExtension(e.ToString("D3") + "_" + mdl.ToString("D2") + "_" + nmldEntries[e].name, ".nj");

					int ptr_njcmChunk = ByteConverter.ToInt32(file, objTablePointers[mdl]) + objTablePointers[mdl];
					int ptr_eofChunk = ByteConverter.ToInt32(file, objTablePointers[mdl] + 4) + objTablePointers[mdl];
					int ptr_njtlChunk = ByteConverter.ToInt32(file, objTablePointers[mdl] + 8) + objTablePointers[mdl];

					Console.WriteLine("NJCM file starts at {0} and ends at {1}", ptr_njtlChunk.ToString("X"), ptr_eofChunk.ToString("X"));

					byte[] njcmFile = new byte[ptr_eofChunk - ptr_njtlChunk];
					Array.Copy(file, ptr_njtlChunk, njcmFile, 0, njcmFile.Length);
					Entries.Add(new MLDArchiveEntry(njcmFile, mdlName));

					if (motTablePointers.Count != 0)
					{
						string mdlAction = "";
						foreach (string motion in motTablePointers)
						{
							mdlAction += (motion);
						}
						Entries.Add(new MLDArchiveEntry(Encoding.ASCII.GetBytes(mdlAction), (Path.GetFileNameWithoutExtension(mdlName) + ".action")));
					}

					//Write NMLD Entry output
					Console.WriteLine("Generate NMLD Entry List");
					nmldInfo += mdlName + ", " + nmldEntries[e].pos.ToString() + ", " + nmldEntries[e].rot.ToString() + ", " + nmldEntries[e].scl.ToString() + "\n";
				}
			}

			Entries.Add(new MLDArchiveEntry(Encoding.ASCII.GetBytes(nmldInfo), "nmldInfo.amld"));

			//Scan file for all NMDM Chunks
			Console.WriteLine("Scanning for NMDM Chunks");
			byte[] nmdmByte = new byte[] { 0x4E, 0x4D, 0x44, 0x4D };
			List<int> nmdmAddr = SearchBytePattern(nmdmByte, file);
			//Add NMDM Entries to Entries List.
			for (int anm = 0; anm < nmdmAddr.Count; anm++)
			{
				int nmdmLength = ByteConverter.ToInt32(file, nmdmAddr[anm] + 4) + 8 + nmdmAddr[anm];
				int nmdmEOF = ByteConverter.ToInt32(file, nmdmLength + 4) + 8 + nmdmLength;
				byte[] nmdmFile = new byte[nmdmEOF - nmdmAddr[anm]];
				Array.Copy(file, (nmdmAddr[anm]), nmdmFile, 0, nmdmFile.Length);
				string njmName = ("motion_" + nmdmAddr[anm].ToString("X") + ".njm");
				Console.WriteLine("Adding {0} to Archive Entries", njmName);
				Entries.Add(new MLDArchiveEntry(nmdmFile, njmName));
			}

			int numtex = ByteConverter.ToInt32(file, (int)textablepointer);
			List<string> texnames = new List<string>();
			string texList = "";
			// Extract textures
			Console.WriteLine("Number of textures: {0}, pointer: {1}", numtex, textablepointer.ToString("X"));
			if (numtex > 0)
			{
				int texdataoffset = (int)textablepointer + 4 + numtex * 44;
				Console.WriteLine("Texture offset original: {0}", texdataoffset.ToString("X"));
				// Get through the padding
				if (file[texdataoffset] == 0)
				{
					do
					{
						texdataoffset += 1;
					}
					while (file[texdataoffset] == 0);
				}
				Console.WriteLine("Textures from {0}", texdataoffset.ToString("X"));
				int currenttextureoffset = texdataoffset;
				string ext = "";
				if (ByteConverter.BigEndian == true)
					ext = ".gvr";
				else
					ext = ".pvr";
				for (int i = 0; i < numtex; i++)
				{
					byte[] namestring = new byte[36];
					Array.Copy(file, textablepointer + 4 + i * 44, namestring, 0, 36);
					string entryfn = Encoding.ASCII.GetString(namestring).TrimEnd((char)0);
					int size = ByteConverter.ToInt32(file, (int)textablepointer + 4 + i * 44 + 40);
					Console.WriteLine("Entry {0} name {1} size {2}", i, entryfn, size);
					byte[] texture = new byte[size];
					Array.Copy(file, currenttextureoffset, texture, 0, size);
					Entries.Add(new MLDArchiveEntry(texture, entryfn + ext));
					texnames.Add(entryfn);
					currenttextureoffset += size;
				}

				for (int t = 0; t < numtex; t++)
				{
					texList += t.ToString() + "," + texnames[t] + ".png\n";
				}

				Entries.Add(new MLDArchiveEntry(Encoding.ASCII.GetBytes(texList), "index.txt"));
			}
		}

		static public List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
		{
			List<int> positions = new List<int>();
			int patternLength = pattern.Length;
			int totalLength = bytes.Length;
			byte firstMatchByte = pattern[0];
			for (int i = 0; i < totalLength; i++)
			{
				if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
				{
					byte[] match = new byte[patternLength];
					Array.Copy(bytes, i, match, 0, patternLength);
					if (match.SequenceEqual<byte>(pattern))
					{
						positions.Add(i);
						i += patternLength - 1;
					}
				}
			}
			return positions;
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}
	}
}
