using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using PuyoTools.Modules.Archive;
using SonicRetro.SAModel;
using VrSharp;
using VrSharp.Gvr;
using VrSharp.Pvr;
using ArchiveLib;

namespace ArchiveTool
{
	static class Program
	{
        static void Main(string[] args)
        {
            string dir;
            string filePath;
            string directoryName;
            ArchiveBase pvmArchive;
            ArchiveWriter pvmWriter;
            string archiveName;
            string path;
            byte[] filedata;
            bool isPRS;
            string extension;
            // Usage
            if (args.Length == 0)
            {
                Console.WriteLine("ArchiveTool is a command line tool to extract and create PVM, GVM, PRS, DAT and PB archives.\nIt can also decompress SADX Gamecube 'SaCompGC' REL files.\n");
                Console.WriteLine("Usage:\n");
                Console.WriteLine("Extracting a PVM/GVM/PRS/PB/DAT/REL file:\nArchiveTool <archivefile>\nIf the archive is PRS compressed, it will be decompressed first.\nIf the archive contains textures/sounds, the program will extract them and create a list of files named 'index.txt'.\n");
                Console.WriteLine("Converting PVM/GVM to a folder texture pack: ArchiveTool -png <archivefile>\n");
                Console.WriteLine("Creating a PB archive from a folder with textures: ArchiveTool -pb <foldername>");
                Console.WriteLine("Creating a PVM/GVM/DAT from a folder with textures/sounds: ArchiveTool <foldername> [-prs]\nThe program will create an archive from files listed in 'index.txt' in the folder.\nThe -prs option will make the program output a PRS compressed archive.\n");
                Console.WriteLine("Creating a PVM from PNG textures: ArchiveTool -pvm <folder> [-prs]\nThe texture list 'index.txt' must contain global indices listed before each texture filename for this option to work.\n");
                Console.WriteLine("Converting GVM to PVM (lossy): ArchiveTool -gvm2pvm <file.gvm> [-prs]\n");
                Console.WriteLine("Creating a PRS compressed binary: ArchiveTool <file.bin>\nFile extension must be .BIN for this option to work.\n");
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
                return;
            }
            switch (args[0].ToLowerInvariant())
            {
                // GVM2PVM mode
                case "-gvm2pvm":
                    filePath = args[1];
                    isPRS = false;
                    if (args.Length > 2 && args[2] == "-prs") isPRS = true;
                    Console.WriteLine("Converting GVM to PVM: {0}", filePath);
                    directoryName = Path.GetDirectoryName(filePath);
                    extension = Path.GetExtension(filePath).ToLowerInvariant();
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("Supplied GVM archive does not exist!");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    if (extension != ".gvm")
                    {
                        Console.WriteLine("GVM2PVM mode can only be used with GVM files.");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    path = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(filePath));
                    Directory.CreateDirectory(path);
                    filedata = File.ReadAllBytes(filePath);
                    using (TextWriter texList = File.CreateText(Path.Combine(path, Path.GetFileName(path) + ".txt")))
                    {
                        try
                        {
                            ArchiveBase gvmfile = null;
                            byte[] gvmdata = File.ReadAllBytes(filePath);
                            gvmfile = new GvmArchive();
                            ArchiveReader gvmReader = gvmfile.Open(gvmdata);
                            Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create);
                            pvmArchive = new PvmArchive();
                            pvmWriter = pvmArchive.Create(pvmStream);
                            foreach (ArchiveEntry file in gvmReader.Entries)
                            {
                                if (!File.Exists(Path.Combine(path, file.Name)))
                                    gvmReader.ExtractToFile(file, Path.Combine(path, file.Name));
                                Stream data = File.Open(Path.Combine(path, file.Name), FileMode.Open);
                                VrTexture vrfile = new GvrTexture(data);
                                Bitmap tempTexture = vrfile.ToBitmap();
                                System.Drawing.Imaging.BitmapData bmpd = tempTexture.LockBits(new Rectangle(Point.Empty, tempTexture.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                                int stride = bmpd.Stride;
                                byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
                                System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
                                tempTexture.UnlockBits(bmpd);
                                int tlevels = 0;
                                archiveName = Path.GetFileNameWithoutExtension(filePath);
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
                                PvrDataFormat pdf;
                                if (!vrfile.HasMipmaps)
                                {
                                    if (tempTexture.Width == tempTexture.Height)
                                        pdf = PvrDataFormat.SquareTwiddled;
                                    else
                                        pdf = PvrDataFormat.Rectangle;
                                }
                                else
                                {
                                    if (tempTexture.Width == tempTexture.Height)
                                        pdf = PvrDataFormat.SquareTwiddledMipmaps;
                                    else
                                        pdf = PvrDataFormat.RectangleTwiddled;
                                }
                                PvrTextureEncoder encoder = new PvrTextureEncoder(tempTexture, ppf, pdf);
                                encoder.GlobalIndex = vrfile.GlobalIndex;
                                string pvrPath = Path.ChangeExtension(Path.Combine(path, file.Name), ".pvr");
                                if (!File.Exists(pvrPath))
                                    encoder.Save(pvrPath);
                                data.Close();
                                File.Delete(Path.Combine(path, file.Name));
                                pvmWriter.CreateEntryFromFile(pvrPath);
                                texList.WriteLine(Path.GetFileName(pvrPath));
                                Console.WriteLine("Adding texture {0}", pvrPath);
                            }
                            pvmWriter.Flush();
                            pvmStream.Flush();
                            pvmStream.Close();
                            if (isPRS)
                            {
                                Console.WriteLine("Compressing to PRS...");
                                byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pvm"));
                                pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
                                File.WriteAllBytes(Path.ChangeExtension(filePath, ".PVM.PRS"), pvmdata);
                                File.Delete(Path.ChangeExtension(filePath, ".PVM"));
                            }
                            Console.WriteLine("Archive converted!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception thrown: {0}", ex.ToString());
                            Console.WriteLine("Press ENTER to exit.");
                            Console.ReadLine();
                            return;
                        }
                    }
                    break;
                // CompilePVM mode
                case "-pvm":
                    bool IsPRS = false;
                    if (args[args.Length - 1] == "-prs") IsPRS = true;
                    filePath = args[1];
                    string FullLine;
                    string texturename;
                    uint GBIX = 0;
                    List<string> textureNames = new List<String>();
                    List<PvrTexture> finalTextureList = new List<PvrTexture>();
                    directoryName = Path.GetDirectoryName(filePath);
                    archiveName = Path.GetFileNameWithoutExtension(filePath);
                    if (Directory.Exists(filePath))
                    {
                        Console.WriteLine("Converting texture pack to PVM: {0}", filePath);
                        StreamReader texlistStream = File.OpenText(Path.Combine(filePath, "index.txt"));
                        while (!texlistStream.EndOfStream) textureNames.Add(texlistStream.ReadLine());
                        pvmArchive = new PvmArchive();
                        Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ".pvm"), FileMode.Create);
                        pvmWriter = (PvmArchiveWriter)pvmArchive.Create(pvmStream);
                        // Reading in textures
                        for (uint imgIndx = 0; imgIndx < textureNames.Count; imgIndx++)
                        {
                            FullLine = textureNames[(int)imgIndx];
                            if (string.IsNullOrEmpty(FullLine)) continue;
                            String[] substrings = FullLine.Split(',');
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
                                Console.WriteLine(String.Concat("Texture ", textureNames[(int)imgIndx], " not found. Generating a placeholder. Check your files."));
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
                            pvmWriter.CreateEntryFromFile(pvrPath);
                        }
                        pvmWriter.Flush();
                        pvmStream.Close();
                        if (IsPRS)
                        {
                            Console.WriteLine("Compressing to PRS...");
                            byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pvm"));
                            pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
                            File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), pvmdata);
                            File.Delete(Path.ChangeExtension(filePath, ".pvm"));
                        }
                        Console.WriteLine("Archive was compiled successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Supplied texture list does not exist!");
                        Console.WriteLine("Press ENTER to continue...");
                        Console.ReadLine();
                        return;
                    }
                    break;
                // Create PB mode
                case "-pb":
                    filePath = args[1];
                    Console.WriteLine("Building PB from folder: {0}", filePath);
                    if (Directory.Exists(filePath))
                    {
                        string indexfilename = Path.Combine(filePath, "index.txt");
                        if (!File.Exists(indexfilename))
                        {
                            Console.WriteLine("Supplied path does not have an index file.");
                            Console.WriteLine("Press ENTER to exit.");
                            Console.ReadLine();
                            return;
                        }
                        List<string> filenames = new List<string>(File.ReadAllLines(indexfilename).Where(a => !string.IsNullOrEmpty(a)));
                        PBFile pba = new PBFile(filenames.Count);
                        int l = 0;
                        foreach (string tex in filenames)
                        {
                            byte[] texbytes = File.ReadAllBytes(Path.Combine(filePath, tex));
                            pba.AddPVR(texbytes, l);
                            l++;
                        }
                        path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filePath)), Path.GetFileNameWithoutExtension(filePath));
                        string filename_full = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filePath)), Path.GetFileName(filePath) + ".pb");
                        Console.WriteLine("Output file: {0}", filename_full);
                        File.WriteAllBytes(filename_full, pba.GetBytes());
                    }
                    else
                    {
                        Console.WriteLine("Supplied path does not exist.");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    break;
                // Extract NjArchive mode
                case "-nju":
                    filePath = args[1];
                    NjArchive njarc = new NjArchive(File.ReadAllBytes(filePath));
                    Console.WriteLine("Extracting Ninja archive: {0}", filePath);
                    dir = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                    Console.WriteLine("Output folder: {0}", dir);
                    Directory.CreateDirectory(dir);
                    for (int i = 0; i < njarc.Entries.Count; i++)
                    {
                        byte[] data = njarc.Entries[i];
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
                            case "PVMH":
                                desc = "PVM";
                                extension = ".pvm";
                                break;
                        }
                        Console.WriteLine("Entry {0} is {1}", i, desc);
                        string outpath = Path.Combine(dir, i.ToString("D3") + extension);
                        File.WriteAllBytes(outpath, njarc.Entries[i]);
                    }
                    break;
                // PVM2TexPack mode
                case "-png":
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
                        path = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileNameWithoutExtension(filename));
                        string filename_full = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(filename)), Path.GetFileName(filename));
                        Console.WriteLine("Converting file to texture pack: {0}", filename_full);
                        Directory.CreateDirectory(path);
                        filedata = File.ReadAllBytes(filename_full);
                        using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
                        {
                            try
                            {
                                if (PvrTexture.Is(filedata))
                                {
                                    if (!AddTexture(false, path, Path.GetFileName(filename_full), new MemoryStream(filedata), texList))
                                    {
                                        texList.Close();
                                        Directory.Delete(path, true);
                                    }
                                    continue;
                                }
                                else if (GvrTexture.Is(filedata))
                                {
                                    if (!AddTexture(true, path, Path.GetFileName(filename_full), new MemoryStream(filedata), texList))
                                    {
                                        texList.Close();
                                        Directory.Delete(path, true);
                                    }
                                    continue;
                                }
                                bool gvm = false;
                                ArchiveBase pvmfile = null;
                                byte[] pvmdata = File.ReadAllBytes(filename_full);
                                if (Path.GetExtension(filename_full).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                                    pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
                                pvmfile = new PvmArchive();
                                MemoryStream stream = new MemoryStream(pvmdata);
                                if (!PvmArchive.Identify(stream))
                                {
                                    pvmfile = new GvmArchive();
                                    gvm = true;
                                }
                                ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
                                bool fail = false;
                                foreach (ArchiveEntry file in pvmentries)
                                    if (!AddTexture(gvm, path, file.Name, file.Open(), texList))
                                    {
                                        texList.Close();
                                        Directory.Delete(path, true);
                                        fail = true;
                                        break;
                                    }
                                if (fail)
                                    continue;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception thrown: " + ex.ToString() + "\nCanceling conversion.");
                                return;
                            }
                            Console.WriteLine("Conversion complete!");
                        }
                    }
                    break;
                // Other modes
                default:
                    filePath = args[0];
                    IsPRS = false;
                    bool IsGVM = false;
                    bool IsBIN = false;
                    bool isDAT = false;
                    if (args.Length > 1 && args[1] == "-prs") IsPRS = true;
                    extension = Path.GetExtension(filePath).ToLowerInvariant();
                    //Folder mode
                    if (Directory.Exists(filePath))
                    {
                        string indexfilename = Path.Combine(filePath, "index.txt");
                        List<string> filenames = new List<string>(File.ReadAllLines(indexfilename).Where(a => !string.IsNullOrEmpty(a)));
                        string ext = Path.GetExtension(filenames[0]).ToLowerInvariant();
                        if (filenames.Any(a => !Path.GetExtension(a).Equals(ext, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("Cannot create archive from mixed file types.");
                            Console.WriteLine("Press ENTER to exit.");
                            Console.ReadLine();
                            return;
                        }
                        switch (ext)
                        {
                            case ".pvr":
                                Console.WriteLine("Creating PVM archive from folder: {0}", filePath);
                                pvmArchive = new PvmArchive();
                                break;
                            case ".gvr":
                                Console.WriteLine("Creating GVM archive from folder: {0}", filePath);
                                pvmArchive = new GvmArchive();
                                IsGVM = true;
                                break;
                            case ".wav":
                                Console.WriteLine("Creating DAT archive from folder: {0}", filePath);
                                isDAT = true;
                                pvmArchive = null;
                                break;
                            default:
                                Console.WriteLine("Unknown file type \"{0}\".", ext);
                                Console.WriteLine("Press ENTER to exit.");
                                Console.ReadLine();
                                return;
                        }
                        if (isDAT)
                        {
                            // Load index
                            DATFile dat = new DATFile();
                            TextReader tr = File.OpenText(Path.Combine(filePath, "index.txt"));
                            string line = tr.ReadLine();
                            while (line != null)
                            {
                                Console.WriteLine("Adding file {0}", Path.Combine(filePath, line));
                                dat.AddFile(Path.Combine(filePath, line));
                                line = tr.ReadLine();
                            }
                            tr.Close();
                            // Save DAT archive
                            File.WriteAllBytes(filePath + ".DAT", dat.GetBytes());
                            if (IsPRS)
                            {
                                Console.WriteLine("Compressing to PRS...");
                                byte[] datdata = File.ReadAllBytes(filePath + ".DAT");
                                datdata = FraGag.Compression.Prs.Compress(datdata);
                                File.WriteAllBytes(filePath + ".PRS", datdata);
                                File.Delete(filePath + ".DAT");
                            }
                            Console.WriteLine("Archive compiled successfully!");
                            return;
                        }
                        else
                        {
                            if (!IsGVM) ext = ".pvm"; else ext = ".gvm";
                            using (Stream pvmStream = File.Open(Path.ChangeExtension(filePath, ext), FileMode.Create))
                            {
                                pvmWriter = pvmArchive.Create(pvmStream);
                                // Reading in textures
                                foreach (string tex in filenames)
                                {
                                    if (!IsGVM) pvmWriter.CreateEntryFromFile(Path.Combine(filePath, Path.ChangeExtension(tex, ".pvr")));
                                    else pvmWriter.CreateEntryFromFile(Path.Combine(filePath, Path.ChangeExtension(tex, ".gvr")));
                                    Console.WriteLine("Adding file: {0}", tex);
                                }
                                pvmWriter.Flush();
                            }
                        }
                        if (IsPRS)
                        {
                            Console.WriteLine("Compressing to PRS...");
                            byte[] pvmdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ext));
                            pvmdata = FraGag.Compression.Prs.Compress(pvmdata);
                            File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), pvmdata);
                            File.Delete(Path.ChangeExtension(filePath, ext));
                        }
                        Console.WriteLine("Archive was compiled successfully!");
                        return;
                    }
                    //Continue with file mode otherwise
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("Supplied archive/texture list does not exist!");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    switch (extension)
                    {
                        case ".rel":
                            Console.WriteLine("Decompressing REL file: {0}", filePath);
                            byte[] input = File.ReadAllBytes(args[0]);
                            byte[] output = SA_Tools.HelperFunctions.DecompressREL(input);
                            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_dec.rel"), output);
                            return;
                        case ".dat":
                            Console.WriteLine("Extracting DAT file: {0}", filePath);
                            DATFile dat = new DATFile(File.ReadAllBytes(filePath));
                            dir = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                            if (Directory.Exists(dir)) Directory.Delete(dir, true);
                            Directory.CreateDirectory(dir);
                            using (StreamWriter sw = File.CreateText(Path.Combine(dir, "index.txt")))
                            {
                                dat.Entries.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.name, f2.name));
                                for (int i = 0; i < dat.GetCount(); i++)
                                {
                                    string fname = dat.Entries[i].name;
                                    sw.WriteLine(fname);
                                    if (dat.Steam)
                                    {
                                        fname = Path.GetFileNameWithoutExtension(fname) + ".adx";
                                    }
                                    Console.WriteLine("Extracting file: {0}", fname);
                                    File.WriteAllBytes(Path.Combine(dir, fname), dat.GetFile(i));
                                }
                                sw.Flush();
                                sw.Close();
                            }
                            Console.WriteLine("Archive extracted!");
                            break;
                        case ".pb":
                            Console.WriteLine("Extracting PB file: {0}", filePath);
                            byte[] pbdata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".pb"));
                            dir = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                            Directory.CreateDirectory(dir);
                            PBFile pba = new PBFile(pbdata);
                            using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
                            {
                                for (int u = 0; u < pba.GetCount(); u++)
                                {
                                    byte[] pvrt = pba.GetPVR(u);
                                    string outpath = Path.Combine(dir, u.ToString("D3") + ".pvr");
                                    File.WriteAllBytes(outpath, pvrt);
                                    texList.WriteLine(u.ToString("D3") + ".pvr");
                                }
                                texList.Flush();
                                texList.Close();
                            }
                            Console.WriteLine("Archive extracted!");
                            break;
                        case ".bin":
                            Console.WriteLine("Compressing BIN file: {0}", filePath);
                            byte[] bindata = File.ReadAllBytes(Path.ChangeExtension(filePath, ".bin"));
                            bindata = FraGag.Compression.Prs.Compress(bindata);
                            File.WriteAllBytes(Path.ChangeExtension(filePath, ".prs"), bindata);
                            Console.WriteLine("PRS archive was compiled successfully!");
                            return;
                        case ".prs":
                        case ".pvm":
                        case ".gvm":
                            Console.WriteLine("Extracting archive: {0}", filePath);
                            path = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                            Directory.CreateDirectory(path);
                            filedata = File.ReadAllBytes(filePath);
                            using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
                            {
                                try
                                {
                                    ArchiveBase pvmfile = null;
                                    byte[] pvmdata = File.ReadAllBytes(filePath);
                                    if (extension == ".prs") pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
                                    pvmfile = new PvmArchive();
                                    MemoryStream stream = new MemoryStream(pvmdata);
                                    if (!PvmArchive.Identify(stream))
                                    {
                                        pvmfile = new GvmArchive();
                                        if (!GvmArchive.Identify(stream))
                                        {
                                            File.WriteAllBytes(Path.ChangeExtension(filePath, ".bin"), pvmdata);
                                            IsBIN = true;
                                            Console.WriteLine("PRS archive extracted!");
                                        }
                                    }
                                    if (!IsBIN)
                                    {
                                        ArchiveReader pvmReader = pvmfile.Open(pvmdata);
                                        foreach (ArchiveEntry pvmentry in pvmReader.Entries)
                                        {
                                            Console.WriteLine("Extracting file: {0}", pvmentry.Name);
                                            texList.WriteLine(pvmentry.Name);
                                            pvmReader.ExtractToFile(pvmentry, Path.Combine(path, pvmentry.Name));
                                        }
                                        Console.WriteLine("Archive extracted!");
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("Exception thrown. Canceling conversion.");
                                    Console.WriteLine("Press ENTER to exit.");
                                    Console.ReadLine();
                                    Directory.Delete(path, true);
                                    throw;
                                }
                            }
                            if (IsBIN)
                            {
                                Directory.Delete(path, true);
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown extension \"{0}\".", extension);
                            Console.WriteLine("Press ENTER to exit.");
                            Console.ReadLine();
                            break;
                    }
                    break;
            }
        }
		static bool AddTexture(bool gvm, string path, string filename, Stream data, TextWriter index)
		{
			Console.WriteLine("Adding texture: {0}", filename);
			VrTexture vrfile = gvm ? (VrTexture)new GvrTexture(data) : (VrTexture)new PvrTexture(data);
			if (vrfile.NeedsExternalPalette)
			{
				Console.WriteLine("Cannot convert texture files which require external palettes!");
				return false;
			}
			Bitmap bmp;
			try { bmp = vrfile.ToBitmap(); }
			catch { bmp = new Bitmap(1, 1); }
			bmp.Save(Path.Combine(path, Path.ChangeExtension(filename, "png")));
			bmp.Dispose();
			index.WriteLine("{0},{1}", vrfile.HasGlobalIndex ? vrfile.GlobalIndex : uint.MaxValue, Path.ChangeExtension(filename, "png"));
			return true;
		}
        	

    }
}
