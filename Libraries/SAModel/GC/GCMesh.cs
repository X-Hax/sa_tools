using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public readonly List<GCParameter> parameters;

		/// <summary>
		/// The polygon data
		/// </summary>
		public readonly List<GCPrimitive> primitives;

		/// <summary>
		/// The index attribute flags of this mesh. If it has no IndexAttribParam, it will return null
		/// </summary>
		public GCIndexAttributeFlags? IndexFlags
		{
			get
			{
				IndexAttributeParameter index_param = (IndexAttributeParameter)parameters.Find(x => x.type == ParameterType.IndexAttributeFlags);
				if (index_param == null) return null;
				else return index_param.IndexAttributes;
			}
		}

		/// <summary>
		/// The location to which the parameters have been written
		/// </summary>
		private uint paramAddress;
		public string ParameterName { get; set; }

		/// <summary>
		/// The location to which the primitives have been written
		/// </summary>
		private uint primitiveAddress;
		public string PrimitiveName { get; set; }

		/// <summary>
		/// The amount of bytes which have been written for the primitives
		/// </summary>
		private uint primitiveSize;


		/// <summary>
		/// Create an empty mesh
		/// </summary>
		public GCMesh()
		{
			parameters = new List<GCParameter>();
			primitives = new List<GCPrimitive>();
		}

		/// <summary>
		/// Create a new mesh from existing primitives and parameters
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="primitives"></param>
		public GCMesh(List<GCParameter> parameters, List<GCPrimitive> primitives)
		{
			this.parameters = parameters;
			this.primitives = primitives;
		}

		public GCMesh(byte[] file, int address, uint imageBase, GCIndexAttributeFlags indexFlags)
		: this(file, address, imageBase, indexFlags, new Dictionary<int, string>())
		{
		}

		/// <summary>
		/// Read a mesh from a file
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The address at which the mesh is located</param>
		/// <param name="imageBase">The imagebase (used for when reading from an exe)</param>
		/// <param name="index">Indexattribute parameter of the previous mesh</param>
		public GCMesh(byte[] file, int address, uint imageBase, GCIndexAttributeFlags indexFlags, Dictionary<int, string> labels)
		{
			// getting the addresses and sizes
			int parameters_offset = (int)(ByteConverter.ToInt32(file, address) - imageBase);
			int parameters_count = ByteConverter.ToInt32(file, address + 4);

			int primitives_offset = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			int primitives_size = ByteConverter.ToInt32(file, address + 12);

			// reading the parameters
			parameters = new List<GCParameter>();
			if (labels.ContainsKey(parameters_offset))
				ParameterName = labels[parameters_offset];
			else
				ParameterName = "parameter_" + parameters_offset.ToString("X8");
			for (int i = 0; i < parameters_count; i++)
			{
				parameters.Add(GCParameter.Read(file, parameters_offset));
				parameters_offset += 8;
			}

			// getting the index attribute parameter
			GCIndexAttributeFlags? flags = IndexFlags;
			if (flags.HasValue)
				indexFlags = flags.Value;

			// reading the primitives
			primitives = new List<GCPrimitive>();
			if (labels.ContainsKey(primitives_offset))
				PrimitiveName = labels[primitives_offset];
			else
				PrimitiveName = "primitive_" + primitives_offset.ToString("X8");
			int end_pos = primitives_offset + primitives_size;

			while (primitives_offset < end_pos)
			{
				// if the primitive isnt valid
				if (file[primitives_offset] == 0) break;
				primitives.Add(new GCPrimitive(file, primitives_offset, indexFlags, out primitives_offset));
			}
		}

		/// <summary>
		/// Writes the parameters and primitives to a stream
		/// </summary>
		/// <param name="writer">The ouput stream</param>
		/// <param name="indexFlags">The index flags</param>
		public void WriteData(BinaryWriter writer, GCIndexAttributeFlags indexFlags)
		{
			paramAddress = (uint)writer.BaseStream.Length;

			foreach(GCParameter param in parameters)
			{
				param.Write(writer);
			}

			primitiveAddress = (uint)writer.BaseStream.Length;

			foreach(GCPrimitive prim in primitives)
			{
				prim.Write(writer, indexFlags);
			}

			primitiveSize = (uint)writer.BaseStream.Length - primitiveAddress;
		}

		/// <summary>
		/// Writes the location and sizes of
		/// </summary>
		/// <param name="writer">The output stream</param>
		/// <param name="imagebase">The imagebase</param>
		public void WriteProperties(BinaryWriter writer, uint imagebase, List<uint> njOffsets)
		{
			if (primitiveAddress == 0)
				throw new Exception("Data has not been written yet");
			if (primitiveSize == 0)
				throw new Exception("Geometry is empty; No primitives found");

			//POF0 offsets
			njOffsets.Add((uint)writer.BaseStream.Position);
			njOffsets.Add((uint)writer.BaseStream.Position + 8);

			writer.Write(paramAddress + imagebase);
			writer.Write((uint)parameters.Count);
			writer.Write(primitiveAddress + imagebase);
			writer.Write(primitiveSize);
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(parameters != null ? ParameterName : "NULL");
			result.Append(", ");
			result.Append(parameters != null ? (ushort)parameters.Count : 0);
			result.Append(", ");
			result.Append(primitives != null ? PrimitiveName : "NULL");
			result.Append(", ");
			result.Append(primitives != null ? (ushort)primitives.Count : 0);
			result.Append(" }");
			return result.ToString();
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
			// setting the material properties according to the parameters
			foreach (GCParameter param in parameters)
			{
				switch (param.type)
				{
					case ParameterType.BlendAlpha:
						BlendAlphaParameter blend = param as BlendAlphaParameter;
						material.SourceAlpha = blend.NJSourceAlpha;
						material.DestinationAlpha = blend.NJDestAlpha;
						break;
					case ParameterType.AmbientColor:
						AmbientColorParameter ambientCol = param as AmbientColorParameter;
						material.DiffuseColor = ambientCol.AmbientColor.SystemCol;
						break;
					case ParameterType.Texture:
						TextureParameter tex = param as TextureParameter;
						material.TextureID = tex.TextureID;
						material.FlipU = tex.Tile.HasFlag(GCTileMode.MirrorU);
						material.FlipV = tex.Tile.HasFlag(GCTileMode.MirrorV);
						material.ClampU = tex.Tile.HasFlag(GCTileMode.WrapU);
						material.ClampV = tex.Tile.HasFlag(GCTileMode.WrapV);

						// no idea why, but ok
						material.ClampU &= tex.Tile.HasFlag(GCTileMode.Unk_1);
						material.ClampV &= tex.Tile.HasFlag(GCTileMode.Unk_1);
						break;
					case ParameterType.TexCoordGen:
						TexCoordGenParameter gen = param as TexCoordGenParameter;
						material.EnvironmentMap = gen.TexGenSrc == GCTexGenSrc.Normal;
						break;
				}
			}

			// filtering out the double loops
			List<Loop> corners = new List<Loop>();
			List<Poly> polys = new List<Poly>();

			foreach (GCPrimitive prim in primitives)
			{
				int j = 0;
				ushort[] indices = new ushort[prim.loops.Count];
				foreach (Loop l in prim.loops)
				{
					ushort t = (ushort)corners.FindIndex(x => x.Equals(l));
					if (t == 0xFFFF)
					{
						indices[j] = (ushort)corners.Count;
						corners.Add(l);
					}
					else indices[j] = t;
					j++;
				}
				

				// creating the polygons
				if(prim.primitiveType == GCPrimitiveType.Triangles)
					for(int i = 0; i < indices.Length; i+= 3)
					{
						Triangle t = new Triangle();
						t.Indexes[0] = indices[i];
						t.Indexes[1] = indices[i + 1];
						t.Indexes[2] = indices[i + 2];
						polys.Add(t);
					}
				else if(prim.primitiveType == GCPrimitiveType.TriangleStrip)
					polys.Add(new Strip(indices, false));
			}

			// creating the vertex data
			VertexData[] vertData = new VertexData[corners.Count];
			bool hasNormals = normals != null;
			bool hasColors = colors != null;
			bool hasUVs = uvs != null;

			for(int i = 0; i < corners.Count; i++)
			{
				Loop l = corners[i];
				vertData[i] = new SAModel.VertexData(
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
			GCMesh result = (GCMesh)MemberwiseClone();
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
