using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using AuroraLib.Compression;
using AuroraLib.Compression.Algorithms;
using AuroraLib.Core.IO;
using SplitTools;
using SAModel;
//using SAModel.SAEditorCommon.ModelConversion;

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

		public class GRND
		{
			// For Reference, the setup for a GRND is as follows:
			// 0x00	- "GRND"
			// 0x04	- Chunk Size (Includes first 16 bytes).
			// 0x08	- Int; null[2]

			// GRND Header begins at 0x10 in a GRND Chunk. The GRND Chunk appears to be made up of a quad-tree to check for collision, followed by the actual
			// triangle/vertex data
			// 0x00	- Pointer to Triangles Chunk
			// 0x04	- Pointer to Quadtree Chunk
			// 0x08	- Float; X Pos 0,0
			// 0x0C	- Float; Z Pos 0,0
			// 0x10	- Short; X quad Number
			// 0x12	- Short; Z quad Number
			// 0x14	- Short; X quad Length
			// 0x16 - Short; Z quad Length
			// 0x18	- Short; Triangle Count
			// 0x1A	- Short; Poly Count

			/*
			 * GRND blocks contain a quad tree for quick lookup and collision detection
			 * It is made up of two chunks, a chunk for the quad tree, and a chunk for the polygons present in the GRND
			 * The each polygon in the polygon chunk is made up of a compressed list of triangle info, and a compressed list of vertices
			 * Since the polygon chunk uses compression to overlap triangle info and vertices, and the triangle indices which define all
			 * of the triangles are not listed, this algorithm uses the quad tree chunk to first identify all used triangle info (indices into the 
			 * triangle info block)
			*/

			public Vertex[] Vertices;
			public List<NJS_MESHSET> Meshes;
			public Vertex Center;
			public Vertex Origin;
			public short XCount;
			public short ZCount;
			public short XLen;
			public short ZLen;

			public NJS_OBJECT ToObject()
			{
				NJS_OBJECT obj = new ();
					
				BasicAttach attach = new(Vertices, Array.Empty<Vertex>(), Meshes, Array.Empty<NJS_MATERIAL>());
				obj.Attach = attach;
				//obj.Attach = obj.Attach.ToChunk();

				return obj;
			}

			public GRND(byte[] file, int address)
			{
				Meshes = new List<NJS_MESHSET>();

				int addr = 16;
				int ptr_triangles = ByteConverter.ToInt32(file, addr) + addr;
				int ptr_quadtree = ByteConverter.ToInt32(file, addr + 4) + addr + 4;
				
				Origin = new Vertex(ByteConverter.ToSingle(file, addr + 8), 0.0f, ByteConverter.ToSingle(file, addr + 0xc));
				XCount = ByteConverter.ToInt16(file, addr + 0x10);   //Might be unsigned?
				ZCount = ByteConverter.ToInt16(file, addr + 0x12);   //Might be unsigned?
				XLen = ByteConverter.ToInt16(file, addr + 0x14);   //Might be unsigned?
				ZLen = ByteConverter.ToInt16(file, addr + 0x16);   //Might be unsigned?
				Center = new Vertex(Origin.X + XCount / 2 * XLen, 0.0f, Origin.Z + (ZCount / 2) * ZLen);
				
				short tri_count = ByteConverter.ToInt16(file, addr + 0x18);
				short quad_count = ByteConverter.ToInt16(file, addr + 0x1a);


				// This section uses the quad tree to detect the position of all used triangle info blocks in each polygon
				List<List<ushort>> unique_triangles = new List<List<ushort>>(tri_count);
				for (int i = 0; i < tri_count; i++) { unique_triangles.Add(new List<ushort>()); }

				
				for (int i = 0; i < quad_count; i++)
				{
					int cur_quad_tri_count = ByteConverter.ToInt32(file, ptr_quadtree + (i * 8));
					int cur_quad_list_offset = ByteConverter.ToInt32(file, ptr_quadtree + (i * 8) + 4) + ptr_quadtree + (i * 8) + 4;
					for (int j = 0; j < cur_quad_tri_count; j++)
					{
						ushort tri_set = ByteConverter.ToUInt16(file, cur_quad_list_offset + j * 4);
						ushort tri_ind = ByteConverter.ToUInt16(file, cur_quad_list_offset + j * 4 + 2);
						if (!unique_triangles[tri_set].Contains(tri_ind)) {
							unique_triangles[tri_set].Add(tri_ind);
						}
					}
				}

				// This section uses the unique triangles to create triangles from the detected triangle info indices for each polygon
				List <Vertex> vert_list = new List <Vertex>(0);
				for (int i = 0;i < tri_count;i++)
				{
					List<Triangle> tris = new List<Triangle>();
					List<Vertex> verts = new List<Vertex>();
					int triInfo_offset = i * 0x18 + ptr_triangles;
					int tri_offset = ByteConverter.ToInt32(file, triInfo_offset + 0x10) + triInfo_offset + 0x10;
					int vert_offset = ByteConverter.ToInt32(file, triInfo_offset + 0xc) + triInfo_offset + 0xc;
					foreach (int j in unique_triangles[i])
					{
						int v1_ind = (int)ByteConverter.ToUInt16(file, tri_offset + j * 4);
						int v2_ind = (int) ByteConverter.ToUInt16(file, tri_offset + j * 4 + 4);
						int v3_ind = (int)ByteConverter.ToUInt16(file, tri_offset + j * 4 + 8);
						bool reversed = ByteConverter.ToInt16(file, tri_offset + j * 4 + 0xa) < 0;

						if (reversed) { tris.Add(new Triangle((ushort)(vert_list.Count + 2), (ushort) (vert_list.Count + 1), (ushort) vert_list.Count)); }
						else { tris.Add(new Triangle((ushort)vert_list.Count, (ushort)(vert_list.Count + 1), (ushort)(vert_list.Count + 2))); }

						vert_list.Add(new Vertex(file, vert_offset + v1_ind * 4));
						vert_list.Add(new Vertex(file, vert_offset + v2_ind * 4));
						vert_list.Add(new Vertex(file, vert_offset + v3_ind * 4));
					}
					
					if (tris.Count > 0)
					{
						// Create meshset from the current polygon
						Meshes.Add(new NJS_MESHSET(tris.ToArray(), false, false, false));
					}
				}

				// Convert the vertex list to an array and store it
				Vertices = vert_list.ToArray();
			}
		}

		public class GOBJ
		{
			// For Reference, the setup for a GOBJ is as follows:
			// 0x00	- "GOBJ"
			// 0x04	- Chunk Size (Includes first 16 bytes),
			// 0x08	- Int; null[2]

			// GOBJ "Header" begins at 0x10 in a GOBJ Chunk.
			// 0x00	- NJS_OBJECT
			// NJS_OBJECT should have a child.
			// Said child node will have a ChunkAttach/NJS_MODEL_CNK, the child pointer is set but it also seems to always follow the first NJS_OBJECT.
			// As stated above, all pointers are relative to the location of the pointer EXCEPT for the ChunkAttach Pointer for the child.
			// It has a 1 which does not correspond to the ChunkAttach/NJS_MODEL_CNK's location.
			// Its location will be immediately after the child NJS_OBJECT.
			// It's also in a flipped order. The Center/Radius comes first, then the VertexChunk pointer, and the PolyChunk pointer at the end.

			public NJS_OBJECT Object;
			public BoundingSphere Bounds;
			public NJS_OBJECT GroundObject;

			public GOBJ(byte[] file, int address)
			{                   
				int addr = 16;
				GroundObject = get_GOBJ_node(file, addr);

			}

			private NJS_OBJECT get_GOBJ_node(byte[] file, int address)
			{
				NJS_OBJECT obj = new NJS_OBJECT();
				int data_ptr = ByteConverter.ToInt32(file, address);

				obj.Position = new Vertex(file, address + 0x8);
				obj.Rotation = new Rotation(file, address + 0x14);
				obj.Scale = new Vertex(file, address + 0x20);

				int leftptr = ByteConverter.ToInt32(file, address + 0x2c);
				if (leftptr > 0)
				{
					leftptr += 0x2c + address;
					obj.AddChild(get_GOBJ_node(file, leftptr));
				}

				int rightptr = ByteConverter.ToInt32(file, address + 0x30);
				if (rightptr > 0)
				{
					rightptr += 0x2c + address;
					obj.AddChild(get_GOBJ_node(file, rightptr));
				}

				if (data_ptr != 0)
				{
					data_ptr += address;
				ChunkAttach attach = new ChunkAttach(true, true);
					attach.Bounds = new BoundingSphere(file, data_ptr);
					data_ptr += 0x10;

					int vertptr = ByteConverter.ToInt32(file, data_ptr) + data_ptr;
					int polyptr = data_ptr + 76;

					//The geometry structure may not be in the correct format, but leaving this here for now
				VertexChunk vertexchunk = new VertexChunk(file, vertptr);
				PolyChunk polychunk = PolyChunk.Load(file, polyptr);

				attach.Vertex.Add(vertexchunk);
				attach.Poly.Add(polychunk);

					obj.Attach = attach;
				}
				return obj;
			}
		}

		public string Name;
		public GroundType Type;
		public byte[] File;
		public NJS_OBJECT ConvertedObject;

		private GRND GRNDObj;
		private GOBJ GOBJChunk;

		public nmldGround(byte[] file, int address, string name)
		{
			// These chunks are actually condensed chunk models.
			// GOBJ has actual NJS_OBJECTs and a "flipped" ChunkAttach/NJS_MODEL_CNK.
			// GRND does not, but does seem to have possible grid set bounds in the custom header.
			// Both use pointers that are relative to the position of the pointer in the file. 
			// Switch Case includes comments on the structures.

			string magic = Encoding.ASCII.GetString(file, address, 4);

			int filesize = ByteConverter.ToInt32(file, address + 4);

			File = new byte[filesize];
			Array.Copy(file, address, File, 0, filesize);

			Name = name;

			switch (magic)
			{
				case "GRND":
					Type = GroundType.Ground;
					GRNDObj = new GRND(File, 0);
					break;
				case "GOBJ":

					Type = GroundType.GroundObject;
					//GOBJChunk = new GOBJ(File, 0);
					break;
				default:
					Console.WriteLine("Unknown Ground Format Found: %s", magic);
					Type = GroundType.Unknown;
					break;
			}

			// Currently non-function due to weird poly format.
			// Can uncomment once conversion is fixed.
			if (Type == GroundType.Ground)
			{
				ConvertedObject = GRNDObj.ToObject();
			}
			/*
			if (Type == GroundType.GroundObject)
			{
				ConvertedObject = GOBJChunk.GroundObject;
			}
			*/
		}
	}

	public class nmldMotion
	{
		public enum MotionType
		{
			Node = 0,
			Shape = 1,
			Camera = 2,
			Unknown
		}

		public string Name;
		public MotionType Type;
		public byte[] File;

		public nmldMotion(byte[] file, int address, string name, string idx)
		{
			string magic = Encoding.ASCII.GetString(file, address, 4);
			string suffix = "";

			switch (magic)
			{
				case "NMDM":
					Type = MotionType.Node;
					suffix = "_motion";
					break;
				case "NSSM":
					Type = MotionType.Shape;
					suffix = "_shape";
					break;
				case "NCAM":
					Type = MotionType.Camera;
					suffix = "_camera";
					break;
				default:
					Console.WriteLine("Unidentified Motion Type: %s", magic);
					Type = MotionType.Unknown;
					suffix = "_unknown";
					break;
			}

			Name = name + suffix + idx;

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
		public int TblID { get; set; } = 0;
		public List<int> GroundLinks { get; set; } = new();
		public List<int> ParamList2 { get; set; } = new();
		public List<int> FunctionParameters { get; set; } = new();
		public List<nmldObject> Objects { get; set; } = new();
		public List<nmldMotion> Motions { get; set; } = new();
		public List<nmldGround> Grounds { get; set; } = new();
		public nmldTextureList Texlist { get; set; } = new();
		public string Fxn { get; set; } = string.Empty;
		public Vertex Position { get; set; } = new();
		public Vertex Rotation { get; set; } = new();
		public Vertex Scale { get; set; } = new();

		private string GetNameWithIndex()
		{
			string bitID = "";
			if (Fxn == "eventhook")
			{
				bitID = FunctionParameters[FunctionParameters.Count - 1].ToString();
			}
			return Index.ToString("D3") + "_" + Fxn + bitID;
		}

		private string GetNameAndIndex(int index)
		{
			string bitID = "";
			if (Fxn == "eventhook")
			{
				bitID = FunctionParameters[FunctionParameters.Count - 1].ToString();
			}
			return Index.ToString("D3") + "_" + Fxn + bitID + "_" + index.ToString("D2");
		}

		private void GetParamList(byte[] file, int offset, List<int> target_var)
		{
			int count = ByteConverter.ToInt32(file, offset);

			for (int i = 0; i < count; i ++)
			{
				target_var.Add(ByteConverter.ToInt32(file, offset + i * 4 + 4));
			}
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

			List<int> addrs = new List<int>();

			for (int i = 0; i < count; i++)
			{
				int address = ByteConverter.ToInt32(file, offset + (4 * (i + 1)));

				if (address != 0)
				{
					if (!addrs.Contains(address))
						addrs.Add(address);
				}
			}

			int idx = 0;
			foreach (int addr in addrs)
			{
				Motions.Add(new nmldMotion(file, addr, GetNameWithIndex(), idx.ToString()));
				idx++;
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
					Index.ToString("D3") + "_" + Fxn + "_" + i.ToString("D2") + 
					", " + Position.ToString() + 
					", " + Rotation.ToString() + 
					", " + Scale.ToString());
			}

			return sb.ToString();
		}

		public nmldEntry(int offset, byte[] file)
		{
			Index = ByteConverter.ToInt32(file, offset);
			TblID = ByteConverter.ToInt32(file, offset + 4);	

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
			Fxn = Encoding.ASCII.GetString(namechunk);

			int ptrGroundLinks = ByteConverter.ToInt32(file, offset + 0x8);
			GetParamList(file, ptrGroundLinks, GroundLinks);
			int ptrParamList2 = ByteConverter.ToInt32(file, offset + 0xc);
			GetParamList(file, ptrParamList2, ParamList2);
			int ptrFunctionParameters = ByteConverter.ToInt32(file, offset + 0x10);
			GetParamList(file, ptrFunctionParameters, FunctionParameters);

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
			int ptr_fxnparams	= ByteConverter.ToInt32(file, 0x08);
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
					ModelFile mfile = new ModelFile(ModelFormat.Basic, ground.ConvertedObject, null, null);
					switch (ground.Type)
					{
						case nmldGround.GroundType.Ground:
							// Entries.Add(new MLDArchiveEntry(ground.File, ground.Name + ".grnd"));
							mfile.SaveToFile(Path.Combine(directory, ground.Name + ".grnd.sa2mdl"));
							break;
						case nmldGround.GroundType.GroundObject:
							//Entries.Add(new MLDArchiveEntry(ground.File, ground.Name + ".gobj"));
							//mfile.SaveToFile(Path.Combine(directory, ground.Name + ".gobj.sa2mdl"));
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
						case nmldMotion.MotionType.Camera:
							Entries.Add(new MLDArchiveEntry(motion.File, motion.Name + ".njc"));
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
			string aklzcheck = Encoding.ASCII.GetString(file, 0, 4);
			if (aklzcheck == "AKLZ")
				ByteConverter.BigEndian = true;
			else
				ByteConverter.BigEndian = SplitTools.HelperFunctions.CheckBigEndianInt32(file, 0xC);

			nmldArchiveFile archive;

			if (ByteConverter.BigEndian)
			{
				Console.WriteLine("Skies of Arcadia: Legends MLD File");

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
						Entries.Add(new MLDArchiveEntry(dfile, ("..\\" + filename + "_dec.mld")));
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
