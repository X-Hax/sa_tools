using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PuyoTools.Modules.Archive;
using VrSharp.Pvr;
using ArchiveLib;
using SA_Tools;
using static ArchiveLib.DATFile;
using static ArchiveLib.PVMXFile;

namespace ArchiveTool
{
    static partial class Program
    {
        static string outputPath;
        static ArchiveFromFolderMode folderMode;
        /// <summary>
        /// Compress a binary to PRS.
        /// </summary>
        static void CompressPRS(string[] args)
		{
            filePath = args[1];
            Console.WriteLine("Compressing file to PRS: {0}", Path.GetFullPath(filePath));
            Console.WriteLine("Output file: {0}", Path.GetFullPath(Path.ChangeExtension(filePath, ".prs")));
            byte[] bindata = File.ReadAllBytes(filePath);
            bindata = FraGag.Compression.Prs.Compress(bindata);
            File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), bindata);
            Console.WriteLine("PRS archive was compiled successfully!");
        }
        /// <summary>
        /// Main function for automatic archive building from a folder.
        /// </summary>
        static void BuildFromFolder(string[] args)
        {
            bool createPB = false;
            filePath = args[0];
            compressPRS = false;
            for (int a = 0; a < args.Length; a++)
            {
                if (args[a] == "-prs") compressPRS = true;
                if (args[a] == "-pb") createPB = true;
            }
            //Folder mode
            if (Directory.Exists(filePath))
            {
                GenericArchive arc;
                string indexfilename = Path.Combine(filePath, "index.txt");
                List<string> filenames = new List<string>(File.ReadAllLines(indexfilename).Where(a => !string.IsNullOrEmpty(a)));
                string ext = Path.GetExtension(filenames[0]).ToLowerInvariant();
                switch (ext)
                {
                    case ".pvr":
                        if (createPB)
                        {
                            folderMode = ArchiveFromFolderMode.PB;
                            arc = new PBFile();
                        }
                        else
                        {
                            folderMode = ArchiveFromFolderMode.PVM;
                            arc = new PuyoFile();
                        }
                        break;
                    case ".gvr":
                        arc = new PuyoFile();
                        break;
                    case ".wav":
                    case ".adx":
                        folderMode = ArchiveFromFolderMode.DAT;
                        arc = new DATFile();
                        break;
                    case ".png":
                    case ".jpg":
                    case ".bmp":
                    case ".dds":
                    case ".gif":
                    default:
                        folderMode = ArchiveFromFolderMode.PVMX;
                        arc = new PVMXFile();
                        break;
                }
                Console.WriteLine("Creating {0} archive from folder: {1}", folderMode.ToString(), filePath);
                int id = 0;
                foreach (string line in filenames)
                {
                    string[] split = line.Split(',');
                    string filename = split[0];
                    switch (folderMode)
                    {
                        case ArchiveFromFolderMode.DAT:
                            arc.Entries.Add(new DATEntry(Path.Combine(filePath, filename)));
                            extension = ".dat";
                            break;
                        case ArchiveFromFolderMode.PVM:
                            arc.Entries.Add(new PVMEntry(Path.Combine(filePath, filename)));
                            extension = ".pvm";
                            break;
                        case ArchiveFromFolderMode.GVM:
                            arc.Entries.Add(new GVMEntry(Path.Combine(filePath, filename)));
                            extension = ".gvm";
                            break;
                        case ArchiveFromFolderMode.PB:
                            PBFile pbf = (PBFile)arc;
                            arc.Entries.Add(new PBEntry(Path.Combine(filePath, filename), pbf.GetCurrentOffset(id, filenames.Count)));
                            extension = ".pb";
                            break;
                        case ArchiveFromFolderMode.PVMX:
                            extension = ".pvmx";
                            filename = split[1];
                            int width = 0;
                            int height = 0;
                            uint gbix = uint.Parse(split[0]);
                            if (split.Length > 2)
                            {
                                width = int.Parse(split[2].Split('x')[0]);
                                height = int.Parse(split[2].Split('x')[1]);
                            }
                            arc.Entries.Add(new PVMXEntry(Path.GetFileName(filename), gbix, File.ReadAllBytes(Path.Combine(filePath, filename)), width, height));
                            break;
                        default:
                            extension = ".bin";
                            break;
                    }
                    Console.WriteLine("Added entry {0}: {1}", id.ToString(), filename);
                    id++;
                }
                byte[] data = arc.GetBytes();
                outputPath = Path.GetFullPath(filePath) + extension;
                if (compressPRS)
                {
                    Console.WriteLine("Compressing to PRS...");
                    data = FraGag.Compression.Prs.Compress(data);
                    outputPath = Path.ChangeExtension(outputPath, ".PRS");
                }
                Console.WriteLine("Output file: {0}", outputPath);
                File.WriteAllBytes(outputPath, data);
            }
        }
        /// <summary>
        /// Create a PAK archive from a folder produced by ArchiveTool or PAKTool.
        /// </summary>
        static void BuildPAK(string[] args)
        {
            filePath = args[1];
            Console.WriteLine("Building PAK from folder: {0}", Path.GetFullPath(filePath));
            outputPath = Path.Combine(Environment.CurrentDirectory, filePath);
            Environment.CurrentDirectory = Path.GetDirectoryName(outputPath);
            Dictionary<string, PAKFile.PAKIniItem> list = IniSerializer.Deserialize<Dictionary<string, PAKFile.PAKIniItem>>(Path.Combine(Path.GetFileNameWithoutExtension(outputPath), Path.GetFileNameWithoutExtension(outputPath) + ".ini"));
            PAKFile pak = new PAKFile();
            foreach (KeyValuePair<string, PAKFile.PAKIniItem> item in list)
            {
                Console.WriteLine("Adding file: {0}", item.Key);
                pak.Entries.Add(new PAKFile.PAKEntry(item.Key, item.Value.LongPath, File.ReadAllBytes(item.Key)));
            }
            Console.WriteLine("Output file: {0}", Path.ChangeExtension(outputPath, "pak"));
            pak.Save(Path.ChangeExtension(outputPath, "pak"));
        }
        /// <summary>
		/// Convert a GBIX indexed texture pack to PVM.
		/// </summary>
        static void TexPack2PVM(string[] args)
        {
            bool compressPRS = false;
            if (args[args.Length - 1] == "-prs") compressPRS = true;
            filePath = args[1];
            string FullLine;
            string texturename;
            uint GBIX = 0;
            List<string> textureNames = new List<String>();
            List<PvrTexture> finalTextureList = new List<PvrTexture>();
            directoryName = Path.GetDirectoryName(filePath);
            string archiveName = Path.GetFileNameWithoutExtension(filePath);
            if (Directory.Exists(filePath))
            {
                Console.WriteLine("Converting texture pack to PVM: {0}", Path.GetFullPath(filePath));
                StreamReader texlistStream = File.OpenText(Path.Combine(filePath, "index.txt"));
                while (!texlistStream.EndOfStream) textureNames.Add(texlistStream.ReadLine());
                puyoArchiveBase = new PvmArchive();
                outputPath = Path.ChangeExtension(filePath, ".pvm");
                Stream pvmStream = File.Open(outputPath, FileMode.Create);
                puyoArchiveWriter = (PvmArchiveWriter)puyoArchiveBase.Create(pvmStream);
                // Reading in textures
                for (uint imgIndx = 0; imgIndx < textureNames.Count; imgIndx++)
                {
                    FullLine = textureNames[(int)imgIndx];
                    if (string.IsNullOrEmpty(FullLine)) continue;
                    string[] substrings = FullLine.Split(',');
                    GBIX = UInt32.Parse(substrings[0]);
                    texturename = substrings[1];
                    Bitmap tempTexture = new Bitmap(8, 8);
                    string texturePath = Path.Combine(filePath, Path.ChangeExtension(texturename, ".png"));
                    if (File.Exists(texturePath))
                    {
                        Console.WriteLine("Adding texture: " + (texturePath));
                        tempTexture = (Bitmap)Bitmap.FromFile(texturePath);
                        tempTexture = tempTexture.Clone(new Rectangle(Point.Empty, tempTexture.Size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                    else
                    {
                        Console.WriteLine(string.Concat("Texture ", textureNames[(int)imgIndx], " not found. Generating a placeholder. Check your files."));
                    }

                    System.Drawing.Imaging.BitmapData bmpd = tempTexture.LockBits(new Rectangle(Point.Empty, tempTexture.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    int stride = bmpd.Stride;
                    byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
                    System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
                    tempTexture.UnlockBits(bmpd);
                    int tlevels = 0;
                    for (int y = 0; y < tempTexture.Height; y++)
                    {
                        int srcaddr = y * Math.Abs(stride);
                        for (int x = 0; x < tempTexture.Width; x++)
                        {
                            Color c = Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
                            if (c.A == 0)
                                tlevels = 1;
                            else if (c.A < 255)
                            {
                                tlevels = 2;
                                break;
                            }
                        }
                        if (tlevels == 2)
                            break;
                    }
                    PvrPixelFormat ppf = PvrPixelFormat.Rgb565;
                    if (tlevels == 1)
                        ppf = PvrPixelFormat.Argb1555;
                    else if (tlevels == 2)
                        ppf = PvrPixelFormat.Argb4444;
                    PvrDataFormat pdf = PvrDataFormat.Rectangle;
                    if (tempTexture.Width == tempTexture.Height)
                        pdf = PvrDataFormat.SquareTwiddled;
                    PvrTextureEncoder encoder = new PvrTextureEncoder(tempTexture, ppf, pdf);
                    encoder.GlobalIndex = GBIX;
                    string pvrPath = Path.ChangeExtension(texturePath, ".pvr");
                    encoder.Save(pvrPath);
                    puyoArchiveWriter.CreateEntryFromFile(pvrPath);
                }
                puyoArchiveWriter.Flush();
                pvmStream.Close();
                if (compressPRS)
                {
                    Console.WriteLine("Compressing to PRS...");
                    byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pvm"));
                    pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
                    outputPath = Path.ChangeExtension(filePath, ".prs");
                    File.WriteAllBytes(outputPath, pvmdata);
                    File.Delete(Path.ChangeExtension(filePath, ".pvm"));
                }
                Console.WriteLine("Output file: {0}", Path.GetFullPath(outputPath));
                Console.WriteLine("Archive was compiled successfully!");
            }
            else
            {
                Console.WriteLine("Supplied texture list does not exist!");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }
        }

        enum ArchiveFromFolderMode
        {
            PVM = 0,
            GVM = 1,
            DAT = 2,
            PVMX = 3,
            PB = 4
        }
    }
}