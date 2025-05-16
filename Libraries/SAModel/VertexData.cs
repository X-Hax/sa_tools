﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace SAModel
{
	/// <summary>
	/// This is a standard tri mesh representation of a BasicAttach or ChunkAttach.
	/// </summary>
	public class MeshInfo
	{
		public NJS_MATERIAL Material { get; private set; }
		public Poly[] Polys { get; private set; }
		public VertexData[] Vertices { get; private set; }
		public bool HasUV { get; private set; }
		public bool HasVC { get; private set; }
		public bool IsMod { get; private set; }
		public int GCUVScaleType { get; private set; }

		public MeshInfo(NJS_MATERIAL material, Poly[] polys, VertexData[] vertices, bool hasUV, bool hasVC, bool isMod = false)
		{
			Material = material;
			Polys = polys;
			Vertices = vertices;
			HasUV = hasUV;
			HasVC = hasVC;
			IsMod = isMod;
		}

		public ushort[] ToTriangles()
		{
			List<ushort> tris = new List<ushort>();
			foreach (Poly poly in Polys)
			{
				if (poly is Triangle)
					tris.AddRange(poly.Indexes);
				else if (poly is Quad)
				{
					tris.Add(poly.Indexes[0]);
					tris.Add(poly.Indexes[1]);
					tris.Add(poly.Indexes[2]);
					tris.Add(poly.Indexes[2]);
					tris.Add(poly.Indexes[1]);
					tris.Add(poly.Indexes[3]);
				}
				else if (poly is Strip)
				{
					bool flip = !((Strip)poly).Reversed;
					for (int k = 0; k < poly.Indexes.Length - 2; k++)
					{
						flip = !flip;
						if (!flip)
						{
							tris.Add(poly.Indexes[k]);
							tris.Add(poly.Indexes[k + 1]);
							tris.Add(poly.Indexes[k + 2]);
						}
						else
						{
							tris.Add(poly.Indexes[k + 1]);
							tris.Add(poly.Indexes[k]);
							tris.Add(poly.Indexes[k + 2]);
						}
					}
				}
			}
			return tris.ToArray();
		}
	}

	public struct VertexData : IEquatable<VertexData>
	{
		public Vertex Position;
		public Vertex Normal;
		public Color? Color;
		public UV UV;

		public VertexData(Vertex position)
			: this(position, null, null, null)
		{
		}

		public VertexData(Vertex position, Vertex normal)
			: this(position, normal, null, null)
		{
		}

		public VertexData(Vertex position, Vertex normal, Color? color, UV uv)
		{
			Position = position;
			Normal = normal ?? Vertex.UpNormal;
			Color = color;
			UV = uv;
		}

		public VertexData(GC.Vector3 position, GC.Vector3 normal, GC.Color color, GC.UV uv, GC.GCUVScale scale = GC.GCUVScale.Default)
		{
			Position = new Vertex(position.X, position.Y, position.Z);
			Normal = new Vertex(normal.X, normal.Y, normal.Z) ?? Vertex.UpNormal;

			//why does this work, i fed R in as A
			//Color = System.Drawing.Color.FromArgb((int)(color.R), (int)(color.G), (int)(color.B), (int)(color.A));

			//Color = color.SystemCol;
			Color = System.Drawing.Color.FromArgb(color.Red, color.Alpha, color.Blue, color.Green);

			//Color = color;
			short divider;
			switch (scale)
			{
				default:
				case GC.GCUVScale.Default:
				case GC.GCUVScale.Scale1:
					divider = 1;
					break;
				case GC.GCUVScale.Scale2:
					divider = 2;
					break;
				case GC.GCUVScale.Scale3:
					divider = 4;
					break;
				case GC.GCUVScale.Scale4:
					divider = 8;
					break;
				case GC.GCUVScale.Scale5:
					divider = 16;
					break;
				case GC.GCUVScale.Scale6:
					divider = 32;
					break;
				case GC.GCUVScale.Scale7:
					divider = 64;
					break;
				case GC.GCUVScale.Scale8:
					divider = 128;
					break;
			}
			UV = new UV() { U = uv.XF / divider, V = uv.YF / divider };
		}

		public override bool Equals(object obj)
		{
			if (obj is VertexData)
				return Equals((VertexData)obj);
			return false;
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode() ^ Normal.GetHashCode() ^ (Color?.GetHashCode() ?? 0) ^ (UV?.GetHashCode() ?? 0);
		}

		public bool Equals(VertexData other)
		{
			return Position.Equals(other.Position) && Equals(Normal, other.Normal) && Color == other.Color &&
			       (UV == null ? other.UV == null : UV.Equals(other.UV));
		}
	}
}