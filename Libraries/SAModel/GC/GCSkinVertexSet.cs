using System;
using System.Collections.Generic;
using System.Text;

namespace SAModel.GC
{
	/// <summary>
	/// A vertex weight data set
	/// </summary>
	[Serializable]
	public class GCSkinVertexSet
	{
		//Both the data and the mesh struct here are 0x20 aligned
		public GCSkinAttribute elementType;
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

		public GCSkinVertexSet() { }

		public GCSkinVertexSet(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			elementType = (GCSkinAttribute)ByteConverter.ToUInt16(file, (int)address);
			totalVertIndices = ByteConverter.ToUInt16(file, (int)address + 0x2);
			startingIndex = ByteConverter.ToUInt16(file, (int)address + 0x4);
			indexCount = ByteConverter.ToUInt16(file, (int)address + 0x6);
			positionNormalsOffset = (int)(ByteConverter.ToInt32(file, (int)address + 0x8) - imageBase);
			weightsOffset = (int)(ByteConverter.ToInt32(file, (int)address + 0xC) - imageBase);

			DataName = $"vertexSkin_{elementType}_" + address.ToString("X8");

			switch (elementType)
			{
				case GCSkinAttribute.StaticWeight:
					for (int i = 0; i < indexCount; i++)
					{
						posNrms.Add(new GCSkinVertexSetPosNrm()
						{
							pos = new Vector3Short(file, (int)positionNormalsOffset + (i * 0xC)),
							nrm = new Vector3Short(file, (int)positionNormalsOffset + (i * 0xC) + 0x6),
						});
					}
					break;
				case GCSkinAttribute.PartialWeightStart:
				case GCSkinAttribute.PartialWeight:
					for (int i = 0; i < indexCount; i++)
					{
						posNrms.Add(new GCSkinVertexSetPosNrm()
						{
							pos = new Vector3Short(file, (int)positionNormalsOffset + (i * 0xC)),
							nrm = new Vector3Short(file, (int)positionNormalsOffset + (i * 0xC) + 0x6),
						});
					}
					for (int i = 0; i < indexCount; i++)
					{
						weightData.Add(new GCSkinVertexSetWeight()
						{
							vertIndex = ByteConverter.ToInt16(file, (int)weightsOffset + (i * 0x4)),
							weight = ByteConverter.ToInt16(file, (int)weightsOffset + (i * 0x4) + 0x2),
						});
					}
					break;
				case GCSkinAttribute.WeightStructEndMarker:
					break;
				default:
					throw new System.Exception($"Bad GCSkinVertexSet type {elementType:X}");
			}
		}
		public byte[] GetBytes(uint dataAddress)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes((ushort)elementType));
			result.AddRange(ByteConverter.GetBytes(totalVertIndices));
			result.AddRange(ByteConverter.GetBytes(startingIndex));
			result.AddRange(ByteConverter.GetBytes(indexCount));
			result.AddRange(ByteConverter.GetBytes(positionNormalsOffset));
			result.AddRange(ByteConverter.GetBytes(weightsOffset));
			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");

			result.Append((ushort)elementType);
			result.Append(", ");
			result.Append(totalVertIndices);
			result.Append(", ");
			result.Append(startingIndex);
			result.Append(" }, {");
			result.Append(indexCount);
			result.Append(", ");
			result.Append(posNrms != null? DataName: "NULL");
			result.Append(", ");
			result.Append(weightData != null? weightsOffset : "NULL");
			result.Append(" }");

			return result.ToString();
		}
	}
}
