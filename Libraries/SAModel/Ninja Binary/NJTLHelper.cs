using System;
using System.Collections.Generic;
using System.Text;

namespace SAModel
{
	public class NJTLHelper
	{
		public static byte[] GenerateNJTexList(string[] texList, bool isGC, bool sizeLittleEndian)
		{
			List<byte> njTexList = [];
			List<byte> njTLHeader = [];
			List<byte> pof0List = [];

			njTLHeader.AddRange(isGC ? "GJTL"u8 : "NTJL"u8);
			
			njTexList.AddRange(ByteConverter.GetBytes(0x8));
			njTexList.AddRange(ByteConverter.GetBytes(texList.Length));

			var offset = texList.Length * 0xC + 0x8;
			
			for (var i = 0; i < texList.Length; i++)
			{
				if (i > 0)
				{
					offset += texList[i - 1].Length + 1;
				}
				njTexList.AddRange(ByteConverter.GetBytes(offset));
				njTexList.AddRange(ByteConverter.GetBytes(0));
				njTexList.AddRange(ByteConverter.GetBytes(0));
			}
			
			foreach (var tex in texList)
			{
				njTexList.AddRange(Encoding.ASCII.GetBytes(tex));
				njTexList.Add(0);
			}
			
			njTexList.Align(0x4);

			njTLHeader.AddRange(BitConverter.GetBytes(njTexList.Count));

			pof0List.Add(0x40);
			pof0List.Add(0x42);
			
			for (var i = 1; i < texList.Length; i++)
			{
				pof0List.Add(0x43);
			}
			
			pof0List.Align(4);

			var pofLength = pof0List.Count;
			byte[] magic = [0x50, 0x4F, 0x46, 0x30];

			pof0List.InsertRange(0, sizeLittleEndian ? BitConverter.GetBytes(pofLength) : ByteConverter.GetBytes(pofLength));
			pof0List.InsertRange(0, magic);

			njTexList.InsertRange(0, njTLHeader.ToArray());
			njTexList.AddRange(pof0List.ToArray());

			return njTexList.ToArray();
		}
	}
}
