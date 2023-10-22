using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

// Dreamcast Ninja Binary (.nj) format and its platform variations (.gj, .xj)
namespace SAModel
{
	public class NinjaBinaryFile
	{
		enum NinjaBinaryChunkType
		{
			BasicModel,
			ChunkModel,
			Texlist,
			Motion,
			SimpleShapeMotion,
			POF0,
			CustomTexture, // CMCK from Illbleed
			Unimplemented,
			Invalid
		}

		public List<NJS_OBJECT> Models; // In NJBM or NJCM
		public List<NJS_MOTION> Motions; // In NMDM
		public List<string[]> Texnames; // In NJTL
		public List<byte[]> Textures; // In CNCK (Illbleed)

		private class NinjaDataChunk
		{
			public NinjaBinaryChunkType Type;
			public int ImageBase;
			public byte[] Data;

			public NinjaDataChunk(NinjaBinaryChunkType type, byte[] data)
			{
				Type = type;
				Data = data;
			}
		}

		public NinjaBinaryFile(byte[] data, bool bigEndian, ModelFormat format)
		{
			Models = new List<NJS_OBJECT>();
			Motions = new List<NJS_MOTION>();
			Texnames = new List<string[]>();
			Textures = new List<byte[]>();
			int startoffset = 0; // Current reading position.
			int modelcount = 0; // This is used to keep track of the model added last to get data for motions.
			int currentchunk = 0; // Keep track of current data chunk in case a POF0 chunk is found.
			int imgBase = 0; // Key added to pointers.
			List<NinjaDataChunk> chunks = new List<NinjaDataChunk>();
			// Set Big Endian mode
			bool bigEndianBk = ByteConverter.BigEndian;
			ByteConverter.BigEndian = bigEndian;
			// Read the file until the end
			while (startoffset < data.Length - 8) // 8 is the size of chunk ID + chunk size
			{
				// Skip padding and unrecognized data
				if (IdentifyChunk(data, startoffset) == NinjaBinaryChunkType.Invalid)
				{
					while (IdentifyChunk(data, startoffset) == NinjaBinaryChunkType.Invalid)
					{
						// Stop if reached the end of file
						if (startoffset >= data.Length - 4)
							break;
						startoffset += 1;
					}
				}
				// Stop if reached the end of file
				if (startoffset >= data.Length - 4)
					break;
				// Get Ninja data chunk type
				NinjaBinaryChunkType idtype = IdentifyChunk(data, startoffset);
				// This check is done because in PSO Gamecube chunk size is in Little Endian despite the rest of the data being Big Endian.
				bool isLittleEndian = BitConverter.ToUInt32(data, startoffset + 4) < ByteConverter.ToUInt32(data, startoffset + 4);
				int size = isLittleEndian ? BitConverter.ToInt32(data, startoffset + 4) : ByteConverter.ToInt32(data, startoffset + 4);
				//MessageBox.Show(idtype.ToString() + " chunk at " + (startoffset + 8).ToString("X8") + " size " + size.ToString());
				// Add the chunk to the list to process
				chunks.Add(new NinjaDataChunk(idtype, new byte[size]));
				Array.Copy(data, startoffset + 8, chunks[currentchunk].Data, 0, chunks[currentchunk].Data.Length);
				// If a POF0 chunk is reached, fix up the previous chunk's pointers
				if (idtype == NinjaBinaryChunkType.POF0)
				{
					List<int> offs = POF0Helper.GetPointerListFromPOF(chunks[currentchunk].Data);
					//MessageBox.Show("POF at " + (startoffset + 8).ToString("X") + " imgBase: " + imgBase.ToString("X") + " size " + chunks[currentchunk].Data.Length.ToString());
					POF0Helper.FixPointersWithPOF(chunks[currentchunk - 1].Data, offs, imgBase);
					chunks[currentchunk - 1].ImageBase = imgBase;
					//File.WriteAllBytes("C:\\Users\\PkR\\Desktop\\chunk\\" + currentchunk.ToString("D3") + "_pof.bin", chunks[currentchunk].Data);
					//File.WriteAllBytes("C:\\Users\\PkR\\Desktop\\chunk\\" + currentchunk.ToString("D3") + ".bin", chunks[currentchunk - 1].Data);
					startoffset += chunks[currentchunk].Data.Length + 8;
				}
				// Otherwise advance the reading position and pointer image base
				else
				{
					imgBase += startoffset;
					startoffset += chunks[currentchunk].Data.Length + 8;
				}
				currentchunk++;
			}
			// Go over the fixed chunks and add final data
			foreach (NinjaDataChunk chunk in chunks)
			{
				switch (chunk.Type)
				{
					case NinjaBinaryChunkType.BasicModel:
						//MessageBox.Show("Basic model at " + chunk.ImageBase.ToString("X") + " size " + chunk.Data.Length.ToString());
						// Add a label so that all models aren't called "object_00000000"
						Dictionary<int, string> labelb = new Dictionary<int, string>();
						labelb.Add(0, "object_" + chunk.ImageBase.ToString("X8"));
						Models.Add(new NJS_OBJECT(chunk.Data, 0, (uint)chunk.ImageBase, ModelFormat.Basic, labelb, new Dictionary<int, Attach>()));
						modelcount++;
						break;
					case NinjaBinaryChunkType.ChunkModel:
						//MessageBox.Show(format.ToString() + " model at " + chunk.ImageBase.ToString("X") + " size " + chunk.Data.Length.ToString());
						// Add a label so that all models aren't called "object_00000000"
						Dictionary<int, string> labelc = new Dictionary<int, string>();
						labelc.Add(0, "object_" + chunk.ImageBase.ToString("X8"));
						// NJCM can be Chunk (NJ file, Big or Little Endian), Ginja (GJ file) or XJ (XJ file)
						Models.Add(new NJS_OBJECT(chunk.Data, 0, (uint)chunk.ImageBase, format, labelc, new Dictionary<int, Attach>()));
						modelcount++;
						break;
					case NinjaBinaryChunkType.Texlist:
						//MessageBox.Show("Texlist at " + chunk.ImageBase.ToString("X") + " size " + chunk.Data.Length.ToString());
						int firstEntry = ByteConverter.ToInt32(chunk.Data, 0) - chunk.ImageBase; // Prooobably, seems to be 8 always
						int numTextures = ByteConverter.ToInt32(chunk.Data, 0x4);
						List<string> texNames = new List<string>();
						// Add texture names
						for (int i = 0; i < numTextures; i++)
						{
							int textAddress = ByteConverter.ToInt32(chunk.Data, firstEntry + i * 0xC) - chunk.ImageBase; // 0xC is the size of NJS_TEXNAME
																														 // Read the null terminated string
							List<byte> namestring = new List<byte>();
							byte namechar = (chunk.Data[textAddress]);
							int j = 0;
							while (namechar != 0)
							{
								namestring.Add(namechar);
								j++;
								namechar = (chunk.Data[textAddress + j]);
							}
							texNames.Add(Encoding.ASCII.GetString(namestring.ToArray()));
						}
						Texnames.Add(texNames.ToArray());
						break;
					case NinjaBinaryChunkType.Motion:
						//MessageBox.Show("Motion with ImgBase " + chunk.ImageBase.ToString("X") + " size " + chunk.Data.Length.ToString());
						try
						{
							// Add a label so that all motions aren't called "motion_00000000"
							Dictionary<int, string> labelm = new Dictionary<int, string>();
							labelm.Add(0, "motion_" + chunk.ImageBase.ToString("X8"));
							Motions.Add(new NJS_MOTION(chunk.Data, 0, (uint)chunk.ImageBase, Models.Count > 0 ? Models[modelcount - 1].CountAnimated() : -1, labelm, objectName: Models.Count > 0 ? Models[modelcount - 1].Name : ""));
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error adding motion at 0x" + chunk.ImageBase.ToString("X") + ": " + ex.Message.ToString());
						}
						break;
					case NinjaBinaryChunkType.SimpleShapeMotion:
						//MessageBox.Show("Shape Motion with ImgBase " + chunk.ImageBase.ToString("X") + " size " + chunk.Data.Length.ToString());
						try
						{
							// Add a label so that all motions aren't called "motion_00000000"
							Dictionary<int, string> labels = new Dictionary<int, string>();
							labels.Add(0, "shape_" + chunk.ImageBase.ToString("X8"));
							Motions.Add(new NJS_MOTION(chunk.Data, 0, (uint)chunk.ImageBase, Models.Count > 0 ? Models[modelcount - 1].CountAnimated() : -1, labels, numverts: Models[modelcount].GetVertexCounts()));
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error adding shape motion at 0x" + chunk.ImageBase.ToString("X") + ": " + ex.Message.ToString());
						}
						break;
					case NinjaBinaryChunkType.CustomTexture:
						// ImageBase set to 0 usually means it's not texture data, so there's no need to process it.
						if (chunk.ImageBase == 0)
							break;
						//MessageBox.Show("Textures at " + chunk.ImageBase.ToString("X") + " size " + chunk.Data.Length.ToString());
						int numPvr = ByteConverter.ToInt32(chunk.Data, 0);
						for (int i = 0; i < numPvr; i++)
						{
							int currentPos = 4 + i * 4; // Pointer to current entry
							if (currentPos >= chunk.Data.Length)
								break;
							//MessageBox.Show("Item " + i.ToString());
							int texOffset = ByteConverter.ToInt32(chunk.Data, currentPos); // Offset of the current entry
							if (texOffset >= chunk.Data.Length - 0x14) // Texture start + 0x14 should be after GBIX header and PVRT magic
								break;
							// Total size = size specified in PVR header + GBIX header + PVRT header
							int texSize = ByteConverter.ToInt32(chunk.Data, texOffset + 0x14) + 0x18;
							// Check if this is actually a texture
							string magic = System.Text.Encoding.ASCII.GetString(chunk.Data, texOffset, 4);
							if (magic == "GBIX" || magic == "PVRT" || magic == "GVRT")
							{
								byte[] texture = new byte[texSize];
								Array.Copy(chunk.Data, texOffset, texture, 0, texSize);
								Textures.Add(texture);
							}
						}
						break;

				}
			}
			//MessageBox.Show("Models: " + Models.Count.ToString() + " Animations: " + Motions.Count.ToString() + " Texlists: " + Texnames.Count.ToString() + ", Texture arrays: " + Textures.Count.ToString());
			ByteConverter.BigEndian = bigEndianBk;
		}

