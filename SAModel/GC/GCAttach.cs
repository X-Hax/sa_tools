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
			VertexData = new VertexData();
			GeometryData = new GeometryData();
		}

		public GCAttach(byte[] file, int address, uint imageBase)
		{
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

			ReadVertexAttributes(file, vertex_attribute_offset, imageBase);

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

		private void ReadVertexAttributes(byte[] file, uint address, uint imageBase)
		{
			VertexAttribute attrib = new VertexAttribute(file, address, imageBase);

			while (attrib.Attribute != GXVertexAttribute.Null + 8)
			{
				object attrib_data = GetVertexData(file, attrib);
				if (attrib_data != null)
					VertexData.SetAttributeData(attrib.Attribute, attrib_data);

				address += 16;
				attrib = new VertexAttribute(file, address, imageBase);
			}
		}

		private object GetVertexData(byte[] file, VertexAttribute attribute)
		{
			object attribute_data = null;

			switch (attribute.Attribute)
			{
				case GXVertexAttribute.Position:
					switch (attribute.ComponentCount)
					{
						case GXComponentCount.Position_XY:
							attribute_data = LoadVec2Data(file, attribute);
							break;
						case GXComponentCount.Position_XYZ:
							attribute_data = LoadVec3Data(file, attribute);
							break;
					}
					break;
				case GXVertexAttribute.Normal:
					switch (attribute.ComponentCount)
					{
						case GXComponentCount.Normal_XYZ:
							attribute_data = LoadVec3Data(file, attribute);
							break;
					}
					break;
				case GXVertexAttribute.Color0:
				case GXVertexAttribute.Color1:
					break;
				case GXVertexAttribute.Tex0:
				case GXVertexAttribute.Tex1:
				case GXVertexAttribute.Tex2:
				case GXVertexAttribute.Tex3:
				case GXVertexAttribute.Tex4:
				case GXVertexAttribute.Tex5:
				case GXVertexAttribute.Tex6:
				case GXVertexAttribute.Tex7:
					switch (attribute.ComponentCount)
					{
						case GXComponentCount.TexCoord_S:
							attribute_data = LoadSingleFloat(file, attribute);
							break;
						case GXComponentCount.TexCoord_ST:
							attribute_data = LoadVec2Data(file, attribute);
							break;
					}
					break;
			}

			return attribute_data;
		}

		private List<float> LoadSingleFloat(byte[] file, VertexAttribute attribute)
		{
			List<float> floatList = new List<float>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.Unsigned8:
						byte compu81 = file[cur_address];
						float compu81Float = (float)compu81 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(compu81Float);

						cur_address++;
						break;
					case GXDataType.Signed8:
						sbyte comps81 = (sbyte)file[cur_address];
						float comps81Float = (float)comps81 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(comps81Float);

						cur_address++;
						break;
					case GXDataType.Unsigned16:
						ushort compu161 = ByteConverter.ToUInt16(file, cur_address);
						float compu161Float = (float)compu161 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(compu161Float);

						cur_address += 2;
						break;
					case GXDataType.Signed16:
						short comps161 = ByteConverter.ToInt16(file, cur_address);
						float comps161Float = (float)comps161 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(comps161Float);

						cur_address += 2;
						break;
					case GXDataType.Float32:
						floatList.Add(ByteConverter.ToSingle(file, cur_address));

						cur_address += 4;
						break;
				}
			}

			return floatList;
		}

		private List<Vector2> LoadVec2Data(byte[] file, VertexAttribute attribute)
		{
			List<Vector2> vec2List = new List<Vector2>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.Unsigned8:
						byte compu81 = file[cur_address];
						byte compu82 = file[cur_address + 1];
						float compu81Float = (float)compu81 / (float)(1 << attribute.FractionalBitCount);
						float compu82Float = (float)compu82 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(compu81Float, compu82Float));

						cur_address += 2;
						break;
					case GXDataType.Signed8:
						sbyte comps81 = (sbyte)file[cur_address];
						sbyte comps82 = (sbyte)file[cur_address + 1];
						float comps81Float = (float)comps81 / (float)(1 << attribute.FractionalBitCount);
						float comps82Float = (float)comps82 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(comps81Float, comps82Float));

						cur_address += 2;
						break;
					case GXDataType.Unsigned16:
						ushort compu161 = ByteConverter.ToUInt16(file, cur_address);
						ushort compu162 = ByteConverter.ToUInt16(file, cur_address + 2);
						float compu161Float = (float)compu161 / (float)(1 << attribute.FractionalBitCount);
						float compu162Float = (float)compu162 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(compu161Float, compu162Float));

						cur_address += 4;
						break;
					case GXDataType.Signed16:
						short comps161 = ByteConverter.ToInt16(file, cur_address);
						short comps162 = ByteConverter.ToInt16(file, cur_address + 2);
						float comps161Float = (float)comps161 / (float)(1 << attribute.FractionalBitCount);
						float comps162Float = (float)comps162 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(comps161Float, comps162Float));

						cur_address += 4;
						break;
					case GXDataType.Float32:
						vec2List.Add(new Vector2(ByteConverter.ToSingle(file, cur_address),
							                     ByteConverter.ToSingle(file, cur_address + 4)));

						cur_address += 8;
						break;
				}
			}

			return vec2List;
		}

		private List<Vector3> LoadVec3Data(byte[] file, VertexAttribute attribute)
		{
			List<Vector3> vec3List = new List<Vector3>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.Unsigned8:
						byte compu81 = file[cur_address];
						byte compu82 = file[cur_address + 1];
						byte compu83 = file[cur_address + 2];
						float compu81Float = (float)compu81 / (float)(1 << attribute.FractionalBitCount);
						float compu82Float = (float)compu82 / (float)(1 << attribute.FractionalBitCount);
						float compu83Float = (float)compu83 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(compu81Float, compu82Float, compu83Float));

						cur_address += 3;
						break;
					case GXDataType.Signed8:
						sbyte comps81 = (sbyte)file[cur_address];
						sbyte comps82 = (sbyte)file[cur_address + 1];
						sbyte comps83 = (sbyte)file[cur_address + 2];
						float comps81Float = (float)comps81 / (float)(1 << attribute.FractionalBitCount);
						float comps82Float = (float)comps82 / (float)(1 << attribute.FractionalBitCount);
						float comps83Float = (float)comps83 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(comps81Float, comps82Float, comps83Float));

						cur_address += 3;
						break;
					case GXDataType.Unsigned16:
						ushort compu161 = ByteConverter.ToUInt16(file, cur_address);
						ushort compu162 = ByteConverter.ToUInt16(file, cur_address + 2);
						ushort compu163 = ByteConverter.ToUInt16(file, cur_address + 4);
						float compu161Float = (float)compu161 / (float)(1 << attribute.FractionalBitCount);
						float compu162Float = (float)compu162 / (float)(1 << attribute.FractionalBitCount);
						float compu163Float = (float)compu163 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(compu161Float, compu162Float, compu163Float));

						cur_address += 6;
						break;
					case GXDataType.Signed16:
						short comps161 = ByteConverter.ToInt16(file, cur_address);
						short comps162 = ByteConverter.ToInt16(file, cur_address + 2);
						short comps163 = ByteConverter.ToInt16(file, cur_address + 4);
						float comps161Float = (float)comps161 / (float)(1 << attribute.FractionalBitCount);
						float comps162Float = (float)comps162 / (float)(1 << attribute.FractionalBitCount);
						float comps163Float = (float)comps163 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(comps161Float, comps162Float, comps163Float));

						cur_address += 6;
						break;
					case GXDataType.Float32:
						vec3List.Add(new Vector3(ByteConverter.ToSingle(file, cur_address),
							                     ByteConverter.ToSingle(file, cur_address + 4),
												 ByteConverter.ToSingle(file, cur_address + 8)));

						cur_address += 12;
						break;
				}
			}

			return vec3List;
		}

		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			throw new System.NotImplementedException();
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
