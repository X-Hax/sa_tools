using System;
using System.Collections.Generic;
using System.Drawing;

namespace SonicRetro.SAModel
{
    public class Material
    {
        public Color DiffuseColor { get; set; }
        public Color SpecularColor { get; set; }
        public float Exponent { get; set; }
        public int TextureID { get; set; }
        public uint Flags { get; set; }

        public byte UserFlags { get { return (byte)(Flags & 0x7F); } set { Flags = (uint)((Flags & ~0x7F) | (value & 0x7Fu)); } }
        public bool PickStatus { get { return (Flags & 0x80) == 0x80; } set { Flags = (uint)((Flags & ~0x80) | (value ? 0x80u : 0)); } }
        public bool SuperSample { get { return (Flags & 0x1000) == 0x1000; } set { Flags = (uint)((Flags & ~0x1000) | (value ? 0x1000u : 0)); } }
        public FilterMode FilterMode { get { return (FilterMode)((Flags >> 13) & 3); } set { Flags = (uint)((Flags & ~0x6000) | ((uint)value << 13)); } }
        public bool ClampV { get { return (Flags & 0x8000) == 0x8000; } set { Flags = (uint)((Flags & ~0x8000) | (value ? 0x8000u : 0)); } }
        public bool ClampU { get { return (Flags & 0x10000) == 0x10000; } set { Flags = (uint)((Flags & ~0x10000) | (value ? 0x10000u : 0)); } }
        public bool FlipV { get { return (Flags & 0x20000) == 0x20000; } set { Flags = (uint)((Flags & ~0x20000) | (value ? 0x20000u : 0)); } }
        public bool FlipU { get { return (Flags & 0x40000) == 0x40000; } set { Flags = (uint)((Flags & ~0x40000) | (value ? 0x40000u : 0)); } }
        public bool IgnoreSpecular { get { return (Flags & 0x80000) == 0x80000; } set { Flags = (uint)((Flags & ~0x80000) | (value ? 0x80000u : 0)); } }
        public bool UseAlpha { get { return (Flags & 0x100000) == 0x100000; } set { Flags = (uint)((Flags & ~0x100000) | (value ? 0x100000u : 0)); } }
        public bool UseTexture { get { return (Flags & 0x200000) == 0x200000; } set { Flags = (uint)((Flags & ~0x200000) | (value ? 0x200000u : 0)); } }
        public bool EnvironmentMap { get { return (Flags & 0x400000) == 0x400000; } set { Flags = (uint)((Flags & ~0x400000) | (value ? 0x400000u : 0)); } }
        public bool DoubleSided { get { return (Flags & 0x800000) == 0x800000; } set { Flags = (uint)((Flags & ~0x800000) | (value ? 0x800000u : 0)); } }
        public bool FlatShading { get { return (Flags & 0x1000000) == 0x1000000; } set { Flags = (uint)((Flags & ~0x1000000) | (value ? 0x1000000u : 0)); } }
        public bool IgnoreLighting { get { return (Flags & 0x2000000) == 0x2000000; } set { Flags = (uint)((Flags & ~0x2000000) | (value ? 0x2000000u : 0)); } }
        public AlphaInstruction DestinationAlpha { get { return (AlphaInstruction)((Flags >> 26) & 7); } set { Flags = (uint)((Flags & ~0x1C000000) | ((uint)value << 26)); } }
        public AlphaInstruction SourceAlpha { get { return (AlphaInstruction)((Flags >> 29) & 7); } set { Flags = (uint)((Flags & ~0xE0000000) | ((uint)value << 29)); } }

        public static int Size { get { return 0x14; } }

        public Material()
        {
            DiffuseColor = Color.FromArgb(0xFF, 0xB2, 0xB2, 0xB2);
            SpecularColor = Color.Transparent;
            UseAlpha = true;
            UseTexture = true;
            DoubleSided = false;
            FlatShading = false;
            IgnoreLighting = false;
            ClampU = false;
            ClampV = false;
            FlipU = false;
            FlipV = false;
            EnvironmentMap = false;
            DestinationAlpha = AlphaInstruction.InverseSourceAlpha;
            SourceAlpha = AlphaInstruction.SourceAlpha;
        }

        public Material(byte[] file, int address)
            : this(file, address, new Dictionary<int, string>()) { }

        public Material(byte[] file, int address, Dictionary<int, string> labels)
        {
            DiffuseColor = Color.FromArgb(ByteConverter.ToInt32(file, address));
            SpecularColor = Color.FromArgb(ByteConverter.ToInt32(file, address + 4));
            Exponent = ByteConverter.ToSingle(file, address + 8);
            TextureID = ByteConverter.ToInt32(file, address + 0xC);
            Flags = ByteConverter.ToUInt32(file, address + 0x10);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(ByteConverter.GetBytes(DiffuseColor.ToArgb()));
            result.AddRange(ByteConverter.GetBytes(SpecularColor.ToArgb()));
            result.AddRange(ByteConverter.GetBytes(Exponent));
            result.AddRange(ByteConverter.GetBytes(TextureID));
            result.AddRange(ByteConverter.GetBytes(Flags));
            return result.ToArray();
        }

        public string ToStruct(string[] textures)
        {
            if (DiffuseColor == Color.Empty && SpecularColor == Color.Empty && Exponent == 0 && TextureID == 0 && Flags == 0)
                return "{ 0 }";
            System.Text.StringBuilder result = new System.Text.StringBuilder("{ ");
            result.Append(DiffuseColor.ToStruct());
            result.Append(", ");
            result.Append(SpecularColor.ToStruct());
            result.Append(", ");
            result.Append(Exponent.ToC());
            result.Append(", ");
            int callback = (int)(TextureID & 0xC0000000);
            int texid = (int)(TextureID & ~0xC0000000);
            if (callback != 0)
                result.Append(((StructEnums.NJD_CALLBACK)callback).ToString().Replace(", ", " | ") + " | ");
			if (textures == null || texid >= textures.Length)
				result.Append(texid);
			else
				result.Append(textures[texid].MakeIdentifier());
            result.Append(", ");
            result.Append(((StructEnums.MaterialFlags)(Flags & ~0x7F)).ToString().Replace(", ", " | "));
            if (UserFlags != 0)
                result.Append(" | 0x" + UserFlags.ToString("X"));
            result.Append(" }");
            return result.ToString();
        }

		public string ToStruct() { return ToStruct(null); }

        public override bool Equals(object obj)
        {
            if (!(obj is Material))
                return false;
            Material other = (Material)obj;
            return this.DiffuseColor == other.DiffuseColor && this.SpecularColor == other.SpecularColor && this.Exponent == other.Exponent && this.TextureID == other.TextureID && this.Flags == other.Flags;
        }

        public override int GetHashCode()
        {
            return DiffuseColor.GetHashCode() ^ SpecularColor.GetHashCode() ^ Exponent.GetHashCode() ^ TextureID.GetHashCode() ^ Flags.GetHashCode();
        }
    }
}