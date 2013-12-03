using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class BasicAttach : Attach
    {
        public Vertex[] Vertex { get; private set; }
        public string VertexName { get; set; }
        public Vertex[] Normal { get; private set; }
        public string NormalName { get; set; }
        public List<Mesh> Mesh { get; set; }
        public string MeshName { get; set; }
        public List<Material> Material { get; set; }
        public string MaterialName { get; set; }

        public BasicAttach()
        {
            Name = "attach_" + Object.GenerateIdentifier();
            Bounds = new BoundingSphere();
            Material = new List<Material>();
            MaterialName = "matlist_" + Object.GenerateIdentifier();
            Mesh = new List<Mesh>();
            MeshName = "meshlist_" + Object.GenerateIdentifier();
            Vertex = new Vertex[0];
            VertexName = "vertex_" + Object.GenerateIdentifier();
            Normal = new Vertex[0];
            NormalName = "normal_" + Object.GenerateIdentifier();
        }

        public BasicAttach(byte[] file, int address, uint imageBase, bool DX)
            : this(file, address, imageBase, DX, new Dictionary<int, string>()) { }

        public BasicAttach(byte[] file, int address, uint imageBase, bool DX, Dictionary<int, string> labels)
            : this()
        {
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "attach_" + address.ToString("X8");
            Vertex = new Vertex[ByteConverter.ToInt32(file, address + 8)];
            Normal = new Vertex[Vertex.Length];
            int tmpaddr = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
            if (labels.ContainsKey(tmpaddr))
                VertexName = labels[tmpaddr];
            else
                VertexName = "vertex_" + tmpaddr.ToString("X8");
            for (int i = 0; i < Vertex.Length; i++)
            {
                Vertex[i] = new Vertex(file, tmpaddr);
                tmpaddr += SAModel.Vertex.Size;
            }
            tmpaddr = ByteConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)((uint)tmpaddr - imageBase);
                if (labels.ContainsKey(tmpaddr))
                    NormalName = labels[tmpaddr];
                else
                    NormalName = "normal_" + tmpaddr.ToString("X8");
                for (int i = 0; i < Vertex.Length; i++)
                {
                    Normal[i] = new Vertex(file, tmpaddr);
                    tmpaddr += SAModel.Vertex.Size;
                }
            }
            else
                for (int i = 0; i < Vertex.Length; i++)
                    Normal[i] = new Vertex(0, 1, 0);
            int meshcnt = ByteConverter.ToInt16(file, address + 0x14);
            tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                if (labels.ContainsKey(tmpaddr))
                    MeshName = labels[tmpaddr];
                else
                    MeshName = "meshlist_" + tmpaddr.ToString("X8");
                for (int i = 0; i < meshcnt; i++)
                {
                    Mesh.Add(new Mesh(file, tmpaddr, imageBase, labels));
                    tmpaddr += SAModel.Mesh.Size(DX);
                }
            }
            int matcnt = ByteConverter.ToInt16(file, address + 0x16);
            tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                if (labels.ContainsKey(tmpaddr))
                    MaterialName = labels[tmpaddr];
                else
                    MaterialName = "matlist_" + tmpaddr.ToString("X8");
                for (int i = 0; i < matcnt; i++)
                {
                    Material.Add(new Material(file, tmpaddr, labels));
                    tmpaddr += SAModel.Material.Size;
                }
            }
            Bounds = new BoundingSphere(file, address + 0x18);
        }

        public BasicAttach(Vertex[] vertex, Vertex[] normal, IEnumerable<Mesh> mesh, IEnumerable<Material> material)
            : this()
        {
            Vertex = vertex;
            Normal = normal;
            Mesh = new List<Mesh>(mesh);
            Material = new List<Material>(material);
        }

        public override byte[] GetBytes(uint imageBase, bool DX, Dictionary<string, uint> labels, out uint address)
        {
            List<byte> result = new List<byte>();
            uint materialAddress = 0;
            if (Material != null && Material.Count > 0)
            {
                if (labels.ContainsKey(MaterialName))
                    materialAddress = labels[MaterialName];
                else
                {
                    materialAddress = imageBase;
                    labels.Add(MaterialName, materialAddress);
                    foreach (Material item in Material)
                        result.AddRange(item.GetBytes());
                }
            }
            uint meshAddress = 0;
            if (labels.ContainsKey(MeshName))
                meshAddress = labels[MeshName];
            else
            {
                uint[] polyAddrs = new uint[Mesh.Count];
                uint[] polyNormalAddrs = new uint[Mesh.Count];
                uint[] vColorAddrs = new uint[Mesh.Count];
                uint[] uVAddrs = new uint[Mesh.Count];
                for (int i = 0; i < Mesh.Count; i++)
                    if (labels.ContainsKey(Mesh[i].PolyName))
                        polyAddrs[i] = labels[Mesh[i].PolyName];
                    else
                    {
                        result.Align(4);
                        polyAddrs[i] = (uint)result.Count + imageBase;
                        labels.Add(Mesh[i].PolyName, polyAddrs[i]);
                        for (int j = 0; j < Mesh[i].Poly.Count; j++)
                            result.AddRange(Mesh[i].Poly[j].GetBytes());
                    }
                for (int i = 0; i < Mesh.Count; i++)
                    if (Mesh[i].PolyNormal != null)
                    {
                        if (labels.ContainsKey(Mesh[i].PolyNormalName))
                            polyNormalAddrs[i] = labels[Mesh[i].PolyNormalName];
                        else
                        {
                            result.Align(4);
                            polyNormalAddrs[i] = (uint)result.Count + imageBase;
                            labels.Add(Mesh[i].PolyNormalName, polyNormalAddrs[i]);
                            for (int j = 0; j < Mesh[i].PolyNormal.Length; j++)
                                result.AddRange(Mesh[i].PolyNormal[j].GetBytes());
                        }
                    }
                for (int i = 0; i < Mesh.Count; i++)
                    if (Mesh[i].VColor != null)
                    {
                        if (labels.ContainsKey(Mesh[i].VColorName))
                            vColorAddrs[i] = labels[Mesh[i].VColorName];
                        else
                        {
                            result.Align(4);
                            vColorAddrs[i] = (uint)result.Count + imageBase;
                            labels.Add(Mesh[i].VColorName, vColorAddrs[i]);
                            for (int j = 0; j < Mesh[i].VColor.Length; j++)
                                result.AddRange(VColor.GetBytes(Mesh[i].VColor[j]));
                        }
                    }
                for (int i = 0; i < Mesh.Count; i++)
                    if (Mesh[i].UV != null)
                    {
                        if (labels.ContainsKey(Mesh[i].UVName))
                            uVAddrs[i] = labels[Mesh[i].UVName];
                        else
                        {
                            result.Align(4);
                            uVAddrs[i] = (uint)result.Count + imageBase;
                            labels.Add(Mesh[i].UVName, uVAddrs[i]);
                            for (int j = 0; j < Mesh[i].UV.Length; j++)
                                result.AddRange(Mesh[i].UV[j].GetBytes());
                        }
                    }
                result.Align(4);
                meshAddress = (uint)result.Count + imageBase;
                labels.Add(MeshName, meshAddress);
                for (int i = 0; i < Mesh.Count; i++)
                    result.AddRange(Mesh[i].GetBytes(polyAddrs[i], polyNormalAddrs[i], vColorAddrs[i], uVAddrs[i], DX));
            }
            result.Align(4);
            uint vertexAddress;
            if (labels.ContainsKey(VertexName))
                vertexAddress = labels[VertexName];
            else
            {
                vertexAddress = (uint)result.Count + imageBase;
                labels.Add(VertexName, vertexAddress);
                foreach (Vertex item in Vertex)
                    if (item == null)
                        result.AddRange(new byte[SAModel.Vertex.Size]);
                    else
                        result.AddRange(item.GetBytes());
            }
            result.Align(4);
            uint normalAddress;
            if (labels.ContainsKey(NormalName))
                normalAddress = labels[NormalName];
            else
            {
                normalAddress = (uint)result.Count + imageBase;
                labels.Add(NormalName, normalAddress);
                foreach (Vertex item in Normal)
                    if (item == null)
                        result.AddRange(new byte[SAModel.Vertex.Size]);
                    else
                        result.AddRange(item.GetBytes());
            }
            result.Align(4);
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes(vertexAddress));
            result.AddRange(ByteConverter.GetBytes(normalAddress));
            result.AddRange(ByteConverter.GetBytes(Vertex.Length));
            result.AddRange(ByteConverter.GetBytes(meshAddress));
            result.AddRange(ByteConverter.GetBytes(materialAddress));
            result.AddRange(ByteConverter.GetBytes((short)Mesh.Count));
            result.AddRange(ByteConverter.GetBytes((short)Material.Count));
            result.AddRange(Bounds.GetBytes());
            if (DX) result.AddRange(new byte[4]);
            labels.Add(Name, address + imageBase);
            return result.ToArray();
        }

        public override string ToStruct(bool DX)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder("{ ");
            result.Append(Vertex != null ? VertexName : "NULL");
            result.Append(", ");
            result.Append(Normal != null ? NormalName : "NULL");
            result.Append(", ");
            result.Append(Vertex != null ? "LengthOfArray(" + VertexName + ")" : "0");
            result.Append(", ");
            result.Append(Mesh != null? MeshName : "NULL");
            result.Append(", ");
            result.Append(Material != null && Material.Count > 0 ? MaterialName : "NULL");
            result.Append(", ");
            result.Append(Mesh != null? "LengthOfArray(" + MeshName + ")" : "0");
            result.Append(", ");
            result.Append(Material != null && Material.Count > 0 ? "LengthOfArray(" + MaterialName + ")" : "0");
            result.Append(", ");
            result.Append(Bounds.ToStruct());
            if (DX) result.Append(", NULL");
            result.Append(" }");
            return result.ToString();
        }

        public override string ToStructVariables(bool DX, List<string> labels)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            if (Material != null && Material.Count > 0 && !labels.Contains(MaterialName))
            {
                labels.Add(MaterialName);
                result.Append("NJS_MATERIAL ");
                result.Append(MaterialName);
                result.AppendLine("[] = {");
                List<string> mtls = new List<string>(Material.Count);
                foreach (Material item in Material)
                    mtls.Add(item.ToStruct());
                result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", mtls.ToArray()));
                result.AppendLine("};");
                result.AppendLine();
            }
            if (!labels.Contains(MeshName))
            {
                for (int i = 0; i < Mesh.Count; i++)
                    if (!labels.Contains(Mesh[i].PolyName))
                    {
                        labels.Add(Mesh[i].PolyName);
                        result.Append("Sint16 ");
                        result.Append(Mesh[i].PolyName);
                        result.AppendLine("[] = {");
                        List<string> plys = new List<string>(Mesh[i].Poly.Count);
                        foreach (Poly item in Mesh[i].Poly)
                            plys.Add(item.ToStruct());
                        result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", plys.ToArray()));
                        result.AppendLine("};");
                        result.AppendLine();
                    }
                for (int i = 0; i < Mesh.Count; i++)
                    if (Mesh[i].PolyNormal != null && !labels.Contains(Mesh[i].PolyNormalName))
                    {
                        labels.Add(Mesh[i].PolyNormalName);
                        result.Append("NJS_VECTOR ");
                        result.Append(Mesh[i].PolyNormalName);
                        result.AppendLine("[] = {");
                        List<string> plys = new List<string>(Mesh[i].PolyNormal.Length);
                        foreach (Vertex item in Mesh[i].PolyNormal)
                            plys.Add(item.ToStruct());
                        result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", plys.ToArray()));
                        result.AppendLine("};");
                        result.AppendLine();
                    }
                for (int i = 0; i < Mesh.Count; i++)
                    if (Mesh[i].VColor != null && !labels.Contains(Mesh[i].VColorName))
                    {
                        labels.Add(Mesh[i].VColorName);
                        result.Append("NJS_COLOR ");
                        result.Append(Mesh[i].VColorName);
                        result.AppendLine("[] = {");
                        List<string> vcs = new List<string>(Mesh[i].VColor.Length);
                        foreach (Color item in Mesh[i].VColor)
                            vcs.Add(item.ToStruct());
                        result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", vcs.ToArray()));
                        result.AppendLine("};");
                        result.AppendLine();
                    }
                for (int i = 0; i < Mesh.Count; i++)
                    if (Mesh[i].UV != null && !labels.Contains(Mesh[i].UVName))
                    {
                        labels.Add(Mesh[i].UVName);
                        result.Append("NJS_TEX ");
                        result.Append(Mesh[i].UVName);
                        result.AppendLine("[] = {");
                        List<string> uvs = new List<string>(Mesh[i].UV.Length);
                        foreach (UV item in Mesh[i].UV)
                            uvs.Add(item.ToStruct());
                        result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", uvs.ToArray()));
                        result.AppendLine("};");
                        result.AppendLine();
                    }
                labels.Add(MeshName);
                result.Append("NJS_MESHSET");
                if (DX) result.Append("_SADX");
                result.Append(" ");
                result.Append(MeshName);
                result.AppendLine("[] = {");
                List<string> mshs = new List<string>(Mesh.Count);
                foreach (Mesh item in Mesh)
                    mshs.Add(item.ToStruct(DX));
                result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", mshs.ToArray()));
                result.AppendLine("};");
                result.AppendLine();
            }
            if (!labels.Contains(VertexName))
            {
                labels.Add(VertexName);
                result.Append("NJS_VECTOR ");
                result.Append(VertexName);
                result.AppendLine("[] = {");
                List<string> vtxs = new List<string>(Vertex.Length);
                foreach (Vertex item in Vertex)
                    vtxs.Add(item != null ? item.ToStruct() : "{ 0 }");
                result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", vtxs.ToArray()));
                result.AppendLine("};");
                result.AppendLine();
            }
            if (Normal != null && !labels.Contains(NormalName))
            {
                labels.Add(VertexName);
                result.Append("NJS_VECTOR ");
                result.Append(NormalName);
                result.AppendLine("[] = {");
                List<string> vtxs = new List<string>(Normal.Length);
                foreach (Vertex item in Normal)
                    vtxs.Add(item != null ? item.ToStruct() : "{ 0 }");
                result.AppendLine("\t" + string.Join("," + Environment.NewLine + "\t", vtxs.ToArray()));
                result.AppendLine("};");
                result.AppendLine();
            }
            result.Append("NJS_MODEL");
            if (DX) result.Append("_SADX");
            result.Append(" ");
            result.Append(Name);
            result.Append(" = ");
            result.Append(ToStruct(DX));
            result.AppendLine(";");
            return result.ToString();
        }
        
        public override void ProcessVertexData()
        {
            List<MeshInfo> result = new List<MeshInfo>();
            foreach (Mesh mesh in Mesh)
            {
                bool hasVColor = mesh.VColor != null;
                bool hasUV = mesh.UV != null;
                List<VertexData> verts = new List<VertexData>();
                int currentstriptotal = 0;
                foreach (Poly poly in mesh.Poly)
                {
                    switch (mesh.PolyType)
                    {
                        case Basic_PolyType.Triangles:
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[0]],
                                Normal[poly.Indexes[0]],
                                hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[1]],
                                Normal[poly.Indexes[1]],
                                hasVColor ? mesh.VColor[currentstriptotal + 1] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 1] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[2]],
                                Normal[poly.Indexes[2]],
                                hasVColor ? mesh.VColor[currentstriptotal + 2] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 2] : new UV()));
                            currentstriptotal += 3;
                            break;
                        case Basic_PolyType.Quads:
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[0]],
                                Normal[poly.Indexes[0]],
                                hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[1]],
                                Normal[poly.Indexes[1]],
                                hasVColor ? mesh.VColor[currentstriptotal + 1] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 1] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[2]],
                                Normal[poly.Indexes[2]],
                                hasVColor ? mesh.VColor[currentstriptotal + 2] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 2] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[1]],
                                Normal[poly.Indexes[1]],
                                hasVColor ? mesh.VColor[currentstriptotal + 1] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 1] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[2]],
                                Normal[poly.Indexes[2]],
                                hasVColor ? mesh.VColor[currentstriptotal + 2] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 2] : new UV()));
                            verts.Add(new VertexData(
                                Vertex[poly.Indexes[3]],
                                Normal[poly.Indexes[3]],
                                hasVColor ? mesh.VColor[currentstriptotal + 3] : Color.White,
                                hasUV ? mesh.UV[currentstriptotal + 3] : new UV()));
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
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k]],
                                        Normal[poly.Indexes[k]],
                                        hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal] : new UV()));
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k + 1]],
                                        Normal[poly.Indexes[k + 1]],
                                        hasVColor ? mesh.VColor[currentstriptotal + 1] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal + 1] : new UV()));
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k + 2]],
                                        Normal[poly.Indexes[k + 2]],
                                        hasVColor ? mesh.VColor[currentstriptotal + 2] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal + 2] : new UV()));
                                }
                                else
                                {
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k + 1]],
                                        Normal[poly.Indexes[k + 1]],
                                        hasVColor ? mesh.VColor[currentstriptotal + 1] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal + 1] : new UV()));
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k]],
                                        Normal[poly.Indexes[k]],
                                        hasVColor ? mesh.VColor[currentstriptotal] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal] : new UV()));
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k + 2]],
                                        Normal[poly.Indexes[k + 2]],
                                        hasVColor ? mesh.VColor[currentstriptotal + 2] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal + 2] : new UV()));
                                }
                                currentstriptotal += 1;
                            }
                            currentstriptotal += 2;
                            break;
                    }
                }
                Material mat = null;
                if (Material != null && mesh.MaterialID < Material.Count)
                    mat = Material[mesh.MaterialID];
                result.Add(new MeshInfo(mat, verts.ToArray()));
            }
            MeshInfo = result.ToArray();
        }

        public void ResizeVertexes(int newSize)
        {
            Vertex[] vert = Vertex;
            Array.Resize(ref vert, newSize);
            Vertex = vert;
            vert = Normal;
            Array.Resize(ref vert, newSize);
            Normal = vert;
        }

        public override BasicAttach ToBasicModel()
        {
            return this;
        }

        public override ChunkAttach ToChunkModel()
        {
            throw new NotImplementedException(); // TODO
        }
    }
}