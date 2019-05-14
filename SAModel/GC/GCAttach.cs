using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SAModel.GC
{
	public class GCAttach : Attach
	{
		public VertexData VertexData { get; private set; }
		public GeometryData GeometryData { get; private set; }

		public Vector3 BoundingSphereCenter { get; private set; }
		public float BoundingSphereRadius { get; private set; }

		public GCAttach()
		{
			Name = "gcattach_" + Extensions.GenerateIdentifier();

			VertexData = new VertexData();
			GeometryData = new GeometryData();
		}

		public GCAttach(byte[] file, int address, uint imageBase)
		{
			Name = "gcattach_" + Extensions.GenerateIdentifier();

			// The struct is 36/0x24 bytes long.

			VertexData = new VertexData();
			GeometryData = new GeometryData();

			uint vertex_attribute_offset = ByteConverter.ToUInt32(file, address) - imageBase;
			int unknown_1 = ByteConverter.ToInt32(file, address + 4);
			int opaque_geometry_data_offset = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			int translucent_geometry_data_offset = (int)(ByteConverter.ToInt32(file, address + 12) - imageBase);

			int opaque_geometry_count = ByteConverter.ToInt16(file, address + 16);
			int translucent_geometry_count = ByteConverter.ToInt16(file, address + 18);

			BoundingSphereCenter = new Vector3(ByteConverter.ToSingle(file, address + 20),
											   ByteConverter.ToSingle(file, address + 24),
				                               ByteConverter.ToSingle(file, address + 28));
			BoundingSphereRadius = ByteConverter.ToSingle(file, address + 32);

			VertexData.Load(file, vertex_attribute_offset, imageBase);

			if (opaque_geometry_count > 0)
			{
				GeometryData.Load(file, opaque_geometry_data_offset, imageBase, opaque_geometry_count, GeometryType.Opaque);
			}

			if (translucent_geometry_count > 0)
			{
				GeometryData.Load(file, translucent_geometry_data_offset, imageBase, translucent_geometry_count, GeometryType.Translucent);
			}
		}

		public void ExportOBJ(string file_name)
		{
			StringWriter writer = new StringWriter();

			if (VertexData.CheckAttribute(GXVertexAttribute.Position))
			{
				for (int i = 0; i < VertexData.Positions.Count; i++)
				{
					Vector3 pos = VertexData.Positions[i];
					writer.WriteLine($"v {pos.X} {pos.Y} {pos.Z}");
				}
			}

			if (VertexData.CheckAttribute(GXVertexAttribute.Normal))
			{
				for (int i = 0; i < VertexData.Normals.Count; i++)
				{
					Vector3 nrm = VertexData.Normals[i];
					writer.WriteLine($"vn {nrm.X} {nrm.Y} {nrm.Z}");
				}
			}

			if (VertexData.CheckAttribute(GXVertexAttribute.Tex0))
			{
				for (int i = 0; i < VertexData.TexCoord_0.Count; i++)
				{
					Vector2 tex = VertexData.TexCoord_0[i];
					writer.WriteLine($"vt {tex.X} {tex.Y}");
				}
			}

			int mesh_index = 0;

			foreach (Mesh m in GeometryData.OpaqueMeshes)
			{
				writer.WriteLine($"o mesh_{ mesh_index++ }");
				foreach (Primitive p in m.Primitives)
				{
					List<Vertex> triangles = p.ToTriangles();
					if (triangles == null)
						continue;

					for (int i = 0; i < triangles.Count; i += 3)
					{
						writer.Write($"f {triangles[i].PositionIndex + 1}/{triangles[i].UVIndex + 1} {triangles[i + 1].PositionIndex + 1}/{triangles[i + 1].UVIndex + 1} {triangles[i + 2].PositionIndex + 1}/{triangles[i + 2].UVIndex + 1}");
						writer.WriteLine();
					}
				}
			}

			foreach (Mesh m in GeometryData.TranslucentMeshes)
			{
				foreach (Primitive p in m.Primitives)
				{
					List<Vertex> triangles = p.ToTriangles();
					if (triangles == null)
						continue;

					for (int i = 0; i < triangles.Count; i += 3)
					{
						writer.WriteLine($"f {triangles[i].PositionIndex + 1} {triangles[i + 1].PositionIndex + 1} {triangles[i + 2].PositionIndex + 1}");
					}
				}
			}

			File.WriteAllText(file_name, writer.ToString());
		}

		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			byte[] output = new byte[1];

			using (MemoryStream strm = new MemoryStream())
			{
				BinaryWriter gc_file = new BinaryWriter(strm);

				gc_file.Write((int)0);
				gc_file.Write((int)0);
				gc_file.Write((int)0);
				gc_file.Write((int)0);

				gc_file.Write((short)GeometryData.OpaqueMeshes.Count);
				gc_file.Write((short)GeometryData.TranslucentMeshes.Count);

				gc_file.Write(BoundingSphereCenter.X);
				gc_file.Write(BoundingSphereCenter.Y);
				gc_file.Write(BoundingSphereCenter.Z);
				gc_file.Write(BoundingSphereRadius);

				VertexData.WriteVertexAttributes(gc_file, imageBase);

				output = strm.ToArray();
			}

			address = (uint)output.Length;
			return output;
		}

		public override string ToStruct(bool DX)
		{
			throw new System.NotImplementedException();
		}

		public override void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures = null)
		{
			throw new System.NotImplementedException();
		}

		public override void ProcessVertexData()
		{
			throw new System.NotImplementedException();
		}

		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, int frame, int animindex)
		{
			throw new System.NotImplementedException();
		}

		public override BasicAttach ToBasicModel()
		{
			throw new System.NotImplementedException();
		}

		public override ChunkAttach ToChunkModel()
		{
			throw new System.NotImplementedException();
		}

		public override Attach Clone()
		{
			throw new System.NotImplementedException();
		}
	}
}
