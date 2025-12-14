using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TextureLib;
using static ArchiveLib.GenericArchive;

// PVM/GVM archives used in Dreamcast/Gamecube games and their ports.
namespace ArchiveLib
{
    public enum PuyoArchiveType
    {
        Unknown,
        PVMFile,
        GVMFile,
		XVMFile,
    }

    public class PuyoFile : GenericArchive
    {
        // Archive chunks
        const uint Magic_PVM = 0x484D5650; // PVMH archive
        const uint Magic_GVM = 0x484D5647; // GVMH archive
		const uint Magic_XVM = 0x484D5658; // XVMH archive

		// PVM metadata chunks
		const uint Magic_MDLN = 0x4E4C444D; // Model Name
        const uint Magic_COMM = 0x4D4D4F43; // Comment
        const uint Magic_CONV = 0x564E4F43; // PVM Converter
        const uint Magic_IMGC = 0x43474D49; // Image Container
        const uint Magic_PVMI = 0x494D5650; // PVM File Info
		
		// Texture chunks
		const uint Magic_GBIX = 0x58494247; // PVR texture header (GBIX)
        const uint Magic_PVRT = 0x54525650; // PVR texture header (texture data)
        const uint Magic_GVRT = 0x54525647; // GVR texture header (texture data)
        const uint Magic_PVRI = 0x49525650; // PVR texture header (metadata)
		const uint Magic_XVRT = 0x54525658; // XVR texture header (texture data)

		// Palette chunks
		const uint Magic_PVPL = 0x4C505650; // Palette data chunk
		const uint Magic_PVPN = 0x4E505650; // Palette name chunk
		
        public PuyoArchiveType Type;
		public PuyoArchiveFlags Flags;

		[FlagsAttribute]
        public enum PuyoArchiveFlags : ushort
        {
			Filenames = 0x1, // PVMH_PS_PVRNAME
			PixelDataFormat = 0x2, // VMH_PS_CATEGORYCODE
			TextureDimensions = 0x4, // VMH_PS_ENTRYINFO (EntryStatus)
			GlobalIndex = 0x8, // VMH_PS_GLOBALINDEX
			ModelName = 0x10, // PVMH_PS_CHUNK_MDLN 
			ConverterName = 0x20, // PVMH_PS_CHUNK_CONV
			PvmInfo = 0x40, // PVMH_PS_CHUNK_PVMI
			OriginalImage = 0x80, // PVMH_PS_CHUNK_IMGC
			PVRT = 0x100, // PVMH_PS_CHUNK_PVRT
			Comment = 0x200, // PVMH_PS_CHUNK_COMM
			PaletteBankID = 0x400, // PVMH_PS_BANKID 
			PaletteData = 0x800, // PVMH_PS_CHUNK_PVPL
			PaletteName = 0x1000 // PVMH_PS_CHUNK_PVPN
        }

        public override void CreateIndexFile(string path)
        {
            using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
            {
                foreach (GenericArchiveEntry pvmentry in Entries)
                {
                    texList.WriteLine(pvmentry.Name);
                }
                texList.Flush();
                texList.Close();
            }
        }

        public static PuyoArchiveType Identify(byte[] data)
        {
			if (data.Length < 4)
				return PuyoArchiveType.Unknown;
			uint magic = BitConverter.ToUInt32(data, 0);
            switch (magic)
            {
                case Magic_PVM:
                    return PuyoArchiveType.PVMFile;
                case Magic_GVM:
                    return PuyoArchiveType.GVMFile;
				case Magic_XVM:
					return PuyoArchiveType.XVMFile;
				default:
                    return PuyoArchiveType.Unknown;
            }
        }

