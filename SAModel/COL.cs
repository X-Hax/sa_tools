using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class COL
    {
        public BoundingSphere Bounds { get; set; }
        public ulong Unknown1 { get; private set; }
        public Object Model { get; set; }
        public int Unknown2 { get; set; }
        public int Flags { get; set; }
        public SurfaceFlags SurfaceFlags
        {
            get { return (SurfaceFlags)Flags; }
            set { Flags = (int)value; }
        }
        public string Name { get; set; }

        public static int Size(ModelFormat format)
        {
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    return 0x24;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    return 0x20;
            }
            return -1;
        }

        public COL()
        {
            Name = "col_" + Object.GenerateIdentifier();
            Bounds = new BoundingSphere();
        }

        public COL(byte[] file, int address, uint imageBase, ModelFormat format)
            : this(file, address, imageBase, format, new Dictionary<int, string>()) { }

        public COL(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "col_" + address.ToString("X8");
            Bounds = new BoundingSphere(file, address);
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    Unknown1 = ByteConverter.ToUInt64(file, 0x10);
                    uint tmpaddr = ByteConverter.ToUInt32(file, address + 0x18) - imageBase;
                    Model = new Object(file, (int)tmpaddr, imageBase, format, labels);
                    Unknown2 = ByteConverter.ToInt32(file, address + 0x1C);
                    Flags = ByteConverter.ToInt32(file, address + 0x20);
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    Flags = ByteConverter.ToInt32(file, address + 0x1C);
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x10) - imageBase;
                    Model = new Object(file, (int)tmpaddr, imageBase, format, labels, (SurfaceFlags & SAModel.SurfaceFlags.Solid) == SAModel.SurfaceFlags.Solid);
                    Unknown1 = ByteConverter.ToUInt64(file, 0x14);
                    break;
            }
        }

        public byte[] GetBytes(uint imageBase, uint modelptr, ModelFormat format)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            List<byte> result = new List<byte>();
            result.AddRange(Bounds.GetBytes());
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    result.AddRange(ByteConverter.GetBytes(Unknown1));
                    result.AddRange(ByteConverter.GetBytes(modelptr));
                    result.AddRange(ByteConverter.GetBytes(Unknown2));
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    result.AddRange(ByteConverter.GetBytes(modelptr));
                    result.AddRange(ByteConverter.GetBytes(Unknown1));
                    break;
            }
            result.AddRange(ByteConverter.GetBytes(Flags));
            return result.ToArray();
        }
    }
}