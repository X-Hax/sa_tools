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

		public VertexAttribute(byte[] file, uint address, uint imageBase)
		{
			// They skipped the first 8 attributes when they wrote the files,
			// so we need to add 8 to get the proper attribute enum value.
			Attribute = (GXVertexAttribute)(file[address] + 8);

			FractionalBitCount = file[address + 1];
			Unknown1 = ByteConverter.ToInt16(file, (int)(address + 2));
			DataType = (GXDataType)(file[address + 4] >> 4);
			ComponentCount = (GXComponentCount)(file[address + 4] & 0x0F);

			DataOffset = (int)(ByteConverter.ToInt32(file, (int)(address + 8)) - imageBase);
			DataCount = ByteConverter.ToInt32(file, (int)(address + 0xC));
			DataCount /= CalculateComponentSize();
		}

		public int CalculateComponentSize()
		{
			int size = 0;
			int num_components = 1;

			switch (ComponentCount)
			{
				case GXComponentCount.Position_XY:
				case GXComponentCount.TexCoord_ST:
					num_components = 2;
					break;
				case GXComponentCount.Position_XYZ:
				case GXComponentCount.Normal_XYZ:
				case GXComponentCount.Color_RGB:
					num_components = 3;
					break;
				case GXComponentCount.Color_RGBA:
					num_components = 4;
					break;
			}

			switch (DataType)
			{
				case GXDataType.Unsigned8:
				case GXDataType.Signed8:
					size = num_components;
					break;
				case GXDataType.Unsigned16:
				case GXDataType.Signed16:
				case GXDataType.RGB565:
					size = num_components * 2;
					break;
				case GXDataType.Float32:
				case GXDataType.RGBA8:
					size = num_components * 4;
					break;
			}

			return size;
		}
	}
}