		/// <summary>
		/// This function sets all textures in the archive to use the specified PVP/GVP palette file.
		/// If the specified palette file exists, the PaletteRequired flag is removed.
		/// </summary>
		public void SetPalette(string palettePath) 
		{
			if (File.Exists(palettePath))
			{
				TexturePalette Palette = null;
				bool gvm = Type == PuyoArchiveType.GVMFile;
				Palette = new TexturePalette(File.ReadAllBytes(palettePath));
				foreach (GenericArchiveEntry entry in Entries)
				{
					if (entry is PVMEntry pvme)
					{
						PvrTexture pvrt = new PvrTexture(pvme.Data);
						if (pvrt.RequiresPaletteFile)
							pvme.Palette = Palette;
					}
					else if (entry is GVMEntry gvme)
					{
						GvrTexture gvrt = new GvrTexture(gvme.Data);
						if (gvrt.RequiresPaletteFile)
							gvme.Palette = Palette;
					}
				}
			}
		}

		/// <summary>
		/// This function displays a dialog to select a PVP/GVP palette file and sets all textures in the archive to use the specified palette. 
		/// </summary>
		public void AddPaletteFromDialog(string startPath)
		{
			bool gvm = Type == PuyoArchiveType.GVMFile;
			using (System.Windows.Forms.OpenFileDialog a = new System.Windows.Forms.OpenFileDialog
			{
				DefaultExt = gvm ? "gvp" : "pvp",
				Filter = gvm ? "GVP Files|*.gvp" : "PVP Files|*.pvp",
				InitialDirectory = startPath,
				Title = "External palette file"
			})
				if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					SetPalette(a.FileName);
		}

        public PuyoFile(PuyoArchiveType type = PuyoArchiveType.PVMFile) 
        {
            Type = type;
			Flags |= PuyoArchiveFlags.Filenames;
			Flags |= PuyoArchiveFlags.GlobalIndex;
			Flags |= PuyoArchiveFlags.PVRT;
			// The flags above are used commonly in SA1DC.
			// The flags below are often used in general.
			Flags |= PuyoArchiveFlags.TextureDimensions;
			Flags |= PuyoArchiveFlags.PixelDataFormat;

		}

        /// <summary>
        /// This function checks the specified offset in a byte array to verify if it begins with a PVR/GVR/XVR texture header. 
		/// Sometimes there is padding or metadata that needs to be skipped in order to read the texture.
		/// The function loops through the array until it finds a PVRT/GVRT/XVRT header and returns its offset.
        /// </summary>
        public int GetPVRTOffset(byte[] pvmdata, int offset)
        {
            uint header = BitConverter.ToUInt32(pvmdata, offset);
            int currentoffset = offset;
            switch (header)
            {
                // If a valid texture header is found, that's it
                case Magic_PVRT:
                case Magic_GVRT:
                case Magic_XVRT:
                    return currentoffset;

                // If a metadata chunk is recognized, skip it
                case Magic_MDLN:
                case Magic_CONV:
                case Magic_IMGC:
                case Magic_COMM:
                case Magic_PVMI:
                case Magic_PVPN: // Will do later
                case Magic_PVPL: // Will do later
				case Magic_PVRI: // This one probably shouldn't be here but there are no known examples yet of how this was used
                    int size = BitConverter.ToInt32(pvmdata, offset + 4);
                    currentoffset += size + 8;
                    // Go through the metadata until it gets to a PVRT/GVRT header
                    return GetPVRTOffset(pvmdata, currentoffset);

                // If the header is unrecognized, add 1 byte to the offset and continue looking
                default:
                    currentoffset += 1;
                    // Go through the padding until it gets to a PVRT/GVRT header
                    return GetPVRTOffset(pvmdata, currentoffset);
            }
        }

