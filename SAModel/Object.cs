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

        public Object(byte[] file, int address, uint imageBase, bool DX)
        {
            Name = "object_" + address.ToString("X8");
            Flags = (ObjectFlags)BitConverter.ToInt32(file, address);
            int tmpaddr = BitConverter.ToInt32(file, address + 4);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Attach = new Attach(file, tmpaddr, imageBase, DX);
            }
            Position = new Vertex(file, address + 8);
            Rotation = new Rotation(file, address + 0x14);
            Scale = new Vertex(file, address + 0x20);
            Children = new List<Object>();
            Object child = null;
            tmpaddr = BitConverter.ToInt32(file, address + 0x2C);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                child = new Object(file, tmpaddr, imageBase, DX);
            }
            while (child != null)
            {
                Children.Add(child);
                child = child.Sibling;
            }
            tmpaddr = BitConverter.ToInt32(file, address + 0x30);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Sibling = new Object(file, tmpaddr, imageBase, DX);
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

        public byte[] GetBytes(uint imageBase, bool DX, out uint address)
        {
            for (int i = 1; i < Children.Count; i++)
                Children[i - 1].Sibling = Children[i];
            List<byte> result = new List<byte>();
            uint childaddr = 0;
            uint siblingaddr = 0;
            uint attachaddr = 0;
            if (Children.Count > 0)
            {
                childaddr = imageBase;
                result.AddRange(Children[0].GetBytes(imageBase, DX));
            }
            if (Sibling != null)
            {
                siblingaddr = imageBase + (uint)result.Count;
                result.AddRange(Sibling.GetBytes(imageBase + (uint)result.Count, DX));
            }
            if (Attach != null)
            {
                attachaddr = imageBase + (uint)result.Count;
                result.AddRange(Attach.GetBytes(imageBase + (uint)result.Count, DX));
            }
            address = imageBase + (uint)result.Count;
            result.AddRange(BitConverter.GetBytes((int)Flags));
            result.AddRange(BitConverter.GetBytes(attachaddr));
            result.AddRange(Position.GetBytes());
            result.AddRange(Rotation.GetBytes());
            result.AddRange(Scale.GetBytes());
            result.AddRange(BitConverter.GetBytes(childaddr));
            result.AddRange(BitConverter.GetBytes(siblingaddr));
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, bool DX)
        {
            uint address;
            return GetBytes(imageBase, DX, out address);
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
        }
    }
}