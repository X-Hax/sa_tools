using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Collada141;

namespace SonicRetro.SAModel
{
	[Serializable]
	public class NJS_OBJECT
	{
		[Browsable(false)]
		public Attach Attach { get; set; }

		public Vertex Position { get; set; }
		public Rotation Rotation { get; set; }
		public Vertex Scale { get; set; }

		[Browsable(false)]
		public List<NJS_OBJECT> Children { get; set; }

		internal NJS_OBJECT Sibling { get; set; }
		public string Name { get; set; }

		public bool RotateZYX { get; set; }

		[DefaultValue(true)]
		public bool Animate { get; set; }

		[DefaultValue(true)]
		public bool Morph { get; set; }

		public static int Size
		{
			get { return 0x34; }
		}

		public NJS_OBJECT()
		{
			Name = "object_" + Extensions.GenerateIdentifier();
			Position = new Vertex();
			Rotation = new Rotation();
			Scale = new Vertex(1, 1, 1);
			Children = new List<NJS_OBJECT>();
		}

		public NJS_OBJECT(byte[] file, int address, uint imageBase, ModelFormat format)
			: this(file, address, imageBase, format, new Dictionary<int, string>())
		{
		}

		public NJS_OBJECT(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
		{
			if (labels.ContainsKey(address))
				Name = labels[address];
			else
				Name = "object_" + address.ToString("X8");
			ObjectFlags flags = (ObjectFlags)ByteConverter.ToInt32(file, address);
			RotateZYX = (flags & ObjectFlags.RotateZYX) == ObjectFlags.RotateZYX;
			Animate = (flags & ObjectFlags.NoAnimate) == 0;
			Morph = (flags & ObjectFlags.NoMorph) == 0;
			int tmpaddr = ByteConverter.ToInt32(file, address + 4);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				Attach = Attach.Load(file, tmpaddr, imageBase, format, labels);
			}
			Position = new Vertex(file, address + 8);
			Rotation = new Rotation(file, address + 0x14);
			Scale = new Vertex(file, address + 0x20);
			Children = new List<NJS_OBJECT>();
			NJS_OBJECT child = null;
			tmpaddr = ByteConverter.ToInt32(file, address + 0x2C);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				child = new NJS_OBJECT(file, tmpaddr, imageBase, format, labels);
			}
			while (child != null)
			{
				Children.Add(child);
				child = child.Sibling;
			}
			tmpaddr = ByteConverter.ToInt32(file, address + 0x30);
			if (tmpaddr != 0)
			{
				tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
				Sibling = new NJS_OBJECT(file, tmpaddr, imageBase, format, labels);
			}
		}

		public byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
		{
			for (int i = 1; i < Children.Count; i++)
				Children[i - 1].Sibling = Children[i];
			List<byte> result = new List<byte>();
			uint childaddr = 0;
			uint siblingaddr = 0;
			uint attachaddr = 0;
			byte[] tmpbyte;
			if (Children.Count > 0)
			{
				if (labels.ContainsKey(Children[0].Name))
					childaddr = labels[Children[0].Name];
				else
				{
					result.Align(4);
					result.AddRange(Children[0].GetBytes(imageBase, DX, labels, out childaddr));
					childaddr += imageBase;
				}
			}
			if (Sibling != null)
			{
				if (labels.ContainsKey(Sibling.Name))
					siblingaddr = labels[Sibling.Name];
				else
				{
					result.Align(4);
					tmpbyte = Sibling.GetBytes(imageBase + (uint)result.Count, DX, labels, out siblingaddr);
					siblingaddr += imageBase + (uint)result.Count;
					result.AddRange(tmpbyte);
				}
			}
			if (Attach != null)
			{
				if (labels.ContainsKey(Attach.Name))
					attachaddr = labels[Attach.Name];
				else
				{
					result.Align(4);
					tmpbyte = Attach.GetBytes(imageBase + (uint)result.Count, DX, labels, out attachaddr);
					attachaddr += imageBase + (uint)result.Count;
					result.AddRange(tmpbyte);
				}
			}
			result.Align(4);
			address = (uint)result.Count;
			ObjectFlags flags = GetFlags();
			result.AddRange(ByteConverter.GetBytes((int)flags));
			result.AddRange(ByteConverter.GetBytes(attachaddr));
			result.AddRange(Position.GetBytes());
			result.AddRange(Rotation.GetBytes());
			result.AddRange(Scale.GetBytes());
			result.AddRange(ByteConverter.GetBytes(childaddr));
			result.AddRange(ByteConverter.GetBytes(siblingaddr));
			labels.Add(Name, address + imageBase);
			return result.ToArray();
		}

