using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SAModel
{
    public class Object
    {
        [Browsable(false)]
        public ObjectFlags Flags { get; set; }
        [Browsable(false)]
        public Attach Attach { get; set; }
        public Vertex Position { get; set; }
        public Rotation Rotation { get; set; }
        public Vertex Scale { get; set; }
        [Browsable(false)]
        public List<Object> Children { get; set; }
        internal Object Sibling { get; set; }
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool Render { get { return (Flags & ObjectFlags.NoDisplay) == 0; } set { Flags = (ObjectFlags)((Flags & ~ObjectFlags.NoDisplay) | (value ? 0 : ObjectFlags.NoDisplay)); } }
        [DefaultValue(true)]
        public bool Animate { get { return (Flags & ObjectFlags.NoAnimate) == 0; } set { Flags = (ObjectFlags)((Flags & ~ObjectFlags.NoAnimate) | (value ? 0 : ObjectFlags.NoAnimate)); } }
        [DefaultValue(true)]
        public bool Morph { get { return (Flags & ObjectFlags.NoMorph) == 0; } set { Flags = (ObjectFlags)((Flags & ~ObjectFlags.NoMorph) | (value ? 0 : ObjectFlags.NoMorph)); } }

        public static int Size { get { return 0x34; } }

        public Object()
        {
            Name = "object_" + GenerateIdentifier();
            Position = new Vertex();
            Rotation = new Rotation();
            Scale = new Vertex(1, 1, 1);
            Children = new List<Object>();
        }

        public Object(byte[] file, int address, uint imageBase, ModelFormat format)
            : this(file, address, imageBase, format, new Dictionary<int, string>()) { }

        public Object(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "object_" + address.ToString("X8");
            Flags = (ObjectFlags)ByteConverter.ToInt32(file, address);
            int tmpaddr = ByteConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Attach = Attach.Load(file, tmpaddr, imageBase, format, labels);
            }
            Position = new Vertex(file, address + 8);
            Rotation = new Rotation(file, address + 0x14);
            Scale = new Vertex(file, address + 0x20);
            Children = new List<Object>();
            Object child = null;
            tmpaddr = ByteConverter.ToInt32(file, address + 0x2C);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                child = new Object(file, tmpaddr, imageBase, format, labels);
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
                Sibling = new Object(file, tmpaddr, imageBase, format, labels);
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
            result.AddRange(ByteConverter.GetBytes((int)Flags));
            result.AddRange(ByteConverter.GetBytes(attachaddr));
            result.AddRange(Position.GetBytes());
            result.AddRange(Rotation.GetBytes());
            result.AddRange(Scale.GetBytes());
            result.AddRange(ByteConverter.GetBytes(childaddr));
            result.AddRange(ByteConverter.GetBytes(siblingaddr));
            labels.Add(Name, address + imageBase);
            return result.ToArray();
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

        public Object[] GetObjects()
        {
            List<Object> result = new List<Object>() { this };
            foreach (Object item in Children)
                result.AddRange(item.GetObjects());
            return result.ToArray();
        }

        public int CountAnimated()
        {
            int result = (Flags & ObjectFlags.NoAnimate) == ObjectFlags.NoAnimate ? 0 : 1;
            foreach (Object item in Children)
                result += item.CountAnimated();
            return result;
        }

        public int CountMorph()
        {
            int result = (Flags & ObjectFlags.NoMorph) == ObjectFlags.NoMorph ? 0 : 1;
            foreach (Object item in Children)
                result += item.CountMorph();
            return result;
        }

        public void ProcessVertexData()
        {
            if (Attach != null) Attach.ProcessVertexData();
            foreach (Object item in Children)
                item.ProcessVertexData();
        }

        public Collada141.COLLADA ToCollada(int texcount)
        {
            string[] texs = new string[texcount];
            for (int i = 0; i < texcount; i++)
                texs[i] = "image_" + (i + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            return ToCollada(texs);
        }

        public Collada141.COLLADA ToCollada(string[] textures)
        {
            Collada141.COLLADA result = new Collada141.COLLADA();
            result.version = Collada141.VersionType.Item140;
            result.asset = new Collada141.asset() { contributor = new Collada141.assetContributor[] { new Collada141.assetContributor() { authoring_tool = "SAModel" } }, created = DateTime.UtcNow, modified = DateTime.UtcNow };
            List<object> libraries = new List<object>();
            List<Collada141.image> images = new List<Collada141.image>();
            if (textures != null)
                for (int i = 0; i < textures.Length; i++)
                    images.Add(new Collada141.image()
                    {
                        id = "image_" + (i + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo),
                        name = "image_" + (i + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo),
                        Item = textures[i] + ".png"
                    });
            libraries.Add(new Collada141.library_images() { image = images.ToArray() });
            List<Collada141.material> materials = new List<Collada141.material>();
            List<Collada141.effect> effects = new List<Collada141.effect>();
            List<Collada141.geometry> geometries = new List<Collada141.geometry>();
            List<string> visitedAttaches = new List<string>();
            Collada141.node node = AddToCollada(materials, effects, geometries, visitedAttaches, textures != null);
            libraries.Add(new Collada141.library_materials() { material = materials.ToArray() });
            libraries.Add(new Collada141.library_effects() { effect = effects.ToArray() });
            libraries.Add(new Collada141.library_geometries() { geometry = geometries.ToArray() });
            libraries.Add(new Collada141.library_visual_scenes()
            {
                visual_scene = new Collada141.visual_scene[]
                {
                    new Collada141.visual_scene()
                    {
                        id = "RootNode",
                        node = new Collada141.node[] { node }
                    }
                }
            });
            result.Items = libraries.ToArray();
            result.scene = new Collada141.COLLADAScene() { instance_visual_scene = new Collada141.InstanceWithExtra() { url = "#RootNode" } };
            return result;
        }

        protected Collada141.node AddToCollada(List<Collada141.material> materials, List<Collada141.effect> effects, List<Collada141.geometry> geometries, List<string> visitedAttaches, bool hasTextures)
        {
            BasicAttach Attach = this.Attach as BasicAttach;
            if (Attach == null || visitedAttaches.Contains(Attach.Name))
                goto skipAttach;
            visitedAttaches.Add(Attach.Name);
            int m = 0;
            foreach (Material item in Attach.Material)
            {
                materials.Add(new Collada141.material()
                {
                    id = "material_" + Attach.Name + "_" + m,
                    name = "material_" + Attach.Name + "_" + m,
                    instance_effect = new Collada141.instance_effect()
                    {
                        url = "#" + "material_" + Attach.Name + "_" + m + "_eff"
                    }
                });
                if (hasTextures & item.UseTexture)
                {
                    effects.Add(new Collada141.effect()
                    {
                        id = "material_" + Attach.Name + "_" + m + "_eff",
                        name = "material_" + Attach.Name + "_" + m + "_eff",
                        Items = new Collada141.effectFx_profile_abstractProfile_COMMON[]
                        {
                            new Collada141.effectFx_profile_abstractProfile_COMMON()
                            {
                                Items = new object[]
                                {
                                    new Collada141.common_newparam_type()
                                    {
                                         sid = "material_" + Attach.Name + "_" + m + "_eff_surface",
                                         /*Item = new Collada141.fx_sampler2D_common()
                                         { instance_image = new Collada141.instance_image() { url = "#image_" + (item.TextureID + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) } },
                                         ItemElementName = Collada141.ItemChoiceType.sampler2D*/
                                         Item = new Collada141.fx_surface_common()
                                         {
                                             type = Collada141.fx_surface_type_enum.Item2D,
                                             init_from = new Collada141.fx_surface_init_from_common[] { new Collada141.fx_surface_init_from_common() { Value  = "image_" + (item.TextureID + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) } }
                                         },
                                         ItemElementName = Collada141.ItemChoiceType.surface
                                    }
                                },
                                technique = new Collada141.effectFx_profile_abstractProfile_COMMONTechnique()
                                {
                                    sid = "standard",
                                    Item = new Collada141.effectFx_profile_abstractProfile_COMMONTechniquePhong()
                                    {
                                        ambient = new Collada141.common_color_or_texture_type()
                                        {
                                            Item = new Collada141.common_color_or_texture_typeTexture()
                                            {
                                                texture = "material_" + Attach.Name + "_" + m + "_eff_surface",
                                                texcoord = "CHANNEL0"
                                            }
                                        },
                                        diffuse = new Collada141.common_color_or_texture_type()
                                        {
                                            Item = new Collada141.common_color_or_texture_typeColor()
                                            {
                                                Values = new double[] { item.DiffuseColor.R / 255d, item.DiffuseColor.G / 255d, item.DiffuseColor.B / 255d, item.UseAlpha ? item.DiffuseColor.A / 255d : 1 }
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
                    effects.Add(new Collada141.effect()
                    {
                        id = "material_" + Attach.Name + "_" + m + "_eff",
                        name = "material_" + Attach.Name + "_" + m + "_eff",
                        Items = new Collada141.effectFx_profile_abstractProfile_COMMON[]
                        {
                            new Collada141.effectFx_profile_abstractProfile_COMMON()
                            {
                                technique = new Collada141.effectFx_profile_abstractProfile_COMMONTechnique()
                                {
                                    sid = "standard",
                                    Item = new Collada141.effectFx_profile_abstractProfile_COMMONTechniquePhong()
                                    {
                                        diffuse = new Collada141.common_color_or_texture_type()
                                        {
                                            Item = new Collada141.common_color_or_texture_typeColor()
                                            {
                                                Values = new double[] { item.DiffuseColor.R / 255d, item.DiffuseColor.G / 255d, item.DiffuseColor.B / 255d, item.UseAlpha ? item.DiffuseColor.A / 255d : 1 }
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
            foreach (Vertex item in Attach.Vertex)
            {
                verts.Add(item.X);
                verts.Add(item.Y);
                verts.Add(item.Z);
            }
            Collada141.source pos = new Collada141.source()
            {
                id = Attach.Name + "_position",
                Item = new Collada141.float_array()
                {
                    id = Attach.Name + "_position_array",
                    count = (ulong)verts.Count,
                    Values = verts.ToArray()
                },
                technique_common = new Collada141.sourceTechnique_common()
                {
                    accessor = new Collada141.accessor()
                    {
                        source = "#" + Attach.Name + "_position_array",
                        count = (ulong)(verts.Count / 3),
                        stride = 3,
                        param = new Collada141.param[] { new Collada141.param() { name = "X", type = "float" }, new Collada141.param() { name = "Y", type = "float" }, new Collada141.param() { name = "Z", type = "float" } }
                    }
                }
            };
            verts = new List<double>();
            foreach (Vertex item in Attach.Normal)
            {
                verts.Add(item.X);
                verts.Add(item.Y);
                verts.Add(item.Z);
            }
            Collada141.source nor = new Collada141.source()
            {
                id = Attach.Name + "_normal",
                Item = new Collada141.float_array()
                {
                    id = Attach.Name + "_normal_array",
                    count = (ulong)verts.Count,
                    Values = verts.ToArray()
                },
                technique_common = new Collada141.sourceTechnique_common()
                {
                    accessor = new Collada141.accessor()
                    {
                        source = "#" + Attach.Name + "_normal_array",
                        count = (ulong)(verts.Count / 3),
                        stride = 3,
                        param = new Collada141.param[] { new Collada141.param() { name = "X", type = "float" }, new Collada141.param() { name = "Y", type = "float" }, new Collada141.param() { name = "Z", type = "float" } }
                    }
                }
            };
            List<Collada141.source> srcs = new List<Collada141.source>() { pos, nor };
            foreach (Mesh mitem in Attach.Mesh)
            {
                if (mitem.UV != null)
                {
                    verts = new List<double>();
                    foreach (UV item in mitem.UV)
                    {
                        verts.Add(item.U);
                        verts.Add(-item.V);
                    }
                    srcs.Add(new Collada141.source()
                    {
                        id = mitem.UVName,
                        Item = new Collada141.float_array()
                        {
                            id = mitem.UVName + "_array",
                            count = (ulong)verts.Count,
                            Values = verts.ToArray()
                        },
                        technique_common = new Collada141.sourceTechnique_common()
                        {
                            accessor = new Collada141.accessor()
                            {
                                source = "#" + mitem.UVName + "_array",
                                count = (ulong)(verts.Count / 2),
                                stride = 2,
                                param = new Collada141.param[] { new Collada141.param() { name = "S", type = "float" }, new Collada141.param() { name = "T", type = "float" } }
                            }
                        }
                    });
                }
            }
            List<Collada141.triangles> tris = new List<Collada141.triangles>();
            foreach (Mesh mesh in Attach.Mesh)
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
                                if (hasUV) inds.Add(currentstriptotal + i);
                            }
                            currentstriptotal += 3;
                            break;
                        case Basic_PolyType.Quads:
                            for (uint i = 0; i < 3; i++)
                            {
                                inds.Add(poly.Indexes[i]);
                                if (hasUV) inds.Add(currentstriptotal + i);
                            }
                            for (uint i = 1; i < 4; i++)
                            {
                                inds.Add(poly.Indexes[i]);
                                if (hasUV) inds.Add(currentstriptotal + i);
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
                                        if (hasUV) inds.Add(currentstriptotal + i);
                                    }
                                }
                                else
                                {
                                    inds.Add(poly.Indexes[k + 1]);
                                    if (hasUV) inds.Add(currentstriptotal + 1);
                                    inds.Add(poly.Indexes[k]);
                                    if (hasUV) inds.Add(currentstriptotal);
                                    inds.Add(poly.Indexes[k + 2]);
                                    if (hasUV) inds.Add(currentstriptotal + 2);
                                }
                                currentstriptotal += 1;
                            }
                            currentstriptotal += 2;
                            break;
                    }
                    string[] indstr = new string[inds.Count];
                    for (int i = 0; i < inds.Count; i++)
                        indstr[i] = inds[i].ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                    List<Collada141.InputLocalOffset> inp = new List<Collada141.InputLocalOffset>()
                    {
                        new Collada141.InputLocalOffset() { semantic = "VERTEX", offset = 0, source = "#" + Attach.Name + "_vertices"},
                    };
                    if (hasUV) inp.Add(new Collada141.InputLocalOffset() { semantic = "TEXCOORD", offset = 1, source = "#" + mesh.UVName, setSpecified = true });
                    tris.Add(new Collada141.triangles()
                    {
                        material = "material_" +  Attach.Name + "_" + mesh.MaterialID,
                        count = (ulong)(inds.Count / (hasUV ? 6 : 3)),
                        input = inp.ToArray(),
                        p = string.Join(" ", indstr)
                    });
                }
            }
            geometries.Add(new Collada141.geometry()
            {
                id = Attach.Name,
                name = Attach.Name,
                Item = new Collada141.mesh()
                {
                    source = srcs.ToArray(),
                    vertices = new Collada141.vertices()
                    {
                        id = Attach.Name + "_vertices",
                        input = new Collada141.InputLocal[]
                        {
                            new Collada141.InputLocal()
                            {
                                semantic = "POSITION",
                                source = "#" + Attach.Name + "_position"
                            },
                            new Collada141.InputLocal()
                            {
                                semantic = "NORMAL",
                                source = "#" + Attach.Name + "_normal"
                            }
                        }
                    },
                    Items = tris.ToArray()
                }
            });
        skipAttach:
            Collada141.node node = new Collada141.node()
            {
                id = Name,
                name = Name,
                Items = new object[]
                {
                    new Collada141.TargetableFloat3() { sid = "translate", Values = new double[] { Position.X, Position.Y, Position.Z } },
                    new Collada141.rotate() { sid = "rotateZ", Values = new double[] { 0, 0, 1, Rotation.ZDeg } },
                    new Collada141.rotate() { sid = "rotateX", Values = new double[] { 1, 0, 0, Rotation.XDeg } },
                    new Collada141.rotate() { sid = "rotateY", Values = new double[] { 0, 1, 0, Rotation.YDeg } },
                    new Collada141.TargetableFloat3() { sid = "scale", Values = new double[] { Scale.X, Scale.Y, Scale.Z } }
                },
                ItemsElementName = new Collada141.ItemsChoiceType2[]
                {
                    Collada141.ItemsChoiceType2.translate,
                    Collada141.ItemsChoiceType2.rotate,
                    Collada141.ItemsChoiceType2.rotate,
                    Collada141.ItemsChoiceType2.rotate,
                    Collada141.ItemsChoiceType2.scale
                }
            };
            if (Attach != null)
            {
                List<Collada141.instance_material> mats = new List<Collada141.instance_material>();
                foreach (Mesh item in Attach.Mesh)
                    mats.Add(new Collada141.instance_material() { symbol = "material_" + Attach.Name + "_" + item.MaterialID, target = "#" + "material_" + Attach.Name + "_" + item.MaterialID });
                node.instance_geometry = new Collada141.instance_geometry[]
                {
                    new Collada141.instance_geometry()
                    {
                        url = "#" + Attach.Name,
                        bind_material = new Collada141.bind_material() { technique_common = mats.ToArray() }
                    }
                };
            }
            List<Collada141.node> childnodes = new List<Collada141.node>();
            foreach (Object item in Children)
                childnodes.Add(item.AddToCollada(materials, effects, geometries, visitedAttaches, hasTextures));
            node.node1 = childnodes.ToArray();
            return node;
        }

        public Object ToBasicModel()
        {
            List<Object> newchildren = new List<Object>(Children.Count);
            foreach (Object item in Children)
                newchildren.Add(item.ToBasicModel());
            Object result = new Object();
            result.Flags = Flags;
            if (Attach != null)
                result.Attach = Attach.ToBasicModel();
            result.Position = Position;
            result.Rotation = Rotation;
            result.Scale = Scale;
            result.Children = newchildren;
            return result;
        }

        public Object ToChunkModel()
        {
            List<Object> newchildren = new List<Object>(Children.Count);
            foreach (Object item in Children)
                newchildren.Add(item.ToBasicModel());
            Object result = new Object();
            result.Flags = Flags;
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
            System.Text.StringBuilder result = new System.Text.StringBuilder("{ ");
            result.Append(((StructEnums.NJD_EVAL)Flags).ToString().Replace(", ", " | "));
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

        public string ToStructVariables(bool DX, List<string> labels)
        {
            for (int i = 1; i < Children.Count; i++)
                Children[i - 1].Sibling = Children[i];
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int i = Children.Count - 1; i >= 0; i--)
                if (!labels.Contains(Children[i].Name))
                {
                    labels.Add(Children[i].Name);
                    result.AppendLine(Children[i].ToStructVariables(DX, labels));
                }
            if (Attach != null && !labels.Contains(Attach.Name))
            {
                labels.Add(Attach.Name);
                result.AppendLine(Attach.ToStructVariables(DX, labels));
            }
            result.Append("NJS_OBJECT ");
            result.Append(Name);
            result.Append(" = ");
            result.Append(ToStruct());
            result.AppendLine(";");
            return result.ToString();
        }

        static readonly Random rand = new Random();

        internal static string GenerateIdentifier()
        {
            return DateTime.Now.Ticks.ToString("X") + rand.Next(0, 65536).ToString("X4");
        }
    }
}