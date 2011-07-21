using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public sealed class Triangle : Poly
    {
        public Triangle()
        {
            Name = "tri_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Indexes = new ushort[3];
        }

        public Triangle(byte[] file, int address)
        {
            Name = "tri_" + address.ToString("X8");
            Indexes = new ushort[3];
            Indexes[0] = BitConverter.ToUInt16(file, address);
            Indexes[1] = BitConverter.ToUInt16(file, address + 2);
            Indexes[2] = BitConverter.ToUInt16(file, address + 4);
        }

        public Triangle(Dictionary<string, string> group, string name)
        {
            Name = name;
            Indexes = new ushort[3];
            string[] inds = group["Indexes"].Split(',');
            for (int i = 0; i < 3; i++)
                Indexes[i] = ushort.Parse(inds[i], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public override PolyType PolyType { get { return SAModel.PolyType.Triangles; } }
    }

    public sealed class Quad : Poly
    {
        public Quad()
        {
            Name = "quad_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Indexes = new ushort[4];
        }

        public Quad(byte[] file, int address)
        {
            Name = "quad_" + address.ToString("X8");
            Indexes = new ushort[4];
            Indexes[0] = BitConverter.ToUInt16(file, address);
            Indexes[1] = BitConverter.ToUInt16(file, address + 2);
            Indexes[2] = BitConverter.ToUInt16(file, address + 4);
            Indexes[3] = BitConverter.ToUInt16(file, address + 8);
        }

        public Quad(Dictionary<string, string> group, string name)
        {
            Name = name;
            Indexes = new ushort[4];
            string[] inds = group["Indexes"].Split(',');
            for (int i = 0; i < 4; i++)
                Indexes[i] = ushort.Parse(inds[i], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public override PolyType PolyType { get { return SAModel.PolyType.Quads; } }
    }

    public sealed class Strip : Poly
    {
        public bool Reversed { get; private set; }

        public Strip(int NumVerts, bool Reverse)
        {
            Name = "strip_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Indexes = new ushort[NumVerts];
            Reversed = Reverse;
        }

        public Strip(ushort[] Verts, bool Reverse)
        {
            Name = "strip_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Indexes = Verts;
            Reversed = Reverse;
        }

        public Strip(byte[] file, int address)
        {
            Name = "strip_" + address.ToString("X8");
            Indexes = new ushort[BitConverter.ToUInt16(file, address) & 0x7FFF];
            Reversed = (BitConverter.ToUInt16(file, address) & 0x8000) == 0x8000;
            address += 2;
            for (int i = 0; i < Indexes.Length; i++)
            {
                Indexes[i] = BitConverter.ToUInt16(file, address);
                address += 2;
            }
        }

        public Strip(Dictionary<string, string> group, string name)
        {
            Name = name;
            string[] inds = group["Indexes"].Split(',');
            Indexes = new ushort[inds.Length];
            for (int i = 0; i < inds.Length; i++)
                Indexes[i] = ushort.Parse(inds[i], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Reversed = bool.Parse(group["Reversed"]);
        }

        public override int Size { get { return base.Size + 2; } }

        public override PolyType PolyType { get { return SAModel.PolyType.Strips; } }

        public override byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((ushort)(Indexes.Length | (Reversed ? 0x8000 : 0))));
            result.AddRange(base.GetBytes());
            return result.ToArray();
        }

        public override void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            string[] inds = new string[Indexes.Length];
            for (int i = 0; i < inds.Length; i++)
                inds[i] = Indexes[i].ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            group.Add("Indexes", string.Join(",", inds));
            group.Add("Reversed", Reversed.ToString());
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }
    }

    public abstract class Poly
    {
        public virtual ushort[] Indexes { get; protected set; }
        public string Name { get; set; }

        internal Poly() { }

        public virtual int Size { get { return Indexes.Length * 2; } }

        public abstract PolyType PolyType { get; }

        public virtual byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            foreach (ushort item in Indexes)
                result.AddRange(BitConverter.GetBytes(item));
            return result.ToArray();
        }

        public virtual void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            string[] inds = new string[Indexes.Length];
            for (int i = 0; i < inds.Length; i++)
                inds[i] = Indexes[i].ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            group.Add("Indexes", string.Join(",", inds));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }

        public static Poly CreatePoly(PolyType type)
        {
            switch (type)
            {
                case PolyType.Triangles:
                    return new Triangle();
                case PolyType.Quads:
                    return new Quad();
                case PolyType.Strips:
                case PolyType.Strips2:
                    throw new ArgumentException("Cannot create strip-type poly without additional information.\nUse Strip.Strip(int NumVerts, bool Reverse) instead.", "type");
            }
            throw new ArgumentException("Unknown poly type!", "type");
        }

        public static Poly CreatePoly(PolyType type, byte[] file, int address)
        {
            switch (type)
            {
                case PolyType.Triangles:
                    return new Triangle(file, address);
                case PolyType.Quads:
                    return new Quad(file, address);
                case PolyType.Strips:
                case PolyType.Strips2:
                    return new Strip(file, address);
            }
            throw new ArgumentException("Unknown poly type!", "type");
        }

        public static Poly CreatePoly(PolyType type, Dictionary<string, string> group, string name)
        {
            switch (type)
            {
                case PolyType.Triangles:
                    return new Triangle(group, name);
                case PolyType.Quads:
                    return new Quad(group, name);
                case PolyType.Strips:
                case PolyType.Strips2:
                    return new Strip(group, name);
            }
            throw new ArgumentException("Unknown poly type!", "type");
        }
    }
}