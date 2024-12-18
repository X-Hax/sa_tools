using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
		private readonly uint _flags;

		/// <summary>
		/// The seperate sets of vertex data in this attach
		/// </summary>
		private readonly List<XjVertexSet> _vertexData = [];

		/// <summary>
		/// The meshes with opaque rendering properties
		/// </summary>
		private readonly List<XJMesh> _opaqueMeshes = [];

		/// <summary>
		/// The meshes with translucent rendering properties
		/// </summary>
		private readonly List<XJMesh> _translucentMeshes = [];

		/// <summary>
		/// Reads a XJ attach from a file
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The address at which the attach is located</param>
		/// <param name="imageBase">The imagebase of the file</param>
		public XJAttach(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}
		
		public XJAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			if (labels.TryGetValue(address, out var name))
			{
				Name = name;
			}
			else
			{
				Name = $"attach_{address:X8}";
			}

			_flags = ByteConverter.ToUInt32(file, address);
			
			var vertexSetOffset = ByteConverter.ToUInt32(file, address + 0x4) - imageBase;
			var vertexSetCount = ByteConverter.ToUInt32(file, address + 0x8); //Should always be 1?
			var opaqueMeshesOffset = ByteConverter.ToUInt32(file, address + 0xC) - imageBase;
			var opaqueMeshesCount = ByteConverter.ToUInt32(file, address + 0x10);
			var translucentMeshesOffset = ByteConverter.ToUInt32(file, address + 0x14) - imageBase;
			var translucentMeshesCount = ByteConverter.ToUInt32(file, address + 0x18);
			
			Bounds = new BoundingSphere(file, address + 0x1C);

			for (var i = 0; i < vertexSetCount; i++)
			{
				_vertexData.Add(new XjVertexSet(file, (int)(vertexSetOffset + 0x10 * i), imageBase));
			}

			for (var i = 0; i < opaqueMeshesCount; i++)
			{
				_opaqueMeshes.Add(new XJMesh(file, (uint)(opaqueMeshesOffset + 0x14 * i), imageBase));
			}

			for (var i = 0; i < translucentMeshesCount; i++)
			{
				_translucentMeshes.Add(new XJMesh (file, (uint)(translucentMeshesOffset + 0x14 * i), imageBase));
			}
		}

		public override byte[] GetBytes(uint imageBase, bool dx, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			List<byte> result = [];
			List<uint> vDataAddresses = [];

			foreach(var vData in _vertexData)
			{
				result.AddRange(vData.GetBytes((uint)(imageBase + result.Count), dx, labels, njOffsets, out var vDataAddress));
				vDataAddresses.Add(vDataAddress + imageBase);
			}
			
			result.Align(0x10);
			var curCount = result.Count;
			
			result.AddRange(XJMesh.GetBytes((uint)(imageBase + result.Count), dx, labels, njOffsets, _opaqueMeshes, out var opaqueAddress));
			opaqueAddress += (uint)curCount;
			result.Align(0x10);
			curCount = result.Count;
			result.AddRange(XJMesh.GetBytes((uint)(imageBase + result.Count), dx, labels, njOffsets, _translucentMeshes, out var translucentAddress));
			translucentAddress += (uint)curCount;
			result.Align(0x10);

			address = (uint)result.Count;
			
			njOffsets.Add((uint)(imageBase + result.Count + 0x4));
			njOffsets.Add((uint)(imageBase + result.Count + 0xC));
			njOffsets.Add((uint)(imageBase + result.Count + 0x14));
			
			result.AddRange(ByteConverter.GetBytes(_flags));
			result.AddRange(ByteConverter.GetBytes(vDataAddresses[0]));
			result.AddRange(ByteConverter.GetBytes(vDataAddresses.Count));
			result.AddRange(ByteConverter.GetBytes(opaqueAddress + imageBase));
			result.AddRange(ByteConverter.GetBytes(_opaqueMeshes.Count));
			result.AddRange(ByteConverter.GetBytes(translucentAddress + imageBase));
			result.AddRange(ByteConverter.GetBytes(_translucentMeshes.Count));
			result.AddRange(Bounds.GetBytes());
			result.Align(0x10);

			return result.ToArray();
		}

		public override void ProcessVertexData()
		{
			List<MeshInfo> meshInfo = [];
			List<List<VertexData>> verts = [];
			List<bool> hasNormals = [];
			List<bool> hasColors = [];
			List<bool> hasUVs = [];

			if (_vertexData != null)
			{
				for (var vd = 0; vd < _vertexData.Count; vd++) //There should only be 1 of these, but in case there's not...
				{
					var vertData = _vertexData[vd];
					List<VertexData> vertSet = [];

					hasNormals.Add(vertData.Normals.Count > 0);
					hasColors.Add(vertData.Colors.Count > 0);
					hasUVs.Add(vertData.UVs.Count > 0);
					for (var i = 0; i < vertData.Positions.Count; i++)
					{
						var normal = hasNormals[vd] ? vertData.Normals[i] : new Vertex(0, 1, 0);
						var color = hasColors[vd] ? vertData.Colors[i] : System.Drawing.Color.White;
						var uv = hasUVs[vd] ? vertData.UVs[i] : new UV(0, 0);
						
						vertSet.Add(new VertexData(vertData.Positions[i], normal, color, uv));
					}
					verts.Add(vertSet);
				}
			}

			meshInfo.AddRange(_opaqueMeshes.Select(m => m.Process(verts[0], hasNormals[0], hasColors[0])));
			meshInfo.AddRange(_translucentMeshes.Select(m => m.Process(verts[0], hasNormals[0], hasColors[0])));

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
		public override string ToStruct(bool dx)
		{
			throw new NotImplementedException();
		}

		public override void ToStructVariables(TextWriter writer, bool dx, List<string> labels, string[] textures = null)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
