using System;
using System.IO;
using PuyoTools.Modules.Archive;

namespace ArchiveTool
{
    static partial class Program
    {
        static bool compressPRS;
        static string filePath;
        static string extension;
        static ArchiveBase puyoArchiveBase;
        static ArchiveWriter puyoArchiveWriter;
        static void Main(string[] args)
        {
            // Usage
            if (args.Length == 0)
            {
                Console.WriteLine("ArchiveTool is a command line tool to extract and create archives in various formats used in SA1/2.");
                Console.WriteLine("Usage:\n");
                Console.WriteLine("Extracting a PVM/GVM/PRS/PB/PVMX/DAT/PAK/REL file:\nArchiveTool <archivefile>\nIf the archive is PRS compressed, it will be decompressed first.\nIf the archive contains textures/sounds, the program will extract them and create index file(s).\n");
                Console.WriteLine("Extracting an NjUtil archive: ArchiveTool -nju <archivefile>\n");
                Console.WriteLine("Converting a PVM/GVM to a folder texture pack (PVM2TexPack mode): ArchiveTool -png <archivefile>\n");
                Console.WriteLine("Converting GVM to PVM (lossy): ArchiveTool -gvm2pvm <file.gvm> [-prs]\n");
                Console.WriteLine("Creating a PVM from a folder texture pack (CompilePVM mode): ArchiveTool -pvm <folder> [-prs]\nThe texture list 'index.txt' must contain global indices listed before each texture filename for this option to work.\n");
                Console.WriteLine("Creating a PAK archive (PAKTool mode): ArchiveTool -pak <foldername>\n");
                Console.WriteLine("Creating a PVM/GVM/DAT/PB/PVMX from a folder with textures/sounds: ArchiveTool <foldername> [-prs]\nOnly PVR and GVR textures can be used for PVMs/GVMs in this mode.\nThe program will create an archive from files listed in 'index.txt' in the folder.\nThe -prs option will make the program output a PRS compressed archive.\n");
                Console.WriteLine("Creating a PRS compressed binary: ArchiveTool <file.bin>\nFile extension must be .BIN for this option to work.\n");
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
                return;
            }
            filePath = args[0];
            extension = Path.GetExtension(filePath.ToLowerInvariant());
            switch (args[0].ToLowerInvariant())
            {
                #region Convert archives
                // GVM2PVM mode
                case "-gvm2pvm":
                    GVM2PVM(args);
                    break;
                // PVM2TexPack mode
                case "-png":
                    Puyo2Texpack(args);
                    break;
                #endregion

                #region Create archives
                // Create PRS
                case "-prs":
                    CompressPRS(args);
                    return;
                // Create PVM from a texture pack
                case "-pvm":
                    TexPack2PVM(args);
                    break;
                // Create PAK
                case "-pak":
                    BuildPAK(args);
                    break;
                #endregion

                #region Extracting archives
                // Extract NjArchive
                case "-nju":
                    ExtractNjUtil(args);
                    break;

                // Extract mode
                default:
                    if (Directory.Exists(filePath))
                    {
                        BuildFromFolder(args);
                        return;
                    }
                    else if (!File.Exists(filePath))
                    {
                        Console.WriteLine("Supplied archive/texture list does not exist!");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        return;
                    }
                    switch (extension)
                    {
                        case ".pak":  
                        case ".rel":
                        case ".dat":
                        case ".pvmx":
                        case ".pb":
                        case ".prs":
                        case ".pvm":
                        case ".gvm":
                            ExtractArchive(args);
                            return;
                        case ".bin":
                            CompressPRS(args);
                            return;
                        default:
                            Console.WriteLine("Unknown extension \"{0}\".", extension);
                            Console.WriteLine("Press ENTER to exit.");
                            Console.ReadLine();
                            break;
                    }
                    break;
                    #endregion
            }
        }
    }
}