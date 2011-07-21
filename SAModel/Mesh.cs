using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class Mesh
    {
        public ushort MaterialID { get; set; }
        public PolyType PolyType { get; private set; }
        public ReadOnlyCollection<Poly> Poly { get; private set; }
        public int PAttr { get; set; }
        public PolyNormal[] PolyNormal { get; private set; }
        public Color[] VColor { get; private set; }
        public UV[] UV { get; private set; }
        public string Name { get; set; }

        public static int Size(bool DX) { return DX ? 0x1C : 0x18; }

        public Mesh(byte[] file, int address, uint imageBase)
        {
            Name = "mesh_" + address.ToString("X8");
            MaterialID = BitConverter.ToUInt16(file, address);
            PolyType = (PolyType)(MaterialID >> 0xE);
            MaterialID &= 0x3FFF;
            Poly[] polys = new Poly[BitConverter.ToInt16(file, address + 2)];
            int tmpaddr = (int)(BitConverter.ToUInt32(file, address + 4) - imageBase);
            int striptotal = 0;
            for (int i = 0; i < polys.Length; i++)
            {
                polys[i] = SAModel.Poly.CreatePoly(PolyType, file, tmpaddr);
                striptotal += polys[i].Indexes.Length;
                tmpaddr += polys[i].Size;
            }
            Poly = new ReadOnlyCollection<SAModel.Poly>(polys);
            PAttr = BitConverter.ToInt32(file, address + 8);
            tmpaddr = BitConverter.ToInt32(file, address + 0xC);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                PolyNormal = new PolyNormal[polys.Length];
                for (int i = 0; i < polys.Length; i++)
                {
                    PolyNormal[i] = new PolyNormal(file, tmpaddr);
                    tmpaddr += SAModel.PolyNormal.Size;
                }
            }
            tmpaddr = BitConverter.ToInt32(file, address + 0x10);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                VColor = new Color[striptotal];
                for (int i = 0; i < striptotal; i++)
                {
                    VColor[i] = SAModel.VColor.FromBytes(file, tmpaddr);
                    tmpaddr += SAModel.VColor.Size;
                }
            }
            tmpaddr = BitConverter.ToInt32(file, address + 0x14);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                UV = new UV[striptotal];
                for (int i = 0; i < striptotal; i++)
                {
                    UV[i] = new UV(file, tmpaddr);
                    tmpaddr += SAModel.UV.Size;
                }
            }
        }

        public Mesh(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Name = groupname;
            Dictionary<string, string> group = INI[groupname];
            MaterialID = (ushort)(ushort.Parse(group["Material"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo) & 0x3FFF);
            PolyType = (PolyType)Enum.Parse(typeof(PolyType), group["PolyType"]);
            string[] polylist = group["Poly"].Split(',');
            Poly[] polys = new Poly[polylist.Length];
            int striptotal = 0;
            for (int i = 0; i < polys.Length; i++)
            {
                polys[i] = SAModel.Poly.CreatePoly(PolyType, INI[polylist[i]], polylist[i]);
                striptotal += polys[i].Indexes.Length;
            }
            Poly = new ReadOnlyCollection<SAModel.Poly>(polys);
            PAttr = int.Parse(group["PAttr"], System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo);
            if (group.ContainsKey("PolyNormal"))
            {
                PolyNormal = new PolyNormal[polys.Length];
                for (int i = 0; i < polys.Length; i++)
                    PolyNormal[i] = new PolyNormal(INI[group["PolyNormal"]], group["PolyNormal"]);
            }
            if (group.ContainsKey("VColor"))
            {
                VColor = new Color[striptotal];
                string[] vc = group["VColor"].Split(',');
                for (int i = 0; i < striptotal; i++)
                    VColor[i] = Color.FromArgb(int.Parse(vc[i], System.Globalization.NumberStyles.HexNumber));
            }
            if (group.ContainsKey("UV"))
            {
                UV = new UV[striptotal];
                string[] uvs = group["UV"].Split('|');
                for (int i = 0; i < striptotal; i++)
                    UV[i] = new UV(uvs[i]);
            }
        }

        public Mesh(PolyType polyType, int polyCount, bool hasPolyNormal, bool hasUV, bool hasVColor)
        {
            if (polyType == SAModel.PolyType.Strips | polyType == SAModel.PolyType.Strips2) throw new ArgumentException("Cannot create a Poly of that type!\nTry another overload to create Strip-type Polys.", "polyType");
            Name = "mesh_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            PolyType = polyType;
            Poly[] polys = new Poly[polyCount];
            int striptotal = 0;
            for (int i = 0; i < polys.Length; i++)
            {
                polys[i] = SAModel.Poly.CreatePoly(PolyType);
                striptotal += polys[i].Indexes.Length;
            }
            Poly = new ReadOnlyCollection<SAModel.Poly>(polys);
            if (hasPolyNormal)
            {
                PolyNormal = new PolyNormal[polys.Length];
                for (int i = 0; i < polys.Length; i++)
                    PolyNormal[i] = new PolyNormal();
            }
            if (hasVColor)
                VColor = new Color[striptotal];
            if (hasUV)
            {
                UV = new UV[striptotal];
                for (int i = 0; i < striptotal; i++)
                    UV[i] = new UV();
            }
        }

        public Mesh(Poly[] polys, bool hasPolyNormal, bool hasUV, bool hasVColor)
        {
            Name = "mesh_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            PolyType = polys[0].PolyType;
            int striptotal = 0;
            for (int i = 0; i < polys.Length; i++)
                striptotal += polys[i].Indexes.Length;
            Poly = new ReadOnlyCollection<SAModel.Poly>(polys);
            if (hasPolyNormal)
            {
                PolyNormal = new PolyNormal[polys.Length];
                for (int i = 0; i < polys.Length; i++)
                    PolyNormal[i] = new PolyNormal();
            }
            if (hasVColor)
                VColor = new Color[striptotal];
            if (hasUV)
            {
                UV = new UV[striptotal];
                for (int i = 0; i < striptotal; i++)
                    UV[i] = new UV();
            }
        }

        public byte[] GetBytes(uint polyAddress, uint polyNormalAddress, uint vColorAddress, uint uVAddress, bool DX)
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((ushort)((MaterialID & 0x3FFF) | ((int)PolyType << 0xE))));
            result.AddRange(BitConverter.GetBytes((ushort)Poly.Count));
            result.AddRange(BitConverter.GetBytes(polyAddress));
            result.AddRange(BitConverter.GetBytes(PAttr));
            result.AddRange(BitConverter.GetBytes(polyNormalAddress));
            result.AddRange(BitConverter.GetBytes(vColorAddress));
            result.AddRange(BitConverter.GetBytes(uVAddress));
            if (DX) result.AddRange(new byte[4]);
            return result.ToArray();
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Material", MaterialID.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("PolyType", PolyType.ToString());
            List<string> polylist = new List<string>();
            for (int i = 0; i < Poly.Count; i++)
            {
                polylist.Add(Poly[i].Name);
                Poly[i].Save(INI);
            }
            group.Add("Poly", string.Join(",", polylist.ToArray()));
            group.Add("PAttr", PAttr.ToString("X8"));
            if (PolyNormal != null)
            {
                polylist = new List<string>();
                for (int i = 0; i < PolyNormal.Length; i++)
                {
                    polylist.Add(PolyNormal[i].Name);
                    PolyNormal[i].Save(INI);
                }
                group.Add("PolyNormal", string.Join(",", polylist.ToArray()));
            }
            if (VColor != null)
            {
                polylist = new List<string>();
                for (int i = 0; i < VColor.Length; i++)
                    polylist.Add(VColor[i].ToArgb().ToString("X8"));
                group.Add("VColor", string.Join(",", polylist.ToArray()));
            }
            if (UV != null)
            {
                polylist = new List<string>();
                for (int i = 0; i < UV.Length; i++)
                    polylist.Add(UV[i].ToString());
                group.Add("UV", string.Join("|", polylist.ToArray()));
            }
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }
    }
}