		public ObjectFlags GetFlags()
		{
			ObjectFlags flags = 0;
			if (Position.IsEmpty)
				flags = ObjectFlags.NoPosition;
			if (Rotation.IsEmpty)
				flags |= ObjectFlags.NoRotate;
			if (Scale.X == 1 && Scale.Y == 1 && Scale.Z == 1)
				flags |= ObjectFlags.NoScale;
			if (Attach == null)
				flags |= ObjectFlags.NoDisplay;
			if (Children.Count == 0)
				flags |= ObjectFlags.NoChildren;
			if (RotateZYX)
				flags |= ObjectFlags.RotateZYX;
			if (!Animate)
				flags |= ObjectFlags.NoAnimate;
			if (!Morph)
				flags |= ObjectFlags.NoMorph;
			return flags;
		}

		public byte[] GetBytes(uint imageBase, bool DX, out uint address)
		{
			return GetBytes(imageBase, DX, new Dictionary<string, uint>(), out address);
		}

		public byte[] GetBytes(uint imageBase, bool DX)
		{
			uint address;
			return GetBytes(imageBase, DX, out address);
		}

		public NJS_OBJECT[] GetObjects()
		{
			List<NJS_OBJECT> result = new List<NJS_OBJECT> { this };
			foreach (NJS_OBJECT item in Children)
				result.AddRange(item.GetObjects());
			return result.ToArray();
		}

		public int CountAnimated()
		{
			int result = Animate ? 1 : 0;
			foreach (NJS_OBJECT item in Children)
				result += item.CountAnimated();
			return result;
		}

		public int CountMorph()
		{
			int result = Morph ? 1 : 0;
			foreach (NJS_OBJECT item in Children)
				result += item.CountMorph();
			return result;
		}

		public void ProcessVertexData()
		{
#if modellog
			Extensions.Log("Processing Object " + Name + Environment.NewLine);
#endif
			if (Attach != null)
				Attach.ProcessVertexData();
			foreach (NJS_OBJECT item in Children)
				item.ProcessVertexData();
		}

		public COLLADA ToCollada(int texcount)
		{
			string[] texs = new string[texcount];
			for (int i = 0; i < texcount; i++)
				texs[i] = "image_" + (i + 1).ToString(NumberFormatInfo.InvariantInfo);
			return ToCollada(texs);
		}

		public COLLADA ToCollada(string[] textures)
		{
			COLLADA result = new COLLADA
			{
				version = VersionType.Item140,
				asset = new asset
				{
					contributor = new assetContributor[] { new assetContributor() { authoring_tool = "SAModel" } },
					created = DateTime.UtcNow,
					modified = DateTime.UtcNow
				}
			};
			List<object> libraries = new List<object>();
			List<image> images = new List<image>();
			if (textures != null)
			{
				for (int i = 0; i < textures.Length; i++)
				{
					images.Add(new image
					{
						id = "image_" + (i + 1).ToString(NumberFormatInfo.InvariantInfo),
						name = "image_" + (i + 1).ToString(NumberFormatInfo.InvariantInfo),
						Item = textures[i] + ".png"
					});
				}
			}
			libraries.Add(new library_images { image = images.ToArray() });
			List<material> materials = new List<material>();
			List<effect> effects = new List<effect>();
			List<geometry> geometries = new List<geometry>();
			List<string> visitedAttaches = new List<string>();
			node node = AddToCollada(materials, effects, geometries, visitedAttaches, textures != null);
			libraries.Add(new library_materials { material = materials.ToArray() });
			libraries.Add(new library_effects { effect = effects.ToArray() });
			libraries.Add(new library_geometries { geometry = geometries.ToArray() });
			libraries.Add(new library_visual_scenes
			{
				visual_scene = new visual_scene[]
				{
					new visual_scene
					{
						id = "RootNode",
						node = new node[] { node }
					}
				}
			});
			result.Items = libraries.ToArray();
			result.scene = new COLLADAScene { instance_visual_scene = new InstanceWithExtra { url = "#RootNode" } };
			return result;
		}

