using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
	public class GCPrimitive : ICloneable
	{
		/// <summary>
		/// The way in which triangles are being stored
		/// </summary>
		public GCPrimitiveType PrimitiveType;

		/// <summary>
		/// The stored polygons
		/// </summary>
		public List<Loop> Loops { get; set; } = [];

		/// <summary>
		/// Create a new empty Primitive
		/// </summary>
		/// <param name="type">The type of primitive</param>
		public GCPrimitive(GCPrimitiveType type)
		{
			PrimitiveType = type;
		}

		/// <summary>
		/// Read a primitive object from a file
		/// </summary>
		/// <param name="file">The files contents as a byte array</param>
		/// <param name="address">The starting address of the primitive</param>
		/// <param name="indexFlags">How the indices of the loops are structured</param>
		/// <param name="end"></param>
		public GCPrimitive(byte[] file, int address, GCIndexAttributeFlags indexFlags, out int end)
		{
			PrimitiveType = (GCPrimitiveType)file[address];

			var wasBigEndian = ByteConverter.BigEndian;
			ByteConverter.BigEndian = true;

			var vtxCount = ByteConverter.ToUInt16(file, address + 1);

			// Position always exists
			var hasColor = indexFlags.HasFlag(GCIndexAttributeFlags.HasColor);
			var hasNormal = indexFlags.HasFlag(GCIndexAttributeFlags.HasNormal);
			var hasUv = indexFlags.HasFlag(GCIndexAttributeFlags.HasUV);

			// Whether any of the indices use 16 bits instead of 8
			var pos16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.Position16BitIndex);
			var col16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.Color16BitIndex);
			var nrm16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.Normal16BitIndex);
			var uv16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.UV16BitIndex);

			var tempAddr = address + 3;

			for (ushort i = 0; i < vtxCount; i++)
			{
				var l = new Loop();

				// Reading position, which should always exist
				if (pos16Bit)
				{
					l.PositionIndex = ByteConverter.ToUInt16(file, tempAddr);
					tempAddr += 2;
				}
				else
				{
					l.PositionIndex = file[tempAddr];
					tempAddr++;
				}

				// Reading normals
				if (hasNormal)
				{
					if (nrm16Bit)
					{
						l.NormalIndex = ByteConverter.ToUInt16(file, tempAddr);
						tempAddr += 2;
					}
					else
					{
						l.NormalIndex = file[tempAddr];
						tempAddr++;
					}
				}

				// Reading colors
				if (hasColor)
				{
					if (col16Bit)
					{
						l.Color0Index = ByteConverter.ToUInt16(file, tempAddr);
						tempAddr += 2;
					}
					else
					{
						l.Color0Index = file[tempAddr];
						tempAddr++;
					}
				}

				// Reading uvs
				if (hasUv)
				{
					if (uv16Bit)
					{
						l.UV0Index = ByteConverter.ToUInt16(file, tempAddr);
						tempAddr += 2;
					}
					else
					{
						l.UV0Index = file[tempAddr];
						tempAddr++;
					}
				}

				Loops.Add(l);
			}

			end = tempAddr;

			ByteConverter.BigEndian = wasBigEndian;
		}

		/// <summary>
		/// Write the contents
		/// </summary>
		/// <param name="indexFlags">How the indices of the loops are structured</param>
		public byte[] GetBytes(GCIndexAttributeFlags indexFlags)
		{
			List<byte> result = [(byte)PrimitiveType];

			var bigEndianCount = BitConverter.GetBytes((ushort)Loops.Count);
			// Writing count as big endian
			result.Add(bigEndianCount[1]);
			result.Add(bigEndianCount[0]);
			
			// position always exists
			var hasColor = indexFlags.HasFlag(GCIndexAttributeFlags.HasColor);
			var hasNormal = indexFlags.HasFlag(GCIndexAttributeFlags.HasNormal);
			var hasUv = indexFlags.HasFlag(GCIndexAttributeFlags.HasUV);

			var isPosition16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.Position16BitIndex);
			var isColor16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.Color16BitIndex);
			var isNormal16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.Normal16BitIndex);
			var isUv16Bit = indexFlags.HasFlag(GCIndexAttributeFlags.UV16BitIndex);

			foreach (var v in Loops)
			{
				// Position should always exist
				if (isPosition16Bit)
				{
					var bigEndianPos = BitConverter.GetBytes(v.PositionIndex);
					// Writing count as big endian
					result.Add(bigEndianPos[1]);
					result.Add(bigEndianPos[0]);
				}
				else
				{
					result.Add((byte)v.PositionIndex);
				}

				if (hasNormal)
				{
					if (isNormal16Bit)
					{
						var bigEndianNrm = BitConverter.GetBytes(v.NormalIndex);
						// Writing count as big endian
						result.Add(bigEndianNrm[1]);
						result.Add(bigEndianNrm[0]);
					}
					else
					{
						result.Add((byte)v.NormalIndex);
					}
				}

				if (hasColor)
				{
					if (isColor16Bit)
					{
						var bigEndianCol = BitConverter.GetBytes(v.Color0Index);
						// Writing count as big endian
						result.Add(bigEndianCol[1]);
						result.Add(bigEndianCol[0]);
					}
					else
					{
						result.Add((byte)v.Color0Index);
					}
				}

				if (hasUv)
				{
					if (isUv16Bit)
					{
						var bigEndianUv = BitConverter.GetBytes(v.UV0Index);
						// Writing count as big endian
						result.Add(bigEndianUv[1]);
						result.Add(bigEndianUv[0]);
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
			var s = new List<string>(Loops.Count);
			s.AddRange(Loops.Select(loop => loop.ToString()));

			return string.Join(", ", s.ToArray());
		}
		
		public virtual string ToStruct()
		{
			var result = new StringBuilder("{ ");
			
			result.Append((byte)PrimitiveType);
			result.Append(", ");
			result.Append((ushort)Loops.Count);
			result.Append(", { ");
			result.Append(LoopStruct());
			result.Append(" }");
			result.Append(" }");
			
			return result.ToString();
		}

		object ICloneable.Clone() => Clone();

		public GCPrimitive Clone()
		{
			GCPrimitive result = (GCPrimitive)MemberwiseClone();
			return result;
		}

		public void ToNJA(TextWriter writer)
		{
			var primtype = PrimitiveType switch
			{
				GCPrimitiveType.Triangles => "GJD_PRIM_TRIANGLE",
				GCPrimitiveType.TriangleStrip => "GJD_PRIM_TRISTRIP",
				GCPrimitiveType.TriangleFan => "GJD_PRIM_TRIFAN",
				GCPrimitiveType.Lines => "GJD_PRIM_LINE",
				GCPrimitiveType.LineStrip => "GJD_PRIM_LINESTRIP",
				GCPrimitiveType.Points => "GJD_PRIM_POINT",
				_ => null
			};
			
			writer.WriteLine($"\t{primtype}({Loops.Count}),");
			foreach (var loop in Loops)
			{
				writer.WriteLine($"\t{loop},");
			}
		}

		/// <summary>
		/// Convert the primitive into a triangle list
		/// </summary>
		/// <returns></returns>
		public List<Loop> ToTriangles()
		{
			var sortedVertices = new List<Loop>();
			var degTriangles = 0;

			switch (PrimitiveType)
			{
				case GCPrimitiveType.Triangles:
					return Loops;
				case GCPrimitiveType.TriangleStrip:
					var isEven = false;
					for (var v = 2; v < Loops.Count; v++)
					{
						var newTri = new[]
						{
							Loops[v - 2],
							isEven ? Loops[v] : Loops[v - 1],
							isEven ? Loops[v - 1] : Loops[v]
						};
						isEven = !isEven;

						// Check against degenerate triangles (a triangle which shares indexes)
						if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
						{
							sortedVertices.AddRange(newTri);
						}
						else
						{
							degTriangles++;
						}
					}
					break;
				case GCPrimitiveType.TriangleFan:
					for (var v = 1; v < Loops.Count - 1; v++)
					{
						// Triangle is always, v, v+1, and index[0]?
						var newTri = new[]
						{
							Loops[v],
							Loops[v + 1],
							Loops[0],
						};

						// Check against degenerate triangles (a triangle which shares indexes)
						if (newTri[0] != newTri[1] && newTri[1] != newTri[2] && newTri[2] != newTri[0])
						{
							sortedVertices.AddRange(newTri);
						}
						else
						{
							degTriangles++;
						}
					}
					break;
				default:
					Console.WriteLine($"Attempted to triangulate primitive type { PrimitiveType }");
					break;
			}

			if (degTriangles > 0)
			{
				Console.WriteLine($"Degenerate triangles skipped: {degTriangles}");
			}

			return sortedVertices;
		}

		public override string ToString()
		{
			return $"{PrimitiveType}: {Loops.Count}";
		}
	}
}
