using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SAModel.GC
{
	/// <summary>
	/// A vertex data set, which can hold various data
	/// </summary>
	[Serializable]
	public class VertexAttribute
	{
		/// <summary>
		/// The type of vertex data that is stored
		/// </summary>
		public readonly GCVertexAttribute attribute;

		/// <summary>
		/// The datatype as which the data is stored
		/// </summary>
		public readonly GCDataType dataType;

		/// <summary>
		/// The structure in which the data is stored
		/// </summary>
		public readonly GCStructType structType;

		/// <summary>
		/// The size of a single object in the list in bytes
		/// </summary>
		public uint StructSize
		{
			get
			{
				uint num_components = 1;

				switch (structType)
				{
					case GCStructType.Position_XY:
					case GCStructType.TexCoord_ST:
						num_components = 2;
						break;
					case GCStructType.Position_XYZ:
					case GCStructType.Normal_XYZ:
						num_components = 3;
						break;
				}

				switch (dataType)
				{
					case GCDataType.Unsigned8:
					case GCDataType.Signed8:
						return num_components;
					case GCDataType.Unsigned16:
					case GCDataType.Signed16:
					case GCDataType.RGB565:
					case GCDataType.RGBA4:
						return num_components * 2;
					case GCDataType.Float32:
					case GCDataType.RGBA8:
					case GCDataType.RGB8:
					case GCDataType.RGBX8:
					default:
						return num_components * 4;
				}
			}
		}

		/// <summary>
		/// The vertex data
		/// </summary>
		public readonly List<IOVtx> data;

		/// <summary>
		/// The address of the vertex attribute (gets set after writing
		/// </summary>
		private uint dataAddress;

		/// <summary>
		/// Creates a new empty vertex attribute using the default struct setups
		/// </summary>
		/// <param name="attributeType">The attribute type of the vertex attribute</param>
		public VertexAttribute(GCVertexAttribute attributeType)
		{
			attribute = attributeType;

			switch (attribute)
			{
				case GCVertexAttribute.Position:
					dataType = GCDataType.Float32;
					structType = GCStructType.Position_XYZ;
					break;
				case GCVertexAttribute.Normal:
					dataType = GCDataType.Float32;
					structType = GCStructType.Normal_XYZ;
					break;
				case GCVertexAttribute.Color0:
					dataType = GCDataType.RGBA8;
					structType = GCStructType.Color_RGBA;
					break;
				case GCVertexAttribute.Tex0:
					dataType = GCDataType.Signed16;
					structType = GCStructType.TexCoord_ST;
					break;
				default:
					throw new ArgumentException($"Datatype { attribute } is not a valid vertex type for SA2");
			}

			data = new List<IOVtx>();
		}

		/// <summary>
		/// Create a custom empty vertex attribute
		/// </summary>
		/// <param name="attribute"></param>
		/// <param name="dataType"></param>
		/// <param name="structType"></param>
		/// <param name="fractionalBitCount"></param>
		public VertexAttribute(GCVertexAttribute attribute, GCDataType dataType, GCStructType structType)
		{
			this.attribute = attribute;
			this.dataType = dataType;
			this.structType = structType;
			data = new List<IOVtx>();
		}

		/// <summary>
		/// Read an entire vertex data set
		/// </summary>
		/// <param name="file">The files contents</param>
		/// <param name="address">The starting address of the file</param>
		/// <param name="imageBase">The image base of the addresses</param>
		public VertexAttribute(byte[] file, uint address, uint imageBase)
		{
			attribute = (GCVertexAttribute)file[address];
			if (attribute == GCVertexAttribute.Null) return;

			uint structure = ByteConverter.ToUInt32(file, (int)address + 4);
			structType = (GCStructType)(structure & 0x0F);
			dataType = (GCDataType)((structure >> 4) & 0x0F);
			if (file[address + 1] != StructSize)
			{
				throw new Exception($"Read structure size doesnt match calculated structure size: {file[address + 1]} != {StructSize}");
			}

			// reading the data
			int count = ByteConverter.ToUInt16(file, (int)address + 2);
			int tmpaddr = (int)(ByteConverter.ToUInt32(file, (int)address + 8) - imageBase);

			data = new List<IOVtx>();

			switch (attribute)
			{
				case GCVertexAttribute.Position:
				case GCVertexAttribute.Normal:
					for (int i = 0; i < count; i++)
					{
						data.Add(new Vector3(file, tmpaddr));
						tmpaddr += 12;
					}
					break;
				case GCVertexAttribute.Color0:
					for (int i = 0; i < count; i++)
					{
						data.Add(new Color(file, tmpaddr, dataType, out tmpaddr));
					}
					break;
				case GCVertexAttribute.Tex0:
					for (int i = 0; i < count; i++)
					{
						data.Add(new UV(file, tmpaddr));
						tmpaddr += 4;
					}
					break;
				default:
					throw new ArgumentException($"Attribute type not valid sa2 type: {attribute}");
			}
		}

		/// <summary>
		/// Writes the vertex data to the current writing location
		/// </summary>
		/// <param name="writer">The output stream</param>
		/// <param name="imagebase">The imagebase</param>
		public void WriteData(BinaryWriter writer)
		{
			dataAddress = (uint)writer.BaseStream.Length;

			foreach(IOVtx vtx in data)
			{
				vtx.Write(writer, dataType, structType);
			}
		}

		/// <summary>
		/// Writes the vertex attribute information <br/>
		/// Assumes that <see cref="WriteData(BinaryWriter)"/> has been called prior
		/// </summary>
		/// <param name="writer">The output stream</param>
		/// <param name="imagebase">The imagebase</param>
		public void WriteAttribute(BinaryWriter writer, uint imagebase)
		{
			if (dataAddress == 0)
				throw new Exception("Data has not been written yet!");

			writer.Write((byte)attribute);
			writer.Write((byte)StructSize);
			writer.Write((ushort)data.Count);
			uint structure = (uint)structType;
			structure |= (uint)((byte)dataType << 4);
			writer.Write(structure);
			writer.Write(dataAddress + imagebase);
			writer.Write((uint)(data.Count * StructSize));

			dataAddress = 0;
		}
	}
}