        public PuyoFile(byte[] pvmdata)
        {
            ByteConverter.BackupBigEndian();
            Entries = new List<GenericArchiveEntry>();
            Type = Identify(pvmdata);
            switch (Type)
            {
                case PuyoArchiveType.PVMFile:
                    ByteConverter.BigEndian = false;
                    break;
                case PuyoArchiveType.GVMFile:
                    ByteConverter.BigEndian = true;
                    break;
				case PuyoArchiveType.XVMFile:
					ByteConverter.BigEndian = false;
					break;
				default:
                    throw new Exception("Error: Unknown archive format");
            }

            // Get PVM/GVM flags and calculate item size in the entry table
            ushort numtextures = ByteConverter.ToUInt16(pvmdata, Type == PuyoArchiveType.XVMFile ? 0x08 : 0x0A);
            int pvmentrysize = 2;
            int gbixoffset = 0;
            int nameoffset = 0;
			if (Type != PuyoArchiveType.XVMFile)
			{
				Flags = (PuyoArchiveFlags)ByteConverter.ToUInt16(pvmdata, 0x08);
				if (Flags.HasFlag(PuyoArchiveFlags.Filenames))
					nameoffset = pvmentrysize;
				pvmentrysize += 28;
				if (Flags.HasFlag(PuyoArchiveFlags.PixelDataFormat))
					pvmentrysize += 2;
				if (Flags.HasFlag(PuyoArchiveFlags.TextureDimensions))
					pvmentrysize += 2;
				if (Flags.HasFlag(PuyoArchiveFlags.GlobalIndex))
					gbixoffset = pvmentrysize;
				pvmentrysize += 4;
			}			

			int offsetfirst = BitConverter.ToInt32(pvmdata, 0x4) + 8; // Always Little Endian
            int textureaddr = GetPVRTOffset(pvmdata, offsetfirst); // Where texture data begins

            for (int t = 0; t < numtextures; t++)
            {
                int size_gbix = Flags.HasFlag(PuyoArchiveFlags.GlobalIndex) ? 16 : 0;
                int size = BitConverter.ToInt32(pvmdata, textureaddr + 4); // Always Little Endian
                byte[] pvrchunk = new byte[size + 8 + size_gbix];

                // Handle cases when data size in the PVR/GVR header goes beyond the range of the archive (Billy Hatcher)
                if ((textureaddr + size + 8) > pvmdata.Length)
                    do
                        size--;
                    while ((textureaddr + size + 8) > pvmdata.Length);

                Array.Copy(pvmdata, textureaddr, pvrchunk, 0 + size_gbix, size + 8);
                // Add GBIX header if the PVM/GVM has GBIX enabled
                if (Flags.HasFlag(PuyoArchiveFlags.GlobalIndex))
                {
                    Array.Copy(BitConverter.GetBytes(Magic_GBIX), 0, pvrchunk, 0, 4); // Little Endian
                    pvrchunk[4] = 0x08; // Always 8 according to PuyoTools
                    uint gbix = BitConverter.ToUInt32(pvmdata, 0xC + pvmentrysize * t + gbixoffset);
                    byte[] gbixb = BitConverter.GetBytes(gbix);
                    Array.Copy(gbixb, 0, pvrchunk, 8, 4);
                }

                // Set filename if the PVM/GVM has filenames
                string entryfn = t.ToString("D3");
                if (Flags.HasFlag(PuyoArchiveFlags.Filenames))
                {
                    byte[] namestring = new byte[28];
                    for (int n = 0; n < 28; n++)
                    {
                        // Entry names in some PVMs (e.g. DEMO.PVM in Dream Passport 3) have garbage data after the first 0, so it needs to be truncated
                        byte ndt = pvmdata[0xC + pvmentrysize * t + nameoffset + n];
                        if (ndt == 0)
                            break;
                        namestring[n] = ndt;
                    }
                    entryfn = Encoding.ASCII.GetString(namestring).TrimEnd((char)0);
                } else
				{
					hasNameData = false;
				}

                if (t < numtextures - 1)  // Get the address of the next PVRT chunk, unless it's the last one
                    textureaddr = GetPVRTOffset(pvmdata, textureaddr + size + 8);

				// Add PVR/GVR/XVR texture to the entry list
				switch (Type)
				{
					case PuyoArchiveType.PVMFile:
						PvrTexture pvrt = new PvrTexture(pvrchunk);
						Entries.Add(new PVMEntry(pvrchunk, entryfn + ".pvr"));
						break;
					case PuyoArchiveType.GVMFile:
						GvrTexture gvrt = new GvrTexture(pvrchunk);
						Entries.Add(new GVMEntry(pvrchunk, entryfn + ".gvr"));
						break;
					case PuyoArchiveType.XVMFile:
						XvrTexture xvrt = new XvrTexture(pvrchunk);
						Entries.Add(new XVMEntry(pvrchunk, entryfn + ".xvr"));
						break;
				}
            }
            ByteConverter.RestoreBigEndian();
        }

