using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using PuyoTools.Modules.Archive;
using PuyoTools.Modules.Texture;
using PuyoTools.Modules.Compression;

using VrSharp;
using VrSharp.GvrTexture;
using VrSharp.PvrTexture;

namespace SonicRetro.SAModel.Direct3D.TextureSystem
{
	/// <summary>
	/// This TextureArchive class is the primary interface for retrieving a texture list/array from a container format, such as PVM/GVM, and eventually PAK.
	/// </summary>
	public static class TextureArchive
	{
        public static BMPInfo[] GetTextures(string fileName)
        {
            List<BMPInfo> functionReturnValue = new List<BMPInfo>();

            using (FileStream inStream = File.OpenRead(fileName))
            {
                Stream source = inStream;

                bool gvm = false; // not yet used.

                ArchiveFormat format = Archive.GetFormat(source, fileName);
                
                if (format == ArchiveFormat.Unknown)
                {
                    // handling compression
                    CompressionFormat compressionFormat = Compression.GetFormat(source, fileName);

                    if (compressionFormat != CompressionFormat.Unknown)
                    {
                        // Ok, it appears to be compressed. Let's decompress it, and then check the format again
                        source = new MemoryStream();
                        Compression.Decompress(inStream, source, compressionFormat);

                        source.Position = 0;
                        format = Archive.GetFormat(source, Path.GetFileName(fileName));
                    }

                    if (format == ArchiveFormat.Unknown)
                    {
                        System.Windows.Forms.MessageBox.Show("Texture archive compression/format could not be determined. Continuing without Textures.");
                        return functionReturnValue.ToArray();
                    }
                }

                if (format == ArchiveFormat.Gvm) gvm = true;
  
                PvmArchive pvmArchive = new PvmArchive();
                GvmArchive gvmArchive = new GvmArchive();

                ArchiveReader archive;

                if (!gvm) archive = pvmArchive.Open(source);
                else archive = gvmArchive.Open(source);

                // processing texture entries
                for (int entryIndx = 0; entryIndx < archive.Entries.Count; entryIndx++)
                {
                    ArchiveEntry entry = archive.Entries[entryIndx];
                    Stream entryData = entry.Open();

                    // Get the texture format, if it is a texture that is.
                    TextureFormat textureFormat = Texture.GetFormat(entryData, entry.Name);
                    if (textureFormat != TextureFormat.Unknown)
                    {
                        // Ok, it appears to be a texture. We're going to attempt to convert it here.
                        try
                        {
                            MemoryStream textureData = new MemoryStream();
                            Texture.Read(entryData, textureData, textureFormat);

                            // If no exception was thrown, then we are all good doing what we need to do
                            entryData = textureData;
                            entryData.Position = 0;
                        }
                        catch (TextureNeedsPaletteException)
                        {
                            // Uh oh, looks like we need a palette.
                            // We are going to alert the user of the texture's name, and that they must provide the palette.
                            using (System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog
                            {
                                DefaultExt = gvm ? "gvp" : "pvp",
                                Filter = gvm ? "GVP Files|*.gvp" : "PVP Files|*.pvp",
                                InitialDirectory = System.IO.Path.GetDirectoryName(fileName),
                                Title = "External palette file"
                            })
                            {
                                a.FileName = entry.Name;
                                System.Windows.Forms.DialogResult paletteOpenResult = a.ShowDialog();

                                if ((paletteOpenResult == System.Windows.Forms.DialogResult.OK) && (File.Exists(a.FileName)))
                                {
                                    using (FileStream inPaletteStream = File.OpenRead(a.FileName))
                                    {
                                        MemoryStream textureData = new MemoryStream();
                                        TextureBase texture = Texture.Formats[textureFormat];
                                        texture.PaletteStream = inPaletteStream;
                                        texture.PaletteLength = (int)inPaletteStream.Length;
                                        entryData.Position = 0;
                                        texture.Read(entryData, textureData, (int)entryData.Length);

                                        entryData = textureData;
                                        entryData.Position = 0;
                                    }
                                }
                                else // user couldn't or didn't find file. Create placeholder.
                                {
                                    // Invalid palette. Provide a placeholder so texid values don't break/shift
                                    Bitmap newBitmap = new Bitmap(2, 2);
                                    functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(entry.Name), newBitmap));
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        // unknown texture format. Provide a placeholder so texid values don't break/shift
                        Bitmap newBitmap = new Bitmap(2, 2);
                        functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(entry.Name), newBitmap));
                        continue;
                    }

                    // if we've made it this far, the texture is good. Convert to BMPInfo

                    Bitmap currentTexture = (Bitmap)Bitmap.FromStream(entryData);
                    functionReturnValue.Add(new BMPInfo(Path.GetFileNameWithoutExtension(entry.Name), currentTexture));
                }
            }

