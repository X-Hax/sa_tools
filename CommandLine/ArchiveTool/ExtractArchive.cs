using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ArchiveLib;
using static ArchiveLib.GenericArchive;

namespace ArchiveTool
{
    static partial class Program
    {
        /// <summary>
        /// Main function for extracting archives.
        /// </summary>
        static void ExtractArchive(string[] args)
        {
            GenericArchive arc;
            string arcname = extension.ToUpperInvariant();
            Console.WriteLine("Extracting {0} file: {1}", arcname.Substring(1, arcname.Length - 1), filePath);
            byte[] arcdata = File.ReadAllBytes(filePath);
            outputPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
            switch (extension.ToLowerInvariant())
            {
                case (".rel"):
                    outputPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_dec.rel");
                    Console.WriteLine("Output file: {0}", outputPath);
                    byte[] inputData = File.ReadAllBytes(args[0]);
                    byte[] outputData = SplitTools.HelperFunctions.DecompressREL(inputData);
                    File.WriteAllBytes(outputPath, outputData);
                    Console.WriteLine("File extracted!");
                    return;
                case (".pvmx"):
                    arc = new PVMXFile(arcdata);
                    break;
                case (".prs"):
                    arcdata = FraGag.Compression.Prs.Decompress(arcdata);
                    if (PuyoFile.Identify(arcdata) == PuyoArchiveType.Unknown)
                    {
                        outputPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".bin");
                        Console.WriteLine("Output file: {0}", Path.GetFullPath(outputPath));
                        File.WriteAllBytes(outputPath, arcdata);
                        Console.WriteLine("Archive extracted!");
                        return;
                    }
                    arc = new PuyoFile(arcdata);
                    break;
                case (".pvm"):
                case (".gvm"):
                    arc = new PuyoFile(arcdata);
                    break;
                case (".pb"):
                    arc = new PBFile(arcdata);
                    break;
                case (".pak"):
                    arc = new PAKFile(filePath);
                    break;
                case (".dat"):
                    arc = new DATFile(arcdata);
                    break;
                case (".mdl"):
                    arc = new MDLArchive(arcdata);
                    break;
                case (".mdt"):
                    arc = new MDTArchive(arcdata);
                    break;
                case (".mld"):
                    arc = new MLDArchive(arcdata);
                    break;
                case (".mlt"):
                case (".gcaxmlt"):
                    string test = System.Text.Encoding.ASCII.GetString(arcdata, 0, 4);
                    if (test == "gcax")
                        arc = new gcaxMLTFile(arcdata, Path.GetFileNameWithoutExtension(filePath));
                    else
                        arc = new MLTFile(arcdata, Path.GetFileNameWithoutExtension(filePath));
                    break;
                default:
                    Console.WriteLine("Unknown archive type");
                    return;
            }
            Console.WriteLine("Output folder: {0}", Path.GetFullPath(outputPath));
            Directory.CreateDirectory(outputPath);
            foreach (GenericArchiveEntry entry in arc.Entries)
            {
                if (entry.Data == null)
                {
                    Console.WriteLine("Entry {0} has no data", entry.Name);
                    continue;
                }
                Console.WriteLine("Extracting file: {0}", entry.Name);
                File.WriteAllBytes(Path.Combine(outputPath, entry.Name), entry.Data);
            }
            arc.CreateIndexFile(outputPath);
            Console.WriteLine("Archive extracted!");
        }
        /// <summary>
        /// Convert PVM or GVM to a texture pack.
        /// </summary>
        static void Puyo2Texpack(string[] args)
        {
            Queue<string> files = new Queue<string>();
            for (int u = 1; u < args.Length; u++)
            {
                files.Enqueue(args[u]);
            }
            if (files.Count == 0)
            {
                Console.Write("File: ");
                files.Enqueue(Console.ReadLine());
            }
            while (files.Count > 0)
            {
                string filename = files.Dequeue();
                outputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
                string filename_full = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileName(filename));
                Console.WriteLine("Converting file to texture pack: {0}", filename_full);
                Console.WriteLine("Output folder: {0}", Path.GetFullPath(outputPath));
                Directory.CreateDirectory(outputPath);
                byte[] filedata = File.ReadAllBytes(filename_full);
                if (Path.GetExtension(filename_full).ToLowerInvariant() == ".prs")
                    filedata = FraGag.Compression.Prs.Decompress(filedata);
                PuyoFile puyo = new PuyoFile(filedata);
                using (TextWriter texList = File.CreateText(Path.Combine(outputPath, "index.txt")))
                {
                    try
                    {
                        foreach (GenericArchiveEntry entry in puyo.Entries)
                        {
                            uint gbix = 0;
                            if (entry is PVMEntry pvme)
                                gbix = pvme.GetGBIX();
                            else if (entry is GVMEntry gvme)
                                gbix = gvme.GetGBIX();
                            Bitmap bmp = entry.GetBitmap();
                            Console.WriteLine("Converting entry: {0}", entry.Name);
                            texList.WriteLine(string.Join(",", gbix, Path.GetFileNameWithoutExtension(entry.Name) + ".png", bmp.Width + "x" + bmp.Height));
                            bmp.Save(Path.Combine(outputPath, Path.GetFileNameWithoutExtension(entry.Name) + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception thrown: " + ex.ToString() + "\nCanceling conversion.");
                        return;
                    }
                    Console.WriteLine("Conversion complete!");
                    if (files.Count > 0) Console.WriteLine();
                }
            }
        }
        /// <summary>
        /// Extract an NjUtil archive.
        /// </summary>
        static void ExtractNjUtil(string[] args)
        {
            filePath = args[1];
            byte[] filedata = File.ReadAllBytes(filePath);
            if (Path.GetExtension(filePath).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                filedata = FraGag.Compression.Prs.Decompress(filedata);
            NjArchive njarc = new NjArchive(filedata);
            Console.WriteLine("Extracting Ninja archive: {0}", Path.GetFullPath(filePath));
            outputPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
            Directory.CreateDirectory(outputPath);
            for (int i = 0; i < njarc.Entries.Count; i++)
            {
                byte[] data = njarc.Entries[i].Data;
                extension = ".bin";
                string desc = "Unknown";
                switch (System.Text.Encoding.ASCII.GetString(data, 0, 4))
                {
                    case "NJIN":
                        desc = "Ninja Information";
                        extension = ".nji";
                        break;
                    case "NJCM":
                        desc = "Ninja Chunk model";
                        extension = ".nj";
                        break;
                    case "GJCM":
                        desc = "Ninja Chunk model (GC)";
                        extension = ".gj";
                        break;
                    case "NJBM":
                        desc = "Ninja Basic model";
                        extension = ".nj";
                        break;
                    case "NMDM":
                        desc = "Ninja Motion";
                        extension = ".njm";
                        break;
                    case "NJLI":
                        desc = "Ninja Light";
                        extension = ".njl";
                        break;
                    case "NLIM":
                        desc = "Ninja Light Motion";
                        extension = ".njlm";
                        break;
                    case "NSSM":
                        desc = "Ninja Simple Shape Motion";
                        extension = ".njsm";
                        break;
                    case "NCAM":
                        desc = "Ninja Camera Motion";
                        extension = ".ncm";
                        break;
                    case "NJTL":
                        desc = "Ninja Texlist";
                        extension = ".nj";
                        break;
                    case "GJTL":
                        desc = "Ninja Texlist (GC)";
                        extension = ".gj";
                        break;
                    case "PVMH":
                        desc = "PVM";
                        extension = ".pvm";
                        break;
                    case "GVMH":
                        desc = "GVM";
                        extension = ".gvm";
                        break;
                }
                Console.WriteLine("Entry {0} is {1}", i, desc);
                string outpath = Path.Combine(outputPath, i.ToString("D3") + extension);
                File.WriteAllBytes(outpath, njarc.Entries[i].Data);
            }
            Console.WriteLine("Output folder: {0}", Path.GetFullPath(outputPath));
            Console.WriteLine("Archive extracted!");
        }
    }
}