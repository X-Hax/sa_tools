using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SAModel.GC
{
	/// <summary>
	/// A single mesh, with its own parameter and primitive data <br/>
	/// </summary>
	[Serializable]
	public class GCMesh
	{
		/// <summary>
		/// The parameters that this mesh sets
		/// </summary>
		public List<GCParameter> Parameters { get; private set; }

		/// <summary>
		/// The polygon data
		/// </summary>
		public List<GCPrimitive> Primitives { get; private set; }

		/// <summary>
		/// The index attribute flags of this mesh. If it has no IndexAttribParam, it will return null
		/// </summary>
		public GCIndexAttributeFlags? IndexFlags
		{
			get
			{
				var indexParam = (IndexAttributeParameter)Parameters.Find(x => x.Type == ParameterType.IndexAttributeFlags);
				return indexParam?.IndexAttributes;
			}
		}

		/// <summary>
		/// The location to which the parameters have been written
		/// </summary>
		private uint _paramAddress;
		public string ParameterName { get; set; }

		/// <summary>
		/// The location to which the primitives have been written
		/// </summary>
		private uint _primitiveAddress;
		public string PrimitiveName { get; set; }

		/// <summary>
		/// The amount of bytes which have been written for the primitives
		/// </summary>
		private uint _primitiveSize;

		/// <summary>
		/// Create an empty mesh
		/// </summary>
		public GCMesh()
		{
			Parameters = [];
			Primitives = [];
		}

		/// <summary>
		/// Create a new mesh from existing primitives and parameters
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="primitives"></param>
		public GCMesh(List<GCParameter> parameters, List<GCPrimitive> primitives)
		{
			Parameters = parameters;
			Primitives = primitives;
		}

		public GCMesh(byte[] file, int address, uint imageBase, GCIndexAttributeFlags indexFlags)
		: this(file, address, imageBase, new Dictionary<int, string>(), indexFlags)
		{
		}

		/// <summary>
		/// Read a mesh from a file
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The address at which the mesh is located</param>
		/// <param name="imageBase">The image base (used for when reading from an exe)</param>
		/// <param name="labels"></param>
		/// <param name="indexFlags"></param>
		public GCMesh(byte[] file, int address, uint imageBase, Dictionary<int, string> labels, GCIndexAttributeFlags indexFlags)
		{
			// Getting the addresses and sizes
			var parametersOffset = (int)(ByteConverter.ToInt32(file, address) - imageBase);
			var parametersCount = ByteConverter.ToInt32(file, address + 4);

			var primitivesOffset = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			var primitivesSize = ByteConverter.ToUInt32(file, address + 12);

			// Reading the parameters
			Parameters = [];
			
			if (parametersCount != 0)
			{
				if (labels.TryGetValue(parametersOffset, out var parameterName))
				{
					ParameterName = parameterName;
				}
				else
				{
					ParameterName = "parameter_" + parametersOffset.ToString("X8");
				}
			}
			
			for (var i = 0; i < parametersCount; i++)
			{
				Parameters.Add(GCParameter.Read(file, parametersOffset));
				parametersOffset += 8;
			}

			// Getting the index attribute parameter
			var flags = IndexFlags;
			if (flags.HasValue)
			{
				indexFlags = flags.Value;
			}

			// Reading the primitives
			Primitives = [];
			if (primitivesSize != 0)
			{
				if (labels.TryGetValue(primitivesOffset, out var primitiveName))
				{
					PrimitiveName = primitiveName;
				}
				else
				{
					PrimitiveName = "primitive_" + primitivesOffset.ToString("X8");
				}
			}

			var endPos = primitivesOffset + (int)primitivesSize;

			while (primitivesOffset < endPos)
			{
				// If the primitive isn't valid
				if (file[primitivesOffset] == 0)
				{
					break;
				}

				Primitives.Add(new GCPrimitive(file, primitivesOffset, indexFlags, out primitivesOffset));
			}
			
			_primitiveSize = primitivesSize;
		}

		/// <summary>
		/// Writes the parameters and primitives to a stream
		/// </summary>
		/// <param name="parameterAddress"></param>
		/// <param name="primitiveAddress"></param>
		public byte[] GetBytes(uint parameterAddress, uint primitiveAddress)
		{
			var primitiveSize = Convert.ToUInt32(Math.Ceiling((decimal)_primitiveSize / 32) * 32);
			var result = new List<byte>();
			
			result.AddRange(ByteConverter.GetBytes(parameterAddress));
			result.AddRange(ByteConverter.GetBytes((uint)Parameters.Count));
			result.AddRange(ByteConverter.GetBytes(primitiveAddress));
			result.AddRange(ByteConverter.GetBytes(primitiveSize));
			
			return result.ToArray();
		}

		public string ToStruct()
		{
			var primitiveSize = Convert.ToUInt32(Math.Ceiling((decimal)_primitiveSize / 32) * 32);
			var result = new StringBuilder("{ ");
			
			result.Append(Parameters.Count != 0 ? ParameterName : "NULL");
			result.Append(", ");
			result.Append(Parameters != null ? (uint)Parameters.Count : 0);
			result.Append(", ");
			result.Append(_primitiveSize != 0 ? PrimitiveName : "NULL");
			result.Append(", ");
			result.Append(Primitives != null ? primitiveSize : 0);
			result.Append(" }");
			
			return result.ToString();
		}

		// WIP
		public void ToNJA(TextWriter writer)
		{
			if (Parameters != null && Parameters.Count != 0)
			{
				writer.WriteLine($"PARAMETER   {ParameterName}[]");
				writer.WriteLine("START");
				
				foreach (var item in Parameters)
				{
					item.ToNJA(writer);
				}

				writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
			}

			if (Primitives != null)
			{
				writer.WriteLine($"PRIMITIVE   {PrimitiveName}[]");
				writer.WriteLine("START");
				
				foreach (var item in Primitives)
				{
					item.ToNJA(writer);
				}

				writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
			}
		}
		
		public void RefToNJA(TextWriter writer)
		{
			if (Parameters != null && Parameters.Count != 0)
			{
				writer.WriteLine($"Parameter   {ParameterName},");
			}
			else
			{
				writer.WriteLine($"Parameter   NULL{ParameterName},");
			}

			writer.WriteLine($"ParamNum    {Parameters.Count},");

			if (Primitives != null)
			{
				writer.WriteLine($"Primitive   {PrimitiveName},");
			}
			else
			{
				writer.WriteLine($"Primitive   NULL{PrimitiveName},");
			}

			writer.WriteLine($"PrimNum     {_primitiveSize},");
		}

		/// <summary>
		/// Creates meshinfo to render
		/// </summary>
		/// <param name="material">A material with the current material properties</param>
		/// <param name="positions">The position data</param>
		/// <param name="normals">The normal data</param>
		/// <param name="colors">The color data</param>
		/// <param name="uvs">The uv data</param>
		/// <returns>A mesh info for the mesh</returns>
		public MeshInfo Process(NJS_MATERIAL material, List<IOVtx> positions, List<IOVtx> normals, List<IOVtx> colors, List<IOVtx> uvs)
		{
			// Setting the material properties according to the parameters
			foreach (var param in Parameters)
			{
				switch (param.Type)
				{ 
					case ParameterType.BlendAlpha: 
						var blend = param as BlendAlphaParameter; 
						material.SourceAlpha = blend.NJSourceAlpha; 
						material.DestinationAlpha = blend.NJDestAlpha;
						break;
					case ParameterType.AmbientColor:
						var ambientCol = param as AmbientColorParameter; 
						material.DiffuseColor = ambientCol.AmbientColor.SystemCol; 
						break;
					case ParameterType.Texture:
						var tex = param as TextureParameter;
						material.TextureID = tex.TextureId;
						material.FlipU = tex.Tile.HasFlag(GCTileMode.MirrorU);
						material.FlipV = tex.Tile.HasFlag(GCTileMode.MirrorV);
						material.ClampU = tex.Tile.HasFlag(GCTileMode.WrapU);
						material.ClampV = tex.Tile.HasFlag(GCTileMode.WrapV);

						// No idea why, but ok
						material.ClampU &= tex.Tile.HasFlag(GCTileMode.Unk_1);
						material.ClampV &= tex.Tile.HasFlag(GCTileMode.Unk_1);
						break;
					case ParameterType.TexCoordGen:
						var gen = param as TexCoordGenParameter;
						material.EnvironmentMap = gen.TexGenSrc == GCTexGenSrc.Normal;
						break;
				}
			}

			// Filtering out the double loops
			var corners = new List<Loop>();
			var polys = new List<Poly>();

			foreach (var prim in Primitives)
			{
				var j = 0;
				var indices = new ushort[prim.Loops.Count];
				
				foreach (var l in prim.Loops)
				{
					var t = (ushort)corners.FindIndex(x => x.Equals(l));
					if (t == 0xFFFF)
					{
						indices[j] = (ushort)corners.Count;
						corners.Add(l);
					}
					else
					{
						indices[j] = t;
					}

					j++;
				}

				// Creating the polygons
				if (prim.PrimitiveType == GCPrimitiveType.Triangles)
				{
					for (var i = 0; i < indices.Length; i += 3)
					{ 
						var t = new Triangle
						{
							Indexes =
							{
								[0] = indices[i],
								[1] = indices[i + 1],
								[2] = indices[i + 2]
							}
						};
						
						polys.Add(t);
					}
				}
				else if (prim.PrimitiveType == GCPrimitiveType.TriangleStrip)
				{
					polys.Add(new Strip(indices, false));
				}
			}

			// Creating the vertex data
			var vertData = new VertexData[corners.Count];
			var hasNormals = normals != null;
			var hasColors = colors != null;
			var hasUVs = uvs != null;

			for (var i = 0; i < corners.Count; i++)
			{
				var l = corners[i];
				vertData[i] = new VertexData(
					(Vector3)positions[l.PositionIndex],
					hasNormals ? (Vector3)normals[l.NormalIndex] : new Vector3(0, 1, 0),
					hasColors ? (Color)colors[l.Color0Index] : new Color(255, 255, 255, 255),
					hasUVs ? (UV)uvs[l.UV0Index] : new UV(0, 0)
					);
			}

			return new MeshInfo(new NJS_MATERIAL(material), polys.ToArray(), vertData, hasUVs, hasColors);
		}

		public GCMesh Clone()
		{
			//throw new NotImplementedException();
			var result = (GCMesh)MemberwiseClone();
			//result.Vertices = new List<Vertex>(Vertices.Count);
			//foreach (Vertex item in Vertices)
			//	result.Vertices.Add(item.Clone());
			//result.Normals = new List<Vertex>(Normals.Count);
			//foreach (Vertex item in Normals)
			//	result.Normals.Add(item.Clone());
			//result.Diffuse = new List<Color>(Diffuse);
			//result.Specular = new List<Color>(Specular);
			//result.UserFlags = new List<uint>(UserFlags);
			//result.NinjaFlags = new List<uint>(NinjaFlags);
			return result;
		}
	}
}
