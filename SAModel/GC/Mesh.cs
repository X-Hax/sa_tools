using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonicRetro.SAModel.GC
{
	public class Mesh
	{
		public List<Parameter> Parameters { get; private set; }
		public List<Primitive> Primitives { get; private set; }

		public Mesh(byte[] file, int address, uint imageBase, IndexAttributeParameter index = null)
		{
			Parameters = new List<Parameter>();
			Primitives = new List<Primitive>();

			int parameters_offset = (int)(ByteConverter.ToInt32(file, address) - imageBase);
			int parameters_count = ByteConverter.ToInt32(file, address + 4);

			int primitives_offset = (int)(ByteConverter.ToInt32(file, address + 8) - imageBase);
			int primitives_size = ByteConverter.ToInt32(file, address + 12);

			ReadParameters(file, parameters_offset, parameters_count);

			IndexAttributeParameter index_param = (IndexAttributeParameter)Parameters.Find(x => x.ParameterType == ParameterType.IndexAttributeFlags);
			if (index_param == null)
			{
				index_param = index;
			}

			ReadPrimitives(file, primitives_offset, primitives_size, index_param);
		}

		private void ReadParameters(byte[] file, int address, int count)
		{
			for (int i = 0; i < count; i++)
			{
				switch ((ParameterType)ByteConverter.ToInt32(file, address))
				{
					case ParameterType.VtxAttrFmt:
						VtxAttrFmtParameter vtx_param = new VtxAttrFmtParameter();
						vtx_param.Read(file, address + 4);
						Parameters.Add(vtx_param);
						break;
					case ParameterType.IndexAttributeFlags:
						IndexAttributeParameter index_param = new IndexAttributeParameter();
						index_param.Read(file, address + 4);
						Parameters.Add(index_param);
						break;
					case ParameterType.Lighting:
						LightingParameter light_param = new LightingParameter();
						light_param.Read(file, address + 4);
						Parameters.Add(light_param);
						break;
					case ParameterType.BlendAlpha:
						BlendAlphaParameter blend_param = new BlendAlphaParameter();
						blend_param.Read(file, address + 4);
						Parameters.Add(blend_param);
						break;
					case ParameterType.AmbientColor:
						AmbientColorParameter ambient_param = new AmbientColorParameter();
						ambient_param.Read(file, address + 4);
						Parameters.Add(ambient_param);
						break;
					case ParameterType.Texture:
						TextureParameter texture_param = new TextureParameter();
						texture_param.Read(file, address + 4);
						Parameters.Add(texture_param);
						break;
					case ParameterType.Unknown_9:
						Unknown9Parameter unk9_param = new Unknown9Parameter();
						unk9_param.Read(file, address + 4);
						Parameters.Add(unk9_param);
						break;
					case ParameterType.TexCoordGen:
						TexCoordGenParameter mip_param = new TexCoordGenParameter();
						mip_param.Read(file, address + 4);
						Parameters.Add(mip_param);
						break;
				}

				address += 8;
			}
		}

		private void ReadPrimitives(byte[] file, int address, int size, IndexAttributeParameter index_parameter)
		{
			int end_pos = address + size;

			while (address < end_pos)
			{
				if (file[address] == 0)
				{
					address++;
					continue;
				}

				Primitive prim = new Primitive((GXPrimitiveType)file[address]);

				short raw_index_count = ByteConverter.ToInt16(file, address + 1);
				byte[] raw_index_bytes = ByteConverter.GetBytes(raw_index_count);

				int real_index_count = ByteConverter.ToInt16(new byte[] { raw_index_bytes[1], raw_index_bytes[0] }, 0);

				address += 3;

				for (int i = 0; i < real_index_count; i++)
				{
					Vertex vert = new Vertex();

					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasPosition))
					{
						bool is_16bit = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.Position16BitIndex);

						ushort raw_pos_index = is_16bit ? ByteConverter.ToUInt16(file, address) : file[address];

						if (!is_16bit)
						{
							vert.PositionIndex = raw_pos_index;
							address++;
						}
						else
						{
							byte[] pos_bytes = BitConverter.GetBytes(raw_pos_index);
							Array.Reverse(pos_bytes);

							vert.PositionIndex = BitConverter.ToUInt16(pos_bytes, 0);
							address += 2;
						}
					}
					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasNormal))
					{
						bool is_16bit = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.Normal16BitIndex);

						ushort raw_nrm_index = is_16bit ? ByteConverter.ToUInt16(file, address) : file[address];

						if (!is_16bit)
						{
							vert.NormalIndex = raw_nrm_index;
							address++;
						}
						else
						{
							byte[] nrm_bytes = BitConverter.GetBytes(raw_nrm_index);
							Array.Reverse(nrm_bytes);

							vert.Color0Index = BitConverter.ToUInt16(nrm_bytes, 0);
							address += 2;
						}
					}
					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasColor))
					{
						bool is_16bit = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.Color16BitIndex);

						ushort raw_col_index = is_16bit ? ByteConverter.ToUInt16(file, address) : file[address];

						if (!is_16bit)
						{
							vert.Color0Index = raw_col_index;
							address++;
						}
						else
						{
							byte[] col_bytes = BitConverter.GetBytes(raw_col_index);
							Array.Reverse(col_bytes);

							vert.Color0Index = BitConverter.ToUInt16(col_bytes, 0);
							address += 2;
						}
					}
					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasUV))
					{
						bool is_16bit = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.UV16BitIndex);

						ushort raw_tex_index = is_16bit ? ByteConverter.ToUInt16(file, address) : file[address];

						if (!is_16bit)
						{
							vert.UVIndex = raw_tex_index;
							address++;
						}
						else
						{
							byte[] tex_bytes = BitConverter.GetBytes(raw_tex_index);
							Array.Reverse(tex_bytes);

							vert.UVIndex = BitConverter.ToUInt16(tex_bytes, 0);
							address += 2;
						}
					}

					prim.Vertices.Add(vert);
				}

				Primitives.Add(prim);
			}
		}
	}
}
