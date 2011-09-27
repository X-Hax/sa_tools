using System;
using System.Collections.Generic;

namespace SonicRetro.SAModel
{
    public class Object
    {
        public ObjectFlags Flags { get; set; }
        public Attach Attach { get; set; }
        public Vertex Position { get; set; }
        public Rotation Rotation { get; set; }
        public Vertex Scale { get; set; }
        public List<Object> Children { get; set; }
        internal Object Sibling { get; set; }
        public string Name { get; set; }

        public static int Size { get { return 0x34; } }

        public Object()
        {
            Name = "object_" + DateTime.Now.Ticks.ToString("X") + rand.Next(0, 256).ToString("X2");
            Position = new Vertex();
            Rotation = new Rotation();
            Scale = new Vertex(1, 1, 1);
        }

        public Object(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Name = "object_" + address.ToString("X8");
            Flags = (ObjectFlags)ByteConverter.ToInt32(file, address);
            int tmpaddr = ByteConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Attach = new Attach(file, tmpaddr, imageBase, format);
            }
            Position = new Vertex(file, address + 8);
            Rotation = new Rotation(file, address + 0x14);
            Scale = new Vertex(file, address + 0x20);
            Children = new List<Object>();
            Object child = null;
            tmpaddr = ByteConverter.ToInt32(file, address + 0x2C);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                child = new Object(file, tmpaddr, imageBase, format);
            }
            while (child != null)
            {
                Children.Add(child);
                child = child.Sibling;
            }
            tmpaddr = ByteConverter.ToInt32(file, address + 0x30);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Sibling = new Object(file, tmpaddr, imageBase, format);
            }
        }

        public Object(Dictionary<string, Dictionary<string, string>> INI, string groupname)
        {
            Dictionary<string, string> group = INI[groupname];
            Name = groupname;
            Flags = (ObjectFlags)Enum.Parse(typeof(ObjectFlags), group["Flags"]);
            if (group.ContainsKey("Attach"))
                Attach = new Attach(INI, group["Attach"]);
            Position = new Vertex(group["Position"]);
            Rotation = new Rotation(group["Rotation"]);
            Scale = new Vertex(group["Scale"]);
            Children = new List<Object>();
            if (group.ContainsKey("Children"))
            {
                string[] chldrn = group["Children"].Split(',');
                foreach (string item in chldrn)
                    Children.Add(new Object(INI, item));
            }
        }

        public static Object LoadFromFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C444D314153u)
                return new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA1);
            else if (magic == 0x00004C444D324153u)
                return new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA2);
            else
                throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
        }

        public static bool CheckModelFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C444D314153u)
                return true;
            else if (magic == 0x00004C444D324153u)
                return true;
            else
                return false;
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, Dictionary<string, uint> attachaddrs, out uint address)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            for (int i = 1; i < Children.Count; i++)
                Children[i - 1].Sibling = Children[i];
            List<byte> result = new List<byte>();
            uint childaddr = 0;
            uint siblingaddr = 0;
            uint attachaddr = 0;
            byte[] tmpbyte;
            if (Children.Count > 0)
            {
                result.Align(4);
                result.AddRange(Children[0].GetBytes(imageBase, format, out childaddr));
                childaddr += imageBase;
            }
            if (Sibling != null)
            {
                result.Align(4);
                tmpbyte = Sibling.GetBytes(imageBase + (uint)result.Count, format, out siblingaddr);
                siblingaddr += imageBase + (uint)result.Count;
                result.AddRange(tmpbyte);
            }
            if (Attach != null)
            {
                if (attachaddrs.ContainsKey(Attach.Name))
                    attachaddr = attachaddrs[Attach.Name];
                else
                {
                    result.Align(4);
                    tmpbyte = Attach.GetBytes(imageBase + (uint)result.Count, format, out attachaddr);
                    attachaddr += imageBase + (uint)result.Count;
                    result.AddRange(tmpbyte);
                    attachaddrs.Add(Attach.Name, attachaddr);
                }
            }
            result.Align(4);
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes((int)Flags));
            result.AddRange(ByteConverter.GetBytes(attachaddr));
            result.AddRange(Position.GetBytes());
            result.AddRange(Rotation.GetBytes());
            result.AddRange(Scale.GetBytes());
            result.AddRange(ByteConverter.GetBytes(childaddr));
            result.AddRange(ByteConverter.GetBytes(siblingaddr));
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, out uint address)
        {
            return GetBytes(imageBase, format, new Dictionary<string, uint>(), out address);
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format)
        {
            uint address;
            return GetBytes(imageBase, format, out address);
        }

        public void Save(Dictionary<string, Dictionary<string, string>> INI)
        {
            Dictionary<string, string> group = new Dictionary<string, string>();
            group.Add("Flags", Flags.ToString());
            if (Attach != null)
            {
                group.Add("Attach", Attach.Name);
                Attach.Save(INI);
            }
            group.Add("Position", Position.ToString());
            group.Add("Rotation", Rotation.ToString());
            group.Add("Scale", Scale.ToString());
            if (Children.Count > 0)
            {
                List<string> chldrn = new List<string>();
                foreach (Object child in Children)
                {
                    chldrn.Add(child.Name);
                    child.Save(INI);
                }
                group.Add("Children", string.Join(",", chldrn.ToArray()));
            }
            if (!INI.ContainsKey(Name))
                INI.Add(Name, group);
        }

        public void SaveToFile(string filename, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            switch (format)
            {
                case ModelFormat.SA1:
                    file.AddRange(ByteConverter.GetBytes(0x00004C444D314153u));
                    uint addr = 0;
                    byte[] mdl = GetBytes(0x10, ModelFormat.SA1, out addr);
                    file.AddRange(ByteConverter.GetBytes(addr + 0x10));
                    file.Align(0x10);
                    file.AddRange(mdl);
                    break;
                case ModelFormat.SADX:
                    throw new ArgumentException("Cannot save SADX format models to file!", "format");
                case ModelFormat.SA2:
                    file.AddRange(ByteConverter.GetBytes(0x00004C444D314153u));
                    addr = 0;
                    mdl = GetBytes(0x10, ModelFormat.SA2, out addr);
                    file.AddRange(ByteConverter.GetBytes(addr + 0x10));
                    file.Align(0x10);
                    file.AddRange(mdl);
                    break;
                case ModelFormat.SA2B:
                    throw new ArgumentException("Cannot save SA2B format levels to file!", "format");
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }

        public Object[] GetObjects()
        {
            List<Object> result = new List<Object>() { this };
            foreach (Object item in Children)
                result.AddRange(item.GetObjects());
            return result.ToArray();
        }

        internal static Random rand = new Random();
    }
}