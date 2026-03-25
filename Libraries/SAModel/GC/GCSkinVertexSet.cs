using System;
using System.Collections.Generic;
using System.IO;
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

		public string DataNamePos;
		public string DataNameWeight;

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

			var weightType = elementType switch
			{
				GCSkinAttribute.StaticWeight => "Static",
				GCSkinAttribute.PartialWeightStart => "Start",
				GCSkinAttribute.PartialWeight => "Middle",
				GCSkinAttribute.WeightStructEndMarker => "End",
				_ => null
			};
			switch (elementType)
			{
				case GCSkinAttribute.StaticWeight:
					if (labels.TryGetValue(positionNormalsOffset, out var wPosName))
					{
						DataNamePos = wPosName;
					}
					else
					{
						DataNamePos = $"weightpoint_{positionNormalsOffset:X8}";
					}
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
					if (labels.TryGetValue(positionNormalsOffset, out var wPosPWName))
					{
						DataNamePos = wPosPWName;
					}
					else
					{
						DataNamePos = $"weightpoint_{positionNormalsOffset:X8}";
					}
					if (labels.TryGetValue(weightsOffset, out var wDataName))
					{
						DataNameWeight = wDataName;
					}
					else
					{
						DataNameWeight = $"weightdata_{weightsOffset:X8}";
					}
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
							vertIndex = ByteConverter.ToUInt16(file, (int)weightsOffset + (i * 0x4)),
							weight = ByteConverter.ToUInt16(file, (int)weightsOffset + (i * 0x4) + 0x2),
						});
					}
					break;
				case GCSkinAttribute.WeightStructEndMarker:
					break;
				default:
					throw new System.Exception($"Bad GCSkinVertexSet type {elementType:X}");
			}
		}
		public byte[] GetBytes()
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
			result.Append(", ");
			result.Append(indexCount);
			result.Append(", ");
			result.Append(posNrms.Count > 0 ? DataNamePos: "NULL");
			result.Append(", ");
			result.Append(weightData.Count > 0  ? DataNameWeight : "NULL");
			result.Append(" }");

			return result.ToString();
		}
		public void ToNJA(TextWriter writer)
		{
			switch (elementType)
			{
				case GCSkinAttribute.StaticWeight:
					writer.WriteLine($"GJWPOINT      {DataNamePos}[]");
					writer.WriteLine("START");
					foreach (var vtx in posNrms)
					{
						vtx.ToNJA(writer);
					}
					writer.WriteLine($"END{Environment.NewLine}");
					break;
				case GCSkinAttribute.PartialWeightStart:
				case GCSkinAttribute.PartialWeight:
					var weightmax = 0;
					foreach (var weights in weightData)
					{
						if (weights.weight > 255)
							weightmax++;
					}
					writer.WriteLine($"GJWPOINT      {DataNamePos}[]");
					writer.WriteLine("START");
					foreach (var vtx in posNrms)
					{
						vtx.ToNJA(writer);
					}
					writer.WriteLine($"END{Environment.NewLine}");
					writer.WriteLine($"GJWDATA      {DataNameWeight}[]");
					writer.WriteLine("START");
					foreach (var wght in weightData)
					{
						wght.ToNJA(writer, weightmax != 0);
					}
					writer.WriteLine($"END{Environment.NewLine}");
					break;
			}
		}
		public void RefToNJA(TextWriter writer)
		{
			var weightType = elementType switch
			{
				GCSkinAttribute.StaticWeight => "GJ_WA_NONE",
				GCSkinAttribute.PartialWeightStart => "GJ_WA_START",
				GCSkinAttribute.PartialWeight => "GJ_WA_MIDDLE",
				GCSkinAttribute.WeightStructEndMarker => "GJ_WA_END",
				_ => null
			};
			writer.WriteLine("ATTRSTART");
			writer.WriteLine($"\tGJWAttr      {weightType},");
			writer.WriteLine($"\tGJWSize      {totalVertIndices},");
			writer.WriteLine($"\tGJWStIdx     {startingIndex},");
			writer.WriteLine($"\tGJWCount     {indexCount},");
			if (posNrms.Count > 0)
				writer.WriteLine($"\tGJWPNPtr     {DataNamePos},");
			else
				writer.WriteLine($"\tGJWPNPtr     NULL,");
			if (weightData.Count > 0)
				writer.WriteLine($"\tGJWIdxPtr    {DataNameWeight},");
			else
				writer.WriteLine($"\tGJWIdxPtr    NULL,");
			writer.WriteLine($"ATTREND{Environment.NewLine}");
		}
	}
}
