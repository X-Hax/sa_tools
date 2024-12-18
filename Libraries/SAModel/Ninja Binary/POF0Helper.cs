using System;
using System.Collections.Generic;

namespace SAModel
{
	// Class to work with pointer offset lists (POF) in Ninja Binary files
	class POF0Helper
	{
		private enum POFOffsetType: byte
		{
			Padding = 0x00,
			Char = 0x40,
			Short = 0x80,
			Long = 0xC0,
			TypeMask = 0xC0,
			DataMask = 0x3F,
		}

		// Creates a POF chunk out of a list of absolute offsets
		public static byte[] GetPOFData(List<uint> offsets)
		{
			List<byte> result = [];
			byte[] magic = [0x50, 0x4F, 0x46, 0x30]; // POF0
			
			result.AddRange(magic);
			
			for (var i = 0; i < offsets.Count; i++)
			{
				var offsetDiff = i == 0 ? offsets[i] : offsets[i] - offsets[i - 1];
				var offsetDiv = offsetDiff / 4;
				byte[] offsetBytes;
				
				// Write offset with mask
				if (offsetDiff > 0xFF)
				{
					// Long
					if (offsetDiff > 0xFFFF)
					{
						var converted = BitConverter.GetBytes(offsetDiv); // Same Endianness
						offsetBytes = new byte[4];
						offsetBytes[0] = (byte)(converted[3] | (byte)POFOffsetType.Long);
						offsetBytes[1] = converted[2];
						offsetBytes[2] = converted[1];
						offsetBytes[3] = converted[0];
					}
					// Short
					else
					{
						var shortCalc = (ushort)offsetDiv;
						var converted = BitConverter.GetBytes(shortCalc); // Same Endianness
						offsetBytes = new byte[2];
						offsetBytes[0] = (byte)(converted[1] | (byte)POFOffsetType.Short);
						offsetBytes[1] = converted[0];
					}
				}
				// Char
				else
				{
					var byteCalc = (byte)(offsetDiv | (byte)POFOffsetType.Char);
					offsetBytes = [byteCalc];
				}
				
				result.AddRange(offsetBytes);
			}
			
			result.Align(4);
			// Add length
			result.InsertRange(4, ByteConverter.GetBytes(result.Count - 4)); // Variable Endianness
			return result.ToArray();
		}

		// Adjusts pointers in a data chunk using a pointer list
		public static void FixPointersWithPOF(byte[] data, List<int> pointerList, int imgBase)
		{
			var currentPos = 0;
			
			foreach (var pointer in pointerList)
			{
				currentPos += pointer;
				var oldPointer = ByteConverter.ToInt32(data, currentPos);
				
				if (oldPointer != 0)
				{
					oldPointer += imgBase;
				}

				var newPointerBytes = ByteConverter.GetBytes(oldPointer);
				Array.Copy(newPointerBytes, 0, data, currentPos, 4);
			}
		}

		// Gets a pointer list out of a POF data chunk
		public static List<int> GetPointerListFromPOF(byte[] pofdata)
		{
			List<int> offsets = [];
			var currentOffset = 0;
			
			while (currentOffset < pofdata.Length)
			{
				var first = (byte)(pofdata[currentOffset] & (byte)POFOffsetType.DataMask);
				var type = (POFOffsetType)(pofdata[currentOffset] & (byte)POFOffsetType.TypeMask);
				currentOffset++;
				
				switch (type)
				{
					// Padding
					case POFOffsetType.Padding:
						break;
					// Single byte
					case POFOffsetType.Char:
						offsets.Add(4 * first);
						break;
					// Two bytes
					case POFOffsetType.Short:
						var second = pofdata[currentOffset];
						offsets.Add(4 * ((first << 8) | second));
						currentOffset += 1;
						break;
					// Four bytes
					case POFOffsetType.Long:
						var secondL = pofdata[currentOffset];
						var thirdL = pofdata[currentOffset + 1];
						var fourthL = pofdata[currentOffset + 2];
						offsets.Add(4 * ((first << 24) | (secondL << 16) | (thirdL << 8) | fourthL));
						currentOffset += 3;
						break;
				}
			}

			return offsets;
		}
	}
}
