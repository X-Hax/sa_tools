using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class Attach
    {
        public string Name { get; set; }
        public Vertex[] Vertex { get; private set; }
        public Vertex[] Normal { get; private set; }
        public List<Mesh> Mesh { get; set; }
        public List<Material> Material { get; set; }
        public Vertex Center { get; set; }
        public float Radius { get; set; }

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
            Name = "attach_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Center = new Vertex();
            Material = new List<Material>();
            Mesh = new List<Mesh>();
            Vertex = new Vertex[0];
            Normal = new Vertex[0];
        }

        public Attach(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Name = "attach_" + address.ToString("X8");
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    Vertex = new Vertex[ByteConverter.ToInt32(file, address + 8)];
                    Normal = new Vertex[Vertex.Length];
                    int tmpaddr = (int)(ByteConverter.ToUInt32(file, address) - imageBase);
                    for (int i = 0; i < Vertex.Length; i++)
                    {
                        Vertex[i] = new Vertex(file, tmpaddr);
                        tmpaddr += SAModel.Vertex.Size;
                    }
                    tmpaddr = (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase);
                    for (int i = 0; i < Vertex.Length; i++)
                    {
                        Normal[i] = new Vertex(file, tmpaddr);
                        tmpaddr += SAModel.Vertex.Size;
                    }
                    Mesh = new List<Mesh>();
                    int meshcnt = ByteConverter.ToInt16(file, address + 0x14);
                    tmpaddr = ByteConverter.ToInt32(file, address + 0xC);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        for (int i = 0; i < meshcnt; i++)
                        {
                            Mesh.Add(new Mesh(file, tmpaddr, imageBase));
                            tmpaddr += SAModel.Mesh.Size(format == ModelFormat.SADX);
                        }
                    }
                    Material = new List<Material>();
                    int matcnt = ByteConverter.ToInt16(file, address + 0x16);
                    tmpaddr = ByteConverter.ToInt32(file, address + 0x10);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        for (int i = 0; i < matcnt; i++)
                        {
                            Material.Add(new Material(file, tmpaddr));
                            tmpaddr += SAModel.Material.Size;
                        }
                    }
                    Center = new Vertex(file, address + 0x18);
                    Radius = ByteConverter.ToSingle(file, address + 0x24);
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
                        ctype = file[tmpaddr];
                        while (ctype == 0x00)
                        {
                            tmpaddr += 2;
                            ctype = file[tmpaddr];
                        }
                        if (ctype < 0x20 | ctype > 0x36)
                        {
                            System.Diagnostics.Debug.WriteLine("Unrecognized chunk 0x" + ctype.ToString("X2") + " at 0x" + tmpaddr.ToString("X8") + ".");
                            goto poly;
                        }
                        while (ctype != 0xFF)
                        {
                            int curvert = ByteConverter.ToUInt16(file, tmpaddr + 4);
                            int vcnt = ByteConverter.ToUInt16(file, tmpaddr + 6);
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
                            ctype = file[tmpaddr];
                            while (ctype == 0x00)
                            {
                                tmpaddr += 2;
                                ctype = file[tmpaddr];
                            }
                            if (ctype < 0x20 | ctype > 0x36)
                            {
                                System.Diagnostics.Debug.WriteLine("Unrecognized chunk 0x" + ctype.ToString("X2") + " at 0x" + tmpaddr.ToString("X8") + ".");
                                goto poly;
                            }
                        }
                    }
                poly: Material = new List<Material>();
                    Mesh = new List<Mesh>();
                    Material mat = new Material();
                    tmpaddr = ByteConverter.ToInt32(file, address + 4);
                    if (tmpaddr != 0)
                    {
                        tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                        ctype = file[tmpaddr];
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
                                    bool hasDiffuse = (file[tmpaddr] & 1) != 0;
                                    int curaddr = tmpaddr + 4;
                                    if (hasDiffuse)
                                    {
                                        mat.DiffuseColor = Color.FromArgb(ByteConverter.ToInt32(file, curaddr));
                                        curaddr += 4;
                                    }
                                    else
                                        mat.DiffuseColor = Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC);
                                    bool hasAmbient = (file[tmpaddr] & 2) != 0;
                                    if (hasAmbient)
                                        curaddr += 4;
                                    bool hasSpecular = (file[tmpaddr] & 4) != 0;
                                    if (hasSpecular)
                                    {
                                        mat.SpecularColor = Color.FromArgb(ByteConverter.ToInt32(file, curaddr));
                                        curaddr += 4;
                                    }
                                    else
                                        mat.SpecularColor = Color.Transparent;
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
                                    int striptype = file[tmpaddr] & 0xF;
                                    int numflags = file[tmpaddr + 1] & 3;
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
                            ctype = file[tmpaddr];
                        }
                    }
                    Center = new Vertex(file, address + 8);
                    Radius = ByteConverter.ToSingle(file, address + 0x14);
                    break;
            }
        }

        public Attach(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Name = groupname;
            Dictionary<string, string> group = INI[groupname];
            string[] verts = group["Vertex"].Split('|');
            Vertex = new Vertex[verts.Length];
            Normal = new Vertex[Vertex.Length];
            for (int i = 0; i < Vertex.Length; i++)
                Vertex[i] = new Vertex(verts[i]);
            verts = group["Normal"].Split('|');
            for (int i = 0; i < Vertex.Length; i++)
                Normal[i] = new Vertex(verts[i]);
            string[] meshlist = group["Mesh"].Split(',');
            Mesh = new List<Mesh>();
            for (int i = 0; i < meshlist.Length; i++)
                Mesh.Add(new Mesh(INI, meshlist[i]));
            Material = new List<Material>();
            if (!string.IsNullOrEmpty(group["Material"]))
            {
                string[] matlist = group["Material"].Split(',');
                for (int i = 0; i < matlist.Length; i++)
                    Material.Add(new Material(INI[matlist[i]], matlist[i]));
            }
            Center = new Vertex(group["Center"]);
            Radius = float.Parse(group["Radius"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public Attach(Vertex[] vertex, Vertex[] normal, IEnumerable<Mesh> mesh, IEnumerable<Material> material)
        {
            Name = "attach_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Vertex = vertex;
            Normal = normal;
            Mesh = new List<Mesh>(mesh);
            Material = new List<Material>(material);
            Center = new Vertex();
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, out uint address)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            List<byte> result = new List<byte>();
            address = 0;
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    uint materialAddress = imageBase;
                    if (Material != null)
                    {
                        materialAddress = imageBase;
                        foreach (Material item in Material)
                            result.AddRange(item.GetBytes());
                    }
                    uint[] polyAddrs = new uint[Mesh.Count];
                    uint[] polyNormalAddrs = new uint[Mesh.Count];
                    uint[] vColorAddrs = new uint[Mesh.Count];
                    uint[] uVAddrs = new uint[Mesh.Count];
                    for (int i = 0; i < Mesh.Count; i++)
                    {
                        result.Align(4);
                        polyAddrs[i] = (uint)result.Count + imageBase;
                        for (int j = 0; j < Mesh[i].Poly.Count; j++)
                            result.AddRange(Mesh[i].Poly[j].GetBytes());
                    }
                    for (int i = 0; i < Mesh.Count; i++)
                    {
                        if (Mesh[i].PolyNormal != null)
                        {
                            result.Align(4);
                            polyNormalAddrs[i] = (uint)result.Count + imageBase;
                            for (int j = 0; j < Mesh[i].PolyNormal.Length; j++)
                                result.AddRange(Mesh[i].PolyNormal[j].GetBytes());
                        }
                    }
                    for (int i = 0; i < Mesh.Count; i++)
                    {
                        if (Mesh[i].VColor != null)
                        {
                            result.Align(4);
                            vColorAddrs[i] = (uint)result.Count + imageBase;
                            for (int j = 0; j < Mesh[i].VColor.Length; j++)
                                result.AddRange(VColor.GetBytes(Mesh[i].VColor[j]));
                        }
                    }
                    for (int i = 0; i < Mesh.Count; i++)
                    {
                        if (Mesh[i].UV != null)
                        {
                            result.Align(4);
                            uVAddrs[i] = (uint)result.Count + imageBase;
                            for (int j = 0; j < Mesh[i].UV.Length; j++)
                                result.AddRange(Mesh[i].UV[j].GetBytes());
                        }
                    }
                    result.Align(4);
                    uint meshAddress = (uint)result.Count + imageBase;
                    for (int i = 0; i < Mesh.Count; i++)
                        result.AddRange(Mesh[i].GetBytes(polyAddrs[i], polyNormalAddrs[i], vColorAddrs[i], uVAddrs[i], format == ModelFormat.SADX));
                    result.Align(4);
                    uint vertexAddress = (uint)result.Count + imageBase;
                    foreach (Vertex item in Vertex)
                        if (item == null)
                            result.AddRange(new byte[SAModel.Vertex.Size]);
                        else
                            result.AddRange(item.GetBytes());
                    result.Align(4);
                    uint normalAddress = (uint)result.Count + imageBase;
                    foreach (Vertex item in Normal)
                        if (item == null)
                            result.AddRange(new byte[SAModel.Vertex.Size]);
                        else
                            result.AddRange(item.GetBytes());
                    result.Align(4);
                    address = (uint)result.Count;
                    result.AddRange(ByteConverter.GetBytes(vertexAddress));
                    result.AddRange(ByteConverter.GetBytes(normalAddress));
                    result.AddRange(ByteConverter.GetBytes(Vertex.Length));
                    result.AddRange(ByteConverter.GetBytes(meshAddress));
                    result.AddRange(ByteConverter.GetBytes(materialAddress));
                    result.AddRange(ByteConverter.GetBytes((short)Mesh.Count));
                    result.AddRange(ByteConverter.GetBytes((short)Material.Count));
                    result.AddRange(Center.GetBytes());
                    result.AddRange(ByteConverter.GetBytes(Radius));
                    if (format == ModelFormat.SADX) result.AddRange(new byte[4]);
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    throw new Exception(); // implement this later
            }
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format)
        {
            uint address;
            return GetBytes(imageBase, format, out address);
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            List<string> verts = new List<string>();
            for (int i = 0; i < Vertex.Length; i++)
                verts.Add(Vertex[i].ToString());
            group.Add("Vertex", string.Join("|", verts.ToArray()));
            verts = new List<string>();
            for (int i = 0; i < Vertex.Length; i++)
                verts.Add(Normal[i].ToString());
            group.Add("Normal", string.Join("|", verts.ToArray())); List<string> mlist = new List<string>();
            for (int i = 0; i < Mesh.Count; i++)
            {
                mlist.Add(Mesh[i].Name);
                Mesh[i].Save(INI);
            }
            group.Add("Mesh", string.Join(",", mlist.ToArray()));
            mlist = new List<string>();
            for (int i = 0; i < Material.Count; i++)
            {
                mlist.Add(Material[i].Name);
                Material[i].Save(INI);
            }
            group.Add("Material", string.Join(",", mlist.ToArray()));
            group.Add("Center", Center.ToString());
            group.Add("Radius", Radius.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
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
                        case PolyType.Strips:
                        case PolyType.Strips2:
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