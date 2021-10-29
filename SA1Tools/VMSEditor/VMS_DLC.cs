using System;
using System.Collections.Generic;
using System.Linq;
using SplitTools;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.ColorReduction;
using SAModel;
using System.IO;

namespace VMSEditor
{
    public class SA1DLC
    {
        public class VMS_DLC : VMSFile
        {
            // Stuff that goes in the header
            [IniAlwaysInclude]
            public string Title;
            [IniAlwaysInclude]
            public string Description;
            [IniAlwaysInclude]
            [IniName("Author")]
            public string AppName;
            [IniAlwaysInclude]
            public uint Identifier;
            [IniAlwaysInclude]
            public bool ContainsSound;
            [IniAlwaysInclude]
            public bool EnableSonic;
            [IniAlwaysInclude]
            public bool EnableTails;
            [IniAlwaysInclude]
            public bool EnableKnuckles;
            [IniAlwaysInclude]
            public bool EnableGamma;
            [IniAlwaysInclude]
            public bool EnableAmy;
            [IniAlwaysInclude]
            public bool EnableBig;
            public bool EnableWhatever1;
            public bool EnableWhatever2;
            [IniAlwaysInclude]
            public DLCRegionLocks Region;
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            [IniName("Item ")]
            public List<DLCObjectData> Items;
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            [IniName("JapaneseMessage")]
            public string[] JapaneseStrings;
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            [IniName("EnglishMessage")]
            public string[] EnglishStrings;
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            [IniName("FrenchMessage")]
            public string[] FrenchStrings;
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            [IniName("SpanishMessage")]
            public string[] SpanishStrings;
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            [IniName("GermanMessage")]
            public string[] GermanStrings;

            // DLC icon
            [IniIgnore]
            public Bitmap Icon;

            // Stuff that goes after the header
            [IniIgnore]
            public byte[] TextureData;
            [IniIgnore]
            public byte[] SoundData;
            [IniIgnore]
            public byte[] ModelData;

            public VMS_DLC()
            {
                Title = "";
                AppName = "";
                Description = "";
                Icon = new Bitmap(32, 32);
                Items = new List<DLCObjectData>();
                JapaneseStrings = new string[16];
                EnglishStrings = new string[16];
                FrenchStrings = new string[16];
                GermanStrings = new string[16];
                SpanishStrings = new string[16];
                TextureData = new byte[0];
                SoundData = new byte[0];
                ModelData = new byte[0];
                for (int i = 0; i < 16; i++)
                {
                    JapaneseStrings[i] = "";
                    EnglishStrings[i] = "";
                    FrenchStrings[i] = "";
                    GermanStrings[i] = "";
                    SpanishStrings[i] = "";
                }
            }