		public override byte[] GetBytes()
		{
			bool bigendianbk = ByteConverter.BigEndian;
			ByteConverter.BigEndian = Type == PuyoArchiveType.GVMFile;
			List<byte> result = new List<byte>();
			bool hasEntryTable = true;

			uint magic = 0;
			switch (Type)
			{
				case PuyoArchiveType.PVMFile:
					magic = Magic_PVM;
					hasEntryTable = true;
					break;
				case PuyoArchiveType.GVMFile:
					magic = Magic_GVM;
					hasEntryTable = true;
					break;
				case PuyoArchiveType.XVMFile:
					magic = Magic_XVM;
					hasEntryTable = false;
					break;
			}
			result.AddRange(BitConverter.GetBytes(magic));

			// Create entry list
			if (hasEntryTable)
			{
				List<byte> entrytable = new List<byte>();
				uint firstoffset = 0xC;
				for (int i = 0; i < Entries.Count; i++)
				{
					entrytable.AddRange(ByteConverter.GetBytes((ushort)i));
					if (Flags.HasFlag(PuyoArchiveFlags.Filenames))
					{
						byte[] namestring = System.Text.Encoding.ASCII.GetBytes(Path.GetFileNameWithoutExtension(Entries[i].Name));
						byte[] namefull = new byte[28];
						Array.Copy(namestring, namefull, namestring.Length);
						entrytable.AddRange(namefull);
					}
					ushort dimensions = 0;
					uint gbix = 0;
					if (Entries[i] is PVMEntry pvme)
					{
						PvrTexture pvrt = new PvrTexture(pvme.Data);
						if (Flags.HasFlag(PuyoArchiveFlags.PixelDataFormat))
						{
							entrytable.Add((byte)pvrt.PvrPixelFormat);
							entrytable.Add((byte)pvrt.PvrDataFormat);
						}
						dimensions |= (ushort)(((byte)Math.Log(pvrt.Width, 2) - 2) & 0xF);
						dimensions |= (ushort)((((byte)Math.Log(pvrt.Height, 2) - 2) & 0xF) << 4);
						gbix = pvrt.Gbix;
					}
					else if (Entries[i] is GVMEntry gvme)
					{
						GvrTexture gvrt = new GvrTexture(gvme.Data);
						if (Flags.HasFlag(PuyoArchiveFlags.PixelDataFormat))
						{
							entrytable.Add((byte)gvrt.GvrPaletteFormat);
							entrytable.Add((byte)gvrt.GvrDataFormat);
						}
						dimensions |= (ushort)(((byte)Math.Log(gvrt.Width, 2) - 2) & 0xF);
						dimensions |= (ushort)((((byte)Math.Log(gvrt.Height, 2) - 2) & 0xF) << 4);
						gbix = gvrt.Gbix;
					}
					if (Flags.HasFlag(PuyoArchiveFlags.TextureDimensions))
						entrytable.AddRange(ByteConverter.GetBytes(dimensions));
					if (Flags.HasFlag(PuyoArchiveFlags.GlobalIndex))
						entrytable.AddRange(ByteConverter.GetBytes(gbix));
				}
				// Add padding if the data isn't aligned by 32
				if ((12 + entrytable.Count) % 32 != 0)
				{
					do
					{
						entrytable.Add(0);
					}
					while ((12 + entrytable.Count) % 32 != 0);
				}
				// Write other header stuff
				result.AddRange(BitConverter.GetBytes((uint)(firstoffset + entrytable.Count - 8))); // Offset of the first texture, Little Endian
				result.AddRange(ByteConverter.GetBytes((ushort)Flags)); // PVM/GVM flags
				result.AddRange(ByteConverter.GetBytes((ushort)Entries.Count));
				result.AddRange(entrytable);
			}
			// Or write empty bytes like in XVM
			else
			{
				result.AddRange(BitConverter.GetBytes(0x38));
				result.AddRange(BitConverter.GetBytes(Entries.Count));
				result.AddRange(new byte[0x34]);
			}
			// Write texture data
			for (int i = 0; i < Entries.Count; i++)
			{
				if (hasEntryTable)
				{
					// Align by 32
					int length = Entries[i].Data.Length - 16;
					if (length % 32 != 0)
						do
							length++;
						while (length % 32 != 0);
					byte[] nogbix = new byte[length];
					Array.Copy(Entries[i].Data, 16, nogbix, 0, Entries[i].Data.Length - 16);
					Array.Copy(BitConverter.GetBytes(length - 8), 0, nogbix, 4, 4); // Write updated length to PVRT
					result.AddRange(nogbix);
				}
				else
				{
					result.AddRange(Entries[i].Data);
					result.AddRange(new byte[Entries[i].Data.Length % 32]);
				}
			}
			ByteConverter.BigEndian = bigendianbk;
			return result.ToArray();
		}

