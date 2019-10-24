using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public enum GeometryType
	{
		Opaque,
		Translucent
	}

	public class GeometryData
	{
		public List<Mesh> OpaqueMeshes { get; private set; }
		public List<Mesh> TranslucentMeshes { get; private set; }

		private IndexAttributeParameter cur_attributes;

		public GeometryData()
		{
			OpaqueMeshes = new List<Mesh>();
			TranslucentMeshes = new List<Mesh>();
		}

		public void Load(byte[] file, int address, uint imageBase, int count, GeometryType geometry_type)
		{
			for (int i = 0; i < count; i++)
			{
				Mesh new_mesh = new Mesh(file, address, imageBase, cur_attributes);

				if (new_mesh.Parameters.Exists(x => x.ParameterType == ParameterType.IndexAttributeFlags))
				{
					cur_attributes = (IndexAttributeParameter)new_mesh.Parameters.Find(x => x.ParameterType == ParameterType.IndexAttributeFlags);
				}

				if (geometry_type == GeometryType.Translucent)
				{
					TranslucentMeshes.Add(new_mesh);
				}
				else
				{
					OpaqueMeshes.Add(new_mesh);
				}

				address += 16;
			}
		}

		public void WriteGeometryData(BinaryWriter writer, uint imageBase)
		{
			using (MemoryStream opaque = new MemoryStream(), translucent = new MemoryStream())
			{
				BinaryWriter opaque_writer = new BinaryWriter(opaque);
				BinaryWriter trans_writer = new BinaryWriter(translucent);

				// First, write the mesh parameters for the two groups.

				foreach (Mesh m in OpaqueMeshes)
				{
					opaque_writer.Write((int)(writer.BaseStream.Length + imageBase));
					opaque_writer.Write((int)m.Parameters.Count);
					opaque_writer.Write((int)0); // Placeholder for index data pointer
					opaque_writer.Write((int)0); // Placeholder for index data size

					foreach (Parameter p in m.Parameters)
					{
						p.Write(writer);
					}
				}

				foreach (Mesh m in TranslucentMeshes)
				{
					trans_writer.Write((int)(writer.BaseStream.Length + imageBase));
					trans_writer.Write((int)m.Parameters.Count);
					trans_writer.Write((int)0); // Placeholder for index data pointer
					trans_writer.Write((int)0); // Placeholder for index data size

					foreach (Parameter p in m.Parameters)
					{
						p.Write(writer);
					}
				}

				// Reset the geometry header writers
				opaque_writer.Seek(0, SeekOrigin.Begin);
				trans_writer.Seek(0, SeekOrigin.Begin);

				// Second, write the primitive data for each mesh.

				IndexAttributeParameter cur_index_param = null; // This will hold the index parameters that are currently in use

				foreach (Mesh m in OpaqueMeshes)
				{
					long init_file_pos = writer.BaseStream.Position;

					opaque_writer.Seek(8, SeekOrigin.Current);
					opaque_writer.Write((int)(init_file_pos + imageBase));

					// Switch the index parameters if this mesh specifies new ones
					if (m.Parameters.Exists(x => x.ParameterType == ParameterType.IndexAttributeFlags))
					{
						cur_index_param = (IndexAttributeParameter)m.Parameters.Find(x => x.ParameterType == ParameterType.IndexAttributeFlags);
					}

					foreach (Primitive p in m.Primitives)
					{
						p.Write(writer, cur_index_param);
					}

					PadMesh(writer, writer.BaseStream.Position - init_file_pos);

					// Write the primitive data size
					opaque_writer.Write((int)(writer.BaseStream.Position - init_file_pos));
				}

				foreach (Mesh m in TranslucentMeshes)
				{
					long init_file_pos = writer.BaseStream.Position;

					trans_writer.Seek(8, SeekOrigin.Current);
					trans_writer.Write((int)(init_file_pos + imageBase));

					// Switch the index parameters if this mesh specifies new ones
					if (m.Parameters.Exists(x => x.ParameterType == ParameterType.IndexAttributeFlags))
					{
						cur_index_param = (IndexAttributeParameter)m.Parameters.Find(x => x.ParameterType == ParameterType.IndexAttributeFlags);
					}

					foreach (Primitive p in m.Primitives)
					{
						p.Write(writer, cur_index_param);
					}

					PadMesh(writer, writer.BaseStream.Position - init_file_pos);

					// Write the primitive data size
					trans_writer.Write((int)(writer.BaseStream.Position - init_file_pos));
				}

				// Lastly, append the mesh header data and update the file header.

				if (OpaqueMeshes.Count > 0)
				{
					writer.Seek(8, SeekOrigin.Begin);
					writer.Write((int)(writer.BaseStream.Length + imageBase));
					writer.Seek(0, SeekOrigin.End);
					writer.Write(opaque.ToArray());
				}

				if (TranslucentMeshes.Count > 0)
				{
					writer.Seek(12, SeekOrigin.Begin);
					writer.Write((int)(writer.BaseStream.Length + imageBase));
					writer.Seek(0, SeekOrigin.End);
					writer.Write(translucent.ToArray());
				}
			}
		}

		private void PadMesh(BinaryWriter writer, long length)
		{
			// Pad up to a 32 byte alignment
			// Formula: (x + (n-1)) & ~(n-1)
			long nextAligned = (length + (0x1F)) & ~(0x1F);

			long delta = nextAligned - length;
			writer.BaseStream.Position = writer.BaseStream.Length;
			for (int i = 0; i < delta; i++)
			{
				writer.Write((byte)0);
			}
		}
	}
}
