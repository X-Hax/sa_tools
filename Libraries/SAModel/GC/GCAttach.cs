using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SAModel.GC
{
	/// <summary>
	/// An attach/mesh using the Gamecube format
	/// </summary>
	[Serializable]
	public class GCAttach : Attach
	{
		/// <summary>
		/// The seperate sets of vertex data in this attach
		/// </summary>
		public List<GCVertexSet> vertexData;
		public string VertexName { get; set; }

		/// <summary>
		/// The meshes with opaque rendering properties
		/// </summary>
		public List<GCMesh> opaqueMeshes;
		public string OpaqueMeshName { get; set; }

		/// <summary>
		/// The meshes with translucent rendering properties
		/// </summary>
		public List<GCMesh> translucentMeshes;
		public string TranslucentMeshName { get; set; }


		/// <summary>
		/// Create a new empty GC attach
		/// </summary>
		public GCAttach()
		{
			Name = "gcattach_" + Extensions.GenerateIdentifier();

			vertexData = new List<GCVertexSet>();
			VertexName = "vertex_" + Extensions.GenerateIdentifier();
			Bounds = new BoundingSphere();
		}

		public GCAttach(bool HasOPoly, bool HasTPoly)
			:this()
		{
			if (HasOPoly)
			{
				opaqueMeshes = new List<GCMesh>();
				OpaqueMeshName = "opoly_" + Extensions.GenerateIdentifier();
			}
			if (HasTPoly)
			{
				translucentMeshes = new List<GCMesh>();
				TranslucentMeshName = "tpoly_" + Extensions.GenerateIdentifier();
			}
		}

		/// <summary>
		/// Reads a GC attach from a file
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The address at which the attach is located</param>
		/// <param name="imageBase">The imagebase of the file</param>
		/// <param name="labels">The labels of the file</param>
		/// 
		public GCAttach(byte[] file, int address, uint imageBase)
			: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}
		public GCAttach(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "attach_" + address.ToString("X8");

			// The struct is 36/0x24 bytes long

			uint vertexAddress = ByteConverter.ToUInt32(file, address) - imageBase;
			//uint gap = ByteConverter.ToUInt32(file, address + 4);
			int opaqueAddress = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			int translucentAddress = (int)(ByteConverter.ToInt32(file, address + 12) - imageBase);

			int opaqueCount = ByteConverter.ToInt16(file, address + 16);
			int translucentCount = ByteConverter.ToInt16(file, address + 18);

			Bounds = new BoundingSphere(file, address + 20);

			// reading vertex data
			vertexData = new List<GCVertexSet>();
			GCVertexSet vertexSet = new GCVertexSet(file, vertexAddress, imageBase, labels);
			if (labels.ContainsKey((int)vertexAddress))
				VertexName = labels[(int)vertexAddress];
			else
				VertexName = "vertex_" + vertexAddress.ToString("X8");
			while (vertexSet.attribute != GCVertexAttribute.Null)
			{
				vertexData.Add(vertexSet);
				vertexAddress += 16;
				vertexSet = new GCVertexSet(file, vertexAddress, imageBase, labels);
			}
			// reading geometry
			GCIndexAttributeFlags indexFlags = GCIndexAttributeFlags.HasPosition;
			opaqueMeshes = new List<GCMesh>();
			if (opaqueCount != 0)
			{
				if (labels.ContainsKey(opaqueAddress))
					OpaqueMeshName = labels[opaqueAddress];
				else
					OpaqueMeshName = "opoly_" + opaqueAddress.ToString("X8");
			}
			for (int i = 0; i < opaqueCount; i++)
			{
				GCMesh mesh = new GCMesh(file, opaqueAddress, imageBase, indexFlags, labels);

				GCIndexAttributeFlags? t = mesh.IndexFlags;
				if (t.HasValue) indexFlags = t.Value;

				opaqueMeshes.Add(mesh);
				opaqueAddress += 16;
			}
			translucentMeshes = new List<GCMesh>();
			if (translucentCount != 0)
			{
				if (labels.ContainsKey(translucentAddress))
					TranslucentMeshName = labels[translucentAddress];
				else
					TranslucentMeshName = "tpoly_" + translucentAddress.ToString("X8");
			}
			for (int i = 0; i < translucentCount; i++)
			{
				GCMesh mesh = new GCMesh(file, translucentAddress, imageBase, indexFlags, labels);

				GCIndexAttributeFlags? t = mesh.IndexFlags;
				if (t.HasValue) indexFlags = t.Value;

				translucentMeshes.Add(mesh);
				translucentAddress += 16;
			}
		}

		/// <summary>
		/// Writes the attaches contents into a byte array
		/// </summary>
		/// <param name="imageBase">The files imagebase</param>
		/// <param name="DX">Unused</param>
		/// <param name="labels">The files labels</param>
		/// <param name="address"></param>
		/// <returns></returns>
		public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			byte[] output;
			using (MemoryStream strm = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(strm);

				writer.Write(new byte[16]); // address placeholders
				writer.Write((ushort)opaqueMeshes.Count);
				writer.Write((ushort)translucentMeshes.Count);
				writer.Write(Bounds.GetBytes());

				// writing vertex data
				foreach (GCVertexSet vtx in vertexData)
				{
					vtx.WriteData(writer);
				}

				uint vtxAddr = (uint)writer.BaseStream.Length + imageBase;
				if (labels.ContainsKey(VertexName))
					vtxAddr = labels[VertexName];
				else
				{
					uint[] vertAddrs = new uint[writer.BaseStream.Length + imageBase];
					for (int i = 0; i < vertexData.Count; i++)
					{
						if (vertexData[i].data != null)
						{
							if (labels.ContainsKey(vertexData[i].DataName))
								vertAddrs[i] = labels[vertexData[i].DataName];
							else
								labels.Add(vertexData[i].DataName, vertAddrs[i]);
						}							
					}
					labels.Add(VertexName, vtxAddr);
				}
				// writing vertex attributes
				foreach (GCVertexSet vtx in vertexData)
				{
					vtx.WriteAttribute(writer, imageBase, njOffsets);
				}
				//foreach (IOVtx data in vertexData)
				//{
				//	data.WriteAttribute(writer, imageBase, njOffsets);
				//}

				writer.Write((byte)255);
				writer.Write(new byte[15]); // empty vtx attribute

				// writing geometry data
				GCIndexAttributeFlags indexFlags = GCIndexAttributeFlags.HasPosition;
				if (opaqueMeshes != null)
				{
					foreach (GCMesh m in opaqueMeshes)
					{
						GCIndexAttributeFlags? t = m.IndexFlags;
						if (t.HasValue) indexFlags = t.Value;
						m.WriteData(writer, indexFlags);
					}
				}
				if (translucentMeshes != null)
				{
					foreach (GCMesh m in translucentMeshes)
					{
						GCIndexAttributeFlags? t = m.IndexFlags;
						if (t.HasValue) indexFlags = t.Value;
						m.WriteData(writer, indexFlags);
					}
				}

				// writing geometry properties
				uint opaqueAddress = (uint)writer.BaseStream.Length + imageBase;
				if (opaqueMeshes.Count != 0)
				{
					if (labels.ContainsKey(OpaqueMeshName))
						opaqueAddress = labels[OpaqueMeshName];
					else
					{
						uint[] paramAddrs = new uint[opaqueMeshes.Count];
						uint[] primAddrs = new uint[opaqueMeshes.Count];
						for (int i = 0; i < opaqueMeshes.Count; i++)
						{
							if (opaqueMeshes[i].parameters != null)
							{
								if (labels.ContainsKey(opaqueMeshes[i].ParameterName))
									paramAddrs[i] = labels[opaqueMeshes[i].ParameterName];
								else
									labels.Add(opaqueMeshes[i].ParameterName, paramAddrs[i]);
							}
						}

						for (int i = 0; i < opaqueMeshes.Count; i++)
						{
							if (opaqueMeshes[i].primitives != null)
							{
								if (labels.ContainsKey(opaqueMeshes[i].PrimitiveName))
									primAddrs[i] = labels[opaqueMeshes[i].PrimitiveName];
								else
									labels.Add(opaqueMeshes[i].PrimitiveName, primAddrs[i]);
							}
						}
						labels.Add(OpaqueMeshName, opaqueAddress);
					}
				}
				foreach (GCMesh m in opaqueMeshes)
					m.WriteProperties(writer, imageBase, njOffsets);
				uint translucentAddress = (uint)writer.BaseStream.Length + imageBase;
				if (translucentMeshes.Count != 0)
				{
					if (labels.ContainsKey(TranslucentMeshName))
						translucentAddress = labels[TranslucentMeshName];
					else
					{
						uint[] paramAddrs = new uint[translucentMeshes.Count];
						uint[] primAddrs = new uint[translucentMeshes.Count];
						for (int i = 0; i < translucentMeshes.Count; i++)
						{
							if (translucentMeshes[i].parameters != null)
							{
								if (labels.ContainsKey(translucentMeshes[i].ParameterName))
									paramAddrs[i] = labels[translucentMeshes[i].ParameterName];
								else
									labels.Add(translucentMeshes[i].ParameterName, paramAddrs[i]);
							}
						}

						for (int i = 0; i < translucentMeshes.Count; i++)
						{
							if (translucentMeshes[i].primitives != null)
							{
								if (labels.ContainsKey(translucentMeshes[i].PrimitiveName))
									primAddrs[i] = labels[translucentMeshes[i].PrimitiveName];
								else
									labels.Add(translucentMeshes[i].PrimitiveName, primAddrs[i]);
							}
						}
						labels.Add(TranslucentMeshName, translucentAddress);
					}
				}
					foreach (GCMesh m in translucentMeshes)
						m.WriteProperties(writer, imageBase, njOffsets);

				// write POF0 Offsets
				if (vtxAddr != 0)
					njOffsets.Add(imageBase);
				if (opaqueAddress != 0)
					njOffsets.Add(imageBase + 8);
				if (translucentAddress != 0)
					njOffsets.Add(imageBase + 0xC);

				// replacing the placeholders
				writer.Seek(0, SeekOrigin.Begin);
				writer.Write(vtxAddr);
				writer.Write(0);
				writer.Write(opaqueAddress);
				writer.Write(translucentAddress);
				writer.Seek(0, SeekOrigin.End);

				output = strm.ToArray();
			}

			address = 0;
			labels.Add(Name, imageBase);
			return output;
		}

		/// <summary>
		/// Processes the vertex data to be rendered
		/// </summary>
		public override void ProcessVertexData()
		{
			List<MeshInfo> meshInfo = new List<MeshInfo>();

			List<IOVtx> positions = vertexData.Find(x => x.attribute == GCVertexAttribute.Position)?.data;
			List<IOVtx> normals = vertexData.Find(x => x.attribute == GCVertexAttribute.Normal)?.data;
			List<IOVtx> colors = vertexData.Find(x => x.attribute == GCVertexAttribute.Color0)?.data;
			List<IOVtx> uvs = vertexData.Find(x => x.attribute == GCVertexAttribute.Tex0)?.data;

			NJS_MATERIAL mat = new NJS_MATERIAL();

				mat.UseAlpha = false;
				foreach (GCMesh m in opaqueMeshes)
					meshInfo.Add(m.Process(mat, positions, normals, colors, uvs));

				mat.UseAlpha = true;
				foreach (GCMesh m in translucentMeshes)
					meshInfo.Add(m.Process(mat, positions, normals, colors, uvs));

			MeshInfo = meshInfo.ToArray();
		}

		/// <summary>
		/// Creates a C Struct string identical to the data given (WIP)
		/// </summary>
		/// <param name="DX">Unused</param>
		/// <returns></returns>
		public override string ToStruct(bool DX)
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(vertexData != null ? VertexName : "NULL");
			result.Append(", ");
			result.Append(opaqueMeshes.Count != 0 ? OpaqueMeshName : "NULL");
			result.Append(", ");
			result.Append(translucentMeshes.Count != 0 ? TranslucentMeshName : "NULL");
			result.Append(", ");
			result.Append(Bounds.ToStruct());
			result.Append(" }");
			return result.ToString();
		}

		/// <summary>
		/// WIP
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="DX">Unused</param>
		/// <param name="labels"></param>
		/// <param name="textures"></param>
		public override void ToStructVariables(TextWriter writer, bool DX, List<string> labels, string[] textures = null)
		{
			if (vertexData.Count != 0 && !labels.Contains(VertexName))
			{
				for (int i = 0; i < vertexData.Count; i++)
				{
					if (!labels.Contains(vertexData[i].DataName))
					{
						labels.Add(vertexData[i].DataName);
						switch (vertexData[i].attribute)
						{
							case GCVertexAttribute.Position:
							case GCVertexAttribute.Normal:
								writer.Write("Vector3 ");
								break;
							case GCVertexAttribute.Color0:
								writer.Write("Color ");
								break;
							case GCVertexAttribute.Tex0:
								writer.Write("UV ");
								break;
						}
								writer.Write(vertexData[i].DataName);
						writer.WriteLine("[] = {");
						List<string> vtx = new List<string>(vertexData[i].data.Count);
						switch (vertexData[i].attribute)
						{
							case GCVertexAttribute.Position:
							case GCVertexAttribute.Normal:
								{
									foreach (Vector3 item in vertexData[i].data)
										vtx.Add(item.ToStruct());
								}
								break;
							case GCVertexAttribute.Color0:
								{
									foreach (Color item in vertexData[i].data)
										vtx.Add(item.ToStruct());
								}
								break;
							case GCVertexAttribute.Tex0:
								{
									foreach (UV item in vertexData[i].data)
										vtx.Add(item.ToStruct());
								}
								break;
						}
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", vtx.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();

					}
				}
				labels.Add(VertexName);
				writer.Write("Sint32 ");
				writer.Write(VertexName);
				writer.Write("[] = { ");
				List<string> chunks = new List<string>(vertexData.Count);
				foreach (GCVertexSet item in vertexData)
					chunks.Add(item.ToStruct());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", chunks.ToArray()));
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			if (opaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
			{
				for (int i = 0; i < opaqueMeshes.Count; i++)
				{
					if (!labels.Contains(opaqueMeshes[i].ParameterName))
					{
						labels.Add(opaqueMeshes[i].ParameterName);
						writer.Write("Sint32 ");
						writer.Write(opaqueMeshes[i].ParameterName);
						writer.WriteLine("[] = {");
						List<string> param = new List<string>(opaqueMeshes[i].parameters.Count);
						foreach (GCParameter item in opaqueMeshes[i].parameters)
						param.Add(item.ToStruct());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", param.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < opaqueMeshes.Count; i++)
				{
					if (!labels.Contains(opaqueMeshes[i].PrimitiveName))
					{
						labels.Add(opaqueMeshes[i].PrimitiveName);
						writer.Write("Sint16 ");
						writer.Write(opaqueMeshes[i].PrimitiveName);
						writer.WriteLine("[] = {");
						//List<string> prim = new List<string>(opaqueMeshes[i].primitives.Count);
						//foreach (GCPrimitive item in opaqueMeshes[i].primitives)
						//prim.Add(item.ToStruct());
						//writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", prim.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				labels.Add(OpaqueMeshName);
				writer.Write("Sint32 ");
				writer.Write(OpaqueMeshName);
				writer.Write("[] = { ");
				List<string> chunks = new List<string>();
				foreach (GCMesh item in opaqueMeshes)
				chunks.Add(item.ToStruct());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", chunks.ToArray()));
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			if (translucentMeshes.Count != 0 && !labels.Contains(TranslucentMeshName))
			{
				for (int i = 0; i < translucentMeshes.Count; i++)
				{
					if (!labels.Contains(translucentMeshes[i].ParameterName))
					{
						labels.Add(translucentMeshes[i].ParameterName);
						writer.Write("Sint32 ");
						writer.Write(translucentMeshes[i].ParameterName);
						writer.WriteLine("[] = {");
						List<string> param = new List<string>(translucentMeshes[i].parameters.Count);
						foreach (GCParameter item in translucentMeshes[i].parameters)
						param.Add(item.ToStruct());
						writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", param.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				for (int i = 0; i < translucentMeshes.Count; i++)
				{
					if (!labels.Contains(translucentMeshes[i].PrimitiveName))
					{
						labels.Add(translucentMeshes[i].PrimitiveName);
						writer.Write("Sint16 ");
						writer.Write(translucentMeshes[i].PrimitiveName);
						writer.WriteLine("[] = {");
						//List<string> prim = new List<string>(translucentMeshes[i].primitiveSize);
						//foreach (GCPrimitive item in translucentMeshes[i].primitives)
						//prim.Add(item.ToStruct(GCIndexAttributeFlags));
						//writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", prim.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				labels.Add(TranslucentMeshName);
				writer.Write("Sint32 ");
				writer.Write(TranslucentMeshName);
				writer.Write("[] = { ");
				List<string> chunks = new List<string>();
				foreach (GCMesh item in translucentMeshes)
				chunks.Add(item.ToStruct());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", chunks.ToArray()));
				writer.WriteLine(" };");
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			writer.Write("NJS_GC_MODEL ");
			writer.Write(Name);
			writer.Write(" = ");
			writer.Write(ToStruct(DX));
			writer.WriteLine(";");
		}

		#region Unused

		/// <summary>
		/// Unused, does not apply to SA2
		/// </summary>
		/// <param name="motion"></param>
		/// <param name="frame"></param>
		/// <param name="animindex"></param>
		public override void ProcessShapeMotionVertexData(NJS_MOTION motion, float frame, int animindex)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Unused
		/// </summary>
		/// <returns></returns>
		public override Attach Clone()
		{
			GCAttach result = (GCAttach)MemberwiseClone();
			if (vertexData != null)
			{
				result.vertexData = new List<GCVertexSet>(vertexData.Count);
				foreach (GCVertexSet item in vertexData)
					result.vertexData.Add(item.Clone());
			}
			if (opaqueMeshes != null)
			{
				result.opaqueMeshes = new List<GCMesh>(opaqueMeshes.Count);
				foreach (GCMesh item in opaqueMeshes)
					result.opaqueMeshes.Add(item.Clone());
			}

			if (translucentMeshes != null)
			{
				result.translucentMeshes = new List<GCMesh>(translucentMeshes.Count);
				foreach (GCMesh item in translucentMeshes)
					result.translucentMeshes.Add(item.Clone());
			}
			result.Bounds = Bounds.Clone();
			return result;
		}

		#endregion
	}
}
