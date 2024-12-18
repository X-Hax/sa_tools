using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
		/// <param name="dataType"></param>
		/// <param name="structType"></param>
		void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType);
		public byte[] GetBytes();
		void ToNJA(TextWriter writer, string vtype);
	}

	[Serializable]
	public class Vector3(float x, float y, float z) : IOVtx
	{
		public float X = x;
		public float Y = y;
		public float Z = z;

		public Vector3(byte[] file, int address) : this(ByteConverter.ToSingle(file, address), ByteConverter.ToSingle(file, address + 4), ByteConverter.ToSingle(file, address + 8))
		{
		}

		public void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Z);
		}

		public byte[] GetBytes()
		{
			List<byte> result = [];
			
			result.AddRange(ByteConverter.GetBytes(X));
			result.AddRange(ByteConverter.GetBytes(Y));
			result.AddRange(ByteConverter.GetBytes(Z));
			
			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");
			
			result.Append(X.ToC());
			result.Append(", ");
			result.Append(Y.ToC());
			result.Append(", ");
			result.Append(Z.ToC());
			result.Append(" }");
			
			return result.ToString();
		}
		public void ToNJA(TextWriter writer, string vtype)
		{
			writer.WriteLine($"\t{vtype}( {X.ToNJA()}, {Y.ToNJA()}, {Z.ToNJA()} ),");
		}
	}

	[Serializable]
	public class UV : IOVtx
	{
		public short X;
		public short Y;

		public float XF
		{
			get => X / 256f;
			set => X = (short)(value * 256);
		}
		
		public float YF
		{
			get => Y / 256f;
			set => Y = (short)(value * 256);
		}

		public UV(short x, short y)
		{
			X = x;
			Y = y;
		}

		public UV(float x, float y)
		{
			XF = x;
			YF = y;
		}

		public UV(byte[] file, int address)
		{
			X = ByteConverter.ToInt16(file, address);
			Y = ByteConverter.ToInt16(file, address + 2);
		}

		public void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType)
		{
			writer.Write(X);
			writer.Write(Y);
		}

		public byte[] GetBytes()
		{
			List<byte> result = [];
			
			result.AddRange(ByteConverter.GetBytes(X));
			result.AddRange(ByteConverter.GetBytes(Y));
			
			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");
			
			result.Append(X);
			result.Append(", ");
			result.Append(Y);
			result.Append(" }");
			
			return result.ToString();
		}
		
		public void ToNJA(TextWriter writer, string vtype)
		{
			writer.WriteLine($"\t{vtype}( {X}, {Y} ),");
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
		public byte Red;
		/// <summary>
		/// Green value
		/// </summary>
		public byte Green;
		/// <summary>
		/// Blue value
		/// </summary>
		public byte Blue;
		/// <summary>
		/// Alpha value
		/// </summary>
		public byte Alpha;

		/// <summary>
		/// Red float value. Ranges from 0 - 1
		/// </summary>
		public float RedF
		{
			get => Red / 255.0f;
			set => Red = (byte)Math.Round(value * 255);
		}
		
		/// <summary>
		/// Green float value. Ranges from 0 - 1
		/// </summary>
		public float GreenF
		{
			get => Green / 255.0f;
			set => Green = (byte)Math.Round(value * 255);
		}
		
		/// <summary>
		/// Blue float value. Ranges from 0 - 1
		/// </summary>
		public float BlueF
		{
			get => Blue / 255.0f;
			set => Blue = (byte)Math.Round(value * 255);
		}
		
		/// <summary>
		/// Alpha float value. Ranges from 0 - 1
		/// </summary>
		public float AlphaF
		{
			get => Alpha / 255.0f;
			set => Alpha = (byte)Math.Round(value * 255);
		}

		/// <summary>
		/// Returns the color as an RGBA integer
		/// </summary>
		public uint RGBA
		{
			get => (uint)(Red | (Green << 8) | (Blue << 16) | (Alpha << 24));
			set
			{
				Red = (byte)(value & 0xFF);
				Green = (byte)((value >> 8) & 0xFF);
				Blue = (byte)((value >> 16) & 0xFF);
				Alpha = (byte)(value >> 24);
			}
		}

		/// <summary>
		/// Returns the color as an ARGB integer
		/// </summary>
		public uint ARGB
		{
			get => (uint)(Alpha | (Red << 8) | (Green << 16) | (Blue << 24));
			set
			{
				Alpha = (byte)(value & 0xFF);
				Red = (byte)((value >> 8) & 0xFF);
				Green = (byte)((value >> 16) & 0xFF);
				Blue = (byte)(value >> 24);
			}
		}

		/// <summary>
		/// Returns the color as <see cref="System.Drawing.Color"/> object
		/// </summary>
		public System.Drawing.Color SystemCol
		{
			get => System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
			set
			{
				Alpha = value.A;
				Red = value.R;
				Green = value.G;
				Blue = value.B;
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
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		/// <summary>
		/// Create a new Color object from float values
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <param name="alpha"></param>
		public Color(float red, float green, float blue, float alpha) : this(0,0,0,0)
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
					var colorShort = ByteConverter.ToInt16(file, address);
					
					Red = (byte)((colorShort & 0xF800) >> 8);
					Green = (byte)((colorShort & 0x07E0) >> 3);
					Blue = (byte)((colorShort & 0x001F) << 3);
					
					endaddr = address + 2;
					return;
				case GCDataType.RGBA4:
					var colorShortA = ByteConverter.ToUInt16(file, address);
					
					// Multiplying all by 0x11, so that e.g. 0xF becomes 0xFF
					Red = (byte)(((colorShortA & 0xF000) >> 12) * 0x11);
					Green = (byte)(((colorShortA & 0x0F00) >> 8) * 0x11);
					Blue = (byte)(((colorShortA & 0x00F0) >> 4) * 0x11);
					Alpha = (byte)((colorShortA & 0x000F) * 0x11);
					
					endaddr = address + 2;
					return;
				case GCDataType.RGBA6:
					var colorInt = ByteConverter.ToUInt32(file, address);
					
					// Shifting all 2 less to the left, so that they are more accurate to the color that they should represent
					Red = (byte)((colorInt & 0xFC0000) >> 16);
					Green = (byte)((colorInt & 0x03F000) >> 10);
					Blue = (byte)((colorInt & 0x000FC0) >> 4);
					Alpha = (byte)((colorInt & 0x00003F) << 2);
					
					endaddr = address + 3;
					return;
				case GCDataType.RGB8:
				case GCDataType.RGBX8:
				case GCDataType.RGBA8:
					RGBA = ByteConverter.ToUInt32(file, address);
					
					if (dataType != GCDataType.RGBA8)
					{
						Alpha = 255;
					}

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
		/// <param name="dataType"></param>
		/// <param name="structType"></param>
		public void Write(BinaryWriter writer, GCDataType dataType, GCStructType structType)
		{
			switch (dataType)
			{
				case GCDataType.RGB8:
					writer.Write(Red);
					writer.Write(Green);
					writer.Write(Blue);
					writer.Write((byte)255);
					break;
				case GCDataType.RGBA8:
					writer.Write(Red);
					writer.Write(Green);
					writer.Write(Blue);
					writer.Write(Alpha);
					break;
				default:
					throw new ArgumentException($"{dataType} is not a valid output color type");
			}
		}

		public byte[] GetBytes()
		{
			List<byte> result = [];
			
			if (ByteConverter.BigEndian)
			{
				result.Add(Alpha);
				result.Add(Blue);
				result.Add(Green);
				result.Add(Red);
			}
			else
			{
				result.Add(Red);
				result.Add(Green);
				result.Add(Blue);
				result.Add(Alpha);
			}
			
			return result.ToArray();
		}

		public string ToStruct()
		{
			var result = new StringBuilder("{ ");
			
			result.Append($"0x{Alpha:X2}{Blue:X2}{Green:X2}{Red:X2}");
			result.Append(" }");
			
			return result.ToString();
		}
		
		public void ToNJA(TextWriter writer, string vtype)
		{
			writer.WriteLine($"\t{vtype}( {Red}, {Green}, {Blue}, {Alpha} ),");
		}
	}
}
