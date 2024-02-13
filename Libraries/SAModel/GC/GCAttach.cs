using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

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
		public uint gap { get; set; }
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

		public static int testCounter = 0;
		public static int testCounterPrev = 0;
		public static int testCounterDifference = 0;

		public List<GCSKinMeshElement> gcSkinMeshElements = new List<GCSKinMeshElement>();

		public class GCSKinMeshElement
		{
			//Both the data and the mesh struct here are 0x20 aligned
			public ushort elementType;
			public ushort dataType; //?
			public ushort startingIndex;
			public ushort indexCount;
			public int offset0;
			public int offset1;

			public List<GCSkinMeshVertPosNrm> posNrms = new List<GCSkinMeshVertPosNrm>();
			public List<GCSkinMeshVertUnk> unkData = new List<GCSkinMeshVertUnk>();

			//Testing
			public List<Vector3> posList = new List<Vector3>();
			public List<Vector3> nrmList = new List<Vector3>();
			public List<System.Numerics.Vector2> unkList = new List<System.Numerics.Vector2>();

			public GCSKinMeshElement() { }

			public GCSKinMeshElement(byte[] file, int address, uint imageBase, Dictionary<int, string> labels)
			{
				elementType = ByteConverter.ToUInt16(file, (int)address + 0x0);
				dataType = ByteConverter.ToUInt16(file, (int)address + 0x2);
				startingIndex = ByteConverter.ToUInt16(file, (int)address + 0x4);
				indexCount = ByteConverter.ToUInt16(file, (int)address + 0x6);
				offset0 = (int)(ByteConverter.ToInt32(file, (int)address + 0x8) - imageBase);
				offset1 = (int)(ByteConverter.ToInt32(file, (int)address + 0xC) - imageBase);

				//DEBUG
				if(!dataTypeValues.Contains(dataType))
				{
					dataTypeValues.Add(dataType);
				}
				testCounterDifference = -(testCounter - startingIndex);
				testCounterPrev = testCounter;
				testCounter = startingIndex;
				//DEBUG

				switch (elementType)
				{
					case 0:
						for(int i = 0; i < indexCount; i++)
						{
							posNrms.Add(new GCSkinMeshVertPosNrm() { 
								posX = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x0), 
								posY = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x2),
								posZ = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x4),
								nrmX = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x6),
								nrmY = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x8),
								nrmZ = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0xA),
							});
						}
						break;
					case 1:
						for (int i = 0; i < indexCount; i++)
						{
							posNrms.Add(new GCSkinMeshVertPosNrm()
							{
								posX = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x0),
								posY = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x2),
								posZ = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x4),
								nrmX = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x6),
								nrmY = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x8),
								nrmZ = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0xA),
							});
						}
						for (int i = 0; i < indexCount; i++)
						{
							unkData.Add(new GCSkinMeshVertUnk()
							{
								sht0 = ByteConverter.ToInt16(file, (int)offset1 + (i * 0x4) + 0x0),
								sht1 = ByteConverter.ToInt16(file, (int)offset1 + (i * 0x4) + 0x2),
							});
						}
						break;
					case 2:
						for (int i = 0; i < indexCount; i++)
						{
							posNrms.Add(new GCSkinMeshVertPosNrm()
							{
								posX = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x0),
								posY = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x2),
								posZ = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x4),
								nrmX = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x6),
								nrmY = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0x8),
								nrmZ = ByteConverter.ToInt16(file, (int)offset0 + (i * 0xC) + 0xA),
							});
						}
						for (int i = 0; i < indexCount; i++)
						{
							unkData.Add(new GCSkinMeshVertUnk()
							{
								sht0 = ByteConverter.ToInt16(file, (int)offset1 + (i * 0x4) + 0x0),
								sht1 = ByteConverter.ToInt16(file, (int)offset1 + (i * 0x4) + 0x2),
							});
						}
						break;
					case 3:
					default:
						break;
				}
				foreach(var posNrm in posNrms)
				{
					posList.Add(new Vector3((float)(posNrm.posX / 255.0), (float)(posNrm.posY / 255.0), (float)(posNrm.posZ / 255.0)));
					nrmList.Add(new Vector3((float)(posNrm.nrmX / 255.0), (float)(posNrm.nrmY / 255.0), (float)(posNrm.nrmZ / 255.0)));
				}
				foreach(var unk in unkData)
				{
					unkList.Add(new System.Numerics.Vector2((float)(unk.sht0 / 255.0), (float)(unk.sht1 / 255.0)));
				}
				foreach(var unk in unkList)
				{
					if(unk.X < 0 || unk.Y < 0)
					{
						throw new Exception();
					}
				}
			}
		}

		public struct GCSkinMeshVertPosNrm
		{
			public short posX;
			public short posY;
			public short posZ;
			public short nrmX;
			public short nrmY;
			public short nrmZ;
		}

		public struct GCSkinMeshVertUnk
		{
			public short sht0;
			public short sht1;
		}

		public static List<int> dataTypeValues = new List<int>();

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
			uint gcSkinnedMeshAddress = ByteConverter.ToUInt32(file, address + 4) - imageBase;
			int opaqueAddress = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			int translucentAddress = (int)(ByteConverter.ToInt32(file, address + 12) - imageBase);

			int opaqueCount = ByteConverter.ToInt16(file, address + 16);
			int translucentCount = ByteConverter.ToInt16(file, address + 18);

			Bounds = new BoundingSphere(file, address + 20);

			if(gcSkinnedMeshAddress > 0)
			{
				GCSKinMeshElement skinMesh = new GCSKinMeshElement();
				int i = 0;
				do
				{
					skinMesh = new GCSKinMeshElement(file, (int)gcSkinnedMeshAddress + (0x10 * i) , imageBase, labels);
					gcSkinMeshElements.Add(skinMesh);
					i++;
				} while (skinMesh.elementType < 3);

				if (labels.ContainsKey((int)gcSkinnedMeshAddress))
					VertexName = labels[(int)gcSkinnedMeshAddress];
				else
					VertexName = "vertex_" + gcSkinnedMeshAddress.ToString("X8");

				var pos = new GCVertexSet(GCVertexAttribute.Position, GCDataType.Float32, GCStructType.Position_XYZ);
				foreach(var posData in gcSkinMeshElements[0].posList)
				{
					pos.data.Add(new Vector3(posData.x, posData.y, posData.z));
				}
				vertexData = new List<GCVertexSet>();
				vertexData.Add(pos);

				if (labels.ContainsKey((int)gcSkinnedMeshAddress * 2))
					OpaqueMeshName = labels[((int)gcSkinnedMeshAddress * 2)];
				else
					OpaqueMeshName = "opoly_" + ((int)gcSkinnedMeshAddress * 2).ToString("X8");

				opaqueMeshes = new List<GCMesh>();
				var primitives = new List<GCPrimitive>
				{
					new GCPrimitive(GCPrimitiveType.Triangles) { loops = new List<Loop>() { new Loop() { PositionIndex = 0 } } }
				};
				opaqueMeshes.Add(new GCMesh(new List<GCParameter>(), primitives));

			}

			// reading vertex data
			vertexData = new List<GCVertexSet>();
			if(vertexAddress > 0)
			{
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
				GCMesh mesh = new GCMesh(file, opaqueAddress, imageBase, labels, indexFlags);

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
				GCMesh mesh = new GCMesh(file, translucentAddress, imageBase, labels, indexFlags);

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
			List<byte> result = new List<byte>();

			uint vertexAddress = 0;
			if (vertexData != null && vertexData.Count > 0)
			{
				if (labels.ContainsKey(VertexName))
					vertexAddress = labels[VertexName];
				else
				{
					uint[] vdataAddrs = new uint[vertexData.Count];
					for (int i = 0; i < vertexData.Count; i++)
					{
						{
							if (labels.ContainsKey(vertexData[i].DataName))
								vdataAddrs[i] = labels[vertexData[i].DataName];
							else
							{
								result.Align(4);
								vdataAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(vertexData[i].DataName, vdataAddrs[i]);
								for (int j = 0; j < vertexData[i].data.Count; j++)
									result.AddRange(vertexData[i].data[j].GetBytes());
							}
						}
					}
					vertexAddress = (uint)result.Count + imageBase;
					labels.Add(VertexName, vertexAddress);
					for (int i = 0; i < vertexData.Count; i++)
					{
						//POF0
						if (vdataAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));

						result.AddRange(vertexData[i].GetBytes(vdataAddrs[i]));
					}
					result.Add(255);
					result.AddRange(new byte[15]);
				}
			}
			uint opolyAddress = 0;
			// writing geometry data
			GCIndexAttributeFlags indexFlags = GCIndexAttributeFlags.HasPosition;
			if (opaqueMeshes != null && opaqueMeshes.Count > 0)
			{
				if (labels.ContainsKey(OpaqueMeshName))
					opolyAddress = labels[OpaqueMeshName];
				else
				{
					uint[] paramAddrs = new uint[opaqueMeshes.Count];
					uint[] primAddrs = new uint[opaqueMeshes.Count];
					for (int i = 0; i < opaqueMeshes.Count; i++)
					{
						if (opaqueMeshes[i].parameters != null && opaqueMeshes[i].parameters.Count > 0)
						{
							if (labels.ContainsKey(opaqueMeshes[i].ParameterName))
								paramAddrs[i] = labels[opaqueMeshes[i].ParameterName];
							else
							{
								result.Align(4);
								paramAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(opaqueMeshes[i].ParameterName, paramAddrs[i]);
								for (int j = 0; j < opaqueMeshes[i].parameters.Count; j++)
									result.AddRange(opaqueMeshes[i].parameters[j].GetBytes());
							}
						}
					}
					for (int i = 0; i < opaqueMeshes.Count; i++)
					{
						if (opaqueMeshes[i].primitives != null && opaqueMeshes[i].primitives.Count > 0)
						{
							if (labels.ContainsKey(opaqueMeshes[i].PrimitiveName))
								primAddrs[i] = labels[opaqueMeshes[i].PrimitiveName];
							else
							{
								result.Align(32);
								primAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(opaqueMeshes[i].PrimitiveName, primAddrs[i]);
								GCIndexAttributeFlags? t = opaqueMeshes[i].IndexFlags;
								if (t.HasValue) indexFlags = t.Value;
								for (int j = 0; j < opaqueMeshes[i].primitives.Count; j++)
									result.AddRange(opaqueMeshes[i].primitives[j].GetBytes(indexFlags));
							}
						}
					}
					result.Align(32);
					opolyAddress = (uint)result.Count + imageBase;
					labels.Add(OpaqueMeshName, opolyAddress);
					for (int i = 0; i < opaqueMeshes.Count; i++)
					{
						//POF0
						if (paramAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase));
						if (primAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));

						result.AddRange(opaqueMeshes[i].GetBytes(paramAddrs[i], primAddrs[i], indexFlags));
					}
				}
			}
			result.Align(4);
			uint tpolyAddress = 0;
			if (translucentMeshes != null && translucentMeshes.Count > 0)
			{
				if (labels.ContainsKey(TranslucentMeshName))
					tpolyAddress = labels[TranslucentMeshName];
				else
				{
					uint[] paramAddrs = new uint[translucentMeshes.Count];
					uint[] primAddrs = new uint[translucentMeshes.Count];
					for (int i = 0; i < translucentMeshes.Count; i++)
					{
						if (translucentMeshes[i].parameters != null && translucentMeshes[i].parameters.Count > 0)
						{
							if (labels.ContainsKey(translucentMeshes[i].ParameterName))
								paramAddrs[i] = labels[translucentMeshes[i].ParameterName];
							else
							{
								result.Align(4);
								paramAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(translucentMeshes[i].ParameterName, paramAddrs[i]);
								for (int j = 0; j < translucentMeshes[i].parameters.Count; j++)
									result.AddRange(translucentMeshes[i].parameters[j].GetBytes());
							}
						}
					}
					for (int i = 0; i < translucentMeshes.Count; i++)
					{
						if (translucentMeshes[i].primitives != null && translucentMeshes[i].primitives.Count > 0)
						{
							if (labels.ContainsKey(translucentMeshes[i].PrimitiveName))
								primAddrs[i] = labels[translucentMeshes[i].PrimitiveName];
							else
							{
								result.Align(32);
								primAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(translucentMeshes[i].PrimitiveName, primAddrs[i]);
								GCIndexAttributeFlags? t = translucentMeshes[i].IndexFlags;
								if (t.HasValue) indexFlags = t.Value;
								for (int j = 0; j < translucentMeshes[i].primitives.Count; j++)
									result.AddRange(translucentMeshes[i].primitives[j].GetBytes(indexFlags));
							}
						}
					}
					result.Align(32);
					tpolyAddress = (uint)result.Count + imageBase;
					labels.Add(TranslucentMeshName, tpolyAddress);
					for (int i = 0; i < translucentMeshes.Count; i++)
					{
						//POF0
						if (paramAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase));
						if (primAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));

						result.AddRange(translucentMeshes[i].GetBytes(paramAddrs[i], primAddrs[i], indexFlags));
					}
				}
			}

				result.Align(4);
				address = (uint)result.Count;

				//POF0
				if (vertexAddress != 0)
					njOffsets.Add((uint)(result.Count + imageBase));
				if (opolyAddress != 0)
					njOffsets.Add((uint)(result.Count + imageBase + 0x8));
				if (tpolyAddress != 0)
					njOffsets.Add((uint)(result.Count + imageBase + 0xC));

				result.AddRange(ByteConverter.GetBytes(vertexAddress));
				result.AddRange(new byte[4]);
				result.AddRange(ByteConverter.GetBytes(opolyAddress));
				result.AddRange(ByteConverter.GetBytes(tpolyAddress));
				result.AddRange(ByteConverter.GetBytes((ushort)opaqueMeshes.Count));
				result.AddRange(ByteConverter.GetBytes((ushort)translucentMeshes.Count));
				result.AddRange(Bounds.GetBytes());
				labels.Add(Name, address + imageBase);
				return result.ToArray();
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
			result.Append(gap);
			result.Append(", ");
			result.Append(opaqueMeshes.Count != 0 ? OpaqueMeshName : "NULL");
			result.Append(", ");
			result.Append(translucentMeshes.Count != 0 ? TranslucentMeshName : "NULL");
			result.Append(", ");
			result.Append(opaqueMeshes.Count != 0 ? "LengthOfArray<Uint16>(" + OpaqueMeshName + ")" : "0");
			result.Append(", ");
			result.Append(translucentMeshes.Count != 0 ? "LengthOfArray<Uint16>(" + TranslucentMeshName + ")" : "0");
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
								writer.Write("SA2B_PositionData ");
								break;
							case GCVertexAttribute.Normal:
								writer.Write("SA2B_NormalData ");
								break;
							case GCVertexAttribute.Color0:
								writer.Write("SA2B_Color0Data ");
								break;
							case GCVertexAttribute.Tex0:
								writer.Write("SA2B_Tex0Data ");
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
				writer.Write("SA2B_VertexData ");
				writer.Write(VertexName);
				writer.Write("[] = {");
				List<string> chunks = new List<string>(vertexData.Count);
				foreach (GCVertexSet item in vertexData)
					chunks.Add(item.ToStruct());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", chunks.ToArray()) + ",");
				writer.WriteLine("\t" + string.Join(Environment.NewLine + "\t", "{ 255, 0, 0, 0, NULL, 0 }"));
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			GCIndexAttributeFlags indexFlags = GCIndexAttributeFlags.HasPosition;
			if (opaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
			{
				for (int i = 0; i < opaqueMeshes.Count; i++)
				{
					for (int j = 0; j < opaqueMeshes[i].parameters.Count; j++)
					{
						if (opaqueMeshes[i].parameters[j] != null && !labels.Contains(opaqueMeshes[i].ParameterName))
						{
							labels.Add(opaqueMeshes[i].ParameterName);
							writer.Write("SA2B_ParameterData ");
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
				}
				for (int i = 0; i < opaqueMeshes.Count; i++)
				{
					if (!labels.Contains(opaqueMeshes[i].PrimitiveName))
					{
						labels.Add(opaqueMeshes[i].PrimitiveName);
						writer.Write("Uint8 ");
						writer.Write(opaqueMeshes[i].PrimitiveName);
						writer.WriteLine("[] = {");
						List<byte> pbytes = new List<byte>();
						foreach (GCPrimitive item in opaqueMeshes[i].primitives)
						{
							GCIndexAttributeFlags? t = opaqueMeshes[i].IndexFlags;
							if (t.HasValue) indexFlags = t.Value;
							for (int k = 0; k < opaqueMeshes[i].primitives.Count; k++)
								pbytes.AddRange(opaqueMeshes[i].primitives[k].GetBytes(indexFlags));
						}
						byte[] cb = pbytes.ToArray();
						int dataSize = Convert.ToInt32(Math.Ceiling(decimal.Divide(cb.Length, 32)) * 32);
						int buffsize = (int)dataSize - cb.Length;
						List<string> s = new List<string>(dataSize);
						for (int j = 0; j < cb.Length; j++)
							s.Add("0x" + cb[j].ToString("X") + (cb[j] < 0 ? "u" : ""));
						for (int l = 0; l < buffsize; l++)
							s.Add("0x0");
						writer.Write(string.Join(", ", s.ToArray()));
						//
						//List<string> prim = new List<string>(opaqueMeshes[i].primitives.Count);
						//foreach (GCPrimitive item in opaqueMeshes[i].primitives)
						//prim.Add(item.ToStruct());
						//writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", prim.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				labels.Add(OpaqueMeshName);
				writer.Write("SA2B_GeometryData ");
				writer.Write(OpaqueMeshName);
				writer.Write("[] = {");
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
					for (int j = 0; j < translucentMeshes[i].parameters.Count; j++)
					{
						if (translucentMeshes[i].parameters[j] != null && !labels.Contains(translucentMeshes[i].ParameterName))
						{
							labels.Add(translucentMeshes[i].ParameterName);
							writer.Write("SA2B_ParameterData ");
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
				}
				for (int i = 0; i < translucentMeshes.Count; i++)
				{
					if (!labels.Contains(translucentMeshes[i].PrimitiveName))
					{
						labels.Add(translucentMeshes[i].PrimitiveName);
						writer.Write("Uint8 ");
						writer.Write(translucentMeshes[i].PrimitiveName);
						writer.WriteLine("[] = {");
						List<byte> pbytes = new List<byte>();
						foreach (GCPrimitive item in translucentMeshes[i].primitives)
						{
							GCIndexAttributeFlags? t = translucentMeshes[i].IndexFlags;
							if (t.HasValue) indexFlags = t.Value;
							for (int k = 0; k < translucentMeshes[i].primitives.Count; k++)
								pbytes.AddRange(translucentMeshes[i].primitives[k].GetBytes(indexFlags));
						}
						byte[] cb = pbytes.ToArray();
						int dataSize = Convert.ToInt32(Math.Ceiling(decimal.Divide(cb.Length, 32)) * 32);
						int buffsize = dataSize - cb.Length;
						List<string> s = new List<string>(dataSize);
						for (int j = 0; j < cb.Length; j++)
							s.Add("0x" + cb[j].ToString("X") + (cb[j] < 0 ? "u" : ""));
						for (int l = 0; l < buffsize; l++)
							s.Add("0x0");
						writer.Write(string.Join(", ", s.ToArray()));
						//writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", s.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				labels.Add(TranslucentMeshName);
				writer.Write("SA2B_GeometryData ");
				writer.Write(TranslucentMeshName);
				writer.Write("[] = {");
				List<string> chunks = new List<string>();
				foreach (GCMesh item in translucentMeshes)
					chunks.Add(item.ToStruct());
				writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", chunks.ToArray()));
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			writer.Write("SA2B_Model ");
			writer.Write(Name);
			writer.Write(" = ");
			writer.Write(ToStruct(DX));
			writer.WriteLine(";");
		}

		//WIP
		public void ToNJA(TextWriter writer, List<string> labels, string[] textures)
		{
			if (!labels.Contains(VertexName))
			{
				writer.WriteLine("VLIST      " + VertexName + "[]");
				writer.WriteLine("START" + Environment.NewLine);
				foreach (GCVertexSet item in vertexData)
					item.ToNJA(writer);
				foreach (GCVertexSet item in vertexData)
					item.RefToNJA(writer);
				writer.WriteLine("\tVertAttr     NULL" + ",");
				writer.WriteLine("\tElementSize  0" + ",");
				writer.WriteLine("\tPoints       0" + ",");
				writer.WriteLine("\tType         NULL" + ",");
				writer.WriteLine("\tName         NULL" + ",");
				writer.WriteLine("\tCheckSize    0" + ",");
				writer.Write("END" + Environment.NewLine + Environment.NewLine);
			}

			if (opaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
			{
				writer.WriteLine("OPLIST      " + OpaqueMeshName + "[]");
				writer.WriteLine("START" + Environment.NewLine);
				foreach (GCMesh item in opaqueMeshes)
					item.ToNJA(writer);
				foreach (GCMesh item in opaqueMeshes)
					item.RefToNJA(writer);
				writer.Write("END" + Environment.NewLine + Environment.NewLine);
			}

			if (translucentMeshes.Count != 0 && !labels.Contains(TranslucentMeshName))
			{
				writer.WriteLine("APLIST      " + TranslucentMeshName + "[]");
				writer.WriteLine("START" + Environment.NewLine);
				foreach (GCMesh item in translucentMeshes)
					item.ToNJA(writer);
				foreach (GCMesh item in translucentMeshes)
					item.RefToNJA(writer);
				writer.Write("END" + Environment.NewLine + Environment.NewLine);
			}

			writer.WriteLine("GINJAMODEL  " + Name + "[]");
			writer.WriteLine("START");
			writer.WriteLine("VList       " + VertexName + ",");
			if (opaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
				writer.WriteLine("OPList      " + OpaqueMeshName + ",");
			else
				writer.WriteLine("OPList      NULL,");
			if (translucentMeshes.Count != 0 && !labels.Contains(TranslucentMeshName))
				writer.WriteLine("APList      " + TranslucentMeshName + ",");
			else
				writer.WriteLine("APList      NULL,");
			writer.WriteLine("OPNum       " + opaqueMeshes.Count + ",");
			writer.WriteLine("APNum       " + translucentMeshes.Count + ",");
			writer.WriteLine("Center     " + Bounds.Center.X.ToNJA() + ", " + Bounds.Center.Y.ToNJA() + ", " + Bounds.Center.Z.ToNJA() + ",");
			writer.WriteLine("Radius     " + Bounds.Radius.ToNJA() + ",");
			writer.Write("END" + Environment.NewLine + Environment.NewLine);
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
