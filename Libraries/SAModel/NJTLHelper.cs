using System;
using System.Collections.Generic;
using System.Text;

namespace SAModel
{
	public class NJTLHelper
	{
		public static byte[] GenerateNJTexList(string[] texList, bool isGC)
		{
			List<byte> njTexList = new List<byte>();
			List<byte> njTLHeader = new List<byte>();
			List<byte> pof0List = new List<byte>();

			if (isGC)
			{
				njTLHeader.AddRange(new byte[] { 0x47, 0x4A, 0x54, 0x4C });
			}
			else
			{
				njTLHeader.AddRange(new byte[] { 0x4E, 0x4A, 0x54, 0x4C });
			}
			njTexList.AddRange(ByteConverter.GetBytes(0x8));
			njTexList.AddRange(ByteConverter.GetBytes(texList.Length));

			int offset = texList.Length * 0xC + 0x8;
			for (int i = 0; i < texList.Length; i++)
			{
				if (i > 0)
				{
					offset += texList[i].Length + 1;
				}
				njTexList.AddRange(ByteConverter.GetBytes(offset));
				njTexList.AddRange(ByteConverter.GetBytes(0));
				njTexList.AddRange(ByteConverter.GetBytes(0));
			}
			for (int i = 0; i < texList.Length; i++)
			{
				njTexList.AddRange(Encoding.ASCII.GetBytes(texList[i]));
				njTexList.Add(0);
			}
			njTexList.Align(0x4);

			njTLHeader.AddRange(BitConverter.GetBytes(njTexList.Count));

			pof0List.Add(0x40);
			pof0List.Add(0x42);
			for (int i = 1; i < texList.Length; i++)
			{
				pof0List.Add(0x43);
			}
			pof0List.Align(4);

			int pofLength = pof0List.Count;
			byte[] magic = { 0x50, 0x4F, 0x46, 0x30 };

			pof0List.InsertRange(0, BitConverter.GetBytes(pofLength));
			pof0List.InsertRange(0, magic);

			njTexList.InsertRange(0, njTLHeader.ToArray());
			njTexList.AddRange(pof0List.ToArray());

			return njTexList.ToArray();
		}

	}
}
