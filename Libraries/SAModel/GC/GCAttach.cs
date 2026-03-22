using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public List<GCVertexSet> VertexData = new List<GCVertexSet>();
		public string VertexName { get; set; }
		/// <summary>
		/// For some reason, skinned GC meshes have a separate type of vertex data.
		/// </summary>
		public List<GCSkinVertexSet> vertexSkinData = new List<GCSkinVertexSet>();
		public string VertexSkinName { get; set; }
		/// <summary>
		/// The meshes with opaque rendering properties
		/// </summary>
		public List<GCMesh> OpaqueMeshes = new List<GCMesh>();
		public string OpaqueMeshName { get; set; }

		/// <summary>
		/// The meshes with translucent rendering properties
		/// </summary>
		public List<GCMesh> TranslucentMeshes = new List<GCMesh>();
		public string TranslucentMeshName { get; set; }

		public override bool HasWeight
		{
			get
			{
			//	if (VertexData?.Count > 0)
			//	{
			//		foreach (var set in VertexData)
			//		{
			//			if (set.Attribute == GCVertexAttribute.Position)
			//			{
			//				return false;
			//			}
			//		}

			//		return true;
			//	}
			//	else
			//	{
					return vertexSkinData?.Count > 0;
				//}
			}
		}

		/// <summary>
		/// Create a new empty GC attach
		/// </summary>
		public GCAttach()
		{
			Name = $"gcattach_{Extensions.GenerateIdentifier()}";

			VertexData = [];
			VertexName = $"vertex_{Extensions.GenerateIdentifier()}";
			Bounds = new BoundingSphere();
		}

		public GCAttach(bool hasOPoly, bool hasTPoly)
			:this()
		{
			if (hasOPoly)
			{
				OpaqueMeshes = [];
				OpaqueMeshName = $"opoly_{Extensions.GenerateIdentifier()}";
			}
			
			if (hasTPoly)
			{
				TranslucentMeshes = [];
				TranslucentMeshName = $"tpoly_{Extensions.GenerateIdentifier()}";
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
			if (labels.TryGetValue(address, out var name))
			{
				Name = name;
			}
			else
			{
				Name = "attach_" + address.ToString("X8");
			}

			// The struct is 36/0x24 bytes long
			var vertexAddrTest = ByteConverter.ToInt32(file, address);
			var vertexAddress = 0;
			if (vertexAddrTest != 0)
				vertexAddress = (int)(ByteConverter.ToInt32(file, address) - imageBase);
			//This part is meant to be a pointer for weight data.
			//SA2 doesn't utilize this part for GC models, but it is implemented for Billy Hatcher.
			var vertexSkinAddrTest = ByteConverter.ToInt32(file, address + 4);
			var gcSkinnedVertexAddress = 0;
			if (vertexSkinAddrTest != 0)
			    gcSkinnedVertexAddress = (int)(ByteConverter.ToInt32(file, address + 4) - imageBase);
			var opaqueAddress = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			var translucentAddress = (int)(ByteConverter.ToInt32(file, address + 12) - imageBase);

			int opaqueCount = ByteConverter.ToInt16(file, address + 16);
			int translucentCount = ByteConverter.ToInt16(file, address + 18);

			Bounds = new BoundingSphere(file, address + 20);

			// Reading vertex data
			VertexData = [];
			if (vertexAddress > 0)
			{
				var vertexSet = new GCVertexSet(file, (uint)vertexAddress, imageBase, labels);

				if (labels.TryGetValue(vertexAddress, out var vertexName))
				{
					VertexName = vertexName;
				}
				else
				{
					VertexName = $"vertex_{vertexAddress:X8}";
				}

				while (vertexSet.Attribute != GCVertexAttribute.Null)
				{
					VertexData.Add(vertexSet);
					vertexAddress += 16;
					vertexSet = new GCVertexSet(file, (uint)vertexAddress, imageBase, labels);
				}
			}

			if (gcSkinnedVertexAddress > 0)
			{
				vertexSkinData = new List<GCSkinVertexSet>();
				GCSkinVertexSet skinMesh;
				int i = 0;
				do
				{
					skinMesh = new GCSkinVertexSet(file, (int)gcSkinnedVertexAddress + (0x10 * i), imageBase, labels);
					vertexSkinData.Add(skinMesh);
					i++;
				} while (skinMesh.elementType < GCSkinAttribute.WeightStructEndMarker);

				if (labels.ContainsKey((int)gcSkinnedVertexAddress))
					VertexSkinName = labels[(int)gcSkinnedVertexAddress];
				else
					VertexSkinName = "vertexskin_" + gcSkinnedVertexAddress.ToString("X8");
			}

			// Reading geometry
			var indexFlags = GCIndexAttributeFlags.HasPosition;
			OpaqueMeshes = [];
			
			if (opaqueCount != 0)
			{
				if (labels.TryGetValue(opaqueAddress, out var oMeshName))
				{
					OpaqueMeshName = oMeshName;
				}
				else
				{
					OpaqueMeshName = $"opoly_{opaqueAddress:X8}";
				}
			}
			
			for (var i = 0; i < opaqueCount; i++)
			{
				var mesh = new GCMesh(file, opaqueAddress, imageBase, labels, indexFlags);

				var t = mesh.IndexFlags;
				if (t.HasValue)
				{
					indexFlags = t.Value;
				}

				OpaqueMeshes.Add(mesh);
				opaqueAddress += 16;
			}
			
			TranslucentMeshes = [];
			
			if (translucentCount != 0)
			{
				if (labels.TryGetValue(translucentAddress, out var tMeshName))
				{
					TranslucentMeshName = tMeshName;
				}
				else
				{
					TranslucentMeshName = $"tpoly_{translucentAddress:X8}";
				}
			}
			
			for (var i = 0; i < translucentCount; i++)
			{
				var mesh = new GCMesh(file, translucentAddress, imageBase, labels, indexFlags);

				var t = mesh.IndexFlags;
				if (t.HasValue)
				{
					indexFlags = t.Value;
				}

				TranslucentMeshes.Add(mesh);
				translucentAddress += 16;
			}
		}

		/// <summary>
		/// Writes the attaches contents into a byte array
		/// </summary>
		/// <param name="imageBase">The files imagebase</param>
		/// <param name="dx">Unused</param>
		/// <param name="labels">The files labels</param>
		/// <param name="njOffsets"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public override byte[] GetBytes(uint imageBase, bool dx, Dictionary<string, uint> labels, List<uint> njOffsets, out uint address)
		{
			var result = new List<byte>();
			uint vertexAddress = 0;
			
			if (VertexData != null && VertexData?.Count > 0)
			{
				if (labels.TryGetValue(VertexName, out var vertAddr))
				{
					vertexAddress = vertAddr;
				}
				else
				{
					var vDataAddrs = new uint[VertexData.Count];
					for (var i = 0; i < VertexData.Count; i++)
					{
						{
							if (labels.TryGetValue(VertexData[i].DataName, out var dataAddr))
							{
								vDataAddrs[i] = dataAddr;
							}
							else
							{
								result.Align(4);
								vDataAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(VertexData[i].DataName, vDataAddrs[i]);
								
								foreach (var data in VertexData[i].Data)
								{
									result.AddRange(data.GetBytes());
								}
							}
						}
					}
					
					vertexAddress = (uint)result.Count + imageBase;
					labels.Add(VertexName, vertexAddress);
					
					for (var i = 0; i < VertexData.Count; i++)
					{
						// POF0
						if (vDataAddrs[i] != 0)
						{
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));
						}

						result.AddRange(VertexData[i].GetBytes(vDataAddrs[i]));
					}
					result.Add(255);
					result.AddRange(new byte[15]);
				}
			}
			uint vertexSkinAddress = 0;
			if (vertexSkinData?.Count > 0)
			{
				if (labels.TryGetValue(VertexSkinName, out var vertSkinAddr))
					vertexSkinAddress = vertSkinAddr;
				else
				{
					var vdataAddrs = new uint[vertexSkinData.Count];
					var wdataAddrs = new uint[vertexSkinData.Count];
					for (int i = 0; i < vertexSkinData.Count; i++)
					{
						if (vertexSkinData[i].elementType < GCSkinAttribute.WeightStructEndMarker)
						{
							if (labels.ContainsKey(vertexSkinData[i].DataNamePos))
								vdataAddrs[i] = labels[vertexSkinData[i].DataNamePos];
							else
							{
								result.Align(4);
								vdataAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(vertexSkinData[i].DataNamePos, vdataAddrs[i]);
								for (int j = 0; j < vertexSkinData[i].posNrms.Count; j++)
								{
									result.AddRange((vertexSkinData[i].posNrms[j].pos.GetBytes()));
									result.AddRange((vertexSkinData[i].posNrms[j].nrm.GetBytes()));
								}
							}
							if (vertexSkinData[i].elementType == GCSkinAttribute.PartialWeight || vertexSkinData[i].elementType == GCSkinAttribute.PartialWeightStart)
							{
								if (labels.ContainsKey(vertexSkinData[i].DataNameWeight))
									vdataAddrs[i] = labels[vertexSkinData[i].DataNameWeight];
								else
								{
									result.Align(4);
									wdataAddrs[i] = (uint)result.Count + imageBase;
									labels.Add(vertexSkinData[i].DataNameWeight, wdataAddrs[i]);
									for (int j = 0; j < vertexSkinData[i].weightData.Count; j++)
									{
										result.AddRange(ByteConverter.GetBytes(vertexSkinData[i].weightData[j].vertIndex));
										result.AddRange(ByteConverter.GetBytes(vertexSkinData[i].weightData[j].weight));
									}
								}
							}
						}
					}

					vertexSkinAddress = (uint)result.Count + imageBase;
					labels.Add(VertexSkinName, vertexSkinAddress);
					for (int i = 0; i < vertexSkinData.Count; i++)
					{
						//POF0
						if (vdataAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));
						if (wdataAddrs[i] != 0)
							njOffsets.Add((uint)(result.Count + imageBase + 0xC));

						result.AddRange(vertexSkinData[i].GetBytes());
					}
				}
			}

			uint oPolyAddress = 0;
			// Writing geometry data
			var indexFlags = GCIndexAttributeFlags.HasPosition;
			
			if (OpaqueMeshes != null && OpaqueMeshes.Count > 0)
			{
				if (labels.TryGetValue(OpaqueMeshName, out var polyAddr))
				{
					oPolyAddress = polyAddr;
				}
				else
				{
					var paramAddrs = new uint[OpaqueMeshes.Count];
					var primAddrs = new uint[OpaqueMeshes.Count];
					
					for (var i = 0; i < OpaqueMeshes.Count; i++)
					{
						if (OpaqueMeshes[i].Parameters != null && OpaqueMeshes[i].Parameters.Count > 0)
						{
							if (labels.TryGetValue(OpaqueMeshes[i].ParameterName, out var paramAddr))
							{
								paramAddrs[i] = paramAddr;
							}
							else
							{
								result.Align(4);
								paramAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(OpaqueMeshes[i].ParameterName, paramAddrs[i]);
								
								foreach (var parameter in OpaqueMeshes[i].Parameters)
								{
									result.AddRange(parameter.GetBytes());
								}
							}
						}
					}
					
					for (var i = 0; i < OpaqueMeshes.Count; i++)
					{
						if (OpaqueMeshes[i].Primitives != null && OpaqueMeshes[i].Primitives.Count > 0)
						{
							if (labels.TryGetValue(OpaqueMeshes[i].PrimitiveName, out var primAddr))
							{
								primAddrs[i] = primAddr;
							}
							else
							{
								result.Align(32);
								primAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(OpaqueMeshes[i].PrimitiveName, primAddrs[i]);
								
								var t = OpaqueMeshes[i].IndexFlags;
								if (t.HasValue)
								{
									indexFlags = t.Value;
								}

								foreach (var primitive in OpaqueMeshes[i].Primitives)
								{
									result.AddRange(primitive.GetBytes(indexFlags));
								}
							}
						}
					}
					
					result.Align(32);
					oPolyAddress = (uint)result.Count + imageBase;
					labels.Add(OpaqueMeshName, oPolyAddress);
					
					for (var i = 0; i < OpaqueMeshes.Count; i++)
					{
						// POF0
						if (paramAddrs[i] != 0)
						{
							njOffsets.Add((uint)(result.Count + imageBase));
						}

						if (primAddrs[i] != 0)
						{
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));
						}

						result.AddRange(OpaqueMeshes[i].GetBytes(paramAddrs[i], primAddrs[i]));
					}
				}
			}
			
			result.Align(4);
			uint tPolyAddress = 0;
			
			if (TranslucentMeshes != null && TranslucentMeshes.Count > 0)
			{
				if (labels.TryGetValue(TranslucentMeshName, out var polyAddr))
				{
					tPolyAddress = polyAddr;
				}
				else
				{
					var paramAddrs = new uint[TranslucentMeshes.Count];
					var primAddrs = new uint[TranslucentMeshes.Count];
					
					for (var i = 0; i < TranslucentMeshes.Count; i++)
					{
						if (TranslucentMeshes[i].Parameters != null && TranslucentMeshes[i].Parameters.Count > 0)
						{
							if (labels.TryGetValue(TranslucentMeshes[i].ParameterName, out var paramAddr))
							{
								paramAddrs[i] = paramAddr;
							}
							else
							{
								result.Align(4);
								paramAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(TranslucentMeshes[i].ParameterName, paramAddrs[i]);
								
								foreach (var parameter in TranslucentMeshes[i].Parameters)
								{
									result.AddRange(parameter.GetBytes());
								}
							}
						}
					}
					
					for (var i = 0; i < TranslucentMeshes.Count; i++)
					{
						if (TranslucentMeshes[i].Primitives != null && TranslucentMeshes[i].Primitives.Count > 0)
						{
							if (labels.TryGetValue(TranslucentMeshes[i].PrimitiveName, out var primAddr))
							{
								primAddrs[i] = primAddr;
							}
							else
							{
								result.Align(32);
								primAddrs[i] = (uint)result.Count + imageBase;
								labels.Add(TranslucentMeshes[i].PrimitiveName, primAddrs[i]);
								
								var t = TranslucentMeshes[i].IndexFlags;
								if (t.HasValue)
								{
									indexFlags = t.Value;
								}

								foreach (var primitive in TranslucentMeshes[i].Primitives)
								{
									result.AddRange(primitive.GetBytes(indexFlags));
								}
							}
						}
					}
					
					result.Align(32);
					tPolyAddress = (uint)result.Count + imageBase;
					labels.Add(TranslucentMeshName, tPolyAddress);
					
					for (var i = 0; i < TranslucentMeshes.Count; i++)
					{
						// POF0
						if (paramAddrs[i] != 0)
						{
							njOffsets.Add((uint)(result.Count + imageBase));
						}

						if (primAddrs[i] != 0)
						{
							njOffsets.Add((uint)(result.Count + imageBase + 0x8));
						}

						result.AddRange(TranslucentMeshes[i].GetBytes(paramAddrs[i], primAddrs[i]));
					}
				}
			}

			result.Align(4);
			address = (uint)result.Count;

			// POF0
			if (vertexAddress != 0)
			{
				njOffsets.Add((uint)(result.Count + imageBase));
			}
			
			if (vertexSkinAddress != 0)
			{
				njOffsets.Add((uint)(result.Count + imageBase + 8));
			}

			if (oPolyAddress != 0)
			{
				njOffsets.Add((uint)(result.Count + imageBase + 0x8));
			}

			if (tPolyAddress != 0)
			{
				njOffsets.Add((uint)(result.Count + imageBase + 0xC));
			}

			result.AddRange(ByteConverter.GetBytes(vertexAddress));
			result.AddRange(ByteConverter.GetBytes(vertexSkinAddress));
			result.AddRange(ByteConverter.GetBytes(oPolyAddress));
			result.AddRange(ByteConverter.GetBytes(tPolyAddress));
			result.AddRange(ByteConverter.GetBytes((ushort)OpaqueMeshes.Count));
			result.AddRange(ByteConverter.GetBytes((ushort)TranslucentMeshes.Count));
			result.AddRange(Bounds.GetBytes());
			labels.Add(Name, address + imageBase);
			
			return result.ToArray();
		}

		/// <summary>
		/// Processes the vertex data to be rendered
		/// </summary>
		public override void ProcessVertexData()
		{
			var positions = VertexData.Find(x => x.Attribute == GCVertexAttribute.Position)?.Data;
			var normals = VertexData.Find(x => x.Attribute == GCVertexAttribute.Normal)?.Data;
			var colors = VertexData.Find(x => x.Attribute == GCVertexAttribute.Color0)?.Data;
			var uvs = VertexData.Find(x => x.Attribute == GCVertexAttribute.Tex0)?.Data;
			GCUVScale ouvscale = GCUVScale.Default;
			List<VtxAttrFmtParameter> ovtxparams = new List<VtxAttrFmtParameter>();
			List<VtxAttrFmtParameter> ovtxparamsCheck = new List<VtxAttrFmtParameter>();
			List<VtxAttrFmtParameter> tvtxparams = new List<VtxAttrFmtParameter>();
			if (OpaqueMeshes.Count > 0)
			{
				var oparams = OpaqueMeshes[0].Parameters;
				for (int i = 0; i < oparams.Count; i++)
				{
					if (oparams[i].Type == ParameterType.VtxAttrFmt)
					{
						ovtxparams.Add((VtxAttrFmtParameter)oparams[i]);
					}
				}
				for (int i = 0; i < ovtxparams.Count; i++)
				{
					if (ovtxparams[i].VertexAttribute == GCVertexAttribute.Tex0)
					{
						ouvscale = ovtxparams[i].UVScale;
						ovtxparamsCheck.Add(ovtxparams[i]);
					}
				}
				if (OpaqueMeshes.Count > 1)
				{
					//For Cannon's Core (Rouge)
					var oparams2 = OpaqueMeshes[1].Parameters;
					if (ovtxparamsCheck.Count < 1)
					{
						for (int i = 0; i < oparams2.Count; i++)
						{
							if (oparams2[i].Type == ParameterType.VtxAttrFmt)
							{
								ovtxparams.Add((VtxAttrFmtParameter)oparams2[i]);
							}
						}
						for (int i = 0; i < ovtxparams.Count; i++)
						{
							if (ovtxparams[i].VertexAttribute == GCVertexAttribute.Tex0)
							{
								ouvscale = ovtxparams[i].UVScale;
								ovtxparamsCheck.Add(ovtxparams[i]);
							}
						}
					}
				}
			}
			GCUVScale tuvscale = GCUVScale.Default;
			if (TranslucentMeshes.Count > 0)
			{
				var tparams = TranslucentMeshes[0].Parameters;
				for (int i = 0; i < tparams.Count; i++)
				{
					if (tparams[i].Type == ParameterType.VtxAttrFmt)
					{
						tvtxparams.Add((VtxAttrFmtParameter)tparams[i]);
					}
				}
				for (int i = 0; i < tvtxparams.Count; i++)
				{
					if (tvtxparams[i].VertexAttribute == GCVertexAttribute.Tex0)
					{
						tuvscale = tvtxparams[i].UVScale;
					}
				}
			}

			var mat = new NJS_MATERIAL();

			mat.UseAlpha = false;
			var meshInfo = OpaqueMeshes.Select(m => m.Process(mat, positions, normals, colors, uvs, ouvscale)).ToList();

			mat.UseAlpha = true;
			meshInfo.AddRange(TranslucentMeshes.Select(m => m.Process(mat, positions, normals, colors, uvs, tuvscale)));

			MeshInfo = meshInfo.ToArray();
		}

		/// <summary>
		/// Creates a C Struct string identical to the data given (WIP)
		/// </summary>
		/// <param name="dx">Unused</param>
		/// <returns></returns>
		public override string ToStruct(bool dx)
		{
			var result = new StringBuilder("{ ");
			result.Append(VertexData != null ? VertexName : "NULL");
			result.Append(", ");
			result.Append(vertexSkinData != null ? VertexSkinName : "NULL");
			result.Append(", ");
			result.Append(OpaqueMeshes.Count != 0 ? OpaqueMeshName : "NULL");
			result.Append(", ");
			result.Append(TranslucentMeshes.Count != 0 ? TranslucentMeshName : "NULL");
			result.Append(", ");
			result.Append(OpaqueMeshes.Count != 0 ? $"LengthOfArray<Uint16>({OpaqueMeshName})" : "0");
			result.Append(", ");
			result.Append(TranslucentMeshes.Count != 0 ? $"LengthOfArray<Uint16>({TranslucentMeshName})" : "0");
			result.Append(", ");
			result.Append(Bounds.ToStruct());
			result.Append(" }");
			return result.ToString();
		}

		/// <summary>
		/// WIP
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="dx">Unused</param>
		/// <param name="labels"></param>
		/// <param name="textures"></param>
		public override void ToStructVariables(TextWriter writer, bool dx, List<string> labels, string[] textures = null)
		{
			if (VertexData.Count != 0 && !labels.Contains(VertexName))
			{
				foreach (var data in VertexData)
				{
					if (!labels.Contains(data.DataName))
					{
						labels.Add(data.DataName);
						
						switch (data.Attribute)
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
						
						writer.Write(data.DataName);
						writer.WriteLine("[] = {");
						var vtx = new List<string>(data.Data.Count);
						
						switch (data.Attribute)
						{
							case GCVertexAttribute.Position:
							case GCVertexAttribute.Normal:
								foreach (Vector3 item in data.Data)
								{
									vtx.Add(item.ToStruct());
								}
								break;
							case GCVertexAttribute.Color0:
								foreach (Color item in data.Data)
								{
									vtx.Add(item.ToStruct());
								}
								break;
							case GCVertexAttribute.Tex0:
								foreach (UV item in data.Data)
								{
									vtx.Add(item.ToStruct());
								}
								break;
						}
						
						writer.WriteLine("\t" + string.Join($",{Environment.NewLine}\t", vtx.ToArray()));
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				
				labels.Add(VertexName);
				writer.Write("SA2B_VertexData ");
				writer.Write(VertexName);
				writer.Write("[] = {");
				
				var chunks = new List<string>(VertexData.Count);
				chunks.AddRange(VertexData.Select(item => item.ToStruct()));
				
				writer.WriteLine($"\t{string.Join($",{Environment.NewLine}\t", chunks.ToArray())},");
				writer.WriteLine($"\t{string.Join($"{Environment.NewLine}\t", "{ 255, 0, 0, 0, NULL, 0 }")}");
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			if (vertexSkinData.Count != 0 && !labels.Contains(VertexSkinName))
			{
				foreach (var data in vertexSkinData)
				{
					if (!labels.Contains(data.DataNamePos))
					{
						labels.Add(data.DataNamePos);
						switch (data.elementType)
						{
							case GCSkinAttribute.StaticWeight:
								writer.Write("SA2B_WeightData ");
								break;
							case GCSkinAttribute.PartialWeightStart:
							case GCSkinAttribute.PartialWeight:
								writer.Write("SA2B_WeightData ");
								break;
						}
						writer.Write(data.DataNamePos);
						writer.WriteLine("[] = {");
						switch (data.elementType)
						{
							case GCSkinAttribute.StaticWeight:
								foreach (GCSkinVertexSetPosNrm item in data.posNrms)
									writer.WriteLine("\t" + string.Join($",{Environment.NewLine}\t", item.ToStruct()));
								break;
							case GCSkinAttribute.PartialWeightStart:
							case GCSkinAttribute.PartialWeight:
								foreach (GCSkinVertexSetPosNrm item in data.posNrms)
									writer.WriteLine("\t" + string.Join($",{Environment.NewLine}\t", item.ToStruct()));
								foreach (GCSkinVertexSetWeight weight in data.weightData)
									writer.WriteLine("\t" + string.Join($",{Environment.NewLine}\t", weight.ToStruct()));
								break;
							case GCSkinAttribute.WeightStructEndMarker:
								break;
						}
					}
					writer.WriteLine("};");
					writer.WriteLine();
				}
				labels.Add(VertexSkinName);
				writer.Write("SA2B_VertexSkinData ");
				writer.Write(VertexSkinName);
				writer.Write("[] = {");
				var chunks = new List<string>(VertexData.Count);
				chunks.AddRange(vertexSkinData.Select(item => item.ToStruct()));
				writer.WriteLine($"\t{string.Join($",{Environment.NewLine}\t", chunks.ToArray())},");
				writer.WriteLine(" };");
				writer.WriteLine();
			}
				var indexFlags = GCIndexAttributeFlags.HasPosition;
			if (OpaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
			{
				foreach (var mesh in OpaqueMeshes)
				{
					foreach (var parameter in mesh.Parameters)
					{
						if (parameter == null || labels.Contains(mesh.ParameterName))
						{
							continue;
						}

						labels.Add(mesh.ParameterName);
						writer.Write("SA2B_ParameterData ");
						writer.Write(mesh.ParameterName);
						writer.WriteLine("[] = {");
							
						var param = new List<string>(mesh.Parameters.Count);
						param.AddRange(mesh.Parameters.Select(item => item.ToStruct()));
							
						writer.WriteLine($"\t{string.Join($",{Environment.NewLine}\t", param.ToArray())}");
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				
				foreach (var mesh in OpaqueMeshes)
				{
					if (labels.Contains(mesh.PrimitiveName))
					{
						continue;
					}

					labels.Add(mesh.PrimitiveName);
					writer.Write("Uint8 ");
					writer.Write(mesh.PrimitiveName);
					writer.WriteLine("[] = {");
					
					var pBytes = new List<byte>();
					
					foreach (var item in mesh.Primitives)
					{
						var t = mesh.IndexFlags;
						if (t.HasValue)
						{
							indexFlags = t.Value;
						}

						foreach (var primitive in mesh.Primitives)
						{
							pBytes.AddRange(primitive.GetBytes(indexFlags));
						}
					}
					
					var cb = pBytes.ToArray();
					var dataSize = Convert.ToInt32(Math.Ceiling(decimal.Divide(cb.Length, 32)) * 32);
					var buffSize = dataSize - cb.Length;
					var s = new List<string>(dataSize);
						
					s.AddRange(cb.Select(b => $"0x{b:X}"));

					for (var l = 0; l < buffSize; l++)
					{
						s.Add("0x0");
					}

					writer.Write(string.Join(", ", s.ToArray()));
					//
					//List<string> prim = new List<string>(opaqueMeshes[i].primitives.Count);
					//foreach (GCPrimitive item in opaqueMeshes[i].primitives)
					//prim.Add(item.ToStruct());
					//writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", prim.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				
				labels.Add(OpaqueMeshName);
				writer.Write("SA2B_GeometryData ");
				writer.Write(OpaqueMeshName);
				writer.Write("[] = {");
				writer.WriteLine($"\t{string.Join($",{Environment.NewLine}\t", OpaqueMeshes.Select(item => item.ToStruct()).ToArray())}");
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			
			if (TranslucentMeshes.Count != 0 && !labels.Contains(TranslucentMeshName))
			{
				foreach (var mesh in TranslucentMeshes)
				{
					foreach (var parameter in mesh.Parameters)
					{
						if (parameter == null || labels.Contains(mesh.ParameterName))
						{
							continue;
						}

						labels.Add(mesh.ParameterName);
						writer.Write("SA2B_ParameterData ");
						writer.Write(mesh.ParameterName);
						writer.WriteLine("[] = {");
							
						var param = new List<string>(mesh.Parameters.Count);
						param.AddRange(mesh.Parameters.Select(item => item.ToStruct()));
							
						writer.WriteLine($"\t{string.Join($",{Environment.NewLine}\t", param.ToArray())}");
						writer.WriteLine("};");
						writer.WriteLine();
					}
				}
				
				foreach (var mesh in TranslucentMeshes)
				{
					if (labels.Contains(mesh.PrimitiveName))
					{
						continue;
					}

					labels.Add(mesh.PrimitiveName);
					writer.Write("Uint8 ");
					writer.Write(mesh.PrimitiveName);
					writer.WriteLine("[] = {");
					
					var pbytes = new List<byte>();
					foreach (var item in mesh.Primitives)
					{
						var t = mesh.IndexFlags;
						if (t.HasValue)
						{
							indexFlags = t.Value;
						}

						foreach (var primitive in mesh.Primitives)
						{
							pbytes.AddRange(primitive.GetBytes(indexFlags));
						}
					}
					
					var cb = pbytes.ToArray();
					var dataSize = Convert.ToInt32(Math.Ceiling(decimal.Divide(cb.Length, 32)) * 32);
					var buffSize = dataSize - cb.Length;
					var s = new List<string>(dataSize);
					
					s.AddRange(cb.Select(b => $"0x{b:X}"));

					for (var l = 0; l < buffSize; l++)
					{
						s.Add("0x0");
					}

					writer.Write(string.Join(", ", s.ToArray()));
					//writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", s.ToArray()));
					writer.WriteLine("};");
					writer.WriteLine();
				}
				
				labels.Add(TranslucentMeshName);
				writer.Write("SA2B_GeometryData ");
				writer.Write(TranslucentMeshName);
				writer.Write("[] = {");

				writer.WriteLine($"\t{string.Join($",{Environment.NewLine}\t", TranslucentMeshes.Select(item => item.ToStruct()).ToArray())}");
				writer.WriteLine(" };");
				writer.WriteLine();
			}
			
			writer.Write("SA2B_Model ");
			writer.Write(Name);
			writer.Write(" = ");
			writer.Write(ToStruct(dx));
			writer.WriteLine(";");
		}

		// WIP
		public void ToNJA(TextWriter writer, List<string> labels, string[] textures)
		{
			if (VertexData.Count != 0 && !labels.Contains(VertexName))
			{
				foreach (var item in VertexData)
				{
					item.ToNJA(writer);
				}

				writer.WriteLine($"GJARRAY      {VertexName}[]");
				writer.WriteLine($"START{Environment.NewLine}");

				foreach (var item in VertexData)
				{
					item.RefToNJA(writer);
				}

				writer.WriteLine("ATTRSTART");
				writer.WriteLine($"GjId         GJ_VA_NULL,");
				writer.WriteLine($"ATTREND{Environment.NewLine}");

				writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
			}

			if (vertexSkinData.Count != 0 && !labels.Contains(VertexSkinName))
			{
				writer.WriteLine($"GJWEIGHTARRAY {VertexSkinName}[]");
				writer.WriteLine($"START{Environment.NewLine}");

				foreach (var item in vertexSkinData)
				{
					item.ToNJA(writer);
				}
				foreach (var item in vertexSkinData)
				{
					item.RefToNJA(writer);
				}

				writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
			}

			var indexFlags = GCIndexAttributeFlags.HasPosition;

			if (OpaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
			{
				foreach (var item in OpaqueMeshes)
				{
					item.ToNJA(writer, ref indexFlags);
				}

				writer.WriteLine($"GJMESHSET     {OpaqueMeshName}[]");
				writer.WriteLine($"START{Environment.NewLine}");

				foreach (var item in OpaqueMeshes)
				{
					item.RefToNJA(writer);
				}

				writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
			}

			if (TranslucentMeshes.Count != 0 && !labels.Contains(TranslucentMeshName))
			{
				foreach (var item in TranslucentMeshes)
				{
					item.ToNJA(writer, ref indexFlags);
				}

				writer.WriteLine($"GJMESHSET    {TranslucentMeshName}[]");
				writer.WriteLine($"START{Environment.NewLine}");
				
				foreach (var item in TranslucentMeshes)
				{
					item.RefToNJA(writer);
				}

				writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
			}

			writer.WriteLine($"GJMODEL     {Name}[]");
			writer.WriteLine("START");
			if (VertexData.Count != 0 && !labels.Contains(VertexName))
			    writer.WriteLine($"GjArray     {VertexName},");
			else
				writer.WriteLine($"GjArray     NULL,");
			if (vertexSkinData.Count != 0 && !labels.Contains(VertexSkinName))
			    writer.WriteLine($"GjVlist     {VertexSkinName},");
			else
				writer.WriteLine("GjVlist     NULL,");

			if (OpaqueMeshes.Count != 0 && !labels.Contains(OpaqueMeshName))
			{
				writer.WriteLine($"GjOMesh     {OpaqueMeshName},");
			}
			else
			{
				writer.WriteLine("GjOMesh     NULL,");
			}

			if (TranslucentMeshes.Count != 0 && !labels.Contains(TranslucentMeshName))
			{
				writer.WriteLine($"GjAMesh     {TranslucentMeshName},");
			}
			else
			{
				writer.WriteLine("GjAMesh     NULL,");
			}

			writer.WriteLine($"GjNbOMesh   {OpaqueMeshes.Count},");
			writer.WriteLine($"GjNbAMesh   {TranslucentMeshes.Count},");
			writer.WriteLine($"Center     {Bounds.Center.X.ToNJA()}, {Bounds.Center.Y.ToNJA()}, {Bounds.Center.Z.ToNJA()},");
			writer.WriteLine($"Radius     {Bounds.Radius.ToNJA()},");
			writer.Write($"END{Environment.NewLine}{Environment.NewLine}");
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
			var result = (GCAttach)MemberwiseClone();
			
			if (VertexData != null)
			{
				result.VertexData = new List<GCVertexSet>(VertexData.Count);
				
				foreach (var item in VertexData)
				{
					result.VertexData.Add(item.Clone());
				}
			}
			
			if (OpaqueMeshes != null)
			{
				result.OpaqueMeshes = new List<GCMesh>(OpaqueMeshes.Count);
				
				foreach (var item in OpaqueMeshes)
				{
					result.OpaqueMeshes.Add(item.Clone());
				}
			}

			if (TranslucentMeshes != null)
			{
				result.TranslucentMeshes = new List<GCMesh>(TranslucentMeshes.Count);
				
				foreach (var item in TranslucentMeshes)
				{
					result.TranslucentMeshes.Add(item.Clone());
				}
			}
			
			result.Bounds = Bounds.Clone();
			return result;
		}

		#endregion
	}
}
