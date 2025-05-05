using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SAModel
{
	[Serializable]
	public class NJS_MATERIAL : ICloneable
	{
		#region Basic Variables (internal use)

		public Color AmbientColor { get; set; }
		public Color DiffuseColor  { get; set; }
		public Color SpecularColor { get; set; }
		public float Exponent      { get; set; }
		public int   TextureID     { get; set; }
		public uint  Flags         { get; set; }

		#endregion

		#region Accessors (user use)

		public byte UserFlags
		{
			get { return (byte)(Flags & 0x7F); }
			set { Flags = (uint)((Flags & ~0x7F) | (value & 0x7Fu)); }
		}

		public bool PickStatus
		{
			get { return (Flags & 0x80) == 0x80; }
			set { Flags = (uint)((Flags & ~0x80) | (value ? 0x80u : 0)); }
		}

		public float MipmapDAdjust
		{
			get { return ((Flags & 0xF00) >> 8) * 0.25f; }
			set
			{
				Flags = (uint)(Flags & ~0xF00) | ((uint)Math.Max(0, Math.Min(0xF, Math.Round(value / 0.25, MidpointRounding.AwayFromZero))) << 8);
			}
		}

		public bool SuperSample
		{
			get { return (Flags & 0x1000) == 0x1000; }
			set { Flags = (uint)((Flags & ~0x1000) | (value ? 0x1000u : 0)); }
		}

		public FilterMode FilterMode
		{
			get { return (FilterMode)((Flags >> 13) & 3); }
			set { Flags = (uint)((Flags & ~0x6000) | ((uint)value << 13)); }
		}

		public bool ClampV
		{
			get { return (Flags & 0x8000) == 0x8000; }
			set { Flags = (uint)((Flags & ~0x8000) | (value ? 0x8000u : 0)); }
		}

		public bool ClampU
		{
			get { return (Flags & 0x10000) == 0x10000; }
			set { Flags = (uint)((Flags & ~0x10000) | (value ? 0x10000u : 0)); }
		}

		public bool FlipV
		{
			get { return (Flags & 0x20000) == 0x20000; }
			set { Flags = (uint)((Flags & ~0x20000) | (value ? 0x20000u : 0)); }
		}

		public bool FlipU
		{
			get { return (Flags & 0x40000) == 0x40000; }
			set { Flags = (uint)((Flags & ~0x40000) | (value ? 0x40000u : 0)); }
		}

		public bool IgnoreSpecular
		{
			get { return (Flags & 0x80000) == 0x80000; }
			set { Flags = (uint)((Flags & ~0x80000) | (value ? 0x80000u : 0)); }
		}

		// The following two are Chunk only
		public bool IgnoreAmbient { get; set; }
		public bool NoAlphaTest { get; set; }

		public bool UseAlpha
		{
			get { return (Flags & 0x100000) == 0x100000; }
			set { Flags = (uint)((Flags & ~0x100000) | (value ? 0x100000u : 0)); }
		}

		public bool UseTexture
		{
			get { return (Flags & 0x200000) == 0x200000; }
			set { Flags = (uint)((Flags & ~0x200000) | (value ? 0x200000u : 0)); }
		}

		public bool EnvironmentMap
		{
			get { return (Flags & 0x400000) == 0x400000; }
			set { Flags = (uint)((Flags & ~0x400000) | (value ? 0x400000u : 0)); }
		}

		public bool DoubleSided
		{
			get { return (Flags & 0x800000) == 0x800000; }
			set { Flags = (uint)((Flags & ~0x800000) | (value ? 0x800000u : 0)); }
		}

		public bool FlatShading
		{
			get { return (Flags & 0x1000000) == 0x1000000; }
			set { Flags = (uint)((Flags & ~0x1000000) | (value ? 0x1000000u : 0)); }
		}

		public bool IgnoreLighting
		{
			get { return (Flags & 0x2000000) == 0x2000000; }
			set { Flags = (uint)((Flags & ~0x2000000) | (value ? 0x2000000u : 0)); }
		}

		public AlphaInstruction DestinationAlpha
		{
			get { return (AlphaInstruction)((Flags >> 26) & 7); }
			set { Flags = (uint)((Flags & ~0x1C000000) | ((uint)value << 26)); }
		}

		public AlphaInstruction SourceAlpha
		{
			get { return (AlphaInstruction)((Flags >> 29) & 7); }
			set { Flags = (Flags & ~0xE0000000) | ((uint)value << 29); }
		}

		#endregion

		public static int Size
		{
			get { return 0x14; }
		}

		/// <summary>
		/// Create a new material.
		/// </summary>
		public NJS_MATERIAL()
		{
			AmbientColor = Color.White;
			DiffuseColor = Color.White;
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

		/// <summary>
		/// Use this if you need a copy of a material.
		/// </summary>
		/// <param name="copy"></param>
		public NJS_MATERIAL(NJS_MATERIAL copy)
		{
			AmbientColor = copy.AmbientColor;
			DiffuseColor = copy.DiffuseColor;
			SpecularColor = copy.SpecularColor;
			Exponent = copy.Exponent;
			TextureID = copy.TextureID;
			Flags = copy.Flags;
		}

		/// <summary>
		/// Load a material from a file buffer
		/// </summary>
		/// <param name="file">byte array representing file</param>
		/// <param name="address">address of this material within 'file' byte array.</param>
		public NJS_MATERIAL(byte[] file, int address)
			: this(file, address, new Dictionary<int, string>())
		{
		}

		/// <summary>
		/// Load a material from a file buffer, with labels.
		/// </summary>
		/// <param name="file">byte array representing file</param>
		/// <param name="address">address of this material within 'file' byte array.</param>
		/// <param name="labels"></param>
		public NJS_MATERIAL(byte[] file, int address, Dictionary<int, string> labels)
		{
			if (ByteConverter.BigEndian)
			{
				//"Reverse" is for the order used in SADX Gamecube
				if (ByteConverter.Reverse)
				{
					DiffuseColor = Color.FromArgb(file[address + 3], file[address], file[address + 1], file[address + 2]);
					SpecularColor = Color.FromArgb(file[address + 7], file[address + 4], file[address + 5], file[address + 6]);
				}
				else
				{
					DiffuseColor = Color.FromArgb(file[address], file[address + 1], file[address + 2], file[address + 3]);
					SpecularColor = Color.FromArgb(file[address + 4], file[address + 5], file[address + 6], file[address + 7]);
				}
			}
			else
			{
				DiffuseColor = Color.FromArgb(file[address + 3], file[address + 2], file[address + 1], file[address]);
				SpecularColor = Color.FromArgb(file[address + 7], file[address + 6], file[address + 5], file[address + 4]);
			}
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

		public string ToStruct(string[] textures = null)
		{
			if (DiffuseColor == Color.Empty && SpecularColor == Color.Empty && Exponent == 0 && TextureID == 0 && Flags == 0)
				return "{ 0 }";
			StringBuilder result = new StringBuilder("{ ");
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

		public string ToNJA(string[] textures)
		{
			if (DiffuseColor == Color.Empty && SpecularColor == Color.Empty && Exponent == 0 && TextureID == 0 && Flags == 0)
				return "{ 0 }";
			StringBuilder result = new StringBuilder("MATSTART" + Environment.NewLine);
			result.Append("Diffuse   ( ");
			result.Append(DiffuseColor.A + ", " + DiffuseColor.R + ", " + DiffuseColor.G + ", " + DiffuseColor.B);
			result.Append(" ), " + Environment.NewLine);
			//result.Append(SpecularColor.ToStruct());
			result.Append("Specular  ( ");
			result.Append(SpecularColor.A + ", " + SpecularColor.R + ", " + SpecularColor.G + ", " + SpecularColor.B);
			result.Append(" ), " + Environment.NewLine);
			result.Append("Exponent  ( ");
			result.Append(Exponent.ToNJA(true));
			result.Append(" ), " + Environment.NewLine);
			int callback = (int)(TextureID & 0xC0000000);
			int texid = (int)(TextureID & ~0xC0000000);
			result.Append("AttrTexId ( 0x" + callback.ToString("X") + ", " + texid + " )," + Environment.NewLine);
			result.Append("AttrFlags ( 0x" + Flags.ToString("X8").ToLowerInvariant() + " )," + Environment.NewLine);
			result.Append("MATEND" + Environment.NewLine);
			return result.ToString();
		}

		public void UpdateFromPolyChunk(PolyChunk chunk)
		{
			switch (chunk)
			{
				case PolyChunkBitsBlendAlpha bba:
					SourceAlpha = bba.SourceAlpha;
					DestinationAlpha = bba.DestinationAlpha;
					break;
				case PolyChunkBitsMipmapDAdjust bmda:
					MipmapDAdjust = bmda.MipmapDAdjust;
					break;
				case PolyChunkBitsSpecularExponent bse:
					Exponent = bse.SpecularExponent;
					break;
				case PolyChunkTinyTextureID ttid:
					MipmapDAdjust = ttid.MipmapDAdjust;
					ClampU = ttid.ClampU;
					ClampV = ttid.ClampV;
					FilterMode = ttid.FilterMode;
					FlipU = ttid.FlipU;
					FlipV = ttid.FlipV;
					SuperSample = ttid.SuperSample;
					TextureID = ttid.TextureID;
					break;
				case PolyChunkMaterial mat:
					SourceAlpha = mat.SourceAlpha;
					DestinationAlpha = mat.DestinationAlpha;
					if (mat.Ambient.HasValue)
						AmbientColor = mat.Ambient.Value;
					if (mat.Diffuse.HasValue)
						DiffuseColor = mat.Diffuse.Value;
					if (mat.Specular.HasValue)
					{
						SpecularColor = mat.Specular.Value;
						Exponent = mat.SpecularExponent;
					}
					break;
				case PolyChunkStrip str:
					DoubleSided = str.DoubleSide;
					EnvironmentMap = str.EnvironmentMapping;
					FlatShading = str.FlatShading;
					IgnoreLighting = str.IgnoreLight;
					IgnoreSpecular = str.IgnoreSpecular;
					IgnoreAmbient = str.IgnoreAmbient;
					UseAlpha = str.UseAlpha;
					UseTexture = EnvironmentMap;
					NoAlphaTest = str.NoAlphaTest;
					switch (chunk.Type)
					{
						case ChunkType.Strip_StripUVN:
						case ChunkType.Strip_StripUVH:
						case ChunkType.Strip_StripUVNColor:
						case ChunkType.Strip_StripUVHColor:
						case ChunkType.Strip_StripUVN2:
						case ChunkType.Strip_StripUVH2:
							UseTexture = true;
							break;
					}
					break;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NJS_MATERIAL))
				return false;
			NJS_MATERIAL other = (NJS_MATERIAL)obj;
			return AmbientColor == other.AmbientColor && DiffuseColor == other.DiffuseColor && SpecularColor == other.SpecularColor &&
				Exponent == other.Exponent && TextureID == other.TextureID && Flags == other.Flags;
		}

		public override int GetHashCode()
		{
			return AmbientColor.GetHashCode() ^ DiffuseColor.GetHashCode() ^ SpecularColor.GetHashCode() ^ Exponent.GetHashCode() ^
				TextureID.GetHashCode() ^ Flags.GetHashCode();
		}

		object ICloneable.Clone() => Clone();

		public NJS_MATERIAL Clone() => (NJS_MATERIAL)MemberwiseClone();
	}
}