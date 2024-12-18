using System;
using System.Collections.Generic;

namespace SAModel.XJ
{
	public class XJMesh
	{
		private NJS_MATERIAL _mat;
		
		public int mat2_08; //These might just be padding since all mat types seem to be 0xC
		public int mat3_04;
		public int mat3_08;
		public int mat4_00;
		public int mat4_04;
		public int mat4_08;
		public int mat5_04;
		public int mat5_08;
		public int mat6_00;
		public int mat6_04;
		public int mat6_08;

		private byte[] _stripIndices;

		public XJMesh(byte[] file, uint address, uint imageBase)
		{
			var materialOffset = ByteConverter.ToUInt32(file, (int)address);
			var materialStructCount = ByteConverter.ToUInt32(file, (int)address + 0x4);
			var indexListOffset = ByteConverter.ToUInt32(file, (int)address + 0x8);
			var indexCount = ByteConverter.ToUInt32(file, (int)address + 0xC);
			var int_0x10 = ByteConverter.ToUInt32(file, (int)address + 0x10);

			ReadMaterial(file, materialOffset - imageBase, materialStructCount);
			ReadStrips(file, indexListOffset - imageBase, indexCount);
		}

		public static byte[] GetBytes(uint imageBase, bool dx, Dictionary<string, uint> labels, List<uint> njOffsets, List<XJMesh> xjMeshes, out uint address)
		{
			var result = new List<byte>();
			var matOffsetList = new List<int>();
			var stripOffsetList = new List<int>();

			address = (uint)result.Count;
			
			foreach (var mesh in xjMeshes)
			{
				matOffsetList.Add(result.Count);
				stripOffsetList.Add(result.Count + 0x8);
				njOffsets.Add((uint)(result.Count + imageBase));
				njOffsets.Add((uint)(result.Count + imageBase + 0x8));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(3));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(mesh._stripIndices.Length / 2));
				result.AddRange(ByteConverter.GetBytes(0));
			}
			
			result.Align(0x10);

			for(var i = 0; i < xjMeshes.Count; i++)
			{
				var mesh = xjMeshes[i];
				
				// Material
				result.SetByteListInt(matOffsetList[i], (int)(result.Count + imageBase));

				// Blend types
				result.AddRange(ByteConverter.GetBytes(2));
				result.AddRange(ByteConverter.GetBytes((uint)mesh._mat.SourceAlpha));
				result.AddRange(ByteConverter.GetBytes((uint)mesh._mat.DestinationAlpha));
				result.AddRange(ByteConverter.GetBytes(0));
				
				// Texture ID
				result.AddRange(ByteConverter.GetBytes(3));
				result.AddRange(ByteConverter.GetBytes((uint)mesh._mat.TextureID));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(0));
				
				// Diffuse color
				result.AddRange(ByteConverter.GetBytes(5));
				result.AddRange(VColor.GetBytes(mesh._mat.DiffuseColor, ColorType.RGBA8888_32));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(0));
				result.Align(0x10);

				// Strips
				result.SetByteListInt(stripOffsetList[i], (int)(result.Count + imageBase));
				result.AddRange(mesh._stripIndices);
				result.Align(0x10);
			}

			return result.ToArray();
		}

		private void ReadStrips(byte[] file, uint address, uint indexCount)
		{
			var curAddress = (int)address;
			_stripIndices = new byte[indexCount * 2];
			Array.Copy(file, address, _stripIndices, 0, indexCount * 2);
		}

		//Nobody ever worked out the real way to read these types of strips so these will be returned double-sided
		private List<Poly> GetTriangles()
		{
			var tris = new List<Poly>();
			
			for (var i = 0; i < _stripIndices.Length - 4; i += 2)
			{
				var a = ByteConverter.ToUInt16(_stripIndices, i);
				var b = ByteConverter.ToUInt16(_stripIndices, i + 2);
				var c = ByteConverter.ToUInt16(_stripIndices, i + 4);

				if(a == b || b == c || c == a)
				{
					continue;
				}

				tris.Add(new Triangle(a, b, c));
				tris.Add(new Triangle(a, c, b));
			}

			return tris;
		}

		private void ReadMaterial(byte[] file, uint address, uint materialStructCount)
		{
			var newMat = new NJS_MATERIAL();
			var curAddress = (int)address;
			
			for (var i = 0; i < materialStructCount; i++)
			{
				var type = ByteConverter.ToUInt32(file, curAddress);
				curAddress += 4;
				
				switch(type)
				{
					case 2:
						newMat.SourceAlpha = (AlphaInstruction)ByteConverter.ToUInt32(file, curAddress);
						curAddress += 4;
						newMat.DestinationAlpha = (AlphaInstruction)ByteConverter.ToUInt32(file, curAddress);
						curAddress += 4;
						mat2_08 = ByteConverter.ToInt32(file, curAddress);
						break;
					case 3:
						newMat.TextureID = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat3_04 = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat3_08 = ByteConverter.ToInt32(file, curAddress);
						break;
					case 4:
						mat4_00 = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat4_04 = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat4_08 = ByteConverter.ToInt32(file, curAddress);
						break;
					case 5:
						newMat.DiffuseColor = VColor.FromBytes(file, (int)address, ColorType.RGBA8888_32);
						mat5_04 = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat5_08 = ByteConverter.ToInt32(file, curAddress);
						break;
					case 6:
						mat6_00 = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat6_04 = ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						mat6_08 = ByteConverter.ToInt32(file, curAddress);
						break;
					default:
						ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						ByteConverter.ToInt32(file, curAddress);
						curAddress += 4;
						ByteConverter.ToInt32(file, curAddress);
						//Debug.WriteLine($"Unexpected xj material type {type} at {(curAddress - 4).ToString("X")}");
						break;
				}

				curAddress += 4;
			}

			_mat = newMat;
		}

		public MeshInfo Process(List<VertexData> verts, bool hasNormal, bool hasColors)
		{
			var polys = GetTriangles();
			return new MeshInfo(new NJS_MATERIAL(_mat), polys.ToArray(), verts.ToArray(), hasNormal, hasColors);
		}
	}
}
