using System;

namespace SAModel.DataToolbox
{
	public static partial class ObjScan
	{
		static bool CheckForModelData(NJS_OBJECT obj)
		{
			if (obj.Attach != null) return true;
			if (obj.Sibling != null && obj.Sibling.Attach != null) return true;
			if (obj.Children != null && obj.Children.Count > 0)
			{
				foreach (NJS_OBJECT ch in obj.Children)
				{
					bool checc = CheckForModelData(ch);
					if (checc) return true;
				}
			}
			return false;
		}

		static bool CheckModel(uint address, int numhierarchy, ModelFormat modelfmt, bool landtable = false)
		{
			//Console.WriteLine("Check: {0}", address.ToString("X"));
			ByteConverter.BigEndian = BigEndian;
			if (address > (uint)datafile.Length - 20) return false;
			int flags = 0;
			uint vertlist = 0;
			uint polylist = 0;
			uint chunkend = 0;
			float radius = 0;
			int radiusChunk = 0;
			uint attach = 0;
			uint child = 0;
			uint sibling = 0;
			uint vertices = 0;
			uint normals = 0;
			uint vert_count = 0;
			uint meshlists = 0;
			short mesh_count = 0;
			short mat_count = 0;
			uint opaquepoly = 0;
			short opaquecount = 0;
			uint alphapoly = 0;
			short alphacount = 0;
			float center_x = 0;
			float center_y = 0;
			float center_z = 0;
			Vertex pos;
			Vertex scl;
			switch (modelfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					flags = ByteConverter.ToInt32(datafile, (int)address);
					if (flags > 0x3FFF || flags < 0) return false;
					attach = ByteConverter.ToUInt32(datafile, (int)address + 4);
					pos = new Vertex(datafile, (int)address + 8);
					scl = new Vertex(datafile, (int)address + 0x20);
					child = ByteConverter.ToUInt32(datafile, (int)address + 0x2C);
					sibling = ByteConverter.ToUInt32(datafile, (int)address + 0x30);
					if (landtable && (child != 0 || sibling != 0)) return false;
					if (child > address + ImageBase) return false;
					if (sibling > address + ImageBase) return false;
					if (child != 0 && child < ImageBase) return false;
					if (child > datafile.Length - 52 + ImageBase) return false;
					if (sibling > datafile.Length - 52 + ImageBase) return false;
					if (sibling != 0 && sibling < ImageBase) return false;
					if (SimpleSearch)
					{
						if (scl.X == 1.0f && scl.Y == 1.0f && scl.Z == 1.0f)
						{
							Console.WriteLine("{0} model at {1}", modelfmt.ToString(), address.ToString("X"));
							return true;
						}
					}
					if (attach != 0)
					{
						if (attach < ImageBase) return false;
						if (attach > datafile.Length - 51 + ImageBase) return false;
						vertices = ByteConverter.ToUInt32(datafile, ((int)(attach - ImageBase)));
						if (vertices < ImageBase) return false;
						if (vertices > datafile.Length - 51 + ImageBase) return false;
						normals = ByteConverter.ToUInt32(datafile, ((int)(attach - ImageBase) + 4));
						if (normals != 0 && normals < ImageBase) return false;
						if (normals > datafile.Length - 51 + ImageBase) return false;
						vert_count = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 8);
						if (vert_count > 2048 || vert_count == 0) return false;
						meshlists = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 0xC);
						if (meshlists != 0 && meshlists < ImageBase) return false;
						if (meshlists > datafile.Length - 51 + ImageBase) return false;
						mesh_count = ByteConverter.ToInt16(datafile, (int)(attach - ImageBase) + 0x14);
						if (mesh_count > 2048 || mesh_count < 0) return false;
						mat_count = ByteConverter.ToInt16(datafile, (int)(attach - ImageBase) + 0x16);
						if (mat_count > 2048 || mat_count < 0) return false;
						center_x = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x18);
						center_y = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x1C);
						center_z = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x20);
						radius = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x24);
						if (center_x < -100000.0f || center_x > 100000.0f) return false;
						if (center_y < -100000.0f || center_y > 100000.0f) return false;
						if (center_z < -100000.0f || center_z > 100000.0f) return false;
						if (radius < 0.0f || radius > 100000.0f) return false;
					}
					if (pos.X < -100000 || pos.X > 100000) return false;
					if (pos.Y < -100000 || pos.Y > 100000) return false;
					if (pos.Z < -100000 || pos.Z > 100000) return false;
					if (scl.X <= 0 || scl.X > 10000) return false;
					if (scl.Y <= 0 || scl.Y > 10000) return false;
					if (scl.Z <= 0 || scl.Z > 10000) return false;
					if (child == address + ImageBase || (attach != 0 && child == attach)) return false;
					if (sibling == address + ImageBase || (attach != 0 && sibling == attach)) return false;
					if (child != 0 && child == sibling) return false;
					if (numhierarchy != -1 && child != 0)
					{
						if (numhierarchy < 3)
						{
							numhierarchy++;
							return CheckModel(child - ImageBase, numhierarchy, modelfmt);
						}
						else
							return CheckModel(child - ImageBase, -1, modelfmt);
					}
					if (numhierarchy != -1 && sibling != 0)
					{
						if (numhierarchy < 3)
						{
							numhierarchy++;
							return CheckModel(sibling - ImageBase, numhierarchy, modelfmt);
						}
						else
							return CheckModel(sibling - ImageBase, -1, modelfmt);
					}
					if (attach == 0 && flags == 0) return false;
					//Console.WriteLine("Attach pointer {0}, Vertices count {1}, Mesh count {2}, Center {3} {4} {5}, Radius {6} at {7}", attach.ToString("X"), vert_count, mesh_count, center_x, center_y, center_z, radius, address.ToString("X"));
					break;
				case ModelFormat.Chunk:
					if ((int)address > datafile.Length - 20) return false;
					flags = ByteConverter.ToInt32(datafile, (int)address);
					if (flags > 0x3FFF || flags < 0) return false;
					attach = ByteConverter.ToUInt32(datafile, (int)address + 4);
					if (attach != 0)
					{
						if (attach < ImageBase) return false;
						if (attach > datafile.Length - 51 + ImageBase) return false;
						chunkend = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) - 4);
						if (vertlist != 0 && chunkend != 0xFF) return false;
						vertlist = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase));
						if (vertlist > datafile.Length - 51 + ImageBase) return false;
						if (vertlist != 0 && vertlist < ImageBase) return false;
						polylist = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 4);
						if (polylist != 0 && polylist < ImageBase) return false;
						if (polylist > datafile.Length - 51 + ImageBase) return false;
						radiusChunk = ByteConverter.ToInt32(datafile, (int)(attach - ImageBase) + 0x14);
						if (radiusChunk < 0) return false;

					}
					pos = new Vertex(datafile, (int)address + 8);
					if (pos.X < -100000 || pos.X > 100000) return false;
					if (pos.Y < -100000 || pos.Y > 100000) return false;
					if (pos.Z < -100000 || pos.Z > 100000) return false;
					scl = new Vertex(datafile, (int)address + 0x20);
					if (scl.X <= 0 || scl.X > 10000) return false;
					if (scl.Y <= 0 || scl.Y > 10000) return false;
					if (scl.Z <= 0 || scl.Z > 10000) return false;
					child = ByteConverter.ToUInt32(datafile, (int)address + 0x2C);
					sibling = ByteConverter.ToUInt32(datafile, (int)address + 0x30);
					if (child > address + ImageBase) return false;
					if (sibling > address + ImageBase) return false;
					if (child > datafile.Length - 52 + ImageBase) return false;
					if (sibling > datafile.Length - 52 + ImageBase) return false;
					if (child != 0 && child < ImageBase) return false;
					if (sibling != 0 && sibling < ImageBase) return false;
					if (numhierarchy != 0 && child != 0 && !CheckModel(child - ImageBase, -1, modelfmt)) return false;
					if (numhierarchy != 0 && sibling != 0 && !CheckModel(sibling - ImageBase, -1, modelfmt)) return false;
					if (vertlist == 0 && child == 0 && sibling == 0) return false;
					if (attach == 0 && flags == 0) return false;
					if (attach == 0 && child == 0 && sibling == 0) return false;
					if (child == address + ImageBase || child == attach) return false;
					if (sibling == address + ImageBase || sibling == attach) return false;
					if (child != 0 && child == sibling) return false;
					if (numhierarchy != -1 && child != 0)
					{
						if (numhierarchy < 3)
						{
							numhierarchy++;
							return CheckModel(child - ImageBase, numhierarchy, modelfmt);
						}
						else
							return CheckModel(child - ImageBase, -1, modelfmt);
					}
					if (numhierarchy != -1 && sibling != 0)
					{
						if (numhierarchy < 3)
						{
							numhierarchy++;
							return CheckModel(sibling - ImageBase, numhierarchy, modelfmt);
						}
						else
							return CheckModel(sibling - ImageBase, -1, modelfmt);
					}
					if (attach == 0 && flags == 0) return false;
					break;
				case ModelFormat.GC:
					if (address <= 0 || address > datafile.Length - 20) return false;
					flags = ByteConverter.ToInt32(datafile, (int)address);
					if (flags > 0x3FFF || flags < 0) return false;
					attach = ByteConverter.ToUInt32(datafile, (int)address + 4);
					if (attach != 0)
					{
						if (attach < ImageBase) return false;
						if (attach > datafile.Length - 51 + ImageBase) return false;
						vertlist = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase));
						if (vertlist > datafile.Length - 51 + ImageBase) return false;
						if (vertlist < ImageBase) return false;
						opaquepoly = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 8);
						if (opaquepoly != 0 && opaquepoly < ImageBase) return false;
						if (opaquepoly > datafile.Length - 51 + ImageBase) return false;
						alphapoly = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 0xC);
						if (alphapoly != 0 && alphapoly < ImageBase) return false;
						if (alphapoly > datafile.Length - 51 + ImageBase) return false;
						opaquecount = ByteConverter.ToInt16(datafile, (int)(attach - ImageBase) + 0x10);
						if (opaquepoly != 0 && opaquecount < 0) return false;
						if (opaquepoly == 0 && opaquecount > 0) return false;
						alphacount = ByteConverter.ToInt16(datafile, (int)(attach - ImageBase) + 0x12);
						if (alphapoly != 0 && alphacount < 0) return false;
						if (alphapoly == 0 && alphacount > 0) return false;
						radius = ByteConverter.ToInt32(datafile, (int)(attach - ImageBase) + 0x20);
						if (radius < 0) return false;
					}
					pos = new Vertex(datafile, (int)address + 8);
					if (pos.X < -100000 || pos.X > 100000) return false;
					if (pos.Y < -100000 || pos.Y > 100000) return false;
					if (pos.Z < -100000 || pos.Z > 100000) return false;
					scl = new Vertex(datafile, (int)address + 0x20);
					if (scl.X <= 0 || scl.X > 10000) return false;
					if (scl.Y <= 0 || scl.Y > 10000) return false;
					if (scl.Z <= 0 || scl.Z > 10000) return false;
					child = ByteConverter.ToUInt32(datafile, (int)address + 0x2C);
					sibling = ByteConverter.ToUInt32(datafile, (int)address + 0x30);
					if (child > (int)address + ImageBase) return false;
					if (sibling > (int)address + ImageBase) return false;
					if (child > datafile.Length - 52 + ImageBase) return false;
					if (sibling > datafile.Length - 52 + ImageBase) return false;
					if (child != 0 && child < ImageBase) return false;
					if (sibling != 0 && sibling < ImageBase) return false;
					if (numhierarchy != -1 && child != 0 && !CheckModel(child - ImageBase, -1, modelfmt)) return false;
					if (numhierarchy != -1 && sibling != 0 && !CheckModel(sibling - ImageBase, -1, modelfmt)) return false;
					if (attach == 0 && flags == 0) return false;
					if (attach == 0 && child == 0 && sibling == 0) return false;
					if (child == address + ImageBase || child == attach) return false;
					if (sibling == address + ImageBase || sibling == attach) return false;
					if (child != 0 && child == sibling) return false;
					if (numhierarchy != -1 && child != 0)
					{
						if (numhierarchy < 3)
						{
							numhierarchy++;
							return CheckModel(child - ImageBase, numhierarchy, modelfmt);
						}
						else
							return CheckModel(child - ImageBase, -1, modelfmt);
					}
					if (numhierarchy != -1 && sibling != 0)
					{
						if (numhierarchy < 3)
						{
							numhierarchy++;
							return CheckModel(sibling - ImageBase, numhierarchy, modelfmt);
						}
						else
							return CheckModel(sibling - ImageBase, -1, modelfmt);
					}
					if (attach == 0 && flags == 0) return false;
					break;
			}
			if (numhierarchy != -1) Console.WriteLine("{0} model at {1}", modelfmt.ToString(), address.ToString("X"));
			return true;
		}

		static bool CheckLandTable(uint address, LandTableFormat landfmt)
		{
			ByteConverter.BigEndian = BigEndian;
			if (address > (uint)datafile.Length - 52) return false;
			short COLCount;
			short AnimCount;
			short ChunkCount;
			ushort Unknown1;
			uint COLAddress;
			uint AnimPointer;
			uint Texlist;
			uint Buffer;
			int ObjAddrPointer;
			uint ObjAddr;
			ModelFormat modelfmt = ModelFormat.Basic;
			switch (landfmt)
			{
				case LandTableFormat.SA1:
					modelfmt = ModelFormat.Basic;
					break;
				case LandTableFormat.SADX:
					modelfmt = ModelFormat.BasicDX;
					break;
				case LandTableFormat.SA2:
					modelfmt = ModelFormat.Chunk;
					break;
				case LandTableFormat.SA2B:
					modelfmt = ModelFormat.GC;
					break;
			}
			switch (landfmt)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					COLCount = ByteConverter.ToInt16(datafile, (int)address);
					if (COLCount <= 0 || COLCount > 2048) return false;
					AnimCount = ByteConverter.ToInt16(datafile, (int)address + 2);
					if (AnimCount < 0 || AnimCount > 2048) return false;
					COLAddress = ByteConverter.ToUInt32(datafile, (int)address + 0xC);
					if (COLAddress < ImageBase || COLAddress == 0) return false;
					if (COLAddress > datafile.Length - 32 + ImageBase) return false;
					AnimPointer = ByteConverter.ToUInt32(datafile, (int)address + 0x10);
					if (AnimPointer != 0 && AnimPointer < ImageBase) return false;
					if (AnimPointer > datafile.Length - 32 + ImageBase) return false;
					Texlist = ByteConverter.ToUInt32(datafile, (int)address + 0x18);
					if (Texlist != 0 && Texlist < ImageBase) return false;
					if (Texlist > datafile.Length - 32 + ImageBase) return false;
					ObjAddrPointer = (int)(COLAddress - ImageBase) + 0x18;
					ObjAddr = ByteConverter.ToUInt32(datafile, ObjAddrPointer);
					if (ObjAddr < ImageBase) return false;
					if (!CheckModel(ObjAddr - ImageBase, -1, modelfmt, true)) return false;
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					COLCount = ByteConverter.ToInt16(datafile, (int)address);
					if (COLCount < 0) return false;
					ChunkCount = ByteConverter.ToInt16(datafile, (int)address + 2);
					if (ChunkCount < -1) return false;
					Unknown1 = ByteConverter.ToUInt16(datafile, (int)address + 4);
					if (Unknown1 != 65535) return false;
					COLAddress = ByteConverter.ToUInt32(datafile, (int)address + 0x10);
					if (COLAddress < ImageBase) return false;
					if (COLAddress > datafile.Length - 32 + ImageBase) return false;
					Buffer = ByteConverter.ToUInt32(datafile, (int)address + 0x14);
					if (Buffer != 0) return false;
					AnimPointer = ByteConverter.ToUInt32(datafile, (int)address + 0x18);
					if (AnimPointer != 0 && AnimPointer < ImageBase) return false;
					if (AnimPointer > datafile.Length - 32 + ImageBase) return false;
					Texlist = ByteConverter.ToUInt32(datafile, (int)address + 0x1C);
					if (Texlist > datafile.Length - 32 + ImageBase) return false;
					if (Texlist == 0 || Texlist < ImageBase) return false;
					break;
			}
			return true;
		}
	}
}