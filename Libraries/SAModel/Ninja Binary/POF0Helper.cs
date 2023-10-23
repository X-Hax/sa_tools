using System;
using System.Collections.Generic;

namespace SAModel
{
	// Class to work with pointer offset lists (POF) in Ninja Binary files
	class POF0Helper
	{
		enum POFOffsetType: byte
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
			List<byte> result = new List<byte>();
			byte[] magic = { 0x50, 0x4F, 0x46, 0x30 }; // POF0
			result.AddRange(magic);
			for (int i = 0; i < offsets.Count; i++)
			{
				uint offsetDiff = (i == 0) ? offsets[i] : offsets[i] - offsets[i - 1];
				uint offsetDiv = offsetDiff / 4;
				byte[] offsetBytes;
				// Write offset with mask
				if (offsetDiff > 0xFF)
				{
					// Long
					if (offsetDiff > 0xFFFF)
					{
						byte[] converted = BitConverter.GetBytes(offsetDiv); // Same Endianness
						offsetBytes = new byte[4];
						offsetBytes[0] = (byte)(converted[3] | (byte)POFOffsetType.Long);
						offsetBytes[1] = converted[2];
						offsetBytes[2] = converted[1];
						offsetBytes[3] = converted[0];
					}
					// Short
					else
					{
						ushort shortCalc = (ushort)offsetDiv;
						byte[] converted = BitConverter.GetBytes(shortCalc); // Same Endianness
						offsetBytes = new byte[2];
						offsetBytes[0] = (byte)(converted[1] | (byte)POFOffsetType.Short);
						offsetBytes[1] = converted[0];
					}
				}
				// Char
				else
				{
					byte byteCalc = (byte)(offsetDiv | (byte)POFOffsetType.Char);
					offsetBytes = new byte[] { byteCalc };
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
			//System.Text.StringBuilder sb = new StringBuilder();
			//System.Windows.Forms.MessageBox.Show(data.Length.ToString());
			int currentPos = 0;
			foreach (int pointer in pointerList)
			{
				currentPos += pointer;
				//System.Windows.Forms.MessageBox.Show("Pointer: " + pointer.ToString("X") + " currentPos: " + currentPos.ToString("X"));
				int oldPointer = ByteConverter.ToInt32(data, currentPos);
				if (oldPointer != 0)
					oldPointer += imgBase;
				byte[] newPointerBytes = ByteConverter.GetBytes(oldPointer);
				//sb.Append("\n" + "Fixing pointer at " + (currentPos).ToString("X") + ": " + (oldPointer < imgBase ? "0" : ((uint)oldPointer - (uint)imgBase).ToString("X")) + " to " + oldPointer.ToString("X"));
				//System.IO.File.WriteAllText("C:\\Users\\PkR\\Desktop\\sb" + imgBase.ToString("X") + ".txt", sb.ToString());
				//System.Windows.Forms.MessageBox.Show("Fixing pointer at " + (currentPos).ToString("X") + ": " + oldPointer.ToString("X"));
				Array.Copy(newPointerBytes, 0, data, currentPos, 4);
			}
		}

		// Gets a pointer list out of a POF data chunk
		public static List<int> GetPointerListFromPOF(byte[] pofdata)
		{
			int chars = 0;
			int pads = 0;
			int shorts = 0;
			int longs = 0;
			List<int> offsets = new List<int>();
			int currentoff = 0;
			//MessageBox.Show("Pof length: " + pofdata.Length.ToString());
			while (currentoff < pofdata.Length)
			{
				//MessageBox.Show("Tock " + currentoff.ToString());
				byte first = (byte)(pofdata[currentoff] & (byte)POFOffsetType.DataMask);
				POFOffsetType type = (POFOffsetType)(pofdata[currentoff] & (byte)POFOffsetType.TypeMask);
				currentoff++;
				switch (type)
				{
					// Padding
					case POFOffsetType.Padding:
						//MessageBox.Show("This is padding: " + pofdata[currentoff-1].ToString("X") + "at " + currentoff.ToString());
						pads++;
						break;
					// Single byte
					case POFOffsetType.Char:
						offsets.Add(4 * first);
						chars++;
						break;
					// Two bytes
					case POFOffsetType.Short:
						byte second = pofdata[currentoff];
						offsets.Add(4 * ((first << 8) | second));
						currentoff += 1;
						shorts++;
						break;
					// Four bytes
					case POFOffsetType.Long:
						byte second_l = pofdata[currentoff];
						byte third_l = pofdata[currentoff + 1];
						byte fourth_l = pofdata[currentoff + 2];
						offsets.Add(4 * ((first << 24) | (second_l << 16) | (third_l << 8) | fourth_l));
						currentoff += 3;
						longs++;
						break;
				}
			}
			/*
			StringBuilder sb = new StringBuilder();
			int currentPos = 0;
			for (int i = 0; i < offsets.Count; i++)
			{
				currentPos += offsets[i];
				sb.Append(", " + currentPos.ToString("X"));	
			}
			sb.AppendJoin(',', offsets.ToArray());
			
			System.Windows.Forms.MessageBox.Show("offsets in POF: " + offsets.Count.ToString() + ", pads " + pads.ToString() + ", chars " + chars.ToString() + ", shorts " + shorts.ToString());
			System.IO.File.WriteAllText("C:\\Users\\PkR\\Desktop\\sb_.txt", sb.ToString());
			*/
			return offsets;
		}
	}
}
