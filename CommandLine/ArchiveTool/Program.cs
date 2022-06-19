using System;
using System.IO;

namespace ArchiveTool
{
    static partial class Program
    {
        static bool compressPRS;
        static string filePath;
        static string extension;

        static void Main(string[] args)
        {
            // Usage
            if (args.Length == 0)
            {
                Console.WriteLine("ArchiveTool is a command line tool to extract and create archives in various formats used in SA1/2.");
                Console.WriteLine("Usage:\n");
                Console.WriteLine("Extracting a PVM/GVM/XVM/PRS/PB/PVMX/DAT/PAK/REL/ARCX file:\nArchiveTool <archivefile>\nIf the archive is PRS compressed, it will be decompressed first.\nIf the archive contains textures/sounds, the program will extract them and create index file(s).\n");
                Console.WriteLine("Extracting an NjUtil archive: ArchiveTool -nju <archivefile>\n");
				Console.WriteLine("Creating an ARCX archive: ArchiveTool -arcx <path>\n");
				Console.WriteLine("Converting a PVM/GVM to a folder texture pack (PVM2TexPack mode): ArchiveTool -png <archivefile>\n");
                Console.WriteLine("Converting GVM to PVM (lossy): ArchiveTool -gvm2pvm <file.gvm> [-prs]\n");
                Console.WriteLine("Creating a PVM from a folder texture pack (CompilePVM mode): ArchiveTool -pvm <folder> [-prs]\nThe texture list 'index.txt' must contain global indices listed before each texture filename for this option to work.\n");
                Console.WriteLine("Creating a PVM/GVM/XVM/DAT/PB/PVMX/PAK from a folder with textures/sounds: ArchiveTool <foldername> [-pb] [-prs]\nOnly PVR/GVR/XVR textures can be used for PVMs/GVMs/XVMs in this mode.\nThe program will create an archive from files listed in 'index.txt' (or INI file for PAK) in the folder.\nThe -prs option will make the program output a PRS compressed archive.\nThe -pb option will make it output a PB archive instead of PVM.\n");
                Console.WriteLine("Creating a PRS compressed binary: ArchiveTool -prs <file>\n");
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
						case ".xvm":
                        case ".mdl":
                        case ".mdt":
                        case ".mld":
                        case ".mlt":
						case ".arcx":
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