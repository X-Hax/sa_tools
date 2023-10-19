using System;
using System.Collections.Generic;
using System.IO;

namespace SAModel.XJ
{
	/// <summary>
	/// An attach/mesh using the Xbox/PC format
	/// </summary>
	public class XJAttach : Attach
	{
		/// <summary>
		/// Flags for the XJ Attach
		/// </summary>
		public uint flags;

		/// <summary>
		/// The seperate sets of vertex data in this attach
		/// </summary>
		public readonly List<XJVertexSet> vertexData = new List<XJVertexSet>();

		/// <summary>
		/// The meshes with opaque rendering properties
		/// </summary>
		public readonly List<XJMesh> opaqueMeshes = new List<XJMesh>();

		/// <summary>
		/// The meshes with translucent rendering properties
		/// </summary>
		public readonly List<XJMesh> translucentMeshes = new List<XJMesh>();

		/// <summary>
		/// Create a new empty XJ attach
		/// </summary>
		public XJAttach()
		{
			Name = "xjattach_" + Extensions.GenerateIdentifier();

			flags = 0;
			vertexData = new List<XJVertexSet>();
			opaqueMeshes = new List<XJMesh>();
			translucentMeshes = new List<XJMesh>();
			Bounds = new BoundingSphere();
		}

		/// <summary>
		/// Reads a XJ attach from a file
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The address at which the attach is located</param>
		/// <param name="imageBase">The imagebase of the file</param>
		/// <param name="labels">The labels of the file</param>
		/// 
		public XJAttach(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}
		public XJAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "attach_" + address.ToString("X8");
			flags = ByteConverter.ToUInt32(file, address);
			uint vertexSetOffset = ByteConverter.ToUInt32(file, address + 0x4) - imageBase;
			uint vertexSetCount = ByteConverter.ToUInt32(file, address + 0x8); //Should always be 1?
			uint opaqueMeshesOffset = ByteConverter.ToUInt32(file, address + 0xC) - imageBase;
			uint opaqueMeshesCount = ByteConverter.ToUInt32(file, address + 0x10);
			uint translucentMeshesOffset = ByteConverter.ToUInt32(file, address + 0x14) - imageBase;
			uint translucentMeshesCount = ByteConverter.ToUInt32(file, address + 0x18);
			Bounds = new BoundingSphere(file, address + 0x1C);

			for(int i = 0; i < vertexSetCount; i++)
			{
				vertexData.Add(new XJVertexSet(file, (int)(vertexSetOffset + (0x10 * i)), imageBase));
			}

			for (int i = 0; i < opaqueMeshesCount; i++)
			{
				opaqueMeshes.Add(new XJMesh(file, (uint)(opaqueMeshesOffset + (0x14 * i)), imageBase));
			}

			for (int i = 0; i < translucentMeshesCount; i++)
			{
				translucentMeshes.Add(new XJMesh (file, (uint)(translucentMeshesOffset + (0x14 * i)), imageBase));
			}
		}

		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			List<byte> result = new List<byte>();
			List<uint> vdataAddresses = new List<uint>();

			foreach(var vdata in vertexData)
			{
				result.AddRange(vdata.GetBytes((uint)(imageBase + result.Count), DX, labels, njOffsets, out uint vdataAddress));
				vdataAddresses.Add(vdataAddress + imageBase);
			}
			result.Align(0x10);
			int curCount = result.Count;
			result.AddRange(XJMesh.GetBytes((uint)(imageBase + result.Count), DX, labels, njOffsets, opaqueMeshes, out uint opaqueAddress));
			opaqueAddress += (uint)curCount;
			result.Align(0x10);
			curCount = result.Count;
			result.AddRange(XJMesh.GetBytes((uint)(imageBase + result.Count), DX, labels, njOffsets, translucentMeshes, out uint translucentAddress));
			translucentAddress += (uint)curCount;
			result.Align(0x10);

			address = (uint)(result.Count);
			njOffsets.Add((uint)(imageBase + result.Count + 0x4));
			njOffsets.Add((uint)(imageBase + result.Count + 0xC));
			njOffsets.Add((uint)(imageBase + result.Count + 0x14));
			result.AddRange(ByteConverter.GetBytes(flags));
			result.AddRange(ByteConverter.GetBytes(vdataAddresses[0]));
			result.AddRange(ByteConverter.GetBytes(vdataAddresses.Count));
			result.AddRange(ByteConverter.GetBytes(opaqueAddress + imageBase));
			result.AddRange(ByteConverter.GetBytes(opaqueMeshes.Count));
			result.AddRange(ByteConverter.GetBytes(translucentAddress + imageBase));
			result.AddRange(ByteConverter.GetBytes(translucentMeshes.Count));
			result.AddRange(Bounds.GetBytes());
			result.Align(0x10);

			return result.ToArray();
		}

		public override void ProcessVertexData()
		{
			List<MeshInfo> meshInfo = new List<MeshInfo>();
			List<List<VertexData>> verts = new List<List<VertexData>>();
			List<bool> hasNormals = new List<bool>();
			List<bool> hasColors = new List<bool>();
			List<bool> hasUVs = new List<bool>();

			if (vertexData != null)
			{
				for(int vd = 0; vd < vertexData.Count; vd++) //There should only be 1 of these, but in case there's not...
				{
					var vertData = vertexData[vd];
					List<VertexData> vertSet = new List<VertexData>();

					hasNormals.Add(vertData.Normals.Count > 0);
					hasColors.Add(vertData.Colors.Count > 0);
					hasUVs.Add(vertData.UVs.Count > 0);
					for(int i = 0; i < vertData.Positions.Count; i++)
					{
						var normal = hasNormals[vd] ? vertData.Normals[i] : new Vertex(0, 1, 0);
						var color = hasColors[vd] ? vertData.Colors[i] : System.Drawing.Color.White;
						var uv = hasUVs[vd] ? vertData.UVs[i] : new UV(0, 0);
						vertSet.Add(new VertexData(vertData.Positions[i], normal, color, uv));
					}
					verts.Add(vertSet);
				}
			}

			foreach(XJMesh m in opaqueMeshes)
				meshInfo.Add(m.Process(verts[0], hasNormals[0], hasColors[0]));

			foreach (XJMesh m in translucentMeshes)
				meshInfo.Add(m.Process(verts[0], hasNormals[0], hasColors[0]));

			MeshInfo = meshInfo.ToArray();
		}

		#region Unused
		public override Attach Clone()
		{
			throw new NotImplementedException();
		}

		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, float frame, int animindex)
		{
			throw new NotImplementedException();
		}
		public override string ToStruct(bool DX)
		{
			throw new NotImplementedException();
		}

		public override void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures = null)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
