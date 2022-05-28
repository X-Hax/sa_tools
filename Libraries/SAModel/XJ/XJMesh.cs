using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAModel.XJ
{
	
	public class XJMesh
	{
		public NJS_MATERIAL mat;
		public int mat2_08; //These might just be padding sicne all mat types seem to be 0xC
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

		public byte[] stripIndices;

		public XJMesh(byte[] file, uint address, uint imageBase)
		{
			uint materialOffset = ByteConverter.ToUInt32(file, (int)address);
			uint materialStructCount = ByteConverter.ToUInt32(file, (int)address + 0x4);
			uint indexListOffset = ByteConverter.ToUInt32(file, (int)address + 0x8);
			uint indexCount = ByteConverter.ToUInt32(file, (int)address + 0xC);
			uint int_0x10 = ByteConverter.ToUInt32(file, (int)address + 0x10);

			ReadMaterial(file, materialOffset - imageBase, materialStructCount);
			ReadStrips(file, indexListOffset - imageBase, indexCount);
		}

		public static byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, List<XJMesh> xjMeshes, out uint address)
		{
			List<byte> result = new List<byte>();
			List<int> matOffsetList = new List<int>();
			List<int> stripOffsetList = new List<int>();

			address = (uint)result.Count;
			for (int i = 0; i < xjMeshes.Count; i++)
			{
				matOffsetList.Add(result.Count);
				stripOffsetList.Add(result.Count + 0x8);
				njOffsets.Add((uint)(result.Count + imageBase));
				njOffsets.Add((uint)(result.Count + imageBase + 0x8));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(3));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(xjMeshes[i].stripIndices.Length / 2));
				result.AddRange(ByteConverter.GetBytes(0));
			}
			result.Align(0x10);

			for(int i = 0; i < xjMeshes.Count; i++)
			{
				var mesh = xjMeshes[i];
				//Material
				result.SetByteListInt(matOffsetList[i], (int)(result.Count + imageBase));

				//Blend types
				result.AddRange(ByteConverter.GetBytes(2));
				result.AddRange(ByteConverter.GetBytes((uint)mesh.mat.SourceAlpha));
				result.AddRange(ByteConverter.GetBytes((uint)mesh.mat.DestinationAlpha));
				result.AddRange(ByteConverter.GetBytes(0));
				//Texture ID
				result.AddRange(ByteConverter.GetBytes(3));
				result.AddRange(ByteConverter.GetBytes((uint)mesh.mat.TextureID));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(0));
				//Diffuse color
				result.AddRange(ByteConverter.GetBytes(5));
				result.AddRange(VColor.GetBytes(mesh.mat.DiffuseColor, ColorType.RGBA8888_32));
				result.AddRange(ByteConverter.GetBytes(0));
				result.AddRange(ByteConverter.GetBytes(0));
				result.Align(0x10);

				//Strips
				result.SetByteListInt(stripOffsetList[i], (int)(result.Count + imageBase));
				result.AddRange(mesh.stripIndices);
				result.Align(0x10);
			}

			return result.ToArray();
		}

		public void ReadStrips(byte[] file, uint address, uint indexCount)
		{
			int curAddress = (int)address;
			stripIndices = new byte[indexCount * 2];
			Array.Copy(file, address, stripIndices, 0, indexCount * 2);
		}

		//Nobody ever worked out the real way to read these types of strips so these will be returned double sided
		public List<Poly> GetTriangles()
		{
			List<Poly> tris = new List<Poly>();
			for(int i = 0; i < stripIndices.Length - 4; i += 2)
			{
				var a = ByteConverter.ToUInt16(stripIndices, i);
				var b = ByteConverter.ToUInt16(stripIndices, i + 2);
				var c = ByteConverter.ToUInt16(stripIndices, i + 4);

				if(a == b || b == c || c == a)
				{
					continue;
				}

				tris.Add(new Triangle(a, b, c));
				tris.Add(new Triangle(a, c, b));
			}

			return tris;
		}

		public void ReadMaterial(byte[] file, uint address, uint materialStructCount)
		{
			NJS_MATERIAL newMat = new NJS_MATERIAL();
			int curAddress = (int)address;
			for(int i = 0; i < materialStructCount; i++)
			{
				uint type = ByteConverter.ToUInt32(file, curAddress); curAddress += 4;
				switch(type)
				{
					case 2:
						newMat.SourceAlpha = (AlphaInstruction)ByteConverter.ToUInt32(file, curAddress); curAddress += 4;
						newMat.DestinationAlpha = (AlphaInstruction)ByteConverter.ToUInt32(file, curAddress); curAddress += 4;
						mat2_08 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						break;
					case 3:
						newMat.TextureID = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat3_04 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat3_08 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						break;
					case 4:
						mat4_00 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat4_04 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat4_08 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						break;
					case 5:
						newMat.DiffuseColor = VColor.FromBytes(file, (int)address, ColorType.RGBA8888_32);
						mat5_04 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat5_08 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						break;
					case 6:
						mat6_00 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat6_04 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						mat6_08 = ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						break;
					default:
						ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						ByteConverter.ToInt32(file, curAddress); curAddress += 4;
						//Debug.WriteLine($"Unexpected xj material type {type} at {(curAddress - 4).ToString("X")}");
						break;
				}
			}

			mat = newMat;
		}

		public MeshInfo Process(List<VertexData> verts, bool hasNormal, bool hasColors)
		{
			var polys = GetTriangles();
			return new MeshInfo(new NJS_MATERIAL(mat), polys.ToArray(), verts.ToArray(), hasNormal, hasColors);
		}
	}
}
