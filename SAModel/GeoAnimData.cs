using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class GeoAnimData
    {
        public string Name { get; set; }
        public int Unknown1 { get; set; }
        public float Unknown2 { get; set; }
        public float Unknown3 { get; set; }
        public Object Model { get; set; }
        public Animation Animation { get; set; }
        public int Unknown4 { get; set; }

        public static int Size { get { return 0x18; } }

        public GeoAnimData(byte[] file, int address, uint imageBase, ModelFormat format)
            : this(file, address, imageBase, format, new Dictionary<int, string>()) { }

        public GeoAnimData(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "anim_" + address.ToString("X8");
            Unknown1 = ByteConverter.ToInt32(file, address);
            Unknown2 = ByteConverter.ToSingle(file, address + 4);
            Unknown3 = ByteConverter.ToSingle(file, address + 8);
            Model = new Object(file, (int)(ByteConverter.ToUInt32(file, address + 0xC) - imageBase), imageBase, format);
            Animation = Animation.ReadHeader(file, (int)(ByteConverter.ToUInt32(file, address + 0x10) - imageBase), imageBase, format, labels);
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
    }
}