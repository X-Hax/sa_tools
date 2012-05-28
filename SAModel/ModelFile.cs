using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SonicRetro.SAModel
{
    public class ModelFile
    {
        public const ulong SA1MDL = 0x4C444D314153u;
        public const ulong SA2MDL = 0x4C444D324153u;
        public const ulong FormatMask = 0xFFFFFFFFFFFFu;
        public const ulong CurrentVersion = 1;
        public const ulong SA1MDLVer = SA1MDL | (CurrentVersion << 56);
        public const ulong SA2MDLVer = SA2MDL | (CurrentVersion << 56);

        public Object Model { get; private set; }
        public ReadOnlyCollection<Animation> Animations { get; private set; }
        public ReadOnlyCollection<Animation> Morphs { get; private set; }
        private string[] animationFiles;
        private string[] morphFiles;

        public ModelFile(string filename)
        {
            int tmpaddr;
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0) & FormatMask;
            byte version = file[7];
            if (version > CurrentVersion)
                throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
            Dictionary<int, string> labels = new Dictionary<int, string>();
            if (version == 1)
            {
                tmpaddr = BitConverter.ToInt32(file, 0x14);
                if (tmpaddr != 0)
                {
                    int addr = BitConverter.ToInt32(file, tmpaddr);
                    while (addr != -1)
                    {
                        labels.Add(addr, file.GetCString(BitConverter.ToInt32(file, tmpaddr + 4)));
                        tmpaddr += 8;
                        addr = BitConverter.ToInt32(file, tmpaddr);
                    }
                }
            }
            if (magic == SA1MDL)
                Model = new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA1, labels);
            else if (magic == SA2MDL)
                Model = new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA2, labels);
            else
                throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
            tmpaddr = BitConverter.ToInt32(file, 0xC);
            if (tmpaddr != 0)
            {
                List<string> animfiles = new List<string>();
                int addr = BitConverter.ToInt32(file, tmpaddr);
                while (addr != -1)
                {
                    animfiles.Add(file.GetCString(addr));
                    tmpaddr += 4;
                    addr = BitConverter.ToInt32(file, tmpaddr);
                }
                animationFiles = animfiles.ToArray();
            }
            else
                animationFiles = new string[0];
            string path = System.IO.Path.GetDirectoryName(filename);
            List<Animation> anims = new List<Animation>();
            foreach (string item in animationFiles)
                anims.Add(Animation.Load(System.IO.Path.Combine(path, item), Model.CountAnimated()));
            Animations = anims.AsReadOnly();
            if (version == 1)
            {
                tmpaddr = BitConverter.ToInt32(file, 0x10);
                if (tmpaddr != 0)
                {
                    List<string> morphfiles = new List<string>();
                    int addr = BitConverter.ToInt32(file, tmpaddr);
                    while (addr != -1)
                    {
                        morphfiles.Add(file.GetCString(addr));
                        tmpaddr += 4;
                        addr = BitConverter.ToInt32(file, tmpaddr);
                    }
                    morphFiles = morphfiles.ToArray();
                }
                else
                    morphFiles = new string[0];
                List<Animation> morphs = new List<Animation>();
                foreach (string item in morphFiles)
                    morphs.Add(Animation.Load(System.IO.Path.Combine(path, item), Model.CountMorph()));
                Morphs = morphs.AsReadOnly();
            }
            else
            {
                morphFiles = new string[0];
                Morphs = new ReadOnlyCollection<Animation>(new List<Animation>());
            }
        }

        public static bool CheckModelFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            switch (ByteConverter.ToUInt64(file, 0) & FormatMask)
            {
                case SA1MDL:
                case SA2MDL:
                    return file[7] <= CurrentVersion;
                default:
                    return false;
            }
        }

        public void SaveToFile(string filename, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            ulong magic;
            switch (format)
            {
                case ModelFormat.SA1:
                    magic = SA1MDLVer;
                    break;
                case ModelFormat.SA2:
                    magic = SA2MDLVer;
                    break;
                default:
                    throw new ArgumentException("Cannot save " + format.ToString() + " format models to file!", "format");
            }
            file.AddRange(ByteConverter.GetBytes(magic));
            uint addr = 0;
            Dictionary<string, uint> labels = new Dictionary<string, uint>();
            byte[] mdl = Model.GetBytes(0x18, format, labels, out addr);
            file.AddRange(ByteConverter.GetBytes(addr + 0x18));
            file.Align(0x18);
            file.AddRange(mdl);
            file.Align(4);
            string path = System.IO.Path.GetDirectoryName(filename);
            if (Animations.Count > 0)
            {
                file.RemoveRange(0xC, 4);
                file.InsertRange(0xC, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (Animations.Count * 4) + 4;
                List<byte> strbytes = new List<byte>();
                for (int i = 0; i < Animations.Count; i++)
                {
                    Animations[i].Save(System.IO.Path.Combine(path, animationFiles[i]));
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(animationFiles[i]));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1));
                file.AddRange(strbytes);
                file.Align(4);
            }
            if (Morphs.Count > 0)
            {
                file.RemoveRange(0x10, 4);
                file.InsertRange(0x10, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (Morphs.Count * 4) + 4;
                List<byte> strbytes = new List<byte>();
                for (int i = 0; i < Morphs.Count; i++)
                {
                    Morphs[i].Save(System.IO.Path.Combine(path, morphFiles[i]));
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(morphFiles[i]));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1));
                file.AddRange(strbytes);
                file.Align(4);
            }
            if (labels.Count > 0)
            {
                file.RemoveRange(0x14, 4);
                file.InsertRange(0x14, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (labels.Count * 8) + 8;
                List<byte> strbytes = new List<byte>();
                foreach (KeyValuePair<string, uint> label in labels)
                {
                    file.AddRange(ByteConverter.GetBytes(label.Value));
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(label.Key));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1L));
                file.AddRange(strbytes);
                file.Align(4);
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }

        public static void CreateFile(string filename, Object model, ModelFormat format)
        {
            CreateFile(filename, model, new string[0], format);
        }

        public static void CreateFile(string filename, Object model, string[] animationFiles, ModelFormat format)
        {
            CreateFile(filename, model, animationFiles, new string[0], format);
        }

        public static void CreateFile(string filename, Object model, string[] animationFiles, string[] morphFiles, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            ulong magic;
            switch (format)
            {
                case ModelFormat.SA1:
                    magic = SA1MDLVer;
                    break;
                case ModelFormat.SA2:
                    magic = SA2MDLVer;
                    break;
                default:
                    throw new ArgumentException("Cannot save " + format.ToString() + " format models to file!", "format");
            }
            file.AddRange(ByteConverter.GetBytes(magic));
            uint addr = 0;
            Dictionary<string, uint> labels = new Dictionary<string, uint>();
            byte[] mdl = model.GetBytes(0x18, format, labels, out addr);
            file.AddRange(ByteConverter.GetBytes(addr + 0x18));
            file.Align(0x18);
            file.AddRange(mdl);
            string path = System.IO.Path.GetDirectoryName(filename);
            if (animationFiles.Length > 0)
            {
                file.RemoveRange(0xC, 4);
                file.InsertRange(0xC, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (animationFiles.Length * 4) + 4;
                List<byte> strbytes = new List<byte>();
                for (int i = 0; i < animationFiles.Length; i++)
                {
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(animationFiles[i]));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1));
                file.AddRange(strbytes);
            }
            if (morphFiles.Length > 0)
            {
                file.RemoveRange(0x10, 4);
                file.InsertRange(0x10, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (morphFiles.Length * 4) + 4;
                List<byte> strbytes = new List<byte>();
                for (int i = 0; i < morphFiles.Length; i++)
                {
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(morphFiles[i]));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1));
                file.AddRange(strbytes);
            }
            if (labels.Count > 0)
            {
                file.RemoveRange(0x14, 4);
                file.InsertRange(0x14, ByteConverter.GetBytes(file.Count + 4));
                int straddr = file.Count + (labels.Count * 8) + 8;
                List<byte> strbytes = new List<byte>();
                foreach (KeyValuePair<string, uint> label in labels)
                {
                    file.AddRange(ByteConverter.GetBytes(label.Value));
                    file.AddRange(ByteConverter.GetBytes(straddr + strbytes.Count));
                    strbytes.AddRange(Encoding.UTF8.GetBytes(label.Key));
                    strbytes.Add(0);
                    strbytes.Align(4);
                }
                file.AddRange(ByteConverter.GetBytes(-1L));
                file.AddRange(strbytes);
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }
    }
}