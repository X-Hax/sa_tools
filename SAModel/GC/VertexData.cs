using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SonicRetro.SAModel;

namespace SonicRetro.SAModel.GC
{
    public class VertexData
    {
		public List<VertexAttribute> VertexAttributes { get; private set; }
        private List<GXVertexAttribute> m_Attributes;

        public List<Vector3> Positions { get; private set; }
        public List<Vector3> Normals { get; private set; }
        public List<Color> Color_0 { get; private set; }
        public List<Color> Color_1 { get; private set; }
        public List<Vector2> TexCoord_0 { get; private set; }
        public List<Vector2> TexCoord_1 { get; private set; }
        public List<Vector2> TexCoord_2 { get; private set; }
        public List<Vector2> TexCoord_3 { get; private set; }
        public List<Vector2> TexCoord_4 { get; private set; }
        public List<Vector2> TexCoord_5 { get; private set; }
        public List<Vector2> TexCoord_6 { get; private set; }
        public List<Vector2> TexCoord_7 { get; private set; }

        public VertexData()
        {
			VertexAttributes = new List<VertexAttribute>();
            m_Attributes = new List<GXVertexAttribute>();
            Positions = new List<Vector3>();
            Normals = new List<Vector3>();
            Color_0 = new List<Color>();
            Color_1 = new List<Color>();
            TexCoord_0 = new List<Vector2>();
            TexCoord_1 = new List<Vector2>();
            TexCoord_2 = new List<Vector2>();
            TexCoord_3 = new List<Vector2>();
            TexCoord_4 = new List<Vector2>();
            TexCoord_5 = new List<Vector2>();
            TexCoord_6 = new List<Vector2>();
            TexCoord_7 = new List<Vector2>();
        }

        public bool CheckAttribute(GXVertexAttribute attribute)
        {
            if (m_Attributes.Contains(attribute))
                return true;
            else
                return false;
        }

        public object GetAttributeData(GXVertexAttribute attribute)
        {
            if (!CheckAttribute(attribute))
                return null;

            switch (attribute)
            {
                case GXVertexAttribute.Position:
                    return Positions;
                case GXVertexAttribute.Normal:
                    return Normals;
                case GXVertexAttribute.Color0:
                    return Color_0;
                case GXVertexAttribute.Color1:
                    return Color_1;
                case GXVertexAttribute.Tex0:
                    return TexCoord_0;
                case GXVertexAttribute.Tex1:
                    return TexCoord_1;
                case GXVertexAttribute.Tex2:
                    return TexCoord_2;
                case GXVertexAttribute.Tex3:
                    return TexCoord_3;
                case GXVertexAttribute.Tex4:
                    return TexCoord_4;
                case GXVertexAttribute.Tex5:
                    return TexCoord_5;
                case GXVertexAttribute.Tex6:
                    return TexCoord_6;
                case GXVertexAttribute.Tex7:
                    return TexCoord_7;
                default:
                    throw new ArgumentException("attribute");
            }
        }

        public void SetAttributeData(GXVertexAttribute attribute, object data)
        {
            if (!CheckAttribute(attribute))
                m_Attributes.Add(attribute);

            switch (attribute)
            {
                case GXVertexAttribute.Position:
                    if (data.GetType() != typeof(List<Vector3>))
                        throw new ArgumentException("position data");
                    else
                        Positions = (List<Vector3>)data;
                    break;
                case GXVertexAttribute.Normal:
                    if (data.GetType() != typeof(List<Vector3>))
                        throw new ArgumentException("normal data");
                    else
                        Normals = (List<Vector3>)data;
                    break;
                case GXVertexAttribute.Color0:
                    if (data.GetType() != typeof(List<Color>))
                        throw new ArgumentException("color0 data");
                    else
                        Color_0 = (List<Color>)data;
                    break;
                case GXVertexAttribute.Color1:
                    if (data.GetType() != typeof(List<Color>))
                        throw new ArgumentException("color1 data");
                    else
                        Color_1 = (List<Color>)data;
                    break;
                case GXVertexAttribute.Tex0:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord0 data");
                    else
                        TexCoord_0 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex1:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord1 data");
                    else
                        TexCoord_1 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex2:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord2 data");
                    else
                        TexCoord_2 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex3:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord3 data");
                    else
                        TexCoord_3 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex4:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord4 data");
                    else
                        TexCoord_4 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex5:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord5 data");
                    else
                        TexCoord_5 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex6:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord6 data");
                    else
                        TexCoord_6 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex7:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord7 data");
                    else
                        TexCoord_7 = (List<Vector2>)data;
                    break;
            }
        }

