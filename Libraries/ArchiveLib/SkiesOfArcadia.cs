using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using AuroraLib.Compression;
using AuroraLib.Compression.Algorithms;
using AuroraLib.Core.IO;
using SplitTools;
using SAModel;
using System.Net;

// Skies of Arcadia MLD archives.
namespace ArchiveLib
{
	public class nmldObject
	{
		public string Name;
		public byte[] File;

		public nmldObject(byte[] file, int offset, string name)
		{
			int ptrNJCM = ByteConverter.ToInt32(file, offset);
			uint chunksize = ByteConverter.ToUInt32(file, offset + 4) - 16;
			int ptrNJTL = ByteConverter.ToInt32(file, offset + 8);
			uint unknown = ByteConverter.ToUInt32(file, offset + 12);

			if (unknown != 0)
				Console.WriteLine("Unknown Pointer in Object is populated: %s", unknown.ToString());

			int start = (ptrNJTL != 0) ? ptrNJTL : ptrNJCM;

			if (start == 0)
			{
				Console.WriteLine("Objects have no data pointers.");
				return;
			}

			File = new byte[chunksize];
			Array.Copy(file, start + offset, File, 0, chunksize);

			Name = name;
		}
	}

	public class nmldGround
	{
		public enum GroundType
		{
			Ground = 0,
			GroundObject = 1,
			Unknown
		}

		public string Name;
		public GroundType Type;
		public byte[] File;

		public nmldGround(byte[] file, int address, string name)
		{
			string magic = Encoding.ASCII.GetString(file, address, 4);

			switch (magic)
			{
				case "GRND":
					Type = GroundType.Ground;
					break;
				case "GOBJ":
					Type = GroundType.GroundObject;
					break;
				default:
					Console.WriteLine("Unknown Ground Format Found: %s", magic);
					Type = GroundType.Unknown;
					break;
			}

			int filesize = ByteConverter.ToInt32(file, address + 4) + 8;

			File = new byte[filesize];
			Array.Copy(file, address, File, 0, filesize);

			Name = name;
		}
	}

	public class nmldMotion
	{
		public enum MotionType
		{
			Node = 0,
			Shape = 1,
			Unknown
		}

		public string Name;
		public MotionType Type;
		public byte[] File;

		public nmldMotion(byte[] file, int address, string name, string idx)
		{
			string magic = Encoding.ASCII.GetString(file, address, 4);

			switch (magic)
			{
				case "NMDM":
					Type = MotionType.Node;
					Name = name + "_motion" + idx;
					break;
				case "NSSM":
					Type = MotionType.Shape;
					Name = name + "_shape" + idx;
					break;
				default:
					Console.WriteLine("Unidentified Motion Type: %s", magic);
					Type = MotionType.Unknown;
					break;
			}

			int njmsize = ByteConverter.ToInt32(file, address + 4) + 8;
			int pofsize = ByteConverter.ToInt32(file, address + njmsize + 4) + 8;

			File = new byte[njmsize + pofsize];
			Array.Copy(file, address, File, 0, njmsize + pofsize);
		}
	}

	public class nmldTextureList
	{
		public string Name;
		public NJS_TEXLIST TexList;

		public nmldTextureList()
		{
			Name = string.Empty;
			TexList = new NJS_TEXLIST();
		}

		public nmldTextureList(byte[] file, int address, string name)
		{
			TexList = NJS_TEXLIST.Load(file, address, 0);

			Name = name + ".tls";
		}
	}

	public class nmldEntry
	{
		public int Index { get; set; } = 0;
		public List<nmldObject> Objects { get; set; } = new();
		public List<nmldMotion> Motions { get; set; } = new();
		public List<nmldGround> Grounds { get; set; } = new();
		public nmldTextureList Texlist { get; set; } = new();
		public string Name { get; set; } = string.Empty;
		public Vertex Position { get; set; } = new();
		public Vertex Rotation { get; set; } = new();
		public Vertex Scale { get; set; } = new();

