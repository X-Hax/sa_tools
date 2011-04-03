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
        public Object Child { get; set; }
        public Object Sibling { get; set; }

        public static int Size { get { return 0x34; } }

        public Object(byte[] file, int address, uint imageBase, bool DX)
        {
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
            tmpaddr = BitConverter.ToInt32(file, address + 0x2C);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Child = new Object(file, tmpaddr, imageBase, DX);
            }
            tmpaddr = BitConverter.ToInt32(file, address + 0x30);
            if (tmpaddr != 0)
            {
                tmpaddr = (int)unchecked((uint)tmpaddr - imageBase);
                Sibling = new Object(file, tmpaddr, imageBase, DX);
            }
        }

        public Object[] GetChildren()
        {
            List<Object> result = new List<Object>();
            if (Child != null)
            {
                Object obj = Child;
                result.Add(obj);
                while (obj.Sibling != null)
                {
                    obj = obj.Sibling;
                    result.Add(obj);
                }
            }
            return result.ToArray();
        }
    }
}
