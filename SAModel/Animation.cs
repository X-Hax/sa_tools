using System;
using System.Collections.Generic;
using System.Xml;

namespace SonicRetro.SAModel
{
    public class AnimationHeader
    {
        public Object Model { get; private set; }
        public Animation Animation { get; private set; }

        public AnimationHeader(byte[] file, int address, uint imageBase, ModelFormat format)
            : this(file, address, imageBase, format, new Dictionary<int, string>()) { }

        public AnimationHeader(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Model = new Object(file, (int)(ByteConverter.ToUInt32(file, address) - imageBase), imageBase, format);
            Animation = new Animation(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase, format, Model.CountAnimated(), labels);
        }

        public byte[] GetBytes(uint imageBase, ModelFormat format, Dictionary<string, uint> labels, out uint address)
        {
            List<byte> result = new List<byte>();
            uint modeladdr;
            result.AddRange(Model.GetBytes(imageBase, format, labels, out modeladdr));
            uint tmp = (uint)result.Count;
            uint head2;
            result.AddRange(Animation.GetBytes(imageBase + tmp, labels, out head2));
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes(modeladdr + imageBase));
            result.AddRange(ByteConverter.GetBytes(head2 + tmp + imageBase));
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
    }
    
    public class Animation
    {
        public const ulong SAANIM = 0x4D494E414153u;
        public const ulong FormatMask = 0xFFFFFFFFFFFFu;
        public const ulong CurrentVersion = 0;
        public const ulong SAANIMVer = SAANIM | (CurrentVersion << 56);

        public int Frames { get; set; }
        public string Name { get; set; }
        private int modelParts;

        public Dictionary<int, AnimModelData> Models = new Dictionary<int, AnimModelData>();
        public Animation()
        {
            Name = "animation_" + Object.GenerateIdentifier();
        }

        public Animation(byte[] file, int address, uint imageBase, ModelFormat format, int nummodels)
            : this(file, address, imageBase, format, nummodels, new Dictionary<int, string>()) { }

        public Animation(byte[] file, int address, uint imageBase, ModelFormat format, int nummodels, Dictionary<int, string> labels)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            if (labels.ContainsKey(address))
                Name = labels[address];
            else
                Name = "animation_" + address.ToString("X8");
            Int32 ptr = address;
            Frames = ByteConverter.ToInt32(file, ptr + 4);
            AnimFlags animtype = (AnimFlags)ByteConverter.ToUInt16(file, ptr + 8);
            ptr = (int)(ByteConverter.ToUInt32(file, ptr) - imageBase);
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
            modelParts = nummodels;
        }

        public static Animation ReadHeader(byte[] file, int address, uint imageBase, ModelFormat format)
        {
            return ReadHeader(file, address, imageBase, format, new Dictionary<int, string>());
        }

        public static Animation ReadHeader(byte[] file, int address, uint imageBase, ModelFormat format, Dictionary<int, string> labels)
        {
            if (format == ModelFormat.SA2B) ByteConverter.BigEndian = true;
            else ByteConverter.BigEndian = false;
            Object Model = new Object(file, (int)(ByteConverter.ToUInt32(file, address) - imageBase), imageBase, format);
            return new Animation(file, (int)(ByteConverter.ToUInt32(file, address + 4) - imageBase), imageBase, format, Model.CountAnimated(), labels);
        }

