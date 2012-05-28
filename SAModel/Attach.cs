using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class Attach
    {
        public string Name { get; set; }
        public Vertex[] Vertex { get; private set; }
        public string VertexName { get; set; }
        public Vertex[] Normal { get; private set; }
        public string NormalName { get; set; }
        public List<Mesh> Mesh { get; set; }
        public string MeshName { get; set; }
        public List<Material> Material { get; set; }
        public string MaterialName { get; set; }
        public BoundingSphere Bounds { get; set; }

        public static int Size(ModelFormat format)
        {
            switch (format)
            {
                case ModelFormat.SA1:
                    return 0x28;
                case ModelFormat.SADX:
                    return 0x2C;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    return 0x18;
            }
            return -1;
        }

        public Attach()
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

        public Attach(byte[] file, int address, uint imageBase, ModelFormat format)
            : this(file, address, imageBase, format, new Dictionary<int, string>()) { }

        public Attach(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
            : this(file, address, imageBase, format, labels, false) { }

        public Attach(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels, bool forceBasic)
            : this()
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "attach_" + address.ToString("X8");
            switch (forceBasic ? ModelFormat.SA1 : format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
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
                            tmpaddr += SAModel.Mesh.Size(format == ModelFormat.SADX);
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
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    Vertex = new Vertex[0];
                    Normal = new Vertex[0];
                    byte ctype;
                    tmpaddr = ByteConverter.ToInt32(file, address);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        uint header = ByteConverter.ToUInt32(file, tmpaddr);
                        ctype = (byte)(header & 0xFF);
                        while (ctype == 0x00)
                        {
                            tmpaddr += 4;
                            header = ByteConverter.ToUInt32(file, tmpaddr);
                            ctype = (byte)(header & 0xFF);
                        }
                        if (ctype < 0x20 | ctype > 0x36)
                        {
                            System.Diagnostics.Debug.WriteLine("Unrecognized chunk 0x" + ctype.ToString("X2") + " at 0x" + tmpaddr.ToString("X8") + ".");
                            goto poly;
                        }
                        while (ctype != 0xFF)
                        {
                            uint inf = ByteConverter.ToUInt32(file, tmpaddr + 4);
                            int curvert = (int)(inf & 0xFFFF);
                            int vcnt = (int)(inf >> 16);
                            ResizeVertexes(Math.Max(Vertex.Length, curvert + vcnt));
                            tmpaddr += 8;
                            for (int i = curvert; i < curvert + vcnt; i++)
                            {
                                switch (ctype)
                                {
                                    case 0x20:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x21:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0x10);
                                        tmpaddr += 0x20;
                                        break;
                                    case 0x22:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0xC;
                                        break;
                                    case 0x23:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x24:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x25:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x26:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x27:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x28:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x29:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x18;
                                        break;
                                    case 0x2A:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x2B:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x2C:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x2D:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x2E:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x2F:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(file, tmpaddr + 0xC);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x30:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x10;
                                        break;
                                    case 0x31:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x18;
                                        break;
                                    case 0x32:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x33:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x34:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x35:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x1C;
                                        break;
                                    case 0x36:
                                        Vertex[i] = new Vertex(file, tmpaddr);
                                        Normal[i] = new Vertex(0, 1, 0);
                                        tmpaddr += 0x1C;
                                        break;
                                }
                            }
                            header = ByteConverter.ToUInt32(file, tmpaddr);
                            ctype = (byte)(header & 0xFF);
                            while (ctype == 0x00)
                            {
                                tmpaddr += 4;
                                header = ByteConverter.ToUInt32(file, tmpaddr);
                                ctype = (byte)(header & 0xFF);
                            }
                            if (ctype < 0x20 | ctype > 0x36)
                            {
                                System.Diagnostics.Debug.WriteLine("Unrecognized chunk 0x" + ctype.ToString("X2") + " at 0x" + tmpaddr.ToString("X8") + ".");
                                goto poly;
                            }
                        }
                    }
            poly:   Material mat = new Material();
                    tmpaddr = ByteConverter.ToInt32(file, address + 4);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        ushort header = ByteConverter.ToUInt16(file, tmpaddr);
                        ctype = (byte)(header & 0xFF);
                        while (ctype != 0xFF)
                        {
                            switch (ctype)
                            {
                                case 0x08:
                                case 0x09:
                                    mat.TextureID = ByteConverter.ToUInt16(file, tmpaddr + 2) & 0x1FFF;
                                    tmpaddr += 4;
                                    break;
                                case 0x10:
                                case 0x11:
                                case 0x12:
                                case 0x13:
                                case 0x14:
                                case 0x15:
                                case 0x16:
                                case 0x17:
                                    bool hasDiffuse = (ctype & 1) != 0;
                                    int curaddr = tmpaddr + 4;
                                    if (hasDiffuse)
                                    {
                                        mat.DiffuseColor = Color.FromArgb(ByteConverter.ToInt32(file, curaddr));
                                        curaddr += 4;
                                    }
                                    bool hasAmbient = (ctype & 2) != 0;
                                    if (hasAmbient)
                                        curaddr += 4;
                                    bool hasSpecular = (ctype & 4) != 0;
                                    if (hasSpecular)
                                    {
                                        mat.SpecularColor = Color.FromArgb(ByteConverter.ToInt32(file, curaddr));
                                        curaddr += 4;
                                    }
                                    tmpaddr = curaddr;
                                    break;
                                case 0x38:
                                    tmpaddr += ByteConverter.ToInt16(file, tmpaddr + 2) + 4;
                                    break;
                                case 0x40:
                                case 0x41:
                                case 0x42:
                                case 0x43:
                                case 0x44:
                                case 0x45:
                                case 0x46:
                                case 0x47:
                                case 0x48:
                                    int matnum = Material.Count;
                                    Material.Add(mat);
                                    mat = new Material() { TextureID = mat.TextureID, DiffuseColor = mat.DiffuseColor, SpecularColor = mat.SpecularColor };
                                    int striptype = header & 0xF;
                                    int numflags = (header >> 8) & 3;
                                    Poly[] polys = new Poly[ByteConverter.ToUInt16(file, tmpaddr + 4) & 0x3FFF];
                                    List<UV> uvs = new List<UV>();
                                    List<Color> vcs = new List<Color>();
                                    tmpaddr += 6;
                                    for (int i = 0; i < polys.Length; i++)
                                    {
                                        int stripcnt = ByteConverter.ToInt16(file, tmpaddr);
                                        tmpaddr += 2;
                                        polys[i] = new Strip(Math.Abs(stripcnt), stripcnt < 0);
                                        for (int j = 0; j < polys[i].Indexes.Length; j++)
                                        {
                                            polys[i].Indexes[j] = ByteConverter.ToUInt16(file, tmpaddr);
                                            tmpaddr += 2;
                                            switch (striptype)
                                            {
                                                case 1:
                                                    uvs.Add(new UV(file, tmpaddr));
                                                    tmpaddr += UV.Size;
                                                    break;
                                                case 2:
                                                    uvs.Add(new UV() { U = (short)(ByteConverter.ToInt16(file, tmpaddr) / 4), V = (short)(ByteConverter.ToInt16(file, tmpaddr + 2) / 4) });
                                                    tmpaddr += UV.Size;
                                                    break;
                                                case 3:
                                                    tmpaddr += 6;
                                                    break;
                                                case 4:
                                                    uvs.Add(new UV(file, tmpaddr));
                                                    tmpaddr += UV.Size;
                                                    tmpaddr += 6;
                                                    break;
                                                case 5:
                                                    uvs.Add(new UV() { U = (short)(ByteConverter.ToInt16(file, tmpaddr) / 4), V = (short)(ByteConverter.ToInt16(file, tmpaddr + 2) / 4) });
                                                    tmpaddr += UV.Size;
                                                    tmpaddr += 6;
                                                    break;
                                                case 6:
                                                    vcs.Add(Color.FromArgb(ByteConverter.ToInt32(file, tmpaddr)));
                                                    tmpaddr += 4;
                                                    break;
                                                case 7:
                                                    uvs.Add(new UV(file, tmpaddr));
                                                    tmpaddr += UV.Size;
                                                    vcs.Add(Color.FromArgb(ByteConverter.ToInt32(file, tmpaddr)));
                                                    tmpaddr += 4;
                                                    break;
                                                case 8:
                                                    uvs.Add(new UV() { U = (short)(ByteConverter.ToInt16(file, tmpaddr) / 4), V = (short)(ByteConverter.ToInt16(file, tmpaddr + 2) / 4) });
                                                    tmpaddr += UV.Size;
                                                    vcs.Add(Color.FromArgb(ByteConverter.ToInt32(file, tmpaddr)));
                                                    tmpaddr += 4;
                                                    break;
                                            }
                                        }
                                    }
                                    bool hasUVs = uvs.Count > 0;
                                    bool hasVCs = vcs.Count > 0;
                                    Mesh mesh = new Mesh(polys, false, hasUVs, hasVCs) { MaterialID = (ushort)matnum };
                                    if (hasUVs)
                                        for (int i = 0; i < uvs.Count; i++)
                                            mesh.UV[i] = uvs[i];
                                    if (hasVCs)
                                        for (int i = 0; i < vcs.Count; i++)
                                            mesh.VColor[i] = vcs[i];
                                    Mesh.Add(mesh);
                                    break;
                                default:
                                    tmpaddr += 2;
                                    break;
                            }
                            header = ByteConverter.ToUInt16(file, tmpaddr);
                            ctype = (byte)(header & 0xFF);
                        }
                    }
                    Bounds = new BoundingSphere(file, address + 8);
                    break;
            }
        }

        public Attach(Vertex[] vertex, Vertex[] normal, IEnumerable<Mesh> mesh, IEnumerable<Material> material)
            : this()
        {
            Vertex = vertex;
            Normal = normal;
            Mesh = new List<Mesh>(mesh);
            Material = new List<Material>(material);
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, Dictionary<string, uint> labels, out uint address)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            List<byte> result = new List<byte>();
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
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
                            {
                                labels.Add(item.Name, (uint)result.Count + imageBase);
                                result.AddRange(item.GetBytes());
                            }
                        }
                    }
                    uint meshAddress = 0;
                    if (labels.ContainsKey(MaterialName))
                        materialAddress = labels[MaterialName];
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
                        {
                            labels.Add(Mesh[i].Name, (uint)result.Count + imageBase);
                            result.AddRange(Mesh[i].GetBytes(polyAddrs[i], polyNormalAddrs[i], vColorAddrs[i], uVAddrs[i], format == ModelFormat.SADX));
                        }
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
                    if (format == ModelFormat.SADX) result.AddRange(new byte[4]);
                    break;
                default:
                    address = 0;
                    throw new Exception(); // implement this later
            }
            labels.Add(Name, address + imageBase);
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

        public VertexData[][] GetVertexData()
        {
            List<VertexData[]> result = new List<VertexData[]>();
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
                        case PolyType.Triangles:
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
                        case PolyType.Quads:
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
                        case PolyType.NPoly:
                        case PolyType.Strips:
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
                result.Add(verts.ToArray());
            }
            return result.ToArray();
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
    }
}