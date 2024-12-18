using System;
using System.Collections.Generic;
using System.Text;

// Dreamcast Ninja Binary (.nj) format and its platform variations (.gj, .xj)
namespace SAModel
{
	public class NinjaBinaryFile
	{
		private enum NinjaBinaryChunkType
		{
			BasicModel,
			ChunkModel,
			Texlist,
			Motion,
			SimpleShapeMotion,
			POF0,
			Unimplemented,
			Invalid
		}

		public readonly List<NJS_OBJECT> Models; // In NJBM or NJCM
		public readonly List<NJS_MOTION> Motions; // In NMDM
		public readonly List<string[]> Texnames; // In NJTL

		private class NinjaDataChunk(NinjaBinaryChunkType type, byte[] data)
		{
			public readonly NinjaBinaryChunkType Type = type;
			public int ImageBase;
			public readonly byte[] Data = data;
		}

		private enum BigEndianResult
		{
			LittleEndian,
			BigEndian,
			CantTell
		}

		private static BigEndianResult CheckPointerBigEndian(byte[] data, int address)
		{
			var result = BigEndianResult.CantTell;
			// Back up Big Endian mode
			var bk = ByteConverter.BigEndian;
			// Set Big Endian mode
			ByteConverter.BigEndian = true;
			
			// Get Little Endian version
			var pntLittle = BitConverter.ToUInt32(data, address);
			// Get Big Endian version
			var pngBig = ByteConverter.ToUInt32(data, address);
			
			if (pntLittle > pngBig)
			{
				// If Little is bigger, it's likely Big Endian
				result = BigEndianResult.BigEndian;
			}
			else if (pntLittle < pngBig)
			{
				// If Big is bigger, it's likely Little Endian
				result = BigEndianResult.LittleEndian;
			}

			// Restore Big Endian mode
			ByteConverter.BigEndian = bk;
			return result;
		}