        public static Animation Load(string filename, int nummodels)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
            byte version = file[7];
            if (version > CurrentVersion)
                throw new FormatException("Not a valid SAANIM file.");
            int aniaddr = ByteConverter.ToInt32(file, 8);
            Dictionary<int, string> labels = new Dictionary<int, string>();
            int tmpaddr = BitConverter.ToInt32(file, 0xC);
            if (tmpaddr != 0)
                labels.Add(aniaddr, file.GetCString(tmpaddr));
            if (magic == SAANIM)
                return new Animation(file, aniaddr, 0, ModelFormat.SA1, nummodels, labels);
            else
                throw new FormatException("Not a valid SAANIM file.");
        }

        public byte[] GetBytes(uint imageBase, Dictionary<string, uint> labels, out uint address)
        {
            List<byte> result = new List<byte>();
            uint[] posoffs = new uint[modelParts];
            int[] posframes = new int[modelParts];
            uint[] rotoffs = new uint[modelParts];
            int[] rotframes = new int[modelParts];
            uint[] scloffs = new uint[modelParts];
            int[] sclframes = new int[modelParts];
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
                        result.AddRange(ByteConverter.GetBytes(item.Key));
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
                        result.AddRange(ByteConverter.GetBytes(item.Key));
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
                        result.AddRange(ByteConverter.GetBytes(item.Key));
                        result.AddRange(item.Value.GetBytes());
                    }
                }
            }
            result.Align(4);
            uint modeldata = imageBase + (uint)result.Count;
            for (int i = 0; i < modelParts; i++)
            {
                if (hasPos)
                    result.AddRange(ByteConverter.GetBytes(posoffs[i]));
                if (hasRot)
                    result.AddRange(ByteConverter.GetBytes(rotoffs[i]));
                if (hasScl)
                    result.AddRange(ByteConverter.GetBytes(scloffs[i]));
                if (hasPos)
                    result.AddRange(ByteConverter.GetBytes(posframes[i]));
                if (hasRot)
                    result.AddRange(ByteConverter.GetBytes(rotframes[i]));
                if (hasScl)
                    result.AddRange(ByteConverter.GetBytes(sclframes[i]));
            }
            result.Align(4);
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes(modeldata));
            result.AddRange(ByteConverter.GetBytes(Frames));
            AnimFlags flags = 0;
            if (hasPos)
                flags |= AnimFlags.Translate;
            if (hasRot)
                flags |= AnimFlags.Rotate;
            if (hasScl)
                flags |= AnimFlags.Scale;
            result.AddRange(ByteConverter.GetBytes((ushort)flags));
            ushort numpairs = 0;
            if (hasPos)
                numpairs += 1;
            if (hasRot)
                numpairs += 1;
            if (hasScl)
                numpairs += 1;
            result.AddRange(ByteConverter.GetBytes(numpairs));
            return result.ToArray();
        }

        public byte[] GetBytes(uint imageBase, out uint address)
        {
            return GetBytes(imageBase, new Dictionary<string, uint>(), out address);
        }

        public byte[] GetBytes(uint imageBase)
        {
            uint address;
            return GetBytes(imageBase, out address);
        }

        public byte[] WriteHeader(uint imageBase, uint modeladdr, Dictionary<string, uint> labels, out uint address)
        {
            List<byte> result = new List<byte>();
            uint head2;
            result.AddRange(GetBytes(imageBase, labels, out head2));
            address = (uint)result.Count;
            result.AddRange(ByteConverter.GetBytes(modeladdr));
            result.AddRange(ByteConverter.GetBytes(head2 + imageBase));
            return result.ToArray();
        }

        public byte[] WriteHeader(uint imageBase, uint modeladdr, out uint address)
        {
            return WriteHeader(imageBase, modeladdr, new Dictionary<string, uint>(), out address);
        }

        public byte[] WriteHeader(uint imageBase, uint modeladdr)
        {
            uint address;
            return WriteHeader(imageBase, modeladdr, new Dictionary<string, uint>(), out address);
        }

        public void Save(string filename)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            file.AddRange(ByteConverter.GetBytes(SAANIMVer));
            uint addr;
            byte[] anim = GetBytes(0x10, out addr);
            file.AddRange(ByteConverter.GetBytes(addr + 0x10));
            file.Align(0x10);
            file.AddRange(anim);
            file.Align(4);
            file.RemoveRange(0xC, 4);
            file.InsertRange(0xC, ByteConverter.GetBytes(file.Count + 4));
            file.AddRange(System.Text.Encoding.UTF8.GetBytes(Name));
            file.Add(0);
            file.Align(4);
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }
    }

    public class AnimModelData
    {
        public Dictionary<int, Vertex> Position = new Dictionary<int, Vertex>();
        public Dictionary<int, Rotation> Rotation = new Dictionary<int, Rotation>();
        public Dictionary<int, Vertex> Scale = new Dictionary<int, Vertex>();

        public AnimModelData() { }

        public AnimModelData(byte[] file, int address, uint imageBase, AnimFlags animtype)
        {
            int posoff = 0, rotoff = 0, scaoff = 0;
            if ((animtype & AnimFlags.Translate) == AnimFlags.Translate)
            {
                posoff = ByteConverter.ToInt32(file, address);
                if (posoff > 0)
                    posoff = (int)((uint)posoff - imageBase);
                address += 4;
            }
            if ((animtype & AnimFlags.Rotate) == AnimFlags.Rotate)
            {
                rotoff = ByteConverter.ToInt32(file, address);
                if (rotoff > 0)
                    rotoff = (int)((uint)rotoff - imageBase);
                address += 4;
            }
            if ((animtype & AnimFlags.Scale) == AnimFlags.Scale)
            {
                scaoff = ByteConverter.ToInt32(file, address);
                if (scaoff > 0)
                    scaoff = (int)((uint)scaoff - imageBase);
                address += 4;
            }
            try
            {
                int tmpaddr;
                if ((animtype & AnimFlags.Translate) == AnimFlags.Translate)
                {
                    int posframes = ByteConverter.ToInt32(file, address);
                    if (posframes > 0)
                    {
                        tmpaddr = posoff;
                        for (int i = 0; i < posframes; i++)
                        {
                            Position.Add(ByteConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
                            tmpaddr += 16;
                        }
                    }
                    address += 4;
                }
                if ((animtype & AnimFlags.Rotate) == AnimFlags.Rotate)
                {
                    int rotframes = ByteConverter.ToInt32(file, address);
                    if (rotframes > 0)
                    {
                        tmpaddr = rotoff;
                        for (int i = 0; i < rotframes; i++)
                        {
                            Rotation.Add(ByteConverter.ToInt32(file, tmpaddr), new Rotation(file, tmpaddr + 4));
                            tmpaddr += 16;
                        }
                    }
                    address += 4;
                }
                if ((animtype & AnimFlags.Scale) == AnimFlags.Scale)
                {
                    int scaframes = ByteConverter.ToInt32(file, address);
                    if (scaframes > 0)
                    {
                        tmpaddr = scaoff;
                        for (int i = 0; i < scaframes; i++)
                        {
                            Scale.Add(ByteConverter.ToInt32(file, tmpaddr), new Vertex(file, tmpaddr + 4));
                            tmpaddr += 16;
                        }
                    }
                    address += 4;
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