using System;
using System.Collections.Generic;
using System.Drawing;

namespace SAModel.XJ
{
	public class XjVertexSet
	{
		public readonly List<Vertex> Positions = [];
		public readonly List<Vertex> Normals = [];
		public readonly List<Color> Colors = [];
		public readonly List<UV> UVs = [];

		private readonly ushort _vertexType;
		private readonly ushort _ushort02;
		private readonly uint _vertexSize;
		
		public XjVertexSet(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}

		private XjVertexSet(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			_vertexType = ByteConverter.ToUInt16(file, address);
			_ushort02 = ByteConverter.ToUInt16(file, address + 0x2);
			var vertListOffset = ByteConverter.ToUInt32(file, address + 0x4);
			_vertexSize = ByteConverter.ToUInt32(file, address + 0x8);
			var vertCount = ByteConverter.ToUInt32(file, address + 0xC);

			var hasUv = (_vertexType & 0x1) > 0;
			var hasNormal = (_vertexType & 0x2) > 0;
			var hasColor = (_vertexType & 0x4) > 0;
			var hasUnk0 = (_vertexType & 0x8) > 0;
			var hasUnk1 = (_vertexType & 0x10) > 0;
			var hasUnk2 = (_vertexType & 0x20) > 0;
			var hasUnk3 = (_vertexType & 0x40) > 0;
			var hasUnk4 = (_vertexType & 0x80) > 0;

			ushort calcedVertSize = 0xC;
			if (hasNormal)
			{
				calcedVertSize += 0xC;
			}

			if (hasColor)
			{
				calcedVertSize += 0x4;
			}

			if (hasUv)
			{
				calcedVertSize += 0x8;
			}

			if (_vertexSize != calcedVertSize)
			{
				throw new Exception($"Vertsize {_vertexSize} is not equal to Calculated Vertsize {calcedVertSize}");
			}

			address = (int)(vertListOffset - imageBase);
			
			for (var i = 0; i < vertCount; i++)
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
				
				if (hasUv)
				{
					UVs.Add(new UV(file, address, false, false, true));
					address += 0x8;
				}
			}
		}

		public byte[] GetBytes(uint imageBase, bool dx, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			var hasUv = (_vertexType & 0x1) > 0;
			var hasNormal = (_vertexType & 0x2) > 0;
			var hasColor = (_vertexType & 0x4) > 0;
			var hasUnk0 = (_vertexType & 0x8) > 0;
			var hasUnk1 = (_vertexType & 0x10) > 0;
			var hasUnk2 = (_vertexType & 0x20) > 0;
			var hasUnk3 = (_vertexType & 0x40) > 0;
			var hasUnk4 = (_vertexType & 0x80) > 0;

			List<byte> result = [];

			for (var i = 0; i < Positions.Count; i++)
			{
				result.AddRange(Positions[i].GetBytes());
				
				if(hasNormal)
				{
					result.AddRange(Normals[i].GetBytes());
				}

				if(hasColor)
				{
					result.AddRange(VColor.GetBytes(Colors[i], ColorType.RGBA8888_32));
				}

				if(hasUv)
				{
					result.AddRange(UVs[i].GetBytesXJ());
				}
			}
			
			result.Align(0x10);

			address = (uint)result.Count;
			njOffsets.Add((uint)result.Count + imageBase + 4);
			result.AddRange(ByteConverter.GetBytes(_vertexType));
			result.AddRange(ByteConverter.GetBytes(_ushort02));
			result.AddRange(ByteConverter.GetBytes(imageBase));
			result.AddRange(ByteConverter.GetBytes(_vertexSize));
			result.AddRange(ByteConverter.GetBytes(Positions.Count));

			result.Align(0x10);

			return result.ToArray();
		}
	}
}
