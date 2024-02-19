using System.Collections.Generic;

namespace SAModel.GC
{
	public class GCSkinVertexSetElement
	{
		//Both the data and the mesh struct here are 0x20 aligned
		public ushort elementType;
		/// <summary>
		/// 3 * indexCount if elementType 0, 4 * indexCount if 1 or 2.
		/// Data assumedly vert positions, normals, and uvs. Weights would optionally be the 4th.
		/// </summary>
		public ushort totalVertIndices;
		public ushort startingIndex;
		public ushort indexCount;
		public int positionNormalsOffset;
		public int weightsOffset;

		public string DataName;

		public List<GCSkinVertexSetPosNrm> posNrms = new List<GCSkinVertexSetPosNrm>();
		public List<GCSkinVertexSetWeight> weightData = new List<GCSkinVertexSetWeight>();

		public GCSkinVertexSetElement() { }

		public GCSkinVertexSetElement(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			elementType = ByteConverter.ToUInt16(file, (int)address + 0x0);
			totalVertIndices = ByteConverter.ToUInt16(file, (int)address + 0x2);
			startingIndex = ByteConverter.ToUInt16(file, (int)address + 0x4);
			indexCount = ByteConverter.ToUInt16(file, (int)address + 0x6);
			positionNormalsOffset = (int)(ByteConverter.ToInt32(file, (int)address + 0x8) - imageBase);
			weightsOffset = (int)(ByteConverter.ToInt32(file, (int)address + 0xC) - imageBase);

			DataName = $"vertexSkin_{elementType}_" + address.ToString("X8");

			switch (elementType)
			{
				case 0:
					for (int i = 0; i < indexCount; i++)
					{
						posNrms.Add(new GCSkinVertexSetPosNrm()
						{
							posX = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x0),
							posY = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x2),
							posZ = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x4),
							nrmX = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x6),
							nrmY = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x8),
							nrmZ = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0xA),
						});
					}
					break;
				case 1:
					for (int i = 0; i < indexCount; i++)
					{
						posNrms.Add(new GCSkinVertexSetPosNrm()
						{
							posX = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x0),
							posY = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x2),
							posZ = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x4),
							nrmX = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x6),
							nrmY = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x8),
							nrmZ = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0xA),
						});
					}
					for (int i = 0; i < indexCount; i++)
					{
						weightData.Add(new GCSkinVertexSetWeight()
						{
							vertIndex = ByteConverter.ToInt16(file, (int)weightsOffset + (i * 0x4) + 0x0),
							weight = ByteConverter.ToInt16(file, (int)weightsOffset + (i * 0x4) + 0x2),
						});
					}
					break;
				case 2:
					for (int i = 0; i < indexCount; i++)
					{
						posNrms.Add(new GCSkinVertexSetPosNrm()
						{
							posX = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x0),
							posY = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x2),
							posZ = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x4),
							nrmX = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x6),
							nrmY = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0x8),
							nrmZ = ByteConverter.ToInt16(file, (int)positionNormalsOffset + (i * 0xC) + 0xA),
						});
					}
					for (int i = 0; i < indexCount; i++)
					{
						weightData.Add(new GCSkinVertexSetWeight()
						{
							vertIndex = ByteConverter.ToInt16(file, (int)weightsOffset + (i * 0x4) + 0x0),
							weight = ByteConverter.ToInt16(file, (int)weightsOffset + (i * 0x4) + 0x2),
						});
					}
					break;
				case 3:
					break;
				default:
					throw new System.Exception($"Bad GCSkinVertexSetElement type {elementType:X}");
			}
		}
	}
}
