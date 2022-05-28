using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModel.XJ
{
	public class XJVertexSet
	{
		public List<Vertex> Positions = new List<Vertex>();
		public List<Vertex> Normals = new List<Vertex>();
		public List<Color> Colors = new List<Color>();
		public List<UV> UVs = new List<UV>();

		public ushort VertexType;
		public ushort Ushort_02;
		public uint VertexSize;
		public XJVertexSet(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}
		public XJVertexSet(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			VertexType = ByteConverter.ToUInt16(file, address);
			Ushort_02 = ByteConverter.ToUInt16(file, address + 0x2);
			var vertListOffset = ByteConverter.ToUInt32(file, address + 0x4);
			VertexSize = ByteConverter.ToUInt32(file, address + 0x8);
			var vertCount = ByteConverter.ToUInt32(file, address + 0xC);

			bool hasUV = (VertexType & 0x1) > 0;
			bool hasNormal = (VertexType & 0x2) > 0;
			bool hasColor = (VertexType & 0x4) > 0;
			bool hasUnk0 = (VertexType & 0x8) > 0;
			bool hasUnk1 = (VertexType & 0x10) > 0;
			bool hasUnk2 = (VertexType & 0x20) > 0;
			bool hasUnk3 = (VertexType & 0x40) > 0;
			bool hasUnk4 = (VertexType & 0x80) > 0;

			ushort calcedVertSize = 0xC;
			if (hasNormal)
				calcedVertSize += 0xC;
			if (hasColor)
				calcedVertSize += 0x4;
			if (hasUV)
				calcedVertSize += 0x8;

			if(VertexSize != calcedVertSize)
			{
				throw new Exception($"Vertsize {VertexSize} is not equal to Calculated Vertsize {calcedVertSize}");
			}

			address = (int)(vertListOffset - imageBase);
			for(int i = 0; i < vertCount; i++)
			{
				Positions.Add(new Vertex(file, address));
				address += 0xC;

				if (hasNormal)
				{
					Normals.Add(new Vertex(file, address));
					address += 0xC;
				}
				if (hasColor)
				{
					Colors.Add(VColor.FromBytes(file, address, ColorType.RGBA8888_32));
					address += 0x4;
				}
				if (hasUV)
				{
					UVs.Add(new UV(file, address, false, false, true));
					address += 0x8;
				}
			}
		}

		public byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			bool hasUV = (VertexType & 0x1) > 0;
			bool hasNormal = (VertexType & 0x2) > 0;
			bool hasColor = (VertexType & 0x4) > 0;
			bool hasUnk0 = (VertexType & 0x8) > 0;
			bool hasUnk1 = (VertexType & 0x10) > 0;
			bool hasUnk2 = (VertexType & 0x20) > 0;
			bool hasUnk3 = (VertexType & 0x40) > 0;
			bool hasUnk4 = (VertexType & 0x80) > 0;

			List<byte> result = new List<byte>();

			for (int i = 0; i < Positions.Count; i++)
			{
				result.AddRange(Positions[i].GetBytes());
				if(hasNormal)
					result.AddRange(Normals[i].GetBytes());
				if(hasColor)
					result.AddRange(VColor.GetBytes(Colors[i], ColorType.RGBA8888_32));
				if(hasUV)
					result.AddRange(UVs[i].GetBytesXJ());
			}
			result.Align(0x10);

			address = (uint)(result.Count);
			njOffsets.Add((uint)result.Count + imageBase + 4);
			result.AddRange(ByteConverter.GetBytes(VertexType));
			result.AddRange(ByteConverter.GetBytes(Ushort_02));
			result.AddRange(ByteConverter.GetBytes(imageBase));
			result.AddRange(ByteConverter.GetBytes(VertexSize));
			result.AddRange(ByteConverter.GetBytes(Positions.Count));

			result.Align(0x10);

			return result.ToArray();
		}
	}
}