		private string GetNameWithIndex()
		{
			return Index.ToString("D3") + "_" + Name;
		}

		private string GetNameAndIndex(int index)
		{
			return Index.ToString("D3") + "_" + Name + "_" + index.ToString("D2");
		}

		private void GetObjects(byte[] file, int offset)
		{
			int count = ByteConverter.ToInt32(file, offset);

			for (int i = 0; i < count; i++)
			{
				int address = ByteConverter.ToInt32(file, offset + (4 * (i + 1)));
				
				if (address != 0)
				{
					Objects.Add(new nmldObject(file, address, GetNameAndIndex(i)));
				}
			}
		}

		private void GetMotions(byte[] file, int offset)
		{
			int count = ByteConverter.ToInt32(file, offset);

			for (int i = 0; i < count; i++)
			{
				int address = ByteConverter.ToInt32(file, offset + (4 * (i + 1)));

				if (address != 0)
				{
					Motions.Add(new nmldMotion(file, address, GetNameWithIndex(), i.ToString()));
				}
			}
		}

		private void GetGrounds(byte[] file, int offset)
		{
			int count = ByteConverter.ToInt32(file, offset);

			for (int i = 0; i < count; i++)
			{
				int address = ByteConverter.ToInt32(file, offset + (4 * (i + 1)));

				if (address != 0)
				{
					Grounds.Add(new nmldGround(file, address, GetNameAndIndex(i)));
				}
			}
		}

		private void GetTextures(byte[] file, int offset)
		{
			Texlist = new nmldTextureList(file, offset, GetNameWithIndex());
		}

		public string WriteEntryInfo()
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < Objects.Count; i++)
			{
				sb.AppendLine(
					Index.ToString("D3") + "_" + i.ToString("D2") + "_" + Name + ", " + 
					Position.ToString() + ", " + Rotation.ToString() + ", " + Scale.ToString());
			}

