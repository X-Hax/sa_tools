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
            get
            {
                return (SurfaceFlags)Flags;
            }
            set
            {
                Flags = (int)value;
            }
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
            Name = "col_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
            Bounds = new BoundingSphere();
        }

        public COL(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Name = "col_" + address.ToString("X8");
            Bounds = new BoundingSphere(file, address);
            switch (format)
            {
                case ModelFormat.SA1:
                case ModelFormat.SADX:
                    Unknown1 = ByteConverter.ToUInt64(file, 0x10);
                    uint tmpaddr = ByteConverter.ToUInt32(file, address + 0x18) - imageBase;
                    Model = new Object(file, (int)tmpaddr, imageBase, format);
                    Unknown2 = ByteConverter.ToInt32(file, address + 0x1C);
                    Flags = ByteConverter.ToInt32(file, address + 0x20);
                    break;
                case ModelFormat.SA2:
                case ModelFormat.SA2B:
                    tmpaddr = ByteConverter.ToUInt32(file, address + 0x10) - imageBase;
                    Model = new Object(file, (int)tmpaddr, imageBase, format);
                    Unknown1 = ByteConverter.ToUInt64(file, 0x14);
                    Flags = ByteConverter.ToInt32(file, address + 0x1C);
                    break;
            }
        }

        public COL(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Name = groupname;
            Dictionary<string, string> group = INI[groupname];
            Bounds = new BoundingSphere(group["Bounds"]);
            Unknown1 = ulong.Parse(group["Unknown1"], System.Globalization.NumberStyles.HexNumber);
            Model = new Object(INI, group["Model"]);
            Unknown2 = int.Parse(group["Unknown2"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            Flags = int.Parse(group["Flags"], System.Globalization.NumberStyles.HexNumber);
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

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Bounds", Bounds.ToString());
            group.Add("Unknown1", Unknown1.ToString("X16"));
            group.Add("Model", Model.Name);
            Model.Save(INI);
            group.Add("Unknown2", Unknown2.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            group.Add("Flags", Flags.ToString("X8"));
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }
    }
}