            /// <summary>
            /// Load from a byte array (VMS file)
            /// </summary>
            public VMS_DLC(byte[] file)
            {
                // Decrypt if encrypted
                if (BitConverter.ToUInt32(file, 0x280) != 0x2C0)
                {
                    int aniFrames = file[0x40] + (file[0x41] << 8);
                    int dataStart = 0x80 + (aniFrames * 0x200);
                    byte[] encrypted = new byte[file.Length - dataStart];
                    Array.Copy(file, dataStart, encrypted, 0, encrypted.Length);
                    VMSFile.DecryptData(ref encrypted);
                    Array.Copy(encrypted, 0, file, dataStart, encrypted.Length);
                }
                // Get title and other stuff
                byte[] title_b = GetTextItem(ref file, 0, 16);
                byte[] description_b = GetTextItem(ref file, 0x10, 32);
                byte[] appname_b = GetTextItem(ref file, 0x30, 16);
                Description = System.Text.Encoding.GetEncoding(932).GetString(description_b);
                Title = System.Text.Encoding.GetEncoding(932).GetString(title_b);
                AppName = System.Text.Encoding.GetEncoding(932).GetString(appname_b);
                Icon = GetIconFromFile(file);
                // Get items
                int item_pointer = BitConverter.ToInt32(file, 0x280);
                if (item_pointer != 0x2C0)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to find SA1 DLC data.", "DLC Tool Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Items = new List<DLCObjectData>();
                    JapaneseStrings = new string[16];
                    EnglishStrings = new string[16];
                    FrenchStrings = new string[16];
                    GermanStrings = new string[16];
                    SpanishStrings = new string[16];
                    TextureData = new byte[0];
                    SoundData = new byte[0];
                    ModelData = new byte[0];
                    return;
                }
                Identifier = BitConverter.ToUInt32(file, item_pointer);
                if ((file[item_pointer + 4] & 0xF) > 0) EnableSonic = true;
                if ((file[item_pointer + 4] >> 4) > 0) EnableTails = true;
                if ((file[item_pointer + 5] & 0xF) > 0) EnableKnuckles = true;
                if ((file[item_pointer + 5] >> 4) > 0) EnableGamma = true;
                if ((file[item_pointer + 6] & 0xF) > 0) EnableAmy = true;
                if ((file[item_pointer + 6] >> 4) > 0) EnableBig = true;
                if ((file[item_pointer + 7] & 0xF) > 0) EnableWhatever1 = true;
                if ((file[item_pointer + 7] >> 4) > 0) EnableWhatever2 = true;
                Region = (DLCRegionLocks)BitConverter.ToInt32(file, item_pointer + 8);
                // Item table
                int item_count = BitConverter.ToInt32(file, 0x284);
                Items = new List<DLCObjectData>(item_count);
                item_pointer += 12; // Skip 12-byte item section header
                for (int u = 0; u < item_count; u++)
                {
                    DLCObjectData dlcitem = new DLCObjectData();
                    dlcitem.Level = file[item_pointer + u * 30];
                    dlcitem.Act = file[item_pointer + u * 30 + 1];
                    dlcitem.ScaleX = file[item_pointer + u * 30 + 2];
                    dlcitem.ScaleY = file[item_pointer + u * 30 + 3];
                    dlcitem.ScaleZ = file[item_pointer + u * 30 + 4];
                    dlcitem.RotSpeedX = (sbyte)file[item_pointer + u * 30 + 5];
                    dlcitem.RotSpeedY = (sbyte)file[item_pointer + u * 30 + 6];
                    dlcitem.RotSpeedX = (sbyte)file[item_pointer + u * 30 + 7];
                    dlcitem.ObjectType = (DLCObjectTypes)file[item_pointer + u * 30 + 8];
                    dlcitem.Texture = file[item_pointer + u * 30 + 9];
                    dlcitem.Flags = (DLCObjectFlags)(BitConverter.ToUInt16(file, item_pointer + u * 30 + 10));
                    dlcitem.InternalID = file[item_pointer + u * 30 + 12];
                    dlcitem.Unknown3 = file[item_pointer + u * 30 + 13];
                    dlcitem.Message = file[item_pointer + u * 30 + 14];
                    dlcitem.TriggerDistance = file[item_pointer + u * 30 + 15];
                    dlcitem.WarpLevelOrSoundbank = file[item_pointer + u * 30 + 16];
                    dlcitem.WarpActOrSoundID = file[item_pointer + u * 30 + 17];
                    dlcitem.RotationX = BitConverter.ToUInt16(file, item_pointer + u * 30 + 18);
                    dlcitem.RotationY = BitConverter.ToUInt16(file, item_pointer + u * 30 + 20);
                    dlcitem.RotationZ = BitConverter.ToUInt16(file, item_pointer + u * 30 + 22);
                    dlcitem.X = BitConverter.ToInt16(file, item_pointer + u * 30 + 24);
                    dlcitem.Y = BitConverter.ToInt16(file, item_pointer + u * 30 + 26);
                    dlcitem.Z = BitConverter.ToInt16(file, item_pointer + u * 30 + 28);
                    Items.Add(dlcitem);
                }
                // Get strings
                JapaneseStrings = new string[16];
                EnglishStrings = new string[16];
                FrenchStrings = new string[16];
                SpanishStrings = new string[16];
                GermanStrings = new string[16];
                int text_pointer = BitConverter.ToInt32(file, 0x288);
                int text_count = BitConverter.ToInt32(file, 0x28C);
                List<string> strings = new List<string>();
                for (int u = 0; u < text_count; u++)
                {
                    byte[] arr = new byte[64];
                    Array.Copy(file, text_pointer + 64 * u, arr, 0, 64);
                    int charcount = 0;
                    for (int a = 0; a < arr.Length; a++)
                    {
                        if (arr[a] == 0) break;
                        charcount++;
                    }
                    System.Text.Encoding enc = System.Text.Encoding.GetEncoding(932);
                    if (u >= 32) enc = System.Text.Encoding.GetEncoding(1252);
                    string str = enc.GetString(arr, 0, charcount);
                    strings.Add(str);
                }
                // Process special characters
                List<string> strings_new = new List<string>();
                foreach (string str in strings)
                {
                    string newstr = System.String.Empty;
                    for (int s = 0; s < str.Length; s++)
                    {
                        if (s == 0 && (str[s] == '~' || str[s] == 't'))
                        {
                            newstr += ("\t");
                        }
                        else if (str[s] == 't' && str[0] == 't') newstr += ("\t");
                        else if (str[s] == '~' && str[0] == '~') newstr += ("\t");
                        else if ((str[s] == 'n' && str[0] == 't') || (str[s] == '@' && str[0] == '~'))
                            newstr += ("\n");
                        else newstr += str[s];
                    }
                    strings_new.Add(newstr);
                }
                string[] stringarr = strings_new.ToArray();
                for (int u = 0; u < 16; u++)
                {
                    if (u >= text_count) break;
                    JapaneseStrings[u] = stringarr[u];
                    if (text_count <= 16) continue;
                    EnglishStrings[u] = stringarr[u + 16];
                    if (text_count <= 32) continue;
                    FrenchStrings[u] = stringarr[u + 32];
                    if (text_count <= 48) continue;
                    SpanishStrings[u] = stringarr[u + 48];
                    if (text_count <= 64) continue;
                    GermanStrings[u] = stringarr[u + 64];
                }
                TextureData = new byte[0];
                SoundData = new byte[0];
                ModelData = new byte[0];
                // Get PVM pointer
                uint pvm_pointer = BitConverter.ToUInt32(file, 0x290); // Number of PVMs at 0x294, number of textures at 0x298
                // Get MLT pointer
                uint mlt_pointer = BitConverter.ToUInt32(file, 0x29C);
                int nummlt = BitConverter.ToInt32(file, 0x2A0);
                if (nummlt != 0)
                    ContainsSound = true;
                // Get PRS pointer
                uint prs_pointer = BitConverter.ToUInt32(file, 0x2A4); // Number of PRSes at 0x2A8
                // Get PVM data
                int pvm_size = (int)mlt_pointer - (int)pvm_pointer;
                if (pvm_size > 0)
                {
                    TextureData = new byte[pvm_size];
                    Array.Copy(file, pvm_pointer, TextureData, 0, pvm_size);
                }
                // Get MLT data
                int mlt_size = (int)prs_pointer - (int)mlt_pointer;
                if (mlt_size > 0)
                {
                    SoundData = new byte[mlt_size];
                    Array.Copy(file, mlt_pointer, SoundData, 0, mlt_size);
                }
                int item_size = (item_count * 30 + 12); // 12-byte header, headerless section size at 0x48
                do
                    item_size++;
                while (item_size % 16 != 0);
                // The size of the PRS cannot be determined reliably because the file might be padded to have a size divisible by 512
                // To get the correct size, the PRS chunk is read until the end of file or a sequence of 16 empty bytes
                int prs_size = 0;
                for (int indx = (int)prs_pointer; indx < file.Length; indx += 16)
                {
                    bool valid = false;
                    for (int u = 0; u < 16; u++)
                    {
                        if (file[indx + u] != 0)
                        {
                            valid = true;
                            break;
                        }
                    }
                    if (valid)
                        prs_size += 16;
                    else
                        break;
                }
                // Get PRS data
                if (prs_size > 0)
                {
                    ModelData = new byte[prs_size];
                    Array.Copy(file, prs_pointer, ModelData, 0, prs_size);
                }
            }

