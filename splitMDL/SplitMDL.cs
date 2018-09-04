using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SonicRetro.SAModel;

namespace SplitMDL
{
    public static class SplitMDL
    {
        public static void Split(bool isBigEndian, string filePath, string outputFolder, string[] animationPaths)
        {
            string dir = Environment.CurrentDirectory;
            try
            {
                if (outputFolder[outputFolder.Length - 1] != '/') outputFolder = string.Concat(outputFolder, "/");
                ByteConverter.BigEndian = isBigEndian;

                // get file name, read it from the console if nothing
                string mdlfilename = filePath;

                mdlfilename = Path.GetFullPath(mdlfilename);

                // look through the argumetns for animationfiles
                string[] anifilenames = animationPaths;

                // load model file
                Environment.CurrentDirectory = (outputFolder.Length != 0) ? outputFolder : Path.GetDirectoryName(mdlfilename);
                byte[] mdlfile = File.ReadAllBytes(mdlfilename);
                if (Path.GetExtension(mdlfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                    mdlfile = FraGag.Compression.Prs.Decompress(mdlfile);
                Directory.CreateDirectory(Path.GetFileNameWithoutExtension(mdlfilename));

                // getting model pointers
                int address = 0;
                int i = ByteConverter.ToInt32(mdlfile, address);
                SortedDictionary<int, int> modeladdrs = new SortedDictionary<int, int>();
                while (i != -1)
                {
                    modeladdrs[i] = ByteConverter.ToInt32(mdlfile, address + 4);
                    address += 8;
                    i = ByteConverter.ToInt32(mdlfile, address);
                }

                // load models from pointer list
                Dictionary<int, NJS_OBJECT> models = new Dictionary<int, NJS_OBJECT>();
                Dictionary<int, string> modelnames = new Dictionary<int, string>();
                List<string> partnames = new List<string>();
                foreach (KeyValuePair<int, int> item in modeladdrs)
                {
                    NJS_OBJECT obj = new NJS_OBJECT(mdlfile, item.Value, 0, ModelFormat.Chunk);
                    modelnames[item.Key] = obj.Name;
                    if (!partnames.Contains(obj.Name))
                    {
                        List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
                        foreach (int idx in modelnames.Where(a => names.Contains(a.Value)).Select(a => a.Key))
                            models.Remove(idx);
                        models[item.Key] = obj;
                        partnames.AddRange(names);
                    }
                }

                // load animations
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
                        animfns[i] = Path.Combine(Path.GetFileNameWithoutExtension(anifilename), i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
                        address += 8;
                        i = ByteConverter.ToInt16(anifile, address);
                    }
                }

                // save output model files
                foreach (KeyValuePair<int, NJS_OBJECT> model in models)
                {
                    List<string> animlist = new List<string>();
                    foreach (KeyValuePair<int, Animation> anim in anims)
                        if (model.Value.CountAnimated() == anim.Value.ModelParts)
                            animlist.Add("../" + animfns[anim.Key]);
                    ModelFile.CreateFile(Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename),
						model.Key.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl"), model.Value, animlist.ToArray(),
						null, null, null, null, ModelFormat.Chunk);
                }

                // save ini file
                MDLIniSerializer.Serialize(modelnames, new IniCollectionSettings(IniCollectionMode.IndexOnly),
                    Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename), Path.GetFileNameWithoutExtension(mdlfilename) + ".ini"));
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
