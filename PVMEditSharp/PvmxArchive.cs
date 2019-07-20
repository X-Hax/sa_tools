using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVMEditSharp
{
	static class PvmxArchive
	{
		const int FourCC = 0x584D5650; // 'PVMX'
		const byte Version = 1;

		enum dictionary_field : byte
		{
			none,
			/// <summary>
			/// 32-bit integer global index
			/// </summary>
			global_index,
			/// <summary>
			/// Null-terminated file name
			/// </summary>
			name,
			/// <summary>
			/// Two 32-bit integers defining width and height
			/// </summary>
			dimensions,
		}

		public static bool Is(byte[] file)
		{
			return file.Length > 4 && BitConverter.ToInt32(file, 0) == FourCC;
		}

		public static List<PvmxTextureInfo> GetTextures(byte[] file)
		{
			if (!Is(file)) throw new FormatException("File is not a PVMX archive.");
			if (file[4] != Version) throw new FormatException("Incorrect PVMX archive version.");
			int off = 5;
			List<PvmxTextureInfo> textures = new List<PvmxTextureInfo>();
			dictionary_field type;
			for (type = (dictionary_field)file[off++]; type != dictionary_field.none; type = (dictionary_field)file[off++])
			{
				PvmxTextureInfo tex = new PvmxTextureInfo();
				while (type != dictionary_field.none)
				{
					switch (type)
					{
						case dictionary_field.global_index:
							tex.GlobalIndex = BitConverter.ToUInt32(file, off);
							off += sizeof(uint);
							break;

						case dictionary_field.name:
							int count = 0;
							while (file[off + count] != 0)
								count++;
							tex.Name = Path.ChangeExtension(Encoding.UTF8.GetString(file, off, count), null);
							off += count + 1;
							break;

						case dictionary_field.dimensions:
							System.Drawing.Size size = new System.Drawing.Size();
							size.Width = BitConverter.ToInt32(file, off);
							off += sizeof(int);
							size.Height = BitConverter.ToInt32(file, off);
							off += sizeof(int);
							break;

						default:
							break;
					}

					type = (dictionary_field)file[off++];
				}

				ulong offset = BitConverter.ToUInt64(file, off);
				off += sizeof(ulong);
				ulong length = BitConverter.ToUInt64(file, off);
				off += sizeof(ulong);

				using (MemoryStream ms = new MemoryStream(file, (int)offset, (int)length))
					tex.Image = new System.Drawing.Bitmap(ms);

				textures.Add(tex);
			}
			return textures;
		}

		public static void Save(Stream str, IEnumerable<PvmxTextureInfo> textures)
		{
			BinaryWriter bw = new BinaryWriter(str);
			bw.Write(FourCC);
			bw.Write(Version);
			List<(long off, byte[] data)> texdata = new List<(long off, byte[] data)>();
			foreach (PvmxTextureInfo tex in textures)
			{
				bw.Write((byte)dictionary_field.global_index);
				bw.Write(tex.GlobalIndex);
				bw.Write((byte)dictionary_field.name);
				bw.Write(tex.Name.ToCharArray());
				bw.Write(new[] { '.', 'p', 'n', 'g' });
				bw.Write((byte)0);
				if (tex.Dimensions.HasValue)
				{
					bw.Write((byte)dictionary_field.dimensions);
					bw.Write(tex.Dimensions.Value.Width);
					bw.Write(tex.Dimensions.Value.Height);
				}
				bw.Write((byte)dictionary_field.none);
				long size;
				using (MemoryStream ms = new MemoryStream())
				{
					tex.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					texdata.Add((str.Position, ms.ToArray()));
					size = ms.Length;
				}
				bw.Write(0ul);
				bw.Write(size);
			}
			bw.Write((byte)dictionary_field.none);
			foreach ((long off, byte[] data) in texdata)
			{
				long pos = str.Position;
				str.Position = off;
				bw.Write(pos);
				str.Position = pos;
				bw.Write(data);
			}
		}
	}
}
