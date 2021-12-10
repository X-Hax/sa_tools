using System.Collections.Generic;
using System.Text;

namespace SAModel
{
	public class GeoAnimData
	{
		public float Frame { get; set; }
		public float Step { get; set; }
		public float MaxFrame { get; set; }
		public NJS_OBJECT Model { get; set; }
		public NJS_MOTION Animation { get; set; }
		public uint TexlistPointer { get; set; } // Unused

		public string ActionName { get; set; }
		public static int Size
		{
			get { return 0x18; }
		}

		public GeoAnimData(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, Attach> attaches)
			: this(file, address, imageBase, format, new Dictionary<int, string>(), attaches)
		{
		}

		public GeoAnimData(NJS_OBJECT model, NJS_MOTION animation)
		{
			Model = model;
			Animation = animation;
		}

		public GeoAnimData(NJS_ACTION action)
		{
			Model = action.Model;
			Animation = action.Animation;
			ActionName = action.Name;
		}

		public GeoAnimData(byte[] file, int address, uint imageBase, LandTableFormat format, Dictionary<int, string> labels, Dictionary<int, Attach> attaches)
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
			Frame = ByteConverter.ToSingle(file, address);
			Step = ByteConverter.ToSingle(file, address + 4);
			MaxFrame = ByteConverter.ToSingle(file, address + 8);
			Model = new NJS_OBJECT(file, (int)(ByteConverter.ToUInt32(file, address + 0xC) - imageBase), imageBase, mfmt, labels, attaches);
			int actionaddr = (int)(ByteConverter.ToUInt32(file, address + 0x10) - imageBase);
			int motionaddr = (int)(ByteConverter.ToUInt32(file, actionaddr + 4) - imageBase);
			Animation = NJS_MOTION.ReadDirect(file, Model.CountAnimated(), motionaddr, imageBase, labels, attaches);
			TexlistPointer = ByteConverter.ToUInt32(file, address + 0x14);
			if (labels.ContainsKey(actionaddr)) ActionName = labels[actionaddr];
			else
			{
				NJS_ACTION action = new NJS_ACTION(file, actionaddr, imageBase, mfmt, labels, attaches);
				ActionName = action.Name;
				labels.Add(actionaddr + (int)imageBase, ActionName);
			}
		}

		public byte[] GetBytes(uint imageBase, uint modelptr, uint animptr)
		{
			List<byte> result = new List<byte>();
			result.AddRange(ByteConverter.GetBytes(Frame));
			result.AddRange(ByteConverter.GetBytes(Step));
			result.AddRange(ByteConverter.GetBytes(MaxFrame));
			result.AddRange(ByteConverter.GetBytes(modelptr));
			result.AddRange(ByteConverter.GetBytes(animptr));
			result.AddRange(ByteConverter.GetBytes(TexlistPointer));
			return result.ToArray();
		}

		public string ToStruct()
		{
			StringBuilder result = new StringBuilder("{ ");
			result.Append(Frame.ToC());
			result.Append(", ");
			result.Append(Step.ToC());
			result.Append(", ");
			result.Append(MaxFrame.ToC());
			result.Append(", ");
			result.Append(Model != null ? "&" + Model.Name : "NULL");
			result.Append(", ");
			result.Append(Animation != null ? "&" + ActionName : "NULL");
			result.Append(", (NJS_TEXLIST *)");
			result.Append(TexlistPointer.ToCHex());
			result.Append(" }");
			return result.ToString();
		}
	}
}