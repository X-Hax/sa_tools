using System;
using System.Collections.Generic;

namespace TextureLib
{
	/// <summary>
	/// Class that converts DXT1 texture data between DDS and Gamecube GVR.
	/// This class was made solely for the above purpose and may not be suitable for other conversions.
	/// In particular, there are assumptions regarding byte order that would need to be adjusted.
	/// </summary>
	public class DxtConverter
	{
		// A DXT1 data block. Consists of two RGB565 colors and 16 2-bit palette indices. 16 (4x4) pixels and 8 bytes total.
		public class DxtBlock
		{
			ushort Color1;
			ushort Color2;
			byte[] Indices;

			public DxtBlock(byte[] data, int offset)
			{
				Indices = new byte[16];
				Color1 = ByteConverter.ToUInt16BE(data, offset);
				Color2 = ByteConverter.ToUInt16BE(data, offset + 2);
				byte bb1 = data[offset + 4];
				Indices[0] = (byte)((bb1 >> 6) & 0b_11);
				Indices[1] = (byte)((bb1 >> 4) & 0b_11);
				Indices[2] = (byte)((bb1 >> 2) & 0b_11);
				Indices[3] = (byte)(bb1 & 0b_11);

				byte bb2 = data[offset + 5];
				Indices[4] = (byte)((bb2 >> 6) & 0b_11);
				Indices[5] = (byte)((bb2 >> 4) & 0b_11);
				Indices[6] = (byte)((bb2 >> 2) & 0b_11);
				Indices[7] = (byte)(bb2 & 0b_11);

				byte bb3 = data[offset + 6];
				Indices[8] = (byte)((bb3 >> 6) & 0b_11);
				Indices[9] = (byte)((bb3 >> 4) & 0b_11);
				Indices[10] = (byte)((bb3 >> 2) & 0b_11);
				Indices[11] = (byte)(bb3 & 0b_11);

				byte bb4 = data[offset + 7];
				Indices[12] = (byte)((bb4 >> 6) & 0b_11);
				Indices[13] = (byte)((bb4 >> 4) & 0b_11);
				Indices[14] = (byte)((bb4 >> 2) & 0b_11);
				Indices[15] = (byte)(bb4 & 0b_11);
			}

			public byte[] GetBytes()
			{
				List<byte> result = new List<byte>();
				result.AddRange(BitConverter.GetBytes(Color1));
				result.AddRange(BitConverter.GetBytes(Color2));
				byte b1 = (byte)((Indices[3] << 6) | (Indices[2] << 4) | (Indices[1] << 2) | Indices[0]);
				result.Add(b1);
				byte b2 = (byte)((Indices[7] << 6) | (Indices[6] << 4) | (Indices[5] << 2) | Indices[4]);
				result.Add(b2);
				byte b3 = (byte)((Indices[11] << 6) | (Indices[10] << 4) | (Indices[9] << 2) | Indices[8]);
				result.Add(b3);
				byte b4 = (byte)((Indices[15] << 6) | (Indices[14] << 4) | (Indices[13] << 2) | Indices[12]);
				result.Add(b4);
				return result.ToArray();
			}
		}

		// A Gamecube CMPR tile which consists of four (2x2) DXT1 blocks. 64 (8x8) pixels and 32 bytes total.
		// Bit and byte order is different from regular DXT1.
		public class GamecubeDxtTile
		{
			public DxtBlock TopLeft;
			public DxtBlock TopRight;
			public DxtBlock BottomLeft;
			public DxtBlock BottomRight;

			public GamecubeDxtTile(byte[] data, int offset, bool bigEndian)
			{
				TopLeft = new DxtBlock(data, offset);
				TopRight = new DxtBlock(data, offset + 8);
				BottomLeft = new DxtBlock(data, offset + 16);
				BottomRight = new DxtBlock(data, offset + 24);
			}

			public GamecubeDxtTile()
			{ }

		}