		public NinjaBinaryFile(byte[] data, ModelFormat format)
		{
			Models = [];
			Motions = [];
			Texnames = [];
			
			var startOffset = 0; // Current reading position.
			var modelCount = 0; // This is used to keep track of the model added last to get data for motions.
			var currentChunk = 0; // Keep track of current data chunk in case a POF0 chunk is found.
			var imgBase = 0; // Key added to pointers.
			var sizeIsLittleEndian = true; // In Gamecube games, size can be either Big or Little Endian.
			var chunks = new List<NinjaDataChunk>();
			// Back up Big Endian mode
			var bigEndianBk = ByteConverter.BigEndian;
			
			// Read the file until the end
			while (startOffset < data.Length - 8) // 8 is the size of chunk ID + chunk size
			{
				// Skip padding and unrecognized data
				if (IdentifyChunk(data, startOffset) == NinjaBinaryChunkType.Invalid)
				{
					while (IdentifyChunk(data, startOffset) == NinjaBinaryChunkType.Invalid)
					{
						// Stop if reached the end of file
						if (startOffset >= data.Length - 4)
						{
							break;
						}

						startOffset += 1;
					}
				}
				
				// Stop if reached the end of file
				if (startOffset >= data.Length - 4)
				{
					break;
				}

				// Get Ninja data chunk type
				var idType = IdentifyChunk(data, startOffset);
				// Endianness checks for the first chunk
				if (currentChunk == 0)
				{
					// This check is done because in PSO GC chunk size is in Little Endian despite the rest of the data being Big Endian.
					// First, determine whether size is Big Endian or not.
					ByteConverter.BigEndian = true;
					sizeIsLittleEndian = BitConverter.ToUInt32(data, startOffset + 4) < ByteConverter.ToUInt32(data, startOffset + 4);
					// Then, check if the actual data is Big Endian. Unfortunately this is just guessing so it may not always work.
					// startoffset + 8 is where the data begins
					
					switch (idType)
					{
						case NinjaBinaryChunkType.BasicModel:
						case NinjaBinaryChunkType.ChunkModel:
							// Check attach pointer
							var resEndian = CheckPointerBigEndian(data, startOffset + 8 + 4);
							if (resEndian == BigEndianResult.CantTell)
							{
								// Check child pointer
								resEndian = CheckPointerBigEndian(data, startOffset + 8 + 0x2C);
								if (resEndian == BigEndianResult.CantTell)
								{
									// Check sibling pointer
									resEndian = CheckPointerBigEndian(data, startOffset + 8 + 0x30);
								}
							}
							ByteConverter.BigEndian = resEndian == BigEndianResult.BigEndian;
							break;
						case NinjaBinaryChunkType.Motion: // Number of frames
						case NinjaBinaryChunkType.SimpleShapeMotion: // Number of frames
						case NinjaBinaryChunkType.Texlist: // Number of texnames
							ByteConverter.BigEndian = BitConverter.ToUInt32(data, startOffset + 12) > ByteConverter.ToUInt32(data, startOffset + 12);
							break;
						default: // Old check
							ByteConverter.BigEndian = BitConverter.ToUInt32(data, startOffset + 8) > ByteConverter.ToUInt32(data, startOffset + 8);
							break;							
					}
				}
				
				var size = sizeIsLittleEndian ? BitConverter.ToInt32(data, startOffset + 4) : ByteConverter.ToInt32(data, startOffset + 4);
				// Add the chunk to the list to process
				chunks.Add(new NinjaDataChunk(idType, new byte[size]));
				Array.Copy(data, startOffset + 8, chunks[currentChunk].Data, 0, chunks[currentChunk].Data.Length);
				
				if (idType == NinjaBinaryChunkType.POF0)
				{
					// If a POF0 chunk is reached, fix up the previous chunk's pointers
					var offs = POF0Helper.GetPointerListFromPOF(chunks[currentChunk].Data);
					POF0Helper.FixPointersWithPOF(chunks[currentChunk - 1].Data, offs, imgBase);
					chunks[currentChunk - 1].ImageBase = imgBase;
				}
				else
				{
					// Otherwise advance the reading position and pointer image base
					imgBase += startOffset;
				}

				startOffset += chunks[currentChunk].Data.Length + 8;
				currentChunk++;
			}
			
			// Go over the fixed chunks and add final data
			foreach (var chunk in chunks)
			{
				switch (chunk.Type)
				{
					case NinjaBinaryChunkType.BasicModel:
						// Add a label so that all models aren't called "object_00000000"
						Dictionary<int, string> basicLabels = [];
						basicLabels.Add(0, "object_" + chunk.ImageBase.ToString("X8"));
						Models.Add(new NJS_OBJECT(chunk.Data, 0, (uint)chunk.ImageBase, ModelFormat.Basic, basicLabels, new Dictionary<int, Attach>()));
						modelCount++;
						break;
					case NinjaBinaryChunkType.ChunkModel:
						// Add a label so that all models aren't called "object_00000000"
						Dictionary<int, string> chunkLabels = [];
						chunkLabels.Add(0, "object_" + chunk.ImageBase.ToString("X8"));
						// NJCM can be Chunk (NJ file, Big or Little Endian), Ginja (GJ file) or XJ (XJ file)
						Models.Add(new NJS_OBJECT(chunk.Data, 0, (uint)chunk.ImageBase, format, chunkLabels, new Dictionary<int, Attach>()));
						modelCount++;
						break;
					case NinjaBinaryChunkType.Texlist:
						var firstEntry = ByteConverter.ToInt32(chunk.Data, 0) - chunk.ImageBase; // Prooobably, seems to be 8 always
						var numTextures = ByteConverter.ToInt32(chunk.Data, 0x4);
						List<string> texNames = [];
						
						// Add texture names
						for (var i = 0; i < numTextures; i++)
						{
							var textAddress = ByteConverter.ToInt32(chunk.Data, firstEntry + i * 0xC) - chunk.ImageBase; // 0xC is the size of NJS_TEXNAME
							// Read the null terminated string
							List<byte> nameString = [];
							var nameChar = chunk.Data[textAddress];
							var j = 0;
							
							while (nameChar != 0)
							{
								nameString.Add(nameChar);
								j++;
								nameChar = chunk.Data[textAddress + j];
							}
							
							texNames.Add(Encoding.ASCII.GetString(nameString.ToArray()));
						}
						
						Texnames.Add(texNames.ToArray());
						break;
					case NinjaBinaryChunkType.Motion:
						try
						{
							// Add a label so that all motions aren't called "motion_00000000"
							Dictionary<int, string> motionLabels = [];
							motionLabels.Add(0, $"motion_{chunk.ImageBase:X8}");
							Motions.Add(new NJS_MOTION(chunk.Data, 0, (uint)chunk.ImageBase, Models.Count > 0 ? Models[modelCount - 1].CountAnimated() : -1, motionLabels, objectName: Models.Count > 0 ? Models[modelCount - 1].Name : ""));
						}
						catch (Exception ex)
						{
							throw new Exception($"Error adding motion at 0x{chunk.ImageBase:X}: {ex.Message}");
						}
						break;
					case NinjaBinaryChunkType.SimpleShapeMotion:
						try
						{
							// Add a label so that all motions aren't called "motion_00000000"
							Dictionary<int, string> shapeLabels = [];
							shapeLabels.Add(0, $"shape_{chunk.ImageBase:X8}");
							Motions.Add(new NJS_MOTION(chunk.Data, 0, (uint)chunk.ImageBase, Models.Count > 0 ? Models[modelCount - 1].CountAnimated() : -1, shapeLabels, numverts: Models[modelCount].GetVertexCounts()));
						}
						catch (Exception ex)
						{
							throw new Exception($"Error adding shape motion at 0x{chunk.ImageBase:X}: {ex.Message}");
						}
						break;

				}
			}
			
			// Restore Big Endian mode
			ByteConverter.BigEndian = bigEndianBk;
		}

		private NinjaBinaryChunkType IdentifyChunk(byte[] data, int offset)
		{
			if (offset >= data.Length - 8)
			{
				return NinjaBinaryChunkType.Invalid;
			}

			if (BitConverter.ToUInt32(data, offset + 4) == 0)
			{
				return NinjaBinaryChunkType.Invalid;
			}

			switch (Encoding.ASCII.GetString(data, offset, 4))
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
				case "CMCK": // Illbleed
					return NinjaBinaryChunkType.Unimplemented;
				// Invalid/unknown chunks. These can be ignored as they aren't followed by POF0.
				case "POF1": // Pointer Offset List (absolute)
				case "POF2": // Pointer Offset List (unknown)
				case "GRND": // Skies of Arcadia
				case "GOBJ": // Skies of Arcadia
				default:
					return NinjaBinaryChunkType.Invalid;
			}
		}
	}
}