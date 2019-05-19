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
					case ParameterType.IndexAttributeFlags:
						IndexAttributeParameter index_param = new IndexAttributeParameter();
						index_param.Read(file, address + 4);
						Parameters.Add(index_param);
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
						vert.PositionIndex = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.Position16BitIndex) ?
							ByteConverter.ToInt16(file, address += 2) : file[address++];
					}
					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasNormal))
					{
						vert.NormalIndex = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.Normal16BitIndex) ?
							ByteConverter.ToInt16(file, address += 2) : file[address++];
					}
					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasColor))
					{
						vert.Color0Index = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.Color16BitIndex) ?
							ByteConverter.ToInt16(file, address += 2) : file[address++];
					}
					if (index_parameter.IndexAttributes.HasFlag(IndexAttributeParameter.IndexAttributeFlags.HasUV))
					{
						vert.UVIndex = index_parameter.IndexAttributes.HasFlag(
							IndexAttributeParameter.IndexAttributeFlags.UV16BitIndex) ?
							ByteConverter.ToInt16(file, address += 2) : file[address++];
					}

					prim.Vertices.Add(vert);
				}

				Primitives.Add(prim);
			}
		}
	}
}
