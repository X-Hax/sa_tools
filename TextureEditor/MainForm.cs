using PAKLib;
using PuyoTools.Modules.Archive;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VrSharp.Gvr;
using VrSharp.Pvr;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace TextureEditor
{
    public partial class MainForm : Form
    {
        readonly Properties.Settings Settings = Properties.Settings.Default;

        public MainForm()
        {
            InitializeComponent();
        }

        TextureFormat format;
        string filename;
        List<TextureInfo> textures = new List<TextureInfo>();

        private void SetFilename(string filename)
        {
            if (Settings.MRUList.Count > 10)
            {
                for (int i = 9; i < Settings.MRUList.Count; i++)
                {
                    Settings.MRUList.RemoveAt(i);
                }
            }
            this.filename = filename;
            if (Settings.MRUList.Contains(filename))
            {
                int i = Settings.MRUList.IndexOf(filename);
                Settings.MRUList.RemoveAt(i);
                recentFilesToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            Settings.MRUList.Insert(0, filename);
            recentFilesToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename.Replace("&", "&&")));
            Text = format.ToString() + " Editor - " + filename;
        }

        private void UpdateTextureCount()
        {
            if (textures.Count == 1)
                toolStripStatusLabel1.Text = "1 texture";
            else
                toolStripStatusLabel1.Text = textures.Count + " textures";
            alphaSortingToolStripMenuItem.Enabled = format == TextureFormat.PAK;
        }

        private bool GetTextures(string filename)
        {
            byte[] pvmdata = File.ReadAllBytes(filename);
            if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
            ArchiveBase pvmfile = new PvmArchive();
            List<TextureInfo> newtextures;
            if (PvmxArchive.Is(pvmdata))
            {
                format = TextureFormat.PVMX;
                newtextures = new List<TextureInfo>(PvmxArchive.GetTextures(pvmdata).Cast<TextureInfo>());
            }
            else if (PAKFile.Is(filename))
            {
                format = TextureFormat.PAK;
                PAKFile pak = new PAKFile(filename);
                string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
                bool hasIndex = false;
                foreach (PAKFile.File fl in pak.Files)
                {
                    if (fl.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase))
                        hasIndex = true;
                }
                if (!hasIndex)
                {
                    MessageBox.Show("This PAK archive does not contain an index file, and Texture Editor cannot open it. Use PAKTool to open such files.", "Texture Editor");
                    textures.Clear();
                    listBox1.Items.Clear();
                    filename = null;
                    format = TextureFormat.PAK;
                    Text = "PAK Editor";
                    UpdateTextureCount();
                    return true;
                }
                byte[] inf = pak.Files.Single((file) => file.Name.Equals(filenoext + '\\' + filenoext + ".inf", StringComparison.OrdinalIgnoreCase)).Data;
                newtextures = new List<TextureInfo>(inf.Length / 0x3C);
                for (int i = 0; i < inf.Length; i += 0x3C)
                {
                    // Load a PAK INF entry
                    byte[] pakentry = new byte[0x3C];
                    Array.Copy(inf, i, pakentry, 0, 0x3C);
                    PAKInfEntry entry = new PAKInfEntry(pakentry);
                    // Load texture data
                    byte[] dds = pak.Files.First((file) => file.Name.Equals(filenoext + '\\' + entry.GetFilename() + ".dds", StringComparison.OrdinalIgnoreCase)).Data;
                    using (MemoryStream str = new MemoryStream(dds))
                    {
                        // Check if the texture is DDS
                        uint check = BitConverter.ToUInt32(dds, 0);
                        if (check == 0x20534444) // DDS header
                        {
                            PixelFormat pxformat;
                            var image = Pfim.Pfim.FromStream(str, new Pfim.PfimConfig());
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    pxformat = PixelFormat.Format32bppArgb;
                                    break;
                                default:
                                    MessageBox.Show("Unsupported image format.");
                                    throw new NotImplementedException();
                            }
                            var bitmap = new Bitmap(image.Width, image.Height, pxformat);
                            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, pxformat);
                            Marshal.Copy(image.Data, 0, bmpData.Scan0, image.DataLen);
                            bitmap.UnlockBits(bmpData);
                            newtextures.Add(new PakTextureInfo(entry.GetFilename(), entry.globalindex, bitmap, entry.Type, entry.fSurfaceFlags));
                        }
                        else // Not DDS
                            newtextures.Add(new PakTextureInfo(entry.GetFilename(), entry.globalindex, new Bitmap(str), entry.Type, entry.fSurfaceFlags));
                    }
                }
            }
            else
            {
                MemoryStream stream = new MemoryStream(pvmdata);
                if (PvmArchive.Identify(stream))
                    format = TextureFormat.PVM;
                else
                {
                    pvmfile = new GvmArchive();
                    if (!GvmArchive.Identify(stream))
                    {
                        MessageBox.Show(this, "Could not open file \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    format = TextureFormat.GVM;
                }
                ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
                newtextures = new List<TextureInfo>(pvmentries.Count);
                switch (format)
                {
                    case TextureFormat.PVM:
                        PvpPalette pvp = null;
                        foreach (ArchiveEntry file in pvmentries)
                        {
                            MemoryStream str = (MemoryStream)file.Open();
                            PvrTexture vrfile = new PvrTexture(str);
                            if (vrfile.NeedsExternalPalette)
                            {
                                if (pvp == null)
                                    using (OpenFileDialog a = new OpenFileDialog
                                    {
                                        DefaultExt = "pvp",
                                        Filter = "PVP Files|*.pvp",
                                        InitialDirectory = Path.GetDirectoryName(filename),
                                        Title = "External palette file"
                                    })
                                        if (a.ShowDialog(this) == DialogResult.OK)
                                            pvp = new PvpPalette(a.FileName);
                                        else
                                        {
                                            MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return false;
                                        }
                            }
                            str.Seek(0, SeekOrigin.Begin);
                            newtextures.Add(new PvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str, pvp));
                        }
                        break;
                    case TextureFormat.GVM:
                        GvpPalette gvp = null;
                        foreach (ArchiveEntry file in pvmentries)
                        {
                            MemoryStream str = (MemoryStream)file.Open();
                            GvrTexture vrfile = new GvrTexture(file.Open());
                            if (vrfile.NeedsExternalPalette)
                            {
                                if (gvp == null)
                                    using (OpenFileDialog a = new OpenFileDialog
                                    {
                                        DefaultExt = "gvp",
                                        Filter = "GVP Files|*.gvp",
                                        InitialDirectory = Path.GetDirectoryName(filename),
                                        Title = "External palette file"
                                    })
                                        if (a.ShowDialog(this) == DialogResult.OK)
                                            gvp = new GvpPalette(a.FileName);
                                        else
                                        {
                                            MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return false;
                                        }
                            }
                            str.Seek(0, SeekOrigin.Begin);
                            newtextures.Add(new GvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str, gvp));
                        }
                        break;
                }
            }
            textures.Clear();
            textures.AddRange(newtextures);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
            UpdateTextureCount();
            SetFilename(Path.GetFullPath(filename));
            return true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            System.Collections.Specialized.StringCollection newlist = new System.Collections.Specialized.StringCollection();

            if (Settings.MRUList != null)
            {
                foreach (string file in Settings.MRUList)
                {
                    if (File.Exists(file))
                    {
                        newlist.Add(file);
                        recentFilesToolStripMenuItem.DropDownItems.Add(file.Replace("&", "&&"));
                    }
                }
            }

            Settings.MRUList = newlist;

            makePCCompatibleGVMsToolStripMenuItem.Checked = Settings.PCCompatGVM;

            if (Program.Arguments.Length > 0 && !GetTextures(Program.Arguments[0]))
                Close();
        }

        private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
                UpdateTextureView(textures[listBox1.SelectedIndex].Image);
        }

        private void newPVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textures.Clear();
            listBox1.Items.Clear();
            filename = null;
            format = TextureFormat.PVM;
            Text = "PVM Editor";
            UpdateTextureCount();
        }

        private void newGVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textures.Clear();
            listBox1.Items.Clear();
            filename = null;
            format = TextureFormat.GVM;
            Text = "GVM Editor";
            UpdateTextureCount();
        }

        private void newPVMXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textures.Clear();
            listBox1.Items.Clear();
            filename = null;
            format = TextureFormat.PVMX;
            Text = "PVMX Editor";
            UpdateTextureCount();
        }

        private void newPAKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textures.Clear();
            listBox1.Items.Clear();
            filename = null;
            format = TextureFormat.PAK;
            Text = "PAK Editor";
            UpdateTextureCount();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs;*.pvmx;*.pak" })
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    GetTextures(dlg.FileName);
                    listBox1_SelectedIndexChanged(sender, e);
                }
        }

        private int GetAlphaLevel(Bitmap img)
        {
            BitmapData bmpd = img.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmpd.Stride;
            byte[] bits = new byte[Math.Abs(stride) * bmpd.Height];
            Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
            img.UnlockBits(bmpd);
            int tlevels = 0;
            for (int y = 0; y < img.Height; y++)
            {
                int srcaddr = y * Math.Abs(stride);
                for (int x = 0; x < img.Width; x++)
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
            return tlevels;
        }

        private MemoryStream UpdateGBIX(MemoryStream stream, uint gbix, bool bigendian = false)
        {
            byte[] arr = stream.ToArray();
            for (int u = 0; u < arr.Length - 4; u++)
            {
                if (BitConverter.ToUInt32(arr, u) == 0x58494247) // GBIX
                {
                    byte[] value = BitConverter.GetBytes(gbix);
                    if (bigendian)
                    {
                        arr[u + 11] = value[0];
                        arr[u + 10] = value[1];
                        arr[u + 9] = value[2];
                        arr[u + 8] = value[3];
                    }
                    else
                    {
                        arr[u + 8] = value[0];
                        arr[u + 9] = value[1];
                        arr[u + 10] = value[2];
                        arr[u + 11] = value[3];
                    }
                    return new MemoryStream(arr);
                }
            }
            MessageBox.Show("Unable to set the global index. The texture may be corrupted.");
            return stream;
        }

        private MemoryStream EncodePVR(PvrTextureInfo tex)
        {
            if (tex.TextureData != null)
                return UpdateGBIX(tex.TextureData, tex.GlobalIndex);
            if (tex.DataFormat != PvrDataFormat.Index4 && tex.DataFormat != PvrDataFormat.Index8)
            {
                int tlevels = GetAlphaLevel(tex.Image);
                if (tlevels == 0)
                    tex.PixelFormat = PvrPixelFormat.Rgb565;
                else if (tlevels == 1)
                    tex.PixelFormat = PvrPixelFormat.Argb1555;
                else if (tlevels == 2)
                    tex.PixelFormat = PvrPixelFormat.Argb4444;
                if (tex.Image.Width == tex.Image.Height)
                    if (tex.Mipmap)
                        tex.DataFormat = PvrDataFormat.SquareTwiddledMipmaps;
                    else
                        tex.DataFormat = PvrDataFormat.SquareTwiddled;
                else
                    tex.DataFormat = PvrDataFormat.Rectangle;
            }
            else // Cannot encode a paletted texture 
                return null;
            PvrTextureEncoder encoder = new PvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
            encoder.GlobalIndex = tex.GlobalIndex;
            MemoryStream pvr = new MemoryStream();
            encoder.Save(pvr);
            pvr.Seek(0, SeekOrigin.Begin);
            return pvr;
        }

        private MemoryStream EncodeGVR(GvrTextureInfo tex)
        {
            if (tex.TextureData != null)
                return UpdateGBIX(tex.TextureData, tex.GlobalIndex, true);
            if (tex.DataFormat != GvrDataFormat.Index4 && tex.DataFormat != GvrDataFormat.Index8)
            {
                if (Settings.PCCompatGVM)
                {
                    int tlevels = GetAlphaLevel(tex.Image);
                    if (tlevels == 0)
                        tex.DataFormat = GvrDataFormat.Dxt1;
                    else
                        tex.DataFormat = GvrDataFormat.Rgb5a3;
                }
                else
                    tex.DataFormat = GvrDataFormat.Argb8888;
            }
            else // Cannot encode a paletted texture
                return null;
            GvrTextureEncoder encoder = new GvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
            encoder.GlobalIndex = tex.GlobalIndex;
            MemoryStream gvr = new MemoryStream();
            encoder.Save(gvr);
            gvr.Seek(0, SeekOrigin.Begin);
            return gvr;
        }

        private void SaveTextures()
        {
            byte[] data;
            using (MemoryStream str = new MemoryStream())
            {
                ArchiveWriter writer = null;
                bool paletteerror = false;
                switch (format)
                {
                    case TextureFormat.PVM:
                        writer = new PvmArchiveWriter(str);
                        foreach (PvrTextureInfo tex in textures)
                        {
                            MemoryStream pvr = EncodePVR(tex);
                            if (pvr == null)
                            {
                                if (!paletteerror)
                                {
                                    MessageBox.Show("Unable to encode textures with an external palette.\nUse ArchiveTool to convert archives with paletted textures.", "Texture Editor Error");
                                    paletteerror = true;
                                }
                                continue;
                            }
                            writer.CreateEntry(pvr, tex.Name);
                        }
                        break;
                    case TextureFormat.GVM:
                        writer = new GvmArchiveWriter(str);
                        foreach (GvrTextureInfo tex in textures)
                        {
                            MemoryStream gvr = EncodeGVR(tex);
                            if (gvr == null)
                            {
                                if (!paletteerror)
                                {
                                    MessageBox.Show("Unable to encode textures with an external palette.\nUse ArchiveTool to convert archives with paletted textures.", "Texture Editor Error");
                                    paletteerror = true;
                                }
                                continue;
                            }
                            writer.CreateEntry(gvr, tex.Name);
                        }
                        break;
                    case TextureFormat.PVMX:
                        PvmxArchive.Save(str, textures.Cast<PvmxTextureInfo>());
                        break;
                    case TextureFormat.PAK:
                        PAKFile pak = new PAKFile();
                        string filenoext = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
                        string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
                        List<byte> inf = new List<byte>(textures.Count * 0x3C);
                        foreach (TextureInfo item in textures)
                        {
                            using (MemoryStream tex = new MemoryStream())
                            {
                                item.Image.Save(tex, ImageFormat.Png);
                                byte[] tb = tex.ToArray();
                                string name = item.Name.ToLowerInvariant();
                                if (name.Length > 0x1C)
                                    name = name.Substring(0, 0x1C);
                                pak.Files.Add(new PAKFile.File(filenoext + '\\' + name + ".dds", longdir + '\\' + name + ".dds", tb));
                                // Create a new PAK INF entry
                                PAKInfEntry entry = new PAKInfEntry();
                                byte[] namearr = Encoding.ASCII.GetBytes(name);
                                Array.Copy(namearr, entry.filename, namearr.Length);
                                entry.globalindex = item.GlobalIndex;
                                entry.nWidth = (uint)item.Image.Width;
                                entry.nHeight = (uint)item.Image.Height;
                                // Salvage GVR data if available
                                if (item is PakTextureInfo pk)
                                {
                                    entry.Type = entry.PixelFormat = pk.DataFormat;
                                    entry.fSurfaceFlags = pk.SurfaceFlags;
                                }
                                if (item.Mipmap)
                                    entry.fSurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
                                inf.AddRange(entry.GetBytes());
                            }
                        }
                        pak.Files.Insert(0, new PAKFile.File(filenoext + '\\' + filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
                        pak.Save(filename);

                        return;
                }
                writer?.Flush();
                data = str.ToArray();
                str.Close();
            }
            if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                FraGag.Compression.Prs.Compress(data, filename);
            else
                File.WriteAllBytes(filename, data);
        }

        private void ConvertTextures(TextureFormat newfmt)
        {
            switch (newfmt)
            {
                case TextureFormat.PVM:
                    switch (format)
                    {
                        case TextureFormat.GVM:
                            textures = new List<TextureInfo>(textures.Cast<GvrTextureInfo>().Select(a => new PvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                        case TextureFormat.PVMX:
                        case TextureFormat.PAK:
                            textures = new List<TextureInfo>(textures.Select(a => new PvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
                case TextureFormat.GVM:
                    switch (format)
                    {
                        case TextureFormat.PVM:
                            textures = new List<TextureInfo>(textures.Cast<PvrTextureInfo>().Select(a => new GvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                        case TextureFormat.PVMX:
                        case TextureFormat.PAK:
                            textures = new List<TextureInfo>(textures.Select(a => new GvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
                case TextureFormat.PVMX:
                    switch (format)
                    {
                        case TextureFormat.PVM:
                        case TextureFormat.GVM:
                        case TextureFormat.PAK:
                            textures = new List<TextureInfo>(textures.Select(a => new PvmxTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
                case TextureFormat.PAK:
                    switch (format)
                    {
                        case TextureFormat.PVM:
                        case TextureFormat.GVM:
                        case TextureFormat.PVMX:
                            textures = new List<TextureInfo>(textures.Select(a => new PakTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
            }
            format = newfmt;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filename == null)
                saveAsToolStripMenuItem_Click(sender, e);
            else
                SaveTextures();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            string defext = null;
            switch (format)
            {
                case TextureFormat.PVM:
                    defext = "pvm";
                    break;
                case TextureFormat.GVM:
                    defext = "gvm";
                    break;
                case TextureFormat.PVMX:
                    defext = "pvmx";
                    break;
                case TextureFormat.PAK:
                    defext = "pak";
                    break;
            }
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = defext, Filter = "PVM Files|*.pvm;*.prs|GVM Files|*.gvm;*.prs|PVMX Files|*.pvmx|PAK Files|*.pak", FilterIndex = (int)format + 1 })
            {
                if (filename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                    dlg.FileName = Path.GetFileName(filename);
                }
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ConvertTextures((TextureFormat)(dlg.FilterIndex - 1));
                    SetFilename(dlg.FileName);
                    SaveTextures();
                }
            }
        }

        private void saveAsPVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pvm", Filter = "PVM Files|*.pvm;*.prs" })
            {
                if (filename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                    dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "pvm");
                }
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ConvertTextures(TextureFormat.PVM);
                    SetFilename(dlg.FileName);
                    SaveTextures();
                }
            }
        }

        private void saveAsGVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "gvm", Filter = "GVM Files|*.gvm;*.prs" })
            {
                if (filename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                    dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "gvm");
                }
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ConvertTextures(TextureFormat.GVM);
                    SetFilename(dlg.FileName);
                    SaveTextures();
                }
            }
        }

        private void saveAsPVMXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pvmx", Filter = "PVMX Files|*.pvmx" })
            {
                if (filename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                    dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "pvmx");
                }
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ConvertTextures(TextureFormat.PVMX);
                    SetFilename(dlg.FileName);
                    SaveTextures();
                }
            }
        }

        private void saveAsPAKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "pak", Filter = "PAK Files|*.pak" })
            {
                if (filename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                    dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), "pak");
                }
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ConvertTextures(TextureFormat.PAK);
                    SetFilename(dlg.FileName);
                    SaveTextures();
                }
            }
        }

        private void importAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "txt", Filter = "index.txt|index.txt", FileName = "index.txt" })
            {
                if (filename != null)
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    using (TextReader texList = File.OpenText(dlg.FileName))
                    {
                        string dir = Path.GetDirectoryName(dlg.FileName);
                        listBox1.BeginUpdate();
                        string line = texList.ReadLine();
                        while (line != null)
                        {
                            string[] split = line.Split(',');
                            if (split.Length > 1)
                            {
                                uint gbix = uint.Parse(split[0]);
                                string name = Path.ChangeExtension(split[1], null);
                                Bitmap bmp;
                                using (Bitmap tmp = new Bitmap(Path.Combine(dir, split[1])))
                                    bmp = new Bitmap(tmp);
                                switch (format)
                                {
                                    case TextureFormat.PVM:
                                        textures.Add(new PvrTextureInfo(name, gbix, bmp));
                                        break;
                                    case TextureFormat.GVM:
                                        textures.Add(new GvrTextureInfo(name, gbix, bmp));
                                        break;
                                    case TextureFormat.PVMX:
                                        PvmxTextureInfo pvmx = new PvmxTextureInfo(name, gbix, bmp);
                                        if (split.Length > 2)
                                        {
                                            string[] dim = split[2].Split('x');
                                            if (dim.Length > 1)
                                                pvmx.Dimensions = new Size(int.Parse(dim[0]), int.Parse(dim[1]));
                                        }
                                        textures.Add(pvmx);
                                        break;
                                    case TextureFormat.PAK:
                                        textures.Add(new PakTextureInfo(name, gbix, bmp));
                                        break;
                                }
                                listBox1.Items.Add(name);

                            }
                            line = texList.ReadLine();
                        }
                        listBox1.EndUpdate();
                        listBox1.SelectedIndex = textures.Count - 1;
                        UpdateTextureCount();
                    }
            }
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "", Filter = "", FileName = "texturepack" })
            {

                if (filename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(filename);
                    dlg.FileName = Path.GetFileNameWithoutExtension(filename);
                }

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Directory.CreateDirectory(dlg.FileName);
                    string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
                    using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
                    {
                        foreach (TextureInfo tex in textures)
                        {
                            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(tex.Image);
                            bmp.Save(Path.Combine(dir, tex.Name + ".png"));
                            if (tex is PvmxTextureInfo xtex && xtex.Dimensions.HasValue)
                                texList.WriteLine("{0},{1},{2}x{3}", xtex.GlobalIndex, xtex.Name + ".png", xtex.Dimensions.Value.Width, xtex.Dimensions.Value.Height);
                            else
                                texList.WriteLine("{0},{1},{2}x{3}", tex.GlobalIndex, tex.Name + ".png", tex.Image.Width, tex.Image.Height);
                        }
                    }
                }
            }
        }

        private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            GetTextures(Settings.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateTextureView(Image image)
        {
            int width = image.Width;
            int height = image.Height;
            int maxwidth = splitContainer1.Panel2.Width - 20;
            int maxheight = splitContainer1.Panel2.Height - (tableLayoutPanel1.Top + (tableLayoutPanel1.Height - textureImage.Height)) - 20;
            if (width > maxwidth || height > maxheight)
            {
                double widthpct = width / (double)maxwidth;
                double heightpct = height / (double)maxheight;
                switch (Math.Sign(widthpct - heightpct))
                {
                    case -1: // height > width
                        maxwidth = (int)(width / heightpct);
                        break;
                    case 1:
                        maxheight = (int)(height / widthpct);
                        break;
                }
                Bitmap bmp = new Bitmap(maxwidth, maxheight);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gfx.DrawImage(image, 0, 0, maxwidth, maxheight);
                }
                textureImage.Image = bmp;
            }
            else
                textureImage.Image = image;
        }

        bool suppress = false;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
            bool en = listBox1.SelectedIndex != -1;
            removeTextureButton.Enabled = textureName.Enabled = globalIndex.Enabled = importButton.Enabled = exportButton.Enabled = en;
            if (en)
            {
                suppress = true;
                textureUpButton.Enabled = listBox1.SelectedIndex > 0;
                textureDownButton.Enabled = listBox1.SelectedIndex < textures.Count - 1;
                textureName.Text = textures[listBox1.SelectedIndex].Name;
                globalIndex.Value = textures[listBox1.SelectedIndex].GlobalIndex;
                if (textures[listBox1.SelectedIndex].CheckMipmap())
                {
                    mipmapCheckBox.Enabled = true;
                    mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
                }
                else
                    mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
                UpdateTextureView(textures[listBox1.SelectedIndex].Image);
                textureSizeLabel.Text = $"Actual Size: {textures[listBox1.SelectedIndex].Image.Width}x{textures[listBox1.SelectedIndex].Image.Height}";
                switch (textures[listBox1.SelectedIndex])
                {
                    case PakTextureInfo pak:
                        dataFormatLabel.Text = $"Data Format: {pak.DataFormat}";
                        pixelFormatLabel.Text = $"Surface Flags: {pak.GetSurfaceFlags()}";
                        dataFormatLabel.Show();
                        pixelFormatLabel.Show();
                        checkBoxPAKUseAlpha.Enabled = true;
                        checkBoxPAKUseAlpha.Show();
                        if (pak.DataFormat == GvrDataFormat.Rgb5a3)
                            checkBoxPAKUseAlpha.Checked = true;
                        else
                            checkBoxPAKUseAlpha.Checked = false;
                        break;
                    case PvrTextureInfo pvr:
                        dataFormatLabel.Text = $"Data Format: {pvr.DataFormat}";
                        pixelFormatLabel.Text = $"Pixel Format: {pvr.PixelFormat}";
                        dataFormatLabel.Show();
                        pixelFormatLabel.Show();
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
                        numericUpDownOrigSizeX.Value = pvr.Image.Width;
                        numericUpDownOrigSizeY.Value = pvr.Image.Height;
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                    case GvrTextureInfo gvr:
                        dataFormatLabel.Text = $"Data Format: {gvr.DataFormat}";
                        pixelFormatLabel.Text = $"Pixel Format: {gvr.PixelFormat}";
                        dataFormatLabel.Show();
                        pixelFormatLabel.Show();
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
                        numericUpDownOrigSizeX.Value = gvr.Image.Width;
                        numericUpDownOrigSizeY.Value = gvr.Image.Height;
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                    case PvmxTextureInfo pvmx:
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = true;
                        if (pvmx.Dimensions.HasValue)
                        {
                            numericUpDownOrigSizeX.Value = pvmx.Dimensions.Value.Width;
                            numericUpDownOrigSizeY.Value = pvmx.Dimensions.Value.Height;
                        }
                        else
                        {
                            numericUpDownOrigSizeX.Value = pvmx.Image.Width;
                            numericUpDownOrigSizeY.Value = pvmx.Image.Height;
                        }
                        dataFormatLabel.Hide();
                        pixelFormatLabel.Hide();
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                    default:
                        dataFormatLabel.Hide();
                        pixelFormatLabel.Hide();
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
                        numericUpDownOrigSizeX.Value = textures[listBox1.SelectedIndex].Image.Width;
                        numericUpDownOrigSizeY.Value = textures[listBox1.SelectedIndex].Image.Height;
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                }
                suppress = false;
            }
            else
            {
                textureUpButton.Enabled = false;
                textureDownButton.Enabled = false;
                mipmapCheckBox.Enabled = false;
            }
        }

        private KeyValuePair<string, Bitmap>? BrowseForTexture(string textureName = null)
        {
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp" })
            {
                if (!String.IsNullOrEmpty(textureName))
                    dlg.FileName = textureName;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string name = Path.GetFileNameWithoutExtension(dlg.FileName);
                    if (PvrTexture.Is(dlg.FileName))
                        return new KeyValuePair<string, Bitmap>(name, new PvrTexture(dlg.FileName).ToBitmap());
                    else if (GvrTexture.Is(dlg.FileName))
                        return new KeyValuePair<string, Bitmap>(name, new GvrTexture(dlg.FileName).ToBitmap());
                    else
                        return new KeyValuePair<string, Bitmap>(name, new Bitmap(dlg.FileName));
                }
                else
                {
                    return null;
                }
            }
        }

        private void addTextureButton_Click(object sender, EventArgs e)
        {
            string defext = null;
            string filter = null;
            switch (format)
            {
                case TextureFormat.PVM:
                    defext = "pvr";
                    filter = "Texture Files|*.prs;*.pvm;*.pvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp";
                    break;
                case TextureFormat.GVM:
                    defext = "gvr";
                    filter = "Texture Files|*.prs;*.gvm;*.gvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp";
                    break;
                case TextureFormat.PVMX:
                case TextureFormat.PAK:
                    defext = "png";
                    filter = "Texture Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp";
                    break;
            }
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = defext, Filter = filter, Multiselect = true })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    uint gbix = textures.Count == 0 ? 0 : textures.Max((item) => item.GlobalIndex);
                    if (gbix != uint.MaxValue)
                        gbix++;
                    listBox1.BeginUpdate();
                    foreach (string file in dlg.FileNames)
                    {
                        switch (Path.GetExtension(file).ToLowerInvariant())
                        {
                            case ".prs":
                            case ".pvm":
                            case ".gvm":
                                byte[] pvmdata = File.ReadAllBytes(file);
                                if (Path.GetExtension(file).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                                    pvmdata = FraGag.Compression.Prs.Decompress(pvmdata);
                                ArchiveBase pvmfile = null;
                                switch (format)
                                {
                                    case TextureFormat.PVM:
                                        pvmfile = new PvmArchive();
                                        break;
                                    case TextureFormat.GVM:
                                        pvmfile = new GvmArchive();
                                        break;
                                }
                                MemoryStream stream = new MemoryStream(pvmdata);
                                if (!PvmArchive.Identify(stream) && !GvmArchive.Identify(stream))
                                {
                                    MessageBox.Show(this, "Could not open file \"" + file + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    continue;
                                }
                                ArchiveEntryCollection pvmentries = pvmfile.Open(pvmdata).Entries;
                                List<PvrTextureInfo> newtextures = new List<PvrTextureInfo>(pvmentries.Count);
                                switch (format)
                                {
                                    case TextureFormat.PVM:
                                        PvpPalette pvp = null;
                                        foreach (ArchiveEntry entry in pvmentries)
                                        {
                                            MemoryStream vrstream = (MemoryStream)(entry.Open());
                                            PvrTexture vrfile = new PvrTexture(vrstream);
                                            if (vrfile.NeedsExternalPalette)
                                            {
                                                if (pvp == null)
                                                    using (OpenFileDialog a = new OpenFileDialog
                                                    {
                                                        DefaultExt = "pvp",
                                                        Filter = "PVP Files|*.pvp",
                                                        InitialDirectory = Path.GetDirectoryName(file),
                                                        Title = "External palette file"
                                                    })
                                                        if (a.ShowDialog(this) == DialogResult.OK)
                                                            pvp = new PvpPalette(a.FileName);
                                                        else
                                                        {
                                                            MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                            continue;
                                                        }
                                            }
                                            string name = Path.GetFileNameWithoutExtension(entry.Name);
                                            textures.Add(new PvrTextureInfo(name, vrstream, pvp));
                                            listBox1.Items.Add(name);
                                        }
                                        break;
                                    case TextureFormat.GVM:
                                        GvpPalette gvp = null;
                                        foreach (ArchiveEntry entry in pvmentries)
                                        {
                                            MemoryStream vrstream = (MemoryStream)(entry.Open());
                                            GvrTexture vrfile = new GvrTexture(vrstream);
                                            if (vrfile.NeedsExternalPalette)
                                            {
                                                if (gvp == null)
                                                    using (OpenFileDialog a = new OpenFileDialog
                                                    {
                                                        DefaultExt = "gvp",
                                                        Filter = "GVP Files|*.gvp",
                                                        InitialDirectory = Path.GetDirectoryName(file),
                                                        Title = "External palette file"
                                                    })
                                                        if (a.ShowDialog(this) == DialogResult.OK)
                                                            gvp = new GvpPalette(a.FileName);
                                                        else
                                                        {
                                                            MessageBox.Show(this, "Could not open file \"" + Program.Arguments[0] + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                            continue;
                                                        }
                                            }
                                            string name = Path.GetFileNameWithoutExtension(entry.Name);
                                            textures.Add(new GvrTextureInfo(name, vrstream, gvp));
                                            listBox1.Items.Add(name);
                                        }
                                        break;
                                }
                                break;
                            default:
                                {
                                    string name = Path.GetFileNameWithoutExtension(file);
                                    MemoryStream str = new MemoryStream(File.ReadAllBytes(file));
                                    if (format == TextureFormat.PVM && PvrTexture.Is(file))
                                        textures.Add(new PvrTextureInfo(name, str));
                                    else if (format == TextureFormat.GVM && GvrTexture.Is(file))
                                        textures.Add(new GvrTextureInfo(name, str));
                                    else
                                    {
                                        switch (format)
                                        {
                                            case TextureFormat.PVM:
                                                textures.Add(new PvrTextureInfo(name, gbix, new Bitmap(file)));
                                                break;
                                            case TextureFormat.GVM:
                                                textures.Add(new GvrTextureInfo(name, gbix, new Bitmap(file)));
                                                break;
                                            case TextureFormat.PVMX:
                                                textures.Add(new PvmxTextureInfo(name, gbix, new Bitmap(file)));
                                                break;
                                            case TextureFormat.PAK:
                                                textures.Add(new PakTextureInfo(name, gbix, new Bitmap(file)));
                                                break;
                                        }
                                        if (gbix != uint.MaxValue)
                                            gbix++;
                                    }
                                    listBox1.Items.Add(name);
                                }
                                break;
                        }
                    }
                    listBox1.EndUpdate();
                    listBox1.SelectedIndex = textures.Count - 1;
                    UpdateTextureCount();
                }
            }
        }

        private void removeTextureButton_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            textures.RemoveAt(i);
            listBox1.Items.RemoveAt(i);
            if (i == textures.Count)
                --i;
            listBox1.SelectedIndex = i;
            UpdateTextureCount();
        }

        private void TextureUpButton_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            TextureInfo ti = textures[i];
            textures.RemoveAt(i);
            textures.Insert(i - 1, ti);
            listBox1.BeginUpdate();
            listBox1.Items.RemoveAt(i);
            listBox1.Items.Insert(i - 1, ti.Name);
            listBox1.EndUpdate();
            listBox1.SelectedIndex = i - 1;
        }

        private void TextureDownButton_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            TextureInfo ti = textures[i];
            textures.RemoveAt(i);
            textures.Insert(i + 1, ti);
            listBox1.BeginUpdate();
            listBox1.Items.RemoveAt(i);
            listBox1.Items.Insert(i + 1, ti.Name);
            listBox1.EndUpdate();
            listBox1.SelectedIndex = i + 1;
        }

        private void HexIndexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
        }

        private void textureName_TextChanged(object sender, EventArgs e)
        {
            bool focus = textureName.Focused;
            int st = textureName.SelectionStart;
            int len = textureName.SelectionLength;
            string name = textureName.Text;
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            listBox1.Items[listBox1.SelectedIndex] = textureName.Text = textures[listBox1.SelectedIndex].Name = name;
            if (focus)
                textureName.Focus();
            textureName.SelectionStart = st;
            textureName.SelectionLength = len;
        }

        private void globalIndex_ValueChanged(object sender, EventArgs e)
        {
            textures[listBox1.SelectedIndex].GlobalIndex = (uint)globalIndex.Value;
        }

        private void mipmapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            textures[listBox1.SelectedIndex].Mipmap = mipmapCheckBox.Checked;

            // Update surface flags for PAK textures
            if (textures[listBox1.SelectedIndex] is PakTextureInfo pk)
            {
                if (!mipmapCheckBox.Checked)
                    pk.SurfaceFlags &= ~NinjaSurfaceFlags.Mipmapped;
                else
                    pk.SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
                pixelFormatLabel.Text = $"Surface Flags: {pk.GetSurfaceFlags()}";
            }
        }

        private void textureImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && e.Button == MouseButtons.Left)
            {
                Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
                DataObject dobj = new DataObject();
                dobj.SetImage(bmp);
                string fn = Path.Combine(Path.GetTempPath(), textures[listBox1.SelectedIndex].Name + ".png");
                bmp.Save(fn);
                dobj.SetFileDropList(new System.Collections.Specialized.StringCollection() { fn });
                DoDragDrop(dobj, DragDropEffects.Copy);
            }
        }

        private void textureImage_DragEnter(object sender, DragEventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Bitmap)))
                e.Effect = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        }

        private void textureImage_DragDrop(object sender, DragEventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            Bitmap tex = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                using (Bitmap bmp = new Bitmap(((string[])e.Data.GetData(DataFormats.FileDrop, true))[0]))
                    tex = new Bitmap(bmp);
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
                tex = new Bitmap((Image)e.Data.GetData(DataFormats.Bitmap));
            else
                return;
            textures[listBox1.SelectedIndex].Image = tex;
            UpdateTextureView(textures[listBox1.SelectedIndex].Image);
            textureSizeLabel.Text = $"Actual Size: {tex.Width}x{tex.Height}";
            if (textures[listBox1.SelectedIndex].CheckMipmap())
            {
                mipmapCheckBox.Enabled = true;
                mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
            }
            else
                mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
        }

        private void textureImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && e.Button == MouseButtons.Right)
            {
                pasteToolStripMenuItem.Enabled = Clipboard.ContainsImage();
                contextMenuStrip1.Show(textureImage, e.Location);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(textures[listBox1.SelectedIndex].Image);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap tex = new Bitmap(Clipboard.GetImage());
            textures[listBox1.SelectedIndex].Image = tex;
            UpdateTextureView(textures[listBox1.SelectedIndex].Image);
            textureSizeLabel.Text = $"Size: {tex.Width}x{tex.Height}";
            if (textures[listBox1.SelectedIndex].CheckMipmap())
            {
                mipmapCheckBox.Enabled = true;
                mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
            }
            else
                mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            KeyValuePair<string, Bitmap>? tex = BrowseForTexture(listBox1.GetItemText(listBox1.SelectedItem));
            if (tex.HasValue)
            {
                textures[listBox1.SelectedIndex].Image = tex.Value.Value;
                UpdateTextureView(textures[listBox1.SelectedIndex].Image);
                textureSizeLabel.Text = $"Size: {tex.Value.Value.Width}x{tex.Value.Value.Height}";
                if (textures[listBox1.SelectedIndex].CheckMipmap())
                {
                    mipmapCheckBox.Enabled = true;
                    mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
                }
                else
                    mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
            }

            listBox1.Select();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
                    bmp.Save(dlg.FileName);
                }

            listBox1.Select();
        }

        private void addMipmapsToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TextureInfo info in textures)
                if (info.CheckMipmap())
                {
                    info.Mipmap = true;
                    if (info is PakTextureInfo pk)
                        pk.SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
                }
            if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].CheckMipmap())
                mipmapCheckBox.Checked = true;
        }

        private void makePCCompatibleGVMsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Settings.PCCompatGVM = makePCCompatibleGVMsToolStripMenuItem.Checked;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Save();
        }

        private void numericUpDownOrigSizeX_ValueChanged(object sender, EventArgs e)
        {
            if (!suppress && textures[listBox1.SelectedIndex] is PvmxTextureInfo tex)
                tex.Dimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
        }

        private void numericUpDownOrigSizeY_ValueChanged(object sender, EventArgs e)
        {
            if (!suppress && textures[listBox1.SelectedIndex] is PvmxTextureInfo tex)
                tex.Dimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
        }

        private void checkBoxPAKUseAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (!(textures[listBox1.SelectedIndex] is PakTextureInfo))
            {
                // This shouldn't trigger though
                MessageBox.Show("This flag is only meant to be used with textures in PAK archives.");
                return;
            }
            PakTextureInfo pk = (PakTextureInfo)textures[listBox1.SelectedIndex];
            // Use alpha
            if (checkBoxPAKUseAlpha.Checked)
                pk.DataFormat = GvrDataFormat.Rgb5a3;
            // Don't use alpha (palettized)
            else if ((pk.SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
                pk.DataFormat = GvrDataFormat.Index4;
            // Don't use alpha (regular)
            else
                pk.DataFormat = GvrDataFormat.Dxt1;
            dataFormatLabel.Text = $"Data Format: {pk.DataFormat}";
        }

        private void PAKEnableAlphaForAll(bool enable)
        {
            if (textures == null || textures.Count == 0)
                return;
            foreach (PakTextureInfo paktxt in textures)
            {
                if (enable)
                    paktxt.DataFormat = GvrDataFormat.Rgb5a3;
                else
                {
                    if ((paktxt.SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
                        paktxt.DataFormat = GvrDataFormat.Index4;
                    else paktxt.DataFormat = GvrDataFormat.Dxt1;
                }
            }
            if (listBox1.SelectedIndex != -1)
            {
                PakTextureInfo pakcur = (PakTextureInfo)textures[listBox1.SelectedIndex];
                checkBoxPAKUseAlpha.Checked = pakcur.DataFormat == GvrDataFormat.Rgb5a3;
                dataFormatLabel.Text = $"Data Format: {pakcur.DataFormat}";
            }
        }

        private void enablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PAKEnableAlphaForAll(true);
        }

        private void disablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PAKEnableAlphaForAll(false);
        }
    }

    abstract class TextureInfo
    {
        public string Name { get; set; }
        public uint GlobalIndex { get; set; }
        public bool Mipmap { get; set; }
        public Bitmap Image { get; set; }

        public abstract bool CheckMipmap();
    }

    class PvrTextureInfo : TextureInfo
    {
        public PvrDataFormat DataFormat { get; set; }
        public PvrPixelFormat PixelFormat { get; set; }
        public MemoryStream TextureData { get; set; }
        public PvrTextureInfo() { }

        public PvrTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            PixelFormat = PvrPixelFormat.Unknown;
            DataFormat = PvrDataFormat.Unknown;
            if (tex is PvrTextureInfo pv)
                TextureData = pv.TextureData;
        }

        public PvrTextureInfo(GvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            switch (tex.DataFormat)
            {
                case GvrDataFormat.Index4:
                    DataFormat = PvrDataFormat.Index4;
                    break;
                case GvrDataFormat.Index8:
                    DataFormat = PvrDataFormat.Index8;
                    break;
            }
        }

        public PvrTextureInfo(PvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            PixelFormat = tex.PixelFormat;
            DataFormat = tex.DataFormat;
            TextureData = tex.TextureData;
        }

        public PvrTextureInfo(string name, uint gbix, Bitmap bitmap)
        {
            Name = name;
            GlobalIndex = gbix;
            DataFormat = PvrDataFormat.Unknown;
            PixelFormat = PvrPixelFormat.Unknown;
            Image = bitmap;
        }

        public PvrTextureInfo(string name, MemoryStream str, PvpPalette pvp = null)
        {
            TextureData = str;
            PvrTexture texture = new PvrTexture(str);
            if (pvp != null)
                texture.SetPalette(pvp);
            Name = name;
            GlobalIndex = texture.GlobalIndex;
            DataFormat = texture.DataFormat;
            Mipmap = DataFormat == PvrDataFormat.SquareTwiddledMipmaps || DataFormat == PvrDataFormat.SquareTwiddledMipmapsAlt;
            PixelFormat = texture.PixelFormat;
            Image = texture.ToBitmap();
        }

        public override bool CheckMipmap()
        {
            return DataFormat != PvrDataFormat.Index4 && DataFormat != PvrDataFormat.Index8 && Image.Width == Image.Height;
        }
    }

    class GvrTextureInfo : TextureInfo
    {
        public GvrDataFormat DataFormat { get; set; }
        public GvrPixelFormat PixelFormat { get; set; }
        public MemoryStream TextureData { get; set; }

        public GvrTextureInfo() { }

        public GvrTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            PixelFormat = GvrPixelFormat.Unknown;
            DataFormat = GvrDataFormat.Unknown;
            if (tex is GvrTextureInfo gvrt)
                TextureData = gvrt.TextureData;
        }

        public GvrTextureInfo(PvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            switch (tex.DataFormat)
            {
                case PvrDataFormat.Index4:
                    DataFormat = GvrDataFormat.Index4;
                    break;
                case PvrDataFormat.Index8:
                    DataFormat = GvrDataFormat.Index8;
                    break;
            }
        }

        public GvrTextureInfo(GvrTextureInfo tex)
            : this((TextureInfo)tex)
        {
            PixelFormat = tex.PixelFormat;
            DataFormat = tex.DataFormat;
            TextureData = tex.TextureData;
        }

        public GvrTextureInfo(string name, uint gbix, Bitmap bitmap)
        {
            Name = name;
            GlobalIndex = gbix;
            DataFormat = GvrDataFormat.Unknown;
            PixelFormat = GvrPixelFormat.Unknown;
            Image = bitmap;
        }

        public GvrTextureInfo(string name, MemoryStream str, GvpPalette gvp = null)
        {
            Name = name;
            TextureData = str;
            GvrTexture texture = new GvrTexture(str);
            if (gvp != null)
                texture.SetPalette(gvp);
            GlobalIndex = texture.GlobalIndex;
            DataFormat = texture.DataFormat;
            Mipmap = texture.HasMipmaps;
            PixelFormat = texture.PixelFormat;
            Image = texture.ToBitmap();
        }

        public override bool CheckMipmap()
        {
            return DataFormat != GvrDataFormat.Index4 && DataFormat != GvrDataFormat.Index8;
        }
    }

    class PvmxTextureInfo : TextureInfo
    {
        public Size? Dimensions { get; set; }

        public PvmxTextureInfo() { }

        public PvmxTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            Image = tex.Image;
        }

        public PvmxTextureInfo(PvmxTextureInfo tex)
            : this((TextureInfo)tex)
        {
            Dimensions = tex.Dimensions;
        }

        public PvmxTextureInfo(string name, uint gbix, Bitmap bitmap)
        {
            Name = name;
            GlobalIndex = gbix;
            Image = bitmap;
        }

        public override bool CheckMipmap()
        {
            return false;
        }
    }

    enum NinjaSurfaceFlags : uint
    {
        Mipmapped = 0x80000000,
        VQ = 0x40000000,
        NotTwiddled = 0x04000000,
        Twiddled = 0x00000000,
        Stride = 0x02000000,
        Palettized = 0x00008000
    }

    class PAKInfEntry
    {
        public byte[] filename; // 28
        public uint globalindex;
        public GvrDataFormat Type;
        public uint BitDepth; // Unused
        public GvrDataFormat PixelFormat; // Duplicate of Type
        public uint nWidth;
        public uint nHeight;
        public uint TextureSize; // Unused
        public NinjaSurfaceFlags fSurfaceFlags;
        public PAKInfEntry()
        {
            filename = new byte[28];
        }
        public PAKInfEntry(byte[] data)
        {
            filename = new byte[28];
            Array.Copy(data, filename, 0x1C);
            globalindex = BitConverter.ToUInt32(data, 0x1C);
            Type = (GvrDataFormat)BitConverter.ToUInt32(data, 0x20);
            BitDepth = BitConverter.ToUInt32(data, 0x24);
            PixelFormat = (GvrDataFormat)BitConverter.ToUInt32(data, 0x28);
            nWidth = BitConverter.ToUInt32(data, 0x2C);
            nHeight = BitConverter.ToUInt32(data, 0x30);
            TextureSize = BitConverter.ToUInt32(data, 0x34);
            fSurfaceFlags = (NinjaSurfaceFlags)BitConverter.ToUInt32(data, 0x38);
        }
        public string GetFilename()
        {
            StringBuilder sb = new StringBuilder(0x1C);
            for (int j = 0; j < 0x1C; j++)
                if (filename[j] != 0)
                    sb.Append((char)filename[j]);
                else
                    break;
            return sb.ToString();
        }
        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(filename);
            result.AddRange(BitConverter.GetBytes(globalindex));
            result.AddRange(BitConverter.GetBytes((uint)Type));
            result.AddRange(BitConverter.GetBytes(BitDepth));
            result.AddRange(BitConverter.GetBytes((uint)PixelFormat));
            result.AddRange(BitConverter.GetBytes(nWidth));
            result.AddRange(BitConverter.GetBytes(nHeight));
            result.AddRange(BitConverter.GetBytes(TextureSize));
            result.AddRange(BitConverter.GetBytes((uint)fSurfaceFlags));
            return result.ToArray();
        }
    };

    class PakTextureInfo : TextureInfo
    {
        public GvrDataFormat DataFormat { get; set; }
        public NinjaSurfaceFlags SurfaceFlags { get; set; }
        public PakTextureInfo() { }
        public string GetSurfaceFlags()
        {
            List<string> flags = new List<string>();
            if ((SurfaceFlags & NinjaSurfaceFlags.NotTwiddled) != 0)
                flags.Add("Not Twiddled");
            else
                flags.Add("Twiddled");
            if ((SurfaceFlags & NinjaSurfaceFlags.Mipmapped) != 0)
                flags.Add("Mipmapped");
            if ((SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
                flags.Add("Palettized");
            if ((SurfaceFlags & NinjaSurfaceFlags.Stride) != 0)
                flags.Add("Stride");
            if ((SurfaceFlags & NinjaSurfaceFlags.VQ) != 0)
                flags.Add("VQ");
            return string.Join(", ", flags);
        }
        public PakTextureInfo(TextureInfo tex)
        {
            Name = tex.Name;
            GlobalIndex = tex.GlobalIndex;
            if (tex is GvrTextureInfo gvrt)
            {
                DataFormat = gvrt.DataFormat;
                if (gvrt.DataFormat == GvrDataFormat.Index4 || gvrt.DataFormat == GvrDataFormat.Index8)
                    SurfaceFlags |= NinjaSurfaceFlags.Palettized;
            }
            else
                DataFormat = GvrDataFormat.Dxt1;
            Image = tex.Image;
            Mipmap = tex.Mipmap;
            if (tex.Mipmap)
                SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
        }

        public PakTextureInfo(string name, uint gbix, Bitmap bitmap, GvrDataFormat format = GvrDataFormat.Dxt1, NinjaSurfaceFlags flags = NinjaSurfaceFlags.Mipmapped)
        {
            Name = name;
            GlobalIndex = gbix;
            Image = bitmap;
            DataFormat = format;
            SurfaceFlags = flags;
            Mipmap = (SurfaceFlags & NinjaSurfaceFlags.Mipmapped) != 0;
        }

        public override bool CheckMipmap()
        {
            return true;
        }
    }

    enum TextureFormat { PVM, GVM, PVMX, PAK }
}