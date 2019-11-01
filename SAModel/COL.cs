using System;
using System.Collections.Generic;
using System.Text;

namespace SonicRetro.SAModel
{
	[Serializable]
	public class COL
	{
		public BoundingSphere Bounds { get; set; }
		public int Unknown1 { get; set; }
		public int Unknown2 { get; set; }
		public NJS_OBJECT Model { get; set; }
		public int Unknown3 { get; set; }
		public int Flags { get; set; }

		public SurfaceFlags SurfaceFlags
		{
			get { return (SurfaceFlags)Flags; }
			set { Flags = (int)value; }
		}

		public static int Size(LandTableFormat format)
		{
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					return 0x24;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					return 0x20;
				default:
					throw new ArgumentOutOfRangeException("format");
			}
		}

		public COL()
		{
			Bounds = new BoundingSphere();
		}

		public COL(byte[] file, int address, uint imageBase, LandTableFormat format)
			: this(file, address, imageBase, format, new Dictionary<int, string>())
		{
		}

		public COL(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels)
			: this(file, address, imageBase, format, labels, false)
		{
		}

		public COL(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels, bool? forceBasic)
		{
			Bounds = new BoundingSphere(file, address);
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
					if (forceBasic.HasValue && forceBasic.Value)
						mfmt = ModelFormat.Basic;
					else
						mfmt = ModelFormat.Chunk;
					break;
				case LandTableFormat.SA2B:
					if (forceBasic.HasValue && forceBasic.Value)
						mfmt = ModelFormat.Basic;
					else
						mfmt = ModelFormat.GC;
					break;
			}
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					Unknown1 = ByteConverter.ToInt32(file, address + 0x10);
					Unknown2 = ByteConverter.ToInt32(file, address + 0x14);
					uint tmpaddr = ByteConverter.ToUInt32(file, address + 0x18) - imageBase;
					Model = new NJS_OBJECT(file, (int)tmpaddr, imageBase, mfmt, labels);
					Unknown3 = ByteConverter.ToInt32(file, address + 0x1C);
					Flags = ByteConverter.ToInt32(file, address + 0x20);
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					Flags = ByteConverter.ToInt32(file, address + 0x1C);
					if (!forceBasic.HasValue && Flags >= 0)
						mfmt = ModelFormat.Basic;
					tmpaddr = ByteConverter.ToUInt32(file, address + 0x10) - imageBase;
					Model = new NJS_OBJECT(file, (int)tmpaddr, imageBase, mfmt, labels);
					Unknown2 = ByteConverter.ToInt32(file, address + 0x14);
					Unknown3 = ByteConverter.ToInt32(file, address + 0x18);
					break;
			}
		}

		public byte[] GetBytes(uint imageBase, uint modelptr, LandTableFormat format)
		{
			List<byte> result = new List<byte>();
			result.AddRange(Bounds.GetBytes());
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					result.AddRange(ByteConverter.GetBytes(Unknown1));
					result.AddRange(ByteConverter.GetBytes(Unknown2));
					result.AddRange(ByteConverter.GetBytes(modelptr));
					result.AddRange(ByteConverter.GetBytes(Unknown3));
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					result.AddRange(ByteConverter.GetBytes(modelptr));
					result.AddRange(ByteConverter.GetBytes(Unknown2));
					result.AddRange(ByteConverter.GetBytes(Unknown3));
					break;
			}
			result.AddRange(ByteConverter.GetBytes(Flags));
			return result.ToArray();
		}

		public string ToStruct(LandTableFormat format)
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Bounds.ToStruct());
			result.Append(", ");
			switch (format)
			{
				case LandTableFormat.SA1:
				case LandTableFormat.SADX:
					result.Append(Unknown1.ToCHex());
					result.Append(", ");
					result.Append(Unknown2.ToCHex());
					result.Append(", ");
					result.Append(Model != null ? "&" + Model.Name : "NULL");
					result.Append(", ");
					result.AppendFormat(Unknown3.ToCHex());
					break;
				case LandTableFormat.SA2:
					result.Append(Model != null ? "&" + Model.Name : "NULL");
					result.Append(", ");
					result.Append(Unknown2.ToCHex());
					result.Append(", ");
					result.Append(Unknown3.ToCHex());
					break;
			}
			result.Append(", ");
			result.AppendFormat(Flags.ToCHex());
			result.Append(" }");
			return result.ToString();
		}
	}
}