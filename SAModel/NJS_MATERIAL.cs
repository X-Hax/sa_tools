using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;

namespace SonicRetro.SAModel
{
	/// <summary>
	/// Flags used for materials.
	/// </summary>
	[Flags]
	public enum NJD_FLAG : uint
	{
		Pick           = 0x80,
		UseAnisotropic = 0x1000,
		ClampV         = 0x8000,
		ClampU         = 0x10000,
		FlipV          = 0x20000,
		FlipU          = 0x40000,
		IgnoreSpecular = 0x80000,
		UseAlpha       = 0x100000,
		UseTexture     = 0x200000,
		UseEnv         = 0x400000,
		DoubleSide     = 0x800000,
		UseFlat        = 0x1000000,
		IgnoreLight    = 0x2000000
	}

	[Serializable]
	public class NJS_MATERIAL : ICloneable
	{
		#region Basic Variables (internal use)

		public Color    DiffuseColor  { get; set; }
		public Color    SpecularColor { get; set; }
		public float    Exponent      { get; set; }
		public int      TextureID     { get; set; }
		public NJD_FLAG Flags         { get; set; }

		#endregion

		#region Accessors (user use)

		public byte UserFlags
		{
			get => (byte)((uint)Flags & 0x7F);
			set => Flags = (NJD_FLAG)(((uint)Flags & ~0x7F) | (value & 0x7Fu));
		}

