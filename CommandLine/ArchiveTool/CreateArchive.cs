using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using VrSharp.Pvr;
using ArchiveLib;
using SplitTools;
using static ArchiveLib.DATFile;
using static ArchiveLib.PVMXFile;
using static ArchiveLib.ARCXFile;
using static ArchiveLib.MLTFile;
using static ArchiveLib.gcaxMLTFile;
using static ArchiveLib.AFSFile;

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
			bool createARCX = false;
			bool createAFS = false;
			int afsblock = 0x80000;
			AFSMetaMode afsmetamode = AFSMetaMode.OffsetEndTable;
            filePath = args[0];
            compressPRS = false;
            for (int a = 0; a < args.Length; a++)
            {
                if (args[a] == "-prs") compressPRS = true;
                if (args[a] == "-pb") createPB = true;
				if (args[a] == "-arcx") createARCX = true;
				if (args[a] == "-afs") createAFS = true;
				if (args[a] == "-nometa") afsmetamode = AFSMetaMode.NoMeta;
				if (args[a] == "-metafirst") afsmetamode = AFSMetaMode.OffsetBeforeFirstEntry;
				if (args[a] == "-block") afsblock = int.Parse(args[a + 1], System.Globalization.NumberStyles.HexNumber);
			}
            // Folder mode
            if (Directory.Exists(filePath))
            {
                GenericArchive arc;
                string indexfilename = Path.Combine(filePath, "index.txt");
				if (createARCX)
				{
					CreateARCX(filePath);
					return;
				}
                if (!File.Exists(indexfilename))
                {
                    BuildPAK(filePath);
                    return;
                }
                List<string> filenames = new List<string>(File.ReadAllLines(indexfilename).Where(a => !string.IsNullOrEmpty(a)));
                string ext = Path.GetExtension(filenames[0]).ToLowerInvariant();
                if (filenames[0].Contains(","))
                {
                    string[] checkf = filenames[0].Split(',');
                    ext = Path.GetExtension(checkf[0].ToLowerInvariant());
                }
				if (createAFS)
				{
					folderMode = ArchiveFromFolderMode.AFS;
					arc = new AFSFile(AFSType.AFS1, afsmetamode, afsblock);
				}
				else
				{
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
							arc = new PuyoFile(PuyoArchiveType.GVMFile);
							folderMode = ArchiveFromFolderMode.GVM;
							break;
						case ".xvr":
							arc = new PuyoFile(PuyoArchiveType.XVMFile);
							folderMode = ArchiveFromFolderMode.XVM;
							break;
						case ".wav":
						case ".adx":
							folderMode = ArchiveFromFolderMode.DAT;
							arc = new DATFile();
							break;
						case ".mpb":
						case ".mdb":
						case ".msb":
						case ".osb":
						case ".fpb":
						case ".fob":
						case ".fpw":
						case ".psr":
							folderMode = ArchiveFromFolderMode.MLT;
							arc = new MLTFile();
							break;
						case ".gcaxmpb":
						case ".gcaxmsb":
							folderMode = ArchiveFromFolderMode.gcaxMLT;
							arc = new gcaxMLTFile();
							break;
						case ".nj":
						case ".gj":
						case "njm":
						case "njtl":
							folderMode = ArchiveFromFolderMode.NJ;
							arc = new NinjaBinaryFile();
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
				}
                Console.WriteLine("Creating {0} archive from folder: {1}", folderMode.ToString(), filePath);
                int id = 0;
				bool gj = false;
                foreach (string line in filenames)
                {
                    string[] split = line.Split(',');
                    string filename = split[0];
                    switch (folderMode)
                    {
						case ArchiveFromFolderMode.NJ:
							arc.Entries.Add(new NinjaBinaryEntry(Path.Combine(filePath, filename)));
							if (Path.GetExtension(filename).ToUpperInvariant().Contains("GJ"))
								gj = true;
							extension = gj ? ".gj" : ".nj";
							break;
						case ArchiveFromFolderMode.AFS:
							uint customData = 0;
							if (split.Length > 1)
								customData = uint.Parse(split[1]);
							arc.Entries.Add(new AFSEntry(Path.Combine(filePath, filename), File.GetLastWriteTime(Path.Combine(filePath, filename)), customData));
							extension = ".afs";
							break;
						case ArchiveFromFolderMode.gcaxMLT:
                            int bIDgc = int.Parse(split[1]);
                            arc.Entries.Add(new gcaxMLTEntry(Path.Combine(filePath, filename), bIDgc));
                            extension = ".mlt";
                            break;
                        case ArchiveFromFolderMode.MLT:
                            int bID = int.Parse(split[1]);
                            int mem = int.Parse(split[2], System.Globalization.NumberStyles.HexNumber);
                            int sz = int.Parse(split[3]);
                            int version = 1;
                            int revision = 1;
                            string versionfilename = Path.Combine(filePath, "version.txt");
                            if (File.Exists(versionfilename))
                            {
                                string[] ver = File.ReadAllLines(versionfilename);
                                version = int.Parse(ver[0]);
                                revision = int.Parse(ver[1]);
                                MLTFile mlt = (MLTFile)arc;
                                mlt.Version = (byte)version;
                                mlt.Revision = (byte)revision;
                            }
                            arc.Entries.Add(new MLTEntry(Path.Combine(filePath, filename), bID, mem, sz));
                            extension = ".mlt";
                            break;
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
						case ArchiveFromFolderMode.XVM:
							arc.Entries.Add(new XVMEntry(Path.Combine(filePath, filename)));
							extension = ".xvm";
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
        static void BuildPAK(string filePath)
        {
            Console.WriteLine("Building PAK from folder: {0}", Path.GetFullPath(filePath));
            outputPath = Path.Combine(Environment.CurrentDirectory, filePath);
            Environment.CurrentDirectory = Path.GetDirectoryName(outputPath);
            string inipath = Path.Combine(Path.GetFileNameWithoutExtension(outputPath), Path.GetFileNameWithoutExtension(outputPath) + ".ini");
            if (!File.Exists(inipath))
            {
                Console.WriteLine("PAK INI file not found: {0}", inipath);
                Console.WriteLine("The folder must contain either index.txt or an INI file to be recognized as a buildable archive folder.");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
                return;
            }
            PAKFile.PAKIniData list = IniSerializer.Deserialize<PAKFile.PAKIniData>(inipath);
            PAKFile pak = new PAKFile() { FolderName = list.FolderName };
            foreach (KeyValuePair<string, PAKFile.PAKIniItem> item in list.Items)
            {
                Console.WriteLine("Adding file: {0}", item.Key);
                pak.Entries.Add(new PAKFile.PAKEntry(item.Key, item.Value.LongPath, File.ReadAllBytes(Path.Combine(Path.GetFileNameWithoutExtension(outputPath), item.Key))));
            }
            Console.WriteLine("Output file: {0}", Path.ChangeExtension(outputPath, "pak"));
            pak.Save(Path.ChangeExtension(outputPath, "pak"));
            Console.WriteLine("Done!");
        }
		/// <summary>
		/// Create an ARCX archive from a folder.
		/// </summary>
		static void CreateARCX(string filePath)
		{
			Console.WriteLine("Building ARCX from folder: {0}", Path.GetFullPath(filePath));
			outputPath = Path.Combine(Environment.CurrentDirectory, filePath);
			Environment.CurrentDirectory = Path.GetDirectoryName(outputPath);
			ARCXFile arc = new ARCXFile();
			string[] filenames = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories);
			int id = 0;
			foreach (string line in filenames)
			{
				string folderEntry = Path.GetDirectoryName(Path.GetRelativePath(filePath, line));
				arc.Entries.Add(new ARCXEntry(Path.GetFileName(line), folderEntry, File.ReadAllBytes(line)));
				Console.WriteLine("Added entry {0}: {1}", id.ToString(), line);
				id++;
			}
			byte[] data = arc.GetBytes();
			outputPath = Path.GetFullPath(filePath) + ".arcx";
			if (compressPRS)
			{
				Console.WriteLine("Compressing to PRS...");
				data = FraGag.Compression.Prs.Compress(data);
				outputPath = Path.ChangeExtension(outputPath, ".PRS");
			}
			Console.WriteLine("Output file: {0}", outputPath);
			File.WriteAllBytes(outputPath, data);
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
                PuyoFile puyo = new PuyoFile();
                outputPath = Path.ChangeExtension(filePath, ".pvm");
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
                    puyo.Entries.Add(new PVMEntry(pvrPath));
                }
                File.WriteAllBytes(outputPath, puyo.GetBytes());
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
            PVM,
            GVM,
            DAT,
            PVMX,
            PB,
            MLT,
            gcaxMLT,
			XVM,
			AFS,
			NJ
        }
    }
}