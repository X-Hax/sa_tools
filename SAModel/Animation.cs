using System;
using System.Collections.Generic;
using System.Xml;

namespace SonicRetro.SAModel
{
    public class Animation
    {
        public int Frames { get; set; }
        public string Name { get; set; }

        public Dictionary<int, AnimModelData> Models = new Dictionary<int, AnimModelData>();
        public Animation()
        {
            Name = "animation_" + DateTime.Now.Ticks.ToString("X") + Object.rand.Next(0, 256).ToString("X2");
        }

        public Animation(byte[] file, int address, uint imageBase, bool DX)
        {
            Name = "animation_" + address.ToString("X8");
            Object Model = new Object(file, (int)(BitConverter.ToUInt32(file, address) - imageBase), imageBase, DX);
            int nummodels = 0;
            foreach (Object modl in Model.GetObjects())
                if ((modl.Flags & ObjectFlags.NoAnimate) != ObjectFlags.NoAnimate)
                    nummodels += 1;
            Int32 ptr = (int)(BitConverter.ToUInt32(file, address + 4) - imageBase);
            Frames = BitConverter.ToInt32(file, ptr + 4);
            AnimFlags animtype = (AnimFlags)BitConverter.ToUInt16(file, ptr + 8);
            ptr = (int)(BitConverter.ToUInt32(file, ptr) - imageBase);
            int framesize = 0;
            if ((animtype & AnimFlags.Translate) == AnimFlags.Translate)
                framesize += 8;
            if ((animtype & AnimFlags.Rotate) == AnimFlags.Rotate)
                framesize += 8;
            if ((animtype & AnimFlags.Scale) == AnimFlags.Scale)
                framesize += 8;
            for (int i = 0; i < nummodels; i++)
            {
                Models.Add(i, new AnimModelData(file, ptr + (i * framesize), imageBase, animtype));
                if (Models[i].Position.Count == 0 & Models[i].Rotation.Count == 0 & Models[i].Scale.Count == 0)
                    Models.Remove(i);
            }
        }