            return functionReturnValue.ToArray();
        }
    }

    /// <summary>
    /// This was taken nearly vertabim from PuyoTools. The only change is to the static constructor. It is no longer a method called Initialize(),
    /// and the GimTexture format has been removed to prevent an error with assembly loading.
    /// </summary>
    static class Texture
    {
        // Texture format dictionary
        public static Dictionary<TextureFormat, TextureBase> Formats;

        // Initalize the texture format dictionary
        static Texture()
        {
            Formats = new Dictionary<TextureFormat, TextureBase>();

            Formats.Add(TextureFormat.Gim, null);
            Formats.Add(TextureFormat.Gvr, new PuyoTools.Modules.Texture.GvrTexture());
            Formats.Add(TextureFormat.Pvr, new PuyoTools.Modules.Texture.PvrTexture());
            Formats.Add(TextureFormat.Svr, new SvrTexture());
        }

        // Reads a texture with the specified texture format
        public static void Read(Stream source, Stream destination, TextureFormat format)
        {
            Formats[format].Read(source, destination);
        }

        // Reads a texture with the specified texture format and returns a bitmap
        public static void Read(Stream source, out Bitmap destination, TextureFormat format)
        {
            Formats[format].Read(source, out destination);
        }

        // Writes a texture to the specified texture format
        public static void Write(Stream source, Stream destination, TextureFormat format)
        {
            Formats[format].Write(source, destination);
        }

        // Returns the format used by the source texture.
        public static TextureFormat GetFormat(Stream source, string fname)
        {
            foreach (KeyValuePair<TextureFormat, TextureBase> format in Formats)
            {
                if (format.Value == null) continue;
                if (format.Value.Is(source, fname))
                    return format.Key;
            }

            return TextureFormat.Unknown;
        }

        // Returns the module for this texture format.
        public static TextureBase GetModule(TextureFormat format)
        {
            return Formats[format];
        }
    }

    // List of texture formats
    enum TextureFormat
    {
        Unknown,
        Gim,
        Gvr,
        Pvr,
        Svr,
        Plugin,
    }

    /// <summary>
    /// This too was taken from PuyoTools. The only change so far is changing the initializer to a static constructor.
    /// </summary>
    public static class Archive
    {
        // Archive format dictionary
        public static Dictionary<ArchiveFormat, ArchiveBase> Formats;

        // Initalize the archive format dictionary
        static Archive()
        {
            Formats = new Dictionary<ArchiveFormat, ArchiveBase>();

            Formats.Add(ArchiveFormat.Acx, new AcxArchive());
            Formats.Add(ArchiveFormat.Afs, new AfsArchive());
            Formats.Add(ArchiveFormat.Gnt, new GntArchive());
            Formats.Add(ArchiveFormat.Gvm, new GvmArchive());
            Formats.Add(ArchiveFormat.Mrg, new MrgArchive());
            Formats.Add(ArchiveFormat.Narc, new NarcArchive());
            Formats.Add(ArchiveFormat.OneUnleashed, new OneUnleashedArchive());
            Formats.Add(ArchiveFormat.Pvm, new PvmArchive());
            Formats.Add(ArchiveFormat.Snt, new SntArchive());
            Formats.Add(ArchiveFormat.Spk, new SpkArchive());
            Formats.Add(ArchiveFormat.Tex, new TexArchive());
            Formats.Add(ArchiveFormat.U8, new U8Archive());
        }

        // Opens an archive with the specified archive format.
        public static ArchiveReader Open(Stream source, ArchiveFormat format)
        {
            return Formats[format].Open(source);
        }

        // Creates an archive with the specified archive format and writer settings.
        public static ArchiveWriter Create(Stream source, ArchiveFormat format)
        {
            return Formats[format].Create(source);
        }

        // Returns the archive format used by the source archive.
        public static ArchiveFormat GetFormat(Stream source, string fname)
        {
            foreach (KeyValuePair<ArchiveFormat, ArchiveBase> format in Formats)
            {
                if (format.Value.Is(source, fname))
                    return format.Key;
            }

            return ArchiveFormat.Unknown;
        }

        // Returns the module for this archive format.
        public static ArchiveBase GetModule(ArchiveFormat format)
        {
            return Formats[format];
        }
    }

    // List of archive formats
    public enum ArchiveFormat
    {
        Unknown,
        Acx,
        Afs,
        Gnt,
        Gvm,
        Mrg,
        Narc,
        OneUnleashed,
        Pvm,
        Snt,
        Spk,
        Tex,
        U8,
        Plugin,
    }

    public static class Compression
    {
        // Compression format dictionary
        public static Dictionary<CompressionFormat, CompressionBase> Formats;

        // Initalize the compression format dictionary
        static Compression()
        {
            Formats = new Dictionary<CompressionFormat, CompressionBase>();

            Formats.Add(CompressionFormat.Cnx, new CnxCompression());
            Formats.Add(CompressionFormat.Cxlz, new CxlzCompression());
            Formats.Add(CompressionFormat.Lz00, new Lz00Compression());
            Formats.Add(CompressionFormat.Lz01, new Lz01Compression());
            Formats.Add(CompressionFormat.Lz10, new Lz10Compression());
            Formats.Add(CompressionFormat.Lz11, new Lz11Compression());
            Formats.Add(CompressionFormat.Prs, new PrsCompression());
        }

        // Decompress a file when the compression format is not known.
        public static CompressionFormat Decompress(Stream source, Stream destination, string fname)
        {
            CompressionFormat format = GetFormat(source, fname);

            if (format == CompressionFormat.Unknown)
                return format;

            Formats[format].Decompress(source, destination);

            return format;
        }

        // Decompress a file with the specified compression format
        public static void Decompress(Stream source, Stream destination, CompressionFormat format)
        {
            Formats[format].Decompress(source, destination);
        }

        // Compress a file with the specified compression format
        public static void Compress(Stream source, Stream destination, CompressionFormat format)
        {
            Formats[format].Compress(source, destination);
        }

        // Returns the compression format used by the source file.
        public static CompressionFormat GetFormat(Stream source, string fname)
        {
            foreach (KeyValuePair<CompressionFormat, CompressionBase> format in Formats)
            {
                if (format.Value.Is(source, fname))
                    return format.Key;
            }

            return CompressionFormat.Unknown;
        }

        // Returns the module for this compression format.
        public static CompressionBase GetModule(CompressionFormat format)
        {
            return Formats[format];
        }
    }

    // List of compression formats
    public enum CompressionFormat
    {
        Unknown,
        Cnx,
        Cxlz,
        Lz00,
        Lz01,
        Lz10,
        Lz11,
        Prs,
        Plugin,
    }
}