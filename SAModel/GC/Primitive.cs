using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public class Vertex
	{
		public uint PositionIndex { get; set; }
		public uint NormalIndex { get; set; }
		public uint Color0Index { get; set; }
		public uint UVIndex { get; set; }
	}

	public struct Primitive
	{
		public GXPrimitiveType PrimitiveType { get; private set; }
		public List<Vertex> Vertices { get; set; }

		public Primitive(GXPrimitiveType type)
		{
			PrimitiveType = type;
			Vertices = new List<Vertex>();
		}

		public void Write(BinaryWriter writer, IndexAttributeParameter attribute_parameters)
		{
			writer.Write((byte)PrimitiveType);

			byte[] big_endian_count = BitConverter.GetBytes((ushort)Vertices.Count);
			Array.Reverse(big_endian_count);
			writer.Write(big_endian_count);

			bool has_color = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasColor);
			bool has_normal = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasNormal);
			bool has_uv = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasUV);

			bool is_position_16bit = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.Position16BitIndex);
			bool is_color_16bit = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.Color16BitIndex);
			bool is_normal_16bit = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.Normal16BitIndex);
			bool is_uv_16bit = attribute_parameters.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.UV16BitIndex);

			foreach (Vertex v in Vertices)
			{
				// Position should always exist

				if (is_position_16bit)
				{
					byte[] big_endian_pos = BitConverter.GetBytes((ushort)v.PositionIndex);
					Array.Reverse(big_endian_pos);
					writer.Write(big_endian_pos);
				}
				else
				{
					writer.Write((byte)v.PositionIndex);
				}

				if (has_normal)
				{
					if (is_normal_16bit)
					{
						byte[] big_endian_nrm = BitConverter.GetBytes((ushort)v.NormalIndex);
						Array.Reverse(big_endian_nrm);
						writer.Write(big_endian_nrm);
					}
					else
					{
						writer.Write((byte)v.NormalIndex);
					}
				}

				if (has_color)
				{
					if (is_color_16bit)
					{
						byte[] big_endian_col = BitConverter.GetBytes((ushort)v.Color0Index);
						Array.Reverse(big_endian_col);
						writer.Write(big_endian_col);
					}
					else
					{
						writer.Write((byte)v.Color0Index);
					}
				}

				if (has_uv)
				{
					if (is_uv_16bit)
					{
						byte[] big_endian_uv = BitConverter.GetBytes((ushort)v.UVIndex);
						Array.Reverse(big_endian_uv);
						writer.Write(big_endian_uv);
					}
					else
					{
						writer.Write((byte)v.UVIndex);
					}
				}
			}
		}

		public List<Vertex> ToTriangles()
		{
			List<Vertex> sorted_vertices = new List<Vertex>();

			if (PrimitiveType == GXPrimitiveType.Triangles)
			{
				sorted_vertices = Vertices;
			}
			else if (PrimitiveType == GXPrimitiveType.TriangleStrip)
			{
				for (int v = 2; v < Vertices.Count; v++)
				{
					bool isEven = v % 2 != 0;
					Vertex[] newTri = new Vertex[3];

					newTri[0] = Vertices[v - 2];
					newTri[1] = isEven ? Vertices[v] : Vertices[v - 1];
					newTri[2] = isEven ? Vertices[v - 1] : Vertices[v];

					// Check against degenerate triangles (a triangle which shares indexes)
					if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
						sorted_vertices.AddRange(newTri);
					else
						System.Console.WriteLine("Degenerate triangle detected, skipping TriangleStrip conversion to triangle.");
				}
			}
			else if (PrimitiveType == GXPrimitiveType.TriangleFan)
			{
				for (int v = 1; v < Vertices.Count - 1; v++)
				{
					// Triangle is always, v, v+1, and index[0]?
					Vertex[] newTri = new Vertex[3];
					newTri[0] = Vertices[v];
					newTri[1] = Vertices[v + 1];
					newTri[2] = Vertices[0];

					// Check against degenerate triangles (a triangle which shares indexes)
					if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
						sorted_vertices.AddRange(newTri);
					else
						System.Console.WriteLine("Degenerate triangle detected, skipping TriangleFan conversion to triangle.");
				}
			}
			else
			{
				System.Console.WriteLine($"Attempted to triangulate primitive type { PrimitiveType }");
			}

			return sorted_vertices;
		}
	}
}
