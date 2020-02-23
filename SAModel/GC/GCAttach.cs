using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SAModel.GC
{
	/// <summary>
	/// An attach/mesh using the Gamecube format
	/// </summary>
	[Serializable]
	public class GCAttach : Attach
	{
		/// <summary>
		/// The seperate sets of vertex data in this attach
		/// </summary>
		public readonly List<GCVertexSet> vertexData;

		/// <summary>
		/// The meshes with opaque rendering properties
		/// </summary>
		public readonly List<GCMesh> opaqueMeshes;

		/// <summary>
		/// The meshes with translucent rendering properties
		/// </summary>
		public readonly List<GCMesh> translucentMeshes;


		/// <summary>
		/// Create a new empty GC attach
		/// </summary>
		public GCAttach()
		{
			Name = "gcattach_" + Extensions.GenerateIdentifier();

			vertexData = new List<GCVertexSet>();
			opaqueMeshes = new List<GCMesh>();
			translucentMeshes = new List<GCMesh>();
			Bounds = new BoundingSphere();
		}

		/// <summary>
		/// Reads a GC attach from a file
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The address at which the attach is located</param>
		/// <param name="imageBase">The imagebase of the file</param>
		/// <param name="labels">The labels of the file</param>
		public GCAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "attach_" + address.ToString("X8");

			// The struct is 36/0x24 bytes long

			uint vertexAddress = ByteConverter.ToUInt32(file, address) - imageBase;
			//uint gap = ByteConverter.ToUInt32(file, address + 4);
			int opaqueAddress = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			int translucentAddress = (int)(ByteConverter.ToInt32(file, address + 12) - imageBase);

			int opaqueCount = ByteConverter.ToInt16(file, address + 16);
			int translucentCount = ByteConverter.ToInt16(file, address + 18);

			Bounds = new BoundingSphere(file, address + 20);

			// reading vertex data
			vertexData = new List<GCVertexSet>();
			GCVertexSet vertexSet = new GCVertexSet(file, vertexAddress, imageBase);
			while(vertexSet.attribute != GCVertexAttribute.Null)
			{
				vertexData.Add(vertexSet);
				vertexAddress += 16;
				vertexSet = new GCVertexSet(file, vertexAddress, imageBase);
			}

			// reading geometry
			GCIndexAttributeFlags indexFlags = GCIndexAttributeFlags.HasPosition;

			opaqueMeshes = new List<GCMesh>();
			for (int i = 0; i < opaqueCount; i++)
			{
				GCMesh mesh = new GCMesh(file, opaqueAddress, imageBase, indexFlags);

				GCIndexAttributeFlags? t = mesh.IndexFlags;
				if (t.HasValue) indexFlags = t.Value;

				opaqueMeshes.Add(mesh);
				opaqueAddress += 16;
			}

			translucentMeshes = new List<GCMesh>();
			for (int i = 0; i < translucentCount; i++)
			{
				GCMesh mesh = new GCMesh(file, translucentAddress, imageBase, indexFlags);

				GCIndexAttributeFlags? t = mesh.IndexFlags;
				if (t.HasValue) indexFlags = t.Value;

				translucentMeshes.Add(mesh);
				translucentAddress += 16;
			}
		}

		/// <summary>
		/// Writes the attaches contents into a byte array
		/// </summary>
		/// <param name="imageBase">The files imagebase</param>
		/// <param name="DX">Unused</param>
		/// <param name="labels">The files labels</param>
		/// <param name="address"></param>
		/// <returns></returns>
		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			byte[] output;

			using (MemoryStream strm = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(strm);

				writer.Write(new byte[16]); // address placeholders
				writer.Write((ushort)opaqueMeshes.Count);
				writer.Write((ushort)translucentMeshes.Count);
				writer.Write(Bounds.GetBytes());

				// writing vertex data
				foreach(GCVertexSet vtx in vertexData)
				{
					vtx.WriteData(writer);
				}

				uint vtxAddr = (uint)writer.BaseStream.Length + imageBase;

				// writing vertex attributes
				foreach (GCVertexSet vtx in vertexData)
				{
					vtx.WriteAttribute(writer, imageBase);
				}
				writer.Write((byte)255);
				writer.Write(new byte[15]); // empty vtx attribute

				// writing geometry data
				GCIndexAttributeFlags indexFlags = GCIndexAttributeFlags.HasPosition;
				foreach(GCMesh m in opaqueMeshes)
				{
					GCIndexAttributeFlags? t = m.IndexFlags;
					if (t.HasValue) indexFlags = t.Value;
					m.WriteData(writer, indexFlags);
				}
				foreach (GCMesh m in translucentMeshes)
				{
					GCIndexAttributeFlags? t = m.IndexFlags;
					if (t.HasValue) indexFlags = t.Value;
					m.WriteData(writer, indexFlags);
				}

				// writing geometry properties
				uint opaqueAddress = (uint)writer.BaseStream.Length + imageBase;
				foreach (GCMesh m in opaqueMeshes)
				{
					m.WriteProperties(writer, imageBase);
				}
				uint translucentAddress = (uint)writer.BaseStream.Length + imageBase;
				foreach (GCMesh m in translucentMeshes)
				{
					m.WriteProperties(writer, imageBase);
				}

				// replacing the placeholders
				writer.Seek(0, SeekOrigin.Begin);
				writer.Write(vtxAddr);
				writer.Write(0);
				writer.Write(opaqueAddress);
				writer.Write(translucentAddress);
				writer.Seek(0, SeekOrigin.End);

				output = strm.ToArray();
			}

			address = 0;
			labels.Add(Name, imageBase);
			return output;
		}

		/// <summary>
		/// Processes the vertex data to be rendered
		/// </summary>
		public override void ProcessVertexData()
		{
			List<MeshInfo> meshInfo = new List<MeshInfo>();

			List<IOVtx> positions	= vertexData.Find(x => x.attribute == GCVertexAttribute.Position)?.data;
			List<IOVtx> normals		= vertexData.Find(x => x.attribute == GCVertexAttribute.Normal)?.data;
			List<IOVtx> colors		= vertexData.Find(x => x.attribute == GCVertexAttribute.Color0)?.data;
			List<IOVtx> uvs			= vertexData.Find(x => x.attribute == GCVertexAttribute.Tex0)?.data;

			NJS_MATERIAL mat = new NJS_MATERIAL();

			mat.UseAlpha = false;
			foreach (GCMesh m in opaqueMeshes)
				meshInfo.Add(m.Process(mat, positions, normals, colors, uvs));

			mat.UseAlpha = true;
			foreach (GCMesh m in translucentMeshes)
				meshInfo.Add(m.Process(mat, positions, normals, colors, uvs));

			MeshInfo = meshInfo.ToArray();
		}

		/// <summary>
		/// Creates a C Struct string identical to the data given (WIP)
		/// </summary>
		/// <param name="DX">Unused</param>
		/// <returns></returns>
		public override string ToStruct(bool DX)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// WIP
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="DX">Unused</param>
		/// <param name="labels"></param>
		/// <param name="textures"></param>
		public override void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures = null)
		{
			throw new NotImplementedException();
		}

		#region Unused

		/// <summary>
		/// Unused
		/// </summary>
		/// <param name="motion"></param>
		/// <param name="frame"></param>
		/// <param name="animindex"></param>
		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, int frame, int animindex)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Unused
		/// </summary>
		/// <returns></returns>
		public override Attach Clone()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
