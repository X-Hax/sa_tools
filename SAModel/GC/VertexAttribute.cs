using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public struct VertexAttribute
	{
        public GXVertexAttribute Attribute { get; set; }
        public GXDataType DataType { get; set; }
		public GXComponentCount ComponentCount { get; set; }
		public byte FractionalBitCount { get; set; }
		public short Unknown1 { get; set; }

		public int DataOffset { get; private set; }
		public int DataCount { get; private set; }

		public VertexAttribute(byte[] file, int address, uint imageBase)
		{
			// They skipped the first 8 attributes when they wrote the files,
			// so we need to add 8 to get the proper attribute enum value.
			Attribute = (GXVertexAttribute)(file[address] + 8);

			FractionalBitCount = file[address + 1];
			Unknown1 = ByteConverter.ToInt16(file, address + 2);
			DataType = (GXDataType)(file[address + 4] >> 4);
			ComponentCount = (GXComponentCount)(file[address + 4] & 0x0F);

			DataOffset = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			DataCount = ByteConverter.ToInt32(file, address + 0xC);
		}
	}
}
