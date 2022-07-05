using System;
using System.IO;
using System.Text;

namespace SAModel.GC
{
	/// <summary>
	/// An interface used to write the structs
	/// </summary>
	public interface IOVtx
	{
		/// <summary>
		/// Writes the struct
		/// </summary>
		/// <param name="writer">The output stream</param>
		/// <param name="attrib"></param>
		void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType);
	}

	[Serializable]
	public class Vector3 : IOVtx
	{
		public float x;
		public float y;
		public float z;

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(byte[] file, int address)
		{
			x = ByteConverter.ToSingle(file, address);
			y = ByteConverter.ToSingle(file, address + 4);
			z = ByteConverter.ToSingle(file, address + 8);
		}

		public void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType)
		{
			writer.Write(x);
			writer.Write(y);
			writer.Write(z);
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(x);
			result.Append(", ");
			result.Append(y);
			result.Append(", ");
			result.Append(z);
			result.Append(" }");
			return result.ToString();
		}
	}

	[Serializable]
	public class UV : IOVtx
	{
		public short x;
		public short y;

		public float XF
		{
			get
			{
				return x / 256f;
			}
			set
			{
				x = (short)(value * 256);
			}
		}
		public float YF
		{
			get
			{
				return y / 256f;
			}
			set
			{
				y = (short)(value * 256);
			}
		}

		public UV(short x, short y)
		{
			this.x = x;
			this.y = y;
		}

		public UV(float x, float y)
		{
			XF = x;
			YF = y;
		}

		public UV(byte[] file, int address)
		{
			x = ByteConverter.ToInt16(file, address);
			y = ByteConverter.ToInt16(file, address + 2);
		}

		public void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType)
		{
			writer.Write(x);
			writer.Write(y);
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(x);
			result.Append(", ");
			result.Append(y);
			result.Append(" }");
			return result.ToString();
		}
	}

	/// <summary>
	/// Color struct for the gamecube vertex data
	/// </summary>
	[Serializable]
	public class Color : IOVtx
	{
		/// <summary>
		/// Red value
		/// </summary>
		public byte red;
		/// <summary>
		/// Green value
		/// </summary>
		public byte green;
		/// <summary>
		/// Blue value
		/// </summary>
		public byte blue;
		/// <summary>
		/// Alpha value
		/// </summary>
		public byte alpha;

		/// <summary>
		/// Red float value. Ranges from 0 - 1
		/// </summary>
		public float RedF
		{
			get
			{
				return red / 255.0f;
			}
			set
			{
				red = (byte)Math.Round(value * 255);
			}
		}
		/// <summary>
		/// Green float value. Ranges from 0 - 1
		/// </summary>
		public float GreenF
		{
			get
			{
				return green / 255.0f;
			}
			set
			{
				green = (byte)Math.Round(value * 255);
			}
		}
		/// <summary>
		/// Blue float value. Ranges from 0 - 1
		/// </summary>
		public float BlueF
		{
			get
			{
				return blue / 255.0f;
			}
			set
			{
				blue = (byte)Math.Round(value * 255);
			}
		}
		/// <summary>
		/// Alpha float value. Ranges from 0 - 1
		/// </summary>
		public float AlphaF
		{
			get
			{
				return alpha / 255.0f;
			}
			set
			{
				alpha = (byte)Math.Round(value * 255);
			}
		}

		/// <summary>
		/// Returns the color as an RGBA integer
		/// </summary>
		public uint RGBA
		{
			get
			{
				return (uint)(red | (green << 8) | (blue << 16) | (alpha << 24));
			}
			set
			{
				red = (byte)(value & 0xFF);
				green = (byte)((value >> 8) & 0xFF);
				blue = (byte)((value >> 16) & 0xFF);
				alpha = (byte)(value >> 24);
			}
		}

		/// <summary>
		/// Returns the color as an ARGB integer
		/// </summary>
		public uint ARGB
		{
			get
			{
				return (uint)(alpha | (red << 8) | (green << 16) | (blue << 24));
			}
			set
			{
				alpha = (byte)(value & 0xFF);
				red = (byte)((value >> 8) & 0xFF);
				green = (byte)((value >> 16) & 0xFF);
				blue = (byte)(value >> 24);
			}
		}

		/// <summary>
		/// Returns the color as <see cref="System.Drawing.Color"/> object
		/// </summary>
		public System.Drawing.Color SystemCol
		{
			get
			{
				return System.Drawing.Color.FromArgb(alpha, red, green, blue);
			}
			set
			{
				alpha = value.A;
				red = value.R;
				green = value.G;
				blue = value.B;
			}
		}

		public Color()
		{
			RGBA = uint.MaxValue;
		}

		/// <summary>
		/// Create a new Color object from byte values
		/// </summary>
		/// <param name="red">Red color value</param>
		/// <param name="green">Green color value</param>
		/// <param name="blue">Blue color value</param>
		/// <param name="alpha">Alpha color value</param>
		public Color(byte red, byte green, byte blue, byte alpha)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
			this.alpha = alpha;
		}

		/// <summary>
		/// Create a new Color object from float values
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <param name="alpha"></param>
		public Color(float red, float green, float blue, float alpha) : this((byte)0,0,0,0)
		{
			RedF = red;
			GreenF = green;
			BlueF = blue;
			AlphaF = alpha;
		}

		public Color(byte[] file, int address, GCDataType dataType, out int endaddr)
		{
			switch (dataType)
			{
				case GCDataType.RGB565:
					short colorShort = ByteConverter.ToInt16(file, address);
					red = (byte)((colorShort & 0xF800) >> 8);
					green = (byte)((colorShort & 0x07E0) >> 3);
					blue = (byte)((colorShort & 0x001F) << 3);
					endaddr = address + 2;
					return;
				case GCDataType.RGBA4:
					ushort colorShortA = ByteConverter.ToUInt16(file, address);
					// multiplying all by 0x11, so that e.g. 0xF becomes 0xFF
					red = (byte)(((colorShortA & 0xF000) >> 12) * 0x11);
					green = (byte)(((colorShortA & 0x0F00) >> 8) * 0x11);
					blue = (byte)(((colorShortA & 0x00F0) >> 4) * 0x11);
					alpha = (byte)((colorShortA & 0x000F) * 0x11);
					endaddr = address + 2;
					return;
				case GCDataType.RGBA6:
					uint colorInt = ByteConverter.ToUInt32(file, address);
					// shifting all 2 less to the left, so that they are more accurate to the color that they should represent
					red = (byte)((colorInt & 0xFC0000) >> 16);
					green = (byte)((colorInt & 0x03F000) >> 10);
					blue = (byte)((colorInt & 0x000FC0) >> 4);
					alpha = (byte)((colorInt & 0x00003F) << 2);
					endaddr = address + 3;
					return;
				case GCDataType.RGB8:
				case GCDataType.RGBX8:
				case GCDataType.RGBA8:
					RGBA = ByteConverter.ToUInt32(file, address);
					if (dataType != GCDataType.RGBA8)
						alpha = 255;
					endaddr = address + 4;
					return;
				default:
					throw new ArgumentException($"{dataType} is not a valid color type");
			}
		}

		/// <summary>
		/// Write the color data to a stream
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attrib"></param>
		public void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType)
		{
			switch (dataType)
			{
				case GCDataType.RGB8:
					writer.Write(red);
					writer.Write(green);
					writer.Write(blue);
					writer.Write((byte)255);
					break;
				case GCDataType.RGBA8:
					writer.Write(red);
					writer.Write(green);
					writer.Write(blue);
					writer.Write(alpha);
					break;
				default:
					throw new ArgumentException($"{dataType} is not a valid output color type");
			}
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
					result.Append(red);
					result.Append(", ");
					result.Append(green);
					result.Append(", ");
					result.Append(blue);
					result.Append(", ");
					result.Append(alpha);
					result.Append(" }");
			return result.ToString();
		}
	}
}