		private NinjaBinaryChunkType IdentifyChunk(byte[] data, int offset)
		{
			if (offset >= data.Length - 8)
				return NinjaBinaryChunkType.Invalid;
			if (BitConverter.ToUInt32(data, offset + 4) == 0)
				return NinjaBinaryChunkType.Invalid;
			switch (System.Text.Encoding.ASCII.GetString(data, offset, 4))
			{
				// Implemented chunk types
				case "NJBM":
				case "GJBM":
					return NinjaBinaryChunkType.BasicModel;
				case "NJCM":
				case "GJCM":
					return NinjaBinaryChunkType.ChunkModel;
				case "NMDM":
					return NinjaBinaryChunkType.Motion;
				case "NSSM":
					return NinjaBinaryChunkType.SimpleShapeMotion;
				case "NJTL":
				case "GJTL":
					return NinjaBinaryChunkType.Texlist;
				case "POF0":
					return NinjaBinaryChunkType.POF0;
				case "CMCK":
					return NinjaBinaryChunkType.CustomTexture; // Illbleed
				// Unimplemented types. These have to be accounted for because they are followed by POF0.
				case "NJLI": // Ninja Light
				case "NJCA": // Ninja Camera
				case "NLIM": // Ninja Light Motion
				case "NJIN": // Ninja Metadata
				case "N2CM": // Ninja2 Chunk Model
				case "NJSP": // Ninja Cell Sprite
				case "NJCS": // Ninja Cell Stream
				case "NCSM": // Ninja Cell Sprite Motion
				case "CPSM": // Ninja Compact Shape Motion
				case "NJSL": // Ninja Compact Shape List
				case "CGCL": // Illbleed
				case "CGLC": // Illbleed
				case "CGMP": // Illbleed
				case "CGSP": // Illbleed
				case "CGAL": // Illbleed
				case "CGAM": // Illbleed
				case "NCAM": // Illbleed
					return NinjaBinaryChunkType.Unimplemented;
				// Invalid/unknown chunks. These can be ignored as they aren't followed by POF0.
				case "POF1": // Pointer Offset List (absolute)
				case "POF2": // Pointer Offset List (unknown)
				case "GRND": // Skies of Arcadia
				case "GOBJ": // Skies of Arcadia
				case "GLKH": // Skies of Arcadia (stores filenames)
				default:
					return NinjaBinaryChunkType.Invalid;
			}
		}
	}
}