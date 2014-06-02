using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using PuyoTools.Modules;
using PuyoTools.Modules.Archive;
using VrSharp;
using VrSharp.PvrTexture;

namespace CompilePVM
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryName;
            UInt32 startingGBIX = 0;
            List<String> textureNames = new List<String>();
            List<PvrTexture> finalTextureList = new List<PvrTexture>();
 
            if (args.Length == 0)
            {
                Console.WriteLine("Error - no texturelist provided. Provide a path to a texturelist as your first command line argument");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }

            String filePath = args[0];
            directoryName = Path.GetDirectoryName(filePath);
            string archiveName = Path.GetFileNameWithoutExtension(filePath);

            if (File.Exists(filePath))
            {
                StreamReader texlistStream = File.OpenText(filePath);

                startingGBIX = UInt32.Parse(texlistStream.ReadLine());

                while (!texlistStream.EndOfStream)
                {
                    string newName = texlistStream.ReadLine();
                    textureNames.Add(newName);
                }

                PvmArchive pvmArchive = new PvmArchive();
                Stream pvmStream = File.Open(String.Concat(directoryName, String.Format("//{0}.pvm", archiveName)), FileMode.Create);
                PvmArchiveWriter pvmWriter = (PvmArchiveWriter)pvmArchive.Create(pvmStream);

                // Reading in textures
                for (uint imgIndx = 0; imgIndx < textureNames.Count; imgIndx++)
                {
                    Bitmap tempTexture = new Bitmap(8, 8);
                    string texturePath = String.Concat(directoryName, String.Format("//{0}.png", textureNames[(int)imgIndx]));
                    if (File.Exists(texturePath))
                    {
                        tempTexture = (Bitmap)Bitmap.FromFile(texturePath);
                    }
                    else
                    {
                        Console.WriteLine(String.Concat("Texture ", textureNames[(int)imgIndx], " not found. Generating a placeholder. Check your files."));
                    }

                    PvrTextureEncoder encoder = new PvrTextureEncoder(tempTexture, PvrPixelFormat.Argb4444, PvrDataFormat.Rectangle); // currently using the most wasteful format. Do checks to make this more accurate later.
                    encoder.GlobalIndex = startingGBIX + imgIndx;
                    encoder.Save(String.Concat(directoryName, String.Format("//{0}.pvr", textureNames[(int)imgIndx])));

                    pvmWriter.CreateEntryFromFile(String.Concat(directoryName, String.Format("//{0}.pvr", textureNames[(int)imgIndx])));
                }

                pvmWriter.Flush();
                pvmStream.Close();

                Console.WriteLine("PVM was compiled successfully!");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }
            else // error, supplied path is invalid
            {
                Console.WriteLine("Supplied texturelist does not exist!");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
                return;
            }
        }
    }
}