            /// <summary>
            /// Load from a file (VMS or INI)
            /// </summary>
            public static VMS_DLC LoadFile(string filename)
            {
                switch (Path.GetExtension(filename.ToLowerInvariant()))
                {
                    case ".vms":
                        return new VMS_DLC(File.ReadAllBytes(filename));
                    case ".ini":
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show("Unknown file format. Use VMS or INI.");
                        return new VMS_DLC();
                }
                VMS_DLC header = IniSerializer.Deserialize<VMS_DLC>(filename);
                header.Icon = new Bitmap(Path.ChangeExtension(filename, ".bmp"));
                header.SoundData = new byte[0];
                header.TextureData = File.ReadAllBytes(Path.ChangeExtension(filename, ".pvm"));
                if (File.Exists(Path.ChangeExtension(filename, ".bin")))
                {
                    header.ModelData = FraGag.Compression.Prs.Compress(File.ReadAllBytes(Path.ChangeExtension(filename, ".bin")));
                }
                else header.ModelData = ConvertModel(Path.ChangeExtension(filename, ".sa1mdl"));
                if (header.ContainsSound) header.SoundData = File.ReadAllBytes(Path.ChangeExtension(filename, ".mlt"));
                return header;
            }

            /// <summary>
            /// Export an a folder with metadata + files for SADX DLC mod
            /// </summary>
			/// <param name="dir">Export path. Must be a folder.</param>
			/// <param name="writeall">Also write raw binary sections as separate files.</param>
            public void Export(string dir, bool writeall = false)
            {
                byte[] file = GetBytes();
                Directory.CreateDirectory(dir);
                string fname = Path.GetFileName(dir);
                IniSerializer.Serialize(new VMS_DLC(file), Path.Combine(dir, fname + ".ini"));
                Bitmap bmp = GetIconFromFile(file);
                bmp.Save(Path.Combine(dir, fname + ".bmp"), ImageFormat.Bmp);
                uint pvm_pointer = BitConverter.ToUInt32(file, 0x290);
                uint mlt_pointer = BitConverter.ToUInt32(file, 0x29C);
                uint prs_pointer = BitConverter.ToUInt32(file, 0x2A4);
                // Save sections
                int pvm_size = (int)mlt_pointer - (int)pvm_pointer;
                if (pvm_size > 0)
                {
                    byte[] pvmdata = new byte[pvm_size];
                    Array.Copy(file, pvm_pointer, pvmdata, 0, pvm_size);
                    File.WriteAllBytes(Path.Combine(dir, fname + ".pvm"), pvmdata);
                }
                int mlt_size = (int)prs_pointer - (int)mlt_pointer;
                if (mlt_size > 0)
                {
                    byte[] mltdata = new byte[mlt_size];
                    Array.Copy(file, mlt_pointer, mltdata, 0, mlt_size);
                    File.WriteAllBytes(Path.Combine(dir, fname + ".mlt"), mltdata);
                }
                uint sectionsize = BitConverter.ToUInt32(file, 0x48);
                int text_count = BitConverter.ToInt32(file, 0x28C);
                int item_count = BitConverter.ToInt32(file, 0x284);
                int item_size = (item_count * 30 + 12); //12-byte header
                do
                {
                    item_size++;
                }
                while (item_size % 16 != 0);
                int prs_size = file.Length - (int)prs_pointer;
                if (prs_size > 0)
                {
                    byte[] prsdata = new byte[prs_size];
                    Array.Copy(file, prs_pointer, prsdata, 0, prs_size);
                    if (writeall) File.WriteAllBytes(Path.Combine(dir, fname + ".prs"), prsdata);
                    prsdata = FraGag.Compression.Prs.Decompress(prsdata);
                    if (writeall) File.WriteAllBytes(Path.Combine(dir, fname + ".bin"), prsdata);
                    // Model pointer
                    uint modelpointer = BitConverter.ToUInt32(prsdata, 0) - 0xCCA4000;
                    //Console.WriteLine("Model pointer: {0}", modelpointer.ToString("X"));
                    NJS_OBJECT mdl = new NJS_OBJECT(prsdata, (int)modelpointer, 0xCCA4000, ModelFormat.Basic, null);
                    ModelFile.CreateFile((Path.Combine(dir, fname + ".sa1mdl")), mdl, null, null, null, null, ModelFormat.Basic);
                }
                //Console.WriteLine("Output folder: {0}", dir);
            }

