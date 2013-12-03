using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SonicRetro.SAModel;

namespace splitMDL
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Environment.CurrentDirectory;
            try
            {
                Queue<string> argq = new Queue<string>(args);
                if (argq.Count > 0 && argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
                {
                    ByteConverter.BigEndian = true;
                    argq.Dequeue();
                }
                string mdlfilename;
                if (argq.Count > 0)
                {
                    mdlfilename = argq.Dequeue();
                    Console.WriteLine("File: {0}", mdlfilename);
                }
                else
                {
                    Console.Write("File: ");
                    mdlfilename = Console.ReadLine();
                }
                mdlfilename = Path.GetFullPath(mdlfilename);
                string[] anifilenames = new string[argq.Count];
                for (int j = 0; j < anifilenames.Length; j++)
                {
                    Console.WriteLine("Animations: {0}", argq.Peek());
                    anifilenames[j] = Path.GetFullPath(argq.Dequeue());
                }
                Environment.CurrentDirectory = Path.GetDirectoryName(mdlfilename);
                byte[] mdlfile = File.ReadAllBytes(mdlfilename);
                if (Path.GetExtension(mdlfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                    mdlfile = FraGag.Compression.Prs.Decompress(mdlfile);
                Directory.CreateDirectory(Path.GetFileNameWithoutExtension(mdlfilename));
                int address = 0;
                int i = ByteConverter.ToInt32(mdlfile, address);
                Dictionary<int, SonicRetro.SAModel.Object> models = new Dictionary<int, SonicRetro.SAModel.Object>();
                while (i != -1)
                {
                    models[i] = new SonicRetro.SAModel.Object(mdlfile, ByteConverter.ToInt32(mdlfile, address + 4), 0, ModelFormat.Chunk);
                    address += 8;
                    i = ByteConverter.ToInt32(mdlfile, address);
                }
                Dictionary<int, string> animfns = new Dictionary<int, string>();
                Dictionary<int, Animation> anims = new Dictionary<int, Animation>();
                foreach (string anifilename in anifilenames)
                {
                    byte[] anifile = File.ReadAllBytes(anifilename);
                    if (Path.GetExtension(anifilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                        anifile = FraGag.Compression.Prs.Decompress(anifile);
                    Directory.CreateDirectory(Path.GetFileNameWithoutExtension(anifilename));
                    address = 0;
                    i = ByteConverter.ToInt16(anifile, address);
                    while (i != -1)
                    {
                        anims[i] = new Animation(anifile, ByteConverter.ToInt32(anifile, address + 4), 0, ByteConverter.ToInt16(anifile, address + 2));
                        animfns[i] = Path.GetFileNameWithoutExtension(anifilename) + "/" + i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim";
                        address += 8;
                        i = ByteConverter.ToInt16(anifile, address);
                    }
                }
                foreach (KeyValuePair<int, SonicRetro.SAModel.Object> model in models)
                {
                    List<string> animlist = new List<string>();
                    foreach (KeyValuePair<int, Animation> anim in anims)
                        if (model.Value.CountAnimated() == anim.Value.ModelParts)
                            animlist.Add("../" + animfns[anim.Key]);
                    ModelFile.CreateFile(Path.GetFileNameWithoutExtension(mdlfilename) + "/" + model.Key.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl", model.Value, animlist.ToArray(), null, null, null, "splitMDL", null, ModelFormat.Chunk);
                }
                foreach (KeyValuePair<int, Animation> anim in anims)
                    anim.Value.Save(animfns[anim.Key]);
            }
            finally
            {
                Environment.CurrentDirectory = dir;
            }
        }
    }
}