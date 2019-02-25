using System.Collections.Generic;
using System.Text;

namespace SonicRetro.SAModel
{
	public class GeoAnimData
	{
		public int Unknown1 { get; set; }
		public float Unknown2 { get; set; }
		public float Unknown3 { get; set; }
		public NJS_OBJECT Model { get; set; }
		public NJS_MOTION Animation { get; set; }
		public int Unknown4 { get; set; }

		public static int Size
		{
			get { return 0x18; }
		}

		public GeoAnimData(byte[] file, int address, uint imageBase, LandTableFormat format)
			: this(file, address, imageBase, format, new Dictionary<int, string>())
		{
		}

		public GeoAnimData(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels)
		{
			ModelFormat mfmt = 0;
			switch (format)
			{
				case LandTableFormat.SA1:
					mfmt = ModelFormat.Basic;
					break;
				case LandTableFormat.SADX:
					mfmt = ModelFormat.BasicDX;
					break;
				case LandTableFormat.SA2:
					mfmt = ModelFormat.Chunk;
					break;
			}
			Unknown1 = ByteConverter.ToInt32(file, address);
			Unknown2 = ByteConverter.ToSingle(file, address + 4);
			Unknown3 = ByteConverter.ToSingle(file, address + 8);
			Model = new NJS_OBJECT(file, (int)(ByteConverter.ToUInt32(file, address + 0xC) - imageBase), imageBase, mfmt);
			Animation = NJS_MOTION.ReadHeader(file, (int)(ByteConverter.ToUInt32(file, address + 0x10) - imageBase), imageBase, mfmt, labels);
			Unknown4 = ByteConverter.ToInt32(file, address + 0x14);
		}

		public byte[] GetBytes(uint imageBase, uint modelptr, uint animptr)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(Unknown1));
			result.AddRange(ByteConverter.GetBytes(Unknown2));
			result.AddRange(ByteConverter.GetBytes(Unknown3));
			result.AddRange(ByteConverter.GetBytes(modelptr));
			result.AddRange(ByteConverter.GetBytes(animptr));
			result.AddRange(ByteConverter.GetBytes(Unknown4));
			return result.ToArray();
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Unknown1.ToCHex());
			result.Append(", ");
			result.Append(Unknown2.ToC());
			result.Append(", ");
			result.Append(Unknown3.ToC());
			result.Append(", ");
			result.Append(Model != null ? "&" + Model.Name : "NULL");
			result.Append(", ");
			result.Append(Animation != null ? "&action_" + Animation.Name : "NULL");
			result.Append(", (NJS_TEXLIST *)");
			result.Append(Unknown4.ToCHex());
			result.Append(" }");
			return result.ToString();
		}
	}
}