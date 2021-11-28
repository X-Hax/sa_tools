using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModel
{
	class POF0Helper
	{
		public static byte[] calcPOF0Pointer(int lastPOF, int currentAddress)
		{
			byte[] finalPOF;
			int offsetDiff = currentAddress - lastPOF;
			int offsetDiv = offsetDiff / 4;
			ByteConverter.BackupEndian();
			ByteConverter.BigEndian = true;

			if (offsetDiff > 0xFF)
			{
				if (offsetDiff > 0xFFFF)
				{
					finalPOF = ByteConverter.GetBytes(offsetDiv);
					finalPOF[0] += 0xC0;
				}
				else
				{
					short shortCalc = (short)(offsetDiv);
					finalPOF = ByteConverter.GetBytes(shortCalc);
					finalPOF[0] += 0x80;
				}
			}
			else
			{
				byte byteCalc = (byte)(offsetDiv);
				byteCalc += 0x40;
				finalPOF = new byte[] { byteCalc };
			}

			ByteConverter.RestoreEndian();
			return finalPOF;
		}

		public static void finalizePOF0(List<byte> pof0)
		{
			byte[] magic = { 0x50, 0x4F, 0x46, 0x30 };
			pof0.Align(4);
			pof0.InsertRange(0, ByteConverter.GetBytes(pof0.Count));
			pof0.InsertRange(0, magic);
		}

	}
}
