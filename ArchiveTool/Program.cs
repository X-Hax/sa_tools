using System;
using System.Collections.Generic;
using System.IO;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.PvrTexture;

namespace ArchiveTool
{
    class Program
    {
        static bool AddTexture(string path, string filename, Stream data, TextWriter index)
        {
            index.WriteLine("{0}", Path.GetFileName(filename));
            return true;
        }
        static void Main(string[] args)
        {
            string directoryName;
            List<String> textureNames = new List<String>();
            List<PvrTexture> finalTextureList = new List<PvrTexture>();

            if (args.Length == 0)
            {
                Console.WriteLine("Error - please specify a texture list (.txt) or a PVM archive (.pvm)");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }
            String filePath = args[0];
            directoryName = Path.GetDirectoryName(filePath);
            string extension = Path.GetExtension(filePath);
            if (extension == ".txt" || extension == ".TXT")
            {
                string archiveName = Path.GetFileNameWithoutExtension(filePath);
                if (File.Exists(filePath))
                {
                    StreamReader texlistStream = File.OpenText(filePath);

                    while (!texlistStream.EndOfStream)
                        textureNames.Add(texlistStream.ReadLine());
                    PvmArchive pvmArchive = new PvmArchive();
                    Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create);
                    PvmArchiveWriter pvmWriter = (PvmArchiveWriter)pvmArchive.Create(pvmStream);
                    // Reading in textures
                    for (uint imgIndx = 0; imgIndx < textureNames.Count; imgIndx++)
                    {
                        string texturePath = Path.Combine(directoryName, Path.ChangeExtension(textureNames[(int)imgIndx], ".pvr"));
                        pvmWriter.CreateEntryFromFile(texturePath);
                    }
                    pvmWriter.Flush();
                    pvmStream.Close();
                    Console.WriteLine("PVM was compiled successfully!");
                    return;
                }
                else // error, supplied path is invalid
                {
                    Console.WriteLine("Supplied texture list/PVM does not exist!");
                    Console.WriteLine("Press ENTER to continue...");
                    Console.ReadLine();
                    return;
                }
            }
            if (extension == ".pvm" || extension == ".PVM")
            {
                Queue<string> files = new Queue<string>(args);
                while (files.Count > 0)
                {
                    string filename = files.Dequeue();
                    string path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
                    Directory.CreateDirectory(path);
                    byte[] filedata = File.ReadAllBytes(filename);
                    using (TextWriter texList = File.CreateText(Path.Combine(path, Path.ChangeExtension(filename, ".txt"))))
                    {
                        try
                        {
                            if (PvrTexture.Is(filedata))
                            {
                                if (!AddTexture(path, Path.GetFileName(filename), new MemoryStream(filedata), texList))
                                {
                                    texList.Close();
                                    Directory.Delete(path, true);
                                }
                                continue;
                            }
                            ArchiveBase pvmfile = null;
                            byte[] pvmdata = File.ReadAllBytes(filename);
                            pvmfile = new PvmArchive();
                            ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
                            bool fail = false;
                            foreach (ArchiveEntry file in pvmentries)
                                if (!AddTexture(path, file.Name, file.Open(), texList))
                                {
                                    texList.Close();
                                    Directory.Delete(path, true);
                                    fail = true;
                                    break;
                                }
                                else pvmfile.Open(pvmdata).ExtractToFile(file, Path.Combine(path, file.Name));
                            if (fail)
                                continue;
                        }
                        catch
                        {
                            Console.WriteLine("Exception thrown. Canceling conversion.");
                            Directory.Delete(path, true);
                            throw;
                        }
                        Console.WriteLine("Archive extracted!");
                    }
                }
            }
        }
    }
}