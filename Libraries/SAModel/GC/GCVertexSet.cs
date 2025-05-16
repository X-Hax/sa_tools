using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SAModel.GC
{
	/// <summary>
	/// A vertex data set, which can hold various data
	/// </summary>
	[Serializable]
	public class GCVertexSet
	{
		/// <summary>
		/// The type of vertex data that is stored
		/// </summary>
		public readonly GCVertexAttribute Attribute;

		/// <summary>
		/// The datatype as which the data is stored
		/// </summary>
		public readonly GCDataType DataType;

		/// <summary>
		/// The structure in which the data is stored
		/// </summary>
		public readonly GCStructType StructType;

		/// <summary>
		/// The size of a single object in the list in bytes
		/// </summary>
		public uint StructSize
		{
			get
			{
				uint numComponents = StructType switch
				{
					GCStructType.Position_XY or GCStructType.TexCoord_ST => 2,
					GCStructType.Position_XYZ or GCStructType.Normal_XYZ => 3,
					_ => 1
				};

				switch (DataType)
				{
					case GCDataType.Unsigned8:
					case GCDataType.Signed8:
						return numComponents;
					case GCDataType.Unsigned16:
					case GCDataType.Signed16:
					case GCDataType.RGB565:
					case GCDataType.RGBA4:
						return numComponents * 2;
					case GCDataType.Float32:
					case GCDataType.RGBA8:
					case GCDataType.RGB8:
					case GCDataType.RGBX8:
					default:
						return numComponents * 4;
				}
			}
		}

		/// <summary>
		/// The vertex data
		/// </summary>
		public List<IOVtx> Data;

		/// <summary>
		/// The address of the vertex attribute (gets set after writing)
		/// </summary>
		private uint _dataAddress;

		public string DataName { get; set; }

		/// <summary>
		/// Creates a new empty vertex attribute using the default struct setups
		/// </summary>
		/// <param name="attributeType">The attribute type of the vertex attribute</param>
		public GCVertexSet(GCVertexAttribute attributeType)
		{
			Attribute = attributeType;

			switch (Attribute)
			{
				case GCVertexAttribute.Position:
					DataType = GCDataType.Float32;
					StructType = GCStructType.Position_XYZ;
					break;
				case GCVertexAttribute.Normal:
					DataType = GCDataType.Float32;
					StructType = GCStructType.Normal_XYZ;
					break;
				case GCVertexAttribute.Color0:
					DataType = GCDataType.RGBA8;
					StructType = GCStructType.Color_RGBA;
					break;
				case GCVertexAttribute.Tex0:
					DataType = GCDataType.Signed16;
					StructType = GCStructType.TexCoord_ST;
					break;
				default:
					throw new ArgumentException($"Datatype { Attribute } is not a valid vertex type for SA2");
			}

			Data = [];
		}

		/// <summary>
		/// Create a custom empty vertex attribute
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="dataType"></param>
		/// <param name="structType"></param>
		public GCVertexSet(GCVertexAttribute attribute, GCDataType dataType, GCStructType structType)
		{
			Attribute = attribute;
			DataType = dataType;
			StructType = structType;
			Data = [];
		}

		public GCVertexSet(byte[] file, uint address, uint imageBase)
		: this(file, address, imageBase, new Dictionary<int, string>())
		{
		}

		/// <summary>
		/// Read an entire vertex data set
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The starting address of the file</param>
		/// <param name="imageBase">The image base of the addresses</param>
		public GCVertexSet(byte[] file, uint address, uint imageBase, Dictionary<int, string> labels)
		{
			Attribute = (GCVertexAttribute)file[address];
			if (Attribute == GCVertexAttribute.Null)
			{
				return;
			}

			var structure = ByteConverter.ToUInt32(file, (int)address + 4);
			StructType = (GCStructType)(structure & 0x0F);
			DataType = (GCDataType)((structure >> 4) & 0x0F);
			
			if (file[address + 1] != StructSize)
			{
				throw new Exception($"Read structure size doesnt match calculated structure size: {file[address + 1]} != {StructSize}");
			}

			// Reading the data
			int count = ByteConverter.ToUInt16(file, (int)address + 2);
			var tempAddr = (int)(ByteConverter.ToUInt32(file, (int)address + 8) - imageBase);

			Data = [];

			switch (Attribute)
			{
				case GCVertexAttribute.Position:
					if (labels.TryGetValue(tempAddr, out var positionName))
					{
						DataName = positionName;
					}
					else
					{
						DataName = $"position_{tempAddr:X8}";
					}

					for (var i = 0; i < count; i++)
					{
						Data.Add(new Vector3(file, tempAddr));
						tempAddr += 12;
					}
					break;
				case GCVertexAttribute.Normal:
					if (labels.TryGetValue(tempAddr, out var noramlName))
					{
						DataName = noramlName;
					}
					else
					{
						DataName = $"normal_{tempAddr:X8}";
					}

					for (var i = 0; i < count; i++)
					{
						Data.Add(new Vector3(file, tempAddr));
						tempAddr += 12;
					}
					break;
				case GCVertexAttribute.Color0:
					if (labels.TryGetValue(tempAddr, out var colorName))
					{
						DataName = colorName;
					}
					else
					{
						DataName = $"vcolor_{tempAddr:X8}";
					}

					for (var i = 0; i < count; i++)
					{
						Data.Add(new Color(file, tempAddr, DataType, out tempAddr));
					}
					break;
				case GCVertexAttribute.Tex0:
					if (labels.TryGetValue(tempAddr, out var texName))
					{
						DataName = texName;
					}
					else
					{
						DataName = $"uv_{tempAddr:X8}";
					}

					for (var i = 0; i < count; i++)
					{
						Data.Add(new UV(file, tempAddr));
						tempAddr += 4;
					}
					break;
				default:
					throw new ArgumentException($"Attribute type not valid sa2 type: {Attribute}");
			}
		}

		/// <summary>
		/// Writes the vertex data to the current writing location
		/// </summary>
		/// <param name="writer">The output stream</param>
		public void WriteData(BinaryWriter writer)
		{
			_dataAddress = (uint)writer.BaseStream.Length;

			foreach(var vtx in Data)
			{
				vtx.Write(writer, DataType, StructType);
			}
		}

		/// <summary>
		/// Writes the vertex attribute information <br/>
		/// Assumes that <see cref="WriteData(BinaryWriter)"/> has been called prior
		/// </summary>
		/// <param name="writer">The output stream</param>
		/// <param name="imageBase">The image base</param>
		/// <param name="njOffsets"></param>
		public void WriteAttribute(BinaryWriter writer, uint imageBase, List<uint> njOffsets)
		{
			if (_dataAddress == 0)
			{
				throw new Exception("Data has not been written yet!");
			}

			// POF0 Offsets
			njOffsets.Add((uint)(writer.BaseStream.Position + 8));

			writer.Write((byte)Attribute);
			writer.Write((byte)StructSize);
			writer.Write((ushort)Data.Count);
			
			var structure = (uint)StructType;
			structure |= (uint)((byte)DataType << 4);
			
			writer.Write(structure);
			writer.Write(_dataAddress + imageBase);
			writer.Write((uint)(Data.Count * StructSize));

			_dataAddress = 0;
		}

		public byte[] GetBytes(uint dataAddress)
		{
			List<byte> result =
			[
				(byte)Attribute,
				(byte)StructSize
			];
			
			result.AddRange(ByteConverter.GetBytes((ushort)Data.Count));
			
			var structure = (uint)StructType;
			structure |= (uint)((byte)DataType << 4);
			
			result.AddRange(ByteConverter.GetBytes(structure));
			result.AddRange(ByteConverter.GetBytes(dataAddress));
			result.AddRange(ByteConverter.GetBytes((uint)(Data.Count * StructSize)));
			
			return result.ToArray();
		}
		
		public string ToStruct()
		{
			var result = new StringBuilder("{ ");
			
			result.Append((byte)Attribute);
			result.Append(", ");
			result.Append(StructSize);
			result.Append(", ");
			result.Append(Data.Count);
			result.Append(", ");
			
			var structure = (uint)StructType;
			structure |= (uint)((byte)DataType << 4);
			
			result.Append(structure);
			result.Append(", ");
			result.Append(Data != null ? DataName : "NULL");
			result.Append(", ");
			result.Append((uint)(Data.Count * StructSize));
			result.Append(" }");
			
			return result.ToString();
		}

		// WIP
		public void ToNJA(TextWriter writer)
		{
			string vertType = null;
			string vertType2 = null;
			
			switch (Attribute)
			{
				case GCVertexAttribute.Position:
					vertType = "POINT";
					vertType2 = "POSITION   ";
					break;
				case GCVertexAttribute.Normal:
					vertType = "NORMAL";
					vertType2 = "NORMAL     ";
					break;
				case GCVertexAttribute.Color0:
					vertType = "COLOR";
					vertType2 = "COLOR0     ";
					break;
				case GCVertexAttribute.Tex0:
					vertType = "UV";
					vertType2 = "TEX0       ";
					break;
			}
			
			writer.WriteLine($"{vertType2}{DataName}[]");
			writer.WriteLine("START");
			
			foreach (var vtx in Data)
			{
				vtx.ToNJA(writer, vertType);
			}
			
			writer.WriteLine($"END{Environment.NewLine}");
		}
		
		public void RefToNJA(TextWriter writer)
		{
			var vertType = Attribute switch
			{
				GCVertexAttribute.Position => "POSITION",
				GCVertexAttribute.Normal => "NORMAL",
				GCVertexAttribute.Color0 => "COLOR0",
				GCVertexAttribute.Tex0 => "TEX0",
				_ => null
			};

			writer.WriteLine($"\tVertAttr     {vertType},");
			writer.WriteLine($"\tElementSize  {StructSize},");
			writer.WriteLine($"\tPoints       {Data.Count},");
			writer.WriteLine($"\tType         ( {StructType}, {DataType} ),");
			writer.WriteLine($"\tName         {DataName},");
			writer.WriteLine($"\tCheckSize    {Data.Count * StructSize},{Environment.NewLine}");
		}

		public GCVertexSet Clone()
		{
			var result = (GCVertexSet)MemberwiseClone();
			return result;
		}
	}
}