		protected node AddToCollada(List<material> materials, List<effect> effects, List<geometry> geometries,
			List<string> visitedAttaches, bool hasTextures)
		{
			BasicAttach attach = Attach as BasicAttach;
			if (attach == null || visitedAttaches.Contains(attach.Name))
				goto skipAttach;
			visitedAttaches.Add(attach.Name);
			int m = 0;
			foreach (NJS_MATERIAL item in attach.Material)
			{
				materials.Add(new material
				{
					id = "material_" + attach.Name + "_" + m,
					name = "material_" + attach.Name + "_" + m,
					instance_effect = new instance_effect
					{
						url = "#" + "material_" + attach.Name + "_" + m + "_eff"
					}
				});
				if (hasTextures & item.UseTexture)
				{
					effects.Add(new effect
					{
						id = "material_" + attach.Name + "_" + m + "_eff",
						name = "material_" + attach.Name + "_" + m + "_eff",
						Items = new effectFx_profile_abstractProfile_COMMON[]
						{
							new effectFx_profile_abstractProfile_COMMON
							{
								Items = new object[]
								{
									new common_newparam_type
									{
										sid = "material_" + attach.Name + "_" + m + "_eff_surface",
										/*Item = new Collada141.fx_sampler2D_common()
                                         { instance_image = new Collada141.instance_image() { url = "#image_" + (item.TextureID + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) } },
                                         ItemElementName = Collada141.ItemChoiceType.sampler2D*/
										Item = new fx_surface_common
										{
											type = fx_surface_type_enum.Item2D,
											init_from =
												new fx_surface_init_from_common[]
												{
													new fx_surface_init_from_common
													{
														Value = "image_" + (item.TextureID + 1).ToString(NumberFormatInfo.InvariantInfo)
													}
												}
										},
										ItemElementName = ItemChoiceType.surface
									}
								},
								technique = new effectFx_profile_abstractProfile_COMMONTechnique
								{
									sid = "standard",
									Item = new effectFx_profile_abstractProfile_COMMONTechniquePhong
									{
										ambient = new common_color_or_texture_type
										{
											Item = new common_color_or_texture_typeTexture
											{
												texture = "material_" + attach.Name + "_" + m + "_eff_surface",
												texcoord = "CHANNEL0"
											}
										},
										diffuse = new common_color_or_texture_type
										{
											Item = new common_color_or_texture_typeColor
											{
												Values =
													new double[]
													{
														item.DiffuseColor.R / 255d, item.DiffuseColor.G / 255d, item.DiffuseColor.B / 255d,
														item.UseAlpha ? item.DiffuseColor.A / 255d : 1
													}
											}
										}
									}
								}
							}
						}
					});
				}
				else
				{
					effects.Add(new effect
					{
						id = "material_" + attach.Name + "_" + m + "_eff",
						name = "material_" + attach.Name + "_" + m + "_eff",
						Items = new effectFx_profile_abstractProfile_COMMON[]
						{
							new effectFx_profile_abstractProfile_COMMON
							{
								technique = new effectFx_profile_abstractProfile_COMMONTechnique
								{
									sid = "standard",
									Item = new effectFx_profile_abstractProfile_COMMONTechniquePhong
									{
										diffuse = new common_color_or_texture_type
										{
											Item = new common_color_or_texture_typeColor
											{
												Values =
													new double[]
													{
														item.DiffuseColor.R / 255d, item.DiffuseColor.G / 255d, item.DiffuseColor.B / 255d,
														item.UseAlpha ? item.DiffuseColor.A / 255d : 1
													}
											}
										}
									}
								}
							}
						}
					});
				}
				m++;
			}
			List<double> verts = new List<double>();
			foreach (Vertex item in attach.Vertex)
			{
				verts.Add(item.X);
				verts.Add(item.Y);
				verts.Add(item.Z);
			}
			source pos = new source
			{
				id = attach.Name + "_position",
				Item = new float_array
				{
					id = attach.Name + "_position_array",
					count = (ulong)verts.Count,
					Values = verts.ToArray()
				},
				technique_common = new sourceTechnique_common
				{
					accessor = new accessor
					{
						source = "#" + attach.Name + "_position_array",
						count = (ulong)(verts.Count / 3),
						stride = 3,
						param =
							new param[]
							{
								new param { name = "X", type = "float" }, new param { name = "Y", type = "float" },
								new param { name = "Z", type = "float" }
							}
					}
				}
			};
			verts = new List<double>();
			foreach (Vertex item in attach.Normal)
			{
				verts.Add(item.X);
				verts.Add(item.Y);
				verts.Add(item.Z);
			}
			source nor = new source
			{
				id = attach.Name + "_normal",
				Item = new float_array
				{
					id = attach.Name + "_normal_array",
					count = (ulong)verts.Count,
					Values = verts.ToArray()
				},
				technique_common = new sourceTechnique_common
				{
					accessor = new accessor
					{
						source = "#" + attach.Name + "_normal_array",
						count = (ulong)(verts.Count / 3),
						stride = 3,
						param =
							new param[]
							{
								new param { name = "X", type = "float" }, new param { name = "Y", type = "float" },
								new param { name = "Z", type = "float" }
							}
					}
				}
			};
			List<source> srcs = new List<source> { pos, nor };
			foreach (NJS_MESHSET mitem in attach.Mesh)
			{
				if (mitem.UV != null)
				{
					verts = new List<double>();
					foreach (UV item in mitem.UV)
					{
						verts.Add(item.U);
						verts.Add(-item.V);
					}
					srcs.Add(new source
					{
						id = mitem.UVName,
						Item = new float_array
						{
							id = mitem.UVName + "_array",
							count = (ulong)verts.Count,
							Values = verts.ToArray()
						},
						technique_common = new sourceTechnique_common
						{
							accessor = new accessor
							{
								source = "#" + mitem.UVName + "_array",
								count = (ulong)(verts.Count / 2),
								stride = 2,
								param = new param[] { new param { name = "S", type = "float" }, new param { name = "T", type = "float" } }
							}
						}
					});
				}
			}
			List<triangles> tris = new List<triangles>();
			foreach (NJS_MESHSET mesh in attach.Mesh)
			{
				bool hasVColor = mesh.VColor != null;
				bool hasUV = mesh.UV != null;
				uint currentstriptotal = 0;
				foreach (Poly poly in mesh.Poly)
				{
					List<uint> inds = new List<uint>();
					switch (mesh.PolyType)
					{
						case Basic_PolyType.Triangles:
							for (uint i = 0; i < 3; i++)
							{
								inds.Add(poly.Indexes[i]);
								if (hasUV)
									inds.Add(currentstriptotal + i);
							}
							currentstriptotal += 3;
							break;
						case Basic_PolyType.Quads:
							for (uint i = 0; i < 3; i++)
							{
								inds.Add(poly.Indexes[i]);
								if (hasUV)
									inds.Add(currentstriptotal + i);
							}
							for (uint i = 1; i < 4; i++)
							{
								inds.Add(poly.Indexes[i]);
								if (hasUV)
									inds.Add(currentstriptotal + i);
							}
							currentstriptotal += 4;
							break;
						case Basic_PolyType.NPoly:
						case Basic_PolyType.Strips:
							bool flip = !((Strip)poly).Reversed;
							for (int k = 0; k < poly.Indexes.Length - 2; k++)
							{
								flip = !flip;
								if (!flip)
								{
									for (uint i = 0; i < 3; i++)
									{
										inds.Add(poly.Indexes[k + i]);
										if (hasUV)
											inds.Add(currentstriptotal + i);
									}
								}
								else
								{
									inds.Add(poly.Indexes[k + 1]);
									if (hasUV)
										inds.Add(currentstriptotal + 1);
									inds.Add(poly.Indexes[k]);
									if (hasUV)
										inds.Add(currentstriptotal);
									inds.Add(poly.Indexes[k + 2]);
									if (hasUV)
										inds.Add(currentstriptotal + 2);
								}
								currentstriptotal += 1;
							}
							currentstriptotal += 2;
							break;
					}
					string[] indstr = new string[inds.Count];
					for (int i = 0; i < inds.Count; i++)
						indstr[i] = inds[i].ToString(NumberFormatInfo.InvariantInfo);
					List<InputLocalOffset> inp = new List<InputLocalOffset>
					{
						new InputLocalOffset { semantic = "VERTEX", offset = 0, source = "#" + attach.Name + "_vertices" }
					};
					if (hasUV)
					{
						inp.Add(new InputLocalOffset
						{
							semantic = "TEXCOORD",
							offset = 1,
							source = "#" + mesh.UVName,
							setSpecified = true
						});
					}
					tris.Add(new triangles
					{
						material = "material_" + attach.Name + "_" + mesh.MaterialID,
						count = (ulong)(inds.Count / (hasUV ? 6 : 3)),
						input = inp.ToArray(),
						p = string.Join(" ", indstr)
					});
				}
			}
			geometries.Add(new geometry
			{
				id = attach.Name,
				name = attach.Name,
				Item = new mesh
				{
					source = srcs.ToArray(),
					vertices = new vertices
					{
						id = attach.Name + "_vertices",
						input = new InputLocal[]
						{
							new InputLocal
							{
								semantic = "POSITION",
								source = "#" + attach.Name + "_position"
							},
							new InputLocal
							{
								semantic = "NORMAL",
								source = "#" + attach.Name + "_normal"
							}
						}
					},
					Items = tris.ToArray()
				}
			});
			skipAttach:
			node node = new node
			{
				id = Name,
				name = Name,
				Items = new object[]
				{
					new TargetableFloat3 { sid = "translate", Values = new double[] { Position.X, Position.Y, Position.Z } },
					new rotate { sid = "rotateZ", Values = new double[] { 0, 0, 1, Rotation.ZDeg } },
					new rotate { sid = "rotateX", Values = new double[] { 1, 0, 0, Rotation.XDeg } },
					new rotate { sid = "rotateY", Values = new double[] { 0, 1, 0, Rotation.YDeg } },
					new TargetableFloat3 { sid = "scale", Values = new double[] { Scale.X, Scale.Y, Scale.Z } }
				},
				ItemsElementName = new ItemsChoiceType2[]
				{
					ItemsChoiceType2.translate,
					ItemsChoiceType2.rotate,
					ItemsChoiceType2.rotate,
					ItemsChoiceType2.rotate,
					ItemsChoiceType2.scale
				}
			};
			if (attach != null)
			{
				List<instance_material> mats = new List<instance_material>();
				foreach (NJS_MESHSET item in attach.Mesh)
				{
					mats.Add(new instance_material
					{
						symbol = "material_" + attach.Name + "_" + item.MaterialID,
						target = "#" + "material_" + attach.Name + "_" + item.MaterialID
					});
				}
				node.instance_geometry = new instance_geometry[]
				{
					new instance_geometry
					{
						url = "#" + attach.Name,
						bind_material = new bind_material { technique_common = mats.ToArray() }
					}
				};
			}
			List<node> childnodes = new List<node>();
			foreach (NJS_OBJECT item in Children)
				childnodes.Add(item.AddToCollada(materials, effects, geometries, visitedAttaches, hasTextures));
			node.node1 = childnodes.ToArray();
			return node;
		}

