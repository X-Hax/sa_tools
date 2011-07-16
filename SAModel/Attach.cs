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
        public Mesh[] Mesh { get; private set; }
        public Material[] Material { get; private set; }
        public Vertex Center { get; set; }
        public float Radius { get; set; }

        public static int Size(bool DX) { return DX ? 0x2C : 0x28; }

        public Attach(byte[] file, int address, uint imageBase, bool DX)
        {
            Name = "attach_" + address.ToString("X8");
            Vertex = new Vertex[BitConverter.ToInt32(file, address + 8)];
            Normal = new Vertex[Vertex.Length];
            int tmpaddr = (int)(BitConverter.ToUInt32(file, address) - imageBase);
            for (int i = 0; i < Vertex.Length; i++)
            {
                Vertex[i] = new Vertex(file, tmpaddr);
                tmpaddr += SAModel.Vertex.Size;
            }
            tmpaddr = (int)(BitConverter.ToUInt32(file, address + 4) - imageBase);
            for (int i = 0; i < Vertex.Length; i++)
            {
                Normal[i] = new Vertex(file, tmpaddr);
                tmpaddr += SAModel.Vertex.Size;
            }
            Mesh = new Mesh[BitConverter.ToInt16(file, address + 0x14)];
            tmpaddr = BitConverter.ToInt32(file, address + 0xC);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                for (int i = 0; i < Mesh.Length; i++)
                {
                    Mesh[i] = new Mesh(file, tmpaddr, imageBase);
                    tmpaddr += SAModel.Mesh.Size(DX);
                }
            }
            Material = new Material[BitConverter.ToInt16(file, address + 0x16)];
            tmpaddr = BitConverter.ToInt32(file, address + 0x10);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                for (int i = 0; i < Material.Length; i++)
                {
                    Material[i] = new Material(file, tmpaddr);
                    tmpaddr += SAModel.Material.Size;
                }
            }
            Center = new Vertex(file, address + 0x18);
            Radius = BitConverter.ToSingle(file, address + 0x24);
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
            Mesh = new Mesh[meshlist.Length];
            for (int i = 0; i < Mesh.Length; i++)
                Mesh[i] = new Mesh(INI, meshlist[i]);
            string[] matlist = group["Material"].Split(',');
            Material = new Material[matlist.Length];
            for (int i = 0; i < Material.Length; i++)
                Material[i] = new Material(INI[meshlist[i]], meshlist[i]);
            Center = new Vertex(group["Center"]);
            Radius = float.Parse(group["Radius"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public byte[] GetBytes(uint imageBase, bool DX, out uint address)
        {
            List<byte> result = new List<byte>();
            uint materialAddress = imageBase;
            if (Material != null)
            {
                materialAddress = imageBase;
                foreach (Material item in Material)
                {
                    result.AddRange(item.GetBytes());
                }
            }
            uint[] polyAddrs = new uint[Mesh.Length];
            uint[] polyNormalAddrs = new uint[Mesh.Length];
            uint[] vColorAddrs = new uint[Mesh.Length];
            uint[] uVAddrs = new uint[Mesh.Length];
            for (int i = 0; i < Mesh.Length; i++)
            {
                polyAddrs[i] = (uint)result.Count + imageBase;
                for (int j = 0; j < Mesh[i].Poly.Count; j++)
                    result.AddRange(Mesh[i].Poly[j].GetBytes());
            }
            for (int i = 0; i < Mesh.Length; i++)
            {
                if (Mesh[i].PolyNormal != null)
                {
                    polyNormalAddrs[i] = (uint)result.Count + imageBase;
                    for (int j = 0; j < Mesh[i].PolyNormal.Length; j++)
                        result.AddRange(Mesh[i].PolyNormal[j].GetBytes());
                }
            }
            for (int i = 0; i < Mesh.Length; i++)
            {
                if (Mesh[i].VColor != null)
                {
                    vColorAddrs[i] = (uint)result.Count + imageBase;
                    for (int j = 0; j < Mesh[i].VColor.Length; j++)
                        result.AddRange(VColor.GetBytes(Mesh[i].VColor[j]));
                }
            }
            for (int i = 0; i < Mesh.Length; i++)
            {
                if (Mesh[i].UV != null)
                {
                    uVAddrs[i] = (uint)result.Count + imageBase;
                    for (int j = 0; j < Mesh[i].UV.Length; j++)
                        result.AddRange(Mesh[i].UV[j].GetBytes());
                }
            }
            uint meshAddress = (uint)result.Count + imageBase;
            for (int i = 0; i < Mesh.Length; i++)
                result.AddRange(Mesh[i].GetBytes(polyAddrs[i], polyNormalAddrs[i], vColorAddrs[i], uVAddrs[i], DX));
            uint vertexAddress = (uint)result.Count + imageBase;
            foreach (Vertex item in Vertex)
                result.AddRange(item.GetBytes());
            uint normalAddress = (uint)result.Count + imageBase;
            foreach (Vertex item in Normal)
                result.AddRange(item.GetBytes());
            address = imageBase + (uint)result.Count;
            result.AddRange(BitConverter.GetBytes(vertexAddress));
            result.AddRange(BitConverter.GetBytes(normalAddress));
            result.AddRange(BitConverter.GetBytes(Vertex.Length));
            result.AddRange(BitConverter.GetBytes(meshAddress));
            result.AddRange(BitConverter.GetBytes(materialAddress));
            result.AddRange(BitConverter.GetBytes((short)Mesh.Length));
            result.AddRange(BitConverter.GetBytes((short)Material.Length));
            result.AddRange(Center.GetBytes());
            result.AddRange(BitConverter.GetBytes(Radius));
            if (DX) result.AddRange(new byte[4]);
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, bool DX)
        {
            uint address;
            return GetBytes(imageBase, DX, out address);
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
            for (int i = 0; i < Mesh.Length; i++)
            {
                mlist.Add(Mesh[i].Name);
                Mesh[i].Save(INI);
            }
            group.Add("Mesh", string.Join(",", mlist.ToArray()));
            mlist = new List<string>();
            for (int i = 0; i < Material.Length; i++)
            {
                mlist.Add(Material[i].Name);
                Material[i].Save(INI);
            }
            group.Add("Material", string.Join(",", mlist.ToArray()));
            group.Add("Center", Center.ToString());
            group.Add("Radius", Radius.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
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
                                        Vertex[poly.Indexes[k+1]],
                                        Normal[poly.Indexes[k+1]],
                                        hasVColor ? mesh.VColor[currentstriptotal + 1] : Color.White,
                                        hasUV ? mesh.UV[currentstriptotal + 1] : new UV()));
                                    verts.Add(new VertexData(
                                        Vertex[poly.Indexes[k+2]],
                                        Normal[poly.Indexes[k+2]],
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
    }
}
