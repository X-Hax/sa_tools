using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SAModel.GC
{
	/// <summary>
	/// A single corner of a polygon, called loop
	/// </summary>
	[Serializable]
	public class Loop : IEquatable<Loop>
	{
		/// <summary>
		/// The index of the position value
		/// </summary>
		public ushort PositionIndex;

		/// <summary>
		/// The index of the normal value
		/// </summary>
		public ushort NormalIndex;

		/// <summary>
		/// The index of the color value
		/// </summary>
		public ushort Color0Index;

		/// <summary>
		/// The index of the texture coordinate value
		/// </summary>
		public ushort UV0Index;

		public bool Equals(Loop other)
		{
			return PositionIndex == other.PositionIndex && NormalIndex == other.NormalIndex && Color0Index == other.Color0Index && UV0Index == other.UV0Index;
		}

		public override string ToString()
		{
			return $"({PositionIndex}, {NormalIndex}, {Color0Index}, {UV0Index})";
		}
	}

		/// <summary>
		/// A collection of polygons
		/// </summary>
		[Serializable]
		public class GCPrimitive
		{
			/// <summary>
			/// The way in which triangles are being stored
			/// </summary>
			public GCPrimitiveType primitiveType;

			/// <summary>
			/// The stored polygons
			/// </summary>
			public List<Loop> loops { get; set; }

			/// <summary>
			/// Create a new empty Primitive
			/// </summary>
			/// <param name="type">The type of primitive</param>
			public GCPrimitive(GCPrimitiveType type)
			{
				primitiveType = type;
				loops = new List<Loop>();
			}

			/// <summary>
			/// Write the contents
			/// </summary>
			/// <param name="writer">The output stream</param>
			/// <param name="indexFlags">How the indices of the loops are structured</param>

		public byte[] GetBytes(GCIndexAttributeFlags indexFlags)
		{
			List<byte> result = new List<byte>();
			result.Add((byte)primitiveType);

			byte[] big_endian_count = BitConverter.GetBytes((ushort)loops.Count);
			Array.Reverse(big_endian_count);
			result.AddRange(big_endian_count);

			bool has_color = indexFlags.HasFlag(GCIndexAttributeFlags.HasColor);
			bool has_normal = indexFlags.HasFlag(GCIndexAttributeFlags.HasNormal);
			bool has_uv = indexFlags.HasFlag(GCIndexAttributeFlags.HasUV);

			bool is_position_16bit = indexFlags.HasFlag(GCIndexAttributeFlags.Position16BitIndex);
			bool is_color_16bit = indexFlags.HasFlag(GCIndexAttributeFlags.Color16BitIndex);
			bool is_normal_16bit = indexFlags.HasFlag(GCIndexAttributeFlags.Normal16BitIndex);
			bool is_uv_16bit = indexFlags.HasFlag(GCIndexAttributeFlags.UV16BitIndex);

			foreach (Loop v in loops)
			{
				// Position should always exist

				if (is_position_16bit)
				{
					byte[] big_endian_pos = BitConverter.GetBytes((ushort)v.PositionIndex);
					Array.Reverse(big_endian_pos);
					result.AddRange(big_endian_pos);
				}
				else
				{
					result.Add((byte)v.PositionIndex);
				}

				if (has_normal)
				{
					if (is_normal_16bit)
					{
						byte[] big_endian_nrm = BitConverter.GetBytes((ushort)v.NormalIndex);
						Array.Reverse(big_endian_nrm);
						result.AddRange(big_endian_nrm);
					}
					else
					{
						result.Add((byte)v.NormalIndex);
					}
				}

				if (has_color)
				{
					if (is_color_16bit)
					{
						byte[] big_endian_col = BitConverter.GetBytes((ushort)v.Color0Index);
						Array.Reverse(big_endian_col);
						result.AddRange(big_endian_col);
					}
					else
					{
						result.Add((byte)v.Color0Index);
					}
				}

				if (has_uv)
				{
					if (is_uv_16bit)
					{
						byte[] big_endian_uv = BitConverter.GetBytes((ushort)v.UV0Index);
						Array.Reverse(big_endian_uv);
						result.AddRange(big_endian_uv);
					}
					else
					{
						result.Add((byte)v.UV0Index);
					}
				}
			}
			return result.ToArray();
		}

			public virtual string ToStruct()
			{
				StringBuilder result = new StringBuilder("{ ");
				result.Append(primitiveType);
				result.Append(", ");
				result.Append((ushort)loops.Count);
				result.Append(", ");
				for (int i = 0; i < loops.Count; i++)
					result.Append(loops[i]);
				result.Append(" }");
				return result.ToString();
			}

		/// <summary>
		/// Convert the primitive into a triangle list
		/// </summary>
		/// <returns></returns>
		public List<Loop> ToTriangles()
		{
			List<Loop> sorted_vertices = new List<Loop>();
			switch (primitiveType)
			{
				case GCPrimitiveType.Triangles:
					sorted_vertices = loops;
					break;
				case GCPrimitiveType.TriangleStrip:
				{
					for (int v = 2; v < loops.Count; v++)
					{
						bool isEven = v % 2 != 0;
						Loop[] newTri = new Loop[3];

						newTri[0] = loops[v - 2];
						newTri[1] = isEven ? loops[v] : loops[v - 1];
						newTri[2] = isEven ? loops[v - 1] : loops[v];

						// Check against degenerate triangles (a triangle which shares indexes)
						if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
							sorted_vertices.AddRange(newTri);
						else
							System.Console.WriteLine("Degenerate triangle detected, skipping TriangleStrip conversion to triangle.");
					}
				}
					break;
				case GCPrimitiveType.TriangleFan:
				{
					for (int v = 1; v < loops.Count - 1; v++)
					{
						// Triangle is always, v, v+1, and index[0]?
						Loop[] newTri = new Loop[3];
						newTri[0] = loops[v];
						newTri[1] = loops[v + 1];
						newTri[2] = loops[0];

						// Check against degenerate triangles (a triangle which shares indexes)
						if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
							sorted_vertices.AddRange(newTri);
						else
							System.Console.WriteLine("Degenerate triangle detected, skipping TriangleFan conversion to triangle.");
					}
				}
					break;
				default:
				System.Console.WriteLine($"Attempted to triangulate primitive type { primitiveType }");
					break;
			}
			return sorted_vertices;
		}

		public override string ToString()
			{
				return $"{primitiveType}: {loops.Count}";
			}
		}
	}
