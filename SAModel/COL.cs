using System;
using System.Collections.Generic;
using System.Text;

namespace SonicRetro.SAModel
{
	[Serializable]
	public class COL
	{
		public BoundingSphere Bounds { get; set; }
		public float WidthY { get; set; }
		public float WidthZ { get; set; }
		public NJS_OBJECT Model { get; set; }
		public int BlockBits { get; set; }
		public int Flags { get; set; }

		public SA1SurfaceFlags SurfaceFlags
		{
			get { return (SA1SurfaceFlags)Flags; }
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

		public COL(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, new Dictionary<int, string>(), attaches)
		{
		}

		public COL(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, labels, false, attaches)
		{
		}

		public COL(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels, bool? forceBasic, Dictionary<int, Attach> attaches)
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
					WidthY = ByteConverter.ToSingle(file, address + 0x10);
					WidthZ = ByteConverter.ToSingle(file, address + 0x14);
					uint tmpaddr = ByteConverter.ToUInt32(file, address + 0x18) - imageBase;
					Model = new NJS_OBJECT(file, (int)tmpaddr, imageBase, mfmt, labels, attaches);
					BlockBits = ByteConverter.ToInt32(file, address + 0x1C);
					Flags = ByteConverter.ToInt32(file, address + 0x20);
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					Flags = ByteConverter.ToInt32(file, address + 0x1C);
					if (!forceBasic.HasValue && Flags >= 0)
						mfmt = ModelFormat.Basic;
					tmpaddr = ByteConverter.ToUInt32(file, address + 0x10) - imageBase;
					Model = new NJS_OBJECT(file, (int)tmpaddr, imageBase, mfmt, labels, attaches);
					WidthZ = ByteConverter.ToInt32(file, address + 0x14);
					BlockBits = ByteConverter.ToInt32(file, address + 0x18);
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
					result.AddRange(ByteConverter.GetBytes(WidthY));
					result.AddRange(ByteConverter.GetBytes(WidthZ));
					result.AddRange(ByteConverter.GetBytes(modelptr));
					result.AddRange(ByteConverter.GetBytes(BlockBits));
					break;
				case LandTableFormat.SA2:
				case LandTableFormat.SA2B:
					result.AddRange(ByteConverter.GetBytes(modelptr));
					result.AddRange(ByteConverter.GetBytes(WidthZ));
					result.AddRange(ByteConverter.GetBytes(BlockBits));
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
					result.Append(WidthY.ToC());
					result.Append(", ");
					result.Append(WidthZ.ToC());
					result.Append(", ");
					result.Append(Model != null ? "&" + Model.Name : "NULL");
					result.Append(", ");
					result.AppendFormat(BlockBits.ToCHex());
					break;
				case LandTableFormat.SA2:
					result.Append(Model != null ? "&" + Model.Name : "NULL");
					result.Append(", ");
					result.Append(WidthZ.ToC());
					result.Append(", ");
					result.Append(BlockBits.ToCHex());
					break;
			}
			result.Append(", ");
			result.AppendFormat(Flags.ToCHex());
			result.Append(" }");
			return result.ToString();
		}
	}
}