            /// <summary>
            /// Make a clone
            /// </summary>
            public VMS_DLC(VMS_DLC original)
            {
                Title = original.Title;
                Description = original.Description;
                AppName = original.AppName;
                Icon = original.Icon;
                Identifier = original.Identifier;
                EnableSonic = original.EnableSonic;
                EnableTails = original.EnableTails;
                EnableKnuckles = original.EnableKnuckles;
                EnableGamma = original.EnableGamma;
                EnableBig = original.EnableBig;
                EnableAmy = original.EnableAmy;
                EnableWhatever1 = original.EnableWhatever1;
                EnableWhatever2 = original.EnableWhatever2;
                Region = original.Region;
                JapaneseStrings = new string[16];
                EnglishStrings = new string[16];
                FrenchStrings = new string[16];
                GermanStrings = new string[16];
                SpanishStrings = new string[16];
                for (int i = 0; i < 16; i++)
                {
                    JapaneseStrings[i] = original.JapaneseStrings[i];
                    EnglishStrings[i] = original.EnglishStrings[i];
                    FrenchStrings[i] = original.FrenchStrings[i];
                    GermanStrings[i] = original.GermanStrings[i];
                    SpanishStrings[i] = original.SpanishStrings[i];
                }
                Items = new List<DLCObjectData>();
                if (original.Items != null && original.Items.Count > 0)
                foreach (DLCObjectData objdata in original.Items)
                {
                    Items.Add(new DLCObjectData(objdata));
                }
                if (original.TextureData != null)
                {
                    TextureData = new byte[original.TextureData.Length];
                    Array.Copy(original.TextureData, TextureData, TextureData.Length);
                }
                if (original.SoundData != null)
                {
                    SoundData = new byte[original.SoundData.Length];
                    Array.Copy(original.SoundData, SoundData, SoundData.Length);
                }
                if (original.ModelData != null)
                {
                    ModelData = new byte[original.ModelData.Length];
                    Array.Copy(original.ModelData, ModelData, ModelData.Length);
                }
            }