		/// <summary>
		/// Converts DXT1 bytes from the Gamecube format to the PC DDS format and vice versa.
		/// </summary>
		/// <param name="src">Source byte array.</param>
		/// <param name="width">Texture width.</param>
		/// <param name="height">Texture height.</param>
		/// <param name="fromGvr">True if converting from GVR to DDS, false if converting from DDS to GVR.</param>
		/// <returns>Converted byte array containing DXT1 compressed texture data in the target format.</returns>
		public static byte[] ConvertDxt(byte[] src, int width, int height, bool fromGvr)
		{
			// DXT block minimum dimensions
			width = Math.Max(width, 4);
			height = Math.Max(height, 4);
			const int CmprTileSizeBytes = 32; // GVR CMPR block size in bytes
			const int CmprTileWidth = 8; // GVR CMPR block width and height

			const int DxtBlockSizeBytes = 8;
			const int DxtBlockSizeWidth = 4;

			List<byte> res = new List<byte>();

			int numDxtBlockHz = width / DxtBlockSizeWidth;
			int numDxtBlockVt = height / DxtBlockSizeWidth;
			int numDxtBlocks = numDxtBlockHz * numDxtBlockVt;

			int numTiles = src.Length / CmprTileSizeBytes;
			int numTileHz = width / CmprTileWidth;
			int numTileVt = height / CmprTileWidth;

#if DEBUG
			Console.WriteLine(fromGvr ? "From GVR" : "From DDS");
			Console.WriteLine("DXT blocks: {0} ({1}x{2})", numDxtBlocks, numDxtBlockHz, numDxtBlockVt);
			Console.WriteLine("CMPR blocks: {0} ({1}x{2})", numTiles, numTileHz, numTileVt);
			Console.WriteLine("Dimensions: {0}x{1}", width,height);
#endif
			// From GVR to DDS
			if (fromGvr)
			{
				// Build Gamecube CMPR blocks
				List<GamecubeDxtTile> tiles = new List<GamecubeDxtTile>();
				// Add each block
				for (int p = 0; p < numTiles; p++)
				{
					tiles.Add(new GamecubeDxtTile(src, p * CmprTileSizeBytes, fromGvr));
				}
				// Write out DXT blocks
				if (numTileVt > 0)
				{
					for (int y = 0; y < numTileVt; y++)
					{
						// First row
						for (int x = 0; x < numTileHz; x++)
						{
							res.AddRange(tiles[numTileHz * y + x].TopLeft.GetBytes());
							res.AddRange(tiles[numTileHz * y + x].TopRight.GetBytes());
						}
						// Second row
						for (int x = 0; x < numTileHz; x++)
						{
							res.AddRange(tiles[numTileHz * y + x].BottomLeft.GetBytes());
							res.AddRange(tiles[numTileHz * y + x].BottomRight.GetBytes());
						}
					}
				}
				// If there isn't enough data to form a Gamecube block, process all blocks in sequential order
				else
				{
					int blockCount = 0;
					DxtBlock block = null;
					for (int b = 0; b < numDxtBlocks; b++)
					{
						Console.WriteLine("Block {0}", b);
						block = new DxtBlock(src, 8 * b);
						res.AddRange(block.GetBytes());
						blockCount++;
					}
				}
			}
			// From DDS to GVR
			else
			{
				// Build DXT clocks
				List<DxtBlock> blocks = new List<DxtBlock>();
				// Add each block
				for (int p = 0; p < numDxtBlocks; p++)
				{
					blocks.Add(new DxtBlock(src, p * DxtBlockSizeBytes));
				}
				// Build Gamecube CMPR blocks
				List<GamecubeDxtTile> tiles = new List<GamecubeDxtTile>();
				for (int y = 0; y < numTileVt; y++)
				{
					for (int x = 0; x < numTileHz; x++)
					{
						GamecubeDxtTile tile = new GamecubeDxtTile()
						{
							TopLeft = blocks[numDxtBlockHz * y * 2 + x * 2],
							TopRight = blocks[numDxtBlockHz * y * 2 + x * 2 + 1],
							BottomLeft = blocks[numDxtBlockHz * (y * 2 + 1) + x * 2],
							BottomRight = blocks[numDxtBlockHz * (y * 2 + 1) + x * 2 + 1]
						};
						tiles.Add(tile);
					}
				}
				// Write out CMPR blocks
				for (int t = 0; t < tiles.Count; t++)
				{
					res.AddRange(tiles[t].TopLeft.GetBytes());
					res.AddRange(tiles[t].TopRight.GetBytes());
					res.AddRange(tiles[t].BottomLeft.GetBytes());
					res.AddRange(tiles[t].BottomRight.GetBytes());
				}
			}
			return res.ToArray();
		}
	}
}