        public void SetAttributesFromList(List<GXVertexAttribute> attributes)
        {
            m_Attributes = new List<GXVertexAttribute>(attributes);
        }

		public void Load(byte[] file, uint address, uint imageBase)
		{
			VertexAttribute attrib = new VertexAttribute(file, address, imageBase);

			while (attrib.Attribute != GXVertexAttribute.Null + 8)
			{
				VertexAttributes.Add(attrib);

				object attrib_data = GetVertexData(file, attrib);
				if (attrib_data != null)
					SetAttributeData(attrib.Attribute, attrib_data);

				address += 16;
				attrib = new VertexAttribute(file, address, imageBase);
			}
		}

		private object GetVertexData(byte[] file, VertexAttribute attribute)
		{
			object attribute_data = null;

			switch (attribute.Attribute)
			{
				case GXVertexAttribute.Position:
					switch (attribute.ComponentCount)
					{
						case GXComponentCount.Position_XY:
							attribute_data = LoadVec2Data(file, attribute);
							break;
						case GXComponentCount.Position_XYZ:
							attribute_data = LoadVec3Data(file, attribute);
							break;
					}
					break;
				case GXVertexAttribute.Normal:
					switch (attribute.ComponentCount)
					{
						case GXComponentCount.Normal_XYZ:
							attribute_data = LoadVec3Data(file, attribute);
							break;
					}
					break;
				case GXVertexAttribute.Color0:
				case GXVertexAttribute.Color1:
					attribute_data = LoadColorData(file, attribute);
					break;
				case GXVertexAttribute.Tex0:
				case GXVertexAttribute.Tex1:
				case GXVertexAttribute.Tex2:
				case GXVertexAttribute.Tex3:
				case GXVertexAttribute.Tex4:
				case GXVertexAttribute.Tex5:
				case GXVertexAttribute.Tex6:
				case GXVertexAttribute.Tex7:
					switch (attribute.ComponentCount)
					{
						case GXComponentCount.TexCoord_S:
							attribute_data = LoadSingleFloat(file, attribute);
							break;
						case GXComponentCount.TexCoord_ST:
							attribute_data = LoadVec2Data(file, attribute);
							break;
					}
					break;
			}

			return attribute_data;
		}

		private List<float> LoadSingleFloat(byte[] file, VertexAttribute attribute)
		{
			List<float> floatList = new List<float>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.Unsigned8:
						byte compu81 = file[cur_address];
						float compu81Float = (float)compu81 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(compu81Float);

						cur_address++;
						break;
					case GXDataType.Signed8:
						sbyte comps81 = (sbyte)file[cur_address];
						float comps81Float = (float)comps81 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(comps81Float);

						cur_address++;
						break;
					case GXDataType.Unsigned16:
						ushort compu161 = ByteConverter.ToUInt16(file, cur_address);
						float compu161Float = (float)compu161 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(compu161Float);

						cur_address += 2;
						break;
					case GXDataType.Signed16:
						short comps161 = ByteConverter.ToInt16(file, cur_address);
						float comps161Float = (float)comps161 / (float)(1 << attribute.FractionalBitCount);
						floatList.Add(comps161Float);

						cur_address += 2;
						break;
					case GXDataType.Float32:
						floatList.Add(ByteConverter.ToSingle(file, cur_address));

						cur_address += 4;
						break;
				}
			}