		public bool PickStatus
		{
			get => (Flags & NJD_FLAG.Pick) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.Pick;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.Pick);
				}
			}
		}

		public bool SuperSample
		{
			get => (Flags & NJD_FLAG.UseAnisotropic) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.UseAnisotropic;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.UseAnisotropic);
				}
			}
		}

		public FilterMode FilterMode
		{
			get => (FilterMode)(((uint)Flags >> 13) & 3);
			set => Flags = (NJD_FLAG)(((uint)Flags & ~0x6000) | ((uint)value << 13));
		}

		public bool ClampU
		{
			get => (Flags & NJD_FLAG.ClampU) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.ClampU;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.ClampU);
				}
			}
		}

		public bool ClampV
		{
			get => (Flags & NJD_FLAG.ClampV) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.ClampV;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.ClampV);
				}
			}
		}

		public bool FlipU
		{
			get => (Flags & NJD_FLAG.FlipU) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.FlipU;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.FlipU);
				}
			}
		}

		public bool FlipV
		{
			get => (Flags & NJD_FLAG.FlipV) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.FlipV;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.FlipV);
				}
			}
		}

		public bool IgnoreSpecular
		{
			get => (Flags & NJD_FLAG.IgnoreSpecular) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.IgnoreSpecular;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.IgnoreSpecular);
				}
			}
		}

		public bool UseAlpha
		{
			get => (Flags & NJD_FLAG.UseAlpha) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.UseAlpha;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.UseAlpha);
				}
			}
		}

		public bool UseTexture
		{
			get => (Flags & NJD_FLAG.UseTexture) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.UseTexture;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.UseTexture);
				}
			}
		}

		public bool EnvironmentMap
		{
			get => (Flags & NJD_FLAG.UseEnv) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.UseEnv;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.UseEnv);
				}
			}
		}

		public bool DoubleSided
		{
			get => (Flags & NJD_FLAG.DoubleSide) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.DoubleSide;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.DoubleSide);
				}
			}
		}

		public bool FlatShading
		{
			get => (Flags & NJD_FLAG.UseFlat) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.UseFlat;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.UseFlat);
				}
			}
		}

		public bool IgnoreLighting
		{
			get => (Flags & NJD_FLAG.IgnoreLight) != 0;
			set
			{
				if (value)
				{
					Flags |= NJD_FLAG.IgnoreLight;
				}
				else
				{
					Flags = (NJD_FLAG)((uint)Flags & ~(uint)NJD_FLAG.IgnoreLight);
				}
			}
		}

		public AlphaInstruction DestinationAlpha
		{
			get => (AlphaInstruction)(((uint)Flags >> 26) & 7);
			set => Flags = (NJD_FLAG)(((uint)Flags & ~0x1C000000) | ((uint)value << 26));
		}

		public AlphaInstruction SourceAlpha
		{
			get => (AlphaInstruction)(((uint)Flags >> 29) & 7);
			set => Flags = (NJD_FLAG)(((uint)Flags & ~0xE0000000) | ((uint)value << 29));
		}

		#endregion

		public static int Size => 0x14;

		/// <summary>
		/// Create a new material.
		/// </summary>
		public NJS_MATERIAL()
		{
			DiffuseColor     = new Color(0xB2, 0xB2, 0xB2, 0xFF);
			SpecularColor    = Color.Transparent;
			UseAlpha         = true;
			UseTexture       = true;
			DoubleSided      = false;
			FlatShading      = false;
			IgnoreLighting   = false;
			ClampU           = false;
			ClampV           = false;
			FlipU            = false;
			FlipV            = false;
			EnvironmentMap   = false;
			DestinationAlpha = AlphaInstruction.InverseSourceAlpha;
			SourceAlpha      = AlphaInstruction.SourceAlpha;
		}

		/// <summary>
		/// Use this if you need a copy of a material.
		/// </summary>
		/// <param name="copy"></param>
		public NJS_MATERIAL(NJS_MATERIAL copy)
		{
			DiffuseColor  = copy.DiffuseColor;
			SpecularColor = copy.SpecularColor;
			Exponent      = copy.Exponent;
			TextureID     = copy.TextureID;
			Flags         = copy.Flags;
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
			DiffuseColor  = Color.FromRgba(ByteConverter.ToInt32(file, address));
			SpecularColor = Color.FromRgba(ByteConverter.ToInt32(file, address + 4));
			Exponent      = ByteConverter.ToSingle(file, address + 8);
			TextureID     = ByteConverter.ToInt32(file, address + 0xC);
			Flags         = (NJD_FLAG)ByteConverter.ToUInt32(file, address + 0x10);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(DiffuseColor.ToRgba()));
			result.AddRange(ByteConverter.GetBytes(SpecularColor.ToRgba()));
			result.AddRange(ByteConverter.GetBytes(Exponent));
			result.AddRange(ByteConverter.GetBytes(TextureID));
			result.AddRange(ByteConverter.GetBytes((uint)Flags));
			return result.ToArray();
		}

		public string ToStruct(string[] textures)
		{
			if (DiffuseColor == Color.Zero && SpecularColor == Color.Zero && Exponent == 0 && TextureID == 0 && Flags == 0)
				return "{ 0 }";
			StringBuilder result = new StringBuilder("{ ");
			result.Append(DiffuseColor.ToStruct());
			result.Append(", ");
			result.Append(SpecularColor.ToStruct());
			result.Append(", ");
			result.Append(Exponent.ToC());
			result.Append(", ");
			int callback = (int)(TextureID & 0xC0000000);
			int texid    = (int)(TextureID & ~0xC0000000);
			if (callback != 0)
				result.Append(((StructEnums.NJD_CALLBACK)callback).ToString().Replace(", ", " | ") + " | ");
			if (textures == null || texid >= textures.Length)
				result.Append(texid);
			else
				result.Append(textures[texid].MakeIdentifier());
			result.Append(", ");
			result.Append(((StructEnums.MaterialFlags)((uint)Flags & ~0x7F)).ToString().Replace(", ", " | "));
			if (UserFlags != 0)
				result.Append(" | 0x" + UserFlags.ToString("X"));
			result.Append(" }");
			return result.ToString();
		}

		public string ToStruct()
		{
			return ToStruct(null);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is NJS_MATERIAL))
				return false;
			NJS_MATERIAL other = (NJS_MATERIAL)obj;
			return DiffuseColor == other.DiffuseColor && SpecularColor == other.SpecularColor && Exponent == other.Exponent &&
			       TextureID == other.TextureID && Flags == other.Flags;
		}

		public override int GetHashCode()
		{
			return DiffuseColor.GetHashCode() ^ SpecularColor.GetHashCode() ^ Exponent.GetHashCode() ^ TextureID.GetHashCode() ^
			       Flags.GetHashCode();
		}

		object ICloneable.Clone() => Clone();

		public NJS_MATERIAL Clone() => (NJS_MATERIAL)MemberwiseClone();
	}
}