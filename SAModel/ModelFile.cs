using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SonicRetro.SAModel
{
    public class ModelFile
    {
        public Object Model { get; private set; }
        public ReadOnlyCollection<Animation> Animations { get; private set; }
        private string[] animationFiles;

        public ModelFile(string filename)
        {
            ByteConverter.BigEndian = false;
            byte[] file = System.IO.File.ReadAllBytes(filename);
            ulong magic = ByteConverter.ToUInt64(file, 0);
            if (magic == 0x00004C444D314153u)
                Model = new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA1);
            else if (magic == 0x00004C444D324153u)
                Model = new Object(file, ByteConverter.ToInt32(file, 8), 0, ModelFormat.SA2);
            else
                throw new FormatException("Not a valid SA1MDL/SA2MDL file.");
            int tmpaddr = BitConverter.ToInt32(file, 0xC);
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
                anims.Add(new Animation(System.IO.Path.Combine(path, item)));
            Animations = anims.AsReadOnly();
        }

        public ModelFile(Dictionary<string, Dictionary<string, string>> INI, string path)
        {
            ModelFormat format = (ModelFormat)Enum.Parse(typeof(ModelFormat), INI[string.Empty]["Format"]);
            Model = new Object(INI, INI[string.Empty]["Root"]);
            if (INI[string.Empty].ContainsKey("Animations"))
                animationFiles = INI[string.Empty]["Animations"].Split(',');
            else
                animationFiles = new string[0];
            List<Animation> anims = new List<Animation>();
            foreach (string item in animationFiles)
                anims.Add(new Animation(System.IO.Path.Combine(path, item)));
            Animations = anims.AsReadOnly();
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

        public void SaveToFile(string filename, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            ulong magic;
            switch (format)
            {
                case ModelFormat.SA1:
                    magic = 0x00004C444D314153u;
                    break;
                case ModelFormat.SA2:
                    magic = 0x00004C444D314153u;
                    break;
                default:
                    throw new ArgumentException("Cannot save " + format.ToString() + " format levels to file!", "format");
            }
            file.AddRange(ByteConverter.GetBytes(magic));
            uint addr = 0;
            byte[] mdl = Model.GetBytes(0x10, format, out addr);
            file.AddRange(ByteConverter.GetBytes(addr + 0x10));
            if (Animations.Count > 0)
                file.AddRange(ByteConverter.GetBytes(0x10 + mdl.Length));
            file.Align(0x10);
            file.AddRange(mdl);
            if (Animations.Count > 0)
            {
                string path = System.IO.Path.GetDirectoryName(filename);
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
            }
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }

        public Dictionary<string, Dictionary<string, string>> Save(string path, ModelFormat format)
        {
            Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> gr = new Dictionary<string, string>();
            gr.Add("Root", Model.Name);
            gr.Add("Format", format.ToString());
            if (animationFiles.Length > 0)
                gr.Add("Animations", string.Join(",", animationFiles));
            ini.Add(string.Empty, gr);
            Model.Save(ini);
            for (int i = 0; i < Animations.Count; i++)
                Animations[i].Save(System.IO.Path.Combine(path, animationFiles[i]));
            return ini;
        }

        public static void CreateFile(string filename, Object model, string[] animationFiles, ModelFormat format)
        {
            ByteConverter.BigEndian = false;
            List<byte> file = new List<byte>();
            ulong magic;
            switch (format)
            {
                case ModelFormat.SA1:
                    magic = 0x00004C444D314153u;
                    break;
                case ModelFormat.SA2:
                    magic = 0x00004C444D314153u;
                    break;
                default:
                    throw new ArgumentException("Cannot save " + format.ToString() + " format levels to file!", "format");
            }
            file.AddRange(ByteConverter.GetBytes(magic));
            uint addr = 0;
            byte[] mdl = model.GetBytes(0x10, format, out addr);
            file.AddRange(ByteConverter.GetBytes(addr + 0x10));
            if (animationFiles.Length > 0)
                file.AddRange(ByteConverter.GetBytes(0x10 + mdl.Length));
            file.Align(0x10);
            file.AddRange(mdl);
            if (animationFiles.Length > 0)
            {
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
            System.IO.File.WriteAllBytes(filename, file.ToArray());
        }

        public static Dictionary<string, Dictionary<string, string>> Create(Object model, string[] animationFiles, ModelFormat format)
        {
            Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> gr = new Dictionary<string, string>();
            gr.Add("Root", model.Name);
            gr.Add("Format", format.ToString());
            if (animationFiles.Length > 0)
                gr.Add("Animations", string.Join(",", animationFiles));
            ini.Add(string.Empty, gr);
            model.Save(ini);
            return ini;
        }
    }
}