			return floatList;
		}

		private List<Vector2> LoadVec2Data(byte[] file, VertexAttribute attribute)
		{
			List<Vector2> vec2List = new List<Vector2>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.Unsigned8:
						byte compu81 = file[cur_address];
						byte compu82 = file[cur_address + 1];
						float compu81Float = (float)compu81 / (float)(1 << attribute.FractionalBitCount);
						float compu82Float = (float)compu82 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(compu81Float, compu82Float));

						cur_address += 2;
						break;
					case GXDataType.Signed8:
						sbyte comps81 = (sbyte)file[cur_address];
						sbyte comps82 = (sbyte)file[cur_address + 1];
						float comps81Float = (float)comps81 / (float)(1 << attribute.FractionalBitCount);
						float comps82Float = (float)comps82 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(comps81Float, comps82Float));

						cur_address += 2;
						break;
					case GXDataType.Unsigned16:
						ushort compu161 = ByteConverter.ToUInt16(file, cur_address);
						ushort compu162 = ByteConverter.ToUInt16(file, cur_address + 2);
						float compu161Float = (float)compu162 / (float)(1 << attribute.FractionalBitCount);
						float compu162Float = (float)compu162 / (float)(1 << attribute.FractionalBitCount);
						vec2List.Add(new Vector2(compu161Float, compu162Float));

						cur_address += 4;
						break;
					case GXDataType.Signed16:
						short comps161 = ByteConverter.ToInt16(file, cur_address);
						short comps162 = ByteConverter.ToInt16(file, cur_address + 2);
						float comps161Float = (float)comps161 / 255f; //(float)(1 << (int)Math.Pow(2, attribute.FractionalBitCount));
						float comps162Float = (float)comps162 / 255f; //(float)(1 << (int)Math.Pow(2, attribute.FractionalBitCount));
						vec2List.Add(new Vector2(comps161Float, comps162Float));

						cur_address += 4;
						break;
					case GXDataType.Float32:
						vec2List.Add(new Vector2(ByteConverter.ToSingle(file, cur_address),
												 ByteConverter.ToSingle(file, cur_address + 4)));

						cur_address += 8;
						break;
				}
			}

			return vec2List;
		}

		private List<Vector3> LoadVec3Data(byte[] file, VertexAttribute attribute)
		{
			List<Vector3> vec3List = new List<Vector3>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.Unsigned8:
						byte compu81 = file[cur_address];
						byte compu82 = file[cur_address + 1];
						byte compu83 = file[cur_address + 2];
						float compu81Float = (float)compu81 / (float)(1 << attribute.FractionalBitCount);
						float compu82Float = (float)compu82 / (float)(1 << attribute.FractionalBitCount);
						float compu83Float = (float)compu83 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(compu81Float, compu82Float, compu83Float));

						cur_address += 3;
						break;
					case GXDataType.Signed8:
						sbyte comps81 = (sbyte)file[cur_address];
						sbyte comps82 = (sbyte)file[cur_address + 1];
						sbyte comps83 = (sbyte)file[cur_address + 2];
						float comps81Float = (float)comps81 / (float)(1 << attribute.FractionalBitCount);
						float comps82Float = (float)comps82 / (float)(1 << attribute.FractionalBitCount);
						float comps83Float = (float)comps83 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(comps81Float, comps82Float, comps83Float));

						cur_address += 3;
						break;
					case GXDataType.Unsigned16:
						ushort compu161 = ByteConverter.ToUInt16(file, cur_address);
						ushort compu162 = ByteConverter.ToUInt16(file, cur_address + 2);
						ushort compu163 = ByteConverter.ToUInt16(file, cur_address + 4);
						float compu161Float = (float)compu161 / (float)(1 << attribute.FractionalBitCount);
						float compu162Float = (float)compu162 / (float)(1 << attribute.FractionalBitCount);
						float compu163Float = (float)compu163 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(compu161Float, compu162Float, compu163Float));

						cur_address += 6;
						break;
					case GXDataType.Signed16:
						short comps161 = ByteConverter.ToInt16(file, cur_address);
						short comps162 = ByteConverter.ToInt16(file, cur_address + 2);
						short comps163 = ByteConverter.ToInt16(file, cur_address + 4);
						float comps161Float = (float)comps161 / (float)(1 << attribute.FractionalBitCount);
						float comps162Float = (float)comps162 / (float)(1 << attribute.FractionalBitCount);
						float comps163Float = (float)comps163 / (float)(1 << attribute.FractionalBitCount);
						vec3List.Add(new Vector3(comps161Float, comps162Float, comps163Float));

						cur_address += 6;
						break;
					case GXDataType.Float32:
						vec3List.Add(new Vector3(ByteConverter.ToSingle(file, cur_address),
												 ByteConverter.ToSingle(file, cur_address + 4),
												 ByteConverter.ToSingle(file, cur_address + 8)));

						cur_address += 12;
						break;
				}
			}

			return vec3List;
		}

		private List<Color> LoadColorData(byte[] file, VertexAttribute attribute)
		{
			List<Color> colorList = new List<Color>();
			int cur_address = attribute.DataOffset;

			for (int i = 0; i < attribute.DataCount; i++)
			{
				switch (attribute.DataType)
				{
					case GXDataType.RGB565:
						short colorShort = ByteConverter.ToInt16(file, cur_address);
						int r5 = (colorShort & 0xF800) >> 11;
						int g6 = (colorShort & 0x07E0) >> 5;
						int b5 = (colorShort & 0x001F);
						colorList.Add(new Color((float)r5 / 255.0f, (float)g6 / 255.0f, (float)b5 / 255.0f, 1.0f));

						cur_address += 2;
						break;
					case GXDataType.RGB8:
						byte r8 = file[cur_address];
						byte g8 = file[cur_address + 1];
						byte b8 = file[cur_address + 2];
						colorList.Add(new Color((float)r8 / 255.0f, (float)g8 / 255.0f, (float)b8 / 255.0f, 1.0f));

						cur_address += 4;
						break;
					case GXDataType.RGBX8:
						byte rx8 = file[cur_address];
						byte gx8 = file[cur_address + 1];
						byte bx8 = file[cur_address + 2];

						cur_address += 4;
						colorList.Add(new Color((float)rx8 / 255.0f, (float)gx8 / 255.0f, (float)bx8 / 255.0f, 1.0f));
						break;
					case GXDataType.RGBA4:
						short colorShortA = ByteConverter.ToInt16(file, cur_address);
						int r4 = (colorShortA & 0xF000) >> 12;
						int g4 = (colorShortA & 0x0F00) >> 8;
						int b4 = (colorShortA & 0x00F0) >> 4;
						int a4 = (colorShortA & 0x000F);

						cur_address += 2;
						colorList.Add(new Color((float)r4 / 255.0f, (float)g4 / 255.0f, (float)b4 / 255.0f, (float)a4 / 255.0f));
						break;
					case GXDataType.RGBA6:
						int colorInt = ByteConverter.ToInt32(file, cur_address);
						int r6 = (colorInt & 0xFC0000) >> 18;
						int ga6 = (colorInt & 0x03F000) >> 12;
						int b6 = (colorInt & 0x000FC0) >> 6;
						int a6 = (colorInt & 0x00003F);
						colorList.Add(new Color((float)r6 / 255.0f, (float)ga6 / 255.0f, (float)b6 / 255.0f, (float)a6 / 255.0f));

						cur_address += 4;
						break;
					case GXDataType.RGBA8:
						byte ra8 = file[cur_address];
						byte ga8 = file[cur_address + 1];
						byte ba8 = file[cur_address + 2];
						byte aa8 = file[cur_address + 3];
						colorList.Add(new Color((float)ra8, (float)ga8, (float)ba8, (float)aa8));

						cur_address += 4;
						break;
				}
			}

			return colorList;
		}

		public void WriteVertexAttributes(BinaryWriter writer, uint imageBase)
		{
			using (MemoryStream attributes = new MemoryStream())
			{
				BinaryWriter attrib_writer = new BinaryWriter(attributes);

				foreach (VertexAttribute v in VertexAttributes)
				{
					attrib_writer.Write((byte)(v.Attribute - 8));
					attrib_writer.Write(v.FractionalBitCount);
					attrib_writer.Write(v.Unknown1);
					attrib_writer.Write((int)(((int)v.DataType << 4) | ((int)v.ComponentCount)));
					attrib_writer.Write((int)(writer.BaseStream.Length + imageBase));

					switch (v.Attribute)
					{
						case GXVertexAttribute.Position:
							attrib_writer.Write((int)(Positions.Count * v.CalculateComponentSize()));

							foreach (Vector3 vec3pos in Positions)
							{
								vec3pos.Write(writer, v);
							}
							break;
						case GXVertexAttribute.Normal:
							attrib_writer.Write((int)(Normals.Count * v.CalculateComponentSize()));

							foreach (Vector3 vec3nrm in Normals)
							{
								vec3nrm.Write(writer, v);
							}
							break;
						case GXVertexAttribute.Color0:
							attrib_writer.Write((int)(Color_0.Count * v.CalculateComponentSize()));

							foreach (Color color in Color_0)
							{
								color.Write(writer, v);
							}
							break;
						case GXVertexAttribute.Tex0:
							attrib_writer.Write((int)(TexCoord_0.Count * v.CalculateComponentSize()));

							foreach (Vector2 vec2tex in TexCoord_0)
							{
								vec2tex.Write(writer, v);
							}
							break;
					}
				}

				// A vertex attribute with a value of 255 (Null) terminates the attribute list
				attrib_writer.Write((byte)GXVertexAttribute.Null);
				attrib_writer.Write((byte)0);
				attrib_writer.Write((short)0);
				attrib_writer.Write((int)0);
				attrib_writer.Write((int)0);
				attrib_writer.Write((int)0);

				writer.Seek(0, SeekOrigin.Begin);
				writer.Write((int)(writer.BaseStream.Length + imageBase));
				writer.Seek(0, SeekOrigin.End);

				writer.Write(attributes.ToArray());
			}
		}
	}
}
