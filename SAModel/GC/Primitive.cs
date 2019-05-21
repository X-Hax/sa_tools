using System;
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
