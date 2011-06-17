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

        public static int Size(bool DX) { return DX ? 0x1C : 0x18; }

        public Mesh(byte[] file, int address, uint imageBase)
        {
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

        public Mesh(PolyType polyType, int polyCount, bool hasPolyNormal, bool hasUV, bool hasVColor)
        {
            if (polyType == SAModel.PolyType.Strips | polyType == SAModel.PolyType.Strips2) throw new ArgumentException("Cannot create a Poly of that type!\nTry another overload to create Strip-type Polys.", "polyType");
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

        public Mesh(int[] stripCount, bool[] stripDir, int polyCount, bool hasPolyNormal, bool hasUV, bool hasVColor)
        {
            if (stripCount.Length != polyCount)
                throw new ArgumentException("stripCount length does not match polyCount!", "stripCount");
            if (stripDir.Length != polyCount)
                throw new ArgumentException("stripDir length does not match polyCount!", "stripDir");
            PolyType = SAModel.PolyType.Strips;
            Poly[] polys = new Poly[polyCount];
            int striptotal = 0;
            for (int i = 0; i < polys.Length; i++)
            {
                polys[i] = new Strip(stripCount[i], stripDir[i]);
                striptotal += stripCount[i];
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
    }
}