			return sb.ToString();
		}

		public nmldEntry(int offset, byte[] file)
		{
			Index = ByteConverter.ToInt32(file, offset);

			// Get Entry Name
			int namesize = 0;
			for (int s = 0; s < 32; s++)
			{
				if (file[offset + 0x24 + s] != 0)
					namesize++;
				else
					break;
			}
			byte[] namechunk = new byte[namesize];
			Array.Copy(file, offset + 0x24, namechunk, 0, namesize);
			Name = Encoding.ASCII.GetString(namechunk);

			Position	= new Vertex(ByteConverter.ToSingle(file, offset + 0x44), ByteConverter.ToSingle(file, offset + 0x48), ByteConverter.ToSingle(file, offset + 0x4C));
			Rotation	= new Vertex(ByteConverter.ToSingle(file, offset + 0x50), ByteConverter.ToSingle(file, offset + 0x54), ByteConverter.ToSingle(file, offset + 0x58));
			Scale		= new Vertex(ByteConverter.ToSingle(file, offset + 0x5C), ByteConverter.ToSingle(file, offset + 0x60), ByteConverter.ToSingle(file, offset + 0x64));

			// Get Entry Objects
			int ptrObjects = ByteConverter.ToInt32(file, offset + 0x14);
			if (ByteConverter.ToInt32(file, ptrObjects + 4) != 0)
				GetObjects(file, ptrObjects);

			// Get Entry Motions
			int ptrMotions = ByteConverter.ToInt32(file, offset + 0x1C);
			if (ByteConverter.ToInt32(file, ptrMotions) != 0)
				GetMotions(file, ptrMotions);

			// Get Entry Grounds
			int ptrGrounds = ByteConverter.ToInt32(file, offset + 0x18);
			if (ByteConverter.ToInt32(file, ptrGrounds + 4) != 0)
				GetGrounds(file, ptrGrounds);

			// Get Entry Textures
			int ptrTextures = ByteConverter.ToInt32(file, offset + 0x20);
			if (ByteConverter.ToInt32(file, ptrTextures + 4) != 0)
				GetTextures(file, ptrTextures);
		}
	}

	public class nmldArchiveFile
	{
		public string Name { get; set; } = string.Empty;
		public List<nmldEntry> Entries { get; set; } = new();
		public PuyoFile TextureFile { get; set; }

		private void GetTextureArchive(byte[] file, int offset)
		{
			Console.WriteLine("Getting Textures...");
			if (ByteConverter.BigEndian == true)
				TextureFile = new PuyoFile(PuyoArchiveType.GVMFile);
			else
				TextureFile = new PuyoFile();

			int numtex = ByteConverter.ToInt32(file, offset);
			int texnamearray = offset + 4;
			Dictionary<string, int> texnames = new();

			if (numtex > 0)
			{
				Console.WriteLine("Textures Found! Creating Archive.");
				for (int i = 0; i < numtex; i++)
				{
					int element = texnamearray + (i * 44);
					texnames.Add(file.GetCString(element, Encoding.UTF8), ByteConverter.ToInt32(file, element + 40));
				}

				// Texture Embeds have an unspecified spacing between the end of the names and the start of the texture data.
				// So we do this to get through the padding
				int texdataoffset = offset + 4 + numtex * 44;
				if (file[texdataoffset] == 0)
				{
					do
					{
						texdataoffset += 1;
					}
					while (file[texdataoffset] == 0);
				}
				int texdataptr = texdataoffset;

				bool isBig = ByteConverter.BigEndian;

				foreach (KeyValuePair<string, int> tex in texnames)
				{
					ByteConverter.BigEndian = false;
					int texdataptr2 = texdataptr;
					string magic = Encoding.ASCII.GetString(file, texdataptr2, 4);
					int size = 0;

					switch (magic)
					{
						case "GBIX":
						case "GCIX":
							size += ByteConverter.ToInt32(file, texdataptr2 + 4) + 8;
							texdataptr2 += 16;
							break;
					}

					size += ByteConverter.ToInt32(file, texdataptr2 + 4) + 8;
					byte[] texture = new byte[size];
					Array.Copy(file, texdataptr, texture, 0, size);

					switch (TextureFile.Type)
					{
						case PuyoArchiveType.PVMFile:
							TextureFile.Entries.Add(new PVMEntry(texture, tex.Key));
							break;
						case PuyoArchiveType.GVMFile:
							TextureFile.Entries.Add(new GVMEntry(texture, tex.Key));
							break;
					}

					texdataptr += tex.Value;
				}

				ByteConverter.BigEndian = isBig;
			}
		}

		private void GetEntries(byte[] file, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Entries.Add(new nmldEntry(offset + (i * 104), file));
			}
		}

		public nmldArchiveFile()
		{
			Name = string.Empty;
			Entries = new List<nmldEntry>();
			TextureFile = new();
		}

		public nmldArchiveFile(byte[] file, string name)
		{
			Name = name;

			int nmldCount		= ByteConverter.ToInt32(file, 0);
			int ptr_nmldTable	= ByteConverter.ToInt32(file, 0x04);
			int eof_nmldTable	= ByteConverter.ToInt32(file, 0x08);
			int realdatapointer = ByteConverter.ToInt32(file, 0x0C);
			int textablepointer = ByteConverter.ToInt32(file, 0x10);
			Console.WriteLine("Number of NMLD entries: {0}, NMLD data starts at {1}, real data starts at {2}", nmldCount, ptr_nmldTable.ToString("X"), realdatapointer.ToString("X"));

			// Go ahead and extract the texture archive.
			GetTextureArchive(file, textablepointer);

			// Collect Entries and their contents
			GetEntries(file, ptr_nmldTable, nmldCount);
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

		private void ExtractEntries(nmldArchiveFile archive, string directory)
		{
			StringBuilder sb = new StringBuilder();

			// Add Entries
			foreach (nmldEntry entry in archive.Entries)
			{
				// Add Objects
				foreach (nmldObject model in entry.Objects)
				{
					Entries.Add(new MLDArchiveEntry(model.File, model.Name + ".nj"));
				}

				// Add Ground/Ground Object Files
				foreach (nmldGround ground in entry.Grounds)
				{
					switch (ground.Type)
					{
						case nmldGround.GroundType.Ground:
							Entries.Add(new MLDArchiveEntry(ground.File, ground.Name + ".grnd"));
							break;
						case nmldGround.GroundType.GroundObject:
							Entries.Add(new MLDArchiveEntry(ground.File, ground.Name + ".gobj"));
							break;
						case nmldGround.GroundType.Unknown:
							Entries.Add(new MLDArchiveEntry(ground.File, ground.Name + ".gunk"));
							break;
					}
				}

				// Add Motions
				foreach (nmldMotion motion in entry.Motions)
				{
					switch (motion.Type)
					{
						case nmldMotion.MotionType.Node:
							Entries.Add(new MLDArchiveEntry(motion.File, motion.Name + ".njm"));
							break;
						case nmldMotion.MotionType.Shape:
							Entries.Add(new MLDArchiveEntry(motion.File, motion.Name + ".njs"));
							break;
						case nmldMotion.MotionType.Unknown:
							Entries.Add(new MLDArchiveEntry(motion.File, motion.Name + ".num"));
							break;
					}
				}

				// Save Texlist
				if (entry.Texlist.TexList.NumTextures > 0)
				{
					if (!Directory.Exists(directory))
						Directory.CreateDirectory(directory);

					entry.Texlist.TexList.Save(Path.Combine(directory, entry.Texlist.Name));
				}

				sb.Append(entry.WriteEntryInfo());
			}

			// Add Info File
			Entries.Add(new MLDArchiveEntry(Encoding.ASCII.GetBytes(sb.ToString()), "FileInfo.amld"));

			// Add Texture Archive
			if (archive.TextureFile != new PuyoFile())
			{
				string ext = "";
				switch (archive.TextureFile.Type)
				{
					case PuyoArchiveType.PVMFile:
						ext = ".pvm";
						break;
					case PuyoArchiveType.GVMFile:
						ext = ".gvm";
						break;
				}
				Entries.Add(new MLDArchiveEntry(archive.TextureFile.GetBytes(), archive.Name + ext));
			}
		}

		public MLDArchive(string filepath, byte[] file)
		{
			string directory = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath));
			string filename = Path.GetFileNameWithoutExtension(filepath);
			ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0xC);

			nmldArchiveFile archive;

			if (ByteConverter.BigEndian)
			{
				Console.WriteLine("Skies of Arcadia: Legends MLD File");
				string aklzcheck = Encoding.ASCII.GetString(file, 0, 4);

				if (aklzcheck == "AKLZ")
				{
					Console.WriteLine("MLD Archive is Compressed. Decompressing...");
					byte[] dfile = new byte[0];

					// Decompress File Here
					using (Stream stream = new MemoryStream(file))
					{
						using (MemoryPoolStream pool = new AKLZ().Decompress(stream))
						{
							dfile = new byte[pool.ToArray().Length];
							Array.Copy(pool.ToArray(), dfile, pool.ToArray().Length);
						}
					}

					if (dfile.Length > 0)
					{
						Console.WriteLine("File Decompressed, saving and reading decompressed archive.");
						Entries.Add(new MLDArchiveEntry(dfile, (filename + "_dec.mld")));
						archive = new nmldArchiveFile(dfile, filename);
					}
					else
					{
						Console.WriteLine("Decompression Failed.");
						archive = new();
					}
				}
				else
					archive = new nmldArchiveFile(file, filename);
			}
			else
			{
				Console.WriteLine("Skies of Arcadia MLD File");
				archive = new nmldArchiveFile(file, filename);
			}

			if (archive != new nmldArchiveFile())
			{
				ExtractEntries(archive, directory);
			}
			else
				Console.WriteLine("Unable to read archive.");
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}
	}
}
