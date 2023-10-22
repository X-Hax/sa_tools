using System;
using System.Collections.Generic;

namespace SAModel
{
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

		public static byte[] calcPOF0Pointer(int lastPOF, int currentAddress)
		{
			byte[] finalPOF;
			int offsetDiff = currentAddress - lastPOF;
			int offsetDiv = offsetDiff / 4;
			bool storedBE = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;
			if (offsetDiff > 0xFF)
			{
				// Short
				if (offsetDiff > 0xFFFF)
				{
					finalPOF = ByteConverter.GetBytes(offsetDiv);
					finalPOF[0] += 0xC0;
				}
				// Long
				else
				{
					short shortCalc = (short)(offsetDiv);
					finalPOF = ByteConverter.GetBytes(shortCalc);
					finalPOF[0] += 0x80;
				}
			}
			// Char
			else
			{
				byte byteCalc = (byte)(offsetDiv);
				byteCalc += 0x40;
				finalPOF = new byte[] { byteCalc };
			}

			ByteConverter.BigEndian = storedBE;
			return finalPOF;
		}

		public static byte[] calcPOF0Pointer(uint lastPOF, uint currentAddress)
		{
			return calcPOF0Pointer((int)lastPOF, (int)currentAddress);
		}

		public static void finalizePOF0(List<byte> pof0)
		{
			byte[] magic = { 0x50, 0x4F, 0x46, 0x30 };
			pof0.Align(4);
			pof0.InsertRange(0, BitConverter.GetBytes(pof0.Count));
			pof0.InsertRange(0, magic);
		}

		public static void FixPointersWithPOF(byte[] data, List<int> pointerList, int imgBase)
		{
            //System.Windows.Forms.MessageBox.Show(data.Length.ToString());
			int currentPos = 0;
			foreach (int pointer in pointerList)
			{
				currentPos += pointer;
				int oldPointer = ByteConverter.ToInt32(data, currentPos);
				if (oldPointer != 0)
					oldPointer += imgBase;
				byte[] newPointerBytes = ByteConverter.GetBytes(oldPointer);
                //System.Windows.Forms.MessageBox.Show("Fixing pointer at " + (currentPos).ToString("X") + ": " + oldPointer.ToString("X"));
				Array.Copy(newPointerBytes, 0, data, currentPos, 4);
			}
		}

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
			System.IO.File.WriteAllText("C:\\Users\\PkR\\Desktop\\sb.txt", sb.ToString());
			*/
			return offsets;
		}
	}
}
