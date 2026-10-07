using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TextureLib;
using static ArchiveLib.GenericArchive;

namespace ArchiveLib
{
	/// <summary>Type of the PVM-like archive (PVM, GVM, XVM).</summary>
	public enum PuyoArchiveType
    {
        Unknown,
        PVMFile,
        GVMFile,
		XVMFile,
    }

	/// <summary>PVM archives used in Dreamcast games and their Gamecube (GVM) and Xbox (XVM) ports.</summary>
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

		/// <summary>Type of the archive (PVM, GVM or XVM).</summary>
		public PuyoArchiveType Type;

		/// <summary>Flags indicating the presence of various metadata (filenames, global indices etc.)</summary>
		public PuyoArchiveFlags Flags;

		/// <summary>True if the archive contains textures that require a palette file.</summary>
		public bool PaletteRequired;

		/// <summary>Flags indicating the presence of various metadata (filenames, global indices etc.)</summary>
		[FlagsAttribute]
        public enum PuyoArchiveFlags : ushort
        {
			/// <summary>PVMH_PS_PVRNAME. Indicates that the archive stores entry names.</summary>
			Filenames = 0x1,

			/// <summary>VMH_PS_CATEGORYCODE. Indicates that the archive stores texture pixel and data formats.</summary>
			PixelDataFormat = 0x2,

			/// <summary>VMH_PS_ENTRYINFO. Indicates that the archive stores texture dimensions.</summary>
			TextureDimensions = 0x4, //  (EntryStatus)

			/// <summary>VMH_PS_GLOBALINDEX. Indicates that the archive stores global indices.</summary>
			GlobalIndex = 0x8,

			/// <summary>PVMH_PS_CHUNK_MDLN. Indicates that the archive contains the "Model Name" metadata chunk.</summary>
			ModelName = 0x10,

			/// <summary>PVMH_PS_CHUNK_CONV.Indicates that the archive contains the "Converter Name" metadata chunk.</summary>
			ConverterName = 0x20,

			/// <summary>PVMH_PS_CHUNK_PVMI. Indicates that the archive stores the "PVM Information" metadata chunk.</summary>
			PvmInfo = 0x40,

			/// <summary>PVMH_PS_CHUNK_IMGC. Indicates that the archive stores the original images in addition to PVR textures.</summary>
			OriginalImage = 0x80,

			/// <summary>PVMH_PS_CHUNK_PVRT. Indicates that the archive stores PVRT headers.</summary>
			PVRT = 0x100,

			/// <summary>PVMH_PS_CHUNK_COMM. Indicates that the archive contains a "Comment" metadata chunk.</summary>
			Comment = 0x200, // 

			/// <summary>PVMH_PS_BANKID. Indicates that the archive stores a "Palette Bank ID" metadata chunk.</summary>
			PaletteBankID = 0x400,

			/// <summary>PVMH_PS_CHUNK_PVPL. Indicates that the archive stores a palette file.</summary>
			PaletteData = 0x800, // PVMH_PS_CHUNK_PVPL

			/// <summary>PVMH_PS_CHUNK_PVPN. Indicates that the archive stores the "Palette Name" metadata chunk.</summary>
			PaletteName = 0x1000
		}

		/// <summary>Checks if the specified byte array contains a GBIX header.</summary>
		/// <param name="data">Byte array to analyze.</param>
		/// <returns>True if a GBIX header is present.</returns>
		private bool HasGbix(byte[] data)
		{
			if (data.Length < 16)
				return false;
			for (int i = 0; i < data.Length - 4; i++)
			{
				if (BitConverter.ToUInt32(data,i) == Magic_GBIX)
					return true;
			}
			return false;
		}