        public Animation(string file)
        {
            XmlDocument xml = new System.Xml.XmlDocument();
            xml.Load(file);
            XmlNode root = xml.DocumentElement;
            Name = root.Attributes["name"].Value;
            Frames = int.Parse(root.Attributes["frames"].Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            foreach (System.Xml.XmlNode item in root.ChildNodes)
            {
                if (item.Name == "model")
                {
                    AnimModelData mm = new AnimModelData();
                    if (item["position"] != null)
                        foreach (System.Xml.XmlNode pos in item["position"])
                            if (pos.Name == "posframe")
                                mm.Position.Add(int.Parse(pos.Attributes["frame"].Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), new Vertex(pos.InnerText));
                    if (item["rotation"] != null)
                        foreach (System.Xml.XmlNode rot in item["rotation"])
                            if (rot.Name == "rotframe")
                                mm.Rotation.Add(int.Parse(rot.Attributes["frame"].Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), new Rotation(rot.InnerText));
                    if (item["scale"] != null)
                        foreach (System.Xml.XmlNode scl in item["scale"])
                            if (scl.Name == "sclframe")
                                mm.Scale.Add(int.Parse(scl.Attributes["frame"].Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), new Vertex(scl.InnerText));
                    Models.Add(int.Parse(item.Attributes["id"].Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), mm);
                }
            }
        }

        public byte[] GetBytes(uint imageBase, uint modeladdr, int modelparts, out uint address)
        {
            List<byte> result = new List<byte>();
            uint[] posoffs = new uint[modelparts];
            int[] posframes = new int[modelparts];
            uint[] rotoffs = new uint[modelparts];
            int[] rotframes = new int[modelparts];
            uint[] scloffs = new uint[modelparts];
            int[] sclframes = new int[modelparts];
            bool hasPos = false, hasRot = false, hasScl = false;
            foreach (KeyValuePair<int, AnimModelData> model in Models)
            {
                if (model.Value.Position.Count > 0)
                {
                    hasPos = true;
                    result.Align(4);
                    posoffs[model.Key] = imageBase + (uint)result.Count;
                    posframes[model.Key] = model.Value.Position.Count;
                    foreach (KeyValuePair<int, Vertex> item in model.Value.Position)
                    {
                        result.AddRange(BitConverter.GetBytes(item.Key));
                        result.AddRange(item.Value.GetBytes());
                    }
                }
                if (model.Value.Rotation.Count > 0)
                {
                    hasRot = true;
                    result.Align(4);
                    rotoffs[model.Key] = imageBase + (uint)result.Count;
                    rotframes[model.Key] = model.Value.Rotation.Count;
                    foreach (KeyValuePair<int, Rotation> item in model.Value.Rotation)
                    {
                        result.AddRange(BitConverter.GetBytes(item.Key));
                        result.AddRange(item.Value.GetBytes());
                    }
                }
                if (model.Value.Scale.Count > 0)
                {
                    hasScl = true;
                    result.Align(4);
                    scloffs[model.Key] = imageBase + (uint)result.Count;
                    sclframes[model.Key] = model.Value.Scale.Count;
                    foreach (KeyValuePair<int, Vertex> item in model.Value.Scale)
                    {
                        result.AddRange(BitConverter.GetBytes(item.Key));
                        result.AddRange(item.Value.GetBytes());
                    }
                }
            }
            result.Align(4);
            uint modeldata = imageBase + (uint)result.Count;
            for (int i = 0; i < modelparts; i++)
            {
                if (hasPos)
                    result.AddRange(BitConverter.GetBytes(posoffs[i]));
                if (hasRot)
                    result.AddRange(BitConverter.GetBytes(rotoffs[i]));
                if (hasScl)
                    result.AddRange(BitConverter.GetBytes(scloffs[i]));
                if (hasPos)
                    result.AddRange(BitConverter.GetBytes(posframes[i]));
                if (hasRot)
                    result.AddRange(BitConverter.GetBytes(rotframes[i]));
                if (hasScl)
                    result.AddRange(BitConverter.GetBytes(sclframes[i]));
            }
            result.Align(4);
            uint head2 = imageBase + (uint)result.Count;
            result.AddRange(BitConverter.GetBytes(modeldata));
            result.AddRange(BitConverter.GetBytes(Frames));
            AnimFlags flags = 0;
            if (hasPos)
                flags |= AnimFlags.Translate;
            if (hasRot)
                flags |= AnimFlags.Rotate;
            if (hasScl)
                flags |= AnimFlags.Scale;
            result.AddRange(BitConverter.GetBytes((ushort)flags));
            ushort numpairs = 0;
            if (hasPos)
                numpairs += 1;
            if (hasRot)
                numpairs += 1;
            if (hasScl)
                numpairs += 1;
            result.AddRange(BitConverter.GetBytes(numpairs));
            result.Align(4);
            address = (uint)result.Count;
            result.AddRange(BitConverter.GetBytes(modeladdr));
            result.AddRange(BitConverter.GetBytes(head2));
            return result.ToArray();
        }

        public void Save(string file)
        {
            XmlDocument xml = new XmlDocument();
            xml.InsertBefore(xml.CreateXmlDeclaration("1.0", "utf-8", null), xml.DocumentElement);
            System.Xml.XmlNode mynode = xml.AppendChild(xml.CreateElement("animation"));
            mynode.Attributes.Append(xml.CreateAttribute("name")).Value = Name;
            mynode.Attributes.Append(xml.CreateAttribute("frames")).Value = Frames.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            foreach (KeyValuePair<int, AnimModelData> moi in Models)
            {
                System.Xml.XmlNode mm = mynode.AppendChild(xml.CreateElement("model"));
                mm.Attributes.Append(xml.CreateAttribute("id")).Value = moi.Key.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                if (moi.Value.Position.Count > 0)
                {
                    System.Xml.XmlNode x = mm.AppendChild(xml.CreateElement("position"));
                    foreach (KeyValuePair<int, Vertex> pos in moi.Value.Position)
                    {
                        System.Xml.XmlNode y = x.AppendChild(xml.CreateElement("posframe"));
                        y.Attributes.Append(xml.CreateAttribute("frame")).Value = pos.Key.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                        y.AppendChild(xml.CreateTextNode(pos.Value.ToString()));
                    }
                }
                if (moi.Value.Rotation.Count > 0)
                {
                    System.Xml.XmlNode x = mm.AppendChild(xml.CreateElement("rotation"));
                    foreach (KeyValuePair<int, Rotation> rot in moi.Value.Rotation)
                    {
                        System.Xml.XmlNode y = x.AppendChild(xml.CreateElement("rotframe"));
                        y.Attributes.Append(xml.CreateAttribute("frame")).Value = rot.Key.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                        y.AppendChild(xml.CreateTextNode(rot.Value.ToString()));
                    }
                }
                if (moi.Value.Scale.Count > 0)
                {
                    System.Xml.XmlNode x = mm.AppendChild(xml.CreateElement("scale"));
                    foreach (KeyValuePair<int, Vertex> scl in moi.Value.Scale)
                    {
                        System.Xml.XmlNode y = x.AppendChild(xml.CreateElement("sclframe"));
                        y.Attributes.Append(xml.CreateAttribute("frame")).Value = scl.Key.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                        y.AppendChild(xml.CreateTextNode(scl.Value.ToString()));
                    }
                }
            }
            xml.Save(file);
        }

        public class AnimModelData
        {
            public Dictionary<int, Vertex> Position = new Dictionary<int, Vertex>();
            public Dictionary<int, Rotation> Rotation = new Dictionary<int, Rotation>();
            public Dictionary<int, Vertex> Scale = new Dictionary<int, Vertex>();

            public AnimModelData() { }

            public AnimModelData(byte[] file, int address, uint imageBase, AnimFlags animtype)
            {
                int posoff = 0;
                int rotoff = 4;
                int scaoff = 8;
                int posframeoff = 12;
                int rotframeoff = 16;
                int scaframeoff = 20;
                if ((animtype & AnimFlags.Translate) == 0)
                {
                    rotoff -= 4;
                    rotframeoff -= 4;
                    scaoff -= 4;
                    scaframeoff -= 4;
                }
                if ((animtype & AnimFlags.Rotate) == 0)
                {
                    posframeoff -= 4;
                    scaoff -= 4;
                    scaframeoff -= 4;
                }
                if ((animtype & AnimFlags.Scale) == 0)
                {
                    posframeoff -= 4;
                    rotframeoff -= 4;
                }
                try
                {
                    int tmpaddr;
                    if ((animtype & AnimFlags.Translate) == AnimFlags.Translate && BitConverter.ToUInt32(file, address + posoff) > 0)
                    {
                        tmpaddr = (int)(BitConverter.ToUInt32(file, address + posoff) - imageBase);
                        for (int i = 0; i < BitConverter.ToUInt32(file, address + posframeoff); i++)
                        {
                            Position.Add(BitConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
                            tmpaddr += 16;
                        }
                    }
                    if ((animtype & AnimFlags.Rotate) == AnimFlags.Rotate && BitConverter.ToUInt32(file, address + rotoff) > 0)
                    {
                        tmpaddr = (int)(BitConverter.ToUInt32(file, address + rotoff) - imageBase);
                        for (int i = 0; i < BitConverter.ToUInt32(file, address + rotframeoff); i++)
                        {
                            Rotation.Add(BitConverter.ToInt32(file, tmpaddr), new Rotation(file, tmpaddr + 4));
                            tmpaddr += 16;
                        }
                    }
                    if ((animtype & AnimFlags.Scale) == AnimFlags.Scale && BitConverter.ToUInt32(file, address + scaoff) > 0)
                    {
                        tmpaddr = (int)(BitConverter.ToUInt32(file, address + scaoff) - imageBase);
                        for (int i = 0; i < BitConverter.ToUInt32(file, address + scaframeoff); i++)
                        {
                            Scale.Add(BitConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
                            tmpaddr += 16;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { }
            }

            public Vertex GetPosition(int frame)
            {
                if (Position.ContainsKey(frame))
                    return Position[frame];
                int f1 = 0;
                int f2 = 0;
                List<int> keys = new List<int>();
                foreach (int k in Position.Keys)
                    keys.Add(k);
                for (int i = 0; i < Position.Count; i++)
                {
                    if (keys[i] < frame)
                        f1 = keys[i];
                }
                for (int i = Position.Count - 1; i >= 0; i--)
                {
                    if (keys[i] > frame)
                        f2 = keys[i];
                }
                if (f2 == 0)
                    return GetPosition(0);
                Vertex val = new Vertex();
                val.X = (((Position[f2].X - Position[f1].X) / (f2 - f1)) * (frame - f1)) + Position[f1].X;
                val.Y = (((Position[f2].Y - Position[f1].Y) / (f2 - f1)) * (frame - f1)) + Position[f1].Y;
                val.Z = (((Position[f2].Z - Position[f1].Z) / (f2 - f1)) * (frame - f1)) + Position[f1].Z;
                return val;
            }

            public Rotation GetRotation(int frame)
            {
                if (Rotation.ContainsKey(frame))
                    return Rotation[frame];
                int f1 = 0;
                int f2 = 0;
                List<int> keys = new List<int>();
                foreach (int k in Rotation.Keys)
                    keys.Add(k);
                for (int i = 0; i < Rotation.Count; i++)
                {
                    if (keys[i] < frame)
                        f1 = keys[i];
                }
                for (int i = Rotation.Count - 1; i >= 0; i--)
                {
                    if (keys[i] > frame)
                        f2 = keys[i];
                }
                if (f2 == 0)
                    return GetRotation(0);
                Rotation val = new Rotation();
                val.X = (int)Math.Round((((Rotation[f2].X - Rotation[f1].X) / (double)(f2 - f1)) * (frame - f1)) + Rotation[f1].X, MidpointRounding.AwayFromZero);
                val.Y = (int)Math.Round((((Rotation[f2].Y - Rotation[f1].Y) / (double)(f2 - f1)) * (frame - f1)) + Rotation[f1].Y, MidpointRounding.AwayFromZero);
                val.Z = (int)Math.Round((((Rotation[f2].Z - Rotation[f1].Z) / (double)(f2 - f1)) * (frame - f1)) + Rotation[f1].Z, MidpointRounding.AwayFromZero);
                return val;
            }

            public Vertex GetScale(int frame)
            {
                if (Scale.ContainsKey(frame))
                    return Scale[frame];
                int f1 = 0;
                int f2 = 0;
                List<int> keys = new List<int>();
                foreach (int k in Scale.Keys)
                    keys.Add(k);
                for (int i = 0; i < Scale.Count; i++)
                {
                    if (keys[i] < frame)
                        f1 = keys[i];
                }
                for (int i = Scale.Count - 1; i >= 0; i--)
                {
                    if (keys[i] > frame)
                        f2 = keys[i];
                }
                if (f2 == 0)
                    return GetScale(0);
                Vertex val = new Vertex();
                val.X = (((Scale[f2].X - Scale[f1].X) / (f2 - f1)) * (frame - f1)) + Scale[f1].X;
                val.Y = (((Scale[f2].Y - Scale[f1].Y) / (f2 - f1)) * (frame - f1)) + Scale[f1].Y;
                val.Z = (((Scale[f2].Z - Scale[f1].Z) / (f2 - f1)) * (frame - f1)) + Scale[f1].Z;
                return val;
            }
        }
    }
}