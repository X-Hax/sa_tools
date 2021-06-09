using SharpDX;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Color = System.Drawing.Color;

namespace SonicRetro.SAModel.Direct3D
{
	// Legacy code for Import/export
	public static partial class Extensions
	{
		/// <summary>
		/// Old OBJ import code.
		/// </summary>
		public static Attach obj2nj(string objfile, string[] textures = null)
		{
			string[] objFile = File.ReadAllLines(objfile);
			List<UV> uvs = new List<UV>();
			List<Color> vcolors = new List<Color>();
			List<Vertex> verts = new List<Vertex>();
			List<Vertex> norms = new List<Vertex>();
			Dictionary<string, NJS_MATERIAL> materials = new Dictionary<string, NJS_MATERIAL>();

			NJS_MATERIAL lastMaterial = null;
			// Determines whether or not the Texture ID for lastMaterial has been set.
			bool textureIdAssigned = false;
			// Used for materials that don't have a texid field.
			int lastTextureId = 0;

			List<Vertex> model_Vertex = new List<Vertex>();
			List<Vertex> model_Normal = new List<Vertex>();
			List<NJS_MATERIAL> model_Material = new List<NJS_MATERIAL>();
			List<NJS_MESHSET> model_Mesh = new List<NJS_MESHSET>();
			List<ushort> model_Mesh_MaterialID = new List<ushort>();
			List<List<Poly>> model_Mesh_Poly = new List<List<Poly>>();
			List<List<UV>> model_Mesh_UV = new List<List<UV>>();
			List<List<Color>> model_Mesh_VColor = new List<List<Color>>();

			foreach (string objLine in objFile)
			{
				string[] lin = objLine.Split('#')[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

				if (lin.Length == 0)
					continue;

				switch (lin[0].ToLowerInvariant())
				{
					case "mtllib":
						string[] mtlFile = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(objfile), lin[1]));
						foreach (string mtlLine in mtlFile)
						{
							string[] mlin = mtlLine.Split('#')[0].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

							if (mlin.Length == 0)
								continue;

							#region Parsing Material Properties
							// Calling trim on this to be compatible with 3ds Max mtl files.
							// It likes to indent them with tabs (\t)
							switch (mlin[0].ToLowerInvariant().Trim())
							{
								case "newmtl":
									// Texture ID failsafe
									if (!textureIdAssigned && lastMaterial != null)
										lastMaterial.TextureID = ++lastTextureId;

									textureIdAssigned = false;
									lastMaterial = new NJS_MATERIAL { UseAlpha = false, UseTexture = false };
									materials.Add(mlin[1], lastMaterial);
									break;

								case "kd":
									lastMaterial.DiffuseColor = Color.FromArgb(
										(int)Math.Round(float.Parse(mlin[1], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[2], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[3], CultureInfo.InvariantCulture) * 255));
									break;

								case "map_ka":
									lastMaterial.UseAlpha = true;
									if (textures != null && mlin.Length > 1)
									{
										string baseName = Path.GetFileNameWithoutExtension(mlin[1]);
										for (int tid = 0; tid < textures.Length; tid++)
										{
											if (textures[tid] == baseName)
											{
												lastMaterial.TextureID = tid;
												lastTextureId = tid;
												textureIdAssigned = true;
												break;
											}
										}
									}
									break;

								case "map_kd":
									lastMaterial.UseTexture = true;
									if (textures != null && mlin.Length > 1)
									{
										string baseName = Path.GetFileNameWithoutExtension(mlin[1]);
										for (int tid = 0; tid < textures.Length; tid++)
										{
											if (textures[tid] == baseName)
											{
												lastMaterial.TextureID = tid;
												lastTextureId = tid;
												textureIdAssigned = true;
												break;
											}
										}
									}
									break;

								case "ke":
									lastMaterial.Exponent = float.Parse(mlin[1], CultureInfo.InvariantCulture);
									break;

								case "d":
								case "tr":
									lastMaterial.DiffuseColor = Color.FromArgb(
										(int)Math.Round(float.Parse(mlin[1], CultureInfo.InvariantCulture) * 255),
										lastMaterial.DiffuseColor);
									break;

								case "ks":
									lastMaterial.SpecularColor = Color.FromArgb(
										(int)Math.Round(float.Parse(mlin[1], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[2], CultureInfo.InvariantCulture) * 255),
										(int)Math.Round(float.Parse(mlin[3], CultureInfo.InvariantCulture) * 255));
									break;

								case "texid":
									if (!textureIdAssigned)
									{
										int textureID = int.Parse(mlin[1], CultureInfo.InvariantCulture);
										lastMaterial.TextureID = textureID;
										lastTextureId = textureID;
										textureIdAssigned = true;
									}
									break;

								case "-u_mirror":
									if (bool.TryParse(mlin[1], out bool uMirror))
										lastMaterial.FlipU = uMirror;

									break;

								case "-v_mirror":
									if (bool.TryParse(mlin[1], out bool vMirror))
										lastMaterial.FlipV = vMirror;

									break;

								case "-u_tile":
									if (bool.TryParse(mlin[1], out bool uTile))
										lastMaterial.ClampU = !uTile;

									break;

								case "-v_tile":
									if (bool.TryParse(mlin[1], out bool vTile))
										lastMaterial.ClampV = !vTile;

									break;

								case "-enviromap":
									lastMaterial.EnvironmentMap = true;
									break;

								case "-doublesided":
									lastMaterial.DoubleSided = true;
									break;

								case "-ignorelighting":
									lastMaterial.IgnoreLighting = bool.Parse(mlin[1]);
									break;

								case "-flatshaded":
									lastMaterial.FlatShading = bool.Parse(mlin[1]);
									break;
							}
							#endregion
						}

						break;

					case "v":
						verts.Add(new Vertex(float.Parse(lin[1], CultureInfo.InvariantCulture),
							float.Parse(lin[2], CultureInfo.InvariantCulture),
							float.Parse(lin[3], CultureInfo.InvariantCulture)));
						break;

					case "vn":
						norms.Add(new Vertex(float.Parse(lin[1], CultureInfo.InvariantCulture),
							float.Parse(lin[2], CultureInfo.InvariantCulture),
							float.Parse(lin[3], CultureInfo.InvariantCulture)));
						break;

					case "vt":
						uvs.Add(new UV
						{
							U = float.Parse(lin[1], CultureInfo.InvariantCulture) * -1,
							V = float.Parse(lin[2], CultureInfo.InvariantCulture) * -1
						});
						break;

					case "vc":
						vcolors.Add(Color.FromArgb((int)Math.Round(float.Parse(lin[1], CultureInfo.InvariantCulture)),
							(int)Math.Round(float.Parse(lin[2], CultureInfo.InvariantCulture)),
							(int)Math.Round(float.Parse(lin[3], CultureInfo.InvariantCulture)),
							(int)Math.Round(float.Parse(lin[4], CultureInfo.InvariantCulture))));
						break;

					case "usemtl":
						model_Mesh_Poly.Add(new List<Poly>());
						model_Mesh_UV.Add(new List<UV>());
						model_Mesh_VColor.Add(new List<Color>());
						if (materials.ContainsKey(lin[1]))
						{
							NJS_MATERIAL mtl = materials[lin[1]];
							if (model_Material.Contains(mtl))
								model_Mesh_MaterialID.Add((ushort)model_Material.IndexOf(mtl));
							else
							{
								model_Mesh_MaterialID.Add((ushort)model_Material.Count);
								model_Material.Add(mtl);
							}
						}
						else
							model_Mesh_MaterialID.Add(0);
						break;

					case "f":
						if (model_Mesh_MaterialID.Count == 0)
						{
							model_Mesh_MaterialID.Add(0);
							model_Mesh_Poly.Add(new List<Poly>());
							model_Mesh_UV.Add(new List<UV>());
							model_Mesh_VColor.Add(new List<Color>());
						}
						ushort[] pol = new ushort[3];
						for (int i = 1; i <= 3; i++)
						{
							string[] lne = lin[i].Split('/');
							Vertex ver = verts.GetItemNeg(int.Parse(lne[0]));
							Vertex nor = norms.GetItemNeg(int.Parse(lne[2]));
							if (uvs.Count > 0)
							{
								if (!string.IsNullOrEmpty(lne[1]))
								{
									model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lne[1])));
								}
							}
							if (vcolors.Count > 0)
							{
								if (lne.Length == 4)
								{
									model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lne[3])));
								}
								else if (!string.IsNullOrEmpty(lne[1]))
								{
									model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lne[1])));
								}
							}
							int verind = model_Vertex.IndexOf(ver);
							while (verind > -1)
							{
								if (Equals(model_Normal[verind], nor))
									break;
								verind = model_Vertex.IndexOf(ver, verind + 1);
							}
							if (verind > -1)
							{
								pol[i - 1] = (ushort)verind;
							}
							else
							{
								model_Vertex.Add(ver);
								model_Normal.Add(nor);
								pol[i - 1] = (ushort)(model_Vertex.Count - 1);
							}
						}
						Poly tri = Poly.CreatePoly(Basic_PolyType.Triangles);
						for (int i = 0; i < 3; i++)
							tri.Indexes[i] = pol[i];
						model_Mesh_Poly[model_Mesh_Poly.Count - 1].Add(tri);
						break;

					case "t":
						if (model_Mesh_MaterialID.Count == 0)
						{
							model_Mesh_MaterialID.Add(0);
							model_Mesh_Poly.Add(new List<Poly>());
							model_Mesh_UV.Add(new List<UV>());
							model_Mesh_VColor.Add(new List<Color>());
						}
						List<ushort> str = new List<ushort>();
						for (int i = 1; i <= lin.Length - 1; i++)
						{
							Vertex ver2 = verts.GetItemNeg(int.Parse(lin[i]));
							Vertex nor2 = norms.GetItemNeg(int.Parse(lin[i]));
							if (uvs.Count > 0)
								model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lin[i])));
							if (vcolors.Count > 0)
								model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lin[i])));
							int verind = model_Vertex.IndexOf(ver2);
							while (verind > -1)
							{
								if (Equals(model_Normal[verind], nor2))
									break;
								verind = model_Vertex.IndexOf(ver2, verind + 1);
							}
							if (verind > -1)
							{
								str.Add((ushort)verind);
							}
							else
							{
								model_Vertex.Add(ver2);
								model_Normal.Add(nor2);
								str.Add((ushort)(model_Vertex.Count - 1));
							}
						}
						model_Mesh_Poly[model_Mesh_Poly.Count - 1].Add(new Strip(str.ToArray(), false));
						break;

					case "q":
						List<ushort> str2 = new List<ushort>(model_Mesh_Poly[model_Mesh_Poly.Count - 1][model_Mesh_Poly[model_Mesh_Poly.Count - 1].Count - 1].Indexes);
						for (int i = 1; i <= lin.Length - 1; i++)
						{
							Vertex ver3 = verts.GetItemNeg(int.Parse(lin[i]));
							Vertex nor3 = norms.GetItemNeg(int.Parse(lin[i]));
							if (uvs.Count > 0)
								model_Mesh_UV[model_Mesh_UV.Count - 1].Add(uvs.GetItemNeg(int.Parse(lin[i])));
							if (vcolors.Count > 0)
								model_Mesh_VColor[model_Mesh_VColor.Count - 1].Add(vcolors.GetItemNeg(int.Parse(lin[i])));
							int verind = model_Vertex.IndexOf(ver3);
							while (verind > -1)
							{
								if (Equals(model_Normal[verind], nor3))
									break;
								verind = model_Vertex.IndexOf(ver3, verind + 1);
							}
							if (verind > -1)
							{
								str2.Add((ushort)verind);
							}
							else
							{
								model_Vertex.Add(ver3);
								model_Normal.Add(nor3);
								str2.Add((ushort)(model_Vertex.Count - 1));
							}
						}
						model_Mesh_Poly[model_Mesh_Poly.Count - 1][model_Mesh_Poly[model_Mesh_Poly.Count - 1].Count - 1] = new Strip(str2.ToArray(), false);
						break;
				}
			}

			// Material failsafe
			if (model_Material.Count == 0)
				model_Material.Add(new NJS_MATERIAL());

			for (int i = 0; i < model_Mesh_MaterialID.Count; i++)
			{
				model_Mesh.Add(new NJS_MESHSET(model_Mesh_Poly[i].ToArray(), false, model_Mesh_UV[i].Count > 0, model_Mesh_VColor[i].Count > 0));
				model_Mesh[i].MaterialID = model_Mesh_MaterialID[i];
				if (model_Mesh[i].UV != null)
				{
					// HACK: Checking if j < model_Mesh_UV[i].Count prevents an out-of-range exception with SADXLVL2 when importing a whole stage export of Emerald Coast 1.
					for (int j = 0; j < model_Mesh[i].UV.Length && j < model_Mesh_UV[i].Count; j++)
						model_Mesh[i].UV[j] = model_Mesh_UV[i][j];
				}
				if (model_Mesh[i].VColor != null)
				{
					for (int j = 0; j < model_Mesh[i].VColor.Length; j++)
						model_Mesh[i].VColor[j] = model_Mesh_VColor[i][j];
				}
			}

			Attach model = new BasicAttach(model_Vertex.ToArray(), model_Normal.ToArray(), model_Mesh, model_Material);
			model.ProcessVertexData();
			model.CalculateBounds();
			return model;
		}

		/// <summary>
		/// Writes an object model (basic format) to the specified stream, in Alias-Wavefront *.OBJ format.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">idk</param>
		/// <param name="transform">Used for calculating transforms.</param>
		/// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
		/// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
		/// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
		private static void WriteObjFromBasicAttach(StreamWriter objstream, NJS_OBJECT obj, ref List<NJS_MATERIAL> materials, Matrix transform, ref int totalVerts, ref int totalNorms, ref int totalUVs)
		{
			if (obj.Attach != null)
			{
				var basicAttach = (BasicAttach)obj.Attach;
				bool wroteNormals = false;
				objstream.WriteLine("g " + obj.Name);

				#region Outputting Verts and Normals
				foreach (Vertex v in basicAttach.Vertex)
				{
					Vector3 inputVert = new Vector3(v.X, v.Y, v.Z);
					Vector3 outputVert = Vector3.TransformCoordinate(inputVert, transform);
					objstream.WriteLine("v {0} {1} {2}", outputVert.X.ToString(NumberFormatInfo.InvariantInfo), outputVert.Y.ToString(NumberFormatInfo.InvariantInfo), outputVert.Z.ToString(NumberFormatInfo.InvariantInfo));
				}

				if (basicAttach.Vertex.Length == basicAttach.Normal.Length)
				{
					foreach (Vertex v in basicAttach.Normal)
					{
						objstream.WriteLine("vn {0} {1} {2}", v.X.ToString(NumberFormatInfo.InvariantInfo), v.Y.ToString(NumberFormatInfo.InvariantInfo), v.Z.ToString(NumberFormatInfo.InvariantInfo));
					}
					wroteNormals = true;
				}
				#endregion

				#region Outputting Meshes
				int meshID = 0;
				foreach (NJS_MESHSET set in basicAttach.Mesh)
				{
					if (basicAttach.Material.Count > 0)
					{
						/*if (basicAttach.Material[set.MaterialID].UseTexture)
						{
							objstream.WriteLine("usemtl {0}_material_{1}", materialPrefix, basicAttach.Material[set.MaterialID].TextureID);
						}*/


						int materialIndexInList = 0;

						NJS_MATERIAL material = basicAttach.Material[set.MaterialID];

						if (!materials.Contains(material))
						{
							materials.Add(material);
						}

						materialIndexInList = materials.IndexOf(material);

						objstream.WriteLine("usemtl material_{0}", materialIndexInList);
					}

					if (set.UV != null)
					{
						foreach (UV uv in set.UV)
						{
							objstream.WriteLine("vt {0} {1}", uv.U.ToString(NumberFormatInfo.InvariantInfo), (-uv.V).ToString(NumberFormatInfo.InvariantInfo));
						}
					}

					int processedUVStripCount = 0;
					foreach (Poly poly in set.Poly)
					{
						switch (poly.PolyType)
						{
							case Basic_PolyType.Strips:
								var polyStrip = (Strip)poly;
								int expectedTrisCount = polyStrip.Indexes.Length - 2;
								bool triangleWindReversed = polyStrip.Reversed;

								for (int stripIndx = 0; stripIndx < expectedTrisCount; stripIndx++)
								{
									if (triangleWindReversed)
									{
										Vector3 newFace = new Vector3((polyStrip.Indexes[stripIndx + 1] + 1),
											(polyStrip.Indexes[stripIndx] + 1),
											(polyStrip.Indexes[stripIndx + 2] + 1));

										if (set.UV != null)
										{
											int uv1 = (stripIndx + 1) + processedUVStripCount + 1;
											int uv2 = (stripIndx) + processedUVStripCount + 1;
											int uv3 = (stripIndx + 2) + processedUVStripCount + 1;

											if (wroteNormals)
											{
												objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
													(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
													(int)newFace.X + totalVerts, uv1 + totalUVs,
													(int)newFace.Y + totalVerts, uv2 + totalUVs,
													(int)newFace.Z + totalVerts, uv3 + totalUVs);
											}
										}
										else
										{
											if (wroteNormals)
											{
												objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
													(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
											}
										}
									}
									else
									{
										Vector3 newFace = new Vector3((polyStrip.Indexes[stripIndx] + 1), (polyStrip.Indexes[stripIndx + 1] + 1), (polyStrip.Indexes[stripIndx + 2] + 1));

										if (set.UV != null)
										{
											int uv1 = (stripIndx) + processedUVStripCount + 1;
											int uv2 = stripIndx + 1 + processedUVStripCount + 1;
											int uv3 = stripIndx + 2 + processedUVStripCount + 1;

											if (wroteNormals)
											{
												objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
													(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
													(int)newFace.X + totalVerts, uv1 + totalUVs,
													(int)newFace.Y + totalVerts, uv2 + totalUVs,
													(int)newFace.Z + totalVerts, uv3 + totalUVs);
											}
										}
										else
										{
											if (wroteNormals)
											{
												objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
													(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
													(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
													(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
											}
											else
											{
												objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
											}
										}
									}

									triangleWindReversed = !triangleWindReversed; // flip every other triangle or the output will be wrong
								}

								if (set.UV != null)
								{
									processedUVStripCount += polyStrip.Indexes.Length;
									objstream.WriteLine("# processed UV strips this poly: {0}", processedUVStripCount);
								}
								break;

							case Basic_PolyType.Triangles:
								for (int faceVIndx = 0; faceVIndx < poly.Indexes.Length / 3; faceVIndx++)
								{
									Vector3 newFace = new Vector3((poly.Indexes[faceVIndx] + 1),
										(poly.Indexes[faceVIndx + 1] + 1), (poly.Indexes[faceVIndx + 2] + 1));

									if (set.UV != null)
									{
										int uv1 = (faceVIndx) + processedUVStripCount + 1;
										int uv2 = faceVIndx + 1 + processedUVStripCount + 1;
										int uv3 = faceVIndx + 2 + processedUVStripCount + 1;

										if (wroteNormals)
										{
											objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
												(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(int)newFace.X + totalVerts, uv1 + totalUVs,
												(int)newFace.Y + totalVerts, uv2 + totalUVs,
												(int)newFace.Z + totalVerts, uv3 + totalUVs);
										}
									}
									else
									{
										if (wroteNormals)
										{
											objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
												(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
										}
									}

									if (set.UV != null)
									{
										processedUVStripCount += 3;
										objstream.WriteLine("# processed UV strips this poly: {0}", processedUVStripCount);
									}
								}
								break;

							case Basic_PolyType.Quads:
								for (int faceVIndx = 0; faceVIndx < poly.Indexes.Length / 4; faceVIndx++)
								{
									Vector4 newFace = new Vector4((poly.Indexes[faceVIndx + 0] + 1),
										(poly.Indexes[faceVIndx + 1] + 1),
										(poly.Indexes[faceVIndx + 2] + 1),
										(poly.Indexes[faceVIndx + 3] + 1));

									if (set.UV != null)
									{
										int uv1 = faceVIndx + 0 + processedUVStripCount + 1; // +1's are because obj indeces always start at 1, not 0
										int uv2 = faceVIndx + 1 + processedUVStripCount + 1;
										int uv3 = faceVIndx + 2 + processedUVStripCount + 1;
										int uv4 = faceVIndx + 3 + processedUVStripCount + 1;

										if (wroteNormals)
										{
											objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
												(int)newFace.X + totalVerts, uv1 + totalUVs, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms);
											objstream.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
												(int)newFace.Z + totalVerts, uv3 + totalUVs, (int)newFace.Z + totalNorms,
												(int)newFace.Y + totalVerts, uv2 + totalUVs, (int)newFace.Y + totalNorms,
												(int)newFace.W + totalVerts, uv4 + totalUVs, (int)newFace.W + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(int)newFace.X + totalVerts, uv1 + totalUVs,
												(int)newFace.Y + totalVerts, uv2 + totalUVs,
												(int)newFace.Z + totalVerts, uv3 + totalUVs);
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(int)newFace.Z + totalVerts, uv3 + totalUVs,
												(int)newFace.Y + totalVerts, uv2 + totalUVs,
												(int)newFace.W + totalVerts, uv4 + totalUVs);
										}
									}
									else
									{
										if (wroteNormals)
										{
											objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
												(int)newFace.X + totalVerts, (int)newFace.X + totalNorms,
												(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
												(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms);
											objstream.WriteLine("f {0}//{1} {2}//{3} {4}//{5}",
												(int)newFace.Z + totalVerts, (int)newFace.Z + totalNorms,
												(int)newFace.Y + totalVerts, (int)newFace.Y + totalNorms,
												(int)newFace.W + totalVerts, (int)newFace.W + totalNorms);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}", (int)newFace.X + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.Z + totalVerts);
											objstream.WriteLine("f {0} {1} {2}", (int)newFace.Z + totalVerts, (int)newFace.Y + totalVerts, (int)newFace.W + totalVerts);
										}
									}

									if (set.UV != null)
									{
										processedUVStripCount += 4;
										objstream.WriteLine("# processed UV strips this poly: {0}", processedUVStripCount);
									}
								}
								break;

							case Basic_PolyType.NPoly:
								objstream.WriteLine("# Error in WriteObjFromBasicAttach() - NPoly not supported yet!");
								continue;
						}
					}

					if (set.UV != null)
					{
						totalUVs += set.UV.Length;
					}

					meshID++;
				}
				#endregion

				objstream.WriteLine("");

				// add totals
				totalVerts += basicAttach.Vertex.Length;
				totalNorms += basicAttach.Normal.Length;
			}
		}

		/// <summary>
		/// Writes an object model (chunk format) to the specified stream, in Alias-Wavefront *.OBJ format.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">idk</param>
		/// <param name="transform">Used for calculating transforms.</param>
		/// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
		/// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
		/// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
		/// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
		private static void WriteObjFromChunkAttach(StreamWriter objstream, NJS_OBJECT obj, ref List<NJS_MATERIAL> materials, Matrix transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
		{
			// add obj writing here
			if (obj.Attach != null)
			{
				ChunkAttach chunkAttach = (ChunkAttach)obj.Attach;

				if ((chunkAttach.Vertex != null) && (chunkAttach.Poly != null))
				{
					int outputVertCount = 0;
					int outputNormalCount = 0;

					objstream.WriteLine("g " + obj.Name);

					#region Outputting Verts and Normals
					int vertexChunkCount = chunkAttach.Vertex.Count;
					int polyChunkCount = chunkAttach.Poly.Count;

					if (vertexChunkCount != 1)
					{
						errorFlag = true;
						objstream.WriteLine("#A chunk model with more than one vertex chunk was found. Output is probably corrupt.");
					}

					for (int vc = 0; vc < vertexChunkCount; vc++)
					{
						for (int vIndx = 0; vIndx < chunkAttach.Vertex[vc].VertexCount; vIndx++)
						{
							if (chunkAttach.Vertex[vc].Flags == 0)
							{
								Vector3 inputVert = new Vector3(chunkAttach.Vertex[vc].Vertices[vIndx].X, chunkAttach.Vertex[vc].Vertices[vIndx].Y, chunkAttach.Vertex[vc].Vertices[vIndx].Z);
								Vector3 outputVert = Vector3.TransformCoordinate(inputVert, transform);
								objstream.WriteLine("v {0} {1} {2}", outputVert.X.ToString(NumberFormatInfo.InvariantInfo), outputVert.Y.ToString(NumberFormatInfo.InvariantInfo), outputVert.Z.ToString(NumberFormatInfo.InvariantInfo));

								outputVertCount++;
							}
						}

						if (chunkAttach.Vertex[vc].Normals.Count <= 0)
						{
							continue;
						}

						if (chunkAttach.Vertex[vc].Flags != 0)
						{
							continue;
						}

						foreach (Vertex v in chunkAttach.Vertex[vc].Normals)
						{
							objstream.WriteLine("vn {0} {1} {2}",
								v.X.ToString(NumberFormatInfo.InvariantInfo), v.Y.ToString(NumberFormatInfo.InvariantInfo), v.Z.ToString(NumberFormatInfo.InvariantInfo));
							outputNormalCount++;
						}
					}
					#endregion

					#region Outputting Polys
					for (int pc = 0; pc < polyChunkCount; pc++)
					{
						PolyChunk polyChunk = chunkAttach.Poly[pc];

						if (polyChunk is PolyChunkStrip chunkStrip)
						{
							for (int stripNum = 0; stripNum < chunkStrip.StripCount; stripNum++)
							{
								// output texture verts before use, if necessary
								bool uvsAreValid = false;
								if (chunkStrip.Strips[stripNum].UVs != null)
								{
									if (chunkStrip.Strips[stripNum].UVs.Length > 0)
									{
										uvsAreValid = true;
										foreach (UV uv in chunkStrip.Strips[stripNum].UVs)
										{
											objstream.WriteLine("vt {0} {1}", uv.U.ToString(NumberFormatInfo.InvariantInfo), (-uv.V).ToString(NumberFormatInfo.InvariantInfo));
										}
									}
								}

								bool windingReversed = chunkStrip.Strips[stripNum].Reversed;
								for (int currentStripIndx = 0; currentStripIndx < chunkStrip.Strips[stripNum].Indexes.Length - 2; currentStripIndx++)
								{
									if (windingReversed)
									{
										if (uvsAreValid)
										{
											// note to self - uvs.length will equal strip indeces length! They are directly linked, just like you remembered.
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, (currentStripIndx + 1 + totalUVs) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, (currentStripIndx + totalUVs) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1, (currentStripIndx + 2 + totalUVs) + 1);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1);
										}
									}
									else
									{
										if (uvsAreValid)
										{
											// note to self - uvs.length will equal strip indeces length! They are directly linked, just like you remembered.
											objstream.WriteLine("f {0}/{1} {2}/{3} {4}/{5}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1, currentStripIndx + totalUVs + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1, currentStripIndx + 1 + totalUVs + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1, currentStripIndx + 2 + totalUVs + 1);
										}
										else
										{
											objstream.WriteLine("f {0} {1} {2}",
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 1] + totalVerts) + 1,
												(chunkStrip.Strips[stripNum].Indexes[currentStripIndx + 2] + totalVerts) + 1);
										}
									}

									windingReversed = !windingReversed;
								}

								// increment output verts
								if (uvsAreValid) totalUVs += chunkStrip.Strips[stripNum].UVs.Length;
							}
						}
						else if (polyChunk is PolyChunkMaterial)
						{
							// no behavior defined yet.
						}
						else if (polyChunk is PolyChunkTinyTextureID chunkTexID)
						{
							//objstream.WriteLine("usemtl {0}_material_{1}", materialPrefix, chunkTexID.TextureID);
							// no behavior defined yet
						}
					}
					#endregion

					totalVerts += outputVertCount;
					totalNorms += outputNormalCount;
				}
				else
				{
					errorFlag = true;
					objstream.WriteLine("#A chunk model with no vertex or no poly was found. Output is definitely corrupt.");
				}
			}
		}

		/// <summary>
		/// Primary method for exporting models to Wavefront *.OBJ format. This will auto-detect the model type and send it to the proper export method.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">used to prevent name collisions if mixing/matching outputs.</param>
		/// <param name="transform">Used for calculating transforms.</param>
		/// <param name="totalVerts">This keeps track of how many verts have been exported to the current file. This is necessary because *.obj vertex indeces are file-level, not object-level.</param>
		/// <param name="totalNorms">This keeps track of how many vert normals have been exported to the current file. This is necessary because *.obj vertex normal indeces are file-level, not object-level.</param>
		/// <param name="totalUVs">This keeps track of how many texture verts have been exported to the current file. This is necessary because *.obj textue vert indeces are file-level, not object-level.</param>
		/// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
		public static void WriteModelAsObj(StreamWriter objstream, NJS_OBJECT obj, ref List<NJS_MATERIAL> materials, MatrixStack transform, ref int totalVerts, ref int totalNorms, ref int totalUVs, ref bool errorFlag)
		{
			transform.Push();
			obj.ProcessTransforms(transform);
			if (obj.Attach is BasicAttach)
				WriteObjFromBasicAttach(objstream, obj, ref materials, transform.Top, ref totalVerts, ref totalNorms, ref totalUVs);
			else if (obj.Attach is ChunkAttach)
				WriteObjFromChunkAttach(objstream, obj, ref materials, transform.Top, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
			foreach (NJS_OBJECT child in obj.Children)
				WriteModelAsObj(objstream, child, ref materials, transform, ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
			transform.Pop();
		}

		/// <summary>
		/// Primary method for exporting models to Wavefront *.OBJ format. This will auto-detect the model type and send it to the proper export method.
		/// </summary>
		/// <param name="objstream">stream representing a wavefront obj file to export to</param>
		/// <param name="obj">Model to export.</param>
		/// <param name="materialPrefix">used to prevent name collisions if mixing/matching outputs.</param>
		/// <param name="errorFlag">Set this to TRUE if you encounter an issue. The user will be alerted.</param>
		public static void WriteSingleModelAsObj(StreamWriter objstream, NJS_OBJECT obj, ref List<NJS_MATERIAL> materials, ref bool errorFlag)
		{
			int v = 0, n = 0, u = 0;
			if (obj.Attach is BasicAttach)
				WriteObjFromBasicAttach(objstream, obj, ref materials, Matrix.Identity, ref v, ref n, ref u);
			else if (obj.Attach is ChunkAttach)
				WriteObjFromChunkAttach(objstream, obj, ref materials, Matrix.Identity, ref v, ref n, ref u, ref errorFlag);
		}

		// Old OBJ export from SAMDL. Keeps original vertex order.
		public static void ExporObjLegacy(string objFileName, string TexturePackName, NJS_OBJECT obj, BMPInfo[] TextureInfo)
		{
			using (StreamWriter objstream = new StreamWriter(objFileName, false))
			{
				List<NJS_MATERIAL> materials = new List<NJS_MATERIAL>();
				if (TexturePackName == null || TexturePackName == "") TexturePackName = Path.ChangeExtension(objFileName, ".mtl");
				objstream.WriteLine("mtllib " + TexturePackName + ".mtl");
				bool errorFlag = false;

				Direct3D.Extensions.WriteSingleModelAsObj(objstream, obj, ref materials, ref errorFlag);

				string mypath = Path.GetDirectoryName(objFileName);
				using (StreamWriter mtlstream = new StreamWriter(Path.Combine(mypath, TexturePackName + ".mtl"), false))
				{
					for (int i = 0; i < materials.Count; i++)
					{
						int texIndx = materials[i].TextureID;

						NJS_MATERIAL material = materials[i];
						mtlstream.WriteLine("newmtl material_{0}", i);
						mtlstream.WriteLine("Ka 1 1 1");
						mtlstream.WriteLine(string.Format("Kd {0} {1} {2}",
							((float)material.DiffuseColor.R / 255.0f).ToC(true),
							((float)material.DiffuseColor.G / 255.0f).ToC(true),
							((float)material.DiffuseColor.B / 255.0f).ToC(true)));

						mtlstream.WriteLine(string.Format("Ks {0} {1} {2}",
							((float)material.SpecularColor.R / 255.0f).ToC(true),
							((float)material.SpecularColor.G / 255.0f).ToC(true),
							((float)material.SpecularColor.B / 255.0f).ToC(true)));
						mtlstream.WriteLine("illum 1");

						if (TextureInfo != null && !string.IsNullOrEmpty(TextureInfo[texIndx].Name) && material.UseTexture)
						{
							mtlstream.WriteLine("Map_Kd " + TextureInfo[texIndx].Name + ".png");

							// save texture

							TextureInfo[texIndx].Image.Save(Path.Combine(mypath, TextureInfo[texIndx].Name + ".png"));
						}
					}
				}
			}

		}
	}
}