		public NJS_OBJECT ToBasicModel()
		{
			List<NJS_OBJECT> newchildren = new List<NJS_OBJECT>(Children.Count);
			foreach (NJS_OBJECT item in Children)
				newchildren.Add(item.ToBasicModel());
			NJS_OBJECT result = new NJS_OBJECT();
			if (Attach != null)
				result.Attach = Attach.ToBasicModel();
			result.Position = Position;
			result.Rotation = Rotation;
			result.Scale = Scale;
			result.Children = newchildren;
			return result;
		}

		public NJS_OBJECT ToChunkModel()
		{
			List<NJS_OBJECT> newchildren = new List<NJS_OBJECT>(Children.Count);
			foreach (NJS_OBJECT item in Children)
				newchildren.Add(item.ToBasicModel());
			NJS_OBJECT result = new NJS_OBJECT();
			if (Attach != null)
				result.Attach = Attach.ToChunkModel();
			result.Position = Position;
			result.Rotation = Rotation;
			result.Scale = Scale;
			result.Children = newchildren;
			return result;
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(((StructEnums.NJD_EVAL)GetFlags()).ToString().Replace(", ", " | "));
			result.Append(", ");
			result.Append(Attach != null ? "&" + Attach.Name : "NULL");
			foreach (float value in Position.ToArray())
			{
				result.Append(", ");
				result.Append(value.ToC());
			}
			foreach (int value in Rotation.ToArray())
			{
				result.Append(", ");
				result.Append(value.ToCHex());
			}
			foreach (float value in Scale.ToArray())
			{
				result.Append(", ");
				result.Append(value.ToC());
			}
			result.Append(", ");
			result.Append(Children.Count > 0 ? "&" + Children[0].Name : "NULL");
			result.Append(", ");
			result.Append(Sibling != null ? "&" + Sibling.Name : "NULL");
			result.Append(" }");
			return result.ToString();
		}

		public string ToStructVariables(bool DX, List<string> labels, string[] textures)
		{
			for (int i = 1; i < Children.Count; i++)
				Children[i - 1].Sibling = Children[i];
			StringBuilder result = new StringBuilder();
			for (int i = Children.Count - 1; i >= 0; i--)
			{
				if (!labels.Contains(Children[i].Name))
				{
					labels.Add(Children[i].Name);
					result.AppendLine(Children[i].ToStructVariables(DX, labels, textures));
				}
			}
			if (Attach != null && !labels.Contains(Attach.Name))
			{
				labels.Add(Attach.Name);
				result.AppendLine(Attach.ToStructVariables(DX, labels, textures));
			}
			result.Append("NJS_OBJECT ");
			result.Append(Name);
			result.Append(" = ");
			result.Append(ToStruct());
			result.AppendLine(";");
			return result.ToString();
		}

		public string ToStructVariables(bool DX, List<string> labels)
		{
			return ToStructVariables(DX, labels, null);
		}
	}
}