		/// <summary>Checks the format of the archive type in the specified byte array.</summary>
		/// <param name="data">Byte array to analyze.</param>
		/// <returns>Archive type (PVM, GVM or XVM) or Unknown.</returns>
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
		/// TODO: Remove this function and use per-texture palettes instead.
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
				PaletteRequired = false;
			}
		}

		/// <summary>
		/// This function displays a dialog to select a PVP/GVP palette file and sets all textures in the archive to use the specified palette. 
		/// TODO: Remove this function and use per-texture palettes instead.
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
				{
					SetPalette(a.FileName);
					PaletteRequired = false;
				}
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
		/// <param name="pvmdata">Byte array to analyze.</param>
		/// <param name="offset">Offset to analyze.</param>
		/// <returns>Offset of the PVRT/GVRT/XVRT header.</returns>
        public static int GetPVRTOffset(byte[] pvmdata, int offset)
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

		public PuyoFile(byte[] pvmdata, int offs = 0)
        {
			if (offs != 0)
			{
				byte[] data = new byte[pvmdata.Length - offs];
				Array.Copy(pvmdata, offs, data, 0, data.Length);
				pvmdata = data;
			}
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

                // Set filename if the PVM/GVM has filenames. Those are stored without extension in PVM/GVM/XVM.
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
                } 
				else				
					HasNameData = false;

                if (t < numtextures - 1)  // Get the address of the next PVRT chunk, unless it's the last one
                    textureaddr = GetPVRTOffset(pvmdata, textureaddr + size + 8);

				// Add PVR/GVR/XVR texture to the entry list
				switch (Type)
				{
					case PuyoArchiveType.PVMFile:
						PvrTexture pvrt = new PvrTexture(pvrchunk);
						Entries.Add(new PVMEntry(pvrchunk, entryfn + ".pvr"));
						if (pvrt.RequiresPaletteFile)
							PaletteRequired = true;
						break;
					case PuyoArchiveType.GVMFile:
						GvrTexture gvrt = new GvrTexture(pvrchunk);
						Entries.Add(new GVMEntry(pvrchunk, entryfn + ".gvr"));
						if (gvrt.RequiresPaletteFile)
							PaletteRequired = true;
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
						// Names without extension
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
					bool hasGbix = HasGbix(Entries[i].Data);
					// Calculate texture length without the GBIX header, align by 32
					int length = Entries[i].Data.Length - (hasGbix ? 16 : 0);
					if (length % 32 != 0)
						do
							length++;
						while (length % 32 != 0);
					// This is a bit awkward and should be done in a better way...
					// It copies the texture's data without the GBIX header (but with the PVRT header and updated texture length).
					byte[] outData = new byte[length];
					Array.Copy(Entries[i].Data, hasGbix ? 16 : 0, outData, 0, Entries[i].Data.Length - (hasGbix ? 16 : 0));
					Array.Copy(BitConverter.GetBytes(length - 8), 0, outData, 4, 4); // Write updated length to PVRT
					result.AddRange(outData);
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

		/// <summary>Converts a PVM archive to a PB archive.</summary>
		/// <returns>PB archive.</returns>
		public PBFile GetPB()
		{
			PBFile pb = new PBFile(Entries.Count);			
			for (int i = 0; i< Entries.Count; i++)
			{
				pb.Entries.Add(new PBEntry(Entries[i].Data, pb.GetCurrentOffset(i, Entries.Count)));
			}
			return pb;
		}

		public override GenericArchiveEntry NewEntry()
		{
			return Type switch
			{
				PuyoArchiveType.PVMFile => new PVMEntry(),
				PuyoArchiveType.GVMFile => new GVMEntry(),
				PuyoArchiveType.XVMFile => new XVMEntry(),
				_ => throw new NotImplementedException(),
			};
		}
	}

	/// <summary>
	/// PVM archive entry.
	/// TODO: Make a parent class and make PVMEntry, GVMEntry and XVMEntry subclasses?
	/// </summary>
	public class PVMEntry : GenericArchiveEntry
    {
		/// <summary>The texture's Global Index.</summary>
        public uint GBIX;

		/// <summary>The texture's palette (if relevant).</summary>
		public TexturePalette Palette;

		/// <summary>Creates a new PVM entry from raw texture data with a GBIX/PVRT header.</summary>
		/// <param name="pvrdata">Byte array with PVR texture data.</param>
		/// <param name="name">Texture filename with extension.</param>
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

		public PVMEntry() {	}

		public override Bitmap GetBitmap()
        {
            PvrTexture pvrt = new PvrTexture(Data);
            if (pvrt.RequiresPaletteFile)
                pvrt.SetPalette(Palette);
            return pvrt.Image;
        }
    }

	/// <summary>
	/// GVM archive entry.
	/// TODO: Merge with PVMEntry?
	/// </summary>
	public class GVMEntry : GenericArchiveEntry
    {
		/// <summary>The texture's Global Index.</summary>
		public uint GBIX;

		/// <summary>The texture's palette (if relevant).</summary>
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

		public GVMEntry() {	}

		public override Bitmap GetBitmap()
        {
            GvrTexture gvrt = new GvrTexture(Data);
            if (gvrt.RequiresPaletteFile)
                gvrt.SetPalette(Palette);
            return gvrt.Image;
        }
    }

	/// <summary>
	/// XVM archive entry.
	/// TODO: Merge with PVMEntry?
	/// </summary>
	public class XVMEntry : GenericArchiveEntry
	{
		/// <summary>The texture's Global Index.</summary>
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

		public XVMEntry() {	}

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