		public PBFile GetPB()
		{
			PBFile pb = new PBFile(Entries.Count);			
			for (int i = 0; i< Entries.Count; i++)
			{
				pb.Entries.Add(new PBEntry(Entries[i].Data, pb.GetCurrentOffset(i, Entries.Count)));
			}
			return pb;
		}
}

    public class PVMEntry : GenericArchiveEntry
    {
        public uint GBIX;
        public TexturePalette Palette;

        public PVMEntry(byte[] pvrdata, string name)
        {
            Name = name;
            Data = pvrdata;
            PvrTexture pvrt = new PvrTexture(pvrdata);
            GBIX = pvrt.Gbix;
        }

        public PVMEntry(string filename)
        {
            Name = Path.GetFileName(filename);
            Data = File.ReadAllBytes(filename);
            PvrTexture pvrt = new PvrTexture(Data);
            GBIX = pvrt.Gbix;
        }

        public uint GetGBIX()
        {
            return GBIX;
        }

        public override Bitmap GetBitmap()
        {
            PvrTexture pvrt = new PvrTexture(Data);
            if (pvrt.RequiresPaletteFile)
                pvrt.SetPalette(Palette);
            return pvrt.Image;
        }
    }

    public class GVMEntry : GenericArchiveEntry
    {
        public uint GBIX;
        public TexturePalette Palette;

        public GVMEntry(byte[] gvrdata, string name)
        {
            Name = name;
            Data = gvrdata;
            GvrTexture gvrt = new GvrTexture(gvrdata);
            GBIX = gvrt.Gbix;
        }

        public GVMEntry(string filename)
        {
            Name = Path.GetFileName(filename);
            Data = File.ReadAllBytes(filename);
            GvrTexture gvrt = new GvrTexture(Data);
            GBIX = gvrt.Gbix;
        }

        public uint GetGBIX()
        {
            return GBIX;
        }

        public override Bitmap GetBitmap()
        {
            GvrTexture gvrt = new GvrTexture(Data);
            if (gvrt.RequiresPaletteFile)
                gvrt.SetPalette(Palette);
            return gvrt.Image;
        }
    }

	public class XVMEntry : GenericArchiveEntry
	{
		public uint GBIX;

		public XVMEntry(byte[] xvrdata, string name)
		{
			Name = name;
			Data = xvrdata;
			XvrTexture xvrt = new XvrTexture(xvrdata);
			GBIX = xvrt.Gbix;
		}

		public XVMEntry(string filename)
		{
			Name = Path.GetFileName(filename);
			Data = File.ReadAllBytes(filename);
			XvrTexture xvrt = new XvrTexture(Data);
			GBIX = xvrt.Gbix;
		}

		public uint GetGBIX()
		{
			return GBIX;
		}

		public override Bitmap GetBitmap()
		{
			XvrTexture xvrt = new XvrTexture(Data);
			return xvrt.Image;
		}
	}
}