﻿using System;
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
		/// Read a primitive object from a file
		/// </summary>
		/// <param name="file">The files contents as a byte array</param>
		/// <param name="address">The starting address of the primitive</param>
		/// <param name="indexFlags">How the indices of the loops are structured</param>
		public GCPrimitive(byte[] file, int address, GCIndexAttributeFlags indexFlags, out int end)
		{
			primitiveType = (GCPrimitiveType)file[address];

			bool wasBigEndian = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;

			ushort vtxCount = ByteConverter.ToUInt16(file, address + 1);

			// checking the flags
			bool hasFlag(GCIndexAttributeFlags flag)
			{
				return indexFlags.HasFlag(flag);
			}

			// position always exists
			bool has_color = hasFlag(GCIndexAttributeFlags.HasColor);
			bool has_normal = hasFlag(GCIndexAttributeFlags.HasNormal);
			bool has_uv = hasFlag(GCIndexAttributeFlags.HasUV);

			//whether any of the indices use 16 bits instead of 8
			bool pos16bit = hasFlag(GCIndexAttributeFlags.Position16BitIndex);
			bool col16bit = hasFlag(GCIndexAttributeFlags.Color16BitIndex);
			bool nrm16bit = hasFlag(GCIndexAttributeFlags.Normal16BitIndex);
			bool uv16bit = hasFlag(GCIndexAttributeFlags.UV16BitIndex);

			int tmpaddr = address + 3;

			loops = new List<Loop>();

			for (ushort i = 0; i < vtxCount; i++)
			{
				Loop l = new Loop();

				// reading position, which should always exist
				if(pos16bit)
				{
					l.PositionIndex = ByteConverter.ToUInt16(file, tmpaddr);
					tmpaddr += 2;
				}
				else
				{
					l.PositionIndex = file[tmpaddr];
					tmpaddr++;
				}

				// reading normals
				if(has_normal)
				{
					if (nrm16bit)
					{
						l.NormalIndex = ByteConverter.ToUInt16(file, tmpaddr);
						tmpaddr += 2;
					}
					else
					{
						l.NormalIndex = file[tmpaddr];
						tmpaddr++;
					}
				}

				// reading colors
				if (has_color)
				{
					if (col16bit)
					{
						l.Color0Index = ByteConverter.ToUInt16(file, tmpaddr);
						tmpaddr += 2;
					}
					else
					{
						l.Color0Index = file[tmpaddr];
						tmpaddr++;
					}
				}

				// reading uvs
				if (has_uv)
				{
					if (uv16bit)
					{
						l.UV0Index = ByteConverter.ToUInt16(file, tmpaddr);
						tmpaddr += 2;
					}
					else
					{
						l.UV0Index = file[tmpaddr];
						tmpaddr++;
					}
				}

				loops.Add(l);
			}

			end = tmpaddr;

			ByteConverter.BigEndian = wasBigEndian;
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
			// writing count as big endian
			result.Add(big_endian_count[1]);
			result.Add(big_endian_count[0]);

			// checking the flags
			bool hasFlag(GCIndexAttributeFlags flag)
			{
				return indexFlags.HasFlag(flag);
			}

			// position always exists
			bool has_color = hasFlag(GCIndexAttributeFlags.HasColor);
			bool has_normal = hasFlag(GCIndexAttributeFlags.HasNormal);
			bool has_uv = hasFlag(GCIndexAttributeFlags.HasUV);

			bool is_position_16bit = hasFlag(GCIndexAttributeFlags.Position16BitIndex);
			bool is_color_16bit = hasFlag(GCIndexAttributeFlags.Color16BitIndex);
			bool is_normal_16bit = hasFlag(GCIndexAttributeFlags.Normal16BitIndex);
			bool is_uv_16bit = hasFlag(GCIndexAttributeFlags.UV16BitIndex);

			foreach (Loop v in loops)
			{
				// Position should always exist

				if (is_position_16bit)
				{
					byte[] big_endian_pos = BitConverter.GetBytes(v.PositionIndex);
					// writing count as big endian
					result.Add(big_endian_pos[1]);
					result.Add(big_endian_pos[0]);
				}
				else
				{
					result.Add((byte)v.PositionIndex);
				}

				if (has_normal)
				{
					if (is_normal_16bit)
					{
						byte[] big_endian_nrm = BitConverter.GetBytes(v.NormalIndex);
						// writing count as big endian
						result.Add(big_endian_nrm[1]);
						result.Add(big_endian_nrm[0]);
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
						byte[] big_endian_col = BitConverter.GetBytes(v.Color0Index);
						// writing count as big endian
						result.Add(big_endian_col[1]);
						result.Add(big_endian_col[0]);
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
						byte[] big_endian_uv = BitConverter.GetBytes(v.UV0Index);
						// writing count as big endian
						result.Add(big_endian_uv[1]);
						result.Add(big_endian_uv[0]);
					}
					else
					{
						result.Add((byte)v.UV0Index);
					}
				}
			}
			return result.ToArray();
		}

		public string LoopStruct()
		{
			List<string> s = new List<string>(loops.Count);
			for (int i = 0; i < loops.Count; i++)
				s.Add(loops[i].ToString());
			return string.Join(", ", s.ToArray());
		}
		public virtual string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append((byte)primitiveType);
			result.Append(", ");
			result.Append((ushort)loops.Count);
			result.Append(", { ");
			result.Append(LoopStruct());
			result.Append(" }");
			result.Append(" }");
			return result.ToString();
		}
		public void ToNJA(TextWriter writer)
		{
			string primtype = null;
			switch (primitiveType)
			{
				case GCPrimitiveType.Triangles:
					primtype = "GJD_PRIM_TRIANGLE";
					break;
				case GCPrimitiveType.TriangleStrip:
					primtype = "GJD_PRIM_TRISTRIP";
					break;
				case GCPrimitiveType.TriangleFan:
					primtype = "GJD_PRIM_TRIFAN";
					break;
				case GCPrimitiveType.Lines:
					primtype = "GJD_PRIM_LINE";
					break;
				case GCPrimitiveType.LineStrip:
					primtype = "GJD_PRIM_LINESTRIP";
					break;
				case GCPrimitiveType.Points:
					primtype = "GJD_PRIM_POINT";
					break;
			}
			writer.WriteLine($"\t{primtype}(" + loops.Count + "),");
			for (int i = 0; i < loops.Count; i++)
				writer.WriteLine("\t" + loops[i] + ",");
		}

		/// <summary>
		/// Convert the primitive into a triangle list
		/// </summary>
		/// <returns></returns>
		public List<Loop> ToTriangles()
		{
			List<Loop> sorted_vertices = new List<Loop>();
			int degTriangles = 0;

			switch (primitiveType)
			{
				case GCPrimitiveType.Triangles:
					return loops;
				case GCPrimitiveType.TriangleStrip:
					bool isEven = false;
					for (int v = 2; v < loops.Count; v++)
					{
						Loop[] newTri = new Loop[]
						{
							loops[v - 2],
							isEven ? loops[v] : loops[v - 1],
							isEven ? loops[v - 1] : loops[v]
						};
						isEven = !isEven;

						// Check against degenerate triangles (a triangle which shares indexes)
						if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
							sorted_vertices.AddRange(newTri);
						else degTriangles++;
					}
					break;
				case GCPrimitiveType.TriangleFan:
					for (int v = 1; v < loops.Count - 1; v++)
					{
						// Triangle is always, v, v+1, and index[0]?
						Loop[] newTri = new Loop[]
						{
							loops[v],
							loops[v + 1],
							loops[0],
						};

						// Check against degenerate triangles (a triangle which shares indexes)
						if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
							sorted_vertices.AddRange(newTri);
						else degTriangles++;
					}
					break;
				default:
					Console.WriteLine($"Attempted to triangulate primitive type { primitiveType }");
					break;
			}

			if (degTriangles > 0)
			{
				Console.WriteLine("Degenerate triangles skipped: " + degTriangles);
			}

			return sorted_vertices;
		}

		public override string ToString()
			{
				return $"{primitiveType}: {loops.Count}";
			}
		}
	}