            /// <summary>
            /// Save to a byte array (VMS file)
            /// </summary>
            public byte[] GetBytes()
            {
                List<byte> result = new List<byte>();
                // Convert item table header
                List<byte> itemtable = new List<byte>();
                itemtable.AddRange(BitConverter.GetBytes(Identifier));
                byte chars_sonictails = 0;
                byte chars_knuxe102 = 0;
                byte chars_amybig = 0;
                byte chars_whatev = 0;
                if (EnableSonic) chars_sonictails |= 0x1;
                if (EnableTails) chars_sonictails |= 0x10;
                itemtable.Add(chars_sonictails);
                if (EnableKnuckles) chars_knuxe102 |= 0x1;
                if (EnableGamma) chars_knuxe102 |= 0x10;
                itemtable.Add(chars_knuxe102);
                if (EnableAmy) chars_amybig |= 0x1;
                if (EnableBig) chars_amybig |= 0x10;
                itemtable.Add(chars_amybig);
                if (EnableWhatever1) chars_whatev |= 0x1;
                if (EnableWhatever2) chars_whatev |= 0x10;
                itemtable.Add(chars_whatev);
                itemtable.AddRange(BitConverter.GetBytes((int)Region));
                // Convert item table
                if (Items != null && Items.Count > 0)
                foreach (DLCObjectData item in Items)
                {
                    itemtable.Add(item.Level);
                    itemtable.Add(item.Act);
                    itemtable.Add(item.ScaleX);
                    itemtable.Add(item.ScaleY);
                    itemtable.Add(item.ScaleZ);
                    itemtable.Add((byte)item.RotSpeedX);
                    itemtable.Add((byte)item.RotSpeedY);
                    itemtable.Add((byte)item.RotSpeedZ);
                    itemtable.Add((byte)item.ObjectType);
                    itemtable.Add(item.Texture);
                    itemtable.AddRange((BitConverter.GetBytes((ushort)item.Flags)));
                    itemtable.Add(item.InternalID);
                    itemtable.Add(item.Unknown3);
                    itemtable.Add(item.Message);
                    itemtable.Add(item.TriggerDistance);
                    itemtable.Add(item.WarpLevelOrSoundbank);
                    itemtable.Add(item.WarpActOrSoundID);
                    itemtable.AddRange(BitConverter.GetBytes(item.RotationX));
                    itemtable.AddRange(BitConverter.GetBytes(item.RotationY));
                    itemtable.AddRange(BitConverter.GetBytes(item.RotationZ));
                    itemtable.AddRange(BitConverter.GetBytes(item.X));
                    itemtable.AddRange(BitConverter.GetBytes(item.Y));
                    itemtable.AddRange(BitConverter.GetBytes(item.Z));
                }
                if (itemtable.Count % 32 != 0)
                do
                    itemtable.Add(0);
                while (itemtable.Count % 32 != 0);
                // Convert Japanese strings
                List<byte> stringtable = new List<byte>();
                foreach (string str in JapaneseStrings)
                {
                    if (str != null)
                        for (int s = 0; s < str.Length; s++)
                    {
                        if (str[s] == '\t')
                        {
                            stringtable.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes("t"));
                        }
                        else if (str[s] == '\n')
                        {
                            stringtable.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes("n"));
                        }
                        else
                        {
                            stringtable.AddRange(System.Text.Encoding.GetEncoding(932).GetBytes(str[s].ToString()));
                        }
                    }
                    do
                    {
                        stringtable.Add(0);
                    }
                    while (stringtable.Count % 64 != 0);
                }
                // Convert European strings
                if (EnglishStrings != null)
                {
                    stringtable.AddRange(ProcessStrings(EnglishStrings, 932)); // English uses the same character set as Japanese
                    stringtable.AddRange(ProcessStrings(FrenchStrings));
                    stringtable.AddRange(ProcessStrings(SpanishStrings));
                    stringtable.AddRange(ProcessStrings(GermanStrings));
                }
                // Set size
                int fullsize = SoundData.Length + ModelData.Length + TextureData.Length + itemtable.Count + stringtable.Count + 64; // 64 is sections table at 0x280 w/checksum + 16 bytes of padding
                if ((fullsize + 640) % 512 != 0)
                {
                    do
                    {
                        fullsize++;
                    }
                    while ((fullsize + 640) % 512 != 0);
                }
                // Convert title, description etc.
                byte[] title_b = new byte[16];
                byte[] title = System.Text.Encoding.GetEncoding(932).GetBytes(Title);
                Array.Copy(title, 0, title_b, 0, title.Length);
                result.AddRange(title_b);
                byte[] desc_b = new byte[32];
                byte[] desc = System.Text.Encoding.GetEncoding(932).GetBytes(Description);
                Array.Copy(desc, 0, desc_b, 0, desc.Length);
                result.AddRange(desc_b);
                byte[] app_b = new byte[16];
                byte[] app = System.Text.Encoding.GetEncoding(932).GetBytes(AppName);
                Array.Copy(app, 0, app_b, 0, app.Length);
                result.AddRange(app_b);
                result.AddRange(BitConverter.GetBytes((ushort)1)); // Number of icons
                result.AddRange(BitConverter.GetBytes((ushort)1)); // Animation speed
                result.AddRange(BitConverter.GetBytes((ushort)0)); // Eyecatch type
                result.AddRange(BitConverter.GetBytes((ushort)0)); // CRC (unused)
                result.AddRange(BitConverter.GetBytes((uint)fullsize)); // Size of the entire thing without VMS header
                for (int u = 0; u < 20; u++)
                    result.Add(0);
                result.AddRange(GetIconBytes(Icon));
                result.AddRange(BitConverter.GetBytes((uint)0x2C0)); // Item layout table pointer
                result.AddRange(BitConverter.GetBytes((uint)Items.Count));
                int textpointer = 704 + itemtable.Count;
                result.AddRange(BitConverter.GetBytes((uint)textpointer)); // Text table pointer
                int textcount = JapaneseStrings.Length + EnglishStrings.Length + FrenchStrings.Length + GermanStrings.Length + SpanishStrings.Length;
                result.AddRange(BitConverter.GetBytes((uint)textcount));
                int pvmpointer = textpointer + 64 * textcount;
                result.AddRange(BitConverter.GetBytes((uint)pvmpointer));
                int pvmcount = (TextureData != null && TextureData.Length != 0) ? 1 : 0;
                result.AddRange(BitConverter.GetBytes((uint)pvmcount)); // PVM count
                ushort numtextures = 0;
                if (pvmcount != 0)
                    numtextures = BitConverter.ToUInt16(TextureData, 0xA);
                result.AddRange(BitConverter.GetBytes((uint)numtextures)); // Number of textures in the PVM
                int mltpointer = pvmpointer + TextureData.Length;
                result.AddRange(BitConverter.GetBytes((uint)mltpointer));
                if (ContainsSound)
                    result.AddRange(BitConverter.GetBytes((uint)1)); // MLT count
                else
                    result.AddRange(BitConverter.GetBytes((uint)0));
                int prspointer = mltpointer + SoundData.Length;
                result.AddRange(BitConverter.GetBytes((uint)prspointer));
                if (ModelData != null && ModelData.Length > 0)
                    result.AddRange(BitConverter.GetBytes((uint)1)); // Number of PRS
                else
                    result.AddRange(BitConverter.GetBytes((uint)0));
                List<byte> final = new List<byte>();
                final.AddRange(itemtable.ToArray());
                final.AddRange(stringtable.ToArray());
                if (TextureData != null && TextureData.Length > 0)
                    final.AddRange(TextureData);
                if (ContainsSound) final.AddRange(SoundData);
                final.AddRange(ModelData);
                byte[] finalarr = final.ToArray();
                uint checksum = CalculateChecksum(ref finalarr, 0, finalarr.Length);
                result.AddRange(BitConverter.GetBytes(checksum));
                for (int u = 0; u < 16; u++)
                    result.Add(0);
                result.AddRange(final);
                if (result.Count % 512 != 0)
                {
                    do
                    {
                        //System.Windows.Forms.MessageBox.Show("Adding to " + result.Count.ToString());
                        //File.WriteAllBytes("C:\\Users\\Pkr\\Desktop\\ass", result.ToArray());
                        result.Add(0);
                    }
                    while (result.Count % 512 != 0);
                }
                // Encrypt
                byte[] resdata = result.ToArray();
                ProcessVMS(ref resdata);
                return resdata;
            }

        }

        public class DLCObjectData
        {
            [IniAlwaysInclude]
            public byte Level;
            [IniAlwaysInclude]
            public byte Act;
            [IniAlwaysInclude]
            public byte ScaleX;
            [IniAlwaysInclude]
            public byte ScaleY;
            [IniAlwaysInclude]
            public byte ScaleZ;
            public sbyte RotSpeedX;
            public sbyte RotSpeedY;
            public sbyte RotSpeedZ;
            [IniAlwaysInclude]
            public DLCObjectTypes ObjectType;
            [IniName("TextureID")]
            public byte Texture;
            [IniAlwaysInclude]
            public DLCObjectFlags Flags;
            [IniName("CollectibleID")]
            public byte InternalID; // Collectible: internal ID, TIMER: Number of objects to collect, CHALLENGE: Number of seconds / 10
            public byte Unknown3;
            [IniName("MessageID")]
            public byte Message;
            [IniName("TriggerRadius")]
            public byte TriggerDistance;
            public byte WarpLevelOrSoundbank;
            public byte WarpActOrSoundID;
            public ushort RotationX;
            public ushort RotationY;
            public ushort RotationZ;
            public short X;
            public short Y;
            public short Z;

            public DLCObjectData() { }

            public DLCObjectData(DLCObjectData origdata)
            {
                Act = origdata.Act;
                Flags = origdata.Flags;
                Level = origdata.Level;
                Message = origdata.Message;
                InternalID = origdata.InternalID;
                ObjectType = origdata.ObjectType;
                TriggerDistance = origdata.TriggerDistance;
                RotSpeedX = origdata.RotSpeedX;
                RotSpeedY = origdata.RotSpeedY;
                RotSpeedZ = origdata.RotSpeedZ;
                RotationX = origdata.RotationX;
                RotationY = origdata.RotationY;
                RotationZ = origdata.RotationZ;
                ScaleX = origdata.ScaleX;
                ScaleY = origdata.ScaleY;
                ScaleZ = origdata.ScaleZ;
                WarpActOrSoundID = origdata.WarpActOrSoundID;
                Texture = origdata.Texture;
                Unknown3 = origdata.Unknown3;
                WarpLevelOrSoundbank = origdata.WarpLevelOrSoundbank;
                X = origdata.X;
                Y = origdata.Y;
                Z = origdata.Z;
            }
        }

        public static void ProcessVMS(ref byte[] input)
        {
            int aniFrames = input[0x40] + (input[0x41] << 8);
            int dataStart = 0x80 + (aniFrames * 0x200);
            byte[] encrypted = new byte[input.Length - dataStart];
            Array.Copy(input, dataStart, encrypted, 0, encrypted.Length);
            VMSFile.DecryptData(ref encrypted);
            Array.Copy(encrypted, 0, input, dataStart, encrypted.Length);
        }

        public static byte[] GetIconBytes(Bitmap bitmap)
        {
            List<byte> result = new List<byte>();
            // Get palette
            ushort[] colors_short = new ushort[16];
            Color[] colors = new Color[16];
            if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed)
            {
                //Console.WriteLine("Icon is {0}, converting to {1} (loss of quality possible)", bitmap.PixelFormat.ToString(), PixelFormat.Format4bppIndexed.ToString());
                ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
                bitmap = ciq.ReduceColors(bitmap, 16);
            }
            for (int u = 0; u < 16; u++)
            {
                colors[u] = bitmap.Palette.Entries[u];
                int a = colors[u].A / 16;
                int r = colors[u].R / 16;
                int g = colors[u].G / 16;
                int b = colors[u].B / 16;
                colors_short[u] = (ushort)(a << 12 | r << 8 | g << 4 | b);
                //Console.WriteLine("Color to binary {0} ({1}) : {2} comp A:{3} R:{4} G:{5} B:{6}", u, colors[u].ToString(), colors_short[u].ToString("X"), a.ToString("X"),r.ToString("X"), g.ToString("X"), b.ToString("X"));
                result.AddRange(BitConverter.GetBytes(colors_short[u]));
                //Console.WriteLine("Color {0}:{1}", u, colors[u].ToString());
            }
            // Get colors
            byte[] image = new byte[1024];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    for (byte c = 0; c < 16; c++)
                    {
                        if (bitmap.GetPixel(x, y) == colors[c])
                        {
                            image[y * 32 + x] = c;
                            break;
                        }
                    }
                }
            }
            byte[] squeeze = new byte[512];
            for (int u = 0; u < 1024; u++)
            {
                if (u % 2 == 0)
                {
                    squeeze[u / 2] = (byte)(image[u] << 4 | image[u + 1] & 0xF);
                    //Console.WriteLine("Byte {0}: {1} / {2}", image[u], image[u] << 4, image[u + 1] & 0xF);
                }
            }
            result.AddRange(squeeze);
            return result.ToArray();
        }

        public static byte[] ConvertModel(string filename)
        {
            List<byte> result = new List<byte>();
            ModelFile mfile = new ModelFile(filename);
            byte[] mdlarr = mfile.Model.GetBytes(0xCCA4000 + 4, false, out uint addr);
            result.AddRange(BitConverter.GetBytes(addr + 4 + 0xCCA4000));
            result.AddRange(mdlarr);
            byte[] res_arr = result.ToArray();
            result = FraGag.Compression.Prs.Compress(res_arr).ToList();
            if (result.Count % 16 != 0)
            {
                do
                {
                    result.Add(0);
                }
                while (result.Count % 16 != 0);
            }
            return result.ToArray();
        }

        public static byte[] ProcessStrings(string[] list, int codepage = 1252)
        {
            List<byte> result = new List<byte>();
            foreach (string str in list)
            {
                if (str != null)
                    for (int s = 0; s < str.Length; s++)
                {
                    if (str[s] == '\t')
                    {
                        result.AddRange(System.Text.Encoding.GetEncoding(1252).GetBytes("~"));
                    }
                    else if (str[s] == '\n')
                    {
                        result.AddRange(System.Text.Encoding.GetEncoding(1252).GetBytes("@"));
                    }
                    else
                    {
                        result.AddRange(System.Text.Encoding.GetEncoding(codepage).GetBytes(str[s].ToString()));
                    }
                }
                do
                {
                    result.Add(0);
                }
                while (result.Count % 64 != 0);
            }
            return result.ToArray();
        }

       
        public static uint CalculateChecksum(ref byte[] buf, int start, int end)
        {
            // Code by Sappharad
            uint result = 0;
            for (int i = start; i < end; i++)
            {
                int notByte = buf[i];
                if (notByte >= 128)
                {
                    notByte -= 256;
                }
                result = (uint)(result + notByte);
            }
            return ~result;
        }  

        public static Bitmap GetIconFromFile(byte[] file)
        {
            Bitmap bmp = new Bitmap(32, 32, PixelFormat.Format4bppIndexed);
            var newpalette = bmp.Palette;
            // Get palette
            ushort[] colors_short = new ushort[16];
            Color[] colors = new Color[16];
            for (int u = 0; u < 16; u++)
            {
                colors_short[u] = BitConverter.ToUInt16(file, 0x60 + u * 2);
                //Console.WriteLine("Source color {0}: {1}", u, colors_short[u].ToString("X4"));
                int a = colors_short[u] >> 12;
                int r = (colors_short[u] & 0xF00) >> 8;
                int g = (colors_short[u] & 0xF0) >> 4;
                int b = (colors_short[u] & 0xF);
                //Console.WriteLine("{0} {1} {2} {3}", a.ToString("X"), r.ToString("X"), g.ToString("X"), b.ToString("X"));
                colors[u] = Color.FromArgb(a * 16, r * 16, g * 16, b * 16);
                newpalette.Entries[u] = colors[u];
                //Console.WriteLine("Color from binary {0}: {1}", colors_short[u].ToString("X"), colors[u].ToString());
            }
            // Save icon
            bmp.Palette = newpalette;
            int pixel = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x += 2)
                {
                    BitmapData bmpData = bmp.LockBits(new Rectangle(x, y, 2, 1), ImageLockMode.ReadOnly, PixelFormat.Format4bppIndexed);
                    System.Runtime.InteropServices.Marshal.WriteByte(bmpData.Scan0, file[0x80 + pixel]);
                    bmp.UnlockBits(bmpData);
                    pixel++;
                }
            }
            return bmp;
        }

        public static string GetModelInfo(NJS_OBJECT obj)
		{
            string result;
            int numverts = 0;
            int numpolys = 0;
            int nummeshes = 0;
            int nummats = 0;
            int numuvs = 0;
            int numvcolors = 0;
            try
            {
                if (obj.Attach != null && obj.Attach is BasicAttach batt)
                {
                    if (batt.Material != null)
                        nummats += batt.Material.Count;
                    if (batt.Vertex != null)
                        numverts += batt.Vertex.Length;
                    if (batt.Mesh != null)
                    {
                        foreach (NJS_MESHSET mesh in batt.Mesh)
                        {
                            nummeshes++;
                            if (mesh.Poly != null)
                                numpolys += mesh.Poly.Count;
                            if (mesh.UV != null)
                                numuvs += mesh.UV.Length;
                            if (mesh.VColor != null)
                                numvcolors += mesh.VColor.Length;
                        }
                    }
                }
                if (obj.Children != null && obj.Children.Count > 0)
                {
                    foreach (NJS_OBJECT child in obj.Children)
                    {
                        if (child.Attach != null && child.Attach is BasicAttach childatt)
                        {
                            if (childatt.Material != null)
                                nummats += childatt.Material.Count;
                            if (childatt.Vertex != null)
                                numverts += childatt.Vertex.Length;
                            foreach (NJS_MESHSET mesh in childatt.Mesh)
                            {
                                nummeshes++;
                                if (mesh.Poly != null)
                                    numpolys += mesh.Poly.Count;
                                if (mesh.UV != null)
                                    numuvs += mesh.UV.Length;
                                if (mesh.VColor != null)
                                    numvcolors += mesh.VColor.Length;
                            }
                        }
                    }
                }
                result = "Vertices: " + numverts.ToString() + ", Polys: " + numpolys.ToString() + ", Meshes: " + nummeshes.ToString() + ", Materials: " + nummats.ToString() + ", UVs: " + numuvs.ToString() + ", VColors: " + numvcolors.ToString();
            }
            catch (Exception)
            {
                result = "Error getting model information.";
            }
            return result;
        }

        public static string[] LevelIDStrings =
        {
            "H. Hammer",
            "Eme. Coast",
            "W. Valley",
            "Tw. Park",
            "Sp. Highway",
            "Red Mount.",
            "Sky Deck",
            "Lost World",
            "Ice Cap",
            "Casino",
            "Fin. Egg",
            "11 (unused)",
            "Hot Shelter",
            "13 (unused)",
            "14 (unused)",
            "Chaos 0",
            "Chaos 2",
            "Chaos 4",
            "Chaos 6",
            "Per. Chaos",
            "E. Hornet",
            "E. Walker",
            "E. Viper",
            "ZERO",
            "E-101",
            "E-101R",
            "St. Square",
            "27 (unused)",
            "28 (unused)",
            "EC Outside",
            "30 (unused)",
            "31 (unused)",
            "EC Inside",
            "M. Ruins",
            "Past",
            "T. Circuit",
            "Sky Chase 1",
            "Sky Chase 2",
            "S. Hill",
            "SS Garden",
            "EC Garden",
            "MR Garden",
        };

        public enum DLCObjectTypes
        {
            TYPE_MODEL = 0,
            TYPE_SPRITE = 0x80,
            TYPE_INVISIBLE = 0xFF,
        }

        public enum DLCRegionLocks
        {
            Disabled = -1,
            Japan = 1,
            US = 3,
            Europe = 4,
            All = 7,
        }

        const int BIT_0 = (1 << 0);
        const int BIT_1 = (1 << 1);
        const int BIT_2 = (1 << 2);
        const int BIT_3 = (1 << 3);
        const int BIT_4 = (1 << 4);
        const int BIT_5 = (1 << 5);
        const int BIT_6 = (1 << 6);
        const int BIT_7 = (1 << 7);
        const int BIT_8 = (1 << 8);
        const int BIT_9 = (1 << 9);
        const int BIT_10 = (1 << 10);
        const int BIT_11 = (1 << 11);
        const int BIT_12 = (1 << 12);
        const int BIT_13 = (1 << 13);
        const int BIT_14 = (1 << 14);
        const int BIT_15 = (1 << 15);

        [Flags]
        public enum DLCObjectFlags
        {
            FLAG_SOLID = BIT_8,
            FLAG_SOUND = BIT_9,
            FLAG_MESSAGE = BIT_10,
            FLAG_COLLISION_ONLY = BIT_11,
            FLAG_WARP = BIT_12,
            FLAG_COLLECTIBLE = BIT_13,
            FLAG_TIMER = BIT_14,
            FLAG_CHALLENGE = BIT_15,
            UNKNOWN_0 = BIT_0, // ??
            UNKNOWN_4 = BIT_4, // ??
            UNUSED_1 = BIT_1,
            UNUSED_2 = BIT_2,
            UNUSED_3 = BIT_3,
            UNUSED_5 = BIT_5,
            UNUSED_6 = BIT_6,
            UNUSED_7 = BIT_7,
        }

    }
}