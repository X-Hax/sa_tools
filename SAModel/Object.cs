using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class Object
    {
        public ObjectFlags Flags { get; set; }
        public Attach Attach { get; set; }
        public Vertex Position { get; set; }
        public Rotation Rotation { get; set; }
        public Vertex Scale { get; set; }
        public List<Object> Children { get; set; }
        internal Object Sibling { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 0x34; } }

        public Object()
        {
            Name = "object_" + DateTime.Now.Ticks.ToString("X") + rand.Next(0, 256).ToString("X2");
            Position = new Vertex();
            Rotation = new Rotation();
            Scale = new Vertex(1, 1, 1);
        }

        public Object(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Name = "object_" + address.ToString("X8");
            Flags = (ObjectFlags)ByteConverter.ToInt32(file, address);
            int tmpaddr = ByteConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Attach = new Attach(file, tmpaddr, imageBase, format);
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
                child = new Object(file, tmpaddr, imageBase, format);
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
                Sibling = new Object(file, tmpaddr, imageBase, format);
            }
        }

        public Object(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Dictionary<string, string> group = INI[groupname];
            Name = groupname;
            Flags = (ObjectFlags)Enum.Parse(typeof(ObjectFlags), group["Flags"]);
            if (group.ContainsKey("Attach"))
                Attach = new Attach(INI, group["Attach"]);
            Position = new Vertex(group["Position"]);
            Rotation = new Rotation(group["Rotation"]);
            Scale = new Vertex(group["Scale"]);
            Children = new List<Object>();
            if (group.ContainsKey("Children"))
            {
                string[] chldrn = group["Children"].Split(',');
                foreach (string item in chldrn)
                    Children.Add(new Object(INI, item));
            }
        }

        public static Object LoadFromFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C444D314153u)
                return new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA1);
            else if (magic == 0x00004C444D324153u)
                return new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA2);
            else
                throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
        }

        public static bool CheckModelFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C444D314153u)
                return true;
            else if (magic == 0x00004C444D324153u)
                return true;
            else
                return false;
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, Dictionary<string, uint> attachaddrs, out uint address)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            for (int i = 1; i < Children.Count; i++)
                Children[i - 1].Sibling = Children[i];
            List<byte> result = new List<byte>();
            uint childaddr = 0;
            uint siblingaddr = 0;
            uint attachaddr = 0;
            byte[] tmpbyte;
            if (Children.Count > 0)
            {
                result.Align(4);
                result.AddRange(Children[0].GetBytes(imageBase, format, out childaddr));
                childaddr += imageBase;
            }
            if (Sibling != null)
            {
                result.Align(4);
                tmpbyte = Sibling.GetBytes(imageBase + (uint)result.Count, format, out siblingaddr);
                siblingaddr += imageBase + (uint)result.Count;
                result.AddRange(tmpbyte);
            }
            if (Attach != null)
            {
                if (attachaddrs.ContainsKey(Attach.Name))
                    attachaddr = attachaddrs[Attach.Name];
                else
                {
                    result.Align(4);
                    tmpbyte = Attach.GetBytes(imageBase + (uint)result.Count, format, out attachaddr);
                    attachaddr += imageBase + (uint)result.Count;
                    result.AddRange(tmpbyte);
                    attachaddrs.Add(Attach.Name, attachaddr);
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
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, out uint address)
        {
            return GetBytes(imageBase, format, new Dictionary<string, uint>(), out address);
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format)
        {
            uint address;
            return GetBytes(imageBase, format, out address);
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Flags", Flags.ToString());
            if (Attach != null)
            {
                group.Add("Attach", Attach.Name);
                Attach.Save(INI);
            }
            group.Add("Position", Position.ToString());
            group.Add("Rotation", Rotation.ToString());
            group.Add("Scale", Scale.ToString());
            if (Children.Count > 0)
            {
                List<string> chldrn = new List<string>();
                foreach (Object child in Children)
                {
                    chldrn.Add(child.Name);
                    child.Save(INI);
                }
                group.Add("Children", string.Join(",", chldrn.ToArray()));
            }
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }

        public void SaveToFile(string filename, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            switch (format)
            {
                case ModelFormat.SA1:
                    file.AddRange(ByteConverter.GetBytes(0x00004C444D314153u));
                    uint addr = 0;
                    byte[] mdl = GetBytes(0x10, ModelFormat.SA1, out addr);
                    file.AddRange(ByteConverter.GetBytes(addr + 0x10));
                    file.Align(0x10);
                    file.AddRange(mdl);
                    break;
                case ModelFormat.SADX:
                    throw new ArgumentException("Cannot save SADX format models to file!", "format");
                case ModelFormat.SA2:
                    file.AddRange(ByteConverter.GetBytes(0x00004C444D314153u));
                    addr = 0;
                    mdl = GetBytes(0x10, ModelFormat.SA2, out addr);
                    file.AddRange(ByteConverter.GetBytes(addr + 0x10));
                    file.Align(0x10);
                    file.AddRange(mdl);
                    break;
                case ModelFormat.SA2B:
                    throw new ArgumentException("Cannot save SA2B format levels to file!", "format");
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }

        public Object[] GetObjects()
        {
            List<Object> result = new List<Object>() { this };
            foreach (Object item in Children)
                result.AddRange(item.GetObjects());
            return result.ToArray();
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
            if (Attach == null || visitedAttaches.Contains(Attach.Name))
                goto skipAttach;
            visitedAttaches.Add(Attach.Name);
            foreach (Material item in Attach.Material)
            {
                materials.Add(new Collada141.material()
                {
                    id = item.Name,
                    name = item.Name,
                    instance_effect = new Collada141.instance_effect()
                    {
                        url = "#" + item.Name + "_eff"
                    }
                });
                if (hasTextures)
                {
                    effects.Add(new Collada141.effect()
                    {
                        id = item.Name + "_eff",
                        name = item.Name + "_eff",
                        Items = new Collada141.effectFx_profile_abstractProfile_COMMON[]
                        {
                            new Collada141.effectFx_profile_abstractProfile_COMMON()
                            {
                                technique = new Collada141.effectFx_profile_abstractProfile_COMMONTechnique()
                                {
                                    sid = "standard",
                                    Item = new Collada141.effectFx_profile_abstractProfile_COMMONTechniquePhong()
                                    {
                                        ambient = new Collada141.common_color_or_texture_type()
                                        {
                                            Item = new Collada141.common_color_or_texture_typeTexture()
                                            {
                                                texture = "image_" + (item.TextureID + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo),
                                                texcoord = "CHANNEL0"
                                            }
                                        },
                                        diffuse = new Collada141.common_color_or_texture_type()
                                        {
                                            Item = new Collada141.common_color_or_texture_typeColor()
                                            {
                                                Values = new double[] { item.DiffuseColor.R / 255d, item.DiffuseColor.G / 255d, item.DiffuseColor.B / 255d, item.DiffuseColor.A / 255d }
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
                        id = item.Name + "_eff",
                        name = item.Name + "_eff",
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
                                                Values = new double[] { item.DiffuseColor.R / 255d, item.DiffuseColor.G / 255d, item.DiffuseColor.B / 255d, item.DiffuseColor.A / 255d }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
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
                        param = new Collada141.param[] { new Collada141.param() { name = "X", type = "float" }, new Collada141.param() { name = "Y", type = "float" }, new Collada141.param() { name = "Y", type = "float" } }
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
                        param = new Collada141.param[] { new Collada141.param() { name = "X", type = "float" }, new Collada141.param() { name = "Y", type = "float" }, new Collada141.param() { name = "Y", type = "float" } }
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
                        verts.Add(item.U / 255d);
                        verts.Add(item.V / 255d);
                    }
                    srcs.Add(new Collada141.source()
                    {
                        id = mitem.Name + "_uv",
                        Item = new Collada141.float_array()
                        {
                            id = mitem.Name + "_uv_array",
                            count = (ulong)verts.Count,
                            Values = verts.ToArray()
                        },
                        technique_common = new Collada141.sourceTechnique_common()
                        {
                            accessor = new Collada141.accessor()
                            {
                                source = "#" + mitem.Name + "_uv_array",
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
                        case PolyType.Triangles:
                            for (uint i = 0; i < 3; i++)
                            {
                                inds.Add(poly.Indexes[i]);
                                if (hasUV) inds.Add(currentstriptotal + i);
                            }
                            currentstriptotal += 3;
                            break;
                        case PolyType.Quads:
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
                        case PolyType.Strips:
                        case PolyType.Strips2:
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
                    if (hasUV) inp.Add(new Collada141.InputLocalOffset() { semantic = "TEXCOORD", offset = 1, source = "#" + mesh.Name + "_uv", setSpecified = true });
                    tris.Add(new Collada141.triangles()
                    {
                        material = Attach.Material[mesh.MaterialID].Name,
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
                    new Collada141.rotate() { sid = "rotateZ", Values = new double[] { 0, 0, 1, Rotation.Z / 65536d } },
                    new Collada141.rotate() { sid = "rotateX", Values = new double[] { 1, 0, 0, Rotation.X / 65536d } },
                    new Collada141.rotate() { sid = "rotateY", Values = new double[] { 0, 1, 0, Rotation.Y / 65536d } },
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
                    mats.Add(new Collada141.instance_material() { symbol = Attach.Material[item.MaterialID].Name, target = "#" + Attach.Material[item.MaterialID].Name });
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

        internal static Random rand = new Random();
    }
}